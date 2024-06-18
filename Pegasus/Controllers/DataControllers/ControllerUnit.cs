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
using Pegasus.DAL;
using Pegasus.Models;
using Pegasus.BPM;
using Pegasus.Notification;
using Pegasus.Extensions;
using Pegasus.Services;

namespace Pegasus.Controllers.DataControllers
{
    public class ControllerUnit : Controller
    {
        private readonly PegasusDBEntities db;
        private USER_ADMINS loggedAdmin;
        private USER_SCHOOLS loggedSchool;

        public const string DOCUMENTS_PATH = "~/Uploads/Documents/";
        public const string TEACHING_PATH = "~/Uploads/Teaching/";
        public const string VOCATION_PATH = "~/Uploads/Vocation/";

        public JavaScriptSerializer jss = new JavaScriptSerializer()
        {
            MaxJsonLength = int.MaxValue
        };

        public ControllerUnit(PegasusDBEntities entities)
        {
            db = entities;
        }


        #region GLOBAL AITISEIS READER

        public IEnumerable<sqlTeacherAitiseisModel> ReadAitiseis(int prokirixiId, int schoolId = 0)
        {
            if (schoolId > 0)
            {
                var data = (from d in db.sqlTEACHER_AITISEIS
                            where d.PROKIRIXI_ID == prokirixiId && d.SCHOOL_ID == schoolId
                            orderby d.FULLNAME, d.SCHOOL_NAME
                            select new sqlTeacherAitiseisModel
                            {
                                AITISI_ID = d.AITISI_ID,
                                AITISI_PROTOCOL = d.AITISI_PROTOCOL,
                                AFM = d.AFM,
                                CHECK_STATUS = d.CHECK_STATUS ?? false,
                                EIDIKOTITA_TEXT = d.EIDIKOTITA_TEXT,
                                FULLNAME = d.FULLNAME,
                                PERIFERIAKI = d.PERIFERIAKI,
                                PERIFERIA_NAME = d.PERIFERIA_NAME,
                                SCHOOL_NAME = d.SCHOOL_NAME,
                                PROTOCOL = d.PROTOCOL
                            }).ToList();
                return data;
            }
            else
            {
                var data = (from d in db.sqlTEACHER_AITISEIS
                            where d.PROKIRIXI_ID == prokirixiId
                            orderby d.FULLNAME, d.SCHOOL_NAME
                            select new sqlTeacherAitiseisModel
                            {
                                AITISI_ID = d.AITISI_ID,
                                AITISI_PROTOCOL = d.AITISI_PROTOCOL,
                                AFM = d.AFM,
                                CHECK_STATUS = d.CHECK_STATUS ?? false,
                                EIDIKOTITA_TEXT = d.EIDIKOTITA_TEXT,
                                FULLNAME = d.FULLNAME,
                                PERIFERIAKI = d.PERIFERIAKI,
                                PERIFERIA_NAME = d.PERIFERIA_NAME,
                                SCHOOL_NAME = d.SCHOOL_NAME,
                                PROTOCOL = d.PROTOCOL
                            }).ToList();
                return data;
            }
        }

        #endregion


        #region POPULATORS

        public void PopulateEidikotites()
        {
            var data = (from d in db.VD_EIDIKOTITES
                        orderby d.EIDIKOTITA_KLADOS_ID, d.EIDIKOTITA_DESC
                        select d).ToList();

            ViewData["eidikotites"] = data;
        }

        public void PopulateSchools()
        {
            var schools = (from d in db.SYS_SCHOOLS orderby d.SCHOOL_NAME where d.SCHOOL_TYPEID == 2 select d).ToList();
            ViewData["schools"] = schools;
            ViewData["defaultSchool"] = schools.First().SCHOOL_ID;
        }

        public void PopulateSchoolTypes()
        {
            var schooltypes = (from t in db.SYS_SCHOOLTYPES where t.SCHOOL_TYPE_ID != 1 select t).ToList();
            ViewData["schooltypes"] = schooltypes;
        }

        public void PopulatePeriferies()
        {
            var periferies = (from periferia in db.SYS_PERIFERIES select periferia).ToList();
            ViewData["Periferies"] = periferies;
        }

