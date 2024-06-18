using Pegasus.Models;
using System.Collections.Generic;

namespace Pegasus.Services
{
    public interface IWorkTeachingService
    {
        void Create(ViewModelTeaching data, int aitisiId);
        void Destroy(ViewModelTeaching data);
        IEnumerable<ViewModelTeaching> Read(int aitisiId);
        ViewModelTeaching Refresh(int entityId);
        void Update(ViewModelTeaching data, int aitisiId);
    }
}