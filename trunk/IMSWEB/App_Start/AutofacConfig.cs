﻿using Autofac;
using Autofac.Integration.Mvc;
using Owin;
using System.Data.Entity;
using System.Reflection;
using System.Web;
using IMSWEB.Data;
using IMSWEB.Service;
using IMSWEB.Report;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.EntityFramework;
using IMSWEB.Model;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using AutoMapper;
using IMSWEB.Data.Repositories.StockRepository;
using Autofac.Integration.WebApi;
using IMSWEB.APIController;
using System.Web.Http;

namespace IMSWEB
{
    public class AutofacConfig
    {
        public static void ConfigureAutofac(IAppBuilder app)
        {
            var builder = new ContainerBuilder();

            builder.RegisterControllers(Assembly.GetExecutingAssembly());
            //builder.RegisterType<AttendenceSyncController>().AsSelf().InstancePerRequest();
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());

            builder.RegisterType<IMSWEBContext>().As<DbContext>().InstancePerRequest();
            builder.RegisterType<ApplicationUserManager>().AsSelf().InstancePerRequest();
            builder.RegisterType<ApplicationSignInManager>().AsSelf().InstancePerRequest();
            builder.Register(c => new UserStore<ApplicationUser, ApplicationRole, int, ApplicationUserLogin,
                ApplicationUserRole, ApplicationUserClaim>(c.Resolve<DbContext>())).AsImplementedInterfaces().InstancePerRequest();
            builder.Register(c => HttpContext.Current.GetOwinContext().Authentication).As<IAuthenticationManager>();
            builder.Register(c => new IdentityFactoryOptions<ApplicationUserManager>
            {
                DataProtectionProvider = new Microsoft.Owin.Security.DataProtection.DpapiDataProtectionProvider("IMSWEB​")
            });
            builder.RegisterType<UnitOfWork>().As<IUnitOfWork>().InstancePerRequest();
            builder.RegisterType<DbFactory>().As<IDbFactory>().InstancePerRequest();
            builder.RegisterInstance(new AutoMapperConfiguration().Configure()).As<IMapper>().SingleInstance();

            builder.RegisterType<BasicReport>().As<IBasicReport>().InstancePerRequest();
            builder.RegisterType<TransactionalReport>().As<ITransactionalReport>().InstancePerRequest();

            #region Repositories IMS
            builder.RegisterGeneric(typeof(BaseRepository<>)).As(typeof(IBaseRepository<>)).InstancePerRequest();
            builder.RegisterType<PurchaseOrderRepository>().As<IPurchaseOrderRepository>().InstancePerRequest();
            builder.RegisterType<SalesOrderRepository>().As<ISalesOrderRepository>().InstancePerRequest();
            builder.RegisterType<CreditSalesOrderRepository>().As<ICreditSalesOrderRepository>().InstancePerRequest();
            builder.RegisterType<CashCollectionRepository>().As<ICashCollectionRepository>().InstancePerRequest();
            builder.RegisterType<ROrderRepository>().As<IROrderRepository>().InstancePerRequest();
            builder.RegisterType<SRVisitRepository>().As<ISRVisitRepository>().InstancePerRequest();
            builder.RegisterType<StockRepository>().As<IStockRepository>().InstancePerRequest();
            builder.RegisterType<BankTransactionRepository>().As<IBankTransactionRepository>().InstancePerRequest();

            builder.RegisterType<TransferHistoryRepository>().As<ITransferHistoryRepository>().InstancePerRequest();
            builder.RegisterType<AccountingRepository>().As<IAccountingRepository>().InstancePerRequest();
            builder.RegisterType<ProductionSetupRepository>().As<IProductionSetupRepository>().InstancePerRequest();
            builder.RegisterType<ProductionRepository>().As<IProductionRepository>().InstancePerRequest();

            #endregion

            #region Repositories Payroll
            builder.RegisterType<AttendenceRepository>().As<IAttendenceRepository>().InstancePerRequest();
            builder.RegisterType<SalaryProcessRepository>().As<ISalaryProcessRepository>().InstancePerRequest();
            builder.RegisterType<TargetSetupRepository>().As<ITargetSetupRepository>().InstancePerRequest();
            #endregion

