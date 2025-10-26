using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Data
{
    public static class StockExtensions
    {
        public static async Task<IEnumerable<Tuple<int, string, string, string, decimal, decimal, decimal,
            Tuple<string, int, int, decimal, decimal, decimal, decimal,
            Tuple<string>>>>> GetAllStockAsync(this IBaseRepository<Stock> stockRepository, IBaseRepository<Product> productRepository,
            IBaseRepository<Color> colorRepository, IBaseRepository<StockDetail> StockDetailRepository, IBaseRepository<Godown> GodownRepository
)
        {
            IQueryable<Product> products = productRepository.All;
            IQueryable<Color> colors = colorRepository.All;
            var StockDetails = StockDetailRepository.All;
            var Godowns = GodownRepository.All;

            var items = await stockRepository.All.Join(products,
                stk => stk.ProductID, prod => prod.ProductID, (stk, prod) => new { Stock = stk, Product = prod }).
                Join(colors, sp => sp.Stock.ColorID, c => c.ColorID,
                (sp, c) => new { Product = sp.Product, Stock = sp.Stock, Color = c }).

                Join(Godowns, sp => sp.Stock.GodownID, g => g.GodownID,
                (sp, g) => new { Product = sp.Product, Stock = sp.Stock, Godown = g, Color = sp.Color }).


                Select(x => new
                {
                    StockId = x.Stock.StockID,
                    StockCode = x.Stock.StockCode,
                    x.Product.ProductName,
                    CompanyName = x.Product.Company.Name,
                    Quantity = x.Product.ProductType == (int)EnumProductType.NoBarcode ? x.Stock.Quantity : StockDetails.Where(i => i.ProductID == x.Product.ProductID && i.ColorID == x.Color.ColorID && i.Status == (int)EnumStockStatus.Stock).Count(),
                    x.Stock.LPPrice,
                    x.Stock.MRPPrice,
                    ColorName = x.Color.Name,
                    x.Product.ProductID,
                    x.Color.ColorID,
                    SalesPrice = StockDetails.FirstOrDefault(i => i.ProductID == x.Product.ProductID && i.ColorID == x.Color.ColorID && i.Status == (int)EnumStockStatus.Stock) != null ? StockDetails.FirstOrDefault(i => i.ProductID == x.Product.ProductID && i.ColorID == x.Color.ColorID && i.Status == (int)EnumStockStatus.Stock).SRate : 0m,
                    CreditSalesPrice = StockDetails.FirstOrDefault(i => i.ProductID == x.Product.ProductID && i.ColorID == x.Color.ColorID && i.Status == (int)EnumStockStatus.Stock) != null ? StockDetails.FirstOrDefault(i => i.ProductID == x.Product.ProductID && i.ColorID == x.Color.ColorID && i.Status == (int)EnumStockStatus.Stock).CreditSRate : 0m,
                    CreditSalesPrice3 = StockDetails.FirstOrDefault(i => i.ProductID == x.Product.ProductID && i.ColorID == x.Color.ColorID && i.Status == (int)EnumStockStatus.Stock) != null ? StockDetails.FirstOrDefault(i => i.ProductID == x.Product.ProductID && i.ColorID == x.Color.ColorID && i.Status == (int)EnumStockStatus.Stock).CRSalesRate3Month : 0m,
                    CreditSalesPrice12 = StockDetails.FirstOrDefault(i => i.ProductID == x.Product.ProductID && i.ColorID == x.Color.ColorID && i.Status == (int)EnumStockStatus.Stock) != null ? StockDetails.FirstOrDefault(i => i.ProductID == x.Product.ProductID && i.ColorID == x.Color.ColorID && i.Status == (int)EnumStockStatus.Stock).CRSalesRate12Month : 0m,
                    GodownName = x.Godown.Name

                }).Where(x => x.Quantity > 0).ToListAsync();

            return items.Select(x => new Tuple<int, string, string, string, decimal, decimal, decimal, Tuple<string, int, int, decimal, decimal, decimal, decimal, Tuple<string>>>
                (
                    x.StockId,
                    x.StockCode,
                    x.ProductName,
                    x.CompanyName,
                    x.Quantity,
                    x.LPPrice,
                    x.MRPPrice,
                    new Tuple<string, int, int, decimal, decimal, decimal, decimal, Tuple<string>>
                        (
                        x.ColorName,
                        x.ProductID,
                        x.ColorID,
                        x.SalesPrice,
                        x.CreditSalesPrice,
                        x.CreditSalesPrice3,
                        x.CreditSalesPrice12,
                        new Tuple<string>
                        (x.GodownName)
                        )
                )).OrderByDescending(x => x.Item1).ToList();
        }

        public static async Task<IEnumerable<Tuple<int, string, string, string, string, string, string, Tuple<string>>>>
            GetAllStockDetailAsync(this IBaseRepository<Stock> stockRepository, IBaseRepository<StockDetail> stockDetailRepository, IBaseRepository<Product> productRepository, IBaseRepository<Color> colorRepository, IBaseRepository<Godown> GodownRepository)
        {
            IQueryable<Product> products = productRepository.All;
            IQueryable<Color> colors = colorRepository.All;
            IQueryable<StockDetail> stockDetail = stockDetailRepository.All;
            IQueryable<Godown> godownDetail = GodownRepository.All;

            var items = await stockDetailRepository.All.Join(products,
                stkDetail => stkDetail.ProductID, prod => prod.ProductID, (stkDetail, prod) => new { StockDetail = stkDetail, Product = prod }).
                Join(colors, sp => sp.StockDetail.ColorID, c => c.ColorID,
                (sp, c) => new { Product = sp.Product, StockDetail = sp.StockDetail, Color = c }).
                 Join(godownDetail, sp => sp.StockDetail.GodownID, g => g.GodownID,
                (spg, g) => new { Product = spg.Product, StockDetail = spg.StockDetail, Color = spg.Color, Godown = g }).
                Select(x => new
                {
                    StockDId = x.StockDetail.SDetailID,
                    StockCode = x.StockDetail.StockCode,
                    x.Product.ProductName,
                    CompanyName = x.Product.Company.Name,
                    x.StockDetail.IMENO,
                    x.StockDetail.Status,
                    ColorName = x.Color.Name,
                    GodownName = x.Godown.Name,
                }).ToListAsync();

            return items.Select(x => new Tuple<int, string, string, string, string, string, string, Tuple<string>>
                (
                    x.StockDId,
                    x.StockCode,
                    x.ProductName,
                    x.CompanyName,
                    x.IMENO,
                    x.Status.ToString(),
                    x.ColorName,
                    new Tuple<string>(x.GodownName)

                )).OrderByDescending(x => x.Item1).ToList();
        }


        public static IEnumerable<ProductWisePurchaseModel> GetforReport(this IBaseRepository<Stock> stockRepository, IBaseRepository<Product> productRepository,
                 IBaseRepository<Color> colorRepository, IBaseRepository<StockDetail> StockDetailRepository, IBaseRepository<Godown> GodownRepository,
                 IBaseRepository<ProductUnitType> ProductUnitTypeRepository, IBaseRepository<Size> SizeRepository,
                 string userName, int concernID, int reportType, int CompanyID, int CategoryID, int ProductID, int GodownID, EnumProductStockType ProductStockType)
        {
            var Products = productRepository.All;
            if (ProductStockType != 0)
                Products = Products.Where(i => i.ProductStockType == ProductStockType);
            if (CompanyID != 0)
                Products = Products.Where(i => i.CompanyID == CompanyID);
            if (CategoryID != 0)
                Products = Products.Where(i => i.CategoryID == CategoryID);
            if (ProductID != 0)
                Products = Products.Where(i => i.ProductID == ProductID);
            var StockDetails = StockDetailRepository.All;

            var Godowns = GodownRepository.All;
            if (GodownID != 0)
                Godowns = Godowns.Where(o => o.GodownID == GodownID);

            var oStockData = (from st in stockRepository.All
                              join pro in Products on st.ProductID equals pro.ProductID
                              join col in colorRepository.All on st.ColorID equals col.ColorID
                              join god in GodownRepository.All on st.GodownID equals god.GodownID
                              join pu in ProductUnitTypeRepository.All on (int)pro.ProUnitTypeID equals pu.ProUnitTypeID
                              join s in SizeRepository.All on pro.SizeID equals s.SizeID
                              select new
                              {
                                  StockID = st.StockID,
                                  pro.IDCode,
                                  pro.Code,
                                  productName = pro.ProductName,
                                  pro.ProductID,
                                  CompanyName = pro.Company.Name,
                                  CategoryName = pro.Category.Description,
                                  ColorName = col.Name,
                                  col.ColorID,
                                  GodownName = god.Name,
                                  SizeName = s.Description,
                                  Qty = pro.ProductType == (int)EnumProductType.NoBarcode ? st.Quantity : StockDetails.Where(i => i.ProductID == st.ProductID && i.ColorID == st.ColorID && i.Status == (int)EnumStockStatus.Stock).Count(),
                                  MRPRate = st.MRPPrice,
                                  StockDetails = StockDetails.FirstOrDefault(i => i.ProductID == st.ProductID && i.ColorID == st.Color.ColorID && i.Status == (int)EnumStockStatus.Stock),
                                  //SalesPrice = StockDetails.FirstOrDefault(i => i.ProductID == st.ProductID && i.ColorID == st.Color.ColorID && i.Status == (int)EnumStockStatus.Stock) != null ? StockDetails.FirstOrDefault(i => i.ProductID == st.ProductID && i.ColorID == st.ColorID && i.Status == (int)EnumStockStatus.Stock).SRate : 0m,
                                  //CreditSalesPrice = StockDetails.FirstOrDefault(i => i.ProductID == st.ProductID && i.ColorID == st.Color.ColorID && i.Status == (int)EnumStockStatus.Stock) != null ? StockDetails.FirstOrDefault(i => i.ProductID == st.ProductID && i.ColorID == st.ColorID && i.Status == (int)EnumStockStatus.Stock).CreditSRate : 0m
                                  pu.Description,
                                  pu.UnitName,
                                  ConvertValue = pro.BundleQty > 0 ? pro.BundleQty : pu.ConvertValue,
                                  st.TotalSFT
                              }).Where(st => st.Qty != 0).ToList();

            return (from x in oStockData
                    select new ProductWisePurchaseModel
                    {
                        StockID = x.StockID,
                        ProductCode = x.CategoryName.ToLower().Equals("tiles") ? x.IDCode : x.Code,
                        ProductName = x.productName,
                        ProductID = x.ProductID,
                        CompanyName = x.CompanyName,
                        CategoryName = x.CategoryName,
                        ColorID = x.ColorID,
                        ColorName = x.ColorName,
                        SizeName = x.SizeName,
                        Quantity = x.Qty,
                        ParentQuantity = (int)(Math.Truncate(x.Qty / x.ConvertValue)),
                        MRP = !x.CategoryName.ToLower().Equals("tiles") ? x.StockDetails != null ? x.StockDetails.PRate * x.ConvertValue : 0m : x.StockDetails != null ? x.StockDetails.SFTRate : 0m,

                        SRate = x.StockDetails != null ? x.StockDetails.SRate * x.ConvertValue : 0m,
                        TotalCreditSR6 = x.StockDetails != null ? x.StockDetails.CreditSRate * x.ConvertValue : 0m,
                        TotalCreditSR3 = x.StockDetails != null ? x.StockDetails.CRSalesRate3Month * x.ConvertValue : 0m,
                        TotalCreditSR12 = x.StockDetails != null ? x.StockDetails.CRSalesRate12Month * x.ConvertValue : 0m,
                        GodownName = x.GodownName,
                        ParentUnitName = x.Description,
                        ChildUnitName = x.UnitName,
                        ConvertValue = x.ConvertValue,
                        TotalSFT = x.TotalSFT,
                    });


        }

        public static IEnumerable<Tuple<string, string, decimal, decimal, decimal, decimal, DateTime>> GetPriceProtectionReport(this IBaseRepository<Stock> stockRepository, IBaseRepository<Product> productRepository,
        IBaseRepository<Color> colorRepository, IBaseRepository<Supplier> suppRepository, IBaseRepository<PriceProtection> priceprotectionRepository, string userName, int concernID, DateTime dFDate, DateTime dToDate)
        {
            var oStockData = (from pp in priceprotectionRepository.All
                              join pro in productRepository.All on pp.ProductID equals pro.ProductID
                              join col in colorRepository.All on pp.ColorID equals col.ColorID
                              join sup in suppRepository.All on pp.SupplierID equals sup.SupplierID
                              select new
                              {
                                  SuppName = sup.Name,
                                  productName = pro.ProductName,
                                  PrvPrice = pp.PrvPrice,
                                  ChangePrice = pp.ChangePrice,
                                  Qty = pp.PrvStockQty,
                                  FallAmt = ((pp.PrvPrice - pp.ChangePrice) * pp.PrvStockQty),
                                  CDate = pp.PChangeDate
                              }).ToList();

            return oStockData.Select(x => new Tuple<string, string, decimal, decimal, decimal, decimal, DateTime>
                                (
                                x.SuppName,
                                x.productName,
                                x.PrvPrice,
                                x.ChangePrice,
                                x.Qty,
                                x.FallAmt,
                                x.CDate
                                ));


        }

        public static IEnumerable<Tuple<int, string, string>> GetStockDetailsByID(this IBaseRepository<Stock> stockRepository, IBaseRepository<StockDetail> stockDetailRepository,
                int stockID)
        {
            var Stocks = stockRepository.All.Where(i => i.StockID == stockID);
            var StockDetails = stockDetailRepository.All.Where(i => i.StockID == stockID && i.Status == 1);
            var oStockData = (from st in Stocks
                              join std in StockDetails on st.StockID equals std.StockID
                              select new
                              {
                                  StockID = st.StockID,
                                  SDetailID = std.SDetailID,
                                  StockCode = std.StockCode,
                                  IMEINO = std.IMENO,
                                  Status = std.Status
                              }).ToList();

            return oStockData.Select(x => new Tuple<int, string, string>
                                (
                                x.SDetailID,
                                x.StockCode,
                                x.IMEINO
                                ));
        }

        public static bool CheckStockIMEIForSRVisit(this IBaseRepository<Stock> stockRepository, IBaseRepository<StockDetail> stockDetailRepository, int ProductID, int ColorID, string IMEI)
        {
            var Stock = stockRepository.All.FirstOrDefault(i => i.ProductID == ProductID && i.ColorID == ColorID);
            if (Stock != null)
            {
                var StockDetails = stockDetailRepository.All;

                if (StockDetails.Any(i => i.ProductID == Stock.ProductID && i.ColorID == Stock.ColorID && i.IMENO.Equals(IMEI.Trim()) && i.Status == (int)EnumStockStatus.Stock))
                    return true;
                else
                    return false;
            }
            return false;
        }
        public static string GetStockProductsHistory(this IBaseRepository<Stock> stockRepository, IBaseRepository<StockDetail> stockDetailRepository,
    IBaseRepository<SRVisit> SRVisitRepository, IBaseRepository<SRVisitDetail> SRVisitDetailRepository, IBaseRepository<SRVProductDetail> SRVProductDetailRepository,
    IBaseRepository<Employee> EmployeeRepository,
    int StockID)
        {
            string History = string.Empty;
            var StockDetails = stockDetailRepository.All.Where(i => i.StockID == StockID && i.Status == (int)EnumStockStatus.Stock);

            var SRStockDetails = (from sv in SRVisitRepository.All
                                  join svd in SRVisitDetailRepository.All on sv.SRVisitID equals svd.SRVisitID
                                  join spd in SRVProductDetailRepository.All on svd.SRVisitDID equals spd.SRVisitDID
                                  join sd in StockDetails on spd.SDetailID equals sd.SDetailID
                                  join emp in EmployeeRepository.All on sv.EmployeeID equals emp.EmployeeID
                                  where spd.Status == (int)EnumSRVProductDetailsStatus.Stock && sv.Status == (int)EnumSRVisitType.Live
                                  select new
                                  {
                                      sv.EmployeeID,

                                      emp.Name,
                                      spd.SDetailID
                                  }).OrderBy(i => i.EmployeeID).ToList();

            var Result = (from sr in SRStockDetails
                          group sr by new { sr.EmployeeID, sr.Name } into g
                          select new
                          {
                              EmployeeID = g.Key.EmployeeID,
                              Name = g.Key.Name,
                              Quantity = g.Count()
                          }).OrderBy(i => i.EmployeeID).ToList();
            if (SRStockDetails.Count() > 0)
                History = "Stock= " + (StockDetails.Count() - SRStockDetails.Count()) + Environment.NewLine;
            else
                History = "Stock= " + (StockDetails.Count() - SRStockDetails.Count());

            int counter = 0;
            foreach (var item in Result)
            {
                counter++;
                if (counter != Result.Count())
                    History = History + item.Name + "= " + item.Quantity + Environment.NewLine;
                else
                    History = History + item.Name + "= " + item.Quantity;

            }
            return History;

        }


        public static List<StockLedger> GetStockLedger(
                     this IBaseRepository<Stock> stockRepository,
                     IBaseRepository<StockDetail> stockDetailRepository,
                     IBaseRepository<POrder> POrderRepository,
                     IBaseRepository<POrderDetail> POrderDetailRepository,
                     IBaseRepository<Product> productRepository,
                     IBaseRepository<Category> categoryRepository,
                     IBaseRepository<Company> companyRepository,
                     IBaseRepository<Color> colorRepository,
                     IBaseRepository<SOrder> SOrderRepository,
                     IBaseRepository<SOrderDetail> SOrderDetailRepository,
                     IBaseRepository<CreditSale> CreditSaleRepository,
                     IBaseRepository<CreditSaleDetails> CreditSaleDetails,
                     IBaseRepository<ROrder> ROrderRepository,
                     IBaseRepository<ROrderDetail> ROrderDetailRepository,
                     IBaseRepository<Production> productionRepository,
                     IBaseRepository<ProductionDetail> productionDetailRepository,
                     int reportType,
                     int CompanyID,
                     int CategoryID,
                     int ProductID,
                     DateTime fromDate,
                     DateTime toDate
            )
        {


            var _Porders = POrderRepository.All;
            var _POrderDetails = POrderDetailRepository.All;
            var _Stocks = stockRepository.All;
            var _StockDetails = stockDetailRepository.All;
            var _Products = productRepository.All;
            var _Categorys = categoryRepository.All;
            var _Companys = companyRepository.All;
            var _Colors = colorRepository.All;
            var _SOrders = SOrderRepository.All;
            var _SOrderDetails = SOrderDetailRepository.All;
            var _CreditSales = CreditSaleRepository.All;
            var _CreditSalesDetails = CreditSaleDetails.All;
            var _ROrders = ROrderRepository.GetAll();
            var _ROrderDetails = ROrderDetailRepository.All;
            var _Productions = productionRepository.GetAll();
            var _ProductionDetails = productionDetailRepository.All;



            var pquery = (from POD in _POrderDetails
                          join PO in _Porders on POD.POrderID equals PO.POrderID
                          join P in _Products on POD.ProductID equals P.ProductID
                          join CAT in _Categorys on P.CategoryID equals CAT.CategoryID
                          join COM in _Companys on P.CompanyID equals COM.CompanyID
                          //join MOD in db.Models on P.ModelID equals MOD.ModelID
                          join CLR in _Colors on POD.ColorID equals CLR.ColorID
                          where
                          PO.Status == (int)EnumPurchaseType.Purchase
                          select new StockLedger
                          {
                              Date = PO.OrderDate,
                              Code = P.Code,
                              CategoryCode = CAT.Code,
                              ProductID = P.ProductID,
                              CategoryID = P.CategoryID,
                              CompanyID = P.CompanyID,
                              ModelID = 1,
                              ColorID = CLR.ColorID,
                              ProductName = P.ProductName,
                              CompanyName = COM.Name,
                              CategoryName = CAT.Description,
                              ModelName = "",
                              ColorName = CLR.Name,
                              PurchaseQuantity = POD.Quantity,
                          }).OrderBy(x => x.Date);

            var Purchase_return = ((from POD in _POrderDetails
                                    join PO in _Porders on POD.POrderID equals PO.POrderID
                                    join P in _Products on POD.ProductID equals P.ProductID
                                    join CAT in _Categorys on P.CategoryID equals CAT.CategoryID
                                    join COM in _Companys on P.CompanyID equals COM.CompanyID
                                    //join MOD in db.Models on P.ModelID equals MOD.ModelID
                                    join CLR in _Colors on POD.ColorID equals CLR.ColorID
                                    where
                                    PO.Status == (int)EnumPurchaseType.ProductReturn
                                    select new StockLedger
                                    {
                                        Date = PO.OrderDate,
                                        Code = P.Code,
                                        CategoryCode = CAT.Code,
                                        ProductID = P.ProductID,
                                        CategoryID = P.CategoryID,
                                        CompanyID = P.CompanyID,
                                        ModelID = 1,
                                        ColorID = CLR.ColorID,
                                        ProductName = P.ProductName,
                                        CompanyName = COM.Name,
                                        CategoryName = CAT.Description,
                                        ModelName = "",
                                        ColorName = CLR.Name,
                                        PurchaseQuantity = 0,
                                        PurchaseReturnQuantity = POD.Quantity,
                                        SalesQuantity = 0,
                                        SalesReturnQuantity = 0,
                                    }).OrderBy(x => x.Date));

            var RawProductDeduct = ((from POD in _POrderDetails
                                     join PO in _Porders on POD.POrderID equals PO.POrderID
                                     join P in _Products on POD.ProductID equals P.ProductID
                                     join CAT in _Categorys on P.CategoryID equals CAT.CategoryID
                                     join COM in _Companys on P.CompanyID equals COM.CompanyID
                                     //join MOD in db.Models on P.ModelID equals MOD.ModelID
                                     join CLR in _Colors on POD.ColorID equals CLR.ColorID
                                     where
                                     PO.Status == (int)EnumPurchaseType.RawStockDeduct
                                     select new StockLedger
                                     {
                                         Date = PO.OrderDate,
                                         Code = P.Code,
                                         CategoryCode = CAT.Code,
                                         ProductID = P.ProductID,
                                         CategoryID = P.CategoryID,
                                         CompanyID = P.CompanyID,
                                         ModelID = 1,
                                         ColorID = CLR.ColorID,
                                         ProductName = P.ProductName,
                                         CompanyName = COM.Name,
                                         CategoryName = CAT.Description,
                                         ModelName = "",
                                         ColorName = CLR.Name,
                                         PurchaseQuantity = 0,
                                         RawDeductQuantity = POD.Quantity,
                                         SalesQuantity = 0,
                                         SalesReturnQuantity = 0,
                                     }).OrderBy(x => x.Date));

            var FinGoodsProduction = (from POD in _ProductionDetails
                                      join PO in _Productions on POD.ProductionID equals PO.ProductionID
                                      join P in _Products on POD.ProductID equals P.ProductID
                                      join CAT in _Categorys on P.CategoryID equals CAT.CategoryID
                                      join COM in _Companys on P.CompanyID equals COM.CompanyID
                                      where
                                      PO.Status == EnumProductionStatus.Production
                                      select new StockLedger
                                      {
                                          Date = PO.Date,
                                          Code = P.Code,
                                          CategoryCode = CAT.Code,
                                          ProductID = P.ProductID,
                                          CategoryID = P.CategoryID,
                                          CompanyID = P.CompanyID,
                                          ModelID = 1,
                                          ProductName = P.ProductName,
                                          CompanyName = COM.Name,
                                          CategoryName = CAT.Description,
                                          ModelName = "",
                                          ProductionQuantity = POD.Quantity,
                                      }).OrderBy(x => x.Date);


            var squery = ((from SOD in _SOrderDetails
                           join SO in _SOrders on SOD.SOrderID equals SO.SOrderID
                           join P in _Products on SOD.ProductID equals P.ProductID
                           join CAT in _Categorys on P.CategoryID equals CAT.CategoryID
                           join COM in _Companys on P.CompanyID equals COM.CompanyID
                           // join MOD in db.Models on P.ModelID equals MOD.ModelID
                           join STD in _StockDetails on SOD.SDetailID equals STD.SDetailID
                           join CLR in _Colors on STD.ColorID equals CLR.ColorID
                           where
                          SO.Status == (int)EnumSalesType.Sales
                           select new StockLedger
                           {
                               Date = SO.InvoiceDate,
                               Code = P.Code,
                               CategoryCode = CAT.Code,
                               ProductID = P.ProductID,
                               CategoryID = P.CategoryID,
                               CompanyID = P.CompanyID,
                               ModelID = 1,
                               ColorID = CLR.ColorID,
                               ProductName = P.ProductName,
                               CompanyName = COM.Name,
                               CategoryName = CAT.Description,
                               ModelName = "",
                               ColorName = CLR.Name,
                               SalesQuantity = SOD.Quantity,
                               PurchaseQuantity = 0,
                               PurchaseReturnQuantity = 0,
                               SalesReturnQuantity = 0,
                           }).OrderBy(x => x.Date));



            var squery_credit = ((from SOD in _CreditSalesDetails
                                  join SO in _CreditSales on SOD.CreditSalesID equals SO.CreditSalesID
                                  join P in _Products on SOD.ProductID equals P.ProductID
                                  join CAT in _Categorys on P.CategoryID equals CAT.CategoryID
                                  join COM in _Companys on P.CompanyID equals COM.CompanyID
                                  //  join MOD in  on P.ModelID equals MOD.ModelID
                                  join STD in _StockDetails on SOD.StockDetailID equals STD.SDetailID
                                  join CLR in _Colors on STD.ColorID equals CLR.ColorID
                                  where SO.IsStatus == EnumSalesType.Sales

                                  select new StockLedger
                                  {
                                      Date = SO.SalesDate,
                                      Code = P.Code,
                                      CategoryCode = CAT.Code,
                                      ProductID = P.ProductID,
                                      CategoryID = P.CategoryID,
                                      CompanyID = P.CompanyID,
                                      ModelID = 1,
                                      ColorID = CLR.ColorID,
                                      ProductName = P.ProductName,
                                      CompanyName = COM.Name,
                                      CategoryName = CAT.Description,
                                      ModelName = "",
                                      ColorName = CLR.Name,
                                      SalesQuantity = SOD.Quantity,
                                      PurchaseQuantity = 0,
                                      PurchaseReturnQuantity = 0,
                                      SalesReturnQuantity = 0,
                                  }).OrderBy(x => x.Date));

            var Sales_return = ((from SO in _ROrders
                                 join SOD in _ROrderDetails on SO.ROrderID equals SOD.ROrderID
                                 join P in _Products on SOD.ProductID equals P.ProductID
                                 join CAT in _Categorys on P.CategoryID equals CAT.CategoryID
                                 join COM in _Companys on P.CompanyID equals COM.CompanyID
                                 // join MOD in db.Models on P.ModelID equals MOD.ModelID
                                 join STD in _StockDetails on SOD.StockDetailID equals STD.SDetailID
                                 join CLR in _Colors on STD.ColorID equals CLR.ColorID
                                 //SO.Status == (int)EnumSalesType.ProductReturn
                                 select new StockLedger
                                 {
                                     Date = SO.ReturnDate,
                                     Code = P.Code,
                                     CategoryCode = CAT.Code,
                                     ProductID = P.ProductID,
                                     CategoryID = P.CategoryID,
                                     CompanyID = P.CompanyID,
                                     ModelID = 1,
                                     ColorID = CLR.ColorID,
                                     ProductName = P.ProductName,
                                     CompanyName = COM.Name,
                                     CategoryName = CAT.Description,
                                     ModelName = "",
                                     ColorName = CLR.Name,
                                     SalesQuantity = 0,
                                     PurchaseQuantity = 0,
                                     PurchaseReturnQuantity = 0,

                                     SalesReturnQuantity = SOD.Quantity,
                                 }).OrderBy(x => x.Date));



            #region Sales Return
            //var Sales_returns = ((from SO in _ROrders
            //                      join SOD in _ROrderDetails on SO.ROrderID equals SOD.ROrderID
            //                      join P in _Products on SOD.ProductID equals P.ProductID
            //                      join CAT in _Categorys on P.CategoryID equals CAT.CategoryID
            //                      join COM in _Companys on P.CompanyID equals COM.CompanyID
            //                      join STD in _StockDetails on SOD.StockDetailID equals STD.SDetailID
            //                      join CLR in _Colors on STD.ColorID equals CLR.ColorID
            //                      select new StockLedger
            //                      {
            //                          Date = SO.ReturnDate,
            //                          Code = P.Code,
            //                          CategoryCode = CAT.Code,
            //                          ProductID = P.ProductID,
            //                          CategoryID = P.CategoryID,
            //                          CompanyID = P.CompanyID,
            //                          ModelID = 1,
            //                          ColorID = CLR.ColorID,
            //                          ProductName = P.ProductName,
            //                          CompanyName = COM.Name,
            //                          CategoryName = CAT.Description,
            //                          ModelName = "",
            //                          ColorName = CLR.Name,
            //                          SalesQuantity = 0,
            //                          PurchaseQuantity = 0,
            //                          PurchaseReturnQuantity = 0,

            //                          SalesReturnQuantity = SOD.Quantity,
            //                          Quantity = SOD.Quantity,
            //                          //PurchaseRate = STD.PRate
            //                      }).OrderBy(x => x.Date));
            #endregion


            var Transdata = pquery.ToList();
            Transdata.AddRange(FinGoodsProduction.ToList());
            Transdata.AddRange(RawProductDeduct.ToList());
            Transdata.AddRange(squery.ToList());
            Transdata.AddRange(squery_credit.ToList());
            Transdata.AddRange(Sales_return.ToList());

            List<StockLedger> DataGroupBy = new List<StockLedger>();

            if (reportType == 0)
            {
                DataGroupBy = ((from item in Transdata
                                group item by new
                                {
                                    item.ProductID,
                                    item.ColorID

                                } into g
                                select new StockLedger
                                {

                                    ProductID = g.Key.ProductID,
                                    ColorID = g.Key.ColorID,
                                    CategoryID = g.FirstOrDefault().CategoryID,
                                    CompanyID = g.FirstOrDefault().CompanyID,
                                    ModelID = g.FirstOrDefault().ModelID,
                                    ProductName = g.FirstOrDefault().ProductName,
                                    CompanyName = g.FirstOrDefault().CompanyName,
                                    CategoryName = g.FirstOrDefault().CategoryName,
                                    ModelName = g.FirstOrDefault().ModelName,
                                    ColorName = g.FirstOrDefault().ColorName,
                                    Code = g.FirstOrDefault().Code,
                                    PurchaseQuantity = g.Where(o => o.Date >= fromDate && o.Date <= toDate).Sum(o => o.PurchaseQuantity),
                                    PurchaseReturnQuantity = g.Where(o => o.Date >= fromDate && o.Date <= toDate).Sum(o => o.PurchaseReturnQuantity),
                                    RawDeductQuantity = g.Where(o => o.Date >= fromDate && o.Date <= toDate).Sum(o => o.RawDeductQuantity),
                                    ProductionQuantity = g.Where(o => o.Date >= fromDate && o.Date <= toDate).Sum(o => o.ProductionQuantity),




                                    SalesQuantity = g.Where(o => o.Date >= fromDate && o.Date <= toDate).Sum(o => o.SalesQuantity),
                                    SalesReturnQuantity = g.Where(o => o.Date >= fromDate && o.Date <= toDate).Sum(o => o.SalesReturnQuantity),

                                    PreviousSalesQuantity = g.Where(o => o.Date < fromDate).Sum(o => o.SalesQuantity),
                                    PreviousPurchaseReturnQuantity = g.Where(o => o.Date < fromDate).Sum(o => o.PurchaseReturnQuantity),
                                    PreviousRawDeductQuantity = g.Where(o => o.Date < fromDate).Sum(o => o.RawDeductQuantity),
                                    PreviousSalesReturnQuantity = g.Where(o => o.Date < fromDate).Sum(o => o.SalesReturnQuantity),
                                    PreviousPurchaseQuantity = g.Where(o => o.Date < fromDate).Sum(o => o.PurchaseQuantity),
                                    PreviousProductionQuantity = g.Where(o => o.Date < fromDate).Sum(o => o.ProductionQuantity),


                                })).ToList();
                if (ProductID != 0)
                    DataGroupBy = DataGroupBy.Where(o => o.ProductID == ProductID).ToList();
            }

            else if (reportType == 1)
            {

                DataGroupBy = ((from item in Transdata
                                group item by new
                                {
                                    item.CompanyID,
                                    // item.ColorID

                                } into g
                                select new StockLedger
                                {

                                    ProductID = g.Key.CompanyID,
                                    ColorID = g.FirstOrDefault().ColorID,
                                    CategoryID = g.FirstOrDefault().CategoryID,
                                    CompanyID = g.FirstOrDefault().CompanyID,
                                    ModelID = g.FirstOrDefault().ModelID,
                                    ProductName = g.FirstOrDefault().CompanyName,
                                    CompanyName = g.FirstOrDefault().CompanyName,
                                    CategoryName = g.FirstOrDefault().CategoryName,
                                    ModelName = g.FirstOrDefault().ModelName,
                                    ColorName = g.FirstOrDefault().ColorName,
                                    Code = g.FirstOrDefault().Code,
                                    PurchaseQuantity = g.Where(o => o.Date >= fromDate && o.Date <= toDate).Sum(o => o.PurchaseQuantity),
                                    PurchaseReturnQuantity = g.Where(o => o.Date >= fromDate && o.Date <= toDate).Sum(o => o.PurchaseReturnQuantity),
                                    RawDeductQuantity = g.Where(o => o.Date >= fromDate && o.Date <= toDate).Sum(o => o.RawDeductQuantity),
                                    ProductionQuantity = g.Where(o => o.Date >= fromDate && o.Date <= toDate).Sum(o => o.ProductionQuantity),


                                    SalesQuantity = g.Where(o => o.Date >= fromDate && o.Date <= toDate).Sum(o => o.SalesQuantity),
                                    SalesReturnQuantity = g.Where(o => o.Date >= fromDate && o.Date <= toDate).Sum(o => o.SalesReturnQuantity),

                                    PreviousSalesQuantity = g.Where(o => o.Date < fromDate).Sum(o => o.SalesQuantity),
                                    PreviousPurchaseReturnQuantity = g.Where(o => o.Date < fromDate).Sum(o => o.PurchaseReturnQuantity),
                                    PreviousRawDeductQuantity = g.Where(o => o.Date < fromDate).Sum(o => o.RawDeductQuantity),
                                    PreviousSalesReturnQuantity = g.Where(o => o.Date < fromDate).Sum(o => o.SalesReturnQuantity),
                                    PreviousPurchaseQuantity = g.Where(o => o.Date < fromDate).Sum(o => o.PurchaseQuantity),
                                    PreviousProductionQuantity = g.Where(o => o.Date < fromDate).Sum(o => o.ProductionQuantity),

                                })).ToList();
                if (CompanyID != 0)
                    DataGroupBy = DataGroupBy.Where(o => o.ProductID == CompanyID).ToList();

            }

            else if (reportType == 2)
            {
                DataGroupBy = ((from item in Transdata
                                group item by new
                                {
                                    item.CategoryID,
                                    // item.ColorID

                                } into g
                                select new StockLedger
                                {

                                    ProductID = g.Key.CategoryID,
                                    ColorID = g.FirstOrDefault().ColorID,
                                    CategoryID = g.FirstOrDefault().CategoryID,
                                    CompanyID = g.FirstOrDefault().CompanyID,
                                    ModelID = g.FirstOrDefault().ModelID,
                                    ProductName = g.FirstOrDefault().CategoryName,
                                    CompanyName = g.FirstOrDefault().CompanyName,
                                    CategoryName = g.FirstOrDefault().CategoryName,
                                    ModelName = g.FirstOrDefault().ModelName,
                                    ColorName = g.FirstOrDefault().ColorName,
                                    Code = g.FirstOrDefault().CategoryCode,
                                    PurchaseQuantity = g.Where(o => o.Date >= fromDate && o.Date <= toDate).Sum(o => o.PurchaseQuantity),
                                    PurchaseReturnQuantity = g.Where(o => o.Date >= fromDate && o.Date <= toDate).Sum(o => o.PurchaseReturnQuantity),
                                    RawDeductQuantity = g.Where(o => o.Date >= fromDate && o.Date <= toDate).Sum(o => o.RawDeductQuantity),
                                    ProductionQuantity = g.Where(o => o.Date >= fromDate && o.Date <= toDate).Sum(o => o.ProductionQuantity),


                                    SalesQuantity = g.Where(o => o.Date >= fromDate && o.Date <= toDate).Sum(o => o.SalesQuantity),
                                    SalesReturnQuantity = g.Where(o => o.Date >= fromDate && o.Date <= toDate).Sum(o => o.SalesReturnQuantity),

                                    PreviousSalesQuantity = g.Where(o => o.Date < fromDate).Sum(o => o.SalesQuantity),
                                    PreviousPurchaseReturnQuantity = g.Where(o => o.Date < fromDate).Sum(o => o.PurchaseReturnQuantity),
                                    PreviousRawDeductQuantity = g.Where(o => o.Date < fromDate).Sum(o => o.RawDeductQuantity),
                                    PreviousSalesReturnQuantity = g.Where(o => o.Date < fromDate).Sum(o => o.SalesReturnQuantity),
                                    PreviousPurchaseQuantity = g.Where(o => o.Date < fromDate).Sum(o => o.PurchaseQuantity),
                                    PreviousProductionQuantity = g.Where(o => o.Date < fromDate).Sum(o => o.ProductionQuantity),

                                })).ToList();

                if (CategoryID != 0)
                    DataGroupBy = DataGroupBy.Where(o => o.ProductID == CategoryID).ToList();
            }




            return DataGroupBy;
        }

        public static IQueryable<ProductDetailsModel> GetStockDetails(this IBaseRepository<Stock> stockRepository,
           IBaseRepository<StockDetail> StockDetailRepository, IBaseRepository<Product> ProductRepository,
           IBaseRepository<Company> CompanyRepository, IBaseRepository<Category> CategoryRepository,
           IBaseRepository<Godown> GodownRepository, IBaseRepository<Color> ColorRepository)
        {
            var IMEIDetails = (from s in stockRepository.All
                               join sd in StockDetailRepository.All on s.StockID equals sd.StockID
                               join p in ProductRepository.All on sd.ProductID equals p.ProductID
                               join cat in CategoryRepository.All on p.CategoryID equals cat.CategoryID
                               join com in CompanyRepository.All on p.CompanyID equals com.CompanyID
                               join g in GodownRepository.All on sd.GodownID equals g.GodownID
                               join c in ColorRepository.All on sd.ColorID equals c.ColorID
                               select new ProductDetailsModel
                               {
                                   ProductId = s.ProductID,
                                   ProductCode = p.Code,
                                   IMENo = sd.IMENO,
                                   ProductName = p.ProductName,
                                   CategoryName = cat.Description,
                                   CompanyName = com.Name,
                                   GodownName = g.Name,
                                   GodownID = g.GodownID,
                                   StockDetailsId = sd.SDetailID,
                                   ColorName = c.Name,
                                   ColorId = c.ColorID,
                                   Status = sd.Status,
                                   SDetailID = sd.SDetailID
                               });
            return IMEIDetails;
        }

        public static List<ProductDetailsModel> GetSupplierStockDetails(
                        this IBaseRepository<Stock> stockRepository, IBaseRepository<StockDetail> stockDetailRepository,
                        IBaseRepository<POrder> POrderRepository, IBaseRepository<POrderDetail> POrderDetailRepository, IBaseRepository<Product> productRepository,
                        int SupplierID, int ProductID, int ColorID, int GodownID)
        {
            //var SupplierPOrders = POrderRepository.All.Where(i => i.SupplierID == SupplierID && i.Status == (int)EnumPurchaseType.Purchase);

            var Result = (from std in stockDetailRepository.All.Where(i => i.Status == 1)
                          join pod in POrderDetailRepository.All on new { std.ProductID, std.ColorID, std.POrderDetailID, std.GodownID } equals new { pod.ProductID, pod.ColorID, pod.POrderDetailID, pod.GodownID }
                          join p in productRepository.All on pod.ProductID equals p.ProductID
                          //join po in SupplierPOrders on pod.POrderID equals po.POrderID
                          where pod.ProductID == ProductID && pod.ColorID == ColorID && pod.GodownID == GodownID
                          select new ProductDetailsModel
                          {
                              ProductId = pod.ProductID,
                              ColorId = pod.ColorID,
                              GodownID = pod.GodownID,
                              MRPRate = std.PRate,
                              PreStock = std.Quantity,
                              IMENo = std.IMENO,
                              StockDetailsId = std.SDetailID,
                              ProductType = p.ProductType
                          }).ToList();
            return Result;
        }

        public static List<ProductDetailsModel> GetStockProductsBySupplier(
                        this IBaseRepository<Stock> stockRepository, IBaseRepository<StockDetail> stockDetailRepository,
                        IBaseRepository<Product> productRepository, IBaseRepository<Color> colorRepository, IBaseRepository<Supplier> suppRepository,
                        IBaseRepository<POrder> POrderRepository, IBaseRepository<POrderDetail> POrderDetailRepository, IBaseRepository<Category> CategoryRepository,
                        IBaseRepository<Company> CompanyRepository, IBaseRepository<ProductUnitType> ProductUnitTypeRepository,
                        int SupplierID)
        {
            List<ProductDetailsModel> Result = new List<ProductDetailsModel>();
            var SupplierPOrders = POrderRepository.All.Where(i => i.Status == (int)EnumPurchaseType.Purchase);
            if (SupplierPOrders.Count() == 0)
                return Result;

            var Products = (from std in stockDetailRepository.All.Where(i => i.Status == (int)EnumStockStatus.Stock)
                            join pod in POrderDetailRepository.All on new { std.ProductID, std.ColorID, std.GodownID, std.POrderDetailID } equals new { pod.ProductID, pod.ColorID, pod.GodownID, pod.POrderDetailID }
                            join po in SupplierPOrders on pod.POrderID equals po.POrderID
                            select new
                            {
                                pod.ProductID,
                                pod.ColorID,
                                pod.GodownID,
                                std.PRate,
                                std.Quantity,
                                std.IMENO,
                                std.SDetailID,
                                ProductType = pod.Product.ProductType
                            }).ToList();

            var StockProducts = from p in Products
                                group p by new
                                {
                                    p.ProductID,
                                    p.ColorID,
                                    p.GodownID,
                                    p.IMENO,
                                    p.ProductType
                                } into g
                                select new
                                {
                                    g.Key.ProductID,
                                    g.Key.ColorID,
                                    g.Key.GodownID,
                                    g.Key.IMENO,
                                    Quantity = g.Key.ProductType == (int)EnumProductType.NoBarcode ? g.Sum(i => i.Quantity) : 1,
                                    PRate = g.Select(i => i.PRate).FirstOrDefault(),
                                    SDetailID = g.Min(i => i.SDetailID)
                                };

            Result = (from st in StockProducts
                      join p in productRepository.All.Where(i => i.ProductStockType == EnumProductStockType.Raw_Materials) on st.ProductID equals p.ProductID
                      join cat in CategoryRepository.All on p.CategoryID equals cat.CategoryID
                      join com in CompanyRepository.All on p.CompanyID equals com.CompanyID
                      join col in colorRepository.All on st.ColorID equals col.ColorID
                      join ut in ProductUnitTypeRepository.All on p.ProUnitTypeID equals ut.ProUnitTypeID
                      select new ProductDetailsModel
                      {
                          ProductCode = p.Code,
                          ProductId = p.ProductID,
                          ProductName = p.ProductName,
                          CategoryName = cat.Description,
                          CompanyName = com.Name,
                          ColorName = col.Name,
                          ColorId = st.ColorID,
                          GodownID = st.GodownID,
                          PreStock = st.Quantity,
                          IMENo = st.IMENO,
                          StockDetailsId = st.SDetailID,
                          MRPRate = st.PRate,
                          ProductType = p.ProductType,
                          ChildUnitName = ut.UnitName,
                          UnitType = p.ProUnitTypeID,
                          ParentUnitName = ut.Description,
                          ConvertValue = p.BundleQty > 0 ? p.BundleQty : ut.ConvertValue,
                          RawConvertValue = p.BundleQty > 0 ? p.BundleQty : ut.ConvertValue,
                          BundleQty = p.BundleQty,
                      }).OrderBy(i => i.CategoryName).ThenBy(i => i.CompanyName).ToList();

            return Result;

        }

    }
}
