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
    public class CustomerService : ICustomerService
    {
        private readonly IBaseRepository<Customer> _customerRepository;
        private readonly IBaseRepository<Employee> _employeeRepository;
        private readonly IBaseRepository<SisterConcern> _SisterConcernRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBaseRepository<SOrder> _sorderRepository;
        private readonly IBaseRepository<CashCollection> _cashcollectionRepository;

        public CustomerService(IBaseRepository<Customer> customerRepository, 
            IBaseRepository<Employee> employeeRepository,IBaseRepository<SisterConcern> SisterConcernRepository,
            IUnitOfWork unitOfWork, IBaseRepository<SOrder> sorderRepository,
            IBaseRepository<CashCollection> cashcollectionRepository)
        {
            _customerRepository = customerRepository;
            _employeeRepository = employeeRepository;
            _unitOfWork = unitOfWork;
            _SisterConcernRepository = SisterConcernRepository;
            this._sorderRepository = sorderRepository;
            this._cashcollectionRepository = cashcollectionRepository;
        }

        public void AddCustomer(Customer product)
        {
            _customerRepository.Add(product);
        }

        public void UpdateCustomer(Customer product)
        {
            _customerRepository.Update(product);
        }

        public void SaveCustomer()
        {
            _unitOfWork.Commit();
        }

        public IEnumerable<Customer> GetAllCustomer()
        {
            return  _customerRepository.GetAllCustomer().ToList();
        }

        public List<TOCustomer> GetAllCustomerNew(int concernId, int customerId = 0)
        {
           
            string query = string.Format(@"SELECT CustomerID Id, code, Name, ContactNo, Address, TotalDue, CompanyName FROM Customers WHERE Concernid = {0}", concernId);
            if (customerId > 0)
            {
                query = string.Format(@"SELECT CustomerID Id, code, Name, ContactNo, Address, TotalDue, CompanyName FROM Customers WHERE ConcernId = {0} AND CustomerID = {1}", concernId, customerId);
            }
            return _customerRepository.SQLQueryList<TOCustomer>(query).ToList();
        }


        public IQueryable<Customer> GetAllIQueryable()
        {
            return _customerRepository.All;
        }
        public IEnumerable<Customer> GetAllCustomerByEmp(int EmpID)
        {
            return _customerRepository.GetAllCustomerByEmp(EmpID).ToList();
        }

        public IEnumerable<Customer> GetAllCustomerByEmpNew(int EmpID, int customerId = 0)
        {
            return _customerRepository.GetAllCustomerByEmpNew(EmpID, customerId).ToList();
        }


        public async Task<IEnumerable<Customer>> GetAllCustomerAsyncByEmpID(int EmpID)
        {
            return await _customerRepository.GetAllCustomerAsyncByEmpID(EmpID);
        }

        public async Task<IEnumerable<Customer>> GetAllCustomerAsync()
        {
            return await _customerRepository.GetAllCustomerAsync();
        }

        public Customer GetCustomerById(int id)
        {
            return _customerRepository.FindBy(x=>x.CustomerID == id).First();
        }

        public IEnumerable<Tuple<string, string, string, string, string, string, decimal, Tuple<string>>>
        CustomerCategoryWiseDueRpt(int concernId, int customerId, int reportType, int DueType)
        {
            return _customerRepository.CustomerCategoryWiseDueRpt(concernId,customerId,reportType,DueType);
        }

        public IQueryable<Customer> GetAll()
        {
            return _customerRepository.All;
        }
        public IQueryable<Customer> GetAll(int ConcernID)
        {
            return _customerRepository.GetAll().Where(i => i.ConcernID == ConcernID);
        }


        public void DeleteCustomer(int id)
        {
            _customerRepository.Delete(x => x.CustomerID == id);
        }


        public IQueryable<SRWiseCustomerStatusReportModel> AdminCustomerDueReport(int concernID, int CustomerType, int DueType)
        {
            return _customerRepository.AdminCustomerDueReport(_SisterConcernRepository, concernID, CustomerType, DueType);
        }

        public string GetUniqueCodeByType(EnumCustomerType customerType)
        {
            string Code = string.Empty;
            if (_customerRepository.All.Any(i => i.CustomerType == customerType))
            {
                var LastCustomer = _customerRepository.All.Where(i => i.CustomerType == customerType).OrderByDescending(i => i.CustomerID).FirstOrDefault();
                Code = (Convert.ToInt64((LastCustomer.Code.Substring(1))) + 1).ToString("D5");
            }
            else
                Code = "00001";

            if (customerType > 0)
            {
                Code = customerType.ToString().Substring(0, 1) + Code;
            }


            //switch (customerType)
            //{
            //    case EnumCustomerType.Retail:
            //        Code = "R" + Code;
            //        break;
            //    case EnumCustomerType.Dealer:
            //        Code = "D" + Code;
            //        break;
            //    default:
            //        break;
            //}

            return Code;
        }

        public bool IsCustomerSalesOrCollectionExists(int customerID)
        {
            if (_sorderRepository.All.Any(i => i.Status == (int)EnumSalesType.Sales && i.CustomerID == customerID))
                return true;

            if (_cashcollectionRepository.All.Any(i => i.TransactionType == EnumTranType.FromCustomer && i.CustomerID == customerID))
                return true;

            return false;
        }

        public bool IsCollectionFound(int customerId)
        {
            string query = string.Format(@"SELECT ISNULL(COUNT(*),0) FROM CashCollections WHERE CustomerID IN({0})", customerId);

            int result = _customerRepository.SQLQuery<int>(query);
            return result > 0;
        }

    }
}
