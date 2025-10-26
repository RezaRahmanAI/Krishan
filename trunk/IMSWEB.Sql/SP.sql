-- Date: 06-Jun-18 1:16:08 PM 
USE [DEV-IMSWEB]
GO
/****** Object:  StoredProcedure [dbo].[InstallmentPayment]    Script Date: 06-Jun-18 1:16:08 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--------------------------------------------------------------------------------------------------



ALTER PROC [dbo].[InstallmentPayment]
(
	@SalesOrderId int,
	@InstallmentAmount decimal(18,2),
	@Schedules [InsertCSSchedulesTable] readonly,
	@LastPayAdjustment decimal(18,2)
)
  
AS 

UPDATE CreditSales SET Remaining = (Remaining - (@InstallmentAmount+@LastPayAdjustment)),LastPayAdjAmt=@LastPayAdjustment
Where CreditSalesID = @SalesOrderId

UPDATE Customers Set Customers.TotalDue = (Customers.TotalDue - (@InstallmentAmount+@LastPayAdjustment))
from CreditSales s
JOIN Customers ON Customers.CustomerID = s.CustomerID

DELETE CreditSalesSchedules WHERE CreditSalesID = @SalesOrderId

INSERT INTO CreditSalesSchedules
(
MonthDate, Balance, InstallmentAmt, PaymentDate, 
CreditSalesID, PaymentStatus, InterestAmount, ClosingBalance, ScheduleNo, Remarks, IsUnExpected,HireValue,NetValue
)
(
Select MonthDate, Balance, InstallmentAmt, PaymentDate, 
@SalesOrderId CreditSalesID, PaymentStatus, InterestAmount, ClosingBalance, 
ScheduleNo, Remarks, IsUnExpected,HireValue,NetValue from @Schedules
)

RETURN





--Date: 12-APR-2018

alter PROC [dbo].[AddCreditSalesOrder]
(
	@CSalesOrder  [dbo].[InsertCreditSalesOrderTable] readonly,
	@CSODetails [InsertCSODetailTable] readonly,
	@CSSchedules [InsertCSSchedulesTable] readonly
)
  
AS 
DECLARE @CreditSalesId int
  
INSERT INTO CreditSales
(InvoiceNo, CustomerID, TSalesAmt, NoOfInstallment,
IntallmentPrinciple,IssueDate,UserName,Remaining, InterestRate,InterestAmount, SalesDate,
DownPayment, WInterestAmt, FixedAmt, IsStatus, UnExInstallment, Quantity,
Discount, NetAmount, ISUnexpected, Remarks, VATPercentage, VATAmount, Status,
ConcernID, CreatedBy, CreateDate,TotalOffer 
)
(
Select InvoiceNo, CustomerID, TSalesAmt, NoOfInstallment,
InstallmentPrinciple, IssueDate,UserName,Remaining,InterestRate,InterestAmount , SalesDate,
DownPayment, WInterestAmt, FixedAmount, IsStatus, UnExInstallment, Quantity,
Discount, NetAmount, ISUnexpected, Remarks, VATPercentage, VATAmount, Status,
ConcernID, CreatedBy, CreateDate,TotalOffer from @CSalesOrder
)

UPDATE Customers Set Customers.TotalDue = (Customers.TotalDue + s.TotalDue)
from @CSalesOrder s
JOIN Customers ON Customers.CustomerID = s.CustomerID

SET @CreditSalesId = SCOPE_IDENTITY()

INSERT INTO CreditSaleDetails
(
ProductID ,UnitPrice, Quantity, UTAmount, CreditSalesID, 
MPRateTotal, MPRate, StockDetailID,PPOffer,IntPercentage,IntTotalAmt
)
(
Select ProductID ,UnitPrice, Quantity, UTAmount, @CreditSalesId CreditSalesID, 
MPRateTotal, MrpRate, StockDetailID,PPOffer,IntPercentage,IntTotalAmt from @CSODetails
)

INSERT INTO CreditSalesSchedules
(
MonthDate, Balance, InstallmentAmt, PaymentDate, 
CreditSalesID, PaymentStatus, InterestAmount, ClosingBalance, ScheduleNo, Remarks, IsUnExpected,HireValue,NetValue
)
(
Select MonthDate, Balance, InstallmentAmt, PaymentDate, 
@CreditSalesId CreditSalesID, PaymentStatus, InterestAmount, ClosingBalance, 
ScheduleNo, Remarks, IsUnExpected ,HireValue,NetValue from @CSSchedules
)

UPDATE Stocks
SET Stocks.Quantity = (Stocks.Quantity - s.Quantity)
from @CSODetails s
JOIN Stocks ON Stocks.ProductID = s.ProductId AND Stocks.ColorID = s.ColorId

UPDATE StockDetails
SET StockDetails.Status = 2
from @CSODetails s
JOIN StockDetails ON StockDetails.SDetailID = s.StockDetailId
join Products on StockDetails.ProductID = Products.ProductID
where Products.ProductType!=2
RETURN

--------------------------------------------
--------------------------------------------


/****** Object:  UserDefinedTableType [dbo].[InsertCSSchedulesTable]    Script Date: 12-Apr-18 11:58:04 AM ******/
CREATE TYPE [dbo].[InsertCSSchedulesTable] AS TABLE(
	[MonthDate] [datetime] NULL,
	[Balance] [money] NULL,
	[InstallmentAmt] [money] NULL,
	[PaymentDate] [datetime] NULL,
	[PaymentStatus] [nvarchar](150) NULL,
	[InterestAmount] [money] NULL,
	[ClosingBalance] [money] NULL,
	[ScheduleNo] [int] NULL,
	[Remarks] [nvarchar](max) NULL,
	[IsUnExpected] [int] NULL,
	HireValue decimal(18,2) null,
	NetValue decimal(18,2) null
)
GO


