using Pegasus.DAL;
using Pegasus.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Pegasus.Services
{
    public class EidikotitesService : IEidikotitesService, IDisposable
    {
        private readonly PegasusDBEntities entities;

        public EidikotitesService(PegasusDBEntities entities)
        {
            this.entities = entities;
        }

        public IEnumerable<EidikotitesViewModel> Read()
        {
            var data = (from d in entities.SYS_EIDIKOTITES
                        orderby d.EIDIKOTITA_KLADOS_ID, d.EIDIKOTITA_CODE, d.EIDIKOTITA_NAME
                        select new EidikotitesViewModel
                        {
                            EIDIKOTITA_ID = d.EIDIKOTITA_ID,
                            EIDIKOTITA_CODE = d.EIDIKOTITA_CODE,
                            EIDIKOTITA_NAME = d.EIDIKOTITA_NAME,
                            EIDIKOTITA_UNIFIED = d.EIDIKOTITA_UNIFIED,
                            KLADOS_UNIFIED = d.KLADOS_UNIFIED ?? 0,
                            EIDIKOTITA_KLADOS_ID = d.EIDIKOTITA_KLADOS_ID,
                            EIDIKOTITA_GROUP_ID = d.EIDIKOTITA_GROUP_ID ?? 0,
                            EDUCATION_CLASS = d.EDUCATION_CLASS
                        }).ToList();
            return data;
        }

        public void Create(EidikotitesViewModel data)
        {
            SYS_EIDIKOTITES entity = new SYS_EIDIKOTITES()
            {
                EIDIKOTITA_CODE = data.EIDIKOTITA_CODE,
                EIDIKOTITA_NAME = data.EIDIKOTITA_NAME,
                EIDIKOTITA_UNIFIED = data.EIDIKOTITA_UNIFIED,
                KLADOS_UNIFIED = data.KLADOS_UNIFIED,
                EIDIKOTITA_KLADOS_ID = data.EIDIKOTITA_KLADOS_ID,
                EIDIKOTITA_GROUP_ID = data.EIDIKOTITA_GROUP_ID,
            };
            entities.SYS_EIDIKOTITES.Add(entity);
            entities.SaveChanges();

            data.EIDIKOTITA_ID = entity.EIDIKOTITA_ID;
        }

        public void Update(EidikotitesViewModel data)
        {
            SYS_EIDIKOTITES entity = entities.SYS_EIDIKOTITES.Find(data.EIDIKOTITA_ID);

            entity.EIDIKOTITA_CODE = data.EIDIKOTITA_CODE;
            entity.EIDIKOTITA_NAME = data.EIDIKOTITA_NAME;
            entity.EIDIKOTITA_UNIFIED = data.EIDIKOTITA_UNIFIED;
            entity.KLADOS_UNIFIED = data.KLADOS_UNIFIED;
            entity.EIDIKOTITA_KLADOS_ID = data.EIDIKOTITA_KLADOS_ID;
            entity.EIDIKOTITA_GROUP_ID = data.EIDIKOTITA_GROUP_ID;

            entities.Entry(entity).State = EntityState.Modified;
            entities.SaveChanges();
        }

        public void Destroy(EidikotitesViewModel data)
        {
            SYS_EIDIKOTITES entity = entities.SYS_EIDIKOTITES.Find(data.EIDIKOTITA_ID);
            if (entity != null)
            {
                entities.Entry(entity).State = EntityState.Deleted;
                entities.SYS_EIDIKOTITES.Remove(entity);
                entities.SaveChanges();
            }
        }

        public void UpdateGroup(EidikotitesViewModel data)
        {
            SYS_EIDIKOTITES entity = entities.SYS_EIDIKOTITES.Find(data.EIDIKOTITA_ID);

            entity.EIDIKOTITA_GROUP_ID = data.EIDIKOTITA_GROUP_ID;

            entities.Entry(entity).State = EntityState.Modified;
            entities.SaveChanges();
        }

        public void Dispose()
        {
            entities.Dispose();
        }
    }
}