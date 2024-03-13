using Microsoft.ApplicationInsights;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Teams;
using Microsoft.Bot.Schema.Teams;
using Microsoft.Bot.Schema;
using NSSOperationAutomationApp.Models;
using Microsoft.ApplicationInsights.DataContracts;

namespace NSSOperationAutomationApp.Bots
{
    public class UserActivityHandler : TeamsActivityHandler
    {
        private readonly TelemetryClient? _telemetryClient;
        private readonly ILogger<UserActivityHandler>? _logger;
        private readonly IAppLifeCycleHandler? _appLifeCycleHandler;
        private const string _appName = "UserApp";

        public UserActivityHandler(
            ILogger<UserActivityHandler> logger,
            TelemetryClient telemetryClient,
            IAppLifeCycleHandler appLifeCycleHandler)
        {
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this._telemetryClient = telemetryClient ?? throw new ArgumentNullException(nameof(telemetryClient));
            this._appLifeCycleHandler = appLifeCycleHandler ?? throw new ArgumentNullException(nameof(appLifeCycleHandler));
        }

        public override async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default)
        {
            try
            {
                turnContext = turnContext ?? throw new ArgumentNullException(nameof(turnContext));
                this.RecordEvent(nameof(this.OnTurnAsync), turnContext);
                switch (turnContext.Activity.Conversation.ConversationType)
                {
                    case ConversationTypes.Personal:
                        if (turnContext.Activity.Action == "remove"
                            && turnContext.Activity.Type == "installationUpdate"
                            && turnContext.Activity.From.AadObjectId != null)
                        {
                            await this._appLifeCycleHandler.OnBotRemovedInPersonalAsync(turnContext, _appName);
                            return;
                        }
                        break;
                    default:
                        break;
                }
                await base.OnTurnAsync(turnContext, cancellationToken);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, $"Error at {nameof(this.OnTurnAsync)}.");
                await base.OnTurnAsync(turnContext, cancellationToken);
                throw;
            }
        }
        /// <summary>
        /// Invoked when members other than this bot (like a user) are removed from the conversation.
        /// </summary>
        /// <param name="turnContext">Context object containing information cached for a single turn of conversation with a user.</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
        /// <returns>A task that represents the work queued to execute.</returns>
        protected override async Task OnConversationUpdateActivityAsync(ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            try
            {
                turnContext = turnContext ?? throw new ArgumentNullException(nameof(turnContext));
                this.RecordEvent(nameof(this.OnConversationUpdateActivityAsync), turnContext);

                var activity = turnContext.Activity;
                this._logger.LogInformation($"conversationType: {activity.Conversation.ConversationType}, membersAdded: {activity.MembersAdded?.Count}, membersRemoved: {activity.MembersRemoved?.Count}");

                switch (activity.Conversation.ConversationType)
                {
                    case ConversationTypes.Personal:
                        //User Install the app
                        if (activity.MembersAdded != null && activity.MembersAdded.Any(member => member.Id == activity.Recipient.Id))
                        {
                            await this._appLifeCycleHandler.OnBotInstalledInPersonalAsync(turnContext, _appName);
                        }
                        //User unistalled the app
                        else if (activity.MembersRemoved != null && activity.MembersRemoved.Any(member => member.Id == activity.Recipient.Id))
                        {
                            await this._appLifeCycleHandler.OnBotRemovedInPersonalAsync(turnContext, _appName);
                        }
                        break;
                    case ConversationTypes.Channel:
                        var teamsChannelData = turnContext.Activity.GetChannelData<TeamsChannelData>();
                        //App installed in team
                        if (activity.MembersAdded != null && activity.MembersAdded.Any(member => member.Id == activity.Recipient.Id))
                        {
                            await this._appLifeCycleHandler.OnBotInstalledInTeamsAsync(turnContext, _appName, teamsChannelData);
                        }
                        //App unistalled in team
                        else if (activity.MembersRemoved != null && activity.MembersRemoved.Any(member => member.Id == activity.Recipient.Id))
                        {
                            await this._appLifeCycleHandler.OnBotRemovedInTeamsAsync(turnContext, _appName, teamsChannelData);
                        }
                        break;
                    default: break;
                }

            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Exception occurred while bot conversation update event.");
                throw;
            }
        }

        protected override Task<TaskModuleResponse> OnTeamsTaskModuleFetchAsync(
          ITurnContext<IInvokeActivity> turnContext,
          TaskModuleRequest taskModuleRequest,
          CancellationToken cancellationToken)
        {
            try
            {
                return this._appLifeCycleHandler.OnFetchAsync(turnContext, taskModuleRequest);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, $"Error fetching task module : {ex.Message}", SeverityLevel.Error);
                return default;
            }
        }


        /// <summary>
        /// Records event data to Application Insights telemetry client.
        /// </summary>
        /// <param name="eventName">Name of the event.</param>
        /// <param name="turnContext">Provides context for a turn in a bot.</param>
        private void RecordEvent(string eventName, ITurnContext turnContext)
        {
            var teamsChannelData = turnContext.Activity.GetChannelData<TeamsChannelData>();

            this._telemetryClient.TrackEvent(eventName, new Dictionary<string, string>
            {
                { "userId", turnContext.Activity.From.AadObjectId },
                { "tenantId", turnContext.Activity.Conversation.TenantId },
                { "teamId", teamsChannelData?.Team?.Id },
                { "channelId", teamsChannelData?.Channel?.Id },
            });
        }
    }
}
