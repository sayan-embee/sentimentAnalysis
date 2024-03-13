using NSSOperationAutomationApp.Models;

namespace NSSOperationAutomationApp.DataAccessHelper
{
    public interface IConversationData
    {
        Task<IEnumerable<ConversationModel>> GetAllPersonalConversations(string? Filter, string? AppName);
        Task<ConversationModel> GetConversationById(string conversationId);
        Task<ConversationModel> GetConversationByUserId(Guid userId);
        Task<ConversationModel> GetConversationByUserId(Guid userId, string appName);
        Task<ConversationModel> GetConversationByUserEmail(string userEmail, string appName);
        Task<ReturnMessageModel> Insert(ConversationModel data);
        Task<ReturnMessageModel> Update(ConversationModel data);
        Task<ReturnMessageModel> Remove(ConversationModel data);

        Task<IEnumerable<ConversationTeamsModel>> GetAllTeamsConversations();
        Task<ConversationTeamsModel> GetConversationByTeamAadGroupId(string aadGroupId, string appName);
        Task<ReturnMessageModel> UpdateTeamConversation(ConversationTeamsModel data);
        Task<ReturnMessageModel> InsertTeamConversation(ConversationTeamsModel data);
        Task<ReturnMessageModel> RemoveTeamConversation(ConversationTeamsModel data);
    }
}