namespace ProyectoSoftware_Ticketing.DTOs.Payment
{
    public class CreatePaymentRequestDto
    {
        public Guid ReservationId { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; } = string.Empty; // "Card", "DigitalWallet", "Mock"
        
        // Para simulación/prueba
        public string? CardNumber { get; set; }
        public string? CardholderName { get; set; }
        public string? ExpiryMonth { get; set; }
        public string? ExpiryYear { get; set; }
        public string? CVV { get; set; }
    }
}
