using IMSWEB.Model.SPModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Data.Repositories.StockRepository
{
   public interface IStockRepository
    {
       IEnumerable<DailyStockVSSalesSummaryReportModel> DailyStockVSSalesSummary(DateTime fromDate, DateTime toDate, int concernID, int ProductID);
    }
}
