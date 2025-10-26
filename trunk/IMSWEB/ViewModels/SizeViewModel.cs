using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IMSWEB
{
    public class SizeViewModel
    {
        public int? SizeID { get; set; }
        public string Code { get; set; }
        [Display(Name="Size")]
        public string Description { get; set; }
    }
}