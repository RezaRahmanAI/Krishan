using IMSWEB.Model;
using IMSWEB.Model.TOs;
using IMSWEB.Model.TO.Bkash;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Service
{
    public interface ISisterConcernService
    {
        void AddSisterConcern(SisterConcern SisterConcern);
        void UpdateSisterConcern(SisterConcern SisterConcern);
        void SaveSisterConcern();
        IEnumerable<SisterConcern> GetAllSisterConcern();
        Task<IEnumerable<SisterConcern>> GetAllSisterConcernAsync();
        SisterConcern GetSisterConcernById(int id);
        void DeleteSisterConcern(int id);
        IEnumerable<SisterConcern> GetAll();
        List<SisterConcern> GetFamilyTree(int ConcernID);
        List<TOIdNameDDL> GetFamilyTreeDDL(int concernId);

        List<SisterConcern> GetFamilyTreeForNotLoggedInUser(int ConcernID);
        bool IsChildConcern(int concernId);

        string GetConcernNameById(int concernId);

        List<SisterConcerPayTO> GetAllConcernByConcernId(int concernId);
    }
}
