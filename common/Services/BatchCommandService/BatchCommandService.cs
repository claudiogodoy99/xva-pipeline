using Microsoft.Azure.Batch;

public class BatchCommandService : IBatchCommandService
{

    private readonly Dictionary<string, Application> _applications;
    private const string noneService = "none";
    private const string command = @"/bin/sh env > output.txt";

    public BatchCommandService(Dictionary<string, Application> applications)
    {
        _applications = applications;
    }
    public ApplicationPackageReference GetApp(string appId)
    {
        var app = SearchApp(appId);
        //TODO: Tirar essa alocação desnecessária
        return new ApplicationPackageReference()
        {
            ApplicationId = app.Value.ApplicationId,
            Version =  app.Value.Version
        };
    }

    public string GetCommand(string inputFile, string outputFile, string appId)
    {
        if (appId == noneService) return command;
        var app = SearchApp(appId);

        return app.Value.GetCommand(inputFile, outputFile);
    }
    
    private KeyValuePair<string, Application> SearchApp(string appId)
    {
        var app = _applications.FirstOrDefault(x => x.Key == appId);
        if (app.Value == null) throw new Exception("AppId not Found");

        return app;
    }
}