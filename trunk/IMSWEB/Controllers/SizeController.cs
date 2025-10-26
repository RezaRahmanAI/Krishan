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
    [RoutePrefix("size")]
    public class SizeController : CoreController
    {
        ISizeService _sizeService;
        IMiscellaneousService<Size> _miscellaneousService;
        IMapper _mapper;

        public SizeController(IErrorService errorService, ISizeService GradeService, IMapper Mapper,
            IMiscellaneousService<Size> miscellaneousService, IMapper mapper)
            : base(errorService)
        {
            _sizeService = GradeService;
            _miscellaneousService = miscellaneousService;
            _mapper = Mapper;
        }
        [HttpGet]
        [Authorize]
        [Route("index")]
        public ActionResult Index()
        {
            var sizes = _sizeService.GetAll();
            var vmgrades = _mapper.Map<IQueryable<Size>, IEnumerable<SizeViewModel>>(sizes);
            return View(vmgrades);
        }


        [HttpGet]

        public ActionResult Create()
        {
            var code = _miscellaneousService.GetUniqueKey(i => (int)i.SizeID);
            return View(new SizeViewModel() { Code = code });
        }

        [HttpPost]

        public ActionResult Create(SizeViewModel NewSize, FormCollection formcollection)
        {
            if (!ModelState.IsValid)
                return View(NewSize);
            var size = _mapper.Map<SizeViewModel, Size>(NewSize);
            AddAuditTrail(size, true);
            _sizeService.Add(size);
            _sizeService.Save();
            AddToastMessage("", "Size Save Successfully", ToastType.Success);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult Delete(int id)
        {
            if (id != 0)
            {
                _sizeService.Delete(id);
                _sizeService.Save();
                AddToastMessage("", "Delete Successfully", ToastType.Success);
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            SizeViewModel VMObj = new SizeViewModel();
            if (id != 0)
            {
                var obj = _sizeService.GetById(id);
                VMObj = _mapper.Map<Size, SizeViewModel>(obj);
            }
            return View("Create", VMObj);
        }

        [HttpPost]
        public ActionResult Edit(SizeViewModel SizeViewModel)
        {
            SizeViewModel VMObj = new SizeViewModel();
            if (SizeViewModel.SizeID != 0)
            {
                var obj = _sizeService.GetById((int)SizeViewModel.SizeID);
                obj.Description = SizeViewModel.Description;
                AddAuditTrail(obj, false);
                _sizeService.Update(obj);
                _sizeService.Save();
                AddToastMessage("", "Update Successfully", ToastType.Success);
            }

            return RedirectToAction("Index");
        }



        [HttpPost]
        public JsonResult AddSize(string Name)
        {
            if (!string.IsNullOrWhiteSpace(Name))
            {
                if (_sizeService.GetAll().Any(i => i.Description.ToLower().Equals(Name.Trim().ToLower())))
                {
                    return Json(new { result = false, msg = "This Size is  exist." }, JsonRequestBehavior.AllowGet);
                }

                Size size = new Size();
                size.Description = Name.Trim();
                size.Code = _miscellaneousService.GetUniqueKey(x => int.Parse(x.Code));
                AddAuditTrail(size, true);
                _sizeService.Add(size);
                _sizeService.Save();
                return Json(new { result = true, data = size }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { result = false, msg = "failed." }, JsonRequestBehavior.AllowGet);

        }



        public JsonResult GetSizeByName(string prefix)
        {
            if (!string.IsNullOrWhiteSpace(prefix))
            {
                var companies = from c in _sizeService.GetAll()
                                where c.Description.ToLower().Contains(prefix.ToLower())
                                select new
                                {
                                    ID = c.SizeID,
                                    Name = c.Description
                                };
                if (companies.Count() > 0)
                    return Json(companies, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var companies = (from c in _sizeService.GetAll()
                                 select new
                                 {
                                     ID = c.SizeID,
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