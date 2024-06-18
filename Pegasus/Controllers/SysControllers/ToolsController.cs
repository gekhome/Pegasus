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
using Pegasus.DAL;
using Pegasus.Models;
using Pegasus.BPM;
using Pegasus.Filters;
using Pegasus.Notification;
using Pegasus.Services;

namespace Pegasus.Controllers.SysControllers
{
    [ErrorHandlerFilter]
    public class ToolsController : Controller
    {
        private readonly PegasusDBEntities db;
        private USER_ADMINS loggedAdmin;

        private readonly ISchoolYearService schoolYearService;
        private readonly IEidikotitesService eidikotitesService;
        private readonly IKladosUnifiedService kladosUnifiedService;
        private readonly IGroupsService groupsService;
        private readonly IEidikotitesProkirixiService eidikotitesProkirixiService;
        private readonly IApokleismoiService apokleismoiService;
        private readonly ITaxfreeService taxfreeService;
        private readonly IProkirixiService prokirixiService;

        public ToolsController(PegasusDBEntities entities, ISchoolYearService schoolYearService, IEidikotitesService eidikotitesService,
            IKladosUnifiedService kladosUnifiedService, IGroupsService groupsService, IEidikotitesProkirixiService eidikotitesProkirixiService,
            IApokleismoiService apokleismoiService, ITaxfreeService taxfreeService, IProkirixiService prokirixiService)
        {
            db = entities;

            this.schoolYearService = schoolYearService;
            this.eidikotitesService = eidikotitesService;
            this.kladosUnifiedService = kladosUnifiedService;
            this.groupsService = groupsService;
            this.eidikotitesProkirixiService = eidikotitesProkirixiService;
            this.apokleismoiService = apokleismoiService;
            this.taxfreeService = taxfreeService;
            this.prokirixiService = prokirixiService;
        }


        #region ΣΧΟΛΙΚΑ ΕΤΗ

        public ActionResult SchoolYearsList()
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

            return View();
        }

