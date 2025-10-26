using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Service
{
    public interface IProductUnitTypeService
    {
        void Add(ProductUnitType ProductUnitType);
        void Update(ProductUnitType ProductUnitType);
        void Save();
        IQueryable<ProductUnitType> GetAll();
        IQueryable<ProductUnitType> GetAllActive();
        //Task<IEnumerable<ProductUnitType>> GetAllAsync();
        ProductUnitType GetById(int id);
        void Delete(int id);
    }
}
