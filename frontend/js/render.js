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