using Pegasus.Models;
using System.Collections.Generic;

namespace Pegasus.Services
{
    public interface IUploadVocationService
    {
        void Create(UploadVocationModel data, string AFM);
        void Destroy(UploadVocationModel data);
        void DestroyFile(UploadVocationFilesModel data);
        List<UploadVocationModel> Read(int prokirixiId, string AFM);
        List<UploadVocationFilesModel> ReadFiles(int uploadId);
        UploadVocationModel Refresh(int entityId);
        void Update(UploadVocationModel data, string AFM);
    }
}