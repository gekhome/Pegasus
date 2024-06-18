using Pegasus.Models;
using System.Collections.Generic;

namespace Pegasus.Services
{
    public interface IKladosUnifiedService
    {
        void Create(KladosUnifiedViewModel data);
        void Destroy(KladosUnifiedViewModel data);
        List<sqlEidikotitesKUViewModel> GetEidikotites(int kladosunifiedId);
        IEnumerable<KladosUnifiedViewModel> Read();
        KladosUnifiedViewModel Refresh(int entityId);
        sqlEidikotitesKUViewModel RefreshEidikotita(int entityId);
        void ResetEidikotita(sqlEidikotitesKUViewModel data);
        void SetEidikotita(sqlEidikotitesKUViewModel data, int kladosunifiedId);
        void Update(KladosUnifiedViewModel data);
    }
}