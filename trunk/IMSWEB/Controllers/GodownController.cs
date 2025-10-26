﻿using AutoMapper;
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
    [RoutePrefix("godown")]
    public class GodownController : CoreController
    {
        IGodownService _godownService;
        IMiscellaneousService<Godown> _miscellaneousService;
        IMapper _mapper;
         
        public GodownController(IErrorService errorService,
            IGodownService godownService, IMiscellaneousService<Godown> miscellaneousService, IMapper mapper)
            : base(errorService)
        {
            _godownService = godownService;
            _miscellaneousService = miscellaneousService;
            _mapper = mapper;
        }

        [HttpGet]
        [Authorize]
        [Route("index")]
        public async Task<ActionResult> Index()
        {
            var godownsAsync = _godownService.GetAllGodownAsync();
            var vmodel = _mapper.Map<IEnumerable<Godown>, IEnumerable<CreateGodownViewModel>>(await godownsAsync);
            return View(vmodel);
        }

        [HttpGet]
        [Authorize]
        [Route("create")]
        public ActionResult Create()
        {
            string code = _miscellaneousService.GetUniqueKey(x=>int.Parse(x.Code));
            return View(new CreateGodownViewModel { Code = code });
        }

        [HttpPost]
        [Authorize]
        [Route("create/returnUrl")]
        public ActionResult Create(CreateGodownViewModel newGodown, string returnUrl)
        {
            if (!ModelState.IsValid)
                return View(newGodown);

            if (newGodown != null)
            {
                var existingGodown = _miscellaneousService.GetDuplicateEntry(c => c.Name == newGodown.Name);
                if(existingGodown != null)
                {
                    AddToastMessage("", "A Godown with same name already exists in the system. Please try with a different name.", ToastType.Error);
                    return View(newGodown);
                }

                var godown = _mapper.Map<CreateGodownViewModel, Godown>(newGodown);
                AddAuditTrail(godown,true);
                _godownService.AddGodown(godown);
                _godownService.SaveGodown();

                AddToastMessage("", "Godown has been saved successfully.", ToastType.Success);
                return RedirectToAction("Create");
            }
            else
            {
                AddToastMessage("", "No Godown data found to create.", ToastType.Error);
                return RedirectToAction("Create");
            }
        }

        [HttpGet]
        [Authorize]
        [Route("edit/{id}")]
        public ActionResult Edit(int id)
        {
            var godown = _godownService.GetGodownById(id);
            var vmodel = _mapper.Map<Godown, CreateGodownViewModel>(godown);
            return View("Create", vmodel);
        }

        [HttpPost]
        [Authorize]
        [Route("edit/returnUrl")]
        public ActionResult Edit(CreateGodownViewModel newGodown, string returnUrl)
        {
            if (!ModelState.IsValid)
                return View("Create", newGodown);

            if (newGodown != null)
            {
                var godown = _godownService.GetGodownById(int.Parse(newGodown.Id));
                AddAuditTrail(godown,false);

                godown.Code = newGodown.Code;
                godown.Name = newGodown.Name;
                if (newGodown.ISCommon == true)
                    godown.ISCommon = 1;
                else
                    godown.ISCommon = 0;

               
                _godownService.UpdateGodown(godown);
                _godownService.SaveGodown();

                AddToastMessage("", "Godown has been updated successfully.", ToastType.Success);
                return RedirectToAction("Index");
            }
            else
            {
                AddToastMessage("", "No Godown data found to update.", ToastType.Error);
                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        [Authorize]
        [Route("delete/{id}")]
        public ActionResult Delete(int id)
        {
            _godownService.DeleteGodown(id);
            _godownService.SaveGodown();
            AddToastMessage("", "Godown has been deleted successfully.", ToastType.Success);
            return RedirectToAction("Index");
        }
    }
}