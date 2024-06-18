using Pegasus.Models;
using System.Collections.Generic;

namespace Pegasus.Services
{
    public interface IEidikotitesProkirixiService
    {
        void Create(ProkirixisEidikotitesViewModel data, int prokirixiId, int schoolId);
        void Destroy(ProkirixisEidikotitesViewModel data);
        List<ProkirixisEidikotitesViewModel> Read(int prokirixiId, int schoolId);
        void Update(ProkirixisEidikotitesViewModel data, int prokirixiId, int schoolId);
    }
}