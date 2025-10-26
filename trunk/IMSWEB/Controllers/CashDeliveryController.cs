using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;

using AutoMapper;
using IMSWEB.Model;
using IMSWEB.Service;
using IMSWEB.Model.TOs;

namespace IMSWEB
{
    [Authorize]
    [RoutePrefix("Collection-item")]

    public class CashDeliveryController : CoreController
    {
        ICashCollectionService _CashCollectionService;
        ICustomerService _CustomerService;
        ISupplierService _SupplierService;
        IMiscellaneousService<CashCollection> _miscellaneousService;
        private readonly IBankService _bankService;
        private readonly ICashAccountService _cashAccountService;
        IMapper _mapper;

        public CashDeliveryController(IErrorService errorService,
            ICashCollectionService cashCollectionService, ICustomerService customerService,
            ISupplierService supplierService, IMiscellaneousService<CashCollection> miscellaneousService,
            IMapper mapper, IBankService bankService, ICashAccountService cashAccountService)
            : base(errorService)
        {
            _CashCollectionService = cashCollectionService;
            _CustomerService = customerService;
            _SupplierService = supplierService;
            _miscellaneousService = miscellaneousService;
            _mapper = mapper;
            _bankService = bankService;
            _cashAccountService = cashAccountService;

        }

        [HttpGet]
        [Authorize]
        [Route("index")]
        public async Task<ActionResult> Index()
        {
            var DateRange = GetFirstAndLastDateOfMonth(DateTime.Today);
            ViewBag.FromDate = DateRange.Item1;
            ViewBag.ToDate = DateRange.Item2;

            var itemsAsync = _CashCollectionService.GetAllCashDelivaeryAsync(ViewBag.FromDate, ViewBag.ToDate);
            var vmodel = _mapper.Map<IEnumerable<Tuple<int, DateTime, string, string, string,
                string, string>>, IEnumerable<GetCashCollectionViewModel>>(await itemsAsync);
            return View(vmodel);
        }

        [HttpPost]
        public async Task<ActionResult> Index(FormCollection formCollection)
        {
            if (!string.IsNullOrEmpty(formCollection["FromDate"]))
                ViewBag.FromDate = Convert.ToDateTime(formCollection["FromDate"]);
            if (!string.IsNullOrEmpty(formCollection["ToDate"]))
                ViewBag.ToDate = Convert.ToDateTime(formCollection["ToDate"]);

            var itemsAsync = _CashCollectionService.GetAllCashDelivaeryAsync(ViewBag.FromDate, ViewBag.ToDate);
            var vmodel = _mapper.Map<IEnumerable<Tuple<int, DateTime, string, string, string,
                string, string>>, IEnumerable<GetCashCollectionViewModel>>(await itemsAsync);
            return View(vmodel);
        }


        [HttpGet]
        [Authorize]
        [Route("create")]
        public ActionResult Create()
        {
            ViewBag.SupplierIds = GetAllSupplierForDDL();
            var payItems = _CashCollectionService.GetAllPayTypeHeadForPO().Select(d => new SelectListItem
            {
                Value = d.ExpenseItemID.ToString(),
                Text = d.Name
            }).ToList();
            var fpay = _CashCollectionService.GetAllPayTypeHeadForPO().FirstOrDefault();
            string recptNo = _miscellaneousService.GetUniqueKey(x => int.Parse(x.ReceiptNo));
            return View(new CreateCashCollectionViewModel { ReceiptNo = recptNo, PayItems = payItems, PayHeadId = fpay.ExpenseItemID.ToString() });
        }

