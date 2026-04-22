using System;

namespace ProyectoSoftware_Ticketing.DTOs.Reservation
{
    public class ReservationResponseDto
    {
        public int Id { get; set; }

        public int EventId { get; set; }
        public int SectorId { get; set; }
        public int SeatId { get; set; }

        public string User { get; set; }
        public string Status { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}