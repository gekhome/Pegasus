using Pegasus.BPM;
using Pegasus.DAL;
using Pegasus.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Pegasus.Services
{
    public class AitisiTeacherService : IAitisiTeacherService, IDisposable
    {
        private readonly PegasusDBEntities entities;

        public AitisiTeacherService(PegasusDBEntities entities)
        {
            this.entities = entities;
        }

        public IEnumerable<AitisisGridViewModel> Read(int prokirixiId, string Afm)
        {
            var data = (from d in entities.AITISIS
                        where d.AFM == Afm && d.PROKIRIXI_ID == prokirixiId
                        orderby d.AITISI_PROTOCOL descending
                        select new AitisisGridViewModel
                        {
                            AITISI_ID = d.AITISI_ID,
                            AITISI_PROTOCOL = d.AITISI_PROTOCOL,
                            EIDIKOTITA = d.EIDIKOTITA,
                            SCHOOL_ID = d.SCHOOL_ID,
                            PERIFERIA_ID = d.PERIFERIA_ID
                        }).ToList();
            return data;
        }

        public void Destroy(AitisisGridViewModel data)
        {
            AITISIS entity = entities.AITISIS.Find(data.AITISI_ID);
            try
            {
                entities.Entry(entity).State = EntityState.Deleted;
                entities.AITISIS.Remove(entity);
                entities.SaveChanges();

                AitisiOriginalDestroy(data.AITISI_ID);
            }
            catch { }
        }

        public AITISIS Create(AitisisViewModel model, int prokirixiId, string AFM)
        {
            AITISIS entity = new AITISIS();

            entity.PROKIRIXI_ID = prokirixiId;
            entity.AFM = AFM;
            entity.AITISI_DATE = DateTime.Now.Date;
            entity.AITISI_PROTOCOL = Common.GenerateProtocol();
            entity.ANERGIA = model.ANERGIA;
            entity.SOCIALGROUP = model.SOCIALGROUP;
            entity.SOCIALGROUP_PROTOCOL = model.SOCIALGROUP_PROTOCOL;
            entity.SOCIALGROUP_YPIRESIA = model.SOCIALGROUP_YPIRESIA;
            entity.N2190 = model.N2190;
            entity.KLADOS = model.KLADOS;
            entity.EIDIKOTITA = model.EIDIKOTITA ?? entity.EIDIKOTITA;
            entity.EIDIKOTITA_GROUP = model.EIDIKOTITA.HasValue ? Common.GetEidikotitaGroupId((int)model.EIDIKOTITA) : null;
            entity.PTYXIO_TYPE = model.PTYXIO_TYPE;
            entity.PTYXIO_TITLOS = model.PTYXIO_TITLOS;
            entity.PTYXIO_BATHMOS = model.PTYXIO_BATHMOS;
            entity.PTYXIO_DATE = model.PTYXIO_DATE;
            entity.MSC = model.MSC;
            entity.MSC_DIARKEIA = model.MSC_DIARKEIA;
            entity.MSC_TITLOS = model.MSC_TITLOS;
            entity.PHD = model.PHD;
            entity.PHD_TITLOS = model.PHD_TITLOS;
            entity.PED = model.PED;
            entity.PED_TITLOS = model.PED_TITLOS;
            entity.PED_DIARKEIA = model.PED_DIARKEIA;
            entity.AED_MSC = model.AED_MSC;
            entity.AED_MSC_TITLOS = model.AED_MSC_TITLOS;
            entity.AED_PHD = model.AED_PHD;
            entity.AED_PHD_TITLOS = model.AED_PHD_TITLOS;
            entity.LANG_TEXT = model.LANG_TEXT;
            entity.LANG_LEVEL = model.LANG_LEVEL;
            entity.LANG_TITLOS = model.LANG_TITLOS;
            entity.COMPUTER_CERT = model.COMPUTER_CERT;
            entity.COMPUTER_TITLOS = model.COMPUTER_TITLOS;
            entity.EPIMORFOSI1 = model.EPIMORFOSI1;
            entity.EPIMORFOSI1_HOURS = model.EPIMORFOSI1_HOURS;
            entity.EPIMORFOSI2 = model.EPIMORFOSI2;
            entity.EPIMORFOSI2_HOURS = model.EPIMORFOSI2_HOURS;
            entity.BASIC_EDUCATION = model.BASIC_EDUCATION;
            entity.PERIFERIA_ID = model.PERIFERIA_ID;
            entity.SCHOOL_ID = model.SCHOOL_ID;
            entity.EPAGELMA_STATUS = model.EPAGELMA_STATUS;
            entity.CHECK_STATUS = false;
            entity.TRANSFERRED = false;
            // new fields (2016)
            entity.CERTIFIED = model.CERTIFIED;
            entity.SOCIALGROUP1 = model.SOCIALGROUP1;
            entity.SOCIALGROUP2 = model.SOCIALGROUP2;
            entity.SOCIALGROUP3 = model.SOCIALGROUP3;
            entity.SOCIALGROUP4 = model.SOCIALGROUP4;
            entity.SOCIALGROUP1_DOC = model.SOCIALGROUP1_DOC;
            entity.SOCIALGROUP2_DOC = model.SOCIALGROUP2_DOC;
            entity.SOCIALGROUP3_DOC = model.SOCIALGROUP3_DOC;
            entity.SOCIALGROUP4_DOC = model.SOCIALGROUP4_DOC;
            entity.LANG1_TEXT = model.LANG1_TEXT == "Επιλέξτε..." ? "" : model.LANG1_TEXT;
            entity.LANG1_LEVEL = model.LANG1_LEVEL == "Επιλέξτε..." ? "" : model.LANG1_LEVEL;
            entity.LANG1_TITLOS = model.LANG1_TITLOS;
            entity.LANG2_TEXT = model.LANG2_TEXT == "Επιλέξτε..." ? "" : model.LANG2_TEXT;
            entity.LANG2_LEVEL = model.LANG2_LEVEL == "Επιλέξτε..." ? "" : model.LANG2_LEVEL;
            entity.LANG2_TITLOS = model.LANG2_TITLOS;
            entity.EPIMORFOSI3 = model.EPIMORFOSI3;
            entity.EPIMORFOSI3_HOURS = model.EPIMORFOSI3_HOURS;
            // Calculated Fields
            entity.AGE = AitisiRules.CalcAge(entity);
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
            entity.MORIA_ANERGIA = AitisiMoria.MoriaAnergia(entity);
            entity.MORIA_SOCIAL = AitisiMoria.MoriaSocial(entity);
            entity.MORIA_TOTAL = AitisiMoria.MoriaTotal(entity);

            entities.Entry(entity).State = EntityState.Added;
            entities.AITISIS.Add(entity);
            entities.SaveChanges();

            return entity;
        }

        public AITISIS Update(AitisisViewModel model, int prokirixiId, string AFM)
        {
            AITISIS entity = entities.AITISIS.Find(model.AITISI_ID);

            entity.PROKIRIXI_ID = prokirixiId;
            entity.AFM = AFM;
            entity.AITISI_DATE = model.AITISI_DATE ?? entity.AITISI_DATE;
            entity.AITISI_PROTOCOL = model.AITISI_PROTOCOL ?? entity.AITISI_PROTOCOL;
            entity.ANERGIA = model.ANERGIA;
            entity.SOCIALGROUP = model.SOCIALGROUP;
            entity.SOCIALGROUP_PROTOCOL = model.SOCIALGROUP_PROTOCOL;
            entity.SOCIALGROUP_YPIRESIA = model.SOCIALGROUP_YPIRESIA;
            entity.N2190 = model.N2190;
            entity.KLADOS = model.KLADOS;
            entity.EIDIKOTITA = model.EIDIKOTITA ?? entity.EIDIKOTITA;
            entity.EIDIKOTITA_GROUP = model.EIDIKOTITA.HasValue ? Common.GetEidikotitaGroupId((int)model.EIDIKOTITA) : null;
            entity.PTYXIO_TYPE = model.PTYXIO_TYPE;
            entity.PTYXIO_TITLOS = model.PTYXIO_TITLOS;
            entity.PTYXIO_BATHMOS = model.PTYXIO_BATHMOS;
            entity.PTYXIO_DATE = model.PTYXIO_DATE;
            entity.MSC = model.MSC;
            entity.MSC_DIARKEIA = model.MSC_DIARKEIA;
            entity.MSC_TITLOS = model.MSC_TITLOS;
            entity.PHD = model.PHD;
            entity.PHD_TITLOS = model.PHD_TITLOS;
            entity.PED = model.PED;
            entity.PED_TITLOS = model.PED_TITLOS;
            entity.PED_DIARKEIA = model.PED_DIARKEIA;
            entity.AED_MSC = model.AED_MSC;
            entity.AED_MSC_TITLOS = model.AED_MSC_TITLOS;
            entity.AED_PHD = model.AED_PHD;
            entity.AED_PHD_TITLOS = model.AED_PHD_TITLOS;
            entity.LANG_TEXT = model.LANG_TEXT;
            entity.LANG_LEVEL = model.LANG_LEVEL;
            entity.LANG_TITLOS = model.LANG_TITLOS;
            entity.COMPUTER_CERT = model.COMPUTER_CERT;
            entity.COMPUTER_TITLOS = model.COMPUTER_TITLOS;
            entity.EPIMORFOSI1 = model.EPIMORFOSI1;
            entity.EPIMORFOSI1_HOURS = model.EPIMORFOSI1_HOURS;
            entity.EPIMORFOSI2 = model.EPIMORFOSI2;
            entity.EPIMORFOSI2_HOURS = model.EPIMORFOSI2_HOURS;
            entity.BASIC_EDUCATION = model.BASIC_EDUCATION;
            entity.PERIFERIA_ID = model.PERIFERIA_ID;
            entity.SCHOOL_ID = model.SCHOOL_ID;
            entity.EPAGELMA_STATUS = model.EPAGELMA_STATUS;
            entity.CHECK_STATUS = false;
            entity.TRANSFERRED = false;
            // new fields (2016)
            entity.CERTIFIED = model.CERTIFIED;
            entity.SOCIALGROUP1 = model.SOCIALGROUP1;
            entity.SOCIALGROUP2 = model.SOCIALGROUP2;
            entity.SOCIALGROUP3 = model.SOCIALGROUP3;
            entity.SOCIALGROUP4 = model.SOCIALGROUP4;
            entity.SOCIALGROUP1_DOC = model.SOCIALGROUP1_DOC;
            entity.SOCIALGROUP2_DOC = model.SOCIALGROUP2_DOC;
            entity.SOCIALGROUP3_DOC = model.SOCIALGROUP3_DOC;
            entity.SOCIALGROUP4_DOC = model.SOCIALGROUP4_DOC;
            entity.LANG1_TEXT = model.LANG1_TEXT == "Επιλέξτε..." ? "" : model.LANG1_TEXT;
            entity.LANG1_LEVEL = model.LANG1_LEVEL == "Επιλέξτε..." ? "" : model.LANG1_LEVEL;
            entity.LANG1_TITLOS = model.LANG1_TITLOS;
            entity.LANG2_TEXT = model.LANG2_TEXT == "Επιλέξτε..." ? "" : model.LANG2_TEXT;
            entity.LANG2_LEVEL = model.LANG2_LEVEL == "Επιλέξτε..." ? "" : model.LANG2_LEVEL;
            entity.LANG2_TITLOS = model.LANG2_TITLOS;
            entity.EPIMORFOSI3 = model.EPIMORFOSI3;
            entity.EPIMORFOSI3_HOURS = model.EPIMORFOSI3_HOURS;
            // Calculated Fields
            entity.AGE = AitisiRules.CalcAge(entity);
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
            entity.MORIA_ANERGIA = AitisiMoria.MoriaAnergia(entity);
            entity.MORIA_SOCIAL = AitisiMoria.MoriaSocial(entity);
            entity.MORIA_TOTAL = AitisiMoria.MoriaTotal(entity);

            entities.Entry(entity).State = EntityState.Modified;
            entities.SaveChanges();

            return entity;
        }

        public AitisisViewModel GetModel(int aitisiId)
        {
            AitisisViewModel data;

            data = (from d in entities.AITISIS
                    where d.AITISI_ID == aitisiId
                    select new AitisisViewModel
                    {
                        AITISI_ID = d.AITISI_ID,
                        AFM = d.AFM,
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
                        AITISIS_SCHOOLS = (from s in entities.AITISIS_SCHOOLS where s.AITISI_ID == aitisiId select s).ToList(),
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
                        MORIA_TOTAL = d.MORIA_TOTAL ?? 0
                    }).FirstOrDefault();
            return data;
        }

        private void AitisiOriginalDestroy(int aitisiId)
        {
            AITISIS_ORIGINAL entity = entities.AITISIS_ORIGINAL.Find(aitisiId);

            if (entity != null)
            {
                entities.Entry(entity).State = EntityState.Deleted;
                entities.AITISIS_ORIGINAL.Remove(entity);
                entities.SaveChanges();
            }
        }

        public void Dispose()
        {
            entities.Dispose();
        }
    }
}