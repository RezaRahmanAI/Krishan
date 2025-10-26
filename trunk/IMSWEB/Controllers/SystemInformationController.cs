using AutoMapper;
using IMSWEB.Model;
using IMSWEB.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace IMSWEB.Controllers
{
    [Authorize]
    [RoutePrefix("SystemInformation")]
    public class SystemInformationController : CoreController
    {
        ISystemInformationService _systemInformationService;
        IMapper _mapper;

        public SystemInformationController(IErrorService errorService,
            ISystemInformationService systemInformationService, IMapper mapper)
            : base(errorService)
        {
            _systemInformationService = systemInformationService;
            _mapper = mapper;
        }

        [HttpGet]
        [Authorize]
        [Route("index")]
        public ActionResult Index()
        {
            var systemInformation = _systemInformationService.GetSystemInformationByConcernId(User.Identity.GetConcernId());
            var vmodel = _mapper.Map<SystemInformation, CreateSystemInformationViewModel>(systemInformation);
            return View(vmodel);
        }
        [HttpPost]
        [Authorize]
        [Route("edit/returnUrl")]
        public ActionResult Edit(CreateSystemInformationViewModel systemInformation, string returnUrl, HttpPostedFileBase image)
        {
            if (!ModelState.IsValid)
                return View("Index", systemInformation);

            if (systemInformation != null)
            {
                var sysinfo = _systemInformationService.GetSystemInformationById(int.Parse(systemInformation.Id));
                sysinfo.Address = systemInformation.Address;
                sysinfo.ConcernID = User.Identity.GetConcernId();
                sysinfo.CustomerNIDPatht = systemInformation.CustomerNIDPatht;
                sysinfo.CustomerPhotoPath = systemInformation.CustomerPhotoPath;
                sysinfo.EmailAddress = systemInformation.EmailAddress;
                sysinfo.EmployeePhotoPath = systemInformation.EmployeePhotoPath;
                sysinfo.Name = systemInformation.Name;
                sysinfo.ProductPhotoPath = systemInformation.ProductPhotoPath;
                sysinfo.SupplierDocPath = systemInformation.SupplierDocPath;
                sysinfo.SupplierPhotoPath = systemInformation.SupplierPhotoPath;
                sysinfo.SystemStartDate = systemInformation.SystemStartDate;
                sysinfo.TelephoneNo = systemInformation.TelephoneNo;
                sysinfo.WebAddress = systemInformation.WebAddress;
                sysinfo.APIKey = systemInformation.APIKey;
                sysinfo.DeviceIP = systemInformation.DeviceIP;
                sysinfo.DeviceSerialNO = systemInformation.DeviceSerialNO;
                sysinfo.SenderId = systemInformation.SenderId;
                sysinfo.CompanyURL = systemInformation.CompanyURL;
                sysinfo.CustomerSmsWithCustomerName = systemInformation.CustomerSmsWithCustomerName ? 1 : 0;
                sysinfo.smsCharge = systemInformation.SmsCharge;
                sysinfo.SMSServiceEnable = systemInformation.SMSServiceEnable ? 1 : 0;
                sysinfo.CustomerDueLimitApply = systemInformation.CustomerDueLimitApply ? 1 : 0;
                sysinfo.EmployeeDueLimitApply = systemInformation.EmployeeDueLimitApply ? 1 : 0;
                sysinfo.SMSProviderID = (int)systemInformation.SMSProviderID;
                sysinfo.DaysBeforeSendSMS = systemInformation.DaysBeforeSendSMS;
                sysinfo.InsuranceContactNo = systemInformation.InsuranceContactNo;
                sysinfo.SMSSendToOwner = systemInformation.SMSSendToOwner ? 1 : 0;
                sysinfo.ExpireDate = systemInformation.ExpireDate;
                sysinfo.ExpireMessage = systemInformation.ExpireMessage;
                sysinfo.WarningMsg = systemInformation.WarningMsg;
                sysinfo.EmployeeWiseTransactionEnable = systemInformation.EmployeeWiseTransactionEnable ? 1 : 0;
                sysinfo.ApprovalSystemEnable = systemInformation.ApprovalSystemEnable ? 1 : 0;
                if (image!=null)
                {
                string[] sAllowedExt = new string[] { ".jpg", ".jpeg", ".gif", ".png" };
                if (image.ContentLength > 0 && sAllowedExt.Contains(image.FileName.Substring(image.FileName.LastIndexOf('.')).ToLower()))
                {
                    sysinfo.LogoMimeType = image.ContentType;
                    sysinfo.CompanyLogo = new byte[image.ContentLength];
                    image.InputStream.Read(sysinfo.CompanyLogo, 0, image.ContentLength);
                }
                else
                    AddToastMessage("", "Logo type is not supported. Supported types are .jpg, .jpeg, .gif, .png", ToastType.Error);
                }

                _systemInformationService.UpdateSystemInformation(sysinfo);
                _systemInformationService.SaveSystemInformation();
                AddToastMessage("", "Item has been updated successfully.", ToastType.Success);
                return RedirectToAction("Index");
            }
            else
            {
                AddToastMessage("", "No Item data found to update.", ToastType.Error);
                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        public JsonResult GenerateSHA256String()
        {
            return Json(GenerateCoupon(), JsonRequestBehavior.AllowGet);
        }
        public string GenerateCoupon()
        {
            int length = 50;
            Random random = new Random();
            string characters = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
            StringBuilder result = new StringBuilder(length);
            for (int i = 0; i < length; i++)
            {
                result.Append(characters[random.Next(characters.Length)]);
            }
            return result.ToString();
        }

        [HttpGet]
        public FileContentResult GetImage(int Id)
        {
            var SysInfo = _systemInformationService.GetSystemInformationById(Id);
            if (SysInfo != null && SysInfo.CompanyLogo != null && !string.IsNullOrEmpty(SysInfo.LogoMimeType))
            {
                return File(SysInfo.CompanyLogo, SysInfo.LogoMimeType);
            }
            else
            {
                return null;
            }
        }
    }
}