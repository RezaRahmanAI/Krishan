using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IMSWEB.Model.TOs;


namespace IMSWEB.Service
{
    public interface IDepotService
    {
        void AddDepot(Depot depot);
        void UpdateDepot(Depot depot);
        void SaveDepot();
        IEnumerable<Depot> GetAllDepot();
        IQueryable<Depot> GetAll();
        Task<IEnumerable<Depot>> GetAllDepotAsync();
        Depot GetDepotById(int id);
        void DeleteDepot(int id);

        Depot GetDepotByConcernAndDepotName(int ConcernID, string DepotName);

        IQueryable<Depot> GetAllDepotIQueryable();
        string GetDepotNameById(int DepotId);
        List<TOCustomer> GetAllDepotNew(int concernId, int depotId = 0);
    }
}
