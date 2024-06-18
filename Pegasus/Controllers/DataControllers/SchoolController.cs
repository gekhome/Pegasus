using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Text;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using Pegasus.Filters;
using Pegasus.DAL;
using Pegasus.Models;
using Pegasus.BPM;
using Pegasus.Notification;
using Pegasus.Services;


namespace Pegasus.Controllers.DataControllers
{
    [ErrorHandlerFilter]
    public class SchoolController : ControllerUnit
    {
        private readonly PegasusDBEntities db;
        private USER_SCHOOLS loggedSchool;

        private readonly ISchoolYearService schoolYearService;
        private readonly IEidikotitesService eidikotitesService;
        private readonly IKladosUnifiedService kladosUnifiedService;
        private readonly IGroupsService groupsService;
        private readonly IEidikotitesProkirixiService eidikotitesProkirixiService;
        private readonly IApokleismoiService apokleismoiService;
        private readonly ITaxfreeService taxfreeService;
        private readonly IProkirixiService prokirixiService;
        private readonly ITeacherRegistryService teacherRegistryService;
        private readonly IAitisiRegistryService aitisiRegistryService;

        public SchoolController(PegasusDBEntities entities, ISchoolYearService schoolYearService, IEidikotitesService eidikotitesService,
            IKladosUnifiedService kladosUnifiedService, IGroupsService groupsService, IEidikotitesProkirixiService eidikotitesProkirixiService,
            IApokleismoiService apokleismoiService, ITaxfreeService taxfreeService, IProkirixiService prokirixiService,
            ITeacherRegistryService teacherRegistryService, IAitisiRegistryService aitisiRegistryService) : base(entities)
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
            this.teacherRegistryService = teacherRegistryService;
            this.aitisiRegistryService = aitisiRegistryService;
        }


        public ActionResult Index(string notify = null)
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
            if (notify != null) this.ShowMessage(MessageType.Warning, notify);

