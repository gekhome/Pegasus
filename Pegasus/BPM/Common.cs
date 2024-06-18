using Pegasus.DAL;
using Pegasus.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;



namespace Pegasus.BPM
{

    public static class Common
    {
        #region String Functions (equivalent to VB)

        public static string Right(string text, int numberCharacters)
        {
            return text.Substring(numberCharacters > text.Length ? 0 : text.Length - numberCharacters);
        }

        public static string Left(string text, int length)
        {
            if (length < 0)
                throw new ArgumentOutOfRangeException("length", length, "length must be > 0");
            else if (length == 0 || text.Length == 0)
                return "";
            else if (text.Length <= length)
                return text;
            else
                return text.Substring(0, length);
        }

        public static int Len(string text)
        {
            int _length;
            _length = text.Length;
            return _length;
        }

        public static byte Asc(string src)
        {
            return (System.Text.Encoding.GetEncoding("iso-8859-1").GetBytes(src + "")[0]);
        }

        public static char Chr(byte src)
        {
            return (System.Text.Encoding.GetEncoding("iso-8859-1").GetChars(new byte[] { src })[0]);
        }

        public static bool isNumber(string param)
        {
            Regex isNum = new Regex("[^0-9]");
            return !isNum.IsMatch(param);
        }

        #endregion


        #region Date Functions
        /// <summary>
        /// Μετατρέπει τον αριθμό του μήνα σε λεκτικό
        /// στη γενική πτώση.
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public static string monthToGRstring(int m)
        {
            string stGRmonth = "";

            switch (m)
            {
                case 1: stGRmonth = "Ιανουαρίου"; break;
                case 2: stGRmonth = "Φεβρουαρίου"; break;
                case 3: stGRmonth = "Μαρτίου"; break;
                case 4: stGRmonth = "Απριλίου"; break;
                case 5: stGRmonth = "Μαϊου"; break;
                case 6: stGRmonth = "Ιουνίου"; break;
                case 7: stGRmonth = "Ιουλίου"; break;
                case 8: stGRmonth = "Αυγούστου"; break;
                case 9: stGRmonth = "Σεπτεμβρίου"; break;
                case 10: stGRmonth = "Οκτωβρίου"; break;
                case 11: stGRmonth = "Νοεμβρίου"; break;
                case 12: stGRmonth = "Δεκεμβρίου"; break;
                default: break;
            }
            return stGRmonth;
        }

        /// <summary>
        /// Ελέγχει αν η αρχική ημερομηνία είναι μικρότερη
        /// ή ίση με την τελική ημερομηνία.
        /// </summary>
        /// <param name="dateStart"></param>
        /// <param name="dateEnd"></param>
        /// <returns></returns>
        public static bool ValidStartEndDates(DateTime dateStart, DateTime dateEnd)
        {
            bool result;

            if (dateStart > dateEnd)
                result = false;
            else
                result = true;
            return result;
        }

        /// <summary>
        /// Ελέγχει αν δύο ημερομηνίες ανήκουν στο ίδιο έτος.
        /// </summary>
        /// <param name="date1"></param>
        /// <param name="date2"></param>
        /// <returns></returns>
        public static bool DatesInSameYear(DateTime date1, DateTime date2)
        {
            bool result;

            if (date1.Year != date2.Year)
                result = false;
            else
                result = true;
            return result;
        }

        /// <summary>
        /// Ελέγχει αν δύο ημερομηνίες είναι μέσα στο ίδιο Σχ. Έτος
        /// </summary>
        /// <param name="dateStart"></param>
        /// <param name="dateEnd"></param>
        /// <param name="schoolYearID"></param>
        /// <returns></returns>
        public static bool DatesInSchoolYear(DateTime dateStart, DateTime dateEnd, int schoolYearID)
        {
            bool result = true;

            using (var db = new PegasusDBEntities())
            {
                var schoolYear = (from s in db.SYS_SCHOOLYEARS
                                  where s.SY_ID == schoolYearID
                                  select new { s.SY_DATESTART, s.SY_DATEEND }).FirstOrDefault();

                if (dateStart < schoolYear.SY_DATESTART || dateEnd > schoolYear.SY_DATEEND)
                    result = false;

                return result;
            }
        }

