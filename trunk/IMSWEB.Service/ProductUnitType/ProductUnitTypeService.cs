using IMSWEB.Data;
using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Service
{
    public class ProductUnitTypeService : IProductUnitTypeService
    {
        private readonly IBaseRepository<ProductUnitType> _baseRepository;
        private readonly IUnitOfWork _unitOfWork;
        public ProductUnitTypeService(IBaseRepository<ProductUnitType> baseRepository, IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _baseRepository = baseRepository;
        }
        public void Add(ProductUnitType ProductUnitType)
        {
           _baseRepository.Add(ProductUnitType);
        }
        public void Update(ProductUnitType ProductUnitType)
        {
            _baseRepository.Update(ProductUnitType);
        }
        public void Save()
        {
            _unitOfWork.Commit(); ;
        }
        public IQueryable<ProductUnitType> GetAll()
        {
            return _baseRepository.All.OrderBy(i=>i.Position);
        }
        public IQueryable<ProductUnitType> GetAllActive()
        {
            return _baseRepository.All.Where(i=>i.Status==(int)EnumActiveInactive.Active);
        }
        //public async Task<IEnumerable<ProductUnitType>> GetAllAsync()
        //{
        //    return await _baseRepository.GetAllGradeAsync();
        //}
        public ProductUnitType GetById(int id)
        {
            return _baseRepository.FindBy(x => x.ProUnitTypeID == id).First();

        }
        public void Delete(int id)
        {
            _baseRepository.Delete(x => x.ProUnitTypeID == id);
        }
    }
}