--------------------------------------------------------------------------------------------------
--------------------------------------------------------------------------------------------------

alter PROC [dbo].[InstallmentPayment]
(
	@SalesOrderId int,
	@InstallmentAmount decimal(18,2),
	@Schedules [InsertCSSchedulesTable] readonly
)
  
AS 
 
UPDATE CreditSales SET Remaining = (Remaining - @InstallmentAmount)
Where CreditSalesID = @SalesOrderId

UPDATE Customers Set Customers.TotalDue = (Customers.TotalDue - @InstallmentAmount)
from CreditSales s
JOIN Customers ON Customers.CustomerID = s.CustomerID

DELETE CreditSalesSchedules WHERE CreditSalesID = @SalesOrderId

INSERT INTO CreditSalesSchedules
(
MonthDate, Balance, InstallmentAmt, PaymentDate, 
CreditSalesID, PaymentStatus, InterestAmount, ClosingBalance, ScheduleNo, Remarks, IsUnExpected,HireValue,NetValue
)
(
Select MonthDate, Balance, InstallmentAmt, PaymentDate, 
@SalesOrderId CreditSalesID, PaymentStatus, InterestAmount, ClosingBalance, 
ScheduleNo, Remarks, IsUnExpected,HireValue,NetValue from @Schedules
)

RETURN


--------------------------------------
--------------------------------------

--exec CreditSalesPenaltySchedules 1
ALTER PROCEDURE [dbo].[CreditSalesPenaltySchedules]
(@SalesOrderId int)
AS

BEGIN
Declare 

        @TSalesAmt Decimal(18,2),
        @FirstNum Decimal(18,2),
        @FirstNumUpto Decimal(18,2),
        @FirstNumPaid Decimal(18,2),
        @FirstNumDue Decimal(18,2),
        @LastDate DateTime,
		@FirstDate DateTime,
        @FirstTotal Decimal(18,2),
        @RemainingTotal Decimal(18,2),
        @FirstInsAmt Decimal(18,2),
        @no int,
        @InsAmtNo int,
        @Status int,
        @upto Decimal(18,2),
        @id INT,
		@CustomerID INT,
        @CreditSalesID INT 

DECLARE @Min INT
DECLARE @Max INT

declare @field1 int
declare @field2 int
declare cur CURSOR LOCAL for  select CreditSalesID,Status from CreditSales --where CreditSalesID=@SalesOrderId
open cur

            fetch next from cur into @field1, @field2
            set @no=0
