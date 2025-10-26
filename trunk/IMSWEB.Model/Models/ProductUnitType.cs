using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Model
{
    public class ProductUnitType
    {
        public ProductUnitType()
        {
            Products = new HashSet<Product>();
        }
        [Key]
        public int ProUnitTypeID { get; set; }
        public string Description { get; set; }
        public decimal ConvertValue { get; set; }
        public int Status { get; set; }
        public int Position { get; set; }
        public string UnitName { get; set; }
        public virtual SisterConcern SisterConcern { get; set; }
        public int ConcernID { get; set; }
        public virtual ICollection<Product> Products{ get; set; }
    }
}
