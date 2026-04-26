using TicketingAPI.Models;

namespace TicketingAPI.Repositories
{
    public interface IEventRepository
    {
        Task<IEnumerable<Event>> GetAllEventsAsync();
        Task<(IEnumerable<Event> Items, int TotalItems)> GetPagedEventsAsync(int page, int pageSize);
        Task<Event?> GetEventByIdAsync(int id);
        Task<Event?> GetEventWithSeatMapAsync(int eventId);
    }
}

