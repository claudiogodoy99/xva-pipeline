using System.Data;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Azure.Batch;
using Microsoft.Azure.Batch.Common;

public class BatchService : IBatchService
{
  
    private readonly BatchServiceConfiguration _configuration;
    private readonly BatchClient _batchClient;
    private readonly IStorageService _storageService;
    private readonly IBatchCommandService _batchCommandService;

    public BatchService(BatchClient batchClient,
        IStorageService storageService,
        IBatchCommandService batchCommandService,
        BatchServiceConfiguration configuration)
    {
        _batchClient = batchClient;
        _storageService = storageService;
        _batchCommandService = batchCommandService;
        _configuration = configuration;
    }

    public async Task TriggerBookXVA(string fileName, string outPutFileName, string appId)
    {
        var jobId = JobIdFromFileName(fileName);

        Console.WriteLine($"Triggering XVA Book for input file: {fileName}");

        await CreateJobIfNotExists(jobId);
        var task = CreateTask(fileName, jobId, outPutFileName,appId);

        await _batchClient.JobOperations.AddTaskAsync(jobId, task);

        Console.WriteLine($"Task: {task.Id}, Triggered on Job: {fileName}");
    }

    public async Task CreateJobIfNotExists(string jobId)
    {

        try
        {
            CloudJob job = _batchClient.JobOperations.CreateJob();
            job.Id = jobId;
            job.PoolInformation = new PoolInformation { PoolId = _configuration.PoolId };
            await job.CommitAsync();
        }
        catch (BatchException be)
        {
            if (be.RequestInformation?.BatchError?.Code == BatchErrorCodeStrings.JobExists)
                Console.WriteLine("The job {0} already existed when we tried to create it", jobId);
            else throw;
            
        }
    }

    public async Task DeleteJobIfExists(string jobId)
    {
        try
        {
            await _batchClient.JobOperations.DeleteJobAsync(jobId);
        }   
        catch (BatchException ex)
        {
            if (ex.RequestInformation?.HttpStatusCode == System.Net.HttpStatusCode.NotFound) return;
        }

    }

    private CloudTask CreateTask(string inputFileName, string jobId,string outPutFileName,string appId)
    {
        var inputFile = ResourceFile.FromAutoStorageContainer(_configuration.ContainerName, filePath: inputFileName);
        
        var outputFile = new OutputFile(
                                filePattern: outPutFileName,
                                destination: new OutputFileDestination(new OutputFileBlobContainerDestination(
                                    containerUrl: _storageService.CreateSASUri(_configuration.OutputContainerName),
                                    path: $"{jobId}-{outPutFileName}")),
                                uploadOptions: new OutputFileUploadOptions(
                                    uploadCondition: OutputFileUploadCondition.TaskCompletion));

        var task = new CloudTask(_configuration.TaskId, _batchCommandService.GetCommand(inputFileName,outPutFileName,appId))
        {
            ResourceFiles = new List<ResourceFile> { inputFile },
            ApplicationPackageReferences = new List<ApplicationPackageReference> { _batchCommandService.GetApp(appId) },
            OutputFiles = new List<OutputFile> { outputFile },
        };

        return task;
    }

    private string JobIdFromFileName(string fileName)
    {
        var strs = fileName.Split('.');
        if (strs.Length < 2) throw new Exception("Invalid Input");

        return strs[0];
    }

}