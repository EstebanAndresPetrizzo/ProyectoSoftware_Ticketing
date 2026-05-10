import { api } from "./api/api.js";
import { renderCatalog, renderStadium, renderSelection } from "./ui/render.js";
import { formatDateTime } from "./mappers/seatMapMapper.js";
import { PaymentModal } from "./ui/payment-modal.js";
import {
  ensureSessionOrRedirect,
  getSession,
  clearSession
} from "./auth-session.js";
import { applyAppBranding } from "./app-branding.js";

ensureSessionOrRedirect();

const paymentModal = new PaymentModal();

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

function setupUserBar() {
  const bar = document.getElementById("user-bar");
  if (!bar) return;

  const s = getSession();
  if (!s) return;

  bar.innerHTML = `
    <span class="text-slate-200 hidden sm:inline">${escapeHtml(s.name || s.email || "")}</span>
    <button type="button" id="btn-logout"
      class="rounded-lg bg-white/10 px-3 py-1.5 font-medium text-white hover:bg-white/20 transition">
      Cerrar sesión
    </button>
  `;

  document.getElementById("btn-logout")?.addEventListener("click", () => {
    clearSession();
    window.location.href = "index.html";
  });
}

function escapeHtml(text) {
  const div = document.createElement("div");
  div.textContent = text;
  return div.innerHTML;
}

async function init() {
  await applyAppBranding("Eventos");
  setupUserBar();
  await loadCatalog();

  $("btn-back").addEventListener("click", () => {
    void backToCatalog();
  });
  $("btn-buy").addEventListener("click", confirmPurchase);
  $("stadium-map").addEventListener("click", (e) => {
    const btn = e.target.closest("[data-seat-id]");
    if (!btn) return;
    
    const seatId = Number(btn.dataset.seatId);
    if (!isNaN(seatId)) {
      onSeatClick(seatId);
    }
  });
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

  $("event-title").textContent = evt.name;
  $("event-meta").textContent = `📅 ${formatDateTime(evt.date)} · 📍 ${evt.venue}`;

  await refreshSeats();

  clearInterval(state.pollTimer);
  state.pollTimer = setInterval(refreshSeats, 5000);
}

async function backToCatalog() {
  await Promise.all(
    state.selected.map(s =>
      api.releaseSeat(state.eventId, s.sectorId, s.seatId).catch(() => {})
    )
  );

  state.selected = [];

  clearInterval(state.pollTimer);
  clearInterval(state.countdownTimer);

  $("countdown").classList.add("hidden");
  $("view-seats").classList.add("hidden");
  $("view-catalog").classList.remove("hidden");

  loadCatalog();
}

function collectMyPendingSelection(eventState) {
  const sel = [];
  for (const sector of eventState.sectors) {
    const sid = sector.sectorId ?? sector.id;
    for (const seat of sector.seats) {
      if (seat.isMine && seat.myPendingExpiresAtUtc) {
        const existing = state.selected.find(s => s.seatId === seat.id);
        sel.push({
          seatId: seat.id,
          sectorId: sid,
          reservedUntil: new Date(seat.myPendingExpiresAtUtc).getTime(),
          reservationId: existing?.reservationId
        });
      }
    }
  }
  return sel;
}

async function refreshSeats() {
  state.eventState = await api.getSeats(state.eventId);
  state.selected = collectMyPendingSelection(state.eventState);

  renderStadium($("stadium-map"), state.eventState, state.selected, onSeatClick);
  updateSelectionUI();
  updateCountdown();
}

async function onSeatClick(seatId) {
  const alreadySelected = state.selected.find(s => s.seatId === seatId);

  if (alreadySelected) {
    try {
      await api.releaseSeat(state.eventId, alreadySelected.sectorId, seatId);
    } catch (err) {
      alert("⚠️ " + err.message);
    }
    await refreshSeats();
    return;
  }

  try {
    let foundSectorId = null;
    for (const sector of state.eventState.sectors) {
      const seat = sector.seats.find(s => s.id === seatId);
      if (seat) {
        foundSectorId = sector.id || sector.sectorId;
        break;
      }
    }

    if (!foundSectorId) throw new Error("No se encontró el sector del asiento");

    const reservation = await api.reserveSeat(state.eventId, foundSectorId, seatId);
    state.selected.push({
      seatId,
      sectorId: foundSectorId,
      reservedUntil: Date.now() + 5 * 60 * 1000,
      reservationId: reservation.id
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
  if (state.selected.length === 0) {
    alert("⚠️ No has seleccionado ninguna butaca");
    return;
  }

  try {
    let totalAmount = 0;
    for (const selectedSeat of state.selected) {
      for (const sector of state.eventState?.sectors ?? []) {
        const foundSeat = sector.seats.find(s => s.id === selectedSeat.seatId);
        if (foundSeat) {
          totalAmount += sector.price ?? 0;
          break;
        }
      }
    }

    const firstReservation = state.selected[0];
    if (!firstReservation || !firstReservation.reservationId) {
      throw new Error("No se encontró la reservación para procesar el pago.");
    }

    const reservationData = {
      id: firstReservation.reservationId,
      amount: totalAmount,
      seats: state.selected.length
    };

    paymentModal.onPaymentSuccess = async (paymentResult) => {
      alert(`✓ ¡Pago completado!\nID de Pago: ${paymentResult.transactionId || paymentResult.id}`);
      await backToCatalog();
    };

    paymentModal.onPaymentError = async () => {
      alert("✕ El pago fue rechazado. Intenta de nuevo o selecciona otro método.");
    };

    paymentModal.open(reservationData);
  } catch (err) {
    alert("❌ " + err.message);
    await refreshSeats();
  }
}

init();