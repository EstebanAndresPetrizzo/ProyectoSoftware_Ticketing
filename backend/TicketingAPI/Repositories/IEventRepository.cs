using TicketingAPI.Models;

namespace TicketingAPI.Repositories
{
    public interface IEventRepository
    {
        Task<IEnumerable<Event>> GetAllEventsAsync();
        Task<Event?> GetEventByIdAsync(int id);
        Task<Event?> GetEventWithSeatMapAsync(int eventId);
    }
}
