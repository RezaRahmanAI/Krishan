using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using IMSWEB.Model;

namespace IMSWEB
{
    public class CreateProductViewModel : IValidatableObject
    {
        public CreateProductViewModel()
        {
            ProductUnitTypes = new List<ProductUnitType>();
        }
        public string ProductId { get; set; }

        [Display(Name = "Code")]
        public string Code { get; set; }

        [Display(Name = "Identity Code")]
        public string IDCode { get; set; }

        [Display(Name = "Name")]
        public string ProductName { get; set; }

        [Display(Name = "Picture")]
        public string PicturePath { get; set; }

        [Display(Name = "Category")]
        public string CategoryId { get; set; }

        public string CategoryName { get; set; }

        [Display(Name = "Company")]
        public string CompanyId { get; set; }

        public string CompanyName { get; set; }

        [Display(Name = "Sizes")]
        public int SizeID { get; set; }
        public string SizeName { get; set; }

        [Display(Name = "Dia. Sizes")]
        public int DiaSizeID { get; set; }
        public string DiaSizeName { get; set; }
        //public string SizeName { get; set; }




        [Display(Name = "Unit Type")]
        public int UnitType { get; set; }

        [Display(Name = "Discount")]
        public string PWDiscount { get; set; }

        [Display(Name = "Discount From")]
        public DateTime? DisDurationFDate { get; set; }

        [Display(Name = "Discount To")]
        public DateTime? DisDurationToDate { get; set; }
        [Display(Name = "Compressor Warrenty")]
        public string CompressorWarrentyMonth { get; set; }
        [Display(Name = "Panel Warrenty")]
        public string PanelWarrentyMonth { get; set; }
        [Display(Name = "Motor Warrenty")]
        public string MotorWarrentyMonth { get; set; }
        [Display(Name = "SpareParts Warrenty")]
        public string SparePartsWarrentyMonth { get; set; }
        [Display(Name = "Service Warrenty")]
        public string ServiceWarrentyMonth { get; set; }
        [Display(Name = "Product Type")]
        public string Compressor { get; set; }
        public string Motor { get; set; }
        public string Panel { get; set; }
        public string Service { get; set; }
        public string SpareParts { get; set; }

        [Display(Name = "Product Type")]
        public EnumProductType ProductType { get; set; }

        [Display(Name = "Carton Qty")]
        public decimal BundleQty { get; set; }

        [Display(Name = "Sales Rate")]
        public decimal MRP { get; set; }
        [Display(Name = "Cost Price")]
        public decimal DP { get; set; }

        [Display(Name = "P. Sqft/Carton")]
        [DisplayFormat(DataFormatString = "{0:0.000000}", ApplyFormatInEditMode = true)]
        public decimal PurchaseCSft { get; set; }

        [Display(Name = "S. Sqft/Carton")]
       [DisplayFormat(DataFormatString = "{0:0.000000}", ApplyFormatInEditMode = true)]
        public decimal SalesCSft { get; set; }
        public List<ProductUnitType> ProductUnitTypes { get; set; }

        public int ProUnitTypeID { get; set; }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var validator = new CreateProductViewModelValidator();
            var result = validator.Validate(this);
            return result.Errors.Select(item => new ValidationResult(item.ErrorMessage, new[] { item.PropertyName }));
        }
    }

    public class GetProductViewModel
    {
        public int ProductId { get; set; }
        public int CategoryID { get; set; }
        public int StockDetailsId { get; set; }

        [Display(Name = "Code")]
        public string ProductCode { get; set; }

        [Display(Name = "Name")]
        public string ProductName { get; set; }



        public int ColorId { get; set; }

        [Display(Name = "Color")]
        public string ColorName { get; set; }

        [Display(Name = "Picture")]
        public string PicturePath { get; set; }

        [Display(Name = "Category")]
        public string CategoryName { get; set; }

        [Display(Name = "Model")]
        public string ModelName { get; set; }

        [Display(Name = "Company")]



        public string CompanyName { get; set; }

        [Display(Name = "Discount")]
        public decimal PWDiscount { get; set; }

        [Display(Name = "Pre. Stock")]
        public decimal PreStock { get; set; }

        [Display(Name = "IMENo/Barcode")]
        public string IMENo { get; set; }

        [Display(Name = "Sales Rate")]
        public decimal MRPRate { get; set; }
        [Display(Name = "MRP Rate 12")]
        public decimal MRPRate12 { get; set; }
        [Display(Name = "Cost Price")]
        public decimal DP { get; set; }



        public decimal CashSalesRate { get; set; }

        public string OfferDescription { get; set; }

        public int ProductType { get; set; }
        [Display(Name = "Compressor Warrenty(Month)")]
        public string CompressorWarrentyMonth { get; set; }
        [Display(Name = "Panel Warrenty(Month)")]
        public string PanelWarrentyMonth { get; set; }
        [Display(Name = "Motor Warrenty(Month)")]



        public string MotorWarrentyMonth { get; set; }
        [Display(Name = "SpareParts Warrenty(Month)")]
        public string SparePartsWarrentyMonth { get; set; }

        [Display(Name = "Service Warrenty(Month)")]
        public string ServiceWarrentyMonth { get; set; }
        public bool IsSelect { get; set; }
        public EnumStatus Status { get; set; }


        public int GodownID { get; set; }
        public decimal Quantity { get; set; }
        public string GodownName { get; set; }

        public int ProUnitTypeID { get; set; }
        public int SizeID { get; set; }
        [Display(Name = "Size")]
        public string SizeName { get; set; }
        public string ParentUnit { get; set; }
        public decimal ConvertValue { get; set; }
        public string ChildUnit { get; set; }
        public decimal BundleQty { get; set; }
        public int ParentQty { get; set; }

        [Display(Name = "P. Sqft/Carton")]
        [DisplayFormat(DataFormatString = "{0:0.000000}", ApplyFormatInEditMode = true)]
        public decimal PurchaseCSft { get; set; }

        [Display(Name = "S. Sqft./Carton")]

        [DisplayFormat(DataFormatString = "{0:0.000000}", ApplyFormatInEditMode = true)]
        public decimal SalesCSft { get; set; }

        [DisplayFormat(DataFormatString = "{0:0.0000}", ApplyFormatInEditMode = true)]
        public decimal TotalSFT { get; set; }
        public decimal ParentMRP { get; set; }
        public decimal AdvSRate { get; set; }
        public int ChildQty { get; set; }
        public EnumProductStockType ProductStockType { get; set; }
    }
}