namespace ProyectoSoftware_Ticketing.DTOs.Payment
{
    public class PaymentResponseDto
    {
        public Guid Id { get; set; }
        public Guid ReservationId { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string? TransactionId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ProcessedAt { get; set; }
        public string? FailureReason { get; set; }
    }
}
