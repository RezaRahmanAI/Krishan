using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IMSWEB.Model
{
    public partial class SystemInformation
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SystemInfoID { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [StringLength(255)]
        public string Address { get; set; }

        [StringLength(255)]
        public string TelephoneNo { get; set; }

        [StringLength(255)]
        public string EmailAddress { get; set; }

        [StringLength(255)]
        public string WebAddress { get; set; }

        public DateTime SystemStartDate { get; set; }

        [StringLength(150)]
        public string ProductPhotoPath { get; set; }

        [StringLength(150)]
        public string SupplierPhotoPath { get; set; }

        [StringLength(150)]
        public string CustomerPhotoPath { get; set; }

        [StringLength(150)]
        public string CustomerNIDPatht { get; set; }

        [StringLength(150)]
        public string SupplierDocPath { get; set; }

        [StringLength(250)]
        public string EmployeePhotoPath { get; set; }

        public int ConcernID { get; set; }
        public int WorkingDays { get; set; }
        public DateTime NextPayProcessDate { get; set; }
        public string BonusFormula { get; set; }
        public int CustomerDueLimitApply { get; set; }
        public int EmployeeDueLimitApply { get; set; }
        public System.TimeSpan OnDuty { get; set; }
        public System.TimeSpan OffDuty { get; set; }
        public string DeviceIP { get; set; }
        public string DeviceSerialNO { get; set; }
        public string APIKey { get; set; }
        public byte[] CompanyLogo { get; set; }
        public string LogoMimeType { get; set; }

        public int SMSServiceEnable { get; set; }
        public string InsuranceContactNo { get; set; }
        public int DaysBeforeSendSMS { get; set; }
        public int SMSProviderID { get; set; }
        public int SMSSendToOwner { get; set; }

        public virtual SisterConcern SisterConcern { get; set; }

        public static SystemInformation CurrentSystemInfo { get; set; }
        public DateTime? ExpireDate { get; set; }
        public string ExpireMessage { get; set; }
        public string WarningMsg { get; set; }

        public string SenderId { get; set; }
        public decimal smsCharge { get; set; }
        public int IsBanglaSmsEnable { get; set; }
        public string CompanyURL { get; set; }

        public int? IsRetailSMSEnable { get; set; }

        public int? IsHireSMSEnable { get; set; }

        public int? IsCashcollSMSEnable { get; set; }

        public int? IsInstallmentSMSEnable { get; set; }

        public int? IsRemindSMSEnable { get; set; }
        public int? EmployeeWiseTransactionEnable { get; set; }
        public int ApprovalSystemEnable { get; set; }
        public int CustomerSmsWithCustomerName { get; set; }

    }
}
