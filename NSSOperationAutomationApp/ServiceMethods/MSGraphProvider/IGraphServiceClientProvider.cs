using Microsoft.Graph;

namespace NSSOperationAutomationApp.ServiceMethods.MSGraphProvider
{
    public interface IGraphServiceClientProvider
    {
        Task<GraphServiceClient> GetGraphClientApplication();
        Task<GraphServiceClient> GetGraphClientApplication(string accessToken);
        Task<string> GetApplicationAccessToken();
    }
}