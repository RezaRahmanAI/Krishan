using IMSWEB.Model;
using IMSWEB.Model.TO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Service
{
    public interface IAboutUSService
    {
        void Add(AboutUS Model);
        void Update(AboutUS Model);
        void SaveAboutUs();
        bool Save();
        void Delete(int id);
        IEnumerable<AboutUS> GetAll();
        Task<IEnumerable<AboutUS>> GetAllAsync();  
        AboutUS GetById(int id);


    }
}
