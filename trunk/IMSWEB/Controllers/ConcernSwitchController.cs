using IMSWEB.Helper;
using IMSWEB.Model;
using IMSWEB.Model.TOs;
using IMSWEB.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IMSWEB.Controllers
{
    [Authorize]
    [RoutePrefix("concern-switch")]
    public class ConcernSwitchController : CoreController
    {
        private readonly ISisterConcernService _sisterConcernService;
        public ConcernSwitchController(IErrorService errorService, ISisterConcernService sisterConcernService)
            : base(errorService)
        {
            _sisterConcernService = sisterConcernService;
        }

        // GET: ConcernSwitch
        [HttpGet]
        public ActionResult Index(string returnUrl)
        {
            ViewBag.ConcernIds = _sisterConcernService.GetFamilyTreeDDL(User.Identity.GetConcernId());
            ViewBag.ReturnUrl = returnUrl;
            int concernId = User.Identity.GetConcernId();
            return View(new ConcernSwitchTO { ConcernId = concernId });
        }

        [HttpPost]
        public ActionResult Index(ConcernSwitchTO model, string returnUrl)
        {
            if (model.ConcernId > 0)
            {
                #region update identity concernID
                //ClaimsHelper.AddClaim("ProjectId", model.ProjectId.ToString(), true);
                User.AddUpdateClaim("ConcernID", model.ConcernId.ToString());
                int concernId = User.Identity.GetConcernId();
                if (concernId > 0)
                {
                    SisterConcern concern = _sisterConcernService.GetSisterConcernById(concernId);
                    if (TempData.ContainsKey("ConcernName"))
                    {
                        TempData.Remove("ConcernName");
                        TempData["ConcernName"] = concern.Name;
                    }
                }

                #endregion

                if (Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }
                return RedirectToAction("Index", "Home");
            }
            else
            {
                AddToastMessage("", "Please select a concern!", ToastType.Error);
                ViewBag.ConcernIds = _sisterConcernService.GetFamilyTreeDDL(User.Identity.GetConcernId());
                ViewBag.ReturnUrl = returnUrl;
                return View(model);
            }
        }
    }
}