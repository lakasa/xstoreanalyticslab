# Specify account information
$StorageAccountName = "<enter-info>"
$StorageAccountKey = "<enter-info>"
$StorageContainerName = "outputdata"
$DeletedDataPrefix = "data"
$StorageArchivalContainerName = "archivaldata"

# Create a context by specifying storage account name and key
$ctx = New-AzureStorageContext -StorageAccountName $StorageAccountName -StorageAccountKey $StorageAccountKey

# Turn on soft delete
Enable-AzureStorageDeleteRetentionPolicy -RetentionDays 7 -context $ctx

# Show soft delete is turned on
Get-AzureStorageServiceProperty -ServiceType "Blob" -context $ctx

# Get the blobs in the container and show their properties
$Blobs = Get-AzureStorageBlob -Container $StorageContainerName -Prefix $DeletedDataPrefix -Context $ctx -IncludeDeleted
$Blobs
$Blobs.ICloudBlob.Properties

# Undelete the blobs in the container and show their properties
$Blobs.ICloudBlob.Undelete()
$Blobs = Get-AzureStorageBlob -Container $StorageContainerName -Prefix $DeletedDataPrefix -Context $ctx
$Blobs

# Copy the blobs to a new container for archiving
$Blobs | Start-AzureStorageBlobCopy -DestContainer $StorageArchivalContainerName -Context $ctx