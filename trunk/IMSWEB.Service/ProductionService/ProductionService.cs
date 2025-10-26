using IMSWEB.Data;
using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Service
{
    public class ProductionService : IProductionService
    {
        private readonly IBaseRepository<Production> _baseRepository;
        private readonly IBaseRepository<ProductionDetail> _PDetailRepository;
        private readonly IBaseRepository<StockDetail> _stockDetailepository;
        private readonly IProductionRepository _ProductionRepository;
        private readonly IBaseRepository<Product> _productRepository;
        private readonly IBaseRepository<ProductionRawMaterial> _pRawRepository;
        private readonly IUnitOfWork _unitOfWork;


        public ProductionService(IBaseRepository<Production> baseRepository, IUnitOfWork unitOfWork,
            IBaseRepository<ProductionDetail> PSDetailRepository, IProductionRepository PSRepository,
            IBaseRepository<StockDetail> stockDetailRepository, IBaseRepository<Product> productRepository, IBaseRepository<ProductionRawMaterial> pRawRepository)
        {
            _unitOfWork = unitOfWork;
            _baseRepository = baseRepository;
            _PDetailRepository = PSDetailRepository;
            _ProductionRepository = PSRepository;
            _stockDetailepository = stockDetailRepository;
            _productRepository = productRepository;
            _pRawRepository = pRawRepository;
        }

        public Tuple<bool, int> Add(Production Production, int ProductionID)
        {
            return _ProductionRepository.AddProduction(Production, ProductionID);
        }

        public Tuple<bool, int> AddManual(Production Production, int ProductionID)
        {
            return _ProductionRepository.AddManualProduction(Production, ProductionID);
        }

        public void Update(Production Production)
        {
            _baseRepository.Update(Production);
        }

        public void Save()
        {
            _unitOfWork.Commit(); ;
        }

        public IQueryable<Production> GetAll()
        {
            return _baseRepository.All;
        }

        public Production GetById(int id)
        {
            return _baseRepository.FindBy(x => x.ProductionID == id).First();
        }

        public bool Delete(int id, int UserID, DateTime ModifiedDate)
        {
            return _ProductionRepository.DeleteByID(id, UserID, ModifiedDate);
        }

        public IQueryable<ProductionDetail> GetDetailsById(int ProductionID)
        {
            return _PDetailRepository.AllIncluding(/*i => i.ProductionIMEIs*/).Where(x => x.ProductionID == ProductionID);

        }
        public IQueryable<Production> GetAll(DateTime fromDate, DateTime toDate)
        {
            return _baseRepository.All
                .Where(i => i.Date >= fromDate && i.Date <= toDate)
                .OrderByDescending(i => i.Date)
                .ThenByDescending(i => i.ProductionCode);
        }

        public bool HasSoldProductCheckByPDetailID(int PDetailID)
        {
            var DetailQty = _PDetailRepository.All.FirstOrDefault(i => i.PDetailID == PDetailID);
            var StockDetail = _stockDetailepository.All.Count(i => i.PDetailID == PDetailID && i.Status == (int)EnumStockStatus.Stock);
            if (DetailQty.Quantity != StockDetail)
                return true;

            return false;
        }

        public IEnumerable<ProductionReportModel> GetProductionDetailDataByFinProductID(DateTime fromDate, DateTime toDate, int ProductID)
        {
            return _baseRepository.GetProductionDetailDataByFinProductID(_PDetailRepository, _productRepository, _stockDetailepository,
                _pRawRepository, fromDate, toDate, ProductID);
        }
    }
}
