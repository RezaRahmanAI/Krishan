using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Service
{
    public interface IEmployeeWiseCustomerDueService
    {
        void Add(EmployeeWiseCustomerDue EmployeeWiseCustomerDue);
        void Update(EmployeeWiseCustomerDue EmployeeWiseCustomerDue);
        void Save();
        IQueryable<EmployeeWiseCustomerDue> GetAll();
        //Task<IEnumerable<EmployeeWiseCustomerDue>> GetAllAsync();
        IQueryable<EmployeeWiseCustomerDue> Get(int? id = null, int? employeeID = null, int? customerID = null);
        EmployeeWiseCustomerDue GetByEmpCustomer(int? employeeID, int customerID);
        void DeleteByEmplyeeID(int employeeID);
        void DeleteByID(int id);
        List<Tuple<int, int, string, string, string,
                int, string, Tuple<string, string, decimal, decimal>>> GetEmployeeCustomerData(int? employeeID = null, int? customerID = null);
        TOSRDueDetails GetSRDueDetails(int customerId, int? employeeId);
    }
}
