using Pegasus.DAL;
using Pegasus.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Pegasus.Services
{
    public class AitisiSchoolsService : IAitisiSchoolsService, IDisposable
    {
        private readonly PegasusDBEntities entities;

        public AitisiSchoolsService(PegasusDBEntities entities)
        {
            this.entities = entities;
        }

        public List<AITISI_SCHOOLSViewModel> Read(int aitisiId)
        {
            var data = (from school in entities.AITISIS_SCHOOLS
                        where school.AITISI_ID == aitisiId
                        orderby school.SCHOOL
                        select new AITISI_SCHOOLSViewModel
                        {
                            ID = school.ID,
                            AITISI_ID = school.AITISI_ID,
                            PERIFERIA_ID = school.PERIFERIA_ID,
                            SCHOOL = school.SCHOOL,
                            SCHOOL_TYPE = school.SCHOOL_TYPE,
                            PROKIRIXI_ID = school.PROKIRIXI_ID
                        }).ToList();
            return data;
        }

        public void Create(AITISI_SCHOOLSViewModel data, int prokirixiId, int aitisiId)
        {
            AITISIS_SCHOOLS entity = new AITISIS_SCHOOLS()
            {
                //AITISIS = entities.AITISIS.Find(aitisiId),
                AITISI_ID = aitisiId,
                PERIFERIA_ID = (from d in entities.AITISIS where d.AITISI_ID == aitisiId select d.PERIFERIA_ID).FirstOrDefault() ?? 0,
                SCHOOL_TYPE = data.SCHOOL_TYPE,
                SCHOOL = data.SCHOOL,
                PROKIRIXI_ID = prokirixiId
            };
            entities.Entry(entity).State = EntityState.Added;
            entities.AITISIS_SCHOOLS.Add(entity);
            entities.SaveChanges();

            data.ID = entity.ID;
        }

        public void Update(AITISI_SCHOOLSViewModel data, int prokirixiId, int aitisiId)
        {
            AITISIS_SCHOOLS entity = entities.AITISIS_SCHOOLS.Find(data.ID);

            entity.AITISI_ID = aitisiId;
            entity.PERIFERIA_ID = data.PERIFERIA_ID;
            entity.SCHOOL = data.SCHOOL;
            entity.SCHOOL_TYPE = data.SCHOOL_TYPE;
            entity.PROKIRIXI_ID = prokirixiId;

            entities.Entry(entity).State = EntityState.Modified;
            entities.SaveChanges();
        }

        public void Destroy(AITISI_SCHOOLSViewModel data)
        {
            AITISIS_SCHOOLS entity = entities.AITISIS_SCHOOLS.Find(data.ID);

            if (entity != null)
            {
                entities.Entry(entity).State = EntityState.Deleted;
                entities.AITISIS_SCHOOLS.Remove(entity);
                entities.SaveChanges();
            }
        }

        public AITISI_SCHOOLSViewModel Refresh(int entityId)
        {
            return entities.AITISIS_SCHOOLS.Select(d => new AITISI_SCHOOLSViewModel
            {
                ID = d.ID,
                AITISI_ID = d.AITISI_ID,
                PERIFERIA_ID = d.PERIFERIA_ID,
                SCHOOL = d.SCHOOL,
                SCHOOL_TYPE = d.SCHOOL_TYPE,
                PROKIRIXI_ID = d.PROKIRIXI_ID
            }).Where(d => d.ID.Equals(entityId)).FirstOrDefault();
        }

        public void Dispose()
        {
            entities.Dispose();
        }
    }
}