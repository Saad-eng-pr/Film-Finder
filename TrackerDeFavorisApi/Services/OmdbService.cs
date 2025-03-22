
using System.Text.Json;
using System.Text.Json.Serialization;
using TrackerDeFavorisApi.Models;

public class OmdbService 
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private const  string ApiBaseUrl = "http://www.omdbapi.com/";  

    public OmdbService(IConfiguration config){
        _apiKey = config["ApiKeys:OmdbApiKey"] ?? throw new ArgumentNullException("ApiKeys:OmdbApiKey");
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri(ApiBaseUrl)
        };
    }

    public async Task<IEnumerable<Film>> SearchByTitleAsync(string titre) {
        if (String.IsNullOrWhiteSpace(titre))
            return new List<Film>();

        var requestURL = $"?s={Uri.EscapeDataString(titre)}&type=movie&apikey={_apiKey}";

        var response = await _httpClient.GetAsync(requestURL);

        var JsonResponse = await response.Content.ReadAsStringAsync();

        var searchResult = JsonSerializer.Deserialize<OmdbSearchResponse>(JsonResponse);

        return searchResult?.Search ?? new List<Film>();
    }

    public async Task<Film> GetByImdbId(string imdbID)
    {
        if (String.IsNullOrWhiteSpace(imdbID))
            throw new ArgumentException("L'id Imdb ne peut pas etre vide!!");

        var requestURL = $"?i={Uri.EscapeDataString(imdbID)}&type=movie&apikey={_apiKey}";

        var response = await _httpClient.GetAsync(requestURL);

        var JsonResponse = await response.Content.ReadAsStringAsync();

        var film = JsonSerializer.Deserialize<Film>(JsonResponse);

        if(film == null) 
            throw new Exception("Film non trouvé");
        return film;
    }
}

public class OmdbSearchResponse {
    public List<Film>? Search { get; set; }
}

public class OmdbFilm {
    [JsonPropertyName("Title")]
    public  string? Title { get; set; }
    [JsonPropertyName("Year")]
    public string? Year { get; set; }
    [JsonPropertyName("imdbID")]
    public string? imdbID { get; set; }
    [JsonPropertyName("Type")]
    public string? Type { get; set;}
    [JsonPropertyName("Poster")]
    public string? Poster { get; set;}
}







