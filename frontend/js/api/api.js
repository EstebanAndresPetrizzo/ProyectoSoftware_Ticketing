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
        // Estimación: si la página está llena, hay más páginas
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
        userId: "11111111-1111-1111-1111-111111111111"
      })
    });

    if (!res.ok) {
      const err = await res.json();
      throw new Error(err.error || "No se pudo reservar");
    }

    return await res.json();
  }
};