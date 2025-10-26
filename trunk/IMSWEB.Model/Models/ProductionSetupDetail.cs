using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Model
{
    public class ProductionSetupDetail
    {
        [Key]
        public int PSDID { get; set; }

        [ForeignKey("Product")]
        public int RAWProductID { get; set; }
        public virtual Product Product { get; set; }
        public decimal Quantity { get; set; }
        public int PSID { get; set; }
        public decimal ParentQuantity { get; set; }
        public decimal ChildQuantity { get; set; }
        public virtual ProductionSetup ProductionSetup { get; set; }
    }
}
