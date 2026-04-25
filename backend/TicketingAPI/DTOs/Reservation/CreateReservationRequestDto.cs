namespace ProyectoSoftware_Ticketing.DTOs.Reservation
{
    /// <summary>
    /// Representa la información necesaria para crear una nueva reserva dentro del sistema de ticketing.
    /// </summary>
    public class CreateReservationRequestDto
    {
        /// <summary>
        /// Identificador del evento para el cual se está realizando la reserva.
        /// </summary>
        public int EventId { get; set; }
        /// <summary>
        /// Identificador del sector dentro del evento para el cual se desea reservar la entrada.
        /// </summary>
        public int SectorId { get; set; }
        /// <summary>
        /// Identificador del asiento que se desea reservar.
        /// </summary>
        public int SeatId { get; set; }
        /// <summary>
        /// Identificador del usuario que está realizando la reserva.
        /// </summary>
        public Guid UserId { get; set; }
    }
}