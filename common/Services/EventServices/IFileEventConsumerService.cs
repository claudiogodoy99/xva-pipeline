public interface IFileEventConsumerService
{
    Task ConsumeEvent(StartingEvent trigger);
}

