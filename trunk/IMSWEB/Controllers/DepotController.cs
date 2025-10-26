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
    [RoutePrefix("Depot")]
    public class DepotController : CoreController
    {
        IDepotService _DepotService;
        IMiscellaneousService<Depot> _miscellaneousService;
        IMapper _mapper;

        public DepotController(IErrorService errorService,
            IDepotService DepotService, IMiscellaneousService<Depot> miscellaneousService, IMapper mapper)
            : base(errorService)
        {
            _DepotService = DepotService;
            _miscellaneousService = miscellaneousService;
            _mapper = mapper;
        }

        [HttpGet]
        [Authorize]
        [Route("index")]
        public async Task<ActionResult> Index()
        {
            var DepotAsync = _DepotService.GetAllDepotAsync();
            var vmodel = _mapper.Map<IEnumerable<Depot>, IEnumerable<CreateDepotViewModel>>(await DepotAsync);
            return View(vmodel);
        }

        [HttpGet]
        [Authorize]
        [Route("create")]
        public ActionResult Create()
        {
            string code = _miscellaneousService.GetUniqueKey(x => int.Parse(x.Code));
            return View(new CreateDepotViewModel { Code = code });
        }

        [HttpPost]
        [Authorize]
        [Route("create/returnUrl")]
        public ActionResult Create(CreateDepotViewModel newDepot, string returnUrl)
        {
            if (!ModelState.IsValid)
                return View(newDepot);

            if (newDepot != null)
            {

                var existingDepot = _miscellaneousService.GetDuplicateEntry(c => c.DepotName == newDepot.Name);
                if (existingDepot != null)
                {
                    AddToastMessage("", "A Depot with same name already exists in the system. Please try with a different name.", ToastType.Error);
                    return View(existingDepot);
                }

                var model = _mapper.Map<CreateDepotViewModel, Depot>(newDepot);
                model.ConcernID = User.Identity.GetConcernId();
                _DepotService.AddDepot(model);
                _DepotService.SaveDepot();

                AddToastMessage("", "Depot has been saved successfully.", ToastType.Success);
                return RedirectToAction("Create");
            }
            else
            {
                AddToastMessage("", "No Depot data found to create.", ToastType.Error);
                return RedirectToAction("Create");
            }
        }

        [HttpGet]
        [Authorize]
        [Route("edit/{id}")]
        public ActionResult Edit(int id)
        {
            var model = _DepotService.GetDepotById(id);
            var vmodel = _mapper.Map<Depot, CreateDepotViewModel>(model);
            return View("Create", vmodel);
        }

        [HttpPost]
        [Authorize]
        [Route("edit/returnUrl")]
        public ActionResult Edit(CreateDepotViewModel newDepot, string returnUrl)
        {
            if (!ModelState.IsValid)
                return View(newDepot);

            if (newDepot != null)
            {
                var model = _DepotService.GetDepotById(int.Parse(newDepot.Id));

                model.Code = newDepot.Code;
                model.DepotName = newDepot.Name;
                model.ConcernID = User.Identity.GetConcernId(); ;

                _DepotService.UpdateDepot(model);
                _DepotService.SaveDepot();

                AddToastMessage("", "Depot has been updated successfully.", ToastType.Success);
                return RedirectToAction("Index");
            }
            else
            {
                AddToastMessage("", "No Depot data found to update.", ToastType.Error);
                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        [Authorize]
        [Route("delete/{id}")]
        public ActionResult Delete(int id)
        {
            _DepotService.DeleteDepot(id);
            _DepotService.SaveDepot();
            AddToastMessage("", "Depot has been deleted successfully.", ToastType.Success);
            return RedirectToAction("Index");
        }
    }
}