using AutoMapper;
using IMSWEB.Model;
using IMSWEB.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using System.Web;
using System.Web.Mvc;
using IMSWEB.Model.TOs;

namespace IMSWEB.Controllers
{
    [Authorize]
    [RoutePrefix("expense-item")]
    public class ExpenditureController : CoreController
    {
        IExpenditureService _expenditureService;
        IExpenseItemService _expenseItemService;
        IMapper _mapper;
        IMiscellaneousService<Expenditure> _miscellService;
        ISisterConcernService _SisterConcernService;
        ISystemInformationService _SystemInformationService;
        IEmployeeService _employeeService;
        private readonly IBankService _bankService;
        private readonly ICashAccountService _cashAccountService;
        private readonly ICashCollectionService _cashCollectionService;

        public ExpenditureController(IErrorService errorService,
            IExpenditureService expenditureService, IExpenseItemService expenseItemService, IMapper mapper,
            IMiscellaneousService<Expenditure> miscellService, ISisterConcernService SisterConcernService, ICashCollectionService cashCollectionService,
            ISystemInformationService systemInformationService, IEmployeeService employeeService, IBankService bankService, ICashAccountService cashAccountService)
            : base(errorService)
        {
            _expenditureService = expenditureService;
            _expenseItemService = expenseItemService;
            _mapper = mapper;
            _miscellService = miscellService;
            _SisterConcernService = SisterConcernService;
            _SystemInformationService = systemInformationService;
            _employeeService = employeeService;
            _bankService = bankService;
            _cashAccountService = cashAccountService;
            _cashCollectionService = cashCollectionService;
        }

        [HttpGet]
        [Authorize]
        [Route("index")]
        public async Task<ActionResult> Index()
        {
            var DateRange = GetFirstAndLastDateOfMonth(DateTime.Today);
            ViewBag.FromDate = DateRange.Item1;
            ViewBag.ToDate = DateRange.Item2;

            if (User.IsInRole(ConstantData.ROLE_MOBILE_USER))
            {
                var expenditureList = _expenditureService.GetAllExpenditureByUserIDAsync(User.Identity.GetUserId<int>(), ViewBag.FromDate, ViewBag.ToDate);
                var vmodel = _mapper.Map<IEnumerable<Expenditure>, IEnumerable<CreateExpenditureViewModel>>(await expenditureList);
                return View(vmodel);
            }
            else
            {
                var expenditureList = _expenditureService.GetAllExpenditureAsync(ViewBag.FromDate, ViewBag.ToDate);
                var vmodel = _mapper.Map<IEnumerable<Expenditure>, IEnumerable<CreateExpenditureViewModel>>(await expenditureList);
                return View(vmodel);
            }
        }
        [HttpPost]
        [Authorize]
        [Route("index")]
        public async Task<ActionResult> Index(FormCollection formCollection)
        {
            if (!string.IsNullOrEmpty(formCollection["FromDate"]))
                ViewBag.FromDate = Convert.ToDateTime(formCollection["FromDate"]);
            if (!string.IsNullOrEmpty(formCollection["ToDate"]))
                ViewBag.ToDate = Convert.ToDateTime(formCollection["ToDate"]);

            if (User.IsInRole(ConstantData.ROLE_MOBILE_USER))
            {
                var expenditureList = _expenditureService.GetAllExpenditureByUserIDAsync(User.Identity.GetUserId<int>(), ViewBag.FromDate, ViewBag.ToDate);
                var vmodel = _mapper.Map<IEnumerable<Expenditure>, IEnumerable<CreateExpenditureViewModel>>(await expenditureList);
                return View(vmodel);
            }
            else
            {
                var expenditureList = _expenditureService.GetAllExpenditureAsync(ViewBag.FromDate, ViewBag.ToDate);
                var vmodel = _mapper.Map<IEnumerable<Expenditure>, IEnumerable<CreateExpenditureViewModel>>(await expenditureList);
                return View(vmodel);
            }
        }

        [HttpGet]
        [Authorize]
        [Route("create")]
        public ActionResult Create()
        {
            ViewBag.EmployeeIds = GetAllEmployeeForDDL();
            ViewBag.ExpenseItemsId = GetAllExpenseItemForDDL();
            var voucherNo = _miscellService.GetUniqueKey(i => i.ExpenditureID);
            var payItems = _cashCollectionService.GetAllPayTypeHeadForPO().Select(d => new SelectListItem
            {
                Value = d.ExpenseItemID.ToString(),
                Text = d.Name
            }).ToList();

            var fpay = _cashCollectionService.GetAllPayTypeHeadForPO().FirstOrDefault();
            ViewBag.IsEmployeeWiseTransEnable = _SystemInformationService.IsEmployeeWiseTransactionEnable();
            return View(new CreateExpenditureViewModel() { VoucherNo = voucherNo, PayItems = payItems, PayHeadId = fpay.ExpenseItemID.ToString() });
        }

