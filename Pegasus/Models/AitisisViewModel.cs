using Pegasus.DAL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Pegasus.Models
{
    public class AitisisViewModel
    {
        #region ΣΤΟΙΧΕΙΑ ΑΙΤΗΣΗΣ

        public int AITISI_ID { get; set; }

        //[Required(ErrorMessage = "Υποχρεωτική συμπλήρωση")]
        [Display(Name = "Ημερομηνία Αίτησης")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? AITISI_DATE { get; set; }

        [Display(Name = "Αρ. Πρωτοκόλλου")]
        public string AITISI_PROTOCOL { get; set; }

        public int? PROKIRIXI_ID { get; set; }

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
        [Range(0, 20, ErrorMessage = "Η τιμή πρέπει να είναι μεταξύ 0 και 20")]
        public int? CHILDREN { get; set; }

        [Display(Name = "Ονοματεπώνυμο")]
        public string FULLNAME
        { 
            get { return LASTNAME + " " + FIRSTNAME; }
        }
        #endregion

        #region ΚΟΙΝΩΝΙΚΑ ΚΡΙΤΗΡΙΑ

        [Display(Name = "Διάρκεια ανεργίας")]
        public int? ANERGIA_OLD { get; set; }

        [Display(Name = "Διάρκεια ανεργίας")]
        public int? ANERGIA { get; set; }

        [Display(Name = "Ειδική Κατηγορία (Σχ.Έτος 2015-2016)")]
        public int? SOCIALGROUP { get; set; }

        [Display(Name = "Αρ.Πρωτοκόλλου")]
        [StringLength(50, ErrorMessage = "Πρέπει να είναι μέχρι 50 χαρακτήρες.")]
        public string SOCIALGROUP_PROTOCOL { get; set; }

        [Display(Name = "Εκδούσα Αρχή")]
        [StringLength(150, ErrorMessage = "Πρέπει να είναι μέχρι 150 χαρακτήρες.")]
        public string SOCIALGROUP_YPIRESIA { get; set; }

        [Display(Name = "Προστατευόμενος του Ν.2190")]
        public int? N2190 { get; set; }

        [Display(Name = "Γονέας τρίτεκνης οικογένειας")]
        public bool SOCIALGROUP1 { get; set; }

        [Display(Name = "Μέλος πολύτεκνης οικογένειας")]
        public bool SOCIALGROUP2 { get; set; }

        [Display(Name = "Μέλος μονογονεϊκής οικογένειας")]
        public bool SOCIALGROUP3 { get; set; }

        [Display(Name = "ΑΜΕΑ (ο ίδιος ή τέκνα αυτού)")]
        public bool SOCIALGROUP4 { get; set; }

        [Display(Name = "Πιστοποιητικό")]
        [StringLength(200, ErrorMessage = "Πρέπει να είναι μέχρι 200 χαρακτήρες.")]
        public string SOCIALGROUP1_DOC { get; set; }

        [Display(Name = "Πιστοποιητικό")]
        [StringLength(200, ErrorMessage = "Πρέπει να είναι μέχρι 200 χαρακτήρες.")]
        public string SOCIALGROUP2_DOC { get; set; }

        [Display(Name = "Πιστοποιητικό")]
        [StringLength(200, ErrorMessage = "Πρέπει να είναι μέχρι 200 χαρακτήρες.")]
        public string SOCIALGROUP3_DOC { get; set; }

        [Display(Name = "Πιστοποιητικό")]
        [StringLength(200, ErrorMessage = "Πρέπει να είναι μέχρι 200 χαρακτήρες.")]
        public string SOCIALGROUP4_DOC { get; set; }

        #endregion

        #region ΕΠΙΠΕΔΟ ΕΚΠΑΙΔΕΥΣΗΣ

        [Required(ErrorMessage = "Υποχρεωτική συμπλήρωση κλάδου")]
        [Display(Name = "Κλάδος *")]
        public int? KLADOS { get; set; }

        [Required(ErrorMessage = "Υποχρεωτική συμπλήρωση ειδικότητας")]
        [Display(Name = "Ειδικότητα *")]
        public int? EIDIKOTITA { get; set; }

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

        [Required(ErrorMessage = "Υποχρεωτική συμπλήρωση τίτλου πτυχίου")]
        [Display(Name = "Τίτλος βασικού πτυχίου *")]
        [StringLength(150, ErrorMessage = "Πρέπει να είναι μέχρι 150 χαρακτήρες.")]
        public string PTYXIO_TITLOS { get; set; }

        [Required(ErrorMessage = "Υποχρεωτική συμπλήρωση βαθμού πτυχίου")]
        [DisplayFormat(DataFormatString = "{0:0.00}")]
        [Display(Name = "Βαθμός πτυχίου *")]
        public decimal? PTYXIO_BATHMOS { get; set; }

        [Required(ErrorMessage = "Υποχρεωτική συμπλήρωση ημερομηνίας κτήσης")]
        [Display(Name = "Ημερομηνία κτήσης *")]
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
        //[StringLength(50, ErrorMessage = "Πρέπει να είναι μέχρι 50 χαρακτήρες.")]
        public string LANG1_TEXT { get; set; }

        [Display(Name = "Επίπεδο γνώσης")]
        //[StringLength(2, ErrorMessage = "Πρέπει να είναι μέχρι 2 χαρακτήρες.")]
        public string LANG1_LEVEL { get; set; }

        [Display(Name = "Πιστοποιητικό")]
        [StringLength(150, ErrorMessage = "Πρέπει να είναι μέχρι 150 χαρακτήρες.")]
        public string LANG1_TITLOS { get; set; }

        [Display(Name = "2η Ξένη Γλώσσα")]
        //[StringLength(50, ErrorMessage = "Πρέπει να είναι μέχρι 50 χαρακτήρες.")]
        public string LANG2_TEXT { get; set; }

        [Display(Name = "Επίπεδο γνώσης")]
        //[StringLength(2, ErrorMessage = "Πρέπει να είναι μέχρι 2 χαρακτήρες.")]
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
        [Range(0, 1000, ErrorMessage = "Η τιμή πρέπει να είναι μεταξύ 0 και 1000")]
        public int EPIMORFOSI1_HOURS { get; set; }

        [Display(Name = "2. Σχετικά με το θεσμό της επαγγ. εκπαίδευσης και κατάρτισης")]
        public bool EPIMORFOSI2 { get; set; }

        [Display(Name = "Ώρες")]
        [Range(0, 1000, ErrorMessage = "Η τιμή πρέπει να είναι μεταξύ 0 και 1000")]
        public int EPIMORFOSI2_HOURS { get; set; }

        [Display(Name = "3. Στις αρχές εκπαίδευσης ενηλίκων (εκτός θεμάτων κατάρτισης)")]
        public bool EPIMORFOSI3 { get; set; }

        [Display(Name = "Ώρες")]
        [Range(0, 1000, ErrorMessage = "Η τιμή πρέπει να είναι μεταξύ 0 και 1000")]
        public int EPIMORFOSI3_HOURS { get; set; }

        #endregion

        #region ΣΧΟΛΕΙΑ ΠΡΟΤΙΜΗΣΗΣ

        [Required(ErrorMessage = "Υποχρεωτική συμπλήρωση περιφέρειας")]
        [Display(Name = "Περιφέρεια *")]
        public Nullable<int> PERIFERIA_ID { get; set; }

        [Required(ErrorMessage = "Υποχρεωτική συμπλήρωση σχολείου")]
        [Display(Name = "Σχολείο 1ης επιλογής *")]
        public Nullable<int> SCHOOL_ID { get; set; }

        [Required(ErrorMessage = "Υποχρεωτική συμπλήρωση επαγγελματικής ιδιότητας")]
        [Display(Name = "Επαγγελματική Ιδιότητα *")]
        public int? EPAGELMA_STATUS { get; set; }

        [Display(Name = "Σχολικές Μονάδες")]
        public virtual List<AITISIS_SCHOOLS> AITISIS_SCHOOLS { get; set; }

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

        #region ΕΠΙΤΡΟΠΕΣ

        [Display(Name="Ημ/νία Μοριοδότησης")]
        public DateTime MORIODOTISI_DATE { get; set; }

        [Display(Name="Μοριοδότης ID")]
        public string MORIODOTISI_PERSON { get; set; }

        [Display(Name = "Μοριοδότης")]
        public string MORIODOTIS_NAME { get; set; }


        [Display(Name="Αποκλεισμός")]
        public bool APOKLEISMOS { get; set; }

        [Display(Name="Αιτία Αποκλεισμού")]
        public int? APOKLEISMOS_AITIA { get; set; }

        [Display(Name="Κείμενο Α'/βάθμιας Επιτροπής")]
        public string EPITROPI1_TEXT { get; set; }

        [Display(Name = "Κείμενο Β'/βάθμιας Επιτροπής")]
        public string EPITROPI2_TEXT { get; set; }

        [Display(Name = "Έλεγχος Αίτησης")]
        public bool CHECK_STATUS { get; set; }

        [Display(Name = "Έγινε Ένσταση")]
        public bool ENSTASI { get; set; }

        [Display(Name = "Αιτία Ένστασης")]
        public string ENSTASI_AITIA { get; set; }

        #endregion

        // Constructors
        public AitisisViewModel() { }

        public AitisisViewModel(AITISIS a, PegasusDBEntities db)
        {
            this.AITISI_ID = a.AITISI_ID;
            this.AFM = a.AFM;
            this.AITISI_DATE = a.AITISI_DATE;
            this.AITISI_PROTOCOL = a.AITISI_PROTOCOL;
            this.ANERGIA = a.ANERGIA;
            this.SOCIALGROUP = a.SOCIALGROUP;
            this.SOCIALGROUP_PROTOCOL = a.SOCIALGROUP_PROTOCOL;
            this.SOCIALGROUP_YPIRESIA = a.SOCIALGROUP_YPIRESIA;
            this.N2190 = a.N2190;
            this.KLADOS = a.KLADOS;
            this.EIDIKOTITA = a.EIDIKOTITA;
            this.BASIC_EDUCATION = a.BASIC_EDUCATION;
            this.PTYXIO_TYPE = a.PTYXIO_TYPE;
            this.PTYXIO_TITLOS = a.PTYXIO_TITLOS;
            this.PTYXIO_BATHMOS = a.PTYXIO_BATHMOS;
            this.PTYXIO_DATE = a.PTYXIO_DATE;
            this.MSC = a.MSC ?? false;
            this.MSC_DIARKEIA = a.MSC_DIARKEIA;
            this.MSC_TITLOS = a.MSC_TITLOS;
            this.PHD = a.PHD ?? false;
            this.PHD_TITLOS = a.PHD_TITLOS;
            this.PED = a.PED ?? false;
            this.PED_TITLOS = a.PED_TITLOS;
            this.PED_DIARKEIA = a.PED_DIARKEIA;
            this.AED_MSC = a.AED_MSC ?? false;
            this.AED_MSC_TITLOS = a.AED_MSC_TITLOS;
            this.AED_PHD = a.AED_PHD ?? false;
            this.AED_PHD_TITLOS = a.AED_PHD_TITLOS;
            this.LANG_TEXT = a.LANG_TEXT;
            this.LANG_LEVEL = a.LANG_LEVEL;
            this.LANG_TITLOS = a.LANG_TITLOS;
            this.COMPUTER_CERT = a.COMPUTER_CERT;
            this.COMPUTER_TITLOS = a.COMPUTER_TITLOS;
            this.EPIMORFOSI1 = a.EPIMORFOSI1 ?? false;
            this.EPIMORFOSI1_HOURS = a.EPIMORFOSI1_HOURS ?? 0;
            this.EPIMORFOSI2 = a.EPIMORFOSI2 ?? false;
            this.EPIMORFOSI2_HOURS = a.EPIMORFOSI2_HOURS ?? 0;
            this.PERIFERIA_ID = a.PERIFERIA_ID;
            this.SCHOOL_ID = a.SCHOOL_ID;
            this.EPAGELMA_STATUS = a.EPAGELMA_STATUS ?? 0;
            this.PROKIRIXI_ID = a.PROKIRIXI_ID ?? 0;
            this.CHILDREN = a.CHILDREN;
            this.TRANSFERRED = a.TRANSFERRED;
            // new fields (2016)
            this.CERTIFIED = a.CERTIFIED ?? false;
            this.SOCIALGROUP1 = a.SOCIALGROUP1 ?? false;
            this.SOCIALGROUP2 = a.SOCIALGROUP2 ?? false;
            this.SOCIALGROUP3 = a.SOCIALGROUP3 ?? false;
            this.SOCIALGROUP4 = a.SOCIALGROUP4 ?? false;
            this.SOCIALGROUP1_DOC = a.SOCIALGROUP1_DOC;
            this.SOCIALGROUP2_DOC = a.SOCIALGROUP2_DOC;
            this.SOCIALGROUP3_DOC = a.SOCIALGROUP3_DOC;
            this.SOCIALGROUP4_DOC = a.SOCIALGROUP4_DOC;
            this.LANG1_TEXT = a.LANG1_TEXT;
            this.LANG1_LEVEL = a.LANG1_LEVEL;
            this.LANG1_TITLOS = a.LANG1_TITLOS;
            this.LANG2_TEXT = a.LANG2_TEXT;
            this.LANG2_LEVEL = a.LANG2_LEVEL;
            this.LANG2_TITLOS = a.LANG2_TITLOS;
            this.EPIMORFOSI3 = a.EPIMORFOSI3 ?? false;
            this.EPIMORFOSI3_HOURS = a.EPIMORFOSI3_HOURS ?? 0;
            // calculated fields
            this.AGE = a.AGE;
            this.MORIA_ANERGIA = a.MORIA_ANERGIA ?? 0;
            this.MORIA_PTYXIO = a.MORIA_PTYXIO ?? 0;
            this.MORIA_MSC = a.MORIA_MSC ?? 0;
            this.MORIA_PHD = a.MORIA_PHD ?? 0;
            this.MORIA_PED = a.MORIA_PED ?? 0;
            this.MORIA_AED_MSC = a.MORIA_AED_MSC ?? 0;
            this.MORIA_AED_PHD = a.MORIA_AED_PHD ?? 0;
            this.MORIA_LANG = a.MORIA_LANG ?? 0;
            this.MORIA_COMPUTER = a.MORIA_COMPUTER ?? 0;
            this.MORIA_EPIMORFOSI1 = a.MORIA_EPIMORFOSI1 ?? 0;
            this.MORIA_EPIMORFOSI2 = a.MORIA_EPIMORFOSI2 ?? 0;
            this.MORIA_EPIMORFOSI3 = a.MORIA_EPIMORFOSI3 ?? 0;
            this.MORIA_TEACH = a.MORIA_TEACH ?? 0;
            this.MORIA_WORK1 = a.MORIA_WORK1 ?? 0;
            this.MORIA_WORK2 = a.MORIA_WORK2 ?? 0;
            this.MORIA_SOCIAL = a.MORIA_SOCIAL ?? 0;
            this.MORIA_TOTAL = a.MORIA_TOTAL ?? 0;
            this.MORIA_WORK = a.MORIA_WORK ?? 0;
            this.AITISIS_SCHOOLS = (from s in db.AITISIS_SCHOOLS where s.AITISI_ID == a.AITISI_ID select s).ToList();
            
        }

        public AitisisViewModel(AITISIS a, PegasusDBEntities db, bool results)
        {
            this.AITISI_ID = a.AITISI_ID;
            this.AFM = a.AFM;
            this.AITISI_DATE = a.AITISI_DATE;
            this.AITISI_PROTOCOL = a.AITISI_PROTOCOL;
            this.ANERGIA = a.ANERGIA;
            this.SOCIALGROUP = a.SOCIALGROUP;
            this.SOCIALGROUP_PROTOCOL = a.SOCIALGROUP_PROTOCOL;
            this.SOCIALGROUP_YPIRESIA = a.SOCIALGROUP_YPIRESIA;
            this.N2190 = a.N2190;
            this.KLADOS = a.KLADOS;
            this.EIDIKOTITA = a.EIDIKOTITA;
            this.BASIC_EDUCATION = a.BASIC_EDUCATION;
            this.PTYXIO_TYPE = a.PTYXIO_TYPE;
            this.PTYXIO_TITLOS = a.PTYXIO_TITLOS;
            this.PTYXIO_BATHMOS = a.PTYXIO_BATHMOS;
            this.PTYXIO_DATE = a.PTYXIO_DATE;
            this.MSC = a.MSC ?? false;
            this.MSC_DIARKEIA = a.MSC_DIARKEIA;
            this.MSC_TITLOS = a.MSC_TITLOS;
            this.PHD = a.PHD ?? false;
            this.PHD_TITLOS = a.PHD_TITLOS;
            this.PED = a.PED ?? false;
            this.PED_TITLOS = a.PED_TITLOS;
            this.PED_DIARKEIA = a.PED_DIARKEIA;
            this.AED_MSC = a.AED_MSC ?? false;
            this.AED_MSC_TITLOS = a.AED_MSC_TITLOS;
            this.AED_PHD = a.AED_PHD ?? false;
            this.AED_PHD_TITLOS = a.AED_PHD_TITLOS;
            this.LANG_TEXT = a.LANG_TEXT;
            this.LANG_LEVEL = a.LANG_LEVEL;
            this.LANG_TITLOS = a.LANG_TITLOS;
            this.COMPUTER_CERT = a.COMPUTER_CERT;
            this.COMPUTER_TITLOS = a.COMPUTER_TITLOS;
            this.EPIMORFOSI1 = a.EPIMORFOSI1 ?? false;
            this.EPIMORFOSI1_HOURS = a.EPIMORFOSI1_HOURS ?? 0;
            this.EPIMORFOSI2 = a.EPIMORFOSI2 ?? false;
            this.EPIMORFOSI2_HOURS = a.EPIMORFOSI2_HOURS ?? 0;
            this.PERIFERIA_ID = a.PERIFERIA_ID;
            this.SCHOOL_ID = a.SCHOOL_ID;
            this.EPAGELMA_STATUS = a.EPAGELMA_STATUS ?? 0;
            this.PROKIRIXI_ID = a.PROKIRIXI_ID;
            this.CHILDREN = a.CHILDREN;
            this.TRANSFERRED = a.TRANSFERRED;
            // new fields (2016)
            this.CERTIFIED = a.CERTIFIED ?? false;
            this.SOCIALGROUP1 = a.SOCIALGROUP1 ?? false;
            this.SOCIALGROUP2 = a.SOCIALGROUP2 ?? false;
            this.SOCIALGROUP3 = a.SOCIALGROUP3 ?? false;
            this.SOCIALGROUP4 = a.SOCIALGROUP4 ?? false;
            this.SOCIALGROUP1_DOC = a.SOCIALGROUP1_DOC;
            this.SOCIALGROUP2_DOC = a.SOCIALGROUP2_DOC;
            this.SOCIALGROUP3_DOC = a.SOCIALGROUP3_DOC;
            this.SOCIALGROUP4_DOC = a.SOCIALGROUP4_DOC;
            this.LANG1_TEXT = a.LANG1_TEXT;
            this.LANG1_LEVEL = a.LANG1_LEVEL;
            this.LANG1_TITLOS = a.LANG1_TITLOS;
            this.LANG2_TEXT = a.LANG2_TEXT;
            this.LANG2_LEVEL = a.LANG2_LEVEL;
            this.LANG2_TITLOS = a.LANG2_TITLOS;
            this.EPIMORFOSI3 = a.EPIMORFOSI3 ?? false;
            this.EPIMORFOSI3_HOURS = a.EPIMORFOSI3_HOURS ?? 0;
            // calculated fields
            this.AGE = a.AGE;
            this.MORIA_ANERGIA = a.MORIA_ANERGIA ?? 0;
            this.MORIA_PTYXIO = a.MORIA_PTYXIO ?? 0;
            this.MORIA_MSC = a.MORIA_MSC ?? 0;
            this.MORIA_PHD = a.MORIA_PHD ?? 0;
            this.MORIA_PED = a.MORIA_PED ?? 0;
            this.MORIA_AED_MSC = a.MORIA_AED_MSC ?? 0;
            this.MORIA_AED_PHD = a.MORIA_AED_PHD ?? 0;
            this.MORIA_LANG = a.MORIA_LANG ?? 0;
            this.MORIA_COMPUTER = a.MORIA_COMPUTER ?? 0;
            this.MORIA_EPIMORFOSI1 = a.MORIA_EPIMORFOSI1 ?? 0;
            this.MORIA_EPIMORFOSI2 = a.MORIA_EPIMORFOSI2 ?? 0;
            this.MORIA_EPIMORFOSI3 = a.MORIA_EPIMORFOSI3 ?? 0;
            this.MORIA_TEACH = a.MORIA_TEACH ?? 0;
            this.MORIA_WORK1 = a.MORIA_WORK1 ?? 0;
            this.MORIA_WORK2 = a.MORIA_WORK2 ?? 0;
            this.MORIA_SOCIAL = a.MORIA_SOCIAL ?? 0;
            this.MORIA_TOTAL = a.MORIA_TOTAL ?? 0;
            this.MORIA_WORK = a.MORIA_WORK ?? 0;

            this.APOKLEISMOS = a.APOKLEISMOS ?? false;
            this.APOKLEISMOS_AITIA = a.APOKLEISMOS_AITIA ?? 0;
            this.MORIODOTISI_DATE = (DateTime)a.MORIODOTISI_DATE;
            this.MORIODOTISI_PERSON = a.MORIODOTISI_PERSON;
            this.CHECK_STATUS = a.CHECK_STATUS ?? false;
            this.AITISIS_SCHOOLS = (from s in db.AITISIS_SCHOOLS where s.AITISI_ID == a.AITISI_ID select s).ToList();
        }

        public Boolean canDelete { get; set; }
        public Boolean canEdit { get; set; }
    }

    /*LEFTERIS*/
    public class AITISI_SCHOOLSViewModel
    {
        public int ID { get; set; }

        public int AITISI_ID { get; set; }

        [Display(Name = "Τύπος Σχολικής Μονάδας")]
        [Required(ErrorMessage = "Υποχρεωτική συμπλήρωση")]
        [Range(1, int.MaxValue, ErrorMessage = "Υποχρεωτική συμπλήρωση")]
        //[UIHint("GridForeignKey")]
        public int SCHOOL_TYPE { get; set; }

        [Display(Name = "Σχολική Μονάδα")]
        [Required(ErrorMessage = "Υποχρεωτική συμπλήρωση")]
        [Range(1, int.MaxValue, ErrorMessage = "Υποχρεωτική συμπλήρωση")]
        //[UIHint("GridForeignKey")]
        public int SCHOOL { get; set; }

        [Display(Name = "Περιφέρεια Αίτησης")]
        //[UIHint("GridForeignKey")]
        public int PERIFERIA_ID { get; set; }

        public int? PROKIRIXI_ID { get; set; }

    }

    public class AitisisGridViewModel
    {
        public int AITISI_ID { get; set; }

        [Display(Name = "Αρ. Πρωτοκόλλου")]
        public string AITISI_PROTOCOL { get; set; }

        [Display(Name = "Ημερομηνία Αίτησης")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? AITISI_DATE { get; set; }

        [Display(Name = "ΑΦΜ")]
        public string AFM { get; set; }

        [Display(Name = "Επώνυμο")]
        public string LASTNAME { get; set; }

        public int? PROKIRIXI_ID { get; set; }

        [Display(Name = "Ειδικότητα")]
        public int? EIDIKOTITA { get; set; }

        [Display(Name = "Σχολείο αίτησης")]
        public int? SCHOOL_ID { get; set; }

        [Display(Name = "Περιφέρεια")]
        public int? PERIFERIA_ID { get; set; }

    }

    public class sqlTeacherAitiseisModel
    {
        public int AITISI_ID { get; set; }

        [Display(Name = "Αρ. Πρωτ.")]
        public string AITISI_PROTOCOL { get; set; }

        [Display(Name = "Περιφέρεια")]
        public string PERIFERIA_NAME { get; set; }

        [Display(Name = "Α.φ.Μ.")]
        public string AFM { get; set; }

        [Display(Name = "Ονοματεπώνυμο")]
        public string FULLNAME { get; set; }

        [Display(Name = "Κλάδος-Ειδικότητα")]
        public string EIDIKOTITA_TEXT { get; set; }

        [Display(Name = "Σχολείο")]
        public string SCHOOL_NAME { get; set; }

        [Display(Name = "Έλεγχ.")]
        public bool CHECK_STATUS { get; set; }

        [Display(Name = "Προκήρυξη")]
        public string PROTOCOL { get; set; }

        [Display(Name = "Περιφερειακή")]
        public string PERIFERIAKI { get; set; }
    }

}