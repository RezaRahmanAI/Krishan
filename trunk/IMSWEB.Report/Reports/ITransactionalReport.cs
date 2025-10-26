using IMSWEB.Model;
using System;
using System.Collections.Generic;

namespace IMSWEB.Report
{
    public interface ITransactionalReport
    {
        byte[] ExpenditureReport(DateTime fromDate, DateTime toDate, string userName, int concernID, int reportType, int ExpenseItemID);
        byte[] SalesReport(DateTime fromDate, DateTime toDate, string userName, int concernID, int reportType, string period, int CustomerType);
        byte[] SalesBenefitReport(DateTime fromDate, DateTime toDate, string userName, int concernID, int reportType, string period, int CustomerType);
        //byte[] AdminSalesReportDetails(DateTime fromDate, DateTime toDate, string userName, int concernID, int reportType, string period, int CustomerType);
        byte[] PurchaseReport(DateTime fromDate, DateTime toDate, string userName, int concernID, int reportType, string period, EnumPurchaseType PurchaseType);
        byte[] SalesInvoiceReport(SOrder sorder, string userName, int concernID);
        byte[] ChallanReport(SOrder sorder, string userName, int concernID);
        byte[] CustomeWiseSalesReport(DateTime fromDate, DateTime toDate, string userName, int concernID, int reportType, int CustomerID);

        byte[] MOWiseSalesReport(DateTime fromDate, DateTime toDate, string userName, int concernID, int MOID, int RptType);

        byte[] MOWiseCustomerDueRpt(string userName, int concernID, int MOID, int RptType);

        byte[] SuplierWisePurchaseReport(DateTime fromDate, DateTime toDate, string userName, int concernID, int reportType, int SupplierID);

        byte[] StockDetailReport(string userName, int concernID, int reportType, int CompanyID, int CategoryID, int ProductID, int GodownID);
        byte[] StockSummaryReport(string userName, int concernID, int reportType, int CompanyID, int CategoryID, int ProductID, int GodownID, EnumProductStockType ProductStockType);
        byte[] SalesInvoiceReport(int orderId, string p1, int p2);
        byte[] ChallanReport(int orderId, string p1, int p2);

        byte[] InstallmentCollectionReport(DateTime fromDate, DateTime toDate, string p1, int p2);

        byte[] UpComingScheduleReport(DateTime fromDate, DateTime toDate, string p1, int p2);

        byte[] DefaultingCustomerReport(DateTime date, string userName, int concernID);
        byte[] DefaultingCustomerReport(DateTime fromDate, DateTime toDate, string userName, int concernID);

        byte[] CashCollectionReport(DateTime fromDate, DateTime toDate, string userName, int concernID, int customerId, int reportType);

        byte[] CashDeliverReport(DateTime fromDate, DateTime toDate, string userName, int concernID, int supplierId, int reportType);

        byte[] CreditSalesInvoiceReport(CreditSale sorder, string userName, int concernID);

        byte[] CreditSalesInvoiceReportByID(int sorderID, string userName, int concernID);

        byte[] SRInvoiceReport(int orderId, string p1, int p2);

        byte[] SRInvoiceReportByChallanNo(string challanNo, string p1, int p2);

        byte[] MOWiseSDetailReport(DateTime fromDate, DateTime toDate, string userName, int concernID, int MOID);

        byte[] SRVisitStatusReport(DateTime fromDate, DateTime toDate, string userName, int concernID, int MOID);

        byte[] ProductWisePriceProtection(DateTime fromDate, DateTime toDate, string userName, int concernID);
        byte[] ProductWisePandSReport(DateTime fromDate, DateTime toDate, string userName, int concernID, int productID);
        byte[] SRWiseCustomerSalesSummary(DateTime fromDate, DateTime toDate, string userName, int concernID, int EmployeeID);
        byte[] CustomerLedgerDetails(DateTime fromDate, DateTime toDate, string userName, int concernID, int CustomerID);
        byte[] CustomerLedgerSummary(DateTime fromDate, DateTime toDate, string userName, int concernID, int CustomerID);
        byte[] CustomerDueReport(DateTime fromDate, DateTime toDate, string userName, int concernID, int CustomerID, int IsOnlyDue);
        byte[] DailyStockVSSalesSummary(DateTime fromDate, DateTime toDate, string userName, int concernID, int ProductID);
        byte[] DailyCashBookLedger(DateTime fromDate, DateTime toDate, string userName, int concernID);

