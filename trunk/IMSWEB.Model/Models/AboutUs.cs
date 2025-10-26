using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IMSWEB.Model
{
    public class AboutUS : AuditTrailModel
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; }

        public string Description { get; set; }

        public string DocumentPath { get; set; }

        public int ConcernID { get; set; }

        [ForeignKey("ConcernID")]
        public virtual SisterConcern SisterConcern { get; set; }  


    }
}
