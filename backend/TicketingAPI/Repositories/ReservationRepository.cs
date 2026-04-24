using TicketingAPI.Data;
using TicketingAPI.Models;

namespace TicketingAPI.Repositories
{
    /// <summary>
    /// Repositorio destinado al manejo de reservas.
    /// Permite crear nuevos intentos de reserva bloqueando las butacas por el tiempo estipulado.
    /// </summary>
    public class ReservationRepository : IReservationRepository
    {
        private readonly AppDbContext _context;

        public ReservationRepository(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Agrega un nuevo intento de reserva al contexto de datos.
        /// Esta entidad pasará a estar rastreada y será persistida cuando se llame a SaveChanges/CompleteAsync.
        /// </summary>
        /// <param name="reservation">La nueva reserva a registrar.</param>
        public async Task AddReservationAsync(Reservation reservation)
        {
            await _context.Reservations.AddAsync(reservation);
        }
    }
}
