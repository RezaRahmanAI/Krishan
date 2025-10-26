using IMSWEB.Model;
using IMSWEB.Model.SPModel;
using IMSWEB.Model.TOs;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace IMSWEB.Service
{
    public interface ISalesOrderService
    {
        Task<IEnumerable<Tuple<int, string, DateTime, string,
                      string, decimal, EnumSalesType, Tuple<string>>>>
                      GetAllSalesOrderAsync(DateTime fromDate, DateTime toDate,
                      List<EnumSalesType> SalesType, bool IsVATManager, int concernID,
                      string InvoiceNo = "", string ContactNo = "", string CustomerName = "", string AccountNo = "");

        Task<IEnumerable<Tuple<int, string, DateTime, string,
                      string, decimal, EnumSalesType, Tuple<string, bool, string>>>>
                      GetAllAdvanceSalesOrderAsync(DateTime fromDate, DateTime toDate,
                      List<EnumSalesType> SalesType, bool IsVATManager, int concernID,
                      string InvoiceNo = "", string ContactNo = "", string CustomerName = "", string AccountNo = "");

        Task<IEnumerable<Tuple<int, string, DateTime, string,
  string, decimal, EnumSalesType, Tuple<string>>>>
      GetAllSalesOrderAsyncByUserID(int UserID, DateTime fromDate, DateTime toDate,
      EnumSalesType SalesType, string InvoiceNo = "", string ContactNo = "", string CustomerName = "", string AccountNo = "");


        Task<IEnumerable<Tuple<int, string, DateTime, string, string, decimal, EnumSalesType, Tuple<string, bool>>>> GetAllAdvanceSalesOrderAsyncByUserID(int UserID, DateTime fromDate, DateTime toDate, List<EnumSalesType> SalesType, string InvoiceNo = "", string ContactNo = "", string CustomerName = "", string AccountNo = "");



        IQueryable<SOrder> GetAllIQueryable();
        void AddSalesOrder(SOrder salesOrder);
        bool AddSalesOrderUsingSP(DataTable dtSalesOrder, DataTable dtSalesOrderDetail, DateTime RemindDate, int EmployeeID, bool isAdvanceSale = false);
        bool ApproveAdvanceSalesUsingSP(int salesOrderId);
        void AddReplacementOrderUsingSP(DataTable dtSalesOrder, DataTable dtSalesOrderDetail);
        void SaveSalesOrder();
        SOrder GetSalesOrderById(int id);
        void DeleteSalesOrder(int id);

        IEnumerable<SOredersReportModel> GetforSalesReport(DateTime fromDate, DateTime toDate, int EmployeeID, int CustomerID);

        IEnumerable<Tuple<DateTime, string, string, decimal, decimal, decimal, decimal, Tuple<decimal, decimal, decimal, decimal, decimal, string>>>
           GetforSalesDetailReport(DateTime fromDate, DateTime toDate);

        IEnumerable<SOredersReportModel> GetforSalesDetailReportByMO(DateTime fromDate, DateTime toDate, int MOID);

        IEnumerable<Tuple<string, string, DateTime, string, decimal, decimal, decimal, Tuple<decimal, decimal, decimal, decimal>>>
            GetSalesReportByConcernID(DateTime fromDate, DateTime toDate, int concernID, int CustomerType);

        IEnumerable<ProductWiseSalesReportModel>
           GetSalesDetailReportByConcernID(DateTime fromDate, DateTime toDate, int concernID);

        IEnumerable<ProductWiseSalesReportModel>
         GetSalesDetailReportAdminByConcernID(DateTime fromDate, DateTime toDate, int concernID, int CustomerType);

        IEnumerable<SOredersReportModel> GetSalesDetailReportByCustomerID(DateTime fromDate, DateTime toDate, int Customer);

        IEnumerable<Tuple<string, DateTime, string, string, decimal, decimal, Tuple<decimal, decimal, decimal, decimal>>>
            GetSalesDetailReportByMOID(DateTime fromDate, DateTime toDate, int concernId, int MOID, int RptType);

        IEnumerable<Tuple<string, string, string, string, string, decimal, decimal>>
                GetMOWiseCustomerDueRpt(int concernId, int MOID, int RptType);

        IEnumerable<Tuple<DateTime, string, string, decimal, decimal>> GetSalesByProductID(DateTime fromDate, DateTime toDate, int ConcernId, int productID);
        bool UpdateSalesOrderUsingSP(int userId, int salesOrderId, DataTable dtSalesOrder, DataTable dtSODetail, int EmployeeID, bool isAdvanceSale = false);

        void DeleteSalesOrderUsingSP(int orderId, int userId);
        void DeleteAdvanceSalesOrderUsingSP(int orderId, int userId);

        void DeleteSalesOrderDetailUsingSP(int orderId, int userId);

        void CorrectionStockData(int concermID);

        List<SRWiseCustomerSalesSummaryVM> SRWiseCustomerSalesSummary(DateTime fromdate, DateTime todate, int ConcernID, int EmployeeID);
        List<CustomerLedgerModel> CustomerLedger(DateTime fromdate, DateTime todate, int ConcernID, int CustomerID);
        List<LedgerAccountReportModel> CustomerLedger(DateTime fromdate, DateTime todate, int CustomerID);
        List<CustomerDueReportModel> CustomerDue(DateTime fromdate, DateTime todate, int ConcernID, int CustomerID, int IsOnlyDue);

        Task<IEnumerable<Tuple<int, string, DateTime, string, string, decimal, EnumSalesType>>> GetReplacementOrdersByAsync(int EmployeeID);
        Task<IEnumerable<Tuple<int, string, DateTime, string, string, decimal, EnumSalesType>>> GetReturnOrdersByAsync();

        List<ReplaceOrderDetail> GetReplaceOrderInvoiceReportByID(int OrderID);
        bool AddReturnOrderUsingSP(DataTable dtSalesOrder, DataTable dtSalesOrderDetail);
        List<ReplaceOrderDetail> GetReturnOrderInvoiceReportByID(int OrderID);
        List<DailyWorkSheetReportModel> DailyWorkSheetReport(DateTime fromdate, DateTime todate, int ConcernID);
        List<ReplacementReportModel> ReplacementOrderReport(DateTime fromdate, DateTime todate, int ConcernID, int CustomerID);
        List<ReturnReportModel> ReturnOrderReport(DateTime fromdate, DateTime todate, int ConcernID, int CustomerID);
        List<MonthlyBenefitReport> MonthlyBenefitReport(DateTime fromdate, DateTime todate, int ConcernID);
        List<ProductWiseBenefitModel> ProductWiseBenefitReport(DateTime fromdate, DateTime todate, int ConcernID);
        List<ProductWiseSalesReportModel> ProductWiseSalesReport(DateTime fromDate, DateTime toDate, int ConcernID, int CustomerID);
        List<ReplacementReportModel> DamageProductReport(DateTime fromdate, DateTime todate, int ConcernID, int CustomerID);
        List<ProductWiseSalesReportModel> ProductWiseSalesDetailsReport(int CompanyID, int CategoryID, int ProductID, DateTime fromDate, DateTime toDate);
        SOrder GetLastSalesOrderByCustomerID(int CustomerID);
        decimal GetAllCollectionAmountByDateRange(DateTime fromDate, DateTime toDate);
        decimal GetVoltageStabilizerCommission(DateTime fromDate, DateTime toDate);
        decimal GetExtraCommission(DateTime fromDate, DateTime toDate, int ConcernID);
        bool IsIMEIAlreadyReplaced(int StockDetailID);
        List<SOredersReportModel> GetAdminSalesReport(int ConcernID, DateTime fromDate, DateTime toDate);
        decimal GetEmployeeTragetCommission(DateTime fromDate, DateTime toDate, int ConcernID, int EmployeeID);
        IEnumerable<SOredersReportModel> GetSalesDetailReportBySRID(DateTime fromDate, DateTime toDate, int concernId, int SRID, int RptType);
        IEnumerable<SOredersReportModel> GetProductSalesDetailReportBySRID(DateTime fromDate, DateTime toDate, int concernId, int SRID, int ProductId, int RptType);

        List<VoucherTransactionReportModel> BankAndCashAccountLedgerData(DateTime fromDate, DateTime toDate, int ConcernID, int ExpenseItemID, string headType);
        List<RPTPayRecTO> GetReceiptPaymentReport(DateTime fromDate, DateTime toDate);
        List<SummaryReportModel> GetSummaryReport(DateTime fromDate, DateTime toDate, int ConcernID);
        bool IsSoReturn(int SoId);


    }
}
