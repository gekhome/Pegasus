using Pegasus.Models;
using System.Collections.Generic;

namespace Pegasus.Services
{
    public interface IWorkVocationService
    {
        void Create(ViewModelVocational data, int aitisiID);
        void Destroy(ViewModelVocational data);
        IEnumerable<ViewModelVocational> Read(int aitisiID);
        ViewModelVocational Refresh(int entityId);
        void Update(ViewModelVocational data, int aitisiID);
    }
}