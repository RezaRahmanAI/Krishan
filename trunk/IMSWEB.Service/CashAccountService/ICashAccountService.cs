using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Service
{
    public interface ICashAccountService
    {
        void Add(CashAccount model);
        void Update(CashAccount model);
        bool Save();
        //void Save();
        IEnumerable<CashAccount> GetAll();
        CashAccount GetById(int id);
        CashAccount GetFirst();
        void Delete(int id);
        Task<IEnumerable<CashAccount>> GetAllCashAccountAsync();
    }
}
