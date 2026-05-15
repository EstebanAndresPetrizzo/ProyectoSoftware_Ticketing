using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using TicketingAPI.Data;
using TicketingAPI.Models;
using TicketingAPI.Repositories;
using TicketingAPI.Application.Services.Implementations;
using ProyectoSoftware_Ticketing.DTOs.Reservation;

namespace ConcurrencyTest
{
    internal class Program
    {
        private const int ConcurrentRequests = 20;

        private static async Task<int> RunReservationTestAsync()
        {
            const string connectionString = "Data Source=ReservationTestDb;Mode=Memory;Cache=Shared";
            await using var keepAliveConnection = new SqliteConnection(connectionString);
            await keepAliveConnection.OpenAsync();

            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlite(keepAliveConnection)
                .Options;

            // Seed initial data.
            Guid reservedUserId;
            await using (var seedContext = new TestAppDbContext(options))
            {
                seedContext.Database.EnsureDeleted();
                seedContext.Database.EnsureCreated();

                var venue = new Venue { Name = "Test Venue", Capacity = 100, Status = "Active" };
                seedContext.Venues.Add(venue);
                await seedContext.SaveChangesAsync();

                var sector = new Sector
                {
                    VenueId = venue.Id,
                    Name = "Test Sector",
                    Price = 100m,
                    Position = "front",
                    Rows = 1,
                    Cols = 5,
                    Status = "Active"
                };
                seedContext.Sectors.Add(sector);
                await seedContext.SaveChangesAsync();

                var @event = new Event
                {
                    Name = "Test Event",
                    EventDate = DateTime.UtcNow.AddDays(10),
                    VenueId = venue.Id,
                    Status = "Active"
                };
                seedContext.Events.Add(@event);
                await seedContext.SaveChangesAsync();

                var seat = new Seat
                {
                    SectorId = sector.Id,
                    RowIdentifier = "A",
                    SeatNumber = 1,
                    Status = "Available"
                };
                seedContext.Seats.Add(seat);

                var user = new User
                {
                    Id = Guid.NewGuid(),
                    Name = "Concurrent User",
                    Email = "concurrent@example.com",
                    PasswordHash = "x"
                };
                seedContext.Users.Add(user);

                await seedContext.SaveChangesAsync();
                reservedUserId = user.Id;
            }

            var tasks = new List<Task<string>>();
            for (int i = 0; i < ConcurrentRequests; i++)
            {
                tasks.Add(Task.Run(async () =>
                {
                    await using var taskConnection = new SqliteConnection(connectionString);
                    await taskConnection.OpenAsync();

                    var contextOptions = new DbContextOptionsBuilder<AppDbContext>()
                        .UseSqlite(taskConnection)
                        .Options;

                    await using var ctx = new TestAppDbContext(contextOptions);
                    var seatRepo = new SeatRepository(ctx);
                    var reservationRepo = new ReservationRepository(ctx);
                    var auditRepo = new AuditLogRepository(ctx);
                    var eventRepo = new EventRepository(ctx);
                    var paymentRepo = new PaymentRepository(ctx);
                    using var unitOfWork = new UnitOfWork(ctx, eventRepo, seatRepo, reservationRepo, auditRepo, paymentRepo);
                    var reservationService = new ReservationService(unitOfWork);

                    try
                    {
                        var request = new CreateReservationRequestDto
                        {
                            EventId = 1,
                            SectorId = 1,
                            SeatId = 1,
                            UserId = reservedUserId
                        };

                        var result = await reservationService.CreateReservationAsync(request);
                        return $"SUCCESS: Reservation {result.Id} by {request.UserId}";
                    }
                    catch (Exception ex)
                    {
                        return $"FAIL: {ex.GetType().Name} - {ex.Message}";
                    }
                }));
            }

            var results = await Task.WhenAll(tasks);
            var successes = results.Count(r => r.StartsWith("SUCCESS:"));
            var failures = results.Length - successes;

            Console.WriteLine($"Total requests: {results.Length}");
            Console.WriteLine($"Successes: {successes}");
            Console.WriteLine($"Failures: {failures}");
            Console.WriteLine();
            foreach (var result in results)
            {
                Console.WriteLine(result);
            }

            return successes;
        }

        private static async Task Main()
        {
            try
            {
                var successCount = await RunReservationTestAsync();
                Console.WriteLine();
                Console.WriteLine(successCount == 1 ? "Test passed: only one reservation succeeded." : "Test result: concurrency behavior requires review.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error: {ex}");
            }
        }
    }

    internal class TestAppDbContext : AppDbContext
    {
        public TestAppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Seat>(entity =>
            {
                entity.Ignore(s => s.Version);
                entity.Property(s => s.Status).IsConcurrencyToken();
            });
        }
    }
}
