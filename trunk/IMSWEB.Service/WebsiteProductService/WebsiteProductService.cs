using IMSWEB.Data;
using IMSWEB.Model;
using IMSWEB.Model.TO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace IMSWEB.Service
{
    public class WebsiteProductService : IWebsiteProductService
    {
        private readonly IBaseRepository<WebsiteProducts> _websiteProductRepository;
        private readonly IBaseRepository<SisterConcern> _SisterConcernRepository;
        private readonly IUnitOfWork _unitOfWork;

        public WebsiteProductService(IBaseRepository<WebsiteProducts> websiteProductRepository,
            IBaseRepository<SisterConcern> SisterConcernRepository,
            IUnitOfWork unitOfWork)
        {
            _websiteProductRepository = websiteProductRepository;
            _unitOfWork = unitOfWork;
            _SisterConcernRepository = SisterConcernRepository;
        }

        public void Add(WebsiteProducts Model)
        {
            _websiteProductRepository.Add(Model);
        }

        public void Update(WebsiteProducts Model)
        {
            _websiteProductRepository.Update(Model);
        }

        public void SaveWebsitePRoduct()
        {
            _unitOfWork.Commit();
        }

        public bool Save()
        {
            try
            {
                _unitOfWork.Commit();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        public void Delete(int id)
        {
            _websiteProductRepository.Delete(x => x.Id == id);
        }



        public IEnumerable<WebsiteProducts> GetAllProducts()
        {
            return _websiteProductRepository.GetAll();
        }

        public async Task<IEnumerable<WebsiteProducts>> GetAllProductAsync()
        {
            return await _websiteProductRepository.GetAllWebsiteProductAsync();
        }

        public async Task<IEnumerable<WebsiteProducts>> GetAllCategoryProductAsync(int category)
        {
            return await _websiteProductRepository.GetAllCategoryProductAsync(category);
        }

    
          public async Task<IEnumerable<WebsiteProducts>> GetAllNewProductAsync(int category)
        {
            return await _websiteProductRepository.GetAllCategoryProductAsync(category);
        }


        public WebsiteProducts GetById(int id)
        {
            return _websiteProductRepository.FindBy(x => x.Id == id).First();
        }

        public IQueryable<WebsiteProducts> GetAllProductIQueryables(int category)
        {
            return _websiteProductRepository.GetAllProductIQueryables(category);
        }



        public IEnumerable<WebsiteProducts> GetProductAsync()
        {
            return _websiteProductRepository.GetAllProductAsync();
        }

    }
}

