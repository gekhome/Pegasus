using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;

using Pegasus.Models;
using Pegasus.DAL;
using Pegasus.BPM;

namespace Pegasus.BPM
{
    /// <summary>
    /// Εκτελεί τις λειτουργίες Μεταφοράς Προϋπηρεσιών από προηγούμενες Προκηρύξεις
    /// Προστέθηκε 15/3/2016
    /// </summary>
    public static class Transfer
    {
        public const int TICKET_TIMEOUT_MINUTES = 240;


        #region ΜΕΤΑΦΟΡΑ ΠΡΟΗΓΟΥΜΕΝΩΝ ΠΡΟΫΠΗΡΕΣΙΩΝ ΝΕΟ - 6/6/2018

        /// <summary>
        /// Ενώνει σε λίστα παλαιές προϋπηρεσίες που αντιστοιχούν στον κωδικό αίτησης
        /// (τρέχουσα αίτηση) και τις αποθηκεύει στη Βάση απαλοίφοντας διπλότυπες.
        /// Προσθήκη 6/6/2018
        /// </summary>
        /// <param name="aitisiID"></param>
        public static int TransferAllWorkExperience(int aitisiID)
        {
            int res_code = 0;
            bool newDidaktikesExist = false;
            bool newVocationalExist = false;
            bool newFreelanceExist = false;

            int current_aitisiID = aitisiID;

            int current_prokirixi = Common.GetOpenProkirixiID(true);
            if (!(current_prokirixi > 0))
            {
                return res_code = 1;
            }
            string afm = Common.GetAFM(aitisiID);
            int iek = Common.GetIek(aitisiID);
            int eidikotita = Common.GetEidikotita(aitisiID);

            List<AITISIS> aitiseis = new List<AITISIS>();
            // --------------------------------------------------------------------
            // 5/6/2018 - ΑΛΛΑΓΗ ΕΔΩ ΣΤΟ ΠΟΙΕΣ ΑΙΤΗΣΕΙΣ ΘΑ ΔΙΑΛΕΓΕΙ!!!
            // Η επιλογή βασίζεται είτε στην ίδια ειδικότητα ή ίδια ομάδα ειδικότητας
            // Φέρνει αδιακρίτως όλες τις προηγούμενες προϋπηρεσίες που ταιριάζουν
            // και στο τέλος γίνεται η εκκαθάριση από τις τυχόν διπλότυπες.
            // ---------------------------------------------------------------------

            using (var db = new PegasusDBEntities())
            {
                var data = (from d in db.SYS_EIDIKOTITES where d.EIDIKOTITA_ID == eidikotita select d).FirstOrDefault();
                int eidikotitaGroup = data.EIDIKOTITA_GROUP_ID ?? 0;

                if (eidikotitaGroup > 0)
                {
                    aitiseis = (from a in db.AITISIS
                                where a.PROKIRIXI_ID != current_prokirixi && (a.AFM == afm && (a.EIDIKOTITA == eidikotita || a.EIDIKOTITA_GROUP == eidikotitaGroup))
                                select a).ToList();
                }
                else
                {
                    aitiseis = (from a in db.AITISIS
                                where a.PROKIRIXI_ID != current_prokirixi && (a.AFM == afm && a.EIDIKOTITA == eidikotita)
                                select a).ToList();
                }

                // ΠΡΩΤΑ ΕΛΕΓΧΟΣ ΑΝ ΕΧΟΥΝ ΚΑΤΑΧΩΡΗΘΕΙ ΝΕΕΣ ΠΡΟΫΠΗΡΕΣΙΕΣ - ΑΝ ΥΠΑΡΧΟΥΝ ΑΚΥΡΩΝΕΤΑΙ Η ΜΕΤΑΦΟΡΑ
                var new_didaktikes = (from d in db.EXP_TEACHING where d.AITISI_ID == current_aitisiID select d).ToList();
                // Έλεγχος για υπάρχουσες νέες διδακτικές. ΔΕΝ πρέπει να υπάρχουν πριν τη μεταφορά.
                if (new_didaktikes.Count() > 0) newDidaktikesExist = true;

                var new_vocational = (from d in db.EXP_VOCATIONAL where d.AITISI_ID == current_aitisiID select d).ToList();
                // Έλεγχος για υπάρχουσες νέες επαγγελματικές. ΔΕΝ πρέπει να υπάρχουν πριν τη μεταφορά.
                if (new_vocational.Count() > 0) newVocationalExist = true;

                var new_freelance = (from d in db.EXP_FREELANCE where d.AITISI_ID == current_aitisiID select d).ToList();
                // Έλεγχος για υπάρχουσες νέες ελεύθερου επαγγέλματος. ΔΕΝ πρέπει να υπάρχουν πριν τη μεταφορά.
                if (new_freelance.Count() > 0) newFreelanceExist = true;

                if (newDidaktikesExist || newVocationalExist || newFreelanceExist) return res_code = 2;

                if (aitiseis.Count > 0)
                {
                    // ΜΕΤΑΦΟΡΑ ΔΙΔΑΚΤΙΚΩΝ ΠΡΟΫΠΗΡΕΣΙΩΝ -------------------
                    FetchTeaching(current_aitisiID, aitiseis);

                    // ΜΕΤΑΦΟΡΑ ΕΠΑΓΓΕΛΜΑΤΙΚΩΝ ΠΡΟΫΠΗΡΕΣΙΩΝ ---------------
                    FetchVocational(current_aitisiID, aitiseis);

                    // ΜΕΤΑΦΟΡΑ ΠΡΟΫΠΗΡΕΣΙΩΝ ΕΛΕΥΘΕΡΟΥ ΕΠΑΓΓΕΛΜΑΤΟΣ -------
                    FetchFreelance(current_aitisiID, aitiseis);
                }
                else
                {
                    return res_code = 3;
                }
                return res_code;
            }
        }

