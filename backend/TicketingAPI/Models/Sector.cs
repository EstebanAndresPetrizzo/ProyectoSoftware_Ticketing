namespace TicketingAPI.Models
{
    public class Sector
    {
        public int Id { get; set; }
        public int VenueId { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Rows { get; set; }
        public int Cols { get; set; }
        public string Position { get; set; } = string.Empty;
        public string Status { get; set; } = "Active";
        public Venue Venue { get; set; } = null!;
        public ICollection<Seat> Seats { get; set; } = new List<Seat>();
    }
}