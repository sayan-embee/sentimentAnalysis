using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSSOperationAutomationApp.DataAccessHelper;
using NSSOperationAutomationApp.HelperMethods;
using NSSOperationAutomationApp.Models;

namespace NSSOperationAutomationApp.Controllers
{
    [Route("api/masters")]
    [ApiController]
    [Authorize]
    public class MasterAPIController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IDataAccess _dataAccess;

        public MasterAPIController(ILogger<APIController> logger, IDataAccess dataAccess)
        {
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this._dataAccess = dataAccess ?? throw new ArgumentNullException(nameof(dataAccess));
        }

        [HttpGet]
        [Route("callAction/get")]
        public async Task<IActionResult> GetCallAction(int? Id)
        {
            try
            {
                //DateTime startTime = DateTime.UtcNow;
                //ExceptionLogging.WriteMessageToText($"APIController --> Get() execution started: {DateTime.UtcNow}");
                //this._logger.LogInformation($"APIController --> Get() execution started: {DateTime.UtcNow}");

                var result = await _dataAccess.GetCallAction(Id);

                //DateTime endTime = DateTime.UtcNow;
                //ExceptionLogging.WriteMessageToText($"APIController --> Get() execution ended: {DateTime.UtcNow}");
                //this._logger.LogInformation($"APIController --> Get() execution ended: {DateTime.UtcNow}");

                //TimeSpan timeDifference = endTime - startTime;
                //string formattedTimeDifference = timeDifference.ToString(@"hh\:mm\:ss");

                //ExceptionLogging.WriteMessageToText($"APIController --> Get() execution time: {formattedTimeDifference}");
                //this._logger.LogInformation($"APIController --> Get() execution time: {formattedTimeDifference}");

                return this.Ok(result);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, $"MasterAPIController --> GetCallAction() execution failed");
                ExceptionLogging.SendErrorToText(ex);
                return this.Ok(ex.Message);
            }
        }

        [HttpGet]
        [Route("documentType/get")]
        public async Task<IActionResult> GetDocumentType(int? Id)
        {
            try
            {
                //DateTime startTime = DateTime.UtcNow;
                //ExceptionLogging.WriteMessageToText($"APIController --> Get() execution started: {DateTime.UtcNow}");
                //this._logger.LogInformation($"APIController --> Get() execution started: {DateTime.UtcNow}");

                var result = await _dataAccess.GetDocumentType(Id);

                //DateTime endTime = DateTime.UtcNow;
                //ExceptionLogging.WriteMessageToText($"APIController --> Get() execution ended: {DateTime.UtcNow}");
                //this._logger.LogInformation($"APIController --> Get() execution ended: {DateTime.UtcNow}");

                //TimeSpan timeDifference = endTime - startTime;
                //string formattedTimeDifference = timeDifference.ToString(@"hh\:mm\:ss");

                //ExceptionLogging.WriteMessageToText($"APIController --> Get() execution time: {formattedTimeDifference}");
                //this._logger.LogInformation($"APIController --> Get() execution time: {formattedTimeDifference}");

                return this.Ok(result);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, $"MasterAPIController --> GetDocumentType() execution failed");
                ExceptionLogging.SendErrorToText(ex);
                return this.Ok(ex.Message);
            }
        }

        [HttpGet]
        [Route("partConsumptionType/get")]
        public async Task<IActionResult> GetPartConsumptionType(int? Id)
        {
            try
            {
                //DateTime startTime = DateTime.UtcNow;
                //ExceptionLogging.WriteMessageToText($"APIController --> Get() execution started: {DateTime.UtcNow}");
                //this._logger.LogInformation($"APIController --> Get() execution started: {DateTime.UtcNow}");

                var result = await _dataAccess.GetPartConsumptionType(Id);

                //DateTime endTime = DateTime.UtcNow;
                //ExceptionLogging.WriteMessageToText($"APIController --> Get() execution ended: {DateTime.UtcNow}");
                //this._logger.LogInformation($"APIController --> Get() execution ended: {DateTime.UtcNow}");

                //TimeSpan timeDifference = endTime - startTime;
                //string formattedTimeDifference = timeDifference.ToString(@"hh\:mm\:ss");

                //ExceptionLogging.WriteMessageToText($"APIController --> Get() execution time: {formattedTimeDifference}");
                //this._logger.LogInformation($"APIController --> Get() execution time: {formattedTimeDifference}");

                return this.Ok(result);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, $"MasterAPIController --> GetPartConsumptionType() execution failed");
                ExceptionLogging.SendErrorToText(ex);
                return this.Ok(ex.Message);
            }
        }

        [HttpGet]
        [Route("callStatus/get")]
        public async Task<IActionResult> GetCallStatus(int? Id)
        {
            try
            {
                //DateTime startTime = DateTime.UtcNow;
                //ExceptionLogging.WriteMessageToText($"APIController --> Get() execution started: {DateTime.UtcNow}");
                //this._logger.LogInformation($"APIController --> Get() execution started: {DateTime.UtcNow}");

                var result = await _dataAccess.GetCallStatus(Id);

                //DateTime endTime = DateTime.UtcNow;
                //ExceptionLogging.WriteMessageToText($"APIController --> Get() execution ended: {DateTime.UtcNow}");
                //this._logger.LogInformation($"APIController --> Get() execution ended: {DateTime.UtcNow}");

                //TimeSpan timeDifference = endTime - startTime;
                //string formattedTimeDifference = timeDifference.ToString(@"hh\:mm\:ss");

                //ExceptionLogging.WriteMessageToText($"APIController --> Get() execution time: {formattedTimeDifference}");
                //this._logger.LogInformation($"APIController --> Get() execution time: {formattedTimeDifference}");

                return this.Ok(result);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, $"MasterAPIController --> GetPartConsumptionType() execution failed");
                ExceptionLogging.SendErrorToText(ex);
                return this.Ok(ex.Message);
            }
        }
    }
}
