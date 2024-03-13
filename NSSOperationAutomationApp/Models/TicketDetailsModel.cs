using Microsoft.AspNetCore.Components.Routing;
using Newtonsoft.Json;

namespace NSSOperationAutomationApp.Models
{
    public static class TicketType
    {
        public static string Type1 { get; set; } = "HP";
        public static string Type2 { get; set; } = "ITSM";
    }

    public class TicketCreateInBulkModel
    {
        [JsonProperty("caseNumber")]
        public string CaseNumber { get; set; }

        [JsonProperty("ticketId")]
        public long? TicketId { get; set; }

        [JsonProperty("caseSubject")]
        public string CaseSubject { get; set; }

        [JsonProperty("contactName")]
        public string ContactName { get; set; }

        [JsonProperty("contactEmail")]
        public string ContactEmail { get; set; }

        [JsonProperty("serialNumber")]
        public string SerialNumber { get; set; }

        [JsonProperty("productName")]
        public string ProductName { get; set; }

        [JsonProperty("productNumber")]
        public string ProductNumber { get; set; }

        [JsonProperty("createdOn")]
        public string? CreatedOn { get; set; }

        [JsonProperty("ticketType")]
        public string? TicketType { get; set; }

        [JsonProperty("assignedTo")]
        public string? AssignedTo { get; set; }

        [JsonProperty("assignedToEmail")]
        public string? AssignedToEmail { get; set; }

        [JsonProperty("assignedToADID")]
        public string? AssignedToADID { get; set; }

        [JsonProperty("assignedBy")]
        public string? AssignedBy { get; set; }

        [JsonProperty("assignedByEmail")]
        public string? AssignedByEmail { get; set; }

        [JsonProperty("assignedByADID")]
        public string? AssignedByADID { get; set; }
    }

    public class TicketCreateInBulkReturnModel
    {
        [JsonProperty("message")]
        public string? Message { get; set; }

        [JsonProperty("ticketCreationResultList")]
        public List<TicketAssignmentCardModel>? TicketCreationResultList { get; set; }
    }

    public class TicketDetailsModel
    {
        [JsonProperty("caseNumber")]
        public string CaseNumber { get; set; }

        [JsonProperty("ticketId")]
        public long? TicketId { get; set; }

        [JsonProperty("fromEmail")]
        public string FromEmail { get; set; }

        [JsonProperty("toEmail")]
        public string ToEmail { get; set; }

        [JsonProperty("caseSubject")]
        public string CaseSubject { get; set; }

        [JsonProperty("workOrderNumber")]
        public string WorkOrderNumber { get; set; }

        [JsonProperty("serviceAccount")]
        public string ServiceAccount { get; set; }

        [JsonProperty("contactName")]
        public string ContactName { get; set; }

        [JsonProperty("contactEmail")]
        public string ContactEmail { get; set; }

        [JsonProperty("contactPhone")]
        public string ContactPhone { get; set; }

        [JsonProperty("serviceDeliveryStreetAddress")]
        public string ServiceDeliveryStreetAddress { get; set; }

        [JsonProperty("serviceDeliveryCity")]
        public string ServiceDeliveryCity { get; set; }

        [JsonProperty("serviceDeliveryCountry")]
        public string ServiceDeliveryCountry { get; set; }

        [JsonProperty("postalCode")]
        public string PostalCode { get; set; }

        [JsonProperty("serialNumber")]
        public string SerialNumber { get; set; }

        [JsonProperty("productName")]
        public string ProductName { get; set; }

        [JsonProperty("productNumber")]
        public string ProductNumber { get; set; }

        [JsonProperty("otcCode")]
        public string OTCCode { get; set; }

        [JsonProperty("partNumber")]
        public string? PartNumber { get; set; }

        [JsonProperty("partDescription")]
        public string? PartDescription { get; set; }

        [JsonProperty("emailSubject")]
        public string EmailSubject { get; set; }

        [JsonProperty("emailDate")]
        public string EmailDate { get; set; }

