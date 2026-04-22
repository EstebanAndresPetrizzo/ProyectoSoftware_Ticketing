using System;

namespace ProyectoSoftware_Ticketing.DTOs.Event
{
    /// <summary>
    /// Representa una vista resumida de un evento.
    /// Se utiliza para listados donde no se requiere el detalle completo.
    /// </summary>
    public class EventSummaryDto
    {
        /// <summary>
        /// Identificador único del evento.
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Nombre del evento (ej: "Coldplay en River").
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Ubicación o recinto donde se realiza el evento.
        /// </summary>
        public string Venue { get; set; }
        /// <summary>
        /// Fecha y hora programada del evento.
        /// </summary> 
        public DateTime Date { get; set; }
    }
}