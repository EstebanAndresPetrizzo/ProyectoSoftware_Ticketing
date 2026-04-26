namespace ProyectoSoftware_Ticketing.DTOs.Common
{
    /// <summary>
    /// Metadata de paginación incluida en respuestas de endpoints que devuelven listas paginadas.
    /// </summary>
    public class PaginationDto
    {
        public int  Page        { get; set; }
        public int  PageSize    { get; set; }
        public int  TotalItems  { get; set; }
        public int  TotalPages  { get; set; }
        public bool HasNext     { get; set; }
        public bool HasPrevious { get; set; }
    }
}
