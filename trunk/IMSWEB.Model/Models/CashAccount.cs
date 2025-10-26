using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Model
{
    [Table("CashAccounts")]
    public class CashAccount : AuditTrailModel
    {
        [Key]
        public int Id { get; set; }
        [StringLength(250)]
        public string Name { get; set; }
        public decimal OpeningBalance { get; set; }
        public DateTime OpeningDate { get; set; }
        public decimal TotalBalance { get; set; }
        public int ConcernId { get; set; }
        [ForeignKey("ConcernId")]
        public SisterConcern SisterConcern { get; set; }

    }
}
