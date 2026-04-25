namespace TicketingAPI.Models
{
    public class Event
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime EventDate { get; set; }
        public int VenueId { get; set; }
        public string Status { get; set; } = "Active";

        public Venue Venue { get; set; } = null!;
    }
}