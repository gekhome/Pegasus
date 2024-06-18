using Pegasus.DAL;
using Pegasus.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Pegasus.Services
{
    public class EidikotitesProkirixiService : IEidikotitesProkirixiService, IDisposable
    {
        private readonly PegasusDBEntities entities;

        public EidikotitesProkirixiService(PegasusDBEntities entities)
        {
            this.entities = entities;
        }

        public List<ProkirixisEidikotitesViewModel> Read(int prokirixiId, int schoolId)
        {
            var data = (from d in entities.PROKIRIXIS_EIDIKOTITES
                        where d.PROKIRIXI_ID == prokirixiId && d.SCHOOL_ID == schoolId
                        orderby d.SYS_EIDIKOTITES.EIDIKOTITA_KLADOS_ID, d.SYS_EIDIKOTITES.EIDIKOTITA_CODE, d.SYS_EIDIKOTITES.EIDIKOTITA_NAME
                        select new ProkirixisEidikotitesViewModel
                        {
                            PSE_ID = d.PSE_ID,
                            PROKIRIXI_ID = d.PROKIRIXI_ID,
                            SCHOOL_ID = d.SCHOOL_ID,
                            EIDIKOTITA_ID = d.EIDIKOTITA_ID
                        }).ToList();
            return data;
        }

        public void Create(ProkirixisEidikotitesViewModel data, int prokirixiId, int schoolId)
        {
            PROKIRIXIS_EIDIKOTITES entity = new PROKIRIXIS_EIDIKOTITES()
            {
                PROKIRIXI_ID = prokirixiId,
                SCHOOL_ID = schoolId,
                EIDIKOTITA_ID = data.EIDIKOTITA_ID
            };
            entities.PROKIRIXIS_EIDIKOTITES.Add(entity);
            entities.SaveChanges();

            data.PSE_ID = entity.PSE_ID;
        }

        public void Update(ProkirixisEidikotitesViewModel data, int prokirixiId, int schoolId)
        {
            PROKIRIXIS_EIDIKOTITES entity = entities.PROKIRIXIS_EIDIKOTITES.Find(data.PSE_ID);

            entity.PROKIRIXI_ID = prokirixiId;
            entity.SCHOOL_ID = schoolId;
            entity.EIDIKOTITA_ID = data.EIDIKOTITA_ID;

            entities.Entry(entity).State = EntityState.Modified;
            entities.SaveChanges();
        }

        public void Destroy(ProkirixisEidikotitesViewModel data)
        {
            PROKIRIXIS_EIDIKOTITES entity = entities.PROKIRIXIS_EIDIKOTITES.Find(data.PSE_ID);

            if (entity != null)
            {
                entities.Entry(entity).State = EntityState.Deleted;
                entities.PROKIRIXIS_EIDIKOTITES.Remove(entity);
                entities.SaveChanges();
            }
        }

        public void Dispose()
        {
            entities.Dispose();
        }
    }
}