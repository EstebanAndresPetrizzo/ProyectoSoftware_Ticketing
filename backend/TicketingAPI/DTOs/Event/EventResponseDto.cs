using System;
using System.Collections.Generic;
using ProyectoSoftware_Ticketing.DTOs.Sector;

namespace ProyectoSoftware_Ticketing.DTOs.Event
{
    /// <summary>
    /// Representa la respuesta completa de un evento dentro del sistema de ticketing.
    /// Incluye la información principal del evento y los sectores disponibles para la venta.
    /// </summary>
    public class EventResponseDto
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

        /// <summary>
        /// Lista de sectores disponibles dentro del evento.
        /// </summary>
        public List<SectorDto> Sectors { get; set; }
    }
}