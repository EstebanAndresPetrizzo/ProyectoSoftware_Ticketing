using TicketingAPI.Models;

namespace TicketingAPI.Repositories
{
    public interface IReservationRepository
    {
        Task AddReservationAsync(Reservation reservation);

        Task<bool> AnyActiveReservationAsync(int seatId, int eventId);
    }
}
