﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Model
{
    public class ProductWisePurchaseModel
    {
        public int POrderID { get; set; }
        public string ChallanNo { get; set; }
        public DateTime Date { get; set; }
        public string SupplierCode { get; set; }
        public string SupplierName { get; set; }
        public string Address { get; set; }
        public string Mobile { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public decimal Quantity { get; set; }
        public decimal PreStock { get; set; }
        public int ParentQuantity { get; set; }
        public decimal PurchaseRate { get; set; }
        public decimal TotalAmount { get; set; }
        public string CompanyName { get; set; }

        public int UnitType { get; set; }
        public string CategoryName { get; set; }

        public string PWDiscount { get; set; }
        public int CompanyID { get; set; }

        public int CategoryID { get; set; }
        public decimal GrandTotal { get; set; }
        public decimal NetDiscount { get; set; }
        public decimal FlatDiscount { get; set; }
        public decimal LaborCost { get; set; }
        public decimal NetTotal { get; set; }
        public decimal RecAmt { get; set; }
        public decimal PaymentDue { get; set; }
        public int ProductID { get; set; }
        public int POPDID { get; set; }
        public decimal PPDISAmt { get; set; }
        public string DamageIMEI { get; set; }
        public string IMENO { get; set; }
        public int ColorID { get; set; }
        public string ColorName { get; set; }
        public decimal PPOffer { get; set; }
        public decimal SRate { get; set; }
        public decimal MRP { get; set; }
        public int? IsDamageReplaced { get; set; }
        public string ConcenName { get; set; }
        public decimal TotalCreditSR3 { get; set; }
        public decimal TotalCreditSR6 { get; set; }
        public decimal TotalCreditSR12 { get; set; }
        public string GodownName { get; set; }
        public int SizeID { get; set; }
        public string SizeName { get; set; }
        public int DiaSizeID { get; set; }
        public string DiaSizeName { get; set; }
        public int ProductType { get; set; }
        public int ProductStockType { get; set; }
        public decimal ConvertValue { get; set; }
        public decimal BundleQty { get; set; }
        public decimal PurchaseCSft { get; set; }
        public decimal SalesCSft { get; set; }
        public int StockID { get; set; }

        public decimal TotalSFT { get; set; }

        public int SDetailID { get; set; }

        public string OfferDescription { get; set; }

        public decimal SFTRate { get; set; }

        public decimal RatePerParentUnit { get; set; }

        public int ChildQuantity { get; set; }

        public int GodownID { get; set; }

        public string ChildUnitName { get; set; }

        public string ParentUnitName { get; set; }
        //public string CategoryName { get; set; }

        public string FromConcernName { get; set; }

        public string ToConcernName { get; set; }

        public string TransType { get; set; }
        public decimal DP { get; set; }
        public decimal StockQty { get; set; }
        public decimal RawConvertValue { get; set; }

    }
}
