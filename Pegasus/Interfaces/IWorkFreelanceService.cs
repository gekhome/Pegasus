using Pegasus.Models;
using System.Collections.Generic;

namespace Pegasus.Services
{
    public interface IWorkFreelanceService
    {
        void Create(ViewModelFreelance data, int aitisiID);
        void Destroy(ViewModelFreelance data);
        IEnumerable<ViewModelFreelance> Read(int aitisiID);
        ViewModelFreelance Refresh(int entityId);
        void Update(ViewModelFreelance data, int aitisiID);
    }
}