using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Data
{
    public interface ITransferRepository
    {
        Tuple<bool, int> AddTransferUsingSP(DataTable dtTransferOrder, DataTable dtDetails, DataTable dtTransferFromStock, DataTable dtTransferToStock);

        bool TranserferReturnUsingSP(int TransferID);

        bool TranserferApprovedByReceiverUsingSP(DataTable dtDetails, DataTable dtTransferToStock, int TransferID, int ApprovedBy, DateTime ApprovedDate, int ConcernID);

    }
}
