using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using IMSWEB.Model;
using System.Data;

namespace IMSWEB.Data
{
    public interface ICreditSalesOrderRepository
    {
        void AddSalesOrderUsingSP(DataTable dtSalesOrder, DataTable dtSODetail,
            DataTable dtSchedulesl);

        void InstallmentPaymentUsingSP(int orderId, decimal installmentAmount, DataTable dtSchedules,decimal LastPayAdjustment);

        void ReturnSalesOrderUsingSP(int orderId, int userId);

        void CalculatePenaltySchedules(int ConcernID);
        void CorrectionStockData(int concermID);
    }
}
