using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using IMSWEB.Model;
using System.Data;
using IMSWEB.Model.SPModel;

namespace IMSWEB.Data
{
    public interface ICashCollectionRepository
    {
        void UpdateTotalDue(int CustomerId, int SupplierId, int BankID, int BankWithdrawID, decimal TotalDue);
        IEnumerable<DailyCashBookLedgerModel> DailyCashBookLedger(DateTime fromDate, DateTime toDate, int ConcernID);
        void UpdateTotalDueWhenEdit(int CustomerID, int SupplierID, int BankTransactionID, int CashCollectionID, decimal TotalRecAmt);
        IEnumerable<CashInHandReportModel> CashInHandReport(DateTime fromDate, DateTime toDate, int ReportType, int ConcernID, int CustomerType);
    }
}
