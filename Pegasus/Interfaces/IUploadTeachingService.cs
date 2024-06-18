using Pegasus.Models;
using System.Collections.Generic;

namespace Pegasus.Services
{
    public interface IUploadTeachingService
    {
        void Create(UploadTeachingModel data, string AFM);
        void Destroy(UploadTeachingModel data);
        void DestroyFile(UploadTeachingFilesModel data);
        List<UploadTeachingModel> Read(int prokirixiId, string AFM);
        List<UploadTeachingFilesModel> ReadFiles(int uploadId);
        UploadTeachingModel Refresh(int entityId);
        void Update(UploadTeachingModel data, string AFM);
    }
}