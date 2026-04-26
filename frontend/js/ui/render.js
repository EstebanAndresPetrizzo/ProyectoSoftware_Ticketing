// Renderiza catálogo y mapa de asientos basado en backend real.

export function renderCatalog(container, events, onSelect) {
  container.innerHTML = `
    <h2 class="text-2xl font-bold mb-4">Eventos disponibles</h2>
    <div class="grid sm:grid-cols-2 gap-4">
      ${events.map(event => `
        <article
          class="border border-slate-200 rounded-lg p-4 hover:border-blue-400 hover:shadow-md transition cursor-pointer"
          data-event-id="${event.id}"
        >
          <h3 class="font-semibold text-lg">${event.name}</h3>
          <p class="text-sm text-slate-500 mt-1">📅 ${event.date}</p>
          <p class="text-sm text-slate-500">📍 ${event.venue}</p>
          <button class="mt-3 text-sm text-blue-600 font-medium hover:underline">
            Elegir butacas →
          </button>
        </article>
      `).join("")}
    </div>
  `;

  container.querySelectorAll("[data-event-id]").forEach(card => {
    card.addEventListener("click", () => {
      onSelect(Number(card.dataset.eventId));
    });
  });
}

function normalizeSeatStatus(status) {
  if (typeof status === "number") {
    switch (status) {
      case 0: return "available";
      case 1: return "reserved";
      case 2: return "sold";
      default: return "available";
    }
  }

  return String(status).toLowerCase();
}

function seatStatusClass(seat, isSelected, isVip) {
  const classes = ["seat"];

  if (isVip) classes.push("is-vip");

  if (isSelected) {
    classes.push("is-selected");
  } else {
    classes.push(`is-${normalizeSeatStatus(seat.status)}`);
  }

  return classes.join(" ");
}

function seatIcon(seat, isSelected) {
  if (isSelected) return "★";

  switch (normalizeSeatStatus(seat.status)) {
    case "available": return "✓";
    case "reserved": return "⏱";
    case "sold": return "✕";
    default: return "";
  }
}

function renderSector(sector, selected) {
  const isVertical =
    sector.position === "left" ||
    sector.position === "right";

  const rowsHtml = [];

  for (let r = 0; r < sector.rows; r++) {
    const rowLetter = String.fromCharCode(65 + r);
    const colsHtml = [];

    for (let c = 1; c <= sector.cols; c++) {
      const seat = sector.seats.find(
        s => s.row === rowLetter && s.number === c
      );

      if (!seat) continue;

      const isSelected = selected.some(
        s => s.seatId === seat.id
      );

      const status = normalizeSeatStatus(seat.status);

      colsHtml.push(`
        <button
          class="${seatStatusClass(
            seat,
            isSelected,
            sector.position === "vip"
          )}"
          data-seat-id="${seat.id}"
          aria-label="${sector.name} fila ${seat.row} asiento ${seat.number}"
          ${status !== "available" && !isSelected ? "disabled" : ""}
        >
          ${seatIcon(seat, isSelected)}
        </button>
      `);
    }

    rowsHtml.push(`
      <div class="${isVertical ? "flex flex-col gap-1" : "seat-row"}">
        ${colsHtml.join("")}
      </div>
    `);
  }

  return `
    <div class="text-center">
      <div class="sector-label mb-2">
        ${sector.name} · $${sector.price}
      </div>
      <div class="${isVertical ? "flex gap-1" : "seat-grid"}">
        ${rowsHtml.join("")}
      </div>
    </div>
  `;
}

export function renderStadium(container, eventState, selected, onSeatClick) {
  const sectorsByPosition = {
    vip: [],
    front: [],
    left: [],
    center: [],
    right: [],
    back: []
  };

  for (const sector of eventState.sectors) {
    if (!sectorsByPosition[sector.position]) {
      sectorsByPosition[sector.position] = [];
    }

    sectorsByPosition[sector.position].push(sector);
  }

  container.innerHTML = `
    <div class="map-grid max-w-6xl mx-auto">
      <div class="stage" style="grid-column: 1 / 4;">
        🎤 ESCENARIO
      </div>

      ${sectorsByPosition.vip.map(sector => `
        <div style="grid-column: 1 / 4;">
          ${renderSector(sector, selected)}
        </div>
      `).join("")}

      ${sectorsByPosition.front.map(sector => `
        <div style="grid-column: 1 / 4;">
          ${renderSector(sector, selected)}
        </div>
      `).join("")}

      <div style="grid-column: 1;">
        ${sectorsByPosition.left
          .map(sector => renderSector(sector, selected))
          .join("")}
      </div>

      <div style="grid-column: 2;">
        ${sectorsByPosition.center
          .map(sector => renderSector(sector, selected))
          .join("")}
      </div>

      <div style="grid-column: 3;">
        ${sectorsByPosition.right
          .map(sector => renderSector(sector, selected))
          .join("")}
      </div>

      ${sectorsByPosition.back.map(sector => `
        <div style="grid-column: 1 / 4;">
          ${renderSector(sector, selected)}
        </div>
      `).join("")}
    </div>
  `;

  container.querySelectorAll("[data-seat-id]").forEach(btn => {
    btn.addEventListener("click", () => {
      onSeatClick(Number(btn.dataset.seatId));
    });
  });
}

export function renderSelection(listEl, totalEl, buyBtn, selected, seatMap) {
  if (selected.length === 0) {
    listEl.innerHTML =
      `<li class="text-slate-400">Aún no has elegido butacas.</li>`;

    totalEl.textContent = "0,00 $";
    buyBtn.disabled = true;
    return;
  }

  let total = 0;

  listEl.innerHTML = selected.map(sel => {
    let foundSector = null;
    let foundSeat = null;

    for (const sector of seatMap.sectors) {
      const seat = sector.seats.find(
        s => s.id === sel.seatId
      );

      if (seat) {
        foundSector = sector;
        foundSeat = seat;
        break;
      }
    }

    if (!foundSector || !foundSeat) return "";

    total += foundSector.price;

    return `
      <li class="flex justify-between items-center py-1 border-b border-slate-100 last:border-0">
        <span>${foundSector.name} · F${foundSeat.row} · B${foundSeat.number}</span>
        <span class="text-slate-500">$${foundSector.price}</span>
      </li>
    `;
  }).join("");

  totalEl.textContent =
    total.toFixed(2).replace(".", ",") + " $";

  buyBtn.disabled = false;
}