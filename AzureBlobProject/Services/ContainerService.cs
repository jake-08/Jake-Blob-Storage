using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace AzureBlobProject.Services
{
	public class ContainerService : IContainerService
    {
        private readonly BlobServiceClient _blobClient;

        public ContainerService(BlobServiceClient blobClient)
        {
            _blobClient = blobClient;
        }

        public async Task<List<string>> GetAllContainers()
        {
            List<string> containerName = new List<string>();

            // Get all blob containers
            await foreach (BlobContainerItem blobContainerItem in _blobClient.GetBlobContainersAsync())
            {
                containerName.Add(blobContainerItem.Name);
            }

            return containerName;
        }

        public async Task<List<string>> GetAllContainerAndBlobs()
        {
            // Create a list of blob container and names
            List<string> containerAndBlobNames = new();
            
            // Add the accoutn name 
            containerAndBlobNames.Add("Account Name: " + _blobClient.AccountName);
            // Add a separator
            containerAndBlobNames.Add("------------------------------------------------------------------------------------------------------------");
            
            // Loop the container level 
            await foreach (BlobContainerItem blobContainerItem in _blobClient.GetBlobContainersAsync())
            {
                containerAndBlobNames.Add("Container Name: " + blobContainerItem.Name);
                // Create BlobContainerClient with the container name
                BlobContainerClient _blobContainer =
                      _blobClient.GetBlobContainerClient(blobContainerItem.Name);
                // Loop all the blobs under the container
                await foreach (BlobItem blobItem in _blobContainer.GetBlobsAsync())
                {
                    // Create BlobClient with the blob name
                    BlobClient blobClient = _blobContainer.GetBlobClient(blobItem.Name);
                    // Get the properties of the blob
                    BlobProperties blobProperties = await blobClient.GetPropertiesAsync();
                    // Add the blob name
                    string blobToAdd = blobItem.Name;
                    // Get metadata and add if exists 
                    if (blobProperties.Metadata.ContainsKey("title"))
                    {
                        blobToAdd += " (Title: " + blobProperties.Metadata["title"] + ")";
                    }
                    if (blobProperties.Metadata.ContainsKey("comment"))
                    {
                        blobToAdd += " (Description: " + blobProperties.Metadata["comment"] + ")";
                    }

                    containerAndBlobNames.Add("------ " + blobToAdd);
                }
                containerAndBlobNames.Add("------------------------------------------------------------------------------------------------------------");

            }

            return containerAndBlobNames;
        }

        public async Task CreateContainer(string containerName)
		{
            // Create BlobContainerClient with the container name
			BlobContainerClient blobContainerClient = _blobClient.GetBlobContainerClient(containerName);

            // Create if the blob container doesn't exists, ignore if already exists
            await blobContainerClient.CreateIfNotExistsAsync(PublicAccessType.BlobContainer);
        }

        public async Task DeleteContainer(string containerName)
        {
			// Create BlobContainerClient with the container name
			BlobContainerClient blobContainerClient = _blobClient.GetBlobContainerClient(containerName);

            // Delete if blob container exists
            await blobContainerClient.DeleteIfExistsAsync();
        }
    }
}