        public static void FetchTeaching(int current_aitisiID, List<AITISIS> aitiseis)
        {
            var all_didaktikes = new List<EXP_TEACHING>();

            using (var db = new PegasusDBEntities())
            {
                foreach (var aitisi in aitiseis)
                {
                    int old_aitisiID = aitisi.AITISI_ID;
                    var old_didaktikes = (from d in db.EXP_TEACHING where d.AITISI_ID == old_aitisiID select d).ToList();
                    if (old_didaktikes.Count > 0)
                    {
                        all_didaktikes = all_didaktikes.Union(old_didaktikes).ToList();
                    }
                }
                // NOW, CREATE A NEW LIST FROM THE ABOVE, WITHOUT DUPLICATES
                if (all_didaktikes.Count > 0)
                {
                    List<EXP_TEACHING> uniqueDidaktikes = all_didaktikes.GroupBy(d => new { d.TEACH_TYPE, d.SCHOOL_YEAR, d.DATE_FROM, d.DATE_TO, d.HOURS, d.HOURS_WEEK })
                                                            .Select(d => d.FirstOrDefault())
                                                            .ToList();

                    foreach (var didaktiki in uniqueDidaktikes)
                    {
                        didaktiki.AITISI_ID = current_aitisiID;
                        didaktiki.MORIA = (float)Kerberos.MoriaTeaching(didaktiki);
                        db.EXP_TEACHING.Add(didaktiki);
                        db.SaveChanges();
                    }
                }
                return;
            }
        }

        public static void FetchVocational(int current_aitisiID, List<AITISIS> aitiseis)
        {
            var all_vocational = new List<EXP_VOCATIONAL>();

            using (var db = new PegasusDBEntities())
            {
                foreach (var aitisi in aitiseis)
                {
                    int old_aitisiID = aitisi.AITISI_ID;
                    var old_vocational = (from d in db.EXP_VOCATIONAL where d.AITISI_ID == old_aitisiID select d).ToList();
                    if (old_vocational.Count > 0)
                    {
                        all_vocational = all_vocational.Union(old_vocational).ToList();
                    }
                }
                // NOW, CREATE A NEW LIST FROM THE ABOVE, WITHOUT DUPLICATES
                if (all_vocational.Count > 0)
                {
                    List<EXP_VOCATIONAL> uniqueVocational = all_vocational.GroupBy(d => new { d.DATE_FROM, d.DATE_TO, d.DAYS_AUTO, d.DAYS_MANUAL })
                                                            .Select(d => d.FirstOrDefault())
                                                            .ToList();

                    foreach (var vocational in uniqueVocational)
                    {
                        vocational.AITISI_ID = current_aitisiID;
                        vocational.MORIA = (float)Kerberos.MoriaVocational(vocational);
                        db.EXP_VOCATIONAL.Add(vocational);
                        db.SaveChanges();
                    }
                }
                return;
            }
        }

