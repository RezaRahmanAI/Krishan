﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Model

{
    public class VoucherTransactionReportModel
    {
        public int VoucherTransactionID { get; set; }
        public DateTime VoucherDate { get; set; }
        public string VoucherNo { get; set; }
        public string DebitHead { get; set; }
        public decimal DebitAmount { get; set; }
        public string CreditHead { get; set; }
        public decimal CreditAmount { get; set; }
        public string Narration { get; set; }
        public string ModuleType { get; set; }
        public string Particulars { get; set; }
        public decimal Balance { get; set; }
        public decimal Opening { get; set; }
        public string ItemName { get; set; }
    }
}
