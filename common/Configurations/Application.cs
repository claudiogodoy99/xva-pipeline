using System.Text;
using Microsoft.Azure.Batch;

public class Application :  ApplicationPackageReference {
    public string? Path {get; set;}
    public string GetCommand(string inputFile, string outputFile){
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.AppendFormat("/bin/sh -c  \'$AZ_BATCH_APP_PACKAGE_{0}_{1}/{2}", ApplicationId, Version, Path);
        stringBuilder.AppendFormat(" {0}/{1}",inputFile,inputFile);
        stringBuilder.AppendFormat(" {0}\'",outputFile);

        return stringBuilder.ToString();
    }
}