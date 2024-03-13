using NSSOperationAutomationApp.DataAccessHelper.DBAccess;
using NSSOperationAutomationApp.Models;

namespace NSSOperationAutomationApp.DataAccessHelper
{
    public class ConversationData : IConversationData
    {
        private readonly ILogger<ConversationData>? _logger;
        private readonly ISQLDataAccess? _db;

        public ConversationData(ISQLDataAccess db, ILogger<ConversationData> logger)
        {
            this._db = db ?? throw new ArgumentNullException(nameof(db));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IEnumerable<ConversationModel>> GetAllPersonalConversations(string? Filter, string? AppName)
        {
            try
            {
                var results = await _db.LoadData<ConversationModel, dynamic>("usp_M_Conversation_Get", new { Filter, AppName });
                return results;
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, $"Unable to execute GetAllPersonalConversations(usp_M_Conversation_Get)");
                throw ex;
            }
        }

        public async Task<ConversationModel> GetConversationById(string conversationId)
        {
            try
            {
                var results = await _db.LoadData<ConversationModel, dynamic>("usp_M_Conversation_Get", new { conversationId = conversationId });

                return results.FirstOrDefault();
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, $"Unable to GetConversationById - conversation id :{conversationId}.");
                return null;
            }
        }

        public async Task<ConversationModel> GetConversationByUserId(Guid userId)
        {
            try
            {
                var results = await _db.LoadData<ConversationModel, dynamic>("usp_M_Conversation_Get", new { userId = userId });

                return results.FirstOrDefault();
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, $"Unable to GetConversationByUserId - user id :{userId}.");
                return null;
            }
        }

        public async Task<ConversationModel> GetConversationByUserEmail(string userEmail, string appName)
        {
            try
            {
                var results = await _db.LoadData<ConversationModel, dynamic>("usp_M_Conversation_Get", new { UserEmail = userEmail, AppName = appName });

                return results.FirstOrDefault();
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, $"Unable to GetConversationByUserEmail - user email :{userEmail}.");
                return null;
            }
        }

        public async Task<ConversationModel> GetConversationByUserId(Guid userId, string appName)
        {
            try
            {
                var results = await _db.LoadData<ConversationModel, dynamic>("usp_M_Conversation_Get", new { userId = userId, AppName = appName });

                return results.FirstOrDefault();
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, $"Unable to GetConversationByUserId - user id :{userId} & Appname - :{appName}");
                return null;
            }
        }

        public async Task<ReturnMessageModel> Insert(ConversationModel data)
        {
            try
            {
                var results = await _db.SaveData<ReturnMessageModel, dynamic>(storedProcedure: "usp_M_Conversation_Insert",
                new
                {
                    ConversationId = data.ConversationId,
                    BotInstalledOn = data.BotInstalledOn,
                    ServiceUrl = data.ServiceUrl,
                    UserId = data.UserId,
                    ActivityId = data.ActivityId,
                    RecipientId = data.RecipientId,
                    RecipientName = data.RecipientName,
                    UserEmail = data.UserEmail,
                    TenantId = data.TenantId,
                    UserName = data.UserName,
                    UserPrincipalName = data.UserPrincipalName,
                    AppName = data.AppName
                });
                return results.FirstOrDefault();
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, $"Unable to insert conversation data. Conversation Id: {data.ConversationId} User Id:{data.UserId}");
                return null;
            }
        }

        public async Task<ReturnMessageModel> Update(ConversationModel data)
        {
            try
            {
                var results = await _db.SaveData<ReturnMessageModel, dynamic>(storedProcedure: "usp_M_Conversation_Update",
                new
                {
                    ConversationId = data.ConversationId,
                    BotInstalledOn = data.BotInstalledOn,
                    ServiceUrl = data.ServiceUrl,
                    ActivityId = data.ActivityId,
                    UserEmail = data.UserEmail,
                    UserName = data.UserName,
                    UserPrincipalName = data.UserPrincipalName,
                    AppName = data.AppName,
                    Active = data.Active,
                    UserId = data.UserId
                });
                return results.FirstOrDefault();
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, $"Unable to Update conversation data. Conversation Id: {data.ConversationId} User Id:{data.UserId}");
                return null;
            }
        }

        public async Task<ReturnMessageModel> Remove(ConversationModel data)
        {
            try
            {
                var results = await _db.SaveData<ReturnMessageModel, dynamic>(storedProcedure: "usp_M_Conversation_Remove",
                new
                {
                    ConversationId = data.ConversationId,
                    BotInstalledOn = data.BotInstalledOn,
                    UserId = data.UserId,
                    AppName = data.AppName
                });
                return results.FirstOrDefault();
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, $"Unable to Update conversation data. Conversation Id: {data.ConversationId} User Id:{data.UserId}");
                return null;
            }
        }

        #region Channel Scope

        public async Task<IEnumerable<ConversationTeamsModel>> GetAllTeamsConversations()
        {
            try
            {
                var results = await _db.LoadData<ConversationTeamsModel, dynamic>("usp_M_ConversationTeams_Get", new { });
                return results;
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, $"Unable to execute GetAllTeamsConversations()");
                throw ex;
            }
        }

        public async Task<ConversationTeamsModel> GetConversationByTeamAadGroupId(string aadGroupId, string appName)
        {
            try
            {
                var results = await _db.LoadData<ConversationTeamsModel, dynamic>("usp_M_ConversationTeams_Get", new { TeamAadGroupId = aadGroupId, AppName = appName });

                return results.FirstOrDefault();
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, $"Unable to GetConversationByTeamAadGroupId - aadGroupId :{aadGroupId}.");
                return null;
            }
        }

        public async Task<ReturnMessageModel> UpdateTeamConversation(ConversationTeamsModel data)
        {
            try
            {
                var results = await _db.SaveData<ReturnMessageModel, dynamic>(storedProcedure: "usp_M_ConversationTeams_Update",
                new
                {
                    ConversationId = data.ConversationId,
                    BotInstalledOn = data.BotInstalledOn,
                    ServiceUrl = data.ServiceUrl,
                    ActivityId = data.ActivityId,
                    TeamId = data.TeamId,
                    TeamAadGroupId = data.TeamAadGroupId,
                    TeamName = data.TeamName,
                    AppName = data.AppName,
                    Active = data.Active
                });
                return results.FirstOrDefault();
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, $"Unable to Update conversation data. Conversation Id: {data.ConversationId} TeamAadGroupId : {data.TeamAadGroupId}");
                return null;
            }
        }

        public async Task<ReturnMessageModel> InsertTeamConversation(ConversationTeamsModel data)
        {
            try
            {
                var results = await _db.SaveData<ReturnMessageModel, dynamic>(storedProcedure: "usp_M_ConversationTeams_Insert",
                new
                {
                    ConversationId = data.ConversationId,
                    BotInstalledOn = data.BotInstalledOn,
                    ServiceUrl = data.ServiceUrl,
                    ActivityId = data.ActivityId,
                    RecipientId = data.RecipientId,
                    RecipientName = data.RecipientName,
                    TeamId = data.TeamId,
                    TeamAadGroupId = data.TeamAadGroupId,
                    TeamName = data.TeamName,
                    TenantId = data.TenantId,
                    AppName = data.AppName
                });
                return results.FirstOrDefault();
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, $"Unable to insert conversation data. Conversation Id: {data.ConversationId} TeamAadGroupId :{data.TeamAadGroupId}");
                return null;
            }
        }

        public async Task<ReturnMessageModel> RemoveTeamConversation(ConversationTeamsModel data)
        {
            try
            {
                var results = await _db.SaveData<ReturnMessageModel, dynamic>(storedProcedure: "usp_M_ConversationTeams_Remove",
                new
                {
                    ConversationId = data.ConversationId,
                    BotInstalledOn = data.BotInstalledOn,
                    TeamAadGroupId = data.TeamAadGroupId,
                    AppName = data.AppName
                });
                return results.FirstOrDefault();
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, $"Unable to Update conversation data. Conversation Id: {data.ConversationId} TeamAadGroupId :{data.TeamAadGroupId}");
                return null;
            }
        }

        #endregion
    }
}
