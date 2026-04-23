using ProyectoSoftware_Ticketing.DTOs.Seat;

namespace ProyectoSoftware_Ticketing.DTOs.Seat
{
    /// <summary>
    /// Representa un asiento dentro de un sector de un evento, con su información básica y estado actual.
    /// </summary>
    public class SeatDto
    {
        /// <summary>
        /// Identificador único del asiento.
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Identificador del sector al que pertenece el asiento.
        /// </summary>
        public int SectorId { get; set; }

        /// <summary>
        /// Fila a la que pertenece el asiento.
        /// </summary>
        public string Row { get; set; }
        /// <summary>
        /// Número del asiento dentro de su fila.
        /// </summary>
        public int Number { get; set; }

        /// <summary>
        /// Estado actual del asiento.
        /// </summary>
        public SeatStatusDto Status { get; set; }
    }
}