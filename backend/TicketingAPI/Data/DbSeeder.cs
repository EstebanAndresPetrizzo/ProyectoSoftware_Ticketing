using TicketingAPI.Models;

namespace TicketingAPI.Data
{
    public static class DbSeeder
    {
        public static void Seed(AppDbContext db)
        {
            // Si ya hay datos no volvemos a insertar
            if (db.Venues.Any()) return;

            // Usuario de prueba
            var user = new User
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                Name = "Usuario Test",
                Email = "test@ticketing.com",
                PasswordHash = "hash_placeholder"
            };
            db.Users.Add(user);

            // Venue 1: Estadio Nacional
            var venue1 = new Venue { Name = "Estadio Nacional", Status = "Active", Capacity = 500 };

            // Venue 2: Arena Central
            var venue2 = new Venue { Name = "Arena Central", Status = "Active", Capacity = 800 };

            db.Venues.AddRange(venue1, venue2);
            db.SaveChanges();

            // Evento 1: "Concierto de Rock" — Venue 1 — 2 sectores
            var evento1 = new Event { Name = "Concierto de Rock",              EventDate = DateTime.UtcNow.AddMonths(2), VenueId = venue1.Id, Status = "Active" };

            // Evento 2: "Final Copa eSports" — Venue 2 — 5 sectores
            var evento2 = new Event { Name = "Final Copa del Mundo de eSports", EventDate = DateTime.UtcNow.AddMonths(3), VenueId = venue2.Id, Status = "Active" };

            // Evento 3: "Gala de Premiación" — Venue 1 — 1 sector
            var evento3 = new Event { Name = "Gala de Premiación",             EventDate = DateTime.UtcNow.AddMonths(1), VenueId = venue1.Id, Status = "Active" };

            // Evento 4: "Festival de Jazz" — Venue 2 — 3 sectores
            var evento4 = new Event { Name = "Festival de Jazz",               EventDate = DateTime.UtcNow.AddMonths(4), VenueId = venue2.Id, Status = "Active" };

            // Evento 5: "Megafestival Aniversario" — Venue 2 — todos los sectores (6)
            var evento5 = new Event { Name = "Megafestival Aniversario",       EventDate = DateTime.UtcNow.AddMonths(5), VenueId = venue2.Id, Status = "Active" };

            db.Events.AddRange(evento1, evento2, evento3, evento4, evento5);
            db.SaveChanges();

            // Sectores Evento 1 (Venue 1) — 2 sectores
            // Name: nombre descriptivo del sector | Position: ubicacion fisica
            var s1_platea  = new Sector { VenueId = venue1.Id, Name = "Platea General", Price = 5000,  Rows = 5, Cols = 10, Position = "center", Status = "Active" };
            var s1_palco   = new Sector { VenueId = venue1.Id, Name = "Palco Preferencial", Price = 12000, Rows = 5, Cols = 10, Position = "front",  Status = "Active" };

            // Sectores Evento 2 (Venue 2) — 5 sectores
            var s2_palco   = new Sector { VenueId = venue2.Id, Name = "Palco Exclusivo",   Price = 25000, Rows = 2, Cols = 14, Position = "vip",    Status = "Active" };
            var s2_platea  = new Sector { VenueId = venue2.Id, Name = "Platea Central",    Price = 15000, Rows = 6, Cols = 12, Position = "center", Status = "Active" };
            var s2_tribuna = new Sector { VenueId = venue2.Id, Name = "Tribuna Frontal",   Price = 18000, Rows = 3, Cols = 10, Position = "front",  Status = "Active" };
            var s2_graderia= new Sector { VenueId = venue2.Id, Name = "Gradería Trasera",  Price = 8000,  Rows = 4, Cols = 12, Position = "back",   Status = "Active" };
            var s2_lateral = new Sector { VenueId = venue2.Id, Name = "Lateral Izquierdo", Price = 10000, Rows = 5, Cols = 8,  Position = "left",   Status = "Active" };

            // Sectores Evento 3 (Venue 1) — 1 sector
            var s3_palco   = new Sector { VenueId = venue1.Id, Name = "Palco Gala",        Price = 30000, Rows = 3, Cols = 10, Position = "vip",    Status = "Active" };

