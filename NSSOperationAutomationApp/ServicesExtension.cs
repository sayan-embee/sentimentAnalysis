using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using NSSOperationAutomationApp.Bots;
using NSSOperationAutomationApp.DataAccessHelper;
using NSSOperationAutomationApp.DataAccessHelper.DBAccess;
using NSSOperationAutomationApp.HelperMethods;
using NSSOperationAutomationApp.Models;
using Microsoft.Identity.Web;
using Microsoft.Extensions.Configuration;
using NSSOperationAutomationApp.ServiceMethods;

namespace NSSOperationAutomationApp
{
    public static class ServicesExtension
    {
        public static void RegisterConfigurationSettings(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<Settings>(options =>
            {
                options.ConnectionStrings = configuration.GetValue<string>("ConnectionStrings:Default");
            });

            services.Configure<AppSettings>(options =>
            {
                options.AppBaseUrl = configuration.GetValue<string>("App:AppBaseUrl");
                options.TenantId = configuration.GetValue<string>("App:TenantId");
                options.CardCacheDurationInHour = configuration.GetValue<int>("App:CardCacheDurationInHour");
                options.MaxUsers = configuration.GetValue<int>("UserSettings:MaxUser");
            });

            services.Configure<AdminAppSettings>(options =>
            {
                options.AdminAppId = configuration.GetValue<string>("AdminApp:ClientId");
                options.AdminAppPassword = configuration.GetValue<string>("AdminApp:ClientSecret");
                options.AdminManifestId = configuration.GetValue<string>("AdminApp:ManifestId");
                options.AdminEmailIds = configuration.GetValue<string>("AdminSettings:AdminEmailIds");
            });

            services.Configure<UserAppSettings>(options =>
            {
                options.UserAppId = configuration.GetValue<string>("UserApp:ClientId");
                options.UserAppPassword = configuration.GetValue<string>("UserApp:ClientSecret");
                options.UserManifestId = configuration.GetValue<string>("UserApp:ManifestId");
            });

            services.Configure<GroupAppSettings>(options =>
            {
                options.GroupIds = configuration.GetValue<string>("GroupSettings:GroupIds");
            });
        }

        public static void RegisterDataServices(this IServiceCollection services)
        {
            services.AddSingleton<ISQLDataAccess, SQLDataAccess>();

            services.AddScoped<IAdaptiveCardService, AdaptiveCardService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IAzureBlobService, AzureBlobService>();

            services.AddScoped<IConversationData, ConversationData>();
            services.AddScoped<IDataAccess, DataAccess>();
            services.AddScoped<IFileHelper, FileHelper>();                       
            services.AddScoped<INotificationHelper, NotificationHelper>();                       
            services.AddScoped<IOpenAIHelper, OpenAIHelper>();
            
        }

        public static void RegisterAuthenticationServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

            // This works specifically for single tenant application.
            var azureSettings = new AzureSettings();
            configuration.Bind("AzureAd", azureSettings);
            services.Configure<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.Authority = $"{azureSettings.Instance}/{azureSettings.TenantId}/v2.0";
                options.SaveToken = true;
                options.TokenValidationParameters.ValidAudiences = new List<string> { azureSettings.ClientId, azureSettings.ApplicationIdURI.ToUpperInvariant() };
                options.TokenValidationParameters.AudienceValidator = AudienceValidator;
                options.TokenValidationParameters.ValidIssuers = (azureSettings.ValidIssuers?
                    .Split(new char[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries)?
                    .Select(p => p.Trim())).Select(validIssuer => validIssuer.Replace("TENANT_ID", azureSettings.TenantId, StringComparison.OrdinalIgnoreCase));
            });
        }

        private static bool AudienceValidator(
            IEnumerable<string> tokenAudiences,
            SecurityToken securityToken,
            TokenValidationParameters validationParameters)
        {
            if (tokenAudiences.IsNullOrEmpty())
            {
                throw new ApplicationException("No audience defined in token!");
            }

            var validAudiences = validationParameters.ValidAudiences;
            if (validAudiences.IsNullOrEmpty())
            {
                throw new ApplicationException("No valid audiences defined in validationParameters!");
            }

            return tokenAudiences.Intersect(tokenAudiences).Any();
        }

    }
}
