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
                Venue = e.Venue?.Name ?? "Desconocido",
                Date = e.EventDate
            });
        }

        public async Task<EventResponseDto?> GetEventByIdAsync(int id)
        {
            var e = await _unitOfWork.Events.GetEventByIdAsync(id);
            if (e == null) return null;

            return new EventResponseDto
            {
                Id = e.Id,
                Name = e.Name,
                Venue = e.Venue?.Name ?? "Desconocido",
                Date = e.EventDate,
                Sectors = e.Venue?.Sectors.Select(s => new ProyectoSoftware_Ticketing.DTOs.Sector.SectorDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    Price = s.Price,
                    Rows = s.Rows,
                    Cols = s.Cols,
                    Position = s.Position
                }).ToList() ?? new()
            };
        }
    }
}
