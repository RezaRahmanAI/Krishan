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
using IMSWEB.Model.TOs;

namespace IMSWEB.Controllers
{
    [Authorize]
    public class ProductionSetupController : CoreController
    {
        IMapper _mapper;
        IProductionSetupService _ProductionSetupService;
        IMiscellaneousService<ProductionSetup> _miscell;
        IProductService _ProductService;
        private readonly IProductUnitTypeService _productUnitTypeService;
        private readonly ISizeService _sizeService;
        public ProductionSetupController(IErrorService errorService, IProductionSetupService ProductionSetupService,
            IMapper Mapper, IMiscellaneousService<ProductionSetup> miscell, IProductService ProductService, IProductUnitTypeService productUnitTypeService, ISizeService sizeService)
            : base(errorService)
        {
            _ProductionSetupService = ProductionSetupService;
            _mapper = Mapper;
            _miscell = miscell;
            _ProductService = ProductService;
            _productUnitTypeService = productUnitTypeService;
            _sizeService = sizeService;
        }
        public async Task<ActionResult> Index()
        {
            var ProductionSetups = _ProductionSetupService.GetAllProductionSetupAsync();
            var vmProductionSetups = _mapper.Map < IEnumerable<Tuple<string, int>>, IEnumerable< ProductionSetupViewModel >>(await ProductionSetups);
            return View(vmProductionSetups);
        }


        [HttpGet]
        public ActionResult Create()
        {
            //ViewBag.ProductIds = GetAllProductsForDDL();
            var RawMaterials = _ProductService.GetAllProductIQueryableForProduction(EnumProductStockType.Raw_Materials);
            var vmDetails = _mapper.Map<IQueryable<ProductWisePurchaseModel>, List<PSDetailViewModel>>(RawMaterials);
            return View(new ProductionSetupViewModel() { Details = vmDetails.OrderBy(i => i.RAWProductName).ToList() });
        }

        [HttpPost]
        public ActionResult Create(ProductionSetupViewModel newProductionSetup, FormCollection formcollection)
        {
            AddModelError(newProductionSetup, formcollection);
            if (!ModelState.IsValid)
                return View(newProductionSetup);

            ProductionSetup oNewPS = new ProductionSetup();
            oNewPS = _mapper.Map<ProductionSetupViewModel, ProductionSetup>(newProductionSetup);
            var Details = _mapper.Map<List<PSDetailViewModel>, ICollection<ProductionSetupDetail>>(newProductionSetup.Details.Where(i => i.IsSelected).ToList());
            oNewPS.ProductionSetupDetails = Details;

            if (newProductionSetup.PSID == null)
                AddAuditTrail(oNewPS, true);
            else
                AddAuditTrail(oNewPS, false);

            var Result = _ProductionSetupService.ADDPS(oNewPS, Convert.ToInt32(newProductionSetup.PSID));
            if (Result.Item1)
            {
                TempData["PSID"] = Result.Item2;
                TempData["IsProductionSetupReadyById"] = true;
                AddToastMessage("", "Production Setup Save Successfully", ToastType.Success);
            }
            else
                AddToastMessage("", "Production Setup Failed.", ToastType.Error);

            return RedirectToAction("Index");
        }

        private void AddModelError(ProductionSetupViewModel newProductionSetup, FormCollection formcollection)
        {
            if (string.IsNullOrWhiteSpace(formcollection["ProductsId"]))
                ModelState.AddModelError("FINProductID", "Fin. goods is required.");
            else
            {
                newProductionSetup.FINProductID = formcollection["ProductsId"];
                int ProductID = Convert.ToInt32(newProductionSetup.FINProductID);
                if (newProductionSetup.PSID > 0)
                {
                    if (_ProductionSetupService.GetAll().Any(i => i.FINProductID == ProductID && i.PSID != newProductionSetup.PSID))
                        ModelState.AddModelError("FINProductID", "This Fin. goods is already added.");
                }
                else
                {
                    if (_ProductionSetupService.GetAll().Any(i => i.FINProductID == ProductID))
                        ModelState.AddModelError("FINProductID", "This Fin. goods is already added.");
                }
            }

            if (newProductionSetup.Details.Count(i => i.IsSelected) == 0)
                ModelState.AddModelError("Details[0].RawParentQuantity", "Please select raw materials.");
            else
            {
                for (int i = 0; i < newProductionSetup.Details.Count(); i++)
                {
                    if (newProductionSetup.Details[i].IsSelected && newProductionSetup.Details[i].Quantity <= 0)
                        ModelState.AddModelError("Details[" + i + "].RawQuantity", "Quantity is required.");
                }
            }
        }

