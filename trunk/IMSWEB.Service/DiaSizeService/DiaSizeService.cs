using IMSWEB.Data;
using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Service
{
    public class DiaSizeService : IDiaSizeService
    {
        private readonly IBaseRepository<DiaSize> _baseRepository;
        private readonly IBaseRepository<DiaSize> _DiaSizeRepository;

        private readonly IUnitOfWork _unitOfWork;
        public DiaSizeService(IBaseRepository<DiaSize> baseRepository, IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _baseRepository = baseRepository;
        }
        public void Add(DiaSize DiaSize)
        {
            _baseRepository.Add(DiaSize);
        }
        public void Update(DiaSize DiaSize)
        {
            _baseRepository.Update(DiaSize);
        }
        public void Save()
        {
            _unitOfWork.Commit(); ;
        }
        public IQueryable<DiaSize> GetAll()
        {
            return _baseRepository.All;
        }

        public DiaSize GetById(int id)
        {
            return _baseRepository.FindBy(x => x.DiaSizeID == id).First();

        }
        public void Delete(int id)
        {
            _baseRepository.Delete(x => x.DiaSizeID == id);
        }
        public IQueryable<DiaSize> GetAllIQueryable()
        {
            return _DiaSizeRepository.All;
        }


        public IEnumerable<DiaSize> GetAllDiaSize()
        {
            return _DiaSizeRepository.All.ToList();
        }

        public IQueryable<DiaSize> GetAllIQueryable(int ConcernID)
        {
            return _DiaSizeRepository.GetAll().Where(i => i.ConcernID == ConcernID);
        }
    }
}
