# Informe Formal de Pruebas de Concurrencia

## 1. Introducción
Este documento describe de forma formal las modificaciones realizadas en el proyecto `ProyectoSoftware_Ticketing`, la metodología de las pruebas ejecutadas y los resultados obtenidos. El objetivo principal fue verificar que el backend controla correctamente las reservas concurrentes sobre el mismo asiento.

## 2. Objetivos
- Validar que una sola reserva puede confirmarse cuando múltiples solicitudes concurrentes intentan reservar el mismo asiento.
- Confirmar que el backend rechaza las reservas adicionales para el mismo asiento en el mismo momento.
- Asegurar que la implementación existente del servicio de reservas funciona correctamente bajo concurrencia.

## 3. Alcance
Las pruebas cubrieron el flujo de reserva del backend, utilizando la lógica real de `ReservationService.CreateReservationAsync`.
No se realizaron pruebas de UI, ni pruebas de integración con PostgreSQL real en un contenedor Docker durante esta sesión.

## 4. Cambios realizados
Se modificaron principalmente tres componentes del repositorio:

### 4.1. `concurrency-test/concurrency-test.csproj`
- Se añadió la dependencia `Microsoft.EntityFrameworkCore.Sqlite` versión `10.0.6`.
- Se mantuvo la compatibilidad con EF Core 10.0 al alinear los paquetes instalados.

### 4.2. `backend/TicketingAPI/Data/AppDbContext.cs`
- Se ajustó la configuración del campo de concurrencia `Seat.Version` para que sea compatible con proveedores distintos a PostgreSQL.
- La propiedad sigue usando `IsRowVersion()` y solo aplica el tipo `xid` cuando el proveedor es `Npgsql`.

### 4.3. `concurrency-test/Program.cs`
- Se creó un test de concurrencia que simula 20 solicitudes simultáneas.
- Se emplea SQLite en memoria compartida (`Mode=Memory;Cache=Shared`) para que todos los consumidores compartan el mismo estado de base de datos.
- Se añadió un `TestAppDbContext` específico para la prueba, que evita la configuración de Postgres sobre la columna `xmin` y habilita un token de concurrencia usable en SQLite.
- Se sembraron datos de prueba: un evento, un sector, un asiento y un usuario.
- Todas las tareas concurrentes llaman al mismo servicio de reservas con el mismo `SeatId`.

## 5. Metodología de prueba
1. Se creó la base de datos en memoria y se estableció una conexión compartida para todos los hilos de prueba.
2. Se sembraron los datos iniciales necesarios:
   - `Venue`
   - `Sector`
   - `Event`
   - `Seat` (único asiento a reservar)
   - `User`
3. Se lanzaron 20 tareas concurrentes que invocaron `ReservationService.CreateReservationAsync` con la misma combinación `EventId`, `SectorId` y `SeatId`.
4. Se recolectaron los resultados de cada intento para contar cuántas reservas tuvieron éxito y cuántas fallaron.
5. Se interpretó la salida como indicador de control de concurrencia.

## 6. Resultados obtenidos
- Total de solicitudes concurrentes: `20`
- Reservas exitosas: `1`
- Fallos: `19`

El motivo del fallo reportado para las solicitudes rechazadas fue:
- `InvalidOperationException: El asiento ya no está disponible o tiene una reserva pendiente.`

## 7. Interpretación de los resultados
- La prueba demuestra que el backend permite únicamente una reserva válida para el mismo asiento cuando múltiples solicitudes llegan de forma simultánea.
- Esto sugiere que el servicio de reserva actualmente protege el recurso compartido (`Seat`) frente a condiciones de carrera.
- Las 19 solicitudes restantes fueron rechazadas de forma esperada, lo cual es correcto para evitar dobles reservas.

## 8. Conclusiones
- La implementación del backend de reservas presenta control de concurrencia efectivo en el caso probado.
- El test de concurrencia confirmó que el servicio responde consistentemente, evitando la duplicación de reservas.
- El uso de SQLite en memoria compartida permitió validar el comportamiento del servicio sin necesidad de un servidor PostgreSQL activo.

## 9. Recomendaciones
- Para una validación completa, se recomienda agregar una prueba equivalente contra PostgreSQL real, ya que la aplicación en producción usa `Npgsql`.
- También sería conveniente incluir pruebas que verifiquen la expiración automática de reservas y la restauración del asiento a `Available`.
- Mantener la lógica de transacciones y manejo de errores en `ReservationService` y `PaymentService` para asegurar consistencia ACID.

## 10. Archivos modificados
- `concurrency-test/concurrency-test.csproj`
- `backend/TicketingAPI/Data/AppDbContext.cs`
- `concurrency-test/Program.cs`

## 11. Observaciones finales
El documento refleja el análisis y los resultados de la prueba ejecutada en el entorno disponible. La evidencia es clara: la implementación impide múltiples reservas simultáneas del mismo asiento y deja un único resultado exitoso.
