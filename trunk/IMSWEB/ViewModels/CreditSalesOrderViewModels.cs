using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using IMSWEB.Model;
using System.Web;

namespace IMSWEB
{
    public class CreditSalesOrderViewModel
    {
        public CreateCreditSalesOrderDetailViewModel SODetail { get; set; }
        public ICollection<CreateCreditSalesOrderDetailViewModel> SODetails { get; set; }
        public ICollection<CreateCreditSalesSchedules> SOSchedules { get; set; }
        public CreateCreditSalesOrderViewModel SalesOrder { get; set; }
    }

    public class CreateCreditSalesOrderDetailViewModel
    {
        public string SODetailId { get; set; }

        public string SalesOrderId { get; set; }

        [Display(Name = "Product")]
        public string ProductId { get; set; }

        public string StockDetailId { get; set; }

        public string ProductName { get; set; }

        public string ProductCode { get; set; }

        public string ColorId { get; set; }

        [Display(Name = "Color")]
        public string ColorName { get; set; }

        [Display(Name = "Sales Rate")]
        public string UnitPrice { get; set; }

        [Display(Name = "Interest Percent.")]
        public string IntPercentage { get; set; }

        [Display(Name = "Interest Amount")]
        public string IntTotalAmt { get; set; }

        [Display(Name = "Quantity")]
        public string Quantity { get; set; }

        [Display(Name = "Purchase Rate")]
        public string MRPRate { get; set; }

        [Display(Name = "Total")]
        public string UTAmount { get; set; }

        [Display(Name = "Stock")]
        public string PreviousStock { get; set; }

        [Display(Name = "IME/Barcode")]
        public string IMENo { get; set; }

        [Display(Name = "PP Offer")]
        public string PPOffer { get; set; }
        [Display(Name = "Compressor")]
        public string CompressorWarrentyMonth { get; set; }
        [Display(Name = "Panel")]
        public string PanelWarrentyMonth { get; set; }
        [Display(Name = "Motor")]
        public string MotorWarrentyMonth { get; set; }
        [Display(Name = "SpareParts")]
        public string SparePartsWarrentyMonth { get; set; }

        [Display(Name = "Service Warrenty(Month)")]
        public string ServiceWarrentyMonth { get; set; }
        public IEnumerable<Stock> Stocks { get; set; }

        public IEnumerable<StockDetail> StockDetails { get; set; }
    }

    public class CreateCreditSalesOrderViewModel
    {
        public CreateCreditSalesOrderViewModel()
        {
            OfferDescription = "Currently available Offer with this Product";
        }
        public string SalesOrderId { get; set; }

        [Display(Name = "Invoice No.")]
        public string InvoiceNo { get; set; }

        [Display(Name = "Sales Date")]
        public string OrderDate { get; set; }

        [Display(Name = "Installment Date")]
        public string InstallmentDate { get; set; }

        [Display(Name = "Customer")]
        public string CustomerId { get; set; }

        [Display(Name = "PP Discount")]
        public string PPDiscountAmount { get; set; }

        [Display(Name = "VAT Percent.")]
        public string VATPercentage { get; set; }

        [Display(Name = "VAT Amount")]
        public string VATAmount { get; set; }

        [Display(Name = "Flat Discount(%)")]
        public string TotalDiscountPercentage { get; set; }

        [Display(Name = "Flat Discount")]
        public string TotalDiscountAmount { get; set; }

        [Display(Name = "Total Discount")]
        public string NetDiscount { get; set; }

        [Display(Name = "Net Total")]
        public string TotalAmount { get; set; }

        [Display(Name = "Installments")]
        public string InstallmentNo { get; set; }

        [Display(Name = "Installment")]
        public string InstallmentAmount { get; set; }

        [Display(Name = "Grand Total")]
        public string GrandTotal { get; set; }

        [Display(Name = "Down Payment")]
        public string RecieveAmount { get; set; }

        [Display(Name = "Remain. Amount")]
        public string PaymentDue { get; set; }

        [Display(Name = "Last Pay Adj.")]
        public string PayAdjustment { get; set; }

        public string Status { get; set; }

        [Display(Name = "Previous Due")]
        public string CurrentDue { get; set; }

        public string CurrentPreviousDue { get; set; }

        [Display(Name = "Note")]
        public string Remarks { get; set; }

        public string WInterestAmt { get; set; }

         [Display(Name = "Interest Rate")]
        public string InterestRate { get; set; }

         [Display(Name = "Int. Amount")]
        public string InterestAmount { get; set; }

        [Display(Name = "Total Offer")]
        public string TotalOffer { get; set; }
        [Display(Name ="Offer")]
        public string OfferDescription { get; set; }

        [Display(Name = "Extend Time Interest")]
        public string ExtendTimeInterestAmount { get; set; }
        public bool IsAllPaid { get; set; }
        public string InstallmentPeriod { get; set; }

    }

    public class CreateCreditSalesSchedules
    {
        public int CSScheduleID { get; set; }

        public string SalesOrderId { get; set; }

        [Display(Name = "Schedule")]
        public string ScheduleDate { get; set; }
        public string ScheduleNo { get; set; }
        [Display(Name = "Pay Date")]
        public string PayDate { get; set; }

        [Display(Name = "Installment")]
        public string InstallmentAmount { get; set; }

        [Display(Name = "Closing")]
        public string ClosingBalance { get; set; }

        [Display(Name = "Status")]
        public string PaymentStatus { get; set; }

        [Display(Name = "Opening")]
        public string OpeningBalance { get; set; }

        public string Remarks { get; set; }

        public bool IsUnExpected { get; set; }
        public decimal HireValue { get; set; }
        [Display(Name="Net Installment")]
        public decimal NetValue { get; set; }
    }

    public class GetCreditSalesOrderViewModel
    {
        public string SalesOrderId { get; set; }

        [Display(Name = "Sales Date")]
        public string OrderDate { get; set; }

        [Display(Name = "Invoice No")]
        public string InvoiceNo { get; set; }

        [Display(Name = "Customer")]
        public string CustomerName { get; set; }

        [Display(Name = "Contact No")]
        public string ContactNo { get; set; }

        [Display(Name = "Total Amount")]
        public string TotalAmount { get; set; }

        [Display(Name = "Due Amount")]
        public string DueAmount { get; set; }

        public string Status { get; set; }
    }
}