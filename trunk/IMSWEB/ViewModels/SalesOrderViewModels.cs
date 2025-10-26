using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using IMSWEB.Model;
using System.Web;
using System.Web.Mvc;

namespace IMSWEB
{
    public class SalesOrderViewModel
    {
        public CreateSalesOrderDetailViewModel SODetail { get; set; }
        public ICollection<CreateSalesOrderDetailViewModel> SODetails { get; set; }
        public CreateSalesOrderViewModel SalesOrder { get; set; }
        public ICollection<CreateSaleOfferViewModel> SalesOffers { get; set; }
    }

    public class CreateSalesOrderDetailViewModel
    {
        public string SODetailId { get; set; }

        public string SalesOrderId { get; set; }

        [Display(Name = "Product")]
        public string ProductId { get; set; }

        public string StockDetailId { get; set; }
        public string RStockDetailId { get; set; }

        public string ProductName { get; set; }

        public string ProductCode { get; set; }

        public string ColorId { get; set; }

        [Display(Name = "Color")]
        public string ColorName { get; set; }

        [Display(Name = "Size")]
        public string SizeName { get; set; }
        [Display(Name = "Category")]
        public string CategoryName { get; set; }

        [Display(Name = "Rate/gm")]
        public string UnitPrice { get; set; }


        [Display(Name = "Dis. Per.")]
        public string PPDPercentage { get; set; }

        [Display(Name = "Dis. Amt.")]
        public string PPDAmount { get; set; }

        [Display(Name = "Qty")]
        public string Quantity { get; set; }

        public EnumStatus Status { get; set; }

        [Display(Name = "Pur. Rate")]
        public string MRPRate { get; set; }

        [Display(Name = "Total")]
        public string UTAmount { get; set; }

        [Display(Name = "Stock")]
        public string PreviousStock { get; set; }

        [Display(Name = "IME/Barcode")]
        public string IMENo { get; set; }
        public List<string> IMEIList { get; set; }

        [Display(Name = "PP Offer")]
        public string PPOffer { get; set; }

        [Display(Name = "Damage IMEI")]
        public string DamageIMEINO { get; set; }

        [Display(Name = "Damage UnitPrice")]
        public string DamageUnitPrice { get; set; }

        [Display(Name = "Replace IMEI")]
        public string ReplaceIMEINO { get; set; }
        [Display(Name = "Damage Product")]
        public string DamageProductName { get; set; }
        public int RepOrderID { get; set; }
        //[Required(ErrorMessage="Remarks Required")]
        public string Remarks { get; set; }
        public int ProductType { get; set; }
        [Display(Name = "Compressor")]
        public string CompressorWarrentyMonth { get; set; }
        [Display(Name = "Panel")]
        public string PanelWarrentyMonth { get; set; }
        [Display(Name = "Motor")]
        public string MotorWarrentyMonth { get; set; }
        [Display(Name = "SpareParts")]
        public string SparePartsWarrentyMonth { get; set; }
        [Display(Name = "Service")]
        public string ServiceWarrentyMonth { get; set; }
        public int DamagePOPDID { get; set; }


        [Display(Name = "Total(Sqft)")]
        public decimal TotalArea { get; set; }

        [Display(Name = "Sqft/Ctn")]
        public decimal AreaPerCarton { get; set; }

        [Display(Name = "Rate/Kg")]
        public decimal RatePerArea { get; set; }

        [Display(Name = "PCS")]
        public int ChildQuantity { get; set; }

        [Display(Name = "Carton")]
        public decimal ParentQuantity { get; set; }
        [Display(Name = "Bonus Qty")]
        public string BonusQuantity { get; set; }

        [Display(Name = "PCS/Carton")]
        public decimal ConvertValue { get; set; }

        public IEnumerable<Stock> Stocks { get; set; }


        public IEnumerable<StockDetail> StockDetails { get; set; }

        [Display(Name = "Fraction Qty")]
        public decimal FractionQty { get; set; }

        [Display(Name = "Fraction Amt")]
        [DisplayFormat(DataFormatString = "{0:0.0000}", ApplyFormatInEditMode = true)]
        public decimal FractionAmt { get; set; }
    }

    public class CreateSalesOrderViewModel
    {

        public CreateSalesOrderViewModel()
        {
            OfferDescription = "Currently available Offer with this Product";
        }

        public string SalesOrderId { get; set; }

