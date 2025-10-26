using IMSWEB.Data;
using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Service
{
    public class CreditSalesOrderService : ICreditSalesOrderService
    {
        private readonly IBaseRepository<CreditSale> _baseSalesOrderRepository;
        private readonly ICreditSalesOrderRepository _CreditSOrderRepository;
        private readonly IBaseRepository<Customer> _customerRepository;
        private readonly IBaseRepository<CreditSaleDetails> _CrditSOrderDetailsRepository;
        private readonly IBaseRepository<CreditSalesSchedule> _CreditSalesScheduleRepository;
        private readonly IBaseRepository<Product> _productRepository;
        private readonly IBaseRepository<Color> _colorRepository;
        private readonly IBaseRepository<StockDetail> _stockDetailRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBaseRepository<Employee> _EmployeeRepository;
        private readonly IBaseRepository<Company> _CompanyRepository;
        private readonly IBaseRepository<Category> _CategoryRepository;
        private readonly IBaseRepository<SisterConcern> _SisterConcernRepository;
        private readonly IBaseRepository<SOrder> _SOrderRepository;
        private readonly IBaseRepository<SOrderDetail> _SOrderDetailRepository;


        public CreditSalesOrderService(IBaseRepository<CreditSale> baseSalesOrderRepository,
            ICreditSalesOrderRepository salesOrderRepository, IBaseRepository<Customer> customerRepository,
            IBaseRepository<CreditSaleDetails> saleOrderDetailsRepository,
            IBaseRepository<CreditSalesSchedule> saleOrderSchedulesRepository,
            IBaseRepository<Product> productRepository, IBaseRepository<Color> colorRepository, IBaseRepository<Employee> EmployeeRepository,
            IBaseRepository<StockDetail> stockDetailRepository, IUnitOfWork unitOfWork,
            IBaseRepository<Company> CompanyRepository, IBaseRepository<Category> CategoryRepository,
            IBaseRepository<SisterConcern> SisterConcernRepository, IBaseRepository<SOrder> SOrderRepository, IBaseRepository<SOrderDetail> SOrderDetailRepository)
        {
            _baseSalesOrderRepository = baseSalesOrderRepository;
            _CreditSOrderRepository = salesOrderRepository;
            _customerRepository = customerRepository;
            _CrditSOrderDetailsRepository = saleOrderDetailsRepository;
            _CreditSalesScheduleRepository = saleOrderSchedulesRepository;
            _productRepository = productRepository;
            _colorRepository = colorRepository;
            _stockDetailRepository = stockDetailRepository;
            _unitOfWork = unitOfWork;
            _EmployeeRepository = EmployeeRepository;
            _CompanyRepository = CompanyRepository;
            _CategoryRepository = CategoryRepository;
            _SisterConcernRepository = SisterConcernRepository;
            _SOrderRepository = SOrderRepository;
            _SOrderDetailRepository = SOrderDetailRepository;
        }

        public async Task<IEnumerable<Tuple<int, string, DateTime, string,
            string, decimal, EnumSalesType>>> GetAllSalesOrderAsync(DateTime fromDate, DateTime toDate)
        {
            return await _baseSalesOrderRepository.GetAllSalesOrderAsync(_customerRepository, fromDate, toDate);
        }

        public void AddSalesOrder(CreditSale salesOrder)
        {
            _baseSalesOrderRepository.Add(salesOrder);
        }
        public IQueryable<CreditSale> GetAllIQueryable()
        {
            return _baseSalesOrderRepository.All;
        }
        public void AddSalesOrderUsingSP(DataTable dtSalesOrder, DataTable dtSODetail,
            DataTable dtSchedules)
        {
            _CreditSOrderRepository.AddSalesOrderUsingSP(dtSalesOrder, dtSODetail, dtSchedules);
        }

        public void InstallmentPaymentUsingSP(int orderId, decimal installmentAmount, DataTable dtSchedules, decimal LastPayAdjustment)
        {
            _CreditSOrderRepository.InstallmentPaymentUsingSP(orderId, installmentAmount, dtSchedules, LastPayAdjustment);
        }

        public void SaveSalesOrder()
        {
            _unitOfWork.Commit();
        }

        public void UpdateSalesOrder(CreditSale creditSale)
        {
            _baseSalesOrderRepository.Update(creditSale);
        }

        //public CreditSale GetSalesOrderByInvoiceNo(string invoiceNo,int concernID)
        //{
        //    return _baseSalesOrderRepository.FindBy(x => x.InvoiceNo == invoiceNo && x.ConcernID==concernID).First();
        //}

        public CreditSale GetSalesOrderById(int id)
        {
            return _baseSalesOrderRepository.FindBy(x => x.CreditSalesID == id).First();
        }

        public bool HasPaidInstallment(int id)
        {
            return _CreditSalesScheduleRepository.FindBy(x => x.CreditSalesID == id).Any(x => x.PaymentStatus.Equals("Paid"));
        }

        public IEnumerable<CreditSaleDetails> GetSalesOrderDetails(int id)
        {
            return _CrditSOrderDetailsRepository.FindBy(x => x.CreditSalesID == id);
        }

        public IEnumerable<Tuple<int, int, int, int,
            decimal, decimal, decimal, Tuple<decimal,
                string, string, int, string>>> GetCustomSalesOrderDetails(int id)
        {
            return _CrditSOrderDetailsRepository.GetCustomSalesOrderDetails(id, _productRepository,
                _colorRepository, _stockDetailRepository);
        }

        public IEnumerable<CreditSalesSchedule> GetSalesOrderSchedules(int id)
        {
            return _CreditSalesScheduleRepository.FindBy(x => x.CreditSalesID == id);
        }

        public void ReturnSalesOrderUsingSP(int orderId, int userId)
        {
            _CreditSOrderRepository.ReturnSalesOrderUsingSP(orderId, userId);
        }

        public void DeleteSalesOrder(int id)
        {
            _baseSalesOrderRepository.Delete(x => x.CreditSalesID == id);
        }
    
        public IEnumerable<UpcommingScheduleReport> GetUpcomingSchedule(DateTime fromDate, DateTime toDate)
        {
            return _baseSalesOrderRepository.GetUpcomingSchedule(_customerRepository, _CreditSalesScheduleRepository, _productRepository, _CrditSOrderDetailsRepository,_SOrderRepository, _SOrderDetailRepository,fromDate, toDate);
        }
        public IEnumerable<UpcommingScheduleReport> GetScheduleCollection(DateTime fromDate, DateTime toDate, int concernID)
        {
            return _baseSalesOrderRepository.GetScheduleCollection(_customerRepository, _CreditSalesScheduleRepository, fromDate, toDate, concernID);
        }
        public IEnumerable<Tuple<string, string, string, string, DateTime, DateTime, decimal, Tuple<decimal, decimal, decimal, decimal, string, decimal>>> GetCreditCollectionReport(DateTime fromDate, DateTime toDate, int concernID, int CustomerID)
        {
            return _baseSalesOrderRepository.GetCreditCollectionReport(_customerRepository, _CreditSalesScheduleRepository, fromDate, toDate, concernID, CustomerID);
        }

        public IEnumerable<Tuple<string, string, string, decimal, decimal>> GetDefaultingCustomer(DateTime date, int concernID)
        {
            return _baseSalesOrderRepository.GetDefaultingCustomer(_customerRepository, _CreditSalesScheduleRepository, date, concernID);
        }
        public IEnumerable<Tuple<string, string, string, string, DateTime, DateTime, decimal, Tuple<decimal, decimal, decimal, decimal, string, decimal, decimal, Tuple<int, decimal>>>> GetDefaultingCustomer(DateTime fromDate, DateTime toDate, int concernID)
        {
            return _baseSalesOrderRepository.GetDefaultingCustomer(_customerRepository, _CreditSalesScheduleRepository, fromDate, toDate, concernID);
        }

        public void CalculatePenaltySchedules(int ConcernID)
        {
            _CreditSOrderRepository.CalculatePenaltySchedules(ConcernID);
        }

        public void CorrectionStockData(int ConcernId)
        {
            _CreditSOrderRepository.CorrectionStockData(ConcernId);
        }



        public IEnumerable<Tuple<string, string, DateTime, string, decimal, decimal, decimal, Tuple<decimal, decimal, decimal, decimal, decimal, int, string>>>
            GetCreditSalesReportByConcernID(DateTime fromDate, DateTime toDate, int concernID, int CustomerType)
        {
            return _baseSalesOrderRepository.GetCreditSalesReportByConcernID(_customerRepository, _CrditSOrderDetailsRepository, fromDate, toDate, concernID, CustomerType);
        }


        public IEnumerable<Tuple<DateTime, string, string, string, decimal, decimal, decimal, Tuple<decimal, decimal, decimal, decimal, decimal, string, string, Tuple<int>>>> GetCreditSalesDetailReportByConcernID(DateTime fromDate, DateTime toDate, int concernID)
        {
            return _baseSalesOrderRepository.GetCreditSalesDetailReportByConcernID(_CrditSOrderDetailsRepository, _productRepository, _stockDetailRepository, fromDate, toDate, concernID);
        }
        public decimal GetDefaultAmount(int CreditSaleID, DateTime FromDate)
        {
            return _baseSalesOrderRepository.GetDefaultAmount(_CreditSalesScheduleRepository, CreditSaleID, FromDate);
        }

        public List<ProductWiseSalesReportModel> ProductWiseCreditSalesReport(DateTime fromDate, DateTime toDate, int ConcernID, int CustomerID)
        {
            return _baseSalesOrderRepository.ProductWiseCreditSalesReport(_CrditSOrderDetailsRepository, _customerRepository, _EmployeeRepository, _productRepository, ConcernID, CustomerID, fromDate, toDate);
        }

        public List<ProductWiseSalesReportModel> ProductWiseCreditSalesDetailsReport(int CompanyID, int CategoryID, int ProductID, DateTime fromDate, DateTime toDate)
        {
            return _baseSalesOrderRepository.ProductWiseCreditSalesDetailsReport(_CrditSOrderDetailsRepository, _CompanyRepository, _CategoryRepository, _productRepository, _stockDetailRepository, CompanyID, CategoryID, ProductID, fromDate, toDate);
        }


        public void DeleteSchedule(CreditSalesSchedule CreditSalesSchedule)
        {
            _CreditSalesScheduleRepository.Delete(CreditSalesSchedule);
        }
        public void AddSchedule(CreditSalesSchedule CreditSalesSchedule)
        {
            _CreditSalesScheduleRepository.Add(CreditSalesSchedule);
        }
        public void UpdateSchedule(CreditSalesSchedule scheduel)
        {
            _CreditSalesScheduleRepository.Update(scheduel);
        }
        public List<SOredersReportModel> SRWiseCreditSalesReport(int EmployeeID, DateTime fromDate, DateTime toDate)
        {
            return _baseSalesOrderRepository.SRWiseCreditSalesReport(_CreditSalesScheduleRepository, _customerRepository, _EmployeeRepository, EmployeeID, fromDate, toDate);
        }
        public IQueryable<SOredersReportModel> GetAdminCrSalesReport(int ConcernID, DateTime fromDate, DateTime toDate)
        {
            return _baseSalesOrderRepository.GetAdminCrSalesReport(_customerRepository, _SisterConcernRepository, ConcernID, fromDate, toDate);
        }
        public IQueryable<CashCollectionReportModel> AdminInstallmentColllections(int ConcernID, DateTime fromDate, DateTime toDate)
        {
            return _baseSalesOrderRepository.AdminInstallmentColllections(_customerRepository, _SisterConcernRepository, _CreditSalesScheduleRepository, ConcernID, fromDate, toDate);
        }
    }
}
