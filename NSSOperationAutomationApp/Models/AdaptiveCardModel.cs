using Microsoft.Bot.Schema;
using Newtonsoft.Json;

namespace NSSOperationAutomationApp.Models
{
    public class AdaptiveCardType
    {
        public static string Type1 = "ASSIGN-TICKET";
        public static string Type2 = "TICKET-ACTION-ENG";
        public static string Type3 = "TICKET-ACTION-ADMIN";
    }

    public class AdaptiveCardModel
    {
        [JsonProperty("msteams")]
        public CardAction MsteamsCardAction { get; set; }

        [JsonProperty("command")]
        public string Command { get; set; }

        [JsonProperty("postedValues")]
        public string PostedValues { get; set; }

        [JsonProperty("teamId")]
        public string TeamId { get; set; }

        [JsonProperty("refId")]
        public long? RefId { get; set; }

        [JsonProperty("activityId")]
        public string ActivityId { get; set; }

        [JsonProperty("notificationId")]
        public long? NotificationId { get; set; }

        [JsonProperty("taskId")]
        public long? TaskId { get; set; }
    }

    public class CardResponseModel
    {
        [JsonProperty("notificationId")]
        public long NotificationId { get; set; }

        [JsonProperty("activityId")]
        public string? ActivityId { get; set; }

        [JsonProperty("userADID")]
        public string? UserADID { get; set; }

        [JsonProperty("userName")]
        public string? UserName { get; set; }

        [JsonProperty("status")]
        public bool Status { get; set; } = false;

        [JsonProperty("conversationId")]
        public string? ConversationId { get; set; }

        [JsonProperty("replyToId")]
        public string? ReplyToId { get; set; }

        [JsonProperty("serviceUrl")]
        public string? ServiceUrl { get; set; }

        [JsonProperty("notificationDateTimeUTC")]
        public DateTime? NotificationDateTimeUTC { get; set; }

        [JsonProperty("referenceId")]
        public string? ReferenceId { get; set; }
    }

    public class WelcomeCardModel
    {
        public bool SendWelcomeCard { get; set; } = false;
        public string? AppName { get; set; } = String.Empty;
        public string? AdminAppName { get; set; } = String.Empty;
        public List<string>? AdminAppFeatures { get; set; }
        public string? UserAppName { get; set; } = String.Empty;
        public List<string> UserAppFeatures { get; set; }
        public string? DescHeading { get; set; } = String.Empty;
    }

    public class TicketAssignmentCardModel : CardResponseModel
    {
        public string? CaseNumber { get; set; } = String.Empty;
        public long? TicketId { get; set; } = 0;
        public string? CaseSubject { get; set; } = String.Empty;
        public string? ContactName { get; set; } = String.Empty;
        public string? ContactEmail { get; set; } = String.Empty;
        public string? SerialNumber { get; set; } = String.Empty;
        public string? ProductName { get; set; } = String.Empty;
        public string? ProductNumber { get; set; } = String.Empty;
        public string? CreatedOn { get; set; } = String.Empty;
        public string? AssignedBy { get; set; } = String.Empty;
        public string? AssignedByEmail { get; set; } = String.Empty;
        public string? AssignedByADID { get; set; } = String.Empty;
        public string? AssignedTo { get; set; } = String.Empty;
        public string? AssignedToEmail { get; set; } = String.Empty;
        public string? AssignedToADID { get; set; } = String.Empty;
        public string? ServiceAccount { get; set; } = String.Empty;
        public string? AssignedOn { get; set; } = String.Empty;
        public string? CallStatus { get; set; } = String.Empty;
        public long? AssignmentId { get; set; } = 0;
        public long? AssignmentHistoryId { get; set; } = 0;
        public bool? InsertStatus { get; set; } = false;
        public string? InsertMsg { get; set; } = String.Empty;
        public long? CallDetailId { get; set; } = 0;
        public string? Type { get; set; } = String.Empty;
    }

    public class TicketActionCardModel : TicketAssignmentCardModel
    {
        public string? CallAction { get; set; } = String.Empty;
        public string? UpdatedOnIST { get; set; } = String.Empty;
        public string UpdatedBy { get; set; } = String.Empty;
        public string UpdatedByEmail { get; set; } = String.Empty;
        public string? UpdatedByADID { get; set; } = String.Empty;
        public string? CloserRemarks { get; set; } = String.Empty;
        public string? AdminClosureRemarks { get; set; } = String.Empty;
    }
}
