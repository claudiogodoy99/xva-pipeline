
public class FileEventConsumerService : IFileEventConsumerService
{
    private readonly IStorageService _storageService;
    private readonly RandomApiService _randomApiService;
    private readonly BatchServiceConfiguration _batchConfig;
    private IBatchApiService _batchApiService;

    public FileEventConsumerService(IStorageService storageService, BatchServiceConfiguration batchConfig, RandomApiService randomApiService, IBatchApiService batchApiService)
    {
        _storageService = storageService;
        _batchConfig = batchConfig;
        _randomApiService = randomApiService;
        _batchApiService = batchApiService;
    }

    public async Task ConsumeEvent(StartingEvent trigger)
    {
        Console.WriteLine($"{DateTime.Now} - Info: Checking if the file already exists {trigger.FileName}");
        if (await _storageService.BlobExist(_batchConfig.ContainerName, trigger.FileName)) throw new Exception("File Already Exists");

        Console.WriteLine($"{DateTime.Now} - Info: Generating input file {trigger.FileName}");
        var stream = await _randomApiService.GetStream();

        Console.WriteLine($"{DateTime.Now} - Info: Uplading the blob {trigger.FileName}");
        await _storageService.UploadSync(_batchConfig.ContainerName, trigger.FileName, stream);

        Console.WriteLine($"{DateTime.Now} - Info: Triggering XVA Book for {trigger.FileName}");
        var book = new BookModel(trigger.FileName, $"output{trigger.FileExtension}", trigger.BookType == "cpp" ? "xva_moc": "xva_moc");

        await _batchApiService.TriggerVXA(book);



    }
}

