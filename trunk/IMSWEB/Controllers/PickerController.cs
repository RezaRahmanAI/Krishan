using AutoMapper;
using IMSWEB.Model;
using IMSWEB.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;

namespace IMSWEB.Controllers
{
    [Authorize]
    [RoutePrefix("picker")]
    public class PickerController : CoreController
    {
        ICategoryService _categoryService;
        IBankService _bankService;
        IColorService _colorService;
        ICompanyService _companyService;
        IGodownService _godownService;

        ICustomerService _customerService;
        ISupplierService _supplierService;
        IEmployeeService _employeeService;
        IProductService _productService;
        IMapper _mapper;
        IExpenseItemService _expenseItemService;
        IUserService _UserService;
        IAllowanceDeductionService _allowanceDeductionService;
        ISystemInformationService _SystemInformationService;
        ISalaryMonthlyService _SalaryMonthlyService;
        IDepartmentService _DepartmentService;
        ISizeService _SizeService;
        IProductUnitTypeService _ProductUnitTypeService;
        IShareInvestmentHeadService _ShareInvestmentHeadService;
        ITerritoryService _TerritoryService;
        IDepotService _DepotService;
        ICCBanKService _CCBanKService;
        IDiaSizeService _DiaSizeService;
        IProductionSetupService _productionSetupService;
        public PickerController(
            IErrorService errorService, ICategoryService categoryService, IBankService bankService,
            IColorService colorService, ICompanyService companyService,
            IGodownService godownService,
            ICustomerService customerService,
            ISupplierService supplierService, IEmployeeService employeeService, IProductService productService,
            IUserService UserService, IProductionSetupService productionSetupService,
            IExpenseItemService expenseItemService, IAllowanceDeductionService allowanceDeductionService,
            ISystemInformationService SystemInformationService, ISalaryMonthlyService SalaryMonthlyService, IDiaSizeService DiaSizeService,
            IDepartmentService DepartmentService, ITerritoryService TerritoryService, IDepotService DepotService, ICCBanKService CCBanKService,
            IMapper mapper, ISizeService SizeService, IProductUnitTypeService ProductUnitTypeService, IShareInvestmentHeadService ShareInvestmentHeadService
            )
            : base(errorService)
        {
            _categoryService = categoryService;
            _bankService = bankService;
            _colorService = colorService;
            _companyService = companyService;
            _godownService = godownService;
            _customerService = customerService;
            _supplierService = supplierService;
            _employeeService = employeeService;
            _productService = productService;
            _mapper = mapper;
            _expenseItemService = expenseItemService;
            _UserService = UserService;
            _allowanceDeductionService = allowanceDeductionService;
            _SystemInformationService = SystemInformationService;
            _SalaryMonthlyService = SalaryMonthlyService;
            _DepartmentService = DepartmentService;
            _SizeService = SizeService;
            _ProductUnitTypeService = ProductUnitTypeService;
            _ShareInvestmentHeadService = ShareInvestmentHeadService;
            _TerritoryService = TerritoryService;
            _DepotService = DepotService;
            _CCBanKService = CCBanKService;
            _DiaSizeService = DiaSizeService;
            _productionSetupService = productionSetupService;
        }

