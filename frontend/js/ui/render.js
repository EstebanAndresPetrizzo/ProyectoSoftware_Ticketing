// Renderiza catálogo y mapa de asientos basado en backend real.
import { formatDateTime } from "../mappers/seatMapMapper.js";

export function renderCatalog(
  container,
  events,
  onSelect,
  pagination,
  onPageChange,
  onPageSizeChange
) {
  container.innerHTML = `
    <!-- Header -->
    <div class="flex justify-between items-center mb-6">
      <div class="flex items-center gap-3">
        <div class="w-1 h-6 bg-blue-600"></div>
        <h2 class="text-2xl font-bold text-slate-800">Eventos destacados</h2>
      </div>
    </div>

    <!-- Cards Grid -->
    <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-3 mb-8 main-panel">
      ${events.map((e, i) => {
    const eventDate = new Date(e.date);
    const day = eventDate.getDate();
    const month = eventDate.toLocaleString('es-ES', { month: 'short' }).toUpperCase();
    const weekday = eventDate.toLocaleString('es-ES', { weekday: 'short' }).toUpperCase();

    const palette = [
      { block: 'bg-blue-500', text: 'text-blue-100', sub: 'text-blue-200', badge: 'bg-blue-50 text-blue-600 border-blue-100', title: 'group-hover:text-blue-600', arrow: 'group-hover:text-blue-500', border: 'group-hover:border-blue-300', shadow: 'hover:shadow-blue-200/70' },
      { block: 'bg-emerald-500', text: 'text-emerald-100', sub: 'text-emerald-200', badge: 'bg-emerald-50 text-emerald-600 border-emerald-100', title: 'group-hover:text-emerald-600', arrow: 'group-hover:text-emerald-500', border: 'group-hover:border-emerald-300', shadow: 'hover:shadow-emerald-200/70' },
      { block: 'bg-rose-500', text: 'text-rose-100', sub: 'text-rose-200', badge: 'bg-rose-50 text-rose-600 border-rose-100', title: 'group-hover:text-rose-600', arrow: 'group-hover:text-rose-500', border: 'group-hover:border-rose-300', shadow: 'hover:shadow-rose-200/70' },
      { block: 'bg-amber-400', text: 'text-amber-900', sub: 'text-amber-800', badge: 'bg-amber-50 text-amber-600 border-amber-100', title: 'group-hover:text-amber-600', arrow: 'group-hover:text-amber-500', border: 'group-hover:border-amber-300', shadow: 'hover:shadow-amber-200/70' },
    ];
    const c = palette[i % palette.length];

    return `
        <article class="bg-white border border-slate-200 rounded-lg overflow-hidden flex ${c.border} ${c.shadow} hover:shadow-lg hover:-translate-y-1 transition-all duration-300 group cursor-pointer" data-event-id="${e.id}">
          
          <!-- Date Block (Left Column) -->
          <div class="flex-shrink-0 w-16 ${c.block} flex flex-col items-center justify-center p-3 group-hover:scale-x-110 transition-transform duration-300 origin-left">
            <span class="text-[10px] font-bold ${c.sub} uppercase tracking-widest">${weekday}</span>
            <span class="text-2xl font-black ${c.text} leading-none">${day}</span>
            <span class="text-[10px] font-bold ${c.sub} uppercase">${month}</span>
          </div>

          <!-- Card Body (Right Column) -->
          <div class="p-4 flex flex-col flex-1 min-w-0">
            <!-- Category -->
            <span class="inline-block px-2 py-0.5 border text-[10px] font-bold rounded uppercase tracking-wider mb-2 self-start ${c.badge}">Evento</span>

            <!-- Title -->
            <h3 class="font-bold text-sm text-slate-800 mb-2 leading-snug line-clamp-2 flex-1 ${c.title} transition-colors duration-300">${e.name}</h3>

            <!-- Venue -->
            <div class="flex items-center gap-1 text-[11px] text-slate-400 font-medium mt-auto">
              <svg class="w-3 h-3 flex-shrink-0" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M17.657 16.657L13.414 20.9a1.998 1.998 0 01-2.827 0l-4.244-4.243a8 8 0 1111.314 0z"></path><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 11a3 3 0 11-6 0 3 3 0 016 0z"></path></svg>
              <span class="truncate">${e.venue}</span>
            </div>
          </div>

          <!-- Arrow Indicator (Right) -->
          <div class="flex-shrink-0 flex items-center pr-3 text-slate-300 ${c.arrow} transition-colors">
            <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5l7 7-7 7"></path></svg>
          </div>
        </article>
      `}).join("")}
    </div>
    <!-- Pagination -->
    <div class="flex flex-wrap justify-between items-center gap-6 pt-6 border-t border-slate-100">
      <!-- Selector de Tamaño -->
      <div class="flex items-center gap-3">
        <span class="text-[11px] font-bold text-slate-400 uppercase tracking-widest">Mostrar</span>
        <div class="relative">
          <select id="page-size-select" class="appearance-none bg-white border border-slate-200 text-slate-700 text-sm font-bold rounded-lg pl-4 pr-10 py-2 hover:border-slate-300 focus:outline-none focus:ring-4 focus:ring-blue-500/10 transition-all cursor-pointer shadow-sm">
            <option value="1" ${pagination.pageSize === 1 ? "selected" : ""}>1 Eventos</option>
            <option value="3" ${pagination.pageSize === 3 ? "selected" : ""}>3 Eventos</option>
            <option value="6" ${pagination.pageSize === 6 ? "selected" : ""}>6 Eventos</option>
            <option value="9" ${pagination.pageSize === 9 ? "selected" : ""}>9 Eventos</option>
            <option value="10" ${pagination.pageSize === 10 ? "selected" : ""}>10 Eventos</option>
            <option value="20" ${pagination.pageSize === 20 ? "selected" : ""}>20 Eventos</option>
          </select>
          <div class="absolute inset-y-0 right-0 flex items-center px-3 pointer-events-none text-slate-400">
            <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2.5" d="M19 9l-7 7-7-7"></path></svg>
          </div>
        </div>
      </div>

      <!-- Controles de Paginación -->
      <div class="flex items-center gap-2">
        <button id="btn-prev-page" 
          class="p-2 rounded-lg border border-slate-200 bg-white text-slate-600 hover:text-blue-600 hover:border-blue-200 hover:bg-blue-50 disabled:opacity-30 disabled:bg-slate-50 disabled:text-slate-400 disabled:border-slate-200 transition-all duration-200 shadow-sm hover:shadow" 
          ${!pagination.hasPrevious ? "disabled" : ""}
          aria-label="Página anterior">
          <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2.5" d="M15 19l-7-7 7-7"></path></svg>
        </button>
        
        <div class="flex items-center px-4 py-2 bg-slate-100/50 rounded-lg border border-slate-200 shadow-sm">
          <span class="text-[11px] font-bold text-slate-400 uppercase tracking-widest mr-3">Página</span>
          <div class="flex items-center gap-2">
            <span class="text-sm font-black text-blue-600">${pagination.page}</span>
            <span class="text-xs font-bold text-slate-300 uppercase">de</span>
            <span class="text-sm font-bold text-slate-500">${pagination.totalPages}</span>
          </div>
        </div>

        <button id="btn-next-page" 
          class="p-2 rounded-lg border border-slate-200 bg-white text-slate-600 hover:text-blue-600 hover:border-blue-200 hover:bg-blue-50 disabled:opacity-30 disabled:bg-slate-50 disabled:text-slate-400 disabled:border-slate-200 transition-all duration-200 shadow-sm hover:shadow" 
          ${!pagination.hasNext ? "disabled" : ""}
          aria-label="Siguiente página">
          <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2.5" d="M9 5l7 7-7 7"></path></svg>
        </button>
      </div>
    </div>
  `;

  container.querySelectorAll("[data-event-id]").forEach(card => {
    card.addEventListener("click", () =>
      onSelect(Number(card.dataset.eventId))
    );
  });

  container
    .querySelector("#btn-prev-page")
    ?.addEventListener("click", () =>
      onPageChange(pagination.page - 1)
    );

  container
    .querySelector("#btn-next-page")
    ?.addEventListener("click", () =>
      onPageChange(pagination.page + 1)
    );

  container
    .querySelector("#btn-page-size")
    ?.addEventListener("change", (e) =>
      onPageSizeChange(e.target.value)
    );

  container
    .querySelector("#page-size-select")
    ?.addEventListener("change", e =>
      onPageSizeChange(e.target.value)
    );
}

