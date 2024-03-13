using Microsoft.AspNetCore.Mvc;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using NSSOperationAutomationApp.Bots;

namespace NSSOperationAutomationApp.Controllers
{
    [Route("api/messages")]
    [ApiController]
    public class BotController : ControllerBase
    {
        private readonly IBotFrameworkHttpAdapter _adapter;
        private readonly IBot _users;
        private readonly IBot _admin;

        public BotController(CommonBotAdapter adapter,
            AdminActivityHandler admin,
            UserActivityHandler users
            )
        {
            this._adapter = adapter ?? throw new ArgumentNullException(nameof(adapter));
            this._admin = admin ?? throw new ArgumentNullException(nameof(admin));
            this._users = users ?? throw new ArgumentNullException(nameof(users));
        }

        [HttpPost]
        [Route("admin")]
        public async Task PostAdminAppAsync()
        {
            await this._adapter.ProcessAsync(this.Request, this.Response, this._admin);
        }

        [HttpPost]
        [Route("users")]
        public async Task PostUserAppAsync()
        {
            await this._adapter.ProcessAsync(this.Request, this.Response, this._users);
        }
    }
}
