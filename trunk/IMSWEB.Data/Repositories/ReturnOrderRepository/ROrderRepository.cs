﻿using System;
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

namespace IMSWEB.Data
{
    public class ROrderRepository : IROrderRepository
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

        public ROrderRepository(IDbFactory dbFactory)
        {
            DbFactory = dbFactory;
        }

        #endregion

        public void AddReturnOrderUsingSP(DataTable dtReturnOrder, DataTable dtRODetail,
            DataTable dtROProductDetail, DataTable dtStock, DataTable dtStockDetail)
        {


            using (SqlConnection sqlcon = new SqlConnection(ConfigurationManager.ConnectionStrings["IMSWEB"].ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("AddPurchaseOrder", sqlcon))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@PurchaseOrder", SqlDbType.Structured).Value = dtReturnOrder;
                    cmd.Parameters.Add("@PODetails", SqlDbType.Structured).Value = dtRODetail;
                    cmd.Parameters.Add("@POProductDetails", SqlDbType.Structured).Value = dtROProductDetail;
                    cmd.Parameters.Add("@Stocks", SqlDbType.Structured).Value = dtStock;
                    cmd.Parameters.Add("@StockDetails", SqlDbType.Structured).Value = dtStockDetail;
                    sqlcon.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }
       
        public Tuple<bool, int> AddReturnOrderUsingSP(DataTable dtSalesOrder, DataTable dtSalesOrderDetail)
        {
            bool Result = false;
            int ROrderID = 0;

            try
            {
                using (SqlConnection sqlcon = new SqlConnection(ConfigurationManager.ConnectionStrings["IMSWEB"].ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_AddReturnOrder", sqlcon))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@SalesOrder", SqlDbType.Structured).Value = dtSalesOrder;
                        cmd.Parameters.Add("@SODetails", SqlDbType.Structured).Value = dtSalesOrderDetail;

                        cmd.Parameters.Add("@Result", SqlDbType.Int).Direction = ParameterDirection.Output;

                        cmd.Parameters.Add("@ROrderID", SqlDbType.Int).Direction = ParameterDirection.Output;

                        sqlcon.Open();
                        cmd.ExecuteNonQuery();

                        ROrderID = Convert.ToInt32(cmd.Parameters["@ROrderID"].Value);
                        int dbresult = Convert.ToInt32(cmd.Parameters["@Result"].Value);
                        if (dbresult == 1)
                            Result = true;

                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return new Tuple<bool, int>(Result, ROrderID);
        }

        public void UpdateReturnOrderUsingSP(int returnOrderId, DataTable dtReturnOrder, DataTable dtRODetail,
            DataTable dtROProductDetail, DataTable dtStock, DataTable dtStockDetail)
        {
            using (SqlConnection sqlcon = new SqlConnection(ConfigurationManager.ConnectionStrings["IMSWEB"].ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("UpdatePurchaseOrder", sqlcon))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@PurchaseOrderId", SqlDbType.Int).Value = returnOrderId;
                    cmd.Parameters.Add("@PurchaseOrder", SqlDbType.Structured).Value = dtReturnOrder;
                    cmd.Parameters.Add("@PODetails", SqlDbType.Structured).Value = dtRODetail;
                    cmd.Parameters.Add("@POProductDetails", SqlDbType.Structured).Value = dtROProductDetail;
                    cmd.Parameters.Add("@Stocks", SqlDbType.Structured).Value = dtStock;
                    cmd.Parameters.Add("@StockDetails", SqlDbType.Structured).Value = dtStockDetail;
                    sqlcon.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void DeleteReturnOrderDetailUsingSP(int customerId, int rorderDetailId, int productId,
            int colorId, int userId, decimal quantity, decimal totalDue, DataTable dtROProductDetail)
        {
            using (SqlConnection sqlcon = new SqlConnection(ConfigurationManager.ConnectionStrings["IMSWEB"].ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("DeletePurchaseOrderDetail", sqlcon))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@SupplierId", SqlDbType.Int).Value = customerId;
                    cmd.Parameters.Add("@POrderDetailId", SqlDbType.Int).Value = rorderDetailId;
                    cmd.Parameters.Add("@ProductId", SqlDbType.Int).Value = productId;
                    cmd.Parameters.Add("@ColorId", SqlDbType.Int).Value = colorId;
                    cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
                    cmd.Parameters.Add("@Quantity", SqlDbType.Decimal).Value = quantity;
                    cmd.Parameters.Add("@TotalDue", SqlDbType.Decimal).Value = totalDue;
                    cmd.Parameters.Add("@POProductDetails", SqlDbType.Structured).Value = dtROProductDetail;
                    sqlcon.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void DeleteReturnOrderUsingSP(int orderId, int userId)
        {
            using (SqlConnection sqlcon = new SqlConnection(ConfigurationManager.ConnectionStrings["IMSWEB"].ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("DeletePurchaseOrder", sqlcon))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@PurchaseOrderId", SqlDbType.Int).Value = orderId;
                    cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
                    sqlcon.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public int CheckProductStatusByROId(int id)
        {
            using (SqlConnection sqlcon = new SqlConnection(ConfigurationManager.ConnectionStrings["IMSWEB"].ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT [dbo].[CheckProductStatusByPOId](" + id + ")", sqlcon))
                {
                    cmd.CommandType = CommandType.Text;
                    sqlcon.Open();
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }

        public int CheckProductStatusByRODetailId(int id)
        {
            using (SqlConnection sqlcon = new SqlConnection(ConfigurationManager.ConnectionStrings["IMSWEB"].ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT [dbo].[CheckProductStatusByPODetailId](" + id + ")", sqlcon))
                {
                    cmd.CommandType = CommandType.Text;
                    sqlcon.Open();
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }
    }
}
