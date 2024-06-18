using Pegasus.Models;
using System.Collections.Generic;

namespace Pegasus.Services
{
    public interface IUploadedFilesService
    {
        List<xUploadedGeneralFilesModel> ReadGeneral(int aitisiID);
        List<xUploadedTeachingFilesModel> ReadTeaching(int aitisiID);
        List<xUploadedVocationFilesModel> ReadVocation(int aitisiID);
    }
}