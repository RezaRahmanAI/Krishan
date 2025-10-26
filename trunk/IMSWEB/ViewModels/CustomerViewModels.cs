using IMSWEB.Model;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace IMSWEB
{
    public class GetCustomerViewModel
    {
        public string Id { get; set; }

        public string Code { get; set; }

        [Display(Name = "Customer Name")]
        public string Name { get; set; }

        [Display(Name = "Contact No.")]
        public string ContactNo { get; set; }

        [Display(Name = "Picture")]
        public string PhotoPath { get; set; }

        [Display(Name = "Total Due")]
        public string TotalDue { get; set; }

        [Display(Name = "Customer Type")]
        public EnumCustomerType CustomerType { get; set; }

        [Display(Name = "Due Limit")]
        public string CusDueLimit { get; set; }
        public string Address { get; set; }

        [Display(Name = "Company")]
        public string CompanyName { get; set; }
        [Display(Name = "Employee Wise Due")]
        public decimal EmployeeWiseDue { get; set; }

        public int EmpID { get; set; }

        //[Display(Name = "Customer Type")]
        //public EnumCustomerType CustomerType { get; set; }
    }

    public class CreateCustomerViewModel : GetCustomerViewModel, IValidatableObject
    {
        public CreateCustomerViewModel()
        {
            CustomerDues = new List<EmployeewiseCustomerDueViewModel>();
        }
        [Display(Name = "Father Name")]
        public string FName { get; set; }

        [Display(Name = "Proprietor")]
        public string CompanyId { get; set; }

        [Display(Name = "Opening Due")]
        public string OpeningDue { get; set; }

        [Display(Name = "Email")]
        public string EmailId { get; set; }

        [Display(Name = "National Id")]
        public string NId { get; set; }

        [Display(Name = "Ref. Name")]
        public string RefName { get; set; }

        [Display(Name = "Ref. Contact No.")]
        public string RefContact { get; set; }

        [Display(Name = "Ref. Father Name")]
        public string RefFName { get; set; }

        [Display(Name = "Ref. Address")]
        public string RefAddress { get; set; }

        public string Remarks { get; set; }

        [Display(Name = "Employee")]
        public string EmployeeId { get; set; }

        [Display(Name = "Product")]
        public string ProductDetailsId { get; set; }


        [Display(Name = "Credit Due")]
        public decimal CreditDue { get; set; }

        [Display(Name = "Concern")]
        public string ConcernId { get; set; }

        [Display(Name = "Department")]
        public string DepartmentID { get; set; }
        [Display(Name = "Territory")]
        public string TerritoryID { get; set; }

        [Display(Name = "Employee Name")]
        public string EmployeeName { get; set; }

        [Display(Name = "Employee")]
        public int DueEmployeeID { get; set; } // Due employee
        public List<GetEmployeeViewModel> Employees { get; set; }
        public List<EmployeewiseCustomerDueViewModel> CustomerDues { get; set; }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var validator = new CreateCustomerViewModelValidator();
            var result = validator.Validate(this);
            return result.Errors.Select(item => new ValidationResult(item.ErrorMessage, new[] { item.PropertyName }));
        }
    }
}