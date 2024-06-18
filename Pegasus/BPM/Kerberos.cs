using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Pegasus.DAL;
using Pegasus.Models;
using Pegasus.BPM;

namespace Pegasus.BPM
{
    public static class Kerberos
    {
        public const int TICKET_TIMEOUT_MINUTES = 240;

        private const int _WEEKS = 32;
        private const double _CALENDARDAYSYEAR = 365.0;
        private const double _WORKINGDAYSYEAR = 300.0;

        // ΑΝΑΦΟΡΑ: ΦΕΚ 2844/23.10.2012 Αρθρο 3 παρ. 6 (σελ.43766)
        // Ώρες που αντιστοιχούν σε 1 έτος - για αναγωγές
        // ΔΙΔΑΚΤΙΚΗ ΠΡΟΫΠΗΡΕΣΙΑ
        private const double _HOURSYEAR1 = 750;      // ΠΡΩΤΟΒΑΘΜΙΑ ΕΚΠΑΙΔΕΥΣΗ
        private const double _HOURSYEAR2 = 650;      // ΔΕΥΤΕΡΟΒΑΘΜΙΑ ΕΚΠΑΙΔΕΥΣΗ
        private const double _HOURSYEAR3 = 210;      // ΤΡΙΤΟΒΑΘΜΙΑ ΕΚΠΑΙΔΕΥΣΗ
        //double _HOURSYEAR4 = 1200;      // ΙΕΚ-ΣΕΚ-ΚΕΚ
        //double _HOURSYEAR5 = 1200;      // ΑΛΛΗ ΑΤΥΠΗ ΕΚΠΑΙΔΕΥΣΗ

        // Μόρια ανά έτος για ΠΕ, ΤΕ, ΔΕ
        private const double _MORIA1 = 0.5;       // ΠΡΩΤΟΒΑΘΜΙΑ
        private const double _MORIA2 = 1.0;       // ΔΕΥΤΕΡΟΒΑΘΜΙΑ
        private const double _MORIA3 = 1.0;       // ΤΡΙΤΟΒΑΘΜΙΑ
        //double _MORIA1x = 1.0;      // ΝΗΠΙΑΓΩΓΟΙ ΕΙΔΙΚΗΣ ΑΓΩΓΗΣ ΠΑΙΡΝΟΥΝ 1 ΜΟΡΙΟ/ΕΤΟΣ ΓΙΑ ΠΡΩΤΟΒΑΘΜΙΑ

        // ΕΡΓΑΣΙΑΚΗ ΠΡΟΫΠΗΡΕΣΙΑ
        // Μόρια ανά έτος για ΠΕ, ΤΕ, ΔΕ
        private const double _WMORIAYEAR = 1.0;
        // Γενικό μέγιστο όριο μορίων
        private const double _WMORIAMAX = 10;

        // ΕΡΓΑΣΙΑΚΗ ΠΡΟΫΠΗΡΕΣΙΑ
        // Μόρια ανά έτος για ΕΜΠΕΙΡΟΤΕΧΝΙΤΕΣ
        //double _WEMORIAYEAR = 1.0;
        // Γενικό μέγιστο όριο μορίων
        //double _WEMORIAMAX = 10;

        /// <summary>
        /// Υπολογίζει τις εργάσιμες ημέρες μεταξύ δύο ημερομηνιών,
        /// δηλ. χωρίς τα Σαββατοκύριακα.
        /// </summary>
        /// <param name="initial_date"></param>
        /// <param name="final_date"></param>
        /// <returns name="daycount"></returns>
        public static int WorkingDays(DateTime initial_date, DateTime final_date)
        {
            int daycount = 0;

            DateTime date1 = initial_date;
            DateTime date2 = final_date;

            while (date1 <= date2)
            {
                switch (date1.DayOfWeek)
                {
                    case DayOfWeek.Sunday:
                    case DayOfWeek.Saturday:
                        break;
                    case DayOfWeek.Monday:
                    case DayOfWeek.Tuesday:
                    case DayOfWeek.Wednesday:
                    case DayOfWeek.Thursday:
                    case DayOfWeek.Friday:
                        daycount++;
                        break;
                    default:
                        break;
                }
                date1 = date1.AddDays(1);
            }
            return daycount;
        }


