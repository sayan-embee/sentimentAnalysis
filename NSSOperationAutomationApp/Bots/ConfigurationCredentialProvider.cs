using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using NSSOperationAutomationApp.Models;

namespace NSSOperationAutomationApp.Bots
{
    //public class ConfigurationCredentialProvider : ConfigurationBotFrameworkAuthentication
    public class ConfigurationCredentialProvider : ICredentialProvider
    {
        private readonly Dictionary<string, string> credentials;
        private readonly IConfiguration _config;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationCredentialProvider"/> class.
        /// A constructor that accepts a map of bot id list and credentials.
        /// </summary>
        /// <param name="botOptions">bot options.</param>
        public ConfigurationCredentialProvider(IConfiguration config, 
            IOptions<AdminAppSettings> adminOptions, 
            IOptions<UserAppSettings> userOptions) 
            //: base(config)
        {
            this._config = config ?? throw new ArgumentNullException(nameof(config));
            adminOptions = adminOptions ?? throw new ArgumentNullException(nameof(adminOptions));
            userOptions = userOptions ?? throw new ArgumentNullException(nameof(userOptions));
            this.credentials = new Dictionary<string, string>();

            if (!string.IsNullOrEmpty(adminOptions.Value.AdminAppId))
            {
                this.credentials.Add(adminOptions.Value.AdminAppId, adminOptions.Value.AdminAppPassword);
            }

            if (!string.IsNullOrEmpty(userOptions.Value.UserAppId))
            {
                this.credentials.Add(userOptions.Value.UserAppId, userOptions.Value.UserAppPassword);
            }
        }

        /// <summary>
        /// Validates an app ID.
        /// </summary>
        /// <param name="appId">The app ID to validate.</param>
        /// <returns>A task that represents the work queued to execute.</returns>
        /// <remarks>If the task is successful, the result is true if <paramref name="appId"/>
        /// is valid for the controller; otherwise, false.</remarks>
        public Task<bool> IsValidAppIdAsync(string appId)
        {
            return Task.FromResult(this.credentials.ContainsKey(appId));
        }

        /// <summary>
        /// Gets the app password for a given bot app ID.
        /// </summary>
        /// <param name="appId">The ID of the app to get the password for.</param>
        /// <returns>A task that represents the work queued to execute.</returns>
        /// <remarks>If the task is successful and the app ID is valid, the result
        /// contains the password; otherwise, null.
        /// </remarks>
        public Task<string> GetAppPasswordAsync(string appId)
        {
            return Task.FromResult(this.credentials.ContainsKey(appId) ? this.credentials[appId] : null);
        }

        /// <summary>
        /// Checks whether bot authentication is disabled.
        /// </summary>
        /// <returns>A task that represents the work queued to execute.</returns>
        /// <remarks>If the task is successful and bot authentication is disabled, the result
        /// is true; otherwise, false.
        /// </remarks>
        public Task<bool> IsAuthenticationDisabledAsync()
        {
            return Task.FromResult(!this.credentials.Any());
        }
    }
}
