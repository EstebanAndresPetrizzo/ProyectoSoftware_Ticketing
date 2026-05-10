namespace TicketingAPI.Application.Services.Interfaces
{
    public interface IPaymentProcessor
    {
        Task<PaymentProcessResult> ProcessAsync(ProcessPaymentCommand command);
        Task<RefundResult> RefundAsync(Guid transactionId, decimal amount);
    }
    
    public class PaymentProcessResult
    {
        public bool Success { get; set; }
        public string? TransactionId { get; set; }
        public string? ErrorMessage { get; set; }
    }
    
    public class RefundResult
    {
        public bool Success { get; set; }
        public string? RefundId { get; set; }
        public string? ErrorMessage { get; set; }
    }
    
    public class ProcessPaymentCommand
    {
        public string PaymentMethod { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string? CardNumber { get; set; }
        public string? CardholderName { get; set; }
        public string? ExpiryMonth { get; set; }
        public string? ExpiryYear { get; set; }
        public string? CVV { get; set; }
    }
}
