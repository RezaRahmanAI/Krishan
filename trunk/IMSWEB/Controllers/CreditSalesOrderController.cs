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
using System.Data.SqlTypes;
using log4net;

namespace IMSWEB.Controllers
{
    [Authorize]
    [RoutePrefix("credit-sales-order")]
    public class CreditSalesOrderController : CoreController
    {
        ICreditSalesOrderService _creditSalesOrderService;
        IPurchaseOrderDetailService _purchaseOrderDetailService;
        IPOProductDetailService _pOProductDetailService;
        IStockService _stockService;
        ICustomerService _customerService;
        IEmployeeService _employeeService;
        IStockDetailService _stockDetailService;
        IProductService _productService;
        IMiscellaneousService<CreditSale> _miscellaneousService;
        IMapper _mapper;
        ISystemInformationService _SysInfoService;
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public CreditSalesOrderController(IErrorService errorService,
            ICreditSalesOrderService salesOrderService, IPurchaseOrderDetailService purchaseOrderDetailService,
            IPOProductDetailService pOProductDetailService, IStockService stockService,
            IStockDetailService stockDetailService, ICustomerService customerService, IEmployeeService employeeService,
            IMiscellaneousService<CreditSale> miscellaneousService, IProductService productService,
            IMapper mapper, ISystemInformationService SysInfoService)
            : base(errorService)
        {
            _creditSalesOrderService = salesOrderService;
            _purchaseOrderDetailService = purchaseOrderDetailService;
            _pOProductDetailService = pOProductDetailService;
            _stockService = stockService;
            _stockDetailService = stockDetailService;
            _customerService = customerService;
            _employeeService = employeeService;
            _miscellaneousService = miscellaneousService;
            _productService = productService;
            _mapper = mapper;
            _SysInfoService = SysInfoService;
        }

        [HttpGet]
        [Authorize]
        [Route("index")]
        public async Task<ActionResult> Index()
        {
            TempData["creditSalesOrderViewModel"] = null;
            var DateRange = GetFirstAndLastDateOfMonth(DateTime.Today);
            ViewBag.FromDate = DateRange.Item1;
            ViewBag.ToDate = DateRange.Item2;
            var customSO = _creditSalesOrderService.GetAllSalesOrderAsync(ViewBag.FromDate, ViewBag.ToDate);
            var vmSO = _mapper.Map<IEnumerable<Tuple<int, string, DateTime, string, string, decimal, EnumSalesType>>,
                IEnumerable<GetCreditSalesOrderViewModel>>(await customSO);
            return View(vmSO);
        }
        [HttpPost]
        [Authorize]
        public async Task<ActionResult> Index(FormCollection formCollection)
        {
            TempData["creditSalesOrderViewModel"] = null;
            if (!string.IsNullOrEmpty(formCollection["FromDate"]))
                ViewBag.FromDate = Convert.ToDateTime(formCollection["FromDate"]);
            if (!string.IsNullOrEmpty(formCollection["ToDate"]))
                ViewBag.ToDate = Convert.ToDateTime(formCollection["ToDate"]);
            var customSO = _creditSalesOrderService.GetAllSalesOrderAsync(ViewBag.FromDate, ViewBag.ToDate);
            var vmSO = _mapper.Map<IEnumerable<Tuple<int, string, DateTime, string, string, decimal, EnumSalesType>>,
                IEnumerable<GetCreditSalesOrderViewModel>>(await customSO);
            return View(vmSO);
        }
        [HttpGet]
        [Authorize]
        [Route("create")]
        public ActionResult Create()
        {
            CreditSalesOrderViewModel salesOrder = (CreditSalesOrderViewModel)TempData.Peek("creditSalesOrderViewModel");
            if (salesOrder != null)
            {
                return View(salesOrder);
            }
            else
            {
                string invNo = _miscellaneousService.GetUniqueKey(x => int.Parse(x.InvoiceNo));
                return View(new CreditSalesOrderViewModel
                {
                    SODetail = new CreateCreditSalesOrderDetailViewModel(),
                    SODetails = new List<CreateCreditSalesOrderDetailViewModel>(),
                    SOSchedules = new List<CreateCreditSalesSchedules>(),
                    SalesOrder = new CreateCreditSalesOrderViewModel { InvoiceNo = invNo }
                });
            }
        }

        [HttpPost]
        [Authorize]
        [Route("create/returnUrl")]
        public ActionResult Create(CreditSalesOrderViewModel newSalesOrder, FormCollection formCollection, string returnUrl)
        {
            if (newSalesOrder != null)
            {
                CreditSalesOrderViewModel salesOrder = (CreditSalesOrderViewModel)TempData.Peek("creditSalesOrderViewModel");
                salesOrder = salesOrder ?? newSalesOrder;
                if (formCollection.Get("addButton") != null)
                {
                    CheckAndAddModelErrorForAdd(newSalesOrder, salesOrder, formCollection);
                    if (!ModelState.IsValid)
                    {
                        salesOrder.SODetails = salesOrder.SODetails ?? new List<CreateCreditSalesOrderDetailViewModel>();
                        salesOrder.SOSchedules = salesOrder.SOSchedules ?? new List<CreateCreditSalesSchedules>();
                        return View(salesOrder);
                    }
                    if (salesOrder.SODetails != null &&
                        salesOrder.SODetails.Any(x => x.IMENo.Equals(newSalesOrder.SODetail.IMENo)))
                    {
                        AddToastMessage(string.Empty, "This product already exists in the order", ToastType.Error);
                        return View(salesOrder);
                    }

                    AddToOrder(newSalesOrder, salesOrder, formCollection);
                    ModelState.Clear();
                    return View(salesOrder);
                }
                else if (formCollection.Get("submitButton") != null)
                {
                    CheckAndAddModelErrorForSave(newSalesOrder, salesOrder, formCollection);
                    if (!ModelState.IsValid)
                    {
                        salesOrder.SODetails = salesOrder.SODetails ?? new List<CreateCreditSalesOrderDetailViewModel>();
                        salesOrder.SOSchedules = salesOrder.SOSchedules ?? new List<CreateCreditSalesSchedules>();
                        return View(salesOrder);
                    }


                    SaveOrder(newSalesOrder, salesOrder, formCollection);
                    TempData["creditSalesOrderViewModel"] = null;
                    TempData["IsCInvoiceReady"] = true;
                    ModelState.Clear();

                    //mapping for credit sales ivoice
                    var invoiceSalesOrder = _mapper.Map<CreateCreditSalesOrderViewModel, CreditSale>(salesOrder.SalesOrder);
                    invoiceSalesOrder.CreditSaleDetails = _mapper.Map<ICollection<CreateCreditSalesOrderDetailViewModel>,
                        ICollection<CreditSaleDetails>>(salesOrder.SODetails);
                    invoiceSalesOrder.CreditSalesSchedules = _mapper.Map<ICollection<CreateCreditSalesSchedules>,
                       ICollection<CreditSalesSchedule>>(salesOrder.SOSchedules);
                    TempData["CreditSalesInvoiceData"] = invoiceSalesOrder;

                    return RedirectToAction("Index");
                }
                else if (formCollection.Get("installmentButton") != null)
                {
                    CheckAndAddModelErrorForSave(newSalesOrder, salesOrder, formCollection);
                    if (string.IsNullOrEmpty(newSalesOrder.SalesOrder.InstallmentNo) ||
                        decimal.Parse(GetDefaultIfNull(newSalesOrder.SalesOrder.InstallmentNo)) <= 0)
                        ModelState.AddModelError("SalesOrder.InstallmentNo", "Installments is required");

                    if (!ModelState.IsValid)
                    {
                        salesOrder.SODetails = salesOrder.SODetails ?? new List<CreateCreditSalesOrderDetailViewModel>();
                        salesOrder.SOSchedules = salesOrder.SOSchedules ?? new List<CreateCreditSalesSchedules>();
                        return View(salesOrder);
                    }
                    CalculateInstallments(newSalesOrder, salesOrder, formCollection);
                    ModelState.Clear();
                    return View(salesOrder);
                }
                else if (formCollection.Get("paymentButton") != null)
                {
                    //if (string.IsNullOrEmpty(newSalesOrder.SalesOrder.InstallmentAmount) ||
                    //    decimal.Parse(GetDefaultIfNull(newSalesOrder.SalesOrder.InstallmentAmount)) <= 0)
                    //    ModelState.AddModelError("SalesOrder.InstallmentAmount", "Installment is required");
                    //if (!ModelState.IsValid)
                    //    return View(salesOrder);
                    AddPaymentModelError(salesOrder, newSalesOrder, formCollection);


                    if (!ModelState.IsValid)
                        return View("Create", salesOrder);

                    InstallmentPayment(newSalesOrder, salesOrder, formCollection);
                    ModelState.Clear();
                    return View(salesOrder);
                }
                else
                {
                    return View(new CreditSalesOrderViewModel
                    {
                        SODetail = new CreateCreditSalesOrderDetailViewModel(),
                        SODetails = new List<CreateCreditSalesOrderDetailViewModel>(),
                        SOSchedules = new List<CreateCreditSalesSchedules>(),
                        SalesOrder = new CreateCreditSalesOrderViewModel()
                    });
                }
            }
            else
            {
                AddToastMessage("", "No order data found to save.", ToastType.Error);
                return RedirectToAction("Create");
            }
        }

