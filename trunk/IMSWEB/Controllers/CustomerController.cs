using AutoMapper;
using IMSWEB.Model;
using IMSWEB.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using IMSWEB.Report;
using PagedList;


namespace IMSWEB.Controllers
{
    [Authorize]
    [RoutePrefix("customer")]
    public class CustomerController : CoreController
    {
        ICustomerService _customerService;
        IEmployeeService _employeeService;
        IMiscellaneousService<Customer> _miscellaneousService;
        IMapper _mapper;
        string _photoPath = "~/Content/photos/customers";
        IUserService _UserService;
        ISalesOrderService _SalesOrderService;
        ICreditSalesOrderService _creditSalesService;
        ISisterConcernService _SisterConcern;
        ISystemInformationService _SysInfoService;
        IBasicReport _BasicReportService;
        ITerritoryService _TerritoryService;
        private readonly IEmployeeWiseCustomerDueService _employeeWiseCustomerDueService;

        public CustomerController(IErrorService errorService,
            ICustomerService customerService, IMiscellaneousService<Customer> miscellaneousService, IMapper mapper, IEmployeeService employeeService
            , ISalesOrderService SalesOrderService, ISisterConcernService SisterConcern,
              ICreditSalesOrderService creditSalesService, IUserService UserService,
             ISystemInformationService SysInfoService, IBasicReport transactionalReportService, ITerritoryService TerritoryService, IEmployeeWiseCustomerDueService employeeWiseCustomerDueService
            )
            : base(errorService)
        {
            _customerService = customerService;
            _miscellaneousService = miscellaneousService;
            _mapper = mapper;
            _employeeService = employeeService;
            _UserService = UserService;
            _SalesOrderService = SalesOrderService;
            _creditSalesService = creditSalesService;
            _SisterConcern = SisterConcern;
            _SysInfoService = SysInfoService;
            _BasicReportService = transactionalReportService;
            _TerritoryService = TerritoryService;
            this._employeeWiseCustomerDueService = employeeWiseCustomerDueService;

        }

        [HttpGet]
        [Authorize]
        [Route("index")]
        public ActionResult Index(int? Page)
        {
            int PageSize = 15;
            int Pages = Page.HasValue ? Convert.ToInt32(Page) : 1;
            int EmpID = 0;

            if (User.IsInRole(ConstantData.ROLE_MOBILE_USER))
            {
                //EmpID = ConstantData.GetEmployeeIDByUSerID(userId);
                var user = _UserService.GetUserById(User.Identity.GetUserId<int>());
                EmpID = user.EmployeeID;
            }

            //if (EmpID > 0)
            //{
            //    var customersAsync = _customerService.GetAllCustomerAsyncByEmpID(EmpID);
            //    var vmodel = _mapper.Map<IEnumerable<Customer>, IEnumerable<GetCustomerViewModel>>(await customersAsync);
            //    return View(vmodel);
            //}
            var customersAsync = _customerService.GetAll();
            var vmodel = _mapper.Map<IQueryable<Customer>, List<GetCustomerViewModel>>(customersAsync);
            var pagelist = vmodel.ToPagedList(Pages, PageSize);
            return View(pagelist);


        }
        [HttpPost]
        [Authorize]

        public ActionResult Index(FormCollection formCollection)
        {
            int PageSize = 15;
            int Pages = 1;
            int EmpID = 0;

            if (User.IsInRole(ConstantData.ROLE_MOBILE_USER))
            {
                //EmpID = ConstantData.GetEmployeeIDByUSerID(userId);
                var user = _UserService.GetUserById(User.Identity.GetUserId<int>());
                EmpID = user.EmployeeID;
            }
            IQueryable<Customer> customersAsync = null;
            if (!string.IsNullOrEmpty(formCollection["ContactNo"]))
            {
                string contactNo = formCollection["ContactNo"];
                customersAsync = _customerService.GetAllIQueryable().Where(i => i.ContactNo.Contains(contactNo));
            }
            if (!string.IsNullOrEmpty(formCollection["Name"]))
            {
                string name = formCollection["Name"];
                customersAsync = _customerService.GetAllIQueryable().Where(i => i.Name.Contains(name));
            }
            var vmodel = _mapper.Map<IQueryable<Customer>, List<GetCustomerViewModel>>(customersAsync);
            var pagelist = vmodel.ToPagedList(Pages, PageSize);
            return View(pagelist);


        }

