using IMSWEB.Data;
using IMSWEB.Data.Extentions;
using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IMSWEB.Service
{
    public class EmployeeWiseCustomerDueService : IEmployeeWiseCustomerDueService
    {
        private readonly IBaseRepository<EmployeeWiseCustomerDue> _baseRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBaseRepository<Employee> employeeRepository;
        private readonly IBaseRepository<Customer> _customerRepository;
        private readonly IBaseRepository<Designation> _designationRepository;
        private readonly IBaseRepository<Department> _departmentRepository;
        private readonly IBaseRepository<Grade> _gradeRepository;

        public EmployeeWiseCustomerDueService(IBaseRepository<EmployeeWiseCustomerDue> baseRepository,
            IUnitOfWork unitOfWork, IBaseRepository<Employee> employeeRepository,
            IBaseRepository<Customer> customerRepository, IBaseRepository<Designation> designationRepository,
            IBaseRepository<Department> departmentRepository, IBaseRepository<Grade> gradeRepository)
        {
            _unitOfWork = unitOfWork;
            this.employeeRepository = employeeRepository;
            this._customerRepository = customerRepository;
            this._designationRepository = designationRepository;
            this._departmentRepository = departmentRepository;
            this._gradeRepository = gradeRepository;
            _baseRepository = baseRepository;
        }

        public void Add(EmployeeWiseCustomerDue EmployeeWiseCustomerDue)
        {
            _baseRepository.Add(EmployeeWiseCustomerDue);
        }

        public void Update(EmployeeWiseCustomerDue EmployeeWiseCustomerDue)
        {
            _baseRepository.Update(EmployeeWiseCustomerDue);
        }

        public void Save()
        {
            _unitOfWork.Commit();
        }

        public IQueryable<EmployeeWiseCustomerDue> GetAll()
        {
            return _baseRepository.GetAll();
        }

        public IQueryable<EmployeeWiseCustomerDue> Get(int? id = null, int? employeeID = null, int? customerID = null)
        {
            return _baseRepository.FindBy(x => (employeeID.HasValue ? x.EmployeeID == employeeID : true)
            && (customerID.HasValue ? x.CustomerID == customerID : true));
        }

        public void DeleteByEmplyeeID(int employeeID)
        {
            _baseRepository.Delete(x => x.EmployeeID == employeeID);
        }

        public List<Tuple<int, int, string, string, string,
                int, string, Tuple<string, string, decimal, decimal>>> GetEmployeeCustomerData(int? EmployeeID, int? CustomerID)
        {
            return _baseRepository.GetMappingData(employeeRepository, _customerRepository, _designationRepository, _departmentRepository,
                _gradeRepository, EmployeeID, CustomerID);
        }
        public TOSRDueDetails GetSRDueDetails(int customerId, int? employeeId)
        {
            return _baseRepository.GetSRDueDetails(employeeRepository, _customerRepository, customerId, employeeId);
        }

        public void DeleteByID(int id)
        {
            _baseRepository.Delete(x => x.ID == id);
        }

        public EmployeeWiseCustomerDue GetByEmpCustomer(int? employeeID, int customerID)
        {
            return _baseRepository.FindBy(e => (employeeID.HasValue ? e.EmployeeID == employeeID.Value : true) && e.CustomerID == customerID).FirstOrDefault();
        }
    }
}
