using Pegasus.Models;
using System.Collections.Generic;

namespace Pegasus.Services
{
    public interface IUploadGeneralService
    {
        void Create(UploadGeneralModel data, string AFM);
        void Destroy(UploadGeneralModel data);
        void DestroyFile(UploadGeneralFilesModel data);
        List<UploadGeneralModel> Read(int prokirixiId, string AFM);
        List<UploadGeneralFilesModel> ReadFiles(int uploadId);
        UploadGeneralModel Refresh(int entityId);
        void Update(UploadGeneralModel data, string AFM);
    }
}