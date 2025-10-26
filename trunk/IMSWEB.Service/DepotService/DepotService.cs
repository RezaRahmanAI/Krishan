using IMSWEB.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IMSWEB.Model;
using IMSWEB.Model.TOs;


namespace IMSWEB.Service
{
    public class DepotService : IDepotService
    {

        private readonly IBaseRepository<Depot> _DepotRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DepotService(IBaseRepository<Depot> DepotRepository, IUnitOfWork unitOfWork)
        {
            _DepotRepository = DepotRepository;
            _unitOfWork = unitOfWork;
        }

        public void AddDepot(Depot depot)
        {
            _DepotRepository.Add(depot);
        }

        public void UpdateDepot(Depot depot)
        {
            _DepotRepository.Update(depot);
        }

        public void SaveDepot()
        {
            _unitOfWork.Commit();
        }

        public IEnumerable<Depot> GetAllDepot()
        {
            return _DepotRepository.All.ToList();
        }

        public async Task<IEnumerable<Depot>> GetAllDepotAsync()
        {
            return await _DepotRepository.GetAllDepotAsync();
        }

        public Depot GetDepotById(int id)
        {
            return _DepotRepository.FindBy(x => x.DepotID == id).First();
        }

        public void DeleteDepot(int id)
        {
            _DepotRepository.Delete(x => x.DepotID == id);
        }


        public Depot GetDepotByConcernAndDepotName(int ConcernID, string DepotName)
        {
            return _DepotRepository.GetAll().FirstOrDefault(i => i.ConcernID == ConcernID && i.DepotName.ToLower().Equals(DepotName));
        }

        public IQueryable<Depot> GetAll()
        {
            return _DepotRepository.All;
        }

        public IQueryable<Depot> GetAllDepotIQueryable()
        {
            return _DepotRepository.All;
        }
        public string GetDepotNameById(int DepotId)
        {
            var depot = _DepotRepository.FindBy(t => t.DepotID == DepotId).FirstOrDefault();
            return depot != null ? depot.DepotName : string.Empty;
        }
        public List<TOCustomer> GetAllDepotNew(int concernId, int depotId = 0)
        {

            string query = string.Format(@"SELECT DepotID Id, code, DepotName Name FROM Depots WHERE Concernid = {0}", concernId);
            if (depotId > 0)
            {
                query = string.Format(@"SELECT DepotID Id, code, DepotName Name FROM Depots WHERE ConcernId = {0} AND DepotID = {1}", concernId, depotId);
            }
            return _DepotRepository.SQLQueryList<TOCustomer>(query).ToList();
        }
    }
}
