using Newtonsoft.Json;

namespace NSSOperationAutomationApp.Models
{
    public class TicketActionModel : CallDetailsModel
    {
        [JsonProperty("transactionType")]
        public string TransactionType { get; set; }

        [JsonProperty("callStatusId")]
        public int? CallStatusId { get; set; }
    }

    public class CallDetailsModel
    {
        [JsonProperty("callDetailId")]
        public long CallDetailId { get; set; }

        [JsonProperty("assignmentId")]
        public long? AssignmentId { get; set; }

        [JsonProperty("updatedBy")]
        public string UpdatedBy { get; set; }

        [JsonProperty("updatedByEmail")]
        public string UpdatedByEmail { get; set; }

        [JsonProperty("updatedByADID")]
        public string UpdatedByADID { get; set; }

        [JsonProperty("updatedOnUTC")]
        public string UpdatedOnUTC { get; set; }

        [JsonProperty("callActionId")]
        public int? CallActionId { get; set; }

        [JsonProperty("callAction")]
        public string CallAction { get; set; }

        [JsonProperty("customerName")]
        public string CustomerName { get; set; }

        [JsonProperty("caseNumber")]
        public string CaseNumber { get; set; }

        [JsonProperty("unitSlNo")]
        public string UnitSlNo { get; set; }

        [JsonProperty("passId")]
        public string PassId { get; set; }

        [JsonProperty("taskStartDateTimeIST")]
        public DateTime? TaskStartDateTimeIST { get; set; }

        [JsonProperty("taskEndDateTimeIST")]
        public DateTime? TaskEndDateTimeIST { get; set; }

        [JsonProperty("closerRemarks")]
        public string CloserRemarks { get; set; }

        [JsonProperty("partConsumptionTypeId")]
        public int? PartConsumptionTypeId { get; set; }

        [JsonProperty("partConsumptionType")]
        public string PartConsumptionType { get; set; }

        [JsonProperty("soNo")]
        public string SONo { get; set; }

        [JsonProperty("requiredPart")]
        public string RequiredPart { get; set; }

        [JsonProperty("requiredSparePartNo")]
        public string RequiredSparePartNo { get; set; }

        [JsonProperty("requiredPartName")]
        public string RequiredPartName { get; set; }

        [JsonProperty("faultyPartCTNo")]
        public string FaultyPartCTNo { get; set; }

        [JsonProperty("failureId")]
        public string FailureId { get; set; }

        [JsonProperty("issueDescription")]
        public string IssueDescription { get; set; }

        [JsonProperty("troubleshootingStep")]
        public string TroubleshootingStep { get; set; }

        [JsonProperty("firstPartConsumptionTypeId")]
        public int? FirstPartConsumptionTypeId { get; set; }

        [JsonProperty("firstPartConsumptionType")]
        public string FirstPartConsumptionType { get; set; }

        [JsonProperty("firstPartSONo")]
        public string FirstPartSONo { get; set; }

        [JsonProperty("firstRequiredPartName")]
        public string FirstRequiredPartName { get; set; }

        [JsonProperty("receivedPartConsumptionTypeId")]
        public int? ReceivedPartConsumptionTypeId { get; set; }

        [JsonProperty("receivedPartConsumptionType")]
        public string ReceivedPartConsumptionType { get; set; }

        [JsonProperty("receivedPartName")]
        public string ReceivedPartName { get; set; }

        [JsonProperty("callDocumentList")]
        public List<CallDocumentsModel>? CallDocumentList { get; set; }
    }

    public class CallDocumentsModel
    {
        [JsonProperty("documentId")]
        public long DocumentId { get; set; }

        [JsonProperty("callDetailId")]
        public long? CallDetailId { get; set; }

        [JsonProperty("documentTypeId")]
        public int? DocumentTypeId { get; set; }

        [JsonProperty("documentType")]
        public string DocumentType { get; set; }

        [JsonProperty("documentName")]
        public string DocumentName { get; set; }

        [JsonProperty("mimeType")]
        public string MimeType { get; set; }

        [JsonProperty("documentUrlPath")]
        public string DocumentUrlPath { get; set; }

        [JsonProperty("isActive")]
        public bool? IsActive { get; set; }

        [JsonProperty("internalName")]
        public string InternalName { get; set; }

        [JsonProperty("createdOnUTC")]
        public string CreatedOnUCT { get; set; }

        [JsonProperty("updatedOnUTC")]
        public string UpdatedOnUTC { get; set; }
    }
}
