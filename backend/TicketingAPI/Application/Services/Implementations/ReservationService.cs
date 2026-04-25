using ProyectoSoftware_Ticketing.DTOs.Reservation;
using TicketingAPI.Models;
using TicketingAPI.Repositories;

namespace TicketingAPI.Application.Services.Implementations
{
    public class ReservationService : Interfaces.IReservationService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ReservationService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ReservationResponseDto> CreateReservationAsync(CreateReservationRequestDto request)
        {
            var seat = await _unitOfWork.Seats.GetSeatByIdAsync(request.SeatId);
            if (seat == null)
            {
                throw new ArgumentException("El asiento especificado no existe.");
            }

            // Validación de seguridad cruzada
            if (seat.SectorId != request.SectorId)
            {
                throw new ArgumentException("El asiento no pertenece al sector especificado.");
            }

            if (seat.Status != "Available")
            {
                throw new InvalidOperationException("El asiento ya no está disponible.");
            }

            // 1. Cambiar estado de la butaca
            seat.Status = "Reserved";
            await _unitOfWork.Seats.UpdateSeatAsync(seat);

            // 2. Crear modelo Reservation
            var reservation = new Reservation
            {
                SeatId = seat.Id,
                UserId = request.UserId, // Tomamos el Guid real que viene del frontend
                Status = "Pending",
                ReservedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddMinutes(5) // Expiración en 5 minutos para el Background Job
            };
            await _unitOfWork.Reservations.AddReservationAsync(reservation);

            // 3. Crear log de auditoría
            var auditLog = new AuditLog
            {
                UserId = request.UserId,
                Action = "CreateReservation",
                EntityType = "Seat",
                EntityId = seat.Id.ToString(),
                CreatedAt = DateTime.UtcNow,
                Details = $"El usuario con ID '{request.UserId}' reservó la butaca {seat.Id} del sector {request.SectorId} para el evento {request.EventId}"
            };
            await _unitOfWork.AuditLogs.AddAuditLogAsync(auditLog);

            // 4. Persistir todo en la BD
            await _unitOfWork.CompleteAsync();

            return new ReservationResponseDto
            {
                Id = reservation.Id,
                EventId = request.EventId,
                SectorId = request.SectorId,
                SeatId = seat.Id,
                UserId = request.UserId,
                Status = reservation.Status,
                CreatedAt = reservation.ReservedAt
            };
        }
    }
}
