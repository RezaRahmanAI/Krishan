using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Model
{
    public class Size
    {
        public Size()
        {
            Products = new HashSet<Product>();
        }
        [Key]
        public int SizeID { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public virtual SisterConcern SisterConcern { get; set; }
        public int ConcernID { get; set; }
        public ICollection<Product> Products{ get; set; }
    }
}
