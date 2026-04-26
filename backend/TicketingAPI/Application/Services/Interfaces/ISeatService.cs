using ProyectoSoftware_Ticketing.DTOs.Seat;

namespace TicketingAPI.Application.Services.Interfaces
{
    public interface ISeatService
    {
        Task<IEnumerable<SeatDto>> GetSeatsByEventIdAsync(int eventId);

        Task<EventSeatMapDto> GetSeatMapByEventIdAsync(int eventId);
    }
}
