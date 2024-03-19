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

var evt = new StartingEvent(bookName,bookType, fileExtension);
var service = DependencyFactory.CreateFileEventConsumerService(configuration);

await service.ConsumeEvent(evt);