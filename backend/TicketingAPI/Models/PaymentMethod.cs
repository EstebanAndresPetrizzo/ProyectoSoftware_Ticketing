namespace TicketingAPI.Models
{
    public class PaymentMethod
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid UserId { get; set; }
        public string Type { get; set; } = string.Empty; // "Card", "DigitalWallet"
        public string MaskedData { get; set; } = string.Empty; // Últimos 4 dígitos, email, etc.
        public bool IsDefault { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public User User { get; set; } = null!;
    }
}
