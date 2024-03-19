public interface IBatchService {
    public Task TriggerBookXVA(BookModel model);
    public Task DeleteJobIfExists(string jobId);
    public Task CreateJobIfNotExists(string jobId);
}