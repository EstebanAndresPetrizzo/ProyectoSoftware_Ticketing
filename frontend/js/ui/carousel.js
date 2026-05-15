export function initCarousel() {
  const track = document.getElementById('carousel-track');
  const prevBtn = document.getElementById('carousel-prev');
  const nextBtn = document.getElementById('carousel-next');
  const dotsContainer = document.getElementById('carousel-dots');
  
  if (!track || !prevBtn || !nextBtn || !dotsContainer) return;

  const slidesCount = 3; // We hardcoded 3 slides
  let currentSlide = 0;
  let autoplayInterval = null;

  const dots = Array.from(dotsContainer.querySelectorAll('button'));

  function updateCarousel() {
    // Move track
    const translateX = -(currentSlide * (100 / slidesCount));
    track.style.transform = `translateX(${translateX}%)`;

    // Update dots
    dots.forEach((dot, index) => {
      if (index === currentSlide) {
        dot.classList.remove('w-2', 'bg-slate-300', 'hover:bg-slate-400');
        dot.classList.add('w-6', 'bg-blue-600');
      } else {
        dot.classList.remove('w-6', 'bg-blue-600');
        dot.classList.add('w-2', 'bg-slate-300', 'hover:bg-slate-400');
      }
    });
  }

  function nextSlide() {
    currentSlide = (currentSlide + 1) % slidesCount;
    updateCarousel();
  }

  function prevSlide() {
    currentSlide = (currentSlide - 1 + slidesCount) % slidesCount;
    updateCarousel();
  }

  function startAutoplay() {
    stopAutoplay();
    autoplayInterval = setInterval(nextSlide, 5000);
  }

  function stopAutoplay() {
    if (autoplayInterval) {
      clearInterval(autoplayInterval);
      autoplayInterval = null;
    }
  }

  // Event Listeners
  nextBtn.addEventListener('click', () => {
    nextSlide();
    startAutoplay(); // Reset timer on manual interaction
  });

  prevBtn.addEventListener('click', () => {
    prevSlide();
    startAutoplay();
  });

  dots.forEach(dot => {
    dot.addEventListener('click', (e) => {
      currentSlide = parseInt(e.target.dataset.slide, 10);
      updateCarousel();
      startAutoplay();
    });
  });

  // Pause autoplay on hover
  const carouselContainer = document.getElementById('hero-carousel');
  carouselContainer.addEventListener('mouseenter', stopAutoplay);
  carouselContainer.addEventListener('mouseleave', startAutoplay);

  // Initialize
  startAutoplay();
}
