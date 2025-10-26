using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace IMSWEB.Data
{
    public static class InvestmentExtension
    {
        public static async Task<IEnumerable<Tuple<int, DateTime, string, string, decimal>>>
            GetAllAsync(this IBaseRepository<ShareInvestment> ShareInvestmentRepository,
            IBaseRepository<ShareInvestmentHead> ShareInvestmentHeadRepository,
            EnumInvestmentType investmentType, DateTime fromDate, DateTime toDate, int InvestTransType)
        {
            IQueryable<ShareInvestment> ShareInvestments = null;
            if (InvestTransType != 0)
                ShareInvestments = ShareInvestmentRepository.All.Where(i => (int)i.TransactionType == InvestTransType);
            else
                ShareInvestments = ShareInvestmentRepository.All;

            var Result = await (from si in ShareInvestments
                                join h in ShareInvestmentHeadRepository.All on si.SIHID equals h.SIHID
                                where si.EntryDate >= fromDate && si.EntryDate <= toDate
                                && h.ParentId == (int)investmentType
                                select new
                                {
                                    si.SIID,
                                    si.EntryDate,
                                    si.Purpose,
                                    si.Amount,
                                    h.Name
                                }).OrderByDescending(i => i.EntryDate).ToListAsync();

            return Result.Select(x => new Tuple<int, DateTime, string, string, decimal>(
                        x.SIID,
                        x.EntryDate,
                        x.Name,
                        x.Purpose,
                        x.Amount
                          ));
        }

        public static async Task<IEnumerable<Tuple<int, DateTime, string, string, decimal>>>
            GetAllLiabilityAsync(this IBaseRepository<ShareInvestment> ShareInvestmentRepository,
            IBaseRepository<ShareInvestmentHead> ShareInvestmentHeadRepository,
            EnumInvestmentType investmentType, DateTime fromDate, DateTime toDate, int InvestTransType)
        {
            IQueryable<ShareInvestment> ShareInvestments = null;
            if (InvestTransType != 0)
                ShareInvestments = ShareInvestmentRepository.All.Where(i => (int)i.TransactionType == InvestTransType);
            else
                ShareInvestments = ShareInvestmentRepository.All;

            var Result = await (from si in ShareInvestments
                                join h in ShareInvestmentHeadRepository.All on si.SIHID equals h.SIHID
                                where si.EntryDate >= fromDate && si.EntryDate <= toDate
                                && (h.ParentId == (int)EnumInvestmentType.Liability || h.ParentId == (int)EnumInvestmentType.FDR || h.ParentId == (int)EnumInvestmentType.PF || h.ParentId == (int)EnumInvestmentType.Security)
                                select new
                                {
                                    si.SIID,
                                    si.EntryDate,
                                    si.Purpose,
                                    si.Amount,
                                    h.Name
                                }).OrderByDescending(i => i.EntryDate).ToListAsync();

            return Result.Select(x => new Tuple<int, DateTime, string, string, decimal>(
                        x.SIID,
                        x.EntryDate,
                        x.Name,
                        x.Purpose,
                        x.Amount
                          ));
        }
    }
}
