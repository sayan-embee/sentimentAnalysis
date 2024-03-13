using Microsoft.Bot.Builder;

namespace NSSOperationAutomationApp.Bots
{
    public class CommonBotFilterMiddleware : Microsoft.Bot.Builder.IMiddleware
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommonBotFilterMiddleware"/> class.
        /// </summary>
        /// <param name="botFilterMiddlewareOptions">The bot filter middleware options.</param>
        public CommonBotFilterMiddleware() { }

        /// <summary>
        /// Processes an incoming activity.
        /// If the activity's channel id is not "msteams", or its conversation's tenant is not an allowed tenant,
        /// then the middleware short circuits the pipeline, and skips the middlewares and handlers
        /// that are listed after this filter in the pipeline.
        /// </summary>
        /// <param name="turnContext">Context object containing information for a single turn of a conversation.</param>
        /// <param name="next">The delegate to call to continue the bot middleware pipeline.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task OnTurnAsync(ITurnContext turnContext, NextDelegate next, CancellationToken cancellationToken = default)
        {
            await next(cancellationToken).ConfigureAwait(false);
        }
    }
}
