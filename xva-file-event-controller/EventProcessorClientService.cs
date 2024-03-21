using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Consumer;
using Azure.Messaging.EventHubs.Processor;

public class EventProcessorClientService : IHostedService
{
    private readonly ILogger<EventProcessorClientService> _logger;
    private readonly EventProcessorClient _processorClient;
    private readonly IFileEventConsumerService _applicationProcessor;

    public EventProcessorClientService(
        ILogger<EventProcessorClientService> logger,
        EventProcessorClient processorClient,
        IFileEventConsumerService applicationProcessor)
    {
        _logger = logger;
        _processorClient = processorClient;
        _applicationProcessor = applicationProcessor;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
       _processorClient.PartitionInitializingAsync += InitializeEventHandler;
        _processorClient.ProcessEventAsync += ProcessEventHandler;
        _processorClient.ProcessErrorAsync += ProcessErrorHandler;

        await _processorClient.StartProcessingAsync(cancellationToken).ConfigureAwait(false);
    }

    private Task InitializeEventHandler(PartitionInitializingEventArgs args)
    {
        try
        {

            if (args.CancellationToken.IsCancellationRequested)
            {
                return Task.CompletedTask;
            }

            args.DefaultStartingPosition = EventPosition.Latest;
        }
        catch
        {
            _logger.LogError($"{DateTime.Now} Error - Error on Initializing the service");
        }

        return Task.CompletedTask;
    }

    private Task ProcessEventHandler(ProcessEventArgs args)
    {
        try
        {
            if (args.CancellationToken.IsCancellationRequested)
            {
                return Task.CompletedTask;
            }
            _logger.LogInformation($"{DateTime.Now} - Processing Event {args.Data.SequenceNumber}");

            var body = args.Data.EventBody.ToObjectFromJson<StartingEvent>(); ;
            _applicationProcessor.ConsumeEvent(body);
        }
        catch
        {
            _logger.LogError($"{DateTime.Now} Error - Error on Processor");
        }

        return Task.CompletedTask;
    }

    private Task ProcessErrorHandler(ProcessErrorEventArgs args)
    {
        if (args.CancellationToken.IsCancellationRequested)
        {
            return Task.CompletedTask;
        }
        _logger.LogError(args.Exception, "Error in the EventProcessorClient \tOperation: {Operation}", args.Operation);

        return Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        try
        {
            await _processorClient.StopProcessingAsync(cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            _processorClient.PartitionInitializingAsync -= InitializeEventHandler;
            _processorClient.ProcessEventAsync -= ProcessEventHandler;
            _processorClient.ProcessErrorAsync -= ProcessErrorHandler;
        }
    }
}

