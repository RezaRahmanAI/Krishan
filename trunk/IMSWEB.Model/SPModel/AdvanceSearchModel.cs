using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Model
{
    public class AdvanceSearchModel
    {
        public AdvanceSearchModel()
        {
            AdvancePODetails = new List<AdvancePODetail>();
            AdvanceSOrderDetails = new List<AdvanceSOrderDetail>();
        }
        public DateTime PurchaseDate { get; set; }
        public string SupplierCode { get; set; }
        public string SupplierName { get; set; }
        public string ChallanNo { get; set; }

        public DateTime SalesDate { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string InvoiceNo { get; set; }

        public List<AdvancePODetail> AdvancePODetails { get; set; }
        public List<AdvanceSOrderDetail> AdvanceSOrderDetails { get; set; }
    }

    public class AdvancePODetail
    {
        public int ID { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string IMEI { get; set; }
        public decimal PurchaseRate { get; set; }
        public decimal Quantity { get; set; }
        public string CategoryName { get; set; }
    }

    public class AdvanceSOrderDetail
    {
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string IMEI { get; set; }
        public decimal SalesRate { get; set; }
        public decimal Quantity { get; set; }
        public int Status { get; set; }

    }
}
