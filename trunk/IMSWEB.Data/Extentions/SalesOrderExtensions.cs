using IMSWEB.Model;
using IMSWEB.Model.TOs;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace IMSWEB.Data
{
    public static class SalesOrderExtensions
    {
        public static async Task<IEnumerable<Tuple<int, string, DateTime, string,
           string, decimal, EnumSalesType, Tuple<string>>>> GetAllSalesOrderAsync(this IBaseRepository<SOrder> salesOrderRepository,
           IBaseRepository<Customer> customerRepository, IBaseRepository<SisterConcern> SisterConcernRepository,
           DateTime fromDate, DateTime toDate, List<EnumSalesType> SalesType, bool IsVATManager, int concernID,
           string InvoiceNo, string ContactNo, string CustomerName, string AccountNo)
        {
            IQueryable<Customer> customers = customerRepository.All;
            IQueryable<SOrder> sorders = salesOrderRepository.All.Where(x => x.IsReplacement == 0 && SalesType.Contains((EnumSalesType)x.Status) && x.IsApproved);
            bool IsSearchByDate = true;
            if (!string.IsNullOrWhiteSpace(InvoiceNo))
            {
                sorders = sorders.Where(i => i.InvoiceNo.Contains(InvoiceNo));
                IsSearchByDate = false;
            }
            if (!string.IsNullOrWhiteSpace(ContactNo))
            {
                customers = customers.Where(i => i.ContactNo.Contains(ContactNo));
                IsSearchByDate = false;
            }
            if (!string.IsNullOrWhiteSpace(CustomerName))
            {
                customers = customers.Where(i => i.Name.Contains(CustomerName));
                IsSearchByDate = false;
            }

            if (!string.IsNullOrWhiteSpace(AccountNo))
            {
                customers = customers.Where(i => i.Code.Contains(AccountNo));
                IsSearchByDate = false;
            }

            if (IsSearchByDate)
                sorders = sorders.Where(i => (i.InvoiceDate >= fromDate && i.InvoiceDate <= toDate));


            var items = await (from so in sorders
                               join c in customers on so.CustomerID equals c.CustomerID
                               select new ProductWiseSalesReportModel
                               {
                                   SOrderID = so.SOrderID,
                                   InvoiceNo = so.InvoiceNo,
                                   Date = so.InvoiceDate,
                                   CustomerName = c.Name,
                                   //CustomerCode = c.Code,
                                   Mobile = c.ContactNo,
                                   TotalDue = c.TotalDue,
                                   Status = so.Status,
                                   IsReplacement = so.IsReplacement,
                                   //TotalAmount = so.TotalAmount
                               }).ToListAsync();

            List<ProductWiseSalesReportModel> finalData = new List<ProductWiseSalesReportModel>();
            if (IsVATManager)
            {
                items = items.OrderByDescending(i => i.Date).ToList();
                var oConcern = SisterConcernRepository.All.FirstOrDefault(i => i.ConcernID == concernID);
                decimal FalesSales = (items.Sum(i => i.TotalAmount) * oConcern.SalesShowPercent) / 100m;
                decimal FalesSalesCount = 0m;


                foreach (var item in items)
                {
                    FalesSalesCount += item.TotalAmount;
                    if (FalesSalesCount <= FalesSales)
                        finalData.Add(item);
                    else
                        break;
                }
            }
            else
                finalData = items;

            return finalData.Select(x => new Tuple<int, string, DateTime, string, string, decimal, EnumSalesType, Tuple<string>>
                (
                    x.SOrderID,
                    x.InvoiceNo,
                    x.Date,
                    x.CustomerName,
                    x.Mobile,
                    x.TotalDue,
                    (EnumSalesType)x.Status,
                    new Tuple<string>
                    (x.CustomerCode)
                )).OrderByDescending(x => x.Item3).ThenByDescending(i => i.Item2).ToList();
        }


        public static async Task<IEnumerable<Tuple<int, string, DateTime, string, string, decimal, EnumSalesType, Tuple<string, bool, string>>>> GetAllAdvanceSalesOrderAsync(this IBaseRepository<SOrder> salesOrderRepository, IBaseRepository<Customer> customerRepository, IBaseRepository<SisterConcern> SisterConcernRepository, IBaseRepository<Employee> EmployeeRepository, DateTime fromDate, DateTime toDate, List<EnumSalesType> SalesType, bool IsVATManager, int concernID, string InvoiceNo, string ContactNo, string CustomerName, string AccountNo)
        {
            IQueryable<Customer> customers = customerRepository.All;
            IQueryable<Employee> employees = EmployeeRepository.All;
            IQueryable<SOrder> sorders = salesOrderRepository.All.Where(x => x.IsReplacement == 0 && SalesType.Contains((EnumSalesType)x.Status) && x.IsAdvanceSale == true);
            bool IsSearchByDate = true;
            if (!string.IsNullOrWhiteSpace(InvoiceNo))
            {
                sorders = sorders.Where(i => i.InvoiceNo.Contains(InvoiceNo));
                IsSearchByDate = false;
            }
            if (!string.IsNullOrWhiteSpace(ContactNo))
            {
                customers = customers.Where(i => i.ContactNo.Contains(ContactNo));
                IsSearchByDate = false;
            }
            if (!string.IsNullOrWhiteSpace(CustomerName))
            {
                customers = customers.Where(i => i.Name.Contains(CustomerName));
                IsSearchByDate = false;
            }

            if (!string.IsNullOrWhiteSpace(AccountNo))
            {
                customers = customers.Where(i => i.Code.Contains(AccountNo));
                IsSearchByDate = false;
            }

            if (IsSearchByDate)
                sorders = sorders.Where(i => (i.InvoiceDate >= fromDate && i.InvoiceDate <= toDate));


            var items = await (from so in sorders
                               join c in customers on so.CustomerID equals c.CustomerID
                               //join emp in employees on so.EmployeeID equals emp.EmployeeID
                               select new ProductWiseSalesReportModel
                               {
                                   SOrderID = so.SOrderID,
                                   InvoiceNo = so.InvoiceNo,
                                   Date = so.InvoiceDate,
                                   CustomerName = c.Name,
                                   //CustomerCode = c.Code,
                                   Mobile = c.ContactNo,
                                   TotalDue = c.TotalDue,
                                   Status = so.Status,
                                   IsReplacement = so.IsReplacement,
                                   IsApproved = so.IsApproved,
                                   EmployeeName = ""
                                   //TotalAmount = so.TotalAmount
                               }).ToListAsync();

            List<ProductWiseSalesReportModel> finalData = new List<ProductWiseSalesReportModel>();
            if (IsVATManager)
            {
                items = items.OrderByDescending(i => i.Date).ToList();
                var oConcern = SisterConcernRepository.All.FirstOrDefault(i => i.ConcernID == concernID);
                decimal FalesSales = (items.Sum(i => i.TotalAmount) * oConcern.SalesShowPercent) / 100m;
                decimal FalesSalesCount = 0m;


                foreach (var item in items)
                {
                    FalesSalesCount += item.TotalAmount;
                    if (FalesSalesCount <= FalesSales)
                        finalData.Add(item);
                    else
                        break;
                }
            }
            else
                finalData = items;

            return finalData.Select(x => new Tuple<int, string, DateTime, string, string, decimal, EnumSalesType, Tuple<string, bool, string>>
                (
                    x.SOrderID,
                    x.InvoiceNo,
                    x.Date,
                    x.CustomerName,
                    x.Mobile,
                    x.TotalDue,
                    (EnumSalesType)x.Status,
                    new Tuple<string, bool, string>
                    (x.CustomerCode, x.IsApproved, x.EmployeeName)
                )).OrderByDescending(x => x.Item3).ThenByDescending(i => i.Item2).ToList();
        }


        public static async Task<IEnumerable<Tuple<int, string, DateTime, string, string, decimal, EnumSalesType>>>
            GetAllSalesOrderAsync(this IBaseRepository<SOrder> salesOrderRepository,
                                 IBaseRepository<Customer> customerRepository, DateTime fromDate, DateTime toDate,
                                 EnumSalesType SalesType, int ConcernID)
        {
            IQueryable<Customer> customers = customerRepository.GetAll().Where(i => i.ConcernID == ConcernID);

            var items = await salesOrderRepository.GetAll().Where(i => i.ConcernID == ConcernID).
                GroupJoin(customers, p => p.CustomerID, c => c.CustomerID,
                (p, c) => new { SalesOrder = p, Customers = c }).
                SelectMany(x => x.Customers.DefaultIfEmpty(), (p, c) => new { SalesOrder = p.SalesOrder, Customer = c })
                .Where(x => (x.SalesOrder.InvoiceDate >= fromDate && x.SalesOrder.InvoiceDate <= toDate) && x.SalesOrder.Status == (int)SalesType)
                .Select(x => new
                {
                    x.SalesOrder.SOrderID,
                    x.SalesOrder.InvoiceNo,
                    x.SalesOrder.InvoiceDate,
                    x.Customer.Name,
                    x.Customer.ContactNo,
                    x.Customer.TotalDue,
                    x.SalesOrder.Status,
                    x.SalesOrder.IsReplacement
                }).Where(i => i.IsReplacement == 0).ToListAsync();

            return items.Select(x => new Tuple<int, string, DateTime, string, string, decimal, EnumSalesType>
                (
                    x.SOrderID,
                    x.InvoiceNo,
                    x.InvoiceDate,
                    x.Name,
                    x.ContactNo,
                    x.TotalDue,
                    (EnumSalesType)x.Status
                )).OrderByDescending(x => x.Item3).ThenByDescending(i => i.Item2).ToList();
        }
        public static async Task<IEnumerable<Tuple<int, string, DateTime, string,
            string, decimal, EnumSalesType, Tuple<string>>>> GetAllSalesOrderAsyncByUserID(this IBaseRepository<SOrder> salesOrderRepository,
            IBaseRepository<Customer> customerRepository, int UserID,
            DateTime fromDate, DateTime toDate, EnumSalesType SalesType,
            string InvoiceNo, string ContactNo, string CustomerName, string AccountNo)
        {
            IQueryable<Customer> customers = customerRepository.All;
            IQueryable<SOrder> sorders = salesOrderRepository.All
                                        .Where(x => x.Status == (int)SalesType && x.CreatedBy == UserID);

            bool IsSearchByDate = true;
            if (!string.IsNullOrWhiteSpace(InvoiceNo))
            {
                sorders = sorders.Where(i => i.InvoiceNo.Contains(InvoiceNo));
                IsSearchByDate = false;
            }
            if (!string.IsNullOrWhiteSpace(ContactNo))
            {
                customers = customers.Where(i => i.ContactNo.Contains(ContactNo));
                IsSearchByDate = false;
            }
            if (!string.IsNullOrWhiteSpace(CustomerName))
            {
                customers = customers.Where(i => i.Name.Contains(CustomerName));
                IsSearchByDate = false;
            }

            if (!string.IsNullOrWhiteSpace(AccountNo))
            {
                customers = customers.Where(i => i.Name.Contains(AccountNo));
                IsSearchByDate = false;
            }

            if (IsSearchByDate)
                sorders = sorders.Where(i => (i.InvoiceDate >= fromDate && i.InvoiceDate <= toDate));

            var items = await salesOrderRepository.All.
                GroupJoin(customers, p => p.CustomerID, c => c.CustomerID,
                (p, c) => new { SalesOrder = p, Customers = c }).
                SelectMany(x => x.Customers.DefaultIfEmpty(), (p, c) => new { SalesOrder = p.SalesOrder, Customer = c })
                .Select(x => new
                {
                    x.SalesOrder.SOrderID,
                    x.SalesOrder.InvoiceNo,
                    x.SalesOrder.InvoiceDate,
                    x.Customer.Code,
                    x.Customer.Name,
                    x.Customer.ContactNo,
                    x.Customer.TotalDue,
                    x.SalesOrder.Status,
                    x.SalesOrder.CreatedBy,
                    x.SalesOrder.IsReplacement
                }).Where(i => i.IsReplacement == 0).OrderByDescending(i => i.InvoiceDate).ToListAsync();

            return items.Select(x => new Tuple<int, string, DateTime, string, string, decimal, EnumSalesType, Tuple<string>>
                (
                    x.SOrderID,
                    x.InvoiceNo,
                    x.InvoiceDate,
                    x.Name,
                    x.ContactNo,
                    x.TotalDue,
                    (EnumSalesType)x.Status,
                   new Tuple<string>
                    (x.Code)

                )).ToList();
        }


        public static async Task<IEnumerable<Tuple<int, string, DateTime, string, string, decimal, EnumSalesType, Tuple<string, bool>>>> GetAllAdvanceSalesOrderAsyncByUserID(this IBaseRepository<SOrder> salesOrderRepository, IBaseRepository<Customer> customerRepository, IBaseRepository<Employee> EmployeeRepository, int UserID, DateTime fromDate, DateTime toDate, List<EnumSalesType> SalesType, string InvoiceNo, string ContactNo, string CustomerName, string AccountNo)
        {
            IQueryable<Customer> customers = customerRepository.All;
            IQueryable<Employee> employees = EmployeeRepository.All;
            IQueryable<SOrder> sorders = salesOrderRepository.All
                                        .Where(x => x.CreatedBy == UserID && x.IsReplacement == 0 && SalesType.Contains((EnumSalesType)x.Status) && x.IsAdvanceSale);

            bool IsSearchByDate = true;
            if (!string.IsNullOrWhiteSpace(InvoiceNo))
            {
                sorders = sorders.Where(i => i.InvoiceNo.Contains(InvoiceNo));
                IsSearchByDate = false;
            }
            if (!string.IsNullOrWhiteSpace(ContactNo))
            {
                customers = customers.Where(i => i.ContactNo.Contains(ContactNo));
                IsSearchByDate = false;
            }
            if (!string.IsNullOrWhiteSpace(CustomerName))
            {
                customers = customers.Where(i => i.Name.Contains(CustomerName));
                IsSearchByDate = false;
            }

            if (!string.IsNullOrWhiteSpace(AccountNo))
            {
                customers = customers.Where(i => i.Name.Contains(AccountNo));
                IsSearchByDate = false;
            }

            if (IsSearchByDate)
                sorders = sorders.Where(i => (i.InvoiceDate >= fromDate && i.InvoiceDate <= toDate));


            var items = await (from so in sorders
                               join c in customers on so.CustomerID equals c.CustomerID
                               join emp in employees on so.EmployeeID equals emp.EmployeeID
                               select new ProductWiseSalesReportModel
                               {
                                   SOrderID = so.SOrderID,
                                   InvoiceNo = so.InvoiceNo,
                                   Date = so.InvoiceDate,
                                   CustomerName = c.Name,
                                   //CustomerCode = c.Code,
                                   Mobile = c.ContactNo,
                                   TotalDue = c.TotalDue,
                                   Status = so.Status,
                                   IsReplacement = so.IsReplacement,
                                   IsApproved = so.IsApproved,
                                   EmployeeName = emp.Name
                                   //TotalAmount = so.TotalAmount
                               }).ToListAsync();

            //var items = await sorders.
            //    GroupJoin(customers, p => p.CustomerID, c => c.CustomerID,
            //    (p, c) => new { SalesOrder = p, Customers = c }).
            //    SelectMany(x => x.Customers.DefaultIfEmpty(), (p, c) => new { SalesOrder = p.SalesOrder, Customer = c })
            //    .Select(x => new
            //    {
            //        x.SalesOrder.SOrderID,
            //        x.SalesOrder.InvoiceNo,
            //        x.SalesOrder.InvoiceDate,
            //        x.Customer.Code,
            //        x.Customer.Name,
            //        x.Customer.ContactNo,
            //        x.Customer.TotalDue,
            //        x.SalesOrder.Status,
            //        x.SalesOrder.CreatedBy,
            //        x.SalesOrder.IsReplacement,
            //        x.SalesOrder.IsApproved
            //    }).Where(i => i.IsReplacement == 0).OrderByDescending(i => i.InvoiceDate).ToListAsync();

            return items.Select(x => new Tuple<int, string, DateTime, string, string, decimal, EnumSalesType, Tuple<string, bool>>
                (
                    x.SOrderID,
                    x.InvoiceNo,
                    x.Date,
                    x.CustomerName,
                    x.Mobile,
                    x.TotalDue,
                    (EnumSalesType)x.Status,
                   new Tuple<string, bool>
                    (
                       x.EmployeeName,
                       x.IsApproved
                    )

                )).ToList();
        }


        public static IEnumerable<SOredersReportModel> GetforSalesReport(
            this IBaseRepository<SOrder> salesOrderRepository, IBaseRepository<Customer> customerRepository,
            IBaseRepository<Employee> EmployeeRepository,
            DateTime fromDate, DateTime toDate, int EmployeeID, int CustomerID)
        {
            IQueryable<Customer> Customers = null;
            if (CustomerID > 0)
                Customers = customerRepository.All.Where(i => i.CustomerID == CustomerID);
            else if (EmployeeID > 0)
                Customers = customerRepository.All.Where(i => i.EmployeeID == EmployeeID);
            else
                Customers = customerRepository.All;

            var oSalesData = (from sord in salesOrderRepository.All
                              join cus in Customers on sord.CustomerID equals cus.CustomerID
                              join emp in EmployeeRepository.All on cus.EmployeeID equals emp.EmployeeID
                              where (sord.InvoiceDate >= fromDate && sord.InvoiceDate <= toDate) && sord.Status == (int)EnumSalesType.Sales && sord.RecAmount > 0m
                              select new SOredersReportModel
                              {
                                  CustomerCode = cus.Code,
                                  CustomerName = cus.Name,
                                  InvoiceDate = sord.InvoiceDate,
                                  InvoiceNo = sord.InvoiceNo,
                                  Grandtotal = sord.GrandTotal,
                                  FlatDiscount = sord.TDAmount,
                                  TotalAmount = sord.TotalAmount,
                                  RecAmount = (decimal)sord.RecAmount,
                                  PaymentDue = sord.PaymentDue,
                                  CustomerID = sord.CustomerID,
                                  CustomerTotalDue = cus.TotalDue,
                                  EmployeeName = emp.Name,
                              }).ToList();
            return oSalesData;
        }

        public static IEnumerable<Tuple<DateTime, string, string, decimal, decimal, decimal, decimal, Tuple<decimal, decimal, decimal, decimal, decimal, string>>>
            GetforSalesDetailReport(this IBaseRepository<SOrder> salesOrderRepository, IBaseRepository<SOrderDetail> SorderDetailRepository, IBaseRepository<Product> productRepository,
            IBaseRepository<StockDetail> stockdetailRepository, DateTime fromDate, DateTime toDate)
        {
            var oSalesDetailData = (from SOD in SorderDetailRepository.All
                                    from SO in salesOrderRepository.All
                                    from P in productRepository.All
                                    from std in stockdetailRepository.All
                                    where (SOD.SOrderID == SO.SOrderID && SOD.SDetailID == std.SDetailID && P.ProductID == SOD.ProductID && SO.InvoiceDate >= fromDate && SO.InvoiceDate <= toDate && SO.Status == 1)
                                    select new { SO.InvoiceNo, SO.InvoiceDate, SO.GrandTotal, SO.TDAmount, SO.TotalAmount, SO.RecAmount, SO.PaymentDue, P.ProductID, P.ProductName, SOD.UnitPrice, SOD.UTAmount, SOD.PPDAmount, SOD.Quantity, std.IMENO }).OrderByDescending(x => x.InvoiceDate).ToList();

            return oSalesDetailData.Select(x => new Tuple<DateTime, string, string, decimal, decimal, decimal, decimal, Tuple<decimal, decimal, decimal, decimal, decimal, string>>
                (
                 x.InvoiceDate,
                 x.InvoiceNo,
                x.ProductName,
                x.UnitPrice,
                x.PPDAmount,
                x.UTAmount,
                x.GrandTotal, new Tuple<decimal, decimal, decimal, decimal, decimal, string>(
                                    x.TDAmount,
                                    x.TotalAmount,
                                   (decimal)x.RecAmount,
                                   x.PaymentDue,
                                   x.Quantity,
                                   x.IMENO)
                ));
        }

        public static IEnumerable<SOredersReportModel>
         GetforSalesDetailReportByMO(this IBaseRepository<SOrder> salesOrderRepository, IBaseRepository<SOrderDetail> SorderDetailRepository, IBaseRepository<Product> productRepository,
         IBaseRepository<StockDetail> stockdetailRepository, IBaseRepository<Customer> customerRepository, IBaseRepository<Employee> employeeRepository, DateTime fromDate, DateTime toDate, int MOId)
        {
            var oSalesDetailData = (from SOD in SorderDetailRepository.All
                                    join SO in salesOrderRepository.All on SOD.SOrderID equals SO.SOrderID
                                    join P in productRepository.All on SOD.ProductID equals P.ProductID
                                    join std in stockdetailRepository.All on SOD.SDetailID equals std.SDetailID
                                    join CO in customerRepository.All on SO.CustomerID equals CO.CustomerID
                                    join emp in employeeRepository.All on CO.EmployeeID equals emp.EmployeeID
                                    //where (CO.CustomerID == SO.CustomerID && SOD.SOrderID == SO.SOrderID && SOD.SDetailID == std.SDetailID && P.ProductID == SOD.ProductID && SO.InvoiceDate >= fromDate && SO.InvoiceDate <= toDate && CO.EmployeeID == emp.EmployeeID && CO.EmployeeID == MOId && SO.Status ==(int) EnumSalesType.Sales)
                                    where (SO.InvoiceDate >= fromDate && SO.InvoiceDate <= toDate && CO.EmployeeID == MOId && SO.Status == (int)EnumSalesType.Sales && SO.IsReplacement != 1)
                                    select new SOredersReportModel
                                    {
                                        SOrderID = SO.SOrderID,
                                        EmployeeName = emp.Name,
                                        CustomerCode = CO.Code,
                                        CustomerName = CO.Name,
                                        InvoiceNo = SO.InvoiceNo,
                                        InvoiceDate = SO.InvoiceDate,
                                        TotalAmount = SO.TotalAmount,
                                        NetDiscount = SO.NetDiscount,
                                        AdjAmount = SO.AdjAmount,
                                        RecAmount = (decimal)SO.RecAmount,
                                        PaymentDue = SO.PaymentDue,
                                        ProductName = P.ProductName,
                                        UnitPrice = SOD.UnitPrice,
                                        PPDAmount = SOD.PPDAmount,
                                        Quantity = SOD.Quantity,
                                        IMENO = std.IMENO,
                                        Grandtotal = SO.GrandTotal,
                                        Trems = SO.Terms
                                    }).OrderByDescending(x => x.SOrderID).ToList();

            var Replacements = (from SO in salesOrderRepository.All
                                join SOD in SorderDetailRepository.All on SO.SOrderID equals SOD.RepOrderID
                                join P in productRepository.All on SOD.ProductID equals P.ProductID
                                join std in stockdetailRepository.All on SOD.RStockDetailID equals std.SDetailID
                                join CO in customerRepository.All on SO.CustomerID equals CO.CustomerID
                                join emp in employeeRepository.All on CO.EmployeeID equals emp.EmployeeID
                                //where (CO.CustomerID == SO.CustomerID && SOD.SOrderID == SO.SOrderID && SOD.SDetailID == std.SDetailID && P.ProductID == SOD.ProductID && SO.InvoiceDate >= fromDate && SO.InvoiceDate <= toDate && CO.EmployeeID == emp.EmployeeID && CO.EmployeeID == MOId && SO.Status ==(int) EnumSalesType.Sales)
                                where (SO.InvoiceDate >= fromDate && SO.InvoiceDate <= toDate && CO.EmployeeID == MOId && SO.Status == (int)EnumSalesType.Sales && SO.IsReplacement == 1)
                                select new SOredersReportModel
                                {
                                    SOrderID = SO.SOrderID,
                                    EmployeeName = emp.Name,
                                    CustomerCode = CO.Code,
                                    CustomerName = CO.Name,
                                    InvoiceNo = "REP-" + SO.InvoiceNo,
                                    InvoiceDate = SO.InvoiceDate,
                                    TotalAmount = SO.TotalAmount,
                                    NetDiscount = SO.NetDiscount,
                                    AdjAmount = SO.AdjAmount,
                                    RecAmount = (decimal)SO.RecAmount,
                                    PaymentDue = SO.PaymentDue,
                                    ProductName = P.ProductName,
                                    UnitPrice = (decimal)SOD.RepUnitPrice,
                                    PPDAmount = SOD.PPDAmount,
                                    Quantity = SOD.Quantity,
                                    IMENO = std.IMENO,
                                    Grandtotal = SO.GrandTotal
                                }).OrderByDescending(x => x.SOrderID).ToList();

            oSalesDetailData.AddRange(Replacements);
            return oSalesDetailData;

        }


        public static IEnumerable<Tuple<string, string, DateTime, string, decimal, decimal, decimal, Tuple<decimal, decimal, decimal, decimal>>>
            GetSalesReportByConcernID(this IBaseRepository<SOrder> salesOrderRepository, IBaseRepository<Customer> customerRepository,
            IBaseRepository<SOrderDetail> SOrderDetailRepsitory,
            DateTime fromDate, DateTime toDate, int concernID, int CustomerType)
        {
            IQueryable<Customer> Customers = null;
            if (CustomerType == 0)
                Customers = customerRepository.All;
            else
                Customers = customerRepository.All.Where(i => i.CustomerType == (EnumCustomerType)CustomerType);

            var oSalesData = (from SO in salesOrderRepository.All
                              join SOD in SOrderDetailRepsitory.All on SO.SOrderID equals SOD.SOrderID
                              join cus in Customers on SO.CustomerID equals cus.CustomerID
                              where (SO.InvoiceDate >= fromDate && SO.InvoiceDate <= toDate && SO.Status == (int)EnumSalesType.Sales && SO.IsReplacement != 1)
                              group SO by new
                              {
                                  cus.Code,
                                  cus.Name,
                                  SO.InvoiceNo,
                                  SO.InvoiceDate,
                                  SO.GrandTotal,
                                  SO.NetDiscount,
                                  SO.TotalAmount,
                                  SO.RecAmount,
                                  SO.PaymentDue,
                                  SO.AdjAmount,

                              } into g
                              select new
                              {
                                  g.Key.Code,
                                  g.Key.Name,
                                  g.Key.InvoiceDate,
                                  g.Key.InvoiceNo,
                                  g.Key.GrandTotal,
                                  g.Key.NetDiscount,
                                  g.Key.TotalAmount,
                                  g.Key.RecAmount,
                                  g.Key.PaymentDue,
                                  g.Key.AdjAmount,
                                  TotalOffer = g.Select(i => i.SOrderDetails).FirstOrDefault()
                              }).ToList();

            var Replacements = (from SO in salesOrderRepository.All
                                join SOD in SOrderDetailRepsitory.All on SO.SOrderID equals SOD.RepOrderID
                                join cus in Customers on SO.CustomerID equals cus.CustomerID
                                where (SO.InvoiceDate >= fromDate && SO.InvoiceDate <= toDate && SO.Status == (int)EnumSalesType.Sales && SO.IsReplacement == 1)
                                group SO by new
                                {
                                    cus.Code,
                                    cus.Name,
                                    InvoiceNo = "REP-" + SO.InvoiceNo,
                                    SO.InvoiceDate,
                                    SO.GrandTotal,
                                    SO.NetDiscount,
                                    SO.TotalAmount,
                                    SO.RecAmount,
                                    SO.PaymentDue,
                                    SO.AdjAmount,

                                } into g
                                select new
                                {
                                    g.Key.Code,
                                    g.Key.Name,
                                    g.Key.InvoiceDate,
                                    g.Key.InvoiceNo,
                                    g.Key.GrandTotal,
                                    g.Key.NetDiscount,
                                    g.Key.TotalAmount,
                                    g.Key.RecAmount,
                                    g.Key.PaymentDue,
                                    g.Key.AdjAmount,
                                    TotalOffer = g.Select(i => i.SOrderDetails).FirstOrDefault()
                                }).ToList();

            oSalesData.AddRange(Replacements);

            return oSalesData.Select(x => new Tuple<string, string, DateTime, string, decimal, decimal, decimal, Tuple<decimal, decimal, decimal, decimal>>
                (
                 x.Code,
                 x.Name,
                x.InvoiceDate,
                                    x.InvoiceNo,
                                    x.GrandTotal,
                                    x.NetDiscount,
                                    x.TotalAmount,
                                     new Tuple<decimal, decimal, decimal, decimal>(
                                    (decimal)x.RecAmount,
                                    x.PaymentDue,
                                    x.AdjAmount,
                                    x.TotalOffer.Sum(i => i.PPOffer)
                                    )

                ));
        }

        public static IEnumerable<ProductWiseSalesReportModel> GetSalesDetailReportByConcernID(this IBaseRepository<SOrder> salesOrderRepository,
            IBaseRepository<SOrderDetail> SorderDetailRepository, IBaseRepository<Product> productRepository,
            IBaseRepository<Size> sizeRepository, IBaseRepository<ProductUnitType> productUnitTypeRepository,
            IBaseRepository<StockDetail> stockdetailRepository, IBaseRepository<Category> categoryRepository, DateTime fromDate, DateTime toDate, int concernID)
        {
            var oSalesDetailData = (from SO in salesOrderRepository.All
                                    join SOD in SorderDetailRepository.All on SO.SOrderID equals SOD.SOrderID
                                    join STD in stockdetailRepository.All on SOD.SDetailID equals STD.SDetailID
                                    join P in productRepository.All on SOD.ProductID equals P.ProductID
                                    join CAT in categoryRepository.All on P.CategoryID equals CAT.CategoryID
                                    join PU in productUnitTypeRepository.All on (int)P.ProUnitTypeID equals PU.ProUnitTypeID
                                    join SZ in sizeRepository.All on P.SizeID equals SZ.SizeID
                                    join std in stockdetailRepository.All on SOD.SDetailID equals std.SDetailID
                                    where (SO.InvoiceDate >= fromDate && SO.InvoiceDate <= toDate && SO.Status == (int)EnumSalesType.Sales && SO.IsReplacement != 1)
                                    select new ProductWiseSalesReportModel
                                    {
                                        SOrderID = SO.SOrderID,
                                        InvoiceNo = SO.InvoiceNo,
                                        CustomerName = SO.Customer.Name,
                                        Date = SO.InvoiceDate,
                                        GrandTotal = SO.GrandTotal,
                                        NetDiscount = SO.NetDiscount,
                                        TotalAmount = SO.TotalAmount,
                                        RecAmount = (decimal)SO.RecAmount,
                                        PaymentDue = SO.PaymentDue,
                                        ProductID = P.ProductID,
                                        ProductName = P.ProductName,
                                        UnitPrice = SOD.SFTRate == 0 ? SOD.UnitPrice / PU.ConvertValue : SOD.SFTRate,
                                        UTAmount = SOD.UTAmount,
                                        PPDAmount = SOD.PPDAmount,
                                        PPOffer = SOD.PPOffer,
                                        Quantity = SOD.Quantity,
                                        IMEI = std.IMENO,
                                        ColorName = std.Color.Name,
                                        AdjAmount = SO.AdjAmount,
                                        UnitName = PU.Description,
                                        ProductCode = P.Code,
                                        IDCode = P.IDCode,
                                        SizeName = SZ.Description,
                                        CategoryName = CAT.Description,
                                        ConvertValue = P.BundleQty != 0 ? P.BundleQty : PU.ConvertValue,
                                        SalesPerCartonSft = P.SalesCSft,
                                        ExtraAmt = SOD.FractionAmt,
                                        ExtraSFT = SOD.TotalSFT - SOD.ActualSFT,
                                        PurchaseRate = STD.PRate,
                                        PurchaseSFTRate = STD.SFTRate






                                    }).OrderBy(x => x.SOrderID).ToList();

            var Replacements = (from SO in salesOrderRepository.All
                                join SOD in SorderDetailRepository.All on SO.SOrderID equals SOD.RepOrderID
                                join P in productRepository.All on SOD.ProductID equals P.ProductID
                                join PU in productUnitTypeRepository.All on (int)P.ProUnitTypeID equals PU.ProUnitTypeID
                                join SZ in sizeRepository.All on P.SizeID equals SZ.SizeID
                                join std in stockdetailRepository.All on SOD.RStockDetailID equals std.SDetailID
                                where (SO.InvoiceDate >= fromDate && SO.InvoiceDate <= toDate && SO.Status == (int)EnumSalesType.Sales && SO.IsReplacement == 1)
                                select new ProductWiseSalesReportModel
                                {
                                    SOrderID = SO.SOrderID,
                                    InvoiceNo = SO.InvoiceNo,
                                    CustomerName = SO.Customer.Name,
                                    Date = SO.InvoiceDate,
                                    GrandTotal = SO.GrandTotal,
                                    NetDiscount = SO.NetDiscount,
                                    TotalAmount = SO.TotalAmount,
                                    RecAmount = (decimal)SO.RecAmount,
                                    PaymentDue = SO.PaymentDue,
                                    ProductID = P.ProductID,
                                    ProductName = P.ProductName,
                                    UnitPrice = SOD.UnitPrice,
                                    UTAmount = SOD.UTAmount,
                                    PPDAmount = SOD.PPDAmount,
                                    PPOffer = SOD.PPOffer,
                                    Quantity = SOD.Quantity,
                                    IMEI = std.IMENO,
                                    ColorName = std.Color.Name,
                                    AdjAmount = SO.AdjAmount,
                                    UnitName = PU.Description,
                                    ProductCode = string.IsNullOrEmpty(P.IDCode) ? P.Code : P.IDCode,
                                    SizeName = SZ.Code,
                                    ConvertValue = P.BundleQty != 0 ? P.BundleQty : PU.ConvertValue,
                                    SalesPerCartonSft = P.SalesCSft,

                                }).OrderBy(x => x.SOrderID).ToList();

            oSalesDetailData.AddRange(Replacements);

            return oSalesDetailData;
        }

        public static IEnumerable<ProductWiseSalesReportModel> GetSalesDetailReportAdminByConcernID(this IBaseRepository<SOrder> salesOrderRepository,
           IBaseRepository<SOrderDetail> SorderDetailRepository, IBaseRepository<Product> productRepository,
           IBaseRepository<Size> sizeRepository, IBaseRepository<ProductUnitType> productUnitTypeRepository,
           IBaseRepository<StockDetail> stockdetailRepository, IBaseRepository<Category> categoryRepository, IBaseRepository<SisterConcern> sisterConcernRepository, IBaseRepository<Customer> customerRepository, DateTime fromDate, DateTime toDate, int concernID, int CustomerType)
        {
            var SisterConcerns = sisterConcernRepository.GetAll();
            if (concernID != 0)
                SisterConcerns = SisterConcerns.Where(o => o.ConcernID == concernID);
            var Customers = customerRepository.GetAll();
            if (CustomerType == 1)
                Customers = Customers.Where(o => o.CustomerType == EnumCustomerType.Retail);
            else if (CustomerType == 2)
                Customers = Customers.Where(o => o.CustomerType == EnumCustomerType.Dealer);

            var oSalesDetailData = (from SO in salesOrderRepository.GetAll()

                                    join SIS in SisterConcerns on SO.ConcernID equals SIS.ConcernID
                                    join CUS in Customers on SO.CustomerID equals CUS.CustomerID
                                    join SOD in SorderDetailRepository.GetAll() on SO.SOrderID equals SOD.SOrderID
                                    join P in productRepository.GetAll() on SOD.ProductID equals P.ProductID
                                    join CAT in categoryRepository.GetAll() on P.CategoryID equals CAT.CategoryID
                                    join PU in productUnitTypeRepository.GetAll() on (int)P.ProUnitTypeID equals PU.ProUnitTypeID
                                    join SZ in sizeRepository.GetAll() on P.SizeID equals SZ.SizeID
                                    join std in stockdetailRepository.GetAll() on SOD.SDetailID equals std.SDetailID

                                    where (SO.InvoiceDate >= fromDate && SO.InvoiceDate <= toDate && SO.Status == (int)EnumSalesType.Sales && SO.IsReplacement != 1)
                                    select new ProductWiseSalesReportModel
                                    {
                                        SOrderID = SO.SOrderID,
                                        InvoiceNo = SO.InvoiceNo,
                                        CustomerName = SO.Customer.Name,
                                        Date = SO.InvoiceDate,
                                        GrandTotal = SO.GrandTotal,
                                        NetDiscount = SO.NetDiscount,
                                        TotalAmount = SO.TotalAmount,
                                        RecAmount = (decimal)SO.RecAmount,
                                        PaymentDue = SO.PaymentDue,
                                        ProductID = P.ProductID,
                                        ProductName = P.ProductName,
                                        UnitPrice = SOD.SFTRate == 0 ? SOD.UnitPrice / PU.ConvertValue : SOD.SFTRate,
                                        UTAmount = SOD.UTAmount,
                                        PPDAmount = SOD.PPDAmount,
                                        PPOffer = SOD.PPOffer,
                                        Quantity = SOD.Quantity,
                                        IMEI = std.IMENO,
                                        ColorName = std.Color.Name,
                                        AdjAmount = SO.AdjAmount,
                                        UnitName = PU.Description,
                                        ProductCode = P.Code,
                                        IDCode = P.IDCode,
                                        SizeName = SZ.Description,
                                        CategoryName = CAT.Description,
                                        ConvertValue = P.BundleQty != 0 ? P.BundleQty : PU.ConvertValue,
                                        SalesPerCartonSft = P.SalesCSft,
                                        ConcernName = SIS.Name
                                    }).OrderBy(x => x.SOrderID).ToList();

            var Replacements = (from SO in salesOrderRepository.GetAll()
                                join SIS in SisterConcerns on SO.ConcernID equals SIS.ConcernID
                                join SOD in SorderDetailRepository.GetAll() on SO.SOrderID equals SOD.RepOrderID
                                join P in productRepository.GetAll() on SOD.ProductID equals P.ProductID
                                join PU in productUnitTypeRepository.GetAll() on (int)P.ProUnitTypeID equals PU.ProUnitTypeID
                                join SZ in sizeRepository.GetAll() on P.SizeID equals SZ.SizeID
                                join std in stockdetailRepository.GetAll() on SOD.RStockDetailID equals std.SDetailID
                                where (SO.InvoiceDate >= fromDate && SO.InvoiceDate <= toDate && SO.Status == (int)EnumSalesType.Sales && SO.IsReplacement == 1)
                                select new ProductWiseSalesReportModel
                                {
                                    SOrderID = SO.SOrderID,
                                    InvoiceNo = SO.InvoiceNo,
                                    CustomerName = SO.Customer.Name,
                                    Date = SO.InvoiceDate,
                                    GrandTotal = SO.GrandTotal,
                                    NetDiscount = SO.NetDiscount,
                                    TotalAmount = SO.TotalAmount,
                                    RecAmount = (decimal)SO.RecAmount,
                                    PaymentDue = SO.PaymentDue,
                                    ProductID = P.ProductID,
                                    ProductName = P.ProductName,
                                    UnitPrice = SOD.UnitPrice,
                                    UTAmount = SOD.UTAmount,
                                    PPDAmount = SOD.PPDAmount,
                                    PPOffer = SOD.PPOffer,
                                    Quantity = SOD.Quantity,
                                    IMEI = std.IMENO,
                                    ColorName = std.Color.Name,
                                    AdjAmount = SO.AdjAmount,
                                    UnitName = PU.Description,
                                    ProductCode = string.IsNullOrEmpty(P.IDCode) ? P.Code : P.IDCode,
                                    SizeName = SZ.Code,
                                    ConvertValue = P.BundleQty != 0 ? P.BundleQty : PU.ConvertValue,
                                    SalesPerCartonSft = P.SalesCSft,
                                    ConcernName = SIS.Name
                                }).OrderBy(x => x.SOrderID).ToList();

            oSalesDetailData.AddRange(Replacements);

            return oSalesDetailData;
        }
        public static IEnumerable<SOredersReportModel> GetSalesDetailReportByCustomerID(
                                                            this IBaseRepository<SOrder> salesOrderRepository, IBaseRepository<SOrderDetail> SorderDetailRepository,
                                                            IBaseRepository<Product> productRepository, IBaseRepository<StockDetail> stockdetailRepository,
                                                            IBaseRepository<Color> ColorRepository,
                                                            DateTime fromDate, DateTime toDate, int customerID
            )
        {
            var oSalesDetailData = (from SO in salesOrderRepository.All
                                    join SOD in SorderDetailRepository.All on SO.SOrderID equals SOD.SOrderID
                                    join P in productRepository.All on SOD.ProductID equals P.ProductID
                                    join std in stockdetailRepository.All on SOD.SDetailID equals std.SDetailID
                                    join col in ColorRepository.All on std.ColorID equals col.ColorID
                                    where (SO.InvoiceDate >= fromDate && SO.InvoiceDate <= toDate && SO.Status == (int)EnumSalesType.Sales && SO.CustomerID == customerID && SO.IsReplacement != 1)
                                    select new SOredersReportModel
                                    {
                                        CustomerID = SO.CustomerID,
                                        CustomerName = SO.Customer.Name,
                                        CustomerCode = SO.Customer.Code,
                                        CustomerAddress = SO.Customer.Address,
                                        CustomerContactNo = SO.Customer.ContactNo,
                                        CustCompanyName = SO.Customer.CompanyName,
                                        CustomerTotalDue = SO.Customer.TotalDue,
                                        SOrderID = SO.SOrderID,
                                        InvoiceNo = SO.InvoiceNo,
                                        InvoiceDate = SO.InvoiceDate,
                                        Grandtotal = SO.GrandTotal,
                                        FlatDiscount = SO.TDAmount,
                                        TotalAmount = SO.TotalAmount,
                                        NetDiscount = SO.NetDiscount,
                                        RecAmount = (decimal)SO.RecAmount,
                                        PaymentDue = SO.PaymentDue,
                                        AdjAmount = SO.AdjAmount,
                                        ProductID = P.ProductID,
                                        ProductName = P.ProductName,
                                        UnitPrice = SOD.UnitPrice - SOD.PPDAmount,
                                        UTAmount = SOD.UTAmount,
                                        PPDAmount = SOD.PPDAmount,
                                        PPDPercentage = SOD.PPDPercentage,
                                        Quantity = SOD.Quantity,
                                        IMENO = std.IMENO,
                                        ColorName = col.Name,
                                        CustomerType = SO.Customer.CustomerType,
                                        CustomerNID = SO.Customer.NID
                                    }).OrderByDescending(x => x.SOrderID).ToList();
            var ReplacementOrders = (from SO in salesOrderRepository.All
                                     join SOD in SorderDetailRepository.All on SO.SOrderID equals SOD.RepOrderID
                                     join P in productRepository.All on SOD.ProductID equals P.ProductID
                                     join std in stockdetailRepository.All on SOD.RStockDetailID equals std.SDetailID
                                     join col in ColorRepository.All on std.ColorID equals col.ColorID
                                     where (SO.InvoiceDate >= fromDate && SO.InvoiceDate <= toDate && SO.Status == (int)EnumSalesType.Sales && SO.CustomerID == customerID && SO.IsReplacement == 1)
                                     select new SOredersReportModel
                                     {
                                         CustomerID = SO.CustomerID,
                                         CustomerName = SO.Customer.Name,
                                         CustomerCode = SO.Customer.Code,
                                         CustomerAddress = SO.Customer.Address,
                                         CustomerContactNo = SO.Customer.ContactNo,
                                         CustCompanyName = SO.Customer.CompanyName,
                                         CustomerTotalDue = SO.Customer.TotalDue,
                                         SOrderID = SO.SOrderID,
                                         InvoiceNo = SO.InvoiceNo,
                                         InvoiceDate = SO.InvoiceDate,
                                         Grandtotal = SO.GrandTotal,
                                         FlatDiscount = SO.TDAmount,
                                         TotalAmount = SO.TotalAmount,
                                         NetDiscount = SO.NetDiscount,
                                         RecAmount = (decimal)SO.RecAmount,
                                         PaymentDue = SO.PaymentDue,
                                         AdjAmount = SO.AdjAmount,
                                         ProductID = P.ProductID,
                                         ProductName = P.ProductName,
                                         UnitPrice = SOD.UnitPrice - SOD.PPDAmount,
                                         UTAmount = SOD.UTAmount,
                                         PPDAmount = SOD.PPDAmount,
                                         PPDPercentage = SOD.PPDPercentage,
                                         Quantity = SOD.Quantity,
                                         IMENO = std.IMENO,
                                         ColorName = col.Name,
                                         CustomerType = SO.Customer.CustomerType,
                                         CustomerNID = SO.Customer.NID
                                     }).OrderByDescending(x => x.SOrderID).ToList();

            oSalesDetailData.AddRange(ReplacementOrders);

            return oSalesDetailData;
        }

        public static IEnumerable<Tuple<string, DateTime, string, string, decimal, decimal, Tuple<decimal, decimal, decimal, decimal>>>
        GetSalesDetailReportByMOID(this IBaseRepository<SOrder> salesOrderRepository, IBaseRepository<Customer> customerRepository, IBaseRepository<Employee> employeeRepository,
        DateTime fromDate, DateTime toDate, int concernID, int MOID, int RptType)
        {
            //var oMOWiseSalesDetailData = (dynamic)null;

            if (RptType == 1)
            {
                var oAllMOWiseSalesDetailData = (from CO in customerRepository.All
                                                 from SO in salesOrderRepository.All
                                                 from Emp in employeeRepository.All
                                                 where (CO.CustomerID == SO.CustomerID && SO.Status == 1 && SO.EmployeeID == Emp.EmployeeID && (SO.InvoiceDate >= fromDate && SO.InvoiceDate <= toDate))
                                                 select new
                                                 {
                                                     Emp.Name,
                                                     SO.InvoiceDate,
                                                     CusName = CO.Name,
                                                     SO.InvoiceNo,
                                                     SO.GrandTotal,
                                                     SO.NetDiscount,
                                                     SO.TotalAmount,
                                                     SO.RecAmount,
                                                     SO.PaymentDue,
                                                     SO.AdjAmount
                                                 }).OrderByDescending(x => x.InvoiceDate).ToList();



                return oAllMOWiseSalesDetailData.Select(x => new Tuple<string, DateTime, string, string, decimal, decimal, Tuple<decimal, decimal, decimal, decimal>>
                    (
                     x.Name,
                     x.InvoiceDate,
                     x.CusName,
                     x.InvoiceNo,
                     x.GrandTotal,
                     x.NetDiscount, new Tuple<decimal, decimal, decimal, decimal>(
                                        x.TotalAmount,
                                       (decimal)x.RecAmount,
                                       x.PaymentDue,
                                       x.AdjAmount
                                       )
                    ));
            }
            else
            {
                var oMOWiseSalesDetailData = (from CO in customerRepository.All
                                              from SO in salesOrderRepository.All
                                              from Emp in employeeRepository.All
                                              where (CO.CustomerID == SO.CustomerID && SO.Status == 1 && CO.EmployeeID == Emp.EmployeeID && Emp.EmployeeID == MOID && (SO.InvoiceDate >= fromDate && SO.InvoiceDate <= toDate))
                                              select new { Emp.Name, SO.InvoiceDate, CusName = CO.Name, SO.InvoiceNo, SO.GrandTotal, SO.NetDiscount, SO.TotalAmount, SO.RecAmount, SO.PaymentDue, SO.AdjAmount }).OrderByDescending(x => x.InvoiceDate).ToList();



                return oMOWiseSalesDetailData.Select(x => new Tuple<string, DateTime, string, string, decimal, decimal, Tuple<decimal, decimal, decimal, decimal>>
                    (
                     x.Name,
                     x.InvoiceDate,
                     x.CusName,
                     x.InvoiceNo,
                     x.GrandTotal,
                     x.NetDiscount, new Tuple<decimal, decimal, decimal, decimal>(
                                        x.TotalAmount,
                                       (decimal)x.RecAmount,
                                       x.PaymentDue,
                                       x.AdjAmount)
                    ));
            }


        }

        public static IEnumerable<Tuple<string, string, string, string, string, decimal, decimal>>
        GetMOWiseCustomerDueRpt(this IBaseRepository<SOrder> salesOrderRepository,
            IBaseRepository<Customer> customerRepository,
            IBaseRepository<Employee> employeeRepository,
            IBaseRepository<EmployeeWiseCustomerDue> employeeWiseRepository,
            int concernID, int MOID, int RptType)
        {
            IQueryable<Employee> employees = null;
            if (MOID > 0)
                employees = employeeRepository.All.Where(i => i.EmployeeID == MOID);
            else
                employees = employeeRepository.All;

            var oAllMOWiseCustomerDue = (from CO in customerRepository.All
                                             //join ewc in employeeWiseRepository.All on CO.CustomerID equals ewc.CustomerID
                                             //where ewc.CustomerDue > 0
                                         join Emp in employees on CO.EmployeeID equals Emp.EmployeeID
                                         where (CO.TotalDue > 0 || CO.TotalDue < 0)

                                         select new
                                         {
                                             EmpName = Emp.Name,
                                             CusCode = CO.Code,
                                             CusName = CO.Name,
                                             CusContact = CO.ContactNo,
                                             Address = CO.Address,
                                             TotalDue = CO.TotalDue, //CO.TotalDue + CO.CreditDue,

                                             SupplierDue = 0
                                         }).OrderBy(x => x.CusCode).ToList();



            return oAllMOWiseCustomerDue.Select(x => new Tuple<string, string, string, string, string, decimal, decimal>
                (
                 x.EmpName,
                 x.CusCode,
                 x.CusName,
                 x.CusContact,
                 x.Address,
                 x.TotalDue,

                 x.SupplierDue
                ));


        }


        public static IEnumerable<Tuple<int, int, int, int, string, string, string,
            Tuple<decimal, decimal, decimal, decimal, decimal, decimal, int, Tuple<string, decimal, decimal>>>>
            GetSalesOrderDetailByOrderId(this IBaseRepository<SOrderDetail> salesOrderDetailRepository, int orderId, IBaseRepository<Product> productRepository,
            IBaseRepository<Color> colorRepository, IBaseRepository<StockDetail> stockDetailRepository, IBaseRepository<ProductUnitType> ProductUnitTypeRepo)
        {
            IQueryable<Product> products = productRepository.All;
            IQueryable<Color> colors = colorRepository.All;
            IQueryable<StockDetail> details = stockDetailRepository.All;

            var items = (from sod in salesOrderDetailRepository.All
                         join p in productRepository.All on sod.ProductID equals p.ProductID
                         join pu in ProductUnitTypeRepo.All on p.ProUnitTypeID equals pu.ProUnitTypeID
                         join sd in stockDetailRepository.All on sod.SDetailID equals sd.SDetailID
                         join c in colorRepository.All on sd.ColorID equals c.ColorID
                         where sod.SOrderID == orderId
                         select new
                         {
                             sod.SOrderDetailID,
                             sod.SOrderID,
                             sod.ProductID,
                             StockDetailID = sod.SDetailID,
                             ProductName = p.ProductName,
                             p.Code,
                             sd.IMENO,
                             sod.Quantity,
                             sod.UnitPrice,
                             sod.MPRate,
                             sod.UTAmount,
                             sod.PPDPercentage,
                             sod.PPDAmount,
                             ColorId = sd.ColorID,
                             ColorName = c.Name,
                             ConvertValue = p.BundleQty == 0 ? pu.ConvertValue : p.BundleQty,
                         }

            ).ToList();

            return items.Select(x => new Tuple<int, int, int, int, string, string, string,
                Tuple<decimal, decimal, decimal, decimal, decimal, decimal, int, Tuple<string, decimal, decimal>>>
                (
                    x.SOrderDetailID,
                    x.SOrderID,
                    x.ProductID,
                    x.StockDetailID,
                    x.ProductName,
                    x.Code,
                    x.IMENO,
                    new Tuple<decimal, decimal, decimal, decimal, decimal, decimal, int, Tuple<string, decimal, decimal>>(
                    x.Quantity,
                    x.UnitPrice,
                    x.MPRate,
                    x.UTAmount,
                    x.PPDPercentage,
                    x.PPDAmount,
                    x.ColorId,
                    new Tuple<string, decimal, decimal>(
                        x.ColorName,
                        x.ConvertValue,
                        0
                        ))
                    ));
        }

        public static IEnumerable<Tuple<int, int, int, int, string, string, string,
        Tuple<decimal, decimal, decimal, decimal, decimal, decimal, int, Tuple<string, decimal, string, string, string, string>>>>
        GetSalesOrderDetailByOrderIdForInvoice(this IBaseRepository<SOrderDetail> salesOrderDetailRepository, int orderId, IBaseRepository<Product> productRepository,
        IBaseRepository<Color> colorRepository, IBaseRepository<StockDetail> stockDetailRepository)
        {
            IQueryable<Product> products = productRepository.All;
            IQueryable<Color> colors = colorRepository.All;
            IQueryable<StockDetail> details = stockDetailRepository.All;

            var items = salesOrderDetailRepository.All.
                GroupJoin(products, s => s.ProductID, p => p.ProductID,
                (s, p) => new { SalesOrder = s, Products = p }).
                SelectMany(x => x.Products.DefaultIfEmpty(), (s, p) => new { SalesOrder = s.SalesOrder, Products = p }).
                GroupJoin(details, s => s.SalesOrder.SDetailID, d => d.SDetailID,
                (s, d) => new { SalesOrder = s.SalesOrder, Products = s.Products, Details = d }).
                SelectMany(x => x.Details.DefaultIfEmpty(), (s, d) => new { SalesOrder = s.SalesOrder, Products = s.Products, Details = d }).
                GroupJoin(colors, s => s.Details.ColorID, c => c.ColorID,
                (d, c) => new { SalesOrder = d.SalesOrder, Details = d.Details, Products = d.Products, Colors = c }).
                SelectMany(x => x.Colors.DefaultIfEmpty(), (d, c) => new { SalesOrder = d.SalesOrder, Products = d.Products, Details = d.Details, Color = c }).
                Where(x => x.SalesOrder.SOrderID == orderId).
                Select(x => new
                {
                    x.SalesOrder.SOrderDetailID,
                    x.SalesOrder.SOrderID,
                    x.SalesOrder.ProductID,
                    StockDetailID = x.SalesOrder.SDetailID,
                    x.Products.ProductName,
                    x.Products.Code,
                    x.Details.IMENO,
                    x.SalesOrder.Quantity,
                    x.SalesOrder.UnitPrice,
                    x.SalesOrder.MPRate,
                    x.SalesOrder.UTAmount,
                    x.SalesOrder.PPDPercentage,
                    x.SalesOrder.PPDAmount,
                    ColorId = x.Color.ColorID,
                    ColorName = x.Color.Name,
                    PPOffer = x.SalesOrder.PPOffer,
                    x.Products.CompressorWarrentyMonth,
                    x.Products.MotorWarrentyMonth,
                    x.Products.PanelWarrentyMonth,
                    x.Products.SparePartsWarrentyMonth
                }).ToList();

            return items.Select(x => new Tuple<int, int, int, int, string, string, string,
                Tuple<decimal, decimal, decimal, decimal, decimal, decimal, int, Tuple<string, decimal, string, string, string, string>>>
                (
                    x.SOrderDetailID,
                    x.SOrderID,
                    x.ProductID,
                    x.StockDetailID,
                    x.ProductName,
                    x.Code,
                    x.IMENO,
                    new Tuple<decimal, decimal, decimal, decimal, decimal, decimal, int, Tuple<string, decimal, string, string, string, string>>(
                    x.Quantity,
                    x.UnitPrice,
                    x.MPRate,
                    x.UTAmount,
                    x.PPDPercentage,
                    x.PPDAmount,
                    x.ColorId,
                    new Tuple<string, decimal, string, string, string, string>(
                        x.ColorName,
                        x.PPOffer,
                        x.CompressorWarrentyMonth,
                        x.MotorWarrentyMonth,
                        x.PanelWarrentyMonth,
                        x.SparePartsWarrentyMonth
                        ))
                    ));
        }

        public static IEnumerable<Tuple<DateTime, string, string, decimal, decimal>>
            GetSalesByProductID(this IBaseRepository<SOrder> salesOrderRepository, IBaseRepository<SOrderDetail> SorderDetailRepository, IBaseRepository<Product> productRepository,
            DateTime fromDate, DateTime toDate, int productID)
        {
            var oSalesDetailData = (from SOD in SorderDetailRepository.All
                                    from SO in salesOrderRepository.All
                                    from P in productRepository.All
                                    where (SOD.SOrderID == SO.SOrderID && P.ProductID == SOD.ProductID && SO.InvoiceDate >= fromDate && SO.InvoiceDate <= toDate && SO.Status == 1 && SOD.ProductID == productID)
                                    select new { SO.InvoiceNo, SO.InvoiceDate, SO.GrandTotal, SO.TDAmount, SO.TotalAmount, SO.RecAmount, SO.PaymentDue, P.ProductID, P.ProductName, SOD.UnitPrice, SOD.UTAmount, SOD.PPDAmount, SOD.Quantity }).OrderByDescending(x => x.InvoiceDate).ToList();

            return oSalesDetailData.Select(x => new Tuple<DateTime, string, string, decimal, decimal>
                (
                 x.InvoiceDate,
                 x.InvoiceNo,
                x.ProductName,
                x.UnitPrice,
                x.Quantity
                ));
        }

        public static IEnumerable<Tuple<DateTime, string, string, decimal, decimal>> GetSalesByProductID(this IBaseRepository<SOrder> SOrderRepository, IBaseRepository<SOrderDetail> SOrderDetailRepository, IBaseRepository<Product> productRepository,
        IBaseRepository<CreditSale> CreditSalesRepository, IBaseRepository<CreditSaleDetails> CreditSalesDetailRepository, DateTime fromDate, DateTime toDate, int ConcernID, int productid)
        {
            //var oSales = ((from POD in SOrderDetailRepository.All
            //               from PO in SOrderRepository.All
            //               from P in productRepository.All
            //               where (POD.SOrderID == PO.SOrderID && P.ProductID == POD.ProductID && PO.InvoiceDate >= fromDate && PO.InvoiceDate <= toDate && P.ProductID == productid && PO.Status == 1)
            //               select new {POD.StockDetailID, PO.InvoiceNo, SalesDate = PO.InvoiceDate, P.ProductName, POD.Quantity, POD.UnitPrice })
            //                 .Union(
            //                              from SOD in CreditSalesDetailRepository.All
            //                              from SO in CreditSalesRepository.All
            //                              from P in productRepository.All
            //                              where SOD.CreditSalesID == SO.CreditSalesID && P.ProductID == SOD.ProductID
            //                              && P.ProductID == productid && SO.SalesDate >= fromDate && SO.SalesDate <= toDate
            //                              select new
            //                              {
            //                                  SOD.StockDetailID,
            //                                  InvoiceNo = SO.InvoiceNo + " (Credit)",
            //                                  SO.SalesDate,
            //                                  P.ProductName,
            //                                  SOD.Quantity,
            //                                  SOD.UnitPrice
            //                              })).OrderBy(x => x.SalesDate).ToList();

            //return oSales.Select(x => new Tuple<DateTime, string, string, decimal, decimal>
            //    (
            //     x.SalesDate,
            //     x.InvoiceNo,
            //    x.ProductName,
            //    x.Quantity,
            //    x.UnitPrice
            //    ));


            var oSales = ((from POD in SOrderDetailRepository.All
                           from PO in SOrderRepository.All
                           from P in productRepository.All
                           where (POD.SOrderID == PO.SOrderID && P.ProductID == POD.ProductID && PO.InvoiceDate >= fromDate && PO.InvoiceDate <= toDate && P.ProductID == productid && PO.Status == 1)
                           group POD by new { PO.InvoiceNo, PO.InvoiceDate, P.ProductName, POD.UnitPrice } into g
                           select new { g.Key.InvoiceNo, SalesDate = g.Key.InvoiceDate, g.Key.ProductName, Quantity = g.Sum(x => x.Quantity), g.Key.UnitPrice })
                                         .Union(
                                                      from SOD in CreditSalesDetailRepository.All
                                                      from SO in CreditSalesRepository.All
                                                      from P in productRepository.All
                                                      where SOD.CreditSalesID == SO.CreditSalesID && P.ProductID == SOD.ProductID
                                                      && P.ProductID == productid && SO.SalesDate >= fromDate && SO.SalesDate <= toDate
                                                      group SOD by new { SO.InvoiceNo, SO.SalesDate, P.ProductName, SOD.UnitPrice } into g
                                                      select new
                                                      {
                                                          InvoiceNo = g.Key.InvoiceNo + " (Credit)",
                                                          g.Key.SalesDate,
                                                          g.Key.ProductName,
                                                          Quantity = g.Sum(x => x.Quantity),
                                                          g.Key.UnitPrice
                                                      })).OrderBy(x => x.SalesDate).ToList();

            return oSales.Select(x => new Tuple<DateTime, string, string, decimal, decimal>
                (
                 x.SalesDate,
                 x.InvoiceNo,
                x.ProductName,
                x.Quantity,
                x.UnitPrice
                ));
        }

        /// <summary>
        /// Author:Aminul
        /// Date: 06/03/2018
        /// </summary>
        /// <param name="SOrderRepository"></param>
        /// <param name="SOrderDetailRepo"></param>
        /// <param name="CustomerRepo"></param>
        /// <param name="CustomerID"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<Tuple<int, string, DateTime, string,
            string, decimal, EnumSalesType>>> GetReplacementOrdersAsync(this IBaseRepository<SOrder> SOrderRepository, IBaseRepository<SOrderDetail> SOrderDetailRepo, IBaseRepository<Customer> CustomerRepo, int EmployeeID)
        {
            var SOrders = SOrderRepository.All;
            //var SOrderDetails = SOrderDetailRepo.All;
            IQueryable<Customer> CustomerList = null;
            if (EmployeeID != 0)
                CustomerList = CustomerRepo.All.Where(i => i.EmployeeID == EmployeeID);
            else
                CustomerList = CustomerRepo.All;
            var result = await (from so in SOrders.Where(i => i.IsReplacement == 1)
                                join cus in CustomerList on so.CustomerID equals cus.CustomerID
                                select new
                                {
                                    so.SOrderID,
                                    so.InvoiceNo,
                                    SalesDate = so.InvoiceDate,
                                    CustomerName = cus.Name,
                                    cus.ContactNo,
                                    cus.TotalDue,
                                    so.Status
                                }).OrderByDescending(s => s.SOrderID).ToListAsync();


            return result.Select(x => new Tuple<int, string, DateTime, string, string, decimal, EnumSalesType>
                (
                    x.SOrderID,
                    x.InvoiceNo,
                    x.SalesDate,
                    x.CustomerName,
                    x.ContactNo,
                    x.TotalDue,
                    (EnumSalesType)x.Status
                )).ToList();
        }

        public static async Task<IEnumerable<Tuple<int, string, DateTime, string,
    string, decimal, EnumSalesType>>> GetReturnOrdersAsync(this IBaseRepository<SOrder> SOrderRepository, IBaseRepository<SOrderDetail> SOrderDetailRepo, IBaseRepository<Customer> CustomerRepo)
        {
            var SOrders = SOrderRepository.All;
            //var SOrderDetails = SOrderDetailRepo.All;
            var Customers = CustomerRepo.All;

            var result = await (from so in SOrders
                                where so.Status == (int)EnumSalesType.ProductReturn
                                join cus in Customers on so.CustomerID equals cus.CustomerID
                                select new
                                {
                                    so.SOrderID,
                                    so.InvoiceNo,
                                    SalesDate = so.InvoiceDate,
                                    CustomerName = cus.Name,
                                    cus.ContactNo,
                                    cus.TotalDue,
                                    so.Status
                                }).OrderByDescending(s => s.SOrderID).ToListAsync();


            return result.Select(x => new Tuple<int, string, DateTime, string, string, decimal, EnumSalesType>
                (
                    x.SOrderID,
                    x.InvoiceNo,
                    x.SalesDate,
                    x.CustomerName,
                    x.ContactNo,
                    x.TotalDue,
                    (EnumSalesType)x.Status
                )).ToList();
        }

        public static List<ReplaceOrderDetail> GetReplaceOrderInvoiceReportByID(this IBaseRepository<SOrder> SOrderRepository, IBaseRepository<SOrderDetail> SOrderDetailRepo, IBaseRepository<StockDetail> StockDetailRepo, IBaseRepository<Product> ProductRepo, int OrderID)
        {
            List<ReplaceOrderDetail> list = new List<ReplaceOrderDetail>();
            var dbsorder = SOrderRepository.FindBy(i => i.SOrderID == OrderID);
            var sorderDetails = SOrderDetailRepo.All;
            var stockdetails = StockDetailRepo.All;
            var products = ProductRepo.All;

            var dresult = (from so in dbsorder
                           join sod in sorderDetails on so.SOrderID equals sod.RepOrderID
                           join std in stockdetails on sod.SDetailID equals std.SDetailID
                           join p in products on std.ProductID equals p.ProductID
                           select new
                           {
                               SOrderDetailID = sod.SOrderDetailID,
                               DamageIMEINO = std.IMENO,
                               DamageProductName = p.ProductName,
                               DamageUnitPrice = sod.UnitPrice.ToString(),
                               Quantity = 1,
                               Remarks = sod.Remarks
                           }).ToList();

            var rresult = (from so in dbsorder
                           join sod in sorderDetails on so.SOrderID equals sod.RepOrderID
                           join std in stockdetails on sod.RStockDetailID equals std.SDetailID
                           join p in products on std.ProductID equals p.ProductID
                           select new
                           {
                               SOrderDetailID = sod.SOrderDetailID,
                               ReplaceIMEINO = std.IMENO,
                               ProductName = p.ProductName,
                               UnitPrice = sod.RepUnitPrice,
                               Quantity = 1,
                               Remarks = sod.Remarks
                           }).ToList();

            var result = (from d in dresult
                          join r in rresult on d.SOrderDetailID equals r.SOrderDetailID
                          select new ReplaceOrderDetail
                          {
                              DamageProductName = d.DamageProductName,
                              DamageIMEINO = d.DamageIMEINO,
                              DamageUnitPrice = d.DamageUnitPrice,
                              Quantity = d.Quantity,
                              ProductName = r.ProductName,
                              ReplaceIMEINO = r.ReplaceIMEINO,
                              UnitPrice = (decimal)r.UnitPrice,
                              Remarks = r.Remarks
                          }).ToList();
            return result;

        }

        public static List<ReplaceOrderDetail> GetReturnOrderInvoiceReportByID(this IBaseRepository<SOrder> SOrderRepository, IBaseRepository<SOrderDetail> SOrderDetailRepo, IBaseRepository<StockDetail> StockDetailRepo, IBaseRepository<Product> ProductRepo, int OrderID)
        {
            List<ReplaceOrderDetail> list = new List<ReplaceOrderDetail>();
            var dbsorder = SOrderRepository.FindBy(i => i.SOrderID == OrderID);
            var sorderDetails = SOrderDetailRepo.All;
            var stockdetails = StockDetailRepo.All;
            var products = ProductRepo.All;

            var dresult = (from so in dbsorder
                           join sod in sorderDetails on so.SOrderID equals sod.SOrderID
                           join std in stockdetails on sod.SDetailID equals std.SDetailID
                           join p in products on std.ProductID equals p.ProductID
                           select new ReplaceOrderDetail
                           {
                               SOrderDetailID = sod.SOrderDetailID,
                               DamageIMEINO = std.IMENO,
                               DamageProductName = p.ProductName,
                               UnitPrice = sod.UnitPrice,
                               Quantity = 1,
                               MPRate = sod.MPRate
                           }).ToList();

            //var result = (from d in dresult
            //              select new ReplaceOrderDetail
            //              {
            //                  DamageProductName = d.DamageProductName,
            //                  DamageIMEINO = d.DamageIMEINO,
            //                  DamageUnitPrice = d.DamageUnitPrice,
            //                  Quantity = d.Quantity,
            //                  //ProductName = r.ProductName,
            //                  //ReplaceIMEINO = r.ReplaceIMEINO,
            //                  //UnitPrice = (decimal)r.UnitPrice
            //              }).ToList();
            return dresult;

        }


        public static List<ProductWiseSalesReportModel> ProductWiseSalesReport(this IBaseRepository<SOrder> SOrderRepository, IBaseRepository<SOrderDetail> SOrderDetailRepo, IBaseRepository<Customer> CustomerRepository, IBaseRepository<Employee> EmployeeRepository, IBaseRepository<Product> ProductRepository, int ConcernID, int CustomerID, DateTime fromDate, DateTime toDate)
        {
            List<SOrder> SOrders = new List<SOrder>();
            if (CustomerID != 0)
                SOrders = SOrderRepository.All.Where(i => i.CustomerID == CustomerID && i.InvoiceDate >= fromDate && i.InvoiceDate <= toDate && i.ConcernID == ConcernID).ToList();
            else
                SOrders = SOrderRepository.All.Where(i => i.InvoiceDate >= fromDate && i.InvoiceDate <= toDate && i.ConcernID == ConcernID).ToList();

            var SOrderDetails = SOrderDetailRepo.All;
            var Products = ProductRepository.All;
            var Customers = CustomerRepository.All;
            var Employees = EmployeeRepository.All;


            var result = from SO in SOrders.Where(i => i.Status == (int)EnumSalesType.Sales)
                         join SOD in SOrderDetails on SO.SOrderID equals SOD.SOrderID
                         join P in Products on SOD.ProductID equals P.ProductID
                         join C in Customers on SO.CustomerID equals C.CustomerID
                         join E in Employees on C.EmployeeID equals E.EmployeeID
                         select new ProductWiseSalesReportModel
                         {
                             Date = SO.InvoiceDate,
                             EmployeeCode = E.Code,
                             EmployeeName = E.Name,
                             CustomerCode = C.Code,
                             CustomerName = C.Name,
                             Address = C.Address,
                             Mobile = C.ContactNo,
                             ProductName = P.ProductName,
                             Quantity = SOD.Quantity,
                             SalesRate = SOD.UnitPrice - SOD.PPDAmount - SOD.PPOffer,
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

        public static List<ProductWiseSalesReportModel> ProductWiseSalesDetailsReport(this IBaseRepository<SOrder> SOrderRepository,
            IBaseRepository<SOrderDetail> SOrderDetailRepo, IBaseRepository<Company> CompanyRepository,
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

            var SOrderDetails = SOrderDetailRepo.All;
            var SOrders = SOrderRepository.All.Where(i => i.InvoiceDate >= fromDate && i.InvoiceDate <= toDate && i.Status == (int)EnumSalesType.Sales);

            var result = from SO in SOrders
                         join SOD in SOrderDetails on SO.SOrderID equals SOD.SOrderID
                         join STD in StockDetailRepository.All on SOD.SDetailID equals STD.SDetailID
                         join P in Products on SOD.ProductID equals P.ProductID
                         join COM in CompanyRepository.All on P.CompanyID equals COM.CompanyID
                         join CAT in CategoryRepository.All on P.CategoryID equals CAT.CategoryID
                         select new ProductWiseSalesReportModel
                         {
                             Date = SO.InvoiceDate,
                             InvoiceNo = SO.InvoiceNo,
                             ProductID = P.ProductID,
                             CategoryID = CAT.CategoryID,
                             CompanyID = COM.CompanyID,
                             ProductName = P.ProductName,
                             CategoryName = CAT.Description,
                             CompanyName = COM.Name,
                             Quantity = SOD.Quantity,
                             SalesRate = SOD.UnitPrice - SOD.PPDAmount,
                             TotalAmount = SOD.UTAmount,
                             IMEI = STD.IMENO
                         };

            return result.ToList();
        }

        public static decimal GetAllCollectionAmountByDateRange(this IBaseRepository<SOrder> SOrderRepository,
            IBaseRepository<CreditSale> CreditSaleRepository, IBaseRepository<CreditSalesSchedule> CreditSalesScheduleRepository,
            IBaseRepository<CashCollection> CashCollectionRepository, IBaseRepository<BankTransaction> BankTransactionRepository, DateTime fromDate, DateTime toDate)
        {
            decimal TotalCollection = 0m;

            var CashSales = SOrderRepository.All.Where(so => so.InvoiceDate >= fromDate && so.InvoiceDate <= toDate && so.Status == (int)EnumSalesType.Sales).ToList();
            if (CashSales.Count() > 0)
                TotalCollection += (decimal)CashSales.Sum(i => i.RecAmount);

            var Downpayment = CreditSaleRepository.All.Where(so => so.SalesDate >= fromDate && so.SalesDate <= toDate && so.IsStatus == EnumSalesType.Sales).ToList();
            if (Downpayment.Count() > 0)
                TotalCollection += (decimal)Downpayment.Sum(i => i.DownPayment);

            var InstallmentCollections = from so in CreditSaleRepository.All
                                         join css in CreditSalesScheduleRepository.All on so.CreditSalesID equals css.CreditSalesID
                                         where ((css.PaymentDate >= fromDate && css.PaymentDate <= toDate) && css.PaymentStatus.Equals("Paid") && so.IsStatus == EnumSalesType.Sales)
                                         select css;

            if (InstallmentCollections.ToList().Count() > 0)
                TotalCollection += (decimal)InstallmentCollections.Sum(i => i.InstallmentAmt);

            var CashCollections = CashCollectionRepository.All.Where(so => so.EntryDate >= fromDate && so.EntryDate <= toDate && so.TransactionType == EnumTranType.FromCustomer).ToList();
            if (CashCollections.Count() > 0)
                TotalCollection += (decimal)CashCollections.Sum(i => i.Amount);

            var BankCollections = BankTransactionRepository.All.Where(i => i.TranDate >= fromDate && i.TranDate <= toDate && i.CustomerID != 0).ToList();
            if (BankCollections.Count() > 0)
                TotalCollection += (decimal)BankCollections.Sum(i => i.Amount);

            return TotalCollection;
        }

        public static decimal GetVoltageStabilizerCommission(this IBaseRepository<SOrder> SOrderRepository, IBaseRepository<SOrderDetail> SOrderDetailRepository,
             IBaseRepository<CreditSale> CreditSaleRepository, IBaseRepository<CreditSaleDetails> CreditSaleDetailsRepository,
             IBaseRepository<Product> ProductRepository, IBaseRepository<ExtraCommissionSetup> ExtraCommissionSetupRepository,
             DateTime fromDate, DateTime toDate)
        {
            decimal TotalVSComm = 0m;
            var TargetCategory = ExtraCommissionSetupRepository.All.FirstOrDefault(i => i.Status == EnumCommissionType.VoltageStabilizerComm);
            var Sales = (from so in SOrderRepository.All
                         join sod in SOrderDetailRepository.All on so.SOrderID equals sod.SOrderID
                         join p in ProductRepository.All on sod.ProductID equals p.ProductID
                         where (so.Status == (int)EnumSalesType.Sales && so.InvoiceDate >= fromDate && so.InvoiceDate <= toDate) && (p.CategoryID == TargetCategory.CategoryID1 || p.CategoryID == TargetCategory.CategoryID2)
                         select new
                         {
                             so.InvoiceDate,
                             so.CustomerID,
                             sod.ProductID,
                             p.CategoryID
                         }).ToList();

            var CreditSales = (from so in CreditSaleRepository.All
                               join sod in CreditSaleDetailsRepository.All on so.CreditSalesID equals sod.CreditSalesID
                               join p in ProductRepository.All on sod.ProductID equals p.ProductID
                               where (so.IsStatus == EnumSalesType.Sales && so.SalesDate >= fromDate && so.SalesDate <= toDate) && (p.CategoryID == TargetCategory.CategoryID1 || p.CategoryID == TargetCategory.CategoryID2)
                               select new
                               {
                                   InvoiceDate = so.SalesDate,
                                   so.CustomerID,
                                   sod.ProductID,
                                   p.CategoryID
                               }).ToList();

            Sales.AddRange(CreditSales);

            var SalesVoltageStabilizerComm = (from so in Sales
                                              group so by new { so.InvoiceDate, so.CustomerID } into g
                                              select new
                                              {
                                                  InvoiceDate = g.Key.InvoiceDate,
                                                  CustomerID = g.Key.CustomerID,
                                                  Categories = g.Select(i => i.CategoryID).ToList()
                                              }).ToList();

            int Flag1 = 0, Flag2 = 0, Counter = 0;
            foreach (var item in SalesVoltageStabilizerComm)
            {
                if (item.Categories.Any(i => i == TargetCategory.CategoryID1))
                    Flag1++;
                if (item.Categories.Any(i => i == TargetCategory.CategoryID2))
                    Flag2++;
                if (Flag1 > 0 && Flag2 > 0)
                {
                    Counter++;
                }
                Flag1 = 0;
                Flag2 = 0;
            }

            TotalVSComm = 250m * Counter;

            return TotalVSComm;
        }

        /// <summary>
        /// Date: 25-02-2019
        /// Author: aminul
        /// </summary>
        public static decimal GetExtraCommission(this IBaseRepository<SOrder> SOrderRepository, IBaseRepository<SOrderDetail> SOrderDetailRepository,
                         IBaseRepository<CreditSale> CreditSaleRepository, IBaseRepository<CreditSaleDetails> CreditSaleDetailsRepository,
                         IBaseRepository<Product> ProductRepository, IBaseRepository<ExtraCommissionSetup> ExtraCommissionSetupRepository,
                         DateTime fromDate, DateTime toDate, int ConcernID)
        {
            decimal TotalExtraComm = 0m;

            if (ConcernID == (int)EnumSisterConcern.SAMSUNG_ELECTRA_CONCERNID)
            {
                var TargetCategory = ExtraCommissionSetupRepository.All.FirstOrDefault(i => i.Status == EnumCommissionType.ExtraComm);
                var Sales = (from so in SOrderRepository.All
                             join sod in SOrderDetailRepository.All on so.SOrderID equals sod.SOrderID
                             join p in ProductRepository.All on sod.ProductID equals p.ProductID
                             where (so.Status == (int)EnumSalesType.Sales && so.InvoiceDate >= fromDate && so.InvoiceDate <= toDate)
                             && ((p.CategoryID == TargetCategory.CategoryID1 && p.CompanyID == TargetCategory.CompanyID) || (p.CategoryID == TargetCategory.CategoryID2 && p.CompanyID == TargetCategory.CompanyID))
                             //&& sod.PPDAmount <= 250
                             select new
                             {
                                 so.InvoiceDate,
                                 so.CustomerID,
                                 sod.ProductID,
                                 p.CategoryID,
                                 sod.PPDAmount,
                                 sod.PPDPercentage,
                                 sod.UnitPrice,
                                 sod.Quantity,
                                 sod.UTAmount
                             }).ToList();
                decimal TotalSalesAmt = Sales.Sum(i => i.Quantity) * 1000m;
                decimal AcceptedAmount = (TotalSalesAmt * 25m) / 100m;
                decimal TotalGivenDiscount = Sales.Sum(i => (i.PPDAmount * i.Quantity));
                if (TotalGivenDiscount <= AcceptedAmount)
                    TotalExtraComm = 250m * Sales.Count();
            }
            else if (ConcernID == (int)EnumSisterConcern.HAWRA_ENTERPRISE_CONCERNID || ConcernID == (int)EnumSisterConcern.HAVEN_ENTERPRISE_CONCERNID)
            {
                var Creditsales6M = CreditSaleRepository.All.Where(i => i.InstallmentPeriod.ToLower().Equals("6 months")
                    && (i.SalesDate >= fromDate && i.SalesDate <= toDate && i.IsStatus == EnumSalesType.Sales)).ToList();
                if (Creditsales6M.Count > 0)
                    TotalExtraComm = Creditsales6M.Sum(i => i.NetAmount) * .0025m;

                var Creditsales12M = CreditSaleRepository.All.Where(i => i.InstallmentPeriod.ToLower().Equals("12 months")
                    && (i.SalesDate >= fromDate && i.SalesDate <= toDate && i.IsStatus == EnumSalesType.Sales)).ToList();
                if (Creditsales12M.Count() > 0)
                    TotalExtraComm += Creditsales12M.Sum(i => i.NetAmount) * .0050m;
            }


            return TotalExtraComm;
        }
        public static bool IsIMEIAlreadyReplaced(this IBaseRepository<SOrder> SOrderRepository,
         IBaseRepository<SOrderDetail> SOrderDetailRepo, int StockDetailID)
        {
            var RepORders = from so in SOrderRepository.All
                            join sod in SOrderDetailRepo.All on so.SOrderID equals sod.RepOrderID
                            where sod.RStockDetailID == StockDetailID && so.Status == (int)EnumSalesType.Sales && so.IsReplacement == 1
                            select sod;

            if (RepORders.Count() > 0)
                return true;
            else
                return false;
        }

        public static List<SOredersReportModel> GetAdminSalesReport(this IBaseRepository<SOrder> SOrderRepository, IBaseRepository<SOrderDetail> SOrderDetailRepository,
            IBaseRepository<Customer> CustomerRepository, IBaseRepository<SisterConcern> SisterConcernRepository,
            int ConcernID, DateTime fromDate, DateTime toDate)
        {
            IQueryable<Customer> Customers = null;
            if (ConcernID != 0)
                Customers = CustomerRepository.GetAll().Where(i => i.ConcernID == ConcernID);
            else
                Customers = CustomerRepository.GetAll();
            var Sales = (from so in SOrderRepository.GetAll()
                         join c in Customers on so.CustomerID equals c.CustomerID
                         join s in SisterConcernRepository.GetAll() on so.ConcernID equals s.ConcernID
                         where so.Status == (int)EnumSalesType.Sales && (so.InvoiceDate >= fromDate && so.InvoiceDate <= toDate) && so.IsReplacement != 1
                         select new SOredersReportModel
                         {
                             ConcernID = so.ConcernID,
                             ConcernName = s.Name,
                             CustomerCode = c.Code,
                             CustomerName = c.Name,
                             InvoiceDate = so.InvoiceDate,
                             InvoiceNo = so.InvoiceNo,
                             Grandtotal = so.GrandTotal,
                             NetDiscount = so.NetDiscount,
                             TotalOffer = 0,
                             AdjAmount = so.AdjAmount,
                             TotalAmount = so.TotalAmount,
                             RecAmount = (decimal)so.RecAmount,
                             PaymentDue = so.PaymentDue,
                             CustomerTotalDue = c.TotalDue
                         }).ToList();

            var Replaces = (from so in SOrderRepository.GetAll()
                            join c in Customers on so.CustomerID equals c.CustomerID
                            join s in SisterConcernRepository.GetAll() on so.ConcernID equals s.ConcernID
                            where so.Status == (int)EnumSalesType.Sales && (so.InvoiceDate >= fromDate && so.InvoiceDate <= toDate) && so.IsReplacement == 1
                            select new SOredersReportModel
                            {
                                ConcernID = so.ConcernID,
                                ConcernName = s.Name,
                                CustomerCode = c.Code,
                                CustomerName = c.Name,
                                InvoiceDate = so.InvoiceDate,
                                InvoiceNo = "REP-" + so.InvoiceNo,
                                Grandtotal = so.GrandTotal,
                                NetDiscount = so.NetDiscount,
                                TotalOffer = 0,
                                AdjAmount = so.AdjAmount,
                                TotalAmount = so.TotalAmount,
                                RecAmount = (decimal)so.RecAmount,
                                PaymentDue = so.PaymentDue,
                                CustomerTotalDue = c.TotalDue
                            }).ToList();
            Sales.AddRange(Replaces);
            return Sales.OrderBy(i => i.ConcernID).ThenByDescending(i => i.InvoiceDate).ToList();
        }


        public static List<LedgerAccountReportModel> CustomerLedger(this IBaseRepository<SOrder> SOrderRepository, IBaseRepository<SOrderDetail> SOrderDetailRepository,
                                                               IBaseRepository<Customer> CustomerRepository, IBaseRepository<ApplicationUser> UserRepository,
                                                               IBaseRepository<BankTransaction> BankTransactionRepository,
                                                               IBaseRepository<CashCollection> CashCollectionRepository, IBaseRepository<CreditSale> CreditSaleRepository,
                                                               IBaseRepository<CreditSaleDetails> CreditSaleDetailsRepo, IBaseRepository<CreditSalesSchedule> CreditSalesScheduleRepo, IBaseRepository<ROrder> RorderRepository, IBaseRepository<ROrderDetail> rOrderDetail,
                                                               IBaseRepository<Product> ProductRepository, IBaseRepository<Bank> BankRepository,
                                                               int CustomerID, DateTime fromDate, DateTime toDate)
        {

            List<LedgerAccountReportModel> ledgers = new List<LedgerAccountReportModel>();
            List<LedgerAccountReportModel> FinalLedgers = new List<LedgerAccountReportModel>();

            var Customer = CustomerRepository.GetAll().FirstOrDefault(i => i.CustomerID == CustomerID);

            #region Cash Sales
            var CashSales = from so in SOrderRepository.All
                            join sod in SOrderDetailRepository.All on so.SOrderID equals sod.SOrderID
                            join p in ProductRepository.All on sod.ProductID equals p.ProductID
                            join u in UserRepository.All on so.CreatedBy equals u.Id into lj
                            from u in lj.DefaultIfEmpty()
                            where so.Status == (int)EnumSalesType.Sales && so.CustomerID == CustomerID
                            select new
                            {
                                so.TotalAmount,
                                so.InvoiceDate,
                                so.InvoiceNo,
                                so.RecAmount,
                                CreditAdj = so.AdjAmount + so.NetDiscount,
                                Credit = (decimal)so.RecAmount,
                                CashCollectionAmt = (decimal)so.RecAmount,
                                Debit = (decimal)(so.TotalAmount),
                                GrandTotal = so.GrandTotal,
                                sod.UnitPrice,
                                sod.UTAmount,
                                sod.Quantity,
                                ProductName = p.ProductName + " " + sod.Quantity.ToString() + " " + p.ProUnitTypeID.ToString() + " " + sod.SRate.ToString() + " " + sod.UTAmount.ToString(),
                                EnteredBy = u == null ? string.Empty : u.UserName,
                                Remarks = so.Remarks,
                                terms = so.Terms
                            };

            var VmCashSales = (from cs in CashSales
                               group cs by new { cs.Debit, cs.Credit, cs.CreditAdj, cs.GrandTotal, cs.CashCollectionAmt, cs.InvoiceDate, cs.InvoiceNo, cs.EnteredBy } into g
                               select new LedgerAccountReportModel
                               {
                                   VoucherType = "Sales",
                                   InvoiceNo = g.Key.InvoiceNo,
                                   Date = g.Key.InvoiceDate,
                                   EnteredBy = "Entered By: " + g.Key.EnteredBy,
                                   ProductList = g.Select(i => i.ProductName).ToList(),
                                   Debit = g.Key.Debit,
                                   Credit = g.Key.Credit,
                                   CreditAdj = g.Key.CreditAdj,
                                   GrandTotal = g.Key.GrandTotal,
                                   CashCollectionAmt = g.Key.CashCollectionAmt,
                                   Quantity = g.Sum(i => i.Quantity),
                                   Balance = 0,
                                   Remarks = g.Select(i => i.Remarks).FirstOrDefault(),
                                   Terms = g.Select(i => i.terms).FirstOrDefault()
                               }).ToList();

            ledgers.AddRange(VmCashSales);
            #endregion

            #region Cash Sales Return
            var CashSalesReturn = from so in SOrderRepository.All
                                  join sod in SOrderDetailRepository.All on so.SOrderID equals sod.SOrderID
                                  join p in ProductRepository.All on sod.ProductID equals p.ProductID
                                  join u in UserRepository.All on so.CreatedBy equals u.Id into lj
                                  from u in lj.DefaultIfEmpty()
                                  where so.Status == (int)EnumSalesType.ProductReturn && so.CustomerID == CustomerID
                                  select new
                                  {
                                      so.TotalAmount,
                                      so.InvoiceDate,
                                      so.InvoiceNo,
                                      so.RecAmount,
                                      so.AdjAmount,
                                      Credit = (decimal)(so.TotalAmount),
                                      Debit = (decimal)(so.RecAmount),
                                      Return = (decimal)(so.TotalAmount - so.RecAmount),
                                      sod.UnitPrice,
                                      sod.UTAmount,
                                      sod.Quantity,
                                      ProductName = p.ProductName + " " + sod.Quantity.ToString() + " " + p.ProUnitTypeID.ToString() + " " + sod.SRate + " " + sod.UTAmount,
                                      EnteredBy = u == null ? string.Empty : u.UserName,
                                      Remarks = so.Remarks,
                                  };

            var VmCashSalesReturn = (from cs in CashSalesReturn
                                     group cs by new { cs.Debit, cs.Credit, cs.Return, cs.InvoiceDate, cs.InvoiceNo, cs.EnteredBy } into g
                                     select new LedgerAccountReportModel
                                     {
                                         VoucherType = "Sales Return",
                                         InvoiceNo = g.Key.InvoiceNo,
                                         Date = g.Key.InvoiceDate,
                                         EnteredBy = "Entered By: " + g.Key.EnteredBy,
                                         ProductList = g.Select(i => i.ProductName).ToList(),
                                         Debit = g.Key.Debit,
                                         Credit = g.Key.Credit,
                                         SalesReturn = g.Key.Return,
                                         Quantity = g.Sum(i => i.Quantity),
                                         Balance = 0,
                                         Remarks = g.Select(i => i.Remarks).FirstOrDefault()
                                     }).ToList();

            ledgers.AddRange(VmCashSalesReturn);
            #endregion

            #region Cash Sales Return ROrders
            var CashSalesProductReturn = from so in RorderRepository.All
                                         join sod in rOrderDetail.All on so.ROrderID equals sod.ROrderID
                                         join p in ProductRepository.All on sod.ProductID equals p.ProductID
                                         join u in UserRepository.All on so.CreatedBy equals u.Id into lj
                                         from u in lj.DefaultIfEmpty()
                                         where so.CustomerID == CustomerID
                                         //so.Status == (int)EnumSalesType.ProductReturn && 
                                         select new
                                         {
                                             TotalAmount = so.GrandTotal,
                                             InvoiceDate = so.ReturnDate,
                                             so.InvoiceNo,
                                             RecAmount = so.PaidAmount,
                                             AdjAmount = 0m,
                                             Credit = (decimal)(so.GrandTotal),
                                             Debit = (decimal)(so.PaidAmount),
                                             Return = (decimal)(so.GrandTotal - so.PaidAmount),
                                             sod.UnitPrice,
                                             sod.UTAmount,
                                             sod.Quantity,
                                             ProductName = p.ProductName + " " + sod.Quantity.ToString() + " " + p.ProUnitTypeID.ToString() + " " + sod.UnitPrice + " " + sod.UTAmount,
                                             EnteredBy = u == null ? string.Empty : u.UserName,
                                             Remarks = so.Remarks,
                                         };

            var VmCashSalesProductReturn = (from cs in CashSalesProductReturn
                                            group cs by new { cs.Debit, cs.Credit, cs.Return, cs.InvoiceDate, cs.InvoiceNo, cs.EnteredBy } into g
                                            select new LedgerAccountReportModel
                                            {
                                                VoucherType = "Sales Return",
                                                InvoiceNo = g.Key.InvoiceNo,
                                                Date = g.Key.InvoiceDate,
                                                EnteredBy = "Entered By: " + g.Key.EnteredBy,
                                                ProductList = g.Select(i => i.ProductName).ToList(),
                                                Debit = g.Key.Debit,
                                                Credit = g.Key.Credit,
                                                SalesReturn = g.Key.Return,
                                                Quantity = g.Sum(i => i.Quantity),
                                                Balance = 0,
                                                Remarks = g.Select(i => i.Remarks).FirstOrDefault()
                                            }).ToList();

            ledgers.AddRange(VmCashSalesProductReturn);
            #endregion

            #region Credit Sales
            var CreditSales = from so in CreditSaleRepository.All
                              join sod in CreditSaleDetailsRepo.All on so.CreditSalesID equals sod.CreditSalesID
                              join p in ProductRepository.All on sod.ProductID equals p.ProductID
                              join u in UserRepository.All on so.CreatedBy equals u.Id into lj
                              from u in lj.DefaultIfEmpty()
                              where so.IsStatus == EnumSalesType.Sales && so.CustomerID == CustomerID
                              select new
                              {
                                  so.NetAmount,
                                  so.SalesDate,
                                  so.InvoiceNo,
                                  CreditAdj = so.Discount,
                                  Credit = so.DownPayment,
                                  CashCollectionAmt = so.DownPayment,
                                  Debit = so.NetAmount,
                                  GrandTotal = so.TSalesAmt,
                                  sod.UnitPrice,
                                  sod.UTAmount,
                                  sod.Quantity,
                                  ProductName = p.ProductName + " " + sod.Quantity.ToString() + " " + p.ProUnitTypeID.ToString() + " " + sod.UnitPrice + " " + sod.UTAmount,
                                  EnteredBy = u == null ? string.Empty : u.UserName,
                                  Remarks = so.Remarks,
                              };

            var VmCreditSales = (from cs in CreditSales
                                 group cs by new { cs.Debit, cs.Credit, cs.GrandTotal, cs.CashCollectionAmt, cs.SalesDate, cs.InvoiceNo, cs.EnteredBy } into g
                                 select new LedgerAccountReportModel
                                 {
                                     VoucherType = "Hire Sales",
                                     InvoiceNo = g.Key.InvoiceNo,
                                     Date = g.Key.SalesDate,
                                     EnteredBy = "Entered By: " + g.Key.EnteredBy,
                                     ProductList = g.Select(i => i.ProductName).ToList(),
                                     Debit = g.Key.Debit,
                                     Credit = g.Key.Credit,
                                     CashCollectionAmt = g.Key.CashCollectionAmt,
                                     GrandTotal = g.Key.GrandTotal,
                                     Quantity = g.Sum(i => i.Quantity),
                                     Balance = 0,
                                     Remarks = g.Select(i => i.Remarks).FirstOrDefault()
                                 }).ToList();

            ledgers.AddRange(VmCreditSales);
            #endregion

            #region Installment Collection
            var CreditSchedule = from so in CreditSaleRepository.All
                                 join sod in CreditSalesScheduleRepo.All on so.CreditSalesID equals sod.CreditSalesID
                                 where so.IsStatus == EnumSalesType.Sales && sod.PaymentStatus == "Paid" && so.CustomerID == CustomerID && sod.InstallmentAmt != 0
                                 select new LedgerAccountReportModel
                                 {
                                     VoucherType = "Installment",
                                     InvoiceNo = so.InvoiceNo + "-" + sod.ScheduleNo,
                                     Date = sod.PaymentDate,
                                     Debit = 0m,
                                     Quantity = 0m,
                                     Credit = sod.InstallmentAmt + 0,//sod.LastPayAdjust,
                                     CashCollectionAmt = sod.InstallmentAmt,
                                     CreditAdj = 0,//sod.LastPayAdjust,
                                     Balance = 0,
                                     Remarks = sod.Remarks
                                 };
            ledgers.AddRange(CreditSchedule);
            #endregion

            #region Cash Collection
            var CashCollection = from cc in CashCollectionRepository.All
                                 join u in UserRepository.All on cc.CreatedBy equals u.Id into lj
                                 from u in lj.DefaultIfEmpty()
                                 where cc.CustomerID == CustomerID && cc.TransactionType == EnumTranType.FromCustomer
                                 select new LedgerAccountReportModel
                                 {
                                     Date = (DateTime)cc.EntryDate,
                                     Debit = cc.InterestAmt,
                                     VoucherType = "Cash Collection",
                                     Credit = cc.Amount + cc.AdjustAmt + cc.CashBAmt + cc.YearlyBnsAmt,
                                     CashCollectionAmt = cc.Amount,
                                     CreditAdj = cc.AdjustAmt + cc.CashBAmt + cc.YearlyBnsAmt,
                                     InvoiceNo = cc.ReceiptNo + ",SInv: " + cc.InvoiceNo,
                                     EnteredBy = "Entered By: " + u.UserName,
                                     Remarks = cc.Remarks,
                                     CashCollectionIntAmt = cc.InterestAmt
                                 };
            ledgers.AddRange(CashCollection);
            #endregion


            #region Cash Collection
            var CashCollectionSOComission = from cc in CashCollectionRepository.All
                                            join u in UserRepository.All on cc.CreatedBy equals u.Id into lj
                                            from u in lj.DefaultIfEmpty()
                                            where cc.CustomerID == CustomerID && cc.TransactionType == EnumTranType.SalesCommission
                                            select new LedgerAccountReportModel
                                            {
                                                Date = (DateTime)cc.EntryDate,
                                                Debit = cc.InterestAmt,
                                                VoucherType = "Sales Commission",
                                                Credit = cc.Amount + cc.AdjustAmt,
                                                CashCollectionAmt = cc.Amount,
                                                CreditAdj = cc.AdjustAmt,
                                                InvoiceNo = cc.ReceiptNo,
                                                EnteredBy = "Entered By: " + u.UserName,
                                                Remarks = cc.Remarks,
                                                CashCollectionIntAmt = cc.InterestAmt
                                            };
            ledgers.AddRange(CashCollectionSOComission);
            #endregion


            #region Cash Collection Return
            var CashCollectionReturn = from ccr in CashCollectionRepository.All
                                       join u in UserRepository.All on ccr.CreatedBy equals u.Id into lj
                                       from u in lj.DefaultIfEmpty()
                                       where ccr.CustomerID == CustomerID && ccr.TransactionType == EnumTranType.CollectionReturn
                                       select new LedgerAccountReportModel
                                       {
                                           Date = (DateTime)ccr.EntryDate,
                                           Credit = 0m,
                                           VoucherType = "Collection Return",
                                           Debit = ccr.Amount + ccr.AdjustAmt,
                                           CashCollectionReturn = ccr.Amount,
                                           CreditAdj = ccr.AdjustAmt,
                                           InvoiceNo = ccr.ReceiptNo,
                                           EnteredBy = "Entered By: " + u.UserName,
                                           Remarks = ccr.Remarks
                                       };
            ledgers.AddRange(CashCollectionReturn);
            #endregion

            #region PrevSales Cash Collection Return
            var PrevSalesCashCollectionReturn = from ccr in CashCollectionRepository.All
                                                join u in UserRepository.All on ccr.CreatedBy equals u.Id into lj
                                                from u in lj.DefaultIfEmpty()
                                                where ccr.CustomerID == CustomerID && ccr.TransactionType == EnumTranType.PreviousSalesRetrun
                                                select new LedgerAccountReportModel
                                                {
                                                    Date = (DateTime)ccr.EntryDate,
                                                    Credit = ccr.Amount + ccr.AdjustAmt,
                                                    VoucherType = "Previous Sales Return",
                                                    Debit = 0m,
                                                    CashCollectionReturn = ccr.Amount,
                                                    CreditAdj = ccr.AdjustAmt,
                                                    InvoiceNo = ccr.ReceiptNo,
                                                    EnteredBy = "Entered By: " + u.UserName,
                                                    Remarks = ccr.Remarks
                                                };
            ledgers.AddRange(PrevSalesCashCollectionReturn);
            #endregion

            #region Bank Transaction
            var bankTrans = from bt in BankTransactionRepository.All
                            join b in BankRepository.All on bt.BankID equals b.BankID
                            where bt.CustomerID == CustomerID
                            select new LedgerAccountReportModel
                            {
                                Date = (DateTime)bt.TranDate,
                                Debit = 0m,
                                VoucherType = "Bank Collect.",
                                Credit = bt.Amount,
                                CashCollectionAmt = bt.Amount,
                                CreditAdj = 0m,
                                InvoiceNo = bt.TransactionNo,
                                Particulars = b.AccountName + " " + b.AccountNo + " Chk. No: " + bt.ChecqueNo,
                                Remarks = bt.Remarks
                            };
            ledgers.AddRange(bankTrans);
            #endregion

            decimal balance = Customer.OpeningDue;
            ledgers = ledgers.OrderBy(i => i.Date).ToList();
            foreach (var item in ledgers)
            {
                item.Balance = balance + (item.Debit - item.Credit);
                item.Particulars = string.IsNullOrEmpty(item.Particulars) ? string.Join(Environment.NewLine, item.ProductList) + Environment.NewLine + item.EnteredBy : item.Particulars;
                balance = item.Balance;
            }

            var oOpening = new LedgerAccountReportModel() { Date = new DateTime(2015, 1, 1), Particulars = "Opening Balance", Debit = Customer.OpeningDue, Balance = 0, Credit = 0 };

            if (ledgers.Count > 0)
            {
                //ledgers.Insert(0, oOpening);
                var OpeningTrans = ledgers.Where(i => i.Date < fromDate).OrderByDescending(i => i.Date).FirstOrDefault();
                if (OpeningTrans != null)
                    FinalLedgers.Add(new LedgerAccountReportModel() { Date = OpeningTrans.Date, Particulars = "Opening Balance", Balance = OpeningTrans.Balance, Debit = 0m });
                else
                    FinalLedgers.Add(new LedgerAccountReportModel() { Date = fromDate, Particulars = "Opening Balance", Balance = Customer.OpeningDue, Debit = 0m });

                ledgers = ledgers.Where(i => i.Date >= fromDate && i.Date <= toDate).OrderBy(i => i.Date).ToList();
                FinalLedgers.AddRange(ledgers);
            }
            else
            {
                FinalLedgers.Add(new LedgerAccountReportModel() { Date = fromDate, Particulars = "Opening Balance", Debit = Customer.OpeningDue, Credit = 0m, Balance = Customer.OpeningDue });
            }

            return FinalLedgers;
        }

        public static decimal GetEmployeeTragetCommission(this IBaseRepository<SOrder> SOrderRepository, IBaseRepository<SOrderDetail> SOrderDetailRepository,
                 IBaseRepository<Product> ProductRepository, IBaseRepository<EmployeeTargetSetup> EmployeeTargetSetupRepository,
                 DateTime fromDate, DateTime toDate, int ConcernID, int EmployeeID)
        {
            //decimal TotalExtraComm = 0m;

            var TargetCategory = EmployeeTargetSetupRepository.All;
            var Sales = (from so in SOrderRepository.All
                         join sod in SOrderDetailRepository.All on so.SOrderID equals sod.SOrderID
                         join p in ProductRepository.All on sod.ProductID equals p.ProductID
                         where (so.Status == (int)EnumSalesType.Sales && so.InvoiceDate >= fromDate && so.InvoiceDate <= toDate) && so.EmployeeID == EmployeeID
                         select new
                         {
                             so.InvoiceDate,
                             so.CustomerID,
                             sod.ProductID,
                             p.CategoryID,
                             sod.PPDAmount,
                             sod.PPDPercentage,
                             sod.UnitPrice,
                             sod.Quantity,
                             sod.UTAmount,
                             so.TotalAmount
                         }).ToList();
            decimal TotalSalesAmt = Sales.Sum(i => i.TotalAmount);
            //decimal AcceptedAmount = (TotalSalesAmt * 25m) / 100m;
            //decimal TotalGivenDiscount = Sales.Sum(i => (i.PPDAmount * i.Quantity));
            //if (TotalGivenDiscount <= AcceptedAmount)
            //    TotalExtraComm = 250m * Sales.Count();

            return TotalSalesAmt;
        }

        public static IEnumerable<SOredersReportModel> GetSalesReportBySRID(this IBaseRepository<SOrder> salesOrderRepository,
        IBaseRepository<Customer> customerRepository, IBaseRepository<Employee> employeeRepository,
        IBaseRepository<CreditSale> CreditSaleRepository,
        DateTime fromDate, DateTime toDate, int concernID, int SRID, int RptType)
        {
            List<SOredersReportModel> oAllMOWiseSalesDetailData = null;
            IQueryable<Employee> employees = null;

            if (SRID > 0)
                employees = employeeRepository.All.Where(i => i.EmployeeID == SRID);
            else
                employees = employeeRepository.All;

            oAllMOWiseSalesDetailData = (from SO in salesOrderRepository.All
                                         join CO in customerRepository.All on SO.CustomerID equals CO.CustomerID
                                         join Emp in employees on CO.EmployeeID equals Emp.EmployeeID
                                         where ((SO.Status == (int)EnumSalesType.Sales)
                                                && (SO.InvoiceDate >= fromDate && SO.InvoiceDate <= toDate))
                                         select new SOredersReportModel
                                         {
                                             EmployeeName = Emp.Name,
                                             InvoiceDate = SO.InvoiceDate,
                                             CustomerName = CO.Name,
                                             InvoiceNo = SO.InvoiceNo,
                                             Grandtotal = SO.GrandTotal,
                                             NetDiscount = SO.NetDiscount,
                                             TotalAmount = SO.TotalAmount,
                                             RecAmount = (decimal)SO.RecAmount,
                                             PaymentDue = SO.PaymentDue,
                                             AdjAmount = SO.AdjAmount,
                                             CustomerContactNo = CO.ContactNo,
                                             Trems = SO.Terms
                                         }).OrderByDescending(x => x.InvoiceDate).ToList();

            return oAllMOWiseSalesDetailData;

        }


        public static IEnumerable<SOredersReportModel> GetProductSalesReportBySRID(this IBaseRepository<SOrder> salesOrderRepository,
        IBaseRepository<Product> productRepository, IBaseRepository<Category> categoryRepository, IBaseRepository<ProductUnitType> productUnitTypeRepository, IBaseRepository<Employee> employeeRepository, IBaseRepository<CreditSale> CreditSaleRepository, IBaseRepository<SOrderDetail> sOrderDetailsRepository, IBaseRepository<Customer> customerRepository,
        DateTime fromDate, DateTime toDate, int concernID, int SRID, int ProductId, int RptType)
        {
            List<SOredersReportModel> oAllMOWiseSalesDetailData = null;
            IQueryable<Employee> employees = null;
            IQueryable<Product> products = null;

            if (SRID > 0)
                employees = employeeRepository.All.Where(i => i.EmployeeID == SRID);
            else
                employees = employeeRepository.All;

            if (ProductId > 0)
            {
                products = productRepository.All.Where(i => i.ProductID == ProductId);
            }
            else
            {
                products = productRepository.All;
            }

            oAllMOWiseSalesDetailData = (from SO in salesOrderRepository.All
                                         join SOD in sOrderDetailsRepository.All on SO.SOrderID equals SOD.SOrderID
                                         join C in customerRepository.All on SO.CustomerID equals C.CustomerID
                                         join Emp in employees on C.EmployeeID equals Emp.EmployeeID
                                         join P in productRepository.All on SOD.ProductID equals P.ProductID
                                         join UT in productUnitTypeRepository.All on P.ProUnitTypeID equals UT.ProUnitTypeID
                                         where ((SO.Status == (int)EnumSalesType.Sales)
                                                && (SO.InvoiceDate >= fromDate && SO.InvoiceDate <= toDate) && (products.Contains(P)))
                                         where ((SO.Status == (int)EnumSalesType.Sales)
                                                && (SO.InvoiceDate >= fromDate && SO.InvoiceDate <= toDate))
                                         select new SOredersReportModel
                                         {
                                             EmployeeName = Emp.Name,
                                             InvoiceDate = SO.InvoiceDate,
                                             ProductName = P.ProductName,
                                             SizeName = P.Size.Description,
                                             Quantity = Math.Truncate(SOD.Quantity / (P.BundleQty > 0 ? P.BundleQty : UT.ConvertValue)),
                                             ParentUnit = UT.Description,
                                             UnitQty = SOD.Quantity % (P.BundleQty > 0 ? P.BundleQty : UT.ConvertValue),
                                             UnitPrice = SOD.UnitPrice * (P.BundleQty > 0 ? P.BundleQty : UT.ConvertValue),
                                             UTAmount = SOD.UTAmount,
                                             UnitName = UT.UnitName,
                                             Trems = SO.Terms
                                         }).OrderByDescending(x => x.InvoiceDate).ToList();

            return oAllMOWiseSalesDetailData;

        }

        public static List<VoucherTransactionReportModel> BankAndCashAccountLedgerData(this IBaseRepository<SOrder> salesOrderRepository,
        IBaseRepository<Customer> _CustomerRepository, IBaseRepository<Supplier> _SupplierRepository, IBaseRepository<ExpenseItem> _ExpenseItemRepository,
        IBaseRepository<ShareInvestmentHead> _ShareInvestmentHeadRepository, IBaseRepository<Bank> _BankRepository, IBaseRepository<POrder> POrderRepository,
        IBaseRepository<CashAccount> cashAccRepository, IBaseRepository<CashCollection> CashCollectionRepository, IBaseRepository<Expenditure> _ExpenseditureRepository,
        IBaseRepository<ShareInvestment> _ShareInvestmentRepository, IBaseRepository<BankTransaction> _BankTransactionRepository,
        DateTime fromDate, DateTime toDate, int ConcernID, int ExpenseItemID, string headType)
        {
            List<VoucherTransactionReportModel> ledgers = new List<VoucherTransactionReportModel>();
            List<VoucherTransactionReportModel> FinalLedgers = new List<VoucherTransactionReportModel>();

            #region Cash Account
            var CashAccountOpeningData = (from exp in cashAccRepository.All
                                          where exp.Id == ExpenseItemID
                                          select new VoucherTransactionReportModel
                                          {
                                              Opening = exp.OpeningBalance,
                                              ModuleType = "Opening",
                                              ItemName = exp.Name,
                                              VoucherDate = exp.OpeningDate,
                                              Narration = "Opening Balance"

                                          }).ToList();
            ledgers.AddRange(CashAccountOpeningData);

            var CashPayPurchaseData = (from VT in POrderRepository.All
                                       join exp in cashAccRepository.All on VT.PayCashAccountId equals exp.Id
                                       where VT.Status == (int)EnumPurchaseType.Purchase && VT.PayCashAccountId == ExpenseItemID
                                       select new VoucherTransactionReportModel
                                       {
                                           VoucherNo = VT.ChallanNo,
                                           VoucherDate = VT.OrderDate,
                                           DebitAmount = 0m,
                                           CreditAmount = VT.RecAmt,
                                           ModuleType = "Purchase Payment",
                                           Narration = VT.Remarks,
                                           ItemName = exp.Name

                                       }).ToList();
            ledgers.AddRange(CashPayPurchaseData);

            var CashSalesData = from VT in salesOrderRepository.All
                                join exp in cashAccRepository.All on VT.PayCashAccountId equals exp.Id
                                where VT.Status == 1 && VT.PayCashAccountId == ExpenseItemID
                                select new VoucherTransactionReportModel
                                {
                                    VoucherNo = VT.InvoiceNo,
                                    VoucherDate = VT.InvoiceDate,
                                    DebitAmount = (decimal)VT.RecAmount,
                                    CreditAmount = 0m,
                                    ModuleType = "Sales Order",
                                    Narration = VT.Remarks,
                                    ItemName = exp.Name
                                };
            ledgers.AddRange(CashSalesData);

            var CashDelivery = from VT in CashCollectionRepository.All
                               join exp in cashAccRepository.All on VT.PayCashAccountId equals exp.Id
                               where VT.TransactionType == EnumTranType.ToCompany && VT.PayCashAccountId == ExpenseItemID
                               select new VoucherTransactionReportModel
                               {
                                   VoucherNo = VT.ReceiptNo,
                                   VoucherDate = (DateTime)VT.EntryDate,
                                   DebitAmount = 0m,
                                   CreditAmount = VT.Amount,
                                   ModuleType = "Cash Delivery",
                                   Narration = VT.Remarks,
                                   ItemName = exp.Name
                               };
            ledgers.AddRange(CashDelivery);

            var CashCollection = from VT in CashCollectionRepository.All
                                 join exp in cashAccRepository.All on VT.PayCashAccountId equals exp.Id
                                 where VT.TransactionType == EnumTranType.FromCustomer && VT.PayCashAccountId == ExpenseItemID
                                 select new VoucherTransactionReportModel
                                 {
                                     VoucherNo = VT.ReceiptNo,
                                     VoucherDate = (DateTime)VT.EntryDate,
                                     DebitAmount = VT.Amount,
                                     CreditAmount = 0m,
                                     ModuleType = "Cash Collection",
                                     Narration = VT.Remarks,
                                     ItemName = exp.Name
                                 };
            ledgers.AddRange(CashCollection);

            var CashCollectionReturn = from VT in CashCollectionRepository.All
                                       join exp in cashAccRepository.All on VT.PayCashAccountId equals exp.Id
                                       where VT.TransactionType == EnumTranType.CollectionReturn && VT.PayCashAccountId == ExpenseItemID
                                       select new VoucherTransactionReportModel
                                       {
                                           VoucherNo = VT.ReceiptNo,
                                           VoucherDate = (DateTime)VT.EntryDate,
                                           DebitAmount = 0m,
                                           CreditAmount = VT.Amount,
                                           ModuleType = "Collection Return",
                                           Narration = VT.Remarks,
                                           ItemName = exp.Name
                                       };
            ledgers.AddRange(CashCollectionReturn);

            var CashPayExpenseData = (from VT in _ExpenseditureRepository.All
                                      join exp in cashAccRepository.All on VT.PayCashAccountId equals exp.Id
                                      join expi in _ExpenseItemRepository.All on VT.ExpenseItemID equals expi.ExpenseItemID
                                      where expi.Status == EnumCompanyTransaction.Expense && VT.PayCashAccountId == ExpenseItemID
                                      select new VoucherTransactionReportModel
                                      {
                                          VoucherNo = VT.VoucherNo,
                                          VoucherDate = VT.EntryDate,
                                          DebitAmount = 0m,
                                          CreditAmount = VT.Amount,
                                          ModuleType = "Expense",
                                          Narration = VT.Purpose,
                                          ItemName = exp.Name

                                      }).ToList();
            ledgers.AddRange(CashPayExpenseData);

            var CashPayIncomeData = (from VT in _ExpenseditureRepository.All
                                     join exp in cashAccRepository.All on VT.PayCashAccountId equals exp.Id
                                     join expi in _ExpenseItemRepository.All on VT.ExpenseItemID equals expi.ExpenseItemID
                                     where expi.Status == EnumCompanyTransaction.Income && VT.PayCashAccountId == ExpenseItemID
                                     select new VoucherTransactionReportModel
                                     {
                                         VoucherNo = VT.VoucherNo,
                                         VoucherDate = VT.EntryDate,
                                         DebitAmount = VT.Amount,
                                         CreditAmount = 0m,
                                         ModuleType = "Income",
                                         Narration = VT.Purpose,
                                         ItemName = exp.Name

                                     }).ToList();
            ledgers.AddRange(CashPayIncomeData);

            var CashPayLiabilityPayData = (from VT in _ShareInvestmentRepository.All
                                           join exp in cashAccRepository.All on VT.PayCashAccountId equals exp.Id
                                           join expi in _ShareInvestmentHeadRepository.All on VT.SIHID equals expi.SIHID
                                           where expi.ParentId == 3 && (VT.LiabilityType == EnumLiabilityType.GiveLoan || VT.LiabilityType == EnumLiabilityType.LoanPay) && VT.PayCashAccountId == ExpenseItemID
                                           select new VoucherTransactionReportModel
                                           {
                                               VoucherNo = "",
                                               VoucherDate = VT.EntryDate,
                                               DebitAmount = 0m,
                                               CreditAmount = VT.Amount,
                                               ModuleType = "Liability Pay",
                                               Narration = VT.Purpose,
                                               ItemName = exp.Name

                                           }).ToList();
            ledgers.AddRange(CashPayLiabilityPayData);

            var CashPayLiabilityRecData = (from VT in _ShareInvestmentRepository.All
                                           join exp in cashAccRepository.All on VT.PayCashAccountId equals exp.Id
                                           join expi in _ShareInvestmentHeadRepository.All on VT.SIHID equals expi.SIHID
                                           where expi.ParentId == 3 && (VT.LiabilityType == EnumLiabilityType.TakeLoan || VT.LiabilityType == EnumLiabilityType.LoanCollection) && VT.PayCashAccountId == ExpenseItemID
                                           select new VoucherTransactionReportModel
                                           {
                                               VoucherNo = "",
                                               VoucherDate = VT.EntryDate,
                                               DebitAmount = VT.Amount,
                                               CreditAmount = 0m,
                                               ModuleType = "Liability Receive",
                                               Narration = VT.Purpose,
                                               ItemName = exp.Name

                                           }).ToList();
            ledgers.AddRange(CashPayLiabilityRecData);

            var BankDepositeCash = from VT in _BankTransactionRepository.All
                                   join exp in _BankRepository.All on VT.BankID equals exp.BankID
                                   where VT.TransactionType == 1 && VT.PayCashAccountId == ExpenseItemID
                                   select new VoucherTransactionReportModel
                                   {
                                       VoucherNo = VT.TransactionNo,
                                       VoucherDate = (DateTime)VT.TranDate,
                                       DebitAmount = 0m,
                                       CreditAmount = VT.Amount,
                                       ModuleType = "Deposite",
                                       Narration = VT.Remarks,
                                       ItemName = exp.BankName
                                   };
            ledgers.AddRange(BankDepositeCash);


            var BankWithdrawCash = from VT in _BankTransactionRepository.All
                                   join exp in _BankRepository.All on VT.BankID equals exp.BankID
                                   where VT.TransactionType == 2 && VT.PayCashAccountId == ExpenseItemID
                                   select new VoucherTransactionReportModel
                                   {
                                       VoucherNo = VT.TransactionNo,
                                       VoucherDate = (DateTime)VT.TranDate,
                                       DebitAmount = VT.Amount,
                                       CreditAmount = 0m,
                                       ModuleType = "Withdraw",
                                       Narration = VT.Remarks,
                                       ItemName = exp.BankName
                                   };
            ledgers.AddRange(BankWithdrawCash);

            #endregion

            #region Bank

            var BankOpeningData = (from exp in _BankRepository.All
                                   where exp.BankID == ExpenseItemID
                                   select new VoucherTransactionReportModel
                                   {
                                       Opening = exp.OpeningBalance,
                                       ModuleType = "Opening",
                                       ItemName = exp.BankName,
                                       VoucherDate = exp.OpeningDate,
                                       Narration = "Opening Balance"

                                   }).ToList();
            ledgers.AddRange(BankOpeningData);

            var BankPayPurchaseData = (from VT in POrderRepository.All
                                       join exp in _BankRepository.All on VT.PayBankId equals exp.BankID
                                       where VT.Status == (int)EnumPurchaseType.Purchase && VT.PayBankId == ExpenseItemID
                                       select new VoucherTransactionReportModel
                                       {
                                           VoucherNo = VT.ChallanNo,
                                           VoucherDate = VT.OrderDate,
                                           DebitAmount = 0m,
                                           CreditAmount = VT.RecAmt,
                                           ModuleType = "Purchase Payment",
                                           Narration = VT.Remarks,
                                           ItemName = exp.BankName

                                       }).ToList();
            ledgers.AddRange(BankPayPurchaseData);

            var BankSalesData = from VT in salesOrderRepository.All
                                join exp in _BankRepository.All on VT.PayBankId equals exp.BankID
                                where VT.Status == 1 && VT.PayBankId == ExpenseItemID
                                select new VoucherTransactionReportModel
                                {
                                    VoucherNo = VT.InvoiceNo,
                                    VoucherDate = VT.InvoiceDate,
                                    DebitAmount = (decimal)VT.RecAmount,
                                    CreditAmount = 0m,
                                    ModuleType = "Sales Order",
                                    Narration = VT.Remarks,
                                    ItemName = exp.BankName
                                };
            ledgers.AddRange(BankSalesData);

            var BankCashDelivery = from VT in CashCollectionRepository.All
                                   join exp in _BankRepository.All on VT.PayBankId equals exp.BankID
                                   where VT.TransactionType == EnumTranType.ToCompany && VT.PayBankId == ExpenseItemID
                                   select new VoucherTransactionReportModel
                                   {
                                       VoucherNo = VT.ReceiptNo,
                                       VoucherDate = (DateTime)VT.EntryDate,
                                       DebitAmount = 0m,
                                       CreditAmount = VT.Amount,
                                       ModuleType = "Cash Delivery",
                                       Narration = VT.Remarks,
                                       ItemName = exp.BankName
                                   };
            ledgers.AddRange(BankCashDelivery);

            var BankCashCollection = from VT in CashCollectionRepository.All
                                     join exp in _BankRepository.All on VT.PayBankId equals exp.BankID
                                     where VT.TransactionType == EnumTranType.FromCustomer && VT.PayBankId == ExpenseItemID
                                     select new VoucherTransactionReportModel
                                     {
                                         VoucherNo = VT.ReceiptNo,
                                         VoucherDate = (DateTime)VT.EntryDate,
                                         DebitAmount = VT.Amount,
                                         CreditAmount = 0m,
                                         ModuleType = "Cash Collection",
                                         Narration = VT.Remarks,
                                         ItemName = exp.BankName
                                     };
            ledgers.AddRange(BankCashCollection);

            var BankCashCollectionReturn = from VT in CashCollectionRepository.All
                                           join exp in _BankRepository.All on VT.PayBankId equals exp.BankID
                                           where VT.TransactionType == EnumTranType.CollectionReturn && VT.PayBankId == ExpenseItemID
                                           select new VoucherTransactionReportModel
                                           {
                                               VoucherNo = VT.ReceiptNo,
                                               VoucherDate = (DateTime)VT.EntryDate,
                                               DebitAmount = 0m,
                                               CreditAmount = VT.Amount,
                                               ModuleType = "Collection Retun",
                                               Narration = VT.Remarks,
                                               ItemName = exp.BankName
                                           };
            ledgers.AddRange(BankCashCollectionReturn);

            var BankPayExpenseData = (from VT in _ExpenseditureRepository.All
                                      join exp in _BankRepository.All on VT.PayBankId equals exp.BankID
                                      join expi in _ExpenseItemRepository.All on VT.ExpenseItemID equals expi.ExpenseItemID
                                      where expi.Status == EnumCompanyTransaction.Expense && VT.PayBankId == ExpenseItemID
                                      select new VoucherTransactionReportModel
                                      {
                                          VoucherNo = VT.VoucherNo,
                                          VoucherDate = VT.EntryDate,
                                          DebitAmount = 0m,
                                          CreditAmount = VT.Amount,
                                          ModuleType = "Expense",
                                          Narration = VT.Purpose,
                                          ItemName = exp.BankName

                                      }).ToList();
            ledgers.AddRange(BankPayExpenseData);

            var BankPayIncomeData = (from VT in _ExpenseditureRepository.All
                                     join exp in _BankRepository.All on VT.PayBankId equals exp.BankID
                                     join expi in _ExpenseItemRepository.All on VT.ExpenseItemID equals expi.ExpenseItemID
                                     where expi.Status == EnumCompanyTransaction.Income && VT.PayBankId == ExpenseItemID
                                     select new VoucherTransactionReportModel
                                     {
                                         VoucherNo = VT.VoucherNo,
                                         VoucherDate = VT.EntryDate,
                                         DebitAmount = VT.Amount,
                                         CreditAmount = 0m,
                                         ModuleType = "Income",
                                         Narration = VT.Purpose,
                                         ItemName = exp.BankName

                                     }).ToList();
            ledgers.AddRange(BankPayIncomeData);

            var BankPayLiabilityPayData = (from VT in _ShareInvestmentRepository.All
                                           join exp in _BankRepository.All on VT.PayBankId equals exp.BankID
                                           join expi in _ShareInvestmentHeadRepository.All on VT.SIHID equals expi.SIHID
                                           where expi.ParentId == 3 && (VT.LiabilityType == EnumLiabilityType.GiveLoan || VT.LiabilityType == EnumLiabilityType.LoanPay) && VT.PayBankId == ExpenseItemID
                                           select new VoucherTransactionReportModel
                                           {
                                               VoucherNo = "",
                                               VoucherDate = VT.EntryDate,
                                               DebitAmount = 0m,
                                               CreditAmount = VT.Amount,
                                               ModuleType = "Liability Pay",
                                               Narration = VT.Purpose,
                                               ItemName = exp.BankName

                                           }).ToList();
            ledgers.AddRange(BankPayLiabilityPayData);

            var BankPayLiabilityRecData = (from VT in _ShareInvestmentRepository.All
                                           join exp in _BankRepository.All on VT.PayBankId equals exp.BankID
                                           join expi in _ShareInvestmentHeadRepository.All on VT.SIHID equals expi.SIHID
                                           where expi.ParentId == 3 && (VT.LiabilityType == EnumLiabilityType.TakeLoan || VT.LiabilityType == EnumLiabilityType.LoanCollection) && VT.PayBankId == ExpenseItemID
                                           select new VoucherTransactionReportModel
                                           {
                                               VoucherNo = "",
                                               VoucherDate = VT.EntryDate,
                                               DebitAmount = VT.Amount,
                                               CreditAmount = 0m,
                                               ModuleType = "Liability Receive",
                                               Narration = VT.Purpose,
                                               ItemName = exp.BankName

                                           }).ToList();
            ledgers.AddRange(BankPayLiabilityRecData);

            var BankWithdraw = from VT in _BankTransactionRepository.All
                               join exp in _BankRepository.All on VT.BankID equals exp.BankID
                               where VT.TransactionType == 2 && VT.BankID == ExpenseItemID
                               select new VoucherTransactionReportModel
                               {
                                   VoucherNo = VT.TransactionNo,
                                   VoucherDate = (DateTime)VT.TranDate,
                                   DebitAmount = 0m,
                                   CreditAmount = VT.Amount,
                                   ModuleType = "Withdraw",
                                   Narration = VT.Remarks,
                                   ItemName = exp.BankName
                               };
            ledgers.AddRange(BankWithdraw);

            var BankDeposite = from VT in _BankTransactionRepository.All
                               join exp in _BankRepository.All on VT.BankID equals exp.BankID
                               where VT.TransactionType == 1 && VT.BankID == ExpenseItemID
                               select new VoucherTransactionReportModel
                               {
                                   VoucherNo = VT.TransactionNo,
                                   VoucherDate = (DateTime)VT.TranDate,
                                   DebitAmount = VT.Amount,
                                   CreditAmount = 0m,
                                   ModuleType = "Deposite",
                                   Narration = VT.Remarks,
                                   ItemName = exp.BankName
                               };
            ledgers.AddRange(BankDeposite);

            var BankFundOutWithdraw = from VT in _BankTransactionRepository.All
                                      join exp in _BankRepository.All on VT.BankID equals exp.BankID
                                      where VT.TransactionType == 5 && VT.BankID == ExpenseItemID
                                      select new VoucherTransactionReportModel
                                      {
                                          VoucherNo = VT.TransactionNo,
                                          VoucherDate = (DateTime)VT.TranDate,
                                          DebitAmount = 0m,
                                          CreditAmount = VT.Amount,
                                          ModuleType = "Fund Transfer Out",
                                          Narration = VT.Remarks,
                                          ItemName = exp.BankName
                                      };
            ledgers.AddRange(BankFundOutWithdraw);


            var BankFundIn = from VT in _BankTransactionRepository.All
                             join exp in _BankRepository.All on VT.BankID equals exp.BankID
                             where VT.TransactionType == 5 && VT.AnotherBankID == ExpenseItemID
                             select new VoucherTransactionReportModel
                             {
                                 VoucherNo = VT.TransactionNo,
                                 VoucherDate = (DateTime)VT.TranDate,
                                 DebitAmount = VT.Amount,
                                 CreditAmount = 0m,
                                 ModuleType = "Fund Transfer In",
                                 Narration = VT.Remarks,
                                 ItemName = exp.BankName
                             };
            ledgers.AddRange(BankFundIn);



            #endregion

            decimal openingdue = ledgers.Select(i => i.Opening).FirstOrDefault();

            decimal balance = openingdue;
            ledgers = ledgers.OrderBy(i => i.VoucherDate).ToList();

            foreach (var item in ledgers)
            {
                //decimal balance = item.Opening;
                if (headType.ToLower().Equals("s"))
                {
                    item.Balance = balance + (item.CreditAmount - item.DebitAmount);
                }
                else if (headType.ToLower().Equals("in"))
                {
                    item.Balance = balance + (item.CreditAmount - item.DebitAmount);
                }
                else if (headType.ToLower().Equals("sc"))
                {
                    item.Balance = balance + (item.CreditAmount - item.DebitAmount);
                }
                else
                {
                    item.Balance = balance + (item.DebitAmount - item.CreditAmount);
                }

                //item.Particulars = string.IsNullOrEmpty(item.Particulars) ? string.Join(Environment.NewLine, item.ProductList) + Environment.NewLine + item.EnteredBy : item.Particulars;
                item.Particulars = string.IsNullOrEmpty(item.Particulars) ? string.Join(Environment.NewLine, item.Narration) : item.Particulars;
                balance = item.Balance;
            }

            //var oOpening = new VoucherTransactionReportModel() { VoucherDate = new DateTime(2015, 1, 1), Particulars = "Opening Balance", DebitAmount = openingdue, Balance = 0m, CreditAmount = 0 };

            if (ledgers.Count > 0)
            {
                if (headType.ToLower().Equals("s"))
                {
                    var OpeningTrans = ledgers.Where(i => i.VoucherDate < fromDate).OrderByDescending(i => i.VoucherDate < fromDate).LastOrDefault();
                    if (OpeningTrans != null)
                        FinalLedgers.Add(new VoucherTransactionReportModel() { VoucherDate = OpeningTrans.VoucherDate, Particulars = "Opening Balance", Narration = "Opening Balance", Balance = OpeningTrans.Balance, CreditAmount = 0m });
                    else
                        FinalLedgers.Add(new VoucherTransactionReportModel() { VoucherDate = fromDate, Particulars = "Opening Balance", Balance = openingdue, CreditAmount = 0m });

                    ledgers = ledgers.Where(i => i.VoucherDate >= fromDate && i.VoucherDate <= toDate).OrderBy(i => i.VoucherDate).ToList();
                    FinalLedgers.AddRange(ledgers);
                }
                else if (headType.ToLower().Equals("IN"))
                {
                    var OpeningTrans = ledgers.Where(i => i.VoucherDate < fromDate).OrderByDescending(i => i.VoucherDate < fromDate).LastOrDefault();
                    if (OpeningTrans != null)
                        FinalLedgers.Add(new VoucherTransactionReportModel() { VoucherDate = OpeningTrans.VoucherDate, Particulars = "Opening Balance", Narration = "Opening Balance", Balance = OpeningTrans.Balance, CreditAmount = 0m });
                    else
                        FinalLedgers.Add(new VoucherTransactionReportModel() { VoucherDate = fromDate, Particulars = "Opening Balance", Balance = openingdue, CreditAmount = 0m });

                    ledgers = ledgers.Where(i => i.VoucherDate >= fromDate && i.VoucherDate <= toDate).OrderBy(i => i.VoucherDate).ToList();
                    FinalLedgers.AddRange(ledgers);
                }
                else if (headType.ToLower().Equals("sc"))
                {
                    var OpeningTrans = ledgers.Where(i => i.VoucherDate < fromDate).OrderByDescending(i => i.VoucherDate < fromDate).LastOrDefault();
                    if (OpeningTrans != null)
                        FinalLedgers.Add(new VoucherTransactionReportModel() { VoucherDate = OpeningTrans.VoucherDate, Particulars = "Opening Balance", Narration = "Opening Balance", Balance = OpeningTrans.Balance, CreditAmount = 0m });
                    else
                        FinalLedgers.Add(new VoucherTransactionReportModel() { VoucherDate = fromDate, Particulars = "Opening Balance", Balance = openingdue, CreditAmount = 0m });

                    ledgers = ledgers.Where(i => i.VoucherDate >= fromDate && i.VoucherDate <= toDate).OrderBy(i => i.VoucherDate).ToList();
                    FinalLedgers.AddRange(ledgers);
                }
                else
                {
                    var OpeningTrans = ledgers.Where(i => i.VoucherDate < fromDate).OrderByDescending(i => i.VoucherDate < fromDate).LastOrDefault();
                    if (OpeningTrans != null)
                        FinalLedgers.Add(new VoucherTransactionReportModel() { VoucherDate = OpeningTrans.VoucherDate, Particulars = "Opening Balance", Narration = "Opening Balance", Balance = OpeningTrans.Balance, DebitAmount = 0m });
                    else
                        FinalLedgers.Add(new VoucherTransactionReportModel() { VoucherDate = fromDate, Particulars = "Opening Balance", Balance = openingdue, DebitAmount = 0m });

                    ledgers = ledgers.Where(i => i.VoucherDate >= fromDate && i.VoucherDate <= toDate).OrderBy(i => i.VoucherDate).ToList();
                    FinalLedgers.AddRange(ledgers);
                }

            }
            else
            {
                if (headType.ToLower().Equals("s"))
                {
                    FinalLedgers.Add(new VoucherTransactionReportModel() { VoucherDate = fromDate, Particulars = "Opening Balance", CreditAmount = openingdue, DebitAmount = 0m, Balance = openingdue });
                }
                else if (headType.ToLower().Equals("IN"))
                {
                    FinalLedgers.Add(new VoucherTransactionReportModel() { VoucherDate = fromDate, Particulars = "Opening Balance", CreditAmount = openingdue, DebitAmount = 0m, Balance = openingdue });
                }
                else
                {
                    FinalLedgers.Add(new VoucherTransactionReportModel() { VoucherDate = fromDate, Particulars = "Opening Balance", DebitAmount = openingdue, CreditAmount = 0m, Balance = openingdue });
                }

            }

            return FinalLedgers;

        }



        public static List<RPTPayRecTO> GetReceiptPaymentReport(this IBaseRepository<SOrder> _salesOrderRepository, IBaseRepository<Bank> _bankRepository, IBaseRepository<Customer> _customerRepository, IBaseRepository<ShareInvestmentHead> _investmentHeadRepository, IBaseRepository<Supplier> _supplierRepository, IBaseRepository<ExpenseItem> _expenseRepository, IBaseRepository<CashAccount> _cashRepository, IBaseRepository<POrder> _pOrderRepository,
          IBaseRepository<CashCollection> _cashCollectionRepository, IBaseRepository<Expenditure> _expenseditureRepository, IBaseRepository<ShareInvestment> _shareInvestmentRepository, IBaseRepository<BankTransaction> _bankTransactionRepository, DateTime fromDate, DateTime toDate)
        {
            List<RPTPayRecTO> result = new List<RPTPayRecTO>();

            #region Debit

            #region Cash
            var allPOCashWithSupp = _pOrderRepository.All.Where(d => d.Status == (int)EnumPurchaseType.Purchase && d.PayCashAccountId > 0 && DbFunctions.TruncateTime(d.OrderDate) <= DbFunctions.TruncateTime(toDate))
              .OrderBy(d => d.OrderDate)
              .ToList();

            int serial = 1;
            CashAccount cashAccount = _cashRepository.All.Where(i => i.OpeningDate <= fromDate).FirstOrDefault();
            decimal cashOpening = 0m;
            decimal cashClosing = 0m;
            if (cashAccount != null)
            {
                cashOpening = cashAccount.OpeningBalance;
                cashClosing = cashOpening;
            }

            foreach (var cash in allPOCashWithSupp)
            {
                cashClosing = cashOpening;
                if (cash.OrderDate < fromDate)
                {
                    cashOpening -= cash.RecAmt;
                }

                if (cash.OrderDate >= fromDate)
                {
                    cashClosing = cashOpening;

                    if (cash.Status == (int)EnumPurchaseType.Purchase)
                    {
                        cashClosing -= cash.RecAmt;
                    }
                }
                else
                {
                    cashClosing = cashOpening;
                }
            }

            var allSalesCASH = _salesOrderRepository.All.Where(d => d.Status == 1 && d.PayCashAccountId > 0 && DbFunctions.TruncateTime(d.InvoiceDate) <= DbFunctions.TruncateTime(toDate))
              .OrderBy(d => d.InvoiceDate)
              .ToList();
            foreach (var cash in allSalesCASH)
            {
                cashClosing = cashOpening;
                if (cash.InvoiceDate < fromDate)
                {
                    cashOpening += (decimal)cash.RecAmount;
                }

                if (cash.InvoiceDate >= fromDate)
                {
                    cashClosing = cashOpening;

                    if (cash.Status == 1)
                    {
                        cashClosing += (decimal)cash.RecAmount;
                    }
                }
                else
                {
                    cashClosing = cashOpening;
                }
            }

            var allCashDeliveryCash = _cashCollectionRepository.All.Where(d => d.TransactionType == EnumTranType.ToCompany && d.PayCashAccountId > 0 && DbFunctions.TruncateTime(d.EntryDate) <= DbFunctions.TruncateTime(toDate))
              .OrderBy(d => d.EntryDate)
              .ToList();
            foreach (var cash in allCashDeliveryCash)
            {
                cashClosing = cashOpening;
                if (cash.EntryDate < fromDate)
                {
                    cashOpening -= cash.Amount;
                }

                if (cash.EntryDate >= fromDate)
                {
                    cashClosing = cashOpening;

                    if (cash.TransactionType == EnumTranType.ToCompany)
                    {
                        cashClosing -= cash.Amount;
                    }
                }
                else
                {
                    cashClosing = cashOpening;
                }
            }

            var allCashCollectionCash = _cashCollectionRepository.All.Where(d => d.TransactionType == EnumTranType.FromCustomer && d.PayCashAccountId > 0 && DbFunctions.TruncateTime(d.EntryDate) <= DbFunctions.TruncateTime(toDate))
              .OrderBy(d => d.EntryDate)
              .ToList();
            foreach (var cash in allCashCollectionCash)
            {
                cashClosing = cashOpening;
                if (cash.EntryDate < fromDate)
                {
                    cashOpening += (decimal)cash.Amount;
                }

                if (cash.EntryDate >= fromDate)
                {
                    cashClosing = cashOpening;

                    if (cash.TransactionType == EnumTranType.FromCustomer)
                    {
                        cashClosing += (decimal)cash.Amount;
                    }
                }
                else
                {
                    cashClosing = cashOpening;
                }
            }

            var allExpenseCash = _expenseditureRepository.All.Where(d => d.ExpenseIncomeStatus == EnumCompanyTransaction.Expense && d.PayCashAccountId > 0 && DbFunctions.TruncateTime(d.EntryDate) <= DbFunctions.TruncateTime(toDate))
              .OrderBy(d => d.EntryDate)
              .ToList();
            foreach (var cash in allExpenseCash)
            {
                cashClosing = cashOpening;
                if (cash.EntryDate < fromDate)
                {
                    cashOpening -= cash.Amount;
                }

                if (cash.EntryDate >= fromDate)
                {
                    cashClosing = cashOpening;

                    if (cash.ExpenseIncomeStatus == EnumCompanyTransaction.Expense)
                    {
                        cashClosing -= cash.Amount;
                    }
                }
                else
                {
                    cashClosing = cashOpening;
                }
            }

            var allIncomeCash = _expenseditureRepository.All.Where(d => d.ExpenseIncomeStatus == EnumCompanyTransaction.Income && d.PayCashAccountId > 0 && DbFunctions.TruncateTime(d.EntryDate) <= DbFunctions.TruncateTime(toDate))
              .OrderBy(d => d.EntryDate)
              .ToList();
            foreach (var cash in allIncomeCash)
            {
                cashClosing = cashOpening;
                if (cash.EntryDate < fromDate)
                {
                    cashOpening += (decimal)cash.Amount;
                }

                if (cash.EntryDate >= fromDate)
                {
                    cashClosing = cashOpening;

                    if (cash.ExpenseIncomeStatus == EnumCompanyTransaction.Income)
                    {
                        cashClosing += (decimal)cash.Amount;
                    }
                }
                else
                {
                    cashClosing = cashOpening;
                }
            }

            if (cashAccount != null)
            {
                RPTPayRecTO CashopeningData = new RPTPayRecTO
                {
                    Id = serial,
                    DebitAmount = cashOpening,
                    CreditAmount = 0m,
                    IsClosing = false,
                    DebitParticular = "Cash In Hand (Opening)",
                    ClosingBalance = 0m,
                    IsDrHeader = true
                };
                result.Add(CashopeningData);
            }
            #endregion

            #region Bank
            List<RecPayTransactionTO> allBankTransaction = new List<RecPayTransactionTO>();

            var allBankDeposite = _bankTransactionRepository.All.Where(d => d.TransactionType == (int)EnumTransactionType.Deposit && d.Amount > 0 && DbFunctions.TruncateTime(d.TranDate) <= DbFunctions.TruncateTime(toDate))
              .OrderBy(d => d.BankID)
              .ThenBy(d => d.TranDate)
              .Select(d => new RecPayTransactionTO
              {
                  BankId = d.BankID,
                  Amount = d.Amount,
                  TransactionDate = (DateTime)d.TranDate,
                  TransactionType = EnumTransactionType.Deposit
              })
              .ToList();
            allBankTransaction.AddRange(allBankDeposite);

            var allBankFundInDeposite = _bankTransactionRepository.All.Where(d => d.TransactionType == (int)EnumTransactionType.FundTransfer && d.Amount > 0 && DbFunctions.TruncateTime(d.TranDate) <= DbFunctions.TruncateTime(toDate))
              .OrderBy(d => d.AnotherBankID)
              .ThenBy(d => d.TranDate)
              .Select(d => new RecPayTransactionTO
              {
                  BankId = d.AnotherBankID,
                  Amount = d.Amount,
                  TransactionDate = (DateTime)d.TranDate,
                  TransactionType = EnumTransactionType.Deposit
              })
              .ToList();
            allBankTransaction.AddRange(allBankFundInDeposite);

            var allBankWithDraw = _bankTransactionRepository.All.Where(d => d.TransactionType == (int)EnumTransactionType.Withdraw && d.Amount > 0 && DbFunctions.TruncateTime(d.TranDate) <= DbFunctions.TruncateTime(toDate))
              .OrderBy(d => d.BankID)
              .ThenBy(d => d.TranDate)
              .Select(d => new RecPayTransactionTO
              {
                  BankId = d.BankID,
                  Amount = d.Amount,
                  TransactionDate = (DateTime)d.TranDate,
                  TransactionType = EnumTransactionType.Withdraw
              })
              .ToList();
            allBankTransaction.AddRange(allBankWithDraw);

            var allBankFundOutWithDraw = _bankTransactionRepository.All.Where(d => d.TransactionType == (int)EnumTransactionType.FundTransfer && d.Amount > 0 && DbFunctions.TruncateTime(d.TranDate) <= DbFunctions.TruncateTime(toDate))
              .OrderBy(d => d.BankID)
              .ThenBy(d => d.TranDate)
              .Select(d => new RecPayTransactionTO
              {
                  BankId = d.BankID,
                  Amount = d.Amount,
                  TransactionDate = (DateTime)d.TranDate,
                  TransactionType = EnumTransactionType.Withdraw
              })
              .ToList();
            allBankTransaction.AddRange(allBankFundOutWithDraw);

            var allPOBank = _pOrderRepository.All.Where(d => d.Status == 1 && d.PayBankId > 0 && DbFunctions.TruncateTime(d.OrderDate) <= DbFunctions.TruncateTime(toDate))
              .OrderBy(d => d.PayBankId)
              .ThenBy(d => d.OrderDate)
              .Select(d => new RecPayTransactionTO
              {
                  BankId = d.PayBankId,
                  Amount = (decimal)d.RecAmt,
                  TransactionDate = d.OrderDate,
                  TransactionType = EnumTransactionType.Withdraw
              })
              .ToList();
            allBankTransaction.AddRange(allPOBank);

            var allSalesBank = _salesOrderRepository.All.Where(d => d.Status == 1 && d.PayBankId > 0 && DbFunctions.TruncateTime(d.InvoiceDate) <= DbFunctions.TruncateTime(toDate))
              .OrderBy(d => d.PayBankId)
              .ThenBy(d => d.InvoiceDate)
              .Select(d => new RecPayTransactionTO
              {
                  BankId = d.PayBankId,
                  Amount = (decimal)d.RecAmount,
                  TransactionDate = d.InvoiceDate,
                  TransactionType = EnumTransactionType.Deposit
              })
              .ToList();
            allBankTransaction.AddRange(allSalesBank);

            var allCashDeliveryBank = _cashCollectionRepository.All.Where(d => d.TransactionType == EnumTranType.ToCompany && d.PayBankId > 0 && DbFunctions.TruncateTime(d.EntryDate) <= DbFunctions.TruncateTime(toDate))
              .OrderBy(d => d.PayBankId)
              .ThenBy(d => d.EntryDate)
              .Select(d => new RecPayTransactionTO
              {
                  BankId = d.PayBankId,
                  Amount = (decimal)d.Amount,
                  TransactionDate = (DateTime)d.EntryDate,
                  TransactionType = EnumTransactionType.Withdraw
              })
              .ToList();
            allBankTransaction.AddRange(allCashDeliveryBank);

            var allCashCollectionBank = _cashCollectionRepository.All.Where(d => d.TransactionType == EnumTranType.FromCustomer && d.PayBankId > 0 && DbFunctions.TruncateTime(d.EntryDate) <= DbFunctions.TruncateTime(toDate))
              .OrderBy(d => d.PayBankId)
              .ThenBy(d => d.EntryDate)
              .Select(d => new RecPayTransactionTO
              {
                  BankId = d.PayBankId,
                  Amount = (decimal)d.Amount,
                  TransactionDate = (DateTime)d.EntryDate,
                  TransactionType = EnumTransactionType.Deposit
              })
              .ToList();
            allBankTransaction.AddRange(allCashCollectionBank);

            var allCashCollectionReturnBank = _cashCollectionRepository.All.Where(d => d.TransactionType == EnumTranType.CollectionReturn && d.PayBankId > 0 && DbFunctions.TruncateTime(d.EntryDate) <= DbFunctions.TruncateTime(toDate))
              .OrderBy(d => d.PayBankId)
              .ThenBy(d => d.EntryDate)
              .Select(d => new RecPayTransactionTO
              {
                  BankId = d.PayBankId,
                  Amount = (decimal)d.Amount,
                  TransactionDate = (DateTime)d.EntryDate,
                  TransactionType = EnumTransactionType.Withdraw
              })
              .ToList();
            allBankTransaction.AddRange(allCashCollectionReturnBank);

            var allExpenseBank = _expenseditureRepository.All.Where(d => d.ExpenseIncomeStatus == EnumCompanyTransaction.Expense && d.PayBankId > 0 && DbFunctions.TruncateTime(d.EntryDate) <= DbFunctions.TruncateTime(toDate))
              .OrderBy(d => d.PayBankId)
              .ThenBy(d => d.EntryDate)
              .Select(d => new RecPayTransactionTO
              {
                  BankId = d.PayBankId,
                  Amount = (decimal)d.Amount,
                  TransactionDate = d.EntryDate,
                  TransactionType = EnumTransactionType.Withdraw
              })
              .ToList();
            allBankTransaction.AddRange(allExpenseBank);

            var allIncomeBank = _expenseditureRepository.All.Where(d => d.ExpenseIncomeStatus == EnumCompanyTransaction.Income && d.PayBankId > 0 && DbFunctions.TruncateTime(d.EntryDate) <= DbFunctions.TruncateTime(toDate))
              .OrderBy(d => d.PayBankId)
              .ThenBy(d => d.EntryDate)
              .Select(d => new RecPayTransactionTO
              {
                  BankId = d.PayBankId,
                  Amount = (decimal)d.Amount,
                  TransactionDate = d.EntryDate,
                  TransactionType = EnumTransactionType.Deposit
              })
              .ToList();
            allBankTransaction.AddRange(allIncomeBank);

            var allLiaPayBank = _shareInvestmentRepository.All.Where(d => d.TransactionType == EnumInvestTransType.Pay && (d.LiabilityType == EnumLiabilityType.GiveLoan || d.LiabilityType == EnumLiabilityType.LoanPay) && d.PayBankId > 0 && DbFunctions.TruncateTime(d.EntryDate) <= DbFunctions.TruncateTime(toDate))
              .OrderBy(d => d.PayBankId)
              .ThenBy(d => d.EntryDate)
              .Select(d => new RecPayTransactionTO
              {
                  BankId = d.PayBankId,
                  Amount = (decimal)d.Amount,
                  TransactionDate = d.EntryDate,
                  TransactionType = EnumTransactionType.Withdraw
              })
              .ToList();
            allBankTransaction.AddRange(allLiaPayBank);

            var allLiaRecBank = _shareInvestmentRepository.All.Where(d => d.TransactionType == EnumInvestTransType.Receive && (d.LiabilityType == EnumLiabilityType.TakeLoan || d.LiabilityType == EnumLiabilityType.LoanCollection) && d.PayBankId > 0 && DbFunctions.TruncateTime(d.EntryDate) <= DbFunctions.TruncateTime(toDate))
              .OrderBy(d => d.PayBankId)
              .ThenBy(d => d.EntryDate)
              .Select(d => new RecPayTransactionTO
              {
                  BankId = d.PayBankId,
                  Amount = (decimal)d.Amount,
                  TransactionDate = d.EntryDate,
                  TransactionType = EnumTransactionType.Deposit
              })
              .ToList();
            allBankTransaction.AddRange(allLiaRecBank);

            var bankGroups = allBankTransaction.GroupBy(d => d.BankId);
            serial = result.Count > 0 ? result.Max(r => r.Id) + 1 : 1;
            foreach (var bankGroup in bankGroups)
            {
                int bankId = bankGroup.Key.Value;
                Bank bank = _bankRepository.FindBy(d => d.BankID == bankId).FirstOrDefault();
                string bName = bank.BankName;

                decimal opening = bank.OpeningBalance;
                decimal closing = opening;

                foreach (var bankTr in bankGroup.OrderBy(d => d.TransactionDate))
                {
                    //closing = opening;
                    if (bankTr.TransactionDate < fromDate)
                    {
                        if (bankTr.TransactionType == EnumTransactionType.Deposit)
                        {
                            opening += bankTr.Amount;
                            closing = opening;
                        }
                        else if (bankTr.TransactionType == EnumTransactionType.Withdraw)
                        {
                            opening -= bankTr.Amount;
                            closing = opening;
                        }
                    }

                    if (bankTr.TransactionDate >= fromDate)
                    {
                        //closing = opening;
                        if (bankTr.TransactionType == EnumTransactionType.Deposit)
                        {
                            closing += bankTr.Amount;
                        }
                        else if (bankTr.TransactionType == EnumTransactionType.Withdraw)
                        {
                            closing -= bankTr.Amount;
                        }
                    }
                    else
                    {
                        closing = opening;
                    }

                }

                // Add the final opening data to the result list
                RPTPayRecTO openingData = new RPTPayRecTO
                {
                    Id = serial,
                    DebitAmount = opening,
                    CreditAmount = 0m,
                    IsClosing = true,
                    DebitParticular = bName,
                    ClosingBalance = closing,
                    IsDrHeader = true,
                    BankDebitAmount = opening
                };
                result.Add(openingData);
                serial++;
            }
            #region get bank without transactions
            if (bankGroups != null && bankGroups.Any())
            {
                List<int?> allBankTrBankIds = bankGroups.Select(d => d.Key).ToList();
                List<Bank> allBankWithNoTR = _bankRepository.All.Where(d => !allBankTrBankIds.Contains(d.BankID)).ToList();
                if (allBankWithNoTR != null && allBankWithNoTR.Any())
                {
                    serial = result.Count > 0 ? result.Max(r => r.Id) + 1 : 1;
                    foreach (var bank in allBankWithNoTR)
                    {
                        result.Add(new RPTPayRecTO
                        {
                            Id = serial,
                            DebitAmount = bank.OpeningBalance,
                            CreditAmount = 0m,
                            IsClosing = true,
                            DebitParticular = bank.BankName,
                            ClosingBalance = bank.OpeningBalance,
                            IsDrHeader = true,
                            BankDebitAmount = bank.OpeningBalance
                        });
                        serial++;
                    }
                }
            }

            #endregion
            #endregion

            serial = result.Count > 0 ? result.Max(r => r.Id) + 1 : 1;

            #region Customer Data

            var CashsalesDataList = (from s in _salesOrderRepository.All
                                     join c in _customerRepository.All on s.CustomerID equals c.CustomerID
                                     where s.Status == 1 && s.PayCashAccountId > 0 && DbFunctions.TruncateTime(s.InvoiceDate) >= DbFunctions.TruncateTime(fromDate) && DbFunctions.TruncateTime(s.InvoiceDate) <= DbFunctions.TruncateTime(toDate)
                                     select new
                                     {
                                         c.CustomerID,
                                         Amount = (decimal)s.RecAmount,
                                         c.Name,
                                         VoucherDate = s.InvoiceDate,
                                         BankAmt = 0m,
                                     }).ToList();

            var BanksalesDataList = (from s in _salesOrderRepository.All
                                     join c in _customerRepository.All on s.CustomerID equals c.CustomerID
                                     where s.Status == 1 && s.PayBankId > 0 && DbFunctions.TruncateTime(s.InvoiceDate) >= DbFunctions.TruncateTime(fromDate) && DbFunctions.TruncateTime(s.InvoiceDate) <= DbFunctions.TruncateTime(toDate)
                                     select new
                                     {
                                         c.CustomerID,
                                         Amount = (decimal)s.RecAmount,
                                         c.Name,
                                         VoucherDate = s.InvoiceDate,
                                         BankAmt = (decimal)s.RecAmount,
                                     }).ToList();

            var CashCollectionDataList = (from v in _cashCollectionRepository.All
                                          join c in _customerRepository.All on v.CustomerID equals c.CustomerID
                                          where v.TransactionType == EnumTranType.FromCustomer && v.PayCashAccountId > 0 && DbFunctions.TruncateTime(v.EntryDate) >= DbFunctions.TruncateTime(fromDate) && DbFunctions.TruncateTime(v.EntryDate) <= DbFunctions.TruncateTime(toDate)
                                          select new
                                          {
                                              c.CustomerID,
                                              v.Amount,
                                              c.Name,
                                              VoucherDate = (DateTime)v.EntryDate,
                                              BankAmt = 0m
                                          }).ToList();


            var CashCollectionBankDataList = (from v in _cashCollectionRepository.All
                                              join c in _customerRepository.All on v.CustomerID equals c.CustomerID
                                              where v.TransactionType == EnumTranType.FromCustomer && v.PayBankId > 0 && DbFunctions.TruncateTime(v.EntryDate) >= DbFunctions.TruncateTime(fromDate) && DbFunctions.TruncateTime(v.EntryDate) <= DbFunctions.TruncateTime(toDate)
                                              select new
                                              {
                                                  c.CustomerID,
                                                  v.Amount,
                                                  c.Name,
                                                  VoucherDate = (DateTime)v.EntryDate,
                                                  BankAmt = v.Amount
                                              }).ToList();


            CashsalesDataList.AddRange(BanksalesDataList);
            CashsalesDataList.AddRange(CashCollectionDataList);
            CashsalesDataList.AddRange(CashCollectionBankDataList);

            if (CashsalesDataList.Any())
            {
                var saleGroups = CashsalesDataList.GroupBy(d => d.CustomerID);

                foreach (var sales in saleGroups)
                {
                    result.Add(new RPTPayRecTO
                    {
                        Id = serial,
                        DebitAmount = sales.Sum(d => (decimal)d.Amount),
                        DebitParticular = sales.First().Name,
                        ClosingBalance = 0m,
                        CreditAmount = 0m,
                        IsClosing = false,
                        TransactionDate = DateTime.MinValue,
                        BankRecAmount = sales.Sum(d => (decimal)d.BankAmt),
                    });
                    serial++;
                }
            }

            #endregion

            #region SalesData Bank

            //var BanksalesDataList = (from s in _salesOrderRepository.All
            //                         join c in _customerRepository.All on s.CustomerID equals c.CustomerID
            //                         where s.Status == 1 && s.PayBankId > 0 && DbFunctions.TruncateTime(s.InvoiceDate) >= DbFunctions.TruncateTime(fromDate) && DbFunctions.TruncateTime(s.InvoiceDate) <= DbFunctions.TruncateTime(toDate)
            //                         select new
            //                         {
            //                             c.CustomerID,
            //                             s.RecAmount,
            //                             c.Name,
            //                             s.InvoiceDate
            //                         }).ToList();

            //if (BanksalesDataList.Any())
            //{
            //    var saleGroups = BanksalesDataList.GroupBy(d => d.CustomerID);

            //    foreach (var sales in saleGroups)
            //    {
            //        result.Add(new RPTPayRecTO
            //        {
            //            Id = serial,
            //            DebitAmount = sales.Sum(d => (decimal)d.RecAmount),
            //            DebitParticular = sales.First().Name,
            //            ClosingBalance = 0m,
            //            CreditAmount = 0m,
            //            IsClosing = false,
            //            TransactionDate = DateTime.MinValue,
            //            BankRecAmount = sales.Sum(d => (decimal)d.RecAmount),
            //        });
            //        serial++;
            //    }
            //}

            #endregion

            #region BankWithdraw

            var bankWithdrawDataList = (from vt in _bankTransactionRepository.All.Where(d => d.TransactionType == (int)EnumTransactionType.Withdraw)
                                        join b in _bankRepository.All on vt.BankID equals b.BankID
                                        where DbFunctions.TruncateTime(vt.TranDate) >= DbFunctions.TruncateTime(fromDate) && DbFunctions.TruncateTime(vt.TranDate) <= DbFunctions.TruncateTime(toDate)
                                        select new
                                        {
                                            b.BankID,
                                            vt.Amount,
                                            b.BankName,
                                            vt.TranDate
                                        }).ToList();


            if (bankWithdrawDataList.Any())
            {
                var bankWithdrawDataGroups = bankWithdrawDataList.GroupBy(d => d.BankID);

                foreach (var bankWithdraw in bankWithdrawDataGroups)
                {
                    serial++;
                    RPTPayRecTO openingData = new RPTPayRecTO
                    {
                        Id = serial,
                        DebitAmount = bankWithdraw.Sum(d => d.Amount),
                        DebitParticular = bankWithdraw.First().BankName + "- Withdraw",
                        ClosingBalance = 0m,
                        CreditAmount = 0m,
                        IsClosing = false,
                        //TransactionDate = DateTime.MinValue,
                        //BankDebitAmount = bankWithdraw.Sum(d => d.Amount)
                    };
                    result.Add(openingData);

                }
            }

            #endregion

            #region Income

            var incomeDataList = (from v in _expenseditureRepository.All
                                  join e in _expenseRepository.All on v.ExpenseItemID equals e.ExpenseItemID
                                  where v.PayCashAccountId > 0 && e.Status == EnumCompanyTransaction.Income && DbFunctions.TruncateTime(v.EntryDate) >= DbFunctions.TruncateTime(fromDate) && DbFunctions.TruncateTime(v.EntryDate) <= DbFunctions.TruncateTime(toDate)
                                  select new
                                  {
                                      v.Amount,
                                      e.ExpenseItemID,
                                      e.Description,
                                      v.EntryDate,
                                      BankAmt = 0m,
                                      IncomeJournal = 0m
                                  }).ToList();

            var incomeBakDataList = (from v in _expenseditureRepository.All
                                     join e in _expenseRepository.All on v.ExpenseItemID equals e.ExpenseItemID
                                     where v.PayBankId > 0 && e.Status == EnumCompanyTransaction.Income && DbFunctions.TruncateTime(v.EntryDate) >= DbFunctions.TruncateTime(fromDate) && DbFunctions.TruncateTime(v.EntryDate) <= DbFunctions.TruncateTime(toDate)
                                     select new
                                     {
                                         v.Amount,
                                         e.ExpenseItemID,
                                         e.Description,
                                         v.EntryDate,
                                         BankAmt = v.Amount,
                                         IncomeJournal = 0m
                                     }).ToList();
            incomeDataList.AddRange(incomeBakDataList);


            if (incomeDataList.Any())
            {
                serial++;
                var incomeGroup = incomeDataList.GroupBy(d => d.ExpenseItemID);
                foreach (var income in incomeGroup)
                {

                    RPTPayRecTO openingData = new RPTPayRecTO
                    {

                        Id = serial,
                        DebitAmount = income.Sum(d => d.Amount),
                        CreditAmount = 0m,
                        IsClosing = false,
                        DebitParticular = income.First().Description,
                        ClosingBalance = 0m,
                        BankRecAmount = income.Sum(d => d.BankAmt),
                        IncomeJournalAmount = income.Sum(d => d.IncomeJournal)
                        //CashJournalDebitAmount = income.Sum(d => d.Amount)

                    };
                    result.Add(openingData);
                }
            }

            #endregion

            #region Investment
            var investmentRecDataList = (from v in _shareInvestmentRepository.All
                                         join i in _investmentHeadRepository.All on v.SIHID equals i.SIHID
                                         where (v.LiabilityType == EnumLiabilityType.TakeLoan || v.LiabilityType == EnumLiabilityType.LoanCollection) && v.PayCashAccountId > 0 && DbFunctions.TruncateTime(v.EntryDate) >= DbFunctions.TruncateTime(fromDate) && DbFunctions.TruncateTime(v.EntryDate) <= DbFunctions.TruncateTime(toDate)
                                         select new
                                         {
                                             v.Amount,
                                             i.SIHID,
                                             i.Name,
                                             v.EntryDate,
                                             BankAmt = 0m
                                         }).ToList();

            var investmentRecBankDataList = (from v in _shareInvestmentRepository.All
                                             join i in _investmentHeadRepository.All on v.SIHID equals i.SIHID
                                             where (v.LiabilityType == EnumLiabilityType.TakeLoan || v.LiabilityType == EnumLiabilityType.LoanCollection) && v.PayBankId > 0 && DbFunctions.TruncateTime(v.EntryDate) >= DbFunctions.TruncateTime(fromDate) && DbFunctions.TruncateTime(v.EntryDate) <= DbFunctions.TruncateTime(toDate)
                                             select new
                                             {
                                                 v.Amount,
                                                 i.SIHID,
                                                 i.Name,
                                                 v.EntryDate,
                                                 BankAmt = v.Amount
                                             }).ToList();

            investmentRecDataList.AddRange(investmentRecBankDataList);

            if (investmentRecDataList.Any())
            {
                serial++;
                var investmentGroup = investmentRecDataList.GroupBy(d => d.SIHID);
                foreach (var invest in investmentGroup)
                {

                    RPTPayRecTO openingData = new RPTPayRecTO
                    {

                        Id = serial,
                        DebitAmount = invest.Sum(d => d.Amount),
                        CreditAmount = 0m,
                        IsClosing = false,
                        DebitParticular = invest.First().Name,
                        ClosingBalance = 0m,
                        BankRecAmount = invest.Sum(d => d.BankAmt),

                    };
                    result.Add(openingData);
                }
            }



            #endregion


            #endregion Debit

            #region Credit

            List<RPTPayRecTO> crResult = new List<RPTPayRecTO>();

            #region supplier
            var supplierDataList = (from v in _cashCollectionRepository.All
                                    join s in _supplierRepository.All on v.SupplierID equals s.SupplierID
                                    where v.TransactionType == EnumTranType.ToCompany && v.PayCashAccountId > 0 && DbFunctions.TruncateTime(v.EntryDate) >= DbFunctions.TruncateTime(fromDate) && DbFunctions.TruncateTime(v.EntryDate) <= DbFunctions.TruncateTime(toDate)
                                    select new
                                    {
                                        v.Amount,
                                        s.SupplierID,
                                        s.Name,
                                        VoucherDate = (DateTime)v.EntryDate,
                                        BankAmt = 0m
                                    }).ToList();

            var supplierDeliveryBankDataList = (from v in _cashCollectionRepository.All
                                                join s in _supplierRepository.All on v.SupplierID equals s.SupplierID
                                                where v.TransactionType == EnumTranType.ToCompany && v.PayBankId > 0 && DbFunctions.TruncateTime(v.EntryDate) >= DbFunctions.TruncateTime(fromDate) && DbFunctions.TruncateTime(v.EntryDate) <= DbFunctions.TruncateTime(toDate)
                                                select new
                                                {
                                                    v.Amount,
                                                    s.SupplierID,
                                                    s.Name,
                                                    VoucherDate = (DateTime)v.EntryDate,
                                                    BankAmt = v.Amount
                                                }).ToList();


            var supplierPODataList = (from v in _pOrderRepository.All
                                      join s in _supplierRepository.All on v.SupplierID equals s.SupplierID
                                      where v.Status == (int)EnumPurchaseType.Purchase && v.RecAmt > 0 && v.PayCashAccountId > 0 && DbFunctions.TruncateTime(v.OrderDate) >= DbFunctions.TruncateTime(fromDate) && DbFunctions.TruncateTime(v.OrderDate) <= DbFunctions.TruncateTime(toDate)
                                      select new
                                      {
                                          Amount = v.RecAmt,
                                          s.SupplierID,
                                          s.Name,
                                          VoucherDate = v.OrderDate,
                                          BankAmt = 0m,
                                      }).ToList();
            var supplierPOBankDataList = (from v in _pOrderRepository.All
                                          join s in _supplierRepository.All on v.SupplierID equals s.SupplierID
                                          where v.Status == (int)EnumPurchaseType.Purchase && v.RecAmt > 0 && v.PayBankId > 0 && DbFunctions.TruncateTime(v.OrderDate) >= DbFunctions.TruncateTime(fromDate) && DbFunctions.TruncateTime(v.OrderDate) <= DbFunctions.TruncateTime(toDate)
                                          select new
                                          {
                                              Amount = v.RecAmt,
                                              s.SupplierID,
                                              s.Name,
                                              VoucherDate = v.OrderDate,
                                              BankAmt = v.RecAmt,
                                          }).ToList();
            supplierDataList.AddRange(supplierDeliveryBankDataList);
            supplierDataList.AddRange(supplierPODataList);
            supplierDataList.AddRange(supplierPOBankDataList);


            int crSerial = 1;
            if (supplierDataList.Any())
            {
                var supplierGroup = supplierDataList.GroupBy(d => d.SupplierID);
                foreach (var supplier in supplierGroup)
                {
                    RPTPayRecTO openingData = new RPTPayRecTO
                    {
                        Id = crSerial,
                        DebitAmount = 0m,
                        CreditAmount = supplier.Sum(d => d.Amount),
                        IsClosing = false,
                        CreditParticular = supplier.First().Name,
                        ClosingBalance = 0m,
                        BankPayAmount = supplier.Sum(d => d.BankAmt),
                    };
                    crResult.Add(openingData);
                    crSerial++;
                }
            }

            #endregion


            #region Customer Collection Return
            var CollectionReturnDataList = (from v in _cashCollectionRepository.All
                                            join s in _customerRepository.All on v.CustomerID equals s.CustomerID
                                            where v.TransactionType == EnumTranType.CollectionReturn && v.PayCashAccountId > 0 && DbFunctions.TruncateTime(v.EntryDate) >= DbFunctions.TruncateTime(fromDate) && DbFunctions.TruncateTime(v.EntryDate) <= DbFunctions.TruncateTime(toDate)
                                            select new
                                            {
                                                v.Amount,
                                                s.CustomerID,
                                                s.Name,
                                                VoucherDate = (DateTime)v.EntryDate,
                                                BankAmt = 0m
                                            }).ToList();

            var CollectionReturnBankDataList = (from v in _cashCollectionRepository.All
                                                join s in _customerRepository.All on v.CustomerID equals s.CustomerID
                                                where v.TransactionType == EnumTranType.CollectionReturn && v.PayBankId > 0 && DbFunctions.TruncateTime(v.EntryDate) >= DbFunctions.TruncateTime(fromDate) && DbFunctions.TruncateTime(v.EntryDate) <= DbFunctions.TruncateTime(toDate)
                                                select new
                                                {
                                                    v.Amount,
                                                    s.CustomerID,
                                                    s.Name,
                                                    VoucherDate = (DateTime)v.EntryDate,
                                                    BankAmt = v.Amount
                                                }).ToList();

            CollectionReturnDataList.AddRange(CollectionReturnBankDataList);

            if (CollectionReturnDataList.Any())
            {
                crSerial = crResult.Count > 0 ? crResult.Max(r => r.Id) + 1 : 1;
                var cusGroup = CollectionReturnDataList.GroupBy(d => d.CustomerID);
                foreach (var cus in cusGroup)
                {
                    RPTPayRecTO openingData = new RPTPayRecTO
                    {
                        Id = crSerial,
                        DebitAmount = 0m,
                        CreditAmount = cus.Sum(d => d.Amount),
                        IsClosing = false,
                        CreditParticular = cus.First().Name,
                        ClosingBalance = 0m,
                        BankPayAmount = cus.Sum(d => d.BankAmt),
                    };
                    crResult.Add(openingData);
                    crSerial++;
                }
            }

            #endregion

            #region BankDeposit
            var bankDepoDataList = (from vt in _bankTransactionRepository.All.Where(d => d.TransactionType == (int)EnumTransactionType.Deposit)
                                    join b in _bankRepository.All on vt.BankID equals b.BankID
                                    where DbFunctions.TruncateTime(vt.TranDate) >= DbFunctions.TruncateTime(fromDate) && DbFunctions.TruncateTime(vt.TranDate) <= DbFunctions.TruncateTime(toDate)
                                    select new
                                    {
                                        b.BankID,
                                        vt.Amount,
                                        b.BankName,
                                        vt.TranDate,
                                    }).ToList();


            if (bankDepoDataList.Any())
            {
                crSerial = crResult.Count > 0 ? crResult.Max(r => r.Id) + 1 : 1;
                var bankDepoDataGroups = bankDepoDataList.GroupBy(d => d.BankID);

                foreach (var bankDepo in bankDepoDataGroups)
                {
                    RPTPayRecTO openingData = new RPTPayRecTO
                    {
                        Id = crSerial,
                        DebitAmount = 0m,
                        CreditAmount = bankDepo.Sum(d => d.Amount),
                        IsClosing = false,
                        CreditParticular = bankDepo.First().BankName + "- Deposit",
                        ClosingBalance = 0m,
                        BankRecAmount = bankDepo.Sum(d => d.Amount),
                    };
                    crResult.Add(openingData);
                    crSerial++;
                }
            }

            #endregion


            #region expense

            var expenseDataList = (from v in _expenseditureRepository.All
                                   join e in _expenseRepository.All on v.ExpenseItemID equals e.ExpenseItemID
                                   where v.PayCashAccountId > 0 && e.Status == EnumCompanyTransaction.Expense && DbFunctions.TruncateTime(v.EntryDate) >= DbFunctions.TruncateTime(fromDate) && DbFunctions.TruncateTime(v.EntryDate) <= DbFunctions.TruncateTime(toDate)
                                   select new
                                   {
                                       v.Amount,
                                       e.ExpenseItemID,
                                       e.Description,
                                       v.EntryDate,
                                       BankAmt = 0m,
                                       ExpeJournal = 0m
                                   }).ToList();

            var expenseBakDataList = (from v in _expenseditureRepository.All
                                      join e in _expenseRepository.All on v.ExpenseItemID equals e.ExpenseItemID
                                      where v.PayBankId > 0 && e.Status == EnumCompanyTransaction.Expense && DbFunctions.TruncateTime(v.EntryDate) >= DbFunctions.TruncateTime(fromDate) && DbFunctions.TruncateTime(v.EntryDate) <= DbFunctions.TruncateTime(toDate)
                                      select new
                                      {
                                          v.Amount,
                                          e.ExpenseItemID,
                                          e.Description,
                                          v.EntryDate,
                                          BankAmt = v.Amount,
                                          ExpeJournal = 0m
                                      }).ToList();
            expenseDataList.AddRange(expenseBakDataList);


            if (expenseDataList.Any())
            {
                crSerial = crResult.Count > 0 ? crResult.Max(r => r.Id) + 1 : 1;
                var expenseGroup = expenseDataList.GroupBy(d => d.ExpenseItemID);
                foreach (var expense in expenseGroup)
                {

                    RPTPayRecTO openingData = new RPTPayRecTO
                    {
                        Id = crSerial,
                        DebitAmount = 0m,
                        CreditAmount = expense.Sum(d => d.Amount),
                        IsClosing = false,
                        CreditParticular = expense.First().Description,
                        ClosingBalance = 0m,
                        BankPayAmount = expense.Sum(d => d.BankAmt),
                        ExpenseJournalAmount = expense.Sum(d => d.ExpeJournal)
                    };
                    crResult.Add(openingData);
                    crSerial++;
                }
            }

            #endregion

            #region Investment
            var investmentDataList = (from v in _shareInvestmentRepository.All
                                      join i in _investmentHeadRepository.All on v.SIHID equals i.SIHID
                                      where (v.LiabilityType == EnumLiabilityType.GiveLoan || v.LiabilityType == EnumLiabilityType.LoanPay) && v.PayCashAccountId > 0 && DbFunctions.TruncateTime(v.EntryDate) >= DbFunctions.TruncateTime(fromDate) && DbFunctions.TruncateTime(v.EntryDate) <= DbFunctions.TruncateTime(toDate)
                                      select new
                                      {
                                          v.Amount,
                                          i.SIHID,
                                          i.Name,
                                          v.EntryDate,
                                          BankAmt = 0m
                                      }).ToList();

            var investmentBankDataList = (from v in _shareInvestmentRepository.All
                                          join i in _investmentHeadRepository.All on v.SIHID equals i.SIHID
                                          where (v.LiabilityType == EnumLiabilityType.GiveLoan || v.LiabilityType == EnumLiabilityType.LoanPay) && v.PayBankId > 0 && DbFunctions.TruncateTime(v.EntryDate) >= DbFunctions.TruncateTime(fromDate) && DbFunctions.TruncateTime(v.EntryDate) <= DbFunctions.TruncateTime(toDate)
                                          select new
                                          {
                                              v.Amount,
                                              i.SIHID,
                                              i.Name,
                                              v.EntryDate,
                                              BankAmt = v.Amount
                                          }).ToList();

            investmentDataList.AddRange(investmentBankDataList);

            if (investmentDataList.Any())
            {
                crSerial = crResult.Count > 0 ? crResult.Max(r => r.Id) + 1 : 1;
                var investmentGroup = investmentDataList.GroupBy(d => d.SIHID);
                foreach (var investment in investmentGroup)
                {

                    RPTPayRecTO openingData = new RPTPayRecTO
                    {
                        Id = crSerial,
                        DebitAmount = 0m,
                        CreditAmount = investment.Sum(d => d.Amount),
                        IsClosing = false,
                        CreditParticular = investment.First().Name,
                        ClosingBalance = 0m,
                        BankPayAmount = investment.Sum(d => d.BankAmt)
                    };
                    crResult.Add(openingData);
                    crSerial++;
                }
            }

            #endregion

            #endregion Credit

            int drS = 1;
            if (result.Any())
            {
                foreach (var item in result)
                {
                    item.Id = drS;
                    drS++;
                }
                serial = drS;
            }

            #region final data

            var leftJoinResult = (from r in result
                                  join cr in crResult on r.Id equals cr.Id into crGroup
                                  from cr in crGroup.DefaultIfEmpty()
                                  select new RPTPayRecTO
                                  {
                                      Id = r.Id,
                                      DebitParticular = r.DebitParticular,
                                      CreditParticular = cr?.CreditParticular,
                                      DebitAmount = r.DebitAmount,
                                      CreditAmount = cr?.CreditAmount ?? 0m,
                                      IsClosing = r.IsClosing,
                                      ClosingBalance = r.ClosingBalance,
                                      TransactionDate = r.TransactionDate,
                                      IsDrHeader = r.IsDrHeader,
                                      IsCrHeader = cr?.IsCrHeader ?? false,
                                      BankDebitAmount = r.BankDebitAmount,
                                      BankRecAmount = r.BankRecAmount,
                                      ProjectName = cr?.ProjectName,
                                      IsProject = false,
                                      CashJournalDebitAmount = r.CashJournalDebitAmount,
                                      IncomeJournalAmount = r.IncomeJournalAmount
                                  }).OrderBy(i => i.Id);

            var rightJoinResult = from cr in crResult
                                  join r in result on cr.Id equals r.Id into rGroup
                                  from r in rGroup.DefaultIfEmpty()
                                  where r == null
                                  select new RPTPayRecTO
                                  {
                                      Id = cr.Id,
                                      DebitParticular = r?.DebitParticular,
                                      CreditParticular = cr.CreditParticular,
                                      DebitAmount = r?.DebitAmount ?? 0m,
                                      CreditAmount = cr.CreditAmount,
                                      IsClosing = r?.IsClosing ?? false,
                                      ClosingBalance = r?.ClosingBalance ?? 0,
                                      TransactionDate = r?.TransactionDate ?? default(DateTime),
                                      IsDrHeader = r?.IsDrHeader ?? false,
                                      IsCrHeader = cr.IsCrHeader,
                                      BankRecAmount = r?.BankRecAmount ?? 0,
                                      BankPayAmount = cr.BankPayAmount,
                                      ProjectName = cr.ProjectName,
                                      IsProject = cr.IsProject,
                                      CashJournalCreditAmount = cr.CashJournalCreditAmount,
                                      ExpenseJournalAmount = cr.ExpenseJournalAmount,

                                  };

            var finalResult = leftJoinResult.Union(rightJoinResult).ToList();


            if (finalResult.Any())
            {
                finalResult = finalResult.OrderBy(i => i.Id).ThenBy(d => d.ProjectName).ToList();
                decimal totalCredit = crResult.Sum(d => d.CreditAmount);
                crSerial += 1;
                RPTPayRecTO crSubTotalata = new RPTPayRecTO
                {
                    Id = crSerial,
                    DebitAmount = 0m,
                    CreditAmount = totalCredit,
                    IsClosing = false,
                    CreditParticular = "Sub Total",
                    ClosingBalance = 0m,
                    IsCrHeader = true
                };
                finalResult.Add(crSubTotalata);

                decimal grandTotalD = result.Sum(d => d.DebitAmount);
                decimal bankamt = result.Sum(d => d.BankDebitAmount);
                decimal debbankrecamt = result.Sum(d => d.BankRecAmount);
                decimal crebankpayamt = crResult.Sum(d => d.BankPayAmount);
                decimal cashjournaldebitamt = result.Sum(d => d.CashJournalDebitAmount);
                decimal crExpeJournalAmt = crResult.Sum(d => d.ExpenseJournalAmount);
                decimal incomejournalamt = result.Sum(d => d.IncomeJournalAmount);
                decimal cashjournalcreditamt = crResult.Sum(d => d.CashJournalCreditAmount);




                crSerial += 1;
                RPTPayRecTO CashClosingData = new RPTPayRecTO
                {
                    Id = crSerial,
                    DebitAmount = 0m,
                    CreditAmount = (grandTotalD - totalCredit) - bankamt - debbankrecamt + crebankpayamt - cashjournalcreditamt + cashjournaldebitamt - incomejournalamt + crExpeJournalAmt/*(grandTotalD - bankamt) - totalCredit*/,
                    IsClosing = true,
                    CreditParticular = "Cash In Hand (Closing)",
                    ClosingBalance = 0m,
                    IsCrHeader = true
                };
                finalResult.Add(CashClosingData);

                var allBankClosing = result.Where(d => d.IsClosing && d.ClosingBalance > 0).ToList();
                if (allBankClosing.Any())
                {
                    foreach (var bClosing in allBankClosing)
                    {
                        crSerial++;
                        RPTPayRecTO bankClosingData = new RPTPayRecTO
                        {
                            Id = crSerial,
                            DebitAmount = 0m,
                            CreditAmount = bClosing.ClosingBalance,
                            IsClosing = true,
                            CreditParticular = bClosing.DebitParticular + "(Closing)",
                            ClosingBalance = 0m,
                            IsCrHeader = true
                        };

                        finalResult.Add(bankClosingData);
                    }
                }


                decimal grandTotal = result.Sum(d => d.DebitAmount);
                crSerial += 1;
                RPTPayRecTO closingBalanceData = new RPTPayRecTO
                {
                    Id = crSerial,
                    DebitAmount = 0m,
                    CreditAmount = grandTotal - totalCredit,
                    IsClosing = true,
                    CreditParticular = "Closing Balance",
                    ClosingBalance = 0m,
                    IsCrHeader = true
                };
                finalResult.Add(closingBalanceData);

            }

            #endregion

            return finalResult.Where(d => d.DebitAmount != 0 || d.CreditAmount != 0 || d.IsCrHeader).ToList();


        }

        public static List<SummaryReportModel> GetSummaryReport(this IBaseRepository<SOrder> SOrderRepository, IBaseRepository<SOrderDetail> SOrderDetailRepository,
                    IBaseRepository<Customer> CustomerRepository, IBaseRepository<BankTransaction> BankTransactionRepository,
                    IBaseRepository<CashCollection> CashCollectionRepository, IBaseRepository<CreditSale> CreditSaleRepository,
                    IBaseRepository<CreditSaleDetails> CreditSaleDetailsRepository, IBaseRepository<CreditSalesSchedule> CreditSalesScheduleRepository,
                    IBaseRepository<Product> ProductRepository, IBaseRepository<Category> CategoryRepository, IBaseRepository<Expenditure> _expenseditureRepository,
                    IBaseRepository<ExpenseItem> _expenseRepository, DateTime fromDate, DateTime toDate, int ConcernID)
        {
            List<SummaryReportModel> summaryData = new List<SummaryReportModel>();

            var SOrders = SOrderRepository.GetAll().Where(i => i.ConcernID == ConcernID);
            var SOrderDetails = SOrderDetailRepository.All;
            var CreditSales = CreditSaleRepository.GetAll().Where(i => i.ConcernID == ConcernID && i.IsStatus == EnumSalesType.Sales);
            var CreditSchedules = CreditSalesScheduleRepository.All;
            var CreditDetails = CreditSaleDetailsRepository.All;
            var CashCollections = CashCollectionRepository.GetAll().Where(i => i.ConcernID == ConcernID && i.CustomerID > 0 && i.TransactionType == EnumTranType.FromCustomer);
            //DateTime StartDate = new DateTime(2000, 1, 1);
            var Products = ProductRepository.GetAll().Where(i => i.ConcernID == ConcernID);
            var Categories = CategoryRepository.GetAll().Where(i => i.ConcernID == ConcernID);
            var Expenditures = _expenseditureRepository.GetAll().Where(i => i.ConcernID == ConcernID);
            var ExpenseItems = _expenseRepository.GetAll().Where(i => i.ConcernID == ConcernID);



            #region Sales Value
            var SalesCategories = (from SO in SOrders
                                   join SOD in SOrderDetails on SO.SOrderID equals SOD.SOrderID
                                   where SO.Status == (int)EnumSalesType.Sales && SO.InvoiceDate >= fromDate && SO.InvoiceDate <= toDate
                                   select new SummaryReportModel
                                   {
                                       Head = "Sales",
                                       Amount = (SOD.Quantity * ((SOD.UnitPrice - SOD.PPDAmount) - (((SOD.UnitPrice - SOD.PPDAmount) * (SO.TDAmount + SO.AdjAmount)) / (SO.GrandTotal - SO.NetDiscount + SO.TDAmount)))),
                                       Category = "Revenue"
                                   }).ToList();

            var gCategoryCredit = from s in SalesCategories
                                  group s by new { s.Head, Catgory = s.Category } into g
                                  select new SummaryReportModel
                                  {
                                      Category = g.Key.Catgory,
                                      Head = g.Key.Head,
                                      Amount = g.Sum(i => i.Amount)

                                  };

            summaryData.AddRange(gCategoryCredit);


            #endregion

            #region Sales Cost value

            var PurchaseCategories = (from SO in SOrders
                                      join SOD in SOrderDetails on SO.SOrderID equals SOD.SOrderID
                                      where SO.Status == (int)EnumSalesType.Sales && SO.InvoiceDate >= fromDate && SO.InvoiceDate <= toDate
                                      select new SummaryReportModel
                                      {
                                          Head = "Cost of Sales",
                                          Amount = SOD.PRate * SOD.Quantity,
                                          Category = "Revenue"
                                      }).ToList();

            var gPurchaseCategoryCredit = from s in PurchaseCategories
                                          group s by new { s.Head, Catgory = s.Category } into g
                                          select new SummaryReportModel
                                          {
                                              Category = g.Key.Catgory,
                                              Head = g.Key.Head,
                                              Amount = g.Sum(i => i.Amount)
                                          };

            summaryData.AddRange(gPurchaseCategoryCredit);


            #endregion

            #region Expense

            //var ExpenseData = (from exp in Expenditures
            //                   join exi in ExpenseItems on exp.ExpenseItemID equals exi.ExpenseItemID
            //                   where exi.Status == EnumCompanyTransaction.Expense && exp.EntryDate >= fromDate && exp.EntryDate <= toDate
            //                   select new SummaryReportModel
            //                   {
            //                       id = exi.ExpenseItemID,
            //                       Head = exi.Description,
            //                       Amount = exp.Amount,
            //                       Category = "Operating Expense"
            //                   }).ToList();

            //var gExpenseData = from s in ExpenseData
            //                   group s by new { s.Head, s.id, Catgory = s.Category } into g
            //                   select new SummaryReportModel
            //                   {
            //                       id = g.Key.id,
            //                       Category = g.Key.Catgory,
            //                       Head = g.Key.Head,
            //                       Amount = g.Sum(i => i.Amount)
            //                   };

            //summaryData.AddRange(gExpenseData);


            #endregion

            return summaryData;
        }



    }
}
