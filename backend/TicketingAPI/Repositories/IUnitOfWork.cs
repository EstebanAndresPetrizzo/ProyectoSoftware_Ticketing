namespace TicketingAPI.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IEventRepository Events { get; }
        ISeatRepository Seats { get; }
        IReservationRepository Reservations { get; }
        IAuditLogRepository AuditLogs { get; }

        Task<int> CompleteAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}