        /// <summary>
        /// Ελέγχει αν το σχολικό έτος έχει τη μορφή ΝΝΝΝ-ΝΝΝΝ
        /// και αν τα έτη είναι συμβατά με τις ημερομηνίες
        /// έναρξης και λήξης.
        /// </summary>
        /// <param name="syear"></param>
        /// <param name="d1"></param>
        /// <param name="d2"></param>
        /// <returns></returns>
        public static bool VerifySchoolYear(string syear, DateTime d1, DateTime d2)
        {

            if (syear.IndexOf('-') == -1)
            {
                return false;
            }

            string[] split = syear.Split(new Char[] { '-' });
            string sy1 = Convert.ToString(split[0]);
            string sy2 = Convert.ToString(split[1]);

            if (!isNumber(sy1) || !isNumber(sy2))
            {
                //ShowAdminMessage("Τα έτη δεν είναι αριθμοί.");
                return false;
            }
            else
            {
                int y1 = Convert.ToInt32(sy1);
                int y2 = Convert.ToInt32(sy2);

                if (y2 - y1 > 1 || y2 - y1 <= 0)
                {
                    return false;
                }
                if (d1.Year != y1 || d2.Year != y2)
                {
                    return false;
                }
            }
            // at this point everything is ok
            return true;
        }

