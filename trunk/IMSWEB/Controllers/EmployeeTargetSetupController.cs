using AutoMapper;
using IMSWEB.Model;
using IMSWEB.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace IMSWEB.Controllers
{
    public class EmployeeTargetSetupController : CoreController
    {
        IMapper _mapper;
        IEmployeeTargetSetupService _EmployeeTargetSetupService;
        IMiscellaneousService<EmployeeTargetSetup> _miscell;
        ISystemInformationService _SysInfo;
        public EmployeeTargetSetupController(IErrorService errorService, IEmployeeTargetSetupService EmployeeTargetSetupService, IMapper Mapper,
            IMiscellaneousService<EmployeeTargetSetup> miscell, ISystemInformationService SysInfo)
            : base(errorService)
        {
            _EmployeeTargetSetupService = EmployeeTargetSetupService;
            _mapper = Mapper;
            _miscell = miscell;
            _SysInfo = SysInfo;
        }
        public async Task<ActionResult> Index()
        {
            var EmployeeTargetSetup = await _EmployeeTargetSetupService.GetAllAsync();
            var vmEmployeeTargetSetup = _mapper.Map<IEnumerable<Tuple<int, DateTime, decimal, decimal, decimal, decimal, int, Tuple<string>>>, IEnumerable<EmployeeTargetSetupViewModel>>(EmployeeTargetSetup);
            return View(vmEmployeeTargetSetup);
        }


        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(EmployeeTargetSetupViewModel NewEmployeeTargetSetup, FormCollection formcollection)
        {
            AddModelError(NewEmployeeTargetSetup, formcollection);

            if (!ModelState.IsValid)
                return View(NewEmployeeTargetSetup);

            var mgrade = _mapper.Map<EmployeeTargetSetupViewModel, EmployeeTargetSetup>(NewEmployeeTargetSetup);
            AddAuditTrail(mgrade, true);
            mgrade.ConcernID = User.Identity.GetConcernId();
            _EmployeeTargetSetupService.Add(mgrade);
            _EmployeeTargetSetupService.Save();
            AddToastMessage("", "Employee Target Setup Save Successfully", ToastType.Success);
            return RedirectToAction("Index");
        }

        private void AddModelError(EmployeeTargetSetupViewModel NewEmployeeTargetSetup, FormCollection formCollection)
        {
            if (string.IsNullOrEmpty(formCollection["EmployeesId"]))
            {
                ModelState.AddModelError("EmployeeID", "Employee is required.");
            }
            else
                NewEmployeeTargetSetup.EmployeeID = Convert.ToInt32(formCollection["EmployeesId"]);

            if (!string.IsNullOrEmpty(formCollection["TargetMonth"]))
            {
                NewEmployeeTargetSetup.TargetMonth = Convert.ToDateTime(formCollection["TargetMonth"]);
                var SysInfo = _SysInfo.GetSystemInformationByConcernId(User.Identity.GetConcernId());
                var DateRange = GetFirstAndLastDateOfMonth(SysInfo.NextPayProcessDate);
                int ETSID = Convert.ToInt32(NewEmployeeTargetSetup.ETSID);
                if (NewEmployeeTargetSetup.TargetMonth < DateRange.Item1)
                {
                    ModelState.AddModelError("TargetMonth", "Target Month can't be smaller than Salary Process Month.");
                }

                var ExisitingCommissions = _EmployeeTargetSetupService.GetByEmployeeIDandMonth(NewEmployeeTargetSetup.EmployeeID, DateRange.Item1, DateRange.Item2);
                if (ExisitingCommissions.Any(i => NewEmployeeTargetSetup.TargetAmt == i.TargetAmt))
                {
                    ModelState.AddModelError("AmtFrom", "This Amount is already exsits.");
                }
            }
            else
            {
                ModelState.AddModelError("TargetMonth", "Target Month is required.");
            }

        }

        [HttpGet]
        public ActionResult Delete(int id)
        {
            if (id != 0)
            {
                _EmployeeTargetSetupService.Delete(id);
                _EmployeeTargetSetupService.Save();
                AddToastMessage("", "Delete Successfully", ToastType.Success);
            }
            return RedirectToAction("Index");
        }


        [HttpGet]
        public ActionResult Edit(int id)
        {
            EmployeeTargetSetupViewModel VMObj = new EmployeeTargetSetupViewModel();
            if (id != 0)
            {
                var obj = _EmployeeTargetSetupService.GetById(id);
                VMObj = _mapper.Map<EmployeeTargetSetup, EmployeeTargetSetupViewModel>(obj);
            }
            return View("Create", VMObj);
        }

        [HttpPost]
        public ActionResult Edit(EmployeeTargetSetupViewModel EmployeeTargetSetupVM, FormCollection formcollection)
        {
            AddModelError(EmployeeTargetSetupVM, formcollection);
            if (!ModelState.IsValid)
                return View("Create", EmployeeTargetSetupVM);
            if (EmployeeTargetSetupVM.ETSID != 0)
            {
                var obj = _EmployeeTargetSetupService.GetById((int)EmployeeTargetSetupVM.ETSID);
                obj.TargetAmt = EmployeeTargetSetupVM.TargetAmt;
                obj.TargetMonth = EmployeeTargetSetupVM.TargetMonth;
                obj.EmployeeID = EmployeeTargetSetupVM.EmployeeID;
                AddAuditTrail(obj, false);
                _EmployeeTargetSetupService.Update(obj);
                _EmployeeTargetSetupService.Save();
                AddToastMessage("", "Update Successfully", ToastType.Success);
            }
            return RedirectToAction("Index");
        }
    }
}