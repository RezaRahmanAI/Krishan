using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Data
{
    public static class TerritoryExtensions
    {
        public static async Task<IEnumerable<Territory>> GetAllTerritoryAsync(this IBaseRepository<Territory> TerritoryRepository)
        {
            return await TerritoryRepository.All.ToListAsync();
        }
    }
}