while @@FETCH_STATUS = 0
 BEGIN 
                 
                set @CreditSalesID=@field1
                set @TSalesAmt=0
                Set @FirstNum=0
                Set @RemainingTotal=(select Sum(InstallmentAmt) from CreditSalesSchedules where PaymentStatus='Due' and CreditSalesID=@CreditSalesID)
                Set @TSalesAmt=(select SUM(MPRateTotal) from CreditSaleDetails  where CreditSalesID=@CreditSalesID)
                Set @FirstNumPaid=(Select COUNT(*) from CreditSalesSchedules where CreditSalesID=@CreditSalesID and PaymentStatus='Paid')
				Set @FirstDate=(Select MIN(MonthDate) from CreditSalesSchedules where CreditSalesID=@CreditSalesID)			 
			   -- Set @LastDate=(Select MIN(MonthDate) from CreditSalesSchedules where PaymentStatus='Due' and CreditSalesID=@CreditSalesID)
                Set @InsAmtNo=(Select COUNT(*) from CreditSalesSchedules where CreditSalesID=@CreditSalesID )
                Set @Status=(select Status from CreditSales where CreditSalesID=@CreditSalesID)
                set @CustomerID=(select CustomerID  from CreditSales where CreditSalesID= @CreditSalesID)
				SET @LastDate=DATEADD(MONTH,5,@FirstDate)
				print @LastDate
				--print @LastDate
                IF  (DATEADD(DAY,-15, GETDATE())>@LastDate and  @Status=0 and  CONVERT(DECIMAL(16,2), @RemainingTotal) >'0')
                    BEGIN
                    set @no=@no+1
                            Set @FirstTotal=Convert(decimal,ISNULL(@RemainingTotal,0))+Convert(decimal,ISNULL( @TSalesAmt,0))*.15
                            --print  @TSalesAmt
                            Set @FirstInsAmt=Convert(Decimal,ISNULL( @FirstTotal,0.0))/6
							print @FirstInsAmt
                            --print @FirstTotal
							update Customers set TotalDue=TotalDue-@RemainingTotal where CustomerID= @CustomerID
                            delete from CreditSalesSchedules where CreditSalesID=@CreditSalesID and PaymentStatus='Due'
                            Set @Upto=6
                            Set @id=1 
                                   WHILE @id<= @Upto
                                    BEGIN

                                    insert into CreditSalesSchedules(MonthDate,Balance,InstallmentAmt,PaymentDate,CreditSalesID,PaymentStatus,InterestAmount,ClosingBalance,ScheduleNo,Remarks,IsUnExpected) VAlues
									(DATEADD(month,@id, @LastDate),@FirstInsAmt*(@Upto-(@id-1)),@FirstInsAmt,DATEADD(month,@id, @LastDate),@CreditSalesID,'Due',ISNUll(0,0.0),@FirstInsAmt*(@Upto-(@id-1))-@FirstInsAmt,@FirstNumPaid+@id,ISNUll(0,0.0),1)
									
                                    set @id=@id+1;

                                    END 
									update CreditSales set PenaltyInterest=PenaltyInterest+	Convert(decimal,ISNULL( @TSalesAmt,0))*.15 where CreditSalesID=@CreditSalesID
                                    update CreditSales set Status=1 where CreditSalesID=@CreditSalesID
									update CreditSales set Remaining=@FirstTotal where CreditSalesID=@CreditSalesID
								    update Customers set TotalDue=TotalDue+@FirstTotal where CustomerID= @CustomerID
 
                   END
				 
                  ELSE IF  (DATEADD(DAY,-15, GETDATE())>@LastDate and  @Status=1)
                   BEGIN
                            Set @FirstTotal=Convert(decimal,ISNULL( @RemainingTotal,0))+Convert(decimal,ISNULL( @RemainingTotal,0))*.15
                            --print  @TSalesAmt
                            Set @FirstInsAmt=Convert(Decimal,ISNULL(@FirstTotal,0.0))/(12)
                            --print @FirstTotal

							update Customers set TotalDue=TotalDue-@RemainingTotal where CustomerID= @CustomerID
                            delete from CreditSalesSchedules where CreditSalesID=@CreditSalesID and PaymentStatus='Due'
                            Set @Upto=(12)
                            Set @id=1 
                                   WHILE @id<= @Upto
                                    BEGIN

                                    insert into CreditSalesSchedules(MonthDate,Balance,InstallmentAmt,PaymentDate,CreditSalesID,PaymentStatus,InterestAmount,ClosingBalance,ScheduleNo,Remarks,IsUnExpected) VAlues
									(DATEADD(month,@id, @LastDate),@FirstInsAmt*(@Upto-(@id-1)),@FirstInsAmt,DATEADD(month,@id, @LastDate),@CreditSalesID,'Due',ISNUll(0,0.0),@FirstInsAmt*(@Upto-(@id-1))-@FirstInsAmt,@FirstNumPaid+@id,ISNUll(0,0.0),1)

                                    set @id=@id+1;

                                    END 
								 update CreditSales set PenaltyInterest=PenaltyInterest+Convert(decimal,ISNULL( @RemainingTotal,0))*.15 where CreditSalesID=@CreditSalesID
                                 update CreditSales set Status=2 where CreditSalesID=@CreditSalesID
								 update CreditSales set Remaining=@FirstTotal where CreditSalesID=@CreditSalesID
								 update Customers set TotalDue=TotalDue+@FirstTotal where CustomerID= @CustomerID
								
                   END
                ELSE IF  (DATEADD(DAY,-15, GETDATE())>@LastDate and  @Status=2)

                   BEGIN
                            Set @FirstTotal=Convert(decimal,ISNULL( @RemainingTotal,0))+Convert(decimal,ISNULL( @RemainingTotal,0))*.15
                            --print  @TSalesAmt
                            Set @FirstInsAmt=Convert(Decimal,ISNULL(@FirstTotal,0.0))/(12)
                            --print @FirstTotal

							update Customers set TotalDue=TotalDue-@RemainingTotal where CustomerID= @CustomerID
                            delete from CreditSalesSchedules where CreditSalesID=@CreditSalesID and PaymentStatus='Due'
                            Set @Upto=(12)
                            Set @id=1 
                                   WHILE @id<= @Upto
                                    BEGIN

                                    insert into CreditSalesSchedules(MonthDate,Balance,InstallmentAmt,PaymentDate,CreditSalesID,PaymentStatus,InterestAmount,ClosingBalance,ScheduleNo,Remarks,IsUnExpected) VAlues
									(DATEADD(month,@id, @LastDate),@FirstInsAmt*(@Upto-(@id-1)),@FirstInsAmt,DATEADD(month,@id, @LastDate),@CreditSalesID,'Due',ISNUll(0,0.0),@FirstInsAmt*(@Upto-(@id-1))-@FirstInsAmt,@FirstNumPaid+@id,ISNUll(0,0.0),1)

                                    set @id=@id+1;

                                    END 
								  	    update CreditSales set PenaltyInterest=PenaltyInterest+Convert(decimal,ISNULL( @RemainingTotal,0))*.15 where CreditSalesID=@CreditSalesID
                                        update CreditSales set Status=3 where CreditSalesID=@CreditSalesID
										update CreditSales set Remaining=@FirstTotal where CreditSalesID=@CreditSalesID
								        update Customers set TotalDue=TotalDue+@FirstTotal where CustomerID= @CustomerID		
                   
				   END
                    fetch next from cur into @field1, @field2    
                END
                close cur
                deallocate cur
				 print @Upto
                
