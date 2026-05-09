const STORAGE_KEY = "ticketing_google_session";

export function parseIdTokenPayload(token) {
  const part = token.split(".")[1];
  const base64 = part.replace(/-/g, "+").replace(/_/g, "/");
  const json = decodeURIComponent(
    atob(base64)
      .split("")
      .map((c) => "%" + ("00" + c.charCodeAt(0).toString(16)).slice(-2))
      .join("")
  );
  return JSON.parse(json);
}

export function getSession() {
  try {
    const raw = sessionStorage.getItem(STORAGE_KEY);
    if (!raw) return null;
    const s = JSON.parse(raw);
    if (!s.userId) {
      clearSession();
      return null;
    }
    if (s.exp && s.exp < Date.now() / 1000) {
      clearSession();
      return null;
    }
    return s;
  } catch {
    return null;
  }
}

export function saveSession(data) {
  sessionStorage.setItem(STORAGE_KEY, JSON.stringify(data));
}

export function clearSession() {
  sessionStorage.removeItem(STORAGE_KEY);
}

export function ensureSessionOrRedirect() {
  if (!getSession()) {
    window.location.replace("index.html");
  }
}