        public static void FetchFreelance(int current_aitisiID, List<AITISIS> aitiseis)
        {
            var all_freelance = new List<EXP_FREELANCE>();

            using (var db = new PegasusDBEntities())
            {
                foreach (var aitisi in aitiseis)
                {
                    int old_aitisiID = aitisi.AITISI_ID;
                    var old_freelance = (from d in db.EXP_FREELANCE where d.AITISI_ID == old_aitisiID select d).ToList();
                    if (old_freelance.Count > 0)
                    {
                        all_freelance = all_freelance.Union(old_freelance).ToList();
                    }
                }
                // NOW, CREATE A NEW LIST FROM THE ABOVE, WITHOUT DUPLICATES
                if (all_freelance.Count > 0)
                {
                    List<EXP_FREELANCE> uniqueFreelance = all_freelance
                        .GroupBy(d => new { d.DATE_FROM, d.DATE_TO, d.DAYS_AUTO, d.DAYS_MANUAL, d.INCOME_YEAR, d.INCOME })
                        .Select(d => d.FirstOrDefault())
                        .ToList();

                    foreach (var freelance in uniqueFreelance)
                    {
                        freelance.AITISI_ID = current_aitisiID;
                        freelance.MORIA = (float)Kerberos.MoriaFreelance(freelance);
                        db.EXP_FREELANCE.Add(freelance);
                        db.SaveChanges();
                    }
                }
                return;
            }

        }

        public static string ErrorCodeExperienceDictionary(int key)
        {
            Dictionary<int, string> ErrorCodes = new Dictionary<int, string>()
            {
                { 0, "Οι προηγούμενες προϋπηρεσίες μεταφέρθηκαν και μοριοδοτήθηκαν, σύμφωνα με τη νέα υπουργική." },
                { 1, "Δεν βρέθηκε τρέχουσα ενεργή προκήρυξη." },
                { 2, "Βρέθηκαν καταχωρημένες προϋπηρεσίες. Πρέπει να γίνει πρώτα μεταφορά και μετά καταχώρηση των νέων. Αλλιώς, έχετε ήδη κάνει μεταφορά." },
                { 3, "Δεν βρέθηκαν προηγούμενες αιτήσεις για μεταφορά προϋπηρεσιών." }
            };

            return ErrorCodes[key];
        }

        #endregion


        #region ΜΕΤΑΦΟΡΑ ΠΡΟΗΓΟΥΜΕΝΩΝ ΕΠΙΜΟΡΦΩΣΕΩΝ (ΝΕΟ 27-04-2019)

        public static int TransferAllReeducations(int aitisiID)
        {
            int res_code = 0;
            bool newReeducationsExist = false;

            int current_aitisiID = aitisiID;

            int current_prokirixi = Common.GetOpenProkirixiID(true);
            if (!(current_prokirixi > 0))
            {
                return res_code = -1;
            }
            string afm = Common.GetAFM(aitisiID);
            int iek = Common.GetIek(aitisiID);

            List<AITISIS> aitiseis = new List<AITISIS>();

            using (var db = new PegasusDBEntities())
            {
                aitiseis = (from a in db.AITISIS
                            where a.PROKIRIXI_ID != current_prokirixi && (a.AFM == afm)
                            select a).ToList();

                // ΠΡΩΤΑ ΕΛΕΓΧΟΣ ΑΝ ΕΧΟΥΝ ΚΑΤΑΧΩΡΗΘΕΙ ΝΕΕΣ ΕΠΙΜΟΡΦΩΣΕΙΣ - ΑΝ ΥΠΑΡΧΟΥΝ ΑΚΥΡΩΝΕΤΑΙ Η ΜΕΤΑΦΟΡΑ
                var new_reeducations = (from d in db.REEDUCATION where d.AITISI_ID == current_aitisiID select d).ToList();
                // Έλεγχος για υπάρχουσες νέες επιμορφώσεις. ΔΕΝ πρέπει να υπάρχουν πριν τη μεταφορά.
                if (new_reeducations.Count() > 0) newReeducationsExist = true;

                var old_reeducations = (from d in db.REEDUCATION where d.PROKIRIXI_ID != current_prokirixi && d.AFM == afm select d).ToList();
                if (old_reeducations.Count == 0) return res_code = 3;

                if (newReeducationsExist) return res_code = 1;
                if (aitiseis.Count > 0)
                {
                    // ΜΕΤΑΦΟΡΑ ΕΠΙΜΟΡΦΩΣΕΩΝ
                    FetchReeducations(current_aitisiID, aitiseis);
                }
                else
                {
                    return res_code = 2;
                }

                return res_code;
            }
        }

