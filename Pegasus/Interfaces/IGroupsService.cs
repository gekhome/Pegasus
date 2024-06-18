using Pegasus.Models;
using System.Collections.Generic;

namespace Pegasus.Services
{
    public interface IGroupsService
    {
        void Create(GroupsViewModel data);
        void Destroy(GroupsViewModel data);
        List<sqlEidikotitesSelectorViewModel> GetEidikotites(int groupId);
        List<GroupsViewModel> Read();
        GroupsViewModel Refresh(int entityId);
        sqlEidikotitesSelectorViewModel RefreshEidikotita(int entityId);
        void ResetEidikotita(sqlEidikotitesSelectorViewModel data);
        void SetEidikotita(sqlEidikotitesSelectorViewModel data, int groupId);
        void Update(GroupsViewModel data);
    }
}