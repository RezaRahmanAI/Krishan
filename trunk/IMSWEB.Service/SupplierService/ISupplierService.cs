using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IMSWEB.Model.TOs;

namespace IMSWEB.Service
{
    public interface ISupplierService
    {
        void AddSupplier(Supplier Supplier);
        void UpdateSupplier(Supplier Supplier);
        void SaveSupplier();
        IEnumerable<Supplier> GetAllSupplier();
        Task<IEnumerable<Supplier>> GetAllSupplierAsync();

        IEnumerable<Tuple<string, string, string, string, string,decimal>>
        ConcernWiseSupplierDueRpt(int nConcernId,int nSupplierId,int nReportType);

        Supplier GetSupplierById(int id);
        void DeleteSupplier(int id);
        List<TOSupplier> GetAllSupplierNew(int concernId, int supplierId = 0);
        IQueryable<Supplier> GetAllIQueryable();
    }
}
