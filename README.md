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

---

## 📖 Descripción

Aplicación full-stack para la gestión de eventos y reserva de entradas. Permite:
- Ver catálogo de eventos disponibles
- Seleccionar butacas en un mapa interactivo
- Reservar asientos en tiempo real
- Gestión de estados de butacas (disponible, reservado, vendido)

---

## 🛠 Tecnologías

| Componente | Tecnología |
|------------|-------------|
| Frontend   | HTML5, JavaScript, Tailwind CSS |
| Backend     | ASP.NET Core 10 (.NET) |
| Base de Datos | PostgreSQL 16 |
| Contenedores | Docker + Docker Compose |
| Web Server | Nginx (Frontend) |

---

## ✅ Requisitos Previos

- [Docker Desktop](https://www.docker.com/products/docker-desktop) instalado y ejecutándose
- [Git](https://git-scm.com/) (opcional, para clonar el repositorio)

---

## 🚀 Levantar el Proyecto

### 1. Clonar o extraer el proyecto
Si tienes el código fuente, asegúrate de estar en la raíz del proyecto:

```bash
cd "c:\Users\Esteban Andres\Desktop\Proyecto de Software\ProyectoSoftware_Ticketing"
```

### 2. Iniciar los contenedores

Desde la raíz del proyecto (donde está el archivo `docker-compose.yml`):

```bash
docker-compose up -d --build
```

Este comando:
- 🔧 Compila el backend (.NET)
- 🐘 Crea y levanta el contenedor de PostgreSQL
- 🌐 Crea y levanta el contenedor del backend (puerto 5000)
- 🎨 Crea y levanta el contenedor del frontend con Nginx (puerto 3000)

### 3. Verificar que todo esté corriendo

```bash
docker-compose ps
```

Deberías ver 3 contenedores en estado `Up`:
- `ticketing-postgres`
- `ticketing-backend`
- `ticketing-frontend`

---

## 🌐 Acceder a la Aplicación

### Frontend (Interfaz de Usuario)

Abre tu navegador y visita:

```
http://localhost:3000
```

Verás la página principal con el catálogo de eventos disponibles.

#### Cómo usar la app:

1. **Ver eventos**: En la página principal verás un listado de eventos con título, fecha y precio.

2. **Seleccionar evento**: Haz clic en un evento para ver el mapa de asientos.

3. **Elegir butacas**: 
   - Las butacas **verdes** están disponibles
   - Las butacas **naranjas/ámbar** están reservadas (temporalmente bloqueadas por otro usuario)
   - Las butacas **grises** no están disponibles

4. **Confirmar reserva**: 
   - Selecciona las butacas haciendo clic en ellas
   - El panel lateral muestra tu selección y el total
   - Haz clic en "Comprar" para confirmar la reserva

### Backend (API)

El API REST está disponible en:

```
http://localhost:5000
```

---

## 🔌 API Endpoints

### Obtener eventos (catálogo)

```http
GET http://localhost:5000/api/v1/events?page=1&pageSize=10
```

**Respuesta:**
```json
{
  "success": true,
  "data": [
    {
      "id": 1,
      "title": "Concierto Rock 2026",
      "description": "...",
      "eventDate": "2026-05-15T20:00:00",
      "venueName": "Estadio Central",
      "basePrice": 50.00
    }
  ],
  "pagination": {
    "page": 1,
    "pageSize": 10,
    "totalItems": 5,
    "totalPages": 1,
    "hasNext": false,
    "hasPrevious": false
  }
}
```

### Obtener mapa de butacas de un evento

```http
GET http://localhost:5000/api/v1/events/{id}/seats
```

Ejemplo:
```http
GET http://localhost:5000/api/v1/events/1/seats
```

**Respuesta:**
```json
{
  "success": true,
  "data": {
    "eventId": 1,
    "eventTitle": "Concierto Rock 2026",
    "sectors": [
      {
        "name": "VIP",
        "price": 100.00,
        "rows": [
          {
            "row": "A",
            "seats": [
              { "id": 1, "number": 1, "status": "Available", "price": 100.00 }
            ]
          }
        ]
      }
    ]
  }
}
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

**Respuesta exitosa (201 Created):**
```json
{
  "success": true,
  "data": {
    "id": 1,
    "eventId": 1,
    "userId": "user-123",
    "seats": [
      { "id": 1, "number": 1, "row": "A" }
    ],
    "totalAmount": 300.00,
    "reservationDate": "2026-04-27T10:30:00Z",
    "status": "Confirmed"
  }
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
├── docker-compose.yml          # Orquestación de contenedores
├── README.md                   # Este archivo
├── backend/
│   └── TicketingAPI/
│       ├── Controllers/        # Controladores API
│       ├── Application/       # Lógica de negocio (Servicios)
│       ├── Data/              # Contexto de base de datos
│       ├── DTOs/              # Objetos de transferencia de datos
│       ├── Models/            # Modelos de entidad
│       ├── Repositories/     # Patrón Repository
│       ├── Dockerfile        # Imagen del backend
│       └── appsettings.json   # Configuración
└── frontend/
    ├── index.html             # Página principal
    ├── css/styles.css         # Estilos personalizados
    ├── js/                    # Lógica JavaScript
    ├── Dockerfile             # Imagen del frontend (Nginx)
    └── Dockerfile
```

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

- La base de datos se sembrar automáticamente con datos de ejemplo al iniciar (`DbSeeder.cs`).
- El frontend se comunica con el backend mediante fetch API.
- Las reservas tienen un tiempo de expiración de 5 minutos.