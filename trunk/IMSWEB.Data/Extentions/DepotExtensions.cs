using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Data
{
    public static class DepotExtensions
    {
        public static async Task<IEnumerable<Depot>> GetAllDepotAsync(this IBaseRepository<Depot> DepotRepository)
        {
            return await DepotRepository.All.ToListAsync();
        }
    }
}
