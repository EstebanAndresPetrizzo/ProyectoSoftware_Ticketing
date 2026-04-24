using TicketingAPI.Models;

namespace TicketingAPI.Repositories
{
    public interface IAuditLogRepository
    {
        Task AddAuditLogAsync(AuditLog log);
    }
}
