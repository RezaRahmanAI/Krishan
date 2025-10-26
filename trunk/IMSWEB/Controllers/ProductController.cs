using AutoMapper;
using IMSWEB.Model;
using IMSWEB.Service;
using PagedList;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace IMSWEB.Controllers
{
    [Authorize]
    [RoutePrefix("product")]
    public class ProductController : CoreController
    {
        IProductService _productService;
        IMiscellaneousService<Product> _miscellaneousService;
        IMapper _mapper;
        string _photoPath = "~/Content/photos/products";
        IProductUnitTypeService _ProductUnitTypeService;
        IPurchaseOrderService _PurchaseOrderService;
        IDiaSizeService _DiaSizeService;
        public ProductController(IErrorService errorService,
            IProductService productService, IMiscellaneousService<Product> miscellaneousService, IMapper mapper,
            IProductUnitTypeService ProductUnitTypeService, IPurchaseOrderService PurchaseOrderService, IDiaSizeService DiaSizeService)
            : base(errorService)
        {
            _productService = productService;
            _miscellaneousService = miscellaneousService;
            _mapper = mapper;
            _ProductUnitTypeService = ProductUnitTypeService;
            _PurchaseOrderService = PurchaseOrderService;
            _DiaSizeService = DiaSizeService;
        }

        [HttpGet]
        [Authorize]
        [Route("index")]
        public ActionResult Index(int? Page)
        {
            int PageSize = 10;
            int Pages = Page.HasValue ? Convert.ToInt32(Page) : 1;
            var customProductAsync = _productService.GetAllProductIQueryableIndex(EnumProductStockType.Finished_Goods);
            var vmodel = _mapper.Map<IQueryable<ProductWisePurchaseModel>, List<GetProductViewModel>>(customProductAsync);
            var pagelist = vmodel.ToPagedList(Pages, PageSize);
            return View(pagelist);


        }

        [HttpPost]
        [Authorize]

        public ActionResult Index(FormCollection formCollection)
        {
            int PageSize = 10;
            int Pages = 1;
            IQueryable<ProductWisePurchaseModel> customProductAsync = null;
            if (!string.IsNullOrEmpty(formCollection["ProductName"]))
            {
                string productName = formCollection["ProductName"];
                customProductAsync = _productService.GetAllProductIQueryableIndex(EnumProductStockType.Finished_Goods).Where(i => i.ProductName.Contains(productName));
            }
            var vmodel = _mapper.Map<IQueryable<ProductWisePurchaseModel>, List<GetProductViewModel>>(customProductAsync);
            var pagelist = vmodel.ToPagedList(Pages, PageSize);
            return View(pagelist);


        }

        CreateProductViewModel PopulateDropdown(CreateProductViewModel newPRoduct)
        {
            newPRoduct.ProductUnitTypes = _ProductUnitTypeService.GetAllActive().ToList();
            return newPRoduct;
        }



        [HttpGet]
        [Authorize]
        [Route("create")]
        public ActionResult Create()

        {
            string code = _miscellaneousService.GetUniqueKey(x => int.Parse(x.Code));
            var UnitType = _ProductUnitTypeService.GetAll().FirstOrDefault();
            //var DiaSize = _DiaSizeService.GetAll().FirstOrDefault();
            return View(PopulateDropdown(new CreateProductViewModel { Code = code, UnitType = (UnitType == null ? 0 : UnitType.ProUnitTypeID), ProductType = EnumProductType.NoBarcode }));
        }



        [HttpPost]
        [Authorize]
        [Route("create/returnUrl")]
        public ActionResult Create(CreateProductViewModel newProduct, FormCollection formCollection,
        HttpPostedFileBase picture, string returnUrl)
        {
            CheckAndAddModelError(newProduct, formCollection);
            if (!ModelState.IsValid)
            {
                string code = _miscellaneousService.GetUniqueKey(x => int.Parse(x.Code));
                var UnitType = _ProductUnitTypeService.GetAll().FirstOrDefault();
                return View(PopulateDropdown(new CreateProductViewModel { Code = code, UnitType = (UnitType == null ? 0 : UnitType.ProUnitTypeID), ProductType = EnumProductType.NoBarcode }));
            }

            if (newProduct != null)
            {
                var existingProduct = _miscellaneousService.GetDuplicateEntry(p => p.ProductName == newProduct.ProductName && p.SizeID == newProduct.SizeID);
                if (existingProduct != null)
                {
                    AddToastMessage("", "A Product with same name and size already exists in the system. Please try with a different name.", ToastType.Error);
                    newProduct.ProductUnitTypes = _ProductUnitTypeService.GetAllActive().ToList();
                    return View(newProduct);
                }

                //MapFormCollectionValueWithNewEntity(newProduct, formCollection);
                if (picture != null)
                {
                    var photoName = newProduct.Code + "_" + newProduct.ProductName;
                    newProduct.PicturePath = SaveHttpPostedImageFile(photoName, Server.MapPath(_photoPath), picture);
                }

                var product = _mapper.Map<CreateProductViewModel, Product>(newProduct);
                product.PWDiscount = 0;
                product.ProductStockType = EnumProductStockType.Finished_Goods;
                product.ConcernID = User.Identity.GetConcernId();
                //product.UnitType = EnumUnitType.Piece;


                _productService.AddProduct(product);
                _productService.SaveProduct();

                AddToastMessage("", "Product has been saved successfully.", ToastType.Success);
                return RedirectToAction("Create");
            }
            else
            {
                AddToastMessage("", "No Product data found to save.", ToastType.Error);
                return RedirectToAction("Create");
            }
        }



        [HttpGet]
        [Authorize]
        [Route("edit/{id}")]
        public ActionResult Edit(int id)
        {
            var product = _productService.GetAllProductIQueryable().FirstOrDefault(i => i.ProductID == id);
            //var productType = _productService.GetAllProductIQueryable().FirstOrDefault(i => i.UnitType == id);
            var vmodel = _mapper.Map<ProductWisePurchaseModel, CreateProductViewModel>(product);
            return View("Create", PopulateDropdown(vmodel));
        }

        
        [HttpPost]
        [Authorize]
        [Route("edit/returnUrl")]
        public ActionResult Edit(CreateProductViewModel newProduct, FormCollection formCollection,
            HttpPostedFileBase picture, string returnUrl)
        {
            CheckAndAddModelError(newProduct, formCollection);
            //if (string.IsNullOrEmpty(newProduct.PicturePath))
            //    ModelState.AddModelError("PicturePath", "Picture is required");

            if (!ModelState.IsValid)
                return View("Create", PopulateDropdown(newProduct));

            if (newProduct != null)
            {
                Product DuplicateProduct = null;
                int ProductID = Convert.ToInt32(newProduct.ProductId);
                if (formCollection["CategoryName"].ToLower().Equals("tiles"))
                    DuplicateProduct = _miscellaneousService.GetDuplicateEntry(p => p.ProductName == newProduct.ProductName && p.SizeID == newProduct.SizeID && p.IDCode.Equals(newProduct.IDCode) && p.ProductID != ProductID);
                else
                    DuplicateProduct = _miscellaneousService.GetDuplicateEntry(p => p.ProductName == newProduct.ProductName && p.SizeID == newProduct.SizeID && p.IDCode.Equals(newProduct.IDCode) && p.ProductID != ProductID);

                if (DuplicateProduct != null)
                {
                    AddToastMessage("", "A Product with same name and size already exists in the system. Please try with a different.", ToastType.Error);
                    return View(PopulateDropdown(newProduct));
                }

                var existingProduct = _productService.GetProductById(int.Parse(newProduct.ProductId));
                MapFormCollectionValueWithExistingEntity(existingProduct, formCollection);
                if (picture != null)
                {
                    var photoName = newProduct.Code + "_" + newProduct.ProductName;
                    existingProduct.PicturePath = SaveHttpPostedImageFile(photoName, Server.MapPath(_photoPath), picture);
                }

                bool Result = _PurchaseOrderService.IsProductPurchase(existingProduct.ProductID);

                if (!Result)
                {
                    existingProduct.PurchaseCSft = newProduct.PurchaseCSft;
                    existingProduct.SalesCSft = newProduct.SalesCSft;
                }
                existingProduct.Code = newProduct.Code;
                existingProduct.ProductName = newProduct.ProductName;
                existingProduct.CompressorWarrentyMonth = newProduct.CompressorWarrentyMonth;
                existingProduct.MotorWarrentyMonth = newProduct.MotorWarrentyMonth;
                existingProduct.PanelWarrentyMonth = newProduct.PanelWarrentyMonth;
                existingProduct.SparePartsWarrentyMonth = newProduct.SparePartsWarrentyMonth;
                existingProduct.ServiceWarrentyMonth = newProduct.ServiceWarrentyMonth;
                existingProduct.BundleQty = newProduct.BundleQty;
                existingProduct.IDCode = newProduct.IDCode;
                existingProduct.SizeID = newProduct.SizeID;
                existingProduct.MRP = newProduct.MRP;
                existingProduct.CompanyID = Convert.ToInt32(newProduct.CompanyId);
                existingProduct.CategoryID = Convert.ToInt32(newProduct.CategoryId);
                existingProduct.DP = newProduct.DP;
                if (newProduct.UnitType != existingProduct.ProUnitTypeID)
                {
                    if (Result)
                        AddToastMessage("", "This product is already purchase. You can't change the unit type.", ToastType.Error);
                    else
                        existingProduct.ProUnitTypeID = newProduct.UnitType;
                }

                if (newProduct.PWDiscount == null)
                    existingProduct.PWDiscount = 0;
                else
                    existingProduct.PWDiscount = decimal.Parse(newProduct.PWDiscount);

                existingProduct.ConcernID = User.Identity.GetConcernId();
                existingProduct.ProductStockType = EnumProductStockType.Finished_Goods;

                _productService.UpdateProduct(existingProduct);
                _productService.SaveProduct();

                AddToastMessage("", "Product has been updated successfully.", ToastType.Success);
                return RedirectToAction("Index");
            }
            else
            {
                AddToastMessage("", "No Product data found to update.", ToastType.Error);
                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        [Authorize]
        [Route("delete/{id}")]
        public ActionResult Delete(int id)
        {
            _productService.DeleteProduct(id);
            _productService.SaveProduct();
            AddToastMessage("", "Product has been deleted successfully.", ToastType.Success);
            return RedirectToAction("Index");
        }

        private void CheckAndAddModelError(CreateProductViewModel newProduct, FormCollection formCollection)
        {
            if (string.IsNullOrEmpty(formCollection["CompanyId"]))
                ModelState.AddModelError("CompanyId", "Company is required");
            else
                newProduct.CompanyId = formCollection["CompanyId"];

            if (string.IsNullOrEmpty(formCollection["CategoryId"]))
                ModelState.AddModelError("CategoryId", "Category is required");
            else
                newProduct.CategoryId = formCollection["CategoryId"];

            if (string.IsNullOrEmpty(formCollection["SizeID"]))
                ModelState.AddModelError("SizeID", "Size is required");
            else
            {
                newProduct.SizeID = Convert.ToInt32(formCollection["SizeID"]);
                if (!string.IsNullOrEmpty(formCollection["CategoryName"]))
                {
                    if (formCollection["CategoryName"].ToLower().Equals("tiles"))
                    {
                        var Size = formCollection["SizeID"].ToString().Split('x');
                        if (Size.Length != 2)
                            ModelState.AddModelError("SizeID", "The format of size is not valid for tiles.");

                        if (string.IsNullOrEmpty(newProduct.IDCode))
                            ModelState.AddModelError("IDCode", "Identity code is required.");
                    }
                }
            }

            if (IsCartonBundlePacket(newProduct.UnitType))
            {
                if (newProduct.BundleQty <= 0)
                    ModelState.AddModelError("BundleQty", "Quantity is required.");
            }

            if (!string.IsNullOrEmpty(newProduct.CompressorWarrentyMonth))
            {
                var comp = newProduct.CompressorWarrentyMonth.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (comp.Length == 2)
                    newProduct.CompressorWarrentyMonth = comp[0] + " " + formCollection["Compressor"];
                else
                    newProduct.CompressorWarrentyMonth = newProduct.CompressorWarrentyMonth + " " + formCollection["Compressor"];
            }

            if (!string.IsNullOrEmpty(newProduct.MotorWarrentyMonth))
            {
                var comp = newProduct.MotorWarrentyMonth.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (comp.Length == 2)
                    newProduct.MotorWarrentyMonth = comp[0] + " " + formCollection["Motor"];
                else
                    newProduct.MotorWarrentyMonth = newProduct.MotorWarrentyMonth + " " + formCollection["Motor"];

            }

            if (!string.IsNullOrEmpty(newProduct.PanelWarrentyMonth))
            {
                var comp = newProduct.PanelWarrentyMonth.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (comp.Length == 2)
                    newProduct.PanelWarrentyMonth = comp[0] + " " + formCollection["Panel"];
                else
                    newProduct.PanelWarrentyMonth = newProduct.PanelWarrentyMonth + " " + formCollection["Panel"];

            }

            if (!string.IsNullOrEmpty(newProduct.ServiceWarrentyMonth))
            {
                var comp = newProduct.ServiceWarrentyMonth.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (comp.Length == 2)
                    newProduct.ServiceWarrentyMonth = comp[0] + " " + formCollection["Service"];
                else
                    newProduct.ServiceWarrentyMonth = newProduct.ServiceWarrentyMonth + " " + formCollection["Service"];

            }

            if (!string.IsNullOrEmpty(newProduct.SparePartsWarrentyMonth))
            {
                var comp = newProduct.SparePartsWarrentyMonth.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (comp.Length == 2)
                    newProduct.SparePartsWarrentyMonth = comp[0] + " " + formCollection["SpareParts"];
                else
                    newProduct.SparePartsWarrentyMonth = newProduct.SparePartsWarrentyMonth + " " + formCollection["SpareParts"];

            }

            //if (User.Identity.GetConcernId() == (int)EnumSisterConcern.KT_Feed)
            //{
            //    newProduct.DiaSizeID = newProduct.DiaSizeID;
            //}
            //else
            //    newProduct.DiaSizeID = 0;
        }

        private bool IsCartonBundlePacket(int UnitTypeID)
        {
            var oUnit = _ProductUnitTypeService.GetById(UnitTypeID);
            if (oUnit != null)
            {
                if (oUnit.UnitName.ToLower().Equals("carton") || oUnit.UnitName.ToLower().Equals("bundle") || oUnit.UnitName.ToLower().Equals("packet"))
                    return true;
            }
            return false;
        }

        private void MapFormCollectionValueWithNewEntity(CreateProductViewModel newProduct,
            FormCollection formCollection)
        {
            newProduct.CompanyId = formCollection["CompaniesId"];
            newProduct.CategoryId = formCollection["CategoriesId"];
        }

        private void MapFormCollectionValueWithExistingEntity(Product product,
            FormCollection formCollection)
        {
            product.DisDurationFDate = Convert.ToDateTime("31 Dec 2017");//DateTime.Parse(formCollection["DisDurationFDate"]);
            product.DisDurationToDate = Convert.ToDateTime("31 Dec 2017");//DateTime.Parse(formCollection["DisDurationToDate"]);
            product.CompanyID = int.Parse(formCollection["CompanyId"]);
            product.CategoryID = int.Parse(formCollection["CategoryId"]);
            product.PWDiscount = 0;
        }

        [HttpGet]
        [Authorize]
        public ActionResult ProductWisePandSReport()
        {
            return View("ProductWisePandSReport");
        }


        [HttpPost]
        public JsonResult GetProductByName(string ProductName)
        {
            var products = (from d in _productService.GetAllProductIQueryable()
                            where d.ProductName.ToLower().Contains(ProductName.ToLower())
                            select new
                            {
                                d.ProductName,
                                d.ProductID
                            }).ToList();
            if (products.Count() > 0)
                return Json(products, JsonRequestBehavior.AllowGet);

            return Json(new { result = false }, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult AddProduct(CreateProductViewModel newProduct, FormCollection formCollection)
        {
            newProduct.Code = _miscellaneousService.GetUniqueKey(x => int.Parse(x.Code));
            CheckAndAddModelError(newProduct, formCollection);
            if (!ModelState.IsValid)
                return Json(new { result = false, msg = "Product save failed." }, JsonRequestBehavior.AllowGet);
            var existingProduct = _miscellaneousService.GetDuplicateEntry(p => p.ProductName == newProduct.ProductName);
            if (existingProduct != null)
            {
                return Json(new { result = false, msg = "A Product with same name already exists in the system. Please try with a different name." }, JsonRequestBehavior.AllowGet);
            }
            var product = _mapper.Map<CreateProductViewModel, Product>(newProduct);
            product.PWDiscount = 0;
            AddAuditTrail(product, true);
            _productService.AddProduct(product);
            _productService.SaveProduct();

            //AddConcernsProducts(newProduct);
            var vmProduct = new
            {
                ProductID = product.ProductID,
                Code = product.Code,
                ProductType = product.ProductType,
                ProductName = product.ProductName,
                MRP = product.MRP
            };
            return Json(new { result = true, msg = "Save successfull", data = vmProduct }, JsonRequestBehavior.AllowGet);
        }

    }
}