        #region Κανόνες - Διδακτικές Προϋπηρεσίες

        public static bool ValidateDatesTeaching(ViewModelTeaching exp)
        {
            DateTime _dateStart = ((DateTime)exp.DATE_FROM).Date;
            DateTime _dateEnd = ((DateTime)exp.DATE_TO).Date;
            bool valid;

            if (Common.ValidStartEndDates(_dateStart, _dateEnd))
            {
                valid = true;
            }
            else 
            {
                valid = false;
            }
            return valid;
        }

        public static bool ValidateDatesInSchoolYear(ViewModelTeaching exp)
        {
            DateTime _dateStart = ((DateTime)exp.DATE_FROM).Date;
            DateTime _dateEnd = ((DateTime)exp.DATE_TO).Date;
            int _schoolYearId = (int)exp.SCHOOL_YEAR;
            bool valid;

            valid = Common.DatesInSchoolYear(_dateStart, _dateEnd, _schoolYearId);
            return valid;
        }

        public static int ValidateHoursData(ViewModelTeaching exp)
        {
            int error = 0;
            int _HOURSWEEKMAX = 30;     // 30 hours/week
            int _WEEKSYEAR = 32;
            int _MAXHOURSYEAR = _HOURSWEEKMAX * _WEEKSYEAR;

            int _hoursWeek = exp.HOURS_WEEK ?? 0;
            int _hours = exp.HOURS ?? 0;

            bool HoursWeekPositive = (_hoursWeek > 0);
            bool HoursPositive = (_hours > 0);
            bool HoursWeekGreaterMax = (_hoursWeek > _HOURSWEEKMAX);

            // Rule 1:  HOURS_WEEK or HOURS must have a positive value
            if (!HoursWeekPositive && !HoursPositive)
            {
                error = 1;
                return error;
            }
            // Rule 2: HOURS_WEEK has a value and this must not exceed 30 h/w
            if (HoursWeekGreaterMax)
            {
                error = 2;
                return error;
            }
            // Rule 3: HOURS must not exceed 960 h/y
            if (_hours > _MAXHOURSYEAR)
            {
                error = 3;
                return error;
            }
            return error;   // no errors found
        }

        public static int ValidateDocumentTeaching(ViewModelTeaching exp)
        {
            int error = 0;
            bool BeforePtyxio = false;

            bool DocProtocolEmpty = string.IsNullOrEmpty(exp.DOC_PROTOCOL);
            bool DocOriginEmpty = string.IsNullOrEmpty(exp.DOC_ORIGIN);
            bool DocValidIsFalse = (exp.DOC_VALID == false);
            bool DocCommentIsEmpty = string.IsNullOrEmpty(exp.DOC_COMMENT);
            bool DocValidIsTrue = (exp.DOC_VALID == true);

            using (var db = new PegasusDBEntities())
            {
                var data = (from d in db.AITISIS where d.AITISI_ID == exp.AITISI_ID select d).FirstOrDefault();
                if (data != null)
                {
                    BeforePtyxio = exp.DATE_FROM.Value.CompareTo((DateTime)data.PTYXIO_DATE) < 0;
                }

                // Rule 1: Document info must not be empty
                if (DocProtocolEmpty && DocOriginEmpty)
                {
                    error = 1;
                    return error;
                }
                // Rule 2: Document Protocol must not be empty
                if (DocProtocolEmpty)
                {
                    error = 2;
                    return error;
                }
                // Rule 3: Document Origin must not be empty
                if (DocOriginEmpty)
                {
                    error = 3;
                    return error;
                }
                // Rule 4: Document Valid = false must have a reason specified in comment area
                if (DocValidIsFalse && DocCommentIsEmpty)
                {
                    error = 4;
                    return error;
                }
                // Rule 5: Experience must be after graduation date
                if (DocValidIsTrue && BeforePtyxio)
                {
                    error = 5;
                    return error;
                }
                return error;   // no errors found
            }
        }

