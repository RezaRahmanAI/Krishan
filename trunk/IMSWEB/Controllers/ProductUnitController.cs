using AutoMapper;
using IMSWEB.Model;
using IMSWEB.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IMSWEB.Controllers
{
    public class ProductUnitController : CoreController
    {
        IMapper _mapper;
        IProductUnitTypeService _ProductUnitTypeService;
        IMiscellaneousService<ProductUnitType> _miscell;
        public ProductUnitController(IErrorService errorService, IProductUnitTypeService GradeService, IMapper Mapper, IMiscellaneousService<ProductUnitType> miscell)
            : base(errorService)
        {
            _ProductUnitTypeService = GradeService;
            _mapper = Mapper;
            _miscell = miscell;
        }
        public ActionResult Index()
        {
            var pu = _ProductUnitTypeService.GetAll();
            var vmpu = _mapper.Map<IEnumerable<ProductUnitType>, IEnumerable<ProductUnitTypeViewModel>>(pu);
            return View(vmpu);
        }


        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(ProductUnitTypeViewModel ProductUnitType, FormCollection formcollection)
        {

            if (!ModelState.IsValid)
                return View(ProductUnitType);
            var productUnit = _mapper.Map<ProductUnitTypeViewModel, ProductUnitType>(ProductUnitType);
            AddAuditTrail(productUnit, true);
            _ProductUnitTypeService.Add(productUnit);
            _ProductUnitTypeService.Save();
            AddToastMessage("", "ProductUnitType Save Successfully", ToastType.Success);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult Delete(int id)
        {
            if (id != 0)
            {
                _ProductUnitTypeService.Delete(id);
                _ProductUnitTypeService.Save();
                AddToastMessage("", "Delete Successfully", ToastType.Success);
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult Active(int id)
        {
            if (id != 0)
            {
                var obj = _ProductUnitTypeService.GetById(id);
                obj.Status = (int)EnumActiveInactive.Active; ;
                _ProductUnitTypeService.Update(obj);
                _ProductUnitTypeService.Save();
                AddToastMessage("", "Active Successfully", ToastType.Success);

            }
            return RedirectToAction("Index");
        }


        [HttpGet]
        public ActionResult Inactive(int id)
        {
            if (id != 0)
            {
                var obj = _ProductUnitTypeService.GetById(id);
                obj.Status = (int)EnumActiveInactive.InActive; ;
                _ProductUnitTypeService.Update(obj);
                _ProductUnitTypeService.Save();
                AddToastMessage("", "Inactive Successfully", ToastType.Success);

            }
            return RedirectToAction("Index");
        }


        [HttpGet]
        public ActionResult Edit(int id)
        {
            ProductUnitTypeViewModel VMObj = new ProductUnitTypeViewModel();
            if (id != 0)
            {
                var obj = _ProductUnitTypeService.GetById(id);
                VMObj = _mapper.Map<ProductUnitType, ProductUnitTypeViewModel>(obj);
            }
            return View("Create", VMObj);
        }

        [HttpPost]
        public ActionResult Edit(ProductUnitTypeViewModel ProductUnitTypeViewModel)
        {
            ProductUnitTypeViewModel VMObj = new ProductUnitTypeViewModel();
            if (ProductUnitTypeViewModel.ProUnitTypeID != 0)
            {
                var obj = _ProductUnitTypeService.GetById(Convert.ToInt32(ProductUnitTypeViewModel.ProUnitTypeID));
                obj.Description = ProductUnitTypeViewModel.Description;
                obj.ConvertValue = ProductUnitTypeViewModel.ConvertValue;
                obj.UnitName = ProductUnitTypeViewModel.UnitName;
                obj.Position = ProductUnitTypeViewModel.Position;
                AddAuditTrail(obj, false);
                _ProductUnitTypeService.Update(obj);
                _ProductUnitTypeService.Save();
                AddToastMessage("", "Update Successfully", ToastType.Success);
            }

            return RedirectToAction("Index");
        }

    }
}