import { getEvents, getSeats } from "./api.js";
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