using Microsoft.Extensions.Configuration;

var bookName = string.Empty;
var bookType = string.Empty;
var fileExtension = string.Empty;

var bookNameIndex = Array.IndexOf(args, "--bookname");
if (bookNameIndex == -1) throw new ArgumentException("input is required");
else bookName = args[bookNameIndex + 1];

var bookTypeIndex = Array.IndexOf(args, "--booktype");
if (bookTypeIndex == -1) throw new ArgumentException("type is required");
else bookType = args[bookTypeIndex + 1];

var fileExtensionIndex = Array.IndexOf(args, "--filetype");
if (fileExtensionIndex == -1) throw new ArgumentException("extension is required");
else fileExtension = args[fileExtensionIndex + 1];

var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();


const string containerName = "xva-inputs";
var client = new HttpClient();
client.BaseAddress = new Uri("https://pokeapi.co/api/v2/");
var service = new RandomApiService(client);
var storageService = DependencyFactory.CreateStorageService(configuration);

string fileName = $"{bookName}-{DateTime.Now.ToString("ddMMyyyy")}.{fileExtension}";

Console.WriteLine($"File name for the book: {fileName}");


Console.WriteLine($"Checking if the file already exists");
if (await storageService.BlobExist(containerName, fileName)) throw new Exception("File Already Exists");

Console.WriteLine($"Generating input file");
var stream = await service.GetStream();


Console.WriteLine($"Uplading the blob");
await storageService.UploadSync(containerName, fileName, stream);


Console.WriteLine($"Uploading done...");

Console.WriteLine($"to schedule the bacth run: dotnet run {fileName} OUTPUTFILENAME xva_moc");