            #region Services IMS
            builder.RegisterType<ErrorService>().As<IErrorService>().InstancePerRequest();
            builder.RegisterType<ProductService>().As<IProductService>().InstancePerRequest();
            builder.RegisterType<BankTransactionService>().As<IBankTransactionService>().InstancePerRequest();
            builder.RegisterType<UserService>().As<IUserService>().InstancePerRequest();
            builder.RegisterType<RoleService>().As<IRoleService>().InstancePerRequest();
            builder.RegisterType<CustomerService>().As<ICustomerService>().InstancePerRequest();
            builder.RegisterType<CompanyService>().As<ICompanyService>().InstancePerRequest();
            builder.RegisterType<GodownService>().As<IGodownService>().InstancePerRequest();
            builder.RegisterType<TransferHistoryService>().As<ITransferHistoryService>().InstancePerRequest();
            builder.RegisterType<ColorService>().As<IColorService>().InstancePerRequest();
            builder.RegisterType<CategoryService>().As<ICategoryService>().InstancePerRequest();
            builder.RegisterType<BankService>().As<IBankService>().InstancePerRequest();
            builder.RegisterType<DesignationService>().As<IDesignationService>().InstancePerRequest();
            builder.RegisterType<ExpenseItemService>().As<IExpenseItemService>().InstancePerRequest();
            builder.RegisterType<EmployeeService>().As<IEmployeeService>().InstancePerRequest();
            builder.RegisterType<SupplierService>().As<ISupplierService>().InstancePerRequest();
            builder.RegisterType<PurchaseOrderService>().As<IPurchaseOrderService>().InstancePerRequest();
            builder.RegisterType<PurchaseOrderDetailService>().As<IPurchaseOrderDetailService>().InstancePerRequest();
            builder.RegisterType<POProductDetailService>().As<IPOProductDetailService>().InstancePerRequest();
            builder.RegisterType<StockService>().As<IStockService>().InstancePerRequest();
            builder.RegisterType<StockDetailService>().As<IStockDetailService>().InstancePerRequest();
            builder.RegisterType<SisterConcernService>().As<ISisterConcernService>().InstancePerRequest();
            builder.RegisterType<SystemInformationService>().As<ISystemInformationService>().InstancePerRequest();
            builder.RegisterType<SalesOrderService>().As<ISalesOrderService>().InstancePerRequest();
            builder.RegisterType<SalesOrderDetailService>().As<ISalesOrderDetailService>().InstancePerRequest();
            builder.RegisterType<CreditSalesOrderService>().As<ICreditSalesOrderService>().InstancePerRequest();
            builder.RegisterType<ExpenditureService>().As<IExpenditureService>().InstancePerRequest();
            builder.RegisterType<CashCollectionService>().As<ICashCollectionService>().InstancePerRequest();
            builder.RegisterType<MenuService>().As<IMenuService>().InstancePerRequest();
            builder.RegisterType<SaleOfferService>().As<ISaleOfferService>().InstancePerRequest();
            builder.RegisterType<ROrderService>().As<IROrderService>().InstancePerRequest();
            builder.RegisterType<ROrderDetailService>().As<IROrderDetailService>().InstancePerRequest();
            builder.RegisterType<ROProductDetailService>().As<IROProductDetailService>().InstancePerRequest();

            builder.RegisterType<SRVisitService>().As<ISRVisitService>().InstancePerRequest();
            builder.RegisterType<SRVisitDetailService>().As<ISRVisitDetailService>().InstancePerRequest();
            builder.RegisterType<SRVProductDetailService>().As<ISRVProductDetailService>().InstancePerRequest();
            builder.RegisterType<PriceProtectionService>().As<IPriceProtectionService>().InstancePerRequest();
            builder.RegisterType<SizeService>().As<ISizeService>().InstancePerRequest();
            builder.RegisterType<ProductUnitTypeService>().As<IProductUnitTypeService>().InstancePerRequest();


            builder.RegisterType<ShareInvestmentHeadService>().As<IShareInvestmentHeadService>().InstancePerRequest();
            builder.RegisterType<ShareInvestmentService>().As<IShareInvestmentService>().InstancePerRequest();
            builder.RegisterType<AccountingService>().As<IAccountingService>().InstancePerRequest();
            builder.RegisterType<EmployeeTargetSetupService>().As<IEmployeeTargetSetupService>().InstancePerRequest();
            builder.RegisterType<ManualAttendencService>().As<IManualAttendencService>().InstancePerRequest();
            builder.RegisterType<MonthlyAttendenceService>().As<IMonthlyAttendenceService>().InstancePerRequest();
            builder.RegisterType<EmployeeCommissionService>().As<IEmployeeCommissionService>().InstancePerRequest();
            builder.RegisterType<TerritoryService>().As<ITerritoryService>().InstancePerRequest();
            builder.RegisterType<DepotService>().As<IDepotService>().InstancePerRequest();
            builder.RegisterType<EmployeeWiseCustomerDueService>().As<IEmployeeWiseCustomerDueService>().InstancePerRequest();
            builder.RegisterType<CCBanKService>().As<ICCBanKService>().InstancePerRequest();
            builder.RegisterType<DiaSizeService>().As<IDiaSizeService>().InstancePerRequest();

