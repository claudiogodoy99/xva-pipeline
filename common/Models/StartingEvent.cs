public class StartingEvent
{
    public string BookName { get; set; }
    public string BookType { get; set; }
    public string FileExtension { get; set; }

    // Constructor
    public StartingEvent(string bookName, string bookType, string fileExtension)
    {
        BookName = bookName;
        BookType = bookType;
        FileExtension = fileExtension;
    }

    public string FileName => $"{BookName}-{DateTime.Now.ToString("ddMMyyyy")}.{FileExtension}";
}


