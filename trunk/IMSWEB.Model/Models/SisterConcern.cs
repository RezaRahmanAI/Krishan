using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IMSWEB.Model
{
    public partial class SisterConcern
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public SisterConcern()
        {
            CashCollections = new HashSet<CashCollection>();
            Categorys = new HashSet<Category>();
            Companies = new HashSet<Company>();
            CreditSales = new HashSet<CreditSale>();
            Customers = new HashSet<Customer>();
            DamageProducts = new HashSet<DamageProduct>();
            Employees = new HashSet<Employee>();
            Expenditures = new HashSet<Expenditure>();
            Colors = new HashSet<Color>();
            Users = new HashSet<ApplicationUser>();
            POrders = new HashSet<POrder>();
            SOrders = new HashSet<SOrder>();
            Stocks = new HashSet<Stock>();
            Suppliers = new HashSet<Supplier>();
            SystemInformations = new HashSet<SystemInformation>();
            CommissionSetups = new HashSet<CommissionSetup>();
            TargetSetups = new HashSet<TargetSetup>();
            DesWiseCommissions = new HashSet<DesWiseCommission>();
            Sizes = new HashSet<Size>();
            ProductUnitTypes = new HashSet<ProductUnitType>();
            Banks = new HashSet<Bank>();
            Departments = new HashSet<Department>();
            ExpenseItems = new HashSet<ExpenseItem>();
            EmployeeWiseCustomerDues = new HashSet<EmployeeWiseCustomerDue>();
            CashAccounts = new HashSet<CashAccount>();

        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ConcernID { get; set; }

        [Required]
        public string Name { get; set; }

        [StringLength(350)]
        public string Address { get; set; }

        [StringLength(250)]
        public string ContactNo { get; set; }
        
        public decimal SalesShowPercent { get; set; }

        public int ParentID { get; set; }
        public decimal ServiceCharge { get; set; }
        [StringLength(20)]
        public string SmsContactNo { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CashCollection> CashCollections { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Category> Categorys { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Company> Companies { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CreditSale> CreditSales { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Customer> Customers { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DamageProduct> DamageProducts { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Employee> Employees { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Expenditure> Expenditures { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Color> Colors { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ApplicationUser> Users { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<POrder> POrders { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SOrder> SOrders { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Stock> Stocks { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Supplier> Suppliers { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SystemInformation> SystemInformations { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CommissionSetup> CommissionSetups { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TargetSetup> TargetSetups { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DesWiseCommission> DesWiseCommissions { get; set; }
        public virtual ICollection<Size> Sizes { get; set; }
        public virtual ICollection<ProductUnitType> ProductUnitTypes { get; set; }
        public virtual ICollection<Bank> Banks { get; set; }
        public virtual ICollection<Department> Departments { get; set; }
        public virtual ICollection<ExpenseItem> ExpenseItems { get; set; }
        public ICollection<EmployeeWiseCustomerDue> EmployeeWiseCustomerDues { get; set; }
        public ICollection<CashAccount> CashAccounts { get; set; }

    }
}
