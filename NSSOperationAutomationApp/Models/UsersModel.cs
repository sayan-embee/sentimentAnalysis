using Newtonsoft.Json;

namespace NSSOperationAutomationApp.Models
{
    public class UsersModel
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("upn")]
        public string UPN { get; set; }

        [JsonProperty("adid")]
        public string ADID { get; set; }

        [JsonProperty("department")]
        public string Department { get; set; }

        [JsonProperty("designation")]
        public string Designation { get; set; }

        [JsonProperty("officeLocation")]
        public string OfficeLocation { get; set; }

        [JsonProperty("profilePhoto")]
        public string ProfilePhoto { get; set; }

        [JsonProperty("managerName")]
        public string ManagerName { get; set; }

        [JsonProperty("managerEmail")]
        public string ManagerEmail { get; set; }

        [JsonProperty("managerUPN")]
        public string ManagerUPN { get; set; }

        [JsonProperty("managerADID")]
        public string ManagerADID { get; set; }
    }
}
