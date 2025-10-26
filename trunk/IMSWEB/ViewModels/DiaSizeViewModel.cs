using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IMSWEB
{
    public class DiaSizeViewModel
    {
        public int? DiaSizeID { get; set; }
        public string Code { get; set; }
        [Display(Name = "Dia. Size")]
        public string Description { get; set; }
    }
}