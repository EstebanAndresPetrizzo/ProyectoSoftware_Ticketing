using TicketingAPI.Data;
using TicketingAPI.Models;

namespace TicketingAPI.Repositories
{
    /// <summary>
    /// Repositorio responsable de interactuar con la tabla AuditLog.
    /// Garantiza la persistencia inmutable de todas las acciones del usuario,
    /// cumpliendo con los requerimientos de auditoría estricta del sistema.
    /// </summary>
    public class AuditLogRepository : IAuditLogRepository
    {
        private readonly AppDbContext _context;

        public AuditLogRepository(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Agrega un nuevo registro de auditoría.
        /// Fundamental para mantener trazabilidad y seguridad sobre las transacciones de reserva.
        /// </summary>
        /// <param name="log">Entidad que describe la acción, el autor y la fecha.</param>
        public async Task AddAuditLogAsync(AuditLog log)
        {
            await _context.AuditLogs.AddAsync(log);
        }
    }
}
