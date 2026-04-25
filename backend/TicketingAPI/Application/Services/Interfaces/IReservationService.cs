using ProyectoSoftware_Ticketing.DTOs.Reservation;

namespace TicketingAPI.Application.Services.Interfaces
{
    public interface IReservationService
    {
        Task<ReservationResponseDto> CreateReservationAsync(CreateReservationRequestDto request);
    }
}
