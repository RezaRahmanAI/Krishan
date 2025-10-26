using IMSWEB.Data;
using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IMSWEB.Model.TOs;

namespace IMSWEB.Service
{
    public class ProductService : IProductService
    {
        private readonly IBaseRepository<Product> _productRepository;
        private readonly IBaseRepository<Category> _categoryRepository;
        private readonly IBaseRepository<Company> _companyRepository;
        private readonly IBaseRepository<Godown> _godownRepository;
        private readonly IBaseRepository<Color> _colorRepository;
        private readonly IBaseRepository<Stock> _stockRepository;
        private readonly IBaseRepository<StockDetail> _stockDetailRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBaseRepository<SaleOffer> _saleOfferRepository;
        private readonly IBaseRepository<SOrder> _SOrderRepository;
        private readonly IBaseRepository<SOrderDetail> _SOrderDetailRepository;
        private readonly IBaseRepository<SRVisit> _SRVisitRepository;
        private readonly IBaseRepository<SRVisitDetail> _SRVisitDetailRepository;
        private readonly IBaseRepository<SRVProductDetail> _SRVProductDetailRepository;
        private readonly IBaseRepository<POrder> _POrderRepository;
        private readonly IBaseRepository<POrderDetail> _POrderDetailRepository;
        private readonly IBaseRepository<POProductDetail> _POProductDetailRepository;
        private readonly IBaseRepository<CreditSale> _CreditSaleRepository;
        private readonly IBaseRepository<CreditSaleDetails> _CreditSaleDetailsRepository;
        private readonly IBaseRepository<Size> _SizeRepository;
        private readonly IBaseRepository<ProductUnitType> _ProductUnitTypeRepository;
        private readonly IBaseRepository<DiaSize> _DiaSizeRepository;


        public ProductService(IBaseRepository<Product> productRepository, IBaseRepository<Category> categoryRepository,
            IBaseRepository<Company> companyRepository, IBaseRepository<Godown> godownRepository, IBaseRepository<Color> colorRepository,
            IBaseRepository<Stock> stockRepository, IBaseRepository<StockDetail> stockDetailRepository, IBaseRepository<SaleOffer> saleOfferRepository,
            IBaseRepository<SOrder> SOrderRepository, IBaseRepository<SOrderDetail> SOrderDetailRepository, IBaseRepository<SRVisit> SRVisitRepository,
            IBaseRepository<SRVisitDetail> SRVisitDetailRepository, IBaseRepository<SRVProductDetail> SRVProductDetailRepository,
            IBaseRepository<POrder> POrderRepository, IBaseRepository<POrderDetail> POrderDetailRepository, IBaseRepository<POProductDetail> POProductDetailRepository,
            IUnitOfWork unitOfWork, IBaseRepository<CreditSale> CreditSaleRepository, IBaseRepository<CreditSaleDetails> CreditSaleDetailsRepository,
            IBaseRepository<Godown> GodownsRepository, IBaseRepository<Size> SizeRepository, IBaseRepository<ProductUnitType> ProductUnitTypeRepository,
            IBaseRepository<DiaSize> DiaSizeRepository
            )
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _companyRepository = companyRepository;
            _godownRepository = godownRepository;
            _stockRepository = stockRepository;
            _stockDetailRepository = stockDetailRepository;
            _colorRepository = colorRepository;
            _saleOfferRepository = saleOfferRepository;
            _unitOfWork = unitOfWork;
            _SOrderRepository = SOrderRepository;
            _SOrderDetailRepository = SOrderDetailRepository;
            _SRVisitRepository = SRVisitRepository;
            _SRVisitDetailRepository = SRVisitDetailRepository;
            _SRVProductDetailRepository = SRVProductDetailRepository;

            _POrderRepository = POrderRepository;
            _POrderDetailRepository = POrderDetailRepository;
            _POProductDetailRepository = POProductDetailRepository;
            _CreditSaleRepository = CreditSaleRepository;
            _CreditSaleDetailsRepository = CreditSaleDetailsRepository;
            _SizeRepository = SizeRepository;
            _ProductUnitTypeRepository = ProductUnitTypeRepository;
            _DiaSizeRepository = DiaSizeRepository;
        }

        public void AddProduct(Product product)
        {
            _productRepository.Add(product);
        }

        public void UpdateProduct(Product product)
        {
            _productRepository.Update(product);
        }

        public void SaveProduct()
        {
            _unitOfWork.Commit();
        }

