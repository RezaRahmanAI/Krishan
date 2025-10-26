using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IMSWEB
{
    public class EmployeewiseCustomerDueViewModel
    {
        public int? ID { get; set; }

        [Display(Name = "Employee")]
        public string EmployeeName { get; set; }
        public string EmployeeContactNo { get; set; }
        public string EmployeeDesignation { get; set; }

        [Display(Name = "Employee")]
        public int EmployeeID { get; set; }

        [Display(Name = "Customer")]
        public string CustomerName { get; set; }
        public string CustomerAddress { get; set; }
        public string CustomerContactNo { get; set; }

        [Display(Name = "Customer")]
        public int CustomerID { get; set; }

        [Display(Name = "Customer Due")]
        public decimal CustomerDue { get; set; }

        [Display(Name = "Customer Opening Due")]
        public decimal CustomerOpeningDue { get; set; }
    }
}