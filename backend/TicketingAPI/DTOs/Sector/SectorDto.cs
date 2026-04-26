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
        /// Número de filas en el sector.
        /// </summary>
        public int Rows { get; set; }
        
        /// <summary>
        /// Número de columnas por fila en el sector.
        /// </summary>
        public int Cols { get; set; }

        /// <summary>
        /// Posición del sector dentro del recinto (ej. Center, Sides).
        /// </summary>
        public string Position { get; set; }
    }
}