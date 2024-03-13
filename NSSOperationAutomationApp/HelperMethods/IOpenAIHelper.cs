using NSSOperationAutomationApp.Models;

namespace NSSOperationAutomationApp.HelperMethods
{
    public interface IOpenAIHelper
    {
        Task<(ReturnMessageModel, SummaryModel?)> ProcessAudioFile(IFormFile file);
    }
}