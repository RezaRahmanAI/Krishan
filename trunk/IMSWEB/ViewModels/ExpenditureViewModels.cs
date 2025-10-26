using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace IMSWEB
{
    public class CreateExpenditureViewModel : IValidatableObject
    {
        public string Id { get; set; }
        [Display(Name = "Entry Date")]
        public string EntryDate { get; set; }

      
        public string Purpose { get; set; }

        public string Amount { get; set; }

        [Display(Name = "Expense Item Name")]
        public string ExpenseItemID { get; set; }

        public string ConcernID { get; set; }
        [Display(Name = "Voucher No")]
        public string VoucherNo { get; set; }
        
        public string CreatedBy { get; set; }

        public string CreateDate { get; set; }

        public string ModifiedBy { get; set; }
       
        public string ModifiedDate { get; set; }

        [Display(Name = "Head")]
        public string ExpenseItemName { get; set; }

        
        [Display(Name = "Expense Item")]
        public ICollection<System.Web.Mvc.SelectListItem> ExpenseItems { get; set; }

        [Display(Name = "Employee")]
        public string EmployeeID { get; set; }
        public List<SelectListItem> PayItems { get; set; }
        [Display(Name = "Payment")]
        public string PayHeadId { get; set; }
        public int? PayCashAccountId { get; set; }
        public int? PayBankId { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var validator = new CreateExpenditureViewModelValidator();
            var result = validator.Validate(this);
            return result.Errors.Select(item => new ValidationResult(item.ErrorMessage, new[] { item.PropertyName }));
        }
    }
}