        public static string ValidateFieldsTeaching(ViewModelTeaching exp)
        {
            string err_msg = "";
            int err_num;

            if (!ValidateDatesTeaching(exp))
                err_msg += "->Η αρχική ημ/νια είναι μεγαλύτερη της τελικής.";
            if (!ValidateDatesInSchoolYear(exp))
                err_msg += "->Οι ημερομηνίες δεν είναι συμβατές με το σχολικό έτος.";

            err_num = ValidateHoursData(exp);
            switch (err_num)
            {
                case 1: err_msg += "->Ένα από τα δύο πεδία ωρών πρέπει να έχει τιμή > 0."; break;
                case 2: err_msg += "->Οι ώρες/εβδ δεν μπορεί να είναι μεγαλύτερες των 30."; break;
                case 3: err_msg += "->Το ανώτατο όριο ωρών είναι 960 ανά σχολικό έτος."; break;
                default: break;
            }

            err_num = ValidateDocumentTeaching(exp);
            switch (err_num)
            {
                case 1: err_msg += "->Οι πληροφορίες του εγγράφου πρέπει να συμπληρωθούν."; break;
                case 2: err_msg += "->Το πεδίο πρωτοκόλλου πρέπει να συμπληρωθεί."; break;
                case 3: err_msg += "->Το πεδίο υπηρεσίας πρέπει να συμπληρωθεί."; break;
                case 4: err_msg += "->Το έγγραφο δεν σημειώθηκε ως έγκυρο.Πρέπει να γρσφεί η αιτία."; break;
                case 5: err_msg += "->Η ημ/νία έναρξης της προϋπηρεσίας είναι πριν την ημ/νία κτήσης πτυχίου."; break;
                default: break;
            }
            return err_msg;
        }

        public static string PreventDuplicateTeaching(ViewModelTeaching exp, int aitisiId)
        {
            string err_msg = "";

            using (var db = new PegasusDBEntities())
            {
                int count = (from d in db.EXP_TEACHING
                             where d.AITISI_ID == aitisiId && d.TEACH_TYPE == exp.TEACH_TYPE && d.SCHOOL_YEAR == exp.SCHOOL_YEAR
                                && d.DATE_FROM == exp.DATE_FROM && d.DATE_TO == exp.DATE_TO
                                && (d.HOURS_WEEK == exp.HOURS_WEEK || d.HOURS == exp.HOURS)
                             select d).Count();
                if (count > 0)
                {
                    err_msg = "Η προύπηρεσία αυτή φαίνεται να υπάρχει ήδη. Εάν δεν πρόκειται για διπλότυπη αλλάξτε την αρχική ημ/νία κατά μία ημέρα.";
                }
                return err_msg;
            }
        }

        #endregion


        #region Κανόνες - Επαγγελματικές Προϋπηρεσίες

        public static bool ValidateDatesVocational(ViewModelVocational exp)
        {
            DateTime _dateStart = ((DateTime)exp.DATE_FROM).Date;
            DateTime _dateEnd = ((DateTime)exp.DATE_TO).Date;
            bool valid;

            if (Common.ValidStartEndDates(_dateStart, _dateEnd))
            {
                valid = true;
            }
            else
            {
                valid = false;
            }
            return valid;
        }

        public static int ValidateDaysData(ViewModelVocational exp)
        {
            int error = 0;

            DateTime _dateInitial = (DateTime)exp.DATE_FROM;
            DateTime _dateFinal = (DateTime)exp.DATE_TO;

            double days = Common.DaysDiff(_dateInitial, _dateFinal);
            double _DAYSMAX = days+1;

            double _daysManual = exp.DAYS_MANUAL ?? 0;
            bool DaysManualIsEmpty = (_daysManual == 0);
            bool DaysManualIsPositive = (_daysManual > 0);

            // Rule 1: Days by user must be a positive number
            if(!DaysManualIsEmpty && !DaysManualIsPositive)
            {
                error = 1;
                return error;
            }
            // Rule 2: Days by user cannot exceed the days between the entered dates
            if (!DaysManualIsEmpty && _daysManual > _DAYSMAX)
            {
                error = 2;
                return error;
            }
            return error;   // no errors found
        }

