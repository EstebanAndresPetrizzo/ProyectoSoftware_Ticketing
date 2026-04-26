const BASE_URL = "http://localhost:5000/api/v1";

export const api = {
  async listEvents() {
    const res = await fetch(`${BASE_URL}/events`);
    const json = await res.json();

    return json.data.map(event => ({
      id: event.id,
      name: event.name,
      date: event.date,
      venue: event.venue
    }));
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