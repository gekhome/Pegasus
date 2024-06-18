using Pegasus.DAL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Pegasus.Models
{
    public class ReeducationViewModel
    {
        public int EDUCATION_ID { get; set; }

        public Nullable<int> AITISI_ID { get; set; }

        public Nullable<int> PROKIRIXI_ID { get; set; }

        public string AFM { get; set; }

        [DataType(DataType.Date)]
        [Required(ErrorMessage = "Υποχρεωτική συμπλήρωση")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        [Display(Name = "Ημ/νία έκδοσης")]
        public Nullable<System.DateTime> CERTIFICATE_DATE { get; set; }

        [Display(Name = "Φορέας επιμόρφωσης")]
        [Required(ErrorMessage = "Υποχρεωτική συμπλήρωση")]
        [StringLength(255, ErrorMessage = "Πρέπει να είναι μέχρι 255 χαρακτήρες.")]
        [RegularExpression(@"^[Α-ΩA-Z]+[ Α-ΩA-Z-_ΪΫ.]*$", ErrorMessage = "Μόνο κεφαλαία ελληνικά, λατινικά")]
        public string CERTIFICATE_FOREAS { get; set; }

        [Display(Name = "Τίτλος πιστοποιητικού")]
        [Required(ErrorMessage = "Υποχρεωτική συμπλήρωση")]
        [StringLength(255, ErrorMessage = "Πρέπει να είναι μέχρι 255 χαρακτήρες.")]
        [RegularExpression(@"^[Α-ΩA-Z]+[ Α-ΩA-Z-_ΪΫ.]*$", ErrorMessage = "Μόνο κεφαλαία ελληνικά, λατινικά")]
        public string CERTIFICATE_TITLE { get; set; }

        [Display(Name = "Ώρες")]
        [Required(ErrorMessage = "Υποχρεωτική συμπλήρωση")]
        [Range(25, 2000, ErrorMessage = "Η τιμή πρέπει να είναι μεταξύ 25 και 2000")]
        public Nullable<int> CERTIFICATE_HOURS { get; set; }
    }

}