            // Sectores Evento 4 (Venue 2) — 3 sectores
            var s4_platea  = new Sector { VenueId = venue2.Id, Name = "Platea Jazz",       Price = 12000, Rows = 5, Cols = 14, Position = "center", Status = "Active" };
            var s4_tribuna = new Sector { VenueId = venue2.Id, Name = "Tribuna Principal", Price = 16000, Rows = 4, Cols = 10, Position = "front",  Status = "Active" };
            var s4_lateral = new Sector { VenueId = venue2.Id, Name = "Lateral Derecho",   Price = 9000,  Rows = 5, Cols = 8,  Position = "right",  Status = "Active" };

            // Sectores Evento 5 (Venue 2) — todos los sectores (los 6 posibles)
            var s5_palco   = new Sector { VenueId = venue2.Id, Name = "Palco Aniversario", Price = 28000, Rows = 2, Cols = 12, Position = "vip",    Status = "Active" };
            var s5_platea  = new Sector { VenueId = venue2.Id, Name = "Platea Central",    Price = 14000, Rows = 7, Cols = 14, Position = "center", Status = "Active" };
            var s5_tribuna = new Sector { VenueId = venue2.Id, Name = "Tribuna Frontal",   Price = 17000, Rows = 3, Cols = 12, Position = "front",  Status = "Active" };
            var s5_graderia= new Sector { VenueId = venue2.Id, Name = "Gradería Trasera",  Price = 7000,  Rows = 5, Cols = 10, Position = "back",   Status = "Active" };
            var s5_latIzq  = new Sector { VenueId = venue2.Id, Name = "Lateral Izquierdo", Price = 11000, Rows = 4, Cols = 8,  Position = "left",   Status = "Active" };
            var s5_latDer  = new Sector { VenueId = venue2.Id, Name = "Lateral Derecho",   Price = 11000, Rows = 4, Cols = 8,  Position = "right",  Status = "Active" };

            db.Sectors.AddRange(
                s1_platea, s1_palco,
                s2_palco, s2_platea, s2_tribuna, s2_graderia, s2_lateral,
                s3_palco,
                s4_platea, s4_tribuna, s4_lateral,
                s5_palco, s5_platea, s5_tribuna, s5_graderia, s5_latIzq, s5_latDer
            );
            db.SaveChanges();

            // Generar butacas para todos los sectores (Rows x Cols)
            var butacas = new List<Seat>();

            butacas.AddRange(GenerarButacas(s1_platea));
            butacas.AddRange(GenerarButacas(s1_palco));

            butacas.AddRange(GenerarButacas(s2_palco));
            butacas.AddRange(GenerarButacas(s2_platea));
            butacas.AddRange(GenerarButacas(s2_tribuna));
            butacas.AddRange(GenerarButacas(s2_graderia));
            butacas.AddRange(GenerarButacas(s2_lateral));

            butacas.AddRange(GenerarButacas(s3_palco));

            butacas.AddRange(GenerarButacas(s4_platea));
            butacas.AddRange(GenerarButacas(s4_tribuna));
            butacas.AddRange(GenerarButacas(s4_lateral));

            butacas.AddRange(GenerarButacas(s5_palco));
            butacas.AddRange(GenerarButacas(s5_platea));
            butacas.AddRange(GenerarButacas(s5_tribuna));
            butacas.AddRange(GenerarButacas(s5_graderia));
            butacas.AddRange(GenerarButacas(s5_latIzq));
            butacas.AddRange(GenerarButacas(s5_latDer));

            db.Seats.AddRange(butacas);
            db.SaveChanges();
        }

        // Genera butacas: fila A..Z por Rows, columna 1..Cols
        private static List<Seat> GenerarButacas(Sector sector)
        {
            var butacas = new List<Seat>();
            for (int fila = 0; fila < sector.Rows; fila++)
            {
                string rowId = ((char)('A' + fila)).ToString();
                for (int col = 1; col <= sector.Cols; col++)
                {
                    butacas.Add(new Seat
                    {
                        SectorId = sector.Id,
                        RowIdentifier = rowId,
                        SeatNumber = col,
                        Status = "Available"
                    });
                }
            }
            return butacas;
        }
    }
}