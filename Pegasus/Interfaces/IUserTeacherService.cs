using Pegasus.Models;
using System.Collections.Generic;

namespace Pegasus.Services
{
    public interface IUserTeacherService
    {
        void Create(UserTeacherEditViewModel data);
        void Destroy(UserTeacherEditViewModel data);
        IEnumerable<UserTeacherEditViewModel> Read();
        List<TeacherAccountInfoViewModel> ReadInfo(string afm);
        UserTeacherEditViewModel Refresh(int entityId);
        void Update(UserTeacherEditViewModel data);
    }
}