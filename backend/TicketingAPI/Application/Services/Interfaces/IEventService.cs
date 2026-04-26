using ProyectoSoftware_Ticketing.DTOs.Event;

namespace TicketingAPI.Application.Services.Interfaces
{
    public interface IEventService
    {
        Task<(IEnumerable<EventSummaryDto> Items, int TotalItems)> GetPagedEventsAsync(int page, int pageSize);
        Task<EventResponseDto?> GetEventByIdAsync(int id);
    }
}
