using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Threading.Tasks;

namespace ST10114423.Functions
{
    public static class UploadBlob
    {
        [Function("UploadBlob")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            string containerName = req.Query["containerName"];
            string blobName = req.Query["blobName"];

            if (string.IsNullOrEmpty(containerName) || string.IsNullOrEmpty(blobName))
            {
                return new BadRequestObjectResult("Container name and blob name must be provided.");
            }

            var connectionString = Environment.GetEnvironmentVariable("DefaultEndpointsProtocol=https;AccountName=cldvst10386045;AccountKey=/sRIRStJFmvZerB+NFDcrAyv8+aPXbB7KS8/Go5A76uPl5SPX/7Q3W/X4JLtJxQy+zO6a2b2dvuQ+AStTLxziA==;EndpointSuffix=core.windows.net");
            var blobServiceClient = new BlobServiceClient(connectionString);
            var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
            await containerClient.CreateIfNotExistsAsync();
            var blobClient = containerClient.GetBlobClient(blobName);

            using var stream = req.Body;
            await blobClient.UploadAsync(stream, true);

            return new OkObjectResult("Blob uploaded");
        }
    }
}
