using IMSWEB.Data;
using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Service
{
    public class AccountingService : IAccountingService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAccountingRepository _AccountingRepository;

        public AccountingService(IUnitOfWork unitOfWork, IAccountingRepository AccountingRepository)
        {
            _unitOfWork = unitOfWork;
            _AccountingRepository = AccountingRepository;
        }

        public IEnumerable<TrialBalanceReportModel> GetTrialBalance(DateTime fromDate, DateTime toDate, int ConcernID)
        {
            return _AccountingRepository.GetTrialBalance(fromDate, toDate, ConcernID);
        }


        public IEnumerable<ProfitLossReportModel> ProfitLossAccount(DateTime fromDate, DateTime toDate, int ConcernID)
        {
            return _AccountingRepository.ProfitLossAccount(fromDate, toDate, ConcernID);
        }

        public IEnumerable<ProfitLossReportModel> BalanceSheet(DateTime fromDate, DateTime toDate, int ConcernID)
        {
            return _AccountingRepository.BalanceSheet(fromDate, toDate, ConcernID);
        }
    }
}
