using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;


namespace Pegasus.Models
{
    public class UserTeacherViewModel
    {
        public int USER_ID { get; set; }

        [StringLength(20, ErrorMessage = "Πρέπει να είναι μέχρι 20 χαρακτήρες.")]
        [Display(Name = "Όνομα χρήστη (άκυρο)")]
        public string USERNAME { get; set; }

        [StringLength(20, ErrorMessage = "Πρέπει να είναι μέχρι 20 χαρακτήρες.")]
        [DataType(DataType.Password)]
        [Display(Name = "Κωδικός πρόσβασης (άκυρος)")]
        public string PASSWORD { get; set; }

        [StringLength(10, ErrorMessage = "Πρέπει να είναι μέχρι 10 χαρακτήρες.", MinimumLength = 9)]
        [Display(Name = "ΑΦΜ")]
        public string USER_AFM { get; set; }

        [Display(Name = "Ενεργός")]
        public bool? ISACTIVE { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        [Display(Name = "Ημ/νία εγγραφής")]
        public DateTime? CREATEDATE { get; set; }

        [StringLength(20, ErrorMessage = "Πρέπει να είναι μέχρι 20 χαρακτήρες.")]
        [DataType(DataType.Password)]
        [Compare("PASSWORD", ErrorMessage = "Ο κωδικός πρόσβασης και αυτός της επιβεβαίωσης δεν είναι ίδιοι.")]
        [Display(Name = "Επιβεβαίωση κωδικού")]
        public string ConfirmPassword { get; set; }

        [Display(Name = "ΑΜΚΑ")]
        public string AMKA { get; set; }

        [Display(Name = "Εισάγετε το κείμενο της εικόνας")]
        public string CAPTCHATEXT { get; set; }

    }

    public class NewUserTeacherViewModel
    {
        public int USER_ID { get; set; }

        [Required(ErrorMessage = "Υποχρεωτική συμπλήρωση ονόματος χρήστη")]
        [StringLength(20, ErrorMessage = "Πρέπει να είναι μέχρι 20 χαρακτήρες.")]
        [RegularExpression(@"^[.A-Za-z0-9_-]+$", ErrorMessage = "Μόνο λατινικοί χαρακτήρες, αριθμοί, τελείες, παύλες χωρίς κενά")]
        [Display(Name = "Όνομα χρήστη")]
        public string USERNAME { get; set; }

        [Required(ErrorMessage = "Υποχρεωτική συμπλήρωση κωδικού ασφαλείας")]
        [StringLength(20, ErrorMessage = "Πρέπει να είναι μέχρι 20 χαρακτήρες.")]
        [RegularExpression(@"^[\S]+$", ErrorMessage = "Δεν επιτρέπεται ο κενός χαρακτήρας")]
        [DataType(DataType.Password)]
        [Display(Name = "Κωδικός πρόσβασης")]
        public string PASSWORD { get; set; }

        [Required(ErrorMessage = "Υποχρεωτική συμπλήρωση ΑΦΜ")]
        [StringLength(10, ErrorMessage = "Πρέπει να είναι μέχρι 10 χαρακτήρες.", MinimumLength = 9)]
        [Display(Name = "ΑΦΜ")]
        public string AFM { get; set; }

        [Display(Name = "Ενεργός")]
        public bool? ISACTIVE { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        [Display(Name = "Ημ/νία εγγραφής")]
        public DateTime? CREATEDATE { get; set; }

        [StringLength(20, ErrorMessage = "Πρέπει να είναι μέχρι 20 χαρακτήρες.")]
        [DataType(DataType.Password)]
        [Compare("PASSWORD", ErrorMessage = "Ο κωδικός πρόσβασης και αυτός της επιβεβαίωσης δεν είναι ίδιοι.")]
        [Display(Name = "Επιβεβαίωση κωδικού")]
        public string ConfirmPassword { get; set; }

    }

    public class UserTeacherEditViewModel
    {
        public int USER_ID { get; set; }

        [Required(ErrorMessage = "Υποχρεωτική συμπλήρωση ονόματος χρήστη")]
        [StringLength(20, ErrorMessage = "Πρέπει να είναι μέχρι 20 χαρακτήρες.")]
        [Display(Name = "Όνομα χρήστη")]
        public string USERNAME { get; set; }

        [Required(ErrorMessage = "Υποχρεωτική συμπλήρωση κωδικού ασφαλείας")]
        [StringLength(20, ErrorMessage = "Πρέπει να είναι μέχρι 20 χαρακτήρες.")]
        [Display(Name = "Κωδικός πρόσβασης")]
        public string PASSWORD { get; set; }

        [Required(ErrorMessage = "Υποχρεωτική συμπλήρωση ΑΦΜ")]
        [StringLength(10, ErrorMessage = "Πρέπει να είναι μέχρι 10 χαρακτήρες.", MinimumLength = 9)]
        [Display(Name = "ΑΦΜ")]
        public string AFM { get; set; }

        [Display(Name = "Ενεργός")]
        public bool? ISACTIVE { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        [Display(Name = "Ημ/νία εγγραφής")]
        public DateTime? CREATEDATE { get; set; }
    }

    public class TeacherAccountInfoViewModel
    {
        public int USER_ID { get; set; }

        [Display(Name = "Όνομα χρήστη")]
        public string USERNAME { get; set; }

        [Display(Name = "Ονοματεπώνυμο")]
        public string FULLNAME { get; set; }

        [Display(Name = "Πατρώνυμο")]
        public string FATHERNAME { get; set; }

        [Display(Name = "Μητρώνυμο")]
        public string MOTHERNAME { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        [Display(Name = "Ημ/νια γέννησης")]
        public Nullable<System.DateTime> BIRTHDATE { get; set; }

        [Display(Name = "ΑΜΚΑ")]
        public string AMKA { get; set; }

        [Display(Name = "Τλεφωνα")]
        public string TELEPHONES { get; set; }


        public string AFM { get; set; }

    }

    public class TaxisnetViewModel
    {
        public int TAXISNET_ID { get; set; }

        public Nullable<int> RANDOM_NUMBER { get; set; }

        public string TAXISNET_AFM { get; set; }
    }

    public class sqlUserTeacherViewModel
    {
        public int USER_ID { get; set; }

        [Display(Name = "ΑΦΜ")]
        public string USER_AFM { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        [Display(Name = "Ημ/νία εγγραφής")]
        public Nullable<System.DateTime> CREATEDATE { get; set; }

        [Display(Name = "Ονοματεπώνυμο")]
        public string FULLNAME { get; set; }
    }

}