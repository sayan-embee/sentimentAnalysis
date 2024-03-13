using AdaptiveCards;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Rest;
using Namotion.Reflection;
using NSSOperationAutomationApp.DataAccessHelper;
using NSSOperationAutomationApp.Models;

namespace NSSOperationAutomationApp.ServiceMethods
{
    public class NotificationService : INotificationService
    {
        private readonly ILogger _logger;
        private readonly IConversationData _conversationData;
        private readonly string? AdminAppId;
        private readonly string? UserAppId;
        private readonly string? TenantId;
        private readonly Dictionary<string, string>? Credentials;
        private readonly IConfiguration _configuration;

        public NotificationService(
            ILogger<NotificationService> logger
            , IConversationData conversationData
            , IOptions<AppSettings> appOptions
            , IOptions<AdminAppSettings> adminOptions
            , IOptions<UserAppSettings> userOptions
            , IConfiguration configuration
            )
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _conversationData = conversationData ?? throw new ArgumentNullException(nameof(conversationData));
            appOptions = appOptions ?? throw new ArgumentNullException(nameof(appOptions));
            adminOptions = adminOptions ?? throw new ArgumentNullException(nameof(adminOptions));
            userOptions = userOptions ?? throw new ArgumentNullException(nameof(userOptions));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

            this.Credentials = new Dictionary<string, string>();

            if (!string.IsNullOrEmpty(appOptions.Value.TenantId))
            {
                this.TenantId = appOptions.Value.TenantId;
            }

            if (!string.IsNullOrEmpty(adminOptions.Value.AdminAppId))
            {
                this.AdminAppId = adminOptions.Value.AdminAppId;
                this.Credentials.Add(adminOptions.Value.AdminAppId, adminOptions.Value.AdminAppPassword);
            }

            if (!string.IsNullOrEmpty(userOptions.Value.UserAppId))
            {
                this.UserAppId = userOptions.Value.UserAppId;
                this.Credentials.Add(userOptions.Value.UserAppId, userOptions.Value.UserAppPassword);
            }
        }

        private async Task<string?> GetAppPassword(string appId)
        {
            return await Task.FromResult(this.Credentials.ContainsKey(appId) ? this.Credentials[appId] : null);
        }

