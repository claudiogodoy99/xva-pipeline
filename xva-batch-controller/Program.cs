using Microsoft.Extensions.Configuration;

var fileName = string.Empty;
var outPutFileName = string.Empty;
var appId = string.Empty;


var fileNameIndex = Array.IndexOf(args, "--input");
if (fileNameIndex == -1) throw new ArgumentException("input is required");
else fileName = args[fileNameIndex + 1];


var outPutFileNameIndex = Array.IndexOf(args, "--output");
if (outPutFileNameIndex == -1) throw new ArgumentException("output is required");
else outPutFileName = args[outPutFileNameIndex + 1];

var appIdIndex = Array.IndexOf(args, "--app");
if (appIdIndex == -1) throw new ArgumentException("App is required");
else appId = args[appIdIndex + 1];


var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();


Console.WriteLine("Initializing Service");

var batchService = DependencyFactory.CreateBatchService(configuration);

await batchService.DeleteJobIfExists("book_retail");

Thread.Sleep(1000 * 60 * 2);

Console.WriteLine("Deleting previous Job if it exists");

await batchService.TriggerBookXVA(fileName,outPutFileName,appId);

Console.WriteLine("Press any key to finish...");
Console.ReadKey();

