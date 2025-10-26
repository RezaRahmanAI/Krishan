﻿
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IMSWEB.Model
{
    public partial class BankTransaction : AuditTrailModel
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public BankTransaction()
        {

        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int BankTranID { get; set; }

        public DateTime? TranDate { get; set; }
        public string TransactionNo { get; set; }
        public int TransactionType { get; set; }
        public int BankID { get; set; }
        public int? CustomerID { get; set; }
        public int? SupplierID { get; set; }
        public int? AnotherBankID { get; set; }
        public int ConcernID { get; set; }
        public decimal Amount { get; set; }
        // public int ProductType { get; set; }

        public string ChecqueNo { get; set; }
        public string Remarks { get; set; }

        public virtual Bank Bank { get; set; }

        public int? SIHID { get; set; }
        public int? PayCashAccountId { get; set; }


    }
}