        private void AddPaymentModelError(CreditSalesOrderViewModel salesOrder, CreditSalesOrderViewModel NewsalesOrder, FormCollection formCollection)
        {
            if (!IsDateValid(Convert.ToDateTime(formCollection["Paydate"])))
            {
                ModelState.AddModelError("SalesOrder.OrderDate", "Back dated entry is not valid.");
            }
            if (NewsalesOrder.SalesOrder.IsAllPaid == false)
            {

                string scheduleDate = formCollection["scheduleDate"];
                CreateCreditSalesSchedules creditSalesSchedule = salesOrder.SOSchedules.FirstOrDefault(x => DateTime.Parse(x.ScheduleDate) ==
                    DateTime.Parse(scheduleDate) && x.PaymentStatus.Equals("Due"));
                if (salesOrder.SOSchedules.Any(i => i.PaymentStatus.Equals("Due") && int.Parse(i.ScheduleNo) < int.Parse(creditSalesSchedule.ScheduleNo)))
                {
                    ModelState.AddModelError("SalesOrder.InstallmentAmount", "Please pay Previous Installment First.");
                    AddToastMessage("", "Please pay Previous Installment First.", ToastType.Error);
                }
                if (decimal.Parse(NewsalesOrder.SalesOrder.InstallmentAmount) > decimal.Parse(salesOrder.SalesOrder.PaymentDue))
                {
                    ModelState.AddModelError("SalesOrder.InstallmentAmount", "Installment amount can't be more than remaining amount.");
                    AddToastMessage("", "Installment amount can't be more than remaining amount.", ToastType.Error);
                }
            }

        }

        [HttpGet]
        [Authorize]
        [Route("edit")]
        public ActionResult Edit(int orderId)
        {
            //credit sales sp for calculating penalty schedules
            //_salesOrderService.CalculatePenaltySchedules(ConcernID);

            var salesOrder = _creditSalesOrderService.GetSalesOrderById(orderId);
            var saleOrderDetails = _creditSalesOrderService.GetCustomSalesOrderDetails(orderId);
            var saleOderSchedules = _creditSalesOrderService.GetSalesOrderSchedules(orderId);

            var vm = new CreditSalesOrderViewModel();
            vm.SalesOrder = _mapper.Map<CreditSale, CreateCreditSalesOrderViewModel>(salesOrder);
            vm.SODetails = _mapper.Map<ICollection<Tuple<int, int, int, int, decimal, decimal, decimal, Tuple<decimal,
                string, string, int, string>>>, ICollection<CreateCreditSalesOrderDetailViewModel>>(saleOrderDetails.ToList());
            vm.SOSchedules = _mapper.Map<ICollection<CreditSalesSchedule>, ICollection<CreateCreditSalesSchedules>>(saleOderSchedules.ToList());

            TempData["creditSalesOrderViewModel"] = vm;
            ViewBag.Status = "data-attr='calculated'";
            return View("Create", vm);
        }

