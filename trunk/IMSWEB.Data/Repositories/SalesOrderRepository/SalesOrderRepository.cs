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
using System.Data.SqlTypes;

namespace IMSWEB.Data
{
    public class SalesOrderRepository : ISalesOrderRepository
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

        public SalesOrderRepository(IDbFactory dbFactory)
        {
            DbFactory = dbFactory;
        }

        #endregion

        public bool AddSalesOrderUsingSP(DataTable dtSalesOrder, DataTable dtSalesOrderDetail, DateTime RemindDate, int EmployeeID, bool isAdvanceSale = false)
        {

            bool Result = false;

            using (SqlConnection sqlcon = new SqlConnection(ConfigurationManager.ConnectionStrings["IMSWEB"].ConnectionString))
            {
                if (!isAdvanceSale)
                {
                    using (SqlCommand cmd = new SqlCommand("AddSalesOrder", sqlcon))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@SalesOrder", SqlDbType.Structured).Value = dtSalesOrder;
                        cmd.Parameters.Add("@SODetails", SqlDbType.Structured).Value = dtSalesOrderDetail;
                        cmd.Parameters.Add("@RemindDate", SqlDbType.DateTime).Value = RemindDate == DateTime.MinValue ? SqlDateTime.Null : RemindDate;
                        cmd.Parameters.Add("@EmployeeID", SqlDbType.Int).Value = EmployeeID;

                        var returnParameter = cmd.Parameters.Add("@ReturnVal", SqlDbType.Int);
                        returnParameter.Direction = ParameterDirection.ReturnValue;

                        sqlcon.Open();
                        cmd.ExecuteNonQuery();

                        int dbresult = (int)returnParameter.Value;
                        if (dbresult == 1)
                            Result = true;
                    }
                }
                else
                {
                    using (SqlCommand cmd = new SqlCommand("AddAdvanceSalesOrder", sqlcon))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@SalesOrder", SqlDbType.Structured).Value = dtSalesOrder;
                        cmd.Parameters.Add("@SODetails", SqlDbType.Structured).Value = dtSalesOrderDetail;
                        cmd.Parameters.Add("@RemindDate", SqlDbType.DateTime).Value = RemindDate == DateTime.MinValue ? SqlDateTime.Null : RemindDate;
                        cmd.Parameters.Add("@EmployeeID", SqlDbType.Int).Value = EmployeeID;

                        var returnParameter = cmd.Parameters.Add("@ReturnVal", SqlDbType.Int);
                        returnParameter.Direction = ParameterDirection.ReturnValue;

                        sqlcon.Open();
                        cmd.ExecuteNonQuery();

                        int dbresult = (int)returnParameter.Value;
                        if (dbresult == 1)
                            Result = true;
                    }
                }
            }

            return Result;
        }

        public void AddReplacementOrderUsingSP(DataTable dtSalesOrder, DataTable dtSalesOrderDetail)
        {
            using (SqlConnection sqlcon = new SqlConnection(ConfigurationManager.ConnectionStrings["IMSWEB"].ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_AddReplacementOrder", sqlcon))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@SalesOrder", SqlDbType.Structured).Value = dtSalesOrder;
                    cmd.Parameters.Add("@SODetails", SqlDbType.Structured).Value = dtSalesOrderDetail;
                    sqlcon.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }


        public bool ApproveAdvanceSalesUsingSP(int salesOrderId)
        {
            bool Result = false;
            using (SqlConnection sqlcon = new SqlConnection(ConfigurationManager.ConnectionStrings["IMSWEB"].ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("ApproveAdvanceSalesOrder", sqlcon))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@SalesOrderId", SqlDbType.Int).Value = salesOrderId;
                   

                    var returnParameter = cmd.Parameters.Add("@ReturnVal", SqlDbType.Int);
                    returnParameter.Direction = ParameterDirection.ReturnValue;

                    sqlcon.Open();
                    cmd.ExecuteNonQuery();

                    int dbresult = (int)returnParameter.Value;
                    if (dbresult == 1)
                        Result = true;
                }
            }
            return Result;
        }

        public bool UpdateSalesOrderUsingSP(int userId, int salesOrderId, DataTable dtSalesOrder, DataTable dtSODetail, int EmployeeID, bool isAdvanceSale = false)
        {
            bool Result = false;

            using (SqlConnection sqlcon = new SqlConnection(ConfigurationManager.ConnectionStrings["IMSWEB"].ConnectionString))
            {
                if (!isAdvanceSale)
                {
                    using (SqlCommand cmd = new SqlCommand("UpdateSalesOrder", sqlcon))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
                        cmd.Parameters.Add("@SalesOrderId", SqlDbType.Int).Value = salesOrderId;
                        cmd.Parameters.Add("@SalesOrder", SqlDbType.Structured).Value = dtSalesOrder;
                        cmd.Parameters.Add("@SODetails", SqlDbType.Structured).Value = dtSODetail;
                        cmd.Parameters.Add("@EmployeeID", SqlDbType.Int).Value = EmployeeID;

                        var returnParameter = cmd.Parameters.Add("@ReturnVal", SqlDbType.Int);
                        returnParameter.Direction = ParameterDirection.ReturnValue;

                        sqlcon.Open();
                        cmd.ExecuteNonQuery();

                        int dbresult = (int)returnParameter.Value;
                        if (dbresult == 1)
                            Result = true;
                    }
                }
                else
                {
                    using (SqlCommand cmd = new SqlCommand("UpdateAdvanceSalesOrder", sqlcon))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
                        cmd.Parameters.Add("@SalesOrderId", SqlDbType.Int).Value = salesOrderId;
                        cmd.Parameters.Add("@SalesOrder", SqlDbType.Structured).Value = dtSalesOrder;
                        cmd.Parameters.Add("@SODetails", SqlDbType.Structured).Value = dtSODetail;
                        cmd.Parameters.Add("@EmployeeID", SqlDbType.Int).Value = EmployeeID;

                        var returnParameter = cmd.Parameters.Add("@ReturnVal", SqlDbType.Int);
                        returnParameter.Direction = ParameterDirection.ReturnValue;

                        sqlcon.Open();
                        cmd.ExecuteNonQuery();

                        int dbresult = (int)returnParameter.Value;
                        if (dbresult == 1)
                            Result = true;
                    }
                }
            }

            return Result;
        }


        public void DeleteSalesOrderUsingSP(int orderId, int userId)
        {
            using (SqlConnection sqlcon = new SqlConnection(ConfigurationManager.ConnectionStrings["IMSWEB"].ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("ReturnSalesOrder", sqlcon))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@SalesOrderId", SqlDbType.Int).Value = orderId;
                    cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
                    sqlcon.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }
        public void DeleteAdvanceSalesOrderUsingSP(int orderId, int userId)
        {
            using (SqlConnection sqlcon = new SqlConnection(ConfigurationManager.ConnectionStrings["IMSWEB"].ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("ReturnAdvanceSalesOrder", sqlcon))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@SalesOrderId", SqlDbType.Int).Value = orderId;
                    cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
                    sqlcon.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void DeleteSalesOrderDetailUsingSP(int sorderDetailId, int userId)
        {
            using (SqlConnection sqlcon = new SqlConnection(ConfigurationManager.ConnectionStrings["IMSWEB"].ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("DeleteSalesOrderDetail", sqlcon))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@SOrderDetailId", SqlDbType.Int).Value = sorderDetailId;
                    cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
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

        public List<SRWiseCustomerSalesSummaryVM> SRWiseCustomerSalesSummary(DateTime fromdate, DateTime todate, int ConcernID, int EmployeeID)
        {
            try
            {
                string fdate = fromdate.ToString("yyyy-MM-dd HH:mm:ss");
                string tdate = todate.ToString("yyyy-MM-dd HH:mm:ss");
                string sql = "exec sp_SRWiseCustomerSalesSummary " + "'" + fdate + "'" + "," + "'" + tdate + "'";
                var data = DbContext.Database.SqlQuery<SRWiseCustomerSalesSummaryVM>(sql).ToList();
                if (EmployeeID == 0)
                    return data.Where(i => i.ConcernID == ConcernID).ToList();
                else
                    return data.Where(i => i.ConcernID == ConcernID && i.EmployeeID == EmployeeID).ToList();
            }
            catch (Exception)
            {

                throw;
            }
        }


        public List<CustomerLedgerModel> CustomerLedger(DateTime fromdate, DateTime todate, int ConcernID, int CustomerID)
        {
            try
            {
                string fdate = fromdate.ToString("yyyy-MM-dd HH:mm:ss");
                string tdate = todate.ToString("yyyy-MM-dd HH:mm:ss");
                string sql = string.Empty;
                //if (CustomerID == 0)
                sql = "exec sp_CustomerLedger " + "'" + fdate + "'" + "," + "'" + tdate + "'" + "," + "'" + ConcernID + "'";
                //else
                //    sql = "exec sp_IndividualCustomerLedger " + "'" + fdate + "'" + "," + "'" + tdate + "'" + "," + "'" + ConcernID + "'" + "," + "'" + CustomerID + "'";

                var data = DbContext.Database.SqlQuery<CustomerLedgerModel>(sql).ToList();
                if (CustomerID == 0)
                    return data.ToList();
                else
                    return data.Where(i => i.CustomerID == CustomerID).ToList();


            }
            catch (Exception)
            {
                throw;
            }
        }


        public List<CustomerDueReportModel> CustomerDue(DateTime fromdate, DateTime todate, int ConcernID, int CustomerID, int IsOnlyDue)
        {
            try
            {
                string fdate = fromdate.ToString("yyyy-MM-dd HH:mm:ss");
                string tdate = todate.ToString("yyyy-MM-dd HH:mm:ss");
                string sql = "exec sp_CustomerDueReport " + "'" + fdate + "'" + "," + "'" + tdate + "'" + "," + "'" + IsOnlyDue + "'";
                var data = DbContext.Database.SqlQuery<CustomerDueReportModel>(sql).ToList();
                if (CustomerID == 0)
                    return data.Where(i => i.ConcernID == ConcernID).ToList();
                else
                    return data.Where(i => i.ConcernID == ConcernID && i.CustomerID == CustomerID).ToList();


            }
            catch (Exception)
            {

                throw;
            }
        }

        public bool AddReturnOrderUsingSP(DataTable dtSalesOrder, DataTable dtSalesOrderDetail)
        {
            bool Result = false;
            try
            {
                using (SqlConnection sqlcon = new SqlConnection(ConfigurationManager.ConnectionStrings["IMSWEB"].ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_AddReturnOrder", sqlcon))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@SalesOrder", SqlDbType.Structured).Value = dtSalesOrder;
                        cmd.Parameters.Add("@SODetails", SqlDbType.Structured).Value = dtSalesOrderDetail;


                        var returnParameter = cmd.Parameters.Add("@ReturnVal", SqlDbType.Int);
                        returnParameter.Direction = ParameterDirection.ReturnValue;

                        sqlcon.Open();
                        cmd.ExecuteNonQuery();

                        int dbresult = (int)returnParameter.Value;
                        if (dbresult == 1)
                            Result = true;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return Result;
        }



        public List<DailyWorkSheetReportModel> DailyWorkSheetReport(DateTime fromdate, DateTime todate, int ConcernID)
        {
            try
            {
                string fdate = fromdate.ToString("yyyy-MM-dd HH:mm:ss");
                string tdate = todate.ToString("yyyy-MM-dd HH:mm:ss");
                string sql = "exec sp_DailyWorkSheet " + "'" + fdate + "'" + "," + "'" + tdate + "'";
                var data = DbContext.Database.SqlQuery<DailyWorkSheetReportModel>(sql).ToList();
                return data.Where(i => i.ConcernID == ConcernID).ToList();
            }
            catch (Exception)
            {

                throw;
            }
        }


        public List<ReturnReportModel> ReturnReport(DateTime fromdate, DateTime todate, int ConcernID, int CustomerID)
        {
            try
            {
                string fdate = fromdate.ToString("yyyy-MM-dd HH:mm:ss");
                string tdate = todate.ToString("yyyy-MM-dd HH:mm:ss");
                string sql = "exec sp_ReturnReport " + "'" + fdate + "'" + "," + "'" + tdate + "'" + "," + "'" + ConcernID + "'";
                var data = DbContext.Database.SqlQuery<ReturnReportModel>(sql).ToList();
                if (CustomerID == 0)
                    return data;
                else
                    return data.Where(i => i.CustomerID == CustomerID).ToList();

            }
            catch (Exception)
            {

                throw;
            }
        }


        public List<MonthlyBenefitReport> MonthlyBenefitReport(DateTime fromdate, DateTime todate, int ConcernID)
        {
            try
            {
                string fdate = fromdate.ToString("yyyy-MM-dd HH:mm:ss");
                string tdate = todate.ToString("yyyy-MM-dd HH:mm:ss");
                string sql = "exec MonthlyBenefitReport " + "'" + fdate + "'" + "," + "'" + tdate + "'" + "," + "'" + ConcernID + "'";
                var data = DbContext.Database.SqlQuery<MonthlyBenefitReport>(sql).ToList();
                return data;

            }
            catch (Exception)
            {

                throw;
            }
        }


        public List<ProductWiseBenefitModel> ProductWiseBenefitReport(DateTime fromdate, DateTime todate, int ConcernID)
        {
            try
            {
                string fdate = fromdate.ToString("yyyy-MM-dd HH:mm:ss");
                string tdate = todate.ToString("yyyy-MM-dd HH:mm:ss");
                string sql = "exec sp_ProductWiseBenefitReport " + "'" + fdate + "'" + "," + "'" + tdate + "'" + "," + "'" + ConcernID + "'";
                var data = DbContext.Database.SqlQuery<ProductWiseBenefitModel>(sql).ToList();
                return data;

            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
