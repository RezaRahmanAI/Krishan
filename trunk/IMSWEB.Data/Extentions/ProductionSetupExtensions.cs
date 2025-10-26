using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Data
{
    public static class ProductionSetupExtensions
    {
        public static async Task<IEnumerable<Tuple<string, int>>> GetAllProductionSetupAsync(this IBaseRepository<ProductionSetup> productionSetupRepository,
            IBaseRepository<Product> productRepository)
        {
            var items = await (from PS in productionSetupRepository.All
                               join Pro in productRepository.All on PS.FINProductID equals Pro.ProductID
                               select new
                               {
                                   Pro.ProductName,
                                   PS.PSID
                               }).ToListAsync();

            return items.Select(x => new Tuple<string, int>
                (
                    x.ProductName,
                    x.PSID
                )).ToList();
        }
    }
}
