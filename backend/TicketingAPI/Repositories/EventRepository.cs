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
    }
}
