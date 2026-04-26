import { api } from "./api/api.js";
import { renderCatalog, renderStadium, renderSelection } from "./ui/render.js";

const state = {
  eventId: null,
  stadium: null,
  eventState: null,
  selected: [],
  pollTimer: null,
  countdownTimer: null,

  currentPage: 1,
  pageSize: 10,
  pagination: null
};

const $ = (id) => document.getElementById(id);

async function init() {
  await loadCatalog();

  $("btn-back").addEventListener("click", backToCatalog);
  $("btn-buy").addEventListener("click", confirmPurchase);
}

async function loadCatalog() {
  const result = await api.listEvents(
    state.currentPage,
    state.pageSize
  );

  state.pagination = result.pagination;

  renderCatalog(
    $("view-catalog"),
    result.events,
    openEvent,
    state.pagination,
    changePage,
    changePageSize
  );
}

async function changePageSize(newSize) {
  state.pageSize = Number(newSize);
  state.currentPage = 1;

  await loadCatalog();
}

async function changePage(page) {
  if (
    page < 1 ||
    page > state.pagination.totalPages
  ) return;

  state.currentPage = page;

  await loadCatalog();
}

async function openEvent(eventId) {
  state.eventId = Number(eventId);
  state.selected = [];

  $("view-catalog").classList.add("hidden");
  $("view-seats").classList.remove("hidden");

  const { events } = await api.listEvents();
  const evt = events.find(e => e.id === state.eventId);

  $("event-title").textContent = evt.title;
  $("event-meta").textContent = `📅 ${evt.date} · 📍 ${evt.venue}`;

  await refreshSeats();

  clearInterval(state.pollTimer);
  state.pollTimer = setInterval(refreshSeats, 5000);
}

function backToCatalog() {
  state.selected.forEach(s =>
    api.releaseSeat(state.eventId, s.sectorId, s.seatId)
  );

  state.selected = [];

  clearInterval(state.pollTimer);
  clearInterval(state.countdownTimer);

  $("countdown").classList.add("hidden");
  $("view-seats").classList.add("hidden");
  $("view-catalog").classList.remove("hidden");

  loadCatalog();
}

async function refreshSeats() {
  state.eventState = await api.getSeats(state.eventId);

  const allSeats = state.eventState.sectors.flatMap(s => s.seats);

  state.selected = state.selected.filter(sel => {
    const seat = allSeats.find(s => s.id === sel.seatId);

    return (
      seat &&
      seat.status.toLowerCase() === "reserved"
    );
  });

  renderStadium(
    $("stadium-map"),
    state.eventState,
    state.selected,
    onSeatClick
  );

  updateSelectionUI();
  updateCountdown();
}

async function onSeatClick(seatId) {
  const alreadySelected = state.selected.find(
    s => s.seatId === seatId
  );

  if (alreadySelected) {
    await api.releaseSeat(
      state.eventId,
      seatId
    );

    state.selected = state.selected.filter(
      s => s.seatId !== seatId
    );

    await refreshSeats();
    return;
  }

  try {
    const result = await api.reserveSeat(
      state.eventId,
      seatId
    );

    state.selected.push({
      seatId,
      reservedUntil: result.reservedUntil
    });

    await refreshSeats();
  } catch (err) {
    alert("⚠️ " + err.message);
    await refreshSeats();
  }
}

function updateSelectionUI() {
  renderSelection(
    $("selection-list"),
    $("total"),
    $("btn-buy"),
    state.selected,
    state.eventState
  );
}

function updateCountdown() {
  clearInterval(state.countdownTimer);

  if (state.selected.length === 0) {
    $("countdown").classList.add("hidden");
    return;
  }

  $("countdown").classList.remove("hidden");

  const tick = () => {
    const earliestReservation = Math.min(
      ...state.selected.map(s => s.reservedUntil)
    );

    const remaining = Math.max(
      0,
      earliestReservation - Date.now()
    );

    const minutes = Math.floor(remaining / 60000);
    const seconds = Math.floor((remaining % 60000) / 1000);

    $("countdown-time").textContent =
      `${minutes}:${seconds.toString().padStart(2, "0")}`;

    if (remaining === 0) {
      refreshSeats();
    }
  };

  tick();

  state.countdownTimer = setInterval(
    tick,
    1000
  );
}

async function confirmPurchase() {
  try {
    const result = await api.confirmPurchase(
      state.eventId,
      state.selected.map(s => s.seatId)
    );

    alert(`✅ ¡Compra confirmada! Ticket: ${result.ticketId}`);

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