        byte[] ProfitAndLossReport(DateTime fromDate, DateTime toDate, string userName, int concernID);

        byte[] SummaryReport(DateTime fromDate, DateTime toDate, string userName, int concernID);
        byte[] ReplacementInvoiceReport(IEnumerable<ReplaceOrderDetail> ROrderDetails, ReplaceOrder ROrder, string userName, int concernID);
        byte[] ReplaceInvoiceReportByID(int orderId, string username, int concernID);
        byte[] ReturnInvoiceReport(IEnumerable<ReplaceOrderDetail> ROrderDetails, ReplaceOrder ROrder, string userName, int concernID);
        byte[] ReturnInvoiceReportByID(int orderId, string username, int concernID);
        byte[] DailyWorkSheet(DateTime fromDate, DateTime toDate, string userName, int concernID);
        byte[] SRVisitReportUsingSP(DateTime fromDate, DateTime toDate, string userName, int concernID, int EmployeeID);
        byte[] SRVisitReportDetails(DateTime fromDate, DateTime toDate, string userName, int concernID, int EmployeeID, int ReportType);
        byte[] SRWiseCustomerStatusReport(DateTime fromDate, DateTime toDate, string userName, int concernID, int EmployeeID);
        byte[] ReplacementReport(DateTime fromDate, DateTime toDate, string userName, int concernID, int CustomerID);
        byte[] ReturntReport(DateTime fromDate, DateTime toDate, string userName, int concernID, int CustomerID);
        byte[] CashCollectionMoneyReceipt(CashCollection cashCollection, string userName, int concernID);
        byte[] CashCollectionMoneyReceiptByID(int cashCollectionID, string userName, int concernID);
        byte[] CashDeliveryMoneyReceiptPrint(int cashCollectionID, string userName, int concernID);
        byte[] CrditSalesMoneyReceipt(CreditSale CreditSale, List<CreditSaleDetails> details, CreditSalesSchedule schedules, string userName, int concernID);
        byte[] CrditSalesMoneyReceiptByID(int CreditSalesID, string userName, int concernID);
        byte[] MonthlyBenefit(DateTime fromDate, DateTime toDate, string userName, int concernID);
        byte[] ProductWiseBenefitReport(DateTime fromDate, DateTime toDate, int ProductID, string userName, int concernID);
        byte[] ProductWiseSalesReport(DateTime fromDate, DateTime toDate, int CustomerID, string userName, int concernID);
        byte[] ProductWisePurchaseReport(DateTime fromDate, DateTime toDate, int SupplierID, string userName, int concernID, EnumPurchaseType PurchaseType);
        byte[] DamageProductReport(DateTime fromDate, DateTime toDate, string userName, int concernID, int CustomerID);
        byte[] SRWiseCashCollectionReport(DateTime fromDate, DateTime toDate, string userName, int concernID, int EmployeeID);
        byte[] ProductwiseSalesDetails(string userName, int concernID, int reportType, int CompanyID, int CategoryID, int ProductID, DateTime fromDate, DateTime toDate);
        byte[] ProductwiseSalesSummary(string userName, int concernID, int reportType, int CompanyID, int CategoryID, int ProductID, DateTime fromDate, DateTime toDate);

        byte[] ProductWisePurchaseDetailsReport(string userName, int concernID, int reportType, int CompanyID, int CategoryID, int ProductID, DateTime fromDate, DateTime toDate, EnumPurchaseType PurchaseType);
        byte[] BankTransactionReport(string userName, int concernID, int reportType, int BankID, DateTime fromDate, DateTime toDate);
        byte[] POInvoice(POrder POrder, string userName, int concernID);
        byte[] POInvoiceByID(int POrderID, string userName, int concernID);
        byte[] BankSummaryReport(DateTime fromDate, DateTime toDate, string userName, int concernID, int ProductID);

