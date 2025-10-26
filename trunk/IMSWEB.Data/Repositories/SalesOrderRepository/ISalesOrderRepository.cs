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
    public interface ISalesOrderRepository
    {
        bool AddSalesOrderUsingSP(DataTable dtSalesOrder, DataTable dtSalesOrderDetail,DateTime RemindDate, int EmployeeID, bool isAdvanceSale = false);
        bool ApproveAdvanceSalesUsingSP(int salesOrderId);
        void AddReplacementOrderUsingSP(DataTable dtSalesOrder, DataTable dtSalesOrderDetail);
        bool UpdateSalesOrderUsingSP(int userId, int salesOrderId, DataTable dtSalesOrder, DataTable dtSODetail, int EmployeeID, bool isAdvanceSale = false);
        void DeleteSalesOrderUsingSP(int orderId, int userId);
        void DeleteAdvanceSalesOrderUsingSP(int orderId, int userId);
        void DeleteSalesOrderDetailUsingSP(int sorderDetailId, int userId);
        void CorrectionStockData(int concermID);
        List<SRWiseCustomerSalesSummaryVM> SRWiseCustomerSalesSummary(DateTime fromdate, DateTime todate, int ConcernID, int EmployeeID);
        List<CustomerLedgerModel> CustomerLedger(DateTime fromdate, DateTime todate, int ConcernID, int CustomerID);
        List<CustomerDueReportModel> CustomerDue(DateTime fromdate, DateTime todate, int ConcernID, int CustomerID, int IsOnlyDue);
        bool AddReturnOrderUsingSP(DataTable dtSalesOrder, DataTable dtSalesOrderDetail);
        List<DailyWorkSheetReportModel> DailyWorkSheetReport(DateTime fromdate, DateTime todate, int ConcernID);

        List<ReturnReportModel> ReturnReport(DateTime fromdate, DateTime todate, int ConcernID, int CustomerID);
        List<MonthlyBenefitReport> MonthlyBenefitReport(DateTime fromdate, DateTime todate, int ConcernID);
        List<ProductWiseBenefitModel> ProductWiseBenefitReport(DateTime fromdate, DateTime todate, int ConcernID);
    }
}
