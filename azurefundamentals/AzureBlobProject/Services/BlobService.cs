using Azure.Core;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using Azure.Storage.Sas;
using AzureBlobProject.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.Net.Http.Headers;

namespace AzureBlobProject.Services
{
    public class BlobService : IBlobService
    {
        private readonly BlobServiceClient _serviceClient;

        public BlobService(BlobServiceClient serviceClient)
        {
            _serviceClient = serviceClient;
        }

        public async Task<bool> DeleteBlob(string? name, string? containerName)
        {
            BlobContainerClient containerClient = _serviceClient.GetBlobContainerClient(containerName);
            BlobClient blobClient = containerClient.GetBlobClient(name);

            return await blobClient.DeleteIfExistsAsync();
        }

        public async Task<IList<string?>> GetAllBlobs(string? containerName)
        {
            BlobContainerClient containerClient = _serviceClient.GetBlobContainerClient(containerName);
            var blobs = containerClient.GetBlobsAsync();

            IList<string?> blobString = new List<string?>();
            await foreach (var blob in blobs)
            {
                blobString.Add(blob.Name);
            }

            return blobString;
        }

        public async Task<IList<Blob>> GetAllBlobsWithUri(string? containerName)
        {
            BlobContainerClient containerClient = _serviceClient.GetBlobContainerClient(containerName);
            var blobs = containerClient.GetBlobsAsync();

            IList<Blob> blobList = new List<Blob>();
            string sasContainerSignature = string.Empty;
            //string sasContainerSignature = "sv=2021-06-08&ss=bfqt&srt=sco&sp=rwdlacupiytfx&se=2023-01-09T22:45:28Z&st=2023-01-09T14:45:28Z&spr=https&sig=QuT1HDjyO%2B5nKdo%2FXUZL09VuzqrlhuwGW6FOPqVVh8Y%3D";

            if (containerClient.CanGenerateSasUri)
            {
                BlobSasBuilder blobSasBuilder = new()
                {
                    BlobContainerName = containerClient.Name,
                    Resource = "c",
                    ExpiresOn = DateTimeOffset.UtcNow.AddHours(1),
                };

                blobSasBuilder.SetPermissions(BlobSasPermissions.Read);
                sasContainerSignature = containerClient.GenerateSasUri(blobSasBuilder).AbsoluteUri.Split('?')[1].ToString();
            }

            await foreach (var item in blobs)
            {
                BlobClient blobClient = containerClient.GetBlobClient(item.Name);
                Blob blob = new() { Uri = $"{blobClient.Uri.AbsoluteUri}?{sasContainerSignature}" };
                //Blob blob = new() { Uri = blobClient.Uri.AbsoluteUri };

                //if (blobClient.CanGenerateSasUri)
                //{
                //    BlobSasBuilder blobSasBuilder = new() 
                //    { 
                //        BlobContainerName = blobClient.GetParentBlobContainerClient().Name,
                //        BlobName = blobClient.Name,
                //        Resource = "b",
                //        ExpiresOn = DateTimeOffset.UtcNow.AddMinutes(1),
                //    };

                //    blobSasBuilder.SetPermissions(BlobSasPermissions.Read);

                //    blob.Uri = blobClient.GenerateSasUri(blobSasBuilder).AbsoluteUri;
                //}

                BlobProperties blobProperties = await blobClient.GetPropertiesAsync();
                if (blobProperties != null)
                {
                    if (blobProperties.Metadata.ContainsKey("title"))
                    {
                        blob.Title = blobProperties.Metadata["title"];
                    }
                    if (blobProperties.Metadata.ContainsKey("comment"))
                    {
                        blob.Comment = blobProperties.Metadata["comment"];
                    }
                }
                blobList.Add(blob);
            }

            return blobList;
        }

        public async Task<string> GetBlob(string? name, string? containerName)
        {
            BlobContainerClient containerClient = _serviceClient.GetBlobContainerClient(containerName);
            BlobClient blobClient = containerClient.GetBlobClient(name);
            return blobClient.Uri.AbsoluteUri;
        }

        public async Task<bool> UploadBlob(string? name, IFormFile file, string? containerName, Blob blob)
        {
            BlobContainerClient containerClient = _serviceClient.GetBlobContainerClient(containerName);
            BlobClient blobClient = containerClient.GetBlobClient(name);

            var httpHeaders = new BlobHttpHeaders() { ContentType = file.ContentType };

            IDictionary<string, string> metadata = new Dictionary<string, string>();
            metadata.Add("title", blob.Title);
            metadata["comment"] = blob.Comment;

            var result = await blobClient.UploadAsync(file.OpenReadStream(), httpHeaders: httpHeaders, metadata: metadata);

            //metadata.Remove("title");
            //await blobClient.SetMetadataAsync(metadata);

            if (result != null)
                return true;
            else
                return false;

        }
    }
}
