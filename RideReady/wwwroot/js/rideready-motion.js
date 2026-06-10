/* ============================================================
   RideReady — Camada de movimento (JS)
   ------------------------------------------------------------
   Dois ajudantes, sem bibliotecas:
   1) Revelação ao scroll  — qualquer elemento com a classe
      .rr-reveal surge quando entra no ecrã (ganha .rr-in).
   2) Count-up             — qualquer elemento com o atributo
      data-rr-count="48" conta de 0 até esse número.
   Chamar rrMotion.scan() outra vez se renderizares conteúdo novo.
   ============================================================ */

window.rrMotion = (function () {
  function countUp(el) {
    var target = parseInt(el.getAttribute('data-rr-count'), 10) || 0;
    var dur = 1000, t0 = null;
    function step(ts) {
      if (!t0) t0 = ts;
      var k = Math.min((ts - t0) / dur, 1);
      el.textContent = Math.round(k * target);
      if (k < 1) requestAnimationFrame(step);
    }
    el.textContent = '0';
    requestAnimationFrame(step);
  }

  function scan() {
    var els = document.querySelectorAll('.rr-reveal:not(.rr-watched)');

    // Sem suporte de IntersectionObserver: mostra tudo já.
    if (!('IntersectionObserver' in window)) {
      els.forEach(function (e) {
        e.classList.add('rr-in', 'rr-watched');
        e.querySelectorAll('[data-rr-count]').forEach(countUp);
      });
      return;
    }

    var io = new IntersectionObserver(function (entries) {
      entries.forEach(function (en) {
        if (en.isIntersecting) {
          en.target.classList.add('rr-in');
          en.target.querySelectorAll('[data-rr-count]').forEach(countUp);
          io.unobserve(en.target);
        }
      });
    }, { threshold: 0.15, rootMargin: '0px 0px -40px 0px' });

    els.forEach(function (e) {
      e.classList.add('rr-watched');
      io.observe(e);
    });
  }

  return { scan: scan, countUp: countUp };
})();

// Arranca quando a página carrega.
if (document.readyState !== 'loading') window.rrMotion.scan();
else document.addEventListener('DOMContentLoaded', function () { window.rrMotion.scan(); });
