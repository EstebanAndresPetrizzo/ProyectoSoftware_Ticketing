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
            var venue2 = new Venue { Name = "Arena Central",    Status = "Active", Capacity = 800 };
            var venue3 = new Venue { Name = "Sala Panorámica",  Status = "Active", Capacity = 300 };

            db.Venues.AddRange(venue1, venue2, venue3);
            db.SaveChanges();

            // Sectores Venue 1 - Estadio Nacional
            var v1_vip    = new Sector { VenueId = venue1.Id, Position = "vip",    Name = "Palco VIP",         Price = 30000, Rows = 2, Cols = 10, Status = "Active" };
            var v1_front  = new Sector { VenueId = venue1.Id, Position = "front",  Name = "Tribuna Frontal",   Price = 12000, Rows = 5, Cols = 10, Status = "Active" };
            var v1_center = new Sector { VenueId = venue1.Id, Position = "center", Name = "Platea Central",    Price = 8000,  Rows = 5, Cols = 10, Status = "Active" };
            var v1_back   = new Sector { VenueId = venue1.Id, Position = "back",   Name = "Gradería Trasera",  Price = 5000,  Rows = 5, Cols = 10, Status = "Active" };

            // Sectores Venue 2 - Arena Central
            var v2_vip    = new Sector { VenueId = venue2.Id, Position = "vip",    Name = "Palco Exclusivo",   Price = 28000, Rows = 2, Cols = 14, Status = "Active" };
            var v2_front  = new Sector { VenueId = venue2.Id, Position = "front",  Name = "Tribuna Frontal",   Price = 18000, Rows = 4, Cols = 12, Status = "Active" };
            var v2_center = new Sector { VenueId = venue2.Id, Position = "center", Name = "Platea Central",    Price = 15000, Rows = 6, Cols = 14, Status = "Active" };
            var v2_back   = new Sector { VenueId = venue2.Id, Position = "back",   Name = "Gradería Trasera",  Price = 7000,  Rows = 5, Cols = 12, Status = "Active" };
            var v2_left   = new Sector { VenueId = venue2.Id, Position = "left",   Name = "Lateral Izquierdo", Price = 10000, Rows = 4, Cols = 10, Status = "Active" };
            var v2_right  = new Sector { VenueId = venue2.Id, Position = "right",  Name = "Lateral Derecho",   Price = 10000, Rows = 4, Cols = 10, Status = "Active" };

            // Sectores Venue 3 - Sala Panorámica
            var v3_vip    = new Sector { VenueId = venue3.Id, Position = "vip",    Name = "Palco Panorámico",  Price = 35000, Rows = 2, Cols = 14,  Status = "Active" };
            var v3_front  = new Sector { VenueId = venue3.Id, Position = "front",  Name = "Platea Frontal",    Price = 20000, Rows = 3, Cols = 14, Status = "Active" };
            var v3_center = new Sector { VenueId = venue3.Id, Position = "center", Name = "Platea Central",    Price = 17000, Rows = 5, Cols = 14, Status = "Active" };
            var v3_left   = new Sector { VenueId = venue3.Id, Position = "left",   Name = "Lateral Izquierdo", Price = 14000, Rows = 5, Cols = 8,  Status = "Active" };
            var v3_right  = new Sector { VenueId = venue3.Id, Position = "right",  Name = "Lateral Derecho",   Price = 14000, Rows = 5, Cols = 8,  Status = "Active" };

            db.Sectors.AddRange(
                v1_vip, v1_front, v1_center, v1_back,
                v2_vip, v2_front, v2_center, v2_back, v2_left, v2_right,
                v3_vip, v3_front, v3_center, v3_left, v3_right
            );
            db.SaveChanges();

            // Eventos
            db.Events.AddRange(
                // Venue 1 - Estadio Nacional
                new Event { Name = "Concierto de Rock",               EventDate = DateTime.UtcNow.AddMonths(2), VenueId = venue1.Id, Status = "Active" },
                new Event { Name = "Gala de Premiación",              EventDate = DateTime.UtcNow.AddMonths(1), VenueId = venue1.Id, Status = "Active" },

                // Venue 2 - Arena Central
                new Event { Name = "Final Copa del Mundo de eSports", EventDate = DateTime.UtcNow.AddMonths(3), VenueId = venue2.Id, Status = "Active" },
                new Event { Name = "Festival de Jazz",                 EventDate = DateTime.UtcNow.AddMonths(4), VenueId = venue2.Id, Status = "Active" },
                new Event { Name = "Megafestival Aniversario",         EventDate = DateTime.UtcNow.AddMonths(5), VenueId = venue2.Id, Status = "Active" },

                // Venue 3 - Sala Panorámica
                new Event { Name = "Noche de Ópera",                  EventDate = DateTime.UtcNow.AddMonths(2), VenueId = venue3.Id, Status = "Active" },
                new Event { Name = "Stand-Up Comedy Night",            EventDate = DateTime.UtcNow.AddMonths(3), VenueId = venue3.Id, Status = "Active" }
            );
            db.SaveChanges();

            // Butacas
            var allSectors = new[]
            {
                v1_vip, v1_front, v1_center, v1_back,
                v2_vip, v2_front, v2_center, v2_back, v2_left, v2_right,
                v3_vip, v3_front, v3_center, v3_left, v3_right
            };

            var butacas = new List<Seat>();
            foreach (var sector in allSectors)
                butacas.AddRange(GenerarButacas(sector));

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
                        SectorId      = sector.Id,
                        RowIdentifier = rowId,
                        SeatNumber    = col,
                        Status        = "Available"
                    });
                }
            }
            return butacas;
        }
    }
}