        [HttpPost]
        [Authorize]
        [Route("create/returnUrl")]
        public ActionResult Create(CreateExpenditureViewModel newExpenditure, FormCollection formcollection, string returnUrl, string PayType)
        {
            ViewBag.EmployeeIds = GetAllEmployeeForDDL();
            ViewBag.ExpenseItemsId = GetAllExpenseItemForDDL();
            var payItems = _cashCollectionService.GetAllPayTypeHeadForPO().Select(d => new SelectListItem
            {
                Value = d.ExpenseItemID.ToString(),
                Text = d.Name
            }).ToList();
            newExpenditure.PayItems = payItems;
            CheckAndAddModelError(newExpenditure, formcollection);

            if (!ModelState.IsValid)
            {
                ViewBag.IsEmployeeWiseTransEnable = _SystemInformationService.IsEmployeeWiseTransactionEnable();
                return View(newExpenditure);
            }


            if (newExpenditure != null)
            {
                newExpenditure.CreateDate = DateTime.Today.ToString();
                newExpenditure.CreatedBy = (User.Identity.GetUserId<string>());
                newExpenditure.ConcernID = User.Identity.GetConcernId().ToString();
                var expenditure = _mapper.Map<CreateExpenditureViewModel, Expenditure>(newExpenditure);
                expenditure.ExpenseIncomeStatus = EnumCompanyTransaction.Expense;
                expenditure.PayCashAccountId = PayType.Equals("CA") ? int.Parse(newExpenditure.PayHeadId) : (int?)null;
                expenditure.PayBankId = PayType.Equals("B") ? int.Parse(newExpenditure.PayHeadId) : (int?)null;

                if (expenditure.PayBankId.HasValue)
                {
                    Bank bank = _bankService.GetBankById(expenditure.PayBankId.Value);
                    bank.TotalAmount -= expenditure.Amount;
                    _bankService.UpdateBank(bank);
                }
                if (expenditure.PayCashAccountId.HasValue)
                {
                    CashAccount ca = _cashAccountService.GetById(expenditure.PayCashAccountId.Value);
                    ca.TotalBalance -= expenditure.Amount;
                    _cashAccountService.Update(ca);
                }

                _expenditureService.AddExpenditure(expenditure);
                _expenditureService.SaveExpenditure();
                TempData["ExpenditureID"] = expenditure.ExpenditureID;
                AddToastMessage("", "Item has been saved successfully.", ToastType.Success);
                return RedirectToAction("Index");
            }
            else
            {
                AddToastMessage("", "No Item data found to create.", ToastType.Error);
                return RedirectToAction("Index");
            }
        }

        private void CheckAndAddModelError(CreateExpenditureViewModel newExpenditure, FormCollection formcollection)
        {
            if (!string.IsNullOrEmpty(formcollection["EntryDate"]))
                newExpenditure.EntryDate = formcollection["EntryDate"].ToString();

            if (!IsDateValid(Convert.ToDateTime(newExpenditure.EntryDate)))
                ModelState.AddModelError("EntryDate", "Back dated entry is not valid");

            if (string.IsNullOrEmpty(newExpenditure.VoucherNo))
                ModelState.AddModelError("VoucherNo", "VoucherNo is required.");

            if (string.IsNullOrEmpty(newExpenditure.ExpenseItemID))
                ModelState.AddModelError("ExpenseItemID", "Head is required.");
            int EXID = Convert.ToInt32(GetDefaultIfNull(newExpenditure.Id));
            if (_miscellService.GetDuplicateEntry(i => i.VoucherNo.Equals(newExpenditure.VoucherNo)
             && i.ExpenditureID != EXID) != null)
            {
                newExpenditure.VoucherNo = _miscellService.GetUniqueKey(i => i.ExpenditureID);
            }

            if (_SystemInformationService.IsEmployeeWiseTransactionEnable())
            {
                if (!string.IsNullOrEmpty(formcollection["EmployeesId"]))
                    newExpenditure.EmployeeID = Convert.ToString(formcollection["EmployeesId"]);
                else
                    ModelState.AddModelError("EmployeeID", "Employee is required.");
            }
            if (string.IsNullOrEmpty(newExpenditure.PayHeadId))
            {
                ModelState.AddModelError("PayHeadId", "Payment type is required!");
                //newExpenditure.PayCashAccountId = Convert.ToInt32(formcollection["PayHeadId"]);
            }

        }

        [HttpGet]
        [Authorize]
        [Route("edit/{id}")]
        public ActionResult Edit(int id)
        {
            ViewBag.EmployeeIds = GetAllEmployeeForDDL();
            ViewBag.ExpenseItemsId = GetAllExpenseItemForDDL();
            var payItems = _cashCollectionService.GetAllPayTypeHeadForPO().Select(d => new SelectListItem
            {
                Value = d.ExpenseItemID.ToString(),
                Text = d.Name
            }).ToList();
            var expenditure = _expenditureService.GetExpenditureById(id);
            if (!IsDateValid(expenditure.EntryDate))
                return RedirectToAction("Index");

            var vmodel = _mapper.Map<Expenditure, CreateExpenditureViewModel>(expenditure);
            vmodel.PayItems = payItems;
            string payType = string.Empty;
            if (vmodel.PayCashAccountId.HasValue)
            {
                vmodel.PayHeadId = vmodel.PayCashAccountId.ToString();
                payType = "CA";
            }
            else if (vmodel.PayBankId.HasValue)
            {
                vmodel.PayHeadId = vmodel.PayBankId.ToString();
                payType = "B";
            }
            ViewBag.IsEmployeeWiseTransEnable = _SystemInformationService.IsEmployeeWiseTransactionEnable();

            return View("Create", vmodel);
        }

