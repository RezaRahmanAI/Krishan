using AutoMapper;
using IMSWEB.Model;
using IMSWEB.Service;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using System.Data;
using IMSWEB.Report;
using IMSWEB.ViewModels;
using log4net;

namespace IMSWEB.Controllers
{
    public class ReturnController : CoreController
    {
        IROrderService _salesOrderService;
        IROrderDetailService _salesOrderDetailService;
        IStockService _stockService;
        IStockDetailService _stockDetailService;
        ICustomerService _customerService;
        IEmployeeService _employeeService;
        ITransactionalReport _transactionalReportService;
        IMiscellaneousService<ROrder> _miscellaneousService;
        IProductService _productService;
        IMapper _mapper;

        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public ReturnController(IErrorService errorService,
            IROrderService salesOrderService, IROrderDetailService salesOrderDetailService,
            IStockService stockService, IStockDetailService stockDetailService,
            ICustomerService customerService, IEmployeeService employeeService,
            ITransactionalReport transactionalReportService,
            IMiscellaneousService<ROrder> miscellaneousService, IMapper mapper,
            IProductService productService)
            : base(errorService)
        {
            _salesOrderService = salesOrderService;
            _salesOrderDetailService = salesOrderDetailService;
            _stockService = stockService;
            _stockDetailService = stockDetailService;
            _customerService = customerService;
            _employeeService = employeeService;
            _transactionalReportService = transactionalReportService;
            _miscellaneousService = miscellaneousService;
            _productService = productService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult> Index()
        {
            TempData["salesOrderViewModel"] = null;
            var repOrders = _salesOrderService.GetReturnOrdersByAsync();
            var vmSO = _mapper.Map<IEnumerable<Tuple<int, string, DateTime, string, string, decimal, EnumSalesType>>,
            IEnumerable<GetSalesOrderViewModel>>(await repOrders);
            return View(vmSO);
        }

        [HttpGet]
        [Authorize]
        [Route("create")]
        public ActionResult Create()
        {
            return ReturnCreateViewWithTempData();
        }

        [HttpPost]
        [Authorize]
        [Route("create/returnUrl")]
        public ActionResult Create(SalesOrderViewModel newReplacementOrder, FormCollection formCollection, string returnUrl)
        {
            return HandleSalesOrder(newReplacementOrder, formCollection);
        }


        private ActionResult ReturnCreateViewWithTempData()
        {
            SalesOrderViewModel salesOrder = (SalesOrderViewModel)TempData.Peek("salesOrderViewModel");
            if (salesOrder != null)
            {
                //tempdata getting null after redirection, so we're restoring salesOrder 
                TempData["salesOrderViewModel"] = salesOrder;
                return View("Create", salesOrder);
            }
            else
            {
                string invNo = _miscellaneousService.GetUniqueKey(x => int.Parse(x.InvoiceNo));
                return View(new SalesOrderViewModel
                {
                    SODetail = new CreateSalesOrderDetailViewModel(),
                    SODetails = new List<CreateSalesOrderDetailViewModel>(),
                    SalesOrder = new CreateSalesOrderViewModel { InvoiceNo = invNo }
                });
            }
        }

        private ActionResult HandleSalesOrder(SalesOrderViewModel newReplacementOrder, FormCollection formCollection)
        {
            Tuple<bool, int> Result = new Tuple<bool,int>(false,0);

            if (newReplacementOrder != null)
            {
                SalesOrderViewModel salesOrder = (SalesOrderViewModel)TempData.Peek("salesOrderViewModel");
                salesOrder = salesOrder ?? new SalesOrderViewModel()
                {
                    SalesOrder = newReplacementOrder.SalesOrder
                };
                salesOrder.SODetail = new CreateSalesOrderDetailViewModel();

                if (formCollection.Get("addButton") != null)
                {
                    CheckAndAddModelErrorForAdd(newReplacementOrder, salesOrder, formCollection);
                    if (!ModelState.IsValid)
                    {
                        salesOrder.SODetails = salesOrder.SODetails ?? new List<CreateSalesOrderDetailViewModel>();
                        return View("Create", salesOrder);
                    }
                    if (salesOrder.SODetails != null &&
                        salesOrder.SODetails.Any(x => x.Status != EnumStatus.Updated && x.Status != EnumStatus.Deleted &&
                       (x.ProductId.Equals(newReplacementOrder.SODetail.ProductId) && x.ColorId.Equals(newReplacementOrder.SODetail.ColorId))))
                    {
                        AddToastMessage(string.Empty, "This product already exists in the order", ToastType.Error);
                        return View("Create", salesOrder);
                    }
                    AddToOrder(newReplacementOrder, salesOrder, formCollection);
                    ModelState.Clear();
                    return View("Create", salesOrder);
                }
                else if (formCollection.Get("btnReturn") != null)
                {
                    CheckAndAddModelErrorForSave(newReplacementOrder, salesOrder, formCollection);
                    if (!ModelState.IsValid)
                    {
                        salesOrder.SODetails = salesOrder.SODetails ?? new List<CreateSalesOrderDetailViewModel>();
                        return View("Create", salesOrder);
                    }
                    Result = SaveOrder(newReplacementOrder, salesOrder, formCollection);
                    ModelState.Clear();

                    //mapping for sales ivoice
                    //var invoiceSalesOrder = _mapper.Map<CreateSalesOrderViewModel, SOrder>(salesOrder.SalesOrder);
                    //invoiceSalesOrder.SOrderDetails = _mapper.Map<ICollection<CreateSalesOrderDetailViewModel>,
                    //    ICollection<ReplaceOrderDetail>>(salesOrder.SODetails);
                    if (Result.Item1)
                    {

                        var invoiceSalesOrder = _mapper.Map<CreateSalesOrderViewModel, ReplaceOrder>(salesOrder.SalesOrder);

                        var invoiceSalesOrderdetails = _mapper.Map<ICollection<CreateSalesOrderDetailViewModel>,
                                ICollection<ReplaceOrderDetail>>(salesOrder.SODetails);

                        TempData["IsInvoiceReadyById"] = true;
                        TempData["OrderId"] = Result.Item2;
                    }

                    return RedirectToAction("Index");
                }
                else
                {
                    return View("Create", new PurchaseOrderViewModel
                    {
                        PODetail = new CreatePurchaseOrderDetailViewModel(),
                        PODetails = new List<CreatePurchaseOrderDetailViewModel>(),
                        PurchaseOrder = new CreatePurchaseOrderViewModel()
                    });
                }
            }
            else
            {
                AddToastMessage("", "No order data found to save.", ToastType.Error);
                return RedirectToAction("Create");
            }
        }

        private void CheckAndAddModelErrorForAdd(SalesOrderViewModel newReturnOrder,
    SalesOrderViewModel returnOrder, FormCollection formCollection)
        {
            if (string.IsNullOrEmpty(formCollection["OrderDate"]))
                ModelState.AddModelError("SalesOrder.OrderDate", "Sales Date is required");

            if (string.IsNullOrEmpty(formCollection["CustomersId"]))
                ModelState.AddModelError("SalesOrder.CustomerId", "Customer is required");
            else
            {
                returnOrder.SalesOrder.CustomerId = formCollection["CustomersId"];
            }

            if (string.IsNullOrEmpty(newReturnOrder.SODetail.ColorId))
                ModelState.AddModelError("SODetail.ColorId", "Color is required");

            //ProductDetailsId is ProductId
            if (string.IsNullOrEmpty(formCollection["ProductDetailsId"]))
                ModelState.AddModelError("SODetail.ProductId", "Product is required");
            else
            {
                newReturnOrder.SODetail.ProductId = formCollection["ProductDetailsId"];

                if (returnOrder.SODetails != null)
                {
                    if (returnOrder.SODetails.Any(i => i.ProductId.Equals(newReturnOrder.SODetail.ProductId) && i.ColorId.Equals(newReturnOrder.SODetail.ColorId)))
                    {
                        ModelState.AddModelError("SODetail.ProductId", "This product is already added.");
                    }
                }
            }

            if (Convert.ToDouble(formCollection["dQuantity"]) < Convert.ToDouble(newReturnOrder.SODetail.Quantity))
            {
                ModelState.AddModelError("SODetail.Quantity", "Quantity Exceeds Sales Quantity");
            }

            if (string.IsNullOrEmpty(newReturnOrder.SalesOrder.InvoiceNo))
                ModelState.AddModelError("SalesOrder.InvoiceNo", "Invoice No. is required");

            if (string.IsNullOrEmpty(newReturnOrder.SODetail.UnitPrice))
                ModelState.AddModelError("SODetail.UnitPrice", "Sales Rate is required");

            //if (string.IsNullOrEmpty(newReturnOrder.SODetail.ProductId))
            //{
            //    ModelState.AddModelError("SODetail.IMENo", "IMENo/Barcode is required");
            //}
            //else
            //{


            //}
            //else
            //{
            //    var stockDetails = _stockDetailService.GetStockDetailByProductId(
            //        int.Parse(GetDefaultIfNull(formCollection["ProductDetailsId"])));

            //    if (!stockDetails.Any(x => x.IMENO.Equals(newReplacementOrder.SODetail.DamageIMEINO)))
            //        ModelState.AddModelError("SODetail.IMENo", "Invalid IMENo/Barcode");
            //}
        }

        private void CheckAndAddModelErrorForSave(SalesOrderViewModel newReplacementOrder, SalesOrderViewModel salesOrder, FormCollection formCollection)
        {

            newReplacementOrder.SalesOrder.CurrentDue = "0";
            newReplacementOrder.SalesOrder.NetDiscount = "0";
            newReplacementOrder.SalesOrder.PPDiscountAmount = "0";
            newReplacementOrder.SalesOrder.TotalDiscountPercentage = "0";
            newReplacementOrder.SalesOrder.VATAmount = "0";
            newReplacementOrder.SalesOrder.VATPercentage = "0";
            newReplacementOrder.SalesOrder.TotalOffer = "0";

            //if (string.IsNullOrEmpty(newSalesOrder.SalesOrder.RecieveAmount) ||
            //    decimal.Parse(GetDefaultIfNull(newSalesOrder.SalesOrder.RecieveAmount)) <= 0)
            //    ModelState.AddModelError("SalesOrder.RecieveAmount", "Pay Amount is required");
            if (newReplacementOrder.SalesOrder.RecieveAmount == null || newReplacementOrder.SalesOrder.RecieveAmount == "")
            {
                newReplacementOrder.SalesOrder.RecieveAmount = "0";
                salesOrder.SalesOrder.RecieveAmount = "0";
            }

            Customer customer = _customerService.GetCustomerById(int.Parse(salesOrder.SalesOrder.CustomerId));
            Employee employee = _employeeService.GetEmployeeById(customer.EmployeeID);

            //if (decimal.Parse(GetDefaultIfNull(newReplacementOrder.SalesOrder.PaymentDue)) > customer.CusDueLimit)
            //    ModelState.AddModelError("SalesOrder.PaymentDue", "Customer due limit is exceeding");

            //if (decimal.Parse(GetDefaultIfNull(newReplacementOrder.SalesOrder.PaymentDue)) > employee.SRDueLimit)
            //    ModelState.AddModelError("SalesOrder.PaymentDue", "SR due limit is exceeding");

            if (!IsDateValid(Convert.ToDateTime(formCollection["OrderDate"])))
                ModelState.AddModelError("SalesOrder.OrderDate", "Back dated entry is not valid");
        }

        private void AddToOrder(SalesOrderViewModel newReturnOrder,
            SalesOrderViewModel returnOrder, FormCollection formCollection)
        {

            #region Parent Order
            returnOrder.SalesOrder.DamageTotalAmount = (decimal.Parse(GetDefaultIfNull(returnOrder.SalesOrder.DamageTotalAmount)) +
               decimal.Parse(GetDefaultIfNull(newReturnOrder.SODetail.UnitPrice)) * decimal.Parse(GetDefaultIfNull(newReturnOrder.SODetail.Quantity))).ToString("0.00");

            //replacementOrder.SalesOrder.ReplaceTotalAmount = (decimal.Parse(GetDefaultIfNull(replacementOrder.SalesOrder.ReplaceTotalAmount)) +
            //    decimal.Parse(GetDefaultIfNull(newReplacementOrder.SODetail.UnitPrice))).ToString();

            returnOrder.SalesOrder.GrandTotal = (decimal.Parse(returnOrder.SalesOrder.DamageTotalAmount)).ToString();

            //replacementOrder.SalesOrder.PPDiscountAmount = (decimal.Parse(GetDefaultIfNull(newReplacementOrder.SalesOrder.PPDiscountAmount)) +
            //    decimal.Parse(GetDefaultIfNull(newReplacementOrder.SODetail.PPDAmount))).ToString();

            //replacementOrder.SalesOrder.TotalDiscountPercentage = decimal.Parse(GetDefaultIfNull(newReplacementOrder.SalesOrder.TotalDiscountPercentage)).ToString();
            //replacementOrder.SalesOrder.TotalDiscountAmount = decimal.Parse(GetDefaultIfNull(newReplacementOrder.SalesOrder.TotalDiscountAmount)).ToString();

            //replacementOrder.SalesOrder.VATPercentage = decimal.Parse(GetDefaultIfNull(newReplacementOrder.SalesOrder.VATPercentage)).ToString();
            //replacementOrder.SalesOrder.VATAmount = decimal.Parse(GetDefaultIfNull(newReplacementOrder.SalesOrder.VATAmount)).ToString();

            returnOrder.SalesOrder.AdjAmount = decimal.Parse(GetDefaultIfNull(newReturnOrder.SalesOrder.AdjAmount)).ToString();

            //replacementOrder.SalesOrder.NetDiscount = (decimal.Parse(GetDefaultIfNull(replacementOrder.SalesOrder.NetDiscount)) + decimal.Parse(GetDefaultIfNull(newReplacementOrder.SODetail.PPDAmount)) +
            //    decimal.Parse(GetDefaultIfNull(newReplacementOrder.SODetail.PPOffer))).ToString();

            var netTotal = ((decimal.Parse(GetDefaultIfNull(returnOrder.SalesOrder.GrandTotal)) +
                decimal.Parse(GetDefaultIfNull(returnOrder.SalesOrder.AdjAmount))));

            // For Total Offer Purpose
            //replacementOrder.SalesOrder.TotalOffer = (decimal.Parse(GetDefaultIfNull(newReplacementOrder.SalesOrder.TotalOffer)) +
            //decimal.Parse(GetDefaultIfNull(newReplacementOrder.SODetail.PPOffer))).ToString();

            returnOrder.SalesOrder.TotalAmount = netTotal.ToString();
            returnOrder.SalesOrder.PaymentDue = (netTotal - decimal.Parse(GetDefaultIfNull(newReturnOrder.SalesOrder.RecieveAmount))).ToString();
            returnOrder.SalesOrder.RecieveAmount = GetDefaultIfNull(newReturnOrder.SalesOrder.RecieveAmount);

            returnOrder.SalesOrder.OrderDate = formCollection["OrderDate"];
            returnOrder.SalesOrder.CustomerId = formCollection["CustomersId"];
            #endregion

            returnOrder.SODetail.SODetailId = newReturnOrder.SODetail.SODetailId;
            returnOrder.SODetail.ProductId = formCollection["ProductDetailsId"];
            returnOrder.SODetail.ColorId = newReturnOrder.SODetail.ColorId;
            returnOrder.SODetail.ColorName = newReturnOrder.SODetail.ColorName;
            returnOrder.SODetail.StockDetailId = newReturnOrder.SODetail.StockDetailId;//damage
            //replacementOrder.SODetail.RStockDetailId = formCollection["StockDetailsId"];//replace 
            returnOrder.SODetail.ProductCode = formCollection["ProductDetailsCode"];
            returnOrder.SODetail.DamageIMEINO = newReturnOrder.SODetail.DamageIMEINO;
            //replacementOrder.SODetail.ReplaceIMEINO = newReplacementOrder.SODetail.IMENo;
            returnOrder.SODetail.Quantity = newReturnOrder.SODetail.Quantity;
            //replacementOrder.SODetail.PPDPercentage = newReplacementOrder.SODetail.PPDPercentage;
            //replacementOrder.SODetail.PPDAmount = newReplacementOrder.SODetail.PPDAmount;
            returnOrder.SODetail.DamageUnitPrice = newReturnOrder.SODetail.UnitPrice;// formCollection["dUnitPrice"];
            returnOrder.SODetail.UnitPrice = returnOrder.SODetail.DamageUnitPrice;
            returnOrder.SODetail.MRPRate = newReturnOrder.SODetail.MRPRate;
            returnOrder.SODetail.UTAmount = (Convert.ToDouble(returnOrder.SODetail.DamageUnitPrice) * Convert.ToDouble(returnOrder.SODetail.Quantity)).ToString("0.00"); //newReplacementOrder.SODetail.UTAmount;
            returnOrder.SODetail.ProductName = formCollection["ProductDetailsName"];
            returnOrder.SODetail.DamageProductName = formCollection["ProductDetailsName"];
            returnOrder.SODetail.Status = newReturnOrder.SODetail.Status == default(int) ? EnumStatus.New : newReturnOrder.SODetail.Status;
            //replacementOrder.SODetail.PPOffer = newReplacementOrder.SODetail.PPOffer;
            returnOrder.SODetail.RatePerArea = newReturnOrder.SODetail.RatePerArea;
            returnOrder.SODetail.TotalArea = newReturnOrder.SODetail.TotalArea;

            returnOrder.SODetails = returnOrder.SODetails ?? new List<CreateSalesOrderDetailViewModel>();
            returnOrder.SODetails.Add(returnOrder.SODetail);

            SalesOrderViewModel vm = new SalesOrderViewModel
            {
                SODetail = new CreateSalesOrderDetailViewModel(),
                SODetails = returnOrder.SODetails,
                SalesOrder = returnOrder.SalesOrder
            };

            TempData["salesOrderViewModel"] = vm;
            returnOrder.SODetail = new CreateSalesOrderDetailViewModel();
            AddToastMessage("", "Order has been added successfully.", ToastType.Success);
        }

        private Tuple<bool, int> SaveOrder(SalesOrderViewModel newReplacementOrder,
            SalesOrderViewModel replacementOrder, FormCollection formCollection)
        {

            replacementOrder.SalesOrder.NetDiscount = GetDefaultIfNull(newReplacementOrder.SalesOrder.NetDiscount);
            replacementOrder.SalesOrder.TotalAmount = decimal.Parse(GetDefaultIfNull(newReplacementOrder.SalesOrder.TotalAmount)).ToString();
            replacementOrder.SalesOrder.PaymentDue = decimal.Parse(GetDefaultIfNull(newReplacementOrder.SalesOrder.PaymentDue)).ToString();

            replacementOrder.SalesOrder.TotalDiscountPercentage = newReplacementOrder.SalesOrder.TotalDiscountPercentage;
            replacementOrder.SalesOrder.TotalDiscountAmount = newReplacementOrder.SalesOrder.TotalDiscountAmount;
            replacementOrder.SalesOrder.RecieveAmount = newReplacementOrder.SalesOrder.RecieveAmount;
            replacementOrder.SalesOrder.VATPercentage = newReplacementOrder.SalesOrder.VATPercentage;
            replacementOrder.SalesOrder.VATAmount = newReplacementOrder.SalesOrder.VATAmount;
            replacementOrder.SalesOrder.AdjAmount = newReplacementOrder.SalesOrder.AdjAmount;
            replacementOrder.SalesOrder.Remarks = newReplacementOrder.SalesOrder.Remarks;

            replacementOrder.SalesOrder.OrderDate = formCollection["OrderDate"];
            replacementOrder.SalesOrder.CustomerId = formCollection["CustomersId"];

            //removing unchanged previous order
            replacementOrder.SODetails.Where(x => !string.IsNullOrEmpty(x.SODetailId) && x.Status == default(int)).ToList()
                .ForEach(x => replacementOrder.SODetails.Remove(x));

            DataTable dtSalesOrder = CreateSalesOrderDataTable(replacementOrder);
            DataTable dtSalesOrderDetail = CreateSODetailDataTable(replacementOrder);

            log.Info(new { ReturnOrder = replacementOrder.SalesOrder, ReturnOrderDetails = replacementOrder.SODetails });
            //if (ControllerContext.RouteData.Values["action"].ToString().ToLower().Equals("edit"))
            //    _salesOrderService.UpdateSalesOrderUsingSP(User.Identity.GetUserId<int>(), int.Parse(replacementOrder.SalesOrder.SalesOrderId),
            //        dtSalesOrder, dtSalesOrderDetail);
            //else
            var Result = _salesOrderService.AddReturnOrderUsingSP(dtSalesOrder, dtSalesOrderDetail);
            if (Result.Item1)
                AddToastMessage("", "Order has been saved successfully.", ToastType.Success);
            else
                AddToastMessage("", "Order has been Failed.", ToastType.Error);

            //  _salesOrderService.CorrectionStockData(User.Identity.GetConcernId());

            return Result;

        }


        private DataTable CreateSalesOrderDataTable(SalesOrderViewModel salesOrder)
        {
            DataTable dtSalesOrder = new DataTable();
            dtSalesOrder.Columns.Add("InvoiceDate", typeof(DateTime));
            dtSalesOrder.Columns.Add("InvoiceNo", typeof(string));
            dtSalesOrder.Columns.Add("VatPercentage", typeof(decimal));
            dtSalesOrder.Columns.Add("VatAmount", typeof(decimal));
            dtSalesOrder.Columns.Add("GrandTotal", typeof(decimal));
            dtSalesOrder.Columns.Add("TDiscountPercentage", typeof(decimal));
            dtSalesOrder.Columns.Add("TDiscountAmount", typeof(decimal));
            dtSalesOrder.Columns.Add("RecAmt", typeof(decimal));
            dtSalesOrder.Columns.Add("PaymentDue", typeof(decimal));
            dtSalesOrder.Columns.Add("TotalAmount", typeof(decimal));
            dtSalesOrder.Columns.Add("TotalDue", typeof(decimal));
            dtSalesOrder.Columns.Add("AdjAmount", typeof(decimal));
            dtSalesOrder.Columns.Add("Status", typeof(int));
            dtSalesOrder.Columns.Add("CustomerId", typeof(int));
            dtSalesOrder.Columns.Add("ConcernId", typeof(int));
            dtSalesOrder.Columns.Add("CreatedBy", typeof(int));
            dtSalesOrder.Columns.Add("CreatedDate", typeof(DateTime));
            dtSalesOrder.Columns.Add("TotalOffer", typeof(decimal));
            dtSalesOrder.Columns.Add("NetDiscount", typeof(decimal));
            dtSalesOrder.Columns.Add("Remarks", typeof(string));

            DataRow row = null;

            row = dtSalesOrder.NewRow();
            row["InvoiceDate"] = salesOrder.SalesOrder.OrderDate;
            row["InvoiceNo"] = salesOrder.SalesOrder.InvoiceNo;
            row["VatPercentage"] = GetDefaultIfNull(salesOrder.SalesOrder.VATPercentage);
            row["VatAmount"] = GetDefaultIfNull(salesOrder.SalesOrder.VATAmount);
            row["GrandTotal"] = GetDefaultIfNull(salesOrder.SalesOrder.GrandTotal);
            row["TDiscountPercentage"] = GetDefaultIfNull(salesOrder.SalesOrder.TotalDiscountPercentage);
            row["TDiscountAmount"] = GetDefaultIfNull(salesOrder.SalesOrder.TotalDiscountAmount);
            row["PaymentDue"] = GetDefaultIfNull(salesOrder.SalesOrder.PaymentDue);
            row["RecAmt"] = GetDefaultIfNull(salesOrder.SalesOrder.RecieveAmount);
            row["TotalAmount"] = GetDefaultIfNull(salesOrder.SalesOrder.TotalAmount);
            row["TotalDue"] = (decimal.Parse(GetDefaultIfNull(salesOrder.SalesOrder.PaymentDue)) - decimal.Parse(GetDefaultIfNull(salesOrder.SalesOrder.TotalDue)));
            row["AdjAmount"] = GetDefaultIfNull(salesOrder.SalesOrder.AdjAmount);
            row["Status"] = EnumSalesType.ProductReturn;
            row["CustomerId"] = salesOrder.SalesOrder.CustomerId;
            row["ConcernId"] = User.Identity.GetConcernId();
            row["CreatedDate"] = DateTime.Now;
            row["CreatedBy"] = User.Identity.GetUserId<int>();
            row["TotalOffer"] = GetDefaultIfNull(salesOrder.SalesOrder.TotalOffer);
            row["NetDiscount"] = GetDefaultIfNull(salesOrder.SalesOrder.NetDiscount);
            row["Remarks"] = salesOrder.SalesOrder.Remarks;

            dtSalesOrder.Rows.Add(row);

            return dtSalesOrder;
        }

        private DataTable CreateSODetailDataTable(SalesOrderViewModel salesOrder)
        {
            DataTable dtSalesOrderDetail = new DataTable();
            dtSalesOrderDetail.Columns.Add("SOrderDetailID", typeof(int));
            dtSalesOrderDetail.Columns.Add("ProductId", typeof(int));
            dtSalesOrderDetail.Columns.Add("StockDetailId", typeof(int));
            //dtSalesOrderDetail.Columns.Add("RStockDetailId", typeof(int));
            dtSalesOrderDetail.Columns.Add("ColorId", typeof(int));
            dtSalesOrderDetail.Columns.Add("Status", typeof(int));
            dtSalesOrderDetail.Columns.Add("Quantity", typeof(decimal));
            dtSalesOrderDetail.Columns.Add("UnitPrice", typeof(decimal));
            dtSalesOrderDetail.Columns.Add("TAmount", typeof(decimal));
            dtSalesOrderDetail.Columns.Add("PPDisPer", typeof(decimal));
            dtSalesOrderDetail.Columns.Add("PPDisAmt", typeof(decimal));
            dtSalesOrderDetail.Columns.Add("MrpRate", typeof(decimal));
            dtSalesOrderDetail.Columns.Add("PPOffer", typeof(decimal));
            dtSalesOrderDetail.Columns.Add("SFTRate", typeof(decimal));
            //dtSalesOrderDetail.Columns.Add("RepOrderID", typeof(int));
            //dtSalesOrderDetail.Columns.Add("RepUnitPrice", typeof(decimal));

            DataRow row = null;

            foreach (var item in salesOrder.SODetails)
            {
                row = dtSalesOrderDetail.NewRow();
                if (!string.IsNullOrEmpty(item.SODetailId))
                    row["SOrderDetailID"] = item.SODetailId;
                row["ProductId"] = item.ProductId;
                row["StockDetailId"] = item.StockDetailId;
                //row["RStockDetailId"] = GetDefaultIfNull(item.RStockDetailId);
                row["ColorId"] = item.ColorId;
                row["Status"] = item.Status;
                row["Quantity"] = item.Quantity;
                row["UnitPrice"] = item.UnitPrice;

                row["TAmount"] = GetDefaultIfNull(item.UTAmount);
                row["PPDisPer"] = GetDefaultIfNull(item.PPDPercentage);
                row["PPDisAmt"] = GetDefaultIfNull(item.PPDAmount);
                row["MrpRate"] = item.MRPRate;
                row["PPOffer"] = GetDefaultIfNull(item.PPOffer);
                row["SFTRate"] = item.RatePerArea;

                //row["RepOrderID"] = item.RepOrderID;
                //row["RepUnitPrice"] = item.UnitPrice;
                dtSalesOrderDetail.Rows.Add(row);
            }

            return dtSalesOrderDetail;
        }

        [HttpGet]
        [Authorize]
        public JsonResult GetProductDetailByIMEINo(string imeiNo)
        {
            var customProductDetails = _productService.GetAllProductFromDetail();
            var vmProductDetails = _mapper.Map<IEnumerable<Tuple<int, string, string,
            decimal, string, string, string, Tuple<decimal?, string, decimal, int, int, string, string,
            Tuple<string, string, string, string, string, string, int, Tuple<int, decimal, decimal, decimal, decimal, int>>>>>, IEnumerable<GetProductViewModel>>(customProductDetails);

            if (!string.IsNullOrEmpty(imeiNo))
            {
                var vmProduct = vmProductDetails.FirstOrDefault(x => x.IMENo.ToLower().Equals(imeiNo.ToLower()));
                if (vmProduct != null)
                {
                    return Json(new
                    {
                        Code = vmProduct.ProductCode,
                        Name = vmProduct.ProductName,
                        Id = vmProduct.ProductId,
                        StockDetailId = vmProduct.StockDetailsId,
                        ColorId = vmProduct.ColorId,
                        ColorName = vmProduct.ColorName,
                        MrpRate = vmProduct.MRPRate,
                        IMEINo = vmProduct.IMENo,
                        OfferDescription = vmProduct.OfferDescription,
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(null, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        [Authorize]
        public JsonResult GetDamageProductDetailByIMEINo(string imeiNo, int CustomerID)
        {


            if (!string.IsNullOrEmpty(imeiNo))
            {
                var customProductDetails = _productService.GetAllSalesProductByCustomerID(CustomerID);

                var vmProduct = customProductDetails.FirstOrDefault(x => x.IMENO.ToLower().Equals(imeiNo.ToLower()));
                if (vmProduct != null)
                {
                    return Json(new
                    {
                        Code = vmProduct.ProductCode,
                        Name = vmProduct.ProductName,
                        Id = vmProduct.ProductID,
                        StockDetailId = vmProduct.SDetailID,
                        ColorId = vmProduct.ColorID,
                        ColorName = vmProduct.ColorName,
                        MrpRate = vmProduct.MRP,
                        IMEINo = vmProduct.IMENO,
                        OfferDescription = vmProduct.OfferDescription,
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(false, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpGet]
        [Authorize]
        public JsonResult GetSalesProductDetailByCustomerID(int CustomerID)
        {
            var customProductDetails = _productService.GetSalesDetailByCustomerIDNew(CustomerID, string.Empty);

            if (customProductDetails != null)
            {
                return Json(customProductDetails, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpGet]
        [Authorize]
        public ActionResult Invoice(int orderId)
        {
            TempData["IsInvoiceReadyById"] = true;
            TempData["OrderId"] = orderId;
            return RedirectToAction("Index");
        }

        [HttpGet]
        [Authorize]
        public ActionResult Challan(int orderId)
        {
            TempData["IsChallanReadyById"] = true;
            TempData["ROrderId"] = orderId;
            return RedirectToAction("Index");
        }

        [HttpGet]
        [Authorize]
        public ActionResult ReturnReport()
        {
            return View();
        }
    }
}