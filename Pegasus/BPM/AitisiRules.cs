using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Pegasus.DAL;
using Pegasus.Models;
using Pegasus.BPM;

namespace Pegasus.BPM
{
    public static class AitisiRules
    {
        public static int CalcAge(AITISIS aitisi)
        {
            DateTime BirthDate;
            DateTime AitisiDate = ((DateTime)aitisi.AITISI_DATE).Date;
            int age = 0;

            // Βρίσκουμε την ημ/νια γέννησης του εκπαιδευτικού της τρέχουσας αίτησης
            using (var db = new PegasusDBEntities())
            {
                var dt1 = (from t in db.TEACHERS where t.AFM == aitisi.AFM select t).FirstOrDefault();

                if (dt1 == null) return age;

                if (dt1.BIRTHDATE != null)
                {
                    BirthDate = ((DateTime)dt1.BIRTHDATE).Date;
                    age = Common.YearsDiff(BirthDate, AitisiDate);
                }
                return age;
            }
        }

        public static int CalcAge(AitisisViewModel aitisi)
        {
            DateTime BirthDate;
            DateTime AitisiDate = ((DateTime)aitisi.AITISI_DATE).Date;
            int age = 0;

            // Βρίσκουμε την ημ/νια γέννησης του εκπαιδευτικού της τρέχουσας αίτησης
            using (var db = new PegasusDBEntities())
            {
                var dt1 = (from t in db.TEACHERS where t.AFM == aitisi.AFM select t).FirstOrDefault();

                if (dt1.BIRTHDATE != null)
                {
                    BirthDate = ((DateTime)dt1.BIRTHDATE).Date;
                    age = Common.YearsDiff(BirthDate, AitisiDate);
                }
                return age;
            }
        }

