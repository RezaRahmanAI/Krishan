using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Service
{
    public interface ICCBanKService
    {
        void AddCCBank(CCBank ccBank);
        void UpdateCCBank(CCBank ccBank);
        void SaveCCBank();
        IEnumerable<CCBank> GetAllCCBank();
        Task<IEnumerable<CCBank>> GetAllCCBankAsync();
        CCBank GetCCBankById(int id);
        void DeleteCCBank(int id);
        IQueryable<CCBank> GetAllIQueryable();


    }
}
