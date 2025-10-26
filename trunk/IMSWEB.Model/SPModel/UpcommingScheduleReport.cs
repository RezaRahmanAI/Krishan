﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Model
{
    public class UpcommingScheduleReport
    {
        public int CreditSalesID { get; set; }
        public int CustomerID { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string CustomerRefName { get; set; }
        public string CustomerRefContact { get; set; }
        public string InvoiceNo { get; set; }
        public DateTime SalesDate { get; set; }
        public DateTime PaymentDate { get; set; }
        public string Remarks { get; set; }
        public string CustomerConctact { get; set; }
        public string CustomerAddress { get; set; }
        public List<string> ProductName { get; set; }
        public decimal SalesPrice { get; set; }
        public decimal DownPayment { get; set; }
        public decimal PenaltyInterest { get; set; }
        public decimal Remaining { get; set; }
        public decimal TSalesAmt { get; set; }
        public decimal NetAmount { get; set; }
        public decimal FixedAmt { get; set; }
        public decimal InstallmentAmount { get; set; }
        public decimal DefaultAmount { get; set; }
        public DateTime? RemaindDate { get; set; }
        public string InstallmentPeriod { get; set; }
        public int NoOfInstallment { get; set; }
    }
}
