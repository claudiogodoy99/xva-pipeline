using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using System.ComponentModel;

public class StorageService : IStorageService
{
    private readonly BlobServiceClient _blobServiceClient;

    public StorageService(BlobServiceClient blobServiceClient)
    {
        _blobServiceClient = blobServiceClient;
    }

    public string CreateSASUri(string containerName)
    {
        BlobContainerClient containerClient = GetBlobContainerClient(containerName);

        if (!containerClient.CanGenerateSasUri) return string.Empty;


        BlobSasBuilder sasBuilder = new BlobSasBuilder()
        {
            BlobContainerName = containerName,
            Resource = "c",
            ExpiresOn = DateTimeOffset.UtcNow.AddDays(1)
        };

        sasBuilder.SetPermissions(BlobContainerSasPermissions.All);

        Uri sasURI = containerClient.GenerateSasUri(sasBuilder);

        return sasURI.ToString();
    }

    public async Task UploadSync(string containerName, string blobFileName, Stream stream)
    {
        var blobClient = CreateBlobClient(containerName, blobFileName);

        await blobClient.UploadAsync(stream);
    }

    public async Task CleanContainer(string containerName)
    {
        var blobContainerClient = GetBlobContainerClient(containerName);

        await blobContainerClient.DeleteAsync();
        await Task.Delay(1000 * 30);
        await blobContainerClient.CreateAsync();
    }

    public async Task<bool> BlobExist(string containerName, string blobFileName)
    {
        var blobClient = CreateBlobClient(containerName, blobFileName);

        return await blobClient.ExistsAsync();
    }

    private BlobContainerClient GetBlobContainerClient(string containerName) => _blobServiceClient.GetBlobContainerClient(containerName);

    private BlobClient CreateBlobClient(string containerName, string blobFileName){
        BlobContainerClient containerClient = GetBlobContainerClient(containerName);
        BlobClient blobClient = containerClient.GetBlobClient(blobFileName);

        return blobClient;
    }
}