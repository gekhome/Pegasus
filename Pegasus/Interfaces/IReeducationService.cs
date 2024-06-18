using Pegasus.Models;
using System.Collections.Generic;

namespace Pegasus.Services
{
    public interface IReeducationService
    {
        void Create(ReeducationViewModel data, int aitisiID);
        void Destroy(ReeducationViewModel data);
        List<ReeducationViewModel> Read(int aitisiID);
        ReeducationViewModel Refresh(int entityId);
        void Update(ReeducationViewModel data, int aitisiID);
    }
}