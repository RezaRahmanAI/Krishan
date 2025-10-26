using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Model
{
    [Table("DocumentsInfo")]
    public class DocumentsInfo
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(150)]
        public string DocType { get; set; }
        [Required]
        [StringLength(250)]
        public string DocName { get; set; }
        [Required]
        public string DocPath { get; set; }
        public int DocSourceId { get; set; }
        public int ConcernID { get; set; }
        [Required]
        [StringLength(50)]
        public string SourceFolder { get; set; }
        [ForeignKey("ConcernID")]
        public virtual SisterConcern SisterConcern { get; set; }

    }
}
