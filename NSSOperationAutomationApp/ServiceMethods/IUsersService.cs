using Microsoft.Graph;

namespace NSSOperationAutomationApp.ServiceMethods
{
    public interface IUsersService
    {
        Task<User> GetMyProfile();
        Task<string> GetUserProfilePhoto(string userId);
        Task<User> GetUserProfile(string userId);
        Task<User> GetUserManager(string userId);
        Task<IEnumerable<User>> GetFilteredUsers(int maxUser, string? filter, string userType);
    }
}