        byte[] BankLedgerReport(DateTime fromDate, DateTime toDate, string userName, int concernID, int BankID);
        byte[] GetDamagePOReport(string userName, int concernID, int SupplierID, DateTime fromDate, DateTime toDate);
        byte[] GetDamageReturnPOReport(string userName, int concernID, int SupplierID, DateTime fromDate, DateTime toDate);
        byte[] GetSalarySheet(DateTime dtSalaryMonth, int EmployeeID, int DepartmentID, List<int> EmployeeIDList, string UserName, int ConcernID, Tuple<DateTime, DateTime> SalaryMonth);
        byte[] GetPaySlip(DateTime dtSalaryMonth, int EmployeeID, string UserName, int ConcernID, Tuple<DateTime, DateTime> SalaryMonth);
        byte[] NewBankTransactionsReport(DateTime fromDate, DateTime toDate, int BankID, string UserName, int ConcernID);
        byte[] SalesReportAdmin(DateTime fromDate, DateTime toDate, string userName, int concernID, int UserConcernID, int CustomerType, int ReportType);
        byte[] AdminPurchaseReport(DateTime fromDate, DateTime toDate, string userName, int concernID, int UserConcernID);
        byte[] AdminCustomerDueRpt(string userName, int concernID, int UserCocernID, int CustomerType, int DueType);
        byte[] AdminCashCollectionReport(string userName, int concernID, int UserCocernID, DateTime fromDate, DateTime toDate);
        byte[] CashInHandReport(string userName, int concernID, int ReportType, DateTime fromDate, DateTime toDate, int CustomerType);
        byte[] CashInHandReportAdmin(string userName, int concernID, int ReportType, DateTime fromDate, DateTime toDate, int CustomerType);
        byte[] BankTransMoneyReceipt(string userName, int concernID, int BankTranID);
        byte[] ExpenseIncomeMoneyReceipt(string userName, int concernID, int ExpenditureID, bool IsExpense);
        byte[] DailyAttendence(string userName, int concernID, int DepartmentID, DateTime Date, bool IsPresent, bool IsAbsent);
        byte[] StockLedgerReport(DateTime fromDate, DateTime toDate, string userName, int concernID, int reportType, int CompanyID, int CategoryID, int ProductID);
        byte[] SupplierLedger(string userName, int concernID, DateTime fromDate, DateTime toDate, int SupplierID);
        byte[] CustomerLedger(DateTime fromDate, DateTime toDate, string UserName, int ConcernID, int CustomerID);
        byte[] SalesReturnChallan(int oOrderID, string userName, int concernID);
        byte[] GetTrialBalance(DateTime fromDate, DateTime toDate, string UserName, int ConcernID, string ClientDateTime);
        byte[] ProfitLossAccount(DateTime fromDate, DateTime toDate, string UserName, int ConcernID, string ClientDateTime);
        byte[] BalanceSheet(DateTime fromDate, DateTime toDate, string UserName, int ConcernID, string ClientDateTime);
        byte[] MonthlyTransactionReport(DateTime fromDate, DateTime toDate, string userName, int concernID);

        byte[] LiabilityReport(DateTime fromDate, DateTime toDate, string UserName, int ConcernID, int HeadID, bool OnlyHead);
        byte[] SRWiseSalesReport(DateTime fromDate, DateTime toDate, string userName, int concernID, int MOID, int RptType);
        byte[] SRWiseProductSalesReport(DateTime fromDate, DateTime toDate, string userName, int concernID, int MOID, int ProductId, int RptType);

        byte[] ProductionSetupInvoiceByID(string UserName, int ConcernID, int PSID);
        byte[] ProductionInvoiceByID(string UserName, int ConcernID, int productionID);
        byte[] ProductionReport(DateTime fromDate, DateTime toDate, string userName, int concernID, int ProductID);
        byte[] SRWiseSaleCollectionReport(DateTime fromDate, DateTime toDate, string userName, int concernID, int EmployeeID, int RptType);
        byte[] BankAndCashAccountLedger(DateTime fromDate, DateTime toDate, string userName, int concernID, int ExpenseItemID, string headType);
        byte[] ReceiptPaymentReport(string userName, int concernId, DateTime fromDate, DateTime toDate);
        byte[] CompanyIncomeStatementReport(DateTime fromDate, DateTime toDate, int ConcernID, string userName);

        byte[] VoucherTransactionLedger(DateTime fromDate, DateTime toDate, string userName, int concernID, int ExpenseItemID, string headType);
    }
}
