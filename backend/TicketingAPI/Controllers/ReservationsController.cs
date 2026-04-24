using Microsoft.AspNetCore.Mvc;
using ProyectoSoftware_Ticketing.DTOs.Common;
using ProyectoSoftware_Ticketing.DTOs.Reservation;
using TicketingAPI.Application.Services.Interfaces;

namespace TicketingAPI.Controllers
{
    [ApiController]
    [Route("api/v1/reservations")]
    public class ReservationsController : ControllerBase
    {
        private readonly IReservationService _reservationService;

        public ReservationsController(IReservationService reservationService)
        {
            _reservationService = reservationService;
        }

        /// <summary>
        /// Implementa una versión inicial que registra el intento de reserva,
        /// cambia el estado de la butaca y guarda la auditoría.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ApiResponse<ReservationResponseDto>>> CreateReservation([FromBody] CreateReservationRequestDto request)
        {
            try
            {
                var result = await _reservationService.CreateReservationAsync(request);
                
                // Retornar 201 Created
                return CreatedAtAction(nameof(CreateReservation), new { id = result.Id }, new ApiResponse<ReservationResponseDto> { Success = true, Data = result });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ApiResponse<ReservationResponseDto> { Success = false, Error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                // Para la Fase 2 devolveremos 409 Conflict si hay problemas de concurrencia.
                // En esta Fase 1, sirve para cuando el asiento ya fue reservado (Estado != Available).
                return Conflict(new ApiResponse<ReservationResponseDto> { Success = false, Error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<ReservationResponseDto> { Success = false, Error = ex.Message });
            }
        }
    }
}