        public async Task<IEnumerable<Tuple<int, string, string,
            decimal, string, string, string, Tuple<string, decimal, decimal, decimal>>>> GetAllProductAsync()
        {
            return await _productRepository.GetAllProductAsync(_categoryRepository,
                _companyRepository,_SizeRepository);
        }
        public IQueryable<ProductWisePurchaseModel> GetAllProductIQueryable()
        {
            return _productRepository.GetAllProductIQueryable(_categoryRepository, _companyRepository, _SizeRepository, _stockRepository, _colorRepository, _DiaSizeRepository, _ProductUnitTypeRepository);
        }

        public IQueryable<ProductWisePurchaseModel> GetAllProductIQueryableIndex(EnumProductStockType StockType)
        {
            return _productRepository.GetAllProductIQueryableIndex(_categoryRepository, _companyRepository, _SizeRepository, _stockRepository, _colorRepository, _DiaSizeRepository, _ProductUnitTypeRepository, StockType);
        }

        public IEnumerable<Tuple<int, string, string,
            decimal, string, string, string, Tuple<decimal?, int>>> GetAllProduct()
        {
            return _productRepository.GetAllProduct(_categoryRepository,
                _companyRepository, _stockRepository, _stockDetailRepository, _colorRepository);
        }

        public IEnumerable<Tuple<int, string, string, decimal, string, string, string, Tuple<decimal?, string, decimal, int, int, string, string,
            Tuple<string, string, string, string, string, string, int, Tuple<int, decimal, decimal, decimal, decimal, int>>>>> GetAllProductFromDetail()
        {
            return _productRepository.GetAllProductFromDetail(_categoryRepository,
                _companyRepository, _colorRepository, _stockRepository, _stockDetailRepository, _saleOfferRepository, _godownRepository);
        }

        public IEnumerable<Tuple<int, string, string, decimal, string, string, string, Tuple<decimal?, string, decimal, int, int, string, string, Tuple<string, string, string, string, string, string, int, Tuple<int, decimal, decimal, decimal, decimal, decimal>>>>> GetAllProductFromDetailById(int productId = 0)
        {
            return _productRepository.GetAllProductFromDetailById(_categoryRepository,
                _companyRepository, _colorRepository, _stockRepository, _stockDetailRepository, _saleOfferRepository, _godownRepository, productId);
        }

        public IEnumerable<Tuple<int, string, string, decimal, string, string, string, Tuple<decimal?, string, decimal, int, int, string, string,
            Tuple<string, string, string, string, string>>>> GetAllDamageProductFromDetail()
        {
            return _productRepository.GetAllDamageProductFromDetail(_categoryRepository,
                _companyRepository, _colorRepository, _stockRepository, _stockDetailRepository, _saleOfferRepository, _SOrderDetailRepository, _SOrderRepository, _POrderRepository, _POrderDetailRepository, _POProductDetailRepository);
        }
        public IEnumerable<Tuple<int, string, string, decimal, string, string, string, Tuple<decimal?, string, decimal, int, int, string, string, Tuple<decimal, string, string, string, string, string, decimal, Tuple<string>>>>>
            GetAllProductFromDetailForCredit()
        {
            return _productRepository.GetAllProductFromDetailForCredit(_categoryRepository,
                _companyRepository, _colorRepository, _stockRepository, _stockDetailRepository, _saleOfferRepository, _godownRepository);
        }

        public IEnumerable<Tuple<int, string, string,
         decimal, string, string, string, Tuple<decimal?, string, decimal, int, int, string, string, Tuple<string>>>> GetAllSalesProductFromDetailByCustomerID(int CustomerID)
        {
            return _productRepository.GetAllSalesProductFromDetailByCustomerID(_categoryRepository,
                _companyRepository, _colorRepository, _stockRepository, _stockDetailRepository, _saleOfferRepository, _SOrderRepository, _SOrderDetailRepository, _godownRepository, CustomerID);
        }

        //public IEnumerable<Tuple<int, string, string,
        //  decimal, string, string, string, Tuple<decimal?, string, decimal, int, int, string, string>>> GetAllSalesProductFromDetailByCustomerID(int CustomerID)
        //{
        //    return _productRepository.GetAllSalesProductFromDetail(_categoryRepository,
        //        _companyRepository, _colorRepository, _stockRepository, _stockDetailRepository, _saleOfferRepository);
        //}
        public Product GetProductById(int id)
        {
            return _productRepository.FindBy(x => x.ProductID == id).First();
        }

