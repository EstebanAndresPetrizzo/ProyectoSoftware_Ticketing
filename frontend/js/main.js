import { api } from "./api.js";
import { renderCatalog, renderStadium, renderSelection } from "./render.js";

const state = {
  eventId: null,
  stadium: null,
  eventState: null,
  selected: [], // [{ sectorId, seatId, reservedUntil }]
  pollTimer: null,
  countdownTimer: null,
};

const $ = (id) => document.getElementById(id);

async function init() {
  state.stadium = await api.getStadium();
  const events = await api.listEvents();
  renderCatalog($("view-catalog"), events, openEvent);
  $("btn-back").addEventListener("click", backToCatalog);
  $("btn-buy").addEventListener("click", confirmPurchase);
}

async function openEvent(eventId) {
  state.eventId = eventId;
  state.selected = [];
  $("view-catalog").classList.add("hidden");
  $("view-seats").classList.remove("hidden");
  const events = await api.listEvents();
  const evt = events.find(e => e.id === eventId);
  $("event-title").textContent = evt.title;
  $("event-meta").textContent = `📅 ${evt.date} · 📍 ${evt.venue}`;
  await refreshSeats();
  state.pollTimer = setInterval(refreshSeats, 5000); // sync concurrencia cada 5s
}

function backToCatalog() {
  // liberar mis reservas pendientes
  state.selected.forEach(s => api.releaseSeat(state.eventId, s.sectorId, s.seatId));
  state.selected = [];
  clearInterval(state.pollTimer);
  clearInterval(state.countdownTimer);
  $("countdown").classList.add("hidden");
  $("view-seats").classList.add("hidden");
  $("view-catalog").classList.remove("hidden");
  updateSelectionUI();
}

async function refreshSeats() {
  state.eventState = await api.getSeats(state.eventId);
  // limpiar selecciones que ya no son mías (TTL expirado o robadas)
  state.selected = state.selected.filter(sel => {
    const seat = state.eventState.sectors[sel.sectorId][sel.seatId];
    return seat.status === "reserved" && seat.owner === "me";
  });
  renderStadium($("stadium-map"), state.stadium, state.eventState, state.selected, onSeatClick);
  updateSelectionUI();
  updateCountdown();
}

async function onSeatClick(sectorId, seatId) {
  const already = state.selected.find(s => s.sectorId === sectorId && s.seatId === seatId);
  if (already) {
    await api.releaseSeat(state.eventId, sectorId, seatId);
    state.selected = state.selected.filter(s => s !== already);
    await refreshSeats();
    return;
  }
  try {
    const { reservedUntil } = await api.reserveSeat(state.eventId, sectorId, seatId);
    state.selected.push({ sectorId, seatId, reservedUntil });
    await refreshSeats();
  } catch (err) {
    alert("⚠️ " + err.message);
    await refreshSeats();
  }
}

function updateSelectionUI() {
  renderSelection($("selection-list"), $("total"), $("btn-buy"), state.selected, state.stadium);
}

function updateCountdown() {
  clearInterval(state.countdownTimer);
  if (state.selected.length === 0) {
    $("countdown").classList.add("hidden");
    return;
  }
  $("countdown").classList.remove("hidden");
  const tick = () => {
    const earliest = Math.min(...state.selected.map(s => s.reservedUntil));
    const remaining = Math.max(0, earliest - Date.now());
    const m = Math.floor(remaining / 60000);
    const s = Math.floor((remaining % 60000) / 1000);
    $("countdown-time").textContent = `${m}:${s.toString().padStart(2, "0")}`;
    if (remaining === 0) refreshSeats();
  };
  tick();
  state.countdownTimer = setInterval(tick, 1000);
}

async function confirmPurchase() {
  try {
    const res = await api.confirmPurchase(state.eventId, state.selected);
    alert(`✅ ¡Compra confirmada! Ticket: ${res.ticketId}`);
    state.selected = [];
    clearInterval(state.countdownTimer);
    $("countdown").classList.add("hidden");
    await refreshSeats();
  } catch (err) {
    alert("❌ " + err.message);
    await refreshSeats();
  }
}

init();
