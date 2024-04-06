using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using BlazorStack.Services.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorStack.Services
{
    public class BlobService
    {
        private readonly BlobServiceClient _blobs;
        private readonly StorageAccountSettings _settings;
        private readonly ILogger<BlobService> _logger;

        public BlobService(IOptions<StorageAccountSettings> settings, ILogger<BlobService> logger)
        {
            _logger = logger;
            _settings = settings.Value;
            _blobs = new BlobServiceClient(_settings.ConnectionString);
        }

        public async Task<string> UploadProfilePhoto(string userId, Stream photoStream) => await UploadPhoto(_settings.ProfilePhotosContainer, userId, photoStream);

        public async Task<string> UploadPhoto(string containerName, string fileName, Stream content)
        {
            try
            {
                fileName += ".png";
                var containerClient = _blobs.GetBlobContainerClient(containerName);
                if (await containerClient.ExistsAsync() == false) await containerClient.CreateAsync();

                var blobClient = containerClient.GetBlobClient(fileName);
                var response = await blobClient.UploadAsync(content);
                var success = response.GetRawResponse().Status == 201;
                return success ? blobClient.Uri.ToString() : string.Empty;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return string.Empty;
            }
        }
    }
}
