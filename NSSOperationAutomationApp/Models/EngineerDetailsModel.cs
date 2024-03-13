using Newtonsoft.Json;

namespace NSSOperationAutomationApp.Models
{
    public class TicketAssignmentModel : EngineerDetailsModel
    {
        [JsonProperty("transactionType")]
        public string TransactionType { get; set; }

        [JsonProperty("ticketId")]
        public long? TicketId { get; set; }

        [JsonProperty("fromDate")]
        public string? FromDate { get; set; }

        [JsonProperty("toDate")]
        public string? ToDate { get; set; }

        [JsonProperty("serialNumber")]
        public string? SerialNumber { get; set; }

        [JsonProperty("partNumber")]
        public string? PartNumber { get; set; }
    }

    public class EngineerDetailsModel
    {
        [JsonProperty("assignmentId")]
        public long? AssignmentId { get; set; }

        [JsonProperty("caseNumber")]
        public string? CaseNumber { get; set; }

        [JsonProperty("ticketId")]
        public long? TicketId { get; set; }

        [JsonProperty("assignedTo")]
        public string? AssignedTo { get; set; }

        [JsonProperty("assignedToEmail")]
        public string? AssignedToEmail { get; set; }

        [JsonProperty("assignedToADID")]
        public string? AssignedToADID { get; set; }

        [JsonProperty("assignedOnUTC")]
        public string? AssignedOnUTC { get; set; }

        [JsonProperty("assignedBy")]
        public string? AssignedBy { get; set; }

        [JsonProperty("assignedByEmail")]
        public string? AssignedByEmail { get; set; }

        [JsonProperty("assignedByADID")]
        public string? AssignedByADID { get; set; }

        [JsonProperty("callActionId")]
        public int? CallActionId { get; set; }

        [JsonProperty("callAction")]
        public string? CallAction { get; set; }

        [JsonProperty("callStatusId")]
        public int? CallStatusId { get; set; }

        [JsonProperty("callStatus")]
        public string? CallStatus { get; set; }

        [JsonProperty("closedBy")]
        public string? ClosedBy { get; set; }

        [JsonProperty("closedByEmail")]
        public string? ClosedByEmail { get; set; }

        [JsonProperty("closedOnUTC")]
        public string? ClosedOnUTC { get; set; }

        [JsonProperty("adminClosureRemarks")]
        public string? AdminClosureRemarks { get; set; }

        [JsonProperty("closedByADID")]
        public string? ClosedByADID { get; set; }

        [JsonProperty("createdOnUTC")]
        public string? CreatedOnUTC { get; set; }

        [JsonProperty("engineerHistoryDetails")]
        public List<EngineerHistoryDetailsModel>? EngineerHistoryDetails { get; set; }
    }

    public class EngineerHistoryDetailsModel
    {
        [JsonProperty("assignmentHistoryId")]
        public long? AssignmentHistoryId { get; set; }

        [JsonProperty("assignmentId")]
        public long? AssignmentId { get; set; }

        [JsonProperty("assignedTo")]
        public string? AssignedTo { get; set; }

        [JsonProperty("assignedToEmail")]
        public string? AssignedToEmail { get; set; }

        [JsonProperty("assignedToADID")]
        public string? AssignedToADID { get; set; }

        [JsonProperty("assignedOnUTC")]
        public string? AssignedOnUTC { get; set; }

        [JsonProperty("assignedBy")]
        public string? AssignedBy { get; set; }

        [JsonProperty("assignedByEmail")]
        public string? AssignedByEmail { get; set; }

        [JsonProperty("assignedByADID")]
        public string? AssignedByADID { get; set; }

        [JsonProperty("callActionId")]
        public int? CallActionId { get; set; }

        [JsonProperty("callAction")]
        public string? CallAction { get; set; }

        [JsonProperty("callStatusId")]
        public int? CallStatusId { get; set; }

        [JsonProperty("callStatus")]
        public string? CallStatus { get; set; }

        [JsonProperty("closedBy")]
        public string? ClosedBy { get; set; }

        [JsonProperty("closedByEmail")]
        public string? ClosedByEmail { get; set; }

        [JsonProperty("closedByADID")]
        public string? ClosedByADID { get; set; }

        [JsonProperty("closedOnUTC")]
        public string? ClosedOnUTC { get; set; }

        [JsonProperty("adminClosureRemarks")]
        public string? AdminClosureRemarks { get; set; }

        [JsonProperty("createdOnUTC")]
        public string? CreatedOnUTC { get; set; }
    }
}
