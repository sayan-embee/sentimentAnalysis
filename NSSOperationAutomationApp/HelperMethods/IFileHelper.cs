using NSSOperationAutomationApp.Models;
using System.Threading.Tasks;

namespace NSSOperationAutomationApp.HelperMethods
{
    public interface IFileHelper
    {
        Task<string> CheckOrCreateDirectory(string CaseNumber);
        Task<List<CallDocumentsModel>?> UploadFilesOnServer(string CaseNumber, List<CallDocumentsModel> callDocumentList, IFormFileCollection fileList);
        Task<(List<TicketCreateInBulkModel>? ticketDetailsList, string message)> ReadCSVFileFromLocal(IFormFile csvFile);
    }
}