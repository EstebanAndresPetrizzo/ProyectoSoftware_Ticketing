using TicketingAPI.Data;
using TicketingAPI.Models;
using Microsoft.EntityFrameworkCore;

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
        /// <summary>
        /// Verifica si existe alguna reserva activa (pagada o pendiente no expirada) para una butaca y evento específicos.
        /// Esto se utiliza para validar la disponibilidad real de una butaca antes de crear una nueva reserva.
        /// </summary> 

        public async Task<bool> AnyActiveReservationAsync(int seatId, int eventId)
        {
            var now = DateTime.UtcNow;

            return await _context.Reservations
                .AnyAsync(r => r.SeatId == seatId 
                            && r.EventId == eventId 
                            && (r.Status == "Paid" || (r.Status == "Pending" && r.ExpiresAt > now)));
        }
    }
}
