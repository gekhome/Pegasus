using Pegasus.DAL;
using Pegasus.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Pegasus.Services
{
    public class ProkirixiService : IProkirixiService, IDisposable
    {
        private readonly PegasusDBEntities entities;

        public ProkirixiService(PegasusDBEntities entities)
        {
            this.entities = entities;
        }

        public List<ProkirixisViewModel> Read()
        {
            var data = (from d in entities.PROKIRIXIS
                        orderby d.SCHOOL_YEAR descending, d.DATE_START descending
                        select new ProkirixisViewModel
                        {
                            ID = d.ID,
                            SCHOOL_YEAR = d.SCHOOL_YEAR,
                            PROTOCOL = d.PROTOCOL,
                            FEK = d.FEK,
                            FOREAS = d.FOREAS,
                            DIOIKITIS = d.DIOIKITIS,
                            DATE_START = d.DATE_START,
                            DATE_END = d.DATE_END,
                            HOUR_START = d.HOUR_START,
                            HOUR_END = d.HOUR_END,
                            STATUS = d.STATUS,
                            ACTIVE = d.ACTIVE ?? false,
                            ADMIN = d.ADMIN ?? false,
                            USER_VIEW = d.USER_VIEW ?? false,
                            ENSTASEIS = d.ENSTASEIS ?? false
                        }).ToList();
            return data;
        }

        public void Create(ProkirixisViewModel data)
        {
            PROKIRIXIS entity = new PROKIRIXIS()
            {
                SCHOOL_YEAR = data.SCHOOL_YEAR,
                PROTOCOL = data.PROTOCOL,
                FEK = data.FEK,
                FOREAS = "Δ.ΥΠ.Α.",
                DIOIKITIS = data.DIOIKITIS,
                DATE_START = data.DATE_START,
                DATE_END = data.DATE_END,
                HOUR_START = data.HOUR_START,
                HOUR_END = data.HOUR_END,
                STATUS = data.STATUS,
                ACTIVE = data.ACTIVE,
                USER_VIEW = data.USER_VIEW,
                ENSTASEIS = data.ENSTASEIS,
                ADMIN = data.ADMIN
            };
            entities.PROKIRIXIS.Add(entity);
            entities.SaveChanges();

            data.ID = entity.ID;
        }

        public void Update(ProkirixisViewModel data)
        {
            PROKIRIXIS entity = entities.PROKIRIXIS.Find(data.ID);

            entity.SCHOOL_YEAR = data.SCHOOL_YEAR;
            entity.PROTOCOL = data.PROTOCOL;
            entity.FEK = data.FEK;
            entity.FOREAS = "Δ.ΥΠ.Α.";
            entity.DIOIKITIS = data.DIOIKITIS;
            entity.DATE_START = data.DATE_START;
            entity.DATE_END = data.DATE_END;
            entity.HOUR_START = data.HOUR_START;
            entity.HOUR_END = data.HOUR_END;
            entity.STATUS = data.STATUS;
            entity.ACTIVE = data.ACTIVE;
            entity.USER_VIEW = data.USER_VIEW;
            entity.ENSTASEIS = data.ENSTASEIS;
            entity.ADMIN = data.ADMIN;

            entities.Entry(entity).State = EntityState.Modified;
            entities.SaveChanges();
        }

        public void Destroy(ProkirixisViewModel data)
        {
            PROKIRIXIS entity = entities.PROKIRIXIS.Find(data.ID);

            if (entity != null)
            {
                entities.Entry(entity).State = EntityState.Deleted;
                entities.PROKIRIXIS.Remove(entity);
                entities.SaveChanges();
            }
        }

        public void Dispose()
        {
            entities.Dispose();
        }
    }
}