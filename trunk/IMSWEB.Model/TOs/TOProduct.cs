using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Model.TOs
{
    public class TOProduct
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string ProductName { get; set; }
        public string CategoryID { get; set; }
        public string CompnayID { get; set; }
        public decimal SizeID { get; set; }
    }
}
