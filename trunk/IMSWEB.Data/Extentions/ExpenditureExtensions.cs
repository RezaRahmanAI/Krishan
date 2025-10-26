using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Data
{
    public static class ExpenditureExtensions
    {
        public static async Task<IEnumerable<Expenditure>> GetAllExpenditureAsync(this IBaseRepository<Expenditure> expenditureRepository,
            IBaseRepository<ExpenseItem> expenseItemRepo, DateTime fromDate, DateTime toDate)
        {
            var result = from ei in expenseItemRepo.FindBy(i => i.Status == EnumCompanyTransaction.Expense)
                         join ed in expenditureRepository.All on ei.ExpenseItemID equals ed.ExpenseItemID
                         where ed.EntryDate >= fromDate && ed.EntryDate <= toDate
                         select ed;
            return await result.Include(i=>i.ExpenseItem).OrderByDescending(i => i.EntryDate).ToListAsync();
        }
        public static async Task<IEnumerable<Expenditure>> GetAllExpenditureByUserID(this IBaseRepository<Expenditure> expenditureRepository,
            IBaseRepository<ExpenseItem> expenseItemRepo, int UserID, DateTime fromDate, DateTime toDate)
        {
            var result = from ei in expenseItemRepo.FindBy(i => i.Status == EnumCompanyTransaction.Expense)
                         join ed in expenditureRepository.All.Where(i => i.CreatedBy == UserID) on ei.ExpenseItemID equals ed.ExpenseItemID
                         where ed.EntryDate >= fromDate && ed.EntryDate <= toDate
                         select ed;
            return await result.OrderByDescending(i => i.EntryDate).ToListAsync();
        }
        public static IEnumerable<Tuple<DateTime, string, string, decimal, string, string>> GetforReport(this IBaseRepository<Expenditure> expenditureRepository,
            IBaseRepository<ExpenseItem> expenseItemRepository, IBaseRepository<ApplicationUser> UserRepository, DateTime fromDate, DateTime toDate, int concernID, int reportType, int ExpenseItemID)
        {
            IEnumerable<ExpenseItem> expenseItems = null;
            if (reportType == 1)
                expenseItems = expenseItemRepository.All.Where(i => i.Status == EnumCompanyTransaction.Expense);
            else
                expenseItems = expenseItemRepository.All.Where(i => i.Status == EnumCompanyTransaction.Income);

            if (ExpenseItemID != 0)
                expenseItems = expenseItems.Where(i => i.ExpenseItemID == ExpenseItemID).ToList();

            var oExpenseData = (from exps in expenditureRepository.All.ToList()
                                join exi in expenseItems on exps.ExpenseItemID equals exi.ExpenseItemID
                                join u in UserRepository.All.ToList() on exps.CreatedBy equals u.Id
                                where (exps.EntryDate >= fromDate && exps.EntryDate <= toDate)
                                group exps by new
                                {
                                    exps.EntryDate,
                                    exi.Description,
                                    exps.Purpose,
                                    exps.VoucherNo,
                                    u.UserName

                                } into g
                                select new
                                {
                                    EntryDate = g.Key.EntryDate,
                                    ItemName = g.Key.Description,
                                    Purpose = g.Key.Purpose,
                                    VoucherNo = g.Key.VoucherNo,
                                    Amount = g.Sum(i3 => i3.Amount),
                                    UserName = g.Key.UserName
                                }
                                             ).ToList();
            return oExpenseData.Select(x => new Tuple<DateTime, string, string, decimal, string, string>
                (
                x.EntryDate,
                x.ItemName,
                x.Purpose,
                x.Amount,
                x.VoucherNo,
                x.UserName
                )).OrderByDescending(x => x.Item1).ToList();

        }


        public static async Task<IEnumerable<Expenditure>> GetAllIncomeAsync(this IBaseRepository<Expenditure> expenditureRepository,
            IBaseRepository<ExpenseItem> expenseItemRepo, DateTime fromDate, DateTime toDate)
        {
            var result = from ei in expenseItemRepo.FindBy(i => i.Status == EnumCompanyTransaction.Income)
                         join ed in expenditureRepository.All on ei.ExpenseItemID equals ed.ExpenseItemID
                         where (ed.EntryDate >= fromDate && ed.EntryDate <= toDate)
                         select ed;
            return await result.Include(i=>i.ExpenseItem).OrderByDescending(i => i.EntryDate).ToListAsync();
        }

        public static decimal GetAllExpenditureAmountByUserID(this IBaseRepository<Expenditure> expenditureRepository, IBaseRepository<ExpenseItem> expenseItemRepo, int UserID, DateTime fromDate, DateTime toDate)
        {
            var expenditures = expenditureRepository.All.Where(i => i.EntryDate >= fromDate && i.EntryDate <= toDate && i.CreatedBy == UserID);
            var eitems = expenseItemRepo.FindBy(i => i.Status == EnumCompanyTransaction.Expense);
            decimal expenseamount = 0;

            var result = from ei in eitems
                         join ed in expenditures on ei.ExpenseItemID equals ed.ExpenseItemID
                         select ed;
            if (result.Count() != 0)
                expenseamount = result.Sum(i => i.Amount);
            return expenseamount;
        }
    }
}
