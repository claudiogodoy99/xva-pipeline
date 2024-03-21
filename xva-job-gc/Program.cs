
using Microsoft.Azure.Batch;
using Microsoft.Extensions.Configuration;

var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

var batchService = DependencyFactory.CreateBatchService(configuration);
var storage = DependencyFactory.CreateStorageService(configuration);

Console.WriteLine($"{DateTime.Now} - Info: Cleaning blob container xva-inputs");
await storage.CleanContainer("xva-inputs");

Console.WriteLine($"{DateTime.Now} - Info: Cleaning blob container xva-outputs");
await storage.CleanContainer("xva-outputs");

Console.WriteLine($"{DateTime.Now} - Info: Getting All Jobs");

var paged = batchService.GetAllJobs();
var tasks = new List<Task>();   

await paged.ForEachAsync(async job => {
    Console.WriteLine($"{DateTime.Now} - Info: Deleting Job {job.Id}");
    await batchService.DeleteJobIfExists(job.Id);
    //tasks.Add();
});

//Task.WaitAll(tasks.ToArray());