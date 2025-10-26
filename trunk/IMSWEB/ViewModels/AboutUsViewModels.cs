using System;
using IMSWEB.Model;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IMSWEB
{
    public class AboutUsViewModels
    {
        public string Id { get; set; }
        public string ConcernID { get; set; }
        public string Title { get; set; }

        public string Description { get; set; }
        public Decimal Price { get; set; }
        public string DocumentPath { get; set; }
        public DateTime CreateDate { get; set; }

        public List<VMDocInfo> DocInfos { get; set; }

      
    }
}