        public static string ValidateFields(AitisisViewModel a, bool isnew)
        {
            int err = 0;
            int tempi = 0;
            string errMsg = "";

            err += ValidateSocialGroup(a);
            switch (err)
            {
                case 1:
                    errMsg += "->Αρ.Πρωτ. Πιστοποιητικού: Πρέπει να συμληρωθεί. ";
                    break;
                case 2:
                    errMsg += "->Εκδούσα Αρχή: Πρέπει να συμπληρωθεί. ";
                    break;
                case 3:
                    errMsg += "->Αρ.Πρωτ. και Εκδούσα Αρχή: Πρέπει να συμπληρωθούν. ";
                    break;
                case 4:
                    errMsg += "->Κοινωνική Ομάδα: Πρέπει να γίνει επιλογή. ";
                    break;
                default:
                    break;
            }

            if (ValidateBasicEducation(a) == false)
            {
                errMsg += "->Βασική Εκπαίδευση: Πρέπει να γίνει επιλογή. ";
                err += 1;
            }
            if (ValidatePtyxioDate(a) == false)
            {
                errMsg += "->Ημ/νια κτήσης πτυχίου: Η τιμή της είναι εκτός λογικών ορίων." + Environment.NewLine;
                err += 1;
            }

            tempi = ValidateMsc(a);
            switch (tempi)
            {
                case 1:
                    errMsg += "->Διάρκεια Μεταπτυχιακού: Πρέπει να γίνει επιλογή. ";
                    break;
                case 2:
                    errMsg += "->Τίτλος Μεταπτυχιακού: Πρέπει να συμπληρωθεί. ";
                    break;
                case 3:
                    errMsg += "->Διάρκεια και Τίτλος Μεταπτυχιακού: Πρέπει να συμπληρωθούν. ";
                    break;
                case 4:
                    errMsg += "->Μεταπτυχιακό: Πρέπει να γίνει επιλογή. ";
                    break;
                default:
                    break;
            }
            err += tempi;

            tempi = ValidatePhd(a);
            switch (tempi)
            {
                case 1:
                    errMsg += "->Τίτλος Διδακτορικού: Πρέπει να συμπληρωθεί. ";
                    break;
                case 2:
                    errMsg += "->Διδακτορικό: Πρέπει να γίνει επιλογή. ";
                    break;
                default:
                    break;
            }
            err += tempi;

            tempi = ValidatePed(a);
            switch (tempi)
            {
                case 1:
                    errMsg += "->Διάρκεια Παιδαγωγικού: Πρέπει να γίνει επιλογή. ";
                    break;
                case 2:
                    errMsg += "->Τίτλος Παιδαγωγικού: Πρέπει να συμπληρωθεί. ";
                    break;
                case 3:
                    errMsg += "->Διάρκεια και Τίτλος Παιδαγωγικού: Πρέπει να συμπληρωθούν. ";
                    break;
                case 4:
                    errMsg += "->Παιδαγωγικό: Πρέπει να γίνει επιλογή. ";
                    break;
                default:
                    break;
            }
            err += tempi;

            tempi = ValidateAedMsc(a);
            switch (tempi)
            {
                case 1:
                    errMsg += "->Τίτλος Μεταπτυχιακού: Πρέπει να συμπληρωθεί. ";
                    break;
                case 2:
                    errMsg += "->Μεταπτυχιακό: Πρέπει να γίνει επιλογή. ";
                    break;
                default:
                    break;
            }
            err += tempi;

            tempi = ValidateAedPhd(a);
            switch (tempi)
            {
                case 1:
                    errMsg += "->Τίτλος Διδακτορικού: Πρέπει να συμπληρωθεί. ";
                    break;
                case 2:
                    errMsg += "->Μεταπτυχιακό: Πρέπει να γίνει επιλογή. ";
                    break;
                default:
                    break;
            }
            err += tempi;

            tempi = ValidateLanguage(a);
            switch (tempi)
            {
                case 1:
                    errMsg += "->Επίπεδο γνώσης γλώσσας: Πρέπει να γίνει επιλογή." + Environment.NewLine;
                    break;
                case 2:
                    errMsg += "->Γλώσσα: Πρέπει να επιλεγεί ή να πληκτρολογηθεί τιμή. ";
                    break;
                case 3:
                    errMsg += "->Τίτλος πιστοποιητικού γλώσσας: Πρέπει να συμπληρωθεί. ";
                    break;
                case 4:
                    errMsg += "->Γλώσσα και Τίτλος πιστοποιητικού: Πρέπει να συμπληρωθούν. ";
                    break;
                default:
                    break;
            }
            err += tempi;

            if (ValidateComputer(a) == false)
            {
                errMsg += "->Πιστοποιητικό Η/Υ: Πρέπει να γίνει επιλογή. ";
                err += 1;
            }

            tempi = ValidateEpimorfosi1(a);
            switch (tempi)
            {
                case 1:
                    errMsg += "->Επιμόρφωση (1): Πρέπει να γίνει επιλογή. ";
                    break;
                case 2:
                    errMsg += "Ώρες Επιμόρφωσης (1): Πρέπει να καταχωρηθεί τιμή. ";
                    break;
                default:
                    break;
            }
            err += tempi;

            tempi = ValidateEpimorfosi2(a);
            switch (tempi)
            {
                case 1:
                    errMsg += "->Επιμόρφωση (2): Πρέπει να γίνει επιλογή. ";
                    break;
                case 2:
                    errMsg += "->Ώρες Επιμόρφωσης (2): Πρέπει να καταχωρηθεί τιμή. ";
                    break;
                default:
                    break;
            }
            err += tempi;

            tempi = ValidateEpimorfosi3(a);
            switch (tempi)
            {
                case 1:
                    errMsg += "->Επιμόρφωση (3): Πρέπει να γίνει επιλογή. ";
                    break;
                case 2:
                    errMsg += "->Ώρες Επιμόρφωσης (3): Πρέπει να καταχωρηθεί τιμή. ";
                    break;
                default:
                    break;
            }
            err += tempi;

            if (!ValidateEpagelmaStatus(a))
            {
                errMsg += "->Επαγγ. Ιδιότητα: Πρέπει να γίνει επιλογή.";
                err += 1;
            }

            if (isnew)
            {
                int prokirixiId = Common.GetOpenProkirixiID();
                if (!ValidatePeriferia(a, prokirixiId))
                {
                    errMsg += "-> Περιφέρεια & Ειδικότητα: Έχετε ήδη καταχωρήσει αίτηση για αυτή την περιφέρεια με αυτή την ειδικότητα. ";
                    err += 1;
                }
            }

            if (!ValidatePtyxioGrade(a))
            {
                errMsg += "Ο βαθμός πτυχίου πρέπει να είναι μεταξύ 5 και 20. ";
                err += 1;
            }

            if (!AitisiValidAge(a))
            {
                errMsg += "Η ηλικία είναι εκτός λογικών ορίων. Διορθώστε πρώτα την ημ/νία γέννησης στα ατομικά στοιχεία. ";
                err += 1;
            }

            return errMsg;
        }


        #region AFM validation

