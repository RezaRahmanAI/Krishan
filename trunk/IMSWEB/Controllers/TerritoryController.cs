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
    [Authorize]
    [RoutePrefix("Territory")]
    public class TerritoryController : CoreController
    {
        ITerritoryService _territoryService;
        IMiscellaneousService<Territory> _miscellaneousService;
        IMapper _mapper;

        public TerritoryController(IErrorService errorService,
            ITerritoryService territoryService, IMiscellaneousService<Territory> miscellaneousService, IMapper mapper)
            : base(errorService)
        {
            _territoryService = territoryService;
            _miscellaneousService = miscellaneousService;
            _mapper = mapper;
        }

        [HttpGet]
        [Authorize]
        [Route("index")]
        public async Task<ActionResult> Index()
        {
            var territoryAsync = _territoryService.GetAllTerritoryAsync();
            var vmodel = _mapper.Map<IEnumerable<Territory>, IEnumerable<CreateTerritoryViewModel>>(await territoryAsync);
            return View(vmodel);
        }

        [HttpGet]
        [Authorize]
        [Route("create")]
        public ActionResult Create()
        {
            string code = _miscellaneousService.GetUniqueKey(x => int.Parse(x.Code));
            return View(new CreateTerritoryViewModel { Code = code });
        }

        [HttpPost]
        [Authorize]
        [Route("create/returnUrl")]
        public ActionResult Create(CreateTerritoryViewModel newTerritory, string returnUrl)
        {
            if (!ModelState.IsValid)
                return View(newTerritory);

            if (newTerritory != null)
            {

                var existingTerritory = _miscellaneousService.GetDuplicateEntry(c => c.TerritoryName == newTerritory.Name);
                if (existingTerritory != null)
                {
                    AddToastMessage("", "A Territory with same name already exists in the system. Please try with a different name.", ToastType.Error);
                    return View(existingTerritory);
                }

                var model = _mapper.Map<CreateTerritoryViewModel, Territory>(newTerritory);
                model.ConcernID = User.Identity.GetConcernId();
                _territoryService.AddTerritory(model);
                _territoryService.SaveTerritory();

                AddToastMessage("", "Territory has been saved successfully.", ToastType.Success);
                return RedirectToAction("Create");
            }
            else
            {
                AddToastMessage("", "No Territory data found to create.", ToastType.Error);
                return RedirectToAction("Create");
            }
        }

        [HttpGet]
        [Authorize]
        [Route("edit/{id}")]
        public ActionResult Edit(int id)
        {
            var model = _territoryService.GetTerritoryById(id);
            var vmodel = _mapper.Map<Territory, CreateTerritoryViewModel>(model);
            return View("Create", vmodel);
        }

        [HttpPost]
        [Authorize]
        [Route("edit/returnUrl")]
        public ActionResult Edit(CreateTerritoryViewModel newTerritory, string returnUrl)
        {
            if (!ModelState.IsValid)
                return View(newTerritory);

            if (newTerritory != null)
            {
                var model = _territoryService.GetTerritoryById(int.Parse(newTerritory.Id));

                model.Code = newTerritory.Code;
                model.TerritoryName = newTerritory.Name;
                model.ConcernID = User.Identity.GetConcernId(); ;

                _territoryService.UpdateTerritory(model);
                _territoryService.SaveTerritory();

                AddToastMessage("", "Territory has been updated successfully.", ToastType.Success);
                return RedirectToAction("Index");
            }
            else
            {
                AddToastMessage("", "No Territory data found to update.", ToastType.Error);
                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        [Authorize]
        [Route("delete/{id}")]
        public ActionResult Delete(int id)
        {
            _territoryService.DeleteTerritory(id);
            _territoryService.SaveTerritory();
            AddToastMessage("", "Territory has been deleted successfully.", ToastType.Success);
            return RedirectToAction("Index");
        }
    }
}