using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IMSWEB
{
    public class EmployeeTargetSetupViewModel
    {
        public int? ETSID { get; set; }

        [Display(Name = "Target Amount")]
        public decimal TargetAmt { get; set; }

        [Display(Name = "Amount To")]
        public decimal AmtTo { get; set; }

        [Display(Name = "Commission Amount")]
        public decimal Commission { get; set; }

        [Display(Name = "Target Month")]
        [Required(ErrorMessage = "Target Month is required")]
        public DateTime TargetMonth { get; set; }

        [Display(Name = "Employee")]
        public int EmployeeID { get; set; }

        [Display(Name = "Employee")]
        public string EmployeeName { get; set; }
        public List<GetEmployeeViewModel> Employees { get; set; }
    }
}