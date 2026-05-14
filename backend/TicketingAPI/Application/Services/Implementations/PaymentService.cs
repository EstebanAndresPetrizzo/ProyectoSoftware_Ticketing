using ProyectoSoftware_Ticketing.DTOs.Payment;
using TicketingAPI.Application.Services.Interfaces;
using TicketingAPI.Models;
using TicketingAPI.Repositories;

namespace TicketingAPI.Application.Services.Implementations
{
    public class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPaymentProcessor _paymentProcessor;

        public PaymentService(IUnitOfWork unitOfWork, IPaymentProcessor paymentProcessor)
        {
            _unitOfWork = unitOfWork;
            _paymentProcessor = paymentProcessor;
        }

        public async Task<PaymentResponseDto> ProcessPaymentAsync(CreatePaymentRequestDto request, Guid userId)
        {
            // 1. Validar que la reserva existe y está en estado "Pending"
            var reservation = await _unitOfWork.Reservations.GetByIdAsync(request.ReservationId);
            if (reservation == null || reservation.Status != "Pending")
                throw new InvalidOperationException("Reserva inválida o ya pagada");

            // 2. Procesar pago con el processor
            var command = new ProcessPaymentCommand
            {
                PaymentMethod = request.PaymentMethod,
                Amount = request.Amount,
                CardNumber = request.CardNumber,
                CardholderName = request.CardholderName,
                ExpiryMonth = request.ExpiryMonth,
                ExpiryYear = request.ExpiryYear,
                CVV = request.CVV
            };

            var result = await _paymentProcessor.ProcessAsync(command);

            // 3. Crear registro de Payment
            var payment = new Payment
            {
                ReservationId = request.ReservationId,
                UserId = userId,
                Amount = request.Amount,
                PaymentMethod = request.PaymentMethod,
                Status = result.Success ? "Completed" : "Failed",
                TransactionId = result.TransactionId,
                FailureReason = result.ErrorMessage,
                ProcessedAt = DateTime.UtcNow
            };
            await _unitOfWork.Payments.AddAsync(payment);

            // 4. Si pago exitoso, actualizar estado de reserva y asiento
            if (result.Success && reservation.Seat != null)
            {
                reservation.Status = "Paid";
                reservation.Seat.Status = "Purchased"; // Cambiar asiento a comprado
                await _unitOfWork.Reservations.UpdateAsync(reservation);
            }

            // 5. Auditar
            var auditLog = new AuditLog
            {
                UserId = userId,
                Action = result.Success ? "PaymentCompleted" : "PaymentFailed",
                EntityType = "Payment",
                EntityId = payment.Id.ToString(),
                CreatedAt = DateTime.UtcNow,
                Details = $"Pago de ${request.Amount} - {result.ErrorMessage ?? "Exitoso"}"
            };
            await _unitOfWork.AuditLogs.AddAuditLogAsync(auditLog);

            await _unitOfWork.CompleteAsync();

            return MapToDto(payment);
        }

