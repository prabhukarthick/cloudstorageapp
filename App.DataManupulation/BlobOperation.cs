using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.DataManupulation
{
    public class BlobOperation
    {
        public void UploadContent(string content, string filename)
        {
            string connString = ConfigurationManager.ConnectionStrings["AzureStorageAccount"].ConnectionString;
            string destContainer = ConfigurationManager.AppSettings["destContainer"];


            //Get reference to the storage account.
            
            CloudStorageAccount oCsa = CloudStorageAccount.Parse(connString);
            CloudBlobClient oCbc = oCsa.CreateCloudBlobClient();

            //Get reference to the container
            
            CloudBlobContainer blobContainer = oCbc.GetContainerReference(destContainer);

            //Creating the container if not exists.
            blobContainer.CreateIfNotExists();

            //string key = DateTime.UtcNow.ToString("yyyy-MM-dd-HH:mm:ss") + "-" + filename;
            string key = "PrabhuKarthick.csv";
            UploadFilesToBlob(blobContainer, key, filename, content);

          
        }

        private static void UploadFilesToBlob(CloudBlobContainer blobContainer, string key, string filepath, string content)
        {
           
            CloudBlockBlob b = blobContainer.GetBlockBlobReference(key);

            using (var stream = new MemoryStream(Encoding.Default.GetBytes(content), false))
            {
                b.UploadFromStream(stream);
            }
        }

    }
}
