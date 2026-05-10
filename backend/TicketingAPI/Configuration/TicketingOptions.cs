namespace TicketingAPI.Configuration;

public class TicketingOptions
{
    public const string SectionName = "Ticketing";

    /// <summary>
    /// Si es true y el entorno es Development, se expone POST /auth/dev-login para pruebas sin Google (solo Docker/local).
    /// </summary>
    public bool EnableDockerDevLogin { get; set; }
}
