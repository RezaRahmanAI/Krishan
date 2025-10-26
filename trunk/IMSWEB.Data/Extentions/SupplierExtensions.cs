using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Data
{
    public static class SupplierExtensions
    {
        public static async Task<IEnumerable<Supplier>> GetAllSupplierAsync(this IBaseRepository<Supplier> supplierRepository)
        {
            return await supplierRepository.All.ToListAsync();
        }

        public static IEnumerable<Tuple<string, string, string, string, string,decimal>>
        ConcernWiseSupplierDueRpt(this IBaseRepository<Supplier> supplierRepository, int concernID, int supplierId, int reportType)
        {
            if(supplierId>0)
            {
                var oSupplierDueData = (from Sup in supplierRepository.All
                                        where (Sup.ConcernID == concernID && Sup.SupplierID==supplierId)
                                        select new { SupCode = Sup.Code, CusName = Sup.Name, OwnerName = Sup.OwnerName, Sup.ContactNo, Sup.Address, Sup.TotalDue }).ToList();

                return oSupplierDueData.Select(x => new Tuple<string, string, string, string, string, decimal>
                    (
                     x.SupCode,
                     x.CusName,
                     x.OwnerName,
                     x.ContactNo,
                     x.Address,
                     x.TotalDue
                    ));
            }
            else
            {
                var oAllSupplierDueData = (from Sup in supplierRepository.All
                                        where (Sup.ConcernID == concernID)
                                        select new { SupCode = Sup.Code, CusName = Sup.Name, OwnerName = Sup.OwnerName, Sup.ContactNo, Sup.Address, Sup.TotalDue }).ToList();

                return oAllSupplierDueData.Select(x => new Tuple<string, string, string, string, string, decimal>
                    (
                     x.SupCode,
                     x.CusName,
                     x.OwnerName,
                     x.ContactNo,
                     x.Address,
                     x.TotalDue
                    ));
            }

        }

    }
}
