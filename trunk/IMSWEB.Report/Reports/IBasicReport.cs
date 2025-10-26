using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Report
{
    public interface IBasicReport
    {
        Task<byte[]> EmployeeInformationReport(string userName, int concernID);
        byte[] CustomerCategoryWiseDueRpt(string userName, int concernID, int CustomerId,int reportType,int DueType);

        byte[] ConcernWiseSupplierDueRpt(string userName, int concernID, int SupplierId,int reportType);
        byte[] GetCustomerDetails(string userName, int concernID, int CustomerID);

    }
}
