using IMSWEB.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IMSWEB.Model;

namespace IMSWEB.Service
{
    public class TerritoryService : ITerritoryService
    {

        private readonly IBaseRepository<Territory> _TerritoryRepository;
        private readonly IUnitOfWork _unitOfWork;

        public TerritoryService(IBaseRepository<Territory> TerritoryRepository, IUnitOfWork unitOfWork)
        {
            _TerritoryRepository = TerritoryRepository;
            _unitOfWork = unitOfWork;
        }

        public void AddTerritory(Territory territory)
        {
            _TerritoryRepository.Add(territory);
        }

        public void UpdateTerritory(Territory territory)
        {
            _TerritoryRepository.Update(territory);
        }

        public void SaveTerritory()
        {
            _unitOfWork.Commit();
        }

        public IEnumerable<Territory> GetAllTerritory()
        {
            return _TerritoryRepository.All.ToList();
        }

        public async Task<IEnumerable<Territory>> GetAllTerritoryAsync()
        {
            return await _TerritoryRepository.GetAllTerritoryAsync();
        }

        public Territory GetTerritoryById(int id)
        {
            return _TerritoryRepository.FindBy(x => x.TerritoryID == id).First();
        }

        public void DeleteTerritory(int id)
        {
            _TerritoryRepository.Delete(x => x.TerritoryID == id);
        }


        public Territory GetTerritoryByConcernAndTerritoryName(int ConcernID, string TerritoryName)
        {
            return _TerritoryRepository.GetAll().FirstOrDefault(i => i.ConcernID == ConcernID && i.TerritoryName.ToLower().Equals(TerritoryName));
        }

        public IQueryable<Territory> GetAll()
        {
            return _TerritoryRepository.All;
        }

        public IQueryable<Territory> GetAllTerritoryIQueryable()
        {
            return _TerritoryRepository.All;
        }

        public string GetTerritoryNameById(int territoryId)
        {
            var terr = _TerritoryRepository.FindBy(t => t.TerritoryID == territoryId).FirstOrDefault();
            return terr != null ? terr.TerritoryName : string.Empty;
        }
    }
}
