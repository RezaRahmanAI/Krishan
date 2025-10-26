using AutoMapper;
using IMSWEB.Model;
using IMSWEB.Service;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using System.Data;
using log4net;
using System.IO;

namespace IMSWEB.Controllers
{
    [Authorize]

    [RoutePrefix("WebSiteProducts")]
    public class WebSiteProductsController : CoreController
    {
        IWebsiteProductService _websiteProductService;     
        IMapper _mapper;
        ISisterConcernService _SisterConcern;       
        IUserService _UserService;

        ISystemInformationService _SysInfoService;
        string _photoPath = "~/Content/Document/WebsiteProduct";
        IDocumentsInfoService _documentsInfoService;
        IMiscellaneousService<WebsiteProducts> _miscellaneousService;
        private readonly string uploadFileName = "WebProduct_";

        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public WebSiteProductsController(IErrorService errorService,                   
            IMiscellaneousService<WebsiteProducts> miscellaneousService, IMapper mapper,ISisterConcernService sisterConcern, IWebsiteProductService websiteProductService,
            IUserService UserService,ISystemInformationService SysInfoService, IDocumentsInfoService documentsInfoService)
            : base(errorService)
        {
            _miscellaneousService = miscellaneousService;
            _mapper = mapper;
            _SisterConcern = sisterConcern;            
            _UserService = UserService;
            _SysInfoService = SysInfoService;
            _documentsInfoService = documentsInfoService;
            _websiteProductService = websiteProductService;
        }


        [HttpGet]
        [Route("index")]
        public async Task<ActionResult> Index()
        {
            var productAsync = _websiteProductService.GetAllProductAsync();
            var vmodel = _mapper.Map<IEnumerable<WebsiteProducts>, IEnumerable<WebsiteProductViewModels>>(await productAsync);
            return View(vmodel);
        }


        [HttpGet]
        [Authorize]
        [Route("create")]
        public ActionResult Create()
        {
            return View(new WebsiteProductViewModels());
        }

        [HttpPost]
        [Authorize]
        [Route("create/returnUrl")]
        public ActionResult Create(WebsiteProductViewModels newProduct, FormCollection formCollection,
          HttpPostedFileBase photo, string returnUrl)
        {
            //CheckAndAddModelError(newNotice, formCollection);
            if (!ModelState.IsValid)
            {

                return View(newProduct);
            }

            if (newProduct != null)
            {
                var products = _mapper.Map<WebsiteProductViewModels, WebsiteProducts>(newProduct);
                products.ConcernID = User.Identity.GetConcernId();
                AddAuditTrail(products, true);
                _websiteProductService.Add(products);
                _websiteProductService.SaveWebsitePRoduct();

                #region save docs
                List<VMDocInfo> docDetails = UploadImage(Request, products.Id.ToString());
                string folderPath = string.Empty;
                //newCustomer.PhotoPath = docDetails.DocPath;
                foreach (var docInfo in docDetails)
                {
                    folderPath = docInfo.DocPath;
                    DocumentsInfo docData = new DocumentsInfo
                    {
                        DocName = docInfo.DocName,
                        DocPath = docInfo.DocPath,
                        DocType = EnumDocType.WebsiteProduct.ToString(),
                        DocSourceId = products.Id,
                        SourceFolder = products.Id.ToString(),
                        ConcernID = User.Identity.GetConcernId()
                    };
                    _documentsInfoService.Add(docData);
                }

                if (_documentsInfoService.Save())
                {
                    WebsiteProducts oldWebsiteProduct = _websiteProductService.GetById(products.Id);
                    oldWebsiteProduct.DocumentPath = folderPath;
                    _websiteProductService.Update(oldWebsiteProduct);
                    _websiteProductService.SaveWebsitePRoduct();
                }

                #endregion

                AddToastMessage("", "Product has been Published successfully.", ToastType.Success);
                return RedirectToAction("Create");
            }
            else
            {
                AddToastMessage("", "No data found to save.", ToastType.Error);
                return RedirectToAction("Create");
            }
        }


        [HttpGet]
        [Authorize]
        [Route("edit/{id}")]
        public ActionResult Edit(int id)
        {
            var products = _websiteProductService.GetById(id);
            var vmodel = _mapper.Map<WebsiteProducts, WebsiteProductViewModels>(products);
            return View("Create", vmodel);
        }