        public void DeleteProduct(int id)
        {
            _productRepository.Delete(x => x.ProductID == id);
        }


        public IEnumerable<Tuple<int, string, string, decimal, string, string, string, Tuple<decimal?, string, decimal, int, int, string, string>>> SRWiseGetAllProductFromDetail(int EmployeeID)
        {
            return _productRepository.SRWiseGetAllProductFromDetail(_categoryRepository,
                _companyRepository, _colorRepository, _stockRepository, _stockDetailRepository, _saleOfferRepository, _SRVisitRepository, _SRVisitDetailRepository, _SRVProductDetailRepository, EmployeeID);
        }

        public IEnumerable<Tuple<int, string, string, string, string>> GetProductDetails()
        {
            return _productRepository.GetProductDetail(_categoryRepository, _companyRepository);
        }

        public IEnumerable<ProductWisePurchaseModel> GetAllSalesProductByCustomerID(int CustomerID)
        {
            return _productRepository.GetAllSalesProductByCustomerID(_categoryRepository,
                _companyRepository, _colorRepository, _stockRepository, _stockDetailRepository, _saleOfferRepository, _SOrderRepository,
                _SOrderDetailRepository, _CreditSaleRepository, _CreditSaleDetailsRepository, _godownRepository, _ProductUnitTypeRepository, _SizeRepository, CustomerID);
        }
        public IEnumerable<ProductWisePurchaseModel> GetSalesDetailByCustomerIDNew(int CustomerID, string IMEI)
        {
            return _productRepository.GetSalesDetailByCustomerIDNew(_categoryRepository,
                _companyRepository, _colorRepository, _stockRepository, _stockDetailRepository, _saleOfferRepository, _SOrderRepository,
                _SOrderDetailRepository, _godownRepository, _ProductUnitTypeRepository, _SizeRepository, CustomerID, IMEI);
        }

        public IEnumerable<Tuple<int, string, string, decimal, string, string, string, Tuple<decimal?, string, decimal, decimal, int, string, int>>> GetAllProductDetails()
        {
            return _productRepository.GetAllProductDetails(_categoryRepository, _companyRepository, _stockRepository, _godownRepository);
        }


        public IQueryable<ProductWisePurchaseModel> GetProducts()
        {
            return _productRepository.GetProducts(_categoryRepository, _companyRepository,_SizeRepository,_ProductUnitTypeRepository);
        }
        public IQueryable<ProductWisePurchaseModel> GetProductDetailsBySDetailID(int StockDetailID)
        {
            return _productRepository.GetProductDetailsBySDetailID(_categoryRepository, _companyRepository, _SizeRepository, _stockDetailRepository, _colorRepository, _ProductUnitTypeRepository, StockDetailID);
        }

        public Product GetProductByConcernAndName(int ConcernID, string ProductName)
        {
            return _productRepository.GetAll().FirstOrDefault(i => i.ConcernID == ConcernID && i.ProductName.ToLower().Equals(ProductName.ToLower()));
        }
        public List<TOProduct> GetAllProductNew(int concernId, int productId = 0)
        {

            string query = string.Format(@"SELECT ProductID Id, code, ProductName, CategoryID, CompnayID, SizeID,  FROM Products WHERE Concernid = {0}", concernId);
            if (productId > 0)
            {
                query = string.Format(@"SELECT ProductID Id, code, ProductName, CategoryID, CompnayID, SizeID,  FROM Products WHERE ConcernId = {0} AND ProductID = {1}", concernId, productId);
            }
            return _productRepository.SQLQueryList<TOProduct>(query).ToList();
        }

        public IQueryable<ProductWisePurchaseModel> GetAllProductIQueryableForProduction(EnumProductStockType StockType)
        {
            return _productRepository.GetAllProductIQueryableForProduction(_categoryRepository, _companyRepository, _SizeRepository, _stockRepository, _colorRepository, _DiaSizeRepository, _ProductUnitTypeRepository, StockType);
        }
        public IQueryable<ProductWisePurchaseModel> GetAllProductIQueryableForProduction()
        {
            return _productRepository.GetAllProductIQueryableForProduction(_categoryRepository, _companyRepository, _SizeRepository, _stockRepository, _colorRepository, _DiaSizeRepository, _ProductUnitTypeRepository, 0);
        }
    }
}