        public void PopulatePeriferiesByUser()
        {
            bool val1 = (System.Web.HttpContext.Current.User != null) && System.Web.HttpContext.Current.User.Identity.IsAuthenticated;
            if (!val1)
            {
                ViewData["AitisiPeriferies"] = new List<SYS_PERIFERIES>();
            }
            else
            {
                TEACHERS teacher = db.TEACHERS.Where(m => m.AFM == System.Web.HttpContext.Current.User.Identity.Name).FirstOrDefault();
                var teacherAitisisPeriferiesIds = (from aitisis in db.AITISIS where aitisis.AFM == teacher.AFM select aitisis.PERIFERIA_ID);
                var peririferiesInTeacherAitisis = (from periferia in db.SYS_PERIFERIES
                                                    where teacherAitisisPeriferiesIds.Contains(periferia.PERIFERIA_ID)
                                                    select periferia).ToList();
                ViewData["AitisiPeriferies"] = peririferiesInTeacherAitisis;
            }
        }

        public void PopulateSchoolTypes(int aitisiID)
        {
            //Εύρεση ειδικότητας της αίτησης
            var eidikotitaAitisis = (from a in db.AITISIS where a.AITISI_ID == aitisiID select a.EIDIKOTITA).FirstOrDefault();
            //Εύρεση ενεργής προκύρηξης
            var activeProkirixi = (from p in db.PROKIRIXIS where p.ADMIN == true select p.ID).FirstOrDefault();
            //Εύρεση προκυρησόμενων σχολείων με την ειδικότητα της αίτησης
            var prokirixiSchools = (from p in db.PROKIRIXIS_EIDIKOTITES 
                                    where p.PROKIRIXI_ID == activeProkirixi && p.EIDIKOTITA_ID == eidikotitaAitisis 
                                    select p.SCHOOL_ID).ToList();
            //Εύρεση περιφέριας της αίτησης
            var periferiaAitisis = (from a in db.AITISIS where a.AITISI_ID == aitisiID select a.PERIFERIA_ID).FirstOrDefault();

            var filteredSchoolsTypes = (from s in db.SYS_SCHOOLS
                                        where prokirixiSchools.Contains(s.SCHOOL_ID) && periferiaAitisis == s.SCHOOL_PERIFERIA_ID
                                        select s.SCHOOL_TYPEID).ToList();
            var schooltypes = (from t in db.SYS_SCHOOLTYPES where filteredSchoolsTypes.Contains(t.SCHOOL_TYPE_ID) select t).ToList();
            ViewData["schooltypes"] = schooltypes;
            ViewData["defaultSchoolType"] = schooltypes.First().SCHOOL_TYPE_ID;
        }

        public void PopulateSchools(int aitisiID)
        {
            //Εύρεση ειδικότητας της αίτησης
            var eidikotitaAitisis = (from a in db.AITISIS where a.AITISI_ID == aitisiID select a.EIDIKOTITA).FirstOrDefault();
            //Εύρεση ενεργής προκύρηξης
            var activeProkirixi = (from p in db.PROKIRIXIS where p.ADMIN == true select p.ID).FirstOrDefault();
            //Εύρεση προκυρησόμενων σχολείων με την ειδικότητα της αίτησης
            var prokirixiSchools = (from p in db.PROKIRIXIS_EIDIKOTITES 
                                    where p.PROKIRIXI_ID == activeProkirixi && p.EIDIKOTITA_ID == eidikotitaAitisis 
                                    select p.SCHOOL_ID).ToList();
            //Εύρεση περιφέριας της αίτησης
            var periferiaAitisis = (from a in db.AITISIS where a.AITISI_ID == aitisiID select a.PERIFERIA_ID).FirstOrDefault();
            //Εύρεση σχολείων στην περιφέρεια τα οποία έχει προκυρηχθεί η ειδικότητα της αίτησης
            var filteredSchools = (from s in db.SYS_SCHOOLS
                                   where s.SCHOOL_PERIFERIA_ID == periferiaAitisis && prokirixiSchools.Contains(s.SCHOOL_ID)
                                   select new SYS_SCHOOLSViewModel
                                   {
                                       SCHOOL_ID = s.SCHOOL_ID,
                                       SCHOOL_NAME = s.SCHOOL_NAME,
                                       SCHOOL_PERIFERIA_ID = s.SCHOOL_PERIFERIA_ID
                                   });
            ViewData["schools"] = filteredSchools;
            ViewData["defaultSchool"] = filteredSchools.First().SCHOOL_ID;
        }

