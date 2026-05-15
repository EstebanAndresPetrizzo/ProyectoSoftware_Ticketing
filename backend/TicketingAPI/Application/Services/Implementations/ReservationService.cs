using Microsoft.EntityFrameworkCore;
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

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var isTaken = await _unitOfWork.Reservations.AnyActiveReservationAsync(seat.Id, request.EventId);
                if (isTaken)
                {
                    throw new InvalidOperationException("El asiento ya tiene una reserva activa o está comprado.");
                }

                var reservation = new Reservation
                {
                    SeatId = seat.Id,
                    EventId = request.EventId,
                    UserId = request.UserId,
                    Status = "Pending",
                    ReservedAt = DateTime.UtcNow,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(5)
                };
                await _unitOfWork.Reservations.AddReservationAsync(reservation);

                var evt = await _unitOfWork.Events.GetEventByIdAsync(request.EventId);
                var sectorName = evt?.Venue?.Sectors.FirstOrDefault(s => s.Id == request.SectorId)?.Name ?? request.SectorId.ToString();
                var eventName = evt?.Name ?? request.EventId.ToString();

                var auditLog = new AuditLog
                {
                    UserId = request.UserId,
                    Action = "CreateReservation",
                    EntityType = "Seat",
                    EntityId = seat.Id.ToString(),
                    CreatedAt = DateTime.UtcNow,
                    Details = $"Reserva realizada para la butaca {seat.SeatNumber} en el sector '{sectorName}' para el evento '{eventName}'."
                };
                await _unitOfWork.AuditLogs.AddAuditLogAsync(auditLog);

                await _unitOfWork.CompleteAsync();
                await _unitOfWork.CommitTransactionAsync();

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
            catch (DbUpdateConcurrencyException)
            {
                await _unitOfWork.RollbackTransactionAsync();

                var conflictAuditLog = new AuditLog
                {
                    UserId = request.UserId,
                    Action = "ReservationConflict",
                    EntityType = "Seat",
                    EntityId = seat.Id.ToString(),
                    CreatedAt = DateTime.UtcNow,
                    Details = $"Intento de reserva fallido por concurrencia para la butaca {seat.SeatNumber} en el evento {request.EventId}."
                };
                await _unitOfWork.AuditLogs.AddAuditLogAsync(conflictAuditLog);
                await _unitOfWork.CompleteAsync();

                throw new InvalidOperationException("Asiento ya no disponible. Otro usuario lo reservó primero.");
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task CancelReservationAsync(CreateReservationRequestDto request, CancellationToken cancellationToken = default)
        {
            var seat = await _unitOfWork.Seats.GetSeatByIdAsync(request.SeatId);
            if (seat == null)
            {
                throw new ArgumentException("El asiento especificado no existe.");
            }

            if (seat.SectorId != request.SectorId)
            {
                throw new ArgumentException("El asiento no pertenece al sector especificado.");
            }

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var reservation = await _unitOfWork.Reservations.GetPendingReservationForUserAsync(
                    request.SeatId, request.EventId, request.UserId, cancellationToken);

                if (reservation == null)
                {
                    throw new InvalidOperationException("No tienes una reserva activa para esta butaca.");
                }

                reservation.Status = "Cancelled";
                // seat.Status ya no se modifica a nivel global

                var evt = await _unitOfWork.Events.GetEventByIdAsync(request.EventId);
                var sectorName = evt?.Venue?.Sectors.FirstOrDefault(s => s.Id == request.SectorId)?.Name ?? request.SectorId.ToString();
                var eventName = evt?.Name ?? request.EventId.ToString();

                var auditLog = new AuditLog
                {
                    UserId = request.UserId,
                    Action = "CancelReservation",
                    EntityType = "Reservation",
                    EntityId = reservation.Id.ToString(),
                    CreatedAt = DateTime.UtcNow,
                    Details = $"Reserva cancelada manualmente para la butaca {seat.SeatNumber} en el sector '{sectorName}' para el evento '{eventName}'."
                };
                await _unitOfWork.AuditLogs.AddAuditLogAsync(auditLog);

                await _unitOfWork.CompleteAsync();
                await _unitOfWork.CommitTransactionAsync();
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }
    }
}