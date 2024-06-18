using Pegasus.Models;
using System.Collections.Generic;

namespace Pegasus.Services
{
    public interface IEidikotitesService
    {
        void Create(EidikotitesViewModel data);
        void Destroy(EidikotitesViewModel data);
        IEnumerable<EidikotitesViewModel> Read();
        void Update(EidikotitesViewModel data);
        void UpdateGroup(EidikotitesViewModel data);
    }
}