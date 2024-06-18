using Pegasus.BPM;
using Pegasus.DAL;
using Pegasus.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Pegasus.Services
{
    public class UploadGeneralService : IUploadGeneralService, IDisposable
    {
        private readonly PegasusDBEntities entities;

        public UploadGeneralService(PegasusDBEntities entities)
        {
            this.entities = entities;
        }

        public List<UploadGeneralModel> Read(int prokirixiId, string AFM)
        {
            var data = (from d in entities.UploadGeneral
                        where d.TeacherAFM == AFM && d.ProkirixiID == prokirixiId
                        orderby d.UploadDate descending
                        select new UploadGeneralModel
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

        public void Create(UploadGeneralModel data, string AFM)
        {
            UploadGeneral entity = new UploadGeneral()
            {
                ProkirixiID = Common.GetOpenProkirixiID(),
                TeacherAFM = AFM,
                AitisiID = data.AitisiID,
                UploadDate = data.UploadDate,
                UploadSummary = data.UploadSummary,
                SchoolID = (from d in entities.AITISIS where d.AITISI_ID == data.AitisiID select d).FirstOrDefault().SCHOOL_ID

            };
            entities.UploadGeneral.Add(entity);
            entities.SaveChanges();

            data.UploadID = entity.UploadID;
        }

        public void Update(UploadGeneralModel data, string AFM)
        {
            UploadGeneral entity = entities.UploadGeneral.Find(data.UploadID);

            entity.ProkirixiID = Common.GetOpenProkirixiID();
            entity.TeacherAFM = AFM;
            entity.AitisiID = data.AitisiID;
            entity.UploadDate = data.UploadDate;
            entity.UploadSummary = data.UploadSummary;

            entities.Entry(entity).State = EntityState.Modified;
            entities.SaveChanges();
        }

        public void Destroy(UploadGeneralModel data)
        {
            UploadGeneral entity = entities.UploadGeneral.Find(data.UploadID);
            try
            {
                if (entity != null)
                {
                    entities.Entry(entity).State = EntityState.Deleted;
                    entities.UploadGeneral.Remove(entity);
                    entities.SaveChanges();
                }
            }
            catch { }
        }

        public UploadGeneralModel Refresh(int entityId)
        {
            return entities.UploadGeneral.Select(d => new UploadGeneralModel
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

        public List<UploadGeneralFilesModel> ReadFiles(int uploadId)
        {
            var data = (from d in entities.UploadGeneralFiles
                        where d.UploadID == uploadId
                        orderby d.FileName
                        select new UploadGeneralFilesModel
                        {
                            FileID = d.FileID,
                            UploadID = d.UploadID,
                            FileName = d.FileName,
                            Category = d.Category,
                            TeacherAFM = d.TeacherAFM
                        }).ToList();
            return data;
        }

        public void DestroyFile(UploadGeneralFilesModel data)
        {
            UploadGeneralFiles entity = entities.UploadGeneralFiles.Find(data.FileID);

            if (entity != null)
            {
                entities.Entry(entity).State = EntityState.Deleted;
                entities.UploadGeneralFiles.Remove(entity);
                entities.SaveChanges();
            }
        }

        public void Dispose()
        {
            entities.Dispose();
        }
    }
}