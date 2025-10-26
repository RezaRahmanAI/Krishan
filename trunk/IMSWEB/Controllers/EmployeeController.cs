using AutoMapper;
using IMSWEB.Model;
using IMSWEB.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using IMSWEB.ViewModels.Common;

namespace IMSWEB.Controllers
{
    [Authorize]
    [RoutePrefix("employee")]
    public class EmployeeController : CoreController
    {
        IEmployeeService _employeeService;
        IDesignationService _designationService;
        IMiscellaneousService<Employee> _miscellaneousService;
        IMapper _mapper;
        string _photoPath = "~/Content/photos/employees";
        IReligionService _religionService;
        IDepartmentService _departmnetService;
        ISystemInformationService _SysInfoService;
        private readonly IEmployeeWiseCustomerDueService _employeeWiseCustomerDueService;
        public EmployeeController(IErrorService errorService,
            IEmployeeService employeeService, IDesignationService designationService, IReligionService religionService,
            IDepartmentService DepartmentService,
            IMiscellaneousService<Employee> miscellaneousService, IMapper mapper, ISystemInformationService SysInfoService, IEmployeeWiseCustomerDueService employeeWiseCustomerDueService)
            : base(errorService)
        {
            _employeeService = employeeService;
            _designationService = designationService;
            _miscellaneousService = miscellaneousService;
            _mapper = mapper;
            _religionService = religionService;
            _departmnetService = DepartmentService;
            _SysInfoService = SysInfoService;
            _employeeWiseCustomerDueService = employeeWiseCustomerDueService;
        }

        [HttpGet]
        [Authorize]
        [Route("index")]
        public async Task<ActionResult> Index()
        {
            var EmployeesAsync = _employeeService.GetAllEmployeeAsync();
            var vmodel = _mapper.Map<IEnumerable<Tuple<int, string, string,
            string, string, DateTime, string,Tuple<int, EnumActiveInactive>>>, IEnumerable<GetEmployeeViewModel>>(await EmployeesAsync);
            return View(vmodel);
        }

        [HttpGet]
        [Authorize]
        [Route("create")]
        public ActionResult Create()
        {
            string code = _miscellaneousService.GetUniqueKey(x => int.Parse(x.Code));
            var designations = _designationService.GetAllDesignation().Select(designation
               => new SelectListItem { Text = designation.Description, Value = designation.DesignationID.ToString() }).ToList();

            var religions = _religionService.GetAll().Select(x => new SelectListItem { Text = x.Name, Value = x.ReligionID.ToString() }).ToList();
            var departments = _departmnetService.GetAllDepartment().Select(x => new SelectListItem { Text = x.DESCRIPTION, Value = x.DepartmentId.ToString() }).ToList();
            return View(new CreateEmployeeViewModel { Code = code, Designations = designations, Religions = religions, Departments = departments });
        }

        [HttpPost]
        [Authorize]
        [Route("create/returnUrl")]
        public ActionResult Create(CreateEmployeeViewModel newEmployee, FormCollection formCollection,
            HttpPostedFileBase photo, string returnUrl)
        {
            CheckAndAddModelError(newEmployee, formCollection);
            if (!ModelState.IsValid)
            {
                var designations = _designationService.GetAllDesignation().Select(designation
                     => new SelectListItem { Text = designation.Description, Value = designation.DesignationID.ToString() }).ToList();
                var religions = _religionService.GetAll().Select(x => new SelectListItem { Text = x.Name, Value = x.ReligionID.ToString() }).ToList();
                var departments = _departmnetService.GetAllDepartment().Select(x => new SelectListItem { Text = x.DESCRIPTION, Value = x.DepartmentId.ToString() }).ToList();

                newEmployee.Designations = designations;
                newEmployee.Religions = religions;
                newEmployee.Departments = departments;
                return View(newEmployee);
            }

            if (newEmployee != null)
            {
                MapFormCollectionValueWithNewEntity(newEmployee, formCollection);

                if (photo != null)
                {
                    var photoName = newEmployee.Code + "_" + newEmployee.Name;
                    newEmployee.PhotoPath = SaveHttpPostedImageFile(photoName, Server.MapPath(_photoPath), photo);
                }
                var employee = _mapper.Map<CreateEmployeeViewModel, Employee>(newEmployee);
                employee.ConcernID = User.Identity.GetConcernId();
                employee.GrossSalary = decimal.Parse(GetDefaultIfNull(newEmployee.GrossSalary));
                _employeeService.AddEmployee(employee);
                _employeeService.SaveEmployee();

                AddToastMessage("", "Employee has been saved successfully.", ToastType.Success);
                return RedirectToAction("Create");
            }
            else
            {
                AddToastMessage("", "No Employee data found to save.", ToastType.Error);
                return RedirectToAction("Create");
            }
        }

