using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Pegasus.BPM;
using Pegasus.DAL;
using Pegasus.Filters;
using Pegasus.Models;
using Pegasus.Notification;
using Pegasus.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace Pegasus.Controllers.SysControllers
{
    [ErrorHandlerFilter]
    public class TeacherUploadController : Controller
    {
        private readonly PegasusDBEntities db;
        private USER_TEACHERS loggedTeacher;
        private TEACHERS loggedTeacherData;

        private const string DOCUMENTS_PATH = "~/Uploads/Documents/";
        private const string TEACHING_PATH = "~/Uploads/Teaching/";
        private const string VOCATION_PATH = "~/Uploads/Vocation/";

        private const string DOCUMENTS_TEXT = "ΓΕΝΙΚΑ ΔΙΚΑΙΟΛΟΓΗΤΙΚΑ";
        private const string TEACHING_TEXT = "ΔΙΔΑΚΤΙΚΗ ΠΡΟΫΠΗΡΕΣΙΑ";
        private const string VOCATION_TEXT = "ΕΠΑΓΓΕΛΜΑΤΙΚΗ ΠΡΟΫΠΗΡΕΣΙΑ";
        // Max allowed upload filesize was set to 900 KB - now 6 MB
        private const int MAX_FILESIZE = 6000 * 1024;

        private readonly IUploadGeneralService uploadGeneralService;
        private readonly IUploadTeachingService uploadTeachingService;
        private readonly IUploadVocationService uploadVocationService;

        public TeacherUploadController(PegasusDBEntities entities, IUploadGeneralService uploadGeneralService, 
            IUploadTeachingService uploadTeachingService, IUploadVocationService uploadVocationService)
        {
            db = entities;

            this.uploadGeneralService = uploadGeneralService;
            this.uploadTeachingService = uploadTeachingService;
            this.uploadVocationService = uploadVocationService;
        }


        #region ΑΝΕΒΑΣΜΑ ΔΙΚΑΙΟΛΟΓΗΤΙΚΩΝ (2021-05-26)

        public ActionResult UploadDocuments()
        {
            bool val1 = (System.Web.HttpContext.Current.User != null) && System.Web.HttpContext.Current.User.Identity.IsAuthenticated;
            if (!val1)
            {
                ViewBag.loggedUser = "(χωρίς σύνδεση)";
                return RedirectToAction("TaxisNetLogin", "USER_TEACHERS");
            }
            else
            {
                loggedTeacher = GetLoginTeacher();
            }

            int prokirixiId = Common.GetOpenProkirixiID();
            if (prokirixiId == 0)
            {
                return RedirectToAction("Index", "TEACHERS", new { notify = "Δεν βρέθηκε ανοικτή Προκήρυξη." });
            }
            // first, find the teacher if he/she exists
            var data = (from d in db.TEACHERS where d.AFM == loggedTeacher.USER_AFM select d).FirstOrDefault();
            if (data == null)
            {
                string message = "Δεν βρέθηκε εκπαιδευτικός με αυτό το ΑΦΜ. Καταχωρήστε πρώτα τα ατομικά στοιχεία.";
                return RedirectToAction("Index", "TEACHERS", new { notify = message });
            }
            if (!AitisisExist())
            {
                string message = "Δεν βρέθηκαν αιτήσεις γι' αυτή την προκήρυξη. Καταχωρήστε πρώτα μία αίτηση.";
                return RedirectToAction("Index", "TEACHERS", new { notify = message });
            }

            PopulateAitisis();

            return View();
        }

        #endregion


        #region ΔΙΚΑΙΟΛΟΓΗΤΙΚΑ ΓΕΝΙΚΗΣ ΦΥΣΕΩΣ

        public ActionResult UploadGeneral()
        {
            bool val1 = (System.Web.HttpContext.Current.User != null) && System.Web.HttpContext.Current.User.Identity.IsAuthenticated;
            if (!val1)
            {
                ViewBag.loggedUser = "(χωρίς σύνδεση)";
                return RedirectToAction("TaxisNetLogin", "USER_TEACHERS");
            }
            else
            {
                loggedTeacher = GetLoginTeacher();
            }

            PopulateAitisis();
            return View();
        }

        #region GENERAL DOCUMENTS MASTER GRID

        public ActionResult UploadGeneral_Read([DataSourceRequest] DataSourceRequest request)
        {
            string AFM = GetLoginTeacher().USER_AFM;
            int prokirixiId = Common.GetOpenProkirixiID();

            List<UploadGeneralModel> data = uploadGeneralService.Read(prokirixiId, AFM);

            return Json(data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult UploadGeneral_Create([DataSourceRequest] DataSourceRequest request, UploadGeneralModel data)
        {
            string teacherAFM = GetLoginTeacher().USER_AFM;

            UploadGeneralModel newdata = new UploadGeneralModel();

            if (data != null && ModelState.IsValid)
            {
                uploadGeneralService.Create(data, teacherAFM);
                newdata = uploadGeneralService.Refresh(data.UploadID);
            }
            return Json(new[] { newdata }.ToDataSourceResult(request, ModelState), JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult UploadGeneral_Update([DataSourceRequest] DataSourceRequest request, UploadGeneralModel data)
        {
            string teacherAFM = GetLoginTeacher().USER_AFM;

            UploadGeneralModel newdata = new UploadGeneralModel();

            if (data != null & ModelState.IsValid)
            {
                uploadGeneralService.Update(data, teacherAFM);
                newdata = uploadGeneralService.Refresh(data.UploadID);
            }
            return Json(new[] { newdata }.ToDataSourceResult(request, ModelState), JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult UploadGeneral_Destroy([DataSourceRequest] DataSourceRequest request, UploadGeneralModel data)
        {
            if (data != null)
            {
                if (Kerberos.CanDeleteUploadGeneral(data.UploadID))
                {
                    uploadGeneralService.Destroy(data);
                }
                else
                {
                    ModelState.AddModelError("", "Δεν μπορεί να γίνει διαγραφή της εγγραφής διότι έχει συνημμένα αρχεία. Διαγράψτε πρώτα τα αρχεία.");
                }
            }
            return Json(new[] { data }.ToDataSourceResult(request, ModelState), JsonRequestBehavior.AllowGet);
        }

        #endregion


        #region UPLOADED FILES CHILD GRID

        public ActionResult UploadGeneralFiles_Read([DataSourceRequest] DataSourceRequest request, int uploadId = 0)
        {
            List<UploadGeneralFilesModel> data = uploadGeneralService.ReadFiles(uploadId);

            return Json(data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult UploadGeneralFiles_Destroy([DataSourceRequest] DataSourceRequest request, UploadGeneralFilesModel data)
        {
            if (data != null)
            {
                // First delete the physical file and then the info record. Important!
                DeleteGeneralUploadedFile(data.FileID);

                uploadGeneralService.DestroyFile(data);
            }
            return Json(new[] { data }.ToDataSourceResult(request, ModelState), JsonRequestBehavior.AllowGet);
        }

        public void DeleteGeneralUploadedFile(int fileId)
        {
            loggedTeacher = GetLoginTeacher();
            string uploadPath = DOCUMENTS_PATH + loggedTeacher.USER_AFM + "/";
            string filename = "";

            var data = (from d in db.UploadGeneralFiles where d.FileID == fileId select d).FirstOrDefault();
            if (data != null)
            {
                filename = data.FileName;
                var physicalPath = Path.Combine(Server.MapPath(uploadPath), filename);

                if (System.IO.File.Exists(physicalPath))
                {
                    System.IO.File.Delete(physicalPath);
                }
            }
        }

        public FileResult DownloadGeneralFile(int file_id)
        {
            loggedTeacher = GetLoginTeacher();
            string physicalPath = DOCUMENTS_PATH + loggedTeacher.USER_AFM + "/";
            string filename = "";

            var fileinfo = (from d in db.UploadGeneralFiles where d.FileID == file_id select d).FirstOrDefault();
            if (fileinfo != null)
            {
                filename = fileinfo.FileName;
            }

            return File(Path.Combine(Server.MapPath(physicalPath), filename), System.Net.Mime.MediaTypeNames.Application.Octet, filename);
        }

        #endregion


        #region UPLOAD FORM WITH SAVE-REMOVE ACTIONS

        public ActionResult UploadGeneralForm(int uploadId, string notify = null)
        {
            bool val1 = (System.Web.HttpContext.Current.User != null) && System.Web.HttpContext.Current.User.Identity.IsAuthenticated;
            if (!val1)
            {
                ViewBag.loggedUser = "(χωρίς σύνδεση)";
                return RedirectToAction("TaxisNetLogin", "USER_TEACHERS");
            }
            else
            {
                loggedTeacher = GetLoginTeacher();
            }
            if (notify != null)
                this.ShowMessage(MessageType.Warning, notify);

            if (!(uploadId > 0))
            {
                string msg = "Άκυρος κωδικός μεταφόρτωσης. Πρέπει πρώτα να αποθηκεύσετε την εγγραφή και μετά να ανεβάσετε αρχείο.";
                return RedirectToAction("ErrorData", "TeacherUpload", new { notify = msg });
            }
            ViewData["uploadId"] = uploadId;

            return View();
        }

        public ActionResult GeneralFiles_Upload(IEnumerable<HttpPostedFileBase> files, int uploadId = 0)
        {
            loggedTeacher = GetLoginTeacher();
            string uploadPath = DOCUMENTS_PATH + loggedTeacher.USER_AFM + "/";

            try
            {
                if (!Directory.Exists(Server.MapPath(uploadPath)))
                    Directory.CreateDirectory(Server.MapPath(uploadPath));

                if (files != null)
                {
                    foreach (var file in files)
                    {
                        // Some browsers send file names with full path. We are only interested in the file name.
                        if (file.ContentLength > 0)
                        {
                            var fileName = Path.GetFileName(file.FileName);
                            var fileExtension = Path.GetExtension(fileName);
                            if (!Kerberos.ValidFileExtension(fileExtension))
                            {
                                string msg = "Μη επιτρεπόμενος τύπος αρχείου. Δοκιμάστε πάλι.";
                                return Content(msg);
                            }
                            if (file.ContentLength > MAX_FILESIZE)
                            {
                                string msg = "Το μέγεθος κάθε αρχείου δεν μπορεί να υπερβαίνει τα 6 MB. Δοκιμάστε πάλι.";
                                return Content(msg);
                            }
                            var physicalPath = Path.Combine(Server.MapPath(uploadPath), fileName);
                            file.SaveAs(physicalPath);
                            Thread.Sleep(500);

                            if (System.IO.File.Exists(physicalPath))
                            {
                                UploadGeneralFiles fileDetail = new UploadGeneralFiles()
                                {
                                    FileName = fileName,
                                    TeacherAFM = loggedTeacher.USER_AFM,
                                    Category = DOCUMENTS_TEXT,
                                    UploadID = uploadId
                                };
                                db.UploadGeneralFiles.Add(fileDetail);
                                db.SaveChanges();
                            }
                            else
                            {
                                string msg = "Το αρχείο " + fileName + " δεν βρέθηκε στο δίσκο του εξυπηρετητή. Δοκιμάστε πάλι να το ανεβάσετε.";
                                return Content(msg);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string msg = "Παρουσιάστηκε σφάλμα στη μεταφόρτωση:<br/>" + ex.Message;
                return Content(msg);
            }
            // Return an empty string to signify success
            return Content("");
        }

        public ActionResult GeneralFiles_Remove(string[] fileNames, int uploadId)
        {
            // The parameter of the Remove action must be called "fileNames"
            loggedTeacher = GetLoginTeacher();
            string uploadPath = DOCUMENTS_PATH + loggedTeacher.USER_AFM + "/";

            if (fileNames != null)
            {
                foreach (var fullName in fileNames)
                {
                    var fileName = Path.GetFileName(fullName);
                    var extension = Path.GetExtension(fileName);

                    var physicalPath = Path.Combine(Server.MapPath(uploadPath), fileName);
                    if (System.IO.File.Exists(physicalPath))
                    {
                        System.IO.File.Delete(physicalPath);
                        DeleteGeneralUploadFileRecord(fileName);
                    }
                }
            }
            // Return an empty string to signify success
            return Content("");
        }

        public ActionResult DeleteGeneralUploadFileRecord(string filename)
        {
            UploadGeneralFiles entity = db.UploadGeneralFiles.Where(d => d.FileName == filename).FirstOrDefault();
            if (entity != null)
            {
                db.Entry(entity).State = EntityState.Deleted;
                db.UploadGeneralFiles.Remove(entity);
                db.SaveChanges();
            }
            return Content("");
        }

        #endregion

        #endregion


        #region  ΔΙΚΑΙΟΛΟΓΗΤΙΚΑ ΔΙΔΑΚΤΙΚΗΣ ΕΜΠΕΙΡΙΑΣ

        public ActionResult UploadTeaching()
        {
            bool val1 = (System.Web.HttpContext.Current.User != null) && System.Web.HttpContext.Current.User.Identity.IsAuthenticated;
            if (!val1)
            {
                ViewBag.loggedUser = "(χωρίς σύνδεση)";
                return RedirectToAction("TaxisNetLogin", "USER_TEACHERS");
            }
            else
            {
                loggedTeacher = GetLoginTeacher();
            }

            PopulateAitisis();

            return View();
        }

        #region TEACHING DOCUMENTS MASTER GRID

        public ActionResult UploadTeaching_Read([DataSourceRequest] DataSourceRequest request)
        {
            string AFM = GetLoginTeacher().USER_AFM;
            int prokirixiId = Common.GetOpenProkirixiID();

            var data = uploadTeachingService.Read(prokirixiId, AFM);

            return Json(data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult UploadTeaching_Create([DataSourceRequest] DataSourceRequest request, UploadTeachingModel data)
        {
            string teacherAFM = GetLoginTeacher().USER_AFM;

            UploadTeachingModel newdata = new UploadTeachingModel();

            if (data != null && ModelState.IsValid)
            {
                uploadTeachingService.Create(data, teacherAFM);
                newdata = uploadTeachingService.Refresh(data.UploadID);
            }
            return Json(new[] { newdata }.ToDataSourceResult(request, ModelState), JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult UploadTeaching_Update([DataSourceRequest] DataSourceRequest request, UploadTeachingModel data)
        {
            string teacherAFM = GetLoginTeacher().USER_AFM;

            UploadTeachingModel newdata = new UploadTeachingModel();

            if (data != null & ModelState.IsValid)
            {
                uploadTeachingService.Update(data, teacherAFM);
                newdata = uploadTeachingService.Refresh(data.UploadID);
            }
            return Json(new[] { newdata }.ToDataSourceResult(request, ModelState), JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult UploadTeaching_Destroy([DataSourceRequest] DataSourceRequest request, UploadTeachingModel data)
        {
            if (data != null)
            {
                UploadTeaching entity = db.UploadTeaching.Find(data.UploadID);
                if (entity != null)
                {
                    if (Kerberos.CanDeleteUploadTeaching(data.UploadID))
                    {
                        db.Entry(entity).State = EntityState.Deleted;
                        db.UploadTeaching.Remove(entity);
                        db.SaveChanges();
                    }
                    else
                        ModelState.AddModelError("", "Δεν μπορεί να γίνει διαγραφή της εγγραφής διότι έχει συσχετισμένα ανεβασμένα αρχεία. Διαγράψτε πρώτα τα αρχεία.");
                }
            }
            return Json(new[] { data }.ToDataSourceResult(request, ModelState), JsonRequestBehavior.AllowGet);
        }

        #endregion


        #region UPLOADED FILES CHILD GRID

        public ActionResult UploadTeachingFiles_Read([DataSourceRequest] DataSourceRequest request, int uploadId = 0)
        {
            List<UploadTeachingFilesModel> data = uploadTeachingService.ReadFiles(uploadId);

            return Json(data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult UploadTeachingFiles_Destroy([DataSourceRequest] DataSourceRequest request, UploadTeachingFilesModel data)
        {
            if (data != null)
            {
                // First delete the physical file and then the info record. Important!
                DeleteTeachingUploadedFile(data.FileID);

                uploadTeachingService.DestroyFile(data);
            }
            return Json(new[] { data }.ToDataSourceResult(request, ModelState), JsonRequestBehavior.AllowGet);
        }

        public void DeleteTeachingUploadedFile(int fileId)
        {
            loggedTeacher = GetLoginTeacher();
            string uploadPath = TEACHING_PATH + loggedTeacher.USER_AFM + "/";
            string filename = "";

            var data = (from d in db.UploadTeachingFiles where d.FileID == fileId select d).FirstOrDefault();
            if (data != null)
            {
                filename = data.FileName;
                var physicalPath = Path.Combine(Server.MapPath(uploadPath), filename);

                if (System.IO.File.Exists(physicalPath))
                {
                    System.IO.File.Delete(physicalPath);
                }
            }
        }

        public FileResult DownloadTeachingFile(int file_id)
        {
            loggedTeacher = GetLoginTeacher();
            string physicalPath = TEACHING_PATH + loggedTeacher.USER_AFM + "/";
            string filename = "";

            var fileinfo = (from d in db.UploadTeachingFiles where d.FileID == file_id select d).FirstOrDefault();
            if (fileinfo != null)
            {
                filename = fileinfo.FileName;
            }

            return File(Path.Combine(Server.MapPath(physicalPath), filename), System.Net.Mime.MediaTypeNames.Application.Octet, filename);
        }

        #endregion


        #region UPLOAD FORM WITH SAVE-REMOVE ACTIONS

        public ActionResult UploadTeachingForm(int uploadId, string notify = null)
        {
            bool val1 = (System.Web.HttpContext.Current.User != null) && System.Web.HttpContext.Current.User.Identity.IsAuthenticated;
            if (!val1)
            {
                ViewBag.loggedUser = "(χωρίς σύνδεση)";
                return RedirectToAction("TaxisNetLogin", "USER_TEACHERS");
            }
            else
            {
                loggedTeacher = GetLoginTeacher();
            }
            if (notify != null)
                this.ShowMessage(MessageType.Warning, notify);

            if (!(uploadId > 0))
            {
                string msg = "Άκυρος κωδικός μεταφόρτωσης. Πρέπει πρώτα να αποθηκεύσετε την εγγραφή και μετά να ανεβάσετε αρχείο.";
                return RedirectToAction("ErrorData", "TeacherUpload", new { notify = msg });
            }
            ViewData["uploadId"] = uploadId;

            return View();
        }

        public ActionResult TeachingFiles_Upload(IEnumerable<HttpPostedFileBase> files, int uploadId = 0)
        {
            loggedTeacher = GetLoginTeacher();
            string uploadPath = TEACHING_PATH + loggedTeacher.USER_AFM + "/";

            try
            {
                if (!Directory.Exists(Server.MapPath(uploadPath)))
                    Directory.CreateDirectory(Server.MapPath(uploadPath));

                if (files != null)
                {
                    foreach (var file in files)
                    {
                        // Some browsers send file names with full path. We are only interested in the file name.
                        if (file.ContentLength > 0)
                        {
                            var fileName = Path.GetFileName(file.FileName);
                            var fileExtension = Path.GetExtension(fileName);
                            if (!Kerberos.ValidFileExtension(fileExtension))
                            {
                                string msg = "Μη επιτρεπόμενος τύπος αρχείου. Δοκιμάστε πάλι.";
                                return Content(msg);
                            }
                            if (file.ContentLength > MAX_FILESIZE)
                            {
                                string msg = "Το μέγεθος κάθε αρχείου δεν μπορεί να υπερβαίνει τα 6 MB. Δοκιμάστε πάλι.";
                                return Content(msg);
                            }
                            var physicalPath = Path.Combine(Server.MapPath(uploadPath), fileName);
                            file.SaveAs(physicalPath);
                            Thread.Sleep(500);

                            if (System.IO.File.Exists(physicalPath))
                            {
                                UploadTeachingFiles fileDetail = new UploadTeachingFiles()
                                {
                                    FileName = fileName,
                                    TeacherAFM = loggedTeacher.USER_AFM,
                                    Category = TEACHING_TEXT,
                                    UploadID = uploadId
                                };
                                db.UploadTeachingFiles.Add(fileDetail);
                                db.SaveChanges();
                            }
                            else
                            {
                                string msg = "Το αρχείο " + fileName + " δεν βρέθηκε στο δίσκο του εξυπηρετητή. Δοκιμάστε πάλι να το ανεβάσετε.";
                                return Content(msg);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string msg = "Παρουσιάστηκε σφάλμα στη μεταφόρτωση:<br/>" + ex.Message;
                return Content(msg);
            }
            // Return an empty string to signify success
            return Content("");
        }

        public ActionResult TeachingFiles_Remove(string[] fileNames, int uploadId)
        {
            // The parameter of the Remove action must be called "fileNames"
            loggedTeacher = GetLoginTeacher();
            string uploadPath = TEACHING_PATH + loggedTeacher.USER_AFM + "/";

            if (fileNames != null)
            {
                foreach (var fullName in fileNames)
                {
                    var fileName = Path.GetFileName(fullName);
                    var extension = Path.GetExtension(fileName);

                    var physicalPath = Path.Combine(Server.MapPath(uploadPath), fileName);
                    if (System.IO.File.Exists(physicalPath))
                    {
                        System.IO.File.Delete(physicalPath);
                        DeleteTeachingUploadFileRecord(fileName);
                    }
                }
            }
            // Return an empty string to signify success
            return Content("");
        }

        public ActionResult DeleteTeachingUploadFileRecord(string filename)
        {
            UploadTeachingFiles entity = db.UploadTeachingFiles.Where(d => d.FileName == filename).FirstOrDefault();
            if (entity != null)
            {
                db.Entry(entity).State = EntityState.Deleted;
                db.UploadTeachingFiles.Remove(entity);
                db.SaveChanges();
            }
            return Content("");
        }

        #endregion

        #endregion


        #region  ΔΙΚΑΙΟΛΟΓΗΤΙΚΑ ΕΠΑΓΓΕΛΜΑΤΙΚΗΣ ΕΜΠΕΙΡΙΑΣ

        public ActionResult UploadVocation()
        {
            bool val1 = (System.Web.HttpContext.Current.User != null) && System.Web.HttpContext.Current.User.Identity.IsAuthenticated;
            if (!val1)
            {
                ViewBag.loggedUser = "(χωρίς σύνδεση)";
                return RedirectToAction("TaxisNetLogin", "USER_TEACHERS");
            }
            else
            {
                loggedTeacher = GetLoginTeacher();
            }

            PopulateAitisis();

            return View();
        }


        #region VOCATION DOCUMENTS MASTER GRID

        public ActionResult UploadVocation_Read([DataSourceRequest] DataSourceRequest request)
        {
            string AFM = GetLoginTeacher().USER_AFM;
            int prokirixiId = Common.GetOpenProkirixiID();

            List<UploadVocationModel> data = uploadVocationService.Read(prokirixiId, AFM);

            return Json(data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult UploadVocation_Create([DataSourceRequest] DataSourceRequest request, UploadVocationModel data)
        {
            string teacherAFM = GetLoginTeacher().USER_AFM;

            UploadVocationModel newdata = new UploadVocationModel();

            if (data != null && ModelState.IsValid)
            {
                uploadVocationService.Create(data, teacherAFM);
                newdata = uploadVocationService.Refresh(data.UploadID);
            }
            return Json(new[] { newdata }.ToDataSourceResult(request, ModelState), JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult UploadVocation_Update([DataSourceRequest] DataSourceRequest request, UploadVocationModel data)
        {
            string teacherAFM = GetLoginTeacher().USER_AFM;

            UploadVocationModel newdata = new UploadVocationModel();

            if (data != null & ModelState.IsValid)
            {
                uploadVocationService.Update(data, teacherAFM);
                newdata = uploadVocationService.Refresh(data.UploadID);
            }
            return Json(new[] { newdata }.ToDataSourceResult(request, ModelState), JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult UploadVocation_Destroy([DataSourceRequest] DataSourceRequest request, UploadVocationModel data)
        {
            if (data != null)
            {
                if (Kerberos.CanDeleteUploadVocation(data.UploadID))
                {
                    uploadVocationService.Destroy(data);
                }
                else
                {
                    ModelState.AddModelError("", "Δεν μπορεί να γίνει διαγραφή της εγγραφής διότι έχει συσχετισμένα ανεβασμένα αρχεία. Διαγράψτε πρώτα τα αρχεία.");
                }
            }
            return Json(new[] { data }.ToDataSourceResult(request, ModelState), JsonRequestBehavior.AllowGet);
        }

        #endregion


        #region UPLOADED FILES CHILD GRID

        public ActionResult UploadVocationFiles_Read([DataSourceRequest] DataSourceRequest request, int uploadId = 0)
        {
            List<UploadVocationFilesModel> data = uploadVocationService.ReadFiles(uploadId);

            return Json(data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult UploadVocationFiles_Destroy([DataSourceRequest] DataSourceRequest request, UploadVocationFilesModel data)
        {
            if (data != null)
            {
                // First delete the physical file and then the info record. Important!
                DeleteVocationUploadedFile(data.FileID);

                uploadVocationService.DestroyFile(data);
            }
            return Json(new[] { data }.ToDataSourceResult(request, ModelState), JsonRequestBehavior.AllowGet);
        }

        public void DeleteVocationUploadedFile(int fileId)
        {
            loggedTeacher = GetLoginTeacher();
            string uploadPath = VOCATION_PATH + loggedTeacher.USER_AFM + "/";
            string filename = "";

            var data = (from d in db.UploadVocationFiles where d.FileID == fileId select d).FirstOrDefault();
            if (data != null)
            {
                filename = data.FileName;
                var physicalPath = Path.Combine(Server.MapPath(uploadPath), filename);

                if (System.IO.File.Exists(physicalPath))
                {
                    System.IO.File.Delete(physicalPath);
                }
            }
        }

        public FileResult DownloadVocationFile(int file_id)
        {
            loggedTeacher = GetLoginTeacher();
            string physicalPath = VOCATION_PATH + loggedTeacher.USER_AFM + "/";
            string filename = "";

            var fileinfo = (from d in db.UploadVocationFiles where d.FileID == file_id select d).FirstOrDefault();
            if (fileinfo != null)
            {
                filename = fileinfo.FileName;
            }

            return File(Path.Combine(Server.MapPath(physicalPath), filename), System.Net.Mime.MediaTypeNames.Application.Octet, filename);
        }

        #endregion


        #region UPLOAD FORM WITH SAVE-REMOVE ACTIONS

        public ActionResult UploadVocationForm(int uploadId, string notify = null)
        {
            bool val1 = (System.Web.HttpContext.Current.User != null) && System.Web.HttpContext.Current.User.Identity.IsAuthenticated;
            if (!val1)
            {
                ViewBag.loggedUser = "(χωρίς σύνδεση)";
                return RedirectToAction("TaxisNetLogin", "USER_TEACHERS");
            }
            else
            {
                loggedTeacher = GetLoginTeacher();
            }
            if (notify != null)
                this.ShowMessage(MessageType.Warning, notify);

            if (!(uploadId > 0))
            {
                string msg = "Άκυρος κωδικός μεταφόρτωσης. Πρέπει πρώτα να αποθηκεύσετε την εγγραφή και μετά να ανεβάσετε αρχείο.";
                return RedirectToAction("ErrorData", "TeacherUpload", new { notify = msg });
            }
            ViewData["uploadId"] = uploadId;

            return View();
        }

        public ActionResult VocationFiles_Upload(IEnumerable<HttpPostedFileBase> files, int uploadId = 0)
        {
            loggedTeacher = GetLoginTeacher();
            string uploadPath = VOCATION_PATH + loggedTeacher.USER_AFM + "/";

            try
            {
                if (!Directory.Exists(Server.MapPath(uploadPath)))
                    Directory.CreateDirectory(Server.MapPath(uploadPath));

                if (files != null)
                {
                    foreach (var file in files)
                    {
                        // Some browsers send file names with full path. We are only interested in the file name.
                        if (file.ContentLength > 0)
                        {
                            var fileName = Path.GetFileName(file.FileName);
                            var fileExtension = Path.GetExtension(fileName);
                            if (!Kerberos.ValidFileExtension(fileExtension))
                            {
                                string msg = "Μη επιτρεπόμενος τύπος αρχείου. Δοκιμάστε πάλι.";
                                return Content(msg);
                            }
                            if (file.ContentLength > MAX_FILESIZE)
                            {
                                string msg = "Το μέγεθος κάθε αρχείου δεν μπορεί να υπερβαίνει τα 6 MB. Δοκιμάστε πάλι.";
                                return Content(msg);
                            }
                            var physicalPath = Path.Combine(Server.MapPath(uploadPath), fileName);
                            file.SaveAs(physicalPath);
                            Thread.Sleep(500);

                            if (System.IO.File.Exists(physicalPath))
                            {
                                UploadVocationFiles fileDetail = new UploadVocationFiles()
                                {
                                    FileName = fileName,
                                    TeacherAFM = loggedTeacher.USER_AFM,
                                    Category = VOCATION_TEXT,
                                    UploadID = uploadId
                                };
                                db.UploadVocationFiles.Add(fileDetail);
                                db.SaveChanges();
                            }
                            else
                            {
                                string msg = "Το αρχείο " + fileName + " δεν βρέθηκε στο δίσκο του εξυπηρετητή. Δοκιμάστε πάλι να το ανεβάσετε.";
                                return Content(msg);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string msg = "Παρουσιάστηκε σφάλμα στη μεταφόρτωση:<br/>" + ex.Message;
                return Content(msg);
            }
            // Return an empty string to signify success
            return Content("");
        }

        public ActionResult VocationFiles_Remove(string[] fileNames, int uploadId)
        {
            // The parameter of the Remove action must be called "fileNames"
            loggedTeacher = GetLoginTeacher();
            string uploadPath = VOCATION_PATH + loggedTeacher.USER_AFM + "/";

            if (fileNames != null)
            {
                foreach (var fullName in fileNames)
                {
                    var fileName = Path.GetFileName(fullName);
                    var extension = Path.GetExtension(fileName);

                    var physicalPath = Path.Combine(Server.MapPath(uploadPath), fileName);
                    if (System.IO.File.Exists(physicalPath))
                    {
                        System.IO.File.Delete(physicalPath);
                        DeleteVocationUploadFileRecord(fileName);
                    }
                }
            }
            // Return an empty string to signify success
            return Content("");
        }

        public ActionResult DeleteVocationUploadFileRecord(string filename)
        {
            UploadVocationFiles entity = db.UploadVocationFiles.Where(d => d.FileName == filename).FirstOrDefault();
            if (entity != null)
            {
                db.Entry(entity).State = EntityState.Deleted;
                db.UploadVocationFiles.Remove(entity);
                db.SaveChanges();
            }
            return Content("");
        }

        #endregion

        #endregion


        #region MISCELLANEOUS FUNCTIONS

        public void PopulateAitisis()
        {
            int prokirixiId = Common.GetOpenProkirixiID();
            loggedTeacher = GetLoginTeacher();

            string teacherAfm = db.TEACHERS.Find(loggedTeacher.USER_AFM).AFM;
            var aitiseis = (from d in db.AITISIS where d.PROKIRIXI_ID == prokirixiId && d.AFM == teacherAfm select d).ToList();

            ViewData["aitiseis"] = aitiseis;
            ViewData["defaultAitisi"] = aitiseis.First().AITISI_ID;
        }

        public bool AitisisExist()
        {
            string teacherAfm = GetLoginTeacher().USER_AFM;
            int prokirixiId = Common.GetOpenProkirixiID(active: true);

            if (teacherAfm != null)
            {
                var aitiseis = (from d in db.AITISIS where d.PROKIRIXI_ID == prokirixiId && d.AFM == teacherAfm select d).ToList();
                if (aitiseis.Count > 0)
                    return true;
            }
            return false;
        }

        public USER_TEACHERS GetLoginTeacher()
        {
            loggedTeacher = db.USER_TEACHERS.Where(m => m.USER_AFM == System.Web.HttpContext.Current.User.Identity.Name).FirstOrDefault();
            ViewBag.loggedUser = loggedTeacher.USERNAME;

            loggedTeacherData = db.TEACHERS.Find(loggedTeacher.USER_AFM);

            ViewBag.loggedTeacher = loggedTeacherData;
            if (loggedTeacherData != null)
            {
                if (!string.IsNullOrEmpty(loggedTeacherData.FIRSTNAME) && !string.IsNullOrEmpty(loggedTeacherData.LASTNAME))
                {
                    ViewBag.loggedUser = loggedTeacherData.FIRSTNAME + " " + loggedTeacherData.LASTNAME;
                }
            }
            return loggedTeacher;
        }

        public ActionResult Error(string notify = null)
        {
            if (notify != null) this.ShowMessage(MessageType.Warning, notify);

            return View();
        }

        public ActionResult ErrorData(string notify = null)
        {
            if (notify != null) this.ShowMessage(MessageType.Warning, notify);

            return View();
        }

        #endregion
    }
}