        [HttpPost]
        [Authorize]
        [Route("edit/returnUrl")]
        public ActionResult Edit(CreateExpenditureViewModel newExpenditure, FormCollection formcollection, string returnUrl)
        {
            ViewBag.EmployeeIds = GetAllEmployeeForDDL();
            ViewBag.ExpenseItemsId = GetAllExpenseItemForDDL();
            CheckAndAddModelError(newExpenditure, formcollection);
            ViewBag.IsEmployeeWiseTransEnable = _SystemInformationService.IsEmployeeWiseTransactionEnable();

            if (!ModelState.IsValid)
                return View("Create", newExpenditure);

            if (newExpenditure != null)
            {
                var expenditure = _expenditureService.GetExpenditureById(int.Parse(newExpenditure.Id));

                expenditure.Amount = decimal.Parse(newExpenditure.Amount);
                expenditure.Purpose = newExpenditure.Purpose;
                expenditure.ExpenseItemID = int.Parse(newExpenditure.ExpenseItemID);
                expenditure.ModifiedBy = User.Identity.GetUserId<int>();
                expenditure.ModifiedDate = DateTime.Now;
                expenditure.EntryDate = Convert.ToDateTime(newExpenditure.EntryDate);
                expenditure.EmployeeID = int.Parse(newExpenditure.EmployeeID);
                expenditure.ExpenseIncomeStatus = EnumCompanyTransaction.Expense;
                _expenditureService.UpdateExpenditure(expenditure);
                _expenditureService.SaveExpenditure();
                TempData["ExpenditureID"] = expenditure.ExpenditureID;
                AddToastMessage("", "Item has been updated successfully.", ToastType.Success);
                return RedirectToAction("Index");
            }
            else
            {
                AddToastMessage("", "No Item data found to update.", ToastType.Error);
                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        [Authorize]
        [Route("delete/{id}")]
        public ActionResult Delete(int id)
        {
            var model = _expenditureService.GetExpenditureById(id);
            if (!IsDateValid(model.EntryDate))
            {
                return RedirectToAction("Index");
            }
            _expenditureService.DeleteExpenditure(id);
            _expenditureService.SaveExpenditure();
            AddToastMessage("", "Item has been deleted successfully.", ToastType.Success);
            return RedirectToAction("Index");
        }


        [HttpGet]
        [Authorize]
        [Route("Expenditure-report")]
        public ActionResult MiscellaneousReport()
        {
            return View("MiscellaneousReport");
        }

        [HttpGet]
        [Authorize]
        public ActionResult IncomeReport()
        {
            return View();
        }

        [HttpGet]
        [Authorize]
        public ActionResult MoneyReceipt(int id)
        {
            TempData["ExpenditureID"] = id;
            return RedirectToAction("Index");
        }

        [HttpGet]
        [Authorize]
        public ActionResult TrialBalance()
        {
            if (User.IsInRole(EnumUserRoles.superadmin.ToString()))
                PopulateConcernsDropdown();
            return View();
        }

        [HttpGet]
        [Authorize]
        public ActionResult ProfitLossAccount()
        {
            if (User.IsInRole(EnumUserRoles.superadmin.ToString()))
                PopulateConcernsDropdown();
            return View();
        }


        [HttpGet]
        [Authorize]
        public ActionResult BalanceSheet()
        {
            if (User.IsInRole(EnumUserRoles.superadmin.ToString()))
                PopulateConcernsDropdown();
            return View();
        }

        void PopulateConcernsDropdown()
        {
            ViewBag.Concerns = new SelectList(_SisterConcernService.GetAll(), "ConcernID", "Name");
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
        [HttpGet]
        [Authorize]
        public JsonResult GetExpenseItemInfoById(int expenseItemId)
        {

            var expenseItems = _expenseItemService.GetAllExpenseItemNew(User.Identity.GetConcernId(), expenseItemId).FirstOrDefault();
            return Json(expenseItems, JsonRequestBehavior.AllowGet);

        }
        private List<TOIdNameDDL> GetAllExpenseItemForDDL()
        {

            var expenseItems = _expenseItemService.GetAllExpenseItemNew(User.Identity.GetConcernId()).Select(s => new TOIdNameDDL
            {
                Id = s.Id,
                Name = s.Name + "(" + s.Code + ")"
            }).ToList();
            return expenseItems;

        }
    }
}