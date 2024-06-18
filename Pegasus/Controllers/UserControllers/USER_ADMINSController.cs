using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Newtonsoft.Json;
using Pegasus.BPM;
using Pegasus.DAL;
using Pegasus.DAL.Security;
using Pegasus.Models;
using Pegasus.Services;
using Pegasus.Filters;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Pegasus.Controllers.UserControllers
{
    [ErrorHandlerFilter]
    public class USER_ADMINSController : Controller
    {
        private readonly PegasusDBEntities db;
        private USER_ADMINS loggedAdmin;

        private readonly IUserAdminService userAdminService;

        public USER_ADMINSController(PegasusDBEntities entities, IUserAdminService userAdminService)
        {
            db = entities;
            this.userAdminService = userAdminService;
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
                loggedAdmin = db.USER_ADMINS.Where(u => u.USERNAME == System.Web.HttpContext.Current.User.Identity.Name).FirstOrDefault();
                if (loggedAdmin != null)
                {
                    ViewBag.loggedUser = loggedAdmin.FULLNAME;
                    return RedirectToAction("Index", "Admin");
                }
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login([Bind(Include = "USERNAME,PASSWORD")]  UserAdminViewModel model)
        {
            var user = db.USER_ADMINS.Where(u => u.USERNAME == model.USERNAME && u.PASSWORD == model.PASSWORD).FirstOrDefault();

            if (user != null)
            {
                WriteUserCookie(model);
                SetLoginStatus(user, true);
                return RedirectToAction("Index", "Admin");
            }
            ModelState.AddModelError("", "Το όνομα χρήστη ή/και ο κωδ.πρόσβασης δεν είναι σωστά");
            return View(model);
        }

        public ActionResult Login2()
        {
            bool val1 = (System.Web.HttpContext.Current.User != null) && System.Web.HttpContext.Current.User.Identity.IsAuthenticated;
            if (!val1)
            {
                ViewBag.loggedUser = "(χωρίς σύνδεση)";
            }
            else
            {
                loggedAdmin = db.USER_ADMINS.Where(u => u.USERNAME == System.Web.HttpContext.Current.User.Identity.Name).FirstOrDefault();
                if (loggedAdmin != null)
                {
                    ViewBag.loggedUser = loggedAdmin.FULLNAME;
                    return RedirectToAction("Index", "Admin");
                }
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login2([Bind(Include = "USERNAME,PASSWORD")]  UserAdminViewModel model)
        {
            var user = db.USER_ADMINS.Where(u => u.USERNAME == model.USERNAME && u.PASSWORD == model.PASSWORD).FirstOrDefault();

            if (user != null)
            {
                WriteUserCookie(model);
                SetLoginStatus(user, true);
                return RedirectToAction("UserTeachersList", "USER_TEACHERS");
            }
            ModelState.AddModelError("", "Το όνομα χρήστη ή/και ο κωδ.πρόσβασης δεν είναι σωστά");
            return View(model);
        }


        [AllowAnonymous]
        public ActionResult LogOut([Bind(Include = "ISACTIVE")] USER_ADMINS item)
        {
            var user = db.USER_ADMINS.Where(u => u.USERNAME == item.USERNAME && u.PASSWORD == item.PASSWORD).FirstOrDefault();

            FormsAuthentication.SignOut();
            SetLoginStatus(user, false);

            return RedirectToAction("Index", "Home");
        }

        public void WriteUserCookie(UserAdminViewModel user)
        {
            AdminPrincipalSerializeModel serializeModel = new AdminPrincipalSerializeModel();
            serializeModel.UserId = user.USER_ID;
            serializeModel.Username = user.USERNAME;
            serializeModel.FullName = user.FULLNAME;

            string userData = JsonConvert.SerializeObject(serializeModel);
            FormsAuthenticationTicket authTicket = new FormsAuthenticationTicket(1, user.USERNAME, 
                DateTime.Now, DateTime.Now.AddMinutes(Kerberos.TICKET_TIMEOUT_MINUTES), false, userData);

            string encTicket = FormsAuthentication.Encrypt(authTicket);
            HttpCookie faCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encTicket);
            Response.Cookies.Add(faCookie);
        }

        public void SetLoginStatus(USER_ADMINS user, bool value)
        {
            user.ISACTIVE = value;
            db.Entry(user).State = EntityState.Modified;
            db.SaveChanges();
         }

        public ActionResult ListAdmin()
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

        public USER_ADMINS GetLoginAdmin()
        {
            loggedAdmin = db.USER_ADMINS.Where(u => u.USERNAME == System.Web.HttpContext.Current.User.Identity.Name).FirstOrDefault();

            ViewBag.loggedAdmin = loggedAdmin;
            ViewBag.loggedUser = loggedAdmin.FULLNAME;

            return loggedAdmin;
        }

        #region Grid CRUD Functions

        public ActionResult Admin_Read([DataSourceRequest] DataSourceRequest request)
        {
            List<UserAdminViewModel> data = userAdminService.Read();

            return Json(data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Admin_Create([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<UserAdminViewModel> data)
        {
            List<UserAdminViewModel> results = new List<UserAdminViewModel>();

            foreach (var item in data)
            {
                if (item != null && ModelState.IsValid)
                {
                    userAdminService.Create(item);
                    results.Add(item);
                }
            }
            return Json(results.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Admin_Update([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<UserAdminViewModel> data)
        {
            if (data != null && ModelState.IsValid)
            {
                foreach (var item in data)
                {
                    userAdminService.Update(item);
                }
            }
            return Json(data.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Admin_Destroy([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<UserAdminViewModel> data)
        {
            if (data.Any())
            {
                foreach (var item in data)
                {
                    userAdminService.Destroy(item);
                }
            }
            return Json(data.ToDataSourceResult(request, ModelState));
        }

        #endregion


    }
}