        public static void FetchReeducations(int current_aitisiID, List<AITISIS> aitiseis)
        {
            int current_prokirixi = Common.GetOpenProkirixiID(true);
            var all_reeducations = new List<REEDUCATION>();

            using (var db = new PegasusDBEntities())
            {
                foreach (var aitisi in aitiseis)
                {
                    int old_aitisiID = aitisi.AITISI_ID;
                    var old_reeducations = (from d in db.REEDUCATION where d.AITISI_ID == old_aitisiID select d).ToList();
                    if (old_reeducations.Count > 0)
                    {
                        all_reeducations = all_reeducations.Union(old_reeducations).ToList();
                    }
                }
                // NOW, CREATE A NEW LIST FROM THE ABOVE, WITHOUT DUPLICATES
                if (all_reeducations.Count > 0)
                {
                    List<REEDUCATION> uniqueReeducations = all_reeducations
                        .GroupBy(d => new { d.CERTIFICATE_DATE, d.CERTIFICATE_FOREAS, d.CERTIFICATE_TITLE, d.CERTIFICATE_HOURS })
                        .Select(d => d.FirstOrDefault())
                        .ToList();

                    foreach (var reeducation in uniqueReeducations)
                    {
                        reeducation.AITISI_ID = current_aitisiID;
                        reeducation.PROKIRIXI_ID = current_prokirixi;
                        db.REEDUCATION.Add(reeducation);
                        db.SaveChanges();
                    }
                }
                return;
            }
        }

        public static string ErrorCodeReeducationDictionary(int key)
        {
            Dictionary<int, string> ErrorCodes = new Dictionary<int, string>()
            {
                { 0, "Η μεταφορά των προηγόυμενων επιμορφώσεων ολοκληρώθηκε." },
                { -1, "Δεν βρέθηκε τρέχουσα ενεργή προκήρυξη."},
                { 1, "Βρέθηκαν καταχωρημένες επιμορφώσεις. Πρέπει να γίνει πρώτα μεταφορά και μετά καταχώρηση των νέων. Αλλιώς, έχετε ήδη κάνει μεταφορά." },
                { 2, "Δεν βρέθηκαν προηγούμενες αιτήσεις για μεταφορά επιμορφώσεων." },
                { 3, "Δεν υπάρχουν προηγούμενες επιμορφώσεις για αυτόν τον υποψήφιο εκπαιδευτικό." }
            };

            return ErrorCodes[key];
        }

        #endregion


        #region ΠΑΛΑΙΟΣ ΤΡΟΠΟΣ ΜΕΤΑΦΟΡΑΣ ΠΡΟΫΠΗΡΕΣΙΩΝ (ΜΕΧΡΙ 2017-2018)

