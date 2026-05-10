using Google.Apis.Auth;
using Microsoft.Extensions.Options;
using ProyectoSoftware_Ticketing.DTOs.Auth;
using TicketingAPI.Application.Services.Interfaces;
using TicketingAPI.Configuration;
using TicketingAPI.Repositories;

namespace TicketingAPI.Application.Services.Implementations;

public class UserAuthService : IUserAuthService
{
    private readonly IUserRepository _users;
    private readonly GoogleAuthOptions _options;

    public UserAuthService(IUserRepository users, IOptions<GoogleAuthOptions> options)
    {
        _users = users;
        _options = options.Value;
    }

    public async Task<GoogleSignInResponseDto> SignInWithGoogleAsync(string idToken, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(idToken))
        {
            throw new ArgumentException("El token de Google es obligatorio.");
        }

        if (string.IsNullOrWhiteSpace(_options.ClientId))
        {
            throw new InvalidOperationException(
                "Falta Authentication:Google:ClientId en la configuración (debe coincidir con el Client ID del frontend).");
        }

        GoogleJsonWebSignature.Payload payload;
        try
        {
            payload = await GoogleJsonWebSignature.ValidateAsync(idToken, new GoogleJsonWebSignature.ValidationSettings
            {
                Audience = [_options.ClientId]
            });
        }
        catch (InvalidJwtException ex)
        {
            throw new UnauthorizedAccessException("Token de Google no válido o expirado.", ex);
        }

        var email = payload.Email;
        if (string.IsNullOrWhiteSpace(email))
        {
            throw new InvalidOperationException("Google no devolvió email en el token.");
        }

        var name = string.IsNullOrWhiteSpace(payload.Name) ? email : payload.Name!;
        var user = await _users.UpsertGoogleUserAsync(payload.Subject, email, name, cancellationToken);

        return new GoogleSignInResponseDto
        {
            UserId = user.Id,
            Email = user.Email,
            Name = user.Name
        };
    }
}
