using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Data
{
    public static class CreditSalesOrderExtensions
    {
        public static async Task<IEnumerable<Tuple<int, string, DateTime, string,
            string, decimal, EnumSalesType>>> GetAllSalesOrderAsync(this IBaseRepository<CreditSale> salesOrderRepository,
            IBaseRepository<Customer> customerRepository, DateTime fromDate, DateTime toDate)
        {
            IQueryable<Customer> customers = customerRepository.All;

            var items = await salesOrderRepository.All.Where(i => i.IsStatus == EnumSalesType.Sales && i.IsReturn == 0 && (i.SalesDate >= fromDate && i.SalesDate <= toDate)).
                GroupJoin(customers, p => p.CustomerID, c => c.CustomerID,
                (p, c) => new { SalesOrder = p, Customers = c }).
                SelectMany(x => x.Customers.DefaultIfEmpty(), (p, c) => new { SalesOrder = p.SalesOrder, Customer = c })
                .Select(x => new
                {
                    x.SalesOrder.CreditSalesID,
                    x.SalesOrder.InvoiceNo,
                    x.SalesOrder.SalesDate,
                    x.Customer.Name,
                    x.Customer.ContactNo,
                    x.SalesOrder.Remaining,
                    x.SalesOrder.IsStatus
                }).ToListAsync();

            return items.Select(x => new Tuple<int, string, DateTime, string, string, decimal, EnumSalesType>
                (
                    x.CreditSalesID,
                    x.InvoiceNo,
                    x.SalesDate,
                    x.Name,
                    x.ContactNo,
                    x.Remaining,
                    x.IsStatus
                )).OrderByDescending(x => x.Item1).ToList();
        }

        public static IEnumerable<Tuple<int, int, int, int,
            decimal, decimal, decimal, Tuple<decimal,
                string, string, int, string>>> GetCustomSalesOrderDetails(this IBaseRepository<CreditSaleDetails> saleOrderDetailsRepository,
            int orderId, IBaseRepository<Product> productRepository, IBaseRepository<Color> colorRepository, IBaseRepository<StockDetail> stockDetailRepository)
        {
            IQueryable<Product> products = productRepository.All;
            IQueryable<Color> colors = colorRepository.All;
            IQueryable<StockDetail> stockDetails = stockDetailRepository.All;

            var items = saleOrderDetailsRepository.FindBy(x => x.CreditSalesID == orderId)
                .GroupJoin(stockDetails, s => s.StockDetailID, d => d.SDetailID, (s, d) => new { SaleDetails = s, Details = d })
                .SelectMany(x => x.Details.DefaultIfEmpty(), (s, d) => new { SaleDetails = s.SaleDetails, Details = d })
                .GroupJoin(products, d => d.Details.ProductID, p => p.ProductID,
                (d, p) => new { SaleDetails = d.SaleDetails, Details = d.Details, Products = p }).
                SelectMany(x => x.Products.DefaultIfEmpty(), (d, p) => new { SaleDetails = d.SaleDetails, Details = d.Details, Products = p })
                .GroupJoin(colors, d => d.Details.ColorID, c => c.ColorID,
                (d, c) => new { SaleDetails = d.SaleDetails, Details = d.Details, Products = d.Products, Colors = c }).
                SelectMany(x => x.Colors.DefaultIfEmpty(), (d, c) => new { SaleDetails = d.SaleDetails, Details = d.Details, Products = d.Products, Color = c })
                .Select(x => new
                {
                    x.SaleDetails.CreditSalesID,
                    x.SaleDetails.CreditSaleDetailsID,
                    x.SaleDetails.ProductID,
                    x.SaleDetails.StockDetailID,
                    x.SaleDetails.Quantity,
                    x.SaleDetails.MPRate,
                    x.SaleDetails.UnitPrice,
                    x.SaleDetails.UTAmount,
                    x.Products.ProductName,
                    x.Details.IMENO,
                    ColorId = x.Color.ColorID,
                    ColorName = x.Color.Name
                }).ToList();

            return items.Select(x => new Tuple<int, int, int, int, decimal, decimal, decimal,
                Tuple<decimal, string, string, int, string>>
                (
                    x.CreditSalesID,
                    x.CreditSaleDetailsID,
                    x.ProductID,
                    x.StockDetailID,
                    x.Quantity,
                    x.MPRate,
                    x.UnitPrice,
                    new Tuple<decimal, string, string, int, string>(x.UTAmount, x.ProductName, x.IMENO, x.ColorId, x.ColorName)
                ));
        }


        public static IEnumerable<UpcommingScheduleReport> GetUpcomingSchedule(this IBaseRepository<CreditSale> CreditSOrderRepository,
            IBaseRepository<Customer> customerRepository, IBaseRepository<CreditSalesSchedule> creditScheduleRepository,
            IBaseRepository<Product> productRepository, IBaseRepository<CreditSaleDetails> creditSalesDetailRepository,
            IBaseRepository<SOrder> SOrderRepository, IBaseRepository<SOrderDetail> SOrderDetailRepository,
            DateTime fromDate, DateTime toDate)
        {

            List<UpcommingScheduleReport> UpCommings = new List<UpcommingScheduleReport>();

            #region Credit Sales
            //var items = (from cs in CreditSOrderRepository.All.Where(i => i.IsStatus == EnumSalesType.Sales)
            //             join csd in creditSalesDetailRepository.All on cs.CreditSalesID equals csd.CreditSalesID
            //             join css in creditScheduleRepository.All on cs.CreditSalesID equals css.CreditSalesID
            //             join cus in customerRepository.All on cs.CustomerID equals cus.CustomerID
            //             join pro in productRepository.All on csd.ProductID equals pro.ProductID
            //             where (css.RemindDate == null ? (css.MonthDate >= fromDate && css.MonthDate <= toDate) : (css.RemindDate >= fromDate && css.RemindDate <= toDate)) && css.PaymentStatus == "Due"
            //             select new
            //             {
            //                 css.CSScheduleID,
            //                 CustomerRefName = cus.RefName,
            //                 CustomerRefContact = cus.RefContact,
            //                 CustomerCode = cus.Code,
            //                 CreditSalesID = cs.CreditSalesID,
            //                 InvoiceNo = cs.InvoiceNo,
            //                 CustomerName = cus.Name,
            //                 CustomerConctact = cus.ContactNo,
            //                 CustomerAddress = cus.Address,
            //                 ProductName = pro.ProductName,
            //                 SalesDate = cs.SalesDate,
            //                 PaymentDate = css.MonthDate,
            //                 TSalesAmt = cs.TSalesAmt,
            //                 SalesPrice = cs.TSalesAmt,
            //                 NetAmount = cs.NetAmount,
            //                 FixedAmt = (decimal)cs.FixedAmt,
            //                 Remaining = cs.Remaining,
            //                 InstallmentAmount = css.InstallmentAmt,
            //                 Remarks = cs.Remarks,
            //                 DownPayment = cs.DownPayment,
            //                 PenaltyInterest = cs.PenaltyInterest,
            //                 css.RemindDate,
            //                 cs.InstallmentPeriod,
            //                 cs.NoOfInstallment
            //             }).ToList();

            //UpCommings = (from upin in items
            //              group upin by new { upin.CSScheduleID, upin.PaymentDate } into g
            //              select new UpcommingScheduleReport
            //              {
            //                  CustomerRefName = g.Select(i => i.CustomerRefName).FirstOrDefault(),
            //                  CustomerRefContact = g.Select(i => i.CustomerRefContact).FirstOrDefault(),
            //                  CustomerCode = g.Select(i => i.CustomerCode).FirstOrDefault(),
            //                  CreditSalesID = g.Select(i => i.CreditSalesID).FirstOrDefault(),
            //                  InvoiceNo = g.Select(i => i.InvoiceNo).FirstOrDefault(),
            //                  CustomerName = g.Select(i => i.CustomerName).FirstOrDefault(),
            //                  CustomerConctact = g.Select(i => i.CustomerConctact).FirstOrDefault(),
            //                  CustomerAddress = g.Select(i => i.CustomerAddress).FirstOrDefault(),
            //                  ProductName = g.Select(i => i.ProductName).Distinct().ToList(),
            //                  SalesDate = g.Select(i => i.SalesDate).FirstOrDefault(),
            //                  PaymentDate = g.Select(i => i.PaymentDate).FirstOrDefault(),
            //                  TSalesAmt = g.Select(i => i.TSalesAmt).FirstOrDefault(),
            //                  SalesPrice = g.Select(i => i.SalesPrice).FirstOrDefault(),
            //                  NetAmount = g.Select(i => i.NetAmount).FirstOrDefault(),
            //                  FixedAmt = g.Select(i => i.FixedAmt).FirstOrDefault(),
            //                  Remaining = g.Select(i => i.Remaining).FirstOrDefault(),
            //                  InstallmentAmount = g.Select(i => i.InstallmentAmount).FirstOrDefault(),
            //                  Remarks = g.Select(i => i.Remarks).FirstOrDefault(),
            //                  DownPayment = g.Select(i => i.DownPayment).FirstOrDefault(),
            //                  PenaltyInterest = g.Select(i => i.PenaltyInterest).FirstOrDefault(),
            //                  RemaindDate = g.Select(i => i.RemindDate).FirstOrDefault(),
            //                  InstallmentPeriod = g.Select(i => i.InstallmentPeriod).FirstOrDefault(),
            //                  NoOfInstallment = g.Select(i => i.NoOfInstallment).FirstOrDefault()
            //              }).OrderBy(i => i.InstallmentPeriod).ToList();
            #endregion

            #region CashSales Order
            var CashSOrdersSchedules = (from c in customerRepository.All
                                        join so in SOrderRepository.All on c.CustomerID equals so.CustomerID
                                        join sod in SOrderDetailRepository.All on so.SOrderID equals sod.SOrderID
                                        join p in productRepository.All on sod.ProductID equals p.ProductID
                                        where so.Status == (int)EnumSalesType.Sales && (c.RemindDate >= fromDate && c.RemindDate <= toDate && c.TotalDue != 0)
                                        select new
                                        {
                                            c.CustomerID,
                                            c.Code,
                                            c.Name,
                                            c.Address,
                                            c.ContactNo,
                                            c.TotalDue,
                                            c.RefName,
                                            c.RemindDate,
                                            c.RefContact,
                                            c.RefAddress,
                                            so.SOrderID,
                                            so.InvoiceDate,
                                            so.InvoiceNo,
                                            p.ProductName
                                        }).OrderByDescending(i => i.InvoiceDate);

            var CashRemindOrders = (from so in CashSOrdersSchedules
                                    group so by new { so.CustomerID, so.Code, so.Name, so.Address, so.ContactNo, so.TotalDue, so.RefName, so.RefContact, so.RefAddress, so.RemindDate, so.SOrderID, so.InvoiceDate, so.InvoiceNo } into g
                                    select new
                                    {
                                        g.Key.CustomerID,
                                        g.Key.Name,
                                        g.Key.Code,
                                        g.Key.Address,
                                        g.Key.ContactNo,
                                        g.Key.TotalDue,
                                        g.Key.RefName,
                                        g.Key.RefContact,
                                        g.Key.RemindDate,
                                        g.Key.RefAddress,
                                        SalesDate = g.Key.InvoiceDate,
                                        InvoiceNo = g.Key.InvoiceNo,
                                        ProductName = g.Select(i => i.ProductName).ToList()
                                    }).OrderByDescending(i => i.SalesDate).ToList();

            var FinalCashRemindOrders = (from so in CashRemindOrders
                                         group so by new { so.CustomerID, so.Code, so.Name, so.Address, so.ContactNo, so.TotalDue, so.RefName, so.RefContact, so.RefAddress, so.RemindDate } into g
                                         select new UpcommingScheduleReport
                                         {
                                             CustomerID = g.Key.CustomerID,
                                             CustomerName = g.Key.Name,
                                             CustomerCode = g.Key.Code,
                                             CustomerAddress = g.Key.Address,
                                             CustomerConctact = g.Key.ContactNo,
                                             InstallmentAmount = g.Key.TotalDue,
                                             CustomerRefName = g.Key.RefName,
                                             CustomerRefContact = g.Key.RefContact,
                                             RemaindDate = g.Key.RemindDate,
                                             SalesDate = g.Select(i => i.SalesDate).FirstOrDefault(),
                                             InvoiceNo = g.Select(i => i.InvoiceNo).FirstOrDefault(),
                                             ProductName = g.Select(i => i.ProductName).FirstOrDefault(),
                                             InstallmentPeriod = "Cash Sales"
                                         }).OrderByDescending(i => i.SalesDate);

            #endregion

            UpCommings.AddRange(FinalCashRemindOrders);
            return UpCommings;
        }


        public static IEnumerable<UpcommingScheduleReport> GetScheduleCollection(this IBaseRepository<CreditSale> salesOrderRepository,
          IBaseRepository<Customer> customerRepository, IBaseRepository<CreditSalesSchedule> creditScheduleRepository,
          DateTime fromDate, DateTime toDate, int concernID)
        {
            var items = (from cs in salesOrderRepository.All
                         join csd in creditScheduleRepository.All on cs.CreditSalesID equals csd.CreditSalesID
                         join cus in customerRepository.All on cs.CustomerID equals cus.CustomerID
                         // join pro in db.Products on cs.ProductID equals pro.ProductID
                         where (csd.PaymentDate >= fromDate && csd.PaymentDate <= toDate) && (csd.PaymentStatus == "Paid" && cs.ConcernID == concernID && csd.InstallmentAmt != 0)
                         select new UpcommingScheduleReport
                         {
                             CustomerCode = cus.Code,
                             InvoiceNo = cs.InvoiceNo,
                             CustomerName = cus.Name,
                             CustomerConctact = cus.ContactNo,
                             CustomerAddress = cus.Address,
                             // pro.ProductName,
                             SalesDate = cs.SalesDate,
                             PaymentDate = csd.PaymentDate,
                             TSalesAmt = cs.TSalesAmt,
                             NetAmount = cs.NetAmount,
                             FixedAmt = (decimal)cs.FixedAmt,
                             Remaining = cs.Remaining,
                             InstallmentAmount = csd.InstallmentAmt,
                             Remarks = csd.Remarks,
                             DownPayment = cs.DownPayment,
                             InstallmentPeriod = cs.InstallmentPeriod,
                             PenaltyInterest = cs.PenaltyInterest,
                             SalesPrice = cs.TSalesAmt
                         }).OrderBy(i => i.InstallmentPeriod).ToList();
            return items;
        }

        /// <summary>
        /// Defaulting Customer Summary
        /// </summary>
        /// <param name="salesOrderRepository"></param>
        /// <param name="customerRepository"></param>
        /// <param name="creditScheduleRepository"></param>
        /// <param name="date"></param>
        /// <param name="concernID"></param>
        /// <returns></returns>
        public static IEnumerable<Tuple<string, string, string, decimal, decimal>> GetDefaultingCustomer(this IBaseRepository<CreditSale> salesOrderRepository,
         IBaseRepository<Customer> customerRepository, IBaseRepository<CreditSalesSchedule> creditScheduleRepository, DateTime date, int concernID)
        {
            var items = (from o in salesOrderRepository.All.Where(i => i.IsStatus == EnumSalesType.Sales && i.Remaining > 0)
                         join od in creditScheduleRepository.All on o.CreditSalesID equals od.CreditSalesID
                         join c in customerRepository.All on o.CustomerID equals c.CustomerID
                         where (od.MonthDate <= date && od.PaymentStatus == "Due")
                         group od by new { o.CustomerID, c.Code, c.Name, c.ContactNo, c.Address } into g //c.CompanyName
                         select new
                         {
                             code = g.Key.Code,
                             name = g.Key.Name, //g.Key.CompanyName
                             contact = g.Key.ContactNo + " & " + g.Key.Address,
                             count = g.Count(),
                             amount = g.Sum(od => od.InstallmentAmt)
                         }).ToList();
            return items.Select(x => new Tuple<string, string, string, decimal, decimal>
                (
                    x.code,
                    x.name,
                    x.contact,
                    x.count,
                    x.amount
                ));
        }

        /// <summary>
        /// Defaulting customer Details
        /// </summary>
        /// <param name="salesOrderRepository"></param>
        /// <param name="customerRepository"></param>
        /// <param name="creditScheduleRepository"></param>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="concernID"></param>
        /// <returns></returns>
        public static IEnumerable<Tuple<string, string, string, string, DateTime, DateTime, decimal, Tuple<decimal, decimal, decimal, decimal, string, decimal, decimal, Tuple<int, decimal>>>>
            GetDefaultingCustomer(this IBaseRepository<CreditSale> salesOrderRepository,
                                 IBaseRepository<Customer> customerRepository, IBaseRepository<CreditSalesSchedule> creditScheduleRepository,
                                 DateTime fromDate, DateTime toDate, int concernID)
        {
            var CreditSchedules = creditScheduleRepository.All;
            var items = (from cs in salesOrderRepository.All.Where(i => i.IsStatus == EnumSalesType.Sales && i.Remaining > 0)
                         join csd in CreditSchedules on cs.CreditSalesID equals csd.CreditSalesID
                         join cus in customerRepository.All on cs.CustomerID equals cus.CustomerID
                         where (cs.SalesDate >= fromDate && cs.SalesDate <= toDate) && csd.PaymentStatus == "Due" && cs.ConcernID == concernID && csd.MonthDate <= DateTime.Today
                         select new
                         {
                             cus.CustomerID,
                             cus.Code,
                             cs.CreditSalesID,
                             cs.InvoiceNo,
                             cus.Name,
                             ContactNo = cus.ContactNo + " & " + cus.Address,
                             // pro.ProductName,
                             cs.SalesDate,
                             csd.MonthDate,
                             cs.TSalesAmt,
                             cs.NetAmount,
                             cs.FixedAmt,
                             cs.Remaining,
                             csd.InstallmentAmt,
                             csd.Remarks,
                             cs.DownPayment,
                             cs.PenaltyInterest
                         }).ToList();

            var Result = (from item in items
                          group item by new { item.CustomerID, item.Code, item.Name, item.ContactNo, item.CreditSalesID, item.TSalesAmt, item.NetAmount, item.FixedAmt, item.Remaining, item.DownPayment, item.PenaltyInterest } into g
                          select new
                          {
                              g.Key.CustomerID,
                              g.Key.Code,
                              g.LastOrDefault().InvoiceNo,
                              g.Key.Name,
                              g.Key.ContactNo,
                              g.LastOrDefault().SalesDate,
                              g.LastOrDefault().MonthDate,
                              TSalesAmt = g.Key.TSalesAmt,
                              NetAmount = g.Key.NetAmount,
                              FixedAmt = g.Key.FixedAmt,
                              Remaining = g.Key.Remaining,
                              DefaultAmt = g.Sum(i => i.InstallmentAmt),
                              g.LastOrDefault().Remarks,
                              DownPayment = g.Key.DownPayment,
                              PenaltyInterest = g.Key.PenaltyInterest,
                              g.Key.CreditSalesID,
                              TotalReceiveAmt = CreditSchedules.Where(i => i.CreditSalesID == g.Key.CreditSalesID && i.PaymentStatus == "Paid").Count() == 0 ? g.Key.DownPayment : CreditSchedules.Where(i => i.CreditSalesID == g.Key.CreditSalesID && i.PaymentStatus == "Paid").Sum(j => j.InstallmentAmt) + g.Key.DownPayment,
                          }).Where(i => i.DefaultAmt > 0);

            var Result2 = (from item in Result
                           group item by new { item.CustomerID, item.Code, item.Name, item.ContactNo } into g
                           select new
                           {
                               g.Key.Code,
                               g.LastOrDefault().InvoiceNo,
                               g.Key.Name,
                               g.Key.ContactNo,
                               g.LastOrDefault().SalesDate,
                               g.LastOrDefault().MonthDate,
                               TSalesAmt = g.Sum(i => i.TSalesAmt),
                               NetAmount = g.Sum(i => i.NetAmount),
                               FixedAmt = g.Sum(i => i.FixedAmt),
                               Remaining = g.Sum(i => i.Remaining),
                               DefaultAmt = g.Sum(i => i.DefaultAmt),
                               g.LastOrDefault().Remarks,
                               DownPayment = g.Sum(i => i.DownPayment),
                               PenaltyInterest = g.Sum(i => i.PenaltyInterest),
                               g.LastOrDefault().CreditSalesID,
                               TotalReceiveAmt = Convert.ToDecimal(g.Sum(i => i.TotalReceiveAmt)),
                           }).Where(i => i.DefaultAmt > 0);

            return Result2.Select(x => new Tuple<string, string, string, string, DateTime, DateTime, decimal, Tuple<decimal, decimal, decimal, decimal, string, decimal, decimal, Tuple<int, decimal>>>
                (
                    x.Code,
                    x.InvoiceNo,
                    x.Name,
                    x.ContactNo,
                    x.SalesDate,
                    x.MonthDate,
                    x.TSalesAmt,
                    new Tuple<decimal, decimal, decimal, decimal, string, decimal, decimal, Tuple<int, decimal>>
                    (
                    x.NetAmount,
                    (decimal)x.FixedAmt,
                    x.Remaining,
                    x.DefaultAmt,
                    x.Remarks,
                    x.DownPayment,
                    x.PenaltyInterest,
                    new Tuple<int, decimal>(x.CreditSalesID, x.TotalReceiveAmt)
                    )
                ));
        }


        public static IEnumerable<Tuple<string, string, DateTime, string, decimal, decimal, decimal, Tuple<decimal, decimal, decimal, decimal, decimal, int, string>>>
           GetCreditSalesReportByConcernID(this IBaseRepository<CreditSale> CreditSaleRepository, IBaseRepository<Customer> customerRepository,
            IBaseRepository<CreditSaleDetails> CreditSaleDetailsRepository, DateTime fromDate, DateTime toDate, int concernID, int CustomerType)
        {
            IQueryable<Customer> Customers = null;
            if (CustomerType == 0)
                Customers = customerRepository.All;
            else
                Customers = customerRepository.All.Where(i => i.CustomerType == (EnumCustomerType)CustomerType);

            var oSalesData = (from SO in CreditSaleRepository.All
                              join SOD in CreditSaleDetailsRepository.All on SO.CreditSalesID equals SOD.CreditSalesID
                              join cus in Customers on SO.CustomerID equals cus.CustomerID
                              where (SO.SalesDate >= fromDate && SO.SalesDate <= toDate && SO.IsStatus == EnumSalesType.Sales && SO.ConcernID == concernID)
                              group SO by new
                              {
                                  cus.CustomerID,
                                  cus.Code,
                                  cus.Name,
                                  cus.TotalDue,
                                  SO.InvoiceNo,
                                  SO.SalesDate,
                                  SO.TSalesAmt,
                                  SO.Discount,
                                  SO.NetAmount,
                                  SO.DownPayment,
                                  Remaining = SO.NetAmount - SO.DownPayment,
                                  SO.InstallmentPeriod
                              } into g
                              select new
                              {
                                  g.Key.Code,
                                  g.Key.Name,
                                  g.Key.SalesDate,
                                  g.Key.InvoiceNo,
                                  g.Key.TSalesAmt,
                                  g.Key.Discount,
                                  g.Key.NetAmount,
                                  g.Key.DownPayment,
                                  g.Key.Remaining,
                                  TotalOffer = g.Select(i => i.CreditSaleDetails).FirstOrDefault(),
                                  g.Key.TotalDue,
                                  g.Key.CustomerID,
                                  g.Key.InstallmentPeriod
                              }).ToList();

            return oSalesData.Select(x => new Tuple<string, string, DateTime, string, decimal, decimal, decimal, Tuple<decimal, decimal, decimal, decimal, decimal, int, string>>
                (
                 x.Code,
                 x.Name,
                x.SalesDate,
                                    x.InvoiceNo,
                                    x.TSalesAmt,
                                    x.Discount,
                                    x.NetAmount,
                                     new Tuple<decimal, decimal, decimal, decimal, decimal, int, string>(
                                    (decimal)x.DownPayment,
                                    x.Remaining,
                                    0m,
                                    x.TotalOffer.Sum(i => i.PPOffer),
                                    x.TotalDue,
                                    x.CustomerID,
                                    x.InstallmentPeriod
                                    )

                ));
        }

        public static IEnumerable<Tuple<DateTime, string, string, string, decimal, decimal, decimal, Tuple<decimal, decimal, decimal, decimal, decimal, string, string, Tuple<int>>>>
    GetCreditSalesDetailReportByConcernID(this IBaseRepository<CreditSale> CreditSaleRepository, IBaseRepository<CreditSaleDetails> CreditSaleDetailsRepository, IBaseRepository<Product> productRepository,
    IBaseRepository<StockDetail> stockdetailRepository, DateTime fromDate, DateTime toDate, int concernID)
        {
            var oSalesDetailData = (from SOD in CreditSaleDetailsRepository.All
                                    join SO in CreditSaleRepository.All on SOD.CreditSalesID equals SO.CreditSalesID
                                    join P in productRepository.All on SOD.ProductID equals P.ProductID
                                    join std in stockdetailRepository.All on SOD.StockDetailID equals std.SDetailID
                                    where (SO.SalesDate >= fromDate && SO.SalesDate <= toDate && SO.IsStatus == EnumSalesType.Sales && SO.ConcernID == concernID)
                                    select new
                                    {
                                        SO.CreditSalesID,
                                        SO.InvoiceNo,
                                        SO.Customer.Name,
                                        SO.SalesDate,
                                        SO.TSalesAmt,
                                        SO.Discount,
                                        SO.NetAmount,
                                        SO.DownPayment,
                                        SO.Remaining,
                                        P.ProductID,
                                        P.ProductName,
                                        SOD.UnitPrice,
                                        SOD.UTAmount,
                                        SOD.PPOffer,
                                        SOD.Quantity,
                                        std.IMENO,
                                        ColorName = std.Color.Name
                                    }).OrderByDescending(x => x.CreditSalesID).ToList();

            return oSalesDetailData.Select(x => new Tuple<DateTime, string, string, string, decimal, decimal, decimal, Tuple<decimal, decimal, decimal, decimal, decimal, string, string, Tuple<int>>>
                (
                 x.SalesDate,
                 x.InvoiceNo,
                 x.ProductName,
                 x.Name,
                 x.UnitPrice,
                //x.PPDAmount,
                 x.UTAmount,// - x.PPOffer,
                 x.TSalesAmt,
                 new Tuple<decimal, decimal, decimal, decimal, decimal, string, string, Tuple<int>>(
                                    x.Discount,
                                    x.NetAmount,
                                   (decimal)x.DownPayment,
                                   x.Remaining,
                                   x.Quantity,
                                   x.IMENO,
                                   x.ColorName
                                   ,
                                   new Tuple<int>(x.CreditSalesID)
                                   )

                ));
        }

        public static IEnumerable<Tuple<string, string, string, string, DateTime, DateTime, decimal, Tuple<decimal, decimal, decimal, decimal, string, decimal>>>
            GetCreditCollectionReport(this IBaseRepository<CreditSale> salesOrderRepository,
            IBaseRepository<Customer> customerRepository, IBaseRepository<CreditSalesSchedule> creditScheduleRepository,
            DateTime fromDate, DateTime toDate, int concernID, int CustomerID)
        {
            IEnumerable<CreditSale> CrditSaleList = new List<CreditSale>();
            if (CustomerID > 0)
                CrditSaleList = salesOrderRepository.All.Where(i => i.CustomerID == CustomerID);
            else
                CrditSaleList = salesOrderRepository.All;

            var items = (from cs in CrditSaleList
                         join csd in creditScheduleRepository.All on cs.CreditSalesID equals csd.CreditSalesID
                         join cus in customerRepository.All on cs.CustomerID equals cus.CustomerID
                         where (csd.PaymentDate >= fromDate && csd.PaymentDate <= toDate) && csd.PaymentStatus == "Paid"
                         select new
                         {
                             cus.Code,
                             cs.InvoiceNo,
                             cus.Name,
                             cus.ContactNo,
                             cs.SalesDate,
                             csd.PaymentDate,
                             cs.TSalesAmt,
                             cs.NetAmount,
                             cs.FixedAmt,
                             cs.Remaining,
                             csd.InstallmentAmt,
                             csd.Remarks,
                             cs.DownPayment
                         }).ToList();

            return items.Select(x => new Tuple<string, string, string, string, DateTime, DateTime, decimal, Tuple<decimal, decimal, decimal, decimal, string, decimal>>
                (
                    x.Code,
                    x.InvoiceNo,
                    x.Name,
                    x.ContactNo,
                    x.SalesDate,
                    x.PaymentDate,
                    x.TSalesAmt,
                    new Tuple<decimal, decimal, decimal, decimal, string, decimal>
                    (
                    x.NetAmount,
                    (decimal)x.FixedAmt,
                    x.Remaining,
                    x.InstallmentAmt,
                    x.Remarks,
                    x.DownPayment
                    )
                ));
        }

        public static decimal GetDefaultAmount(this IBaseRepository<CreditSale> CreditSaleRepository, IBaseRepository<CreditSalesSchedule> CreditSalesSchedulesRepository, int CreditSaleID, DateTime FromDate)
        {
            var CSS = CreditSalesSchedulesRepository.All.Where(i => i.CreditSalesID == CreditSaleID && i.MonthDate < FromDate && i.PaymentStatus == "Due");
            if (CSS.Count() > 0)
            {
                return CSS.Sum(i => i.InstallmentAmt);
            }
            return 0m;
        }

        public static List<ProductWiseSalesReportModel> ProductWiseCreditSalesReport(this IBaseRepository<CreditSale> CreditSOrderRepository, IBaseRepository<CreditSaleDetails> CreditSOrderDetailRepo,
            IBaseRepository<Customer> CustomerRepository, IBaseRepository<Employee> EmployeeRepository, IBaseRepository<Product> ProductRepository,
            int ConcernID, int CustomerID, DateTime fromDate, DateTime toDate)
        {
            List<CreditSale> CreditSOrders = new List<CreditSale>();
            if (CustomerID != 0)
                CreditSOrders = CreditSOrderRepository.All.Where(i => i.CustomerID == CustomerID && i.SalesDate >= fromDate && i.SalesDate <= toDate).ToList();
            else
                CreditSOrders = CreditSOrderRepository.All.Where(i => i.SalesDate >= fromDate && i.SalesDate <= toDate).ToList();

            var CreditSOrderDetails = CreditSOrderDetailRepo.All;
            var Products = ProductRepository.All;
            var Customers = CustomerRepository.All;
            var Employees = EmployeeRepository.All;


            var result = from SO in CreditSOrders.Where(i => i.IsStatus == EnumSalesType.Sales)
                         join SOD in CreditSOrderDetails on SO.CreditSalesID equals SOD.CreditSalesID
                         join P in Products on SOD.ProductID equals P.ProductID
                         join C in Customers on SO.CustomerID equals C.CustomerID
                         join E in Employees on C.EmployeeID equals E.EmployeeID
                         select new ProductWiseSalesReportModel
                         {
                             Date = SO.SalesDate,
                             EmployeeCode = E.Code,
                             EmployeeName = E.Name,
                             CustomerCode = C.Code,
                             CustomerName = C.Name,
                             Address = C.Address,
                             Mobile = C.ContactNo,
                             ProductName = P.ProductName,
                             Quantity = SOD.Quantity,
                             SalesRate = SOD.UnitPrice - SOD.PPOffer,
                             TotalAmount = SOD.UTAmount
                         };

            var fresult = from r in result
                          group r by new { r.Date, r.EmployeeCode, r.EmployeeName, r.CustomerCode, r.CustomerName, r.Address, r.Mobile, r.ProductName, r.SalesRate } into g
                          select new ProductWiseSalesReportModel
                          {
                              Date = g.Key.Date,
                              EmployeeCode = g.Key.EmployeeCode,
                              EmployeeName = g.Key.EmployeeName,
                              CustomerCode = g.Key.CustomerCode,
                              CustomerName = g.Key.CustomerName,
                              Address = g.Key.Address,
                              Mobile = g.Key.Mobile,
                              ProductName = g.Key.ProductName,
                              SalesRate = g.Key.SalesRate,
                              Quantity = g.Sum(i => i.Quantity),
                              TotalAmount = g.Sum(i => i.TotalAmount)
                          };

            return fresult.ToList();
        }

        public static List<ProductWiseSalesReportModel> ProductWiseCreditSalesDetailsReport(this IBaseRepository<CreditSale> CreditSOrderRepository,
    IBaseRepository<CreditSaleDetails> CreditSOrderDetailRepo, IBaseRepository<Company> CompanyRepository,
    IBaseRepository<Category> CategoryRepository, IBaseRepository<Product> ProductRepository, IBaseRepository<StockDetail> StockDetailRepository,
    int CompanyID, int CategoryID, int ProductID, DateTime fromDate, DateTime toDate)
        {
            var Products = ProductRepository.All;
            if (CompanyID != 0)
                Products = Products.Where(i => i.CompanyID == CompanyID);
            if (CategoryID != 0)
                Products = Products.Where(i => i.CategoryID == CategoryID);
            if (ProductID != 0)
                Products = Products.Where(i => i.ProductID == ProductID);

            var CreditSOrderDetails = CreditSOrderDetailRepo.All;
            var CreditSOrders = CreditSOrderRepository.All.Where(i => i.SalesDate >= fromDate && i.SalesDate <= toDate && i.IsStatus == EnumSalesType.Sales);

            var result = from SO in CreditSOrders
                         join SOD in CreditSOrderDetails on SO.CreditSalesID equals SOD.CreditSalesID
                         join STD in StockDetailRepository.All on SOD.StockDetailID equals STD.SDetailID
                         join P in Products on SOD.ProductID equals P.ProductID
                         join COM in CompanyRepository.All on P.CompanyID equals COM.CompanyID
                         join CAT in CategoryRepository.All on P.CategoryID equals CAT.CategoryID
                         select new ProductWiseSalesReportModel
                         {
                             Date = SO.SalesDate,
                             InvoiceNo = SO.InvoiceNo,
                             ProductName = P.ProductName,
                             CategoryName = CAT.Description,
                             CompanyName = COM.Name,
                             Quantity = SOD.Quantity,
                             SalesRate = SOD.UnitPrice - SOD.PPOffer,
                             TotalAmount = SOD.UTAmount,
                             IMEI = STD.IMENO
                         };

            return result.ToList();
        }

        public static List<SOredersReportModel> SRWiseCreditSalesReport(this IBaseRepository<CreditSale> CreditSOrderRepository, IBaseRepository<CreditSalesSchedule> CreditSalesScheduleRepo,
         IBaseRepository<Customer> CustomerRepository, IBaseRepository<Employee> EmployeeRepository,
          int EmployeeID, DateTime fromDate, DateTime toDate)
        {
            IQueryable<Customer> Customers = null;
            var CreditSOrders = CreditSOrderRepository.All.Where(i => i.SalesDate >= fromDate && i.SalesDate <= toDate && i.IsStatus == EnumSalesType.Sales).ToList();

            if (EmployeeID != 0)
                Customers = CustomerRepository.All.Where(i => i.EmployeeID == EmployeeID);
            else
                Customers = CustomerRepository.All;

            var Employees = EmployeeRepository.All;


            var downpayments = (from SO in CreditSOrders
                                join C in Customers on SO.CustomerID equals C.CustomerID
                                join E in Employees on C.EmployeeID equals E.EmployeeID
                                select new SOredersReportModel
                                {
                                    InvoiceDate = SO.SalesDate,
                                    InvoiceNo = SO.InvoiceNo,
                                    EmployeeCode = E.Code,
                                    EmployeeName = E.Name,
                                    CustomerCode = C.Code,
                                    CustomerName = C.Name,
                                    CustomerAddress = C.Address,
                                    CustomerContactNo = C.ContactNo,
                                    RecAmount = SO.DownPayment,
                                    AdjAmount = SO.LastPayAdjAmt,
                                }).ToList();

            var InstallCollections = from SO in CreditSOrderRepository.All.Where(i => i.IsStatus == EnumSalesType.Sales)
                                     join sod in CreditSalesScheduleRepo.All on SO.CreditSalesID equals sod.CreditSalesID
                                     join C in Customers on SO.CustomerID equals C.CustomerID
                                     join E in Employees on C.EmployeeID equals E.EmployeeID
                                     where sod.PaymentStatus.Equals("Paid") && (sod.PaymentDate >= fromDate && sod.PaymentDate <= toDate)
                                     select new SOredersReportModel
                                     {
                                         InvoiceDate = sod.PaymentDate,
                                         InvoiceNo = SO.InvoiceNo + "-" + sod.ScheduleNo,
                                         EmployeeCode = E.Code,
                                         EmployeeName = E.Name,
                                         CustomerCode = C.Code,
                                         CustomerName = C.Name,
                                         CustomerAddress = C.Address,
                                         CustomerContactNo = C.ContactNo,
                                         RecAmount = sod.InstallmentAmt,
                                         AdjAmount = 0,
                                     };

            downpayments.AddRange(InstallCollections);
            return downpayments;
        }

        public static IQueryable<SOredersReportModel> GetAdminCrSalesReport(this IBaseRepository<CreditSale> SOrderRepository,
             IBaseRepository<Customer> CustomerRepository, IBaseRepository<SisterConcern> SisterConcernRepository,
            int ConcernID, DateTime fromDate, DateTime toDate)
        {
            IQueryable<Customer> Customers = null;
            if (ConcernID != 0)
                Customers = CustomerRepository.GetAll().Where(i => i.ConcernID == ConcernID);
            else
                Customers = CustomerRepository.GetAll();

            var Sales = from so in SOrderRepository.GetAll()
                        join c in Customers on so.CustomerID equals c.CustomerID
                        join s in SisterConcernRepository.GetAll() on so.ConcernID equals s.ConcernID
                        where so.IsStatus == EnumSalesType.Sales && (so.SalesDate >= fromDate && so.SalesDate <= toDate)
                        select new SOredersReportModel
                        {
                            ConcernID = so.ConcernID,
                            ConcernName = s.Name,
                            CustomerCode = c.Code,
                            CustomerAddress = c.Address,
                            CustomerName = c.Name,
                            CustomerContactNo = c.ContactNo,
                            CustomerTotalDue = c.TotalDue,
                            InvoiceDate = so.SalesDate,
                            InvoiceNo = so.InvoiceNo,
                            Grandtotal = so.TSalesAmt,
                            NetDiscount = so.Discount,
                            TotalOffer = 0m,
                            AdjAmount = 0m,
                            TotalAmount = so.NetAmount,
                            RecAmount = (decimal)so.DownPayment,
                            PaymentDue = so.NetAmount - (decimal)so.DownPayment,
                            InstallmentPeriod = so.InstallmentPeriod
                        };

            return Sales.OrderBy(i => i.ConcernID).ThenByDescending(i => i.InvoiceDate);
        }


        public static IQueryable<CashCollectionReportModel> AdminInstallmentColllections(this IBaseRepository<CreditSale> SOrderRepository,
                    IBaseRepository<Customer> CustomerRepository, IBaseRepository<SisterConcern> SisterConcernRepository,
                    IBaseRepository<CreditSalesSchedule> CreditSalesScheduleRepository,
                    int ConcernID, DateTime fromDate, DateTime toDate)
        {
            IQueryable<Customer> Customers = null;
            if (ConcernID != 0)
                Customers = CustomerRepository.GetAll().Where(i => i.ConcernID == ConcernID);
            else
                Customers = CustomerRepository.GetAll();

            var Sales = from so in SOrderRepository.GetAll()
                        join cs in CreditSalesScheduleRepository.GetAll() on so.CreditSalesID equals cs.CreditSalesID
                        join c in Customers on so.CustomerID equals c.CustomerID
                        join s in SisterConcernRepository.GetAll() on so.ConcernID equals s.ConcernID
                        where so.IsStatus == EnumSalesType.Sales && cs.PaymentStatus.Equals("Paid") && (cs.PaymentDate >= fromDate && cs.PaymentDate <= toDate)
                        select new CashCollectionReportModel
                        {
                            ConcernID = so.ConcernID,
                            ConcernName = s.Name,
                            CustomerCode = c.Code,
                            CustomerName = c.Name,
                            EntryDate = cs.PaymentDate,
                            ReceiptNo = so.InvoiceNo + "-" + cs.ScheduleNo,
                            Amount = cs.InstallmentAmt,
                            ModuleType = "Installment",
                            AdjustAmt = (so.NoOfInstallment == cs.ScheduleNo && cs.InstallmentAmt > 0) ? so.LastPayAdjAmt : 0m
                        };

            return Sales;
        }

    }
}