        [HttpPost]
        [Authorize]
        [Route("create/returnUrl")]
        public ActionResult Create(CreateCashCollectionViewModel newCashCollection, FormCollection formCollection, string returnUrl, string PayType)
        {
            ViewBag.SupplierIds = GetAllSupplierForDDL();
            var payItems = _CashCollectionService.GetAllPayTypeHeadForPO().Select(d => new SelectListItem
            {
                Value = d.ExpenseItemID.ToString(),
                Text = d.Name
            }).ToList();
            newCashCollection.PayItems = payItems;
            AddModelError(newCashCollection, formCollection);

            if (!ModelState.IsValid)
                return View(newCashCollection);

            if (newCashCollection != null)
            {
                newCashCollection.CreateDate = DateTime.Today.ToString();
                newCashCollection.CreatedBy = (User.Identity.GetUserId<string>());
                newCashCollection.ConcernID = User.Identity.GetConcernId().ToString();

                newCashCollection.CustomerID = "0";
                newCashCollection.EmployeeID = 0;
                newCashCollection.CCBankID = "0";
                newCashCollection.SupplierID = formCollection["SuppliersId"];
                newCashCollection.TransactionType = EnumTranType.ToCompany;
                newCashCollection.ModifiedDate = DateTime.Now.ToString();


                if (newCashCollection.AccountNo == null)
                    newCashCollection.AccountNo = "No A/C";

                var cashCollection = _mapper.Map<CreateCashCollectionViewModel, CashCollection>(newCashCollection);
                cashCollection.PayCashAccountId = PayType.Equals("CA") ? int.Parse(newCashCollection.PayHeadId) : (int?)null;
                cashCollection.PayBankId = PayType.Equals("B") ? int.Parse(newCashCollection.PayHeadId) : (int?)null;

                if (cashCollection.PayCashAccountId.HasValue)
                {
                    cashCollection.PaymentType = EnumPayType.Cash;
                }
                else
                {
                    cashCollection.PaymentType = EnumPayType.Bank;

                }

                if (cashCollection.PayBankId.HasValue)
                {
                    Bank bank = _bankService.GetBankById(cashCollection.PayBankId.Value);
                    bank.TotalAmount -= cashCollection.Amount;
                    _bankService.UpdateBank(bank);
                }
                if (cashCollection.PayCashAccountId.HasValue)
                {
                    CashAccount ca = _cashAccountService.GetById(cashCollection.PayCashAccountId.Value);
                    ca.TotalBalance -= cashCollection.Amount;
                    _cashAccountService.Update(ca);
                }

                _CashCollectionService.AddCashCollection(cashCollection);
                _CashCollectionService.SaveCashCollection();



                _CashCollectionService.UpdateTotalDue(0, Convert.ToInt32(newCashCollection.SupplierID), 0, 0, Convert.ToDecimal(Convert.ToDecimal(newCashCollection.Amount) + Convert.ToDecimal(newCashCollection.AdjustAmt)));
                TempData["CashCollectionID"] = cashCollection.CashCollectionID;

                AddToastMessage("", "Item has been saved successfully.", ToastType.Success);
                return RedirectToAction("Index");
            }
            else
            {
                AddToastMessage("", "No Item data found to create.", ToastType.Error);
                return RedirectToAction("Index");
            }
        }
        private void AddModelError(CreateCashCollectionViewModel newCashCollection, FormCollection formCollection)
        {
            if (string.IsNullOrEmpty(formCollection["SuppliersId"]))
                ModelState.AddModelError("SupplierID", "Supplier is Required.");
            else
                newCashCollection.SupplierID = formCollection["SuppliersId"];

            if (decimal.Parse(GetDefaultIfNull(newCashCollection.Amount)) < 0m)
                ModelState.AddModelError("Amount", "Amount can't be negative");
            int CCID = Convert.ToInt32(newCashCollection.CashCollectionID);
            if (_CashCollectionService.GetAllCashCollection().Any(i => i.ReceiptNo.Equals(newCashCollection.ReceiptNo) && i.CashCollectionID != CCID))
                ModelState.AddModelError("ReceiptNo", "This ReceiptNo is already exists.");

            if (!IsDateValid(Convert.ToDateTime(newCashCollection.EntryDate)))
            {
                ModelState.AddModelError("EntryDate", "Back dated entry is not valid.");
            }
            if (string.IsNullOrEmpty(newCashCollection.PayHeadId))
            {
                ModelState.AddModelError("PayHeadId", "Payment type is required!");
            }
        }
        [HttpGet]
        [Authorize]
        [Route("edit/{id}")]
        public ActionResult Edit(int id)
        {
            //var customerItems = _CustomerService.GetAllCustomer().Select(cusItem
            //   => new SelectListItem { Text = cusItem.Name, Value = cusItem.CustomerID.ToString() }).ToList();

            //var supplierItems = _SupplierService.GetAllSupplier().Select(suppItem
            //    => new SelectListItem { Text = suppItem.Name, Value = suppItem.SupplierID.ToString() }).ToList();

            ViewBag.SupplierIds = GetAllSupplierForDDL();
            var payItems = _CashCollectionService.GetAllPayTypeHeadForPO().Select(d => new SelectListItem
            {
                Value = d.ExpenseItemID.ToString(),
                Text = d.Name
            }).ToList();
            var cashCollection = _CashCollectionService.GetCashCollectionById(id);

            var vmodel = _mapper.Map<CashCollection, CreateCashCollectionViewModel>(cashCollection);
            var Supplier = _SupplierService.GetSupplierById(Convert.ToInt32(vmodel.SupplierID));
            vmodel.CurrentDue = Supplier.TotalDue.ToString();
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
            return View("Create", vmodel);
        }

