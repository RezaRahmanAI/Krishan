using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IMSWEB
{
    public class GetCashCollectionViewModel
    {
        public string Id { get; set; }
        public string SOrderID { get; set; }

        [Display(Name = "Entry Date")]
        public string EntryDate { get; set; }

        [Display(Name = "Receipt No")]
        public string ReceiptNo { get; set; }

        [Display(Name = "A/C")]
        public string Code { get; set; }

        [Display(Name = "Name")]
        public string Name { get; set; }

        //[Display(Name = "Supplier Name")]
        //public string SupplierName { get; set; }

        [Display(Name = "AccountNo")]
        public string AccountNo { get; set; }

        [Display(Name = "Amount")]
        public string Amount { get; set; }
        public string CCAmount { get; set; }
        public string CCAdjustment { get; set; }

        [Display(Name = "InvoiceDue")]
        public string InvoiceDue { get; set; }

        [Display(Name = "Transaction Type")]
        public EnumTranType TransactionType { get; set; }
        public string Remarks { get; set; }
        [Display(Name = "Employee Name")]
        public string EmployeeName { get; set; }

        [Display(Name = "CashBonus %")]
        public string CashBPercentage { get; set; }
        [Display(Name = "CashBonus Amt.")]
        public string CashBAmt { get; set; }
        [Display(Name = "Yearly Bonus %")]
        public string YearlyBPercentage { get; set; }

        [Display(Name = "Yearly Amt.")]
        public string YearlyBnsAmt { get; set; }

    }

    public class CreateCashCollectionViewModel : GetCashCollectionViewModel, IValidatableObject
    {
        public string CashCollectionID { get; set; }

        [Display(Name = "Payment Type")]
        public EnumPayType PaymentType { get; set; }

        [Display(Name = "Bank Name")]
        public string BankName { get; set; }

        [Display(Name = "Branch Name")]
        public string BranchName { get; set; }

        public string InterestAmt { get; set; }

        //[Display(Name = "Entry Date")]
        //public string EntryDate { get; set; }
        public string RemindDate { get; set; }

        [Display(Name = "Send SMS ")]
        public bool IsSmsEnable { get; set; } = true;


        //[Display(Name = "Amount")]
        //public string Amount { get; set; }
        public string TempAmount { get; set; }

        [Display(Name = "Total Due")]
        public string CurrentDue { get; set; }
        [Display(Name = "Employee Total Due")]
        public string CurrentEmpDue { get; set; }
        [Display(Name = "Employee Due Amt.")]
        public string EmpDueBalance { get; set; }

        [Display(Name = "Adjustment")]
        public string AdjustAmt { get; set; }
        public string TempAdjustAmt { get; set; }

        [Display(Name = "Total Remaining Due")]
        public string BalanceDue { get; set; }

        [Display(Name = "Total Dis Amt")]
        public string TotalDisAmt { get; set; }

        [Display(Name = "Invoice Net Total")]
        public string InvNetTotal { get; set; }

        [Display(Name = "Invoice Due")]
        public string InvoiceDue { get; set; }

        [Display(Name = "Inv Remaining Due")]
        public string InvRemainingDue { get; set; }

        //[Display(Name = "AccountNo")]
        //public string AccountNo { get; set; }

        [Display(Name = "MBAccount No")]
        public string MBAccountNo { get; set; }

        [Display(Name = "BKashNo")]
        public string BKashNo { get; set; }

        //[Display(Name = "Transaction Type")]
        //public EnumTranType TransactionType { get; set; }

        [Display(Name = "Customer Name")]
        public string CustomerID { get; set; }
        
        [Display(Name = "Invoice No")]
        public string InvoiceNo { get; set; }

        [Display(Name = "Supplier Name")]
        public string SupplierID { get; set; }
        [Display(Name = "Bank Name")]
        public string CCBankID { get; set; }

        //[Display(Name = "Receipt No")]
        //public string ReceiptNo { get; set; }

        public string ConcernID { get; set; }

        public string CreatedBy { get; set; }

        public string CreateDate { get; set; }

        public string ModifiedBy { get; set; }

        public string ModifiedDate { get; set; }

        [Display(Name = "Offer Amt")]
        public string OfferAmt { get; set; }
        [Display(Name = "Bonus Amt")]
        public string BonusAmt { get; set; }

        [Display(Name = "Trans. Type")]
        [Required(ErrorMessage = "Trans Type is required.")]
        public EnumDropdownTranType Type { get; set; }
        [Display(Name = "Employee")]
        public int? EmployeeID { get; set; }

        [Display(Name = "Employee")]
        public string EmpName { get; set; }
        public List<SelectListItem> PayItems { get; set; }
        [Display(Name = "Payment")]
        public string PayHeadId { get; set; }
        public int? PayCashAccountId { get; set; }
        public int? PayBankId { get; set; }

        public ICollection<System.Web.Mvc.SelectListItem> CustomerItems { get; set; }

        public ICollection<System.Web.Mvc.SelectListItem> SupplierItems { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var validator = new CreateCashCollectionViewModelValidator();
            var result = validator.Validate(this);
            return result.Errors.Select(item => new ValidationResult(item.ErrorMessage, new[] { item.PropertyName }));
        }


    }
}