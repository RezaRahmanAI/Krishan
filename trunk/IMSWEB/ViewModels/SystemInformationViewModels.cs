using IMSWEB.Service;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IMSWEB
{
    public class CreateSystemInformationViewModel : IValidatableObject
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Address { get; set; }

        public string TelephoneNo { get; set; }

        public string EmailAddress { get; set; }

        public string WebAddress { get; set; }

        public DateTime SystemStartDate { get; set; }

        public string ProductPhotoPath { get; set; }

        public string SupplierPhotoPath { get; set; }

        public string CustomerPhotoPath { get; set; }

        public string CustomerNIDPatht { get; set; }

        public string SupplierDocPath { get; set; }

        public string EmployeePhotoPath { get; set; }

        public int ConcernID { get; set; }
        [Display(Name="Device IP")]
        public string DeviceIP { get; set; }

        [Display(Name = "Device SN")]
        public string DeviceSerialNO { get; set; }
        public string APIKey { get; set; }

        [Display(Name = "Logo")]
        public byte[] CompanyLogo { get; set; }
        public string LogoMimeType { get; set; }


        [Display(Name = "Enable SMS Service")]
        public bool SMSServiceEnable { get; set; }

        [Display(Name = "Owner Mobile")]
        public string InsuranceContactNo { get; set; }
        public int DaysBeforeSendSMS { get; set; }

        [Display(Name = "SMS Provider")]
        public EnumSMSServiceProvider SMSProviderID { get; set; }

        [Display(Name = "SMS Send To Owner")]
        public bool SMSSendToOwner { get; set; }

        public bool CustomerDueLimitApply { get; set; }
        public bool EmployeeDueLimitApply { get; set; }

        [Display(Name = "Expire Date")]
        public DateTime? ExpireDate { get; set; }

        [Display(Name = "Expire Message")]
        public string ExpireMessage { get; set; }
        public string WarningMsg { get; set; }

        [Display(Name = "Sms Cost")]
        public decimal SmsCharge { get; set; }

        public string SenderId { get; set; }
        [Display(Name = "URL Link")]
        public string CompanyURL { get; set; }
        [Display(Name = "Employee Wise Transaction Enable")]
        public bool EmployeeWiseTransactionEnable { get; set; }
        [Display(Name = "Is Approval Enable")]
        public bool ApprovalSystemEnable { get; set; }

        [Display(Name = "CustomerSmsWithCustomerName")]
        public bool CustomerSmsWithCustomerName { get; set; }



        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var validator = new CreateSystemInformationViewModelValidator();
            var result = validator.Validate(this);
            return result.Errors.Select(item => new ValidationResult(item.ErrorMessage, new[] { item.PropertyName }));
        }

    }
    public class CreateSMSSettingViewModel
    {
        public string Id { get; set; }


        [Display(Name = "Is Bangla SMS Service eEnable")]
        public bool IsBanglaSmsEnable { get; set; }

        [Display(Name = "Is Retail Sale SMS Service Applicable")]
        public bool RetailSaleSmsService { get; set; }

        [Display(Name = "Is Hire Sale SMS Service Applicable")]
        public bool HireSaleSmsService { get; set; }

        [Display(Name = "Is Cash Collection SMS Service Applicable")]
        public bool CashCollectionSmsService { get; set; }

        [Display(Name = "Is Installment SMS Service Applicable")]
        public bool InstallmentSmsService { get; set; }

        [Display(Name = "Is Remind Date SMS Service Applicable")]
        public bool RemindDateSmsService { get; set; }
    }



}