        [HttpPost]
        [Authorize]
        [Route("edit/returnUrl")]
        public ActionResult Edit(CreateCashCollectionViewModel newCashCollection, FormCollection formCollection, string returnUrl)
        {
            AddModelError(newCashCollection, formCollection);
            if (!ModelState.IsValid)
                return View("Create", newCashCollection);

            if (newCashCollection != null)
            {
                var cashCollection = _CashCollectionService.GetCashCollectionById(int.Parse(newCashCollection.CashCollectionID));

                cashCollection.PaymentType = newCashCollection.PaymentType;
                cashCollection.BankName = newCashCollection.BankName;
                cashCollection.BranchName = newCashCollection.BranchName;
                cashCollection.EntryDate = Convert.ToDateTime(newCashCollection.EntryDate);
                cashCollection.Amount = decimal.Parse(newCashCollection.Amount);
                cashCollection.AccountNo = newCashCollection.AccountNo;
                cashCollection.MBAccountNo = newCashCollection.MBAccountNo;
                cashCollection.BKashNo = newCashCollection.BKashNo;
                cashCollection.TransactionType = newCashCollection.TransactionType;
                cashCollection.CustomerID = 0;
                cashCollection.SupplierID = int.Parse(formCollection["SuppliersId"]);

                _CashCollectionService.UpdateTotalDueWhenEdit(0, (int)cashCollection.SupplierID, 0, cashCollection.CashCollectionID, (cashCollection.Amount + cashCollection.AdjustAmt));
                cashCollection.BalanceDue = _SupplierService.GetSupplierById((int)cashCollection.SupplierID).TotalDue;
                _CashCollectionService.UpdateCashCollection(cashCollection);
                _CashCollectionService.SaveCashCollection();
                TempData["CashCollectionID"] = cashCollection.CashCollectionID;
                AddToastMessage("", "Item has been updated successfully.", ToastType.Success);
                return RedirectToAction("Index");
            }
            else
            {
                AddToastMessage("", "No Item data found to update.", ToastType.Error);
                return RedirectToAction("Index");
            }
        }

        private List<TOIdNameDDL> GetAllSupplierForDDL()
        {

            var suppliers = _SupplierService.GetAllSupplierNew(User.Identity.GetConcernId()).Select(s => new TOIdNameDDL
            {
                Id = s.Id,
                Name = s.Name + "(" + s.Code + "), " + "Mobile:" + s.ContactNo + ", " + "Add:" + s.Address
            }).ToList();
            return suppliers;

        }

        [HttpGet]
        [Authorize]
        [Route("delete/{id}")]
        public ActionResult Delete(int id)
        {
            var CashDelivery = _CashCollectionService.GetCashCollectionById(id);
            int SupplierID = (int)CashDelivery.SupplierID;
            decimal amt = CashDelivery.Amount + CashDelivery.AdjustAmt;
            if (!IsDateValid(Convert.ToDateTime(CashDelivery.EntryDate)))
            {
                return RedirectToAction("Index");
            }
            _CashCollectionService.DeleteCashCollection(id);
            _CashCollectionService.SaveCashCollection();
            _CashCollectionService.UpdateTotalDue(0, SupplierID, 0, 0, -amt);

            AddToastMessage("", "Item has been deleted successfully.", ToastType.Success);
            return RedirectToAction("Index");
        }

        [HttpGet]
        [Authorize]
        public ActionResult CashDeliveryReport()
        {
            return View("CashDeliveryReport");
        }


        [HttpGet]
        [Authorize]
        public ActionResult MoneyReceipt(int id)
        {
            TempData["CashCollectionID"] = id;
            return RedirectToAction("Index");
        }


    }
}