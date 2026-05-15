import { API_BASE_URL } from "./api/api.js";

let cache = null;


export async function fetchAppBranding() {
  if (cache) {
    return cache;
  }
  try {
    const res = await fetch(`${API_BASE_URL}/app/branding`);
    const json = await res.json();
    if (res.ok && json.success && json.data) {
      cache = json.data;
      return cache;
    }
  } catch {
  }
  cache = {
    name: "TicketingApp",
    contactEmail: "petrizzoesteban@gmail.com"
  };
  return cache;
}

export async function applyAppBranding(titleSuffix) {
  const b = await fetchAppBranding();
  const name = (b.name || "TicketingApp").trim();
  document.title = `${name} — ${titleSuffix}`;
  document.querySelector('meta[name="application-name"]')?.setAttribute("content", name);
}