        public async Task<CardResponseModel?> SendCard_PersonalScope(string userADID, Attachment cardAttachment, string referenceId, string appName)
        {
            try
            {
                if (!string.IsNullOrEmpty(userADID) && !string.IsNullOrEmpty(referenceId) && cardAttachment != null)
                {
                    Guid Id;
                    var atMention = false;
                    var baseURL = string.Empty;

                    try
                    {
                        atMention = _configuration.GetValue<bool>("CardSettings:AtMention");
                        baseURL = _configuration.GetValue<string>("App:AppBaseUrl");
                    }
                    catch (Exception ex)
                    {
                        atMention = false;
                    }

                    if (Guid.TryParse(userADID, out Id))
                    {
                        var conversationDetails = await _conversationData.GetConversationByUserId(Id);

                        if (conversationDetails != null
                            && !string.IsNullOrEmpty(conversationDetails.ServiceUrl)
                            && !string.IsNullOrEmpty(conversationDetails.ConversationId)
                            )
                        {
                            string AppId = this.UserAppId;

                            if (appName == "ADMIN")
                            {
                                AppId = this.AdminAppId;
                            }

                            Uri url = new Uri(conversationDetails.ServiceUrl);

                            ConnectorClient connectorClient = new ConnectorClient(url, AppId, await GetAppPassword(AppId));                            

                            var activity = new Activity()
                            {
                                Type = ActivityTypes.Message,
                                Conversation = new ConversationAccount()
                                {
                                    Id = conversationDetails.ConversationId
                                },
                                Attachments = new List<Attachment>()
                                {
                                    cardAttachment
                                }
                            };

                            if (atMention)
                            {
                                try
                                {
                                    var userToMention = await ((Conversations)connectorClient.Conversations).GetConversationMemberAsync(userADID, conversationDetails.ConversationId);

                                    if (userToMention != null && !string.IsNullOrEmpty(userToMention.Name))
                                    {
                                        var mention = new Mention
                                        {
                                            Mentioned = userToMention,
                                            Text = $"<at>{userToMention.Name}</at>",
                                        };

                                        AdaptiveCard adaptiveCard = cardAttachment.Content as AdaptiveCard;

                                        List<AdaptiveElement> adaptiveElementList = new List<AdaptiveElement>();

                                        adaptiveElementList.Add(new AdaptiveColumnSet()
                                        {
                                            Columns = new List<AdaptiveColumn>()
                                        {
                                                new AdaptiveColumn()
                                                {
                                                    Width="auto",
                                                    Spacing=AdaptiveSpacing.None,
                                                    Items = new List<AdaptiveElement>()
                                                    {
                                                            new AdaptiveTextBlock
                                                            {
                                                                Text =$"@{mention.Text}",
                                                                Wrap = true,
                                                                Size=AdaptiveTextSize.Medium,
                                                                Spacing=AdaptiveSpacing.Small,
                                                                Color=AdaptiveTextColor.Default,
                                                                Weight=AdaptiveTextWeight.Default,
                                                                FontType=AdaptiveFontType.Default
                                                            }
                                                    }
                                                }

                                        }
                                        });

                                        var container = new AdaptiveContainer()
                                        {
                                            Separator = true,
                                            Items = adaptiveElementList
                                        };
                                        adaptiveCard.Body.Add(container);

                                        var entities = new { entities = new List<Entity> { mention } };
                                        adaptiveCard.AdditionalProperties.Add("msteams", entities);

                                        var attachment = new Attachment
                                        {
                                            Content = adaptiveCard,
                                            ContentType = AdaptiveCard.ContentType,
                                        };

                                        activity.Attachments = new List<Attachment>()
                                        {
                                            cardAttachment
                                        };
                                    }
                                }
                                catch (Exception ex)
                                {
                                    this._logger.LogError(ex, $"NotificationService --> SendCard_PersonalScope() --> @mention execution failed for user-id ${userADID}");
                                    ExceptionLogging.SendErrorToText(ex);
                                }
                            }                            

                            var result = await connectorClient.Conversations.SendToConversationAsync(activity);
                            if (result != null)
                            {
                                var returnObj = new CardResponseModel();
                                returnObj.ReplyToId = result.Id;
                                //returnObj.ActivityId = conversationDetails.ActivityId;
                                returnObj.ActivityId = result.Id;
                                returnObj.ConversationId = conversationDetails.ConversationId;
                                returnObj.ServiceUrl = conversationDetails.ServiceUrl;
                                returnObj.UserName = conversationDetails.UserName;
                                returnObj.UserADID = conversationDetails.UserId.ToString();
                                returnObj.Status = true;
                                returnObj.ReferenceId = referenceId;

                                return returnObj;
                            }
                        }
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, $"NotificationService --> SendCard_PersonalScope() execution failed");
                ExceptionLogging.SendErrorToText(ex);
                return null;
            }
        }

        public async Task<CardResponseModel?> SendCard_PersonalScope(ConversationModel conversation, Attachment cardAttachment, string referenceId, string appName)
        {
            try
            {
                if (conversation != null && !string.IsNullOrEmpty(conversation.ConversationId) && !string.IsNullOrEmpty(conversation.ServiceUrl))
                {
                    Guid Id;
                    var atMention = false;
                    var baseURL = string.Empty;

                    try
                    {
                        atMention = _configuration.GetValue<bool>("CardSettings:AtMention");
                        baseURL = _configuration.GetValue<string>("App:AppBaseUrl");
                    }
                    catch (Exception ex)
                    {
                        atMention = false;
                    }

                    if (conversation.UserId != Guid.Empty)
                    {
                        string AppId = this.UserAppId;

                        if (appName == "ADMIN")
                        {
                            AppId = this.AdminAppId;
                        }

                        Uri url = new Uri(conversation.ServiceUrl);

                        ConnectorClient connectorClient = new ConnectorClient(url, AppId, await GetAppPassword(AppId));

                        var activity = new Activity()
                        {
                            Type = ActivityTypes.Message,
                            Conversation = new ConversationAccount()
                            {
                                Id = conversation.ConversationId
                            },
                            Attachments = new List<Attachment>()
                                {
                                    cardAttachment
                                }
                        };

                        if (atMention)
                        {
                            try
                            {
                                var userToMention = await ((Conversations)connectorClient.Conversations).GetConversationMemberAsync((conversation.UserId).ToString(), conversation.ConversationId);

                                if (userToMention != null && !string.IsNullOrEmpty(userToMention.Name))
                                {
                                    var mention = new Mention
                                    {
                                        Mentioned = userToMention,
                                        Text = $"<at>{userToMention.Name}</at>",
                                    };

                                    AdaptiveCard adaptiveCard = cardAttachment.Content as AdaptiveCard;

                                    List<AdaptiveElement> adaptiveElementList = new List<AdaptiveElement>();

                                    adaptiveElementList.Add(new AdaptiveColumnSet()
                                    {
                                        Columns = new List<AdaptiveColumn>()
                                        {
                                                new AdaptiveColumn()
                                                {
                                                    Width="auto",
                                                    Spacing=AdaptiveSpacing.None,
                                                    Items = new List<AdaptiveElement>()
                                                    {
                                                            new AdaptiveTextBlock
                                                            {
                                                                Text =$"@{mention.Text}",
                                                                Wrap = true,
                                                                Size=AdaptiveTextSize.Medium,
                                                                Spacing=AdaptiveSpacing.Small,
                                                                Color=AdaptiveTextColor.Default,
                                                                Weight=AdaptiveTextWeight.Default,
                                                                FontType=AdaptiveFontType.Default
                                                            }
                                                    }
                                                }

                                        }
                                    });

                                    var container = new AdaptiveContainer()
                                    {
                                        Separator = true,
                                        Items = adaptiveElementList
                                    };
                                    adaptiveCard.Body.Add(container);

                                    var entities = new { entities = new List<Entity> { mention } };
                                    adaptiveCard.AdditionalProperties.Add("msteams", entities);

                                    var attachment = new Attachment
                                    {
                                        Content = adaptiveCard,
                                        ContentType = AdaptiveCard.ContentType,
                                    };

                                    activity.Attachments = new List<Attachment>()
                                        {
                                            cardAttachment
                                        };
                                }
                            }
                            catch (Exception ex)
                            {
                                this._logger.LogError(ex, $"NotificationService --> SendCard_PersonalScope() --> @mention execution failed for user-id ${(conversation.UserId).ToString()}");
                                ExceptionLogging.SendErrorToText(ex);
                            }
                        }

                        var result = await connectorClient.Conversations.SendToConversationAsync(activity);
                        if (result != null)
                        {
                            var returnObj = new CardResponseModel();
                            returnObj.ReplyToId = result.Id;
                            //returnObj.ActivityId = conversationDetails.ActivityId;
                            returnObj.ActivityId = result.Id;
                            returnObj.ConversationId = conversation.ConversationId;
                            returnObj.ServiceUrl = conversation.ServiceUrl;
                            returnObj.UserName = conversation.UserName;
                            returnObj.UserADID = conversation.UserId.ToString();
                            returnObj.Status = true;
                            returnObj.ReferenceId = referenceId;

                            return returnObj;
                        }
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, $"NotificationService --> SendCard_PersonalScope() execution failed");
                ExceptionLogging.SendErrorToText(ex);
                return null;
            }
        }
    }
}
