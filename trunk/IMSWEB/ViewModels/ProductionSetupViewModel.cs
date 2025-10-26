using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IMSWEB
{
    public class ProductionSetupViewModel
    {
        public ProductionSetupViewModel()
        {
            Details = new List<PSDetailViewModel>();
        }
        public int? PSID { get; set; }

        [Display(Name = "Fin. Goods")]
        public string FINProductID { get; set; }

        [Display(Name = "Fin. Goods")]
        public string ProductName { get; set; }

        [Display(Name = "Color")]
        public string ColorID { get; set; }

        [Display(Name = "Color")]
        public string ColorName { get; set; }

        public List<PSDetailViewModel> Details { get; set; }
    }

    public class PSDetailViewModel
    {
        public int? PSDID { get; set; }
        public int? PSID { get; set; }

        [Display(Name = "Raw Material")]
        public int RAWProductID { get; set; }

        [Display(Name = "Raw Material")]
        public string RAWProductName { get; set; }

        //[Range(1, double.MaxValue, ErrorMessage = "Raw Material quantity is required.")]

        [Display(Name = "Parent Qty")]
        public decimal RawParentQuantity { get; set; }


        [Display(Name = "Child Qty")]
        public decimal RAWChildQuantity { get; set; }

        [Display(Name = "Total Qty")]
        public decimal Quantity { get; set; }

        [Display(Name = "Color")]
        public string ColorID { get; set; }

        [Display(Name = "Color")]
        public string ColorName { get; set; }

        public bool IsSelected { get; set; }
        [Display(Name = "PCS/Carton")]
        public decimal ConvertValue { get; set; }
    }
}