END


----------------------------------------------------------
----------------------------------------------------------

--Date: 11-APR-2018

--exec MonthlyBenefitReport '2000-04-25 00:00:00.000','2018-04-25 00:00:00.000',1
create PROCEDURE [dbo].[MonthlyBenefitReport]

(
@FromDate DATETIME,
@ToDate DATETIME,
@ConcernId int

)
 AS
 Declare
       
        @ToatalPurchaseAmt Decimal(18,2),
		@TotalSalesAmt Decimal(18,2),
		@NetBenefitAmt Decimal(18,2)

Create Table #Temp1(InvoiceDate DateTime,SalesTotal decimal(18,2),PurchaseTotal decimal(18,2),
TDAmount_Sale decimal(18,2),TDAmount_CreditSale decimal(18,2),FirstTotalInterest decimal(18,2),   
HireCollection decimal(18,2),   CreditSalesTotal decimal(18,2),CreditPurchase decimal(18,2),
TotalIncome decimal(18,2),Adjustment decimal(18,2),  LastPayAdjustment decimal(18,2), TotalExpense decimal(18,2))
 

 insert into #temp1
select MIN(InvoiceDate),
sum(UTAmount), sum(MPRate),MAX(TDAmount),0,0,0,0,0,0,0,0,0 from SOrderDetails SOD 
inner Join SOrders So on So.SOrderID=SOD.SOrderID 
where ConcernId=@ConcernId group by MONTH(InvoiceDate),YEAR(InvoiceDate),SO.SOrderID  


insert into #temp1 
select MIN(SalesDate),0,0,0,MAX(Discount),MAX(InterestAmount),0,sum(UTAmount),sum(MPRate),0,0,MAX(isnuLl(WInterestAmt,0)) ,0 from CreditSaleDetails CSP 
inner Join CreditSales CS on CS.CreditSalesID=CSP.CreditSalesID 
 where ConcernId=@ConcernId
group by CS.CreditSalesID

--insert into #temp1 
--select MIN(SalesDate),0,0,0,MAX(Discount),MAX(FirstTotalInterest)      ,0,sum(UTAmount),
--sum(MPRate),0,0,MAX(isnuLl(WInterestAmt,0)) ,0 from CreditSaleProducts CSP inner Join CreditSales CS on CS.CreditSalesID=CSP.CreditSalesID 
--group by CS.CreditSalesID

insert into #temp1
 select EntryDate,0,0,0,0,0,0,   0,0,0,0,0,Amount from Expenditures 
 join ExpenseItems on Expenditures.ExpenseItemID=ExpenseItems.ExpenseItemID
 where Status=1 and ConcernId=@ConcernId  --Total Expense
insert into #temp1 
select EntryDate,0,0,0,0,0,0, 0,0, 0,0,Amount,0 from Expenditures 
 join ExpenseItems on Expenditures.ExpenseItemID=ExpenseItems.ExpenseItemID
