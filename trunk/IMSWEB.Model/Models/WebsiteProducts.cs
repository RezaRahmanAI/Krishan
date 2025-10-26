using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IMSWEB.Model
{
    public class WebsiteProducts : AuditTrailModel
    {
        [Key]
        public int Id { get; set; }
        public int ProcutCategory { get; set; }
        public string Title { get; set; }

        public string Description { get; set; }

        public string DocumentPath { get; set; }
        public Decimal Price { get; set; }

        public int ConcernID { get; set; }

        [ForeignKey("ConcernID")]
        public virtual SisterConcern SisterConcern { get; set; }  


    }
}