        [HttpGet]
        [Authorize]
        [Route("create")]
        public ActionResult Create()
        {
            ViewBag.IsEmployeeWiseTransEnable = _SysInfoService.IsEmployeeWiseTransactionEnable();
            string code = _customerService.GetUniqueCodeByType(EnumCustomerType.Retail);
            ViewBag.IsSalesOrCollectionExist = false;
            var FirstEmolyee = _employeeService.GetAllEmployeeIQueryable().FirstOrDefault();
            var FirstTerritory = _TerritoryService.GetAllTerritory().FirstOrDefault();
            if (FirstEmolyee != null && FirstTerritory != null)
                return View(new CreateCustomerViewModel { Code = code, CustomerType = EnumCustomerType.Retail, EmployeeId = FirstEmolyee.EmployeeID.ToString(), TerritoryID = FirstTerritory.TerritoryID.ToString() });
            else
                return View(new CreateCustomerViewModel { Code = code, CustomerType = EnumCustomerType.Retail });

        }

        [HttpPost]
        [Authorize]
        [Route("create/returnUrl")]
        public ActionResult Create(CreateCustomerViewModel newCustomer, FormCollection formCollection,
            HttpPostedFileBase photo, string returnUrl)
        {
            ViewBag.IsEmployeeWiseTransEnable = _SysInfoService.IsEmployeeWiseTransactionEnable();
            ViewBag.IsSalesOrCollectionExist = _customerService.IsCustomerSalesOrCollectionExists(Convert.ToInt32(GetDefaultIfNull(newCustomer.Id)));
            CheckAndAddModelError(newCustomer, formCollection);
            if (!ModelState.IsValid)
                return View(newCustomer);

            if (newCustomer != null)
            {
                //var existingCustomerc = _miscellaneousService.GetDuplicateEntry(c => c.ContactNo == newCustomer.ContactNo);
                //if (existingCustomerc != null)
                //{
                //    AddToastMessage("", "A Customer with same contact no already exists in the system. Please try with a different contact no.", ToastType.Error);
                //    return View(newCustomer);
                //}

                if (formCollection.Get("btnEmpWiseDue") != null)
                {
                    newCustomer.CustomerDues = newCustomer.CustomerDues ?? new List<EmployeewiseCustomerDueViewModel>();
                    if (newCustomer.DueEmployeeID == 0)
                    {
                        AddToastMessage("", "Please add employee", ToastType.Error);
                        return View(newCustomer);
                    }
                    if (newCustomer.EmployeeWiseDue == 0)
                    {
                        AddToastMessage("", "Please add employee wise Customer due", ToastType.Error);
                        return View(newCustomer);
                    }
                    if (newCustomer.CustomerDues.Any(i => i.EmployeeID == newCustomer.DueEmployeeID))
                    {
                        AddToastMessage("", "Customer due for this employee is already added.", ToastType.Error);
                        return View(newCustomer);
                    }
                    newCustomer.CustomerDues.Add(new EmployeewiseCustomerDueViewModel
                    {
                        EmployeeID = newCustomer.DueEmployeeID,
                        CustomerDue = newCustomer.EmployeeWiseDue,
                        CustomerOpeningDue = newCustomer.EmployeeWiseDue,
                        EmployeeName = newCustomer.EmployeeName
                    });

                    newCustomer.DueEmployeeID = 0;
                    newCustomer.EmployeeWiseDue = 0;
                    newCustomer.EmployeeName = string.Empty;
                    newCustomer.TerritoryID = formCollection["TerritorysId"];
                    ModelState.Clear();
                    TempData["customerData"] = newCustomer;
                    return View(newCustomer);
                }
                else
                {
                    MapFormCollectionValueWithNewEntity(newCustomer, formCollection);

                    if (newCustomer.Id.IsNotNullOrEmpty())
                    {
                        var existingCustomer = _customerService.GetCustomerById(int.Parse(newCustomer.Id));

                        if (photo != null)
                        {
                            var photoName = newCustomer.Code + "_" + newCustomer.Name;
                            existingCustomer.PhotoPath = SaveHttpPostedImageFile(photoName, Server.MapPath(_photoPath), photo);
                        }

                        existingCustomer.Code = newCustomer.Code.Trim();
                        existingCustomer.Name = newCustomer.Name;
                        existingCustomer.ContactNo = newCustomer.ContactNo;
                        existingCustomer.TotalDue = decimal.Parse(newCustomer.TotalDue);
                        existingCustomer.TerritoryID = int.Parse(newCustomer.TerritoryID);
                        existingCustomer.EmployeeID = int.Parse(newCustomer.EmployeeId);
                        if (!existingCustomer.CustomerType.Equals(newCustomer.CustomerType))
                        {
                            if (ViewBag.IsSalesOrCollectionExist)
                            {
                                AddToastMessage("", "Type can't be changed. Because this customer has transactions.", ToastType.Error);
                            }
                            else
                            {
                                existingCustomer.CustomerType = newCustomer.CustomerType;

                            }
                        }

                        if (!ViewBag.IsSalesOrCollectionExist)
                        {
                            existingCustomer.OpeningDue = decimal.Parse(GetDefaultIfNull(newCustomer.OpeningDue));
                            existingCustomer.TotalDue = decimal.Parse(GetDefaultIfNull(newCustomer.TotalDue));
                        }
                        existingCustomer.CusDueLimit = decimal.Parse(GetDefaultIfNull(newCustomer.CusDueLimit));
                        existingCustomer.FName = newCustomer.FName;
                        existingCustomer.CompanyName = newCustomer.CompanyId;
                        existingCustomer.EmailID = newCustomer.EmailId;
                        existingCustomer.NID = newCustomer.NId;
                        existingCustomer.Address = newCustomer.Address;
                        existingCustomer.RefName = newCustomer.RefName;
                        existingCustomer.RefContact = newCustomer.RefContact;
                        existingCustomer.RefFName = newCustomer.RefFName;
                        existingCustomer.RefAddress = newCustomer.RefAddress;
                        existingCustomer.ConcernID = User.Identity.GetConcernId();
                        existingCustomer.ModifiedBy = User.Identity.GetUserId<int>();
                        existingCustomer.ModifiedDate = GetLocalDateTime();

                        _customerService.UpdateCustomer(existingCustomer);
                        _customerService.SaveCustomer();
                        if (existingCustomer.CustomerID > 0 && newCustomer.CustomerDues != null && newCustomer.CustomerDues.Count() > 0)
                        {
                            AddEmployeeWiseCustomerDue(newCustomer.Id, newCustomer.CustomerDues);
                        }
                    }
                    else
                    {
                        var existingCustomerc = _miscellaneousService.GetDuplicateEntry(c => c.ContactNo == newCustomer.ContactNo);
                        if (existingCustomerc != null)
                        {
                            AddToastMessage("", "A Customer with same contact no already exists in the system. Please try with a different contact no.", ToastType.Error);
                            return View(newCustomer);
                        }

                        if (photo != null)
                        {
                            var photoName = newCustomer.Code + "_" + newCustomer.Name;
                            newCustomer.PhotoPath = SaveHttpPostedImageFile(photoName, Server.MapPath(_photoPath), photo);
                        }

                        newCustomer.Code = newCustomer.Code.Trim();
                        var customer = _mapper.Map<CreateCustomerViewModel, Customer>(newCustomer);
                        customer.ConcernID = User.Identity.GetConcernId();
                        customer.OpeningDue = decimal.Parse(GetDefaultIfNull(newCustomer.OpeningDue));
                        customer.CreatedDate = GetLocalDateTime();
                        customer.CreatedBy = User.Identity.GetUserId<int>();


                        if (!ControllerContext.RouteData.Values["action"].ToString().ToLower().Equals("edit"))
                        {
                            if (_miscellaneousService.GetDuplicateEntry(i => i.Code == customer.Code) != null)
                                customer.Code = _customerService.GetUniqueCodeByType(customer.CustomerType);
                        }

                        _customerService.AddCustomer(customer);
                        _customerService.SaveCustomer();
                        if (customer.CustomerID > 0 && newCustomer.CustomerDues != null && newCustomer.CustomerDues.Count() > 0)
                        {
                            AddEmployeeWiseCustomerDue(customer.CustomerID.ToString(), newCustomer.CustomerDues);
                        }
                    }

                    AddToastMessage("", "Customer has been saved successfully.", ToastType.Success);
                    return RedirectToAction("Create");
                }
            }
            else
            {
                AddToastMessage("", "No Customer data found to save.", ToastType.Error);
                return RedirectToAction("Create");
            }
        }
        private void AddEmployeeWiseCustomerDue(string customerID, List<EmployeewiseCustomerDueViewModel> CustomerDues)
        {
            //delete
            var oldData = _employeeWiseCustomerDueService.Get(customerID: int.Parse(customerID));
            foreach (var item in oldData)
            {
                if (!CustomerDues.Any(i => i.EmployeeID == item.EmployeeID && i.CustomerID == item.CustomerID))
                {
                    _employeeWiseCustomerDueService.DeleteByID(item.ID);
                }
            }

            foreach (var item in CustomerDues)
            {
                //Create
                if (!oldData.Any(i => i.EmployeeID == item.EmployeeID && i.CustomerID == item.CustomerID))
                {
                    var empCustomerDue = new EmployeeWiseCustomerDue
                    {
                        EmployeeID = item.EmployeeID,
                        CustomerID = Convert.ToInt32(customerID),
                        CustomerDue = item.CustomerDue,
                        CustomerOpeningDue = item.CustomerOpeningDue
                    };
                    AddAuditTrail(empCustomerDue, true);
                    _employeeWiseCustomerDueService.Add(empCustomerDue);
                }
                else //update
                {
                    var updateItem = _employeeWiseCustomerDueService.Get(id: item.ID).FirstOrDefault();
                    updateItem.CustomerDue = item.CustomerDue;
                    updateItem.CustomerOpeningDue = item.CustomerOpeningDue;
                    AddAuditTrail(updateItem, false);
                    _employeeWiseCustomerDueService.Update(updateItem);
                }
            }

            _employeeWiseCustomerDueService.Save();
            AddToastMessage("", "Employee wise customer due updated successfully.", ToastType.Success);
        }

