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
        public Reservation? Reservation { get; set; }
    }
}