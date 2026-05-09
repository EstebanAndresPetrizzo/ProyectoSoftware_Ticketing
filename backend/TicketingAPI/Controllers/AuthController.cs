using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using ProyectoSoftware_Ticketing.DTOs.Auth;
using ProyectoSoftware_Ticketing.DTOs.Common;
using TicketingAPI.Application.Services.Interfaces;
using TicketingAPI.Configuration;
using TicketingAPI.Repositories;

namespace TicketingAPI.Controllers;

[ApiController]
[Route("api/v1/auth")]
public class AuthController : ControllerBase
{
    private const string DevGoogleSub = "dev-docker-ticketing-local";

    private readonly IUserAuthService _userAuth;
    private readonly GoogleAuthOptions _googleOptions;
    private readonly TicketingOptions _ticketingOptions;
    private readonly IWebHostEnvironment _env;
    private readonly IUserRepository _users;

    public AuthController(
        IUserAuthService userAuth,
        IOptions<GoogleAuthOptions> googleOptions,
        IOptions<TicketingOptions> ticketingOptions,
        IWebHostEnvironment env,
        IUserRepository users)
    {
        _userAuth = userAuth;
        _googleOptions = googleOptions.Value;
        _ticketingOptions = ticketingOptions.Value;
        _env = env;
        _users = users;
    }

    private bool IsDevDockerLoginEnabled =>
        _env.IsDevelopment() && _ticketingOptions.EnableDockerDevLogin;

    /// <summary>Expone el Client ID configurado en el servidor para que el login lo cargue sin duplicarlo en el front estático.</summary>
    [HttpGet("google-config")]
    public ActionResult<ApiResponse<GooglePublicConfigDto>> GetGooglePublicConfig()
    {
        return Ok(new ApiResponse<GooglePublicConfigDto>
        {
            Success = true,
            Data = new GooglePublicConfigDto
            {
                ClientId = _googleOptions.ClientId?.Trim() ?? "",
                DevLoginAvailable = IsDevDockerLoginEnabled
            }
        });
    }

    /// <summary>
    /// Login de prueba sin Google: crea o reutiliza un usuario fijo en BD. Solo con Development + Ticketing:EnableDockerDevLogin.
    /// </summary>
    [HttpPost("dev-login")]
    public async Task<ActionResult<ApiResponse<GoogleSignInResponseDto>>> DevLogin(CancellationToken cancellationToken)
    {
        if (!IsDevDockerLoginEnabled)
        {
            return NotFound(new ApiResponse<GoogleSignInResponseDto>
            {
                Success = false,
                Error = "El login de desarrollo no está habilitado en este servidor."
            });
        }

        var user = await _users.UpsertGoogleUserAsync(
            DevGoogleSub,
            "dev-docker@ticketing.local",
            "Usuario prueba (Docker)",
            cancellationToken);

        return Ok(new ApiResponse<GoogleSignInResponseDto>
        {
            Success = true,
            Data = new GoogleSignInResponseDto
            {
                UserId = user.Id,
                Email = user.Email,
                Name = user.Name
            }
        });
    }

    /// <summary>Valida el id_token de Google, crea o actualiza el usuario en BD y devuelve el UserId interno.</summary>
    [HttpPost("google")]
    public async Task<ActionResult<ApiResponse<GoogleSignInResponseDto>>> GoogleSignIn(
        [FromBody] GoogleSignInRequestDto request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(_googleOptions.ClientId))
        {
            return StatusCode(503, new ApiResponse<GoogleSignInResponseDto>
            {
                Success = false,
                Error = "El servidor no tiene configurado Authentication:Google:ClientId."
            });
        }

        try
        {
            var data = await _userAuth.SignInWithGoogleAsync(request.IdToken, cancellationToken);
            return Ok(new ApiResponse<GoogleSignInResponseDto> { Success = true, Data = data });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new ApiResponse<GoogleSignInResponseDto> { Success = false, Error = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new ApiResponse<GoogleSignInResponseDto> { Success = false, Error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new ApiResponse<GoogleSignInResponseDto> { Success = false, Error = ex.Message });
        }
    }
}