where Status=2  and ConcernId=@ConcernId -- othersIncome

insert into #temp1
select CSD.PaymentDate,0,0,0,0,0, HireValue  ,        0,0,0,0,0,0     from CreditSalesSchedules CSD 
join CreditSales CS on CS.CreditSalesID=CSD.CreditSalesID
 where CS.Status=1 and CSD.PaymentStatus='Paid' and ConcernId=@ConcernId

 


Create Table #Temp2(InvoiceDate DateTime,SalesTotal decimal(18,2),PurchaseTotal decimal(18,2),
TDAmount_Sale decimal(18,2),TDAmount_CreditSale decimal(18,2), 
FirstTotalInterest decimal(18,2),HireCollection decimal(18,2),    CreditSalesTotal decimal(18,2),
CreditPurchase decimal(18,2), CommisionProfit decimal(18,2),
HireProfit decimal(18,2),TotalProfit decimal(18,2), OthersIncome decimal(18,2),
 TotalIncome decimal(18,2),Adjustment decimal(18,2),LastPayAdjustment decimal(18,2), TotalExpense decimal(18,2), Benefit decimal(18,2)  )

insert into #Temp2  select convert(Date,Min(InvoiceDate)),sum(SalesTotal),sum(PurchaseTotal), 
sum(TDAmount_Sale ),sum(TDAmount_CreditSale) ,
suM( FirstTotalInterest -LastPayAdjustment) as 'FirstTotalInterest', sum(HireCollection -LastPayAdjustment) , sum(CreditSalesTotal),sum(CreditPurchase),
sum(SalesTotal+CreditSalesTotal-TDAmount_Sale -FirstTotalInterest-PurchaseTotal-CreditPurchase   -  case      when (LastPayAdjustment-HireCollection )>0  then LastPayAdjustment-HireCollection   else 0   end            )       as 'CommisionProfit',
sum(HireCollection -LastPayAdjustment) as 'HireProfit',
sum(SalesTotal+CreditSalesTotal-FirstTotalInterest-PurchaseTotal-CreditPurchase+HireCollection  -TDAmount_Sale  ) as 'TotalProfit',     sum(TotalIncome) as 'OthersIncome',
sum(SalesTotal+CreditSalesTotal-FirstTotalInterest-PurchaseTotal-CreditPurchase+HireCollection +  TotalIncome-TDAmount_Sale ) as 'TotalIncome',sum(Adjustment) as 'Adjustment',sum(LastPayAdjustment), sum(TotalExpense) as 'TotalExpense',
sum(SalesTotal+CreditSalesTotal-FirstTotalInterest-PurchaseTotal-CreditPurchase+HireCollection +  TotalIncome +TotalExpense -TDAmount_Sale-Adjustment ) 
 from #Temp1 group by MONTH(InvoiceDate),YEAR(InvoiceDate)

 select  * from #Temp2  where InvoiceDate>=@FromDate  and InvoiceDate<=@ToDate


---------------------------------------------------------------------
---------------------------------------------------------------------


create procedure [dbo].[sp_ProductWiseBenefitReport]
(
@Fromdate DateTime,
@ToDate DateTime,
@ConcernID int
)
AS
--exec sp_ProductWiseBenefitReport '2000-05-04 14:31:38.373', '2018-05-04 14:31:38.373',1

