namespace ProyectoSoftware_Ticketing.DTOs.Common
{
    /// <summary>
    /// Clase genérica para representar la respuesta de una API, incluyendo información sobre el éxito de la operación, los datos resultantes y cualquier error que pueda haber ocurrido.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ApiResponse<T>
    {
        /// <summary>
        /// Indica si la operación fue exitosa.
        /// </summary>
        public bool Success { get; set; }
        /// <summary>
        /// Datos resultantes de la operación.
        /// </summary>
        public T Data { get; set; } = default(T)!;
        /// <summary>
        /// Metadata de paginación. Solo se incluye en endpoints que devuelven listas paginadas.
        /// </summary>
        public PaginationDto? Pagination { get; set; }
        /// <summary>
        /// Mensaje de error en caso de que la operación haya fallado.
        /// </summary>
        public string Error { get; set; } = string.Empty;
    }
}