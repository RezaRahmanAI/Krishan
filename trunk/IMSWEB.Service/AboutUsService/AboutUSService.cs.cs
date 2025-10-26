using IMSWEB.Data;
using IMSWEB.Model;
using IMSWEB.Model.TO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace IMSWEB.Service
{
    public class AboutUsService : IAboutUSService
    {
        private readonly IBaseRepository<AboutUS> _aboutUsRepository;
        private readonly IBaseRepository<SisterConcern> _SisterConcernRepository;
        private readonly IUnitOfWork _unitOfWork;

        public AboutUsService(IBaseRepository<AboutUS> aboutUsRepository,
            IBaseRepository<SisterConcern> SisterConcernRepository,
            IUnitOfWork unitOfWork)
        {
            _aboutUsRepository = aboutUsRepository;
            _unitOfWork = unitOfWork;
            _SisterConcernRepository = SisterConcernRepository;
        }

        public void Add(AboutUS Model)
        {
            _aboutUsRepository.Add(Model);
        }

        public void Update(AboutUS Model)
        {
            _aboutUsRepository.Update(Model);
        }

        public void SaveAboutUs()
        {
            _unitOfWork.Commit();
        }

        public bool Save()
        {
            try
            {
                _unitOfWork.Commit();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        public void Delete(int id)
        {
            _aboutUsRepository.Delete(x => x.Id == id);
        }



        public IEnumerable<AboutUS> GetAll() 
        {
            return _aboutUsRepository.GetAll();
        }

        public async Task<IEnumerable<AboutUS>> GetAllAsync()
        {
            return await _aboutUsRepository.GetAllAsyncAbout();
        }

      

        public AboutUS GetById(int id)
        {
            return _aboutUsRepository.FindBy(x => x.Id == id).First();
        }


    }
}

