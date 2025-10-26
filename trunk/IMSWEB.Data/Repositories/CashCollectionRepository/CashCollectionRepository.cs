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
    public class CashCollectionRepository : ICashCollectionRepository
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

        public CashCollectionRepository(IDbFactory dbFactory)
        {
            DbFactory = dbFactory;
        }

        #endregion

        public void UpdateTotalDue(int CustomerID, int SupplierID, int BankID, int BankWithdrawID, decimal TotalRecAmt)
        {
            using (SqlConnection sqlcon = new SqlConnection(ConfigurationManager.ConnectionStrings["IMSWEB"].ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("UpdateTotalDue", sqlcon))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@CusId", SqlDbType.Int).Value = CustomerID;
                    cmd.Parameters.Add("@SupId", SqlDbType.Int).Value = SupplierID;
                    cmd.Parameters.Add("@BankDepositId", SqlDbType.Int).Value = BankID;
                    cmd.Parameters.Add("@BankWithdrawId", SqlDbType.Int).Value = BankWithdrawID;
                    cmd.Parameters.Add("@CollectionAmount", SqlDbType.Decimal).Value = TotalRecAmt;
                    sqlcon.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void UpdateTotalDueWhenEdit(int CustomerID, int SupplierID, int BankTransactionID, int CashCollectionID, decimal TotalRecAmt)
        {
            using (SqlConnection sqlcon = new SqlConnection(ConfigurationManager.ConnectionStrings["IMSWEB"].ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("UpdateTotalDueWhenEdit", sqlcon))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@CusId", SqlDbType.Int).Value = CustomerID;
                    cmd.Parameters.Add("@SupId", SqlDbType.Int).Value = SupplierID;
                    cmd.Parameters.Add("@CashCollectionID", SqlDbType.Int).Value = CashCollectionID;
                    cmd.Parameters.Add("@BankTransactionID", SqlDbType.Int).Value = BankTransactionID;
                    cmd.Parameters.Add("@NewCollectionAmount", SqlDbType.Decimal).Value = TotalRecAmt;
                    sqlcon.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }



        public IEnumerable<DailyCashBookLedgerModel> DailyCashBookLedger(DateTime fromDate, DateTime toDate, int ConcernID)
        {
            try
            {
                string fdate = fromDate.ToString("yyyy-MM-dd HH:mm:ss");
                string tdate = toDate.ToString("yyyy-MM-dd HH:mm:ss");
                string sql = "exec sp_DailyCashBookLedger " + "'" + fdate + "'" + "," + "'" + tdate + "'";
                var data = DbContext.Database.SqlQuery<DailyCashBookLedgerModel>(sql).ToList();
                return data.Where(i => i.ConcernID == ConcernID).ToList();
            }
            catch (Exception)
            {

                throw;
            }
        }
        public IEnumerable<CashInHandReportModel> CashInHandReport(DateTime fromDate, DateTime toDate, int ReportType, int ConcernID, int CustomerType)
        {
            try
            {
                string fdate = fromDate.ToString("yyyy-MM-dd HH:mm:ss");
                string tdate = toDate.ToString("yyyy-MM-dd HH:mm:ss");
                string sql = string.Empty;

                if (ReportType == 1)
                {
                    //if (ConcernID == (int)EnumSisterConcern.KINGSTAR_CONCERNID)
                    //    sql = "exec sp_KSDailyCashInHand " + "'" + fdate + "'" + "," + "'" + tdate + "'" + "," + "'" + ConcernID + "'";
                    //else
                    sql = "exec sp_DailyCashInHand " + "'" + fdate + "'" + "," + "'" + tdate + "'" + "," + "'" + ConcernID + "'";
                }
                else if (ReportType == 2)
                    sql = "exec sp_MonthlyCashInHand " + "'" + fdate + "'" + "," + "'" + tdate + "'" + "," + "'" + ConcernID + "'";
                else if (ReportType == 3)
                    sql = "exec sp_YearlyCashInHand " + "'" + fdate + "'" + "," + "'" + tdate + "'" + "," + "'" + ConcernID + "'";


                var data = DbContext.Database.SqlQuery<CashInHandReportModel>(sql).ToList();
                return data.ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
