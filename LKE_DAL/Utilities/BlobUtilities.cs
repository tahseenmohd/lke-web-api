using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace LKEWebAPI.Utilities
{
    public class BlobUtilities
    {
        public static CloudBlobClient GetBlobClient
        {
            get
            {
                var blobConnectionString = ConfigurationManager.AppSettings["BlobStorageConnectionString"];
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(blobConnectionString);
                CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
                return blobClient;
            }
        }
    }
}