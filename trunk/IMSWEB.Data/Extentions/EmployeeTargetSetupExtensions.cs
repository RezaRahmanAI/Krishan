using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
namespace IMSWEB.Data
{
    public static class EmployeeTargetSetupExtensions
    {
        public static async Task<IEnumerable<Tuple<int, DateTime, decimal, decimal, decimal, decimal, int, Tuple<string>>>> GetAllAsync(this IBaseRepository<EmployeeTargetSetup> EmployeeTargetSetupRepository,
            IBaseRepository<Employee> EmployeeRepository
            )
        {
            var result = await (from com in EmployeeTargetSetupRepository.All
                                join emp in EmployeeRepository.All on com.EmployeeID equals emp.EmployeeID
                                select new
                                {
                                    com.ETSID,
                                    com.TargetMonth,
                                    com.TargetAmt,
                                    AmtTo = 0m,
                                    Commission = 0m,
                                    emp.Name
                                }).ToListAsync();

            return result.Select(x => new Tuple<int, DateTime, decimal, decimal, decimal, decimal, int, Tuple<string>>(
                x.ETSID,
                x.TargetMonth,
                x.TargetAmt,
                x.AmtTo,
                x.Commission,
                0,
                0, new Tuple<string>(x.Name)
                ));
        }
    }
}
