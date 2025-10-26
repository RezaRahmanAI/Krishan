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
using log4net;
using IMSWEB.Model.TOs;

namespace IMSWEB.Controllers
{
    [Authorize]
    [RoutePrefix("advance-sales-order")]
    public class AdvanceSalesOrderController : CoreController
    {
        ISalesOrderService _salesOrderService;
        ISalesOrderDetailService _salesOrderDetailService;
        IStockService _stockService;
        IStockDetailService _stockDetailService;
        ICustomerService _customerService;
        IEmployeeService _employeeService;
        ITransactionalReport _transactionalReportService;
        IMiscellaneousService<SOrder> _miscellaneousService;
        IProductService _productService;
        IMapper _mapper;
        ISisterConcernService _SisterConcern;
        ISRVisitService _SRVisitService;
        IUserService _UserService;
        ISystemInformationService _SysInfoService;
        ISMSStatusService _SMSStatusService;
        private readonly IProductUnitTypeService _productUnitTypeService;
        private readonly ISizeService _sizeService;
        private readonly IDepotService _DepotService;
        private readonly ISMSBillPaymentBkashService _smsBillPaymentBkashService;
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public AdvanceSalesOrderController(IErrorService errorService, ISalesOrderService salesOrderService, ISalesOrderDetailService salesOrderDetailService, IStockService stockService, IStockDetailService stockDetailService, ICustomerService customerService, IEmployeeService employeeService, ITransactionalReport transactionalReportService, IMiscellaneousService<SOrder> miscellaneousService, IMapper mapper, ISRVisitService SRVisitService, IUserService UserService, IProductService productService, ISisterConcernService sisterConcern, ISystemInformationService SysInfoService, ISMSStatusService SMSStatusService, IProductUnitTypeService productUnitTypeService, ISizeService sizeService, IDepotService DepotService, ISMSBillPaymentBkashService smsBillPaymentBkashService)
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
            _SisterConcern = sisterConcern;
            _SRVisitService = SRVisitService;
            _UserService = UserService;
            _SysInfoService = SysInfoService;
            _SMSStatusService = SMSStatusService;
            _productUnitTypeService = productUnitTypeService;
            _sizeService = sizeService;
            _DepotService = DepotService;
            _smsBillPaymentBkashService = smsBillPaymentBkashService;
        }

