using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Model
{
    public class DiaSize
    {
        //public DiaSize()
        //{
        //    Products = new HashSet<Product>();
        //}
        [Key]
        public int DiaSizeID { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public virtual SisterConcern SisterConcern { get; set; }
        public int ConcernID { get; set; }
    }
}
