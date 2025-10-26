using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IMSWEB.Model;
using System.Data;

namespace IMSWEB.Service
{
    public interface ICreditSalesOrderService
    {
        Task<IEnumerable<Tuple<int, string, DateTime, string,
            string, decimal, EnumSalesType>>> GetAllSalesOrderAsync(DateTime fromDate, DateTime toDate);

        void AddSalesOrder(CreditSale salesOrder);

        void AddSalesOrderUsingSP(DataTable dtSalesOrder, DataTable dtSODetail,
            DataTable dtSchedules);
        IQueryable<CreditSale> GetAllIQueryable();
        void InstallmentPaymentUsingSP(int orderId, decimal installmentAmount, DataTable dtSchedules, decimal LastPayAdjustment);

        void SaveSalesOrder();

        CreditSale GetSalesOrderById(int id);

        IEnumerable<CreditSaleDetails> GetSalesOrderDetails(int id);

        void UpdateSalesOrder(CreditSale creditSale);

        IEnumerable<Tuple<int, int, int, int,
            decimal, decimal, decimal, Tuple<decimal, string, string, int, string>>> GetCustomSalesOrderDetails(int id);

        IEnumerable<CreditSalesSchedule> GetSalesOrderSchedules(int id);
        IEnumerable<UpcommingScheduleReport> GetUpcomingSchedule(DateTime fromDate, DateTime toDate);
        IEnumerable<UpcommingScheduleReport> GetScheduleCollection(DateTime fromDate, DateTime toDate, int concernID);
        IEnumerable<Tuple<string, string, string, string, DateTime, DateTime, decimal, Tuple<decimal, decimal, decimal, decimal, string, decimal>>> GetCreditCollectionReport(DateTime fromDate, DateTime toDate, int concernID,int CustomerID);
        IEnumerable<Tuple<string, string, string, decimal, decimal>> GetDefaultingCustomer(DateTime date, int concernID);
        IEnumerable<Tuple<string, string, string, string, DateTime, DateTime, decimal, Tuple<decimal, decimal, decimal, decimal, string, decimal, decimal, Tuple<int, decimal>>>>
            GetDefaultingCustomer(DateTime fromDate, DateTime toDate, int concernID);

        void ReturnSalesOrderUsingSP(int orderId, int userId);

        void DeleteSalesOrder(int id);

        bool HasPaidInstallment(int id);

        void CalculatePenaltySchedules(int ConcernID);

        void CorrectionStockData(int concermID);

        //CreditSale GetSalesOrderByInvoiceNo(string InvoiceNo,int concernID);
        IEnumerable<Tuple<string, string, DateTime, string, decimal, decimal, decimal, Tuple<decimal, decimal, decimal, decimal, decimal, int, string>>>
         GetCreditSalesReportByConcernID(DateTime fromDate, DateTime toDate, int concernID, int CustomerType);

        IEnumerable<Tuple<DateTime, string, string, string, decimal, decimal, decimal, Tuple<decimal, decimal, decimal, decimal, decimal, string, string, Tuple<int>>>>
        GetCreditSalesDetailReportByConcernID(DateTime fromDate, DateTime toDate, int concernID);
        decimal GetDefaultAmount(int CreditSaleID, DateTime FromDate);
        List<ProductWiseSalesReportModel> ProductWiseCreditSalesReport(DateTime fromDate, DateTime toDate, int ConcernID, int CustomerID);
        List<ProductWiseSalesReportModel> ProductWiseCreditSalesDetailsReport(int CompanyID, int CategoryID, int ProductID, DateTime fromDate, DateTime toDate);
        void DeleteSchedule(CreditSalesSchedule CreditSalesSchedule);
        void AddSchedule(CreditSalesSchedule CreditSalesSchedule);
        void UpdateSchedule(CreditSalesSchedule scheduel);
        List<SOredersReportModel> SRWiseCreditSalesReport(int EmployeeID, DateTime fromDate, DateTime toDate);
        IQueryable<SOredersReportModel> GetAdminCrSalesReport(int ConcernID, DateTime fromDate, DateTime toDate);
        IQueryable<CashCollectionReportModel> AdminInstallmentColllections(int ConcernID, DateTime fromDate, DateTime toDate);
    }
}
