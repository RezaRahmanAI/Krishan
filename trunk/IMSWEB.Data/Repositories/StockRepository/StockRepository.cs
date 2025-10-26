using IMSWEB.Model.SPModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Data.Repositories.StockRepository
{
    public class StockRepository : IStockRepository
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

        public StockRepository(IDbFactory dbFactory)
        {
            DbFactory = dbFactory;
        }

        #endregion
        public IEnumerable<DailyStockVSSalesSummaryReportModel> DailyStockVSSalesSummary(DateTime fromDate, DateTime toDate, int ConcernID, int ProductID)
        {
            try
            {
                string fdate = fromDate.ToString("yyyy-MM-dd HH:mm:ss");
                string tdate = toDate.ToString("yyyy-MM-dd HH:mm:ss");
                string sql = "exec sp_DailyStockVSSalesSummary " + "'" + fdate + "'" + "," + "'" + tdate + "'";
                var data = DbContext.Database.SqlQuery<DailyStockVSSalesSummaryReportModel>(sql).ToList();
                if (ProductID != 0)
                    return data.Where(i => i.ConcernID == ConcernID && i.ProductID==ProductID).ToList();
                else
                    return data.Where(i => i.ConcernID == ConcernID).ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