WITH Final_table 
AS  
( 
select P.Code, P.ProductID,P.ProductName,C.Description as 'CategoryName' ,STD.IMENO , SOD.UTAmount as 'SalesTotal',SOD.PPDAmount as 'Discount',
(SOD.MPRate*SOD.Quantity) as 'PurchaseTotal',  (SOD.UTAmount -(SOD.MPRate*SOD.Quantity))  as 'CommisionProfit'    ,0 as 'HireProfit' ,0 as 'HireCollection',(SOD.UTAmount -SOD.MPRate*SOD.Quantity) as'TotalProfit'
 from SOrderDetails  SOD 
 join SOrders SO on SOD.SOrderID=SO.SOrderID
join StockDetails STD on STD.SDetailID=SOD.SDetailID
join Products P on P.ProductID=STD.ProductID
join Categorys C on C.CategoryID=P.CategoryID

where SO.Status=1 and SO.InvoiceDate>=@Fromdate and SO.InvoiceDate<@ToDate and SO.ConcernID=@ConcernID
union
select  p.Code,P.ProductID,P.ProductName,C.Description as 'CategoryName' ,STD.IMENO, CSP.UTAmount as 'SalesTotal',  0 as 'Discount',
CSP.MPRateTotal as 'PurchaseTotal',(CSP.UTAmount-CSP.MPRateTotal-IntTotalAmt) as 'CommisionProfit' ,  IntTotalAmt  as 'HireProfit',0 as 'HireCollection',   (CSP.UTAmount-CSP.MPRateTotal-IntTotalAmt) as'TotalProfit'
 from CreditSaleDetails CSP
join CreditSales CS on CS.CreditSalesID=CSP.CreditSalesID 
join StockDetails STD on STD.SDetailID=CSP.StockDetailID
join Products P on P.ProductID=STD.ProductID
join Categorys C on C.CategoryID=P.CategoryID
  where CS.Status=1 and CS.SalesDate>=@Fromdate and CS.SalesDate<=@ToDate and CS.ConcernID=@ConcernID

union

  select P.Code,P.ProductID,P.ProductName,C.Description as 'CategoryName' ,Std.IMENO, 0 as 'SalesTotal', 0 as 'Discount',
  
   0 as 'PurchaseTotal',0 as 'CommisionProfit',0 as'HireProfit',   SUM(HireValue)  as 'HireCollection',  SUM(HireValue)  as 'TotalProfit'    from CreditSales CS 
  join CreditSaleDetails CSP on CSP.CreditSalesID=CS.CreditSalesID
  join CreditSalesSchedules CSD on CSD.CreditSalesID=CS.CreditSalesID
  join StockDetails STD on  STD.SDetailID=CSP.StockDetailID
  join Products P on P.ProductID=STD.ProductID
  join Categorys C on C.CategoryID=P.CategoryID
  where Cs.Status=1 and CS.SalesDate>=@Fromdate and CS.SalesDate<=@ToDate and CSD.PaymentStatus='Paid' and CS.ConcernID=@ConcernID
  group by  P.Code,P.ProductID,P.ProductName,C.Description ,Std.IMENO



)  select Code, ProductID,ProductName,CategoryName,IMENO, sum(SalesTotal) as 'SalesTotal',sum(Discount) as 'Discount',sum(SalesTotal-Discount) as 'NetSales',
sum(PurchaseTotal) as 'PurchaseTotal' ,sum(CommisionProfit) as 'CommisionProfit' ,sum(HireProfit)  as 'HireProfit',  sum(HireCollection) as 'HireCollection',      sum(TotalProfit ) as 'TotalProfit'    from Final_table group by  Code,ProductID,ProductName,CategoryName,IMENO
  






--Date: 26-Mar-2018
ALTER PROC [dbo].[AddCreditSalesOrder]
(
	@CSalesOrder  [dbo].[InsertCreditSalesOrderTable] readonly,
	@CSODetails [InsertCSODetailTable] readonly,
	@CSSchedules [InsertCSSchedulesTable] readonly
)
  
AS 
DECLARE @CreditSalesId int
  
INSERT INTO CreditSales
(InvoiceNo, CustomerID, TSalesAmt, NoOfInstallment,
IntallmentPrinciple,IssueDate,UserName,Remaining, InterestRate,InterestAmount, SalesDate,
DownPayment, WInterestAmt, FixedAmt, IsStatus, UnExInstallment, Quantity,
Discount, NetAmount, ISUnexpected, Remarks, VATPercentage, VATAmount, Status,
ConcernID, CreatedBy, CreateDate,TotalOffer 
)
(
Select InvoiceNo, CustomerID, TSalesAmt, NoOfInstallment,
InstallmentPrinciple, IssueDate,UserName,Remaining,InterestRate,InterestAmount , SalesDate,
DownPayment, WInterestAmt, FixedAmount, IsStatus, UnExInstallment, Quantity,
Discount, NetAmount, ISUnexpected, Remarks, VATPercentage, VATAmount, Status,
ConcernID, CreatedBy, CreateDate,TotalOffer from @CSalesOrder
)

UPDATE Customers Set Customers.TotalDue = (Customers.TotalDue + s.TotalDue)
from @CSalesOrder s
JOIN Customers ON Customers.CustomerID = s.CustomerID

SET @CreditSalesId = SCOPE_IDENTITY()

INSERT INTO CreditSaleDetails
(
ProductID ,UnitPrice, Quantity, UTAmount, CreditSalesID, 
MPRateTotal, MPRate, StockDetailID,PPOffer,IntPercentage,IntTotalAmt
)
(
Select ProductID ,UnitPrice, Quantity, UTAmount, @CreditSalesId CreditSalesID, 
MPRateTotal, MrpRate, StockDetailID,PPOffer,IntPercentage,IntTotalAmt from @CSODetails
)

INSERT INTO CreditSalesSchedules
(
MonthDate, Balance, InstallmentAmt, PaymentDate, 
CreditSalesID, PaymentStatus, InterestAmount, ClosingBalance, ScheduleNo, Remarks, IsUnExpected
)
(
Select MonthDate, Balance, InstallmentAmt, PaymentDate, 
@CreditSalesId CreditSalesID, PaymentStatus, InterestAmount, ClosingBalance, 
ScheduleNo, Remarks, IsUnExpected from @CSSchedules
)