        /// ------------------------------------------------------------------------
        /// CheckAFM: Ελέγχει αν ένα ΑΦΜ είναι σωστό
        /// Το ΑΦΜ που θα ελέγξουμε
        /// true = ΑΦΜ σωστό, false = ΑΦΜ Λάθος
        /// Αυτή είναι η χρησιμοποιούμενη μεθοδος.
        /// Προσθήκη: Αποκλεισμός όταν όλα τα ψηφία = 0 (ο αλγόριθμος τα δέχεται!)
        /// Ημ/νια: 12/3/2013
        /// ------------------------------------------------------------------------
        public static bool CheckAFM(string cAfm)
        {
            int nExp = 1;
            // Ελεγχος αν περιλαμβάνει μόνο γράμματα
            try { long nAfm = Convert.ToInt64(cAfm); }

            catch (Exception) { return false; }

            // Ελεγχος μήκους ΑΦΜ
            if (string.IsNullOrWhiteSpace(cAfm))
            {
                return false;
            }

            cAfm = cAfm.Trim();
            int nL = cAfm.Length;
            if (nL != 9) return false;

            // Έλεγχος αν όλα τα ψηφία είναι 0
            var count = cAfm.Count(x => x == '0');
            if (count == cAfm.Length) return false;

            //Υπολογισμός αν το ΑΦΜ είναι σωστό

            int nSum = 0;
            int xDigit = 0;
            int nT = 0;

            for (int i = nL - 2; i >= 0; i--)
            {
                xDigit = int.Parse(cAfm.Substring(i, 1));
                nT = xDigit * (int)(Math.Pow(2, nExp));
                nSum += nT;
                nExp++;
            }

            xDigit = int.Parse(cAfm.Substring(nL - 1, 1));

            nT = nSum / 11;
            int k = nT * 11;
            k = nSum - k;
            if (k == 10) k = 0;
            if (xDigit != k) return false;

            return true;
        }

        #endregion


        #region Age validation (Teacher Rule)

        public static bool ValidateBirthdate(TeacherViewModel teacher)
        {
            if (teacher.BIRTHDATE == null) return false;

            DateTime _birthdate = (DateTime)teacher.BIRTHDATE;

            if (!ValidBirthDate(_birthdate)) return false;
            else return true;
        }

        public static bool ValidBirthDate(DateTime birthdate)
        {
            bool result = true;
            int maxAge = 75;
            int minAge = 18;

            DateTime minDate = DateTime.Today.Date.AddYears(-maxAge);
            DateTime maxDate = DateTime.Today.Date.AddYears(-minAge);

            if (birthdate >= minDate && birthdate <= maxDate)
                result = true;
            else
                result = false;
            return result;
        }

        public static bool AitisiValidAge(AitisisViewModel aitisi)
        {
            string afm = aitisi.AFM;

            using (var db = new PegasusDBEntities())
            {
                DateTime? birthdate = (from d in db.TEACHERS where d.AFM == afm select d.BIRTHDATE).FirstOrDefault();

                if (birthdate == null) return false;
                else if (!ValidBirthDate((DateTime)birthdate)) return false;
                else return true;
            }
        }

        #endregion


        #region Social Group Rule

        public static int ValidateSocialGroup(AitisisViewModel aitisi)
        {
            int error_code = 0;     // no errors

            bool TrueSocialGroup = (aitisi.SOCIALGROUP > 0);
            bool EmptySocialGroup = !TrueSocialGroup;
            bool EmptyProtocol = string.IsNullOrEmpty(aitisi.SOCIALGROUP_PROTOCOL);
            bool EmptyService = string.IsNullOrEmpty(aitisi.SOCIALGROUP_YPIRESIA);

            if(TrueSocialGroup)
            {
                if (EmptyProtocol && !EmptyService)
                {
                    error_code = 1;
                }
                else if (!EmptyProtocol && EmptyService)
                {
                    error_code = 2;
                }
                else if (EmptyProtocol && EmptyService)
                {
                    error_code = 3;
                }
                else error_code = 0;
            }

            if (EmptySocialGroup)
            {
                if (!EmptyProtocol || !EmptyService)
                {
                    error_code = 4;
                }
            }
            return error_code;
        }

        #endregion


        #region Klados (Basic Education) Rule

        public static bool ValidateBasicEducation(AitisisViewModel aitisi)
        {
            int et = 4;     // Εμπειροτεχνίτες

            if (aitisi.KLADOS == et)
            {
                if (aitisi.BASIC_EDUCATION == null)
                {
                    return false;
                }
                else return true;
            }
            else return true;
        }

        #endregion


