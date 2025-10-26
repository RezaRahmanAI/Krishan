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
    [RoutePrefix("SisterConcern")]
    public class SisterConcernController : CoreController
    {
        ISisterConcernService _sisterConcernService;
        IMapper _mapper;
         
        public SisterConcernController(IErrorService errorService,
            ISisterConcernService sisterConcernService, IMapper mapper)
            : base(errorService)
        {
            _sisterConcernService = sisterConcernService;
            _mapper = mapper;
        }

        [HttpGet]
        [Authorize]
        [Route("index")]
        public async Task<ActionResult> Index()
        {
            var sisterconcernsAsync = _sisterConcernService.GetAllSisterConcernAsync();
            var vmodel = _mapper.Map<IEnumerable<SisterConcern>, IEnumerable<CreateSisterConcernViewModel>>(await sisterconcernsAsync);
            return View(vmodel);
        }

        [HttpGet]
        [Authorize]
        [Route("create")]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        [Route("create/returnUrl")]
        public ActionResult Create(CreateSisterConcernViewModel newSisterConcern, string returnUrl)
        {
            if (!ModelState.IsValid)
                return View(newSisterConcern);

            if (newSisterConcern != null)
            {
                var sisterConcern = _mapper.Map<CreateSisterConcernViewModel, SisterConcern>(newSisterConcern);
                _sisterConcernService.AddSisterConcern(sisterConcern);
                _sisterConcernService.SaveSisterConcern();

                AddToastMessage("", "SisterConcern has been saved successfully.", ToastType.Success);
                return RedirectToAction("Create");
            }
            else
            {
                AddToastMessage("", "No SisterConcern data found to create.", ToastType.Error);
                return RedirectToAction("Create");
            }
        }

        [HttpGet]
        [Authorize]
        [Route("edit/{id}")]
        public ActionResult Edit(int id)
        {
            var sisterConcern = _sisterConcernService.GetSisterConcernById(id);
            var vmodel = _mapper.Map<SisterConcern, CreateSisterConcernViewModel>(sisterConcern);
            return View("Create", vmodel);
        }

        [HttpPost]
        [Authorize]
        [Route("edit/returnUrl")]
        public ActionResult Edit(CreateSisterConcernViewModel newSisterConcern, string returnUrl)
        {

            if (!ModelState.IsValid)
                return View("Create", newSisterConcern);
            if (Convert.ToInt32(newSisterConcern.Id) == newSisterConcern.ParentID)
                ModelState.AddModelError("ParentID", "Parent and child can't be same.");
            if (newSisterConcern != null)
            {
                var sisterConcern = _sisterConcernService.GetSisterConcernById(int.Parse(newSisterConcern.Id));

                sisterConcern.Address = newSisterConcern.Address;
                sisterConcern.Name = newSisterConcern.Name;
                sisterConcern.ContactNo = newSisterConcern.ContactNo;
                sisterConcern.ParentID = newSisterConcern.ParentID == null ? 0 : (int)newSisterConcern.ParentID;
                sisterConcern.ServiceCharge = newSisterConcern.ServiceCharge;
                sisterConcern.SmsContactNo = newSisterConcern.SmsContactNo;

                _sisterConcernService.UpdateSisterConcern(sisterConcern);
                _sisterConcernService.SaveSisterConcern();

                AddToastMessage("", "SisterConcern has been updated successfully.", ToastType.Success);
                return RedirectToAction("Index");
            }
            else
            {
                AddToastMessage("", "No SisterConcern data found to update.", ToastType.Error);
                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        [Authorize]
        [Route("delete/{id}")]
        public ActionResult Delete(int id)
        {
            _sisterConcernService.DeleteSisterConcern(id);
            _sisterConcernService.SaveSisterConcern();
            AddToastMessage("", "SisterConcern has been deleted successfully.", ToastType.Success);
            return RedirectToAction("Index");
        }
    }
}