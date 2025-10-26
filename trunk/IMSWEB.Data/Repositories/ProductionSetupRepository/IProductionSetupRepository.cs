using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Data
{
    public interface IProductionSetupRepository
    {
        Tuple<bool, int> ADDPS(ProductionSetup newProductionSetup, int PSID);
        bool DeleteByID(int PSID);
    }
}
