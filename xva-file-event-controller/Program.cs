using Azure;
using Azure.Messaging.EventHubs.Consumer;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;

var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();




var consumer = DependencyFactory.CreateEventHubConsumerClient(configuration);
var service = DependencyFactory.CreateFileEventConsumerService(configuration);

try
{
    while (true)
    {
        await Task.Delay(1);
        await foreach (PartitionEvent partitionEvent in consumer.ReadEventsAsync())
        {
            string readFromPartition = partitionEvent.Partition.PartitionId;
            var triggerEvent = partitionEvent.Data.EventBody.ToObjectFromJson<StartingEvent>();

            await service.ConsumeEvent(triggerEvent);
        }
    }
}
catch (TaskCanceledException)
{
    // This is expected if the cancellation token is
    // signaled.
}
finally
{
    await consumer.CloseAsync();
}

