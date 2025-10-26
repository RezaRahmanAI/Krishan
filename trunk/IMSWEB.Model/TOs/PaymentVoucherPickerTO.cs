using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Model.TOs
{
    public class PaymentVoucherPickerTO
    {
        public int ExpenseItemID { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public int IsSupplier { get; set; }
        public int IsCustomer { get; set; }
        public int IsInvestment { get; set; }
        public string HeadType { get; set; }
        public string Name { get; set; }
    }
}
