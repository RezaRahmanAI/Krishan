using IMSWEB.Data;
using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Service
{
    public class DocumentsInfoService : IDocumentsInfoService
    {
        private readonly IBaseRepository<DocumentsInfo> _documentsRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DocumentsInfoService(IBaseRepository<DocumentsInfo> documentsRepository, IUnitOfWork unitOfWork)
        {
            _documentsRepository = documentsRepository;
            _unitOfWork = unitOfWork;
        }

        public void Add(DocumentsInfo model)
        {
            _documentsRepository.Add(model);
        }

        public void Delete(int id)
        {
            _documentsRepository.Delete(d => d.Id == id);
        }

        public IEnumerable<DocumentsInfo> GetAll()
        {
            return _documentsRepository.All;
        }

        public List<DocumentsInfo> GetByDocTypeAndSource(string docType, int docSourceId)
        {
            return _documentsRepository.FindBy(d => d.DocType.ToLower().Equals(docType.ToLower()) && d.DocSourceId == docSourceId).ToList();
        }

        public DocumentsInfo GetById(int id)
        {
            return _documentsRepository.FindBy(d => d.Id == id).FirstOrDefault();
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

        public void Update(DocumentsInfo model)
        {
            _documentsRepository.Update(model);
        }
    }
}
