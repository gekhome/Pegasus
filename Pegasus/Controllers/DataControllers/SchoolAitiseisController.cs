using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Mvc;
using System.IO;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using Pegasus.Filters;
using Pegasus.DAL;
using Pegasus.Models;
using Pegasus.BPM;
using Pegasus.Notification;
using Pegasus.Extensions;
using Pegasus.Services;

namespace Pegasus.Controllers.DataControllers
{
    [ErrorHandlerFilter]
    public class SchoolAitiseisController : ControllerUnit
    {
        private readonly PegasusDBEntities db;
        private USER_SCHOOLS loggedSchool;

        private readonly IAitisiService aitisiService;
        private readonly IAitisiSchoolsService aitisiSchoolsService;
        private readonly IReeducationService reeducationService;    
        private readonly IUploadedFilesService uploadedFilesService;
        private readonly IWorkTeachingService workTeachingService;
        private readonly IWorkVocationService workVocationService;
        private readonly IWorkFreelanceService workFreelanceService;

        public SchoolAitiseisController(PegasusDBEntities entities, IAitisiService aitisiService, 
            IAitisiSchoolsService aitisiSchoolsService, IReeducationService reeducationService,
            IUploadedFilesService uploadedFilesService, IWorkTeachingService workTeachingService,
            IWorkFreelanceService workFreelanceService, IWorkVocationService workVocationService) : base(entities)
        {
            db = entities;

            this.aitisiService = aitisiService;
            this.aitisiSchoolsService = aitisiSchoolsService;
            this.reeducationService = reeducationService;
            this.uploadedFilesService = uploadedFilesService;
            this.workTeachingService = workTeachingService;
            this.workVocationService = workVocationService;
            this.workFreelanceService = workFreelanceService;
        }


        #region ΣΧΟΛΙΚΕΣ ΜΟΝΑΔΕΣ ΕΠΙΛΟΓΗΣ

        public ActionResult ListSchools(int? aitisiId)
        {
            bool val1 = (System.Web.HttpContext.Current.User != null) && System.Web.HttpContext.Current.User.Identity.IsAuthenticated;
            if (!val1)
            {
                ViewBag.loggedUser = "(χωρίς σύνδεση)";
                return RedirectToAction("Login", "USER_SCHOOLS");
            }
            else
            {
                loggedSchool = GetLoginSchool();
            }

            var selectedAitisi = (from a in db.AITISIS
                                  where a.AITISI_ID == aitisiId
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
                if (VerifyAitisis(selectedAitisi) == false)
                {
                    string msg = "Η επιλεγμένη αίτηση έχει ασυμπλήρωτα στοιχεία.Πατήστε 'Επεξεργασία' πρώτα και συμπληρώστε την αίτηση.";
                    return RedirectToAction("MainSchool", "SchoolAitiseis", new { notify = msg });
                }
                else
                {
                    PopulateViewBagWithAitisi(selectedAitisi);
                    PopulateSchoolTypes(selectedAitisi.AITISI_ID);
                    PopulateSchools(selectedAitisi.AITISI_ID);
                    return View();
                }
            }
            else 
            {
                string msg = "Δεν υπάρχει επιλεγμένη αίτηση.";
                return RedirectToAction("MainSchool", "SchoolAitiseis", new { notify = msg }); 
            }
        }

