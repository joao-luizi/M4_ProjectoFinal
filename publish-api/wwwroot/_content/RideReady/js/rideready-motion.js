/* RideReady — camada de movimento e interação leve.
   - rrMotion.scan(): revela elementos .rr-reveal ao entrarem no ecrã
   - rrMotion.countUp/countAll: números a contar ([data-rr-count])
   - rrMotion.initWizard(): os passos do registo (página SSR)
   Liga-se no carregamento normal E na navegação melhorada do Blazor. */

window.rrMotion = (function () {

    /* ---------- reveal ao scroll ---------- */
    function scan() {
        var els = document.querySelectorAll(".rr-reveal:not(.rr-in)");
        if (!els.length) return;
        var io = new IntersectionObserver(function (entries) {
            entries.forEach(function (e) {
                if (e.isIntersecting) {
                    e.target.classList.add("rr-in");
                    io.unobserve(e.target);
                }
            });
        }, { threshold: 0.15 });
        els.forEach(function (el) { io.observe(el); });
    }

    /* ---------- contagem animada ---------- */
    function countUp(el) {
        var target = parseInt(el.getAttribute("data-rr-count"), 10) || 0;
        var dur = 1200, t0 = null;
        function tick(t) {
            if (!t0) t0 = t;
            var p = Math.min((t - t0) / dur, 1);
            el.textContent = Math.round(target * (1 - Math.pow(1 - p, 3)));
            if (p < 1) requestAnimationFrame(tick);
        }
        requestAnimationFrame(tick);
    }
    /* Olhinho de mostrar/ocultar palavra-passe (paginas SSR do Identity).
       Delegado no documento: funciona em qualquer pagina, sem registo por campo. */
    document.addEventListener("click", function (e) {
        var btn = e.target.closest("[data-rr-eye]");
        if (!btn) return;
        var wrap = btn.closest(".form-floating");
        var input = wrap && wrap.querySelector("input");
        if (!input) return;
        var show = input.type === "password";
        input.type = show ? "text" : "password";
        btn.classList.toggle("rr-eye-on", show);
        btn.setAttribute("aria-label", show ? "Ocultar palavra-passe" : "Mostrar palavra-passe");
    });

    function countAll() {
        /* gatilho das animações dos gráficos: o CSS só anima
           debaixo de .rr-charts-in, acrescentada neste instante —
           em sincronia com o count-up dos números */
        document.body.classList.add("rr-charts-in");
        document.querySelectorAll("[data-rr-count]").forEach(countUp);
    }

    /* ---------- wizard do registo (3 passos + cavalo) ---------- */
    function initWizard() {
        var wiz = document.querySelector("[data-wizard]");
        if (!wiz || wiz.dataset.wired === "1") return;
        wiz.dataset.wired = "1";

        var steps = Array.prototype.slice.call(wiz.querySelectorAll("[data-wstep]"));
        if (!steps.length) return;

        /* Se o servidor devolveu erros de validação, mostramos os 3 passos
           de uma vez — para nenhum erro ficar escondido num passo fechado. */
        var hasErrors = Array.prototype.some.call(
            wiz.querySelectorAll(".validation-message"),
            function (m) { return m.textContent.trim().length > 0; }
        ) || !!wiz.querySelector(".alert-danger");

        var cur = 1;
        var stage = wiz.querySelector("[data-wstage]");

        function apply(dir) {
            steps.forEach(function (s) {
                var on = (hasErrors || parseInt(s.dataset.wstep, 10) === cur);
                s.style.display = on ? "" : "none";
                s.classList.remove("wiz-in-r", "wiz-in-l");
                if (on && dir) {
                    void s.offsetWidth; /* reinicia a animação */
                    s.classList.add(dir > 0 ? "wiz-in-r" : "wiz-in-l");
                }
            });
            if (stage) {
                stage.classList.remove("pos-1", "pos-2", "pos-3");
                stage.classList.add("pos-" + cur);
                if (dir) { /* salta a barreira durante a viagem */
                    stage.classList.remove("wiz-jumping");
                    void stage.offsetWidth;
                    stage.classList.add("wiz-jumping");
                    setTimeout(function () { stage.classList.remove("wiz-jumping"); }, 760);
                }
            }
            wiz.querySelectorAll("[data-wlabel]").forEach(function (l) {
                l.classList.toggle("on", parseInt(l.dataset.wlabel, 10) <= cur);
            });
        }

        wiz.addEventListener("click", function (e) {
            var next = e.target.closest(".wiz-next");
            var back = e.target.closest(".wiz-back");
            if (next) {
                /* validação leve: os obrigatórios do passo atual têm de estar preenchidos */
                var sec = steps.filter(function (s) { return parseInt(s.dataset.wstep, 10) === cur; })[0];
                var inputs = sec.querySelectorAll('input[aria-required="true"], select[aria-required="true"]');
                var bad = Array.prototype.filter.call(inputs, function (i) {
                    if (i.type === "checkbox") return !i.checked;
                    return !i.value || i.value === "0";
                })[0];
                if (bad) {
                    bad.focus();
                    bad.classList.add("wiz-shake");
                    setTimeout(function () { bad.classList.remove("wiz-shake"); }, 500);
                    return;
                }
                if (cur < steps.length) { cur++; apply(1); }
            }
            if (back && cur > 1) { cur--; apply(-1); }
        });

        apply();
    }

    function init() {
        scan();
        initWizard();
    }

    return { scan: scan, countUp: countUp, countAll: countAll, initWizard: initWizard, init: init };
})();

/* arranque normal */
document.addEventListener("DOMContentLoaded", function () { window.rrMotion.init(); });

/* navegação melhorada do Blazor (SSR): voltar a ligar quando a página troca */
window.addEventListener("load", function () {
    if (window.Blazor && window.Blazor.addEventListener) {
        window.Blazor.addEventListener("enhancedload", function () { window.rrMotion.init(); });
    }
});
