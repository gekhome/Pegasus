using Pegasus.Models;
using System.Collections.Generic;

namespace Pegasus.Services
{
    public interface ISchoolYearService
    {
        void Create(SchoolYearsViewModel data);
        void Destroy(SchoolYearsViewModel data);
        IEnumerable<SchoolYearsViewModel> Read();
        void Update(SchoolYearsViewModel data);
    }
}