        [JsonProperty("createdOn")]
        public string? CreatedOn { get; set; }

        [JsonProperty("partNumber2")]
        public string? PartNumber2 { get; set; }

        [JsonProperty("partDescription2")]
        public string? PartDescription2 { get; set; }

        [JsonProperty("partNumber3")]
        public string? PartNumber3 { get; set; }

        [JsonProperty("partDescription3")]
        public string? PartDescription3 { get; set; }

        [JsonProperty("ticketType")]
        public string? TicketType { get; set; }

        [JsonProperty("roasterEngineer")]
        public string? RoasterEngineer { get; set; }
    }

    public class TicketDetailsOverviewModel
    {
        [JsonProperty("caseNumber")]
        public string CaseNumber { get; set; }

        [JsonProperty("ticketId")]
        public long? TicketId { get; set; }

        [JsonProperty("fromEmail")]
        public string FromEmail { get; set; }

        [JsonProperty("toEmail")]
        public string ToEmail { get; set; }

        [JsonProperty("caseSubject")]
        public string CaseSubject { get; set; }

        [JsonProperty("workOrderNumber")]
        public string WorkOrderNumber { get; set; }

        [JsonProperty("serviceAccount")]
        public string ServiceAccount { get; set; }

        [JsonProperty("contactName")]
        public string ContactName { get; set; }

        [JsonProperty("contactEmail")]
        public string ContactEmail { get; set; }

        [JsonProperty("contactPhone")]
        public string ContactPhone { get; set; }

        [JsonProperty("serviceDeliveryStreetAddress")]
        public string ServiceDeliveryStreetAddress { get; set; }

        [JsonProperty("serviceDeliveryCity")]
        public string ServiceDeliveryCity { get; set; }

        [JsonProperty("serviceDeliveryCountry")]
        public string ServiceDeliveryCountry { get; set; }

        [JsonProperty("postalCode")]
        public string PostalCode { get; set; }

        [JsonProperty("serialNumber")]
        public string SerialNumber { get; set; }

        [JsonProperty("productName")]
        public string ProductName { get; set; }

        [JsonProperty("productNumber")]
        public string ProductNumber { get; set; }

        [JsonProperty("otcCode")]
        public string OTCCode { get; set; }

        [JsonProperty("partNumber")]
        public string? PartNumber { get; set; }

        [JsonProperty("partDescription")]
        public string? PartDescription { get; set; }

        [JsonProperty("emailSubject")]
        public string EmailSubject { get; set; }

        [JsonProperty("emailDate")]
        public string EmailDate { get; set; }

        [JsonProperty("createdOn")]
        public string? CreatedOn { get; set; }

        [JsonProperty("partNumber2")]
        public string? PartNumber2 { get; set; }

        [JsonProperty("partDescription2")]
        public string? PartDescription2 { get; set; }

        [JsonProperty("partNumber3")]
        public string? PartNumber3 { get; set; }

        [JsonProperty("partDescription3")]
        public string? PartDescription3 { get; set; }

        [JsonProperty("assignmentId")]
        public long? AssignmentId { get; set; }

        [JsonProperty("assignedTo")]
        public string AssignedTo { get; set; }

        [JsonProperty("assignedBy")]
        public string AssignedBy { get; set; }

        [JsonProperty("callStatus")]
        public string CallStatus { get; set; }

        [JsonProperty("callAction")]
        public string CallAction { get; set; }

        [JsonProperty("engineerDetails")]
        public EngineerDetailsModel? EngineerDetails { get; set; }

        [JsonProperty("callDetailsList")]
        public List<CallDetailsModel>? CallDetailsList { get; set; }

        [JsonProperty("ticketTimeline")]
        public TicketTimelineViewModel? TicketTimeline { get; set; }
    }

    public class TicketTimelineModel
    {
        [JsonProperty("slNo")]
        public int SlNo { get; set; }

        [JsonProperty("caseNumber")]
        public string CaseNumber { get; set; }

        [JsonProperty("ticketId")]
        public long? TicketId { get; set; }

