using AdaptiveCards;
using AdaptiveCards.Templating;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Microsoft.VisualBasic;
using NSSOperationAutomationApp.Models;

namespace NSSOperationAutomationApp.ServiceMethods
{
    public class AdaptiveCardService : IAdaptiveCardService
    {
        private const string WelcomeCardCacheKey = "_welcome-card";

        private const string AssignTicket_CacheKey = "_assignTicket";

        private const string TicketActionByEng_CacheKey = "_ticketActionByEng";

        private const string TicketActionByAdmin_CacheKey = "_ticketActionByAdmin";

        /// <summary>
        /// Memory cache instance to store and retrieve adaptive card payload.
        /// </summary>
        private readonly IMemoryCache memoryCache;

        /// <summary>
        /// Information about the web hosting environment an application is running in.
        /// </summary>
        private readonly IWebHostEnvironment env;

        private readonly int CardCacheInHours = 12;

        public AdaptiveCardService(IMemoryCache memoryCache, IWebHostEnvironment env, IOptions<AppSettings> appOptions)
        {
            this.memoryCache = memoryCache;
            this.env = env;
            if ((appOptions.Value.CardCacheDurationInHour) > 0)
            {
                CardCacheInHours = appOptions.Value.CardCacheDurationInHour;
            }
        }

        #region PRIVATE METHODS

        private string GetCardPayload(string cardCacheKey, string jsonTemplateFileName)
        {
            bool isCacheEntryExists = this.memoryCache.TryGetValue(cardCacheKey, out string cardPayload);

            if (!isCacheEntryExists)
            {
                // If cache duration is not specified then by default cache for 12 hours.
                var cacheDurationInHour = TimeSpan.FromHours(CardCacheInHours);
                cacheDurationInHour = cacheDurationInHour.Hours <= 0 ? TimeSpan.FromHours(12) : cacheDurationInHour;

                var cardJsonFilePath = Path.Combine(this.env.ContentRootPath, $".\\Cards\\{jsonTemplateFileName}");
                cardPayload = File.ReadAllText(cardJsonFilePath);
                this.memoryCache.Set(cardCacheKey, cardPayload, cacheDurationInHour);
            }
            return cardPayload;
        }



        #endregion

        public Attachment GetCard_Welcome_PersonalScope(WelcomeCardModel data)
        {
            var cardPayload = this.GetCardPayload(WelcomeCardCacheKey, "\\welcomeCard.json");
            var template = new AdaptiveCardTemplate(cardPayload);

            var cardJson = template.Expand(data);
            AdaptiveCard card = AdaptiveCard.FromJson(cardJson).Card;

            var adaptiveCardAttachment = new Attachment()
            {
                ContentType = AdaptiveCard.ContentType,
                Content = card,
            };
            return adaptiveCardAttachment;
        }

        public Attachment GetCard_AssignTicket_PersonalScope(TicketAssignmentCardModel data)
        {
            var cardPayload = this.GetCardPayload(AssignTicket_CacheKey, "\\NotificationCard\\newTicketAssignmentCard.json");
            var template = new AdaptiveCardTemplate(cardPayload);

            if (string.IsNullOrEmpty(data.ServiceAccount))
            {
                data.ServiceAccount = "\\-";
            }

            var cardJson = template.Expand(data);
            AdaptiveCard card = AdaptiveCard.FromJson(cardJson).Card;

            var adaptiveCardAttachment = new Attachment()
            {
                ContentType = AdaptiveCard.ContentType,
                Content = card,
            };
            return adaptiveCardAttachment;
        }

        public Attachment GetCard_TicketActionByEng_PersonalScope(TicketActionCardModel data)
        {
            var cardPayload = this.GetCardPayload(TicketActionByEng_CacheKey, "\\NotificationCard\\newTicketActionCard.json");
            var template = new AdaptiveCardTemplate(cardPayload);

            if (string.IsNullOrEmpty(data.ServiceAccount))
            {
                data.ServiceAccount = "\\-";
            }

            if (string.IsNullOrEmpty(data.CloserRemarks))
            {
                data.CloserRemarks = "\\-";
            }

            var cardJson = template.Expand(data);
            AdaptiveCard card = AdaptiveCard.FromJson(cardJson).Card;

            var adaptiveCardAttachment = new Attachment()
            {
                ContentType = AdaptiveCard.ContentType,
                Content = card,
            };
            return adaptiveCardAttachment;
        }

        public Attachment GetCard_TicketActionByAdmin_PersonalScope(TicketActionCardModel data)
        {
            var cardPayload = this.GetCardPayload(TicketActionByAdmin_CacheKey, "\\NotificationCard\\newTicketActionAdminCard.json");
            var template = new AdaptiveCardTemplate(cardPayload);

            if (string.IsNullOrEmpty(data.ServiceAccount))
            {
                data.ServiceAccount = "\\-";
            }

            if (string.IsNullOrEmpty(data.CloserRemarks))
            {
                data.CloserRemarks = "\\-";
            }

            var cardJson = template.Expand(data);
            AdaptiveCard card = AdaptiveCard.FromJson(cardJson).Card;

            var adaptiveCardAttachment = new Attachment()
            {
                ContentType = AdaptiveCard.ContentType,
                Content = card,
            };
            return adaptiveCardAttachment;
        }
    }
}
