using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using Pegasus.DAL;
using Pegasus.Models;
using Pegasus.BPM;
using Pegasus.Notification;
using Pegasus.Extensions;
using Pegasus.Filters;
using Pegasus.Services;

namespace Pegasus.Controllers.DataControllers
{
    [ErrorHandlerFilter]
    public class AITISISController : Controller
    {
        private readonly PegasusDBEntities db;
        private USER_TEACHERS loggedTeacher;
        private TEACHERS loggedTeacherData;

        private const string ENSTASEIS_PATH = "~/Uploads/Enstaseis/";

        private readonly IEnstaseisService enstaseisService;
        private readonly IAitisiTeacherService aitisiTeacherService;
        private readonly IAitisiSchoolsService aitisiSchoolsService;

        public AITISISController(PegasusDBEntities entities, IEnstaseisService enstaseisService,
            IAitisiTeacherService aitisiTeacherService, IAitisiSchoolsService aitisiSchoolsService)
        {
            db = entities;

            this.enstaseisService = enstaseisService;
            this.aitisiTeacherService = aitisiTeacherService;
            this.aitisiSchoolsService = aitisiSchoolsService;
        }


        #region ΕΠΕΞΕΡΓΑΣΙΑ - ΕΚΤΥΠΩΣΗ ΑΙΤΗΣΗΣ

