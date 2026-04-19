// Renderiza catálogo y mapa de estadio en forma de cuadrado.

export function renderCatalog(container, events, onSelect) {
  container.innerHTML = `
    <h2 class="text-2xl font-bold mb-4">Eventos disponibles</h2>
    <div class="grid sm:grid-cols-2 gap-4">
      ${events.map(e => `
        <article class="border border-slate-200 rounded-lg p-4 hover:border-blue-400 hover:shadow-md transition cursor-pointer" data-event-id="${e.id}">
          <h3 class="font-semibold text-lg">${e.title}</h3>
          <p class="text-sm text-slate-500 mt-1">📅 ${e.date}</p>
          <p class="text-sm text-slate-500">📍 ${e.venue}</p>
          <button class="mt-3 text-sm text-blue-600 font-medium hover:underline">Elegir butacas →</button>
        </article>
      `).join("")}
    </div>
  `;
  container.querySelectorAll("[data-event-id]").forEach(card => {
    card.addEventListener("click", () => onSelect(card.dataset.eventId));
  });
}

function seatStatusClass(seat, isSelected, isVip) {
  const classes = ["seat"];
  if (isVip) classes.push("is-vip");
  if (isSelected) classes.push("is-selected");
  else classes.push("is-" + seat.status);
  return classes.join(" ");
}

function seatIcon(seat, isSelected) {
  if (isSelected) return "★";
  if (seat.status === "available") return "✓";
  if (seat.status === "reserved") return "⏱";
  if (seat.status === "sold") return "✕";
  return "";
}

// Renderiza un sector "frontal" (filas horizontales: norte, sur, vip)
function renderHorizontalSector(sectorId, sectorCfg, sectorState, selected, onClick) {
  const rows = [];
  for (let r = 0; r < sectorCfg.rows; r++) {
    const seats = [];
    for (let c = 0; c < sectorCfg.cols; c++) {
      const seatId = `${r}-${c}`;
      const seat = sectorState[seatId];
      const isSelected = selected.some(s => s.sectorId === sectorId && s.seatId === seatId);
      const isVip = sectorId === "vip";
      seats.push(`
        <button
          class="${seatStatusClass(seat, isSelected, isVip)}"
          data-sector="${sectorId}" data-seat="${seatId}"
          aria-label="Sector ${sectorCfg.name}, fila ${r + 1}, butaca ${c + 1}, ${isSelected ? 'seleccionada' : seat.status}"
          ${seat.status !== "available" && !isSelected ? "disabled" : ""}
        >${seatIcon(seat, isSelected)}</button>
      `);
    }
    rows.push(`<div class="seat-row">${seats.join("")}</div>`);
  }
  return `
    <div class="text-center">
      <div class="sector-label mb-1">${sectorCfg.name} · ${sectorCfg.price}$</div>
      <div class="seat-grid">${rows.join("")}</div>
    </div>
  `;
}

// Renderiza un sector lateral (filas verticales: oeste, este)
function renderVerticalSector(sectorId, sectorCfg, sectorState, selected) {
  // En laterales: cada "row" es una columna vertical de butacas.
  const cols = [];
  for (let r = 0; r < sectorCfg.rows; r++) {
    const seats = [];
    for (let c = 0; c < sectorCfg.cols; c++) {
      const seatId = `${r}-${c}`;
      const seat = sectorState[seatId];
      const isSelected = selected.some(s => s.sectorId === sectorId && s.seatId === seatId);
      seats.push(`
        <button
          class="${seatStatusClass(seat, isSelected, false)}"
          data-sector="${sectorId}" data-seat="${seatId}"
          aria-label="Sector ${sectorCfg.name}, fila ${r + 1}, butaca ${c + 1}, ${isSelected ? 'seleccionada' : seat.status}"
          ${seat.status !== "available" && !isSelected ? "disabled" : ""}
        >${seatIcon(seat, isSelected)}</button>
      `);
    }
    cols.push(`<div class="flex gap-1">${seats.join("")}</div>`);
  }
  return `
    <div class="text-center">
      <div class="sector-label mb-1" style="writing-mode: vertical-rl; transform: rotate(180deg);">${sectorCfg.name} · ${sectorCfg.price}$</div>
      <div class="flex flex-col gap-1">${cols.join("")}</div>
    </div>
  `;
}

export function renderStadium(container, stadium, eventState, selected, onSeatClick) {
  // Layout en grid 3x5:
  //  fila 1: ESCENARIO (col 1-3)
  //  fila 2: VIP (col 1-3)
  //  fila 3: valla (col 1-3)
  //  fila 4: OESTE (col 1) | NORTE (col 2) | ESTE (col 3)
  //  fila 5: SUR (col 1-3)
  const html = `
    <div class="map-grid max-w-4xl mx-auto">
      <div class="stage" style="grid-column: 1 / 4; width: 70%;">🎤 ESCENARIO</div>

      <div style="grid-column: 1 / 4;">
        ${renderHorizontalSector("vip", stadium.vip, eventState.sectors.vip, selected, onSeatClick)}
      </div>

      <div class="fence" style="grid-column: 1 / 4; width: 80%;"></div>

      <div style="grid-column: 1; grid-row: 4;" class="self-start">
        ${renderVerticalSector("oeste", stadium.oeste, eventState.sectors.oeste, selected)}
      </div>
      <div style="grid-column: 2; grid-row: 4;">
        ${renderHorizontalSector("norte", stadium.norte, eventState.sectors.norte, selected, onSeatClick)}
      </div>
      <div style="grid-column: 3; grid-row: 4;" class="self-start">
        ${renderVerticalSector("este", stadium.este, eventState.sectors.este, selected)}
      </div>

      <div style="grid-column: 1 / 4;">
        ${renderHorizontalSector("sur", stadium.sur, eventState.sectors.sur, selected, onSeatClick)}
      </div>
    </div>
  `;
  container.innerHTML = html;
  container.querySelectorAll(".seat[data-seat]").forEach(btn => {
    btn.addEventListener("click", () => {
      onSeatClick(btn.dataset.sector, btn.dataset.seat);
    });
  });
}

export function renderSelection(listEl, totalEl, buyBtn, selected, stadium) {
  if (selected.length === 0) {
    listEl.innerHTML = `<li class="text-slate-400">Aún no has elegido butacas.</li>`;
    totalEl.textContent = "0,00 $";
    buyBtn.disabled = true;
    return;
  }
  let total = 0;
  listEl.innerHTML = selected.map(s => {
    const cfg = stadium[s.sectorId];
    const [r, c] = s.seatId.split("-").map(Number);
    total += cfg.price;
    return `
      <li class="flex justify-between items-center py-1 border-b border-slate-100 last:border-0">
        <span>${cfg.name} · F${r + 1}·B${c + 1}</span>
        <span class="text-slate-500">${cfg.price}$</span>
      </li>
    `;
  }).join("");
  totalEl.textContent = total.toFixed(2).replace(".", ",") + " $";
  buyBtn.disabled = false;
}
