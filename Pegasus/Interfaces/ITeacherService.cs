using Pegasus.DAL;
using Pegasus.Models;
using System.Collections.Generic;

namespace Pegasus.Services
{
    public interface ITeacherService
    {
        void Create(string AFM, TeacherViewModel model);
        AitisisViewModel GetAitisiModel(int aitisiID);
        TeacherViewModel GetModel(string AFM);
        ExperienceResultsViewModel GetMoriaNew(int AITISI_ID);
        ExperienceResultsViewModel GetMoriaOld(int AITISI_ID);
        ExperienceResultsViewModel GetMoriaOld2(int AITISI_ID);
        List<sqlTEACHER_AITISEIS> ReadAitiseis(string AFM);
        void Update(string AFM, TeacherViewModel model);
    }
}