        [HttpGet]
        [Authorize]
        [Route("edit/{id}")]
        public ActionResult Edit(int id)
        {
            var designations = _designationService.GetAllDesignation().Select(designation
                  => new SelectListItem { Text = designation.Description, Value = designation.DesignationID.ToString() }).ToList();
            var employee = _employeeService.GetEmployeeById(id);
            var religions = _religionService.GetAll().Select(x => new SelectListItem { Text = x.Name, Value = x.ReligionID.ToString() }).ToList();
            var departments = _departmnetService.GetAllDepartment().Select(x => new SelectListItem { Text = x.DESCRIPTION, Value = x.DepartmentId.ToString() }).ToList();

            var vmodel = _mapper.Map<Employee, CreateEmployeeViewModel>(employee);

            vmodel.Designations = designations;
            vmodel.Religions = religions;
            vmodel.Departments = departments;

            return View("Create", vmodel);
        }

        [HttpPost]
        [Authorize]
        [Route("edit/returnUrl")]
        public ActionResult Edit(CreateEmployeeViewModel newEmployee, FormCollection formCollection,
            HttpPostedFileBase photo, string returnUrl)
        {
            CheckAndAddModelError(newEmployee, formCollection);
            if (!ModelState.IsValid)
            {
                var designations = _designationService.GetAllDesignation().Select(designation
                     => new SelectListItem { Text = designation.Description, Value = designation.DesignationID.ToString() }).ToList();
                newEmployee.Designations = designations;
                var religions = _religionService.GetAll().Select(x => new SelectListItem { Text = x.Name, Value = x.ReligionID.ToString() }).ToList();
                var departments = _departmnetService.GetAllDepartment().Select(x => new SelectListItem { Text = x.DESCRIPTION, Value = x.DepartmentId.ToString() }).ToList();

                newEmployee.Religions = religions;
                newEmployee.Departments = departments;
                return View("Create", newEmployee);
            }

            if (newEmployee != null)
            {
                var existingEmployee = _employeeService.GetEmployeeById(int.Parse(newEmployee.Id));
                MapFormCollectionValueWithExistingEntity(existingEmployee, formCollection);

                if (photo != null)
                {
                    var photoName = newEmployee.Code + "_" + newEmployee.Name;
                    existingEmployee.PhotoPath = SaveHttpPostedImageFile(photoName, Server.MapPath(_photoPath), photo);
                }

                existingEmployee.Code = newEmployee.Code;
                existingEmployee.Name = newEmployee.Name;
                existingEmployee.ContactNo = newEmployee.ContactNo;
                existingEmployee.JoiningDate = newEmployee.JoiningDate;
                existingEmployee.DesignationID = int.Parse(newEmployee.DesignationName);
                existingEmployee.FName = newEmployee.FName;
                existingEmployee.MName = newEmployee.MName;
                existingEmployee.EmailID = newEmployee.EmailId;
                existingEmployee.NID = newEmployee.NId;
                existingEmployee.PermanentAdd = newEmployee.PermanentAddress;
                existingEmployee.PresentAdd = newEmployee.PresentAddress;
                existingEmployee.BloodGroup = newEmployee.BloodGroup;
                existingEmployee.GrossSalary = decimal.Parse(newEmployee.GrossSalary);
                existingEmployee.DOB = newEmployee.DateOfBirth;
                existingEmployee.SRDueLimit = decimal.Parse(newEmployee.SRDueLimit);
                existingEmployee.ConcernID = User.Identity.GetConcernId();
                existingEmployee.DepartmentID = newEmployee.DepartmentID;
                existingEmployee.MachineEMPID = newEmployee.MachineEMPID;
                existingEmployee.ReligionID = newEmployee.ReligionID;

                _employeeService.UpdateEmployee(existingEmployee);
                _employeeService.SaveEmployee();

                AddToastMessage("", "Employee has been updated successfully.", ToastType.Success);
                return RedirectToAction("Index");
            }
            else
            {
                AddToastMessage("", "No Employee data found to update.", ToastType.Error);
                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        [Authorize]
        [Route("delete/{id}")]
        public ActionResult Delete(int id)
        {
            _employeeService.DeleteEmployee(id);
            _employeeService.SaveEmployee();
            AddToastMessage("", "Employee has been deleted successfully.", ToastType.Success);
            return RedirectToAction("Index");
        }

        private void CheckAndAddModelError(CreateEmployeeViewModel newEmployee, FormCollection formCollection)
        {
            if (string.IsNullOrEmpty(formCollection["JoiningDate"]))
                ModelState.AddModelError("JoiningDate", "Joining Date is required");

            if (string.IsNullOrEmpty(formCollection["DateOfBirth"]))
                ModelState.AddModelError("DateOfBirth", "Birth Date is required");

            if (newEmployee.DepartmentID == null)
                ModelState.AddModelError("DepartmentID", "Department is required");

            if (newEmployee.MachineEMPID == 0)
                ModelState.AddModelError("MachineEMPID", "Machine account No is required");
            else
            {
                int EmployeeID = Convert.ToInt32(newEmployee.Id);
                if (_employeeService.GetAllEmployeeIQueryable().Any(i => i.MachineEMPID == newEmployee.MachineEMPID && i.EmployeeID != EmployeeID))
                {
                    ModelState.AddModelError("MachineEMPID", "This account No is already exists.");
                }
            }

            var sysInfo = _SysInfoService.GetSystemInformationByConcernId(User.Identity.GetConcernId());
            if (sysInfo != null)
            {
                if (sysInfo.EmployeeDueLimitApply == 1)
                {
                    if (Convert.ToDecimal(newEmployee.SRDueLimit) <= 0m)
                        ModelState.AddModelError("SRDueLimit", "Customer Due Limit is Required.");
                }
            }
        }

        private void MapFormCollectionValueWithNewEntity(CreateEmployeeViewModel newProduct,
            FormCollection formCollection)
        {
            newProduct.JoiningDate = DateTime.Parse(formCollection["JoiningDate"]);
            newProduct.DateOfBirth = DateTime.Parse(formCollection["DateOfBirth"]);
            if (!string.IsNullOrEmpty(formCollection["EndOfContractDate"]))
            {
                newProduct.EndOfContractDate = DateTime.Parse(formCollection["EndOfContractDate"]);
            }
        }

        private void MapFormCollectionValueWithExistingEntity(Employee product,
            FormCollection formCollection)
        {
            product.JoiningDate = DateTime.Parse(formCollection["JoiningDate"]);
            product.DOB = DateTime.Parse(formCollection["DateOfBirth"]);
        }

        [HttpGet]
        public ActionResult Active(int id)
        {
            var employee = _employeeService.GetEmployeeById(id);
            employee.Status = EnumActiveInactive.Active;
            AddAuditTrail(employee, false);
            _employeeService.UpdateEmployee(employee);
            _employeeService.SaveEmployee();
            AddToastMessage("", "Employee activated succesfully.", ToastType.Success);
            return RedirectToAction("Index");

        }

        [HttpGet]
        public ActionResult InActive(int id)
        {
            var employee = _employeeService.GetEmployeeById(id);
            employee.Status = EnumActiveInactive.InActive;
            AddAuditTrail(employee, false);
            _employeeService.UpdateEmployee(employee);
            _employeeService.SaveEmployee();
            AddToastMessage("", "Employee deactivated succesfully.", ToastType.Success);
            return RedirectToAction("Index");

        }
        [HttpGet]
        public JsonResult GetEmployees()
        {
            var employees = (from e in _employeeService.GetAllEmployee()
                             select new
                             {
                                 e.EmployeeID,
                                 Name = e.Name + "(" + e.ContactNo + ")"
                             }).ToList();
            if (employees.Count() == 0)
            {
                return Json(new { status = false, msg = "Employee not found." }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { status = true, data = employees }, JsonRequestBehavior.AllowGet);

        }
        public JsonResult GetEmployeesByName(string prefix)
        {
            if (!string.IsNullOrWhiteSpace(prefix))
            {
                var employees = from c in _employeeService.GetAllEmployee()
                                where c.Name.ToLower().Contains(prefix.ToLower())
                                select new
                                {
                                    ID = c.EmployeeID,
                                    Name = c.Name
                                };
                if (employees.Count() > 0)
                    return Json(employees, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var employees = (from c in _employeeService.GetAllEmployee()
                                 select new
                                 {
                                     ID = c.EmployeeID,
                                     Name = c.Name
                                 }).Take(10);
                if (employees.Count() > 0)
                    return Json(employees, JsonRequestBehavior.AllowGet);
            }

            return Json(false, JsonRequestBehavior.AllowGet);

        }

        public JsonResult GetEmployeesWithDueByNameAndCustomer(string prefix, int customerId)
        {
            var employeeCustomerDueList = _employeeWiseCustomerDueService.GetAll();
            if (!string.IsNullOrWhiteSpace(prefix))
            {
                var employees = (from c in _employeeService.GetAllEmployee()
                                 where c.Name.ToLower().Contains(prefix.ToLower())
                                 select new VMCommonDDL
                                 {
                                     Id = c.EmployeeID,
                                     Name = c.Name,
                                     Due = 0m
                                 }).ToList();
                if (employees.Any())
                {
                    foreach (var emp in employees)
                    {
                        var singleEmp = employeeCustomerDueList.Where(e => e.EmployeeID == emp.Id && e.CustomerID == customerId).FirstOrDefault();
                        emp.Due = singleEmp != null ? singleEmp.CustomerDue : emp.Due;
                    }
                    return Json(employees, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                var employees = (from c in _employeeService.GetAllEmployee()
                                 select new VMCommonDDL
                                 {
                                     Id = c.EmployeeID,
                                     Name = c.Name,
                                     Due = 0m
                                 }).ToList().Take(10);

                if (employees.Any())
                {
                    foreach (var emp in employees)
                    {
                        var singleEmp = employeeCustomerDueList.Where(e => e.EmployeeID == emp.Id && e.CustomerID == customerId).FirstOrDefault();
                        emp.Due = singleEmp != null ? singleEmp.CustomerDue : emp.Due;
                    }
                    return Json(employees, JsonRequestBehavior.AllowGet);
                }
            }

            return Json(false, JsonRequestBehavior.AllowGet);

        }


        public JsonResult GetEmployeesWithDueByCustomer(int customerId)
        {

            int empID = User.Identity.GetEmployeeID();
            string employeeName = string.Empty;
            if (empID > 0)
            {
                var employeeCustomerDue = _employeeWiseCustomerDueService.GetByEmpCustomer(empID, customerId);
                if (employeeCustomerDue != null)
                {
                    return Json(new
                    {
                        result = true,
                        data = new
                        {
                            Id = employeeCustomerDue.EmployeeID,
                            Due = employeeCustomerDue.CustomerDue
                        }
                    },
                  JsonRequestBehavior.AllowGet);
                }
            }

            return Json(false, JsonRequestBehavior.AllowGet);

        }
    }
}