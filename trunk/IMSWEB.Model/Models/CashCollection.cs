using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IMSWEB.Model
{
    public partial class CashCollection : IModelBase
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CashCollectionID { get; set; }

        public EnumPayType PaymentType { get; set; }

        //[StringLength(250)]
        public string BankName { get; set; }

        //[StringLength(250)]
        public string BranchName { get; set; }

        public DateTime? EntryDate { get; set; }

        public decimal Amount { get; set; }

        public decimal AdjustAmt { get; set; }

        public decimal BalanceDue { get; set; }

        //[StringLength(350)]
        public string AccountNo { get; set; }

        //[StringLength(350)]
        public string MBAccountNo { get; set; }

        //[StringLength(350)]
        public string BKashNo { get; set; }

        public EnumTranType TransactionType { get; set; }

        public int? CustomerID { get; set; }

        public int? SupplierID { get; set; }

        public string ReceiptNo { get; set; }

        public int ConcernID { get; set; }

        public int CreatedBy { get; set; }
        public string Remarks { get; set; }
        public DateTime CreateDate { get; set; }

        public int? ModifiedBy { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public virtual Customer Customer { get; set; }

        public virtual SisterConcern SisterConcern { get; set; }

        public virtual Supplier Supplier { get; set; }
        public decimal InterestAmt { get; set; }
        public int? EmployeeID { get; set; }
        public decimal OfferAmt { get; set; }
        public decimal BonusAmt { get; set; }
        public int? CCBankID { get; set; }
        public int? PayCashAccountId { get; set; }
        public int? PayBankId { get; set; }
        public decimal CashBPercentage { get; set; }
        public decimal CashBAmt { get; set; }       
        public decimal YearlyBPercentage { get; set; }
        public decimal YearlyBnsAmt { get; set; }
        public decimal TotalDisAmt { get; set; }
        public decimal InvRemainingDue { get; set; }
        public string InvoiceNo { get; set; }
        public decimal InvoiceDue { get; set; }
        public decimal InvNetTotal { get; set; }
        public int SOrderID { get; set; }
        
    }
}
