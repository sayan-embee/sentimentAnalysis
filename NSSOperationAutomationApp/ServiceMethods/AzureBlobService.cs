using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Options;
using NSSOperationAutomationApp.Models;

namespace NSSOperationAutomationApp.ServiceMethods
{
    public class AzureBlobService : IAzureBlobService
    {
        private readonly IOptions<AzureBlobSettings> blobOptions;
        private readonly ILogger<AzureBlobService> logger;
        private readonly IConfiguration configuration;

        public AzureBlobService(
            ILogger<AzureBlobService> logger, IOptions<AzureBlobSettings> blobOptions, IConfiguration configuration)
        {
            this.blobOptions = blobOptions ?? throw new ArgumentNullException(nameof(blobOptions));
            this.logger = logger;
            this.configuration = configuration;
        }


        public async Task<(Uri listUri, string fileName, string refId)> UploadFile(IFormFile file)
        {            
            try
            {
                string connectionString = configuration.GetValue<string>("AzureBlobSettings:StorageConnectionString");
                string containerName = configuration.GetValue<string>("AzureBlobSettings:ContainerName");
                BlobContainerClient container = new BlobContainerClient(connectionString, containerName);

                var result = await container.ExistsAsync(cancellationToken: default);
                if (result)
                {
                    var refId = Guid.NewGuid().ToString();
                    string fileName = $"{refId}_{file.FileName}";
                    // Get a reference to a blob
                    BlobClient blobClient = container.GetBlobClient(fileName);

                    this.logger.LogInformation($"Uploading file {fileName}.");
                    await blobClient.UploadAsync(file.OpenReadStream(), new BlobHttpHeaders { ContentType = file.ContentType }, cancellationToken: default);
                    this.logger.LogInformation($"Setting metadata Id for file : {fileName}.");

                    IDictionary<string, string> metadata = new Dictionary<string, string>();
                    // Add metadata to the dictionary by using key/value syntax
                    metadata["Id"] = refId;
                    // Set the blob's metadata.
                    await blobClient.SetMetadataAsync(metadata);
                    Uri listUri = blobClient.Uri;

                    return (listUri, fileName, refId);
                }
                else
                {
                    this.logger.LogInformation($"Blob container {this.blobOptions.Value.ContainerName} not found.");
                    return (null, null, null);
                    
                }
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $"Error occurred while uploading file -> Id :{file.FileName}");
                return (null, null, null);
                
            }
        }

    }
}
