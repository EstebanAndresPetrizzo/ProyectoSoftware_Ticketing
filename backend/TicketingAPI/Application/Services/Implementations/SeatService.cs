using ProyectoSoftware_Ticketing.DTOs.Seat;
using TicketingAPI.Repositories;

namespace TicketingAPI.Application.Services.Implementations
{
    public class SeatService : Interfaces.ISeatService
    {
        private readonly IUnitOfWork _unitOfWork;

        public SeatService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<EventSeatMapDto> GetSeatMapByEventIdAsync(int eventId)
        {
            var evt = await _unitOfWork.Events.GetEventWithSeatMapAsync(eventId);

            if (evt == null)
                throw new ArgumentException("Evento no encontrado.");

            return new EventSeatMapDto
            {
                EventId = evt.Id,
                EventName = evt.Name,
                VenueName = evt.Venue.Name,
                Sectors = evt.Venue.Sectors.Select(sector => new SectorSeatMapDto
                {
                    SectorId = sector.Id,
                    Name = sector.Name,
                    Price = sector.Price,
                    Rows = sector.Rows,
                    Cols = sector.Cols,
                    Position = sector.Position,
                    Seats = sector.Seats.Select(seat => new SeatDto
                    {
                        Id = seat.Id,
                        SectorId = seat.SectorId,
                        Row = seat.RowIdentifier,
                        Number = seat.SeatNumber,
                        Status = ParseStatus(seat.Status)
                    }).ToList()
                }).ToList()
            };
        }

        public async Task<IEnumerable<SeatDto>> GetSeatsByEventIdAsync(int eventId)
        {
            var seats = await _unitOfWork.Seats.GetSeatsByEventIdAsync(eventId);
            return seats.Select(s => new SeatDto
            {
                Id = s.Id,
                SectorId = s.SectorId,
                Row = s.RowIdentifier,
                Number = s.SeatNumber,
                Status = ParseStatus(s.Status)
            });
        }

        private SeatStatusDto ParseStatus(string status)
        {
            if (Enum.TryParse<SeatStatusDto>(status, true, out var result))
            {
                return result;
            }
            return SeatStatusDto.Available; // default
        }
    }
}
