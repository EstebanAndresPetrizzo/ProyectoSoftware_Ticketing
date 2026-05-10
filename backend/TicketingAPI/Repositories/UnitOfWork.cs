using Microsoft.EntityFrameworkCore.Storage;
using TicketingAPI.Data;

namespace TicketingAPI.Repositories
{
    /// <summary>
    /// Patrón de diseño Unit of Work que agrupa los repositorios para compartir el mismo contexto de datos (AppDbContext).
    /// Permite agrupar las operaciones de negocio y coordinar su persistencia (SaveChanges)
    /// o el manejo explícito de transacciones transaccionales (ACID).
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        private IDbContextTransaction? _transaction;

        public IEventRepository Events { get; private set; }
        public ISeatRepository Seats { get; private set; }
        public IReservationRepository Reservations { get; private set; }
        public IAuditLogRepository AuditLogs { get; private set; }

        public UnitOfWork(
            AppDbContext context,
            IEventRepository eventRepository,
            ISeatRepository seatRepository,
            IReservationRepository reservationRepository,
            IAuditLogRepository auditLogRepository)
        {
            _context = context;
            Events = eventRepository;
            Seats = seatRepository;
            Reservations = reservationRepository;
            AuditLogs = auditLogRepository;
        }

        /// <summary>
        /// Aplica todos los cambios rastreados por los repositorios en la base de datos de forma atómica.
        /// Retorna el número de registros afectados.
        /// </summary>
        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Inicia una transacción explícita en la base de datos.
        /// Requerido para la fase 2 donde necesitamos transacciones ACID estrictas para reservas y pagos.
        /// </summary>
        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        /// <summary>
        /// Confirma la transacción actual y libera los recursos asociados.
        /// Debe llamarse solo después de un CompleteAsync exitoso y cobro procesado.
        /// </summary>
        public async Task CommitTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.CommitAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        /// <summary>
        /// Revierte cualquier cambio en la base de datos dentro de la transacción activa.
        /// Se utiliza si ocurre un error (ej: conflicto de concurrencia o fallo en el pago).
        /// </summary>
        public async Task RollbackTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public void Dispose()
        {
            _context.Dispose();
            _transaction?.Dispose();
        }
    }
}
