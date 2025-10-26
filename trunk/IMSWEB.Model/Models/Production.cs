using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Model
{
    public class Production : AuditTrailModel
    {
        public Production()
        {
            ProductionDetails = new HashSet<ProductionDetail>();
        }
        [Key]
        public int ProductionID { get; set; }
        public string ProductionCode { get; set; }
        public DateTime Date { get; set; }
        public EnumProductionStatus Status { get; set; }
        public virtual SisterConcern SisterConcern { get; set; }
        public int ConcernID { get; set; }
        public virtual ICollection<ProductionDetail> ProductionDetails { get; set; }
    }
}