        [HttpPost]
        [Authorize]
        [Route("edit/returnUrl")]
        public ActionResult Edit(WebsiteProducts websiteProducts, FormCollection formCollection)
        {
            if (!ModelState.IsValid)
                return View("Create", websiteProducts);
            if (websiteProducts != null)
            {
                var oldWebProduct = _websiteProductService.GetById(websiteProducts.Id);

                oldWebProduct.Title = websiteProducts.Title;
                oldWebProduct.ProcutCategory = websiteProducts.ProcutCategory;
                oldWebProduct.Description = websiteProducts.Description;
                //oldWebProduct.DocumentPath = websiteProducts.DocumentPath;       
                oldWebProduct.Price = websiteProducts.Price;       

                AddAuditTrail(oldWebProduct, false);

                _websiteProductService.Update(oldWebProduct);
               

                #region save docs
                List<VMDocInfo> docDetails = UploadImage(Request, oldWebProduct.Id.ToString());
                string folderPath = string.Empty;
                
                //newCustomer.PhotoPath = docDetails.DocPath;
                foreach (var docInfo in docDetails)
                {
                    folderPath = docInfo.DocPath;
                    DocumentsInfo docData = new DocumentsInfo
                    {
                        DocName = docInfo.DocName,
                        DocPath = docInfo.DocPath,
                        DocType = EnumDocType.WebsiteProduct.ToString(),
                        DocSourceId = oldWebProduct.Id,
                        SourceFolder = oldWebProduct.Id.ToString(),
                        ConcernID = User.Identity.GetConcernId()
                    };
                    DeletePrevFiles(oldWebProduct.Id, true);
                    _documentsInfoService.Add(docData);
                }

                if (_documentsInfoService.Save())
                {
                    WebsiteProducts oldWebProducts = _websiteProductService.GetById(oldWebProduct.Id);
                    if(folderPath.IsNotNullOrEmpty())
                    {
                        oldWebProducts.DocumentPath = folderPath;
                    }
                    _websiteProductService.Update(oldWebProducts);
                    _websiteProductService.Save();
                }

                #endregion
                if (_websiteProductService.Save())
                {
                    AddToastMessage("", "Web Product has been updated successfully.", ToastType.Success);
                    return RedirectToAction("Index");
                }
                else
                {
                    AddToastMessage("", "Failed to update.", ToastType.Error);
                    return RedirectToAction("Index");
                }
            }
            else
            {
                AddToastMessage("", "No data found to update.", ToastType.Error);
                return RedirectToAction("Index");
            }
        }


        [HttpGet]
        [Authorize]
        [Route("delete/{id}")]
        public ActionResult Delete(int id)
        {
            _websiteProductService.Delete(id);

            if (_websiteProductService.Save())
            {
                DeletePrevFiles(id, true);
                AddToastMessage("", "Web Product has been deleted successfully.", ToastType.Success);
                return RedirectToAction("Index");
            }
            else
            {
                AddToastMessage("", "Failed to delete.", ToastType.Error);
                return RedirectToAction("Index");
            }
        }


        [HttpGet]
        [AllowAnonymous]
        public ActionResult AllProductView()
        {
            var noticeAsync = _websiteProductService.GetProductAsync();
            var vmodel = _mapper.Map<IEnumerable<WebsiteProducts>, IEnumerable<WebsiteProductViewModels>>(noticeAsync);
            return View(vmodel);
        }


