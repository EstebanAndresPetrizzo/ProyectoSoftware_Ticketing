using TicketingAPI.Application.Services.Interfaces;

namespace TicketingAPI.Application.Services.Implementations
{
    public class MockPaymentProcessor : IPaymentProcessor
    {
        public async Task<PaymentProcessResult> ProcessAsync(ProcessPaymentCommand command)
        {
            // Simular delay de procesamiento
            await Task.Delay(500);
            
            // Simular fallo si card es "4000000000000002"
            if (command.CardNumber == "4000000000000002")
            {
                return new PaymentProcessResult
                {
                    Success = false,
                    ErrorMessage = "Tarjeta rechazada (simulación)"
                };
            }
            
            // Simular éxito
            return new PaymentProcessResult
            {
                Success = true,
                TransactionId = $"MOCK_{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}"
            };
        }
        
        public async Task<RefundResult> RefundAsync(Guid transactionId, decimal amount)
        {
            await Task.Delay(300);
            return new RefundResult
            {
                Success = true,
                RefundId = $"REFUND_{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}"
            };
        }
    }
}
