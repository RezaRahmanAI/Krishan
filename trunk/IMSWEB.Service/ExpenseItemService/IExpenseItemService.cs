using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IMSWEB.Model.TOs;

namespace IMSWEB.Service
{
    public interface IExpenseItemService
    {
        void AddExpenseItem(ExpenseItem ExpenseItem);
        void UpdateExpenseItem(ExpenseItem ExpenseItem);
        void SaveExpenseItem();
        IEnumerable<ExpenseItem> GetAllExpenseItem();
        Task<IEnumerable<ExpenseItem>> GetAllExpenseItemAsync();
        ExpenseItem GetExpenseItemById(int id);
        void DeleteExpenseItem(int id);
        List<TOCustomer> GetAllExpenseItemNew(int concernId, int expenseItemId = 0);
        List<TOCustomer> GetAllIncomeItemNew(int concernId, int expenseItemId = 0);
        IQueryable<ExpenseItem> GetAll();
    }
}
