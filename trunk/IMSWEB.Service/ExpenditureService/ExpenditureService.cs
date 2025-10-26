using IMSWEB.Data;
using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Service
{
    public class ExpenditureService : IExpenditureService
    {
        private readonly IBaseRepository<Expenditure> _ExpenditureRepository;
        private readonly IBaseRepository<ExpenseItem> _expenseItemRepository;
        private readonly IBaseRepository<SOrder> _sorderRepository;
        private readonly IBaseRepository<ApplicationUser> _userRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ExpenditureService(IBaseRepository<Expenditure> expenditureRepository,
            IBaseRepository<ExpenseItem> expenseItemRepository, IBaseRepository<ApplicationUser> userRepository, IBaseRepository<SOrder> sorderRepository, IUnitOfWork unitOfWork)
        {
            _ExpenditureRepository = expenditureRepository;
            _expenseItemRepository = expenseItemRepository;
            _sorderRepository = sorderRepository;
            _unitOfWork = unitOfWork;
            _userRepository = userRepository;
        }

        public void AddExpenditure(Expenditure expenditure)
        {
            _ExpenditureRepository.Add(expenditure);
        }

        public void UpdateExpenditure(Expenditure expenditure)
        {
            _ExpenditureRepository.Update(expenditure);
        }

        public void SaveExpenditure()
        {
            _unitOfWork.Commit();
        }

        public async Task<IEnumerable<Expenditure>> GetAllExpenditureByUserIDAsync(int UserID, DateTime fromDate, DateTime toDate)
        {
            return await _ExpenditureRepository.GetAllExpenditureByUserID(_expenseItemRepository, UserID, fromDate, toDate);
        }

        public async Task<IEnumerable<Expenditure>> GetAllExpenditureAsync(DateTime fromDate, DateTime toDate)
        {
            return await _ExpenditureRepository.GetAllExpenditureAsync(_expenseItemRepository, fromDate, toDate);
        }

        public Expenditure GetExpenditureById(int id)
        {
            return _ExpenditureRepository.FindBy(x => x.ExpenditureID == id).First();
        }
        public IEnumerable<Tuple<DateTime, string, string, decimal, string, string>> GetforExpenditureReport(DateTime fromDate, DateTime toDate, int concernID, int reportType, int ExpenseItemID)
        {
            return _ExpenditureRepository.GetforReport(_expenseItemRepository, _userRepository, fromDate, toDate, concernID, reportType, ExpenseItemID);
        }

        public void DeleteExpenditure(int id)
        {
            _ExpenditureRepository.Delete(x => x.ExpenditureID == id);
        }

        public async Task<IEnumerable<Expenditure>> GetAllIncomeAsync(DateTime fromDate, DateTime toDate)
        {
            return await _ExpenditureRepository.GetAllIncomeAsync(_expenseItemRepository, fromDate, toDate);
        }
        public decimal GetExpenditureAmountByUserID(int UserID, DateTime fromDate, DateTime toDate)
        {
            return _ExpenditureRepository.GetAllExpenditureAmountByUserID(_expenseItemRepository, UserID, fromDate, toDate);
        }
        public IQueryable<Expenditure> GetAllExpense()
        {
            return _ExpenditureRepository.All;
        }
    }
}
