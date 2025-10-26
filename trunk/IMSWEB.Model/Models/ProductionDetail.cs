using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Model
{
    public class ProductionDetail
    {
        public ProductionDetail()
        {
            ProductionRawMaterials = new HashSet<ProductionRawMaterial>();
        }
        [Key]
        public int PDetailID { get; set; }
        public virtual Product Product { get; set; }
        public int ProductID { get; set; }
        public decimal Quantity { get; set; }
        public decimal TotalCost { get; set; }
        public virtual Production Production { get; set; }
        public int ProductionID { get; set; }

        public virtual ICollection<ProductionRawMaterial> ProductionRawMaterials { get; set; }
    }
}
