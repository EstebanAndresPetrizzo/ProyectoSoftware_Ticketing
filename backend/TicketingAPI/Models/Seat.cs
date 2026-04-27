namespace TicketingAPI.Models
{
    public class Seat
    {
        public int Id { get; set; }
        public int SectorId { get; set; }
        public string RowIdentifier { get; set; } = string.Empty;
        public int SeatNumber { get; set; }
        public string Status { get; set; } = "Available";
        public uint Version { get; set; }
        public Sector Sector { get; set; } = null!;
        // Relación inversa para reservas (una silla puede tener 0 o 1 reserva activa)
        public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
    }
}