UPDATE Stocks
SET Stocks.Quantity = (Stocks.Quantity - s.Quantity)
from @CSODetails s
JOIN Stocks ON Stocks.ProductID = s.ProductId AND Stocks.ColorID = s.ColorId

UPDATE StockDetails
SET StockDetails.Status = 2
from @CSODetails s
JOIN StockDetails ON StockDetails.SDetailID = s.StockDetailId
join Products on StockDetails.ProductID = Products.ProductID
where Products.ProductType!=2
RETURN


--------------------------------------------------------------------------------------------------------------

ALTER PROC [dbo].[AddPurchaseOrder]
(
	@PurchaseOrder [InsertPurchaseOrderTable] readonly,
	@PODetails [InsertPODetailTable] readonly,
	@POProductDetails [InsertPOProductDetailTable] readonly,
	@Stocks [InsertStockTable] readonly,
	@StockDetails [InsertStockDetailTable] readonly
)
  
AS 
DECLARE @PurchaseOrderId int
  
INSERT INTO POrders
(
OrderDate, ChallanNo, SupplierID, GrandTotal, TDiscount, TotalAmt, AdjAmount, RecAmt, PaymentDue, TotalDue,
Status, PPDisAmt, NetDiscount, LaborCost, ConcernID, CreateDate, CreatedBy 
)
(
Select OrderDate, ChallanNo, SupplierID, GrandTotal, TDiscount, TotalAmt, AdjAmount, RecAmt, PaymentDue, TotalDue,
Status, PPTDisAmt, NetDiscount, LabourCost, ConcernID, CreateDate, CreatedBy from @PurchaseOrder)

UPDATE Suppliers Set Suppliers.TotalDue = (Suppliers.TotalDue + p.TotalDue)
from @PurchaseOrder p
JOIN Suppliers ON Suppliers.SupplierID = p.SupplierId

SET @PurchaseOrderId = SCOPE_IDENTITY()

INSERT INTO PriceProtections
(
ProductID, ColorID, PrvPrice, ChangePrice, POrderID, ConcernID, SupplierID, PrvStockQty, PChangeDate
)
(
Select stk.ProductId, stk.ColorId, stk.MRPPrice, stkt.MRPPrice, @PurchaseOrderId, stk.ConcernID, (Select SupplierId From POrders Where POrderID = @PurchaseOrderId), stk.Quantity, GETDATE()
From @Stocks stkt INNER JOIN Stocks stk ON stk.ProductID = stkt.ProductId AND stk.ColorID = stkt.ColorId
Where stk.MRPPrice > stkt.MRPPrice
)

INSERT INTO POrderDetails
(
ProductID, ColorID, Quantity, UnitPrice, TAmount, PPDISPer, PPDISAmt, MRPRate, POrderID
)
(
Select ProductId, ColorId, Quantity, UnitPrice, TAmount,  PPDisPer,
PPDisAmt, MrpRate, @PurchaseOrderId POrderID from @PODetails)

INSERT INTO POProductDetails
(
ProductID, ColorID, IMENO, POrderDetailID
)
(
Select POPD.ProductId, POPD.ColorId, POPD.IMENo, POD.POrderDetailID from @POProductDetails POPD
INNER JOIN POrderDetails POD ON POD.ProductID = POPD.ProductId AND POD.ColorID = POPD.ColorId AND POD.POrderID = @PurchaseOrderId
)

UPDATE Stocks
SET Stocks.Quantity = (Stocks.Quantity + s.Quantity), Stocks.MRPPrice = s.MRPPrice,
LPPrice = s.LPPrice, ModifiedBy = s.CreatedBy, ModifiedDate =  s.CreateDate
from @Stocks s
JOIN Stocks ON Stocks.StockID = s.StockID

INSERT INTO Stocks
(
StockCode, EntryDate, Quantity, ProductID, ColorID, MRPPrice, LPPrice, ConcernID, CreatedBy, CreateDate
)
(
Select StockCode, EntryDate, Quantity, ProductId, ColorId, MRPPrice, LPPrice, ConcernId, CreatedBy,
CreateDate from @Stocks Where StockId Is Null
)

--For No Barcode
update StockDetails set status=2
from @StockDetails dtsd
join StockDetails on StockDetails.ColorID = dtsd.ColorId and StockDetails.ProductId = dtsd.ProductId and StockDetails.IMENo = dtsd.IMENo

