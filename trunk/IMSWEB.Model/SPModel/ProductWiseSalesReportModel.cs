using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Model
{
    public class ProductWiseSalesReportModel
    {
        public string ConcernName { get; set; }
        public int SOrderID { get; set; }
        public DateTime Date { get; set; }
        public string InvoiceNo { get; set; }
        public string EmployeeCode { get; set; }
        public string EmployeeName { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string Address { get; set; }
        public string Mobile { get; set; }

        //Raihan Vai start here

        public int IsReplacement { get; set; }
        public int Status { get; set; }

        public decimal TotalDue { get; set; }

        //Raihan Vai end here
        public int ProductID { get; set; }
        public int CategoryID { get; set; }
        public int CompanyID { get; set; }

        public string ProductName { get; set; }
        public decimal Quantity { get; set; }
        public decimal SalesRate { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal Adjustment { get; set; }
        public string CompanyName { get; set; }
        public string CategoryName { get; set; }
        public string IMEI { get; set; }


        public decimal GrandTotal { get; set; }

        public decimal NetDiscount { get; set; }

        public decimal RecAmount { get; set; }

        public decimal PaymentDue { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal UTAmount { get; set; }

        public decimal PPDAmount { get; set; }

        public decimal PPOffer { get; set; }

        public string ColorName { get; set; }

        public decimal AdjAmount { get; set; }

        public string UnitName { get; set; }

        public string ProductCode { get; set; }

        public string IDCode { get; set; }

        public string SizeName { get; set; }

        public decimal ConvertValue { get; set; }

        public decimal SalesPerCartonSft { get; set; }
        public decimal ExtraAmt { get; set; }
        public decimal ExtraSFT { get; set; }
        public decimal PurchaseRate { get; set; }
        public decimal PurchaseSFTRate { get; set; }
        public decimal PurchaseTotal { get; set; }
        public decimal NetBenefit { get; set; }
        public decimal Totalbenefit { get; set; }



        public string ChildUnitName { get; set; }

        public decimal SFTRate { get; set; }

        public decimal TotalSFT { get; set; }

        public decimal SalesCSft { get; set; }
        public bool IsAdvanceSale { get; set; }
        public bool IsApproved { get; set; }

    }
}
