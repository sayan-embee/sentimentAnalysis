using Microsoft.AspNetCore.Mvc;
using Microsoft.SqlServer.Server;
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
        private readonly IAzureBlobService _azureBlobService;
        private readonly IDataAccess _dataAccess;

        public OpenAIController(ILogger<OpenAIController> logger, IOpenAIHelper openAIHelper, IAzureBlobService azureBlobService, IDataAccess dataAccess)
        {
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this._openAIHelper = openAIHelper ?? throw new ArgumentNullException(nameof(openAIHelper));
            this._azureBlobService = azureBlobService ?? throw new ArgumentNullException(nameof(azureBlobService));
            this._dataAccess = dataAccess ?? throw new ArgumentNullException(nameof(dataAccess));
        }

        #region TRANSCRIBE AUDIO FILE

        #region Old Code 
        //[HttpPost]
        //[Route("audioSummary")]
        //public async Task<IActionResult> GetAudioSummary([FromForm] IFormCollection formdata)
        //{
        //    DateTime startTime = DateTime.UtcNow;

        //    var responseModel = new ReturnMessageModel
        //    {
        //        Status = 0
        //    };

        //    try
        //    {
        //        if (formdata.Files != null && formdata.Files.Count > 0)
        //        {
        //            var FileInfo = new FileInfo(formdata.Files[0].FileName);

        //            if (FileInfo.Extension.Equals(".mp3", StringComparison.OrdinalIgnoreCase))
        //            {

        //                var (result, output) = await this._openAIHelper.ProcessAudioFile(formdata.Files[0]);

        //                if (result == null || (result != null && result.Status == 0))
        //                {
        //                    responseModel.ErrorMessage = result?.ErrorMessage ?? string.Empty;

        //                    return this.Ok(new OpenAIModel { ResponseModel = responseModel });
        //                }

        //                if (output != null && !string.IsNullOrEmpty(output.SummaryText))
        //                {
        //                    responseModel.Status = 1;

        //                    responseModel.Message = result.Message;

        //                    DateTime endTime = DateTime.UtcNow;
        //                    TimeSpan timeDifference = endTime - startTime;
        //                    string formattedTimeDifference = timeDifference.ToString(@"hh\:mm\:ss");
        //                    responseModel.ExecutionTime = formattedTimeDifference;

        //                    #region Blob Storage

        //                    try
        //                    {

        //                        var (uri, internalFileName, refId) = await _azureBlobService.UploadFile(formdata.Files[0]);
        //                        var fileList = new List<BlobFileUploadModel>();
        //                        if (!string.IsNullOrEmpty(internalFileName) && !string.IsNullOrEmpty(refId) && uri != null)
        //                        {
        //                            var fileUploadModel = new BlobFileUploadModel();
        //                            fileUploadModel.ContentType = formdata.Files[0].ContentType;
        //                            fileUploadModel.FileUrl = uri.ToString();
        //                            fileUploadModel.FileName = formdata.Files[0].FileName;
        //                            fileUploadModel.FileInternalName = internalFileName.ToString();
        //                            fileUploadModel.RefId = refId.ToString();
        //                            fileList.Add(fileUploadModel);
        //                        }
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        this._logger.LogError(ex, $"OpenAIController --> GetAudioSummary() --> Blob Storage execution failed");
        //                        ExceptionLogging.SendErrorToText(ex);
        //                    }

        //                    #endregion


        //                    return this.Ok(new OpenAIModel { ResponseModel = responseModel, OutputModel = output });
        //                }
        //            }
        //        }

        //        responseModel.ErrorMessage = "Invalid file / file-extension!";

        //        return this.Ok(new OpenAIModel { ResponseModel = responseModel });
        //    }
        //    catch (Exception ex)
        //    {
        //        this._logger.LogError(ex, $"OpenAIController --> GetAudioSummary() execution failed");
        //        ExceptionLogging.SendErrorToText(ex);
        //        responseModel.ErrorMessage = ex.Message.ToString();
        //        return this.Ok(new OpenAIModel { ResponseModel = responseModel });
        //    }
        //}
        #endregion


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

                        #region Blob Storage

                        var fileUploadModel = new BlobFileUploadModel();

                        try
                        {
                            var (uri, internalFileName, refId) = await _azureBlobService.UploadFile(formdata.Files[0]);
                            var fileList = new List<BlobFileUploadModel>();
                            if (!string.IsNullOrEmpty(internalFileName) && !string.IsNullOrEmpty(refId) && uri != null)
                            {
                                fileUploadModel.ContentType = formdata.Files[0].ContentType;
                                fileUploadModel.FileUrl = uri.ToString();
                                fileUploadModel.FileName = formdata.Files[0].FileName;
                                fileUploadModel.FileInternalName = internalFileName.ToString();
                                fileUploadModel.RefId = refId.ToString();
                            }
                        }
                        catch (Exception ex)
                        {
                            this._logger.LogError(ex, $"OpenAIController --> GetAudioSummary() --> Blob Storage execution failed");
                            ExceptionLogging.SendErrorToText(ex);

                            responseModel.ErrorMessage = "File not uploaded to blob!";
                            return this.Ok(new OpenAIModel { ResponseModel = responseModel });
                        }

                        #endregion

                        var (result, output) = await this._openAIHelper.ProcessAudioFile(formdata.Files[0]);

                        if (result == null || (result != null && result.Status == 0))
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
                            //return this.Ok(new OpenAIModel { ResponseModel = responseModel, OutputModel = output, FileOutputModel = fileUploadModel });
                            
                            var data = new OpenAIModel
                            {
                                ResponseModel = responseModel,
                                OutputModel = output, 
                                FileOutputModel = fileUploadModel 
                            };

                            _ = await _dataAccess.InsertAudioSummary(data);

                            return this.Ok(data);

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

        [HttpGet]
        [Route("getAudioSummary")]
        public async Task<IActionResult> GetAudioSummary(int? Id)
        {
            try
            {
                var result = await _dataAccess.GetAudioSummary(Id);

                return this.Ok(result);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, $"MasterAPIController --> GetCallAction() execution failed");
                ExceptionLogging.SendErrorToText(ex);
                return this.Ok(ex.Message);
            }
        }


        #endregion
    }
}
