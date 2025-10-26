using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IMSWEB.Model;
using IMSWEB.Model.TOs;

namespace IMSWEB.Service
{
    public interface ISystemInformationService
    {
        void UpdateSystemInformation(SystemInformation systemInformation);

        void SaveSystemInformation();

        SystemInformation GetSystemInformationById(int id);
        SystemInformation GetSystemInformationByConcernId(int id);

        IQueryable<SystemInformation> GetAllConcernSysInfo();

        List<TOHomeWidget> GetHomeWidgeSales(string dataLength, int concernId);
        List<TOHomeWidget> GetYearlyData(string dataLength, int concernId);

        bool IsEmployeeWiseTransactionEnable();
        List<TOHomeWidget> GetHomeWidgetLoanExpense(string dataLength, int concernId);
        List<decimal> GetFullYearIncome(int concernId);
        List<decimal> GetFullYearExpense(int concernId);
        List<decimal> GetFullYearPurchase(int concernId);
        List<decimal> GetFullYearSale(int concernId);
    }
}
