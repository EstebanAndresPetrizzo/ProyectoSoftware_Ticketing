namespace ProyectoSoftware_Ticketing.DTOs.Seat
{
    /// <summary>
    /// Representa un sector del estadio con su metadata y asientos.
    /// </summary>
    public class SectorSeatMapDto
    {
        public int SectorId { get; set; }

        public string Name { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public int Rows { get; set; }

        public int Cols { get; set; }

        public string Position { get; set; } = string.Empty;

        public List<SeatDto> Seats { get; set; } = new List<SeatDto>();
    }
}