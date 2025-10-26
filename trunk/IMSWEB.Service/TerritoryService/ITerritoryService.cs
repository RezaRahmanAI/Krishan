using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Service
{
    public interface ITerritoryService
    {
        void AddTerritory(Territory territory);
        void UpdateTerritory(Territory territory);
        void SaveTerritory();
        IEnumerable<Territory> GetAllTerritory();
        IQueryable<Territory> GetAll();
        Task<IEnumerable<Territory>> GetAllTerritoryAsync();
        Territory GetTerritoryById(int id);
        void DeleteTerritory(int id);

        Territory GetTerritoryByConcernAndTerritoryName(int ConcernID, string TerritoryName);

        IQueryable<Territory> GetAllTerritoryIQueryable();
        string GetTerritoryNameById(int territoryId);
    }
}
