using IMSWEB.Data;
using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IMSWEB.Model.TOs;

namespace IMSWEB.Service
{
    public class SupplierService : ISupplierService
    {
        private readonly IBaseRepository<Supplier> _supplierRepository;
        private readonly IUnitOfWork _unitOfWork;

        public SupplierService(IBaseRepository<Supplier> supplierRepository, IUnitOfWork unitOfWork)
        {
            _supplierRepository = supplierRepository;
            _unitOfWork = unitOfWork;
        }

        public void AddSupplier(Supplier Supplier)
        {
            _supplierRepository.Add(Supplier);
        }

        public void UpdateSupplier(Supplier Supplier)
        {
            _supplierRepository.Update(Supplier);
        }

        public void SaveSupplier()
        {
            _unitOfWork.Commit();
        }

        public IEnumerable<Supplier> GetAllSupplier()
        {
            return _supplierRepository.All.ToList();
        }

        public async Task<IEnumerable<Supplier>> GetAllSupplierAsync()
        {
            return await _supplierRepository.GetAllSupplierAsync();
        }

        public Supplier GetSupplierById(int id)
        {
            return _supplierRepository.FindBy(x=>x.SupplierID == id).First();
        }

        public IEnumerable<Tuple<string, string, string, string, string,decimal>>
        ConcernWiseSupplierDueRpt(int concernId, int nSupplierId, int nReportType)
        {
            return _supplierRepository.ConcernWiseSupplierDueRpt(concernId,nSupplierId,nReportType);
        }

        public void DeleteSupplier(int id)
        {
            _supplierRepository.Delete(x => x.SupplierID == id);
        }

        public List<TOSupplier> GetAllSupplierNew(int concernId, int supplierId = 0)
        {

            string query = string.Format(@"SELECT SupplierID Id, code, Name, ContactNo, Address, TotalDue, OwnerName FROM Suppliers WHERE Concernid = {0}", concernId);
            if (supplierId > 0)
            {
                query = string.Format(@"SELECT SupplierID Id, code, Name, ContactNo, Address, TotalDue, OwnerName FROM Suppliers WHERE ConcernId = {0} AND SupplierID = {1}", concernId, supplierId);
            }
            return _supplierRepository.SQLQueryList<TOSupplier>(query).ToList();
        }

        public IQueryable<Supplier> GetAllIQueryable()
        {
            return _supplierRepository.All;
        }
    }
}
