using Pegasus.DAL;
using Pegasus.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Pegasus.Services
{
    public class TaxfreeService : ITaxfreeService, IDisposable
    {
        private readonly PegasusDBEntities entities;

        public TaxfreeService(PegasusDBEntities entities)
        {
            this.entities = entities;
        }

        public List<TaxFreeViewModel> Read()
        {
            var data = (from d in entities.SYS_TAXFREE
                        orderby d.YEAR_TEXT
                        select new TaxFreeViewModel
                        {
                            YEAR_ID = d.YEAR_ID,
                            YEAR_TEXT = d.YEAR_TEXT,
                            TAXFREE = d.TAXFREE,
                            NOMISMA = d.NOMISMA,
                        }).ToList();
            return data;
        }

        public void Create(TaxFreeViewModel data)
        {
            SYS_TAXFREE entity = new SYS_TAXFREE()
            {
                YEAR_TEXT = data.YEAR_TEXT,
                TAXFREE = data.TAXFREE,
                NOMISMA = data.NOMISMA
            };
            entities.SYS_TAXFREE.Add(entity);
            entities.SaveChanges();

            data.YEAR_ID = entity.YEAR_ID;
        }

        public void Update(TaxFreeViewModel data)
        {
            SYS_TAXFREE entity = entities.SYS_TAXFREE.Find(data.YEAR_ID);

            entity.YEAR_TEXT = data.YEAR_TEXT;
            entity.TAXFREE = data.TAXFREE;
            entity.NOMISMA = data.NOMISMA;

            entities.Entry(entity).State = EntityState.Modified;
            entities.SaveChanges();
        }

        public void Destroy(TaxFreeViewModel data)
        {
            SYS_TAXFREE entity = entities.SYS_TAXFREE.Find(data.YEAR_ID);

            if (entity != null)
            {
                entities.Entry(entity).State = EntityState.Deleted;
                entities.SYS_TAXFREE.Remove(entity);
                entities.SaveChanges();
            }
        }

        public void Dispose()
        {
            entities.Dispose();
        }
    }
}