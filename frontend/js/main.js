import { getEvents, getSeats, reserveSeat } from "./api.js";
import { renderEvents, renderSeats } from "./render.js";

async function init() {
  const events = await getEvents();
  renderEvents(events);
}

init();

// detectar click en botones de eventos
document.getElementById("events-container").addEventListener("click", async (e) => {
  if (e.target.tagName === "BUTTON") {
    const eventId = e.target.dataset.id;

    // traer asientos
    const seats = await getSeats(eventId);

    // renderizar
    renderSeats(seats);

    // cambiar vista
    showSeatsView();
  }
});

//funcionalidad del boton volver
document.getElementById("back-button").addEventListener("click", () => {
  showEventsView();
});

// para ver asientos y volver a eventos
function showSeatsView() {
  document.getElementById("events-view").classList.add("hidden");
  document.getElementById("seats-view").classList.remove("hidden");
}

function showEventsView() {
  document.getElementById("events-view").classList.remove("hidden");
  document.getElementById("seats-view").classList.add("hidden");
}

// detectar click en asientos
document.getElementById("seats-container").addEventListener("click", async (e) => {
  if (e.target.classList.contains("seat")) {

    const seatId = e.target.dataset.id;
    const status = e.target.dataset.status;

    // validar estado y evitar acción si no está disponible
    if (status == "SOLD") {
      alert("Asiento no disponible");
      return;
    }
    if (status == "RESERVED") {
      alert("Otro usuario tomó el asiento");
      return;
    }

    // feedback visual de reserva en proceso
    e.target.classList.add("opacity-50");

    const success = await reserveSeat(seatId);

    // limpiar feedback
    e.target.classList.remove("opacity-50");

    if (success) {
      //reservado exitosamente
      e.target.className = "text-center text-sm p-2 rounded bg-yellow-400";
      e.target.dataset.status = "RESERVED";
    } else {
      // reserva fallida por concurrencia
      alert("Otro usuario ya tomó el asiento");
    }
  }
});