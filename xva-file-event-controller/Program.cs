using Azure;
using Azure.Messaging.EventHubs.Consumer;
using System.Diagnostics;

var connectionString = "<< CONNECTION STRING FOR THE EVENT HUBS NAMESPACE >>";
var eventHubName = "<< NAME OF THE EVENT HUB >>";
var consumerGroup = EventHubConsumerClient.DefaultConsumerGroupName;

var consumer = new EventHubConsumerClient(
    consumerGroup,
    connectionString,
    eventHubName);

try
{
    while (true)
    {
        await Task.Delay(1);
        await foreach (PartitionEvent partitionEvent in consumer.ReadEventsAsync())
        {
            string readFromPartition = partitionEvent.Partition.PartitionId;
            //byte[] eventBodyBytes = partitionEvent.Data.EventBody.ToObject<ob>;
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