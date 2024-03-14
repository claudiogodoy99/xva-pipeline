public interface IBatchService {
    public Task TriggerBookXVA(string fileName,string outPutFileName,string appId);
    public Task DeleteJobIfExists(string jobId);
    public Task CreateJobIfNotExists(string jobId);
}