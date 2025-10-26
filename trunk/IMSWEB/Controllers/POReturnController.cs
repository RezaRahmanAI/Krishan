using AutoMapper;
using IMSWEB.Model;
using IMSWEB.Service;
using log4net;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using System.Threading.Tasks;
namespace IMSWEB.Controllers
{
    public class POReturnController : CoreController
    {
        IPurchaseOrderService _purchaseOrderService;
        IPurchaseOrderDetailService _purchaseOrderDetailService;
        IPOProductDetailService _pOProductDetailService;
        IStockService _stockService;
        IStockDetailService _stockDetailService;
        IMiscellaneousService<POrder> _miscellaneousService;
        IMapper _mapper;
        ISupplierService _supplierService;
        IColorService _colorService;
        IProductService _productService;
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public POReturnController(IErrorService errorService,
            IPurchaseOrderService purchaseOrderService, IPurchaseOrderDetailService purchaseOrderDetailService,
            IPOProductDetailService pOProductDetailService, IStockService stockService, ISupplierService supplierService,
            IStockDetailService stockDetailService, IMiscellaneousService<POrder> miscellaneousService, IMapper mapper,
            IColorService colorService, IProductService productService
            )
            : base(errorService)
        {
            _purchaseOrderService = purchaseOrderService;
            _purchaseOrderDetailService = purchaseOrderDetailService;
            _pOProductDetailService = pOProductDetailService;
            _stockService = stockService;
            _stockDetailService = stockDetailService;
            _miscellaneousService = miscellaneousService;
            _mapper = mapper;
            _supplierService = supplierService;
            _colorService = colorService;
            _productService = productService;
        }

        public async Task<ActionResult> Index()
        {
            TempData["purchaseOrderViewModel"] = null;
            var customPO = _purchaseOrderService.GetAllReturnPurchaseOrderAsync();
            var vmPO = _mapper.Map<IEnumerable<Tuple<int, string, DateTime, string, string, string, EnumPurchaseType>>,
                IEnumerable<GetPurchaseOrderViewModel>>(await customPO);
            return View(vmPO);
        }


        public ActionResult Create()
        {
            return ReturnCreateViewWithTempData();
        }


        private ActionResult ReturnCreateViewWithTempData()
        {
            PurchaseReturnOrderViewModel purchaseOrder = (PurchaseReturnOrderViewModel)TempData.Peek("purchaseOrderViewModel");
            if (purchaseOrder != null)
            {
                //tempdata getting null after redirection, so we're restoring purchaseOrder 
                TempData["purchaseOrderViewModel"] = purchaseOrder;
                return View("Create", purchaseOrder);
            }
            else
            {
                string chllnNo ="RC-"+ _miscellaneousService.GetUniqueKey(x => x.POrderID);
                var color = _colorService.GetAllColor().FirstOrDefault();
                return View(new PurchaseReturnOrderViewModel
                {
                    POProductDetails = new CreatePOProductDetailViewModel(),
                    PurchaseOrder = new CreatePurchaseOrderViewModel { ChallanNo = chllnNo }
                });
            }
        }

