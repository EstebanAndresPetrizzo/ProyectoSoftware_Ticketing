using Microsoft.EntityFrameworkCore;
using TicketingAPI.Data;
using TicketingAPI.Models;

namespace TicketingAPI.Repositories
{
    /// <summary>
    /// Repositorio encargado de gestionar el acceso a los datos de los Eventos.
    /// Provee métodos para listar el catálogo de eventos y consultar sus detalles.
    /// </summary>
    public class EventRepository : IEventRepository
    {
        private readonly AppDbContext _context;

        public EventRepository(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Obtiene el catálogo completo de todos los eventos disponibles.
        /// </summary>
        /// <returns>Una lista de eventos.</returns>
        public async Task<IEnumerable<Event>> GetAllEventsAsync()
        {
            return await _context.Events.Include(e => e.Venue).ToListAsync();
        }

        /// <summary>
        /// Obtiene una página de eventos ordenados por fecha, junto con el total de registros.
        /// </summary>
        /// <returns>Tupla con los eventos de la página y el total de registros.</returns>
        public async Task<(IEnumerable<Event> Items, int TotalItems)> GetPagedEventsAsync(int page, int pageSize)
        {
            var query = _context.Events
                .Include(e => e.Venue)
                .OrderBy(e => e.EventDate);

            var totalItems = await query.CountAsync();

            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalItems);
        }

        /// <summary>
        /// Busca un evento específico por su ID, incluyendo todos los sectores asociados.
        /// </summary>
        /// <param name="id">El identificador único del evento.</param>
        /// <returns>El evento si existe, de lo contrario null.</returns>
        public async Task<Event?> GetEventByIdAsync(int id)
        {
            return await _context.Events
                .Include(e => e.Venue)
                .ThenInclude(v => v.Sectors)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<Event?> GetEventWithSeatMapAsync(int eventId)
        {
            return await _context.Events
                .Include(e => e.Venue)                       // Carga el lugar
                    .ThenInclude(v => v.Sectors)             // Carga los sectores del lugar
                        .ThenInclude(s => s.Seats)           // Carga los asientos de cada sector
                            .ThenInclude(st => st.Reservations) // Carga las reservas para calcular el estado
                .FirstOrDefaultAsync(e => e.Id == eventId);
        }
    }
}
