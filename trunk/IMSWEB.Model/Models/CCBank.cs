using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IMSWEB.Model
{
    public partial class CCBank
    {

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public CCBank()
        {
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CCBankID { get; set; }

        [Required]
        [StringLength(250)]
        public string CCBankName { get; set; }

        [Required]
        [StringLength(250)]
        public string CCBankCode { get; set; }

    }
}