        public ActionResult AitisiCreate()
        {
            bool val1 = (System.Web.HttpContext.Current.User != null) && System.Web.HttpContext.Current.User.Identity.IsAuthenticated;
            if (!val1)
            {
                ViewBag.loggedUser = "(χωρίς σύνδεση)";
                return RedirectToAction("TaxisNetLogin", "USER_TEACHERS");
            }
            else
            {
                loggedTeacher = GetLoginTeacher();
            }

            int prokirixiId = Common.GetOpenProkirixiID();
            if (prokirixiId == 0)
            {
                this.ShowMessage(MessageType.Warning, "Δεν βρέθηκε Ανοικτή Προκήρυξη.");
                return RedirectToAction("Index", "TEACHERS");
            }
            AitisisViewModel entity = new AitisisViewModel() 
            { AFM = loggedTeacher.USER_AFM, 
              AITISIS_SCHOOLS = new List<AITISIS_SCHOOLS>(), 
              AITISI_DATE = DateTime.Now.Date, 
              PROKIRIXI_ID = prokirixiId
            };

            // try to select an existing teacher by AFM
            var data = (from d in db.TEACHERS where d.AFM == loggedTeacher.USER_AFM select d).FirstOrDefault();
            if (data == null) 
            {
                string msg = "Δεν βρέθηκε εκπαιδευτικός με αυτό το ΑΦΜ. Καταχωρήστε πρώτα τα ατομικά στοιχεία.";
                return RedirectToAction("Index", "TEACHERS", new { notify = msg });
            }
            entity.AGE = AitisiRules.CalcAge(entity);
            return View(entity);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AitisiCreate(AitisisViewModel data)
        {
            bool val1 = (System.Web.HttpContext.Current.User != null) && System.Web.HttpContext.Current.User.Identity.IsAuthenticated;
            if (!val1)
            {
                ViewBag.loggedUser = "(χωρίς σύνδεση)";
                return RedirectToAction("TaxisNetLogin", "USER_TEACHERS");
            }
            else
            {
                loggedTeacher = GetLoginTeacher();
            }

            int prokirixiId = Common.GetOpenProkirixiID();
            if (prokirixiId == 0)
            {
                this.ShowMessage(MessageType.Warning, "Δεν βρέθηκε Ανοικτή Προκήρυξη.");
                return RedirectToAction("Index", "TEACHERS");
            }

            data.AFM = loggedTeacher.USER_AFM;
            string ErrorMsg = AitisiRules.ValidateFields(data, isnew: true);
            if (!string.IsNullOrEmpty(ErrorMsg))
            {
                this.AddNotification("Η αποθήκευση απέτυχε λόγω επικύρωσης δεδομένων. " + ErrorMsg, NotificationType.ERROR);
                return View(data);
            }

            if (ModelState.IsValid)
            {
                AITISIS entity = aitisiTeacherService.Create(data, prokirixiId, loggedTeacher.USER_AFM);

                this.AddNotification("Η αποθήκευση της αίτησης ολοκληρώθηκε με επιτυχία.", NotificationType.SUCCESS);

                AitisisViewModel newdata = aitisiTeacherService.GetModel(entity.AITISI_ID);
                // CREATE COPY OF ORIGINAL AITISI - NEW ADDITION 08/06/2018
                bool result = CreateOriginal(newdata);
                if (result)
                    this.AddNotification("Η δημιουργία αντιγράφου της αίτησης ολοκληρώθηκε με επιτυχία.", NotificationType.SUCCESS);
                else
                    this.AddNotification("Η δημιουργία αντιγράφου απέτυχε να ολοκληρωθεί.", NotificationType.WARNING);
                // END OF CREATE COPY
                return RedirectToAction("AitisiEdit", "AITISIS", new { AITISI_ID = newdata.AITISI_ID });
            }
            this.AddNotification("Η αποθήκευση απέτυχε λόγω σφαλμάτων χρήστη. Δείτε τη σύνοψη στο κάτω μέρος της σελίδας.", NotificationType.ERROR);
            return View(data);
        }

        public ActionResult AitisiEdit(int? AITISI_ID)
        {
            bool val1 = (System.Web.HttpContext.Current.User != null) && System.Web.HttpContext.Current.User.Identity.IsAuthenticated;
            if (!val1)
            {
                ViewBag.loggedUser = "(χωρίς σύνδεση)";
                return RedirectToAction("TaxisNetLogin", "USER_TEACHERS");
            }
            else
            {
                loggedTeacher = GetLoginTeacher();
            }

            int prokirixiId = Common.GetOpenProkirixiID();
            if (prokirixiId == 0)
            {
                this.ShowMessage(MessageType.Warning, "Δεν βρέθηκε Ανοικτή Προκήρυξη.");
                return RedirectToAction("Index", "TEACHERS");
            }
            if (AITISI_ID == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            int aitisiID = (int)AITISI_ID;
            AitisisViewModel aitisi = aitisiTeacherService.GetModel(aitisiID);
            
            if (aitisi == null)
            {
                string msg = "Προέκυψε αδυναμία εύρεσης της επιλεγμένης αίτησης.";
                return RedirectToAction("Error", "AITISIS", new { notify = msg });
            }

            return View(aitisi);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AitisiEdit(AitisisViewModel data)
        {
            bool val1 = (System.Web.HttpContext.Current.User != null) && System.Web.HttpContext.Current.User.Identity.IsAuthenticated;
            if (!val1)
            {
                ViewBag.loggedUser = "(χωρίς σύνδεση)";
                return RedirectToAction("TaxisNetLogin", "USER_TEACHERS");
            }
            else
            {
                loggedTeacher = GetLoginTeacher();
            }

            int prokirixiId = Common.GetOpenProkirixiID();
            if (prokirixiId == 0)
            {
                this.ShowMessage(MessageType.Warning, "Δεν βρέθηκε Ανοικτή Προκήρυξη.");
                return RedirectToAction("Index", "TEACHERS");
            }

            int aitisiID;
            if (data == null)
            {
                string msg = "Προέκυψε αδυναμία εύρεσης της επιλεγμένης αίτησης.";
                return RedirectToAction("Error", "AITISIS", new { notify = msg });
            }
            else
            {
                aitisiID = data.AITISI_ID;
            }

            data.AFM = loggedTeacher.USER_AFM;
            string ErrorMsg = AitisiRules.ValidateFields(data, isnew: false);
            if (!string.IsNullOrEmpty(ErrorMsg))
            {
                this.AddNotification("Η αποθήκευση απέτυχε λόγω επικύρωσης δεδομένων. " + ErrorMsg, NotificationType.ERROR);
                return View(data);
            }

            if (ModelState.IsValid)
            {
                AITISIS entity = aitisiTeacherService.Update(data, prokirixiId, loggedTeacher.USER_AFM);

                this.AddNotification("Η αποθήκευση της αίτησης ολοκληρώθηκε με επιτυχία.", NotificationType.SUCCESS);

                AitisisViewModel newdata = aitisiTeacherService.GetModel(entity.AITISI_ID);
                // EDIT COPY OF ORIGINAL AITISI - NEW ADDITION 08/06/2018
                bool result = EditOriginal(newdata);
                if (result)
                    this.AddNotification("Η ενημέρωση του αντιγράφου ολοκληρώθηκε με επιτυχία.", NotificationType.SUCCESS);
                else
                    this.AddNotification("Η ενημέρωση του αντιγράφου απέτυχε να ολοκληρωθεί.", NotificationType.WARNING);
                // END OF EDIT COPY
                return View(newdata);
            }
            this.AddNotification("Η αποθήκευση απέτυχε λόγω σφαλμάτων χρήστη. Δείτε τη σύνοψη στο κάτω μέρος της σελίδας.", NotificationType.ERROR);
            AitisisViewModel model = aitisiTeacherService.GetModel(aitisiID);
            return View(model);
        }

        public ActionResult AitisiEdit2(int? AITISI_ID)
        {
            bool val1 = (System.Web.HttpContext.Current.User != null) && System.Web.HttpContext.Current.User.Identity.IsAuthenticated;
            if (!val1)
            {
                ViewBag.loggedUser = "(χωρίς σύνδεση)";
                return RedirectToAction("TaxisNetLogin", "USER_TEACHERS");
            }
            else
            {
                loggedTeacher = GetLoginTeacher();
            }

            int prokirixiId = Common.GetOpenProkirixiID();
            if (prokirixiId == 0)
            {
                this.ShowMessage(MessageType.Warning, "Δεν βρέθηκε Ανοικτή Προκήρυξη.");
                return RedirectToAction("Index", "TEACHERS");
            }
            if (AITISI_ID == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            int aitisiID = (int)AITISI_ID;
            AitisisViewModel aitisi = aitisiTeacherService.GetModel(aitisiID);

            if (aitisi == null)
            {
                string msg = "Προέκυψε αδυναμία εύρεσης της επιλεγμένης αίτησης.";
                return RedirectToAction("Error", "AITISIS", new { notify = msg });
            }
            return View(aitisi);
        }

        [HttpPost]
        public ActionResult AitisiEdit2(AitisisViewModel data)
        {
            bool val1 = (System.Web.HttpContext.Current.User != null) && System.Web.HttpContext.Current.User.Identity.IsAuthenticated;
            if (!val1)
            {
                ViewBag.loggedUser = "(χωρίς σύνδεση)";
                return RedirectToAction("TaxisNetLogin", "USER_TEACHERS");
            }
            else
            {
                loggedTeacher = GetLoginTeacher();
            }

            int prokirixiId = Common.GetOpenProkirixiID();
            if (prokirixiId == 0)
            {
                this.ShowMessage(MessageType.Warning, "Δεν βρέθηκε Ανοικτή Προκήρυξη.");
                return RedirectToAction("Index", "TEACHERS");
            }

            int aitisiID;
            if (data == null)
            {
                string msg = "Προέκυψε αδυναμία εύρεσης της επιλεγμένης αίτησης.";
                return RedirectToAction("Error", "AITISIS", new { notify = msg });
            }
            else
            {
                aitisiID = data.AITISI_ID;
            }

            data.AFM = loggedTeacher.USER_AFM;
            string ErrorMsg = AitisiRules.ValidateFields(data, isnew: false);
            if (!string.IsNullOrEmpty(ErrorMsg))
            {
                this.AddNotification("Η αποθήκευση απέτυχε λόγω επικύρωσης δεδομένων. " + ErrorMsg, NotificationType.ERROR);
                return View(data);
            }

            if (ModelState.IsValid)
            {
                AITISIS entity = aitisiTeacherService.Update(data, prokirixiId, loggedTeacher.USER_AFM);

                this.AddNotification("Η αποθήκευση της αίτησης ολοκληρώθηκε με επιτυχία.", NotificationType.SUCCESS);
                AitisisViewModel newdata = aitisiTeacherService.GetModel(entity.AITISI_ID);

                // EDIT COPY OF ORIGINAL AITISI - NEW ADDITION 08/06/2018
                bool result = EditOriginal(newdata);
                if (result)
                    this.AddNotification("Η ενημέρωση του αντιγράφου ολοκληρώθηκε με επιτυχία.", NotificationType.SUCCESS);
                else
                    this.AddNotification("Η ενημέρωση του αντιγράφου απέτυχε να ολοκληρωθεί.", NotificationType.WARNING);
                // END OF EDIT COPY
                return View(newdata);
            }
            this.AddNotification("Η αποθήκευση απέτυχε λόγω σφαλμάτων χρήστη. Δείτε τη σύνοψη στο κάτω μέρος της σελίδας.", NotificationType.ERROR);
            AitisisViewModel model = aitisiTeacherService.GetModel(aitisiID);
            return View(model);
        }

        public ActionResult AitisiPrint(int? AITISI_ID)
        {
            bool val1 = (System.Web.HttpContext.Current.User != null) && System.Web.HttpContext.Current.User.Identity.IsAuthenticated;
            if (!val1)
            {
                ViewBag.loggedUser = "(χωρίς σύνδεση)";
                return RedirectToAction("TaxisNetLogin", "USER_TEACHERS");
            }
            else
            {
                loggedTeacher = GetLoginTeacher();
            }
            var data = (from t in db.AITISIS
                        where t.AFM == loggedTeacher.USER_AFM && t.AITISI_ID == AITISI_ID
                        select new AitisisViewModel
                        {
                            AITISI_ID = t.AITISI_ID,
                            AITISI_PROTOCOL = t.AITISI_PROTOCOL,
                            AITISI_DATE = t.AITISI_DATE,
                            AFM = t.AFM
                        }).FirstOrDefault();
            return View(data);
        }

        #endregion


        #region ΔΙΑΧΕΙΡΙΣΗ ΑΙΤΗΣΕΩΝ

        public ActionResult ListAitisis()
        {
            bool val1 = (System.Web.HttpContext.Current.User != null) && System.Web.HttpContext.Current.User.Identity.IsAuthenticated;
            if (!val1)
            {
                return RedirectToAction("TaxisNetLogin", "USER_TEACHERS");
            }
            else
            {
                loggedTeacher = GetLoginTeacher();
            }

            int prokirixiId = Common.GetOpenProkirixiID();
            if (prokirixiId == 0)
            {
                this.ShowMessage(MessageType.Warning, "Δεν βρέθηκε Ανοικτή Προκήρυξη.");
                return RedirectToAction("Index", "TEACHERS");
            }
            // first, find the teacher if he/she exists
            var data = (from d in db.TEACHERS where d.AFM == loggedTeacher.USER_AFM select d).FirstOrDefault();
            if (data == null)
            {
                string msg = "Δεν βρέθηκε εκπαιδευτικός με αυτό το ΑΦΜ. Καταχωρήστε πρώτα τα ατομικά στοιχεία.";
                return RedirectToAction("Index", "TEACHERS", new { notify = msg });
            }
            PopulatePeriferiesByUser();
            PopulateEidikotites();
            PopulateSchoolTypes();
            PopulateSchools();
            return View();
        }

        public ActionResult Aitisis_Read([DataSourceRequest] DataSourceRequest request)
        {
            int prokirixiId = Common.GetOpenProkirixiID();
            string teacherAFM = GetLoginTeacher().USER_AFM;

            IEnumerable<AitisisGridViewModel> data = aitisiTeacherService.Read(prokirixiId, teacherAFM);

            return Json(data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        public ActionResult Aitisis_Destroy([DataSourceRequest] DataSourceRequest request, AitisisGridViewModel data)
        {
            if (Common.CanDeleteAitisi(data.AITISI_ID))
            {
                aitisiTeacherService.Destroy(data);
            }
            else
            {
                ModelState.AddModelError("AITISIS_SCHOOLS", "Για διαγραφή αίτησης πρέπει πρώτα να διαγραφούν οι σχολικές μονάδες της αίτησης.");
            }
            return Json(new[] { data }.ToDataSourceResult(request, ModelState));
        }

        public void AitisiOriginalDestroy(int aitisiId)
        {
            AITISIS_ORIGINAL entity = db.AITISIS_ORIGINAL.Find(aitisiId);
            if (entity != null)
            {
                db.Entry(entity).State = EntityState.Deleted;
                db.AITISIS_ORIGINAL.Remove(entity);
                db.SaveChanges();
            }
        }


        #endregion


        #region ΣΧΟΛΕΙΑ ΑΙΤΗΣΗΣ

        public ActionResult ListSchools(int? AITISI_ID)
        {
            //check if user is unauthenticated to redirect him
            bool val1 = (System.Web.HttpContext.Current.User != null) && System.Web.HttpContext.Current.User.Identity.IsAuthenticated;
            if (!val1)
            {
                return RedirectToAction("TaxisNetLogin", "USER_TEACHERS");
            }
            else
            {
                loggedTeacher = GetLoginTeacher();

                var selectedAitisi = (from a in db.AITISIS
                                          where a.AITISI_ID == AITISI_ID
                                          select new AitisisViewModel()
                                          {
                                              AITISI_ID = a.AITISI_ID,
                                              AITISI_PROTOCOL = a.AITISI_PROTOCOL,
                                              AITISI_DATE = a.AITISI_DATE,
                                              PERIFERIA_ID = a.PERIFERIA_ID,
                                              EIDIKOTITA = a.EIDIKOTITA,
                                              KLADOS = a.KLADOS
                                          }).FirstOrDefault();
                if (selectedAitisi != null) 
                {
                    PopulateViewBagWithAitisi(selectedAitisi);
                    PopulateSchoolTypes(selectedAitisi.AITISI_ID);
                    PopulateSchools(selectedAitisi.AITISI_ID);
                }
                else
                {
                    return RedirectToAction("ListAitisis", "AITISIS");
                }
            }
            return View();
        }

        public ActionResult Schools_Read([DataSourceRequest] DataSourceRequest request, int aitisiID = 0)
        {
            PopulateSchoolTypes(aitisiID);
            PopulateSchools(aitisiID);

            List<AITISI_SCHOOLSViewModel> data = aitisiSchoolsService.Read(aitisiID);

            return Json(data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Schools_Create([DataSourceRequest] DataSourceRequest request, AITISI_SCHOOLSViewModel data, int aitisiID)
        {
            var newdata = new AITISI_SCHOOLSViewModel();
            int prokirixiId = Common.GetOpenProkirixiID();

            if (!(aitisiID > 0)) 
                ModelState.AddModelError("", "Πρέπει να επιλέξετε πρώτα μια αίτηση με κλικ επάνω της.");

            if (data != null && ModelState.IsValid)
            {
                var savedSchools = db.AITISIS_SCHOOLS.Where(x => x.AITISI_ID == aitisiID && x.SCHOOL == data.SCHOOL).Count();
                if (savedSchools == 0)
                {
                    aitisiSchoolsService.Create(data, prokirixiId, aitisiID);
                    newdata = aitisiSchoolsService.Refresh(data.ID);
                }
            }
            return Json(new[] { newdata }.ToDataSourceResult(request, ModelState), JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Schools_Update([DataSourceRequest] DataSourceRequest request, AITISI_SCHOOLSViewModel data, int aitisiID)
        {
            var newdata = new AITISI_SCHOOLSViewModel();
            int prokirixiId = Common.GetOpenProkirixiID();

            if (!(aitisiID > 0)) ModelState.AddModelError("", "Πρέπει να επιλέξετε πρώτα μια αίτηση με κλικ επάνω της.");

            if (data != null && ModelState.IsValid)
            {
                aitisiSchoolsService.Update(data, prokirixiId, aitisiID);
                newdata = aitisiSchoolsService.Refresh(data.ID);
            }
            return Json(new[] { newdata }.ToDataSourceResult(request, ModelState), JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Schools_Destroy([DataSourceRequest] DataSourceRequest request, AITISI_SCHOOLSViewModel data)
        {
            if (data != null)
            {
                aitisiSchoolsService.Destroy(data);
            }
            return Json(new[] { data }.ToDataSourceResult(request, ModelState), JsonRequestBehavior.AllowGet);
        }

        #endregion


        #region ΠΡΩΤΟΤΥΠΕΣ ΑΙΤΗΣΕΙΣ - ΝΕΟ (2018-06-08)

        public ActionResult ListAitisisOriginal()
        {
            bool val1 = (System.Web.HttpContext.Current.User != null) && System.Web.HttpContext.Current.User.Identity.IsAuthenticated;
            if (!val1)
            {
                return RedirectToAction("TaxisNetLogin", "USER_TEACHERS");
            }
            else
            {
                loggedTeacher = GetLoginTeacher();
            }
            // first, find the teacher if he/she exists
            var data = (from d in db.TEACHERS where d.AFM == loggedTeacher.USER_AFM select d).FirstOrDefault();
            if (data == null)
            {
                string msg = "Δεν βρέθηκε εκπαιδευτικός με αυτό το ΑΦΜ. Καταχωρήστε πρώτα τα ατομικά στοιχεία.";
                return RedirectToAction("Index", "TEACHERS", new { notify = msg });
            }
            var aitisis = (from a in db.AITISIS_ORIGINAL
                           where a.AFM == loggedTeacher.USER_AFM
                           select new AitisisOriginalViewModel
                           {
                               AITISI_ID = a.AITISI_ID,
                               AITISI_PROTOCOL = a.AITISI_PROTOCOL,
                               EIDIKOTITA = a.EIDIKOTITA,
                               SCHOOL_ID = a.SCHOOL_ID,
                               PERIFERIA_ID = a.PERIFERIA_ID
                           }).ToList();

            PopulatePeriferiesByUser();
            PopulateEidikotites();
            PopulateSchoolTypes();
            PopulateSchools();
            return View(aitisis);
        }

        public ActionResult AitisisOriginal_Read([DataSourceRequest] DataSourceRequest request)
        {
            TEACHERS teacher = db.TEACHERS.Where(m => m.AFM == System.Web.HttpContext.Current.User.Identity.Name).FirstOrDefault();

            var aitisis = (from a in db.AITISIS_ORIGINAL
                           where a.AFM == teacher.AFM
                           select new AitisisOriginalViewModel
                           {
                               AITISI_ID = a.AITISI_ID,
                               AITISI_PROTOCOL = a.AITISI_PROTOCOL,
                               EIDIKOTITA = a.EIDIKOTITA,
                               PERIFERIA_ID = a.PERIFERIA_ID
                           }).ToList();

            DataSourceResult result = aitisis.ToDataSourceResult(request);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public bool CreateOriginal(AitisisViewModel model)
        {
            loggedTeacher = GetLoginTeacher();

            if (model == null) return false;

            AITISIS_ORIGINAL entity = new AITISIS_ORIGINAL()
            {
                AITISI_ID = model.AITISI_ID,
                PROKIRIXI_ID = model.PROKIRIXI_ID,
                AFM = model.AFM,
                AITISI_DATE = model.AITISI_DATE,
                AITISI_PROTOCOL = model.AITISI_PROTOCOL,
                ANERGIA = model.ANERGIA,
                SOCIALGROUP = model.SOCIALGROUP,
                SOCIALGROUP_PROTOCOL = model.SOCIALGROUP_PROTOCOL,
                SOCIALGROUP_YPIRESIA = model.SOCIALGROUP_YPIRESIA,
                N2190 = model.N2190,
                KLADOS = model.KLADOS,
                EIDIKOTITA = model.EIDIKOTITA,
                EIDIKOTITA_GROUP = model.EIDIKOTITA_GROUP,
                BASIC_EDUCATION = model.BASIC_EDUCATION,
                PTYXIO_TITLOS = model.PTYXIO_TITLOS,
                PTYXIO_BATHMOS = model.PTYXIO_BATHMOS,
                PTYXIO_DATE = model.PTYXIO_DATE,
                MSC = model.MSC,
                MSC_DIARKEIA = model.MSC_DIARKEIA,
                MSC_TITLOS = model.MSC_TITLOS,
                PHD = model.PHD,
                PHD_TITLOS = model.PHD_TITLOS,
                PED = model.PED,
                PED_TITLOS = model.PED_TITLOS,
                PED_DIARKEIA = model.PED_DIARKEIA,
                AED_MSC = model.AED_MSC,
                AED_MSC_TITLOS = model.AED_MSC_TITLOS,
                AED_PHD = model.AED_PHD,
                AED_PHD_TITLOS = model.AED_PHD_TITLOS,
                LANG_TEXT = model.LANG_TEXT,
                LANG_LEVEL = model.LANG_LEVEL,
                LANG_TITLOS = model.LANG_TITLOS,
                COMPUTER_CERT = model.COMPUTER_CERT,
                COMPUTER_TITLOS = model.COMPUTER_TITLOS,
                EPIMORFOSI1 = model.EPIMORFOSI1,
                EPIMORFOSI1_HOURS = model.EPIMORFOSI1_HOURS,
                EPIMORFOSI2 = model.EPIMORFOSI2,
                EPIMORFOSI2_HOURS = model.EPIMORFOSI2_HOURS,
                PERIFERIA_ID = model.PERIFERIA_ID,
                SCHOOL_ID = model.SCHOOL_ID,
                EPAGELMA_STATUS = model.EPAGELMA_STATUS,
                // new fields (2016)
                CERTIFIED = model.CERTIFIED,
                SOCIALGROUP1 = model.SOCIALGROUP1,
                SOCIALGROUP2 = model.SOCIALGROUP2,
                SOCIALGROUP3 = model.SOCIALGROUP3,
                SOCIALGROUP4 = model.SOCIALGROUP4,
                SOCIALGROUP1_DOC = model.SOCIALGROUP1_DOC,
                SOCIALGROUP2_DOC = model.SOCIALGROUP2_DOC,
                SOCIALGROUP3_DOC = model.SOCIALGROUP3_DOC,
                SOCIALGROUP4_DOC = model.SOCIALGROUP4_DOC,
                LANG1_TEXT = model.LANG1_TEXT,
                LANG1_LEVEL = model.LANG1_LEVEL,
                LANG1_TITLOS = model.LANG1_TITLOS,
                LANG2_TEXT = model.LANG2_TEXT,
                LANG2_LEVEL = model.LANG2_LEVEL,
                LANG2_TITLOS = model.LANG2_TITLOS,
                EPIMORFOSI3 = model.EPIMORFOSI3,
                EPIMORFOSI3_HOURS = model.EPIMORFOSI3_HOURS,
                AGE = model.AGE,
                MORIA_ANERGIA = model.MORIA_ANERGIA,
                MORIA_PTYXIO = model.MORIA_PTYXIO,
                MORIA_MSC = model.MORIA_MSC,
                MORIA_PHD = model.MORIA_PHD,
                MORIA_PED = model.MORIA_PED,
                MORIA_AED_MSC = model.MORIA_AED_MSC,
                MORIA_AED_PHD = model.MORIA_AED_PHD,
                MORIA_LANG = model.MORIA_LANG,
                MORIA_COMPUTER = model.MORIA_COMPUTER,
                MORIA_EPIMORFOSI1 = model.MORIA_EPIMORFOSI1,
                MORIA_EPIMORFOSI2 = model.MORIA_EPIMORFOSI2,
                MORIA_EPIMORFOSI3 = model.MORIA_EPIMORFOSI3,
                MORIA_TEACH = model.MORIA_TEACH,
                MORIA_WORK1 = model.MORIA_WORK1,
                MORIA_WORK2 = model.MORIA_WORK2,
                MORIA_WORK = model.MORIA_WORK,
                MORIA_SOCIAL = model.MORIA_SOCIAL,
                MORIA_TOTAL = model.MORIA_TOTAL,
                TIMESTAMP = DateTime.Now
            };

            db.Entry(entity).State = EntityState.Added;
            db.AITISIS_ORIGINAL.Add(entity);
            db.SaveChanges();
            return true;
        }

        public bool EditOriginal(AitisisViewModel a)
        {
            AITISIS_ORIGINAL entity = db.AITISIS_ORIGINAL.Find(a.AITISI_ID);
            if (entity != null)
            {
                entity.AITISI_ID = a.AITISI_ID;
                entity.PROKIRIXI_ID = a.PROKIRIXI_ID;
                entity.AFM = a.AFM;
                entity.AITISI_DATE = a.AITISI_DATE;
                entity.AITISI_PROTOCOL = a.AITISI_PROTOCOL;
                entity.ANERGIA = a.ANERGIA;
                entity.SOCIALGROUP = a.SOCIALGROUP;
                entity.SOCIALGROUP_PROTOCOL = a.SOCIALGROUP_PROTOCOL;
                entity.SOCIALGROUP_YPIRESIA = a.SOCIALGROUP_YPIRESIA;
                entity.N2190 = a.N2190;
                entity.KLADOS = a.KLADOS;
                entity.EIDIKOTITA = a.EIDIKOTITA;
                entity.EIDIKOTITA_GROUP = a.EIDIKOTITA_GROUP;
                entity.PTYXIO_TITLOS = a.PTYXIO_TITLOS;
                entity.PTYXIO_BATHMOS = a.PTYXIO_BATHMOS;
                entity.PTYXIO_DATE = a.PTYXIO_DATE;
                entity.MSC = a.MSC;
                entity.MSC_DIARKEIA = a.MSC_DIARKEIA;
                entity.MSC_TITLOS = a.MSC_TITLOS;
                entity.PHD = a.PHD;
                entity.PHD_TITLOS = a.PHD_TITLOS;
                entity.PED = a.PED;
                entity.PED_TITLOS = a.PED_TITLOS;
                entity.PED_DIARKEIA = a.PED_DIARKEIA;
                entity.AED_MSC = a.AED_MSC;
                entity.AED_MSC_TITLOS = a.AED_MSC_TITLOS;
                entity.AED_PHD = a.AED_PHD;
                entity.AED_PHD_TITLOS = a.AED_PHD_TITLOS;
                entity.LANG_TEXT = a.LANG_TEXT;
                entity.LANG_LEVEL = a.LANG_LEVEL;
                entity.LANG_TITLOS = a.LANG_TITLOS;
                entity.COMPUTER_CERT = a.COMPUTER_CERT;
                entity.COMPUTER_TITLOS = a.COMPUTER_TITLOS;
                entity.EPIMORFOSI1 = a.EPIMORFOSI1;
                entity.EPIMORFOSI1_HOURS = a.EPIMORFOSI1_HOURS;
                entity.EPIMORFOSI2 = a.EPIMORFOSI2;
                entity.EPIMORFOSI2_HOURS = a.EPIMORFOSI2_HOURS;
                entity.BASIC_EDUCATION = a.BASIC_EDUCATION;
                entity.PERIFERIA_ID = a.PERIFERIA_ID;
                entity.SCHOOL_ID = a.SCHOOL_ID;
                entity.EPAGELMA_STATUS = a.EPAGELMA_STATUS;
                entity.CHILDREN = a.CHILDREN;
                // new fields (2016)
                entity.CERTIFIED = a.CERTIFIED;
                entity.SOCIALGROUP1 = a.SOCIALGROUP1;
                entity.SOCIALGROUP2 = a.SOCIALGROUP2;
                entity.SOCIALGROUP3 = a.SOCIALGROUP3;
                entity.SOCIALGROUP4 = a.SOCIALGROUP4;
                entity.SOCIALGROUP1_DOC = a.SOCIALGROUP1_DOC;
                entity.SOCIALGROUP2_DOC = a.SOCIALGROUP2_DOC;
                entity.SOCIALGROUP3_DOC = a.SOCIALGROUP3_DOC;
                entity.SOCIALGROUP4_DOC = a.SOCIALGROUP4_DOC;
                entity.LANG1_TEXT = a.LANG1_TEXT;
                entity.LANG1_LEVEL = a.LANG1_LEVEL;
                entity.LANG1_TITLOS = a.LANG1_TITLOS;
                entity.LANG2_TEXT = a.LANG2_TEXT;
                entity.LANG2_LEVEL = a.LANG2_LEVEL;
                entity.LANG2_TITLOS = a.LANG2_TITLOS;
                entity.EPIMORFOSI3 = a.EPIMORFOSI3;
                entity.EPIMORFOSI3_HOURS = a.EPIMORFOSI3_HOURS;
                // Calculated Fields
                entity.AGE = a.AGE;
                entity.MORIA_ANERGIA = a.MORIA_ANERGIA;
                entity.MORIA_PTYXIO = a.MORIA_PTYXIO;
                entity.MORIA_MSC = a.MORIA_MSC;
                entity.MORIA_PHD = a.MORIA_PHD;
                entity.MORIA_PED = a.MORIA_PED;
                entity.MORIA_AED_MSC = a.MORIA_AED_MSC;
                entity.MORIA_AED_PHD = a.MORIA_AED_PHD;
                entity.MORIA_LANG = a.MORIA_LANG;
                entity.MORIA_COMPUTER = a.MORIA_COMPUTER;
                entity.MORIA_EPIMORFOSI1 = a.MORIA_EPIMORFOSI1;
                entity.MORIA_EPIMORFOSI2 = a.MORIA_EPIMORFOSI2;
                entity.MORIA_EPIMORFOSI3 = a.MORIA_EPIMORFOSI3;
                entity.MORIA_TEACH = a.MORIA_TEACH;
                entity.MORIA_WORK1 = a.MORIA_WORK1;
                entity.MORIA_WORK2 = a.MORIA_WORK2;
                entity.MORIA_WORK = a.MORIA_WORK;
                entity.MORIA_SOCIAL = a.MORIA_SOCIAL;
                entity.MORIA_TOTAL = a.MORIA_TOTAL;
                entity.TIMESTAMP = DateTime.Now;

                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                return true;
            }
            else return false;
        }

        public AitisisOriginalViewModel AitisiOriginalViewModelFromDB(int aitisiID)
        {
            AitisisOriginalViewModel dataAitisi;

            dataAitisi = (from qa in db.AITISIS_ORIGINAL
                          where qa.AITISI_ID == aitisiID
                          select new AitisisOriginalViewModel
                          {
                              AITISI_ID = qa.AITISI_ID,
                              AFM = qa.AFM,
                              DOY = qa.TEACHERS.DOY,
                              AMKA = qa.TEACHERS.AMKA,
                              FATHERNAME = qa.TEACHERS.FATHERNAME,
                              LASTNAME = qa.TEACHERS.LASTNAME,
                              FIRSTNAME = qa.TEACHERS.FIRSTNAME,
                              AITISI_DATE = qa.AITISI_DATE,
                              AITISI_PROTOCOL = qa.AITISI_PROTOCOL,
                              ANERGIA = qa.ANERGIA,
                              SOCIALGROUP = qa.SOCIALGROUP,
                              SOCIALGROUP_PROTOCOL = qa.SOCIALGROUP_PROTOCOL,
                              SOCIALGROUP_YPIRESIA = qa.SOCIALGROUP_YPIRESIA,
                              N2190 = qa.N2190,
                              KLADOS = qa.KLADOS,
                              EIDIKOTITA = qa.EIDIKOTITA,
                              EIDIKOTITA_GROUP = qa.EIDIKOTITA_GROUP,
                              BASIC_EDUCATION = qa.BASIC_EDUCATION,
                              PTYXIO_TITLOS = qa.PTYXIO_TITLOS,
                              PTYXIO_BATHMOS = qa.PTYXIO_BATHMOS,
                              PTYXIO_DATE = qa.PTYXIO_DATE,
                              MSC = qa.MSC ?? false,
                              MSC_DIARKEIA = qa.MSC_DIARKEIA,
                              MSC_TITLOS = qa.MSC_TITLOS,
                              PHD = qa.PHD ?? false,
                              PHD_TITLOS = qa.PHD_TITLOS,
                              PED = qa.PED ?? false,
                              PED_TITLOS = qa.PED_TITLOS,
                              PED_DIARKEIA = qa.PED_DIARKEIA,
                              AED_MSC = qa.AED_MSC ?? false,
                              AED_MSC_TITLOS = qa.AED_MSC_TITLOS,
                              AED_PHD = qa.AED_PHD ?? false,
                              AED_PHD_TITLOS = qa.AED_PHD_TITLOS,
                              LANG_TEXT = qa.LANG_TEXT,
                              LANG_LEVEL = qa.LANG_LEVEL,
                              LANG_TITLOS = qa.LANG_TITLOS,
                              COMPUTER_CERT = qa.COMPUTER_CERT,
                              COMPUTER_TITLOS = qa.COMPUTER_TITLOS,
                              EPIMORFOSI1 = qa.EPIMORFOSI1 ?? false,
                              EPIMORFOSI1_HOURS = qa.EPIMORFOSI1_HOURS ?? 0,
                              EPIMORFOSI2 = qa.EPIMORFOSI2 ?? false,
                              EPIMORFOSI2_HOURS = qa.EPIMORFOSI2_HOURS ?? 0,
                              PERIFERIA_ID = qa.PERIFERIA_ID,
                              SCHOOL_ID = qa.SCHOOL_ID,
                              PROKIRIXI_ID = qa.PROKIRIXI_ID,
                              EPAGELMA_STATUS = qa.EPAGELMA_STATUS,
                              CHILDREN = qa.CHILDREN,
                              // new fields (2016)
                              CERTIFIED = qa.CERTIFIED ?? false,
                              SOCIALGROUP1 = qa.SOCIALGROUP1 ?? false,
                              SOCIALGROUP2 = qa.SOCIALGROUP2 ?? false,
                              SOCIALGROUP3 = qa.SOCIALGROUP3 ?? false,
                              SOCIALGROUP4 = qa.SOCIALGROUP4 ?? false,
                              SOCIALGROUP1_DOC = qa.SOCIALGROUP1_DOC,
                              SOCIALGROUP2_DOC = qa.SOCIALGROUP2_DOC,
                              SOCIALGROUP3_DOC = qa.SOCIALGROUP3_DOC,
                              SOCIALGROUP4_DOC = qa.SOCIALGROUP4_DOC,
                              LANG1_TEXT = qa.LANG1_TEXT,
                              LANG1_LEVEL = qa.LANG1_LEVEL,
                              LANG1_TITLOS = qa.LANG1_TITLOS,
                              LANG2_TEXT = qa.LANG2_TEXT,
                              LANG2_LEVEL = qa.LANG2_LEVEL,
                              LANG2_TITLOS = qa.LANG2_TITLOS,
                              EPIMORFOSI3 = qa.EPIMORFOSI3 ?? false,
                              EPIMORFOSI3_HOURS = qa.EPIMORFOSI3_HOURS ?? 0,
                              // calculated fields
                              AGE = qa.AGE,
                              MORIA_ANERGIA = qa.MORIA_ANERGIA ?? 0,
                              MORIA_PTYXIO = qa.MORIA_PTYXIO ?? 0,
                              MORIA_MSC = qa.MORIA_MSC ?? 0,
                              MORIA_PHD = qa.MORIA_PHD ?? 0,
                              MORIA_PED = qa.MORIA_PED ?? 0,
                              MORIA_AED_MSC = qa.MORIA_AED_MSC ?? 0,
                              MORIA_AED_PHD = qa.MORIA_AED_PHD ?? 0,
                              MORIA_LANG = qa.MORIA_LANG ?? 0,
                              MORIA_COMPUTER = qa.MORIA_COMPUTER ?? 0,
                              MORIA_EPIMORFOSI1 = qa.MORIA_EPIMORFOSI1 ?? 0,
                              MORIA_EPIMORFOSI2 = qa.MORIA_EPIMORFOSI2 ?? 0,
                              MORIA_EPIMORFOSI3 = qa.MORIA_EPIMORFOSI3 ?? 0,
                              MORIA_TEACH = qa.MORIA_TEACH ?? 0,
                              MORIA_WORK1 = qa.MORIA_WORK1 ?? 0,
                              MORIA_WORK2 = qa.MORIA_WORK2 ?? 0,
                              MORIA_WORK = qa.MORIA_WORK ?? 0,
                              MORIA_SOCIAL = qa.MORIA_SOCIAL ?? 0,
                              MORIA_TOTAL = qa.MORIA_TOTAL ?? 0,
                              TIMESTAMP = qa.TIMESTAMP
                          }).FirstOrDefault();

            return dataAitisi;
        }

        public ActionResult ViewOriginal(int aitisiID)
        {
            bool val1 = (System.Web.HttpContext.Current.User != null) && System.Web.HttpContext.Current.User.Identity.IsAuthenticated;
            if (!val1) return RedirectToAction("TaxisNetLogin", "USER_TEACHERS");
            else loggedTeacher = GetLoginTeacher();

            if (!(aitisiID > 0)) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            AitisisOriginalViewModel aitisi = AitisiOriginalViewModelFromDB(aitisiID);

            if (aitisi == null)
            {
                return HttpNotFound();
            }
            return View(aitisi);
        }
       
        public ActionResult PrintOriginal(int? AITISI_ID)
        {
            bool val1 = (System.Web.HttpContext.Current.User != null) && System.Web.HttpContext.Current.User.Identity.IsAuthenticated;
            if (!val1)
            {
                return RedirectToAction("TaxisNetLogin", "USER_TEACHERS");
            }
            else
            {
                loggedTeacher = GetLoginTeacher();
                var data = (from t in db.AITISIS_ORIGINAL
                            where t.AFM == loggedTeacher.USER_AFM && t.AITISI_ID == AITISI_ID
                            select new AitisisOriginalViewModel
                            {
                                AITISI_ID = t.AITISI_ID,
                                AITISI_PROTOCOL = t.AITISI_PROTOCOL,
                                AITISI_DATE = t.AITISI_DATE,
                                AFM = t.AFM
                            }).FirstOrDefault();
                return View(data);
            }
        }


        #endregion ΠΡΩΤΟΤΥΠΕΣ ΑΙΤΗΣΕΙΣ - ΝΕΟ (2018-06-08)

        
        #region ΜΕΤΑΦΟΡΤΩΣΗ ΕΝΣΤΑΣΕΩΝ (2020-09-18)

        public ActionResult UploadData(string notify = null)
        {
            bool val1 = (System.Web.HttpContext.Current.User != null) && System.Web.HttpContext.Current.User.Identity.IsAuthenticated;
            if (!val1)
            {
                ViewBag.loggedUser = "(χωρίς σύνδεση)";
                return RedirectToAction("TaxisNetLogin", "USER_TEACHERS");
            }
            else
            {
                loggedTeacher = GetLoginTeacher();
            }
            bool enstasi_allow = Common.GetOpenProkirixiEnstasi();
            if (enstasi_allow == false)
            {
                string Msg = "Δεν μπορείτε να υποβάλλετε ένσταση διότι η υποβολή ενστάσεων είναι απενεργοποιημένη.";
                return RedirectToAction("Index", "TEACHERS", new { notify = Msg });
            }

            if (notify != null)
            {
                this.ShowMessage(MessageType.Warning, notify);
            }
            if (!AitisisEnstasiExist())
            {
                string msg = "Δεν βρέθηκαν αιτήσεις για τις οποίες επιτρέπεται μεταφόρτωση ενστάσεων.";
                return RedirectToAction("Index", "TEACHERS", new { notify = msg });
            }
            int prokirixiId = Common.GetEnstasiProkirixiID();

            if (!Common.VerifyUploadIntegrity(prokirixiId, loggedTeacher))
            {
                notify = "Το σχολείο της αίτησης δεν συμφωνεί με το σχολείο για το οποίο έχουν ανέβει τα αρχεία. ";
                notify += "Διαγράψτε τα ανεβασμένα αρχεία και μεταφορτώστε τα πάλι, αλλιώς δεν θα μπορούν να βρεθούν.";
                this.ShowMessage(MessageType.Error, notify);
            }

            PopulateSchoolYears();
            PopulateEnstasiAitisis();
            return View();
        }

        #region MASTER GRID CRUD FUNCTIONS

        public ActionResult Upload_Read([DataSourceRequest] DataSourceRequest request)
        {
            int prokirixiId = Common.GetEnstasiProkirixiID();
            string teacherAfm = GetLoginTeacher().USER_AFM;

            IEnumerable<UploadsViewModel> data = enstaseisService.Read(prokirixiId, teacherAfm);

            return Json(data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Upload_Create([DataSourceRequest] DataSourceRequest request, UploadsViewModel data)
        {
            string teacherAfm = GetLoginTeacher().USER_AFM;
            int prokirixiId = Common.GetEnstasiProkirixiID();

            if (!(prokirixiId > 0))
                ModelState.AddModelError("", "Δεν βρέθηκε προκήρυξη ανοικτή για ενστάσεις.");

            var newdata = new UploadsViewModel();

            if (data != null && ModelState.IsValid)
            {
                enstaseisService.Create(data, prokirixiId, teacherAfm);
                newdata = enstaseisService.Refresh(data.UPLOAD_ID);
            }
            return Json(new[] { newdata }.ToDataSourceResult(request, ModelState), JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Upload_Update([DataSourceRequest] DataSourceRequest request, UploadsViewModel data)
        {
            string teacherAfm = GetLoginTeacher().USER_AFM;
            int prokirixiId = Common.GetEnstasiProkirixiID();

            var newdata = new UploadsViewModel();

            if (data != null && ModelState.IsValid)
            {
                enstaseisService.Update(data, prokirixiId, teacherAfm);
                newdata = enstaseisService.Refresh(data.UPLOAD_ID);
            }
            return Json(new[] { newdata }.ToDataSourceResult(request, ModelState), JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Upload_Destroy([DataSourceRequest] DataSourceRequest request, UploadsViewModel data)
        {
            if (data != null)
            {
                if (Kerberos.CanDeleteUpload(data.UPLOAD_ID))
                {
                    enstaseisService.Destroy(data);
                }
                else
                {
                    ModelState.AddModelError("", "Για να γίνει διαγραφή πρέπει πρώτα να διαγραφούν τα σχετικά αρχεία μεταφόρτωσης");
                }
            }
            return Json(new[] { data }.ToDataSourceResult(request, ModelState), JsonRequestBehavior.AllowGet);
        }

        // --------------------------
        // Αντικαθιστά τη συνηθισμένη action Destroy διότι όταν χρειάζεται
        // error message η ModelState.AddModelError έχει το σύμπτωμα να
        // καλεί ξανά την Destroy μετά από μια Create/Update Action!
        // Χρησιμοποείται σε συνδυασμό με την jQuery deleteRow() στον client.
        // Ημερομηνία : 26/03/2020
        // --------------------------
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Upload_Delete(int uploadId = 0)
        {
            string msg = enstaseisService.Delete(uploadId);

            return Json(msg, JsonRequestBehavior.AllowGet);
        }

        #endregion


        #region CHILD GRID (UPLOADED FILEDETAILS)

        public ActionResult UploadFiles_Read([DataSourceRequest] DataSourceRequest request, int uploadId = 0)
        {
            List<UploadsFilesViewModel> data = enstaseisService.ReadFiles(uploadId);

            return Json(data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult UploadFiles_Destroy([DataSourceRequest] DataSourceRequest request, UploadsFilesViewModel data)
        {
            if (data != null)
            {
                // First delete the physical file and then the info record. Important!
                DeleteUploadedFile(data.ID);

                enstaseisService.DestroyFile(data);
            }
            return Json(new[] { data }.ToDataSourceResult(request, ModelState), JsonRequestBehavior.AllowGet);
        }

        #endregion


        #region UPLOAD FORM WITH SAVE-REMOVE ACTIONS

        public ActionResult UploadForm(int uploadId, string notify = null)
        {
            bool val1 = (System.Web.HttpContext.Current.User != null) && System.Web.HttpContext.Current.User.Identity.IsAuthenticated;
            if (!val1)
            {
                ViewBag.loggedUser = "(χωρίς σύνδεση)";
                return RedirectToAction("TaxisNetLogin", "USER_TEACHERS");
            }
            else
            {
                loggedTeacher = GetLoginTeacher();
                if (loggedTeacher == null)
                    return RedirectToAction("Error", "TEACHERS", new { notify = "Δεν βρέθηκε εξουσιοδοτημένος χρήστης για το αίτημα." });
            }
            if (notify != null) this.ShowMessage(MessageType.Warning, notify);
            if (!(uploadId > 0))
            {
                string msg = "Άκυρος κωδικός μεταφόρτωσης. Πρέπει πρώτα να αποθηκεύσετε την εγγραφή μεταφόρτωσης.";
                return RedirectToAction("ErrorData", "TEACHERS", new { notify = msg });
            }
            ViewData["uploadId"] = uploadId;

            return View();
        }

        public ActionResult Upload(IEnumerable<HttpPostedFileBase> files, int uploadId = 0)
        {
            string uploadPath = ENSTASEIS_PATH;
            loggedTeacher = GetLoginTeacher();

            // returns tuple with Item1=school_id, Item2=prokirixi_id, Item3=aitisi_id
            var upload_info = Common.GetUploadInfo(uploadId);
            int schoolyearId = (int)db.PROKIRIXIS.Find(upload_info.Item2).SCHOOL_YEAR;

            string folder = Common.GetUserSchoolFromSchoolId(upload_info.Item1);
            string subfolder = Common.GetSchoolYearText(schoolyearId);

            if (!string.IsNullOrEmpty(folder) && !string.IsNullOrEmpty(subfolder))
                uploadPath += folder + "/" + subfolder + "/";

            try
            {
                bool exists = Directory.Exists(Server.MapPath(uploadPath));
                if (!exists)
                    Directory.CreateDirectory(Server.MapPath(uploadPath));

                if (files != null)
                {
                    foreach (var file in files)
                    {
                        // Some browsers send file names with full path.
                        // We are only interested in the file name.
                        if (file.ContentLength > 0)
                        {
                            var fileName = Path.GetFileName(file.FileName);
                            var fileExtension = Path.GetExtension(fileName);
                            if (!ValidFileExtension(fileExtension))
                            {
                                string msg = "Εκτελέσιμα αρχεία δεν επιτρέπονται για μεταφόρτωση. Δοκιμάστε πάλι.";
                                return Content(msg);
                            }
                            UploadEnstaseisFiles fileDetail = new UploadEnstaseisFiles()
                            {
                                FILENAME = fileName.Length > 255 ? fileName.Substring(0, 255) : fileName,
                                EXTENSION = fileExtension,
                                SCHOOL_USER = folder,
                                SCHOOLYEAR_TEXT = subfolder,
                                UPLOAD_ID = uploadId,
                                ID = loggedTeacher.USER_AFM + "_" + Guid.NewGuid()
                            };
                            db.UploadEnstaseisFiles.Add(fileDetail);
                            db.SaveChanges();

                            var physicalPath = Path.Combine(Server.MapPath(uploadPath), fileDetail.ID + fileDetail.EXTENSION);
                            file.SaveAs(physicalPath);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string msg = "Παρουσιάστηκε σφάλμα στη μεταφόρτωση:<br/>" + ex.Message;
                return Content(msg);
            }
            // Return an empty string to signify success
            return Content("");
        }

        public bool ValidFileExtension(string extension)
        {
            string[] extensions = { ".EXE", ".COM", "BAT", ".MSI", ".BIN", ".CMD", ".JSE", ".REG", ".VBS", ".VBE", ".WS", ".WSF" };

            List<string> forbidden_extensions = new List<string>(extensions);

            if (forbidden_extensions.Contains(extension.ToUpper()))
                return false;
            return true;
        }

        public ActionResult Remove(string[] fileNames, int uploadId)
        {
            string uploadPath =ENSTASEIS_PATH;

            // returns tuple with Item1=school_id, Item2=prokirixi_id, Item3=aitisi_id
            var upload_info = Common.GetUploadInfo(uploadId);
            int schoolyearId = (int)db.PROKIRIXIS.Find(upload_info.Item2).SCHOOL_YEAR;

            // The parameter of the Remove action must be called "fileNames"
            string folder = Common.GetUserSchoolFromSchoolId(upload_info.Item1);
            string subfolder = Common.GetSchoolYearText(schoolyearId);

            if (!string.IsNullOrEmpty(folder) && !string.IsNullOrEmpty(subfolder))
                uploadPath += folder + "/" + subfolder + "/";

            if (fileNames != null)
            {
                foreach (var fullName in fileNames)
                {
                    var fileName = Path.GetFileName(fullName);
                    var extension = Path.GetExtension(fileName);

                    string file_guid = Common.GetFileGuidFromName(fileName, uploadId);

                    string fileToDelete = file_guid + extension;
                    var physicalPath = Path.Combine(Server.MapPath(uploadPath), fileToDelete);

                    if (System.IO.File.Exists(physicalPath))
                    {
                        System.IO.File.Delete(physicalPath);
                        DeleteUploadFileRecord(file_guid);
                    }
                }
            }

            // Return an empty string to signify success
            return Content("");
        }

        public FileResult Download(string file_id)
        {
            string p = "";
            string f = "";
            string the_path = ENSTASEIS_PATH;

            var fileinfo = (from d in db.UploadEnstaseisFiles where d.ID == file_id select d).FirstOrDefault();
            if (fileinfo != null)
            {
                the_path += fileinfo.SCHOOL_USER + "/" + fileinfo.SCHOOLYEAR_TEXT + "/";
                p = fileinfo.ID.ToString() + fileinfo.EXTENSION;
                f = fileinfo.FILENAME;
            }

            return File(Path.Combine(Server.MapPath(the_path), p), System.Net.Mime.MediaTypeNames.Application.Octet, f);
        }

        public ActionResult DeleteUploadFileRecord(string file_guid)
        {
            UploadEnstaseisFiles entity = db.UploadEnstaseisFiles.Find(file_guid);
            if (entity != null)
            {
                db.Entry(entity).State = EntityState.Deleted;
                db.UploadEnstaseisFiles.Remove(entity);
                db.SaveChanges();
            }
            return Content("");
        }

        public ActionResult DeleteUploadedFile(string file_guid)
        {
            string folder = "";
            string uploadPath = ENSTASEIS_PATH;
            string subfolder = "";
            string extension = "";

            var data = (from d in db.UploadEnstaseisFiles where d.ID == file_guid select d).FirstOrDefault();
            if (data != null)
            {
                folder = data.SCHOOL_USER;
                subfolder = data.SCHOOLYEAR_TEXT;
                extension = data.EXTENSION;

                if (!string.IsNullOrEmpty(folder) && !string.IsNullOrEmpty(subfolder))
                    uploadPath += folder + "/" + subfolder + "/";

                string fileToDelete = file_guid + extension;
                var physicalPath = Path.Combine(Server.MapPath(uploadPath), fileToDelete);
                if (System.IO.File.Exists(physicalPath))
                {
                    System.IO.File.Delete(physicalPath);
                }
            }
            return Content("");
        }

        #endregion


        #endregion


        #region GETTERS (COMBO BOXES)

        public JsonResult GetPtyxiaTypes()
        {
            var data = db.SYS_PTYXIA_TYPES.ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAnergies(string text)
        {
            var anergies = db.SYS_ANERGIA.AsQueryable();

            if (!string.IsNullOrEmpty(text))
            {
                anergies = anergies.Where(p => p.ANERGIA_TAG.Contains(text));
            }

            return Json(anergies, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSocialGroups(string text)
        {
            var anergies = db.SYS_SOCIALGROUPS.AsQueryable();

            if (!string.IsNullOrEmpty(text))
            {
                anergies = anergies.Where(p => p.CATEGORY.Contains(text));
            }

            return Json(anergies, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetKlados(string text)
        {
            var kladoi = db.SYS_KLADOS.Select(p => new SYS_KLADOSViewModel
            {
                KLADOS_NAME = p.KLADOS_NAME,
                KLADOS_ID = p.KLADOS_ID
            });

            if (!string.IsNullOrEmpty(text))
            {
                kladoi = kladoi.Where(p => p.KLADOS_NAME.Contains(text));
            }
            return Json(kladoi, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCascadeEidikotites(int? klados, string eidikotitaFilter)
        {
            var eidikotites = db.VD_EIDIKOTITES.AsQueryable();

            var prokirixiEidikotites = (from pe in db.PROKIRIXIS_EIDIKOTITES select pe.EIDIKOTITA_ID).ToList();
            eidikotites = eidikotites.Where(e => prokirixiEidikotites.Contains(e.EIDIKOTITA_ID));
            if (klados != null)
            {
                eidikotites = eidikotites.Where(m => m.EIDIKOTITA_KLADOS_ID == klados);
            }

            if (!string.IsNullOrEmpty(eidikotitaFilter))
            {
                int possibleInt;
                if (int.TryParse(eidikotitaFilter, out possibleInt))
                {
                    eidikotites = eidikotites.Where(m => m.EIDIKOTITA_ID.Equals(possibleInt));
                }
                else
                {
                    eidikotites = eidikotites.Where(m => m.EIDIKOTITA_DESC.Contains(eidikotitaFilter));
                }
            }
            var result = eidikotites.Select(m => new VD_EIDIKOTITESViewModel
            {
                EIDIKOTITA_ID = m.EIDIKOTITA_ID,
                EIDIKOTITA_CODE = m.EIDIKOTITA_CODE,
                EIDIKOTITA_NAME = m.EIDIKOTITA_NAME,
                EIDIKOTITA_DESC = m.EIDIKOTITA_DESC
            }).OrderBy(m => m.EIDIKOTITA_DESC);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetBasicEducation(string text)
        {
            var basic = db.SYS_BASICEDUCATION.Select(p => new SYS_BASICEDUCATIONViewModel
            {
                BASIC_TEXT = p.BASIC_TEXT,
                BASIC_ID = p.BASIC_ID
            });

            if (!string.IsNullOrEmpty(text))
            {
                basic = basic.Where(p => p.BASIC_TEXT.Contains(text));
            }
            return Json(basic, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetMscPeriods(string text)
        {
            var periods = db.SYS_MSCPERIODS.Select(p => new SYS_MSCPERIODSViewModel
            {
                MSCPERIOD_ID = p.MSCPERIOD_ID,
                MSCPERIOD_TEXT = p.MSCPERIOD_TEXT
            });

            if (!string.IsNullOrEmpty(text))
            {
                periods = periods.Where(p => p.MSCPERIOD_TEXT.Contains(text));
            }
            return Json(periods, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPedPeriods(string text)
        {
            var periods = db.SYS_PEDAGOGICPERIOD.Select(p => new SYS_PEDAGOGICPERIODViewModel
            {
                PERIOD_ID = p.PERIOD_ID,
                PERIOD_TEXT = p.PERIOD_TEXT
            });

            if (!string.IsNullOrEmpty(text))
            {
                periods = periods.Where(p => p.PERIOD_TEXT.Contains(text));
            }
            return Json(periods, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetLanguages(string text)
        {
            var lang = (from l in db.SYS_LANGUAGE
                        orderby l.LANGUAGE_TEXT
                        select new { l.LANGUAGE_TEXT }).ToList();

            if (!string.IsNullOrEmpty(text))
            {
                lang = (from a in db.SYS_LANGUAGE where a.LANGUAGE_TEXT.Contains(text) orderby a.LANGUAGE_TEXT select new { a.LANGUAGE_TEXT }).ToList();
            }
            return Json(lang, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetLangLevels(string text)
        {
            var periods = db.SYS_LANGUAGELEVEL.Select(p => new SYS_LANGUAGELEVELViewModel
            {
                LEVEL = p.LEVEL
            });

            if (!string.IsNullOrEmpty(text))
            {
                periods = periods.Where(p => p.LEVEL.Contains(text));
            }
            return Json(periods, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetComputerCertificates(string text)
        {
            var periods = db.SYS_COMPUTERASEP.Select(p => new SYS_COMPUTERASEPViewModel
            {
                CERTIFICATE_ID = p.CERTIFICATE_ID,
                CERTIFICATE = p.CERTIFICATE
            });

            if (!string.IsNullOrEmpty(text))
            {
                periods = periods.Where(p => p.CERTIFICATE.Contains(text));
            }
            return Json(periods, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPeriferies(string text, int? filterEidikotita)
        {
            var prokirixiSchools = (from d in db.PROKIRIXIS_EIDIKOTITES select d.SCHOOL_ID).ToList();
            if (filterEidikotita.HasValue)
            {
                prokirixiSchools = (from d in db.PROKIRIXIS_EIDIKOTITES where (d.EIDIKOTITA_ID == filterEidikotita) select d.SCHOOL_ID).ToList();
            }
            var prokirixiSchoolsPeriferies = (from s in db.SYS_SCHOOLS where prokirixiSchools.Contains(s.SCHOOL_ID) select s.SCHOOL_PERIFERIA_ID).ToList();
            var periferiesWithSchools = (from p in db.SYS_PERIFERIES
                                         where prokirixiSchoolsPeriferies.Contains(p.PERIFERIA_ID)
                                         select new SYS_PERIFERIESViewModel
                                         {
                                             PERIFERIA_ID = p.PERIFERIA_ID,
                                             PERIFERIA_NAME = p.PERIFERIA_NAME
                                         });

            if (!string.IsNullOrEmpty(text))
            {
                int possibleInt;
                if (int.TryParse(text, out possibleInt))
                {
                    periferiesWithSchools = periferiesWithSchools.Where(p => p.PERIFERIA_ID == possibleInt);
                }
                else
                {
                    periferiesWithSchools = periferiesWithSchools.Where(p => p.PERIFERIA_NAME.Contains(text));
                }
            }
            return Json(periferiesWithSchools, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetEpagelma(string text)
        {
            var epagelma = db.SYS_EPAGELMA.AsQueryable();

            if (!string.IsNullOrEmpty(text))
            {
                epagelma = epagelma.Where(p => p.EPAGELMA_TEXT.Contains(text));
            }
            return Json(epagelma, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCascadeSchools(int? periferia, int? eidikotita, string schoolFilter)
        {
            //Εύρεση ανοικτής προκύρηξης
            var openProkirixi = (from d in db.PROKIRIXIS where d.STATUS == 1 select d.ID).FirstOrDefault();
            //Εύρεση προκυρησόμενων σχολείων με την ειδικότητα της αίτησης
            var prokirixiSchools = (from d in db.PROKIRIXIS_EIDIKOTITES where d.PROKIRIXI_ID == openProkirixi && d.EIDIKOTITA_ID == eidikotita select d.SCHOOL_ID).ToList();

            var schools = db.sqlPERIFERIES_SCHOOLS.AsQueryable()
                .Where(s => s.SCHOOL_PERIFERIA_ID == periferia && prokirixiSchools
                .Contains(s.SCHOOL_ID))
                .GroupBy(p => p.SCHOOL_ID)
                .Select(grp => grp.FirstOrDefault());

            if (periferia != null)
            {
                schools = schools.Where(p => p.SCHOOL_PERIFERIA_ID == periferia).GroupBy(p => p.SCHOOL_ID).Select(grp => grp.FirstOrDefault());
            }
            else
                if (!string.IsNullOrEmpty(schoolFilter))
                {
                    int possibleInt;
                    if (int.TryParse(schoolFilter, out possibleInt))
                    {
                        schools = schools.Where(p => p.SCHOOL_ID.Equals(possibleInt)).GroupBy(p => p.SCHOOL_ID).Select(grp => grp.FirstOrDefault());
                    }
                    else
                    {
                        schools = schools.Where(p => p.SCHOOL_NAME.Contains(schoolFilter)).GroupBy(p => p.SCHOOL_ID).Select(grp => grp.FirstOrDefault());
                    }
                    schools = schools.Where(p => p.SCHOOL_NAME.Contains(schoolFilter)).GroupBy(p => p.SCHOOL_ID).Select(grp => grp.FirstOrDefault());
                }

            return Json(schools.Select(p => new { SCHOOL_ID = p.SCHOOL_ID, SCHOOL_NAME = p.SCHOOL_NAME }), JsonRequestBehavior.AllowGet);
        }

        #endregion


        #region POPULATORS

        public void PopulateEidikotites()
        {
            var data = (from e in db.SYS_EIDIKOTITES
                        select new SYS_EIDIKOTITESViewModel
                        {
                            EIDIKOTITA_ID = e.EIDIKOTITA_ID,
                            EIDIKOTITA_CODE = e.EIDIKOTITA_CODE,
                            EIDIKOTITA_NAME = e.EIDIKOTITA_NAME
                        }).ToList();
            ViewData["AitisiEidikotites"] = data;

        }

        public void PopulateSchoolYears()
        {
            var syears = (from s in db.SYS_SCHOOLYEARS
                          orderby s.SY_TEXT descending
                          select s).ToList();

            ViewData["school_years"] = syears;
            ViewData["defaultSchoolYear"] = syears.First().SY_ID;
        }

        public void PopulateSchoolTypes()
        {
            var schooltypes = (from t in db.SYS_SCHOOLTYPES where t.SCHOOL_TYPE_ID > 1 select t).ToList();
            ViewData["schooltypes"] = schooltypes;
            ViewData["defaultSchoolType"] = schooltypes.First().SCHOOL_TYPE_ID;
        }

        public void PopulateSchools()
        {
            var filteredSchools = (from school in db.SYS_SCHOOLS
                                   where school.SCHOOL_TYPEID > 1
                                   orderby school.SCHOOL_NAME
                                   select new SYS_SCHOOLSViewModel
                                   {
                                       SCHOOL_ID = school.SCHOOL_ID,
                                       SCHOOL_NAME = school.SCHOOL_NAME,
                                       SCHOOL_PERIFERIA_ID = school.SCHOOL_PERIFERIA_ID
                                   }).ToList();
            ViewData["schools"] = filteredSchools;
            ViewData["defaultSchool"] = filteredSchools.First().SCHOOL_ID;
        }

        public void PopulatePeriferiesByUser()
        {
            bool val1 = (System.Web.HttpContext.Current.User != null) && System.Web.HttpContext.Current.User.Identity.IsAuthenticated;
            if (!val1)
            {
                ViewData["AitisiPeriferies"] = new List<SYS_PERIFERIES>();
            }
            else
            {
                TEACHERS teacher = db.TEACHERS.Where(m => m.AFM == System.Web.HttpContext.Current.User.Identity.Name).FirstOrDefault();
                var teacherAitisisPeriferiesIds = (from aitisis in db.AITISIS where aitisis.AFM == teacher.AFM select aitisis.PERIFERIA_ID);
                var peririferiesInTeacherAitisis = (from periferia in db.SYS_PERIFERIES
                                                    where teacherAitisisPeriferiesIds.Contains(periferia.PERIFERIA_ID)
                                                    select periferia).ToList();
                ViewData["AitisiPeriferies"] = peririferiesInTeacherAitisis;
            }
        }

        public void PopulateSchoolTypes(int aitisiID = 0)
        {
            //Εύρεση ειδικότητας της αίτησης
            var eidikotitaAitisis = (from d in db.AITISIS where d.AITISI_ID == aitisiID select d.EIDIKOTITA).FirstOrDefault();
            //Εύρεση ενεργής προκύρηξης
            var openProkirixi = (from d in db.PROKIRIXIS where d.STATUS == 1 select d.ID).FirstOrDefault();
            //Εύρεση προκυρησόμενων σχολείων με την ειδικότητα της αίτησης
            var prokirixiSchools = (from d in db.PROKIRIXIS_EIDIKOTITES where d.PROKIRIXI_ID == openProkirixi && d.EIDIKOTITA_ID == eidikotitaAitisis select d.SCHOOL_ID).ToList();
            //Εύρεση περιφέριας της αίτησης
            var periferiaAitisis = (from d in db.AITISIS where d.AITISI_ID == aitisiID select d.PERIFERIA_ID).FirstOrDefault();
            
            // aitisiID = 0, return all school types
            var schooltypes = (from t in db.SYS_SCHOOLTYPES where t.SCHOOL_TYPE_ID > 1 select t).ToList();
            
            var filteredSchoolsTypes = (from d in db.SYS_SCHOOLS
                                        where prokirixiSchools.Contains(d.SCHOOL_ID) && periferiaAitisis == d.SCHOOL_PERIFERIA_ID
                                        select d.SCHOOL_TYPEID).ToList();

            if (filteredSchoolsTypes.Count > 0)
            {
                schooltypes = (from t in db.SYS_SCHOOLTYPES where filteredSchoolsTypes.Contains(t.SCHOOL_TYPE_ID) select t).ToList();
            }
            ViewData["schooltypes"] = schooltypes;
            ViewData["defaultSchoolType"] = schooltypes.First().SCHOOL_TYPE_ID;
        }

        public void PopulateSchools(int aitisiID = 0)
        {
            //Εύρεση ειδικότητας της αίτησης
            var eidikotitaAitisis = (from d in db.AITISIS where d.AITISI_ID == aitisiID select d.EIDIKOTITA).FirstOrDefault();
            //Εύρεση ενεργής προκύρηξης
            var openProkirixi = (from d in db.PROKIRIXIS where d.STATUS == 1 select d.ID).FirstOrDefault();
            //Εύρεση προκυρησόμενων σχολείων με την ειδικότητα της αίτησης
            var prokirixiSchools = (from d in db.PROKIRIXIS_EIDIKOTITES where d.PROKIRIXI_ID == openProkirixi && d.EIDIKOTITA_ID == eidikotitaAitisis select d.SCHOOL_ID).ToList();
            //Εύρεση περιφέριας της αίτησης
            var periferiaAitisis = (from d in db.AITISIS where d.AITISI_ID == aitisiID select d.PERIFERIA_ID);
            // aitisiID = 0, return all schools
            var schools = (from s in db.SYS_SCHOOLS where s.SCHOOL_TYPEID > 1 select s).ToList();
            //Εύρεση σχολείων στην περιφέρεια τα οποία έχει προκυρηχθεί η ειδικότητα της αίτησης
            var filteredSchools = (from d in db.SYS_SCHOOLS
                                   where periferiaAitisis.Contains(d.SCHOOL_PERIFERIA_ID) && prokirixiSchools.Contains(d.SCHOOL_ID)
                                   orderby d.SCHOOL_NAME
                                   select new SYS_SCHOOLSViewModel
                                   {
                                       SCHOOL_ID = d.SCHOOL_ID,
                                       SCHOOL_NAME = d.SCHOOL_NAME,
                                       SCHOOL_PERIFERIA_ID = d.SCHOOL_PERIFERIA_ID
                                   }).ToList();

            if (filteredSchools.Count > 0)
            {
                ViewData["schools"] = filteredSchools;
                ViewData["defaultSchool"] = filteredSchools.First().SCHOOL_ID;
            }
            else
            {
                ViewData["schools"] = schools;
                ViewData["defaultSchool"] = schools.First().SCHOOL_ID;
            }
        }

        public void PopulateAitisis()
        {
            int prokirixiId = Common.GetOpenProkirixiID();
            loggedTeacher = GetLoginTeacher();

            string teacherAfm = db.TEACHERS.Find(loggedTeacher.USER_AFM).AFM;
            var aitiseis = (from d in db.AITISIS where d.PROKIRIXI_ID == prokirixiId && d.AFM == teacherAfm select d).ToList();

            ViewData["aitiseis"] = aitiseis;
            ViewData["defaultAitisi"] = aitiseis.First().AITISI_ID;
        }

        public void PopulateEnstasiAitisis()
        {
            int prokirixiId = Common.GetEnstasiProkirixiID();
            loggedTeacher = GetLoginTeacher();

            string teacherAfm = db.TEACHERS.Find(loggedTeacher.USER_AFM).AFM;
            var aitiseis = (from d in db.AITISIS where d.PROKIRIXI_ID == prokirixiId && d.AFM == teacherAfm select d).ToList();

            ViewData["aitiseis"] = aitiseis;
            ViewData["defaultAitisi"] = aitiseis.First().AITISI_ID;
        }

        public void PopulateViewBagWithAitisi(AitisisViewModel selectedAitisi)
        {
            ViewBag.SelectedAitisiData = selectedAitisi;

            ViewBag.SelectedAitisiPeriferia = (from p in db.SYS_PERIFERIES
                                               where p.PERIFERIA_ID == selectedAitisi.PERIFERIA_ID
                                               select new SYS_PERIFERIESViewModel
                                               {
                                                   PERIFERIA_ID = p.PERIFERIA_ID,
                                                   PERIFERIA_NAME = p.PERIFERIA_NAME,
                                                   SYS_DIMOS = p.SYS_DIMOS
                                               }).FirstOrDefault();

            ViewBag.SelectedAitisiEidikotita = (from e in db.SYS_EIDIKOTITES
                                                where e.EIDIKOTITA_ID == (selectedAitisi.EIDIKOTITA ?? 0)
                                                select new SYS_EIDIKOTITESViewModel
                                                {
                                                    EIDIKOTITA_ID = e.EIDIKOTITA_ID,
                                                    EIDIKOTITA_CODE = e.EIDIKOTITA_CODE,
                                                    EIDIKOTITA_NAME = e.EIDIKOTITA_NAME
                                                }).FirstOrDefault();
        }

        #endregion


        #region LOCAL FUNCTIONS

        public ActionResult FilteredSchoolsRead(int? aitisiID)
        {
            //Εύρεση ειδικότητας της αίτησης
            var eidikotitaAitisis = (from d in db.AITISIS where d.AITISI_ID == aitisiID select d.EIDIKOTITA).FirstOrDefault();
            //Εύρεση ενεργής προκύρηξης
            var openProkirixi = (from d in db.PROKIRIXIS where d.STATUS == 1 select d.ID).FirstOrDefault();
            //Εύρεση προκυρησόμενων σχολείων με την ειδικότητα της αίτησης
            var prokirixiSchools = (from d in db.PROKIRIXIS_EIDIKOTITES where d.PROKIRIXI_ID == openProkirixi && d.EIDIKOTITA_ID == eidikotitaAitisis select d.SCHOOL_ID).ToList();
            //Εύρεση περιφέριας της αίτησης
            var periferiaAitisis = (from d in db.AITISIS where d.AITISI_ID == aitisiID select d.PERIFERIA_ID);
            //Εύρεση σχολείων στην περιφέρεια τα οποία έχει προκυρηχθεί η ειδικότητα της αίτησης
            var filteredSchools = (from d in db.SYS_SCHOOLS
                                   where periferiaAitisis.Contains(d.SCHOOL_PERIFERIA_ID) && prokirixiSchools.Contains(d.SCHOOL_ID)
                                   select new SYS_SCHOOLSViewModel
                                   {
                                       SCHOOL_ID = d.SCHOOL_ID,
                                       SCHOOL_NAME = d.SCHOOL_NAME,
                                       SCHOOL_PERIFERIA_ID = d.SCHOOL_PERIFERIA_ID
                                   });
            return Json(filteredSchools, JsonRequestBehavior.AllowGet);
        }

        public bool AitisisExist()
        {
            loggedTeacher = GetLoginTeacher();
            int prokirixiId = Common.GetOpenProkirixiID(true);

            string teacherAfm =db.TEACHERS.Find(loggedTeacher.USER_AFM).AFM;

            if (teacherAfm != null)
            {
                var aitiseis = (from d in db.AITISIS where d.PROKIRIXI_ID == prokirixiId && d.AFM == teacherAfm select d).ToList();
                if (aitiseis.Count > 0)
                    return true;
            }
            return false;
        }

        public bool AitisisEnstasiExist()
        {
            loggedTeacher = GetLoginTeacher();
            int prokirixiId = Common.GetEnstasiProkirixiID();

            string teacherAfm = db.TEACHERS.Find(loggedTeacher.USER_AFM).AFM;

            if (teacherAfm != null)
            {
                var aitiseis = (from d in db.AITISIS where d.PROKIRIXI_ID == prokirixiId && d.AFM == teacherAfm select d).ToList();
                if (aitiseis.Count > 0)
                    return true;
            }
            return false;
        }

        public TEACHERS GetLoginTeacher2()
        {
            USER_TEACHERS loggedUser = db.USER_TEACHERS.Where(m => m.USER_AFM == System.Web.HttpContext.Current.User.Identity.Name).FirstOrDefault();
            ViewBag.loggedUser = loggedUser.USERNAME;

            TEACHERS loggedTeacher = db.TEACHERS.Find(loggedUser.USER_AFM);
            ViewBag.loggedTeacher = loggedTeacher;

            if (loggedTeacher != null)
            {
                if (!string.IsNullOrEmpty(loggedTeacher.FIRSTNAME) && !string.IsNullOrEmpty(loggedTeacher.LASTNAME))
                {
                    ViewBag.loggedUser = loggedTeacher.FIRSTNAME + " " + loggedTeacher.LASTNAME;
                }
            }
            return loggedTeacher;
        }

        public USER_TEACHERS GetLoginTeacher()
        {
            loggedTeacher = db.USER_TEACHERS.Where(m => m.USER_AFM == System.Web.HttpContext.Current.User.Identity.Name).FirstOrDefault();
            ViewBag.loggedUser = loggedTeacher.USERNAME;

            loggedTeacherData = db.TEACHERS.Find(loggedTeacher.USER_AFM);

            ViewBag.loggedTeacher = loggedTeacherData;
            if (loggedTeacherData != null)
            {
                if (!string.IsNullOrEmpty(loggedTeacherData.FIRSTNAME) && !string.IsNullOrEmpty(loggedTeacherData.LASTNAME))
                {
                    ViewBag.loggedUser = loggedTeacherData.FIRSTNAME + " " + loggedTeacherData.LASTNAME;
                }
            }
            return loggedTeacher;
        }

        public ActionResult Error(string notify = null)
        {
            if (notify != null) this.ShowMessage(MessageType.Warning, notify);

            return View();
        }

        #endregion
    }
}
