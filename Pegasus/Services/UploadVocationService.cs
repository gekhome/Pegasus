using Pegasus.BPM;
using Pegasus.DAL;
using Pegasus.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Pegasus.Services
{
    public class UploadVocationService : IUploadVocationService, IDisposable
    {
        private readonly PegasusDBEntities entities;

        public UploadVocationService(PegasusDBEntities entities)
        {
            this.entities = entities;
        }

        public List<UploadVocationModel> Read(int prokirixiId, string AFM)
        {
            var data = (from d in entities.UploadVocation
                        where d.TeacherAFM == AFM && d.ProkirixiID == prokirixiId
                        orderby d.UploadDate descending
                        select new UploadVocationModel
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

        public void Create(UploadVocationModel data, string AFM)
        {
            UploadVocation entity = new UploadVocation()
            {
                ProkirixiID = Common.GetOpenProkirixiID(),
                TeacherAFM = AFM,
                AitisiID = data.AitisiID,
                UploadDate = data.UploadDate,
                UploadSummary = data.UploadSummary,
                SchoolID = (from d in entities.AITISIS where d.AITISI_ID == data.AitisiID select d).FirstOrDefault().SCHOOL_ID

            };
            entities.UploadVocation.Add(entity);
            entities.SaveChanges();

            data.UploadID = entity.UploadID;
        }

        public void Update(UploadVocationModel data, string AFM)
        {
            UploadVocation entity = entities.UploadVocation.Find(data.UploadID);

            entity.ProkirixiID = Common.GetOpenProkirixiID();
            entity.TeacherAFM = AFM;
            entity.AitisiID = data.AitisiID;
            entity.UploadDate = data.UploadDate;
            entity.UploadSummary = data.UploadSummary;

            entities.Entry(entity).State = EntityState.Modified;
            entities.SaveChanges();
        }

        public void Destroy(UploadVocationModel data)
        {
            UploadVocation entity = entities.UploadVocation.Find(data.UploadID);
            try
            {
                if (entity != null)
                {
                    entities.Entry(entity).State = EntityState.Deleted;
                    entities.UploadVocation.Remove(entity);
                    entities.SaveChanges();
                }
            }
            catch { }
        }

        public UploadVocationModel Refresh(int entityId)
        {
            return entities.UploadVocation.Select(d => new UploadVocationModel
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

        public List<UploadVocationFilesModel> ReadFiles(int uploadId)
        {
            var data = (from d in entities.UploadVocationFiles
                        where d.UploadID == uploadId
                        orderby d.FileName
                        select new UploadVocationFilesModel
                        {
                            FileID = d.FileID,
                            UploadID = d.UploadID,
                            FileName = d.FileName,
                            Category = d.Category,
                            TeacherAFM = d.TeacherAFM
                        }).ToList();
            return data;
        }

        public void DestroyFile(UploadVocationFilesModel data)
        {
            UploadVocationFiles entity = entities.UploadVocationFiles.Find(data.FileID);

            if (entity != null)
            {
                entities.Entry(entity).State = EntityState.Deleted;
                entities.UploadVocationFiles.Remove(entity);
                entities.SaveChanges();
            }
        }

        public void Dispose()
        {
            entities.Dispose();
        }
    }
}