        [ChildActionOnly]
        public PartialViewResult RenderPicker(PickerType pickerType, int id)
        {
            switch (pickerType)
            {
                case PickerType.Category:

                    var categories = _categoryService.GetAllCategory();
                    var vmCategories = _mapper.Map<IEnumerable<Category>, IEnumerable<CreateCategoryViewModel>>(categories);
                    if (id != default(int))
                    {
                        var vmCategory = vmCategories.First(x => int.Parse(x.Id) == id);
                        ViewBag.CategoriesCode = vmCategory.Code;
                        ViewBag.CategoriesName = vmCategory.Name;
                        ViewBag.CategoriesId = vmCategory.Id;
                    }
                    return PartialView("~/Views/Pickers/_CategoryPicker.cshtml", vmCategories);


                case PickerType.Size:

                    var sizes = _SizeService.GetAll();
                    var vmsizes = _mapper.Map<IEnumerable<Size>, IEnumerable<SizeViewModel>>(sizes);
                    if (id != default(int))
                    {
                        var vmCategory = vmsizes.First(x => x.SizeID == id);
                        ViewBag.SizesCode = vmCategory.Code;
                        ViewBag.SizesName = vmCategory.Description;
                        ViewBag.SizesId = vmCategory.SizeID;
                    }
                    return PartialView("~/Views/Pickers/_SizePicker.cshtml", vmsizes);
                case PickerType.DiaSize:

                    var diasizes = _DiaSizeService.GetAll();
                    var vmdiasizes = _mapper.Map<IEnumerable<DiaSize>, IEnumerable<DiaSizeViewModel>>(diasizes);
                    if (id != default(int))
                    {
                        var vmCategory = vmdiasizes.First(x => x.DiaSizeID == id);
                        ViewBag.DiaSizesCode = vmCategory.Code;
                        ViewBag.DiaSizesName = vmCategory.Description;
                        ViewBag.DiaSizesId = vmCategory.DiaSizeID;
                    }
                    return PartialView("~/Views/Pickers/_DiaSizePicker.cshtml", vmdiasizes);
                case PickerType.Bank:

                    var Banks = _bankService.GetAllBank();
                    var vmBanks = _mapper.Map<IEnumerable<Bank>, IEnumerable<CreateBankViewModel>>(Banks);
                    if (id != default(int))
                    {
                        var vmBank = vmBanks.First(x => int.Parse(x.Id) == id);
                        ViewBag.BanksCode = vmBank.Code;
                        ViewBag.BanksName = vmBank.BankName;
                        ViewBag.BanksId = vmBank.Id;
                    }
                    return PartialView("~/Views/Pickers/_BankPicker.cshtml", vmBanks);

                case PickerType.AnotherBank:

                    var AnotherBanks = _bankService.GetAllBank();
                    var vmAnotherBanks = _mapper.Map<IEnumerable<Bank>, IEnumerable<CreateBankViewModel>>(AnotherBanks);
                    if (id != default(int))
                    {
                        var vmAnotherBank = vmAnotherBanks.First(x => int.Parse(x.Id) == id);
                        ViewBag.AnotherBanksCode = vmAnotherBank.Code;
                        ViewBag.AnotherBanksName = vmAnotherBank.BankName;
                        ViewBag.AnotherBanksId = vmAnotherBank.Id;
                    }
                    return PartialView("~/Views/Pickers/_AnotherBankPicker.cshtml", vmAnotherBanks);

                case PickerType.Company:

                    var companies = _companyService.GetAllCompany();
                    var vmCompanies = _mapper.Map<IEnumerable<Company>, IEnumerable<CreateCompanyViewModel>>(companies);
                    if (id != default(int))
                    {
                        var vmCompany = vmCompanies.First(x => int.Parse(x.Id) == id);
                        ViewBag.CompaniesCode = vmCompany.Code;
                        ViewBag.CompaniesName = vmCompany.Name;
                        ViewBag.CompaniesId = vmCompany.Id;
                    }
                    return PartialView("~/Views/Pickers/_CompanyPicker.cshtml", vmCompanies);

                case PickerType.Color:

                    var colors = _colorService.GetAllColor();
                    var vmColors = _mapper.Map<IEnumerable<Color>, IEnumerable<CreateColorViewModel>>(colors);
                    if (id != default(int))
                    {
                        var vmColor = vmColors.First(x => int.Parse(x.Id) == id);
                        ViewBag.ColorsCode = vmColor.Code;
                        ViewBag.ColorsName = vmColor.Name;
                        ViewBag.ColorsId = vmColor.Id;
                    }
                    return PartialView("~/Views/Pickers/_ColorPicker.cshtml", vmColors);


                case PickerType.Godown:

                    var godowns = _godownService.GetAllGodown();
                    var vmGodowns = _mapper.Map<IEnumerable<Godown>, IEnumerable<CreateGodownViewModel>>(godowns);
                    if (id != default(int))
                    {
                        var vmGodown = vmGodowns.First(x => int.Parse(x.Id) == id);
                        ViewBag.GodownsCode = vmGodown.Code;
                        ViewBag.GodownsName = vmGodown.Name;
                        ViewBag.GodownsId = vmGodown.Id;
                    }
                    return PartialView("~/Views/Pickers/_GodownPicker.cshtml", vmGodowns);



                case PickerType.Customer:

                    int userId = System.Web.HttpContext.Current.User.Identity.GetUserId<int>();
                    int EmpID = 0;
                    string userAgent = Request.UserAgent;

                    if (User.IsInRole(ConstantData.ROLE_MOBILE_USER))
                    {

                        //EmpID = ConstantData.GetEmployeeIDByUSerID(userId);
                        var user = _UserService.GetUserById(userId);
                        EmpID = user.EmployeeID;

                        var customers = _customerService.GetAllCustomerByEmp(EmpID);
                        var vmCustomers = _mapper.Map<IEnumerable<Customer>, IEnumerable<CreateCustomerViewModel>>(customers);
                        if (id != default(int))
                        {
                            var vmCustomer = vmCustomers.First(x => int.Parse(x.Id) == id);
                            ViewBag.CustomersCode = vmCustomer.Code;
                            ViewBag.CustomersName = vmCustomer.Name;
                            ViewBag.CustomersId = vmCustomer.Id;
                            ViewBag.CustomersCurrentDue = !string.IsNullOrEmpty(vmCustomer.TotalDue) ?
                                Math.Round(decimal.Parse(vmCustomer.TotalDue), 2).ToString() : string.Empty;
                        }
                        if (!userAgent.ToLower().Contains("windows"))
                            return PartialView("~/Views/Pickers/_CustomerPicker.mobile.cshtml", vmCustomers);
                        return PartialView("~/Views/Pickers/_CustomerPicker.cshtml", vmCustomers);

                    }
                    else
                    {
                        var customers = _customerService.GetAllCustomer();
                        var vmCustomers = _mapper.Map<IEnumerable<Customer>, IEnumerable<CreateCustomerViewModel>>(customers);
                        if (id != default(int))
                        {
                            var vmCustomer = vmCustomers.First(x => int.Parse(x.Id) == id);
                            ViewBag.CustomersCode = vmCustomer.Code;
                            ViewBag.CustomersName = vmCustomer.Name;
                            ViewBag.CustomersId = vmCustomer.Id;
                            ViewBag.CustomersCurrentDue = !string.IsNullOrEmpty(vmCustomer.TotalDue) ?
                                Math.Round(decimal.Parse(vmCustomer.TotalDue), 2).ToString() : string.Empty;
                        }
                        if (!userAgent.ToLower().Contains("windows"))
                            return PartialView("~/Views/Pickers/_CustomerPicker.mobile.cshtml", vmCustomers);
                        return PartialView("~/Views/Pickers/_CustomerPicker.cshtml", vmCustomers);

                    }
                case PickerType.ExceptCreditCustomer:

                    int CuserId = System.Web.HttpContext.Current.User.Identity.GetUserId<int>();
                    int CEmpID = 0;
                    string CuserAgent = Request.UserAgent;

                    if (User.IsInRole(ConstantData.ROLE_MOBILE_USER))
                    {

                        //CEmpID = ConstantData.GetEmployeeIDByUSerID(CuserId);
                        var user = _UserService.GetUserById(CuserId);
                        CEmpID = user.EmployeeID;
                        // new mthod add by Mostafizur 


                        var Ccustomers = _customerService.GetAllCustomerByEmpNew(CEmpID);//.Where(i => i.CustomerType != EnumCustomerType.Credit);
                        var vmCustomers = _mapper.Map<IEnumerable<Customer>, IEnumerable<CreateCustomerViewModel>>(Ccustomers);
                        if (id != default(int))
                        {
                            var vmCustomer = vmCustomers.First(x => int.Parse(x.Id) == id);
                            ViewBag.CustomersCode = vmCustomer.Code;
                            ViewBag.CustomersName = vmCustomer.Name;
                            ViewBag.CustomersId = vmCustomer.Id;
                            ViewBag.CustomersCurrentDue = !string.IsNullOrEmpty(vmCustomer.TotalDue) ?
                                Math.Round(decimal.Parse(vmCustomer.TotalDue), 2).ToString() : string.Empty;
                        }
                        if (!CuserAgent.ToLower().Contains("windows"))
                            return PartialView("~/Views/Pickers/_CustomerPicker.mobile.cshtml", vmCustomers);
                        return PartialView("~/Views/Pickers/_CustomerPicker.cshtml", vmCustomers);

                    }
                    else
                    {
                        var customers = _customerService.GetAllCustomerNew(User.Identity.GetConcernId());//.Where(i => i.CustomerType != EnumCustomerType.Credit);
                        //var vmCustomers = _mapper.Map<IEnumerable<Customer>, IEnumerable<CreateCustomerViewModel>>(customers);
                        if (id != default(int))
                        {
                            var vmCustomer = customers.First(x => x.Id == id);
                            ViewBag.CustomersCode = vmCustomer.Code;
                            ViewBag.CustomersName = vmCustomer.Name;
                            ViewBag.CustomersId = vmCustomer.Id;
                            ViewBag.CustomersCurrentDue = Math.Round(vmCustomer.TotalDue, 2).ToString();
                        }
                        if (!CuserAgent.ToLower().Contains("windows"))
                            return PartialView("~/Views/Pickers/_CustomerPicker.mobile.cshtml", customers);
                        return PartialView("~/Views/Pickers/_CustomerPicker.cshtml", customers);

                    }
                case PickerType.CreditCustomer:

                    int CreditCuserId = System.Web.HttpContext.Current.User.Identity.GetUserId<int>();
                    int CreditCEmpID = 0;
                    string CreditCuserAgent = Request.UserAgent;

                    if (User.IsInRole(ConstantData.ROLE_MOBILE_USER))
                    {

                        //CreditCEmpID = ConstantData.GetEmployeeIDByUSerID(CreditCuserId);
                        var user = _UserService.GetUserById(CreditCuserId);
                        CreditCEmpID = user.EmployeeID;
                        var Ccustomers = _customerService.GetAllCustomerByEmp(CreditCEmpID);//.Where(i => i.CustomerType == EnumCustomerType.Credit);
                        var vmCustomers = _mapper.Map<IEnumerable<Customer>, IEnumerable<CreateCustomerViewModel>>(Ccustomers);
                        if (id != default(int))
                        {
                            var vmCustomer = vmCustomers.First(x => int.Parse(x.Id) == id);
                            ViewBag.CustomersCode = vmCustomer.Code;
                            ViewBag.CustomersName = vmCustomer.Name;
                            ViewBag.CustomersId = vmCustomer.Id;
                            ViewBag.CustomersCurrentDue = !string.IsNullOrEmpty(vmCustomer.TotalDue) ?
                                Math.Round(decimal.Parse(vmCustomer.TotalDue), 2).ToString() : string.Empty;
                        }
                        if (!CreditCuserAgent.ToLower().Contains("windows"))
                            return PartialView("~/Views/Pickers/_CustomerPicker.mobile.cshtml", vmCustomers);
                        return PartialView("~/Views/Pickers/_CustomerPicker.cshtml", vmCustomers);

                    }
                    else
                    {
                        var customers = _customerService.GetAllCustomer();//.Where(i => i.CustomerType == EnumCustomerType.Credit);
                        var vmCustomers = _mapper.Map<IEnumerable<Customer>, IEnumerable<CreateCustomerViewModel>>(customers);
                        if (id != default(int))
                        {
                            var vmCustomer = vmCustomers.First(x => int.Parse(x.Id) == id);
                            ViewBag.CustomersCode = vmCustomer.Code;
                            ViewBag.CustomersName = vmCustomer.Name;
                            ViewBag.CustomersId = vmCustomer.Id;
                            ViewBag.CustomersCurrentDue = !string.IsNullOrEmpty(vmCustomer.TotalDue) ?
                                Math.Round(decimal.Parse(vmCustomer.TotalDue), 2).ToString() : string.Empty;
                        }
                        if (!CreditCuserAgent.ToLower().Contains("windows"))
                            return PartialView("~/Views/Pickers/_CustomerPicker.mobile.cshtml", vmCustomers);
                        return PartialView("~/Views/Pickers/_CustomerPicker.cshtml", vmCustomers);

                    }

                case PickerType.Supplier:

                    var suppliers = _supplierService.GetAllSupplier();
                    var vmSuppliers = _mapper.Map<IEnumerable<Supplier>, IEnumerable<CreateSupplierViewModel>>(suppliers);
                    if (id != default(int))
                    {
                        var vmSupplier = vmSuppliers.First(x => int.Parse(x.Id) == id);
                        ViewBag.SuppliersCode = vmSupplier.Code;
                        ViewBag.SuppliersName = vmSupplier.Name;
                        ViewBag.SuppliersId = vmSupplier.Id;
                        ViewBag.SuppliersCurrentDue = !string.IsNullOrEmpty(vmSupplier.TotalDue) ?
                            Math.Round(decimal.Parse(vmSupplier.TotalDue), 2).ToString() : string.Empty;
                    }
                    return PartialView("~/Views/Pickers/_SupplierPicker.cshtml", vmSuppliers);

                case PickerType.Employee:

                    var employees = _employeeService.GetAllEmployee();
                    var vmEmployees = _mapper.Map<IEnumerable<Employee>, IEnumerable<CreateEmployeeViewModel>>(employees);
                    if (id != default(int))
                    {
                        var vmEmployee = vmEmployees.First(x => int.Parse(x.Id) == id);
                        ViewBag.EmployeesCode = vmEmployee.Code;
                        ViewBag.EmployeesName = vmEmployee.Name;
                        ViewBag.EmployeesId = vmEmployee.Id;
                    }
                    return PartialView("~/Views/Pickers/_EmployeePicker.cshtml", vmEmployees);


                case PickerType.PickEmployee:

                    var pemployees = _employeeService.GetAllEmployee();
                    var vmpEmployees = _mapper.Map<IEnumerable<Employee>, IEnumerable<CreateEmployeeViewModel>>(pemployees);
                    if (id != default(int))
                    {
                        var vmEmployee = vmpEmployees.First(x => int.Parse(x.Id) == id);
                        ViewBag.EmployeesCode = vmEmployee.Code;
                        ViewBag.EmployeesName = vmEmployee.Name;
                        ViewBag.EmployeesId = vmEmployee.Id;
                    }
                    return PartialView("~/Views/Pickers/_EmployeePickPicker.cshtml", vmpEmployees);

                case PickerType.Product:

                    var customProducts = _productService.GetAllProductIQueryable().Where(i => i.ProductStockType == (int)EnumProductStockType.Raw_Materials);
                    if (id != default(int))
                    {
                        var vmProduct = customProducts.First(x => x.ProductID == id);
                        ViewBag.ProductsCode = vmProduct.ProductCode;
                        ViewBag.ProductsName = vmProduct.ProductName;
                        ViewBag.ProductsId = vmProduct.ProductID;
                        ViewBag.ProductsPreviousStock = vmProduct.Quantity;
                        ViewBag.ProductsType = (int)vmProduct.ProductType;
                    }
                    return PartialView("~/Views/Pickers/_ProductPicker.cshtml", customProducts);

                case PickerType.CombineProduct:

                    var customCombineProducts = _productService.GetAllProductIQueryable();
                    if (id != default(int))
                    {
                        var vmProduct = customCombineProducts.First(x => x.ProductID == id);
                        ViewBag.ProductsCode = vmProduct.ProductCode;
                        ViewBag.ProductsName = vmProduct.ProductName;
                        ViewBag.ProductsId = vmProduct.ProductID;
                        ViewBag.ProductsPreviousStock = vmProduct.Quantity;
                        ViewBag.ProductsType = (int)vmProduct.ProductType;
                    }
                    return PartialView("~/Views/Pickers/_CombineProductPicker.cshtml", customCombineProducts);

                case PickerType.ProductDetail:

                    ViewBag.Type = "Details";
                    var customProductDetails = _productService.GetAllProductFromDetail()/*.Where(i => i.Rest.Rest.Rest.Item6 == (int)EnumProductStockType.Finished_Goods)*/;

                    var vmProductDetails = _mapper.Map<IEnumerable<Tuple<int, string, string,
                    decimal, string, string, string, Tuple<decimal?, string, decimal, int, int, string, string,
                    Tuple<string, string, string, string, string, string, int, Tuple<int, decimal, decimal, decimal, decimal, int>>>>>, IEnumerable<GetProductViewModel>>(customProductDetails).ToList();


                    var vmProductGroupBY = (from vm in vmProductDetails
                                            join pu in _ProductUnitTypeService.GetAll() on vm.ProUnitTypeID equals pu.ProUnitTypeID
                                            join s in _SizeService.GetAll() on vm.SizeID equals s.SizeID
                                            group vm by new
                                            {
                                                vm.IMENo,
                                                vm.ProductId,
                                                vm.CategoryID,
                                                vm.ProductName,
                                                vm.ProductCode,

                                                vm.ColorId,
                                                vm.CategoryName,
                                                vm.ColorName,
                                                vm.ModelName,
                                                vm.GodownName,
                                                vm.CompanyName,
                                                vm.ProUnitTypeID,
                                                ChildUnit = pu.UnitName,
                                                ParentUnit = pu.Description,
                                                ConvertValue = vm.BundleQty == 0 ? pu.ConvertValue : vm.BundleQty,
                                                s.SizeID,
                                                SizeName = s.Description,
                                                vm.PurchaseCSft,
                                                vm.SalesCSft,
                                                vm.TotalSFT

                                            } into g
                                            select new GetProductViewModel
                                            {
                                                IMENo = g.Key.IMENo,
                                                ProductId = g.Key.ProductId,
                                                ProductCode = g.Key.ProductCode,
                                                ProductName = g.Key.ProductName,
                                                CategoryID = g.Key.CategoryID,

                                                CategoryName = g.Key.CategoryName,
                                                ColorName = g.Key.ColorName,
                                                ColorId = g.Key.ColorId,
                                                ModelName = g.Key.ModelName,
                                                StockDetailsId = g.Select(o => o.StockDetailsId).FirstOrDefault(),

                                                MRPRate = ((g.Select(o => o.MRPRate).FirstOrDefault()) / g.Key.ConvertValue),
                                                ParentMRP = g.Select(o => o.MRPRate).FirstOrDefault(),
                                                MRPRate12 = g.Select(o => o.MRPRate12).FirstOrDefault(),
                                                CashSalesRate = g.Select(o => o.CashSalesRate).FirstOrDefault(),
                                                PWDiscount = g.Select(o => o.PWDiscount).FirstOrDefault(),
                                                PicturePath = g.Select(o => o.PicturePath).FirstOrDefault(),

                                                PreStock = g.Select(o => o.PreStock).FirstOrDefault(),
                                                OfferDescription = g.Select(o => o.OfferDescription).FirstOrDefault(),
                                                ProductType = g.Select(o => o.ProductType).FirstOrDefault(),
                                                CompressorWarrentyMonth = g.Select(o => o.CompressorWarrentyMonth).FirstOrDefault(),
                                                PanelWarrentyMonth = g.Select(o => o.PanelWarrentyMonth).FirstOrDefault(),

                                                MotorWarrentyMonth = g.Select(o => o.MotorWarrentyMonth).FirstOrDefault(),
                                                SparePartsWarrentyMonth = g.Select(o => o.SparePartsWarrentyMonth).FirstOrDefault(),
                                                ServiceWarrentyMonth = g.Select(o => o.ServiceWarrentyMonth).FirstOrDefault(),
                                                IsSelect = g.Select(o => o.IsSelect).FirstOrDefault(),
                                                Status = g.Select(o => o.Status).FirstOrDefault(),

                                                Quantity = g.Select(o => o.Quantity).FirstOrDefault(), //e.g.; gm
                                                GodownName = g.Key.GodownName,
                                                SizeID = g.Key.SizeID,
                                                SizeName = g.Key.SizeName,
                                                CompanyName = g.Key.CompanyName,

                                                ChildUnit = g.Key.ChildUnit,
                                                ConvertValue = g.Key.ConvertValue,
                                                ParentUnit = g.Key.ParentUnit,
                                                PurchaseCSft = g.Key.PurchaseCSft,
                                                SalesCSft = g.Key.SalesCSft,
                                                ParentQty = (int)Math.Truncate(g.Select(o => o.PreStock).FirstOrDefault() / g.Key.ConvertValue), //e.g. KG
                                                ChildQty = (int)(g.Select(o => o.PreStock).FirstOrDefault() % g.Key.ConvertValue), //e.g. gm

                                                TotalSFT = g.Key.TotalSFT
                                            }).OrderBy(p => p.ProductId);



                    if (id != default(int))
                    {
                        var vmProduct = vmProductDetails.First(x => x.ProductId == id);
                        ViewBag.PDetailsCode = vmProduct.ProductCode;
                        ViewBag.PDetailsName = vmProduct.ProductName;
                        ViewBag.PDetailsId = vmProduct.ProductId;
                        ViewBag.PDetailsStockDetailId = vmProduct.StockDetailsId;
                        ViewBag.PDetailsColorId = vmProduct.ColorId;
                        ViewBag.OfferDescription = vmProduct.OfferDescription;
                        ViewBag.ProductType = _productService.GetProductById(vmProduct.ProductId).ProductType;

                    }
                    //return PartialView("~/Views/Pickers/_ProductDetailPicker.cshtml", vmProductDetails);
                    return PartialView("~/Views/Pickers/_ProductDetailPicker.cshtml", vmProductGroupBY);
                case PickerType.DamageProductDetail:

                    ViewBag.Type = "Details";
                    var DamagecustomProductDetails = _productService.GetAllDamageProductFromDetail();
                    var vmDamageProductDetails = _mapper.Map<IEnumerable<Tuple<int, string, string,
                    decimal, string, string, string, Tuple<decimal?, string, decimal, int, int, string, string, Tuple<string, string, string, string, string>>>>, IEnumerable<GetProductViewModel>>(DamagecustomProductDetails).ToList();

                    if (id != default(int))
                    {
                        var vmProduct = vmDamageProductDetails.First(x => x.ProductId == id);
                        ViewBag.PDetailsCode = vmProduct.ProductCode;
                        ViewBag.PDetailsName = vmProduct.ProductName;
                        ViewBag.PDetailsId = vmProduct.ProductId;
                        ViewBag.PDetailsStockDetailId = vmProduct.StockDetailsId;
                        ViewBag.PDetailsColorId = vmProduct.ColorId;
                        ViewBag.OfferDescription = vmProduct.OfferDescription;
                        ViewBag.ProductType = _productService.GetProductById(vmProduct.ProductId).ProductType;

                    }
                    return PartialView("~/Views/Pickers/_ProductDetailPicker.cshtml", vmDamageProductDetails);
                case PickerType.CreditProductDetail:

                    ViewBag.Type = "Details";
                    var cproductdetails = _productService.GetAllProductFromDetailForCredit();
                    var vmcreditpdeatils = _mapper.Map<IEnumerable<Tuple<int, string, string,
                    decimal, string, string, string, Tuple<decimal?, string, decimal, int, int, string, string, Tuple<decimal, string, string, string, string, string, decimal, Tuple<string>>>>>, IEnumerable<GetProductViewModel>>(cproductdetails).ToList();





                    var vmCreditProductGroupBY = (from vm in vmcreditpdeatils
                                                  group vm by new { vm.IMENo, vm.ProductId, vm.CategoryID, vm.ProductName, vm.ProductCode, vm.ColorId, vm.CategoryName, vm.ColorName, vm.ModelName, vm.GodownName }
                                                      into g
                                                  select new GetProductViewModel
                                                  {
                                                      IMENo = g.Key.IMENo,
                                                      ProductId = g.Key.ProductId,
                                                      ProductCode = g.Key.ProductCode,
                                                      ProductName = g.Key.ProductName,
                                                      CategoryID = g.Key.CategoryID,

                                                      CategoryName = g.Key.CategoryName,
                                                      ColorName = g.Key.ColorName,
                                                      ColorId = g.Key.ColorId,
                                                      ModelName = g.Key.ModelName,
                                                      StockDetailsId = g.Select(o => o.StockDetailsId).FirstOrDefault(),

                                                      MRPRate = g.Select(o => o.MRPRate).FirstOrDefault(),
                                                      MRPRate12 = g.Select(o => o.MRPRate12).FirstOrDefault(),
                                                      CashSalesRate = g.Select(o => o.CashSalesRate).FirstOrDefault(),
                                                      PWDiscount = g.Select(o => o.PWDiscount).FirstOrDefault(),
                                                      PicturePath = g.Select(o => o.PicturePath).FirstOrDefault(),

                                                      PreStock = g.Select(o => o.PreStock).FirstOrDefault(),
                                                      OfferDescription = g.Select(o => o.OfferDescription).FirstOrDefault(),
                                                      ProductType = g.Select(o => o.ProductType).FirstOrDefault(),
                                                      CompressorWarrentyMonth = g.Select(o => o.CompressorWarrentyMonth).FirstOrDefault(),
                                                      PanelWarrentyMonth = g.Select(o => o.PanelWarrentyMonth).FirstOrDefault(),

                                                      MotorWarrentyMonth = g.Select(o => o.MotorWarrentyMonth).FirstOrDefault(),
                                                      SparePartsWarrentyMonth = g.Select(o => o.SparePartsWarrentyMonth).FirstOrDefault(),
                                                      ServiceWarrentyMonth = g.Select(o => o.ServiceWarrentyMonth).FirstOrDefault(),
                                                      IsSelect = g.Select(o => o.IsSelect).FirstOrDefault(),
                                                      Status = g.Select(o => o.Status).FirstOrDefault(),

                                                      Quantity = g.Select(o => o.Quantity).FirstOrDefault(),
                                                      GodownName = g.Key.GodownName

                                                  });



                    if (id != default(int))
                    {
                        var vmProduct = vmcreditpdeatils.First(x => x.ProductId == id);
                        ViewBag.PDetailsCode = vmProduct.ProductCode;
                        ViewBag.PDetailsName = vmProduct.ProductName;
                        ViewBag.PDetailsId = vmProduct.ProductId;
                        ViewBag.PDetailsStockDetailId = vmProduct.StockDetailsId;
                        ViewBag.PDetailsColorId = vmProduct.ColorId;
                        ViewBag.OfferDescription = vmProduct.OfferDescription;
                        ViewBag.ProductType = _productService.GetProductById(vmProduct.ProductId).ProductType;

                    }
                    return PartialView("~/Views/Pickers/_CreditProductDetailPicker.cshtml", vmCreditProductGroupBY);
                case PickerType.SRProductDetail:

                    ViewBag.Type = "Details";
                    int SRUID = User.Identity.GetUserId<int>();

                    int SRID = 0;

                    if (User.IsInRole(ConstantData.ROLE_MOBILE_USER))
                    {
                        //SRID = ConstantData.GetEmployeeIDByUSerID(SRUID);
                        var user = _UserService.GetUserById(SRUID);
                        SRID = user.EmployeeID;
                    }
                    var srProducts = _productService.SRWiseGetAllProductFromDetail(SRID);
                    var vmsrProducts = _mapper.Map<IEnumerable<Tuple<int, string, string,
                    decimal, string, string, string, Tuple<decimal?, string, decimal, int, int, string, string>>>, IEnumerable<GetProductViewModel>>(srProducts).ToList();

                    if (id != default(int))
                    {
                        var vmProduct = vmsrProducts.First(x => x.ProductId == id);
                        ViewBag.PDetailsCode = vmProduct.ProductCode;
                        ViewBag.PDetailsName = vmProduct.ProductName;
                        ViewBag.PDetailsId = vmProduct.ProductId;
                        ViewBag.PDetailsStockDetailId = vmProduct.StockDetailsId;
                        ViewBag.PDetailsColorId = vmProduct.ColorId;
                        ViewBag.OfferDescription = vmProduct.OfferDescription;
                        ViewBag.ProductType = _productService.GetProductById(vmProduct.ProductId).ProductType;

                    }
                    return PartialView("~/Views/Pickers/_ProductDetailPicker.cshtml", vmsrProducts);
                case PickerType.ProductDetailMobile:

                    ViewBag.Type = "Details";
                    //var customProductDetailsMobile = _productService.GetAllProductFromDetail();
                    //var vmProductDetailsMobile = _mapper.Map<IEnumerable<Tuple<int, string, string,
                    //decimal, string, string, string, Tuple<decimal?, string, decimal, int, int, string, string>>>, IEnumerable<GetProductViewModel>>(customProductDetailsMobile);
                    int MSRUID = User.Identity.GetUserId<int>();

                    int MSRID = 0;

                    if (User.IsInRole(ConstantData.ROLE_MOBILE_USER))
                    {
                        //MSRID = ConstantData.GetEmployeeIDByUSerID(MSRUID);
                        var user = _UserService.GetUserById(MSRUID);
                        MSRID = user.EmployeeID;
                    }
                    var MsrProducts = _productService.SRWiseGetAllProductFromDetail(MSRID);
                    var MvmsrProducts = _mapper.Map<IEnumerable<Tuple<int, string, string,
                    decimal, string, string, string, Tuple<decimal?, string, decimal, int, int, string, string>>>, IEnumerable<GetProductViewModel>>(MsrProducts).ToList();

                    if (id != default(int))
                    {
                        var vmProduct = MvmsrProducts.First(x => x.ProductId == id);
                        ViewBag.PDetailsCode = vmProduct.ProductCode;
                        ViewBag.PDetailsName = vmProduct.ProductName;
                        ViewBag.PDetailsId = vmProduct.ProductId;
                        ViewBag.PDetailsStockDetailId = vmProduct.StockDetailsId;
                        ViewBag.PDetailsColorId = vmProduct.ColorId;
                        ViewBag.OfferDescription = vmProduct.OfferDescription;

                    }
                    return PartialView("~/Views/Pickers/_ProductDetailPicker.mobile.cshtml", MvmsrProducts);
                case PickerType.SalesProductDetail:

                    ViewBag.Type = "Details";
                    var salesProductDetailsMobile = _productService.GetAllProductFromDetail();
                    var vmsalesProductDetails = _mapper.Map<IEnumerable<Tuple<int, string, string,
                    decimal, string, string, string, Tuple<decimal?, string, decimal, int, int, string, string,
                    Tuple<string, string, string, string, string, string, int, Tuple<int, decimal, decimal, decimal, decimal, int>>>>>, IEnumerable<GetProductViewModel>>(salesProductDetailsMobile);

                    if (id != default(int))
                    {
                        var vmProduct = vmsalesProductDetails.First(x => x.ProductId == id);
                        ViewBag.PDetailsCode = vmProduct.ProductCode;
                        ViewBag.PDetailsName = vmProduct.ProductName;
                        ViewBag.PDetailsId = vmProduct.ProductId;
                        ViewBag.PDetailsStockDetailId = vmProduct.StockDetailsId;
                        ViewBag.PDetailsColorId = vmProduct.ColorId;
                        ViewBag.OfferDescription = vmProduct.OfferDescription;

                    }
                    return PartialView("~/Views/Pickers/_SalesProductDetailPicker.cshtml", vmsalesProductDetails);

                case PickerType.ExpenseItemHead:

                    ViewBag.Type = "ExpenseItem";
                    var expenseItems = _expenseItemService.GetAllExpenseItem().Where(i => i.Status == EnumCompanyTransaction.Expense).ToList();

                    if (id != default(int))
                    {
                        var expensitem = expenseItems.FirstOrDefault(i => i.ExpenseItemID == id);
                        ViewBag.ExpenseItemsId = expensitem.ExpenseItemID;
                        ViewBag.ExpenseItemsCode = expensitem.Code;
                        ViewBag.ExpensItemsName = expensitem.Description;

                    }
                    return PartialView("~/Views/Pickers/_ExpenseItemsPicker.cshtml", expenseItems);
                case PickerType.IncomeItemHead:

                    ViewBag.Type = "IncomeItem";
                    var incomeItems = _expenseItemService.GetAllExpenseItem().Where(i => i.Status == EnumCompanyTransaction.Income).ToList();

                    if (id != default(int))
                    {
                        var Incomeitem = incomeItems.FirstOrDefault(i => i.ExpenseItemID == id);
                        ViewBag.ExpenseItemsId = Incomeitem.ExpenseItemID;
                        ViewBag.ExpenseItemsCode = Incomeitem.Code;
                        ViewBag.ExpensItemsName = Incomeitem.Description;

                    }
                    return PartialView("~/Views/Pickers/_ExpenseItemsPicker.cshtml", incomeItems);
                #region StockProduct
                case PickerType.StockProduct:

                    var stocks = _productService.GetAllProduct().Where(i => i.Rest.Item1 > 0);
                    var vmstocks = _mapper.Map<IEnumerable<Tuple<int, string, string,
                    decimal, string, string, string, Tuple<decimal?, int>>>, IEnumerable<GetProductViewModel>>(stocks);

                    if (id != default(int))
                    {
                        var vmProduct = vmstocks.First(x => x.ProductId == id);
                        ViewBag.ProductsCode = vmProduct.ProductCode;
                        ViewBag.ProductsName = vmProduct.ProductName;
                        ViewBag.ProductsId = vmProduct.ProductId;
                        ViewBag.ProductsPreviousStock = vmProduct.PreStock;
                        ViewBag.ProductsType = (int)vmProduct.ProductType;
                    }
                    return PartialView("~/Views/Pickers/_ProductPicker.cshtml", vmstocks);
                #endregion
                #region Allowance
                case PickerType.Allowance:

                    var allowances = _allowanceDeductionService.GetAll().Where(i => i.AllowORDeduct == (int)EnumAllowOrDeduct.Allowance);
                    var vmallowances = _mapper.Map<IEnumerable<AllowanceDeduction>, IEnumerable<AllowanceDeductionViewModel>>(allowances);

                    if (id != default(int))
                    {
                        var vmallowance = vmallowances.First(x => x.AllowDeductID == id);
                        ViewBag.AllowancesCode = vmallowance.Code;
                        ViewBag.AllowancesName = vmallowance.Name;
                        ViewBag.AllowancesId = vmallowance.AllowDeductID;
                    }
                    return PartialView("~/Views/Pickers/_AllowancePicker.cshtml", vmallowances);
                #endregion
                #region Deduction
                case PickerType.Deduction:

                    var Deductions = _allowanceDeductionService.GetAll().Where(i => i.AllowORDeduct == (int)EnumAllowOrDeduct.Deduction);
                    var vmDeductions = _mapper.Map<IEnumerable<AllowanceDeduction>, IEnumerable<AllowanceDeductionViewModel>>(Deductions);

                    if (id != default(int))
                    {
                        var vmallowance = vmDeductions.First(x => x.AllowDeductID == id);
                        ViewBag.AllowancesCode = vmallowance.Code;
                        ViewBag.AllowancesName = vmallowance.Name;
                        ViewBag.AllowancesId = vmallowance.AllowDeductID;
                    }
                    return PartialView("~/Views/Pickers/_AllowancePicker.cshtml", vmDeductions);
                #endregion
                #region Multiple Select Employee
                case PickerType.MSEmployee:

                    var sysinfo = _SystemInformationService.GetSystemInformationByConcernId(User.Identity.GetConcernId());
                    var NextpayDate = ConstantData.GetFirstAndLastDateOfMonth(sysinfo.NextPayProcessDate);
                    var SalaryMonths = _SalaryMonthlyService.GetAllIQueryable().Where(i => i.SalaryMonth >= NextpayDate.Item1 && i.SalaryMonth <= NextpayDate.Item2);

                    var _Employees = _employeeService.GetAllEmployeeDetails().Where(i => (!SalaryMonths.Select(j => j.EmployeeID).Contains(i.Item1)) && i.Item6 <= NextpayDate.Item2);
                    var Employees = _mapper.Map<IEnumerable<Tuple<int, string, string, string, string, DateTime, string, Tuple<string, string>>>, IEnumerable<GetEmployeeViewModel>>(_Employees);
                    return PartialView("~/Views/Pickers/_MSEmployeePicker.cshtml", Employees);
                #endregion
                #region Department
                case PickerType.Department:
                    var Departments = _DepartmentService.GetAllDepartment();
                    if (id != default(int))
                    {
                        var department = Departments.FirstOrDefault(i => i.DepartmentId == id);
                        ViewBag.DepartmentsId = department.DepartmentId;
                        ViewBag.DepartmentsCode = department.CODE;
                        ViewBag.DepartmentsName = department.DESCRIPTION;
                    }
                    return PartialView("~/Views/Pickers/_DepartmentPicker.cshtml", Departments);
                #endregion

                #region ProductPickerFiltered
                case PickerType.ProductPickerFiltered:
                    if (id != default(int))
                    {
                        var vmProduct = _productService.GetProductDetailsBySDetailID(id).FirstOrDefault();
                        ViewBag.ProductsCode = vmProduct.ProductCode;
                        ViewBag.ProductsName = vmProduct.ProductName;
                        ViewBag.ProductsId = vmProduct.ProductID;
                        ViewBag.PDetailsStockDetailId = vmProduct.SDetailID;
                        ViewBag.PDetailsColorId = vmProduct.ColorID;
                        ViewBag.ProductsType = vmProduct.ProductType;
                    }
                    return PartialView("~/Views/Pickers/_ProductPickerFiltered.cshtml");
                #endregion


                case PickerType.MSCustomer:

                    ViewBag.IsCreditSales = false;
                    userId = User.Identity.GetUserId<int>();
                    if (User.IsInRole(ConstantData.ROLE_MOBILE_USER))
                    {

                        //EmpID = ConstantData.GetEmployeeIDByUSerID(userId);
                        var user = _UserService.GetUserById(userId);
                        EmpID = user.EmployeeID;

                        var customers = _customerService.GetAllCustomerByEmp(EmpID);
                        var vmCustomers = _mapper.Map<IEnumerable<Customer>, IEnumerable<CreateCustomerViewModel>>(customers);
                        if (id != default(int))
                        {
                            var vmCustomer = vmCustomers.First(x => int.Parse(x.Id) == id);
                            ViewBag.CustomersCode = vmCustomer.Code;
                            ViewBag.CustomersName = vmCustomer.Name;
                            ViewBag.CustomersId = vmCustomer.Id;
                            ViewBag.CustomersCurrentDue = !string.IsNullOrEmpty(vmCustomer.TotalDue) ?
                                Math.Round(decimal.Parse(vmCustomer.TotalDue), 2).ToString() : string.Empty;
                        }
                        return PartialView("~/Views/Pickers/_MSCustomerPicker.cshtml", vmCustomers);
                    }
                    else
                    {
                        var customers = _customerService.GetAllCustomer();
                        var vmCustomers = _mapper.Map<IEnumerable<Customer>, IEnumerable<CreateCustomerViewModel>>(customers);
                        if (id != default(int))
                        {
                            var vmCustomer = vmCustomers.First(x => int.Parse(x.Id) == id);
                            ViewBag.CustomersCode = vmCustomer.Code;
                            ViewBag.CustomersName = vmCustomer.Name;
                            ViewBag.CustomersId = vmCustomer.Id;
                            ViewBag.CustomersCurrentDue = !string.IsNullOrEmpty(vmCustomer.TotalDue) ?
                                Math.Round(decimal.Parse(vmCustomer.TotalDue), 2).ToString() : string.Empty;
                        }
                        return PartialView("~/Views/Pickers/_MSCustomerPicker.cshtml", vmCustomers);

                    }


                #region InvestmentHeads
                case PickerType.InvestmentHeads:
                    if (id != default(int))
                    {
                        var InvestmentHead = _ShareInvestmentHeadService.GetById(id);
                        ViewBag.SIHID = InvestmentHead.SIHID;
                        ViewBag.InvestmentHeadCode = InvestmentHead.Code;
                        ViewBag.HeadName = InvestmentHead.Name;
                    }
                    return PartialView("~/Views/Pickers/_InvestmentHeadPicker.cshtml");
                #endregion

                case PickerType.Territory:

                    var territorys = _TerritoryService.GetAllTerritory();
                    var vmTerritorys = _mapper.Map<IEnumerable<Territory>, IEnumerable<CreateTerritoryViewModel>>(territorys);
                    if (id != default(int))
                    {
                        var vmTerritory = vmTerritorys.First(x => int.Parse(x.Id) == id);
                        ViewBag.TerritorysCode = vmTerritory.Code;
                        ViewBag.TerritorysName = vmTerritory.Name;
                        ViewBag.TerritorysId = vmTerritory.Id;
                    }
                    return PartialView("~/Views/Pickers/_TerritoryPicker.cshtml", vmTerritorys);

                case PickerType.Depot:

                    var depots = _DepotService.GetAllDepot();
                    var vmDepots = _mapper.Map<IEnumerable<Depot>, IEnumerable<CreateDepotViewModel>>(depots);
                    if (id != default(int))
                    {
                        var vmDepot = vmDepots.First(x => int.Parse(x.Id) == id);
                        ViewBag.DepotsCode = vmDepot.Code;
                        ViewBag.DepotsName = vmDepot.Name;
                        ViewBag.DepotsId = vmDepot.Id;
                    }
                    return PartialView("~/Views/Pickers/_DepotPicker.cshtml", vmDepots);

                case PickerType.CCBank:

                    var CCBanks = _CCBanKService.GetAllCCBank();
                    var vmCCBanks = _mapper.Map<IEnumerable<CCBank>, IEnumerable<CCBankViewModel>>(CCBanks);
                    if (id != default(int))
                    {
                        var vmCCBank = vmCCBanks.First(x => int.Parse(x.Id) == id);
                        ViewBag.CCBanksCode = vmCCBank.Code;
                        ViewBag.CCBanksName = vmCCBank.Name;
                        ViewBag.CCBanksId = vmCCBank.Id;
                    }
                    return PartialView("~/Views/Pickers/_CCBankPicker.cshtml", vmCCBanks);

                case PickerType.FinGoods:

                    var FinGoods = _productService.GetAllProductIQueryableForProduction(EnumProductStockType.Finished_Goods);
                    //var vmFinGoods = _mapper.Map<IQueryable<ProductWisePurchaseModel>, List<GetProductViewModel>>(FinGoods);
                    if (id != default(int))
                    {
                        var vmProduct = FinGoods.First(x => x.ProductID == id);
                        ViewBag.ProductsCode = vmProduct.ProductCode;
                        ViewBag.ProductsName = vmProduct.ProductName;
                        ViewBag.ProductsId = vmProduct.ProductID;
                        ViewBag.ProductsType = (int)vmProduct.ProductType;
                    }
                    return PartialView("~/Views/Pickers/_ProductionSetupPicker.cshtml", FinGoods);


                case PickerType.ManualProductionPicker:

                    var ManualFinGoods = _productService.GetAllProductIQueryableForProduction(EnumProductStockType.Finished_Goods);
                    //var vmFinGoods = _mapper.Map<IQueryable<ProductWisePurchaseModel>, List<GetProductViewModel>>(FinGoods);
                    if (id != default(int))
                    {
                        var vmProduct = ManualFinGoods.First(x => x.ProductID == id);
                        ViewBag.ProductsCode = vmProduct.ProductCode;
                        ViewBag.ProductsName = vmProduct.ProductName;
                        ViewBag.ProductsId = vmProduct.ProductID;
                        ViewBag.ProductsType = (int)vmProduct.ProductType;
                    }
                    return PartialView("~/Views/Pickers/_ManualProductionPicker.cshtml", ManualFinGoods);

                case PickerType.ProductionProducts:

                    var ProductionProducts = from ps in _productionSetupService.GetAll()
                                             join p in _productService.GetAllProductIQueryableForProduction() on ps.FINProductID equals p.ProductID
                                             select new GetProductViewModel
                                             {
                                                 ProductId = p.ProductID,
                                                 ProductCode = p.ProductCode,
                                                 ProductName = p.ProductName,
                                                 ProductType = p.ProductType,
                                                 CategoryName = p.CategoryName,
                                                 CompanyName = p.CompanyName,
                                                 ParentUnit = p.ParentUnitName,
                                                 ChildUnit = p.ChildUnitName,
                                                 ConvertValue = p.ConvertValue
                                             };
                    if (id != default(int))
                    {
                        var vmProduct = ProductionProducts.First(x => x.ProductId == id);
                        ViewBag.ProductsCode = vmProduct.ProductCode;
                        ViewBag.ProductsName = vmProduct.ProductName;
                        ViewBag.ProductsId = vmProduct.ProductId;
                        ViewBag.ProductsType = (int)vmProduct.ProductType;
                    }
                    return PartialView("~/Views/Pickers/_ProductionProductPicker.cshtml", ProductionProducts);

                case PickerType.SupplierProducts:

                    if (id != default(int))
                    {
                        var vmProduct = _productService.GetAllProductIQueryable().First(x => x.ProductID == id);

                        ViewBag.ProductsCode = vmProduct.ProductCode;
                        ViewBag.ProductsName = vmProduct.ProductName;
                        ViewBag.ProductsId = vmProduct.ProductID;
                        ViewBag.ProductsPreviousStock = 0m;
                        ViewBag.ProductsType = (int)vmProduct.ProductType;
                    }
                    return PartialView("~/Views/Pickers/_SupplierProductDetailPicker.cshtml");
                default:
                    ViewBag.Type = "Invalid Picker";
                    return PartialView("~/Views/Pickers/_ModulePicker.cshtml", null);
            }
        }