        [HttpGet]
        [AllowAnonymous]
        public ActionResult NavBarProduct() 
        {
            var noticeAsync = _websiteProductService.GetProductAsync();
            var vmodel = _mapper.Map<IEnumerable<WebsiteProducts>, IEnumerable<WebsiteProductViewModels>>(noticeAsync);
            return View(vmodel);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<JsonResult> CateGoryWiseProductView(int category)
        {            
            var allProductsAsync = await _websiteProductService.GetAllCategoryProductAsync(category);        
            return Json(allProductsAsync, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<JsonResult> GetAllProduct() 
        {
            var allProductsAsync = await _websiteProductService.GetAllProductAsync();         
            return Json(allProductsAsync, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        [AllowAnonymous]
        public ActionResult AllActiveProduct() 
        {
            var noticeAsync = _websiteProductService.GetProductAsync();
            var vmodel = _mapper.Map<IEnumerable<WebsiteProducts>, IEnumerable<WebsiteProductViewModels>>(noticeAsync);
            return View(vmodel);
        }


        [HttpGet]
        [AllowAnonymous]
        public ActionResult GetProductDetails(int Id)
        {
            var productDetails = _websiteProductService.GetById(Id);
            var vmodel = _mapper.Map<WebsiteProducts, WebsiteProductViewModels>(productDetails);
            ViewBag.Path = productDetails.DocumentPath;
            return View(vmodel);
        }


        #region Image Upload
        private List<VMDocInfo> UploadImage(HttpRequestBase httpRequest, string title)
        {
            List<VMDocInfo> fileInfos = new List<VMDocInfo>();
            string filePath = "";
            string fileName = "";
            if (httpRequest.Files.Count > 0)
            {
                for (int i = 0; i < httpRequest.Files.Count; i++)
                {
                    var file = Request.Files[i];
                    if (file != null && file.ContentLength > 0)
                    {
                        string fileExtension = Path.GetExtension(Request.Files["Photo"].FileName);
                        //if (fileExtension == ".png" || fileExtension == ".jpg" || fileExtension == ".jpeg")
                        //{
                        var fileExt = Path.GetExtension(file.FileName);
                        fileName = uploadFileName + "_" + title + "_" + Guid.NewGuid().ToString() + fileExt;
                        string serverPath = _photoPath + "/" + title;
                        bool exists = Directory.Exists(Server.MapPath(serverPath));
                        if (!exists)
                            Directory.CreateDirectory(Server.MapPath(serverPath));

                        var path = Path.Combine(Server.MapPath(serverPath), fileName);
                        file.SaveAs(path);
                        filePath = serverPath + "/" + fileName;

                        filePath = filePath.Replace("~", string.Empty);

                        fileInfos.Add(new VMDocInfo { DocName = fileName, DocPath = filePath });
                        //}
                    }
                }

            }
            return fileInfos;
        }
        #endregion

        #region Delete Prev Files
        public void DeletePrevFiles(int sourceId, bool isDelete = false)
        {
            if (!isDelete)
            {
                var hasFile = Request.Files[0];
                if (hasFile != null && hasFile.ContentLength > 0)
                {
                    bool exists = Directory.Exists(Server.MapPath(_photoPath));
                    if (exists)
                    {
                        List<DocumentsInfo> allDocs = _documentsInfoService.GetByDocTypeAndSource(EnumDocType.WebsiteProduct.ToString(), sourceId);
                        var NoticeID = _websiteProductService.GetById(sourceId);

                        string serverPath = _photoPath + "/" + NoticeID;

                        if (allDocs.Any())
                        {
                            foreach (var item in allDocs)
                            {
                                var fileName = String.IsNullOrEmpty(item.DocName) ? "" : item.DocName;
                                var filteredByFilename = Directory
                                        .GetFiles(Server.MapPath(serverPath))
                                        .Select(f => Path.GetFileName(f))
                                        .Where(f => f.Equals(fileName));

                                if (filteredByFilename != null)
                                {
                                    foreach (var filname in filteredByFilename)
                                    {
                                        var path = Path.Combine(Server.MapPath(serverPath), filname);
                                        if (System.IO.File.Exists(path))
                                        {
                                            System.IO.File.Delete(path);
                                        }
                                    }

                                }
                                _documentsInfoService.Delete(item.Id);
                            }
                            _documentsInfoService.Save();
                        }
                    }
                }
            }
            else
            {
                bool exists = Directory.Exists(Server.MapPath(_photoPath));
                if (exists)
                {
                    List<DocumentsInfo> allDocs = _documentsInfoService.GetByDocTypeAndSource(EnumDocType.WebsiteProduct.ToString(), sourceId);
                    //string customerCode = _customerService.GetCustomerCodeById(sourceId);



                    if (allDocs.Any())
                    {
                        string serverPath = _photoPath + "/" + allDocs.First().SourceFolder;
                        foreach (var item in allDocs)
                        {
                            var fileName = String.IsNullOrEmpty(item.DocName) ? "" : item.DocName;
                            var filteredByFilename = Directory
                                    .GetFiles(Server.MapPath(serverPath))
                                    .Select(f => Path.GetFileName(f))
                                    .Where(f => f.Equals(fileName));

                            if (filteredByFilename != null)
                            {
                                foreach (var filname in filteredByFilename)
                                {
                                    var path = Path.Combine(Server.MapPath(serverPath), filname);
                                    if (System.IO.File.Exists(path))
                                    {
                                        System.IO.File.Delete(path);
                                    }
                                }

                            }
                            _documentsInfoService.Delete(item.Id);
                        }
                        _documentsInfoService.Save();
                    }
                }
            }
        }
        #endregion

        [HttpGet]
        [Authorize]
        public ActionResult ViewDocs(int id)
        {
            var allProduct = _websiteProductService.GetById(id);
            var vmodel = _mapper.Map<WebsiteProducts, WebsiteProductViewModels>(allProduct);
            var docs = _documentsInfoService.GetByDocTypeAndSource(EnumDocType.WebsiteProduct.ToString(), allProduct.Id);
            var vmDocs = _mapper.Map<List<DocumentsInfo>, List<VMDocInfo>>(docs);
            vmodel.DocInfos = vmDocs;
            return View(vmodel);
        }

    }
}