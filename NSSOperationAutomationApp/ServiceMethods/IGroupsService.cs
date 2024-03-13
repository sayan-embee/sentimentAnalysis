using Microsoft.Graph;

namespace NSSOperationAutomationApp.ServiceMethods
{
    public interface IGroupsService
    {
        Task<IEnumerable<User>> GetGroupMembers(string groupId);
    }
}