        [HttpGet]
        public ActionResult Delete(int id)
        {
            if (id != 0)
            {
                _ProductionSetupService.Delete(id);
                _ProductionSetupService.Save();
                AddToastMessage("", "Delete Successfully", ToastType.Success);
            }
            return RedirectToAction("Index");
        }


        [HttpGet]
        public ActionResult Edit(int id)
        {
            ProductionSetupViewModel VMObj = new ProductionSetupViewModel();
            if (id != 0)
            {
                var obj = _ProductionSetupService.GetById(id);
                var Details = (from p in _ProductService.GetAllProductIQueryableForProduction(EnumProductStockType.Raw_Materials)
                               join d in _ProductionSetupService.GetDetailsById(id)
                               on p.ProductID equals d.RAWProductID into ld
                               from d in ld.DefaultIfEmpty()
                               select new PSDetailViewModel
                               {
                                   PSDID = d != null ? d.PSDID : 0,
                                   PSID = d != null ? d.PSID : 0,
                                   RawParentQuantity = d != null ? d.ParentQuantity : 0,
                                   RAWChildQuantity = d != null ? d.ChildQuantity : 0,
                                   Quantity = d != null ? d.Quantity : 0,
                                   RAWProductID = d != null ? d.RAWProductID : p.ProductID,
                                   RAWProductName = p.ProductName,
                                   IsSelected = d != null ? true : false,
                                   ConvertValue = p.ConvertValue
                               }).OrderByDescending(i => i.PSDID).ThenBy(i => i.RAWProductName).ToList();
                VMObj = _mapper.Map<ProductionSetup, ProductionSetupViewModel>(obj);
                VMObj.Details = Details;
            }
            return View("Create", VMObj);
        }


        [HttpGet]
        [Authorize]
        public ActionResult Invoice(int orderId)
        {
            TempData["IsProductionSetupReadyById"] = true;
            TempData["PSID"] = orderId;
            return RedirectToAction("Index");
        }

        //[HttpGet]
        //[Authorize]
        //public JsonResult GetProductInfoById(int productId)
        //{
        //    var products = _ProductService.GetAllProductFromDetailById(productId);

        //    var vmProductDetails = _mapper.Map<IEnumerable<Tuple<int, string, string, decimal, string, string, string, Tuple<decimal?, string, decimal, int, int, string, string, Tuple<string, string, string, string, string, string, int, Tuple<int, decimal, decimal, decimal, decimal, decimal>>>>>, IEnumerable<GetProductViewModel>>(products).ToList();


        //    var vmProductGroupBY = (from vm in vmProductDetails
        //                            join pu in _productUnitTypeService.GetAll() on vm.ProUnitTypeID equals pu.ProUnitTypeID
        //                            join s in _sizeService.GetAll() on vm.SizeID equals s.SizeID
        //                            group vm by new
        //                            {
        //                                vm.IMENo,
        //                                vm.ProductId,
        //                                vm.CategoryID,
        //                                vm.ProductName,
        //                                vm.ProductCode,

        //                                vm.ColorId,
        //                                vm.CategoryName,
        //                                vm.ColorName,
        //                                vm.ModelName,
        //                                vm.GodownName,
        //                                vm.CompanyName,
        //                                vm.ProUnitTypeID,
        //                                ChildUnit = pu.UnitName,
        //                                ParentUnit = pu.Description,
        //                                ConvertValue = vm.BundleQty == 0 ? pu.ConvertValue : vm.BundleQty,
        //                                s.SizeID,
        //                                SizeName = s.Description,
        //                                vm.PurchaseCSft,
        //                                vm.SalesCSft,
        //                                vm.TotalSFT,
        //                                vm.AdvSRate

        //                            } into g
        //                            select new GetProductViewModel
        //                            {
        //                                IMENo = g.Key.IMENo,
        //                                ProductId = g.Key.ProductId,
        //                                ProductCode = g.Key.ProductCode,
        //                                ProductName = g.Key.ProductName,
        //                                CategoryID = g.Key.CategoryID,