        public static int ValidateDocumentVocational(ViewModelVocational exp)
        {
            int error = 0;
            bool DocProtocolEmpty = string.IsNullOrEmpty(exp.DOC_PROTOCOL);
            bool DocOriginEmpty = string.IsNullOrEmpty(exp.DOC_ORIGIN);
            bool DocValidIsFalse = (exp.DOC_VALID == false);
            bool DocCommentIsEmpty = string.IsNullOrEmpty(exp.DOC_COMMENT);
            bool DocValidIsTrue = (exp.DOC_VALID == true);
            bool ExpBeforePtyxio = false;

            using (var db = new PegasusDBEntities())
            {
                var data = (from d in db.AITISIS where d.AITISI_ID == exp.AITISI_ID select d).FirstOrDefault();
                if (data != null)
                {
                    ExpBeforePtyxio = exp.DATE_FROM.Value.CompareTo((DateTime)data.PTYXIO_DATE) < 0;
                }

                // Rule 1: Document info must not be empty
                if (DocProtocolEmpty && DocOriginEmpty)
                {
                    error = 1;
                    return error;
                }
                // Rule 2: Document Protocol must not be empty
                if (DocProtocolEmpty)
                {
                    error = 2;
                    return error;
                }
                // Rule 3: Document Origin must not be empty
                if (DocOriginEmpty)
                {
                    error = 3;
                    return error;
                }
                // Rule 4: Document Valid = false must have a reason specified in comment area
                if (DocValidIsFalse && DocCommentIsEmpty)
                {
                    error = 4;
                    return error;
                }
                // Rule 5: Experience must be after graduation date
                if (DocValidIsTrue && ExpBeforePtyxio)
                {
                    error = 5;
                    return error;
                }
                return error;   // no errors found
            }
        }

        public static string ValidateFieldsVocational(ViewModelVocational exp)
        {
            string err_msg = "";
            int err_num;

            if (!ValidateDatesVocational(exp))
                err_msg += "->Η αρχική ημ/νια είναι μεγαλύτερη της τελικής.";

            err_num = ValidateDaysData(exp);
            switch (err_num)
            {
                case 1: err_msg += "->Οι ημέρες πρέπει να είναι θετικός αριθμός."; break;
                case 2: err_msg += "->Οι ημέρες δεν μπορεί να υπερβαίνουν το χρονικό διάστημα των ημερομηνιών."; break;
                default: break;
            }

            err_num = ValidateDocumentVocational(exp);
            switch (err_num)
            {
                case 1: err_msg += "->Οι πληροφορίες του εγγράφου πρέπει να συμπληρωθούν."; break;
                case 2: err_msg += "->Το πεδίο πρωτοκόλλου πρέπει να συμπληρωθεί."; break;
                case 3: err_msg += "->Το πεδίο υπηρεσίας πρέπει να συμπληρωθεί."; break;
                case 4: err_msg += "->Το έγγραφο δεν σημειώθηκε ως έγκυρο.Πρέπει να γρσφεί η αιτία."; break;
                case 5: err_msg += "->Η ημ/νία έναρξης της προϋπηρεσίας είναι πριν την ημ/νία κτήσης πτυχίου."; break;
                default: break;
            }
            return err_msg;
        }

        public static string PreventDuplicateVocational(ViewModelVocational exp, int aitisiId)
        {
            string err_msg = "";

            using (var db = new PegasusDBEntities())
            {
                int count = (from d in db.EXP_VOCATIONAL
                             where d.AITISI_ID == aitisiId && d.DATE_FROM == exp.DATE_FROM && d.DATE_TO == exp.DATE_TO && d.DAYS_MANUAL == exp.DAYS_MANUAL
                             select d).Count();
                if (count > 0)
                {
                    err_msg = "Η προύπηρεσία αυτή φαίνεται να υπάρχει ήδη. Εάν δεν πρόκειται για διπλότυπη αλλάξτε την αρχική ημ/νία κατά μία ημέρα.";
                }
                return err_msg;
            }
        }

        #endregion


        #region Κανόνες - Ελευθερο Επάγγελμα Προϋπηρεσίες

        public static bool ValidateDatesFreelance(ViewModelFreelance exp)
        {
            DateTime _dateStart = ((DateTime)exp.DATE_FROM).Date;
            DateTime _dateEnd = ((DateTime)exp.DATE_TO).Date;
            bool valid;

            if (Common.ValidStartEndDates(_dateStart, _dateEnd))
            {
                valid = true;
            }
            else
            {
                valid = false;
            }
            return valid;
        }

