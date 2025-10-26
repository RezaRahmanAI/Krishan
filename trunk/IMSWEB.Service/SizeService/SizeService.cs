using IMSWEB.Data;
using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Service
{
    public class SizeService : ISizeService
    {
        private readonly IBaseRepository<Size> _baseRepository;

        private readonly IUnitOfWork _unitOfWork;
        public SizeService(IBaseRepository<Size> baseRepository, IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _baseRepository = baseRepository;
        }
        public void Add(Size Size)
        {
           _baseRepository.Add(Size);
        }
        public void Update(Size Size)
        {
            _baseRepository.Update(Size);
        }
        public void Save()
        {
            _unitOfWork.Commit(); ;
        }
        public IQueryable<Size> GetAll()
        {
            return _baseRepository.All;
        }
        //public async Task<IEnumerable<Size>> GetAllAsync()
        //{
        //    return await _baseRepository.GetAllGradeAsync();
        //}
        public Size GetById(int id)
        {
            return _baseRepository.FindBy(x => x.SizeID == id).First();

        }
        public void Delete(int id)
        {
            _baseRepository.Delete(x => x.SizeID == id);
        }
        public IQueryable<Size> GetAllIQueryable()
        {
            return _baseRepository.All;
        }


        public IEnumerable<Size> GetAllSize()
        {
            return _baseRepository.All.ToList();
        }

        public IQueryable<Size> GetAllIQueryable(int ConcernID)
        {
            return _baseRepository.GetAll().Where(i => i.ConcernID == ConcernID);
        }
    }
}
