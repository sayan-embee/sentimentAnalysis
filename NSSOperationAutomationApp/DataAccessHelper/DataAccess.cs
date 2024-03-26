using Dapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Graph;
using NSSOperationAutomationApp.Controllers;
using NSSOperationAutomationApp.DataAccessHelper;
using NSSOperationAutomationApp.DataAccessHelper.DBAccess;
using NSSOperationAutomationApp.Models;
using System;
using System.Data;
using System.Data.SqlClient;
using static NSSOperationAutomationApp.Models.MasterModels;

namespace NSSOperationAutomationApp.DataAccessHelper
{
    public class DataAccess : IDataAccess
    {
        private readonly ILogger _logger;
        private readonly IOptions<Settings> _settings;
        private readonly ISQLDataAccess _db;
        private readonly IConfiguration _config;

        public DataAccess(IOptions<Settings> settings, 
            ISQLDataAccess db, 
            ILogger<DataAccess> logger, 
            IConfiguration config)
        {
            this._settings = settings ?? throw new ArgumentNullException(nameof(settings));
            this._db = db ?? throw new ArgumentNullException(nameof(db));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this._config = config ?? throw new ArgumentNullException(nameof(config));
        }

        #region MASTER TABLES

        public async Task<List<CallActionModel>?> GetCallAction(int? Id)
        {
            try
            {
                var results = await _db.LoadData<CallActionModel, dynamic>("usp_M_CallAction_Get",
                new
                {
                   Id
                });

                return results.ToList();
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, $"DataAccess --> GetCallAction() --> SQL(usp_M_CallAction_Get) execution failed");
                return null;
            }
        }