            return View();
        }


        #region TOOLS (READ-ONLY)

        #region ΣΧΟΛΙΚΑ ΕΤΗ (READ-ONLY)

        public ActionResult SchoolYearsList()
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

            return View();
        }

        [HttpPost]
        public ActionResult SchoolYear_Read([DataSourceRequest] DataSourceRequest request)
        {
            IEnumerable<SchoolYearsViewModel> data = schoolYearService.Read();

            return Json(data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        #endregion


        #region ΕΙΔΙΚΟΤΗΤΕΣ (READ-ONLY)

        public ActionResult EidikotitesList()
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

            PopulateKladoi();
            PopulateKladoiUnified();
            PopulateGroups();
            PopulateClasses();

            return View();
        }

        [HttpPost]
        public ActionResult Eidikotita_Read([DataSourceRequest] DataSourceRequest request)
        {
            IEnumerable<EidikotitesViewModel> data = eidikotitesService.Read();

            return Json(data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        #endregion


        #region ΑΙΤΙΕΣ ΑΠΟΚΛΕΙΣΜΟΥ (READ-ONLY)

        public ActionResult ApokleismoiList()
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
            return View();
        }

        [HttpPost]
        public ActionResult Apokleismos_Read([DataSourceRequest] DataSourceRequest request)
        {
            List<ApokleismoiViewModel> data = apokleismoiService.Read();

            return Json(data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        #endregion


        #region ΑΦΟΡΟΛΟΓΗΤΑ ΕΙΣΟΔΗΜΑΤΑ (READ-ONLY)

        public ActionResult TaxfreeList()
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

            return View();
        }

        [HttpPost]
        public ActionResult Taxfree_Read([DataSourceRequest] DataSourceRequest request)
        {
            List<TaxFreeViewModel> data = taxfreeService.Read();

            return Json(data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        #endregion


        #region ΠΡΟΚΗΡΥΞΕΙΣ (READ-ONLY)

        public ActionResult ProkirixisList()
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

        #endregion


        #region ΕΥΡΕΣΗ ΣΧΟΛΩΝ ΠΡΟΚΗΡΥΣΣΟΜΕΝΩΝ ΕΙΔΙΚΟΤΗΤΩΝ ΝΕΟ (25-8-2019)

        public ActionResult EidikotitaGroup_Read([DataSourceRequest] DataSourceRequest request)
        {
            IEnumerable<EidikotitesViewModel> data = eidikotitesService.Read();

            return Json(data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        public ActionResult EidikotitesSchools(string notify = null)
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
            if (notify != null) this.ShowMessage(MessageType.Info, notify);

            int schoolYearId = (int)Common.GetAdminProkirixi().SCHOOL_YEAR;
            ViewData["prokirixiProtocol"] = Common.GetAdminProkirixi().PROTOCOL;
            ViewData["schoolYearText"] = Common.GetSchoolYearText(schoolYearId);

            PopulateGroups();
            return View();
        }

        [HttpPost]
        public ActionResult EidikotitesSchools_Read([DataSourceRequest] DataSourceRequest request, int prokirixiId = 0, int eidikotitaId = 0)
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


        #region ΠΡΟΚΗΡΥΣΣΟΜΕΝΕΣ ΕΙΔΙΚΟΤΗΤΕΣ (READ-ONLY)

        public ActionResult EidikotitesInProkirixi()
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
            PopulatePeriferies();
            PopulateEidikotites();
            return View();
        }

        public ActionResult SchoolsRead([DataSourceRequest] DataSourceRequest request)
        {
            var data = db.SYS_SCHOOLS.Where(o => o.SCHOOL_ID >= 100).Select(p => new SYS_SCHOOLSViewModel()
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
       
        public ActionResult EidikotitesInProkirixiPrint()
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
            return View();
        }

        #endregion


        #region ΠΕΡΙΦΕΡΕΙΕΣ-ΔΗΜΟΙ

        public ActionResult PeriferiesDimoi()
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
                return RedirectToAction("Login", "USER_SCHOOLS");
            }
            else
            {
                loggedSchool = GetLoginSchool();
            }
            return View();
        }

        #endregion

        #endregion


        #region ΜΗΤΡΩΟ ΕΚΠΑΙΔΕΥΤΩΝ

        public ActionResult TeachersRegistry()
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
            if (!(prokirixiId > 0))
            {
                string notify = "Δεν βρέθηκε διαχειριστική Προκήρυξη.";
                return RedirectToAction("Index", "School", new { notify });
            }

            IEnumerable<sqlTEACHERS_WITH_AITISEIS_UNIQUE> data = teacherRegistryService.Read();

            return View(data);
        }

        public ActionResult Teachers_Read([DataSourceRequest] DataSourceRequest request)
        {
            IEnumerable<sqlTEACHERS_WITH_AITISEIS_UNIQUE> data = teacherRegistryService.Read();

            return Json(data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        #endregion


        #region ΜΗΤΡΩΟ ΑΙΤΗΣΕΩΝ

        public ActionResult AitiseisRegistry()
        {
            bool val1 = (System.Web.HttpContext.Current.User != null) && System.Web.HttpContext.Current.User.Identity.IsAuthenticated;
            if (!val1)
            {
                return RedirectToAction("Login", "USER_SCHOOLS");
            }
            else
            {
                loggedSchool = GetLoginSchool();
            }
            int prokirixiId = Common.GetAdminProkirixiID();

            IEnumerable<sqlTEACHER_AITISEIS> data = aitisiRegistryService.Read(prokirixiId);

            PopulateTeachTypes();
            PopulateSchoolYears();
            PopulateIncomeYears();

            return View(data);
        }

        public ActionResult Aitiseis_Read([DataSourceRequest] DataSourceRequest request, int prokirixiId = 0)
        {
            IEnumerable<sqlTEACHER_AITISEIS> data = aitisiRegistryService.Read(prokirixiId);

            return Json(data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
      
        public ActionResult AitiseisRegistryPrint()
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

            return View();
        }

        public ActionResult ViewAitisi(int aitisiId)
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

            AitisisViewModel aitisi = aitisiRegistryService.GetModel(aitisiId);

            if (aitisi == null)
            {
                return HttpNotFound();
            }
            return View(aitisi);
        }
    
        public ActionResult PrintAitisiNewOld(int aitisiId)
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
                            PROKIRIXI_ID = t.PROKIRIXI_ID,
                            AITISI_ID = t.AITISI_ID,
                            AITISI_PROTOCOL = t.AITISI_PROTOCOL,
                        }).FirstOrDefault();
            return View(data);

        }

        public ActionResult ResultsMoriaView(int aitisiId)
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

            ExperienceResultsViewModel moriaResults = aitisiRegistryService.GetResults(aitisiId);

            return View(moriaResults);
        }
       
        public ActionResult ResultsMoriaPrint(int aitisiId)
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

            var data = (from d in db.AITISIS
                        where d.AITISI_ID == aitisiId
                        select new AitisisViewModel
                        {
                            AITISI_ID = d.AITISI_ID,
                            AITISI_PROTOCOL = d.AITISI_PROTOCOL,
                        }).FirstOrDefault();

            return View(data);

        }

        public ActionResult AitisiExperiencePrint(int aitisiId = 0)
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

            AitisiParameters aitisi = new AitisiParameters();
            aitisi.AITISI_ID = aitisiId;

            return View(aitisi);
        }

        #endregion


        #region ΜΗΤΡΩΟ ΑΙΤΗΣΕΩΝ - ΠΡΟΫΠΗΡΕΣΙΕΣ

        public ActionResult Teaching_Read([DataSourceRequest] DataSourceRequest request, int aitisiID = 0)
        {
            IEnumerable<ViewModelTeaching> data = aitisiRegistryService.ReadTeaching(aitisiID);

            return Json(data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        public ActionResult Vocation_Read([DataSourceRequest] DataSourceRequest request, int aitisiID = 0)
        {
            IEnumerable<ViewModelVocational> data = aitisiRegistryService.ReadVocation(aitisiID);

            return Json(data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        public ActionResult Freelance_Read([DataSourceRequest] DataSourceRequest request, int aitisiID = 0)
        {
            IEnumerable<ViewModelFreelance> data = aitisiRegistryService.ReadFreelance(aitisiID);

            return Json(data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        #endregion


        #region ΕΙΔΙΚΕΣ ΕΚΘΕΣΕΙΣ

        public ActionResult CheckInformaticsTeachers()
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
            PARAMETROI_STAT_REPORTS parameters = new PARAMETROI_STAT_REPORTS
            {
                SCHOOL_ID = loggedSchool.USER_SCHOOLID,
                PROKIRIXI_ID = Common.GetOpenProkirixiID(true)
            };
            return View(parameters);
        }

        public ActionResult TeachersEidikotitaPrint()
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

            TeacherRegistryParameters parameters = new TeacherRegistryParameters
            {
                SCHOOL_ID = loggedSchool.USER_SCHOOLID,
                PROKIRIXI_ID = prokirixiId
            };
            return View(parameters);
        }

        public ActionResult TeachersPrint()
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

            TeacherRegistryParameters parameters = new TeacherRegistryParameters
            {
                SCHOOL_ID = loggedSchool.USER_SCHOOLID,
                PROKIRIXI_ID = prokirixiId
            };
            return View(parameters);
        }

        public ActionResult EidikotitesPrint()
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
            return View();
        }

        public ActionResult EidikotitesOldNewPrint()
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
            return View();
        }

        public ActionResult MultipleAitiseis()
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
            return View();
        }

        public ActionResult DuplicateAitiseisPrint()
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
            return View();
        }
     
        public ActionResult EnstaseisDetail()
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

            EnstaseisParametroi ep = new EnstaseisParametroi
            {
                PROKIRIXI_ID = prokirixiId,
                SCHOOL_ID = (int)loggedSchool.USER_SCHOOLID
            };
            return View(ep);
        }

        public ActionResult EnstaseisDetailPost()
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

            EnstaseisParametroi ep = new EnstaseisParametroi
            {
                PROKIRIXI_ID = prokirixiId,
                SCHOOL_ID = (int)loggedSchool.USER_SCHOOLID
            };
            return View(ep);
        }

        public ActionResult EnstaseisSummary()
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
            EnstaseisParametroi ep = new EnstaseisParametroi
            {
                PROKIRIXI_ID = prokirixiId
            };
            return View(ep);
        }

        public ActionResult EnstaseisUploadCount()
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
            return View();
        }

        public ActionResult TeachersMoriaPrint()
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
            return View();
        }

        #endregion


        #region ΠΙΝΑΚΕΣ ΜΟΡΙΟΔΟΤΗΣΗΣ ΜΕ ΟΝΟΜΑΤΑ


        public ActionResult Pinakas1_School_Print()
        {
            bool val1 = (System.Web.HttpContext.Current.User != null) && System.Web.HttpContext.Current.User.Identity.IsAuthenticated;
            if (!val1)
            {
                return RedirectToAction("Login", "USER_SCHOOLS");
            }
            else
            {
                loggedSchool = GetLoginSchool();
                int prokirixiID = Common.GetAdminProkirixiID();      // ΜΙΑ ΔΙΑΧΕΙΡΙΣΤΙΚΗ ΠΡΟΚΗΡΥΞΗ ΕΙΝΑΙ ΠΑΝΤΑ ΑΝΟΙΚΤΗ
                PARAMETROI_PINAKES_REPORTS parameters = new PARAMETROI_PINAKES_REPORTS();
                parameters.PROKIRIXI_ID = prokirixiID;
                parameters.PERIFERIAKI_ID = (from d in db.SYS_SCHOOLS where d.SCHOOL_ID == loggedSchool.USER_SCHOOLID select d.SCHOOL_PERIFERIAKI_ID).FirstOrDefault();
                parameters.SCHOOL_ID = loggedSchool.USER_SCHOOLID;
                return View(parameters);
            }
        }

         
        public ActionResult Pinakas1_Web_Print()
        {
            bool val1 = (System.Web.HttpContext.Current.User != null) && System.Web.HttpContext.Current.User.Identity.IsAuthenticated;
            if (!val1)
            {
                return RedirectToAction("Login", "USER_SCHOOLS");
            }
            else
            {
                loggedSchool = GetLoginSchool();
                int prokirixiID = Common.GetAdminProkirixiID();      // ΜΙΑ ΔΙΑΧΕΙΡΙΣΤΙΚΗ ΠΡΟΚΗΡΥΞΗ ΕΙΝΑΙ ΠΑΝΤΑ ΑΝΟΙΚΤΗ
                PARAMETROI_PINAKES_REPORTS parameters = new PARAMETROI_PINAKES_REPORTS();
                parameters.PROKIRIXI_ID = prokirixiID;
                parameters.SCHOOL_ID = loggedSchool.USER_SCHOOLID;
                parameters.PERIFERIAKI_ID = (from d in db.SYS_SCHOOLS where d.SCHOOL_ID == loggedSchool.USER_SCHOOLID select d.SCHOOL_PERIFERIAKI_ID).FirstOrDefault();
                return View(parameters);
            }
        }

         
        public ActionResult Pinakas11_School_Print()
        {
            bool val1 = (System.Web.HttpContext.Current.User != null) && System.Web.HttpContext.Current.User.Identity.IsAuthenticated;
            if (!val1)
            {
                return RedirectToAction("Login", "USER_SCHOOLS");
            }
            else
            {
                loggedSchool = GetLoginSchool();
                int prokirixiID = Common.GetAdminProkirixiID();      // ΜΙΑ ΔΙΑΧΕΙΡΙΣΤΙΚΗ ΠΡΟΚΗΡΥΞΗ ΕΙΝΑΙ ΠΑΝΤΑ ΑΝΟΙΚΤΗ
                PARAMETROI_PINAKES_REPORTS parameters = new PARAMETROI_PINAKES_REPORTS();
                parameters.PROKIRIXI_ID = prokirixiID;
                parameters.SCHOOL_ID = loggedSchool.USER_SCHOOLID;
                parameters.PERIFERIAKI_ID = (from d in db.SYS_SCHOOLS where d.SCHOOL_ID == loggedSchool.USER_SCHOOLID select d.SCHOOL_PERIFERIAKI_ID).FirstOrDefault();
                return View(parameters);
            }
        }

         
        public ActionResult Pinakas11_Web_Print()
        {
            bool val1 = (System.Web.HttpContext.Current.User != null) && System.Web.HttpContext.Current.User.Identity.IsAuthenticated;
            if (!val1)
            {
                return RedirectToAction("Login", "USER_SCHOOLS");
            }
            else
            {
                loggedSchool = GetLoginSchool();
                int prokirixiID = Common.GetAdminProkirixiID();      // ΜΙΑ ΔΙΑΧΕΙΡΙΣΤΙΚΗ ΠΡΟΚΗΡΥΞΗ ΕΙΝΑΙ ΠΑΝΤΑ ΑΝΟΙΚΤΗ
                PARAMETROI_PINAKES_REPORTS parameters = new PARAMETROI_PINAKES_REPORTS();
                parameters.PROKIRIXI_ID = prokirixiID;
                parameters.SCHOOL_ID = loggedSchool.USER_SCHOOLID;
                parameters.PERIFERIAKI_ID = (from d in db.SYS_SCHOOLS where d.SCHOOL_ID == loggedSchool.USER_SCHOOLID select d.SCHOOL_PERIFERIAKI_ID).FirstOrDefault();
                return View(parameters);
            }
        }

         
        public ActionResult Pinakas2_School_Print()
        {
            bool val1 = (System.Web.HttpContext.Current.User != null) && System.Web.HttpContext.Current.User.Identity.IsAuthenticated;
            if (!val1)
            {
                return RedirectToAction("Login", "USER_SCHOOLS");
            }
            else
            {
                loggedSchool = GetLoginSchool();
                int prokirixiID = Common.GetAdminProkirixiID();      // ΜΙΑ ΔΙΑΧΕΙΡΙΣΤΙΚΗ ΠΡΟΚΗΡΥΞΗ ΕΙΝΑΙ ΠΑΝΤΑ ΑΝΟΙΚΤΗ
                PARAMETROI_PINAKES_REPORTS parameters = new PARAMETROI_PINAKES_REPORTS();
                parameters.PROKIRIXI_ID = prokirixiID;
                parameters.SCHOOL_ID = loggedSchool.USER_SCHOOLID;
                parameters.PERIFERIAKI_ID = (from d in db.SYS_SCHOOLS where d.SCHOOL_ID == loggedSchool.USER_SCHOOLID select d.SCHOOL_PERIFERIAKI_ID).FirstOrDefault();
                return View(parameters);
            }
        }

         
        public ActionResult Pinakas2_Web_Print()
        {
            bool val1 = (System.Web.HttpContext.Current.User != null) && System.Web.HttpContext.Current.User.Identity.IsAuthenticated;
            if (!val1)
            {
                return RedirectToAction("Login", "USER_SCHOOLS");
            }
            else
            {
                loggedSchool = GetLoginSchool();
                int prokirixiID = Common.GetAdminProkirixiID();      // ΜΙΑ ΔΙΑΧΕΙΡΙΣΤΙΚΗ ΠΡΟΚΗΡΥΞΗ ΕΙΝΑΙ ΠΑΝΤΑ ΑΝΟΙΚΤΗ
                PARAMETROI_PINAKES_REPORTS parameters = new PARAMETROI_PINAKES_REPORTS();
                parameters.PROKIRIXI_ID = prokirixiID;
                parameters.SCHOOL_ID = loggedSchool.USER_SCHOOLID;
                parameters.PERIFERIAKI_ID = (from d in db.SYS_SCHOOLS where d.SCHOOL_ID == loggedSchool.USER_SCHOOLID select d.SCHOOL_PERIFERIAKI_ID).FirstOrDefault();
                return View(parameters);
            }
        }

         
        public ActionResult Pinakas21_School_Print()
        {
            bool val1 = (System.Web.HttpContext.Current.User != null) && System.Web.HttpContext.Current.User.Identity.IsAuthenticated;
            if (!val1)
            {
                return RedirectToAction("Login", "USER_SCHOOLS");
            }
            else
            {
                loggedSchool = GetLoginSchool();
                int prokirixiID = Common.GetAdminProkirixiID();      // ΜΙΑ ΔΙΑΧΕΙΡΙΣΤΙΚΗ ΠΡΟΚΗΡΥΞΗ ΕΙΝΑΙ ΠΑΝΤΑ ΑΝΟΙΚΤΗ
                PARAMETROI_PINAKES_REPORTS parameters = new PARAMETROI_PINAKES_REPORTS();
                parameters.PROKIRIXI_ID = prokirixiID;
                parameters.SCHOOL_ID = loggedSchool.USER_SCHOOLID;
                parameters.PERIFERIAKI_ID = (from d in db.SYS_SCHOOLS where d.SCHOOL_ID == loggedSchool.USER_SCHOOLID select d.SCHOOL_PERIFERIAKI_ID).FirstOrDefault();
                return View(parameters);
            }
        }

         
        public ActionResult Pinakas21_Web_Print()
        {
            bool val1 = (System.Web.HttpContext.Current.User != null) && System.Web.HttpContext.Current.User.Identity.IsAuthenticated;
            if (!val1)
            {
                return RedirectToAction("Login", "USER_SCHOOLS");
            }
            else
            {
                loggedSchool = GetLoginSchool();
                int prokirixiID = Common.GetAdminProkirixiID();      // ΜΙΑ ΔΙΑΧΕΙΡΙΣΤΙΚΗ ΠΡΟΚΗΡΥΞΗ ΕΙΝΑΙ ΠΑΝΤΑ ΑΝΟΙΚΤΗ
                PARAMETROI_PINAKES_REPORTS parameters = new PARAMETROI_PINAKES_REPORTS();
                parameters.PROKIRIXI_ID = prokirixiID;
                parameters.SCHOOL_ID = loggedSchool.USER_SCHOOLID;
                parameters.PERIFERIAKI_ID = (from d in db.SYS_SCHOOLS where d.SCHOOL_ID == loggedSchool.USER_SCHOOLID select d.SCHOOL_PERIFERIAKI_ID).FirstOrDefault();
                return View(parameters);
            }
        }

         
        public ActionResult Pinakas3_School_Print()
        {
            bool val1 = (System.Web.HttpContext.Current.User != null) && System.Web.HttpContext.Current.User.Identity.IsAuthenticated;
            if (!val1)
            {
                return RedirectToAction("Login", "USER_SCHOOLS");
            }
            else
            {
                loggedSchool = GetLoginSchool();
                int prokirixiID = Common.GetAdminProkirixiID();      // ΜΙΑ ΔΙΑΧΕΙΡΙΣΤΙΚΗ ΠΡΟΚΗΡΥΞΗ ΕΙΝΑΙ ΠΑΝΤΑ ΑΝΟΙΚΤΗ
                PARAMETROI_PINAKES_REPORTS parameters = new PARAMETROI_PINAKES_REPORTS();
                parameters.PROKIRIXI_ID = prokirixiID;
                parameters.SCHOOL_ID = loggedSchool.USER_SCHOOLID;
                parameters.PERIFERIAKI_ID = (from d in db.SYS_SCHOOLS where d.SCHOOL_ID == loggedSchool.USER_SCHOOLID select d.SCHOOL_PERIFERIAKI_ID).FirstOrDefault();
                return View(parameters);
            }
        }

         
        public ActionResult Pinakas3_Web_Print()
        {
            bool val1 = (System.Web.HttpContext.Current.User != null) && System.Web.HttpContext.Current.User.Identity.IsAuthenticated;
            if (!val1)
            {
                return RedirectToAction("Login", "USER_SCHOOLS");
            }
            else
            {
                loggedSchool = GetLoginSchool();
                int prokirixiID = Common.GetAdminProkirixiID();      // ΜΙΑ ΔΙΑΧΕΙΡΙΣΤΙΚΗ ΠΡΟΚΗΡΥΞΗ ΕΙΝΑΙ ΠΑΝΤΑ ΑΝΟΙΚΤΗ
                PARAMETROI_PINAKES_REPORTS parameters = new PARAMETROI_PINAKES_REPORTS();
                parameters.PROKIRIXI_ID = prokirixiID;
                parameters.SCHOOL_ID = loggedSchool.USER_SCHOOLID;
                parameters.PERIFERIAKI_ID = (from d in db.SYS_SCHOOLS where d.SCHOOL_ID == loggedSchool.USER_SCHOOLID select d.SCHOOL_PERIFERIAKI_ID).FirstOrDefault();
                return View(parameters);
            }
        }

         
        public ActionResult Pinakas4_School_Print()
        {
            bool val1 = (System.Web.HttpContext.Current.User != null) && System.Web.HttpContext.Current.User.Identity.IsAuthenticated;
            if (!val1)
            {
                return RedirectToAction("Login", "USER_SCHOOLS");
            }
            else
            {
                loggedSchool = GetLoginSchool();
                int prokirixiID = Common.GetAdminProkirixiID();      // ΜΙΑ ΔΙΑΧΕΙΡΙΣΤΙΚΗ ΠΡΟΚΗΡΥΞΗ ΕΙΝΑΙ ΠΑΝΤΑ ΑΝΟΙΚΤΗ
                PARAMETROI_PINAKES_REPORTS parameters = new PARAMETROI_PINAKES_REPORTS();
                parameters.PROKIRIXI_ID = prokirixiID;
                parameters.SCHOOL_ID = loggedSchool.USER_SCHOOLID;
                parameters.PERIFERIAKI_ID = (from d in db.SYS_SCHOOLS where d.SCHOOL_ID == loggedSchool.USER_SCHOOLID select d.SCHOOL_PERIFERIAKI_ID).FirstOrDefault();
                return View(parameters);
            }
        }

         
        public ActionResult Pinakas4_Web_Print()
        {
            bool val1 = (System.Web.HttpContext.Current.User != null) && System.Web.HttpContext.Current.User.Identity.IsAuthenticated;
            if (!val1)
            {
                return RedirectToAction("Login", "USER_SCHOOLS");
            }
            else
            {
                loggedSchool = GetLoginSchool();
                int prokirixiID = Common.GetAdminProkirixiID();      // ΜΙΑ ΔΙΑΧΕΙΡΙΣΤΙΚΗ ΠΡΟΚΗΡΥΞΗ ΕΙΝΑΙ ΠΑΝΤΑ ΑΝΟΙΚΤΗ
                PARAMETROI_PINAKES_REPORTS parameters = new PARAMETROI_PINAKES_REPORTS();
                parameters.PROKIRIXI_ID = prokirixiID;
                parameters.SCHOOL_ID = loggedSchool.USER_SCHOOLID;
                parameters.PERIFERIAKI_ID = (from d in db.SYS_SCHOOLS where d.SCHOOL_ID == loggedSchool.USER_SCHOOLID select d.SCHOOL_PERIFERIAKI_ID).FirstOrDefault();
                return View(parameters);
            }
        }

         
        public ActionResult Pinakas5_School_Print()
        {
            bool val1 = (System.Web.HttpContext.Current.User != null) && System.Web.HttpContext.Current.User.Identity.IsAuthenticated;
            if (!val1)
            {
                return RedirectToAction("Login", "USER_SCHOOLS");
            }
            else
            {
                loggedSchool = GetLoginSchool();
                int prokirixiID = Common.GetAdminProkirixiID();      // ΜΙΑ ΔΙΑΧΕΙΡΙΣΤΙΚΗ ΠΡΟΚΗΡΥΞΗ ΕΙΝΑΙ ΠΑΝΤΑ ΑΝΟΙΚΤΗ
                PARAMETROI_PINAKES_REPORTS parameters = new PARAMETROI_PINAKES_REPORTS();
                parameters.PROKIRIXI_ID = prokirixiID;
                parameters.SCHOOL_ID = loggedSchool.USER_SCHOOLID;
                parameters.PERIFERIAKI_ID = (from d in db.SYS_SCHOOLS where d.SCHOOL_ID == loggedSchool.USER_SCHOOLID select d.SCHOOL_PERIFERIAKI_ID).FirstOrDefault();
                return View(parameters);
            }
        }

         
        public ActionResult Pinakas5_Web_Print()
        {
            bool val1 = (System.Web.HttpContext.Current.User != null) && System.Web.HttpContext.Current.User.Identity.IsAuthenticated;
            if (!val1)
            {
                return RedirectToAction("Login", "USER_SCHOOLS");
            }
            else
            {
                loggedSchool = GetLoginSchool();
                int prokirixiID = Common.GetAdminProkirixiID();      // ΜΙΑ ΔΙΑΧΕΙΡΙΣΤΙΚΗ ΠΡΟΚΗΡΥΞΗ ΕΙΝΑΙ ΠΑΝΤΑ ΑΝΟΙΚΤΗ
                PARAMETROI_PINAKES_REPORTS parameters = new PARAMETROI_PINAKES_REPORTS();
                parameters.PROKIRIXI_ID = prokirixiID;
                parameters.SCHOOL_ID = loggedSchool.USER_SCHOOLID;
                parameters.PERIFERIAKI_ID = (from d in db.SYS_SCHOOLS where d.SCHOOL_ID == loggedSchool.USER_SCHOOLID select d.SCHOOL_PERIFERIAKI_ID).FirstOrDefault();
                return View(parameters);
            }
        }
        #endregion


        #region ΠΙΝΑΚΕΣ ΜΟΡΙΟΔΟΤΗΣΗΣ ΧΩΡΙΣ ΟΝΟΜΑΤΑ

        public ActionResult xPinakas1a_Print()
        {
            bool val1 = (System.Web.HttpContext.Current.User != null) && System.Web.HttpContext.Current.User.Identity.IsAuthenticated;
            if (!val1)
            {
                return RedirectToAction("Login", "USER_SCHOOLS");
            }
            else
            {
                loggedSchool = GetLoginSchool();
                int prokirixiID = Common.GetAdminProkirixiID();      // ΜΙΑ ΔΙΑΧΕΙΡΙΣΤΙΚΗ ΠΡΟΚΗΡΥΞΗ ΕΙΝΑΙ ΠΑΝΤΑ ΑΝΟΙΚΤΗ
                PARAMETROI_PINAKES_REPORTS parameters = new PARAMETROI_PINAKES_REPORTS();
                parameters.PROKIRIXI_ID = prokirixiID;
                parameters.PERIFERIAKI_ID = (from d in db.SYS_SCHOOLS where d.SCHOOL_ID == loggedSchool.USER_SCHOOLID select d.SCHOOL_PERIFERIAKI_ID).FirstOrDefault();
                parameters.SCHOOL_ID = loggedSchool.USER_SCHOOLID;
                return View(parameters);
            }
        }


        public ActionResult xPinakas1b_Print()
        {
            bool val1 = (System.Web.HttpContext.Current.User != null) && System.Web.HttpContext.Current.User.Identity.IsAuthenticated;
            if (!val1)
            {
                return RedirectToAction("Login", "USER_SCHOOLS");
            }
            else
            {
                loggedSchool = GetLoginSchool();
                int prokirixiID = Common.GetAdminProkirixiID();      // ΜΙΑ ΔΙΑΧΕΙΡΙΣΤΙΚΗ ΠΡΟΚΗΡΥΞΗ ΕΙΝΑΙ ΠΑΝΤΑ ΑΝΟΙΚΤΗ
                PARAMETROI_PINAKES_REPORTS parameters = new PARAMETROI_PINAKES_REPORTS();
                parameters.PROKIRIXI_ID = prokirixiID;
                parameters.SCHOOL_ID = loggedSchool.USER_SCHOOLID;
                parameters.PERIFERIAKI_ID = (from d in db.SYS_SCHOOLS where d.SCHOOL_ID == loggedSchool.USER_SCHOOLID select d.SCHOOL_PERIFERIAKI_ID).FirstOrDefault();
                return View(parameters);
            }
        }


        public ActionResult xPinakas2a_Print()
        {
            bool val1 = (System.Web.HttpContext.Current.User != null) && System.Web.HttpContext.Current.User.Identity.IsAuthenticated;
            if (!val1)
            {
                return RedirectToAction("Login", "USER_SCHOOLS");
            }
            else
            {
                loggedSchool = GetLoginSchool();
                int prokirixiID = Common.GetAdminProkirixiID();      // ΜΙΑ ΔΙΑΧΕΙΡΙΣΤΙΚΗ ΠΡΟΚΗΡΥΞΗ ΕΙΝΑΙ ΠΑΝΤΑ ΑΝΟΙΚΤΗ
                PARAMETROI_PINAKES_REPORTS parameters = new PARAMETROI_PINAKES_REPORTS();
                parameters.PROKIRIXI_ID = prokirixiID;
                parameters.SCHOOL_ID = loggedSchool.USER_SCHOOLID;
                parameters.PERIFERIAKI_ID = (from d in db.SYS_SCHOOLS where d.SCHOOL_ID == loggedSchool.USER_SCHOOLID select d.SCHOOL_PERIFERIAKI_ID).FirstOrDefault();
                return View(parameters);
            }
        }


        public ActionResult xPinakas2b_Print()
        {
            bool val1 = (System.Web.HttpContext.Current.User != null) && System.Web.HttpContext.Current.User.Identity.IsAuthenticated;
            if (!val1)
            {
                return RedirectToAction("Login", "USER_SCHOOLS");
            }
            else
            {
                loggedSchool = GetLoginSchool();
                int prokirixiID = Common.GetAdminProkirixiID();      // ΜΙΑ ΔΙΑΧΕΙΡΙΣΤΙΚΗ ΠΡΟΚΗΡΥΞΗ ΕΙΝΑΙ ΠΑΝΤΑ ΑΝΟΙΚΤΗ
                PARAMETROI_PINAKES_REPORTS parameters = new PARAMETROI_PINAKES_REPORTS();
                parameters.PROKIRIXI_ID = prokirixiID;
                parameters.SCHOOL_ID = loggedSchool.USER_SCHOOLID;
                parameters.PERIFERIAKI_ID = (from d in db.SYS_SCHOOLS where d.SCHOOL_ID == loggedSchool.USER_SCHOOLID select d.SCHOOL_PERIFERIAKI_ID).FirstOrDefault();
                return View(parameters);
            }
        }


        public ActionResult xPinakas3a_Print()
        {
            bool val1 = (System.Web.HttpContext.Current.User != null) && System.Web.HttpContext.Current.User.Identity.IsAuthenticated;
            if (!val1)
            {
                return RedirectToAction("Login", "USER_SCHOOLS");
            }
            else
            {
                loggedSchool = GetLoginSchool();
                int prokirixiID = Common.GetAdminProkirixiID();      // ΜΙΑ ΔΙΑΧΕΙΡΙΣΤΙΚΗ ΠΡΟΚΗΡΥΞΗ ΕΙΝΑΙ ΠΑΝΤΑ ΑΝΟΙΚΤΗ
                PARAMETROI_PINAKES_REPORTS parameters = new PARAMETROI_PINAKES_REPORTS();
                parameters.PROKIRIXI_ID = prokirixiID;
                parameters.SCHOOL_ID = loggedSchool.USER_SCHOOLID;
                parameters.PERIFERIAKI_ID = (from d in db.SYS_SCHOOLS where d.SCHOOL_ID == loggedSchool.USER_SCHOOLID select d.SCHOOL_PERIFERIAKI_ID).FirstOrDefault();
                return View(parameters);
            }
        }


        public ActionResult xPinakas4a_Print()
        {
            bool val1 = (System.Web.HttpContext.Current.User != null) && System.Web.HttpContext.Current.User.Identity.IsAuthenticated;
            if (!val1)
            {
                return RedirectToAction("Login", "USER_SCHOOLS");
            }
            else
            {
                loggedSchool = GetLoginSchool();
                int prokirixiID = Common.GetAdminProkirixiID();      // ΜΙΑ ΔΙΑΧΕΙΡΙΣΤΙΚΗ ΠΡΟΚΗΡΥΞΗ ΕΙΝΑΙ ΠΑΝΤΑ ΑΝΟΙΚΤΗ
                PARAMETROI_PINAKES_REPORTS parameters = new PARAMETROI_PINAKES_REPORTS();
                parameters.PROKIRIXI_ID = prokirixiID;
                parameters.SCHOOL_ID = loggedSchool.USER_SCHOOLID;
                parameters.PERIFERIAKI_ID = (from d in db.SYS_SCHOOLS where d.SCHOOL_ID == loggedSchool.USER_SCHOOLID select d.SCHOOL_PERIFERIAKI_ID).FirstOrDefault();
                return View(parameters);
            }
        }


        public ActionResult xPinakas5a_Print()
        {
            bool val1 = (System.Web.HttpContext.Current.User != null) && System.Web.HttpContext.Current.User.Identity.IsAuthenticated;
            if (!val1)
            {
                return RedirectToAction("Login", "USER_SCHOOLS");
            }
            else
            {
                loggedSchool = GetLoginSchool();
                int prokirixiID = Common.GetAdminProkirixiID();      // ΜΙΑ ΔΙΑΧΕΙΡΙΣΤΙΚΗ ΠΡΟΚΗΡΥΞΗ ΕΙΝΑΙ ΠΑΝΤΑ ΑΝΟΙΚΤΗ
                PARAMETROI_PINAKES_REPORTS parameters = new PARAMETROI_PINAKES_REPORTS();
                parameters.PROKIRIXI_ID = prokirixiID;
                parameters.SCHOOL_ID = loggedSchool.USER_SCHOOLID;
                parameters.PERIFERIAKI_ID = (from d in db.SYS_SCHOOLS where d.SCHOOL_ID == loggedSchool.USER_SCHOOLID select d.SCHOOL_PERIFERIAKI_ID).FirstOrDefault();
                return View(parameters);
            }
        }

        #endregion


        #region ΣΤΑΤΙΣΤΙΚΕΣ ΕΚΘΕΣΕΙΣ
    
        public ActionResult Stat1_Gender()
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
            PARAMETROI_STAT_REPORTS parametes = new PARAMETROI_STAT_REPORTS
            {
                PROKIRIXI_ID = prokirixiId
            };
            return View(parametes);
        }

        public ActionResult Stat2_Postgrad()
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
            PARAMETROI_STAT_REPORTS parametes = new PARAMETROI_STAT_REPORTS
            {
                PROKIRIXI_ID = prokirixiId
            };
            return View(parametes);
        }

        public ActionResult Stat3_Epimorfosi()
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
            PARAMETROI_STAT_REPORTS parametes = new PARAMETROI_STAT_REPORTS
            {
                PROKIRIXI_ID = prokirixiId
            };
            return View(parametes);
        }

        public ActionResult Stat4_Anergia()
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
            PARAMETROI_STAT_REPORTS parametes = new PARAMETROI_STAT_REPORTS
            {
                PROKIRIXI_ID = prokirixiId
            };
            return View(parametes);
        }

        public ActionResult Stat5_Anergia2()
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
            PARAMETROI_STAT_REPORTS parametes = new PARAMETROI_STAT_REPORTS
            {
                PROKIRIXI_ID = prokirixiId
            };
            return View(parametes);
        }

        public ActionResult Stat6_AgeGroups()
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
            PARAMETROI_STAT_REPORTS parametes = new PARAMETROI_STAT_REPORTS
            {
                PROKIRIXI_ID = prokirixiId
            };
            return View(parametes);
        }

        public ActionResult Stat7_SocialGroup()
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
            PARAMETROI_STAT_REPORTS parametes = new PARAMETROI_STAT_REPORTS
            {
                PROKIRIXI_ID = prokirixiId
            };
            return View(parametes);
        }

        public ActionResult Stat8_PtyxiaTypes()
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
            PARAMETROI_STAT_REPORTS parametes = new PARAMETROI_STAT_REPORTS
            {
                PROKIRIXI_ID = prokirixiId
            };
            return View(parametes);
        }

        #endregion

    }
}