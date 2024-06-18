using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Pegasus.Models
{
    public class ExperienceTeachingViewModel
    {
        [Display(Name = "Τύπος Διδακτικής")]
        public string TYPE_TEXT { get; set; }
        public Nullable<int> TEACH_TYPE { get; set; }
        public Nullable<int> KLADOS { get; set; }
        [Display(Name = "Μόρια Διδακτικής")]
        public Nullable<double> MORIA_TOTAL { get; set; }
    }
}