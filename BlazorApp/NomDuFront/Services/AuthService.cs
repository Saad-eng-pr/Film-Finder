namespace NomDuFront.Services ; 
using NomDuFront.Models ; 
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using System.Text.Json;
using System.Security.Claims;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using System.IdentityModel.Tokens.Jwt;


public class AuthService : AuthenticationStateProvider
{
    private readonly HttpClient _httpClient;
    private readonly IJSRuntime _jsRuntime;
     public event Action? OnAuthStateChanged;

    public AuthService(HttpClient httpClient,IJSRuntime jsRuntime)
    {
        _httpClient = httpClient;
         _jsRuntime = jsRuntime;
    }
    


    public async Task<(string? Token, string? Role)> Login(User u)
{
    try 
    {
        var res = await _httpClient.PostAsJsonAsync("User/login", u);

        if (res.IsSuccessStatusCode)
        {
            var response = await res.Content.ReadFromJsonAsync<JsonElement>();
            
            // Extraction du token et du rôle
            string? token = response.GetProperty("token").GetString();
            int? role = response.GetProperty("role").GetInt32();

            if (!string.IsNullOrEmpty(token) )
            {
                // Stockage des deux valeurs dans le localStorage
                await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "jwtToken", token);
                await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "userRole", role.ToString());
                
                // Notification du changement d'état
                NotifyUserLoggedIn(role.ToString());
                return (token, role.ToString());
            }
        }
        else
        {
            string errorMessage = await res.Content.ReadAsStringAsync();
            Console.Error.WriteLine($"Échec de la connexion : {res.StatusCode} - {errorMessage}");
        }
    }
    catch (Exception ex) 
    {
        Console.Error.WriteLine($"Exception lors de la connexion : {ex.Message}");
    }
    
    return (null, null);
}



    public async Task<User?> Register(User u)
{
    try 
    {
        var res = await _httpClient.PostAsJsonAsync("User/register", u);

        if (res.IsSuccessStatusCode)
        {
            var user = await res.Content.ReadFromJsonAsync<User>();
            if (user == null)
            {
                Console.Error.WriteLine("Erreur : la réponse du serveur est vide.");
            }
            return user;
        }
        else
        {
            string errorMessage = await res.Content.ReadAsStringAsync();
            Console.Error.WriteLine($"Échec de la connexion : {res.StatusCode} - {errorMessage}");
        }
    }
    catch (Exception ex) 
    {
        Console.Error.WriteLine($"Exception lors de la connexion : {ex.Message}");
        
    }
    
    return null; // Toujours retourner null en cas d'erreur
}

   // Méthode requise pour AuthenticationStateProvider
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "jwtToken");
        var role = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "userRole");

        var identity = new ClaimsIdentity();
        if (!string.IsNullOrEmpty(token))
        {
            identity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Role, role),
                new Claim("Token", token)
            }, "jwt_auth");
        }

        return new AuthenticationState(new ClaimsPrincipal(identity));
    }

    public void NotifyAuthStateChanged()
    {
        OnAuthStateChanged?.Invoke();
    }

    public async Task<string?> GetUserIdAsync()
{
    // Récupérer le token JWT depuis le localStorage
    var token = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "jwtToken");

    if (string.IsNullOrEmpty(token))
        return null; // Aucun token trouvé

    try
    {
        var handler = new JwtSecurityTokenHandler();

        if (!handler.CanReadToken(token)) 
            return null; // Format du token invalide

        var jwtToken = handler.ReadJwtToken(token);

        // Extraire l'ID utilisateur (souvent stocké sous "sub" ou "id")
        var userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier || c.Type == "sub" || c.Type == "id");

        return userIdClaim?.Value; // Retourne l'ID ou null si absent
    }
    catch
    {
        return null; // Erreur lors du parsing => token invalide
    }
}
     // Méthodes de notification
    private void NotifyUserLoggedIn(string role)
    {
        var identity = new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.Role, role),
            new Claim(ClaimTypes.Authentication, "true")
        }, "jwt_auth");

        NotifyAuthenticationStateChanged(
            Task.FromResult(new AuthenticationState(new ClaimsPrincipal(identity))));
    }

    private void NotifyUserLoggedOut()
    {
        var identity = new ClaimsIdentity();
        NotifyAuthenticationStateChanged(
            Task.FromResult(new AuthenticationState(new ClaimsPrincipal(identity))));
    }
    
    public async Task<bool> IsAuthenticated()
{
    // Vérifie si un token JWT est stocké dans le localStorage
    var token = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "jwtToken");
    return !string.IsNullOrEmpty(token);
}

    public async Task Logout()
{
    // Supprimer les éléments du localStorage
    await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "jwtToken");
    await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "userRole");
    
}

}


