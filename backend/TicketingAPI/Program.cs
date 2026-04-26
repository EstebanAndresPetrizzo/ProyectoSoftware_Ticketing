using Microsoft.EntityFrameworkCore;
using TicketingAPI.Data;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);
// Configuración de servicios
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Configura el serializador JSON para convertir los enums a sus representaciones 
        // de cadena en lugar de números enteros.
        // Esto es especialmente útil para el enum SeatStatusDto, ya que mejora la legibilidad de las respuestas JSON.
        options.JsonSerializerOptions.Converters.Add(
            new JsonStringEnumConverter()
        );
    });;
// Generador nativo de .NET 10 — forzamos OpenAPI 3.0 para compatibilidad con Swagger UI
builder.Services.AddOpenApi(options =>
{
    options.OpenApiVersion = Microsoft.OpenApi.OpenApiSpecVersion.OpenApi3_0;
});
// Conexión a PostgreSQL con EF Core
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// CORS
builder.Services.AddScoped<TicketingAPI.Repositories.IEventRepository, TicketingAPI.Repositories.EventRepository>();
builder.Services.AddScoped<TicketingAPI.Repositories.ISeatRepository, TicketingAPI.Repositories.SeatRepository>();
builder.Services.AddScoped<TicketingAPI.Repositories.IReservationRepository, TicketingAPI.Repositories.ReservationRepository>();
builder.Services.AddScoped<TicketingAPI.Repositories.IAuditLogRepository, TicketingAPI.Repositories.AuditLogRepository>();
builder.Services.AddScoped<TicketingAPI.Repositories.IUnitOfWork, TicketingAPI.Repositories.UnitOfWork>();

// Application Services
builder.Services.AddScoped<TicketingAPI.Application.Services.Interfaces.IEventService, TicketingAPI.Application.Services.Implementations.EventService>();
builder.Services.AddScoped<TicketingAPI.Application.Services.Interfaces.ISeatService, TicketingAPI.Application.Services.Implementations.SeatService>();
builder.Services.AddScoped<TicketingAPI.Application.Services.Interfaces.IReservationService, TicketingAPI.Application.Services.Implementations.ReservationService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    // .NET 10 genera el JSON en /openapi/v1.json
    app.MapOpenApi();
    // Swagger UI
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/openapi/v1.json", "Ticketing API v1");
        c.RoutePrefix = "swagger";
    });
}

app.UseCors("AllowFrontend");
app.UseAuthorization();
app.MapControllers();
// Ejecutar seed al arrancar
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    // Aplicar migraciones pendientes
    db.Database.Migrate();
    // Sembrar datos iniciales si es necesario
    DbSeeder.Seed(db);
}
app.Run();