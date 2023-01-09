using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace AzureBlobProject.Services
{
    public class ContainerService : IContainerService
    {
        private readonly BlobServiceClient _serviceClient;

        public ContainerService(BlobServiceClient serviceClient)
        {
            _serviceClient = serviceClient;
        }

        public async Task CreateContainer(string? containerName)
        {
            BlobContainerClient blobContainerClient = _serviceClient.GetBlobContainerClient(containerName);
            await blobContainerClient.CreateIfNotExistsAsync(PublicAccessType.BlobContainer);
        }

        public async Task DeleteContainer(string? containerName)
        {
            BlobContainerClient blobContainerClient = _serviceClient.GetBlobContainerClient(containerName);
            await blobContainerClient.DeleteIfExistsAsync();
        }

        public async Task<List<string>> GetAllContainer()
        {
            List<string> containerNames = new();

            await foreach (BlobContainerItem item in _serviceClient.GetBlobContainersAsync())
            {
                containerNames.Add(item.Name);
            }

            return containerNames;
        }

        public async Task<List<string>> GetAllContainerAndBlobs()
        {
            List<string> containerAndBlobNames = new()
            {
                $"Account Name: {_serviceClient.AccountName}",
                "----------------------------------------------------------------"
            };
            await foreach (var blobContainerItem in _serviceClient.GetBlobContainersAsync())
            {
                containerAndBlobNames.Add($"----{blobContainerItem.Name}");
                BlobContainerClient blobContainer = _serviceClient.GetBlobContainerClient(blobContainerItem.Name);

                await foreach (BlobItem blobItem in blobContainer.GetBlobsAsync())
                {
                    string blobName = blobItem.Name;
                    BlobClient blobClient = blobContainer.GetBlobClient(blobName);
                    BlobProperties blobProperties = await blobClient.GetPropertiesAsync();

                    if(blobProperties.Metadata.ContainsKey("title"))
                    {
                        blobName += $"({blobProperties.Metadata["title"]})";
                    }
                    containerAndBlobNames.Add($"--------{blobName}");
                }
                containerAndBlobNames.Add("----------------------------------------------------------------");
            }

            return containerAndBlobNames;
        }
    }
}
