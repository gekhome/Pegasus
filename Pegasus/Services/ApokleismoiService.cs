using Pegasus.DAL;
using Pegasus.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Pegasus.Services
{
    public class ApokleismoiService : IApokleismoiService, IDisposable
    {
        private readonly PegasusDBEntities entities;

        public ApokleismoiService(PegasusDBEntities entities)
        {
            this.entities = entities;
        }

        public List<ApokleismoiViewModel> Read()
        {
            var data = (from d in entities.SYS_APOKLEISMOI
                        orderby d.APOKLEISMOS_TEXT
                        select new ApokleismoiViewModel
                        {
                            APOKLEISMOS_ID = d.APOKLEISMOS_ID,
                            APOKLEISMOS_TEXT = d.APOKLEISMOS_TEXT,
                        }).ToList();
            return data;
        }

        public void Create(ApokleismoiViewModel data)
        {
            SYS_APOKLEISMOI entity = new SYS_APOKLEISMOI()
            {
                APOKLEISMOS_TEXT = data.APOKLEISMOS_TEXT,
            };
            entities.SYS_APOKLEISMOI.Add(entity);
            entities.SaveChanges();

            data.APOKLEISMOS_ID = entity.APOKLEISMOS_ID;
        }

        public void Update(ApokleismoiViewModel data)
        {
            SYS_APOKLEISMOI entity = entities.SYS_APOKLEISMOI.Find(data.APOKLEISMOS_ID);

            entity.APOKLEISMOS_TEXT = data.APOKLEISMOS_TEXT;

            entities.Entry(entity).State = EntityState.Modified;
            entities.SaveChanges();
        }

        public void Destroy(ApokleismoiViewModel data)
        {
            SYS_APOKLEISMOI entity = entities.SYS_APOKLEISMOI.Find(data.APOKLEISMOS_ID);

            if (entity != null)
            {
                entities.Entry(entity).State = EntityState.Deleted;
                entities.SYS_APOKLEISMOI.Remove(entity);
                entities.SaveChanges();
            }
        }

        public void Dispose()
        {
            entities.Dispose();
        }
    }
}