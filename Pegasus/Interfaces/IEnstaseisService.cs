using Pegasus.Models;
using System.Collections.Generic;

namespace Pegasus.Services
{
    public interface IEnstaseisService
    {
        void Create(UploadsViewModel data, int prokirixiId, string Afm);
        string Delete(int uploadId);
        void Destroy(UploadsViewModel data);
        void DestroyFile(UploadsFilesViewModel data);
        IEnumerable<UploadsViewModel> Read(int prokirixiId, int schoolId);
        IEnumerable<UploadsViewModel> Read(int prokirixiId, string Afm);
        List<UploadsFilesViewModel> ReadFiles(int uploadId);
        UploadsViewModel Refresh(int entityId);
        void Update(UploadsViewModel data, int prokirixiId, string Afm);
    }
}