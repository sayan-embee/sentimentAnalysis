using Microsoft.Bot.Schema;
using NSSOperationAutomationApp.Models;

namespace NSSOperationAutomationApp.ServiceMethods
{
    public interface INotificationService
    {
        Task<CardResponseModel?> SendCard_PersonalScope(string userADID, Attachment cardAttachment, string referenceId, string appName);
        Task<CardResponseModel?> SendCard_PersonalScope(ConversationModel conversation, Attachment cardAttachment, string referenceId, string appName);
    }
}