INSERT INTO StockDetails
(
StockID, StockCode, ProductID, ColorID, POrderDetailID, IMENO, Status,PRate
)
(
Select stk.StockID, stkd.StockCode, stkd.ProductId, stkd.ColorId, POD.POrderDetailID, stkd.IMENo, stkd.Status,POD.UnitPrice from @StockDetails stkd
INNER JOIN Stocks stk ON stk.ProductID = stkd.ProductId AND stk.ColorID = stkd.ColorID
INNER JOIN POrderDetails POD ON POD.ProductID = stkd.ProductId AND POD.ColorID = stkd.ColorId AND POD.POrderID = @PurchaseOrderId
)

RETURN


------------------------------------------------------------------------------------------------------


ALTER PROC [dbo].[AddSalesOrder]
(
	@SalesOrder [InsertSalesOrderTable] readonly,
	@SODetails [InsertSODetailTable] readonly
)
  
AS 
DECLARE @SalesOrderId int
  
INSERT INTO SOrders
(
InvoiceDate, InvoiceNo, CustomerID, VATPercentage, VATAmount, GrandTotal, TDPercentage, TDAmount, NetDiscount,TotalAmount, 
PaymentDue, RecAmount, AdjAmount, TotalDue, Status, ConcernID, CreatedBy, CreateDate,TotalOffer
)
(
Select InvoiceDate, InvoiceNo, CustomerID, VATPercentage, VATAmount, GrandTotal, TDiscountPercentage, TDiscountAmount,NetDiscount,
TotalAmount, PaymentDue, RecAmt, AdjAmount, TotalDue, Status, ConcernID, CreatedBy, CreateDate,TotalOffer from @SalesOrder)

UPDATE Customers Set Customers.TotalDue = (Customers.TotalDue + s.TotalDue)
from @SalesOrder s
JOIN Customers ON Customers.CustomerID = s.CustomerID

SET @SalesOrderId = SCOPE_IDENTITY()

INSERT INTO SOrderDetails
(
ProductID, Quantity, UnitPrice, UTAmount, PPDPercentage, PPDAmount, MPRate, SOrderID, SDetailId,PPOffer
)
(
Select ProductId, Quantity, UnitPrice, TAmount,  PPDisPer,
PPDisAmt, MrpRate, @SalesOrderId SOrderID, StockDetailId,PPOffer from @SODetails)

UPDATE Stocks
SET Stocks.Quantity = (Stocks.Quantity - s.Quantity)
from @SODetails s
JOIN Stocks ON Stocks.ProductID = s.ProductId AND Stocks.ColorID= s.ColorId

UPDATE StockDetails
SET StockDetails.Status = 2
from @SODetails s
JOIN StockDetails ON StockDetails.SDetailID = s.StockDetailId
join Products on StockDetails.ProductID = Products.ProductID
where Products.ProductType!=2
RETURN





--Date: 25-Mar-2018
--exec sp_ReturnReport '2010-12-30 00:00:00.000', '2018-12-30 00:00:00.000',3
alter proc sp_ReturnReport
(
  @fromDate datetime,
  @toDate datetime,
  @ConcernID int
)
as
select SO.Invoicedate as 'ReturnDate',SO.InvoiceNo as 'ReturnInvoice',CUS.CustomerID,CUS.Code as 'CustomerCode',Cus.Name as 'CustomerName',Cus.Address as 'CustomerAddress',cus.ContactNo as 'CustomerMobile',SO.Remarks,STD.IMENO as 'ReturnIMEI',p.ProductName,SOD.Quantity as 'ReturnQty',SOD.UnitPrice as 'ReturnAmount' from SOrders SO
join SORderDetails SOD on SO.SOrderID = SOD.SOrderID
join Products P on SOD.ProductID = p.ProductID
join Stockdetails STD on SOD.SDetailID = STD.SDetailID
join Customers CUS on SO.CustomerID = CUS.CustomerID
where SO.Status=2 and SO.InvoiceDate>= @fromDate and SO.InvoiceDate<= @toDate and SO.ConcernID=@ConcernID order by SO.InvoiceDate desc


---------------------------------------------
--exec  sp_ReplacemnetReport '2017-04-25 00:00:00.000','2018-04-25 00:00:00.000',3

ALTER proc [dbo].[sp_ReplacemnetReport]
(
@FromDate DATETIME,
@ToDate DATETIME,
@ConcernID int 
)
as

select SO.InvoiceDate from SORders SO
join SOrderDetails SOD on SO.SOrderID = SOD.RepOrderID
join Customers CUS on SO.CustomerID = CUS.CustomerID
join StockDetails STD on SOD.SDetailID = STD.SDetailID
where IsReplacement=1 and SO.ConcernId=@ConcernID and CreateDate>=@FromDate and CreateDate<=@ToDate