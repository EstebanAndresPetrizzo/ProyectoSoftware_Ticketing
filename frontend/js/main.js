import { api } from "./api/api.js";
import { renderCatalog, renderStadium, renderSelection } from "./ui/render.js";
import { showToast } from "./ui/toast.js";
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

  const displayName = escapeHtml(s.name || s.email || "");
  const initial = displayName ? displayName.charAt(0).toUpperCase() : "U";

  bar.innerHTML = `
    <div class="flex items-center gap-2">
      <div class="w-8 h-8 rounded bg-white/20 text-white border border-white/30 flex items-center justify-center font-bold text-sm">
        ${initial}
      </div>
      <span class="text-white font-medium hidden sm:inline">${displayName}</span>
    </div>
    <button type="button" id="btn-logout"
      class="rounded bg-white px-4 py-1.5 text-sm font-bold text-blue-600 hover:bg-slate-50 transition-colors shadow-sm hover:shadow">
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
    if (state.selected && state.selected.length > 0) {
      $("exit-warning-modal").classList.remove("hidden");
    } else {
      void backToCatalog();
    }
  });

  $("btn-stay-modal").addEventListener("click", () => {
    $("exit-warning-modal").classList.add("hidden");
  });

  $("btn-leave-modal").addEventListener("click", async () => {
    $("exit-warning-modal").classList.add("hidden");

    // El usuario confirmó salir, liberamos las reservas
    await Promise.all(
      state.selected.map(s =>
        api.releaseSeat(state.eventId, s.sectorId, s.seatId).catch(() => { })
      )
    );
    state.selected = [];

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

  $("main-content-grid").classList.remove("lg:grid-cols-[1fr]");
  $("main-content-grid").classList.add("lg:grid-cols-[1fr_320px]");
  $("sidebar-panel").classList.remove("hidden");

  const { events } = await api.listEvents();
  const evt = events.find(e => e.id === state.eventId);

  $("event-title").textContent = evt.name;
  $("event-meta").innerHTML = `
    <span class="inline-flex items-center gap-1.5 px-2.5 py-1 bg-blue-50 text-blue-700 text-[11px] font-bold rounded border border-blue-200">
      <svg class="w-3 h-3 text-blue-400" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8 7V3m8 4V3m-9 8h10M5 21h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v12a2 2 0 002 2z"></path></svg>
      ${formatDateTime(evt.date)}
    </span>
    <span class="inline-flex items-center gap-1.5 px-2.5 py-1 bg-emerald-50 text-emerald-700 text-[11px] font-bold rounded border border-emerald-200">
      <svg class="w-3 h-3 text-emerald-500" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M17.657 16.657L13.414 20.9a1.998 1.998 0 01-2.827 0l-4.244-4.243a8 8 0 1111.314 0z"></path><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 11a3 3 0 11-6 0 3 3 0 016 0z"></path></svg>
      ${evt.venue}
    </span>
  `;

  await refreshSeats();

  clearInterval(state.pollTimer);
  state.pollTimer = setInterval(refreshSeats, 5000);
}

async function backToCatalog() {
  state.selected = [];

  clearInterval(state.pollTimer);
  clearInterval(state.countdownTimer);

  $("countdown").classList.add("hidden");
  $("view-seats").classList.add("hidden");
  $("sidebar-panel").classList.add("hidden");
  $("main-content-grid").classList.add("lg:grid-cols-[1fr]");
  $("main-content-grid").classList.remove("lg:grid-cols-[1fr_320px]");

  $("view-catalog").classList.remove("hidden");

  loadCatalog();
}

function collectMyPendingSelection(eventState) {
  const sel = [];
  for (const sector of eventState.sectors) {
    const sid = sector.id ?? sector.sectorId;
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
      showToast(err.message, "warning");
    }
    await refreshSeats();
    return;
  }

  try {
    let foundSectorId = null;
    for (const sector of state.eventState.sectors) {
      const seat = sector.seats.find(s => s.id === seatId);
      if (seat) {
        foundSectorId = sector.id ?? sector.sectorId;
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
    showToast(err.message, "warning");
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
      showToast("Tu reserva ha expirado. Las butacas han sido liberadas.", "warning");
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

    // Recopilar todos los IDs de reserva
    const reservationIds = state.selected.map(s => s.reservationId).filter(id => id);
    
    if (reservationIds.length === 0) {
      throw new Error("No se encontraron las reservaciones para procesar el pago.");
    }

    const reservationData = {
      ids: reservationIds,  // Enviar todos los IDs
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
    showToast(err.message, "error");
    await refreshSeats();
  }
}

init();