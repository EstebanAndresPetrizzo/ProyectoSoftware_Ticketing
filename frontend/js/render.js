export function renderEvents(events) {
  const container = document.getElementById("events-container");
  container.innerHTML = "";

  events.forEach(event => {
    const card = document.createElement("div");

    card.className = "bg-white p-4 rounded shadow";

    card.innerHTML = `
      <h3 class="font-bold">${event.name}</h3>
      <p class="text-sm text-gray-500">${event.date}</p>
      <button 
        class="mt-2 bg-blue-500 text-white px-3 py-1 rounded"
        data-id="${event.id}"
      >
        Ver asientos
      </button>
    `;

    container.appendChild(card);
  });
}

export function renderSeats(seats) {
  const container = document.getElementById("seats-container");
  container.innerHTML = "";

  seats.forEach(seat => {
    const div = document.createElement("div");

    div.className = `
      text-center text-sm p-2 rounded cursor-pointer
      ${getSeatColor(seat.status)}
    `;

    div.innerText = seat.number;

    container.appendChild(div);
  });
}

function getSeatColor(status) {
  switch (status) {
    case "AVAILABLE":
      return "bg-green-400";
    case "RESERVED":
      return "bg-yellow-400";
    case "SOLD":
      return "bg-red-400";
    default:
      return "bg-gray-300";
  }
}