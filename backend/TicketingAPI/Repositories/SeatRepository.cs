using Microsoft.EntityFrameworkCore;
using TicketingAPI.Data;
using TicketingAPI.Models;

namespace TicketingAPI.Repositories
{
    /// <summary>
    /// Repositorio encargado de gestionar el estado de las butacas (Seats).
    /// Fundamental para la visualización del mapa interactivo y la lógica de reserva de lugares.
    /// </summary>
    public class SeatRepository : ISeatRepository
    {
        private readonly AppDbContext _context;

        public SeatRepository(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Obtiene la lista completa de butacas asociadas a un evento en particular, 
        /// incluyendo los datos del sector para poder renderizar el mapa de asientos.
        /// </summary>
        /// <param name="eventId">ID del evento.</param>
        /// <returns>Colección de butacas (Seats).</returns>
        public async Task<IEnumerable<Seat>> GetSeatsByEventIdAsync(int eventId)
        {
            var eventEntity = await _context.Events.FindAsync(eventId);
            if (eventEntity == null) return new List<Seat>();

            return await _context.Seats
                .Include(s => s.Sector)
                .Where(s => s.Sector.VenueId == eventEntity.VenueId)
                .ToListAsync();
        }

        /// <summary>
        /// Busca una butaca específica por su ID.
        /// Útil para validar disponibilidad antes de crear una reserva.
        /// </summary>
        /// <param name="id">ID de la butaca.</param>
        /// <returns>La butaca si existe, null de lo contrario.</returns>
        public async Task<Seat?> GetSeatByIdAsync(int id)
        {
            return await _context.Seats.FindAsync(id);
        }

        /// <summary>
        /// Actualiza el estado de una butaca en el contexto (ej. de Disponible a Reservado).
        /// No guarda los cambios en la base de datos hasta que el UnitOfWork haga CompleteAsync.
        /// </summary>
        /// <param name="seat">La entidad butaca modificada.</param>
        public Task UpdateSeatAsync(Seat seat)
        {
            _context.Seats.Update(seat);
            return Task.CompletedTask;
        }
    }
}