        public static int ValidateIncomeDates(ViewModelFreelance exp)
        {
            int error = 0;

            int yearIndex = exp.INCOME_YEAR ?? 0;

            if (yearIndex == 0)
            {
                error = 1;
                return error;
            }
            // Index found
            string syear = Common.GetFinancialYear(yearIndex);
            string sdateFrom = "01/01/" + syear;
            string sdateTo = "31/12/" + syear;

            DateTime _dateFrom = (DateTime.Parse(sdateFrom)).Date;
            DateTime _dateTo = (DateTime.Parse(sdateTo)).Date;

            bool DatesNotInYear = ((DateTime)exp.DATE_FROM < _dateFrom || (DateTime)exp.DATE_TO > _dateTo);

            if(DatesNotInYear)
            {
                error = 2;
                return error;
            }
            return error;   // no error found
        }

        public static int ValidateDocumentFreelance(ViewModelFreelance exp)
        {
            int error = 0;
            bool DocProtocolEmpty = string.IsNullOrEmpty(exp.DOC_PROTOCOL);
            bool DocOriginEmpty = string.IsNullOrEmpty(exp.DOC_ORIGIN);
            bool DocValidIsFalse = (exp.DOC_VALID == false);
            bool DocCommentIsEmpty = string.IsNullOrEmpty(exp.DOC_COMMENT);
            bool DocValidIsTrue = (exp.DOC_VALID == true);
            bool ExpBeforePtyxio = false;

            using (var db = new PegasusDBEntities())
            {
                var data = (from d in db.AITISIS where d.AITISI_ID == exp.AITISI_ID select d).FirstOrDefault();
                if (data != null)
                {
                    ExpBeforePtyxio = exp.DATE_FROM.Value.CompareTo((DateTime)data.PTYXIO_DATE) < 0;
                }

                // Rule 1: Document info must not be empty
                if (DocProtocolEmpty && DocOriginEmpty)
                {
                    error = 1;
                    return error;
                }
                // Rule 2: Document Protocol must not be empty
                if (DocProtocolEmpty)
                {
                    error = 2;
                    return error;
                }
                // Rule 3: Document Origin must not be empty
                if (DocOriginEmpty)
                {
                    error = 3;
                    return error;
                }
                // Rule 4: Document Valid = false must have a reason specified in comment area
                if (DocValidIsFalse && DocCommentIsEmpty)
                {
                    error = 4;
                    return error;
                }
                // Rule 5: Experience must be after graduation date
                if (DocValidIsTrue && ExpBeforePtyxio)
                {
                    error = 5;
                    return error;
                }
                return error;   // no errors found
            }
        }

        public static string ValidateFieldsFreelance(ViewModelFreelance exp)
        {
            string err_msg = "";
            int err_num;

            if (!ValidateDatesFreelance(exp))
                err_msg += "->Η αρχική ημ/νια είναι μεγαλύτερη της τελικής.";

            err_num = ValidateIncomeDates(exp);
            switch (err_num)
            {
                case 1: err_msg += "->Το έτος εισοδήματος πρέπει να έχει τιμή."; break;
                case 2: err_msg += "->Οι ημερομηνίες δεν είναι συμβατές με το έτος εισοδήματος."; break;
                default: break;
            }

            err_num = ValidateDocumentFreelance(exp);
            switch (err_num)
            {
                case 1: err_msg += "->Οι πληροφορίες του εγγράφου πρέπει να συμπληρωθούν."; break;
                case 2: err_msg += "->Το πεδίο πρωτοκόλλου πρέπει να συμπληρωθεί."; break;
                case 3: err_msg += "->Το πεδίο υπηρεσίας πρέπει να συμπληρωθεί."; break;
                case 4: err_msg += "->Το έγγραφο δεν σημειώθηκε ως έγκυρο.Πρέπει να γρσφεί η αιτία."; break;
                case 5: err_msg += "->Η ημ/νία έναρξης της προϋπηρεσίας είναι πριν την ημ/νία κτήσης πτυχίου."; break;
                default: break;
            }
            return err_msg;
        }

        public static string PreventDuplicateFreelance(ViewModelFreelance exp, int aitisiId)
        {
            string err_msg = "";

            using (var db = new PegasusDBEntities())
            {
                int count = (from d in db.EXP_FREELANCE
                            where d.AITISI_ID == aitisiId && d.INCOME_YEAR == exp.INCOME_YEAR && d.INCOME == exp.INCOME && d.DATE_FROM == exp.DATE_FROM && d.DATE_TO == exp.DATE_TO
                            select d).Count();
                if (count > 0)
                {
                    err_msg = "Η προύπηρεσία αυτή φαίνεται να υπάρχει ήδη. Εάν δεν πρόκειται για διπλότυπη αλλάξτε την αρχική ημ/νία κατά μία ημέρα.";
                }
                return err_msg;
            }
        }

