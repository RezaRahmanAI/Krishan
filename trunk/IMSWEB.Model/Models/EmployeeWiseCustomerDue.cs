using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Model
{
    public class EmployeeWiseCustomerDue : AuditTrailModel
    {
        public int ID { get; set; }
        public virtual Employee Employee { get; set; }
        public int EmployeeID { get; set; }
        public virtual Customer Customer { get; set; }
        public int CustomerID { get; set; }
        public int ConcernID { get; set; }
        public virtual SisterConcern SisterConcern { get; set; }
        public decimal CustomerDue { get; set; }
        public decimal CustomerOpeningDue { get; set; }
    }
}
