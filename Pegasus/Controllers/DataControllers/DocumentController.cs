using System;
using System.IO;
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
using Pegasus.DAL;
using Pegasus.Models;
using Pegasus.BPM;
using Pegasus.Filters;
using Pegasus.Notification;
using Pegasus.Services;

namespace Pegasus.Controllers.DataControllers
{
    [ErrorHandlerFilter]
    public class DocumentController : Controller
    {
        private readonly PegasusDBEntities db;
        private USER_ADMINS loggedAdmin;
        private USER_SCHOOLS loggedSchool;

        private const string ENSTASEIS_PATH = "~/Uploads/Enstaseis/";

        private readonly IEnstaseisService enstaseisService;

        public DocumentController(PegasusDBEntities entities, IEnstaseisService enstaseisService)
        {
            db = entities;
            this.enstaseisService = enstaseisService;
        }


        #region ΜΕΤΑΦΟΡΤΩΜΕΝΕΣ ΕΝΣΤΑΣΕΙΣ (ΣΧΟΛΕΙΟ)

        public ActionResult EnstaseisSchool(string notify = null)
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
                string Msg = "Δεν βρέθηκε ενεργή Προκήρυξη.";
                return RedirectToAction("Index", "School", new { notify = Msg });
            }
            int schoolYearId = (int)Common.GetAdminProkirixi().SCHOOL_YEAR;
            ViewData["prokirixiProtocol"] = Common.GetAdminProkirixi().PROTOCOL;
            ViewData["schoolYearText"] = Common.GetSchoolYearText(schoolYearId);

            if (notify != null)
            {
                this.ShowMessage(MessageType.Warning, notify);
            }
            if (!AitisisSchoolExist())
            {
                string msg = "Δεν βρέθηκαν αιτήσεις για το σχολείο αυτό. Η προβολή μεταφορτωμένων αρχείων είναι αδύνατη.";
                return RedirectToAction("Index", "School", new { notify = msg });
            }

            PopulateSchoolYears();
            PopulateSchoolAitisis();

