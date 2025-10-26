using IMSWEB.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IMSWEB.Model;

namespace IMSWEB.Service
{
    public class CashAccountService : ICashAccountService
    {
        private readonly IBaseRepository<CashAccount> _baseRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CashAccountService(IBaseRepository<CashAccount> baseRepository, IUnitOfWork unitOfWork)
        {
            _baseRepository = baseRepository;
            _unitOfWork = unitOfWork;
        }

        public void Add(CashAccount model)
        {
            _baseRepository.Add(model);
        }

        public void Update(CashAccount model)
        {
            _baseRepository.Update(model);
        }

        //public void Save()
        //{
        //    _unitOfWork.Commit();
        //}
        public bool Save()
        {
            try
            {
                _unitOfWork.Commit();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        public IEnumerable<CashAccount> GetAll()
        {
            return _baseRepository.All.ToList();
        }

        public CashAccount GetById(int id)
        {
            return _baseRepository.FindBy(x => x.Id == id).FirstOrDefault();
        }

        public CashAccount GetFirst()
        {
            return _baseRepository.All.FirstOrDefault();
        }

        public void Delete(int id)
        {
            _baseRepository.Delete(x => x.Id == id);
        }
        public async Task<IEnumerable<CashAccount>> GetAllCashAccountAsync()
        {
            return await _baseRepository.GetAllCashAccountAsync();
        }
    }
}
