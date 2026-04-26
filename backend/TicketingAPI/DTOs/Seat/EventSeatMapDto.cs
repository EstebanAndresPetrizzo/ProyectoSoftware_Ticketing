namespace ProyectoSoftware_Ticketing.DTOs.Seat
{
    /// <summary>
    /// Representa el mapa completo de asientos de un evento.
    /// </summary>
    public class EventSeatMapDto
    {
        public int EventId { get; set; }

        public string EventName { get; set; } = string.Empty;

        public string VenueName { get; set; } = string.Empty;

        public List<SectorSeatMapDto> Sectors { get; set; } = new List<SectorSeatMapDto>();
    }
}