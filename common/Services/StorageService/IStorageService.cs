public interface IStorageService{
    public string CreateSASUri(string containerName);
    Task<bool> BlobExist(string containerName, string blobFileName);
    Task UploadSync(string containerName, string blobFileName, Stream stream);
    Task CleanContainer(string containerName);
}