import { GOOGLE_CLIENT_ID } from "./auth-config.js";
import { saveSession, getSession, parseIdTokenPayload } from "./auth-session.js";
import { api, getAuthPublicConfig, devLogin } from "./api/api.js";
import { applyAppBranding } from "./app-branding.js";
import { showToast } from "./ui/toast.js";
import { initCarousel } from "./ui/carousel.js";
import { renderCatalog } from "./ui/render.js";

const $ = (id) => document.getElementById(id);

function waitForGoogle(callback) {
  if (window.google?.accounts?.id) {
    callback();
    return;
  }
  const poll = setInterval(() => {
    if (window.google?.accounts?.id) {
      clearInterval(poll);
      callback();
    }
  }, 50);
}

function showStatus(text) {
  showToast(text, "error");
}

function setupDevLoginButton() {
  const wrap = $("dev-login-wrap");
  if (!wrap) return;

  wrap.classList.remove("hidden");
  wrap.innerHTML = `
    <button type="button" id="btn-dev-login"
      class="w-full rounded-lg border border-slate-300 bg-slate-50 px-4 py-3 text-sm font-semibold text-slate-800 hover:bg-slate-100 transition">
      Entrar en modo prueba (sin Google)
    </button>
    <p class="text-xs text-slate-500 mt-2">Usuario fijo en la base de datos; solo para desarrollo con Docker.</p>
  `;

  $("btn-dev-login").addEventListener("click", async () => {
    $("login-status").classList.add("hidden");
    try {
      const profile = await devLogin();
      const exp = Math.floor(Date.now() / 1000) + 86400 * 365;
      saveSession({
        userId: profile.userId,
        email: profile.email,
        name: profile.name,
        sub: "dev-docker-ticketing-local",
        exp
      });
      window.location.href = "app.html";
    } catch (e) {
      showStatus(e.message || "Error al iniciar sesión de prueba.");
    }
  });
}

function initGoogleButton(clientId) {
  waitForGoogle(() => {
    window.google.accounts.id.initialize({
      client_id: clientId,
      callback: async (response) => {
        $("login-status").classList.add("hidden");
        try {
          const profile = await api.syncGoogleUser(response.credential);
          const payload = parseIdTokenPayload(response.credential);
          saveSession({
            credential: response.credential,
            userId: profile.userId,
            email: profile.email,
            name: profile.name,
            picture: payload.picture,
            sub: payload.sub,
            exp: payload.exp
          });
          window.location.href = "app.html";
        } catch (e) {
          showStatus(e.message || "Error al conectar con el servidor.");
        }
      },
      ux_mode: "popup"
    });

    window.google.accounts.id.renderButton($("google-signin"), {
      theme: "filled_blue",
      size: "large",
      text: "continue_with",
      shape: "rectangular",
      locale: "es"
    });
  });
}

async function boot() {
  await applyAppBranding("Inicio");

  if (getSession()) {
    window.location.replace("app.html");
    return;
  }

  // Modal Logic
  const modal = $("login-modal");
  const btnHeader = $("btn-login-header");
  const btnClose = $("btn-close-modal");

  function openModal() {
    modal.classList.remove("hidden");
  }

  function closeModal() {
    modal.classList.add("hidden");
  }

  if (btnHeader) btnHeader.addEventListener("click", openModal);
  if (btnClose) btnClose.addEventListener("click", closeModal);
  if (modal) {
    modal.addEventListener("click", (e) => {
      if (e.target === modal) closeModal();
    });
  }

  initCarousel();


  const catalogContainer = $("view-catalog");
  if (catalogContainer) {
    const publicState = { page: 1, pageSize: 10 };

    async function loadPublicCatalog() {
      try {
        const result = await api.listEvents(publicState.page, publicState.pageSize);
        renderCatalog(
          catalogContainer,
          result.events,
          openModal,
          result.pagination,
          async (newPage) => {
            publicState.page = newPage;
            await loadPublicCatalog();
          },
          async (newSize) => {
            publicState.pageSize = Number(newSize);
            publicState.page = 1;
            await loadPublicCatalog();
          }
        );
      } catch (e) {
        console.error("Error loading public catalog", e);
      }
    }

    await loadPublicCatalog();
  }

  const localId = (GOOGLE_CLIENT_ID || "").trim();
  const serverCfg = await getAuthPublicConfig();
  const clientId = localId || serverCfg.clientId;
  const devOk = serverCfg.devLoginAvailable;

  if (!clientId && !devOk) {
    showStatus(
      "No hay forma de iniciar sesión: configura GOOGLE_CLIENT_ID en .env o en la API, " +
      "o habilita Ticketing:EnableDockerDevLogin en Development."
    );
    return;
  }

  if (clientId) {
    $("google-signin").classList.remove("hidden");
    initGoogleButton(clientId);
  } else {
    $("google-signin").classList.add("hidden");
    const hint = document.querySelector("#login-google-hint");
    if (hint) hint.classList.add("hidden");
  }

  if (devOk) {
    setupDevLoginButton();
  }

  if (clientId && devOk) {
    const sep = document.createElement("p");
    sep.className = "text-xs text-slate-400 my-4";
    sep.textContent = "— o —";
    $("dev-login-wrap")?.parentNode?.insertBefore(sep, $("dev-login-wrap"));
  }
}

void boot();
