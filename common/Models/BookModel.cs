public class BookModel
{
    public string FileName { get; set; }
    public string OutputFileName { get; set; }
    public string AppId { get; set; }

    public BookModel(string fileName, string outputFileName, string appId)
    {
        FileName = fileName;
        OutputFileName = outputFileName;
        AppId = appId;
    }

    public string JobIdFromFileName()
    {
        var strs = FileName.Split('.');
        if (strs.Length < 2) throw new Exception("Invalid Input");

        return strs[0];
    }

}

