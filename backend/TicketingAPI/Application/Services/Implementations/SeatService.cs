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
