using Pegasus.Models;
using System.Collections.Generic;

namespace Pegasus.Services
{
    public interface IAitisiSchoolsService
    {
        void Create(AITISI_SCHOOLSViewModel data, int prokirixiId, int aitisiId);
        void Destroy(AITISI_SCHOOLSViewModel data);
        List<AITISI_SCHOOLSViewModel> Read(int aitisiId);
        AITISI_SCHOOLSViewModel Refresh(int entityId);
        void Update(AITISI_SCHOOLSViewModel data, int prokirixiId, int aitisiId);
    }
}