using Pegasus.BPM;
using Pegasus.DAL;
using Pegasus.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Pegasus.Services
{
    public class WorkVocationService : IWorkVocationService, IDisposable
    {
        private readonly PegasusDBEntities entities;

        public WorkVocationService(PegasusDBEntities entities)
        {
            this.entities = entities;
        }

        public IEnumerable<ViewModelVocational> Read(int aitisiID)
        {
            var data = (from d in entities.EXP_VOCATIONAL
                        where d.AITISI_ID == aitisiID
                        orderby d.DATE_FROM descending
                        select new ViewModelVocational
                        {
                            AITISI_ID = d.AITISI_ID,
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

        public void Create(ViewModelVocational data, int aitisiID)
        {
            EXP_VOCATIONAL entity = new EXP_VOCATIONAL()
            {
                AITISI_ID = aitisiID,
                DATE_FROM = data.DATE_FROM,
                DATE_TO = data.DATE_TO,
                DAYS_AUTO = (float)Kerberos.SetDaysAutoVocational(data),
                DAYS_MANUAL = data.DAYS_MANUAL,
                MORIA = data.MORIA,
                DOC_PROTOCOL = data.DOC_PROTOCOL,
                DOC_ORIGIN = data.DOC_ORIGIN,
                DOC_COMMENT = data.DOC_COMMENT,
                DOC_VALID = data.DOC_VALID,
                DUPLICATE = data.DUPLICATE ?? false
            };
            entity.DAYS_AUTO = (float)Kerberos.SetDaysAutoVocational(entity);
            entity.MORIA = (float)Kerberos.MoriaVocational(entity);

            entities.EXP_VOCATIONAL.Add(entity);
            entities.SaveChanges();

            data.EXP_ID = entity.EXP_ID;
        }

        public void Update(ViewModelVocational data, int aitisiID)
        {
            EXP_VOCATIONAL entity = entities.EXP_VOCATIONAL.Find(data.EXP_ID);

            entity.AITISI_ID = aitisiID;
            entity.DATE_FROM = data.DATE_FROM;
            entity.DATE_TO = data.DATE_TO;
            entity.DAYS_AUTO = (float)Kerberos.SetDaysAutoVocational(data);
            entity.DAYS_MANUAL = data.DAYS_MANUAL;
            entity.MORIA = data.MORIA;
            entity.DOC_PROTOCOL = data.DOC_PROTOCOL;
            entity.DOC_ORIGIN = data.DOC_ORIGIN;
            entity.DOC_VALID = data.DOC_VALID;
            entity.DOC_COMMENT = data.DOC_COMMENT;
            entity.DUPLICATE = data.DUPLICATE ?? false;
            entity.DAYS_AUTO = (float)Kerberos.SetDaysAutoVocational(entity);
            entity.MORIA = (float)Kerberos.MoriaVocational(entity);

            entities.Entry(entity).State = EntityState.Modified;
            entities.SaveChanges();
        }

        public void Destroy(ViewModelVocational data)
        {
            EXP_VOCATIONAL entity = entities.EXP_VOCATIONAL.Find(data.EXP_ID);

            if (entity != null)
            {
                entities.Entry(entity).State = EntityState.Deleted;
                entities.EXP_VOCATIONAL.Remove(entity);
                entities.SaveChanges();
            }
        }

        public ViewModelVocational Refresh(int entityId)
        {
            return entities.EXP_VOCATIONAL.Select(d => new ViewModelVocational
            {
                AITISI_ID = d.AITISI_ID,
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
            }).Where(d => d.EXP_ID.Equals(entityId)).FirstOrDefault();
        }

        public void Dispose()
        {
            entities.Dispose();
        }
    }
}