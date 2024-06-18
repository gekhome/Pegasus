using Pegasus.DAL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Pegasus.Models
{
    public class AitisisOriginalViewModel
    {
        #region ΣΤΟΙΧΕΙΑ ΑΙΤΗΣΗΣ

        public int AITISI_ID { get; set; }

        //[Required(ErrorMessage = "Υποχρεωτική συμπλήρωση")]
        [Display(Name = "Ημερομηνία Αίτησης")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public Nullable<System.DateTime> AITISI_DATE { get; set; }

        [Display(Name = "Αρ. Πρωτοκόλλου")]
        public string AITISI_PROTOCOL { get; set; }

        public Nullable<int> PROKIRIXI_ID { get; set; }

        public bool? TRANSFERRED { get; set; }

        #endregion

        #region ΕΚΠΑΙΔΕΥΤΙΚΟΣ

        [Display(Name = "ΑΦΜ")]
        public string AFM { get; set; }

        [Display(Name = "Ηλικία")]
        public Nullable<int> AGE { get; set; }

        [Display(Name = "Δ.Ο.Υ.")]
        public string DOY { get; set; }

        [Display(Name = "Α.Μ.Κ.Α.")]
        public string AMKA { get; set; }

        [Display(Name = "Επώνυμο")]
        public string LASTNAME { get; set; }

        [Display(Name = "Όνομα")]
        public string FIRSTNAME { get; set; }

        [Display(Name = "Πατρώνυμο")]
        public string FATHERNAME { get; set; }

        [Display(Name = "Αρ. Τέκνων")]
        public int? CHILDREN { get; set; }

        [Display(Name = "Ονοματεπώνυμο")]
        public string FULLNAME
        { 
            get { return LASTNAME + " " + FIRSTNAME; }
        }
        #endregion

        #region ΚΟΙΝΩΝΙΚΑ ΚΡΙΤΗΡΙΑ

        [Display(Name = "Διάρκεια ανεργίας")]
        public Nullable<int> ANERGIA_OLD { get; set; }

        [Display(Name = "Διάρκεια ανεργίας")]
        public Nullable<int> ANERGIA { get; set; }


        [Display(Name = "Ειδική Κατηγορία (Σχ.Έτος 2015-2016)")]
        public Nullable<int> SOCIALGROUP { get; set; }

        [Display(Name = "Αρ.Πρωτοκόλλου")]
        public string SOCIALGROUP_PROTOCOL { get; set; }

        [Display(Name = "Εκδούσα Αρχή")]
        public string SOCIALGROUP_YPIRESIA { get; set; }

        [Display(Name = "Προστατευόμενος του Ν.2190")]
        public Nullable<int> N2190 { get; set; }

        [Display(Name = "Γονέας τρίτεκνης οικογένειας")]
        public bool SOCIALGROUP1 { get; set; }

        [Display(Name = "Μέλος πολύτεκνης οικογένειας")]
        public bool SOCIALGROUP2 { get; set; }

        [Display(Name = "Μέλος μονογονεϊκής οικογένειας")]
        public bool SOCIALGROUP3 { get; set; }

        [Display(Name = "ΑΜΕΑ (ο ίδιος ή τέκνα αυτού)")]
        public bool SOCIALGROUP4 { get; set; }

        [Display(Name = "Πιστοποιητικό")]
        public string SOCIALGROUP1_DOC { get; set; }

        [Display(Name = "Πιστοποιητικό")]
        public string SOCIALGROUP2_DOC { get; set; }
        [Display(Name = "Πιστοποιητικό")]
        public string SOCIALGROUP3_DOC { get; set; }
        [Display(Name = "Πιστοποιητικό")]
        public string SOCIALGROUP4_DOC { get; set; }

        #endregion

        #region ΕΠΙΠΕΔΟ ΕΚΠΑΙΔΕΥΣΗΣ

        [Required(ErrorMessage = "Υποχρεωτική συμπλήρωση")]
        [Display(Name = "Βαθμίδα")]
        public Nullable<int> KLADOS { get; set; }

        [Required(ErrorMessage = "Υποχρεωτική συμπλήρωση")]
        [Display(Name = "Ειδικότητα")]
        public Nullable<int> EIDIKOTITA { get; set; }

        [Display(Name = "Ομάδα")]
        public Nullable<int> EIDIKOTITA_GROUP { get; set; }

        [Display(Name = "Βασική Εκπαίδευση (μόνο για Εμπειροτεχνίτες)")]
        public int? BASIC_EDUCATION { get; set; }

        [Display(Name = "Πιστοποίηση στα Μητρώα Εκπαιδευτών Ενηλίκων")]
        public bool CERTIFIED { get; set; }


        #endregion

        #region ΤΙΤΛΟΙ ΣΠΟΥΔΩΝ

        [Required(ErrorMessage = "Υποχρεωτική συμπλήρωση κατηγορίας πτυχίου")]
        [Display(Name = "Κατηγορία πτυχίου *")]
        public int? PTYXIO_TYPE { get; set; }

        [Required(ErrorMessage = "Υποχρεωτική συμπλήρωση")]
        [Display(Name = "Τίτλος Βασικού Πτυχίου")]
        [StringLength(150, ErrorMessage = "Πρέπει να είναι μέχρι 150 χαρακτήρες.")]
        public string PTYXIO_TITLOS { get; set; }

        [Required(ErrorMessage = "Υποχρεωτική συμπλήρωση")]
        [DisplayFormat(DataFormatString = "{0:0.00}")]
        //[Range(5, 20, ErrorMessage = "Η τιμή πρέπει να είναι μεταξύ 5 και 20")]
        [Display(Name = "Βαθμός Πτυχίου")]
        public decimal? PTYXIO_BATHMOS { get; set; }

        [Required(ErrorMessage = "Υποχρεωτική συμπλήρωση")]
        [Display(Name = "Ημ/νία κτήσης")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? PTYXIO_DATE { get; set; }

        [Display(Name = "Μεταπτυχιακό")]
        public bool MSC { get; set; }

        [Display(Name = "Διάρκεια Μεταπτυχιακού")]
        public int? MSC_DIARKEIA { get; set; }

        [Display(Name = "Τίτλος Μεταπτυχιακού")]
        [StringLength(150, ErrorMessage = "Πρέπει να είναι μέχρι 150 χαρακτήρες.")]
        public string MSC_TITLOS { get; set; }

        [Display(Name = "Διδακτορικό")]
        public bool PHD { get; set; }

        [Display(Name = "Τίτλος Διδακτορικού")]
        [StringLength(150, ErrorMessage = "Πρέπει να είναι μέχρι 150 χαρακτήρες.")]
        public string PHD_TITLOS { get; set; }

        [Display(Name = "Παιδαγωγικό")]
        public bool PED { get; set; }

        [Display(Name = "Διάρκεια Παιδαγωγικού")]
        public int? PED_DIARKEIA { get; set; }

        [Display(Name = "Τίτλος Παιδαγωγικού")]
        [StringLength(150, ErrorMessage = "Πρέπει να είναι μέχρι 150 χαρακτήρες.")]
        public string PED_TITLOS { get; set; }

        #endregion

        #region ΕΚΠΑΙΔΕΥΣΗ ΕΝΗΛΙΚΩΝ

        [Display(Name = "Μεταπτυχιακό Εκπ. Ενηλίκων")]
        public bool AED_MSC { get; set; }

        [Display(Name = "Τίτλος Μεταπτυχιακού")]
        [StringLength(150, ErrorMessage = "Πρέπει να είναι μέχρι 150 χαρακτήρες.")]
        public string AED_MSC_TITLOS { get; set; }

        [Display(Name = "Διδακτορικό Εκπ. Ενηλίκων")]
        public bool AED_PHD { get; set; }

        [Display(Name = "Τίτλος Διδακτορικού")]
        [StringLength(150, ErrorMessage = "Πρέπει να είναι μέχρι 150 χαρακτήρες.")]
        public string AED_PHD_TITLOS { get; set; }

        #endregion

        #region ΑΛΛΑ ΠΡΟΣΟΝΤΑ (ΓΛΩΣΣΑ; Η/Υ)

        [Display(Name = "Ξένη Γλώσσα")]
        [StringLength(50, ErrorMessage = "Πρέπει να είναι μέχρι 50 χαρακτήρες.")]
        public string LANG_TEXT { get; set; }

        [Display(Name = "Επίπεδο γνώσης")]
        [StringLength(2, ErrorMessage = "Πρέπει να είναι μέχρι 2 χαρακτήρες.")]
        public string LANG_LEVEL { get; set; }

        [Display(Name = "Τίτλος Πιστοποιητικού")]
        [StringLength(150, ErrorMessage = "Πρέπει να είναι μέχρι 150 χαρακτήρες.")]
        public string LANG_TITLOS { get; set; }

        [Display(Name = "1η Ξένη Γλώσσα")]
        [StringLength(50, ErrorMessage = "Πρέπει να είναι μέχρι 50 χαρακτήρες.")]
        public string LANG1_TEXT { get; set; }

        [Display(Name = "Επίπεδο γνώσης")]
        [StringLength(2, ErrorMessage = "Πρέπει να είναι μέχρι 2 χαρακτήρες.")]
        public string LANG1_LEVEL { get; set; }

        [Display(Name = "Πιστοποιητικό")]
        [StringLength(150, ErrorMessage = "Πρέπει να είναι μέχρι 150 χαρακτήρες.")]
        public string LANG1_TITLOS { get; set; }

        [Display(Name = "2η Ξένη Γλώσσα")]
        [StringLength(50, ErrorMessage = "Πρέπει να είναι μέχρι 50 χαρακτήρες.")]
        public string LANG2_TEXT { get; set; }

        [Display(Name = "Επίπεδο γνώσης")]
        [StringLength(2, ErrorMessage = "Πρέπει να είναι μέχρι 2 χαρακτήρες.")]
        public string LANG2_LEVEL { get; set; }

        [Display(Name = "Πιστοποιητικό")]
        [StringLength(150, ErrorMessage = "Πρέπει να είναι μέχρι 150 χαρακτήρες.")]
        public string LANG2_TITLOS { get; set; }


        [Display(Name = "Πιστοποιητικό κατά ΑΣΕΠ")]
        public int? COMPUTER_CERT { get; set; }

        [Display(Name = "Τίτλος Πιστοποιητικού")]
        [StringLength(150, ErrorMessage = "Πρέπει να είναι μέχρι 150 χαρακτήρες.")]
        public string COMPUTER_TITLOS { get; set; }

        #endregion

        #region ΕΠΙΜΟΡΦΩΣΗ (ΜΟΝΟ ΓΙΑ ΙΕΚ)

        [Display(Name = "1. Στο διδακτικό αντικείμενο των προκηρυσσόμενων θέσεων")]
        public bool EPIMORFOSI1 { get; set; }

        [Display(Name="Ώρες")]
        [Range(0, 100000, ErrorMessage = "Η τιμή πρέπει να είναι μεταξύ 0 και 100000")]
        public int EPIMORFOSI1_HOURS { get; set; }

        [Display(Name = "2. Σχετικά με το θεσμό της επαγγ. εκπαίδευσης και κατάρτισης")]
        public bool EPIMORFOSI2 { get; set; }

        [Display(Name = "Ώρες")]
        [Range(0, 100000, ErrorMessage = "Η τιμή πρέπει να είναι μεταξύ 0 και 100000")]
        public int EPIMORFOSI2_HOURS { get; set; }

        [Display(Name = "3. Στις αρχές εκπαίδευσης ενηλίκων (εκτός θεμάτων κατάρτισης)")]
        public bool EPIMORFOSI3 { get; set; }

        [Display(Name = "Ώρες")]
        [Range(0, 100000, ErrorMessage = "Η τιμή πρέπει να είναι μεταξύ 0 και 100000")]
        public int EPIMORFOSI3_HOURS { get; set; }

        #endregion

        #region ΣΧΟΛΕΙΑ ΠΡΟΤΙΜΗΣΗΣ ΚΑΙ ΛΟΙΠΑ ΣΤΟΙΧΕΙΑ

        [Required(ErrorMessage = "Υποχρεωτική συμπλήρωση")]
        [Display(Name = "Περιφέρεια")]
        public Nullable<int> PERIFERIA_ID { get; set; }

        [Required(ErrorMessage = "Υποχρεωτική συμπλήρωση")]
        [Display(Name = "Σχολείο 1ης επιλογής")]
        public Nullable<int> SCHOOL_ID { get; set; }

        [Required(ErrorMessage = "Υποχρεωτική συμπλήρωση")]
        [Display(Name = "Επαγγελματική Ιδιότητα")]
        public int? EPAGELMA_STATUS { get; set; }

        [Display(Name = "Χρόνος αποθήκευσης")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm:ss}")]
        public Nullable<System.DateTime> TIMESTAMP { get; set; }

        #endregion

        #region ΑΠΟΤΕΛΕΣΜΑΤΑ (ΜΟΡΙΑ)

        [Display(Name="Μόρια Ανεργίας")]
        [DisplayFormat(DataFormatString = "{0:0.00}")]
        public float MORIA_ANERGIA { get; set; }

        [Display(Name = "Μόρια Κοινωνικών Κριτηρίων")]
        [DisplayFormat(DataFormatString = "{0:0.00}")]
        public float MORIA_SOCIAL { get; set; }

        [Display(Name = "Μόρια Τίτλου Σπουδών")]
        [DisplayFormat(DataFormatString = "{0:0.00}")]
        public float MORIA_PTYXIO { get; set; }

        [Display(Name = "Μόρια Μεταπτυχιακού")]
        [DisplayFormat(DataFormatString = "{0:0.00}")]
        public float MORIA_MSC { get; set; }

        [Display(Name = "Μόρια Διδακτορικού")]
        [DisplayFormat(DataFormatString = "{0:0.00}")]
        public float MORIA_PHD { get; set; }

        [Display(Name = "Μόρια Παιδαγωγικού")]
        [DisplayFormat(DataFormatString = "{0:0.00}")]
        public float MORIA_PED { get; set; }

        [Display(Name = "Μόρια Μεταπτ. Εκπ. Ενηλίκων")]
        [DisplayFormat(DataFormatString = "{0:0.00}")]
        public float MORIA_AED_MSC { get; set; }

        [Display(Name = "Μόρια Διδακτ. Εκπ. Ενηλίκων")]
        [DisplayFormat(DataFormatString = "{0:0.00}")]
        public float MORIA_AED_PHD { get; set; }

        [Display(Name = "Μόρια Ξένης Γλώσσας")]
        [DisplayFormat(DataFormatString = "{0:0.00}")]
        public float MORIA_LANG { get; set; }

        [Display(Name = "Μόρια Γνώσης Η/Υ")]
        [DisplayFormat(DataFormatString = "{0:0.00}")]
        public float MORIA_COMPUTER { get; set; }

        [Display(Name = "Μόρια Επιμόρφωσης στο διδ. αντικείμενο")]
        [DisplayFormat(DataFormatString = "{0:0.00}")]
        public float MORIA_EPIMORFOSI1 { get; set; }

        [Display(Name = "Μόρια Επιμόρφωσης στην επαγγ. κατ./εκπ. ενηλ.")]
        [DisplayFormat(DataFormatString = "{0:0.00}")]
        public float MORIA_EPIMORFOSI2 { get; set; }

        [Display(Name = "Μόρια Επιμόρφωσης στις αρχές εκπ. ενηλίκων")]
        [DisplayFormat(DataFormatString = "{0:0.00}")]
        public float MORIA_EPIMORFOSI3 { get; set; }

        [Display(Name = "Μόρια Διδακτικής Εμπειρίας")]
        [DisplayFormat(DataFormatString = "{0:0.00}")]
        public float MORIA_TEACH { get; set; }

        [Display(Name = "Μόρια Επαγγελματικής Εμπειρίας")]
        [DisplayFormat(DataFormatString = "{0:0.00}")]
        public float MORIA_WORK1 { get; set; }

        [Display(Name = "Μόρια Ελευθέριου Επαγγέλματος")]
        [DisplayFormat(DataFormatString = "{0:0.00}")]
        public float MORIA_WORK2 { get; set; }

        [Display(Name = "Μόρια Εργασιακής Εμπειρίας")]
        [DisplayFormat(DataFormatString = "{0:0.00}")]
        public float MORIA_WORK { get; set; }

        [Display(Name = "Συνολικά Μόρια")]
        [DisplayFormat(DataFormatString="{0:0.00}")]
        public float MORIA_TOTAL { get; set; }

        #endregion

        public virtual TEACHERS TEACHERS { get; set; }
    }

}