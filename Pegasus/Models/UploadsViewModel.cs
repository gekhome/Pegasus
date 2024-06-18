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
    public class UploadGeneralModel
    {
        public int UploadID { get; set; }
        public int? ProkirixiID { get; set; }

        [Required(ErrorMessage = "Υποχρεωτική συμπλήρωση")]
        [Display(Name = "Αίτηση")]
        public int? AitisiID { get; set; }

        [Display(Name = "Εκπαιδευτικός")]
        public string TeacherAFM { get; set; }

        [Display(Name = "Σχολείο")]
        public int? SchoolID { get; set; }

        [Required(ErrorMessage = "Υποχρεωτική συμπλήρωση")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        [Display(Name = "Ημερομηνία")]
        public DateTime? UploadDate { get; set; }

        [Required(ErrorMessage = "Υποχρεωτική συμπλήρωση")]
        [StringLength(255, ErrorMessage = "Πρέπει να είναι μέχρι 255 χαρακτήρες.")]
        [Display(Name = "Περιγραφή αρχείων")]
        public string UploadSummary { get; set; }

    }

    public class UploadGeneralFilesModel
    {
        public int FileID { get; set; }

        [Display(Name = "Όνομα αρχείου")]
        public string FileName { get; set; }

        [Display(Name = "Κατηγορία")]
        public string Category { get; set; }

        [Display(Name = "Φάκελος (ΑΦΜ)")]
        public string TeacherAFM { get; set; }

        public Nullable<int> UploadID { get; set; }

        public virtual UploadGeneral UploadGeneral { get; set; }
    }

    public class UploadTeachingModel
    {
        public int UploadID { get; set; }
        public int? ProkirixiID { get; set; }

        [Required(ErrorMessage = "Υποχρεωτική συμπλήρωση")]
        [Display(Name = "Αίτηση")]
        public int? AitisiID { get; set; }

        [Display(Name = "Εκπαιδευτικός")]
        public string TeacherAFM { get; set; }

        [Display(Name = "Σχολείο")]
        public int? SchoolID { get; set; }

        [Required(ErrorMessage = "Υποχρεωτική συμπλήρωση")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        [Display(Name = "Ημερομηνία")]
        public DateTime? UploadDate { get; set; }

        [Required(ErrorMessage = "Υποχρεωτική συμπλήρωση")]
        [StringLength(255, ErrorMessage = "Πρέπει να είναι μέχρι 255 χαρακτήρες.")]
        [Display(Name = "Περιγραφή αρχείων")]
        public string UploadSummary { get; set; }

    }

    public class UploadTeachingFilesModel
    {
        public int FileID { get; set; }

        [Display(Name = "Όνομα αρχείου")]
        public string FileName { get; set; }

        [Display(Name = "Κατηγορία")]
        public string Category { get; set; }

        [Display(Name = "Φάκελος (ΑΦΜ)")]
        public string TeacherAFM { get; set; }

        public Nullable<int> UploadID { get; set; }

        public virtual UploadTeaching UploadTeaching { get; set; }
    }

    public class UploadVocationModel
    {
        public int UploadID { get; set; }
        public int? ProkirixiID { get; set; }

        [Required(ErrorMessage = "Υποχρεωτική συμπλήρωση")]
        [Display(Name = "Αίτηση")]
        public int? AitisiID { get; set; }

        [Display(Name = "Εκπαιδευτικός")]
        public string TeacherAFM { get; set; }

        [Display(Name = "Σχολείο")]
        public int? SchoolID { get; set; }

        [Required(ErrorMessage = "Υποχρεωτική συμπλήρωση")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        [Display(Name = "Ημερομηνία")]
        public DateTime? UploadDate { get; set; }

        [Required(ErrorMessage = "Υποχρεωτική συμπλήρωση")]
        [StringLength(255, ErrorMessage = "Πρέπει να είναι μέχρι 255 χαρακτήρες.")]
        [Display(Name = "Περιγραφή αρχείων")]
        public string UploadSummary { get; set; }

    }

    public class UploadVocationFilesModel
    {
        public int FileID { get; set; }

        [Display(Name = "Όνομα αρχείου")]
        public string FileName { get; set; }

        [Display(Name = "Κατηγορία")]
        public string Category { get; set; }

        [Display(Name = "Φάκελος (ΑΦΜ)")]
        public string TeacherAFM { get; set; }

        public Nullable<int> UploadID { get; set; }

        public virtual UploadVocation UploadVocation { get; set; }
    }

    //--- VIEW MODELS OF ULOADED FILES ---//
    public class xUploadedGeneralFilesModel
    {
        public int FileID { get; set; }

        [Display(Name = "Όνομα αρχείου")]
        public string FileName { get; set; }

        [Display(Name = "Κατηγορία")]
        public string Category { get; set; }

        public int? ProkirixiID { get; set; }

        public int? AitisiID { get; set; }

        public int? SchoolID { get; set; }

        [Display(Name = "Περιγραφή")]
        public string UploadSummary { get; set; }

        [Display(Name = "Φάκελος (ΑΦΜ)")]
        public string TeacherAFM { get; set; }

    }

    public class xUploadedTeachingFilesModel
    {
        public int FileID { get; set; }

        [Display(Name = "Όνομα αρχείου")]
        public string FileName { get; set; }

        [Display(Name = "Κατηγορία")]
        public string Category { get; set; }

        public int? ProkirixiID { get; set; }

        public int? AitisiID { get; set; }

        public int? SchoolID { get; set; }

        [Display(Name = "Περιγραφή")]
        public string UploadSummary { get; set; }

        [Display(Name = "Φάκελος (ΑΦΜ)")]
        public string TeacherAFM { get; set; }

    }

    public class xUploadedVocationFilesModel
    {
        public int FileID { get; set; }

        [Display(Name = "Όνομα αρχείου")]
        public string FileName { get; set; }

        [Display(Name = "Κατηγορία")]
        public string Category { get; set; }

        public int? ProkirixiID { get; set; }

        public int? AitisiID { get; set; }

        public int? SchoolID { get; set; }

        [Display(Name = "Περιγραφή")]
        public string UploadSummary { get; set; }

        [Display(Name = "Φάκελος (ΑΦΜ)")]
        public string TeacherAFM { get; set; }

    }

}