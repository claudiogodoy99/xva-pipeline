using Azure;
using Azure.Messaging.EventHubs.Consumer;

var builder = WebApplication.CreateBuilder(args);


builder
    .Services
    .AddSingleton(DependencyFactory.CreateEventHubProcessor(builder.Configuration));

builder
    .Services
    .AddSingleton(DependencyFactory.CreateFileEventConsumerService(builder.Configuration));

builder
         .Services
         .Configure<HostOptions>(options =>
         {
             options.ShutdownTimeout = TimeSpan.FromSeconds(30);
         });
builder
    .Services
    .AddHostedService<EventProcessorClientService>();

var app = builder.Build();
app.MapGet("/", () => "Hello World!");
app.Run();