        public void PopulateIncomeYears()
        {
            var incomeYears = (from ys in db.SYS_TAXFREE
                               orderby ys.YEAR_TEXT
                               select ys).ToList();

            ViewData["income_years"] = incomeYears;
        }

        public void PopulateTeachTypes()
        {
            var teachTypes = (from tt in db.SYS_TEACH1_TYPES
                              select tt).ToList();

            ViewData["teach_types"] = teachTypes;
        }

        public void PopulateSchoolYears()
        {
            var syears = (from d in db.SYS_SCHOOLYEARS
                          orderby d.SY_TEXT descending
                          select d).ToList();

            ViewData["school_years"] = syears;
            ViewData["defaultSchoolYear"] = syears.First().SY_ID;
        }

        public void PopulateViewBagWithAitisi(AitisisViewModel selectedAitisi)
        {
            ViewBag.SelectedAitisiData = selectedAitisi;
            ViewBag.SelectedAitisiPeriferia = (from p in db.SYS_PERIFERIES
                                               where p.PERIFERIA_ID == selectedAitisi.PERIFERIA_ID
                                               select new SYS_PERIFERIESViewModel
                                               {
                                                   PERIFERIA_ID = p.PERIFERIA_ID,
                                                   PERIFERIA_NAME = p.PERIFERIA_NAME,
                                                   SYS_DIMOS = p.SYS_DIMOS
                                               }).FirstOrDefault();
            ViewBag.SelectedAitisiEidikotita = (from e in db.SYS_EIDIKOTITES
                                                where e.EIDIKOTITA_ID == (selectedAitisi.EIDIKOTITA ?? 0)
                                                select new SYS_EIDIKOTITESViewModel
                                                {
                                                    EIDIKOTITA_ID = e.EIDIKOTITA_ID,
                                                    EIDIKOTITA_CODE = e.EIDIKOTITA_CODE,
                                                    EIDIKOTITA_NAME = e.EIDIKOTITA_NAME
                                                }).FirstOrDefault();
        }

        public void PopulateStatus()
        {
            var status = (from s in db.SYS_PROKIRIXI_STATUS
                          select s).ToList();

            ViewData["Status"] = status;
        }

        public void PopulateKladoiUnified()
        {
            var kladosUnified = (from k in db.SYS_KLADOS_ENIAIOS
                                 select k).ToList();

            ViewData["kladoiUnified"] = kladosUnified;
        }

