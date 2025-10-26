using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Data
{
    public static class ProductionExtensions
    {
        public static IEnumerable<ProductionReportModel> GetProductionDetailDataByFinProductID(
        this IBaseRepository<Production> productionRepository, IBaseRepository<ProductionDetail> productionDetailRepository,
                                                            IBaseRepository<Product> productRepository, IBaseRepository<StockDetail> stockdetailRepository,
                                                            IBaseRepository<ProductionRawMaterial> productionRawMaterialRepository,
                                                            DateTime fromDate, DateTime toDate, int ProductID)
        {

            IQueryable<Product> Products = null;
            if (ProductID > 0)
                Products = productRepository.All.Where(i => i.ProductID == ProductID);
            else
                Products = productRepository.All;
            var oProductionDetailData = (from PD in productionRepository.All
                                         join POD in productionDetailRepository.All on PD.ProductionID equals POD.ProductionID
                                         join PDR in productionRawMaterialRepository.All on POD.PDetailID equals PDR.PDetailID
                                         join FP in Products on POD.ProductID equals FP.ProductID
                                         join RP in productRepository.All on PDR.ProductID equals RP.ProductID
                                         join std in stockdetailRepository.All on PDR.SDetailID equals std.SDetailID
                                         where (PD.Date >= fromDate && PD.Date <= toDate && PD.Status == EnumProductionStatus.Production)
                                         select new ProductionReportModel
                                         {
                                             FinProductID = POD.ProductionID,
                                             FinProductName = POD.Product.ProductName,
                                             ProductionID = PD.ProductionID,
                                             ProductionCode = PD.ProductionCode,
                                             ProductionDate = PD.Date,
                                             FinQuantity = POD.Quantity,
                                             RawProductID = PDR.ProductID,
                                             RawProductName = PDR.Product.ProductName,
                                             RawPRate = PDR.PRate,
                                             RawPRateTotal = PDR.Quantity * PDR.PRate,
                                             RawQuantity = PDR.Quantity,
                                             FinConvertValue = FP.ProductUnitType.ConvertValue,
                                             RawConvertValue = RP.ProductUnitType.ConvertValue,
                                             TotalCost = POD.TotalCost,
                                             FinParentUnitName = POD.Product.ProductUnitType.Description,
                                             FinChildUnitName = POD.Product.ProductUnitType.UnitName,
                                             RawParentUnitName = PDR.Product.ProductUnitType.Description,
                                             RawChildUnitName = PDR.Product.ProductUnitType.UnitName,
                                         }).OrderByDescending(x => x.ProductionID).ToList();


            return oProductionDetailData;
        }

    }
}
