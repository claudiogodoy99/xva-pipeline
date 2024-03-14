using System.Net.Http;

public class RandomApiService
{
    private readonly HttpClient _httpClient;
    public RandomApiService(HttpClient httpClient)
    {
            _httpClient = httpClient;
    }

    public async Task<Stream> GetStream()
    {

        var result = await _httpClient.GetAsync("pokemon/pikachu");

        return result.Content.ReadAsStream();
    }
}
