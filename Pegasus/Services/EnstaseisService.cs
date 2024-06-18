using Pegasus.BPM;
using Pegasus.DAL;
using Pegasus.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Pegasus.Services
{
    public class EnstaseisService : IEnstaseisService, IDisposable
    {
        private readonly PegasusDBEntities entities;

        public EnstaseisService(PegasusDBEntities entities)
        {
            this.entities = entities;
        }

        public IEnumerable<UploadsViewModel> Read(int prokirixiId, string Afm)
        {
            var data = (from d in entities.UploadEnstaseis
                        where d.PROKIRIXI_ID == prokirixiId && d.TEACHER_AFM == Afm
                        orderby d.UPLOAD_DATE descending
                        select new UploadsViewModel
                        {
                            UPLOAD_ID = d.UPLOAD_ID,
                            AITISI_ID = d.AITISI_ID,
                            TEACHER_AFM = d.TEACHER_AFM,
                            SCHOOL_ID = d.SCHOOL_ID,
                            PROKIRIXI_ID = d.PROKIRIXI_ID,
                            UPLOAD_DATE = d.UPLOAD_DATE,
                            UPLOAD_NAME = d.UPLOAD_NAME,
                            UPLOAD_SUMMARY = d.UPLOAD_SUMMARY
                        }).ToList();
            return data;
        }

        public IEnumerable<UploadsViewModel> Read(int prokirixiId, int schoolId)
        {
            var data = (from d in entities.UploadEnstaseis
                        where d.PROKIRIXI_ID == prokirixiId && d.SCHOOL_ID == schoolId
                        orderby d.UPLOAD_NAME, d.UPLOAD_DATE descending
                        select new UploadsViewModel
                        {
                            UPLOAD_ID = d.UPLOAD_ID,
                            AITISI_ID = d.AITISI_ID,
                            TEACHER_AFM = d.TEACHER_AFM,
                            SCHOOL_ID = d.SCHOOL_ID,
                            PROKIRIXI_ID = d.PROKIRIXI_ID,
                            UPLOAD_DATE = d.UPLOAD_DATE,
                            UPLOAD_NAME = d.UPLOAD_NAME,
                            UPLOAD_SUMMARY = d.UPLOAD_SUMMARY
                        }).ToList();
            return data;
        }

        public void Create(UploadsViewModel data, int prokirixiId, string Afm)
        {
            UploadEnstaseis entity = new UploadEnstaseis()
            {
                TEACHER_AFM = Afm,
                PROKIRIXI_ID = prokirixiId,
                SCHOOL_ID = entities.AITISIS.Find(data.AITISI_ID).SCHOOL_ID,
                AITISI_ID = data.AITISI_ID,
                UPLOAD_DATE = data.UPLOAD_DATE,
                UPLOAD_NAME = Common.GetTeacherNameFromUser(Afm),
                UPLOAD_SUMMARY = data.UPLOAD_SUMMARY
            };
            entities.UploadEnstaseis.Add(entity);
            entities.SaveChanges();

            data.UPLOAD_ID = entity.UPLOAD_ID;
        }

        public void Update(UploadsViewModel data, int prokirixiId, string Afm)
        {
            UploadEnstaseis entity = entities.UploadEnstaseis.Find(data.UPLOAD_ID);

            entity.TEACHER_AFM = Afm;
            entity.PROKIRIXI_ID = prokirixiId;
            entity.AITISI_ID = data.AITISI_ID;
            entity.SCHOOL_ID = entities.AITISIS.Find(data.AITISI_ID).SCHOOL_ID;
            entity.UPLOAD_DATE = data.UPLOAD_DATE;
            entity.UPLOAD_NAME = Common.GetTeacherNameFromUser(Afm);
            entity.UPLOAD_SUMMARY = data.UPLOAD_SUMMARY;

            entities.Entry(entity).State = EntityState.Modified;
            entities.SaveChanges();
        }

        public void Destroy(UploadsViewModel data)
        {
            UploadEnstaseis entity = entities.UploadEnstaseis.Find(data.UPLOAD_ID);
            try
            {
                if (entity != null)
                {
                    entities.Entry(entity).State = EntityState.Deleted;
                    entities.UploadEnstaseis.Remove(entity);
                    entities.SaveChanges();
                }
            }
            catch { }
        }

        public string Delete(int uploadId)
        {
            string msg = "";

            if (Kerberos.CanDeleteUpload(uploadId))
            {
                UploadEnstaseis entity = entities.UploadEnstaseis.Find(uploadId);
                try
                {
                    if (entity != null)
                    {
                        entities.Entry(entity).State = EntityState.Deleted;
                        entities.UploadEnstaseis.Remove(entity);
                        entities.SaveChanges();
                    }
                }
                catch { }
            }
            else
            {
                msg = "Για να γίνει η διαγραφή πρέπει πρώτα να διαγραφούν τα σχετικά αρχεία μεταφόρτωσης.";
            }
            return msg;
        }

        public UploadsViewModel Refresh(int entityId)
        {
            return entities.UploadEnstaseis.Select(d => new UploadsViewModel
            {
                UPLOAD_ID = d.UPLOAD_ID,
                AITISI_ID = d.AITISI_ID,
                TEACHER_AFM = d.TEACHER_AFM,
                SCHOOL_ID = d.SCHOOL_ID,
                PROKIRIXI_ID = d.PROKIRIXI_ID,
                UPLOAD_DATE = d.UPLOAD_DATE,
                UPLOAD_NAME = d.UPLOAD_NAME,
                UPLOAD_SUMMARY = d.UPLOAD_SUMMARY
            }).Where(d => d.UPLOAD_ID.Equals(entityId)).FirstOrDefault();
        }

        public List<UploadsFilesViewModel> ReadFiles(int uploadId)
        {
            var data = (from d in entities.UploadEnstaseisFiles
                        where d.UPLOAD_ID == uploadId
                        orderby d.SCHOOL_USER, d.SCHOOLYEAR_TEXT, d.FILENAME
                        select new UploadsFilesViewModel
                        {
                            ID = d.ID,
                            UPLOAD_ID = d.UPLOAD_ID,
                            SCHOOL_USER = d.SCHOOL_USER,
                            SCHOOLYEAR_TEXT = d.SCHOOLYEAR_TEXT,
                            FILENAME = d.FILENAME,
                            EXTENSION = d.EXTENSION
                        }).ToList();
            return data;
        }

        public void DestroyFile(UploadsFilesViewModel data)
        {
            UploadEnstaseisFiles entity = entities.UploadEnstaseisFiles.Find(data.ID);

            if (entity != null)
            {
                entities.Entry(entity).State = EntityState.Deleted;
                entities.UploadEnstaseisFiles.Remove(entity);
                entities.SaveChanges();
            }
        }

        public void Dispose()
        {
            entities.Dispose();
        }
    }
}