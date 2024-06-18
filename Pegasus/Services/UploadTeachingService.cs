using Pegasus.BPM;
using Pegasus.DAL;
using Pegasus.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Pegasus.Services
{
    public class UploadTeachingService : IUploadTeachingService, IDisposable
    {
        private readonly PegasusDBEntities entities;

        public UploadTeachingService(PegasusDBEntities entities)
        {
            this.entities = entities;
        }

        public List<UploadTeachingModel> Read(int prokirixiId, string AFM)
        {
            var data = (from d in entities.UploadTeaching
                        where d.TeacherAFM == AFM && d.ProkirixiID == prokirixiId
                        orderby d.UploadDate descending
                        select new UploadTeachingModel
                        {
                            UploadID = d.UploadID,
                            AitisiID = d.AitisiID,
                            ProkirixiID = d.ProkirixiID,
                            TeacherAFM = d.TeacherAFM,
                            SchoolID = d.SchoolID,
                            UploadDate = d.UploadDate,
                            UploadSummary = d.UploadSummary
                        }).ToList();
            return data;
        }

        public void Create(UploadTeachingModel data, string AFM)
        {
            UploadTeaching entity = new UploadTeaching()
            {
                ProkirixiID = Common.GetOpenProkirixiID(),
                TeacherAFM = AFM,
                AitisiID = data.AitisiID,
                UploadDate = data.UploadDate,
                UploadSummary = data.UploadSummary,
                SchoolID = (from d in entities.AITISIS where d.AITISI_ID == data.AitisiID select d).FirstOrDefault().SCHOOL_ID

            };
            entities.UploadTeaching.Add(entity);
            entities.SaveChanges();

            data.UploadID = entity.UploadID;
        }

        public void Update(UploadTeachingModel data, string AFM)
        {
            UploadTeaching entity = entities.UploadTeaching.Find(data.UploadID);

            entity.ProkirixiID = Common.GetOpenProkirixiID();
            entity.TeacherAFM = AFM;
            entity.AitisiID = data.AitisiID;
            entity.UploadDate = data.UploadDate;
            entity.UploadSummary = data.UploadSummary;

            entities.Entry(entity).State = EntityState.Modified;
            entities.SaveChanges();
        }

        public void Destroy(UploadTeachingModel data)
        {
            UploadTeaching entity = entities.UploadTeaching.Find(data.UploadID);
            try
            {
                if (entity != null)
                {
                    entities.Entry(entity).State = EntityState.Deleted;
                    entities.UploadTeaching.Remove(entity);
                    entities.SaveChanges();
                }
            }
            catch { }
        }

        public UploadTeachingModel Refresh(int entityId)
        {
            return entities.UploadTeaching.Select(d => new UploadTeachingModel
            {
                UploadID = d.UploadID,
                AitisiID = d.AitisiID,
                ProkirixiID = d.ProkirixiID,
                TeacherAFM = d.TeacherAFM,
                SchoolID = d.SchoolID,
                UploadDate = d.UploadDate,
                UploadSummary = d.UploadSummary
            }).Where(d => d.UploadID.Equals(entityId)).FirstOrDefault();
        }

        public List<UploadTeachingFilesModel> ReadFiles(int uploadId)
        {
            var data = (from d in entities.UploadTeachingFiles
                        where d.UploadID == uploadId
                        orderby d.FileName
                        select new UploadTeachingFilesModel
                        {
                            FileID = d.FileID,
                            UploadID = d.UploadID,
                            FileName = d.FileName,
                            Category = d.Category,
                            TeacherAFM = d.TeacherAFM
                        }).ToList();
            return data;
        }

        public void DestroyFile(UploadTeachingFilesModel data)
        {
            UploadTeachingFiles entity = entities.UploadTeachingFiles.Find(data.FileID);

            if (entity != null)
            {
                entities.Entry(entity).State = EntityState.Deleted;
                entities.UploadTeachingFiles.Remove(entity);
                entities.SaveChanges();
            }
        }

        public void Dispose()
        {
            entities.Dispose();
        }
    }
}