using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Service
{
    public interface IDiaSizeService
    {
        void Add(DiaSize DiaSize);
        void Update(DiaSize DiaSize);
        void Save();
        IQueryable<DiaSize> GetAll();
        DiaSize GetById(int id);
        void Delete(int id);
        IQueryable<DiaSize> GetAllIQueryable();
        IQueryable<DiaSize> GetAllIQueryable(int ConcernID);
        IEnumerable<DiaSize> GetAllDiaSize();
    }
}
