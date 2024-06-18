using Pegasus.DAL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Pegasus.Models
{
    public class TeacherInAitiseisViewModel
    {
        [Display(Name = "Α.Φ.Μ.")]
        public string AFM { get; set; }

        [Display(Name = "Επώνυμο, Όνομα")]
        public string FULLNAME { get; set; }

        [Display(Name = "Τηλ. Σταθερό")]
        public string TELEPHONE { get; set; }

        [Display(Name = "Τηλ. Κινητό")]
        public string MOBILE { get; set; }

        [Display(Name = "Διεύθυνση Email")]
        public string EMAIL { get; set; }

        [Display(Name = "Ηλικία")]
        public Nullable<int> AGE { get; set; }

        [Display(Name = "Τέκνα")]
        public Nullable<int> CHILDREN { get; set; }

        [Display(Name = "Κλάδος-Ειδικότητα")]
        public string EIDIKOTITA_TEXT { get; set; }

        [Display(Name = "Σχολείο Αίτησης")]
        public string SCHOOL_NAME { get; set; }

        [Display(Name = "Αρ.Πρωτ. Αίτησης")]
        public string AITISI_PROTOCOL { get; set; }

        public Nullable<int> SCHOOL_ID { get; set; }
        public int EIDIKOTITA_ID { get; set; }
        public Nullable<int> PROKIRIXI_ID { get; set; }
        public int AITISI_ID { get; set; }


    }
}