using IMSWEB.Model;
using IMSWEB.Model.SPModel;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Service
{
    public interface IStockService
    {
        void AddStock(Stock stock);
        void SaveStock();
        IEnumerable<Stock> GetAllStock();
        Stock GetStockById(int id);
        Stock GetStockByProductIdandGodownID(int ProductID, int GodownID);
        Stock GetStockByProductIdandColorIDandGodownID(int ProductID, int GodownID, int ColorID);

        Stock GetStockByProductId(int id);
        Task<IEnumerable<Tuple<int, string, string, string,
            decimal, decimal, decimal, Tuple<string, int, int, decimal, decimal, decimal, decimal, Tuple<string>>>>> GetAllStockAsync();

        Task<IEnumerable<Tuple<int, string, string, string,
            string, string, string, Tuple<string>>>> GetAllStockDetailAsync();

        IEnumerable<ProductWisePurchaseModel> GetforStockReport(string userName, int concernId, int reportType, int CompanyID, int CategoryID, int ProductID, int GodownID, EnumProductStockType ProductStockType);
        void DeleteStock(int id);
        IEnumerable<Tuple<string, string, decimal, decimal, decimal, decimal, DateTime>> GetPriceProtectionReport(string userName, int concernId, DateTime dFDate, DateTime dTDate);
        IEnumerable<Tuple<int, string, string>> GetStockDetailsByID(int stockId);
        IEnumerable<DailyStockVSSalesSummaryReportModel> DailyStockVSSalesSummary(DateTime fromDate, DateTime toDate, int concernID, int ProductID);
        bool IsIMEIAvailableForSRVisit(int ProductID, int ColorID, string IMEI);
        string GetStockProductsHistory(int StockID);
        List<StockLedger> GetStockLedgerReport(int reportType, int CompanyID, int CategoryID, int ProductID, DateTime dFDate, DateTime dTDate);
        IQueryable<ProductDetailsModel> GetStockDetails();
        List<ProductDetailsModel> GetSupplierStockDetails(int SupplierID, int ProductID, int ColorID, int GodownID);
        List<ProductDetailsModel> GetStockProductsBySupplier(int SupplierID);

    }
}
