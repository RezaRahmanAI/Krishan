using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Service
{
    public interface IDocumentsInfoService
    {
        void Add(DocumentsInfo model);
        void Update(DocumentsInfo model);
        bool Save();
        IEnumerable<DocumentsInfo> GetAll();
        DocumentsInfo GetById(int id);
        void Delete(int id);
        List<DocumentsInfo> GetByDocTypeAndSource(string docType, int docSourceId);
    }
}
