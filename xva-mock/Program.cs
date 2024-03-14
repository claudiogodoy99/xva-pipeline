if (args.Length != 2)
{
	Console.WriteLine("Usage: dotnet run inputFile outputFile");
	return;
}

string inputFile = args[0];
string outputFile = args[1];

try
{
	// Read content from input file
	string content = File.ReadAllText(inputFile);

	// Write content to output file
	File.WriteAllText(outputFile, content);

	Console.WriteLine("Content copied successfully.");
}
catch (Exception ex)
{
	Console.WriteLine($"An error occurred: {ex.Message}");
}