        //                                CategoryName = g.Key.CategoryName,
        //                                ColorName = g.Key.ColorName,
        //                                ColorId = g.Key.ColorId,
        //                                ModelName = g.Key.ModelName,
        //                                StockDetailsId = g.Select(o => o.StockDetailsId).FirstOrDefault(),

        //                                MRPRate = g.Select(o => o.MRPRate).FirstOrDefault(),
        //                                AdvSRate = g.Select(o => o.AdvSRate).FirstOrDefault(),
        //                                ParentMRP = ((g.Select(o => o.AdvSRate).FirstOrDefault()) * g.Key.ConvertValue),
        //                                MRPRate12 = g.Select(o => o.MRPRate12).FirstOrDefault(),
        //                                CashSalesRate = g.Select(o => o.CashSalesRate).FirstOrDefault(),
        //                                PWDiscount = g.Select(o => o.PWDiscount).FirstOrDefault(),
        //                                PicturePath = g.Select(o => o.PicturePath).FirstOrDefault(),

        //                                PreStock = g.Select(o => o.PreStock).FirstOrDefault(),
        //                                OfferDescription = g.Select(o => o.OfferDescription).FirstOrDefault(),
        //                                ProductType = g.Select(o => o.ProductType).FirstOrDefault(),
        //                                CompressorWarrentyMonth = g.Select(o => o.CompressorWarrentyMonth).FirstOrDefault(),
        //                                PanelWarrentyMonth = g.Select(o => o.PanelWarrentyMonth).FirstOrDefault(),

        //                                MotorWarrentyMonth = g.Select(o => o.MotorWarrentyMonth).FirstOrDefault(),
        //                                SparePartsWarrentyMonth = g.Select(o => o.SparePartsWarrentyMonth).FirstOrDefault(),
        //                                ServiceWarrentyMonth = g.Select(o => o.ServiceWarrentyMonth).FirstOrDefault(),
        //                                IsSelect = g.Select(o => o.IsSelect).FirstOrDefault(),
        //                                Status = g.Select(o => o.Status).FirstOrDefault(),

        //                                Quantity = g.Select(o => o.Quantity).FirstOrDefault(), //e.g.; gm
        //                                GodownName = g.Key.GodownName,
        //                                SizeID = g.Key.SizeID,
        //                                SizeName = g.Key.SizeName,
        //                                CompanyName = g.Key.CompanyName,

        //                                ChildUnit = g.Key.ChildUnit,
        //                                ConvertValue = g.Key.ConvertValue,
        //                                ParentUnit = g.Key.ParentUnit,
        //                                PurchaseCSft = g.Key.PurchaseCSft,
        //                                SalesCSft = g.Key.SalesCSft,
        //                                ParentQty = (int)Math.Truncate(g.Select(o => o.PreStock).FirstOrDefault() / g.Key.ConvertValue), //e.g. KG
        //                                ChildQty = (int)(g.Select(o => o.PreStock).FirstOrDefault() % g.Key.ConvertValue), //e.g. gm

        //                                TotalSFT = g.Key.TotalSFT
        //                            }).OrderBy(p => p.ProductId).FirstOrDefault();



        //    return Json(vmProductGroupBY, JsonRequestBehavior.AllowGet);

        //}

        //private List<TOIdNameDDL> GetAllProductsForDDL()
        //{
        //    var products = _ProductService.GetAllProductFromDetailById();

        //    var vmProductDetails = _mapper.Map<IEnumerable<Tuple<int, string, string, decimal, string, string, string, Tuple<decimal?, string, decimal, int, int, string, string, Tuple<string, string, string, string, string, string, int, Tuple<int, decimal, decimal, decimal, decimal, decimal>>>>>, IEnumerable<GetProductViewModel>>(products).ToList();


        //    var vmProductGroupBY = (from vm in vmProductDetails
        //                            join pu in _productUnitTypeService.GetAll() on vm.ProUnitTypeID equals pu.ProUnitTypeID
        //                            join s in _sizeService.GetAll() on vm.SizeID equals s.SizeID
        //                            group vm by new
        //                            {
        //                                vm.IMENo,
        //                                vm.ProductId,
        //                                vm.CategoryID,
        //                                vm.ProductName,
        //                                vm.ProductCode,

