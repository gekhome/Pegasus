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
using Pegasus.Filters;
using Pegasus.Models;
using Pegasus.DAL;
using Pegasus.BPM;
using Pegasus.Notification;
using Pegasus.Extensions;
using Pegasus.Services;

namespace Pegasus.Controllers.DataControllers
{
    [ErrorHandlerFilter]
    public class TEACHERSController : Controller
    {
        private readonly PegasusDBEntities db;
        private USER_TEACHERS loggedTeacher;
        private TEACHERS loggedTeacherData;

        private const int PROKIRIXI_2015_2016 = 2;
        private const int PROKIRIXI_2017_2018 = 1004;

        private readonly ITeacherService teacherService;

        public TEACHERSController(PegasusDBEntities entities, ITeacherService teacherService)
        {
            db =entities;

            this.teacherService = teacherService;
        }


        public ActionResult Index(string notify = null)
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

            string Msg = "Δεν βρέθηκε ανοικτή προκήρυξη. Όλες οι ενέργειες δημιουργίας και ";
            Msg += "επεξεργασίας δεδομένων είναι απενεργοποιημένες.";

            int prokirixiId = Common.GetOpenProkirixiID();
            if (prokirixiId == 0)
            {
                this.ShowMessage(MessageType.Warning, Msg);
            }
            if (notify != null)
            {
                this.ShowMessage(MessageType.Warning, notify);
            }
            return View();
        }


        #region ΕΠΕΞΕΡΓΑΣΙΑ - ΕΚΤΥΠΩΣΗ ΣΤΟΙΧΕΙΩΝ ΕΚΠΑΙΔΕΥΤΗ

