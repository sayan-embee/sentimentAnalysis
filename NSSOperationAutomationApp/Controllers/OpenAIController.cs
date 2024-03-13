using Microsoft.AspNetCore.Mvc;
using NSSOperationAutomationApp.DataAccessHelper;
using NSSOperationAutomationApp.HelperMethods;
using NSSOperationAutomationApp.Models;
using NSSOperationAutomationApp.ServiceMethods;

namespace NSSOperationAutomationApp.Controllers
{
    [Route("api/openAI")]
    [ApiController]
    public class OpenAIController : Controller
    {
        private readonly ILogger _logger;
        private readonly IOpenAIHelper _openAIHelper;

        public OpenAIController(ILogger<OpenAIController> logger, IOpenAIHelper openAIHelper)
        {
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this._openAIHelper = openAIHelper ?? throw new ArgumentNullException(nameof(openAIHelper));
        }

        #region TRANSCRIBE AUDIO FILE

        [HttpPost]
        [Route("audioSummary")]
        public async Task<IActionResult> GetAudioSummary([FromForm] IFormCollection formdata)
        {
            DateTime startTime = DateTime.UtcNow;

            var responseModel = new ReturnMessageModel
            {
                Status = 0                
            };

            try
            {
                if (formdata.Files != null && formdata.Files.Count > 0)
                {
                    var FileInfo = new FileInfo(formdata.Files[0].FileName);

                    if (FileInfo.Extension.Equals(".mp3", StringComparison.OrdinalIgnoreCase))
                    {
                        var (result, output) = await this._openAIHelper.ProcessAudioFile(formdata.Files[0]);

                        if (result ==  null || (result != null && result.Status == 0))
                        {
                            responseModel.ErrorMessage = result?.ErrorMessage ?? string.Empty;

                            return this.Ok(new OpenAIModel { ResponseModel = responseModel });
                        }

                        if (output != null && !string.IsNullOrEmpty(output.SummaryText))
                        {
                            responseModel.Status = 1;

                            responseModel.Message = result.Message;

                            DateTime endTime = DateTime.UtcNow;
                            TimeSpan timeDifference = endTime - startTime;
                            string formattedTimeDifference = timeDifference.ToString(@"hh\:mm\:ss");
                            responseModel.ExecutionTime = formattedTimeDifference;

                            return this.Ok(new OpenAIModel { ResponseModel = responseModel, OutputModel = output });
                        }
                    }
                }

                responseModel.ErrorMessage = "Invalid file / file-extension!";

                return this.Ok(new OpenAIModel { ResponseModel = responseModel });
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, $"OpenAIController --> GetAudioSummary() execution failed");
                ExceptionLogging.SendErrorToText(ex);
                responseModel.ErrorMessage = ex.Message.ToString();
                return this.Ok(new OpenAIModel { ResponseModel = responseModel });
            }
        }

        #endregion
    }
}
