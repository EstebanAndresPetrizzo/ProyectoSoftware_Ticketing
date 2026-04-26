using System;

namespace ProyectoSoftware_Ticketing.DTOs.Reservation
{
    /// <summary>
    /// Representa la respuesta completa de una reserva dentro del sistema de ticketing.
    /// Incluye la información principal de la reserva, como el evento, sector, asiento, usuario y estado de la reserva.
    /// </summary>
    public class ReservationResponseDto
    {
        /// <summary>
        /// Identificador único de la reserva.
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// Identificador del evento para el cual se realizó la reserva.
        /// </summary>

        public int EventId { get; set; }
        /// <summary>
        /// Identificador del sector dentro del evento para el cual se realizó la reserva.
        /// </summary>
        public int SectorId { get; set; }
        /// <summary>
        /// Identificador del asiento que se reservó.
        /// </summary>
        public int SeatId { get; set; }

        /// <summary>
        /// Identificador del usuario que realizó la reserva.
        /// </summary>
        public Guid UserId { get; set; }
        /// <summary>
        /// Estado actual de la reserva.
        /// </summary>
        public string Status { get; set; } = string.Empty;
        /// <summary>
        /// Fecha y hora en que se creó la reserva.
        /// </summary>
        public DateTime CreatedAt { get; set; }
    }
}