using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Model
{
    public class ProductDetailsModel
    {
        public int ProductId { get; set; }
        public int StockDetailsId { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public int ColorId { get; set; }
        public string ColorName { get; set; }
        public string PicturePath { get; set; }
        public string CategoryName { get; set; }
        public string ModelName { get; set; }
        public string CompanyName { get; set; }
        public decimal PWDiscount { get; set; }
        public decimal PreStock { get; set; }
        public string IMENo { get; set; }
        public decimal MRPRate { get; set; }
        public decimal CashSalesRate { get; set; }
        public string OfferDescription { get; set; }
        public int ProductType { get; set; }
        public int CompressorWarrentyMonth { get; set; }
        public int PanelWarrentyMonth { get; set; }
        public int MotorWarrentyMonth { get; set; }
        public int SparePartsWarrentyMonth { get; set; }
        public int ServiceWarrentyMonth { get; set; }
        public decimal SalesQty { get; set; }

        public decimal ActualSFT { get; set; }

        public decimal TotalSFT { get; set; }

        public decimal SFTRate { get; set; }

        public string GodownName { get; set; }

        public int GodownID { get; set; }

        public int Status { get; set; }

        public int SDetailID { get; set; }
        public string ChildUnitName { get; set; }

        public string ParentUnitName { get; set; }
        public decimal ConvertValue { get; set; }
        public decimal BundleQty { get; set; }
        public decimal RawConvertValue { get; set; }
        public int UnitType { get; set; }
    }
}
