using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace IMSWEB.Data
{
    public class CreditSalesOrderRepository : ICreditSalesOrderRepository
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

        public CreditSalesOrderRepository(IDbFactory dbFactory)
        {
            DbFactory = dbFactory;
        }

        #endregion

        public void AddSalesOrderUsingSP(DataTable dtSalesOrder, DataTable dtSODetail,
            DataTable dtSchedules)
        {
            using (SqlConnection sqlcon = new SqlConnection(ConfigurationManager.ConnectionStrings["IMSWEB"].ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("AddCreditSalesOrder", sqlcon))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@CSalesOrder", SqlDbType.Structured).Value = dtSalesOrder;
                    cmd.Parameters.Add("@CSODetails", SqlDbType.Structured).Value = dtSODetail;
                    cmd.Parameters.Add("@CSSchedules", SqlDbType.Structured).Value = dtSchedules;
                    sqlcon.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void InstallmentPaymentUsingSP(int orderId, decimal installmentAmount, DataTable dtSchedules, decimal LastPayAdjustment)
        {
            using (SqlConnection sqlcon = new SqlConnection(ConfigurationManager.ConnectionStrings["IMSWEB"].ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("InstallmentPayment", sqlcon))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@SalesOrderId", SqlDbType.Int).Value = orderId;
                    cmd.Parameters.Add("@InstallmentAmount", SqlDbType.Decimal).Value = installmentAmount;
                    cmd.Parameters.Add("@Schedules", SqlDbType.Structured).Value = dtSchedules;
                    cmd.Parameters.Add("@LastPayAdjustment", SqlDbType.Decimal).Value = LastPayAdjustment;

                    sqlcon.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void ReturnSalesOrderUsingSP(int orderId, int userId)
        {
            using (SqlConnection sqlcon = new SqlConnection(ConfigurationManager.ConnectionStrings["IMSWEB"].ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("ReturnCreditSalesOrder", sqlcon))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@SalesOrderId", SqlDbType.Int).Value = orderId;
                    cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
                    sqlcon.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void CalculatePenaltySchedules(int ConcernID)
        {
            using (SqlConnection sqlcon = new SqlConnection(ConfigurationManager.ConnectionStrings["IMSWEB"].ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("CreditSalesPenaltySchedules", sqlcon))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@ConcernID", SqlDbType.Int).Value = ConcernID;
                    sqlcon.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void CorrectionStockData(int ConcernId)
        {
            using (SqlConnection sqlcon = new SqlConnection(ConfigurationManager.ConnectionStrings["IMSWEB"].ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SP_StockCorrection", sqlcon))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@ConcernId", SqlDbType.Int).Value = ConcernId;
                    sqlcon.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public System.Collections.Generic.IEnumerable<System.Tuple<string, string, string, string, System.DateTime, System.DateTime, decimal, System.Tuple<decimal, decimal, decimal, decimal, string, decimal>>> GetUpcomingSchedule(System.DateTime fromDate, System.DateTime toDate, int concernID)
        {
            throw new System.NotImplementedException();
        }
    }
}
