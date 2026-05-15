namespace TicketingAPI.Models
{
    public class Payment
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid ReservationId { get; set; }
        public Guid UserId { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; } = string.Empty; // "Card", "DigitalWallet", "Mock"
        public string Status { get; set; } = "Pending"; // "Pending", "Completed", "Failed", "Refunded"
        public string? TransactionId { get; set; } // ID externo del proveedor
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ProcessedAt { get; set; }
        public string? FailureReason { get; set; }
        
        // Relaciones
        public Reservation Reservation { get; set; } = null!;
        public User User { get; set; } = null!;
    }
}