function normalizeSeatStatus(status) {
  if (typeof status === "number") {
    switch (status) {
      case 0: return "available";
      case 1: return "reserved";
      case 2: return "sold";
      case 3: return "purchased";
      default: return "available";
    }
  }

  const statusStr = String(status).toLowerCase();
  return statusStr === "purchased" ? "purchased" : statusStr;
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
    case "purchased": return "💳";
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
      const isMine = Boolean(seat.isMine);

      colsHtml.push(`
        <button
          class="${seatStatusClass(
        seat,
        isSelected,
        sector.position === "vip"
      )}"
          data-seat-id="${seat.id}"
          aria-label="${sector.name} fila ${seat.row} asiento ${seat.number}"
          ${status !== "available" && !isSelected && !isMine ? "disabled" : ""}
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
      <div class="${isVertical ? "flex justify-center gap-1" : "seat-grid"}">
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

  function getGridStyles(pos) {
    switch (pos) {
      case 'vip': return "grid-column: 2; grid-row: 2;";
      case 'front': return "grid-column: 2; grid-row: 3;";
      case 'left': return "grid-column: 1; grid-row: 4;";
      case 'center': return "grid-column: 2; grid-row: 4;";
      case 'right': return "grid-column: 3; grid-row: 4;";
      case 'back': return "grid-column: 2; grid-row: 5;";
      default: return "grid-column: 1 / 4;";
    }
  }

  container.innerHTML = `
    <div class="map-grid max-w-6xl mx-auto">
      <div class="stage" style="grid-column: 1 / 4; grid-row: 1;">
        🎤 ESCENARIO
      </div>

      ${Object.entries(sectorsByPosition).map(([pos, sectors]) =>
    sectors.map(sector => `
          <div style="${getGridStyles(pos)}">
            ${renderSector(sector, selected)}
          </div>
        `).join("")
  ).join("")}
    </div>
  `;
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