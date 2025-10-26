using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Data
{
    public static class PurchaseOrderExtensions
    {
        public static async Task<IEnumerable<Tuple<int, string, DateTime, string,
        string, string, EnumPurchaseType>>> GetAllPurchaseOrderAsync(this IBaseRepository<POrder> purchaseOrderRepository,
            IBaseRepository<Supplier> supplierRepository, DateTime fromDate, DateTime toDate)
        {
            IQueryable<Supplier> suppliers = supplierRepository.All;

            var items = await purchaseOrderRepository.All.Where(i => i.Status != (int)EnumPurchaseType.DeliveryOrder && i.Status != (int)EnumPurchaseType.DamageReturn && i.Status != (int)EnumPurchaseType.ProductReturn && i.Status != (int)EnumPurchaseType.RawStockDeduct && (i.OrderDate >= fromDate && i.OrderDate <= toDate)).
                GroupJoin(suppliers, p => p.SupplierID, s => s.SupplierID,
                (p, s) => new { PurchaseOrder = p, Suppliers = s }).
                SelectMany(x => x.Suppliers.DefaultIfEmpty(), (p, s) => new { PurchaseOrder = p.PurchaseOrder, Supplier = s })
                .Select(x => new
                {
                    x.PurchaseOrder.POrderID,
                    x.PurchaseOrder.ChallanNo,
                    x.PurchaseOrder.OrderDate,
                    x.Supplier.Name,
                    x.Supplier.OwnerName,
                    x.Supplier.ContactNo,
                    x.PurchaseOrder.Status
                }).ToListAsync();

            return items.Select(x => new Tuple<int, string, DateTime, string, string, string, EnumPurchaseType>
                (
                    x.POrderID,
                    x.ChallanNo,
                    x.OrderDate,
                    x.Name,
                    x.OwnerName,
                    x.ContactNo,
                    (EnumPurchaseType)x.Status
                )).OrderByDescending(x => x.Item1).ToList();
        }

        public static async Task<IEnumerable<Tuple<int, string, DateTime, string,
string, string, EnumPurchaseType>>> GetAllDeliveryOrderAsync(this IBaseRepository<POrder> purchaseOrderRepository,
    IBaseRepository<Supplier> supplierRepository)
        {
            IQueryable<Supplier> suppliers = supplierRepository.All;

            var items = await purchaseOrderRepository.All.Where(i => i.Status == (int)EnumPurchaseType.DeliveryOrder).
                GroupJoin(suppliers, p => p.SupplierID, s => s.SupplierID,
                (p, s) => new { PurchaseOrder = p, Suppliers = s }).
                SelectMany(x => x.Suppliers.DefaultIfEmpty(), (p, s) => new { PurchaseOrder = p.PurchaseOrder, Supplier = s })
                .Select(x => new
                {
                    x.PurchaseOrder.POrderID,
                    x.PurchaseOrder.ChallanNo,
                    x.PurchaseOrder.OrderDate,
                    x.Supplier.Name,
                    x.Supplier.OwnerName,
                    x.Supplier.ContactNo,
                    x.PurchaseOrder.Status
                }).ToListAsync();

            return items.Select(x => new Tuple<int, string, DateTime, string, string, string, EnumPurchaseType>
                (
                    x.POrderID,
                    x.ChallanNo,
                    x.OrderDate,
                    x.Name,
                    x.OwnerName,
                    x.ContactNo,
                    (EnumPurchaseType)x.Status
                )).OrderByDescending(x => x.Item1).ToList();
        }

        public static async Task<IEnumerable<Tuple<int, string, DateTime, string,
string, string, EnumPurchaseType>>> GetAllDamageReturnOrderAsync(this IBaseRepository<POrder> purchaseOrderRepository,
IBaseRepository<Supplier> supplierRepository)
        {
            IQueryable<Supplier> suppliers = supplierRepository.All;

            var items = await purchaseOrderRepository.All.Where(i => i.Status == (int)EnumPurchaseType.DamageReturn).
                GroupJoin(suppliers, p => p.SupplierID, s => s.SupplierID,
                (p, s) => new { PurchaseOrder = p, Suppliers = s }).
                SelectMany(x => x.Suppliers.DefaultIfEmpty(), (p, s) => new { PurchaseOrder = p.PurchaseOrder, Supplier = s })
                .Select(x => new
                {
                    x.PurchaseOrder.POrderID,
                    x.PurchaseOrder.ChallanNo,
                    x.PurchaseOrder.OrderDate,
                    x.Supplier.Name,
                    x.Supplier.OwnerName,
                    x.Supplier.ContactNo,
                    x.PurchaseOrder.Status
                }).ToListAsync();

            return items.Select(x => new Tuple<int, string, DateTime, string, string, string, EnumPurchaseType>
                (
                    x.POrderID,
                    x.ChallanNo,
                    x.OrderDate,
                    x.Name,
                    x.OwnerName,
                    x.ContactNo,
                    (EnumPurchaseType)x.Status
                )).OrderByDescending(x => x.Item1).ToList();
        }

        public static async Task<IEnumerable<Tuple<int, string, DateTime, string,
   string, string, EnumPurchaseType>>> GetAllReturnPurchaseOrderAsync(this IBaseRepository<POrder> purchaseOrderRepository,
       IBaseRepository<Supplier> supplierRepository)
        {
            IQueryable<Supplier> suppliers = supplierRepository.All;

            var items = await purchaseOrderRepository.All.Where(i => i.Status == (int)EnumPurchaseType.ProductReturn).
                GroupJoin(suppliers, p => p.SupplierID, s => s.SupplierID,
                (p, s) => new { PurchaseOrder = p, Suppliers = s }).
                SelectMany(x => x.Suppliers.DefaultIfEmpty(), (p, s) => new { PurchaseOrder = p.PurchaseOrder, Supplier = s })
                .Select(x => new
                {
                    x.PurchaseOrder.POrderID,
                    x.PurchaseOrder.ChallanNo,
                    x.PurchaseOrder.OrderDate,
                    x.Supplier.Name,
                    x.Supplier.OwnerName,
                    x.Supplier.ContactNo,
                    x.PurchaseOrder.Status
                }).ToListAsync();

            return items.Select(x => new Tuple<int, string, DateTime, string, string, string, EnumPurchaseType>
                (
                    x.POrderID,
                    x.ChallanNo,
                    x.OrderDate,
                    x.Name,
                    x.OwnerName,
                    x.ContactNo,
                    (EnumPurchaseType)x.Status
                )).OrderByDescending(x => x.Item1).ToList();
        }
        public static IEnumerable<Tuple<string, string, DateTime, string, decimal, decimal, decimal, Tuple<decimal, decimal>>> GetPurchaseReport(this IBaseRepository<POrder> purchaseOrderRepository, IBaseRepository<Supplier> supplierRepository,
            DateTime fromDate, DateTime toDate, EnumPurchaseType PurchaseType)
        {
            var oPurchaseData = (from pOrd in purchaseOrderRepository.All
                                 join sup in supplierRepository.All on pOrd.SupplierID equals sup.SupplierID
                                 where (pOrd.OrderDate >= fromDate && pOrd.OrderDate <= toDate && pOrd.Status == (int)PurchaseType)
                                 group pOrd by new
                                 {
                                     sup.Code,
                                     sup.Name,
                                     pOrd.ChallanNo,
                                     pOrd.OrderDate,
                                     pOrd.GrandTotal,
                                     pOrd.NetDiscount,
                                     pOrd.TotalAmt,
                                     pOrd.RecAmt,
                                     pOrd.PaymentDue
                                 } into g
                                 select new
                                 {
                                     g.Key.Code,
                                     g.Key.Name,
                                     g.Key.OrderDate,
                                     g.Key.ChallanNo,
                                     g.Key.GrandTotal,
                                     g.Key.NetDiscount,
                                     g.Key.TotalAmt,
                                     g.Key.RecAmt,
                                     g.Key.PaymentDue
                                 }).ToList();
            return oPurchaseData.Select(x => new Tuple<string, string, DateTime, string, decimal, decimal, decimal, Tuple<decimal, decimal>>
                  (
                   x.Code,
                   x.Name,
                  x.OrderDate,
                                      x.ChallanNo,
                                      x.GrandTotal,
                                      x.NetDiscount,
                                      x.TotalAmt,
                                       new Tuple<decimal, decimal>(
                                      (decimal)x.RecAmt,
                                      x.PaymentDue)

                  ));
        }

        public static IEnumerable<Tuple<DateTime, string, string, decimal, decimal, decimal, decimal, Tuple<decimal, decimal, decimal, decimal, string, string, string, Tuple<decimal>>>>
           GetPurchaseDetailReportByConcernID(this IBaseRepository<POrder> purchaseOrderRepository, IBaseRepository<POrderDetail> pOrderDetailRepository,
            IBaseRepository<Product> productRepository, IBaseRepository<POProductDetail> poProductRepository, IBaseRepository<Color> colorRepository,
            DateTime fromDate, DateTime toDate, EnumPurchaseType PurchaseType)
        {
            var oPurchaseDetailData = (from POD in pOrderDetailRepository.All
                                       from PO in purchaseOrderRepository.All
                                       from P in productRepository.All
                                       from POP in poProductRepository.All
                                       from C in colorRepository.All
                                       where (POD.POrderID == PO.POrderID && POD.POrderDetailID == POP.POrderDetailID && P.ProductID == POD.ProductID && C.ColorID == POP.ColorID && PO.OrderDate >= fromDate && PO.OrderDate <= toDate && PO.Status == (int)PurchaseType)
                                       select new
                                       {
                                           PO.ChallanNo,
                                           PO.OrderDate,
                                           PO.GrandTotal,
                                           PO.NetDiscount,
                                           PO.TotalAmt,
                                           PO.RecAmt,
                                           PO.PaymentDue,
                                           P.ProductID,
                                           P.ProductName,
                                           POD.UnitPrice,
                                           POD.TAmount,
                                           PPDISAmt = POD.PPDISAmt + POD.ExtraPPDISAmt,
                                           CateName = P.Category.Description,
                                           POP.IMENO,
                                           ColorName = C.Name,
                                           POD.PPOffer
                                       }).OrderByDescending(x => x.OrderDate).ToList();

            return oPurchaseDetailData.Select(x => new Tuple<DateTime, string, string, decimal, decimal, decimal, decimal, Tuple<decimal, decimal, decimal, decimal, string, string, string, Tuple<decimal>>>
                (
                 x.OrderDate,
                 x.ChallanNo,
                x.ProductName,
                x.UnitPrice,
                x.PPDISAmt,
                x.TAmount,
                x.GrandTotal, new Tuple<decimal, decimal, decimal, decimal, string, string, string, Tuple<decimal>>(
                                    x.NetDiscount,
                                    x.TotalAmt,
                                   (decimal)x.RecAmt,
                                   x.PaymentDue,
                                   x.CateName,
                                   x.IMENO, x.ColorName,
                                   new Tuple<decimal>(x.PPOffer)
                                   )
                ));
        }


        public static IEnumerable<Tuple<decimal, int, decimal, decimal, int, int, decimal,
            Tuple<decimal, decimal, string, string, int, string, decimal, Tuple<decimal>>>>
            GetPurchaseOrderDetailById(this IBaseRepository<POrderDetail> purchaseOrderDetailRepository,
            IBaseRepository<Product> productRepository, IBaseRepository<Color> colorRepository,
            IBaseRepository<ProductUnitType> ProductUnitTypeRepository,
            int orderId)
        {

            var items = (from pod in purchaseOrderDetailRepository.All
                        join p in productRepository.All on pod.ProductID equals p.ProductID
                        join c in colorRepository.All on pod.ColorID equals c.ColorID
                        join pu in ProductUnitTypeRepository.All on p.ProUnitTypeID equals pu.ProUnitTypeID
                        where pod.POrderID==orderId
                        select new
                        {
                            pod.MRPRate,
                            pod.POrderDetailID,
                            pod.PPDISAmt,
                            pod.PPDISPer,
                            pod.ProductID,
                            pod.POrderID,
                            pod.Quantity,
                            pod.TAmount,
                            pod.UnitPrice,
                            p.ProductName,
                            ProductCode = p.Code,
                            c.ColorID,
                            ColorName = c.Name,
                            pod.SalesRate,
                            ConvertValue = p.BundleQty == 0 ? pu.ConvertValue : p.BundleQty
                        }).ToList();


            return items.Select(x => new Tuple<decimal, int, decimal, decimal, int, int, decimal,
                Tuple<decimal, decimal, string, string, int, string, decimal, Tuple<decimal>>>
                (
                    x.MRPRate,
                    x.POrderDetailID,
                    x.PPDISAmt,
                    x.PPDISPer,
                    x.ProductID,
                    x.POrderID,
                    x.Quantity,
                    new Tuple<decimal, decimal, string, string, int, string, decimal,Tuple<decimal>>
                    (
                        x.TAmount,
                        x.UnitPrice,
                        x.ProductName,
                        x.ProductCode,
                        x.ColorID,
                        x.ColorName,
                        x.SalesRate, new Tuple<decimal>(x.ConvertValue)
                    )
                ));
        }

        //public static IEnumerable<Tuple<DateTime, string, string, decimal, decimal, decimal, decimal, Tuple<decimal, decimal, decimal, decimal, decimal, string, string,Tuple<string, string, string, string, string, decimal>>>>
        //GetPurchaseDetailReportBySupplierID(this IBaseRepository<POrder> purchaseOrderRepository, IBaseRepository<POrderDetail> pOrderDetailRepository, IBaseRepository<Product> productRepository,
        //IBaseRepository<POProductDetail> poProductRepository, IBaseRepository<Color> colorRepository, DateTime fromDate, DateTime toDate, int concernID,int supplierId)
        //{
        //    var oPurchaseDetailData = (from POD in pOrderDetailRepository.All
        //                               from PO in purchaseOrderRepository.All
        //                               from P in productRepository.All
        //                               from POP in poProductRepository.All
        //                               from C in colorRepository.All
        //                               where (POD.POrderID == PO.POrderID && POD.POrderDetailID == POP.POrderDetailID && P.ProductID == POD.ProductID && C.ColorID == POP.ColorID && PO.OrderDate >= fromDate && PO.OrderDate <= toDate && PO.Status == 1 && PO.ConcernID == concernID && PO.SupplierID==supplierId)
        //                               select new { PO.ChallanNo, PO.OrderDate, PO.GrandTotal, PO.TDiscount, PO.TotalAmt, PO.RecAmt, PO.PaymentDue, P.ProductID, P.ProductName, POD.UnitPrice, POD.TAmount, POD.PPDISAmt, POD.Quantity, POP.IMENO, ColorName = C.Name }).OrderByDescending(x => x.OrderDate).ToList();

        //    return oPurchaseDetailData.Select(x => new Tuple<DateTime, string, string, decimal, decimal, decimal, decimal, Tuple<decimal, decimal, decimal, decimal, decimal, string, string>>
        //        (
        //         x.OrderDate,
        //         x.ChallanNo,
        //        x.ProductName,
        //        x.UnitPrice,
        //        x.PPDISAmt,
        //        x.TAmount,
        //        x.GrandTotal, new Tuple<decimal, decimal, decimal, decimal, decimal, string, string>(
        //                            x.TDiscount,
        //                            x.TotalAmt,
        //                           (decimal)x.RecAmt,
        //                           x.PaymentDue,
        //                           x.Quantity,
        //                           x.IMENO, x.ColorName)
        //        ));
        //}

        public static IEnumerable<Tuple<DateTime, string, string, decimal, decimal, decimal, decimal, Tuple<decimal, decimal, decimal, decimal, decimal, string, Tuple<string, string, string, string, string, decimal, decimal>>>>
        GetPurchaseDetailReportBySupplierID(this IBaseRepository<POrder> POrderRepository, IBaseRepository<POrderDetail> POrderDetailRepository, IBaseRepository<Product> productRepository,
        IBaseRepository<POProductDetail> POProductDetailRepository, DateTime fromDate, DateTime toDate, int ConcernID, int SupplierID)
        {
            var oSalesDetailData = (from POD in POrderDetailRepository.All
                                    from PO in POrderRepository.All
                                    from P in productRepository.All
                                    from POPD in POProductDetailRepository.All
                                    where (POD.POrderID == PO.POrderID && POD.POrderDetailID == POPD.POrderDetailID && P.ProductID == POD.ProductID && PO.OrderDate >= fromDate && PO.OrderDate <= toDate && PO.Status == 1 && PO.SupplierID == SupplierID)
                                    select new
                                    {
                                        PO.ChallanNo,
                                        PO.OrderDate,
                                        PO.GrandTotal,
                                        PO.NetDiscount,
                                        PO.TotalAmt,
                                        PO.RecAmt,
                                        PO.PaymentDue,
                                        P.ProductID,
                                        P.ProductName,
                                        POD.UnitPrice,
                                        POD.TAmount,
                                        PPDISAmt = POD.PPDISAmt + POD.ExtraPPDISAmt,
                                        POD.Quantity,
                                        POPD.IMENO,
                                        PO.Supplier.Name,
                                        PO.Supplier.Code,
                                        PO.Supplier.Address,
                                        PO.Supplier.ContactNo,
                                        PO.Supplier.OwnerName,
                                        PO.Supplier.TotalDue,
                                        POD.PPOffer
                                    }).OrderByDescending(x => x.OrderDate).ToList();

            return oSalesDetailData.Select(x => new Tuple<DateTime, string, string, decimal, decimal, decimal, decimal, Tuple<decimal, decimal, decimal, decimal, decimal, string, Tuple<string, string, string, string, string, decimal, decimal>>>
                (
                 x.OrderDate,
                 x.ChallanNo,
                x.ProductName,
                x.UnitPrice,
                x.PPDISAmt,
                x.TAmount,
                x.GrandTotal, new Tuple<decimal, decimal, decimal, decimal, decimal, string, Tuple<string, string, string, string, string, decimal, decimal>>(
                                    x.NetDiscount,
                                    x.TotalAmt,
                                   (decimal)x.RecAmt,
                                   x.PaymentDue,
                                   x.Quantity,
                                   x.IMENO,
                                   new Tuple<string, string, string, string, string, decimal, decimal>(
                                       x.Code,
                                       x.Name,
                                       x.Address,
                                       x.ContactNo,
                                       x.OwnerName,
                                       x.TotalDue,
                                       x.PPOffer
                                       ))
                ));
        }

        public static IEnumerable<Tuple<DateTime, string, string, decimal, decimal>> GetPurchaseByProductID(this IBaseRepository<POrder> POrderRepository, IBaseRepository<POrderDetail> POrderDetailRepository, IBaseRepository<Product> productRepository,
         DateTime fromDate, DateTime toDate, int ConcernID, int productid)
        {
            var opurchase = (from POD in POrderDetailRepository.All
                             from PO in POrderRepository.All
                             from P in productRepository.All
                             where (POD.POrderID == PO.POrderID && P.ProductID == POD.ProductID && PO.OrderDate >= fromDate && PO.OrderDate <= toDate && P.ProductID == productid && PO.Status == 1)
                             group POD by new { PO.ChallanNo, PO.OrderDate, P.ProductName, POD.UnitPrice } into g
                             select new { g.Key.ChallanNo, g.Key.OrderDate, g.Key.ProductName, g.Key.UnitPrice, Quantity = g.Sum(x => x.Quantity) }).OrderBy(x => x.OrderDate).ToList();

            return opurchase.Select(x => new Tuple<DateTime, string, string, decimal, decimal>
                (
                 x.OrderDate,
                 x.ChallanNo,
                x.ProductName,
                x.Quantity,
                x.UnitPrice
                ));

            //return _basePurchaseOrderRepository.GetPurchaseByProductID(_pOrderDetailRepository, _productRepository, _pOProductetailRepository, fromDate, toDate, ConcernId, productID);
        }

        public static AdvanceSearchModel AdvanceSearchByIMEI(this IBaseRepository<POrder> POrderRepository, IBaseRepository<POrderDetail> POrderDetailRepository, IBaseRepository<POProductDetail> POProductDetailRepository,
            IBaseRepository<Product> productRepository, IBaseRepository<SOrder> SOrderRepository, IBaseRepository<SOrderDetail> SOrderDetailRepository, IBaseRepository<StockDetail> StockDetailRepository,
            IBaseRepository<Stock> StockRepository, IBaseRepository<Supplier> SupplierRepository, IBaseRepository<Customer> CustomerRepository,
            IBaseRepository<CreditSale> CreditSaleRepository, IBaseRepository<CreditSaleDetails> CreditSaleDetailsRepository,
          int ConcernID, string IMEINO)
        {
            IMEINO = IMEINO.Trim();
            AdvanceSearchModel advanceSearchModel = new AdvanceSearchModel();
            IEnumerable<StockDetail> StockDetaillist = null;
            Stock objStock = null;
            StockDetail ConcernStockDetail = null;
            SOrderDetail objSOrderDetail = null;
            CreditSaleDetails objCreditSaleDetail = null;
            var Products = productRepository.All;
            var POrderDetails = POrderDetailRepository.All;
            var POProductDetails = POProductDetailRepository.All;
            var POProductDetail = POProductDetailRepository.All.Where(i => i.IMENO.Equals(IMEINO));
            var StockDetails = StockDetailRepository.All;
            var CreditSales = CreditSaleRepository.All;
            var CreditSalesDetails = CreditSaleDetailsRepository.All;

            if (POProductDetail != null)
            {
                var POrderDetail = POrderDetailRepository.All.Where(i => (POProductDetail.Select(j => j.POrderDetailID).Contains(i.POrderDetailID)) && (POProductDetail.Select(j => j.ProductID).Contains(i.ProductID)) && (POProductDetail.Select(j => j.ColorID).Contains(i.ColorID)));
                var POrder = POrderRepository.All.FirstOrDefault(i => i.ConcernID == ConcernID && (POrderDetail.Select(j => j.POrderID).Contains(i.POrderID)));
                if (POrder != null)
                {
                    var concernPOrderDetail = POrderDetails.Where(i => i.POrderID == POrder.POrderID);
                    var ConcernPOProductDetail = POProductDetails.Where(i => (concernPOrderDetail.Select(j => j.POrderDetailID).Contains(i.POrderDetailID)));

                    var POResult = (from POD in concernPOrderDetail
                                    join POPD in ConcernPOProductDetail on POD.POrderDetailID equals POPD.POrderDetailID
                                    join P in Products on POD.ProductID equals P.ProductID
                                    select new AdvancePODetail
                                    {
                                        ProductCode = P.Code,
                                        ProductName = P.ProductName,
                                        PurchaseRate = POD.UnitPrice,
                                        Quantity = 1,
                                        IMEI = POPD.IMENO
                                    }).ToList();
                    var SelectedIMEI = POResult.FirstOrDefault(i => i.IMEI.Equals(IMEINO));
                    POResult.Remove(SelectedIMEI);
                    POResult.Insert(0, SelectedIMEI);
                    advanceSearchModel.AdvancePODetails.AddRange(POResult);
                    advanceSearchModel.ChallanNo = POrder.ChallanNo;
                    advanceSearchModel.PurchaseDate = POrder.OrderDate;
                    var Supplier = SupplierRepository.FindBy(i => i.SupplierID == POrder.SupplierID).FirstOrDefault();
                    advanceSearchModel.SupplierCode = Supplier.Code;
                    advanceSearchModel.SupplierName = Supplier.Name;
                }

            }

            StockDetaillist = StockDetails.Where(i => i.IMENO.Equals(IMEINO) && i.Status == 2);

            if (StockDetaillist != null || StockDetaillist.Count() != 0)
            {
                //  objStock = StockRepository.All.FirstOrDefault(i => i.ConcernID == ConcernID && (StockDetaillist.Select(j => j.StockID).Contains(i.StockID)));
                objStock = StockRepository.All.FirstOrDefault();
                if (objStock != null)
                {
                    ConcernStockDetail = StockDetails.FirstOrDefault(i => i.StockID == objStock.StockID && i.Status == 2 && i.IMENO.Equals(IMEINO));

                    if (ConcernStockDetail != null)
                    {
                        objSOrderDetail = SOrderDetailRepository.All.Where(i => i.SDetailID == ConcernStockDetail.SDetailID && i.IsProductReturn == 0).OrderByDescending(i => i.SOrderDetailID).FirstOrDefault();
                        bool IsReplaceOrder = false;
                        if (objSOrderDetail == null)//Replace Order
                        {
                            objSOrderDetail = SOrderDetailRepository.All.Where(i => i.RStockDetailID == ConcernStockDetail.SDetailID && i.IsProductReturn == 0).OrderByDescending(i => i.SOrderDetailID).FirstOrDefault();
                            IsReplaceOrder = true;
                        }

                        if (objSOrderDetail != null) //Sales 
                        {
                            var objSorder = IsReplaceOrder ? SOrderRepository.All.FirstOrDefault(i => i.SOrderID == objSOrderDetail.RepOrderID) : SOrderRepository.All.FirstOrDefault(i => i.SOrderID == objSOrderDetail.SOrderID);

                            var SOrderDetails = IsReplaceOrder ? SOrderDetailRepository.All.Where(i => i.RepOrderID == objSorder.SOrderID && i.IsProductReturn == 0) : SOrderDetailRepository.All.Where(i => i.SOrderID == objSorder.SOrderID && i.IsProductReturn == 0);
                            List<AdvanceSOrderDetail> SOResult = new List<AdvanceSOrderDetail>();
                            if (!IsReplaceOrder)
                            {

                                SOResult = (from SOD in SOrderDetails
                                            join p in Products on SOD.ProductID equals p.ProductID
                                            join SD in StockDetails on SOD.SDetailID equals SD.SDetailID
                                            select new AdvanceSOrderDetail
                                            {
                                                ProductCode = p.Code,
                                                ProductName = p.ProductName,
                                                Quantity = SOD.Quantity,
                                                SalesRate = SOD.UnitPrice,
                                                IMEI = SD.IMENO
                                            }).ToList();
                            }
                            else //Replace Order
                            {
                                SOResult = (from SOD in SOrderDetails
                                            join p in Products on SOD.ProductID equals p.ProductID
                                            join SD in StockDetails on SOD.RStockDetailID equals SD.SDetailID
                                            select new AdvanceSOrderDetail
                                            {
                                                ProductCode = p.Code,
                                                ProductName = p.ProductName,
                                                Quantity = SOD.Quantity,
                                                SalesRate = SOD.UnitPrice,
                                                IMEI = SD.IMENO
                                            }).ToList();
                            }

                            var SelectedIMEI = SOResult.FirstOrDefault(i => i.IMEI.Equals(IMEINO));
                            SOResult.Remove(SelectedIMEI);
                            SOResult.Insert(0, SelectedIMEI);

                            advanceSearchModel.AdvanceSOrderDetails.AddRange(SOResult);
                            var customer = CustomerRepository.All.FirstOrDefault(i => i.CustomerID == objSorder.CustomerID);
                            advanceSearchModel.CustomerCode = customer.Code;
                            advanceSearchModel.CustomerName = customer.Name;
                            advanceSearchModel.SalesDate = objSorder.InvoiceDate;
                            advanceSearchModel.InvoiceNo = objSorder.InvoiceNo;
                        }
                        else //Credit Sales
                        {
                            objCreditSaleDetail = CreditSalesDetails.Where(i => i.StockDetailID == ConcernStockDetail.SDetailID).OrderByDescending(i => i.CreditSaleDetailsID).FirstOrDefault();
                            if (objCreditSaleDetail != null)
                            {
                                var objCreditSOrder = CreditSales.FirstOrDefault(i => i.CreditSalesID == objCreditSaleDetail.CreditSalesID);

                                var CreditSOrderDetails = CreditSalesDetails.Where(i => i.CreditSalesID == objCreditSOrder.CreditSalesID);

                                var SOResult = (from SOD in CreditSOrderDetails
                                                join p in Products on SOD.ProductID equals p.ProductID
                                                join SD in StockDetails on SOD.StockDetailID equals SD.SDetailID
                                                select new AdvanceSOrderDetail
                                                {
                                                    ProductCode = p.Code,
                                                    ProductName = p.ProductName,
                                                    Quantity = SOD.Quantity,
                                                    SalesRate = SOD.UnitPrice,
                                                    IMEI = SD.IMENO
                                                }).ToList();
                                var SelectedIMEI = SOResult.FirstOrDefault(i => i.IMEI.Equals(IMEINO));
                                SOResult.Remove(SelectedIMEI);
                                SOResult.Insert(0, SelectedIMEI);

                                advanceSearchModel.AdvanceSOrderDetails.AddRange(SOResult);
                                var customer = CustomerRepository.All.FirstOrDefault(i => i.CustomerID == objCreditSOrder.CustomerID);
                                advanceSearchModel.CustomerCode = customer.Code;
                                advanceSearchModel.CustomerName = customer.Name;
                                advanceSearchModel.SalesDate = objCreditSOrder.IssueDate;
                                advanceSearchModel.InvoiceNo = objCreditSOrder.InvoiceNo;
                            }
                        }

                    }
                }

            }
            return advanceSearchModel;
        }


        public static List<ProductWisePurchaseModel> ProductWisePurchaseReport(this IBaseRepository<POrder> POrderRepository, IBaseRepository<POrderDetail> POrderDetailRepository,
            IBaseRepository<Product> ProductRepository, IBaseRepository<Supplier> SupplierRepository, int ConcernID,
            int SupplierID, DateTime fromDate, DateTime toDate, EnumPurchaseType PurchaseType)
        {
            List<POrder> POrders = new List<POrder>();
            if (SupplierID != 0)
                POrders = POrderRepository.All.Where(i => i.SupplierID == SupplierID && i.ConcernID == ConcernID && i.OrderDate >= fromDate && i.OrderDate <= toDate && i.Status == (int)PurchaseType).ToList();
            else
                POrders = POrderRepository.All.Where(i => i.ConcernID == ConcernID && i.OrderDate >= fromDate && i.OrderDate <= toDate && i.Status == (int)PurchaseType).ToList();

            var POrderDetails = POrderDetailRepository.All;
            var Suppliers = SupplierRepository.All;
            var Products = ProductRepository.All;

            var result = from PO in POrders
                         join POD in POrderDetails on PO.POrderID equals POD.POrderID
                         join S in Suppliers on PO.SupplierID equals S.SupplierID
                         join P in Products on POD.ProductID equals P.ProductID
                         select new ProductWisePurchaseModel
                         {
                             Date = PO.OrderDate,
                             SupplierCode = S.Code,
                             SupplierName = S.Name,
                             Address = S.Address,
                             Mobile = S.ContactNo,
                             ProductName = P.ProductName,
                             Quantity = POD.Quantity,
                             PurchaseRate = POD.UnitPrice,
                             TotalAmount = POD.TAmount
                         };

            var fresult = from r in result
                          group r by new { r.Date, r.SupplierCode, r.SupplierName, r.Address, r.Mobile, r.ProductName, r.PurchaseRate } into g
                          select new ProductWisePurchaseModel
                          {
                              Date = g.Key.Date,
                              SupplierCode = g.Key.SupplierCode,
                              SupplierName = g.Key.SupplierName,
                              Address = g.Key.Address,
                              Mobile = g.Key.Mobile,
                              ProductName = g.Key.ProductName,
                              PurchaseRate = g.Key.PurchaseRate,
                              Quantity = g.Sum(i => i.Quantity),
                              TotalAmount = g.Sum(i => i.TotalAmount)
                          };

            return fresult.ToList();


        }

        public static List<ProductWisePurchaseModel> ProductWisePurchaseDetailsReport(this IBaseRepository<POrder> POrderRepository,
            IBaseRepository<POrderDetail> POrderDetailRepository, IBaseRepository<Product> ProductRepository, IBaseRepository<Company> CompanyRepository,
            IBaseRepository<Category> categoryRepository, IBaseRepository<Size> SizeRepository, IBaseRepository<ProductUnitType> ProductUnitTypeRepository,
            IBaseRepository<Color> ColorRepository,
            int CompanyID, int CategoryID, int ProductID, DateTime fromDate, DateTime toDate, EnumPurchaseType PurchaseType)
        {
            var Products = ProductRepository.All;
            if (CompanyID != 0)
                Products = Products.Where(i => i.CompanyID == CompanyID);
            if (CategoryID != 0)
                Products = Products.Where(i => i.CategoryID == CategoryID);
            if (ProductID != 0)
                Products = Products.Where(i => i.ProductID == ProductID);

            var POrders = POrderRepository.All.Where(i => i.OrderDate >= fromDate && i.OrderDate <= toDate && i.Status == (int)PurchaseType);
            var POrderDetails = POrderDetailRepository.All;

            var result = from PO in POrders
                         join POD in POrderDetails on PO.POrderID equals POD.POrderID
                         join P in Products on POD.ProductID equals P.ProductID
                         join com in CompanyRepository.All on P.CompanyID equals com.CompanyID
                         join cate in categoryRepository.All on P.CategoryID equals cate.CategoryID
                         join s in SizeRepository.All on P.SizeID equals s.SizeID
                         join pu in ProductUnitTypeRepository.All on (int)P.ProUnitTypeID equals pu.ProUnitTypeID
                         join c in ColorRepository.All on POD.ColorID equals c.ColorID
                         select new ProductWisePurchaseModel
                         {
                             POrderID = PO.POrderID,
                             Date = PO.OrderDate,
                             ChallanNo = PO.ChallanNo,
                             PaymentDue = PO.PaymentDue,
                             GrandTotal = PO.GrandTotal,
                             NetTotal = PO.TotalAmt,
                             NetDiscount = PO.NetDiscount,
                             RecAmt = PO.RecAmt,
                             ProductCode = string.IsNullOrEmpty(P.IDCode) ? P.Code : P.IDCode,
                             ProductName = P.ProductName,
                             CompanyName = com.Name,
                             CategoryName = cate.Description,
                             ColorName = c.Name,
                             ColorID = c.ColorID,
                             Quantity = POD.Quantity,
                             PurchaseRate = POD.UnitPrice,
                             TotalAmount = POD.TAmount,
                             SizeName = s.Description,
                             ChildUnitName = pu.UnitName,
                             ParentUnitName = pu.Description,
                             ConvertValue = P.BundleQty > 0 ? P.BundleQty : pu.ConvertValue,
                             PurchaseCSft = P.PurchaseCSft,
                             SalesCSft = P.SalesCSft
                         };

            return result.ToList();


        }

        /// <summary>
        /// For SRVisit Issued IMEI search
        /// </summary>
        /// <param name="POrderRepository"></param>
        /// <param name="POrderDetailRepository"></param>
        /// <param name="POProductDetailRepository"></param>
        /// <param name="productRepository"></param>
        /// <param name="StockDetailRepository"></param>
        /// <param name="StockRepository"></param>
        /// <param name="SupplierRepository"></param>
        /// <param name="SRVisitRepository"></param>
        /// <param name="SRVisitDetailRepository"></param>
        /// <param name="SRVProductDetailRepository"></param>
        /// <param name="EmployeeRepository"></param>
        /// <param name="ConcernID"></param>
        /// <param name="IMEINO"></param>
        /// <returns></returns>
        public static AdvanceSearchModel SRVisitAdvanceSearchByIMEI(this IBaseRepository<POrder> POrderRepository, IBaseRepository<POrderDetail> POrderDetailRepository, IBaseRepository<POProductDetail> POProductDetailRepository,
            IBaseRepository<Product> productRepository, IBaseRepository<Supplier> SupplierRepository, IBaseRepository<SRVisit> SRVisitRepository,
            IBaseRepository<SRVisitDetail> SRVisitDetailRepository, IBaseRepository<SRVProductDetail> SRVProductDetailRepository,
            IBaseRepository<Employee> EmployeeRepository,
            int ConcernID, string IMEINO)
        {
            IMEINO = IMEINO.Trim();
            AdvanceSearchModel advanceSearchModel = new AdvanceSearchModel();
            SRVProductDetail ObjSRVProductDetail = null;
            SRVisitDetail objSRVisitDetail = null;
            var Products = productRepository.All;
            var POrderDetails = POrderDetailRepository.All;
            var POProductDetails = POProductDetailRepository.All;
            var POProductDetail = POProductDetailRepository.All.Where(i => i.IMENO.Equals(IMEINO));
            var SRVisits = SRVisitRepository.All;
            var SRVisitDetails = SRVisitDetailRepository.All;
            var SRVProductDetails = SRVProductDetailRepository.All;

            if (POProductDetail != null)
            {
                var POrderDetail = POrderDetailRepository.All.Where(i => (POProductDetail.Select(j => j.POrderDetailID).Contains(i.POrderDetailID)) && (POProductDetail.Select(j => j.ProductID).Contains(i.ProductID)) && (POProductDetail.Select(j => j.ColorID).Contains(i.ColorID)));
                var POrder = POrderRepository.All.FirstOrDefault(i => i.ConcernID == ConcernID && (POrderDetail.Select(j => j.POrderID).Contains(i.POrderID)));
                if (POrder != null)
                {
                    var concernPOrderDetail = POrderDetails.Where(i => i.POrderID == POrder.POrderID);
                    var ConcernPOProductDetail = POProductDetails.Where(i => (concernPOrderDetail.Select(j => j.POrderDetailID).Contains(i.POrderDetailID)));

                    var POResult = (from POD in concernPOrderDetail
                                    join POPD in ConcernPOProductDetail on POD.POrderDetailID equals POPD.POrderDetailID
                                    join P in Products on POD.ProductID equals P.ProductID
                                    select new AdvancePODetail
                                    {
                                        ProductCode = P.Code,
                                        ProductName = P.ProductName,
                                        PurchaseRate = POD.UnitPrice,
                                        Quantity = 1,
                                        IMEI = POPD.IMENO
                                    }).ToList();
                    var SelectedIMEI = POResult.FirstOrDefault(i => i.IMEI.Equals(IMEINO));
                    POResult.Remove(SelectedIMEI);
                    POResult.Insert(0, SelectedIMEI);
                    advanceSearchModel.AdvancePODetails.AddRange(POResult);
                    advanceSearchModel.ChallanNo = POrder.ChallanNo;
                    advanceSearchModel.PurchaseDate = POrder.OrderDate;
                    var Supplier = SupplierRepository.FindBy(i => i.SupplierID == POrder.SupplierID).FirstOrDefault();
                    advanceSearchModel.SupplierCode = Supplier.Code;
                    advanceSearchModel.SupplierName = Supplier.Name;
                }

            }

            ObjSRVProductDetail = SRVProductDetails.FirstOrDefault(i => i.IMENO.Equals(IMEINO) && (i.Status == (int)EnumStockStatus.Sold || i.Status == (int)EnumStockStatus.Stock));

            if (ObjSRVProductDetail != null)
            {
                objSRVisitDetail = SRVisitDetails.FirstOrDefault(i => i.SRVisitDID == ObjSRVProductDetail.SRVisitDID);
                if (objSRVisitDetail != null)
                {
                    var ObjSRVisit = SRVisits.FirstOrDefault(i => i.SRVisitID == objSRVisitDetail.SRVisitID && i.Status == 1);
                    if (ObjSRVisit != null)
                    {
                        var SRVisitDetailList = SRVisitDetails.Where(i => i.SRVisitID == ObjSRVisit.SRVisitID);
                        var SRVProductDetailList = SRVProductDetails.Where(i => SRVisitDetailList.Select(j => j.SRVisitDID).Contains(i.SRVisitDID) && i.Status != (int)EnumStockStatus.Return);
                        var Result = (from SRVPD in SRVProductDetailList
                                      join P in Products on SRVPD.ProductID equals P.ProductID
                                      select new AdvanceSOrderDetail
                                      {
                                          ProductCode = P.Code,
                                          ProductName = P.ProductName,
                                          Quantity = 1,
                                          IMEI = SRVPD.IMENO,
                                          Status = SRVPD.Status
                                      }).ToList();

                        var SearchedIMEI = Result.FirstOrDefault(i => i.IMEI.Equals(IMEINO));
                        Result.Remove(SearchedIMEI);
                        Result.Insert(0, SearchedIMEI);

                        advanceSearchModel.AdvanceSOrderDetails.AddRange(Result);
                        advanceSearchModel.SalesDate = ObjSRVisit.VisitDate;
                        advanceSearchModel.InvoiceNo = ObjSRVisit.ChallanNo;
                        var Employee = EmployeeRepository.FindBy(i => i.EmployeeID == ObjSRVisit.EmployeeID).FirstOrDefault();
                        advanceSearchModel.CustomerCode = Employee.Code;
                        advanceSearchModel.CustomerName = Employee.Name;
                    }
                }
            }

            return advanceSearchModel;
        }


        public static POProductDetail GetDamageReturnPOPDetail(this IBaseRepository<POrder> POrderRepository, IBaseRepository<POrderDetail> POrderDetailRepository,
            IBaseRepository<POProductDetail> POProductDetailRepository, string DamageIMEI, int ProductID, int ColorID)
        {
            var POPD = from po in POrderRepository.All
                       join pod in POrderDetailRepository.All on po.POrderID equals pod.POrderID
                       join popd in POProductDetailRepository.All on pod.POrderDetailID equals popd.POrderDetailID
                       where (po.Status == (int)EnumPurchaseType.DamageReturn && popd.IMENO.Equals(DamageIMEI.Trim()) && popd.IsDamageReplaced != 1 && popd.ProductID == ProductID && popd.ColorID == ColorID)
                       select popd;

            return POPD.OrderByDescending(i => i.POrderDetailID).FirstOrDefault();
        }

        public static IEnumerable<ProductWisePurchaseModel> GetDamagePOReport(
                            this IBaseRepository<POrder> purchaseOrderRepository,
                            IBaseRepository<POrderDetail> pOrderDetailRepository, IBaseRepository<Product> productRepository,
                            IBaseRepository<POProductDetail> poProductRepository, IBaseRepository<Color> colorRepository,
                            DateTime fromDate, DateTime toDate, int SupplierID)
        {
            IQueryable<POrder> Porders = null;
            if (SupplierID != 0)
                Porders = purchaseOrderRepository.All.Where(i => i.SupplierID == SupplierID);
            else
                Porders = purchaseOrderRepository.All;

            var oPurchaseDetailData = (from PO in Porders
                                       join POD in pOrderDetailRepository.All on PO.POrderID equals POD.POrderID
                                       join POP in poProductRepository.All on POD.POrderDetailID equals POP.POrderDetailID
                                       join DPOP in poProductRepository.All on POP.DamagePOPDID equals DPOP.POPDID
                                       join P in productRepository.All on POD.ProductID equals P.ProductID
                                       from C in colorRepository.All
                                       where (PO.OrderDate >= fromDate && PO.OrderDate <= toDate && PO.Status == (int)EnumPurchaseType.Purchase && PO.IsDamageOrder == 1)
                                       select new ProductWisePurchaseModel
                                       {
                                           POrderID = PO.POrderID,
                                           ChallanNo = PO.ChallanNo,
                                           Date = PO.OrderDate,
                                           GrandTotal = PO.GrandTotal,
                                           NetDiscount = PO.NetDiscount,
                                           FlatDiscount = PO.TDiscount,
                                           NetTotal = PO.TotalAmt,
                                           RecAmt = PO.RecAmt,
                                           PaymentDue = PO.PaymentDue,
                                           ProductID = P.ProductID,
                                           ProductName = P.ProductName,
                                           PurchaseRate = POD.UnitPrice,
                                           TotalAmount = POD.TAmount,
                                           PPDISAmt = POD.PPDISAmt + POD.ExtraPPDISAmt,
                                           CategoryName = P.Category.Description,
                                           IMENO = POP.IMENO,
                                           DamageIMEI = DPOP.IMENO,
                                           ColorName = C.Name,
                                           PPOffer = POD.PPOffer,
                                           Quantity = 1
                                       }).OrderByDescending(x => x.Date).ThenByDescending(i => i.ChallanNo).ToList();
            return oPurchaseDetailData;
        }


        public static List<ProductWisePurchaseModel> GetDamageReturnProductDetails(this IBaseRepository<POrder> POrderRepository, IBaseRepository<POrderDetail> POrderDetailRepository,
       IBaseRepository<POProductDetail> POProductDetailRepository,
            IBaseRepository<Product> ProductRepository, IBaseRepository<Company> CompanyRepository, IBaseRepository<Category> CategoryRepository,
            IBaseRepository<Color> ColorRepository,
            int ProductID, int ColorID)
        {
            IQueryable<POProductDetail> POPDetails = null;
            if (ProductID != 0 && ColorID != 0)
                POPDetails = POProductDetailRepository.All.Where(i => i.ProductID == ProductID && i.ColorID == ColorID);
            else
                POPDetails = POProductDetailRepository.All;

            var POPD = (from po in POrderRepository.All
                        join pod in POrderDetailRepository.All on po.POrderID equals pod.POrderID
                        join popd in POPDetails on pod.POrderDetailID equals popd.POrderDetailID
                        join p in ProductRepository.All on popd.ProductID equals p.ProductID
                        join com in CompanyRepository.All on p.CompanyID equals com.CompanyID
                        join cat in CategoryRepository.All on p.CategoryID equals cat.CategoryID
                        join col in ColorRepository.All on popd.ColorID equals col.ColorID
                        where (po.Status == (int)EnumPurchaseType.DamageReturn && popd.IsDamageReplaced != 1)
                        select new ProductWisePurchaseModel
                        {
                            ProductName = p.ProductName,
                            ProductCode = p.Code,
                            IMENO = popd.IMENO,
                            CategoryName = cat.Description,
                            CompanyName = com.Name,
                            ColorName = col.Name,
                            SRate = pod.SalesRate,
                            ColorID = pod.ColorID,
                            MRP = pod.MRPRate
                        });

            return POPD.ToList();
        }


        /// <summary>
        /// Get all damage IMEIs which were sent to Company for replacement
        /// and which damage IMEIs were replaced by new IMEIs
        /// </summary>
        public static List<ProductWisePurchaseModel> DamageReturnProductDetailsReport(this IBaseRepository<POrder> POrderRepository, IBaseRepository<POrderDetail> POrderDetailRepository,
                                    IBaseRepository<POProductDetail> POProductDetailRepository,
                                    IBaseRepository<Product> ProductRepository, IBaseRepository<Company> CompanyRepository, IBaseRepository<Category> CategoryRepository,
                                    IBaseRepository<Color> ColorRepository,
                                    int SupplierID, DateTime fromDate, DateTime toDate)
        {
            IQueryable<POrder> POrders = null;
            if (SupplierID != 0)
                POrders = POrderRepository.All.Where(i => i.SupplierID == SupplierID && i.OrderDate >= fromDate && i.OrderDate <= toDate);
            else
                POrders = POrderRepository.All.Where(i => i.OrderDate >= fromDate && i.OrderDate <= toDate);


            var POPD = (from po in POrders
                        join pod in POrderDetailRepository.All on po.POrderID equals pod.POrderID
                        join popd in POProductDetailRepository.All on pod.POrderDetailID equals popd.POrderDetailID
                        join p in ProductRepository.All on popd.ProductID equals p.ProductID
                        join com in CompanyRepository.All on p.CompanyID equals com.CompanyID
                        join cat in CategoryRepository.All on p.CategoryID equals cat.CategoryID
                        join col in ColorRepository.All on popd.ColorID equals col.ColorID
                        join newPOP in POProductDetailRepository.All on popd.POPDID equals newPOP.DamagePOPDID into lj
                        from newPOP in lj.DefaultIfEmpty()
                        where (po.Status == (int)EnumPurchaseType.DamageReturn)
                        select new ProductWisePurchaseModel
                        {
                            Date = po.OrderDate,
                            ChallanNo = po.ChallanNo,
                            POrderID = po.POrderID,
                            ProductName = p.ProductName,
                            ProductCode = p.Code,
                            DamageIMEI = popd.IMENO,
                            CategoryName = cat.Description,
                            CompanyName = com.Name,
                            ColorName = col.Name,
                            SRate = pod.SalesRate,
                            ColorID = pod.ColorID,
                            PurchaseRate = pod.MRPRate,
                            POPDID = popd.POPDID,
                            IsDamageReplaced = popd.IsDamageReplaced,
                            Quantity = 1,
                            IMENO = (newPOP == null ? "Pending" : newPOP.IMENO),
                            NetTotal = po.TotalAmt
                        });



            return POPD.ToList();
        }



        /// <summary>
        /// Date: 08-01-2019
        /// Author: aminul
        /// Admin Purchase report of  all concern
        /// </summary>
        public static IQueryable<ProductWisePurchaseModel> AdminPurchaseReport(this IBaseRepository<POrder> POrderRepository,
                        IBaseRepository<Supplier> SupplierRepository, IBaseRepository<SisterConcern> SisterConcernRepository,
                        int ConcernID, DateTime fromDate, DateTime toDate, EnumPurchaseType PurchaseType)
        {
            IQueryable<Supplier> Suppliers = null;
            if (ConcernID != 0)
                Suppliers = SupplierRepository.GetAll().Where(i => i.ConcernID == ConcernID);
            else
                Suppliers = SupplierRepository.GetAll();

            var Result = from po in POrderRepository.GetAll()
                         join s in Suppliers on po.SupplierID equals s.SupplierID
                         join sis in SisterConcernRepository.GetAll() on po.ConcernID equals sis.ConcernID
                         where po.Status == (int)PurchaseType && (po.OrderDate >= fromDate && po.OrderDate <= toDate)
                         select new ProductWisePurchaseModel
                         {
                             ConcenName = sis.Name,
                             SupplierCode = s.Code,
                             SupplierName = s.Name,
                             ChallanNo = po.ChallanNo,
                             Date = po.OrderDate,
                             GrandTotal = po.GrandTotal,
                             NetDiscount = po.NetDiscount,
                             TotalAmount = po.TotalAmt,
                             RecAmt = po.RecAmt,
                             PaymentDue = po.PaymentDue,
                             LaborCost = po.LaborCost
                         };
            return Result.OrderByDescending(i => i.Date);
        }


        public static List<LedgerAccountReportModel> SupplierLedger(this IBaseRepository<POrder> POrderRepository, IBaseRepository<POrderDetail> POrderDetailRepository,
                             IBaseRepository<Product> ProductRepository, IBaseRepository<Company> CompanyRepository, IBaseRepository<Category> CategoryRepository,
                             IBaseRepository<Supplier> SupplierRepository, IBaseRepository<ApplicationUser> UserRepository,
                             IBaseRepository<BankTransaction> BankTransactionRepository, IBaseRepository<CashCollection> CashCollectionRepository, IBaseRepository<Bank> BankRepository,
                             int SupplierID, DateTime fromDate, DateTime toDate)
        {
            List<LedgerAccountReportModel> ledgers = new List<LedgerAccountReportModel>();
            List<LedgerAccountReportModel> FinalLedgers = new List<LedgerAccountReportModel>();

            var Supplier = SupplierRepository.All.FirstOrDefault(i => i.SupplierID == SupplierID);

            #region Purchase
            var Purchases = from po in POrderRepository.All
                            join pod in POrderDetailRepository.All on po.POrderID equals pod.POrderID
                            join p in ProductRepository.All on pod.ProductID equals p.ProductID
                            join u in UserRepository.All on po.CreatedBy equals u.Id into lj
                            from u in lj.DefaultIfEmpty()
                            where po.Status == (int)EnumPurchaseType.Purchase && po.SupplierID == SupplierID
                            select new
                            {
                                po.TotalAmt,
                                po.ChallanNo,
                                po.OrderDate,
                                po.RecAmt,
                                CreditAdj = po.AdjAmount + po.NetDiscount,
                                Credit = po.RecAmt,
                                CashCollectionAmt = po.RecAmt,
                                Debit = po.TotalAmt,
                                GrandTotal = po.GrandTotal,
                                pod.UnitPrice,
                                pod.TAmount,
                                pod.Quantity,
                                ProductName = p.ProductName + " " + pod.Quantity.ToString() + " " + p.ProUnitTypeID.ToString() + " " + pod.UnitPrice.ToString() + " " + pod.TAmount.ToString(),
                                EnteredBy = u == null ? string.Empty : u.UserName,
                                Remarks = string.Empty,
                            };

            var VmPurchases = (from cs in Purchases
                               group cs by new { cs.Debit, cs.Credit, cs.CreditAdj, cs.GrandTotal, cs.CashCollectionAmt, cs.OrderDate, cs.ChallanNo, cs.EnteredBy } into g
                               select new LedgerAccountReportModel
                               {
                                   VoucherType = "Purchase",
                                   InvoiceNo = g.Key.ChallanNo,
                                   Date = g.Key.OrderDate,
                                   EnteredBy = "Entered By: " + g.Key.EnteredBy,
                                   ProductList = g.Select(i => i.ProductName).ToList(),
                                   Debit = g.Key.Debit,
                                   Credit = g.Key.Credit,
                                   CreditAdj = g.Key.CreditAdj,
                                   GrandTotal = g.Key.GrandTotal,
                                   CashCollectionAmt = g.Key.CashCollectionAmt,
                                   Quantity = g.Sum(i => i.Quantity),
                                   Balance = 0,
                                   Remarks = g.Select(i => i.Remarks).FirstOrDefault()
                               }).ToList();

            ledgers.AddRange(VmPurchases);
            #endregion

            #region Purchases Return
            var PurchasesReturns = from po in POrderRepository.All
                                   join pod in POrderDetailRepository.All on po.POrderID equals pod.POrderID
                                   join p in ProductRepository.All on pod.ProductID equals p.ProductID
                                   join u in UserRepository.All on po.CreatedBy equals u.Id into lj
                                   from u in lj.DefaultIfEmpty()
                                   where po.Status == (int)EnumPurchaseType.ProductReturn && po.SupplierID == SupplierID
                                   select new
                                   {
                                       po.TotalAmt,
                                       po.ChallanNo,
                                       po.OrderDate,
                                       po.RecAmt,
                                       CreditAdj = po.AdjAmount + po.NetDiscount,
                                       Credit = po.TotalAmt,
                                       Return = po.TotalAmt - po.RecAmt,
                                       CashCollectionAmt = po.RecAmt,
                                       Debit = po.RecAmt,
                                       GrandTotal = po.GrandTotal,
                                       pod.UnitPrice,
                                       pod.TAmount,
                                       pod.Quantity,
                                       ProductName = p.ProductName + " " + pod.Quantity.ToString() + " " + p.ProUnitTypeID.ToString() + " " + pod.UnitPrice.ToString() + " " + pod.TAmount.ToString(),
                                       EnteredBy = u == null ? string.Empty : u.UserName,
                                       Remarks = string.Empty,
                                   };

            var VmPurchasesReturns = (from cs in PurchasesReturns
                                      group cs by new { cs.Debit, cs.Credit, cs.Return, cs.OrderDate, cs.ChallanNo, cs.EnteredBy } into g
                                      select new LedgerAccountReportModel
                                      {
                                          VoucherType = "PO Return",
                                          InvoiceNo = g.Key.ChallanNo,
                                          Date = g.Key.OrderDate,
                                          EnteredBy = "Entered By: " + g.Key.EnteredBy,
                                          ProductList = g.Select(i => i.ProductName).ToList(),
                                          Debit = g.Key.Debit,
                                          Credit = g.Key.Credit,
                                          SalesReturn = g.Key.Return,
                                          Quantity = g.Sum(i => i.Quantity),
                                          Balance = 0,
                                          Remarks = g.Select(i => i.Remarks).FirstOrDefault()
                                      }).ToList();

            ledgers.AddRange(VmPurchasesReturns);
            #endregion

            #region Cash Delivery
            var CashDelivery = from cc in CashCollectionRepository.All
                               join u in UserRepository.All on cc.CreatedBy equals u.Id into lj
                               from u in lj.DefaultIfEmpty()
                               where cc.SupplierID == SupplierID
                               select new LedgerAccountReportModel
                               {
                                   Date = (DateTime)cc.EntryDate,
                                   Debit = 0m,
                                   VoucherType = "Cash Delivery",
                                   Credit = cc.Amount + cc.AdjustAmt,
                                   CashCollectionAmt = cc.Amount,
                                   CreditAdj = cc.AdjustAmt,
                                   InvoiceNo = cc.ReceiptNo,
                                   EnteredBy = "Entered By: " + u.UserName,
                                   Remarks = cc.Remarks
                               };
            ledgers.AddRange(CashDelivery);
            #endregion

            #region Bank Transaction
            var bankTrans = from bt in BankTransactionRepository.All
                            join b in BankRepository.All on bt.BankID equals b.BankID
                            where bt.SupplierID == SupplierID
                            select new LedgerAccountReportModel
                            {
                                Date = (DateTime)bt.TranDate,
                                Debit = 0m,
                                VoucherType = "Bank",
                                Credit = bt.Amount,
                                CashCollectionAmt = bt.Amount,
                                CreditAdj = 0m,
                                InvoiceNo = bt.TransactionNo,
                                Particulars = b.AccountName + " " + b.AccountNo + " Chk. No: " + bt.ChecqueNo,
                                Remarks = bt.Remarks
                            };
            ledgers.AddRange(bankTrans);
            #endregion

            decimal balance = Supplier.OpeningDue;
            ledgers = ledgers.OrderBy(i => i.Date).ToList();
            foreach (var item in ledgers)
            {
                item.Balance = balance + (item.Debit - item.Credit);
                item.Particulars = string.IsNullOrEmpty(item.Particulars) ? string.Join(Environment.NewLine, item.ProductList) + Environment.NewLine + item.EnteredBy : item.Particulars;
                balance = item.Balance;
            }

            var oOpening = new LedgerAccountReportModel() { Date = new DateTime(2015, 1, 1), Particulars = "Opening Balance", Debit = Supplier.OpeningDue, Balance = 0, Credit = 0 };

            if (ledgers.Count > 0)
            {
                //ledgers.Insert(0, oOpening);
                var OpeningTrans = ledgers.Where(i => i.Date < fromDate).OrderByDescending(i => i.Date).FirstOrDefault();
                if (OpeningTrans != null)
                    FinalLedgers.Add(new LedgerAccountReportModel() { Date = OpeningTrans.Date, Particulars = "Opening Balance", Balance = OpeningTrans.Balance, Debit = 0m });
                else
                    FinalLedgers.Add(new LedgerAccountReportModel() { Date = fromDate, Particulars = "Opening Balance", Balance = Supplier.OpeningDue, Debit = 0m });

                ledgers = ledgers.Where(i => i.Date >= fromDate && i.Date <= toDate).OrderBy(i => i.Date).ToList();
                FinalLedgers.AddRange(ledgers);
            }
            else
            {
                FinalLedgers.Add(new LedgerAccountReportModel() { Date = fromDate, Particulars = "Opening Balance", Debit = Supplier.OpeningDue, Credit = 0m, Balance = Supplier.OpeningDue });
            }

            return FinalLedgers;
        }

        public static async Task<IEnumerable<Tuple<int, string, DateTime, string,
                string, string, EnumPurchaseType>>> GetAllRawStockDeductOrderAsync(this IBaseRepository<POrder> purchaseOrderRepository,
                IBaseRepository<Supplier> supplierRepository, DateTime fromDate, DateTime toDate)
        {
            IQueryable<Supplier> suppliers = supplierRepository.All;
            var POrders = purchaseOrderRepository.All;

            var items = await (from po in POrders
                               join sup in suppliers on po.SupplierID equals sup.SupplierID
                               where po.Status == (int)EnumPurchaseType.RawStockDeduct &&
                               (po.OrderDate >= fromDate && po.OrderDate <= toDate)
                               select new
                               {
                                   po.POrderID,
                                   po.ChallanNo,
                                   po.OrderDate,
                                   sup.Name,
                                   sup.OwnerName,
                                   sup.ContactNo,
                                   po.Status
                               }).OrderByDescending(s => s.POrderID).ToListAsync();

            return items.Select(x => new Tuple<int, string, DateTime, string, string, string, EnumPurchaseType>
                (
                    x.POrderID,
                    x.ChallanNo,
                    x.OrderDate,
                    x.Name,
                    x.OwnerName,
                    x.ContactNo,
                    (EnumPurchaseType)x.Status
                )).OrderByDescending(x => x.Item1).ToList();
        }


    }
}
