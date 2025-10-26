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
    public class ManualProductionController : CoreController
    {
        IMapper _mapper;
        IProductionService _ProductionService;
        IMiscellaneousService<Production> _miscell;
        IProductService _ProductService;
        IColorService _colorService;
        IPurchaseOrderService _purchaseOrderService;
        IProductionSetupService _productionSetupService;
        IStockService _stockService;
        IGodownService _GodownService;
        public ManualProductionController(IErrorService errorService, IProductionService ProductionSetupService,
            IMapper Mapper, IMiscellaneousService<Production> miscell, IProductService ProductService,
            IColorService colorService, IPurchaseOrderService purchaseOrderService, IProductionSetupService productionSetupService,
            IStockService stockService, IGodownService godownService)
            : base(errorService)
        {
            _ProductionService = ProductionSetupService;
            _mapper = Mapper;
            _miscell = miscell;
            _ProductService = ProductService;
            _colorService = colorService;
            _purchaseOrderService = purchaseOrderService;
            _productionSetupService = productionSetupService;
            _stockService = stockService;
            _GodownService = godownService;
        }
        public ActionResult Index()
        {
            TempData["ProductionViewModel"] = null;
            var dateRange = GetFirstAndLastDateOfMonth(GetLocalDateTime());
            ViewBag.FromDate = dateRange.Item1;
            ViewBag.ToDate = dateRange.Item2;
            var productions = _ProductionService.GetAll(ViewBag.FromDate, ViewBag.ToDate);
            var vmProductions = _mapper.Map<IEnumerable<Production>, IEnumerable<ProductionViewModel>>(productions);
            return View(vmProductions);
        }

        [HttpPost]
        public ActionResult Index(FormCollection formCollection)
        {
            if (!string.IsNullOrEmpty(formCollection["FromDate"]))
                ViewBag.FromDate = Convert.ToDateTime(formCollection["FromDate"]);
            if (!string.IsNullOrEmpty(formCollection["ToDate"]))
                ViewBag.ToDate = Convert.ToDateTime(formCollection["ToDate"]);

            var productions = _ProductionService.GetAll(ViewBag.FromDate, ViewBag.ToDate);
            var vmProductions = _mapper.Map<IEnumerable<Production>, IEnumerable<ProductionViewModel>>(productions);
            return View(vmProductions);
        }

        [HttpGet]
        public ActionResult Create()
        {
            return ReturnCreateViewWithTempData();
        }

        private ActionResult ReturnCreateViewWithTempData()
        {
            ProductionViewModel production = (ProductionViewModel)TempData.Peek("ProductionViewModel");
            if (production != null)
            {
                //tempdata getting null after redirection, so we're restoring purchaseOrder 
                TempData["ProductionViewModel"] = production;
                return View("Create", production);
            }
            else
            {
                string chllnNo = _miscell.GetUniqueKey(x => x.ProductionID);
                return View(new ProductionViewModel
                {
                    ProductionCode = chllnNo
                });
            }
        }

        [HttpPost]
        public ActionResult Create(ProductionViewModel newProduction, FormCollection formcollection)
        {
            var production = (ProductionViewModel)TempData.Peek("ProductionViewModel") ?? new ProductionViewModel();
            production.Detail = production.Detail ?? new ProductionDetailViewModel();
            if (formcollection.Get("btnAddProduct") != null)
            {
                AddModelError(newProduction, production, formcollection);
                ModelState.Remove("Detail.ChildQuantity");
                ModelState.Remove("Detail.ParentQuantity");
                if (!ModelState.IsValid)
                    return View(production);
                //else if (HasDuplicateIMEIORBarcode(newProduction, production, formcollection))
                //{
                //    AddToastMessage("", "Duplicate IMEI/Barcode found", ToastType.Error);
                //    return View("Create", production);
                //}
                else if (production.Details.Any(i => i.ProductID == newProduction.Detail.ProductID))
                {
                    AddToastMessage("", "This product is already added.", ToastType.Error);
                    return View(production);
                }
                AddProducts(newProduction, production, formcollection);
                ModelState.Clear();
                AddToastMessage("", "Product add successfully.", ToastType.Success);
                return View(production);
            }
            else if (formcollection.Get("btnSave") != null)
            {
                AddSaveModelError(production);
                if (!ModelState.IsValid)
                    return View(production);
                Production oNewPS = new Production();
                oNewPS = _mapper.Map<ProductionViewModel, Production>(production);
                var Details = _mapper.Map<List<ProductionDetailViewModel>, ICollection<ProductionDetail>>(production.Details.ToList());
                oNewPS.ProductionDetails = Details;
                ProductionRawMaterial productionRawMaterial = null;
                ProductWisePurchaseModel oProduct = null;
                int ProductID = 0;
                foreach (var item in oNewPS.ProductionDetails)
                {
                    //var Setup = _productionSetupService.GetByID(item.ProductID);
                    //foreach (var rawitem in Setup.ProductionSetupDetails)
                    //{
                        ProductID = Convert.ToInt32(item.ProductID);
                        oProduct = _ProductService.GetAllProductIQueryableForProduction().FirstOrDefault(i => i.ProductID == ProductID);
                        //productionRawMaterial = new ProductionRawMaterial();
                        //productionRawMaterial.ProductID = rawitem.RAWProductID;
                        //productionRawMaterial.Quantity = rawitem.Quantity * ((item.Quantity) / oProduct.ConvertValue);
                        //item.ProductionRawMaterials.Add(productionRawMaterial);
                    //}
                }
                if (newProduction.ProductionID == null)
                    AddAuditTrail(oNewPS, true);
                else
                    AddAuditTrail(oNewPS, false);
                oNewPS.Status = EnumProductionStatus.Production;
                var Result = _ProductionService.AddManual(oNewPS, Convert.ToInt32(newProduction.ProductionID));
                if (Result.Item1)
                {
                    TempData["ProductionID"] = Result.Item2;
                    TempData["IsProductionReadyById"] = true;
                    AddToastMessage("", "Production Save Successfully", ToastType.Success);
                }
                else
                    AddToastMessage("", "Production Failed.", ToastType.Error);
            }
            return RedirectToAction("Index");
        }

        private void AddSaveModelError(ProductionViewModel production)
        {
            if (production.Details.Count() == 0)
            {
                ModelState.AddModelError("", "Add Product first.");
                AddToastMessage("", "Add Product first.", ToastType.Error);
            }
        }

        private void AddProducts(ProductionViewModel newProduction, ProductionViewModel production, FormCollection formcollection)
        {
            production.Date = newProduction.Date;
            production.ProductionCode = newProduction.ProductionCode;
            production.ProductionID = newProduction.ProductionID;

            //Details add
            production.Detail.ProductName = formcollection["ProductsName"];
            production.Detail.Quantity = newProduction.Detail.Quantity;
            production.Detail.PDetailID = newProduction.Detail.PDetailID;

            production.Details.Add(production.Detail);
            production.Detail = new ProductionDetailViewModel();

            TempData["ProductionViewModel"] = production;
        }

        //private void GetPickerValues(ProductionViewModel newProduction, ProductionViewModel production, FormCollection formcollection)
        //{
        //    var IMEIS = formcollection.AllKeys
        //         .Where(key => key.Contains("IMEI"))
        //         .Select(i => formcollection[i]).ToList();

        //    ProductionIMEIViewModel PIMEI = null;
        //    foreach (var item in IMEIS)
        //    {
        //        PIMEI = new ProductionIMEIViewModel();
        //        PIMEI.IMEI = item;
        //        production.Detail.ProductionIMEIs.Add(PIMEI);
        //    }
        //}

        private void AddModelError(ProductionViewModel newProduction, ProductionViewModel production, FormCollection formcollection)
        {
            if (string.IsNullOrWhiteSpace(formcollection["ProductsId"]))
                ModelState.AddModelError("Detail.ProductID", "Fin. goods is required.");
            else
            {
                production.Detail.ProductID = Convert.ToInt32(formcollection["ProductsId"]);
                newProduction.Detail.ProductID = Convert.ToInt32(formcollection["ProductsId"]);
            }

            //if (newProduction.Detail.ProductID != null)
            //{
            //    var Setup = _productionSetupService.GetByID((int)newProduction.Detail.ProductID);
            //    var SetupDetails = (from d in Setup.ProductionSetupDetails
            //                        join p in _ProductService.GetAllProductIQueryable() on d.RAWProductID equals p.ProductID
            //                        join st in _stockService.GetAllStock() on new { ProductID = d.RAWProductID } equals new { st.ProductID } into lst
            //                        from st in lst.DefaultIfEmpty()
            //                        select new ProductWisePurchaseModel
            //                        {
            //                            ProductCode = p.ProductCode,
            //                            ProductName = p.ProductName,
            //                            CategoryName = p.CategoryName,
            //                            Quantity = d.Quantity * ((newProduction.Detail.Quantity) / newProduction.Detail.ConvertValue),
            //                            StockQty = st != null ? st.Quantity : 0m,
            //                        }).Where(i => i.Quantity > i.StockQty).ToList();

            //    if (SetupDetails.Count() > 0)
            //    {
            //        ModelState.AddModelError("Detail.Quantity", "Raw material stock is not available.");
            //        TempData["RawProducStock"] = SetupDetails;
            //    }
            //    else
            //        TempData.Remove("RawProducStock");

            //}


            //var IMEIS = newProduction.Detail.ProductionIMEIs.ToList();
            //var SIMEIs = IMEIS.GroupBy(i => i.IMEI).Select(i => i.Key);

            //if (IMEIS.Count != SIMEIs.Count())
            //    ModelState.AddModelError("Detail.Quantity", "Duplicate IMEI exists.");

            //if (newProduction.Detail.Quantity != IMEIS.Count)
            //    ModelState.AddModelError("Detail.Quantity", "IMEI number and Qty don't match.");

            if (newProduction.Detail.Quantity <= 0)
                ModelState.AddModelError("Detail.Quantity", "Quantity is required.");

            //production.Detail.ProductionIMEIs = newProduction.Detail.ProductionIMEIs;
            //foreach (var item in production.Details)
            //{
            //    for (int i = 0; i < production.Detail.ProductionIMEIs.Count(); i++)
            //    {
            //        if (item.ProductionIMEIs.Any(j => j.IMEI.Equals(production.Detail.ProductionIMEIs[i].IMEI)))
            //        {
            //            ModelState.AddModelError("Detail.ProductionIMEIs[" + i + "].IMEI", "Duplicate IMEI");
            //        }
            //    }
            //}

        }
        /// <summary>
        /// This method Check in Database and current add to order Product
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        //private bool HasDuplicateIMEIORBarcode(ProductionViewModel newPurchaseOrder, ProductionViewModel purchaseOrder, FormCollection formCollection)
        //{
        //    bool isDuplicate = false;
        //    string[] IMEIS = formCollection.AllKeys
        //                     .Where(key => key.StartsWith("IMEI"))
        //                     .Select(key => formCollection[key])
        //                     .ToArray();

        //    if (!ControllerContext.RouteData.Values["action"].ToString().ToLower().Equals("edit"))
        //    {
        //        isDuplicate = formCollection.AllKeys
        //                .Where(key => key.StartsWith("IMEI"))
        //                .Any(key => _purchaseOrderService.CheckIMENoDuplicacyByConcernId(User.Identity.GetConcernId(), formCollection[key]) > 0);
        //    }
        //    else // edit 
        //    {
        //        var POProductDetails = (List<ProductionIMEIViewModel>)TempData.Peek("OldProducitonIMEI");
        //        int Counter = 0;

        //        if (POProductDetails != null) // edit the existing PODetails(add or delete IMEI)
        //        {
        //            for (int i = 0; i < IMEIS.Count(); i++)
        //            {
        //                if (!POProductDetails.Any(m => m.IMEI.Equals(IMEIS[i])))
        //                    Counter += _purchaseOrderService.CheckIMENoDuplicacyByConcernId(User.Identity.GetConcernId(), IMEIS[i]);
        //            }
        //            if (Counter > 0)
        //                isDuplicate = true;
        //        }
        //        else // add New Product to PO
        //        {
        //            isDuplicate = formCollection.AllKeys
        //               .Where(key => key.StartsWith("IMEI"))
        //               .Any(key => _purchaseOrderService.CheckIMENoDuplicacyByConcernId(User.Identity.GetConcernId(), formCollection[key]) > 0);
        //        }


        //    }

        //    return IMEIS.Length != IMEIS.Distinct().Count() || isDuplicate;
        //}

        [HttpGet]
        [Authorize]
        public ActionResult EditFromView(int id, string previousAction, bool IsDO)
        {
            var production = (ProductionViewModel)TempData.Peek("ProductionViewModel");
            if (production == null)
            {
                AddToastMessage("", "Item has been expired to edit", ToastType.Error);
                if (IsForEdit(previousAction))
                    return RedirectToAction("Index");
                else
                    return RedirectToAction("Create");
            }

            var itemToEdit = production.Details.Where(x => x.ProductID == id).FirstOrDefault();

            if (itemToEdit != null)
            {
                if (IsForEdit(previousAction) && _ProductionService.HasSoldProductCheckByPDetailID(Convert.ToInt32(itemToEdit.PDetailID)))
                {
                    AddToastMessage("", "Some product(s) has already been sold from this order. This order is not editable",
                        ToastType.Error);
                    return RedirectToAction("Edit", new { id = default(int), previousAction = "Edit" });
                }

                production.Details.Remove(itemToEdit);

                //TempData["OLDProductionIMEI"] = itemToEdit.ProductionIMEIs.ToList();
                production.Detail = itemToEdit;
                //production.Detail.ProductionIMEIs = itemToEdit.ProductionIMEIs;
                //itemToEdit.ProductionIMEIs.Clear();
                TempData["ProductionViewModel"] = production;
                if (IsForEdit(previousAction))
                    return RedirectToAction("Edit", new { id = itemToEdit.ProductionID, previousAction = "Edit" });
                else
                    return RedirectToAction("Create");

            }
            else
            {
                AddToastMessage("", "No item found to edit", ToastType.Info);

                if (IsForEdit(previousAction))
                    return RedirectToAction("Index");
                else
                    return RedirectToAction("Create");

            }
        }

        [HttpGet]
        [Authorize]
        [Route("deleteFromView/{id}")]
        public ActionResult DeleteFromView(int id, string previousAction, bool IsDO)
        {
            ProductionViewModel production = (ProductionViewModel)TempData.Peek("ProductionViewModel");
            if (production == null)
            {
                AddToastMessage("", "Item has been expired to delete", ToastType.Error);
                if (IsForEdit(previousAction))
                    return RedirectToAction("Index");
                else
                    return RedirectToAction("Create");
            }

            var itemToDelete = production.Details.Where(x => x.ProductID == id).FirstOrDefault();

            if (itemToDelete != null)
            {
                if (IsForEdit(previousAction) && _ProductionService.HasSoldProductCheckByPDetailID(Convert.ToInt32(itemToDelete.PDetailID)))
                {
                    AddToastMessage("", "Some product(s) has already been sold from this order. This order is not deletable",
                        ToastType.Error);
                    return RedirectToAction("Edit", new { id = default(int), previousAction = "Edit" });
                }
                production.Details.Remove(itemToDelete);
                TempData["ProductionViewModel"] = production;
                //TempData["OLDProductionIMEI"] = itemToDelete.ProductionIMEIs.ToList();
                production.Detail = new ProductionDetailViewModel();
                AddToastMessage("", "Item has been removed successfully", ToastType.Success);
                if (IsForEdit(previousAction))
                    return RedirectToAction("Edit", new { id = default(int), previousAction = "Edit" });
                else
                    return RedirectToAction("Create");
            }
            else
            {
                AddToastMessage("", "No item found to remove", ToastType.Info);

                if (IsForEdit(previousAction))
                    return RedirectToAction("Index");
                else
                    return RedirectToAction("Create");
            }
        }

        private bool IsForEdit(string previousAction)
        {
            return previousAction.Equals("edit");
        }
        [HttpGet]
        public ActionResult Delete(int id)
        {
            if (id != 0)
            {
                var Production = _ProductionService.GetById(id);
                foreach (var item in Production.ProductionDetails)
                {
                    if (_ProductionService.HasSoldProductCheckByPDetailID(item.PDetailID))
                    {
                        AddToastMessage("", "Some product(s) has already been sold from this order. This order is not deletable", ToastType.Error);
                        return RedirectToAction("Index");
                    }
                }
                if (_ProductionService.Delete(id, User.Identity.GetUserId<int>(), GetLocalDateTime()))
                    AddToastMessage("", "Delete Successfully", ToastType.Success);
                else
                    AddToastMessage("", "Delete failed", ToastType.Error);

            }
            return RedirectToAction("Index");
        }


        [HttpGet]
        public ActionResult Edit(int id, string previousAction)
        {
            ProductionViewModel VMObj = new ProductionViewModel();
            if (id != 0 && TempData["ProductionViewModel"] == null)
            {
                var obj = _ProductionService.GetById(id);
                var Detail = _ProductionService.GetDetailsById(id);
                var vmDetails = _mapper.Map<IQueryable<ProductionDetail>, List<ProductionDetailViewModel>>(Detail);

                var Details = (from p in _ProductService.GetAllProductIQueryableForProduction(EnumProductStockType.Finished_Goods).ToList()
                               join d in vmDetails on p.ProductID equals d.ProductID
                               select new ProductionDetailViewModel
                               {
                                   PDetailID = (int)d.PDetailID,
                                   ProductionID = d.ProductionID,
                                   Quantity = d.Quantity,
                                   ProductID = d.ProductID,
                                   ProductName = p.ProductName,
                                   //ProductionIMEIs = d.ProductionIMEIs,
                                   //ProductionRawMaterials = d.ProductionRawMaterials
                               }).OrderByDescending(i => i.PDetailID).ThenBy(i => i.ProductName).ToList();
                VMObj = _mapper.Map<Production, ProductionViewModel>(obj);
                VMObj.Details = Details;
                VMObj.Detail = new ProductionDetailViewModel();
                TempData["ProductionViewModel"] = VMObj;
            }
            else
            {
                return ReturnCreateViewWithTempData();
            }
            return View("Create", VMObj);
        }


        [HttpGet]
        [Authorize]
        public ActionResult Invoice(int id)
        {
            TempData["IsProductionReadyById"] = true;
            TempData["ProductionID"] = id;
            return RedirectToAction("Index");
        }

        [HttpGet]
        [Authorize]
        public ActionResult ProductionReport()
        {
            return View();
        }
    }
}