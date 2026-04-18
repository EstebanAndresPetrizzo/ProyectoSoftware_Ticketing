export async function getEvents() {
  return [
    {
      id: 1,
      name: "Concierto de Rock",
      date: "2026-05-10"
    },
    {
      id: 2,
      name: "Festival Electrónico",
      date: "2026-06-15"
    }
  ];
}

export async function getSeats(eventId) {
  const seats = [];

  for (let i = 1; i <= 50; i++) {
    seats.push({
      id: i,
      number: `A${i}`,
      status: randomStatus()
    });
  }

  return seats;
}

function randomStatus() {
  const states = ["AVAILABLE", "RESERVED", "SOLD"];
  return states[Math.floor(Math.random() * states.length)];
}

export async function reserveSeat(seatId) {
  // simulamos latencia
  await new Promise(resolve => setTimeout(resolve, 500));

  // simulamos concurrencia (70% éxito)
  return Math.random() > 0.3;
}