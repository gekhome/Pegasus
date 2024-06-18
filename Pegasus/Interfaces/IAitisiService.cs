using Pegasus.DAL;
using Pegasus.Models;
using System.Collections.Generic;

namespace Pegasus.Services
{
    public interface IAitisiService
    {
        void Create(AitisisGridViewModel data, int prokirixiId);
        void Destroy(AitisisGridViewModel data);
        AITISIS EditAitisi(AitisisViewModel data, int aitisiId, string auditor = null);
        AitisisViewModel GetModel(int aitisiID);
        ExperienceResultsViewModel GetResults(int aitisiId);
        IEnumerable<AitisisGridViewModel> Read(int prokirixiId);
        void Update(AitisisGridViewModel data, int prokirixiId);
    }
}