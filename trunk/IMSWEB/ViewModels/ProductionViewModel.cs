using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using IMSWEB.Model;

namespace IMSWEB
{
    public class ProductionViewModel
    {
        public ProductionViewModel()
        {
            Details = new List<ProductionDetailViewModel>();
        }
        public int? ProductionID { get; set; }

        [Display(Name = "Production No")]
        public string ProductionCode { get; set; }
        public DateTime Date { get; set; }
        public EnumProductionStatus Status { get; set; }
        public ProductionDetailViewModel Detail { get; set; }
        public List<ProductionDetailViewModel> Details { get; set; }
    }

    public class ProductionDetailViewModel
    {
        public ProductionDetailViewModel()
        {
            //ProductionIMEIs = new List<ProductionIMEIViewModel>();
            ProductionRawMaterials = new List<ProductionRawMaterialViewModel>();
        }
        public int? PDetailID { get; set; }

        [Display(Name = "Product")]
        public int? ProductID { get; set; }
        [Display(Name = "Product")]
        public string ProductName { get; set; }
        public decimal Quantity { get; set; }
        public string TotalCost { get; set; }
        public int? ProductionID { get; set; }
        //public List<ProductionIMEIViewModel> ProductionIMEIs { get; set; }
        public List<ProductionRawMaterialViewModel> ProductionRawMaterials { get; set; }
        [Display(Name = "PCS/Carton")]
        public decimal ConvertValue { get; set; }
        [Display(Name = "PCS")]
        public decimal ChildQuantity { get; set; }

        [Display(Name = "Qty")]
        public decimal ParentQuantity { get; set; }
    }
    //public class ProductionIMEIViewModel
    //{
    //    public int? PIID { get; set; }
    //    public string IMEI { get; set; }
    //    public int? PDetailID { get; set; }
    //}

    public class ProductionRawMaterialViewModel
    {
        public int? PRMID { get; set; }
        public int? ProductID { get; set; }
        public int? SDetailID { get; set; }
        public string Quantity { get; set; }
        public string PRate { get; set; }
        public int? PDetailID { get; set; }

    }
}