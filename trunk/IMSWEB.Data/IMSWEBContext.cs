using IMSWEB.Model;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Data
{
    public class IMSWEBContext : IdentityDbContext<ApplicationUser, ApplicationRole,
                                    int, ApplicationUserLogin, ApplicationUserRole, ApplicationUserClaim>
    {
        public IMSWEBContext()
            : base("IMSWEB")
        {
            Database.SetInitializer<IMSWEBContext>(null);
            Configuration.LazyLoadingEnabled = false;
        }

        public virtual bool Commit()
        {
            try
            {
              return  base.SaveChanges()>0;
            }
            catch (DbEntityValidationException ex)
            {
                var errorMessages = ex.EntityValidationErrors
                        .SelectMany(x => x.ValidationErrors)
                        .Select(x => x.ErrorMessage);

                var fullErrorMessage = string.Join("; ", errorMessages);
                var exceptionMessage = string.Concat(ex.Message, " The validation errors are: ", fullErrorMessage);
                throw new DbEntityValidationException(exceptionMessage, ex.EntityValidationErrors);
            }
        }

        #region DbSets IMS
        public virtual DbSet<CashCollection> CashCollections { get; set; }
        public virtual DbSet<Category> Categorys { get; set; }
        public virtual DbSet<Bank> Banks { get; set; }
        public virtual DbSet<BankTransaction> BankTransactions { get; set; }
        public virtual DbSet<Company> Companies { get; set; }
        public virtual DbSet<Godown> Godowns { get; set; }
        public virtual DbSet<TransferHistory> TransferHistorys { get; set; }
        public virtual DbSet<CreditSaleDetails> CreditSaleDetails { get; set; }
        public virtual DbSet<CreditSale> CreditSales { get; set; }
        public virtual DbSet<CreditSalesSchedule> CreditSalesSchedules { get; set; }
        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<DamageProduct> DamageProducts { get; set; }
        public virtual DbSet<Designation> Designations { get; set; }
        public virtual DbSet<Employee> Employees { get; set; }
        public virtual DbSet<Expenditure> Expenditures { get; set; }
        public virtual DbSet<ExpenseItem> ExpenseItems { get; set; }
        public virtual DbSet<Error> Errors { get; set; }
        public virtual DbSet<Color> Models { get; set; }
        public virtual DbSet<POProductDetail> POProductDetails { get; set; }
        public virtual DbSet<POrderDetail> POrderDetails { get; set; }
        public virtual DbSet<POrder> POrders { get; set; }
        public virtual DbSet<Product> Products { get; set; }

        public virtual DbSet<SaleOffer> SaleOffers { get; set; }
        public virtual DbSet<SisterConcern> SisterConcerns { get; set; }
        public virtual DbSet<SOrderDetail> SOrderDetails { get; set; }
        public virtual DbSet<SOrder> SOrders { get; set; }
        public virtual DbSet<StockDetail> StockDetails { get; set; }
        public virtual DbSet<Stock> Stocks { get; set; }
        public virtual DbSet<Supplier> Suppliers { get; set; }
        public virtual DbSet<sysdiagram> sysdiagrams { get; set; }
        public virtual DbSet<SystemInformation> SystemInformations { get; set; }
        public virtual DbSet<ApplicationRoleMenu> ApplicationRoleMenus { get; set; }
        public virtual DbSet<MenuItem> MenuItems { get; set; }
        public virtual DbSet<MenuPermission> MenuPermissions { get; set; }
        public virtual DbSet<Permission> Permissions { get; set; }
        public virtual DbSet<ROProductDetail> ROProductDetails { get; set; }
        public virtual DbSet<ROrderDetail> ROrderDetails { get; set; }
        public virtual DbSet<ROrder> ROrders { get; set; }

        public virtual DbSet<SRVProductDetail> SRVProductDetails { get; set; }
        public virtual DbSet<SRVisitDetail> SRVisitDetails { get; set; }
        public virtual DbSet<SRVisit> SRVisits { get; set; }
        public virtual DbSet<PriceProtection> PriceProtections { get; set; }
        public virtual DbSet<Size> Sizes { get; set; }
        public virtual DbSet<ProductUnitType> ProductUnitTypes { get; set; }

        public virtual DbSet<ShareInvestmentHead> ShareInvestmentHeads { get; set; }
        public virtual DbSet<ShareInvestment> ShareInvestments { get; set; }
        public virtual DbSet<EmployeeTargetSetup> EmployeeTargetSetups { get; set; }
        public virtual DbSet<MonthlyAttendence> MonthlyAttendences { get; set; }
        public virtual DbSet<ManualAttendence> ManualAttendences { get; set; }
        public virtual DbSet<EmployeeCommission> EmployeeCommissions { get; set; }
        public virtual DbSet<Territory> Territories { get; set; }
        public virtual DbSet<Depot> Depots { get; set; }
        public virtual DbSet<EmployeeWiseCustomerDue> EmployeeWiseCustomerDues { get; set; }
        public virtual DbSet<CCBank> CCBanks { get; set; }
        public virtual DbSet<DiaSize> DiaSizes { get; set; }
        public virtual DbSet<ProductionSetup> ProductionSetups { get; set; }
        public virtual DbSet<ProductionSetupDetail> ProductionSetupDetails { get; set; }
        public DbSet<Production> Productions { get; set; }
        public DbSet<ProductionDetail> ProductionDetails { get; set; }
        public DbSet<ProductionRawMaterial> ProductionRawMaterials { get; set; }
        public virtual DbSet<CashAccount> CashAccounts { get; set; }

        #endregion

        #region DbSets Payroll
        public virtual DbSet<AllowanceDeduction> AllowanceDeductions { get; set; }
        public virtual DbSet<Grade> Grades { get; set; }
        public virtual DbSet<Religion> Religions { get; set; }
        public virtual DbSet<EmpGradeSalaryAssignment> EmpGradeSalaryAssignments { get; set; }
        public virtual DbSet<GradeSalaryChangeType> GradeSalaryChangeTypes { get; set; }
        public virtual DbSet<ADParameterBasic> ADParameterBasics { get; set; }
        public virtual DbSet<ADParameterEmployee> ADParameterEmployees { get; set; }
        public virtual DbSet<ADParameterGrade> ADParameterGrades { get; set; }
        public virtual DbSet<Department> Departments { get; set; }
        public virtual DbSet<HolidayCalender> HolidayCalenders { get; set; }
        public virtual DbSet<EmployeeLeave> EmployeeLeaves { get; set; }
        public virtual DbSet<SalaryProcess> SalaryProcesses { get; set; }
        public virtual DbSet<SalaryMonthly> SalaryMonthlies { get; set; }
        public virtual DbSet<SalaryMonthlyDetail> SalaryMonthlyDetails { get; set; }
        public virtual DbSet<Attendence> Attendences { get; set; }
        public virtual DbSet<AttendenceMonth> AttendenceMonths { get; set; }
        public virtual DbSet<AdvanceSalary> AdvanceSalaries { get; set; }
        public virtual DbSet<TargetSetup> TargetSetups { get; set; }
        public virtual DbSet<TargetSetupDetail> TargetSetupDetails { get; set; }
        public virtual DbSet<CommissionSetup> CommissionSetups { get; set; }
        public virtual DbSet<ExtraCommissionSetup> ExtraCommissionSetups { get; set; }
        public virtual DbSet<DesWiseCommission> DesWiseCommissions { get; set; }

        #endregion

        #region DBSets payment informations
        public DbSet<ServiceCharge> ServiceCharges { get; set; }
        public DbSet<ServiceChargeDetails> ServiceChargeDetails { get; set; }
        #endregion
        public DbSet<SMSBillPayment> SMSBillPayments { get; set; }
        public DbSet<SMSFormate> SMSFormates { get; set; }
        public DbSet<SMSStatus> SMSStatuses { get; set; }
        public DbSet<SMSPaymentMaster> SMSPaymentMasters { get; set; }
        public DbSet<SMSPaymentMasterDetails> SMSPaymentMasterDetails { get; set; }


        #region Website Section 
        public DbSet<WebsiteProducts> WebsiteProducts { get; set; }
        public DbSet<DocumentsInfo> DocumentsInfo { get; set; } 
        public DbSet<AboutUS> AboutUs { get; set; } 
        #endregion 
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            #region Configurations
            modelBuilder.Entity<ApplicationUser>()
                .ToTable("Users");

            modelBuilder.Entity<ApplicationRole>()
                .ToTable("Roles");

            modelBuilder.Entity<ApplicationUserRole>()
                .ToTable("UserRoles");

            modelBuilder.Entity<ApplicationUserClaim>()
                .ToTable("UserClaims");

            modelBuilder.Entity<ApplicationUserLogin>()
                .ToTable("UserLogins");

            modelBuilder.Entity<Error>()
                .Property(e => e.Message)
                .IsRequired()
                .IsUnicode(false);

            modelBuilder.Entity<Error>()
                .Property(e => e.StackTrace)
                .IsRequired()
                .IsUnicode(false);

            modelBuilder.Entity<Error>()
                .Property(e => e.CreatedBy)
                .IsRequired();

            modelBuilder.Entity<Error>()
                .Property(e => e.CreateDate)
                .IsRequired();

            modelBuilder.Entity<Error>()
                .Property(e => e.ModifiedBy);

            modelBuilder.Entity<Error>()
                .Property(e => e.ModifiedDate);

            modelBuilder.Entity<CommissionSetup>()
                .Property(o => o.CommissionPercent).HasPrecision(18, 4);

            modelBuilder.Entity<Product>()
                .Property(x => x.PurchaseCSft).HasPrecision(18,6);
            modelBuilder.Entity<Product>()
               .Property(x => x.SalesCSft).HasPrecision(18, 6);


            modelBuilder.Entity<POrderDetail>()
                .Property(x =>x.TotalSFT).HasPrecision(18, 4);

            modelBuilder.Entity<SOrderDetail>()
               .Property(x => x.TotalSFT).HasPrecision(18, 4);

            modelBuilder.Entity<StockDetail>()
               .Property(x => x.TotalSFT).HasPrecision(18, 4);

            #endregion
        }

     
    }
}
