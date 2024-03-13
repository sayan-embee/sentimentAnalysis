using Microsoft.Bot.Connector;
using Microsoft.Graph;
using Newtonsoft.Json;
using NSSOperationAutomationApp.Controllers;
using NSSOperationAutomationApp.DataAccessHelper;
using NSSOperationAutomationApp.DataAccessHelper.DBAccess;
using NSSOperationAutomationApp.Models;
using NSSOperationAutomationApp.ServiceMethods;
using System.Net.Sockets;
using System.Security.Cryptography.Xml;

namespace NSSOperationAutomationApp.HelperMethods
{
    public class NotificationHelper : INotificationHelper
    {
        private readonly ILogger _logger;
        //private readonly IConfiguration _config;
        private readonly IAdaptiveCardService _adaptiveCardService;
        private readonly INotificationService _notificationService;

        private static string AdminAppName = "ADMIN";
        private static string UserAppName = "USER";

        public NotificationHelper(ILogger<NotificationHelper> logger, IAdaptiveCardService adaptiveCardService, INotificationService notificationService)
        {
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this._adaptiveCardService = adaptiveCardService ?? throw new ArgumentNullException(nameof(adaptiveCardService));
            this._notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
        }

        public async Task<List<TicketAssignmentCardModel>?> ProcessNotification_AssignReassignTicketInBulk(List<TicketAssignmentCardModel> dataList)
        {
            try
            {
                if (dataList != null && dataList.Any())
                {
                    var sendCardTaskList = new List<Task<CardResponseModel?>>();

                    foreach (var data in dataList)
                    {
                        try
                        {
                            var cardAttachment = this._adaptiveCardService.GetCard_AssignTicket_PersonalScope(data);

                            if (!string.IsNullOrEmpty(data.AssignedToADID) && !string.IsNullOrEmpty(data.CaseNumber) && cardAttachment != null)
                            {
                                Guid userId = Guid.Empty;

                                Guid guidValue;
                                if (Guid.TryParse(data.AssignedToADID, out guidValue))
                                {
                                    userId = guidValue;
                                }

                                var conversationModel = new ConversationModel
                                {
                                    UserName = data.AssignedTo,
                                    UserId = userId,
                                    ConversationId = data.ConversationId,
                                    ServiceUrl = data.ServiceUrl,
                                };

                                sendCardTaskList.Add(_notificationService.SendCard_PersonalScope(conversationModel, cardAttachment, data.CaseNumber, UserAppName));
                            }
                        }
                        catch (Exception ex)
                        {
                            this._logger.LogError(ex, $"NotificationHelper --> GetCard_AssignTicket_PersonalScope() execution failed: {JsonConvert.SerializeObject(data, Formatting.Indented)}");
                            ExceptionLogging.SendErrorToText(ex);
                        }
                    }

                    try
                    {
                        if (sendCardTaskList != null && sendCardTaskList.Any())
                        {
                            var sendCardTaskList_Response = await Task.WhenAll(sendCardTaskList);

                            if (sendCardTaskList_Response != null && sendCardTaskList_Response.Any())
                            {
                                foreach (var data in dataList)
                                {
                                    var cardDetails = sendCardTaskList_Response.FirstOrDefault(result => result?.ReferenceId == data.CaseNumber);

                                    data.ReplyToId = cardDetails?.ReplyToId;
                                    data.ActivityId = cardDetails?.ActivityId;
                                    data.ConversationId = cardDetails?.ConversationId;
                                    data.ServiceUrl = cardDetails?.ServiceUrl;
                                    data.UserName = cardDetails?.UserName ?? data.AssignedTo;
                                    data.UserADID = cardDetails?.UserADID ?? data.AssignedToADID;
                                    data.Status = cardDetails?.Status ?? false;
                                    data.ReferenceId = cardDetails?.ReferenceId;
                                    data.Type = AdaptiveCardType.Type1.ToString();
                                }

                                return dataList;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        this._logger.LogError(ex, $"NotificationHelper --> ProcessNotification_AssignReassignTicket() execution failed");
                        ExceptionLogging.SendErrorToText(ex);
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, $"NotificationHelper --> ProcessNotification_AssignReassignTicket() execution failed");
                ExceptionLogging.SendErrorToText(ex);
                return null;
            }
        }

        public async Task<List<TicketAssignmentCardModel>?> ProcessNotification_AssignReassignTicket(List<TicketAssignmentCardModel> dataList)
        {
            try
            {
                if (dataList != null && dataList.Any())
                {
                    var sendCardTaskList = new List<Task<CardResponseModel?>>();

                    foreach (var data in dataList)
                    {
                        try
                        {
                            var cardAttachment = this._adaptiveCardService.GetCard_AssignTicket_PersonalScope(data);

                            if (!string.IsNullOrEmpty(data.AssignedToADID) && !string.IsNullOrEmpty(data.CaseNumber) && cardAttachment != null)
                            {
                                sendCardTaskList.Add(_notificationService.SendCard_PersonalScope(data.AssignedToADID, cardAttachment, data.CaseNumber, UserAppName));
                            }
                        }
                        catch(Exception ex)
                        {
                            this._logger.LogError(ex, $"NotificationHelper --> GetCard_AssignTicket_PersonalScope() execution failed: {JsonConvert.SerializeObject(data, Formatting.Indented)}");
                            ExceptionLogging.SendErrorToText(ex);
                        }
                    }

                    try
                    {
                        if (sendCardTaskList != null && sendCardTaskList.Any())
                        {
                            var sendCardTaskList_Response = await Task.WhenAll(sendCardTaskList);

                            if (sendCardTaskList_Response != null && sendCardTaskList_Response.Any())
                            {
                                foreach (var data in dataList)
                                {
                                    var cardDetails = sendCardTaskList_Response.FirstOrDefault(result => result?.ReferenceId == data.CaseNumber);

                                    data.ReplyToId = cardDetails?.ReplyToId;
                                    data.ActivityId = cardDetails?.ActivityId;
                                    data.ConversationId = cardDetails?.ConversationId;
                                    data.ServiceUrl = cardDetails?.ServiceUrl;
                                    data.UserName = cardDetails?.UserName ?? data.AssignedTo;
                                    data.UserADID = cardDetails?.UserADID ?? data.AssignedToADID;
                                    data.Status = cardDetails?.Status ?? false;
                                    data.ReferenceId = cardDetails?.ReferenceId;
                                    data.Type = AdaptiveCardType.Type1.ToString();
                                }

                                return dataList;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        this._logger.LogError(ex, $"NotificationHelper --> ProcessNotification_AssignReassignTicket() execution failed");
                        ExceptionLogging.SendErrorToText(ex);
                    }
                }

                return null;
            }
            catch (Exception ex) 
            {
                this._logger.LogError(ex, $"NotificationHelper --> ProcessNotification_AssignReassignTicket() execution failed");
                ExceptionLogging.SendErrorToText(ex);
                return null;
            }
        }

        public async Task<List<TicketAssignmentCardModel>?> ProcessNotification_TicketActionByEngineer(List<TicketActionCardModel> dataList)
        {
            try
            {
                if (dataList != null && dataList.Any())
                {
                    var returnModelList = new List<TicketAssignmentCardModel>();
                    var sendCardTaskList = new List<Task<CardResponseModel?>>();

                    foreach (var data in dataList)
                    {
                        try
                        {
                            var cardAttachment = this._adaptiveCardService.GetCard_TicketActionByEng_PersonalScope(data);

                            if (!string.IsNullOrEmpty(data.AssignedByADID) && !string.IsNullOrEmpty(data.CaseNumber) && cardAttachment != null)
                            {
                                sendCardTaskList.Add(_notificationService.SendCard_PersonalScope(data.AssignedByADID, cardAttachment, data.CaseNumber, AdminAppName));
                            }
                        }
                        catch (Exception ex)
                        {
                            this._logger.LogError(ex, $"NotificationHelper --> GetCard_TicketActionByEng_PersonalScope() execution failed: {JsonConvert.SerializeObject(data, Formatting.Indented)}");
                            ExceptionLogging.SendErrorToText(ex);
                        }
                    }

                    try
                    {
                        if (sendCardTaskList != null && sendCardTaskList.Any())
                        {
                            var sendCardTaskList_Response = await Task.WhenAll(sendCardTaskList);

                            if (sendCardTaskList_Response != null && sendCardTaskList_Response.Any())
                            {
                                foreach (var data in dataList)
                                {
                                    var cardDetails = sendCardTaskList_Response.FirstOrDefault(result => result?.ReferenceId == data.CaseNumber);

                                    data.ReplyToId = cardDetails?.ReplyToId;
                                    data.ActivityId = cardDetails?.ActivityId;
                                    data.ConversationId = cardDetails?.ConversationId;
                                    data.ServiceUrl = cardDetails?.ServiceUrl;
                                    data.UserName = cardDetails?.UserName ?? data.AssignedTo;
                                    data.UserADID = cardDetails?.UserADID ?? data.AssignedToADID;
                                    data.Status = cardDetails?.Status ?? false;
                                    data.ReferenceId = cardDetails?.ReferenceId;
                                    data.Type = AdaptiveCardType.Type2.ToString();

                                    returnModelList.Add(new TicketAssignmentCardModel
                                    {
                                        ReplyToId = cardDetails?.ReplyToId,
                                        ActivityId = cardDetails?.ActivityId,
                                        ConversationId = cardDetails?.ConversationId,
                                        ServiceUrl = cardDetails?.ServiceUrl,
                                        UserName = cardDetails?.UserName ?? data.AssignedTo,
                                        UserADID = cardDetails?.UserADID ?? data.AssignedToADID,
                                        Status = cardDetails?.Status ?? false,
                                        ReferenceId = cardDetails?.ReferenceId,
                                        Type = AdaptiveCardType.Type2.ToString(),
                                        AssignmentId = data.AssignmentId,
                                        AssignmentHistoryId = data.AssignmentHistoryId,
                                        CallDetailId = data.CallDetailId,
                                        CaseNumber = data.CaseNumber
                                    });
                                }

                                return returnModelList;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        this._logger.LogError(ex, $"NotificationHelper --> ProcessNotification_TicketActionByEngineer() execution failed");
                        ExceptionLogging.SendErrorToText(ex);
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, $"NotificationHelper --> ProcessNotification_TicketActionByEngineer() execution failed");
                ExceptionLogging.SendErrorToText(ex);
                return null;
            }
        }

        public async Task<List<TicketAssignmentCardModel>?> ProcessNotification_TicketActionByAdmin(List<TicketActionCardModel> dataList)
        {
            try
            {
                if (dataList != null && dataList.Any())
                {
                    var returnModelList = new List<TicketAssignmentCardModel>();
                    var sendCardTaskList = new List<Task<CardResponseModel?>>();

                    foreach (var data in dataList)
                    {
                        try
                        {
                            var cardAttachment = this._adaptiveCardService.GetCard_TicketActionByAdmin_PersonalScope(data);

                            if (!string.IsNullOrEmpty(data.AssignedToADID) && !string.IsNullOrEmpty(data.CaseNumber) && cardAttachment != null)
                            {
                                sendCardTaskList.Add(_notificationService.SendCard_PersonalScope(data.AssignedToADID, cardAttachment, data.CaseNumber, UserAppName));
                            }
                        }
                        catch (Exception ex)
                        {
                            this._logger.LogError(ex, $"NotificationHelper --> GetCard_TicketActionByAdmin_PersonalScope() execution failed: {JsonConvert.SerializeObject(data, Formatting.Indented)}");
                            ExceptionLogging.SendErrorToText(ex);
                        }
                    }

                    try
                    {
                        if (sendCardTaskList != null && sendCardTaskList.Any())
                        {
                            var sendCardTaskList_Response = await Task.WhenAll(sendCardTaskList);

                            if (sendCardTaskList_Response != null && sendCardTaskList_Response.Any())
                            {
                                foreach (var data in dataList)
                                {
                                    var cardDetails = sendCardTaskList_Response.FirstOrDefault(result => result?.ReferenceId == data.CaseNumber);

                                    data.ReplyToId = cardDetails?.ReplyToId;
                                    data.ActivityId = cardDetails?.ActivityId;
                                    data.ConversationId = cardDetails?.ConversationId;
                                    data.ServiceUrl = cardDetails?.ServiceUrl;
                                    data.UserName = cardDetails?.UserName ?? data.AssignedTo;
                                    data.UserADID = cardDetails?.UserADID ?? data.AssignedToADID;
                                    data.Status = cardDetails?.Status ?? false;
                                    data.ReferenceId = cardDetails?.ReferenceId;
                                    data.Type = AdaptiveCardType.Type3.ToString();

                                    returnModelList.Add(new TicketAssignmentCardModel
                                    {
                                        ReplyToId = cardDetails?.ReplyToId,
                                        ActivityId = cardDetails?.ActivityId,
                                        ConversationId = cardDetails?.ConversationId,
                                        ServiceUrl = cardDetails?.ServiceUrl,
                                        UserName = cardDetails?.UserName ?? data.AssignedTo,
                                        UserADID = cardDetails?.UserADID ?? data.AssignedToADID,
                                        Status = cardDetails?.Status ?? false,
                                        ReferenceId = cardDetails?.ReferenceId,
                                        Type = AdaptiveCardType.Type3.ToString(),
                                        AssignmentId = data.AssignmentId,
                                        AssignmentHistoryId = data.AssignmentHistoryId,
                                        CallDetailId = data.CallDetailId,
                                        CaseNumber = data.CaseNumber
                                    });
                                }

                                return returnModelList;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        this._logger.LogError(ex, $"NotificationHelper --> ProcessNotification_TicketActionByAdmin() execution failed");
                        ExceptionLogging.SendErrorToText(ex);
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, $"NotificationHelper --> ProcessNotification_TicketActionByAdmin() execution failed");
                ExceptionLogging.SendErrorToText(ex);
                return null;
            }
        }
    }
}
