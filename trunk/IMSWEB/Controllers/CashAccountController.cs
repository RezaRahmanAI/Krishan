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
    public class CashAccountController : CoreController
    {
        private readonly IMapper _mapper;
        private readonly ICashAccountService _cashAccountService;

        public CashAccountController(IErrorService errorService, ICashAccountService cashAccountService, IMapper Mapper)
            : base(errorService)
        {
            _cashAccountService = cashAccountService;
            _mapper = Mapper;
        }

        [HttpGet]
        [Authorize]
        [Route("index")]
        public async Task<ActionResult> Index()
        {
            var itemsAsync = _cashAccountService.GetAllCashAccountAsync();
            var vmodel = _mapper.Map<IEnumerable<CashAccount>, IEnumerable<CashAccountVM>>(await itemsAsync);
            return View(vmodel);
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View(new CashAccountVM());
        }

        [HttpPost]
        [Authorize]
        [Route("create/returnUrl")]
        public ActionResult Create(CashAccountVM model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(new CashAccountVM());
            }

            if (model != null)
            {
                CashAccount newModel = _mapper.Map<CashAccountVM, CashAccount>(model);
                newModel.TotalBalance = model.OpeningBalance;
                AddAuditTrail(newModel, true);
                _cashAccountService.Add(newModel);
                if (_cashAccountService.Save())
                    AddToastMessage("", "Saved successfully.", ToastType.Success);
                else
                    AddToastMessage("", "Failed to save.", ToastType.Error);
                return RedirectToAction("Create");
            }
            else
            {
                AddToastMessage("", "No Cash account data found to save.", ToastType.Error);
                return RedirectToAction("Create");
            }
        }

        [HttpGet]
        [Authorize]
        [Route("edit/{id}")]
        public ActionResult Edit(int id)
        {
            var cashacc = _cashAccountService.GetById(id);
            var vmodel = _mapper.Map<CashAccount, CashAccountVM>(cashacc);
            return View("Create", vmodel);
        }

        [HttpPost]
        [Authorize]
        [Route("edit/returnUrl")]
        public ActionResult Edit(CashAccountVM newCashacc, string returnUrl)
        {
            if (!ModelState.IsValid)
                return View("Create",newCashacc);

            if (newCashacc != null)
            {
                var cashacc = _cashAccountService.GetById(newCashacc.Id);

                cashacc.Name = newCashacc.Name;
                cashacc.OpeningBalance = newCashacc.OpeningBalance;
                cashacc.OpeningDate = newCashacc.OpeningDate;


                _cashAccountService.Update(cashacc);
                _cashAccountService.Save();
                AddToastMessage("", "Cash Account has been updated successfully.", ToastType.Success);
                return RedirectToAction("Index");
            }
            else
            {
                AddToastMessage("", "No Cash Account found to update.", ToastType.Error);
                return RedirectToAction("Index");
            }
        }

    }

}