namespace ProyectoSoftware_Ticketing.DTOs.Seat
{
    /// <summary>
    /// Enumera los posibles estados de una entrada (asiento) dentro del sistema de ticketing.
    /// Estos estados se utilizan para gestionar la disponibilidad de las entradas durante el proceso de compra.
    /// - Available: La entrada está disponible para ser reservada o comprada.
    /// - Reserved: La entrada ha sido reservada por un usuario, pero aún no se ha completado la compra. Puede ser liberada si el usuario no finaliza la compra en un tiempo determinado.
    /// - Sold: La entrada ha sido vendida y ya no está disponible para otros usuarios.
    /// </summary>
    public enum SeatStatusDto
    {
        Available = 0,
        Reserved = 1,
        Sold = 2
    }
}