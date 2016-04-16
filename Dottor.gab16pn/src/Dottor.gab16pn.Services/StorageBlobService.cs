namespace Dottor.gab16pn.Services
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Configuration;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Blob;

    // This project can output the Class library as a NuGet Package.
    // To enable this option, right-click on the project and select the Properties menu item. In the Build tab select "Produce outputs on build".
    public class StorageBlobService: IStorageBlobService
    {
        CloudBlobClient blobClient;
        public StorageBlobService(IConfigurationRoot config)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(config.Get<string>("AzureStorageConnectionString"));
            blobClient = storageAccount.CreateCloudBlobClient();

            CreateContainers().Wait();
        }

        private async Task CreateContainers()
        {
            var containerBase = blobClient.GetContainerReference("gab16pn");
            await containerBase.CreateIfNotExistsAsync();

            var containerInput = blobClient.GetContainerReference("gab16pn-input");
            await containerInput.CreateIfNotExistsAsync();
        }

        /// <summary>
        /// Restituisce tutti i file contenuti in una cartella all'interno di un preciso container di Azure
        /// </summary>
        /// <param name="containerName">Nome del container</param>
        /// <param name="folderName">Nome di una cartella</param>
        public async Task<IEnumerable<IListBlobItem>> GetAllFilesAsync(string containerName, string folderName)
        {
            var container = blobClient.GetContainerReference(containerName);
            var directory = container.GetDirectoryReference(folderName);

            BlobContinuationToken continuationToken = null;
            string prefix = folderName;
            bool useFlatBlobListing = true;
            BlobListingDetails blobListingDetails = BlobListingDetails.All;
            int maxBlobsPerRequest = 25;
            List<IListBlobItem> blobs = new List<IListBlobItem>();
            do
            {
                var listingResult = await container.ListBlobsSegmentedAsync(prefix, useFlatBlobListing, blobListingDetails, maxBlobsPerRequest, continuationToken, null, null);
                continuationToken = listingResult.ContinuationToken;
                blobs.AddRange(listingResult.Results);
            }
            while (continuationToken != null);

            return blobs;
        }

        /// <summary>
        /// Caricamento di un file nel blob storage di Azure
        /// </summary>
        /// <param name="containerName">Nome del container</param>
        /// <param name="name">Nome del file</param>
        /// <param name="contentType">Content-type del file</param>
        /// <param name="content">Contenuto del file</param>
        public async Task UploadFromStreamAsync(string containerName, string name, string contentType, byte[] content)
        {
            var container = blobClient.GetContainerReference(containerName);
            //var directory = container.GetDirectoryReference(folderName);

            CloudBlockBlob blockBlob = container.GetBlockBlobReference(name);
            blockBlob.Properties.ContentType = contentType;
            await blockBlob.UploadFromByteArrayAsync(content, 0, content.Length);
        }
    }
}
