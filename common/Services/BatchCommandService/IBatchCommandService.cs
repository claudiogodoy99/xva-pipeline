using Microsoft.Azure.Batch;
public interface IBatchCommandService{
    string GetCommand(string inputFile, string outputFile, string appId);
    ApplicationPackageReference GetApp(string appId);
}