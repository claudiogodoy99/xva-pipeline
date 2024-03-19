using System.Text.Json;

public class BatchApiService : IBatchApiService
{
    private readonly HttpClient _httpClient;

    public BatchApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task TriggerVXA(BookModel model)
    {
        string jsonBody = JsonSerializer.Serialize(model);


        HttpContent content = new StringContent(jsonBody, System.Text.Encoding.UTF8, "application/json");

        var result = await _httpClient.PostAsync("api/book", content);
    }
}