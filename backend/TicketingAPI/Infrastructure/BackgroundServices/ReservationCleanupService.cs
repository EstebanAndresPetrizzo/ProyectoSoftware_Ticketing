using TicketingAPI.Models;
using TicketingAPI.Repositories;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using System.Threading;

namespace TicketingAPI.Infrastructure.BackgroundServices
{
    public class ReservationCleanupService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<ReservationCleanupService> _logger;

        public ReservationCleanupService(IServiceProvider serviceProvider, ILogger<ReservationCleanupService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Reservation Cleanup Service is starting.");

            using PeriodicTimer timer = new PeriodicTimer(TimeSpan.FromMinutes(1));

            while (await timer.WaitForNextTickAsync(stoppingToken) && !stoppingToken.IsCancellationRequested)
            {
                await CleanupExpiredReservationsAsync();
            }

            _logger.LogInformation("Reservation Cleanup Service is stopping.");
        }

        private async Task CleanupExpiredReservationsAsync()
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                var expiredReservations = await unitOfWork.Reservations.GetExpiredReservationsAsync();

                if (!expiredReservations.Any())
                {
                    return;
                }

                _logger.LogInformation("Cleaning up {Count} expired reservations.", expiredReservations.Count());

                foreach (var reservation in expiredReservations)
                {
                    reservation.Status = "Expired";

                    await unitOfWork.AuditLogs.AddAuditLogAsync(new AuditLog
                    {
                        Action = "ReservationExpired",
                        UserId = reservation.UserId,
                        EntityType = "Reservation",
                        EntityId = reservation.Id.ToString(),
                        Details = $"La reserva de la butaca {reservation.Seat?.SeatNumber} en el sector '{reservation.Seat?.Sector?.Name}' para el evento '{reservation.Event?.Name}' expiró automáticamente por límite de tiempo.",
                        CreatedAt = DateTime.UtcNow
                    });
                }

                await unitOfWork.CompleteAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while cleaning up expired reservations.");
            }
        }
    }
}
