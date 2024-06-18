using Pegasus.BPM;
using Pegasus.DAL;
using Pegasus.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Pegasus.Services
{
    public class AitisiRegistryService : IAitisiRegistryService, IDisposable
    {
        private readonly PegasusDBEntities entities;

        public AitisiRegistryService(PegasusDBEntities entities)
        {
            this.entities = entities;
        }

        public IEnumerable<sqlTEACHER_AITISEIS> Read(int prokirixiId)
        {
            if (prokirixiId > 0)
            {
                var data = (from d in entities.sqlTEACHER_AITISEIS
                            where d.PROKIRIXI_ID == prokirixiId
                            orderby d.FULLNAME
                            select d).ToList();
                return data;
            }
            else
            {
                var data = (from d in entities.sqlTEACHER_AITISEIS
                            orderby d.PROKIRIXI_ID descending, d.FULLNAME
                            select d).ToList();
                return data;
            }
        }

        public AitisisViewModel GetModel(int aitisiId)
        {
            var data = (from d in entities.AITISIS
                        where d.AITISI_ID == aitisiId
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
                            AITISIS_SCHOOLS = (from s in entities.AITISIS_SCHOOLS where s.AITISI_ID == aitisiId select s).ToList(),
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

        public ExperienceResultsViewModel GetResults(int aitisiId)
        {
            AITISIS aitisi = (from d in entities.AITISIS
                              where d.AITISI_ID == aitisiId
                              select d).FirstOrDefault();

            TEACHERS teacher = (from d in entities.TEACHERS where d.AFM == aitisi.AFM select d).FirstOrDefault();

            ExperienceResultsViewModel moriaResults = new ExperienceResultsViewModel()
            {
                AITISI_ID = aitisiId,
                LASTNAME = teacher.LASTNAME,
                FIRSTNAME = teacher.FIRSTNAME,

                EIDIKOTITA = (from e in entities.SYS_EIDIKOTITES
                              where e.EIDIKOTITA_ID == aitisi.EIDIKOTITA
                              select new SYS_EIDIKOTITESViewModel
                              {
                                  EIDIKOTITA_CODE = e.EIDIKOTITA_CODE,
                                  EIDIKOTITA_ID = e.EIDIKOTITA_ID,
                                  EIDIKOTITA_NAME = e.EIDIKOTITA_NAME,
                                  EIDIKOTITA_KLADOS_ID = e.EIDIKOTITA_KLADOS_ID
                              }).FirstOrDefault(),

                TEACHING_MORIA = (from d in entities.sqlEXP_TEACHING_2
                                  where d.AITISI_ID == aitisiId
                                  select new ExperienceTeachingViewModel
                                  {
                                      KLADOS = d.KLADOS,
                                      TYPE_TEXT = d.TYPE_TEXT,
                                      TEACH_TYPE = d.TEACH_TYPE,
                                      MORIA_TOTAL = d.MORIA_TOTAL ?? 0
                                  }).ToList(),
                TEACHING_MORIA_FINAL = (from d in entities.sqlEXP_TEACHING_FINAL where d.AITISI_ID == aitisiId select d.MORIA_FINAL).FirstOrDefault() ?? 0d,
                VOCATION_MORIA = (from d in entities.sqlEXP_VOCATION_1 where d.AITISI_ID == aitisiId select d.MSUM).FirstOrDefault() ?? 0d,
                VOCATION_MORIA_FINAL = (from d in entities.sqlEXP_VOCATION_FINAL where d.AITISI_ID == aitisiId select d.MORIA_TOTAL).FirstOrDefault() ?? 0d,
                FREELANCE_MORIA = (from d in entities.sqlEXP_FREELANCE_1 where d.AITISI_ID == aitisiId select d.MSUM).FirstOrDefault() ?? 0d,
                FREELANCE_MORIA_FINAL = (from d in entities.sqlEXP_FREELANCE_FINAL where d.AITISI_ID == aitisiId select d.MORIA_TOTAL).FirstOrDefault() ?? 0d,
                WORK_MORIA_FINAL = (from d in entities.sqlEXP_WORK_FINAL where d.AITISI_ID == aitisiId select d.WORK_MORIA_FINAL).FirstOrDefault() ?? 0d
            };

            aitisi.MORIA_TEACH = (float?)moriaResults.TEACHING_MORIA_FINAL;
            aitisi.MORIA_WORK1 = (float?)moriaResults.VOCATION_MORIA_FINAL;
            aitisi.MORIA_WORK2 = (float?)moriaResults.FREELANCE_MORIA_FINAL;
            aitisi.MORIA_WORK = (float)moriaResults.WORK_MORIA_FINAL;
            aitisi.MORIA_SOCIAL = AitisiMoria.MoriaSocial(aitisi);
            aitisi.MORIA_TOTAL = AitisiMoria.MoriaTotal(aitisi);

            moriaResults.AITISI = GetModel(aitisi.AITISI_ID);

            return moriaResults;
        }

        public IEnumerable<ViewModelTeaching> ReadTeaching(int aitisiId)
        {
            var data = (from d in entities.EXP_TEACHING
                        where d.AITISI_ID == aitisiId
                        orderby d.TEACH_TYPE, d.SCHOOL_YEAR descending
                        select new ViewModelTeaching
                        {
                            AITISI_ID = d.AITISI_ID,
                            EXP_ID = d.EXP_ID,
                            TEACH_TYPE = d.TEACH_TYPE,
                            SCHOOL_YEAR = d.SCHOOL_YEAR,
                            DATE_FROM = d.DATE_FROM,
                            DATE_TO = d.DATE_TO,
                            HOURS_WEEK = d.HOURS_WEEK,
                            HOURS = d.HOURS,
                            MORIA = d.MORIA,
                            DOC_PROTOCOL = d.DOC_PROTOCOL,
                            DOC_ORIGIN = d.DOC_ORIGIN,
                            DOC_COMMENT = d.DOC_COMMENT,
                            DOC_VALID = d.DOC_VALID ?? false,
                            ERROR_TEXT = d.ERROR_TEXT,
                            DUPLICATE = d.DUPLICATE ?? false
                        }).ToList();
            return data;
        }

        public IEnumerable<ViewModelVocational> ReadVocation(int aitisiId)
        {
            var data = (from d in entities.EXP_VOCATIONAL
                        where d.AITISI_ID == aitisiId
                        orderby d.DATE_FROM descending
                        select new ViewModelVocational
                        {
                            AITISI_ID = d.AITISI_ID,
                            AFM = d.AITISIS.TEACHERS.AFM,
                            EXP_ID = d.EXP_ID,
                            DATE_FROM = d.DATE_FROM,
                            DATE_TO = d.DATE_TO,
                            DAYS_AUTO = d.DAYS_AUTO,
                            DAYS_MANUAL = d.DAYS_MANUAL,
                            MORIA = d.MORIA,
                            DOC_PROTOCOL = d.DOC_PROTOCOL,
                            DOC_ORIGIN = d.DOC_ORIGIN,
                            DOC_COMMENT = d.DOC_COMMENT,
                            DOC_VALID = d.DOC_VALID ?? false,
                            ERROR_TEXT = d.ERROR_TEXT,
                            DUPLICATE = d.DUPLICATE ?? false
                        }).ToList();
            return data;
        }

        public IEnumerable<ViewModelFreelance> ReadFreelance(int aitisiID)
        {
            var data = (from d in entities.EXP_FREELANCE
                        where d.AITISI_ID == aitisiID
                        orderby d.INCOME_YEAR descending, d.DATE_FROM descending
                        select new ViewModelFreelance
                        {
                            AITISI_ID = d.AITISI_ID,
                            EXP_ID = d.EXP_ID,
                            INCOME_YEAR = d.INCOME_YEAR,
                            INCOME = d.INCOME,
                            INCOME_TAXFREE = d.INCOME_TAXFREE,
                            INCOME_NOMISMA = d.INCOME_NOMISMA,
                            DATE_FROM = d.DATE_FROM,
                            DATE_TO = d.DATE_TO,
                            DAYS_AUTO = d.DAYS_AUTO,
                            DAYS_MANUAL = d.DAYS_MANUAL,
                            MORIA = d.MORIA,
                            DOC_PROTOCOL = d.DOC_PROTOCOL,
                            DOC_ORIGIN = d.DOC_ORIGIN,
                            DOC_COMMENT = d.DOC_COMMENT,
                            DOC_VALID = d.DOC_VALID ?? false,
                            ERROR_TEXT = d.ERROR_TEXT,
                            DUPLICATE = d.DUPLICATE ?? false
                        }).ToList();
            return data;
        }

        public void Dispose()
        {
            entities.Dispose();
        }
    }
}