        public async Task<List<DocumentTypeModel>?> GetDocumentType(int? Id)
        {
            try
            {
                var results = await _db.LoadData<DocumentTypeModel, dynamic>("usp_M_DocumentType_Get",
                new
                {
                    Id
                });

                return results.ToList();
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, $"DataAccess --> GetDocumentType() --> SQL(usp_M_DocumentType_Get) execution failed");
                return null;
            }
        }

        public async Task<List<PartConsumptionTypeModel>?> GetPartConsumptionType(int? Id)
        {
            try
            {
                var results = await _db.LoadData<PartConsumptionTypeModel, dynamic>("usp_M_PartConsumptionType_Get",
                new
                {
                    Id
                });

                return results.ToList();
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, $"DataAccess --> GetPartConsumptionType() --> SQL(usp_M_PartConsumptionType_Get) execution failed");
                return null;
            }
        }

        public async Task<List<CallStatusModel>?> GetCallStatus(int? Id)
        {
            try
            {
                var results = await _db.LoadData<CallStatusModel, dynamic>("usp_M_CallStatus_Get",
                new
                {
                    Id
                });

                return results.ToList();
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, $"DataAccess --> GetCallStatus() --> SQL(usp_M_CallStatus_Get) execution failed");
                return null;
            }
        }

        #endregion

        #region TRANSACTION TABLES

        public async Task<ReturnMessageModel?> Insert(TicketDetailsModel data)
        {
            try
            {   
                var result = await _db.SaveData<ReturnMessageModel, dynamic>(storedProcedure: "usp_Ticket_Insert",
                new
                {
                    data.CaseNumber,
                    data.FromEmail,
                    data.ToEmail,
                    data.CaseSubject,
                    data.WorkOrderNumber,
                    data.ServiceAccount,
                    data.ContactName,
                    data.ContactEmail,
                    data.ContactPhone,
                    data.ServiceDeliveryStreetAddress,
                    data.ServiceDeliveryCity,
                    data.ServiceDeliveryCountry,
                    data.PostalCode,
                    data.SerialNumber,
                    data.ProductName,
                    data.ProductNumber,
                    data.OTCCode,
                    data.PartNumber,
                    data.PartDescription,
                    data.EmailSubject,
                    data.EmailDate,
                    data.PartNumber2,
                    data.PartDescription2,
                    data.PartNumber3,
                    data.PartDescription3
                });

                return result.FirstOrDefault();
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, $"DataAccess --> Insert --> SQL(usp_Ticket_Insert) execution failed");
                ExceptionLogging.SendErrorToText(ex);
                return null;
            }
        }

        public async Task<ReturnMessageModel?> InsertTicketInBulk(List<TicketCreateInBulkModel> dataList)
        {
            try
            {
                var udt = TableValuedParameters_Get_TicketList(dataList);

                var result = await _db.SaveData<ReturnMessageModel, dynamic>(storedProcedure: "usp_TicketInBulk_Insert",
                new
                {
                    udt_TicketDetailsList = udt
                });

                return result.FirstOrDefault();
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, $"DataAccess --> InsertTicketInBulk --> SQL(usp_TicketInBulk_Insert) execution failed");
                ExceptionLogging.SendErrorToText(ex);
                return null;
            }
        }

        public async Task<List<TicketDetailsModel>?> Get(FilterModel data)
        {
            try
            {
                var results = await _db.LoadData<TicketDetailsModel, dynamic>("dbo.usp_Ticket_Get",
                new
                {
                    CaseNumber = data.CaseNumber
                    ,SerialNumber = data.SerialNumber
                    ,PartNumber = data.PartNumber
                    ,FromDate = data.FromDate
                    ,ToDate = data.ToDate
                });

                return results.ToList();
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, $"DataAccess --> Get() --> SQL(usp_Ticket_Get) execution failed");
                return null;
            }
        }

        public async Task<List<TicketDetailsOverviewModel>?> GetTicketsOverview(FilterModel data)
        {
            try
            {
                if (string.IsNullOrEmpty(data.CaseNumber))
                {
                    var results = await _db.LoadData<TicketDetailsOverviewModel, dynamic>("usp_T_Ticket_ByCaseNumber",
                    new
                    {
                        data.CaseNumber,
                        data.TicketId,
                        data.SerialNumber,
                        data.PartNumber,
                        data.FromDate,
                        data.ToDate,
                        data.AssignedTo,
                        data.AssignedToEmail,
                        data.IsAssigned,
                        data.CallStatusId,
                        data.CallActionId
                    });

                    if (results != null && results.Any())
                    {
                        return results.ToList();
                    }                    
                }
                else
                {
                    using IDbConnection connection = new SqlConnection(_config.GetConnectionString("Default"));
                    var results = await connection.QueryMultipleAsync("usp_T_Ticket_ByCaseNumber",
                    new
                    {
                        data.CaseNumber,
                        data.TicketId,
                        data.SerialNumber,
                        data.PartNumber,
                        data.FromDate,
                        data.ToDate,
                        data.AssignedTo,
                        data.AssignedToEmail,
                        data.IsAssigned,
                        data.CallStatusId,
                        data.CallActionId
                    }, commandType: CommandType.StoredProcedure);

                    if (results != null)
                    {                       

                        var ticketDetails = await results.ReadAsync<TicketDetailsOverviewModel>();

                        if (ticketDetails != null && ticketDetails.Count() > 0)
                        {
                            // Get the ticket timeline
                            if (data.IsTimelineView ?? false)
                            {
                                var timeline = await GetTicketTimelineByCaseNo(data.CaseNumber);

                                if (timeline != null)
                                {
                                    ticketDetails.FirstOrDefault().TicketTimeline = timeline;
                                }
                            }

                            var assignmentDetails = await results.ReadAsync<EngineerDetailsModel>();

                            if (assignmentDetails != null && assignmentDetails.Any())
                            {                              
                                var assignmentHistoryDetails = await results.ReadAsync<EngineerHistoryDetailsModel>();

                                if (assignmentHistoryDetails != null && assignmentHistoryDetails.Any())
                                {
                                    assignmentDetails.FirstOrDefault().EngineerHistoryDetails = assignmentHistoryDetails.ToList();
                                }

                                ticketDetails.FirstOrDefault().EngineerDetails = assignmentDetails.FirstOrDefault();

                                var callDetails = await results.ReadAsync<CallDetailsModel>();

                                if (callDetails != null)
                                {
                                    var callDocumentList = await results.ReadAsync<CallDocumentsModel>();

                                    if (callDocumentList != null && callDocumentList.Any())
                                    {
                                        foreach (var call in callDetails.ToList())
                                        {
                                            var callDocumentListPerCall = callDocumentList.Where(x => x.CallDetailId == call.CallDetailId);

                                            if (callDocumentListPerCall != null && callDocumentListPerCall.Any())
                                            {
                                                call.CallDocumentList = callDocumentListPerCall.ToList();
                                            }
                                        }
                                    }

                                    ticketDetails.FirstOrDefault().CallDetailsList = callDetails.ToList();
                                }
                            }                            

                            return ticketDetails.ToList();
                        }
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, $"DataAccess --> GetTicketsByCaseNo() --> SQL(usp_T_Ticket_ByCaseNumber) execution failed");
                return null;
            }
        }

        public async Task<TicketTimelineViewModel?> GetTicketTimelineByCaseNo(string CaseNumber)
        {
            try
            {
                if (!string.IsNullOrEmpty(CaseNumber))
                {                  
                    using IDbConnection connection = new SqlConnection(_config.GetConnectionString("Default"));
                    var results = await connection.QueryMultipleAsync("usp_T_TicketTimeline_ByCaseNumber",
                    new
                    {
                        CaseNumber
                    }, commandType: CommandType.StoredProcedure);

                    if (results != null)
                    {
                        var outputModel = new TicketTimelineViewModel();

                        var timelineList = await results.ReadAsync<TicketTimelineModel>();

                        if (timelineList != null && timelineList.Any())
                        {
                            outputModel.TicketTimelineList = timelineList.ToList();

                            var callDocumentList = await results.ReadAsync<CallDocumentsModel>();

                            if (callDocumentList != null && callDocumentList.Any())
                            {
                                outputModel.CallDocumentList = callDocumentList.ToList();
                            }

                            return outputModel;
                        }
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, $"DataAccess --> GetTicketTimelineByCaseNo() --> SQL(usp_T_TicketTimeline_ByCaseNumber) execution failed");
                return null;
            }
        }

        public async Task<ReturnMessageModel?> TicketUpdateByAdmin(TicketAssignmentModel data)
        {
            try
            {
                var result = await _db.SaveData<ReturnMessageModel, dynamic>(storedProcedure: "usp_T_EngineerAssignment_InsertUpdate",
                new
                {
                    data.TransactionType,
                    data.AssignmentId,
                    data.CaseNumber,
                    data.CallActionId,
                    data.CallStatusId,
                    data.ClosedBy,
                    data.ClosedByEmail,
                    data.ClosedByADID,
                    data.AdminClosureRemarks
                });

                return result.FirstOrDefault();
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, $"DataAccess --> AssignEngineer --> SQL(usp_T_EngineerAssignment_InsertUpdate) execution failed");
                ExceptionLogging.SendErrorToText(ex);
                return null;
            }
        }

        public async Task<ReturnMessageModel?> AssignReassignEngineer(TicketAssignmentModel data)
        {
            try
            {
                var result = await _db.SaveData<ReturnMessageModel, dynamic>(storedProcedure: "usp_T_EngineerAssignment_InsertUpdate",
                new
                {
                    data.TransactionType,
                    data.TicketId,
                    data.AssignmentId,
                    data.CaseNumber,
                    data.AssignedTo,
                    data.AssignedToEmail,
                    data.AssignedToADID,
                    data.AssignedBy,
                    data.AssignedByEmail,
                    data.AssignedByADID,
                    data.AdminClosureRemarks
                });

                return result.FirstOrDefault();
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, $"DataAccess --> AssignReassignEngineer --> SQL(usp_T_EngineerAssignment_InsertUpdate) execution failed");
                ExceptionLogging.SendErrorToText(ex);
                return null;
            }
        }

        public async Task<List<EngineerDetailsModel>?> GetAssignedTickets(TicketAssignmentModel data)
        {
            try
            {
                var results = await _db.LoadData<EngineerDetailsModel, dynamic>("usp_T_EngineerAssignment_Get",
                new
                {
                    data.AssignmentId,
                    data.CaseNumber,
                    data.SerialNumber,
                    data.PartNumber,
                    data.FromDate,
                    data.ToDate,
                    data.AssignedTo,
                    data.AssignedToEmail,
                    data.AssignedBy,
                    data.AssignedByEmail
                });

                return results.ToList();
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, $"DataAccess --> GetAssignedTickets() --> SQL(usp_T_EngineerAssignment_Get) execution failed");
                return null;
            }
        }

        public async Task<ReturnMessageModel?> EngineerActionOnTicket(TicketActionModel data)
        {
            var udt = TableValuedParameters_Get_DocumentList(data.CallDocumentList);

            try
            {
                var result = await _db.SaveData<ReturnMessageModel, dynamic>(storedProcedure: "usp_T_CallDetails_InsertUpdate",
                new
                {
                    data.TransactionType,
                    data.CallDetailId,
                    data.AssignmentId,
                    data.UpdatedBy,
                    data.UpdatedByEmail,
                    data.UpdatedByADID,
                    data.CallActionId,
                    data.CallStatusId,
                    data.CustomerName,
                    data.CaseNumber,
                    data.UnitSlNo,
                    data.PassId,
                    data.TaskStartDateTimeIST,
                    data.TaskEndDateTimeIST,
                    data.CloserRemarks,
                    data.PartConsumptionTypeId,
                    data.SONo,
                    data.RequiredPart,
                    data.RequiredSparePartNo,
                    data.RequiredPartName,
                    data.FaultyPartCTNo,
                    data.FailureId,
                    data.IssueDescription,
                    data.TroubleshootingStep,
                    data.FirstPartConsumptionTypeId,
                    data.FirstPartSONo,
                    data.FirstRequiredPartName,
                    data.ReceivedPartConsumptionTypeId,
                    data.ReceivedPartName,
                    udt_CallDocuments = udt
                });

                return result.FirstOrDefault();
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, $"DataAccess --> EngineerActionOnTicket --> SQL(usp_T_CallDetails_InsertUpdate) execution failed");
                ExceptionLogging.SendErrorToText(ex);
                return null;
            }
        }

        public async Task<ReturnMessageModel?> EngineerDocumentUpdateOnTicket(List<CallDocumentsModel> dataList)
        {
            var udt = TableValuedParameters_Get_DocumentList(dataList);

            try
            {
                var result = await _db.SaveData<ReturnMessageModel, dynamic>(storedProcedure: "usp_T_CallDocuments_InsertUpdate",
                new
                {                    
                    udt_CallDocuments = udt
                });

                return result.FirstOrDefault();
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, $"DataAccess --> EngineerDocumentUpdateOnTicket --> SQL(usp_T_CallDocuments_InsertUpdate) execution failed");
                ExceptionLogging.SendErrorToText(ex);
                return null;
            }
        }

        #region NOTIFICATION

        public async Task<ReturnMessageModel?> Insert_AssignReassignTicket_CardResponse(List<TicketAssignmentCardModel> dataList)
        {
            try
            {
                var udt = TableValuedParameters_Get_CardList(dataList);

                var result = await _db.SaveData<ReturnMessageModel, dynamic>(storedProcedure: "usp_AssignReassignTicket_CardResponse_Insert",
                new
                {
                    udt_CardList = udt
                });

                return result.FirstOrDefault();
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, $"DataAccess --> Insert_AssignReassignTicket_CardResponse() --> SQL(usp_AssignReassignTicket_CardResponse_Insert) execution failed");
                ExceptionLogging.SendErrorToText(ex);
                return null;
            }
        }

        #endregion

        #region Private Helper Methods

        private DataTable TableValuedParameters_Get_TicketList(List<TicketCreateInBulkModel> udt)
        {
            var output = new DataTable();

            // Define columns for the DataTable based on the udt_T_TicketDetails type
            output.Columns.Add("CaseNumber", typeof(string));
            output.Columns.Add("TicketId", typeof(long));
            //output.Columns.Add("FromEmail", typeof(string));
            //output.Columns.Add("ToEmail", typeof(string));
            output.Columns.Add("CaseSubject", typeof(string));
            //output.Columns.Add("WorkOrderNumber", typeof(string));
            //output.Columns.Add("ServiceAccount", typeof(string));
            output.Columns.Add("ContactName", typeof(string));
            output.Columns.Add("ContactEmail", typeof(string));
            //output.Columns.Add("ContactPhone", typeof(string));
            //output.Columns.Add("ServiceDeliveryStreetAddress", typeof(string));
            //output.Columns.Add("ServiceDeliveryCity", typeof(string));
            //output.Columns.Add("ServiceDeliveryCountry", typeof(string));
            //output.Columns.Add("PostalCode", typeof(string));
            output.Columns.Add("SerialNumber", typeof(string));
            output.Columns.Add("ProductName", typeof(string));
            output.Columns.Add("ProductNumber", typeof(string));
            //output.Columns.Add("OTCCode", typeof(string));
            //output.Columns.Add("PartNumber", typeof(string));
            //output.Columns.Add("PartDescription", typeof(string));
            //output.Columns.Add("EmailSubject", typeof(string));
            //output.Columns.Add("EmailDate", typeof(string));
            output.Columns.Add("CreatedOn", typeof(string));
            //output.Columns.Add("PartNumber2", typeof(string));
            //output.Columns.Add("PartDescription2", typeof(string));
            //output.Columns.Add("PartNumber3", typeof(string));
            //output.Columns.Add("PartDescription3", typeof(string));
            output.Columns.Add("TicketType", typeof(string));
            output.Columns.Add("AssignedTo", typeof(string));
            output.Columns.Add("AssignedToEmail", typeof(string));
            output.Columns.Add("AssignedToADID", typeof(string));
            output.Columns.Add("AssignedBy", typeof(string));
            output.Columns.Add("AssignedByEmail", typeof(string));
            output.Columns.Add("AssignedByADID", typeof(string));

            if (udt != null)
            {
                foreach (var row in udt)
                {
                    if (row != null)
                    {
                        output.Rows.Add(
                            row.CaseNumber,
                            row.TicketId,
                            //row.FromEmail,
                            //row.ToEmail,
                            row.CaseSubject,
                            //row.WorkOrderNumber,
                            //row.ServiceAccount,
                            row.ContactName,
                            row.ContactEmail,
                            //row.ContactPhone,
                            //row.ServiceDeliveryStreetAddress,
                            //row.ServiceDeliveryCity,
                            //row.ServiceDeliveryCountry,
                            //row.PostalCode,
                            row.SerialNumber,
                            row.ProductName,
                            row.ProductNumber,
                            //row.OTCCode,
                            //row.PartNumber,
                            //row.PartDescription,
                            //row.EmailSubject,
                            //row.EmailDate,
                            row.CreatedOn,
                            //row.PartNumber2,
                            //row.PartDescription2,
                            //row.PartNumber3,
                            //row.PartDescription3,
                            row.TicketType,
                            row.AssignedTo,
                            row.AssignedToEmail,
                            row.AssignedToADID,
                            row.AssignedBy,
                            row.AssignedByEmail,
                            row.AssignedByADID
                        );
                    }
                }
            }

            return output;
        }

        private DataTable TableValuedParameters_Get_DocumentList(List<CallDocumentsModel> udt)
        {
            var output = new DataTable();
            output.Columns.Add("DocumentId", typeof(Int64));
            output.Columns.Add("CallDetailId", typeof(Int64));
            output.Columns.Add("DocumentTypeId", typeof(Int32));
            output.Columns.Add("DocumentName", typeof(string));
            output.Columns.Add("MimeType", typeof(string));
            output.Columns.Add("DocumentUrlPath", typeof(string));
            output.Columns.Add("IsActive", typeof(bool));
            output.Columns.Add("InternalName", typeof(string));             

            if (udt != null)
            {
                foreach (var row in udt)
                {
                    if (row != null)
                    {
                        output.Rows.Add(
                        row.DocumentId,
                        row.CallDetailId,
                        row.DocumentTypeId,
                        row.DocumentName,
                        row.MimeType,
                        row.DocumentUrlPath,
                        row.IsActive,
                        row.InternalName
                        );
                    }                    
                }
            }
            return output;
        }

        private DataTable TableValuedParameters_Get_CardList(List<TicketAssignmentCardModel> udt)
        {
            var output = new DataTable();

            // Define columns for the DataTable based on the udt_T_TicketDetails type
            output.Columns.Add("ReplyToId", typeof(string));
            output.Columns.Add("ActivityId", typeof(string));
            output.Columns.Add("ConversationId", typeof(string));
            output.Columns.Add("ServiceUrl", typeof(string));
            output.Columns.Add("UserName", typeof(string));
            output.Columns.Add("UserADID", typeof(string));
            output.Columns.Add("Type", typeof(string));
            output.Columns.Add("CaseNumber", typeof(string));
            output.Columns.Add("AssignmentId", typeof(long));
            output.Columns.Add("AssignmentHistoryId", typeof(long));
            output.Columns.Add("CallDetailId", typeof(long));
            output.Columns.Add("Status", typeof(bool));
            if (udt != null)
            {
                foreach (var row in udt)
                {
                    if (row != null)
                    {
                        output.Rows.Add(
                            row.ReplyToId,
                            row.ActivityId,
                            row.ConversationId,
                            row.ServiceUrl,
                            row.UserName,
                            row.UserADID,
                            row.Type,
                            row.CaseNumber,
                            row.AssignmentId,
                            row.AssignmentHistoryId,
                            row.CallDetailId,
                            row.Status
                        );
                    }
                }
            }

            return output;
        }

        #endregion



        #endregion

        #region Azure OpenAI Sentiment Analysis DataAccess
        public async Task<ReturnMessageModel?> InsertAudioSummary(OpenAIModel data)
        {
            try
            {
                var result = await _db.SaveData<ReturnMessageModel, dynamic>(storedProcedure: "usp_SentimentAnalysis_Insert",
                new
                {
                    data.OutputModel?.SummaryText,
                    data.OutputModel?.Sentiment,
                    data.OutputModel?.Reason,
                    data.OutputModel?.TranscribeText,
                    data.FileOutputModel?.RefId,
                    data.FileOutputModel?.FileName,
                    data.FileOutputModel?.FileInternalName,
                    data.FileOutputModel?.FileUrl,
                    data.FileOutputModel?.ContentType
                });

                return result.FirstOrDefault();
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, $"DataAccess --> Insert --> SQL(usp_SentimentAnalysis_Insert) execution failed");
                ExceptionLogging.SendErrorToText(ex);
                return null;
            }
        }


        public async Task<List<GetSentimentAnalysisModel>?> GetAudioSummary(int? Id)
        {
            try
            {
                var results = await _db.LoadData<GetSentimentAnalysisModel, dynamic>("usp_SentimentAnalysis_Get",
                new
                {
                    Id
                });

                return results.ToList();
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, $"DataAccess --> GetAudioSummary() --> SQL(usp_SentimentAnalysis_Get) execution failed");
                return null;
            }
        }
        #endregion
    }
}
