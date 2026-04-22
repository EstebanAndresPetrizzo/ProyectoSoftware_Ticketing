using ProyectoSoftware_Ticketing.DTOs.Seat;

namespace ProyectoSoftware_Ticketing.DTOs.Seat
{
    public class SeatDto
    {
        public int Id { get; set; }
        public int SectorId { get; set; }

        public string Row { get; set; }
        public int Number { get; set; }

        public SeatStatusDto Status { get; set; }
    }
}