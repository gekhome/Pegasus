using Pegasus.DAL;
using Pegasus.Models;
using System.Collections.Generic;

namespace Pegasus.Services
{
    public interface IAitisiRegistryService
    {
        AitisisViewModel GetModel(int aitisiId);
        ExperienceResultsViewModel GetResults(int aitisiId);
        IEnumerable<sqlTEACHER_AITISEIS> Read(int prokirixiId);
        IEnumerable<ViewModelFreelance> ReadFreelance(int aitisiID);
        IEnumerable<ViewModelTeaching> ReadTeaching(int aitisiId);
        IEnumerable<ViewModelVocational> ReadVocation(int aitisiId);
    }
}