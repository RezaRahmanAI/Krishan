using IMSWEB.Data;
using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IMSWEB.Model.TOs;

namespace IMSWEB.Service
{
    public class SystemInformationService : ISystemInformationService
    {
        private readonly IBaseRepository<SystemInformation> _systemInformationRepository;
        private readonly IUnitOfWork _unitOfWork;

        public SystemInformationService(IBaseRepository<SystemInformation> systemInformationRepository, IUnitOfWork unitOfWork)
        {
            _systemInformationRepository = systemInformationRepository;
            _unitOfWork = unitOfWork;
        }

        public void UpdateSystemInformation(SystemInformation systemInformation)
        {
            _systemInformationRepository.Update(systemInformation);
        }

        public void SaveSystemInformation()
        {
            _unitOfWork.Commit();
        }

        public SystemInformation GetSystemInformationById(int id)
        {
            return _systemInformationRepository.FindBy(x => x.SystemInfoID == id).First();
        }
        public SystemInformation GetSystemInformationByConcernId(int id)
        {
            return _systemInformationRepository.FindBy(x => x.ConcernID == id).First();
        }

        public IQueryable<SystemInformation> GetAllConcernSysInfo()
        {
            return _systemInformationRepository.GetAll();
        }
        public bool IsEmployeeWiseTransactionEnable()
        {
            return _systemInformationRepository.All.FirstOrDefault().EmployeeWiseTransactionEnable == 1 ? true : false;
        }

