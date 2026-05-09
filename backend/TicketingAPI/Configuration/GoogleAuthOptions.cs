namespace TicketingAPI.Configuration;

public class GoogleAuthOptions
{
    public const string SectionName = "Authentication:Google";

    /// <summary>OAuth Client ID (mismo valor que en el frontend de Google Sign-In).</summary>
    public string ClientId { get; set; } = string.Empty;
}
