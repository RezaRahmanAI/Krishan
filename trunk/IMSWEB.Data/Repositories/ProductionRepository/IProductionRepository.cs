using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Data
{
    public interface IProductionRepository
    {
        Tuple<bool, int> AddProduction(Production newProductionSetup, int PSID);
        bool DeleteByID(int ProductionID, int UserID, DateTime ModifiedDate);
        Tuple<bool, int> AddManualProduction(Production newProduction, int ProductionID);
    }
}
