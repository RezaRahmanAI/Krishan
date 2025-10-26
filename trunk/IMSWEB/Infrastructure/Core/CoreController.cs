using IMSWEB.Model;
using IMSWEB.Service;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace IMSWEB
{
    public class CoreController : Controller
    {
        #region Variables
        protected readonly IErrorService _errorService;
        #endregion

        #region Constructor
        public CoreController(IErrorService errorService)
        {
            _errorService = errorService;
        }
        #endregion

        #region Methods

        #region OnException
        protected override void OnException(ExceptionContext filterContext)
        {

            LogError(filterContext.Exception);

            if (filterContext.ExceptionHandled)

                filterContext.Result = new ViewResult
                {
                    ViewName = "~/Views/Shared/Error.cshtml"
                };

            filterContext.ExceptionHandled = true;
        }
        #endregion

        #region LogError
        private void LogError(Exception ex)
        {
            try
            {
                Error error = new Error()
                {
                    Message = new string(ex.Message.Take(4000).ToArray()),
                    StackTrace = new string(ex.StackTrace.Take(4000).ToArray()),
                    CreatedBy = User.Identity.GetUserId<int>(),
                    CreateDate = DateTime.Now
                };

                _errorService.AddError(error);
                _errorService.SaveError();
            }
            catch (Exception exp)
            {
            }
        }
        #endregion

        #region Helpers Method

        public string GetDefaultIfNull(string obj)
        {
            return string.IsNullOrEmpty(obj) ? "0" : obj;
        }

        public void AddToastMessage(string title = "", string message = "", ToastType toastType = ToastType.Info)
        {
            Toastr toastr = TempData["Toastr"] as Toastr;
            toastr = toastr ?? new Toastr();
            toastr.AddToastMessage(title, message, toastType);
            TempData["Toastr"] = toastr;
        }

        public ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        public string SaveHttpPostedImageFile(string fileName, string path, HttpPostedFileBase file)
        {
            string[] sAllowedExt = new string[] { ".jpg", ".jpeg", ".gif", ".png" };
            if (file != null && file.ContentLength > 0 && sAllowedExt.Contains(file.FileName.Substring(file.FileName.LastIndexOf('.')).ToLower()))
            {
                string extension = Path.GetExtension(file.FileName);
                var fullFileName = fileName + extension;
                file.SaveAs(Path.Combine(path, fullFileName));

                return fullFileName;
            }
            else
            {
                return "Error";
            }
        }

        #endregion

        #endregion

        public static string GetUniqueBarCode(int maxSize)
        {
            char[] chars = new char[62];
            chars = "123456789".ToCharArray();
            byte[] data = new byte[1];
            RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider();
            crypto.GetNonZeroBytes(data);
            data = new byte[maxSize];
            crypto.GetNonZeroBytes(data);
            StringBuilder result = new StringBuilder(maxSize);
            foreach (byte b in data)
            {
                result.Append(chars[b % (chars.Length)]);
            }
            return result.ToString();
        }

        public Tuple<DateTime, DateTime> GetFirstAndLastDateOfMonth(DateTime date)
        {
            DateTime firstDate = new DateTime(date.Year, date.Month, 1);
            DateTime lastDate = firstDate.AddMonths(1).AddDays(-1);
            DateTime ld = new DateTime(lastDate.Year, lastDate.Month, lastDate.Day, 23, 59, 59);
            return new Tuple<DateTime, DateTime>(firstDate, ld);
        }

        public Tuple<DateTime, DateTime> GetFirstAndLastDateOfYear(int Year)
        {
            DateTime firstDate = new DateTime(Year, 1, 1);
            DateTime lastDate = firstDate.AddYears(1).AddDays(-1);
            return new Tuple<DateTime, DateTime>(firstDate, lastDate);
        }

        public DateTime GetLocalDateTime()
        {
            return DateTime.Now.AddHours(10);
        }

        public void AddAuditTrail(dynamic obj, bool IsNew)
        {
            if (IsNew)
            {
                if (obj.GetType().GetProperty("CreatedBy") != null && obj.GetType().GetProperty("CreateDate") != null)
                {
                    obj.CreatedBy = User.Identity.GetUserId<int>();
                    obj.CreateDate = GetLocalDateTime();
                }
            }
            else
            {
                if (obj.GetType().GetProperty("ModifiedBy") != null && obj.GetType().GetProperty("ModifiedDate") != null)
                {
                    obj.ModifiedBy = User.Identity.GetUserId<int>();
                    obj.ModifiedDate = GetLocalDateTime();
                }
            }

            if (obj.GetType().GetProperty("ConcernID") != null)
                obj.ConcernID = User.Identity.GetConcernId();
            else if (obj.GetType().GetProperty("ConcernId") != null)
                obj.ConcernId = User.Identity.GetConcernId();
            //if (IsNew)
            //{
            //    if (obj.GetType().GetProperty("CreatedBy") != null && obj.GetType().GetProperty("CreateDate") != null)
            //    {
            //        obj.CreatedBy = User.Identity.GetUserId<int>();
            //        obj.CreateDate = GetLocalDateTime();
            //    }

            //    if (obj.GetType().GetProperty("ConcernID") != null)
            //        obj.ConcernID = User.Identity.GetConcernId();
            //}
            //else
            //{
            //    if (obj.GetType().GetProperty("ModifiedBy") != null && obj.GetType().GetProperty("ModifiedDate") != null)
            //    {
            //        obj.ModifiedBy = User.Identity.GetUserId<int>();
            //        obj.ModifiedDate = GetLocalDateTime();
            //    }
            //}

        }

        public bool IsDateValid(DateTime dateTime)
        {
            //if (User.IsInRole(EnumUserRoles.Admin.ToString()) || User.IsInRole(EnumUserRoles.LocalAdmin.ToString()) || User.IsInRole(EnumUserRoles.Manager.ToString()))
            //    return true;

            //if (dateTime < DateTime.Today)
            //{
            //    AddToastMessage("", "Back dated entry,update or delete are not valid. Please contact Object Canvas Technology.", ToastType.Error);
            //    return false;
            //}

            return true;
        }
        public bool IsVATManager()
        {
            bool IsVATManager = false;
            if (User.IsInRole(EnumUserRoles.VATManager.ToString()))
                IsVATManager = true;

            return IsVATManager;
        }
        public bool IsDateValidForReport(DateTime dateTime)
        {
            if (
                // User.IsInRole(EnumUserRoles.Admin.ToString())
                //User.IsInRole(EnumUserRoles.MobileUser.ToString())
                //|| User.IsInRole(EnumUserRoles.Manager.ToString())
                User.IsInRole(EnumUserRoles.MobileUser.ToString())
               )
            {
                if (dateTime < DateTime.Today)
                {
                    // AddToastMessage("", "Back dated entry,update or delete are not permitted. Please contact with Object Canvas Technology.", ToastType.Error);
                    return false;
                }
                else
                {
                    return true;
                }
            }




            return true;
        }


        public static string RemoveSpecialCharacters(string input)
        {
            // Replace any character that is not a letter or digit with an empty string
            return Regex.Replace(input, "[^a-zA-Z0-9]", " ");
        }
    }
}