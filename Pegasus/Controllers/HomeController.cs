using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.IO;
using Pegasus.DAL;
using Pegasus.Models;
using Pegasus.BPM;
using Pegasus.Filters;

namespace Pegasus.Controllers
{
    [ErrorHandlerFilter]
    public class HomeController : Controller
    {
        private readonly PegasusDBEntities db;

        public HomeController(PegasusDBEntities entities)
        {
            db = entities;
        }

        [AllowAnonymous]
        public ActionResult Index()
        {
            string userTxt = "(χωρίς σύνδεση)";

            try
            {
                bool AppStatusOn = GetApplicationStatus();
                if (AppStatusOn == false)
                {
                    return RedirectToAction("AppStatusOff", "Home");
                }
            }
            catch
            {
                return RedirectToAction("ErrorConnect", "Home");
            }

            // first, delete any remaining Captcha image files in directory
            //CleanupCaptchaImages();
            if (isApplicationLocal())
                ViewBag.appTest = true;

            ViewBag.loggedUser = userTxt;

            ProkirixisViewModel prokirixi = Common.GetAdminProkirixi();
            if (prokirixi == null)
                prokirixi = new ProkirixisViewModel();
            return View(prokirixi);
        }

        [AllowAnonymous]
        public ActionResult AppStatusOff()
        {
            string message = GetStatusMessage();
            if (string.IsNullOrEmpty(message))
                message = "Η εφαρμογή είναι προσωρινά απενεργοποιημένη για εργασίες συντήρησης και αναβάθμισης.";

            ViewData["message"] = message;
            return View();
        }

        [AllowAnonymous]
        public ActionResult ErrorConnect()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult About()
        {
            ViewBag.Message = "Σύντομη περιγραφή της εφαρμογής.";

            return View();
        }

        [AllowAnonymous]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        public ActionResult Contact()
        {
            ViewBag.Message = "Στοιχεία επικοινωνίας.";

            var data = (from d in db.SUPPORT select d).ToList();

            return View(data);
        }

        public string GetStatusMessage()
        {
            var data = (from d in db.APP_STATUS select d).FirstOrDefault();

            return (data.STATUS_MESSAGE);
        }

        public bool GetApplicationStatus()
        {
            var data = (from d in db.APP_STATUS select d).FirstOrDefault();
            bool status = data.STATUS_VALUE ?? false;
            return status;
        }

        public bool isApplicationLocal()
        {
            var data = (from d in db.APP_STATUS select d).FirstOrDefault();
            bool status = data.LOCAL_TEST ?? false;
            return status;
        }

        public void CleanupCaptchaImages()
        {
            string[] files = System.IO.Directory.GetFiles(Server.MapPath("~/CAPTCHA/"), "*.png");

            foreach (string file in files)
            {
                if (System.IO.File.Exists(file))
                    System.IO.File.Delete(file);
            }
        }

        [AllowAnonymous]
        public ActionResult Print()
        {
            var invoices = db.SYS_PERIFERIES.ToList();

            var items = new PERIFERIESViewModel(invoices);
            items.SelectedInvoiceId = 1;
            return View(items);
        }


        #region States of Grids

        [ValidateInput(false)]
        public ActionResult Save(string data)
        {
            Session["data"] = data;

            //int temp = 1;

            return new EmptyResult();
        }

        [AllowAnonymous]
        public ActionResult Load()
        {
            if (Session["data"] != null)
            {
                string data = Session["data"].ToString();
            }

            //int temp = 1;

            return Json(Session["data"], JsonRequestBehavior.AllowGet);
        }

        [ValidateInput(false)]
        public ActionResult SaveRow(string data)
        {
            Session["row"] = data;

            //int temp = 1;

            return new EmptyResult();
        }

        [AllowAnonymous]
        public ActionResult LoadRow()
        {
            if (Session["row"] != null)
            {
                string data = Session["row"].ToString();
            }

            //int temp = 1;

            return Json(Session["row"], JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}