        [HttpGet]
        [Authorize]
        [Route("edit/{id}")]
        public ActionResult Edit(int id)
        {
            ViewBag.IsEmployeeWiseTransEnable = _SysInfoService.IsEmployeeWiseTransactionEnable();
            //var customer = _customerService.GetCustomerById(id);
            //var vmodel = _mapper.Map<Customer, CreateCustomerViewModel>(customer);
            //return View("Create", vmodel);
            var customer = _customerService.GetCustomerById(id);
            var vmodel = _mapper.Map<Customer, CreateCustomerViewModel>(customer);

            var empWiseDue = _employeeWiseCustomerDueService.GetEmployeeCustomerData(customerID: customer.CustomerID);
            var vmEmpWiseDue = _mapper.Map<List<Tuple<int, int, string, string, string,
                int, string, Tuple<string, string, decimal, decimal>>>, List<EmployeewiseCustomerDueViewModel>>(empWiseDue);
            if (empWiseDue.Count() > 0)
                vmodel.CustomerDues = vmEmpWiseDue;

            ViewBag.IsSalesOrCollectionExist = _customerService.IsCustomerSalesOrCollectionExists(customer.CustomerID);
            TempData["customerData"] = vmodel;
            return View("Create", vmodel);
        }

        //[HttpPost]
        //[Authorize]
        //[Route("edit/returnUrl")]
        //public ActionResult Edit(CreateCustomerViewModel newCustomer, FormCollection formCollection,
        //    HttpPostedFileBase photo, string returnUrl)
        //{
        //    CheckAndAddModelError(newCustomer, formCollection);
        //    if (!ModelState.IsValid)
        //        return View("Create", newCustomer);