        [HttpGet]
        [Authorize]
        [Route("index")]
        public async Task<ActionResult> Index()
        {
            int userId = System.Web.HttpContext.Current.User.Identity.GetUserId<int>();
            TempData["salesOrderViewModel"] = null;
            //if(userId==1014 || userId==1015 || userId==1016 || userId==1017|| userId==1018)
            var DateRange = GetFirstAndLastDateOfMonth(DateTime.Today);
            ViewBag.FromDate = DateRange.Item1;
            ViewBag.ToDate = DateRange.Item2;
            if (User.IsInRole(ConstantData.ROLE_MOBILE_USER))
            {
                List<EnumSalesType> status = new List<EnumSalesType>();
                status.Add(EnumSalesType.Sales);
                status.Add(EnumSalesType.Advance);
                var customSO = _salesOrderService.GetAllAdvanceSalesOrderAsyncByUserID(userId, DateRange.Item1, DateRange.Item2, status);
                var vmSO = _mapper.Map<IEnumerable<Tuple<int, string, DateTime, string, string, decimal, EnumSalesType, Tuple<string, bool>>>,
                IEnumerable<GetSalesOrderViewModel>>(await customSO);
                return View(vmSO);

            }
            else
            {
                List<EnumSalesType> status = new List<EnumSalesType>();
                status.Add(EnumSalesType.Sales);
                status.Add(EnumSalesType.Pending);
                status.Add(EnumSalesType.Advance);
                var customSO = _salesOrderService.GetAllAdvanceSalesOrderAsync(DateRange.Item1, DateRange.Item2, status, IsVATManager(), User.Identity.GetConcernId());
                var vmSO = _mapper.Map<IEnumerable<Tuple<int, string, DateTime, string, string, decimal, EnumSalesType, Tuple<string, bool, string>>>,
                IEnumerable<GetSalesOrderViewModel>>(await customSO);
                return View(vmSO);
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult> Index(FormCollection formCollection)
        {
            TempData["salesOrderViewModel"] = null;
            string InvoiceNo = string.Empty, ContactNo = "", CustomerName = "", AccountNo = "";
            DateTime fromDate = DateTime.MinValue;
            DateTime toDate = DateTime.MinValue;

            if (!string.IsNullOrEmpty(formCollection["FromDate"]))
                fromDate = Convert.ToDateTime(formCollection["FromDate"]);
            if (!string.IsNullOrEmpty(formCollection["ToDate"]))
                toDate = Convert.ToDateTime(formCollection["ToDate"]);

            ViewBag.FromDate = fromDate;
            ViewBag.ToDate = toDate;

            if (!string.IsNullOrEmpty(formCollection["InvoiceNo"]))
                InvoiceNo = formCollection["InvoiceNo"].Trim();
            if (!string.IsNullOrEmpty(formCollection["ContactNo"]))
                ContactNo = formCollection["ContactNo"].Trim();
            if (!string.IsNullOrEmpty(formCollection["CustomerName"]))
                CustomerName = formCollection["CustomerName"].Trim();

            if (!string.IsNullOrEmpty(formCollection["AccountNo"]))
                AccountNo = formCollection["AccountNo"].Trim();

            if (User.IsInRole(ConstantData.ROLE_MOBILE_USER))
            {
                List<EnumSalesType> status = new List<EnumSalesType>();
                status.Add(EnumSalesType.Sales);
                status.Add(EnumSalesType.Advance);

                int userId = System.Web.HttpContext.Current.User.Identity.GetUserId<int>();
                var customSO = _salesOrderService.GetAllAdvanceSalesOrderAsyncByUserID(userId, ViewBag.FromDate, ViewBag.ToDate, status,
                    InvoiceNo, ContactNo, CustomerName, AccountNo);
                var vmSO = _mapper.Map<IEnumerable<Tuple<int, string, DateTime, string, string, decimal, EnumSalesType, Tuple<string, bool>>>,
                IEnumerable<GetSalesOrderViewModel>>(await customSO);
                return View("Index", vmSO);
            }
            else

            {
                List<EnumSalesType> status = new List<EnumSalesType>();
                status.Add(EnumSalesType.Sales);
                status.Add(EnumSalesType.Pending);

                var customSO = _salesOrderService.GetAllAdvanceSalesOrderAsync(fromDate, toDate,
                    status, IsVATManager(), User.Identity.GetConcernId(), InvoiceNo, ContactNo, CustomerName, AccountNo);
                var vmSO = _mapper.Map<IEnumerable<Tuple<int, string, DateTime, string, string, decimal, EnumSalesType, Tuple<string, bool, string>>>,
                IEnumerable<GetSalesOrderViewModel>>(await customSO);
                return View("Index", vmSO);
            }
        }


        [HttpGet]
        [Authorize]
        [Route("create")]
        public ActionResult Create()
        {
            ViewBag.IsEmployeeWiseTransEnable = _SysInfoService.IsEmployeeWiseTransactionEnable();
            return ReturnCreateViewWithTempData();
        }

        [HttpPost]
        [Authorize]
        [Route("create/returnUrl")]
        [ValidateAntiForgeryToken]
        public ActionResult Create(SalesOrderViewModel newSalesOrder, FormCollection formCollection, string returnUrl)
        {
            ViewBag.IsEmployeeWiseTransEnable = _SysInfoService.IsEmployeeWiseTransactionEnable();
            return HandleSalesOrder(newSalesOrder, formCollection);
        }

        [HttpGet]
        [Authorize]
        [Route("edit/{orderId}")]
        public ActionResult Edit(int orderId, string previousAction)
        {
            ViewBag.IsEmployeeWiseTransEnable = _SysInfoService.IsEmployeeWiseTransactionEnable();

            ViewBag.ProductIds = GetAllProductsForDDL();
            ViewBag.CustomerIds = GetAllCustomerForDDL();
            ViewBag.EmployeeIds = GetAllEmployeeForDDL();
            //ViewBag.DepotIds = GetAllDepotForDDL();

            if (TempData["salesOrderViewModel"] == null || string.IsNullOrEmpty(previousAction))
            {
                var salesOrder = _salesOrderService.GetSalesOrderById(orderId);
                var soDetails = _salesOrderDetailService.GetSalesOrderDetailByOrderId(orderId);

                var vmSalesOrder = _mapper.Map<SOrder, CreateSalesOrderViewModel>(salesOrder);
                var vmSoDetails = _mapper.Map<IEnumerable<Tuple<int, int, int, int, string, string, string,
                    Tuple<decimal, decimal, decimal, decimal, decimal, decimal, int, Tuple<string, decimal, decimal>>>>, IEnumerable<CreateSalesOrderDetailViewModel>>(soDetails).ToList();
                log.Info(new { vmSalesOrder, vmSoDetails });
                var vm = new SalesOrderViewModel
                {
                    SODetail = new CreateSalesOrderDetailViewModel(),
                    SODetails = vmSoDetails,
                    SalesOrder = vmSalesOrder
                };

                TempData["salesOrderViewModel"] = vm;
                return View("Create", vm);
            }
            else
            {
                return ReturnCreateViewWithTempData();
            }
        }

        [HttpPost]
        [Authorize]
        [Route("edit/returnUrl")]
        public ActionResult Edit(SalesOrderViewModel newSalesOrder, FormCollection formCollection, string returnUrl)
        {
            ViewBag.IsEmployeeWiseTransEnable = _SysInfoService.IsEmployeeWiseTransactionEnable();
            return HandleSalesOrder(newSalesOrder, formCollection);
        }

        [HttpGet]
        [Authorize]
        [Route("delete/{orderId}")]
        public ActionResult Delete(int orderId)
        {
            var Sales = _salesOrderService.GetSalesOrderById(orderId);
            if (!IsDateValid(Sales.InvoiceDate))
            {
                return RedirectToAction("Index");
            }
            _salesOrderService.DeleteAdvanceSalesOrderUsingSP(orderId, User.Identity.GetUserId<int>());
            log.Info(new { SOrderID = orderId });
            AddToastMessage("", "Item has been deleted successfully", ToastType.Success);
            return RedirectToAction("Index");
        }

        [HttpGet]
        [Authorize]
        [Route("editfromview/{id}/{detailId}")]
        public ActionResult EditFromView(int id, int detailId, string previousAction)
        {
            ViewBag.IsEmployeeWiseTransEnable = _SysInfoService.IsEmployeeWiseTransactionEnable();

            SalesOrderViewModel salesOrder = (SalesOrderViewModel)TempData.Peek("salesOrderViewModel");
            if (salesOrder == null)
            {
                AddToastMessage("", "Item has been expired to edit", ToastType.Error);
                if (IsForEdit(previousAction))
                    return RedirectToAction("Index");
                else
                    return RedirectToAction("Create");
            }

            CreateSalesOrderDetailViewModel itemToEdit =
                salesOrder.SODetails.Where(x => int.Parse(x.ProductId) == id &&
                             int.Parse(x.StockDetailId) == detailId).FirstOrDefault();
            if (itemToEdit != null)
            {
                salesOrder.SalesOrder.GrandTotal = (decimal.Parse(GetDefaultIfNull(salesOrder.SalesOrder.GrandTotal)) -
                    (decimal.Parse(GetDefaultIfNull(itemToEdit.UTAmount)) + decimal.Parse(GetDefaultIfNull(itemToEdit.PPDAmount)))).ToString();

                salesOrder.SalesOrder.PPDiscountAmount = (decimal.Parse(GetDefaultIfNull(salesOrder.SalesOrder.PPDiscountAmount)) -
                    decimal.Parse(GetDefaultIfNull(itemToEdit.PPDAmount))).ToString();

                salesOrder.SalesOrder.NetDiscount = (decimal.Parse(GetDefaultIfNull(salesOrder.SalesOrder.NetDiscount)) -
                    decimal.Parse(GetDefaultIfNull(itemToEdit.PPDAmount)) - decimal.Parse(GetDefaultIfNull(itemToEdit.PPOffer))).ToString();

                salesOrder.SalesOrder.TotalAmount = (decimal.Parse(GetDefaultIfNull(salesOrder.SalesOrder.TotalAmount)) -
                    (decimal.Parse(GetDefaultIfNull(itemToEdit.UTAmount)))).ToString();

                salesOrder.SalesOrder.PaymentDue = ((decimal.Parse(GetDefaultIfNull(salesOrder.SalesOrder.PaymentDue)) + decimal.Parse(GetDefaultIfNull(salesOrder.SalesOrder.RecieveAmount))) -
                    (decimal.Parse(GetDefaultIfNull(itemToEdit.UTAmount)))).ToString();

                //For Total Offer Calculation
                salesOrder.SalesOrder.TotalOffer = (decimal.Parse(GetDefaultIfNull(salesOrder.SalesOrder.TotalOffer)) -
                decimal.Parse(GetDefaultIfNull(itemToEdit.PPOffer))).ToString();

                if (IsForEdit(previousAction) && !string.IsNullOrEmpty(itemToEdit.SODetailId))
                {
                    itemToEdit.Status = EnumStatus.Deleted;
                    //int sorderDetailId = int.Parse(itemToEdit.SODetailId);
                    //int userId = User.Identity.GetUserId<int>();
                    //_salesOrderService.DeleteSalesOrderDetailUsingSP(sorderDetailId, userId);
                }
                else
                {
                    salesOrder.SODetails.Remove(itemToEdit);
                }

                salesOrder.SODetail = itemToEdit;
                TempData["salesOrderViewModel"] = salesOrder;

                if (IsForEdit(previousAction))
                    return RedirectToAction("Edit", new { orderId = default(int), previousAction = "Edit" });
                else
                    return RedirectToAction("Create");
            }
            else
            {
                AddToastMessage("", "No item found to edit", ToastType.Info);
                if (IsForEdit(previousAction))
                    return RedirectToAction("Index");
                else
                    return RedirectToAction("Create");
            }
        }

        [HttpGet]
        [Authorize]
        [Route("deletefromview/{id}/{detailId}")]
        public ActionResult DeleteFromView(int id, int detailId, string previousAction)
        {
            ViewBag.IsEmployeeWiseTransEnable = _SysInfoService.IsEmployeeWiseTransactionEnable();

            SalesOrderViewModel salesOrder = (SalesOrderViewModel)TempData.Peek("salesOrderViewModel");
            if (salesOrder == null)
            {
                AddToastMessage("", "Item has been expired to delete", ToastType.Error);
                if (IsForEdit(previousAction))
                    return RedirectToAction("Index");
                else
                    return RedirectToAction("Create");
            }

            CreateSalesOrderDetailViewModel itemToDelete =
                salesOrder.SODetails.Where(x => int.Parse(x.ProductId) == id &&
                             int.Parse(x.StockDetailId) == detailId).FirstOrDefault();
            if (itemToDelete != null)
            {
                salesOrder.SalesOrder.GrandTotal = (decimal.Parse(GetDefaultIfNull(salesOrder.SalesOrder.GrandTotal)) -
                    (decimal.Parse(GetDefaultIfNull(itemToDelete.UTAmount)) + decimal.Parse(GetDefaultIfNull(itemToDelete.PPDAmount)) + itemToDelete.FractionAmt)).ToString();

                salesOrder.SalesOrder.PPDiscountAmount = (decimal.Parse(GetDefaultIfNull(salesOrder.SalesOrder.PPDiscountAmount)) -
                    decimal.Parse(GetDefaultIfNull(itemToDelete.PPDAmount))).ToString();

                salesOrder.SalesOrder.NetDiscount = (decimal.Parse(GetDefaultIfNull(salesOrder.SalesOrder.NetDiscount)) -
                    decimal.Parse(GetDefaultIfNull(itemToDelete.PPDAmount))).ToString();

                salesOrder.SalesOrder.TotalAmount = (decimal.Parse(GetDefaultIfNull(salesOrder.SalesOrder.TotalAmount)) -
                    (decimal.Parse(GetDefaultIfNull(itemToDelete.UTAmount)) + itemToDelete.FractionAmt)).ToString();

                //salesOrder.SalesOrder.PaymentDue = ((decimal.Parse(GetDefaultIfNull(salesOrder.SalesOrder.PaymentDue)) + decimal.Parse(GetDefaultIfNull(salesOrder.SalesOrder.RecieveAmount))) -
                //    (decimal.Parse(GetDefaultIfNull(itemToDelete.UTAmount)))).ToString();

                salesOrder.SalesOrder.PaymentDue = ((decimal.Parse(GetDefaultIfNull(salesOrder.SalesOrder.PaymentDue))) -
          (decimal.Parse(GetDefaultIfNull(itemToDelete.UTAmount)) + itemToDelete.FractionAmt)).ToString();

                salesOrder.SalesOrder.TotalFractionAmt = salesOrder.SalesOrder.TotalFractionAmt - itemToDelete.FractionAmt;

                //For Offer Purpose
                salesOrder.SalesOrder.TotalOffer = (decimal.Parse(GetDefaultIfNull(salesOrder.SalesOrder.TotalOffer)) -
               (decimal.Parse(GetDefaultIfNull(itemToDelete.PPOffer)))).ToString();

                if (IsForEdit(previousAction) && !string.IsNullOrEmpty(itemToDelete.SODetailId))
                {
                    itemToDelete.Status = EnumStatus.Deleted;
                    //int sorderDetailId = int.Parse(itemToDelete.SODetailId);
                    //int userId = User.Identity.GetUserId<int>();
                    //_salesOrderService.DeleteSalesOrderDetailUsingSP(sorderDetailId, userId);
                }
                else
                {
                    salesOrder.SODetails.Remove(itemToDelete);
                }

                salesOrder.SODetail = new CreateSalesOrderDetailViewModel();
                TempData["salesOrderViewModel"] = salesOrder;
                AddToastMessage("", "Item has been removed successfully", ToastType.Success);

                if (IsForEdit(previousAction))
                    return RedirectToAction("Edit", new { orderId = default(int), previousAction = "Edit" });
                else
                    return RedirectToAction("Create");
            }
            else
            {
                AddToastMessage("", "No item found to remove", ToastType.Info);
                if (IsForEdit(previousAction))
                    return RedirectToAction("Index");
                else
                    return RedirectToAction("Create");
            }
        }

        private void CheckAndAddModelErrorForAdd(SalesOrderViewModel newSalesOrder,
            SalesOrderViewModel salesOrder, FormCollection formCollection)
        {
            bool IsEmployeeWiseTransactionEnable = _SysInfoService.IsEmployeeWiseTransactionEnable();

            if (string.IsNullOrEmpty(formCollection["OrderDate"]))
                ModelState.AddModelError("SalesOrder.OrderDate", "Sales Date is required");

            int customerId = 0;
            int.TryParse(newSalesOrder.SalesOrder.CustomerId, out customerId);

            if (customerId == 0 && string.IsNullOrEmpty(formCollection["CustomersId"]))
                ModelState.AddModelError("SalesOrder.CustomerId", "Customer is required");

            //if (string.IsNullOrEmpty(formCollection["DepotsId"]))
            //    ModelState.AddModelError("SalesOrder.DepotId", "Depot is required");

            int productId = 0;

            int.TryParse(newSalesOrder.SODetail.ProductId, out productId);
            //ProductDetailsId is ProductId
            if (productId == 0 && string.IsNullOrEmpty(formCollection["ProductDetailsId"]))
                ModelState.AddModelError("SODetail.ProductId", "Product is required");
            else
            {
                newSalesOrder.SODetail.ProductId = productId > 0 ? productId.ToString() : formCollection["ProductDetailsId"];
                salesOrder.SODetail.ProductId = formCollection["ProductDetailsId"];
            }

            if (string.IsNullOrEmpty(newSalesOrder.SODetail.Quantity) || Convert.ToInt32(double.Parse(newSalesOrder.SODetail.Quantity)) <= 0)
            {
                ModelState.AddModelError("SODetail.Quantity", "Quantity is required");
            }

            if (string.IsNullOrEmpty(newSalesOrder.SalesOrder.InvoiceNo))
                ModelState.AddModelError("SalesOrder.InvoiceNo", "Invoice No. is required");

            if (IsEmployeeWiseTransactionEnable && !User.IsInRole(EnumUserRoles.MobileUser.ToString()))
            {
                if (formCollection["EmployeesId"].IsNullOrEmpty())
                {
                    ModelState.AddModelError("SalesOrder.EmployeeID", "Employee is required");
                    return;
                }
                newSalesOrder.SalesOrder.EmployeeID = formCollection["EmployeesId"];
            }
            else if (User.IsInRole(EnumUserRoles.MobileUser.ToString()))
            {
                int SRUID = User.Identity.GetUserId<int>();
                var user = _UserService.GetUserById(SRUID);
                newSalesOrder.SalesOrder.EmployeeID = user.EmployeeID.ToString();
            }
            else
                newSalesOrder.SalesOrder.EmployeeID = "0";

            if (string.IsNullOrEmpty(newSalesOrder.SODetail.MRPRate))
                ModelState.AddModelError("SODetail.MRPRate", "Purchase Rate is required");

            if (string.IsNullOrEmpty(newSalesOrder.SODetail.UnitPrice))
                ModelState.AddModelError("SODetail.UnitPrice", "Sales Rate is required");

            if (string.IsNullOrEmpty(newSalesOrder.SODetail.IMENo))
            {
                ModelState.AddModelError("SODetail.IMENo", "IMENo/Barcode is required");
            }
            else
            {
                //var stockDetails = _stockDetailService.GetStockDetailByProductId(
                //    int.Parse(GetDefaultIfNull(formCollection["ProductDetailsId"])));

                //if (stockDetails.Count() < int.Parse(newSalesOrder.SODetail.Quantity))
                //{
                //    ModelState.AddModelError("SODetail.Quantity", "Stock is not available. Stock Quantity: " + stockDetails.Count());
                //}
                //if (!stockDetails.Any(x => x.IMENO.Equals(newSalesOrder.SODetail.IMENo)))
                //    ModelState.AddModelError("SODetail.IMENo", "Invalid IMENo/Barcode");

                var product = _productService.GetProductById(int.Parse(GetDefaultIfNull(formCollection["ProductDetailsId"])));

                int SDetailID = int.Parse(GetDefaultIfNull(formCollection["StockDetailsId"]));
                //  int ColorID = int.Parse(GetDefaultIfNull(formCollection["ProductDetailsId"]));

                if (product.ProductType == (int)EnumProductType.NoBarcode)
                {

                    var stockDeatilCount = _stockDetailService.GetById(SDetailID);// _stockService.GET.GetStockByProductIdandColorIDandGodownID(product.ProductID, 1, 1);
                    var StockCount = _stockService.GetStockById(stockDeatilCount.StockID);
                    if (StockCount.Quantity < int.Parse(newSalesOrder.SODetail.Quantity))
                        ModelState.AddModelError("SODetail.Quantity", "Stock is not available. Stock Quantity: " + StockCount.Quantity);
                }
                else
                {
                    var stockDetails = _stockDetailService.GetStockDetailByProductId(int.Parse(GetDefaultIfNull(formCollection["ProductDetailsId"])));
                    if (!stockDetails.Any(x => x.IMENO.Equals(newSalesOrder.SODetail.IMENo)))
                        ModelState.AddModelError("SODetail.IMENo", "Invalid IMENo/Barcode");
                }
            }
            SetEmployeeID(newSalesOrder, formCollection);
        }

        private void SetEmployeeID(SalesOrderViewModel newSalesOrder, FormCollection formCollection)
        {
            if (!_SysInfoService.IsEmployeeWiseTransactionEnable())
                return;

            if (User.IsInRole(EnumUserRoles.MobileUser.ToString()))
            {
                int SRUID = User.Identity.GetUserId<int>();
                var user = _UserService.GetUserById(SRUID);
                newSalesOrder.SalesOrder.EmployeeID = user.EmployeeID.ToString();
            }
            else if (User.IsInRole(EnumUserRoles.RetailManager.ToString()))
            {
                var emp = _employeeService.GetAllEmployeeIQueryable().FirstOrDefault(i => i.Name.Equals("showroom"));
                int EmployeeID = emp != null ? emp.EmployeeID : 0;
                if (EmployeeID == 0)
                {
                    AddToastMessage(message: "Showroom Employee is required for manager.", toastType: ToastType.Error);
                    ModelState.AddModelError("SalesOrder.EmployeeID", "Showroom Employee is required for manager.");
                }

                newSalesOrder.SalesOrder.EmployeeID = EmployeeID.ToString();
            }
            else
            {

                if (string.IsNullOrEmpty(formCollection["EmployeesId"]))
                {
                    ModelState.AddModelError("SalesOrder.EmployeeID", "Employee is required.");
                }
                else
                {
                    newSalesOrder.SalesOrder.EmployeeID = formCollection["EmployeesId"];
                }
            }
        }
        private void CheckAndAddModelErrorForSave(SalesOrderViewModel newSalesOrder, SalesOrderViewModel salesOrder, FormCollection formCollection)
        {
            if (string.IsNullOrEmpty(newSalesOrder.SalesOrder.GrandTotal) ||
                decimal.Parse(GetDefaultIfNull(newSalesOrder.SalesOrder.GrandTotal)) <= 0)
                ModelState.AddModelError("SalesOrder.GrandTotal", "Grand Total is required");

            if (string.IsNullOrEmpty(newSalesOrder.SalesOrder.TotalAmount) ||
                decimal.Parse(GetDefaultIfNull(newSalesOrder.SalesOrder.TotalAmount)) <= 0)
                ModelState.AddModelError("SalesOrder.TotalAmount", "Net Total is required");

            int customerId = 0;
            int.TryParse(newSalesOrder.SalesOrder.CustomerId, out customerId);

            if (customerId == 0 && formCollection["CustomersId"].IsNullOrEmpty())
            {
                ModelState.AddModelError("SalesOrder.CustomerId", "Customer is required");
            }

            //if (string.IsNullOrEmpty(newSalesOrder.SalesOrder.RecieveAmount) ||
            //    decimal.Parse(GetDefaultIfNull(newSalesOrder.SalesOrder.RecieveAmount)) <= 0)
            //    ModelState.AddModelError("SalesOrder.RecieveAmount", "Pay Amount is required");
            if (newSalesOrder.SalesOrder.RecieveAmount == null || newSalesOrder.SalesOrder.RecieveAmount == "")
            {
                newSalesOrder.SalesOrder.RecieveAmount = "0";
                salesOrder.SalesOrder.RecieveAmount = "0";
            }

            SetEmployeeID(newSalesOrder, formCollection);

            #region Old Customer and Employee Due Limit Check
            //Customer customer = _customerService.GetCustomerById(int.Parse(salesOrder.SalesOrder.CustomerId));
            //Employee employee = _employeeService.GetEmployeeById(customer.EmployeeID);
            //var sysInfo = _SysInfoService.GetSystemInformationByConcernId(User.Identity.GetConcernId());
            //if (sysInfo != null)
            //{
            //    if (sysInfo.CustomerDueLimitApply == 1)
            //    {
            //        if (decimal.Parse(GetDefaultIfNull(newSalesOrder.SalesOrder.PaymentDue)) > customer.CusDueLimit)
            //            ModelState.AddModelError("SalesOrder.PaymentDue", "Customer due limit is exceeding");
            //    }
            //    if (sysInfo.EmployeeDueLimitApply == 1)
            //    {
            //        if (decimal.Parse(GetDefaultIfNull(newSalesOrder.SalesOrder.PaymentDue)) > employee.SRDueLimit)
            //            ModelState.AddModelError("SalesOrder.PaymentDue", "SR due limit is exceeding");
            //    }
            //}

            #endregion

            #region Customer and Employee Due Limit Check
            if (customerId > 0 || formCollection["CustomersId"].IsNotNullOrEmpty())
            {
                Customer customer = _customerService.GetCustomerById(customerId > 0 ? customerId : int.Parse(formCollection["CustomersId"]));
                Employee employee = _employeeService.GetEmployeeById(customer.EmployeeID);
                var sysInfo = _SysInfoService.GetSystemInformationByConcernId(User.Identity.GetConcernId());
                if (sysInfo != null)
                {
                    if (sysInfo.CustomerDueLimitApply == 1)
                    {
                        if (decimal.Parse(GetDefaultIfNull(newSalesOrder.SalesOrder.PaymentDue)) > customer.CusDueLimit)
                            ModelState.AddModelError("SalesOrder.PaymentDue", "Customer due limit is exceeding");
                    }
                    if (sysInfo.EmployeeDueLimitApply == 1)
                    {
                        if (decimal.Parse(GetDefaultIfNull(newSalesOrder.SalesOrder.PaymentDue)) > employee.SRDueLimit)
                            ModelState.AddModelError("SalesOrder.PaymentDue", "SR due limit is exceeding");
                    }
                }
            }
            #endregion

            var distinctIMEI = salesOrder.SODetails
                                .GroupBy(i => i.StockDetailId)
                                .Select(g => g.First())
                                .ToList();

            if (distinctIMEI.Count() != salesOrder.SODetails.Count())
            {
                ModelState.AddModelError("SODetail.IMENo", "");
                AddToastMessage("", "Duplicate IMEI added.", ToastType.Error);
            }
            salesOrder.SalesOrder.OrderDate = formCollection["OrderDate"];
            if (!IsDateValid(Convert.ToDateTime(salesOrder.SalesOrder.OrderDate)))
            {
                ModelState.AddModelError("SalesOrder.OrderDate", "Back dated entry is not valid.");
            }
            //if (newSalesOrder.SalesOrder.TermsType == 0)
            //    ModelState.AddModelError("SalesOrder.TermsType", "Terms type is required.");

        }

        private void AddToOrder(SalesOrderViewModel newSalesOrder,
            SalesOrderViewModel salesOrder, FormCollection formCollection)
        {
            decimal quantity = decimal.Parse(GetDefaultIfNull(newSalesOrder.SODetail.Quantity));
            decimal totalDisAmount = decimal.Parse(GetDefaultIfNull(newSalesOrder.SODetail.PPDAmount));
            decimal totalOffer = quantity * decimal.Parse(GetDefaultIfNull(newSalesOrder.SODetail.PPOffer));
            //decimal Bonusquantity = decimal.Parse(GetDefaultIfNull(newSalesOrder.SODetail.BonusQuantity));

            salesOrder.SalesOrder.GrandTotal = (decimal.Parse(GetDefaultIfNull(newSalesOrder.SalesOrder.GrandTotal)) +
                decimal.Parse(GetDefaultIfNull(newSalesOrder.SODetail.UTAmount)) + totalDisAmount + totalOffer
                + newSalesOrder.SODetail.FractionAmt
                ).ToString();

            salesOrder.SalesOrder.PPDiscountAmount = (decimal.Parse(GetDefaultIfNull(newSalesOrder.SalesOrder.PPDiscountAmount)) + totalDisAmount).ToString();

            salesOrder.SalesOrder.TotalDiscountPercentage = decimal.Parse(GetDefaultIfNull(newSalesOrder.SalesOrder.TotalDiscountPercentage)).ToString();
            salesOrder.SalesOrder.TotalDiscountAmount = decimal.Parse(GetDefaultIfNull(newSalesOrder.SalesOrder.TotalDiscountAmount)).ToString();
            salesOrder.SalesOrder.TempFlatDiscountAmount = salesOrder.SalesOrder.TotalDiscountAmount;

            salesOrder.SalesOrder.VATPercentage = decimal.Parse(GetDefaultIfNull(newSalesOrder.SalesOrder.VATPercentage)).ToString();
            salesOrder.SalesOrder.VATAmount = decimal.Parse(GetDefaultIfNull(newSalesOrder.SalesOrder.VATAmount)).ToString();

            salesOrder.SalesOrder.AdjAmount = decimal.Parse(GetDefaultIfNull(newSalesOrder.SalesOrder.AdjAmount)).ToString();

            //salesOrder.SalesOrder.NetDiscount = (decimal.Parse(GetDefaultIfNull(salesOrder.SalesOrder.NetDiscount)) + decimal.Parse(GetDefaultIfNull(newSalesOrder.SODetail.PPDAmount)) +
            //    decimal.Parse(GetDefaultIfNull(newSalesOrder.SODetail.PPOffer))).ToString();
            // decimal.Parse(GetDefaultIfNull(salesOrder.SalesOrder.TotalDiscountAmount)) +

            salesOrder.SalesOrder.NetDiscount = (decimal.Parse(GetDefaultIfNull(salesOrder.SalesOrder.NetDiscount)) + totalDisAmount + totalOffer).ToString();

            var netTotal = ((decimal.Parse(GetDefaultIfNull(salesOrder.SalesOrder.GrandTotal)) + decimal.Parse(GetDefaultIfNull(salesOrder.SalesOrder.VATAmount))) -
                (decimal.Parse(GetDefaultIfNull(salesOrder.SalesOrder.NetDiscount)) + decimal.Parse(GetDefaultIfNull(salesOrder.SalesOrder.AdjAmount))
                ));

            salesOrder.SalesOrder.TotalFractionAmt = salesOrder.SalesOrder.TotalFractionAmt + newSalesOrder.SODetail.FractionAmt;

            // For Total Offer Purpose



            salesOrder.SalesOrder.TotalOffer = (decimal.Parse(GetDefaultIfNull(newSalesOrder.SalesOrder.TotalOffer)) + totalOffer).ToString();

            salesOrder.SalesOrder.TotalAmount = netTotal.ToString();
            salesOrder.SalesOrder.PaymentDue = (netTotal - decimal.Parse(GetDefaultIfNull(newSalesOrder.SalesOrder.RecieveAmount))).ToString();
            salesOrder.SalesOrder.RecieveAmount = GetDefaultIfNull(newSalesOrder.SalesOrder.RecieveAmount);

            salesOrder.SalesOrder.OrderDate = formCollection["OrderDate"];
            salesOrder.SalesOrder.RemindDate = formCollection["RemindDate"];

            int customerId = 0;
            int.TryParse(newSalesOrder.SalesOrder.CustomerId, out customerId);

            salesOrder.SalesOrder.CustomerId = customerId > 0 ? customerId.ToString() : formCollection["CustomersId"];
            //salesOrder.SalesOrder.DepotId = formCollection["DepotsId"];
            //salesOrder.SalesOrder.TermsType = newSalesOrder.SalesOrder.TermsType;

            salesOrder.SODetail.SODetailId = newSalesOrder.SODetail.SODetailId;
            salesOrder.SODetail.ProductId = formCollection["ProductDetailsId"];
            salesOrder.SODetail.ColorId = formCollection["ColorsId"];
            salesOrder.SODetail.ColorName = newSalesOrder.SODetail.ColorName;
            salesOrder.SODetail.StockDetailId = formCollection["StockDetailsId"];
            salesOrder.SODetail.ProductCode = formCollection["ProductDetailsCode"];
            salesOrder.SODetail.IMENo = newSalesOrder.SODetail.IMENo;
            salesOrder.SODetail.Quantity = newSalesOrder.SODetail.Quantity;
            salesOrder.SODetail.PPDPercentage = newSalesOrder.SODetail.PPDPercentage;
            salesOrder.SODetail.ConvertValue = newSalesOrder.SODetail.ConvertValue;

            salesOrder.SODetail.RatePerArea = newSalesOrder.SODetail.RatePerArea;
            salesOrder.SODetail.TotalArea = newSalesOrder.SODetail.TotalArea;
            salesOrder.SODetail.FractionQty = newSalesOrder.SODetail.FractionQty;
            salesOrder.SODetail.FractionAmt = newSalesOrder.SODetail.FractionAmt;

            //salesOrder.SODetail.PPDAmount = newSalesOrder.SODetail.PPDAmount;
            salesOrder.SODetail.PPDAmount = totalDisAmount.ToString();
            salesOrder.SODetail.UnitPrice = newSalesOrder.SODetail.UnitPrice;
            salesOrder.SODetail.MRPRate = newSalesOrder.SODetail.MRPRate;
            salesOrder.SODetail.UTAmount = newSalesOrder.SODetail.UTAmount;
            salesOrder.SODetail.ProductName = formCollection["ProductDetailsName"];
            salesOrder.SODetail.Status = newSalesOrder.SODetail.Status == default(int) ? EnumStatus.New : newSalesOrder.SODetail.Status;
            //salesOrder.SODetail.PPOffer = newSalesOrder.SODetail.PPOffer;
            salesOrder.SODetail.PPOffer = totalOffer.ToString();
            salesOrder.SODetail.CompressorWarrentyMonth = newSalesOrder.SODetail.CompressorWarrentyMonth;
            salesOrder.SODetail.MotorWarrentyMonth = newSalesOrder.SODetail.MotorWarrentyMonth;
            salesOrder.SODetail.PanelWarrentyMonth = newSalesOrder.SODetail.PanelWarrentyMonth;
            salesOrder.SODetail.SparePartsWarrentyMonth = newSalesOrder.SODetail.SparePartsWarrentyMonth;
            salesOrder.SODetail.ServiceWarrentyMonth = newSalesOrder.SODetail.ServiceWarrentyMonth;
            //salesOrder.SODetail.BonusQuantity = newSalesOrder.SODetail.BonusQuantity;

            salesOrder.SODetails = salesOrder.SODetails ?? new List<CreateSalesOrderDetailViewModel>();
            salesOrder.SODetails.Add(salesOrder.SODetail);

            SalesOrderViewModel vm = new SalesOrderViewModel
            {
                SODetail = new CreateSalesOrderDetailViewModel(),
                SODetails = salesOrder.SODetails,
                SalesOrder = salesOrder.SalesOrder
            };

            TempData["salesOrderViewModel"] = vm;
            salesOrder.SODetail = new CreateSalesOrderDetailViewModel();
            AddToastMessage("", "Order has been added successfully.", ToastType.Success);
        }

        private bool SaveOrder(SalesOrderViewModel newSalesOrder,
            SalesOrderViewModel salesOrder, FormCollection formCollection)
        {
            bool Result = false;
            DateTime RemindDate = DateTime.MinValue;
            var Customer = _customerService.GetCustomerById(Convert.ToInt32(salesOrder.SalesOrder.CustomerId));
            //salesOrder.SalesOrder.EmployeeID = Customer.EmployeeID;
            salesOrder.SalesOrder.PrevDue = Customer.TotalDue;

            salesOrder.SalesOrder.NetDiscount = GetDefaultIfNull(newSalesOrder.SalesOrder.NetDiscount);
            salesOrder.SalesOrder.TotalAmount = decimal.Parse(GetDefaultIfNull(newSalesOrder.SalesOrder.TotalAmount)).ToString();
            salesOrder.SalesOrder.PaymentDue = decimal.Parse(GetDefaultIfNull(newSalesOrder.SalesOrder.PaymentDue)).ToString();

            salesOrder.SalesOrder.TotalDiscountPercentage = newSalesOrder.SalesOrder.TotalDiscountPercentage;
            salesOrder.SalesOrder.TotalDiscountAmount = newSalesOrder.SalesOrder.TotalDiscountAmount;
            salesOrder.SalesOrder.RecieveAmount = newSalesOrder.SalesOrder.RecieveAmount;
            salesOrder.SalesOrder.VATPercentage = newSalesOrder.SalesOrder.VATPercentage;
            salesOrder.SalesOrder.VATAmount = newSalesOrder.SalesOrder.VATAmount;
            salesOrder.SalesOrder.AdjAmount = newSalesOrder.SalesOrder.AdjAmount;
            salesOrder.SalesOrder.Remarks = newSalesOrder.SalesOrder.Remarks;
            salesOrder.SalesOrder.OrderDate = formCollection["OrderDate"];
            salesOrder.SalesOrder.RemindDate = formCollection["RemindDate"];
            salesOrder.SalesOrder.CustomerId = formCollection["CustomersId"];
            salesOrder.SalesOrder.TotalFractionAmt = newSalesOrder.SalesOrder.TotalFractionAmt;
            salesOrder.SalesOrder.IsSmsEnable = Convert.ToBoolean(newSalesOrder.SalesOrder.IsSmsEnable ? 1 : 0);
            //salesOrder.SalesOrder.DepotId = formCollection["DepotsId"];
            //salesOrder.SalesOrder.TermsType = newSalesOrder.SalesOrder.TermsType;
            //salesOrder.SalesOrder.EmployeeID = formCollection["EmployeesID"];

            //removing unchanged previous order
            salesOrder.SODetails.Where(x => !string.IsNullOrEmpty(x.SODetailId) && x.Status == default(int)).ToList()
                .ForEach(x => salesOrder.SODetails.Remove(x));

            if (!ControllerContext.RouteData.Values["action"].ToString().ToLower().Equals("edit"))
            {
                string invNo = _miscellaneousService.GetUniqueKey(x => int.Parse(x.InvoiceNo));
                salesOrder.SalesOrder.InvoiceNo = invNo;
            }
            if (Convert.ToDateTime(salesOrder.SalesOrder.RemindDate) > Convert.ToDateTime(salesOrder.SalesOrder.OrderDate))
                RemindDate = Convert.ToDateTime(salesOrder.SalesOrder.RemindDate);
            DataTable dtSalesOrder = CreateSalesOrderDataTable(salesOrder);
            DataTable dtSalesOrderDetail = CreateSODetailDataTable(salesOrder);
            var SystemInfo = _SysInfoService.GetSystemInformationByConcernId(User.Identity.GetConcernId());
            #region Log
            log.Info(new { SalesOrder = salesOrder.SalesOrder, SODetails = salesOrder.SODetails });
            #endregion
            if (ControllerContext.RouteData.Values["action"].ToString().ToLower().Equals("edit"))
            {
                Result = _salesOrderService.UpdateSalesOrderUsingSP(User.Identity.GetUserId<int>(), int.Parse(salesOrder.SalesOrder.SalesOrderId),
                 dtSalesOrder, dtSalesOrderDetail, Convert.ToInt32(salesOrder.SalesOrder.EmployeeID), true);
                if (Result)
                {
                    var invoiceSalesOrder = _mapper.Map<CreateSalesOrderViewModel, SOrder>(salesOrder.SalesOrder);
                    invoiceSalesOrder.SOrderDetails = _mapper.Map<ICollection<CreateSalesOrderDetailViewModel>,
                        ICollection<SOrderDetail>>(salesOrder.SODetails);

                    TempData["IsInvoiceReadyById"] = true;
                    TempData["OrderId"] = int.Parse(salesOrder.SalesOrder.SalesOrderId);
                    
                }
            }

            else
            {

                Result = _salesOrderService.AddSalesOrderUsingSP(dtSalesOrder, dtSalesOrderDetail, RemindDate, Convert.ToInt32(salesOrder.SalesOrder.EmployeeID), true);

                if (Result)
                {
                    var invoiceSalesOrder = _mapper.Map<CreateSalesOrderViewModel, SOrder>(salesOrder.SalesOrder);
                    invoiceSalesOrder.SOrderDetails = _mapper.Map<ICollection<CreateSalesOrderDetailViewModel>,
                        ICollection<SOrderDetail>>(salesOrder.SODetails);
                    TempData["salesInvoiceData"] = invoiceSalesOrder;


                    TempData["IsInvoiceReady"] = true;


                    #region Sales SMS Service
                    //var ProductNameList = (from sod in invoiceSalesOrder.SOrderDetails
                    //                       join p in _productService.GetAllIQueryable() on sod.ProductID equals p.ProductID
                    //                       select new { p.ProductName }).ToList();

                    if (SystemInfo.IsRetailSMSEnable == 1 && salesOrder.SalesOrder.IsSmsEnable == true)
                    {
                        if (SystemInfo.IsBanglaSmsEnable == 1)
                        {
                            var _oCustomer = _customerService.GetCustomerById(invoiceSalesOrder.CustomerID);
                            List<SMSRequest> sms = new List<SMSRequest>();
                            sms.Add(new SMSRequest()
                            {
                                MobileNo = _oCustomer.ContactNo,
                                CustomerID = _oCustomer.CustomerID,
                                //TransNumber = invoiceSalesOrder.InvoiceNo,
                                //Date = (DateTime)invoiceSalesOrder.InvoiceDate,
                                //PreviousDue = _oCustomer.TotalDue,
                                ReceiveAmount = (decimal)invoiceSalesOrder.RecAmount,
                                PresentDue = _oCustomer.TotalDue + invoiceSalesOrder.PaymentDue,
                                SMSType = EnumSMSType.SalesTime,
                                //SalesAmount = invoiceSalesOrder.TotalAmount,
                                CustomerCode = _oCustomer.Code,
                                //ProductNameList = ProductNameList.Select(i=>i.ProductName).ToList()
                            });

                            if (SystemInfo.SMSSendToOwner == 1)
                            {
                                sms.Add(new SMSRequest()
                                {
                                    MobileNo = SystemInfo.InsuranceContactNo,
                                    CustomerID = _oCustomer.CustomerID,
                                    TransNumber = invoiceSalesOrder.InvoiceNo,
                                    Date = (DateTime)invoiceSalesOrder.InvoiceDate,
                                    PreviousDue = _oCustomer.TotalDue,
                                    ReceiveAmount = (decimal)invoiceSalesOrder.RecAmount,
                                    PresentDue = _oCustomer.TotalDue + invoiceSalesOrder.PaymentDue,
                                    SMSType = EnumSMSType.SalesTime,
                                    SalesAmount = invoiceSalesOrder.TotalAmount,
                                    CustomerCode = _oCustomer.Code,
                                    //ProductNameList = ProductNameList.Select(i=>i.ProductName).ToList()
                                });
                            }

                            int concernId = User.Identity.GetConcernId();
                            decimal previousBalance;
                            SMSPaymentMaster smsAmountDetails = _smsBillPaymentBkashService.GetByConcernId(concernId);
                            previousBalance = smsAmountDetails.TotalRecAmt;
                            var sysInfos = _SysInfoService.GetSystemInformationByConcernId(User.Identity.GetConcernId());
                            decimal smsFee = sysInfos.smsCharge;
                            if (smsAmountDetails.TotalRecAmt > 1)
                            {
                                var response = SMSHTTPServiceBangla.SendSMS(EnumOnnoRokomSMSType.NumberSms, sms, previousBalance, SystemInfo, User.Identity.GetUserId<int>());
                                if (response != null || response.Count > 0)
                                {
                                    decimal smsBalanceCount = 0m;
                                    foreach (var item in response)
                                    {
                                        smsBalanceCount = smsBalanceCount + item.NoOfSMS;
                                    }
                                    #region udpate payment info                  
                                    decimal sysLastPayUpdateDate = smsBalanceCount * smsFee;
                                    smsAmountDetails.TotalRecAmt = smsAmountDetails.TotalRecAmt - Convert.ToDecimal(sysLastPayUpdateDate);
                                    _smsBillPaymentBkashService.Update(smsAmountDetails);
                                    _smsBillPaymentBkashService.Save();
                                    #endregion

                                    response.Select(x => { x.ConcernID = User.Identity.GetConcernId(); return x; }).ToList();
                                    _SMSStatusService.AddRange(response);
                                    _SMSStatusService.Save();
                                }
                            }


                        }
                        else
                        {
                            var _oCustomer = _customerService.GetCustomerById(invoiceSalesOrder.CustomerID);
                            List<SMSRequest> sms = new List<SMSRequest>();
                            sms.Add(new SMSRequest()
                            {
                                MobileNo = _oCustomer.ContactNo,
                                CustomerID = _oCustomer.CustomerID,
                                CustomerName = _oCustomer.Name,
                                TransNumber = invoiceSalesOrder.InvoiceNo,
                                Date = (DateTime)invoiceSalesOrder.InvoiceDate,
                                PreviousDue = _oCustomer.TotalDue,
                                ReceiveAmount = (decimal)invoiceSalesOrder.RecAmount,
                                PresentDue = _oCustomer.TotalDue + invoiceSalesOrder.PaymentDue,
                                SMSType = EnumSMSType.SalesTime,
                                SalesAmount = invoiceSalesOrder.TotalAmount,
                                CustomerCode = _oCustomer.Code,
                                //ProductNameList = ProductNameList.Select(i=>i.ProductName).ToList()
                            });

                            if (SystemInfo.SMSSendToOwner == 1)
                            {
                                sms.Add(new SMSRequest()
                                {
                                    MobileNo = SystemInfo.InsuranceContactNo,
                                    CustomerID = _oCustomer.CustomerID,
                                    CustomerName = _oCustomer.Name,
                                    TransNumber = invoiceSalesOrder.InvoiceNo,
                                    Date = (DateTime)invoiceSalesOrder.InvoiceDate,
                                    PreviousDue = _oCustomer.TotalDue,
                                    ReceiveAmount = (decimal)invoiceSalesOrder.RecAmount,
                                    PresentDue = _oCustomer.TotalDue + invoiceSalesOrder.PaymentDue,
                                    SMSType = EnumSMSType.SalesTime,
                                    SalesAmount = invoiceSalesOrder.TotalAmount,
                                    CustomerCode = _oCustomer.Code,
                                    //ProductNameList = ProductNameList.Select(i=>i.ProductName).ToList()
                                });
                            }

                            int concernId = User.Identity.GetConcernId();
                            decimal previousBalance;
                            SMSPaymentMaster smsAmountDetails = _smsBillPaymentBkashService.GetByConcernId(concernId);
                            previousBalance = smsAmountDetails.TotalRecAmt;
                            var sysInfos = _SysInfoService.GetSystemInformationByConcernId(User.Identity.GetConcernId());
                            decimal smsFee = sysInfos.smsCharge;

                            if (smsAmountDetails.TotalRecAmt > 1)
                            {
                                var response = SMSHTTPService.SendSMS(EnumOnnoRokomSMSType.NumberSms, sms, previousBalance, SystemInfo, User.Identity.GetUserId<int>());

                                if (response != null || response.Count > 0)
                                {
                                    decimal smsBalanceCount = 0m;
                                    foreach (var item in response)
                                    {
                                        smsBalanceCount = smsBalanceCount + item.NoOfSMS;
                                    }
                                    #region udpate payment info                  
                                    decimal sysLastPayUpdateDate = smsBalanceCount * smsFee;
                                    smsAmountDetails.TotalRecAmt = smsAmountDetails.TotalRecAmt - Convert.ToDecimal(sysLastPayUpdateDate);
                                    _smsBillPaymentBkashService.Update(smsAmountDetails);
                                    _smsBillPaymentBkashService.Save();
                                    #endregion

                                    response.Select(x => { x.ConcernID = User.Identity.GetConcernId(); return x; }).ToList();
                                    _SMSStatusService.AddRange(response);
                                    _SMSStatusService.Save();

                                }

                            }
                            else
                            {
                                AddToastMessage("", "SMS Balance is Low Plz Recharge your SMS Balance.", ToastType.Error);
                            }


                        }
                    }


                    #endregion

                }
            }



            //_salesOrderService.CorrectionStockData(User.Identity.GetConcernId());
            #region For POS Invoice
            //PrintInvoice oPriInvoice = new PrintInvoice();
            //oPriInvoice.print(salesOrder, _SisterConcern);
            #endregion

            if (Result)
                AddToastMessage("", "Order has been saved successfully.", ToastType.Success);
            else
                AddToastMessage("", "Order has been failed.", ToastType.Error);

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
            dtSalesOrder.Columns.Add("TotalFractionAmt", typeof(decimal));
            //dtSalesOrder.Columns.Add("EmployeeID", typeof(int));
            dtSalesOrder.Columns.Add("PrevDue", typeof(decimal));
            dtSalesOrder.Columns.Add("DepotId", typeof(int));
            dtSalesOrder.Columns.Add("Terms", typeof(int));
            dtSalesOrder.Columns.Add("PayCashAccountId", typeof(int));
            dtSalesOrder.Columns.Add("PayBankId", typeof(int));

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
            row["TotalDue"] = decimal.Parse(GetDefaultIfNull(salesOrder.SalesOrder.PaymentDue));
            row["AdjAmount"] = GetDefaultIfNull(salesOrder.SalesOrder.AdjAmount);
            row["Status"] = EnumSalesType.Advance;
            row["CustomerId"] = salesOrder.SalesOrder.CustomerId;
            row["ConcernId"] = User.Identity.GetConcernId();
            row["CreatedDate"] = DateTime.Now;
            row["CreatedBy"] = User.Identity.GetUserId<int>();
            row["TotalOffer"] = GetDefaultIfNull(salesOrder.SalesOrder.TotalOffer);
            row["NetDiscount"] = GetDefaultIfNull(salesOrder.SalesOrder.NetDiscount);
            row["Remarks"] = salesOrder.SalesOrder.Remarks;
            row["TotalFractionAmt"] = salesOrder.SalesOrder.TotalFractionAmt;
            //row["EmployeeID"] = salesOrder.SalesOrder.EmployeeID;
            row["PrevDue"] = salesOrder.SalesOrder.PrevDue;
            row["DepotId"] = 0;
            row["Terms"] = 0;
            row["PayCashAccountId"] = salesOrder.SalesOrder.PayCashAccountId.HasValue ? (object)salesOrder.SalesOrder.PayCashAccountId.GetValueOrDefault() : DBNull.Value;
            row["PayBankId"] = salesOrder.SalesOrder.PayBankId.HasValue ? (object)salesOrder.SalesOrder.PayBankId.GetValueOrDefault() : DBNull.Value;


            dtSalesOrder.Rows.Add(row);

            return dtSalesOrder;
        }

        private DataTable CreateSODetailDataTable(SalesOrderViewModel salesOrder)
        {
            DataTable dtSalesOrderDetail = new DataTable();
            dtSalesOrderDetail.Columns.Add("SOrderDetailID", typeof(int));
            dtSalesOrderDetail.Columns.Add("ProductId", typeof(int));
            dtSalesOrderDetail.Columns.Add("StockDetailId", typeof(int));
            dtSalesOrderDetail.Columns.Add("ColorId", typeof(int));
            dtSalesOrderDetail.Columns.Add("Status", typeof(int));
            dtSalesOrderDetail.Columns.Add("Quantity", typeof(decimal));
            dtSalesOrderDetail.Columns.Add("UnitPrice", typeof(decimal));
            dtSalesOrderDetail.Columns.Add("TAmount", typeof(decimal));
            dtSalesOrderDetail.Columns.Add("PPDisPer", typeof(decimal));
            dtSalesOrderDetail.Columns.Add("PPDisAmt", typeof(decimal));
            dtSalesOrderDetail.Columns.Add("MrpRate", typeof(decimal));
            dtSalesOrderDetail.Columns.Add("PPOffer", typeof(decimal));
            dtSalesOrderDetail.Columns.Add("Compressor", typeof(int));
            dtSalesOrderDetail.Columns.Add("Motor", typeof(int));
            dtSalesOrderDetail.Columns.Add("Panel", typeof(int));
            dtSalesOrderDetail.Columns.Add("Spareparts", typeof(int));
            dtSalesOrderDetail.Columns.Add("Service", typeof(int));

            dtSalesOrderDetail.Columns.Add("SFTRate", typeof(decimal));
            dtSalesOrderDetail.Columns.Add("TotalSFT", typeof(decimal));
            dtSalesOrderDetail.Columns.Add("FractionQty", typeof(decimal));
            dtSalesOrderDetail.Columns.Add("FractionAmt", typeof(decimal));
            dtSalesOrderDetail.Columns.Add("ActualSFT", typeof(decimal));
            dtSalesOrderDetail.Columns.Add("BonusQuantity", typeof(decimal));


            DataRow row = null;
            ProductWisePurchaseModel oProduct = null;
            int ProductID = 0;
            foreach (var item in salesOrder.SODetails)
            {
                row = dtSalesOrderDetail.NewRow();
                if (!string.IsNullOrEmpty(item.SODetailId))
                    row["SOrderDetailID"] = item.SODetailId;
                ProductID = Convert.ToInt32(item.ProductId);
                oProduct = _productService.GetAllProductIQueryable().FirstOrDefault(i => i.ProductID == ProductID);
                row["ProductId"] = item.ProductId;
                row["StockDetailId"] = item.StockDetailId;
                row["ColorId"] = item.ColorId;
                row["Status"] = item.Status;
                row["Quantity"] = item.Quantity;
                row["UnitPrice"] = Math.Round(Convert.ToDecimal(item.UnitPrice), 4);
                row["TAmount"] = item.UTAmount;
                row["PPDisPer"] = GetDefaultIfNull(item.PPDPercentage);
                row["PPDisAmt"] = GetDefaultIfNull(item.PPDAmount);
                row["MrpRate"] = Math.Round(Convert.ToDecimal(item.MRPRate), 4);
                row["PPOffer"] = GetDefaultIfNull(item.PPOffer);
                row["Compressor"] = 0;
                row["Motor"] = 0;
                row["Panel"] = 0;
                row["Spareparts"] = 0;
                row["Service"] = 0;
                row["SFTRate"] = item.RatePerArea;
                row["TotalSFT"] = item.TotalArea;
                row["FractionQty"] = item.FractionQty;
                row["FractionAmt"] = item.FractionAmt;
                row["ActualSFT"] = 0;
                row["BonusQuantity"] = 0;

                dtSalesOrderDetail.Rows.Add(row);
            }

            return dtSalesOrderDetail;
        }

        private bool IsForEdit(string previousAction)
        {
            return previousAction.Equals("edit");
        }

        private ActionResult ReturnCreateViewWithTempData()
        {

            ViewBag.ProductIds = GetAllProductsForDDL();
            ViewBag.CustomerIds = GetAllCustomerForDDL();
            ViewBag.EmployeeIds = GetAllEmployeeForDDL();
            //ViewBag.DepotIds = GetAllDepotForDDL();



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
                int EmployeeID = 0;
                if (User.IsInRole(EnumUserRoles.RetailManager.ToString()))
                {
                    var emp = _employeeService.GetAllEmployeeIQueryable().FirstOrDefault(i => i.Name.Equals("showroom"));
                    EmployeeID = emp != null ? emp.EmployeeID : 0;
                }
                return View(new SalesOrderViewModel
                {
                    SODetail = new CreateSalesOrderDetailViewModel(),
                    SODetails = new List<CreateSalesOrderDetailViewModel>(),
                    SalesOrder = new CreateSalesOrderViewModel { InvoiceNo = invNo, EmployeeID = EmployeeID.ToString() }
                });
            }
        }


        private List<TOIdNameDDL> GetAllProductsForDDL()
        {
            var products = _productService.GetAllProductFromDetailById();

            var vmProductDetails = _mapper.Map<IEnumerable<Tuple<int, string, string, decimal, string, string, string, Tuple<decimal?, string, decimal, int, int, string, string, Tuple<string, string, string, string, string, string, int, Tuple<int, decimal, decimal, decimal, decimal, decimal>>>>>, IEnumerable<GetProductViewModel>>(products).ToList();


            var vmProductGroupBY = (from vm in vmProductDetails
                                    join pu in _productUnitTypeService.GetAll() on vm.ProUnitTypeID equals pu.ProUnitTypeID
                                    join s in _sizeService.GetAll() on vm.SizeID equals s.SizeID
                                    group vm by new
                                    {
                                        vm.IMENo,
                                        vm.ProductId,
                                        vm.CategoryID,
                                        vm.ProductName,
                                        vm.ProductCode,

                                        vm.ColorId,
                                        vm.CategoryName,
                                        vm.ColorName,
                                        vm.ModelName,
                                        vm.GodownName,
                                        vm.CompanyName,
                                        vm.ProUnitTypeID,
                                        ChildUnit = pu.UnitName,
                                        ParentUnit = pu.Description,
                                        ConvertValue = vm.BundleQty == 0 ? pu.ConvertValue : vm.BundleQty,
                                        s.SizeID,
                                        SizeName = s.Description,
                                        vm.PurchaseCSft,
                                        vm.SalesCSft,
                                        vm.TotalSFT,
                                        vm.AdvSRate

                                    } into g
                                    select new GetProductViewModel
                                    {
                                        IMENo = g.Key.IMENo,
                                        ProductId = g.Key.ProductId,
                                        ProductCode = g.Key.ProductCode,
                                        ProductName = g.Key.ProductName,
                                        CategoryID = g.Key.CategoryID,

                                        CategoryName = g.Key.CategoryName,
                                        ColorName = g.Key.ColorName,
                                        ColorId = g.Key.ColorId,
                                        ModelName = g.Key.ModelName,
                                        StockDetailsId = g.Select(o => o.StockDetailsId).FirstOrDefault(),

                                        MRPRate = ((g.Select(o => o.AdvSRate).FirstOrDefault()) / g.Key.ConvertValue),
                                        AdvSRate = g.Select(o => o.AdvSRate).FirstOrDefault(),
                                        ParentMRP = g.Select(o => o.AdvSRate).FirstOrDefault(),
                                        MRPRate12 = g.Select(o => o.MRPRate12).FirstOrDefault(),
                                        CashSalesRate = g.Select(o => o.CashSalesRate).FirstOrDefault(),
                                        PWDiscount = g.Select(o => o.PWDiscount).FirstOrDefault(),
                                        PicturePath = g.Select(o => o.PicturePath).FirstOrDefault(),

                                        PreStock = g.Select(o => o.PreStock).FirstOrDefault(),
                                        OfferDescription = g.Select(o => o.OfferDescription).FirstOrDefault(),
                                        ProductType = g.Select(o => o.ProductType).FirstOrDefault(),
                                        CompressorWarrentyMonth = g.Select(o => o.CompressorWarrentyMonth).FirstOrDefault(),
                                        PanelWarrentyMonth = g.Select(o => o.PanelWarrentyMonth).FirstOrDefault(),

                                        MotorWarrentyMonth = g.Select(o => o.MotorWarrentyMonth).FirstOrDefault(),
                                        SparePartsWarrentyMonth = g.Select(o => o.SparePartsWarrentyMonth).FirstOrDefault(),
                                        ServiceWarrentyMonth = g.Select(o => o.ServiceWarrentyMonth).FirstOrDefault(),
                                        IsSelect = g.Select(o => o.IsSelect).FirstOrDefault(),
                                        Status = g.Select(o => o.Status).FirstOrDefault(),

                                        Quantity = g.Select(o => o.Quantity).FirstOrDefault(), //e.g.; gm
                                        GodownName = g.Key.GodownName,
                                        SizeID = g.Key.SizeID,
                                        SizeName = g.Key.SizeName,
                                        CompanyName = g.Key.CompanyName,

                                        ChildUnit = g.Key.ChildUnit,
                                        ConvertValue = g.Key.ConvertValue,
                                        ParentUnit = g.Key.ParentUnit,
                                        PurchaseCSft = g.Key.PurchaseCSft,
                                        SalesCSft = g.Key.SalesCSft,
                                        ParentQty = (int)Math.Truncate(g.Select(o => o.PreStock).FirstOrDefault() / g.Key.ConvertValue), //e.g. KG
                                        ChildQty = (int)(g.Select(o => o.PreStock).FirstOrDefault() % g.Key.ConvertValue), //e.g. gm

                                        TotalSFT = g.Key.TotalSFT
                                    }).OrderBy(p => p.ProductId).Select(s => new TOIdNameDDL
                                    {
                                        Id = s.ProductId,
                                        Name = s.ProductName + ", " + s.CategoryName + " (" + s.SizeName + ")"
                                    });
            return vmProductGroupBY.ToList();
        }

        private List<TOIdNameDDL> GetAllCustomerForDDL()
        {
            int CuserId = System.Web.HttpContext.Current.User.Identity.GetUserId<int>();
            int CEmpID = 0;

            if (User.IsInRole(ConstantData.ROLE_MOBILE_USER))
            {
                var user = _UserService.GetUserById(CuserId);
                CEmpID = user.EmployeeID;

                var Ccustomers = _customerService.GetAllCustomerByEmpNew(CEmpID);
                var vmCustomers = _mapper.Map<IEnumerable<Customer>, IEnumerable<CreateCustomerViewModel>>(Ccustomers).Select(s => new TOIdNameDDL
                {
                    Id = int.Parse(s.Id),
                    Name = s.Name + "(" + s.Code + "), " + "Mobile:" + s.ContactNo + ", " + "Add:" + s.Address + ", " + "Prop:" + s.CompanyName
                }).ToList();
                return vmCustomers;
            }
            else
            {
                var customers = _customerService.GetAllCustomerNew(User.Identity.GetConcernId()).Select(s => new TOIdNameDDL
                {
                    Id = s.Id,
                    Name = s.Name + "(" + s.Code + "), " + "Mobile:" + s.ContactNo + ", " + "Add:" + s.Address + ", " + "Prop:" + s.CompanyName
                }).ToList();
                return customers;
            }
        }

        private ActionResult HandleSalesOrder(SalesOrderViewModel newSalesOrder, FormCollection formCollection)
        {
            ViewBag.ProductIds = GetAllProductsForDDL();
            ViewBag.CustomerIds = GetAllCustomerForDDL();
            ViewBag.EmployeeIds = GetAllEmployeeForDDL();
            //ViewBag.DepotIds = GetAllDepotForDDL();
            if (newSalesOrder != null)
            {
                SalesOrderViewModel salesOrder = (SalesOrderViewModel)TempData.Peek("salesOrderViewModel");
                salesOrder = salesOrder ?? new SalesOrderViewModel()
                {
                    SalesOrder = newSalesOrder.SalesOrder
                };
                salesOrder.SODetail = new CreateSalesOrderDetailViewModel();

                if (formCollection.Get("addButton") != null)
                {
                    CheckAndAddModelErrorForAdd(newSalesOrder, salesOrder, formCollection);
                    if (!ModelState.IsValid)
                    {
                        salesOrder.SODetails = salesOrder.SODetails ?? new List<CreateSalesOrderDetailViewModel>();
                        return View("Create", salesOrder);
                    }
                    var product = _productService.GetProductById(int.Parse(newSalesOrder.SODetail.ProductId));
                    if (salesOrder.SODetails != null &&
                        salesOrder.SODetails.Any(x => x.Status != EnumStatus.Updated && x.Status != EnumStatus.Deleted &&
                        x.IMENo.Equals(newSalesOrder.SODetail.IMENo) && x.ProductId.Equals(newSalesOrder.SODetail.ProductId)))
                    {
                        AddToastMessage(string.Empty, "This product already exists in the order", ToastType.Error);
                        return View("Create", salesOrder);
                    }

                    AddToOrder(newSalesOrder, salesOrder, formCollection);
                    ModelState.Clear();
                    return View("Create", salesOrder);
                }
                else if (formCollection.Get("submitButton") != null)
                {
                    CheckAndAddModelErrorForSave(newSalesOrder, salesOrder, formCollection);
                    //decimal calGrandtotal = salesOrder.SODetails.Where(i => i.Status != EnumStatus.Deleted).Sum(i => Convert.ToDecimal(i.UnitPrice) * Convert.ToDecimal(i.Quantity))
                    //                        + newSalesOrder.SalesOrder.TotalFractionAmt;

                    decimal calGrandTotal = Convert.ToDecimal(newSalesOrder.SalesOrder.TotalAmount) + Convert.ToDecimal(newSalesOrder.SalesOrder.NetDiscount) + Convert.ToDecimal(newSalesOrder.SalesOrder.AdjAmount) - Convert.ToDecimal(newSalesOrder.SalesOrder.VATAmount);

                    if (Convert.ToDecimal(newSalesOrder.SalesOrder.GrandTotal) != calGrandTotal)
                    {
                        TempData["salesOrderViewModel"] = null;
                        AddToastMessage("", "Order has been failed. Please try again.", ToastType.Error);
                        return RedirectToAction("Index");
                    }

                    if (!ModelState.IsValid)
                    {
                        //IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                        salesOrder.SODetails = salesOrder.SODetails ?? new List<CreateSalesOrderDetailViewModel>();
                        return View("Create", salesOrder);
                    }
                    bool Result = SaveOrder(newSalesOrder, salesOrder, formCollection);
                    ModelState.Clear();
                    TempData["salesOrderViewModel"] = null;

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
            TempData["OrderId"] = orderId;
            return RedirectToAction("Index");
        }


        [HttpGet]
        [Authorize]
        public JsonResult GetProductDetailByIMEINo(string imeiNo)
        {
            if (!string.IsNullOrEmpty(imeiNo))
            {

                if (User.IsInRole(ConstantData.ROLE_MOBILE_USER))
                {
                    //int EmployeeID = ConstantData.GetEmployeeIDByUSerID(User.Identity.GetUserId<int>());
                    var user = _UserService.GetUserById(User.Identity.GetUserId<int>());
                    int EmployeeID = user.EmployeeID;
                    var customProductDetails = _productService.SRWiseGetAllProductFromDetail(EmployeeID);
                    var vmProductDetails = _mapper.Map<IEnumerable<Tuple<int, string, string, decimal, string, string, string, Tuple<decimal?, string, decimal, int, int, string, string>>>, IEnumerable<GetProductViewModel>>(customProductDetails);

                    var vmProduct = vmProductDetails.FirstOrDefault(x => x.IMENo.ToLower().Equals(imeiNo.Trim().ToLower()));
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

                    return Json(false, JsonRequestBehavior.AllowGet);

                }
                else
                {
                    var customProductDetails = _productService.GetAllProductFromDetail();
                    var vmProductDetails = _mapper.Map<IEnumerable<Tuple<int, string, string, decimal, string, string, string,
                        Tuple<decimal?, string, decimal, int, int, string, string, Tuple<string, string, string, string, string, string, int, Tuple<int, decimal, decimal, decimal, decimal, int>>>>>, IEnumerable<GetProductViewModel>>(customProductDetails);

                    var vmProduct = vmProductDetails.FirstOrDefault(x => x.IMENo.ToLower().Equals(imeiNo.Trim().ToLower()));
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
                        return Json(false, JsonRequestBehavior.AllowGet);
                    }
                }


            }
            else
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }

        }



        [HttpGet]
        [Authorize]
        [Route("index")]
        public async Task<ActionResult> SOrder(int id)
        {
            int userId = System.Web.HttpContext.Current.User.Identity.GetUserId<int>();
            var Concern = _SisterConcern.GetSisterConcernById(id);
            var DateRange = GetFirstAndLastDateOfMonth(DateTime.Today);
            ViewBag.FromDate = DateRange.Item1;
            ViewBag.ToDate = DateRange.Item2;
            ViewBag.ConcernID = id;
            ViewBag.ConcernName = Concern.Name;
            if (TempData.ContainsKey("ConcernSOrderData"))
            {
                var Sorder = (List<GetSalesOrderViewModel>)TempData["ConcernSOrderData"];
                return View(Sorder);
            }
            List<EnumSalesType> status = new List<EnumSalesType>();
            status.Add(EnumSalesType.Sales);
            status.Add(EnumSalesType.Advance);
            var customSO = _salesOrderService.GetAllAdvanceSalesOrderAsyncByUserID(userId, DateRange.Item1, DateRange.Item2, status);
            var vmSO = _mapper.Map<IEnumerable<Tuple<int, string, DateTime, string, string, decimal, EnumSalesType, Tuple<string, bool>>>,
        IEnumerable<GetSalesOrderViewModel>>(await customSO);
            return View(vmSO);

        }
        [HttpPost]
        [Authorize]
        public async Task<ActionResult> SOrder(FormCollection formCollection)
        {
            int ConcernID = 0;
            if (!string.IsNullOrEmpty(formCollection["FromDate"]))
                ViewBag.FromDate = Convert.ToDateTime(formCollection["FromDate"]);
            if (!string.IsNullOrEmpty(formCollection["ToDate"]))
                ViewBag.ToDate = Convert.ToDateTime(formCollection["ToDate"]);
            if (!string.IsNullOrEmpty(formCollection["ConcernID"]))
                ConcernID = Convert.ToInt32(formCollection["ConcernID"]);
            var DateRange = GetFirstAndLastDateOfMonth(DateTime.Today);
            ViewBag.FromDate = DateRange.Item1;
            ViewBag.ToDate = DateRange.Item2;

            List<EnumSalesType> status = new List<EnumSalesType>();
            status.Add(EnumSalesType.Sales);
            status.Add(EnumSalesType.Pending);
            var customSO = _salesOrderService.GetAllAdvanceSalesOrderAsync(DateRange.Item1, DateRange.Item2, status, IsVATManager(), User.Identity.GetConcernId());
            var vmSO = _mapper.Map<IEnumerable<Tuple<int, string, DateTime, string, string, decimal, EnumSalesType, Tuple<string, bool, string>>>,
            IEnumerable<GetSalesOrderViewModel>>(await customSO);
            return View(vmSO);
            //return RedirectToAction("SOrder", ConcernID);
        }


        //public ActionResult AdminSalesReportDetails()
        //{
        //    @ViewBag.Concerns = new SelectList(_SisterConcern.GetAll(), "ConcernID", "Name");
        //    return View();
        //}


        [HttpGet]
        [Authorize]
        public JsonResult GetProductInfoById(int productId)
        {
            var products = _productService.GetAllProductFromDetailById(productId);

            var vmProductDetails = _mapper.Map<IEnumerable<Tuple<int, string, string, decimal, string, string, string, Tuple<decimal?, string, decimal, int, int, string, string, Tuple<string, string, string, string, string, string, int, Tuple<int, decimal, decimal, decimal, decimal, decimal>>>>>, IEnumerable<GetProductViewModel>>(products).ToList();


            var vmProductGroupBY = (from vm in vmProductDetails
                                    join pu in _productUnitTypeService.GetAll() on vm.ProUnitTypeID equals pu.ProUnitTypeID
                                    join s in _sizeService.GetAll() on vm.SizeID equals s.SizeID
                                    group vm by new
                                    {
                                        vm.IMENo,
                                        vm.ProductId,
                                        vm.CategoryID,
                                        vm.ProductName,
                                        vm.ProductCode,

                                        vm.ColorId,
                                        vm.CategoryName,
                                        vm.ColorName,
                                        vm.ModelName,
                                        vm.GodownName,
                                        vm.CompanyName,
                                        vm.ProUnitTypeID,
                                        ChildUnit = pu.UnitName,
                                        ParentUnit = pu.Description,
                                        ConvertValue = vm.BundleQty == 0 ? pu.ConvertValue : vm.BundleQty,
                                        s.SizeID,
                                        SizeName = s.Description,
                                        vm.PurchaseCSft,
                                        vm.SalesCSft,
                                        vm.TotalSFT,
                                        vm.AdvSRate

                                    } into g
                                    select new GetProductViewModel
                                    {
                                        IMENo = g.Key.IMENo,
                                        ProductId = g.Key.ProductId,
                                        ProductCode = g.Key.ProductCode,
                                        ProductName = g.Key.ProductName,
                                        CategoryID = g.Key.CategoryID,

                                        CategoryName = g.Key.CategoryName,
                                        ColorName = g.Key.ColorName,
                                        ColorId = g.Key.ColorId,
                                        ModelName = g.Key.ModelName,
                                        StockDetailsId = g.Select(o => o.StockDetailsId).FirstOrDefault(),

                                        MRPRate = ((g.Select(o => o.AdvSRate).FirstOrDefault()) / g.Key.ConvertValue),
                                        AdvSRate = g.Select(o => o.AdvSRate).FirstOrDefault(),
                                        ParentMRP = g.Select(o => o.AdvSRate).FirstOrDefault(),
                                        MRPRate12 = g.Select(o => o.MRPRate12).FirstOrDefault(),
                                        CashSalesRate = g.Select(o => o.CashSalesRate).FirstOrDefault(),
                                        PWDiscount = g.Select(o => o.PWDiscount).FirstOrDefault(),
                                        PicturePath = g.Select(o => o.PicturePath).FirstOrDefault(),

                                        PreStock = g.Select(o => o.PreStock).FirstOrDefault(),
                                        OfferDescription = g.Select(o => o.OfferDescription).FirstOrDefault(),
                                        ProductType = g.Select(o => o.ProductType).FirstOrDefault(),
                                        CompressorWarrentyMonth = g.Select(o => o.CompressorWarrentyMonth).FirstOrDefault(),
                                        PanelWarrentyMonth = g.Select(o => o.PanelWarrentyMonth).FirstOrDefault(),

                                        MotorWarrentyMonth = g.Select(o => o.MotorWarrentyMonth).FirstOrDefault(),
                                        SparePartsWarrentyMonth = g.Select(o => o.SparePartsWarrentyMonth).FirstOrDefault(),
                                        ServiceWarrentyMonth = g.Select(o => o.ServiceWarrentyMonth).FirstOrDefault(),
                                        IsSelect = g.Select(o => o.IsSelect).FirstOrDefault(),
                                        Status = g.Select(o => o.Status).FirstOrDefault(),

                                        Quantity = g.Select(o => o.Quantity).FirstOrDefault(), //e.g.; gm
                                        GodownName = g.Key.GodownName,
                                        SizeID = g.Key.SizeID,
                                        SizeName = g.Key.SizeName,
                                        CompanyName = g.Key.CompanyName,

                                        ChildUnit = g.Key.ChildUnit,
                                        ConvertValue = g.Key.ConvertValue,
                                        ParentUnit = g.Key.ParentUnit,
                                        PurchaseCSft = g.Key.PurchaseCSft,
                                        SalesCSft = g.Key.SalesCSft,
                                        ParentQty = (int)Math.Truncate(g.Select(o => o.PreStock).FirstOrDefault() / g.Key.ConvertValue), //e.g. KG
                                        ChildQty = (int)(g.Select(o => o.PreStock).FirstOrDefault() % g.Key.ConvertValue), //e.g. gm

                                        TotalSFT = g.Key.TotalSFT
                                    }).OrderBy(p => p.ProductId).FirstOrDefault();



            return Json(vmProductGroupBY, JsonRequestBehavior.AllowGet);

        }


        [HttpGet]
        [Authorize]
        public JsonResult GetCustomerInfoById(int customerId)
        {
            int CuserId = System.Web.HttpContext.Current.User.Identity.GetUserId<int>();
            int CEmpID = 0;

            if (User.IsInRole(ConstantData.ROLE_MOBILE_USER))
            {
                var user = _UserService.GetUserById(CuserId);
                CEmpID = user.EmployeeID;

                var Ccustomers = _customerService.GetAllCustomerByEmpNew(CEmpID, customerId);
                var vmCustomers = _mapper.Map<IEnumerable<Customer>, IEnumerable<CreateCustomerViewModel>>(Ccustomers).FirstOrDefault();
                return Json(vmCustomers, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var customers = _customerService.GetAllCustomerNew(User.Identity.GetConcernId(), customerId).FirstOrDefault();
                return Json(customers, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        [Authorize]
        public ActionResult ApproveSale(int orderId)
        {
            if (orderId > 0)
            {
                if(_salesOrderService.ApproveAdvanceSalesUsingSP(orderId))
                    AddToastMessage("", "Order approved successfully!", ToastType.Success);
            }
            else
                AddToastMessage("", "No order found to approve!", ToastType.Error);

            return RedirectToAction("Index");
        }

        [HttpGet]
        [Authorize]
        public JsonResult GetEmployeeInfoById(int employeeId)
        {

            var employees = _employeeService.GetAllEmployeeNew(User.Identity.GetConcernId(), employeeId).FirstOrDefault();
            return Json(employees, JsonRequestBehavior.AllowGet);

        }
        private List<TOIdNameDDL> GetAllEmployeeForDDL()
        {

            var employees = _employeeService.GetAllEmployeeNew(User.Identity.GetConcernId()).Select(s => new TOIdNameDDL
            {
                Id = s.Id,
                Name = s.Name + "(" + s.Code + ")"
            }).ToList();
            return employees;

        }

        //[HttpGet]
        //[Authorize]
        //public JsonResult GetDepotInfoById(int depotId)
        //{

        //    var depots = _DepotService.GetAllDepotNew(User.Identity.GetConcernId(), depotId).FirstOrDefault();
        //    return Json(depots, JsonRequestBehavior.AllowGet);

        //}
        //private List<TOIdNameDDL> GetAllDepotForDDL()
        //{

        //    var depots = _DepotService.GetAllDepotNew(User.Identity.GetConcernId()).Select(s => new TOIdNameDDL
        //    {
        //        Id = s.Id,
        //        Name = s.Name + "(" + s.Code + ")"
        //    }).ToList();
        //    return depots;

        //}

    }
}