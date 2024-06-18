using Pegasus.BPM;
using Pegasus.DAL;
using Pegasus.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Pegasus.Services
{
    public class TeacherService : ITeacherService, IDisposable
    {
        private readonly PegasusDBEntities entities;

        public TeacherService(PegasusDBEntities entities)
        {
            this.entities = entities;
        }

        public void Create(string AFM, TeacherViewModel model)
        {
            TEACHERS entity = new TEACHERS()
            {
                LASTNAME = model.LASTNAME.Trim(),
                FIRSTNAME = model.FIRSTNAME.Trim(),
                FATHERNAME = model.FATHERNAME.Trim(),
                MOTHERNAME = model.MOTHERNAME.Trim(),
                AFM = AFM,
                DOY = model.DOY,
                ADT = model.ADT,
                AMKA = model.AMKA,
                GENDER = model.GENDER,
                BIRTHDATE = model.BIRTHDATE,
                ARMY_STATUS = model.ARMY_STATUS,
                MARITAL_STATUS = model.MARITAL_STATUS,
                ADDRESS = model.ADDRESS,
                CITY = model.CITY,
                TK = model.TK,
                PERIFERIA = model.PERIFERIA,
                DIMOS = model.DIMOS,
                TELEPHONE = model.TELEPHONE,
                MOBILE = model.MOBILE,
                EMAIL = model.EMAIL,
                COMMENT = model.COMMENT
            };
            entities.TEACHERS.Add(entity);
            entities.SaveChanges();
        }

        public void Update(string AFM, TeacherViewModel model)
        {
            TEACHERS entity = entities.TEACHERS.Find(AFM);

            entity.LASTNAME = model.LASTNAME.Trim();
            entity.FIRSTNAME = model.FIRSTNAME.Trim();
            entity.FATHERNAME = model.FATHERNAME.Trim();
            entity.MOTHERNAME = model.MOTHERNAME.Trim();
            entity.DOY = model.DOY;
            entity.ADT = model.ADT;
            entity.AMKA = model.AMKA;
            entity.GENDER = model.GENDER;
            entity.BIRTHDATE = model.BIRTHDATE;
            entity.ARMY_STATUS = model.ARMY_STATUS;
            entity.MARITAL_STATUS = model.MARITAL_STATUS;
            entity.ADDRESS = model.ADDRESS;
            entity.CITY = model.CITY;
            entity.TK = model.TK;
            entity.PERIFERIA = model.PERIFERIA;
            entity.DIMOS = model.DIMOS;
            entity.TELEPHONE = model.TELEPHONE;
            entity.MOBILE = model.MOBILE;
            entity.EMAIL = model.EMAIL;
            entity.COMMENT = model.COMMENT;

            entities.Entry(entity).State = EntityState.Modified;
            entities.SaveChanges();
        }

        public TeacherViewModel GetModel(string AFM)
        {
            var data = (from d in entities.TEACHERS
                        where d.AFM == AFM
                        select new TeacherViewModel
                        {
                            LASTNAME = d.LASTNAME,
                            FIRSTNAME = d.FIRSTNAME,
                            FATHERNAME = d.FATHERNAME,
                            MOTHERNAME = d.MOTHERNAME,
                            AFM = d.AFM,
                            DOY = d.DOY,
                            ADT = d.ADT,
                            AMKA = d.AMKA,
                            GENDER = d.GENDER,
                            BIRTHDATE = d.BIRTHDATE,
                            ARMY_STATUS = d.ARMY_STATUS,
                            MARITAL_STATUS = d.MARITAL_STATUS,
                            ADDRESS = d.ADDRESS,
                            CITY = d.CITY,
                            TK = d.TK,
                            PERIFERIA = d.PERIFERIA,
                            DIMOS = d.DIMOS,
                            TELEPHONE = d.TELEPHONE,
                            MOBILE = d.MOBILE,
                            EMAIL = d.EMAIL,
                            COMMENT = d.COMMENT
                        }).FirstOrDefault();
            return data;
        }

        public List<sqlTEACHER_AITISEIS> ReadAitiseis(string AFM)
        {
            List<sqlTEACHER_AITISEIS> results = new List<sqlTEACHER_AITISEIS>();

            bool ViewAllowed = Common.GetOpenProkirixiUserView();

            if (ViewAllowed == true)
            {
                try
                {
                    var data = (from a in entities.sqlTEACHER_AITISEIS
                                where a.AFM == AFM
                                orderby a.DATE_START descending
                                select a).ToList();
                    results = data;
                }
                catch (Exception e)
                {
                    string errmsg = e.Message;
                    return results;
                }
            }
            return results;
        }

        public AitisisViewModel GetAitisiModel(int aitisiID)
        {
            var data = (from d in entities.AITISIS
                        where d.AITISI_ID == aitisiID
                        orderby d.TEACHERS.LASTNAME, d.TEACHERS.FIRSTNAME
                        select new AitisisViewModel
                        {
                            DOY = d.TEACHERS.DOY,
                            AMKA = d.TEACHERS.AMKA,
                            LASTNAME = d.TEACHERS.LASTNAME,
                            FIRSTNAME = d.TEACHERS.FIRSTNAME,
                            FATHERNAME = d.TEACHERS.FATHERNAME,
                            AFM = d.AFM,
                            AITISI_ID = d.AITISI_ID,
                            AITISI_DATE = d.AITISI_DATE,
                            AITISI_PROTOCOL = d.AITISI_PROTOCOL,
                            ANERGIA = d.ANERGIA,
                            SOCIALGROUP = d.SOCIALGROUP,
                            SOCIALGROUP_PROTOCOL = d.SOCIALGROUP_PROTOCOL,
                            SOCIALGROUP_YPIRESIA = d.SOCIALGROUP_YPIRESIA,
                            N2190 = d.N2190,
                            KLADOS = d.KLADOS,
                            EIDIKOTITA = d.EIDIKOTITA,
                            BASIC_EDUCATION = d.BASIC_EDUCATION,
                            PTYXIO_TYPE = d.PTYXIO_TYPE,
                            PTYXIO_TITLOS = d.PTYXIO_TITLOS,
                            PTYXIO_BATHMOS = d.PTYXIO_BATHMOS,
                            PTYXIO_DATE = d.PTYXIO_DATE,
                            MSC = d.MSC ?? false,
                            MSC_DIARKEIA = d.MSC_DIARKEIA,
                            MSC_TITLOS = d.MSC_TITLOS,
                            PHD = d.PHD ?? false,
                            PHD_TITLOS = d.PHD_TITLOS,
                            PED = d.PED ?? false,
                            PED_TITLOS = d.PED_TITLOS,
                            PED_DIARKEIA = d.PED_DIARKEIA,
                            AED_MSC = d.AED_MSC ?? false,
                            AED_MSC_TITLOS = d.AED_MSC_TITLOS,
                            AED_PHD = d.AED_PHD ?? false,
                            AED_PHD_TITLOS = d.AED_PHD_TITLOS,
                            LANG_TEXT = d.LANG_TEXT,
                            LANG_LEVEL = d.LANG_LEVEL,
                            LANG_TITLOS = d.LANG_TITLOS,
                            COMPUTER_CERT = d.COMPUTER_CERT,
                            COMPUTER_TITLOS = d.COMPUTER_TITLOS,
                            EPIMORFOSI1 = d.EPIMORFOSI1 ?? false,
                            EPIMORFOSI1_HOURS = d.EPIMORFOSI1_HOURS ?? 0,
                            EPIMORFOSI2 = d.EPIMORFOSI2 ?? false,
                            EPIMORFOSI2_HOURS = d.EPIMORFOSI2_HOURS ?? 0,
                            EPIMORFOSI3 = d.EPIMORFOSI3 ?? false,
                            EPIMORFOSI3_HOURS = d.EPIMORFOSI3_HOURS ?? 0,
                            PERIFERIA_ID = d.PERIFERIA_ID,
                            SCHOOL_ID = d.SCHOOL_ID,
                            PROKIRIXI_ID = d.PROKIRIXI_ID,
                            EPAGELMA_STATUS = d.EPAGELMA_STATUS,
                            CHECK_STATUS = d.CHECK_STATUS ?? false,
                            CHILDREN = d.CHILDREN,
                            TRANSFERRED = d.TRANSFERRED ?? false,
                            AITISIS_SCHOOLS = (from s in entities.AITISIS_SCHOOLS where s.AITISI_ID == aitisiID select s).ToList(),
                            // new fields (2016)
                            CERTIFIED = d.CERTIFIED ?? false,
                            SOCIALGROUP1 = d.SOCIALGROUP1 ?? false,
                            SOCIALGROUP2 = d.SOCIALGROUP2 ?? false,
                            SOCIALGROUP3 = d.SOCIALGROUP3 ?? false,
                            SOCIALGROUP4 = d.SOCIALGROUP4 ?? false,
                            SOCIALGROUP1_DOC = d.SOCIALGROUP1_DOC,
                            SOCIALGROUP2_DOC = d.SOCIALGROUP2_DOC,
                            SOCIALGROUP3_DOC = d.SOCIALGROUP3_DOC,
                            SOCIALGROUP4_DOC = d.SOCIALGROUP4_DOC,
                            LANG1_TEXT = d.LANG1_TEXT,
                            LANG1_LEVEL = d.LANG1_LEVEL,
                            LANG1_TITLOS = d.LANG1_TITLOS,
                            LANG2_TEXT = d.LANG2_TEXT,
                            LANG2_LEVEL = d.LANG2_LEVEL,
                            LANG2_TITLOS = d.LANG2_TITLOS,
                            // calculated fields
                            AGE = d.AGE,
                            MORIA_ANERGIA = d.MORIA_ANERGIA ?? 0,
                            MORIA_PTYXIO = d.MORIA_PTYXIO ?? 0,
                            MORIA_MSC = d.MORIA_MSC ?? 0,
                            MORIA_PHD = d.MORIA_PHD ?? 0,
                            MORIA_PED = d.MORIA_PED ?? 0,
                            MORIA_AED_MSC = d.MORIA_AED_MSC ?? 0,
                            MORIA_AED_PHD = d.MORIA_AED_PHD ?? 0,
                            MORIA_LANG = d.MORIA_LANG ?? 0,
                            MORIA_COMPUTER = d.MORIA_COMPUTER ?? 0,
                            MORIA_EPIMORFOSI1 = d.MORIA_EPIMORFOSI1 ?? 0,
                            MORIA_EPIMORFOSI2 = d.MORIA_EPIMORFOSI2 ?? 0,
                            MORIA_EPIMORFOSI3 = d.MORIA_EPIMORFOSI3 ?? 0,
                            MORIA_TEACH = d.MORIA_TEACH ?? 0,
                            MORIA_WORK1 = d.MORIA_WORK1 ?? 0,
                            MORIA_WORK2 = d.MORIA_WORK2 ?? 0,
                            MORIA_WORK = d.MORIA_WORK ?? 0,
                            MORIA_SOCIAL = d.MORIA_SOCIAL ?? 0,
                            MORIA_TOTAL = d.MORIA_TOTAL ?? 0,
                            // epitropes fields
                            APOKLEISMOS = d.APOKLEISMOS ?? false,
                            APOKLEISMOS_AITIA = d.APOKLEISMOS_AITIA,
                            MORIODOTISI_DATE = d.MORIODOTISI_DATE ?? DateTime.Now,
                            MORIODOTISI_PERSON = d.MORIODOTISI_PERSON,
                            EPITROPI1_TEXT = d.EPITROPI1_TEXT,
                            EPITROPI2_TEXT = d.EPITROPI2_TEXT,
                            ENSTASI = d.ENSTASI ?? false,
                            ENSTASI_AITIA = d.ENSTASI_AITIA
                        }).FirstOrDefault();
            return data;
        }

        public ExperienceResultsViewModel GetMoriaNew(int AITISI_ID)
        {
            AITISIS aitisi = (from t in entities.AITISIS where t.AITISI_ID == AITISI_ID select t).FirstOrDefault();

            ExperienceResultsViewModel moriaResults = new ExperienceResultsViewModel()
            {
                AITISI_ID = AITISI_ID,
                EIDIKOTITA = (from e in entities.SYS_EIDIKOTITES
                              where e.EIDIKOTITA_ID == aitisi.EIDIKOTITA
                              select new SYS_EIDIKOTITESViewModel
                              {
                                  EIDIKOTITA_CODE = e.EIDIKOTITA_CODE,
                                  EIDIKOTITA_ID = e.EIDIKOTITA_ID,
                                  EIDIKOTITA_NAME = e.EIDIKOTITA_NAME,
                                  EIDIKOTITA_KLADOS_ID = e.EIDIKOTITA_KLADOS_ID
                              }).FirstOrDefault(),

                TEACHING_MORIA = (from t in entities.sqlEXP_TEACHING_2
                                  where t.AITISI_ID == AITISI_ID
                                  select new ExperienceTeachingViewModel
                                  {
                                      KLADOS = t.KLADOS,
                                      TYPE_TEXT = t.TYPE_TEXT,
                                      TEACH_TYPE = t.TEACH_TYPE,
                                      MORIA_TOTAL = t.MORIA_TOTAL ?? 0
                                  }).ToList(),
                TEACHING_MORIA_FINAL = (from t in entities.sqlEXP_TEACHING_FINAL where t.AITISI_ID == AITISI_ID select t.MORIA_FINAL).FirstOrDefault() ?? 0d,
                VOCATION_MORIA = (from t in entities.sqlEXP_VOCATION_1 where t.AITISI_ID == AITISI_ID select t.MSUM).FirstOrDefault() ?? 0d,
                VOCATION_MORIA_FINAL = (from t in entities.sqlEXP_VOCATION_FINAL where t.AITISI_ID == AITISI_ID select t.MORIA_TOTAL).FirstOrDefault() ?? 0d,
                FREELANCE_MORIA = (from t in entities.sqlEXP_FREELANCE_1 where t.AITISI_ID == AITISI_ID select t.MSUM).FirstOrDefault() ?? 0d,
                FREELANCE_MORIA_FINAL = (from t in entities.sqlEXP_FREELANCE_FINAL where t.AITISI_ID == AITISI_ID select t.MORIA_TOTAL).FirstOrDefault() ?? 0d,
                WORK_MORIA_FINAL = (from t in entities.sqlEXP_WORK_FINAL where t.AITISI_ID == AITISI_ID select t.WORK_MORIA_FINAL).FirstOrDefault() ?? 0d,

                MORIA_MAX_TYPIKH = "5",
                MORIA_MAX_IEKSEKPSEK = "10",
                MORIA_MAX_ATYPH = "5",
                MORIA_MAX_WORK = "10"
            };
            aitisi.MORIA_TEACH = (float?)moriaResults.TEACHING_MORIA_FINAL;
            aitisi.MORIA_WORK1 = (float?)moriaResults.VOCATION_MORIA_FINAL;
            aitisi.MORIA_WORK2 = (float?)moriaResults.FREELANCE_MORIA_FINAL;
            aitisi.MORIA_WORK = (float)moriaResults.WORK_MORIA_FINAL;
            aitisi.MORIA_ANERGIA = AitisiMoria.MoriaAnergia(aitisi);
            aitisi.MORIA_SOCIAL = AitisiMoria.MoriaSocial(aitisi);
            aitisi.MORIA_TOTAL = AitisiMoria.MoriaTotal(aitisi);

            moriaResults.AITISI = GetAitisiModel(AITISI_ID);
            return (moriaResults);
        }

        public ExperienceResultsViewModel GetMoriaOld2(int AITISI_ID)
        {
            AITISIS aitisi = (from t in entities.AITISIS where t.AITISI_ID == AITISI_ID select t).FirstOrDefault();

            ExperienceResultsViewModel moriaResults = new ExperienceResultsViewModel()
            {
                AITISI_ID = AITISI_ID,
                EIDIKOTITA = (from e in entities.SYS_EIDIKOTITES
                              where e.EIDIKOTITA_ID == aitisi.EIDIKOTITA
                              select new SYS_EIDIKOTITESViewModel
                              {
                                  EIDIKOTITA_CODE = e.EIDIKOTITA_CODE,
                                  EIDIKOTITA_ID = e.EIDIKOTITA_ID,
                                  EIDIKOTITA_NAME = e.EIDIKOTITA_NAME,
                                  EIDIKOTITA_KLADOS_ID = e.EIDIKOTITA_KLADOS_ID
                              }).FirstOrDefault(),

                TEACHING_MORIA = (from t in entities.sqlEXP_TEACHING_2
                                  where t.AITISI_ID == AITISI_ID
                                  select new ExperienceTeachingViewModel
                                  {
                                      KLADOS = t.KLADOS,
                                      TYPE_TEXT = t.TYPE_TEXT,
                                      TEACH_TYPE = t.TEACH_TYPE,
                                      MORIA_TOTAL = t.MORIA_TOTAL ?? 0
                                  }).ToList(),
                TEACHING_MORIA_FINAL = (from t in entities.sqlEXP_TEACHING_FINAL where t.AITISI_ID == AITISI_ID select t.MORIA_FINAL).FirstOrDefault() ?? 0d,
                VOCATION_MORIA = (from t in entities.sqlEXP_VOCATION_1 where t.AITISI_ID == AITISI_ID select t.MSUM).FirstOrDefault() ?? 0d,
                VOCATION_MORIA_FINAL = (from t in entities.sqlEXP_VOCATION_FINAL where t.AITISI_ID == AITISI_ID select t.MORIA_TOTAL).FirstOrDefault() ?? 0d,
                FREELANCE_MORIA = (from t in entities.sqlEXP_FREELANCE_1 where t.AITISI_ID == AITISI_ID select t.MSUM).FirstOrDefault() ?? 0d,
                FREELANCE_MORIA_FINAL = (from t in entities.sqlEXP_FREELANCE_FINAL where t.AITISI_ID == AITISI_ID select t.MORIA_TOTAL).FirstOrDefault() ?? 0d,
                MORIA_MAX_TYPIKH = "8",
                MORIA_MAX_IEKSEKPSEK = "6",
                MORIA_MAX_ATYPH = "6",
                MORIA_MAX_WORK = "10"
            };
            aitisi.MORIA_TEACH = (float?)moriaResults.TEACHING_MORIA_FINAL;
            aitisi.MORIA_WORK1 = (float?)moriaResults.VOCATION_MORIA_FINAL;
            aitisi.MORIA_WORK2 = (float?)moriaResults.FREELANCE_MORIA_FINAL;
            aitisi.MORIA_WORK = aitisi.MORIA_WORK1 + aitisi.MORIA_WORK2;
            aitisi.MORIA_SOCIAL = AitisiMoria.MoriaSocialOld2(aitisi);
            aitisi.MORIA_TOTAL = AitisiMoria.MoriaTotalOld2(aitisi);

            moriaResults.AITISI = GetAitisiModel(AITISI_ID);
            return (moriaResults);
        }

        public ExperienceResultsViewModel GetMoriaOld(int AITISI_ID)
        {
            AITISIS aitisi = (from t in entities.AITISIS where t.AITISI_ID == AITISI_ID select t).FirstOrDefault();

            ExperienceResultsViewModel moriaResults = new ExperienceResultsViewModel()
            {
                AITISI_ID = AITISI_ID,
                EIDIKOTITA = (from e in entities.SYS_EIDIKOTITES
                              where e.EIDIKOTITA_ID == aitisi.EIDIKOTITA
                              select new SYS_EIDIKOTITESViewModel
                              {
                                  EIDIKOTITA_CODE = e.EIDIKOTITA_CODE,
                                  EIDIKOTITA_ID = e.EIDIKOTITA_ID,
                                  EIDIKOTITA_NAME = e.EIDIKOTITA_NAME,
                                  EIDIKOTITA_KLADOS_ID = e.EIDIKOTITA_KLADOS_ID
                              }).FirstOrDefault(),

                TEACHING_MORIA = (from t in entities.sqlEXP_TEACHING_2_OLD
                                  where t.AITISI_ID == AITISI_ID
                                  select new ExperienceTeachingViewModel
                                  {
                                      KLADOS = t.KLADOS,
                                      TYPE_TEXT = t.TYPE_TEXT,
                                      TEACH_TYPE = t.TEACH_TYPE,
                                      MORIA_TOTAL = t.MORIA_TOTAL ?? 0
                                  }).ToList(),
                TEACHING_MORIA_FINAL = (from t in entities.sqlEXP_TEACHING_FINAL_OLD where t.AITISI_ID == AITISI_ID select t.MORIA_FINAL).FirstOrDefault() ?? 0d,
                VOCATION_MORIA = (from t in entities.sqlEXP_VOCATION_1_OLD where t.AITISI_ID == AITISI_ID select t.MSUM).FirstOrDefault() ?? 0d,
                VOCATION_MORIA_FINAL = (from t in entities.sqlEXP_VOCATION_FINAL_OLD where t.AITISI_ID == AITISI_ID select t.MORIA_TOTAL).FirstOrDefault() ?? 0d,
                FREELANCE_MORIA = (from t in entities.sqlEXP_FREELANCE_1_OLD where t.AITISI_ID == AITISI_ID select t.MSUM).FirstOrDefault() ?? 0d,
                FREELANCE_MORIA_FINAL = (from t in entities.sqlEXP_FREELANCE_FINAL_OLD where t.AITISI_ID == AITISI_ID select t.MORIA_TOTAL).FirstOrDefault() ?? 0d,
                MORIA_MAX_TYPIKH = "10",
                MORIA_MAX_IEKSEKPSEK = "12",
                MORIA_MAX_ATYPH = "-",
                MORIA_MAX_WORK = "10"
            };
            aitisi.MORIA_TEACH = (float?)moriaResults.TEACHING_MORIA_FINAL;
            aitisi.MORIA_WORK1 = (float?)moriaResults.VOCATION_MORIA_FINAL;
            aitisi.MORIA_WORK2 = (float?)moriaResults.FREELANCE_MORIA_FINAL;
            aitisi.MORIA_WORK = aitisi.MORIA_WORK1 + aitisi.MORIA_WORK2;
            aitisi.MORIA_SOCIAL = AitisiMoria.MoriaSocialOld(aitisi);
            aitisi.MORIA_TOTAL = AitisiMoria.MoriaTotalOld(aitisi);

            moriaResults.AITISI = GetAitisiModel(AITISI_ID);
            return (moriaResults);
        }

        public void Dispose()
        {
            entities.Dispose();
        }
    }
}