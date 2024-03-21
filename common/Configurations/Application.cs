using System.Text;
using Microsoft.Azure.Batch;

public class Application : ApplicationPackageReference {
    public string? Command { get; set; }
    public string GetCommand(string inputFile, string outputFile){
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.Append(Command);
        stringBuilder.Replace("<<INPUTFILE>>",inputFile);
        stringBuilder.Replace("<<OUTPUTFILE>>", outputFile);

        return stringBuilder.ToString();
    }
}