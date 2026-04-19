namespace TicketingAPI.Models
{
    public class Seat
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public int SectorId { get; set; }
        public string RowIdentifier { get; set; } = string.Empty;
        public int SeatNumber { get; set; }
        public string Status { get; set; } = "Available";
        [System.ComponentModel.DataAnnotations.Timestamp]
        public byte[] Version { get; set; } = null!;
        public Sector Sector { get; set; } = null!;
        public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
    }
}