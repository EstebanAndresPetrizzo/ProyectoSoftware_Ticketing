using ProyectoSoftware_Ticketing.DTOs.Payment;

namespace TicketingAPI.Application.Services.Interfaces
{
    public interface IPaymentService
    {
        Task<PaymentResponseDto> ProcessPaymentAsync(CreatePaymentRequestDto request, Guid userId);
        Task<List<PaymentResponseDto>> ProcessBulkPaymentAsync(CreateBulkPaymentRequestDto request, Guid userId);
        Task<PaymentResponseDto> GetPaymentAsync(Guid paymentId);
        Task<PaymentResponseDto> RefundPaymentAsync(Guid paymentId);
    }
}
