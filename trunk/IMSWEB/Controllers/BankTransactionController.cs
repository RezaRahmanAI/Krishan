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
namespace IMSWEB.Controllers
{
    [Authorize]
    public class BankTransactionController : CoreController
    {
        IBankService _baseBankService;
        IBankTransactionService _bankTransactionService;
        ICashCollectionService _CashCollectionService;
        IMiscellaneousService<BankTransaction> _miscellaneousService;
        IMapper _mapper;
        string _photoPath = "~/Content/photos/products";
        ISMSStatusService _SMSService;
        ISystemInformationService _SysService;
        ISMSStatusService _SMSStatusService;
        ICustomerService _CustomerService;
        private readonly ICashAccountService _cashAccountService;
        private readonly ISMSBillPaymentBkashService _smsBillPaymentBkashService;
        public BankTransactionController
            (
            IErrorService errorService,
            IBankService baseBankService,
            IBankTransactionService bankTransactionService,
            ICashCollectionService CashCollectionService,
            IMiscellaneousService<BankTransaction> miscellaneousService,
            IMapper mapper, ISMSStatusService SMSService, ISystemInformationService SysService,
            ISMSStatusService SMSStatusService, ICustomerService CustomerService, ICashAccountService cashAccountService, ISMSBillPaymentBkashService smsBillPaymentBkashService
            )
            : base(errorService)
        {
            _bankTransactionService = bankTransactionService;
            _CashCollectionService = CashCollectionService;
            _miscellaneousService = miscellaneousService;
            _baseBankService = baseBankService;
            _mapper = mapper;
            _SMSService = SMSService;
            _SMSStatusService = SMSStatusService;
            _SysService = SysService;
            _CustomerService = CustomerService;
            _cashAccountService = cashAccountService;
            _smsBillPaymentBkashService = smsBillPaymentBkashService;
        }

