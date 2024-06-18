using Pegasus.BPM;
using Pegasus.DAL;
using Pegasus.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Pegasus.Services
{
    public class WorkTeachingService : IWorkTeachingService, IDisposable
    {
        private readonly PegasusDBEntities entities;

        public WorkTeachingService(PegasusDBEntities entities)
        {
            this.entities = entities;
        }

        public IEnumerable<ViewModelTeaching> Read(int aitisiId)
        {
            var data = (from d in entities.EXP_TEACHING
                        where d.AITISI_ID == aitisiId
                        orderby d.TEACH_TYPE, d.SCHOOL_YEAR descending
                        select new ViewModelTeaching
                        {
                            EXP_ID = d.EXP_ID,
                            AITISI_ID = d.AITISI_ID,
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

        public void Create(ViewModelTeaching data, int aitisiId)
        {
            EXP_TEACHING entity = new EXP_TEACHING()
            {
                AITISI_ID = aitisiId,
                TEACH_TYPE = data.TEACH_TYPE,
                SCHOOL_YEAR = data.SCHOOL_YEAR,
                DATE_FROM = data.DATE_FROM,
                DATE_TO = data.DATE_TO,
                HOURS_WEEK = data.HOURS_WEEK,
                HOURS = data.HOURS,
                DOC_PROTOCOL = data.DOC_PROTOCOL,
                DOC_ORIGIN = data.DOC_ORIGIN,
                DOC_VALID = data.DOC_VALID,
                DOC_COMMENT = data.DOC_COMMENT,
                DUPLICATE = data.DUPLICATE ?? false,
            };
            entity.MORIA = (float)Kerberos.MoriaTeaching(entity);
            entities.EXP_TEACHING.Add(entity);
            entities.SaveChanges();

            data.EXP_ID = entity.EXP_ID;
        }

        public void Update(ViewModelTeaching data, int aitisiId)
        {
            EXP_TEACHING entity = entities.EXP_TEACHING.Find(data.EXP_ID);

            entity.AITISI_ID = aitisiId;
            entity.TEACH_TYPE = data.TEACH_TYPE;
            entity.SCHOOL_YEAR = data.SCHOOL_YEAR;
            entity.DATE_FROM = data.DATE_FROM;
            entity.DATE_TO = data.DATE_TO;
            entity.HOURS_WEEK = data.HOURS_WEEK;
            entity.HOURS = data.HOURS;
            entity.DOC_PROTOCOL = data.DOC_PROTOCOL;
            entity.DOC_ORIGIN = data.DOC_ORIGIN;
            entity.DOC_VALID = data.DOC_VALID;
            entity.DOC_COMMENT = data.DOC_COMMENT;
            entity.DUPLICATE = false;
            entity.MORIA = (float)Kerberos.MoriaTeaching(entity);

            entities.Entry(entity).State = EntityState.Modified;
            entities.SaveChanges();
        }

        public void Destroy(ViewModelTeaching data)
        {
            EXP_TEACHING entity = entities.EXP_TEACHING.Find(data.EXP_ID);

            if (entity != null)
            {
                entities.Entry(entity).State = EntityState.Deleted;
                entities.EXP_TEACHING.Remove(entity);
                entities.SaveChanges();
            }
        }

        public ViewModelTeaching Refresh(int entityId)
        {
            return entities.EXP_TEACHING.Select(d => new ViewModelTeaching
            {
                EXP_ID = d.EXP_ID,
                AITISI_ID = d.AITISI_ID,
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
            }).Where(d => d.EXP_ID.Equals(entityId)).FirstOrDefault();
        }

        public void Dispose()
        {
            entities.Dispose();
        }
    }
}