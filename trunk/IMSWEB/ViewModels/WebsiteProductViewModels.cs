using System;
using IMSWEB.Model;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IMSWEB
{
    public class WebsiteProductViewModels
    {
        public string Id { get; set; }
        public EnumProcutCategory ProcutCategory { get; set; }
        public string ConcernID { get; set; }
        public string ConcernName { get; set; }
        public string Title { get; set; }

        public string Description { get; set; }
        public decimal Price { get; set; }
        public string DocumentPath { get; set; }
        public DateTime CreateDate { get; set; }

        public List<VMDocInfo> DocInfos { get; set; }

        public string CategoryDisplay
        {
            get
            {
                return Enum.IsDefined(typeof(EnumProcutCategory), ProcutCategory)
                    ? ProcutCategory.ToString().Replace("_", " ")
                    : "Featured";
            }
        }

        public string ConcernDisplay
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(ConcernName))
                {
                    return ConcernName;
                }

                return !string.IsNullOrWhiteSpace(ConcernID)
                    ? $"Concern #{ConcernID}"
                    : "General";
            }
        }

        public string FormattedPrice
        {
            get { return Price > 0 ? Price.ToString("N0") : "On request"; }
        }
    }
}