using Pegasus.DAL;
using Pegasus.Models;
using System.Collections.Generic;

namespace Pegasus.Services
{
    public interface IAitisiTeacherService
    {
        AITISIS Create(AitisisViewModel model, int prokirixiId, string AFM);
        void Destroy(AitisisGridViewModel data);
        AitisisViewModel GetModel(int aitisiId);
        IEnumerable<AitisisGridViewModel> Read(int prokirixiId, string Afm);
        AITISIS Update(AitisisViewModel model, int prokirixiId, string AFM);
    }
}