        public void PopulateKladoi()
        {
            var kladosTypes = (from k in db.SYS_KLADOS
                               select k).ToList();

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

        public void PopulateProkirixis()
        {
            var pdata = (from d in db.PROKIRIXIS
                         orderby d.SCHOOL_YEAR descending
                         select d).ToList();

            ViewData["Prokirixis"] = pdata;
        }

        #endregion


        #region COMBOS DATA SOURCES

        public JsonResult GetAnergies(string text)
        {
            var anergies = db.SYS_ANERGIA.AsQueryable();

            if (!string.IsNullOrEmpty(text))
            {
                anergies = anergies.Where(p => p.ANERGIA_TAG.Contains(text));
            }

            return Json(anergies, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPtyxiaTypes()
        {
            var data = db.SYS_PTYXIA_TYPES.ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSocialGroups(string text)
        {
            var anergies = db.SYS_SOCIALGROUPS.AsQueryable();

            if (!string.IsNullOrEmpty(text))
            {
                anergies = anergies.Where(p => p.CATEGORY.Contains(text));
            }

            return Json(anergies, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetKlados(string text)
        {
            var kladoi = db.SYS_KLADOS.Select(p => new SYS_KLADOSViewModel
            {
                KLADOS_NAME = p.KLADOS_NAME,
                KLADOS_ID = p.KLADOS_ID
            });

            if (!string.IsNullOrEmpty(text))
            {
                kladoi = kladoi.Where(p => p.KLADOS_NAME.Contains(text));
            }
            return Json(kladoi, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCascadeEidikotites(int? klados, string eidikotitaFilter)
        {
            var eidikotites = db.VD_EIDIKOTITES.AsQueryable();

            var prokirixiEidikotites = (from pe in db.PROKIRIXIS_EIDIKOTITES select pe.EIDIKOTITA_ID).ToList();
            eidikotites = eidikotites.Where(e => prokirixiEidikotites.Contains(e.EIDIKOTITA_ID));
            if (klados != null)
            {
                eidikotites = eidikotites.Where(m => m.EIDIKOTITA_KLADOS_ID == klados);
            }

            if (!string.IsNullOrEmpty(eidikotitaFilter))
            {
                int possibleInt;
                if (int.TryParse(eidikotitaFilter, out possibleInt))
                {
                    eidikotites = eidikotites.Where(m => m.EIDIKOTITA_ID.Equals(possibleInt));
                }
                else
                {
                    eidikotites = eidikotites.Where(m => m.EIDIKOTITA_DESC.Contains(eidikotitaFilter));
                }
            }
            var result = eidikotites.Select(m => new VD_EIDIKOTITESViewModel
            {
                EIDIKOTITA_ID = m.EIDIKOTITA_ID,
                EIDIKOTITA_CODE = m.EIDIKOTITA_CODE,
                EIDIKOTITA_NAME = m.EIDIKOTITA_NAME,
                EIDIKOTITA_DESC = m.EIDIKOTITA_DESC
            }).OrderBy(m => m.EIDIKOTITA_DESC);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetBasicEducation(string text)
        {
            var basic = db.SYS_BASICEDUCATION.Select(p => new SYS_BASICEDUCATIONViewModel
            {
                BASIC_TEXT = p.BASIC_TEXT,
                BASIC_ID = p.BASIC_ID
            });

            if (!string.IsNullOrEmpty(text))
            {
                basic = basic.Where(p => p.BASIC_TEXT.Contains(text));
            }
            return Json(basic, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetMscPeriods(string text)
        {
            var periods = db.SYS_MSCPERIODS.Select(p => new SYS_MSCPERIODSViewModel
            {
                MSCPERIOD_ID = p.MSCPERIOD_ID,
                MSCPERIOD_TEXT = p.MSCPERIOD_TEXT
            });

            if (!string.IsNullOrEmpty(text))
            {
                periods = periods.Where(p => p.MSCPERIOD_TEXT.Contains(text));
            }
            return Json(periods, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPedPeriods(string text)
        {
            var periods = db.SYS_PEDAGOGICPERIOD.Select(p => new SYS_PEDAGOGICPERIODViewModel
            {
                PERIOD_ID = p.PERIOD_ID,
                PERIOD_TEXT = p.PERIOD_TEXT
            });

            if (!string.IsNullOrEmpty(text))
            {
                periods = periods.Where(p => p.PERIOD_TEXT.Contains(text));
            }
            return Json(periods, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetLanguages(string text)
        {
            var lang = (from l in db.SYS_LANGUAGE
                        orderby l.LANGUAGE_TEXT
                        select new { l.LANGUAGE_TEXT }).ToList();

            if (!string.IsNullOrEmpty(text))
            {
                lang = (from a in db.SYS_LANGUAGE where a.LANGUAGE_TEXT.Contains(text) orderby a.LANGUAGE_TEXT select new { a.LANGUAGE_TEXT }).ToList();
            }
            return Json(lang, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetLangLevels(string text)
        {
            var periods = db.SYS_LANGUAGELEVEL.Select(p => new SYS_LANGUAGELEVELViewModel
            {
                LEVEL = p.LEVEL
            });

            if (!string.IsNullOrEmpty(text))
            {
                periods = periods.Where(p => p.LEVEL.Contains(text));
            }
            return Json(periods, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetComputerCertificates(string text)
        {
            var periods = db.SYS_COMPUTERASEP.Select(p => new SYS_COMPUTERASEPViewModel
            {
                CERTIFICATE_ID = p.CERTIFICATE_ID,
                CERTIFICATE = p.CERTIFICATE
            });

            if (!string.IsNullOrEmpty(text))
            {
                periods = periods.Where(p => p.CERTIFICATE.Contains(text));
            }
            return Json(periods, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPeriferies(string text, int? filterEidikotita)
        {
            var prokirixiSchools = (from prox in db.PROKIRIXIS_EIDIKOTITES select prox.SCHOOL_ID).ToList();
            if (filterEidikotita.HasValue)
            {
                prokirixiSchools = (from prox in db.PROKIRIXIS_EIDIKOTITES where (prox.EIDIKOTITA_ID == filterEidikotita) select prox.SCHOOL_ID).ToList();
            }
            var prokirixiSchoolsPeriferies = (from d in db.SYS_SCHOOLS where prokirixiSchools.Contains(d.SCHOOL_ID) select d.SCHOOL_PERIFERIA_ID).ToList();
            var periferiesWithSchools = (from p in db.SYS_PERIFERIES
                                         where prokirixiSchoolsPeriferies.Contains(p.PERIFERIA_ID)
                                         select new SYS_PERIFERIESViewModel
                                         {
                                             PERIFERIA_ID = p.PERIFERIA_ID,
                                             PERIFERIA_NAME = p.PERIFERIA_NAME
                                         });

            if (!string.IsNullOrEmpty(text))
            {
                int possibleInt;
                if (int.TryParse(text, out possibleInt))
                {
                    periferiesWithSchools = periferiesWithSchools.Where(p => p.PERIFERIA_ID == possibleInt);
                }
                else
                {
                    periferiesWithSchools = periferiesWithSchools.Where(p => p.PERIFERIA_NAME.Contains(text));
                }
            }

            return Json(periferiesWithSchools, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetApokleismoi(string text)
        {
            var ap = db.SYS_APOKLEISMOI.AsQueryable();

            if (!string.IsNullOrEmpty(text))
            {
                ap = ap.Where(p => p.APOKLEISMOS_TEXT.Contains(text));
            }

            return Json(ap, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetEpagelma(string text)
        {
            var epagelma = db.SYS_EPAGELMA.AsQueryable();

            if (!string.IsNullOrEmpty(text))
            {
                epagelma = epagelma.Where(p => p.EPAGELMA_TEXT.Contains(text));
            }

            return Json(epagelma, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetIncomeYears(string text)
        {
            var iyears = db.SYS_TAXFREE.AsQueryable();

            if (!string.IsNullOrEmpty(text))
            {
                int possibleInt;
                if (int.TryParse(text, out possibleInt))
                {
                    iyears = iyears.Where(p => p.YEAR_ID == possibleInt);
                }
                else
                {
                    iyears = iyears.Where(p => p.YEAR_TEXT.Contains(text));
                }
            }
            return Json(iyears, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetTeachTypes(string text)
        {
            var teach_types = db.SYS_TEACH1_TYPES.AsQueryable();

            if (!string.IsNullOrEmpty(text))
            {
                teach_types = teach_types.Where(p => p.TYPE_TEXT.Contains(text));
            }
            return Json(teach_types, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSchoolYears(string text)
        {
            var iyears = db.SYS_SCHOOLYEARS.AsQueryable();

            if (!string.IsNullOrEmpty(text))
            {
                int possibleInt;
                if (int.TryParse(text, out possibleInt))
                {
                    iyears = iyears.Where(p => p.SY_ID == possibleInt).OrderBy(p => p.SY_TEXT);
                }
                else
                {
                    iyears = iyears.Where(p => p.SY_TEXT.Contains(text)).OrderBy(p => p.SY_TEXT);
                }
            }
            return Json(iyears, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCascadeSchools(int? periferia, int? eidikotita, string schoolFilter)
        {
            //Εύρεση ενεργής προκύρηξης
            var activeProkirixi = (from prox in db.PROKIRIXIS where prox.ADMIN == true select prox.ID).FirstOrDefault();
            //Εύρεση προκυρησόμενων σχολείων με την ειδικότητα της αίτησης
            var prokirixiSchools = (from prox in db.PROKIRIXIS_EIDIKOTITES where prox.PROKIRIXI_ID == activeProkirixi && prox.EIDIKOTITA_ID == eidikotita select prox.SCHOOL_ID).ToList();

            var schools = db.sqlPERIFERIES_SCHOOLS.AsQueryable().Where(d => d.SCHOOL_PERIFERIA_ID == periferia && prokirixiSchools.Contains(d.SCHOOL_ID)).GroupBy(p => p.SCHOOL_ID).Select(grp => grp.FirstOrDefault());

            if (periferia != null)
            {
                schools = schools.Where(p => p.SCHOOL_PERIFERIA_ID == periferia).GroupBy(p => p.SCHOOL_ID).Select(grp => grp.FirstOrDefault());
            }
            else
                if (!string.IsNullOrEmpty(schoolFilter))
            {
                int possibleInt;
                if (int.TryParse(schoolFilter, out possibleInt))
                {
                    schools = schools.Where(p => p.SCHOOL_ID.Equals(possibleInt)).GroupBy(p => p.SCHOOL_ID).Select(grp => grp.FirstOrDefault());
                }
                else
                {
                    schools = schools.Where(p => p.SCHOOL_NAME.Contains(schoolFilter)).GroupBy(p => p.SCHOOL_ID).Select(grp => grp.FirstOrDefault());
                }
                schools = schools.Where(p => p.SCHOOL_NAME.Contains(schoolFilter)).GroupBy(p => p.SCHOOL_ID).Select(grp => grp.FirstOrDefault());
            }

            return Json(schools.Select(p => new { SCHOOL_ID = p.SCHOOL_ID, SCHOOL_NAME = p.SCHOOL_NAME }), JsonRequestBehavior.AllowGet);
        }

        public ActionResult FilteredSchoolsRead(string text, int? SCHOOL_TYPE, int? aitisiID)
        {
            //Εύρεση ειδικότητας της αίτησης
            var eidikotitaAitisis = (from aitisi in db.AITISIS where aitisi.AITISI_ID == aitisiID select aitisi.EIDIKOTITA).FirstOrDefault();
            //Εύρεση ενεργής προκύρηξης
            var activeProkirixi = (from prox in db.PROKIRIXIS where prox.ADMIN == true select prox.ID).FirstOrDefault();
            //Εύρεση προκυρησόμενων σχολείων με την ειδικότητα της αίτησης
            var prokirixiSchools = (from prox in db.PROKIRIXIS_EIDIKOTITES where prox.PROKIRIXI_ID == activeProkirixi && prox.EIDIKOTITA_ID == eidikotitaAitisis select prox.SCHOOL_ID).ToList();
            //Εύρεση περιφέριας της αίτησης
            var periferiaAitisis = (from aitisis in db.AITISIS where aitisis.AITISI_ID == aitisiID select aitisis.PERIFERIA_ID);
            //Εύρεση σχολείων στην περιφέρεια τα οποία έχει προκυρηχθεί η ειδικότητα της αίτησης
            var filteredSchools = (from school in db.SYS_SCHOOLS
                                   where periferiaAitisis.Contains(school.SCHOOL_PERIFERIA_ID) && prokirixiSchools.Contains(school.SCHOOL_ID)
                                   select new SYS_SCHOOLSViewModel
                                   {
                                       SCHOOL_ID = school.SCHOOL_ID,
                                       SCHOOL_NAME = school.SCHOOL_NAME,
                                       SCHOOL_PERIFERIA_ID = school.SCHOOL_PERIFERIA_ID
                                   }).GroupBy(p => p.SCHOOL_ID).Select(grp => grp.FirstOrDefault());     // return distinct values
            if (!string.IsNullOrEmpty(text))
            {
                int possibleInt;
                if (int.TryParse(text, out possibleInt))
                {
                    filteredSchools = filteredSchools.Where(p => p.SCHOOL_ID == possibleInt).GroupBy(p => p.SCHOOL_ID).Select(grp => grp.FirstOrDefault()); // return distinct values
                }
                else
                {
                    filteredSchools = filteredSchools.Where(p => p.SCHOOL_NAME.Contains(text)).GroupBy(p => p.SCHOOL_ID).Select(grp => grp.FirstOrDefault()); // return distinct values
                }

            }
            return Json(filteredSchools, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetProkirixeis()
        {
            var data = (from d in db.PROKIRIXIS
                        orderby d.DATE_START descending
                        select new ProkirixisViewModel
                        {
                            ID = d.ID,
                            PROTOCOL = d.PROTOCOL
                        }).ToList();

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSchools(string schoolFilter)
        {
            var schools = db.SYS_SCHOOLS.AsQueryable();

            if (!string.IsNullOrEmpty(schoolFilter))
            {
                int possibleInt;
                if (int.TryParse(schoolFilter, out possibleInt))
                {
                    schools = schools.Where(p => p.SCHOOL_ID.Equals(possibleInt));
                }
                else
                {
                    schools = schools.Where(p => p.SCHOOL_NAME.Contains(schoolFilter));
                }
                schools = schools.Where(p => p.SCHOOL_NAME.Contains(schoolFilter));
            }
            return Json(schools.Select(p => new { SCHOOL_ID = p.SCHOOL_ID, SCHOOL_NAME = p.SCHOOL_NAME }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAnergiesOld(string text)
        {
            var anergies = db.SYS_ANERGIA_OLD.AsQueryable();

            if (!string.IsNullOrEmpty(text))
            {
                anergies = anergies.Where(p => p.ANERGIA_TAG.Contains(text));
            }
            return Json(anergies, JsonRequestBehavior.AllowGet);
        }


        #endregion


        #region LOCAL FUNCTIONS

        public USER_ADMINS GetLoginAdmin()
        {
            loggedAdmin = db.USER_ADMINS.Where(u => u.USERNAME == System.Web.HttpContext.Current.User.Identity.Name).FirstOrDefault();

            ViewBag.loggedAdmin = loggedAdmin;
            ViewBag.loggedUser = loggedAdmin.FULLNAME;

            return loggedAdmin;
        }

        public USER_SCHOOLS GetLoginSchool()
        {
            loggedSchool = db.USER_SCHOOLS.Where(u => u.USERNAME == System.Web.HttpContext.Current.User.Identity.Name).FirstOrDefault();

            int SchoolID = loggedSchool.USER_SCHOOLID ?? 0;
            var _school = (from d in db.sqlUSER_SCHOOL
                           where d.USER_SCHOOLID == SchoolID
                           select new { d.SCHOOL_NAME }).FirstOrDefault();

            ViewBag.loggedUser = _school.SCHOOL_NAME;
            return loggedSchool;
        }

        public bool VerifyAitisis(AitisisViewModel aitisi)
        {
            if (!(aitisi.KLADOS > 0) && !(aitisi.EIDIKOTITA > 0) && !(aitisi.PERIFERIA_ID > 0) && !((aitisi.SCHOOL_ID) > 0)) return false;
            else return true;

        }

        public sqlTEACHER_AITISEIS GetSelectedAitisi(int aitisiID)
        {
            var aitisi = (from ta in db.sqlTEACHER_AITISEIS
                          where ta.AITISI_ID == aitisiID
                          orderby ta.FULLNAME
                          select ta).FirstOrDefault();
            return (aitisi);
        }

        public sqlTEACHER_AITISEIS GetSelectedAitisi(int? schoolId)
        {
            int prokirixiId = Common.GetOpenProkirixiID(true);

            if (AitiseisExist(prokirixiId, schoolId))
            {
                var aitisi = (from ta in db.sqlTEACHER_AITISEIS
                              where ta.PROKIRIXI_ID == prokirixiId && ta.SCHOOL_ID == schoolId
                              orderby ta.FULLNAME
                              select ta).First();
                return (aitisi);
            }
            else return null;
        }

        public sqlTEACHER_AITISEIS GetSelectedAitisi()
        {
            int prokirixiId = Common.GetAdminProkirixiID();

            if (AitiseisExist(prokirixiId))
            {

                var aitisi = (from ta in db.sqlTEACHER_AITISEIS
                              where ta.PROKIRIXI_ID == prokirixiId
                              orderby ta.FULLNAME
                              select ta).First();
                return aitisi;
            }
            else return null;
        }

        public bool AitiseisExist(int prokirixiId)
        {
            var aitisi = (from ta in db.sqlTEACHER_AITISEIS
                          where ta.PROKIRIXI_ID == prokirixiId
                          orderby ta.FULLNAME
                          select ta).ToList();

            if (aitisi.Count() == 0) return false;
            else return true;
        }

        public bool AitiseisExist(int prokirixiId, int? schoolID)
        {
            var aitisi = (from ta in db.sqlTEACHER_AITISEIS
                          where ta.PROKIRIXI_ID == prokirixiId && ta.SCHOOL_ID == schoolID
                          orderby ta.FULLNAME
                          select ta).ToList();

            if (aitisi.Count() == 0) return false;
            else return true;
        }

        #endregion


        #region STATES OF GRIDS - NOT USED

        [ValidateInput(false)]
        public ActionResult Save(string data)
        {
            Session["data"] = data;

            //int temp = 1;

            return new EmptyResult();
        }

        public ActionResult Load()
        {
            //int temp = 1;

            return Json(Session["data"], JsonRequestBehavior.AllowGet);
        }

        #endregion

    }
}