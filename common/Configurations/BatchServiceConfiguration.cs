public class BatchServiceConfiguration {
    public string PoolId {get;set;}
    public string Command {get;set;}
    public string TaskId {get;set;}
    public string ContainerName {get;set;}
    public string OutputContainerName {get;set;}

    public BatchServiceConfiguration()
    {
        PoolId = string.Empty;
        Command = string.Empty;
        TaskId = string.Empty;
        ContainerName = string.Empty;
        OutputContainerName = string.Empty;
    }
}