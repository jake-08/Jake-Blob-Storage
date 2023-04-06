using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using AzureBlobProject.Models;

namespace AzureBlobProject.Services
{
    public class BlobService : IBlobService
    {
        private readonly BlobServiceClient _blobServiceClient;

        public BlobService(BlobServiceClient blobServiceClient)
        {
            _blobServiceClient = blobServiceClient;
        }

        public async Task<List<string>> GetAllBlobs(string containerName)
        {
			// Create BlobContainerClient to access blobs in the container
			BlobContainerClient blobContainerClient = _blobServiceClient.GetBlobContainerClient(containerName);

            // Create all the blobs from that container
            var blobs = blobContainerClient.GetBlobsAsync();

			// Create the list of blob strings
			List<string> blobString = new List<string>();

            // Add all the blobs urls to the blobString List
            await foreach (var item in blobs)
            {
                blobString.Add(item.Name);
            }

            return blobString;
        }

        public async Task<List<Blob>> GetAllBlobsWithUri(string containerName)
        {
            List<Blob> blobList = new List<Blob>();

            // Create BlobContainerClient to access blobs in the container
            BlobContainerClient blobContainerClient = _blobServiceClient.GetBlobContainerClient(containerName);

            bool isExists = blobContainerClient.Exists();
            if (!isExists)
            {
                throw new Exception();
            }

            // Create all the blobs from that container
            var blobs = blobContainerClient.GetBlobsAsync();

            // Generate Sas uri for private containers
            string sasContainerSignature = "";

            if (blobContainerClient.CanGenerateSasUri)
            {
                BlobSasBuilder sasBuilder = new()
                {
                    BlobContainerName = blobContainerClient.Name,
                    Resource = "c",
                    ExpiresOn = DateTimeOffset.UtcNow.AddHours(1)
                };

                sasBuilder.SetPermissions(BlobSasPermissions.Read);

                // Generate new uri with Sas token
                sasContainerSignature = blobContainerClient.GenerateSasUri(sasBuilder).AbsoluteUri.Split('?')[1].ToString();
            }

            await foreach (var item in blobs)
            {
                var blobClient = blobContainerClient.GetBlobClient(item.Name);

                Blob blobIndividual = new()
                {
                    Uri = blobClient.Uri.AbsoluteUri + "?" + sasContainerSignature,
                };

                // Generate Sas uri for private blobs
                //if (blobClient.CanGenerateSasUri)
                //{
                //    BlobSasBuilder sasBuilder = new()
                //    {
                //        BlobContainerName = blobClient.GetParentBlobContainerClient().Name,
                //        BlobName = blobClient.Name,
                //        Resource = "b",
                //        ExpiresOn = DateTimeOffset.UtcNow.AddHours(1)
                //    };

                //    sasBuilder.SetPermissions(BlobSasPermissions.Read);

                //    // Generate new uri with Sas token
                //    blobIndividual.Uri = blobClient.GenerateSasUri(sasBuilder).AbsoluteUri;
                //}

                BlobProperties blobProperties = await blobClient.GetPropertiesAsync();

                if (blobProperties.Metadata.ContainsKey("title"))
                {
                    blobIndividual.Title = blobProperties.Metadata["title"];
                }
                if (blobProperties.Metadata.ContainsKey("comment"))
                {
                    blobIndividual.Comment = blobProperties.Metadata["comment"];
                }

                blobList.Add(blobIndividual);
            }

            return blobList;
        }

        public async Task<string> GetBlob(string name, string containerName)
        {
			// Create BlobContainerClient object to access blobs in the container
			BlobContainerClient blobContainerClient = _blobServiceClient.GetBlobContainerClient(containerName);

            // Create BlobClient object based on the name of the blob
            BlobClient blobClient = blobContainerClient.GetBlobClient(name);

            // Return the entire uri of blob
            return blobClient.Uri.AbsoluteUri;
        }

        public async Task<bool> UploadBlob(string name, IFormFile file, string containerName, Blob blob)
        {
			// Create BlobContainerClient object to access blobs in the container
			BlobContainerClient blobContainerClient = _blobServiceClient.GetBlobContainerClient(containerName);

			// Create BlobClient object based on the name of the blob
			BlobClient blobClient = blobContainerClient.GetBlobClient(name);

            var httpHeaders = new BlobHttpHeaders()
            {
                ContentType = file.ContentType
            };

            // Add metadata for blobs 
            IDictionary<string, string> metadata = new Dictionary<string, string>();
            metadata.Add("title", blob.Title);
            if(blob.Comment != null)
            {
                metadata["comment"] = blob.Comment;
            }

            // Upload a new Blob, overides if exists
            var result = await blobClient.UploadAsync(file.OpenReadStream(), httpHeaders, metadata);  

            if (result != null)
            {
                return true;
            }

            return false;
        }

        public async Task<bool> DeleteBlob(string name, string containerName)
        {
			// Create BlobContainerClient object to access blobs in the container
			BlobContainerClient blobContainerClient = _blobServiceClient.GetBlobContainerClient(containerName);

			// Create BlobClient object based on the name of the blob
			BlobClient blobClient = blobContainerClient.GetBlobClient(name);

            return await blobClient.DeleteIfExistsAsync();
        }
    }
}
