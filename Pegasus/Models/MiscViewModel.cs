using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using Pegasus.Models;
using Pegasus.DAL;
using System.Web.Mvc;

namespace Pegasus.Models
{
    public class AitiseisWithWorkViewModel
    {
        public int AITISI_ID { get; set; }

        [Display(Name = "Προκήρυξη")]
        public Nullable<int> PROKIRIXI_ID { get; set; }

        [Display(Name = "Προκήρυξη")]
        public string PROTOCOL { get; set; }

        [Display(Name = "Αρ. Πρωτ.")]
        public string AITISI_PROTOCOL { get; set; }

        [Display(Name = "Α.Φ.Μ.")]
        public string AFM { get; set; }

        [Display(Name = "Ηλικία")]
        public Nullable<int> AGE { get; set; }

        [Display(Name = "Ονοματεπώνυμο")]
        public string FULLNAME { get; set; }

        [Display(Name = "Σχολική Μονάδα")]
        public string SCHOOL_NAME { get; set; }

        [Display(Name = "Σχολική Μονάδα")]
        public int SCHOOL_ID { get; set; }

        [Display(Name = "Κλάδος-Ειδικότητα")]
        public string EIDIKOTITA_DESC { get; set; }

        [Display(Name = "Κλάδος")]
        public string KLADOS_NAME { get; set; }

        public Nullable<int> EIDIKOTITA_KLADOS_ID { get; set; }
        public int EIDIKOTITA_ID { get; set; }

    }

    public class AitiseisWithoutWorkViewModel
    {
        public int AITISI_ID { get; set; }

        [Display(Name = "Προκήρυξη")]
        public Nullable<int> PROKIRIXI_ID { get; set; }

        [Display(Name = "Προκήρυξη")]
        public string PROTOCOL { get; set; }

        [Display(Name = "Αρ. Πρωτ.")]
        public string AITISI_PROTOCOL { get; set; }

        [Display(Name = "Α.Φ.Μ.")]
        public string AFM { get; set; }

        [Display(Name = "Ηλικία")]
        public Nullable<int> AGE { get; set; }

        [Display(Name = "Ονοματεπώνυμο")]
        public string FULLNAME { get; set; }

        [Display(Name = "Σχολική Μονάδα")]
        public string SCHOOL_NAME { get; set; }

        [Display(Name = "Σχολική Μονάδα")]
        public int SCHOOL_ID { get; set; }

        [Display(Name = "Κλάδος-Ειδικότητα")]
        public string EIDIKOTITA_DESC { get; set; }

        [Display(Name = "Κλάδος")]
        public string KLADOS_NAME { get; set; }

        public Nullable<int> EIDIKOTITA_KLADOS_ID { get; set; }
        public int EIDIKOTITA_ID { get; set; }

    }

    public class EidikotitesInSchoolsViewModel
    {
        public int PSE_ID { get; set; }
        public int PROKIRIXI_ID { get; set; }
        public int SCHOOL_ID { get; set; }
        public int EIDIKOTITA_ID { get; set; }

        [Display(Name = "Σχολική Μονάδα")]
        public string SCHOOL_NAME { get; set; }

        [Display(Name = "Περιφερειακή Ενότητα")]
        public string PERIFERIA_NAME { get; set; }

        [Display(Name = "Περιφερειακή Διεύθυνση")]
        public string PERIFERIAKI { get; set; }

    }

    public class sqlTeacherAitiseisViewModel
    {

        [Display(Name = "Προκήρυξη")]
        public string AITISI_PROTOCOL { get; set; }

        [Display(Name = "Περιφέρεια αίτησης")]
        public string PERIFERIA_NAME { get; set; }

        [Display(Name = "Α.Φ.Μ.")]
        public string AFM { get; set; }

        [Display(Name = "Ονοματεπώνυμο")]
        public string FULLNAME { get; set; }

        [Display(Name = "Κλάδος-Ειδικότητα")]
        public string EIDIKOTITA_TEXT { get; set; }

        [Display(Name = "ΙΕΚ αίτησης")]
        public string SCHOOL_NAME { get; set; }

        [Display(Name = "Περιφερειακή")]
        public string PERIFERIAKI { get; set; }

        public int? PROKIRIXI_ID { get; set; }

        public int AITISI_ID { get; set; }

        public int? PERIFERIA_ID { get; set; }

        public int? SCHOOL_ID { get; set; }

        public bool? CHECK_STATUS { get; set; }

        public bool? ENSTASI { get; set; }

        public string PROTOCOL { get; set; }

        public DateTime? DATE_START { get; set; }

        public int PERIFERIAKI_ID { get; set; }
    }

}