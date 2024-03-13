namespace NSSOperationAutomationApp.Models
{
    public class ConversationModel
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public string ConversationId { get; set; }
        public string ActivityId { get; set; }
        public Guid TenantId { get; set; }
        public string ServiceUrl { get; set; }
        public DateTime BotInstalledOn { get; set; }
        public DateTime? BotRemovedOn { get; set; }
        public string RecipientId { get; set; }
        public string RecipientName { get; set; }
        public string UserPrincipalName { get; set; }
        public string AppName { get; set; }
        public bool Active { get; set; }
    }

    public class ConversationTeamsModel : ConversationModel
    {
        public string TeamId { get; set; }
        public string TeamName { get; set; }
        public string TeamAadGroupId { get; set; }
    }
}
