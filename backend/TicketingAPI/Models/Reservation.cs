namespace TicketingAPI.Models
{
    public class Reservation
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public int UserId { get; set; }
        public Guid? SeatId { get; set; }
        public string Status { get; set; } = "Pending"; //"Pending, Paid,Expired"
        public DateTime ReservedAt { get; set; } = DateTime.UtcNow;
        public DateTime ExpiresAt { get; set; }
        public User User { get; set; } = null!;
        public Seat? Seat { get; set; } = null!;
    }
}