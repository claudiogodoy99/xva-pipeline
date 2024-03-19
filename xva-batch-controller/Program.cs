using Microsoft.Extensions.Configuration;

var fileName = string.Empty;
var outPutFileName = string.Empty;
var appId = string.Empty;
bool deletePrevius = false; ;


var fileNameIndex = Array.IndexOf(args, "--input");
if (fileNameIndex == -1) throw new ArgumentException("input is required");
else fileName = args[fileNameIndex + 1];


var outPutFileNameIndex = Array.IndexOf(args, "--output");
if (outPutFileNameIndex == -1) throw new ArgumentException("output is required");
else outPutFileName = args[outPutFileNameIndex + 1];

var appIdIndex = Array.IndexOf(args, "--app");
if (appIdIndex == -1) throw new ArgumentException("App is required");
else appId = args[appIdIndex + 1];

var deletePIdenx = Array.IndexOf(args, "--delete-previus");
if (appIdIndex != -1) deletePrevius = true;



var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();


Console.WriteLine("Initializing Service");

var batchService = DependencyFactory.CreateBatchService(configuration);

var model = new BookModel(fileName, outPutFileName, appId);

if(deletePrevius) {
    await batchService.DeleteJobIfExists(model.JobIdFromFileName());
    Thread.Sleep(1000 * 60 * 2);
    Console.WriteLine("Deleting previous Job if it exists");
}

await batchService.TriggerBookXVA(model);

Console.WriteLine("Press any key to finish...");
Console.ReadKey();