        //    if (newCustomer != null)
        //    {
        //        var existingCustomer = _customerService.GetCustomerById(int.Parse(newCustomer.Id));
        //        MapFormCollectionValueWithExistingEntity(existingCustomer, formCollection);

        //        if (photo != null)
        //        {
        //            var photoName = newCustomer.Code + "_" + newCustomer.Name;
        //            existingCustomer.PhotoPath = SaveHttpPostedImageFile(photoName, Server.MapPath(_photoPath), photo);
        //        }

        //        existingCustomer.Code = newCustomer.Code.Trim();
        //        existingCustomer.Remarks = newCustomer.Remarks;
        //        existingCustomer.Name = newCustomer.Name;
        //        existingCustomer.ContactNo = newCustomer.ContactNo;
        //        existingCustomer.TotalDue = decimal.Parse(newCustomer.TotalDue);
        //        if (!existingCustomer.CustomerType.Equals(newCustomer.CustomerType))
        //        {
        //            if ((_SalesOrderService.GetAllIQueryable().Any(i => i.Status != (int)EnumSalesType.Return && i.CustomerID == existingCustomer.CustomerID))
        //                || (_creditSalesService.GetAllIQueryable().Any(i => i.Status != (int)EnumSalesType.Return && i.CustomerID == existingCustomer.CustomerID)))
        //            {
        //                AddToastMessage("", "Type can't be changed.Because this customer has transactions.", ToastType.Error);
        //            }
        //            else
        //            {
        //                existingCustomer.CustomerType = newCustomer.CustomerType;
        //            }
        //        }
        //        existingCustomer.CusDueLimit = decimal.Parse(newCustomer.CusDueLimit);
        //        existingCustomer.FName = newCustomer.FName;
        //        existingCustomer.CompanyName = newCustomer.CompanyId;
        //        existingCustomer.EmailID = newCustomer.EmailId;
        //        existingCustomer.NID = newCustomer.NId;
        //        existingCustomer.Address = newCustomer.Address;
        //        existingCustomer.RefName = newCustomer.RefName;
        //        existingCustomer.RefContact = newCustomer.RefContact;
        //        existingCustomer.RefFName = newCustomer.RefFName;
        //        existingCustomer.RefAddress = newCustomer.RefAddress;
        //        existingCustomer.ConcernID = User.Identity.GetConcernId();
        //        existingCustomer.ModifiedBy = User.Identity.GetUserId<int>();
        //        existingCustomer.ModifiedDate = GetLocalDateTime();
        //        existingCustomer.TerritoryID = int.Parse(newCustomer.TerritoryID);

