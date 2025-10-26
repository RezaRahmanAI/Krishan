using IMSWEB.Data;
using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Service
{
    public class EmployeeTargetSetupService : IEmployeeTargetSetupService
    {
        private readonly IBaseRepository<EmployeeTargetSetup> _baseRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBaseRepository<Employee> _EmployeeRepository;


        public EmployeeTargetSetupService(IBaseRepository<EmployeeTargetSetup> baseRepository,
            IBaseRepository<Employee> EmployeeRepository, IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _baseRepository = baseRepository;
            _EmployeeRepository = EmployeeRepository;
        }

        public void Add(EmployeeTargetSetup EmployeeTargetSetup)
        {
            _baseRepository.Add(EmployeeTargetSetup);
        }

        public void Update(EmployeeTargetSetup EmployeeTargetSetup)
        {
            _baseRepository.Update(EmployeeTargetSetup);
        }

        public void Save()
        {
            _unitOfWork.Commit(); ;
        }

        public IQueryable<EmployeeTargetSetup> GetAll()
        {
            return _baseRepository.All;
        }
        public IQueryable<EmployeeTargetSetup> GetAll(DateTime FromDate, DateTime ToDate)
        {
            return _baseRepository.All.Where(i => i.TargetMonth >= FromDate && i.TargetMonth <= ToDate);
        }

        public EmployeeTargetSetup GetById(int id)
        {
            return _baseRepository.FindBy(x => x.ETSID == id).First();

        }

        public void Delete(int id)
        {
            _baseRepository.Delete(x => x.ETSID == id);
        }

        public IQueryable<EmployeeTargetSetup> GetByEmployeeIDandMonth(int EmployeeID, DateTime fromDate, DateTime toDate)
        {
            return _baseRepository.All.Where(i => i.EmployeeID == EmployeeID && i.TargetMonth >= fromDate && i.TargetMonth <= toDate);
        }

        public async Task<IEnumerable<Tuple<int, DateTime, decimal, decimal, decimal, decimal, int, Tuple<string>>>> GetAllAsync()
        {
            return await _baseRepository.GetAllAsync(_EmployeeRepository);
        }
        public EmployeeTargetSetup GetByEmployeeIDandTargetMonth(int EmployeeID, DateTime fromDate, DateTime toDate)
        {
            return _baseRepository.AllIncluding().FirstOrDefault(i => i.EmployeeID == EmployeeID && (i.TargetMonth >= fromDate && i.TargetMonth <= toDate));
        }
    }
}
