using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Options;
using Microsoft.Graph;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using NSSOperationAutomationApp.Models;
using Microsoft.AspNetCore.Identity;
using System.Net.Http.Headers;
using System.Net.Sockets;

namespace NSSOperationAutomationApp.ServiceMethods.MSGraphProvider
{
    public class GraphServiceClientProvider : IGraphServiceClientProvider
    {
        private readonly string? AdminAppId;
        private readonly string? UserAppId;
        private readonly string? TenantId;
        private readonly Dictionary<string, string>? credentials;

        public GraphServiceClientProvider(IOptions<AppSettings> appOptions
            , IOptions<AdminAppSettings> adminOptions
            , IOptions<UserAppSettings> userOptions)
        {
            appOptions = appOptions ?? throw new ArgumentNullException(nameof(appOptions));
            adminOptions = adminOptions ?? throw new ArgumentNullException(nameof(adminOptions));
            userOptions = userOptions ?? throw new ArgumentNullException(nameof(userOptions));

            this.credentials = new Dictionary<string, string>();

            if (!string.IsNullOrEmpty(appOptions.Value.TenantId))
            {
                this.TenantId = appOptions.Value.TenantId;
            }

            if (!string.IsNullOrEmpty(adminOptions.Value.AdminAppId))
            {
                this.AdminAppId = adminOptions.Value.AdminAppId;
                this.credentials.Add(adminOptions.Value.AdminAppId, adminOptions.Value.AdminAppPassword);
            }

            if (!string.IsNullOrEmpty(userOptions.Value.UserAppId))
            {
                this.UserAppId = userOptions.Value.UserAppId;
                this.credentials.Add(userOptions.Value.UserAppId, userOptions.Value.UserAppPassword);
            }
        }

        private async Task<string?> GetAppPassword(string appId)
        {
            return await Task.FromResult(this.credentials.ContainsKey(appId) ? this.credentials[appId] : null);
        }

        public async Task<GraphServiceClient> GetGraphClientApplication()
        {
            var credentials = new Microsoft.IdentityModel.Clients.ActiveDirectory.ClientCredential(this.AdminAppId, await GetAppPassword(AdminAppId));
            var authContext = new AuthenticationContext($"https://login.microsoftonline.com/{this.TenantId}/");
            var token = await authContext.AcquireTokenAsync("https://graph.microsoft.com/", credentials);
            var accessToken = token.AccessToken;

            var graphServiceClient = new GraphServiceClient(
                new DelegateAuthenticationProvider((requestMessage) =>
                {
                    requestMessage
                    .Headers
                    .Authorization = new AuthenticationHeaderValue("bearer", accessToken);

                    return Task.CompletedTask;
                }));

            return graphServiceClient;
        }

        public async Task<GraphServiceClient> GetGraphClientApplication(string accessToken)
        {
            var graphServiceClient = new GraphServiceClient(
                new DelegateAuthenticationProvider((requestMessage) =>
                {
                    requestMessage
                .Headers
                .Authorization = new AuthenticationHeaderValue("bearer", accessToken);

                    return Task.CompletedTask;
                }));

            return await Task.FromResult(graphServiceClient);
        }

        public async Task<string> GetApplicationAccessToken()
        {
            var credentials = new Microsoft.IdentityModel.Clients.ActiveDirectory.ClientCredential(this.AdminAppId, await GetAppPassword(AdminAppId));
            var authContext = new AuthenticationContext($"https://login.microsoftonline.com/{this.TenantId}/");
            var token = await authContext.AcquireTokenAsync("https://graph.microsoft.com/", credentials);
            return token.AccessToken;
        }
    }
}
