using Pegasus.DAL;
using Pegasus.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Pegasus.Services
{
    public class UploadedFilesService : IUploadedFilesService, IDisposable
    {
        private readonly PegasusDBEntities entities;

        public UploadedFilesService(PegasusDBEntities entities)
        {
            this.entities = entities;
        }

        public List<xUploadedGeneralFilesModel> ReadGeneral(int aitisiID)
        {
            var data = (from d in entities.xUploadedGeneralFiles
                        where d.AitisiID == aitisiID
                        select new xUploadedGeneralFilesModel
                        {
                            FileID = d.FileID,
                            AitisiID = d.AitisiID,
                            Category = d.Category,
                            FileName = d.FileName,
                            ProkirixiID = d.ProkirixiID,
                            SchoolID = d.SchoolID,
                            TeacherAFM = d.TeacherAFM,
                            UploadSummary = d.UploadSummary
                        }).ToList();
            return data;
        }

        public List<xUploadedTeachingFilesModel> ReadTeaching(int aitisiID)
        {
            var data = (from d in entities.xUploadedTeachingFiles
                        where d.AitisiID == aitisiID
                        select new xUploadedTeachingFilesModel
                        {
                            FileID = d.FileID,
                            AitisiID = d.AitisiID,
                            Category = d.Category,
                            FileName = d.FileName,
                            ProkirixiID = d.ProkirixiID,
                            SchoolID = d.SchoolID,
                            TeacherAFM = d.TeacherAFM,
                            UploadSummary = d.UploadSummary
                        }).ToList();
            return data;
        }

        public List<xUploadedVocationFilesModel> ReadVocation(int aitisiID)
        {
            var data = (from d in entities.xUploadedVocationFiles
                        where d.AitisiID == aitisiID
                        select new xUploadedVocationFilesModel
                        {
                            FileID = d.FileID,
                            AitisiID = d.AitisiID,
                            Category = d.Category,
                            FileName = d.FileName,
                            ProkirixiID = d.ProkirixiID,
                            SchoolID = d.SchoolID,
                            TeacherAFM = d.TeacherAFM,
                            UploadSummary = d.UploadSummary
                        }).ToList();
            return data;
        }

        public void Dispose()
        {
            entities.Dispose();
        }
    }
}