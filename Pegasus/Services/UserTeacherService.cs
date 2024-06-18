using Pegasus.BPM;
using Pegasus.DAL;
using Pegasus.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Pegasus.Services
{
    public class UserTeacherService : IUserTeacherService, IDisposable
    {
        private readonly PegasusDBEntities entities;

        public UserTeacherService(PegasusDBEntities entities)
        {
            this.entities = entities;
        }

        public IEnumerable<UserTeacherEditViewModel> Read()
        {
            var data = (from a in entities.USER_TEACHERS
                        select new UserTeacherEditViewModel
                        {
                            USER_ID = a.USER_ID,
                            USERNAME = a.USERNAME,
                            PASSWORD = a.PASSWORD,
                            AFM = a.USER_AFM,
                            CREATEDATE = a.CREATEDATE,
                            ISACTIVE = a.ISACTIVE ?? false
                        }).ToList();
            return data;
        }

        public void Create(UserTeacherEditViewModel data)
        {
            USER_TEACHERS entity = new USER_TEACHERS()
            {
                USERNAME = data.USERNAME,
                PASSWORD = data.PASSWORD,
                USER_AFM = data.AFM,
                CREATEDATE = data.CREATEDATE
            };
            entities.USER_TEACHERS.Add(entity);
            entities.SaveChanges();

            data.USER_ID = entity.USER_ID;
        }

        public void Update(UserTeacherEditViewModel data)
        {
            USER_TEACHERS entity = entities.USER_TEACHERS.Find(data.USER_ID);

            entity.USER_ID = data.USER_ID;
            entity.USERNAME = data.USERNAME;
            entity.PASSWORD = data.PASSWORD;
            entity.USER_AFM = data.AFM;
            entity.CREATEDATE = data.CREATEDATE;

            entities.Entry(entity).State = EntityState.Modified;
            entities.SaveChanges();
        }

        public void Destroy(UserTeacherEditViewModel data)
        {
            USER_TEACHERS entity = entities.USER_TEACHERS.Find(data.USER_ID);

            if (entity != null)
            {
                entities.Entry(entity).State = EntityState.Deleted;
                entities.USER_TEACHERS.Remove(entity);
                entities.SaveChanges();
            }
        }

        public UserTeacherEditViewModel Refresh(int entityId)
        {
            return entities.USER_TEACHERS.Select(d => new UserTeacherEditViewModel
            {
                USER_ID = d.USER_ID,
                USERNAME = d.USERNAME,
                PASSWORD = d.PASSWORD,
                AFM = d.USER_AFM,
                CREATEDATE = d.CREATEDATE,
                ISACTIVE = d.ISACTIVE ?? false
            }).Where(d => d.USER_ID.Equals(entityId)).FirstOrDefault();
        }

        public List<TeacherAccountInfoViewModel> ReadInfo(string afm)
        {
            var data = (from d in entities.sqlTEACHER_ACCOUNTS_INFO
                        where d.AFM == afm
                        select new TeacherAccountInfoViewModel
                        {
                            USER_ID = d.USER_ID,
                            USERNAME = d.USERNAME,
                            FULLNAME = d.FULLNAME,
                            FATHERNAME = d.FATHERNAME,
                            MOTHERNAME = d.MOTHERNAME,
                            BIRTHDATE = d.BIRTHDATE,
                            TELEPHONES = d.TELEPHONES,
                            AMKA = d.AMKA,
                            AFM = d.AFM
                        }).ToList();
            return data;
        }

        public void Dispose()
        {
            entities.Dispose();
        }
    }
}