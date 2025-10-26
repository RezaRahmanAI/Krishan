using IMSWEB.Data;
using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Service
{
    public class BankService : IBankService
    {
        private readonly IBaseRepository<Bank> _bankRepository;
        private readonly IUnitOfWork _unitOfWork;

        public BankService(IBaseRepository<Bank> bankRepository, IUnitOfWork unitOfWork)
        {
            _bankRepository = bankRepository;
            _unitOfWork = unitOfWork;
        }

        public void AddBank(Bank bank)
        {
            _bankRepository.Add(bank);
        }

        public void UpdateBank(Bank bank)
        {
            _bankRepository.Update(bank);
        }

        public void SaveBank()
        {
            _unitOfWork.Commit();
        }

        public IEnumerable<Bank> GetAllBank()
        {
            return _bankRepository.All.ToList();
        }

        public async Task<IEnumerable<Bank>> GetAllBankAsync()
        {
            return await _bankRepository.GetAllBankAsync();
        }

        public Bank GetBankById(int id)
        {
            return _bankRepository.FindBy(x => x.BankID == id).First();
        }

        public void DeleteBank(int id)
        {
            _bankRepository.Delete(x => x.BankID == id);
        }
    }
}
