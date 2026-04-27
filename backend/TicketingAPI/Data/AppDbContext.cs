using Microsoft.EntityFrameworkCore;
using TicketingAPI.Models;

namespace TicketingAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Venue> Venues { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Sector> Sectors { get; set; }
        public DbSet<Seat> Seats { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Venue
            modelBuilder.Entity<Venue>(entity =>
            {
                entity.HasKey(v => v.Id);
                entity.Property(v => v.Name).IsRequired().HasMaxLength(200);
                entity.Property(v => v.Status).IsRequired().HasDefaultValue("Active");
            });

            // Event
            modelBuilder.Entity<Event>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Status).IsRequired().HasDefaultValue("Active");

                entity.HasOne(e => e.Venue)
                      .WithMany(v => v.Events)
                      .HasForeignKey(e => e.VenueId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Sector
            modelBuilder.Entity<Sector>(entity =>
            {
                entity.HasKey(s => s.Id);
                entity.Property(s => s.Name).IsRequired().HasMaxLength(100);
                entity.Property(s => s.Price).HasColumnType("decimal(10,2)");
                entity.Property(s => s.Position).HasMaxLength(50);
                entity.Property(s => s.Status).IsRequired().HasDefaultValue("Active");

                entity.HasOne(s => s.Venue)
                      .WithMany(v => v.Sectors)
                      .HasForeignKey(s => s.VenueId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Seat
            modelBuilder.Entity<Seat>(entity =>
            {
                entity.HasKey(s => s.Id);
                entity.Property(s => s.RowIdentifier).IsRequired().HasMaxLength(10);
                entity.Property(s => s.Status).IsRequired().HasDefaultValue("Available");
                // control de concurrencia
                entity.Property(s => s.Version)
                      .HasColumnName("xmin")
                      .HasColumnType("xid")
                      .IsRowVersion();

                entity.HasOne(s => s.Sector)
                      .WithMany(sec => sec.Seats)
                      .HasForeignKey(s => s.SectorId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // User
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.Id);
                entity.Property(u => u.Name).IsRequired().HasMaxLength(100);
                entity.Property(u => u.Email).IsRequired().HasMaxLength(150);
                entity.HasIndex(u => u.Email).IsUnique();
            });

            // Relación Seat (1) → Reservations (N)
            modelBuilder.Entity<Reservation>(entity =>
            {
                entity.HasKey(r => r.Id);
                entity.Property(r => r.Status).IsRequired().HasDefaultValue("Pending");

                entity.HasOne(r => r.User)
                    .WithMany(u => u.Reservations)
                    .HasForeignKey(r => r.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                // CAMBIA ESTE BLOQUE:
                entity.HasOne(r => r.Seat)
                    .WithMany(s => s.Reservations) // Antes decía .WithOne(s => s.Reservation)
                    .HasForeignKey(r => r.SeatId)
                    .IsRequired(false)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(r => r.Event)
                    .WithMany(e => e.Reservations)
                    .HasForeignKey(r => r.EventId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // AuditLog
            modelBuilder.Entity<AuditLog>(entity =>
            {
                entity.HasKey(a => a.Id);
                entity.Property(a => a.Action).IsRequired().HasMaxLength(50);
                entity.Property(a => a.EntityType).IsRequired().HasMaxLength(50);
                entity.Property(a => a.EntityId).IsRequired().HasMaxLength(100);
                entity.Property(a => a.Details).HasColumnType("text");

                entity.HasOne(a => a.User)
                      .WithMany(u => u.AuditLogs)
                      .HasForeignKey(a => a.UserId)
                      .OnDelete(DeleteBehavior.SetNull);
            });
        }
    }
}