        [Display(Name = "Invoice No.")]
        public string InvoiceNo { get; set; }

        [Display(Name = "Sales Date")]
        public string OrderDate { get; set; }

        [Display(Name = "Remind Date")]
        public string RemindDate { get; set; }

        [Display(Name = "Customer")]
        public string CustomerId { get; set; }

        [Display(Name = "Depot")]
        public string DepotId { get; set; }

        [Display(Name = "PP Discount")]
        public string PPDiscountAmount { get; set; } //To store Net discount in view
        public string TempFlatDiscountAmount { get; set; } //To store Flat discount in view

        [Display(Name = "VAT Percent.")]
        public string VATPercentage { get; set; }

        [Display(Name = "VAT Amount")]
        public string VATAmount { get; set; }

        [Display(Name = "Flat Dis. Per.")]
        public string TotalDiscountPercentage { get; set; }

        [Display(Name = "Flat Dis. Amt.")]
        public string TotalDiscountAmount { get; set; }

        [Display(Name = "Carton Bonus %")]        
        public string CartonPercentage { get; set; }

        [Display(Name = "Carton Amt.")]
        public string CartonAmt { get; set; }

        [Display(Name = "After Carton")]
        public string AfterCartonAmt { get; set; }

        [Display(Name = "Cash Bonus %")]
        public string CashBPercentage { get; set; }

        [Display(Name = "CashBonus Amt.")]
        public string CashBAmt { get; set; }

        [Display(Name = "After CashBonus")]
        public string AfterCashAmt { get; set; }

        [Display(Name = "Yearly Bonus %")]
        public string YearlyBPercentage { get; set; }

        [Display(Name = "Yearly Amt.")]
        public string YearlyBnsAmt { get; set; }

        [Display(Name = "After Yearly")]
        public string AfterYearlyAmt { get; set; }

        [Display(Name = "Total Dis.")]
        public string NetDiscount { get; set; }

        [Display(Name = "Net Total")]
        public string TotalAmount { get; set; }

        [Display(Name = "Adjust. Amount")]
        public string AdjAmount { get; set; }

        [Display(Name = "Grand Total")]
        public string GrandTotal { get; set; }

        [Display(Name = "Pay Amount")]
        public string RecieveAmount { get; set; }

        [Display(Name = "Payment Due")]
        public string PaymentDue { get; set; }

        [Display(Name = "Total Due")]
        public string TotalDue { get; set; }

        [Display(Name = "Send SMS ")]
        public bool IsSmsEnable { get; set; }

        public string Status { get; set; }

        [Display(Name = "Previous Due")]
        public string CurrentDue { get; set; }

        [Display(Name = "Remarks")]
        public string Remarks { get; set; }

        [Display(Name = "Total Offer")]
        public string TotalOffer { get; set; }


        public string OfferDescription { get; set; }

        [Display(Name = "Damage Total")]
        public string DamageTotalAmount { get; set; }

        [Display(Name = "Replace Total")]
        public string ReplaceTotalAmount { get; set; }

        [Display(Name = "Total Frac. Amt")]
        [DisplayFormat(DataFormatString = "{0:0.0000}", ApplyFormatInEditMode = true)]
        public decimal TotalFractionAmt { get; set; }
        [Display(Name = "Employee")]
        public string EmployeeID { get; set; }
        public decimal PrevDue { get; set; }
        public EnumTerms TermsType { get; set; }
        public List<SelectListItem> PayItems { get; set; }
        [Display(Name = "Payment")]
        public string ItemId { get; set; }
        public int? PayCashAccountId { get; set; }
        public int? PayBankId { get; set; }
        public ICollection<SelectListItem> PayHeads { get; set; }

    }

    public class GetSalesOrderViewModel
    {
        public string SalesOrderId { get; set; }

        [Display(Name = "Sales Date")]
        public string OrderDate { get; set; }

        [Display(Name = "Invoice No")]
        public string InvoiceNo { get; set; }

        [Display(Name = "Customer")]
        public string CustomerName { get; set; }
        [Display(Name = "Employee Name")]
        public string EmployeeName { get; set; }

        [Display(Name = "Contact No")]
        public string ContactNo { get; set; }

        [Display(Name = "Total Amount")]
        public string TotalAmount { get; set; }

        [Display(Name = "Due Amount")]
        public string DueAmount { get; set; }

        public string Status { get; set; }
        public bool IsApproved { get; set; }

    }
}