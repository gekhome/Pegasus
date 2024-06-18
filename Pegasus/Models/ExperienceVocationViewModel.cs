using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Pegasus.Models
{
    public class ExperienceVocationViewModel
    {
        public int AITISI_ID { get; set; }
        public string KLADOS_NAME { get; set; }
        public Nullable<int> KLADOS { get; set; }
        [Display(Name = "Μόρια Επαγγελματικής")]
        public Nullable<double> MSUM { get; set; }
    }
}