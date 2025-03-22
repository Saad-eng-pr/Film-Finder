using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

public class JwtService
{
    private readonly string _secretKey;
    private readonly string _issuer;
    private readonly string _audience;
    private readonly SymmetricSecurityKey _securityKey;
    private readonly ILogger<JwtService> _logger;

    public JwtService(IConfiguration configuration, ILogger<JwtService> logger)
    {
        _secretKey = configuration["Jwt:SecretKey"] ?? throw new ArgumentNullException(nameof(configuration), "SecretKey is missing");
        _issuer = configuration["Jwt:Issuer"] ?? throw new ArgumentNullException(nameof(configuration), "Issuer is missing");
        _audience = configuration["Jwt:Audience"] ?? throw new ArgumentNullException(nameof(configuration), "Audience is missing");
        _securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
        _logger = logger;

        _logger.LogInformation("JWT Service initialized successfully.");
    }

    public string GenerateToken(string userId, string username, string role)
    {
        try
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId),
                new Claim(JwtRegisteredClaimNames.UniqueName, username),
                new Claim(ClaimTypes.Role, role),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) // Pour éviter la réutilisation du token
            };

            var credentials = new SigningCredentials(_securityKey, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: _issuer,
                audience: _audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(25), // Expiration dans 25 minutes
                signingCredentials: credentials
            );


            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Erreur lors de la génération du token: {ex.Message}");
            throw;
        }
    }

    public ClaimsPrincipal? ValidateToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = _issuer,
            ValidAudience = _audience,
            IssuerSigningKey = _securityKey,
            RoleClaimType = ClaimTypes.Role
        };

        try
        {
            var principal = tokenHandler.ValidateToken(token, validationParameters, out _);
            return principal;
        }
        catch (SecurityTokenExpiredException)
        {
            _logger.LogWarning("Le token a expiré.");
            return null;
        }
        catch (SecurityTokenInvalidSignatureException)
        {
            _logger.LogWarning("La signature du token est invalide.");
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Erreur lors de la validation du token: {ex.Message}");
            return null;
        }
    }
}
