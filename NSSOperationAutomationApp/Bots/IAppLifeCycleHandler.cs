using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema.Teams;
using Microsoft.Bot.Schema;

namespace NSSOperationAutomationApp.Bots
{
    public interface IAppLifeCycleHandler
    {
        Task OnBotInstalledInPersonalAsync(ITurnContext<IConversationUpdateActivity> turnContext, string appName);
        Task OnBotInstalledInTeamsAsync(ITurnContext<IConversationUpdateActivity> turnContext, string appName, TeamsChannelData teamsChannelData);
        Task OnBotRemovedInPersonalAsync(ITurnContext turnContext, string appName);
        Task OnBotRemovedInTeamsAsync(ITurnContext turnContext, string appName, TeamsChannelData teamsChannelData);


        Task<TaskModuleResponse> OnFetchAsync(ITurnContext<IInvokeActivity> turnContext, TaskModuleRequest taskModuleRequest);
        Task<TaskModuleResponse> OnSubmitAsync(ITurnContext<IInvokeActivity> turnContext, TaskModuleRequest taskModuleRequest);
    }
}