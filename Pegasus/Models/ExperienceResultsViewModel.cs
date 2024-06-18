using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Pegasus.Models
{
    public class ExperienceResultsViewModel
    {
        public int AITISI_ID { get; set; }
        public AitisisViewModel AITISI { get; set; }
        public SYS_EIDIKOTITESViewModel EIDIKOTITA { get; set; }

        public string LASTNAME { get; set; }
        public string FIRSTNAME { get; set; }

        #region TEACHING
        public IEnumerable<ExperienceTeachingViewModel> TEACHING_MORIA { get; set; }
        [Display(Name = "Σύνολο Μορίων Διδακτικής")]
        [DisplayFormat(DataFormatString = "{0:0.##}")]
        public double TEACHING_MORIA_FINAL { get; set; }

        #endregion

        #region FREELANCE
        [Display(Name = "Μόρια Επιτηδεύματος")]
        public double FREELANCE_MORIA { get; set; }
        [Display(Name = "Σύνολο Μορίων Επιτηδεύματος")]
        [DisplayFormat(DataFormatString = "{0:0.##}")]
        public double FREELANCE_MORIA_FINAL { get; set; }

        #endregion

        #region VOCATION
        [Display(Name = "Μόρια Επαγγελματικής")]
        public double VOCATION_MORIA { get; set; }
        [Display(Name = "Σύνολο Μορίων Επαγγελματικής")]
        [DisplayFormat(DataFormatString = "{0:0.##}")]
        public double VOCATION_MORIA_FINAL { get; set; }

        #endregion

        #region FOOTNOTE

        [Display(Name = "Μέγιστος αρ. μορίων Α/βάθμια + Β/βάθμια + Γ/βάθμια εκπαίδευση")]
        public string MORIA_MAX_TYPIKH { get; set; }

        [Display(Name = "Μέγιστος αρ. μορίων εκπαίδευσης σε ΙΕΚ, ΣΕΚ-ΠΣΕΚ")]
        public string MORIA_MAX_IEKSEKPSEK { get; set; }

        [Display(Name = "Μέγιστος αρ. μορίων άλλης άτυπης εκπαίδευσης")]
        public string MORIA_MAX_ATYPH { get; set; }

        [Display(Name = "Μέγιστος αρ. μορίων συνολικής επαγγελματικής εμπειρίας")]
        public string MORIA_MAX_WORK { get; set; }

        #endregion

        [Display(Name = "Σύνολο Μορίων Εργασιακής")]
        [DisplayFormat(DataFormatString = "{0:0.##}")]
        public double WORK_MORIA_FINAL { get; set; }

    }
}