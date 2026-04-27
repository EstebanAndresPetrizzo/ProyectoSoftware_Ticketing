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
            // 1. Traemos el evento con TODA su jerarquía desde el Repositorio
            var evt = await _unitOfWork.Events.GetEventWithSeatMapAsync(eventId);
            if (evt == null) throw new ArgumentException("Evento no encontrado.");

            var now = DateTime.UtcNow;

            // 2. Mapeamos manualmente cada propiedad del DTO
            return new EventSeatMapDto
            {
                EventId = evt.Id,
                EventName = evt.Name,
                VenueName = evt.Venue?.Name ?? "Estadio Desconocido",
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
                        // Aquí calculamos el estado dinámico basado en las reservas
                        Status = DetermineRealStatus(seat, eventId, now)
                    }).ToList()
                }).ToList()
            };
        }

        private SeatStatusDto DetermineRealStatus(TicketingAPI.Models.Seat seat, int eventId, DateTime now)
            {
                // Buscamos si hay alguna reserva activa para este evento
                var activeReservation = seat.Reservations
                    .FirstOrDefault(r => r.EventId == eventId && r.Status != "Expired");

                if (activeReservation == null) return SeatStatusDto.Available;

                if (activeReservation.Status == "Paid") return SeatStatusDto.Sold;

                if (activeReservation.Status == "Pending")
                {
                    // Si es Pending pero ya pasaron los 5 minutos... ¡está disponible!
                    return activeReservation.ExpiresAt > now 
                        ? SeatStatusDto.Reserved 
                        : SeatStatusDto.Available;
                }

                return SeatStatusDto.Available;
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
