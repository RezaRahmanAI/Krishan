using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Service
{
    public interface IProductionService
    {
        Tuple<bool, int> Add(Production Production, int PSID);
        void Update(Production Production);
        void Save();
        IQueryable<Production> GetAll();
        IQueryable<Production> GetAll(DateTime fromDate, DateTime toDate);
        Production GetById(int id);
        bool Delete(int id, int UserID, DateTime ModifiedDate);
        IQueryable<ProductionDetail> GetDetailsById(int PSID);
        bool HasSoldProductCheckByPDetailID(int PDetailID);
        IEnumerable<ProductionReportModel> GetProductionDetailDataByFinProductID(DateTime fromDate, DateTime toDate, int ProductID);
        Tuple<bool, int> AddManual(Production Production, int ProductionID);
    }
}
