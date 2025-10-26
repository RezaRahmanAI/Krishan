using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Data
{
    public static class CustomerExtensions
    {
        public static async Task<IEnumerable<Customer>> GetAllCustomerAsync(this IBaseRepository<Customer> customerRepository)
        {
            return await customerRepository.All.ToListAsync();
        }

        public static async Task<IEnumerable<Customer>> GetAllCustomerAsyncByEmpID(this IBaseRepository<Customer> customerRepository, int EmpID)
        {
            return await customerRepository.All.Where(x => x.EmployeeID == EmpID).ToListAsync();
        }

        public static IEnumerable<Customer> GetAllCustomer(this IBaseRepository<Customer> customerRepository)
        {
            return customerRepository.All;
        }

        public static IEnumerable<Customer> GetAllCustomerByEmp(this IBaseRepository<Customer> customerRepository, int EmpID)
        {
            return customerRepository.All.Where(x => x.EmployeeID == EmpID).ToList();
        }

        public static IEnumerable<Customer> GetAllCustomerByEmpNew(this IBaseRepository<Customer> customerRepository, int EmpID, int customerId = 0)
        {
            return customerRepository.All.Where(x => x.EmployeeID == EmpID && (customerId == 0 ? true : x.CustomerID == customerId)).ToList();
        }


        public static IEnumerable<Tuple<string, string, string, string, string, string, decimal, Tuple<string>>>
        CustomerCategoryWiseDueRpt(this IBaseRepository<Customer> customerRepository, int concernID, int customerId, int reportType, int DueType)
        {
            List<Customer> CustomerList = null;
            if (customerId > 0)
                CustomerList = customerRepository.All.Where(i => i.CustomerID == customerId).ToList();
            else
            {
                if (reportType != 0)
                    CustomerList = customerRepository.All.Where(i => i.CustomerType == (EnumCustomerType)reportType).ToList();
                else
                    CustomerList = customerRepository.All.ToList();
            }



            var oCustomerDueData = (from CO in CustomerList
                                    where (DueType == 0 ? true : CO.TotalDue != 0)
                                    select new 
                                    {
                                        CusCode = CO.Code,
                                        CusName = CO.Name,
                                        CusCompany = CO.CompanyName,
                                        CusType = CO.CustomerType.ToString(),
                                        CO.ContactNo,
                                        CO.Address,
                                        CO.TotalDue,
                                        CO.FName
                                    }).ToList();

            return oCustomerDueData.Select(x => new Tuple<string, string, string, string, string, string, decimal,Tuple<string>>
                (
                 x.CusCode,
                 x.CusName,
                 x.CusCompany,
                 x.CusType,
                 x.ContactNo,
                 x.Address,
                 x.TotalDue,new Tuple<string>(x.FName)
                ));
        }

        public static IQueryable<SRWiseCustomerStatusReportModel> AdminCustomerDueReport(this IBaseRepository<Customer> customerRepository,
             IBaseRepository<SisterConcern> SisterConcernRepository,
             int concernID, int CustomerType, int DueType)
        {
            IQueryable<Customer> CustomerList = customerRepository.GetAll();
            if (concernID > 0)
                CustomerList = customerRepository.GetAll().Where(i => i.ConcernID == concernID);

            if (CustomerType != 0)
                CustomerList = CustomerList.Where(i => i.CustomerType == (EnumCustomerType)CustomerType);

            var oCustomerDueData = from CO in CustomerList
                                   join sis in SisterConcernRepository.GetAll() on CO.ConcernID equals sis.ConcernID
                                   where (DueType == 0 ? true : CO.TotalDue != 0)
                                   select new SRWiseCustomerStatusReportModel
                                   {
                                       Code = CO.Code,
                                       Name = CO.Name,
                                       CompanyName = CO.CompanyName,
                                       CustomerType = CO.CustomerType.ToString(),
                                       ContactNo = CO.ContactNo,
                                       Address = CO.Address,
                                       TotalDue = CO.TotalDue,
                                       ConcernName = sis.Name
                                   };
            return oCustomerDueData;
        }

    }
}
