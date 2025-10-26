using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Service
{
    public interface IExpenditureService
    {
        void AddExpenditure(Expenditure expenditure);
        void UpdateExpenditure(Expenditure expenditure);
        void SaveExpenditure();
        Task<IEnumerable<Expenditure>> GetAllExpenditureByUserIDAsync(int UserID, DateTime fromDate, DateTime toDate);
        Task<IEnumerable<Expenditure>> GetAllExpenditureAsync(DateTime fromDate, DateTime toDate);
        IEnumerable<Tuple<DateTime, string, string, decimal, string, string>> GetforExpenditureReport(DateTime fromDate, DateTime toDate, int concernId, int reportType, int ExpenseItemID);

        Expenditure GetExpenditureById(int id);
        void DeleteExpenditure(int id);
        Task<IEnumerable<Expenditure>> GetAllIncomeAsync( DateTime fromDate, DateTime toDate);
        decimal GetExpenditureAmountByUserID(int UserID, DateTime fromDate, DateTime toDate);
        IQueryable<Expenditure> GetAllExpense();
    }
}
