namespace NSSOperationAutomationApp.Models
{
    public class ReturnMessageModel
    {
        public string Message { get; set; }
        public string ErrorMessage { get; set; }
        public int Status { get; set; }
        public string Id { get; set; }
        public string ReferenceNo { get; set; }
        public string ReferenceObject { get; set; }
        public string ExecutionTime { get; set; }
    }
}
