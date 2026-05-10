using Microsoft.AspNetCore.Mvc;
using ProyectoSoftware_Ticketing.DTOs.Common;
using ProyectoSoftware_Ticketing.DTOs.Payment;
using TicketingAPI.Application.Services.Interfaces;

namespace TicketingAPI.Controllers
{
    [ApiController]
    [Route("api/v1/payments")]
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentsController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        /// <summary>
        /// Procesa un pago para una reservación.
        /// Para simulación, usa la tarjeta "4000000000000002" para simular rechazo.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ApiResponse<PaymentResponseDto>>> ProcessPayment(
            [FromBody] CreatePaymentRequestDto request, 
            [FromHeader(Name = "X-User-Id")] Guid userId)
        {
            try
            {
                var result = await _paymentService.ProcessPaymentAsync(request, userId);
                return Ok(new ApiResponse<PaymentResponseDto> { Success = true, Data = result });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ApiResponse<PaymentResponseDto> { Success = false, Error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new ApiResponse<PaymentResponseDto> { Success = false, Error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<PaymentResponseDto> { Success = false, Error = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene los detalles de un pago específico.
        /// </summary>
        [HttpGet("{paymentId}")]
        public async Task<ActionResult<ApiResponse<PaymentResponseDto>>> GetPayment(Guid paymentId)
        {
            try
            {
                var result = await _paymentService.GetPaymentAsync(paymentId);
                return Ok(new ApiResponse<PaymentResponseDto> { Success = true, Data = result });
            }
            catch (ArgumentException ex)
            {
                return NotFound(new ApiResponse<PaymentResponseDto> { Success = false, Error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<PaymentResponseDto> { Success = false, Error = ex.Message });
            }
        }

        /// <summary>
        /// Reembolsa un pago completado.
        /// </summary>
        [HttpPost("{paymentId}/refund")]
        public async Task<ActionResult<ApiResponse<PaymentResponseDto>>> RefundPayment(Guid paymentId)
        {
            try
            {
                var result = await _paymentService.RefundPaymentAsync(paymentId);
                return Ok(new ApiResponse<PaymentResponseDto> { Success = true, Data = result });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ApiResponse<PaymentResponseDto> { Success = false, Error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new ApiResponse<PaymentResponseDto> { Success = false, Error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<PaymentResponseDto> { Success = false, Error = ex.Message });
            }
        }
    }
}
