namespace ProyectoSoftware_Ticketing.DTOs.Sector
{
    /// <summary>
    /// Representa un sector dentro de un evento, con su información básica.
    /// </summary>
    public class SectorDto
    {
        /// <summary>
        /// Identificador único del sector.
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Nombre del sector (ej: "Platea Alta", "Campo", "VIP").
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Precio por entrada en el sector.
        /// </summary>
        public decimal Price { get; set; }
        /// <summary>
        /// Capacidad máxima de entradas para el sector.
        /// </summary>
        public int Capacity { get; set; }
    }
}