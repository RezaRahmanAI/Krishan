using IMSWEB.Model;
using IMSWEB.Model.SPModel;
using IMSWEB.Report.DataSets;
using IMSWEB.Service;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace IMSWEB.Report
{
    public class TransactionalReport : ITransactionalReport
    {
        DataSet _dataSet = null;
        ReportParameter _reportParameter = null;
        List<ReportParameter> _reportParameters = null;

        IExpenditureService _expenditureService;
        ISalesOrderService _salesOrderService;
        ISalesOrderDetailService _salesOrderDetailService;
        IPurchaseOrderService _purchaseOrderService;
        ICustomerService _customerService;
        IProductService _productService;
        ICreditSalesOrderService _creditSalesOrderService;
        IStockDetailService _stockdetailService;
        IStockService _StockServce;
        IColorService _ColorServce;
        ICashCollectionService _CashCollectionService;
        ISystemInformationService _systemInformationService;
        ISRVisitService _SRVisitService;
        ISRVisitDetailService _SRVisitDetailService;
        IEmployeeService _EmployeeService;
        ISRVProductDetailService _SRVProductDetailService;
        ICategoryService _CategoryService;
        ICompanyService _CompanyService;
        IUserService _userService;
        ISupplierService _SupplierService;
        IPurchaseOrderDetailService _PurchaseOrderDetailService;
        IBankTransactionService _bankTransactionService;
        IPOProductDetailService _POProductDetailService;
        IDesignationService _DesignationService;
        ISalaryMonthlyService _SalaryMonthlyService;
        IDepartmentService _DepartmentService;
        IGradeService _GradeService;
        IEmpGradeSalaryAssignmentService _GradeSalaryAssignment;
        IBankService _BankService;
        IExpenseItemService _ExpenseItemService;
        IAttendenceService _attendenceService;

        IROrderService _returnOrderService;
        IROrderDetailService _returnDetailOrderService;

        IAccountingService _AccountingService;
        IShareInvestmentService _ShareInvestmentService;
        IShareInvestmentHeadService _ShareInvestmentHeadService;
        ITerritoryService _TerritoryServiceService;
        IDepotService _DepotService;
        IProductionSetupService _productionSetupService;
        IProductionService _productionService;



        public TransactionalReport(IExpenditureService expenditureService, ICustomerService customerService, IPurchaseOrderService purchaseOrderService, IBankTransactionService bankTransactionService, ICreditSalesOrderService creditSalesOrderService,
            ISalesOrderService salesOrderService, ISalesOrderDetailService salesOrderDetailService, IProductService productService, IStockDetailService stockDetailService, IStockService stockService, ICashCollectionService cashCollectionService,
            IColorService colorServce, ISystemInformationService systemInformationService,
            ISRVisitService srVisitService, ISRVisitDetailService srVisitDetailService, IEmployeeService employeeService,
            ISRVProductDetailService srVProductDetailService, ISupplierService SupplierService,
            ICategoryService categoryService, ICompanyService companyService, IUserService userservice, IPurchaseOrderDetailService PurchaseOrderDetailService,
            IPOProductDetailService POProductDetailService, IDesignationService DesignationService
            , ISalaryMonthlyService SalaryMonthlyService, IDepartmentService DepartmentService,
            IGradeService GradeService, IEmpGradeSalaryAssignmentService GradeSalaryAssignment, IBankService BankService, IExpenseItemService ExpenseItemService,
            IAttendenceService attendenceService,
            IROrderService returnOrderService,
            IROrderDetailService returnDetailOrderService, IProductionSetupService productionSetupService, IProductionService productionService,
        IAccountingService AccountingService, IShareInvestmentService ShareInvestmentService,
            IShareInvestmentHeadService ShareInvestmentHeadService, ITerritoryService TerritoryService, IDepotService DepotService

            )
        {
            _expenditureService = expenditureService;
            _salesOrderService = salesOrderService;
            _productService = productService;
            _stockdetailService = stockDetailService;
            _customerService = customerService;
            _purchaseOrderService = purchaseOrderService;
            _bankTransactionService = bankTransactionService;
            _StockServce = stockService;
            _systemInformationService = systemInformationService;
            _salesOrderDetailService = salesOrderDetailService;
            _CashCollectionService = cashCollectionService;
            _ColorServce = colorServce;
            _creditSalesOrderService = creditSalesOrderService;
            _SRVisitService = srVisitService;
            _SRVisitDetailService = srVisitDetailService;
            _EmployeeService = employeeService;
            _SRVProductDetailService = srVProductDetailService;
            _CategoryService = categoryService;
            _CompanyService = companyService;
            _userService = userservice;
            _SupplierService = SupplierService;
            _PurchaseOrderDetailService = PurchaseOrderDetailService;
            _POProductDetailService = POProductDetailService;
            _DesignationService = DesignationService;
            _SalaryMonthlyService = SalaryMonthlyService;
            _DepartmentService = DepartmentService;
            _GradeService = GradeService;
            _GradeSalaryAssignment = GradeSalaryAssignment;
            _BankService = BankService;
            _ExpenseItemService = ExpenseItemService;
            _attendenceService = attendenceService;
            _returnOrderService = returnOrderService;
            _returnDetailOrderService = returnDetailOrderService;
            _AccountingService = AccountingService;
            _ShareInvestmentService = ShareInvestmentService;
            _ShareInvestmentHeadService = ShareInvestmentHeadService;
            _TerritoryServiceService = TerritoryService;
            _DepotService = DepotService;
            _productionSetupService = productionSetupService;
            _productionService = productionService;
        }

        public static string TakaFormat(double TotalAmt)
        {

            string sInWords = string.Empty;

            string sPoisa = string.Empty;
            string[] words = TotalAmt.ToString().Split('.');
            string sTaka = words[0];
            if (words.Length == 1)
            {
                sPoisa = "00";
            }
            else
            {
                sPoisa = words[1];
                if (sPoisa.Length == 1)
                {
                    sPoisa = sPoisa + "0";
                }
            }

            int i = sTaka.Length;
            string sDH1 = string.Empty;

            if (i == 9)
            {
                sDH1 = Spell.SpellAmount.F_Crores(sTaka, sPoisa);
            }
            else if (i == 8)
            {
                sDH1 = Spell.SpellAmount.F_Crore(sTaka, sPoisa);
            }
            else if (i == 7)
            {
                sDH1 = Spell.SpellAmount.F_Lakhs(sTaka, sPoisa);
            }
            else if (i == 6)
            {
                sDH1 = Spell.SpellAmount.F_Lakh(sTaka, sPoisa);
            }
            else if (i == 5)
            {
                sDH1 = Spell.SpellAmount.F_Thousands(sTaka, sPoisa);
            }
            else if (i == 4)
            {
                sDH1 = Spell.SpellAmount.F_Thousand(sTaka, sPoisa);
            }
            else if (i == 3)
            {
                sDH1 = Spell.SpellAmount.F_Hundred(sTaka, sPoisa);
            }
            else if (i == 2)
            {
                sDH1 = Spell.SpellAmount.Tens(sTaka);
            }

            sInWords = sDH1 + ".";

            return sInWords;

        }
        public string GetLocalTime()
        {
            DateTime utcTime = DateTime.UtcNow;
            TimeZoneInfo BdZone = TimeZoneInfo.FindSystemTimeZoneById("Bangladesh Standard Time");
            DateTime localDateTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, BdZone);
            return localDateTime.ToString("dd MMM yyyy hh:mm:ss tt");
        }
        public byte[] ExpenditureReport(DateTime fromDate, DateTime toDate, string userName, int concernID, int reportType, int ExpenseItemID)
        {
            try
            {

                var expenseInfos = _expenditureService.GetforExpenditureReport(fromDate, toDate, concernID, reportType, ExpenseItemID);
                DataRow row = null;

                TransactionalDataSet.dtExpenditureDataTable dtExpenditure = new TransactionalDataSet.dtExpenditureDataTable();
                //BasicDataSet.dtEmployeesInfoDataTable dtEmployeesInfo = new BasicDataSet.dtEmployeesInfoDataTable();

                foreach (var item in expenseInfos)
                {
                    row = dtExpenditure.NewRow();
                    row["ExpDate"] = item.Item1.ToString("dd MMM yyyy");
                    row["Description"] = item.Item3;
                    row["Amount"] = item.Item4.ToString("#,###");
                    row["ItemName"] = item.Item2;// item.ExpenseItem.Description;
                    row["VoucherNo"] = item.Item5;
                    row["UserName"] = item.Item6;
                    dtExpenditure.Rows.Add(row);
                }

                dtExpenditure.TableName = "dtExpenditure";
                _dataSet = new DataSet();
                _dataSet.Tables.Add(dtExpenditure);

                GetCommonParameters(userName, concernID);
                if (reportType == (int)EnumCompanyTransaction.Expense)
                    _reportParameter = new ReportParameter("Month", "Expense Report from " + fromDate.ToString("dd MMM yyyy") + " To " + toDate.ToString("dd MMM yyyy"));
                else
                    _reportParameter = new ReportParameter("Month", "Income Report from " + fromDate.ToString("dd MMM yyyy") + " To " + toDate.ToString("dd MMM yyyy"));
                _reportParameters.Add(_reportParameter);
                return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Others\\rptExpenditure.rdlc");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public byte[] SalesBenefitReport(DateTime fromDate, DateTime toDate, string userName, int concernID, int reportType, string period, int CustomerType)
        {
            try
            {
                #region Summary Report
                if (reportType == 1)
                {
                    decimal length = 0m, width = 0m, AreaSqftPerPcs = 0m, AreaSqft = 0m;
                    var salseDetailInfos = _salesOrderService.GetSalesDetailReportByConcernID(fromDate, toDate, concernID);

                    //IEnumerable<Tuple<DateTime, string, string, string, decimal, decimal, decimal, Tuple<decimal, decimal, decimal, decimal, decimal, string, string, Tuple<int>>>> CreditsalseDetailInfos =
                    //                    _creditSalesOrderService.GetCreditSalesDetailReportByConcernID(fromDate, toDate, concernID);

                    IEnumerable<Tuple<DateTime, string, string, string, decimal, decimal, decimal, Tuple<decimal, decimal, decimal, decimal, decimal, string, string, Tuple<int, decimal>>>> returnDetailInfos =
                                                          _returnOrderService.GetReturnDetailReportByConcernID(fromDate, toDate, concernID);


                    TransactionalDataSet.dtCustomerWiseReturnDataTable dtReturn = new TransactionalDataSet.dtCustomerWiseReturnDataTable();
                    TransactionalDataSet.dtCustomerWiseSalesDataTable dt = new TransactionalDataSet.dtCustomerWiseSalesDataTable();
                    int SOrderID = 0, CreditSaleID = 0;




                    var DataGroupBy = ((from item in salseDetailInfos
                                        group item by new
                                        {
                                            item.InvoiceNo,
                                            item.CustomerCode,
                                            item.CustomerName,
                                            item.Date,

                                        } into g
                                        select new ProductWiseSalesReportModel
                                        {

                                            InvoiceNo = g.Key.InvoiceNo,
                                            CustomerCode = g.Key.CustomerCode,
                                            CustomerName = g.Key.CustomerName,
                                            Date = g.FirstOrDefault().Date,

                                            CategoryID = g.FirstOrDefault().CategoryID,
                                            CompanyID = g.FirstOrDefault().CompanyID,

                                            ProductName = g.FirstOrDefault().ProductName,
                                            CompanyName = g.FirstOrDefault().CompanyName,
                                            CategoryName = g.FirstOrDefault().CategoryName,

                                            UTAmount = g.Sum(o => o.UTAmount),
                                            PurchaseTotal = g.Sum(item => item.PurchaseRate * item.Quantity),
                                            ExtraAmt = g.Sum(o => o.ExtraAmt),

                                            NetBenefit = g.Sum(item => item.UTAmount - item.PurchaseRate * item.Quantity),
                                            Totalbenefit = g.Sum(item => item.UTAmount - item.PurchaseRate * item.Quantity + item.ExtraAmt)
                                        }));





                    decimal TotalDueSales = 0, AdjAmount = 0;
                    decimal GrandTotal = 0;
                    decimal TotalDis = 0;
                    decimal NetTotal = 0;
                    decimal RecAmt = 0;
                    decimal CurrDue = 0;
                    DataRow row = null;
                    foreach (var item in DataGroupBy)
                    {
                        //StockDetail std = oSTDList.FirstOrDefault(x => x.SDetailID == grd.StockDetailID);
                        row = dt.NewRow();
                        //if (SOrderID != item.SOrderID)
                        //{
                        //    TotalDueSales = TotalDueSales + item.PaymentDue;
                        //    GrandTotal = GrandTotal + (decimal)item.GrandTotal;
                        //    TotalDis = TotalDis + (decimal)item.NetDiscount;
                        //    NetTotal = NetTotal + (decimal)item.TotalAmount;
                        //    RecAmt = RecAmt + (decimal)item.RecAmount;
                        //    CurrDue = CurrDue + (decimal)item.PaymentDue;
                        //    AdjAmount = AdjAmount + item.AdjAmount;
                        //}

                        row["SalesDate"] = item.Date;
                        row["InvoiceNo"] = item.InvoiceNo;
                        //  row["ProductName"] = item.ProductName;
                        row["CName"] = item.CustomerName;
                        //  row["SalesPrice"] = item.UnitPrice;
                        //  row["NetAmt"] = item.TotalAmount;
                        //  row["GrandTotal"] = item.GrandTotal;
                        // row["TotalDis"] = item.NetDiscount;
                        row["NetTotal"] = item.UTAmount;
                        // row["PaidAmount"] = item.RecAmount;
                        //  row["RemainingAmt"] = item.PaymentDue;
                        row["Quantity"] = 0;// (int)(item.Quantity / item.ConvertValue);
                        row["ChildQty"] = 0;// Convert.ToInt32(item.Quantity % item.ConvertValue);
                        // row["IMENo"] = item.IMEI;
                        // row["ColorInfo"] = item.ColorName;
                        //  row["SalesType"] = string.Empty;
                        //  row["AdjAmount"] = item.AdjAmount;
                        //  row["UnitName"] = item.UnitName;
                        row["Code"] = item.CustomerCode;
                        //  row["IdCode"] = item.IDCode;
                        row["TotalArea"] = 0;// item.Quantity * (item.SalesPerCartonSft / item.ConvertValue);
                        //   row["PerCartonSft"] = item.SalesPerCartonSft;
                        // row["SizeName"] = item.SizeName;

                        //if (item.CategoryName.ToLower().Equals("tiles"))
                        //{
                        //    var area = item.SizeName.Split('x');
                        //    length = Math.Round(Convert.ToDecimal(area[0]) / 2.5m);
                        //    width = Math.Round(Convert.ToDecimal(area[1]) / 2.5m);
                        //    //AreaSqftPerPcs = Math.Round(((length * width) / 144m), 4); //sqft
                        //    //AreaSqft = Math.Round(AreaSqftPerPcs * item.Quantity, 4);

                        //    row["SizeName"] = length + "x" + width;

                        //}
                        //else
                        //{
                        //    row["SizeName"] = item.SizeName;

                        //}

                        row["PurchaseRate"] = item.PurchaseSFTRate;
                        row["PurchaseTotal"] = item.PurchaseTotal;
                        row["ExtraSFT"] = item.ExtraSFT;
                        row["ExtraAmount"] = item.ExtraAmt;
                        row["NetBenefit"] = item.NetBenefit;
                        row["TotalBenefit"] = item.UTAmount - item.PurchaseTotal + item.ExtraAmt;

                        dt.Rows.Add(row);
                        SOrderID = item.SOrderID;



                    }


                    decimal ReturnTotalDueSales = 0, ReturnAdjAmount = 0;
                    decimal ReturnGrandTotal = 0;
                    decimal ReturnTotalDis = 0;
                    decimal ReturnNetTotal = 0;
                    decimal ReturnRecAmt = 0;
                    decimal ReturnCurrDue = 0;


                    foreach (var grd in returnDetailInfos)
                    {
                        //StockDetail std = oSTDList.FirstOrDefault(x => x.SDetailID == grd.StockDetailID);

                        if (SOrderID != grd.Rest.Rest.Item1)
                        {
                            ReturnTotalDueSales = ReturnTotalDueSales + (decimal)grd.Rest.Item4;
                            ReturnGrandTotal = ReturnGrandTotal + (decimal)grd.Item7;
                            ReturnTotalDis = ReturnTotalDis + (decimal)grd.Rest.Item1;
                            ReturnNetTotal = ReturnNetTotal + (decimal)grd.Rest.Item2;
                            ReturnRecAmt = ReturnRecAmt + (decimal)grd.Rest.Item3;
                            ReturnCurrDue = ReturnCurrDue + (decimal)grd.Rest.Item4;
                            ReturnAdjAmount = ReturnAdjAmount + grd.Rest.Rest.Item2;
                        }

                        SOrderID = grd.Rest.Rest.Item1;
                        dtReturn.Rows.Add(grd.Item1.ToString("dd MMM yyyy"), grd.Item2, grd.Item3, grd.Item4, grd.Item5, grd.Item6, grd.Item7, grd.Rest.Item1, grd.Rest.Item2, grd.Rest.Item3, grd.Rest.Item4, grd.Rest.Item5, grd.Rest.Item6, grd.Rest.Item7, "Sales", grd.Rest.Rest.Item2);

                    }



                    decimal NetTotalDueSales = 0, NetAdjAmount = 0;
                    decimal NetGrandTotal = 0;
                    decimal NetTotalDis = 0;
                    decimal NetNetTotal = 0;
                    decimal NetRecAmt = 0;
                    decimal NetCurrDue = 0;


                    NetTotalDueSales = TotalDueSales - ReturnTotalDueSales;
                    NetAdjAmount = AdjAmount - ReturnAdjAmount;
                    NetGrandTotal = GrandTotal - ReturnGrandTotal;
                    NetTotalDis = TotalDis - ReturnTotalDis;
                    NetNetTotal = NetTotal - ReturnNetTotal;
                    NetRecAmt = RecAmt - ReturnRecAmt;
                    NetCurrDue = CurrDue - ReturnCurrDue;

                    dt.TableName = "dtCustomerWiseSales";
                    _dataSet = new DataSet();
                    _dataSet.Tables.Add(dt);


                    dtReturn.TableName = "dtCustomerWiseReturn";

                    _dataSet.Tables.Add(dtReturn);







                    GetCommonParameters(userName, concernID);
                    if (period == "Daily")
                        _reportParameter = new ReportParameter("Date", "Sales details for the date from : " + fromDate.ToString("dd MMM yyyy") + " To " + toDate.ToString("dd MMM yyyy"));
                    else if (period == "Monthly")
                        _reportParameter = new ReportParameter("Date", "Sales details for the Month : " + fromDate.ToString("MMM, yyyy"));
                    else if (period == "Yearly")
                        _reportParameter = new ReportParameter("Date", "Sales details the Year : " + fromDate.ToString("yyyy"));

                    _reportParameters.Add(_reportParameter);





                    _reportParameter = new ReportParameter("GrandTotal", GrandTotal.ToString());
                    _reportParameters.Add(_reportParameter);

                    _reportParameter = new ReportParameter("TotalDis", TotalDis.ToString());
                    _reportParameters.Add(_reportParameter);

                    _reportParameter = new ReportParameter("NetTotal", NetTotal.ToString());
                    _reportParameters.Add(_reportParameter);

                    _reportParameter = new ReportParameter("RecAmt", RecAmt.ToString());
                    _reportParameters.Add(_reportParameter);

                    _reportParameter = new ReportParameter("CurrDue", CurrDue.ToString());
                    _reportParameters.Add(_reportParameter);

                    _reportParameter = new ReportParameter("AdjAmount", AdjAmount.ToString());
                    _reportParameters.Add(_reportParameter);



                    _reportParameter = new ReportParameter("ReturnGrandTotal", ReturnGrandTotal.ToString());
                    _reportParameters.Add(_reportParameter);

                    _reportParameter = new ReportParameter("ReturnTotalDis", ReturnTotalDis.ToString());
                    _reportParameters.Add(_reportParameter);

                    _reportParameter = new ReportParameter("ReturnNetTotal", ReturnNetTotal.ToString());
                    _reportParameters.Add(_reportParameter);

                    _reportParameter = new ReportParameter("ReturnRecAmt", ReturnRecAmt.ToString());
                    _reportParameters.Add(_reportParameter);

                    _reportParameter = new ReportParameter("ReturnCurrDue", ReturnCurrDue.ToString());
                    _reportParameters.Add(_reportParameter);

                    _reportParameter = new ReportParameter("ReturnAdjAmount", ReturnAdjAmount.ToString());
                    _reportParameters.Add(_reportParameter);




                    _reportParameter = new ReportParameter("NetGrandTotal", NetGrandTotal.ToString());
                    _reportParameters.Add(_reportParameter);

                    _reportParameter = new ReportParameter("NetTotalDis", NetTotalDis.ToString());
                    _reportParameters.Add(_reportParameter);

                    _reportParameter = new ReportParameter("NetNetTotal", NetNetTotal.ToString());
                    _reportParameters.Add(_reportParameter);

                    _reportParameter = new ReportParameter("NetRecAmt", NetRecAmt.ToString());
                    _reportParameters.Add(_reportParameter);

                    _reportParameter = new ReportParameter("NetCurrDue", NetCurrDue.ToString());
                    _reportParameters.Add(_reportParameter);

                    _reportParameter = new ReportParameter("NetAdjAmount", NetAdjAmount.ToString());
                    _reportParameters.Add(_reportParameter);


                    return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Sales\\rptSalesBenefitSummary.rdlc");
                }
                #endregion

                #region Details Report
                else
                {
                    decimal length = 0m, width = 0m, AreaSqftPerPcs = 0m, AreaSqft = 0m;
                    var salseDetailInfos = _salesOrderService.GetSalesDetailReportByConcernID(fromDate, toDate, concernID);

                    //IEnumerable<Tuple<DateTime, string, string, string, decimal, decimal, decimal, Tuple<decimal, decimal, decimal, decimal, decimal, string, string, Tuple<int>>>> CreditsalseDetailInfos =
                    //                    _creditSalesOrderService.GetCreditSalesDetailReportByConcernID(fromDate, toDate, concernID);

                    IEnumerable<Tuple<DateTime, string, string, string, decimal, decimal, decimal, Tuple<decimal, decimal, decimal, decimal, decimal, string, string, Tuple<int, decimal>>>> returnDetailInfos =
                                                          _returnOrderService.GetReturnDetailReportByConcernID(fromDate, toDate, concernID);


                    TransactionalDataSet.dtCustomerWiseReturnDataTable dtReturn = new TransactionalDataSet.dtCustomerWiseReturnDataTable();
                    TransactionalDataSet.dtCustomerWiseSalesDataTable dt = new TransactionalDataSet.dtCustomerWiseSalesDataTable();
                    int SOrderID = 0, CreditSaleID = 0;

                    decimal TotalDueSales = 0, AdjAmount = 0;
                    decimal GrandTotal = 0;
                    decimal TotalDis = 0;
                    decimal NetTotal = 0;
                    decimal RecAmt = 0;
                    decimal CurrDue = 0;
                    DataRow row = null;
                    foreach (var item in salseDetailInfos)
                    {
                        //StockDetail std = oSTDList.FirstOrDefault(x => x.SDetailID == grd.StockDetailID);
                        row = dt.NewRow();
                        if (SOrderID != item.SOrderID)
                        {
                            TotalDueSales = TotalDueSales + item.PaymentDue;
                            GrandTotal = GrandTotal + (decimal)item.GrandTotal;
                            TotalDis = TotalDis + (decimal)item.NetDiscount;
                            NetTotal = NetTotal + (decimal)item.TotalAmount;
                            RecAmt = RecAmt + (decimal)item.RecAmount;
                            CurrDue = CurrDue + (decimal)item.PaymentDue;
                            AdjAmount = AdjAmount + item.AdjAmount;
                        }

                        row["SalesDate"] = item.Date;
                        row["InvoiceNo"] = item.InvoiceNo;
                        row["ProductName"] = item.ProductName;
                        row["CName"] = item.CustomerName;
                        row["SalesPrice"] = item.UnitPrice;
                        row["NetAmt"] = item.TotalAmount;
                        row["GrandTotal"] = item.GrandTotal;
                        row["TotalDis"] = item.NetDiscount;
                        row["NetTotal"] = item.UTAmount;
                        row["PaidAmount"] = item.RecAmount;
                        row["RemainingAmt"] = item.PaymentDue;
                        row["Quantity"] = (int)(item.Quantity / item.ConvertValue);
                        row["ChildQty"] = Convert.ToInt32(item.Quantity % item.ConvertValue);
                        row["IMENo"] = item.IMEI;
                        row["ColorInfo"] = item.ColorName;
                        row["SalesType"] = string.Empty;
                        row["AdjAmount"] = item.AdjAmount;
                        row["UnitName"] = item.UnitName;
                        row["Code"] = item.CustomerCode;
                        row["IdCode"] = item.IDCode;
                        row["TotalArea"] = item.Quantity * (item.SalesPerCartonSft / item.ConvertValue);
                        row["PerCartonSft"] = item.SalesPerCartonSft;
                        // row["SizeName"] = item.SizeName;

                        if (item.CategoryName.ToLower().Equals("tiles"))
                        {
                            var area = item.SizeName.Split('x');
                            length = Math.Round(Convert.ToDecimal(area[0]) / 2.5m);
                            width = Math.Round(Convert.ToDecimal(area[1]) / 2.5m);
                            //AreaSqftPerPcs = Math.Round(((length * width) / 144m), 4); //sqft
                            //AreaSqft = Math.Round(AreaSqftPerPcs * item.Quantity, 4);

                            row["SizeName"] = length + "x" + width;

                        }
                        else
                        {
                            row["SizeName"] = item.SizeName;

                        }

                        row["PurchaseRate"] = item.PurchaseSFTRate;
                        row["PurchaseTotal"] = item.PurchaseRate * item.Quantity;
                        row["ExtraSFT"] = item.ExtraSFT;
                        row["ExtraAmount"] = item.ExtraAmt;
                        row["NetBenefit"] = item.UTAmount - item.PurchaseRate * item.Quantity;
                        row["TotalBenefit"] = item.UTAmount - item.PurchaseRate * item.Quantity + item.ExtraAmt;

                        dt.Rows.Add(row);
                        SOrderID = item.SOrderID;



                        //if (item.CategoryName.ToLower().Equals("tiles"))
                        //{
                        //    //var area = item.SizeName.Split('x');
                        //    //length = Math.Round(Convert.ToDecimal(area[0]) / 2.5m);
                        //    //width = Math.Round(Convert.ToDecimal(area[1]) / 2.5m);
                        //    //AreaSqftPerPcs = Math.Round(((length * width) / 144m), 4); //sqft
                        //    //AreaSqft = Math.Round(AreaSqftPerPcs * item.Quantity, 4);
                        //    //row["TotalArea"] = item.TotalSFT;
                        //    //row["SizeName"] = length + "x" + width;

                        //    row["TotalPrice"] = (item.Quantity * (item.SalesPerCartonSft / item.ConvertValue) * item.UnitPrice);
                        //}
                        //else
                        //{
                        //    row["SizeName"] = "N/A";
                        //    row["TotalPrice"] = ((item.Quantity / item.ConvertValue) * item.MRP);
                        //}


                    }

                    //For Credit Sales
                    //foreach (var grd in CreditsalseDetailInfos)
                    //{
                    //    //StockDetail std = oSTDList.FirstOrDefault(x => x.SDetailID == grd.StockDetailID);

                    //    if (CreditSaleID != grd.Rest.Rest.Item1)
                    //    {
                    //        TotalDueSales = TotalDueSales + (decimal)grd.Rest.Item4;
                    //        GrandTotal = GrandTotal + (decimal)grd.Item7;
                    //        TotalDis = TotalDis + (decimal)grd.Rest.Item1;
                    //        NetTotal = NetTotal + (decimal)grd.Rest.Item2;
                    //        RecAmt = RecAmt + (decimal)grd.Rest.Item3;
                    //        CurrDue = CurrDue + (decimal)grd.Rest.Item4;
                    //    }

                    //    CreditSaleID = grd.Rest.Rest.Item1;
                    //    dt.Rows.Add(grd.Item1.ToString("dd MMM yyyy"), grd.Item2, grd.Item3, grd.Item4, grd.Item5, grd.Item6, grd.Item7, grd.Rest.Item1, grd.Rest.Item2, grd.Rest.Item3, grd.Rest.Item4, grd.Rest.Item5, grd.Rest.Item6, grd.Rest.Item7, "Credit Sales", 0m);

                    //}




                    decimal ReturnTotalDueSales = 0, ReturnAdjAmount = 0;
                    decimal ReturnGrandTotal = 0;
                    decimal ReturnTotalDis = 0;
                    decimal ReturnNetTotal = 0;
                    decimal ReturnRecAmt = 0;
                    decimal ReturnCurrDue = 0;


                    foreach (var grd in returnDetailInfos)
                    {
                        //StockDetail std = oSTDList.FirstOrDefault(x => x.SDetailID == grd.StockDetailID);

                        if (SOrderID != grd.Rest.Rest.Item1)
                        {
                            ReturnTotalDueSales = ReturnTotalDueSales + (decimal)grd.Rest.Item4;
                            ReturnGrandTotal = ReturnGrandTotal + (decimal)grd.Item7;
                            ReturnTotalDis = ReturnTotalDis + (decimal)grd.Rest.Item1;
                            ReturnNetTotal = ReturnNetTotal + (decimal)grd.Rest.Item2;
                            ReturnRecAmt = ReturnRecAmt + (decimal)grd.Rest.Item3;
                            ReturnCurrDue = ReturnCurrDue + (decimal)grd.Rest.Item4;
                            ReturnAdjAmount = ReturnAdjAmount + grd.Rest.Rest.Item2;
                        }

                        SOrderID = grd.Rest.Rest.Item1;
                        dtReturn.Rows.Add(grd.Item1.ToString("dd MMM yyyy"), grd.Item2, grd.Item3, grd.Item4, grd.Item5, grd.Item6, grd.Item7, grd.Rest.Item1, grd.Rest.Item2, grd.Rest.Item3, grd.Rest.Item4, grd.Rest.Item5, grd.Rest.Item6, grd.Rest.Item7, "Sales", grd.Rest.Rest.Item2);
                    }

                    decimal NetTotalDueSales = 0, NetAdjAmount = 0;
                    decimal NetGrandTotal = 0;
                    decimal NetTotalDis = 0;
                    decimal NetNetTotal = 0;
                    decimal NetRecAmt = 0;
                    decimal NetCurrDue = 0;

                    NetTotalDueSales = TotalDueSales - ReturnTotalDueSales;
                    NetAdjAmount = AdjAmount - ReturnAdjAmount;
                    NetGrandTotal = GrandTotal - ReturnGrandTotal;
                    NetTotalDis = TotalDis - ReturnTotalDis;
                    NetNetTotal = NetTotal - ReturnNetTotal;
                    NetRecAmt = RecAmt - ReturnRecAmt;
                    NetCurrDue = CurrDue - ReturnCurrDue;
                    dt.TableName = "dtCustomerWiseSales";
                    _dataSet = new DataSet();
                    _dataSet.Tables.Add(dt);
                    dtReturn.TableName = "dtCustomerWiseReturn";
                    _dataSet.Tables.Add(dtReturn);
                    GetCommonParameters(userName, concernID);
                    if (period == "Daily")
                        _reportParameter = new ReportParameter("Date", "Sales details for the date from : " + fromDate.ToString("dd MMM yyyy") + " To " + toDate.ToString("dd MMM yyyy"));
                    else if (period == "Monthly")
                        _reportParameter = new ReportParameter("Date", "Sales details for the Month : " + fromDate.ToString("MMM, yyyy"));
                    else if (period == "Yearly")
                        _reportParameter = new ReportParameter("Date", "Sales details the Year : " + fromDate.ToString("yyyy"));

                    _reportParameters.Add(_reportParameter);
                    _reportParameter = new ReportParameter("GrandTotal", GrandTotal.ToString());
                    _reportParameters.Add(_reportParameter);
                    _reportParameter = new ReportParameter("TotalDis", TotalDis.ToString());
                    _reportParameters.Add(_reportParameter);
                    _reportParameter = new ReportParameter("NetTotal", NetTotal.ToString());
                    _reportParameters.Add(_reportParameter);

                    _reportParameter = new ReportParameter("RecAmt", RecAmt.ToString());
                    _reportParameters.Add(_reportParameter);

                    _reportParameter = new ReportParameter("CurrDue", CurrDue.ToString());
                    _reportParameters.Add(_reportParameter);

                    _reportParameter = new ReportParameter("AdjAmount", AdjAmount.ToString());
                    _reportParameters.Add(_reportParameter);



                    _reportParameter = new ReportParameter("ReturnGrandTotal", ReturnGrandTotal.ToString());
                    _reportParameters.Add(_reportParameter);

                    _reportParameter = new ReportParameter("ReturnTotalDis", ReturnTotalDis.ToString());
                    _reportParameters.Add(_reportParameter);

                    _reportParameter = new ReportParameter("ReturnNetTotal", ReturnNetTotal.ToString());
                    _reportParameters.Add(_reportParameter);

                    _reportParameter = new ReportParameter("ReturnRecAmt", ReturnRecAmt.ToString());
                    _reportParameters.Add(_reportParameter);

                    _reportParameter = new ReportParameter("ReturnCurrDue", ReturnCurrDue.ToString());
                    _reportParameters.Add(_reportParameter);

                    _reportParameter = new ReportParameter("ReturnAdjAmount", ReturnAdjAmount.ToString());
                    _reportParameters.Add(_reportParameter);




                    _reportParameter = new ReportParameter("NetGrandTotal", NetGrandTotal.ToString());
                    _reportParameters.Add(_reportParameter);

                    _reportParameter = new ReportParameter("NetTotalDis", NetTotalDis.ToString());
                    _reportParameters.Add(_reportParameter);

                    _reportParameter = new ReportParameter("NetNetTotal", NetNetTotal.ToString());
                    _reportParameters.Add(_reportParameter);

                    _reportParameter = new ReportParameter("NetRecAmt", NetRecAmt.ToString());
                    _reportParameters.Add(_reportParameter);

                    _reportParameter = new ReportParameter("NetCurrDue", NetCurrDue.ToString());
                    _reportParameters.Add(_reportParameter);

                    _reportParameter = new ReportParameter("NetAdjAmount", NetAdjAmount.ToString());
                    _reportParameters.Add(_reportParameter);


                    return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Sales\\rptSalesBenefitDetails.rdlc");
                }
                #endregion

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public byte[] SalesReport(DateTime fromDate, DateTime toDate, string userName, int concernID, int reportType, string period, int CustomerType)
        {
            try
            {
                #region Summary
                if (reportType == 1)
                {
                    var salseInfos = _salesOrderService.GetSalesReportByConcernID(fromDate, toDate, concernID, CustomerType);

                    var CreditsalseInfos = _creditSalesOrderService.GetCreditSalesReportByConcernID(fromDate, toDate, concernID, CustomerType);

                    var ReturnInfos = _returnOrderService.GetReturnReportByConcernID(fromDate, toDate, concernID, CustomerType);


                    decimal NetRecAmount = 0m;
                    decimal NetPaymentDue = 0m;

                    decimal Sales = salseInfos.Count() != 0 ? salseInfos.Sum(o => o.Item5) : 0m;
                    decimal CrediSales = CreditsalseInfos.Count() != 0 ? CreditsalseInfos.Sum(o => o.Item5) : 0m;
                    decimal ReturnAmt = ReturnInfos.Count() != 0 ? ReturnInfos.Sum(o => o.Item5) : 0m;
                    decimal NetSales = Sales + CrediSales - ReturnAmt;

                    decimal SalesRec = salseInfos.Count() != 0 ? salseInfos.Sum(o => o.Rest.Item1) : 0m;
                    decimal CrediSalesRec = CreditsalseInfos.Count() != 0 ? CreditsalseInfos.Sum(o => o.Rest.Item1) : 0m;
                    decimal ReturnAmtRec = ReturnInfos.Count() != 0 ? ReturnInfos.Sum(o => o.Rest.Item1) : 0m;
                    decimal NetSalesRec = SalesRec + CrediSalesRec - ReturnAmtRec;

                    decimal SalesDue = salseInfos.Count() != 0 ? salseInfos.Sum(o => o.Rest.Item2) : 0m;
                    decimal CrediSalesDue = CreditsalseInfos.Count() != 0 ? CreditsalseInfos.Sum(o => o.Rest.Item2) : 0m;
                    decimal ReturnAmtDue = ReturnInfos.Count() != 0 ? ReturnInfos.Sum(o => o.Rest.Item2) : 0m;
                    decimal NetSalesDue = SalesDue + CrediSalesDue - ReturnAmtDue;

                    DataRow row = null;

                    TransactionalDataSet.dtOrderDataTable dt = new TransactionalDataSet.dtOrderDataTable();
                    //BasicDataSet.dtEmployeesInfoDataTable dtEmployeesInfo = new BasicDataSet.dtEmployeesInfoDataTable();
                    TransactionalDataSet.dtReturnOrderDataTable dtReturn = new TransactionalDataSet.dtReturnOrderDataTable();


                    foreach (var item in salseInfos)
                    {
                        row = dt.NewRow();
                        row["CustomerCode"] = item.Item1;
                        row["Name"] = item.Item2;
                        row["Date"] = item.Item3.ToString("dd MMM yyyy");
                        row["InvoiceNo"] = item.Item4;
                        // item.ExpenseItem.Description;
                        row["GrandTotal"] = item.Item5;
                        row["DiscountAmount"] = item.Item6 - item.Rest.Item4;
                        row["Amount"] = item.Item7;
                        row["RecAmt"] = item.Rest.Item1;
                        row["DueAmount"] = item.Rest.Item2;
                        row["SalesType"] = "Cash Sales";
                        row["AdjustAmt"] = item.Rest.Item3;
                        row["TotalOffer"] = item.Rest.Item4;

                        dt.Rows.Add(row);
                    }

                    foreach (var item in CreditsalseInfos)
                    {
                        row = dt.NewRow();
                        row["CustomerCode"] = item.Item1;
                        row["Name"] = item.Item2;
                        row["Date"] = item.Item3.ToString("dd MMM yyyy");
                        row["InvoiceNo"] = item.Item4;
                        // item.ExpenseItem.Description;
                        row["GrandTotal"] = item.Item5;
                        row["DiscountAmount"] = item.Item6 - item.Rest.Item4;
                        row["Amount"] = item.Item7;
                        row["RecAmt"] = item.Rest.Item1;
                        row["DueAmount"] = item.Rest.Item2;
                        row["SalesType"] = "Credit Sales";
                        row["AdjustAmt"] = 0;
                        row["TotalOffer"] = item.Rest.Item4;
                        row["InstallmentPeriod"] = item.Rest.Item7;

                        dt.Rows.Add(row);
                    }




                    foreach (var item in ReturnInfos)
                    {
                        row = dtReturn.NewRow();
                        row["CustomerCode"] = item.Item1;
                        row["Name"] = item.Item2;
                        row["Date"] = item.Item3.ToString("dd MMM yyyy");
                        row["InvoiceNo"] = item.Item4;
                        // item.ExpenseItem.Description;
                        row["GrandTotal"] = item.Item5;
                        row["DiscountAmount"] = item.Item6 - item.Rest.Item4;
                        row["Amount"] = item.Item7;
                        row["RecAmt"] = item.Rest.Item1;
                        row["DueAmount"] = item.Rest.Item2;
                        row["SalesType"] = "Cash Sales";
                        row["AdjustAmt"] = item.Rest.Item3;
                        row["TotalOffer"] = item.Rest.Item4;

                        dtReturn.Rows.Add(row);
                    }


                    dt.TableName = "dtOrder";
                    _dataSet = new DataSet();
                    _dataSet.Tables.Add(dt);

                    dtReturn.TableName = "dtReturnOrder";

                    _dataSet.Tables.Add(dtReturn);

                    GetCommonParameters(userName, concernID);

                    _reportParameter = new ReportParameter("NetSales", NetSales.ToString());
                    _reportParameters.Add(_reportParameter);
                    _reportParameter = new ReportParameter("NetSalesRec", NetSalesRec.ToString());
                    _reportParameters.Add(_reportParameter);
                    _reportParameter = new ReportParameter("NetSalesDue", NetSalesDue.ToString());
                    _reportParameters.Add(_reportParameter);




                    if (period == "Daily")
                        _reportParameter = new ReportParameter("Month", "Sales report for the date from : " + fromDate.ToString("dd MMM yyyy") + " To " + toDate.ToString("dd MMM yyyy"));
                    else if (period == "Monthly")
                        _reportParameter = new ReportParameter("Month", "Sales report for the Month : " + fromDate.ToString("MMM, yyyy"));
                    else if (period == "Yearly")
                        _reportParameter = new ReportParameter("Month", "Sales report for the Year : " + fromDate.ToString("yyyy"));
                    _reportParameters.Add(_reportParameter);
                    return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Sales\\rptMonthlyOrder.rdlc");

                }
                #endregion



                #region Details Report
                else
                {
                    decimal length = 0m, width = 0m, AreaSqftPerPcs = 0m, AreaSqft = 0m;
                    var salseDetailInfos = _salesOrderService.GetSalesDetailReportByConcernID(fromDate, toDate, concernID);

                    //IEnumerable<Tuple<DateTime, string, string, string, decimal, decimal, decimal, Tuple<decimal, decimal, decimal, decimal, decimal, string, string, Tuple<int>>>> CreditsalseDetailInfos =
                    //                    _creditSalesOrderService.GetCreditSalesDetailReportByConcernID(fromDate, toDate, concernID);

                    IEnumerable<Tuple<DateTime, string, string, string, decimal, decimal, decimal, Tuple<decimal, decimal, decimal, decimal, decimal, string, string, Tuple<int, decimal>>>> returnDetailInfos =
                                                          _returnOrderService.GetReturnDetailReportByConcernID(fromDate, toDate, concernID);


                    TransactionalDataSet.dtCustomerWiseReturnDataTable dtReturn = new TransactionalDataSet.dtCustomerWiseReturnDataTable();
                    TransactionalDataSet.dtCustomerWiseSalesDataTable dt = new TransactionalDataSet.dtCustomerWiseSalesDataTable();
                    int SOrderID = 0, CreditSaleID = 0;

                    decimal TotalDueSales = 0, AdjAmount = 0;
                    decimal GrandTotal = 0;
                    decimal TotalDis = 0;
                    decimal NetTotal = 0;
                    decimal RecAmt = 0;
                    decimal CurrDue = 0;
                    DataRow row = null;
                    foreach (var item in salseDetailInfos)
                    {
                        //StockDetail std = oSTDList.FirstOrDefault(x => x.SDetailID == grd.StockDetailID);
                        row = dt.NewRow();
                        if (SOrderID != item.SOrderID)
                        {
                            TotalDueSales = TotalDueSales + item.PaymentDue;
                            GrandTotal = GrandTotal + (decimal)item.GrandTotal;
                            TotalDis = TotalDis + (decimal)item.NetDiscount;
                            NetTotal = NetTotal + (decimal)item.TotalAmount;
                            RecAmt = RecAmt + (decimal)item.RecAmount;
                            CurrDue = CurrDue + (decimal)item.PaymentDue;
                            AdjAmount = AdjAmount + item.AdjAmount;
                        }

                        row["SalesDate"] = item.Date;
                        row["InvoiceNo"] = item.InvoiceNo;
                        row["ProductName"] = item.ProductName;
                        row["CName"] = item.CustomerName;
                        row["SalesPrice"] = item.UnitPrice;
                        row["NetAmt"] = item.TotalAmount;
                        row["GrandTotal"] = item.GrandTotal;
                        row["TotalDis"] = item.NetDiscount;
                        row["NetTotal"] = item.UTAmount;
                        row["PaidAmount"] = item.RecAmount;
                        row["RemainingAmt"] = item.PaymentDue;
                        row["Quantity"] = (int)(item.Quantity / item.ConvertValue);
                        row["ChildQty"] = Convert.ToInt32(item.Quantity % item.ConvertValue);
                        row["IMENo"] = item.IMEI;
                        row["ColorInfo"] = item.ColorName;
                        row["SalesType"] = string.Empty;
                        row["AdjAmount"] = item.AdjAmount;
                        row["UnitName"] = item.UnitName;
                        row["Code"] = item.CustomerCode;
                        row["IdCode"] = item.IDCode;
                        row["TotalArea"] = item.Quantity * (item.SalesPerCartonSft / item.ConvertValue);
                        row["PerCartonSft"] = item.SalesPerCartonSft;
                        // row["SizeName"] = item.SizeName;

                        if (item.CategoryName.ToLower().Equals("tiles"))
                        {
                            var area = item.SizeName.Split('x');
                            length = Math.Round(Convert.ToDecimal(area[0]) / 2.5m);
                            width = Math.Round(Convert.ToDecimal(area[1]) / 2.5m);
                            //AreaSqftPerPcs = Math.Round(((length * width) / 144m), 4); //sqft
                            //AreaSqft = Math.Round(AreaSqftPerPcs * item.Quantity, 4);

                            row["SizeName"] = length + "x" + width;

                        }
                        else
                        {
                            row["SizeName"] = item.SizeName;

                        }

                        row["PurchaseRate"] = item.PurchaseRate;
                        row["PurchaseTotal"] = item.PurchaseRate * item.Quantity;
                        row["ExtraSFT"] = item.ExtraSFT;
                        row["ExtraAmount"] = item.ExtraAmt;
                        row["TotalBenefit"] = item.UTAmount - item.PurchaseRate * item.Quantity + item.ExtraAmt;

                        dt.Rows.Add(row);
                        SOrderID = item.SOrderID;



                        //if (item.CategoryName.ToLower().Equals("tiles"))
                        //{
                        //    //var area = item.SizeName.Split('x');
                        //    //length = Math.Round(Convert.ToDecimal(area[0]) / 2.5m);
                        //    //width = Math.Round(Convert.ToDecimal(area[1]) / 2.5m);
                        //    //AreaSqftPerPcs = Math.Round(((length * width) / 144m), 4); //sqft
                        //    //AreaSqft = Math.Round(AreaSqftPerPcs * item.Quantity, 4);
                        //    //row["TotalArea"] = item.TotalSFT;
                        //    //row["SizeName"] = length + "x" + width;

                        //    row["TotalPrice"] = (item.Quantity * (item.SalesPerCartonSft / item.ConvertValue) * item.UnitPrice);
                        //}
                        //else
                        //{
                        //    row["SizeName"] = "N/A";
                        //    row["TotalPrice"] = ((item.Quantity / item.ConvertValue) * item.MRP);
                        //}


                    }

                    //For Credit Sales
                    //foreach (var grd in CreditsalseDetailInfos)
                    //{
                    //    //StockDetail std = oSTDList.FirstOrDefault(x => x.SDetailID == grd.StockDetailID);

                    //    if (CreditSaleID != grd.Rest.Rest.Item1)
                    //    {
                    //        TotalDueSales = TotalDueSales + (decimal)grd.Rest.Item4;
                    //        GrandTotal = GrandTotal + (decimal)grd.Item7;
                    //        TotalDis = TotalDis + (decimal)grd.Rest.Item1;
                    //        NetTotal = NetTotal + (decimal)grd.Rest.Item2;
                    //        RecAmt = RecAmt + (decimal)grd.Rest.Item3;
                    //        CurrDue = CurrDue + (decimal)grd.Rest.Item4;
                    //    }

                    //    CreditSaleID = grd.Rest.Rest.Item1;
                    //    dt.Rows.Add(grd.Item1.ToString("dd MMM yyyy"), grd.Item2, grd.Item3, grd.Item4, grd.Item5, grd.Item6, grd.Item7, grd.Rest.Item1, grd.Rest.Item2, grd.Rest.Item3, grd.Rest.Item4, grd.Rest.Item5, grd.Rest.Item6, grd.Rest.Item7, "Credit Sales", 0m);

                    //}




                    decimal ReturnTotalDueSales = 0, ReturnAdjAmount = 0;
                    decimal ReturnGrandTotal = 0;
                    decimal ReturnTotalDis = 0;
                    decimal ReturnNetTotal = 0;
                    decimal ReturnRecAmt = 0;
                    decimal ReturnCurrDue = 0;


                    foreach (var grd in returnDetailInfos)
                    {
                        //StockDetail std = oSTDList.FirstOrDefault(x => x.SDetailID == grd.StockDetailID);

                        if (SOrderID != grd.Rest.Rest.Item1)
                        {
                            ReturnTotalDueSales = ReturnTotalDueSales + (decimal)grd.Rest.Item4;
                            ReturnGrandTotal = ReturnGrandTotal + (decimal)grd.Item7;
                            ReturnTotalDis = ReturnTotalDis + (decimal)grd.Rest.Item1;
                            ReturnNetTotal = ReturnNetTotal + (decimal)grd.Rest.Item2;
                            ReturnRecAmt = ReturnRecAmt + (decimal)grd.Rest.Item3;
                            ReturnCurrDue = ReturnCurrDue + (decimal)grd.Rest.Item4;
                            ReturnAdjAmount = ReturnAdjAmount + grd.Rest.Rest.Item2;
                        }

                        SOrderID = grd.Rest.Rest.Item1;
                        dtReturn.Rows.Add(grd.Item1.ToString("dd MMM yyyy"), grd.Item2, grd.Item3, grd.Item4, grd.Item5, grd.Item6, grd.Item7, grd.Rest.Item1, grd.Rest.Item2, grd.Rest.Item3, grd.Rest.Item4, grd.Rest.Item5, grd.Rest.Item6, grd.Rest.Item7, "Sales", grd.Rest.Rest.Item2);

                    }



                    decimal NetTotalDueSales = 0, NetAdjAmount = 0;
                    decimal NetGrandTotal = 0;
                    decimal NetTotalDis = 0;
                    decimal NetNetTotal = 0;
                    decimal NetRecAmt = 0;
                    decimal NetCurrDue = 0;


                    NetTotalDueSales = TotalDueSales - ReturnTotalDueSales;
                    NetAdjAmount = AdjAmount - ReturnAdjAmount;
                    NetGrandTotal = GrandTotal - ReturnGrandTotal;
                    NetTotalDis = TotalDis - ReturnTotalDis;
                    NetNetTotal = NetTotal - ReturnNetTotal;
                    NetRecAmt = RecAmt - ReturnRecAmt;
                    NetCurrDue = CurrDue - ReturnCurrDue;

                    dt.TableName = "dtCustomerWiseSales";
                    _dataSet = new DataSet();
                    _dataSet.Tables.Add(dt);


                    dtReturn.TableName = "dtCustomerWiseReturn";

                    _dataSet.Tables.Add(dtReturn);







                    GetCommonParameters(userName, concernID);
                    if (period == "Daily")
                        _reportParameter = new ReportParameter("Date", "Sales details for the date from : " + fromDate.ToString("dd MMM yyyy") + " To " + toDate.ToString("dd MMM yyyy"));
                    else if (period == "Monthly")
                        _reportParameter = new ReportParameter("Date", "Sales details for the Month : " + fromDate.ToString("MMM, yyyy"));
                    else if (period == "Yearly")
                        _reportParameter = new ReportParameter("Date", "Sales details the Year : " + fromDate.ToString("yyyy"));

                    _reportParameters.Add(_reportParameter);





                    _reportParameter = new ReportParameter("GrandTotal", GrandTotal.ToString());
                    _reportParameters.Add(_reportParameter);

                    _reportParameter = new ReportParameter("TotalDis", TotalDis.ToString());
                    _reportParameters.Add(_reportParameter);

                    _reportParameter = new ReportParameter("NetTotal", NetTotal.ToString());
                    _reportParameters.Add(_reportParameter);

                    _reportParameter = new ReportParameter("RecAmt", RecAmt.ToString());
                    _reportParameters.Add(_reportParameter);

                    _reportParameter = new ReportParameter("CurrDue", CurrDue.ToString());
                    _reportParameters.Add(_reportParameter);

                    _reportParameter = new ReportParameter("AdjAmount", AdjAmount.ToString());
                    _reportParameters.Add(_reportParameter);



                    _reportParameter = new ReportParameter("ReturnGrandTotal", ReturnGrandTotal.ToString());
                    _reportParameters.Add(_reportParameter);

                    _reportParameter = new ReportParameter("ReturnTotalDis", ReturnTotalDis.ToString());
                    _reportParameters.Add(_reportParameter);

                    _reportParameter = new ReportParameter("ReturnNetTotal", ReturnNetTotal.ToString());
                    _reportParameters.Add(_reportParameter);

                    _reportParameter = new ReportParameter("ReturnRecAmt", ReturnRecAmt.ToString());
                    _reportParameters.Add(_reportParameter);

                    _reportParameter = new ReportParameter("ReturnCurrDue", ReturnCurrDue.ToString());
                    _reportParameters.Add(_reportParameter);

                    _reportParameter = new ReportParameter("ReturnAdjAmount", ReturnAdjAmount.ToString());
                    _reportParameters.Add(_reportParameter);




                    _reportParameter = new ReportParameter("NetGrandTotal", NetGrandTotal.ToString());
                    _reportParameters.Add(_reportParameter);

                    _reportParameter = new ReportParameter("NetTotalDis", NetTotalDis.ToString());
                    _reportParameters.Add(_reportParameter);

                    _reportParameter = new ReportParameter("NetNetTotal", NetNetTotal.ToString());
                    _reportParameters.Add(_reportParameter);

                    _reportParameter = new ReportParameter("NetRecAmt", NetRecAmt.ToString());
                    _reportParameters.Add(_reportParameter);

                    _reportParameter = new ReportParameter("NetCurrDue", NetCurrDue.ToString());
                    _reportParameters.Add(_reportParameter);

                    _reportParameter = new ReportParameter("NetAdjAmount", NetAdjAmount.ToString());
                    _reportParameters.Add(_reportParameter);


                    return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Sales\\rptSalesDetails.rdlc");
                }
                #endregion

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        //public byte[] AdminSalesReportDetails(DateTime fromDate, DateTime toDate, string userName, int concernID, int reportType, string period, int CustomerType)
        //{
        //    try
        //    {
        //        #region Summary
        //        if (reportType == 1)
        //        {
        //            var salseInfos = _salesOrderService.GetSalesReportByConcernID(fromDate, toDate, concernID, CustomerType);

        //            var CreditsalseInfos = _creditSalesOrderService.GetCreditSalesReportByConcernID(fromDate, toDate, concernID, CustomerType);

        //            var ReturnInfos = _returnOrderService.GetReturnReportByConcernID(fromDate, toDate, concernID, CustomerType);


        //            decimal NetRecAmount = 0m;
        //            decimal NetPaymentDue = 0m;

        //            decimal Sales = salseInfos.Count() != 0 ? salseInfos.Sum(o => o.Item5) : 0m;
        //            decimal CrediSales = CreditsalseInfos.Count() != 0 ? CreditsalseInfos.Sum(o => o.Item5) : 0m;
        //            decimal ReturnAmt = ReturnInfos.Count() != 0 ? ReturnInfos.Sum(o => o.Item5) : 0m;
        //            decimal NetSales = Sales + CrediSales - ReturnAmt;

        //            decimal SalesRec = salseInfos.Count() != 0 ? salseInfos.Sum(o => o.Rest.Item1) : 0m;
        //            decimal CrediSalesRec = CreditsalseInfos.Count() != 0 ? CreditsalseInfos.Sum(o => o.Rest.Item1) : 0m;
        //            decimal ReturnAmtRec = ReturnInfos.Count() != 0 ? ReturnInfos.Sum(o => o.Rest.Item1) : 0m;
        //            decimal NetSalesRec = SalesRec + CrediSalesRec - ReturnAmtRec;

        //            decimal SalesDue = salseInfos.Count() != 0 ? salseInfos.Sum(o => o.Rest.Item2) : 0m;
        //            decimal CrediSalesDue = CreditsalseInfos.Count() != 0 ? CreditsalseInfos.Sum(o => o.Rest.Item2) : 0m;
        //            decimal ReturnAmtDue = ReturnInfos.Count() != 0 ? ReturnInfos.Sum(o => o.Rest.Item2) : 0m;
        //            decimal NetSalesDue = SalesDue + CrediSalesDue - ReturnAmtDue;

        //            DataRow row = null;

        //            TransactionalDataSet.dtOrderDataTable dt = new TransactionalDataSet.dtOrderDataTable();
        //            //BasicDataSet.dtEmployeesInfoDataTable dtEmployeesInfo = new BasicDataSet.dtEmployeesInfoDataTable();
        //            TransactionalDataSet.dtReturnOrderDataTable dtReturn = new TransactionalDataSet.dtReturnOrderDataTable();


        //            foreach (var item in salseInfos)
        //            {
        //                row = dt.NewRow();
        //                row["CustomerCode"] = item.Item1;
        //                row["Name"] = item.Item2;
        //                row["Date"] = item.Item3.ToString("dd MMM yyyy");
        //                row["InvoiceNo"] = item.Item4;
        //                // item.ExpenseItem.Description;
        //                row["GrandTotal"] = item.Item5;
        //                row["DiscountAmount"] = item.Item6 - item.Rest.Item4;
        //                row["Amount"] = item.Item7;
        //                row["RecAmt"] = item.Rest.Item1;
        //                row["DueAmount"] = item.Rest.Item2;
        //                row["SalesType"] = "Cash Sales";
        //                row["AdjustAmt"] = item.Rest.Item3;
        //                row["TotalOffer"] = item.Rest.Item4;

        //                dt.Rows.Add(row);
        //            }

        //            foreach (var item in CreditsalseInfos)
        //            {
        //                row = dt.NewRow();
        //                row["CustomerCode"] = item.Item1;
        //                row["Name"] = item.Item2;
        //                row["Date"] = item.Item3.ToString("dd MMM yyyy");
        //                row["InvoiceNo"] = item.Item4;
        //                // item.ExpenseItem.Description;
        //                row["GrandTotal"] = item.Item5;
        //                row["DiscountAmount"] = item.Item6 - item.Rest.Item4;
        //                row["Amount"] = item.Item7;
        //                row["RecAmt"] = item.Rest.Item1;
        //                row["DueAmount"] = item.Rest.Item2;
        //                row["SalesType"] = "Credit Sales";
        //                row["AdjustAmt"] = 0;
        //                row["TotalOffer"] = item.Rest.Item4;
        //                row["InstallmentPeriod"] = item.Rest.Item7;

        //                dt.Rows.Add(row);
        //            }




        //            foreach (var item in ReturnInfos)
        //            {
        //                row = dtReturn.NewRow();
        //                row["CustomerCode"] = item.Item1;
        //                row["Name"] = item.Item2;
        //                row["Date"] = item.Item3.ToString("dd MMM yyyy");
        //                row["InvoiceNo"] = item.Item4;
        //                // item.ExpenseItem.Description;
        //                row["GrandTotal"] = item.Item5;
        //                row["DiscountAmount"] = item.Item6 - item.Rest.Item4;
        //                row["Amount"] = item.Item7;
        //                row["RecAmt"] = item.Rest.Item1;
        //                row["DueAmount"] = item.Rest.Item2;
        //                row["SalesType"] = "Cash Sales";
        //                row["AdjustAmt"] = item.Rest.Item3;
        //                row["TotalOffer"] = item.Rest.Item4;

        //                dtReturn.Rows.Add(row);
        //            }


        //            dt.TableName = "dtOrder";
        //            _dataSet = new DataSet();
        //            _dataSet.Tables.Add(dt);

        //            dtReturn.TableName = "dtReturnOrder";

        //            _dataSet.Tables.Add(dtReturn);

        //            GetCommonParameters(userName, concernID);

        //            _reportParameter = new ReportParameter("NetSales", NetSales.ToString());
        //            _reportParameters.Add(_reportParameter);
        //            _reportParameter = new ReportParameter("NetSalesRec", NetSalesRec.ToString());
        //            _reportParameters.Add(_reportParameter);
        //            _reportParameter = new ReportParameter("NetSalesDue", NetSalesDue.ToString());
        //            _reportParameters.Add(_reportParameter);




        //            if (period == "Daily")
        //                _reportParameter = new ReportParameter("Month", "Sales report for the date from : " + fromDate.ToString("dd MMM yyyy") + " To " + toDate.ToString("dd MMM yyyy"));
        //            else if (period == "Monthly")
        //                _reportParameter = new ReportParameter("Month", "Sales report for the Month : " + fromDate.ToString("MMM, yyyy"));
        //            else if (period == "Yearly")
        //                _reportParameter = new ReportParameter("Month", "Sales report for the Year : " + fromDate.ToString("yyyy"));
        //            _reportParameters.Add(_reportParameter);
        //            return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Sales\\rptMonthlyOrder.rdlc");
        //        }
        //        #endregion

        //        #region Details Report
        //        else
        //        {
        //            decimal length = 0m, width = 0m, AreaSqftPerPcs = 0m, AreaSqft = 0m;
        //            var salseDetailInfos = _salesOrderService.GetSalesDetailReportByConcernID(fromDate, toDate, concernID);

        //            //IEnumerable<Tuple<DateTime, string, string, string, decimal, decimal, decimal, Tuple<decimal, decimal, decimal, decimal, decimal, string, string, Tuple<int>>>> CreditsalseDetailInfos =
        //            //                    _creditSalesOrderService.GetCreditSalesDetailReportByConcernID(fromDate, toDate, concernID);

        //            IEnumerable<Tuple<DateTime, string, string, string, decimal, decimal, decimal, Tuple<decimal, decimal, decimal, decimal, decimal, string, string, Tuple<int, decimal>>>> returnDetailInfos =
        //                                                  _returnOrderService.GetReturnDetailReportByConcernID(fromDate, toDate, concernID);


        //            TransactionalDataSet.dtCustomerWiseReturnDataTable dtReturn = new TransactionalDataSet.dtCustomerWiseReturnDataTable();
        //            TransactionalDataSet.dtCustomerWiseSalesDataTable dt = new TransactionalDataSet.dtCustomerWiseSalesDataTable();
        //            int SOrderID = 0, CreditSaleID = 0;

        //            decimal TotalDueSales = 0, AdjAmount = 0;
        //            decimal GrandTotal = 0;
        //            decimal TotalDis = 0;
        //            decimal NetTotal = 0;
        //            decimal RecAmt = 0;
        //            decimal CurrDue = 0;
        //            DataRow row = null;
        //            foreach (var item in salseDetailInfos)
        //            {
        //                //StockDetail std = oSTDList.FirstOrDefault(x => x.SDetailID == grd.StockDetailID);
        //                row = dt.NewRow();
        //                if (SOrderID != item.SOrderID)
        //                {
        //                    TotalDueSales = TotalDueSales + item.PaymentDue;
        //                    GrandTotal = GrandTotal + (decimal)item.GrandTotal;
        //                    TotalDis = TotalDis + (decimal)item.NetDiscount;
        //                    NetTotal = NetTotal + (decimal)item.TotalAmount;
        //                    RecAmt = RecAmt + (decimal)item.RecAmount;
        //                    CurrDue = CurrDue + (decimal)item.PaymentDue;
        //                    AdjAmount = AdjAmount + item.AdjAmount;
        //                }

        //                row["SalesDate"] = item.Date;
        //                row["InvoiceNo"] = item.InvoiceNo;
        //                row["ProductName"] = item.ProductName;
        //                row["CName"] = item.CustomerName;
        //                row["SalesPrice"] = item.UnitPrice;
        //                row["NetAmt"] = item.TotalAmount;
        //                row["GrandTotal"] = item.GrandTotal;
        //                row["TotalDis"] = item.NetDiscount;
        //                row["NetTotal"] = item.UTAmount;
        //                row["PaidAmount"] = item.RecAmount;
        //                row["RemainingAmt"] = item.PaymentDue;
        //                row["Quantity"] = (int)(item.Quantity / item.ConvertValue);
        //                row["ChildQty"] = Convert.ToInt32(item.Quantity % item.ConvertValue);
        //                row["IMENo"] = item.IMEI;
        //                row["ColorInfo"] = item.ColorName;
        //                row["SalesType"] = string.Empty;
        //                row["AdjAmount"] = item.AdjAmount;
        //                row["UnitName"] = item.UnitName;
        //                row["Code"] = item.CustomerCode;
        //                row["IdCode"] = item.IDCode;
        //                row["TotalArea"] = item.Quantity * (item.SalesPerCartonSft / item.ConvertValue);
        //                row["PerCartonSft"] = item.SalesPerCartonSft;
        //                // row["SizeName"] = item.SizeName;

        //                if (item.CategoryName.ToLower().Equals("tiles"))
        //                {
        //                    var area = item.SizeName.Split('x');
        //                    length = Math.Round(Convert.ToDecimal(area[0]) / 2.5m);
        //                    width = Math.Round(Convert.ToDecimal(area[1]) / 2.5m);
        //                    //AreaSqftPerPcs = Math.Round(((length * width) / 144m), 4); //sqft
        //                    //AreaSqft = Math.Round(AreaSqftPerPcs * item.Quantity, 4);

        //                    row["SizeName"] = length + "x" + width;

        //                }
        //                else
        //                {
        //                    row["SizeName"] = item.SizeName;

        //                }


        //                dt.Rows.Add(row);
        //                SOrderID = item.SOrderID;



        //                //if (item.CategoryName.ToLower().Equals("tiles"))
        //                //{
        //                //    //var area = item.SizeName.Split('x');
        //                //    //length = Math.Round(Convert.ToDecimal(area[0]) / 2.5m);
        //                //    //width = Math.Round(Convert.ToDecimal(area[1]) / 2.5m);
        //                //    //AreaSqftPerPcs = Math.Round(((length * width) / 144m), 4); //sqft
        //                //    //AreaSqft = Math.Round(AreaSqftPerPcs * item.Quantity, 4);
        //                //    //row["TotalArea"] = item.TotalSFT;
        //                //    //row["SizeName"] = length + "x" + width;

        //                //    row["TotalPrice"] = (item.Quantity * (item.SalesPerCartonSft / item.ConvertValue) * item.UnitPrice);
        //                //}
        //                //else
        //                //{
        //                //    row["SizeName"] = "N/A";
        //                //    row["TotalPrice"] = ((item.Quantity / item.ConvertValue) * item.MRP);
        //                //}


        //            }

        //            //For Credit Sales
        //            //foreach (var grd in CreditsalseDetailInfos)
        //            //{
        //            //    //StockDetail std = oSTDList.FirstOrDefault(x => x.SDetailID == grd.StockDetailID);

        //            //    if (CreditSaleID != grd.Rest.Rest.Item1)
        //            //    {
        //            //        TotalDueSales = TotalDueSales + (decimal)grd.Rest.Item4;
        //            //        GrandTotal = GrandTotal + (decimal)grd.Item7;
        //            //        TotalDis = TotalDis + (decimal)grd.Rest.Item1;
        //            //        NetTotal = NetTotal + (decimal)grd.Rest.Item2;
        //            //        RecAmt = RecAmt + (decimal)grd.Rest.Item3;
        //            //        CurrDue = CurrDue + (decimal)grd.Rest.Item4;
        //            //    }

        //            //    CreditSaleID = grd.Rest.Rest.Item1;
        //            //    dt.Rows.Add(grd.Item1.ToString("dd MMM yyyy"), grd.Item2, grd.Item3, grd.Item4, grd.Item5, grd.Item6, grd.Item7, grd.Rest.Item1, grd.Rest.Item2, grd.Rest.Item3, grd.Rest.Item4, grd.Rest.Item5, grd.Rest.Item6, grd.Rest.Item7, "Credit Sales", 0m);

        //            //}




        //            decimal ReturnTotalDueSales = 0, ReturnAdjAmount = 0;
        //            decimal ReturnGrandTotal = 0;
        //            decimal ReturnTotalDis = 0;
        //            decimal ReturnNetTotal = 0;
        //            decimal ReturnRecAmt = 0;
        //            decimal ReturnCurrDue = 0;


        //            foreach (var grd in returnDetailInfos)
        //            {
        //                //StockDetail std = oSTDList.FirstOrDefault(x => x.SDetailID == grd.StockDetailID);

        //                if (SOrderID != grd.Rest.Rest.Item1)
        //                {
        //                    ReturnTotalDueSales = ReturnTotalDueSales + (decimal)grd.Rest.Item4;
        //                    ReturnGrandTotal = ReturnGrandTotal + (decimal)grd.Item7;
        //                    ReturnTotalDis = ReturnTotalDis + (decimal)grd.Rest.Item1;
        //                    ReturnNetTotal = ReturnNetTotal + (decimal)grd.Rest.Item2;
        //                    ReturnRecAmt = ReturnRecAmt + (decimal)grd.Rest.Item3;
        //                    ReturnCurrDue = ReturnCurrDue + (decimal)grd.Rest.Item4;
        //                    ReturnAdjAmount = ReturnAdjAmount + grd.Rest.Rest.Item2;
        //                }

        //                SOrderID = grd.Rest.Rest.Item1;
        //                dtReturn.Rows.Add(grd.Item1.ToString("dd MMM yyyy"), grd.Item2, grd.Item3, grd.Item4, grd.Item5, grd.Item6, grd.Item7, grd.Rest.Item1, grd.Rest.Item2, grd.Rest.Item3, grd.Rest.Item4, grd.Rest.Item5, grd.Rest.Item6, grd.Rest.Item7, "Sales", grd.Rest.Rest.Item2);

        //            }



        //            decimal NetTotalDueSales = 0, NetAdjAmount = 0;
        //            decimal NetGrandTotal = 0;
        //            decimal NetTotalDis = 0;
        //            decimal NetNetTotal = 0;
        //            decimal NetRecAmt = 0;
        //            decimal NetCurrDue = 0;


        //            NetTotalDueSales = TotalDueSales - ReturnTotalDueSales;
        //            NetAdjAmount = AdjAmount - ReturnAdjAmount;
        //            NetGrandTotal = GrandTotal - ReturnGrandTotal;
        //            NetTotalDis = TotalDis - ReturnTotalDis;
        //            NetNetTotal = NetTotal - ReturnNetTotal;
        //            NetRecAmt = RecAmt - ReturnRecAmt;
        //            NetCurrDue = CurrDue - ReturnCurrDue;

        //            dt.TableName = "dtCustomerWiseSales";
        //            _dataSet = new DataSet();
        //            _dataSet.Tables.Add(dt);


        //            dtReturn.TableName = "dtCustomerWiseReturn";

        //            _dataSet.Tables.Add(dtReturn);







        //            GetCommonParameters(userName, concernID);
        //            if (period == "Daily")
        //                _reportParameter = new ReportParameter("Date", "Sales details for the date from : " + fromDate.ToString("dd MMM yyyy") + " To " + toDate.ToString("dd MMM yyyy"));
        //            else if (period == "Monthly")
        //                _reportParameter = new ReportParameter("Date", "Sales details for the Month : " + fromDate.ToString("MMM, yyyy"));
        //            else if (period == "Yearly")
        //                _reportParameter = new ReportParameter("Date", "Sales details the Year : " + fromDate.ToString("yyyy"));

        //            _reportParameters.Add(_reportParameter);





        //            _reportParameter = new ReportParameter("GrandTotal", GrandTotal.ToString());
        //            _reportParameters.Add(_reportParameter);

        //            _reportParameter = new ReportParameter("TotalDis", TotalDis.ToString());
        //            _reportParameters.Add(_reportParameter);

        //            _reportParameter = new ReportParameter("NetTotal", NetTotal.ToString());
        //            _reportParameters.Add(_reportParameter);

        //            _reportParameter = new ReportParameter("RecAmt", RecAmt.ToString());
        //            _reportParameters.Add(_reportParameter);

        //            _reportParameter = new ReportParameter("CurrDue", CurrDue.ToString());
        //            _reportParameters.Add(_reportParameter);

        //            _reportParameter = new ReportParameter("AdjAmount", AdjAmount.ToString());
        //            _reportParameters.Add(_reportParameter);



        //            _reportParameter = new ReportParameter("ReturnGrandTotal", ReturnGrandTotal.ToString());
        //            _reportParameters.Add(_reportParameter);

        //            _reportParameter = new ReportParameter("ReturnTotalDis", ReturnTotalDis.ToString());
        //            _reportParameters.Add(_reportParameter);

        //            _reportParameter = new ReportParameter("ReturnNetTotal", ReturnNetTotal.ToString());
        //            _reportParameters.Add(_reportParameter);

        //            _reportParameter = new ReportParameter("ReturnRecAmt", ReturnRecAmt.ToString());
        //            _reportParameters.Add(_reportParameter);

        //            _reportParameter = new ReportParameter("ReturnCurrDue", ReturnCurrDue.ToString());
        //            _reportParameters.Add(_reportParameter);

        //            _reportParameter = new ReportParameter("ReturnAdjAmount", ReturnAdjAmount.ToString());
        //            _reportParameters.Add(_reportParameter);




        //            _reportParameter = new ReportParameter("NetGrandTotal", NetGrandTotal.ToString());
        //            _reportParameters.Add(_reportParameter);

        //            _reportParameter = new ReportParameter("NetTotalDis", NetTotalDis.ToString());
        //            _reportParameters.Add(_reportParameter);

        //            _reportParameter = new ReportParameter("NetNetTotal", NetNetTotal.ToString());
        //            _reportParameters.Add(_reportParameter);

        //            _reportParameter = new ReportParameter("NetRecAmt", NetRecAmt.ToString());
        //            _reportParameters.Add(_reportParameter);

        //            _reportParameter = new ReportParameter("NetCurrDue", NetCurrDue.ToString());
        //            _reportParameters.Add(_reportParameter);

        //            _reportParameter = new ReportParameter("NetAdjAmount", NetAdjAmount.ToString());
        //            _reportParameters.Add(_reportParameter);


        //            return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Sales\\rptSalesDetails.rdlc");
        //        }
        //        #endregion

        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}



        public byte[] PurchaseReport(DateTime fromDate, DateTime toDate, string userName, int concernID, int reportType, string period, EnumPurchaseType PurchaseType)
        {
            try
            {
                #region Summary
                if (reportType == 1)//Summary
                {
                    var purchaseInfos = _purchaseOrderService.GetPurchaseReport(fromDate, toDate, PurchaseType);

                    DataRow row = null;

                    TransactionalDataSet.dtReceiveOrderDataTable dt = new TransactionalDataSet.dtReceiveOrderDataTable();
                    //BasicDataSet.dtEmployeesInfoDataTable dtEmployeesInfo = new BasicDataSet.dtEmployeesInfoDataTable();

                    foreach (var item in purchaseInfos)
                    {
                        row = dt.NewRow();
                        row["CompanyCode"] = item.Item1;
                        row["Name"] = item.Item2;
                        row["OrderDare"] = item.Item3.ToString("dd MMM yyyy");
                        row["ChallanNo"] = item.Item4;
                        // item.ExpenseItem.Description;
                        row["GrandTotal"] = item.Item5;
                        row["DisAmt"] = item.Item6;
                        row["TotalAmt"] = item.Item7;
                        row["RecAmt"] = item.Rest.Item1;
                        row["DueAmt"] = item.Rest.Item2;
                        dt.Rows.Add(row);
                    }

                    dt.TableName = "dtReceiveOrder";
                    _dataSet = new DataSet();
                    _dataSet.Tables.Add(dt);

                    GetCommonParameters(userName, concernID);
                    if (PurchaseType == EnumPurchaseType.Purchase)
                        _reportParameter = new ReportParameter("Month", "Purchase report for the date from : " + fromDate.ToString("dd MMM yyyy") + " To " + toDate.ToString("dd MMM yyyy"));
                    else if (PurchaseType == EnumPurchaseType.ProductReturn)
                        _reportParameter = new ReportParameter("Month", "Purchase Return report for the date from : " + fromDate.ToString("dd MMM yyyy") + " To " + toDate.ToString("dd MMM yyyy"));

                    _reportParameters.Add(_reportParameter);

                    return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Purchase\\rptPurchaseOrder.rdlc");
                }
                #endregion

                else
                {
                    var Purchases = _purchaseOrderService.ProductWisePurchaseDetailsReport(0, 0, 0, fromDate, toDate, PurchaseType);

                    decimal TotalDuePurchase = 0;
                    decimal GrandTotal = 0;
                    decimal TotalDis = 0;
                    decimal NetTotal = 0;
                    decimal RecAmt = 0;
                    decimal CurrDue = 0;


                    if (Purchases != null)
                    {
                        TransactionalDataSet.dtSuppWiseDataDataTable dt = new TransactionalDataSet.dtSuppWiseDataDataTable();

                        DataRow row = null;

                        int POrderID = 0;

                        decimal AreaPerCarton = 0m, RatePerSqft = 0m, TotalSqft = 0m, AreaPerPcs = 0m;
                        decimal[] sizeXY = null;

                        foreach (var item in Purchases)
                        {
                            if (POrderID != item.POrderID)
                            {
                                TotalDuePurchase = TotalDuePurchase + item.PaymentDue;
                                GrandTotal = GrandTotal + item.GrandTotal;
                                TotalDis = TotalDis + item.NetDiscount;
                                NetTotal = NetTotal + item.NetTotal;
                                RecAmt = RecAmt + item.RecAmt;
                                CurrDue = CurrDue + item.PaymentDue;

                            }
                            row = dt.NewRow();
                            row["PurchaseDate"] = item.Date;
                            row["ChallanNo"] = item.ChallanNo;
                            row["ProductName"] = item.ProductName;
                            row["ProductCode"] = item.ProductCode;
                            row["PurchaseRate"] = Math.Round(item.PurchaseRate * item.ConvertValue, 4);
                            row["DisAmt"] = item.NetDiscount;
                            row["NetAmt"] = item.NetTotal;
                            row["GrandTotal"] = item.GrandTotal;
                            row["TotalDis"] = item.NetDiscount;
                            row["NetTotal"] = item.TotalAmount;
                            row["PaidAmt"] = item.RecAmt;
                            row["RemainingAmt"] = item.PaymentDue;
                            row["Quantity"] = Convert.ToInt32(item.Quantity / item.ConvertValue);
                            row["UnitQty"] = Convert.ToInt32(item.Quantity % item.ConvertValue);
                            row["ChasisNo"] = string.Empty;
                            row["Model"] = item.CategoryName;
                            row["Color"] = item.ColorName;
                            row["PPOffer"] = item.PPOffer;
                            row["DamageIMEI"] = item.ConvertValue.ToString();
                            row["UnitName"] = item.ParentUnitName;
                            row["SizeName"] = item.SizeName;
                            var sa = item.SizeName.ToLower().Split('x');
                            if (sa.Length == 2)
                            {
                                //sizeXY = Array.ConvertAll(item.SizeName.ToLower().Split('x'), decimal.Parse);
                                AreaPerCarton = item.PurchaseCSft; //Math.Round((((sizeXY[0] * sizeXY[1]) / 10000m) * item.ConvertValue) * 10.76m, 2);
                                AreaPerPcs = item.PurchaseCSft / item.ConvertValue; //Math.Round((((sizeXY[0] * sizeXY[1]) / 10000m)) * 10.76m, 4);
                                row["AreaPerCarton"] = AreaPerCarton;
                                row["RatePerSqft"] = AreaPerPcs > 0 ? (Math.Round(item.MRP / AreaPerPcs, 2)) : 0m;
                                TotalSqft = Math.Round(AreaPerCarton * (item.Quantity / item.ConvertValue), 2);
                                row["TotalSqft"] = TotalSqft;
                            }
                            dt.Rows.Add(row);

                            POrderID = item.POrderID;
                        }

                        dt.TableName = "dtSuppWiseData";
                        _dataSet = new DataSet();
                        _dataSet.Tables.Add(dt);

                        GetCommonParameters(userName, concernID);
                        if (PurchaseType == EnumPurchaseType.ProductReturn)
                        {

                            if (period == "Daily")
                                _reportParameter = new ReportParameter("Date", "Purchase Return details for the date from : " + fromDate.ToString("dd MMM yyyy") + " To " + toDate.ToString("dd MMM yyyy"));
                            else if (period == "Monthly")
                                _reportParameter = new ReportParameter("Date", "Purchase Return details for the Month : " + fromDate.ToString("MMM, yyyy"));
                            else if (period == "Yearly")
                                _reportParameter = new ReportParameter("Date", "Purchase Return details the Year : " + fromDate.ToString("yyyy"));
                        }
                        else
                        {
                            if (period == "Daily")
                                _reportParameter = new ReportParameter("Date", "Purchase details for the date from : " + fromDate.ToString("dd MMM yyyy") + " To " + toDate.ToString("dd MMM yyyy"));
                            else if (period == "Monthly")
                                _reportParameter = new ReportParameter("Date", "Purchase details for the Month : " + fromDate.ToString("MMM, yyyy"));
                            else if (period == "Yearly")
                                _reportParameter = new ReportParameter("Date", "Purchase details the Year : " + fromDate.ToString("yyyy"));
                        }

                        _reportParameters.Add(_reportParameter);

                        _reportParameter = new ReportParameter("GrandTotal", GrandTotal.ToString());
                        _reportParameters.Add(_reportParameter);

                        _reportParameter = new ReportParameter("TotalDis", TotalDis.ToString());
                        _reportParameters.Add(_reportParameter);

                        _reportParameter = new ReportParameter("NetTotal", NetTotal.ToString());
                        _reportParameters.Add(_reportParameter);

                        _reportParameter = new ReportParameter("RecAmt", RecAmt.ToString());
                        _reportParameters.Add(_reportParameter);

                        _reportParameter = new ReportParameter("CurrDue", CurrDue.ToString());
                        _reportParameters.Add(_reportParameter);

                    }
                    return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Purchase\\rptPurchaseDetails.rdlc");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public byte[] SalesInvoiceReportPrint(SOrder oOrder, string userName, int concernID)
        {
            try
            {

                DataTable orderdDT = new DataTable();
                TransactionalDataSet.dtInvoiceDataTable dt = new TransactionalDataSet.dtInvoiceDataTable();
                TransactionalDataSet.dtWarrentyDataTable dtWarrenty = new TransactionalDataSet.dtWarrentyDataTable();
                Customer customer = _customerService.GetCustomerById(oOrder.CustomerID);
                Employee employee = _EmployeeService.GetEmployeeById(customer.EmployeeID);

                //string TerritoryName = _TerritoryServiceService.GetTerritoryNameById(oOrder.Customer.TerritoryID);

                //string DepotName = _DepotService.GetDepotNameById(oOrder.DepotID);

                ProductWisePurchaseModel product = null;
                List<ProductWisePurchaseModel> warrentyList = new List<ProductWisePurchaseModel>();
                ProductWisePurchaseModel warrentyModel = null;
                DataRow row = null;
                string Warrenty = string.Empty;
                string IMEIs = string.Empty;
                int Count = 0;
                bool IsTiles = false;
                decimal AreaPerCarton = 0m, Length = 0m, Width = 0m, AreaPerPcs = 0m;
                decimal[] sizeXY = null;

                #region LINQ
                var ProductInfos = from sd in oOrder.SOrderDetails
                                   join std in _stockdetailService.GetAll() on sd.SDetailID equals std.SDetailID
                                   join col in _ColorServce.GetAllColor() on std.ColorID equals col.ColorID
                                   join p in _productService.GetAllProductIQueryable() on sd.ProductID equals p.ProductID
                                   select new
                                   {
                                       ProductID = p.ProductID,
                                       ProductName = p.ProductName,
                                       Quantity = sd.Quantity,
                                       UnitPrice = sd.UnitPrice,
                                       SalesRate = sd.UTAmount,
                                       UTAmount = sd.UTAmount,
                                       PPDPercentage = sd.PPDPercentage,
                                       PPDAmount = sd.PPDAmount,
                                       PPOffer = sd.PPOffer,
                                       IMENO = std.IMENO,
                                       ColorName = col.Name,
                                       CompanyName = p.CompanyName,
                                       CategoryName = p.CategoryName,
                                       Compressor = sd.Compressor,
                                       Motor = sd.Motor,
                                       Service = sd.Service,
                                       Spareparts = sd.Spareparts,
                                       Panel = sd.Panel,
                                       p.ConvertValue,
                                       ChildUnit = p.ChildUnitName,
                                       ParentUnit = p.ParentUnitName,
                                       p.ProductCode,
                                       p.SizeName,
                                       sd.SFTRate,
                                       sd.TotalSFT,
                                       p.SalesCSft,
                                       DiaSizeName = p.DiaSizeName

                                   };

                var GroupProductInfos = from w in ProductInfos
                                        group w by new
                                        {
                                            w.ProductCode,
                                            w.ProductName,
                                            w.CategoryName,
                                            w.ColorName,
                                            w.CompanyName,
                                            w.UnitPrice,
                                            w.PPDAmount,
                                            w.PPDPercentage,
                                            w.PPOffer,
                                            w.ParentUnit,
                                            w.ChildUnit,
                                            w.SizeName,
                                            w.ConvertValue,
                                            w.SFTRate,
                                            w.SalesCSft
                                        } into g
                                        select new
                                        {
                                            ProductCode = g.Key.ProductCode,
                                            ProductName = g.Key.ProductName,
                                            CategoryName = g.Key.CategoryName,
                                            ColorName = g.Key.ColorName,
                                            CompanyName = g.Key.CompanyName,
                                            SizeName = g.Key.SizeName,
                                            DiaSizeName = g.Select(i => i.DiaSizeName).FirstOrDefault(),
                                            UnitPrice = g.Key.UnitPrice,
                                            PPDPercentage = g.Key.PPDPercentage,
                                            PPDAmount = g.Key.PPDAmount,
                                            PPOffer = g.Key.PPOffer,
                                            SalesRate = g.Key.UnitPrice,
                                            Quantity = g.Sum(i => i.Quantity),
                                            TotalAmt = g.Sum(i => i.UTAmount),
                                            //TotalAmt = g.Select(i => i.UTAmount).FirstOrDefault(),
                                            Compressor = g.Select(i => i.Compressor).FirstOrDefault(),
                                            Motor = g.Select(i => i.Motor).FirstOrDefault(),
                                            Service = g.Select(i => i.Service).FirstOrDefault(),
                                            Spareparts = g.Select(i => i.Spareparts).FirstOrDefault(),
                                            Panel = g.Select(i => i.Panel).FirstOrDefault(),
                                            IMENOs = g.Select(i => i.IMENO).ToList(),
                                            ParentUnit = g.Key.ParentUnit,
                                            ChildUnit = g.Key.ChildUnit,
                                            g.Key.ConvertValue,
                                            g.Key.SFTRate,
                                            TotalSFT = g.Select(i => i.TotalSFT).FirstOrDefault(),
                                            g.Key.SalesCSft,
                                        };
                #endregion

                foreach (var item in GroupProductInfos)
                {
                    row = dt.NewRow();
                    row["ProductName"] = item.ProductName + " ," + item.CategoryName;
                    row["Quantity"] = Math.Truncate(item.Quantity / item.ConvertValue);
                    row["UnitQty"] = item.Quantity % item.ConvertValue;
                    row["Rate"] = item.UnitPrice * item.ConvertValue;
                    row["Discount"] = item.PPDAmount * item.ConvertValue;
                    row["Amount"] = item.TotalAmt;
                    row["DisPer"] = item.PPDPercentage;
                    row["DisAmt"] = item.PPDAmount;
                    row["ChasisNo"] = string.Empty;
                    row["Color"] = item.ColorName;
                    row["EngineNo"] = item.ChildUnit;
                    row["OfferValue"] = item.PPOffer;
                    row["CategoryName"] = item.CategoryName;
                    row["UnitName"] = item.ParentUnit;
                    row["ProductCode"] = item.ProductCode;
                    row["CompanyName"] = item.CompanyName;
                    row["WDisAmt"] = item.UnitPrice - item.PPDAmount;
                    row["TPDisAmt"] = item.PPDAmount * item.Quantity;

                    if (item.CategoryName.ToLower().Equals("tiles"))
                    {
                        row["Rate"] = item.SFTRate; //item.UnitPrice * item.ConvertValue;
                        row["Amount"] = item.SFTRate * item.TotalSFT; //item.TotalAmt;
                        sizeXY = Array.ConvertAll(item.SizeName.ToLower().Split('x'), decimal.Parse);
                        Length = (sizeXY[0] / 2.5m); //ft
                        Width = (sizeXY[1] / 2.5m); //ft
                        row["SizeName"] = Length.ToString() + "X" + Width.ToString();
                        //AreaPerCarton = Math.Round((((Length * Width) / 144m) * item.ConvertValue), 2); //sq ft
                        //AreaPerPcs = Math.Round((((Length * Width) / 144m)), 4); //sq ft
                        row["AreaPerCarton"] = item.SalesCSft;
                        row["RatePerSqft"] = item.SFTRate; //Math.Round(item.UnitPrice / AreaPerPcs, 2);
                        row["TotalArea"] = item.TotalSFT; //Math.Round(item.Quantity * AreaPerPcs, 2);
                        IsTiles = true;
                    }
                    else
                    {
                        row["SizeName"] = item.SizeName;
                        row["DiaSizeName"] = item.DiaSizeName;
                        row["RatePerSqft"] = Math.Round(item.UnitPrice * item.ConvertValue, 2);
                    }
                    dt.Rows.Add(row);

                    #region Warrenty
                    //foreach (var IMEI in item.IMENOs)
                    //{
                    //    Count++;
                    //    if (item.IMENOs.Count() != Count)
                    //        IMEIs = IMEIs + IMEI + Environment.NewLine;
                    //    else
                    //        IMEIs = IMEIs + IMEI;
                    //}

                    //if (!string.IsNullOrEmpty(item.Compressor))
                    //    Warrenty = "Compressor: " + item.Compressor + Environment.NewLine;
                    //if (!string.IsNullOrEmpty(item.Motor))
                    //    Warrenty = Warrenty + "Motor: " + item.Motor + Environment.NewLine;
                    //if (!string.IsNullOrEmpty(item.Panel))
                    //    Warrenty = Warrenty + "Panel: " + item.Panel + Environment.NewLine;
                    //if (!string.IsNullOrEmpty(item.Service))
                    //    Warrenty = Warrenty + "Service: " + item.Service + Environment.NewLine;
                    //if (!string.IsNullOrEmpty(item.Spareparts))
                    //    Warrenty = Warrenty + "Spareparts: " + item.Spareparts;


                    //dtWarrenty.Rows.Add(item.ProductName, "IMEI", Warrenty);

                    //IMEIs = string.Empty;
                    //Warrenty = string.Empty;
                    //Count = 0;
                    #endregion
                }

                if (dt != null && (dt.Rows != null && dt.Rows.Count > 0))
                    orderdDT = dt.AsEnumerable().OrderBy(o => (String)o["ProductName"]).CopyToDataTable();
                dt.TableName = "dtInvoice";
                _dataSet = new DataSet();
                _dataSet.Tables.Add(dt);
                dtWarrenty.TableName = "dtWarrenty";
                _dataSet.Tables.Add(dtWarrenty);

                //string sInwodTk = TakaFormat(Convert.ToDouble(oOrder.RecAmount));

                string sInwodTk = TakaFormat(Convert.ToDouble(oOrder.TotalAmount.ToString()));
                sInwodTk = sInwodTk.Replace("Taka", "");
                sInwodTk = sInwodTk.Replace("Only", "Taka Only");
                GetCommonParameters(userName, concernID);

                _reportParameter = new ReportParameter("CartonP", oOrder.CartonPercentage.ToString("0.00") + "%");
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("CashP", oOrder.CashBPercentage.ToString("0.00") + "%");
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("YearlyP", oOrder.YearlyBPercentage.ToString("0.00") + "%");
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("CartonAmt", oOrder.CartonAmt.ToString("0.00"));
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("CashAmt", oOrder.CashBAmt.ToString("0.00"));
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("YearlyAmt", oOrder.YearlyBnsAmt.ToString("0.00"));
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("AfterCarton", oOrder.AfterCartonAmt.ToString("0.00"));
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("AfterCash", oOrder.AfterCashAmt.ToString("0.00"));
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("AfterYearly", oOrder.AfterYearlyAmt.ToString("0.00"));
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("TDiscount", oOrder.TDAmount.ToString("0.00"));
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("Total", (oOrder.TotalAmount).ToString("0.00"));
                _reportParameters.Add(_reportParameter);

                //_reportParameter = new ReportParameter("GTotal", (oOrder.GrandTotal + (oOrder.Customer.TotalDue - oOrder.PaymentDue)).ToString());

                _reportParameter = new ReportParameter("GTotal", "0.00");
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("Paid", oOrder.RecAmount.ToString());
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("CurrDue", (oOrder.PaymentDue).ToString());
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("InvoiceNo", oOrder.InvoiceNo);
                _reportParameters.Add(_reportParameter);

                //_reportParameter = new ReportParameter("TotalDue", oOrder.TotalDue.ToString());
                _reportParameter = new ReportParameter("TotalDue", (oOrder.PrevDue + oOrder.PaymentDue).ToString("F2"));
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("PrevDue", (oOrder.PrevDue).ToString("F2"));
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("InvoiceDate", oOrder.InvoiceDate.ToString("dd MMM yyyy"));
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("RemindDate", customer.RemindDate != null ? customer.RemindDate.Value.ToString("dd MMM yyyy") : "");
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("Company", customer.CompanyName);
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("CAddress", customer.Address);
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("Remarks", oOrder.Remarks);
                _reportParameters.Add(_reportParameter);


                _reportParameter = new ReportParameter("Name", customer.Name);
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("CusCode", customer.Code);
                _reportParameters.Add(_reportParameter);

                //_reportParameter = new ReportParameter("Territory", TerritoryName);
                //_reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("MobileNo", customer.ContactNo);
                _reportParameters.Add(_reportParameter);
                _reportParameter = new ReportParameter("PrintDate", GetClientDateTime());
                _reportParameters.Add(_reportParameter);
                _reportParameter = new ReportParameter("Header", "Sales Invoice");
                _reportParameters.Add(_reportParameter);

                //_reportParameter = new ReportParameter("Depot", DepotName);
                //_reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("RefSO", employee.Name);
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("RefSOMobile", employee.ContactNo);
                _reportParameters.Add(_reportParameter);

                if (oOrder.Terms == EnumTerms.Cash)
                {
                    _reportParameter = new ReportParameter("Terms", "Cash");
                    _reportParameters.Add(_reportParameter);
                }
                else
                {
                    _reportParameter = new ReportParameter("Terms", "Credit");
                    _reportParameters.Add(_reportParameter);
                }

                _reportParameter = new ReportParameter("InWordTK", sInwodTk);
                _reportParameters.Add(_reportParameter);

                if (concernID == (int)EnumSisterConcern.KT_Feed)
                {

                    return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Sales\\KTFeedSalesInvoice.rdlc");
                }
                return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Sales\\KTSalesInvoice.rdlc");

            }

            catch (Exception Ex)
            {
                throw Ex;
            }
        }

        public byte[] ChallanReportPrint(SOrder oOrder, string userName, int concernID)
        {
            try
            {

                DataTable orderdDT = new DataTable();
                TransactionalDataSet.dtInvoiceDataTable dt = new TransactionalDataSet.dtInvoiceDataTable();
                TransactionalDataSet.dtWarrentyDataTable dtWarrenty = new TransactionalDataSet.dtWarrentyDataTable();
                Customer customer = _customerService.GetCustomerById(oOrder.CustomerID);

                ProductWisePurchaseModel product = null;
                List<ProductWisePurchaseModel> warrentyList = new List<ProductWisePurchaseModel>();
                ProductWisePurchaseModel warrentyModel = null;

                string Warrenty = string.Empty;
                string IMEIs = string.Empty;
                int Count = 0;
                DataRow row = null;
                decimal AreaPerCarton = 0m, Length = 0m, Width = 0m, AreaPerPcs = 0m;
                decimal[] sizeXY = null;
                #region LINQ
                var ProductInfos = from sd in oOrder.SOrderDetails
                                   join std in _stockdetailService.GetAll() on sd.SDetailID equals std.SDetailID
                                   join col in _ColorServce.GetAllColor() on std.ColorID equals col.ColorID
                                   join p in _productService.GetProducts() on sd.ProductID equals p.ProductID
                                   select new
                                   {
                                       ProductID = p.ProductID,
                                       ProductName = p.ProductName,
                                       p.SalesCSft,
                                       p.ProductCode,
                                       ParentUnit = p.ParentUnitName,
                                       ChildUnit = p.ChildUnitName,
                                       Quantity = sd.Quantity,
                                       UnitPrice = sd.UnitPrice,
                                       SalesRate = sd.UTAmount,
                                       UTAmount = sd.UTAmount,
                                       PPDPercentage = sd.PPDPercentage,
                                       PPDAmount = sd.PPDAmount,
                                       PPOffer = sd.PPOffer,
                                       //IMENO = std.IMENO,
                                       ColorName = col.Name,
                                       CompanyName = p.CompanyName,
                                       CategoryName = p.CategoryName,
                                       p.SizeName,
                                       Compressor = sd.Compressor,
                                       Motor = sd.Motor,
                                       Service = sd.Service,
                                       Spareparts = sd.Spareparts,
                                       Panel = sd.Panel,
                                       ConvertValue = p.ConvertValue,
                                       sd.TotalSFT,
                                       sd.SFTRate
                                   };

                var GroupProductInfos = from w in ProductInfos
                                        group w by new
                                        {
                                            w.ProductName,
                                            w.CategoryName,
                                            w.ColorName,
                                            w.CompanyName,
                                            w.UnitPrice,
                                            w.PPDAmount,
                                            w.PPDPercentage,
                                            w.PPOffer,
                                            w.ConvertValue,
                                            w.ProductCode,
                                            w.ParentUnit,
                                            w.ChildUnit,
                                            w.SizeName,
                                            w.SalesCSft,
                                            w.SFTRate
                                        } into g
                                        select new
                                        {
                                            ProductName = g.Key.ProductName,
                                            CategoryName = g.Key.CategoryName,
                                            ColorName = g.Key.ColorName,
                                            CompanyName = g.Key.CompanyName,
                                            g.Key.SizeName,
                                            g.Key.ProductCode,
                                            g.Key.ParentUnit,
                                            g.Key.ChildUnit,
                                            g.Key.SalesCSft,
                                            UnitPrice = g.Key.UnitPrice,
                                            PPDPercentage = g.Key.PPDPercentage,
                                            PPDAmount = g.Key.PPDAmount,
                                            PPOffer = g.Key.PPOffer,
                                            SalesRate = g.Key.UnitPrice,
                                            Quantity = g.Sum(i => i.Quantity),
                                            TotalAmt = g.Sum(i => i.UTAmount),
                                            Compressor = g.Select(i => i.Compressor).FirstOrDefault(),
                                            Motor = g.Select(i => i.Motor).FirstOrDefault(),
                                            Service = g.Select(i => i.Service).FirstOrDefault(),
                                            Spareparts = g.Select(i => i.Spareparts).FirstOrDefault(),
                                            Panel = g.Select(i => i.Panel).FirstOrDefault(),
                                            //IMENOs = g.Select(i => i.IMENO).ToList(),
                                            g.Key.ConvertValue,
                                            g.Key.SFTRate,
                                            TotalSFT = g.Select(i => i.TotalSFT).FirstOrDefault()
                                        };
                #endregion

                foreach (var item in GroupProductInfos)
                {

                    //foreach (var IMEI in item.IMENOs)
                    //{
                    //    Count++;
                    //    if (item.IMENOs.Count() != Count)
                    //        IMEIs = IMEIs + IMEI + Environment.NewLine;
                    //    else
                    //        IMEIs = IMEIs + IMEI;
                    //}

                    //dt.Rows.Add(item.ProductName, item.Quantity, "Pcs", item.UnitPrice, "0 %", item.TotalAmt, item.PPDPercentage, item.PPDAmount, IMEIs, item.ColorName, "", item.PPOffer, item.CompanyName + " & " + item.CategoryName);

                    //if (!string.IsNullOrEmpty(item.Compressor))
                    //    Warrenty = "Compressor: " + item.Compressor + Environment.NewLine;
                    //if (!string.IsNullOrEmpty(item.Motor))
                    //    Warrenty = Warrenty + "Motor: " + item.Motor + Environment.NewLine;
                    //if (!string.IsNullOrEmpty(item.Panel))
                    //    Warrenty = Warrenty + "Panel: " + item.Panel + Environment.NewLine;
                    //if (!string.IsNullOrEmpty(item.Service))
                    //    Warrenty = Warrenty + "Service: " + item.Service + Environment.NewLine;
                    //if (!string.IsNullOrEmpty(item.Spareparts))
                    //    Warrenty = Warrenty + "Spareparts: " + item.Spareparts;


                    //dtWarrenty.Rows.Add(item.ProductName, "IMEI", Warrenty);

                    row = dt.NewRow();
                    row["ProductName"] = item.ProductName;
                    row["Quantity"] = Math.Truncate(item.Quantity / item.ConvertValue);
                    row["UnitQty"] = Math.Round(item.Quantity % item.ConvertValue);
                    row["Rate"] = item.UnitPrice;
                    row["Discount"] = item.PPDAmount;
                    row["Amount"] = item.TotalAmt;
                    row["DisPer"] = item.PPDPercentage;
                    row["DisAmt"] = item.PPDAmount;
                    row["ChasisNo"] = string.Empty;
                    row["Color"] = item.ColorName;
                    row["EngineNo"] = string.Empty;
                    row["OfferValue"] = item.PPOffer;
                    row["CategoryName"] = item.CategoryName;
                    row["UnitName"] = item.ParentUnit;
                    row["ProductCode"] = item.ProductCode;
                    row["SizeName"] = item.SizeName;
                    row["CompanyName"] = item.CompanyName;
                    if (item.CategoryName.ToLower().Equals("tiles"))
                    {
                        sizeXY = Array.ConvertAll(item.SizeName.ToLower().Split('x'), decimal.Parse);
                        Length = (sizeXY[0] / 2.5m); //ft
                        Width = (sizeXY[1] / 2.5m); //ft
                        row["SizeName"] = Length.ToString() + "X" + Width.ToString();
                        //AreaPerCarton = item.SalesCSft; //Math.Round((((Length * Width) / 144m) * item.ConvertValue), 2); //sq ft
                        //AreaPerPcs = AreaPerCarton / item.ConvertValue; //Math.Round((((Length * Width) / 144m)), 4); //sq ft
                        row["AreaPerCarton"] = item.SalesCSft;
                        row["RatePerSqft"] = item.SFTRate;// Math.Round(item.UnitPrice / AreaPerPcs, 2);
                        row["TotalArea"] = item.TotalSFT;// Math.Round(item.Quantity * AreaPerPcs, 2);
                    }
                    IMEIs = string.Empty;
                    Warrenty = string.Empty;
                    Count = 0;

                    dt.Rows.Add(row);
                }

                if (dt != null && (dt.Rows != null && dt.Rows.Count > 0))
                    orderdDT = dt.AsEnumerable().OrderBy(o => (String)o["ProductName"]).CopyToDataTable();
                dt.TableName = "dtInvoice";
                _dataSet = new DataSet();
                _dataSet.Tables.Add(dt);
                dtWarrenty.TableName = "dtWarrenty";
                _dataSet.Tables.Add(dtWarrenty);

                string sInwodTk = TakaFormat(Convert.ToDouble(oOrder.RecAmount));
                GetCommonParameters(userName, concernID);

                _reportParameter = new ReportParameter("TDiscount", oOrder.TDAmount.ToString("0.00"));
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("Total", (oOrder.TotalAmount).ToString("0.00"));
                _reportParameters.Add(_reportParameter);

                //_reportParameter = new ReportParameter("GTotal", (oOrder.GrandTotal + (oOrder.Customer.TotalDue - oOrder.PaymentDue)).ToString());

                _reportParameter = new ReportParameter("GTotal", "0.00");
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("Paid", oOrder.RecAmount.ToString());
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("CurrDue", (oOrder.PaymentDue).ToString());
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("InvoiceNo", oOrder.InvoiceNo);
                _reportParameters.Add(_reportParameter);

                //_reportParameter = new ReportParameter("TotalDue", oOrder.TotalDue.ToString());
                _reportParameter = new ReportParameter("TotalDue", customer.TotalDue.ToString());
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("InvoiceDate", oOrder.InvoiceDate.ToString());
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("RemindDate", customer.RemindDate != null ? customer.RemindDate.Value.ToString("dd MMM yyyy") : "");
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("Company", customer.CompanyName);
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("CAddress", customer.Address);
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("Name", customer.Name);
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("MobileNo", customer.ContactNo);
                _reportParameters.Add(_reportParameter);
                _reportParameter = new ReportParameter("PrintDate", GetClientDateTime());
                _reportParameters.Add(_reportParameter);
                //_reportParameter =new ReportParameter("InWordTK", sInwodTk);
                //_reportParameters.Add(_reportParameter);

                //if (concernID == (int)EnumSisterConcern.NOKIA_CONCERNID || concernID == (int)EnumSisterConcern.WALTON_CONCERNID || concernID == (int)EnumSisterConcern.NOKIA_STORE_MAGURA_CONCERNID)
                //    return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Sales\\rptMSalesInvoice.rdlc");
                //else if (concernID == (int)EnumSisterConcern.KINGSTAR_CONCERNID)
                //    return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Sales\\rptKSalesInvoice.rdlc");
                //else if (concernID == (int)EnumSisterConcern.HAVEN_ENTERPRISE_CONCERNID || concernID == (int)EnumSisterConcern.HAWRA_ENTERPRISE_CONCERNID)

                return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Sales\\Challan.rdlc");
                // else
                //  return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Sales\\rptSSSalesInvoice.rdlc");
            }

            catch (Exception Ex)
            {
                throw Ex;
            }
        }

        public byte[] SalesInvoiceReport(SOrder oOrder, string userName, int concernID)
        {
            return SalesInvoiceReportPrint(oOrder, userName, concernID);
        }

        public byte[] ChallanReport(SOrder oOrder, string userName, int concernID)
        {
            return ChallanReportPrint(oOrder, userName, concernID);
        }


        public byte[] SalesInvoiceReport(int oOrderID, string userName, int concernID)
        {
            SOrder oOrder = new SOrder();
            oOrder = _salesOrderService.GetSalesOrderById(Convert.ToInt32(oOrderID));
            oOrder.SOrderDetails = _salesOrderDetailService.GetSOrderDetailsBySOrderID(Convert.ToInt32(oOrderID)).ToList();

            return SalesInvoiceReportPrint(oOrder, userName, concernID);

        }


        public byte[] ChallanReport(int oOrderID, string userName, int concernID)
        {
            SOrder oOrder = new SOrder();
            oOrder = _salesOrderService.GetSalesOrderById(Convert.ToInt32(oOrderID));
            oOrder.SOrderDetails = _salesOrderDetailService.GetSOrderDetailsBySOrderID(Convert.ToInt32(oOrderID)).ToList();

            return ChallanReportPrint(oOrder, userName, concernID);

        }
        public byte[] CreditSalesInvoiceReportPrint(CreditSale oOrder, string userName, int concernID)
        {
            try
            {

                DataTable orderdDT = new DataTable();
                TransactionalDataSet.CreditSalesInfoDataTable dt = new TransactionalDataSet.CreditSalesInfoDataTable();
                TransactionalDataSet.dtWarrentyDataTable dtWarrenty = new TransactionalDataSet.dtWarrentyDataTable();
                Customer customer = _customerService.GetCustomerById(oOrder.CustomerID);

                DataRow oSDRow = null;
                Product product = null;
                StockDetail oSTDetail = null;
                int count = 1;
                string Warrenty = string.Empty;

                foreach (CreditSalesSchedule item in oOrder.CreditSalesSchedules)
                {
                    oSDRow = dt.NewRow();

                    oSDRow["ScheduleNo"] = count;
                    oSDRow["PaymentDate"] = item.PaymentStatus == "Paid" ? Convert.ToDateTime(item.PaymentDate).ToString("dd MMM yyyy") : "";
                    oSDRow["Balance"] = item.Balance;
                    oSDRow["InstallmetAmt"] = item.InstallmentAmt;
                    oSDRow["ClosingBalance"] = item.ClosingBalance;
                    oSDRow["PaymentStatus"] = item.PaymentStatus;
                    oSDRow["ScheduleDate"] = Convert.ToDateTime(item.MonthDate).ToString("dd MMM yyyy");
                    dt.Rows.Add(oSDRow);
                    count++;


                }

                dt.TableName = "CreditSalesInfo";
                _dataSet = new DataSet();
                _dataSet.Tables.Add(dt);

                TransactionalDataSet.CSalesProductDataTable CSProductDT = new TransactionalDataSet.CSalesProductDataTable();
                DataRow oCSPRow = null;
                int nCOunt = 1;

                foreach (CreditSaleDetails item in oOrder.CreditSaleDetails)
                {
                    oSTDetail = _stockdetailService.GetById(item.StockDetailID);
                    product = _productService.GetProductById(item.ProductID);

                    oCSPRow = CSProductDT.NewRow();
                    oCSPRow["SLNo"] = nCOunt.ToString();
                    oCSPRow["PName"] = product.ProductName;
                    oCSPRow["IMENO"] = oSTDetail.Color.Name;
                    oCSPRow["Qty"] = item.Quantity.ToString();
                    oCSPRow["UnitPrice"] = item.UnitPrice.ToString();
                    oCSPRow["PPOffer"] = item.PPOffer.ToString();
                    oCSPRow["TotalAmt"] = item.UTAmount.ToString();
                    CSProductDT.Rows.Add(oCSPRow);

                    if (!string.IsNullOrEmpty(item.Compressor))
                        Warrenty = "Compressor: " + item.Compressor + Environment.NewLine;
                    if (!string.IsNullOrEmpty(item.Motor))
                        Warrenty = Warrenty + "Motor: " + item.Motor + Environment.NewLine;
                    if (!string.IsNullOrEmpty(item.Panel))
                        Warrenty = Warrenty + "Panel: " + item.Panel + Environment.NewLine;
                    if (!string.IsNullOrEmpty(item.Service))
                        Warrenty = Warrenty + "Service: " + item.Service + Environment.NewLine;
                    if (!string.IsNullOrEmpty(item.Spareparts))
                        Warrenty = Warrenty + "Spareparts: " + item.Spareparts;

                    dtWarrenty.Rows.Add(product.ProductName, oSTDetail.IMENO, Warrenty);
                    Warrenty = string.Empty;
                }

                CSProductDT.TableName = "CSalesProduct";
                _dataSet.Tables.Add(CSProductDT);

                dtWarrenty.TableName = "dtWarrenty";
                _dataSet.Tables.Add(dtWarrenty);


                GetCommonParameters(userName, concernID);

                _reportParameter = new ReportParameter("InvoiceNo", oOrder.InvoiceNo);
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("IssueDate", oOrder.SalesDate.ToString());
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("ProductName", "");
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("CustomerName", customer.Name);
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("CAddress", customer.Address);
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("CContactNo", customer.ContactNo);
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("Remarks", "Note: " + oOrder.Remarks);
                _reportParameters.Add(_reportParameter);

                string salesprice = (oOrder.TSalesAmt - oOrder.TotalOffer).ToString("F");
                _reportParameter = new ReportParameter("SalesPrice", salesprice);
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("DownPayment", oOrder.DownPayment.ToString("F"));
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("RemainingAmt", (oOrder.TSalesAmt - oOrder.TotalOffer - oOrder.DownPayment).ToString("F"));
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("CustomerCode", customer.Code);
                _reportParameters.Add(_reportParameter);

                if (concernID == (int)EnumSisterConcern.SAMSUNG_ELECTRA_CONCERNID)
                {
                    return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "CreditSales\\CreditSalesInvoiceSS.rdlc");
                }
                else
                {
                    _reportParameter = new ReportParameter("HelpLine", "HELPLINE: 16267" + Environment.NewLine + "waltonbd.com");
                    _reportParameters.Add(_reportParameter);
                    return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "CreditSales\\CreditSalesInvoice.rdlc");
                }

            }

            catch (Exception Ex)
            {
                throw Ex;
            }
        }
        public byte[] CreditSalesInvoiceReport(CreditSale oOrder, string userName, int concernID)
        {
            return CreditSalesInvoiceReportPrint(oOrder, userName, concernID);
        }

        public byte[] CreditSalesInvoiceReportByID(int oOrderID, string userName, int concernID)
        {
            CreditSale oOrder = new CreditSale();
            oOrder = _creditSalesOrderService.GetSalesOrderById(oOrderID);
            oOrder.CreditSalesSchedules = _creditSalesOrderService.GetSalesOrderSchedules(oOrderID).ToList();
            oOrder.CreditSaleDetails = _creditSalesOrderService.GetSalesOrderDetails(oOrderID).ToList();

            return CreditSalesInvoiceReportPrint(oOrder, userName, concernID);

        }
        public byte[] CustomeWiseSalesReport(DateTime fromDate, DateTime toDate, string userName, int concernID, int reportType, int CustomerID)
        {
            try
            {

                var salesInfos = _salesOrderService.GetSalesDetailReportByCustomerID(fromDate, toDate, CustomerID);
                //DataRow row = null;

                decimal TotalDueSales = 0;
                decimal GrandTotal = 0;
                decimal TotalDis = 0;
                decimal NetTotal = 0;

                decimal RecAmt = 0;
                decimal CurrDue = 0;
                decimal CusTotalDue = 0;
                int customerID = 0, SOrderID = 0;
                TransactionalDataSet.dtCustomerWiseSalesDataTable dt = new TransactionalDataSet.dtCustomerWiseSalesDataTable();
                TransactionalDataSet.dtCustomerDataTable cdt = new TransactionalDataSet.dtCustomerDataTable();
                _dataSet = new DataSet();
                DataRow srow = null;
                DataRow crow = null;

                foreach (var item in salesInfos)
                {
                    srow = dt.NewRow();
                    if (customerID != item.CustomerID)
                    {
                        CusTotalDue = item.CustomerTotalDue;
                        crow = cdt.NewRow();
                        crow["CCode"] = item.CustomerCode;
                        crow["CName"] = item.CustomerName;
                        crow["CompanyName"] = item.CustCompanyName;
                        crow["CusType"] = item.CustomerType;
                        crow["ContactNo"] = item.CustomerContactNo;
                        crow["NID"] = item.CustomerNID;
                        crow["Address"] = item.CustomerAddress;
                        crow["TotalDue"] = item.CustomerTotalDue;
                        cdt.Rows.Add(crow);
                    }

                    customerID = item.CustomerID;

                    if (SOrderID != item.SOrderID)
                    {
                        TotalDueSales = TotalDueSales + (decimal)item.PaymentDue;
                        GrandTotal = GrandTotal + (decimal)item.Grandtotal;
                        TotalDis = TotalDis + (decimal)item.NetDiscount;
                        NetTotal = NetTotal + (decimal)item.TotalAmount;
                        RecAmt = RecAmt + (decimal)item.RecAmount;
                        CurrDue = CurrDue + (decimal)item.PaymentDue;
                    }

                    SOrderID = item.SOrderID;

                    srow["SalesDate"] = item.InvoiceDate.ToString("dd MMM yyyy");
                    srow["InvoiceNo"] = item.InvoiceNo;
                    srow["ProductName"] = item.ProductName;
                    srow["CName"] = item.CustomerName;
                    srow["SalesPrice"] = item.UnitPrice;
                    srow["NetAmt"] = item.UTAmount;
                    srow["GrandTotal"] = item.Grandtotal;
                    srow["TotalDis"] = item.NetDiscount;
                    srow["NetTotal"] = item.TotalAmount;
                    srow["PaidAmount"] = item.RecAmount;
                    srow["RemainingAmt"] = item.PaymentDue;
                    srow["Quantity"] = item.Quantity;
                    srow["IMENo"] = item.IMENO;
                    srow["ColorInfo"] = item.ColorName;
                    srow["SalesType"] = "Cash Sales";
                    srow["AdjAmount"] = item.AdjAmount;
                    dt.Rows.Add(srow);
                }

                cdt.TableName = "dtCustomer";
                _dataSet.Tables.Add(cdt);

                dt.TableName = "dtCustomerWiseSales";
                //_dataSet = new DataSet();
                _dataSet.Tables.Add(dt);

                GetCommonParameters(userName, concernID);

                _reportParameter = new ReportParameter("Date", "Sales report for the date of : " + fromDate.ToString("dd MMM yyyy") + " to " + toDate.ToString("dd MMM yyyy"));
                _reportParameters.Add(_reportParameter);

                //_reportParameter = new ReportParameter("PrintedBy", Global.CurrentUser.UserName);
                //_reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("TotalDue", CusTotalDue.ToString());
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("TotalDueUpTo", "0.00");
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("GrandTotal", (GrandTotal).ToString());
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("TotalDis", TotalDis.ToString());
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("NetTotal", (NetTotal).ToString());
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("RecAmt", RecAmt.ToString());
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("CurrDue", CurrDue.ToString());
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("FreeAmt", "0.00");
                _reportParameters.Add(_reportParameter);

                //GetCommonParameters(userName, concernID);

                return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Sales\\rptCustomerWiseSales.rdlc");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public byte[] SuplierWisePurchaseReport(DateTime fromDate, DateTime toDate, string userName, int concernID, int reportType, int SupplierID)
        {
            try
            {
                IEnumerable<Tuple<DateTime, string, string, decimal, decimal, decimal, decimal, Tuple<decimal, decimal, decimal, decimal, decimal, string, Tuple<string, string, string, string, string, decimal, decimal>>>> purchaseInfos = _purchaseOrderService.GetPurchaseDetailReportBySupplierID(fromDate, toDate, concernID, SupplierID);
                //DataRow row = null;

                decimal TotalDueSales = 0;
                decimal GrandTotal = 0;
                decimal TotalDis = 0;
                decimal NetTotal = 0;
                decimal RecAmt = 0;
                decimal CurrDue = 0;
                decimal CusTotalDue = 0;
                string ChallanNo = "";

                string SuppCode = "";

                TransactionalDataSet.dtSuppWiseDataDataTable dt = new TransactionalDataSet.dtSuppWiseDataDataTable();
                TransactionalDataSet.dtSupplierDataTable cdt = new TransactionalDataSet.dtSupplierDataTable();

                _dataSet = new DataSet();

                foreach (var grd in purchaseInfos)
                {
                    if (SuppCode != grd.Rest.Item7.Item1)
                    {
                        CusTotalDue = grd.Item6;
                        cdt.Rows.Add(grd.Rest.Item7.Item1, grd.Rest.Item7.Item2, grd.Rest.Item7.Item5, grd.Rest.Item7.Item4, grd.Rest.Item7.Item3, grd.Item6);
                        //cdt.TableName = "dtSupplier";
                        //_dataSet.Tables.Add(cdt);
                    }

                    SuppCode = grd.Rest.Item7.Item1;

                    if (ChallanNo != grd.Item2)
                    {
                        TotalDueSales = TotalDueSales + (decimal)grd.Rest.Item4;
                        GrandTotal = GrandTotal + (decimal)grd.Item7;
                        TotalDis = TotalDis + (decimal)grd.Rest.Item1;
                        NetTotal = NetTotal + (decimal)grd.Rest.Item2;
                        RecAmt = RecAmt + (decimal)grd.Rest.Item3;
                        CurrDue = CurrDue + (decimal)grd.Rest.Item4;
                    }

                    ChallanNo = grd.Item2;
                    dt.Rows.Add(grd.Item1.ToString("dd MMM yyyy"), grd.Item2, grd.Item3, grd.Item4, grd.Item5, grd.Item4, grd.Item7, grd.Rest.Item1, grd.Rest.Item2, grd.Rest.Item3, grd.Rest.Item4, 1, grd.Rest.Item6, "", grd.Rest.Item6, grd.Rest.Item7.Item7);

                }

                _dataSet.Tables.Add(cdt);
                cdt.TableName = "dtSupplier";

                dt.TableName = "dtSuppWiseData";
                //_dataSet = new DataSet();
                _dataSet.Tables.Add(dt);

                GetCommonParameters(userName, concernID);

                _reportParameter = new ReportParameter("Date", "Purchase report for the date of : " + fromDate.ToString("dd MMM yyyy") + " to " + toDate.ToString("dd MMM yyyy"));
                _reportParameters.Add(_reportParameter);

                //_reportParameter = new ReportParameter("PrintedBy", Global.CurrentUser.UserName);
                //_reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("TotalDue", CusTotalDue.ToString());
                _reportParameters.Add(_reportParameter);

                //_reportParameter = new ReportParameter("TotalDueUpTo", "Total Due Upto Date: " + "0.00");
                //_reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("GrandTotal", (GrandTotal).ToString());
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("TotalDis", TotalDis.ToString());
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("NetTotal", (NetTotal).ToString());
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("RecAmt", RecAmt.ToString());
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("CurrDue", CurrDue.ToString());
                _reportParameters.Add(_reportParameter);

                //_reportParameter = new ReportParameter("FreeAmt", "0.00");
                //_reportParameters.Add(_reportParameter);


                return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Purchase\\rptSupplierWiseDetails.rdlc");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public byte[] MOWiseSalesReport(DateTime fromDate, DateTime toDate, string userName, int concernID, int MOID, int RptType)
        {
            try
            {
                IEnumerable<Tuple<string, DateTime, string, string, decimal, decimal, Tuple<decimal, decimal, decimal, decimal>>> MOWiseSalesInfos = _salesOrderService.GetSalesDetailReportByMOID(fromDate, toDate, concernID, MOID, RptType);

                TransactionalDataSet.MOSDetailsDataTable dt = new TransactionalDataSet.MOSDetailsDataTable();

                _dataSet = new DataSet();
                foreach (var grd in MOWiseSalesInfos)
                {
                    dt.Rows.Add(grd.Item1, grd.Item2.ToString("dd MMM yyyy"), grd.Item3, grd.Item4, grd.Item5, grd.Item6, grd.Item7.Item1, grd.Item7.Item2, grd.Item7.Item3, grd.Item7.Item4);

                }

                dt.TableName = "MOSDetails";
                _dataSet.Tables.Add(dt);

                GetCommonParameters(userName, concernID);

                _reportParameter = new ReportParameter("DateRange", "SR wise sales summary report for the date of : " + fromDate.ToString("dd MMM yyyy") + " to " + toDate.ToString("dd MMM yyyy"));
                _reportParameters.Add(_reportParameter);


                return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Sales\\MOWiseSalesDetails.rdlc");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public byte[] MOWiseCustomerDueRpt(string userName, int concernID, int MOID, int RptType)
        {
            try
            {
                var MOWiseCustomerDue = _salesOrderService.GetMOWiseCustomerDueRpt(concernID, MOID, RptType);

                TransactionalDataSet.MOWiseDueRptDataTable dt = new TransactionalDataSet.MOWiseDueRptDataTable();

                _dataSet = new DataSet();
                foreach (var grd in MOWiseCustomerDue)
                {
                    dt.Rows.Add(grd.Item1, grd.Item2, grd.Item3, grd.Item4, grd.Item5, grd.Item6, grd.Item7, (grd.Item6 - grd.Item7));
                }

                dt.TableName = "MOWiseDueRpt";
                _dataSet.Tables.Add(dt);

                GetCommonParameters(userName, concernID);
                return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Sales\\MOWiseDueRpt.rdlc");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public byte[] StockDetailReport(string userName, int concernID, int reportType, int CompanyID, int CategoryID, int ProductID, int GodownID)
        {
            try
            {

                var stockInfos = _StockServce.GetforStockReport(userName, concernID, reportType, CompanyID, CategoryID, ProductID, GodownID, 0).ToList();
                DataRow row = null;
                string reportName = string.Empty;

                TransactionalDataSet.StockInfoDataTable dtStockInfoDT = new TransactionalDataSet.StockInfoDataTable();

                string IMENO = "";
                int count;
                //StockDetails = _stockdetailService.GetAll();
                foreach (var item in stockInfos)
                {
                    row = dtStockInfoDT.NewRow();
                    row["UnitType"] = "Pice";
                    //row["StockCode"] = item.Item1;
                    row["ProName"] = item.ProductName;
                    row["CompanyName"] = item.CompanyName;
                    row["CategoryName"] = item.CategoryName;
                    row["ModelName"] = item.SizeName;
                    row["Quantity"] = item.Quantity;
                    row["PRate"] = item.MRP;
                    row["TotalPrice"] = (item.MRP * item.Quantity);
                    row["Godown"] = item.GodownName;

                    var SDetails = _StockServce.GetStockDetailsByID(item.StockID);
                    //var SDetails = StockDetails.Where(i=>i.StockID==item.Item1).ToList();

                    IMENO = "";
                    count = 0;

                    foreach (var itemime in SDetails)
                    {
                        if (count == 0)
                            IMENO = IMENO + itemime.Item3;
                        else
                            IMENO = IMENO + System.Environment.NewLine + itemime.Item3;
                        count++;
                    }

                    row["StockCode"] = IMENO;

                    dtStockInfoDT.Rows.Add(row);
                }

                dtStockInfoDT.TableName = "StockInfo";
                _dataSet = new DataSet();
                _dataSet.Tables.Add(dtStockInfoDT);

                GetCommonParameters(userName, concernID);

                if (reportType == 0)
                {
                    reportName = "Stock\\StockInfo.rdlc";
                }
                else if (reportType == 1)
                {
                    reportName = "Stock\\rptCompanyWiseStock.rdlc";
                }
                else if (reportType == 2)
                {
                    reportName = "Stock\\rptCategoryWiseStock.rdlc";
                }

                else if (reportType == 3)
                {
                    reportName = "Stock\\rptGodownWiseStock.rdlc";
                }
                return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, reportName);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public byte[] StockSummaryReport(string userName, int concernID, int reportType, int CompanyID, int CategoryID, int ProductID, int GodownID, EnumProductStockType ProductStockType)
        {
            try
            {
                DataRow row = null;
                string reportName = string.Empty;
                string IMENO = "";
                decimal length = 0m, width = 0m, AreaSqftPerPcs = 0m, AreaSqft = 0m;
                TransactionalDataSet.StockInfoDataTable dtStockInfoDT = new TransactionalDataSet.StockInfoDataTable();

                var stockInfos = _StockServce.GetforStockReport(userName, concernID, reportType, CompanyID, CategoryID, ProductID, GodownID, ProductStockType).ToList();
                //var InhouseDamageProductDetails = _productService.GetAllDamageProductFromDetail();
                //var CompanyDamageProductDetails = _purchaseOrderService.GetDamageReturnProductDetails(0, 0);
                List<ProductWisePurchaseModel> InHouseProducts = new List<ProductWisePurchaseModel>();
                List<ProductWisePurchaseModel> CompanyDamageStocks = new List<ProductWisePurchaseModel>();
                //if (reportType == 0)//Product Wise
                //{
                #region
                //InHouseProducts = (from p in InhouseDamageProductDetails
                //                   group p by new { p.Item1, p.Item2, p.Item3, p.Item6, p.Item7, p.Rest.Item5 } into g
                //                   select new ProductWisePurchaseModel
                //                   {
                //                       ProductCode = g.Key.Item2,
                //                       ProductName = g.Key.Item3,
                //                       CategoryName = g.Key.Item6,
                //                       CompanyName = g.Key.Item7,
                //                       Quantity = g.Count(),
                //                       TotalAmount = g.Sum(i => i.Rest.Item3),
                //                   }).ToList();

                //CompanyDamageStocks = (from cds in CompanyDamageProductDetails
                //                       group cds by new { cds.ProductID, cds.ColorID, cds.ProductCode, cds.ProductName, cds.CompanyName, cds.CategoryName } into g
                //                       select new ProductWisePurchaseModel
                //                       {
                //                           ProductCode = g.Key.ProductCode,
                //                           ProductName = g.Key.ProductName,
                //                           CategoryName = g.Key.CategoryName,
                //                           CompanyName = g.Key.CompanyName,
                //                           Quantity = g.Count(),
                //                           TotalAmount = g.Sum(i => i.MRP),
                //                       }).ToList();
                #endregion

                bool IsHistoryShow = false;


                if ((concernID != 1 || concernID != 5 || concernID != 6) && reportType == 0)
                    IsHistoryShow = true;
                foreach (var item in stockInfos)
                {
                    row = dtStockInfoDT.NewRow();
                    row["UnitType"] = item.ParentUnitName;
                    row["ProName"] = item.ProductName;
                    row["CompanyName"] = item.CompanyName;
                    row["CategoryName"] = item.CategoryName;
                    row["ModelName"] = item.SizeName;
                    row["Quantity"] = Math.Truncate(item.Quantity / item.ConvertValue);
                    row["ChildQty"] = Math.Round(item.Quantity % item.ConvertValue);
                    row["PRate"] = item.MRP;

                    row["StockCode"] = item.ProductCode;
                    row["SalesRate"] = item.SRate;
                    row["CreditSRate"] = item.TotalCreditSR6;// 6 months
                    row["TotalCreditPrice"] = (item.TotalCreditSR6 * item.Quantity);
                    row["StockType"] = "Normal Stock";
                    row["CrSR3"] = item.TotalCreditSR3;
                    row["TotalCrSR3"] = (item.TotalCreditSR3 * item.Quantity);
                    row["CrSR12"] = item.TotalCreditSR12;
                    row["TotalCrSR12"] = (item.TotalCreditSR12 * item.Quantity);
                    row["Godown"] = item.GodownName;
                    row["ColorName"] = item.ColorName;
                    if (item.CategoryName.ToLower().Equals("tiles"))
                    {
                        var area = item.SizeName.Split('x');
                        length = Math.Round(Convert.ToDecimal(area[0]) / 2.5m);
                        width = Math.Round(Convert.ToDecimal(area[1]) / 2.5m);
                        //AreaSqftPerPcs = Math.Round(((length * width) / 144m), 4); //sqft
                        //AreaSqft = Math.Round(AreaSqftPerPcs * item.Quantity, 4);
                        row["TotalArea"] = item.TotalSFT;
                        row["SizeName"] = length + "x" + width;

                        row["TotalPrice"] = (item.TotalSFT * item.MRP);
                    }
                    else
                    {
                        row["SizeName"] = item.SizeName;
                        row["TotalPrice"] = ((item.Quantity / item.ConvertValue) * item.MRP);
                    }

                    row["ChildUnitName"] = item.ChildUnitName;

                    //if (IsHistoryShow)
                    //{
                    //    row["StockHistory"] = _StockServce.GetStockProductsHistory(item.Item1);
                    //}
                    dtStockInfoDT.Rows.Add(row);
                }
                //    }
                //else if (reportType == 1) //Company wise
                //{

                //    #region
                //    //InHouseProducts = (from p in InhouseDamageProductDetails
                //    //                   group p by new { p.Item6, p.Item7, p.Rest.Item5 } into g
                //    //                   select new ProductWisePurchaseModel
                //    //                   {
                //    //                       CategoryName = g.Key.Item6,
                //    //                       CompanyName = g.Key.Item7,
                //    //                       Quantity = g.Count(),
                //    //                       TotalAmount = g.Sum(i => i.Rest.Item3),
                //    //                   }).ToList();

                //    //CompanyDamageStocks = (from cds in CompanyDamageProductDetails
                //    //                       group cds by new { cds.CompanyName, cds.CategoryName } into g
                //    //                       select new ProductWisePurchaseModel
                //    //                       {
                //    //                           CategoryName = g.Key.CategoryName,
                //    //                           CompanyName = g.Key.CompanyName,
                //    //                           Quantity = g.Count(),
                //    //                           TotalAmount = g.Sum(i => i.MRP),
                //    //                       }).ToList();
                //    #endregion

                //    var Normalstocks = (from ns in stockInfos
                //                        group ns by new { Company = ns.CompanyName, Category = ns.CategoryName } into g
                //                        select new ProductWisePurchaseModel
                //                        {
                //                            CategoryName = g.Key.Category,
                //                            CompanyName = g.Key.Company,
                //                            Quantity = g.Sum(i => i.ParentQuantity),
                //                            TotalAmount = g.Sum(i => i.SRate * i.Quantity),
                //                            TotalCreditSR3 = g.Sum(i => i.TotalCreditSR3 * i.Quantity),
                //                            TotalCreditSR6 = g.Sum(i => i.TotalCreditSR6 * i.Quantity),
                //                            TotalCreditSR12 = g.Sum(i => i.TotalCreditSR12 * i.Quantity),
                //                        }).ToList();

                //    foreach (var item in Normalstocks)
                //    {
                //        row = dtStockInfoDT.NewRow();
                //        row["UnitType"] = item.ParentUnit;
                //        //row["StockCode"] = item.Item1;
                //        row["ProName"] = item.ProductName;
                //        row["CompanyName"] = item.CompanyName;
                //        row["CategoryName"] = item.CategoryName;
                //        row["ModelName"] = item.ProductName;
                //        row["Quantity"] = item.Quantity;
                //        row["PRate"] = 0;
                //        row["TotalPrice"] = item.TotalAmount;
                //        row["StockCode"] = item.ProductCode; ;
                //        row["SalesRate"] = 0m;
                //        row["CreditSRate"] = 0m;// 6 months
                //        row["TotalCreditPrice"] = item.TotalCreditSR6;
                //        row["StockType"] = "Normal Stock";
                //        row["CrSR3"] = 0m;
                //        row["TotalCrSR3"] = item.TotalCreditSR3;
                //        row["CrSR12"] = 0m;
                //        row["TotalCrSR12"] = item.TotalCreditSR12;
                //        dtStockInfoDT.Rows.Add(row);
                //    }
                //}
                //else if (reportType == 2) //category wise
                //{
                //    #region
                //    //InHouseProducts = (from p in InhouseDamageProductDetails
                //    //                   group p by new { p.Item6 } into g
                //    //                   select new ProductWisePurchaseModel
                //    //                   {
                //    //                       CategoryName = g.Key.Item6,
                //    //                       Quantity = g.Count(),
                //    //                       TotalAmount = g.Sum(i => i.Rest.Item3),
                //    //                   }).ToList();

                //    //CompanyDamageStocks = (from cds in CompanyDamageProductDetails
                //    //                       group cds by new { cds.CategoryName } into g
                //    //                       select new ProductWisePurchaseModel
                //    //                       {
                //    //                           CategoryName = g.Key.CategoryName,
                //    //                           Quantity = g.Count(),
                //    //                           TotalAmount = g.Sum(i => i.MRP),
                //    //                       }).ToList();
                //    #endregion

                //    var Normalstocks = (from ns in stockInfos
                //                        group ns by new { Category = ns.CategoryName } into g
                //                        select new ProductWisePurchaseModel
                //                        {
                //                            CategoryName = g.Key.Category,
                //                            Quantity = g.Sum(i => i.ParentQuantity),
                //                            TotalAmount = g.Sum(i => i.MRP * i.Quantity/i.ConvertValue),
                //                            TotalCreditSR3 = g.Sum(i => i.TotalCreditSR3 * i.Quantity),
                //                            TotalCreditSR6 = g.Sum(i => i.TotalCreditSR6 * i.Quantity),
                //                            TotalCreditSR12 = g.Sum(i => i.TotalCreditSR12 * i.Quantity),
                //                        }).ToList();

                //    foreach (var item in Normalstocks)
                //    {
                //        row = dtStockInfoDT.NewRow();
                //        row["UnitType"] = item.ParentUnit;
                //        //row["StockCode"] = item.Item1;
                //        row["ProName"] = item.ProductName;
                //        row["CompanyName"] = item.CompanyName;
                //        row["CategoryName"] = item.CategoryName;
                //        row["ModelName"] = item.ProductName;
                //        row["Quantity"] = item.Quantity;
                //        row["PRate"] = 0;
                //        row["TotalPrice"] = item.TotalAmount;
                //        row["StockCode"] = item.ProductCode; ;
                //        row["SalesRate"] = 0m;
                //        row["CreditSRate"] = 0m;// 6 months
                //        row["TotalCreditPrice"] = item.TotalCreditSR6;
                //        row["StockType"] = "Normal Stock";
                //        row["CrSR3"] = 0m;
                //        row["TotalCrSR3"] = item.TotalCreditSR3;
                //        row["CrSR12"] = 0m;
                //        row["TotalCrSR12"] = item.TotalCreditSR12;



                //        dtStockInfoDT.Rows.Add(row);
                //    }
                //}

                //else if (reportType == 3) //GodownName wise
                //{
                //    #region
                //    //InHouseProducts = (from p in InhouseDamageProductDetails
                //    //                   group p by new { p.Item6 } into g
                //    //                   select new ProductWisePurchaseModel
                //    //                   {
                //    //                       CategoryName = g.Key.Item6,
                //    //                       Quantity = g.Count(),
                //    //                       TotalAmount = g.Sum(i => i.Rest.Item3),
                //    //                   }).ToList();

                //    //CompanyDamageStocks = (from cds in CompanyDamageProductDetails
                //    //                       group cds by new { cds.CategoryName } into g
                //    //                       select new ProductWisePurchaseModel
                //    //                       {
                //    //                           CategoryName = g.Key.CategoryName,
                //    //                           Quantity = g.Count(),
                //    //                           TotalAmount = g.Sum(i => i.MRP),
                //    //                       }).ToList();
                //    #endregion

                //    var Normalstocks = (from ns in stockInfos
                //                        group ns by new { GodownName = ns.GodownName } into g
                //                        select new ProductWisePurchaseModel
                //                        {
                //                            GodownName = g.Key.GodownName,
                //                            Quantity = g.Sum(i => i.ParentQuantity),
                //                            TotalAmount = g.Sum(i => i.SRate * i.Quantity),
                //                            TotalCreditSR3 = g.Sum(i => i.TotalCreditSR3 * i.Quantity),
                //                            TotalCreditSR6 = g.Sum(i => i.TotalCreditSR6 * i.Quantity),
                //                            TotalCreditSR12 = g.Sum(i => i.TotalCreditSR12 * i.Quantity),
                //                        }).ToList();

                //    foreach (var item in Normalstocks)
                //    {
                //        row = dtStockInfoDT.NewRow();
                //        row["UnitType"] = item.ParentUnit;
                //        //row["StockCode"] = item.Item1;
                //        row["ProName"] = item.ProductName;
                //        row["CompanyName"] = item.CompanyName;
                //        row["CategoryName"] = item.CategoryName;
                //        row["ModelName"] = item.ProductName;
                //        row["Quantity"] = item.Quantity;
                //        row["PRate"] = 0;
                //        row["TotalPrice"] = item.TotalAmount;
                //        row["StockCode"] = item.ProductCode; ;
                //        row["SalesRate"] = 0m;
                //        row["CreditSRate"] = 0m;// 6 months
                //        row["TotalCreditPrice"] = item.TotalCreditSR6;
                //        row["StockType"] = "Normal Stock";
                //        row["CrSR3"] = 0m;
                //        row["TotalCrSR3"] = item.TotalCreditSR3;
                //        row["CrSR12"] = 0m;
                //        row["TotalCrSR12"] = item.TotalCreditSR12;
                //        row["Godown"] = item.GodownName;
                //        dtStockInfoDT.Rows.Add(row);
                //    }
                //}

                #region
                //foreach (var item in InHouseProducts)
                //{
                //    row = dtStockInfoDT.NewRow();
                //    row["UnitType"] = item.ParentUnit;
                //    //row["StockCode"] = item.Item1;
                //    row["ProName"] = item.ProductName;
                //    row["CompanyName"] = item.CompanyName;
                //    row["CategoryName"] = item.CategoryName;
                //    row["ModelName"] = item.ProductName;
                //    row["Quantity"] = item.Quantity;
                //    row["PRate"] = 0;
                //    row["TotalPrice"] = item.TotalAmount;
                //    row["StockCode"] = IMENO;
                //    row["SalesRate"] = item.SRate;
                //    row["CreditSRate"] = 0;
                //    row["TotalCreditPrice"] = 0;
                //    row["StockType"] = "In-house Damage Stock";
                //    dtStockInfoDT.Rows.Add(row);
                //}

                //foreach (var item in CompanyDamageStocks)
                //{
                //    row = dtStockInfoDT.NewRow();
                //    row["UnitType"] = "Pice";
                //    //row["StockCode"] = item.Item1;
                //    row["ProName"] = item.ProductName;
                //    row["CompanyName"] = item.CompanyName;
                //    row["CategoryName"] = item.CategoryName;
                //    row["ModelName"] = item.ProductName;
                //    row["Quantity"] = item.Quantity;
                //    row["PRate"] = 0;
                //    row["TotalPrice"] = item.TotalAmount;
                //    row["StockCode"] = IMENO;
                //    row["SalesRate"] = item.MRP;
                //    row["CreditSRate"] = 0;
                //    row["TotalCreditPrice"] = 0;
                //    row["StockType"] = "Company Damage Stock";
                //    dtStockInfoDT.Rows.Add(row);
                //}


                //foreach (var item in InHouseProducts)
                //{
                //    row = dtStockInfoDT.NewRow();
                //    row["UnitType"] = "Pice";
                //    //row["StockCode"] = item.Item1;
                //    row["ProName"] = item.ProductName;
                //    row["CompanyName"] = item.CompanyName;
                //    row["CategoryName"] = item.CategoryName;
                //    row["ModelName"] = item.ProductName;
                //    row["Quantity"] = item.Quantity;
                //    row["PRate"] = 0;
                //    row["TotalPrice"] = item.TotalAmount;
                //    row["StockCode"] = IMENO;
                //    row["SalesRate"] = item.SRate;
                //    row["CreditSRate"] = 0;
                //    row["TotalCreditPrice"] = 0;
                //    row["StockType"] = "In-house Damage Stock";
                //    dtStockInfoDT.Rows.Add(row);
                //}

                //foreach (var item in CompanyDamageStocks)
                //{
                //    row = dtStockInfoDT.NewRow();
                //    row["UnitType"] = "Pice";
                //    //row["StockCode"] = item.Item1;
                //    row["ProName"] = item.ProductName;
                //    row["CompanyName"] = item.CompanyName;
                //    row["CategoryName"] = item.CategoryName;
                //    row["ModelName"] = item.ProductName;
                //    row["Quantity"] = item.Quantity;
                //    row["PRate"] = 0;
                //    row["TotalPrice"] = item.TotalAmount;
                //    row["StockCode"] = IMENO;
                //    row["SalesRate"] = item.MRP;
                //    row["CreditSRate"] = 0;
                //    row["TotalCreditPrice"] = 0;
                //    row["StockType"] = "Company Damage Stock";
                //    dtStockInfoDT.Rows.Add(row);
                //}
                #endregion

                dtStockInfoDT.TableName = "StockInfo";
                _dataSet = new DataSet();
                _dataSet.Tables.Add(dtStockInfoDT);

                GetCommonParameters(userName, concernID);
                //if (concernID == (int)EnumSisterConcern.SAMSUNG_ELECTRA_CONCERNID || concernID == (int)EnumSisterConcern.HAWRA_ENTERPRISE_CONCERNID || concernID == (int)EnumSisterConcern.HAVEN_ENTERPRISE_CONCERNID)
                //{
                //    if (reportType == 0)
                //    {
                //        reportName = "Stock\\StockSummaryInfoForCredit.rdlc";
                //    }
                //    else if (reportType == 1)
                //    {
                //        reportName = "Stock\\rptCompanyWiseStockSummaryForCredit.rdlc";
                //    }
                //    else if (reportType == 2)
                //    {
                //        reportName = "Stock\\rptCategoryWiseStockSummaryForCredit.rdlc";
                //    }
                //}
                //else
                //{
                if (reportType == 0)
                {
                    reportName = "Stock\\StockSummaryInfo.rdlc";
                }
                else if (reportType == 1)
                {
                    reportName = "Stock\\StockSummaryInfoCompanyWise.rdlc";
                }
                else if (reportType == 2)
                {
                    reportName = "Stock\\StockSummaryInfoCategoryWise.rdlc";
                }
                else if (reportType == 3)
                {
                    reportName = "Stock\\rptGodownWiseStockSummary.rdlc";
                }

                return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, reportName);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public byte[] SRInvoiceReport(int oOrderID, string userName, int concernID)
        {
            try
            {

                SRVisit oOrder = _SRVisitService.GetSRVisitById(Convert.ToInt32(oOrderID));
                var soDetails = _SRVisitDetailService.GetSRVisitDetailById(Convert.ToInt32(oOrderID));

                DataTable orderdDT = new DataTable();
                TransactionalDataSet.dtSRVisitDataTable dt = new TransactionalDataSet.dtSRVisitDataTable();
                Employee employee = _EmployeeService.GetEmployeeById(oOrder.EmployeeID);
                Product product = null;
                Category oCategory = null;
                Company oCompany = null;
                string IMENO = "";

                int count = 0;

                foreach (var item in soDetails)
                {
                    product = _productService.GetProductById(item.Item2);

                    if (product != null)
                    {
                        oCategory = _CategoryService.GetCategoryById(product.CategoryID);
                        oCompany = _CompanyService.GetCompanyById(product.CompanyID);
                    }

                    IMENO = "";
                    count = 0;
                    IEnumerable<SRVProductDetail> SRVPD = _SRVProductDetailService.GetSRVProductDetailsById(item.Item1, item.Item2);

                    foreach (SRVProductDetail itemime in SRVPD)
                    {
                        if (count == 0)
                            IMENO = IMENO + itemime.IMENO;
                        else
                            IMENO = IMENO + System.Environment.NewLine + itemime.IMENO;
                        count++;
                    }

                    dt.Rows.Add(item.Item5, oCategory.Description, oCompany.Name, SRVPD.Count(), IMENO);
                }
                orderdDT = dt.AsEnumerable().OrderBy(o => (String)o["ProductName"]).CopyToDataTable();
                dt.TableName = "dtSRVisit";
                _dataSet = new DataSet();
                _dataSet.Tables.Add(dt);


                GetCommonParameters(userName, concernID);

                _reportParameter = new ReportParameter("InvoiceNo", oOrder.ChallanNo);
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("InvoiceDate", oOrder.VisitDate.ToString());
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("Name", employee.Name);
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("MobileNo", employee.ContactNo);
                _reportParameters.Add(_reportParameter);

                return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Sales\\rptSRVisit.rdlc");
            }

            catch (Exception Ex)
            {

                throw Ex;
            }
        }

        public byte[] SRInvoiceReportByChallanNo(string ChallanNo, string userName, int concernID)
        {
            try
            {
                SRVisit oOrder = _SRVisitService.GetSRVisitByChallanNo(ChallanNo, concernID);
                var soDetails = _SRVisitDetailService.GetSRVisitDetailById(Convert.ToInt32(oOrder.SRVisitID));

                DataTable orderdDT = new DataTable();
                TransactionalDataSet.dtSRVisitDataTable dt = new TransactionalDataSet.dtSRVisitDataTable();
                Employee employee = _EmployeeService.GetEmployeeById(oOrder.EmployeeID);
                Product product = null;
                Category oCategory = null;
                Company oCompany = null;

                string IMENO = "";
                int count = 0;

                foreach (var item in soDetails)
                {
                    product = _productService.GetProductById(item.Item2);

                    if (product != null)
                    {
                        oCategory = _CategoryService.GetCategoryById(product.CategoryID);
                        oCompany = _CompanyService.GetCompanyById(product.CompanyID);
                    }

                    IMENO = "";
                    count = 0;
                    IEnumerable<SRVProductDetail> SRVPD = _SRVProductDetailService.GetSRVProductDetailsById(item.Item1, item.Item2);

                    foreach (SRVProductDetail itemime in SRVPD)
                    {
                        if (count == 0)
                            IMENO = IMENO + itemime.IMENO;
                        else
                            IMENO = IMENO + System.Environment.NewLine + itemime.IMENO;
                        count++;
                    }


                    ////product = _productService.GetProductById(item.);
                    //IMENO = "";
                    //var SRVPD = _SRVProductDetailService.GetSRVProductDetailsById(item.Item1, item.Item2);

                    //foreach (var itemime in SRVPD)
                    //{
                    //    IMENO = IMENO + System.Environment.NewLine + itemime.IMENO;
                    //}

                    dt.Rows.Add(item.Item5, oCategory.Description, oCompany.Name, item.Item4, IMENO);
                }
                orderdDT = dt.AsEnumerable().OrderBy(o => (String)o["ProductName"]).CopyToDataTable();
                dt.TableName = "dtSRVisit";
                _dataSet = new DataSet();
                _dataSet.Tables.Add(dt);


                GetCommonParameters(userName, concernID);

                _reportParameter = new ReportParameter("InvoiceNo", oOrder.ChallanNo);
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("InvoiceDate", oOrder.VisitDate.ToString());
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("Name", employee.Name);
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("MobileNo", employee.ContactNo);
                _reportParameters.Add(_reportParameter);

                return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Sales\\rptSRVisit.rdlc");
            }

            catch (Exception Ex)
            {

                throw Ex;
            }
        }
        public string GetClientDateTime()
        {
            DateTime utcTime = DateTime.UtcNow;
            TimeZoneInfo BdZone = TimeZoneInfo.FindSystemTimeZoneById("Bangladesh Standard Time");
            DateTime localDateTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, BdZone);
            return localDateTime.ToString("dd MMM yyyy HH:mm:ss");
        }

        private void GetCommonParameters(string userName, int concernID)
        {
            string logoPath = string.Empty;
            SystemInformation currentSystemInfo = _systemInformationService.GetSystemInformationByConcernId(concernID);
            _reportParameters = new List<ReportParameter>();

            _reportParameter = new ReportParameter("Logo", logoPath);
            _reportParameters.Add(_reportParameter);
            string CompanyName = string.Empty;
            if (currentSystemInfo.Name.Contains("\r"))
            {
                var Sysinfo = currentSystemInfo.Name.Split(new char[] { '\r' }, StringSplitOptions.RemoveEmptyEntries);
                CompanyName = Sysinfo[0] + Environment.NewLine + Sysinfo[1];
            }
            else
                CompanyName = currentSystemInfo.Name;

            #region Logo
            if (currentSystemInfo.CompanyLogo != null)
            {
                TransactionalDataSet.dtLogoDataTable dtLogo = new TransactionalDataSet.dtLogoDataTable();
                dtLogo.Rows.Add(currentSystemInfo.CompanyLogo);
                _dataSet.Tables.Add(dtLogo);
            }
            #endregion

            _reportParameter = new ReportParameter("CompanyName", CompanyName);
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("Phone", currentSystemInfo.TelephoneNo);
            _reportParameters.Add(_reportParameter);


            _reportParameter = new ReportParameter("Address", currentSystemInfo.Address);
            _reportParameters.Add(_reportParameter);
            _reportParameter = new ReportParameter("PrintedBy", userName);
            _reportParameters.Add(_reportParameter);
        }
        public static string GetUserFriendlyDescription(int SalaryItemCode, string Description)
        {
            switch (SalaryItemCode)
            {
                case (int)EnumSalaryItemCode.Tot_Attend_Days_Amount:
                    return "Attendenc Salary";
                case (int)EnumSalaryItemCode.Over_Time_Amount:
                    return "Over Time";
                case (int)EnumSalaryItemCode.Tot_Attend_Days_Bonus:
                    return "Attendence Bonus";
                case (int)EnumSalaryItemCode.Bonus:
                    return "Bonus";
                case (int)EnumSalaryItemCode.Advance_Deduction:
                    return "Advance";
                case (int)EnumSalaryItemCode.Target_Failed_Deduct:
                    return "Target Failed Deduction";
                case (int)EnumSalaryItemCode.Gross_Salary:
                    return "Gross Salary";
                case (int)EnumSalaryItemCode.Extra_Commission:
                    return "Extra Commission";
                case (int)EnumSalaryItemCode.Voltage_StabilizerComm:
                    return "Voltage Stabilizer Commission";
                default:
                    return Description;
            }
        }
        public byte[] DefaultingCustomerReport(DateTime date, string userName, int concernID)
        {
            try
            {
                _dataSet = new DataSet();
                IEnumerable<Tuple<string, string, string, decimal, decimal>> defaultingCustomers = _creditSalesOrderService.GetDefaultingCustomer(date, concernID);

                TransactionalDataSet.dtDefaultingCustomerDataTable dt = new TransactionalDataSet.dtDefaultingCustomerDataTable();


                foreach (var item in defaultingCustomers)
                {

                    dt.Rows.Add(item.Item1, item.Item2, item.Item3, item.Item4, item.Item5);

                }
                dt.TableName = "dtDefaultingCustomer";
                _dataSet.Tables.Add(dt);
                GetCommonParameters(userName, concernID);
                List<ReportParameter> parameters = new List<ReportParameter>();
                _reportParameter = new ReportParameter("Date", " till " + date.ToString("dd MMM yyyy"));
                _reportParameters.Add(_reportParameter);
                return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "CreditSales\\rptDefaultingCustomer.rdlc");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Date: 05-Jun-2018
        /// New Method
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="userName"></param>
        /// <param name="concernID"></param>
        /// <returns></returns>
        public byte[] DefaultingCustomerReport(DateTime fromDate, DateTime toDate, string userName, int concernID)
        {
            try
            {
                _dataSet = new DataSet();
                IEnumerable<Tuple<string, string, string, string, DateTime, DateTime, decimal, Tuple<decimal, decimal, decimal, decimal, string, decimal, decimal, Tuple<int, decimal>>>> upComing = _creditSalesOrderService.GetDefaultingCustomer(fromDate, toDate, concernID);

                TransactionalDataSet.dtUpcomingScheduleDataTable dt = new TransactionalDataSet.dtUpcomingScheduleDataTable();
                foreach (var item in upComing)
                {
                    dt.Rows.Add(item.Item2, item.Item3, item.Item4, "", item.Item5, item.Item6, item.Item7, item.Rest.Item1 + item.Rest.Item7, item.Rest.Rest.Item2, item.Rest.Item3, 0m, item.Rest.Item5, item.Rest.Item6, item.Rest.Item7, item.Rest.Item4, item.Rest.Rest.Item2 - item.Rest.Item6);
                }

                dt.TableName = "dtUpcomingSchedule";
                _dataSet.Tables.Add(dt);

                GetCommonParameters(userName, concernID);
                _reportParameter = new ReportParameter("DateRange", "Default Customer Report Sales Date From: " + fromDate.ToString("dd MMM yyyy") + " To " + toDate.ToString("dd MMM yyyy"));
                _reportParameters.Add(_reportParameter);

                return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "CreditSales\\rptDefaultingCustomerNew.rdlc");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public byte[] InstallmentCollectionReport(DateTime fromDate, DateTime toDate, string userName, int concernID)
        {
            try
            {
                _dataSet = new DataSet();
                var InstallmentCollections = _creditSalesOrderService.GetScheduleCollection(fromDate, toDate, concernID);
                TransactionalDataSet.dtUpcomingScheduleDataTable dt = new TransactionalDataSet.dtUpcomingScheduleDataTable();
                DataRow row = null;
                foreach (var item in InstallmentCollections)
                {
                    row = dt.NewRow();
                    row["InvoiceNo"] = item.InvoiceNo;
                    row["CustomerName"] = item.CustomerName;
                    row["ContactNo"] = item.CustomerName + " & " + item.CustomerConctact + " & " + item.CustomerAddress;
                    row["ProductName"] = item.ProductName;
                    row["SalesDate"] = item.SalesDate;
                    row["PaymentDate"] = item.PaymentDate;
                    row["SalesPrice"] = item.SalesPrice;
                    row["NetAmt"] = item.NetAmount + item.PenaltyInterest;
                    row["TotalAmt"] = item.NetAmount;
                    row["RemainingAmt"] = item.Remaining;
                    row["Installment"] = item.InstallmentAmount;
                    row["Remarks"] = item.Remarks;
                    row["DownPayment"] = item.DownPayment;
                    row["InterestAmount"] = item.PenaltyInterest;
                    row["DefaultAmount"] = 0m;
                    row["TotalInstCollectionAmt"] = 0m;
                    row["InstallmentPeriod"] = item.InstallmentPeriod;
                    dt.Rows.Add(row);
                }

                dt.TableName = "dtUpcomingSchedule";
                _dataSet.Tables.Add(dt);

                GetCommonParameters(userName, concernID);

                _reportParameter = new ReportParameter("PaymentDate", "Installment Collections Date from " + fromDate.ToString("dd MMM yyyy") + " To " + toDate.ToString("dd MMM yyyy"));
                _reportParameters.Add(_reportParameter);

                return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "CreditSales\\rptInstallmentCollections.rdlc");

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //public byte[] UpComingScheduleReport(DateTime fromDate, DateTime toDate, string userName, int concernID)
        //{
        //    try
        //    {
        //        _dataSet = new DataSet();
        //        var upComing = _creditSalesOrderService.GetUpcomingSchedule(fromDate, toDate);

        //        TransactionalDataSet.dtUpcomingScheduleDataTable dt = new TransactionalDataSet.dtUpcomingScheduleDataTable();
        //        decimal DefaultAmount = 0;
        //        foreach (var item in upComing)
        //        {

        //            //dt.Rows.Add(oCSDItem.InvoiceNo, oCSDItem.Name, oCSDItem.ContactNo, "", oCSDItem.SalesDate, oCSDItem.MonthDate, oCSDItem.TSalesAmt, oCSDItem.NetAmount, oCSDItem.FixedAmt, oCSDItem.Remaining, oCSDItem.InstallmentAmt, oCSDItem.Remarks, oCSDItem.DownPayment);
        //            DefaultAmount = _creditSalesOrderService.GetDefaultAmount(item.Rest.Rest.Item1, fromDate);
        //            dt.Rows.Add(item.Item2, item.Item3, item.Item4, "", item.Item5, item.Item6, item.Item7, item.Rest.Item1 + item.Rest.Item7, item.Rest.Item2, item.Rest.Item3, item.Rest.Item4 + DefaultAmount, item.Rest.Item5, item.Rest.Item6, item.Rest.Item7, DefaultAmount);
        //        }

        //        dt.TableName = "dtUpcomingSchedule";
        //        _dataSet.Tables.Add(dt);

        //        GetCommonParameters(userName, concernID);
        //        _reportParameter = new ReportParameter("PaymentDate", fromDate.ToString("dd MMM yyyy"));
        //        _reportParameters.Add(_reportParameter);
        //        _reportParameter = new ReportParameter("ToDate", toDate.ToString("dd MMM yyyy"));
        //        _reportParameters.Add(_reportParameter);

        //        return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "CreditSales\\UpComingSchedule.rdlc");
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}


        public byte[] UpComingScheduleReport(DateTime fromDate, DateTime toDate, string userName, int concernID)
        {
            try
            {
                _dataSet = new DataSet();
                var upComing = _creditSalesOrderService.GetUpcomingSchedule(fromDate, toDate);

                TransactionalDataSet.dtUpcomingScheduleDataTable dt = new TransactionalDataSet.dtUpcomingScheduleDataTable();
                decimal DefaultAmount = 0;
                DataRow row = null;
                string ProductName = string.Empty;
                foreach (var item in upComing)
                {
                    row = dt.NewRow();
                    DefaultAmount = _creditSalesOrderService.GetDefaultAmount(item.CreditSalesID, fromDate);
                    row["InvoiceNo"] = item.InvoiceNo;
                    row["CustomerName"] = item.CustomerName;
                    row["ContactNo"] = item.CustomerCode + " & " + item.CustomerName + " & " + item.CustomerAddress + " & " + item.CustomerConctact;
                    for (int i = 0; i < item.ProductName.Count; i++)
                    {
                        ProductName = ProductName + item.ProductName[i];
                        if (item.ProductName.Count != i + 1)
                            ProductName = ProductName + System.Environment.NewLine;
                    }
                    row["ProductName"] = ProductName;
                    row["SalesDate"] = item.SalesDate;
                    row["PaymentDate"] = item.PaymentDate;
                    row["SalesPrice"] = item.SalesPrice;
                    row["NetAmt"] = item.NetAmount + item.PenaltyInterest;
                    row["TotalAmt"] = item.FixedAmt;
                    row["RemainingAmt"] = item.Remaining;
                    row["Installment"] = item.InstallmentAmount + DefaultAmount;
                    row["Remarks"] = item.Remarks;
                    row["DownPayment"] = item.DownPayment;
                    row["InterestAmount"] = item.PenaltyInterest;
                    row["DefaultAmount"] = DefaultAmount;
                    row["InstallmentPeriod"] = item.InstallmentPeriod;
                    row["TotalInstCollectionAmt"] = 0;
                    row["CustomerCode"] = item.CustomerCode;
                    row["TotalPaidAmt"] = item.NetAmount - item.Remaining;
                    row["RefNameContact"] = item.CustomerRefName + " & " + item.CustomerRefContact;
                    row["RemaindDate"] = item.RemaindDate == null ? "" : item.RemaindDate.Value.ToString("dd MMM yyyy");
                    row["NoOfInstallment"] = item.NoOfInstallment;
                    dt.Rows.Add(row);
                    ProductName = string.Empty;
                }

                dt.TableName = "dtUpcomingSchedule";
                _dataSet.Tables.Add(dt);

                GetCommonParameters(userName, concernID);
                _reportParameter = new ReportParameter("PaymentDate", fromDate.ToString("dd MMM yyyy"));
                _reportParameters.Add(_reportParameter);
                _reportParameter = new ReportParameter("ToDate", toDate.ToString("dd MMM yyyy"));
                _reportParameters.Add(_reportParameter);

                return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "CreditSales\\UpComingSchedule.rdlc");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public byte[] CashCollectionReport(DateTime fromDate, DateTime toDate, string userName, int concernID, int customerId, int reportType)
        {
            try
            {

                TransactionalDataSet.dtCollectionRptDataTable dt = new TransactionalDataSet.dtCollectionRptDataTable();
                _dataSet = new DataSet();
                List<SOredersReportModel> SalesOrders = new List<SOredersReportModel>();

                if (reportType == 1)//Cash Sales
                {
                    SalesOrders = _salesOrderService.GetforSalesReport(fromDate, toDate, 0, customerId).ToList();


                }
                else if (reportType == 2)//Cash+bank collection
                {
                    var CashCollectionInfos = _CashCollectionService.GetCashCollectionData(fromDate, toDate, concernID, customerId, reportType);
                    foreach (var grd in CashCollectionInfos)
                    {
                        dt.Rows.Add(grd.Item1.ToString("dd MMM yyyy"), grd.Item2, grd.Item4 + " & " + grd.Item3, grd.Item4, grd.Item5, grd.Item6, grd.Item7, grd.Rest.Item1, grd.Rest.Item2, grd.Rest.Item3, grd.Rest.Item4, grd.Rest.Item5, grd.Rest.Item6);
                    }
                    var BankCollectionInfos = _bankTransactionService.GetBankTransactionData(fromDate, toDate, concernID, customerId, 0, 0).Where(i => !string.IsNullOrEmpty(i.Item3));
                    foreach (var grd in BankCollectionInfos)
                    {
                        dt.Rows.Add(
                            grd.Rest.Item2.Value.ToString("dd MMM yyyy"), //Date
                            grd.Item3,  //CName
                            grd.Rest.Item4, //Address
                            grd.Rest.Item5, //ContactNo
                            0,     //TotalDue
                            grd.Rest.Item1,   //RecAmount

                            0,
                            0,
                            "Cheque",
                            grd.Item2,
                            grd.Rest.Rest.Item2,
                            grd.Rest.Rest.Item1,
                            grd.Rest.Rest.Item3,
                            grd.Rest.Rest.Item2,
                             grd.Item6);

                    }
                }
                else // All
                {
                    SalesOrders = _salesOrderService.GetforSalesReport(fromDate, toDate, 0, customerId).ToList();
                    var CashCollectionInfos = _CashCollectionService.GetCashCollectionData(fromDate, toDate, concernID, customerId, reportType);
                    foreach (var grd in CashCollectionInfos)
                    {
                        dt.Rows.Add(grd.Item1.ToString("dd MMM yyyy"), grd.Item2, grd.Item4 + " & " + grd.Item3, grd.Item4, grd.Item5, grd.Item6, grd.Item7, grd.Rest.Item1, grd.Rest.Item2, grd.Rest.Item3, grd.Rest.Item4, grd.Rest.Item5, grd.Rest.Item6);
                    }
                    var BankCollectionInfos = _bankTransactionService.GetBankTransactionData(fromDate, toDate, concernID, customerId, 0, 0).Where(i => !string.IsNullOrEmpty(i.Item3));
                    foreach (var grd in BankCollectionInfos)
                    {
                        dt.Rows.Add(
                            grd.Rest.Item2.Value.ToString("dd MMM yyyy"), //Date
                            grd.Item3,  //CName
                            grd.Rest.Item4, //Address
                            grd.Rest.Item5, //ContactNo
                            0,     //TotalDue
                            grd.Rest.Item1,   //RecAmount

                            0,
                            0,
                            "Cheque",
                            grd.Item2,
                            grd.Rest.Rest.Item2,
                            grd.Rest.Rest.Item1,
                            grd.Rest.Rest.Item3,
                            grd.Rest.Rest.Item2,
                             grd.Item6);

                    }
                }


                foreach (var item in SalesOrders)
                {
                    dt.Rows.Add(item.InvoiceDate.ToString("dd MMM yyyy"), item.CustomerName, item.CustomerAddress, item.CustomerContactNo, item.CustomerTotalDue, item.RecAmount, 0m, 0m, "Cash Sales", "", "", "", "", item.EmployeeName, item.InvoiceNo);
                }





                //if (concernID == (int)EnumSisterConcern.SAMSUNG_ELECTRA_CONCERNID || concernID == (int)EnumSisterConcern.HAWRA_ENTERPRISE_CONCERNID || concernID == (int)EnumSisterConcern.HAVEN_ENTERPRISE_CONCERNID)
                //{

                //    IEnumerable<Tuple<string, string, DateTime, string, decimal, decimal, decimal, Tuple<decimal, decimal, decimal, decimal, decimal, int, string>>> CreditSales = null;
                //    CreditSales = _creditSalesOrderService.GetCreditSalesReportByConcernID(fromDate, toDate, concernID, 0);
                //    if (customerId != 0)
                //        CreditSales = CreditSales.Where(i => i.Rest.Item6 == customerId);

                //    foreach (var grd in CreditSales)
                //    {
                //        dt.Rows.Add(grd.Item3.ToString("dd MMM yyyy"), grd.Item2, "Address", "Contact", grd.Rest.Item5, grd.Rest.Item1, 0m, 0m, "Credit Sales", "", "", "", "", "Emp", grd.Item4);
                //    }

                //    IEnumerable<Tuple<string, string, string, string, DateTime, DateTime, decimal, Tuple<decimal, decimal, decimal, decimal, string, decimal>>> CreditCollections = _creditSalesOrderService.GetCreditCollectionReport(fromDate, toDate, concernID, customerId);
                //    foreach (var grd in CreditCollections)
                //    {
                //        dt.Rows.Add(grd.Item6.ToString("dd MMM yyyy"), grd.Item3, "", grd.Item4, grd.Rest.Item3, grd.Rest.Item4, grd.Rest.Item3, 0m, "CreditCollection", "N/A", "N/A", "N/A", "N/A", "EMP Name");
                //    }
                //}

                dt.TableName = "dtCollectionRpt";
                _dataSet.Tables.Add(dt);

                GetCommonParameters(userName, concernID);

                _reportParameter = new ReportParameter("Month", "Cash Collection report for the date of : " + fromDate.ToString("dd MMM yyyy") + " to " + toDate.ToString("dd MMM yyyy"));
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("PrintDate", GetClientDateTime());
                _reportParameters.Add(_reportParameter);

                return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Others\\rptCollectionRpt.rdlc");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public byte[] CashDeliverReport(DateTime fromDate, DateTime toDate, string userName, int concernID, int supplierId, int reportType)
        {
            try
            {
                var CashCollectionInfos = _CashCollectionService.GetCashDeliveryData(fromDate, toDate, concernID, supplierId, reportType);
                var BankCollectionInfos = _bankTransactionService.GetBankTransactionData(fromDate, toDate, concernID, 0, supplierId, reportType);

                TransactionalDataSet.dtCollectionRptDataTable dt = new TransactionalDataSet.dtCollectionRptDataTable();
                _dataSet = new DataSet();

                foreach (var grd in CashCollectionInfos)
                {
                    dt.Rows.Add(grd.Item1.ToString("dd MMM yyyy"), grd.Item2, grd.Item3, grd.Item4, grd.Item5, grd.Item6, grd.Item7, grd.Rest.Item1, grd.Rest.Item2, grd.Rest.Item3, grd.Rest.Item4, grd.Rest.Item5, grd.Rest.Item6);
                }

                foreach (var grd in BankCollectionInfos)
                {
                    dt.Rows.Add(
                        grd.Rest.Item2.Value.ToString("dd MMM yyyy"), //Date
                        grd.Item4,  //SupplierName
                        grd.Rest.Item6, //Address
                        grd.Rest.Item7, //ContactNo
                        0,     //TotalDue
                        grd.Rest.Item1,   //RecAmount

                        0,
                        0,
                        "Cheque",
                        grd.Item2,
                        grd.Rest.Rest.Item2,
                        grd.Rest.Rest.Item1,
                        grd.Rest.Rest.Item3,
                        grd.Rest.Rest.Item2,
                        grd.Item6);

                }

                dt.TableName = "dtCollectionRpt";
                _dataSet.Tables.Add(dt);

                GetCommonParameters(userName, concernID);
                _reportParameter = new ReportParameter("Month", "Cash Delivery report for the date of : " + fromDate.ToString("dd MMM yyyy") + " to " + toDate.ToString("dd MMM yyyy"));
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("PrintDate", GetClientDateTime());
                _reportParameters.Add(_reportParameter);

                return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Others\\rptCollectionRpt.rdlc");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public byte[] MOWiseSDetailReport(DateTime fromDate, DateTime toDate, string userName, int concernID, int MOID)
        {
            try
            {
                var MOWiseSalesInfos = _salesOrderService.GetforSalesDetailReportByMO(fromDate, toDate, MOID);

                TransactionalDataSet.dtMOWSDetailsDataTable dt = new TransactionalDataSet.dtMOWSDetailsDataTable();
                _dataSet = new DataSet();
                string EmpName = "";
                int SORderID = 0;
                decimal TotalAmount = 0, GrandTotal = 0, totalDueAmount = 0, totalCashSales = 0, NetDiscount = 0;
                foreach (var grd in MOWiseSalesInfos)
                {

                    EmpName = grd.EmployeeName;

                    if (SORderID != grd.SOrderID)
                    {
                        TotalAmount = TotalAmount + grd.TotalAmount;
                        totalCashSales = totalCashSales + grd.RecAmount;
                        totalDueAmount = totalDueAmount + grd.PaymentDue;
                        GrandTotal = GrandTotal + grd.Grandtotal;
                        NetDiscount = NetDiscount + grd.NetDiscount;
                    }
                    dt.Rows.Add(grd.EmployeeName, grd.InvoiceDate, grd.InvoiceNo, grd.ProductName, grd.CustomerName, grd.UnitPrice, grd.RecAmount, grd.PaymentDue, grd.TotalAmount, grd.Grandtotal, grd.NetDiscount, grd.TotalAmount, grd.RecAmount, grd.PaymentDue, grd.Quantity, grd.IMENO, grd.CustomerCode, grd.AdjAmount, grd.Trems);
                    SORderID = grd.SOrderID;
                }

                dt.TableName = "dtMOWSDetails";
                _dataSet.Tables.Add(dt);

                GetCommonParameters(userName, concernID);

                _reportParameter = new ReportParameter("Date", "" + fromDate.ToString("dd MMM yyyy") + " to " + toDate.ToString("dd MMM yyyy"));
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("EmpName", "Sales Representative:[" + EmpName + "]");
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("GrandTotal", GrandTotal.ToString());
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("NetTotal", TotalAmount.ToString());
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("totalCashSales", totalCashSales.ToString());
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("totalDueAmount", totalDueAmount.ToString());
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("NetDiscount", NetDiscount.ToString());
                _reportParameters.Add(_reportParameter);

                return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Sales\\rptMOWiseSDetails.rdlc");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public byte[] ProductWisePriceProtection(DateTime fromDate, DateTime toDate, string userName, int concernID)
        {
            try
            {
                IEnumerable<Tuple<string, string, decimal, decimal, decimal, decimal, DateTime>> PWPriceProtection = _StockServce.GetPriceProtectionReport(userName, concernID, fromDate, toDate);

                TransactionalDataSet.dtPProtectionDataTable dt = new TransactionalDataSet.dtPProtectionDataTable();
                _dataSet = new DataSet();
                //string SuppName = "";

                foreach (var grd in PWPriceProtection)
                {
                    //SuppName = grd.Item6;
                    if (grd.Item5 > 0)
                        dt.Rows.Add(grd.Item1, grd.Item7, grd.Item2, grd.Item3, grd.Item4, grd.Item5, grd.Item6);
                }

                dt.TableName = "dtPProtection";
                _dataSet.Tables.Add(dt);

                GetCommonParameters(userName, concernID);

                _reportParameter = new ReportParameter("DateRange", "Price Protection Data From " + fromDate.ToString("dd MMM yyyy") + " to " + toDate.ToString("dd MMM yyyy"));
                _reportParameters.Add(_reportParameter);

                return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Stock\\rptPProtection.rdlc");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public byte[] ProductWisePandSReport(DateTime fromDate, DateTime toDate, string userName, int concernID, int productID)
        {
            try
            {
                IEnumerable<Tuple<DateTime, string, string, decimal, decimal>> purchase = _purchaseOrderService.GetPurchaseByProductID(fromDate, toDate, concernID, productID);
                IEnumerable<Tuple<DateTime, string, string, decimal, decimal>> sales = _salesOrderService.GetSalesByProductID(fromDate, toDate, concernID, productID);

                TransactionalDataSet.PWPDetailsDataTable pwp = new TransactionalDataSet.PWPDetailsDataTable();
                TransactionalDataSet.PWSDetailsDataTable pws = new TransactionalDataSet.PWSDetailsDataTable();
                _dataSet = new DataSet();
                decimal TotalPurchase = 0;
                decimal TotalSales = 0;
                decimal StockIn = 0;
                foreach (var grd in purchase)
                {
                    TotalPurchase = TotalPurchase + grd.Item4;
                    pwp.Rows.Add(grd.Item1, grd.Item2, "", "", grd.Item3, grd.Item4, grd.Item5, 0);
                }
                foreach (var grd in sales)
                {
                    TotalSales = TotalSales + grd.Item4;
                    pws.Rows.Add(grd.Item1, grd.Item2, "", "", grd.Item3, grd.Item4, grd.Item5, 0);
                }

                StockIn = TotalPurchase - TotalSales;
                pwp.TableName = "PWPDetails";
                _dataSet.Tables.Add(pws);
                pws.TableName = "PWSDetails";
                _dataSet.Tables.Add(pwp);

                GetCommonParameters(userName, concernID);

                _reportParameter = new ReportParameter("DateRange", "Date from: " + fromDate.ToString("dd MMM yyyy") + " to " + toDate.ToString("dd MMM yyyy"));
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("OutStandingStock", Math.Round(StockIn, 0).ToString());
                _reportParameters.Add(_reportParameter);


                return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Purchase\\rptProductWPandS.rdlc");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public byte[] SRVisitStatusReport(DateTime fromDate, DateTime toDate, string userName, int concernID, int MOID)
        {
            try
            {
                //IEnumerable<SRVisitStatusReportModel> MOWiseSalesInfos= _SRVisitService.SRVisitStatusReport(fromDate, toDate, concernID, MOID);

                IEnumerable<Tuple<DateTime, string, string, decimal, string, string, string, Tuple<string>>> MOWiseSalesInfos = _SRVisitService.GetSRViistDetailReportByEmployeeID(fromDate, toDate, concernID, MOID);
                TransactionalDataSet.dtSRVisitStatusDataTable dt = new TransactionalDataSet.dtSRVisitStatusDataTable();
                _dataSet = new DataSet();
                string EmpName = "";
                string IMENO = "";
                string VChallanNo = "";
                string ContactNo = "";
                string InvoiceNo = "";
                int count = 0;
                var gdata = from d in MOWiseSalesInfos
                            group d by d.Item2;
                foreach (var grd in MOWiseSalesInfos)
                {
                    EmpName = grd.Item6;
                    ContactNo = grd.Rest.Item1;
                    InvoiceNo = grd.Item2;

                    //count = 0;

                    if (VChallanNo == grd.Item2 || VChallanNo == "")
                    {
                        if (count == 0)
                            IMENO = IMENO + grd.Item5;
                        else
                            IMENO = IMENO + System.Environment.NewLine + grd.Item5;
                        count++;
                    }

                    //IEnumerable<SRVProductDetail> SRVPD = _SRVProductDetailService.GetSRVProductDetailsById(item.Item1, item.Item2);
                    //foreach (SRVProductDetail itemime in SRVPD)
                    //{
                    //    if (count == 0)
                    //        IMENO = IMENO + itemime.IMENO;
                    //    else
                    //        IMENO = IMENO + System.Environment.NewLine + itemime.IMENO;
                    //    count++;
                    //}


                    if (VChallanNo != grd.Item2)
                    {

                        dt.Rows.Add(grd.Item1, grd.Item3, "", "", grd.Item4, IMENO, "0", "0");
                        IMENO = "";
                        count = 0;
                    }
                    VChallanNo = grd.Item2;

                }

                dt.TableName = "dtSRVisitStatus";
                _dataSet.Tables.Add(dt);

                GetCommonParameters(userName, concernID);

                _reportParameter = new ReportParameter("InvoiceDate", "" + fromDate.ToString("dd MMM yyyy") + " to " + toDate.ToString("dd MMM yyyy"));
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("InvoiceNo", InvoiceNo);
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("Name", "Sales Representative:[" + EmpName + "]");
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("MobileNo", ContactNo);
                _reportParameters.Add(_reportParameter);


                return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Sales\\rptSRVisitStatus.rdlc");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public byte[] SRWiseCustomerSalesSummary(DateTime fromDate, DateTime toDate, string userName, int concernID, int EmployeeID)
        {

            var SRwiseCustomerSalesList = _salesOrderService.SRWiseCustomerSalesSummary(fromDate, toDate, concernID, EmployeeID);
            TransactionalDataSet.dtSRWiseCustSalesSummaryDataTable dt = new TransactionalDataSet.dtSRWiseCustSalesSummaryDataTable();
            _dataSet = new DataSet();
            foreach (var item in SRwiseCustomerSalesList)
            {
                dt.Rows.Add(item.EmployeeID, item.SRName, item.ConcernID, item.CustomerID, item.Code, item.CustomerName, item.Address, item.ContactNo, item.BarUnitPrice, item.SmartUnitPrice, item.BarQuantity, item.SmartQuantity, item.TotalPriceBar, item.TotalPriceSmart, item.BarAndSmartQty, item.BarAndSmartPrice);
            }

            dt.TableName = "dtSRWiseCustSalesSummary";
            _dataSet.Tables.Add(dt);

            GetCommonParameters(userName, concernID);

            _reportParameter = new ReportParameter("DateRange", "Date from: " + fromDate.ToString("dd MMM yyyy") + " to " + toDate.ToString("dd MMM yyyy"));
            _reportParameters.Add(_reportParameter);

            return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Sales\\rptSRWiseCustSalesSummary.rdlc");

        }
        private string GetRemarksByTransID(string TransID)
        {
            string Remarks = string.Empty;
            var ID = TransID.Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
            if (ID.Length == 2)
            {
                int Transid = Convert.ToInt32(ID[1]);
                switch (ID[0])
                {
                    case "SO":
                        var SORder = _salesOrderService.GetAllIQueryable().FirstOrDefault(i => i.SOrderID == Transid);
                        Remarks = SORder != null ? SORder.Remarks : "";
                        break;
                    case "CRO":
                        var CRS = _creditSalesOrderService.GetAllIQueryable().FirstOrDefault(i => i.CreditSalesID == Transid);
                        Remarks = CRS != null ? CRS.Remarks : "";
                        break;
                    case "CC":
                        var CC = _CashCollectionService.GetCashCollectionById(Transid);
                        Remarks = CC != null ? CC.Remarks : "";
                        break;
                    default:
                        break;
                }
            }
            return Remarks;
        }

        private List<CustomerLedgerModel> NonTransCustomers(DateTime fromDate, DateTime toDate)
        {
            var SOrders = _salesOrderService.GetAllIQueryable().Where(i => i.InvoiceDate >= fromDate && i.InvoiceDate <= toDate);
            var CashCollections = _CashCollectionService.GetAllIQueryable().Where(i => i.EntryDate >= fromDate && i.EntryDate <= toDate);
            var CreditSales = _creditSalesOrderService.GetAllIQueryable().Where(i => i.SalesDate >= fromDate && i.SalesDate <= toDate);
            var Customers = _customerService.GetAllCustomer();
            var NonTransCustomers = Customers.Where(i => !SOrders.Select(j => j.CustomerID).Contains(i.CustomerID) && !CashCollections.Select(j => j.CustomerID).Contains(i.CustomerID) && !CreditSales.Select(j => j.CustomerID).Contains(i.CustomerID));
            return (from c in NonTransCustomers
                    where c.TotalDue != 0
                    select new CustomerLedgerModel
                    {
                        ConcernID = c.ConcernID,
                        CustomerID = c.CustomerID,
                        Code = c.Code,
                        CustomerName = c.Name,
                        InvoiceDate = toDate,
                        InvoiceNo = c.Code,
                        Opening = c.TotalDue,
                        TotalDue = c.TotalDue,
                        Closing = c.TotalDue
                    }).ToList();


        }
        public byte[] CustomerLedgerSummary(DateTime fromDate, DateTime toDate, string userName, int concernID, int CustomerID)
        {
            var customerLedgerdata = _salesOrderService.CustomerLedger(fromDate, toDate, concernID, CustomerID);
            TransactionalDataSet.dtCustomerLedgerDataTable dt = new TransactionalDataSet.dtCustomerLedgerDataTable();
            _dataSet = new DataSet();
            string Remarks = string.Empty;
            List<CustomerLedgerModel> Ledgers = new List<CustomerLedgerModel>();
            Ledgers = (from cl in customerLedgerdata
                       group cl by new { cl.ConcernID, cl.CustomerID, cl.Code, cl.CustomerName, cl.SOrderID, cl.InvoiceDate, cl.InvoiceNo, cl.Opening, cl.TotalSalesAmt, cl.CollectionAmt, cl.CashSales, cl.DueSales, cl.TotalDue, cl.AdjustAmt, cl.ProductReturnAmt, cl.PenaltyInterestAmt, cl.Closing } into g
                       select new CustomerLedgerModel
                       {
                           ConcernID = g.Key.ConcernID,
                           CustomerID = g.Key.CustomerID,
                           Code = g.Key.Code,
                           CustomerName = g.Key.CustomerName,
                           SOrderID = g.Key.SOrderID,
                           InvoiceDate = g.Key.InvoiceDate,
                           InvoiceNo = g.Key.InvoiceNo,
                           Opening = g.Key.Opening,
                           TotalSalesAmt = g.Key.TotalSalesAmt,
                           CollectionAmt = g.Key.CollectionAmt,
                           CashSales = g.Key.CashSales,
                           DueSales = g.Key.DueSales,
                           TotalDue = g.Key.TotalDue,
                           AdjustAmt = g.Key.AdjustAmt,
                           PenaltyInterestAmt = g.Key.PenaltyInterestAmt,
                           ProductReturnAmt = g.Key.ProductReturnAmt,
                           Closing = g.Key.Closing
                       }).ToList();
            decimal AllCustomerTotalDue = 0m, Closing = 0m;

            if (CustomerID > 0)
            {

                if (Ledgers.Count() == 0)
                {
                    Ledgers = (from c in _customerService.GetAll().Where(i => i.CustomerID == CustomerID)
                               select new CustomerLedgerModel
                               {
                                   Code = c.Code,
                                   CustomerName = c.Name,
                                   Opening = c.TotalDue,
                                   TotalDue = c.TotalDue,
                                   Closing = c.TotalDue,
                                   InvoiceNo = c.Code,
                                   InvoiceDate = toDate
                               }).ToList();
                }

                var totaldue = (from l in Ledgers
                                group l by new { l.InvoiceDate, l.CustomerID, l.Closing } into g
                                select new
                                {
                                    g.Key.InvoiceDate,
                                    g.Key.CustomerID,
                                    g.Key.Closing
                                }).OrderBy(i => i.CustomerID).ThenByDescending(i => i.InvoiceDate).ToList();

                var due = (from d in totaldue
                           group d by new { d.CustomerID } into g
                           select new
                           {
                               g.Key.CustomerID,
                               InvoiceDate = g.Select(i => i.InvoiceDate).FirstOrDefault(),
                               Closing = g.Select(i => i.Closing).FirstOrDefault()
                           }).ToList();
                AllCustomerTotalDue = due.Sum(i => i.Closing);
            }

            foreach (var item in Ledgers)
            {
                Remarks = item.SOrderID != null ? GetRemarksByTransID(item.SOrderID) : "";
                dt.Rows.Add(item.ConcernID, item.CustomerID, item.Code, item.CustomerName, item.InvoiceDate, item.InvoiceNo, item.SOrderID, item.Opening, item.CashSales, item.DueSales, item.TotalSalesAmt, item.TotalDue, item.CollectionAmt + item.CashSales, item.AdjustAmt, item.Closing, 0, "", 0, 0, item.ProductReturnAmt, item.PenaltyInterestAmt, Remarks);
            }

            if (CustomerID == 0)
            {
                var NonTrans = NonTransCustomers(fromDate, toDate);
                AllCustomerTotalDue = _customerService.GetAll().Where(i => i.TotalDue != 0).Sum(i => i.TotalDue);
                foreach (var item in NonTrans)
                {
                    dt.Rows.Add(item.ConcernID, item.CustomerID, item.Code, item.CustomerName, item.InvoiceDate, item.InvoiceNo, item.SOrderID, item.Opening, item.CashSales, item.DueSales, item.TotalSalesAmt, item.TotalDue, item.CollectionAmt + item.CashSales, item.AdjustAmt, item.Closing, 0, "", 0, 0, item.ProductReturnAmt, item.PenaltyInterestAmt, "No Transaction Exists");
                }
            }

            dt.TableName = "dtCustomerLedger";
            _dataSet.Tables.Add(dt);

            GetCommonParameters(userName, concernID);

            _reportParameter = new ReportParameter("DateRange", " Summary Date from: " + fromDate.ToString("dd MMM yyyy") + " to " + toDate.ToString("dd MMM yyyy"));
            _reportParameters.Add(_reportParameter);
            AllCustomerTotalDue = AllCustomerTotalDue == 0m ? Closing : AllCustomerTotalDue;
            _reportParameter = new ReportParameter("AllCustomerTotalDue", AllCustomerTotalDue.ToString("F"));
            _reportParameters.Add(_reportParameter);


            return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Sales\\rptCustomerLedgerSummary.rdlc");

        }


        public byte[] CustomerLedgerDetails(DateTime fromDate, DateTime toDate, string userName, int concernID, int CustomerID)
        {
            var customerLedgerdata = _salesOrderService.CustomerLedger(fromDate, toDate, concernID, CustomerID);
            TransactionalDataSet.dtCustomerLedgerDataTable dt = new TransactionalDataSet.dtCustomerLedgerDataTable();
            _dataSet = new DataSet();

            foreach (var item in customerLedgerdata)
            {
                dt.Rows.Add(item.ConcernID, item.CustomerID, item.Code, item.CustomerName, item.InvoiceDate, item.InvoiceNo, item.SOrderID, item.Opening, item.CashSales, item.DueSales, item.TotalSalesAmt, item.TotalDue, item.CollectionAmt + item.CashSales, item.AdjustAmt, item.Closing, item.ProductID, item.ProductName, item.Quantity, item.ProSalesAmt, item.ProductReturnAmt, item.PenaltyInterestAmt, "");
            }

            dt.TableName = "dtCustomerLedger";
            _dataSet.Tables.Add(dt);

            GetCommonParameters(userName, concernID);

            _reportParameter = new ReportParameter("DateRange", "Date from: " + fromDate.ToString("dd MMM yyyy") + " to " + toDate.ToString("dd MMM yyyy"));
            _reportParameters.Add(_reportParameter);
            return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Sales\\rptCustomerLedger.rdlc");

        }


        public byte[] CustomerDueReport(DateTime fromDate, DateTime toDate, string userName, int concernID, int CustomerID, int IsOnlyDue)
        {
            var customerdue = _salesOrderService.CustomerDue(fromDate, toDate, concernID, CustomerID, IsOnlyDue).ToList();
            decimal closingGrandtotal = 0;

            var tempZeroClosing = (from cd in customerdue
                                   group cd by cd.CustomerID into g
                                   select new
                                   {
                                       CustomerID = g.Key,
                                       Name = g.FirstOrDefault().Name,
                                       Closing = g.LastOrDefault().Balance,
                                   }).Where(i => i.Closing == 0).ToList();

            customerdue.RemoveAll(i => tempZeroClosing.Any(j => j.CustomerID == i.CustomerID));

            List<Tuple<int, decimal>> GrandTotalCal = new List<Tuple<int, decimal>>();

            TransactionalDataSet.dtCustomerDueDataTable dt = new TransactionalDataSet.dtCustomerDueDataTable();
            _dataSet = new DataSet();
            int CID = 0;
            DateTime CDate = DateTime.MinValue;
            bool IsSalesFound = false;
            foreach (var item in customerdue)
            {
                if (CID != item.CustomerID)
                {
                    IsSalesFound = false;
                    if (item.Status.Equals("aSales") || item.Status.Equals("bCreditSales") || item.Status.Equals("RSales"))
                    {
                        if (item.TransDate >= fromDate && item.TransDate <= toDate)
                        {
                            IsSalesFound = true;
                            CDate = item.TransDate;
                            GrandTotalCal.Add(new Tuple<int, decimal>(item.CustomerID, item.Balance));
                            dt.Rows.Add(item.TransDate, item.CustomerID, item.ConcernID, item.TransDate, item.Code, item.Name, item.Address, item.ContactNo, item.InvoiceNo, item.SalesAmount, item.DueSales, item.InterestAmt, item.TotalSalesAmt, item.RecAmount, item.CollectionAmt, item.Status, item.Balance, item.AdjustAmt, item.ReturnAmt, item.InstallmentPeriod);
                        }
                    }
                    else
                    {
                        //no operation for Cash collection and credit collection status
                    }

                }
                else
                {
                    if (item.Status.Equals("aSales") || item.Status.Equals("bCreditSales") || item.Status.Equals("RSales"))
                    {
                        if (item.TransDate >= fromDate && item.TransDate <= toDate)
                        {
                            if (CDate == DateTime.MinValue)
                                CDate = item.TransDate;
                            IsSalesFound = true;
                            GrandTotalCal.Add(new Tuple<int, decimal>(item.CustomerID, item.Balance));
                            dt.Rows.Add(
                                item.TransDate,
                                item.CustomerID,
                                item.ConcernID,
                                CDate,
                                item.Code,
                                item.Name,
                                item.Address,
                                item.ContactNo,
                                item.InvoiceNo,
                                item.SalesAmount,
                                item.DueSales,
                                item.InterestAmt,
                                item.TotalSalesAmt,
                                item.RecAmount,
                                item.CollectionAmt,
                                item.Status,
                                item.Balance,
                                item.AdjustAmt,
                                item.ReturnAmt,
                                item.InstallmentPeriod);
                        }
                    }
                    else
                    {
                        if (IsSalesFound == true)
                        {
                            if (item.TransDate >= fromDate)
                            {
                                GrandTotalCal.Add(new Tuple<int, decimal>(item.CustomerID, item.Balance));
                                dt.Rows.Add(item.TransDate, item.CustomerID, item.ConcernID, CDate, item.Code, item.Name, item.Address, item.ContactNo, item.InvoiceNo, item.SalesAmount, item.DueSales, item.InterestAmt, item.TotalSalesAmt, item.RecAmount, item.CollectionAmt, item.Status, item.Balance, item.AdjustAmt, item.ReturnAmt, item.InstallmentPeriod);
                            }
                        }
                    }
                }

                CID = item.CustomerID;
            }

            dt.TableName = "dtCustomerDue";
            _dataSet.Tables.Add(dt);

            var cl = (from cd in GrandTotalCal
                      group cd by cd.Item1 into g
                      select new
                      {
                          Id = g.Key,
                          Closing = g.LastOrDefault().Item2,
                      }).ToList();

            closingGrandtotal = cl.Sum(i => i.Closing);


            GetCommonParameters(userName, concernID);

            _reportParameter = new ReportParameter("DateRange", "Date from: " + fromDate.ToString("dd MMM yyyy") + " to " + toDate.ToString("dd MMM yyyy"));
            _reportParameters.Add(_reportParameter);
            _reportParameter = new ReportParameter("ClosingGrandTotal", closingGrandtotal.ToString());
            _reportParameters.Add(_reportParameter);

            if (concernID == (int)EnumSisterConcern.NOKIA_CONCERNID || concernID == (int)EnumSisterConcern.WALTON_CONCERNID || concernID == (int)EnumSisterConcern.KINGSTAR_CONCERNID || concernID == (int)EnumSisterConcern.NOKIA_STORE_MAGURA_CONCERNID)
                return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Sales\\rptCustomerDueMobile.rdlc");

            return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Sales\\rptCustomerDue.rdlc");
        }


        public byte[] DailyStockVSSalesSummary(DateTime fromDate, DateTime toDate, string userName, int concernID, int ProductID)
        {
            var customerLedgerdata = _StockServce.DailyStockVSSalesSummary(fromDate, toDate, concernID, ProductID);
            TransactionalDataSet.dtDailyStockandSalesSummaryDataTable dt = new TransactionalDataSet.dtDailyStockandSalesSummaryDataTable();
            _dataSet = new DataSet();

            int ProductID_old = 0;

            double TotalOpeningStockQty = 0;
            double TotalOpeningStockValue = 0;
            double TotalClosingQty = 0;
            double TotalClosingValue = 0;
            double TotalClosingQtyTemp = 0;
            double TotalClosingValueTemp = 0;

            foreach (var item in customerLedgerdata)
            {
                if (ProductID_old != item.ProductID)
                {
                    TotalOpeningStockQty = TotalOpeningStockQty + (double)item.OpeningStockQuantity;
                    TotalOpeningStockValue = TotalOpeningStockValue + (double)item.OpeningStockValue;

                    TotalClosingQty = TotalClosingQty + TotalClosingQtyTemp;
                    TotalClosingValue = TotalClosingValue + TotalClosingValueTemp;

                    TotalClosingQtyTemp = (double)item.ClosingStockQuantity;
                    TotalClosingValueTemp = (double)item.ClosingStockValue;

                }
                else
                {
                    TotalClosingQtyTemp = (double)item.ClosingStockQuantity;
                    TotalClosingValueTemp = (double)item.ClosingStockValue;
                }
                dt.Rows.Add(item.Date, item.ConcernID, item.ProductID, item.Code, item.ProductName, item.ColorID, item.ColorName, item.OpeningStockQuantity, item.TotalStockQuantity, item.PurchaseQuantity, item.SalesQuantity, item.ClosingStockQuantity, item.OpeningStockValue, item.TotalStockValue, item.ClosingStockValue, item.ReturnQuantity, item.SalesQuantity - item.ReturnQuantity);



                ProductID_old = item.ProductID;
            }


            TotalClosingQty = TotalClosingQty + TotalClosingQtyTemp;
            TotalClosingValue = TotalClosingValue + TotalClosingValueTemp;





            dt.TableName = "dtDailyStockandSalesSummary";
            _dataSet.Tables.Add(dt);

            GetCommonParameters(userName, concernID);

            _reportParameter = new ReportParameter("TotalOpeningStockQty", TotalOpeningStockQty.ToString());
            _reportParameters.Add(_reportParameter);
            _reportParameter = new ReportParameter("TotalOpeningStockValue", Convert.ToDecimal(TotalOpeningStockValue).ToString());
            _reportParameters.Add(_reportParameter);
            _reportParameter = new ReportParameter("TotalClosingQty", TotalClosingQty.ToString());
            _reportParameters.Add(_reportParameter);
            _reportParameter = new ReportParameter("TotalClosingValue", Convert.ToDecimal(TotalClosingValue).ToString());
            _reportParameters.Add(_reportParameter);
            _reportParameter = new ReportParameter("DateRange", "Date from: " + fromDate.ToString("dd MMM yyyy") + " to " + toDate.ToString("dd MMM yyyy"));
            _reportParameters.Add(_reportParameter);

            return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Stock\\rptDailyStockandSalesSummary.rdlc");
        }

        public byte[] DailyCashBookLedger(DateTime fromDate, DateTime toDate, string userName, int concernID)
        {
            var data = _CashCollectionService.CashInHandReport(fromDate, toDate, concernID);
            //TransactionalDataSet.dtDailyCashbookLedgerDataTable dt = new TransactionalDataSet.dtDailyCashbookLedgerDataTable();
            TransactionalDataSet.dtDailyCashbookLedgerNewDataTable dt = new TransactionalDataSet.dtDailyCashbookLedgerNewDataTable();
            _dataSet = new DataSet();

            decimal OpeningCashInHand = 0m;
            decimal CurrentCashInHand = 0m;
            decimal ClosingCashInHand = 0m;
            decimal TotalPayable = 0m;
            decimal TotalRecivable = 0m;


            foreach (var item in data)
            {

                if (item.Expense == "Opening Cash In Hand")
                {
                    OpeningCashInHand = item.ExpenseAmt;
                }
                else if (item.Expense == "Current Cash In Hand")
                {
                    CurrentCashInHand = item.ExpenseAmt;
                }
                else if (item.Expense == "Closing Cash In Hand")
                {
                    ClosingCashInHand = item.ExpenseAmt;
                }

                else if (item.Expense == "Total Payable")
                {
                    TotalPayable = item.ExpenseAmt;
                }


                else if (item.Expense == "Total Recivable")
                {
                    TotalRecivable = item.ExpenseAmt;
                }
                else
                {


                    dt.Rows.Add(item.TransDate, item.id, item.Expense, item.ExpenseAmt, item.Income, item.IncomeAmt);
                    //dt.Rows.Add(item.ConcernID, item.Date, item.OpeningBalance, item.CashSales, item.DueCollection, item.DownPayment, item.InstallAmt, item.Loan, item.BankWithdrwal, item.OthersIncome, item.TotalIncome, item.PaidAmt, item.Delivery, item.EmployeeSalary, item.Conveyance, item.BankDeposit, item.LoanPaid, item.Vat, item.OthersExpense, item.SRET, item.TotalExpense, item.ClosingBalance);
                }

            }





            dt.TableName = "dtDailyCashbookLedgerNew";
            _dataSet.Tables.Add(dt);

            GetCommonParameters(userName, concernID);

            _reportParameter = new ReportParameter("DateRange", "Date from: " + fromDate.ToString("dd MMM yyyy") + " to " + toDate.ToString("dd MMM yyyy"));
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("OpeningCashInHand", OpeningCashInHand.ToString("0.00"));
            _reportParameters.Add(_reportParameter);
            _reportParameter = new ReportParameter("CurrentCashInHand", CurrentCashInHand.ToString("0.00"));
            _reportParameters.Add(_reportParameter);
            _reportParameter = new ReportParameter("ClosingCashInHand", ClosingCashInHand.ToString("0.00"));
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("TotalPayable", TotalPayable.ToString("0.00"));
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("TotalRecivable", TotalRecivable.ToString("0.00"));
            _reportParameters.Add(_reportParameter);

            return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Others\\rptDailyCashStatementNew.rdlc");

        }




        public byte[] ProfitAndLossReport(DateTime fromDate, DateTime toDate, string userName, int concernID)
        {
            var data = _CashCollectionService.ProfitAndLossReport(fromDate, toDate, concernID);
            //TransactionalDataSet.dtDailyCashbookLedgerDataTable dt = new TransactionalDataSet.dtDailyCashbookLedgerDataTable();
            TransactionalDataSet.dtDailyCashbookLedgerNewDataTable dt = new TransactionalDataSet.dtDailyCashbookLedgerNewDataTable();
            _dataSet = new DataSet();

            decimal OpeningCashInHand = 0m;
            decimal CurrentCashInHand = 0m;
            decimal ClosingCashInHand = 0m;
            decimal TotalPayable = 0m;
            decimal TotalRecivable = 0m;


            foreach (var item in data)
            {

                if (item.Expense == "Opening Cash In Hand")
                {
                    OpeningCashInHand = item.ExpenseAmt;
                }
                else if (item.Expense == "Current Cash In Hand")
                {
                    CurrentCashInHand = item.ExpenseAmt;
                }
                else if (item.Expense == "Closing Cash In Hand")
                {
                    ClosingCashInHand = item.ExpenseAmt;
                }

                else if (item.Expense == "Total Payable")
                {
                    TotalPayable = item.ExpenseAmt;
                }


                else if (item.Expense == "Total Recivable")
                {
                    TotalRecivable = item.ExpenseAmt;
                }
                else
                {


                    dt.Rows.Add(item.TransDate, item.id, item.Expense, item.ExpenseAmt, item.Income, item.IncomeAmt);
                    //dt.Rows.Add(item.ConcernID, item.Date, item.OpeningBalance, item.CashSales, item.DueCollection, item.DownPayment, item.InstallAmt, item.Loan, item.BankWithdrwal, item.OthersIncome, item.TotalIncome, item.PaidAmt, item.Delivery, item.EmployeeSalary, item.Conveyance, item.BankDeposit, item.LoanPaid, item.Vat, item.OthersExpense, item.SRET, item.TotalExpense, item.ClosingBalance);
                }

            }





            dt.TableName = "dtProfitAndLoss";
            _dataSet.Tables.Add(dt);

            GetCommonParameters(userName, concernID);

            _reportParameter = new ReportParameter("DateRange", "Date from: " + fromDate.ToString("dd MMM yyyy") + " to " + toDate.ToString("dd MMM yyyy"));
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("OpeningCashInHand", OpeningCashInHand.ToString("0.00"));
            _reportParameters.Add(_reportParameter);
            _reportParameter = new ReportParameter("CurrentCashInHand", CurrentCashInHand.ToString("0.00"));
            _reportParameters.Add(_reportParameter);
            _reportParameter = new ReportParameter("ClosingCashInHand", ClosingCashInHand.ToString("0.00"));
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("TotalPayable", TotalPayable.ToString("0.00"));
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("TotalRecivable", TotalRecivable.ToString("0.00"));
            _reportParameters.Add(_reportParameter);

            return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Others\\rptProfitAndLossRpt.rdlc");

        }




        public byte[] SummaryReport(DateTime fromDate, DateTime toDate, string userName, int concernID)
        {



            var data = _CashCollectionService.CashInHandReport(fromDate, toDate, concernID);



            decimal OpeningCashInHand = 0m;
            decimal CurrentCashInHand = 0m;
            decimal ClosingCashInHand = 0m;
            decimal TotalPayable = 0m;
            decimal TotalRecivable = 0m;


            foreach (var item in data)
            {

                if (item.Expense == "Opening Cash In Hand")
                {
                    OpeningCashInHand = item.ExpenseAmt;
                }
                else if (item.Expense == "Current Cash In Hand")
                {
                    CurrentCashInHand = item.ExpenseAmt;
                }
                else if (item.Expense == "Closing Cash In Hand")
                {
                    ClosingCashInHand = item.ExpenseAmt;
                }

                else if (item.Expense == "Total Payable")
                {
                    TotalPayable = item.ExpenseAmt;
                }


                else if (item.Expense == "Total Recivable")
                {
                    TotalRecivable = item.ExpenseAmt;
                }
                else
                {



                }

            }









            var Summarydata = _CashCollectionService.SummaryReport(fromDate, toDate, OpeningCashInHand, CurrentCashInHand, ClosingCashInHand, concernID);
            //TransactionalDataSet.dtDailyCashbookLedgerDataTable dt = new TransactionalDataSet.dtDailyCashbookLedgerDataTable();
            TransactionalDataSet.dtSummaryReportNewDataTable dt = new TransactionalDataSet.dtSummaryReportNewDataTable();
            //   TransactionalDataSet.dtDailyCashbookLedgerNewDataTable dt = new TransactionalDataSet.dtDailyCashbookLedgerNewDataTable();
            _dataSet = new DataSet();




            foreach (var item in Summarydata)
            {



                dt.Rows.Add(item.id, item.Category, item.Head, item.Amount, item.Total);
                //dt.Rows.Add(item.ConcernID, item.Date, item.OpeningBalance, item.CashSales, item.DueCollection, item.DownPayment, item.InstallAmt, item.Loan, item.BankWithdrwal, item.OthersIncome, item.TotalIncome, item.PaidAmt, item.Delivery, item.EmployeeSalary, item.Conveyance, item.BankDeposit, item.LoanPaid, item.Vat, item.OthersExpense, item.SRET, item.TotalExpense, item.ClosingBalance);


            }





            dt.TableName = "dtSummaryReportNew";
            _dataSet.Tables.Add(dt);

            GetCommonParameters(userName, concernID);

            _reportParameter = new ReportParameter("ToDate", "Date from: " + fromDate.ToString("dd MMM yyyy") + " to " + toDate.ToString("dd MMM yyyy"));
            _reportParameters.Add(_reportParameter);

            //_reportParameter = new ReportParameter("OpeningCashInHand", OpeningCashInHand.ToString("0.00"));
            //_reportParameters.Add(_reportParameter);
            //_reportParameter = new ReportParameter("CurrentCashInHand", CurrentCashInHand.ToString("0.00"));
            //_reportParameters.Add(_reportParameter);
            //_reportParameter = new ReportParameter("ClosingCashInHand", ClosingCashInHand.ToString("0.00"));
            //_reportParameters.Add(_reportParameter);

            //_reportParameter = new ReportParameter("TotalPayable", TotalPayable.ToString("0.00"));
            //_reportParameters.Add(_reportParameter);

            //_reportParameter = new ReportParameter("TotalRecivable", TotalRecivable.ToString("0.00"));
            //_reportParameters.Add(_reportParameter);

            return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Others\\rptSummaryReport.rdlc");

        }




        public byte[] BankSummaryReport(DateTime fromDate, DateTime toDate, string userName, int concernID, int ProductID)
        {




            var customerLedgerdata = _StockServce.DailyStockVSSalesSummary(fromDate, toDate, concernID, ProductID);
            TransactionalDataSet.dtDailyStockandSalesSummaryDataTable dt = new TransactionalDataSet.dtDailyStockandSalesSummaryDataTable();
            _dataSet = new DataSet();

            int ProductID_old = 0;

            double TotalOpeningStockQty = 0;
            double TotalOpeningStockValue = 0;
            double TotalClosingQty = 0;
            double TotalClosingValue = 0;
            double TotalClosingQtyTemp = 0;
            double TotalClosingValueTemp = 0;

            foreach (var item in customerLedgerdata)
            {
                if (ProductID_old != item.ProductID)
                {
                    TotalOpeningStockQty = TotalOpeningStockQty + (double)item.OpeningStockQuantity;
                    TotalOpeningStockValue = TotalOpeningStockValue + (double)item.OpeningStockValue;

                    TotalClosingQty = TotalClosingQty + TotalClosingQtyTemp;
                    TotalClosingValue = TotalClosingValue + TotalClosingValueTemp;

                    TotalClosingQtyTemp = (double)item.ClosingStockQuantity;
                    TotalClosingValueTemp = (double)item.ClosingStockValue;

                }
                else
                {
                    TotalClosingQtyTemp = (double)item.ClosingStockQuantity;
                    TotalClosingValueTemp = (double)item.ClosingStockValue;
                }
                dt.Rows.Add(item.Date, item.ConcernID, item.ProductID, item.Code, item.ProductName, item.ColorID, item.ColorName, item.OpeningStockQuantity, item.TotalStockQuantity, item.PurchaseQuantity, item.SalesQuantity, item.ClosingStockQuantity, item.OpeningStockValue, item.TotalStockValue, item.ClosingStockValue, item.ReturnQuantity, item.SalesQuantity - item.ReturnQuantity);



                ProductID_old = item.ProductID;
            }


            TotalClosingQty = TotalClosingQty + TotalClosingQtyTemp;
            TotalClosingValue = TotalClosingValue + TotalClosingValueTemp;





            dt.TableName = "dtDailyStockandSalesSummary";
            _dataSet.Tables.Add(dt);

            GetCommonParameters(userName, concernID);

            _reportParameter = new ReportParameter("TotalOpeningStockQty", TotalOpeningStockQty.ToString());
            _reportParameters.Add(_reportParameter);
            _reportParameter = new ReportParameter("TotalOpeningStockValue", Convert.ToDecimal(TotalOpeningStockValue).ToString());
            _reportParameters.Add(_reportParameter);
            _reportParameter = new ReportParameter("TotalClosingQty", TotalClosingQty.ToString());
            _reportParameters.Add(_reportParameter);
            _reportParameter = new ReportParameter("TotalClosingValue", Convert.ToDecimal(TotalClosingValue).ToString());
            _reportParameters.Add(_reportParameter);
            _reportParameter = new ReportParameter("DateRange", "Date from: " + fromDate.ToString("dd MMM yyyy") + " to " + toDate.ToString("dd MMM yyyy"));
            _reportParameters.Add(_reportParameter);

            return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Stock\\rptDailyStockandSalesSummary.rdlc");
        }

        //public byte[] BankLedgerReport(DateTime fromDate, DateTime toDate, string userName, int concernID, int BankID)
        //{

        //    var bankLedgerdata = _bankTransactionService.BankLedger(fromDate, toDate, concernID, BankID);
        //    TransactionalDataSet.dtBankLedgerDataTable dttemp = new TransactionalDataSet.dtBankLedgerDataTable();
        //    TransactionalDataSet.dtBankLedgerDataTable dt = new TransactionalDataSet.dtBankLedgerDataTable();

        //    _dataSet = new DataSet();
        //    double Opening = 0;
        //    double Closing = 0;
        //    double GrandClosing = 0;
        //    double GrandOpening = 0;
        //    int BankIDTemp = 0;

        //    foreach (var item in bankLedgerdata)
        //    {
        //        if (BankIDTemp != item.BankID)
        //        {
        //            Opening = (double)item.Opening;
        //            Closing = Opening + (double)item.Deposit - (double)item.Widthdraw + (double)item.CashCollection - (double)item.CashDelivery + (double)item.FundIN - (double)item.FundOut;

        //        }
        //        else
        //        {

        //            Opening = Closing;
        //            Closing = Opening + (double)item.Deposit - (double)item.Widthdraw + (double)item.CashCollection - (double)item.CashDelivery + (double)item.FundIN - (double)item.FundOut;

        //        }

        //        if (item.TransDate >= fromDate && item.TransDate <= toDate)
        //            dt.Rows.Add(item.ConcernID, item.BankID, item.BankName, item.TransDate, item.TransactionNo, Opening, item.Deposit, item.Widthdraw, item.CashCollection, item.CashDelivery, item.FundIN, item.FundOut, Closing);

        //        BankIDTemp = item.BankID;




        //    }

        //    BankIDTemp = 0;
        //    Opening = 0;
        //    foreach (var item in bankLedgerdata)
        //    {
        //        if (BankIDTemp != item.BankID)
        //        {
        //            Opening = (double)item.Opening;
        //            GrandOpening = GrandOpening + (double)item.Opening;
        //        }
        //        else
        //        {
        //            Opening = 0;
        //        }

        //        GrandClosing = GrandClosing + Opening + (double)item.Deposit - (double)item.Widthdraw + (double)item.CashCollection - (double)item.CashDelivery + (double)item.FundIN - (double)item.FundOut;



        //        BankIDTemp = item.BankID;

        //    }

        //    dt.TableName = "dtBankLedger";
        //    _dataSet.Tables.Add(dt);

        //    GetCommonParameters(userName, concernID);

        //    _reportParameter = new ReportParameter("DateRange", "Date from: " + fromDate.ToString("dd MMM yyyy") + " to " + toDate.ToString("dd MMM yyyy"));
        //    _reportParameters.Add(_reportParameter);

        //    _reportParameter = new ReportParameter("GrandClosing", GrandClosing.ToString());
        //    _reportParameters.Add(_reportParameter);
        //    _reportParameter = new ReportParameter("GrandOpening", GrandOpening.ToString());
        //    _reportParameters.Add(_reportParameter);


        //    return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Bank\\rptBankLedger.rdlc");
        //}

        public byte[] BankLedgerReport(DateTime fromDate, DateTime toDate, string userName, int concernID, int BankID)
        {

            var bankLedgerdata = _bankTransactionService.BankLedger(fromDate, toDate, BankID);
            TransactionalDataSet.dtBankLedgerDataTable dt = new TransactionalDataSet.dtBankLedgerDataTable();
            _dataSet = new DataSet();
            DataRow row = null;
            foreach (var item in bankLedgerdata)
            {
                row = dt.NewRow();
                row["ConcernID"] = item.ConcernID;
                row["BankID"] = item.BankID;
                row["BankName"] = item.BankName;
                row["TransDate"] = item.TransDate;
                row["TransactionNo"] = item.TransactionNo;
                row["Opening"] = item.Opening;
                row["Deposit"] = item.Deposit;
                row["Withdraw"] = item.Withdraw;
                row["CashCollection"] = item.CashCollection;
                row["CashDelivery"] = item.CashDelivery;
                row["FundIN"] = item.FundIN;
                row["FundOut"] = item.FundOut;
                row["Closing"] = item.Closing;
                row["AccountNo"] = item.AccountNO;
                row["AccountName"] = item.AccountName;
                row["FromToAccNo"] = item.FromToAccountNo;
                row["ConcernName"] = item.ConcernName;
                dt.Rows.Add(row);
            }
            dt.TableName = "dtBankLedger";
            _dataSet.Tables.Add(dt);

            GetCommonParameters(userName, concernID);
            var bank = bankLedgerdata.FirstOrDefault();
            string header = bank != null ? ("Bank:" + bank.BankName + ", A/C Name: " + bank.AccountName + ", A/C No.: " + bank.AccountNO) : "";
            _reportParameter = new ReportParameter("DateRange", "Bank Ledger of " + header + " Date from: " + fromDate.ToString("dd MMM yyyy") + " to " + toDate.ToString("dd MMM yyyy"));
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("GrandClosing", "");
            _reportParameters.Add(_reportParameter);
            _reportParameter = new ReportParameter("GrandOpening", "");
            _reportParameters.Add(_reportParameter);


            return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Bank\\rptBankLedger.rdlc");
        }
        public byte[] ReplacementInvoiceReport(IEnumerable<ReplaceOrderDetail> ROrderDetails, ReplaceOrder ROrder, string userName, int concernID)
        {
            TransactionalDataSet.dtReplaceInvoiceDataTable dt = new TransactionalDataSet.dtReplaceInvoiceDataTable();
            _dataSet = new DataSet();
            Customer customer = _customerService.GetCustomerById(ROrder.CustomerId);
            decimal dtotalamount = 0, rtotlaamount = 0;
            int TotalQty = 0;
            foreach (var item in ROrderDetails)
            {
                dt.Rows.Add(item.DamageProductName, item.ProductName, item.DamageIMEINO, item.ReplaceIMEINO, item.DamageUnitPrice, item.UnitPrice, item.Quantity, item.Quantity, item.Remarks);
                dtotalamount += Convert.ToDecimal(item.DamageUnitPrice);
                TotalQty += Convert.ToInt32(item.Quantity);
                rtotlaamount += Convert.ToDecimal(item.UnitPrice);
            }

            dt.TableName = "dtReplaceInvoice";
            _dataSet.Tables.Add(dt);

            GetCommonParameters(userName, concernID);

            //_reportParameter = new ReportParameter("Total", ROrder.TotalAmount);
            //_reportParameters.Add(_reportParameter);

            //_reportParameter = new ReportParameter("GTotal", (oOrder.GrandTotal + (oOrder.Customer.TotalDue - oOrder.PaymentDue)).ToString());

            //_reportParameter = new ReportParameter("GTotal", "0.00");
            //_reportParameters.Add(_reportParameter);

            //_reportParameter = new ReportParameter("Paid", ROrder.RecieveAmount.ToString());
            //_reportParameters.Add(_reportParameter);

            //_reportParameter = new ReportParameter("CurrDue", (ROrder.PaymentDue).ToString());
            //_reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("InvoiceNo", ROrder.InvoiceNo);
            _reportParameters.Add(_reportParameter);

            //_reportParameter = new ReportParameter("TotalDue", oOrder.TotalDue.ToString());
            _reportParameter = new ReportParameter("TotalDue", customer.TotalDue.ToString());
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("InvoiceDate", ROrder.OrderDate.ToString());
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("Company", customer.CompanyName);
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("CAddress", customer.Address);
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("Name", customer.Name);
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("MobileNo", customer.ContactNo);
            _reportParameters.Add(_reportParameter);


            _reportParameter = new ReportParameter("DamageTotalAmount", dtotalamount.ToString());
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("ReplaceTotalAmount", rtotlaamount.ToString());
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("TotalQty", TotalQty.ToString());
            _reportParameters.Add(_reportParameter);

            return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Sales\\rptReplacementOrderInvoice.rdlc");
        }


        public byte[] ReplaceInvoiceReportByID(int orderId, string username, int concernID)
        {
            TransactionalDataSet.dtReplaceInvoiceDataTable dt = new TransactionalDataSet.dtReplaceInvoiceDataTable();
            var ROrder = _salesOrderService.GetSalesOrderById(Convert.ToInt32(orderId));
            var rorderdetaisl = _salesOrderService.GetReplaceOrderInvoiceReportByID(orderId);
            Customer customer = _customerService.GetCustomerById(ROrder.CustomerID);
            decimal dtotalamount = 0, rtotlaamount = 0;
            int TotalQty = 0;
            _dataSet = new DataSet();
            foreach (var item in rorderdetaisl)
            {
                dt.Rows.Add(item.DamageProductName, item.ProductName, item.DamageIMEINO, item.ReplaceIMEINO, item.DamageUnitPrice, item.UnitPrice, item.Quantity, item.Quantity, item.Remarks);
                dtotalamount += Convert.ToDecimal(item.DamageUnitPrice);
                TotalQty += Convert.ToInt32(item.Quantity);
                rtotlaamount += Convert.ToDecimal(item.UnitPrice);
            }

            dt.TableName = "dtReplaceInvoice";
            _dataSet.Tables.Add(dt);

            #region Parameter
            GetCommonParameters(username, concernID);

            //_reportParameter = new ReportParameter("Total", ROrder.TotalAmount.ToString());
            //_reportParameters.Add(_reportParameter);

            //_reportParameter = new ReportParameter("GTotal", (oOrder.GrandTotal + (oOrder.Customer.TotalDue - oOrder.PaymentDue)).ToString());

            //_reportParameter = new ReportParameter("GTotal", "0.00");
            //_reportParameters.Add(_reportParameter);

            //_reportParameter = new ReportParameter("Paid", ROrder.RecAmount.ToString());
            //_reportParameters.Add(_reportParameter);

            //_reportParameter = new ReportParameter("CurrDue", (ROrder.PaymentDue).ToString());
            //_reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("InvoiceNo", ROrder.InvoiceNo);
            _reportParameters.Add(_reportParameter);

            //_reportParameter = new ReportParameter("TotalDue", oOrder.TotalDue.ToString());
            _reportParameter = new ReportParameter("TotalDue", customer.TotalDue.ToString());
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("InvoiceDate", ROrder.InvoiceDate.ToString());
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("Company", customer.CompanyName);
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("CAddress", customer.Address);
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("Name", customer.Name);
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("MobileNo", customer.ContactNo);
            _reportParameters.Add(_reportParameter);


            _reportParameter = new ReportParameter("DamageTotalAmount", dtotalamount.ToString());
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("ReplaceTotalAmount", rtotlaamount.ToString());
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("TotalQty", TotalQty.ToString());
            _reportParameters.Add(_reportParameter);
            #endregion

            return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Sales\\rptReplacementOrderInvoice.rdlc");
        }


        public byte[] ReturnInvoiceReport(IEnumerable<ReplaceOrderDetail> ROrderDetails, ReplaceOrder ROrder, string userName, int concernID)
        {
            TransactionalDataSet.dtReturnInvoiceDataTable dt = new TransactionalDataSet.dtReturnInvoiceDataTable();
            _dataSet = new DataSet();
            Customer customer = _customerService.GetCustomerById(ROrder.CustomerId);
            foreach (var item in ROrderDetails)
            {
                dt.Rows.Add(item.DamageProductName, item.DamageIMEINO, item.UnitPrice, item.Quantity, item.UnitPrice * item.Quantity);
            }

            dt.TableName = "dtReturnInvoice";
            _dataSet.Tables.Add(dt);

            GetCommonParameters(userName, concernID);

            //_reportParameter = new ReportParameter("Total", ROrder.TotalAmount);
            //_reportParameters.Add(_reportParameter);

            //_reportParameter = new ReportParameter("GTotal", (oOrder.GrandTotal + (oOrder.Customer.TotalDue - oOrder.PaymentDue)).ToString());

            //_reportParameter = new ReportParameter("GTotal", "0.00");
            //_reportParameters.Add(_reportParameter);

            //_reportParameter = new ReportParameter("Paid", ROrder.RecieveAmount.ToString());
            //_reportParameters.Add(_reportParameter);

            //_reportParameter = new ReportParameter("CurrDue", (ROrder.PaymentDue).ToString());
            //_reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("InvoiceNo", ROrder.InvoiceNo);
            _reportParameters.Add(_reportParameter);

            //_reportParameter = new ReportParameter("TotalDue", oOrder.TotalDue.ToString());
            _reportParameter = new ReportParameter("TotalDue", customer.TotalDue.ToString());
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("InvoiceDate", ROrder.OrderDate.ToString());
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("Company", customer.CompanyName);
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("CAddress", customer.Address);
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("Name", customer.Name);
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("MobileNo", customer.ContactNo);
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("Remarks", "Remarks: " + ROrder.Remarks);
            _reportParameters.Add(_reportParameter);

            return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Sales\\rptReturnInvoice.rdlc");
        }

        public byte[] ReturnInvoiceReportByID(int orderId, string username, int concernID)
        {
            ROrder ROrder = new ROrder();
            ROrder = _returnOrderService.GetReturnOrderById(Convert.ToInt32(orderId));
            var Details = _returnOrderService.GetDetailsByReturnID(orderId);
            return DisplaySalesReturnInvoice(ROrder, Details, username, concernID);
        }
        private byte[] DisplaySalesReturnInvoice(ROrder oROrder, List<ProductWiseSalesReportModel> Details, string username, int concernID)
        {
            try
            {
                DataTable orderdDT = new DataTable();
                TransactionalDataSet.dtInvoiceDataTable dt = new TransactionalDataSet.dtInvoiceDataTable();
                TransactionalDataSet.dtWarrentyDataTable dtWarrenty = new TransactionalDataSet.dtWarrentyDataTable();
                Customer customer = _customerService.GetCustomerById(oROrder.CustomerID);

                ProductWisePurchaseModel product = null;
                List<ProductWisePurchaseModel> warrentyList = new List<ProductWisePurchaseModel>();
                ProductWisePurchaseModel warrentyModel = null;
                DataRow row = null;
                string Warrenty = string.Empty;
                string IMEIs = string.Empty;
                int Count = 0;
                bool IsTiles = false;
                decimal AreaPerCarton = 0m, Length = 0m, Width = 0m, AreaPerPcs = 0m;
                decimal[] sizeXY = null;

                foreach (var item in Details)
                {
                    row = dt.NewRow();
                    row["ProductName"] = item.ProductName;
                    row["Quantity"] = Math.Round(item.Quantity / item.ConvertValue, 2);
                    row["UnitQty"] = item.Quantity;
                    row["Rate"] = item.UnitPrice * item.ConvertValue;
                    row["Discount"] = item.PPDAmount * item.ConvertValue;
                    row["Amount"] = item.TotalAmount;
                    row["DisPer"] = item.PPDAmount;
                    row["DisAmt"] = item.PPDAmount;
                    row["ChasisNo"] = string.Empty;
                    row["Color"] = item.ColorName;
                    row["EngineNo"] = string.Empty;
                    row["OfferValue"] = item.PPOffer;
                    row["CategoryName"] = item.CategoryName;
                    row["UnitName"] = item.UnitName;
                    row["ProductCode"] = item.ProductCode;
                    row["CompanyName"] = item.CompanyName;

                    if (item.CategoryName.ToLower().Equals("tiles"))
                    {
                        row["Rate"] = item.SFTRate;
                        row["Amount"] = item.SFTRate * item.TotalSFT; //item.TotalAmt;
                        sizeXY = Array.ConvertAll(item.SizeName.ToLower().Split('x'), decimal.Parse);
                        Length = (sizeXY[0] / 2.5m); //ft
                        Width = (sizeXY[1] / 2.5m); //ft
                        row["SizeName"] = Length.ToString() + "X" + Width.ToString();
                        row["AreaPerCarton"] = item.SalesCSft;
                        row["RatePerSqft"] = item.SFTRate;
                        row["TotalArea"] = item.TotalSFT;
                        IsTiles = true;
                    }
                    else
                    {
                        row["SizeName"] = item.SizeName;
                        row["RatePerSqft"] = Math.Round(item.UnitPrice * item.ConvertValue, 2);
                    }
                    dt.Rows.Add(row);
                }

                if (dt != null && (dt.Rows != null && dt.Rows.Count > 0))
                    orderdDT = dt.AsEnumerable().OrderBy(o => (String)o["ProductName"]).CopyToDataTable();
                dt.TableName = "dtInvoice";
                _dataSet = new DataSet();
                _dataSet.Tables.Add(dt);
                dtWarrenty.TableName = "dtWarrenty";
                _dataSet.Tables.Add(dtWarrenty);

                string sInwodTk = TakaFormat(Convert.ToDouble(oROrder.PaidAmount));
                GetCommonParameters(username, concernID);

                _reportParameter = new ReportParameter("TDiscount", "0.00");
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("Total", (oROrder.GrandTotal).ToString("0.00"));
                _reportParameters.Add(_reportParameter);

                //_reportParameter = new ReportParameter("GTotal", (oROrder.GrandTotal + (oROrder.Customer.TotalDue - oROrder.PaymentDue)).ToString());

                _reportParameter = new ReportParameter("GTotal", "0.00");
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("Paid", oROrder.PaidAmount.ToString());
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("CurrDue", (customer.TotalDue).ToString());
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("InvoiceNo", oROrder.InvoiceNo);
                _reportParameters.Add(_reportParameter);

                //_reportParameter = new ReportParameter("TotalDue", oROrder.TotalDue.ToString());
                _reportParameter = new ReportParameter("TotalDue", customer.TotalDue.ToString());
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("InvoiceDate", oROrder.ReturnDate.ToString("dd MMM yyyy"));
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("RemindDate", customer.RemindDate != null ? customer.RemindDate.Value.ToString("dd MMM yyyy") : "");
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("Company", customer.CompanyName);
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("CAddress", customer.Address);
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("Remarks", oROrder.Remarks);
                _reportParameters.Add(_reportParameter);


                _reportParameter = new ReportParameter("Name", customer.Name);
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("MobileNo", customer.ContactNo);
                _reportParameters.Add(_reportParameter);
                _reportParameter = new ReportParameter("PrintDate", GetClientDateTime());
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("Header", "Sales Return Invoice");
                _reportParameters.Add(_reportParameter);
                //_reportParameter =new ReportParameter("InWordTK", sInwodTk);
                //_reportParameters.Add(_reportParameter);

                //if (concernID == (int)EnumSisterConcern.NOKIA_CONCERNID || concernID == (int)EnumSisterConcern.WALTON_CONCERNID || concernID == (int)EnumSisterConcern.NOKIA_STORE_MAGURA_CONCERNID)
                //    return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Sales\\rptMSalesInvoice.rdlc");
                //else if (concernID == (int)EnumSisterConcern.KINGSTAR_CONCERNID)
                //    return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Sales\\rptKSalesInvoice.rdlc");
                //else if (concernID == (int)EnumSisterConcern.HAVEN_ENTERPRISE_CONCERNID || concernID == (int)EnumSisterConcern.HAWRA_ENTERPRISE_CONCERNID)
                if (IsTiles)
                    return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Sales\\TilesInvoice.rdlc");
                else
                    return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Sales\\AMSalesInvoice.rdlc");
                // else
                //  return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Sales\\rptSSSalesInvoice.rdlc");
            }
            catch (Exception)
            {

                throw;
            }

        }
        public byte[] DailyWorkSheet(DateTime fromDate, DateTime toDate, string userName, int concernID)
        {
            var reportData = _salesOrderService.DailyWorkSheetReport(fromDate, toDate, concernID);
            TransactionalDataSet.dtDailyWorkSheetDataTable dt = new TransactionalDataSet.dtDailyWorkSheetDataTable();
            _dataSet = new DataSet();
            foreach (var item in reportData)
            {
                dt.Rows.Add(item.ConcernID, item.Date, item.OpeningBalance, item.CashSales, item.DueCollection, item.DownPayment, item.InstallAmt, item.Loan, item.BankWithdrwal, item.OthersIncome, item.TotalIncome, item.DueSales, item.PaidAmt, item.Delivery, item.EmployeeSalary, item.Conveyance, item.BankDeposit, item.LoanPaid, item.Vat, item.OthersExpense, item.SRET, item.TotalExpense, item.ClosingBalance);
            }

            dt.TableName = "dtDailyWorkSheet";
            _dataSet.Tables.Add(dt);

            GetCommonParameters(userName, concernID);

            _reportParameter = new ReportParameter("DateRange", "Date from: " + fromDate.ToString("dd MMM yyyy") + " to " + toDate.ToString("dd MMM yyyy"));
            _reportParameters.Add(_reportParameter);

            return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Sales\\rptDailyWorkSheet.rdlc");
        }

        /// <summary>
        /// Author:aminul
        /// Date: 20-Mar-2018
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="userName"></param>
        /// <param name="concernID"></param>
        /// <param name="EmployeeID"></param>
        /// <returns></returns>
        public byte[] SRVisitReportUsingSP(DateTime fromDate, DateTime toDate, string userName, int concernID, int EmployeeID)
        {
            var SRVisitData = _SRVisitService.SRVisitReport(fromDate, toDate, concernID, EmployeeID);
            TransactionalDataSet.dtSRVisitReportDataTable dt = new TransactionalDataSet.dtSRVisitReportDataTable();
            _dataSet = new DataSet();
            List<SRVisitReportModel> SRVisitList = new List<SRVisitReportModel>();
            StringBuilder strBuilderSalesIMEI = new StringBuilder();
            StringBuilder strBuilderOpening = new StringBuilder();
            StringBuilder strBuilderTaken = new StringBuilder();
            StringBuilder strBuilderBalance = new StringBuilder();

            if (SRVisitData.Count() != 0)
            {
                var tempSRVisits = SRVisitData.Where(i => i.TransDate >= fromDate && i.TransDate <= toDate);
                if (tempSRVisits.Count() == 0)
                {
                    var LastSRVisit = SRVisitData.OrderByDescending(i => i.TransDate).FirstOrDefault();
                    if (LastSRVisit.TransDate <= toDate)
                    {
                        var TodaySRVisit = new SRVisitReportModel();
                        TodaySRVisit.TransDate = toDate;
                        TodaySRVisit.balance_qty = LastSRVisit.balance_qty;
                        TodaySRVisit.ConcernID = LastSRVisit.ConcernID;
                        TodaySRVisit.EmployeeId = LastSRVisit.EmployeeId;
                        TodaySRVisit.EmployeeName = LastSRVisit.EmployeeName;
                        TodaySRVisit.imeno_balance = LastSRVisit.imeno_balance;
                        TodaySRVisit.Opening_productno = LastSRVisit.Opening_productno;
                        TodaySRVisit.Opening_imeno = LastSRVisit.imeno_balance;
                        TodaySRVisit.sale_imeno = string.Empty;
                        TodaySRVisit.taken_imeno = string.Empty;
                        SRVisitList.Add(TodaySRVisit);
                    }

                }
                else
                    SRVisitList.AddRange(tempSRVisits);
            }

            string linedraw = "------------------------------------------------------------";
            int OpeningCount = 0, OpeningGrandCount = 0, TakenCount = 0, TakenCountGrand = 0, SalesCount = 0, SalesCountGrand = 0, BalanceCount = 0, BalanceCountGrand = 0;
            foreach (var item in SRVisitList)
            {


                #region SaleIMEI
                string[] SaleIMEI = item.sale_imeno.Split(new char[] { '\r' }, StringSplitOptions.RemoveEmptyEntries);
                SalesCount = SaleIMEI.Length;
                SalesCountGrand += SalesCount;
                string tempProduct = string.Empty;
                int productcounter = 0;
                foreach (var imei in SaleIMEI)
                {
                    int len = imei.LastIndexOf('-');
                    if (len < 0)
                        len = 0;
                    string product = imei.Substring(0, len);

                    if (!product.Equals(tempProduct) && tempProduct.Equals(string.Empty)) //new product
                    {
                        strBuilderSalesIMEI.Append(product);
                        strBuilderSalesIMEI.Append(System.Environment.NewLine);
                        strBuilderSalesIMEI.Append(linedraw);
                        strBuilderSalesIMEI.Append(System.Environment.NewLine);
                        tempProduct = product;
                    }

                    if (!product.Equals(tempProduct)) //last imei of product
                    {
                        strBuilderSalesIMEI.Append(System.Environment.NewLine);
                        strBuilderSalesIMEI.Append(linedraw);
                        strBuilderSalesIMEI.Append(System.Environment.NewLine);
                        strBuilderSalesIMEI.Append("           SubTotal: " + productcounter);
                        strBuilderSalesIMEI.Append(System.Environment.NewLine);
                        productcounter = 0;


                        tempProduct = product;
                        strBuilderSalesIMEI.Append(product);
                        strBuilderSalesIMEI.Append(System.Environment.NewLine);
                        strBuilderSalesIMEI.Append(linedraw);
                        strBuilderSalesIMEI.Append(System.Environment.NewLine);
                    }

                    strBuilderSalesIMEI.Append(imei.Substring(product.Length + 1) + ", ");
                    productcounter++;


                }
                strBuilderSalesIMEI.Append(linedraw);
                strBuilderSalesIMEI.Append(System.Environment.NewLine);
                strBuilderSalesIMEI.Append("             Total: " + SalesCount);
                #endregion

                #region Opening
                tempProduct = string.Empty;
                productcounter = 0;
                string[] Opening_imeno = item.Opening_imeno.Split(new char[] { '\r' }, StringSplitOptions.RemoveEmptyEntries);
                OpeningCount = Opening_imeno.Length;
                OpeningGrandCount += OpeningCount;
                foreach (var imei in Opening_imeno)
                {
                    int len = imei.LastIndexOf('-');
                    if (len < 0)
                        len = 0;
                    string product = imei.Substring(0, len);

                    if (!product.Equals(tempProduct) && tempProduct.Equals(string.Empty))
                    {
                        strBuilderOpening.Append(product);
                        strBuilderOpening.Append(System.Environment.NewLine);
                        strBuilderOpening.Append(linedraw);
                        strBuilderOpening.Append(System.Environment.NewLine);
                        tempProduct = product;
                    }


                    if (!product.Equals(tempProduct))
                    {
                        strBuilderOpening.Append(System.Environment.NewLine);
                        strBuilderOpening.Append(linedraw);
                        strBuilderOpening.Append(System.Environment.NewLine);
                        strBuilderOpening.Append("           SubTotal: " + productcounter);
                        strBuilderOpening.Append(System.Environment.NewLine);
                        productcounter = 0;

                        tempProduct = product;
                        strBuilderOpening.Append(product);
                        strBuilderOpening.Append(System.Environment.NewLine);
                        strBuilderOpening.Append(linedraw);
                        strBuilderOpening.Append(System.Environment.NewLine);
                    }

                    strBuilderOpening.Append(imei.Substring(product.Length + 1) + ", ");
                    productcounter++;


                }
                strBuilderOpening.Append(linedraw);
                strBuilderOpening.Append(System.Environment.NewLine);
                strBuilderOpening.Append("             Total: " + OpeningCount);
                #endregion

                #region Taken
                tempProduct = string.Empty;
                productcounter = 0;
                string[] taken_imeno = item.taken_imeno.Split(new char[] { '\r' }, StringSplitOptions.RemoveEmptyEntries);
                TakenCount = taken_imeno.Length;
                TakenCountGrand += TakenCount;
                foreach (var imei in taken_imeno)
                {

                    int len = imei.LastIndexOf('-');
                    if (len < 0)
                        len = 0;
                    string product = imei.Substring(0, len);

                    if (!product.Equals(tempProduct) && tempProduct.Equals(string.Empty))
                    {
                        strBuilderTaken.Append(product);
                        strBuilderTaken.Append(System.Environment.NewLine);
                        strBuilderTaken.Append(linedraw);
                        strBuilderTaken.Append(System.Environment.NewLine);
                        tempProduct = product;
                    }


                    if (!product.Equals(tempProduct))
                    {
                        strBuilderTaken.Append(System.Environment.NewLine);
                        strBuilderTaken.Append(linedraw);
                        strBuilderTaken.Append(System.Environment.NewLine);
                        strBuilderTaken.Append("           SubTotal: " + productcounter);
                        strBuilderTaken.Append(System.Environment.NewLine);
                        productcounter = 0;

                        tempProduct = product;
                        strBuilderTaken.Append(product);
                        strBuilderTaken.Append(System.Environment.NewLine);
                        strBuilderTaken.Append(linedraw);
                        strBuilderTaken.Append(System.Environment.NewLine);
                    }
                    strBuilderTaken.Append(imei.Substring(product.Length + 1) + ", ");
                    productcounter++;


                }
                strBuilderTaken.Append(linedraw);
                strBuilderTaken.Append(System.Environment.NewLine);
                strBuilderTaken.Append("             Total: " + TakenCount);
                #endregion

                #region Balance
                tempProduct = string.Empty;
                productcounter = 0;
                string[] imeno_balance = item.imeno_balance.Split(new char[] { '\r' }, StringSplitOptions.RemoveEmptyEntries);
                BalanceCount = imeno_balance.Length;
                BalanceCountGrand += BalanceCount;
                foreach (var imei in imeno_balance)
                {
                    int len = imei.LastIndexOf('-');
                    if (len < 0)
                        len = 0;
                    string product = imei.Substring(0, len);

                    if (!product.Equals(tempProduct) && tempProduct.Equals(string.Empty))
                    {
                        strBuilderBalance.Append(product);
                        strBuilderBalance.Append(System.Environment.NewLine);
                        strBuilderBalance.Append(linedraw);
                        strBuilderBalance.Append(System.Environment.NewLine);
                        tempProduct = product;
                    }


                    if (!product.Equals(tempProduct))
                    {
                        strBuilderBalance.Append(System.Environment.NewLine);
                        strBuilderBalance.Append(linedraw);
                        strBuilderBalance.Append(System.Environment.NewLine);
                        strBuilderBalance.Append("           SubTotal: " + productcounter);
                        strBuilderBalance.Append(System.Environment.NewLine);
                        productcounter = 0;

                        tempProduct = product;
                        strBuilderBalance.Append(product);
                        strBuilderBalance.Append(System.Environment.NewLine);
                        strBuilderBalance.Append(linedraw);
                        strBuilderBalance.Append(System.Environment.NewLine);
                    }

                    strBuilderBalance.Append(imei.Substring(product.Length + 1) + ", ");
                    productcounter++;

                }
                strBuilderBalance.Append(linedraw);
                strBuilderBalance.Append(System.Environment.NewLine);
                strBuilderBalance.Append("             Total: " + BalanceCount);
                #endregion

                dt.Rows.Add(item.ConcernID, item.EmployeeId, item.EmployeeName, item.TransDate, item.OpeningQty, strBuilderOpening, item.Opening_productno, item.taken_qty, strBuilderTaken, item.taken_product, item.Total_qty, item.sale_qty, strBuilderSalesIMEI, item.sale_product, item.balance_qty, strBuilderBalance, item.product_balance);

                strBuilderBalance.Clear();
                strBuilderOpening.Clear();
                strBuilderSalesIMEI.Clear();
                strBuilderTaken.Clear();
            }

            dt.TableName = "dtSRVisitReport";
            _dataSet.Tables.Add(dt);

            GetCommonParameters(userName, concernID);
            var SR = SRVisitData.FirstOrDefault();
            string SRName = string.Empty;
            if (SR != null)
            {
                SRName = SR.EmployeeName;
            }
            _reportParameter = new ReportParameter("SRName", SRName);
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("DateRange", "SR visit Report from date: " + fromDate.ToString("dd MMM yyyy") + " to " + toDate.ToString("dd MMM yyyy"));
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("OpeningGrandCount", OpeningGrandCount.ToString());
            _reportParameters.Add(_reportParameter);
            _reportParameter = new ReportParameter("TakenCountGrand", TakenCountGrand.ToString());
            _reportParameters.Add(_reportParameter);
            _reportParameter = new ReportParameter("SalesCountGrand", SalesCountGrand.ToString());
            _reportParameters.Add(_reportParameter);
            _reportParameter = new ReportParameter("BalanceCountGrand", BalanceCountGrand.ToString());
            _reportParameters.Add(_reportParameter);
            _reportParameter = new ReportParameter("PrintDate", "Date:" + GetClientDateTime());
            _reportParameters.Add(_reportParameter);

            return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "SR\\rptSRVisitReportWithIMEI.rdlc");
        }

        /// <summary>
        /// Author:aminul
        /// Date: 20-Mar-2018
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="userName"></param>
        /// <param name="concernID"></param>
        /// <param name="EmployeeID"></param>
        /// <returns></returns>
        public byte[] SRWiseCustomerStatusReport(DateTime fromDate, DateTime toDate, string userName, int concernID, int EmployeeID)
        {
            var reportData = _SRVisitService.SRWiseCustomerStatusReport(fromDate, toDate, concernID, EmployeeID);
            TransactionalDataSet.dtSRWiseCustomerStatusReportDataTable dt = new TransactionalDataSet.dtSRWiseCustomerStatusReportDataTable();
            _dataSet = new DataSet();
            decimal netsales = 0, expenseAmount = 0, NetExpenseAmt = 0;
            int employeeID = 0;
            reportData.OrderBy(i => i.EmployeeID);
            foreach (var item in reportData)
            {
                netsales = item.SlaesAmount - item.ReturnAmount;
                expenseAmount = _expenditureService.GetExpenditureAmountByUserID(_userService.GetUserIDByEmployeeID(item.EmployeeID), fromDate, toDate);
                if (employeeID != item.EmployeeID)
                {
                    NetExpenseAmt += expenseAmount;
                    employeeID = item.EmployeeID;
                }
                dt.Rows.Add(item.ConcernID, item.EmployeeID, item.EmployeeName, item.CustomerID, item.Code, item.Name, (item.Address + ", " + item.ContactNo), item.Address, item.Quantity, item.OpeningDue, item.SlaesAmount, item.ReturnAmount, netsales, item.Collection, item.ClosingAmount, expenseAmount);
            }

            dt.TableName = "dtSRWiseCustomerStatusReport";
            _dataSet.Tables.Add(dt);

            GetCommonParameters(userName, concernID);
            _reportParameter = new ReportParameter("SRName", reportData.Count() == 0 ? "" : reportData.FirstOrDefault().EmployeeName);
            _reportParameters.Add(_reportParameter);
            _reportParameter = new ReportParameter("NetExpenseAmt", NetExpenseAmt.ToString());
            _reportParameters.Add(_reportParameter);
            _reportParameter = new ReportParameter("DateRange", "SR wise customer status Report Date from: " + fromDate.ToString("dd MMM yyyy") + " to " + toDate.ToString("dd MMM yyyy"));
            _reportParameters.Add(_reportParameter);

            return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "SR\\rptSRWiseCustomerStatusReport.rdlc");
        }


        public byte[] ReplacementReport(DateTime fromDate, DateTime toDate, string userName, int concernID, int CustomerID)
        {
            var reportData = _salesOrderService.ReplacementOrderReport(fromDate, toDate, concernID, CustomerID);
            TransactionalDataSet.dtReplacementOrderReportDataTable dt = new TransactionalDataSet.dtReplacementOrderReportDataTable();
            _dataSet = new DataSet();
            foreach (var item in reportData)
            {
                dt.Rows.Add(item.SOrderID, item.SalesDate, item.Invoice, item.ReturnDate, item.ReturnInvoice, item.CustomerCode, item.CustomerName, (item.CustomerAddress + " & " + item.CustomerMobile), item.CustomerMobile, item.DamageProudct, item.DamageIMEI, item.DamageQty, item.DamageSalesRate, item.ReplaceProduct, item.ReplaceIMEI, item.ReplaceQty, item.ReplaceRate, item.Remarks, item.PODate);
            }

            dt.TableName = "dtReplacementOrderReport";
            _dataSet.Tables.Add(dt);

            GetCommonParameters(userName, concernID);

            _reportParameter = new ReportParameter("DateRange", "Replacement report from date: " + fromDate.ToString("dd MMM yyyy") + " to " + toDate.ToString("dd MMM yyyy"));
            _reportParameters.Add(_reportParameter);

            return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Sales\\rptReplacementOrderReport.rdlc");
        }


        public byte[] ReturntReport(DateTime fromDate, DateTime toDate, string userName, int concernID, int CustomerID)
        {
            var reportData = _salesOrderService.ReturnOrderReport(fromDate, toDate, concernID, CustomerID);
            TransactionalDataSet.dtReturnOrderReportDataTable dt = new TransactionalDataSet.dtReturnOrderReportDataTable();
            _dataSet = new DataSet();
            foreach (var item in reportData)
            {
                dt.Rows.Add(item.ReturnDate, item.ReturnInvoice, item.CustomerCode, item.CustomerName, (item.CustomerAddress + " & " + item.CustomerMobile), item.CustomerMobile, item.Remarks, item.ProductName, item.ReturnIMEI, item.ReturnQty, item.ReturnAmount);
            }

            dt.TableName = "dtReturnOrderReport";
            _dataSet.Tables.Add(dt);

            GetCommonParameters(userName, concernID);

            _reportParameter = new ReportParameter("DateRange", "Return report from date: " + fromDate.ToString("dd MMM yyyy") + " to " + toDate.ToString("dd MMM yyyy"));
            _reportParameters.Add(_reportParameter);

            return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Sales\\rptReturnOrderReport.rdlc");
        }


        private byte[] CashCollectionMoneyReceiptPrint(CashCollection cashCollection, string userName, int concernID)
        {
            var Customer = _customerService.GetCustomerById((int)cashCollection.CustomerID);
            var Sales = _salesOrderService.GetLastSalesOrderByCustomerID((int)cashCollection.CustomerID);

            var TotalAdjustment = cashCollection.CashBAmt + cashCollection.YearlyBnsAmt + cashCollection.AdjustAmt;
            _dataSet = new DataSet();
            //dt.TableName = "dtReturnOrderReport";
            //_dataSet.Tables.Add(dt);
            GetCommonParameters(userName, concernID);

            _reportParameter = new ReportParameter("ReceiptNo", cashCollection.ReceiptNo);
            _reportParameters.Add(_reportParameter);
            string sInwodTk = TakaFormat(Convert.ToDouble(cashCollection.Amount.ToString()));
            sInwodTk = sInwodTk.Replace("Taka", "");
            sInwodTk = sInwodTk.Replace("Only", "Taka Only");
            //_SOrder.RecAmount.ToString()
            _reportParameter = new ReportParameter("ReceiptTK", cashCollection.Amount.ToString());
            _reportParameters.Add(_reportParameter);
            //_SOrder.InvoiceDate.ToString()
            _reportParameter = new ReportParameter("ReceiptDate", cashCollection.EntryDate.ToString());
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("LastSalesDate", Sales != null ? Sales.InvoiceDate.ToString("dd MMM yyyy") : "");
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("Name", Customer.Name);
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("BalanceDue", (Customer.TotalDue).ToString());
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("CusAddress", Customer.Address);
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("CustomerContactNo", Customer.ContactNo);
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("InWordTK", sInwodTk);
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("Adjustment", TotalAdjustment.ToString("F"));
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("InvNo", cashCollection.InvoiceNo);
            _reportParameters.Add(_reportParameter);

            //if (concernID == (int)EnumSisterConcern.SAMSUNG_ELECTRA_CONCERNID)
            return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Sales\\AMMoneyReceiptSS.rdlc");
            //else if (concernID == (int)EnumSisterConcern.KINGSTAR_CONCERNID)
            //    return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Sales\\rptKMoneyReceipt.rdlc");
            //else
            //    return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Sales\\AMMoneyReceipt.rdlc");

        }
        public byte[] CashDeliveryMoneyReceiptPrint(int CashDeliveryID, string userName, int concernID)
        {
            var cashCollection = _CashCollectionService.GetCashCollectionById(CashDeliveryID);
            var Supplier = _SupplierService.GetSupplierById((int)cashCollection.SupplierID);
            var POrder = _purchaseOrderService.GetAllIQueryable().Where(i => i.SupplierID == (int)cashCollection.SupplierID).OrderByDescending(i => i.OrderDate).FirstOrDefault();
            _dataSet = new DataSet();
            //dt.TableName = "dtReturnOrderReport";
            //_dataSet.Tables.Add(dt);
            GetCommonParameters(userName, concernID);

            _reportParameter = new ReportParameter("ReceiptNo", cashCollection.ReceiptNo);
            _reportParameters.Add(_reportParameter);
            string sInwodTk = TakaFormat(Convert.ToDouble(cashCollection.Amount.ToString()));
            sInwodTk = sInwodTk.Replace("Taka", "");
            sInwodTk = sInwodTk.Replace("Only", "Taka Only");
            //_SOrder.RecAmount.ToString()
            _reportParameter = new ReportParameter("ReceiptTK", cashCollection.Amount.ToString());
            _reportParameters.Add(_reportParameter);
            //_SOrder.InvoiceDate.ToString()
            _reportParameter = new ReportParameter("ReceiptDate", cashCollection.EntryDate.ToString());
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("LastSalesDate", POrder != null ? POrder.OrderDate.ToString("dd MMM yyyy") : "");
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("Name", Supplier.Name);
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("BalanceDue", (Supplier.TotalDue).ToString());
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("CusAddress", Supplier.Address);
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("CustomerContactNo", Supplier.ContactNo);
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("Remarks", cashCollection.Remarks);
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("InWordTK", sInwodTk);
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("Adjustment", cashCollection.AdjustAmt.ToString("F"));
            _reportParameters.Add(_reportParameter);

            //if (concernID == (int)EnumSisterConcern.SAMSUNG_ELECTRA_CONCERNID)
            return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "CashDelivery\\CDMoneyReceiptSS.rdlc");
            //else if (concernID == (int)EnumSisterConcern.KINGSTAR_CONCERNID)
            //    return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "CashDelivery\\CDKMoneyReceipt.rdlc");
            //else
            //    return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "CashDelivery\\CDMoneyReceipt.rdlc");

        }
        public byte[] CashCollectionMoneyReceipt(CashCollection cashCollection, string userName, int concernID)
        {
            return CashCollectionMoneyReceiptPrint(cashCollection, userName, concernID);
        }
        public byte[] CashCollectionMoneyReceiptByID(int CashCollectionID, string userName, int concernID)
        {
            var cashCollection = _CashCollectionService.GetCashCollectionById(CashCollectionID);
            return CashCollectionMoneyReceiptPrint(cashCollection, userName, concernID);
        }
        private byte[] CrditSalesMoneyReceiptShow(CreditSale CreditSale, List<CreditSaleDetails> details, CreditSalesSchedule schedules, string userName, int concernID)
        {
            if (schedules == null)
                schedules = new CreditSalesSchedule();
            var Customer = _customerService.GetCustomerById(CreditSale.CustomerID);
            _dataSet = new DataSet();
            GetCommonParameters(userName, concernID);
            _reportParameter = new ReportParameter("CusCode", Customer.Code);
            _reportParameters.Add(_reportParameter);

            string sInwodTk = TakaFormat(Convert.ToDouble(schedules.InstallmentAmt));

            _reportParameter = new ReportParameter("CusName", Customer.Name);
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("CustomerContact", Customer.ContactNo);
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("CustomerAddress", Customer.Address);
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("InvoiceNo", CreditSale.InvoiceNo);
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("TSalesAmt", sInwodTk);
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("Remaining", CreditSale.Remaining.ToString("F"));
            _reportParameters.Add(_reportParameter);
            if (CreditSale.DownPayment != 0m)
                sInwodTk = TakaFormat(Convert.ToDouble(CreditSale.DownPayment.ToString()));
            else
                sInwodTk = TakaFormat(Convert.ToDouble(schedules.InstallmentAmt.ToString()));

            sInwodTk = sInwodTk.Replace("Taka", "");
            sInwodTk = sInwodTk.Replace("Only", "Taka Only");
            _reportParameter = new ReportParameter("InWordTK", sInwodTk);
            _reportParameters.Add(_reportParameter);

            if (CreditSale.DownPayment != 0m)
                _reportParameter = new ReportParameter("InstallmentOrDownPayment", CreditSale.DownPayment.ToString("F"));
            else
                _reportParameter = new ReportParameter("InstallmentOrDownPayment", schedules.InstallmentAmt.ToString("F"));

            _reportParameters.Add(_reportParameter);
            _reportParameter = new ReportParameter("PaymentDate", schedules.PaymentDate.ToString("dd MMM yyyy"));
            _reportParameters.Add(_reportParameter);

            sInwodTk = TakaFormat(Convert.ToDouble((schedules.InstallmentAmt).ToString()));
            sInwodTk = sInwodTk.Replace("Taka", "");
            sInwodTk = sInwodTk.Replace("Only", "Taka Only");

            _reportParameter = new ReportParameter("TReceiveAmt", (CreditSale.NetAmount + CreditSale.PenaltyInterest - CreditSale.Remaining).ToString("F"));
            _reportParameters.Add(_reportParameter);


            _reportParameter = new ReportParameter("SalesDate", CreditSale.IssueDate.ToString("dd MMM yyy"));
            _reportParameters.Add(_reportParameter);

            string SModels = "";
            Product objProduct = null;
            foreach (var oSItem in details)
            {
                objProduct = _productService.GetProductById(oSItem.ProductID);
                if (SModels == "")
                {
                    SModels = objProduct.ProductName;
                }
                else
                {
                    SModels = SModels + "," + objProduct.ProductName;
                }
            }

            _reportParameter = new ReportParameter("PModels", SModels);
            _reportParameters.Add(_reportParameter);
            _reportParameter = new ReportParameter("Adjustment", CreditSale.LastPayAdjAmt.ToString("F"));
            _reportParameters.Add(_reportParameter);

            if (concernID == (int)EnumSisterConcern.SAMSUNG_ELECTRA_CONCERNID)
                return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "CreditSales\\SSCreditMoneyReceipt.rdlc");
            else if (concernID == (int)EnumSisterConcern.KINGSTAR_CONCERNID)
                return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "CreditSales\\KCreditMoneyReceipt.rdlc");
            else
                return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "CreditSales\\CreditMoneyReceipt.rdlc");

        }

        public byte[] CrditSalesMoneyReceipt(CreditSale CreditSale, List<CreditSaleDetails> details, CreditSalesSchedule schedules, string userName, int concernID)
        {
            return CrditSalesMoneyReceiptShow(CreditSale, details, schedules, userName, concernID);
        }

        public byte[] CrditSalesMoneyReceiptByID(int CreditSalesID, string userName, int concernID)
        {
            var CreditSale = _creditSalesOrderService.GetSalesOrderById(CreditSalesID);

            var details = _creditSalesOrderService.GetSalesOrderDetails(CreditSalesID).ToList();
            var schedules = _creditSalesOrderService.GetSalesOrderSchedules(CreditSalesID).Where(i => i.PaymentStatus == "Paid" && i.InstallmentAmt != 0m).OrderByDescending(i => i.CSScheduleID).FirstOrDefault();
            if (schedules != null)
                CreditSale.DownPayment = 0m;
            return CrditSalesMoneyReceiptShow(CreditSale, details, schedules, userName, concernID);
        }

        public byte[] MonthlyBenefit(DateTime fromDate, DateTime toDate, string userName, int concernID)
        {
            var Data = _salesOrderService.MonthlyBenefitReport(fromDate, toDate, concernID);
            _dataSet = new DataSet();
            TransactionalDataSet.dtMonthlyBenefitReportDataTable dt = new TransactionalDataSet.dtMonthlyBenefitReportDataTable();

            foreach (var it in Data)
            {
                dt.Rows.Add(it.InvoiceDate,
                    it.SalesTotal + it.CreditSalesTotal - it.TDAmount_Sale - it.TDAmount_CreditSale,
                    it.PurchaseTotal + it.CreditPurchase,
                    it.TDAmount_Sale,
                    it.TDAmount_CreditSale,
                    it.FirstTotalInterest,
                    it.HireCollection,
                    it.CreditSalesTotal,
                    it.CreditPurchase,
                    it.CommisionProfit,
                    it.HireProfit,
                    it.TotalProfit,
                    it.OthersIncome,
                    it.TotalIncome,
                    it.Adjustment,
                    it.LastPayAdjustment,
                    it.TotalExpense,
                    it.Benefit);
            }


            dt.TableName = "dtMonthlyBenefitReport";
            _dataSet.Tables.Add(dt);

            GetCommonParameters(userName, concernID);

            _reportParameter = new ReportParameter("DateRange", "Monthly Benefit Report From Month: " + fromDate.ToString("MMMM-yyyy") + " to " + toDate.ToString("MMMM-yyyy"));
            _reportParameters.Add(_reportParameter);

            if (concernID == (int)EnumSisterConcern.NOKIA_CONCERNID || concernID == (int)EnumSisterConcern.WALTON_CONCERNID || concernID == (int)EnumSisterConcern.KINGSTAR_CONCERNID || concernID == (int)EnumSisterConcern.NOKIA_STORE_MAGURA_CONCERNID)
                return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Sales\\MonthlyBenefitRptMobile.rdlc");

            return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Sales\\MonthlyBenefitRpt.rdlc");


        }


        public byte[] ProductWiseBenefitReport(DateTime fromDate, DateTime toDate, int ProductID, string userName, int concernID)
        {
            var Data = _salesOrderService.ProductWiseBenefitReport(fromDate, toDate, concernID);
            _dataSet = new DataSet();
            TransactionalDataSet.dtBenefitRptDataTable dt = new TransactionalDataSet.dtBenefitRptDataTable();
            if (ProductID != 0)
                Data = Data.Where(i => i.ProductID == ProductID).ToList();

            foreach (var item in Data)
            {
                dt.Rows.Add(item.Code, item.ProductName, item.CategoryName, item.IMENO, item.SalesTotal, item.Discount, item.NetSales, item.PurchaseTotal, item.CommisionProfit, item.HireProfit, item.HireCollection, item.TotalProfit);
            }


            dt.TableName = "dtBenefitRpt";
            _dataSet.Tables.Add(dt);

            GetCommonParameters(userName, concernID);

            _reportParameter = new ReportParameter("Month", "Product Wise Benefit Report From Date: " + fromDate.ToString("dd MMM yyyy") + " to " + toDate.ToString("dd MMM yyyy"));
            _reportParameters.Add(_reportParameter);

            if (concernID == (int)EnumSisterConcern.NOKIA_CONCERNID || concernID == (int)EnumSisterConcern.WALTON_CONCERNID || concernID == (int)EnumSisterConcern.KINGSTAR_CONCERNID || concernID == (int)EnumSisterConcern.NOKIA_STORE_MAGURA_CONCERNID)
                return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Sales\\BenefitRptMobile.rdlc");

            return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Sales\\BenefitRpt.rdlc");
        }


        public byte[] ProductWiseSalesReport(DateTime fromDate, DateTime toDate, int CustomerID, string userName, int concernID)
        {
            var Data = _salesOrderService.ProductWiseSalesReport(fromDate, toDate, concernID, CustomerID);

            var ReturnData = _returnOrderService.ProductWiseReturnReport(fromDate, toDate, concernID, CustomerID);



            _dataSet = new DataSet();

            TransactionalDataSet.dtProductWiseSalesDataTable dt = new TransactionalDataSet.dtProductWiseSalesDataTable();
            TransactionalDataSet.dtProductWiseReturnDataTable dtReturn = new TransactionalDataSet.dtProductWiseReturnDataTable();


            var CreditSalesData = _creditSalesOrderService.ProductWiseCreditSalesReport(fromDate, toDate, concernID, CustomerID);

            foreach (var item in Data)
            {
                dt.Rows.Add(item.SOrderID, item.Date, item.EmployeeCode, item.EmployeeName, item.CustomerCode, item.CustomerName, (item.Mobile + " & " + item.Address), item.Mobile, item.ProductName, item.Quantity, item.SalesRate, item.TotalAmount, "Sales");
            }

            foreach (var item in CreditSalesData)
            {
                dt.Rows.Add(item.SOrderID, item.Date, item.EmployeeCode, item.EmployeeName, item.CustomerCode, item.CustomerName, (item.Mobile + " & " + item.Address), item.Mobile, item.ProductName, item.Quantity, item.SalesRate, item.TotalAmount, "Credit Sales");
            }


            foreach (var item in ReturnData)
            {
                dtReturn.Rows.Add(item.SOrderID, item.Date, item.EmployeeCode, item.EmployeeName, item.CustomerCode, item.CustomerName, (item.Mobile + " & " + item.Address), item.Mobile, item.ProductName, item.Quantity, item.SalesRate, item.TotalAmount, "Sales");
            }


            decimal TotalSalesQuantity = Data.Count() != 0 ? Data.Sum(o => o.Quantity) : 0m;
            decimal TotalReturnQuantity = Data.Count() != 0 ? Data.Sum(o => o.Quantity) : 0m;


            decimal TotalSalesPrice = ReturnData.Count() != 0 ? ReturnData.Sum(o => o.TotalAmount) : 0m;
            decimal TotalReturnPrice = ReturnData.Count() != 0 ? ReturnData.Sum(o => o.TotalAmount) : 0m;




            decimal NetQuantity = TotalSalesQuantity - TotalReturnQuantity;
            decimal NetPrice = TotalSalesPrice - TotalReturnPrice;

            dt.TableName = "dtProductWiseSales";
            _dataSet.Tables.Add(dt);
            dtReturn.TableName = "dtProductWiseReturn";
            _dataSet.Tables.Add(dtReturn);

            GetCommonParameters(userName, concernID);

            _reportParameter = new ReportParameter("DateRange", "Product Wise Sales Report From Date: " + fromDate.ToString("dd MMM yyyy") + " to " + toDate.ToString("dd MMM yyyy"));
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("NetQuantity", NetQuantity.ToString("0.00"));
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("NetPrice", NetPrice.ToString("0.00"));
            _reportParameters.Add(_reportParameter);

            return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Sales\\ProductWiseSalesRpt.rdlc");

        }

        public byte[] ProductWisePurchaseReport(DateTime fromDate, DateTime toDate, int SupplierID, string userName, int concernID, EnumPurchaseType PurchaseType)
        {
            var Data = _purchaseOrderService.ProductWisePurchaseReport(fromDate, toDate, concernID, SupplierID, PurchaseType);
            _dataSet = new DataSet();
            TransactionalDataSet.dtProductWiseSalesDataTable dt = new TransactionalDataSet.dtProductWiseSalesDataTable();

            foreach (var item in Data)
            {
                dt.Rows.Add(0, item.Date, item.SupplierCode, item.SupplierName, "", "", (item.Mobile + " & " + item.Address), item.Mobile, item.ProductName, item.Quantity, item.PurchaseRate, item.TotalAmount);
            }


            dt.TableName = "dtProductWiseSales";
            _dataSet.Tables.Add(dt);

            GetCommonParameters(userName, concernID);

            _reportParameter = new ReportParameter("DateRange", "Product Wise Purchase Report From Date: " + fromDate.ToString("dd MMM yyyy") + " to " + toDate.ToString("dd MMM yyyy"));
            _reportParameters.Add(_reportParameter);

            return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Purchase\\ProductWisePurchaseRpt.rdlc");

        }

        public byte[] DamageProductReport(DateTime fromDate, DateTime toDate, string userName, int concernID, int CustomerID)
        {
            var reportData = _salesOrderService.DamageProductReport(fromDate, toDate, concernID, CustomerID);
            TransactionalDataSet.dtReplacementOrderReportDataTable dt = new TransactionalDataSet.dtReplacementOrderReportDataTable();
            _dataSet = new DataSet();
            foreach (var item in reportData)
            {
                dt.Rows.Add(item.SOrderID, item.SalesDate, item.Invoice, item.ReturnDate, item.ReturnInvoice, item.CustomerCode, item.CustomerName, (item.CustomerAddress + " & " + item.CustomerMobile), item.CustomerMobile, item.DamageProudct, item.DamageIMEI, item.DamageQty, item.DamageSalesRate, item.ReplaceProduct, item.ReplaceIMEI, item.ReplaceQty, item.ReplaceRate, item.Remarks);
            }

            dt.TableName = "dtReplacementOrderReport";
            _dataSet.Tables.Add(dt);

            GetCommonParameters(userName, concernID);

            _reportParameter = new ReportParameter("DateRange", "Damage Product report from date: " + fromDate.ToString("dd MMM yyyy") + " to " + toDate.ToString("dd MMM yyyy"));
            _reportParameters.Add(_reportParameter);

            return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Sales\\rptDamageProductReport.rdlc");
        }

        /// <summary>
        /// Date: 16-May-2018
        /// </summary>
        public byte[] SRWiseCashCollectionReport(DateTime fromDate, DateTime toDate, string userName, int concernID, int EmployeeID)
        {
            try
            {
                TransactionalDataSet.dtCollectionRptDataTable dt = new TransactionalDataSet.dtCollectionRptDataTable();
                _dataSet = new DataSet();

                //Cash and Bank Transactions
                var CashCollectionInfos = _CashCollectionService.GetSRWiseCashCollectionReportData(fromDate, toDate, concernID, EmployeeID);

                //Receive Amount
                var SalesOrders = _salesOrderService.GetforSalesReport(fromDate, toDate, EmployeeID, 0);

                foreach (var item in SalesOrders)
                {
                    dt.Rows.Add(item.InvoiceDate, item.CustomerName, item.CustomerAddress, item.CustomerContactNo, item.CustomerTotalDue, item.RecAmount, 0m, 0m, "Cash Sales", "", "", "", "", item.EmployeeName, item.InvoiceNo);
                }
                foreach (var grd in CashCollectionInfos)
                {
                    dt.Rows.Add(grd.Item1.ToString("dd MMM yyyy"), grd.Item2, grd.Item4 + " & " + grd.Item3, grd.Item4, grd.Item5, grd.Item6, grd.Item7, grd.Rest.Item1, grd.Rest.Item2, grd.Rest.Item3, grd.Rest.Item4, grd.Rest.Item5, grd.Rest.Item6, grd.Rest.Item7);
                }

                if (concernID == (int)EnumSisterConcern.SAMSUNG_ELECTRA_CONCERNID || concernID == (int)EnumSisterConcern.HAWRA_ENTERPRISE_CONCERNID || concernID == (int)EnumSisterConcern.HAVEN_ENTERPRISE_CONCERNID)
                {
                    //Downpayment and Installemnt Collections
                    var CreditSales = _creditSalesOrderService.SRWiseCreditSalesReport(EmployeeID, fromDate, toDate);
                    foreach (var item in CreditSales)
                    {
                        dt.Rows.Add(item.InvoiceDate, item.CustomerName, item.CustomerAddress, item.CustomerContactNo, item.CustomerTotalDue, item.RecAmount, 0m, 0m, "Cr. Sales", "", "", "", "", item.EmployeeName, item.InvoiceNo);
                    }
                }

                dt.TableName = "dtCollectionRpt";
                _dataSet.Tables.Add(dt);

                GetCommonParameters(userName, concernID);

                _reportParameter = new ReportParameter("Month", "SR Wise Cash Collection report for the date of : " + fromDate.ToString("dd MMM yyyy") + " to " + toDate.ToString("dd MMM yyyy"));
                _reportParameters.Add(_reportParameter);

                return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Others\\rptSRWiseCashCollection.rdlc");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public byte[] ProductwiseSalesDetails(string userName, int concernID, int reportType, int CompanyID, int CategoryID, int ProductID, DateTime fromDate, DateTime toDate)
        {
            try
            {
                var Data = _salesOrderService.ProductWiseSalesDetailsReport(CompanyID, CategoryID, ProductID, fromDate, toDate);
                var CreditData = _creditSalesOrderService.ProductWiseCreditSalesDetailsReport(CompanyID, CategoryID, ProductID, fromDate, toDate);

                var ReturnData = _returnOrderService.ProductWiseReturnDetailsReport(CompanyID, CategoryID, ProductID, fromDate, toDate);

                TransactionalDataSet.PWSDetailsDataTable dt = new TransactionalDataSet.PWSDetailsDataTable();

                TransactionalDataSet.PWSDetailsDataTable dtReturn = new TransactionalDataSet.PWSDetailsDataTable();


                _dataSet = new DataSet();


                string InvoiceNo = string.Empty;

                foreach (var item in Data)
                {

                    dt.Rows.Add(item.Date, item.InvoiceNo, item.CompanyName, item.CategoryName, item.ProductName, item.Quantity, item.SalesRate, item.TotalAmount, item.IMEI);


                }
                foreach (var item in CreditData)
                {
                    dt.Rows.Add(item.Date, item.InvoiceNo, item.CompanyName, item.CategoryName, item.ProductName, item.Quantity, item.SalesRate, item.TotalAmount, item.IMEI);
                }





                foreach (var item in ReturnData)
                {
                    dtReturn.Rows.Add(item.Date, item.InvoiceNo, item.CompanyName, item.CategoryName, item.ProductName, item.Quantity, item.SalesRate, item.TotalAmount, item.IMEI);
                }





                decimal TotalSalesQuantity = Data.Count() != 0 ? Data.Sum(o => o.Quantity) : 0m;
                decimal TotalReturnQuantity = Data.Count() != 0 ? Data.Sum(o => o.Quantity) : 0m;

                decimal CreiditSales = CreditData.Count() != 0 ? CreditData.Sum(o => o.Quantity) : 0m;
                decimal CreditQuantity = CreditData.Count() != 0 ? CreditData.Sum(o => o.Quantity) : 0m;


                decimal TotalSalesPrice = ReturnData.Count() != 0 ? ReturnData.Sum(o => o.TotalAmount) : 0m;
                decimal TotalReturnPrice = ReturnData.Count() != 0 ? ReturnData.Sum(o => o.TotalAmount) : 0m;


                TotalSalesQuantity = TotalSalesQuantity + CreditQuantity;
                TotalSalesPrice = TotalSalesPrice + CreiditSales;

                decimal NetQuantity = TotalSalesQuantity - TotalReturnQuantity;
                decimal NetPrice = TotalSalesPrice - TotalReturnPrice;


                dt.TableName = "PWSDetails";
                _dataSet.Tables.Add(dt);

                dtReturn.TableName = "PWRDetails";
                _dataSet.Tables.Add(dtReturn);

                GetCommonParameters(userName, concernID);

                _reportParameter = new ReportParameter("PrintDate", GetClientDateTime());
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("NetQuantity", NetQuantity.ToString("0.00"));
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("NetPrice", NetPrice.ToString("0.00"));
                _reportParameters.Add(_reportParameter);

                if (reportType == 0)
                {
                    _reportParameter = new ReportParameter("DateRange", "Product Wise Sales Date From : " + fromDate.ToString("dd MMM yyyy") + " to " + toDate.ToString("dd MMM yyyy"));
                    _reportParameters.Add(_reportParameter);
                    return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Sales\\rptPWSalesDetails.rdlc");
                }
                else if (reportType == 1)
                {
                    _reportParameter = new ReportParameter("DateRange", "Company Wise Sales Date From : " + fromDate.ToString("dd MMM yyyy") + " to " + toDate.ToString("dd MMM yyyy"));
                    _reportParameters.Add(_reportParameter);
                    return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Sales\\rptCompanyWiseSDetails.rdlc");
                }
                else
                {
                    _reportParameter = new ReportParameter("DateRange", "Category Wise Sales Date From : " + fromDate.ToString("dd MMM yyyy") + " to " + toDate.ToString("dd MMM yyyy"));
                    _reportParameters.Add(_reportParameter);
                    return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Sales\\rptCategoryWSalesDetails.rdlc");
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public byte[] ProductwiseSalesSummary(string userName, int concernID, int reportType, int CompanyID, int CategoryID, int ProductID, DateTime fromDate, DateTime toDate)
        {
            try
            {
                var Data = _salesOrderService.ProductWiseSalesDetailsReport(CompanyID, CategoryID, ProductID, fromDate, toDate);
                var CreditData = _creditSalesOrderService.ProductWiseCreditSalesDetailsReport(CompanyID, CategoryID, ProductID, fromDate, toDate);
                var ReturnData = _returnOrderService.ProductWiseReturnDetailsReport(CompanyID, CategoryID, ProductID, fromDate, toDate);
                TransactionalDataSet.PWSDetailsDataTable dt = new TransactionalDataSet.PWSDetailsDataTable();
                TransactionalDataSet.PWSDetailsDataTable dtReturn = new TransactionalDataSet.PWSDetailsDataTable();
                _dataSet = new DataSet();
                List<ProductWiseSalesReportModel> AllSales = new List<ProductWiseSalesReportModel>();


                List<ProductWiseSalesReportModel> AllReturns = new List<ProductWiseSalesReportModel>();
                if (reportType == 0) //Product Wise
                {


                    var sales = from s in Data
                                group s by new { s.Date, s.ProductID, s.ProductName, s.CategoryID, s.CategoryName, s.CompanyID, s.CompanyName, s.SalesRate } into g
                                select new ProductWiseSalesReportModel
                                {
                                    ProductID = g.Key.ProductID,
                                    ProductName = g.Key.ProductName,
                                    CategoryID = g.Key.CategoryID,
                                    CategoryName = g.Key.CategoryName,
                                    CompanyID = g.Key.CompanyID,
                                    CompanyName = g.Key.CompanyName,
                                    Date = g.Key.Date,
                                    Quantity = g.Sum(i => i.Quantity),
                                    SalesRate = g.Key.SalesRate,
                                    TotalAmount = g.Sum(i => i.Quantity) * g.Key.SalesRate
                                };
                    AllSales.AddRange(sales);

                    var Creditsales = from s in CreditData
                                      group s by new { s.Date, s.ProductID, s.ProductName, s.CategoryID, s.CategoryName, s.CompanyID, s.CompanyName, s.SalesRate } into g
                                      select new ProductWiseSalesReportModel
                                      {
                                          ProductID = g.Key.ProductID,
                                          ProductName = g.Key.ProductName,
                                          CategoryID = g.Key.CategoryID,
                                          CategoryName = g.Key.CategoryName,
                                          CompanyID = g.Key.CompanyID,
                                          CompanyName = g.Key.CompanyName,
                                          Date = g.Key.Date,
                                          Quantity = g.Sum(i => i.Quantity),
                                          SalesRate = g.Key.SalesRate,
                                          TotalAmount = g.Sum(i => i.Quantity) * g.Key.SalesRate
                                      };
                    AllSales.AddRange(Creditsales);



                    var returns = from s in ReturnData
                                  group s by new { s.Date, s.ProductID, s.ProductName, s.CategoryID, s.CategoryName, s.CompanyID, s.CompanyName, s.SalesRate } into g
                                  select new ProductWiseSalesReportModel
                                  {
                                      ProductID = g.Key.ProductID,
                                      ProductName = g.Key.ProductName,
                                      CategoryID = g.Key.CategoryID,
                                      CategoryName = g.Key.CategoryName,
                                      CompanyID = g.Key.CompanyID,
                                      CompanyName = g.Key.CompanyName,
                                      Date = g.Key.Date,
                                      Quantity = g.Sum(i => i.Quantity),
                                      SalesRate = g.Key.SalesRate,
                                      TotalAmount = g.Sum(i => i.Quantity) * g.Key.SalesRate
                                  };
                    AllReturns.AddRange(returns);

                }
                else if (reportType == 1) // Company Wise
                {

                    var sales = from s in Data
                                group s by new { s.Date, s.CompanyID, s.CompanyName } into g
                                select new ProductWiseSalesReportModel
                                {
                                    CompanyID = g.Key.CompanyID,
                                    CompanyName = g.Key.CompanyName,
                                    Date = g.Key.Date,
                                    Quantity = g.Sum(i => i.Quantity),
                                    TotalAmount = g.Sum(i => i.TotalAmount),
                                };
                    AllSales.AddRange(sales);

                    var Creditsales = from s in CreditData
                                      group s by new { s.Date, s.CompanyID, s.CompanyName } into g
                                      select new ProductWiseSalesReportModel
                                      {
                                          CompanyID = g.Key.CompanyID,
                                          CompanyName = g.Key.CompanyName,
                                          Date = g.Key.Date,
                                          Quantity = g.Sum(i => i.Quantity),
                                          TotalAmount = g.Sum(i => i.TotalAmount),
                                      };



                    AllSales.AddRange(Creditsales);


                    var returns = from s in ReturnData
                                  group s by new { s.Date, s.CompanyID, s.CompanyName } into g
                                  select new ProductWiseSalesReportModel
                                  {
                                      CompanyID = g.Key.CompanyID,
                                      CompanyName = g.Key.CompanyName,
                                      Date = g.Key.Date,
                                      Quantity = g.Sum(i => i.Quantity),
                                      TotalAmount = g.Sum(i => i.TotalAmount),
                                  };
                    AllReturns.AddRange(returns);



                }
                else
                {
                    var sales = from s in Data
                                group s by new { s.Date, s.CategoryID, s.CategoryName } into g
                                select new ProductWiseSalesReportModel
                                {
                                    CategoryID = g.Key.CategoryID,
                                    CategoryName = g.Key.CategoryName,
                                    Date = g.Key.Date,
                                    Quantity = g.Sum(i => i.Quantity),
                                    TotalAmount = g.Sum(i => i.TotalAmount),
                                };
                    AllSales.AddRange(sales);

                    var Creditsales = from s in CreditData
                                      group s by new { s.Date, s.CategoryID, s.CategoryName } into g
                                      select new ProductWiseSalesReportModel
                                      {
                                          CategoryID = g.Key.CategoryID,
                                          CategoryName = g.Key.CategoryName,
                                          Date = g.Key.Date,
                                          Quantity = g.Sum(i => i.Quantity),
                                          TotalAmount = g.Sum(i => i.TotalAmount),
                                      };
                    AllSales.AddRange(Creditsales);


                    var returns = from s in ReturnData
                                  group s by new { s.Date, s.CategoryID, s.CategoryName } into g
                                  select new ProductWiseSalesReportModel
                                  {
                                      CategoryID = g.Key.CategoryID,
                                      CategoryName = g.Key.CategoryName,
                                      Date = g.Key.Date,
                                      Quantity = g.Sum(i => i.Quantity),
                                      TotalAmount = g.Sum(i => i.TotalAmount),
                                  };
                    AllReturns.AddRange(returns);


                }

                foreach (var item in AllSales)
                {
                    dt.Rows.Add(item.Date.ToString("dd MMM yyyy"), "", item.CompanyName, item.CategoryName, item.ProductName, item.Quantity, item.SalesRate, item.TotalAmount, "");
                }

                foreach (var item in AllReturns)
                {
                    dtReturn.Rows.Add(item.Date.ToString("dd MMM yyyy"), "", item.CompanyName, item.CategoryName, item.ProductName, item.Quantity, item.SalesRate, item.TotalAmount, "");
                }


                decimal TotalSalesQuantity = AllSales.Count() != 0 ? AllSales.Sum(o => o.Quantity) : 0m;
                decimal TotalReturnQuantity = AllReturns.Count() != 0 ? AllReturns.Sum(o => o.Quantity) : 0m;
                decimal TotalSalesPrice = AllSales.Count() != 0 ? AllSales.Sum(o => o.TotalAmount) : 0m;
                decimal TotalReturnPrice = AllReturns.Count() != 0 ? AllReturns.Sum(o => o.TotalAmount) : 0m;

                decimal NetQuantity = TotalSalesQuantity - TotalReturnQuantity;

                decimal NetPrice = TotalSalesPrice - TotalReturnPrice;



                //foreach (var item in Creditsales)
                //{
                //    dt.Rows.Add(item.Date.ToString("dd MMM yyyy"), "", item.CompanyName, item.CategoryName, item.ProductName, item.Quantity, item.SalesRate, item.Quantity * item.SalesRate, "");
                //}

                dt.TableName = "PWSDetails";
                _dataSet.Tables.Add(dt);


                dtReturn.TableName = "PWRDetails";
                _dataSet.Tables.Add(dtReturn);


                GetCommonParameters(userName, concernID);

                _reportParameter = new ReportParameter("PrintDate", GetClientDateTime());
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("NetQuantity", NetQuantity.ToString("0.00"));
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("NetPrice", NetPrice.ToString("0.00"));
                _reportParameters.Add(_reportParameter);

                if (reportType == 0)
                {
                    _reportParameter = new ReportParameter("DateRange", "Product Wise Sales Summary Date From : " + fromDate.ToString("dd MMM yyyy") + " to " + toDate.ToString("dd MMM yyyy"));
                    _reportParameters.Add(_reportParameter);
                    return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Sales\\rptPWSalesSummary.rdlc");
                }
                else if (reportType == 1)
                {
                    _reportParameter = new ReportParameter("DateRange", "Company Wise Sales Summary Date From : " + fromDate.ToString("dd MMM yyyy") + " to " + toDate.ToString("dd MMM yyyy"));
                    _reportParameters.Add(_reportParameter);
                    return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Sales\\rptCompanyWiseSalesSummary.rdlc");
                }
                else
                {
                    _reportParameter = new ReportParameter("DateRange", "Category Wise Sales Summary Date From : " + fromDate.ToString("dd MMM yyyy") + " to " + toDate.ToString("dd MMM yyyy"));
                    _reportParameters.Add(_reportParameter);
                    return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Sales\\rptCategoryWiseSalesSummary.rdlc");
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public byte[] ProductWisePurchaseDetailsReport(string userName, int concernID, int reportType, int CompanyID, int CategoryID, int ProductID, DateTime fromDate, DateTime toDate, EnumPurchaseType PurchaseType)
        {
            try
            {
                List<ProductWisePurchaseModel> ReportDate = new List<ProductWisePurchaseModel>();
                TransactionalDataSet.PWPDetailsDataTable dt = new TransactionalDataSet.PWPDetailsDataTable();
                _dataSet = new DataSet();
                var Data = _purchaseOrderService.ProductWisePurchaseDetailsReport(CompanyID, CategoryID, ProductID, fromDate, toDate, PurchaseType);

                if (reportType == 0)
                {
                    ReportDate = Data;
                }
                else if (reportType == 1)
                {
                    ReportDate = (from d in Data
                                  group d by new { d.CategoryName, d.CompanyName } into g
                                  select new ProductWisePurchaseModel
                                  {
                                      CompanyName = g.Key.CompanyName,
                                      CategoryName = g.Key.CategoryName,
                                      Quantity = g.Sum(i => i.Quantity),
                                      TotalAmount = g.Sum(i => i.TotalAmount)
                                  }).ToList();
                }
                else
                {
                    ReportDate = (from d in Data
                                  group d by new { d.CategoryName } into g
                                  select new ProductWisePurchaseModel
                                  {
                                      CategoryName = g.Key.CategoryName,
                                      Quantity = g.Sum(i => i.Quantity),
                                      TotalAmount = g.Sum(i => i.TotalAmount)
                                  }).ToList();
                }

                foreach (var item in ReportDate)
                {
                    dt.Rows.Add(item.Date, item.ChallanNo, item.CompanyName, item.CategoryName, item.ProductName, item.Quantity, item.PurchaseRate, item.TotalAmount);
                }

                dt.TableName = "PWPDetails";
                _dataSet.Tables.Add(dt);

                GetCommonParameters(userName, concernID);
                _reportParameter = new ReportParameter("PrintDate", GetClientDateTime());
                _reportParameters.Add(_reportParameter);
                if (PurchaseType == EnumPurchaseType.ProductReturn)
                {
                    if (reportType == 0)
                    {
                        _reportParameter = new ReportParameter("DateRange", "Product Wise Purchase Return Date From : " + fromDate.ToString("dd MMM yyyy") + " to " + toDate.ToString("dd MMM yyyy"));
                        _reportParameters.Add(_reportParameter);
                        return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Purchase\\rptProductWPDetails.rdlc");
                    }
                    else if (reportType == 1)
                    {
                        _reportParameter = new ReportParameter("DateRange", "Company Wise Purchase Return Date From : " + fromDate.ToString("dd MMM yyyy") + " to " + toDate.ToString("dd MMM yyyy"));
                        _reportParameters.Add(_reportParameter);
                        return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Purchase\\rptCompanyWPDetails.rdlc");
                    }
                    else
                    {
                        _reportParameter = new ReportParameter("DateRange", "Category Wise Purchase Return Date From : " + fromDate.ToString("dd MMM yyyy") + " to " + toDate.ToString("dd MMM yyyy"));
                        _reportParameters.Add(_reportParameter);
                        return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Purchase\\rptCategoryWPDetails.rdlc");
                    }
                }
                else
                {
                    if (reportType == 0)
                    {
                        _reportParameter = new ReportParameter("DateRange", "Product Wise Purchase Date From : " + fromDate.ToString("dd MMM yyyy") + " to " + toDate.ToString("dd MMM yyyy"));
                        _reportParameters.Add(_reportParameter);
                        return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Purchase\\rptProductWPDetails.rdlc");
                    }
                    else if (reportType == 1)
                    {
                        _reportParameter = new ReportParameter("DateRange", "Company Wise Purchase Date From : " + fromDate.ToString("dd MMM yyyy") + " to " + toDate.ToString("dd MMM yyyy"));
                        _reportParameters.Add(_reportParameter);
                        return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Purchase\\rptCompanyWPDetails.rdlc");
                    }
                    else
                    {
                        _reportParameter = new ReportParameter("DateRange", "Category Wise Purchase Date From : " + fromDate.ToString("dd MMM yyyy") + " to " + toDate.ToString("dd MMM yyyy"));
                        _reportParameters.Add(_reportParameter);
                        return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Purchase\\rptCategoryWPDetails.rdlc");
                    }
                }


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public byte[] BankTransactionReport(string userName, int concernID, int reportType, int BankID, DateTime fromDate, DateTime toDate)
        {
            try
            {
                var Data = _bankTransactionService.GetAllBankTransaction();
                TransactionalDataSet.dtBankTransactionDataTable dt = new TransactionalDataSet.dtBankTransactionDataTable();
                _dataSet = new DataSet();
                DataRow row = null;

                if (BankID != 0)
                {
                    Data = Data.Where(o => o.Item4 == BankID.ToString() && o.Rest.Item2 >= fromDate && o.Rest.Item2 <= toDate);
                }
                int TransType = 0;
                foreach (var item in Data)
                {
                    row = dt.NewRow();
                    TransType = Convert.ToInt32(item.Item7);

                    row["TranDate"] = item.Rest.Item2;
                    row["TransactionNo"] = item.Item6;
                    row["TransactionType"] = (EnumTransactionType)TransType;
                    row["BankName"] = item.Item2;

                    if (TransType == (int)EnumTransactionType.Withdraw || TransType == (int)EnumTransactionType.FundTransfer)
                        row["Amount"] = -item.Rest.Item1;
                    else
                        row["Amount"] = item.Rest.Item1;

                    row["Remarks"] = item.Rest.Item3;
                    row["ChecqueNo"] = item.Rest.Rest.Item3;
                    row["BranchName"] = item.Rest.Rest.Item1;
                    row["AccountNO"] = item.Rest.Rest.Item2;
                    row["AccountName"] = item.Rest.Rest.Item4;
                    dt.Rows.Add(row);
                }
                dt.TableName = "BankTransaction";
                _dataSet.Tables.Add(dt);
                GetCommonParameters(userName, concernID);
                _reportParameter = new ReportParameter("PrintDate", GetClientDateTime());
                _reportParameters.Add(_reportParameter);
                _reportParameter = new ReportParameter("DateRange", "Bank Transaction for Date From : " + fromDate.ToString("dd MMM yyyy") + " to " + toDate.ToString("dd MMM yyyy"));
                _reportParameters.Add(_reportParameter);
                return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Bank\\rptBankTransactions.rdlc");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public byte[] POInvoicePrint(POrder POrder, string userName, int concernID)
        {
            TransactionalDataSet.dtPOInvoiceDataTable dtPODetail = new TransactionalDataSet.dtPOInvoiceDataTable();
            _dataSet = new DataSet();
            var Supplier = _SupplierService.GetSupplierById(POrder.SupplierID);
            var Products = _productService.GetAllProductIQueryable();
            string IMEI = string.Empty;
            string DIMEI = string.Empty;
            //int Counter = 0;
            decimal AreaPerCarton = 0m, RatePerSqft = 0m, TotalSqft = 0m, AreaPerPcs = 0m;
            DataRow row = null;
            decimal[] sizeXY = null;
            bool IsTiles = false;
            Color oColor = null;
            ProductWisePurchaseModel oProduct = null;
            if (POrder.POrderDetails != null)
            {
                foreach (var item in POrder.POrderDetails)
                {
                    oProduct = Products.FirstOrDefault(p => p.ProductID == item.ProductID);
                    oColor = _ColorServce.GetColorById(item.ColorID);
                    #region POP
                    //foreach (var sitem in item.POProductDetails)
                    //{
                    //    Counter++;

                    //    if (item.POProductDetails.Count() == Counter)
                    //    {
                    //        IMEI += sitem.IMENO;
                    //        DIMEI += sitem.DIMENO;
                    //    }
                    //    else
                    //    {
                    //        IMEI += sitem.IMENO + Environment.NewLine;
                    //        DIMEI += sitem.DIMENO + Environment.NewLine;
                    //    }

                    //}
                    #endregion

                    row = dtPODetail.NewRow();
                    row["ProductName"] = oProduct.ProductName + " ," + oProduct.CategoryName;
                    row["CategoryName"] = oProduct.CategoryName;
                    row["CompanyName"] = oProduct.CompanyName;
                    row["ModelName"] = oProduct.SizeName;
                    row["UnitPrice"] = item.UnitPrice * oProduct.ConvertValue;
                    row["PPDISAmount"] = item.PPDISAmt * oProduct.ConvertValue;
                    row["PPDISPer"] = item.PPDISPer;
                    row["ExtraPPDISAmt"] = item.ExtraPPDISAmt;
                    row["ExtraPPDISPer"] = item.ExtraPPDISPer;
                    row["Quantity"] = Math.Truncate(item.Quantity / oProduct.ConvertValue);
                    row["ChildQty"] = item.Quantity % oProduct.ConvertValue;
                    row["TAmount"] = item.TAmount;
                    row["ProductCode"] = oProduct.ProductCode;
                    row["MRP"] = item.MRPRate * oProduct.ConvertValue;
                    row["OfferAmount"] = item.PPOffer;
                    row["IMEI"] = oProduct.SizeName; ;
                    row["ChildUnitName"] = oProduct.ChildUnitName;
                    row["UnitName"] = oProduct.ParentUnitName;

                    var sa = oProduct.SizeName.ToLower().Split('x');
                    if (oProduct.CategoryName == "Tiles")
                    {
                        //sizeXY = Array.ConvertAll(oProduct.SizeName.ToLower().Split('x'), decimal.Parse);
                        //AreaPerCarton = oProduct.PurchaseCSft;//Math.Round((((sizeXY[0] * sizeXY[1]) / 10000m) * oProduct.BundleQty) * 10.76m, 4);
                        //AreaPerPcs = oProduct.PurchaseCSft / oProduct.ConvertValue; //Math.Round((((sizeXY[0] * sizeXY[1]) / 10000m)) * 10.76m, 4);
                        row["AreaPerCarton"] = oProduct.PurchaseCSft;
                        row["RatePerSqft"] = item.SFTRate;// Math.Round(item.UnitPrice / AreaPerPcs, 4);
                        if (oProduct.BundleQty != 0)
                            TotalSqft = item.TotalSFT; //Math.Round(oProduct.PurchaseCSft * (item.Quantity / oProduct.BundleQty), 4);
                        else
                            TotalSqft = item.TotalSFT; //Math.Round(oProduct.PurchaseCSft * (item.Quantity), 4);
                        row["TotalSqft"] = TotalSqft;
                        IsTiles = true;
                    }


                    dtPODetail.Rows.Add(row);

                    IMEI = string.Empty;
                    DIMEI = string.Empty;
                    //Counter = 0;
                }
            }
            dtPODetail.TableName = "dtPOInvoice";
            _dataSet.Tables.Add(dtPODetail);


            GetCommonParameters(userName, concernID);

            if (POrder != null)
            {
                _reportParameter = new ReportParameter("SupplierCode", Supplier.Code);
                _reportParameters.Add(_reportParameter);
                _reportParameter = new ReportParameter("SupplierName", Supplier.Name);
                _reportParameters.Add(_reportParameter);
                _reportParameter = new ReportParameter("ChallanNo", POrder.ChallanNo);
                _reportParameters.Add(_reportParameter);
                _reportParameter = new ReportParameter("Remarks", POrder.Remarks);
                _reportParameters.Add(_reportParameter);
                _reportParameter = new ReportParameter("OrderDate", POrder.OrderDate.ToString());
                _reportParameters.Add(_reportParameter);
                _reportParameter = new ReportParameter("FlatDis", POrder.NetDiscount.ToString());
                _reportParameters.Add(_reportParameter);
                _reportParameter = new ReportParameter("NetTotal", POrder.TotalAmt.ToString());
                _reportParameters.Add(_reportParameter);
                _reportParameter = new ReportParameter("PaidAmt", POrder.RecAmt.ToString());
                _reportParameters.Add(_reportParameter);
                _reportParameter = new ReportParameter("CurrentDue", POrder.PaymentDue.ToString());
                _reportParameters.Add(_reportParameter);
                _reportParameter = new ReportParameter("PrintDate", "Date: " + GetClientDateTime());
                _reportParameters.Add(_reportParameter);
            }

            if (POrder.Status == (int)EnumPurchaseType.DeliveryOrder)
            {
                _reportParameter = new ReportParameter("ReportHeader", "Delivery Order");
                _reportParameters.Add(_reportParameter);
            }
            else if (POrder.Status == (int)EnumPurchaseType.DamageReturn)
            {
                _reportParameter = new ReportParameter("ReportHeader", "Damage Return Order");
                _reportParameters.Add(_reportParameter);
                return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Purchase\\rptDamageReturnInvoice.rdlc");
            }
            else if (POrder.Status == (int)EnumPurchaseType.Purchase && POrder.IsDamageOrder == 1)
            {
                _reportParameter = new ReportParameter("ReportHeader", "Damage Order");
                _reportParameters.Add(_reportParameter);
                return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Purchase\\rptDamageRecInvoice.rdlc");
            }
            else if (POrder.Status == (int)EnumPurchaseType.ProductReturn)
            {
                _reportParameter = new ReportParameter("ReportHeader", "Purchase Return Order");
                _reportParameters.Add(_reportParameter);
            }
            else if (POrder.Status == (int)EnumPurchaseType.RawStockDeduct)
            {
                _reportParameter = new ReportParameter("ReportHeader", "Raw Material Deduct Order");
                _reportParameters.Add(_reportParameter);
                return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Purchase\\rptRawDeductInvoice.rdlc");

            }
            else
            {
                _reportParameter = new ReportParameter("ReportHeader", "Purchase Order");
                _reportParameters.Add(_reportParameter);
            }

            //if (POrder.Status == (int)EnumPurchaseType.DeliveryOrder)
            if (IsTiles)
                return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Purchase\\rptDOInvoice.rdlc");
            else
                return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Purchase\\rptPOInvoice.rdlc");


            //return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Purchase\\rptPOInvoice.rdlc");
        }

        public byte[] POInvoice(POrder POrder, string userName, int concernID)
        {
            return POInvoicePrint(POrder, userName, concernID);
        }


        public byte[] POInvoiceByID(int POrderID, string userName, int concernID)
        {
            POrder oPOrder = new POrder();

            oPOrder = _purchaseOrderService.GetPurchaseOrderById(POrderID);
            oPOrder.POrderDetails = _PurchaseOrderDetailService.GetPOrderDetailByID(POrderID).ToList();
            if (oPOrder.IsDamageOrder == 1)
            {
                POProductDetail POPD = null;
                foreach (POrderDetail item in oPOrder.POrderDetails)
                {
                    foreach (var sitem in item.POProductDetails)
                    {
                        POPD = _POProductDetailService.GetPOPDetailByPOPDID((int)sitem.DamagePOPDID);
                        if (POPD != null)
                            sitem.DIMENO = POPD.IMENO;
                    }
                }
            }
            return POInvoicePrint(oPOrder, userName, concernID);
        }

        public byte[] GetDamagePOReport(string userName, int concernID, int SupplierID, DateTime fromDate, DateTime toDate)
        {
            try
            {
                var Purchases = _purchaseOrderService.GetDamagePOReport(fromDate, toDate, SupplierID);

                decimal TotalDuePurchase = 0;
                decimal GrandTotal = 0;
                decimal TotalDis = 0;
                decimal NetTotal = 0;
                decimal RecAmt = 0;
                decimal CurrDue = 0;
                TransactionalDataSet.dtSuppWiseDataDataTable dt = new TransactionalDataSet.dtSuppWiseDataDataTable();

                DataRow row = null;
                int POrderID = 0;
                foreach (var item in Purchases)
                {
                    if (POrderID != item.POrderID)
                    {
                        TotalDuePurchase = TotalDuePurchase + item.PaymentDue;
                        GrandTotal = GrandTotal + item.GrandTotal;
                        TotalDis = TotalDis + item.NetDiscount;
                        NetTotal = NetTotal + item.NetTotal;
                        RecAmt = RecAmt + item.RecAmt;
                        CurrDue = CurrDue + item.PaymentDue;
                    }
                    row = dt.NewRow();
                    row["PurchaseDate"] = item.Date;
                    row["ChallanNo"] = item.ChallanNo;
                    row["ProductName"] = item.ProductName;
                    row["PurchaseRate"] = item.PurchaseRate;
                    row["DisAmt"] = item.PPDISAmt;
                    row["NetAmt"] = item.TotalAmount;
                    row["GrandTotal"] = item.GrandTotal;
                    row["TotalDis"] = item.NetDiscount;
                    row["NetTotal"] = item.NetTotal;
                    row["PaidAmt"] = item.RecAmt;
                    row["RemainingAmt"] = item.PaymentDue;
                    row["Quantity"] = item.Quantity;
                    row["ChasisNo"] = item.IMENO;
                    row["Model"] = item.CategoryName;
                    row["Color"] = item.ColorName;
                    row["PPOffer"] = item.PPOffer;
                    row["DamageIMEI"] = item.DamageIMEI;

                    dt.Rows.Add(row);

                    //dt.Rows.Add(grd.OrderDate, grd.ChallanNo, grd.ProductName, grd.UnitPrice, grd.PPDISAmt, grd.TAmount - grd.PPDISAmt, grd.GrandTotal, grd.TDiscount, grd.TotalAmt, grd.RecAmt, grd.PaymentDue, grd.Quantity, oPOPD.IMENo, "", oPOPD.POrderDetail.Color.Description);
                    //dt.Rows.Add(item.Item1, item.Item2, item.Item3, item.Item4, item.Item5, item.Item6 - item.Item5, item.Item7, item.Rest.Item1, item.Rest.Item2, item.Rest.Item3, item.Rest.Item4, "1", item.Rest.Item6, item.Rest.Item5, item.Rest.Item7, item.Rest.Rest.Item1);

                    POrderID = item.POrderID;
                }

                dt.TableName = "dtSuppWiseData";
                _dataSet = new DataSet();
                _dataSet.Tables.Add(dt);

                GetCommonParameters(userName, concernID);
                _reportParameter = new ReportParameter("Date", "Damage Purchase details from the Date : " + fromDate.ToString("dd MMM yyyy") + " To " + toDate.ToString("dd MMM yyyy"));
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("GrandTotal", GrandTotal.ToString());
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("TotalDis", TotalDis.ToString());
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("NetTotal", NetTotal.ToString());
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("RecAmt", RecAmt.ToString());
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("CurrDue", CurrDue.ToString());
                _reportParameters.Add(_reportParameter);
                return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Purchase\\rptDamagePurchaseDetails.rdlc");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public byte[] GetDamageReturnPOReport(string userName, int concernID, int SupplierID, DateTime fromDate, DateTime toDate)
        {
            try
            {
                var DamageReutruns = _purchaseOrderService.DamageReturnProductDetailsReport(SupplierID, fromDate, toDate);

                decimal TotalDuePurchase = 0;
                decimal GrandTotal = 0;
                decimal TotalDis = 0;
                decimal NetTotal = 0;
                decimal RecAmt = 0;
                decimal CurrDue = 0;
                TransactionalDataSet.dtSuppWiseDataDataTable dt = new TransactionalDataSet.dtSuppWiseDataDataTable();

                DataRow row = null;
                int POrderID = 0;
                foreach (var item in DamageReutruns)
                {
                    if (POrderID != item.POrderID)
                    {
                        TotalDuePurchase = TotalDuePurchase + item.PaymentDue;
                        GrandTotal = GrandTotal + item.GrandTotal;
                        TotalDis = TotalDis + item.NetDiscount;
                        NetTotal = NetTotal + item.NetTotal;
                        RecAmt = RecAmt + item.RecAmt;
                        CurrDue = CurrDue + item.PaymentDue;
                    }
                    row = dt.NewRow();
                    row["PurchaseDate"] = item.Date;
                    row["ChallanNo"] = item.ChallanNo;
                    row["ProductName"] = item.ProductName;
                    row["PurchaseRate"] = item.PurchaseRate;
                    row["DisAmt"] = item.PPDISAmt;
                    row["NetAmt"] = item.TotalAmount;
                    row["GrandTotal"] = item.GrandTotal;
                    row["TotalDis"] = item.NetDiscount;
                    row["NetTotal"] = item.NetTotal;
                    row["PaidAmt"] = item.RecAmt;
                    row["RemainingAmt"] = item.PaymentDue;
                    row["Quantity"] = item.Quantity;
                    row["ChasisNo"] = item.IMENO;
                    row["Model"] = item.CategoryName;
                    row["Color"] = item.ColorName;
                    row["PPOffer"] = item.PPOffer;
                    row["DamageIMEI"] = item.DamageIMEI;

                    dt.Rows.Add(row);

                    //dt.Rows.Add(grd.OrderDate, grd.ChallanNo, grd.ProductName, grd.UnitPrice, grd.PPDISAmt, grd.TAmount - grd.PPDISAmt, grd.GrandTotal, grd.TDiscount, grd.TotalAmt, grd.RecAmt, grd.PaymentDue, grd.Quantity, oPOPD.IMENo, "", oPOPD.POrderDetail.Color.Description);
                    //dt.Rows.Add(item.Item1, item.Item2, item.Item3, item.Item4, item.Item5, item.Item6 - item.Item5, item.Item7, item.Rest.Item1, item.Rest.Item2, item.Rest.Item3, item.Rest.Item4, "1", item.Rest.Item6, item.Rest.Item5, item.Rest.Item7, item.Rest.Rest.Item1);

                    POrderID = item.POrderID;
                }

                dt.TableName = "dtSuppWiseData";
                _dataSet = new DataSet();
                _dataSet.Tables.Add(dt);

                GetCommonParameters(userName, concernID);
                _reportParameter = new ReportParameter("Date", "Damage Return Purchase details from the Date : " + fromDate.ToString("dd MMM yyyy") + " To " + toDate.ToString("dd MMM yyyy"));
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("GrandTotal", GrandTotal.ToString());
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("TotalDis", TotalDis.ToString());
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("NetTotal", NetTotal.ToString());
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("RecAmt", RecAmt.ToString());
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("CurrDue", CurrDue.ToString());
                _reportParameters.Add(_reportParameter);
                return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Purchase\\rptDamageReturnPODetails.rdlc");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public byte[] GetSalarySheet(DateTime dtSalaryMonth, int EmployeeID, int DepartmentID, List<int> EmployeeIDList, string UserName, int ConcernID, Tuple<DateTime, DateTime> SalaryMonth)
        {
            List<SalaryMonthly> salaryMonthlyList = new List<SalaryMonthly>();
            var list = _SalaryMonthlyService.GetAllIQueryable();
            SalaryMonthly obj = null;
            List<SalaryMonthlyDetail> MDetails = null;
            string DepartmentName = string.Empty;
            if (EmployeeID != default(int))
            {
                var employee = _EmployeeService.GetEmployeeById(EmployeeID);
                var department = _DepartmentService.GetDepartmentById((int)employee.DepartmentID);
                obj = new SalaryMonthly();
                obj = list.FirstOrDefault(i => i.EmployeeID == EmployeeID && (i.SalaryMonth >= SalaryMonth.Item1 && i.SalaryMonth <= SalaryMonth.Item2));
                if (obj != null)
                {
                    MDetails = _SalaryMonthlyService.GetSalaryMonthlyDetailBy(obj.SalaryMonthlyID);
                    obj.SalaryMonthlyDetails = MDetails;
                    salaryMonthlyList.Add(obj);
                }
                return ShowSalarySheet(salaryMonthlyList, dtSalaryMonth, department.DESCRIPTION, UserName, ConcernID);
            }


            foreach (var employeeID in EmployeeIDList)
            {
                obj = new SalaryMonthly();
                obj = list.FirstOrDefault(i => i.EmployeeID == employeeID && (i.SalaryMonth >= SalaryMonth.Item1 && i.SalaryMonth <= SalaryMonth.Item2));
                if (obj != null)
                {
                    MDetails = _SalaryMonthlyService.GetSalaryMonthlyDetailBy(obj.SalaryMonthlyID);
                    obj.SalaryMonthlyDetails = MDetails;
                    salaryMonthlyList.Add(obj);
                }
            }

            if (DepartmentID != 0)
            {
                var department = _DepartmentService.GetDepartmentById(DepartmentID);
                if (department != null)
                    DepartmentName = department.DESCRIPTION;
            }

            return ShowSalarySheet(salaryMonthlyList, dtSalaryMonth, DepartmentName, UserName, ConcernID);

        }
        private byte[] ShowSalarySheet(List<SalaryMonthly> salaryMonthlyList, DateTime SalaryMonth, string DepartmentName, string UserName, int ConcernID)
        {

            TransactionalDataSet.dtSalaryMonthlyDataTable dt = new TransactionalDataSet.dtSalaryMonthlyDataTable();
            DataRow row = null;
            var designations = _DesignationService.GetAllIQueryable();
            var employees = _EmployeeService.GetAllEmployeeIQueryable();
            decimal TotalAmount = 0, GrossSalary = 0;
            var data = (from sm in salaryMonthlyList
                        join emp in employees on sm.EmployeeID equals emp.EmployeeID
                        join d in designations on emp.DesignationID equals d.DesignationID
                        select new
                        {
                            Code = emp.Code,
                            AccountNO = emp.MachineEMPID,
                            Name = emp.Name,
                            Designation = d.Description,
                            SalaryDetails = sm.SalaryMonthlyDetails.ToList(),
                            WorkinDays = sm.WorkinDays,
                            sm.OTHours
                        }).OrderBy(i => i.Code);
            foreach (var item in data)
            {
                row = dt.NewRow();
                row["Code"] = item.Code;
                row["Name"] = item.Name;
                row["Designation"] = item.Designation;

                foreach (SalaryMonthlyDetail sitem in item.SalaryDetails)
                {
                    if (sitem.ItemID == (int)EnumSalaryItemCode.Basic_Salary)
                        row["BasicSalary"] = sitem.CalculatedAmount;

                    if (sitem.ItemID == (int)EnumSalaryItemCode.Gross_Salary)
                        row["GrossSalary"] = sitem.CalculatedAmount;

                    row["Attendence"] = item.WorkinDays;
                    row["OT"] = Math.Floor(item.OTHours);

                    if (sitem.ItemID == (int)EnumSalaryItemCode.Tot_Attend_Days_Amount)
                        row["AttendenceSalary"] = sitem.CalculatedAmount;

                    if (sitem.ItemID == (int)EnumSalaryItemCode.Over_Time_Amount)
                        row["OTSalary"] = sitem.CalculatedAmount;

                    if (sitem.ItemID == (int)EnumSalaryItemCode.Tot_Attend_Days_Bonus)
                        row["AttendenceBonus"] = sitem.CalculatedAmount;

                    if (sitem.ItemID == (int)EnumSalaryItemCode.Net_Payable)
                        row["TotalAmount"] = sitem.CalculatedAmount;

                    if (sitem.ItemID == (int)EnumSalaryItemCode.Advance_Deduction)
                    {
                        row["Advance"] = sitem.CalculatedAmount;
                        TotalAmount += sitem.CalculatedAmount;
                    }

                    if (sitem.ItemID == (int)EnumSalaryItemCode.Net_Payable)
                    {
                        row["NetAmount"] = sitem.CalculatedAmount;
                        TotalAmount += sitem.CalculatedAmount;
                    }

                    if (sitem.ItemID == (int)EnumSalaryItemCode.Bonus)
                        row["FestBonus"] = sitem.CalculatedAmount;

                    if (sitem.ItemID == (int)EnumSalaryItemCode.Voltage_StabilizerComm)
                        row["VSCommission"] = sitem.CalculatedAmount;

                    #region Allowance Deduction
                    if (sitem.Description.Contains("House"))
                    {
                        row["HouseRent"] = sitem.CalculatedAmount;
                        //GrossSalary += sitem.CalculatedAmount;
                    }

                    if (sitem.Description.Contains("Medical"))
                    {
                        row["Medical"] = sitem.CalculatedAmount;
                        //GrossSalary += sitem.CalculatedAmount;
                    }

                    if (sitem.Description.Contains("Transport"))
                    {
                        row["Transport"] = sitem.CalculatedAmount;
                        //GrossSalary += sitem.CalculatedAmount;
                    }

                    if (sitem.Description.Contains("Food"))
                    {
                        row["Food"] = sitem.CalculatedAmount;
                        //GrossSalary += sitem.CalculatedAmount;
                    }
                    if (sitem.Description.Contains("Mobile"))
                    {
                        row["Mobile"] = sitem.CalculatedAmount;
                        //GrossSalary += sitem.CalculatedAmount;
                    }
                    #endregion


                    if (sitem.ItemID == (int)EnumSalaryItemCode.Commission)
                        row["Commission"] = sitem.CalculatedAmount;

                    if (sitem.ItemID == (int)EnumSalaryItemCode.Extra_Commission)
                        row["ExtraCommission"] = sitem.CalculatedAmount;

                }
                row["TotalAmount"] = TotalAmount;
                GrossSalary = 0;
                TotalAmount = 0;
                dt.Rows.Add(row);
            }

            dt.TableName = "dtSalaryMonthly";
            _dataSet = new DataSet();
            _dataSet.Tables.Add(dt);

            GetCommonParameters(UserName, ConcernID);

            _reportParameter = new ReportParameter("ReportHeading", "Salary Sheet: " + SalaryMonth.ToString("MMMM-yyyy"));
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("DepartmentName", "Department: " + DepartmentName);
            _reportParameters.Add(_reportParameter);

            return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "HRPR\\rptSalarySheet.rdlc");
        }

        public byte[] GetPaySlip(DateTime dtSalaryMonth, int EmployeeID, string UserName, int ConcernID, Tuple<DateTime, DateTime> DateRange)
        {
            var list = _SalaryMonthlyService.GetAllIQueryable();
            SalaryMonthly obj = null;
            Employee employee = new Employee();
            List<SalaryMonthlyDetail> MDetails = new List<SalaryMonthlyDetail>();
            if (EmployeeID != 0)
            {
                employee = _EmployeeService.GetAllEmployeeIQueryable().FirstOrDefault(i => i.EmployeeID == EmployeeID);
                obj = list.FirstOrDefault(i => i.EmployeeID == EmployeeID && (i.SalaryMonth >= DateRange.Item1 && i.SalaryMonth <= DateRange.Item2));
                if (obj != null)
                {
                    MDetails = _SalaryMonthlyService.GetSalaryMonthlyDetailBy(obj.SalaryMonthlyID);
                    obj.SalaryMonthlyDetails = MDetails;
                }
                else
                    return null;
            }
            return ShowPaySlip(obj, employee, dtSalaryMonth, UserName, ConcernID);

        }
        private byte[] ShowPaySlip(SalaryMonthly salaryMonthly, Employee employee, DateTime dtSalaryMonth, string UserName, int ConcernID)
        {

            TransactionalDataSet.dtPaySlipDataTable dt = new TransactionalDataSet.dtPaySlipDataTable();
            DataRow row = null;
            string description = string.Empty;
            List<PaySlip> paySlipList = new List<PaySlip>();
            PaySlip objPayslip = null;
            List<SalaryMonthlyDetail> Allowancec = salaryMonthly.SalaryMonthlyDetails.Where(i => i.ItemGroup == (int)EnumSalaryGroup.Gross).ToList();
            List<SalaryMonthlyDetail> Deductions = salaryMonthly.SalaryMonthlyDetails.Where(i => i.ItemGroup == (int)EnumSalaryGroup.Deductions).ToList();
            var LastGradeSalary = _GradeSalaryAssignment.GetLastGradeSalaryByEmployeeID(employee.EmployeeID);
            if (Allowancec.Count() > Deductions.Count())
            {
                foreach (var item in Allowancec)
                {
                    objPayslip = new PaySlip();
                    objPayslip.Allowance = GetUserFriendlyDescription(item.ItemID, item.Description);
                    objPayslip.AllowanceAmount = item.CalculatedAmount;
                    paySlipList.Add(objPayslip);
                }
                for (int i = 0; i < Deductions.Count(); i++)
                {
                    paySlipList[i].Deduction = GetUserFriendlyDescription(Deductions[i].ItemID, Deductions[i].Description);
                    paySlipList[i].DeductionAmount = Deductions[i].CalculatedAmount;
                }
            }
            else
            {
                foreach (var item in Deductions)
                {
                    objPayslip = new PaySlip();
                    objPayslip.Allowance = GetUserFriendlyDescription(item.ItemID, item.Description);
                    objPayslip.AllowanceAmount = item.CalculatedAmount;
                    paySlipList.Add(objPayslip);
                }
                for (int i = 0; i < Allowancec.Count(); i++)
                {
                    paySlipList[i].Deduction = GetUserFriendlyDescription(Deductions[i].ItemID, Deductions[i].Description);
                    paySlipList[i].DeductionAmount = Allowancec[i].CalculatedAmount;
                }
            }
            foreach (var item in paySlipList)
            {
                row = dt.NewRow();
                row["Description"] = item.Allowance;
                row["Earnings"] = item.AllowanceAmount;
                row["TotalEarning"] = item.AllowanceAmount;
                row["Deduction"] = item.Deduction;
                row["Amount"] = item.DeductionAmount;
                dt.Rows.Add(row);
            }
            dt.TableName = "dtPaySlip";
            _dataSet = new DataSet();
            _dataSet.Tables.Add(dt);


            var Grade = _GradeService.GetById((int)employee.GradeID);
            var Department = _DepartmentService.GetAllDepartmentIQueryable().FirstOrDefault(i => i.DepartmentId == employee.DepartmentID);
            var designations = _DesignationService.GetAllIQueryable().FirstOrDefault(i => i.DesignationID == employee.DesignationID);
            GetCommonParameters(UserName, ConcernID);

            _reportParameter = new ReportParameter("ReportHeading", "Pay Slip of the Month: " + dtSalaryMonth.ToString("MMMM-yyyy"));
            _reportParameters.Add(_reportParameter);
            _reportParameter = new ReportParameter("Employee", employee.Name);
            _reportParameters.Add(_reportParameter);
            _reportParameter = new ReportParameter("EmployeeCode", employee.Code);
            _reportParameters.Add(_reportParameter);
            _reportParameter = new ReportParameter("Grade", Grade.Description);
            _reportParameters.Add(_reportParameter);
            _reportParameter = new ReportParameter("Designation", designations.Description);
            _reportParameters.Add(_reportParameter);
            _reportParameter = new ReportParameter("BasicSalary", LastGradeSalary.BasicSalary.ToString());
            _reportParameters.Add(_reportParameter);
            _reportParameter = new ReportParameter("PaymentMode", "Cash");
            _reportParameters.Add(_reportParameter);
            _reportParameter = new ReportParameter("AccountNo", employee.MachineEMPID.ToString().PadLeft(5, '0'));
            _reportParameters.Add(_reportParameter);
            _reportParameter = new ReportParameter("DepartmentName", Department.DESCRIPTION);
            _reportParameters.Add(_reportParameter);
            _reportParameter = new ReportParameter("PrintDate", "Print Date: " + GetClientDateTime());
            _reportParameters.Add(_reportParameter);

            return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "HRPR\\rptPaysSlip.rdlc");
        }
        public byte[] SRVisitReportDetails(DateTime fromDate, DateTime toDate, string userName, int concernID, int EmployeeID, int ReportType)
        {
            var SRVisitData = _SRVisitService.SRVisitReportDetails(fromDate, toDate, concernID, EmployeeID);
            var Employee = _EmployeeService.GetEmployeeById(EmployeeID);

            TransactionalDataSet.dtSRVisitReportDataTable dt = new TransactionalDataSet.dtSRVisitReportDataTable();
            _dataSet = new DataSet();
            List<SRVisitReportModel> SRVisitList = new List<SRVisitReportModel>();
            DataRow row = null;

            string IMEIs = string.Empty;
            int Counter = 0;
            foreach (var item in SRVisitData)
            {
                row = dt.NewRow();
                row["ConcernID"] = 0m;
                row["EmployeeId"] = 0m;
                row["EmployeeName"] = "";
                row["TransDate"] = item.TransDate;

                row["OpeningQty"] = item.OpeningIMEIList.Count();
                foreach (var sitem in item.OpeningIMEIList)
                {
                    Counter++;
                    if (Counter != item.OpeningIMEIList.Count())
                        IMEIs = IMEIs + sitem + Environment.NewLine;
                    else
                        IMEIs = IMEIs + sitem;
                }
                row["Opening_imeno"] = IMEIs;
                IMEIs = string.Empty;
                Counter = 0;
                row["Opening_productno"] = item.ProductName;

                row["taken_qty"] = item.ReceiveIMEIList.Count();
                foreach (var sitem in item.ReceiveIMEIList)
                {
                    Counter++;
                    if (Counter != item.ReceiveIMEIList.Count())
                        IMEIs = IMEIs + sitem + Environment.NewLine;
                    else
                        IMEIs = IMEIs + sitem;
                }
                row["taken_imeno"] = IMEIs;

                IMEIs = string.Empty;
                Counter = 0;
                foreach (var sitem in item.TotalIMEIList)
                {
                    Counter++;
                    if (Counter != item.TotalIMEIList.Count())
                        IMEIs = IMEIs + sitem + Environment.NewLine;
                    else
                        IMEIs = IMEIs + sitem;
                }
                row["taken_product"] = IMEIs; //Total IMEIs

                row["Total_qty"] = item.TotalIMEIList.Count();

                row["sale_qty"] = item.SalesIMEIList.Count();

                IMEIs = string.Empty;
                Counter = 0;
                foreach (var sitem in item.SalesIMEIList)
                {
                    Counter++;
                    if (Counter != item.SalesIMEIList.Count())
                        IMEIs = IMEIs + sitem + Environment.NewLine;
                    else
                        IMEIs = IMEIs + sitem;
                }
                row["sale_imeno"] = IMEIs;
                row["sale_product"] = "";

                row["balance_qty"] = item.BalanceIMEIList.Count();
                IMEIs = string.Empty;
                Counter = 0;
                foreach (var sitem in item.BalanceIMEIList)
                {
                    Counter++;
                    if (Counter != item.BalanceIMEIList.Count())
                        IMEIs = IMEIs + sitem + Environment.NewLine;
                    else
                        IMEIs = IMEIs + sitem;
                }
                row["imeno_balance"] = IMEIs;
                row["product_balance"] = "";

                dt.Rows.Add(row);

                IMEIs = string.Empty;
                Counter = 0;
            }

            dt.TableName = "dtSRVisitReport";
            _dataSet.Tables.Add(dt);

            GetCommonParameters(userName, concernID);



            _reportParameter = new ReportParameter("SRName", Employee.Name);
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("DateRange", "SR visit Report from date: " + fromDate.ToString("dd MMM yyyy") + " to " + toDate.ToString("dd MMM yyyy"));
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("OpeningGrandCount", "");
            _reportParameters.Add(_reportParameter);
            _reportParameter = new ReportParameter("TakenCountGrand", "");
            _reportParameters.Add(_reportParameter);
            _reportParameter = new ReportParameter("SalesCountGrand", "");
            _reportParameters.Add(_reportParameter);
            _reportParameter = new ReportParameter("BalanceCountGrand", "");
            _reportParameters.Add(_reportParameter);
            _reportParameter = new ReportParameter("PrintDate", "Date:" + GetClientDateTime());
            _reportParameters.Add(_reportParameter);

            if (ReportType == 1) //Summary
                return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "SR\\rptSRVisitSummary.rdlc");
            else
                return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "SR\\rptSRVisitDetails.rdlc");
        }
        public byte[] NewBankTransactionsReport(DateTime fromDate, DateTime toDate, int BankID, string UserName, int ConcernID)
        {
            var BanksTrans = _bankTransactionService.BankTransactionsReport(fromDate, toDate, BankID);
            TransactionalDataSet.dtBankTransactionDataTable dt = new TransactionalDataSet.dtBankTransactionDataTable();
            _dataSet = new DataSet();
            DataRow row = null;
            foreach (var item in BanksTrans)
            {
                row = dt.NewRow();
                row["TranDate"] = item.TransDate;
                row["TransactionNo"] = item.TransactionNo;
                row["TransactionType"] = item.TransType;
                row["BankName"] = item.BankName;
                row["Amount"] = item.Amount;
                row["Remarks"] = item.Remarks;
                row["ChecqueNo"] = item.ChecqueNo;
                row["BranchName"] = "";
                row["AccountNO"] = item.AccountNO;
                row["AccountName"] = item.AccountName;
                row["ConcernName"] = item.ConcernName;
                row["FromToAccountNo"] = item.FromToAccountNo;
                dt.Rows.Add(row);
            }
            dt.TableName = "dtBankTransaction";
            _dataSet = new DataSet();
            _dataSet.Tables.Add(dt);

            GetCommonParameters(UserName, ConcernID);

            _reportParameter = new ReportParameter("DateRange", "Bank Transaction Report from date " + fromDate.ToString("dd MMM yyyy") + " To " + toDate.ToString("dd MMM yyyy"));
            _reportParameters.Add(_reportParameter);

            //_reportParameter = new ReportParameter("DepartmentName", "Department: " + DepartmentName);
            //_reportParameters.Add(_reportParameter);

            return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Bank\\rptBankTransReport.rdlc");

        }


        #region Admin Report

        public byte[] AdminPurchaseReport(DateTime fromDate, DateTime toDate, string userName, int concernID, int UserConcernID)
        {
            try
            {
                var purchaseInfos = _purchaseOrderService.AdminPurchaseReport(fromDate, toDate, concernID);
                DataRow row = null;
                TransactionalDataSet.dtReceiveOrderDataTable dt = new TransactionalDataSet.dtReceiveOrderDataTable();

                foreach (var item in purchaseInfos)
                {
                    row = dt.NewRow();
                    row["CompanyCode"] = item.SupplierCode;
                    row["Name"] = item.SupplierName;
                    row["OrderDare"] = item.Date.ToString("dd MMM yyyy");
                    row["ChallanNo"] = item.ChallanNo;
                    row["GrandTotal"] = item.GrandTotal;
                    row["DisAmt"] = item.NetDiscount;
                    row["TotalAmt"] = item.TotalAmount;
                    row["RecAmt"] = item.RecAmt;
                    row["DueAmt"] = item.PaymentDue;
                    row["ConcernName"] = item.ConcenName;
                    dt.Rows.Add(row);
                }

                dt.TableName = "dtReceiveOrder";
                _dataSet = new DataSet();
                _dataSet.Tables.Add(dt);

                GetCommonParameters(userName, UserConcernID);

                _reportParameter = new ReportParameter("Month", "Purchase report for the date from : " + fromDate.ToString("dd MMM yyyy") + " To " + toDate.ToString("dd MMM yyyy"));


                //else if (PurchaseType == EnumPurchaseType.ProductReturn)
                //    _reportParameter = new ReportParameter("Month", "Purchase Return report for the date from : " + fromDate.ToString("dd MMM yyyy") + " To " + toDate.ToString("dd MMM yyyy"));

                _reportParameters.Add(_reportParameter);

                return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Admin\\rptAdminPurchaseOrder.rdlc");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Date: 06-01-2019
        /// Author: aminul
        /// For Admin, to show all concern sales
        /// </summary>
        public byte[] SalesReportAdmin(DateTime fromDate, DateTime toDate, string userName, int concernID, int UserConcernID, int CustomerType, int ReportType)
        {

            #region Summary Report
            if (ReportType == 1)
            {
                try
                {
                    var salseInfos = _salesOrderService.GetAdminSalesReport(concernID, fromDate, toDate);

                    var CreditsalseInfos = _creditSalesOrderService.GetAdminCrSalesReport(concernID, fromDate, toDate);

                    DataRow row = null;

                    TransactionalDataSet.dtOrderDataTable dt = new TransactionalDataSet.dtOrderDataTable();

                    foreach (var item in salseInfos)
                    {
                        row = dt.NewRow();
                        row["CustomerCode"] = item.CustomerCode;
                        row["Name"] = item.CustomerName;
                        row["Date"] = item.InvoiceDate.ToString("dd MMM yyyy");
                        row["InvoiceNo"] = item.InvoiceNo;
                        row["GrandTotal"] = item.Grandtotal;
                        row["DiscountAmount"] = item.NetDiscount;
                        row["Amount"] = item.TotalAmount;
                        row["RecAmt"] = item.RecAmount;
                        row["DueAmount"] = item.PaymentDue;
                        row["SalesType"] = "Cash Sales";
                        row["AdjustAmt"] = item.AdjAmount;
                        row["TotalOffer"] = item.TotalOffer;
                        row["ConcernName"] = item.ConcernName;
                        dt.Rows.Add(row);
                    }

                    foreach (var item in CreditsalseInfos)
                    {
                        row = dt.NewRow();
                        row["CustomerCode"] = item.CustomerCode;
                        row["Name"] = item.CustomerName;
                        row["Date"] = item.InvoiceDate.ToString("dd MMM yyyy");
                        row["InvoiceNo"] = item.InvoiceNo;
                        row["GrandTotal"] = item.Grandtotal;
                        row["DiscountAmount"] = item.NetDiscount;
                        row["Amount"] = item.TotalAmount;
                        row["RecAmt"] = item.RecAmount;
                        row["DueAmount"] = item.PaymentDue;
                        row["SalesType"] = "Credit Sales";
                        row["AdjustAmt"] = item.AdjAmount;
                        row["TotalOffer"] = item.TotalOffer;
                        row["ConcernName"] = item.ConcernName;
                        row["InstallmentPeriod"] = item.InstallmentPeriod;
                        dt.Rows.Add(row);
                    }

                    dt.TableName = "dtOrder";
                    _dataSet = new DataSet();
                    _dataSet.Tables.Add(dt);

                    GetCommonParameters(userName, UserConcernID);
                    _reportParameter = new ReportParameter("Month", "Sales report for the date from : " + fromDate.ToString("dd MMM yyyy") + " To " + toDate.ToString("dd MMM yyyy"));
                    _reportParameters.Add(_reportParameter);
                    return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Admin\\rptAdminMonthlyOrder.rdlc");
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            #endregion
            #region Details Report
            else
            {
                decimal length = 0m, width = 0m, AreaSqftPerPcs = 0m, AreaSqft = 0m;
                var salseDetailInfos = _salesOrderService.GetSalesDetailReportAdminByConcernID(fromDate, toDate, concernID, CustomerType);

                //IEnumerable<Tuple<DateTime, string, string, string, decimal, decimal, decimal, Tuple<decimal, decimal, decimal, decimal, decimal, string, string, Tuple<int>>>> CreditsalseDetailInfos =
                //                    _creditSalesOrderService.GetCreditSalesDetailReportByConcernID(fromDate, toDate, concernID);

                IEnumerable<Tuple<DateTime, string, string, string, decimal, decimal, decimal, Tuple<decimal, decimal, decimal, decimal, decimal, string, string, Tuple<int, decimal>>>> returnDetailInfos =
                                                      _returnOrderService.GetReturnDetailReportByConcernID(fromDate, toDate, concernID);


                TransactionalDataSet.dtCustomerWiseReturnDataTable dtReturn = new TransactionalDataSet.dtCustomerWiseReturnDataTable();
                TransactionalDataSet.dtCustomerWiseSalesDataTable dt = new TransactionalDataSet.dtCustomerWiseSalesDataTable();
                int SOrderID = 0, CreditSaleID = 0;

                decimal TotalDueSales = 0, AdjAmount = 0;
                decimal GrandTotal = 0;
                decimal TotalDis = 0;
                decimal NetTotal = 0;
                decimal RecAmt = 0;
                decimal CurrDue = 0;
                DataRow row = null;
                foreach (var item in salseDetailInfos)
                {
                    //StockDetail std = oSTDList.FirstOrDefault(x => x.SDetailID == grd.StockDetailID);
                    row = dt.NewRow();
                    if (SOrderID != item.SOrderID)
                    {
                        TotalDueSales = TotalDueSales + item.PaymentDue;
                        GrandTotal = GrandTotal + (decimal)item.GrandTotal;
                        TotalDis = TotalDis + (decimal)item.NetDiscount;
                        NetTotal = NetTotal + (decimal)item.TotalAmount;
                        RecAmt = RecAmt + (decimal)item.RecAmount;
                        CurrDue = CurrDue + (decimal)item.PaymentDue;
                        AdjAmount = AdjAmount + item.AdjAmount;
                    }

                    row["SalesDate"] = item.Date;
                    row["InvoiceNo"] = item.InvoiceNo;
                    row["ProductName"] = item.ProductName;
                    row["CName"] = item.CustomerName;
                    row["SalesPrice"] = item.UnitPrice;
                    row["NetAmt"] = item.TotalAmount;
                    row["GrandTotal"] = item.GrandTotal;
                    row["TotalDis"] = item.NetDiscount;
                    row["NetTotal"] = item.UTAmount;
                    row["PaidAmount"] = item.RecAmount;
                    row["RemainingAmt"] = item.PaymentDue;
                    row["Quantity"] = (int)(item.Quantity / item.ConvertValue);
                    row["ChildQty"] = Convert.ToInt32(item.Quantity % item.ConvertValue);
                    row["IMENo"] = item.IMEI;
                    row["ColorInfo"] = item.ColorName;
                    row["SalesType"] = string.Empty;
                    row["AdjAmount"] = item.AdjAmount;
                    row["UnitName"] = item.UnitName;
                    row["Code"] = item.CustomerCode;
                    row["IdCode"] = item.IDCode;
                    row["TotalArea"] = item.Quantity * (item.SalesPerCartonSft / item.ConvertValue);
                    row["PerCartonSft"] = item.SalesPerCartonSft;
                    row["ConcernName"] = item.ConcernName;
                    // row["SizeName"] = item.SizeName;

                    if (item.CategoryName.ToLower().Equals("tiles"))
                    {
                        var area = item.SizeName.Split('x');
                        length = Math.Round(Convert.ToDecimal(area[0]) / 2.5m);
                        width = Math.Round(Convert.ToDecimal(area[1]) / 2.5m);
                        //AreaSqftPerPcs = Math.Round(((length * width) / 144m), 4); //sqft
                        //AreaSqft = Math.Round(AreaSqftPerPcs * item.Quantity, 4);

                        row["SizeName"] = length + "x" + width;

                    }
                    else
                    {
                        row["SizeName"] = item.SizeName;

                    }


                    dt.Rows.Add(row);
                    SOrderID = item.SOrderID;



                    //if (item.CategoryName.ToLower().Equals("tiles"))
                    //{
                    //    //var area = item.SizeName.Split('x');
                    //    //length = Math.Round(Convert.ToDecimal(area[0]) / 2.5m);
                    //    //width = Math.Round(Convert.ToDecimal(area[1]) / 2.5m);
                    //    //AreaSqftPerPcs = Math.Round(((length * width) / 144m), 4); //sqft
                    //    //AreaSqft = Math.Round(AreaSqftPerPcs * item.Quantity, 4);
                    //    //row["TotalArea"] = item.TotalSFT;
                    //    //row["SizeName"] = length + "x" + width;

                    //    row["TotalPrice"] = (item.Quantity * (item.SalesPerCartonSft / item.ConvertValue) * item.UnitPrice);
                    //}
                    //else
                    //{
                    //    row["SizeName"] = "N/A";
                    //    row["TotalPrice"] = ((item.Quantity / item.ConvertValue) * item.MRP);
                    //}


                }

                //For Credit Sales
                //foreach (var grd in CreditsalseDetailInfos)
                //{
                //    //StockDetail std = oSTDList.FirstOrDefault(x => x.SDetailID == grd.StockDetailID);

                //    if (CreditSaleID != grd.Rest.Rest.Item1)
                //    {
                //        TotalDueSales = TotalDueSales + (decimal)grd.Rest.Item4;
                //        GrandTotal = GrandTotal + (decimal)grd.Item7;
                //        TotalDis = TotalDis + (decimal)grd.Rest.Item1;
                //        NetTotal = NetTotal + (decimal)grd.Rest.Item2;
                //        RecAmt = RecAmt + (decimal)grd.Rest.Item3;
                //        CurrDue = CurrDue + (decimal)grd.Rest.Item4;
                //    }

                //    CreditSaleID = grd.Rest.Rest.Item1;
                //    dt.Rows.Add(grd.Item1.ToString("dd MMM yyyy"), grd.Item2, grd.Item3, grd.Item4, grd.Item5, grd.Item6, grd.Item7, grd.Rest.Item1, grd.Rest.Item2, grd.Rest.Item3, grd.Rest.Item4, grd.Rest.Item5, grd.Rest.Item6, grd.Rest.Item7, "Credit Sales", 0m);

                //}




                decimal ReturnTotalDueSales = 0, ReturnAdjAmount = 0;
                decimal ReturnGrandTotal = 0;
                decimal ReturnTotalDis = 0;
                decimal ReturnNetTotal = 0;
                decimal ReturnRecAmt = 0;
                decimal ReturnCurrDue = 0;


                foreach (var grd in returnDetailInfos)
                {
                    //StockDetail std = oSTDList.FirstOrDefault(x => x.SDetailID == grd.StockDetailID);

                    if (SOrderID != grd.Rest.Rest.Item1)
                    {
                        ReturnTotalDueSales = ReturnTotalDueSales + (decimal)grd.Rest.Item4;
                        ReturnGrandTotal = ReturnGrandTotal + (decimal)grd.Item7;
                        ReturnTotalDis = ReturnTotalDis + (decimal)grd.Rest.Item1;
                        ReturnNetTotal = ReturnNetTotal + (decimal)grd.Rest.Item2;
                        ReturnRecAmt = ReturnRecAmt + (decimal)grd.Rest.Item3;
                        ReturnCurrDue = ReturnCurrDue + (decimal)grd.Rest.Item4;
                        ReturnAdjAmount = ReturnAdjAmount + grd.Rest.Rest.Item2;
                    }

                    SOrderID = grd.Rest.Rest.Item1;
                    dtReturn.Rows.Add(grd.Item1.ToString("dd MMM yyyy"), grd.Item2, grd.Item3, grd.Item4, grd.Item5, grd.Item6, grd.Item7, grd.Rest.Item1, grd.Rest.Item2, grd.Rest.Item3, grd.Rest.Item4, grd.Rest.Item5, grd.Rest.Item6, grd.Rest.Item7, "Sales", grd.Rest.Rest.Item2);

                }



                decimal NetTotalDueSales = 0, NetAdjAmount = 0;
                decimal NetGrandTotal = 0;
                decimal NetTotalDis = 0;
                decimal NetNetTotal = 0;
                decimal NetRecAmt = 0;
                decimal NetCurrDue = 0;


                NetTotalDueSales = TotalDueSales - ReturnTotalDueSales;
                NetAdjAmount = AdjAmount - ReturnAdjAmount;
                NetGrandTotal = GrandTotal - ReturnGrandTotal;
                NetTotalDis = TotalDis - ReturnTotalDis;
                NetNetTotal = NetTotal - ReturnNetTotal;
                NetRecAmt = RecAmt - ReturnRecAmt;
                NetCurrDue = CurrDue - ReturnCurrDue;

                dt.TableName = "dtCustomerWiseSales";
                _dataSet = new DataSet();
                _dataSet.Tables.Add(dt);


                dtReturn.TableName = "dtCustomerWiseReturn";

                _dataSet.Tables.Add(dtReturn);
                GetCommonParameters(userName, UserConcernID);
                _reportParameter = new ReportParameter("Date", "Sales details for the date from : " + fromDate.ToString("dd MMM yyyy") + " To " + toDate.ToString("dd MMM yyyy"));
                _reportParameters.Add(_reportParameter);
                _reportParameter = new ReportParameter("GrandTotal", GrandTotal.ToString());
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("TotalDis", TotalDis.ToString());
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("NetTotal", NetTotal.ToString());
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("RecAmt", RecAmt.ToString());
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("CurrDue", CurrDue.ToString());
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("AdjAmount", AdjAmount.ToString());
                _reportParameters.Add(_reportParameter);



                _reportParameter = new ReportParameter("ReturnGrandTotal", ReturnGrandTotal.ToString());
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("ReturnTotalDis", ReturnTotalDis.ToString());
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("ReturnNetTotal", ReturnNetTotal.ToString());
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("ReturnRecAmt", ReturnRecAmt.ToString());
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("ReturnCurrDue", ReturnCurrDue.ToString());
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("ReturnAdjAmount", ReturnAdjAmount.ToString());
                _reportParameters.Add(_reportParameter);




                _reportParameter = new ReportParameter("NetGrandTotal", NetGrandTotal.ToString());
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("NetTotalDis", NetTotalDis.ToString());
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("NetNetTotal", NetNetTotal.ToString());
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("NetRecAmt", NetRecAmt.ToString());
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("NetCurrDue", NetCurrDue.ToString());
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("NetAdjAmount", NetAdjAmount.ToString());
                _reportParameters.Add(_reportParameter);


                return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Sales\\rptSalesDetailsAdmin.rdlc");
            }
            #endregion


        }

        public byte[] AdminCustomerDueRpt(string userName, int concernID, int UserCocernID, int CustomerType, int DueType)
        {
            try
            {
                var customerDueInfo = _customerService.AdminCustomerDueReport(concernID, CustomerType, DueType);

                TransactionalDataSet.dtCustomerDataTable dt = new TransactionalDataSet.dtCustomerDataTable();
                _dataSet = new DataSet();
                DataRow row = null;
                foreach (var item in customerDueInfo)
                {
                    row = dt.NewRow();
                    row["CCode"] = item.Code;
                    row["CName"] = item.Name;
                    row["CompanyName"] = item.CompanyName;
                    row["CusType"] = item.CustomerType;
                    row["ContactNo"] = item.ContactNo;
                    row["NID"] = "";
                    row["Address"] = item.Address;
                    row["TotalDue"] = item.TotalDue;
                    row["ConcernName"] = item.ConcernName;
                    dt.Rows.Add(row);
                }

                dt.TableName = "dtCustomer";
                _dataSet.Tables.Add(dt);

                GetCommonParameters(userName, UserCocernID);
                if (DueType == 1)
                {
                    _reportParameter = new ReportParameter("ReportHeader", "Only Due Customer List.");
                    _reportParameters.Add(_reportParameter);
                }
                else
                {
                    _reportParameter = new ReportParameter("ReportHeader", "Customer List.");
                    _reportParameters.Add(_reportParameter);
                }


                return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Admin\\rptAdminCustomer.rdlc");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public byte[] AdminCashCollectionReport(string userName, int concernID, int UserCocernID, DateTime fromDate, DateTime toDate)
        {
            try
            {
                var CashColletions = _CashCollectionService.AdminCashCollectionReport(fromDate, toDate, concernID).ToList();
                var BankCollection = _bankTransactionService.AdminCashCollectionByBank(concernID, fromDate, toDate).ToList();
                var CashSales = _salesOrderService.GetAdminSalesReport(concernID, fromDate, toDate);
                var fcashsales = (from s in CashSales
                                  select new CashCollectionReportModel
                                  {
                                      EntryDate = s.InvoiceDate,
                                      CustomerName = s.CustomerName,
                                      CustomerCode = s.CustomerCode,
                                      Address = s.CustomerAddress,
                                      ContactNo = s.CustomerContactNo,
                                      TotalDue = s.CustomerTotalDue,
                                      Amount = s.RecAmount,
                                      AdjustAmt = s.AdjAmount,
                                      ModuleType = "Cash Sales",
                                      AccountNo = "N/A",
                                      ReceiptNo = s.InvoiceNo,
                                      Remarks = "",
                                      ConcernName = s.ConcernName
                                  }).ToList();
                var DownPayments = _creditSalesOrderService.GetAdminCrSalesReport(concernID, fromDate, toDate);
                var fDownPayments = (from s in DownPayments
                                     select new CashCollectionReportModel
                                     {
                                         EntryDate = s.InvoiceDate,
                                         CustomerName = s.CustomerName,
                                         CustomerCode = s.CustomerCode,
                                         Address = s.CustomerAddress,
                                         ContactNo = s.CustomerContactNo,
                                         TotalDue = s.CustomerTotalDue,
                                         Amount = s.RecAmount,
                                         AdjustAmt = s.AdjAmount,
                                         ModuleType = "Downpayment",
                                         AccountNo = "N/A",
                                         ReceiptNo = s.InvoiceNo,
                                         Remarks = "",
                                         ConcernName = s.ConcernName
                                     }).ToList();

                var installments = _creditSalesOrderService.AdminInstallmentColllections(concernID, fromDate, toDate);
                CashColletions.AddRange(BankCollection);
                CashColletions.AddRange(fcashsales);
                CashColletions.AddRange(fDownPayments);
                CashColletions.AddRange(installments);
                TransactionalDataSet.dtCollectionRptDataTable dt = new TransactionalDataSet.dtCollectionRptDataTable();
                _dataSet = new DataSet();
                DataRow row = null;
                var AllCollections = CashColletions.OrderByDescending(i => i.EntryDate);
                foreach (var item in AllCollections)
                {
                    row = dt.NewRow();
                    row["CollDate"] = item.EntryDate;
                    row["CName"] = item.CustomerName;
                    row["CAddress"] = item.ContactNo + " & " + item.Address;
                    row["CContact"] = item.ContactNo;
                    row["TotalDue"] = item.TotalDue;
                    row["RecAmt"] = item.Amount;
                    row["RemainigAmt"] = item.TotalDue;
                    row["AdjustmentAmt"] = item.AdjustAmt;
                    row["CashType"] = item.ModuleType;
                    row["BankName"] = item.BankName;
                    row["AccountNo"] = item.AccountNo;
                    row["BranchName"] = item.BranchName;
                    row["ChequeNo"] = item.ChecqueNo;
                    row["EmployeeName"] = "";
                    row["InvoiceNo"] = item.ReceiptNo;
                    row["ConcernName"] = item.ConcernName;
                    dt.Rows.Add(row);
                }

                dt.TableName = "dtCollectionRpt";
                _dataSet.Tables.Add(dt);

                GetCommonParameters(userName, UserCocernID);
                _reportParameter = new ReportParameter("Month", "Cash Collection from Date " + fromDate.ToString("dd MMM yyyy") + " to " + toDate.ToString("dd MMM yyyy"));
                _reportParameters.Add(_reportParameter);


                return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Admin\\rptAdminCollectionRpt.rdlc");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        public byte[] CashInHandReport(string userName, int concernID, int ReportType, DateTime fromDate, DateTime toDate, int CustomerType)
        {
            var CashInHandData = _CashCollectionService.CashInHandReport(fromDate, toDate, ReportType, concernID, CustomerType).ToList();
            TransactionalDataSet.dtCashInHandDataTable dt = new TransactionalDataSet.dtCashInHandDataTable();
            _dataSet = new DataSet();
            double TotalPayable = 0;
            double TotalRecivable = 0;

            double OpeningCashInhand = 0;
            double CurrentCashInhand = 0;
            double ClosingCashInhand = 0;

            var DataForTable = CashInHandData.Where(o => o.Expense != "Total Payable" && o.Income != "Total Receivable" && o.Expense != "Current Cash In Hand" && o.Income != "Closing Cash In Hand" && o.Income != "Opening Cash In Hand").ToList();
            var DataForTotal = CashInHandData.Where(o => o.Expense == "Total Payable" && o.Income == "Total Receivable").ToList();
            foreach (var item in DataForTable)
            {
                dt.Rows.Add(item.TransDate, item.id, item.Expense, item.ExpenseAmt, item.Income, item.IncomeAmt, item.Module, item.EmployeeName);
            }

            dt.TableName = "dtCashInHand";
            _dataSet = new DataSet();
            _dataSet.Tables.Add(dt);
            GetCommonParameters(userName, concernID);

            TotalPayable = (double)DataForTotal.Sum(o => o.ExpenseAmt);
            TotalRecivable = (double)DataForTotal.Where(i => !(i.Expense.Equals("Header"))).Sum(o => o.IncomeAmt);
            OpeningCashInhand = (double)CashInHandData.Where(o => o.Income == "Opening Cash In Hand").ToList().Sum(o => o.IncomeAmt);
            CurrentCashInhand = (double)CashInHandData.Where(o => o.Expense == "Current Cash In Hand").ToList().Sum(o => o.ExpenseAmt);
            ClosingCashInhand = (double)CashInHandData.Where(o => o.Expense == "Closing Cash In Hand").ToList().Sum(o => o.ExpenseAmt);

            _reportParameter = new ReportParameter("TotalPayable", TotalPayable.ToString("0.00"));
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("TotalRecivable", TotalRecivable.ToString("0.00"));
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("OpeningCashInhand", OpeningCashInhand.ToString("0.00"));
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("CurrentCashInhand", CurrentCashInhand.ToString("0.00"));
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("ClosingCashInHand", ClosingCashInhand.ToString("0.00"));
            _reportParameters.Add(_reportParameter);

            if (ReportType == 1)
                _reportParameter = new ReportParameter("DateRange", "CashIn Hand of the date " + fromDate.ToString("dd MMM yyyy"));
            else if (ReportType == 2)
                _reportParameter = new ReportParameter("DateRange", "CashIn Hand of the month  " + fromDate.ToString("MMM yyyy"));
            else if (ReportType == 3)
                _reportParameter = new ReportParameter("DateRange", "CashIn Hand of the year  " + fromDate.ToString("yyyy"));

            _reportParameters.Add(_reportParameter);

            //_reportParameter = new ReportParameter("DepartmentName", "Department: " + DepartmentName);
            //_reportParameters.Add(_reportParameter);

            if (concernID == (int)EnumSisterConcern.KINGSTAR_CONCERNID)
                return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Others\\CashInHand\\rptKSDailyCashINHand.rdlc");
            else
                return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Others\\CashInHand\\rptDailyCashINHand.rdlc");

        }


        public byte[] CashInHandReportAdmin(string userName, int concernID, int ReportType, DateTime fromDate, DateTime toDate, int CustomerType)
        {
            var CashInHandData = _CashCollectionService.CashInHandReport(fromDate, toDate, ReportType, concernID, CustomerType).ToList();
            TransactionalDataSet.dtCashInHandDataTable dt = new TransactionalDataSet.dtCashInHandDataTable();
            _dataSet = new DataSet();
            double TotalPayable = 0;
            double TotalRecivable = 0;

            double OpeningCashInhand = 0;
            double CurrentCashInhand = 0;
            double ClosingCashInhand = 0;

            var DataForTable = CashInHandData.Where(o => o.Expense != "Total Payable" && o.Income != "Total Receivable" && o.Expense != "Current Cash In Hand" && o.Income != "Closing Cash In Hand" && o.Income != "Opening Cash In Hand").ToList();
            var DataForTotal = CashInHandData.Where(o => o.Expense == "Total Payable" && o.Income == "Total Receivable").ToList();
            foreach (var item in DataForTable)
            {
                dt.Rows.Add(item.TransDate, item.id, item.Expense, item.ExpenseAmt, item.Income, item.IncomeAmt, item.Module, item.EmployeeName);
            }

            dt.TableName = "dtCashInHand";
            _dataSet = new DataSet();
            _dataSet.Tables.Add(dt);
            GetCommonParameters(userName, concernID);

            TotalPayable = (double)DataForTotal.Sum(o => o.ExpenseAmt);
            TotalRecivable = (double)DataForTotal.Sum(o => o.IncomeAmt);
            OpeningCashInhand = (double)CashInHandData.Where(o => o.Income == "Opening Cash In Hand").ToList().Sum(o => o.IncomeAmt);
            CurrentCashInhand = (double)CashInHandData.Where(o => o.Expense == "Current Cash In Hand").ToList().Sum(o => o.ExpenseAmt);
            ClosingCashInhand = (double)CashInHandData.Where(o => o.Expense == "Closing Cash In Hand").ToList().Sum(o => o.ExpenseAmt);

            _reportParameter = new ReportParameter("TotalPayable", TotalPayable.ToString("0.00"));
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("TotalRecivable", TotalRecivable.ToString("0.00"));
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("OpeningCashInhand", OpeningCashInhand.ToString("0.00"));
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("CurrentCashInhand", CurrentCashInhand.ToString("0.00"));
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("ClosingCashInHand", ClosingCashInhand.ToString("0.00"));
            _reportParameters.Add(_reportParameter);

            if (ReportType == 1)
                _reportParameter = new ReportParameter("DateRange", "CashIn Hand of the date " + fromDate.ToString("dd MMM yyyy"));
            else if (ReportType == 2)
                _reportParameter = new ReportParameter("DateRange", "CashIn Hand of the month  " + fromDate.ToString("MMM yyyy"));
            else if (ReportType == 3)
                _reportParameter = new ReportParameter("DateRange", "CashIn Hand of the year  " + fromDate.ToString("yyyy"));

            _reportParameters.Add(_reportParameter);

            //_reportParameter = new ReportParameter("DepartmentName", "Department: " + DepartmentName);
            //_reportParameters.Add(_reportParameter);

            if (concernID == (int)EnumSisterConcern.KINGSTAR_CONCERNID)
                return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Others\\CashInHand\\rptKSDailyCashINHand.rdlc");
            else
                return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Others\\CashInHand\\rptDailyCashINHand.rdlc");

        }

        public byte[] BankTransMoneyReceipt(string userName, int concernID, int BankTranID)
        {
            try
            {
                string Code = string.Empty, Name = string.Empty, ContactNo = string.Empty;
                decimal TotalDue = 0;
                var BankTrans = _bankTransactionService.GetBankTransactionById(BankTranID);
                var BankInfo = _BankService.GetBankById(BankTrans.BankID);
                if (BankTrans.CustomerID != 0)
                {
                    var Customer = _customerService.GetCustomerById((int)BankTrans.CustomerID);
                    Code = Customer.Code;
                    Name = Customer.Name;
                    ContactNo = Customer.ContactNo + " & " + Customer.Address;
                    TotalDue = Customer.TotalDue;
                }
                else if (BankTrans.SupplierID != 0)
                {
                    var Supplier = _SupplierService.GetSupplierById((int)BankTrans.SupplierID);
                    Code = Supplier.Code;
                    Name = Supplier.Name;
                    ContactNo = Supplier.ContactNo + " & " + Supplier.Address;
                    TotalDue = Supplier.TotalDue;
                }

                TransactionalDataSet.dtBankTransactionDataTable dt = new TransactionalDataSet.dtBankTransactionDataTable();
                _dataSet = new DataSet();
                DataRow row = null;


                row = dt.NewRow();

                row["TranDate"] = BankTrans.TranDate.Value.ToString("dd MMM yyyy");
                row["TransactionNo"] = BankTrans.TransactionNo;
                row["TransactionType"] = (EnumTransactionType)BankTrans.TransactionType;
                row["BankName"] = BankInfo.BankName;
                row["Amount"] = BankTrans.Amount;
                row["Remarks"] = BankTrans.Remarks;
                row["ChecqueNo"] = BankTrans.ChecqueNo;
                row["BranchName"] = BankInfo.BranchName;
                row["AccountNO"] = BankInfo.AccountNo;
                row["AccountName"] = BankInfo.AccountName;
                row["CustomerName"] = Name;
                row["CustomerCode"] = Code;
                row["TotalDue"] = TotalDue;
                row["ContactNo"] = ContactNo;
                dt.Rows.Add(row);
                dt.TableName = "BankTransaction";
                _dataSet.Tables.Add(dt);
                GetCommonParameters(userName, concernID);
                _reportParameter = new ReportParameter("PrintDate", GetClientDateTime());
                _reportParameters.Add(_reportParameter);

                return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Bank\\rptBTransMoneyReceipt.rdlc");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public byte[] ExpenseIncomeMoneyReceipt(string userName, int concernID, int ExpenditureID, bool IsExpense)
        {
            try
            {
                var expenditures = _expenditureService.GetExpenditureById(ExpenditureID);
                var head = _ExpenseItemService.GetExpenseItemById(expenditures.ExpenseItemID);

                TransactionalDataSet.dtExpenditureDataTable dt = new TransactionalDataSet.dtExpenditureDataTable();
                _dataSet = new DataSet();
                DataRow row = null;

                row = dt.NewRow();

                row["ExpDate"] = expenditures.EntryDate.ToString("dd MMM yyyy");
                row["Description"] = expenditures.Purpose;
                row["Amount"] = expenditures.Amount;
                row["ItemName"] = head.Description;
                row["VoucherNo"] = expenditures.VoucherNo;
                row["UserName"] = expenditures.CreatedBy;
                dt.Rows.Add(row);
                dt.TableName = "dtExpenditure";
                _dataSet.Tables.Add(dt);
                GetCommonParameters(userName, concernID);
                _reportParameter = new ReportParameter("PrintDate", GetClientDateTime());
                _reportParameters.Add(_reportParameter);
                if (IsExpense)
                    _reportParameter = new ReportParameter("ReportHeader", "Expense Money Receipt");
                else
                    _reportParameter = new ReportParameter("ReportHeader", "Income Money Receipt");
                _reportParameters.Add(_reportParameter);

                return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Others\\rptEXMoneyReceipt.rdlc");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public byte[] DailyAttendence(string userName, int concernID, int DepartmentID, DateTime Date, bool IsPresent, bool IsAbsent)
        {
            IQueryable<Department> departments = null;
            if (DepartmentID == 0)
                departments = _DepartmentService.GetAllDepartmentIQueryable().Where(i => i.Status == (int)EnumActiveInactive.Active);
            else
                departments = _DepartmentService.GetAllDepartmentIQueryable().Where(i => i.DepartmentId == DepartmentID);

            List<AttendencReportModel> AttendenceList = new List<AttendencReportModel>();
            int TotalEmployee = 0, PresentEmployee = 0, AbsentEmployee = 0;
            IQueryable<Employee> ActiveEmployees = _EmployeeService.GetAllEmployeeIQueryable().Where(i => i.Status == EnumActiveInactive.Active);
            if (IsPresent)
            {
                var Presents = (from da in _attendenceService.GetDetails().Where(i => i.Date == Date.Date)
                                join emp in ActiveEmployees on da.AccountNo equals emp.MachineEMPID
                                join dept in departments on emp.DepartmentID equals dept.DepartmentId
                                join desg in _DesignationService.GetAllIQueryable() on emp.DesignationID equals desg.DesignationID
                                where !string.IsNullOrEmpty(da.ClockIn)
                                select new AttendencReportModel
                                {
                                    AccountNo = emp.MachineEMPID,
                                    EmployeeName = emp.Name,
                                    Designation = desg.Description,
                                    DepartmentName = dept.DESCRIPTION,
                                    ClockIn = da.ClockIn,
                                    ClockOut = da.ClockOut,
                                    Absent = da.Absent
                                }).ToList();
                AttendenceList.AddRange(Presents);
                PresentEmployee = Presents.Count();
            }

            if (IsAbsent)
            {
                var absents = (from da in _attendenceService.GetDetails().Where(i => i.Date == Date.Date)
                               join emp in ActiveEmployees on da.AccountNo equals emp.MachineEMPID
                               join dept in departments on emp.DepartmentID equals dept.DepartmentId
                               join desg in _DesignationService.GetAllIQueryable() on emp.DesignationID equals desg.DesignationID
                               where string.IsNullOrEmpty(da.ClockIn)
                               select new AttendencReportModel
                               {
                                   AccountNo = emp.MachineEMPID,
                                   EmployeeName = emp.Name,
                                   Designation = desg.Description,
                                   DepartmentName = dept.DESCRIPTION,
                                   ClockIn = da.ClockIn,
                                   ClockOut = da.ClockOut,
                                   Absent = da.Absent
                               }).ToList();
                AttendenceList.AddRange(absents);
                AbsentEmployee = absents.Count();
            }
            TotalEmployee = PresentEmployee + AbsentEmployee;
            TransactionalDataSet.dtDailyAttendenceDataTable dt = new TransactionalDataSet.dtDailyAttendenceDataTable();

            _dataSet = new DataSet();
            DataRow row = null;
            AttendenceList = AttendenceList.OrderBy(i => i.AccountNo).ToList();
            foreach (var item in AttendenceList)
            {
                row = dt.NewRow();
                row["AccountNo"] = item.AccountNo;
                row["Name"] = item.EmployeeName;
                row["Designation"] = item.Designation;
                row["Department"] = item.DepartmentName;
                row["ClockIn"] = item.ClockIn;
                row["ClockOut"] = item.ClockOut;
                dt.Rows.Add(row);
            }

            _dataSet.Tables.Add(dt);
            dt.TableName = "dtDailyAttendence";

            GetCommonParameters(userName, concernID);

            _reportParameter = new ReportParameter("ReportHeading", "Attendence report of the date : " + Date.ToString("dd MMM yyyy"));
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("TotalEmployee", "Total Employee: " + TotalEmployee.ToString());
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("Present", "Present: " + PresentEmployee.ToString());
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("Absent", "Absent: " + AbsentEmployee.ToString());
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("PrintDate", GetClientDateTime());
            _reportParameters.Add(_reportParameter);

            return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "HRPR\\rptDailyAttendence.rdlc");
        }


        public byte[] StockLedgerReport(DateTime fromDate, DateTime toDate, string userName, int concernID, int reportType, int CompanyID, int CategoryID, int ProductID)
        {
            try
            {

                List<StockLedger> DataGroupBy = _StockServce.GetStockLedgerReport(reportType, CompanyID, CategoryID, ProductID, fromDate, toDate).ToList();
                DataRow row = null;
                string reportName = string.Empty;

                TransactionalDataSet.dtDailyStockandSalesSummaryNewDataTable dt = new TransactionalDataSet.dtDailyStockandSalesSummaryNewDataTable();

                string IMENO = "";
                int count;
                //StockDetails = _stockdetailService.GetAll();
                foreach (var item in DataGroupBy)
                {

                    decimal OpeningQty =
                        (item.PreviousPurchaseQuantity + item.PreviousProductionQuantity - item.PreviousPurchaseReturnQuantity - item.PreviousRawDeductQuantity - item.PreviousSalesQuantity + item.PreviousSalesReturnQuantity);

                    decimal ClosingQty = (item.PreviousPurchaseQuantity + item.PreviousProductionQuantity - item.PreviousPurchaseReturnQuantity - item.PreviousRawDeductQuantity - item.PreviousSalesQuantity + item.PreviousSalesReturnQuantity)
                     + (item.PurchaseQuantity + item.ProductionQuantity - item.PurchaseReturnQuantity - item.RawDeductQuantity - item.SalesQuantity + item.SalesReturnQuantity);

                    if (OpeningQty == 0 && ClosingQty == 0)
                    {

                    }
                    else
                    {
                        dt.Rows.Add(DateTime.Now,
                                                1,
                                               item.ProductID,
                                               item.Code,
                                               item.ProductName,
                                               item.ColorID,
                                               item.ColorName,
                                              OpeningQty,
                                               (item.PurchaseQuantity + item.ProductionQuantity - item.PurchaseReturnQuantity - item.RawDeductQuantity - item.SalesQuantity + item.SalesReturnQuantity),
                                               item.PurchaseQuantity,
                                               item.SalesQuantity,
                                               item.SalesReturnQuantity,
                                                ClosingQty,
                                               0,
                                               0,
                                               0,
                                               item.RawDeductQuantity,
                                               item.ProductionQuantity
                                               );
                    }

                }

                dt.TableName = "dtDailyStockandSalesSummary";


                _dataSet = new DataSet();
                _dataSet.Tables.Add(dt);

                GetCommonParameters(userName, concernID);

                reportName = "Stock\\rptStockLedger.rdlc";

                _reportParameter = new ReportParameter("DateRange", "Stock Ledger From : " + fromDate.ToString("dd MMM yyyy") + " to " + toDate.ToString("dd MMM yyyy"));
                _reportParameters.Add(_reportParameter);



                return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, reportName);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public byte[] SupplierLedger(string userName, int concernID, DateTime fromDate, DateTime toDate, int SupplierID)
        {

            TransactionalDataSet.dtCustomerLedgerAccountDataTable dt = new TransactionalDataSet.dtCustomerLedgerAccountDataTable();
            _dataSet = new DataSet();
            DataRow row = null;
            decimal LastBalance = 0m;
            int Counter = 0;
            var ledgers = _purchaseOrderService.SupplierLedger(fromDate, toDate, SupplierID);
            var Supplier = _SupplierService.GetSupplierById(SupplierID);
            foreach (var item in ledgers)
            {
                Counter++;
                row = dt.NewRow();
                row["Date"] = item.Date;
                row["Particulars"] = item.Particulars;
                row["VoucherType"] = item.VoucherType;
                row["InvoiceNo"] = item.InvoiceNo;
                row["Debit"] = item.Debit;
                row["DebitAdj"] = item.DebitAdj;
                row["Credit"] = item.Credit;
                row["CreditAdj"] = item.CreditAdj;
                row["GrandTotal"] = item.GrandTotal;
                row["CashCollection"] = item.CashCollectionAmt;
                row["SalesReturn"] = item.SalesReturn;
                row["Balance"] = item.Balance;
                row["Remarks"] = item.Remarks;
                row["CashCollectionIntAmt"] = item.CashCollectionIntAmt;

                if (ledgers.Count() == Counter)
                    LastBalance = item.Balance;

                dt.Rows.Add(row);
            }

            dt.TableName = "dtCustomerLedgerAccount";
            _dataSet.Tables.Add(dt);

            GetCommonParameters(userName, concernID);
            _reportParameter = new ReportParameter("CustomerName", Supplier.Name + " (" + Supplier.Code + ")");
            _reportParameters.Add(_reportParameter);
            _reportParameter = new ReportParameter("CustomerAddress", Supplier.Address);
            _reportParameters.Add(_reportParameter);
            _reportParameter = new ReportParameter("CustomerContact", "Contact: " + Supplier.ContactNo);
            _reportParameters.Add(_reportParameter);
            _reportParameter = new ReportParameter("LastBalance", LastBalance.ToString());
            _reportParameters.Add(_reportParameter);
            _reportParameter = new ReportParameter("DateRange", fromDate.ToString("dd MMM yyyy") + " to " + toDate.ToString("dd MMM yyyy"));
            _reportParameters.Add(_reportParameter);

            return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Purchase\\rptSupplierLedgerAccount.rdlc");
        }


        public byte[] CustomerLedger(DateTime fromDate, DateTime toDate, string userName, int concernID, int CustomerID)
        {
            var customerLedgerdata = _salesOrderService.CustomerLedger(fromDate, toDate, CustomerID);
            var Customer = _customerService.GetCustomerById(CustomerID);
            TransactionalDataSet.dtCustomerLedgerAccountDataTable dt = new TransactionalDataSet.dtCustomerLedgerAccountDataTable();
            _dataSet = new DataSet();
            DataRow row = null;
            decimal LastBalance = 0m;
            int Counter = 0;
            foreach (var item in customerLedgerdata)
            {
                Counter++;
                row = dt.NewRow();
                row["Date"] = item.Date;
                row["Particulars"] = item.Particulars;
                row["VoucherType"] = item.VoucherType;
                row["InvoiceNo"] = item.InvoiceNo;
                row["Debit"] = item.Debit;
                row["DebitAdj"] = item.DebitAdj;
                row["Credit"] = item.Credit;
                row["CreditAdj"] = item.CreditAdj;
                row["GrandTotal"] = item.GrandTotal;
                row["CashCollection"] = item.CashCollectionAmt;
                row["SalesReturn"] = item.SalesReturn;
                row["Balance"] = item.Balance;
                row["Remarks"] = item.Remarks;
                row["CashCollectionReturn"] = item.CashCollectionReturn;
                row["CashCollectionIntAmt"] = item.CashCollectionIntAmt;
                if (item.Terms != 0)
                {
                    row["Terms"] = item.Terms;
                }
                else
                {
                    row["Terms"] = " ";
                }

                if (customerLedgerdata.Count() == Counter)
                    LastBalance = item.Balance;
                dt.Rows.Add(row);
            }

            dt.TableName = "dtCustomerLedgerAccount";
            _dataSet.Tables.Add(dt);

            GetCommonParameters(userName, concernID);
            _reportParameter = new ReportParameter("CustomerName", Customer.Name + " (" + Customer.Code + ")");
            _reportParameters.Add(_reportParameter);
            _reportParameter = new ReportParameter("CustomerAddress", Customer.Address);
            _reportParameters.Add(_reportParameter);
            _reportParameter = new ReportParameter("CustomerContact", "Contact: " + Customer.ContactNo);
            _reportParameters.Add(_reportParameter);
            _reportParameter = new ReportParameter("LastBalance", LastBalance.ToString());
            _reportParameters.Add(_reportParameter);
            _reportParameter = new ReportParameter("DateRange", fromDate.ToString("dd MMM yyyy") + " to " + toDate.ToString("dd MMM yyyy"));
            _reportParameters.Add(_reportParameter);

            return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Sales\\rptCustomerLedgerAccount.rdlc");

        }


        public byte[] SalesReturnChallan(int oOrderID, string userName, int concernID)
        {
            ROrder oOrder = new ROrder();
            oOrder.ROrderDetails = _returnDetailOrderService.GetDetailsByID(Convert.ToInt32(oOrderID));
            oOrder = _returnOrderService.GetReturnOrderById(Convert.ToInt32(oOrderID));
            return SalesReturnChallanPrint(oOrder, userName, concernID);
        }

        public byte[] SalesReturnChallanPrint(ROrder oOrder, string userName, int concernID)
        {
            try
            {

                DataTable orderdDT = new DataTable();
                TransactionalDataSet.dtInvoiceDataTable dt = new TransactionalDataSet.dtInvoiceDataTable();
                TransactionalDataSet.dtWarrentyDataTable dtWarrenty = new TransactionalDataSet.dtWarrentyDataTable();
                Customer customer = _customerService.GetCustomerById(oOrder.CustomerID);

                //List<ProductWisePurchaseModel> warrentyList = new List<ProductWisePurchaseModel>();

                string Warrenty = string.Empty;
                string IMEIs = string.Empty;
                DataRow row = null;
                decimal AreaPerCarton = 0m, Length = 0m, Width = 0m, AreaPerPcs = 0m;
                decimal[] sizeXY = null;
                int Count = 0;
                #region LINQ
                var ProductInfos = from sd in oOrder.ROrderDetails
                                   join std in _stockdetailService.GetAll() on sd.StockDetailID equals std.SDetailID
                                   join col in _ColorServce.GetAllColor() on std.ColorID equals col.ColorID
                                   join p in _productService.GetProducts() on sd.ProductID equals p.ProductID
                                   select new
                                   {
                                       ProductID = p.ProductID,
                                       ProductName = p.ProductName,
                                       p.SalesCSft,
                                       p.ProductCode,
                                       ParentUnit = p.ParentUnitName,
                                       ChildUnit = p.ChildUnitName,
                                       Quantity = sd.Quantity,
                                       UnitPrice = sd.UnitPrice,
                                       SalesRate = sd.UTAmount,
                                       UTAmount = sd.UTAmount,
                                       ColorName = col.Name,
                                       CompanyName = p.CompanyName,
                                       CategoryName = p.CategoryName,
                                       p.SizeName,
                                       ConvertValue = p.ConvertValue,
                                       sd.TotalSFT,
                                       sd.SFTRate
                                   };

                var GroupProductInfos = from w in ProductInfos
                                        group w by new
                                        {
                                            w.ProductName,
                                            w.CategoryName,
                                            w.ColorName,
                                            w.CompanyName,
                                            w.UnitPrice,
                                            w.ConvertValue,
                                            w.ProductCode,
                                            w.ParentUnit,
                                            w.ChildUnit,
                                            w.SizeName,
                                            w.SalesCSft,
                                            w.SFTRate
                                        } into g
                                        select new
                                        {
                                            ProductName = g.Key.ProductName,
                                            CategoryName = g.Key.CategoryName,
                                            ColorName = g.Key.ColorName,
                                            CompanyName = g.Key.CompanyName,
                                            g.Key.SizeName,
                                            g.Key.ProductCode,
                                            g.Key.ParentUnit,
                                            g.Key.ChildUnit,
                                            g.Key.SalesCSft,
                                            UnitPrice = g.Key.UnitPrice,
                                            SalesRate = g.Key.UnitPrice,
                                            Quantity = g.Sum(i => i.Quantity),
                                            TotalAmt = g.Sum(i => i.UTAmount),
                                            g.Key.ConvertValue,
                                            g.Key.SFTRate,
                                            TotalSFT = g.Select(i => i.TotalSFT).FirstOrDefault()
                                        };
                #endregion

                foreach (var item in GroupProductInfos)
                {
                    #region code for IMEI
                    //foreach (var IMEI in item.IMENOs)
                    //{
                    //    Count++;
                    //    if (item.IMENOs.Count() != Count)
                    //        IMEIs = IMEIs + IMEI + Environment.NewLine;
                    //    else
                    //        IMEIs = IMEIs + IMEI;
                    //}

                    //dt.Rows.Add(item.ProductName, item.Quantity, "Pcs", item.UnitPrice, "0 %", item.TotalAmt, item.PPDPercentage, item.PPDAmount, IMEIs, item.ColorName, "", item.PPOffer, item.CompanyName + " & " + item.CategoryName);

                    //if (!string.IsNullOrEmpty(item.Compressor))
                    //    Warrenty = "Compressor: " + item.Compressor + Environment.NewLine;
                    //if (!string.IsNullOrEmpty(item.Motor))
                    //    Warrenty = Warrenty + "Motor: " + item.Motor + Environment.NewLine;
                    //if (!string.IsNullOrEmpty(item.Panel))
                    //    Warrenty = Warrenty + "Panel: " + item.Panel + Environment.NewLine;
                    //if (!string.IsNullOrEmpty(item.Service))
                    //    Warrenty = Warrenty + "Service: " + item.Service + Environment.NewLine;
                    //if (!string.IsNullOrEmpty(item.Spareparts))
                    //    Warrenty = Warrenty + "Spareparts: " + item.Spareparts;


                    //dtWarrenty.Rows.Add(item.ProductName, "IMEI", Warrenty);
                    #endregion

                    row = dt.NewRow();
                    row["ProductName"] = item.ProductName;
                    row["Quantity"] = Math.Truncate(item.Quantity / item.ConvertValue);
                    row["UnitQty"] = Math.Round(item.Quantity % item.ConvertValue);
                    row["Rate"] = item.UnitPrice;
                    row["Amount"] = item.TotalAmt;
                    row["ChasisNo"] = string.Empty;
                    row["Color"] = item.ColorName;
                    row["EngineNo"] = string.Empty;
                    row["CategoryName"] = item.CategoryName;
                    row["UnitName"] = item.ParentUnit;
                    row["ProductCode"] = item.ProductCode;
                    row["SizeName"] = item.SizeName;
                    row["CompanyName"] = item.CompanyName;
                    if (item.CategoryName.ToLower().Equals("tiles"))
                    {
                        sizeXY = Array.ConvertAll(item.SizeName.ToLower().Split('x'), decimal.Parse);
                        Length = (sizeXY[0] / 2.5m); //ft
                        Width = (sizeXY[1] / 2.5m); //ft
                        row["SizeName"] = Length.ToString() + "X" + Width.ToString();
                        //AreaPerCarton = item.SalesCSft; //Math.Round((((Length * Width) / 144m) * item.ConvertValue), 2); //sq ft
                        //AreaPerPcs = AreaPerCarton / item.ConvertValue; //Math.Round((((Length * Width) / 144m)), 4); //sq ft
                        row["AreaPerCarton"] = item.SalesCSft;
                        row["RatePerSqft"] = item.SFTRate;// Math.Round(item.UnitPrice / AreaPerPcs, 2);
                        row["TotalArea"] = item.TotalSFT;// Math.Round(item.Quantity * AreaPerPcs, 2);
                    }
                    IMEIs = string.Empty;
                    Warrenty = string.Empty;
                    Count = 0;

                    dt.Rows.Add(row);
                }

                if (dt != null && (dt.Rows != null && dt.Rows.Count > 0))
                    orderdDT = dt.AsEnumerable().OrderBy(o => (String)o["ProductName"]).CopyToDataTable();
                dt.TableName = "dtInvoice";
                _dataSet = new DataSet();
                _dataSet.Tables.Add(dt);
                dtWarrenty.TableName = "dtWarrenty";
                _dataSet.Tables.Add(dtWarrenty);

                string sInwodTk = TakaFormat(Convert.ToDouble(oOrder.PaidAmount));
                GetCommonParameters(userName, concernID);

                _reportParameter = new ReportParameter("TDiscount", "0.00");
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("Total", (oOrder.GrandTotal).ToString("0.00"));
                _reportParameters.Add(_reportParameter);

                //_reportParameter = new ReportParameter("GTotal", (oOrder.GrandTotal + (oOrder.Customer.TotalDue - oOrder.PaymentDue)).ToString());

                _reportParameter = new ReportParameter("GTotal", "0.00");
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("Paid", oOrder.PaidAmount.ToString());
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("CurrDue", (customer.TotalDue).ToString());
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("InvoiceNo", oOrder.InvoiceNo);
                _reportParameters.Add(_reportParameter);

                //_reportParameter = new ReportParameter("TotalDue", oOrder.TotalDue.ToString());
                _reportParameter = new ReportParameter("TotalDue", customer.TotalDue.ToString());
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("InvoiceDate", oOrder.ReturnDate.ToString());
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("RemindDate", customer.RemindDate != null ? customer.RemindDate.Value.ToString("dd MMM yyyy") : "");
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("Company", customer.CompanyName);
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("CAddress", customer.Address);
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("Name", customer.Name);
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("MobileNo", customer.ContactNo);
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("PrintDate", GetClientDateTime());
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("Header", "Return Challan");
                _reportParameters.Add(_reportParameter);

                //_reportParameter =new ReportParameter("InWordTK", sInwodTk);
                //_reportParameters.Add(_reportParameter);

                //if (concernID == (int)EnumSisterConcern.NOKIA_CONCERNID || concernID == (int)EnumSisterConcern.WALTON_CONCERNID || concernID == (int)EnumSisterConcern.NOKIA_STORE_MAGURA_CONCERNID)
                //    return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Sales\\rptMSalesInvoice.rdlc");
                //else if (concernID == (int)EnumSisterConcern.KINGSTAR_CONCERNID)
                //    return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Sales\\rptKSalesInvoice.rdlc");
                //else if (concernID == (int)EnumSisterConcern.HAVEN_ENTERPRISE_CONCERNID || concernID == (int)EnumSisterConcern.HAWRA_ENTERPRISE_CONCERNID)

                return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Sales\\Challan.rdlc");
                // else
                //  return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Sales\\rptSSSalesInvoice.rdlc");
            }

            catch (Exception Ex)
            {
                throw Ex;
            }
        }

        public byte[] GetTrialBalance(DateTime fromDate, DateTime toDate, string UserName, int ConcernID, string ClientDateTime)
        {

            var trialBalanceData = _AccountingService.GetTrialBalance(fromDate, toDate, ConcernID).ToList();
            //var CashInHandData = _CashCollectionService.CashInHandReport(toDate.Date, toDate, 1, ConcernID).ToList();
            //decimal ClosingCashInhand = (decimal)CashInHandData.Where(o => o.Expense == "Closing Cash In Hand").ToList().Sum(o => o.ExpenseAmt);
            //TrialBalanceReportModel cih = new TrialBalanceReportModel() { Particulars = "Cash In Hand", Debit = ClosingCashInhand, Credit = 0, SerialNumber = 1 };
            //trialBalanceData.Insert(0, cih);

            TransactionalDataSet.dtTrialBalanceDataTable dt = new TransactionalDataSet.dtTrialBalanceDataTable();
            _dataSet = new DataSet();
            DataRow row = null;
            foreach (var item in trialBalanceData)
            {
                row = dt.NewRow();
                row["Particulars"] = item.Particulars;
                row["Debit"] = item.Debit;
                row["Credit"] = item.Credit;
                dt.Rows.Add(row);
            }

            dt.TableName = "dtTrialBalance";
            _dataSet.Tables.Add(dt);

            GetCommonParameters(UserName, ConcernID);

            _reportParameter = new ReportParameter("DateRange", "Trial Balance of the date " + toDate.ToString("dd MMM yyyy"));
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("PrintDate", ClientDateTime);
            _reportParameters.Add(_reportParameter);

            return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Accounting\\rptTrialBalance.rdlc");
        }

        public byte[] ProfitLossAccount(DateTime fromDate, DateTime toDate, string UserName, int ConcernID, string ClientDateTime)
        {

            var data = _AccountingService.ProfitLossAccount(fromDate, toDate, ConcernID).ToList();
            TransactionalDataSet.dtProfitLossAccountDataTable dt = new TransactionalDataSet.dtProfitLossAccountDataTable();
            _dataSet = new DataSet();
            DataRow row = null;
            //decimal debitAmt = data.Sum(i => i.Debit);
            //decimal creditAmt = data.Where(i => !(i.CreditParticulars.Equals("Gross Profit"))).Sum(i => i.Credit);

            decimal debitAmt = data.Where(i => !((i.DebitParticulars.Equals("Purchase Return")) || (i.DebitParticulars.Equals("Purchase")))).Sum(i => i.Debit);
            decimal creditAmt = data.Where(i => !((i.CreditParticulars.Equals("Gross Profit"))
                || (i.CreditParticulars.Equals("Sales"))
                || (i.CreditParticulars.Equals("Sales Return")))).Sum(i => i.Credit);
            foreach (var item in data)
            {
                row = dt.NewRow();
                row["DebitParticulars"] = item.DebitParticulars;
                row["Debit"] = item.Debit;
                row["CreditParticulars"] = item.CreditParticulars;
                row["Credit"] = item.Credit;
                dt.Rows.Add(row);
            }

            dt.TableName = "dtProfitLossAccount";
            _dataSet.Tables.Add(dt);

            GetCommonParameters(UserName, ConcernID);
            _reportParameter = new ReportParameter("DateRange", "Profit and Loss Account of the date " + toDate.ToString("dd MMM yyyy"));
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("PrintDate", ClientDateTime);
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("DebitTotal", debitAmt.ToString());
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("CreditTotal", creditAmt.ToString());
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("NetProfit", (creditAmt - debitAmt).ToString());
            _reportParameters.Add(_reportParameter);

            return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Accounting\\rptProfitLossAccount.rdlc");
        }
        public byte[] BalanceSheet(DateTime fromDate, DateTime toDate, string UserName, int ConcernID, string ClientDateTime)
        {

            var data = _AccountingService.BalanceSheet(fromDate, toDate, ConcernID).ToList();
            var CashInHandData = _CashCollectionService.CashInHandReport(fromDate, toDate, 2, ConcernID, 0).ToList();
            decimal ClosingCashInhand = CashInHandData.Where(o => o.Expense == "Closing Cash In Hand").ToList().Sum(o => o.ExpenseAmt);
            TransactionalDataSet.dtProfitLossAccountDataTable dt = new TransactionalDataSet.dtProfitLossAccountDataTable();
            _dataSet = new DataSet();
            DataRow row = null;
            //decimal debitAmt = data.Sum(i => i.Debit);
            //decimal creditAmt = data.Where(i => !(i.CreditParticulars.Equals("Gross Profit"))).Sum(i => i.Credit);
            var debits = data.Where(i => !string.IsNullOrEmpty(i.DebitParticulars)).ToList();
            debits.Insert(1, new ProfitLossReportModel() { DebitParticulars = "Cash In Hand", Debit = ClosingCashInhand, CreditParticulars = "", Credit = 0m, SerialNumber = 1 });
            var credits = data.Where(i => !string.IsNullOrEmpty(i.CreditParticulars)).ToList();
            decimal ownersEquity = debits.Sum(i => i.Debit) - credits.Sum(i => i.Credit);
            credits.Add(new ProfitLossReportModel() { DebitParticulars = "", Debit = 0, CreditParticulars = "Owner's Equity", Credit = ownersEquity });

            List<ProfitLossReportModel> finalData = new List<ProfitLossReportModel>();
            ProfitLossReportModel bs = null;
            if (debits.Count() > credits.Count())
            {
                for (int i = 0; i < debits.Count(); i++)
                {
                    bs = new ProfitLossReportModel();
                    bs.DebitParticulars = debits[i].DebitParticulars;
                    bs.Debit = debits[i].Debit;
                    finalData.Add(bs);
                }

                for (int i = 0; i < credits.Count(); i++)
                {
                    finalData[i].Credit = credits[i].Credit;
                    finalData[i].CreditParticulars = credits[i].CreditParticulars;
                }
            }
            else
            {
                for (int i = 0; i < credits.Count(); i++)
                {
                    bs = new ProfitLossReportModel();
                    bs.CreditParticulars = credits[i].CreditParticulars;
                    bs.Credit = credits[i].Credit;
                    finalData.Add(bs);
                }

                for (int i = 0; i < debits.Count(); i++)
                {
                    finalData[i].Debit = debits[i].Debit;
                    finalData[i].DebitParticulars = debits[i].DebitParticulars;
                }
            }

            foreach (var item in finalData)
            {
                row = dt.NewRow();
                row["DebitParticulars"] = item.DebitParticulars;
                row["Debit"] = item.Debit;
                row["CreditParticulars"] = item.CreditParticulars;
                row["Credit"] = item.Credit;
                dt.Rows.Add(row);
            }

            dt.TableName = "dtProfitLossAccount";
            _dataSet.Tables.Add(dt);

            GetCommonParameters(UserName, ConcernID);
            _reportParameter = new ReportParameter("DateRange", "Balance Sheet of the date " + toDate.ToString("dd MMM yyyy"));
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("PrintDate", ClientDateTime);
            _reportParameters.Add(_reportParameter);

            //_reportParameter = new ReportParameter("DebitTotal", debitAmt.ToString());
            //_reportParameters.Add(_reportParameter);

            //_reportParameter = new ReportParameter("CreditTotal", creditAmt.ToString());
            //_reportParameters.Add(_reportParameter);

            //_reportParameter = new ReportParameter("NetProfit", (creditAmt-debitAmt).ToString());
            //_reportParameters.Add(_reportParameter);

            return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Accounting\\rptBalanceSheet.rdlc");
        }

        public byte[] MonthlyTransactionReport(DateTime fromDate, DateTime toDate, string userName, int concernID)
        {
            var data = _CashCollectionService.MonthlyAdminTransactionReport(fromDate, toDate, concernID);

            TransactionalDataSet.dtTransactionDataTable dt = new TransactionalDataSet.dtTransactionDataTable();
            _dataSet = new DataSet();
            decimal OpeningCashInHand = 0m;
            decimal CurrentCashInHand = 0m;
            decimal ClosingCashInHand = 0m;
            decimal TotalPayable = 0m;
            decimal TotalRecivable = 0m;



            foreach (var item in data)
            {
                if ((item.RetailSale + item.HireSale + item.DealerCollection + item.TotalSale + item.DownPayment + item.HireCollection + item.DealerCollection +
                   item.TotalCollection + item.DailyExpense + item.CompanyPayment + Math.Abs(item.Balance)) != 0)

                    dt.Rows.Add(item.EntryDate, item.RetailSale, item.HireSale, item.DealerSale, item.TotalSale, item.RetailCash, item.DownPayment, item.HireCollection, item.DealerCollection,
                        item.TotalCollection, item.DailyExpense, item.CompanyPayment, item.Balance, item.CumulativeBalance);
            }

            dt.TableName = "dtMonthlyTransaction";
            _dataSet.Tables.Add(dt);
            GetCommonParameters(userName, concernID);
            _reportParameter = new ReportParameter("DateRange", "Date from: " + fromDate.ToString("dd MMM yyyy") + " to " + toDate.ToString("dd MMM yyyy"));
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("OpeningCashInHand", OpeningCashInHand.ToString("0.00"));
            _reportParameters.Add(_reportParameter);
            _reportParameter = new ReportParameter("CurrentCashInHand", CurrentCashInHand.ToString("0.00"));
            _reportParameters.Add(_reportParameter);
            _reportParameter = new ReportParameter("ClosingCashInHand", ClosingCashInHand.ToString("0.00"));
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("TotalPayable", TotalPayable.ToString("0.00"));
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("TotalRecivable", TotalRecivable.ToString("0.00"));
            _reportParameters.Add(_reportParameter);

            return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Others\\rptMonthlyTransaction.rdlc");

        }





        public byte[] LiabilityReport(DateTime fromDate, DateTime toDate, string UserName, int ConcernID, int HeadID, bool OnlyHead)
        {
            IQueryable<ShareInvestment> shareInvestments = null;
            IQueryable<BankTransaction> bankTransactions = null;
            List<LiabilityReportModel> RecPaydata = new List<LiabilityReportModel>();
            LiabilityReportModel oRecPay = null;

            decimal TotalRec = 0m, TotalPay = 0m, headRec = 0m, headPay = 0m;
            if (OnlyHead)
            {
                if (HeadID > 0)
                    shareInvestments = _ShareInvestmentService.GetAll().Where(i => i.SIHID == HeadID);


                else
                    shareInvestments = from i in _ShareInvestmentService.GetAll()
                                       join h in _ShareInvestmentHeadService.GetAll() on i.SIHID equals h.SIHID
                                       where h.ParentId == (int)EnumInvestmentType.Liability
                                       select i;

                if (HeadID > 0)
                    bankTransactions = _bankTransactionService.GetAll().Where(i => i.SIHID == HeadID);
                else
                    bankTransactions = from b in _bankTransactionService.GetAll()
                                       join h in _ShareInvestmentHeadService.GetAll() on b.SIHID equals h.SIHID
                                       where h.ParentId == (int)EnumInvestmentType.Liability
                                       select b;
            }

            else
            {
                if (HeadID > 0)
                    shareInvestments = _ShareInvestmentService.GetAll().Where(i => i.EntryDate >= fromDate && i.EntryDate <= toDate && i.SIHID == HeadID);
                else
                    shareInvestments = from i in _ShareInvestmentService.GetAll().Where(i => i.EntryDate >= fromDate && i.EntryDate <= toDate)
                                       join h in _ShareInvestmentHeadService.GetAll() on i.SIHID equals h.SIHID
                                       where h.ParentId == (int)EnumInvestmentType.Liability
                                       select i;

                if (HeadID > 0)
                    bankTransactions = _bankTransactionService.GetAll().Where(i => i.TranDate >= fromDate && i.TranDate <= toDate && i.SIHID == HeadID);
                else
                    bankTransactions = from b in _bankTransactionService.GetAll().Where(i => i.TranDate >= fromDate && i.TranDate <= toDate)
                                       join h in _ShareInvestmentHeadService.GetAll() on b.SIHID equals h.SIHID
                                       where h.ParentId == (int)EnumInvestmentType.Liability
                                       select b;
            }

            var lReceive = (from si in shareInvestments
                            join h in _ShareInvestmentHeadService.GetAll() on si.SIHID equals h.SIHID
                            where si.TransactionType == EnumInvestTransType.Receive
                            select new LiabilityReportModel
                            {
                                HeadID = h.SIHID,
                                RecDate = si.EntryDate,
                                ReceiveAmt = si.Amount,
                                HeadName = h.Name,
                                RecLiabilityType = EnumInvestTransType.Receive.ToString(),
                                RecPurpose = si.Purpose,
                                Status = si.CashInHandReportStatus,
                                VoucherNo = "Voucher " + si.SIID.ToString()

                            }).OrderBy(i => i.RecDate).ToList();

            if (lReceive.Count() > 0)
                RecPaydata.AddRange(lReceive);

            var lPay = (from si in shareInvestments
                        join h in _ShareInvestmentHeadService.GetAll() on si.SIHID equals h.SIHID
                        where si.TransactionType == EnumInvestTransType.Pay
                        select new LiabilityReportModel
                        {
                            HeadID = h.SIHID,
                            RecDate = si.EntryDate,
                            PayAmt = si.Amount,
                            HeadName = h.Name,
                            PayLiabilityType = EnumInvestTransType.Pay.ToString(),
                            PayPurpose = si.Purpose,
                            Status = si.CashInHandReportStatus,
                            VoucherNo = "Voucher " + si.SIID.ToString()
                        }).OrderBy(i => i.RecDate).ToList();

            if (lPay.Count() > 0)
                RecPaydata.AddRange(lPay);

            var BlReceive = (from b in bankTransactions
                             join h in _ShareInvestmentHeadService.GetAll() on b.SIHID equals h.SIHID
                             where b.TransactionType == (int)EnumTransactionType.LiaRec
                             select new LiabilityReportModel
                             {
                                 HeadID = h.SIHID,
                                 RecDate = b.TranDate.Value,
                                 ReceiveAmt = b.Amount,
                                 HeadName = h.Name,
                                 BankLiaRecType = b.TransactionType,
                                 RecPurpose = b.Remarks,
                             }).OrderBy(i => i.RecDate).ToList();

            if (BlReceive.Count() > 0)
                RecPaydata.AddRange(BlReceive);

            var BlPay = (from b in bankTransactions
                         join h in _ShareInvestmentHeadService.GetAll() on b.SIHID equals h.SIHID
                         where b.TransactionType == (int)EnumTransactionType.LiaPay
                         select new LiabilityReportModel
                         {
                             HeadID = h.SIHID,
                             RecDate = b.TranDate.Value,
                             PayAmt = b.Amount,
                             HeadName = h.Name,
                             BankLiaPayType = b.TransactionType,
                             RecPurpose = b.Remarks,
                         }).OrderBy(i => i.RecDate).ToList();

            if (BlPay.Count() > 0)
                RecPaydata.AddRange(BlPay);


            var finalData = (from l in RecPaydata
                             group l by new
                             {
                                 l.HeadID,
                                 l.HeadName,
                                 l.RecDate,
                                 //l.PayDate,
                                 l.RecPurpose,
                                 l.RecLiabilityType,
                                 l.PayPurpose,
                                 l.PayLiabilityType,
                                 l.BankLiaRecType,
                                 l.BankLiaPayType,
                                 l.Status,
                                 l.VoucherNo

                             } into g
                             select new LiabilityReportModel
                             {
                                 HeadID = g.Key.HeadID,
                                 HeadName = g.Key.HeadName,
                                 RecDate = g.Key.RecDate,
                                 ReceiveAmt = g.Sum(i => i.ReceiveAmt),
                                 RecPurpose = g.Key.RecPurpose,
                                 RecLiabilityType = g.Key.RecLiabilityType,
                                 PayAmt = g.Sum(i => i.PayAmt),
                                 PayPurpose = g.Key.PayPurpose,
                                 PayLiabilityType = g.Key.PayLiabilityType,
                                 BankLiaRecType = g.Key.BankLiaRecType,
                                 BankLiaPayType = g.Key.BankLiaPayType,
                                 Status = g.Key.Status,
                                 VoucherNo = g.Key.VoucherNo

                             }).OrderBy(i => i.HeadID).ThenBy(i => i.RecDate);

            TransactionalDataSet.dtLiabiltyReportDataTable dt = new TransactionalDataSet.dtLiabiltyReportDataTable();
            _dataSet = new DataSet();
            DataRow row = null;
            int lHeadID = 0;
            foreach (var item in finalData)
            {
                if (item.HeadID != lHeadID)
                {
                    lHeadID = item.HeadID;
                    headRec = 0m;
                    headPay = 0m;
                }
                row = dt.NewRow();
                row["RecDate"] = item.RecDate == DateTime.MinValue || item.ReceiveAmt == 0 ? "" : item.RecDate.ToString("dd MMM yyyy");
                row["RecAmt"] = item.ReceiveAmt;
                row["PayDate"] = item.RecDate == DateTime.MinValue || item.PayAmt == 0 ? "" : item.RecDate.ToString("dd MMM yyyy");
                row["PayAmt"] = item.PayAmt;
                row["RecType"] = item.RecLiabilityType != null ? item.RecLiabilityType : "";
                row["PayType"] = item.PayLiabilityType != null ? item.PayLiabilityType : "";
                row["RecPurpose"] = item.RecPurpose;
                row["PayPurpose"] = item.PayPurpose;
                row["HeadName"] = item.HeadName;
                row["VoucherNo"] = item.VoucherNo;
                TotalRec += item.ReceiveAmt;
                TotalPay += item.PayAmt;

                headRec += item.ReceiveAmt;
                headPay += item.PayAmt;
                row["Balance"] = headRec - headPay;

                if (item.BankLiaPayType == 8 || item.BankLiaRecType == 9)
                {
                    row["Status"] = "Don't Show";
                }
                else
                {
                    if (item.Status == 0)
                    {
                        row["Status"] = "Show";
                    }
                    else
                    {
                        row["Status"] = "Don't Show";
                    }
                }

                dt.Rows.Add(row);
            }

            dt.TableName = "dtLiabiltyReport";
            _dataSet.Tables.Add(dt);

            GetCommonParameters(UserName, ConcernID);
            if (OnlyHead)
                _reportParameter = new ReportParameter("DateRange", "Liabilty Report");
            else
                _reportParameter = new ReportParameter("DateRange", "Liabilty report from date: " + fromDate.ToString("dd MMM yyyy") + " to " + toDate.ToString("dd MMM yyyy"));
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("RemainingAmt", (TotalRec - TotalPay).ToString("F2"));
            _reportParameters.Add(_reportParameter);

            return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Investment\\rptLiabilityRpt.rdlc");
        }


        public byte[] SRWiseSalesReport(DateTime fromDate, DateTime toDate, string userName, int concernID, int SRID, int RptType)
        {
            try
            {
                var SRWiseSalesInfos = _salesOrderService.GetSalesDetailReportBySRID(fromDate, toDate, concernID, SRID, RptType);

                TransactionalDataSet.MOSDetailsDataTable dt = new TransactionalDataSet.MOSDetailsDataTable();

                _dataSet = new DataSet();
                DataRow row = null;

                foreach (var item in SRWiseSalesInfos)
                {
                    row = dt.NewRow();
                    row["MOName"] = item.EmployeeName;
                    row["SDate"] = item.InvoiceDate;
                    row["CustomerName"] = item.CustomerName;
                    row["InvoiceNo"] = item.InvoiceNo;
                    row["GrandTotal"] = item.Grandtotal;
                    row["TDiscount"] = item.NetDiscount;
                    row["TotalAmt"] = item.TotalAmount;
                    row["RecAmt"] = item.RecAmount;
                    row["PaymentDue"] = item.PaymentDue;
                    row["AdjAmount"] = item.AdjAmount;
                    if (item.Trems > 0)
                    {
                        row["SalesType"] = item.Trems.ToString();
                    }
                    else
                    {
                        row["SalesType"] = "";
                    }


                    dt.Rows.Add(row);

                }

                dt.TableName = "MOSDetails";
                _dataSet.Tables.Add(dt);

                GetCommonParameters(userName, concernID);

                _reportParameter = new ReportParameter("DateRange", "SR wise sales summary report for the date of : " + fromDate.ToString("dd MMM yyyy") + " to " + toDate.ToString("dd MMM yyyy"));
                _reportParameters.Add(_reportParameter);


                return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Sales\\MOWiseSalesDetails.rdlc");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public byte[] SRWiseProductSalesReport(DateTime fromDate, DateTime toDate, string userName, int concernID, int SRID, int ProductId, int RptType)
        {
            try
            {
                var SRWiseSalesInfos = _salesOrderService.GetProductSalesDetailReportBySRID(fromDate, toDate, concernID, SRID, ProductId, RptType);

                TransactionalDataSet.MOPSDetailsDataTable dt = new TransactionalDataSet.MOPSDetailsDataTable();

                _dataSet = new DataSet();
                DataRow row = null;

                foreach (var item in SRWiseSalesInfos)
                {
                    row = dt.NewRow();
                    row["MOName"] = item.EmployeeName;
                    row["SDate"] = item.InvoiceDate;
                    row["ProductName"] = item.ProductName;
                    row["SizeName"] = item.SizeName;
                    row["Quantity"] = item.Quantity;
                    row["UnitName"] = item.UnitName;
                    row["UnitQty"] = item.UnitQty;
                    row["Rate"] = item.UnitPrice;
                    row["EngineNo"] = item.UnitName;
                    row["Total"] = item.UTAmount;



                    dt.Rows.Add(row);

                }

                dt.TableName = "MOPSDetails";
                _dataSet.Tables.Add(dt);

                GetCommonParameters(userName, concernID);

                _reportParameter = new ReportParameter("DateRange", "SR wise sales summary report for the date of : " + fromDate.ToString("dd MMM yyyy") + " to " + toDate.ToString("dd MMM yyyy"));
                _reportParameters.Add(_reportParameter);


                return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Sales\\MOWiseProductSalesDetails.rdlc");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public byte[] ProductionSetupInvoiceByID(string UserName, int ConcernID, int PSID)
        {
            var data = _productionSetupService.GetById(PSID);
            var FinProduct = _productService.GetAllProductIQueryable().FirstOrDefault(i => i.ProductID == data.FINProductID);

            var Details = (from d in _productionSetupService.GetDetailsById(PSID)
                           join p in _productService.GetAllProductIQueryable() on d.RAWProductID equals p.ProductID
                           select new
                           {
                               p.ProductName,
                               p.ProductCode,
                               p.CompanyName,
                               p.CategoryName,
                               d.Quantity,
                               p.ConvertValue,
                               ChildUnit = p.ChildUnitName,
                               ParentUnit = p.ParentUnitName
                           }).ToList();

            TransactionalDataSet.dtInvoiceDataTable dt = new TransactionalDataSet.dtInvoiceDataTable();
            _dataSet = new DataSet();
            DataRow row = null;

            foreach (var item in Details)
            {
                row = dt.NewRow();
                row["ProductName"] = item.ProductName;
                //row["Quantity"] = item.Quantity;
                row["Quantity"] = Math.Truncate(item.Quantity / item.ConvertValue);
                row["UnitQty"] = item.Quantity % item.ConvertValue; ;
                row["CategoryName"] = item.CategoryName;
                row["ChasisNo"] = item.CompanyName;
                row["EngineNo"] = item.ChildUnit;
                row["UnitName"] = item.ParentUnit;
                dt.Rows.Add(row);
            }

            dt.TableName = "dtInvoice";
            _dataSet.Tables.Add(dt);
            GetCommonParameters(UserName, ConcernID);

            _reportParameter = new ReportParameter("FinCode", FinProduct.ProductCode);
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("FinName", FinProduct.ProductName);
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("FinCategory", FinProduct.CategoryName);
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("FinCompany", FinProduct.CompanyName);
            _reportParameters.Add(_reportParameter);


            _reportParameter = new ReportParameter("PrintDate", GetLocalTime());
            _reportParameters.Add(_reportParameter);

            return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Production\\PSetupInvoice.rdlc");
        }

        public byte[] ProductionInvoiceByID(string UserName, int ConcernID, int productionID)
        {
            var data = _productionService.GetById(productionID);
            var FinProducts = _productionService.GetDetailsById(productionID);

            var Details = (from d in FinProducts
                           join p in _productService.GetAllProductIQueryable() on d.ProductID equals p.ProductID
                           select new
                           {
                               p.ProductName,
                               p.ProductCode,
                               p.CompanyName,
                               p.CategoryName,
                               d.Quantity,
                               d.TotalCost,
                               p.ConvertValue,
                               ChildUnit = p.ChildUnitName,
                               ParentUnit = p.ParentUnitName,
                               SizeName = p.SizeName
                           }).ToList();

            TransactionalDataSet.dtInvoiceDataTable dt = new TransactionalDataSet.dtInvoiceDataTable();
            _dataSet = new DataSet();
            DataRow row = null;

            foreach (var item in Details)
            {
                row = dt.NewRow();
                row["ProductName"] = item.ProductName;
                //row["Quantity"] = item.Quantity;
                row["Quantity"] = Math.Truncate(item.Quantity / item.ConvertValue);
                row["UnitQty"] = item.Quantity % item.ConvertValue; ;
                row["Rate"] = Math.Round(item.TotalCost / item.Quantity, 2);
                row["Amount"] = item.TotalCost;
                row["CategoryName"] = item.CategoryName;
                row["ChasisNo"] = item.CompanyName;
                row["EngineNo"] = item.ChildUnit;
                row["UnitName"] = item.ParentUnit;
                row["SizeName"] = item.SizeName;
                dt.Rows.Add(row);
            }

            dt.TableName = "dtInvoice";
            _dataSet.Tables.Add(dt);
            GetCommonParameters(UserName, ConcernID);

            _reportParameter = new ReportParameter("FinCode", data.ProductionCode);
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("FinName", data.Date.ToString("dd MMM yyyy"));
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("PrintDate", GetLocalTime());
            _reportParameters.Add(_reportParameter);

            return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Production\\ProductionInvoice.rdlc");
        }

        public byte[] ProductionReport(DateTime fromDate, DateTime toDate, string userName, int concernID, int ProductID)
        {
            try
            {

                var productionInfos = _productionService.GetProductionDetailDataByFinProductID(fromDate, toDate, ProductID);
                TransactionalDataSet.dtProductionReportDataTable dt = new TransactionalDataSet.dtProductionReportDataTable();
                _dataSet = new DataSet();
                DataRow row = null;

                foreach (var item in productionInfos)
                {
                    row = dt.NewRow();
                    row["FinProductName"] = item.FinProductName;
                    row["ProductionDate"] = item.ProductionDate;
                    row["ProductionCode"] = item.ProductionCode;
                    row["FinPQuantity"] = Math.Truncate(item.FinQuantity / item.FinConvertValue) + " (" + item.FinParentUnitName + ")";
                    row["FinChildQuantity"] = Math.Round(item.FinQuantity % item.FinConvertValue) + " (" + item.FinChildUnitName + ")";
                    row["FinProRate"] = Math.Round(item.TotalCost / item.FinQuantity, 2);
                    row["FinProAmount"] = item.TotalCost;
                    row["RawProductName"] = item.RawProductName;
                    row["RawPQuantity"] = Math.Truncate(item.RawQuantity / item.RawConvertValue) + " (" + item.RawParentUnitName + ")";
                    row["RawChildQuantity"] = Math.Round(item.RawQuantity % item.RawConvertValue) + " (" + item.RawChildUnitName + ")";
                    row["RawPRate"] = item.RawPRate;
                    row["RawPRateTotal"] = item.RawPRateTotal;
                    dt.Rows.Add(row);
                }

                dt.TableName = "dtProductionReport";
                _dataSet.Tables.Add(dt);
                GetCommonParameters(userName, concernID);

                _reportParameter = new ReportParameter("Date", "Production report for the date of : " + fromDate.ToString("dd MMM yyyy") + " to " + toDate.ToString("dd MMM yyyy"));
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("PrintDate", GetLocalTime());
                _reportParameters.Add(_reportParameter);

                return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Production\\rptProductionReport.rdlc");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public byte[] SRWiseSaleCollectionReport(DateTime fromDate, DateTime toDate, string userName, int concernID, int EmployeeID, int RptType)
        {
            try
            {
                //IEnumerable<Tuple<string, DateTime, string, string, decimal, decimal, Tuple<decimal, decimal, decimal, decimal>>> MOWiseSalesInfos = _salesOrderService.GetSalesDetailReportByMOID(fromDate, toDate, concernID, EmployeeID, RptType);

                var SRWiseSalesInfos = _salesOrderService.GetSalesDetailReportBySRID(fromDate, toDate, concernID, EmployeeID, RptType);

                var CashCollectionInfos = _CashCollectionService.GetSRWiseCashCollectionReportData(fromDate, toDate, concernID, EmployeeID);
                var SalesRetun = _returnOrderService.GetReturnDetailReportByEmployee(fromDate, toDate, concernID, EmployeeID);

                var employee = _EmployeeService.GetEmployeeById(EmployeeID);

                //var SalesOrders = _salesOrderService.GetforSalesReport(fromDate, toDate, EmployeeID, 0);

                TransactionalDataSet.MOSDetailsDataTable dt = new TransactionalDataSet.MOSDetailsDataTable();
                TransactionalDataSet.dtCollectionRptDataTable dtColl = new TransactionalDataSet.dtCollectionRptDataTable();
                TransactionalDataSet.dtCustomerWiseReturnDataTable dtCReturn = new TransactionalDataSet.dtCustomerWiseReturnDataTable();

                _dataSet = new DataSet();
                DataRow row = null;

                foreach (var item in SRWiseSalesInfos)
                {
                    row = dt.NewRow();
                    row["MOName"] = item.EmployeeName;
                    row["SDate"] = item.InvoiceDate;
                    row["CustomerName"] = item.CustomerName;
                    row["InvoiceNo"] = item.InvoiceNo;
                    row["GrandTotal"] = item.Grandtotal;
                    row["TDiscount"] = item.NetDiscount;
                    row["TotalAmt"] = item.TotalAmount;
                    row["RecAmt"] = item.RecAmount;
                    row["PaymentDue"] = item.PaymentDue;
                    row["AdjAmount"] = item.AdjAmount;
                    row["CContact"] = item.CustomerContactNo;
                    row["SalesType"] = item.Trems.ToString();

                    dt.Rows.Add(row);

                }


                foreach (var items in SalesRetun)
                {
                    row = dtCReturn.NewRow();
                    row["InvoiceNo"] = items.Item2;
                    row["CName"] = items.Item4;
                    row["SalesDate"] = items.Item1;
                    row["GrandTotal"] = items.Item7;
                    row["PaidAmount"] = items.Rest.Item3;
                    row["NetTotal"] = items.Rest.Item2;
                    row["ProductName"] = items.Item3;
                    row["SalesPrice"] = items.Item5;
                    row["Quantity"] = items.Rest.Item5;
                    row["NetAmt"] = items.Rest.Item2;
                    dtCReturn.Rows.Add(row);
                }


                //foreach (var grd in CashCollectionInfos)
                //{
                //    dtColl.Rows.Add(grd.Item1.ToString("dd MMM yyyy"), grd.Item2, grd.Item4 + " & " + grd.Item3, grd.Item4, grd.Item5, grd.Item6, grd.Item7, grd.Rest.Item1, grd.Rest.Item2, grd.Rest.Item3, grd.Rest.Item4, grd.Rest.Item5, grd.Rest.Item6, grd.Rest.Item7, grd.Rest.Rest.Item1,"", grd.Rest.Rest.Item2);
                //}



                foreach (var grd in CashCollectionInfos)
                {
                    EnumTranType tranType = EnumTranType.FromCustomer;
                    Enum.TryParse(grd.Rest.Rest.Item2, out tranType);

                    decimal recAmt = grd.Item6;
                    decimal adjustAmt = grd.Rest.Item1;
                    decimal returnAmount = 0;

                    if (tranType == EnumTranType.CollectionReturn)
                    {
                        returnAmount = recAmt;
                        recAmt = 0;
                    }
                    else if (tranType == EnumTranType.SalesCommission)
                    {
                        adjustAmt += recAmt;
                        recAmt = 0;
                    }

                    dtColl.Rows.Add(
                        grd.Item1.ToString("dd MMM yyyy"),    // Date
                        grd.Item2,                            // Name
                        grd.Item4 + " & " + grd.Item3,        // Phone & Address
                        grd.Item4,                            // Phone
                        grd.Item5,                            // TotalDue
                        recAmt,                               // RecAmt (adjusted)
                        grd.Item7,                            // Remaining Amount
                        adjustAmt,                            // Adjusted Amount
                        grd.Rest.Item2,                       // Payment Type
                        grd.Rest.Item3,                       // Bank Name
                        grd.Rest.Item4,                       // Account No
                        grd.Rest.Item5,                       // Remarks
                        grd.Rest.Item6,                       // Bkash
                        grd.Rest.Item7,                       // Employee Name
                        grd.Rest.Rest.Item1,                  // Invoice No
                        "",                                   // Concern Name or unused
                        grd.Rest.Rest.Item2,                  // Collection Type
                        returnAmount,                         // Return Amount
                        grd.Rest.Rest.Item3                   // Any additional value
                    );
                }



                dt.TableName = "MOSDetails";
                _dataSet.Tables.Add(dt);

                dtCReturn.TableName = "dtCustomerWiseReturn";
                _dataSet.Tables.Add(dtCReturn);

                dtColl.TableName = "dtCollectionRpt";
                _dataSet.Tables.Add(dtColl);

                GetCommonParameters(userName, concernID);

                _reportParameter = new ReportParameter("DateRange", "SR wise sales and collection report for the date of : " + fromDate.ToString("dd MMM yyyy") + " to " + toDate.ToString("dd MMM yyyy"));
                _reportParameters.Add(_reportParameter);

                _reportParameter = new ReportParameter("Employee", employee.Name);
                _reportParameters.Add(_reportParameter);


                return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Sales\\SRWiseSaleCollecReport.rdlc");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public byte[] BankAndCashAccountLedger(DateTime fromDate, DateTime toDate, string userName, int concernID, int ExpenseItemID, string headType)
        {
            try
            {
                string headName = string.Empty;
                _dataSet = new DataSet();
                var VoucherTransactionInfos = _salesOrderService.BankAndCashAccountLedgerData(fromDate, toDate, concernID, ExpenseItemID, headType);
                headName = VoucherTransactionInfos.Select(i => i.ItemName).LastOrDefault();
                TransactionalDataSet.dtVoucherTransactionDataTable dt = new TransactionalDataSet.dtVoucherTransactionDataTable();
                DataRow row = null;
                decimal LastBalance = 0m;
                int Counter = 0;
                foreach (var item in VoucherTransactionInfos)
                {
                    var defualtDate = default(DateTime);


                    Counter++;
                    row = dt.NewRow();
                    if (item.VoucherDate == defualtDate)
                    {
                        row["VoucherDate"] = "_";
                    }
                    else
                    {
                        row["VoucherDate"] = item.VoucherDate.ToString("dd MMM yyyy");
                    }

                    row["ModuleType"] = item.ModuleType;
                    row["VoucherNo"] = item.VoucherNo;
                    row["DebitAmount"] = item.DebitAmount;
                    row["CreditAmount"] = item.CreditAmount;
                    row["Balance"] = item.Balance;
                    row["Narration"] = item.Narration;

                    if (VoucherTransactionInfos.Count() == Counter)
                        LastBalance = item.Balance;

                    dt.Rows.Add(row);
                }

                dt.TableName = "dtVoucherTransaction";
                _dataSet.Tables.Add(dt);

                GetCommonParameters(userName, concernID);

                _reportParameter = new ReportParameter("HeadName", headName);
                _reportParameters.Add(_reportParameter);
                _reportParameter = new ReportParameter("DateRange", fromDate.ToString("dd MMM, yyy") + " to " + toDate.ToString("dd MMM, yyy"));
                _reportParameters.Add(_reportParameter);

                return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Others\\rptBankAndCashLedger.rdlc");
            }
            catch (Exception Ex)
            {
                throw Ex;
            }

        }

        public byte[] ReceiptPaymentReport(string userName, int concernId, DateTime fromDate, DateTime toDate)
        {
            TransactionalDataSet.dtRPTReceiptPaymentDataTable dt = new TransactionalDataSet.dtRPTReceiptPaymentDataTable();
            DataRow row = null;

            var data = _salesOrderService.GetReceiptPaymentReport(fromDate, toDate);
            if (data.Any())
            {
                foreach (var item in data)
                {
                    row = dt.NewRow();
                    row["Debit"] = item.DebitParticular;
                    row["DebitAmount"] = item.DebitAmount;
                    row["Credit"] = item.CreditParticular;
                    row["CreditAmount"] = item.CreditAmount;
                    row["IsDrHeader"] = item.IsDrHeader;
                    row["IsCrHeader"] = item.IsCrHeader;
                    row["IsProject"] = item.IsProject;
                    row["ProjectName"] = item.ProjectName;
                    dt.Rows.Add(row);
                }

            }

            dt.TableName = "dtRPTReceiptPayment";
            _dataSet = new DataSet();
            _dataSet.Tables.Add(dt);
            GetCommonParameters(userName, concernId);

            //_reportParameter = new ReportParameter("PrintDate", GetClientDateTime());
            //_reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("FromDate", fromDate.ToString("dd MMM yyyy") + " to " + toDate.ToString("dd MMM yyyy"));
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("Header", "Receipt Payment");
            _reportParameters.Add(_reportParameter);

            return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Others\\rptReceiptPayment.rdlc");
        }

        public byte[] CompanyIncomeStatementReport(DateTime fromDate, DateTime toDate, int ConcernID, string userName)
        {
            TransactionalDataSet.dtSummaryReportNewDataTable dt = new TransactionalDataSet.dtSummaryReportNewDataTable();
            TransactionalDataSet.dtExpSummaryReportNewDataTable dtEXP = new TransactionalDataSet.dtExpSummaryReportNewDataTable();
            TransactionalDataSet.dtIncSummaryReportNewDataTable dtInc = new TransactionalDataSet.dtIncSummaryReportNewDataTable();

            _dataSet = new DataSet();

            #region Sales
            var Transactions = _salesOrderService.GetSummaryReport(fromDate, toDate, ConcernID);

            DataRow row = null;
            foreach (var item in Transactions)
            {
                decimal SalesAmt = Transactions.Where(t => t.Head.Equals("Sales")).Sum(s => s.Amount);
                decimal CostAmt = Transactions.Where(t => t.Head.Equals("Cost of Sales")).Sum(s => s.Amount);

                row = dt.NewRow();
                row["Category"] = item.Category;
                row["Head"] = item.Head;
                row["Amount"] = item.Amount;
                row["Total"] = SalesAmt - CostAmt;

                dt.Rows.Add(row);
            }
            #endregion

            //#region Cash Collection Adjustments as Operating Expense
            //var CashCollectionDatas = (from exp in _CashCollectionService.GetAllCashCollection()
            //                           where exp.EntryDate >= fromDate && exp.EntryDate <= toDate
            //                           select new SummaryReportModel
            //                           {
            //                               Head = "Adjustment",
            //                               Amount = exp.CashBAmt + exp.YearlyBnsAmt + exp.AdjustAmt,
            //                               Category = "Operating Expense"  // Set to Operating Expense
            //                           }).ToList();

            //#endregion

            #region Expense
            var ExpenseData = (from exp in _expenditureService.GetAllExpense()
                               join exi in _ExpenseItemService.GetAll() on exp.ExpenseItemID equals exi.ExpenseItemID
                               where exi.Status == EnumCompanyTransaction.Expense && exp.EntryDate >= fromDate && exp.EntryDate <= toDate
                               select new SummaryReportModel
                               {
                                   id = exi.ExpenseItemID,
                                   Head = exi.Description,
                                   Amount = exp.Amount,
                                   Category = "Operating Expense"
                               }).ToList();

            // Merge Cash Collection Adjustments into ExpenseData
            //ExpenseData.AddRange(CashCollectionDatas);

            var gExpenseDatas = from s in ExpenseData
                                group s by new { s.Head, s.id, Catgory = s.Category } into g
                                select new SummaryReportModel
                                {
                                    id = g.Key.id,
                                    Category = g.Key.Catgory,
                                    Head = g.Key.Head,
                                    Amount = g.Sum(i => i.Amount)
                                };

            foreach (var item in gExpenseDatas)
            {
                decimal ExpAmt = gExpenseDatas.Where(t => t.Category.Equals("Operating Expense")).Sum(s => s.Amount);

                row = dtEXP.NewRow();
                row["Category"] = item.Category;
                row["Head"] = item.Head;
                row["Amount"] = item.Amount;
                row["Total"] = ExpAmt;

                dtEXP.Rows.Add(row);
            }
            #endregion

            #region Income
            var IncomeData = (from exp in _expenditureService.GetAllExpense()
                              join exi in _ExpenseItemService.GetAll() on exp.ExpenseItemID equals exi.ExpenseItemID
                              where exi.Status == EnumCompanyTransaction.Income && exp.EntryDate >= fromDate && exp.EntryDate <= toDate
                              select new SummaryReportModel
                              {
                                  id = exi.ExpenseItemID,
                                  Head = exi.Description,
                                  Amount = exp.Amount,
                                  Category = "Operating Income"
                              }).ToList();

            var gIncomeData = from s in IncomeData
                              group s by new { s.Head, s.id, Catgory = s.Category } into g
                              select new SummaryReportModel
                              {
                                  id = g.Key.id,
                                  Category = g.Key.Catgory,
                                  Head = g.Key.Head,
                                  Amount = g.Sum(i => i.Amount)
                              };

            foreach (var item in gIncomeData)
            {
                decimal IncAmt = gIncomeData.Where(t => t.Category.Equals("Operating Income")).Sum(s => s.Amount);

                row = dtInc.NewRow();
                row["Category"] = item.Category;
                row["Head"] = item.Head;
                row["Amount"] = item.Amount;
                row["Total"] = IncAmt;

                dtInc.Rows.Add(row);
            }
            #endregion

            // Total Calculations
            decimal SAmt = Transactions.Where(t => t.Head.Equals("Sales")).Sum(s => s.Amount);
            decimal CAmt = Transactions.Where(t => t.Head.Equals("Cost of Sales")).Sum(s => s.Amount);
            decimal EAmt = gExpenseDatas.Where(t => t.Category.Equals("Operating Expense")).Sum(s => s.Amount);
            decimal IAmt = gIncomeData.Where(t => t.Category.Equals("Operating Income")).Sum(s => s.Amount);

            // Prepare DataSet for Report
            dt.TableName = "dtSummaryReportNew";
            _dataSet.Tables.Add(dt);

            dtEXP.TableName = "dtExpSummaryReportNew";
            _dataSet.Tables.Add(dtEXP);

            dtInc.TableName = "dtIncSummaryReportNew";
            _dataSet.Tables.Add(dtInc);

            // Set Report Parameters
            GetCommonParameters(userName, ConcernID);

            _reportParameter = new ReportParameter("Total", (SAmt - CAmt - EAmt + IAmt).ToString());
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("DateRange", "Company Income Statement Report of the Date : " + fromDate.ToString("dd MMM yyyy") + " to " + toDate.ToString("dd MMM yyyy"));
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("PrintDate", GetLocalTime());
            _reportParameters.Add(_reportParameter);

            return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Others\\rptCompanyIncomeStatement.rdlc");
        }




        //public byte[] CompanyIncomeStatementReport(DateTime fromDate, DateTime toDate, int ConcernID, string userName)
        //{
        //    TransactionalDataSet.dtSummaryReportNewDataTable dt = new TransactionalDataSet.dtSummaryReportNewDataTable();
        //    TransactionalDataSet.dtExpSummaryReportNewDataTable dtEXP = new TransactionalDataSet.dtExpSummaryReportNewDataTable();
        //    TransactionalDataSet.dtIncSummaryReportNewDataTable dtInc = new TransactionalDataSet.dtIncSummaryReportNewDataTable();


        //    _dataSet = new DataSet();


        //    #region Sales
        //    var Transactions = _salesOrderService.GetSummaryReport(fromDate, toDate, ConcernID);

        //    DataRow row = null;
        //    foreach (var item in Transactions)
        //    {
        //        decimal SalesAmt = Transactions.Where(t => t.Head.Equals("Sales")).Sum(s => s.Amount);
        //        decimal CostAmt = Transactions.Where(t => t.Head.Equals("Cost of Sales")).Sum(s => s.Amount);

        //        row = dt.NewRow();
        //        row["Category"] = item.Category;
        //        row["Head"] = item.Head;
        //        row["Amount"] = item.Amount;
        //        row["Total"] = SalesAmt - CostAmt;

        //        dt.Rows.Add(row);
        //    }

        //    #endregion

        //    #region Cash Collection
        //    var CashCollectionDatas = (from exp in _CashCollectionService.GetAllCashCollection()
        //                        where exp.EntryDate >= fromDate && exp.EntryDate <= toDate
        //                        select new SummaryReportModel
        //                        {
        //                            Head = "Adjustment",
        //                            Amount = exp.CashBAmt + exp.YearlyBnsAmt + exp.AdjustAmt,
        //                            Category = "Cash Collection Adjustment"
        //                        }).ToList();

        //    var gCashCollectionAdjData = from s in CashCollectionDatas
        //                       group s by new { s.Head, s.Category } into g
        //                       select new SummaryReportModel
        //                       {
        //                           Category = g.Key.Category,
        //                           Head = g.Key.Head,
        //                           Amount = g.Sum(i => i.Amount)
        //                       };

        //    foreach (var item in gCashCollectionAdjData)
        //    {
        //        decimal ExpAmt = gCashCollectionAdjData.Where(t => t.Category.Equals("Cash Collection Adjustment")).Sum(s => s.Amount);

        //        row = dtEXP.NewRow();
        //        row["Category"] = item.Category;
        //        row["Head"] = item.Head;
        //        row["Amount"] = item.Amount;
        //        row["Total"] = ExpAmt;

        //        dtEXP.Rows.Add(row);
        //    }
        //    #endregion


        //    #region Expense
        //    var ExpenseData = (from exp in _expenditureService.GetAllExpense()
        //                       join exi in _ExpenseItemService.GetAll() on exp.ExpenseItemID equals exi.ExpenseItemID
        //                       where exi.Status == EnumCompanyTransaction.Expense && exp.EntryDate >= fromDate && exp.EntryDate <= toDate
        //                       select new SummaryReportModel
        //                       {
        //                           id = exi.ExpenseItemID,
        //                           Head = exi.Description,
        //                           Amount = exp.Amount,
        //                           Category = "Operating Expense"
        //                       }).ToList();

        //    var gExpenseDatas = from s in ExpenseData
        //                        group s by new { s.Head, s.id, Catgory = s.Category } into g
        //                        select new SummaryReportModel
        //                        {
        //                            id = g.Key.id,
        //                            Category = g.Key.Catgory,
        //                            Head = g.Key.Head,
        //                            Amount = g.Sum(i => i.Amount)
        //                        };

        //    foreach (var item in gExpenseDatas)
        //    {
        //        decimal ExpAmt = gExpenseDatas.Where(t => t.Category.Equals("Operating Expense")).Sum(s => s.Amount);

        //        row = dtEXP.NewRow();
        //        row["Category"] = item.Category;
        //        row["Head"] = item.Head;
        //        row["Amount"] = item.Amount;
        //        row["Total"] = ExpAmt;

        //        dtEXP.Rows.Add(row);
        //    }

        //    #endregion

        //    #region Income
        //    var IncomeData = (from exp in _expenditureService.GetAllExpense()
        //                      join exi in _ExpenseItemService.GetAll() on exp.ExpenseItemID equals exi.ExpenseItemID
        //                      where exi.Status == EnumCompanyTransaction.Income && exp.EntryDate >= fromDate && exp.EntryDate <= toDate
        //                      select new SummaryReportModel
        //                      {
        //                          id = exi.ExpenseItemID,
        //                          Head = exi.Description,
        //                          Amount = exp.Amount,
        //                          Category = "Operating Income"
        //                      }).ToList();

        //    var gIncomeData = from s in IncomeData
        //                      group s by new { s.Head, s.id, Catgory = s.Category } into g
        //                      select new SummaryReportModel
        //                      {
        //                          id = g.Key.id,
        //                          Category = g.Key.Catgory,
        //                          Head = g.Key.Head,
        //                          Amount = g.Sum(i => i.Amount)
        //                      };

        //    foreach (var item in gIncomeData)
        //    {
        //        decimal IncAmt = gIncomeData.Where(t => t.Category.Equals("Operating Income")).Sum(s => s.Amount);

        //        row = dtInc.NewRow();
        //        row["Category"] = item.Category;
        //        row["Head"] = item.Head;
        //        row["Amount"] = item.Amount;
        //        row["Total"] = IncAmt;

        //        dtInc.Rows.Add(row);
        //    }

        //    #endregion

        //    decimal SAmt = Transactions.Where(t => t.Head.Equals("Sales")).Sum(s => s.Amount);
        //    decimal CAmt = Transactions.Where(t => t.Head.Equals("Cost of Sales")).Sum(s => s.Amount);
        //    decimal EAmt = gExpenseDatas.Where(t => t.Category.Equals("Operating Expense")).Sum(s => s.Amount);
        //    decimal CAdjAmt = gCashCollectionAdjData.Where(t => t.Category.Equals("Cash Collection Adjustment")).Sum(s => s.Amount);
        //    decimal IAmt = gIncomeData.Where(t => t.Category.Equals("Operating Income")).Sum(s => s.Amount);

        //    dt.TableName = "dtSummaryReportNew";
        //    //_dataSet = new DataSet();
        //    _dataSet.Tables.Add(dt);

        //    dtEXP.TableName = "dtExpSummaryReportNew";
        //    _dataSet.Tables.Add(dtEXP);

        //    dtInc.TableName = "dtIncSummaryReportNew";
        //    _dataSet.Tables.Add(dtInc);

        //    GetCommonParameters(userName, ConcernID);

        //    _reportParameter = new ReportParameter("Total", (SAmt - CAmt - EAmt + IAmt).ToString());
        //    _reportParameters.Add(_reportParameter);

        //    _reportParameter = new ReportParameter("DateRange", "Company Income Statement Report of the Date : " + fromDate.ToString("dd MMM yyyy") + " to " + toDate.ToString("dd MMM yyyy"));
        //    _reportParameters.Add(_reportParameter);

        //    _reportParameter = new ReportParameter("PrintDate", GetLocalTime());
        //    _reportParameters.Add(_reportParameter);

        //    //string gexp = gExpenseData.Where(i => i.Category.Equals("Operating Expense")).ToString();

        //    //string rev = Transactions.Where(i => i.Category.Equals("Revenue")).ToString();

        //    //if (gExpenseData.Select(i => i.Category.Equals("Operating Expense")).ToString() == "Operating Expense")
        //    //    _reportParameter = new ReportParameter("Total", "Total Operating Expense :");
        //    //else if (Transactions.Select(i => i.Category.Equals("Revenue")).ToString() == "Revenue")
        //    //    _reportParameter = new ReportParameter("Total", "Gross Profit :" + rev);
        //    //_reportParameters.Add(_reportParameter);

        //    return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Others\\rptCompanyIncomeStatement.rdlc");

        //}




        public byte[] VoucherTransactionLedger(DateTime fromDate, DateTime toDate, string userName, int concernID, int ExpenseItemID, string headType)
        {
            try
            {
                string headName = string.Empty;
                _dataSet = new DataSet();
                var VoucherTransactionInfos = _ShareInvestmentService.VoucherTransactionLedgerData(fromDate, toDate, concernID, ExpenseItemID, headType);
                headName = VoucherTransactionInfos.Select(i => i.ItemName).LastOrDefault();
                TransactionalDataSet.dtVoucherTransactionDataTable dt = new TransactionalDataSet.dtVoucherTransactionDataTable();
                DataRow row = null;
                decimal LastBalance = 0m;
                int Counter = 0;
                foreach (var item in VoucherTransactionInfos)
                {
                    var defualtDate = default(DateTime);


                    Counter++;
                    row = dt.NewRow();
                    if (item.VoucherDate == defualtDate)
                    {
                        row["VoucherDate"] = "_";
                    }
                    else
                    {
                        row["VoucherDate"] = item.VoucherDate.ToString("dd MMM yyyy");
                    }

                    row["ModuleType"] = item.ModuleType;
                    row["VoucherNo"] = item.VoucherNo;
                    row["DebitAmount"] = item.DebitAmount;
                    row["CreditAmount"] = item.CreditAmount;
                    row["Balance"] = item.Balance;
                    row["Narration"] = item.Narration;

                    if (VoucherTransactionInfos.Count() == Counter)
                        LastBalance = item.Balance;

                    dt.Rows.Add(row);
                }

                dt.TableName = "dtVoucherTransaction";
                _dataSet.Tables.Add(dt);

                GetCommonParameters(userName, concernID);

                _reportParameter = new ReportParameter("HeadName", headName);
                _reportParameters.Add(_reportParameter);
                _reportParameter = new ReportParameter("DateRange", fromDate.ToString("dd MMM, yyy") + " to " + toDate.ToString("dd MMM, yyy"));
                _reportParameters.Add(_reportParameter);

                return ReportBase.GenerateTransactionalReport(_dataSet, _reportParameters, "Others\\rptVoucherTransactionLedger.rdlc");
            }
            catch (Exception Ex)
            {
                throw Ex;
            }


        }

    }


}