        //                                vm.ColorId,
        //                                vm.CategoryName,
        //                                vm.ColorName,
        //                                vm.ModelName,
        //                                vm.GodownName,
        //                                vm.CompanyName,
        //                                vm.ProUnitTypeID,
        //                                ChildUnit = pu.UnitName,
        //                                ParentUnit = pu.Description,
        //                                ConvertValue = vm.BundleQty == 0 ? pu.ConvertValue : vm.BundleQty,
        //                                s.SizeID,
        //                                SizeName = s.Description,
        //                                vm.PurchaseCSft,
        //                                vm.SalesCSft,
        //                                vm.TotalSFT,
        //                                vm.AdvSRate

        //                            } into g
        //                            select new GetProductViewModel
        //                            {
        //                                IMENo = g.Key.IMENo,
        //                                ProductId = g.Key.ProductId,
        //                                ProductCode = g.Key.ProductCode,
        //                                ProductName = g.Key.ProductName,
        //                                CategoryID = g.Key.CategoryID,

        //                                CategoryName = g.Key.CategoryName,
        //                                ColorName = g.Key.ColorName,
        //                                ColorId = g.Key.ColorId,
        //                                ModelName = g.Key.ModelName,
        //                                StockDetailsId = g.Select(o => o.StockDetailsId).FirstOrDefault(),

        //                                MRPRate = g.Select(o => o.MRPRate).FirstOrDefault(),
        //                                AdvSRate = g.Select(o => o.AdvSRate).FirstOrDefault(),
        //                                ParentMRP = ((g.Select(o => o.AdvSRate).FirstOrDefault()) * g.Key.ConvertValue),
        //                                MRPRate12 = g.Select(o => o.MRPRate12).FirstOrDefault(),
        //                                CashSalesRate = g.Select(o => o.CashSalesRate).FirstOrDefault(),
        //                                PWDiscount = g.Select(o => o.PWDiscount).FirstOrDefault(),
        //                                PicturePath = g.Select(o => o.PicturePath).FirstOrDefault(),

        //                                PreStock = g.Select(o => o.PreStock).FirstOrDefault(),
        //                                OfferDescription = g.Select(o => o.OfferDescription).FirstOrDefault(),
        //                                ProductType = g.Select(o => o.ProductType).FirstOrDefault(),
        //                                CompressorWarrentyMonth = g.Select(o => o.CompressorWarrentyMonth).FirstOrDefault(),
        //                                PanelWarrentyMonth = g.Select(o => o.PanelWarrentyMonth).FirstOrDefault(),

        //                                MotorWarrentyMonth = g.Select(o => o.MotorWarrentyMonth).FirstOrDefault(),
        //                                SparePartsWarrentyMonth = g.Select(o => o.SparePartsWarrentyMonth).FirstOrDefault(),
        //                                ServiceWarrentyMonth = g.Select(o => o.ServiceWarrentyMonth).FirstOrDefault(),
        //                                IsSelect = g.Select(o => o.IsSelect).FirstOrDefault(),
        //                                Status = g.Select(o => o.Status).FirstOrDefault(),

        //                                Quantity = g.Select(o => o.Quantity).FirstOrDefault(), //e.g.; gm
        //                                GodownName = g.Key.GodownName,
        //                                SizeID = g.Key.SizeID,
        //                                SizeName = g.Key.SizeName,
        //                                CompanyName = g.Key.CompanyName,

        //                                ChildUnit = g.Key.ChildUnit,
        //                                ConvertValue = g.Key.ConvertValue,
        //                                ParentUnit = g.Key.ParentUnit,
        //                                PurchaseCSft = g.Key.PurchaseCSft,
        //                                SalesCSft = g.Key.SalesCSft,
        //                                ParentQty = (int)Math.Truncate(g.Select(o => o.PreStock).FirstOrDefault() / g.Key.ConvertValue), //e.g. KG
        //                                ChildQty = (int)(g.Select(o => o.PreStock).FirstOrDefault() % g.Key.ConvertValue), //e.g. gm

        //                                TotalSFT = g.Key.TotalSFT
        //                            }).OrderBy(p => p.ProductId).Select(s => new TOIdNameDDL
        //                            {
        //                                Id = s.ProductId,
        //                                Name = s.ProductName + ", " + s.CategoryName + " (" + s.SizeName + ")"
        //                            });
        //    return vmProductGroupBY.ToList();
        //}

    }
}