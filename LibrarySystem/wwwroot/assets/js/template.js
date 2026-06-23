document.addEventListener("DOMContentLoaded", function () {
    var toggle = document.querySelector("[data-menu-toggle]");
    var overlay = document.querySelector(".sidebar-overlay");
    var themeToggle = document.querySelector("[data-theme-toggle]");
    var themeIcon = document.querySelector("[data-theme-icon]");

    function applyTheme(theme) {
        document.documentElement.setAttribute("data-bs-theme", theme);
        document.body.classList.toggle("theme-dark", theme === "dark");

        if (themeIcon) {
            themeIcon.className = theme === "dark" ? "bi bi-sun" : "bi bi-moon-stars";
        }
    }

    var storedTheme = localStorage.getItem("library-theme") || "light";
    applyTheme(storedTheme);

    if (themeToggle) {
        themeToggle.addEventListener("click", function () {
            var nextTheme = document.documentElement.getAttribute("data-bs-theme") === "dark" ? "light" : "dark";
            localStorage.setItem("library-theme", nextTheme);
            applyTheme(nextTheme);
        });
    }

    function setSidebarOpen(open) {
        document.body.classList.toggle("sidebar-open", open);
    }

    if (toggle) {
        toggle.addEventListener("click", function () {
            setSidebarOpen(!document.body.classList.contains("sidebar-open"));
        });
    }

    if (overlay) {
        overlay.addEventListener("click", function () {
            setSidebarOpen(false);
        });
    }

    document.querySelectorAll(".side-link").forEach(function (link) {
        link.addEventListener("click", function () {
            if (window.matchMedia("(max-width: 991.98px)").matches) {
                setSidebarOpen(false);
            }
        });
    });

    document.querySelectorAll("[data-demo-autocomplete]").forEach(function (input) {
        var menu = document.querySelector(input.dataset.demoAutocomplete);
        if (!menu) {
            return;
        }

        var values = (input.dataset.values || "").split("|").filter(Boolean);

        input.addEventListener("input", function () {
            var term = input.value.trim().toLowerCase();
            menu.innerHTML = "";

            if (term.length < 2) {
                menu.style.display = "none";
                return;
            }

            var matches = values.filter(function (value) {
                return value.toLowerCase().includes(term);
            }).slice(0, 6);

            matches.forEach(function (match) {
                var item = document.createElement("button");
                item.type = "button";
                item.className = "autocomplete-item";
                item.textContent = match;
                item.addEventListener("click", function () {
                    input.value = match;
                    menu.style.display = "none";
                });
                menu.appendChild(item);
            });

            menu.style.display = matches.length ? "block" : "none";
        });

        document.addEventListener("click", function (event) {
            if (!input.contains(event.target) && !menu.contains(event.target)) {
                menu.style.display = "none";
            }
        });
    });

    // Stat card modal behavior
    var statModalEl = document.getElementById('statDetailModal');
    var statModal = statModalEl ? new bootstrap.Modal(statModalEl) : null;
    var statDetailTitle = document.getElementById('statDetailModalLabel');
    var statDetailDesc = document.getElementById('statDetailDescription');
    var statDetailLink = document.getElementById('statDetailOpenLink');

    document.querySelectorAll('.stat-link').forEach(function (el) {
        el.addEventListener('click', function (e) {
            // If element has data-modal, show modal. Otherwise let it behave like a link.
            var modalKey = el.getAttribute('data-modal');
            var url = el.getAttribute('data-url') || '#';
            var title = el.getAttribute('data-title') || 'Details';
            var desc = el.getAttribute('data-desc') || '';
            if (modalKey && statModal) {
                e.preventDefault();
                if (statDetailTitle) statDetailTitle.textContent = title;
                if (statDetailDesc) statDetailDesc.textContent = desc;
                if (statDetailLink) statDetailLink.setAttribute('href', url);
                statModal.show();
            }
        });
    });
});
