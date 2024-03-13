using Newtonsoft.Json;

namespace NSSOperationAutomationApp.Models
{
    public class MasterModels
    {
        public class CallActionModel
        {
            [JsonProperty("callActionId")]
            public int CallActionId { get; set; }

            [JsonProperty("callAction")]
            public string CallAction { get; set; }
        }

        public class CallStatusModel
        {
            [JsonProperty("callStatusId")]
            public int CallStatusId { get; set; }

            [JsonProperty("callStatus")]
            public string CallStatus { get; set; }
        }

        public class DocumentTypeModel
        {
            [JsonProperty("documentTypeId")]
            public int DocumentTypeId { get; set; }

            [JsonProperty("documentType")]
            public string DocumentType { get; set; }
        }

        public class PartConsumptionTypeModel
        {
            [JsonProperty("partConsumptionTypeId")]
            public int PartConsumptionTypeId { get; set; }

            [JsonProperty("partConsumptionType")]
            public string PartConsumptionType { get; set; }
        }
    }
}
