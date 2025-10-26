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
    public class ROrderService : IROrderService
    {
        private readonly IBaseRepository<ROrder> _baseROrderRepository;
        private readonly IBaseRepository<ROrderDetail> _ROrderDetailRepository;
        private readonly IBaseRepository<ROProductDetail> _ROProductDetailRepository;

        private readonly IROrderRepository _ROrderRepository;

        private readonly IBaseRepository<Customer> _CustomerRepository;
        private readonly IBaseRepository<Product> _productRepository;
        private readonly IBaseRepository<Color> _colorRepository;
        private readonly IBaseRepository<StockDetail> _stockdetailRepository;
        private readonly IBaseRepository<Employee> _employeeRepository;

        private readonly IBaseRepository<Company> _CompanyRepository;
        private readonly IBaseRepository<Category> _CategoryRepository;

        private readonly IUnitOfWork _unitOfWork;
        private readonly IBaseRepository<ProductUnitType> _ProductUnitTypeRepository;
        private readonly IBaseRepository<Size> _SizeRepository;
        public ROrderService(IBaseRepository<ROrder> baseROrderRepository,
            IROrderRepository returnOrderRepository, IBaseRepository<ROProductDetail> rOProductDetailRepository,
            IBaseRepository<Customer> customerRepository, IBaseRepository<Product> productRepository,
            IBaseRepository<Color> colorRepository, IBaseRepository<ROrderDetail> rOrderDetailRepository, 
            IBaseRepository<StockDetail> stockdetailRepository, IBaseRepository<Employee> employeeRepository, 
            IBaseRepository<Company> CompanyRepository, IBaseRepository<Category> CategoryRepository, IUnitOfWork unitOfWork, 
            IBaseRepository<ProductUnitType> ProductUnitTypeRepository,IBaseRepository<Size> SizeRepository)
        {
            _baseROrderRepository = baseROrderRepository;
            _ROrderRepository = returnOrderRepository;
            _ROrderDetailRepository = rOrderDetailRepository;
            _ROProductDetailRepository = rOProductDetailRepository;
            _CustomerRepository = customerRepository;
            _productRepository = productRepository;
            _colorRepository = colorRepository;
            _stockdetailRepository = stockdetailRepository;
            _employeeRepository = employeeRepository;

            _CompanyRepository = CompanyRepository;
            _CategoryRepository = CategoryRepository;

            _unitOfWork = unitOfWork;
            _ProductUnitTypeRepository = ProductUnitTypeRepository;
            _SizeRepository = SizeRepository;

        }

        public async Task<IEnumerable<Tuple<int, string, DateTime, string,
            string, string>>> GetAllReturnOrderAsync()
        {
            return await _baseROrderRepository.GetAllReturnOrderAsync(_CustomerRepository);
        }

        public void AddReturnOrder(ROrder returnOrder)
        {
            _baseROrderRepository.Add(returnOrder);
        }

        public void AddReturnOrderUsingSP(DataTable dtReturnOrder, DataTable dtRODetail,
            DataTable dtROProductDetail, DataTable dtStock, DataTable dtStockDetail)
        {
            _ROrderRepository.AddReturnOrderUsingSP(dtReturnOrder, dtRODetail,
                dtROProductDetail, dtStock, dtStockDetail);
        }

        public void UpdateReturnOrderUsingSP(int returnOrderId, DataTable dtReturnOrder, DataTable dtRODetail,
            DataTable dtROProductDetail, DataTable dtStock, DataTable dtStockDetail)
        {
            _ROrderRepository.UpdateReturnOrderUsingSP(returnOrderId, dtReturnOrder, dtRODetail,
                dtROProductDetail, dtStock, dtStockDetail);
        }

        public void DeleteReturnOrderDetailUsingSP(int customerId, int rorderDetailId, int productId,
            int colorId, int userId, decimal quantity, decimal totalDue, DataTable dtPOProductDetail)
        {
            _ROrderRepository.DeleteReturnOrderDetailUsingSP(customerId, rorderDetailId, productId,
                colorId, userId, quantity, totalDue, dtPOProductDetail);
        }

        public int CheckProductStatusByROId(int id)
        {
            return _ROrderRepository.CheckProductStatusByROId(id);
        }

        public int CheckProductStatusByRODetailId(int id)
        {
            return _ROrderRepository.CheckProductStatusByRODetailId(id);
        }

        public void SaveReturnOrder()
        {
            _unitOfWork.Commit();
        }

        public ROrder GetReturnOrderById(int id)
        {
            return _baseROrderRepository.FindBy(x => x.ROrderID == id).First();
        }

        public void DeleteReturnOrderUsingSP(int id, int userId)
        {
            _ROrderRepository.DeleteReturnOrderUsingSP(id, userId);
        }

        public IEnumerable<Tuple<string, string, DateTime, string, decimal, decimal, decimal, Tuple<decimal, decimal, decimal, decimal>>>
          GetReturnReportByConcernID(DateTime fromDate, DateTime toDate, int concernID, int CustomerType)
        {
            //commented to skip error
            return _baseROrderRepository.GetReturnReportByConcernID(_CustomerRepository, _ROrderDetailRepository, fromDate, toDate, concernID, CustomerType);

        }

        public IEnumerable<Tuple<DateTime, string, string, string, decimal, decimal, decimal, Tuple<decimal, decimal, decimal, decimal, decimal, string, string, Tuple<int, decimal>>>>
        GetReturnDetailReportByConcernID(DateTime fromDate, DateTime toDate, int concernID)
        {
            return _baseROrderRepository.GetReturnDetailReportByConcernID(_ROrderDetailRepository, _productRepository, _stockdetailRepository, fromDate, toDate, concernID);
        }


        public IEnumerable<Tuple<DateTime, string, string, string, decimal, decimal, decimal, Tuple<decimal, decimal, decimal, decimal, decimal, string, string, Tuple<int, decimal>>>>
        GetReturnDetailReportByEmployee(DateTime fromDate, DateTime toDate, int concernID, int employeeId)
        {
            return _baseROrderRepository.GetReturnDetailReportByEmployee(_ROrderDetailRepository, _productRepository, _stockdetailRepository, _employeeRepository,_CustomerRepository, fromDate, toDate, concernID, employeeId);
        }

        public IEnumerable<Tuple<DateTime, string, string, string, decimal, decimal, decimal, Tuple<decimal, decimal, decimal, decimal, decimal, string, string, Tuple<int, decimal>>>>
       GetReturnDetailReportByReturnID(int ReturnID, int concernID)
        {
            return _baseROrderRepository.GetReturnDetailReportByReturnID(_ROrderDetailRepository, _productRepository, _stockdetailRepository, ReturnID, concernID);
        }



        public List<ProductWiseSalesReportModel> ProductWiseReturnReport(DateTime fromDate, DateTime toDate, int ConcernID, int CustomerID)
        {
            return _baseROrderRepository.ProductWiseReturnReport(_ROrderDetailRepository, _CustomerRepository, _employeeRepository, _productRepository, ConcernID, CustomerID, fromDate, toDate);
        }
        public List<ProductWiseSalesReportModel> ProductWiseReturnDetailsReport(int CompanyID, int CategoryID, int ProductID, DateTime fromDate, DateTime toDate)
        {

            return _baseROrderRepository.ProductWiseReturnDetailsReport(_ROrderDetailRepository, _CompanyRepository, _CategoryRepository, _productRepository, _stockdetailRepository, CompanyID, CategoryID, ProductID, fromDate, toDate);
        }

        public async Task<IEnumerable<Tuple<int, string, DateTime, string, string, decimal, EnumSalesType>>> GetReturnOrdersByAsync()
        {
            return await _baseROrderRepository.GetReturnOrdersAsync(_ROrderDetailRepository, _CustomerRepository);
        }
       

        public Tuple<bool, int> AddReturnOrderUsingSP(DataTable dtSalesOrder, DataTable dtSalesOrderDetail)
        {
            return _ROrderRepository.AddReturnOrderUsingSP(dtSalesOrder, dtSalesOrderDetail);
        }

        public List<ProductWiseSalesReportModel> GetDetailsByReturnID(int RORderID)
        {
            return _baseROrderRepository.SalesReturnDetailsByReturnID(_ROrderDetailRepository, _CompanyRepository, _CategoryRepository, _productRepository,
                _stockdetailRepository, _ProductUnitTypeRepository, _colorRepository, _SizeRepository, RORderID);
        }


    }
}
