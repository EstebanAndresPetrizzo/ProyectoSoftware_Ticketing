using Microsoft.EntityFrameworkCore;
using TicketingAPI.Data;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
// Generador nativo de .NET 10
builder.Services.AddOpenApi();
// Conexión a PostgreSQL con EF Core
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
);
// CORS
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

app.Run();