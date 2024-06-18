using Pegasus.DAL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Pegasus.Models
{
    public class AitiseisTeachersViewModel
    {

        [Display(Name = "Περιφέρεια")]
        public string PERIFERIA_NAME { get; set; }

        [Display(Name = "ΑΦΜ")]
        public string AFM { get; set; }

        [Display(Name = "Ονοματεπώνυμο")]
        public string FULLNAME { get; set; }

        [Display(Name = "Κλάδος-Ειδικότητα")]
        public string EIDIKOTITA_TEXT { get; set; }

        [Display(Name = "Προκήρυξη #")]
        public Nullable<int> PROKIRIXI_ID { get; set; }

        [Display(Name = "Αίτηση #")]
        public int AITISI_ID { get; set; }

        [Display(Name = "Περιφέρεια #")]
        public Nullable<int> PERIFERIA_ID { get; set; }

        [Display(Name = "Σχολείο #")]
        public Nullable<int> SCHOOL_ID { get; set; }

        [Display(Name = "Σχολική μονάδα")]
        public string SCHOOL_NAME { get; set; }

        [Display(Name = "Έλεγχος")]
        public Nullable<bool> CHECK_STATUS { get; set; }

        [Display(Name = "Ένσταση")]
        public Nullable<bool> ENSTASI { get; set; }

        [Display(Name = "Προκήρυξη")]
        public string PROTOCOL { get; set; }
    }
}