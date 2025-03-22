namespace NomDuFront.Services ; 
using NomDuFront.Models ; 
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Blazored.LocalStorage;



public class FavoriteService
{
    private readonly HttpClient _httpClient;
     private readonly ILocalStorageService _localStorageService;
     private readonly MovieService _movieService;

    public FavoriteService(HttpClient httpClient ,ILocalStorageService localStorageService, MovieService movieService )
    {
        _httpClient = httpClient;
        _localStorageService = localStorageService;
        _movieService = movieService;
    }

    //Pour avoir les FilmId 
   public async Task<List<int>> GetFavoriteMovieIds(int userId)
    {
        var response = await _httpClient.GetAsync($"api/Favoris/Get Favorite List?userId={userId}");
        if (response.IsSuccessStatusCode)
        {
            var favoris = await response.Content.ReadFromJsonAsync<List<Favoris>>();
            if (favoris != null)
            {
                // Récupérer les filmId à partir de la liste des favoris
                var filmIds = favoris.Select(f => f.FilmId).ToList();
                return filmIds;
            }
            else
            {
                // Si la liste des favoris est null, retourner une liste vide
                return new List<int>();
            }
        }
        else
        {
            // Gérer l'erreur si nécessaire
            return new List<int>();
        }
    }

    //Pour avoir les film 
    public async Task<List<Film>> GetFavoriteMoviesByIds(List<int> filmIds)
    {
        var films = new List<Film>();

        foreach (var filmId in filmIds)
        {
            var response = await _httpClient.GetAsync($"api/Film/searchID?id={filmId}");
            if (response.IsSuccessStatusCode)
            {
                var film = await response.Content.ReadFromJsonAsync<Film>();
                if (film != null)
                {
                    films.Add(film);
                }
            }
        }

        return films;
    }

    public async Task<Models.Favoris?> AddFavoris(int userId, Film film)
    {
        try
        {
            if (film == null)
            {
                Console.Error.WriteLine("Erreur : le film est null");
                return null;
            }

            // Utiliser la méthode AddFilm pour ajouter le film dans la base si nécessaire
            var addedFilm = await _movieService.AddFilm(film);

            if (addedFilm == null)
            {
                Console.Error.WriteLine("Erreur : le film n'a pas pu être ajouté à la base.");
                return null;
            }

            // Préparer les données pour l'appel à l'API de favoris
            var filmId = addedFilm.Id;

            // Appeler l'API pour ajouter le film aux favoris
           var response = await _httpClient.PostAsJsonAsync<Favoris>($"api/Favoris/Add?userId={userId}&filmId={filmId}", null);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.Error.WriteLine($"Erreur HTTP {response.StatusCode}: {errorContent}");
                return null;
            }

            // Désérialiser la réponse pour obtenir le favori ajouté
            var addedFavoris = await response.Content.ReadFromJsonAsync<Favoris>();

            if (addedFavoris == null)
            {
                Console.Error.WriteLine("Erreur : les favoris n'ont pas pu être ajoutés.");
                return null;
            }

            Console.Error.WriteLine($"Film {addedFilm.Title} ajouté aux favoris de l'utilisateur {userId}");
            return addedFavoris;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Erreur lors de l'ajout des favoris : {ex.Message}");
            Console.Error.WriteLine($"Stack trace : {ex.StackTrace}");
            return null;
        }
    }



}





