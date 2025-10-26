using IMSWEB.Data;
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
    public class SalesOrderService : ISalesOrderService
    {
        private readonly IBaseRepository<SOrder> _baseSalesOrderRepository;
        private readonly ISalesOrderRepository _salesOrderRepository;
        private readonly IBaseRepository<Customer> _customerRepository;
        private readonly IBaseRepository<Product> _productRepository;
        private readonly IBaseRepository<Size> _sizeRepository;
        private readonly IBaseRepository<Category> _categoryRepository;
        private readonly IBaseRepository<ProductUnitType> _productUnitTypeRepository;
        private readonly IBaseRepository<SOrderDetail> _sOrderDetailRepository;
        private readonly IBaseRepository<POrderDetail> _POrderDetailRepository;
        private readonly IBaseRepository<POProductDetail> _POProductDetailRepository;
        private readonly IBaseRepository<StockDetail> _stockdetailRepository;
        private readonly IBaseRepository<CashCollection> _cashCollectionRepository;
        private readonly IBaseRepository<Employee> _employeeRepository;
        private readonly IBaseRepository<CreditSale> _creditSalesRepository;
        private readonly IBaseRepository<CreditSaleDetails> _creditSalesDetailsRepository;
        //private readonly IBaseRepository<CreditSaleP> _creditSalesDetailsRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBaseRepository<POrder> _PorderRepository;
        private readonly IBaseRepository<Company> _CompanyRepository;
        private readonly IBaseRepository<Category> _CategoryRepository;
        private readonly IBaseRepository<Color> _ColorRepository;
        private readonly IBaseRepository<CreditSalesSchedule> _CreditSalesScheduleRepository;
        private readonly IBaseRepository<ExtraCommissionSetup> _ExtraCommissionSetupRepository;
        private readonly IBaseRepository<SisterConcern> _SisterConcernRepository;
        private readonly IBaseRepository<BankTransaction> _BankTransactionRepository;
        private readonly IBaseRepository<EmployeeTargetSetup> _EmployeeTargetSetupRepository;
        private readonly IBaseRepository<EmployeeWiseCustomerDue> _employeeWiseCustomerDueRepository;


        private readonly IBaseRepository<ApplicationUser> _UserRepository;
        private readonly IBaseRepository<Bank> _BankRepository;
        private readonly IBaseRepository<Supplier> _supplierRepository;
        private readonly IBaseRepository<ShareInvestmentHead> _investmentHeadRepository;
        private readonly IBaseRepository<ExpenseItem> _expenseItemRepository;
        private readonly IBaseRepository<Expenditure> _expenseditureRepository;
        private readonly IBaseRepository<CashAccount> _cAccRepository;
        private readonly IBaseRepository<ShareInvestment> _investmentRepository;
        private readonly IBaseRepository<ROrderDetail> _ROrderDetailRepository;
        private readonly IBaseRepository<ROrder> _RorderRepository;
        IProductService _productService;







        public SalesOrderService(IBaseRepository<SOrder> baseSalesOrderRepository,
            IBaseRepository<Size> sizeRepository,
            ISalesOrderRepository salesOrderRepository, IBaseRepository<Customer> customerRepository, IBaseRepository<SOrderDetail> sorderDetailRepository, IBaseRepository<Product> productRepository,
           IBaseRepository<ProductUnitType> productUnitTypeRepository,
           IBaseRepository<StockDetail> stockdetailRepository, IBaseRepository<CashCollection> cashCollectionRepository, IBaseRepository<POrderDetail> pOrderDetail,
           IBaseRepository<POProductDetail> pOProductDetail, IBaseRepository<Employee> employeeRepository,
           IBaseRepository<POrder> PorderRepository, IBaseRepository<Color> ColorRepository,
           IBaseRepository<Company> CompanyRepository,
           IBaseRepository<Category> CategoryRepository,
           IBaseRepository<CreditSale> creditSalesRepository, IBaseRepository<CreditSaleDetails> creditSalesDetailsRepository,
           IBaseRepository<CreditSalesSchedule> CreditSalesScheduleRepository, IBaseRepository<ExtraCommissionSetup> ExtraCommissionSetupRepository,
           IBaseRepository<SisterConcern> SisterConcernRepository, IBaseRepository<BankTransaction> BankTransactionRepository,
           IBaseRepository<ApplicationUser> UserRepository, IBaseRepository<EmployeeTargetSetup> EmployeeTargetSetupRepository,
           IBaseRepository<Bank> BankRepository, IBaseRepository<EmployeeWiseCustomerDue> employeeWiseCustomerDueRepository, IBaseRepository<Supplier> supplierRepository,
           IBaseRepository<ShareInvestmentHead> investmentHeadRepository, IBaseRepository<ExpenseItem> expenseItemRepository, IBaseRepository<Expenditure> expenseditureRepository, IBaseRepository<CashAccount> cAccRepository, IBaseRepository<ShareInvestment> investmentRepository, IBaseRepository<ROrderDetail> rOrderDetail, IBaseRepository<ROrder> RorderRepository,
           IUnitOfWork unitOfWork, IProductService productService)
        {
            _baseSalesOrderRepository = baseSalesOrderRepository;
            _salesOrderRepository = salesOrderRepository;
            _customerRepository = customerRepository;
            _productRepository = productRepository;
            _sizeRepository = sizeRepository;
            _sOrderDetailRepository = sorderDetailRepository;
            _stockdetailRepository = stockdetailRepository;
            _cashCollectionRepository = cashCollectionRepository;
            _POrderDetailRepository = pOrderDetail;
            _POProductDetailRepository = pOProductDetail;
            _employeeRepository = employeeRepository;
            _creditSalesDetailsRepository = creditSalesDetailsRepository;
            _creditSalesRepository = creditSalesRepository;
            _unitOfWork = unitOfWork;
            _PorderRepository = PorderRepository;
            _CompanyRepository = CompanyRepository;
            _CategoryRepository = CategoryRepository;
            _ColorRepository = ColorRepository;
            _CreditSalesScheduleRepository = CreditSalesScheduleRepository;
            _ExtraCommissionSetupRepository = ExtraCommissionSetupRepository;
            _SisterConcernRepository = SisterConcernRepository;
            _BankTransactionRepository = BankTransactionRepository;
            _UserRepository = UserRepository;
            _BankRepository = BankRepository;
            _productUnitTypeRepository = productUnitTypeRepository;
            _EmployeeTargetSetupRepository = EmployeeTargetSetupRepository;
            _supplierRepository = supplierRepository;
            this._employeeWiseCustomerDueRepository = employeeWiseCustomerDueRepository;
            _investmentHeadRepository = investmentHeadRepository;
            _expenseItemRepository = expenseItemRepository;
            _expenseditureRepository = expenseditureRepository;
            _cAccRepository = cAccRepository;
            _investmentRepository = investmentRepository;
            _ROrderDetailRepository = rOrderDetail;
            _RorderRepository = RorderRepository;
            _productService = productService;
        }

        public async Task<IEnumerable<Tuple<int, string, DateTime, string,
               string, decimal, EnumSalesType, Tuple<string>>>> GetAllSalesOrderAsync(DateTime fromDate, DateTime toDate, List<EnumSalesType> SalesType, bool IsVATManager, int concernID
               , string InvoiceNo, string ContactNo, string CustomerName, string AccountNo)
        {
            return await _baseSalesOrderRepository.GetAllSalesOrderAsync(_customerRepository,
                _SisterConcernRepository, fromDate, toDate, SalesType, IsVATManager, concernID,
                InvoiceNo, ContactNo, CustomerName, AccountNo);
        }

        public async Task<IEnumerable<Tuple<int, string, DateTime, string, string, decimal, EnumSalesType, Tuple<string, bool, string>>>> GetAllAdvanceSalesOrderAsync(DateTime fromDate, DateTime toDate, List<EnumSalesType> SalesType, bool IsVATManager, int concernID, string InvoiceNo, string ContactNo, string CustomerName, string AccountNo)
        {
            return await _baseSalesOrderRepository.GetAllAdvanceSalesOrderAsync(_customerRepository,
                _SisterConcernRepository, _employeeRepository, fromDate, toDate, SalesType, IsVATManager, concernID,
                InvoiceNo, ContactNo, CustomerName, AccountNo);
        }


        public async Task<IEnumerable<Tuple<int, string, DateTime, string,
       string, decimal, EnumSalesType, Tuple<string>>>> GetAllSalesOrderAsyncByUserID(int UserID,
           DateTime fromDate, DateTime toDate, EnumSalesType SalesType,
           string InvoiceNo, string ContactNo, string CustomerName, string AccountNo)
        {
            return await _baseSalesOrderRepository.GetAllSalesOrderAsyncByUserID(_customerRepository,
                UserID, fromDate, toDate, SalesType,
                InvoiceNo, ContactNo, CustomerName, AccountNo);
        }

        public async Task<IEnumerable<Tuple<int, string, DateTime, string, string, decimal, EnumSalesType, Tuple<string, bool>>>> GetAllAdvanceSalesOrderAsyncByUserID(int UserID, DateTime fromDate, DateTime toDate, List<EnumSalesType> SalesType, string InvoiceNo, string ContactNo, string CustomerName, string AccountNo)
        {
            return await _baseSalesOrderRepository.GetAllAdvanceSalesOrderAsyncByUserID(_customerRepository, _employeeRepository,
                UserID, fromDate, toDate, SalesType,
                InvoiceNo, ContactNo, CustomerName, AccountNo);
        }


        public IQueryable<SOrder> GetAllIQueryable()
        {
            return _baseSalesOrderRepository.All;
        }
        public void AddSalesOrder(SOrder salesOrder)
        {
            _baseSalesOrderRepository.Add(salesOrder);
        }

        public bool AddSalesOrderUsingSP(DataTable dtSalesOrder, DataTable dtSalesOrderDetail, DateTime RemindDate, int EmployeeID, bool isAdvanceSale = false)
        {
            return _salesOrderRepository.AddSalesOrderUsingSP(dtSalesOrder, dtSalesOrderDetail, RemindDate, EmployeeID, isAdvanceSale);
        }
        public bool ApproveAdvanceSalesUsingSP(int salesOrderId)
        {
            return _salesOrderRepository.ApproveAdvanceSalesUsingSP(salesOrderId);
        }
        public void AddReplacementOrderUsingSP(DataTable dtSalesOrder, DataTable dtSalesOrderDetail)
        {
            _salesOrderRepository.AddReplacementOrderUsingSP(dtSalesOrder, dtSalesOrderDetail);
        }

        public void SaveSalesOrder()
        {
            _unitOfWork.Commit();
        }

        //public SOrder GetSalesOrderById(int id)
        //{
        //    return _baseSalesOrderRepository.FindBy(x => x.SOrderID == id).First();
        //}
        public SOrder GetSalesOrderById(int id)
        {
            //return _baseSalesOrderRepository.FindBy(x => x.SOrderID == id).First();
            return _baseSalesOrderRepository.AllIncluding(d => d.SOrderDetails).Where(x => x.SOrderID == id).First();
        }

        public void DeleteSalesOrder(int id)
        {
            _baseSalesOrderRepository.Delete(x => x.SOrderID == id);
        }

        public IEnumerable<SOredersReportModel> GetforSalesReport(DateTime fromDate, DateTime toDate, int EmployeeID, int CustomerID)
        {
            return _baseSalesOrderRepository.GetforSalesReport(_customerRepository, _employeeRepository, fromDate, toDate, EmployeeID, CustomerID);

        }

        public IEnumerable<Tuple<DateTime, string, string, decimal, decimal, decimal, decimal, Tuple<decimal, decimal, decimal, decimal, decimal, string>>>
           GetforSalesDetailReport(DateTime fromDate, DateTime toDate)
        {
            return _baseSalesOrderRepository.GetforSalesDetailReport(_sOrderDetailRepository, _productRepository, _stockdetailRepository, fromDate, toDate);
        }

        public IEnumerable<SOredersReportModel> GetforSalesDetailReportByMO(DateTime fromDate, DateTime toDate, int MOId)
        {
            return _baseSalesOrderRepository.GetforSalesDetailReportByMO(_sOrderDetailRepository, _productRepository, _stockdetailRepository, _customerRepository, _employeeRepository, fromDate, toDate, MOId);
        }

        public IEnumerable<Tuple<string, string, DateTime, string, decimal, decimal, decimal, Tuple<decimal, decimal, decimal, decimal>>>
            GetSalesReportByConcernID(DateTime fromDate, DateTime toDate, int concernID, int CustomerType)
        {
            //commented to skip error
            return _baseSalesOrderRepository.GetSalesReportByConcernID(_customerRepository, _sOrderDetailRepository, fromDate, toDate, concernID, CustomerType);

        }

        public IEnumerable<ProductWiseSalesReportModel> GetSalesDetailReportByConcernID(DateTime fromDate, DateTime toDate, int concernID)
        {
            return _baseSalesOrderRepository.GetSalesDetailReportByConcernID(_sOrderDetailRepository, _productRepository, _sizeRepository,
                _productUnitTypeRepository, _stockdetailRepository, _CategoryRepository, fromDate, toDate, concernID);
        }
        public IEnumerable<ProductWiseSalesReportModel> GetSalesDetailReportAdminByConcernID(DateTime fromDate, DateTime toDate, int concernID, int CustomerType)
        {
            return _baseSalesOrderRepository.GetSalesDetailReportAdminByConcernID(_sOrderDetailRepository, _productRepository, _sizeRepository,
                _productUnitTypeRepository, _stockdetailRepository, _CategoryRepository, _SisterConcernRepository, _customerRepository, fromDate, toDate, concernID, CustomerType);
        }

        public IEnumerable<SOredersReportModel> GetSalesDetailReportByCustomerID(DateTime fromDate, DateTime toDate, int customerID)
        {
            return _baseSalesOrderRepository.GetSalesDetailReportByCustomerID(_sOrderDetailRepository, _productRepository, _stockdetailRepository, _ColorRepository, fromDate, toDate, customerID);
        }

        public IEnumerable<Tuple<string, DateTime, string, string, decimal, decimal, Tuple<decimal, decimal, decimal, decimal>>>
        GetSalesDetailReportByMOID(DateTime fromDate, DateTime toDate, int concernId, int MOID, int RptType)
        {
            return _baseSalesOrderRepository.GetSalesDetailReportByMOID(_customerRepository, _employeeRepository, fromDate, toDate, concernId, MOID, RptType);
        }

        public IEnumerable<Tuple<string, string, string, string, string, decimal, decimal>>
        GetMOWiseCustomerDueRpt(int concernId, int MOID, int RptType)
        {
            return _baseSalesOrderRepository.GetMOWiseCustomerDueRpt(_customerRepository, _employeeRepository, _employeeWiseCustomerDueRepository, concernId, MOID, RptType);
        }
        public IEnumerable<Tuple<DateTime, string, string, decimal, decimal>> GetSalesByProductID(DateTime fromDate, DateTime toDate, int ConcernId, int productID)
        {
            return _baseSalesOrderRepository.GetSalesByProductID(_sOrderDetailRepository, _productRepository, _creditSalesRepository, _creditSalesDetailsRepository, fromDate, toDate, ConcernId, productID);
        }

        public bool UpdateSalesOrderUsingSP(int userId, int salesOrderId, DataTable dtSalesOrder, DataTable dtSODetail, int EmployeeID, bool isAdvanceSale = false)
        {
            return _salesOrderRepository.UpdateSalesOrderUsingSP(userId, salesOrderId, dtSalesOrder, dtSODetail, EmployeeID, isAdvanceSale);
        }

        public void DeleteSalesOrderUsingSP(int orderId, int userId)
        {
            _salesOrderRepository.DeleteSalesOrderUsingSP(orderId, userId);
        }

        public void DeleteAdvanceSalesOrderUsingSP(int orderId, int userId)
        {
            _salesOrderRepository.DeleteAdvanceSalesOrderUsingSP(orderId, userId);
        }

        public void DeleteSalesOrderDetailUsingSP(int orderId, int userId)
        {
            _salesOrderRepository.DeleteSalesOrderDetailUsingSP(orderId, userId);
        }

        public void CorrectionStockData(int ConcernId)
        {
            _salesOrderRepository.CorrectionStockData(ConcernId);
        }
        public List<SRWiseCustomerSalesSummaryVM> SRWiseCustomerSalesSummary(DateTime fromdate, DateTime todate, int ConcernID, int EmployeeID)
        {
            return _salesOrderRepository.SRWiseCustomerSalesSummary(fromdate, todate, ConcernID, EmployeeID);
        }

        public List<CustomerLedgerModel> CustomerLedger(DateTime fromdate, DateTime todate, int ConcernID, int CustomerID)
        {
            return _salesOrderRepository.CustomerLedger(fromdate, todate, ConcernID, CustomerID);
        }
        public List<LedgerAccountReportModel> CustomerLedger(DateTime fromdate, DateTime todate, int CustomerID)
        {
            return _baseSalesOrderRepository.CustomerLedger(_sOrderDetailRepository, _customerRepository, _UserRepository, _BankTransactionRepository,
                _cashCollectionRepository, _creditSalesRepository, _creditSalesDetailsRepository, _CreditSalesScheduleRepository, _RorderRepository, _ROrderDetailRepository, _productRepository, _BankRepository, CustomerID, fromdate, todate);
        }

        public List<CustomerDueReportModel> CustomerDue(DateTime fromdate, DateTime todate, int ConcernID, int CustomerID, int IsOnlyDue)
        {
            return _salesOrderRepository.CustomerDue(fromdate, todate, ConcernID, CustomerID, IsOnlyDue);
        }


        public async Task<IEnumerable<Tuple<int, string, DateTime, string, string, decimal, EnumSalesType>>> GetReplacementOrdersByAsync(int EmployeeID)
        {
            return await _baseSalesOrderRepository.GetReplacementOrdersAsync(_sOrderDetailRepository, _customerRepository, EmployeeID);
        }

        public async Task<IEnumerable<Tuple<int, string, DateTime, string, string, decimal, EnumSalesType>>> GetReturnOrdersByAsync()
        {
            return await _baseSalesOrderRepository.GetReturnOrdersAsync(_sOrderDetailRepository, _customerRepository);
        }

        public List<ReplaceOrderDetail> GetReplaceOrderInvoiceReportByID(int OrderID)
        {
            return _baseSalesOrderRepository.GetReplaceOrderInvoiceReportByID(_sOrderDetailRepository, _stockdetailRepository, _productRepository, OrderID);
        }


        public bool AddReturnOrderUsingSP(DataTable dtSalesOrder, DataTable dtSalesOrderDetail)
        {
            return _salesOrderRepository.AddReturnOrderUsingSP(dtSalesOrder, dtSalesOrderDetail);
        }


        public List<ReplaceOrderDetail> GetReturnOrderInvoiceReportByID(int OrderID)
        {
            return _baseSalesOrderRepository.GetReturnOrderInvoiceReportByID(_sOrderDetailRepository, _stockdetailRepository, _productRepository, OrderID);
        }


        public List<DailyWorkSheetReportModel> DailyWorkSheetReport(DateTime fromdate, DateTime todate, int ConcernID)
        {
            return _salesOrderRepository.DailyWorkSheetReport(fromdate, todate, ConcernID);
        }


        public List<ReplacementReportModel> ReplacementOrderReport(DateTime fromdate, DateTime todate, int ConcernID, int CustomerID)
        {
            return _baseSalesOrderRepository.ReplacementReport(_sOrderDetailRepository, _customerRepository, _productRepository, _stockdetailRepository, _PorderRepository, _POrderDetailRepository, CustomerID, fromdate, todate);
        }


        public List<ReturnReportModel> ReturnOrderReport(DateTime fromdate, DateTime todate, int ConcernID, int CustomerID)
        {
            //return _baseSalesOrderRepository.ReturntReport(_sOrderDetailRepository, _customerRepository, _productRepository, _stockdetailRepository, CustomerID, fromdate, todate);
            return _salesOrderRepository.ReturnReport(fromdate, todate, ConcernID, CustomerID);
        }


        public List<MonthlyBenefitReport> MonthlyBenefitReport(DateTime fromdate, DateTime todate, int ConcernID)
        {
            return _salesOrderRepository.MonthlyBenefitReport(fromdate, todate, ConcernID);
        }


        public List<ProductWiseBenefitModel> ProductWiseBenefitReport(DateTime fromdate, DateTime todate, int ConcernID)
        {
            return _salesOrderRepository.ProductWiseBenefitReport(fromdate, todate, ConcernID);
        }

        public List<ProductWiseSalesReportModel> ProductWiseSalesReport(DateTime fromDate, DateTime toDate, int ConcernID, int CustomerID)
        {
            return _baseSalesOrderRepository.ProductWiseSalesReport(_sOrderDetailRepository, _customerRepository, _employeeRepository, _productRepository, ConcernID, CustomerID, fromDate, toDate);
        }

        public List<ReplacementReportModel> DamageProductReport(DateTime fromdate, DateTime todate, int ConcernID, int CustomerID)
        {
            return _baseSalesOrderRepository.DamageReport(_sOrderDetailRepository, _customerRepository, _productRepository, _stockdetailRepository, CustomerID, fromdate, todate);
        }

        public List<ProductWiseSalesReportModel> ProductWiseSalesDetailsReport(int CompanyID, int CategoryID, int ProductID, DateTime fromDate, DateTime toDate)
        {
            return _baseSalesOrderRepository.ProductWiseSalesDetailsReport(_sOrderDetailRepository, _CompanyRepository, _CategoryRepository, _productRepository, _stockdetailRepository, CompanyID, CategoryID, ProductID, fromDate, toDate);
        }


        public SOrder GetLastSalesOrderByCustomerID(int CustomerID)
        {
            return _baseSalesOrderRepository.All.Where(i => i.CustomerID == CustomerID && i.Status == (int)EnumPurchaseType.Purchase).OrderByDescending(i => i.InvoiceDate).FirstOrDefault();
        }
        public decimal GetAllCollectionAmountByDateRange(DateTime fromDate, DateTime toDate)
        {
            return _baseSalesOrderRepository.GetAllCollectionAmountByDateRange(_creditSalesRepository, _CreditSalesScheduleRepository, _cashCollectionRepository, _BankTransactionRepository, fromDate, toDate);
        }
        public decimal GetVoltageStabilizerCommission(DateTime fromDate, DateTime toDate)
        {
            return _baseSalesOrderRepository.GetVoltageStabilizerCommission(_sOrderDetailRepository, _creditSalesRepository, _creditSalesDetailsRepository, _productRepository, _ExtraCommissionSetupRepository, fromDate, toDate);
        }
        public decimal GetExtraCommission(DateTime fromDate, DateTime toDate, int ConcernID)
        {
            return _baseSalesOrderRepository.GetExtraCommission(_sOrderDetailRepository, _creditSalesRepository, _creditSalesDetailsRepository, _productRepository, _ExtraCommissionSetupRepository, fromDate, toDate, ConcernID);
        }
        public bool IsIMEIAlreadyReplaced(int StockDetailID)
        {
            return _baseSalesOrderRepository.IsIMEIAlreadyReplaced(_sOrderDetailRepository, StockDetailID);
        }

        public List<SOredersReportModel> GetAdminSalesReport(int ConcernID, DateTime fromDate, DateTime toDate)
        {
            return _baseSalesOrderRepository.GetAdminSalesReport(_sOrderDetailRepository, _customerRepository, _SisterConcernRepository, ConcernID, fromDate, toDate);
        }


        public async Task<IEnumerable<Tuple<int, string, DateTime, string, string, decimal, EnumSalesType>>>
            GetAllSalesOrderAsync(DateTime fromDate, DateTime toDate, EnumSalesType SalesType, int ConcernID)
        {
            return await _baseSalesOrderRepository.GetAllSalesOrderAsync(_customerRepository, fromDate, toDate, SalesType, ConcernID);
        }

        public decimal GetEmployeeTragetCommission(DateTime fromDate, DateTime toDate, int ConcernID, int EmployeeID)
        {
            return _baseSalesOrderRepository.GetEmployeeTragetCommission(_sOrderDetailRepository, _productRepository, _EmployeeTargetSetupRepository, fromDate, toDate, ConcernID, EmployeeID);
        }
        public IEnumerable<SOredersReportModel> GetSalesDetailReportBySRID(DateTime fromDate, DateTime toDate, int concernId, int MOID, int RptType)
        {
            return _baseSalesOrderRepository.GetSalesReportBySRID(_customerRepository, _employeeRepository, _creditSalesRepository, fromDate, toDate, concernId, MOID, RptType);
        }

        public IEnumerable<SOredersReportModel> GetProductSalesDetailReportBySRID(DateTime fromDate, DateTime toDate, int concernId, int MOID, int ProductId, int RptType)
        {
            return _baseSalesOrderRepository.GetProductSalesReportBySRID(_productRepository, _categoryRepository, _productUnitTypeRepository, _employeeRepository, _creditSalesRepository, _sOrderDetailRepository, _customerRepository, fromDate, toDate, concernId, MOID, ProductId, RptType);
        }

        public List<VoucherTransactionReportModel> BankAndCashAccountLedgerData(DateTime fromDate, DateTime toDate, int ConcernID, int ExpenseItemID, string headType)
        {
            return _baseSalesOrderRepository.BankAndCashAccountLedgerData(_customerRepository, _supplierRepository, _expenseItemRepository, _investmentHeadRepository, _BankRepository, _PorderRepository, _cAccRepository, _cashCollectionRepository, _expenseditureRepository, _investmentRepository, _BankTransactionRepository, fromDate, toDate, ConcernID, ExpenseItemID, headType);
        }

        public List<RPTPayRecTO> GetReceiptPaymentReport(DateTime fromDate, DateTime toDate)
        {
            return _baseSalesOrderRepository.GetReceiptPaymentReport(_BankRepository, _customerRepository, _investmentHeadRepository, _supplierRepository, _expenseItemRepository, _cAccRepository, _PorderRepository, _cashCollectionRepository, _expenseditureRepository, _investmentRepository, _BankTransactionRepository, fromDate, toDate);
        }

        public List<SummaryReportModel> GetSummaryReport(DateTime fromDate, DateTime toDate, int ConcernID)
        {
            return _baseSalesOrderRepository.GetSummaryReport(_sOrderDetailRepository, _customerRepository, _BankTransactionRepository,
                _cashCollectionRepository, _creditSalesRepository, _creditSalesDetailsRepository, _CreditSalesScheduleRepository, _productRepository,
                _CategoryRepository, _expenseditureRepository, _expenseItemRepository, fromDate, toDate, ConcernID);
        }
        public bool IsSoReturn(int SoId)
        {
            if (_sOrderDetailRepository.All.Any(i => i.RQuantity > 0 && i.SOrderID == SoId))
                return true;

            return false;
        }
    }
}
