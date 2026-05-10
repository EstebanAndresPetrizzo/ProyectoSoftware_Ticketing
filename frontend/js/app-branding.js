import { API_BASE_URL } from "./api/api.js";

let cache = null;

function escapeHtml(text) {
  const div = document.createElement("div");
  div.textContent = text;
  return div.innerHTML;
}

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
    /* API no disponible: valores por defecto del proyecto */
  }
  cache = {
    name: "TicketingApp",
    contactEmail: "petrizzoesteban@gmail.com"
  };
  return cache;
}

/**
 * @param {string} titleSuffix - parte final del título del documento (ej. "Iniciar sesión").
 */
export async function applyAppBranding(titleSuffix) {
  const b = await fetchAppBranding();
  const name = (b.name || "TicketingApp").trim();
  document.title = `${name} — ${titleSuffix}`;
  document.querySelector('meta[name="application-name"]')?.setAttribute("content", name);
  document.querySelectorAll("[data-app-title]").forEach((el) => {
    el.textContent = `🎫 ${name}`;
  });
  const foot = document.getElementById("app-footer-contact");
  if (foot && b.contactEmail) {
    const raw = String(b.contactEmail).trim();
    const safe = escapeHtml(raw);
    foot.innerHTML = `Contacto: <a class="underline hover:opacity-90" href="mailto:${encodeURIComponent(raw)}">${safe}</a>`;
  }
}
