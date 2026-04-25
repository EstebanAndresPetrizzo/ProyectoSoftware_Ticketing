namespace TicketingAPI.Models
{
    public class Venue
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Status { get; set; } = "Active";
        public int Capacity { get; set; }

        public ICollection<Event> Events { get; set; } = new List<Event>();
        public ICollection<Sector> Sectors { get; set; } = new List<Sector>();
    }
}