        [JsonProperty("eventTimeIST")]
        public string EventTimeIST { get; set; }

        [JsonProperty("eventType")]
        public string EventType { get; set; }

        [JsonProperty("assignmentId")]
        public int? AssignmentId { get; set; }

        [JsonProperty("callDetailId")]
        public int? CallDetailId { get; set; }

        [JsonProperty("callAction")]
        public string CallAction { get; set; }

        [JsonProperty("assignmentHistoryId")]
        public int? AssignmentHistoryId { get; set; }

        [JsonProperty("assignedTo")]
        public string AssignedTo { get; set; }

        [JsonProperty("assignedToEmail")]
        public string AssignedToEmail { get; set; }

        [JsonProperty("assignedBy")]
        public string AssignedBy { get; set; }

        [JsonProperty("assignedByEmail")]
        public string AssignedByEmail { get; set; }

        [JsonProperty("updatedBy")]
        public string UpdatedBy { get; set; }

        [JsonProperty("updatedByEmail")]
        public string UpdatedByEmail { get; set; }

        [JsonProperty("customerName")]
        public string CustomerName { get; set; }

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
        public int? FailureId { get; set; }

        [JsonProperty("issueDescription")]
        public string IssueDescription { get; set; }

        [JsonProperty("troubleshootingStep")]
        public string TroubleshootingStep { get; set; }

        [JsonProperty("firstPartConsumptionType")]
        public string FirstPartConsumptionType { get; set; }

        [JsonProperty("firstPartSONo")]
        public string FirstPartSONo { get; set; }

        [JsonProperty("firstRequiredPartName")]
        public string FirstRequiredPartName { get; set; }

        [JsonProperty("receivedPartConsumptionType")]
        public string ReceivedPartConsumptionType { get; set; }

        [JsonProperty("receivedPartName")]
        public string ReceivedPartName { get; set; }

        [JsonProperty("adminClosureRemarks")]
        public string AdminClosureRemarks { get; set; }

        [JsonProperty("closedBy")]
        public string ClosedBy { get; set; }

        [JsonProperty("closedByEmail")]
        public string ClosedByEmail { get; set; }
    }

    public class TicketTimelineViewModel
    {
        [JsonProperty("ticketTimelineList")]
        public List<TicketTimelineModel>? TicketTimelineList { get; set; }

        [JsonProperty("callDetailsList")]
        public List<CallDocumentsModel>? CallDocumentList { get; set; }
    }

    public class FilterModel
    {
        [JsonProperty("caseNumber")]
        public string? CaseNumber { get; set; }

        [JsonProperty("ticketId")]
        public long? TicketId { get; set; }

        [JsonProperty("serialNumber")]
        public string? SerialNumber { get; set; }

        [JsonProperty("partNumber")]
        public string? PartNumber { get; set; }

        [JsonProperty("fromDate")]
        public string? FromDate { get; set; }

        [JsonProperty("toDate")]
        public string? ToDate { get; set; }

        [JsonProperty("assignedTo")]
        public string? AssignedTo { get; set; }

        [JsonProperty("assignedToEmail")]
        public string? AssignedToEmail { get; set; }

        [JsonProperty("assignedBy")]
        public string? AssignedBy { get; set; }

        [JsonProperty("assignedByEmail")]
        public string? AssignedByEmail { get; set; }

        [JsonProperty("isAssigned")]
        public bool? IsAssigned { get; set; }

        [JsonProperty("callStatusId")]
        public int? CallStatusId { get; set; }

        [JsonProperty("callActionId")]
        public int? CallActionId { get; set; }

        [JsonProperty("isTimelineView")]
        public bool? IsTimelineView { get; set; }
    }

    public class CSVFileModel
    {
        public string CreatedTime { get; set; }
        public string TicketId { get; set; }
        public string Model { get; set; }
        public string RoasterEngineer { get; set; }
        public string PID { get; set; }
        public string RequesterEmail { get; set; }
        public string RequesterName { get; set; }
        public string Slno { get; set; }
        public string Subject { get; set; }
    }
}