        #endregion


        #region Μόρια - Διδακτικές Προϋπηρεσίες

        public static double MoriaTeaching(EXP_TEACHING e)
        {
            double moria = 0;
            if (e.DOC_VALID == false) return moria;

            DateTime d1 = (DateTime)e.DATE_FROM;
            DateTime d2 = (DateTime)e.DATE_TO;
            
            double hw = e.HOURS_WEEK ?? 0;
            double N = e.HOURS ?? 0;
            int domi = e.TEACH_TYPE ?? 0;

            int aitisiId = e.AITISI_ID;

            using (var db = new PegasusDBEntities())
            {
                var aitisi = (from a in db.AITISIS where a.AITISI_ID == aitisiId select a).FirstOrDefault();

                int eidikotitaId = (int)aitisi.EIDIKOTITA;

                // user entered weekly hours which takes precedence            
                if (hw > 0)
                {
                    double _weeks = WorkingDays(d1, d2) / 5.0;
                    N = Common.Min((float)(hw * _weeks), (float)(hw * _WEEKS));
                }

                // ΟΛΟΙ ΟΙ ΚΛΑΔΟΙ ΜΟΡΙΟΔΟΤΟΥΝΤΑΙ ΤΟ ΙΔΙΟ (ΠΕ, ΤΕ, ΔΕ, ΔΕ.ΕΤ)
                switch (domi)
                {
                    case 1:     // ΠΡΩΤΟΒΑΘΜΙΑ
                        {
                            double hours = (float)(N / _HOURSYEAR1);
                            float _moria = (float)(_MORIA1 * hours);
                            float _moriaMax = (float)1.0;
                            moria = Common.Min(_moria, _moriaMax);
                        }; break;
                    case 2:     // ΔΕΥΤΕΡΟΒΑΘΜΙΑ
                        {
                            double hours = (float)(N / _HOURSYEAR2);
                            float _moria = (float)(_MORIA2 * hours);
                            float _moriaMax = (float)1.0;
                            moria = Common.Min(_moria, _moriaMax);
                        }; break;
                    case 3:     // ΤΡΙΤΟΒΑΘΜΙΑ
                        {
                            double hours = (float)(N / _HOURSYEAR3);
                            float _moria = (float)(_MORIA3 * hours);
                            float _moriaMax = (float)1.0;
                            moria = Common.Min(_moria, _moriaMax);
                        }; break;
                    case 4:     // ΙΕΚ, ΣΕΚ, ΠΣΕΚ
                        {
                            double hours = N;
                            float _moria = (float)(N / 200.0);
                            float _moriaMax = (float)5.0;
                            moria = Common.Min(_moria, _moriaMax);
                        }; break;
                    case 5:     // ΑΛΛΗ ΑΤΥΠΗ ΕΚΠΑΙΔΕΥΣΗ
                        {
                            double hours = N;
                            float _moria = (float)(N / 250.0);
                            float _moriaMax = (float)5.0;
                            moria = Common.Min(_moria, _moriaMax);
                        }; break;
                    default: break;
                }
                return moria;
            }
        }

        #endregion Διδακτικές


        #region Μόρια - Επαγγελματικές Προϋπηρεσίες

        public static double SetDaysAutoVocational(EXP_VOCATIONAL e)
        {
            DateTime d1 = (DateTime)e.DATE_FROM;
            DateTime d2 = (DateTime)e.DATE_TO;

            double days = (double)Common.DaysDiff(d1, d2) + 1;
            return days;
        }

        public static double SetDaysAutoVocational(ViewModelVocational e)
        {
            DateTime d1 = (DateTime)e.DATE_FROM;
            DateTime d2 = (DateTime)e.DATE_TO;

            double days = (double)Common.DaysDiff(d1, d2) + 1;
            return days;
        }