        [HttpPost]
        public ActionResult Create(PurchaseReturnOrderViewModel newPurchaseOrder, FormCollection formCollection)
        {
            return HandlePurchaseOrder(newPurchaseOrder, formCollection);
        }
        private void CheckAndAddModelErrorForAdd(PurchaseReturnOrderViewModel newPurchaseOrder, FormCollection formCollection)
        {


            if (string.IsNullOrEmpty(formCollection["ProductDetailsId"]))
                ModelState.AddModelError("POProductDetails.ProductId", "Product is required");

            if (string.IsNullOrEmpty(formCollection["StockDetailsId"]))
                ModelState.AddModelError("POProductDetails.SDetailID", "IMEI is required");


            //if (string.IsNullOrEmpty(formCollection["ColorsId"]))
            //ModelState.AddModelError("PODetail.ColorId", "Color is required");

            if (string.IsNullOrEmpty(newPurchaseOrder.PurchaseOrder.ChallanNo))
                ModelState.AddModelError("PurchaseOrder.ChallanNo", "Challan No. is required");

        }
        private void GetPickersValue(PurchaseReturnOrderViewModel purchaseOrder, PurchaseReturnOrderViewModel NewpurchaseOrder, FormCollection formCollection)
        {
            if (!string.IsNullOrEmpty(formCollection["SuppliersId"]))
                purchaseOrder.PurchaseOrder.SupplierId = formCollection["SuppliersId"];
            if (!string.IsNullOrEmpty(formCollection["OrderDate"]))
                purchaseOrder.PurchaseOrder.OrderDate = formCollection["OrderDate"];
            if (!string.IsNullOrEmpty(formCollection["StockDetailsId"]))
            {
                NewpurchaseOrder.POProductDetails.SDetailID = Convert.ToInt32(formCollection["StockDetailsId"]);
                purchaseOrder.POProductDetails.SDetailID = NewpurchaseOrder.POProductDetails.SDetailID;
            }
            if (!string.IsNullOrEmpty(formCollection["ProductDetailsId"]))
            {
                NewpurchaseOrder.POProductDetails.ProductId = formCollection["ProductDetailsId"];
                purchaseOrder.POProductDetails.ProductId = NewpurchaseOrder.POProductDetails.ProductId;
            }
            if (!string.IsNullOrEmpty(formCollection["ProductDetailsName"]))
            {
                NewpurchaseOrder.POProductDetails.ProductName = formCollection["ProductDetailsName"];
                purchaseOrder.POProductDetails.ProductName = NewpurchaseOrder.POProductDetails.ProductName;
            }
        }

