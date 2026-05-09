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

        public async Task<EventSeatMapDto> GetSeatMapByEventIdAsync(int eventId, Guid? currentUserId = null)
        {
            // 1. Traemos el evento con TODA su jerarquía desde el Repositorio
            var evt = await _unitOfWork.Events.GetEventWithSeatMapAsync(eventId);
            if (evt == null) throw new ArgumentException("Evento no encontrado.");
            if (evt.Venue == null) throw new ArgumentException("El evento no tiene sede asignada.");

            var now = DateTime.UtcNow;

            // 2. Mapeamos manualmente cada propiedad del DTO
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
                    Seats = sector.Seats.Select(seat =>
                    {
                        var (status, isMine, myPendingExpiresAtUtc) = ResolveSeatState(seat, eventId, now, currentUserId);
                        return new SeatDto
                        {
                            Id = seat.Id,
                            SectorId = seat.SectorId,
                            Row = seat.RowIdentifier,
                            Number = seat.SeatNumber,
                            Status = status,
                            IsMine = isMine,
                            MyPendingExpiresAtUtc = myPendingExpiresAtUtc
                        };
                    }).ToList()
                }).ToList()
            };
        }

        private static (SeatStatusDto Status, bool IsMine, DateTime? MyPendingExpiresAtUtc) ResolveSeatState(
            TicketingAPI.Models.Seat seat, int eventId, DateTime now, Guid? currentUserId)
        {
            var activeReservation = seat.Reservations
                .FirstOrDefault(r => r.EventId == eventId && r.Status != "Expired");

            if (activeReservation == null)
            {
                return (SeatStatusDto.Available, false, null);
            }

            if (activeReservation.Status == "Paid")
            {
                return (SeatStatusDto.Sold, false, null);
            }

            if (activeReservation.Status == "Pending")
            {
                if (activeReservation.ExpiresAt <= now)
                {
                    return (SeatStatusDto.Available, false, null);
                }

                var mine = currentUserId.HasValue && activeReservation.UserId == currentUserId.Value;
                return (SeatStatusDto.Reserved, mine, mine ? activeReservation.ExpiresAt : null);
            }

            return (SeatStatusDto.Available, false, null);
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
