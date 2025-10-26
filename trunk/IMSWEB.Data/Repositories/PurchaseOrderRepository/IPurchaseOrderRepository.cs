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
    public interface IPurchaseOrderRepository
    {
        bool AddPurchaseOrderUsingSP(DataTable dtPurchaseOrder, DataTable dtPODetail,
            DataTable dtPOProductDetail, DataTable dtStock, DataTable dtStockDetail);
        bool AddReturnPurchaseOrderUsingSP(DataTable dtPurchaseOrder, DataTable dtPODetail,
          DataTable dtPOProductDetail);
        bool AddDeliveryOrderUsingSP(DataTable dtPurchaseOrder, DataTable dtPODetail,DataTable dtPOProductDetail, DataTable dtStock, DataTable dtStockDetail);
        bool UpdatePurchaseOrderUsingSP(int purchaseOrderId, DataTable dtPurchaseOrder, DataTable dtPODetail,
            DataTable dtPOProductDetail, DataTable dtStock, DataTable dtStockDetail);
        bool UpdateDeliveryOrderUsingSP(int purchaseOrderId, DataTable dtPurchaseOrder, DataTable dtPODetail,
        DataTable dtPOProductDetail, DataTable dtStock, DataTable dtStockDetail);
        void DeletePurchaseOrderDetailUsingSP(int supplierId, int porderDetailId, int productId,
            int colorId, int userId, decimal quantity, decimal totalDue, DataTable dtPOProductDetail);

        bool DeletePurchaseOrderUsingSP(int orderId, int userId);

        int CheckProductStatusByPOId(int id);

        int CheckProductStatusByPODetailId(int id);

        int CheckIMENoDuplicacyByConcernId(int concernId, string imeNo);
        bool IsProductPurchase(int ProductID);
        Tuple<bool, int> AddRawStockOutOrderUsingSP(DataTable dtPurchaseOrder, DataTable dtPODetail,
          DataTable dtPOProductDetail);
    }
}