        /// <summary>
        /// Ελέγχει εάν υπάρχουν προηγούμενες αιτήσεις για μεταφορά των προϋπηρεσιών
        /// Προστέθηκε 15/3/2016
        /// </summary>
        /// <param name="aitisiID"></param>
        /// <returns>transfer_status</returns>
        public static bool AitiseisToTransferExist(int aitisiID)
        {
            bool aitiseis_exist = false;

            int current_prokirixi = Common.GetOpenProkirixiID(true);
            if (!(current_prokirixi > 0))
            {
                return false;
            }
            string afm = Common.GetAFM(aitisiID);
            int iek = Common.GetIek(aitisiID);
            int eidikotita = Common.GetEidikotita(aitisiID);

            using (var db = new PegasusDBEntities())
            {
                // Έλεγχος για null: NULL SPOT
                var qryExist = from a in db.AITISIS
                               where a.PROKIRIXI_ID != current_prokirixi && a.TRANSFERRED != true && (a.AFM == afm && a.EIDIKOTITA == eidikotita)
                               select a;
                if (qryExist.Count() == 0)
                {
                    return aitiseis_exist;
                }
                else
                {
                    aitiseis_exist = true;
                    return aitiseis_exist;
                }
            }
        }

        /// <summary>
        /// Θέτει στις παλιές Αιτήσεις το TRANSFERRED = true (ώστε να μη μεταφέρονται ξανά).
        /// Προσθήκη 16/3/2016
        /// </summary>
        /// <param name="aitisiID"></param>
        public static void OldAitiseisTransferTrue(int aitisiID)
        {
            int current_prokirixi = Common.GetOpenProkirixiID(true);
            if (!(current_prokirixi > 0))
            {
                return;
            }
            string afm = Common.GetAFM(aitisiID);
            int iek = Common.GetIek(aitisiID);
            int eidikotita = Common.GetEidikotita(aitisiID);

            using (var db = new PegasusDBEntities())
            {
                // Έλεγχος για null: NULL SPOT
                var qryExist = from a in db.AITISIS
                               where a.PROKIRIXI_ID != current_prokirixi && a.TRANSFERRED == false && (a.AFM == afm && a.EIDIKOTITA == eidikotita)
                               select a;
                if (qryExist.Count() > 0)
                {
                    var aitiseis = (from a in db.AITISIS
                                    where a.PROKIRIXI_ID != current_prokirixi && a.TRANSFERRED != true && (a.AFM == afm && a.EIDIKOTITA == eidikotita)
                                    select a).ToList();

                    foreach (var aitisi in aitiseis)
                    {
                        AITISIS target_aitisi = aitisi;
                        target_aitisi.TRANSFERRED = true;
                        db.Entry(target_aitisi).State = EntityState.Modified;
                        db.SaveChanges();
                    }

                    // set current aitisi field to false just in case
                    var currentAitisi = (from a in db.AITISIS
                                         where a.AITISI_ID == aitisiID
                                         select a).FirstOrDefault();
                    currentAitisi.TRANSFERRED = false;
                    db.Entry(currentAitisi).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Ενώνει σε λίστα παλαιές και νέες διδακτικές προϋπηρεσίες που αντιστοιχούν στον κωδικό αίτησης
        /// (τρέχουσα αίτηση) και την αποθηκεύει στη Βάση (EXP_TEACHING).
        /// Προσθήκη 15/3/2016
        /// </summary>
        /// <param name="aitisiID"></param>
        public static void TransferDidaktikes(int aitisiID)
        {
            int current_aitisiID = aitisiID;

            int current_prokirixi = Common.GetOpenProkirixiID(true);
            if (!(current_prokirixi > 0))
            {
                return;
            }
            string afm = Common.GetAFM(aitisiID);
            int iek = Common.GetIek(aitisiID);
            int eidikotita = Common.GetEidikotita(aitisiID);

            List<EXP_TEACHING> new_didaktikes = new List<EXP_TEACHING>();
            List<EXP_TEACHING> all_didaktikes = new List<EXP_TEACHING>();

            // --------------------------------------------------------------------
            // MAYBE NEED TO CHANGE WHICH PROKIRIXI IT GETS
            // Only the previous prokirixi is required, otherwise we get duplicates
            // ---------------------------------------------------------------------

            using (var db = new PegasusDBEntities())
            {
                var aitiseis = (from a in db.AITISIS
                                where a.PROKIRIXI_ID != current_prokirixi && a.TRANSFERRED != true && (a.AFM == afm && a.EIDIKOTITA == eidikotita)
                                select a).ToList();
                var qnew_didaktikes = from d in db.EXP_TEACHING
                                      where d.AITISI_ID == current_aitisiID
                                      select d;

                // Έλεγχος για null εδώ: NULL SPOT 1
                if (qnew_didaktikes.Count() > 0)
                {
                    new_didaktikes = (from d in db.EXP_TEACHING
                                      where d.AITISI_ID == current_aitisiID
                                      select d).ToList();
                    all_didaktikes = new_didaktikes;
                }
                // at this point we can proceed: (all_didaktikes = either empty or filled with found collection)
                // Για κάθε Αίτηση βρες τις διδακτικές προϋπηρεσίες
                foreach (var aitisi in aitiseis)
                {
                    int old_aitisiID = aitisi.AITISI_ID;
                    // βρες τις διδακτικές αυτής της (παλαιάς) Αίτησης
                    var previous_didaktikes = from d in db.EXP_TEACHING
                                              where d.AITISI_ID == old_aitisiID
                                              select d;

                    // Έλεγχος για null εδώ: NULL SPOT 2
                    if (previous_didaktikes.Count() > 0)
                    {
                        var old_didaktikes = (from d in db.EXP_TEACHING
                                              where d.AITISI_ID == old_aitisiID
                                              select d).ToList();

                        foreach (var didaktiki in old_didaktikes)
                        {
                            didaktiki.AITISI_ID = current_aitisiID;
                            // πρόσθεσε αυτή στη συλλογή διδακτικών                     
                            db.EXP_TEACHING.Add(didaktiki);
                            db.SaveChanges();
                        } // foreach didaktiki loop

                    } // end if
                } // foreach aitisi loop
            }
        }

        /// <summary>
        /// Ενώνει σε λίστα παλαιές και νέες επαγγελματικές προϋπηρεσίες που αντιστοιχούν στον κωδικό αίτησης
        /// (τρέχουσα αίτηση) και την αποθηκεύει στη Βάση (EXP_VOCATIONAL).
        /// Προσθήκη 15/3/2016
        /// </summary>
        /// <param name="aitisiID"></param>
        public static void TransferVocational(int aitisiID)
        {
            int current_aitisiID = aitisiID;

            int current_prokirixi = Common.GetOpenProkirixiID(true);
            if (!(current_prokirixi > 0))
            {
                return;
            }
            string afm = Common.GetAFM(aitisiID);
            int iek = Common.GetIek(aitisiID);
            int eidikotita = Common.GetEidikotita(aitisiID);

            List<EXP_VOCATIONAL> new_vocational = new List<EXP_VOCATIONAL>();
            List<EXP_VOCATIONAL> all_vocational = new List<EXP_VOCATIONAL>();

            using (var db = new PegasusDBEntities())
            {
                var aitiseis = (from a in db.AITISIS
                                where a.PROKIRIXI_ID != current_prokirixi && a.TRANSFERRED != true && (a.AFM == afm && a.EIDIKOTITA == eidikotita)
                                select a).ToList();
                var qnew_vocational = from d in db.EXP_VOCATIONAL
                                      where d.AITISI_ID == current_aitisiID
                                      select d;

                // Έλεγχος για null εδώ: NULL SPOT 1
                if (qnew_vocational.Count() > 0)
                {
                    new_vocational = (from d in db.EXP_VOCATIONAL
                                      where d.AITISI_ID == current_aitisiID
                                      select d).ToList();
                    all_vocational = new_vocational;
                }
                // at this point we can proceed: (all_didaktikes = either empty or filled with found collection)
                // Για κάθε Αίτηση βρες τις διδακτικές προϋπηρεσίες
                foreach (var aitisi in aitiseis)
                {
                    int old_aitisiID = aitisi.AITISI_ID;
                    // βρες τις διδακτικές αυτής της (παλαιάς) Αίτησης
                    var previous_vocational = from d in db.EXP_VOCATIONAL
                                              where d.AITISI_ID == old_aitisiID
                                              select d;

                    // Έλεγχος για null εδώ: NULL SPOT 2
                    if (previous_vocational.Count() > 0)
                    {
                        var old_vocational = (from d in db.EXP_VOCATIONAL
                                              where d.AITISI_ID == old_aitisiID
                                              select d).ToList();

                        foreach (var vocational in old_vocational)
                        {
                            vocational.AITISI_ID = current_aitisiID;
                            // πρόσθεσε αυτή στην αρχική συλλογή
                            db.EXP_VOCATIONAL.Add(vocational);
                            db.SaveChanges();
                        } // foreach didaktiki loop
                    } // end if
                } // foreach aitisi loop
            }
        }

        /// <summary>
        /// Ενώνει σε λίστα παλαιές και νέες ελ. επάγγελμα προϋπηρεσίες που αντιστοιχούν στον κωδικό αίτησης
        /// (τρέχουσα αίτηση) και την αποθηκεύει στη Βάση (EXP_FREELANCE).
        /// Προσθήκη 15/3/2016
        /// </summary>
        /// <param name="aitisiID"></param>
        public static void TransferFreelance(int aitisiID)
        {
            int current_aitisiID = aitisiID;

            int current_prokirixi = Common.GetOpenProkirixiID(true);
            if (!(current_prokirixi > 0))
            {
                return;
            }
            string afm = Common.GetAFM(aitisiID);
            int iek = Common.GetIek(aitisiID);
            int eidikotita = Common.GetEidikotita(aitisiID);

            List<EXP_FREELANCE> new_freelance = new List<EXP_FREELANCE>();
            List<EXP_FREELANCE> all_freelance = new List<EXP_FREELANCE>();

            using (var db = new PegasusDBEntities())
            {
                var aitiseis = (from a in db.AITISIS
                                where a.PROKIRIXI_ID != current_prokirixi && a.TRANSFERRED == false && (a.AFM == afm && a.EIDIKOTITA == eidikotita)
                                select a).ToList();
                var qnew_freelance = from d in db.EXP_FREELANCE
                                     where d.AITISI_ID == current_aitisiID
                                     select d;

                // Έλεγχος για null εδώ: NULL SPOT 1
                if (qnew_freelance.Count() > 0)
                {
                    new_freelance = (from d in db.EXP_FREELANCE
                                     where d.AITISI_ID == current_aitisiID
                                     select d).ToList();
                    all_freelance = new_freelance;
                }
                // at this point we can proceed: (all_didaktikes = either empty or filled with found collection)
                // Για κάθε Αίτηση βρες τις διδακτικές προϋπηρεσίες
                foreach (var aitisi in aitiseis)
                {
                    int old_aitisiID = aitisi.AITISI_ID;
                    // βρες τις διδακτικές αυτής της (παλαιάς) Αίτησης
                    var previous_freelance = from d in db.EXP_FREELANCE
                                             where d.AITISI_ID == old_aitisiID
                                             select d;

                    // Έλεγχος για null εδώ: NULL SPOT 2
                    if (previous_freelance.Count() > 0)
                    {
                        var old_freelance = (from d in db.EXP_FREELANCE
                                             where d.AITISI_ID == old_aitisiID
                                             select d).ToList();

                        foreach (var freelance in old_freelance)
                        {
                            freelance.AITISI_ID = current_aitisiID;
                            // πρόσθεσε αυτή στην αρχική συλλογή
                            db.EXP_FREELANCE.Add(freelance);
                            db.SaveChanges();
                        } // foreach didaktiki loop
                    } // end if
                } // foreach aitisi loop
            }
        }

        #endregion


        #region DUPLICATES MANIPULATION

        /// <summary>
        /// Ελέγχει αν υπάρχουν πιθανές διπλότυπες διδακτικές με κριτήρια της παραμέτρου.
        /// </summary>
        /// <param name="school_year"></param>
        /// <param name="dateStart"></param>
        /// <param name="dateFinal"></param>
        /// <param name="hours"></param>
        /// <returns>found_status</returns>
        public static bool IsDuplicateTeaching(EXP_TEACHING didaktiki)
        {
            bool found_status = false;

            int teach_type = (int)didaktiki.TEACH_TYPE;
            int school_year = (int)didaktiki.SCHOOL_YEAR;
            DateTime date_from = (DateTime)didaktiki.DATE_FROM;
            DateTime date_to = (DateTime)didaktiki.DATE_TO;
            int aitisiId = didaktiki.AITISI_ID;

            using (var db = new PegasusDBEntities())
            {
                var existingDuplicates = db.EXP_TEACHING
                    .Where(d => (d.AITISI_ID == aitisiId && d.TEACH_TYPE == teach_type && d.SCHOOL_YEAR == school_year && d.DATE_FROM == date_from && d.DATE_TO == date_to))
                    .ToList();

                int numOfDuplicates = existingDuplicates.Count();
                if (numOfDuplicates > 1) found_status = true;

                return found_status;
            }
        }

        /// <summary>
        /// Ελέγχει αν υπάρχουν πιθανές διπλότυπες επαγγελματικές με κριτήρια της παραμέτρου.
        /// </summary>
        /// <param name="vocation"></param>
        /// <returns></returns>
        public static bool IsDuplicateVocational(EXP_VOCATIONAL vocation)
        {
            bool found_status = false;
            int days_auto = 0;
            int days_manual = 0;
            var existingDuplicates = new List<EXP_VOCATIONAL>();

            DateTime date_from = (DateTime)vocation.DATE_FROM;
            DateTime date_to = (DateTime)vocation.DATE_TO;
            int aitisiId = vocation.AITISI_ID;

            using (var db = new PegasusDBEntities())
            {
                if (vocation.DAYS_AUTO > 0)
                {
                    days_auto = (int)vocation.DAYS_AUTO;
                    existingDuplicates = db.EXP_VOCATIONAL
                        .Where(d => (d.AITISI_ID == aitisiId && d.DATE_FROM == date_from && d.DATE_TO == date_to && d.DAYS_AUTO == days_auto))
                        .ToList();
                }
                else if (vocation.DAYS_MANUAL > 0)
                {
                    days_manual = (int)vocation.DAYS_MANUAL;
                    existingDuplicates = db.EXP_VOCATIONAL
                        .Where(d => (d.AITISI_ID == aitisiId && d.DATE_FROM == date_from && d.DATE_TO == date_to && d.DAYS_MANUAL == days_manual))
                        .ToList();
                }
                else
                {
                    existingDuplicates = db.EXP_VOCATIONAL.Where(d => (d.AITISI_ID == aitisiId && d.DATE_FROM == date_from && d.DATE_TO == date_to)).ToList();
                }
                int numOfDuplicates = existingDuplicates.Count();
                if (numOfDuplicates > 1) found_status = true;

                return found_status;
            }
        }

        public static bool IsDuplicateFreelance(EXP_FREELANCE freelance)
        {
            bool found_status = false;

            DateTime date_from = (DateTime)freelance.DATE_FROM;
            DateTime date_to = (DateTime)freelance.DATE_TO;
            float income = (float)freelance.INCOME;
            int aitisiId = freelance.AITISI_ID;

            using (var db = new PegasusDBEntities())
            {
                var existingDuplicates = db.EXP_FREELANCE
                    .Where(d => (d.AITISI_ID == aitisiId && d.DATE_FROM == date_from && d.DATE_TO == date_to && d.INCOME == income))
                    .ToList();

                int numOfDuplicates = existingDuplicates.Count();
                if (numOfDuplicates > 1) found_status = true;

                return found_status;
            }
        }


        // NOT WORKING!
        /// <summary>
        /// Θέτει το πεδίο DUPLICATE σε {true, false}. Βασικά μόνο η τιμή true χρησιμοποιείται.
        /// </summary>
        /// <param name="teachingRecord"></param>
        /// <param name="value"></param>
        public static void MarkDuplicateTeaching(EXP_TEACHING didaktiki, bool value)
        {
            using (var db = new PegasusDBEntities())
            {
                EXP_TEACHING modDidaktiki = didaktiki;//db.EXP_TEACHING.Find(didaktiki.EXP_ID);
                modDidaktiki.DUPLICATE = value;
                db.Entry(modDidaktiki).State = EntityState.Modified;
                db.SaveChanges();
            }
        }

        #endregion

    }   // class Services
}   // namespace