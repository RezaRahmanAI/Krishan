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


namespace IMSWEB.Controllers
{
    [Authorize]
    [RoutePrefix("diasize")]
    public class DiaSizeController : CoreController
    {
        IDiaSizeService _DiaSizeService;
        IMiscellaneousService<DiaSize> _miscellaneousService;
        IMapper _mapper;

        public DiaSizeController(IErrorService errorService, IDiaSizeService DiaSizeService, IMapper Mapper,
            IMiscellaneousService<DiaSize> miscellaneousService, IMapper mapper)
            : base(errorService)
        {
            _DiaSizeService = DiaSizeService;
            _miscellaneousService = miscellaneousService;
            _mapper = Mapper;
        }
        [HttpGet]
        [Authorize]
        [Route("index")]
        public ActionResult Index()
        {
            var sizes = _DiaSizeService.GetAll();
            var vmgrades = _mapper.Map<IQueryable<DiaSize>, IEnumerable<DiaSizeViewModel>>(sizes);
            return View(vmgrades);
        }


        [HttpGet]

        public ActionResult Create()
        {
            var code = _miscellaneousService.GetUniqueKey(i => (int)i.DiaSizeID);
            return View(new DiaSizeViewModel() { Code = code });
        }

        [HttpPost]

        public ActionResult Create(DiaSizeViewModel NewSize, FormCollection formcollection)
        {
            if (!ModelState.IsValid)
                return View(NewSize);
            var size = _mapper.Map<DiaSizeViewModel, DiaSize>(NewSize);
            AddAuditTrail(size, true);
            _DiaSizeService.Add(size);
            _DiaSizeService.Save();
            AddToastMessage("", "Dia. Size Save Successfully", ToastType.Success);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult Delete(int id)
        {
            if (id != 0)
            {
                _DiaSizeService.Delete(id);
                _DiaSizeService.Save();
                AddToastMessage("", "Delete Successfully", ToastType.Success);
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            DiaSizeViewModel VMObj = new DiaSizeViewModel();
            if (id != 0)
            {
                var obj = _DiaSizeService.GetById(id);
                VMObj = _mapper.Map<DiaSize, DiaSizeViewModel>(obj);
            }
            return View("Create", VMObj);
        }

        [HttpPost]
        public ActionResult Edit(SizeViewModel SizeViewModel)
        {
            DiaSizeViewModel VMObj = new DiaSizeViewModel();
            if (SizeViewModel.SizeID != 0)
            {
                var obj = _DiaSizeService.GetById((int)SizeViewModel.SizeID);
                obj.Description = SizeViewModel.Description;
                AddAuditTrail(obj, false);
                _DiaSizeService.Update(obj);
                _DiaSizeService.Save();
                AddToastMessage("", "Update Successfully", ToastType.Success);
            }

            return RedirectToAction("Index");
        }



        [HttpPost]
        public JsonResult AddDiaSize(string Name)
        {
            if (!string.IsNullOrWhiteSpace(Name))
            {
                if (_DiaSizeService.GetAll().Any(i => i.Description.ToLower().Equals(Name.Trim().ToLower())))
                {
                    return Json(new { result = false, msg = "This Dia. Size is  exist." }, JsonRequestBehavior.AllowGet);
                }

                DiaSize size = new DiaSize();
                size.Description = Name.Trim();
                size.Code = _miscellaneousService.GetUniqueKey(x => int.Parse(x.Code));
                AddAuditTrail(size, true);
                _DiaSizeService.Add(size);
                _DiaSizeService.Save();
                return Json(new { result = true, data = size }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { result = false, msg = "failed." }, JsonRequestBehavior.AllowGet);

        }



        public JsonResult GetDiaSizeByName(string prefix)
        {
            if (!string.IsNullOrWhiteSpace(prefix))
            {
                var companies = from c in _DiaSizeService.GetAll()
                                where c.Description.ToLower().Contains(prefix.ToLower())
                                select new
                                {
                                    ID = c.DiaSizeID,
                                    Name = c.Description
                                };
                if (companies.Count() > 0)
                    return Json(companies, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var companies = (from c in _DiaSizeService.GetAll()
                                 select new
                                 {
                                     ID = c.DiaSizeID,
                                     Name = c.Description
                                 }).Take(10);
                if (companies.Count() > 0)
                    return Json(companies, JsonRequestBehavior.AllowGet);
            }
            return Json(false, JsonRequestBehavior.AllowGet);

        }



        //public JsonResult GetSizeByName(string prefix)
        //{
        //    if (!string.IsNullOrWhiteSpace(prefix))
        //    {
        //        var sizes = from c in _SizeService.GetAllIQueryable()
        //                    where c.Description.ToLower().Contains(prefix.ToLower())
        //                    select new
        //                    {
        //                        ID = c.SizeID,
        //                        Name = c.Description
        //                    };
        //        if (sizes.Count() > 0)

        //            return Json(sizes, JsonRequestBehavior.AllowGet);


        //    }
        //    else
        //    {
        //        var sizes = (from c in _SizeService.GetAllIQueryable()
        //                     select new
        //                     {
        //                         ID = c.SizeID,
        //                         Name = c.Description
        //                     }).Take(10);
        //        if (sizes.Count() > 0)
        //            return Json(sizes, JsonRequestBehavior.AllowGet);
        //    }
        //    return Json(false, JsonRequestBehavior.AllowGet);
        //}
    }


}