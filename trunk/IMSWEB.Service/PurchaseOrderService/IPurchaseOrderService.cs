using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Service
{
    public interface IPurchaseOrderService
    {
        IQueryable<POrder> GetAllIQueryable();
        Task<IEnumerable<Tuple<int, string, DateTime, string,
            string, string, EnumPurchaseType>>> GetAllPurchaseOrderAsync(DateTime fromDate, DateTime toDate);

        Task<IEnumerable<Tuple<int, string, DateTime, string,
          string, string, EnumPurchaseType>>> GetAllReturnPurchaseOrderAsync();

        Task<IEnumerable<Tuple<int, string, DateTime, string,
       string, string, EnumPurchaseType>>> GetAllDeliveryOrderAsync();

        Task<IEnumerable<Tuple<int, string, DateTime, string, string, string, EnumPurchaseType>>> GetAllDamageReturnOrderAsync();
        POrder GetDamagerReturnOrderByChallanNo(string ChallanNo);
        void AddPurchaseOrder(POrder purchaseOrder);
        
        bool AddPurchaseOrderUsingSP(DataTable dtPurchaseOrder, DataTable dtPODetail,
            DataTable dtPOProductDetail, DataTable dtStock, DataTable dtStockDetail);

        bool AddReturnPurchaseOrderUsingSP(DataTable dtPurchaseOrder, DataTable dtPODetail,
          DataTable dtPOProductDetail);

        bool UpdatePurchaseOrderUsingSP(int purchaseOrderId, DataTable dtPurchaseOrder, DataTable dtPODetail,
            DataTable dtPOProductDetail, DataTable dtStock, DataTable dtStockDetail);
        bool UpdateDeliveryOrderUsingSP(int purchaseOrderId, DataTable dtPurchaseOrder, DataTable dtPODetail,
    DataTable dtPOProductDetail, DataTable dtStock, DataTable dtStockDetail);

        void DeletePurchaseOrderDetailUsingSP(int supplierId, int porderDetailId, int productId,
            int colorId, int userId, decimal quantity, decimal totalDue, DataTable dtPOProductDetail);

        void SavePurchaseOrder();

        POrder GetPurchaseOrderById(int id);

        bool DeletePurchaseOrderUsingSP(int id, int userId);

        int CheckProductStatusByPOId(int id);

        int CheckIMENoDuplicacyByConcernId(int concernId, string imeNo);

        int CheckProductStatusByPODetailId(int id);

        IEnumerable<Tuple<string, string, DateTime, string, decimal, decimal, decimal, Tuple<decimal, decimal>>>
            GetPurchaseReport(DateTime fromDate, DateTime toDate,EnumPurchaseType PurchaseType);


        IEnumerable<Tuple<DateTime, string, string, decimal, decimal, decimal, decimal, Tuple<decimal, decimal, decimal, decimal, string, string, string, Tuple<decimal>>>>
           GetPurchaseDetailReportByConcernID(DateTime fromDate, DateTime toDate, EnumPurchaseType PurchaseType);

        IEnumerable<Tuple<DateTime, string, string, decimal, decimal, decimal, decimal, Tuple<decimal, decimal, decimal, decimal, decimal, string, Tuple<string, string, string, string, string, decimal, decimal>>>>
        GetPurchaseDetailReportBySupplierID(DateTime fromDate, DateTime toDate, int concernID, int supplierID);



        IEnumerable<Tuple<DateTime, string, string, decimal, decimal>> GetPurchaseByProductID(DateTime fromDate, DateTime toDate, int concernID, int productID);

        AdvanceSearchModel AdvanceSearchByIMEI(int ConcernID, string IMEINO);

        List<ProductWisePurchaseModel> ProductWisePurchaseReport(DateTime fromDate, DateTime toDate, int concernID, int supplierID, EnumPurchaseType PurchaseType);
        List<ProductWisePurchaseModel> ProductWisePurchaseDetailsReport(int CompanyID, int CategoryID, int ProductID, DateTime fromDate, DateTime toDate, EnumPurchaseType PurchaseType);
        AdvanceSearchModel SRVisitAdvanceSearchByIMEI(int ConcernID, string IMEINO);
        bool AddDeliveryOrderUsingSP(DataTable dtPurchaseOrder, DataTable dtPODetail,
        DataTable dtPOProductDetail, DataTable dtStock, DataTable dtStockDetail);
        POProductDetail GetDamagePOPDetail(string DamageIMEI, int ProductID, int ColorID);

        IEnumerable<ProductWisePurchaseModel> GetDamagePOReport(DateTime fromDate, DateTime toDate, int SupplierID);
        IEnumerable<ProductWisePurchaseModel> GetDamageReturnProductDetails(int ProductID, int ColorID);
        IEnumerable<ProductWisePurchaseModel> DamageReturnProductDetailsReport(int SupplierID, DateTime fromDate, DateTime toDate);
        IQueryable<ProductWisePurchaseModel> AdminPurchaseReport(DateTime fromDate, DateTime toDate, int ConcernID);

        List<LedgerAccountReportModel> SupplierLedger(DateTime fromdate, DateTime todate, int SupplierID);
        bool IsProductPurchase(int ProductID);
        Task<IEnumerable<Tuple<int, string, DateTime, string, string, string, EnumPurchaseType>>> GetAllRawStockDeductOrderAsync(DateTime fromDate, DateTime toDate);
        Tuple<bool, int> AddRawStockOutOrderUsingSP(DataTable dtPurchaseOrder, DataTable dtPODetail,
        DataTable dtPOProductDetail);
    }
}
