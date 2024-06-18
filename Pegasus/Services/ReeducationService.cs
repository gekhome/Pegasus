using Pegasus.BPM;
using Pegasus.DAL;
using Pegasus.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Pegasus.Services
{
    public class ReeducationService : IReeducationService, IDisposable
    {
        private readonly PegasusDBEntities entities;

        public ReeducationService(PegasusDBEntities entities)
        {
            this.entities = entities;
        }

        public List<ReeducationViewModel> Read(int aitisiID)
        {
            var data = (from d in entities.REEDUCATION
                        where d.AITISI_ID == aitisiID
                        select new ReeducationViewModel
                        {
                            EDUCATION_ID = d.EDUCATION_ID,
                            AFM = d.AFM,
                            AITISI_ID = d.AITISI_ID,
                            PROKIRIXI_ID = d.PROKIRIXI_ID,
                            CERTIFICATE_DATE = d.CERTIFICATE_DATE,
                            CERTIFICATE_FOREAS = d.CERTIFICATE_FOREAS,
                            CERTIFICATE_TITLE = d.CERTIFICATE_TITLE,
                            CERTIFICATE_HOURS = d.CERTIFICATE_HOURS
                        }).ToList();
            return data;
        }

        public void Create(ReeducationViewModel data, int aitisiID)
        {
            int prokirixiID = 0;
            string afm = "";

            var aitisi = (from d in entities.AITISIS where d.AITISI_ID == aitisiID select d).FirstOrDefault();
            if (aitisi != null)
            {
                afm = aitisi.AFM;
                prokirixiID = (int)aitisi.PROKIRIXI_ID;
            }

            REEDUCATION entity = new REEDUCATION()
            {
                AFM = afm,
                AITISI_ID = aitisiID,
                PROKIRIXI_ID = prokirixiID,
                CERTIFICATE_DATE = data.CERTIFICATE_DATE,
                CERTIFICATE_FOREAS = data.CERTIFICATE_FOREAS,
                CERTIFICATE_TITLE = data.CERTIFICATE_TITLE,
                CERTIFICATE_HOURS = data.CERTIFICATE_HOURS
            };
            entities.REEDUCATION.Add(entity);
            entities.SaveChanges();

            data.EDUCATION_ID = entity.EDUCATION_ID;
        }

        public void Update(ReeducationViewModel data, int aitisiID)
        {
            int prokirixiID = 0;
            string afm = "";

            var aitisi = (from d in entities.AITISIS where d.AITISI_ID == aitisiID select d).FirstOrDefault();
            if (aitisi != null)
            {
                afm = aitisi.AFM;
                prokirixiID = (int)aitisi.PROKIRIXI_ID;
            }

            REEDUCATION entity = entities.REEDUCATION.Find(data.EDUCATION_ID);

            entity.AFM = afm;
            entity.PROKIRIXI_ID = prokirixiID;
            entity.AITISI_ID = aitisiID;
            entity.CERTIFICATE_DATE = data.CERTIFICATE_DATE;
            entity.CERTIFICATE_FOREAS = data.CERTIFICATE_FOREAS;
            entity.CERTIFICATE_TITLE = data.CERTIFICATE_TITLE;
            entity.CERTIFICATE_HOURS = data.CERTIFICATE_HOURS;

            entities.Entry(entity).State = EntityState.Modified;
            entities.SaveChanges();
        }

        public void Destroy(ReeducationViewModel data)
        {
            REEDUCATION entity = entities.REEDUCATION.Find(data.EDUCATION_ID);

            if (entity != null)
            {
                entities.Entry(entity).State = EntityState.Deleted;
                entities.REEDUCATION.Remove(entity);
                entities.SaveChanges();
            }
        }

        public ReeducationViewModel Refresh(int entityId)
        {
            return entities.REEDUCATION.Select(d => new ReeducationViewModel
            {
                EDUCATION_ID = d.EDUCATION_ID,
                AFM = d.AFM,
                AITISI_ID = d.AITISI_ID,
                PROKIRIXI_ID = d.PROKIRIXI_ID,
                CERTIFICATE_DATE = d.CERTIFICATE_DATE,
                CERTIFICATE_FOREAS = d.CERTIFICATE_FOREAS,
                CERTIFICATE_TITLE = d.CERTIFICATE_TITLE,
                CERTIFICATE_HOURS = d.CERTIFICATE_HOURS
            }).Where(d => d.EDUCATION_ID.Equals(entityId)).FirstOrDefault();
        }

        public void Dispose()
        {
            entities.Dispose();
        }
    }
}