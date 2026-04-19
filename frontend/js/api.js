// MOCK API con localStorage. Simula backend con TTL de reservas y concurrencia.
const TTL_MS = 5 * 60 * 1000; // 5 minutos
const STORAGE_KEY = "ticketing_state_v2";

// Configuración del estadio: cuadrado alrededor del escenario.
// Layout (desde el escenario hacia atrás):
//   [ESCENARIO]
//   [VIP]              ← frente al escenario
//   --- valla ---
//   [NORTE]            ← detrás de la valla
//   con [OESTE] a la izquierda y [ESTE] a la derecha (laterales)
//   [SUR]              ← al fondo
const STADIUM = {
  vip:   { name: "VIP",   rows: 2, cols: 14, price: 120, position: "front" },
  norte: { name: "Norte", rows: 5, cols: 14, price: 60,  position: "center" },
  sur:   { name: "Sur",   rows: 5, cols: 14, price: 45,  position: "back" },
  oeste: { name: "Oeste", rows: 8, cols: 3,  price: 50,  position: "left" },
  este:  { name: "Este",  rows: 8, cols: 3,  price: 50,  position: "right" },
};

const EVENTS = [
  { id: "evt-1", title: "Concierto Aurora Boreal", date: "2026-05-12 21:00", venue: "Estadio Central" },
  { id: "evt-2", title: "Final Liga de Robótica",  date: "2026-06-03 18:30", venue: "Estadio Central" },
];

function now() { return Date.now(); }

function loadState() {
  const raw = localStorage.getItem(STORAGE_KEY);
  if (raw) return JSON.parse(raw);
  const state = { events: {} };
  for (const evt of EVENTS) {
    state.events[evt.id] = { sectors: {} };
    for (const [sectorId, cfg] of Object.entries(STADIUM)) {
      state.events[evt.id].sectors[sectorId] = {};
      for (let r = 0; r < cfg.rows; r++) {
        for (let c = 0; c < cfg.cols; c++) {
          state.events[evt.id].sectors[sectorId][`${r}-${c}`] = { status: "available", reservedUntil: null, owner: null };
        }
      }
    }
  }
  // Algunos asientos pre-vendidos para realismo
  const seed = state.events["evt-1"].sectors;
  ["0-3","0-4","1-7","2-2"].forEach(k => seed.norte[k] && (seed.norte[k].status = "sold"));
  ["0-5","0-6"].forEach(k => seed.vip[k] && (seed.vip[k].status = "sold"));
  localStorage.setItem(STORAGE_KEY, JSON.stringify(state));
  return state;
}

function saveState(state) { localStorage.setItem(STORAGE_KEY, JSON.stringify(state)); }

function expireReservations(state) {
  const t = now();
  for (const evt of Object.values(state.events)) {
    for (const sector of Object.values(evt.sectors)) {
      for (const seat of Object.values(sector)) {
        if (seat.status === "reserved" && seat.reservedUntil && seat.reservedUntil < t) {
          seat.status = "available"; seat.reservedUntil = null; seat.owner = null;
        }
      }
    }
  }
}

export const api = {
  async listEvents() { return EVENTS; },

  async getStadium() { return STADIUM; },

  async getSeats(eventId) {
    const state = loadState();
    expireReservations(state);
    saveState(state);
    return state.events[eventId];
  },

  // Simula concurrencia: 10% de probabilidad de que "otro usuario" se nos adelante.
  async reserveSeat(eventId, sectorId, seatId, owner = "me") {
    if (Math.random() < 0.10) {
      const state = loadState();
      const seat = state.events[eventId].sectors[sectorId][seatId];
      if (seat.status === "available") {
        seat.status = "reserved"; seat.reservedUntil = now() + TTL_MS; seat.owner = "otro-usuario";
        saveState(state);
      }
      throw new Error("Otro usuario reservó esta butaca antes que usted");
    }
    const state = loadState();
    expireReservations(state);
    const seat = state.events[eventId].sectors[sectorId][seatId];
    if (!seat) throw new Error("Butaca no existe");
    if (seat.status !== "available") throw new Error("Butaca no disponible");
    seat.status = "reserved"; seat.reservedUntil = now() + TTL_MS; seat.owner = owner;
    saveState(state);
    return { reservedUntil: seat.reservedUntil };
  },

  async releaseSeat(eventId, sectorId, seatId, owner = "me") {
    const state = loadState();
    const seat = state.events[eventId].sectors[sectorId][seatId];
    if (seat && seat.owner === owner && seat.status === "reserved") {
      seat.status = "available"; seat.reservedUntil = null; seat.owner = null;
      saveState(state);
    }
  },

  async confirmPurchase(eventId, seats, owner = "me") {
    const state = loadState();
    expireReservations(state);
    for (const { sectorId, seatId } of seats) {
      const seat = state.events[eventId].sectors[sectorId][seatId];
      if (!seat || seat.owner !== owner || seat.status !== "reserved") {
        throw new Error(`La butaca ${sectorId}/${seatId} ya no es tuya. Inténtalo de nuevo.`);
      }
    }
    for (const { sectorId, seatId } of seats) {
      const seat = state.events[eventId].sectors[sectorId][seatId];
      seat.status = "sold"; seat.reservedUntil = null;
    }
    saveState(state);
    return { ok: true, ticketId: "TCK-" + Math.random().toString(36).slice(2, 8).toUpperCase() };
  },

  TTL_MS,
};
