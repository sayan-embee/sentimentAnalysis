using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.VisualBasic.FileIO;
using NSSOperationAutomationApp.Controllers;
using NSSOperationAutomationApp.DataAccessHelper.DBAccess;
using NSSOperationAutomationApp.Models;
using System.Text.RegularExpressions;

namespace NSSOperationAutomationApp.HelperMethods
{
    public class FileHelper : IFileHelper
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _config;

        public FileHelper(ILogger<FileHelper> logger, 
            IConfiguration config)
        {
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this._config = config ?? throw new ArgumentNullException(nameof(config));
        }

        #region PRIVATE HELPER METHODS

        public async Task<string> CheckOrCreateDirectory(string CaseNumber)
        {
            try
            {
                if (!string.IsNullOrEmpty(CaseNumber))
                {
                    var modifiedCaseNumber = "CaseNo_" + CaseNumber;

                    var mainDirectoryPath = System.IO.Directory.GetCurrentDirectory() + @"\ApplicationFiles";
                    if (!System.IO.Directory.Exists(mainDirectoryPath))
                    {
                        System.IO.Directory.CreateDirectory(mainDirectoryPath);
                    }

                    var subDirectoryPath = Path.Combine(mainDirectoryPath, modifiedCaseNumber);
                    if (!System.IO.Directory.Exists(subDirectoryPath))
                    {
                        System.IO.Directory.CreateDirectory(subDirectoryPath);
                    }

                    return subDirectoryPath;
                }

                await Task.Delay(0);
                return string.Empty;
            }
            catch(Exception ex) 
            {
                this._logger.LogError(ex, $"FileHelper --> CheckOrCreateDirectory() execution failed");
                ExceptionLogging.SendErrorToText(ex);
                return string.Empty;
            }
        }

