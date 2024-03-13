using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.SqlServer.Server;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using NSSOperationAutomationApp.DataAccessHelper;
using NSSOperationAutomationApp.Models;
using NSSOperationAutomationApp.HelperMethods;
using Microsoft.Graph;
using NSSOperationAutomationApp.ServiceMethods;
using System.Net.Sockets;
using Azure;
using Microsoft.AspNetCore.Authorization;

namespace NSSOperationAutomationApp.Controllers
{
    [Route("api/ticket")]
    [ApiController]
    public class APIController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IDataAccess _dataAccess;
        private readonly IFileHelper _fileHelper;
        private readonly IUsersService _usersService;
        private readonly INotificationHelper _notificationHelper;

        public APIController(ILogger<APIController> logger, IDataAccess dataAccess, IFileHelper fileHelper, IUsersService usersService, INotificationHelper notificationHelper)
        {
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this._dataAccess = dataAccess ?? throw new ArgumentNullException(nameof(dataAccess));
            this._fileHelper = fileHelper ?? throw new ArgumentNullException(nameof(fileHelper));
            this._usersService = usersService ?? throw new ArgumentNullException(nameof(usersService));
            this._notificationHelper = notificationHelper ?? throw new ArgumentNullException(nameof(notificationHelper));
        }

        #region CREATE

        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> Create(TicketDetailsModel dataModel)
        {
            try
            {
                //DateTime startTime = DateTime.UtcNow;
                //ExceptionLogging.WriteMessageToText($"APIController --> Create() execution started: {DateTime.UtcNow}");
                //this._logger.LogInformation($"APIController --> Create() execution started: {DateTime.UtcNow}");

                var result = await _dataAccess.Insert(dataModel);                

                //DateTime endTime = DateTime.UtcNow;
                //ExceptionLogging.WriteMessageToText($"APIController --> Create() execution ended: {DateTime.UtcNow}");
                //this._logger.LogInformation($"APIController --> Create() execution ended: {DateTime.UtcNow}");

                //TimeSpan timeDifference = endTime - startTime;
                //string formattedTimeDifference = timeDifference.ToString(@"hh\:mm\:ss");

                //ExceptionLogging.WriteMessageToText($"APIController --> Create() execution time: {formattedTimeDifference}");
                //this._logger.LogInformation($"APIController --> Create() execution time: {formattedTimeDifference}");

                return this.Ok(result);

            }
            catch (Exception ex) 
            {
                this._logger.LogError(ex, $"APIController --> Create() execution failed");
                ExceptionLogging.SendErrorToText(ex);
                return this.Ok(ex.Message);
            }
        }