        #region Ptyxia Rules

        public static bool ValidatePtyxioDate(AitisisViewModel aitisi)
        {
            DateTime pdate = (DateTime)aitisi.PTYXIO_DATE;

            bool result = true;
            int maxAge = 75;

            DateTime minDate = DateTime.Today.Date.AddYears(-maxAge);
            DateTime maxDate = DateTime.Today.Date;

            if (pdate >= minDate && pdate <= maxDate)
                result = true;
            else
                result = false;
            return result;

        }

        public static bool ValidatePtyxioGrade(AitisisViewModel aitisi)
        {
            bool validGrade = aitisi.PTYXIO_BATHMOS >= 5 && aitisi.PTYXIO_BATHMOS <= 20;

            if (!validGrade)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public static int ValidateMsc(AitisisViewModel aitisi)
        {
            int error_code = 0;

            bool TrueMsc = (aitisi.MSC == true);
            bool EmptyDiarkeia = !(aitisi.MSC_DIARKEIA > 0);
            bool EmptyTitlos = string.IsNullOrEmpty(aitisi.MSC_TITLOS);

            if (TrueMsc)
            {
                if (EmptyDiarkeia && !EmptyTitlos)
                {
                    error_code = 1;
                }
                else if (!EmptyDiarkeia && EmptyTitlos)
                {
                    error_code = 2;
                }
                else if (EmptyDiarkeia && EmptyTitlos)
                {
                    error_code = 3;
                }
                else error_code = 0;
            }
            else if (!TrueMsc)
            {
                if (!EmptyDiarkeia || !EmptyTitlos)
                {
                    error_code = 4;
                }
            }
            return error_code;
        }

        public static int ValidatePhd(AitisisViewModel aitisi)
        {
            int error_code = 0;

            bool TruePhD = (aitisi.PHD == true);
            bool EmptyPhD = !TruePhD;
            bool EmptyTitlos = String.IsNullOrEmpty(aitisi.PHD_TITLOS);

            if(TruePhD && EmptyTitlos)
            {
                error_code = 1;
            }
             
            if (EmptyPhD &&!EmptyTitlos)
            {
                error_code = 2;
            }
            return error_code;
        }

        public static int ValidatePed(AitisisViewModel aitisi)
        {
            int error_code = 0;

            bool TruePed = (aitisi.PED == true);
            bool EmptyDiarkeia = !(aitisi.PED_DIARKEIA > 0);
            bool EmptyTitlos = string.IsNullOrEmpty(aitisi.PED_TITLOS);

            if (TruePed)
            {
                if (EmptyDiarkeia && !EmptyTitlos)
                {
                    error_code = 1;
                }
                else if (!EmptyDiarkeia && EmptyTitlos)
                {
                    error_code = 2;
                }
                else if (EmptyDiarkeia && EmptyTitlos)
                {
                    error_code = 3;
                }
            }

            if (!TruePed)
            {
                if (!EmptyDiarkeia || !EmptyTitlos)
                {
                    error_code = 4;
                }
            }
            return error_code;
        }

        #endregion


        #region Adult Education Rules

        public static int ValidateAedMsc(AitisisViewModel aitisi)
        {
            int error_code = 0;

            bool TrueMsc = (aitisi.AED_MSC == true);
            bool EmptyMsc = !TrueMsc;
            bool EmptyTitlos = string.IsNullOrEmpty(aitisi.AED_MSC_TITLOS);

            if (TrueMsc && EmptyTitlos)
            {
                error_code = 1;
            }

            if (EmptyMsc && !EmptyTitlos)
            {
                error_code = 2;
            }
            return error_code;
        }

        public static int ValidateAedPhd(AitisisViewModel aitisi)
        {
            int error_code = 0;

            bool TruePhD = (aitisi.AED_PHD == true);
            bool EmptyPhD = !TruePhD;
            bool EmptyTitlos = string.IsNullOrEmpty(aitisi.AED_PHD_TITLOS);

            if (TruePhD && EmptyTitlos)
            {
                error_code = 1;
            }

            if (EmptyPhD && !EmptyTitlos)
            {
                error_code = 2;
            }
            return error_code;
        }

        #endregion


        #region Extra Qualifications Rules

        public static int ValidateLanguage(AitisisViewModel aitisi)
        {
            int error_code = 0;

            bool EmptyLevel = string.IsNullOrEmpty(aitisi.LANG_LEVEL);
            bool EmptyText = string.IsNullOrEmpty(aitisi.LANG_TEXT);
            bool EmptyTitlos = string.IsNullOrEmpty(aitisi.LANG_TITLOS);
            bool NotEmptyLevel = !string.IsNullOrEmpty(aitisi.LANG_LEVEL);
            bool NotEmptyTextOrTitlos = !string.IsNullOrEmpty(aitisi.LANG_TEXT) || !string.IsNullOrEmpty(aitisi.LANG_TITLOS);


            if (NotEmptyTextOrTitlos && EmptyLevel)
            {
                error_code = 1;
            }

            if (NotEmptyLevel)
            {
                if (EmptyText && !EmptyTitlos)
                {
                    error_code = 2;
                }
                else if (!EmptyText && EmptyTitlos)
                {
                    error_code = 3;
                }
                else if (EmptyText && EmptyTitlos)
                {
                    error_code = 4;
                }
            }
            return error_code;
        }

        public static bool ValidateComputer(AitisisViewModel aitisi)
        {
            bool EmptyCert = !(aitisi.COMPUTER_CERT > 0);
            bool EmptyTitlos = string.IsNullOrEmpty(aitisi.COMPUTER_TITLOS);
            bool EmptyAll = EmptyCert && EmptyTitlos;
            

            if (!EmptyTitlos && EmptyCert)
            {
                return false;
            }
            if (EmptyAll) return true;

            return true;
        }

        #endregion


        #region Further Education Rules

        public static int ValidateEpimorfosi1(AitisisViewModel aitisi)
        {
            int error_code = 0;

            bool EmptyEpimorfosi = aitisi.EPIMORFOSI1 != true;
            bool EmptyHours = !(aitisi.EPIMORFOSI1_HOURS > 0);

            if (EmptyEpimorfosi && !EmptyHours)
            {
                error_code = 1;
            } 
            if (!EmptyEpimorfosi && EmptyHours)
            {
                error_code = 2;
            }
            return error_code;
        }

        public static int ValidateEpimorfosi2(AitisisViewModel aitisi)
        {
            int error_code = 0;

            bool EmptyEpimorfosi = aitisi.EPIMORFOSI2 != true;
            bool EmptyHours = !(aitisi.EPIMORFOSI2_HOURS > 0);

            if (EmptyEpimorfosi && !EmptyHours)
            {
                error_code = 1;
            }
            if (!EmptyEpimorfosi && EmptyHours)
            {
                error_code = 2;
            }

            return error_code;
        }

        public static int ValidateEpimorfosi3(AitisisViewModel aitisi)
        {
            int error_code = 0;

            bool EmptyEpimorfosi = aitisi.EPIMORFOSI3 != true;
            bool EmptyHours = !(aitisi.EPIMORFOSI3_HOURS > 0);

            if (EmptyEpimorfosi && !EmptyHours)
            {
                error_code = 1;
            }
            if (!EmptyEpimorfosi && EmptyHours)
            {
                error_code = 2;
            }

            return error_code;
        }

        #endregion


        #region Epagelma Status Rules

        public static bool ValidateEpagelmaStatus(AitisisViewModel aitisi)
        {
            bool EpagelmaStatusSelected = (aitisi.EPAGELMA_STATUS > 0);

            if (EpagelmaStatusSelected) return true;
            else return false;
        }

        #endregion


        #region Epitropes Rules

        public static bool ValidateCheckStatus(AitisisViewModel aitisi)
        {
            bool ChechStatusTrue = (aitisi.CHECK_STATUS == true);

            if (ChechStatusTrue) return true;
            else return false;
        }

        public static bool ValidateEnstasi(AitisisViewModel aitisi)
        {
            bool EnstasiTrue = (aitisi.ENSTASI == true);
            bool EnstasiAitiaNull = (string.IsNullOrEmpty(aitisi.ENSTASI_AITIA));

            if (EnstasiTrue && EnstasiAitiaNull) return false;
            else if (!EnstasiTrue && !EnstasiAitiaNull) return false;
            else return true;
        }

        #endregion


        internal static bool ValidatePeriferia(AitisisViewModel a, int prokirixiId)
        {
            using (var db = new PegasusDBEntities())
            {
                var aitisisWithPeriferiaAndEidikotita = db.AITISIS
                    .Where(x => x.AFM == a.AFM && x.PERIFERIA_ID == a.PERIFERIA_ID && x.EIDIKOTITA == a.EIDIKOTITA && x.PROKIRIXI_ID == prokirixiId)
                    .Count();

                return (aitisisWithPeriferiaAndEidikotita == 0);
            }
        }
    }
}