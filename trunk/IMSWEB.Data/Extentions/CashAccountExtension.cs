using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Data
{
    public static class CashAccountExtensions
    {
        public static async Task<IEnumerable<CashAccount>> GetAllCashAccountAsync(this IBaseRepository<CashAccount> cashAccRepository)
        {
            return await cashAccRepository.All.ToListAsync();
        }

    }
}