            builder.RegisterType<ProductionSetupService>().As<IProductionSetupService>().InstancePerRequest();
            builder.RegisterType<ProductionService>().As<IProductionService>().InstancePerRequest();
            builder.RegisterType<CashAccountService>().As<ICashAccountService>().InstancePerRequest();



            //builder.RegisterType<ROrderService>().As<IROrderService>().InstancePerRequest();
            builder.RegisterGeneric(typeof(MiscellaneousService<>)).As(typeof(IMiscellaneousService<>)).InstancePerRequest();

            #endregion

            #region Services Payroll
            builder.RegisterType<AllowanceDeductionService>().As<IAllowanceDeductionService>().InstancePerRequest();
            builder.RegisterType<GradeService>().As<IGradeService>().InstancePerRequest();
            builder.RegisterType<ReligionService>().As<IReligionService>().InstancePerRequest();
            builder.RegisterType<EmpGradeSalaryAssignmentService>().As<IEmpGradeSalaryAssignmentService>().InstancePerRequest();
            builder.RegisterType<GradeSalaryChangeTypeService>().As<IGradeSalaryChangeTypeService>().InstancePerRequest();
            builder.RegisterType<ADParameterBasicService>().As<IADParameterBasicService>().InstancePerRequest();
            builder.RegisterType<ADParamADParameterGradeService>().As<IADParamADParameterGradeService>().InstancePerRequest();
            builder.RegisterType<DepartmentService>().As<IDepartmentService>().InstancePerRequest();
            builder.RegisterType<HolidayCalenderService>().As<IHolidayCalenderService>().InstancePerRequest();
            builder.RegisterType<EmployeeLeaveService>().As<IEmployeeLeaveService>().InstancePerRequest();
            builder.RegisterType<AttendenceService>().As<IAttendenceService>().InstancePerRequest();
            builder.RegisterType<AdvanceSalaryService>().As<IAdvanceSalaryService>().InstancePerRequest();
            builder.RegisterType<SalaryProcessorService>().As<ISalaryProcessorService>().InstancePerRequest();
            builder.RegisterType<SalaryMonthlyService>().As<ISalaryMonthlyService>().InstancePerRequest();
            builder.RegisterType<TargetSetupService>().As<ITargetSetupService>().InstancePerRequest();
            builder.RegisterType<CommisssionSetupService>().As<ICommisssionSetupService>().InstancePerRequest();
            builder.RegisterType<DesWiseCommissionService>().As<IDesWiseCommissionService>().InstancePerRequest();
            #endregion

            #region SMS Service
            builder.RegisterType<SMSStatusService>().As<ISMSStatusService>().InstancePerRequest();
            builder.RegisterType<SMSBillPaymentService>().As<ISMSBillPaymentService>().InstancePerRequest();
            #endregion
            #region Website Service
            builder.RegisterType<WebsiteProductService>().As<IWebsiteProductService>().InstancePerRequest();
            builder.RegisterType<DocumentsInfoService>().As<IDocumentsInfoService>().InstancePerRequest();
            builder.RegisterType<AboutUsService>().As<IAboutUSService>().InstancePerRequest();
            #endregion
            #region service of payment info
            builder.RegisterType<ServiceChargeService>().As<IServiceChargeService>().InstancePerRequest();
            builder.RegisterType<ServiceChargeDetailsService>().As<IServiceChargeDetailsService>().InstancePerRequest();
            builder.RegisterType<SMSBillPaymentBkashDetailService>().As<ISMSBillPaymentBkashDetailService>().InstancePerRequest();
            builder.RegisterType<SMSBillPaymentBkashService>().As<ISMSBillPaymentBkashService>().InstancePerRequest();
            #endregion

            IContainer container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
             //DependencyResolver.SetResolver(new AutofacWebApiDependencyResolver(container));
            GlobalConfiguration.Configuration.DependencyResolver = new AutofacWebApiDependencyResolver(container);

            app.UseAutofacMiddleware(container);
        }
    }
}