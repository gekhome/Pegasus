using Pegasus.DAL;
using System.Collections.Generic;

namespace Pegasus.Services
{
    public interface ITeacherRegistryService
    {
        IEnumerable<sqlTEACHERS_WITH_AITISEIS_UNIQUE> Read();
    }
}