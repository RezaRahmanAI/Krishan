using IMSWEB.Data;
using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Service
{
    public class ProductionSetupService : IProductionSetupService
    {
        private readonly IBaseRepository<ProductionSetup> _baseRepository;
        private readonly IBaseRepository<ProductionSetupDetail> _PSDetailRepository;
        private readonly IProductionSetupRepository _PSRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBaseRepository<Product> _productRepository;


        public ProductionSetupService(IBaseRepository<ProductionSetup> baseRepository, IUnitOfWork unitOfWork,
            IBaseRepository<ProductionSetupDetail> PSDetailRepository, IProductionSetupRepository PSRepository, IBaseRepository<Product> productRepository)
        {
            _unitOfWork = unitOfWork;
            _baseRepository = baseRepository;
            _PSDetailRepository = PSDetailRepository;
            _PSRepository = PSRepository;
            _productRepository = productRepository;
        }

        public Tuple<bool, int> ADDPS(ProductionSetup ProductionSetup, int PSID)
        {
            return _PSRepository.ADDPS(ProductionSetup, PSID);
        }

        public void Update(ProductionSetup ProductionSetup)
        {
            _baseRepository.Update(ProductionSetup);
        }

        public void Save()
        {
            _unitOfWork.Commit(); ;
        }

        public IQueryable<ProductionSetup> GetAll()
        {
            return _baseRepository.All;
        }

        //public async Task<IEnumerable<ProductionSetup>> GetAllAsync()
        //{
        //    return await _baseRepository.GetAllAsync();
        //}


        public async Task<IEnumerable<Tuple<string, int>>> GetAllProductionSetupAsync()
        {
            return await _baseRepository.GetAllProductionSetupAsync(_productRepository);
        }
        public ProductionSetup GetById(int id)
        {
            return _baseRepository.FindBy(x => x.PSID == id).First();
        }

        public bool Delete(int id)
        {
            return _PSRepository.DeleteByID(id);
        }

        public IQueryable<ProductionSetupDetail> GetDetailsById(int PSID)
        {
            return _PSDetailRepository.All.Where(x => x.PSID == PSID);

        }

        public ProductionSetup GetByID(int productID)
        {
            return _baseRepository.AllIncluding(i => i.ProductionSetupDetails)
                .FirstOrDefault(i => i.FINProductID == productID);
        }
    }
}
