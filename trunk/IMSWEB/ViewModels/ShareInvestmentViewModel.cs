using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using IMSWEB.Model;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace IMSWEB
{
    public class ShareInvestmentViewModel
    {
        public int SIID { get; set; }

        [Display(Name = "Entry Date")]
        public DateTime EntryDate { get; set; }

        [Required(ErrorMessage = "Purpose is required.")]
        public string Purpose { get; set; }

        [Display(Name = "Head Name")]
        public string HeadName { get; set; }

        [Display(Name = "Head")]
        [Required(ErrorMessage = "Head is required.")]
        public int SIHID { get; set; }
        public int ParentID { get; set; }
        public decimal Amount { get; set; }
        public EnumInvestTransType TransactionType { get; set; }

        [Display(Name = "Type")]
        public EnumLiabilityPayType LiabilityPayType { get; set; }

        [Display(Name = "Type")]
        public EnumLiabilityRecType LiabilityRecType { get; set; }
        public List<ShareInvestmentHead> Heads { get; set; }

        public decimal Balance { get; set; }
        public bool CashInHandReportStatus { get; set; }
        public List<SelectListItem> PayItems { get; set; }
        [Display(Name = "Payment")]
        public string PayHeadId { get; set; }
        public int? PayCashAccountId { get; set; }
        public int? PayBankId { get; set; }
    }
}