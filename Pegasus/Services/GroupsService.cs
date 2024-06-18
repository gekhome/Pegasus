using Pegasus.DAL;
using Pegasus.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Pegasus.Services
{
    public class GroupsService : IGroupsService, IDisposable
    {
        private readonly PegasusDBEntities entities;

        public GroupsService(PegasusDBEntities entities)
        {
            this.entities = entities;
        }

        public List<GroupsViewModel> Read()
        {
            var data = (from d in entities.SYS_EIDIKOTITES_GROUPS
                        orderby d.KLADOS_ID, d.GROUP_TEXT
                        select new GroupsViewModel
                        {
                            GROUP_ID = d.GROUP_ID,
                            GROUP_TEXT = d.GROUP_TEXT,
                            KLADOS_ID = d.KLADOS_ID
                        }).ToList();
            return data;
        }

        public void Create(GroupsViewModel data)
        {
            SYS_EIDIKOTITES_GROUPS entity = new SYS_EIDIKOTITES_GROUPS()
            {
                GROUP_TEXT = data.GROUP_TEXT,
                KLADOS_ID = data.KLADOS_ID
            };
            entities.SYS_EIDIKOTITES_GROUPS.Add(entity);
            entities.SaveChanges();

            data.GROUP_ID = entity.GROUP_ID;
        }

        public void Update(GroupsViewModel data)
        {
            SYS_EIDIKOTITES_GROUPS entity = entities.SYS_EIDIKOTITES_GROUPS.Find(data.GROUP_ID);

            entity.GROUP_TEXT = data.GROUP_TEXT;
            entity.KLADOS_ID = data.KLADOS_ID;

            entities.Entry(entity).State = EntityState.Modified;
            entities.SaveChanges();
        }

        public void Destroy(GroupsViewModel data)
        {
            SYS_EIDIKOTITES_GROUPS entity = entities.SYS_EIDIKOTITES_GROUPS.Find(data.GROUP_ID);

            if (entity != null)
            {
                entities.Entry(entity).State = EntityState.Deleted;
                entities.SYS_EIDIKOTITES_GROUPS.Remove(entity);
                entities.SaveChanges();
            }
        }

        public GroupsViewModel Refresh(int entityId)
        {
            return entities.SYS_EIDIKOTITES_GROUPS.Select(d => new GroupsViewModel
            {
                GROUP_ID = d.GROUP_ID,
                GROUP_TEXT = d.GROUP_TEXT,
                KLADOS_ID = d.KLADOS_ID
            }).Where(d => d.GROUP_ID.Equals(entityId)).FirstOrDefault();
        }

        public List<sqlEidikotitesSelectorViewModel> GetEidikotites(int groupId)
        {
            var data = (from d in entities.sqlEIDIKOTITES_SELECTOR
                        where d.EIDIKOTITA_GROUP_ID == groupId
                        orderby d.EIDIKOTITA_KLADOS_ID, d.EIDIKOTITA_DESC
                        select new sqlEidikotitesSelectorViewModel
                        {
                            EIDIKOTITA_ID = d.EIDIKOTITA_ID,
                            EIDIKOTITA_DESC = d.EIDIKOTITA_DESC,
                            EIDIKOTITA_GROUP_ID = d.EIDIKOTITA_GROUP_ID
                        }).ToList();
            return data;
        }

        public void SetEidikotita(sqlEidikotitesSelectorViewModel data, int groupId)
        {
            SYS_EIDIKOTITES entity = entities.SYS_EIDIKOTITES.Find(data.EIDIKOTITA_ID);

            entity.EIDIKOTITA_GROUP_ID = groupId;

            entities.Entry(entity).State = EntityState.Modified;
            entities.SaveChanges();
        }

        public void ResetEidikotita(sqlEidikotitesSelectorViewModel data)
        {
            SYS_EIDIKOTITES entity = entities.SYS_EIDIKOTITES.Find(data.EIDIKOTITA_ID);

            entity.EIDIKOTITA_GROUP_ID = null;

            entities.Entry(entity).State = EntityState.Modified;
            entities.SaveChanges();
        }

        public sqlEidikotitesSelectorViewModel RefreshEidikotita(int entityId)
        {
            var data = (from d in entities.sqlEIDIKOTITES_SELECTOR
                        where d.EIDIKOTITA_ID == entityId
                        select new sqlEidikotitesSelectorViewModel
                        {
                            EIDIKOTITA_ID = d.EIDIKOTITA_ID,
                            EIDIKOTITA_DESC = d.EIDIKOTITA_DESC,
                            EIDIKOTITA_GROUP_ID = d.EIDIKOTITA_GROUP_ID
                        }).FirstOrDefault();
            return data;
        }

        public void Dispose()
        {
            entities.Dispose();
        }
    }
}