//using Microsoft.AspNetCore.Authentication.AzureAD.UI;

using Microsoft.Identity.Web;

namespace NSSOperationAutomationApp.Models
{
    public class AppSettings
    {
        public string AppBaseUrl { get; set; }
        public string TenantId { get; set; }
        public int CardCacheDurationInHour { get; set; }
        public int MaxUsers { get; set; }
    }

    public class AdminAppSettings
    {
        public string AdminAppId { get; set; }
        public string AdminAppPassword { get; set; }
        public string AdminManifestId { get; set; }
        public string AdminEmailIds { get; set; }
    }

    public class UserAppSettings
    {
        public string UserAppId { get; set; }
        public string UserAppPassword { get; set; }
        public string UserManifestId { get; set; }
    }

    public class GroupAppSettings
    {
        public string GroupIds { get; set; }
    }

    public class ConversationTypes
    {
        public const string Personal = "personal";
        public const string Channel = "channel";
    }

    public class AzureSettings : MicrosoftIdentityOptions
    {
        /// <summary>
        /// Gets or sets application id URI.
        /// </summary>
        public string ApplicationIdURI { get; set; }

        /// <summary>
        /// Gets or sets valid issuer URL.
        /// </summary>
        public string ValidIssuers { get; set; }

        /// <summary>
        /// Gets or sets Graph API scope.
        /// </summary>
        public string GraphScope { get; set; }
    }

    public class GraphConstants
    {
        /// <summary>
        /// Microsoft Graph version 1.0 base Url.
        /// </summary>
        public const string V1BaseUrl = "https://graph.microsoft.com/v1.0";

        /// <summary>
        /// Microsoft Graph Beta base url.
        /// </summary>
        public const string BetaBaseUrl = "https://graph.microsoft.com/beta";

        /// <summary>
        /// Max page size.
        /// </summary>
        public const int MaxPageSize = 999;

        /// <summary>
        /// Max retry for Graph API calls.
        /// Note: Max value allowed is 10.
        /// </summary>
        public const int MaxRetry = 5;
    }

    public class Constants
    {
        /// <summary>
        /// scope claim type.
        /// </summary>
        public const string ClaimTypeScp = "scp";

        /// <summary>
        /// authorization scheme.
        /// </summary>
        public const string BearerAuthorizationScheme = "Bearer";

        /// <summary>
        /// claim type user id.
        /// </summary>
        public const string ClaimTypeUserId = "http://schemas.microsoft.com/identity/claims/objectidentifier";

        /// <summary>
        /// claim type tenant id.
        /// </summary>
        public const string ClaimTypeTenantId = "http://schemas.microsoft.com/identity/claims/tenantid";

        /// <summary>
        /// blob container name.
        /// </summary>
        public const string BlobContainerName = "exportdatablobs";

        /// <summary>
        /// get the group type Hidden Membership.
        /// </summary>
        public const string HiddenMembership = "HiddenMembership";

        /// <summary>
        /// get the header key for graph permission type.
        /// </summary>
        public const string PermissionTypeKey = "x-api-permission";

        /// <summary>
        /// get the default graph scope.
        /// </summary>
        public const string ScopeDefault = "https://graph.microsoft.com/.default";

        /// <summary>
        /// get the OData next page link.
        /// </summary>
        public const string ODataNextPageLink = "@odata.nextLink";
    }

    public enum GraphPermissionType
    {
        /// <summary>
        /// This represents application permission of Microsoft Graph.
        /// </summary>
        Application,

        /// <summary>
        /// This represents delegate permission of Microsoft Graph.
        /// </summary>
        Delegate,
    }

    public class BotCommandConstants
    {
        public const string viewTask = "VIEW-TASK";
        public const string viewTaskAdmin = "VIEW-TASK-ADMIN";
    }
}
