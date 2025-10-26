using AutoMapper;
using IMSWEB.Model;
using IMSWEB.Model.TOs;
using IMSWEB.Service;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace IMSWEB.Controllers
{
    [Authorize]
    [RoutePrefix("Collection-item")]

    public class CashCollectionController : CoreController
    {
        ICashCollectionService _CashCollectionService;
        ICustomerService _CustomerService;
        ISupplierService _SupplierService;
        IMiscellaneousService<CashCollection> _miscellaneousService;
        IMapper _mapper;
        IUserService _UserService;
        ISisterConcernService _SisterConcern;
        ISMSStatusService _SMSService;
        ISystemInformationService _SysService;
        ISMSStatusService _SMSStatusService;
        IRoleService roleService;
        ISalesOrderService _salesOrderService;
        private readonly IEmployeeWiseCustomerDueService _employeeWiseCustomerDueService;
        private readonly IEmployeeService _employeeService;
        private readonly IBankService _bankService;
        private readonly ICashAccountService _cashAccountService;
        private readonly ISMSBillPaymentBkashService _smsBillPaymentBkashService;


        public CashCollectionController(IErrorService errorService,
            ICashCollectionService cashCollectionService, ICustomerService customerService,
            ISupplierService supplierService, IMiscellaneousService<CashCollection> miscellaneousService,
            IMapper mapper, IUserService UserService, ISisterConcernService SisterConcern, ISMSStatusService SMSService, ISystemInformationService SysService,
            ISMSStatusService SMSStatusService, IRoleService _roleService, ISalesOrderService salesOrderService, IEmployeeWiseCustomerDueService employeeWiseCustomerDueService, IEmployeeService employeeService,
            IBankService bankService, ICashAccountService cashAccountService, ISMSBillPaymentBkashService smsBillPaymentBkashService)
            : base(errorService)
        {
            _CashCollectionService = cashCollectionService;
            _CustomerService = customerService;
            _SupplierService = supplierService;
            _miscellaneousService = miscellaneousService;
            _mapper = mapper;
            _UserService = UserService;
            _SisterConcern = SisterConcern;
            _SMSService = SMSService;
            _SMSStatusService = SMSStatusService;
            _SysService = SysService;
            roleService = _roleService;
            _salesOrderService = salesOrderService;
            _employeeWiseCustomerDueService = employeeWiseCustomerDueService;
            _employeeService = employeeService;
            _bankService = bankService;
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

            if (User.IsInRole(ConstantData.ROLE_MOBILE_USER))
            {
                var user = _UserService.GetUserById(User.Identity.GetUserId<int>());
                int EmployeeID = User.Identity.GetEmployeeID();
                var EMPitemsAsync = _CashCollectionService.GetAllCashCollByEmployeeIDAsync(user.EmployeeID, ViewBag.FromDate, ViewBag.ToDate);
                var EMPvmodel = _mapper.Map<IEnumerable<Tuple<int, DateTime, string, string, string,
                    string, string, Tuple<string, string, string>>>, IEnumerable<GetCashCollectionViewModel>>(await EMPitemsAsync);
                return View(EMPvmodel);
            }
            else
            {
                var itemsAsync = _CashCollectionService.GetAllCashCollAsync(ViewBag.FromDate, ViewBag.ToDate);
                var vmodel = _mapper.Map<IEnumerable<Tuple<int, DateTime, string, string, string,
                    string, string, Tuple<string, string, string>>>, IEnumerable<GetCashCollectionViewModel>>(await itemsAsync);
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
                //int EmployeeID = ConstantData.GetEmployeeIDByUSerID(User.Identity.GetUserId<int>());
                var user = _UserService.GetUserById(User.Identity.GetUserId<int>());
                int EmployeeID = User.Identity.GetEmployeeID();
                var EMPitemsAsync = _CashCollectionService.GetAllCashCollByEmployeeIDAsync(user.EmployeeID, ViewBag.FromDate, ViewBag.ToDate);
                var EMPvmodel = _mapper.Map<IEnumerable<Tuple<int, DateTime, string, string, string,
                    string, string, Tuple<string, string, string>>>, IEnumerable<GetCashCollectionViewModel>>(await EMPitemsAsync);
                return View(EMPvmodel);
            }
            else
            {
                var itemsAsync = _CashCollectionService.GetAllCashCollAsync(ViewBag.FromDate, ViewBag.ToDate);
                var vmodel = _mapper.Map<IEnumerable<Tuple<int, DateTime, string, string, string,
                    string, string, Tuple<string, string, string>>>, IEnumerable<GetCashCollectionViewModel>>(await itemsAsync);
                return View(vmodel);
            }
        }

        [HttpGet]
        [Authorize]
        [Route("create")]
        public ActionResult Create()
        {
            ViewBag.IsEmployeeWiseTransEnable = _SysService.IsEmployeeWiseTransactionEnable();
            ViewBag.CustomerIds = GetAllCustomerForDDL();
            var payItems = _CashCollectionService.GetAllPayTypeHeadForPO().Select(d => new SelectListItem
            {
                Value = d.ExpenseItemID.ToString(),
                Text = d.Name
            }).ToList();
            var fpay = _CashCollectionService.GetAllPayTypeHeadForPO().FirstOrDefault();
            string recptNo = _miscellaneousService.GetUniqueKey(x => int.Parse(x.ReceiptNo));
            int empID = User.Identity.GetEmployeeID();
            string employeeName = string.Empty;
            if (empID > 0)
            {
                var employee = _employeeService.GetEmployeeById(empID);
                employeeName = employee != null ? employee.Name : string.Empty;
            }
            return View(new CreateCashCollectionViewModel
            {
                Type = EnumDropdownTranType.FromCustomer,
                ReceiptNo = recptNo,
                EmpName = employeeName,
                PayItems = payItems,
                PayHeadId = fpay.ExpenseItemID.ToString()
            });
        }

        [HttpPost]
        [Authorize]
        [Route("create/returnUrl")]
        public async Task<ActionResult> Create(CreateCashCollectionViewModel newCashCollection, FormCollection formCollection, string returnUrl, string PayType)
        {
            ViewBag.IsEmployeeWiseTransEnable = _SysService.IsEmployeeWiseTransactionEnable();
            ViewBag.CustomerIds = GetAllCustomerForDDL();
            var payItems = _CashCollectionService.GetAllPayTypeHeadForPO().Select(d => new SelectListItem
            {
                Value = d.ExpenseItemID.ToString(),
                Text = d.Name
            }).ToList();
            newCashCollection.PayItems = payItems;
            AddModelError(newCashCollection, formCollection);
            ModelState.Remove("PaymentType");
            if (!ModelState.IsValid)
            {
                //var errors = ModelState.Values.Select(v => v.Errors).ToList();
                return View(newCashCollection);
            }


            if (newCashCollection != null)
            {
                newCashCollection.CreateDate = DateTime.Now.ToString();
                newCashCollection.CreatedBy = (User.Identity.GetUserId<string>());
                newCashCollection.ConcernID = User.Identity.GetConcernId().ToString();

                //newCashCollection.CustomerID = formCollection["CustomerID"];
                newCashCollection.SupplierID = "0";
                //newCashCollection.PaymentType = EnumPayType.Cash;
                if (newCashCollection.Type == EnumDropdownTranType.PreviousSalesRetrun)
                {
                    newCashCollection.TransactionType = EnumTranType.PreviousSalesRetrun;
                }
                else if (newCashCollection.Type == EnumDropdownTranType.SalesCommission)
                {
                    newCashCollection.TransactionType = EnumTranType.SalesCommission;
                }
                else if (newCashCollection.Type == EnumDropdownTranType.CollectionReturn)
                {
                    newCashCollection.TransactionType = EnumTranType.CollectionReturn;
                }
                else
                {
                    newCashCollection.TransactionType = EnumTranType.FromCustomer;
                }

                newCashCollection.CCBankID = "0";

                var sysInfo = _SysService.GetSystemInformationByConcernId(User.Identity.GetConcernId());


                //if (sysInfo.ApprovalSystemEnable == 1 && User.IsInRole(EnumUserRoles.MobileUser.ToString()))

                //    newCashCollection.TransactionType = EnumTranType.CollectionPending;
                //else
                //    newCashCollection.TransactionType = EnumTranType.FromCustomer;

                //if (sysInfo.ApprovalSystemEnable == 0 && newCashCollection.Type == EnumDropdownTranType.CollectionReturn)
                //    newCashCollection.TransactionType = EnumTranType.CollectionReturn;


                //if (sysInfo.ApprovalSystemEnable == 0 || (sysInfo.ApprovalSystemEnable == 1 && !User.IsInRole(EnumUserRoles.MobileUser.ToString())) && newCashCollection.Type == EnumDropdownTranType.FromCustomer)
                //    newCashCollection.TransactionType = EnumTranType.FromCustomer;
                //if (sysInfo.ApprovalSystemEnable == 0 || (sysInfo.ApprovalSystemEnable == 1 && !User.IsInRole(EnumUserRoles.MobileUser.ToString())) && newCashCollection.Type == EnumDropdownTranType.CollectionReturn)
                //    newCashCollection.TransactionType = EnumTranType.CollectionReturn;

                //newCashCollection.ModifiedBy = "0";
                //newCashCollection.ModifiedDate = DateTime.Now.ToString();

                if (newCashCollection.AccountNo == null)
                    newCashCollection.AccountNo = "No A/C";
                var Customer = _CustomerService.GetCustomerById(Convert.ToInt32(newCashCollection.CustomerID));

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

                if (cashCollection.TransactionType == EnumTranType.CollectionReturn)
                {
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
                }
                else
                {
                    if (cashCollection.PayBankId.HasValue)
                    {
                        Bank bank = _bankService.GetBankById(cashCollection.PayBankId.Value);
                        bank.TotalAmount += cashCollection.Amount;
                        _bankService.UpdateBank(bank);
                    }
                    if (cashCollection.PayCashAccountId.HasValue)
                    {
                        CashAccount ca = _cashAccountService.GetById(cashCollection.PayCashAccountId.Value);
                        ca.TotalBalance += cashCollection.Amount;
                        _cashAccountService.Update(ca);
                    }
                }

                if (Convert.ToInt32(newCashCollection.SOrderID) > 0)
                {
                    #region CCAmount and Adjustment add on SOrder
                    var oSales = _salesOrderService.GetSalesOrderById(Convert.ToInt32(newCashCollection.SOrderID));

                    oSales.CCAmount = oSales.CCAmount + cashCollection.Amount;
                    oSales.CCAdjustment = oSales.CCAdjustment + (cashCollection.CashBAmt + cashCollection.YearlyBnsAmt + cashCollection.AdjustAmt);

                    #endregion
                }






                #region Total Due Update
                var oCustomer = _CustomerService.GetCustomerById(Convert.ToInt32(newCashCollection.CustomerID));
                if (sysInfo.ApprovalSystemEnable == 0 || (sysInfo.ApprovalSystemEnable == 1 && !User.IsInRole(EnumUserRoles.MobileUser.ToString())))
                {
                    if (newCashCollection.TransactionType == EnumTranType.FromCustomer)
                        oCustomer.TotalDue = oCustomer.TotalDue - (cashCollection.Amount + cashCollection.CashBAmt + cashCollection.YearlyBnsAmt + cashCollection.AdjustAmt + cashCollection.OfferAmt + cashCollection.BonusAmt);
                    else if (newCashCollection.TransactionType == EnumTranType.PreviousSalesRetrun)
                        oCustomer.TotalDue = oCustomer.TotalDue - (cashCollection.Amount + cashCollection.CashBAmt + cashCollection.YearlyBnsAmt + cashCollection.AdjustAmt + cashCollection.OfferAmt + cashCollection.BonusAmt);
                    else if (newCashCollection.TransactionType == EnumTranType.SalesCommission)
                        oCustomer.TotalDue = oCustomer.TotalDue - (cashCollection.Amount + cashCollection.CashBAmt + cashCollection.YearlyBnsAmt + cashCollection.AdjustAmt + cashCollection.OfferAmt + cashCollection.BonusAmt);
                    else if (newCashCollection.TransactionType == EnumTranType.CollectionReturn)
                        oCustomer.TotalDue = oCustomer.TotalDue + (cashCollection.Amount + cashCollection.CashBAmt + cashCollection.YearlyBnsAmt + cashCollection.AdjustAmt + cashCollection.OfferAmt + cashCollection.BonusAmt);
                    newCashCollection.BalanceDue = oCustomer.TotalDue.ToString();
                }
                #endregion

                #region update employee wise due
                bool isEmpDueAdd = false;
                EmployeeWiseCustomerDue empDue = null;
                var oldEmpDue = _employeeWiseCustomerDueService.GetByEmpCustomer(newCashCollection.EmployeeID, Convert.ToInt32(newCashCollection.CustomerID));
                if (sysInfo.ApprovalSystemEnable == 0 || (sysInfo.ApprovalSystemEnable == 1 && !User.IsInRole(EnumUserRoles.MobileUser.ToString())))
                {
                    if (oldEmpDue != null)
                    {
                        if (newCashCollection.TransactionType == EnumTranType.FromCustomer)
                            oldEmpDue.CustomerDue = oldEmpDue.CustomerDue - (cashCollection.Amount + cashCollection.AdjustAmt + cashCollection.OfferAmt + cashCollection.BonusAmt + cashCollection.YearlyBnsAmt + cashCollection.CashBAmt);
                        else if (newCashCollection.TransactionType == EnumTranType.PreviousSalesRetrun)
                            oldEmpDue.CustomerDue = oldEmpDue.CustomerDue - (cashCollection.Amount + cashCollection.AdjustAmt + cashCollection.OfferAmt + cashCollection.BonusAmt + cashCollection.YearlyBnsAmt + cashCollection.CashBAmt);
                        else if (newCashCollection.TransactionType == EnumTranType.SalesCommission)
                            oldEmpDue.CustomerDue = oldEmpDue.CustomerDue - (cashCollection.Amount + cashCollection.AdjustAmt + cashCollection.OfferAmt + cashCollection.BonusAmt + cashCollection.YearlyBnsAmt + cashCollection.CashBAmt);
                        else if (newCashCollection.TransactionType == EnumTranType.CollectionReturn)
                            oldEmpDue.CustomerDue = oldEmpDue.CustomerDue + (cashCollection.Amount + cashCollection.AdjustAmt + cashCollection.OfferAmt + cashCollection.BonusAmt + cashCollection.YearlyBnsAmt + cashCollection.CashBAmt);
                        newCashCollection.EmpDueBalance = oldEmpDue.CustomerDue.ToString();
                    }
                    else
                    {
                        isEmpDueAdd = true;
                        empDue = new EmployeeWiseCustomerDue
                        {
                            EmployeeID = newCashCollection.EmployeeID.Value,
                            CustomerID = Convert.ToInt32(newCashCollection.CustomerID),
                            CustomerDue = Convert.ToDecimal(newCashCollection.EmpDueBalance),
                            CreatedBy = Convert.ToInt32(newCashCollection.CreatedBy),
                            CreateDate = Convert.ToDateTime(newCashCollection.CreateDate),
                            ConcernID = Convert.ToInt32(newCashCollection.ConcernID)
                        };
                    }
                }

                #endregion

                #region Remind Date Update
                if (Convert.ToDateTime(newCashCollection.RemindDate) > Convert.ToDateTime(newCashCollection.EntryDate))
                {
                    if (oCustomer != null)
                    {
                        oCustomer.RemindDate = Convert.ToDateTime(newCashCollection.RemindDate);
                    }
                }
                #endregion
                bool Status = false;
                try
                {
                    _CashCollectionService.AddCashCollection(cashCollection);
                    _CashCollectionService.SaveCashCollection();

                    Status = true;
                }
                catch (Exception)
                {
                    Status = false;
                }

                if (Status)
                {
                    _CustomerService.UpdateCustomer(oCustomer);
                    _CustomerService.SaveCustomer();
                    if (newCashCollection.EmployeeID > 0)
                    {
                        if (isEmpDueAdd)
                            _employeeWiseCustomerDueService.Add(empDue);
                        else
                            _employeeWiseCustomerDueService.Update(oldEmpDue);

                        _employeeWiseCustomerDueService.Save();
                    }
                }


                //_CashCollectionService.UpdateTotalDue(Convert.ToInt32(newCashCollection.CustomerID), 0, 0, 0, Convert.ToDecimal(Convert.ToDecimal(newCashCollection.Amount) + Convert.ToDecimal(newCashCollection.AdjustAmt)));

                TempData["MoneyReceiptData"] = cashCollection;
                TempData["IsMoneyReceiptReady"] = true;

                AddToastMessage("", "Item has been saved successfully.", ToastType.Success);


                #region SMS Service
                var SystemInfo = _SysService.GetSystemInformationByConcernId(User.Identity.GetConcernId());

                if (SystemInfo.IsCashcollSMSEnable == 1 && newCashCollection.IsSmsEnable == true)
                {
                    if (SystemInfo.IsBanglaSmsEnable == 1)
                    {
                        List<SMSRequest> sms = new List<SMSRequest>();
                        sms.Add(new SMSRequest()
                        {
                            MobileNo = oCustomer.ContactNo,
                            CustomerID = oCustomer.CustomerID,
                            CustomerCode = oCustomer.Code,
                            TransNumber = cashCollection.ReceiptNo,
                            Date = (DateTime)cashCollection.EntryDate,
                            PreviousDue = oCustomer.TotalDue + (decimal)cashCollection.Amount + cashCollection.AdjustAmt,
                            ReceiveAmount = (decimal)cashCollection.Amount,
                            PresentDue = oCustomer.TotalDue,
                            SMSType = EnumSMSType.CashCollection
                        });

                        if (SystemInfo.SMSSendToOwner == 1)
                        {
                            sms.Add(new SMSRequest()
                            {
                                MobileNo = SystemInfo.InsuranceContactNo,
                                CustomerID = oCustomer.CustomerID,
                                CustomerCode = oCustomer.Code,
                                TransNumber = cashCollection.ReceiptNo,
                                Date = (DateTime)cashCollection.EntryDate,
                                PreviousDue = oCustomer.TotalDue + (decimal)cashCollection.Amount + cashCollection.AdjustAmt,
                                ReceiveAmount = (decimal)cashCollection.Amount,
                                PresentDue = oCustomer.TotalDue,
                                SMSType = EnumSMSType.CashCollection
                            });
                        }

                        int concernId = User.Identity.GetConcernId();
                        decimal previousBalance;
                        SMSPaymentMaster smsAmountDetails = _smsBillPaymentBkashService.GetByConcernId(concernId);
                        previousBalance = smsAmountDetails.TotalRecAmt;
                        var sysInfos = _SysService.GetSystemInformationByConcernId(User.Identity.GetConcernId());
                        decimal smsFee = sysInfos.smsCharge;
                        if (smsAmountDetails.TotalRecAmt > 1)
                        {
                            var response = await Task.Run(() => SMSHTTPServiceBangla.SendSMS(EnumOnnoRokomSMSType.NumberSms, sms, previousBalance, SystemInfo, User.Identity.GetUserId<int>()));
                            if (response.Count > 0)
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
                        List<SMSRequest> sms = new List<SMSRequest>();
                        sms.Add(new SMSRequest()
                        {
                            MobileNo = oCustomer.ContactNo,
                            CustomerID = oCustomer.CustomerID,
                            CustomerCode = oCustomer.Code,
                            CustomerName = oCustomer.Name,
                            TransNumber = cashCollection.ReceiptNo,
                            Date = (DateTime)cashCollection.EntryDate,
                            PreviousDue = oCustomer.TotalDue + (decimal)cashCollection.Amount + cashCollection.AdjustAmt,
                            ReceiveAmount = (decimal)cashCollection.Amount,
                            PresentDue = oCustomer.TotalDue,
                            SMSType = EnumSMSType.CashCollection
                        });

                        if (SystemInfo.SMSSendToOwner == 1)
                        {
                            sms.Add(new SMSRequest()
                            {
                                MobileNo = SystemInfo.InsuranceContactNo,
                                CustomerID = oCustomer.CustomerID,
                                CustomerCode = oCustomer.Code,
                                CustomerName = oCustomer.Name,
                                TransNumber = cashCollection.ReceiptNo,
                                Date = (DateTime)cashCollection.EntryDate,
                                PreviousDue = oCustomer.TotalDue + (decimal)cashCollection.Amount + cashCollection.AdjustAmt,
                                ReceiveAmount = (decimal)cashCollection.Amount,
                                PresentDue = oCustomer.TotalDue,
                                SMSType = EnumSMSType.CashCollection
                            });
                        }

                        int concernId = User.Identity.GetConcernId();
                        int paymentMasterId;
                        decimal previousBalance;
                        SMSPaymentMaster smsAmountDetails = _smsBillPaymentBkashService.GetByConcernId(concernId);
                        paymentMasterId = smsAmountDetails.SMSPaymentMasterID;
                        previousBalance = smsAmountDetails.TotalRecAmt;
                        var sysInfos = _SysService.GetSystemInformationByConcernId(User.Identity.GetConcernId());
                        decimal smsFee = sysInfos.smsCharge;
                        if (smsAmountDetails.TotalRecAmt > 1)
                        {
                            var response = await Task.Run(() => SMSHTTPService.SendSMS(EnumOnnoRokomSMSType.NumberSms, sms, previousBalance, SystemInfo, User.Identity.GetUserId<int>()));
                            if (response.Count > 0)
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

                return RedirectToAction("Index");
            }
            else
            {
                AddToastMessage("", "No Item data found to create.", ToastType.Error);
                return RedirectToAction("Create");
            }
        }

        private void AddModelError(CreateCashCollectionViewModel newCashCollection, FormCollection formCollection)
        {
            bool IsEmployeeWiseTransactionEnable = _SysService.IsEmployeeWiseTransactionEnable();

            //if (string.IsNullOrEmpty(formCollection["CustomerID"]))
            //    ModelState.AddModelError("CustomerID", "Customer is Required.");
            //else
            //    newCashCollection.CustomerID = formCollection["CustomerID"];

            if (string.IsNullOrEmpty(newCashCollection.PayHeadId))
            {
                ModelState.AddModelError("PayHeadId", "Payment type is required!");
            }

            //if (newCashCollection.PaymentType == EnumPayType.Banking)
            //{
            //    if (string.IsNullOrEmpty(formCollection["CCBanksId"]))
            //        ModelState.AddModelError("CCBankID", "Bank is Required.");
            //    else
            //        newCashCollection.CCBankID = formCollection["CCBanksId"];
            //}
            //else
            //    newCashCollection.CCBankID = "0";


            if (decimal.Parse(GetDefaultIfNull(newCashCollection.Amount)) < 0m)
                ModelState.AddModelError("Amount", "Amount can't be negative");

            if (decimal.Parse(GetDefaultIfNull(newCashCollection.AdjustAmt)) < 0m)
                ModelState.AddModelError("AdjustAmt", "Adjustment can't be negative");

            //if (decimal.Parse(GetDefaultIfNull(newCashCollection.OfferAmt)) < 0m)
            //    ModelState.AddModelError("OfferAmt", "Offer Amount can't be negative");

            //if (decimal.Parse(GetDefaultIfNull(newCashCollection.BonusAmt)) < 0m)
            //    ModelState.AddModelError("BonusAmt", "Bonus Amount can't be negative");

            int CCID = Convert.ToInt32(newCashCollection.CashCollectionID);
            if (_CashCollectionService.GetAllCashCollection().Any(i => i.ReceiptNo.Equals(newCashCollection.ReceiptNo) && i.CashCollectionID != CCID))
                ModelState.AddModelError("ReceiptNo", "This ReceiptNo is already exists.");

            if (!IsDateValid(Convert.ToDateTime(newCashCollection.EntryDate)))
            {
                ModelState.AddModelError("EntryDate", "Back dated entry is not valid.");
            }

            //if (newCashCollection.Type == 0)
            //    ModelState.AddModelError("Type", "Trans. Type is Required.");

            if (IsEmployeeWiseTransactionEnable && !User.IsInRole(EnumUserRoles.MobileUser.ToString()))
            {
                if (string.IsNullOrWhiteSpace(formCollection["EmployeeID"]))
                {
                    ModelState.AddModelError("EmployeeID", "Employee is required.");
                    AddToastMessage("", "Please select an employee!", ToastType.Error);
                }


                //else
                //    newCashCollection.EmployeeID = Convert.ToInt32(formCollection["EmployeesId"]);
            }
            else if (User.IsInRole(EnumUserRoles.MobileUser.ToString()))
            {
                int SRUID = User.Identity.GetUserId<int>();
                var user = _UserService.GetUserById(SRUID);
                newCashCollection.EmployeeID = user.EmployeeID;
            }
            else
                newCashCollection.EmployeeID = 0;
        }


        [HttpGet]
        [Authorize]
        [Route("edit/{id}")]
        public ActionResult Edit(int id)
        {

            ViewBag.IsEmployeeWiseTransEnable = _SysService.IsEmployeeWiseTransactionEnable();
            ViewBag.CustomerIds = GetAllCustomerForDDL();
            var payItems = _CashCollectionService.GetAllPayTypeHeadForPO().Select(d => new SelectListItem
            {
                Value = d.ExpenseItemID.ToString(),
                Text = d.Name
            }).ToList();
            var cashCollection = _CashCollectionService.GetCashCollectionById(id);
            if (!IsDateValid(Convert.ToDateTime(cashCollection.EntryDate)))
                return RedirectToAction("Index");
            var vmodel = _mapper.Map<CashCollection, CreateCashCollectionViewModel>(cashCollection);
            var Customer = _CustomerService.GetCustomerById((int)cashCollection.CustomerID);
            vmodel.CurrentDue = Customer.TotalDue.ToString();
            vmodel.RemindDate = Customer.RemindDate.ToString();
            if (cashCollection.TransactionType == EnumTranType.PreviousSalesRetrun)
            {
                vmodel.Type = EnumDropdownTranType.PreviousSalesRetrun;
            }
            else if (cashCollection.TransactionType == EnumTranType.SalesCommission)
            {
                vmodel.Type = EnumDropdownTranType.SalesCommission;
            }
            else if (cashCollection.TransactionType == EnumTranType.CollectionReturn)
            {
                vmodel.Type = EnumDropdownTranType.CollectionReturn;
            }
            else
            {
                vmodel.Type = EnumDropdownTranType.FromCustomer;
            }

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

            #region emp wise due part
            var empDue = _employeeWiseCustomerDueService.GetByEmpCustomer(cashCollection.EmployeeID, cashCollection.CustomerID.Value);

            vmodel.EmpName = _employeeService.GetEmpNameById(cashCollection.EmployeeID ?? 0);
            vmodel.CurrentEmpDue = empDue != null ? empDue.CustomerDue.ToString() : "0";
            vmodel.EmpDueBalance = empDue != null ? empDue.CustomerDue.ToString() : "0";
            #endregion
            //vmodel.CustomerItems = customerItems;
            //vmodel.SupplierItems = supplierItems;
            return View("Create", vmodel);
        }

        [HttpPost]
        [Authorize]
        [Route("edit/returnUrl")]
        public ActionResult Edit(CreateCashCollectionViewModel newCashCollection, FormCollection formCollection, string returnUrl)
        {
            ViewBag.IsEmployeeWiseTransEnable = _SysService.IsEmployeeWiseTransactionEnable();

            AddModelError(newCashCollection, formCollection);
            ModelState.Remove("PaymentType");
            if (!ModelState.IsValid)
                return View("Create", newCashCollection);

            if (newCashCollection != null)
            {
                var cashCollection = _CashCollectionService.GetCashCollectionById(int.Parse(newCashCollection.CashCollectionID));

                decimal Amount = 0m, AdjAmt = 0m;
                Amount = cashCollection.Amount;
                AdjAmt = cashCollection.AdjustAmt;

                cashCollection.PaymentType = newCashCollection.PaymentType;
                cashCollection.BankName = newCashCollection.BankName;
                cashCollection.BranchName = newCashCollection.BranchName;
                cashCollection.EntryDate = Convert.ToDateTime(newCashCollection.EntryDate);
                cashCollection.Amount = decimal.Parse(newCashCollection.Amount);
                cashCollection.AccountNo = newCashCollection.AccountNo;
                cashCollection.CashBPercentage = decimal.Parse(newCashCollection.CashBPercentage);
                cashCollection.CashBAmt = decimal.Parse(newCashCollection.CashBAmt);
                cashCollection.YearlyBnsAmt = decimal.Parse(newCashCollection.YearlyBnsAmt);
                cashCollection.YearlyBPercentage = decimal.Parse(newCashCollection.YearlyBnsAmt);
                //cashCollection.AccountNo = "No A/C";
                cashCollection.MBAccountNo = newCashCollection.MBAccountNo;
                cashCollection.BKashNo = newCashCollection.BKashNo;
                cashCollection.Remarks = newCashCollection.Remarks;
                //cashCollection.TransactionType = newCashCollection.TransactionType;
                cashCollection.AdjustAmt = decimal.Parse(newCashCollection.AdjustAmt);
                cashCollection.BalanceDue = decimal.Parse(newCashCollection.BalanceDue);
                cashCollection.CustomerID = int.Parse(newCashCollection.CustomerID);
                if (newCashCollection.Type == EnumDropdownTranType.FromCustomer)
                    newCashCollection.TransactionType = EnumTranType.FromCustomer;
                else if (newCashCollection.Type == EnumDropdownTranType.PreviousSalesRetrun)
                    newCashCollection.TransactionType = EnumTranType.PreviousSalesRetrun;
                else if (newCashCollection.Type == EnumDropdownTranType.SalesCommission)
                    newCashCollection.TransactionType = EnumTranType.SalesCommission;
                else if (newCashCollection.Type == EnumDropdownTranType.CollectionReturn)
                    newCashCollection.TransactionType = EnumTranType.CollectionReturn;
                cashCollection.ModifiedBy = User.Identity.GetUserId<int>();
                cashCollection.ModifiedDate = DateTime.Now;
                //cashCollection.SupplierID = int.Parse(newCashCollection.SupplierID);
                cashCollection.InterestAmt = 0m;
                cashCollection.OfferAmt = 0m;
                cashCollection.BonusAmt = 0m;
                cashCollection.EmployeeID = newCashCollection.EmployeeID;
                cashCollection.CCBankID = 0;


                if (string.IsNullOrEmpty(newCashCollection.AccountNo))
                {
                    cashCollection.AccountNo = "No A/C";
                }
                _CashCollectionService.UpdateTotalDueWhenEdit((int)cashCollection.CustomerID, 0, 0, cashCollection.CashCollectionID, (cashCollection.Amount + cashCollection.AdjustAmt + cashCollection.OfferAmt + cashCollection.BonusAmt - cashCollection.InterestAmt + cashCollection.YearlyBnsAmt + cashCollection.CashBAmt));
                cashCollection.BalanceDue = _CustomerService.GetCustomerById((int)cashCollection.CustomerID).TotalDue;
                _CashCollectionService.UpdateCashCollection(cashCollection);
                _CashCollectionService.SaveCashCollection();

                #region Remind Date Update
                if (Convert.ToDateTime(newCashCollection.RemindDate) > Convert.ToDateTime(newCashCollection.EntryDate))
                {
                    var customer = _CustomerService.GetCustomerById(Convert.ToInt32(cashCollection.CustomerID));
                    if (customer != null)
                    {
                        customer.RemindDate = Convert.ToDateTime(newCashCollection.RemindDate);
                        _CustomerService.UpdateCustomer(customer);
                        _CustomerService.SaveCustomer();
                    }
                }
                #endregion

                if (!User.IsInRole(EnumUserRoles.MobileUser.ToString()))
                {
                    if (newCashCollection.Type == EnumDropdownTranType.FromCustomer)
                        cashCollection.TransactionType = EnumTranType.FromCustomer;
                    else if (newCashCollection.Type == EnumDropdownTranType.PreviousSalesRetrun)
                        cashCollection.TransactionType = EnumTranType.PreviousSalesRetrun;
                    else if (newCashCollection.Type == EnumDropdownTranType.SalesCommission)
                        cashCollection.TransactionType = EnumTranType.SalesCommission;
                    else if (newCashCollection.Type == EnumDropdownTranType.CollectionReturn)
                        cashCollection.TransactionType = EnumTranType.CollectionReturn;
                }
                else
                    cashCollection.TransactionType = EnumTranType.FromCustomer;

                TempData["MoneyReceiptData"] = cashCollection;
                TempData["IsMoneyReceiptReady"] = true;


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
            ViewBag.IsEmployeeWiseTransEnable = _SysService.IsEmployeeWiseTransactionEnable();
            var oldCashCollection = _CashCollectionService.GetCashCollectionById(id);
            decimal Amount = 0m, AdjAmt = 0m, IntAmt = 0m, BonusAmt = 0m, OfferAmt = 0m, YearlyBonusAmt = 0m, CashBonusAmt = 0m;
            Amount = oldCashCollection.Amount;
            AdjAmt = oldCashCollection.AdjustAmt;
            IntAmt = oldCashCollection.InterestAmt;
            BonusAmt = oldCashCollection.BonusAmt;
            OfferAmt = oldCashCollection.OfferAmt;
            YearlyBonusAmt = oldCashCollection.YearlyBnsAmt;
            CashBonusAmt = oldCashCollection.CashBAmt;


            if (!IsDateValid(Convert.ToDateTime(oldCashCollection.EntryDate)))
            {
                return RedirectToAction("Index");
            }

            var oCustomer = _CustomerService.GetCustomerById(Convert.ToInt32(oldCashCollection.CustomerID));
            if (oldCashCollection.TransactionType == EnumTranType.FromCustomer)
                oCustomer.TotalDue = oCustomer.TotalDue + (Amount + AdjAmt + BonusAmt + OfferAmt + YearlyBonusAmt + CashBonusAmt) - IntAmt;
            else if (oldCashCollection.TransactionType == EnumTranType.PreviousSalesRetrun)
                oCustomer.TotalDue = oCustomer.TotalDue + (Amount + AdjAmt + BonusAmt + OfferAmt + YearlyBonusAmt + CashBonusAmt) - IntAmt;
            else if (oldCashCollection.TransactionType == EnumTranType.SalesCommission)
                oCustomer.TotalDue = oCustomer.TotalDue + (Amount + AdjAmt + BonusAmt + OfferAmt + YearlyBonusAmt + CashBonusAmt) - IntAmt;
            else if (oldCashCollection.TransactionType == EnumTranType.CollectionReturn)
                oCustomer.TotalDue = oCustomer.TotalDue - (Amount + AdjAmt + BonusAmt + OfferAmt + YearlyBonusAmt + CashBonusAmt);

            //_CashCollectionService.UpdateTotalDue(Convert.ToInt32(CashCollection.CustomerID), 0, 0, 0, -(Convert.ToDecimal(Convert.ToDecimal(CashCollection.Amount) + Convert.ToDecimal(CashCollection.AdjustAmt))));
            bool Status = false;
            try
            {
                _CashCollectionService.DeleteCashCollection(id);
                _CashCollectionService.SaveCashCollection();
                Status = true;
            }
            catch (Exception)
            {
                Status = false;
            }

            if (Status)
            {
                _CustomerService.UpdateCustomer(oCustomer);
                _CustomerService.SaveCustomer();
            }
            AddToastMessage("", "Item has been deleted successfully.", ToastType.Success);
            return RedirectToAction("Index");
        }

        private List<TOIdNameDDL> GetAllCustomerForDDL()
        {
            int CuserId = System.Web.HttpContext.Current.User.Identity.GetUserId<int>();
            int CEmpID = 0;

            if (User.IsInRole(ConstantData.ROLE_MOBILE_USER))
            {
                var user = _UserService.GetUserById(CuserId);
                CEmpID = user.EmployeeID;

                var Ccustomers = _CustomerService.GetAllCustomerByEmpNew(CEmpID);
                var vmCustomers = _mapper.Map<IEnumerable<Customer>, IEnumerable<CreateCustomerViewModel>>(Ccustomers).Select(s => new TOIdNameDDL
                {
                    Id = int.Parse(s.Id),
                    Name = s.Name + "(" + s.Code + "), " + "Mobile:" + s.ContactNo + ", " + "Add:" + s.Address + ", " + "Prop:" + s.CompanyName
                }).ToList();
                return vmCustomers;
            }
            else
            {
                var customers = _CustomerService.GetAllCustomerNew(User.Identity.GetConcernId()).Select(s => new TOIdNameDDL
                {
                    Id = s.Id,
                    Name = s.Name + "(" + s.Code + "), " + "Mobile:" + s.ContactNo + ", " + "Add:" + s.Address + ", " + "Prop:" + s.CompanyName
                }).ToList();
                return customers;
            }
        }
        [HttpGet]
        [Authorize]
        public ActionResult CashCollectionReport()//CashCollectionReport
        {
            return View("CashCollectionReport");
        }

        [HttpGet]
        [Authorize]
        public ActionResult DailyCashBookLedgerReport()
        {
            return View();
        }

        [HttpGet]
        [Authorize]
        public ActionResult SRWiseCashCollectionReport()//CashCollectionReport
        {
            return View();
        }

        [HttpGet]
        [Authorize]
        public ActionResult MoneyReceipt(int id)
        {
            TempData["CashCollectionID"] = id;
            TempData["IsMoneyReceiptById"] = true;
            return RedirectToAction("Index");
        }

        [HttpGet]
        [Authorize]
        public ActionResult AdminCashcolletionReport()//CashCollectionReport
        {
            @ViewBag.Concerns = new SelectList(_SisterConcern.GetAll(), "ConcernID", "Name");
            return View();
        }

        [HttpGet]
        [Authorize]
        public ActionResult CashInHandReport()
        {

            #region Opening Save for cash in hand report
            //var pb = _PrevBalanceService.DailyBalanceProcess(User.Identity.GetConcernId());
            //if (pb.Count != 0)
            //{
            //    foreach (var item in pb)
            //    {
            //        _PrevBalanceService.AddPrevBalance(item);
            //    }
            //}
            //_PrevBalanceService.Save();
            #endregion
            return View();
        }

        [HttpGet]
        [Authorize]
        public ActionResult TypeWiseCashInHand()
        {
            return View();
        }

        [HttpGet]
        [Authorize]
        public ActionResult ProfitAndLossReport()
        {
            return View();
        }

        [HttpGet]
        [Authorize]
        public ActionResult MonthlyTransactionReport()
        {
            if (User.IsInRole(EnumUserRoles.Admin.ToString()) || User.IsInRole(EnumUserRoles.superadmin.ToString()))
                @ViewBag.Concerns = new SelectList(_SisterConcern.GetAll(), "ConcernID", "Name");
            return View();
        }

        public ActionResult AdminCashInhand()
        {
            @ViewBag.Concerns = new SelectList(_SisterConcern.GetAll(), "ConcernID", "Name");
            return View();
        }

        [HttpGet]
        [Authorize]
        public ActionResult Approved(int orderId)
        {
            var CashCollection = _CashCollectionService.GetCashCollectionById(orderId);
            if (CashCollection.TransactionType != EnumTranType.CollectionPending)
            {
                AddToastMessage("", "Cash collection is not pending.");
                return RedirectToAction("Index");

            }

            decimal NetAmount = (Convert.ToDecimal(Convert.ToDecimal(CashCollection.Amount) + Convert.ToDecimal(CashCollection.AdjustAmt))
                + Convert.ToDecimal(CashCollection.OfferAmt) + Convert.ToDecimal(CashCollection.BonusAmt)) - Convert.ToDecimal(CashCollection.InterestAmt);
            _CashCollectionService.UpdateTotalDue(Convert.ToInt32(CashCollection.CustomerID), 0, 0, 0, NetAmount);

            CashCollection.TransactionType = EnumTranType.FromCustomer;
            _CashCollectionService.UpdateCashCollection(CashCollection);
            _CashCollectionService.SaveCashCollection();
            AddToastMessage("", "Item has been approved successfully.", ToastType.Success);

            return RedirectToAction("Index");
        }


        [HttpGet]
        [Authorize]
        public ActionResult AdjustmentReport()
        {
            return View();
        }
    }
}