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

    [RoutePrefix("AboutUs")]
    public class AboutUsController : CoreController
    {
        IAboutUSService _AboutUSService;
        IMapper _mapper;
        ISisterConcernService _SisterConcern;       
        IUserService _UserService;

        ISystemInformationService _SysInfoService;
        string _photoPath = "~/Content/Document/About";
        IDocumentsInfoService _documentsInfoService;
        IMiscellaneousService<AboutUS> _miscellaneousService;
        private readonly string uploadFileName = "about_";

        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public AboutUsController(IErrorService errorService,                   
            IMiscellaneousService<AboutUS> miscellaneousService, IMapper mapper,ISisterConcernService sisterConcern, IAboutUSService aboutUSService, 
            IUserService UserService,ISystemInformationService SysInfoService, IDocumentsInfoService documentsInfoService)
            : base(errorService)
        {
            _miscellaneousService = miscellaneousService;
            _mapper = mapper;
            _SisterConcern = sisterConcern;            
            _UserService = UserService;
            _SysInfoService = SysInfoService;
            _documentsInfoService = documentsInfoService;
            _AboutUSService = aboutUSService;
        }


        [HttpGet]
        [Route("index")]
        public async Task<ActionResult> Index()
        {
            var productAsync = _AboutUSService.GetAllAsync();
            var vmodel = _mapper.Map<IEnumerable<AboutUS>, IEnumerable<AboutUsViewModels>>(await productAsync);
            return View(vmodel);
        }


        [HttpGet]
        [Authorize]
        [Route("create")]
        public ActionResult Create()
        {
            return View(new AboutUsViewModels());
        }

        [HttpPost]
        [Authorize]
        [Route("create/returnUrl")]
        public ActionResult Create(AboutUsViewModels newAboutUs, FormCollection formCollection,
          HttpPostedFileBase photo, string returnUrl)
        {
            //CheckAndAddModelError(newNotice, formCollection);
            if (!ModelState.IsValid)
            {

                return View(newAboutUs);
            }

            if (newAboutUs != null)
            {
                var aboutUs = _mapper.Map<AboutUsViewModels, AboutUS>(newAboutUs);
                aboutUs.ConcernID = User.Identity.GetConcernId();
                AddAuditTrail(aboutUs, true);
                _AboutUSService.Add(aboutUs);
                _AboutUSService.SaveAboutUs();

                #region save docs
                List<VMDocInfo> docDetails = UploadImage(Request, aboutUs.Id.ToString());
                string folderPath = string.Empty;
                //newCustomer.PhotoPath = docDetails.DocPath;
                foreach (var docInfo in docDetails)
                {
                    folderPath = docInfo.DocPath;
                    DocumentsInfo docData = new DocumentsInfo
                    {
                        DocName = docInfo.DocName,
                        DocPath = docInfo.DocPath,
                        DocType = EnumDocType.AboutUs.ToString(),
                        DocSourceId = aboutUs.Id,
                        SourceFolder = aboutUs.Id.ToString(),
                        ConcernID = User.Identity.GetConcernId()
                    };
                    _documentsInfoService.Add(docData);
                }

                if (_documentsInfoService.Save())
                {
                    AboutUS oldAboutUs = _AboutUSService.GetById(aboutUs.Id);
                    oldAboutUs.DocumentPath = folderPath;
                    _AboutUSService.Update(oldAboutUs);
                    _AboutUSService.SaveAboutUs();
                }

                #endregion

                AddToastMessage("", "About has been Published successfully.", ToastType.Success);
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
            var aboutUs = _AboutUSService.GetById(id);
            var vmodel = _mapper.Map<AboutUS, AboutUsViewModels>(aboutUs);
            return View("Create", vmodel);
        }

        [HttpPost]
        [Authorize]
        [Route("edit/returnUrl")]
        public ActionResult Edit(AboutUS aboutUs, FormCollection formCollection)
        {
            if (!ModelState.IsValid)
                return View("Create", aboutUs);
            if (aboutUs != null)
            {
                var oldAboutUs = _AboutUSService.GetById(aboutUs.Id);

                oldAboutUs.Title = aboutUs.Title;
                oldAboutUs.Description = aboutUs.Description;
                oldAboutUs.DocumentPath = aboutUs.DocumentPath;  
                AddAuditTrail(oldAboutUs, false);

                _AboutUSService.Update(oldAboutUs);
                DeletePrevFiles(oldAboutUs.Id, true);

                #region save docs
                List<VMDocInfo> docDetails = UploadImage(Request, oldAboutUs.Id.ToString());
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
                        DocSourceId = oldAboutUs.Id,
                        SourceFolder = oldAboutUs.Id.ToString(),
                        ConcernID = User.Identity.GetConcernId()
                    };
                    _documentsInfoService.Add(docData);
                }

                if (_documentsInfoService.Save())
                {
                    AboutUS oldAbouts = _AboutUSService.GetById(oldAboutUs.Id);
                    oldAbouts.DocumentPath = folderPath;
                    _AboutUSService.Update(oldAbouts);
                    _AboutUSService.Save();
                }

                #endregion
                if (_AboutUSService.Save())
                {
                    AddToastMessage("", "About has been updated successfully.", ToastType.Success);
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
            _AboutUSService.Delete(id);

            if (_AboutUSService.Save())
            {
                DeletePrevFiles(id, true);
                AddToastMessage("", "About has been deleted successfully.", ToastType.Success);
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
        public ActionResult AboutUs()
        {           
            return View();
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
                        List<DocumentsInfo> allDocs = _documentsInfoService.GetByDocTypeAndSource(EnumDocType.AboutUs.ToString(), sourceId);
                        var NoticeID = _AboutUSService.GetById(sourceId);

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
                    List<DocumentsInfo> allDocs = _documentsInfoService.GetByDocTypeAndSource(EnumDocType.AboutUs.ToString(), sourceId);
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
            var allProduct = _AboutUSService.GetById(id);
            var vmodel = _mapper.Map<AboutUS, AboutUsViewModels>(allProduct);
            var docs = _documentsInfoService.GetByDocTypeAndSource(EnumDocType.WebsiteProduct.ToString(), allProduct.Id);
            var vmDocs = _mapper.Map<List<DocumentsInfo>, List<VMDocInfo>>(docs);
            vmodel.DocInfos = vmDocs;
            return View(vmodel);
        }

    }
}