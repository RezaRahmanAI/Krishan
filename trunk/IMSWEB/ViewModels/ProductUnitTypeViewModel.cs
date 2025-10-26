using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IMSWEB
{
    public class ProductUnitTypeViewModel
    {
        public int? ProUnitTypeID { get; set; }
        public string Description { get; set; }
        [Display(Name="Convert Value")]
        public decimal ConvertValue { get; set; }
        public int Status { get; set; }
        public int Position { get; set; }

        [Display(Name = "Unit Name")]
        public string UnitName { get; set; }
    }
}