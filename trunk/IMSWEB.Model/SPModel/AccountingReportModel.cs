using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Model
{
    public class TrialBalanceReportModel
    {
        public int SerialNumber { get; set; }
        public string Particulars{ get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
    }

    public class ProfitLossReportModel
    {
        public int SerialNumber { get; set; }
        public string DebitParticulars { get; set; }
        public decimal Debit { get; set; }
        public string CreditParticulars { get; set; }
        public decimal Credit { get; set; }
    }
}
