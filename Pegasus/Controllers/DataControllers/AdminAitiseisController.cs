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
    public class AdminAitiseisController : ControllerUnit
    {
        private readonly PegasusDBEntities db;
        private USER_ADMINS loggedAdmin;

        private readonly IAitisiService aitisiService;
        private readonly IAitisiSchoolsService aitisiSchoolsService;
        private readonly IReeducationService reeducationService;

        private readonly IUploadedFilesService uploadedFilesService;
        private readonly IWorkTeachingService workTeachingService;
        private readonly IWorkVocationService workVocationService;
        private readonly IWorkFreelanceService workFreelanceService;

        public AdminAitiseisController(PegasusDBEntities entities, IAitisiService aitisiService,
            IAitisiSchoolsService aitisiSchoolsService, IReeducationService reeducationService,
            IUploadedFilesService uploadedFilesService, IWorkTeachingService workTeachingService,
            IWorkVocationService workVocationService, IWorkFreelanceService workFreelanceService) : base(entities)
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


        #region ΔΙΑΧΕΙΡΙΣΗ ΑΙΤΗΣΕΩΝ

        public ActionResult ListAitiseis(string notify = null)
        {
            bool val1 = (System.Web.HttpContext.Current.User != null) && System.Web.HttpContext.Current.User.Identity.IsAuthenticated;
            if (!val1)
            {
                ViewBag.loggedUser = "(χωρίς σύνδεση)";
                return RedirectToAction("Login", "USER_ADMINS");
            }
            else
            {
                loggedAdmin = GetLoginAdmin();
            }
            int prokirixiId = Common.GetAdminProkirixiID();
            if (prokirixiId == 0)
            {
                this.ShowMessage(MessageType.Warning, "Δεν βρέθηκε Διαχειριστική Προκήρυξη.");
                return RedirectToAction("Index", "Admin");
            }

            int schoolYearId = (int)Common.GetAdminProkirixi().SCHOOL_YEAR;
            ViewData["prokirixiProtocol"] = Common.GetAdminProkirixi().PROTOCOL;
            ViewData["schoolYearText"] = Common.GetSchoolYearText(schoolYearId);

            if (notify != null) this.ShowMessage(MessageType.Warning, notify);

            PopulateEidikotites();
            PopulateSchools();

            return View();
        }

        public ActionResult Read([DataSourceRequest] DataSourceRequest request)
        {
            int prokirixiId = Common.GetAdminProkirixiID();

            IEnumerable<AitisisGridViewModel> data = aitisiService.Read(prokirixiId);

            return Json(data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Create([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<AitisisGridViewModel> data)
        {
            var results = new List<AitisisGridViewModel>();

            int prokirixiId = Common.GetAdminProkirixiID();

            foreach (var item in data)
            {
                if (item != null)
                {
                    aitisiService.Create(item, prokirixiId);
                    results.Add(item);
                }
            }
            return Json(results.ToDataSourceResult(request, ModelState), JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Update([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<AitisisGridViewModel> data)
        {
            int prokirixiId = Common.GetAdminProkirixiID();

            foreach (var item in data)
            {
                if (item != null)
                {
                    aitisiService.Update(item, prokirixiId);
                }
            }
            return Json(data.ToDataSourceResult(request, ModelState), JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Destroy([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<AitisisGridViewModel> data)
        {
            if (data.Any())
            {
                foreach (var item in data)
                {
                    if (Common.CanDeleteAitisi(item.AITISI_ID))
                    {
                        aitisiService.Destroy(item);
                    }
                    else
                    {
                        ModelState.AddModelError("AITISI_ID", "Δεν μπορεί να διαγραφεί η αίτηση διότι έχει σχολεία επιλογής ή/και προϋπηρεσίες.");
                    }
                }
            }
            return Json(data.ToDataSourceResult(request, ModelState), JsonRequestBehavior.AllowGet);
        }

        #endregion


        #region EDIT AITISIS (used by ListAitiseis and MainAdmin)

        public ActionResult EditAitisi(int aitisiId)
        {
            bool val1 = (System.Web.HttpContext.Current.User != null) && System.Web.HttpContext.Current.User.Identity.IsAuthenticated;
            if (!val1)
            {
                ViewBag.loggedUser = "(χωρίς σύνδεση)";
                return RedirectToAction("Login", "USER_ADMINS");
            }
            else
            {
                loggedAdmin = GetLoginAdmin();
            }

            int prokirixiId = Common.GetAdminProkirixiID();
            if (prokirixiId == 0)
            {
                string msg = "Δεν βρέθηκε Διαχειριστική Προκήρυξη.";
                return RedirectToAction("ListAitiseis", "AdminAitiseis", new { notify = msg });
            }

            AitisisViewModel aitisi = aitisiService.GetModel(aitisiId);
            return View(aitisi);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditAitisi(AitisisViewModel data, int aitisiId)
        {
            bool val1 = (System.Web.HttpContext.Current.User != null) && System.Web.HttpContext.Current.User.Identity.IsAuthenticated;
            if (!val1)
            {
                ViewBag.loggedUser = "(χωρίς σύνδεση)";
                return RedirectToAction("Login", "USER_ADMINS");
            }
            else
            {
                loggedAdmin = GetLoginAdmin();
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
                AITISIS entity = aitisiService.EditAitisi(data, aitisiId, auditor: loggedAdmin.USERNAME);

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

        public ActionResult PrintAitisi(int aitisiId)
        {
            bool val1 = (System.Web.HttpContext.Current.User != null) && System.Web.HttpContext.Current.User.Identity.IsAuthenticated;
            if (!val1)
            {
                ViewBag.loggedUser = "(χωρίς σύνδεση)";
                return RedirectToAction("Login", "USER_ADMINS");
            }
            else
            {
                loggedAdmin = GetLoginAdmin();
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

        #endregion


        #region ΣΧΟΛΙΚΕΣ ΜΟΝΑΔΕΣ ΕΠΙΛΟΓΗΣ

        public ActionResult ListSchools(int aitisiId)
        {
            //check if user is unauthenticated to redirect him
            bool val1 = (System.Web.HttpContext.Current.User != null) && System.Web.HttpContext.Current.User.Identity.IsAuthenticated;
            if (!val1)
            {
                ViewBag.loggedUser = "(χωρίς σύνδεση)";
                return RedirectToAction("Login", "USER_ADMINS");
            }
            else
            {
                loggedAdmin = GetLoginAdmin();
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
                    string msg = "Η επιλεγμένη αίτηση έχει ασυμπλήρωτα στοιχεία.Πατήστε Επεξεργασία πρώτα και συμπληρώστε την αίτηση.";
                    return RedirectToAction("ListAitiseis", "AdminAitiseis", new { notify = msg });
                }
                else
                {
                    PopulateViewBagWithAitisi(selectedAitisi);
                    PopulateSchoolTypes(selectedAitisi.AITISI_ID);
                    PopulateSchools(selectedAitisi.AITISI_ID);
                    List<AITISI_SCHOOLSViewModel> data = aitisiSchoolsService.Read(selectedAitisi.AITISI_ID);
                    if (data.Count == 0)
                    {
                        data = new List<AITISI_SCHOOLSViewModel>();
                    }
                    return View(data);
                }
            }
            else return RedirectToAction("ListAitiseis", "AdminAitiseis");            
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


        #region ΑΙΤΗΣΕΙΣ - ΠΡΟΫΠΗΡΕΣΙΕΣ

        public ActionResult MainAdmin(int aitisiID = 0, string notify = null)
        {
            bool val1 = (System.Web.HttpContext.Current.User != null) && System.Web.HttpContext.Current.User.Identity.IsAuthenticated;
            if (!val1)
            {
                ViewBag.loggedUser = "(χωρίς σύνδεση)";
                return RedirectToAction("Login", "USER_ADMINS");
            }
            else
            {
                loggedAdmin = GetLoginAdmin();
            }
            int prokirixiId = Common.GetAdminProkirixiID();
            if (prokirixiId == 0)
            {
                string msg = "Δεν βρέθηκε διαχειριστική Προκήρυξη.";
                return RedirectToAction("Index", "Admin", new { notify = msg });
            }

            int schoolYearId = (int)Common.GetAdminProkirixi().SCHOOL_YEAR;
            ViewData["prokirixiProtocol"] = Common.GetAdminProkirixi().PROTOCOL;
            ViewData["schoolYearText"] = Common.GetSchoolYearText(schoolYearId);
            
            // set Aitisi Info header
            sqlTEACHER_AITISEIS SelectedAitisi = GetSelectedAitisi(aitisiID);

            if (SelectedAitisi == null)
            {
                SelectedAitisi = GetSelectedAitisi();
                // if this too is null then there are no aitiseis
                if (SelectedAitisi == null)
                {
                    string msg = "Δεν υπάρχουν αιτήσεις για την προκήρυξη αυτή";
                    return RedirectToAction("Index", "Admin", new { notify = msg });
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

        public ActionResult Aitiseis_Read([DataSourceRequest] DataSourceRequest request, int aitisiId = 0)
        {
            int prokirixiId = Common.GetAdminProkirixiID();

            IEnumerable<sqlTeacherAitiseisModel> data = ReadAitiseis(prokirixiId);

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

        public ActionResult BatchTransferExperiences()
        {
            int prokirixiId = Common.GetAdminProkirixiID();

            string msg;
            if (!(prokirixiId > 0))
            {
                msg = "Δεν βρέθηκε διαχειριστική προκήρυξη.";
                return Json(msg, JsonRequestBehavior.AllowGet);
            }
            IEnumerable<sqlTeacherAitiseisModel> data = ReadAitiseis(prokirixiId);

            if (data.Count() > 0)
            {
                foreach (var item in data)
                {
                    Transfer.TransferAllWorkExperience(item.AITISI_ID);
                }
            }
            else
            {
                msg = "Δεν μπορεί να γίνει μεταφορά διότι δεν βρέθηκαν αιτήσεις για την προκήρυξη αυτή.";
                return Json(msg, JsonRequestBehavior.AllowGet);
            }
            msg = "Οι προηγούμενες προϋπηρεσίες όλων των αιτήσεων μεταφέρθηκαν και μοριοδοτήθηκαν, σύμφωνα με τη νέα υπουργική.";
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
                return RedirectToAction("Login", "USER_ADMINS");
            }
            else
            {
                loggedAdmin = GetLoginAdmin();
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
                return RedirectToAction("Login", "USER_ADMINS");
            }
            else
            {
                loggedAdmin = GetLoginAdmin();
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

        public ActionResult Supernova()
        {
            int prokirixiId = Common.GetAdminProkirixiID();
            if (prokirixiId == 0)
            {
                string msg = "Δεν βρέθηκε διαχειριστική Προκήρυξη.";
                return RedirectToAction("MainAdmin", "AdminAtiseis", new { notify = msg });
            }

            var aitiseis = (from a in db.AITISIS where a.PROKIRIXI_ID == prokirixiId select a).ToList();

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
            return Json(message,JsonRequestBehavior.AllowGet);
        }

        #endregion


        #endregion


        #region ΕΠΙΜΟΡΦΩΣΕΙΣ (ΝΕΟ 26-04-2019)

        public ActionResult Reeducations(int aitisiId = 0, string notify = null)
        {
            bool val1 = (System.Web.HttpContext.Current.User != null) && System.Web.HttpContext.Current.User.Identity.IsAuthenticated;
            if (!val1)
            {
                ViewBag.loggedUser = "(χωρίς σύνδεση)";
                return RedirectToAction("Login", "USER_ADMINS");
            }
            else
            {
                loggedAdmin = GetLoginAdmin();
            }
            // set Aitisi Info header
            sqlTEACHER_AITISEIS SelectedAitisi = GetSelectedAitisi(aitisiId);
            if (SelectedAitisi == null)
            {
                string errMsg = "Δεν υπάρχει επιλεγμένη αίτηση. Κλείστε την καρτέλα και δοκιμάστε πάλι.";
                return RedirectToAction("MainAdmin", "AdminAitiseis", new { notify = errMsg });
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

            return RedirectToAction("Reeducations", "AdminAitiseis", new { aitisiID = aitisiId, notify = msg });
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
                return RedirectToAction("Login", "USER_ADMINS");
            }
            else
            {
                loggedAdmin = GetLoginAdmin();
            }
            // set Aitisi Info header
            sqlTEACHER_AITISEIS SelectedAitisi = GetSelectedAitisi(aitisiId);
            if (SelectedAitisi == null)
            {
                string errMsg = "Δεν υπάρχει επιλεγμένη αίτηση. Κλείστε την καρτέλα και δοκιμάστε πάλι.";
                return RedirectToAction("MainAdmin", "AdminAitiseis", new { notify = errMsg });
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
                return RedirectToAction("Login", "USER_ADMINS");
            }
            else
            {
                loggedAdmin = GetLoginAdmin();
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

            if (notify != null)  this.ShowMessage(MessageType.Warning, notify);

            return View();
        }

        public ActionResult AitiseisUploads_Read([DataSourceRequest] DataSourceRequest request, int prokirixiId = 0)
        {
            IEnumerable<sqlTeacherAitiseisModel> data = ReadAitiseis(prokirixiId);

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