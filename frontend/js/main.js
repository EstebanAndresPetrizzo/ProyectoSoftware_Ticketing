import { getEvents } from "./api.js";
import { renderEvents } from "./render.js";

async function init() {
  const events = await getEvents();
  renderEvents(events);
}

init();