        [ChildActionOnly]
        public PartialViewResult RenderProductPicker(PickerType pickerType, int id, int godownID)
        {
            switch (pickerType)
            {
                #region ProductWithGodown
                case PickerType.ProductDetail:

                    ViewBag.Type = "Details";
                    var customProductDetails = _productService.GetAllProductDetails();
                    var vmProductDetails = _mapper.Map<IEnumerable<Tuple<int, string, string,
                    decimal, string, string, string, Tuple<decimal?, string, decimal, decimal, int, string, int>>>, IEnumerable<GetProductViewModel>>(customProductDetails);

                    if (id != 0 && godownID != 0)
                    {
                        var vmProduct = vmProductDetails.First(x => x.ProductId == id && x.GodownID == godownID);
                        ViewBag.PDetailsCode = vmProduct.ProductCode;
                        ViewBag.PDetailsName = vmProduct.ProductName;
                        ViewBag.PDetailsId = vmProduct.ProductId;
                        ViewBag.PDetailsSalesRate = 0;
                        ViewBag.PDetailsPKTSheet = 0;
                        ViewBag.PDetailsPreStock = vmProduct.PreStock;
                        ViewBag.PDetailsPacket = 0;
                        ViewBag.PDetailsGodownID = vmProduct.GodownID;
                        ViewBag.PDetailsCategoryName = vmProduct.CategoryName;
                    }
                    return PartialView("~/Views/Pickers/_ProductDetailPicker2.cshtml", vmProductDetails);
                #endregion
                default:

                    ViewBag.Type = "Invalid Picker";
                    return PartialView("~/Views/Pickers/_ModulePicker.cshtml", null);
            }
        }
    }
}