using NomDuFront.Components;
using NomDuFront.Services;
using Microsoft.AspNetCore.Components;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;

namespace NomDuFront;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        
        builder.Services.AddBlazoredLocalStorage(); 



        // Add services to the container.
        builder.Services.AddRazorComponents()
            .AddInteractiveServerComponents();

        // Enregistrer HttpClient pour une API backend (remplacer l'URL par celle de ton API)
        builder.Services.AddScoped<HttpClient>(sp => new HttpClient { BaseAddress = new Uri("http://localhost:5280/") });

        // Enregistrer le service AuthService
        builder.Services.AddScoped<AuthService>();
        // Configuration cruciale :
        builder.Services.AddScoped<AuthenticationStateProvider>(sp => 
            sp.GetRequiredService<AuthService>());
        builder.Services.AddScoped<MovieService>();
        builder.Services.AddScoped<UserService>();
        builder.Services.AddScoped<FavoriteService>();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
        }

        app.UseStaticFiles();
        app.UseAntiforgery();

        app.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode();

        app.Run();
    }
}