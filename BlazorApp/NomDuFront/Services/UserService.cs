namespace NomDuFront.Services ; 
using NomDuFront.Models ; 
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using System.Text.Json;
using System.Net.Http.Headers;



public class UserService
{
    private readonly HttpClient _httpClient;
    private readonly IJSRuntime _jsRuntime;

    public UserService(HttpClient httpClient,IJSRuntime jsRuntime)
    {
        _httpClient = httpClient;
         _jsRuntime = jsRuntime;
    }
    


    // Méthode pour récupérer tous les utilisateurs
    public async Task<List<User>> GetAllUsers()
    {
        try
        {
            // Récupérer le token stocké dans localStorage
            var token = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "jwtToken");

            // Vérifier si le token existe
            if (string.IsNullOrEmpty(token))
            {
                throw new Exception("Token manquant, veuillez vous connecter à nouveau.");
            }

            // Ajouter le token dans les en-têtes Authorization
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.GetAsync("User/Get All Users");

            if (response.IsSuccessStatusCode)
            {
                var users = await response.Content.ReadFromJsonAsync<List<User>>();
                return users ?? new List<User>(); // Retourner la liste des utilisateurs ou une liste vide si aucun utilisateur trouvé
            }
            else
            {
                Console.Error.WriteLine($"Erreur lors de la récupération des utilisateurs : {response.StatusCode}");
                return new List<User>(); // Retourner une liste vide en cas d'erreur
            }
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Exception lors de la récupération des utilisateurs : {ex.Message}");
            return new List<User>(); // Retourner une liste vide en cas d'exception
        }
    }

    public async Task<List<User>> GetAllPseudoUsers()
    {
        try
        {
            // Récupérer le token stocké dans localStorage
            var token = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "jwtToken");

            // Vérifier si le token existe
            if (string.IsNullOrEmpty(token))
            {
                throw new Exception("Token manquant, veuillez vous connecter à nouveau.");
            }

            // Ajouter le token dans les en-têtes Authorization
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.GetAsync("User/Get All Users Pseudos");

            if (response.IsSuccessStatusCode)
            {
                var users = await response.Content.ReadFromJsonAsync<List<User>>();
                return users ?? new List<User>(); // Retourner la liste des utilisateurs ou une liste vide si aucun utilisateur trouvé
            }
            else
            {
                Console.Error.WriteLine($"Erreur lors de la récupération des utilisateurs : {response.StatusCode}");
                return new List<User>(); // Retourner une liste vide en cas d'erreur
            }
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Exception lors de la récupération des utilisateurs : {ex.Message}");
            return new List<User>(); // Retourner une liste vide en cas d'exception
        }
    }


    public async Task<bool> DeleteUser(int userId)
    {
        try
        {
            // Récupérer le token pour l'authentification
            var token = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "jwtToken");
            
            // Configurer le header d'autorisation
            var requestMessage = new HttpRequestMessage(HttpMethod.Delete, $"User/Delete User?id={userId}")
            {
                Headers = {
                    Authorization = new AuthenticationHeaderValue("Bearer", token)
                }
            };

            // Envoyer la requête DELETE
            var response = await _httpClient.SendAsync(requestMessage);

            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Erreur lors de la suppression de l'utilisateur : {ex.Message}");
            return false;
        }
    }
    public async Task<bool> UpdateUserAdmin(User user)
{
    try
    {
        // S'assurer que nous n'envoyons que les champs nécessaires
        var updateData = new
        {
            Pseudo = user.Pseudo,
            Role = user.Role
        };

        var response = await _httpClient.PutAsJsonAsync("User/Update User as Admin", updateData);
        var content = await response.Content.ReadAsStringAsync();
        
        if (!response.IsSuccessStatusCode)
        {
            Console.Error.WriteLine($"Erreur du serveur: {content}");
            return false;
        }

        return true;
    }
    catch (Exception ex)
    {
        Console.Error.WriteLine($"Erreur lors de la mise à jour de l'utilisateur : {ex.Message}");
        return false;
    }
}
    public async Task<bool> UpdateUser(User user, string? newPassword = null)
{
    try
    {
        var token = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "jwtToken");
        
        if (string.IsNullOrEmpty(token))
        {
            throw new Exception("Token manquant, veuillez vous connecter à nouveau.");
        }

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Créer l'objet de mise à jour avec les champs nécessaires
        var updateData = new
        {
            Id = user.Id,
            Pseudo = user.Pseudo,
            Role = user.Role,
            NewPassword = newPassword
        };

        var response = await _httpClient.PutAsJsonAsync("User/Update User as user", updateData);
        
        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            Console.Error.WriteLine($"Erreur du serveur: {content}");
            return false;
        }

        return true;
    }
    catch (Exception ex)
    {
        Console.Error.WriteLine($"Erreur lors de la mise à jour de l'utilisateur : {ex.Message}");
        return false;
    }
}
  // ajout des liste des admin et des users by bpseudo 
    public async Task<List<User>> GetAdminUsers()
{
    try
    {
        var users = await _httpClient.GetFromJsonAsync<List<User>>("User/Get Admins");
        return users ?? new List<User>();
    }
    catch (Exception ex)
    {
        Console.Error.WriteLine($"Erreur lors de la récupération des admins : {ex.Message}");
        return new List<User>();
    }
}




public async Task<User?> GetUserById(int userId)
{
    try
    {
        var token = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "jwtToken");
        
        if (string.IsNullOrEmpty(token))
        {
            throw new Exception("Token manquant, veuillez vous connecter à nouveau.");
        }

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _httpClient.GetAsync($"User/Get User?id={userId}");

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<User>();
        }
        else
        {
            Console.Error.WriteLine($"Erreur lors de la récupération de l'utilisateur : {response.StatusCode}");
            return null;
        }
    }
    catch (Exception ex)
    {
        Console.Error.WriteLine($"Exception lors de la récupération de l'utilisateur : {ex.Message}");
        return null;
    }
}

}