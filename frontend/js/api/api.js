const BASE_URL = "http://localhost:5000/api/v1";

export const api = {
  async listEvents(page = 1, pageSize = 4) {
    const allEvents = [
    { id: "1", title: "Concierto de Rock", venue: "Estadio Nacional", date: "2026-06-26" },
    { id: "2", title: "Final eSports", venue: "Arena Central", date: "2026-07-26" },
    { id: "3", title: "Gala", venue: "Teatro", date: "2026-08-01" },
    { id: "4", title: "Festival Jazz", venue: "Arena", date: "2026-08-15" },
    { id: "5", title: "Mega Show", venue: "Stadium", date: "2026-09-01" },
    { id: "6", title: "Opera", venue: "Teatro", date: "2026-10-01" }
  ];

  const totalItems = allEvents.length;
  const totalPages = Math.ceil(totalItems / pageSize);

  const items = allEvents.slice(
    (page - 1) * pageSize,
    page * pageSize
  );

  return {
    events: items,
    pagination: {
      page,
      pageSize,
      totalItems,
      totalPages,
      hasNext: page < totalPages,
      hasPrevious: page > 1
    }
  };
    /*
    const res = await fetch(`${BASE_URL}/events`);
    const json = await res.json();

    return json.data.map(event => ({
      id: event.id,
      name: event.name,
      date: event.date,
      venue: event.venue
    }));
    */
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