using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Service
{
    public interface IProductionSetupService
    {
        Tuple<bool, int> ADDPS(ProductionSetup ProductionSetup, int PSID);
        void Update(ProductionSetup ProductionSetup);
        void Save();
        IQueryable<ProductionSetup> GetAll();
        //Task<IEnumerable<ProductionSetup>> GetAllAsync();
        Task<IEnumerable<Tuple<string, int>>> GetAllProductionSetupAsync();
        ProductionSetup GetById(int id);
        bool Delete(int id);
        IQueryable<ProductionSetupDetail> GetDetailsById(int PSID);
        ProductionSetup GetByID(int productID);
    }
}
