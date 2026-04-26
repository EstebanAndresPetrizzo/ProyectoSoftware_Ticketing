using Microsoft.AspNetCore.Mvc;
using ProyectoSoftware_Ticketing.DTOs.Common;
using ProyectoSoftware_Ticketing.DTOs.Event;
using ProyectoSoftware_Ticketing.DTOs.Seat;
using TicketingAPI.Application.Services.Interfaces;

namespace TicketingAPI.Controllers
{
    [ApiController]
    [Route("api/v1/events")]
    public class EventsController : ControllerBase
    {
        private readonly IEventService _eventService;
        private readonly ISeatService _seatService;

        public EventsController(IEventService eventService, ISeatService seatService)
        {
            _eventService = eventService;
            _seatService = seatService;
        }

        /// <summary>
        /// Retorna el catálogo paginado de eventos.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<EventSummaryDto>>>> GetEvents([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var (items, totalItems) = await _eventService.GetPagedEventsAsync(page, pageSize);
                var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

                return Ok(new ApiResponse<IEnumerable<EventSummaryDto>>
                {
                    Success = true,
                    Data    = items,
                    Pagination = new PaginationDto
                    {
                        Page        = page,
                        PageSize    = pageSize,
                        TotalItems  = totalItems,
                        TotalPages  = totalPages,
                        HasNext     = page < totalPages,
                        HasPrevious = page > 1
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<IEnumerable<EventSummaryDto>> { Success = false, Error = ex.Message });
            }
        }


        /// <summary>
        /// Devuelve el estado actual de todas las butacas para un evento.
        /// </summary>
        [HttpGet("{id}/seats")]
        public async Task<ActionResult<ApiResponse<EventSeatMapDto>>> GetSeats([FromRoute] int id)
        {
            try
            {
                var seatMap = await _seatService.GetSeatMapByEventIdAsync(id);

                return Ok(new ApiResponse<EventSeatMapDto>
                {
                    Success = true,
                    Data = seatMap
                });
            }
            catch (ArgumentException ex)
            {
                return NotFound(new ApiResponse<EventSeatMapDto>
                {
                    Success = false,
                    Error = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<EventSeatMapDto>
                {
                    Success = false,
                    Error = ex.Message
                });
            }
        }
    }
}
