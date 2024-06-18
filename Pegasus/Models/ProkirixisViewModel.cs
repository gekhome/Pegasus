using Pegasus.DAL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Pegasus.Models
{
    public class ProkirixisViewModel
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "Υποχρεωτική συμπλήρωση")]
        [Display(Name = "Πρωτόκολλο")]
        [StringLength(50, ErrorMessage = "Πρέπει να είναι μέχρι 50 χαρακτήρες.")]
        public string PROTOCOL { get; set; }

        [Required(ErrorMessage = "Υποχρεωτική συμπλήρωση")]
        [Display(Name = "Φ.Ε.Κ.")]
        [StringLength(50, ErrorMessage = "Πρέπει να είναι μέχρι 50 χαρακτήρες.")]
        public string FEK { get; set; }

        //[Required(ErrorMessage = "Υποχρεωτική συμπλήρωση")]
        [Display(Name = "Φορέας")]
        [StringLength(50, ErrorMessage = "Πρέπει να είναι μέχρι 50 χαρακτήρες.")]
        public string FOREAS { get; set; }

        [Required(ErrorMessage = "Υποχρεωτική συμπλήρωση")]
        [Display(Name = "Σχολ. Έτος")]
        public Nullable<int> SCHOOL_YEAR { get; set; }

        [Required(ErrorMessage = "Υποχρεωτική συμπλήρωση")]
        [Display(Name = "Ημ. Έναρξης")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        [DataType(DataType.Date)]
        public Nullable<System.DateTime> DATE_START { get; set; }

        [Required(ErrorMessage = "Υποχρεωτική συμπλήρωση")]
        [Display(Name = "Ημ. Λήξης")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        [DataType(DataType.Date)]
        public Nullable<System.DateTime> DATE_END { get; set; }

        [Required(ErrorMessage = "Υποχρεωτική συμπλήρωση")]
        [Display(Name = "Από")]
        public string HOUR_START { get; set; }

        [Required(ErrorMessage = "Υποχρεωτική συμπλήρωση")]
        [Display(Name = "Έως")]
        public string HOUR_END { get; set; }

        //[Required(ErrorMessage = "Υποχρεωτική συμπλήρωση")]
        [Display(Name = "Διοικητής")]
        [StringLength(100, ErrorMessage = "Πρέπει να είναι μέχρι 100 χαρακτήρες.")]
        public string DIOIKITIS { get; set; }

        [Required(ErrorMessage = "Υποχρεωτική συμπλήρωση")]
        [Display(Name = "Καθεστώς")]
        public Nullable<short> STATUS { get; set; }

        [Required(ErrorMessage = "Υποχρεωτική συμπλήρωση")]
        [Display(Name = "Ενεργή")]
        public bool ACTIVE { get; set; }

        [Required(ErrorMessage = "Υποχρεωτική συμπλήρωση")]
        [Display(Name = "Διαχείρ.")]
        public bool ADMIN { get; set; }

        [Required(ErrorMessage = "Υποχρεωτική συμπλήρωση")]
        [Display(Name = "Προβολή")]
        public bool USER_VIEW { get; set; }

        [Display(Name = "Ενστάσεις")]
        public bool ENSTASEIS { get; set; }

    }
}