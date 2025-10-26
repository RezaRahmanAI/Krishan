using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Service
{
    public interface ISizeService
    {
        void Add(Size Size);
        void Update(Size Size);
        void Save();
        IQueryable<Size> GetAll();
        //Task<IEnumerable<Size>> GetAllAsync();
        Size GetById(int id);
        void Delete(int id);
        IQueryable<Size> GetAllIQueryable();

        IQueryable<Size> GetAllIQueryable(int ConcernID);
        //IQueryable<Category> GetAllIQueryable(int ConcernID);


        IEnumerable<Size> GetAllSize();
    }
}