        public List<TOHomeWidget> GetHomeWidgetLoanExpense(string dataLength, int concernId)
        {

            DateTime dayLength = DateTime.Now;
            switch (dataLength)
            {
                case "d7":
                    dayLength = dayLength.AddDays(-7);
                    break;
                case "d30":
                    dayLength = dayLength.AddDays(-30);
                    break;
                case "d90":
                    dayLength = dayLength.AddDays(-90);
                    break;
                default:
                    break;
            }

            string query = string.Format(@"
                                            SELECT SUM(ex.Amount) Amount, 'Income' TrType FROM Expenditures ex
                                            JOIN ExpenseItems ei ON ex.ExpenseItemID = ei.ExpenseItemID
                                            WHERE ex.ConcernID = {0} AND ei.Status = 2 AND CAST(ex.EntryDate AS DATE) >= CAST('{1}' AS DATE)
                                            UNION ALL

                                            SELECT SUM(ex.Amount), 'Expense' FROM Expenditures ex
                                            JOIN ExpenseItems ei ON ex.ExpenseItemID = ei.ExpenseItemID
                                            WHERE ex.ConcernID = {0} AND ei.Status = 1 AND CAST(ex.EntryDate AS DATE) >= CAST('{1}' AS DATE)
                                            UNION ALL

                                            SELECT SUM(TotalAmt), 'Purchase' FROM POrders WHERE ConcernID = {0} AND Status = 1
                                            AND CAST(OrderDate AS DATE) >= CAST('{1}' AS DATE)
                                            UNION ALL

                                            SELECT SUM(TotalAmount), 'Sale' FROM SOrders WHERE ConcernID = {0} AND Status = 1
                                            AND CAST(InvoiceDate AS DATE) >= CAST('{1}' AS DATE)
                                            ", concernId, dayLength);
            try
            {
                return _systemInformationRepository.SQLQueryList<TOHomeWidget>(query).ToList();
            }
            catch (Exception ex)
            {
                List<TOHomeWidget> list = new List<TOHomeWidget>();
                list.Add(new TOHomeWidget { Amount = 0m, TrType = "Income" });
                list.Add(new TOHomeWidget { Amount = 0m, TrType = "Expense" });
                list.Add(new TOHomeWidget { Amount = 0m, TrType = "Purchase" });
                list.Add(new TOHomeWidget { Amount = 0m, TrType = "Sale" });
                return list;
            }

        }


        public List<decimal> GetFullYearIncome(int concernId)
        {
            string query = string.Format(@"SELECT Amount
                                            FROM
                                            (
                                                SELECT *
                                                FROM 
                                                    (

		                                            SELECT SUM(ex.Amount) Amount, DATEPART(month, ex.EntryDate) AS Month, 'Income' TrType FROM Expenditures ex
			                                            JOIN ExpenseItems ei ON ex.ExpenseItemID = ei.ExpenseItemID
			                                             WHERE ex.ConcernID = {0}
                                                            AND ei.Status = 2
				                                            AND YEAR(ex.EntryDate) = YEAR(GETDATE())
				                                            group by ex.EntryDate
		                                            ) AS SourceTable
                                                PIVOT
                                                (
                                                    SUM(Amount)
                                                    FOR Month IN ([1], [2], [3], [4], [5], [6], [7], [8], [9], [10], [11], [12])
                                                ) AS PivotTable
                                            ) AS PivotResult
                                            UNPIVOT
                                            (
                                                Amount
                                                FOR Month IN ([1], [2], [3], [4], [5], [6], [7], [8], [9], [10], [11], [12])
                                            )AS UnpivotResult", concernId);
            try
            {
                return _systemInformationRepository.SQLQueryList<decimal>(query).ToList();
            }
            catch (Exception ex)
            {
                List<decimal> list = new List<decimal>();
                list.Add(0m);
                return list;
            }

        }

        public List<decimal> GetFullYearExpense(int concernId)
        {
            string query = string.Format(@"SELECT Amount
                                            FROM
                                            (
                                                SELECT *
                                                FROM 
                                                    (

		                                            SELECT SUM(ex.Amount) Amount, DATEPART(month, ex.EntryDate) AS Month, 'Income' TrType FROM Expenditures ex
			                                            JOIN ExpenseItems ei ON ex.ExpenseItemID = ei.ExpenseItemID
			                                             WHERE ex.ConcernID = {0}
                                                            AND ei.Status = 1
				                                            AND YEAR(ex.EntryDate) = YEAR(GETDATE())
				                                            group by ex.EntryDate
		                                            ) AS SourceTable
                                                PIVOT
                                                (
                                                    SUM(Amount)
                                                    FOR Month IN ([1], [2], [3], [4], [5], [6], [7], [8], [9], [10], [11], [12])
                                                ) AS PivotTable
                                            ) AS PivotResult
                                            UNPIVOT
                                            (
                                                Amount
                                                FOR Month IN ([1], [2], [3], [4], [5], [6], [7], [8], [9], [10], [11], [12])
                                            )AS UnpivotResult", concernId);
            try
            {
                return _systemInformationRepository.SQLQueryList<decimal>(query).ToList();
            }
            catch (Exception ex)
            {
                List<decimal> list = new List<decimal>();
                list.Add(0m);
                return list;
            }

        }
        public List<decimal> GetFullYearPurchase(int concernId)
        {
            string query = string.Format(@"SELECT Amount
                                            FROM
                                            (
                                                SELECT *
                                                FROM 
                                                    (

		                                            SELECT SUM(TotalAmt) Amount, DATEPART(month, ex.OrderDate) AS Month, 'Purchase' TrType FROM POrders ex
			                                             WHERE ex.ConcernID = {0}
                                                            AND ex.Status = 1
				                                            AND YEAR(ex.OrderDate) = YEAR(GETDATE())
				                                            group by ex.OrderDate
		                                            ) AS SourceTable
                                                PIVOT
                                                (
                                                    SUM(Amount)
                                                    FOR Month IN ([1], [2], [3], [4], [5], [6], [7], [8], [9], [10], [11], [12])
                                                ) AS PivotTable
                                            ) AS PivotResult
                                            UNPIVOT
                                            (
                                                Amount
                                                FOR Month IN ([1], [2], [3], [4], [5], [6], [7], [8], [9], [10], [11], [12])
                                            )AS UnpivotResult", concernId);
            try
            {
                return _systemInformationRepository.SQLQueryList<decimal>(query).ToList();
            }
            catch (Exception ex)
            {
                List<decimal> list = new List<decimal>();
                list.Add(0m);
                return list;
            }

        }

        public List<decimal> GetFullYearSale(int concernId)
        {
            string query = string.Format(@"SELECT Amount
                                            FROM
                                            (
                                                SELECT *
                                                FROM 
                                                    (

		                                            SELECT SUM(TotalAmount) Amount, DATEPART(month, ex.InvoiceDate) AS Month, 'Sale' TrType FROM SOrders ex
			                                             WHERE ex.ConcernID = {0}
                                                            AND ex.Status = 1
				                                            AND YEAR(ex.InvoiceDate) = YEAR(GETDATE())
				                                            group by ex.InvoiceDate
		                                            ) AS SourceTable
                                                PIVOT
                                                (
                                                    SUM(Amount)
                                                    FOR Month IN ([1], [2], [3], [4], [5], [6], [7], [8], [9], [10], [11], [12])
                                                ) AS PivotTable
                                            ) AS PivotResult
                                            UNPIVOT
                                            (
                                                Amount
                                                FOR Month IN ([1], [2], [3], [4], [5], [6], [7], [8], [9], [10], [11], [12])
                                            )AS UnpivotResult", concernId);
            try
            {
                return _systemInformationRepository.SQLQueryList<decimal>(query).ToList();
            }
            catch (Exception ex)
            {
                List<decimal> list = new List<decimal>();
                list.Add(0m);
                return list;
            }

        }




        public List<TOHomeWidget> GetHomeWidgeSales(string dataLength, int concernId)
        {
            DateTime dayLength = DateTime.Now;

            if (int.TryParse(dataLength, out int days))
            {
                dayLength = dayLength.AddDays(-days);
            }

            string query = string.Format(@"
        SELECT COALESCE(SUM(TotalAmount), 0) AS Amount, 'SalesAmount' AS TrType 
        FROM SOrders 
        WHERE ConcernID = {0} AND Status != 2 AND CAST(InvoiceDate AS DATE) >= CAST('{1}' AS DATE)
        
        UNION ALL
        
        SELECT COALESCE(SUM(TotalAmt), 0) AS Amount, 'PurchaseAmount' AS TrType 
        FROM POrders 
        WHERE ConcernID = {0}  AND Status != 2 AND CAST(OrderDate AS DATE) >= CAST('{1}' AS DATE)
        
        UNION ALL
        
        SELECT COALESCE(SUM(Amount), 0) AS Amount, 'CashCollection' AS TrType 
        FROM CashCollections 
        WHERE ConcernID = {0} AND CAST(EntryDate AS DATE) >= CAST('{1}' AS DATE)

        UNION ALL
        
        SELECT COALESCE(SUM(ex.Amount), 0) AS Amount, 'Income' AS TrType 
        FROM Expenditures ex
        JOIN ExpenseItems ei ON ex.ExpenseItemID = ei.ExpenseItemID
        WHERE ex.ConcernID = {0} AND ei.Status = 2 AND CAST(ex.EntryDate AS DATE) >= CAST('{1}' AS DATE)
        
        UNION ALL
        
        SELECT COALESCE(SUM(ex.Amount), 0) AS Amount, 'Expense' AS TrType 
        FROM Expenditures ex
        JOIN ExpenseItems ei ON ex.ExpenseItemID = ei.ExpenseItemID
        WHERE ex.ConcernID = {0} AND ei.Status = 1 AND CAST(ex.EntryDate AS DATE) >= CAST('{1}' AS DATE)
    ", concernId, dayLength.ToString("yyyy-MM-dd"));

            try
            {
                return _systemInformationRepository.SQLQueryList<TOHomeWidget>(query).ToList();
            }
            catch (Exception ex)
            {
                List<TOHomeWidget> list = new List<TOHomeWidget>();
                list.Add(new TOHomeWidget { Amount = 0m, TrType = "SalesAmount" });
                list.Add(new TOHomeWidget { Amount = 0m, TrType = "PurchaseAmount" });
                list.Add(new TOHomeWidget { Amount = 0m, TrType = "Income" });
                list.Add(new TOHomeWidget { Amount = 0m, TrType = "Expense" });
                list.Add(new TOHomeWidget { Amount = 0m, TrType = "CashCollection" });
                return list;
            }
        }








        public List<TOHomeWidget> GetYearlyData(string dataLength, int concernId)
        {
            DateTime dayLength = DateTime.Now;
            int currentYear = DateTime.Now.Year;

            if (int.TryParse(dataLength, out int days))
            {
                dayLength = dayLength.AddDays(-days);
            }

            string query = string.Format(@"
    SELECT
        DATEPART(YEAR, InvoiceDate) AS Year,
        DATEPART(MONTH, InvoiceDate) AS Month,
        COALESCE(SUM(TotalAmount), 0) AS Amount,
        'SalesAmount' AS TrType 
    FROM SOrders 
    WHERE ConcernID = {0} AND Status != 2 AND InvoiceDate >= '{1}' AND DATEPART(YEAR, InvoiceDate) = {2}
    GROUP BY DATEPART(YEAR, InvoiceDate), DATEPART(MONTH, InvoiceDate)
    
    UNION ALL
    
    SELECT
        DATEPART(YEAR, OrderDate) AS Year,
        DATEPART(MONTH, OrderDate) AS Month,
        COALESCE(SUM(TotalAmt), 0) AS Amount,
        'PurchaseAmount' AS TrType 
    FROM POrders 
    WHERE ConcernID = {0} AND Status != 2 AND OrderDate >= '{1}' AND DATEPART(YEAR, OrderDate) = {2}
    GROUP BY DATEPART(YEAR, OrderDate), DATEPART(MONTH, OrderDate)
    
    UNION ALL
    
    SELECT
        DATEPART(YEAR, EntryDate) AS Year,
        DATEPART(MONTH, EntryDate) AS Month,
        COALESCE(SUM(Amount), 0) AS Amount,
        'CashCollection' AS TrType 
    FROM CashCollections 
    WHERE ConcernID = {0} AND EntryDate >= '{1}' AND DATEPART(YEAR, EntryDate) = {2}
    GROUP BY DATEPART(YEAR, EntryDate), DATEPART(MONTH, EntryDate)
    
    UNION ALL
    
    SELECT
        DATEPART(YEAR, ex.EntryDate) AS Year,
        DATEPART(MONTH, ex.EntryDate) AS Month,
        COALESCE(SUM(ex.Amount), 0) AS Amount,
        'Income' AS TrType 
    FROM Expenditures ex
    JOIN ExpenseItems ei ON ex.ExpenseItemID = ei.ExpenseItemID
    WHERE ex.ConcernID = {0} AND ei.Status = 2 AND ex.EntryDate >= '{1}' AND DATEPART(YEAR, ex.EntryDate) = {2}
    GROUP BY DATEPART(YEAR, ex.EntryDate), DATEPART(MONTH, ex.EntryDate)
    
    UNION ALL
    
    SELECT
        DATEPART(YEAR, ex.EntryDate) AS Year,
        DATEPART(MONTH, ex.EntryDate) AS Month,
        COALESCE(SUM(ex.Amount), 0) AS Amount,
        'Expense' AS TrType 
    FROM Expenditures ex
    JOIN ExpenseItems ei ON ex.ExpenseItemID = ei.ExpenseItemID
    WHERE ex.ConcernID = {0} AND ei.Status = 1 AND ex.EntryDate >= '{1}' AND DATEPART(YEAR, ex.EntryDate) = {2}
    GROUP BY DATEPART(YEAR, ex.EntryDate), DATEPART(MONTH, ex.EntryDate)
    ", concernId, dayLength.ToString("yyyy-MM-dd"), currentYear);

            try
            {
                return _systemInformationRepository.SQLQueryList<TOHomeWidget>(query).ToList();
            }
            catch (Exception ex)
            {
                List<TOHomeWidget> list = new List<TOHomeWidget>();
                for (int i = 1; i <= 12; i++)
                {
                    list.Add(new TOHomeWidget { Amount = 0m, TrType = "SalesAmount", Month = i, Year = DateTime.Now.Year });
                    list.Add(new TOHomeWidget { Amount = 0m, TrType = "PurchaseAmount", Month = i, Year = DateTime.Now.Year });
                    list.Add(new TOHomeWidget { Amount = 0m, TrType = "Income", Month = i, Year = DateTime.Now.Year });
                    list.Add(new TOHomeWidget { Amount = 0m, TrType = "Expense", Month = i, Year = DateTime.Now.Year });
                    list.Add(new TOHomeWidget { Amount = 0m, TrType = "CashCollection", Month = i, Year = DateTime.Now.Year });
                }
                return list;
            }
        }
    }
}
