using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Data.Extentions
{
    public static class EmployeeWiseCustomerDueExtension
    {
        public static List<Tuple<int, int, string, string, string,
                int, string, Tuple<string, string, decimal, decimal>>>
            GetMappingData(this IBaseRepository<EmployeeWiseCustomerDue> employeeWiseCustomerDueRepo,
            IBaseRepository<Employee> employeeRepsitory, IBaseRepository<Customer> customerRepository,
            IBaseRepository<Designation> designationRepository, IBaseRepository<Department> departmentRepository,
            IBaseRepository<Grade> gradeRepository, int? EmployeeID, int? CustomerID)
        {
            var data = (from ecd in employeeWiseCustomerDueRepo.All
                        join e in employeeRepsitory.All on ecd.EmployeeID equals e.EmployeeID
                        join dn in designationRepository.All on e.DesignationID equals dn.DesignationID into ldn
                        from dn in ldn.DefaultIfEmpty()
                        join c in customerRepository.All on ecd.CustomerID equals c.CustomerID
                        where (EmployeeID.HasValue ? ecd.EmployeeID == EmployeeID : true)
                        && (CustomerID.HasValue ? ecd.CustomerID == CustomerID : true)
                        select new
                        {
                            ecd.ID,
                            ecd.EmployeeID,
                            EmployeeName = e.Name,
                            EmployeeContactNo = e.ContactNo,
                            EmployeeDesignation = dn != null ? dn.Description : "",
                            CustomerID = c.CustomerID,
                            CustomerName = c.Name,
                            CustomerContactNo = c.ContactNo,
                            CustomerDue = ecd.CustomerDue,
                            CustomerAddress = c.Address,
                            CustomerOpeningDue = ecd.CustomerDue
                        }).ToList();


            return data.Select(x => new Tuple<int, int, string, string, string,
                int, string, Tuple<string, string, decimal, decimal>>(
                     x.ID,
                     x.EmployeeID,
                     x.EmployeeName,
                     x.EmployeeContactNo,
                     x.EmployeeDesignation,

                     x.CustomerID,
                     x.CustomerName,

                     new Tuple<string,
                     string, decimal, decimal>(
                     x.CustomerAddress,
                     x.CustomerContactNo,
                     x.CustomerDue,
                     x.CustomerOpeningDue)
                )).ToList();
        }

        public static TOSRDueDetails GetSRDueDetails(this IBaseRepository<EmployeeWiseCustomerDue> employeeWiseCustomerDueRepo,
            IBaseRepository<Employee> employeeRepsitory, IBaseRepository<Customer> customerRepository, int customerId, int? employeeId)
        {
            var data = (from sr in employeeWiseCustomerDueRepo.All
                        join e in employeeRepsitory.All on sr.EmployeeID equals e.EmployeeID
                        join c in customerRepository.All on sr.CustomerID equals c.CustomerID
                        where sr.CustomerID == customerId
                        && (employeeId.HasValue ? sr.EmployeeID == employeeId : true)
                        select new TOSRDueDetails
                        {
                            EmployeeId = sr.EmployeeID,
                            CustomerDue = sr.CustomerDue,
                            EmpName = e.Name
                        }).FirstOrDefault();
            return data;
        }

    }
}
