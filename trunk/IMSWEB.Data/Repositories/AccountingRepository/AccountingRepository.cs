using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using IMSWEB.Model;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using IMSWEB.Model.SPModel;

namespace IMSWEB.Data
{
    public class AccountingRepository : IAccountingRepository
    {
        private IMSWEBContext _dbContext;

        #region Properties

        protected IDbFactory DbFactory
        {
            get;
            private set;
        }

        protected IMSWEBContext DbContext
        {
            get { return _dbContext ?? (_dbContext = DbFactory.Init()); }
        }

        public AccountingRepository(IDbFactory dbFactory)
        {
            DbFactory = dbFactory;
        }

        #endregion

        public IEnumerable<TrialBalanceReportModel> GetTrialBalance(DateTime fromDate, DateTime toDate, int ConcernID)
        {
            try
            {
                string fdate = fromDate.ToString("dd MMM yyyy hh:mm:ss tt");
                string tdate = toDate.ToString("dd MMM yyyy hh:mm:ss tt");
                string sql = "exec sp_TrialBalance " + "'" + fdate + "'" + "," + "'" + tdate + "'" + "," + "'" + ConcernID + "'";
                var data = DbContext.Database.SqlQuery<TrialBalanceReportModel>(sql).ToList();
                return data.ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }
        public IEnumerable<ProfitLossReportModel> ProfitLossAccount(DateTime fromDate, DateTime toDate, int ConcernID)
        {
            try
            {
                string fdate = fromDate.ToString("dd MMM yyyy hh:mm:ss tt");
                string tdate = toDate.ToString("dd MMM yyyy hh:mm:ss tt");
                string sql = "exec sp_ProfitandLossAccount " + "'" + fdate + "'" + "," + "'" + tdate + "'" + "," + "'" + ConcernID + "'";
                var data = DbContext.Database.SqlQuery<ProfitLossReportModel>(sql).ToList();
                return data.ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IEnumerable<ProfitLossReportModel> BalanceSheet(DateTime fromDate, DateTime toDate, int ConcernID)
        {
            try
            {
                string fdate = fromDate.ToString("dd MMM yyyy hh:mm:ss tt");
                string tdate = toDate.ToString("dd MMM yyyy hh:mm:ss tt");
                string sql = "exec sp_BalanceSheet " + "'" + fdate + "'" + "," + "'" + tdate + "'" + "," + "'" + ConcernID + "'";
                var data = DbContext.Database.SqlQuery<ProfitLossReportModel>(sql).ToList();
                return data.ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }
    } 
}
