using ProyectoSoftware_Ticketing.DTOs.Event;
using TicketingAPI.Repositories;

namespace TicketingAPI.Application.Services.Implementations
{
    public class EventService : Interfaces.IEventService
    {
        private readonly IUnitOfWork _unitOfWork;

        public EventService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<EventSummaryDto>> GetAllEventsAsync()
        {
            var events = await _unitOfWork.Events.GetAllEventsAsync();
            return events.Select(e => new EventSummaryDto
            {
                Id = e.Id,
                Name = e.Name,
                Venue = e.Venue,
                Date = e.EventDate 
            });
        }
    }
}