        /// <summary>
        /// Ελέγχει αν το χολικό έτος μορφής ΝΝΝΝ-ΝΝΝΝ υπάρχει ήδη.
        /// </summary>
        /// <param name="syear"></param>
        /// <returns></returns>
        public static bool SchoolYearExists(int syear)
        {
            using (var db = new PegasusDBEntities())
            {
                var syear_recs = (from s in db.SYS_SCHOOLYEARS where s.SY_ID == syear select s).Count();

                if (syear_recs != 0)
                {
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// Υπολογίζει τα έτη (στρογγυλοποιημένα) μεταξύ δύο ημερομηνιών
        /// </summary>
        /// <param name="sdate">αρχική ημερομηνία</param>
        /// <param name="edate">τελική ημερομηνία</param>
        /// <returns></returns>
        public static int YearsDiff(DateTime sdate, DateTime edate)
        {
            TimeSpan ts = edate - sdate;
            int days = ts.Days;

            double _years = days / 365;

            int years = Convert.ToInt32(Math.Ceiling(_years));

            return years;
        }

        public static int DaysDiff(DateTime sdate, DateTime edate)
        {
            TimeSpan ts = edate - sdate;
            int days = ts.Days;

            return days;
        }

        #endregion


        #region Custom Pegasus Functions

        public static float Max(params float[] values)
        {
            return Enumerable.Max(values);
        }

        public static float Min(params float[] values)
        {
            return Enumerable.Min(values);
        }

        public static string GetFinancialYear(int yearIndex)
        {
            using (var db = new PegasusDBEntities())
            {
                var qry = (from d in db.SYS_TAXFREE where d.YEAR_ID == yearIndex select new { d.YEAR_TEXT }).FirstOrDefault();

                if (qry == null)
                    return "";
                else
                    return qry.YEAR_TEXT;
            }
        }

        public static int GetOpenProkirixiID()
        {
            using (var db = new PegasusDBEntities())
            {
                var qry = (from p in db.PROKIRIXIS where p.STATUS == 1 select new { p.ID }).FirstOrDefault();

                if (qry == null)
                    return 0;
                else
                    return qry.ID;
            }
        }

        public static int GetOpenProkirixiID(bool active)
        {
            using (var db = new PegasusDBEntities())
            {
                var qry = (from p in db.PROKIRIXIS where p.ACTIVE == true select new { p.ID }).FirstOrDefault();

                if (qry == null)
                    return 0;
                else
                    return qry.ID;
            }
        }

        public static int GetAdminProkirixiID()
        {
            using (var db = new PegasusDBEntities())
            {
                var qry = (from p in db.PROKIRIXIS where p.ADMIN == true select new { p.ID }).FirstOrDefault();

                if (qry == null)
                    return 0;
                else
                    return qry.ID;
            }
        }

        public static int GetEnstasiProkirixiID()
        {
            using (var db = new PegasusDBEntities())
            {
                var qry = (from p in db.PROKIRIXIS where p.ENSTASEIS == true select new { p.ID }).FirstOrDefault();

                if (qry == null)
                    return 0;
                else
                    return qry.ID;
            }
        }

        public static ProkirixisViewModel GetAdminProkirixi()
        {
            using (var db = new PegasusDBEntities())
            {
                var data = (from d in db.PROKIRIXIS
                            where d.ADMIN == true
                            select new ProkirixisViewModel
                            {
                                ID = d.ID,
                                FEK = d.FEK,
                                PROTOCOL = d.PROTOCOL,
                                SCHOOL_YEAR = d.SCHOOL_YEAR,
                                DIOIKITIS = d.DIOIKITIS,
                                DATE_START = d.DATE_START,
                                DATE_END = d.DATE_END,
                                HOUR_START = d.HOUR_START,
                                HOUR_END = d.HOUR_END,
                            }).FirstOrDefault();
                return data;
            }
        }

        public static ProkirixisViewModel GetOpenProkirixi()
        {
            using (var db = new PegasusDBEntities())
            {
                var data = (from d in db.PROKIRIXIS
                            where d.ACTIVE == true
                            select new ProkirixisViewModel
                            {
                                ID = d.ID,
                                FEK = d.FEK,
                                PROTOCOL = d.PROTOCOL,
                                SCHOOL_YEAR = d.SCHOOL_YEAR,
                                DIOIKITIS = d.DIOIKITIS,
                                DATE_START = d.DATE_START,
                                DATE_END = d.DATE_END,
                                HOUR_START = d.HOUR_START,
                                HOUR_END = d.HOUR_END
                            }).FirstOrDefault();
                return data;
            }
        }

        public static string GetSchoolYearText(int syearId)
        {
            using (var db = new PegasusDBEntities())
            {
                var data = (from d in db.SYS_SCHOOLYEARS where d.SY_ID == syearId select d).FirstOrDefault();

                string syearText = data.SY_TEXT;
                return (syearText);
            }
        }

        public static bool GetOpenProkirixiUserView()
        {
            using (var db = new PegasusDBEntities())
            {
                var qry = (from p in db.PROKIRIXIS where p.USER_VIEW == true select new { p.USER_VIEW }).FirstOrDefault();

                if (qry == null)
                    return false;
                else
                    return (bool)qry.USER_VIEW;
            }
        }

        public static bool GetOpenProkirixiEnstasi()
        {
            using (var db = new PegasusDBEntities())
            {
                var qry = (from p in db.PROKIRIXIS where p.ENSTASEIS == true select new { p.ENSTASEIS }).FirstOrDefault();

                if (qry == null)
                    return false;
                else
                    return (bool)qry.ENSTASEIS;
            }
        }

        /// <summary>
        /// Επιστρέι τον ΑΠ της προκήρυξης που αντιστοιχεί σε ένα κωδικό αίτησης
        /// Προστέθηκε 15/3/2016
        /// </summary>
        /// <param name="aitisiId"></param>
        /// <returns>prokirixi_protocol</returns>
        public static string GetOpenProkirixiProtocol(int aitisiId)
        {
            int prokirixiId = GetOpenProkirixiID();

            using (var db = new PegasusDBEntities())
            {
                var prokname = (from p in db.PROKIRIXIS where p.ID == prokirixiId select new { p.PROTOCOL }).FirstOrDefault();

                return prokname.PROTOCOL;
            }
        }


        /// <summary>
        /// Επιστρέφει το ΑΦΜ που αντιστοιχεί σε ένα κωδικό αίτησης.
        /// Προστέθηκε 15/3/2016.
        /// </summary>
        /// <param name="aitisiId"></param>
        /// <returns>safm</returns>
        public static string GetAFM(int aitisiId)
        {
            using (var db = new PegasusDBEntities())
            {
                var queryafm = (from a in db.AITISIS
                                where a.AITISI_ID == aitisiId
                                select new { a.AFM }).FirstOrDefault();

                return queryafm.AFM;
            }
        }

        /// <summary>
        /// Επιστρέφει τον κωδικό ΙΕΚ Αίτησης που αντιστοιχεί σε ένα κωδικό Αίτησης.
        /// Προστέθηκε 15/3/2016.
        /// </summary>
        /// <param name="aitisiId"></param>
        /// <returns>iekId</returns>
        public static int GetIek(int aitisiId)
        {
            int iekId = 0;

            using (var db = new PegasusDBEntities())
            {
                var queryIek = (from a in db.AITISIS
                                where a.AITISI_ID == aitisiId
                                select new { a.SCHOOL_ID }).FirstOrDefault();
                iekId = (int)queryIek.SCHOOL_ID;

                return iekId;
            }
        }

        /// <summary>
        /// Επιστρέφει τον κωδικό Ειδικότητας που αντιστοιχεί σε ένα κωδικό Αίτησης
        /// Προστέθηκε 15/3/2016.
        /// </summary>
        /// <param name="aitisiId"></param>
        /// <returns>EidikotitaId</returns>
        public static int GetEidikotita(int aitisiId)
        {
            int EidikotitaId = 0;

            using (var db = new PegasusDBEntities())
            {
                var queryE = (from a in db.AITISIS
                              where a.AITISI_ID == aitisiId
                              select new { a.EIDIKOTITA }).FirstOrDefault();
                EidikotitaId = (int)queryE.EIDIKOTITA;

                return EidikotitaId;
            }
        }

        /// <summary>
        /// Υπολογίζει τις ημέρες λογιστικού έτους μεταξύ δύο ημερομηνιών,
        /// προσομειώνοντας τη συνάρτηση Days360 του Excel.
        /// </summary>
        /// <param name="initial_date"></param>
        /// <param name="final_date"></param>
        /// <returns name="meres"></returns>
        public static int Days360(DateTime initial_date, DateTime final_date)
        {
            DateTime date1 = initial_date;
            DateTime date2 = final_date;

            var y1 = date1.Year;
            var y2 = date2.Year;
            var m1 = date1.Month;
            var m2 = date2.Month;
            var d1 = date1.Day;
            var d2 = date2.Day;

            DateTime tempDate = date1.AddDays(1);
            if (tempDate.Day == 1 && date1.Month == 2)
            {
                d1 = 30;
            }
            if (d2 == 31 && d1 == 30)
            {
                d2 = 30;
            }

            double meres = (y2 - y1) * 360 + (m2 - m1) * 30 + (d2 - d1);
            meres = (meres / 30) * 25;
            meres = Math.Ceiling(meres);

            return Convert.ToInt32(meres);
        }

        #endregion


        #region User Validation (not used as not applicable for web app)

        public static int ValidateUser(string username, string password)
        {
            using (var db = new PegasusDBEntities())
            {
                // first check if both passed parameters are empty
                if (string.IsNullOrWhiteSpace(username) && string.IsNullOrWhiteSpace(password)) return 1;
                // check username empty and password not empty
                if (string.IsNullOrWhiteSpace(username) && !string.IsNullOrWhiteSpace(password)) return 2;
                // check username not empty and password empty
                if (!string.IsNullOrWhiteSpace(username) && string.IsNullOrWhiteSpace(password)) return 3;

                // user entered both username and password, so safe to LINQ it
                var loginInfo = (from u in db.USER_TEACHERS
                                 where u.USERNAME == username && u.PASSWORD == password
                                 select u).Count();
                // both username and password correct
                if (loginInfo != 0) return 0;
                else
                {
                    // if username is correct
                    if (!UsernameValidated(username) && PasswordValidated(password)) return 4; // incorrect username and password correct
                    else if (!UsernameValidated(username) && !PasswordValidated(password)) return 5; // both incorrect
                    else // correct username
                    {
                        // check if password is correct
                        if (!PasswordValidated(password)) return 6; // incorrect password
                        else return 0; //correct password (at this point both username and password correct)
                    }
                }
            }
        }

        public static bool PasswordValidated(string password)
        {
            using (var db = new PegasusDBEntities())
            {
                var pass = (from p in db.USER_TEACHERS
                            where p.PASSWORD == password
                            select p).Count();

                if (pass != 0) return true;
                else return false;
            }
        }

        public static bool UsernameValidated(string username)
        {
            using (var db = new PegasusDBEntities())
            {
                var user = (from u in db.USER_TEACHERS
                            where u.USERNAME == username
                            select u).Count();

                if (user != 0) return true;
                else return false;
            }
        }

        #endregion


        #region Protocol Generator

        public static string Get8Digits()
        {
            using (var db = new PegasusDBEntities())
            {
                var data = (from d in db.AITISIS orderby d.AITISI_ID descending select d).First();
                int newId = data.AITISI_ID + 1;

                return string.Format("{0:00000000}", newId);
            }
        }

        /*
         * ------------------------------------------------------------------
         * Protocol unique number generator.
         * It is based on RundomNumberGenerator (Microsoft cryptography)
         * and Get8Digits in Common.cs of Pegasus
         * ------------------------------------------------------------------
         */

        public static string GenerateProtocol()
        {
            DateTime date1 = DateTime.Now;
            DateTime dateOnly = date1.Date;

            string stDate = string.Format("{0:dd.MM.yyyy}", dateOnly);               //Convert.ToString(dateOnly);

            string protocol = Get8Digits() + "/" + stDate;
            return protocol;
        }

        public static string GenerateProtocol(DateTime date1)
        {
            DateTime dateOnly = date1.Date;

            string stDate = string.Format("{0:dd.MM.yyyy}", dateOnly);

            string protocol = Get8Digits() + "/" + stDate;
            return protocol;
        }

        public static string GeneratePassword()
        {
            Random rnd = new Random();
            int random = rnd.Next(1, 1000);
            return string.Format("{0:000}", random);
        }

        #endregion


        #region Getters

        public static List<sqlTEACHER_AITISEIS> GetAitiseisList(int? prokirixiId, int? schoolId)
        {
            using (var db = new PegasusDBEntities())
            {
                List<sqlTEACHER_AITISEIS> data = new List<sqlTEACHER_AITISEIS>();

                if (prokirixiId != null && schoolId > 0)
                {
                    try
                    {
                        data = (from a in db.sqlTEACHER_AITISEIS
                                where a.PROKIRIXI_ID == prokirixiId && a.SCHOOL_ID == schoolId
                                orderby a.FULLNAME, a.AITISI_PROTOCOL
                                select new sqlTEACHER_AITISEIS
                                {
                                    AITISI_PROTOCOL = a.AITISI_PROTOCOL,
                                    PERIFERIA_NAME = a.PERIFERIA_NAME,
                                    AFM = a.AFM,
                                    FULLNAME = a.FULLNAME,
                                    EIDIKOTITA_TEXT = a.EIDIKOTITA_TEXT,
                                    PROKIRIXI_ID = a.PROKIRIXI_ID,
                                    AITISI_ID = a.AITISI_ID,
                                    PERIFERIA_ID = a.PERIFERIA_ID,
                                    SCHOOL_ID = a.SCHOOL_ID,
                                    SCHOOL_NAME = a.SCHOOL_NAME,
                                    CHECK_STATUS = a.CHECK_STATUS
                                }).ToList();
                    }
                    catch (Exception e)
                    {
                        string errmsg = e.Message;
                    }
                }
                return data;
            }
        }

        public static int? GetEidikotitaGroupId(int eidikotita)
        {
            int? groupId = null;

            using (var db = new PegasusDBEntities())
            {
                var data = (from d in db.SYS_EIDIKOTITES where d.EIDIKOTITA_ID == eidikotita select d).FirstOrDefault();
                if (data != null)
                {
                    if (data.EIDIKOTITA_GROUP_ID != null) groupId = (int)data.EIDIKOTITA_GROUP_ID;
                }
                return groupId;
            }
        }

        #endregion


        #region Validations

        public static bool CanDeleteAitisi(int aitisiId)
        {
            using (var db = new PegasusDBEntities())
            {
                int scount = (from s in db.AITISIS_SCHOOLS
                              where s.AITISI_ID == aitisiId
                              select s).Count();

                int tcount = (from t in db.EXP_TEACHING
                              where t.AITISI_ID == aitisiId
                              select t).Count();

                int fcount = (from f in db.EXP_FREELANCE
                              where f.AITISI_ID == aitisiId
                              select f).Count();

                int vcount = (from v in db.EXP_VOCATIONAL
                              where v.AITISI_ID == aitisiId
                              select v).Count();

                bool HasAtLeastOneChild;
                HasAtLeastOneChild = (scount > 0 || tcount > 0 || fcount > 0 || vcount > 0);

                if (HasAtLeastOneChild == true)
                    return false;
                else
                    return true;
            }
        }

        public static bool CanDeleteProkirixi(int prokirixiId)
        {
            using (var db = new PegasusDBEntities())
            {
                var data = (from d in db.AITISIS where d.PROKIRIXI_ID == prokirixiId select d).Count();

                if (data > 0) return false;
                else return true;
            }
        }

        public static bool CanDeleteSchoolYear(int schoolyearId)
        {
            using (var db = new PegasusDBEntities())
            {
                var data = (from d in db.PROKIRIXIS where d.SCHOOL_YEAR == schoolyearId select d).Count();

                if (data > 0) return false;
                else return true;
            }
        }

        public static bool CanDeleteEidikotita(int eidikotitaId)
        {
            using (var db = new PegasusDBEntities())
            {
                var data = (from d in db.PROKIRIXIS_EIDIKOTITES where d.EIDIKOTITA_ID == eidikotitaId select d).Count();

                if (data > 0) return false;
                else return true;
            }
        }

        public static bool CanDeleteKladosUnified(int kladosId)
        {
            using (var db = new PegasusDBEntities())
            {
                var data = (from d in db.SYS_EIDIKOTITES where d.KLADOS_UNIFIED == kladosId select d).Count();
                if (data > 0) return false;
                else return true;
            }
        }

        public static bool CanDeleteEidikotitaInProkirixi(ProkirixisEidikotitesViewModel data)
        {
            int currentProkirixi = GetAdminProkirixiID();
            if (data.PROKIRIXI_ID != currentProkirixi) return false;

            return true;
        }

        public static bool EidikotitaSchoolExists(ProkirixisEidikotitesViewModel item)
        {
            using (var db = new PegasusDBEntities())
            {
                var data = (from d in db.PROKIRIXIS_EIDIKOTITES
                            where d.PROKIRIXI_ID == item.PROKIRIXI_ID && d.SCHOOL_ID == item.SCHOOL_ID && d.EIDIKOTITA_ID == item.EIDIKOTITA_ID
                            select d).Count();
                if (data > 0)
                    return true;
                else
                    return false;
            }
        }

        #endregion


        #region Upload Functions

        public static bool VerifyUploadIntegrity(int prokirixiId, USER_TEACHERS loggedTeacher)
        {
            using (var db = new PegasusDBEntities())
            {
                string teacherAfm = db.TEACHERS.Find(loggedTeacher.USER_AFM).AFM;

                var data = (from d in db.UploadEnstaseis where d.PROKIRIXI_ID == prokirixiId && d.TEACHER_AFM == teacherAfm select d).FirstOrDefault();
                if (data == null)
                    return true;

                if (!UploadFilesExist(data.UPLOAD_ID))
                    return true;

                string username = GetUserSchoolFromSchoolId((int)data.SCHOOL_ID);

                var files = (from d in db.UploadEnstaseisFiles where d.UPLOAD_ID == data.UPLOAD_ID && d.SCHOOL_USER == username select d).Count();
                if (files == 0)
                    return false;
                else
                    return true;
            }
        }

        public static bool UploadFilesExist(int uploadId)
        {
            using (var db = new PegasusDBEntities())
            {
                int countFiles = (from d in db.UploadEnstaseisFiles where d.UPLOAD_ID == uploadId select d).Count();
                if (countFiles > 0)
                    return true;
                return false;
            }
        }

        public static Tuple<int, int, int> GetUploadInfo(int uploadId)
        {
            int school_id = 0;
            int prokirixi_id = 0;
            int aitisi_id = 0;

            using (var db = new PegasusDBEntities())
            {
                var upload = (from d in db.UploadEnstaseis where d.UPLOAD_ID == uploadId select d).FirstOrDefault();
                if (upload != null)
                {
                    school_id = (int)upload.SCHOOL_ID;
                    prokirixi_id = (int)upload.PROKIRIXI_ID;
                    aitisi_id = (int)upload.AITISI_ID;
                }

                var data = Tuple.Create(school_id, prokirixi_id, aitisi_id);
                return (data);
            }
        }

        public static string GetUserSchoolFromAitisi(int aitisiId)
        {
            using (var db = new PegasusDBEntities())
            {
                int schoolId = (int)db.AITISIS.Find(aitisiId).SCHOOL_ID;
                var data = (from d in db.USER_SCHOOLS where d.USER_SCHOOLID == schoolId select d).FirstOrDefault();
                if (data != null)
                {
                    return data.USERNAME;
                }
                else
                {
                    return "iek.demo";
                }
            }
        }

        public static string GetUserSchoolFromSchoolId(int schoolId)
        {
            string username = "";

            using (var db = new PegasusDBEntities())
            {
                var data = (from d in db.USER_SCHOOLS where d.USER_SCHOOLID == schoolId select d).FirstOrDefault();
                if (data != null)
                {
                    username = data.USERNAME;
                }
                return (username);
            }
        }

        public static string GetTeacherNameFromUser(string AFM)
        {
            string fullname = "X";

            using (var db = new PegasusDBEntities())
            {
                TEACHERS data = db.TEACHERS.Find(AFM);
                if (data != null)
                    fullname = data.LASTNAME + " " + data.FIRSTNAME;

                return fullname;
            }
        }

        public static string GetFileGuidFromName(string filename, int uploadId)
        {
            string file_id = "";

            using (var db = new PegasusDBEntities())
            {
                var fileData = (from d in db.UploadEnstaseisFiles where d.FILENAME == filename && d.UPLOAD_ID == uploadId select d).FirstOrDefault();
                if (fileData != null) file_id = fileData.ID;

                return (file_id);
            }
        }

        #endregion

    }   // class Common

}   // namespace