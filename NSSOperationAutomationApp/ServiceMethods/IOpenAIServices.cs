using NSSOperationAutomationApp.Models;

namespace NSSOperationAutomationApp.ServiceMethods
{
    public interface IOpenAIServices
    {
        Task<(ReturnMessageModel, AudioOutputModel?)> TranscribeAudioFile(IFormFile formFile);
        Task<(ReturnMessageModel, string)> GetChatAsync(string inputText, int maxTokens = 0);
    }
}