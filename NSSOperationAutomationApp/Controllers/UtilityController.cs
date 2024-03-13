using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.Graph;
using NSSOperationAutomationApp.DataAccessHelper;
using NSSOperationAutomationApp.HelperMethods;
using NSSOperationAutomationApp.Models;
using NSSOperationAutomationApp.ServiceMethods;
using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;

namespace NSSOperationAutomationApp.Controllers
{
    [Route("api")]
    [ApiController]
    [Authorize]
    public class UtilityController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IDataAccess _dataAccess;
        private readonly IConversationData _conversationData;
        private readonly IUsersService _usersService;
        private readonly IGroupsService _groupsService;
        private readonly int MaxUsers;
        private readonly string? AppBaseUrl;
        private readonly string? GroupIds;
        private readonly string? AdminEmailIds;

        private const string AdminAppName = "AdminApp";

        public UtilityController(ILogger<UtilityController> logger
            , IDataAccess dataAccess
            , IConversationData conversationData
            , IUsersService usersService
            , IGroupsService groupsService
            , IOptions<AppSettings> appOptions
            , IOptions<AdminAppSettings> adminOptions
            , IOptions<GroupAppSettings> groupOptions)
        {
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this._dataAccess = dataAccess ?? throw new ArgumentNullException(nameof(dataAccess));
            this._conversationData = conversationData ?? throw new ArgumentNullException(nameof(conversationData));
            this._usersService = usersService ?? throw new ArgumentNullException(nameof(usersService));
            this._groupsService = groupsService ?? throw new ArgumentNullException(nameof(groupsService));
            appOptions = appOptions ?? throw new ArgumentNullException(nameof(appOptions));
            adminOptions = adminOptions ?? throw new ArgumentNullException(nameof(adminOptions));
            groupOptions = groupOptions ?? throw new ArgumentNullException(nameof(groupOptions));

            if ((appOptions.Value.MaxUsers) > 0)
            {
                MaxUsers = appOptions.Value.MaxUsers;
            }
            else
            {
                MaxUsers = 99;
            }

            if (!string.IsNullOrEmpty(appOptions.Value.AppBaseUrl))
            {
                AppBaseUrl = appOptions.Value.AppBaseUrl;
            }

            if (!string.IsNullOrEmpty(groupOptions.Value.GroupIds))
            {
                GroupIds = groupOptions.Value.GroupIds;
            }

            if (!string.IsNullOrEmpty(adminOptions.Value.AdminEmailIds))
            {
                AdminEmailIds = adminOptions.Value.AdminEmailIds;
            }
        }

        #region GET USERS FROM APP SETTINGS

