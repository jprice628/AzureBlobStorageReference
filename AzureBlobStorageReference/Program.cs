using Microsoft.WindowsAzure.Storage;
using Serilog;
using System;
using System.IO;
using System.Reflection;

namespace AzureBlobStorageReference
{
    class Program
    {
        // TODO: Enter your connection string here.
        private const string StorageConnectionString = "";
        private const string ContainerName = "testcontainer";

        private static readonly string BlobName = "testblob-" + Guid.NewGuid().ToString() + ".txt";

        private static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.Console()
                .CreateLogger();

            try
            {
                Log.Information("StorageConnectionString: {StorageConnectionString}", StorageConnectionString);
                Log.Information("ContainerName:           {ContainerName}", ContainerName);
                Log.Information("BlobName:                {BlobName}", BlobName);

                Log.Information("Writing data to blob...");
                WriteDataToBlob();

                Log.Information("Reading data from blob...");
                ReadDataFromBlob();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "An error has occurred.");
            }
        }

        private static void WriteDataToBlob()
        {
            Log.Information("Parsing storage connection string...");
            if (!CloudStorageAccount.TryParse(StorageConnectionString, out CloudStorageAccount storageAccount))
            {
                throw new Exception("Unable to parse the storage connection string.");
            }

            Log.Information("Getting blob reference...");
            var client = storageAccount.CreateCloudBlobClient();
            var container = client.GetContainerReference(ContainerName);
            var blob = container.GetBlockBlobReference(BlobName);

            Log.Information("Uploading from stream...");
            using (var lipsumStream = CreateLipsumStream())
            {
                blob.UploadFromStream(lipsumStream);
            }
        }

        private static Stream CreateLipsumStream()
        {
            const string EmbeddedResourceName = "AzureBlobStorageReference.Lipsum.txt";
            return Assembly
                .GetExecutingAssembly()
                .GetManifestResourceStream(EmbeddedResourceName);
        }

        private static void ReadDataFromBlob()
        {
            Log.Information("Parsing storage connection string...");
            if (!CloudStorageAccount.TryParse(StorageConnectionString, out CloudStorageAccount storageAccount))
            {
                throw new Exception("Unable to parse the storage connection string.");
            }

            Log.Information("Getting blob reference...");
            var client = storageAccount.CreateCloudBlobClient();
            var container = client.GetContainerReference(ContainerName);
            var blob = container.GetBlockBlobReference(BlobName);

            Log.Information("Downloading text...");
            // For some reason an extra character appears on the front of the 
            // string, so lets only display the first twenty characters after 
            // the extra one.
            var text = blob.DownloadText().Trim().Substring(1, 20) + "...";

            Log.Information(text);
        }
    }
}
