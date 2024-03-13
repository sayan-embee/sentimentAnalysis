using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Connector.Authentication;

namespace NSSOperationAutomationApp.Bots
{
    //public class CommonBotAdapter : CloudAdapter
    public class CommonBotAdapter : BotFrameworkHttpAdapter
    {
        public CommonBotAdapter(
            //ConfigurationBotFrameworkAuthentication credentialProvider,
            ICredentialProvider credentialProvider,
            CommonBotFilterMiddleware botFilterMiddleware)
            : base(credentialProvider)
        {
            this.Use(botFilterMiddleware);
        }
    }
}
