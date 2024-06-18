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

    public class SYS_GENDERSViewModel
    {
        public int GENDER_ID { get; set; }
        public string GENDER { get; set; }
    }

    public class SYS_ARMYViewModel
    {
        public int ARMY_ID { get; set; }
        public string ARMY_TEXT { get; set; }
    }

    public class SYS_KLADOSViewModel
    {
        public SYS_KLADOSViewModel()
        {
            this.SYS_EIDIKOTITES = new HashSet<SYS_EIDIKOTITES>();
        }
        public int KLADOS_ID { get; set; }
        public string KLADOS_NAME { get; set; }
        public virtual ICollection<SYS_EIDIKOTITES> SYS_EIDIKOTITES { get; set; }
    }

    public class SYS_KLADOS_ENIAIOSViewModel
    {
        public int ΚΛΑΔΟΣ_ΚΩΔ { get; set; }

        [Display(Name = "Κλάδος ενοποίησης")]
        public string ΚΛΑΔΟΣ_ΕΝΙΑΙΟΣ { get; set; }

        [Display(Name = "Κλάδος")]
        public Nullable<int> ΚΛΑΔΟΣ { get; set; }

        public virtual ICollection<SYS_EIDIKOTITES> SYS_EIDIKOTITES { get; set; }
    }

    public class KladosUnifiedViewModel
    {
        public int ΚΛΑΔΟΣ_ΚΩΔ { get; set; }

        [Display(Name = "Κλάδος ενοποίησης")]
        public string ΚΛΑΔΟΣ_ΕΝΙΑΙΟΣ { get; set; }

        [Display(Name = "Κλάδος")]
        public Nullable<int> ΚΛΑΔΟΣ { get; set; }

        public virtual ICollection<SYS_EIDIKOTITES> SYS_EIDIKOTITES { get; set; }
    }

    public partial class sqlEidikotitesKUViewModel
    {
        public int EIDIKOTITA_ID { get; set; }
        public string EIDIKOTITA_PTYXIO { get; set; }
        public Nullable<int> KLADOS_UNIFIED { get; set; }
        public Nullable<int> EIDIKOTITA_KLADOS_ID { get; set; }
    }

    public class SYS_EIDIKOTITESViewModel
    {
        public int EIDIKOTITA_ID { get; set; }

        [Display(Name = "Κλάδος")]
        public string EIDIKOTITA_CODE { get; set; }

        [Display(Name = "Ειδικότητα")]
        public string EIDIKOTITA_NAME { get; set; }

        [Display(Name = "Κλάδος-Ειδικότητα")]
        public string EIDIKOTITA_DESC 
        { 
            get 
            { return EIDIKOTITA_CODE + "-" + EIDIKOTITA_NAME; }
        }
        public int? EIDIKOTITA_KLADOS_ID { get; set; }

        [Display(Name = "Ομάδα (πίνακες)")]
        public int? EIDIKOTITA_GROUP_ID { get; set; }

        public int? EDUCATION_CLASS { get; set; }

        [Display(Name = "Ειδικότητα ενιαία")]
        public string EIDIKOTITA_UNIFIED { get; set; }

        [Display(Name = "Κλάδος ενοποίησης")]
        public Nullable<int> KLADOS_UNIFIED { get; set; }

        public virtual SYS_KLADOS SYS_KLADOS { get; set; }

        public virtual SYS_KLADOS_ENIAIOS SYS_KLADOS_ENIAIOS { get; set; }

    }

    public class VD_EIDIKOTITESViewModel
    {
        public int EIDIKOTITA_ID { get; set; }
        public string EIDIKOTITA_CODE { get; set; }
        public string EIDIKOTITA_NAME { get; set; }
        public string EIDIKOTITA_DESC { get; set; }
        public int? EIDIKOTITA_KLADOS_ID { get; set; }
        public virtual SYS_KLADOS SYS_KLADOS { get; set; }
    }

    public class SYS_BASICEDUCATIONViewModel
    {
        public int BASIC_ID { get; set; }
        public string BASIC_TEXT { get; set; }
    }

    public class SYS_MSCPERIODSViewModel
    {
        public int MSCPERIOD_ID { get; set; }
        public string MSCPERIOD_TEXT { get; set; }
    }

    public class SYS_LANGUAGEViewModel
    {
        public int LANGUAGE_ID { get; set; }
        public string LANGUAGE_TEXT { get; set; }
    }

    public class SYS_LANGUAGELEVELViewModel
    {
        public string LEVEL { get; set; }
        public Nullable<decimal> MORIA1 { get; set; }
        public Nullable<decimal> MORIA2 { get; set; }
    }

    public class SYS_PEDAGOGICPERIODViewModel
    {
        public int PERIOD_ID { get; set; }
        public string PERIOD_TEXT { get; set; }
    }

    public class SYS_COMPUTERASEPViewModel
    {
        public int CERTIFICATE_ID { get; set; }
        public string CERTIFICATE { get; set; }
    }

    public class SYS_SCHOOLSViewModel
    {
        [Display(Name = "Κωδ. Σχολείου")]
        public int SCHOOL_ID { get; set; }

        [Display(Name = "Σχολική Μονάδα")]
        public string SCHOOL_NAME { get; set; }
        public int? SCHOOL_PERIFERIAKI_ID { get; set; }

        [Display(Name = "Περιφερειακή Ενότητα")]
        public Nullable<int> SCHOOL_PERIFERIA_ID { get; set; }
    }

    public class SYS_PERIFERIAKESViewModel
    {
        public int PERIFEREIAKI_ID { get; set; }
        public string PERIFERIAKI { get; set; }
    }

    public class SYS_PERIFERIESViewModel
    {
        public SYS_PERIFERIESViewModel()
        {
            this.SYS_DIMOS = new HashSet<SYS_DIMOS>();
        }

        [Display(Name = "Κωδ. Περιφέρειας")]
        public int PERIFERIA_ID { get; set; }

        [Display(Name = "Περιφερειακή Ενότητα")]
        public string PERIFERIA_NAME { get; set; }

        public virtual ICollection<SYS_DIMOS> SYS_DIMOS { get; set; }
    }

    public class SYS_DIMOSViewModel
    {
        public int DIMOS_ID { get; set; }
        public string DIMOS { get; set; }
        public Nullable<int> DIMOS_PERIFERIA { get; set; }
        public virtual SYS_PERIFERIES SYS_PERIFERIES { get; set; }
    }


    #region TOOLS
    //----------------------------------------------------
    // new addition 30-07-2016 for MasterChild grids
    public class PeriferiaViewModel
    {
        public int PERIFERIA_ID { get; set; }

        [Display(Name = "Περιφερειακή Ενότητα")]
        public string PERIFERIA_NAME { get; set; }
    }

    public class DimosViewModel
    {
        public int DIMOS_ID { get; set; }

        [Display(Name = "Δήμος")]
        public string DIMOS { get; set; }
        public Nullable<int> DIMOS_PERIFERIA { get; set; }
    }
    //----------------------------------------------------

    public class SchoolYearsViewModel
    {
        public int SY_ID { get; set; }


        [Required(ErrorMessage = "Υποχρεωτική συμπλήρωση")]
        [StringLength(9, ErrorMessage = "Πρέπει να είναι μέχρι 9 χαρακτήρες (π.χ.2015-2016).")]
        [Display(Name = "Σχολικό Έτος")]

        public string SY_TEXT { get; set; }

        [Required(ErrorMessage = "Υποχρεωτική συμπλήρωση")]        
        [Display(Name = "Ημερομηνία Έναρξης")]
        [DataType(DataType.Date)]
        public Nullable<System.DateTime> SY_DATESTART { get; set; }

        [Required(ErrorMessage = "Υποχρεωτική συμπλήρωση")]
        [Display(Name = "Ημερομηνία Λήξης")]
        [DataType(DataType.Date)]
        public Nullable<System.DateTime> SY_DATEEND { get; set; }

    }

    public class EidikotitesViewModel
    {
        public int EIDIKOTITA_ID { get; set; }

        [Required(ErrorMessage = "Υποχρεωτική συμπλήρωση")]
        [StringLength(50, ErrorMessage = "Πρέπει να είναι μέχρι 50 χαρακτήρες.")]
        [Display(Name = "Κωδικός")]
        public string EIDIKOTITA_CODE { get; set; }

        [Required(ErrorMessage = "Υποχρεωτική συμπλήρωση")]
        [StringLength(150, ErrorMessage = "Πρέπει να είναι μέχρι 150 χαρακτήρες.")]
        [Display(Name = "Ειδικότητα")]
        public string EIDIKOTITA_NAME { get; set; }

        [Required(ErrorMessage = "Υποχρεωτική συμπλήρωση")]
        [Display(Name = "Κλάδος")]
        public Nullable<int> EIDIKOTITA_KLADOS_ID { get; set; }

        [Display(Name = "Ομάδα")]
        public int EIDIKOTITA_GROUP_ID { get; set; }

        [Display(Name = "Βαθμίδα")]
        public int? EDUCATION_CLASS { get; set; }

        [Required(ErrorMessage = "Υποχρεωτική συμπλήρωση")]
        [Display(Name = "Ειδικότητα ενιαία")]
        public string EIDIKOTITA_UNIFIED { get; set; }

        [Required(ErrorMessage = "Υποχρεωτική συμπλήρωση")]
        [Display(Name = "Κλάδος ενοποίησης")]
        public int KLADOS_UNIFIED { get; set; }

        public virtual SYS_KLADOS SYS_KLADOS { get; set; }

        public virtual SYS_KLADOS_ENIAIOS SYS_KLADOS_ENIAIOS { get; set; }

    }

    public class sqlEidikotitesSelectorViewModel
    {
        [Display(Name = "Ειδικότητα")]
        public int EIDIKOTITA_ID { get; set; }

        [Display(Name = "Κλάδος-Ειδικότητα")]
        public string EIDIKOTITA_DESC { get; set; }

        public Nullable<int> EIDIKOTITA_GROUP_ID { get; set; }
        public Nullable<int> EIDIKOTITA_KLADOS_ID { get; set; }

    }

    public class GroupsViewModel
    {
        public int GROUP_ID { get; set; }

        [Display(Name = "Ομάδα")]
        public string GROUP_TEXT { get; set; }

        [Display(Name = "Κλάδος")]
        public Nullable<int> KLADOS_ID { get; set; }

    }

    public class ApokleismoiViewModel
    {
        public int APOKLEISMOS_ID { get; set; }

        [Required(ErrorMessage = "Υποχρεωτική συμπλήρωση")]
        [StringLength(150, ErrorMessage = "Πρέπει να είναι μέχρι 150 χαρακτήρες.")]
        [Display(Name = "Περιγραφή Αποκλεισμού")]
        public string APOKLEISMOS_TEXT { get; set; }

    }

    public class TaxFreeViewModel
    {
        public int YEAR_ID { get; set; }

        [Required(ErrorMessage = "Υποχρεωτική συμπλήρωση")]
        [StringLength(4, ErrorMessage = "Πρέπει να είναι μέχρι 4 χαρακτήρες (π.χ. 2015).")]
        [Display(Name = "Έτος Εισοδήματος")]
        public string YEAR_TEXT { get; set; }

        [Required(ErrorMessage = "Υποχρεωτική συμπλήρωση")]
        [Display(Name = "Αφορολόγητο Εισόδημα")]
        public Nullable<float> TAXFREE { get; set; }

        [Required(ErrorMessage = "Υποχρεωτική συμπλήρωση")]
        [StringLength(10, ErrorMessage = "Πρέπει να είναι μέχρι 10 χαρακτήρες.")]
        [Display(Name = "Νόμισμα")]
        public string NOMISMA { get; set; }

    }

    public class ProkirixisSchoolsViewModel
    {
        public int PS_ID { get; set; }

        [Required(ErrorMessage = "Υποχρεωτική συμπλήρωση")]
        [Display(Name = "Προκήρυξη")]
        public int PROKIRIXI_ID { get; set; }

        [Required(ErrorMessage = "Υποχρεωτική συμπλήρωση")]
        [Display(Name = "Σχολική Μονάδα")]
        public int SCHOOL_ID { get; set; }

    }

    public class ProkirixisEidikotitesViewModel
    {
        public int PSE_ID { get; set; }

        [Required(ErrorMessage = "Υποχρεωτική συμπλήρωση")]
        [Display(Name = "Προκήρυξη")]
        public int PROKIRIXI_ID { get; set; }

        [Required(ErrorMessage = "Υποχρεωτική συμπλήρωση")]
        [Display(Name = "Σχολική Μονάδα")]
        public int SCHOOL_ID { get; set; }

        [Required(ErrorMessage = "Υποχρεωτική συμπλήρωση")]
        [Display(Name = "Κλάδος-Ειδικότητα")] 
        public int EIDIKOTITA_ID { get; set; }

        public virtual SYS_EIDIKOTITES SYS_EIDIKOTITES { get; set; }
    }

    #endregion


    //-------------------------------------------------
    // This one is used for print page
    // with ReportViewer for custom parameters
    // dropdown list. 
    // NOT USED in implemented version of Print.cshtml
    //-------------------------------------------------
    public class PERIFERIESViewModel
    {
        readonly List<SYS_PERIFERIES> invoices;

        [Display(Name = "Δήμοι")]
        public int SelectedInvoiceId { get; set; }

        public string SelectedInvoice
        {
            get { return this.invoices[this.SelectedInvoiceId].PERIFERIA_NAME; }
        }

        public IEnumerable<SelectListItem> InvoiceItems
        {
            get { return new SelectList(invoices, "PERIFERIA_ID", "PERIFERIA_NAME"); }
        }

        public PERIFERIESViewModel(List<SYS_PERIFERIES> invoices)
        {
            this.invoices = invoices;
        }
    }

    //-------------------------------------------------
    // This is used for reporting pinakes of results.
    // NOT USED ANYWHERE
    //-------------------------------------------------
    public class PARAMETROI_PINAKES_REPORTS
    {
        public int? PROKIRIXI_ID { get; set; }
        public int? PERIFERIAKI_ID { get; set; }
        public int? PERIFERIA_ID { get; set; }
        public int? SCHOOL_ID { get; set; }
    }

    public class PARAMETROI_STAT_REPORTS
    {
        public int? PROKIRIXI_ID { get; set; }
        public int? SCHOOL_ID { get; set; }
    }

    public class TeacherRegistryParameters
    {
        public int? PROKIRIXI_ID { get; set; }
        public int? SCHOOL_ID { get; set; }
    }

    public class DimoiParameters
    {
        public int PERIFERIA_ID { get; set; }
    }

    public class EidikotitesParametroi
    {
        public int? PROKIRIXI_ID { get; set; }
        public int? PERIFERIA_ID { get; set; }
        public int? SCHOOL_ID { get; set; }
        public string KLADOS_NAME { get; set; }
    }

    public class DuplicateAitiseisParameters
    {
        public int? PROKIRIXI_ID { get; set; }
        public int? SCHOOL_ID { get; set; }
    }

    public class EnstaseisParametroi
    {
        public int? PROKIRIXI_ID { get; set; }
        public int? PERIFERIA_ID { get; set; }
        public int? SCHOOL_ID { get; set; }
    }

    public class TeachersMoriaParametroi
    {
        public int? PROKIRIXI_ID { get; set; }
        public int? PERIFERIAKI_ID { get; set; }
        public int? EIDIKOTITA_ID { get; set; }
        public bool? CERTIFIED { get; set; }
    }

    public class AitisiParameters
    {
        public int AITISI_ID { get; set; }
    }

}