        public static double MoriaVocational(EXP_VOCATIONAL e)
        {
            double moria = 0;
            if (e.DOC_VALID == false) return moria;

            double N = SetDaysAutoVocational(e);
            double days_manual = e.DAYS_MANUAL ?? 0;

            // manual days override calculated value and max working days = 300 (ημερομήσθια)
            if (days_manual > 0)
            {
                N = days_manual;
                double _moria = (N / _WORKINGDAYSYEAR) * _WMORIAYEAR;
                moria = Common.Min((float)_moria, (float)_WMORIAMAX);
                return moria;
            }
            else
            {
                double _moria = (N / _CALENDARDAYSYEAR) * _WMORIAYEAR;
                moria = Common.Min((float)_moria, (float)_WMORIAMAX);
                return moria;
            }
        }

        #endregion Επαγγελματικές


        #region Μόρια - Ελεύθερο Επάγγελμα Προϋπηρεσίες

        public static double SetDaysAutoFreelance(EXP_FREELANCE e)
        {
            DateTime d1 = (DateTime)e.DATE_FROM;
            DateTime d2 = (DateTime)e.DATE_TO;

            double days = (double)Common.DaysDiff(d1, d2) + 1;
            return days;
        }

        public static double SetDaysAutoFreelance(ViewModelFreelance e)
        {
            DateTime d1 = (DateTime)e.DATE_FROM;
            DateTime d2 = (DateTime)e.DATE_TO;

            double days = (double)Common.DaysDiff(d1, d2) + 1;
            return days;
        }

        public static double MoriaFreelance(EXP_FREELANCE e)
        {
            double moria = 0;
            if (e.DOC_VALID == false) return moria;

            double N = SetDaysAutoFreelance(e);
            double days_manual = e.DAYS_MANUAL ?? 0;

            if (e.INCOME < e.INCOME_TAXFREE) return moria;

            // manual days override calculated value
            if (days_manual > 0)
            {
                N = days_manual;
            }
            double _moria = (N / _CALENDARDAYSYEAR) * _WMORIAYEAR;
            moria = Common.Min((float)_moria, (float)_WMORIAMAX);

            return moria;
        }

        #endregion Ελεύθερο Επάγγελμα


        #region Γενικοί Έλεγχοι

        public static bool existsUsername(string username)
        {
            using (var db = new PegasusDBEntities())
            {
                var userTeachers = db.USER_TEACHERS.Where(u => u.USERNAME == username).FirstOrDefault();
                var userAdmins = db.USER_ADMINS.Where(u => u.USERNAME == username).FirstOrDefault(); ;
                var userSchools = db.USER_SCHOOLS.Where(u => u.USERNAME == username).FirstOrDefault();

                return (userTeachers != null || userAdmins != null || userSchools != null);
            }
        }

        public static int CountSpaces(string s)
        {
            int countSpaces = s.Count(char.IsWhiteSpace);

            return countSpaces;
        }

        public static bool CanDeleteUpload(int uploadId)
        {
            using (var db = new PegasusDBEntities())
            {
                var data = (from d in db.UploadEnstaseisFiles where d.UPLOAD_ID == uploadId select d).Count();
                if (data > 0)
                    return false;
                return true;
            }
        }

        public static bool CanDeleteUploadGeneral(int uploadId)
        {
            using (var db = new PegasusDBEntities())
            {
                var data = (from d in db.UploadGeneralFiles where d.UploadID == uploadId select d).Count();
                if (data > 0)
                    return false;
                return true;
            }
        }

        public static bool CanDeleteUploadTeaching(int uploadId)
        {
            using (var db = new PegasusDBEntities())
            {
                var data = (from d in db.UploadTeachingFiles where d.UploadID == uploadId select d).Count();
                if (data > 0)
                    return false;
                return true;
            }
        }

        public static bool CanDeleteUploadVocation(int uploadId)
        {
            using (var db = new PegasusDBEntities())
            {
                var data = (from d in db.UploadVocationFiles where d.UploadID == uploadId select d).Count();
                if (data > 0)
                    return false;
                return true;
            }
        }

        public static bool ValidFileExtension(string extension)
        {
            string[] extensions = { ".PDF", ".JPG" };

            List<string> allowed_extensions = new List<string>(extensions);

            if (allowed_extensions.Contains(extension.ToUpper()))
                return true;
            return false;
        }

        #endregion


    }
}