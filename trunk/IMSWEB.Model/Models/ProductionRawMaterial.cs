using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Model
{
    public class ProductionRawMaterial
    {
        [Key]
        public int PRMID { get; set; }
        public virtual Product Product { get; set; }
        public int ProductID { get; set; }
        public int SDetailID { get; set; }
        public decimal Quantity { get; set; }
        public decimal PRate { get; set; }
        public virtual ProductionDetail ProductionDetail { get; set; }
        public int PDetailID { get; set; }

    }
}


