import { getEvents, getSeats, reserveSeat } from "./api.js";
import { renderEvents, renderSeats } from "./render.js";

let currentEventId = null;

// INIT
async function init() {
  const events = await getEvents();
  renderEvents(events);
}

init();


// ==============================
// EVENTOS → VER ASIENTOS
// ==============================
document.getElementById("events-container").addEventListener("click", async (e) => {
  if (e.target.tagName === "BUTTON") {
    currentEventId = e.target.dataset.id;

    const seats = await getSeats(currentEventId);
    renderSeats(seats);

    showSeatsView();
  }
});


// ==============================
// BOTÓN VOLVER
// ==============================
document.getElementById("back-button").addEventListener("click", () => {
  showEventsView();
});


// ==============================
// NAVEGACIÓN
// ==============================
function showSeatsView() {
  document.getElementById("events-view").classList.add("hidden");
  document.getElementById("seats-view").classList.remove("hidden");
}

function showEventsView() {
  document.getElementById("events-view").classList.remove("hidden");
  document.getElementById("seats-view").classList.add("hidden");
}


// ==============================
// CLICK EN ASIENTOS
// ==============================
document.getElementById("seats-container").addEventListener("click", async (e) => {
  const seatElement = e.target.closest(".seat");

  if (!seatElement) return;

  const seatId = seatElement.dataset.id;
  const status = seatElement.dataset.status;

  console.log("STATUS REAL:", status);

  // No disponible
  if (status !== "AVAILABLE") {
    if (status === "RESERVED") {
      showSeatMessage(seatElement, "Reservado por otro usuario");
    } else if (status === "SOLD") {
      showSeatMessage(seatElement, "Vendido");
    }
    return;
  }

  // feedback visual
  seatElement.classList.add("opacity-50");

  const success = await reserveSeat(seatId);

  seatElement.classList.remove("opacity-50");

  if (success) {
    // actualizar color sin romper clases
    updateSeatColor(seatElement, "RESERVED");
    seatElement.dataset.status = "RESERVED";

    showSeatMessage(seatElement, "Reservado");
  } else {
    // conflicto
    showSeatMessage(seatElement, "Otro usuario lo tomó");

    //refrescar estado desde backend/mock random
    const seats = await getSeats(currentEventId);
    renderSeats(seats);
  }
});


// ==============================
// UTIL: CAMBIO DE COLOR
// ==============================
function updateSeatColor(element, status) {
  element.classList.remove("bg-green-400", "bg-yellow-400", "bg-red-400");

  if (status === "AVAILABLE") {
    element.classList.add("bg-green-400");
  } else if (status === "RESERVED") {
    element.classList.add("bg-yellow-400");
  } else if (status === "SOLD") {
    element.classList.add("bg-red-400");
  }
}


// ==============================
// UTIL: MENSAJE SOBRE ASIENTO
// ==============================
function showSeatMessage(element, message) {
  const msg = document.createElement("div");

  msg.innerText = message;
  msg.className = "absolute bg-black text-white text-xs px-2 py-1 rounded";

  element.style.position = "relative";

  msg.style.top = "-20px";
  msg.style.left = "50%";
  msg.style.transform = "translateX(-50%)";

  element.appendChild(msg);

  setTimeout(() => {
    msg.remove();
  }, 1000);
}