        [HttpGet]
        [Authorize]
        [Route("index")]
        public async Task<ActionResult> Index()
        {
            var DateRange = GetFirstAndLastDateOfMonth(DateTime.Today);
            ViewBag.FromDate = DateRange.Item1;
            ViewBag.ToDate = DateRange.Item2;
            var customBankTransactionAsync = _bankTransactionService.GetAllBankTransactionAsync(ViewBag.FromDate, ViewBag.ToDate);
            var vm = _mapper.Map<IEnumerable<Tuple<int, string, string, string, string, string, string, Tuple<decimal, DateTime?, string, string, string, string>>>, IEnumerable<GetBankTransactionViewModel>>(await customBankTransactionAsync);

            return View(vm);
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult> Index(FormCollection formCollection)
        {
            if (!string.IsNullOrEmpty(formCollection["FromDate"]))
                ViewBag.FromDate = Convert.ToDateTime(formCollection["FromDate"]);
            if (!string.IsNullOrEmpty(formCollection["ToDate"]))
                ViewBag.ToDate = Convert.ToDateTime(formCollection["ToDate"]);
            var customBankTransactionAsync = _bankTransactionService.GetAllBankTransactionAsync(ViewBag.FromDate, ViewBag.ToDate);
            var vm = _mapper.Map<IEnumerable<Tuple<int, string, string, string, string, string, string, Tuple<decimal, DateTime?, string, string, string, string>>>, IEnumerable<GetBankTransactionViewModel>>(await customBankTransactionAsync);
            return View("Index", vm);
        }

        [HttpGet]
        [Authorize]
        [Route("create")]
        public ActionResult Create()
        {
            string TransactionNo = _miscellaneousService.GetUniqueKey(x => int.Parse(x.TransactionNo));
            return View(new CreateBankTransactionViewModel { TransactionType = EnumTransactionType.Deposit, TransactionNo = TransactionNo });
        }

        [HttpPost]
        [Authorize]
        [Route("create/returnUrl")]
        public async Task<ActionResult> Create(CreateBankTransactionViewModel newBankTransaction, FormCollection formCollection,
            HttpPostedFileBase picture, string returnUrl)
        {
            CheckAndAddModelError(formCollection, newBankTransaction);

            if (!ModelState.IsValid)
                return View(newBankTransaction);

            if (newBankTransaction != null)
            {
                MapFormCollectionValueWithNewEntity(newBankTransaction, formCollection);
                var bankTransaction = _mapper.Map<CreateBankTransactionViewModel, BankTransaction>(newBankTransaction);
                var bank = _baseBankService.GetBankById(bankTransaction.BankID);
                var FirstCashAcc = _cashAccountService.GetAll().FirstOrDefault(i => i.ConcernId == User.Identity.GetConcernId());

                if (bank.TotalAmount < bankTransaction.Amount)
                {
                    if (bankTransaction.TransactionType == (int)EnumTransactionType.Withdraw || bankTransaction.TransactionType == (int)EnumTransactionType.FundTransfer)
                    {
                        ModelState.AddModelError("Amount", "Amount is not available");
                        return View(newBankTransaction);
                    }
                }

                if (bankTransaction.TransactionType == (int)EnumTransactionType.Deposit)
                {
                    bankTransaction.PayCashAccountId = FirstCashAcc.Id;
                    _CashCollectionService.UpdateTotalDue(0, 0, Convert.ToInt32(bankTransaction.BankID), 0, Convert.ToDecimal(bankTransaction.Amount));
                }
                else if (bankTransaction.TransactionType == (int)EnumTransactionType.Withdraw)
                {
                    bankTransaction.PayCashAccountId = FirstCashAcc.Id;
                    _CashCollectionService.UpdateTotalDue(0, 0, 0, Convert.ToInt32(bankTransaction.BankID), Convert.ToDecimal(bankTransaction.Amount));

                }
                //else if (bankTransaction.TransactionType == (int)EnumTransactionType.CashCollection)
                //{
                //    if (bankTransaction.CustomerID != 0)
                //        _CashCollectionService.UpdateTotalDue(Convert.ToInt32(bankTransaction.CustomerID), 0, Convert.ToInt32(bankTransaction.BankID), 0, Convert.ToDecimal(bankTransaction.Amount));
                //    else
                //    {
                //        ModelState.AddModelError("CustomerID", "Customer is required");
                //        return View(newBankTransaction);
                //    }
                //}
                //else if (bankTransaction.TransactionType == (int)EnumTransactionType.CashDelivery)
                //{
                //    if (bankTransaction.SupplierID != 0)
                //        _CashCollectionService.UpdateTotalDue(0, Convert.ToInt32(bankTransaction.SupplierID), 0, Convert.ToInt32(bankTransaction.BankID), Convert.ToDecimal(bankTransaction.Amount));
                //    else
                //    {

                //        ModelState.AddModelError("SupplierID", "Supplier is required");
                //        return View(newBankTransaction);
                //    }
                //}
                else if (bankTransaction.TransactionType == (int)EnumTransactionType.FundTransfer)
                {
                    //    _CashCollectionService.UpdateTotalDue(0, 0, 0, Convert.ToInt32(bankTransaction.BankID), Convert.ToDecimal(bankTransaction.Amount));
                    if (bankTransaction.AnotherBankID != 0)
                        _CashCollectionService.UpdateTotalDue(0, 0, Convert.ToInt32(bankTransaction.AnotherBankID), Convert.ToInt32(bankTransaction.BankID), Convert.ToDecimal(bankTransaction.Amount));
                    else
                    {
                        ModelState.AddModelError("AnotherBankID", "Another Bank is required");
                        return View(newBankTransaction);
                    }
                }
                else if (bankTransaction.AnotherBankID != 0)
                    _CashCollectionService.UpdateTotalDue(0, 0, Convert.ToInt32(bankTransaction.AnotherBankID), 0, Convert.ToDecimal(bankTransaction.Amount));

                bankTransaction.ConcernID = User.Identity.GetConcernId();
                AddAuditTrail(bankTransaction, true);
                _bankTransactionService.AddBankTransaction(bankTransaction);
                _bankTransactionService.SaveBankTransaction();
                TempData["BankTranID"] = bankTransaction.BankTranID;
                AddToastMessage("", "Bank Transaction has been saved successfully.", ToastType.Success);

                #region SMS Service
                if (bankTransaction.TransactionType == (int)EnumTransactionType.CashCollection)
                {
                    var oCustomer = _CustomerService.GetCustomerById((int)bankTransaction.CustomerID);
                    List<SMSRequest> sms = new List<SMSRequest>(){
                        new SMSRequest()
                        {
                        MobileNo=oCustomer.ContactNo,
                        CustomerID=oCustomer.CustomerID,
                        CustomerCode = oCustomer.Code,
                        TransNumber = bankTransaction.TransactionNo,
                        Date=(DateTime)bankTransaction.TranDate,
                        PreviousDue=oCustomer.TotalDue+(decimal)bankTransaction.Amount,
                        ReceiveAmount=(decimal)bankTransaction.Amount,
                        PresentDue=oCustomer.TotalDue,
                        SMSType = EnumSMSType.CashCollection
                        }
                    };

                    var SystemInfo = _SysService.GetSystemInformationByConcernId(User.Identity.GetConcernId());
                    int concernId = User.Identity.GetConcernId();
                    decimal previousBalance;
                    SMSPaymentMaster smsAmountDetails = _smsBillPaymentBkashService.GetByConcernId(concernId);
                    previousBalance = smsAmountDetails.TotalRecAmt;
                    var sysInfos = _SysService.GetSystemInformationByConcernId(User.Identity.GetConcernId());
                    decimal smsFee = sysInfos.smsCharge;
                    if (smsAmountDetails.TotalRecAmt > 1)
                    {
                        var response = await Task.Run(() => SMSHTTPService.SendSMSAsync(EnumOnnoRokomSMSType.OneToOne, sms, previousBalance, SystemInfo, User.Identity.GetUserId<int>()));

                        if (response.Count > 0)
                        {
                            decimal smsBalanceCount = 0m;
                            foreach (var item in response)
                            {
                                smsBalanceCount = smsBalanceCount + item.NoOfSMS;
                                if (item.NoOfSMS == 0)
                                {
                                    AddToastMessage("", "Plz Check SMS Balance, Or Check Error Status ", ToastType.Error);
                                }
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
                #endregion
                return RedirectToAction("Index");
            }
            else
            {
                AddToastMessage("", "No Transaction data found to save.", ToastType.Error);
                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        [Authorize]
        [Route("edit/{id}")]
        public ActionResult Edit(int id)
        {
            var bankTransaction = _bankTransactionService.GetBankTransactionById(id);
            if (!IsDateValid(Convert.ToDateTime(bankTransaction.TranDate)))
                return RedirectToAction("Index");
            var vmodel = _mapper.Map<BankTransaction, CreateBankTransactionViewModel>(bankTransaction);
            return View("Create", vmodel);
        }

        [HttpPost]
        [Authorize]
        [Route("edit/returnUrl")]
        public ActionResult Edit(CreateBankTransactionViewModel newBankTransaction, FormCollection formCollection,
            HttpPostedFileBase picture, string returnUrl)
        {
            CheckAndAddModelError(formCollection, newBankTransaction);
            //if (string.IsNullOrEmpty(newProduct.PicturePath))
            //    ModelState.AddModelError("PicturePath", "Picture is required");

            if (!ModelState.IsValid)
                return View("Create", newBankTransaction);

            if (newBankTransaction != null)
            {
                var existingBankTransaction = _bankTransactionService.GetBankTransactionById(int.Parse(newBankTransaction.BankTranID));

                #region old bank a/c and customer due update

                if (existingBankTransaction.TransactionType == (int)EnumTransactionType.Deposit)
                {
                    _CashCollectionService.UpdateTotalDue(0, 0, Convert.ToInt32(existingBankTransaction.BankID), 0, -Convert.ToDecimal(existingBankTransaction.Amount));
                }
                else if (existingBankTransaction.TransactionType == (int)EnumTransactionType.Withdraw)
                {
                    _CashCollectionService.UpdateTotalDue(0, 0, 0, Convert.ToInt32(existingBankTransaction.BankID), -Convert.ToDecimal(existingBankTransaction.Amount));

                }
                //else if (existingBankTransaction.TransactionType == (int)EnumTransactionType.CashCollection)
                //{
                //    if (existingBankTransaction.CustomerID != 0)
                //        _CashCollectionService.UpdateTotalDue(Convert.ToInt32(existingBankTransaction.CustomerID), 0, Convert.ToInt32(existingBankTransaction.BankID), 0, -Convert.ToDecimal(existingBankTransaction.Amount));
                //    else
                //    {
                //        ModelState.AddModelError("CustomerID", "Customer is required");
                //        return View(newBankTransaction);
                //    }
                //}
                //else if (existingBankTransaction.TransactionType == (int)EnumTransactionType.CashDelivery)
                //{
                //    if (existingBankTransaction.SupplierID != 0)
                //        _CashCollectionService.UpdateTotalDue(0, Convert.ToInt32(existingBankTransaction.SupplierID), 0, Convert.ToInt32(existingBankTransaction.BankID), -Convert.ToDecimal(existingBankTransaction.Amount));
                //    else
                //    {

                //        ModelState.AddModelError("SupplierID", "Supplier is required");
                //        return View(newBankTransaction);
                //    }
                //}
                else if (existingBankTransaction.TransactionType == (int)EnumTransactionType.FundTransfer)
                {
                    //    _CashCollectionService.UpdateTotalDue(0, 0, 0, Convert.ToInt32(existingBankTransaction.BankID), Convert.ToDecimal(existingBankTransaction.Amount));
                    if (existingBankTransaction.AnotherBankID != 0)
                        _CashCollectionService.UpdateTotalDue(0, 0, Convert.ToInt32(existingBankTransaction.AnotherBankID), Convert.ToInt32(existingBankTransaction.BankID), -Convert.ToDecimal(existingBankTransaction.Amount));
                    else
                    {
                        ModelState.AddModelError("AnotherBankID", "Another Bank is required");
                        return View(newBankTransaction);
                    }
                }
                else if (existingBankTransaction.AnotherBankID != 0)
                    _CashCollectionService.UpdateTotalDue(0, 0, Convert.ToInt32(existingBankTransaction.AnotherBankID), 0, -Convert.ToDecimal(existingBankTransaction.Amount));
                #endregion

                MapFormCollectionValueWithExistingEntity(existingBankTransaction, formCollection);
                existingBankTransaction.TranDate = newBankTransaction.TranDate;
                existingBankTransaction.TransactionNo = newBankTransaction.TransactionNo;

                existingBankTransaction.Amount = newBankTransaction.Amount;

                existingBankTransaction.ConcernID = User.Identity.GetConcernId();

                _bankTransactionService.UpdateBankTransaction(existingBankTransaction);
                _bankTransactionService.SaveBankTransaction();


                #region New bank a/c and customer due update

                if (existingBankTransaction.TransactionType == (int)EnumTransactionType.Deposit)
                {
                    _CashCollectionService.UpdateTotalDue(0, 0, Convert.ToInt32(existingBankTransaction.BankID), 0, Convert.ToDecimal(existingBankTransaction.Amount));
                }
                else if (existingBankTransaction.TransactionType == (int)EnumTransactionType.Withdraw)
                {
                    _CashCollectionService.UpdateTotalDue(0, 0, 0, Convert.ToInt32(existingBankTransaction.BankID), Convert.ToDecimal(existingBankTransaction.Amount));

                }
                //else if (existingBankTransaction.TransactionType == (int)EnumTransactionType.CashCollection)
                //{
                //    if (existingBankTransaction.CustomerID != 0)
                //        _CashCollectionService.UpdateTotalDue(Convert.ToInt32(existingBankTransaction.CustomerID), 0, Convert.ToInt32(existingBankTransaction.BankID), 0, Convert.ToDecimal(existingBankTransaction.Amount));
                //    else
                //    {
                //        ModelState.AddModelError("CustomerID", "Customer is required");
                //        return View(newBankTransaction);
                //    }
                //}
                //else if (existingBankTransaction.TransactionType == (int)EnumTransactionType.CashDelivery)
                //{
                //    if (existingBankTransaction.SupplierID != 0)
                //        _CashCollectionService.UpdateTotalDue(0, Convert.ToInt32(existingBankTransaction.SupplierID), 0, Convert.ToInt32(existingBankTransaction.BankID), Convert.ToDecimal(existingBankTransaction.Amount));
                //    else
                //    {

                //        ModelState.AddModelError("SupplierID", "Supplier is required");
                //        return View(newBankTransaction);
                //    }
                //}
                else if (existingBankTransaction.TransactionType == (int)EnumTransactionType.FundTransfer)
                {
                    //    _CashCollectionService.UpdateTotalDue(0, 0, 0, Convert.ToInt32(existingBankTransaction.BankID), Convert.ToDecimal(existingBankTransaction.Amount));
                    if (existingBankTransaction.AnotherBankID != 0)
                        _CashCollectionService.UpdateTotalDue(0, 0, Convert.ToInt32(existingBankTransaction.AnotherBankID), Convert.ToInt32(existingBankTransaction.BankID), Convert.ToDecimal(existingBankTransaction.Amount));
                    else
                    {
                        ModelState.AddModelError("AnotherBankID", "Another Bank is required");
                        return View(newBankTransaction);
                    }
                }
                else if (existingBankTransaction.AnotherBankID != 0)
                    _CashCollectionService.UpdateTotalDue(0, 0, Convert.ToInt32(existingBankTransaction.AnotherBankID), 0, Convert.ToDecimal(existingBankTransaction.Amount));
                #endregion
                TempData["BankTranID"] = existingBankTransaction.BankTranID;
                AddToastMessage("", "BankTransaction has been updated successfully.", ToastType.Success);
                return RedirectToAction("Index");
            }
            else
            {
                AddToastMessage("", "No Product data found to update.", ToastType.Error);
                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        [Authorize]
        [Route("delete/{id}")]
        public ActionResult Delete(int id)
        {
            var existingBankTransaction = _bankTransactionService.GetBankTransactionById(id);
            if (!IsDateValid(Convert.ToDateTime(existingBankTransaction.TranDate)))
            {
                return RedirectToAction("Index");
            }
            #region old bank a/c and customer due update

            if (existingBankTransaction.TransactionType == (int)EnumTransactionType.Deposit)
            {
                _CashCollectionService.UpdateTotalDue(0, 0, Convert.ToInt32(existingBankTransaction.BankID), 0, -Convert.ToDecimal(existingBankTransaction.Amount));
            }
            else if (existingBankTransaction.TransactionType == (int)EnumTransactionType.Withdraw)
            {
                _CashCollectionService.UpdateTotalDue(0, 0, 0, Convert.ToInt32(existingBankTransaction.BankID), -Convert.ToDecimal(existingBankTransaction.Amount));

            }
            //else if (existingBankTransaction.TransactionType == (int)EnumTransactionType.CashCollection)
            //{
            //    if (existingBankTransaction.CustomerID != 0)
            //        _CashCollectionService.UpdateTotalDue(Convert.ToInt32(existingBankTransaction.CustomerID), 0, Convert.ToInt32(existingBankTransaction.BankID), 0, -Convert.ToDecimal(existingBankTransaction.Amount));
            //}
            //else if (existingBankTransaction.TransactionType == (int)EnumTransactionType.CashDelivery)
            //{
            //    if (existingBankTransaction.SupplierID != 0)
            //        _CashCollectionService.UpdateTotalDue(0, Convert.ToInt32(existingBankTransaction.SupplierID), 0, Convert.ToInt32(existingBankTransaction.BankID), -Convert.ToDecimal(existingBankTransaction.Amount));
            //}
            else if (existingBankTransaction.TransactionType == (int)EnumTransactionType.FundTransfer)
            {
                //    _CashCollectionService.UpdateTotalDue(0, 0, 0, Convert.ToInt32(existingBankTransaction.BankID), Convert.ToDecimal(existingBankTransaction.Amount));
                if (existingBankTransaction.AnotherBankID != 0)
                    _CashCollectionService.UpdateTotalDue(0, 0, Convert.ToInt32(existingBankTransaction.AnotherBankID), Convert.ToInt32(existingBankTransaction.BankID), -Convert.ToDecimal(existingBankTransaction.Amount));
            }

            #endregion
            _bankTransactionService.DeleteBankTransaction(id);
            _bankTransactionService.SaveBankTransaction();
            AddToastMessage("", "BankTransaction has been deleted successfully.", ToastType.Success);
            return RedirectToAction("Index");
        }

        private void CheckAndAddModelError(FormCollection formCollection, CreateBankTransactionViewModel newBankTransaction)
        {
            if (string.IsNullOrEmpty(formCollection["BanksId"]))
                ModelState.AddModelError("BankID", "Bank is required");
            else
            {
                newBankTransaction.BankID = formCollection["BanksId"].ToString();
                if (!string.IsNullOrEmpty(formCollection["AnotherBanksId"]))
                {
                    if (formCollection["BanksId"].Equals(formCollection["AnotherBanksId"]))
                        ModelState.AddModelError("BankID", "From A/C,To A/C number can't be same.");

                    newBankTransaction.AnotherBankID = formCollection["AnotherBanksId"].ToString();
                }
            }
            if (string.IsNullOrEmpty(formCollection["TransactionType"]))
                ModelState.AddModelError("TransactionType", "TransactionType is required");

            if (!IsDateValid(Convert.ToDateTime(newBankTransaction.TranDate)))
                ModelState.AddModelError("TranDate", "Back dated entry is not valid");

        }

        private void MapFormCollectionValueWithNewEntity(CreateBankTransactionViewModel newBankTransaction,
            FormCollection formCollection)
        {
            newBankTransaction.BankID = formCollection["BanksId"] != "" ? formCollection["BanksId"] : "0";
            newBankTransaction.CustomerID = formCollection["CustomersId"] != "" ? formCollection["CustomersId"] : "0";
            newBankTransaction.SupplierID = formCollection["SuppliersId"] != "" ? formCollection["SuppliersId"] : "0";
            newBankTransaction.AnotherBankID = formCollection["AnotherBanksId"] != "" ? formCollection["AnotherBanksId"] : "0";
        }

        private void MapFormCollectionValueWithExistingEntity(BankTransaction bankTransaction,
            FormCollection formCollection)
        {
            bankTransaction.BankID = int.Parse(formCollection["BanksId"]);
            bankTransaction.TransactionType = int.Parse(formCollection["TransactionType"]);
        }

        [HttpGet]
        [Authorize]
        public ActionResult BankTransactionReport()
        {
            return View();
        }

        [HttpGet]
        [Authorize]
        public ActionResult BankLedger()
        {
            return View();
        }


        [HttpGet]
        [Authorize]
        public ActionResult MoneyReceipt(int id)
        {
            TempData["BankTranID"] = id;
            return RedirectToAction("Index");
        }
    }
}