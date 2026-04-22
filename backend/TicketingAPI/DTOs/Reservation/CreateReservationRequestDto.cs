namespace ProyectoSoftware_Ticketing.DTOs.Reservation
{
    public class CreateReservationRequestDto
    {
        public int EventId { get; set; }
        public int SectorId { get; set; }
        public int SeatId { get; set; }

        public string User { get; set; }
    }
}