using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Pegasus.DAL;

namespace Pegasus.Models
{
    public class ViewModelVocational
    {
        #region Teacher

        [Display(Name = "ΑΦΜ")]
        public string AFM { get; set; }

        [Display(Name = "Ονοματεπώνυμο")]
        public string FULLNAME { get; set; }

        #endregion

        #region Aitisi

        public int AITISI_ID { get; set; }

        [Display(Name = "Αρ. Πρωτοκόλλου")]
        public string AITISI_PROTOCOL { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Ημερομηνία Αίτησης")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public Nullable<System.DateTime> AITISI_DATE { get; set; }

        [Display(Name = "Βαθμίδα")]
        public Nullable<int> KLADOS { get; set; }

        [Display(Name = "Κλάδος-Ειδικότητα")]
        public string EIDIKOTITA_TEXT { get; set; }

        #endregion

        #region Vocational

        public int EXP_ID { get; set; }

        [Required(ErrorMessage = "Υποχρεωτική συμπλήρωση")]
        [DataType(DataType.Date)]        
        [Display(Name = "Ημ/νία από")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public Nullable<System.DateTime> DATE_FROM { get; set; }

        [Required(ErrorMessage = "Υποχρεωτική συμπλήρωση")]
        [DataType(DataType.Date)]        
        [Display(Name = "Ημ/νία έως")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public Nullable<System.DateTime> DATE_TO { get; set; }

        [Display(Name = "Ημέρες*")]        
        public Nullable<float> DAYS_AUTO { get; set; }

        [Display(Name = "Ημέρες")]                
        public Nullable<float> DAYS_MANUAL { get; set; }

        [Display(Name = "Μόρια")]
        [DisplayFormat(DataFormatString = "{0:0.000}")]
        public Nullable<float> MORIA { get; set; }

        [StringLength(50, ErrorMessage = "Πρέπει να είναι μέχρι 50 χαρακτήρες.")]
        [RegularExpression(@"^[Α-Ω0-9-_\.\/]+[ Α-Ω0-9-_\.\/]*$", ErrorMessage = "Μόνο κεφαλαία ελληνικά, αριθμοί και διαχωριστικά")]
        [Display(Name = "Α.Π. Εγγράφου")]
        public string DOC_PROTOCOL { get; set; }

        [StringLength(50, ErrorMessage = "Πρέπει να είναι μέχρι 50 χαρακτήρες.")]
        [RegularExpression(@"^[Α-Ω-_\.\/]+[ Α-Ω-_\.\/]*$", ErrorMessage = "Μόνο κεφαλαία ελληνικά, αριθμοί και διαχωριστικά")]
        [Display(Name = "Εκδούσα Υπηρεσία")]
        public string DOC_ORIGIN { get; set; }

        [Display(Name = "Παρατηρήσεις")]
        public string DOC_COMMENT { get; set; }

        //[Required(ErrorMessage = "Υποχρεωτική συμπλήρωση")]
        [Display(Name = "Για προσμέτρηση")]
        public bool DOC_VALID { get; set; }

        [Display(Name = "Μήνυμα Σφάλματος")]
        public string ERROR_TEXT { get; set; }

        [Display(Name = "Διπλότυπη")]
        public bool? DUPLICATE { get; set; }

        //public int? PROKIRIXI_ID { get; set; }

        public virtual AITISIS AITISIS { get; set; }


        #endregion

        // Constructors
        public ViewModelVocational() { }

        public ViewModelVocational(EXP_VOCATIONAL e)
        {
            this.EXP_ID = e.EXP_ID;
            this.AITISI_ID = e.AITISI_ID;
            this.DATE_FROM = e.DATE_FROM;
            this.DATE_TO = e.DATE_TO;
            this.DAYS_AUTO = e.DAYS_AUTO;
            this.DAYS_MANUAL = e.DAYS_MANUAL;
            this.MORIA = e.MORIA;
            this.DOC_PROTOCOL = e.DOC_PROTOCOL;
            this.DOC_ORIGIN = e.DOC_ORIGIN;
            this.DOC_VALID = e.DOC_VALID ?? false;
            this.DOC_COMMENT = e.DOC_COMMENT;
            this.ERROR_TEXT = e.ERROR_TEXT;
            this.DUPLICATE = e.DUPLICATE;
            this.KLADOS = e.AITISIS.KLADOS;
        }

    }

}