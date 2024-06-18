using Pegasus.BPM;
using Pegasus.DAL;
using Pegasus.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Pegasus.Services
{
    public class UserSchoolService : IUserSchoolService, IDisposable
    {
        private readonly PegasusDBEntities entities;

        public UserSchoolService(PegasusDBEntities entities)
        {
            this.entities = entities;
        }

        public IEnumerable<UserSchoolViewModel> Read()
        {
            var data = (from a in entities.USER_SCHOOLS
                        where a.USER_SCHOOLID >= 100
                        select new UserSchoolViewModel
                        {
                            USER_ID = a.USER_ID,
                            USERNAME = a.USERNAME,
                            PASSWORD = a.PASSWORD,
                            USER_SCHOOLID = a.USER_SCHOOLID ?? 0,
                            ISACTIVE = a.ISACTIVE ?? false
                        }).ToList();
            return data;
        }

        public void Create(UserSchoolViewModel data)
        {
            USER_SCHOOLS entity = new USER_SCHOOLS()
            {
                USERNAME = data.USERNAME,
                PASSWORD = data.PASSWORD,
                USER_SCHOOLID = data.USER_SCHOOLID,
                ISACTIVE = data.ISACTIVE,
            };
            entities.USER_SCHOOLS.Add(entity);
            entities.SaveChanges();

            data.USER_ID = entity.USER_ID;
        }

        public void Update(UserSchoolViewModel data)
        {
            USER_SCHOOLS entity = entities.USER_SCHOOLS.Find(data.USER_ID);

            entity.USER_ID = data.USER_ID;
            entity.USERNAME = data.USERNAME;
            entity.PASSWORD = data.PASSWORD;
            entity.USER_SCHOOLID = data.USER_SCHOOLID;
            entity.ISACTIVE = data.ISACTIVE;

            entities.Entry(entity).State = EntityState.Modified;
            entities.SaveChanges();
        }

        public void Destroy(UserSchoolViewModel data)
        {
            USER_SCHOOLS entity = entities.USER_SCHOOLS.Find(data.USER_ID);

            if (entity != null)
            {
                entities.Entry(entity).State = EntityState.Deleted;
                entities.USER_SCHOOLS.Remove(entity);
                entities.SaveChanges();
            }
        }

        public void Dispose()
        {
            entities.Dispose();
        }
    }
}