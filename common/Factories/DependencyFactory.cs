using Azure.Storage;
using Azure.Storage.Blobs;
using Microsoft.Azure.Batch;
using Microsoft.Azure.Batch.Auth;
using Microsoft.Azure.Batch.Protocol;
using Microsoft.Extensions.Configuration;

public static class DependencyFactory
{

    public static BatchClient CreateBacthClient(IConfiguration configuration)
    {
        var config = new BatchClientConfig();
        configuration.GetSection("BatchClientConfig").Bind(config);

        var cred = new BatchSharedKeyCredentials(config.BatchAccountUrl, config.BatchAccountName, config.BatchAccountKey);
        return BatchClient.Open(cred);
    }

    public static IBatchService CreateBatchService(IConfiguration configuration)
    {
        var dictionary = new Dictionary<string, Application>();
        var applications = new List<Application>();
        configuration.GetSection("Applications").Bind(applications);
        applications.ForEach(app =>
        {
            dictionary.Add(app.ApplicationId, app);
        });

        var configs = new BatchServiceConfiguration();
        configuration.GetSection("BatchConfiguration").Bind(configs);


        var service = new BatchService(CreateBacthClient(configuration),
                            new StorageService(CreateBlobServiceClient(configuration)),
                            new BatchCommandService(dictionary),
                            configs);

        return service;
    }

    public static IStorageService CreateStorageService(IConfiguration configuration)
    {
        return new StorageService(CreateBlobServiceClient(configuration));
    }

    public static BlobServiceClient CreateBlobServiceClient(IConfiguration configuration)
    {
        var config = new StorageClientConfig();
        configuration.GetSection("StorageClientConfig").Bind(config);

        StorageSharedKeyCredential storageSharedKeyCredential = new(config.AccountName, config.AccountKey);
        BlobServiceClient blobServiceClient = new BlobServiceClient(
            new Uri($"https://{config.AccountName}.blob.core.windows.net"),
            storageSharedKeyCredential);

        return blobServiceClient;
    }
}

public class BatchClientConfig
{
    public string BatchAccountName { get; set; }
    public string BatchAccountKey { get; set; }
    public string BatchAccountUrl { get; set; }
}

public class StorageClientConfig
{
    public string AccountName { get; set; }
    public string AccountKey { get; set; }
}