using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Mvc;
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
    public class AdminController : ControllerUnit
    {
        private readonly PegasusDBEntities db;
        private USER_ADMINS loggedAdmin;

        private readonly ITeacherRegistryService teacherRegistryService;
        private readonly IUserTeacherService userTeacherService;
        private readonly IUserSchoolService userSchoolService;
        private readonly IAitisiRegistryService aitisiRegistryService;

        public AdminController(PegasusDBEntities entities, 
            ITeacherRegistryService teacherRegistryService, IAitisiRegistryService aitisiRegistryService,
            IUserTeacherService userTeacherService, IUserSchoolService userSchoolService) : base(entities)
        {
            db = entities;

            this.teacherRegistryService = teacherRegistryService;
            this.userTeacherService = userTeacherService;
            this.userSchoolService = userSchoolService;
            this.aitisiRegistryService = aitisiRegistryService;
        }


        public ActionResult Index(string notify = null)
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
            if (notify != null)
            {
                this.ShowMessage(MessageType.Warning, notify);
            }
            return View();
        }


        #region ΜΗΤΡΩΟ ΑΙΤΗΣΕΩΝ - ΑΙΤΗΣΕΙΣ

        public ActionResult AitiseisRegistry()
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

            IEnumerable<sqlTEACHER_AITISEIS> aitisis = aitisiRegistryService.Read(prokirixiId);

            PopulateTeachTypes();
            PopulateSchoolYears();
            PopulateIncomeYears();

            return View(aitisis);
        }

        public ActionResult Aitiseis_Read([DataSourceRequest] DataSourceRequest request, int prokirixiId = 0)
        {
            IEnumerable<sqlTEACHER_AITISEIS> data = aitisiRegistryService.Read(prokirixiId);

            return Json(data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        
        public ActionResult ViewAitisi(int aitisiId)
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
                return RedirectToAction("Login", "USER_ADMINS");
            }
            else
            {
                loggedAdmin = GetLoginAdmin();
            }

            var data = (from d in db.AITISIS
                        where d.AITISI_ID == aitisiId
                        select new AitisisViewModel
                        {
                            PROKIRIXI_ID = d.PROKIRIXI_ID,
                            AITISI_ID = d.AITISI_ID
                        }).FirstOrDefault();

            return View(data);

        }

        public ActionResult ResultsMoriaView(int aitisiId)
        {
            bool val1 = (System.Web.HttpContext.Current.User != null) && System.Web.HttpContext.Current.User.Identity.IsAuthenticated;
            if (!val1)
            {
                return RedirectToAction("Login", "USER_ADMINS");
            }
            else
            {
                loggedAdmin = GetLoginAdmin();
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
                return RedirectToAction("Login", "USER_ADMINS");
            }
            else
            {
                loggedAdmin = GetLoginAdmin();
            }
            AitisisViewModel aitisi = (from t in db.AITISIS
                                       where t.AITISI_ID == aitisiId
                                       select new AitisisViewModel
                                       {
                                           AITISI_ID = t.AITISI_ID,
                                           AITISI_PROTOCOL = t.AITISI_PROTOCOL,
                                           AITISI_DATE = t.AITISI_DATE,
                                           AFM = t.AFM
                                       }).FirstOrDefault();
            return View(aitisi);
        }

        public ActionResult AitisiExperiencePrint(int aitisiId = 0)
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

            AitisiParameters aitisi = new AitisiParameters();
            aitisi.AITISI_ID = aitisiId;

            return View(aitisi);
        }

        public ActionResult AitiseisRegistryPrint()
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


        #region ΜΗΤΡΩΟ ΕΚΠΑΙΔΕΥΤΙΚΩΝ

        public ActionResult TeachersRegistry()
        {
            bool val1 = (System.Web.HttpContext.Current.User != null) && System.Web.HttpContext.Current.User.Identity.IsAuthenticated;
            if (!val1)
            {
                return RedirectToAction("Login", "USER_ADMINS");
            }
            else
            {
                loggedAdmin = GetLoginAdmin();
            }
            int prokirixiId  = Common.GetAdminProkirixiID();
            if (!(prokirixiId > 0))
            {
                this.ShowMessage(MessageType.Warning, "Δεν βρέθηκε Ενεργή Προκήρυξη.");
                return RedirectToAction("Index", "Admin");
            }

            IEnumerable<sqlTEACHERS_WITH_AITISEIS_UNIQUE> data = teacherRegistryService.Read();

            return View(data);
        }

        public ActionResult Teachers_Read([DataSourceRequest] DataSourceRequest request)
        {
            IEnumerable<sqlTEACHERS_WITH_AITISEIS_UNIQUE> data = teacherRegistryService.Read();

            return Json(data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        public ActionResult TeachersRegistryPrint()
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

            TeacherRegistryParameters parameters = new TeacherRegistryParameters
            {
                SCHOOL_ID = (from d in db.SYS_SCHOOLS where d.SCHOOL_TYPEID == 2 orderby d.SCHOOL_NAME select d).First().SCHOOL_ID,
                PROKIRIXI_ID = prokirixiId
            };
            return View(parameters);
        }

        #endregion


        #region ΛΟΓΑΡΙΑΣΜΟΙ ΥΠΟΨΗΦΙΩΝ ΕΚΠΑΙΔΕΥΤΙΚΩΝ

        public ActionResult ListUserTeachers()
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

        #region ACCOUNT GRID CRUD Functions

        [HttpPost]
        public ActionResult UserTeacher_Read([DataSourceRequest] DataSourceRequest request)
        {
            IEnumerable<UserTeacherEditViewModel> data = userTeacherService.Read();

            return Json(data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult UserTeacher_Create([DataSourceRequest] DataSourceRequest request, UserTeacherEditViewModel data)
        {
            UserTeacherEditViewModel newdata = new UserTeacherEditViewModel();

            if (data != null && ModelState.IsValid)
            {
                userTeacherService.Create(data);
                newdata = userTeacherService.Refresh(data.USER_ID);
            }
            return Json(new[] { newdata }.ToDataSourceResult(request, ModelState), JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult UserTeacher_Update([DataSourceRequest] DataSourceRequest request, UserTeacherEditViewModel data)
        {
            var newdata = new UserTeacherEditViewModel();

            if (data != null && ModelState.IsValid)
            {
                userTeacherService.Update(data);
                newdata = userTeacherService.Refresh(data.USER_ID);
            }
            return Json(new[] { newdata }.ToDataSourceResult(request, ModelState), JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult UserTeacher_Destroy([DataSourceRequest] DataSourceRequest request, UserTeacherEditViewModel data)
        {
            if (data != null)
            {
                userTeacherService.Destroy(data);
            }
            return Json(new[] { data }.ToDataSourceResult(request, ModelState), JsonRequestBehavior.AllowGet);
        }

        #endregion


        #region CHILD GRID - ΣΤΟΙΧΕΙΑ ΕΚΠΑΙΔΕΥΤΙΚΟΥ

        public ActionResult UserTeacherInfo_Read([DataSourceRequest] DataSourceRequest request, string afm = null)
        {
            List<TeacherAccountInfoViewModel> data = userTeacherService.ReadInfo(afm);

            return Json(data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        #endregion

        #endregion


        #region ΛΟΓΑΡΙΑΣΜΟΙ ΣΧΟΛΕΙΩΝ-ΕΠΙΤΡΟΠΩΝ

        public ActionResult ListUserSchools(string notify = null)
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

            PopulateSchools();
            return View();
        }

        public ActionResult CreatePasswords()
        {
            var schools = (from s in db.USER_SCHOOLS
                           where s.USER_SCHOOLID >= 100
                           select s).ToList();

            foreach(var school in schools)
            {
                school.PASSWORD = Common.GeneratePassword() + string.Format("{0:000}", school.USER_SCHOOLID);
                db.Entry(school).State = EntityState.Modified;
                db.SaveChanges();
            }
            
            string notify = "Η δημιουργία νέων κωδικών σχολείων ολοκληρώθηκε.";
            return RedirectToAction("ListUserSchools", "Admin", new { notify });
        }


        #region Grid CRUD Functions

        [HttpPost]
        public ActionResult UserSchool_Read([DataSourceRequest] DataSourceRequest request)
        {
            IEnumerable<UserSchoolViewModel> data = userSchoolService.Read();

            return Json(data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        public ActionResult UserSchool_Create([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<UserSchoolViewModel> data)
        {
            var results = new List<UserSchoolViewModel>();
            foreach (var item in data)
            {
                if (item != null && ModelState.IsValid)
                {
                    userSchoolService.Create(item);
                    results.Add(item);
                }
            }
            return Json(results.ToDataSourceResult(request, ModelState), JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult UserSchool_Update([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<UserSchoolViewModel> data)
        {
            if (data.Any())
            {
                foreach (var item in data)
                {
                    userSchoolService.Update(item);
                }
            }
            return Json(data.ToDataSourceResult(request, ModelState), JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult UserSchool_Destroy([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<UserSchoolViewModel> data)
        {
            if (data.Any())
            {
                foreach (var item in data)
                {
                    userSchoolService.Destroy(item);
                }
            }
            return Json(data.ToDataSourceResult(request, ModelState), JsonRequestBehavior.AllowGet);
        }

        #endregion

        #endregion


        #region ΠΙΝΑΚΕΣ ΜΟΡΙΟΔΟΤΗΣΗΣ ΜΕ ΟΝΟΜΑΤΑ
         
        public ActionResult Pinakas1_School_Print()
        {
            bool val1 = (System.Web.HttpContext.Current.User != null) && System.Web.HttpContext.Current.User.Identity.IsAuthenticated;
            if (!val1)
            {
                return RedirectToAction("Login", "USER_ADMINS");
            }
            else
            {
                PARAMETROI_PINAKES_REPORTS parameters = new PARAMETROI_PINAKES_REPORTS();
                parameters.PROKIRIXI_ID = Common.GetAdminProkirixiID();
                return View(parameters);
            }
        }

         
        public ActionResult Pinakas1_Web_Print()
        {
            bool val1 = (System.Web.HttpContext.Current.User != null) && System.Web.HttpContext.Current.User.Identity.IsAuthenticated;
            if (!val1)
            {
                return RedirectToAction("Login", "USER_ADMINS");
            }
            else
            {
                PARAMETROI_PINAKES_REPORTS parameters = new PARAMETROI_PINAKES_REPORTS();
                parameters.PROKIRIXI_ID = Common.GetAdminProkirixiID();
                return View(parameters);
            }
        }

         
        public ActionResult Pinakas11_School_Print()
        {
            bool val1 = (System.Web.HttpContext.Current.User != null) && System.Web.HttpContext.Current.User.Identity.IsAuthenticated;
            if (!val1)
            {
                return RedirectToAction("Login", "USER_ADMINS");
            }
            else
            {
                PARAMETROI_PINAKES_REPORTS parameters = new PARAMETROI_PINAKES_REPORTS();
                parameters.PROKIRIXI_ID = Common.GetAdminProkirixiID();
                return View(parameters);
            }
        }

         
        public ActionResult Pinakas11_Web_Print()
        {
            bool val1 = (System.Web.HttpContext.Current.User != null) && System.Web.HttpContext.Current.User.Identity.IsAuthenticated;
            if (!val1)
            {
                return RedirectToAction("Login", "USER_ADMINS");
            }
            else
            {
                PARAMETROI_PINAKES_REPORTS parameters = new PARAMETROI_PINAKES_REPORTS();
                parameters.PROKIRIXI_ID = Common.GetAdminProkirixiID();
                return View(parameters);
            }
        }

         
        public ActionResult Pinakas2_School_Print()
        {
            bool val1 = (System.Web.HttpContext.Current.User != null) && System.Web.HttpContext.Current.User.Identity.IsAuthenticated;
            if (!val1)
            {
                return RedirectToAction("Login", "USER_ADMINS");
            }
            else
            {
                PARAMETROI_PINAKES_REPORTS parameters = new PARAMETROI_PINAKES_REPORTS();
                parameters.PROKIRIXI_ID = Common.GetAdminProkirixiID();
                return View(parameters);
            }
        }

         
        public ActionResult Pinakas2_Web_Print()
        {
            bool val1 = (System.Web.HttpContext.Current.User != null) && System.Web.HttpContext.Current.User.Identity.IsAuthenticated;
            if (!val1)
            {
                return RedirectToAction("Login", "USER_ADMINS");
            }
            else
            {
                PARAMETROI_PINAKES_REPORTS parameters = new PARAMETROI_PINAKES_REPORTS();
                parameters.PROKIRIXI_ID = Common.GetAdminProkirixiID();
                return View(parameters);
            }
        }

         
        public ActionResult Pinakas21_School_Print()
        {
            bool val1 = (System.Web.HttpContext.Current.User != null) && System.Web.HttpContext.Current.User.Identity.IsAuthenticated;
            if (!val1)
            {
                return RedirectToAction("Login", "USER_ADMINS");
            }
            else
            {
                PARAMETROI_PINAKES_REPORTS parameters = new PARAMETROI_PINAKES_REPORTS();
                parameters.PROKIRIXI_ID = Common.GetAdminProkirixiID();
                return View(parameters);
            }
        }

         
        public ActionResult Pinakas21_Web_Print()
        {
            bool val1 = (System.Web.HttpContext.Current.User != null) && System.Web.HttpContext.Current.User.Identity.IsAuthenticated;
            if (!val1)
            {
                return RedirectToAction("Login", "USER_ADMINS");
            }
            else
            {
                PARAMETROI_PINAKES_REPORTS parameters = new PARAMETROI_PINAKES_REPORTS();
                parameters.PROKIRIXI_ID = Common.GetAdminProkirixiID();
                return View(parameters);
            }
        }

         
        public ActionResult Pinakas3_School_Print()
        {
            bool val1 = (System.Web.HttpContext.Current.User != null) && System.Web.HttpContext.Current.User.Identity.IsAuthenticated;
            if (!val1)
            {
                return RedirectToAction("Login", "USER_ADMINS");
            }
            else
            {
                PARAMETROI_PINAKES_REPORTS parameters = new PARAMETROI_PINAKES_REPORTS();
                parameters.PROKIRIXI_ID = Common.GetAdminProkirixiID();
                return View(parameters);
            }
        }

         
        public ActionResult Pinakas3_Web_Print()
        {
            bool val1 = (System.Web.HttpContext.Current.User != null) && System.Web.HttpContext.Current.User.Identity.IsAuthenticated;
            if (!val1)
            {
                return RedirectToAction("Login", "USER_ADMINS");
            }
            else
            {
                PARAMETROI_PINAKES_REPORTS parameters = new PARAMETROI_PINAKES_REPORTS();
                parameters.PROKIRIXI_ID = Common.GetAdminProkirixiID();
                return View(parameters);
            }
        }

         
        public ActionResult Pinakas4_School_Print()
        {
            bool val1 = (System.Web.HttpContext.Current.User != null) && System.Web.HttpContext.Current.User.Identity.IsAuthenticated;
            if (!val1)
            {
                return RedirectToAction("Login", "USER_ADMINS");
            }
            else
            {
                PARAMETROI_PINAKES_REPORTS parameters = new PARAMETROI_PINAKES_REPORTS();
                parameters.PROKIRIXI_ID = Common.GetAdminProkirixiID();
                return View(parameters);
            }
        }

         
        public ActionResult Pinakas4_Web_Print()
        {
            bool val1 = (System.Web.HttpContext.Current.User != null) && System.Web.HttpContext.Current.User.Identity.IsAuthenticated;
            if (!val1)
            {
                return RedirectToAction("Login", "USER_ADMINS");
            }
            else
            {
                PARAMETROI_PINAKES_REPORTS parameters = new PARAMETROI_PINAKES_REPORTS();
                parameters.PROKIRIXI_ID = Common.GetAdminProkirixiID();
                return View(parameters);
            }
        }

         
        public ActionResult Pinakas5_School_Print()
        {
            bool val1 = (System.Web.HttpContext.Current.User != null) && System.Web.HttpContext.Current.User.Identity.IsAuthenticated;
            if (!val1)
            {
                return RedirectToAction("Login", "USER_ADMINS");
            }
            else
            {
                PARAMETROI_PINAKES_REPORTS parameters = new PARAMETROI_PINAKES_REPORTS();
                parameters.PROKIRIXI_ID = Common.GetAdminProkirixiID();
                return View(parameters);
            }
        }

         
        public ActionResult Pinakas5_Web_Print()
        {
            bool val1 = (System.Web.HttpContext.Current.User != null) && System.Web.HttpContext.Current.User.Identity.IsAuthenticated;
            if (!val1)
            {
                return RedirectToAction("Login", "USER_ADMINS");
            }
            else
            {
                PARAMETROI_PINAKES_REPORTS parameters = new PARAMETROI_PINAKES_REPORTS();
                parameters.PROKIRIXI_ID = Common.GetAdminProkirixiID();
                return View(parameters);
            }
        }
        #endregion


        #region ΠΙΝΑΚΕΣ ΜΟΡΙΟΔΟΤΗΣΗΣ ΧΩΡΙΣ ΟΝΟΜΑΤΑ (15/06/2020)

        public ActionResult xPinakas1a_Print()
        {
            bool val1 = (System.Web.HttpContext.Current.User != null) && System.Web.HttpContext.Current.User.Identity.IsAuthenticated;
            if (!val1)
            {
                return RedirectToAction("Login", "USER_ADMINS");
            }
            else
            {
                PARAMETROI_PINAKES_REPORTS parameters = new PARAMETROI_PINAKES_REPORTS();
                parameters.PROKIRIXI_ID = Common.GetAdminProkirixiID();
                return View(parameters);
            }
        }


        public ActionResult xPinakas1b_Print()
        {
            bool val1 = (System.Web.HttpContext.Current.User != null) && System.Web.HttpContext.Current.User.Identity.IsAuthenticated;
            if (!val1)
            {
                return RedirectToAction("Login", "USER_ADMINS");
            }
            else
            {
                PARAMETROI_PINAKES_REPORTS parameters = new PARAMETROI_PINAKES_REPORTS();
                parameters.PROKIRIXI_ID = Common.GetAdminProkirixiID();
                return View(parameters);
            }
        }


        public ActionResult xPinakas2a_Print()
        {
            bool val1 = (System.Web.HttpContext.Current.User != null) && System.Web.HttpContext.Current.User.Identity.IsAuthenticated;
            if (!val1)
            {
                return RedirectToAction("Login", "USER_ADMINS");
            }
            else
            {
                PARAMETROI_PINAKES_REPORTS parameters = new PARAMETROI_PINAKES_REPORTS();
                parameters.PROKIRIXI_ID = Common.GetAdminProkirixiID();
                return View(parameters);
            }
        }


        public ActionResult xPinakas2b_Print()
        {
            bool val1 = (System.Web.HttpContext.Current.User != null) && System.Web.HttpContext.Current.User.Identity.IsAuthenticated;
            if (!val1)
            {
                return RedirectToAction("Login", "USER_ADMINS");
            }
            else
            {
                PARAMETROI_PINAKES_REPORTS parameters = new PARAMETROI_PINAKES_REPORTS();
                parameters.PROKIRIXI_ID = Common.GetAdminProkirixiID();
                return View(parameters);
            }
        }


        public ActionResult xPinakas3a_Print()
        {
            bool val1 = (System.Web.HttpContext.Current.User != null) && System.Web.HttpContext.Current.User.Identity.IsAuthenticated;
            if (!val1)
            {
                return RedirectToAction("Login", "USER_ADMINS");
            }
            else
            {
                PARAMETROI_PINAKES_REPORTS parameters = new PARAMETROI_PINAKES_REPORTS();
                parameters.PROKIRIXI_ID = Common.GetAdminProkirixiID();
                return View(parameters);
            }
        }

        public ActionResult xPinakas4a_Print()
        {
            bool val1 = (System.Web.HttpContext.Current.User != null) && System.Web.HttpContext.Current.User.Identity.IsAuthenticated;
            if (!val1)
            {
                return RedirectToAction("Login", "USER_ADMINS");
            }
            else
            {
                PARAMETROI_PINAKES_REPORTS parameters = new PARAMETROI_PINAKES_REPORTS();
                parameters.PROKIRIXI_ID = Common.GetAdminProkirixiID();
                return View(parameters);
            }
        }

        public ActionResult xPinakas5a_Print()
        {
            bool val1 = (System.Web.HttpContext.Current.User != null) && System.Web.HttpContext.Current.User.Identity.IsAuthenticated;
            if (!val1)
            {
                return RedirectToAction("Login", "USER_ADMINS");
            }
            else
            {
                PARAMETROI_PINAKES_REPORTS parameters = new PARAMETROI_PINAKES_REPORTS();
                parameters.PROKIRIXI_ID = Common.GetAdminProkirixiID();
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
                return RedirectToAction("Login", "USER_ADMINS");
            }
            else
            {
                loggedAdmin = GetLoginAdmin();
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
                return RedirectToAction("Login", "USER_ADMINS");
            }
            else
            {
                loggedAdmin = GetLoginAdmin();
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
                return RedirectToAction("Login", "USER_ADMINS");
            }
            else
            {
                loggedAdmin = GetLoginAdmin();
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
                return RedirectToAction("Login", "USER_ADMINS");
            }
            else
            {
                loggedAdmin = GetLoginAdmin();
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
                return RedirectToAction("Login", "USER_ADMINS");
            }
            else
            {
                loggedAdmin = GetLoginAdmin();
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
                return RedirectToAction("Login", "USER_ADMINS");
            }
            else
            {
                loggedAdmin = GetLoginAdmin();
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
                return RedirectToAction("Login", "USER_ADMINS");
            }
            else
            {
                loggedAdmin = GetLoginAdmin();
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
                return RedirectToAction("Login", "USER_ADMINS");
            }
            else
            {
                loggedAdmin = GetLoginAdmin();
            }
            int prokirixiId = Common.GetAdminProkirixiID();
            PARAMETROI_STAT_REPORTS parametes = new PARAMETROI_STAT_REPORTS
            {
                PROKIRIXI_ID = prokirixiId
            };
            return View(parametes);
        }

        #endregion


        #region ΕΚΤΥΠΩΣΕΙΣ

        public ActionResult CheckInformaticsTeachers()
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
            PARAMETROI_STAT_REPORTS parameters = new PARAMETROI_STAT_REPORTS
            {
                PROKIRIXI_ID = prokirixiId
            };
            return View(parameters);
        }

        public ActionResult AitiseisDailyPrint()
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
            int prokirixiId = Common.GetOpenProkirixiID(true);
            PARAMETROI_STAT_REPORTS parameters = new PARAMETROI_STAT_REPORTS
            {
                PROKIRIXI_ID = prokirixiId
            };
            return View(parameters);
        }

        public ActionResult SchoolAitiseisPrint()
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
            int prokirixiId = Common.GetOpenProkirixiID(true);
            PARAMETROI_STAT_REPORTS parameters = new PARAMETROI_STAT_REPORTS
            {
                PROKIRIXI_ID = prokirixiId
            };
            return View(parameters);
        }

        public ActionResult MultipleAitiseis()
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

        public ActionResult DuplicateAitiseisPrint()
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

        public ActionResult SchoolAccountsPrint()
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

        public ActionResult EidikotitesPrint()
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

        public ActionResult EidikotitesOldNewPrint()
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

        public ActionResult ProgressCheck()
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
    
        public ActionResult EnstaseisDetail()
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
            EnstaseisParametroi ep = new EnstaseisParametroi
            {
                PROKIRIXI_ID = prokirixiId
            };
            return View(ep);
        }

        public ActionResult EnstaseisDetailPost()
        {
            bool val1 = (System.Web.HttpContext.Current.User != null) && System.Web.HttpContext.Current.User.Identity.IsAuthenticated;
            if (!val1)
            {
                return RedirectToAction("Login", "USER_ADMINS");
            }
            else
            {
                loggedAdmin = GetLoginAdmin();
            }
            int prokirixiId = Common.GetAdminProkirixiID();
            EnstaseisParametroi ep = new EnstaseisParametroi
            {
                PROKIRIXI_ID = prokirixiId
            };
            return View(ep);
        }

        public ActionResult EnstaseisSummary()
        {
            bool val1 = (System.Web.HttpContext.Current.User != null) && System.Web.HttpContext.Current.User.Identity.IsAuthenticated;
            if (!val1)
            {
                return RedirectToAction("Login", "USER_ADMINS");
            }
            else
            {
                loggedAdmin = GetLoginAdmin();
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
                return RedirectToAction("Login", "USER_ADMINS");
            }
            else
            {
                loggedAdmin = GetLoginAdmin();
            }
            return View();
        }

        public ActionResult TeachersMoriaPrint()
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

        public ActionResult AitiseisToCheck()
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

        public ActionResult TeachersEidikotitaPrint()
        {
            int prokirixiId = Common.GetAdminProkirixiID();

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
            TeacherRegistryParameters parameters = new TeacherRegistryParameters
            {
                SCHOOL_ID = 100,
                PROKIRIXI_ID = prokirixiId
            };
            return View(parameters);
        }

        #endregion


        #region ΑΙΤΗΣΕΙΣ ΜΕ ΠΡΟΫΠΗΡΕΣΙΕΣ (9-8-2019)

        public ActionResult AitiseisWithWork()
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

        public ActionResult AitiseisWithWork_Read([DataSourceRequest] DataSourceRequest request, int prokirixiId = 0)
        {
            List<AitiseisWithWorkViewModel> data = GetAitiseisWithWork(prokirixiId);

            return Json(data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        public List<AitiseisWithWorkViewModel> GetAitiseisWithWork(int prokirixiId = 0)
        {
            List<AitiseisWithWorkViewModel> data = new List<AitiseisWithWorkViewModel>();
            int current_prokirixiID = Common.GetAdminProkirixiID();

            if (prokirixiId > 0)
            {
                    data = (from d in db.sqlAITISEIS_WITH_EXPERIENCE
                        where d.PROKIRIXI_ID == prokirixiId
                        orderby d.FULLNAME
                        select new AitiseisWithWorkViewModel
                        {
                            AITISI_ID = d.AITISI_ID,
                            AFM = d.AFM,
                            AGE = d.AGE,
                            AITISI_PROTOCOL = d.AITISI_PROTOCOL,
                            EIDIKOTITA_DESC = d.EIDIKOTITA_DESC,
                            EIDIKOTITA_ID = d.EIDIKOTITA_ID,
                            EIDIKOTITA_KLADOS_ID = d.EIDIKOTITA_KLADOS_ID,
                            FULLNAME = d.FULLNAME,
                            KLADOS_NAME = d.KLADOS_NAME,
                            PROKIRIXI_ID = d.PROKIRIXI_ID,
                            PROTOCOL = d.PROTOCOL,
                            SCHOOL_ID = d.SCHOOL_ID,
                            SCHOOL_NAME = d.SCHOOL_NAME
                        }).ToList();
            }
            else
            {
                data = (from d in db.sqlAITISEIS_WITH_EXPERIENCE
                        orderby d.FULLNAME
                        where d.PROKIRIXI_ID == current_prokirixiID
                        select new AitiseisWithWorkViewModel
                        {
                            AITISI_ID = d.AITISI_ID,
                            AFM = d.AFM,
                            AGE = d.AGE,
                            AITISI_PROTOCOL = d.AITISI_PROTOCOL,
                            EIDIKOTITA_DESC = d.EIDIKOTITA_DESC,
                            EIDIKOTITA_ID = d.EIDIKOTITA_ID,
                            EIDIKOTITA_KLADOS_ID = d.EIDIKOTITA_KLADOS_ID,
                            FULLNAME = d.FULLNAME,
                            KLADOS_NAME = d.KLADOS_NAME,
                            PROKIRIXI_ID = d.PROKIRIXI_ID,
                            PROTOCOL = d.PROTOCOL,
                            SCHOOL_ID = d.SCHOOL_ID,
                            SCHOOL_NAME = d.SCHOOL_NAME
                        }).ToList();
            }
            return data;
        }

        public ActionResult AitiseisWithWorkPrint()
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


        #region ΑΙΤΗΣΕΙΣ ΧΩΡΙΣ ΠΡΟΫΠΗΡΕΣΙΕΣ (9-8-2019)

        public ActionResult AitiseisWithoutWork()
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

        public ActionResult AitiseisWithoutWork_Read([DataSourceRequest] DataSourceRequest request, int prokirixiId = 0)
        {
            List<AitiseisWithoutWorkViewModel> data = GetAitiseisWithoutWork(prokirixiId);

            return Json(data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        public List<AitiseisWithoutWorkViewModel> GetAitiseisWithoutWork(int prokirixiId = 0)
        {
            List<AitiseisWithoutWorkViewModel> data = new List<AitiseisWithoutWorkViewModel>();
            int current_prokirixiID = Common.GetAdminProkirixiID();

            if (prokirixiId > 0)
            {
                data = (from d in db.sqlAITISEIS_WITHOUT_EXPERIENCE
                        where d.PROKIRIXI_ID == prokirixiId
                        orderby d.FULLNAME
                        select new AitiseisWithoutWorkViewModel
                        {
                            AITISI_ID = d.AITISI_ID,
                            AFM = d.AFM,
                            AGE = d.AGE,
                            AITISI_PROTOCOL = d.AITISI_PROTOCOL,
                            EIDIKOTITA_DESC = d.EIDIKOTITA_DESC,
                            EIDIKOTITA_ID = d.EIDIKOTITA_ID,
                            EIDIKOTITA_KLADOS_ID = d.EIDIKOTITA_KLADOS_ID,
                            FULLNAME = d.FULLNAME,
                            KLADOS_NAME = d.KLADOS_NAME,
                            PROKIRIXI_ID = d.PROKIRIXI_ID,
                            PROTOCOL = d.PROTOCOL,
                            SCHOOL_ID = d.SCHOOL_ID,
                            SCHOOL_NAME = d.SCHOOL_NAME
                        }).ToList();
            }
            else
            {
                data = (from d in db.sqlAITISEIS_WITHOUT_EXPERIENCE
                        orderby d.FULLNAME
                        where d.PROKIRIXI_ID == current_prokirixiID
                        select new AitiseisWithoutWorkViewModel
                        {
                            AITISI_ID = d.AITISI_ID,
                            AFM = d.AFM,
                            AGE = d.AGE,
                            AITISI_PROTOCOL = d.AITISI_PROTOCOL,
                            EIDIKOTITA_DESC = d.EIDIKOTITA_DESC,
                            EIDIKOTITA_ID = d.EIDIKOTITA_ID,
                            EIDIKOTITA_KLADOS_ID = d.EIDIKOTITA_KLADOS_ID,
                            FULLNAME = d.FULLNAME,
                            KLADOS_NAME = d.KLADOS_NAME,
                            PROKIRIXI_ID = d.PROKIRIXI_ID,
                            PROTOCOL = d.PROTOCOL,
                            SCHOOL_ID = d.SCHOOL_ID,
                            SCHOOL_NAME = d.SCHOOL_NAME
                        }).ToList();
            }
            return data;
        }

        public ActionResult AitiseisWithoutWorkPrint()
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


        #region ΑΙΤΗΣΕΙΣ ΧΩΡΙΣ ΚΑΤΑΧΩΡΗΣΗ ΕΠΙΜΟΡΦΩΣΕΩΝ

        public ActionResult MissingReeducations()
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


        #region ΛΟΓΑΡΙΑΣΜΟΙ ΕΚΠΑΙΔΕΥΤΙΚΩΝ ΜΕ ΑΙΤΗΣΕΙΣ (ΓΙΑ ΑΝΑΖΗΤΗΣΕΙΣ)

        public ActionResult TeachersAitiseisAccountsPrint()
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

    }
}