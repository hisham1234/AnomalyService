using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using Azure.Storage.Sas;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace AnomalyService.Helpers
{
    public class AzureStorageHelper
    {
        public IConfiguration Config { get; }

        public string connectionString = string.Empty;
        public string containerName = string.Empty;

        public AzureStorageHelper()
        {
            this.connectionString = Startup.Configuration.GetConnectionString("AzureStorageConnectionString");
            this.containerName = Startup.Configuration.GetConnectionString("AzureContainerName");
            BlobServiceClient blobServiceClient = new BlobServiceClient(this.connectionString);
        }

        public async Task<List<Uri>> ListAsync()
        {
            string connectionString = this.connectionString;
            string containerName = this.containerName;
            List<Uri> names = new List<Uri>();


            BlobContainerClient container = new BlobContainerClient(connectionString, containerName);

            try
            {
                await foreach (BlobItem blob in container.GetBlobsAsync())
                {
                    names.Add(GetServiceSasUriForBlob(blob.Name));
                }

                return names;
            }
            finally
            {

            }

        }

        public Uri GetServiceSasUriForBlob(string blobName,
                                                    string storedPolicyName = null)
        {
            string connectionString = this.connectionString;
            string containerName = this.containerName;

            BlobClient blobClient = new BlobClient(connectionString, containerName, blobName);



            if (blobClient.CanGenerateSasUri)
            {
                BlobSasBuilder sasBuilder = new BlobSasBuilder()
                {
                    BlobContainerName = blobClient.GetParentBlobContainerClient().Name,
                    BlobName = blobClient.Name,
                };

                if (storedPolicyName == null)
                {
                    sasBuilder.ExpiresOn = DateTimeOffset.UtcNow.AddHours(1);
                    sasBuilder.SetPermissions(BlobSasPermissions.Read |
                        BlobSasPermissions.Write);
                }
                else
                {
                    sasBuilder.Identifier = storedPolicyName;
                }

                Uri sasUri = blobClient.GenerateSasUri(sasBuilder);

                return sasUri;
            }
            else
            {
                return null;
            }
        }


        public async Task UploadAsync(string blobName, string savePath, Stream stream)
        {
            string connectionString = this.connectionString;
            string containerName = this.containerName;


            BlobContainerClient container = new BlobContainerClient(connectionString, containerName);

            try
            {
                // Get a reference to a blob
                BlobClient blob = container.GetBlobClient(blobName);

                // Upload file data
                await blob.UploadAsync(savePath);
                //await blob.UploadAsync(stream);

            }
            finally
            {
                //Delete the file from local storage after uplaoding it to Azure Storage
                if (System.IO.File.Exists(savePath))
                {
                    System.IO.File.Delete(savePath);
                }
            }
        }
    }
}