        //        _customerService.UpdateCustomer(existingCustomer);
        //        _customerService.SaveCustomer();

        //        AddToastMessage("", "Customer has been updated successfully.", ToastType.Success);
        //        return RedirectToAction("Index");
        //    }
        //    else
        //    {
        //        AddToastMessage("", "No Customer data found to update.", ToastType.Error);
        //        return RedirectToAction("Index");
        //    }
        //}

        [HttpGet]
        [Authorize]
        [Route("delete/{id}")]
        public ActionResult Delete(int id)
        {
            if (_customerService.IsCollectionFound(id))
            {
                AddToastMessage("", "Cash collection found for selected customer, can't delete!", ToastType.Error);
                return RedirectToAction("Index");
            }


            AddEmployeeWiseCustomerDue(id.ToString(), new List<EmployeewiseCustomerDueViewModel>());
            _customerService.DeleteCustomer(id);
            _customerService.SaveCustomer();

            AddToastMessage("", "Customer has been deleted successfully.", ToastType.Success);
            return RedirectToAction("Index");
        }

        private void CheckAndAddModelError(CreateCustomerViewModel newCustomer, FormCollection formCollection)
        {
            if (User.IsInRole(EnumUserRoles.MobileUser.ToString()))
            {
                var user = _UserService.GetUserById(User.Identity.GetUserId<int>());
                if (user.EmployeeID > 0)
                    newCustomer.EmployeeId = user.EmployeeID.ToString();
                //else
                //    ModelState.AddModelError("EmployeeId", "Employee is required");
            }
            else
            {
                if (string.IsNullOrEmpty(formCollection["EmployeesId"]))
                    ModelState.AddModelError("EmployeeId", "Employee is required");
                else
                    newCustomer.EmployeeId = formCollection["EmployeesId"];
            }

            //if (!newCustomer.Code.Substring(0, 1).Equals(newCustomer.CustomerType.ToString().Substring(0, 1)))
            //    ModelState.AddModelError("Code", "Customer code and type don't match.");
            var sysInfo = _SysInfoService.GetSystemInformationByConcernId(User.Identity.GetConcernId());
            if (sysInfo != null)
            {
                if (sysInfo.CustomerDueLimitApply == 1)
                {
                    //if (Convert.ToDecimal(newCustomer.CusDueLimit) <= 0m)
                    //    ModelState.AddModelError("CusDueLimit", "Customer Due Limit is Required.");
                }
            }
            if (string.IsNullOrEmpty(formCollection["TerritorysId"]))
                ModelState.AddModelError("TerritoryID", "Territory is required");
            else
                newCustomer.TerritoryID = formCollection["TerritorysId"];
        }

