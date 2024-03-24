
namespace NSSOperationAutomationApp.ServiceMethods
{
    public interface IAzureBlobService
    {
        Task<(Uri listUri, string fileName, string refId)> UploadFile(IFormFile file);
    }
}