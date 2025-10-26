using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Model
{
    public class ProductionReportModel
    {
        public int ProductionID { get; set; }
        public DateTime ProductionDate { get; set; }
        public string ProductionCode { get; set; }
        public int FinProductID { get; set; }
        public string FinProductName { get; set; }
        public decimal FinQuantity { get; set; }
        public int RawProductID { get; set; }
        public string RawProductName { get; set; }
        public decimal RawPRate { get; set; }
        public decimal RawQuantity { get; set; }
        public decimal FinConvertValue { get; set; }
        public decimal RawConvertValue { get; set; }
        public decimal TotalCost { get; set; }
        public string FinChildUnitName { get; set; }
        public string FinParentUnitName { get; set; }
        public string RawChildUnitName { get; set; }
        public string RawParentUnitName { get; set; }
        public decimal RawPRateTotal { get; set; }

    }
}
