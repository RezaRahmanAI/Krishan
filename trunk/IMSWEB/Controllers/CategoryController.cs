﻿using AutoMapper;
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
    [Authorize]
    [RoutePrefix("category")]
    public class CategoryController : CoreController
    {
        ICategoryService _categoryService;
        IMiscellaneousService<Category> _miscellaneousService;
        IMapper _mapper;
        public CategoryController(IErrorService errorService,
            ICategoryService categoryService, IMiscellaneousService<Category> miscellaneousService,
            IMapper mapper)
            : base(errorService)
        {
            _categoryService = categoryService;
            _miscellaneousService = miscellaneousService;
            _mapper = mapper;
        }

        [HttpGet]
        [Authorize]
        [Route("index")]
        public async Task<ActionResult> Index()
        {
            var categoriesAsync = _categoryService.GetAllCategoryAsync();
            var vmodel = _mapper.Map<IEnumerable<Category>, IEnumerable<CreateCategoryViewModel>>(await categoriesAsync);
            return View(vmodel);
        }

        [HttpGet]
        [Authorize]
        [Route("create")]
        public ActionResult Create()
        {
            string code = _miscellaneousService.GetUniqueKey(x => int.Parse(x.Code));
            return View(new CreateCategoryViewModel { Code = code });
        }

        [HttpPost]
        [Authorize]
        [Route("create/returnUrl")]
        public ActionResult Create(CreateCategoryViewModel newCategory, string returnUrl)
        {
            if (!ModelState.IsValid)
                return View(newCategory);

            if (newCategory != null)
            {
                var existingCategory = _miscellaneousService.GetDuplicateEntry(c => c.Description == newCategory.Name);
                if (existingCategory != null)
                {
                    AddToastMessage("", "A Category with same name already exists in the system. Please try with a different name.", ToastType.Error);
                    return View(newCategory);
                }

                var category = _mapper.Map<CreateCategoryViewModel, Category>(newCategory);
                category.ConcernID = User.Identity.GetConcernId();
                _categoryService.AddCategory(category);
                _categoryService.SaveCategory();

                AddToastMessage("", "Category has been saved successfully.", ToastType.Success);
                return RedirectToAction("Create");
            }
            else
            {
                AddToastMessage("", "No Category data found to create.", ToastType.Error);
                return RedirectToAction("Create");
            }
        }

        [HttpGet]
        [Authorize]
        [Route("edit/{id}")]
        public ActionResult Edit(int id)
        {
            var category = _categoryService.GetCategoryById(id);
            var vmodel = _mapper.Map<Category, CreateCategoryViewModel>(category);
            return View("Create", vmodel);
        }

        [HttpPost]
        [Authorize]
        [Route("edit/returnUrl")]
        public ActionResult Edit(CreateCategoryViewModel newCategory, string returnUrl)
        {
            if (!ModelState.IsValid)
                return View("Create", newCategory);

            if (newCategory != null)
            {
                var category = _categoryService.GetCategoryById(int.Parse(newCategory.Id));

                category.Code = newCategory.Code;
                category.Description = newCategory.Name;
                category.ConcernID = User.Identity.GetConcernId();

                _categoryService.UpdateCategory(category);
                _categoryService.SaveCategory();

                AddToastMessage("", "Category has been updated successfully.", ToastType.Success);
                return RedirectToAction("Index");
            }
            else
            {
                AddToastMessage("", "No Category data found to update.", ToastType.Error);
                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        [Authorize]
        [Route("delete/{id}")]
        public ActionResult Delete(int id)
        {
            _categoryService.DeleteCategory(id);
            _categoryService.SaveCategory();
            AddToastMessage("", "Category has been deleted successfully.", ToastType.Success);
            return RedirectToAction("Index");
        }



        [HttpPost]
        public JsonResult AddCategory(string Name)
        {
            if (!string.IsNullOrWhiteSpace(Name))
            {
                if (_categoryService.GetAllIQueryable().Any(i => i.Description.ToLower().Equals(Name.Trim().ToLower())))
                {
                    return Json(new { result = false, msg = "This category is already exist." }, JsonRequestBehavior.AllowGet);
                }

                Category category = new Category();
                category.Description = Name.Trim();
                category.Code = _miscellaneousService.GetUniqueKey(x => int.Parse(x.Code));
                AddAuditTrail(category, true);
                _categoryService.AddCategory(category);
                _categoryService.SaveCategory();
                return Json(new { result = true, data = category }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { result = false, msg = "failed." }, JsonRequestBehavior.AllowGet);

        }

        public JsonResult GetCategoryByName(string prefix)
        {
            if (!string.IsNullOrWhiteSpace(prefix))
            {
                var categories = from c in _categoryService.GetAllIQueryable()
                                 where c.Description.ToLower().Contains(prefix.ToLower())
                                 select new
                                 {
                                     ID = c.CategoryID,
                                     Name = c.Description
                                 };
                if (categories.Count() > 0)
                    return Json(categories, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var categories = (from c in _categoryService.GetAllIQueryable()
                                  select new
                                  {
                                      ID = c.CategoryID,
                                      Name = c.Description
                                  }).Take(10);
                if (categories.Count() > 0)
                    return Json(categories, JsonRequestBehavior.AllowGet);
            }
            return Json(false, JsonRequestBehavior.AllowGet);

        }
    }
}