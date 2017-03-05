using Microsoft.Azure;
using Microsoft.Azure.Management.DataFactories;
using Microsoft.Azure.Management.DataFactories.Common.Models;
using Microsoft.Azure.Management.DataFactories.Models;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataFactorySdk
{
    class DataFactoryProcessEngine
    {
        public static string ActiveDirectoryEndpoint = "https://login.windows.net/";
        public static string ResourceManagerEndpoint = "https://management.azure.com";
        public static string WindowsManagementUri = "https://management.core.windows.net/";
        public static string ApplicationId = "<<Target App id >>";
        public static string Password = "Pass@word1";
        public static string SubscriptionId = "<<Sub Id>>";
        public static string ActiveDirectoryTenantId = "<<Tenant id>>";
        static string resourceGroupName = "Prabhurg";
        static string dataFactoryName = "PrabhuTestFactory01";
        string Dataset_Source = "DatasetBlobSource";
        string Dataset_Destination = "DatasetAzureSqlDestination";
        public void initateProcess()
        {
            // create data factory management client  access       

            TokenCloudCredentials aadTokenCredentials =
            new TokenCloudCredentials(
            SubscriptionId,
            GetRequiredTokens());

            Uri resourceManagerUri = new Uri(ResourceManagerEndpoint);

            DataFactoryManagementClient client = new DataFactoryManagementClient(aadTokenCredentials, resourceManagerUri);
            CreateDataFactory(client);


            // create a linked service for input data store: Azure Storage
            
            client.LinkedServices.CreateOrUpdate(resourceGroupName, dataFactoryName,
                new LinkedServiceCreateOrUpdateParameters()
                {
                    LinkedService = new LinkedService()
                    {
                        Name = "AzureStorageLinkedService",
                        Properties = new LinkedServiceProperties
                        (
                            new AzureStorageLinkedService("DefaultEndpointsProtocol=https;AccountName=prabhublob;AccountKey=+37kXwkYRQAt+hjS+roY6v5BQ1Bgd7o/ftmLNzg6VF613nSTGhxKpvyq5nUwJAO7RP/I00PAauGtpPWbC/3HaA==;BlobEndpoint=https://prabhublob.blob.core.windows.net/")
                        )
                    }
                }
            );

            // create a linked service for output data store: Azure SQL Database
           
            client.LinkedServices.CreateOrUpdate(resourceGroupName, dataFactoryName,
                new LinkedServiceCreateOrUpdateParameters()
                {
                    LinkedService = new LinkedService()
                    {
                        Name = "AzureSqlLinkedService",
                        Properties = new LinkedServiceProperties
                        (
                            new AzureSqlDatabaseLinkedService("Data Source=prabhusqlserver.database.windows.net;Initial Catalog=prabhusqldb;Integrated Security=False;User ID=Prabhu;Password=Admin@123.;Connect Timeout=30;Encrypt=True")
                        )
                    }
                }
            );

            CreateInputDataSet(client);
            CreateOutputDataSet(client);
            CreatePipeLineOperation(client);
            //End of datafactory
        }

        private void CreatePipeLineOperation(DataFactoryManagementClient client)
        {         
           
            DateTime PipelineActivePeriodStartTime = new DateTime(2017, 3, 4, 0, 0, 0, 0, DateTimeKind.Utc);
            DateTime PipelineActivePeriodEndTime = PipelineActivePeriodStartTime.AddMinutes(60);
            string PipelineName = "ADFTutorialPipeline";

            client.Pipelines.CreateOrUpdate(resourceGroupName, dataFactoryName,
                new PipelineCreateOrUpdateParameters()
                {
                    Pipeline = new Pipeline()
                    {
                        Name = PipelineName,
                        Properties = new PipelineProperties()
                        {
                            Description = "Demo Pipeline for data transfer between blobs",

                            // Initial value for pipeline's active period. With this, you won't need to set slice status
                            Start = PipelineActivePeriodStartTime,
                            End = PipelineActivePeriodEndTime,

                            Activities = new List<Activity>()
                            {
                                new Activity()
                                {
                                    Name = "BlobToAzureSql",
                                    Inputs = new List<ActivityInput>()
                                    {
                                        new ActivityInput() {
                                            Name = Dataset_Source
                                        }
                                    },
                                    Outputs = new List<ActivityOutput>()
                                    {
                                        new ActivityOutput()
                                        {
                                            Name = Dataset_Destination
                                        }
                                    },
                                    TypeProperties = new CopyActivity()
                                    {
                                        Source = new BlobSource(),
                                        Sink = new BlobSink()
                                        {
                                            WriteBatchSize = 10000,
                                            WriteBatchTimeout = TimeSpan.FromMinutes(10)
                                        }
                                    }
                                }
                            },
                        }
                    }
                });

        }
       
        private void CreateOutputDataSet(DataFactoryManagementClient client)
        {
            
            client.Datasets.CreateOrUpdate(resourceGroupName, dataFactoryName,
                new DatasetCreateOrUpdateParameters()
                {
                    Dataset = new Dataset()
                    {
                        Name = Dataset_Destination,
                        Properties = new DatasetProperties()
                        {
                            Structure = new List<DataElement>()
                            {
                                new DataElement() { Name = "Id", Type = "String" },
                                new DataElement() { Name = "client", Type = "String" },
                                new DataElement() { Name = "groupName", Type = "String" },
                                new DataElement() { Name = "groupDisplayName", Type = "String" },
                                new DataElement() { Name = "page", Type = "String" },
                                new DataElement() { Name = "displayName", Type = "String" },
                                new DataElement() { Name = "thumbnailUrl", Type = "String" },
                                new DataElement() { Name = "reportState", Type = "String" }
                            },
                            LinkedServiceName = "AzureSqlLinkedService",
                            TypeProperties = new AzureSqlTableDataset()
                            {
                                TableName = "emp"
                            },

                            Availability = new Availability()
                            {
                                Frequency = SchedulePeriod.Hour,
                                Interval = 1,
                            },
                        }
                    }
                });
        }

       
        private void CreateInputDataSet(DataFactoryManagementClient client)
        {           
            
            client.Datasets.CreateOrUpdate(resourceGroupName, dataFactoryName,

            new DatasetCreateOrUpdateParameters()
            {
                Dataset = new Dataset()
                {
                    Name = Dataset_Source,
                    Properties = new DatasetProperties()
                    {
                        Structure = new List<DataElement>()
                        {     
                         new DataElement() { Name = "Id", Type = "String" },
                         new DataElement() { Name = "client", Type = "String" },
                         new DataElement() { Name = "groupName", Type = "String" },
                         new DataElement() { Name = "groupDisplayName", Type = "String" },
                         new DataElement() { Name = "page", Type = "String" },
                         new DataElement() { Name = "displayName", Type = "String" },
                         new DataElement() { Name = "thumbnailUrl", Type = "String" },
                         new DataElement() { Name = "reportState", Type = "String" }
                        },
                        LinkedServiceName = "AzureStorageLinkedService",
                        TypeProperties = new AzureBlobDataset()
                        {
                            FolderPath = "demoblobs/",
                            FileName = "Prabhukarthick.csv"
                        },
                        External = true,
                        Availability = new Availability()
                        {
                            Frequency = SchedulePeriod.Hour,
                            Interval = 1,
                        },

                        Policy = new Policy()
                        {
                            Validation = new ValidationPolicy()
                            {
                                MinimumRows = 1
                            }
                        }
                    }
                }
            });
        }

        private static void CreateDataFactory(DataFactoryManagementClient client)
        {
            // create a data factory           
            client.DataFactories.CreateOrUpdate(resourceGroupName,
                new DataFactoryCreateOrUpdateParameters()
                {
                    DataFactory = new DataFactory()
                    {
                        Name = dataFactoryName,
                        Location = "westus",
                        Properties = new DataFactoryProperties() { }
                    }
                }
            );
        }

        private string GetRequiredTokens()
        {
            //https://login.microsoftonline.com/0acda6ed-2e2b-4833-9557-8ac2a983864b/oauth2/token
            //https://login.windows.net/0acda6ed-2e2b-4833-9557-8ac2a983864b/FederationMetadata/2007-06/FederationMetadata.xml
            //AuthenticationContext ac = new AuthenticationContext("https://login.microsoftonline.com/0acda6ed-2e2b-4833-9557-8ac2a983864b/oauth2/token");
            //ClientCredential cc = new ClientCredential("2bf44dca-4cc8-4df7-a023-2c8a07e95aed", "flqBdHtqfCjuMd4mc6cJ6W+75NHra+u7cQ/h2C7R1co=");
            //AuthenticationResult aresult = ac.AcquireTokenAsync("https://management.core.windows.net/", cc).Result;
            //return aresult.AccessToken;
            AuthenticationResult result = null;
            var context = new AuthenticationContext(ActiveDirectoryEndpoint + ActiveDirectoryTenantId + "/oauth2/token");

            ClientCredential credential = new ClientCredential(ApplicationId, Password);
            result = context.AcquireTokenAsync(WindowsManagementUri, credential).Result;
            return result.AccessToken;
        }
    }
}
