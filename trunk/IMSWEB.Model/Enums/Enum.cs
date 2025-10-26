﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace IMSWEB.Model
{
    public enum ToastType
    {
        Error,
        Info,
        Success,
        Warning
    }

    public enum PickerType
    {
        Category,
        Company,
        Color,
        Customer,
        Supplier,
        Employee,
        Product,
        ProductDetail,
        ProductDetailMobile,
        SalesProductDetail,
        ExpenseItemHead,
        IncomeItemHead,
        StockProduct,
        SRProductDetail,
        ExceptCreditCustomer,
        CreditProductDetail,
        CreditCustomer,
        Bank,
        AnotherBank,
        DamageProductDetail,
        Allowance,
        Deduction,
        MSEmployee,
        Department,
        Godown,
        Size,
        ProductPickerFiltered,
        InvestmentHeads,
        MSCustomer,
        Territory,
        Depot,
        CCBank,
        DiaSize,
        SisterConcern,
        CustomerSalesProduct,
        FinGoods,
        ProductionProducts,
        PickEmployee,
        SupplierProducts,
        ManualProductionPicker,
        CombineProduct
    }

    public enum EnumStatus
    {
        Live = 1,
        Discontinue = 2,
        New = 3,
        Updated = 4,
        Deleted = 5
    }

    public enum EnumCompanyTransaction
    {
        Expense = 1,
        Income = 2
    }

    public enum EnumCustomerType
    {
        Retail = 1,
        Dealer = 2,
        //Credit = 3
    }


    //public enum EnumTransactionType
    //{
    //    Deposit = 1,
    //    Withdraw = 2,
    //    CashCollection = 3,
    //    CashDelivery = 4,
    //    FundTransfer = 5
    //}

    public enum EnumTransactionType
    {
        Deposit = 1,
        Withdraw = 2,
        [Display(Name = "Cash Collection")]
        CashCollection = 3,

        [Display(Name = "Cash Delivery")]
        CashDelivery = 4,

        [Display(Name = "Fund Transfer")]
        FundTransfer = 5,

        [Display(Name = "Bank Expense")]
        BankExpense = 6,

        [Display(Name = "Bank Income")]
        BankIncome = 7,

        [Display(Name = "Liability Pay")]
        LiaPay = 8,

        [Display(Name = "Liability Receive")]
        LiaRec = 9
    }
    public enum EnumSalesType
    {
        Sales = 1,
        Return = 2,
        //Replace=3,
        ProductReturn = 4,
        Pending = 5,
        Advance = 6
    }

    public enum EnumPurchaseType
    {
        Purchase = 1,
        Return = 2,
        DeliveryOrder = 3,
        DamageReturn = 4,
        ProductReturn = 5,
        RawStockDeduct = 6,
    }

    public enum EnumSRVisitType
    {
        Live = 1,
        Cancel = 2
    }

    public enum EnumSRVProductDetailsStatus
    {
        Stock = 1,
        Sold = 2,
        SRVisitReturn = 3,
        SalesReturn = 4, //Product Wise Sales Return
    }
    public enum EnumStockStatus
    {
        Stock = 1,
        Sold = 2,
        Return = 3
    }

    public enum EnumSupplierType
    {
        Company = 1
    }

    public enum EnumCategoryType
    {
        Customer = 1,
        Company = 2,
        Product = 3
    }

    public enum EnumTranType
    {
        FromCustomer = 1,
        ToCompany = 2,
        [Display(Name = "Cash Collection Return")]
        CollectionReturn = 3,
        CollectionPending = 4,
        DeliveryPending = 5,
        [Display(Name = "Previous Sales Retrun")]
        PreviousSalesRetrun = 6,
        [Display(Name = "Sales Comission")]
        SalesCommission = 7,

    }

    public enum EnumDropdownTranType
    {
        [Display(Name = "Collection")]
        FromCustomer = 1,
        [Display(Name = "Previous Sales Retrun")]
        PreviousSalesRetrun = 6,
        [Display(Name = "Sales Comission")]
        SalesCommission = 7,
        [Display(Name = "Cash Collection Return")]
        CollectionReturn = 3,
    }

    public enum EnumPayType
    {
        Cash = 1,
        Bank = 2
        //Cheque = 2,
        //bKash = 3,
        //Banking = 4,
        //TT = 5,
        //Online = 6
    }

    public enum EnumSalesOfferType
    {
        PCS = 1,
        FlatAmout = 2,
        TotalValuePer = 3
    }

    //public enum EnumSalesOfferStatus
    //{
    //    Live = 1,
    //    Freeze = 2
    //}

    public enum EnumOfferStatus
    {
        Ongoing = 1,
        Freeze = 2
    }

    public enum EnumUserType
    {
        Administrator = 1,
        Normal = 2

    }

    public enum EnumUserStatus
    {
        Active = 1,
        InActive = 2
    }
    public enum EnumUserRoles
    {
        Admin,
        LocalAdmin,
        Manager,
        //MobileShop,
        MobileUser,
        SalesMan,
        superadmin,
        VATManager,
        RetailManager

    }

    public enum EnumDataUpload
    {
        Product = 1,
        Customer = 2,
        Supplier = 3,
        Sales_Order = 4,
        Cradit_Sales = 5
    }

    public enum EnumUnitType
    {
        PCS = 1,
        DZN = 2,
        Feet = 3,
        Carton = 4,
        COIL_109 = 5,
        COIL_100 = 6,
        BOX = 7,
        KG = 8,
        CFT = 9,
        Yards = 10,
        SFT = 11
    }
    public enum EnumProductType
    {
        [Display(Name="Existing Barcode")]
        ExistingBC = 1,
        [Display(Name = "No Barcode")]
        NoBarcode = 2,
        [Display(Name = "Auto Barcode")]
        AutoBC = 3
    }

    [Serializable]
    public enum EnumAllowOrDeduct
    {
        Allowance = 1,
        Deduction = 2,
    }

    public enum EnumActiveInactive
    {
        Active = 1,
        [Display(Name = "In Active")]
        InActive = 2
    }

    public enum EnumBloodGroup
    {
        None = 1,
        APos = 2,
        ANeg = 3,
        BPos = 4,
        BNeg = 5,
        OPos = 6,
        ONeg = 7,
        ABPos = 8,
        ABNeg = 9
    }

    public enum EnumMaritalStatus
    {
        None = 0,
        Married = 1,
        UnMarried = 2
    }
    public enum EnumPaymentMode
    {
        Cash = 1,
        Cheque = 2,
        BankTransfer = 3
    }

    public enum EnumPeriodicity
    {
        Monthly = 1,
        OnceOff = 2,
    }

    public enum EnumEntitleType
    {
        Grade = 1,
        Individual = 2,
    }

    public enum EnumHolidayType
    {
        WeeklyHoliday = 1,
        SpecialHoliday = 2
    }
    public enum EnumDepartment
    {
        Management = 1,
        Sales = 2,
        HR = 3
    }

    public enum EnumDaysOfWeek
    {
        Saturday = 1,
        Sunday = 2,
        Monday = 3,
        Tuesday = 4,
        Wednesday = 5,
        Thursday = 6,
        Friday = 7
    }

    public enum EnumEmployeeLeaveType
    {
        DayLeave = 1,
        ShortLeave = 2
    }
    public enum EnumEmployeeLeaveStatus
    {
        Pending = 1,
        Approved = 2
    }
    public enum EnumSalaryItemCode
    {
        Basic_Salary = -101,
        Over_Time_Hours = -102,
        Over_Time_Amount = -103,
        Bonus = -107,
        Allowance = -113,
        Deduction = -115,
        Advance_Deduction = -116,
        Loan_Monthly_Installment = -118,
        Loan_Monthly_Interest = -119,
        Loan_Payment = -201,
        Loan_Remain_Installment = -124,
        Loan_Remain_Interest = -125,
        Loan_Remain_Balance = -126,
        PF_Contribution = -128,
        Inc_Tax_Deduction = -129,
        Inc_Tax_Tot_Taxable = -130,
        Inc_Tax_Yearly_Amount = -131,
        Net_Payable = -132,
        Tot_UnauthLeave_Days = -133,
        Tot_Arrear_Days = -134,
        Tot_Attend_Days = -135,
        Conv_Days = -136,
        Short_Leave_Amount = -137,
        OPI = -138,
        Leave_Days = -139,
        Total_HoliDays = -140,
        Tot_Attend_Days_Amount = -141,
        Tot_Attend_Days_Bonus = -142,
        Gross_Salary = -143,
        Commission = -144,
        Extra_Commission = -145,
        Target_Failed_Deduct = -146,
        Voltage_StabilizerComm = -147,
        Due_Salary = -148,
        Due_SalaryPay = -149

    }
    public enum EnumSalaryGroup
    {
        Gross = 1,
        UnauthLeave = 2,
        Deductions = 3,
        Miscellaneous = 4,
        OtherItem = 5,
        Arrear = 8,
        Allowance = 9
    }
    public enum EnumDesignation
    {
        HR_Manager = 1,
        Sales_Manager = 2,
        Operation_Manager = 3,
        Audit_Manager = 4,
        Software_Engineer = 5,
        Staff = 6,
        Worker = 7,
        Helper = 8,
        Operator = 9
    }
    public enum EnumSisterConcern
    {
        SAMSUNG_ELECTRA_CONCERNID = 1,
        NOKIA_CONCERNID = 2,
        WALTON_CONCERNID = 3,
        KINGSTAR_CONCERNID = 4,
        HAWRA_ENTERPRISE_CONCERNID = 5,
        HAVEN_ENTERPRISE_CONCERNID = 6,
        NOKIA_STORE_MAGURA_CONCERNID = 7,
        KT_Feed = 19
    }

    public enum EnumCommissionType
    {
        VoltageStabilizerComm = 1,
        ExtraComm = 2
    }

    public enum EnumInvestmentHeadType
    {
        FixedAsset = 2,
        CurrentAsset = 3,
        Liability = 4
    }

    public enum EnumInvestTransType
    {
        Receive = 1,
        Pay = 2
    }

    public enum EnumSMSType
    {
        SalesTime = 1,
        PurchaseTime = 2,
        CashCollection = 3,
        InstallmentCollection = 4,
        InstallmentAlert = 5,
        Offer = 6,
        Error = 7,
        Registration = 8,
        CashCollectionReturn = 9,
        Promotional=10,
        OCTGreetings =11

    }

    public enum EnumSMSSendStatus
    {
        Success = 1,
        Fail = 2,
        Pending = 3
    }


    public enum EnumTransferStatus
    {
        Transfer = 1,
        Return = 2
    }

    public enum EnumOpeningType
    {
        Payment = 1,
        Receive = 2
    }

    public enum EnumInvestmentType
    {
        [Display(Name = "Fixed Asset")]
        FixedAsset = 1,
        [Display(Name = "Current Asset")]
        CurrentAsset = 2,
        [Display(Name = "Liability")]
        Liability = 3,
        PF = 4,
        FDR = 5,
        Security = 6
    }


    public enum EnumLiabilityType
    {
        [Display(Name = "Take Loan")]
        TakeLoan = 1,
        [Display(Name = "Loan Collection")]
        LoanCollection = 2,

        [Display(Name = "Give Loan")]
        GiveLoan = 3,
        [Display(Name = "Loan Payment")]
        LoanPay = 4,

        [Display(Name = "Take PF")]
        TakePF = 5,
        [Display(Name = "Give PF")]
        GivePF = 6,

        [Display(Name = "Take FDR")]
        TakeFDR = 7,
        [Display(Name = "Give FDR")]
        GiveFDR = 8,

        [Display(Name = "Take Security")]
        TakeSecurity = 9,
        [Display(Name = "Give Security")]
        GiveSecurity = 10
    }


    public enum EnumLiabilityRecType
    {
        [Display(Name = "Take Loan")]
        TakeLoan = 1,
        [Display(Name = "Loan Collection")]
        LoanCollection = 2,

    }

    public enum EnumLiabilityPayType
    {
        [Display(Name = "Give Loan")]
        GiveLoan = 3,
        [Display(Name = "Loan Payment")]
        LoanPay = 4
    }


    public enum EnumOnnoRokomSMSType
    {
        #region Using Username and Password
        OneToOne,
        OneToMany,
        DeliveryStatus,
        GetBalance,
        #endregion

        #region Using API key
        NumberSms,
        ListSms,
        GetCurrentBalance,
        REVEGETStatus
        #endregion

    }

    public enum EnumSalaryType
    {
        [Display(Name = "Advance Salary")]
        AdvanceSalary = 1,
        [Display(Name = " Due Salary")]
        DueSalary = 2,
        [Display(Name = "Due Salary Pay")]
        DueSalayPay = 3,
        [Display(Name = "Advance Loan")]
        AdvanceLoan = 4
    }

    public enum EnumTerms
    {
        Cash = 1,
        Credit = 2
    }

    public enum EnumProductStockType
    {
        Raw_Materials = 1,
        Finished_Goods = 2,
    }
    public enum EnumProductionStatus
    {
        Production = 1,
        Return = 2,
    }


    public enum EnumProcutCategory
    {
        Mosquitos = 1,
        Ants,
        Termites,
        Fleas
    }

    public enum EnumDocType
    {
        WebsiteProduct = 1,
        AboutUs = 2
       
    }
}