        private void MapFormCollectionValueWithNewEntity(CreateCustomerViewModel newCustomer,
            FormCollection formCollection)
        {
            if (User.IsInRole(EnumUserRoles.MobileUser.ToString()))
            {
                var EmployeeID = User.Identity.GetEmployeeID();
                if (EmployeeID > 0)
                    newCustomer.EmployeeId = EmployeeID.ToString();
            }
            else
            {
                newCustomer.EmployeeId = formCollection["EmployeesId"];
            }
            newCustomer.TerritoryID = formCollection["TerritorysId"];
        }

        private void MapFormCollectionValueWithExistingEntity(Customer customer,
            FormCollection formCollection)
        {
            if (!User.IsInRole(EnumUserRoles.MobileUser.ToString()))
                customer.EmployeeID = int.Parse(formCollection["EmployeesId"]);
            else
            {
                var emp = User.Identity.GetEmployeeID();
                if (emp > 0)
                    customer.EmployeeID = emp;
            }
            customer.TerritoryID = int.Parse(formCollection["TerritorysId"]);
        }

        [HttpGet]
        [Authorize]
        public ActionResult ConcernWiseCustomerDueRpt()
        {
            return View("ConcernWiseCustomerDueRpt");
        }

        [HttpGet]
        public JsonResult GetEmployeeByCode(string Code)
        {
            var employee = _employeeService.GetAllEmployee().FirstOrDefault(i => i.Code.Equals(Code.PadLeft(5, '0')));
            if (employee == null)
                return Json(false, JsonRequestBehavior.AllowGet);
            return Json(employee, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult GetEmployeeByID(int ID)
        {
            var employee = _employeeService.GetEmployeeById(ID);
            if (employee == null)
                return Json(false, JsonRequestBehavior.AllowGet);
            return Json(employee, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetTerritoryByCode(string Code)
        {
            var Territory = _TerritoryService.GetAllTerritory().FirstOrDefault(i => i.Code.Equals(Code.PadLeft(5, '0')));
            if (Territory == null)
                return Json(false, JsonRequestBehavior.AllowGet);
            return Json(Territory, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult GetTerritoryByID(int ID)
        {
            var Territory = _TerritoryService.GetTerritoryById(ID);
            if (Territory == null)
                return Json(false, JsonRequestBehavior.AllowGet);
            return Json(Territory, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Authorize]
        public ActionResult CustomerLedger()
        {
            return View();
        }

        [HttpGet]
        [Authorize]
        public ActionResult CustomerDueReport()
        {
            return View();
        }
        [HttpGet]
        [Authorize]
        public ActionResult AdminCustomerDueReport()
        {
            @ViewBag.Concerns = new SelectList(_SisterConcern.GetAll(), "ConcernID", "Name");
            return View();
        }
        [HttpGet]
        public JsonResult GetUniqueCodeByType(int CustomerType)
        {
            string code = string.Empty;
            if (CustomerType != default(int))
            {
                code = _customerService.GetUniqueCodeByType((EnumCustomerType)CustomerType);
            }
            return Json(code, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult CreateCustomer(GetCustomerViewModel newCustomer)
        {
            try
            {
                string code = _customerService.GetUniqueCodeByType(newCustomer.CustomerType);
                if (_customerService.GetAllIQueryable().Any(i => i.ContactNo.Equals(newCustomer.ContactNo)))
                {
                    return Json(new { Result = false, ErrorMsg = "Customer with same contact number already exists in the system." });
                }
                else
                {
                    Customer oNewCustomer = new Customer();
                    oNewCustomer.Code = code;
                    oNewCustomer.Name = newCustomer.Name;
                    //if (newCustomer.CustomerType != EnumCustomerType.Credit)
                    //{
                    //    oNewCustomer.TotalDue = Convert.ToDecimal(newCustomer.TotalDue);
                    //    oNewCustomer.OpeningDue = Convert.ToDecimal(newCustomer.TotalDue);
                    //}
                    //else
                    //{
                    oNewCustomer.TotalDue = 0m;
                    oNewCustomer.OpeningDue = 0m;
                    //}

                    oNewCustomer.Address = newCustomer.Address;
                    oNewCustomer.ContactNo = newCustomer.ContactNo;
                    int ConcernID = User.Identity.GetConcernId();
                    oNewCustomer.EmployeeID = _employeeService.GetAllEmployeeIQueryable().FirstOrDefault(i => i.ConcernID == ConcernID).EmployeeID;
                    oNewCustomer.CustomerType = newCustomer.CustomerType;
                    AddAuditTrail(oNewCustomer, true);
                    _customerService.AddCustomer(oNewCustomer);
                    _customerService.SaveCustomer();
                    GetCustomerViewModel customer = new GetCustomerViewModel();
                    customer.Id = oNewCustomer.CustomerID.ToString();
                    customer.Name = oNewCustomer.Name;
                    customer.Code = oNewCustomer.Code;
                    customer.TotalDue = oNewCustomer.TotalDue.ToString();
                    oNewCustomer.TerritoryID = _TerritoryService.GetAllTerritoryIQueryable().FirstOrDefault(i => i.ConcernID == ConcernID).TerritoryID;
                    return Json(new { Result = true, ErrorMsg = "Customer saved successfully.", Customer = customer });
                }
            }
            catch (Exception ex)
            {
                return Json(new { Result = false, ErrorMsg = ex.Message });
            }
        }

        [HttpPost]
        [Authorize]
        public PartialViewResult DetailsReport(FormCollection formCollection)
        {
            int CustomerID = 0;

            if (!string.IsNullOrEmpty(formCollection["txtCustomerName"]))
            {
                CustomerID = Convert.ToInt32(formCollection["txtCustomerName"]);
            }
            byte[] bytes = _BasicReportService.GetCustomerDetails(User.Identity.Name, User.Identity.GetConcernId(), CustomerID);
            TempData["ReportData"] = bytes;
            return PartialView("~/Views/Shared/_ReportViewer.cshtml");
        }

        public ActionResult DeleteEmployeeDue(int employeeID)
        {
            var model = TempData["customerData"] as CreateCustomerViewModel;

            if (model != null && model.CustomerDues != null && model.CustomerDues.Count() > 0)
            {
                model.CustomerDues = model.CustomerDues.Where(i => i.EmployeeID != employeeID).ToList();
            }
            if (model.Id.IsNotNullOrEmpty())
                ViewBag.IsSalesOrCollectionExist = _customerService.IsCustomerSalesOrCollectionExists(Convert.ToInt32(GetDefaultIfNull(model.Id)));
            else
                ViewBag.IsSalesOrCollectionExist = false;

            AddToastMessage("", "Customer has been deleted successfully.", ToastType.Success);
            return View("Create", model);
        }

    }
}