        [HttpPost]
        [Authorize]
        [Route("edit/returnUrl")]
        public ActionResult Edit(CreditSalesOrderViewModel newSalesOrder, FormCollection formCollection, string returnUrl)
        {
            if (newSalesOrder != null)
            {
                CreditSalesOrderViewModel salesOrder = (CreditSalesOrderViewModel)TempData.Peek("creditSalesOrderViewModel");
                if (formCollection.Get("paymentButton") != null)
                {
                    //if (string.IsNullOrEmpty(newSalesOrder.SalesOrder.InstallmentAmount) ||
                    //    decimal.Parse(GetDefaultIfNull(newSalesOrder.SalesOrder.InstallmentAmount)) <= 0)
                    //    ModelState.AddModelError("SalesOrder.InstallmentAmount", "Installment is required");

                    AddPaymentModelError(salesOrder, newSalesOrder, formCollection);

                    if (!ModelState.IsValid)
                        return View("Create", salesOrder);

                    InstallmentPayment(newSalesOrder, salesOrder, formCollection);
                    ModelState.Clear();
                    //return View("Create", salesOrder);
                    return RedirectToAction("Edit", new { orderId = salesOrder.SalesOrder.SalesOrderId });
                }
                else if (formCollection.Get("updateBtn") != null) // To Increase Installment
                {
                    var AllCreditSalesSchedule = _creditSalesOrderService.GetSalesOrderSchedules(int.Parse(salesOrder.SalesOrder.SalesOrderId));
                    var DueCreditSalesSchedule = AllCreditSalesSchedule.Where(i => i.PaymentStatus == "Due");
                    var PaidCreditSalesSchedule = AllCreditSalesSchedule.Where(i => i.PaymentStatus == "Paid");
                    int ScheduleNo = 0;
                    ScheduleNo = PaidCreditSalesSchedule.Count();

                    log.Info(new { InstallmentIncrease = newSalesOrder.SalesOrder.InstallmentNo, PreviousInstallment = AllCreditSalesSchedule.Count(), DueInstllemntNo = DueCreditSalesSchedule.Count(), PaidInstallment = PaidCreditSalesSchedule.Count() });

                    foreach (var item in DueCreditSalesSchedule)
                    {
                        _creditSalesOrderService.DeleteSchedule(item);
                    }
                    //newSalesOrder.SalesOrder.PaymentDue = (decimal.Parse(GetDefaultIfNull(newSalesOrder.SalesOrder.PaymentDue)) + decimal.Parse(GetDefaultIfNull(newSalesOrder.SalesOrder.ExtendTimeInterestAmount))).ToString();

                    var CreditSale = _creditSalesOrderService.GetSalesOrderById(int.Parse(salesOrder.SalesOrder.SalesOrderId));
                    newSalesOrder.SalesOrder.PaymentDue = (CreditSale.Remaining + decimal.Parse(GetDefaultIfNull(newSalesOrder.SalesOrder.ExtendTimeInterestAmount))).ToString();

                    newSalesOrder.SalesOrder.InterestAmount = (CreditSale.InterestAmount + decimal.Parse(GetDefaultIfNull(newSalesOrder.SalesOrder.ExtendTimeInterestAmount))).ToString();

                    CalculateInstallments(newSalesOrder, salesOrder, formCollection);
                    var Customer = _customerService.GetCustomerById(int.Parse(salesOrder.SalesOrder.CustomerId));
                    if (Customer != null)
                    {
                        Customer.TotalDue = Customer.TotalDue - CreditSale.Remaining;
                        Customer.TotalDue += decimal.Parse(GetDefaultIfNull(salesOrder.SalesOrder.PaymentDue));
                    }
                    if (CreditSale != null)
                    {
                        CreditSale.Remaining = Convert.ToDecimal(newSalesOrder.SalesOrder.PaymentDue);
                        CreditSale.InterestAmount = Convert.ToDecimal(newSalesOrder.SalesOrder.InterestAmount);
                        CreditSale.NoOfInstallment = PaidCreditSalesSchedule.Count() + int.Parse(GetDefaultIfNull(newSalesOrder.SalesOrder.InstallmentNo));
                        CreditSale.ModifiedBy = User.Identity.GetUserId<int>();
                        CreditSale.ModifiedDate = GetLocalDateTime();
                    }
                    var Schedules = _mapper.Map<IEnumerable<CreateCreditSalesSchedules>, IEnumerable<CreditSalesSchedule>>(salesOrder.SOSchedules);
                    foreach (var item in Schedules.ToList())
                    {
                        ScheduleNo++;
                        item.PaymentDate = DateTime.Now;
                        item.ScheduleNo = ScheduleNo;
                        _creditSalesOrderService.AddSchedule(item);
                    }
                    _creditSalesOrderService.SaveSalesOrder();
                    _customerService.SaveCustomer();
                    return RedirectToAction("Edit", new { orderId = salesOrder.SalesOrder.SalesOrderId });
                }
                else if (formCollection.Get("btnremaindDateSetup") != null) // To set remainder date when customer wants to pay installment
                {
                    int CSScheduleID = int.Parse(formCollection["CSScheduleID"]);
                    string scheduleDate = formCollection["Paydate"];
                    var Schedule = _creditSalesOrderService.GetSalesOrderSchedules(int.Parse(salesOrder.SalesOrder.SalesOrderId)).FirstOrDefault(i => i.CSScheduleID == CSScheduleID);
                    Schedule.RemindDate = Convert.ToDateTime(scheduleDate);
                    _creditSalesOrderService.UpdateSchedule(Schedule);
                    _creditSalesOrderService.SaveSalesOrder();
                    AddToastMessage("", "Update Successfull.", ToastType.Success);
                    return RedirectToAction("Edit", new { orderId = salesOrder.SalesOrder.SalesOrderId });
                }
                else
                {
                    return View(new CreditSalesOrderViewModel
                    {
                        SODetail = new CreateCreditSalesOrderDetailViewModel(),
                        SODetails = new List<CreateCreditSalesOrderDetailViewModel>(),
                        SOSchedules = new List<CreateCreditSalesSchedules>(),
                        SalesOrder = new CreateCreditSalesOrderViewModel()
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
        [Route("delete/{orderId}")]
        public ActionResult Delete(int orderId)
        {
            if (HasPaidInstallment(orderId))
            {
                AddToastMessage("", "This order can't be deleted. One or More Installment(s) has already been paid for this order.",
                    ToastType.Error);
                return RedirectToAction("Index");
            }
            var CreditSale = _creditSalesOrderService.GetSalesOrderById(orderId);
            if (!IsDateValid(CreditSale.SalesDate))
            {
                return RedirectToAction("Index");
            }
            _creditSalesOrderService.ReturnSalesOrderUsingSP(orderId, User.Identity.GetUserId<int>());

            AddToastMessage("", "Item has been returned successfully", ToastType.Success);
            return RedirectToAction("Index");
        }

        [HttpGet]
        [Authorize]
        [Route("editfromview/{id}/{detailId}")]
        public ActionResult EditFromView(int id, int detailId)
        {
            CreditSalesOrderViewModel salesOrder = (CreditSalesOrderViewModel)TempData.Peek("creditSalesOrderViewModel");
            if (salesOrder == null)
            {
                AddToastMessage("", "Item has been expired to edit", ToastType.Error);
                return RedirectToAction("Create");
            }

            CreateCreditSalesOrderDetailViewModel itemToEdit =
                salesOrder.SODetails.Where(x => int.Parse(x.ProductId) == id &&
                             int.Parse(x.StockDetailId) == detailId).FirstOrDefault();
            if (itemToEdit != null)
            {
                salesOrder.SalesOrder.GrandTotal = (decimal.Parse(GetDefaultIfNull(salesOrder.SalesOrder.GrandTotal)) -
                    (decimal.Parse(GetDefaultIfNull(itemToEdit.UTAmount)) + decimal.Parse(GetDefaultIfNull(itemToEdit.IntTotalAmt)))).ToString();

                salesOrder.SalesOrder.PPDiscountAmount = (decimal.Parse(GetDefaultIfNull(salesOrder.SalesOrder.PPDiscountAmount)) -
                    decimal.Parse(GetDefaultIfNull(itemToEdit.IntTotalAmt))).ToString();

                salesOrder.SalesOrder.NetDiscount = (decimal.Parse(GetDefaultIfNull(salesOrder.SalesOrder.NetDiscount)) -
                    decimal.Parse(GetDefaultIfNull(itemToEdit.IntTotalAmt))).ToString();

                salesOrder.SalesOrder.TotalAmount = (decimal.Parse(GetDefaultIfNull(salesOrder.SalesOrder.TotalAmount)) -
                    (decimal.Parse(GetDefaultIfNull(itemToEdit.UTAmount)))).ToString();

                salesOrder.SalesOrder.PaymentDue = (decimal.Parse(GetDefaultIfNull(salesOrder.SalesOrder.PaymentDue)) -
                    (decimal.Parse(GetDefaultIfNull(itemToEdit.UTAmount)))).ToString();

                salesOrder.SODetails.Remove(itemToEdit);

                salesOrder.SODetail = itemToEdit;
                TempData["creditSalesOrderViewModel"] = salesOrder;
                return RedirectToAction("Create");
            }
            else
            {
                AddToastMessage("", "No item found to edit", ToastType.Info);
                return RedirectToAction("Create");
            }
        }

        [HttpGet]
        [Authorize]
        [Route("deletefromview/{id}/{detailId}")]
        public ActionResult DeleteFromView(int id, int detailId)
        {
            CreditSalesOrderViewModel salesOrder = (CreditSalesOrderViewModel)TempData.Peek("creditSalesOrderViewModel");
            if (salesOrder == null)
            {
                AddToastMessage("", "Item has been expired to delete", ToastType.Error);
                return RedirectToAction("Create");
            }

            CreateCreditSalesOrderDetailViewModel itemToDelete =
                salesOrder.SODetails.Where(x => int.Parse(x.ProductId) == id &&
                             int.Parse(x.StockDetailId) == detailId).FirstOrDefault();
            if (itemToDelete != null)
            {
                salesOrder.SalesOrder.GrandTotal = (decimal.Parse(GetDefaultIfNull(salesOrder.SalesOrder.GrandTotal)) -
                    (decimal.Parse(GetDefaultIfNull(itemToDelete.UTAmount)) + decimal.Parse(GetDefaultIfNull(itemToDelete.IntTotalAmt)))).ToString();

                salesOrder.SalesOrder.PPDiscountAmount = (decimal.Parse(GetDefaultIfNull(salesOrder.SalesOrder.PPDiscountAmount)) -
                    decimal.Parse(GetDefaultIfNull(itemToDelete.IntTotalAmt))).ToString();

                salesOrder.SalesOrder.NetDiscount = (decimal.Parse(GetDefaultIfNull(salesOrder.SalesOrder.NetDiscount)) -
                    decimal.Parse(GetDefaultIfNull(itemToDelete.IntTotalAmt))).ToString();

                salesOrder.SalesOrder.TotalAmount = (decimal.Parse(GetDefaultIfNull(salesOrder.SalesOrder.TotalAmount)) -
                    (decimal.Parse(GetDefaultIfNull(itemToDelete.UTAmount)))).ToString();

                salesOrder.SalesOrder.PaymentDue = (decimal.Parse(GetDefaultIfNull(salesOrder.SalesOrder.PaymentDue)) -
                    (decimal.Parse(GetDefaultIfNull(itemToDelete.UTAmount)))).ToString();

                salesOrder.SODetails.Remove(itemToDelete);

                salesOrder.SODetail = new CreateCreditSalesOrderDetailViewModel();
                TempData["creditSalesOrderViewModel"] = salesOrder;
                AddToastMessage("", "Item has been removed successfully", ToastType.Success);
                return RedirectToAction("Create");
            }
            else
            {
                AddToastMessage("", "No item found to remove", ToastType.Info);
                return RedirectToAction("Create");
            }
        }

        private void RefreshFinalObject(CreditSalesOrderViewModel newSalesOrder,
            CreditSalesOrderViewModel salesOrder, FormCollection formCollection)
        {
            salesOrder.SalesOrder.NetDiscount = GetDefaultIfNull(newSalesOrder.SalesOrder.NetDiscount);
            salesOrder.SalesOrder.TotalAmount = decimal.Parse(GetDefaultIfNull(newSalesOrder.SalesOrder.TotalAmount)).ToString();
            salesOrder.SalesOrder.PaymentDue = decimal.Parse(GetDefaultIfNull(newSalesOrder.SalesOrder.PaymentDue)).ToString();
            salesOrder.SalesOrder.InterestRate = decimal.Parse(GetDefaultIfNull(newSalesOrder.SalesOrder.InterestRate)).ToString();
            salesOrder.SalesOrder.InterestAmount = decimal.Parse(GetDefaultIfNull(newSalesOrder.SalesOrder.InterestAmount)).ToString();
            salesOrder.SalesOrder.PayAdjustment = newSalesOrder.SalesOrder.PayAdjustment;
            salesOrder.SalesOrder.TotalDiscountPercentage = newSalesOrder.SalesOrder.TotalDiscountPercentage;
            salesOrder.SalesOrder.TotalDiscountAmount = newSalesOrder.SalesOrder.TotalDiscountAmount;
            salesOrder.SalesOrder.RecieveAmount = newSalesOrder.SalesOrder.RecieveAmount;
            salesOrder.SalesOrder.VATPercentage = newSalesOrder.SalesOrder.VATPercentage;
            salesOrder.SalesOrder.VATAmount = newSalesOrder.SalesOrder.VATAmount;
            salesOrder.SalesOrder.Remarks = newSalesOrder.SalesOrder.Remarks;
            salesOrder.SalesOrder.OrderDate = formCollection["OrderDate"];
            salesOrder.SalesOrder.CustomerId = formCollection["CustomersId"];
        }

        private void CreateSchedule(CreditSalesOrderViewModel newSalesOrder,
            CreditSalesOrderViewModel salesOrder, FormCollection formCollection, int noOfInstallment = 0, int total = 0)
        {
            var remainingAmount = decimal.Parse(GetDefaultIfNull(newSalesOrder.SalesOrder.PaymentDue));
            int installmentNo = noOfInstallment == 0 ? int.Parse(newSalesOrder.SalesOrder.InstallmentNo) : noOfInstallment;
            decimal nTotalBalance = remainingAmount;
            decimal nInstallmentAmt = Math.Round((remainingAmount / installmentNo), 2);

            decimal netRemainingAmount = decimal.Parse(GetDefaultIfNull(salesOrder.SalesOrder.TotalAmount)) - (decimal.Parse(GetDefaultIfNull(salesOrder.SalesOrder.InterestAmount)) + decimal.Parse(GetDefaultIfNull(salesOrder.SalesOrder.RecieveAmount)));
            decimal netInstallmentAmt = Math.Round((netRemainingAmount / decimal.Parse(GetDefaultIfNull(salesOrder.SalesOrder.InstallmentNo))));
            decimal hireValue = decimal.Parse(GetDefaultIfNull(salesOrder.SalesOrder.InterestAmount)) / decimal.Parse(GetDefaultIfNull(salesOrder.SalesOrder.InstallmentNo));

            CreateCreditSalesSchedules schedule = null;
            DateTime installmentMonth = total == 0 ? DateTime.Parse(formCollection["InstallmentDate"]).AddMonths(1) :
                DateTime.Parse(formCollection["InstallmentDate"]).AddMonths(total + 1);

            for (int i = 0; i < installmentNo; i++)
            {
                schedule = new CreateCreditSalesSchedules();
                schedule.ScheduleDate = installmentMonth.ToString("dd MMM yyyy");
                schedule.PaymentStatus = "Due";
                schedule.InstallmentAmount = nInstallmentAmt.ToString();
                schedule.PayDate = string.Empty;
                schedule.OpeningBalance = Math.Round(nTotalBalance).ToString();
                nTotalBalance -= nInstallmentAmt;
                schedule.ClosingBalance = Math.Round(nTotalBalance).ToString();
                schedule.IsUnExpected = noOfInstallment == 0 ? false : true;

                schedule.HireValue = hireValue;
                schedule.NetValue = netInstallmentAmt;
                schedule.SalesOrderId = salesOrder.SalesOrder.SalesOrderId;
                salesOrder.SOSchedules.Add(schedule);

                installmentMonth = installmentMonth.AddMonths(1);
            }
        }

        private void CheckAndAddModelErrorForAdd(CreditSalesOrderViewModel newSalesOrder,
            CreditSalesOrderViewModel salesOrder, FormCollection formCollection)
        {
            if (string.IsNullOrEmpty(formCollection["OrderDate"]))
                ModelState.AddModelError("SalesOrder.OrderDate", "Sales Date is required");

            if (string.IsNullOrEmpty(formCollection["CustomersId"]))
                ModelState.AddModelError("SalesOrder.CustomerId", "Customer is required");

            //ProductDetailsId is ProductId
            if (string.IsNullOrEmpty(formCollection["ProductDetailsId"]))
                ModelState.AddModelError("SODetail.ProductId", "Product is required");
            else
            {
                newSalesOrder.SODetail.ProductId = formCollection["ProductDetailsId"];
                salesOrder.SODetail.ProductId = formCollection["ProductDetailsId"];
            }
            if (string.IsNullOrEmpty(newSalesOrder.SODetail.Quantity) || int.Parse(GetDefaultIfNull(newSalesOrder.SODetail.Quantity)) <= 0)
            {
                ModelState.AddModelError("SODetail.Quantity", "Quantity is required");
            }

            if (string.IsNullOrEmpty(newSalesOrder.SalesOrder.InvoiceNo))
                ModelState.AddModelError("SalesOrder.InvoiceNo", "Invoice No. is required");

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
                var product = _productService.GetProductById(int.Parse(GetDefaultIfNull(formCollection["ProductDetailsId"])));

                if (product.ProductType == (int)EnumProductType.NoBarcode)
                {
                    var stockCount = _stockService.GetStockByProductId(product.ProductID);

                    if (stockCount.Quantity < int.Parse(newSalesOrder.SODetail.Quantity))
                        ModelState.AddModelError("SODetail.Quantity", "Stock is not available. Stock Quantity: " + stockCount.Quantity);
                }
                else
                {
                    var stockDetails = _stockDetailService.GetStockDetailByProductId(int.Parse(GetDefaultIfNull(formCollection["ProductDetailsId"])));

                    if (!stockDetails.Any(x => x.IMENO.Equals(newSalesOrder.SODetail.IMENo)))
                        ModelState.AddModelError("SODetail.IMENo", "Invalid IMENo/Barcode");
                }

            }


        }

        private void AddNote(CreditSalesOrderViewModel salesOrder, FormCollection formCollection)
        {
            if (!string.IsNullOrEmpty(formCollection["selSalesRate"]))
            {
                List<int> SDetailIDList = salesOrder.SODetails.Select(i => int.Parse(i.StockDetailId)).ToList();
                var StockDetails = _stockDetailService.GetAll().Where(i => (SDetailIDList.Contains(i.SDetailID)));
                decimal RateFrom = 0, RateTo = 0;
                if (formCollection["selSalesRate"].Equals("1"))
                {
                    salesOrder.SalesOrder.InstallmentNo = "3";
                    RateFrom = StockDetails.Sum(i => i.CRSalesRate3Month);
                    RateTo = StockDetails.Sum(i => i.CreditSRate);
                    salesOrder.SalesOrder.Remarks = "If you can't pay within 3 Months. The rate will be increased from " + RateFrom + " to " + RateTo;
                }
                else if (formCollection["selSalesRate"].Equals("2"))
                {
                    salesOrder.SalesOrder.InstallmentNo = "6";
                    RateFrom = StockDetails.Sum(i => i.CreditSRate);
                    RateTo = StockDetails.Sum(i => i.CRSalesRate12Month);
                    salesOrder.SalesOrder.Remarks = "If you can't pay within 6 Months. The rate will be increased from " + RateFrom + " to " + RateTo;
                }
                else if (formCollection["selSalesRate"].Equals("3"))
                {
                    salesOrder.SalesOrder.InstallmentNo = "12";
                    RateFrom = StockDetails.Sum(i => i.CRSalesRate12Month);
                    RateTo = RateFrom + (StockDetails.Sum(i => i.CreditSRate) - StockDetails.Sum(i => i.CRSalesRate3Month));
                    salesOrder.SalesOrder.Remarks = "If you can't pay within 12 Months. The rate will be increased from " + RateFrom + " to " + RateTo;
                }

            }
        }

        private void CheckAndAddModelErrorForSave(CreditSalesOrderViewModel newSalesOrder,
            CreditSalesOrderViewModel salesOrder, FormCollection formCollection)
        {
            if (string.IsNullOrEmpty(newSalesOrder.SalesOrder.GrandTotal) ||
                decimal.Parse(GetDefaultIfNull(newSalesOrder.SalesOrder.GrandTotal)) <= 0)
                ModelState.AddModelError("SalesOrder.GrandTotal", "Grand Total is required");

            if (string.IsNullOrEmpty(newSalesOrder.SalesOrder.TotalAmount) ||
                decimal.Parse(GetDefaultIfNull(newSalesOrder.SalesOrder.TotalAmount)) <= 0)
                ModelState.AddModelError("SalesOrder.TotalAmount", "Net Total is required");

            if (string.IsNullOrEmpty(newSalesOrder.SalesOrder.RecieveAmount) ||
                decimal.Parse(GetDefaultIfNull(newSalesOrder.SalesOrder.RecieveAmount)) <= 0)
                ModelState.AddModelError("SalesOrder.RecieveAmount", "Down Payment is required");

            #region Customer and Employee Due Limit check
            Customer customer = _customerService.GetCustomerById(int.Parse(salesOrder.SalesOrder.CustomerId));
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
            #endregion

            //if (string.IsNullOrEmpty(newSalesOrder.SalesOrder.VATAmount) ||
            //    decimal.Parse(GetDefaultIfNull(newSalesOrder.SalesOrder.VATAmount)) <= 0)
            //    ModelState.AddModelError("SalesOrder.VATAmount", "Vat Amount is required");

            if (!IsDateValid(Convert.ToDateTime(formCollection["OrderDate"])))
            {
                ModelState.AddModelError("SalesOrder.OrderDate", "Back dated entry is not valid.");
                salesOrder.SalesOrder.OrderDate = formCollection["OrderDate"];
            }
        }

        private void AddToOrder(CreditSalesOrderViewModel newSalesOrder,
            CreditSalesOrderViewModel salesOrder, FormCollection formCollection)
        {
            decimal quantity = decimal.Parse(GetDefaultIfNull(newSalesOrder.SODetail.Quantity));
            decimal totalInterest = quantity * decimal.Parse(GetDefaultIfNull(newSalesOrder.SODetail.IntTotalAmt));
            decimal totalOffer = quantity * decimal.Parse(GetDefaultIfNull(newSalesOrder.SODetail.PPOffer));

            salesOrder.SalesOrder.GrandTotal = (decimal.Parse(GetDefaultIfNull(newSalesOrder.SalesOrder.GrandTotal)) +
            decimal.Parse(GetDefaultIfNull(newSalesOrder.SODetail.UTAmount)) + totalOffer).ToString();

            salesOrder.SalesOrder.TotalDiscountPercentage = decimal.Parse(GetDefaultIfNull(newSalesOrder.SalesOrder.TotalDiscountPercentage)).ToString();
            //Dis Per. Of Total Amount
            salesOrder.SalesOrder.PPDiscountAmount = (decimal.Parse(GetDefaultIfNull(newSalesOrder.SalesOrder.PPDiscountAmount))).ToString();

            //Net Discount
            salesOrder.SalesOrder.TotalDiscountAmount = decimal.Parse(GetDefaultIfNull(newSalesOrder.SalesOrder.TotalDiscountAmount)).ToString();

            //Total Interest Rate and Total Interest Per.
            salesOrder.SalesOrder.InterestRate = decimal.Parse(GetDefaultIfNull(newSalesOrder.SalesOrder.InterestRate)).ToString();
            salesOrder.SalesOrder.InterestAmount = (decimal.Parse(GetDefaultIfNull(newSalesOrder.SalesOrder.InterestAmount)) + totalInterest).ToString();

            salesOrder.SalesOrder.VATPercentage = decimal.Parse(GetDefaultIfNull(newSalesOrder.SalesOrder.VATPercentage)).ToString();
            salesOrder.SalesOrder.VATAmount = decimal.Parse(GetDefaultIfNull(newSalesOrder.SalesOrder.VATAmount)).ToString();

            salesOrder.SalesOrder.NetDiscount = (decimal.Parse(GetDefaultIfNull(salesOrder.SalesOrder.NetDiscount)) + totalOffer).ToString();

            //+ decimal.Parse(GetDefaultIfNull(salesOrder.SalesOrder.PPDiscountAmount))

            // decimal.Parse(GetDefaultIfNull(salesOrder.SalesOrder.TotalDiscountAmount))
            var netTotal = ((decimal.Parse(GetDefaultIfNull(salesOrder.SalesOrder.GrandTotal)) + decimal.Parse(GetDefaultIfNull(salesOrder.SalesOrder.VATAmount))) -
                decimal.Parse(GetDefaultIfNull(salesOrder.SalesOrder.NetDiscount)));



            salesOrder.SalesOrder.TotalAmount = netTotal.ToString();
            salesOrder.SalesOrder.PaymentDue = (netTotal - decimal.Parse(GetDefaultIfNull(newSalesOrder.SalesOrder.RecieveAmount))).ToString();

            salesOrder.SalesOrder.PayAdjustment = decimal.Parse(GetDefaultIfNull(newSalesOrder.SalesOrder.PayAdjustment)).ToString();

            //For Total Offer Purpose
            salesOrder.SalesOrder.TotalOffer = (decimal.Parse(GetDefaultIfNull(salesOrder.SalesOrder.TotalOffer)) + totalOffer).ToString();

            ////add interest for per product
            //salesOrder.SalesOrder.InterestAmount = (decimal.Parse(GetDefaultIfNull(salesOrder.SalesOrder.InterestAmount))
            //    + totalInterest).ToString();

            salesOrder.SalesOrder.OrderDate = formCollection["OrderDate"];
            salesOrder.SalesOrder.CustomerId = formCollection["CustomersId"];

            salesOrder.SODetail.ProductId = formCollection["ProductDetailsId"];
            salesOrder.SODetail.StockDetailId = formCollection["StockDetailsId"];
            salesOrder.SODetail.ColorId = formCollection["ColorsId"];
            salesOrder.SODetail.ColorName = newSalesOrder.SODetail.ColorName;
            salesOrder.SODetail.ProductCode = formCollection["ProductDetailsCode"];
            salesOrder.SODetail.IMENo = newSalesOrder.SODetail.IMENo;
            salesOrder.SODetail.Quantity = newSalesOrder.SODetail.Quantity;
            salesOrder.SODetail.IntPercentage = newSalesOrder.SODetail.IntPercentage;
            //salesOrder.SODetail.IntTotalAmt = newSalesOrder.SODetail.IntTotalAmt;
            salesOrder.SODetail.IntTotalAmt = totalInterest.ToString();
            salesOrder.SODetail.UnitPrice = newSalesOrder.SODetail.UnitPrice;
            salesOrder.SODetail.MRPRate = newSalesOrder.SODetail.MRPRate;
            salesOrder.SODetail.UTAmount = newSalesOrder.SODetail.UTAmount;
            salesOrder.SODetail.ProductName = formCollection["ProductDetailsName"];
            //salesOrder.SODetail.PPOffer = newSalesOrder.SODetail.PPOffer;
            salesOrder.SODetail.PPOffer = totalOffer.ToString();

            salesOrder.SODetail.CompressorWarrentyMonth = newSalesOrder.SODetail.CompressorWarrentyMonth;
            salesOrder.SODetail.MotorWarrentyMonth = newSalesOrder.SODetail.MotorWarrentyMonth;
            salesOrder.SODetail.PanelWarrentyMonth = newSalesOrder.SODetail.PanelWarrentyMonth;
            salesOrder.SODetail.SparePartsWarrentyMonth = newSalesOrder.SODetail.SparePartsWarrentyMonth;
            salesOrder.SODetail.ServiceWarrentyMonth = newSalesOrder.SODetail.ServiceWarrentyMonth;


            salesOrder.SODetails = salesOrder.SODetails ?? new List<CreateCreditSalesOrderDetailViewModel>();
            salesOrder.SODetails.Add(salesOrder.SODetail);

            salesOrder.SOSchedules = salesOrder.SOSchedules ?? new List<CreateCreditSalesSchedules>();
            AddNote(salesOrder, formCollection);
            CreditSalesOrderViewModel vm = new CreditSalesOrderViewModel
            {
                SODetail = new CreateCreditSalesOrderDetailViewModel(),
                SODetails = salesOrder.SODetails,
                SOSchedules = salesOrder.SOSchedules,
                SalesOrder = salesOrder.SalesOrder
            };

            TempData["creditSalesOrderViewModel"] = vm;
            salesOrder.SODetail = new CreateCreditSalesOrderDetailViewModel();
            AddToastMessage("", "Order has been added successfully.", ToastType.Success);
        }

        private void CalculateInstallments(CreditSalesOrderViewModel newSalesOrder,
            CreditSalesOrderViewModel salesOrder, FormCollection formCollection)
        {
            RefreshFinalObject(newSalesOrder, salesOrder, formCollection);
            salesOrder.SalesOrder.InstallmentDate = formCollection["InstallmentDate"];
            salesOrder.SalesOrder.InstallmentNo = newSalesOrder.SalesOrder.InstallmentNo;

            salesOrder.SOSchedules = new List<CreateCreditSalesSchedules>();
            CreateSchedule(newSalesOrder, salesOrder, formCollection);

            TempData["creditSalesOrderViewModel"] = salesOrder;
            ViewBag.Status = "data-attr='calculated'";
            AddToastMessage("", "Schedule has been calculated successfully.", ToastType.Success);
        }

        private void InstallmentPayment(CreditSalesOrderViewModel newSalesOrder,
            CreditSalesOrderViewModel salesOrder, FormCollection formCollection)
        {
            RefreshFinalObject(newSalesOrder, salesOrder, formCollection);

            string scheduleDate = formCollection["scheduleDate"];
            CreateCreditSalesSchedules creditSalesSchedule = null;
            if (newSalesOrder.SalesOrder.IsAllPaid)
                creditSalesSchedule = salesOrder.SOSchedules.FirstOrDefault(x => x.PaymentStatus.Equals("Due"));
            else
                creditSalesSchedule = salesOrder.SOSchedules.FirstOrDefault(x => DateTime.Parse(x.ScheduleDate) == DateTime.Parse(scheduleDate) && x.PaymentStatus.Equals("Due"));

            if (creditSalesSchedule != null)
            {
                salesOrder.SalesOrder.PaymentDue = (decimal.Parse(creditSalesSchedule.OpeningBalance) -
                    ((decimal.Parse(newSalesOrder.SalesOrder.InstallmentAmount)) + decimal.Parse(GetDefaultIfNull(newSalesOrder.SalesOrder.PayAdjustment)))).ToString();
                salesOrder.SalesOrder.WInterestAmt = decimal.Parse(GetDefaultIfNull(newSalesOrder.SalesOrder.PayAdjustment)).ToString();
                if (!string.IsNullOrEmpty(formCollection["Paydate"]))
                {
                    creditSalesSchedule.PayDate = formCollection["Paydate"];
                    creditSalesSchedule.PayDate = DateTime.Parse(creditSalesSchedule.PayDate).ToString("dd MMM yyyy");
                }
                else
                    creditSalesSchedule.PayDate = DateTime.Today.ToString("dd MMM yyyy");

                creditSalesSchedule.PaymentStatus = "Paid";
                creditSalesSchedule.Remarks = formCollection["Remarks" + scheduleDate];

                if (decimal.Parse(creditSalesSchedule.InstallmentAmount) !=
                    (decimal.Parse(newSalesOrder.SalesOrder.InstallmentAmount) +
                    decimal.Parse(GetDefaultIfNull(newSalesOrder.SalesOrder.PayAdjustment))))
                {
                    creditSalesSchedule.InstallmentAmount = decimal.Parse(newSalesOrder.SalesOrder.InstallmentAmount).ToString();
                    creditSalesSchedule.ClosingBalance = salesOrder.SalesOrder.PaymentDue;
                    List<CreateCreditSalesSchedules> schedulesToDelete = salesOrder.SOSchedules.
                        Where(x => x.PaymentStatus.Equals("Due")).ToList();
                    int paidSchedules = salesOrder.SOSchedules.
                        Where(x => x.PaymentStatus.Equals("Paid")).Count();
                    int remainingNoOfIns = (salesOrder.SOSchedules.Count() - paidSchedules);

                    foreach (var item in schedulesToDelete)
                        salesOrder.SOSchedules.Remove(item);

                    newSalesOrder.SalesOrder.PaymentDue = salesOrder.SalesOrder.PaymentDue;
                    CreateSchedule(newSalesOrder, salesOrder, formCollection, remainingNoOfIns, paidSchedules);
                }
                else
                {
                    creditSalesSchedule.InstallmentAmount = decimal.Parse(newSalesOrder.SalesOrder.InstallmentAmount).ToString();
                }
                bool IsEditInstallment = false;

                log.Info(new { salesOrder.SalesOrder.SalesOrderId, creditSalesSchedule.InstallmentAmount, newSalesOrder.SalesOrder.PayAdjustment });

                if (ControllerContext.RouteData.Values["action"].ToString().ToLower().Equals("edit"))
                {
                    DataTable dtSalesOrderSchedules = CreateSOSchedulesDataTable(salesOrder);
                    _creditSalesOrderService.InstallmentPaymentUsingSP(int.Parse(salesOrder.SalesOrder.SalesOrderId),
                        decimal.Parse(creditSalesSchedule.InstallmentAmount), dtSalesOrderSchedules, decimal.Parse(GetDefaultIfNull(newSalesOrder.SalesOrder.PayAdjustment)));
                    IsEditInstallment = true;
                }

                TempData["creditSalesOrderViewModel"] = salesOrder;
                ViewBag.Status = "data-attr='calculated'";

                //TempData["IsMoneyReceiptReady"] = true;

                var creditsalesSchedules = new CreditSalesSchedule()
                {
                    PaymentDate = Convert.ToDateTime(creditSalesSchedule.PayDate),
                    InstallmentAmt = Convert.ToDecimal(creditSalesSchedule.InstallmentAmount),

                };
                //var CreditSales = new CreditSale()
                //{
                //    InvoiceNo = salesOrder.SalesOrder.InvoiceNo,
                //    CustomerID = Convert.ToInt32(salesOrder.SalesOrder.CustomerId),
                //    DownPayment = IsEditInstallment ? 0m : Convert.ToDecimal(salesOrder.SalesOrder.RecieveAmount),
                //    Remaining = Convert.ToDecimal(salesOrder.SalesOrder.PaymentDue),
                //    IssueDate = Convert.ToDateTime(salesOrder.SalesOrder.OrderDate),
                //};

                //List<CreditSaleDetails> Details = (from cd in salesOrder.SODetails
                //                                   select new CreditSaleDetails
                //                                   {
                //                                       ProductID = Convert.ToInt32(cd.ProductId)
                //                                   }).ToList();

                //TempData["MoneyReceiptData"] = CreditSales;
                //TempData["creditsalesSchedules"] = creditsalesSchedules;
                //TempData["Details"] = Details;
                TempData["IsMoneyReceiptReadyByID"] = true;
                TempData["OrderId"] = Convert.ToInt32(salesOrder.SalesOrder.SalesOrderId);
                salesOrder.SODetail = new CreateCreditSalesOrderDetailViewModel();



                AddToastMessage("", "Installment has been paid successfully.", ToastType.Success);
            }
            else
            {
                AddToastMessage("", "No schedule found to paid.", ToastType.Error);
                return;
            }
        }

        private void SaveOrder(CreditSalesOrderViewModel newSalesOrder,
            CreditSalesOrderViewModel salesOrder, FormCollection formCollection)
        {
            RefreshFinalObject(newSalesOrder, salesOrder, formCollection);

            DataTable dtSalesOrder = CreateSalesOrderDataTable(salesOrder);
            DataTable dtSalesOrderDetail = CreateSODetailDataTable(salesOrder);
            DataTable dtSalesOrderSchedules = CreateSOSchedulesDataTable(salesOrder);

            log.Info(new { salesOrder.SalesOrder, salesOrder.SODetails, salesOrder.SOSchedules });

            _creditSalesOrderService.AddSalesOrderUsingSP(dtSalesOrder, dtSalesOrderDetail, dtSalesOrderSchedules);
            _creditSalesOrderService.CorrectionStockData(User.Identity.GetConcernId());

            AddToastMessage("", "Order has been saved successfully.", ToastType.Success);
        }

        private DataTable CreateSalesOrderDataTable(CreditSalesOrderViewModel salesOrder)
        {
            DataTable dtSalesOrder = new DataTable();
            dtSalesOrder.Columns.Add("InvoiceNo", typeof(string));
            dtSalesOrder.Columns.Add("CustomerId", typeof(int));
            dtSalesOrder.Columns.Add("TSalesAmt", typeof(decimal));
            dtSalesOrder.Columns.Add("NoOfInstallment", typeof(int));
            dtSalesOrder.Columns.Add("InstallmentPrinciple", typeof(decimal));
            dtSalesOrder.Columns.Add("IssueDate", typeof(DateTime));
            dtSalesOrder.Columns.Add("UserName", typeof(string));
            dtSalesOrder.Columns.Add("Remaining", typeof(decimal));
            dtSalesOrder.Columns.Add("InterestRate", typeof(decimal));
            dtSalesOrder.Columns.Add("InterestAmount", typeof(decimal));
            dtSalesOrder.Columns.Add("SalesDate", typeof(DateTime));
            dtSalesOrder.Columns.Add("DownPayment", typeof(decimal));
            dtSalesOrder.Columns.Add("WInterestAmt", typeof(decimal));
            dtSalesOrder.Columns.Add("FixedAmount", typeof(decimal));
            dtSalesOrder.Columns.Add("IsStatus", typeof(int));
            dtSalesOrder.Columns.Add("UnExInstallment", typeof(int));
            dtSalesOrder.Columns.Add("Quantity", typeof(int));
            dtSalesOrder.Columns.Add("Discount", typeof(decimal));
            dtSalesOrder.Columns.Add("NetAmount", typeof(decimal));
            dtSalesOrder.Columns.Add("IsUnExpected", typeof(int));
            dtSalesOrder.Columns.Add("Remarks", typeof(string));
            dtSalesOrder.Columns.Add("Status", typeof(int));
            dtSalesOrder.Columns.Add("VatPercentage", typeof(decimal));
            dtSalesOrder.Columns.Add("VatAmount", typeof(decimal));
            dtSalesOrder.Columns.Add("TotalDue", typeof(decimal));
            dtSalesOrder.Columns.Add("ConcernId", typeof(int));
            dtSalesOrder.Columns.Add("CreatedBy", typeof(int));
            dtSalesOrder.Columns.Add("CreatedDate", typeof(DateTime));
            dtSalesOrder.Columns.Add("TotalOffer", typeof(decimal));
            dtSalesOrder.Columns.Add("InstallmentPeriod", typeof(string));

            DataRow row = null;
            int NumberOfInstallment = 0;
            row = dtSalesOrder.NewRow();
            row["InvoiceNo"] = salesOrder.SalesOrder.InvoiceNo;
            row["CustomerId"] = int.Parse(salesOrder.SalesOrder.CustomerId);
            row["TSalesAmt"] = decimal.Parse(salesOrder.SalesOrder.GrandTotal);
            NumberOfInstallment = int.Parse(GetDefaultIfNull(salesOrder.SalesOrder.InstallmentNo));
            row["NoOfInstallment"] = NumberOfInstallment;
            row["InstallmentPrinciple"] = 0.0;
            row["IssueDate"] = DateTime.Parse(salesOrder.SalesOrder.InstallmentDate);
            row["UserName"] = string.Empty;
            row["Remaining"] = decimal.Parse(GetDefaultIfNull(salesOrder.SalesOrder.PaymentDue));
            row["InterestRate"] = 0;
            row["SalesDate"] = DateTime.Parse(salesOrder.SalesOrder.OrderDate);
            row["DownPayment"] = decimal.Parse(GetDefaultIfNull(salesOrder.SalesOrder.RecieveAmount));
            row["WInterestAmt"] = decimal.Parse(GetDefaultIfNull(salesOrder.SalesOrder.WInterestAmt));
            row["InterestRate"] = decimal.Parse(GetDefaultIfNull(salesOrder.SalesOrder.InterestRate));
            row["InterestAmount"] = decimal.Parse(GetDefaultIfNull(salesOrder.SalesOrder.InterestAmount));
            row["FixedAmount"] = 0.0;
            row["IsStatus"] = EnumSalesType.Sales;
            row["UnExInstallment"] = 0.0;
            row["Quantity"] = salesOrder.SODetails.Sum(x => int.Parse(x.Quantity));
            row["Discount"] = decimal.Parse(GetDefaultIfNull(salesOrder.SalesOrder.NetDiscount));
            row["NetAmount"] = decimal.Parse(GetDefaultIfNull(salesOrder.SalesOrder.TotalAmount));
            row["IsUnExpected"] = 0;
            row["Remarks"] = salesOrder.SalesOrder.Remarks;
            row["Status"] = 0;
            row["VatPercentage"] = decimal.Parse(GetDefaultIfNull(salesOrder.SalesOrder.VATPercentage));
            row["VatAmount"] = decimal.Parse(GetDefaultIfNull(salesOrder.SalesOrder.VATAmount));
            row["TotalDue"] = (decimal.Parse(GetDefaultIfNull(salesOrder.SalesOrder.PaymentDue)) - decimal.Parse(GetDefaultIfNull(salesOrder.SalesOrder.CurrentPreviousDue)));
            row["ConcernId"] = User.Identity.GetConcernId();
            row["CreatedBy"] = User.Identity.GetUserId<int>();
            row["CreatedDate"] = DateTime.Now;
            row["TotalOffer"] = decimal.Parse(GetDefaultIfNull(salesOrder.SalesOrder.TotalOffer));
            row["InstallmentPeriod"] = NumberOfInstallment + " Months";

            dtSalesOrder.Rows.Add(row);

            return dtSalesOrder;
        }

        private DataTable CreateSODetailDataTable(CreditSalesOrderViewModel salesOrder)
        {
            DataTable dtSalesOrderDetail = new DataTable();
            dtSalesOrderDetail.Columns.Add("ProductId", typeof(int));
            dtSalesOrderDetail.Columns.Add("StockDetailId", typeof(int));
            dtSalesOrderDetail.Columns.Add("ColorId", typeof(int));
            dtSalesOrderDetail.Columns.Add("Quantity", typeof(decimal));
            dtSalesOrderDetail.Columns.Add("UnitPrice", typeof(decimal));
            dtSalesOrderDetail.Columns.Add("UTAmount", typeof(decimal));
            dtSalesOrderDetail.Columns.Add("MpRateTotal", typeof(decimal));
            dtSalesOrderDetail.Columns.Add("MrpRate", typeof(decimal));
            dtSalesOrderDetail.Columns.Add("PPOffer", typeof(decimal));
            dtSalesOrderDetail.Columns.Add("IntPercentage", typeof(decimal));
            dtSalesOrderDetail.Columns.Add("IntTotalAmt", typeof(decimal));
            dtSalesOrderDetail.Columns.Add("Compressor", typeof(int));
            dtSalesOrderDetail.Columns.Add("Motor", typeof(int));
            dtSalesOrderDetail.Columns.Add("Panel", typeof(int));
            dtSalesOrderDetail.Columns.Add("Spareparts", typeof(int));
            dtSalesOrderDetail.Columns.Add("Service", typeof(int));
            dtSalesOrderDetail.Columns.Add("CreditSalesDetailsId", typeof(int));

            DataRow row = null;

            foreach (var item in salesOrder.SODetails)
            {
                row = dtSalesOrderDetail.NewRow();

                if (!string.IsNullOrEmpty(item.SODetailId))
                    row["CreditSalesDetailsId"] = int.Parse(item.SODetailId);
                row["ProductId"] = int.Parse(item.ProductId);
                row["StockDetailId"] = int.Parse(item.StockDetailId);
                row["ColorId"] = int.Parse(item.ColorId);
                row["Quantity"] = int.Parse(item.Quantity);
                row["UnitPrice"] = decimal.Parse(item.UnitPrice);
                row["UTAmount"] = decimal.Parse(item.UTAmount);
                row["MpRateTotal"] = decimal.Parse(item.MRPRate);
                row["MrpRate"] = decimal.Parse(item.MRPRate);
                row["PPOffer"] = decimal.Parse(GetDefaultIfNull(item.PPOffer));
                row["IntPercentage"] = decimal.Parse(GetDefaultIfNull(item.IntPercentage));
                row["IntTotalAmt"] = decimal.Parse(GetDefaultIfNull(item.IntTotalAmt));
                row["Compressor"] = 0;
                row["Motor"] = 0;
                row["Panel"] = 0;
                row["Spareparts"] = 0;
                row["Service"] = 0;

               
                dtSalesOrderDetail.Rows.Add(row);
            }

            return dtSalesOrderDetail;
        }

        private DataTable CreateSOSchedulesDataTable(CreditSalesOrderViewModel salesOrder)
        {
            DataTable dtSalesOrderSchedules = new DataTable();
            dtSalesOrderSchedules.Columns.Add("MonthDate", typeof(DateTime));
            dtSalesOrderSchedules.Columns.Add("Balance", typeof(decimal));
            dtSalesOrderSchedules.Columns.Add("InstallmentAmt", typeof(decimal));
            dtSalesOrderSchedules.Columns.Add("PaymentDate", typeof(DateTime));
            dtSalesOrderSchedules.Columns.Add("PaymentStatus", typeof(string));
            dtSalesOrderSchedules.Columns.Add("InterestAmount", typeof(decimal));
            dtSalesOrderSchedules.Columns.Add("ClosingBalance", typeof(decimal));
            dtSalesOrderSchedules.Columns.Add("ScheduleNo", typeof(int));
            dtSalesOrderSchedules.Columns.Add("Remarks", typeof(string));
            dtSalesOrderSchedules.Columns.Add("IsUnExpected", typeof(int));
            dtSalesOrderSchedules.Columns.Add("HireValue", typeof(decimal));
            dtSalesOrderSchedules.Columns.Add("NetValue", typeof(decimal));
            DataRow row = null;
            int sno = 1;
            foreach (var item in salesOrder.SOSchedules)
            {

                row = dtSalesOrderSchedules.NewRow();
                row["MonthDate"] = DateTime.Parse(item.ScheduleDate);
                row["Balance"] = decimal.Parse(item.OpeningBalance);
                row["InstallmentAmt"] = decimal.Parse(item.InstallmentAmount);
                row["PaymentDate"] = string.IsNullOrEmpty(item.PayDate) ? SqlDateTime.MinValue.Value : DateTime.Parse(item.PayDate);
                row["PaymentStatus"] = item.PaymentStatus;
                row["InterestAmount"] = 0.0;
                row["ClosingBalance"] = decimal.Parse(GetDefaultIfNull(item.ClosingBalance));
                row["ScheduleNo"] = sno;
                row["Remarks"] = item.Remarks;
                row["IsUnExpected"] = item.IsUnExpected;
                row["HireValue"] = item.HireValue;
                row["NetValue"] = item.NetValue;
                dtSalesOrderSchedules.Rows.Add(row);
                sno++;
            }

            return dtSalesOrderSchedules;
        }

        [HttpGet]
        [Authorize]
        public ActionResult Invoice(int orderId)
        {
            TempData["IsCInvoiceReadyById"] = true;
            TempData["OrderId"] = orderId;
            return RedirectToAction("Index");
        }
        [HttpGet]
        [Authorize]
        public ActionResult MoneyReceipt(int orderId)
        {
            TempData["IsMoneyReceiptReadyByID"] = true;
            TempData["OrderId"] = orderId;
            return RedirectToAction("Index");
        }

        private bool HasPaidInstallment(int id)
        {
            return _creditSalesOrderService.HasPaidInstallment(id);
        }

        [HttpGet]
        [Authorize]
        public ActionResult UpComingScheduleReport()
        {
            return View("UpComingScheduleReport");
        }

        [HttpGet]
        [Authorize]
        public ActionResult InstallmentCollectionReport()
        {
            return View("InstallmentCollectionReport");
        }

        [HttpGet]
        [Authorize]
        public JsonResult GetProductDetailByIMEINo(string imeiNo)
        {

            if (!string.IsNullOrEmpty(imeiNo))
            {
                var customProductDetails = _productService.GetAllProductFromDetailForCredit();
                var vmProductDetails = _mapper.Map<IEnumerable<Tuple<int, string, string,
                decimal, string, string, string, Tuple<decimal?, string, decimal, int, int, string, string, Tuple<decimal, string, string, string, string, string, decimal,Tuple<string>>>>>, IEnumerable<GetProductViewModel>>(customProductDetails);

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
                    return Json(false, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult DefaultingCustomer()
        {
            return View();
        }
    }
}