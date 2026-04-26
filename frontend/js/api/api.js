const BASE_URL = "http://localhost:5000/api/v1";

export const api = {
  async listEvents(page = 1, pageSize = 10) {
    const res = await fetch(
      `${BASE_URL}/events?page=${page}&pageSize=${pageSize}`
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

        // Temporal mientras backend no devuelva metadata real:
        totalItems: json.data.length,
        totalPages: 1,
        hasNext: false,
        hasPrevious: false
      }
    };
  },

  async getSeats(eventId) {
    const res = await fetch(`${BASE_URL}/events/${eventId}/seats`);
    const json = await res.json();

    return json.data;
  },

  async reserveSeat(eventId, sectorId, seatId, owner = "me") {
    const res = await fetch(`${BASE_URL}/reservations`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json"
      },
      body: JSON.stringify({
        eventId: Number(eventId),
        sectorId: Number(sectorId),
        seatId: Number(seatId),
        userId: crypto.randomUUID()
      })
    });

    if (!res.ok) {
      const err = await res.json();
      throw new Error(err.error || "No se pudo reservar");
    }

    return await res.json();
  }
};