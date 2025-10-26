using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Model.TOs
{
    public class ConcernSwitchTO
    {
        [Display(Name = "Concern")]
        public int ConcernId { get; set; }
    }
}
