using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Model.TOs
{
    public class RecPayTransactionTO
    {
        public int? BankId { get; set; }
        public decimal Amount { get; set; }
        public DateTime TransactionDate { get; set; }
        public EnumTransactionType TransactionType { get; set; }
        public EnumPayType PaymentType { get; set; }
        //public EnumVoucherType VoucherType { get; set; }
        public decimal BankDebitAmount { get; set; }
    }
}
