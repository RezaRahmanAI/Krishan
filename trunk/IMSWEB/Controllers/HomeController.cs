using IMSWEB.Model;
using IMSWEB.Model.TOs;
using IMSWEB.Report;
using IMSWEB.Service;
using Microsoft.AspNet.Identity;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace IMSWEB.Controllers
{
    public class HomeController : CoreController
    {
        ICreditSalesOrderService _salesOrderService;
        ITransactionalReport _transactionalReportService;
        ICustomerService _CustomerService;
        ISystemInformationService _SysInfoService;
        ISMSStatusService _SMSStatusService;

        private readonly IServiceChargeService _serviceChargeService;
        private readonly IServiceChargeDetailsService _serviceChargeDetailsService;
        private readonly ISisterConcernService _sisterConcernService;
        private readonly IUserService _user;

        public HomeController(IErrorService errorService, ITransactionalReport transactionalReportService,
            ICreditSalesOrderService SaleOrderService, ICustomerService CustomerService,
              ISystemInformationService SysInfoService, ISMSStatusService SMSStatusService, IServiceChargeService serviceChargeService, IServiceChargeDetailsService serviceChargeDetailsService, ISisterConcernService sisterConcernService, IUserService user)
            : base(errorService)
        {
            _salesOrderService = SaleOrderService;
            _transactionalReportService = transactionalReportService;
            _CustomerService = CustomerService;
            _SysInfoService = SysInfoService;
            _SMSStatusService = SMSStatusService;
            _serviceChargeService = serviceChargeService;
            _serviceChargeDetailsService = serviceChargeDetailsService;
            _sisterConcernService = sisterConcernService;
            _user = user;
        }



        [AllowAnonymous]
        public async Task<ActionResult> Index()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return View();
            }
          
            #region check if payment info exists or not, if not then insert data
            AddPaymentInfoData();
            #endregion

            return View();
        }

        private void AddPaymentInfoData()
        {
            int concernId = User.Identity.GetConcernId();
            DateTime todaysDate = GetLocalDateTime();

            List<SisterConcern> sisterConcerns = _sisterConcernService.GetFamilyTree(concernId);
            if (sisterConcerns.Any())
            {
                foreach (var concern in sisterConcerns)
                {
                    ServiceCharge serviceCharge = _serviceChargeService.GetByYearAndConcern(concern.ConcernID, todaysDate.Year);
                    if (serviceCharge == null)
                    {

                        ServiceCharge newServiceCharge = new ServiceCharge
                        {
                            ServiceYear = todaysDate.Year,
                            ConcernId = concern.ConcernID,
                            TotalServiceCollection = 0m,
                            CreateDate = DateTime.Now,
                            CreatedBy = User.Identity.GetUserId<int>()
                        };
                        _serviceChargeService.Add(newServiceCharge);
                        bool isServiceInserted = _serviceChargeService.Save();
                        if (isServiceInserted)
                        {
                            serviceCharge = _serviceChargeService.GetByYearAndConcern(concern.ConcernID, todaysDate.Year);
                        }
                    }

                    List<ServiceChargeDetails> serviceChargeDetails = _serviceChargeDetailsService.GetAllByServiceId(serviceCharge.Id);
                    SisterConcern sisterConcern = _sisterConcernService.GetSisterConcernById(concern.ConcernID);
                    List<ServiceChargeDetails> addChargeList = new List<ServiceChargeDetails>();
                    if (serviceChargeDetails.Any())
                    {
                        ServiceChargeDetails lastCharge = serviceChargeDetails.OrderByDescending(d => d.Month).ThenByDescending(d => d.ServiceChargeId).FirstOrDefault();

                        if (lastCharge.Month < todaysDate.Month)
                        {
                            int differentsOfMonth = todaysDate.Month - lastCharge.Month;
                            int month = lastCharge.Month;
                            for (int i = 0; i < differentsOfMonth; i++)
                            {
                                ServiceChargeDetails serviceDetail = new ServiceChargeDetails
                                {
                                    ServiceChargeId = serviceCharge.Id,
                                    ExpectedServiceCharge = sisterConcern.ServiceCharge,
                                    Month = month + i + 1,
                                    IsPaid = false,
                                    PaidServiceCharge = 0m
                                };
                                addChargeList.Add(serviceDetail);
                            }
                        }
                    }
                    else
                    {
                        int differentsOfMonth = todaysDate.Month;
                        for (int i = 1; i <= differentsOfMonth; i++)
                        {
                            ServiceChargeDetails serviceDetail = new ServiceChargeDetails
                            {
                                ServiceChargeId = serviceCharge.Id,
                                ExpectedServiceCharge = sisterConcern.ServiceCharge,
                                Month = i,
                                IsPaid = false,
                                PaidServiceCharge = 0m
                            };
                            addChargeList.Add(serviceDetail);
                        }
                    }

                    if (addChargeList.Any())
                    {
                        _serviceChargeDetailsService.AddMultiple(addChargeList);
                        _serviceChargeDetailsService.Save();
                    }
                }
            }


        }


        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        [HttpPost]
        [Authorize]
        public PartialViewResult UpComingScheduleReport(FormCollection formCollection)
        {
            byte[] bytes = _transactionalReportService.UpComingScheduleReport(DateTime.Today, DateTime.Today, User.Identity.Name, User.Identity.GetConcernId());
            TempData["ReportData"] = bytes;
            return PartialView("~/Views/Shared/_ReportViewer.cshtml");
        }

        [HttpPost]
        public JsonResult ChangeRemindDate(int CustomerID, DateTime RemindDate)
        {
            if (CustomerID > 0)
            {
                var customer = _CustomerService.GetCustomerById(CustomerID);
                customer.RemindDate = RemindDate;
                _CustomerService.UpdateCustomer(customer);
                _CustomerService.SaveCustomer();
                return Json(true);
            }
            return Json(false);
        }
        #region Get Widget details for dashboard
        [HttpGet]
        [Authorize]
        public JsonResult GetLoanIncomeDetails(string dataLength)
        {
            var widgetData = _SysInfoService.GetHomeWidgetLoanExpense(dataLength, User.Identity.GetConcernId());
            return Json(widgetData, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Get Yearly income
        [HttpGet]
        [Authorize]
        public JsonResult GetIncomeForFullYear()
        {
            var widgetData = _SysInfoService.GetFullYearIncome(User.Identity.GetConcernId());
            return Json(widgetData, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region Get Yearly income
        [HttpGet]
        [Authorize]
        public JsonResult GetExpenseForFullYear()
        {
            var widgetData = _SysInfoService.GetFullYearExpense(User.Identity.GetConcernId());
            return Json(widgetData, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Get Yearly Purchase
        [HttpGet]
        [Authorize]
        public JsonResult GetPurchaseForFullYear()
        {
            var widgetData = _SysInfoService.GetFullYearPurchase(User.Identity.GetConcernId());
            return Json(widgetData, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region Get Yearly Sale
        [HttpGet]
        [Authorize]
        public JsonResult GetSaleForFullYear()
        {
            var widgetData = _SysInfoService.GetFullYearSale(User.Identity.GetConcernId());
            return Json(widgetData, JsonRequestBehavior.AllowGet);
        }
        #endregion


        [HttpPost]
        public JsonResult getConcernName()
        {
            var userId = Convert.ToInt32(User.Identity.GetUserId());
            var userName = _user.GetUserNameById(userId);
            if (userName != null)
            {
                return Json(new { Status = true, Name = userName }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Status = false, Name = "" }, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        [Authorize]
        public JsonResult GetDailySalesAmt(string dataLength)
        {
            int ConcernID = User.Identity.GetConcernId();
            List<TOHomeWidget> widgetData = new List<TOHomeWidget>();
            if (ConcernID != 9)
            {
                widgetData = _SysInfoService.GetHomeWidgeSales(dataLength, User.Identity.GetConcernId());
            }

            return Json(widgetData, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        [Authorize]
        public JsonResult GetYearlyData(string dataLength)
        {
            int ConcernID = User.Identity.GetConcernId();
            List<TOHomeWidget> widgetData = new List<TOHomeWidget>();

            if (ConcernID != 9)
            {
                widgetData = _SysInfoService.GetYearlyData(dataLength, ConcernID);
            }

            return Json(widgetData, JsonRequestBehavior.AllowGet);
        }

    }
}