            return View();
        }


        #region MASTER GRID CRUD FUNCTIONS

        public ActionResult Upload_Read([DataSourceRequest] DataSourceRequest request)
        {
            int prokirixiId = Common.GetAdminProkirixiID();
            int schoolId = (int)GetLoginSchool().USER_SCHOOLID;

            IEnumerable<UploadsViewModel> data = enstaseisService.Read(prokirixiId, schoolId);

            return Json(data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        #endregion


        #region CHILD GRID (UPLOADED FILEDETAILS)

        public ActionResult UploadFiles_Read([DataSourceRequest] DataSourceRequest request, int uploadId = 0)
        {
            List<UploadsFilesViewModel> data = enstaseisService.ReadFiles(uploadId);

            return Json(data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
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

        #endregion


        #endregion


        #region ΜΕΤΑΦΟΡΤΩΜΕΝΕΣ ΕΝΣΤΑΣΕΙΣ (ΔΙΑΧΕΙΡΙΣΤΗΣ)

        public ActionResult EnstaseisAdmin(string notify = null)
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
                string Msg = "Δεν βρέθηκε ενεργή Προκήρυξη.";
                return RedirectToAction("Index", "Admin", new { notify = Msg });
            }
            int schoolYearId = (int)Common.GetAdminProkirixi().SCHOOL_YEAR;
            ViewData["prokirixiProtocol"] = Common.GetAdminProkirixi().PROTOCOL;
            ViewData["schoolYearText"] = Common.GetSchoolYearText(schoolYearId);

            if (notify != null)
            {
                this.ShowMessage(MessageType.Warning, notify);
            }
            if (!AitisisGlobalExist())
            {
                string msg = "Δεν βρέθηκαν αιτήσεις για την Προκήρυξη αυτή. Η προβολή μεταφορτωμένων αρχείων είναι αδύνατη.";
                return RedirectToAction("Index", "Admin", new { notify = msg });
            }

            PopulateSchoolYears();
            PopulateGlobalAitisis();

            return View();
        }

        #region MASTER GRID CRUD FUNCTIONS

        public ActionResult xUpload_Read([DataSourceRequest] DataSourceRequest request, int schoolId)
        {
            int prokirixiId = Common.GetAdminProkirixiID();

            IEnumerable<UploadsViewModel> data = enstaseisService.Read(prokirixiId, schoolId);

            return Json(data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        #endregion


        #region CHILD GRID (UPLOADED FILEDETAILS)

        // All functions are the same as for schools

        #endregion

        #endregion


        #region GETTERS

        public JsonResult GetSchools()
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

        public USER_SCHOOLS GetLoginSchool()
        {
            loggedSchool = db.USER_SCHOOLS.Where(u => u.USERNAME == System.Web.HttpContext.Current.User.Identity.Name).FirstOrDefault();

            int SchoolID = loggedSchool.USER_SCHOOLID ?? 0;
            var _school = (from s in db.sqlUSER_SCHOOL
                           where s.USER_SCHOOLID == SchoolID
                           select new { s.SCHOOL_NAME }).FirstOrDefault();

            ViewBag.loggedUser = _school.SCHOOL_NAME;
            return loggedSchool;
        }

        public USER_ADMINS GetLoginAdmin()
        {
            loggedAdmin = db.USER_ADMINS.Where(u => u.USERNAME == System.Web.HttpContext.Current.User.Identity.Name).FirstOrDefault();

            ViewBag.loggedAdmin = loggedAdmin;
            ViewBag.loggedUser = loggedAdmin.FULLNAME;

            return loggedAdmin;
        }

        #endregion


        #region POPULATORS

        public bool AitisisSchoolExist()
        {
            int schoolId = (int)GetLoginSchool().USER_SCHOOLID;
            int prokirixiId = Common.GetAdminProkirixiID();

            var aitiseis = (from d in db.AITISIS where d.PROKIRIXI_ID == prokirixiId && d.SCHOOL_ID == schoolId select d).ToList();
            if (aitiseis.Count > 0)
                return true;
            return false;
        }

        public bool AitisisGlobalExist()
        {
            int prokirixiId = Common.GetAdminProkirixiID();

            var aitiseis = (from d in db.AITISIS where d.PROKIRIXI_ID == prokirixiId select d).ToList();
            if (aitiseis.Count > 0)
                return true;
            return false;
        }

        public void PopulateSchoolAitisis()
        {
            int prokirixiId = Common.GetAdminProkirixiID();
            int schoolId = (int)GetLoginSchool().USER_SCHOOLID;

            var aitiseis = (from d in db.AITISIS where d.PROKIRIXI_ID == prokirixiId && d.SCHOOL_ID == schoolId select d).ToList();

            ViewData["aitiseis"] = aitiseis;
            ViewData["defaultAitisi"] = aitiseis.First().AITISI_ID;
        }

        public void PopulateGlobalAitisis()
        {
            int prokirixiId = Common.GetAdminProkirixiID();
            var aitiseis = (from d in db.AITISIS where d.PROKIRIXI_ID == prokirixiId select d).ToList();

            ViewData["aitiseis"] = aitiseis;
            ViewData["defaultAitisi"] = aitiseis.First().AITISI_ID;
        }

        public void PopulateSchoolYears()
        {
            var syears = (from s in db.SYS_SCHOOLYEARS
                          orderby s.SY_TEXT descending
                          select s).ToList();

            ViewData["school_years"] = syears;
            ViewData["defaultSchoolYear"] = syears.First().SY_ID;
        }

        #endregion


        #region ERROR PAGES

        public ActionResult ErrorData(string notify = null)
        {
            if (notify != null) this.ShowMessage(MessageType.Warning, notify);

            return View();
        }

        public ActionResult Error(string notify = null)
        {
            if (notify != null) this.ShowMessage(MessageType.Warning, notify);

            return View();
        }

        #endregion

    }
}