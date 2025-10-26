using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Model
{
    public class EmployeeTargetSetup : AuditTrailModel
    {
        [Key]
        public int ETSID { get; set; }
        public int EmployeeID { get; set; }
        public decimal TargetAmt { get; set; }
        public DateTime TargetMonth { get; set; }
        public virtual SisterConcern SisterConcern { get; set; }
        public int ConcernID { get; set; }

    }
}
