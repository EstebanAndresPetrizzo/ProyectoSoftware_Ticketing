namespace ProyectoSoftware_Ticketing.DTOs.Auth;

public class GoogleSignInRequestDto
{
    public string IdToken { get; set; } = string.Empty;
}

public class GoogleSignInResponseDto
{
    public Guid UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}

/// <summary>Datos públicos para el cliente (el Client ID de OAuth web no es secreto).</summary>
public class GooglePublicConfigDto
{
    public string ClientId { get; set; } = string.Empty;

    /// <summary>True si el servidor permite login de prueba sin Google (solo Development + opción explícita).</summary>
    public bool DevLoginAvailable { get; set; }
}