        [HttpPost]
        public ActionResult SchoolYear_Read([DataSourceRequest] DataSourceRequest request)
        {
            IEnumerable<SchoolYearsViewModel> data = schoolYearService.Read();

            return Json(data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SchoolYear_Create([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<SchoolYearsViewModel> data)
        {
            var results = new List<SchoolYearsViewModel>();

            foreach (var item in data)
            {
                var existingSchoolYears = db.SYS_SCHOOLYEARS.Where(d => d.SY_TEXT == item.SY_TEXT).Count();
                if (existingSchoolYears > 0)
                    ModelState.AddModelError("", "Αυτό το σχολικό έτος είναι υπάρχει ήδη. Η καταχώρηση ακυρώθηκε.");

                if (item != null && ModelState.IsValid)
                {
                    schoolYearService.Create(item);
                    results.Add(item);
                }
            }
            return Json(results.ToDataSourceResult(request, ModelState), JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SchoolYear_Update([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<SchoolYearsViewModel> data)
        {
            foreach (var item in data)
            {
                if (item != null & ModelState.IsValid)
                {
                    schoolYearService.Update(item);
                }
            }
            return Json(data.ToDataSourceResult(request, ModelState), JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SchoolYear_Destroy([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<SchoolYearsViewModel> data)
        {
            if (data.Any())
            {
                foreach (var item in data)
                {
                    if (!Common.CanDeleteSchoolYear(item.SY_ID))
                        ModelState.AddModelError("", "Δεν μπορεί να διαγραφεί αυτό το σχολικό έτος διότι είναι σε χρήση.");

                    if (ModelState.IsValid)
                    {
                        schoolYearService.Destroy(item);
                    }
                }
            }
            return Json(data.ToDataSourceResult(request, ModelState), JsonRequestBehavior.AllowGet);
        }

        #endregion


        #region ΕΙΔΙΚΟΤΗΤΕΣ ΝΕΟ (11-7-2018)

        public ActionResult EidikotitesList()
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
            PopulateKladoi();
            PopulateKladoiUnified();
            PopulateClasses();

            return View();
        }

        [HttpPost]
        public ActionResult Eidikotita_Read([DataSourceRequest] DataSourceRequest request)
        {
            IEnumerable<EidikotitesViewModel> data = eidikotitesService.Read();

            return Json(data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Eidikotita_Create([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<EidikotitesViewModel> data)
        {
            var results = new List<EidikotitesViewModel>();

            foreach (var item in data)
            {
                var existingEidikotites = db.SYS_EIDIKOTITES.Where(d => d.EIDIKOTITA_NAME == item.EIDIKOTITA_NAME && d.EIDIKOTITA_CODE == item.EIDIKOTITA_CODE).Count();
                if (existingEidikotites > 0)
                {
                    ModelState.AddModelError("", "Η εδικότητα αυτή είναι ήδη καταχωρημένη. Η αποθήκευση ακυρώθηκε.");
                }
                if (item != null && ModelState.IsValid)
                {
                    eidikotitesService.Create(item);
                    results.Add(item);
                }
            }
            return Json(results.ToDataSourceResult(request, ModelState), JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Eidikotita_Update([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<EidikotitesViewModel> data)
        {
            foreach (var item in data)
            {
                if (item != null)
                {
                    eidikotitesService.Update(item);
                }
            }
            return Json(data.ToDataSourceResult(request, ModelState), JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Eidikotita_Destroy([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<EidikotitesViewModel> data)
        {
            if (data.Any())
            {
                foreach (var item in data)
                {
                    if (!Common.CanDeleteEidikotita(item.EIDIKOTITA_ID))
                        ModelState.AddModelError("", "Δεν μπορεί να διαγραφεί η ειδικότητα αυτή διότι είναι σε χρήση.");

                    if (ModelState.IsValid)
                    {
                        eidikotitesService.Destroy(item);
                    }
                }
            }
            return Json(data.ToDataSourceResult(request, ModelState), JsonRequestBehavior.AllowGet);
        }

        #endregion


        #region ΕΝΙΑΙΟΙ ΚΛΑΔΟΙ ΚΑΙ ΕΝΤΑΞΗ ΕΙΔΙΚΟΤΗΤΩΝ ΣΕ ΑΥΤΟΥΣ

        public ActionResult KladosUnifiedList()
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

            PopulateKlados();
            PopulateSqlEidikotites2();
            return View();
        }

        [HttpPost]
        public ActionResult KladosUnified_Read([DataSourceRequest] DataSourceRequest request)
        {
            IEnumerable<KladosUnifiedViewModel> data = kladosUnifiedService.Read();

            return Json(data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult KladosUnified_Create([DataSourceRequest] DataSourceRequest request, KladosUnifiedViewModel data)
        {
            var newdata = new KladosUnifiedViewModel();

            var existingKlados = db.SYS_KLADOS_ENIAIOS.Where(d => d.ΚΛΑΔΟΣ_ΕΝΙΑΙΟΣ == data.ΚΛΑΔΟΣ_ΕΝΙΑΙΟΣ).Count();
            if (existingKlados > 0)
            {
                ModelState.AddModelError("", "Αυτός ο ενιαίος κλάδος είναι ήδη καταχωρημένος. Η αποθήκευση ακυρώθηκε.");
            }
            if (data != null && ModelState.IsValid)
            {
                kladosUnifiedService.Create(data);
                newdata = kladosUnifiedService.Refresh(data.ΚΛΑΔΟΣ_ΚΩΔ);
            }
            return Json(new[] { newdata }.ToDataSourceResult(request, ModelState), JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult KladosUnified_Update([DataSourceRequest] DataSourceRequest request, KladosUnifiedViewModel data)
        {
            var newdata = new KladosUnifiedViewModel();

            if (data != null && ModelState.IsValid)
            {
                kladosUnifiedService.Update(data);
                newdata = kladosUnifiedService.Refresh(data.ΚΛΑΔΟΣ_ΚΩΔ);
            }
            return Json(new[] { newdata }.ToDataSourceResult(request, ModelState), JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult KladosUnified_Destroy([DataSourceRequest] DataSourceRequest request, KladosUnifiedViewModel data)
        {
            if (data != null)
            {
                if (!Common.CanDeleteKladosUnified(data.ΚΛΑΔΟΣ_ΚΩΔ))
                    ModelState.AddModelError("", "Ο κλάδος αυτός δεν μπορεί να διαγραφεί διότι έχει συσχετισμένες ειδικότητες.");

                if (ModelState.IsValid)
                {
                    kladosUnifiedService.Destroy(data);
                }
            }
            return Json(new[] { data }.ToDataSourceResult(request, ModelState), JsonRequestBehavior.AllowGet);
        }

        public ActionResult KladosUnifiedPrint()
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
                return View();
            }
        }


        #region ΠΛΕΓΜΑ ΕΙΔΙΚΟΤΗΤΩΝ ΕΝΙΑΙΩΝ ΚΛΑΔΩΝ

        public ActionResult sqlEidikotitaKU_Read([DataSourceRequest] DataSourceRequest request, int kladosunifiedId = 0)
        {
            List<sqlEidikotitesKUViewModel> data = kladosUnifiedService.GetEidikotites(kladosunifiedId);

            return Json(data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult sqlEidikotitaKU_Set([DataSourceRequest] DataSourceRequest request, sqlEidikotitesKUViewModel data, int kladosunifiedId = 0)
        {
            var newdata = new sqlEidikotitesKUViewModel();

            if (kladosunifiedId > 0)
            {
                if (data != null)
                {
                    kladosUnifiedService.SetEidikotita(data, kladosunifiedId);
                    newdata = kladosUnifiedService.RefreshEidikotita(data.EIDIKOTITA_ID);
                }
            }
            else
            {
                ModelState.AddModelError("", "Πρέπει πρώτα να επιλέξετε έναν κλάδο ενοποίησης. Η ενημέρωση ακυρώθηκε.");
            }
            return Json(new[] { newdata }.ToDataSourceResult(request, ModelState), JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult sqlEidikotitaKU_Reset([DataSourceRequest] DataSourceRequest request, sqlEidikotitesKUViewModel data, int kladosunifiedId = 0)
        {
            var newdata = new sqlEidikotitesKUViewModel();

            if (kladosunifiedId > 0)
            {
                if (data != null)
                {
                    kladosUnifiedService.ResetEidikotita(data);
                    newdata = kladosUnifiedService.RefreshEidikotita(data.EIDIKOTITA_ID);
                }
            }
            else
            {
                ModelState.AddModelError("", "Πρέπει πρώτα να επιλέξετε έναν κλάδο ενοποίησης. Η ενημέρωση ακυρώθηκε.");
            }
            return Json(new[] { newdata }.ToDataSourceResult(request, ModelState), JsonRequestBehavior.AllowGet);
        }

        #endregion

        #endregion


        #region ΟΜΑΔΕΣ ΕΙΔΙΚΟΤΗΤΩΝ ΚΑΙ ΕΝΤΑΞΗ ΕΙΔΙΚΟΤΗΤΩΝ ΣΕ ΟΜΑΔΕΣ

        public ActionResult GroupsList()
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

            PopulateSqlEidikotites();
            PopulateKladoi();
            return View();
        }

        [HttpPost]
        public ActionResult Group_Read([DataSourceRequest] DataSourceRequest request)
        {
            List<GroupsViewModel> data = groupsService.Read();

            return Json(data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Group_Create([DataSourceRequest] DataSourceRequest request, GroupsViewModel data)
        {
            var newdata = new GroupsViewModel();

            var existingGroups = db.SYS_EIDIKOTITES_GROUPS.Where(d => d.GROUP_TEXT == data.GROUP_TEXT).Count();
            if (existingGroups > 0)
            {
                ModelState.AddModelError("", "Η ομάδα αυτή είναι ήδη καταχωρημένη. Η αποθήκευση ακυρώθηκε.");
            }
            if (data != null && ModelState.IsValid)
            {
                groupsService.Create(data);
                newdata = groupsService.Refresh(data.GROUP_ID);
            }
            return Json(new[] { newdata }.ToDataSourceResult(request, ModelState), JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Group_Update([DataSourceRequest] DataSourceRequest request, GroupsViewModel data)
        {
            var newdata = new GroupsViewModel();

            if (data != null && ModelState.IsValid)
            {
                groupsService.Update(data);
                newdata = groupsService.Refresh(data.GROUP_ID);
            }
            return Json(new[] { newdata }.ToDataSourceResult(request, ModelState), JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Group_Destroy([DataSourceRequest] DataSourceRequest request, GroupsViewModel data)
        {
            if (data != null)
            {
                if (data.GROUP_TEXT == null)
                {
                    ModelState.AddModelError("", "Η κενή ομάδα δεν επιτρέπεται να διαγραφεί. Η διαγραφή ακυρώθηκε.");
                }
                if (ModelState.IsValid)
                {
                    groupsService.Destroy(data);
                }
            }
            return Json(new[] { data }.ToDataSourceResult(request, ModelState), JsonRequestBehavior.AllowGet);
        }
      
        public ActionResult GroupsPrint()
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
                return View();
            }
        }


        #region ΠΛΕΓΜΑ ΕΙΔΙΚΟΤΗΤΩΝ ΣΕ ΟΜΑΔΑ

        public ActionResult sqlEidikotitaGroup_Read([DataSourceRequest] DataSourceRequest request, int groupId = 0)
        {
            List<sqlEidikotitesSelectorViewModel> data = groupsService.GetEidikotites(groupId);

            return Json(data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult sqlEidikotitaGroup_Set([DataSourceRequest] DataSourceRequest request, sqlEidikotitesSelectorViewModel data, int groupId = 0)
        {
            var newdata = new sqlEidikotitesSelectorViewModel();

            if (groupId > 0)
            {
                if (data != null)
                {
                    groupsService.SetEidikotita(data, groupId);
                    newdata = groupsService.RefreshEidikotita(data.EIDIKOTITA_ID);
                }
            }
            else
            {
                ModelState.AddModelError("", "Πρέπει πρώτα να επιλέξετε μια ομάδα. Η ενημέρωση ακυρώθηκε.");
            }
            return Json(new[] { newdata }.ToDataSourceResult(request, ModelState), JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult sqlEidikotitaGroup_Reset([DataSourceRequest] DataSourceRequest request, sqlEidikotitesSelectorViewModel data, int groupId = 0)
        {
            var newdata = new sqlEidikotitesSelectorViewModel();

            if (groupId > 0)
            {
                if (data != null)
                {
                    groupsService.ResetEidikotita(data);
                    newdata = groupsService.RefreshEidikotita(data.EIDIKOTITA_ID);
                }
            }
            else
            {
                ModelState.AddModelError("", "Πρέπει πρώτα να επιλέξετε μια ομάδα. Η ενημέρωση ακυρώθηκε.");
            }
            return Json(new[] { newdata }.ToDataSourceResult(request, ModelState), JsonRequestBehavior.AllowGet);
        }

        #endregion

        #endregion


        #region ΕΝΗΜΕΡΩΣΗ ΟΜΑΔΩΝ ΕΙΔΙΚΟΤΗΤΩΝ

        // Used by Editor template for GROUP_ID column
        public ActionResult GroupsRead()
        {
            List<GroupsViewModel> data = groupsService.Read();

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult EidikotitesGroups()
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

            PopulateKladoiUnified();
            PopulateGroups();

            return View();
        }

        public ActionResult EidikotitaGroup_Read([DataSourceRequest] DataSourceRequest request)
        {
            IEnumerable<EidikotitesViewModel> data = eidikotitesService.Read();

            return Json(data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        public ActionResult EidikotitaGroup_Update([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<EidikotitesViewModel> data)
        {
            var results = new List<EidikotitesViewModel>();

            foreach (var item in data)
            {
                if (item != null)
                {
                    eidikotitesService.UpdateGroup(item);
                }
            }
            return Json(results.ToDataSourceResult(request, ModelState), JsonRequestBehavior.AllowGet);
        }

        #endregion


        #region ΠΡΟΚΗΡΥΣΣΟΜΕΝΕΣ ΕΙΔΙΚΟΤΗΤΕΣ

        public ActionResult EidikotitesInProkirixi(string notify = null)
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
            if (notify != null) this.ShowMessage(MessageType.Info, notify);

            int schoolYearId = (int)Common.GetAdminProkirixi().SCHOOL_YEAR;
            ViewData["prokirixiProtocol"] = Common.GetAdminProkirixi().PROTOCOL;
            ViewData["schoolYearText"] = Common.GetSchoolYearText(schoolYearId);

            PopulatePeriferies();
            PopulateEidikotites();
            return View();
        }

        public ActionResult SchoolsRead([DataSourceRequest] DataSourceRequest request)
        {
            // ΕΠΙΛΟΓΗ ΜΟΝΟ ΙΕΚ
            var data = db.SYS_SCHOOLS.Where(o => o.SCHOOL_ID >= 100 && o.SCHOOL_TYPEID == 2).Select(p => new SYS_SCHOOLSViewModel()
            {
                SCHOOL_ID = p.SCHOOL_ID,
                SCHOOL_NAME = p.SCHOOL_NAME,
                SCHOOL_PERIFERIA_ID = p.SCHOOL_PERIFERIA_ID
            });
            return Json(data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult EidikotitesRead([DataSourceRequest] DataSourceRequest request, int schoolId)
        {
            int prokirixiId = Common.GetAdminProkirixiID();

            List<ProkirixisEidikotitesViewModel> data = eidikotitesProkirixiService.Read(prokirixiId, schoolId);

            return Json(data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        // CRUD functions
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EidikotitesCreate([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<ProkirixisEidikotitesViewModel> data, int schoolId)
        {
            var results = new List<ProkirixisEidikotitesViewModel>();
            int prokirixiId = Common.GetAdminProkirixiID();
            if (schoolId > 0)
            {
                foreach (var item in data)
                {
                    var existingData = db.PROKIRIXIS_EIDIKOTITES.Where(d => d.PROKIRIXI_ID == prokirixiId && d.SCHOOL_ID == schoolId && d.EIDIKOTITA_ID == item.EIDIKOTITA_ID).Count();
                    if (existingData > 0)
                    {
                        ModelState.AddModelError("", "Υπάρχει ήδη η καταχώρηση αυτή. Η αποθήκευση ακυρώθηκε.");
                    }
                    if (item != null && ModelState.IsValid)
                    {
                        eidikotitesProkirixiService.Create(item, prokirixiId, schoolId);
                        results.Add(item);
                    }
                }
            }
            else
            {
                ModelState.AddModelError("", "Δεν έχει γίνει επιλογή σχολείου. Η αποθήκευση ακυρώθηκε.");
            }
            return Json(results.ToDataSourceResult(request, ModelState), JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EidikotitesUpdate([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<ProkirixisEidikotitesViewModel> data, int schoolId)
        {
            int prokirixiId = Common.GetAdminProkirixiID();
            if (schoolId > 0)
            {
                foreach (var item in data)
                {
                    if (item != null && ModelState.IsValid)
                    {
                        eidikotitesProkirixiService.Update(item, prokirixiId, schoolId);
                    }
                }
            }
            else
            {
                ModelState.AddModelError("", "Δεν έχει γίνει επιλογή σχολείου. Η αποθήκευση ακυρώθηκε.");
            }
            return Json(data.ToDataSourceResult(request, ModelState), JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EidikotitesDestroy([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<ProkirixisEidikotitesViewModel> data)
        {
            if (data.Any())
            {
                foreach (var item in data)
                {
                    eidikotitesProkirixiService.Destroy(item);
                }
            }
            return Json(data.ToDataSourceResult(request, ModelState), JsonRequestBehavior.AllowGet);
        }

        public ActionResult EidikotitesProkirixiPrint()
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

            return View();
        }

        #endregion


        #region ΚΑΤΑΧΩΡΗΣΗ ΕΙΔΙΚΟΤΗΤΩΝ ΠΡΟΚΗΡΥΞΕΩΝ ΝΕΟ (8-8-2019)

        public ActionResult EidikotitesProkirixeisNew(string notify = null)
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
            if (notify != null) this.ShowMessage(MessageType.Info, notify);

            int schoolYearId = (int)Common.GetAdminProkirixi().SCHOOL_YEAR;
            ViewData["prokirixiProtocol"] = Common.GetAdminProkirixi().PROTOCOL;
            ViewData["schoolYearText"] = Common.GetSchoolYearText(schoolYearId);

            PopulateEidikotites();
            return View();
        }

        [HttpPost]
        public ActionResult EidikotitesReadNew([DataSourceRequest] DataSourceRequest request, int prokirixiId, int schoolId)
        {
            List<ProkirixisEidikotitesViewModel> data = eidikotitesProkirixiService.Read(prokirixiId, schoolId);

            return Json(data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EidikotitesCreateNew([DataSourceRequest] DataSourceRequest request,
                            [Bind(Prefix = "models")] IEnumerable<ProkirixisEidikotitesViewModel> data, int prokirixiId, int schoolId)
        {
            var results = new List<ProkirixisEidikotitesViewModel>();

            if (prokirixiId > 0 && schoolId > 0)
            {
                foreach (var item in data)
                {
                    if (Common.EidikotitaSchoolExists(item))
                    {
                        ModelState.AddModelError("", "Έγινε απόπειρα δημιουργίας διπλοεγγραφής. Η διπλή καταχώρηση ακυρώθηκε.");
                        return Json(data.ToDataSourceResult(request, ModelState), JsonRequestBehavior.AllowGet);
                    }
                    if (item != null && ModelState.IsValid)
                    {
                        eidikotitesProkirixiService.Create(item, prokirixiId, schoolId);
                        results.Add(item);
                    }
                }
            }
            else
            {
                ModelState.AddModelError("", "Δεν έχει γίνει επιλογή προκήρυξης και σχολείου. Η αποθήκευση ακυρώθηκε.");
            }
            return Json(results.ToDataSourceResult(request, ModelState), JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EidikotitesUpdateNew([DataSourceRequest] DataSourceRequest request,
                            [Bind(Prefix = "models")]IEnumerable<ProkirixisEidikotitesViewModel> data, int prokirixiId, int schoolId)
        {
            if (prokirixiId > 0 && schoolId > 0)
            {
                foreach (var item in data)
                {
                    if (item != null && ModelState.IsValid)
                    {
                        eidikotitesProkirixiService.Update(item, prokirixiId, schoolId);
                    }
                }
            }
            else
            {
                ModelState.AddModelError("", "Δεν έχει γίνει επιλογή προκήρυξης και σχολείου. Η αποθήκευση ακυρώθηκε.");
            }
            return Json(data.ToDataSourceResult(request, ModelState), JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EidikotitesDestroyNew([DataSourceRequest] DataSourceRequest request,
                            [Bind(Prefix = "models")]IEnumerable<ProkirixisEidikotitesViewModel> data)
        {
            if (data.Any())
            {
                foreach (var item in data)
                {
                    if (item != null)
                    {
                        if (Common.CanDeleteEidikotitaInProkirixi(item))
                        {
                            eidikotitesProkirixiService.Destroy(item);
                        }
                        else
                        {
                            ModelState.AddModelError("", "Δεν μπορεί να διαγραφεί ειδικότητα προηγούμενης προκήρυξης.");
                        }
                    }
                }
            }
            return Json(data.ToDataSourceResult(request, ModelState), JsonRequestBehavior.AllowGet);
        }

        #endregion


        #region ΕΥΡΕΣΗ ΣΧΟΛΩΝ ΠΡΟΚΗΡΥΣΣΟΜΕΝΩΝ ΕΙΔΙΚΟΤΗΤΩΝ ΝΕΟ (25-8-2019)

        public ActionResult EidikotitesInSchools(string notify = null)
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
            if (notify != null) this.ShowMessage(MessageType.Info, notify);

            int schoolYearId = (int)Common.GetAdminProkirixi().SCHOOL_YEAR;
            ViewData["prokirixiProtocol"] = Common.GetAdminProkirixi().PROTOCOL;
            ViewData["schoolYearText"] = Common.GetSchoolYearText(schoolYearId);

            PopulateGroups();
            return View();
        }

        [HttpPost]
        public ActionResult EidikotitesInSchools_Read([DataSourceRequest] DataSourceRequest request, int prokirixiId = 0, int eidikotitaId = 0)
        {
            var data = GetEidikotitesSchoolsFromDB(prokirixiId, eidikotitaId);

            return Json(data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        public List<EidikotitesInSchoolsViewModel> GetEidikotitesSchoolsFromDB(int prokirixiId = 0, int eidikotitaId = 0)
        {
            var data = (from d in db.sqlEIDIKOTITES_IN_SCHOOLS
                        where d.PROKIRIXI_ID == prokirixiId && d.EIDIKOTITA_ID == eidikotitaId
                        orderby d.SCHOOL_NAME
                        select new EidikotitesInSchoolsViewModel
                        {
                            PSE_ID = d.PSE_ID,
                            PROKIRIXI_ID = d.PROKIRIXI_ID,
                            SCHOOL_ID = d.SCHOOL_ID,
                            EIDIKOTITA_ID = d.EIDIKOTITA_ID,
                            SCHOOL_NAME = d.SCHOOL_NAME,
                            PERIFERIA_NAME = d.PERIFERIA_NAME,
                            PERIFERIAKI = d.PERIFERIAKI
                        }).ToList();
            return data;
        }

        #endregion


        #region ΑΙΤΙΕΣ ΑΠΟΚΛΕΙΣΜΟΥ

        public ActionResult ApokleismoiList()
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

            return View();
        }

        [HttpPost]
        public ActionResult Apokleismos_Read([DataSourceRequest] DataSourceRequest request)
        {
            List<ApokleismoiViewModel> data = apokleismoiService.Read();

            return Json(data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Apokleismos_Create([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<ApokleismoiViewModel> data)
        {
            var results = new List<ApokleismoiViewModel>();

            foreach (var item in data)
            {
                var existingApoklismoi = db.SYS_APOKLEISMOI.Where(s => s.APOKLEISMOS_TEXT == item.APOKLEISMOS_TEXT).Count();
                if (existingApoklismoi > 0)
                    ModelState.AddModelError("", "Η καταχώρηση αυτή υπάρχει ήδη. Η αποθήκευση ακυρώθηκε.");

                if (ModelState.IsValid)
                {
                    apokleismoiService.Create(item);
                    results.Add(item);
                }
            }
            return Json(results.ToDataSourceResult(request, ModelState), JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Apokleismos_Update([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<ApokleismoiViewModel> data)
        {
            foreach (var item in data)
            {
                if (item != null && ModelState.IsValid)
                {
                    apokleismoiService.Update(item);
                }
            }
            return Json(data.ToDataSourceResult(request, ModelState), JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Apokleismos_Destroy([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<ApokleismoiViewModel> data)
        {
            if (data.Any())
            {
                foreach (var item in data)
                {
                    apokleismoiService.Destroy(item);
                }
            }
            return Json(data.ToDataSourceResult(request, ModelState), JsonRequestBehavior.AllowGet);
        }

        #endregion


        #region ΑΦΟΡΟΛΟΓΗΤΑ ΕΙΣΟΔΗΜΑΤΑ

        public ActionResult TaxfreeList()
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
            return View();
        }

        [HttpPost]
        public ActionResult Taxfree_Read([DataSourceRequest] DataSourceRequest request)
        {
            List<TaxFreeViewModel> data = taxfreeService.Read();

            return Json(data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Taxfree_Create([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<TaxFreeViewModel> data)
        {
            List<TaxFreeViewModel> results = new List<TaxFreeViewModel>();

            foreach (var item in data)
            {
                var existingTaxFree = db.SYS_TAXFREE.Where(d => d.YEAR_TEXT == item.YEAR_TEXT).Count();
                if (existingTaxFree > 0)
                    ModelState.AddModelError("", "Η καταχώρηση αυτή υπάρχει ήδη. Η αποθήκευση ακυρώθηκε.");

                if (item != null && ModelState.IsValid)
                {
                    taxfreeService.Create(item);
                    results.Add(item);
                }
            }
            return Json(results.ToDataSourceResult(request, ModelState), JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Taxfree_Update([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<TaxFreeViewModel> data)
        {
            foreach (var item in data)
            {
                if (item != null && ModelState.IsValid)
                {
                    taxfreeService.Update(item);
                }
            }
            return Json(data.ToDataSourceResult(request, ModelState), JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Taxfree_Destroy([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<TaxFreeViewModel> data)
        {
            if (data.Any())
            {
                foreach (var item in data)
                {
                    taxfreeService.Destroy(item);
                }
            }
            return Json(data.ToDataSourceResult(request, ModelState), JsonRequestBehavior.AllowGet);
        }

        public ActionResult TaxFreeIncomePrint()
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

            return View();
        }

        #endregion


        #region ΠΡΟΚΗΡΥΞΕΙΣ

        public ActionResult ProkirixisList()
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

            PopulateSchoolYears();
            PopulateStatus();

            return View();
        }

        [HttpPost]
        public ActionResult Prokirixis_Read([DataSourceRequest] DataSourceRequest request)
        {
            List<ProkirixisViewModel> data = prokirixiService.Read();

            return Json(data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Prokirixis_Create([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<ProkirixisViewModel> data)
        {
            List<ProkirixisViewModel> results = new List<ProkirixisViewModel>();

            foreach (var item in data)
            {
                var existingProkirixi = db.PROKIRIXIS.Where(s => s.PROTOCOL == item.PROTOCOL).Count();
                if (existingProkirixi > 0)
                    ModelState.AddModelError("", "Η προκήρυξη αυτή είναι ήδη καταχωρημένη. Η αποθήκευση ακυρώθηκε.");

                if (item != null && ModelState.IsValid)
                {
                    prokirixiService.Create(item);
                    results.Add(item);
                }
            }
            return Json(results.ToDataSourceResult(request, ModelState), JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Prokirixis_Update([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<ProkirixisViewModel> data)
        {
            foreach (var item in data)
            {
                if (item != null && ModelState.IsValid)
                {
                    prokirixiService.Update(item);
                }
            }
            return Json(data.ToDataSourceResult(request, ModelState), JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Prokirixis_Destroy([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<ProkirixisViewModel> data)
        {
            if (data.Any())
            {
                foreach (var item in data)
                {
                    if (Common.CanDeleteProkirixi(item.ID))
                    {
                        prokirixiService.Destroy(item);
                    }
                    else
                    {
                        ModelState.AddModelError("", "Δεν μπορεί να διαγραφεί η προκήρυξη διότι υπάρχουν αιτήσεις με αυτή.");
                    }
                }
            }
            return Json(data.ToDataSourceResult(request, ModelState), JsonRequestBehavior.AllowGet);
        }

        #endregion


        #region ΠΕΡΙΦΕΡΕΙΕΣ-ΔΗΜΟΙ

        public ActionResult PeriferiesDimoi()
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
            return View();
        }

        public ActionResult Periferies([DataSourceRequest] DataSourceRequest request)
        {
            var periferies = db.SYS_PERIFERIES.Select(p => new PeriferiaViewModel()
            {
                PERIFERIA_ID = p.PERIFERIA_ID,
                PERIFERIA_NAME = p.PERIFERIA_NAME
            });
            return Json(periferies.ToDataSourceResult(request));
        }

        public ActionResult Dimoi([DataSourceRequest] DataSourceRequest request, int periferiaId)
        {
            var dimoi = db.SYS_DIMOS.Where(o => o.DIMOS_PERIFERIA == periferiaId).Select(p => new DimosViewModel()
            {
                DIMOS_ID = p.DIMOS_ID,
                DIMOS = p.DIMOS,
                DIMOS_PERIFERIA = p.DIMOS_PERIFERIA
            });
            return Json(dimoi.ToDataSourceResult(request));
        }

        public ActionResult PeriferiesPrint()
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
            return View();
        }

        #endregion


        #region GETTERS, POPULATORS

        public void PopulateSchoolYears()
        {
            var syears = (from d in db.SYS_SCHOOLYEARS
                          orderby d.SY_TEXT descending
                          select d).ToList();

            ViewData["SchoolYears"] = syears;
        }

        public void PopulateStatus()
        {
            var status = (from s in db.SYS_PROKIRIXI_STATUS
                          select s).ToList();

            ViewData["Status"] = status;
        }

        public void PopulateEidikotites()
        {
            var pdata = (from d in db.VD_EIDIKOTITES
                         orderby d.EIDIKOTITA_KLADOS_ID, d.EIDIKOTITA_DESC
                         select d).ToList();

            ViewData["Eidikotites"] = pdata;
            ViewData["defaultEidikotita"] = pdata.First().EIDIKOTITA_ID;
        }

        public void PopulatePeriferies()
        {
            var Periferies = (from d in db.SYS_PERIFERIES select d).ToList();

            ViewData["periferies"] = Periferies;
        }

        public void PopulateSqlEidikotites()
        {
            var data = (from d in db.sqlEIDIKOTITES_SELECTOR
                        orderby d.EIDIKOTITA_KLADOS_ID, d.EIDIKOTITA_DESC
                        select d).ToList();

            ViewData["sqlEidikotites"] = data;
            ViewData["sqlDefaultEidikotita"] = data.First().EIDIKOTITA_ID;
        }

        public void PopulateSqlEidikotites2()
        {
            var data = (from d in db.sqlEIDIKOTITES_KU
                        orderby d.EIDIKOTITA_KLADOS_ID, d.EIDIKOTITA_PTYXIO
                        select d).ToList();

            ViewData["sqlEidikotites2"] = data;
            ViewData["sqlDefaultEidikotita2"] = data.First().EIDIKOTITA_ID;
        }

        public void PopulateKladoiUnified()
        {
            var data = (from k in db.SYS_KLADOS_ENIAIOS orderby k.ΚΛΑΔΟΣ, k.ΚΛΑΔΟΣ_ΕΝΙΑΙΟΣ select k).ToList();

            ViewData["kladoiUnified"] = data;
            ViewData["defaultKladosUnified"] = data.First().ΚΛΑΔΟΣ_ΚΩΔ;
        }

        public void PopulateKladoi()
        {
            var kladosTypes = (from d in db.SYS_KLADOS select d).ToList();

            ViewData["kladoi"] = kladosTypes;
        }

        public void PopulateGroups()
        {
            var groups = (from g in db.SYS_EIDIKOTITES_GROUPS select g).ToList();
            ViewData["groups"] = groups;
        }

        public void PopulateClasses()
        {
            var edclasses = (from e in db.SYS_EDUCLASSES select e).ToList();
            ViewData["edclass"] = edclasses;
        }

        public void PopulateKlados()
        {
            var data = (from d in db.SYS_KLADOS orderby d.KLADOS_ID select d).ToList();

            ViewData["kladoi"] = data;
            ViewData["kladosDefault"] = data.First().KLADOS_ID;
        }

        public USER_ADMINS GetLoginAdmin()
        {
            loggedAdmin = db.USER_ADMINS.Where(u => u.USERNAME == System.Web.HttpContext.Current.User.Identity.Name).FirstOrDefault();

            ViewBag.loggedAdmin = loggedAdmin;
            ViewBag.loggedUser = loggedAdmin.FULLNAME;

            return loggedAdmin;
        }

        public ActionResult GetProkirixeis()
        {
            var data = (from d in db.PROKIRIXIS
                        orderby d.SCHOOL_YEAR descending, d.DATE_START descending
                        select new ProkirixisViewModel
                        {
                            ID = d.ID,
                            PROTOCOL = d.PROTOCOL
                        }).ToList();

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetSchoolsIek()
        {
            var data = (from d in db.SYS_SCHOOLS
                        where d.SCHOOL_TYPEID == 2
                        orderby d.SCHOOL_NAME
                        select new SYS_SCHOOLSViewModel
                        {
                            SCHOOL_ID = d.SCHOOL_ID,
                            SCHOOL_NAME = d.SCHOOL_NAME,
                            SCHOOL_PERIFERIAKI_ID = d.SCHOOL_PERIFERIAKI_ID
                        }).ToList();

            return Json(data, JsonRequestBehavior.AllowGet);
        }


        #endregion


        #region ADDTIONAL TOOLS

        public ActionResult CopyEidikotitesInProkirixi(int sourceProkirixiID)
        {
            int currentProkirixiID = Common.GetAdminProkirixiID();
            string msg = "";

            if (!(sourceProkirixiID > 0))
            {
                msg = "Δεν έχει γίνει επιλογή προηγούμενης προκήρυξης για μεταφορά!";
                return Json(msg, JsonRequestBehavior.AllowGet);
            }

            var source = (from s in db.PROKIRIXIS_EIDIKOTITES where s.PROKIRIXI_ID == sourceProkirixiID select s).ToList();
            var target = (from t in db.PROKIRIXIS_EIDIKOTITES where t.PROKIRIXI_ID == currentProkirixiID select t).ToList();
            if (target.Count > 0)
            {
                msg = "Στην τρέχουσα προκήρυξη έχουν ήδη καταχωρηθεί ειδικότητες!";
                return Json(msg, JsonRequestBehavior.AllowGet);
            }
            if (source.Count == 0)
            {
                msg = "Δεν βρέθηκαν ειδικότητες της προηγούμενης προκήρυξης!";
                return Json(msg, JsonRequestBehavior.AllowGet);
            }

            foreach (var p in source)
            {
                PROKIRIXIS_EIDIKOTITES newdata = new PROKIRIXIS_EIDIKOTITES();

                newdata.PROKIRIXI_ID = currentProkirixiID;
                newdata.SCHOOL_ID = p.SCHOOL_ID;
                newdata.EIDIKOTITA_ID = p.EIDIKOTITA_ID;

                db.PROKIRIXIS_EIDIKOTITES.Add(newdata);
                db.SaveChanges();
            }
            msg = "Η διαδικασία αντιγραφής ειδικοτήτων από προηγούμενη προκήρυξη ολοκληρώθηκε.";
            return Json(msg, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UpdateAitisiEidikotitaGroup()
        {
            var data = (from d in db.AITISIS select new { d.AITISI_ID, d.EIDIKOTITA, d.EIDIKOTITA_GROUP }).ToList();
            if (data.Count > 0)
            {
                foreach (var aitisi in data)
                {
                    AITISIS target = db.AITISIS.Find(aitisi.AITISI_ID);
                    target.EIDIKOTITA_GROUP = Common.GetEidikotitaGroupId((int)target.EIDIKOTITA);
                    db.Entry(target).State = EntityState.Modified;
                    db.SaveChanges();
                }
                string message = "Η διαδικασία ενημέρωσης ομάδων ειδικοτήτων των αιτήσεων ολοκληρώθηκε.";
                return Json(message, JsonRequestBehavior.AllowGet);
            }
            string message2 = "Δεν βρέθηκαν αιτήσεις που χρειάζονται ενημέρωση.";
            return Json(message2, JsonRequestBehavior.AllowGet);
        }

        #endregion

    }
}