using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using IMSWEB.Model;

namespace IMSWEB
{
    public class CashAccountVM
    {
        public CashAccountVM()
        {
        }
        public int Id { get; set; }
        [StringLength(250)]
        public string Name { get; set; }
        [Display(Name = "Opening Balance")]
        public decimal OpeningBalance { get; set; }
        [Display(Name = "Opening Date")]
        public DateTime OpeningDate { get; set; }
        public int ConcernID { get; set; }


    }
}