        [HttpPost]
        [Route("createFromCSVAsync")]
        [Authorize]
        public async Task<IActionResult> CreateTicketFromCSVAsync([FromForm] IFormCollection formdata)
        {
            try
            {
                string assignedByEmail = String.Empty;

                //DateTime startTime = DateTime.UtcNow;
                //ExceptionLogging.WriteMessageToText($"APIController --> Create() execution started: {DateTime.UtcNow}");
                //this._logger.LogInformation($"APIController --> Create() execution started: {DateTime.UtcNow}");

                var strKey = formdata.Keys.Where(x => x == "Email").FirstOrDefault();
                if (strKey != null)
                {
                    assignedByEmail = formdata[strKey].ToString();
                }

                if (formdata.Files != null && formdata.Files.Any())
                {
                    var (ticketDetailsList, message) = await _fileHelper.ReadCSVFileFromLocal(formdata.Files[0]);

                    if (!string.IsNullOrEmpty(message) && message.StartsWith("Some"))
                    {
                        return this.Ok(message);
                    }

                    else if (ticketDetailsList != null && ticketDetailsList.Any())
                    {
                        // INITIATE OTHER ACTIVITIES
                        _ = Task.Factory.StartNew(() => this.ProcessOtherActivities_CreateTicketInBulk(ticketDetailsList, assignedByEmail));

                        return this.Ok("File uploaded successfully & processed for ticket creation");
                    }
                }

                //DateTime endTime = DateTime.UtcNow;
                //ExceptionLogging.WriteMessageToText($"APIController --> Create() execution ended: {DateTime.UtcNow}");
                //this._logger.LogInformation($"APIController --> Create() execution ended: {DateTime.UtcNow}");

                //TimeSpan timeDifference = endTime - startTime;
                //string formattedTimeDifference = timeDifference.ToString(@"hh\:mm\:ss");

                //ExceptionLogging.WriteMessageToText($"APIController --> Create() execution time: {formattedTimeDifference}");
                //this._logger.LogInformation($"APIController --> Create() execution time: {formattedTimeDifference}");

                return this.Ok("Unexpected Error");

            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, $"APIController --> CreateTicketFromCSV() execution failed");
                ExceptionLogging.SendErrorToText(ex);
                return this.Ok(ex.Message);
            }
        }

        private async Task ProcessOtherActivities_CreateTicketInBulk(List<TicketCreateInBulkModel> ticketDetailsList, string assignedByEmail)
        {
            var userList = new List<string>();

            var distinctAssignedToList = ticketDetailsList.Select(item => item.AssignedToEmail).Distinct().ToList();

            if (distinctAssignedToList != null && distinctAssignedToList.Any())
            {
                distinctAssignedToList.Add(assignedByEmail);

                var TaskList_GetUserADDetails = distinctAssignedToList.Select(item => this._usersService.GetUserProfile(item)).ToList();

                if (TaskList_GetUserADDetails != null && TaskList_GetUserADDetails.Any())
                {
                    var adResultList = await Task.WhenAll(TaskList_GetUserADDetails);

                    if (adResultList != null && adResultList.Any())
                    {
                        var assignedByDetails = adResultList.FirstOrDefault(result => result.Mail == assignedByEmail);

                        foreach (var ticket in ticketDetailsList)
                        {
                            var assignedToDetails = adResultList.FirstOrDefault(result => result.Mail == ticket.AssignedToEmail);

                            ticket.AssignedTo = assignedToDetails?.DisplayName;
                            ticket.AssignedToADID = assignedToDetails?.Id;

                            ticket.AssignedBy = assignedByDetails?.DisplayName;
                            ticket.AssignedByEmail = assignedByDetails?.Mail;
                            ticket.AssignedByADID = assignedByDetails?.Id;
                        }

                        var nonNullOrEmptyADIDTickets = ticketDetailsList
                                                        .Where(ticket => !string.IsNullOrEmpty(ticket.AssignedToADID))
                                                        .ToList();

                        if (nonNullOrEmptyADIDTickets != null && nonNullOrEmptyADIDTickets.Any())
                        {
                            var insertResponse = await _dataAccess.InsertTicketInBulk(nonNullOrEmptyADIDTickets);

                            if (insertResponse != null && !string.IsNullOrEmpty(insertResponse.ReferenceObject))
                            {
                                try
                                {
                                    var ticketCardDetailsList = JsonConvert.DeserializeObject<List<TicketAssignmentCardModel>>(insertResponse.ReferenceObject);
                                    if (ticketCardDetailsList != null && ticketCardDetailsList.Any())
                                    {
                                        var cardResponseList = await this._notificationHelper.ProcessNotification_AssignReassignTicket(ticketCardDetailsList);

                                        if (cardResponseList != null && cardResponseList.Any())
                                        {
                                            await _dataAccess.Insert_AssignReassignTicket_CardResponse(cardResponseList);
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    this._logger.LogError(ex, $"APIController --> ProcessOtherActivities_AssignReassignEngineer() execution failed");
                                    ExceptionLogging.SendErrorToText(ex);
                                }
                            }
                        }
                    }
                }
            }
        }

        [HttpPost]
        [Route("createFromCSV")]
        [Authorize]
        public async Task<IActionResult> CreateTicketFromCSV([FromForm] IFormCollection formdata)
        {
            try
            {
                var outputObject = new TicketCreateInBulkReturnModel();

                string assignedByEmail = String.Empty;

                //DateTime startTime = DateTime.UtcNow;
                //ExceptionLogging.WriteMessageToText($"APIController --> Create() execution started: {DateTime.UtcNow}");
                //this._logger.LogInformation($"APIController --> Create() execution started: {DateTime.UtcNow}");

                var strKey = formdata.Keys.Where(x => x == "Email").FirstOrDefault();
                if (strKey != null)
                {
                    assignedByEmail = formdata[strKey].ToString();
                }

                if (string.IsNullOrEmpty(assignedByEmail))
                {
                    outputObject.Message = "Invalid assigner email id";

                    return this.Ok(outputObject);
                }

                if (formdata.Files != null && formdata.Files.Any())
                {
                    var (ticketDetailsList, message) = await _fileHelper.ReadCSVFileFromLocal(formdata.Files[0]);

                    if (!string.IsNullOrEmpty(message) && message.StartsWith("Some"))
                    {
                        outputObject.Message = message;

                        return this.Ok(outputObject);
                    }

                    else if (ticketDetailsList != null && ticketDetailsList.Any())
                    {
                        ticketDetailsList.Select(ticket => { ticket.AssignedByEmail = assignedByEmail; return ticket; }).ToList();

                        var insertResponse = await _dataAccess.InsertTicketInBulk(ticketDetailsList);

                        if (insertResponse != null && !string.IsNullOrEmpty(insertResponse.ReferenceObject))
                        {
                            var ticketCardDetailsList = JsonConvert.DeserializeObject<List<TicketAssignmentCardModel>>(insertResponse.ReferenceObject);

                            if (ticketCardDetailsList != null && ticketCardDetailsList.Any())
                            {
                                outputObject.Message = "File uploaded & ticket created successfully";

                                outputObject.TicketCreationResultList = ticketCardDetailsList;

                                var successTicketList = ticketCardDetailsList
                                                          .Where(ticket => ticket.InsertStatus?.Equals(true) ?? false)
                                                          .ToList();

                                if (successTicketList != null && successTicketList.Any())
                                {
                                    // INITIATE OTHER ACTIVITIES
                                    _ = Task.Factory.StartNew(() => this.ProcessOtherActivities_CreateTicketInBulkSendCards(successTicketList));
                                }                               

                                return this.Ok(outputObject);
                            }                            
                        }

                        outputObject.Message = "File uploaded successfully & processed for ticket creation";

                        return this.Ok(outputObject);
                    }
                }

                //DateTime endTime = DateTime.UtcNow;
                //ExceptionLogging.WriteMessageToText($"APIController --> Create() execution ended: {DateTime.UtcNow}");
                //this._logger.LogInformation($"APIController --> Create() execution ended: {DateTime.UtcNow}");

                //TimeSpan timeDifference = endTime - startTime;
                //string formattedTimeDifference = timeDifference.ToString(@"hh\:mm\:ss");

                //ExceptionLogging.WriteMessageToText($"APIController --> Create() execution time: {formattedTimeDifference}");
                //this._logger.LogInformation($"APIController --> Create() execution time: {formattedTimeDifference}");

                outputObject.Message = "Unexpected Error";

                return this.Ok(outputObject);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, $"APIController --> CreateTicketFromCSV() execution failed");
                ExceptionLogging.SendErrorToText(ex);
                return this.Ok(ex.Message);
            }
        }

        private async Task ProcessOtherActivities_CreateTicketInBulkSendCards(List<TicketAssignmentCardModel> ticketCardDetailsList)
        {
            try
            {
                if (ticketCardDetailsList != null && ticketCardDetailsList.Any())
                {
                    var cardResponseList = await this._notificationHelper.ProcessNotification_AssignReassignTicketInBulk(ticketCardDetailsList);

                    if (cardResponseList != null && cardResponseList.Any())
                    {
                        await _dataAccess.Insert_AssignReassignTicket_CardResponse(cardResponseList);
                    }
                }
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, $"APIController --> ProcessOtherActivities_CreateTicketInBulkSendCards() execution failed");
                ExceptionLogging.SendErrorToText(ex);
            }        
        }

        #endregion

        #region GET

        [HttpPost]
        [Route("get")]
        public async Task<IActionResult> Get(FilterModel dataModel)
        {
            try
            {
                //DateTime startTime = DateTime.UtcNow;
                //ExceptionLogging.WriteMessageToText($"APIController --> Get() execution started: {DateTime.UtcNow}");
                //this._logger.LogInformation($"APIController --> Get() execution started: {DateTime.UtcNow}");

                var result = await _dataAccess.Get(dataModel);

                //DateTime endTime = DateTime.UtcNow;
                //ExceptionLogging.WriteMessageToText($"APIController --> Get() execution ended: {DateTime.UtcNow}");
                //this._logger.LogInformation($"APIController --> Get() execution ended: {DateTime.UtcNow}");

                //TimeSpan timeDifference = endTime - startTime;
                //string formattedTimeDifference = timeDifference.ToString(@"hh\:mm\:ss");

                //ExceptionLogging.WriteMessageToText($"APIController --> Get() execution time: {formattedTimeDifference}");
                //this._logger.LogInformation($"APIController --> Get() execution time: {formattedTimeDifference}");

                return this.Ok(result);
            }
            catch (Exception ex) 
            {
                this._logger.LogError(ex, $"APIController --> Get() execution failed");
                ExceptionLogging.SendErrorToText(ex);
                return this.Ok(ex.Message);
            }
        }

        //Rev Ayon
        [HttpPost]
        [Route("getTicketsOverview")]
        [Authorize]
        public async Task<IActionResult> GetTicketsOverview(FilterModel dataModel)
        {
            try
            {
                var returnModel = new List<String>();

                //DateTime startTime = DateTime.UtcNow;
                //ExceptionLogging.WriteMessageToText($"APIController --> Get() execution started: {DateTime.UtcNow}");
                //this._logger.LogInformation($"APIController --> Get() execution started: {DateTime.UtcNow}");

                var result = await _dataAccess.GetTicketsOverview(dataModel);

                //DateTime endTime = DateTime.UtcNow;
                //ExceptionLogging.WriteMessageToText($"APIController --> Get() execution ended: {DateTime.UtcNow}");
                //this._logger.LogInformation($"APIController --> Get() execution ended: {DateTime.UtcNow}");

                //TimeSpan timeDifference = endTime - startTime;
                //string formattedTimeDifference = timeDifference.ToString(@"hh\:mm\:ss");

                //ExceptionLogging.WriteMessageToText($"APIController --> Get() execution time: {formattedTimeDifference}");
                //this._logger.LogInformation($"APIController --> Get() execution time: {formattedTimeDifference}");

                if (result != null)
                {
                    return this.Ok(result);
                }
                else
                {
                    return this.Ok(returnModel);
                }
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, $"APIController --> GetTicketsByCaseNo() execution failed");
                ExceptionLogging.SendErrorToText(ex);
                return this.Ok(ex.Message);
            }
        }

        //Rev Ayon
        [HttpPost]
        [Route("getTicketTimelineByCaseNo")]
        [Authorize]
        public async Task<IActionResult> GetTicketsTimelineByCaseNo(string caseNumber)
        {
            try
            {
                //DateTime startTime = DateTime.UtcNow;
                //ExceptionLogging.WriteMessageToText($"APIController --> Get() execution started: {DateTime.UtcNow}");
                //this._logger.LogInformation($"APIController --> Get() execution started: {DateTime.UtcNow}");

                var result = await _dataAccess.GetTicketTimelineByCaseNo(caseNumber);

                //DateTime endTime = DateTime.UtcNow;
                //ExceptionLogging.WriteMessageToText($"APIController --> Get() execution ended: {DateTime.UtcNow}");
                //this._logger.LogInformation($"APIController --> Get() execution ended: {DateTime.UtcNow}");

                //TimeSpan timeDifference = endTime - startTime;
                //string formattedTimeDifference = timeDifference.ToString(@"hh\:mm\:ss");

                //ExceptionLogging.WriteMessageToText($"APIController --> Get() execution time: {formattedTimeDifference}");
                //this._logger.LogInformation($"APIController --> Get() execution time: {formattedTimeDifference}");

                return this.Ok(result);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, $"APIController --> GetTicketTimelineByCaseNo() execution failed");
                ExceptionLogging.SendErrorToText(ex);
                return this.Ok(ex.Message);
            }
        }

        #endregion

        #region CLOSE BY ADMIN 

        [HttpPost]
        [Route("updateByAdmin")]
        [Authorize]
        public async Task<IActionResult> TicketUpdateByAdmin(TicketAssignmentModel dataModel)
        {
            try
            {
                //DateTime startTime = DateTime.UtcNow;
                //ExceptionLogging.WriteMessageToText($"APIController --> Create() execution started: {DateTime.UtcNow}");
                //this._logger.LogInformation($"APIController --> Create() execution started: {DateTime.UtcNow}");

                dataModel.TransactionType = "ADMIN-U";
                var result = await _dataAccess.TicketUpdateByAdmin(dataModel);

                if (dataModel.CallStatusId != null && dataModel.CallStatusId == 3 && result != null)
                {
                    // INITIATE OTHER ACTIVITIES
                    _ = Task.Factory.StartNew(() => this.ProcessOtherActivities_TicketActionByAdmin(result));
                }

                //DateTime endTime = DateTime.UtcNow;
                //ExceptionLogging.WriteMessageToText($"APIController --> Create() execution ended: {DateTime.UtcNow}");
                //this._logger.LogInformation($"APIController --> Create() execution ended: {DateTime.UtcNow}");

                //TimeSpan timeDifference = endTime - startTime;
                //string formattedTimeDifference = timeDifference.ToString(@"hh\:mm\:ss");

                //ExceptionLogging.WriteMessageToText($"APIController --> Create() execution time: {formattedTimeDifference}");
                //this._logger.LogInformation($"APIController --> Create() execution time: {formattedTimeDifference}");

                return this.Ok(result);

            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, $"APIController --> TicketUpdateByAdmin() execution failed");
                ExceptionLogging.SendErrorToText(ex);
                return this.Ok(ex.Message);
            }
        }

        private async Task ProcessOtherActivities_TicketActionByAdmin(ReturnMessageModel response)
        {
            if (!string.IsNullOrEmpty(response.Id) && !string.IsNullOrEmpty(response.ReferenceObject))
            {
                try
                {
                    var ticketDetailsList = JsonConvert.DeserializeObject<List<TicketActionCardModel>>(response.ReferenceObject);
                    if (ticketDetailsList != null && ticketDetailsList.Any())
                    {
                        var cardResponseList = await this._notificationHelper.ProcessNotification_TicketActionByAdmin(ticketDetailsList);

                        if (cardResponseList != null && cardResponseList.Any())
                        {
                            await _dataAccess.Insert_AssignReassignTicket_CardResponse(cardResponseList);
                        }
                    }
                }
                catch (Exception ex)
                {
                    this._logger.LogError(ex, $"APIController --> ProcessOtherActivities_TicketActionByAdmin() execution failed");
                    ExceptionLogging.SendErrorToText(ex);
                }
            }
        }

        #endregion

        #region ASSIGN OR REASSIGN ENGINEER / GET

        [HttpPost]
        [Route("getAssignedTickets")]
        [Authorize]
        public async Task<IActionResult> GetAssignedTickets(TicketAssignmentModel dataModel)
        {
            try
            {
                //DateTime startTime = DateTime.UtcNow;
                //ExceptionLogging.WriteMessageToText($"APIController --> Get() execution started: {DateTime.UtcNow}");
                //this._logger.LogInformation($"APIController --> Get() execution started: {DateTime.UtcNow}");

                var result = await _dataAccess.GetAssignedTickets(dataModel);

                //DateTime endTime = DateTime.UtcNow;
                //ExceptionLogging.WriteMessageToText($"APIController --> Get() execution ended: {DateTime.UtcNow}");
                //this._logger.LogInformation($"APIController --> Get() execution ended: {DateTime.UtcNow}");

                //TimeSpan timeDifference = endTime - startTime;
                //string formattedTimeDifference = timeDifference.ToString(@"hh\:mm\:ss");

                //ExceptionLogging.WriteMessageToText($"APIController --> Get() execution time: {formattedTimeDifference}");
                //this._logger.LogInformation($"APIController --> Get() execution time: {formattedTimeDifference}");

                return this.Ok(result);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, $"APIController --> GetAssignedTickets() execution failed");
                ExceptionLogging.SendErrorToText(ex);
                return this.Ok(ex.Message);
            }
        }

        //Rev Ayon
        [HttpPost]
        [Route("assignEngineer")]
        [Authorize]
        public async Task<IActionResult> AssignReassignEngineer(TicketAssignmentModel dataModel)
        {
            try
            {
                //DateTime startTime = DateTime.UtcNow;
                //ExceptionLogging.WriteMessageToText($"APIController --> Create() execution started: {DateTime.UtcNow}");
                //this._logger.LogInformation($"APIController --> Create() execution started: {DateTime.UtcNow}");

                var result = await _dataAccess.AssignReassignEngineer(dataModel);

                if (result != null)
                {
                    // INITIATE OTHER ACTIVITIES
                    _ = Task.Factory.StartNew(() => this.ProcessOtherActivities_AssignReassignEngineer(result));
                }

                //DateTime endTime = DateTime.UtcNow;
                //ExceptionLogging.WriteMessageToText($"APIController --> Create() execution ended: {DateTime.UtcNow}");
                //this._logger.LogInformation($"APIController --> Create() execution ended: {DateTime.UtcNow}");

                //TimeSpan timeDifference = endTime - startTime;
                //string formattedTimeDifference = timeDifference.ToString(@"hh\:mm\:ss");

                //ExceptionLogging.WriteMessageToText($"APIController --> Create() execution time: {formattedTimeDifference}");
                //this._logger.LogInformation($"APIController --> Create() execution time: {formattedTimeDifference}");

                return this.Ok(result);

            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, $"APIController --> AssignReassignEngineer() execution failed");
                ExceptionLogging.SendErrorToText(ex);
                return this.Ok(ex.Message);
            }
        }

        private async Task ProcessOtherActivities_AssignReassignEngineer(ReturnMessageModel response)
        {
            if (!string.IsNullOrEmpty(response.Id) && !string.IsNullOrEmpty(response.ReferenceObject))
            {
                try
                {
                    var ticketDetailsList = JsonConvert.DeserializeObject<List<TicketAssignmentCardModel>>(response.ReferenceObject);
                    if (ticketDetailsList != null && ticketDetailsList.Any())
                    {
                        var cardResponseList = await this._notificationHelper.ProcessNotification_AssignReassignTicket(ticketDetailsList);

                        if (cardResponseList != null && cardResponseList.Any())
                        {
                            await _dataAccess.Insert_AssignReassignTicket_CardResponse(cardResponseList);
                        }
                    }
                }
                catch (Exception ex) 
                {
                    this._logger.LogError(ex, $"APIController --> ProcessOtherActivities_AssignReassignEngineer() execution failed");
                    ExceptionLogging.SendErrorToText(ex);
                }
            }
        }

        #endregion

        #region ACTION ON ASSIGNED TICKET BY ENGINEER  

        [HttpPost]
        [Route("engineerAction")]
        [Authorize]
        public async Task<IActionResult> EngineerActionOnTicket([FromForm] IFormCollection formdata)
        {
            try
            {
                ReturnMessageModel? result = null;
                TicketActionModel? dataModel = null;
                //DateTime startTime = DateTime.UtcNow;
                //ExceptionLogging.WriteMessageToText($"APIController --> Create() execution started: {DateTime.UtcNow}");
                //this._logger.LogInformation($"APIController --> Create() execution started: {DateTime.UtcNow}");           

                var strKey = formdata.Keys.Where(x => x == "eventData").FirstOrDefault();
                if (strKey != null)
                {
                    var data = formdata[strKey];
                    dataModel = new TicketActionModel();
                    dataModel = JsonConvert.DeserializeObject<TicketActionModel>(JObject.Parse(data).ToString());
                }

                if (dataModel != null)
                {
                    if (dataModel.TransactionType == "CALL-I")
                    {
                        if (dataModel.CallDocumentList != null && dataModel.CallDocumentList.Any() 
                            && formdata.Files != null && formdata.Files.Any())
                        {
                            var fileResult = await _fileHelper.UploadFilesOnServer(dataModel.CaseNumber, dataModel.CallDocumentList, formdata.Files);
                            if (fileResult != null)
                            {
                                dataModel.CallDocumentList = fileResult;
                            }
                        }

                        result = await _dataAccess.EngineerActionOnTicket(dataModel);

                        if (result != null)
                        {
                            // INITIATE OTHER ACTIVITIES
                            _ = Task.Factory.StartNew(() => this.ProcessOtherActivities_TicketActionByEngineer(result));
                        }
                    }
                    else if (dataModel.TransactionType == "DOC-U" 
                        && dataModel.CallDocumentList != null 
                        && dataModel.CallDocumentList.Any())
                    {
                        if (formdata.Files != null && formdata.Files.Any())
                        {
                            var fileResult = await _fileHelper.UploadFilesOnServer(dataModel.CaseNumber, dataModel.CallDocumentList, formdata.Files);

                            if (fileResult != null)
                            {
                                dataModel.CallDocumentList = fileResult;
                            }                           
                        }

                        result = await _dataAccess.EngineerDocumentUpdateOnTicket(dataModel.CallDocumentList);
                    }
                }                

                //DateTime endTime = DateTime.UtcNow;
                //ExceptionLogging.WriteMessageToText($"APIController --> Create() execution ended: {DateTime.UtcNow}");
                //this._logger.LogInformation($"APIController --> Create() execution ended: {DateTime.UtcNow}");

                //TimeSpan timeDifference = endTime - startTime;
                //string formattedTimeDifference = timeDifference.ToString(@"hh\:mm\:ss");

                //ExceptionLogging.WriteMessageToText($"APIController --> Create() execution time: {formattedTimeDifference}");
                //this._logger.LogInformation($"APIController --> Create() execution time: {formattedTimeDifference}");

                return this.Ok(result);

            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, $"APIController --> EngineerActionOnTicket() execution failed");
                ExceptionLogging.SendErrorToText(ex);
                return this.Ok(ex.Message);
            }
        }

        private async Task ProcessOtherActivities_TicketActionByEngineer(ReturnMessageModel response)
        {
            if (!string.IsNullOrEmpty(response.Id) && !string.IsNullOrEmpty(response.ReferenceObject))
            {
                try
                {
                    var ticketDetailsList = JsonConvert.DeserializeObject<List<TicketActionCardModel>>(response.ReferenceObject);
                    if (ticketDetailsList != null && ticketDetailsList.Any())
                    {
                        var cardResponseList = await this._notificationHelper.ProcessNotification_TicketActionByEngineer(ticketDetailsList);

                        if (cardResponseList != null && cardResponseList.Any())
                        {
                            await _dataAccess.Insert_AssignReassignTicket_CardResponse(cardResponseList);
                        }
                    }
                }
                catch (Exception ex)
                {
                    this._logger.LogError(ex, $"APIController --> ProcessOtherActivities_TicketActionByEngineer() execution failed");
                    ExceptionLogging.SendErrorToText(ex);
                }
            }
        }

        #endregion
    }
}
