using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Data
{
    public static class CCBankExtensions
    {
        public static async Task<IEnumerable<CCBank>> GetAllCCBankAsync(this IBaseRepository<CCBank> CCBankRepository)
        {
            return await CCBankRepository.All.ToListAsync();
        }
    }
}
