using Microsoft.Graph;
using NSSOperationAutomationApp.Models;
using NSSOperationAutomationApp.ServiceMethods.MSGraphProvider;

namespace NSSOperationAutomationApp.ServiceMethods
{
    public class GroupsService : IGroupsService
    {
        private readonly IGraphServiceClientProvider? _graphServiceClientProvider;

        public GroupsService(IGraphServiceClientProvider graphServiceClientProvider)
        {
            this._graphServiceClientProvider = graphServiceClientProvider ?? throw new ArgumentNullException(nameof(graphServiceClientProvider));
        }

        public async Task<IEnumerable<User>> GetGroupMembers(string groupId)
        {
            GraphServiceClient graphClient = await this._graphServiceClientProvider.GetGraphClientApplication();

            var membersList = new List<User>();

            var members = await graphClient
                .Groups[groupId]
                .Members
                .Request()
                .Select("id,displayName,userPrincipalName,Mail")
                .Header(Models.Constants.PermissionTypeKey, GraphPermissionType.Application.ToString())
                .GetAsync();

            do
            {
                IEnumerable<DirectoryObject> currentPageEvents = members.CurrentPage;

                membersList.AddRange(currentPageEvents.Cast<User>().ToList());

                // If there are more result.
                if (members.NextPageRequest != null)
                {
                    members = await members.NextPageRequest.GetAsync();
                }
                else
                {
                    break;
                }
            }
            while (members.CurrentPage != null);

            return membersList;
        }
    }
}
