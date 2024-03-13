using Microsoft.Bot.Builder.Teams;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema.Teams;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using NSSOperationAutomationApp.DataAccessHelper;
using NSSOperationAutomationApp.Models;
using NSSOperationAutomationApp.ServiceMethods;
using AdaptiveCards;
using static System.Collections.Specialized.BitVector32;
using System.Diagnostics;

namespace NSSOperationAutomationApp.Bots
{
    public class AppLifeCycleHandler : IAppLifeCycleHandler
    {
        private readonly ILogger<AppLifeCycleHandler>? _logger;
        private readonly IConversationData? _conversationData;
        private readonly IOptions<AppSettings>? _appSettings;
        private readonly IConfiguration _configuration;
        private readonly IAdaptiveCardService _adaptiveCardService;

        private const int TaskModuleHeight = 750;
        private const int TaskModuleWidth = 1050;

        private const int AdminTaskModuleHeight = 750;
        private const int AdminTaskModuleWidth = 950;

        private readonly string? AppBaseUrl;

        private readonly bool SendWelcomeCard = false;
        private readonly string? AdminAppName = string.Empty;
        private readonly string? AdminAppFeatures = string.Empty;
        private readonly string? UserAppName = string.Empty;
        private readonly string? UserAppFeatures = string.Empty;
        private readonly string? DescHeading = string.Empty;

        public AppLifeCycleHandler(
            ILogger<AppLifeCycleHandler> logger,
            IConversationData conversationData,
            IOptions<AppSettings> appSettings,
            IConfiguration configuration,
            IAdaptiveCardService adaptiveCardService)
        {
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger)); ;
            this._conversationData = conversationData ?? throw new ArgumentNullException(nameof(conversationData));
            this._appSettings = appSettings ?? throw new ArgumentNullException(nameof(appSettings));
            this._configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            this._adaptiveCardService = adaptiveCardService ?? throw new ArgumentNullException(nameof(adaptiveCardService));

            if (!string.IsNullOrEmpty(_appSettings.Value.AppBaseUrl))
            {
                AppBaseUrl = _appSettings.Value.AppBaseUrl;
            }
            
