using Pegasus.BPM;
using Pegasus.DAL;
using Pegasus.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Pegasus.Services
{
    public class WorkFreelanceService : IWorkFreelanceService, IDisposable
    {
        private readonly PegasusDBEntities entities;

        public WorkFreelanceService(PegasusDBEntities entities)
        {
            this.entities = entities;
        }

        public IEnumerable<ViewModelFreelance> Read(int aitisiID)
        {
            var data = (from d in entities.EXP_FREELANCE
                        where d.AITISI_ID == aitisiID
                        orderby d.INCOME_YEAR descending, d.DATE_FROM descending
                        select new ViewModelFreelance
                        {
                            EXP_ID = d.EXP_ID,
                            AITISI_ID = d.AITISI_ID,
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

        public void Create(ViewModelFreelance data, int aitisiID)
        {
            EXP_FREELANCE entity = new EXP_FREELANCE()
            {
                AITISI_ID = aitisiID,
                INCOME_YEAR = data.INCOME_YEAR,
                INCOME = data.INCOME,
                INCOME_TAXFREE = setIncomeTaxfree((int)data.INCOME_YEAR),
                INCOME_NOMISMA = setIncomeNomisma((int)data.INCOME_YEAR),
                DATE_FROM = data.DATE_FROM,
                DATE_TO = data.DATE_TO,
                DAYS_AUTO = (float)Kerberos.SetDaysAutoFreelance(data),
                DAYS_MANUAL = data.DAYS_MANUAL,
                MORIA = data.MORIA,
                DOC_PROTOCOL = data.DOC_PROTOCOL,
                DOC_ORIGIN = data.DOC_ORIGIN,
                DOC_COMMENT = data.DOC_COMMENT,
                DOC_VALID = data.DOC_VALID,
                DUPLICATE = data.DUPLICATE ?? false
            };
            entity.DAYS_AUTO = (float)Kerberos.SetDaysAutoFreelance(entity);
            entity.MORIA = (float)Kerberos.MoriaFreelance(entity);

            entities.EXP_FREELANCE.Add(entity);
            entities.SaveChanges();

            data.EXP_ID = entity.EXP_ID;
        }

        public void Update(ViewModelFreelance data, int aitisiID)
        {
            EXP_FREELANCE entity = entities.EXP_FREELANCE.Find(data.EXP_ID);

            entity.AITISI_ID = aitisiID;
            entity.INCOME_YEAR = data.INCOME_YEAR;
            entity.INCOME = data.INCOME;
            entity.INCOME_TAXFREE = setIncomeTaxfree((int)data.INCOME_YEAR);
            entity.INCOME_NOMISMA = setIncomeNomisma((int)data.INCOME_YEAR);
            entity.DATE_FROM = data.DATE_FROM;
            entity.DATE_TO = data.DATE_TO;
            entity.DAYS_MANUAL = data.DAYS_MANUAL;
            entity.DOC_PROTOCOL = data.DOC_PROTOCOL;
            entity.DOC_ORIGIN = data.DOC_ORIGIN;
            entity.DOC_VALID = data.DOC_VALID;
            entity.DOC_COMMENT = data.DOC_COMMENT;
            entity.DUPLICATE = data.DUPLICATE ?? false;
            entity.DAYS_AUTO = (float)Kerberos.SetDaysAutoFreelance(data);
            entity.MORIA = (float)Kerberos.MoriaFreelance(entity);

            entities.Entry(entity).State = EntityState.Modified;
            entities.SaveChanges();
        }

        public void Destroy(ViewModelFreelance data)
        {
            EXP_FREELANCE entity = entities.EXP_FREELANCE.Find(data.EXP_ID);

            if (entity != null)
            {
                entities.Entry(entity).State = EntityState.Deleted;
                entities.EXP_FREELANCE.Remove(entity);
                entities.SaveChanges();
            }
        }

        public ViewModelFreelance Refresh(int entityId)
        {
            return entities.EXP_FREELANCE.Select(d => new ViewModelFreelance
            {
                EXP_ID = d.EXP_ID,
                AITISI_ID = d.AITISI_ID,
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
            }).Where(d => d.EXP_ID.Equals(entityId)).FirstOrDefault();
        }

        private float setIncomeTaxfree(int taxyear)
        {
            float IncomeTaxFree = 0;
            if (!(taxyear > 0)) return IncomeTaxFree;
            else
            {
                var itf = (from i in entities.SYS_TAXFREE
                           where i.YEAR_ID == taxyear
                           select new { i.TAXFREE }).FirstOrDefault();

                IncomeTaxFree = (float)itf.TAXFREE;
                return IncomeTaxFree;
            }
        }

        private string setIncomeNomisma(int taxyear)
        {
            string IncomeNomisma = "";
            if (!(taxyear > 0)) return IncomeNomisma;
            else
            {
                var itf = (from i in entities.SYS_TAXFREE
                           where i.YEAR_ID == taxyear
                           select new { i.NOMISMA }).FirstOrDefault();

                IncomeNomisma = itf.NOMISMA;
                return IncomeNomisma;
            }
        }

        public void Dispose()
        {
            entities.Dispose();
        }
    }
}