        public async Task<List<PaymentResponseDto>> ProcessBulkPaymentAsync(CreateBulkPaymentRequestDto request, Guid userId)
        {
            if (request.ReservationIds == null || request.ReservationIds.Count == 0)
                throw new ArgumentException("Debes incluir al menos una reserva para procesar el pago");

            // 1. Validar que todas las reservas existan y estén en estado "Pending"
            var reservations = new List<Reservation>();
            foreach (var reservationId in request.ReservationIds)
            {
                var reservation = await _unitOfWork.Reservations.GetByIdAsync(reservationId);
                if (reservation == null || reservation.Status != "Pending")
                    throw new InvalidOperationException($"Reserva {reservationId} inválida o ya pagada");
                reservations.Add(reservation);
            }

            // 2. Procesar un único pago para todas las reservas
            var command = new ProcessPaymentCommand
            {
                PaymentMethod = request.PaymentMethod,
                Amount = request.Amount,
                CardNumber = request.CardNumber,
                CardholderName = request.CardholderName,
                ExpiryMonth = request.ExpiryMonth,
                ExpiryYear = request.ExpiryYear,
                CVV = request.CVV
            };

            var result = await _paymentProcessor.ProcessAsync(command);

            var paymentResponses = new List<PaymentResponseDto>();

            // 3. Crear registros de Payment para cada reserva
            if (result.Success)
            {
                // Si pago exitoso, crear un Payment por cada reserva y actualizar estados
                foreach (var reservation in reservations)
                {
                    var payment = new Payment
                    {
                        ReservationId = reservation.Id,
                        UserId = userId,
                        Amount = request.Amount / reservations.Count, // Dividir el monto entre las reservas
                        PaymentMethod = request.PaymentMethod,
                        Status = "Completed",
                        TransactionId = result.TransactionId,
                        FailureReason = null,
                        ProcessedAt = DateTime.UtcNow
                    };
                    await _unitOfWork.Payments.AddAsync(payment);

                    // Auditar cada pago individualmente
                    var auditLog = new AuditLog
                    {
                        UserId = userId,
                        Action = "PaymentCompleted",
                        EntityType = "Payment",
                        EntityId = payment.Id.ToString(),
                        CreatedAt = DateTime.UtcNow,
                        Details = $"Pago de ${payment.Amount} para reserva {reservation.Id} (parte de pago múltiple de {reservations.Count} reservas)"
                    };
                    await _unitOfWork.AuditLogs.AddAuditLogAsync(auditLog);

                    // Actualizar reserva y asiento
                    reservation.Status = "Paid";
                    if (reservation.Seat != null)
                        reservation.Seat.Status = "Purchased";
                    await _unitOfWork.Reservations.UpdateAsync(reservation);

                    paymentResponses.Add(MapToDto(payment));
                }
            }
            else
            {
                // Si pago falla, crear un Payment para cada reserva con estado Failed
                foreach (var reservation in reservations)
                {
                    var payment = new Payment
                    {
                        ReservationId = reservation.Id,
                        UserId = userId,
                        Amount = request.Amount / reservations.Count,
                        PaymentMethod = request.PaymentMethod,
                        Status = "Failed",
                        TransactionId = result.TransactionId,
                        FailureReason = result.ErrorMessage,
                        ProcessedAt = DateTime.UtcNow
                    };
                    await _unitOfWork.Payments.AddAsync(payment);

                    // Auditar cada pago fallido individualmente
                    var auditLog = new AuditLog
                    {
                        UserId = userId,
                        Action = "PaymentFailed",
                        EntityType = "Payment",
                        EntityId = payment.Id.ToString(),
                        CreatedAt = DateTime.UtcNow,
                        Details = $"Pago fallido de ${payment.Amount} para reserva {reservation.Id} (parte de pago múltiple de {reservations.Count} reservas) - {result.ErrorMessage}"
                    };
                    await _unitOfWork.AuditLogs.AddAuditLogAsync(auditLog);

                    paymentResponses.Add(MapToDto(payment));
                }
            }

            await _unitOfWork.CompleteAsync();

            return paymentResponses;
        }

        public async Task<PaymentResponseDto> GetPaymentAsync(Guid paymentId)
        {
            var payment = await _unitOfWork.Payments.GetByIdAsync(paymentId);
            if (payment == null)
                throw new ArgumentException("Pago no encontrado");
            
            return MapToDto(payment);
        }

        public async Task<PaymentResponseDto> RefundPaymentAsync(Guid paymentId)
        {
            var payment = await _unitOfWork.Payments.GetByIdAsync(paymentId);
            if (payment?.Status != "Completed")
                throw new InvalidOperationException("Solo se pueden reembolsar pagos completados");

            if (string.IsNullOrEmpty(payment.TransactionId))
                throw new InvalidOperationException("Transacción no tiene ID para reembolso");

            var refundResult = await _paymentProcessor.RefundAsync(
                Guid.Parse(payment.TransactionId), 
                payment.Amount
            );

            if (refundResult.Success)
            {
                payment.Status = "Refunded";
                var reservation = await _unitOfWork.Reservations.GetByIdAsync(payment.ReservationId);
                if (reservation != null)
                {
                    reservation.Status = "Cancelled";
                    if (reservation.Seat != null)
                        reservation.Seat.Status = "Available";
                }
            }

            await _unitOfWork.CompleteAsync();
            return MapToDto(payment);
        }

        private PaymentResponseDto MapToDto(Payment payment) => new()
        {
            Id = payment.Id,
            ReservationId = payment.ReservationId,
            Amount = payment.Amount,
            PaymentMethod = payment.PaymentMethod,
            Status = payment.Status,
            TransactionId = payment.TransactionId,
            CreatedAt = payment.CreatedAt,
            ProcessedAt = payment.ProcessedAt,
            FailureReason = payment.FailureReason
        };
    }
}
