namespace TicketingAPI.Models
{
    public class User
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        /// <summary>Solo usuarios con login clásico; null si el acceso es solo con Google.</summary>
        public string? PasswordHash { get; set; }
        /// <summary>Identificador estable de Google (claim "sub"); null para usuarios creados sin OAuth.</summary>
        public string? GoogleSub { get; set; }
        public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
        public ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();
    }
}