        /// <summary>
        /// This method check duplicate IMEI in recently added product.
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        private bool HasDuplicateIMEIInRecentAddToOrder(FormCollection formCollection, PurchaseReturnOrderViewModel NewpurchaseOrder, PurchaseReturnOrderViewModel purchaseOrder)
        {
            int ProductType = int.Parse(GetDefaultIfNull(formCollection["ProductsType"]));
            if (ProductType != (int)EnumProductType.NoBarcode)
            {
                if (purchaseOrder.POProductDetailList.Count() > 0)
                {
                    if (purchaseOrder.POProductDetailList.Any(i => i.SDetailID == NewpurchaseOrder.POProductDetails.SDetailID))
                        return true;
                }
            }

            return false;
        }
        private ActionResult HandlePurchaseOrder(PurchaseReturnOrderViewModel newPurchaseOrder, FormCollection formCollection)
        {
            if (newPurchaseOrder != null)
            {
                PurchaseReturnOrderViewModel purchaseOrder = (PurchaseReturnOrderViewModel)TempData.Peek("purchaseOrderViewModel");
                purchaseOrder = purchaseOrder ?? new PurchaseReturnOrderViewModel()
                {
                    PurchaseOrder = newPurchaseOrder.PurchaseOrder
                };
                purchaseOrder.POProductDetails = new CreatePOProductDetailViewModel();

                if (formCollection.Get("addButton") != null)
                {
                    CheckAndAddModelErrorForAdd(newPurchaseOrder, formCollection);
                    purchaseOrder.POProductDetailList = purchaseOrder.POProductDetailList ?? new List<CreatePOProductDetailViewModel>();
                    GetPickersValue(purchaseOrder, newPurchaseOrder, formCollection);
                    if (!ModelState.IsValid)
                    {
                        TempData["POProductDetails"] = purchaseOrder.POProductDetailList;
                        return View("Create", purchaseOrder);
                    }
                    else if (HasDuplicateIMEIInRecentAddToOrder(formCollection, newPurchaseOrder, purchaseOrder))
                    {
                        AddToastMessage("", "Duplicate IMEI/Barcode found", ToastType.Error);
                        return View("Create", purchaseOrder);
                    }
                    AddToOrder(newPurchaseOrder, purchaseOrder, formCollection);
                    ModelState.Clear();
                    return View("Create", purchaseOrder);
                }
                else if (formCollection.Get("submitButton") != null)
                {
                    CheckAndAddModelErrorForSave(newPurchaseOrder, formCollection);

                    if (!ModelState.IsValid)
                    {
                        purchaseOrder.POProductDetailList = purchaseOrder.POProductDetailList ?? new List<CreatePOProductDetailViewModel>();
                        return View("Create", purchaseOrder);
                    }
                    else if (purchaseOrder.POProductDetailList == null || purchaseOrder.POProductDetailList.Count <= 0)
                    {
                        AddToastMessage("", "No order data found to save.", ToastType.Error);
                        return View("Create", purchaseOrder);
                    }
                    SaveOrder(newPurchaseOrder, purchaseOrder, formCollection);
                    TempData["purchaseOrderViewModel"] = null;
                    ModelState.Clear();
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

        private void SaveOrder(PurchaseReturnOrderViewModel newPurchaseOrder, PurchaseReturnOrderViewModel purchaseOrder, FormCollection formCollection)
        {
            List<StockDetail> DeleteStockDetails = new List<StockDetail>();
            var StockDetails = _stockDetailService.GetAll();
            StockDetail objSD = null;
            foreach (var item in purchaseOrder.POProductDetailList)
            {
                objSD = StockDetails.FirstOrDefault(i => i.SDetailID == item.SDetailID);
                DeleteStockDetails.Add(objSD);
            }
            decimal TotalAmount = DeleteStockDetails.Sum(i => i.PRate);
            purchaseOrder.PurchaseOrder.GrandTotal = TotalAmount.ToString();
            purchaseOrder.PurchaseOrder.TotalAmount = TotalAmount.ToString();
            purchaseOrder.PurchaseOrder.RecieveAmount = TotalAmount.ToString();
            purchaseOrder.PurchaseOrder.Status = ((int)EnumPurchaseType.ProductReturn).ToString();
            purchaseOrder.PurchaseOrder.Remarks = "";


            PurchaseOrderViewModel POrder = new PurchaseOrderViewModel() { PurchaseOrder = new CreatePurchaseOrderViewModel(), PODetail = new CreatePurchaseOrderDetailViewModel(), PODetails = new List<CreatePurchaseOrderDetailViewModel>() };


            POrder.PurchaseOrder = purchaseOrder.PurchaseOrder;
            List<CreatePurchaseOrderDetailViewModel> POrderDetails = new List<CreatePurchaseOrderDetailViewModel>();

            POrderDetails = (from st in DeleteStockDetails
                             group st by new { st.ProductID, st.PRate, st.ColorID } into g
                             select new CreatePurchaseOrderDetailViewModel
                             {
                                 ProductId = g.Key.ProductID.ToString(),
                                 ColorId = g.Key.ColorID.ToString(),
                                 UnitPrice = g.Key.PRate.ToString(),
                                 MRPRate = g.Key.PRate.ToString(),
                                 TAmount = g.Sum(i => i.PRate).ToString(),
                                 Quantity = g.Count().ToString(),
                                 SDetailIDList = g.Select(i => i.SDetailID).ToList(),
                                 POProductDetails = new List<POProductDetail>()
                             }).ToList();

            POProductDetail POProductDetail = null;

            foreach (var item in POrderDetails)
            {
                foreach (var sitem in item.SDetailIDList)
                {
                    POProductDetail = new POProductDetail();
                    POProductDetail.ProductID = Convert.ToInt32(item.ProductId);
                    POProductDetail.ColorID = Convert.ToInt32(item.ColorId);
                    POProductDetail.IMENO = DeleteStockDetails.FirstOrDefault(i => i.SDetailID == sitem).IMENO;
                    POProductDetail.DamagePOPDID = sitem;
                    item.POProductDetails.Add(POProductDetail);
                }
            }
            POrder.PODetails = POrderDetails;

            DataTable dtPurchaseOrder = CreatePurchaseOrderDataTable(POrder);
            DataSet dsPurchaseOrderDetail = CreatePODetailDataTable(POrder);
            DataTable dtPurchaseOrderDetail = dsPurchaseOrderDetail.Tables[0];
            DataTable dtPOProductDetail = dsPurchaseOrderDetail.Tables[1];
            bool Result = _purchaseOrderService.AddReturnPurchaseOrderUsingSP(dtPurchaseOrder, dtPurchaseOrderDetail, dtPOProductDetail);

            if (Result)
                AddToastMessage("", "Product Return Successfull.", ToastType.Success);
            else
                AddToastMessage("", "Product Return Failed.", ToastType.Error);

        }
        private DataTable CreatePurchaseOrderDataTable(PurchaseOrderViewModel purchaseOrder)
        {
            DataTable dtPurchaseOrder = new DataTable();
            dtPurchaseOrder.Columns.Add("OrderDate", typeof(DateTime));
            dtPurchaseOrder.Columns.Add("ChallanNo", typeof(string));
            dtPurchaseOrder.Columns.Add("SupplierId", typeof(int));
            dtPurchaseOrder.Columns.Add("GrandTotal", typeof(decimal));
            dtPurchaseOrder.Columns.Add("TDiscount", typeof(decimal));
            dtPurchaseOrder.Columns.Add("TotalAmt", typeof(decimal));
            dtPurchaseOrder.Columns.Add("RecAmt", typeof(decimal));
            dtPurchaseOrder.Columns.Add("PaymentDue", typeof(decimal));
            dtPurchaseOrder.Columns.Add("TotalDue", typeof(decimal));
            dtPurchaseOrder.Columns.Add("AdjAmount", typeof(decimal));
            dtPurchaseOrder.Columns.Add("Status", typeof(int));
            dtPurchaseOrder.Columns.Add("PPTDisAmt", typeof(decimal));
            dtPurchaseOrder.Columns.Add("NetDiscount", typeof(decimal));
            dtPurchaseOrder.Columns.Add("LabourCost", typeof(decimal));
            dtPurchaseOrder.Columns.Add("ConcernId", typeof(int));
            dtPurchaseOrder.Columns.Add("CreatedDate", typeof(DateTime));
            dtPurchaseOrder.Columns.Add("CreatedBy", typeof(int));
            dtPurchaseOrder.Columns.Add("IsDamageOrder", typeof(int));
            dtPurchaseOrder.Columns.Add("Remarks", typeof(string));
            DataRow row = null;

            row = dtPurchaseOrder.NewRow();
            row["OrderDate"] = purchaseOrder.PurchaseOrder.OrderDate;
            row["ChallanNo"] = purchaseOrder.PurchaseOrder.ChallanNo;
            row["SupplierId"] = purchaseOrder.PurchaseOrder.SupplierId;
            row["GrandTotal"] = purchaseOrder.PurchaseOrder.GrandTotal;
            row["TDiscount"] = GetDefaultIfNull(purchaseOrder.PurchaseOrder.TotalDiscountAmount);
            row["TotalAmt"] = purchaseOrder.PurchaseOrder.TotalAmount;
            row["RecAmt"] = GetDefaultIfNull(purchaseOrder.PurchaseOrder.RecieveAmount);
            row["PaymentDue"] = GetDefaultIfNull(purchaseOrder.PurchaseOrder.PaymentDue);
            row["TotalDue"] = GetDefaultIfNull(purchaseOrder.PurchaseOrder.TotalDue); ; //(decimal.Parse(GetDefaultIfNull(purchaseOrder.PurchaseOrder.PaymentDue)) - decimal.Parse(GetDefaultIfNull(purchaseOrder.PurchaseOrder.TotalDue)));
            row["AdjAmount"] = GetDefaultIfNull(purchaseOrder.PurchaseOrder.AdjAmount);
            row["Status"] = purchaseOrder.PurchaseOrder.Status;
            row["PPTDisAmt"] = GetDefaultIfNull(purchaseOrder.PurchaseOrder.PPDiscountAmount);
            row["NetDiscount"] = GetDefaultIfNull(purchaseOrder.PurchaseOrder.NetDiscount);
            row["LabourCost"] = GetDefaultIfNull(purchaseOrder.PurchaseOrder.LabourCost);
            row["ConcernId"] = User.Identity.GetConcernId();
            row["CreatedDate"] = DateTime.Now;
            row["CreatedBy"] = User.Identity.GetUserId<int>();
            row["IsDamageOrder"] = purchaseOrder.PurchaseOrder.IsDamagePO ? 1 : 0;
            row["Remarks"] = "";
            dtPurchaseOrder.Rows.Add(row);

            return dtPurchaseOrder;
        }

        private DataSet CreatePODetailDataTable(PurchaseOrderViewModel purchaseOrder)
        {
            DataSet dsPurchaseOrderDetail = new DataSet();
            DataTable dtPurchaseOrderDetail = new DataTable();
            DataTable dtPOProductDetail = new DataTable();
            DataTable dtStockDetail = new DataTable();
            DataRow row = null;
            int id;

            dtPurchaseOrderDetail.Columns.Add("POrderDetailId", typeof(int));
            dtPurchaseOrderDetail.Columns.Add("ProductId", typeof(int));
            dtPurchaseOrderDetail.Columns.Add("ColorId", typeof(int));
            dtPurchaseOrderDetail.Columns.Add("Status", typeof(int));
            dtPurchaseOrderDetail.Columns.Add("Quantity", typeof(decimal));
            dtPurchaseOrderDetail.Columns.Add("UnitPrice", typeof(decimal));
            dtPurchaseOrderDetail.Columns.Add("TAmount", typeof(decimal));
            dtPurchaseOrderDetail.Columns.Add("PPDisPer", typeof(decimal));
            dtPurchaseOrderDetail.Columns.Add("PPDisAmt", typeof(decimal));
            dtPurchaseOrderDetail.Columns.Add("MrpRate", typeof(decimal));
            dtPurchaseOrderDetail.Columns.Add("SalesRate", typeof(decimal));
            dtPurchaseOrderDetail.Columns.Add("ExtraPPDISPer", typeof(decimal));
            dtPurchaseOrderDetail.Columns.Add("ExtraPPDISAmt", typeof(decimal));
            dtPurchaseOrderDetail.Columns.Add("PPOffer", typeof(decimal));
            dtPurchaseOrderDetail.Columns.Add("CreditSalesRate", typeof(decimal));
            dtPurchaseOrderDetail.Columns.Add("CRSalesRate12Month", typeof(decimal));
            dtPurchaseOrderDetail.Columns.Add("CRSalesRate3Month", typeof(decimal));
            dtPurchaseOrderDetail.Columns.Add("GodownId", typeof(int));
            dtPurchaseOrderDetail.Columns.Add("SFTRate", typeof(decimal));
            dtPurchaseOrderDetail.Columns.Add("TotalSFT", typeof(decimal));


            //POProductDetail
            dtPOProductDetail.Columns.Add("ProductID", typeof(int));
            dtPOProductDetail.Columns.Add("ColorId", typeof(int));
            dtPOProductDetail.Columns.Add("IMENO", typeof(string));
            dtPOProductDetail.Columns.Add("DamagePOPDID", typeof(int));



            foreach (var item in purchaseOrder.PODetails)
            {
                row = dtPurchaseOrderDetail.NewRow();
                id = int.Parse(GetDefaultIfNull(item.PODetailId));

                if (id > 0)
                    row["POrderDetailId"] = id;
                else
                    row["POrderDetailId"] = DBNull.Value;

                row["ProductId"] = item.ProductId;
                row["ColorId"] = item.ColorId;
                row["Status"] = (int)item.Status;
                row["Quantity"] = item.Quantity;
                row["UnitPrice"] = item.UnitPrice;
                row["TAmount"] = item.TAmount;
                row["PPDisPer"] = GetDefaultIfNull(item.PPDisPercentage);
                row["PPDisAmt"] = GetDefaultIfNull(item.PPDiscountAmount);
                row["MrpRate"] = GetDefaultIfNull(item.MRPRate);
                row["SalesRate"] = GetDefaultIfNull(item.SalesRate);
                row["ExtraPPDISPer"] = GetDefaultIfNull(item.ExtraPPDISPer);
                row["ExtraPPDISAmt"] = GetDefaultIfNull(item.ExtraPPDISAmt);
                row["PPOffer"] = GetDefaultIfNull(item.PPOffer);
                row["CreditSalesRate"] = GetDefaultIfNull(item.CreditSalesRate);
                row["CRSalesRate12Month"] = GetDefaultIfNull(item.CRSalesRate12Month);
                row["CRSalesRate3Month"] = GetDefaultIfNull(item.CRSalesRate3Month);
                row["GodownId"] = "0";
                row["SFTRate"] = "0.00";
                row["TotalSFT"] = "0.00";
                //POProductDetail
                CreatePOProductDetailDataTable(item, dtPOProductDetail);



                dtPurchaseOrderDetail.Rows.Add(row);
            }

            dsPurchaseOrderDetail.Tables.Add(dtPurchaseOrderDetail);
            dsPurchaseOrderDetail.Tables.Add(dtPOProductDetail);
            dsPurchaseOrderDetail.Tables.Add(dtStockDetail);

            return dsPurchaseOrderDetail;
        }
        private void CreatePOProductDetailDataTable(CreatePurchaseOrderDetailViewModel poDetail, DataTable dtPOProductDetail)
        {
            DataRow row = null;
            foreach (var item in poDetail.POProductDetails)
            {
                row = dtPOProductDetail.NewRow();
                row["ProductID"] = item.ProductID;
                row["ColorID"] = poDetail.ColorId;
                row["IMENO"] = item.IMENO.Trim();
                row["DamagePOPDID"] = item.DamagePOPDID ?? 0;
                dtPOProductDetail.Rows.Add(row);
            }
        }

        private void CheckAndAddModelErrorForSave(PurchaseReturnOrderViewModel newPurchaseOrder, FormCollection formCollection)
        {
            if (string.IsNullOrEmpty(formCollection["OrderDate"]))
                ModelState.AddModelError("PurchaseOrder.OrderDate", "Purchase Date is required");

            if (string.IsNullOrEmpty(formCollection["SuppliersId"]))
                ModelState.AddModelError("PurchaseOrder.SupplierId", "Supplier is required");
        }

        private void AddToOrder(PurchaseReturnOrderViewModel newPurchaseOrder, PurchaseReturnOrderViewModel purchaseOrder, FormCollection formCollection)
        {
            purchaseOrder.POProductDetailList = purchaseOrder.POProductDetailList ?? new List<CreatePOProductDetailViewModel>();

            purchaseOrder.POProductDetailList.Add(newPurchaseOrder.POProductDetails);

            PurchaseReturnOrderViewModel vm = new PurchaseReturnOrderViewModel
            {
                POProductDetails = new CreatePOProductDetailViewModel(),
                POProductDetailList = purchaseOrder.POProductDetailList,
                PurchaseOrder = purchaseOrder.PurchaseOrder
            };
            TempData["purchaseOrderViewModel"] = vm;

            purchaseOrder.POProductDetails = new CreatePOProductDetailViewModel();
            AddToastMessage("", "Order has been added successfully.", ToastType.Success);
        }

        public ActionResult DeleteFromView(int SDetailID)
        {
            PurchaseReturnOrderViewModel purchaseOrder = (PurchaseReturnOrderViewModel)TempData.Peek("purchaseOrderViewModel");
            if (purchaseOrder != null)
            {
                //tempdata getting null after redirection, so we're restoring purchaseOrder 
                var DeletePOPDetail = purchaseOrder.POProductDetailList.FirstOrDefault(i => i.SDetailID == SDetailID);
                purchaseOrder.POProductDetailList.Remove(DeletePOPDetail);
                TempData["purchaseOrderViewModel"] = purchaseOrder;
                return View("Create", purchaseOrder);
            }
            else
            {
                string chllnNo = _miscellaneousService.GetUniqueKey(x => int.Parse(x.ChallanNo));
                var color = _colorService.GetAllColor().FirstOrDefault();
                return View(new PurchaseReturnOrderViewModel
                {
                    POProductDetails = new CreatePOProductDetailViewModel(),
                    PurchaseOrder = new CreatePurchaseOrderViewModel { ChallanNo = chllnNo }
                });
            }
        }

        [Authorize]
        public ActionResult GetInvoiceByID(int orderId)
        {
            TempData["POrderID"] = orderId;
            TempData["IsInvoiceReadyById"] = true;
            return RedirectToAction("Index");
        }

        [HttpGet]
        [Authorize]
        public ActionResult DailyPurchaseReturnReport()
        {
            return View();
        }
        [HttpGet]
        [Authorize]
        public ActionResult MonthlyPurchaseReturnReport()
        {
            return View();
        }

        [HttpGet]
        [Authorize]
        public ActionResult YearlyPurchaseReturnReport()
        {
            return View();
        }

        [HttpGet]
        [Authorize]
        public ActionResult ProductWisePurchaseReturnReport()
        {
            return View();
        }
        [HttpGet]
        [Authorize]
        public ActionResult ModelWiseReport()
        {
            return View();
        }
    }
}
