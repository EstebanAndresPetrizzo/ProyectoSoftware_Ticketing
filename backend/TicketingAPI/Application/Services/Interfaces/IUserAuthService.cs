using ProyectoSoftware_Ticketing.DTOs.Auth;

namespace TicketingAPI.Application.Services.Interfaces;

public interface IUserAuthService
{
    Task<GoogleSignInResponseDto> SignInWithGoogleAsync(string idToken, CancellationToken cancellationToken = default);
}
