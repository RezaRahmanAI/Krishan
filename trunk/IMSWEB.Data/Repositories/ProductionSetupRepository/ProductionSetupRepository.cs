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
    public class ProductionSetupRepository : IProductionSetupRepository
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

        public ProductionSetupRepository(IDbFactory dbFactory)
        {
            DbFactory = dbFactory;
        }

        #endregion

        public Tuple<bool, int> ADDPS(ProductionSetup newProductionSetup, int PSID)
        {

            Tuple<bool, int> Result = new Tuple<bool, int>(false, 0);

            using (var trans = DbContext.Database.BeginTransaction())
            {
                try
                {
                    if (PSID > 0)//update 
                    {
                        var oldProductionSetup = DbContext.ProductionSetups.Where(i => i.PSID == PSID)
                                    .Include(i => i.ProductionSetupDetails).SingleOrDefault();
                        newProductionSetup.CreatedBy = oldProductionSetup.CreatedBy;
                        newProductionSetup.CreateDate = oldProductionSetup.CreateDate;
                        newProductionSetup.ConcernID = oldProductionSetup.ConcernID;

                        //update Parent
                        DbContext.Entry(oldProductionSetup).CurrentValues.SetValues(newProductionSetup);

                        //delete children
                        foreach (var existingChild in oldProductionSetup.ProductionSetupDetails.ToList())
                        {
                            if (!newProductionSetup.ProductionSetupDetails.Any(c => c.PSDID == existingChild.PSDID))
                                DbContext.ProductionSetupDetails.Remove(existingChild);
                        }

                        //update and insert children
                        foreach (var item in newProductionSetup.ProductionSetupDetails)
                        {
                            var oldChild = DbContext.ProductionSetupDetails.FirstOrDefault(i => i.PSDID == item.PSDID);
                            //update
                            if (oldChild != null)
                                DbContext.Entry(oldChild).CurrentValues.SetValues(item);
                            else //insert
                            {
                                item.PSID = PSID;
                                DbContext.ProductionSetupDetails.Add(item);
                            }
                        }
                    }
                    else // New DO Add
                    {
                        DbContext.ProductionSetups.Add(newProductionSetup);
                    }

                    DbContext.SaveChanges();
                    trans.Commit();
                    Result = new Tuple<bool, int>(true, newProductionSetup.PSID);
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    Result = new Tuple<bool, int>(false, 0);
                }
            };

            return Result;
        }

        public bool DeleteByID(int PSID)
        {

            bool Result = false;
            using (var trans = DbContext.Database.BeginTransaction())
            {
                try
                {
                    if (PSID > 0)
                    {
                        var OldFBEntry = DbContext.ProductionSetups.Find(PSID);
                        if (OldFBEntry != null)
                        {
                            DbContext.ProductionSetups.Remove(OldFBEntry);
                        }
                        Result = DbContext.SaveChanges() > 0;
                        trans.Commit();
                    }
                }
                catch (Exception)
                {
                    trans.Rollback();
                    Result = false;
                }
            };
            return Result;
        }
    }
}
