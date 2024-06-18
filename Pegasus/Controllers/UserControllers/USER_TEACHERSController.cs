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
using System.IO;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using Pegasus.Filters;
using Pegasus.Models;
using Pegasus.BPM;
using Pegasus.DAL;
using Pegasus.DAL.Security;
using Pegasus.Notification;
using Pegasus.CAPTCHA;


namespace Pegasus.Controllers.UserControllers
{
    [ErrorHandlerFilter]
    public class USER_TEACHERSController : Controller
    {
        private readonly PegasusDBEntities db;
        private USER_TEACHERS loggedTeacher;

        public USER_TEACHERSController(PegasusDBEntities entities)
        {
            db = entities;
        }


        #region NOT USED AS LOGIN IS EXTERNAL FROM TAXISNET (08/05/2020)

        public ActionResult Login(string notify = null)
        {
            bool AppStatusOn = true;
            try
            {
                AppStatusOn = GetApplicationStatus();
                if (AppStatusOn == false)
                {
                    return RedirectToAction("AppStatusOff", "Home");
                }
            }
            catch
            {
                return RedirectToAction("ErrorConnect", "Home");
            }

            bool val1 = (System.Web.HttpContext.Current.User != null) && System.Web.HttpContext.Current.User.Identity.IsAuthenticated;
            if (!val1)
            {
                ViewBag.loggedUser = "(χωρίς σύνδεση)";
            }
            else
            {
                var data = (from d in db.USER_TEACHERS where d.USER_AFM == System.Web.HttpContext.Current.User.Identity.Name select d).FirstOrDefault();
                if (data != null)
                {
                    loggedTeacher = data;
                    ViewBag.loggedUser = loggedTeacher.USERNAME;
                    return RedirectToAction("Index", "TEACHERS");
                }
            }
            if (notify != null)
            {
                this.ShowMessage(MessageType.Info, notify);
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login([Bind(Include = "USERNAME,PASSWORD, CAPTCHATEXT")]  UserTeacherViewModel model)
        {
            var user = db.USER_TEACHERS.Where(u => u.USERNAME == model.USERNAME && u.PASSWORD == model.PASSWORD).FirstOrDefault();

            if (!ValidateCaptcha(model.CAPTCHATEXT))
            {
                ModelState.AddModelError("", "Το κείμενο δεν είναι ίδιο με αυτό της εικόνας. Δοκιμάστε πάλι.");
                return View(model);
            }

            if (user != null)
            {
                var roles = user.ROLES.Select(m => m.ROLE_NAME).ToArray();

                CustomPrincipalSerializeModel serializeModel = new CustomPrincipalSerializeModel();
                serializeModel.UserId = model.USER_ID;
                serializeModel.Username = model.USERNAME;
                serializeModel.Afm = model.USER_AFM;
                serializeModel.roles = roles;

                string userData = JsonConvert.SerializeObject(serializeModel);
                FormsAuthenticationTicket authTicket = new FormsAuthenticationTicket(
                    1, user.USER_AFM, DateTime.Now, DateTime.Now.AddMinutes(Kerberos.TICKET_TIMEOUT_MINUTES), false, userData);
                string encTicket = FormsAuthentication.Encrypt(authTicket);
                HttpCookie faCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encTicket);
                Response.Cookies.Add(faCookie);

                SetLoginStatus(user, true);
                return RedirectToAction("Index", "TEACHERS");
            }

            ModelState.AddModelError("", "Το όνομα χρήστη ή/και ο κωδ.πρόσβασης δεν είναι σωστά");
            return View(model);
        }

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register([Bind(Include = "USERNAME,PASSWORD,ConfirmPassword,AFM")] NewUserTeacherViewModel UserTeacher)
        {

            //// first, check if an account with the specified AFM already exists
            //if (AccountExists(UserTeacher.AFM))
            //{
            //    // show message and redirect to Manage
            //    // for account recovery
            //    return RedirectToAction("Manage");
            //}

            if (ModelState.IsValid)
            {
                USER_TEACHERS newUserTeacher = new USER_TEACHERS()
                {
                    USERNAME = UserTeacher.USERNAME,
                    PASSWORD = UserTeacher.PASSWORD,
                    USER_AFM = UserTeacher.AFM.Trim(),
                    ISACTIVE = false,
                    CREATEDATE = DateTime.Now
                };
                if (!Kerberos.existsUsername(UserTeacher.USERNAME))
                {
                    if (AitisiRules.CheckAFM(UserTeacher.AFM))
                    {
                        if (AccountExists(UserTeacher.AFM))
                        {
                            string msg = "Υπάρχει ήδη λογαριασμός με αυτό το ΑΦΜ.";
                            return RedirectToAction("Manage", new {  notify = msg });
                        }
                        else
                        {
                            if (!VerifyUsernamePassword(UserTeacher.USERNAME, UserTeacher.PASSWORD))
                            {
                                this.ShowMessage(MessageType.Error, "Το όνομα χρήστη και ο κωδικός δεν πρέπει να περιέχουν κενούς χαρακτήρες!");
                                return View(UserTeacher);
                            }
                            db.USER_TEACHERS.Add(newUserTeacher);
                            db.SaveChanges();
                            return RedirectToAction("Login", new { notify = "Η δημιουργία λογαριασμού ολοκληρώθηκε. Κάνετε χρήση των στοιχείων σας για είσοδο." });
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("AFM", "Το ΑΦΜ που δώσατε δεν είναι έγκυρο.");
                        return View(UserTeacher);
                    }
                }
                else
                {
                    ModelState.AddModelError("USERNAME", "Υπάρχει ήδη καταχωρημένος χρήστης με αυτό το Όνομα Χρήστη. Παρακαλώ διαλέξτε διαφορετικό Όνομα Χρήστη");
                    return View(UserTeacher);
                }
            }

            return View(UserTeacher);
        }

        public bool VerifyUsernamePassword(string username, string password)
        {
            if (Kerberos.CountSpaces(username) > 0 || Kerberos.CountSpaces(password) > 0) return false;
            else return true;
        }


        public ActionResult Manage(string notify = null)
        {
            if (notify != null)
            {
               this.ShowMessage(MessageType.Warning, notify);
            }
            return View();
        }

        [HttpPost]
        public ActionResult Manage(UserTeacherViewModel userTeacher)
        {
            // first, check if this account already exists
            //var user = db.USER_TEACHERS.Where(u => u.USER_AFM == UserTeacher.AFM).FirstOrDefault();

            if(userTeacher.USER_AFM == null || userTeacher.AMKA == null)
            {
                this.ShowMessage(MessageType.Warning, "Πρέπει να δώσετε ΑΦΜ και AMKA.");
                UserTeacherViewModel user = new UserTeacherViewModel();
                return View(user);
            }

            // Verify user with AFM and birthdate
            var teacher = (from t in db.TEACHERS where t.AFM == userTeacher.USER_AFM && t.AMKA == userTeacher.AMKA select t).FirstOrDefault();
            if (teacher == null)
            {
                this.ShowMessage(MessageType.Error, "Τα στοιχεία που δώσατε δεν βρέθηκαν!");
                UserTeacherViewModel user = new UserTeacherViewModel();
                return View(user);
            }
            else
            {
                UserTeacherViewModel user = (from u in db.USER_TEACHERS
                                             where u.USER_AFM == userTeacher.USER_AFM
                                             select new UserTeacherViewModel
                                             {
                                                 USERNAME = u.USERNAME,
                                                 PASSWORD = u.PASSWORD,
                                                 USER_AFM = u.USER_AFM,
                                             }).FirstOrDefault();

                if (user == null)
                {
                    return View("Error");
                }
                return View(user);
            }     
        }

        #endregion


        #region CAPTCHA HANDLERS NOT USED (08-05-2020)

        [AllowAnonymous]
        public ActionResult generateCaptcha()
        {
            System.Drawing.FontFamily family = new System.Drawing.FontFamily("Arial");
            CaptchaImage img = new CaptchaImage(180, 60, family);
            string text = img.CreateRandomText(6);   //+ " " + img.CreateRandomText(3);
            img.SetText(text);
            img.GenerateImage();
            img.Image.Save(Server.MapPath("~/CAPTCHA/") + this.Session.SessionID.ToString() + ".png", System.Drawing.Imaging.ImageFormat.Png);
            Session["captchaText"] = text;
            Session["filename"] = Server.MapPath("~/CAPTCHA/") + this.Session.SessionID.ToString() + ".png";
            return Json(Url.Content("~/CAPTCHA/") + this.Session.SessionID.ToString() + ".png?t=" + DateTime.Now.Ticks, JsonRequestBehavior.AllowGet);
        }

        public bool ValidateCaptcha(string strCaptcha)
        {
            string validText = Session["captchaText"].ToString();

            // delete the png file as no longer needed
            string completePath = Session["filename"].ToString();
            if ((System.IO.File.Exists(completePath))) System.IO.File.Delete(completePath);

            if (strCaptcha == validText) return true;

            return false;
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

        #endregion


        #region NEW USER-REGISTER USING TAXISnet

        // GET: RegisterUser <- accepts a random integer as Id of ΑΦΜ
        public ActionResult RegisterUser(int rnd_numberID = 0, string Afm = null)
        {
            ViewBag.loggedUser = "(χωρίς σύνδεση)";
            string userAFM = "";

            if (!string.IsNullOrEmpty(Afm))
                userAFM = Afm;
            else
                userAFM = GetTaxisnetAfm(rnd_numberID);

            if (string.IsNullOrEmpty(userAFM))
            {
                string msg = "Δεν βρέθηκαν στοιχεία εισόδου (ΑΦΜ) από το TAXISnet";
                return RedirectToAction("ErrorUser", "USER_TEACHERS", new { notify = msg });
            }
            UserTeacherViewModel model = GetUserTeacherFromDB(userAFM);
            if (model == null)
            {
                UserTeacherViewModel newmodel = new UserTeacherViewModel()
                {
                    USER_AFM = userAFM,
                    USERNAME = userAFM,
                    PASSWORD = "XXXXXXXXXX",
                    CREATEDATE = DateTime.Now
                };
                return View(newmodel);
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult RegisterUser(UserTeacherViewModel UserTeacher, int rnd_numberID = 0, string Afm = null)
        {
            string userAFM;

            if (!string.IsNullOrEmpty(Afm))
                userAFM = Afm;
            else
                userAFM = GetTaxisnetAfm(rnd_numberID);

            var user = GetUserTeacherFromDB(userAFM);
            if (user != null)
            {
                WriteUserCookie(user);
                DeleteTaxisRecord(rnd_numberID);

                return RedirectToAction("Index", "TEACHERS");
            }

            // User does not exist, so create one
            if (ModelState.IsValid)
            {
                USER_TEACHERS newUserParent = new USER_TEACHERS()
                {
                    USER_AFM = userAFM,
                    USERNAME = userAFM,
                    PASSWORD = "XXXXXXXXXX",
                    CREATEDATE = DateTime.Now
                };
                db.USER_TEACHERS.Add(newUserParent);
                db.SaveChanges();

                UserTeacherViewModel data = GetUserTeacherFromDB(userAFM);
                WriteUserCookie(data);
                DeleteTaxisRecord(rnd_numberID);

                return RedirectToAction("Index", "TEACHERS");
            }
            UserTeacher.USER_AFM = userAFM;
            UserTeacher.USERNAME = userAFM;
            UserTeacher.PASSWORD = "XXXXXXXXXX";
            return View(UserTeacher);
        }

        public void WriteUserCookie(UserTeacherViewModel user)
        {
            if (user != null)
            {
                CustomPrincipalSerializeModel serializeModel = new CustomPrincipalSerializeModel();
                serializeModel.UserId = user.USER_ID;
                serializeModel.Username = user.USERNAME;
                serializeModel.Afm = user.USER_AFM;

                string userData = JsonConvert.SerializeObject(serializeModel);
                FormsAuthenticationTicket authTicket = new FormsAuthenticationTicket(1, user.USER_AFM, 
                    DateTime.Now, DateTime.Now.AddMinutes(Kerberos.TICKET_TIMEOUT_MINUTES), false, userData);
                string encTicket = FormsAuthentication.Encrypt(authTicket);
                HttpCookie faCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encTicket);
                Response.Cookies.Add(faCookie);
            }
        }

        public void DeleteTaxisRecord(int rnd_numberID = 0)
        {
            var data = (from d in db.TAXISNET where d.RANDOM_NUMBER == rnd_numberID select d).FirstOrDefault();
            if (data == null)
                return;

            TAXISNET entity = db.TAXISNET.Find(data.TAXISNET_ID);
            if (entity != null)
            {
                db.Entry(entity).State = EntityState.Deleted;
                db.TAXISNET.Remove(entity);
                db.SaveChanges();
            };
        }

        public UserTeacherViewModel GetUserTeacherFromDB(string Afm)
        {
            var data = (from d in db.USER_TEACHERS
                        where d.USER_AFM == Afm
                        select new UserTeacherViewModel
                        {
                            USER_ID = d.USER_ID,
                            USER_AFM = d.USER_AFM,
                            USERNAME = d.USERNAME,
                            PASSWORD = d.PASSWORD,
                            CREATEDATE = d.CREATEDATE,
                        }).FirstOrDefault();
            return (data);
        }

        #endregion


        #region REDIRECTION TO EXTERNAL TAXISnet URL AND ERROR PAGES

        /// <summary>
        /// Redirect to TAXISnet Login Application. TODO
        /// </summary>
        /// <returns></returns>
        public ActionResult TaxisNetLogin()
        {
            // TEST: Link to Google maps
            //string address = "Laodikis 31";
            //string Area = "Glyfada";
            //string city = "Athens";
            //string zipCode = "16674";

            //var segment = string.Join(" ", address, Area, city, zipCode);
            //var escapedSegment = Uri.EscapeDataString(segment);

            //var baseFormat = "https://www.google.co.za/maps/search/{0}/";
            //var url = string.Format(baseFormat, escapedSegment);
            //return new RedirectResult(url);

            // ---- This is the actual Url and it works ------------
            string url = "http://auth.oaed.gr/Default.aspx?iek=true";
            return new RedirectResult(url);
        }

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

        #endregion


        #region USER-TEACHER SELECTOR FOR TESTING

        public ActionResult UserTeachersList()
        {
            string userTxt = "(χωρίς σύνδεση)";
            ViewBag.loggedUser = userTxt;

            List<sqlUserTeacherViewModel> data = GetUserTeacherListFromDB();

            return View(data);
        }

        public ActionResult UserTeacher_Read([DataSourceRequest] DataSourceRequest request)
        {
            var data = GetUserTeacherListFromDB();

            var result = new JsonResult();
            result.Data = data.ToDataSourceResult(request);
            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return result;
        }

        public List<sqlUserTeacherViewModel> GetUserTeacherListFromDB()
        {
            var users = (from a in db.sqlUSER_TEACHER_SELECT
                         orderby a.FULLNAME
                         select new sqlUserTeacherViewModel
                         {
                             USER_ID = a.USER_ID,
                             USER_AFM = a.USER_AFM,
                             CREATEDATE = a.CREATEDATE,
                             FULLNAME = a.FULLNAME,
                         }).ToList();
            return users;
        }

        #endregion


        #region GETTERS AND MISC (08/05/2020)

        public string GetTaxisnetAfm(int rnd_numberID = 0)
        {
            string safm = "";
            var data = (from d in db.TAXISNET where d.RANDOM_NUMBER == rnd_numberID select d).FirstOrDefault();
            if (data != null)
            {
                safm = data.TAXISNET_AFM;
            }
            return safm;
        }

        public bool AccountExists(string safm)
        {
            var user = db.USER_TEACHERS.Where(u => u.USER_AFM == safm).FirstOrDefault();

            if (user != null) return true;
            else return false;

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

        [AllowAnonymous]
        public ActionResult LogOut([Bind(Include = "ISACTIVE")] USER_TEACHERS userTeacher)
        {
            var user = db.USER_TEACHERS.Where(u => u.USERNAME == userTeacher.USERNAME && u.PASSWORD == userTeacher.PASSWORD).FirstOrDefault();

            FormsAuthentication.SignOut();

            // maybe we need to add that the profile is empty
            // so when the home page is displayed, it shows "No Connection"
            SetLoginStatus(user, true);
            return RedirectToAction("Index", "Home");
        }

        public void SetLoginStatus(USER_TEACHERS user, bool value)
        {
            db.Entry(user).State = EntityState.Modified;
            user.ISACTIVE = value;
            db.SaveChanges();
        }

        #endregion
    }
}
