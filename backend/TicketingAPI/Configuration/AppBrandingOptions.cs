namespace TicketingAPI.Configuration;

public class AppBrandingOptions
{
    public const string SectionName = "App";

    public string Name { get; set; } = "TicketingApp";

    /// <summary>Correo de contacto o responsable (p. ej. pantalla de consentimiento OAuth).</summary>
    public string ContactEmail { get; set; } = string.Empty;
}
