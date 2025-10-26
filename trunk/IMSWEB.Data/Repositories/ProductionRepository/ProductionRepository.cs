using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace IMSWEB.Data
{
    public class ProductionRepository : IProductionRepository
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

        public ProductionRepository(IDbFactory dbFactory)
        {
            DbFactory = dbFactory;
        }

        #endregion

        public Tuple<bool, int> AddProduction(Production newProduction, int ProductionID)
        {

            Tuple<bool, int> Result = new Tuple<bool, int>(false, 0);

            using (var trans = DbContext.Database.BeginTransaction())
            {
                try
                {
                    if (ProductionID > 0)//update 
                    {
                        var oldProductionSetup = DbContext.Productions.Where(i => i.ProductionID == ProductionID)
                                    .Include(i => i.ProductionDetails).SingleOrDefault();
                        newProduction.CreatedBy = oldProductionSetup.CreatedBy;
                        newProduction.CreateDate = oldProductionSetup.CreateDate;
                        newProduction.ConcernID = oldProductionSetup.ConcernID;

                        //update Parent
                        DbContext.Entry(oldProductionSetup).CurrentValues.SetValues(newProduction);

                        //delete children
                        foreach (var existingChild in oldProductionSetup.ProductionDetails.ToList())
                        {
                            if (!newProduction.ProductionDetails.Any(c => c.PDetailID == existingChild.PDetailID))
                            {
                                //Stock return of fin. goods
                                var stock = DbContext.Stocks.FirstOrDefault(i => i.ProductID == existingChild.ProductID);
                                stock.Quantity -= existingChild.Quantity;

                                var stockDetail = DbContext.StockDetails
                                    .Where(i => i.PDetailID == existingChild.PDetailID);
                                DbContext.StockDetails.RemoveRange(stockDetail);

                                //Add Raw materials in Stock
                                foreach (var itemRM in existingChild.ProductionRawMaterials)
                                {
                                    var rmstock = DbContext.Stocks.FirstOrDefault(i => i.ProductID == itemRM.ProductID);
                                    rmstock.Quantity += itemRM.Quantity;

                                    var rmstockDetail = DbContext.StockDetails
                                        .FirstOrDefault(i => i.SDetailID == itemRM.SDetailID);
                                    rmstockDetail.Quantity += itemRM.Quantity;
                                    rmstockDetail.Status = 1;
                                }
                                //Delete
                                DbContext.ProductionRawMaterials.RemoveRange(existingChild.ProductionRawMaterials);
                                //DbContext.ProductionIMEIs.RemoveRange(existingChild.ProductionIMEIs);
                                DbContext.ProductionDetails.Remove(existingChild);

                            }
                        }

                        //update and insert children
                        foreach (var newPDetail in newProduction.ProductionDetails)
                        {
                            var oldPDetail = DbContext.ProductionDetails.FirstOrDefault(i => i.PDetailID == newPDetail.PDetailID);
                            //update
                            if (oldPDetail != null)
                            {

                                //Return Fin from Stock
                                var oldstock = DbContext.Stocks.FirstOrDefault(i => i.ProductID == oldPDetail.ProductID);
                                oldstock.Quantity -= oldPDetail.Quantity;
                                oldstock.ModifiedBy = newProduction.ModifiedBy;
                                oldstock.ModifiedDate = newProduction.ModifiedDate;

                                var oldstockDetail = DbContext.StockDetails
                                   .Where(i => i.PDetailID == oldPDetail.PDetailID);
                                DbContext.StockDetails.RemoveRange(oldstockDetail);

                                //Add Raw materials in Stock
                                foreach (var itemRM in oldPDetail.ProductionRawMaterials)
                                {
                                    var rmstock = DbContext.Stocks.FirstOrDefault(i => i.ProductID == itemRM.ProductID);
                                    rmstock.Quantity += itemRM.Quantity;

                                    var rmstockDetail = DbContext.StockDetails
                                        .FirstOrDefault(i => i.SDetailID == itemRM.SDetailID);
                                    rmstockDetail.Quantity += itemRM.Quantity;
                                    rmstockDetail.Status = 1;
                                }

                                //Delete
                                //DbContext.ProductionIMEIs.RemoveRange(oldPDetail.ProductionIMEIs);
                                //insert
                                //foreach (var newIMEI in newPDetail.ProductionIMEIs)
                                //{
                                //    newIMEI.PDetailID = newPDetail.PDetailID;
                                //    DbContext.ProductionIMEIs.Add(newIMEI);
                                //}

                                //Delete
                                DbContext.ProductionRawMaterials.RemoveRange(oldPDetail.ProductionRawMaterials);

                                RawMaterialStockUpdate(newProduction, newPDetail);

                                // insert
                                foreach (var newRM in newPDetail.ProductionRawMaterials)
                                {
                                    newRM.PDetailID = newPDetail.PDetailID;
                                    DbContext.ProductionRawMaterials.Add(newRM);
                                }


                                FinGoodsStockUpdate(newProduction, newPDetail);
                                DbContext.Entry(oldPDetail).CurrentValues.SetValues(newPDetail);

                            }
                            else //insert
                            {
                                newPDetail.ProductionID = ProductionID;
                                DbContext.ProductionDetails.Add(newPDetail);
                                RawMaterialStockUpdate(newProduction, newPDetail);
                                DbContext.SaveChanges();
                                FinGoodsStockUpdate(newProduction, newPDetail);
                            }
                        }
                    }
                    else // New DO Add
                    {
                        DbContext.Productions.Add(newProduction);
                        foreach (var item in newProduction.ProductionDetails)
                        {
                            RawMaterialStockUpdate(newProduction, item);
                        }
                        DbContext.SaveChanges();
                        foreach (var item in newProduction.ProductionDetails)
                        {
                            FinGoodsStockUpdate(newProduction, item);
                        }
                    }

                    DbContext.SaveChanges();
                    trans.Commit();
                    Result = new Tuple<bool, int>(true, newProduction.ProductionID);
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    Result = new Tuple<bool, int>(false, 0);
                }
            };

            return Result;
        }

        public Tuple<bool, int> AddManualProduction(Production newProduction, int ProductionID)
        {

            Tuple<bool, int> Result = new Tuple<bool, int>(false, 0);

            using (var trans = DbContext.Database.BeginTransaction())
            {
                try
                {
                    if (ProductionID > 0)//update 
                    {
                        var oldProductionSetup = DbContext.Productions.Where(i => i.ProductionID == ProductionID)
                                    .Include(i => i.ProductionDetails).SingleOrDefault();
                        newProduction.CreatedBy = oldProductionSetup.CreatedBy;
                        newProduction.CreateDate = oldProductionSetup.CreateDate;
                        newProduction.ConcernID = oldProductionSetup.ConcernID;

                        //update Parent
                        DbContext.Entry(oldProductionSetup).CurrentValues.SetValues(newProduction);

                        //delete children
                        foreach (var existingChild in oldProductionSetup.ProductionDetails.ToList())
                        {
                            if (!newProduction.ProductionDetails.Any(c => c.PDetailID == existingChild.PDetailID))
                            {
                                //Stock return of fin. goods
                                var stock = DbContext.Stocks.FirstOrDefault(i => i.ProductID == existingChild.ProductID);
                                stock.Quantity -= existingChild.Quantity;

                                var stockDetail = DbContext.StockDetails
                                    .Where(i => i.PDetailID == existingChild.PDetailID);
                                DbContext.StockDetails.RemoveRange(stockDetail);

                                //Add Raw materials in Stock

                                //foreach (var itemRM in existingChild.ProductionRawMaterials)
                                //{
                                //    var rmstock = DbContext.Stocks.FirstOrDefault(i => i.ProductID == itemRM.ProductID);
                                //    rmstock.Quantity += itemRM.Quantity;

                                //    var rmstockDetail = DbContext.StockDetails
                                //        .FirstOrDefault(i => i.SDetailID == itemRM.SDetailID);
                                //    rmstockDetail.Quantity += itemRM.Quantity;
                                //    rmstockDetail.Status = 1;
                                //}

                                //Delete
                                //DbContext.ProductionRawMaterials.RemoveRange(existingChild.ProductionRawMaterials);
                                DbContext.ProductionDetails.Remove(existingChild);

                            }
                        }

                        //update and insert children
                        foreach (var newPDetail in newProduction.ProductionDetails)
                        {
                            var oldPDetail = DbContext.ProductionDetails.FirstOrDefault(i => i.PDetailID == newPDetail.PDetailID);
                            //update
                            if (oldPDetail != null)
                            {

                                //Return Fin from Stock
                                var oldstock = DbContext.Stocks.FirstOrDefault(i => i.ProductID == oldPDetail.ProductID);
                                oldstock.Quantity -= oldPDetail.Quantity;
                                oldstock.ModifiedBy = newProduction.ModifiedBy;
                                oldstock.ModifiedDate = newProduction.ModifiedDate;

                                var oldstockDetail = DbContext.StockDetails
                                   .Where(i => i.PDetailID == oldPDetail.PDetailID);
                                DbContext.StockDetails.RemoveRange(oldstockDetail);

                                //Add Raw materials in Stock

                                //foreach (var itemRM in oldPDetail.ProductionRawMaterials)
                                //{
                                //    var rmstock = DbContext.Stocks.FirstOrDefault(i => i.ProductID == itemRM.ProductID);
                                //    rmstock.Quantity += itemRM.Quantity;

                                //    var rmstockDetail = DbContext.StockDetails
                                //        .FirstOrDefault(i => i.SDetailID == itemRM.SDetailID);
                                //    rmstockDetail.Quantity += itemRM.Quantity;
                                //    rmstockDetail.Status = 1;
                                //}

                                //Delete
                                //DbContext.ProductionRawMaterials.RemoveRange(oldPDetail.ProductionRawMaterials);

                                //RawMaterialStockUpdate(newProduction, newPDetail);

                                // insert
                                //foreach (var newRM in newPDetail.ProductionRawMaterials)
                                //{
                                //    newRM.PDetailID = newPDetail.PDetailID;
                                //    DbContext.ProductionRawMaterials.Add(newRM);
                                //}


                                FinGoodsStockUpdate(newProduction, newPDetail);
                                DbContext.Entry(oldPDetail).CurrentValues.SetValues(newPDetail);

                            }
                            else //insert
                            {
                                newPDetail.ProductionID = ProductionID;
                                DbContext.ProductionDetails.Add(newPDetail);
                                //RawMaterialStockUpdate(newProduction, newPDetail);
                                DbContext.SaveChanges();
                                FinGoodsStockUpdate(newProduction, newPDetail);
                            }
                        }
                    }
                    else // New DO Add
                    {
                        DbContext.Productions.Add(newProduction);
                        //foreach (var item in newProduction.ProductionDetails)
                        //{
                        //    RawMaterialStockUpdate(newProduction, item);
                        //}
                        DbContext.SaveChanges();
                        foreach (var item in newProduction.ProductionDetails)
                        {
                            FinGoodsStockUpdate(newProduction, item);
                        }
                    }

                    //DbContext.SaveChanges();
                    trans.Commit();
                    Result = new Tuple<bool, int>(true, newProduction.ProductionID);
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    Result = new Tuple<bool, int>(false, 0);
                }
            };

            return Result;
        }

        private void FinGoodsStockUpdate(Production newProduction, ProductionDetail oPDetail)
        {
            #region Fin Goods Stock Update
            #region Stock Update
            Stock stock = null;
            Product product = null;
            ProductUnitType convertvalue = null;

            product = DbContext.Products.FirstOrDefault(i => i.ProductID == oPDetail.ProductID);
            convertvalue = DbContext.ProductUnitTypes.FirstOrDefault(i => i.ProUnitTypeID == product.ProUnitTypeID);
            decimal PRate = product.DP / convertvalue.ConvertValue;
            decimal SRate = product.MRP / convertvalue.ConvertValue;

            stock = DbContext.Stocks.FirstOrDefault(i => i.ProductID == oPDetail.ProductID);
            if (stock != null)
            {
                stock.Quantity += oPDetail.Quantity;
                stock.LPPrice = PRate;
                stock.ModifiedDate = newProduction.CreateDate;
                stock.ModifiedBy = newProduction.CreatedBy;
            }
            else
            {
                stock = new Stock();
                stock.ProductID = oPDetail.ProductID;
                stock.StockCode = DbContext.Products.FirstOrDefault(i => i.ProductID == oPDetail.ProductID).Code;
                stock.Quantity = oPDetail.Quantity;
                stock.LPPrice = PRate;
                stock.ColorID = DbContext.Models.FirstOrDefault(i => i.ConcernID == newProduction.ConcernID).ColorID;
                stock.GodownID = DbContext.Godowns.FirstOrDefault(i => i.ConcernID == newProduction.ConcernID).GodownID;
                stock.ConcernID = newProduction.ConcernID;
                stock.CreateDate = newProduction.CreateDate;
                stock.EntryDate = newProduction.CreateDate;
                stock.CreatedBy = newProduction.CreatedBy;
                DbContext.Stocks.Add(stock);

            }
            #endregion

            #region Stock Detail

            StockDetail stockDetail = new StockDetail();
            stockDetail.ProductID = oPDetail.ProductID;
            stockDetail.StockCode = DbContext.Products.FirstOrDefault(i => i.ProductID == oPDetail.ProductID).Code;
            stockDetail.ColorID = DbContext.Models.FirstOrDefault(i => i.ConcernID == newProduction.ConcernID).ColorID;
            stockDetail.GodownID = DbContext.Godowns.FirstOrDefault(i => i.ConcernID == newProduction.ConcernID).GodownID;
            stockDetail.Quantity = oPDetail.Quantity;
            stockDetail.Status = 1;
            stockDetail.PRate = PRate;
            stockDetail.SRate = SRate;
            stockDetail.StockID = stock.StockID;
            stockDetail.IMENO = "No Barcode";
            stockDetail.PDetailID = oPDetail.PDetailID;
            DbContext.StockDetails.Add(stockDetail);
            DbContext.SaveChanges();
            //foreach (var imei in oPDetail.ProductionIMEIs)
            //{
            //    StockDetail stockDetail = new StockDetail();
            //    stockDetail.ProductID = oPDetail.ProductID;
            //    stockDetail.StockCode = DbContext.Products.FirstOrDefault(i => i.ProductID == oPDetail.ProductID).Code;
            //    stockDetail.ColorID = 1;
            //    stockDetail.GodownID = 1;
            //    stockDetail.Quantity = 0m;
            //    stockDetail.Status = 1;
            //    stockDetail.PRate = PRate;
            //    stockDetail.StockID = stock.StockID;
            //    stockDetail.IMENO = imei.IMEI;
            //    stockDetail.PDetailID = oPDetail.PDetailID;
            //    DbContext.StockDetails.Add(stockDetail);
            //}
            #endregion
            #endregion

        }

        private void RawMaterialStockUpdate(Production newProductionSetup, ProductionDetail oPDetail)
        {
            Stock stock = null;

            #region Out Raw materials from Stock
            decimal rawOutQty = 0m, TotalCost = 0m;
            foreach (var rwitem in oPDetail.ProductionRawMaterials)
            {
                stock = DbContext.Stocks.FirstOrDefault(i => i.ProductID == rwitem.ProductID);
                stock.Quantity -= rwitem.Quantity;
                stock.ModifiedDate = newProductionSetup.CreateDate;
                stock.ModifiedBy = newProductionSetup.CreatedBy;
                rawOutQty = rwitem.Quantity;
                var stockDetails = DbContext.StockDetails.Where(i => i.ProductID == rwitem.ProductID && i.Status == 1 && i.Quantity > 0)
                                     .OrderByDescending(i => i.SDetailID);

                foreach (var rwstock in stockDetails)
                {
                    if (rawOutQty <= rwstock.Quantity)
                    {
                        rwstock.Quantity -= rawOutQty;
                        rwstock.Status = rwstock.Quantity == 0 ? 2 : 1;
                        TotalCost += rwstock.PRate * rawOutQty;
                        rawOutQty = 0m;
                        rwitem.SDetailID = rwstock.SDetailID;
                        rwitem.PRate = rwstock.PRate;
                        break;
                    }
                    else if (rawOutQty > rwstock.Quantity)
                    {
                        rawOutQty -= rwstock.Quantity;
                        rwitem.SDetailID = rwstock.SDetailID;
                        rwitem.PRate = rwstock.PRate;
                        TotalCost += rwstock.PRate * rwstock.Quantity;
                        rwstock.Quantity = 0m;
                        rwstock.Status = 2;
                    }
                }

            }
            oPDetail.TotalCost = TotalCost;
            #endregion
        }

        public bool DeleteByID(int ProductionID, int UserID, DateTime ModifiedDate)
        {

            bool Result = false;
            using (var trans = DbContext.Database.BeginTransaction())
            {
                try
                {
                    if (ProductionID > 0)
                    {
                        var OldFBEntry = DbContext.Productions.Find(ProductionID);
                        if (OldFBEntry != null)
                        {
                            foreach (var oldPDetail in OldFBEntry.ProductionDetails)
                            {
                                //Return Fin from Stock
                                var oldstock = DbContext.Stocks.FirstOrDefault(i => i.ProductID == oldPDetail.ProductID);
                                oldstock.Quantity -= oldPDetail.Quantity;
                                oldstock.ModifiedBy = OldFBEntry.ModifiedBy;
                                oldstock.ModifiedDate = OldFBEntry.ModifiedDate;

                                var oldstockDetail = DbContext.StockDetails
                                   .Where(i => i.PDetailID == oldPDetail.PDetailID);
                                DbContext.StockDetails.RemoveRange(oldstockDetail);

                                //Add Raw materials in Stock
                                foreach (var itemRM in oldPDetail.ProductionRawMaterials)
                                {
                                    var rmstock = DbContext.Stocks.FirstOrDefault(i => i.ProductID == itemRM.ProductID);
                                    rmstock.Quantity += itemRM.Quantity;

                                    var rmstockDetail = DbContext.StockDetails
                                        .FirstOrDefault(i => i.SDetailID == itemRM.SDetailID);
                                    rmstockDetail.Quantity += itemRM.Quantity;
                                    rmstockDetail.Status = 1;
                                }
                            }

                            OldFBEntry.ModifiedBy = UserID;
                            OldFBEntry.ModifiedDate = ModifiedDate;
                            OldFBEntry.Status = EnumProductionStatus.Return;
                        }
                        Result = DbContext.SaveChanges() > 0;
                        trans.Commit();
                    }
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    Result = false;
                }
            };
            return Result;
        }
    }
}
