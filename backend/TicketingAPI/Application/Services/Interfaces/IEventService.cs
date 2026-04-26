using ProyectoSoftware_Ticketing.DTOs.Event;

namespace TicketingAPI.Application.Services.Interfaces
{
    public interface IEventService
    {
        Task<IEnumerable<EventSummaryDto>> GetAllEventsAsync();
        Task<EventResponseDto?> GetEventByIdAsync(int id);
    }
}
