using NomDuFront.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace NomDuFront.Services
{
    public enum SearchType
    {
        ById,
        ByTitle
    }

    public class MovieService
    {
        private readonly HttpClient _httpClient;

        public MovieService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<Film>?> SearchID(string Id)
        {
            try 
            {
                // Pour la recherche par ID, on s'attend à recevoir un seul film
                var film = await _httpClient.GetFromJsonAsync<Film>($"api/Omdb/IdSearch?imdbID={Id}");

                if (film == null)
                { 
                    Console.Error.WriteLine("Erreur : la réponse du serveur est vide.");
                    return null;
                }

                // On retourne une liste contenant ce seul film
                return new List<Film> { film };
            }
            catch (Exception ex) 
            {
                Console.Error.WriteLine($"Exception lors de la connexion : {ex.Message}");
                return null;
            }
        }

        public async Task<List<Film>?> SearchByTitle(string title)
        {
            try 
            {
                var films = await _httpClient.GetFromJsonAsync<List<Film>>($"api/Omdb/Search?titre={Uri.EscapeDataString(title)}");

                if (films == null)
                {
                    Console.Error.WriteLine("Erreur : la réponse du serveur est vide.");
                    return null;
                }

                return films;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Exception lors de la connexion : {ex.Message}");
                return null;
            }
        }

        
        public async Task<List<Film>?> Search(string searchTerm, SearchType searchType)
        {
            return searchType switch
            {
                SearchType.ById => await SearchID(searchTerm),
                SearchType.ByTitle => await SearchByTitle(searchTerm),
                _ => throw new ArgumentException("Type de recherche non valide", nameof(searchType))
            };
        }
    
        public async Task<Film?> AddFilm(Film film)
    {
        try
        {
            if (film == null)
            {
                Console.Error.WriteLine("Erreur : le film est null");
                return null;
            }

            // Log avant l'envoi
            Console.Error.WriteLine($"Tentative d'ajout du film : {film.Title} (ImdbID: {film.Id})");

            // Envoyer la requête POST
            var response = await _httpClient.PostAsJsonAsync("api/Film/Add movies", film);

            // Vérifier si la requête a réussi
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.Error.WriteLine($"Erreur HTTP {response.StatusCode}: {errorContent}");
                return null;
            }

            // Désérialiser la réponse
            var addedFilm = await response.Content.ReadFromJsonAsync<Film>();

            if (addedFilm == null)
            {
                Console.Error.WriteLine("Erreur : le film retourné est null");
                return null;
            }

            Console.Error.WriteLine($"Film ajouté avec succès : {addedFilm.Title}");
            return addedFilm;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Exception lors de l'ajout : {ex.Message}");
            Console.Error.WriteLine($"Stack trace : {ex.StackTrace}");
            return null;
        }
    }
        public async Task<List<Film>> GetAllFilms()
        {
            try
            {
                var films = await _httpClient.GetFromJsonAsync<List<Film>>("api/Film/Get All Films");
                return films ?? new List<Film>();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Erreur lors de la récupération des films : {ex.Message}");
                return new List<Film>();
            }
        }

    }
}