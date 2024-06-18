﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Pegasus.DAL;

namespace Pegasus.Models
{
    public class ViewModelTeaching
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

        public virtual AITISIS AITISIS { get; set; }

        #endregion

        #region Teaching Experience

        public int EXP_ID { get; set; }

        [Required(ErrorMessage = "Υποχρεωτική συμπλήρωση")]
        [Display(Name = "Δομή Εκπαίδευσης")]
        public Nullable<int> TEACH_TYPE { get; set; }

        [Required(ErrorMessage = "Υποχρεωτική συμπλήρωση")]
        [Display(Name = "Σχολ. Έτος")]
        public Nullable<int> SCHOOL_YEAR { get; set; }

        [Required(ErrorMessage = "Υποχρεωτική συμπλήρωση")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]       
        [Display(Name = "Ημ/νία από")]
        public Nullable<System.DateTime> DATE_FROM { get; set; }

        [DataType(DataType.Date)]
        [Required(ErrorMessage = "Υποχρεωτική συμπλήρωση")]
        [Display(Name = "Ημ/νία έως")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public Nullable<System.DateTime> DATE_TO { get; set; }

        [Display(Name = "Ώρες/εβδ")]        
        public Nullable<int> HOURS_WEEK { get; set; }

        [Display(Name = "Σύνολο Ώρες")]
        public Nullable<int> HOURS { get; set; }

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

        [Display(Name = "Για προσμέτρηση")]                
        public bool DOC_VALID { get; set; }

        [Display(Name = "Μήνυμα Σφάλματος")]                
        public string ERROR_TEXT { get; set; }

        [Display(Name = "Διπλότυπη")]
        public bool? DUPLICATE { get; set; }

        //public int? PROKIRIXI_ID { get; set; }

        #endregion

        // Constructors

        public ViewModelTeaching() { }

        public ViewModelTeaching(EXP_TEACHING e)
        {
            this.EXP_ID = e.EXP_ID;
            this.AITISI_ID = e.AITISI_ID;
            this.TEACH_TYPE = e.TEACH_TYPE;
            this.SCHOOL_YEAR = e.SCHOOL_YEAR;
            this.DATE_FROM = e.DATE_FROM;
            this.DATE_TO = e.DATE_TO;
            this.HOURS_WEEK = e.HOURS_WEEK;
            this.HOURS = e.HOURS;
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