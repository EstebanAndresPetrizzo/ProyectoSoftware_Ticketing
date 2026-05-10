import { getSession } from "../auth-session.js";

export const API_BASE_URL = "http://localhost:5000/api/v1";

/** Config pública de auth: Client ID de Google y si el servidor permite login de prueba (Docker). */
export async function getAuthPublicConfig() {
  try {
    const res = await fetch(`${API_BASE_URL}/auth/google-config`);
    const json = await res.json();
    if (!res.ok || json.success === false) {
      return { clientId: "", devLoginAvailable: false };
    }
    const id = json.data?.clientId;
    return {
      clientId: typeof id === "string" ? id.trim() : "",
      devLoginAvailable: Boolean(json.data?.devLoginAvailable)
    };
  } catch {
    return { clientId: "", devLoginAvailable: false };
  }
}

/** @deprecated Usa getAuthPublicConfig; se mantiene por compatibilidad. */
export async function getGoogleClientIdFromServer() {
  const cfg = await getAuthPublicConfig();
  return cfg.clientId;
}

export async function devLogin() {
  const res = await fetch(`${API_BASE_URL}/auth/dev-login`, { method: "POST" });
  const json = await res.json();
  if (!res.ok || !json.success) {
    throw new Error(json.error || "No se pudo iniciar sesión de prueba.");
  }
  return json.data;
}

function requireUserId() {
  const uid = getSession()?.userId;
  if (!uid) {
    throw new Error("Sesión inválida: vuelve a iniciar sesión.");
  }
  return uid;
}

async function readErrorMessage(res) {
  try {
    const err = await res.json();
    return err.error || err.message || res.statusText;
  } catch {
    return res.statusText;
  }
}

export const api = {
  async syncGoogleUser(idToken) {
    const res = await fetch(`${API_BASE_URL}/auth/google`, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ idToken })
    });
    const json = await res.json();
    if (!res.ok || !json.success) {
      throw new Error(json.error || "No se pudo registrar el usuario en el servidor.");
    }
    return json.data;
  },

  async listEvents(page = 1, pageSize = 10) {
    const res = await fetch(
      `${API_BASE_URL}/events?page=${page}&pageSize=${pageSize}`
    );

    const json = await res.json();

    return {
      events: json.data.map(event => ({
        id: event.id,
        name: event.name,
        date: event.date,
        venue: event.venue
      })),

      pagination: {
        page,
        pageSize,
        totalItems: json.data.length === pageSize
          ? (page + 1) * pageSize
          : json.data.length,
        totalPages: json.data.length === pageSize
          ? page + 1
          : Math.max(1, page),
        hasNext: json.data.length === pageSize,
        hasPrevious: page > 1
      }
    };
  },

  async getSeats(eventId) {
    const userId = requireUserId();
    const res = await fetch(
      `${API_BASE_URL}/events/${eventId}/seats?userId=${encodeURIComponent(userId)}`
    );
    const json = await res.json();

    if (!res.ok || !json.success) {
      throw new Error(json.error || "No se pudo cargar el mapa de butacas.");
    }

    return json.data;
  },

  async reserveSeat(eventId, sectorId, seatId) {
    const res = await fetch(`${API_BASE_URL}/reservations`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json"
      },
      body: JSON.stringify({
        eventId: Number(eventId),
        sectorId: Number(sectorId),
        seatId: Number(seatId),
        userId: requireUserId()
      })
    });

    const json = await res.json();
    if (!res.ok || !json.success) {
      throw new Error(json.error || await readErrorMessage(res));
    }

    return json.data;
  },

  async releaseSeat(eventId, sectorId, seatId) {
    const res = await fetch(`${API_BASE_URL}/reservations`, {
      method: "DELETE",
      headers: {
        "Content-Type": "application/json"
      },
      body: JSON.stringify({
        eventId: Number(eventId),
        sectorId: Number(sectorId),
        seatId: Number(seatId),
        userId: requireUserId()
      })
    });

    if (!res.ok) {
      throw new Error(await readErrorMessage(res));
    }

    return await res.json();
  },

  async confirmPurchase(_eventId, _seatIds) {
    throw new Error("La confirmación de compra no está implementada en la API.");
  },

  async processPayment(reservationId, amount, paymentMethod, cardData) {
    const userId = requireUserId();
    const res = await fetch(`${API_BASE_URL}/payments`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
        "X-User-Id": userId
      },
      body: JSON.stringify({
        reservationId,
        amount,
        paymentMethod,
        cardNumber: cardData.cardNumber,
        cardholderName: cardData.cardholderName,
        expiryMonth: cardData.expiryMonth,
        expiryYear: cardData.expiryYear,
        cvv: cardData.cvv
      })
    });

    const json = await res.json();

    if (!res.ok || !json.success) {
      throw new Error(json.error || "Error al procesar el pago");
    }

    return json.data;
  },

  async getPayment(paymentId) {
    const res = await fetch(`${API_BASE_URL}/payments/${paymentId}`);
    const json = await res.json();

    if (!res.ok || !json.success) {
      throw new Error(json.error || "Error al obtener detalles del pago");
    }

    return json.data;
  }
};
