using Pegasus.BPM;
using Pegasus.DAL;
using Pegasus.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Pegasus.Services
{
    public class AitisiService : IAitisiService, IDisposable
    {
        private readonly PegasusDBEntities entities;

        public AitisiService(PegasusDBEntities entities)
        {
            this.entities = entities;
        }

        public IEnumerable<AitisisGridViewModel> Read(int prokirixiId)
        {
            var data = (from d in entities.AITISIS
                        where d.PROKIRIXI_ID == prokirixiId
                        orderby d.TEACHERS.LASTNAME, d.AFM
                        select new AitisisGridViewModel
                        {
                            AITISI_ID = d.AITISI_ID,
                            PROKIRIXI_ID = d.PROKIRIXI_ID,
                            AFM = d.AFM,
                            LASTNAME = d.TEACHERS.LASTNAME + " " + d.TEACHERS.FIRSTNAME,
                            AITISI_PROTOCOL = d.AITISI_PROTOCOL,
                            AITISI_DATE = d.AITISI_DATE,
                            EIDIKOTITA = d.EIDIKOTITA,
                            SCHOOL_ID = d.SCHOOL_ID,
                            PERIFERIA_ID = d.PERIFERIA_ID
                        }).ToList();
            return data;
        }

        public void Create(AitisisGridViewModel data, int prokirixiId)
        {
            AITISIS entity = new AITISIS()
            {
                PROKIRIXI_ID = prokirixiId,
                AFM = data.AFM,
                SCHOOL_ID = data.SCHOOL_ID,
                AITISI_DATE = data.AITISI_DATE ?? DateTime.Now,
                AITISI_PROTOCOL = Common.GenerateProtocol(data.AITISI_DATE ?? DateTime.Now)
            };
            entities.AITISIS.Add(entity);
            entities.SaveChanges();

            SetFields(data, entity);
        }

        public void Update(AitisisGridViewModel data, int prokirixiId)
        {
            AITISIS entity = entities.AITISIS.Find(data.AITISI_ID);

            entity.PROKIRIXI_ID = prokirixiId;
            entity.AFM = data.AFM;
            entity.SCHOOL_ID = data.SCHOOL_ID;
            entity.AITISI_DATE = data.AITISI_DATE ?? DateTime.Now;
            entity.AITISI_PROTOCOL = data.AITISI_PROTOCOL;

            entities.Entry(entity).State = EntityState.Modified;
            entities.SaveChanges();
        }

        public void Destroy(AitisisGridViewModel data)
        {
            AITISIS entity = entities.AITISIS.Find(data.AITISI_ID);
            try
            {
                if (entity != null)
                {
                    entities.Entry(entity).State = EntityState.Deleted;
                    entities.AITISIS.Remove(entity);
                    entities.SaveChanges();
                }
            }
            catch { }
        }

        public AitisisViewModel GetModel(int aitisiID)
        {
            AitisisViewModel data;

            data = (from d in entities.AITISIS
                    where d.AITISI_ID == aitisiID
                    select new AitisisViewModel
                    {
                        AITISI_ID = d.AITISI_ID,
                        AFM = d.AFM,
                        DOY = d.TEACHERS.DOY,
                        AMKA = d.TEACHERS.AMKA,
                        FATHERNAME = d.TEACHERS.FATHERNAME,
                        LASTNAME = d.TEACHERS.LASTNAME,
                        FIRSTNAME = d.TEACHERS.FIRSTNAME,
                        AITISI_DATE = d.AITISI_DATE,
                        AITISI_PROTOCOL = d.AITISI_PROTOCOL,
                        ANERGIA = d.ANERGIA,
                        SOCIALGROUP = d.SOCIALGROUP,
                        SOCIALGROUP_PROTOCOL = d.SOCIALGROUP_PROTOCOL,
                        SOCIALGROUP_YPIRESIA = d.SOCIALGROUP_YPIRESIA,
                        N2190 = d.N2190,
                        KLADOS = d.KLADOS,
                        EIDIKOTITA = d.EIDIKOTITA,
                        EIDIKOTITA_GROUP = d.EIDIKOTITA_GROUP,
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
                        PERIFERIA_ID = d.PERIFERIA_ID,
                        SCHOOL_ID = d.SCHOOL_ID,
                        PROKIRIXI_ID = d.PROKIRIXI_ID,
                        EPAGELMA_STATUS = d.EPAGELMA_STATUS,
                        AITISIS_SCHOOLS = (from s in entities.AITISIS_SCHOOLS where s.AITISI_ID == aitisiID select s).ToList(),
                        CHILDREN = d.CHILDREN,
                        TRANSFERRED = d.TRANSFERRED ?? false,
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
                        EPIMORFOSI3 = d.EPIMORFOSI3 ?? false,
                        EPIMORFOSI3_HOURS = d.EPIMORFOSI3_HOURS ?? 0,
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
                        CHECK_STATUS = d.CHECK_STATUS ?? true,
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

        public AITISIS EditAitisi(AitisisViewModel data, int aitisiId, string auditor = null)
        {
            AITISIS entity = entities.AITISIS.Find(aitisiId);

            entity.AFM = data.AFM ?? entity.AFM;
            entity.AITISI_DATE = data.AITISI_DATE ?? entity.AITISI_DATE;
            entity.AITISI_PROTOCOL = data.AITISI_PROTOCOL ?? entity.AITISI_PROTOCOL;
            entity.ANERGIA = data.ANERGIA;
            entity.SOCIALGROUP = data.SOCIALGROUP;
            entity.SOCIALGROUP_PROTOCOL = data.SOCIALGROUP_PROTOCOL;
            entity.SOCIALGROUP_YPIRESIA = data.SOCIALGROUP_YPIRESIA;
            entity.N2190 = data.N2190;
            entity.KLADOS = data.EIDIKOTITA != null ? (from d in entities.SYS_EIDIKOTITES where d.EIDIKOTITA_ID == data.EIDIKOTITA select d.EIDIKOTITA_KLADOS_ID).FirstOrDefault() : entity.KLADOS;
            entity.EIDIKOTITA = data.EIDIKOTITA ?? entity.EIDIKOTITA;
            entity.EIDIKOTITA_GROUP = data.EIDIKOTITA.HasValue ? Common.GetEidikotitaGroupId((int)data.EIDIKOTITA) : null;
            entity.PTYXIO_TYPE = data.PTYXIO_TYPE;
            entity.PTYXIO_TITLOS = data.PTYXIO_TITLOS;
            entity.PTYXIO_BATHMOS = data.PTYXIO_BATHMOS;
            entity.PTYXIO_DATE = data.PTYXIO_DATE;
            entity.MSC = data.MSC;
            entity.MSC_DIARKEIA = data.MSC_DIARKEIA;
            entity.MSC_TITLOS = data.MSC_TITLOS;
            entity.PHD = data.PHD;
            entity.PHD_TITLOS = data.PHD_TITLOS;
            entity.PED = data.PED;
            entity.PED_TITLOS = data.PED_TITLOS;
            entity.PED_DIARKEIA = data.PED_DIARKEIA;
            entity.AED_MSC = data.AED_MSC;
            entity.AED_MSC_TITLOS = data.AED_MSC_TITLOS;
            entity.AED_PHD = data.AED_PHD;
            entity.AED_PHD_TITLOS = data.AED_PHD_TITLOS;
            entity.LANG_TEXT = data.LANG_TEXT;
            entity.LANG_LEVEL = data.LANG_LEVEL;
            entity.LANG_TITLOS = data.LANG_TITLOS;
            entity.COMPUTER_CERT = data.COMPUTER_CERT;
            entity.COMPUTER_TITLOS = data.COMPUTER_TITLOS;
            entity.EPIMORFOSI1 = data.EPIMORFOSI1;
            entity.EPIMORFOSI1_HOURS = data.EPIMORFOSI1_HOURS;
            entity.EPIMORFOSI2 = data.EPIMORFOSI2;
            entity.EPIMORFOSI2_HOURS = data.EPIMORFOSI2_HOURS;
            entity.BASIC_EDUCATION = data.BASIC_EDUCATION;
            entity.PERIFERIA_ID = data.PERIFERIA_ID;
            entity.SCHOOL_ID = data.SCHOOL_ID;
            entity.EPAGELMA_STATUS = data.EPAGELMA_STATUS;
            entity.TRANSFERRED = false;
            // new fields (2016)
            entity.CERTIFIED = data.CERTIFIED;
            entity.SOCIALGROUP1 = data.SOCIALGROUP1;
            entity.SOCIALGROUP2 = data.SOCIALGROUP2;
            entity.SOCIALGROUP3 = data.SOCIALGROUP3;
            entity.SOCIALGROUP4 = data.SOCIALGROUP4;
            entity.SOCIALGROUP1_DOC = data.SOCIALGROUP1_DOC;
            entity.SOCIALGROUP2_DOC = data.SOCIALGROUP2_DOC;
            entity.SOCIALGROUP3_DOC = data.SOCIALGROUP3_DOC;
            entity.SOCIALGROUP4_DOC = data.SOCIALGROUP4_DOC;
            entity.LANG1_TEXT = data.LANG1_TEXT == "Επιλέξτε..." ? "" : data.LANG1_TEXT;
            entity.LANG1_LEVEL = data.LANG1_LEVEL == "Επιλέξτε..." ? "" : data.LANG1_LEVEL;
            entity.LANG1_TITLOS = data.LANG1_TITLOS;
            entity.LANG2_TEXT = data.LANG2_TEXT == "Επιλέξτε..." ? "" : data.LANG2_TEXT;
            entity.LANG2_LEVEL = data.LANG2_LEVEL == "Επιλέξτε..." ? "" : data.LANG2_LEVEL;
            entity.LANG2_TITLOS = data.LANG2_TITLOS;
            entity.EPIMORFOSI3 = data.EPIMORFOSI3;
            entity.EPIMORFOSI3_HOURS = data.EPIMORFOSI3_HOURS;
            // Calculated Fields
            entity.AGE = AitisiRules.CalcAge(entity);
            entity.MORIA_ANERGIA = AitisiMoria.MoriaAnergia(entity);
            entity.MORIA_PTYXIO = AitisiMoria.MoriaDegree(entity);
            entity.MORIA_MSC = AitisiMoria.MoriaMsc(entity);
            entity.MORIA_PHD = AitisiMoria.MoriaPhd(entity);
            entity.MORIA_PED = AitisiMoria.MoriaPed(entity);
            entity.MORIA_AED_MSC = AitisiMoria.MoriaAedMsc(entity);
            entity.MORIA_AED_PHD = AitisiMoria.MoriaAedPhd(entity);
            entity.MORIA_LANG = AitisiMoria.MoriaLanguage(entity);
            entity.MORIA_COMPUTER = AitisiMoria.MoriaComputer(entity);
            entity.MORIA_EPIMORFOSI1 = AitisiMoria.MoriaEpimorfosi1(entity);
            entity.MORIA_EPIMORFOSI2 = AitisiMoria.MoriaEpimorfosi2(entity);
            entity.MORIA_EPIMORFOSI3 = AitisiMoria.MoriaEpimorfosi3(entity);
            entity.MORIA_TEACH = AitisiMoria.MoriaTeach(entity);
            entity.MORIA_WORK1 = AitisiMoria.MoriaWork1(entity);
            entity.MORIA_WORK2 = AitisiMoria.MoriaWork2(entity);
            entity.MORIA_WORK = AitisiMoria.MoriaWork(entity);
            entity.MORIA_SOCIAL = AitisiMoria.MoriaSocial(entity);
            entity.MORIA_TOTAL = AitisiMoria.MoriaTotal(entity);
            // Epitropes Fields
            entity.CHECK_STATUS = data.CHECK_STATUS;
            entity.APOKLEISMOS = data.APOKLEISMOS;
            entity.APOKLEISMOS_AITIA = data.APOKLEISMOS_AITIA;
            entity.MORIODOTISI_DATE = data.MORIODOTISI_DATE;
            entity.MORIODOTISI_PERSON = auditor;
            entity.EPITROPI1_TEXT = data.EPITROPI1_TEXT;
            entity.EPITROPI2_TEXT = data.EPITROPI2_TEXT;
            entity.ENSTASI = data.ENSTASI;
            entity.ENSTASI_AITIA = data.ENSTASI_AITIA;

            entities.Entry(entity).State = EntityState.Modified;
            entities.SaveChanges();
            return entity;
        }

        public ExperienceResultsViewModel GetResults(int aitisiId)
        {
            AITISIS aitisi = (from d in entities.AITISIS where d.AITISI_ID == aitisiId select d).FirstOrDefault();
            TEACHERS teacher = (from d in entities.TEACHERS where d.AFM == aitisi.AFM select d).FirstOrDefault();

            ExperienceResultsViewModel moriaResults = new ExperienceResultsViewModel()
            {
                AITISI_ID = aitisiId,
                LASTNAME = teacher.LASTNAME,
                FIRSTNAME = teacher.FIRSTNAME,

                EIDIKOTITA = (from d in entities.SYS_EIDIKOTITES
                              where d.EIDIKOTITA_ID == aitisi.EIDIKOTITA
                              select new SYS_EIDIKOTITESViewModel
                              {
                                  EIDIKOTITA_CODE = d.EIDIKOTITA_CODE,
                                  EIDIKOTITA_ID = d.EIDIKOTITA_ID,
                                  EIDIKOTITA_NAME = d.EIDIKOTITA_NAME,
                                  EIDIKOTITA_KLADOS_ID = d.EIDIKOTITA_KLADOS_ID
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
                WORK_MORIA_FINAL = (from d in entities.sqlEXP_WORK_FINAL where d.AITISI_ID == aitisiId select d.WORK_MORIA_FINAL).FirstOrDefault() ?? 0d,

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

            entities.Entry(aitisi).State = EntityState.Modified;
            entities.SaveChanges();

            moriaResults.AITISI = GetModel(aitisi.AITISI_ID);
            return moriaResults;
        }

        private void SetFields(AitisisGridViewModel data, AITISIS entity)
        {
            data.AITISI_ID = entity.AITISI_ID;
            var teacher = (from d in entities.TEACHERS where d.AFM == data.AFM select new { d.LASTNAME, d.FIRSTNAME }).FirstOrDefault();
            data.LASTNAME = teacher.LASTNAME + " " + teacher.FIRSTNAME;
            data.AITISI_PROTOCOL = entity.AITISI_PROTOCOL;
        }

        public void Dispose()
        {
            entities.Dispose();
        }
    }
}