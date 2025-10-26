using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Model.TOs
{
    public class RPTPayRecTO
    {
        public int Id { get; set; }
        public string DebitParticular { get; set; }
        public string CreditParticular { get; set; }
        public decimal DebitAmount { get; set; }
        public decimal CreditAmount { get; set; }
        public bool IsClosing { get; set; }
        public decimal ClosingBalance { get; set; }
        public DateTime TransactionDate { get; set; }
        public bool IsDrHeader { get; set; }
        public bool IsCrHeader { get; set; }
        public decimal BankDebitAmount { get; set; }
        public decimal BankRecAmount { get; set; }
        public decimal BankPayAmount { get; set; }
        public string ProjectName { get; set; }
        public bool IsProject { get; set; } = false;
        public decimal CashJournalCreditAmount { get; set; }
        public decimal CashJournalDebitAmount { get; set; }
        public decimal ExpenseJournalAmount { get; set; }
        public decimal IncomeJournalAmount { get; set; }
        public decimal CBookExpJournalAmount { get; set; }



    }
}
