using Newtonsoft.Json;

namespace NSSOperationAutomationApp.Models
{
    public class Settings
    {
        public string ConnectionStrings { get; set; }
    }

    public class AzureBlobSettings
    {
        public string StorageConnectionString { get; set; }
        public string ContainerName { get; set; }
    }
}
