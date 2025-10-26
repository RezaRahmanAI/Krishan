using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Service
{
    public interface IEmployeeTargetSetupService
    {
        void Add(EmployeeTargetSetup EmployeeTargetSetup);
        void Update(EmployeeTargetSetup EmployeeTargetSetup);
        void Save();
        IQueryable<EmployeeTargetSetup> GetAll();
        EmployeeTargetSetup GetById(int id);
        void Delete(int id);
        IQueryable<EmployeeTargetSetup> GetAll(DateTime FromDate, DateTime ToDate);
        IQueryable<EmployeeTargetSetup> GetByEmployeeIDandMonth(int EmployeeID, DateTime fromDate, DateTime toDate);
        Task<IEnumerable<Tuple<int, DateTime, decimal, decimal, decimal, decimal, int, Tuple<string>>>> GetAllAsync();
        EmployeeTargetSetup GetByEmployeeIDandTargetMonth(int EmployeeID, DateTime fromDate, DateTime toDate);
    }
}
