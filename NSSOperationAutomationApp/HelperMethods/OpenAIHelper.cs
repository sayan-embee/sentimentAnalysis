using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NSSOperationAutomationApp.Models;
using NSSOperationAutomationApp.ServiceMethods;
using System.Text.RegularExpressions;

namespace NSSOperationAutomationApp.HelperMethods
{
    public class OpenAIHelper : IOpenAIHelper
    {
        private readonly IOpenAIServices _openAIServices;
        public OpenAIHelper(IOpenAIServices openAIServices) { this._openAIServices = openAIServices ?? throw new ArgumentNullException(nameof(openAIServices)); }

        public async Task<(ReturnMessageModel, SummaryModel?)> ProcessAudioFile(IFormFile file)
        {
            try
            {
                // Invoking Service Method
                var (result, output) = await this._openAIServices.TranscribeAudioFile(formFile: file);

                if (result == null || ( result != null && result.Status == 0))
                {
                    return (new ReturnMessageModel { Status = 0, ErrorMessage = result?.ErrorMessage ?? string.Empty }, null);
                }

                if (output != null && !string.IsNullOrEmpty(output.ExtractedText))
                {
                    var(summaryResult, summaryOutput) = await _openAIServices.GetChatAsync(output.ExtractedText);

                    if (summaryResult == null || (summaryResult != null && summaryResult.Status == 0))
                    {
                        return (new ReturnMessageModel { Status = 0, ErrorMessage = result?.ErrorMessage ?? string.Empty }, null);
                    }

                    if (summaryOutput != null && !string.IsNullOrEmpty(summaryOutput))
                    {
                        // Regular expression to match JSON-like content
                        string jsonPattern = @"{[^}]+}";

                        Match jsonMatch = Regex.Match(summaryOutput, jsonPattern);

                        if (jsonMatch.Success)
                        {
                            string extractedJson = jsonMatch.Value;

                            try
                            {
                                var summaryObj = JsonConvert.DeserializeObject<SummaryModel>(extractedJson);

                                if (summaryObj != null)
                                {
                                    summaryObj.TranscribeText = output.ExtractedText;

                                    return (new ReturnMessageModel { Status = 1, Message = "Summary has been generated successfully" }, summaryObj);
                                }
                            }
                            catch (Exception ex)
                            {
                                return (new ReturnMessageModel { Status = 0, ErrorMessage = $"Unable to convert summary into JSON object: {ex.Message.ToString()}" }, null);
                            }
                        }

                        return (new ReturnMessageModel { Status = 0, ErrorMessage = "Unable to extract summary from ChatAsync() output string!" }, null);
                    }
                }

                return (new ReturnMessageModel { Status = 0, ErrorMessage = "Unexpected Error!" }, null);
            }
            catch (Exception ex)
            {
                return (new ReturnMessageModel { Status = 0, ErrorMessage = ex.Message.ToString() }, null);
            }
        }
    }
}
