using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Pegasus.DAL;

namespace Pegasus.Models
{
    public class TeacherViewModel
    {
        [Display(Name = "ΑΦΜ")]
        public string AFM { get; set; }

        [Required(ErrorMessage = "Υποχρεωτική συμπλήρωση ΑΔΤ")]
        [StringLength(20, ErrorMessage = "Πρέπει να είναι μέχρι 20 χαρακτήρες.")]
        [RegularExpression(@"^[Α-ΩA-Z0-9]+[ Α-ΩA-Z0-9-]*$", ErrorMessage = "Μόνο κεφαλαία ελληνικά, λατινικά, αριθμοί")]
        [Display(Name = "ΑΔΤ *")]
        public string ADT { get; set; }

        [Required(ErrorMessage = "Υποχρεωτική συμπλήρωση ΑΜΚΑ")]
        [StringLength(20, ErrorMessage = "Πρέπει να είναι μέχρι 20 χαρακτήρες.")]
        [RegularExpression(@"^[0-9]+[ 0-9]*$", ErrorMessage = "Μόνο αριθμοί και κενά")]
        [Display(Name = "ΑΜΚΑ *")]
        public string AMKA { get; set; }

        [Required(ErrorMessage = "Υποχρεωτική συμπλήρωση ονόματος")]
        [StringLength(50, ErrorMessage = "Πρέπει να είναι μέχρι 50 χαρακτήρες.")]
        [RegularExpression(@"^[Α-Ω]+[ Α-Ω-_ΪΫ]*$", ErrorMessage = "Μόνο κεφαλαία ελληνικά")]
        [Display(Name = "Όνομα *")]
        public string FIRSTNAME { get; set; }

        [Required(ErrorMessage = "Υποχρεωτική συμπλήρωση επώνυμου")]
        [StringLength(50, ErrorMessage = "Πρέπει να είναι μέχρι 50 χαρακτήρες.")]
        [RegularExpression(@"^[Α-Ω]+[ Α-Ω-_ΪΫ]*$", ErrorMessage = "Μόνο κεφαλαία ελληνικά")]
        [Display(Name = "Επώνυμο *")]
        public string LASTNAME { get; set; }

        [Required(ErrorMessage = "Υποχρεωτική συμπλήρωση πατρώνυμου")]
        [StringLength(50, ErrorMessage = "Πρέπει να είναι μέχρι 50 χαρακτήρες.")]
        [RegularExpression(@"^[Α-Ω]+[ Α-Ω-_ΪΫ]*$", ErrorMessage = "Μόνο κεφαλαία ελληνικά")]
        [Display(Name = "Πατρώνυμο *")]
        public string FATHERNAME { get; set; }

        [Required(ErrorMessage = "Υποχρεωτική συμπλήρωση μητρώνυμου")]
        [StringLength(50, ErrorMessage = "Πρέπει να είναι μέχρι 50 χαρακτήρες.")]
        [RegularExpression(@"^[Α-Ω]+[ Α-Ω-_ΪΫ]*$", ErrorMessage = "Μόνο κεφαλαία ελληνικά")]
        [Display(Name = "Μητρώνυμο *")]
        public string MOTHERNAME { get; set; }

        [Required(ErrorMessage = "Υποχρεωτική συμπλήρωση φύλου")]
        [Display(Name = "Φύλο *")]
        public Nullable<int> GENDER { get; set; }

