using TicketingAPI.Models;

namespace TicketingAPI.Data
{
    public static class DbSeeder
    {
        public static void Seed(AppDbContext db)
        {
            // Si ya hay datos no volvemos a insertar
            if (db.Events.Any()) return;

            // Usuario de prueba
            var user = new User
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                Name = "Usuario Test",
                Email = "test@ticketing.com",
                PasswordHash = "hash_placeholder"
            };
            db.Users.Add(user);

            // Evento de prueba
            var evento = new Event
            {
                Name = "Concierto de Rock",
                EventDate = DateTime.UtcNow.AddMonths(2),
                Venue = "Estadio Nacional",
                Status = "Active"
            };
            db.Events.Add(evento);
            db.SaveChanges();

            // Sector 1 — Campo
            var sectorCampo = new Sector
            {
                EventId = evento.Id,
                Name = "Campo",
                Price = 5000,
                Position = "center",
                Capacity = 50
            };

            // Sector 2 — Platea
            var sectorPlatea = new Sector
            {
                EventId = evento.Id,
                Name = "Platea",
                Price = 12000,
                Position = "front",
                Capacity = 50
            };

            db.Sectors.AddRange(sectorCampo, sectorPlatea);
            db.SaveChanges();

            // 50 butacas para Campo
            var butacasCampo = new List<Seat>();
            for (int numero = 1; numero <= 50; numero++)
            {
                butacasCampo.Add(new Seat
                {
                    SectorId = sectorCampo.Id,
                    RowIdentifier = ObtenerFila(numero),
                    SeatNumber = numero,
                    Status = "Available"
                });
            }

            // 50 butacas para Platea
            var butacasPlatea = new List<Seat>();
            for (int numero = 1; numero <= 50; numero++)
            {
                butacasPlatea.Add(new Seat
                {
                    SectorId = sectorPlatea.Id,
                    RowIdentifier = ObtenerFila(numero),
                    SeatNumber = numero,
                    Status = "Available"
                });
            }

            db.Seats.AddRange(butacasCampo);
            db.Seats.AddRange(butacasPlatea);
            db.SaveChanges();
        }

        // Convierte número de butaca a fila (1-10 → A, 11-20 → B, etc.)
        private static string ObtenerFila(int numero)
        {
            int filaIndex = (numero - 1) / 10;
            return ((char)('A' + filaIndex)).ToString();
        }
    }
}