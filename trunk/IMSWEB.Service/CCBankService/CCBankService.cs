using IMSWEB.Data;
using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Service
{
    public class CCBanKService : ICCBanKService
    {
        private readonly IBaseRepository<CCBank> _CCBankRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CCBanKService(IBaseRepository<CCBank> CCBankRepository, IUnitOfWork unitOfWork)
        {
            _CCBankRepository = CCBankRepository;
            _unitOfWork = unitOfWork;
        }

        public void AddCCBank(CCBank ccBank)
        {
            _CCBankRepository.Add(ccBank);
        }

        public void UpdateCCBank(CCBank ccBank)
        {
            _CCBankRepository.Update(ccBank);
        }

        public void SaveCCBank()
        {
            _unitOfWork.Commit();
        }

        public IEnumerable<CCBank> GetAllCCBank()
        {
            return _CCBankRepository.All.ToList();
        }

        public async Task<IEnumerable<CCBank>> GetAllCCBankAsync()
        {
            return await _CCBankRepository.GetAllCCBankAsync();
        }

        public CCBank GetCCBankById(int id)
        {
            return _CCBankRepository.FindBy(x => x.CCBankID == id).First();
        }

        public void DeleteCCBank(int id)
        {
            _CCBankRepository.Delete(x => x.CCBankID == id);
        }

        public IQueryable<CCBank> GetAllIQueryable()
        {
            return _CCBankRepository.All;
        }


    }
}