        public ActionResult TeacherCreate()
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
                return RedirectToAction("Index", "TEACHERS");
            }

            if (db.TEACHERS.Find(loggedTeacher.USER_AFM) == null)
            {
                return View(new TeacherViewModel() { AFM = loggedTeacher.USER_AFM });
            }
            else
            {
                return RedirectToAction("TeacherEdit");
            }
        }

        [HttpPost]
        public ActionResult TeacherCreate(TeacherViewModel model)
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
                return RedirectToAction("Index", "TEACHERS");
            }

            if (!AitisiRules.ValidateBirthdate(model))
            {
                ModelState.AddModelError("BIRTHDATE", "Η ημερομηνία γέννησης είναι εκτός λογικών ορίων.");
            }

            if (ModelState.IsValid)
            {
                teacherService.Create(loggedTeacher.USER_AFM, model);

                this.AddNotification("Η αποθήκευση ολοκληρώθηκε με επιτυχία.", NotificationType.SUCCESS);
                return RedirectToAction("TeacherEdit", "TEACHERS");
            }
            this.AddNotification("Η αποθήκευση απέτυχε λόγω σφαλμάτων επικύρωσης. Δείτε τη σύνοψη στο κάτω μέρος.", NotificationType.ERROR);
            model.AFM = loggedTeacher.USER_AFM;
            return View(model);
        }

        public ActionResult TeacherEdit()
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
                return RedirectToAction("Index", "TEACHERS");
            }

            TEACHERS teacher = db.TEACHERS.Find(loggedTeacher.USER_AFM);
            if (teacher == null)
            {
                return RedirectToAction("TeacherCreate", "TEACHERS");
            }
            TeacherViewModel teacherForEdit = new TeacherViewModel(teacher);
            return View(teacherForEdit);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TeacherEdit(TeacherViewModel model)
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
                return RedirectToAction("Index", "TEACHERS");
            }

            if (!AitisiRules.ValidateBirthdate(model))
            {
                ModelState.AddModelError("BIRTHDATE", "Η ημερομηνία γέννησης είναι εκτός λογικών ορίων.");
            }

            if (ModelState.IsValid)
            {
                teacherService.Update(loggedTeacher.USER_AFM, model);

                this.AddNotification("Η αποθήκευση ολοκληρώθηκε με επιτυχία.", NotificationType.SUCCESS);
                TeacherViewModel newdata = teacherService.GetModel(loggedTeacher.USER_AFM);
                return View(newdata);
            }
            this.AddNotification("Η αποθήκευση απέτυχε λόγω σφαλμάτων επικύρωσης. Δείτε τη σύνοψη στο κάτω μέρος.", NotificationType.ERROR);
            model.AFM = loggedTeacher.USER_AFM;
            return View(model);
        }

        public ActionResult TeacherPrint(string AFM)
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
            var data = (from t in db.TEACHERS
                        where t.AFM == AFM
                        select new TeacherViewModel
                        {
                            AFM = t.AFM,
                            FIRSTNAME = t.FIRSTNAME,
                            LASTNAME = t.LASTNAME
                        }).FirstOrDefault();
            return View(data);
        }

        #endregion


        #region ΛΕΙΤΟΥΡΓΙΕΣ ΠΡΟΒΟΛΗΣ ΑΙΤΗΣΕΩΝ, ΠΡΟΫΠΗΡΕΣΙΩΝ ΓΙΑ ΕΝΗΜΕΡΩΣΗ ΥΠΟΨΗΦΙΟΥ

        #region ΑΙΤΗΣΕΙΣ ΧΡΗΣΤΗ

        public ActionResult ListAitiseis()
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

            bool ViewAllowed = Common.GetOpenProkirixiUserView();

            if (ViewAllowed == false)
            {
                this.ShowMessage(MessageType.Warning, "Η προβολή αποτελεσμάτων αξιολόγησης είναι προσωρινά κλειδωμένη.");
            }
            // εάν η προβολή δεν επιτρέπεται, η GetAitiseisList() επιστρέφει κενή λίστα.
            List<sqlTEACHER_AITISEIS> aitiseis = teacherService.ReadAitiseis(loggedTeacher.USER_AFM);
            return View(aitiseis);
        }

        public ActionResult Aitiseis_Read([DataSourceRequest] DataSourceRequest request)
        {
            string AFM = GetLoginTeacher().USER_AFM;
            List<sqlTEACHER_AITISEIS> data = teacherService.ReadAitiseis(AFM);

            return Json(data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        public ActionResult ViewAitisi(int? AITISI_ID)
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

            bool ViewAllowed = Common.GetOpenProkirixiUserView();
            if (ViewAllowed == false)
            {
                this.ShowMessage(MessageType.Warning, "Η προβολή Αιτήσεων, Προϋπηρεσιών είναι κλειδωμένη.");
                return RedirectToAction("Index", "TEACHERS");
            }
            if (AITISI_ID == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            int aitisiID = (int)AITISI_ID;
            AitisisViewModel aitisi = teacherService.GetAitisiModel(aitisiID);

            if (aitisi == null)
            {
                return HttpNotFound();
            }
            return View(aitisi);
        }
  
        public ActionResult PrintAitisi(int? AITISI_ID)
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
                        where t.AITISI_ID == AITISI_ID
                        select new AitisisViewModel
                        {
                            PROKIRIXI_ID = t.PROKIRIXI_ID,
                            AITISI_ID = t.AITISI_ID,
                            AITISI_PROTOCOL = t.AITISI_PROTOCOL,
                            AITISI_DATE = t.AITISI_DATE,
                            AFM = t.AFM
                        }).FirstOrDefault();
            return View(data);
            
        }

        #endregion


        #region ΑΠΟΤΕΛΕΣΜΑΤΑ

        public ActionResult ExperienceResultsView(int? AITISI_ID)
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

            if (AITISI_ID == 0 || AITISI_ID == null)
            {
                // new course of actions: AITISI_ID αναφέρεται στους πίνακες προϋπηρεσιών
                // οπότε είναι 0 όταν δεν υπάρχουν προϋπηρεσίες.
                string msg = "Δεν βρέθηκαν τρέχουσες προϋπηρεσίες. Τα μόρια είναι 0.";
                return RedirectToAction("ListAitiseis", "TEACHERS", new { notify = msg });
            }
            // At this point we can procceed (AITISI_ID != null)
            AITISIS aitisi = (from t in db.AITISIS
                              where t.AITISI_ID == AITISI_ID
                              select t).FirstOrDefault();
            int aitisiId = (int)AITISI_ID;
            int prokirixiID = (int)aitisi.PROKIRIXI_ID;

            if (prokirixiID > PROKIRIXI_2017_2018)
            {
                ExperienceResultsViewModel moriaModel = teacherService.GetMoriaNew(aitisiId);
                return View(moriaModel);
            }
            else if (prokirixiID > PROKIRIXI_2015_2016 && prokirixiID <= PROKIRIXI_2017_2018)
            {
                ExperienceResultsViewModel moriaModel = teacherService.GetMoriaOld2(aitisiId);
                return View(moriaModel);
            }
            else // 2015-2016
            {
                ExperienceResultsViewModel moriaModel = teacherService.GetMoriaOld(aitisiId);
                return View(moriaModel);
            }
        }
     
        public ActionResult ExperienceResultsPrint(int? AITISI_ID)
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
                        where t.AITISI_ID == AITISI_ID
                        select new AitisisViewModel
                        {
                            PROKIRIXI_ID = t.PROKIRIXI_ID,
                            AITISI_ID = t.AITISI_ID,
                            AITISI_PROTOCOL = t.AITISI_PROTOCOL,
                            AITISI_DATE = t.AITISI_DATE,
                            AFM = t.AFM
                        }).FirstOrDefault();
            
            return View(data);
        }

        #endregion


        #endregion


        #region ΠΡΟΒΟΛΗ ΣΧΟΛΕΙΩΝ ΑΙΤΗΣΗΣ

        public ActionResult FilteredSchoolsRead(string text, int? SCHOOL_TYPE, int? aitisiID)
        {
            //Εύρεση ειδικότητας της αίτησης
            var eidikotitaAitisis = (from aitisi in db.AITISIS where aitisi.AITISI_ID == aitisiID select aitisi.EIDIKOTITA).FirstOrDefault();
            //Εύρεση ενεργής προκύρηξης
            var activeProkirixi = (from prox in db.PROKIRIXIS where prox.ACTIVE == true select prox.ID).FirstOrDefault();
            //Εύρεση προκυρησόμενων σχολείων με την ειδικότητα της αίτησης
            var prokirixiSchools = (from prox in db.PROKIRIXIS_EIDIKOTITES where prox.PROKIRIXI_ID == activeProkirixi && prox.EIDIKOTITA_ID == eidikotitaAitisis select prox.SCHOOL_ID).ToList();
            //Εύρεση περιφέριας της αίτησης
            var periferiaAitisis = (from aitisis in db.AITISIS where aitisis.AITISI_ID == aitisiID select aitisis.PERIFERIA_ID);
            //Εύρεση σχολείων στην περιφέρεια τα οποία έχει προκυρηχθεί η ειδικότητα της αίτησης
            var filteredSchools = (from school in db.SYS_SCHOOLS
                                   where periferiaAitisis.Contains(school.SCHOOL_PERIFERIA_ID) && prokirixiSchools.Contains(school.SCHOOL_ID)
                                   select new SYS_SCHOOLSViewModel
                                   {
                                       SCHOOL_ID = school.SCHOOL_ID,
                                       SCHOOL_NAME = school.SCHOOL_NAME,
                                       SCHOOL_PERIFERIA_ID = school.SCHOOL_PERIFERIA_ID
                                   });
            if (!string.IsNullOrEmpty(text))
            {
                int possibleInt;
                if (int.TryParse(text, out possibleInt))
                {
                    filteredSchools = filteredSchools.Where(p => p.SCHOOL_ID == possibleInt);
                }
                else
                {
                    filteredSchools = filteredSchools.Where(p => p.SCHOOL_NAME.Contains(text));
                }

            }
            return Json(filteredSchools, JsonRequestBehavior.AllowGet);
        }

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
                    populateViewBagWithAitisi(selectedAitisi);
                    populateSchoolTypes(selectedAitisi.AITISI_ID);
                    populateSchools(selectedAitisi.AITISI_ID);
                }
                else
                {
                    return RedirectToAction("ListAitiseis", "TEACHERS");
                }
            }
            return View();
        }

        public ActionResult Schools_Read([DataSourceRequest] DataSourceRequest request, int aitisiID)
        {
            var schoolsInTeacherAitisisIds = (from school in db.AITISIS_SCHOOLS
                                              where school.AITISI_ID == aitisiID
                                              select new AITISI_SCHOOLSViewModel
                                              {
                                                  ID = school.ID,
                                                  AITISI_ID = school.AITISI_ID,
                                                  PERIFERIA_ID = school.PERIFERIA_ID,
                                                  SCHOOL = school.SCHOOL,
                                                  SCHOOL_TYPE = school.SCHOOL_TYPE
                                              });
            DataSourceResult result = schoolsInTeacherAitisisIds.ToDataSourceResult(request);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #endregion


        #region POPULATORS

        public void populatePeriferiesByUser()
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

        public void populateSchoolTypes(int aitisiID)
        {
            //Εύρεση ειδικότητας της αίτησης
            var eidikotitaAitisis = (from aitisi in db.AITISIS where aitisi.AITISI_ID == aitisiID select aitisi.EIDIKOTITA).FirstOrDefault();
            //Εύρεση ενεργής προκύρηξης
            var activeProkirixi = (from prox in db.PROKIRIXIS where prox.ACTIVE == true select prox.ID).FirstOrDefault();
            //Εύρεση προκυρησόμενων σχολείων με την ειδικότητα της αίτησης
            var prokirixiSchools = (from prox in db.PROKIRIXIS_EIDIKOTITES where prox.PROKIRIXI_ID == activeProkirixi && prox.EIDIKOTITA_ID == eidikotitaAitisis select prox.SCHOOL_ID).ToList();
            //Εύρεση περιφέριας της αίτησης
            var periferiaAitisis = (from aitisis in db.AITISIS where aitisis.AITISI_ID == aitisiID select aitisis.PERIFERIA_ID).FirstOrDefault();

            var filteredSchoolsTypes = (from school in db.SYS_SCHOOLS
                                        where prokirixiSchools.Contains(school.SCHOOL_ID) && periferiaAitisis == school.SCHOOL_PERIFERIA_ID
                                        select school.SCHOOL_TYPEID).ToList();
            var schooltypes = (from t in db.SYS_SCHOOLTYPES where filteredSchoolsTypes.Contains(t.SCHOOL_TYPE_ID) select t).ToList();
            ViewData["schooltypes"] = schooltypes;
        }

        public void populateSchools(int aitisiID)
        {
            //Εύρεση ειδικότητας της αίτησης
            var eidikotitaAitisis = (from aitisi in db.AITISIS where aitisi.AITISI_ID == aitisiID select aitisi.EIDIKOTITA).FirstOrDefault();
            //Εύρεση ενεργής προκύρηξης
            var activeProkirixi = (from prox in db.PROKIRIXIS where prox.ACTIVE == true select prox.ID).FirstOrDefault();
            //Εύρεση προκυρησόμενων σχολείων με την ειδικότητα της αίτησης
            var prokirixiSchools = (from prox in db.PROKIRIXIS_EIDIKOTITES where prox.PROKIRIXI_ID == activeProkirixi && prox.EIDIKOTITA_ID == eidikotitaAitisis select prox.SCHOOL_ID).ToList();
            //Εύρεση περιφέριας της αίτησης
            var periferiaAitisis = (from aitisis in db.AITISIS where aitisis.AITISI_ID == aitisiID select aitisis.PERIFERIA_ID);
            //Εύρεση σχολείων στην περιφέρεια τα οποία έχει προκυρηχθεί η ειδικότητα της αίτησης
            var filteredSchools = (from school in db.SYS_SCHOOLS
                                   where periferiaAitisis.Contains(school.SCHOOL_PERIFERIA_ID) && prokirixiSchools.Contains(school.SCHOOL_ID)
                                   select new SYS_SCHOOLSViewModel
                                   {
                                       SCHOOL_ID = school.SCHOOL_ID,
                                       SCHOOL_NAME = school.SCHOOL_NAME,
                                       SCHOOL_PERIFERIA_ID = school.SCHOOL_PERIFERIA_ID
                                   });
            ViewData["schools"] = filteredSchools;
        }

        public void populateViewBagWithAitisi(AitisisViewModel selectedAitisi)
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


        #region COMBOS DATA SOURCES

        public JsonResult GetGenders(string text)
        {
            var genders = db.SYS_GENDERS.Select(p => new SYS_GENDERSViewModel
            {
                GENDER = p.GENDER,
                GENDER_ID = p.GENDER_ID
            });

            if (!string.IsNullOrEmpty(text))
            {
                genders = genders.Where(p => p.GENDER.Contains(text));
            }

            return Json(genders, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetArmyStatuses(string text)
        {
            var armyStatuses = db.SYS_ARMY.Select(p => new SYS_ARMYViewModel
            {
                ARMY_ID = p.ARMY_ID,
                ARMY_TEXT = p.ARMY_TEXT
            });

            if (!string.IsNullOrEmpty(text))
            {
                armyStatuses = armyStatuses.Where(p => p.ARMY_TEXT.Contains(text));
            }

            return Json(armyStatuses, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPeriferies(string text)
        {
            var armyStatuses = db.SYS_PERIFERIES.Select(p => new SYS_PERIFERIESViewModel
            {
                PERIFERIA_ID = p.PERIFERIA_ID,
                PERIFERIA_NAME = p.PERIFERIA_NAME
            });

            if (!string.IsNullOrEmpty(text))
            {
                int possibleInt;
                if (int.TryParse(text, out possibleInt))
                {
                    armyStatuses = armyStatuses.Where(p => p.PERIFERIA_ID.Equals(possibleInt));
                }
                else
                {
                    armyStatuses = armyStatuses.Where(p => p.PERIFERIA_NAME.Contains(text));
                }
                armyStatuses = armyStatuses.Where(p => p.PERIFERIA_NAME.Contains(text));
            }

            return Json(armyStatuses, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCascadeDimoi(int? periferia, string dimosFilter)
        {
            var dimoi = db.SYS_DIMOS.AsQueryable();

            if (periferia != null)
            {
                dimoi = dimoi.Where(p => p.DIMOS_PERIFERIA == periferia);
            }
            else if (!string.IsNullOrEmpty(dimosFilter))
            {
                int possibleInt;
                if (int.TryParse(dimosFilter, out possibleInt))
                {
                    dimoi = dimoi.Where(p => p.DIMOS_ID.Equals(possibleInt));
                }
                else
                {
                    dimoi = dimoi.Where(p => p.DIMOS.Contains(dimosFilter));
                }
                dimoi = dimoi.Where(p => p.DIMOS.Contains(dimosFilter));
            }

            return Json(dimoi.Select(p => new { DIMOS_ID = p.DIMOS_ID, DIMOS = p.DIMOS }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSchools(string schoolFilter)
        {
            var schools = db.SYS_SCHOOLS.AsQueryable();

            if (!string.IsNullOrEmpty(schoolFilter))
            {
                int possibleInt;
                if (int.TryParse(schoolFilter, out possibleInt))
                {
                    schools = schools.Where(p => p.SCHOOL_ID.Equals(possibleInt));
                }
                else
                {
                    schools = schools.Where(p => p.SCHOOL_NAME.Contains(schoolFilter));
                }
                schools = schools.Where(p => p.SCHOOL_NAME.Contains(schoolFilter));
            }
            return Json(schools.Select(p => new { SCHOOL_ID = p.SCHOOL_ID, SCHOOL_NAME = p.SCHOOL_NAME }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAnergiesOld(string text)
        {
            var anergies = db.SYS_ANERGIA_OLD.AsQueryable();

            if (!string.IsNullOrEmpty(text))
            {
                anergies = anergies.Where(p => p.ANERGIA_TAG.Contains(text));
            }

            return Json(anergies, JsonRequestBehavior.AllowGet);
        }

        #endregion


        #region LOCAL FUNCTIONS

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

        #endregion


        #region ERROR PAGES

        public ActionResult Error(string notify = null)
        {
            if (notify != null) this.ShowMessage(MessageType.Warning, notify);

            return View();
        }

        public ActionResult ErrorUser(string notify = null)
        {
            if (notify != null) this.ShowMessage(MessageType.Warning, notify);

            return View();
        }

        public ActionResult ErrorData(string notify = null)
        {
            if (notify != null) this.ShowMessage(MessageType.Warning, notify);

            return View();
        }

        #endregion

    }
}