        public async Task<bool> UploadFile(string filePath, IFormFile file)
        {
            try
            {
                if (!string.IsNullOrEmpty(filePath) && file != null)
                {
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                        return true;
                    }
                }
                
                return false;
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, $"FileHelper --> UploadFile() execution failed");
                ExceptionLogging.SendErrorToText(ex);
                return false;
            }
        }

        public async Task<(List<CSVFileModel> data, List<string> missingColumns)> ReadCSVFile(List<string> expectedColumns, IFormFile file)
        {
            List<CSVFileModel> resultData = new List<CSVFileModel>();
            List<string> missingColumns = new List<string>();

            using (TextFieldParser parser = new TextFieldParser(file.OpenReadStream()))
            {
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(",");

                // Check if the header contains all expected columns
                string[] header = parser.ReadFields();
                foreach (string expectedColumn in expectedColumns)
                {
                    if (!header.Contains(expectedColumn))
                    {
                        missingColumns.Add(expectedColumn);
                    }
                }

                // If any columns are missing, return the result
                if (missingColumns.Count > 0)
                {
                    await Task.Delay(0);
                    return (resultData, missingColumns);
                }

                // Read the data rows
                while (!parser.EndOfData)
                {
                    string[] fields = parser.ReadFields();

                    // Create YourObject instances using the values from the CSV
                    CSVFileModel obj = new CSVFileModel
                    {
                        CreatedTime = fields[Array.IndexOf(header, "CreatedTime")],
                        TicketId = fields[Array.IndexOf(header, "TicketId")],
                        Model = fields[Array.IndexOf(header, "Model")],
                        RoasterEngineer = fields[Array.IndexOf(header, "RoasterEngineer")],
                        PID = fields[Array.IndexOf(header, "PID")],
                        RequesterEmail = fields[Array.IndexOf(header, "RequesterEmail")],
                        RequesterName = fields[Array.IndexOf(header, "RequesterName")],
                        Slno = fields[Array.IndexOf(header, "Slno")],
                        Subject = fields[Array.IndexOf(header, "Subject")]
                    };

                    resultData.Add(obj);
                }
            }

            await Task.Delay(0);
            return (resultData, missingColumns);
        }

        #endregion

        public async Task<List<CallDocumentsModel>?> UploadFilesOnServer (string CaseNumber, List<CallDocumentsModel> callDocumentList, IFormFileCollection fileList)
        {
            try
            {
                if (!string.IsNullOrEmpty(CaseNumber) && callDocumentList != null && callDocumentList.Any())
                {
                    var directoryPath = await CheckOrCreateDirectory(CaseNumber);
                    if (!string.IsNullOrEmpty(directoryPath))
                    {
                        foreach (var eachFile in fileList)
                        {
                            if (eachFile != null && eachFile.Length > 0)
                            {
                                FileInfo fi = new FileInfo(eachFile.FileName);
                                string ext = fi.Extension;
                                string customFileName = Guid.NewGuid().ToString() + ext;

                                var objectToUpdate = callDocumentList.FirstOrDefault(f => f.InternalName == eachFile.Name);

                                if (objectToUpdate != null)
                                {
                                    objectToUpdate.DocumentName = eachFile.FileName;
                                    objectToUpdate.MimeType = eachFile.ContentType;                                    
                                    objectToUpdate.InternalName = customFileName;

                                    string filePath = Path.Combine(directoryPath, customFileName);
                                    if (!string.IsNullOrEmpty(filePath))
                                    {
                                        var uploadResult = await UploadFile(filePath, eachFile);
                                        if (uploadResult)
                                        {
                                            objectToUpdate.DocumentUrlPath = filePath;
                                        }
                                    }
                                }                                
                            }
                        }

                        return callDocumentList;
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, $"FileHelper --> UploadFilesOnServer() execution failed");
                ExceptionLogging.SendErrorToText(ex);
                return null;
            }
        }

        public async Task<(List<TicketCreateInBulkModel>? ticketDetailsList, string message)> ReadCSVFileFromLocal(IFormFile csvFile)
        {
            try
            {
                string message = string.Empty;
                var validDomainList = new List<string>();
                var requiredColumnList = new List<string>();

                var expectedColumns = _config.GetValue<string>("CSVFileSettings:ColumnsToRead");

                if (!string.IsNullOrEmpty(expectedColumns))
                {
                    var expectedColumnList = expectedColumns.Split(",").ToList();          
                    
                    if (expectedColumnList != null && expectedColumnList.Any())
                    {
                        var validDomains = _config.GetValue<string>("CSVFileSettings:AllowedDomains");

                        if (!string.IsNullOrEmpty(validDomains))
                        {
                            validDomainList = validDomains.Split(",").ToList();
                        }

                        var requiredColumns = _config.GetValue<string>("CSVFileSettings:RequiredColumnValues");

                        if (!string.IsNullOrEmpty(requiredColumns))
                        {
                            requiredColumnList = requiredColumns.Split(",").ToList();
                        }

                        var (data, missingColumns) = await this.ReadCSVFile(expectedColumnList, csvFile);

                        if (missingColumns.Count > 0)
                        {
                            message = "Some columns are missing: " + string.Join(", ", missingColumns);

                            return (null, message);
                        }
                        else
                        {
                            var validItems = new List<CSVFileModel>();

                            if (requiredColumnList.Any())
                            {
                                // LINQ check for non-empty values and valid TicketId, filtering by required columns
                               validItems = data.Where(item =>
                                {
                                    // Check for empty/whitespace-only values in required columns
                                    return requiredColumnList.All(columnName =>
                                    {
                                        object value = item.GetType().GetProperty(columnName).GetValue(item);
                                        return value != null && !string.IsNullOrWhiteSpace(value as string);
                                    });
                                    //&&
                                    //// Check for numeric TicketId:
                                    //long.TryParse((string)item.GetType().GetProperty("TicketId").GetValue(item), out long parsedTicketId);

                                }).ToList();
                            }
                            else
                            {
                                // LINQ checks for empty/whitespace-only values and numeric TicketId:
                                validItems = data.Where(item =>

                                    // Check for empty/whitespace-only values:
                                    item.GetType().GetProperties().All(prop => !string.IsNullOrWhiteSpace(prop.GetValue(item) as string))

                                    //&&
                                    //// Check for numeric TicketId:
                                    //long.TryParse((string)item.GetType().GetProperty("TicketId").GetValue(item), out long parsedTicketId)
                               ).ToList();
                            }                            

                            if (data.Count != validItems.Count())
                            {
                                message = "Some columns have invalid or missing data";
                                return (null, message);
                            }

                            // LINQ checks for numeric TicketId:
                            validItems = data.Where(item => 
                            long.TryParse((string)item.GetType().GetProperty("TicketId").GetValue(item), out long parsedTicketId)).ToList();

                            if (data.Count != validItems.Count())
                            {
                                message = "Some TicketIds have invalid data";
                                return (null, message);
                            }

                            if (data.Count != (validItems.Select(item => item.TicketId).Distinct().Count()))
                            {
                                message = "Some TicketIds are repeating";
                                return (null, message);
                            }

                            if (validDomainList.Any())
                            {
                                // Filter validItems for valid RoasterEngineer emails
                                var validItemsWithValidEmails = validItems.Where(item =>
                                {
                                    string email = item.RoasterEngineer;
                                    if (Regex.IsMatch(email, @"^[^@]+@[^@]+\.[^@]+$")) // Basic email format check
                                    {
                                        string domain = email.Substring(email.IndexOf('@') + 1);
                                        return validDomains.Contains(domain); // Check against valid domains
                                    }
                                    else
                                    {
                                        return false;
                                    }
                                }).ToList();

                                if (validItemsWithValidEmails.Count() != validItems.Count())
                                {
                                    message = "Some RoasterEngineer emails have invalid domains";
                                    return (null, message);
                                }
                            }                            

                            var ticketDetailsList = validItems.Select(item => new TicketCreateInBulkModel
                            {
                                CaseNumber = (item.TicketId).ToString(),
                                TicketId = long.TryParse(item.TicketId, out long parsedTicketId) ? parsedTicketId : 0,
                                CaseSubject = item.Subject,
                                ProductName = item.Model,
                                ProductNumber = item.PID,
                                AssignedToEmail = item.RoasterEngineer,
                                ContactName = item.RequesterName,
                                ContactEmail = item.RequesterEmail,
                                SerialNumber = item.Slno,
                                CreatedOn = item.CreatedTime,
                                TicketType = TicketType.Type2.ToString()
                            }).ToList();

                            return (ticketDetailsList, "200");
                        }
                    }
                }

                return (null, message);
            }
            catch(Exception ex)
            {
                this._logger.LogError(ex, $"FileHelper --> ReadCSVFileFromLocal() execution failed");
                ExceptionLogging.SendErrorToText(ex);
                return (null, "Execution Failed - "+ex.ToString());
            }
        }
    }
}
