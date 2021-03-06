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
        public string blurContainerName = string.Empty;

        public AzureStorageHelper()
        {
            //this.connectionString = Startup.Configuration.GetConnectionString("AZURE_STORAGE_CONNECTION_STRING");
            //this.containerName = Startup.Configuration.GetConnectionString("AZURE_CONTAINER_NAME");
            //this.blurContainerName= Startup.Configuration.GetConnectionString("AZURE_BLURED_CONTAINER_NAME");

            this.connectionString = Startup.Configuration.GetSection("AzureSettings").GetValue<string>("AZURE_STORAGE_CONNECTION_STRING");    //Startup.Configuration.GetConnectionString("AZURE_STORAGE_CONNECTION_STRING");
            this.containerName = Startup.Configuration.GetSection("AzureSettings").GetValue<string>("AZURE_CONTAINER_NAME"); //Startup.Configuration.GetConnectionString("AZURE_CONTAINER_NAME");
            this.blurContainerName = Startup.Configuration.GetSection("AzureSettings").GetValue<string>("AZURE_BLURED_CONTAINER_NAME"); //Startup.Configuration.GetConnectionString("AZURE_BLURED_CONTAINER_NAME");

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

        public Uri GetServiceSasUriForBlurBlob(string blobName,
                                                   string storedPolicyName = null)
        {
            string connectionString = this.connectionString;
            string containerName = this.blurContainerName;

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
                await blob.UploadAsync(stream,true);
                //await blob.UploadAsync(stream);

            }
            catch(Exception ex)
            {
                //Delete the file from local storage after uplaoding it to Azure Storage
                //if (System.IO.File.Exists(savePath))
                //{
                //    System.IO.File.Delete(savePath);
                //}
            }
        }
    }
}
