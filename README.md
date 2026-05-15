# 🎫 ProyectoSoftware_Ticketing

Sistema de venta de entradas y selección de butacas en tiempo real.

## 📋 Tabla de Contenidos

- [Descripción](#descripción)
- [Tecnologías](#tecnologías)
- [Requisitos Previos](#requisitos-previos)
- [Levantar el Proyecto](#levantar-el-proyecto)
- [Acceder a la Aplicación](#acceder-a-la-aplicación)
- [API Endpoints](#api-endpoints)
- [Credenciales](#credenciales)
- [Estructura del Proyecto](#estructura-del-proyecto)
- [Documentación y pruebas incluidas](#documentación-y-pruebas-incluidas)
- [Ejecutar pruebas de concurrencia manualmente](#ejecutar-pruebas-de-concurrencia-manualmente)
- [Detener el Proyecto](#detener-el-proyecto)
- [Notas](#notas)

---

## 📖 Descripción

Aplicación full-stack para la gestión de eventos y reserva de entradas. Permite:
- Ver catálogo de eventos disponibles.
- Seleccionar butacas en un mapa interactivo.
- Reservar asientos en tiempo real.
- Gestionar estados de butacas (disponible, reservado, vendido).

---

## 🛠 Tecnologías

| Componente | Tecnología |
|------------|-------------|
| Frontend | HTML5, JavaScript |
| Backend | ASP.NET Core 10 (.NET) |
| Base de Datos | PostgreSQL 16 |
| Contenedores | Docker + Docker Compose |
| Web Server | Nginx (Frontend) |

---

## ✅ Requisitos Previos

- [Docker Desktop](https://www.docker.com/products/docker-desktop) instalado y ejecutándose.
- [.NET 10 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/10.0) instalado.
- [Git](https://git-scm.com/) opcional para clonar el repositorio.

---

## 🚀 Levantar el Proyecto

### 1. Abrir terminal en la raíz del proyecto

```bash
cd "d:\Desktop\Proyecto de Software\ProyectoSoftware_Ticketing"
```

### 2. Iniciar contenedores

```bash
docker-compose up -d --build
```

Este comando:
- 🔧 Compila el backend.
- 🐘 Crea y levanta el contenedor de PostgreSQL.
- 🌐 Crea y levanta el contenedor del backend.
- 🎨 Crea y levanta el contenedor del frontend.

### 3. Verificar contenedores

```bash
docker-compose ps
```

Deberías ver los contenedores en estado `Up`.

---

## 🌐 Acceder a la Aplicación

### Frontend

Abrir en el navegador:

```
http://localhost:3000
```

### Backend API

```
http://localhost:5000
```

---

## 🔌 API Endpoints

### Obtener eventos

```http
GET http://localhost:5000/api/v1/events?page=1&pageSize=10
```

### Obtener mapa de butacas de un evento

```http
GET http://localhost:5000/api/v1/events/{id}/seats
```

### Crear una reserva

```http
POST http://localhost:5000/api/v1/reservations
```

**Cuerpo de la solicitud:**

```json
{
  "eventId": 1,
  "seatIds": [1, 2, 3],
  "userId": "user-123"
}
```

---

## 🔑 Credenciales

| Servicio | Variable | Valor |
|----------|----------|-------|
| PostgreSQL | Database | `ticketing` |
| PostgreSQL | User | `ticketing_user` |
| PostgreSQL | Password | `ticketing_pass` |
| PostgreSQL | Port | `5432` |
| Backend | Port | `5000` |
| Frontend | Port | `3000` |

---

## 📁 Estructura del Proyecto

```
ProyectoSoftware_Ticketing/
├── docker-compose.yml                  # Orquestación de contenedores
├── README.md                           # Este archivo
├── REPORTE_PRUEBAS_CONCURRENCIA.md     # Informe formal de las pruebas de concurrencia
├── concurrency-test/                   # Proyecto de pruebas de concurrencia
│   ├── Program.cs                      # Test de 20 reservas concurrentes
│   └── concurrency-test.csproj         # Proyecto .NET para pruebas
├── backend/
│   └── TicketingAPI/
│       ├── Controllers/                # Controladores API
│       ├── Application/                # Lógica de negocio (Servicios)
│       │   ├── Services/               # Implementaciones de servicio
│       │   └── Interfaces/             # Contratos de servicio
│       ├── Data/                       # Contexto de base de datos
│       ├── DTOs/                       # Objetos de transferencia de datos
│       ├── Models/                     # Modelos de entidad
│       ├── Repositories/               # Patrón Repository
│       ├── Migrations/                 # Migraciones EF Core
│       ├── Properties/                 # Configuración de lanzamiento
│       ├── Dockerfile                  # Imagen del backend
│       ├── appsettings.json            # Configuración
│       └── TicketingAPI.csproj         # Proyecto .NET
└── frontend/
    ├── index.html                     # Página principal
    ├── app.html                       # Página alternativa / demo
    ├── css/styles.css                 # Estilos personalizados
    ├── js/                            # Lógica JavaScript
    │   ├── api/                       # Cliente API y mocks
    │   ├── mappers/                   # Transformaciones de datos
    │   └── ui/                        # Componentes de interfaz
    ├── Dockerfile                     # Imagen del frontend (Nginx)
    └── Dockerfile
```

---

## 📚 Documentación y pruebas incluidas

Este repositorio contiene documentación adicional y un proyecto de pruebas dedicado.

- `REPORTE_PRUEBAS_CONCURRENCIA.md`: informe formal con los cambios realizados, la metodología de prueba y los resultados obtenidos.
- `concurrency-test/`: proyecto independiente para ejecutar pruebas de concurrencia del backend.

### Qué valida el proyecto `concurrency-test`
- Simula `20` solicitudes concurrentes que intentan reservar el mismo `SeatId`.
- Usa SQLite en memoria compartida para reproducir el comportamiento de concurrencia.
- Ejecuta la lógica real de `ReservationService.CreateReservationAsync`.

---

## 🧪 Ejecutar pruebas de concurrencia manualmente

Estas pruebas validan que el servicio de reservas solo permite una reserva activa por asiento cuando varias solicitudes llegan simultáneamente.

### Requisitos adicionales
- [.NET 10 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/10.0) instalado.
- No es obligatorio levantar Docker para esta prueba; se ejecuta sobre SQLite en memoria.

### Pasos
1. Abrir una terminal en la carpeta raíz del proyecto:

```bash
cd "d:\Desktop\Proyecto de Software\ProyectoSoftware_Ticketing"
```

2. Restaurar paquetes y ejecutar el proyecto de prueba:

```bash
dotnet restore concurrency-test
cd concurrency-test
dotnet run
```

3. Interpretar el resultado esperado:

- `Total requests: 20`
- `Successes: 1`
- `Failures: 19`

Si el test termina con:

```text
Test passed: only one reservation succeeded.
```

entonces el backend está controlando correctamente la concurrencia sobre el mismo asiento.

> Nota: la prueba se ejecuta en el proyecto `concurrency-test` y utiliza el archivo `concurrency-test/Program.cs`.

---

## 🧹 Detener el Proyecto

Para detener todos los contenedores:

```bash
docker-compose down
```

Para eliminar también los datos de la base de datos (volúmenes):

```bash
docker-compose down -v
```

---

## ⚠️ Notas

- La base de datos se siembra automáticamente con datos de ejemplo al iniciar (`DbSeeder.cs`).
- El frontend se comunica con el backend mediante fetch API.
- Las reservas tienen un tiempo de expiración de 5 minutos.