            this.SendWelcomeCard = this._configuration.GetValue<bool>("CardSettings:SendWelcomeCard");
            this.AdminAppName = this._configuration.GetValue<string>("CardSettings:AdminAppName");
            this.AdminAppFeatures = this._configuration.GetValue<string>("CardSettings:AdminAppFeatures");
            this.UserAppName = this._configuration.GetValue<string>("CardSettings:UserAppName");
            this.UserAppFeatures = this._configuration.GetValue<string>("CardSettings:UserAppFeatures");
            this.DescHeading = this._configuration.GetValue<string>("CardSettings:DescHeading");
        }

        #region Private Methods

        private async Task SendWelcomeCardAsync(ITurnContext<IConversationUpdateActivity> turnContext, IConversationUpdateActivity activity, string appName)
        {
            if (this.SendWelcomeCard)
            {
                try
                {
                    if (!string.IsNullOrEmpty(AdminAppName) && !string.IsNullOrEmpty(UserAppName) && !string.IsNullOrEmpty(DescHeading))
                    {
                        var cardModel = new WelcomeCardModel
                        {
                            SendWelcomeCard = SendWelcomeCard,
                            AdminAppName = this.AdminAppName,
                            UserAppName = this.UserAppName,
                            DescHeading = this.DescHeading
                        };

                        if (appName == "AdminApp")
                        {
                            cardModel.AppName = this.AdminAppName;
                        }
                        else
                        {
                            cardModel.AppName = this.UserAppName;
                        }

                        var cardAttachment = this._adaptiveCardService.GetCard_Welcome_PersonalScope(cardModel);

                        if (cardAttachment != null)
                        {

                            if (!string.IsNullOrEmpty(AdminAppFeatures) && !string.IsNullOrEmpty(UserAppFeatures))
                            {
                                var AdminAppFeatureList = AdminAppFeatures.Split(",").ToList();
                                var UserAppFeatureList = UserAppFeatures.Split(",").ToList();

                                if (AdminAppFeatureList.Any() && UserAppFeatureList.Any())
                                {
                                    cardModel.AdminAppFeatures = AdminAppFeatureList.ToList();
                                    cardModel.UserAppFeatures = UserAppFeatureList.ToList();

                                    AdaptiveCard adaptiveCard = cardAttachment.Content as AdaptiveCard;
                                    List<AdaptiveElement> adaptiveElementList = new List<AdaptiveElement>();

                                    if (appName == "AdminApp")
                                    {
                                        //foreach (var text in cardModel.AdminAppFeatures)
                                        //{
                                        //    AdaptiveColumnSet columnSet = new AdaptiveColumnSet
                                        //    {
                                        //        Columns = new List<AdaptiveColumn>
                                        //        {
                                        //            new AdaptiveColumn
                                        //            {
                                        //                Width = "auto",
                                        //                Spacing = AdaptiveSpacing.None,
                                        //                Items = new List<AdaptiveElement>
                                        //                {
                                        //                    new AdaptiveTextBlock
                                        //                    {
                                        //                        Text = text,
                                        //                        Wrap = true,
                                        //                        Size = AdaptiveTextSize.Medium,
                                        //                        Spacing = AdaptiveSpacing.Small,
                                        //                        Color = AdaptiveTextColor.Default,
                                        //                        Weight = AdaptiveTextWeight.Default,
                                        //                        FontType = AdaptiveFontType.Default
                                        //                    }
                                        //                }
                                        //            }
                                        //        }
                                        //    };

                                        //    adaptiveElementList.Add(columnSet);
                                        //}

                                        foreach (var text in cardModel.AdminAppFeatures)
                                        {
                                            AdaptiveFactSet factSet = new AdaptiveFactSet
                                            {
                                                Facts = new List<AdaptiveFact>
                                                        {
                                                            new AdaptiveFact
                                                            {
                                                                Title = "•", // Bullet point
                                                                Value = text
                                                            }
                                                        }
                                            };

                                            adaptiveElementList.Add(factSet);
                                        }
                                    }

                                    if (appName == "UserApp")
                                    {
                                        //foreach (var text in cardModel.UserAppFeatures)
                                        //{
                                        //    AdaptiveColumnSet columnSet = new AdaptiveColumnSet
                                        //    {
                                        //        Columns = new List<AdaptiveColumn>
                                        //        {
                                        //            new AdaptiveColumn
                                        //            {
                                        //                Width = "auto",
                                        //                Spacing = AdaptiveSpacing.None,
                                        //                Items = new List<AdaptiveElement>
                                        //                {
                                        //                    new AdaptiveTextBlock
                                        //                    {
                                        //                        Text = text,
                                        //                        Wrap = true,
                                        //                        Size = AdaptiveTextSize.Medium,
                                        //                        Spacing = AdaptiveSpacing.Small,
                                        //                        Color = AdaptiveTextColor.Default,
                                        //                        Weight = AdaptiveTextWeight.Default,
                                        //                        FontType = AdaptiveFontType.Default
                                        //                    }
                                        //                }
                                        //            }
                                        //        }
                                        //    };

                                        //    adaptiveElementList.Add(columnSet);
                                        //}

                                        foreach (var text in cardModel.UserAppFeatures)
                                        {
                                            AdaptiveFactSet factSet = new AdaptiveFactSet
                                            {
                                                Facts = new List<AdaptiveFact>
                                                        {
                                                            new AdaptiveFact
                                                            {
                                                                Title = "•", // Bullet point
                                                                Value = text
                                                            }
                                                        }
                                            };

                                            adaptiveElementList.Add(factSet);
                                        }
                                    }

                                    var container = new AdaptiveContainer()
                                    {
                                        Separator = false,
                                        Items = adaptiveElementList
                                    };

                                    adaptiveCard.Body.Add(container);

                                    await turnContext.SendActivityAsync(MessageFactory.Attachment(cardAttachment));
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    this._logger.LogError(ex, $"Unable to send welcome card when user installed bot -  {activity.From.AadObjectId}.");
                }
            }
        }

        #endregion

        #region Personal Scope

        public async Task OnBotRemovedInPersonalAsync(ITurnContext turnContext, string appName)
        {
            turnContext = turnContext ?? throw new ArgumentNullException(nameof(turnContext), "Turncontext cannot be null");

            this._logger.LogInformation($"Removed added in personal scope for user {turnContext.Activity.From.AadObjectId} for app {appName}");

            await UpdateConversationOnBotUninstall(turnContext, appName);

            this._logger.LogInformation($"Successfully installed app for user {turnContext.Activity.From.AadObjectId}.");
        }

        public async Task OnBotInstalledInPersonalAsync(ITurnContext<IConversationUpdateActivity> turnContext, string appName)
        {
            turnContext = turnContext ?? throw new ArgumentNullException(nameof(turnContext), "Turncontext cannot be null");

            this._logger.LogInformation($"Bot added in personal scope for user {turnContext.Activity.From.AadObjectId}");

            var activity = turnContext.Activity;

            await InsertUpdateConversation(turnContext, activity, appName);

            this._logger.LogInformation($"Successfully installed app for user {activity.From.AadObjectId}.");
        }

        private async Task InsertUpdateConversation(ITurnContext<IConversationUpdateActivity> turnContext, IConversationUpdateActivity activity, string appName)
        {
            // Add or update user details when bot is installed.
            var existingRecord = await this._conversationData.GetConversationByUserId(Guid.Parse(turnContext.Activity.From.AadObjectId), appName);

            if (existingRecord != null)
            {
                var userConversation = existingRecord;
                userConversation.ConversationId = activity.Conversation.Id;
                userConversation.ServiceUrl = activity.ServiceUrl;
                userConversation.BotInstalledOn = DateTime.UtcNow;

                userConversation.UserId = Guid.Parse(activity.From.AadObjectId);
                userConversation.UserName = activity.From.Name;
                userConversation.ActivityId = activity.Id;
                userConversation.TenantId = Guid.Parse(activity.Conversation.TenantId);
                userConversation.RecipientId = activity.Recipient.Id;
                userConversation.RecipientName = activity.Recipient.Name;
                userConversation.Active = true;
                userConversation.AppName = appName;
                try
                {
                    var member1 = await TeamsInfo.GetMemberAsync(turnContext, turnContext.Activity.From.Id, cancellationToken: default);
                    if (member1 != null)
                    {
                        userConversation.UserEmail = member1.Email;
                        userConversation.UserName = member1.Name;
                        userConversation.UserPrincipalName = member1.UserPrincipalName;
                    }

                    await this.SendWelcomeCardAsync(turnContext, activity, appName);

                    /*
                    if (SendWelcomeCard)
                    {
                        try
                        {
                            if (!string.IsNullOrEmpty(AdminAppName) && !string.IsNullOrEmpty(UserAppName) && !string.IsNullOrEmpty(DescHeading))
                            {
                                var cardModel = new WelcomeCardModel
                                {
                                    SendWelcomeCard = SendWelcomeCard,
                                    AdminAppName = this.AdminAppName,
                                    UserAppName = this.UserAppName,
                                    DescHeading = this.DescHeading
                                };

                                if (appName == "AdminApp")
                                {
                                    cardModel.AppName = this.AdminAppName;
                                }
                                else
                                {
                                    cardModel.AppName = this.UserAppName;
                                }

                                var cardAttachment = this._adaptiveCardService.GetCard_Welcome_PersonalScope(cardModel);

                                if (cardAttachment != null)
                                {

                                    if (!string.IsNullOrEmpty(AdminAppFeatures) && !string.IsNullOrEmpty(UserAppFeatures))
                                    {
                                        var AdminAppFeatureList = AdminAppFeatures.Split(",").ToList();
                                        var UserAppFeatureList = UserAppFeatures.Split(",").ToList();

                                        if (AdminAppFeatureList.Any() && UserAppFeatureList.Any())
                                        {
                                            cardModel.AdminAppFeatures = AdminAppFeatureList.ToList();
                                            cardModel.UserAppFeatures = UserAppFeatureList.ToList();

                                            AdaptiveCard adaptiveCard = cardAttachment.Content as AdaptiveCard;
                                            List<AdaptiveElement> adaptiveElementList = new List<AdaptiveElement>();

                                            if (appName == "AdminApp")
                                            {
                                                //foreach (var text in cardModel.AdminAppFeatures)
                                                //{
                                                //    AdaptiveColumnSet columnSet = new AdaptiveColumnSet
                                                //    {
                                                //        Columns = new List<AdaptiveColumn>
                                                //        {
                                                //            new AdaptiveColumn
                                                //            {
                                                //                Width = "auto",
                                                //                Spacing = AdaptiveSpacing.None,
                                                //                Items = new List<AdaptiveElement>
                                                //                {
                                                //                    new AdaptiveTextBlock
                                                //                    {
                                                //                        Text = text,
                                                //                        Wrap = true,
                                                //                        Size = AdaptiveTextSize.Medium,
                                                //                        Spacing = AdaptiveSpacing.Small,
                                                //                        Color = AdaptiveTextColor.Default,
                                                //                        Weight = AdaptiveTextWeight.Default,
                                                //                        FontType = AdaptiveFontType.Default
                                                //                    }
                                                //                }
                                                //            }
                                                //        }
                                                //    };

                                                //    adaptiveElementList.Add(columnSet);
                                                //}

                                                foreach (var text in cardModel.AdminAppFeatures)
                                                {
                                                    AdaptiveFactSet factSet = new AdaptiveFactSet
                                                    {
                                                        Facts = new List<AdaptiveFact>
                                                        {
                                                            new AdaptiveFact
                                                            {
                                                                Title = "•", // Bullet point
                                                                Value = text
                                                            }
                                                        }
                                                    };

                                                    adaptiveElementList.Add(factSet);
                                                }
                                            }

                                            if (appName == "UserApp")
                                            {
                                                //foreach (var text in cardModel.UserAppFeatures)
                                                //{
                                                //    AdaptiveColumnSet columnSet = new AdaptiveColumnSet
                                                //    {
                                                //        Columns = new List<AdaptiveColumn>
                                                //        {
                                                //            new AdaptiveColumn
                                                //            {
                                                //                Width = "auto",
                                                //                Spacing = AdaptiveSpacing.None,
                                                //                Items = new List<AdaptiveElement>
                                                //                {
                                                //                    new AdaptiveTextBlock
                                                //                    {
                                                //                        Text = text,
                                                //                        Wrap = true,
                                                //                        Size = AdaptiveTextSize.Medium,
                                                //                        Spacing = AdaptiveSpacing.Small,
                                                //                        Color = AdaptiveTextColor.Default,
                                                //                        Weight = AdaptiveTextWeight.Default,
                                                //                        FontType = AdaptiveFontType.Default
                                                //                    }
                                                //                }
                                                //            }
                                                //        }
                                                //    };

                                                //    adaptiveElementList.Add(columnSet);
                                                //}

                                                foreach (var text in cardModel.UserAppFeatures)
                                                {
                                                    AdaptiveFactSet factSet = new AdaptiveFactSet
                                                    {
                                                        Facts = new List<AdaptiveFact>
                                                        {
                                                            new AdaptiveFact
                                                            {
                                                                Title = "•", // Bullet point
                                                                Value = text
                                                            }
                                                        }
                                                    };

                                                    adaptiveElementList.Add(factSet);
                                                }
                                            }

                                            var container = new AdaptiveContainer()
                                            {
                                                Separator = false,
                                                Items = adaptiveElementList
                                            };

                                            adaptiveCard.Body.Add(container);

                                            await turnContext.SendActivityAsync(MessageFactory.Attachment(cardAttachment));
                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            this._logger.LogError(ex, $"Unable to send welcome card when user installed bot -  {activity.From.AadObjectId}.");
                        }
                    }
                    */

                }
                catch (Exception ex)
                {
                    this._logger.LogError(ex, $"Unable to execute GetMemberAsync when user installed bot -  {activity.From.AadObjectId}.");
                }
                await this._conversationData.Update(userConversation);

            }
            else
            {
                var userConversationDetails = new ConversationModel
                {
                    BotInstalledOn = DateTime.Now,
                    ConversationId = activity.Conversation.Id,
                    ServiceUrl = activity.ServiceUrl,
                    UserId = Guid.Parse(activity.From.AadObjectId),
                    UserName = activity.From.Name,
                    ActivityId = activity.Id,
                    TenantId = Guid.Parse(activity.Conversation.TenantId),
                    RecipientId = activity.Recipient.Id,
                    RecipientName = activity.Recipient.Name,
                    AppName = appName
                };

                try
                {
                    var member1 = await TeamsInfo.GetMemberAsync(turnContext, turnContext.Activity.From.Id, cancellationToken: default);
                    if (member1 != null)
                    {
                        userConversationDetails.UserEmail = member1.Email;
                        userConversationDetails.UserName = member1.Name;
                        userConversationDetails.UserPrincipalName = member1.UserPrincipalName;
                    }

                    await this.SendWelcomeCardAsync(turnContext, activity, appName);

                    /*
                    if (SendWelcomeCard)
                    {
                        try
                        {
                            if (!string.IsNullOrEmpty(AdminAppName) && !string.IsNullOrEmpty(UserAppName) && !string.IsNullOrEmpty(DescHeading))
                            {
                                var cardModel = new WelcomeCardModel
                                {
                                    SendWelcomeCard = SendWelcomeCard,
                                    AdminAppName = this.AdminAppName,
                                    UserAppName = this.UserAppName,
                                    DescHeading = this.DescHeading
                                };

                                if (appName == "AdminApp")
                                {
                                    cardModel.AppName = this.AdminAppName;
                                }
                                else
                                {
                                    cardModel.AppName = this.UserAppName;
                                }

                                var cardAttachment = this._adaptiveCardService.GetCard_Welcome_PersonalScope(cardModel);

                                if (cardAttachment != null)
                                {

                                    if (!string.IsNullOrEmpty(AdminAppFeatures) && !string.IsNullOrEmpty(UserAppFeatures))
                                    {
                                        var AdminAppFeatureList = AdminAppFeatures.Split(",").ToList();
                                        var UserAppFeatureList = UserAppFeatures.Split(",").ToList();

                                        if (AdminAppFeatureList.Any() && UserAppFeatureList.Any())
                                        {
                                            cardModel.AdminAppFeatures = AdminAppFeatureList.ToList();
                                            cardModel.UserAppFeatures = UserAppFeatureList.ToList();

                                            AdaptiveCard adaptiveCard = cardAttachment.Content as AdaptiveCard;
                                            List<AdaptiveElement> adaptiveElementList = new List<AdaptiveElement>();

                                            if (appName == "AdminApp")
                                            {
                                                //foreach (var text in cardModel.AdminAppFeatures)
                                                //{
                                                //    AdaptiveColumnSet columnSet = new AdaptiveColumnSet
                                                //    {
                                                //        Columns = new List<AdaptiveColumn>
                                                //        {
                                                //            new AdaptiveColumn
                                                //            {
                                                //                Width = "auto",
                                                //                Spacing = AdaptiveSpacing.None,
                                                //                Items = new List<AdaptiveElement>
                                                //                {
                                                //                    new AdaptiveTextBlock
                                                //                    {
                                                //                        Text = text,
                                                //                        Wrap = true,
                                                //                        Size = AdaptiveTextSize.Medium,
                                                //                        Spacing = AdaptiveSpacing.Small,
                                                //                        Color = AdaptiveTextColor.Default,
                                                //                        Weight = AdaptiveTextWeight.Default,
                                                //                        FontType = AdaptiveFontType.Default
                                                //                    }
                                                //                }
                                                //            }
                                                //        }
                                                //    };

                                                //    adaptiveElementList.Add(columnSet);
                                                //}

                                                foreach (var text in cardModel.AdminAppFeatures)
                                                {
                                                    AdaptiveFactSet factSet = new AdaptiveFactSet
                                                    {
                                                        Facts = new List<AdaptiveFact>
                                                        {
                                                            new AdaptiveFact
                                                            {
                                                                Title = "•", // Bullet point
                                                                Value = text
                                                            }
                                                        }
                                                    };

                                                    adaptiveElementList.Add(factSet);
                                                }
                                            }

                                            if (appName == "UserApp")
                                            {
                                                //foreach (var text in cardModel.UserAppFeatures)
                                                //{
                                                //    AdaptiveColumnSet columnSet = new AdaptiveColumnSet
                                                //    {
                                                //        Columns = new List<AdaptiveColumn>
                                                //        {
                                                //            new AdaptiveColumn
                                                //            {
                                                //                Width = "auto",
                                                //                Spacing = AdaptiveSpacing.None,
                                                //                Items = new List<AdaptiveElement>
                                                //                {
                                                //                    new AdaptiveTextBlock
                                                //                    {
                                                //                        Text = text,
                                                //                        Wrap = true,
                                                //                        Size = AdaptiveTextSize.Medium,
                                                //                        Spacing = AdaptiveSpacing.Small,
                                                //                        Color = AdaptiveTextColor.Default,
                                                //                        Weight = AdaptiveTextWeight.Default,
                                                //                        FontType = AdaptiveFontType.Default
                                                //                    }
                                                //                }
                                                //            }
                                                //        }
                                                //    };

                                                //    adaptiveElementList.Add(columnSet);
                                                //}

                                                foreach (var text in cardModel.UserAppFeatures)
                                                {
                                                    AdaptiveFactSet factSet = new AdaptiveFactSet
                                                    {
                                                        Facts = new List<AdaptiveFact>
                                                        {
                                                            new AdaptiveFact
                                                            {
                                                                Title = "•", // Bullet point
                                                                Value = text
                                                            }
                                                        }
                                                    };

                                                    adaptiveElementList.Add(factSet);
                                                }
                                            }

                                            var container = new AdaptiveContainer()
                                            {
                                                Separator = false,
                                                Items = adaptiveElementList
                                            };

                                            adaptiveCard.Body.Add(container);

                                            await turnContext.SendActivityAsync(MessageFactory.Attachment(cardAttachment));
                                        }
                                    }
                                }
                            }                            
                        }
                        catch(Exception ex)
                        {
                            this._logger.LogError(ex, $"Unable to send welcome card when user installed bot -  {activity.From.AadObjectId}.");
                        }
                    }
                    */

                }
                catch (Exception ex)
                {
                    this._logger.LogError(ex, $"Unable to execute GetMemberAsync when user installed bot -  {activity.From.AadObjectId}.");
                }
                await this._conversationData.Insert(userConversationDetails);
            }
        }

        private async Task UpdateConversationOnBotUninstall(ITurnContext turnContext, string appName)
        {
            // Add or update user details when bot is uninstalled.
            var existingRecord = await this._conversationData.GetConversationByUserId(Guid.Parse(turnContext.Activity.From.AadObjectId), appName);

            if (existingRecord != null)
            {
                var userConversation = existingRecord;
                userConversation.ConversationId = turnContext.Activity.Conversation.Id;
                userConversation.BotInstalledOn = DateTime.UtcNow;
                userConversation.UserId = Guid.Parse(turnContext.Activity.From.AadObjectId);
                userConversation.AppName = appName;

                await this._conversationData.Remove(userConversation);
            }
        }

        #endregion

        #region Channel Scope

        public async Task OnBotRemovedInTeamsAsync(ITurnContext turnContext, string appName, TeamsChannelData teamsChannelData)
        {
            turnContext = turnContext ?? throw new ArgumentNullException(nameof(turnContext), "Turncontext cannot be null");

            await UpdateTeamsConversationOnBotUninstall(turnContext, appName, teamsChannelData);

            this._logger.LogInformation($"Successfully uninstalled app for team {teamsChannelData.Team.Name}.");
        }

        public async Task OnBotInstalledInTeamsAsync(ITurnContext<IConversationUpdateActivity> turnContext, string appName, TeamsChannelData teamsChannelData)
        {
            turnContext = turnContext ?? throw new ArgumentNullException(nameof(turnContext), "Turncontext cannot be null");

            this._logger.LogInformation($"Bot added in personal scope for user {turnContext.Activity.From.AadObjectId}");

            var activity = turnContext.Activity;

            await InsertUpdateTeamsConversation(turnContext, activity, appName, teamsChannelData);

            this._logger.LogInformation($"Successfully installed app for user {activity.From.AadObjectId}.");
        }

        private async Task InsertUpdateTeamsConversation(ITurnContext<IConversationUpdateActivity> turnContext, IConversationUpdateActivity activity, string appName, TeamsChannelData teamsChannelData)
        {
            // Add or update user details when bot is installed.
            var existingRecord = await this._conversationData.GetConversationByTeamAadGroupId(teamsChannelData.Team.AadGroupId, appName);

            if (existingRecord != null)
            {
                var userConversation = existingRecord;
                userConversation.ConversationId = activity.Conversation.Id;
                userConversation.ServiceUrl = activity.ServiceUrl;
                userConversation.BotInstalledOn = DateTime.UtcNow;

                userConversation.TeamId = teamsChannelData.Team.Id;
                userConversation.TeamName = teamsChannelData.Team.Name;
                userConversation.TeamAadGroupId = teamsChannelData.Team.AadGroupId;
                userConversation.ActivityId = activity.Id;
                userConversation.TenantId = Guid.Parse(activity.Conversation.TenantId);
                userConversation.RecipientId = activity.Recipient.Id;
                userConversation.RecipientName = activity.Recipient.Name;
                userConversation.Active = true;
                userConversation.AppName = appName;

                await this._conversationData.UpdateTeamConversation(userConversation);

            }
            else
            {
                var userConversationDetails = new ConversationTeamsModel
                {
                    BotInstalledOn = DateTime.Now,
                    ConversationId = activity.Conversation.Id,
                    ServiceUrl = activity.ServiceUrl,
                    TeamId = teamsChannelData.Team.Id,
                    TeamAadGroupId = teamsChannelData.Team.AadGroupId,
                    TeamName = teamsChannelData.Team.Name,
                    ActivityId = activity.Id,
                    TenantId = Guid.Parse(activity.Conversation.TenantId),
                    RecipientId = activity.Recipient.Id,
                    RecipientName = activity.Recipient.Name,
                    AppName = appName
                };

                await this._conversationData.InsertTeamConversation(userConversationDetails);
            }
        }

        private async Task UpdateTeamsConversationOnBotUninstall(ITurnContext turnContext, string appName, TeamsChannelData teamsChannelData)
        {
            // Add or update user details when bot is uninstalled.
            var existingRecord = await this._conversationData.GetConversationByTeamAadGroupId(teamsChannelData.Team.AadGroupId, appName);

            if (existingRecord != null)
            {
                var userConversation = existingRecord;
                userConversation.ConversationId = turnContext.Activity.Conversation.Id;
                userConversation.BotInstalledOn = DateTime.UtcNow;
                userConversation.TeamAadGroupId = teamsChannelData.Team.AadGroupId;
                userConversation.AppName = appName;

                await this._conversationData.RemoveTeamConversation(userConversation);
            }
        }

        #endregion


        #region Task module

        private Task<TaskModuleResponse> GetTaskModuleResponseAsync(string taskModuleTitle, string taskModuleUrl, string queryParams = "")
        {
            return Task.FromResult(new TaskModuleResponse
            {
                Task = new TaskModuleContinueResponse
                {
                    Value = new TaskModuleTaskInfo()
                    {
                        Url = queryParams != "" ? $"{taskModuleUrl}?theme={{theme}}&locale={{locale}}&{queryParams}" : $"{taskModuleUrl}?theme={{theme}}&locale={{locale}}",
                        Height = TaskModuleHeight,
                        Width = TaskModuleWidth,
                        Title = taskModuleTitle,
                    },
                },
            });
        }

        private Task<TaskModuleResponse> GetAdminTaskModuleResponseAsync(string taskModuleTitle, string taskModuleUrl, string queryParams = "")
        {
            return Task.FromResult(new TaskModuleResponse
            {
                Task = new TaskModuleContinueResponse
                {
                    Value = new TaskModuleTaskInfo()
                    {
                        Url = queryParams != "" ? $"{taskModuleUrl}?theme={{theme}}&locale={{locale}}&{queryParams}" : $"{taskModuleUrl}?theme={{theme}}&locale={{locale}}",
                        Height = AdminTaskModuleHeight,
                        Width = AdminTaskModuleWidth,
                        Title = taskModuleTitle,
                    },
                },
            });
        }

        public Task<TaskModuleResponse> OnFetchAsync(ITurnContext<IInvokeActivity> turnContext, TaskModuleRequest taskModuleRequest)
        {
            try
            {
                var postedValues = JsonConvert.DeserializeObject<AdaptiveCardModel>(JObject.Parse(taskModuleRequest?.Data?.ToString()).ToString());
                string command = postedValues.Command;
                var queryParams = "";
                switch (command.ToUpperInvariant())
                {
                    case BotCommandConstants.viewTask:
                        queryParams = $"id={postedValues.TaskId}";
                        this._logger.LogInformation($"Invoking task module for task id :{postedValues.TaskId}.");
                        return GetTaskModuleResponseAsync(taskModuleTitle: "Details View", taskModuleUrl: $"{this.AppBaseUrl}/detailticketenduser", queryParams: queryParams);
                    case BotCommandConstants.viewTaskAdmin:
                        queryParams = $"id={postedValues.TaskId}";
                        this._logger.LogInformation($"Invoking task module for task id :{postedValues.TaskId}.");
                        return GetAdminTaskModuleResponseAsync(taskModuleTitle: "Details View", taskModuleUrl: $"{this.AppBaseUrl}/detailticketadmin", queryParams: queryParams);
                    default:
                        this._logger.LogInformation($"Invalid command for task module fetch activity. Command is : {command} ");
                        return null;
                }

            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error while executing OnFetchAsync().");
                throw;
            }
        }

        public async Task<TaskModuleResponse> OnSubmitAsync(ITurnContext<IInvokeActivity> turnContext, TaskModuleRequest taskModuleRequest)
        {
            try
            {
                turnContext = turnContext ?? throw new ArgumentNullException(nameof(turnContext));
                var activity = (Microsoft.Bot.Schema.Activity)turnContext.Activity;

                var postedValues = JsonConvert.DeserializeObject<AdaptiveCardModel>(((JObject)activity.Value).GetValue("data", StringComparison.OrdinalIgnoreCase)?.ToString());
                string command = postedValues.Command;
                switch (command.ToUpperInvariant())
                {
                    default:
                        this._logger.LogInformation($"Invalid command for task module fetch activity.Command is : {command} ");
                        await turnContext.SendActivityAsync("Invalid command for task module fetch activity.Command is : {command} ");
                        return null;
                }

            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error while submit event is received from the user.");
                await turnContext.SendActivityAsync("Error while submit event is received from the user").ConfigureAwait(false);
                throw ex;
            }
        }

        #endregion

        /*
        #region Task module

        private Task<TaskModuleResponse> GetTaskModuleResponseAsync(string taskModuleTitle, string taskModuleUrl, string queryParams = "")
        {
            return Task.FromResult(new TaskModuleResponse
            {
                Task = new TaskModuleContinueResponse
                {
                    Value = new TaskModuleTaskInfo()
                    {
                        Url = queryParams != "" ? $"{taskModuleUrl}?theme={{theme}}&locale={{locale}}&{queryParams}" : $"{taskModuleUrl}?theme={{theme}}&locale={{locale}}",
                        Height = TaskModuleHeight,
                        Width = TaskModuleWidth,
                        Title = taskModuleTitle,
                    },
                },
            });
        }

        public Task<TaskModuleResponse> OnFetchAsync(ITurnContext<IInvokeActivity> turnContext, TaskModuleRequest taskModuleRequest)
        {
            try
            {
                var postedValues = JsonConvert.DeserializeObject<AdaptiveCardModel>(JObject.Parse(taskModuleRequest?.Data?.ToString()).ToString());
                string command = postedValues.Command;
                var queryParams = "";
                switch (command.ToUpperInvariant())
                {
                    case BotCommandConstants.viewTask:
                        queryParams = $"id={postedValues.TaskId}";
                        this.logger.LogInformation($"Invoking task module for task id :{postedValues.TaskId}.");
                        return GetTaskModuleResponseAsync(taskModuleTitle: "View Task", taskModuleUrl: $"{this.botOptions.Value.AppBaseUri}/viewparticulartask", queryParams: queryParams);
                    case BotCommandConstants.updateTask:
                        queryParams = $"id={postedValues.TaskId}";
                        this.logger.LogInformation($"Invoking task module for task id :{postedValues.TaskId}.");
                        return GetTaskModuleResponseAsync(taskModuleTitle: "Update Task", taskModuleUrl: $"{this.botOptions.Value.AppBaseUri}/updatetask", queryParams: queryParams);
                    default:
                        this.logger.LogInformation($"Invalid command for task module fetch activity.Command is : {command} ");
                        return null;
                }

            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error while fetch event is received from the user.");
                throw;
            }
        }

        public async Task<TaskModuleResponse> OnSubmitAsync(ITurnContext<IInvokeActivity> turnContext, TaskModuleRequest taskModuleRequest)
        {
            try
            {
                turnContext = turnContext ?? throw new ArgumentNullException(nameof(turnContext));
                var activity = (Activity)turnContext.Activity;

                var postedValues = JsonConvert.DeserializeObject<AdaptiveCardModel>(((JObject)activity.Value).GetValue("data", StringComparison.OrdinalIgnoreCase)?.ToString());
                string command = postedValues.Command;
                switch (command.ToUpperInvariant())
                {

                    default:
                        this.logger.LogInformation($"Invalid command for task module fetch activity.Command is : {command} ");
                        await turnContext.SendActivityAsync("Invalid command for task module fetch activity.Command is : {command} ");
                        return null;
                }

            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error while submit event is received from the user.");
                await turnContext.SendActivityAsync("Error while submit event is received from the user").ConfigureAwait(false);
                throw ex;
            }
        }

        #endregion

        */
    }
}
