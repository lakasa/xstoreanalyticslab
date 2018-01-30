using System;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

public static void Run(TimerInfo myTimer, TraceWriter log)
{
    log.Info($"C# Timer trigger function executed at: {DateTime.Now}");

    //Enter container name
    string targetContainer = System.Environment.GetEnvironmentVariable("ArchivalDataContainer");

    // Retrieve storage account from connection string.
    CloudStorageAccount storageAccount = CloudStorageAccount.Parse(System.Environment.GetEnvironmentVariable("DataConnection"));
   
    // Create the blob client.
    CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

    // Retrieve reference to a previously created container.
    CloudBlobContainer container = blobClient.GetContainerReference(targetContainer);

    //Change the tier of each blob in the container to Archive
    foreach (IListBlobItem item in container.ListBlobs(null, true, BlobListingDetails.None))
    {
        CloudBlockBlob blob = (CloudBlockBlob)item;
        blob.SetStandardBlobTier(StandardBlobTier.Archive);
    }
}