        public ActionResult Schools_Read([DataSourceRequest] DataSourceRequest request, int aitisiID)
        {
            List<AITISI_SCHOOLSViewModel> data = aitisiSchoolsService.Read(aitisiID);

            return Json(data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Schools_Create([DataSourceRequest] DataSourceRequest request, AITISI_SCHOOLSViewModel data, int aitisiID)
        {
            var newdata = new AITISI_SCHOOLSViewModel();
            int prokirixiId = Common.GetAdminProkirixiID();

            if (data != null && ModelState.IsValid)
            {
                var savedSchools = db.AITISIS_SCHOOLS.Where(x => x.AITISI_ID == aitisiID && x.SCHOOL == data.SCHOOL).ToList();
                if (savedSchools.Count == 0)
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
            int prokirixiId = Common.GetAdminProkirixiID();

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


        #region ΕΠΕΞΕΡΓΑΣΙΑ ΑΙΤΗΣΗΣ

        public ActionResult AitisiEdit(int aitisiId)
        {
            bool val1 = (System.Web.HttpContext.Current.User != null) && System.Web.HttpContext.Current.User.Identity.IsAuthenticated;
            if (!val1)
            {
                ViewBag.loggedUser = "(χωρίς σύνδεση)";
                return RedirectToAction("Login", "USER_SCHOOLS");
            }
            else
            {
                loggedSchool = GetLoginSchool();
            }

            int prokirixiId = Common.GetOpenProkirixiID(active: true);
            if (prokirixiId == 0)
            {
                string msg = "Δεν βρέθηκε ενεργή Προκήρυξη.";
                return RedirectToAction("MainSchool", "SchoolAtiseis", new { notify = msg });
            }

            AitisisViewModel aitisi = aitisiService.GetModel(aitisiId);
            return View(aitisi);
        }

        [HttpPost]
        public ActionResult AitisiEdit(AitisisViewModel data, int aitisiId)
        {
            bool val1 = (System.Web.HttpContext.Current.User != null) && System.Web.HttpContext.Current.User.Identity.IsAuthenticated;
            if (!val1)
            {
                ViewBag.loggedUser = "(χωρίς σύνδεση)";
                return RedirectToAction("Login", "USER_SCHOOLS");
            }
            else
            {
                loggedSchool = GetLoginSchool();
            }

            data.AFM = aitisiService.GetModel(aitisiId).AFM;
            string ErrorMsg = AitisiRules.ValidateFields(data, isnew: false);
            if (!string.IsNullOrEmpty(ErrorMsg))
            {
                this.AddNotification("Η αποθήκευση απέτυχε λόγω επικύρωσης δεδομένων. " + ErrorMsg, NotificationType.ERROR);
                return View(data);
            }
            if (ModelState.IsValid)
            {
                AITISIS entity = aitisiService.EditAitisi(data, aitisiId, auditor: loggedSchool.USERNAME);

                this.AddNotification("Η αποθήκευση της αίτησης ολοκληρώθηκε με επιτυχία.", NotificationType.SUCCESS);
                AitisisViewModel newAitisi = aitisiService.GetModel(entity.AITISI_ID);
                return View(newAitisi);
            }
            else
            {
                this.AddNotification("Η αποθήκευση απέτυχε λόγω σφαλμάτων χρήστη. Δείτε τη σύνοψη στο κάτω μέρος της σελίδας.", NotificationType.ERROR);
                AitisisViewModel model = aitisiService.GetModel(aitisiId);
                return View(model);
            }
        }

        public ActionResult AitisiPrint(int aitisiId)
        {
            bool val1 = (System.Web.HttpContext.Current.User != null) && System.Web.HttpContext.Current.User.Identity.IsAuthenticated;
            if (!val1)
            {
                ViewBag.loggedUser = "(χωρίς σύνδεση)";
                return RedirectToAction("Login", "USER_SCHOOLS");
            }
            else
            {
                loggedSchool = GetLoginSchool();
            }
            var data = (from t in db.AITISIS
                        where t.AITISI_ID == aitisiId
                        select new AitisisViewModel
                        {
                            AITISI_ID = t.AITISI_ID,
                            AITISI_PROTOCOL = t.AITISI_PROTOCOL
                        }).FirstOrDefault();

            return View(data);
        }

        #endregion ΑΙΤΗΣΕΙΣ


        #region ΑΙΤΗΣΕΙΣ - ΠΡΟΫΠΗΡΕΣΙΕΣ

        public ActionResult MainSchool(int aitisiID = 0, string notify = null)
        {
            bool val1 = (System.Web.HttpContext.Current.User != null) && System.Web.HttpContext.Current.User.Identity.IsAuthenticated;
            if (!val1)
            {
                ViewBag.loggedUser = "(χωρίς σύνδεση)";
                return RedirectToAction("Login", "USER_SCHOOLS");
            }
            else
            {
                loggedSchool = GetLoginSchool();
            }
            int prokirixiId = Common.GetOpenProkirixiID(true);
            if (prokirixiId == 0)
            {
                string msg = "Δεν βρέθηκε ενεργή Προκήρυξη.";
                return RedirectToAction("Index", "School", new { notify = msg });
            }
            int schoolYearId = (int)Common.GetOpenProkirixi().SCHOOL_YEAR;
            ViewData["prokirixiProtocol"] = Common.GetOpenProkirixi().PROTOCOL;
            ViewData["schoolYearText"] = Common.GetSchoolYearText(schoolYearId);

            // set Aitisi Info header
            sqlTEACHER_AITISEIS SelectedAitisi = GetSelectedAitisi(aitisiID);
            if (SelectedAitisi == null)
            {
                SelectedAitisi = GetSelectedAitisi(loggedSchool.USER_SCHOOLID);
                // if this too is null then there are no aitiseis
                if (SelectedAitisi == null)
                {
                    string msg = "Δεν βρέθηκαν αιτήσεις για τη συγκεκριμένη προκήρυξη και σχολείο.";
                    return RedirectToAction("Index", "School", new { notify = msg });
                }
                ViewBag.SelectedAitisiData = SelectedAitisi;
                ViewData["aitisi_id"] = SelectedAitisi.AITISI_ID;
            }
            else
            {
                // at this point aitisiID is defined by selection
                ViewBag.SelectedAitisiData = SelectedAitisi;
                ViewData["aitisi_id"] = aitisiID;
            }
            if (notify != null) this.ShowMessage(MessageType.Info, notify);

            PopulateTeachTypes();
            PopulateSchoolYears();
            PopulateIncomeYears();

            return View();
        }

        public ActionResult Aitiseis_Read([DataSourceRequest] DataSourceRequest request, int aitisiID = 0)
        {
            int prokirixiId = Common.GetOpenProkirixiID(active: true);
            int schoolId = (int)GetLoginSchool().USER_SCHOOLID;

            IEnumerable<sqlTeacherAitiseisModel> data = ReadAitiseis(prokirixiId, schoolId);

            return Json(data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }


        #region GRID TEACHING

        public ActionResult Teaching_Read([DataSourceRequest] DataSourceRequest request, int aitisiID = 0)
        {
            IEnumerable<ViewModelTeaching> data = workTeachingService.Read(aitisiID);

            return Json(data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Teaching_Create([DataSourceRequest] DataSourceRequest request, ViewModelTeaching data, int aitisiID = 0)
        {
            ViewModelTeaching newdata = new ViewModelTeaching();

            if (!(aitisiID > 0))
            {
                ModelState.AddModelError("", "Δεν έχει γίνει επιλογή αίτησης. Η ενημέρωση ακυρώθηκε.");
                return Json(new[] { data }.ToDataSourceResult(request, ModelState), JsonRequestBehavior.AllowGet);
            }
            if (data != null)
            {
                string ErrorMsg = Kerberos.ValidateFieldsTeaching(data);
                if (!string.IsNullOrEmpty(ErrorMsg))
                {
                    ModelState.AddModelError("", ErrorMsg);
                }
                string DuplMsg = Kerberos.PreventDuplicateTeaching(data, aitisiID);
                if (!string.IsNullOrEmpty(DuplMsg))
                {
                    ModelState.AddModelError("", DuplMsg);
                }
                if (ModelState.IsValid)
                {
                    workTeachingService.Create(data, aitisiID);
                    newdata = workTeachingService.Refresh(data.EXP_ID);
                }
            }
            return Json(new[] { newdata }.ToDataSourceResult(request, ModelState), JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Teaching_Update([DataSourceRequest] DataSourceRequest request, ViewModelTeaching data, int aitisiID = 0)
        {
            ViewModelTeaching newdata = new ViewModelTeaching();

            if (!(aitisiID > 0))
            {
                ModelState.AddModelError("", "Δεν έχει γίνει επιλογή αίτησης. Η ενημέρωση ακυρώθηκε.");
                return Json(new[] { data }.ToDataSourceResult(request, ModelState), JsonRequestBehavior.AllowGet);
            }
            if (data != null)
            {
                string ErrorMsg = Kerberos.ValidateFieldsTeaching(data);
                if (!string.IsNullOrEmpty(ErrorMsg))
                {
                    ModelState.AddModelError("", ErrorMsg);
                }
                if (ModelState.IsValid)
                {
                    workTeachingService.Update(data, aitisiID);
                    newdata = workTeachingService.Refresh(data.EXP_ID);
                }
            }
            return Json(new[] { newdata }.ToDataSourceResult(request, ModelState), JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Teaching_Destroy([DataSourceRequest] DataSourceRequest request, ViewModelTeaching data)
        {
            if (data != null)
            {
                workTeachingService.Destroy(data);
            }
            return Json(new[] { data }.ToDataSourceResult(request, ModelState), JsonRequestBehavior.AllowGet);
        }

        #endregion


        #region GRID VOCATIONAL

        public ActionResult Vocation_Read([DataSourceRequest] DataSourceRequest request, int aitisiID = 0)
        {
            IEnumerable<ViewModelVocational> data = workVocationService.Read(aitisiID);

            return Json(data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Vocation_Create([DataSourceRequest] DataSourceRequest request, ViewModelVocational data, int aitisiID = 0)
        {
            ViewModelVocational newdata = new ViewModelVocational();

            if (!(aitisiID > 0))
            {
                ModelState.AddModelError("", "Πρέπει πρώτα να γίνει επιλογή αίτησης. Η καταχώρηση ακυρώθηκε.");
                return Json(new[] { data }.ToDataSourceResult(request, ModelState), JsonRequestBehavior.AllowGet);
            }
            if (data != null)
            {
                string ErrorMsg = Kerberos.ValidateFieldsVocational(data);
                if (!string.IsNullOrEmpty(ErrorMsg))
                {
                    ModelState.AddModelError("", ErrorMsg);
                }
                string DuplMsg = Kerberos.PreventDuplicateVocational(data, aitisiID);
                if (!string.IsNullOrEmpty(DuplMsg))
                {
                    ModelState.AddModelError("", DuplMsg);
                }
                if (ModelState.IsValid)
                {
                    workVocationService.Create(data, aitisiID);
                    newdata = workVocationService.Refresh(data.EXP_ID);
                }
            }
            return Json(new[] { newdata }.ToDataSourceResult(request, ModelState), JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Vocation_Update([DataSourceRequest] DataSourceRequest request, ViewModelVocational data, int aitisiID = 0)
        {
            ViewModelVocational newdata = new ViewModelVocational();

            if (!(aitisiID > 0))
            {
                ModelState.AddModelError("", "Πρέπει πρώτα να γίνει επιλογή αίτησης. Η καταχώρηση ακυρώθηκε.");
                return Json(new[] { data }.ToDataSourceResult(request, ModelState), JsonRequestBehavior.AllowGet);
            }
            if (data != null)
            {
                string ErrorMsg = Kerberos.ValidateFieldsVocational(data);
                if (!string.IsNullOrEmpty(ErrorMsg))
                {
                    ModelState.AddModelError("", ErrorMsg);
                }
                if (ModelState.IsValid)
                {
                    workVocationService.Update(data, aitisiID);
                    newdata = workVocationService.Refresh(data.EXP_ID);
                }
            }
            return Json(new[] { newdata }.ToDataSourceResult(request, ModelState), JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Vocation_Destroy([DataSourceRequest] DataSourceRequest request, ViewModelVocational data)
        {
            if (data != null)
            {
                workVocationService.Destroy(data);
            }
            return Json(new[] { data }.ToDataSourceResult(request, ModelState), JsonRequestBehavior.AllowGet);
        }

        #endregion


        #region GRID FREELANCE

        public ActionResult Freelance_Read([DataSourceRequest] DataSourceRequest request, int aitisiID = 0)
        {
            IEnumerable<ViewModelFreelance> data = workFreelanceService.Read(aitisiID);

            return Json(data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Freelance_Create([DataSourceRequest] DataSourceRequest request, ViewModelFreelance data, int aitisiID = 0)
        {
            ViewModelFreelance newdata = new ViewModelFreelance();

            if (!(aitisiID > 0))
            {
                ModelState.AddModelError("", "Πρέπει πρώτα να γίνει επιλογή αίτησης. Η καταχώρηση ακυρώθηκε.");
                return Json(new[] { data }.ToDataSourceResult(request, ModelState), JsonRequestBehavior.AllowGet);
            }
            if (data != null)
            {
                string ErrorMsg = Kerberos.ValidateFieldsFreelance(data);
                if (!string.IsNullOrEmpty(ErrorMsg))
                {
                    ModelState.AddModelError("", ErrorMsg);
                }
                string DuplMsg = Kerberos.PreventDuplicateFreelance(data, aitisiID);
                if (!string.IsNullOrEmpty(DuplMsg))
                {
                    ModelState.AddModelError("", DuplMsg);
                }
                if (ModelState.IsValid)
                {
                    workFreelanceService.Create(data, aitisiID);
                    newdata = workFreelanceService.Refresh(data.EXP_ID);
                }
            }
            return Json(new[] { newdata }.ToDataSourceResult(request, ModelState), JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Freelance_Update([DataSourceRequest] DataSourceRequest request, ViewModelFreelance data, int aitisiID = 0)
        {
            ViewModelFreelance newdata = new ViewModelFreelance();

            if (!(aitisiID > 0))
            {
                ModelState.AddModelError("", "Πρέπει πρώτα να γίνει επιλογή αίτησης. Η καταχώρηση ακυρώθηκε.");
                return Json(new[] { data }.ToDataSourceResult(request, ModelState), JsonRequestBehavior.AllowGet);
            }
            if (data != null)
            {
                string ErrorMsg = Kerberos.ValidateFieldsFreelance(data);
                if (!string.IsNullOrEmpty(ErrorMsg))
                {
                    ModelState.AddModelError("", ErrorMsg);
                }
                if (ModelState.IsValid)
                {
                    workFreelanceService.Update(data, aitisiID);
                    newdata = workFreelanceService.Refresh(data.EXP_ID);
                }
            }
            return Json(new[] { newdata }.ToDataSourceResult(request, ModelState), JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Freelance_Destroy([DataSourceRequest] DataSourceRequest request, ViewModelFreelance data)
        {
            if (data != null)
            {
                workFreelanceService.Destroy(data);
            }
            return Json(new[] { data }.ToDataSourceResult(request, ModelState), JsonRequestBehavior.AllowGet);
        }

        #endregion


        #region ΜΕΤΑΦΟΡΑ ΠΑΛΑΙΩΝ ΠΡΟΫΠΗΡΕΣΙΩΝ (ΝΕΟ 6/6/2018)

        // ΝΕΟΣ ΤΡΟΠΟΣ ΜΕΤΑΦΟΡΑΣ ΠΡΟΫΠΗΡΕΣΙΩΝ (6/6/2018)
        public ActionResult TransferExperiences(int aitisiID = 0)
        {
            int response = Transfer.TransferAllWorkExperience(aitisiID);

            string msg = Transfer.ErrorCodeExperienceDictionary(response);

            return Json(msg, JsonRequestBehavior.AllowGet);
        }

        #endregion


        #region ΟΛΙΚΗ ΜΟΡΙΟΔΟΤΗΣΗ

        public ActionResult MoriaResultsView(int aitisiId)
        {
            bool val1 = (System.Web.HttpContext.Current.User != null) && System.Web.HttpContext.Current.User.Identity.IsAuthenticated;
            if (!val1)
            {
                ViewBag.loggedUser = "(χωρίς σύνδεση)";
                return RedirectToAction("Login", "USER_SCHOOLS");
            }
            else
            {
                loggedSchool = GetLoginSchool();
            }

            ExperienceResultsViewModel moriaResults = aitisiService.GetResults(aitisiId);

            return View(moriaResults);
        }
     
        public ActionResult MoriaResultsPrint(int aitisiId)
        {
            bool val1 = (System.Web.HttpContext.Current.User != null) && System.Web.HttpContext.Current.User.Identity.IsAuthenticated;
            if (!val1)
            {
                ViewBag.loggedUser = "(χωρίς σύνδεση)";
                return RedirectToAction("Login", "USER_SCHOOLS");
            }
            else
            {
                loggedSchool = GetLoginSchool();
            }
            var data = (from t in db.AITISIS
                        where t.AITISI_ID == aitisiId
                        select new AitisisViewModel
                        {
                            AITISI_ID = t.AITISI_ID,
                            AITISI_PROTOCOL = t.AITISI_PROTOCOL,
                        }).FirstOrDefault();
            return View(data);
        }

        public ActionResult Moriodotisi()
        {
            int prokirixiId = Common.GetOpenProkirixiID(true);
            loggedSchool = GetLoginSchool();

            if (prokirixiId == 0)
            {
                string msg = "Δεν βρέθηκε ενεργή Προκήρυξη.";
                return RedirectToAction("MainSchool", "SchoolAtiseis", new { notify = msg });
            }

            var aitiseis = (from a in db.AITISIS where a.PROKIRIXI_ID == prokirixiId && a.SCHOOL_ID == loggedSchool.USER_SCHOOLID select a).ToList();

            foreach (var aitisi in aitiseis)
            {
                int aitisiId = aitisi.AITISI_ID;
                TEACHERS teacher = (from d in db.TEACHERS where d.AFM == aitisi.AFM select d).FirstOrDefault();
                ExperienceResultsViewModel experienceResults = new ExperienceResultsViewModel()
                {

                    TEACHING_MORIA_FINAL = (from t in db.sqlEXP_TEACHING_FINAL where t.AITISI_ID == aitisiId select t.MORIA_FINAL).FirstOrDefault() ?? 0d,
                    VOCATION_MORIA = (from t in db.sqlEXP_VOCATION_1 where t.AITISI_ID == aitisiId select t.MSUM).FirstOrDefault() ?? 0d,
                    VOCATION_MORIA_FINAL = (from t in db.sqlEXP_VOCATION_FINAL where t.AITISI_ID == aitisiId select t.MORIA_TOTAL).FirstOrDefault() ?? 0d,
                    FREELANCE_MORIA = (from t in db.sqlEXP_FREELANCE_1 where t.AITISI_ID == aitisiId select t.MSUM).FirstOrDefault() ?? 0d,
                    FREELANCE_MORIA_FINAL = (from t in db.sqlEXP_FREELANCE_FINAL where t.AITISI_ID == aitisiId select t.MORIA_TOTAL).FirstOrDefault() ?? 0d,
                    WORK_MORIA_FINAL = (from t in db.sqlEXP_WORK_FINAL where t.AITISI_ID == aitisiId select t.WORK_MORIA_FINAL).FirstOrDefault() ?? 0d
                };

                aitisi.MORIA_TEACH = (float?)experienceResults.TEACHING_MORIA_FINAL;
                aitisi.MORIA_WORK1 = (float?)experienceResults.VOCATION_MORIA_FINAL;
                aitisi.MORIA_WORK2 = (float?)experienceResults.FREELANCE_MORIA_FINAL;
                aitisi.MORIA_WORK = (float)experienceResults.WORK_MORIA_FINAL;
                aitisi.MORIA_ANERGIA = AitisiMoria.MoriaAnergia(aitisi);
                aitisi.MORIA_SOCIAL = AitisiMoria.MoriaSocial(aitisi);
                aitisi.MORIA_TOTAL = AitisiMoria.MoriaTotal(aitisi);
                db.Entry(aitisi).State = EntityState.Modified;
                db.SaveChanges();
            }

            string message = "Η μοριοδότηση των αιτήσεων ολοκληρώθηκε.";
            return Json(message, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #endregion ΑΙΤΗΣΕΙΣ - ΠΡΟΫΠΗΡΕΣΙΕΣ


        #region ΕΠΙΜΟΡΦΩΣΕΙΣ (ΝΕΟ 26-04-2019)

        public ActionResult ReeducationList(int aitisiID = 0, string notify = null)
        {
            bool val1 = (System.Web.HttpContext.Current.User != null) && System.Web.HttpContext.Current.User.Identity.IsAuthenticated;
            if (!val1)
            {
                ViewBag.loggedUser = "(χωρίς σύνδεση)";
                return RedirectToAction("Login", "USER_SCHOOLS");
            }
            else
            {
                loggedSchool = GetLoginSchool();
            }
            // set Aitisi Info header
            sqlTEACHER_AITISEIS SelectedAitisi = GetSelectedAitisi(aitisiID);
            if (SelectedAitisi == null)
            {
                string errMsg = "Δεν υπάρχει επιλεγμένη αίτηση. Κλείστε την καρτέλα και δοκιμάστε πάλι.";
                return RedirectToAction("MainSchool", "SchoolAitiseis", new { notify = errMsg });
            }
            else
            {
                ViewBag.SelectedAitisiData = SelectedAitisi;
                ViewData["aitisi_id"] = SelectedAitisi.AITISI_ID;
            }

            if (notify != null) this.ShowMessage(MessageType.Warning, notify);
            return View();
        }

        public ActionResult Reeducation_Read([DataSourceRequest] DataSourceRequest request, int aitisiID)
        {
            List<ReeducationViewModel> data = reeducationService.Read(aitisiID);

            return Json(data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Reeducation_Create([DataSourceRequest] DataSourceRequest request, ReeducationViewModel data, int aitisiID = 0)
        {
            ReeducationViewModel newdata = new ReeducationViewModel();

            if (!(aitisiID > 0))
                ModelState.AddModelError("", "Ο κωδικός αίτησης δεν είναι έγκυρος. Η καταχώρηση ακυρώθηκε.");

            if (data != null && ModelState.IsValid)
            {
                reeducationService.Create(data, aitisiID);
                newdata = reeducationService.Refresh(data.EDUCATION_ID);
            }
            return Json(new[] { newdata }.ToDataSourceResult(request, ModelState), JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Reeducation_Update([DataSourceRequest] DataSourceRequest request, ReeducationViewModel data, int aitisiID = 0)
        {
            ReeducationViewModel newdata = new ReeducationViewModel();

            if (!(aitisiID > 0))
                ModelState.AddModelError("", "Ο κωδικός αίτησης δεν είναι έγκυρος. Η καταχώρηση ακυρώθηκε.");

            if (data != null && ModelState.IsValid)
            {
                reeducationService.Update(data, aitisiID);
                newdata = reeducationService.Refresh(data.EDUCATION_ID);
            }
            return Json(new[] { newdata }.ToDataSourceResult(request, ModelState), JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Reeducation_Destroy([DataSourceRequest] DataSourceRequest request, ReeducationViewModel data)
        {
            if (data != null)
            {
                reeducationService.Destroy(data);
            }
            return Json(new[] { data }.ToDataSourceResult(request, ModelState), JsonRequestBehavior.AllowGet);
        }


        #region ΜΕΤΑΦΟΡΑ ΕΠΙΜΟΡΦΩΣΕΩΝ

        public ActionResult TransferReeducations(int aitisiId = 0)
        {
            int response = Transfer.TransferAllReeducations(aitisiId);

            string msg = Transfer.ErrorCodeReeducationDictionary(response);

            return RedirectToAction("ReeducationList", "SchoolAitiseis", new { aitisiID = aitisiId, notify = msg });
        }


        #endregion

        #endregion


        #region ΒΟΗΘΗΤΙΚΗ ΣΕΛΙΔΑ ΑΡΧΕΙΩΝ ΑΙΤΗΣΗΣ

        public ActionResult AitisiUploadedFiles(int aitisiId = 0, string notify = null)
        {
            bool val1 = (System.Web.HttpContext.Current.User != null) && System.Web.HttpContext.Current.User.Identity.IsAuthenticated;
            if (!val1)
            {
                ViewBag.loggedUser = "(χωρίς σύνδεση)";
                return RedirectToAction("Login", "USER_SCHOOLS");
            }
            else
            {
                loggedSchool = GetLoginSchool();
            }
            // set Aitisi Info header
            sqlTEACHER_AITISEIS SelectedAitisi = GetSelectedAitisi(aitisiId);
            if (SelectedAitisi == null)
            {
                string errMsg = "Δεν υπάρχει επιλεγμένη αίτηση. Κλείστε την καρτέλα και δοκιμάστε πάλι.";
                return RedirectToAction("MainSchool", "SchoolAitiseis", new { notify = errMsg });
            }
            else
            {
                ViewBag.SelectedAitisiData = SelectedAitisi;
                ViewData["aitisi_id"] = SelectedAitisi.AITISI_ID;
            }
            if (notify != null) this.ShowMessage(MessageType.Warning, notify);

            return View();
        }

        #endregion


        #region ΑΙΤΗΣΕΙΣ ΚΑΙ ΑΝΕΒΑΣΜΕΝΑ ΑΡΧΕΙΑ

        public ActionResult AitiseisUploads(string notify = null)
        {
            bool val1 = (System.Web.HttpContext.Current.User != null) && System.Web.HttpContext.Current.User.Identity.IsAuthenticated;
            if (!val1)
            {
                ViewBag.loggedUser = "(χωρίς σύνδεση)";
                return RedirectToAction("Login", "USER_SCHOOLS");
            }
            else
            {
                loggedSchool = GetLoginSchool();
            }
            int prokirixiId = Common.GetAdminProkirixiID();
            if (prokirixiId == 0)
            {
                this.ShowMessage(MessageType.Warning, "Δεν βρέθηκε ενεργοποιημένη προκήρυξη για διαχείριση.");
                return RedirectToAction("Index", "Admin");
            }

            int schoolYearId = (int)Common.GetAdminProkirixi().SCHOOL_YEAR;
            ViewData["prokirixiProtocol"] = Common.GetAdminProkirixi().PROTOCOL;
            ViewData["schoolYearText"] = Common.GetSchoolYearText(schoolYearId);

            if (notify != null) this.ShowMessage(MessageType.Warning, notify);

            return View();
        }

        public ActionResult AitiseisUploads_Read([DataSourceRequest] DataSourceRequest request)
        {
            int prokirixiId = Common.GetOpenProkirixiID(active: true);
            int schoolId = (int)GetLoginSchool().USER_SCHOOLID;

            IEnumerable<sqlTeacherAitiseisModel> data = ReadAitiseis(prokirixiId, schoolId);

            return Json(data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }


        #region CHILDREN GRIDS WITH UPLOAED FILES

        public ActionResult GeneralFiles_Read([DataSourceRequest] DataSourceRequest request, int aitisiID = 0)
        {
            List<xUploadedGeneralFilesModel> data = uploadedFilesService.ReadGeneral(aitisiID);

            return Json(data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        public ActionResult TeachingFiles_Read([DataSourceRequest] DataSourceRequest request, int aitisiID = 0)
        {
            List<xUploadedTeachingFilesModel> data = uploadedFilesService.ReadTeaching(aitisiID);

            return Json(data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        public ActionResult VocationFiles_Read([DataSourceRequest] DataSourceRequest request, int aitisiID = 0)
        {
            List<xUploadedVocationFilesModel> data = uploadedFilesService.ReadVocation(aitisiID);

            return Json(data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        #endregion


        #region DOWNLOAD FILES ACTIONS

        public FileResult DownloadGeneralFile(int fileId, string afm)
        {
            string physicalPath = DOCUMENTS_PATH + afm + "/";
            string filename = "";

            var fileinfo = (from d in db.UploadGeneralFiles where d.FileID == fileId select d).FirstOrDefault();
            if (fileinfo != null)
            {
                filename = fileinfo.FileName;
            }
            return File(Path.Combine(Server.MapPath(physicalPath), filename), System.Net.Mime.MediaTypeNames.Application.Octet, filename);
        }

        public FileResult DownloadTeachingFile(int fileId, string afm)
        {
            string physicalPath = TEACHING_PATH + afm + "/";
            string filename = "";

            var fileinfo = (from d in db.UploadTeachingFiles where d.FileID == fileId select d).FirstOrDefault();
            if (fileinfo != null)
            {
                filename = fileinfo.FileName;
            }
            return File(Path.Combine(Server.MapPath(physicalPath), filename), System.Net.Mime.MediaTypeNames.Application.Octet, filename);
        }

        public FileResult DownloadVocationFile(int fileId, string afm)
        {
            string physicalPath = VOCATION_PATH + afm + "/";
            string filename = "";

            var fileinfo = (from d in db.UploadVocationFiles where d.FileID == fileId select d).FirstOrDefault();
            if (fileinfo != null)
            {
                filename = fileinfo.FileName;
            }
            return File(Path.Combine(Server.MapPath(physicalPath), filename), System.Net.Mime.MediaTypeNames.Application.Octet, filename);
        }

        #endregion

        #endregion

    }
    
}