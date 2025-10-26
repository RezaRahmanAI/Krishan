using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Model
{
    public interface IModelBase
    {
        int CreatedBy { get; set; }

        DateTime CreateDate { get; set; }

        int? ModifiedBy { get; set; }

        DateTime? ModifiedDate { get; set; }
    }
}