        [Required(ErrorMessage = "Υποχρεωτική συμπλήρωση ημερομηνίας γέννησης")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        [Display(Name = "Ημερομηνία γέννησης *")]
        public Nullable<System.DateTime> BIRTHDATE { get; set; }

        [Required(ErrorMessage = "Υποχρεωτική συμπλήρωση οικογενειακής κατάστασης")]
        [Display(Name = "Οικογενειακή κατάσταση *")]
        public string MARITAL_STATUS { get; set; }

        [Required(ErrorMessage = "Υποχρεωτική συμπλήρωση στρατιωτικών υποχρεώσεων")]
        [Display(Name = "Στρατολογικές υποχρεώσεις *")]
        public Nullable<int> ARMY_STATUS { get; set; }

        [Required(ErrorMessage = "Υποχρεωτική συμπλήρωση σταθερού τηλ.")]
        [StringLength(50, ErrorMessage = "Πρέπει να είναι μέχρι 50 χαρακτήρες.")]
        [RegularExpression(@"^[0-9]+[ 0-9-]*$", ErrorMessage = "Μόνο αριθμοί, κενό και παύλες")]
        [Display(Name = "Σταθερό τηλέφωνο *")]
        public string TELEPHONE { get; set; }

        [Required(ErrorMessage = "Υποχρεωτική συμπλήρωση κινητού τηλ.")]
        [StringLength(50, ErrorMessage = "Πρέπει να είναι μέχρι 50 χαρακτήρες.")]
        [RegularExpression(@"^[0-9]+[ 0-9-]*$", ErrorMessage = "Μόνο αριθμοί, κενά και παύλες")]
        [Display(Name = "Κινητό τηλέφωνο *")]
        public string MOBILE { get; set; }

        [Required(ErrorMessage = "Υποχρεωτική συμπλήρωση E-Mail")]
        [StringLength(50, ErrorMessage = "Πρέπει να είναι μέχρι 50 χαρακτήρες.")]
        [DataType(DataType.EmailAddress, ErrorMessage="Δεν είναι έγκυρη μορφή E-mail")]
        [Display(Name = "E-mail *")]
        public string EMAIL { get; set; }

        [Required(ErrorMessage = "Υποχρεωτική συμπλήρωση διεύθυνσης")]
        [StringLength(100, ErrorMessage = "Πρέπει να είναι μέχρι 100 χαρακτήρες.")]
        [RegularExpression(@"^[ΆΈΊΉΌΎΏΑ-Ω0-9']+[ ΆΈΊΉΌΎΏΑ-Ω0-9-.,'ΪΫ]*$", ErrorMessage = "Μόνο κεφαλαία ελληνικά, αριθμοί, κενά, παύλες")]
        [Display(Name = "Ταχυδρομική διέυθυνση *")]
        public string ADDRESS { get; set; }

        [Required(ErrorMessage = "Υποχρεωτική συμπλήρωση πόλης")]
        [StringLength(50, ErrorMessage = "Πρέπει να είναι μέχρι 50 χαρακτήρες.")]
        [RegularExpression(@"^[Α-Ω]+[ Α-Ω.'ΪΫ]*$", ErrorMessage = "Μόνο κεφαλαία ελληνικά")]
        [Display(Name = "Πόλη *")]
        public string CITY { get; set; }

        [Required(ErrorMessage = "Υποχρεωτική συμπλήρωση Τ.Κ.")]
        [StringLength(20, ErrorMessage = "Πρέπει να είναι μέχρι 20 χαρακτήρες.")]
        [RegularExpression(@"^[0-9]+[ 0-9-]*$", ErrorMessage = "Μόνο αριθμοί, κενά και παύλες")]
        [Display(Name = "Τ.Κ. *")]
        public string TK { get; set; }

        [Required(ErrorMessage = "Υποχρεωτική συμπλήρωση περιφέρειας")]
        [Display(Name = "Περιφέρεια *")]
        public Nullable<int> PERIFERIA { get; set; }

        [Required(ErrorMessage = "Υποχρεωτική συμπλήρωση δήμου")]
        [RegularExpression(@"^[ΆΈΊΉΌΎΏΑ-Ω']+[- ΆΈΊΉΌΎΏΑ-Ω.'ΪΫ]*$", ErrorMessage = "Μόνο κεφαλαία ελληνικά")]
        [Display(Name = "Δήμος *")]
        public string DIMOS { get; set; }

        [Display(Name = "Παρατηρήσεις")]
        [DataType(DataType.MultilineText)]
        public string COMMENT { get; set; }

        [Required(ErrorMessage = "Υποχρεωτική συμπλήρωση ΔΟΥ")]
        [StringLength(50, ErrorMessage = "Πρέπει να είναι μέχρι 50 χαρακτήρες.")]
        [RegularExpression(@"^[ΆΈΊΉΌΎΏΑ-Ω']+[ ΆΈΊΉΌΎΏΑ-Ω.'ΪΫ]*$", ErrorMessage = "Μόνο κεφαλαία ελληνικά")]
        [Display(Name = "ΔΟΥ *")]
        public string DOY { get; set; }

        public TeacherViewModel() { }

        public TeacherViewModel(TEACHERS teacherVM)
        {
            this.LASTNAME = teacherVM.LASTNAME;
            this.FIRSTNAME = teacherVM.FIRSTNAME;
            this.FATHERNAME = teacherVM.FATHERNAME;
            this.MOTHERNAME = teacherVM.MOTHERNAME;
            this.AFM = teacherVM.AFM;
            this.DOY = teacherVM.DOY;
            this.ADT = teacherVM.ADT;
            this.AMKA = teacherVM.AMKA;
            this.GENDER = teacherVM.GENDER;
            this.BIRTHDATE = teacherVM.BIRTHDATE;
            this.ARMY_STATUS = teacherVM.ARMY_STATUS;
            this.MARITAL_STATUS = teacherVM.MARITAL_STATUS;
            this.ADDRESS = teacherVM.ADDRESS;
            this.CITY = teacherVM.CITY;
            this.TK = teacherVM.TK;
            this.PERIFERIA = teacherVM.PERIFERIA;
            this.DIMOS = teacherVM.DIMOS;
            this.TELEPHONE = teacherVM.TELEPHONE;
            this.MOBILE = teacherVM.MOBILE;
            this.EMAIL = teacherVM.EMAIL;
            this.COMMENT = teacherVM.COMMENT;
        }
    }
}