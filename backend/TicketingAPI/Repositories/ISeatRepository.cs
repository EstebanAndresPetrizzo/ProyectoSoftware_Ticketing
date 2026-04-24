using TicketingAPI.Models;

namespace TicketingAPI.Repositories
{
    public interface ISeatRepository
    {
        Task<IEnumerable<Seat>> GetSeatsByEventIdAsync(int eventId);
        Task<Seat?> GetSeatByIdAsync(int id);
        Task UpdateSeatAsync(Seat seat);
    }
}
