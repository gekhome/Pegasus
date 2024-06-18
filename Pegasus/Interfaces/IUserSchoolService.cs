using Pegasus.Models;
using System.Collections.Generic;

namespace Pegasus.Services
{
    public interface IUserSchoolService
    {
        void Create(UserSchoolViewModel data);
        void Destroy(UserSchoolViewModel data);
        IEnumerable<UserSchoolViewModel> Read();
        void Update(UserSchoolViewModel data);
    }
}