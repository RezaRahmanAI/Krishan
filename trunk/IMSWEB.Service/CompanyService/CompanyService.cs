using IMSWEB.Data;
using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Service
{
    public class CompanyService : ICompanyService
    {
        private readonly IBaseRepository<Company> _companyRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CompanyService(IBaseRepository<Company> companyRepository, IUnitOfWork unitOfWork)
        {
            _companyRepository = companyRepository;
            _unitOfWork = unitOfWork;
        }

        public void AddCompany(Company comapany)
        {
            _companyRepository.Add(comapany);
        }

        public void UpdateCompany(Company comapany)
        {
            _companyRepository.Update(comapany);
        }

        public void SaveCompany()
        {
            _unitOfWork.Commit();
        }

        public IEnumerable<Company> GetAllCompany()
        {
            return _companyRepository.All.ToList();
        }

        public async Task<IEnumerable<Company>> GetAllCompanyAsync()
        {
            return await _companyRepository.GetAllCompanyAsync();
        }

        public Company GetCompanyById(int id)
        {
            return _companyRepository.FindBy(x=>x.CompanyID == id).First();
        }

        public Company GetCompanyByName(string name)
        {
            return _companyRepository.FindBy(x => x.Name.ToLower().Equals(name.ToLower())).FirstOrDefault();
        }

        public void DeleteCompany(int id)
        {
            _companyRepository.Delete(x => x.CompanyID == id);
        }

        public IQueryable<Company> GetAllIQueryable()
        {
            return _companyRepository.All;
        }
    }
}