        [HttpGet]
        [Route("getAdminFromSettings")]
        public async Task<IActionResult> GetAdminFromSettings(string? filter)
        {
            try
            {
                var adminEmailIdList = new List<string>();
                var returnList = new List<ConversationModel>();

                //DateTime startTime = DateTime.UtcNow;
                //ExceptionLogging.WriteMessageToText($"APIController --> Get() execution started: {DateTime.UtcNow}");
                //this._logger.LogInformation($"APIController --> Get() execution started: {DateTime.UtcNow}");

                if (string.IsNullOrEmpty(this.AdminEmailIds))
                {
                    return this.Ok("No email id found in app-settings!");
                }

                adminEmailIdList = this.AdminEmailIds.Split(",").ToList();

                if (adminEmailIdList.Any())
                {
                    if (!string.IsNullOrEmpty(filter))
                    {
                        var matchingUsers = adminEmailIdList.Where(item =>
                        {
                            return item.Equals(filter, StringComparison.OrdinalIgnoreCase);
                        });

                        if (matchingUsers != null && matchingUsers.Any())
                        {
                            var result = await _conversationData.GetConversationByUserEmail(userEmail: filter, appName: AdminAppName);
                            returnList.Add(result);
                            return this.Ok(returnList);
                        }
                    }
                    else
                    {
                        foreach (var item in adminEmailIdList)
                        {
                            var result = await _conversationData.GetConversationByUserEmail(userEmail: item, appName: AdminAppName);
                            if (result != null && !string.IsNullOrEmpty(result.UserEmail))
                            {
                                returnList.Add(result);
                            }
                        }

                        return this.Ok(returnList);
                    }

                    return this.Ok(returnList);
                }               

                //DateTime endTime = DateTime.UtcNow;
                //ExceptionLogging.WriteMessageToText($"APIController --> Get() execution ended: {DateTime.UtcNow}");
                //this._logger.LogInformation($"APIController --> Get() execution ended: {DateTime.UtcNow}");

                //TimeSpan timeDifference = endTime - startTime;
                //string formattedTimeDifference = timeDifference.ToString(@"hh\:mm\:ss");

                //ExceptionLogging.WriteMessageToText($"APIController --> Get() execution time: {formattedTimeDifference}");
                //this._logger.LogInformation($"APIController --> Get() execution time: {formattedTimeDifference}");

                return this.Ok(returnList);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, $"UtilityController --> GetAdminFromSettings() execution failed");
                ExceptionLogging.SendErrorToText(ex);
                return this.Ok(ex.Message);
            }
        }

        #endregion

        #region GET USERS FROM DB

        [HttpGet]
        [Route("getAppInstalledUsers")]
        public async Task<IActionResult> GetAllUsersFromConversationData(string? filter, string? appName)
        {
            try
            {
                //DateTime startTime = DateTime.UtcNow;
                //ExceptionLogging.WriteMessageToText($"APIController --> Get() execution started: {DateTime.UtcNow}");
                //this._logger.LogInformation($"APIController --> Get() execution started: {DateTime.UtcNow}");

                if (string.IsNullOrEmpty(appName))
                {
                    appName = "UserApp";
                }

                var result = await _conversationData.GetAllPersonalConversations(filter, appName);

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
                this._logger.LogError(ex, $"UtilityController --> GetAllUsersFromConversationData() execution failed");
                ExceptionLogging.SendErrorToText(ex);
                return this.Ok(ex.Message);
            }
        }

        #endregion

        #region AD USERS

        [HttpGet("getMyProfile")]
        public async Task<IActionResult> GetMyProfile()
        {
            try
            {
                var user = await this._usersService.GetMyProfile();
                return this.Ok(user);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, $"UtilityController --> GetMyProfile() execution failed");
                ExceptionLogging.SendErrorToText(ex);
                return this.Ok(ex.Message);
            }
        }

        [HttpGet("getUserProfile")]
        public async Task<IActionResult> GetUserProfile([Required] string Id)
        {
            try
            {
                var user = await this._usersService.GetUserProfile(Id);
                if (user != null)
                {
                    var userProfile = new UsersModel();
                    userProfile.Name = user.DisplayName;
                    userProfile.Email = user.Mail;
                    userProfile.UPN = user.UserPrincipalName;
                    userProfile.ADID = user.Id;
                    userProfile.Department = user.Department;
                    userProfile.Designation = user.JobTitle;
                    userProfile.OfficeLocation = user.OfficeLocation;

                    var resultPhoto = string.Empty;

                    try
                    {
                        resultPhoto = await this._usersService.GetUserProfilePhoto(user.Id);
                    }
                    catch (Exception ex)
                    {
                        this._logger.LogError(ex, "Error occurred while getting user's profile photo for id - " + user.Id);
                    }

                    userProfile.ProfilePhoto = (string.IsNullOrEmpty(resultPhoto)) ? AppBaseUrl + "/images/userImage.png" : resultPhoto;

                    try
                    {
                        var managerDetails = await this._usersService.GetUserManager(user.Id);

                        if (managerDetails != null)
                        {
                            userProfile.ManagerName = managerDetails.DisplayName;
                            userProfile.ManagerEmail = managerDetails.Mail;
                            userProfile.ManagerUPN = managerDetails.UserPrincipalName;
                            userProfile.ManagerADID = managerDetails.Id;
                        }
                    }
                    catch (Exception ex)
                    {
                        this._logger.LogError(ex, "Error occurred while getting user's manager for id - " + user.Id);
                    }

                    return this.Ok(userProfile);
                }

                return this.Ok(null);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, $"UtilityController --> GetUserProfile() execution failed");
                ExceptionLogging.SendErrorToText(ex);
                return this.Ok(ex.Message);
            }
        }

        [HttpGet("getADUsers")]
        public async Task<IActionResult> GetFilteredUsersFromAD([Required] string filter, string? userType)
        {
            try
            {
                List<UsersModel> userList = new List<UsersModel>();

                if (string.IsNullOrEmpty(userType))
                {
                    userType = "Member";
                }

                var resultList = await this._usersService.GetFilteredUsers(MaxUsers, filter, userType);

                if (resultList != null && resultList.Any())
                {
                    foreach (var user in resultList)
                    {
                        var userProfile = new UsersModel();
                        userProfile.Name = user.DisplayName;
                        userProfile.Email = user.Mail;
                        userProfile.UPN = user.UserPrincipalName;
                        userProfile.ADID = user.Id;
                        userProfile.Department = user.Department;
                        userProfile.Designation = user.JobTitle;
                        userProfile.OfficeLocation = user.OfficeLocation;

                        var resultPhoto = string.Empty;

                        try
                        {
                            resultPhoto = await this._usersService.GetUserProfilePhoto(user.Id);
                        }
                        catch (Exception ex)
                        {
                            this._logger.LogError(ex, "Error occurred while getting user's profile photo for id - " + user.Id);
                        }

                        userProfile.ProfilePhoto = (string.IsNullOrEmpty(resultPhoto)) ? AppBaseUrl + "/images/userImage.png" : resultPhoto;

                        userList.Add(userProfile);
                    }
                }

                return this.Ok(userList);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, $"UtilityController --> GetFilteredUsersFromAD() execution failed");
                ExceptionLogging.SendErrorToText(ex);
                return this.Ok(ex.Message);
            }
        }

        [HttpGet("getGroupUsers")]
        public async Task<IActionResult> GetFilteredGroupUsersFromAD(string? filter, string? userType)
        {
            try
            {
                List<UsersModel> userList = new List<UsersModel>();

                if (string.IsNullOrEmpty(userType))
                {
                    userType = "Member";
                }

                if (!string.IsNullOrEmpty(this.GroupIds))
                {
                    var groupIdList = (this.GroupIds).Split(",");

                    if (groupIdList != null && groupIdList.Any())
                    {
                        var groupTaskList = new List<Task<IEnumerable<User>>>();
                        foreach (var g in groupIdList)
                        {
                            if (!string.IsNullOrEmpty(g))
                            {
                                groupTaskList.Add(_groupsService.GetGroupMembers(g));
                            }                            
                        }

                        if (groupTaskList != null && groupTaskList.Any())
                        {
                            var groupList = await Task.WhenAll(groupTaskList);
                            if (groupList != null && groupList.Any())
                            {
                                foreach (var groupMembers in groupList) // Iterate through each group's members
                                {
                                    if (groupMembers != null && groupMembers.Any())
                                    {
                                        foreach (var member in groupMembers) // Iterate through each member within the group
                                        {
                                            var u = new UsersModel();
                                            u.ADID = member.Id;
                                            u.UPN = member.UserPrincipalName;
                                            u.Email = member.Mail;
                                            u.Name = member.DisplayName;

                                            userList.Add(u);
                                        }
                                    }                                    
                                }

                                if (userList != null && userList.Any())
                                {
                                    var uniqueUsers = userList.DistinctBy(user => user.ADID).ToList();

                                    if (uniqueUsers != null && uniqueUsers.Any())
                                    {
                                        var userTaskList = new List<Task<User>>();
                                        foreach (var u in uniqueUsers)
                                        {
                                            if (!string.IsNullOrEmpty(u.ADID))
                                            {
                                                userTaskList.Add(_usersService.GetUserProfile(u.ADID));
                                            }
                                        }

                                        if (userTaskList != null && userTaskList.Any())
                                        {
                                            var adUserList = await Task.WhenAll(userTaskList);
                                            if (adUserList != null && adUserList.Any())
                                            {
                                                foreach(var user in adUserList)
                                                {
                                                    var objectToUpdate = uniqueUsers.FirstOrDefault(f => f.ADID == user.Id);

                                                    if (objectToUpdate != null)
                                                    {
                                                        objectToUpdate.Email = user.Mail;
                                                        objectToUpdate.Department = user.Department;
                                                        objectToUpdate.Designation = user.JobTitle;
                                                        objectToUpdate.OfficeLocation = user.OfficeLocation;
                                                    }
                                                }
                                            }
                                        }

                                        if (!string.IsNullOrEmpty(filter))
                                        {
                                            var matchingUsers = uniqueUsers.Where(user =>
                                            {
                                                if (!string.IsNullOrEmpty(user.Email))
                                                {
                                                    return user.Email.Equals(filter, StringComparison.OrdinalIgnoreCase) ||
                                                            user.Name.Contains(filter, StringComparison.OrdinalIgnoreCase);
                                                }
                                                else
                                                {
                                                    return user.Name.Contains(filter, StringComparison.OrdinalIgnoreCase);
                                                }
                                            });

                                            if (matchingUsers.Any())
                                            {
                                                return this.Ok(matchingUsers.Take(MaxUsers));
                                            }

                                            return this.Ok(matchingUsers);
                                        }
                                        else
                                        {
                                            return this.Ok(uniqueUsers.Take(MaxUsers));
                                        }
                                    }                                                                      
                                }
                            }
                        }
                    }
                }

                return this.Ok(userList);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, $"UtilityController --> GetFilteredUsersFromAD() execution failed");
                ExceptionLogging.SendErrorToText(ex);
                return this.Ok(ex.Message);
            }
        }

        #endregion
    }
}
