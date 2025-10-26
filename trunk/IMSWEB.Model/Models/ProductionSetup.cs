using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Model
{
    public class ProductionSetup : AuditTrailModel
    {
        public ProductionSetup()
        {
            ProductionSetupDetails = new HashSet<ProductionSetupDetail>();
        }
        [Key]
        public int PSID { get; set; }

        [ForeignKey("Product")]
        public int FINProductID { get; set; }
        public virtual Product Product { get; set; }
        public SisterConcern SisterConcern { get; set; }
        public int ConcernID { get; set; }

        public ICollection<ProductionSetupDetail> ProductionSetupDetails { get; set; }
    }
}

