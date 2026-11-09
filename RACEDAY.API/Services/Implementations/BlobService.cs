using Azure.Storage.Blobs;

namespace RaceDay.API.Services
{
    public class BlobService
    {
        private readonly string _connectionString;

        public BlobService(IConfiguration configuration)
        {
            _connectionString = configuration["AzureBlobStorage"] ?? "UseDevelopmentStorage=true";
        }

        public async Task<string> UploadFileAsync(IFormFile file, string containerName = "raceday-images")
        {
            var blobServiceClient = new BlobServiceClient(_connectionString);
            var containerClient = blobServiceClient.GetBlobContainerClient(containerName);

            // Create container if it doesn't exist and make it public so browsers can view the images
            await containerClient.CreateIfNotExistsAsync(Azure.Storage.Blobs.Models.PublicAccessType.Blob);

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var blobClient = containerClient.GetBlobClient(fileName);

            using var stream = file.OpenReadStream();
            await blobClient.UploadAsync(stream, true);

            return blobClient.Uri.ToString();
        }
    }
}