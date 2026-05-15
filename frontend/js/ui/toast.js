export function showToast(message, type = 'info', duration = 5000) {
  let container = document.getElementById('toast-container');
  if (!container) {
    container = document.createElement('div');
    container.id = 'toast-container';
    container.className = 'fixed top-4 right-4 z-50 flex flex-col gap-3 max-w-sm w-full pointer-events-none';
    document.body.appendChild(container);
  }

  const toast = document.createElement('div');
  
  // Base classes for the toast
  let baseClasses = 'pointer-events-auto flex items-center p-3.5 pr-4 rounded-xl shadow-[0_8px_30px_rgb(0,0,0,0.12)] border border-white/60 bg-white/90 backdrop-blur-md transform transition-all duration-300 translate-x-full opacity-0 translate-y-2 sm:translate-y-0';
  
  let typeClasses = '';
  let iconHtml = '';
  let title = '';

  switch (type) {
    case 'success':
      title = 'Éxito';
      iconHtml = `<div class="w-10 h-10 flex-shrink-0 flex items-center justify-center rounded-full bg-emerald-50 text-emerald-500 mr-3 border border-emerald-100/50"><svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2.5" d="M5 13l4 4L19 7"></path></svg></div>`;
      break;
    case 'error':
      title = 'Error';
      iconHtml = `<div class="w-10 h-10 flex-shrink-0 flex items-center justify-center rounded-full bg-rose-50 text-rose-500 mr-3 border border-rose-100/50"><svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2.5" d="M6 18L18 6M6 6l12 12"></path></svg></div>`;
      break;
    case 'warning':
      title = 'Aviso';
      iconHtml = `<div class="w-10 h-10 flex-shrink-0 flex items-center justify-center rounded-full bg-amber-50 text-amber-500 mr-3 border border-amber-100/50"><svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2.5" d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-3L13.732 4c-.77-1.333-2.694-1.333-3.464 0L3.34 16c-.77 1.333.192 3 1.732 3z"></path></svg></div>`;
      break;
    case 'info':
    default:
      title = 'Información';
      iconHtml = `<div class="w-10 h-10 flex-shrink-0 flex items-center justify-center rounded-full bg-blue-50 text-blue-500 mr-3 border border-blue-100/50"><svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2.5" d="M13 16h-1v-4h-1m1-4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z"></path></svg></div>`;
      break;
  }

  toast.className = `${baseClasses} ${typeClasses}`;
  
  toast.innerHTML = `
    ${iconHtml}
    <div class="flex-1 min-w-0 pr-3">
      <h4 class="text-[13px] font-bold text-slate-800 leading-none mb-1">${title}</h4>
      <p class="text-[13px] text-slate-500 leading-snug">${message}</p>
    </div>
    <button class="flex-shrink-0 ml-auto text-slate-400 hover:text-slate-700 bg-slate-50 hover:bg-slate-100 rounded-full p-1.5 transition-colors focus:outline-none focus:ring-2 focus:ring-slate-200" aria-label="Close">
      <svg class="w-4 h-4" fill="currentColor" viewBox="0 0 20 20"><path fill-rule="evenodd" d="M4.293 4.293a1 1 0 011.414 0L10 8.586l4.293-4.293a1 1 0 111.414 1.414L11.414 10l4.293 4.293a1 1 0 01-1.414 1.414L10 11.414l-4.293 4.293a1 1 0 01-1.414-1.414L8.586 10 4.293 5.707a1 1 0 010-1.414z" clip-rule="evenodd"></path></svg>
    </button>
  `;

  container.appendChild(toast);

  // Trigger animation after a brief delay so the browser registers the initial state
  requestAnimationFrame(() => {
    requestAnimationFrame(() => {
      toast.classList.remove('translate-x-full', 'opacity-0');
    });
  });

  const closeButton = toast.querySelector('button');
  
  const removeToast = () => {
    toast.classList.add('translate-x-full', 'opacity-0');
    // Wait for the animation to finish before removing from DOM
    setTimeout(() => {
      if (toast.parentNode) {
        toast.parentNode.removeChild(toast);
      }
    }, 300);
  };

  closeButton.addEventListener('click', removeToast);

  if (duration > 0) {
    setTimeout(removeToast, duration);
  }
}
