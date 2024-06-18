using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.Security;
using Newtonsoft.Json;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using Pegasus.Filters;
using Pegasus.Models;
using Pegasus.BPM;
using Pegasus.DAL;
using Pegasus.DAL.Security;
using Pegasus.Notification;


namespace Pegasus.Controllers.UserControllers
{
    [ErrorHandlerFilter]
    public class USER_SCHOOLSController : Controller
    {
        private PegasusDBEntities db;
        private USER_SCHOOLS loggedSchool;

        public USER_SCHOOLSController(PegasusDBEntities entities)
        {
            db = entities;
        }


        public ActionResult Login()
        {
            bool val1 = (System.Web.HttpContext.Current.User != null) && System.Web.HttpContext.Current.User.Identity.IsAuthenticated;
            if (!val1)
            {
                ViewBag.loggedUser = "(χωρίς σύνδεση)";
            }
            else
            {
                loggedSchool = db.USER_SCHOOLS.Where(u => u.USERNAME == System.Web.HttpContext.Current.User.Identity.Name).FirstOrDefault();
                if (loggedSchool != null)
                {
                    ViewBag.loggedUser = GetLoginSchool();
                    return RedirectToAction("Index", "School");
                }
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login([Bind(Include = "USERNAME,PASSWORD")]  UserSchoolViewModel model)
        {
            var user = db.USER_SCHOOLS.Where(u => u.USERNAME == model.USERNAME && u.PASSWORD == model.PASSWORD).FirstOrDefault();

            if (user != null)
            {
                WriteUserCookie(model);
                SetLoginStatus(user, true);
                return RedirectToAction("Index", "School");
            }
            ModelState.AddModelError("", "Το όνομα χρήστη ή/και ο κωδ.πρόσβασης δεν είναι σωστά");
            return View(model);
        }

        [AllowAnonymous]
        public ActionResult LogOut([Bind(Include = "ISACTIVE")] USER_SCHOOLS userSchool)
        {
            var user = db.USER_SCHOOLS.Where(u => u.USERNAME == userSchool.USERNAME && u.PASSWORD == userSchool.PASSWORD).FirstOrDefault();

            FormsAuthentication.SignOut();
            SetLoginStatus(user, false);

            return RedirectToAction("Index", "Home");
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

        public void SetLoginStatus(USER_SCHOOLS user, bool value)
        {
            db.Entry(user).State = EntityState.Modified;
            user.ISACTIVE = value;
            db.SaveChanges();
        }

        public void WriteUserCookie(UserSchoolViewModel user)
        {
            SchoolPrincipalSerializeModel serializeModel = new SchoolPrincipalSerializeModel();
            serializeModel.UserId = user.USER_ID;
            serializeModel.Username = user.USERNAME;
            serializeModel.SchoolId = user.USER_SCHOOLID ?? 0;

            string userData = JsonConvert.SerializeObject(serializeModel);
            FormsAuthenticationTicket authTicket = new FormsAuthenticationTicket(1, user.USERNAME, 
                DateTime.Now, DateTime.Now.AddMinutes(Kerberos.TICKET_TIMEOUT_MINUTES), false, userData);

            string encTicket = FormsAuthentication.Encrypt(authTicket);
            HttpCookie faCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encTicket);
            Response.Cookies.Add(faCookie);
        }
    }
}
