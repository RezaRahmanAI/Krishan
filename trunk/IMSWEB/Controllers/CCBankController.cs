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
    [RoutePrefix("CCBank")]
    public class CCBankController : CoreController
    {
        ICCBanKService _CCBankService;
        IMiscellaneousService<CCBank> _miscellaneousService;
        IMapper _mapper;
        public CCBankController(IErrorService errorService,
            ICCBanKService CCBankService, IMiscellaneousService<CCBank> miscellaneousService,
            IMapper mapper)
            : base(errorService)
        {
            _CCBankService = CCBankService;
            _miscellaneousService = miscellaneousService;
            _mapper = mapper;
        }

        [HttpGet]
        [Authorize]
        [Route("index")]
        public async Task<ActionResult> Index()
        {
            var ccBankAsync = _CCBankService.GetAllCCBankAsync();
            var vmodel = _mapper.Map<IEnumerable<CCBank>, IEnumerable<CCBankViewModel>>(await ccBankAsync);
            return View(vmodel);
        }

        [HttpGet]
        [Authorize]
        [Route("create")]
        public ActionResult Create()
        {
            string code = _miscellaneousService.GetUniqueKey(x => int.Parse(x.CCBankCode));
            return View(new CCBankViewModel { Code = code });
        }

        [HttpPost]
        [Authorize]
        [Route("create/returnUrl")]
        public ActionResult Create(CCBankViewModel newCCBank, string returnUrl)
        {
            if (!ModelState.IsValid)
                return View(newCCBank);

            if (newCCBank != null)
            {
                var existingCCBankName = _miscellaneousService.GetDuplicateEntry(c => c.CCBankName == newCCBank.Name);
                if (existingCCBankName != null)
                {
                    AddToastMessage("", "A CCBankName with same name already exists in the system. Please try with a different name.", ToastType.Error);
                    return View(newCCBank);
                }

                var CCBankName = _mapper.Map<CCBankViewModel, CCBank>(newCCBank);
                _CCBankService.AddCCBank(CCBankName);
                _CCBankService.SaveCCBank();

                AddToastMessage("", "CCBankName has been saved successfully.", ToastType.Success);
                return RedirectToAction("Create");
            }
            else
            {
                AddToastMessage("", "No CCBankName data found to create.", ToastType.Error);
                return RedirectToAction("Create");
            }
        }

        [HttpGet]
        [Authorize]
        [Route("edit/{id}")]
        public ActionResult Edit(int id)
        {
            var CCBankName = _CCBankService.GetCCBankById(id);
            var vmodel = _mapper.Map<CCBank, CCBankViewModel>(CCBankName);
            return View("Create", vmodel);
        }

        [HttpPost]
        [Authorize]
        [Route("edit/returnUrl")]
        public ActionResult Edit(CCBankViewModel newCCBank, string returnUrl)
        {
            if (!ModelState.IsValid)
                return View("Create", newCCBank);

            if (newCCBank != null)
            {
                var CCBankName = _CCBankService.GetCCBankById(int.Parse(newCCBank.Id));

                CCBankName.CCBankCode = newCCBank.Code;
                CCBankName.CCBankName = newCCBank.Name;


                _CCBankService.UpdateCCBank(CCBankName);
                _CCBankService.SaveCCBank();

                AddToastMessage("", "CCBankName has been updated successfully.", ToastType.Success);
                return RedirectToAction("Index");
            }
            else
            {
                AddToastMessage("", "No CCBankName data found to update.", ToastType.Error);
                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        [Authorize]
        [Route("delete/{id}")]
        public ActionResult Delete(int id)
        {
            _CCBankService.DeleteCCBank(id);
            _CCBankService.SaveCCBank();
            AddToastMessage("", "CCBankName has been deleted successfully.", ToastType.Success);
            return RedirectToAction("Index");
        }



        [HttpPost]
        public JsonResult AddCCBankName(string Name)
        {
            if (!string.IsNullOrWhiteSpace(Name))
            {
                if (_CCBankService.GetAllIQueryable().Any(i => i.CCBankName.ToLower().Equals(Name.Trim().ToLower())))
                {
                    return Json(new { result = false, msg = "This CCBankName is already exist." }, JsonRequestBehavior.AllowGet);
                }

                CCBank CCBank = new CCBank();
                CCBank.CCBankName = Name.Trim();
                CCBank.CCBankCode = _miscellaneousService.GetUniqueKey(x => int.Parse(x.CCBankCode));
                AddAuditTrail(CCBank, true);
                _CCBankService.AddCCBank(CCBank);
                _CCBankService.SaveCCBank();
                return Json(new { result = true, data = CCBank }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { result = false, msg = "failed." }, JsonRequestBehavior.AllowGet);

        }

        public JsonResult GetCCBankNameByName(string prefix)
        {
            if (!string.IsNullOrWhiteSpace(prefix))
            {
                var CCBankName = from c in _CCBankService.GetAllIQueryable()
                                 where c.CCBankName.ToLower().Contains(prefix.ToLower())
                                 select new
                                 {
                                     ID = c.CCBankID,
                                     Name = c.CCBankName
                                 };
                if (CCBankName.Count() > 0)
                    return Json(CCBankName, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var CCBankName = (from c in _CCBankService.GetAllIQueryable()
                                  select new
                                  {
                                      ID = c.CCBankID,
                                      Name = c.CCBankName
                                  }).Take(10);
                if (CCBankName.Count() > 0)
                    return Json(CCBankName, JsonRequestBehavior.AllowGet);
            }
            return Json(false, JsonRequestBehavior.AllowGet);

        }
    }
}