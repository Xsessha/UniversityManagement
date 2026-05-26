(function () {
    function setTheme(theme) {
        document.documentElement.classList.toggle("dark", theme === "dark");
        try {
            localStorage.setItem("lumina-theme", theme);
        } catch (e) {
        }
    }

    function closeSidebar() {
        document.body.classList.remove("lumina-sidebar-open");
    }

    document.addEventListener("click", function (event) {
        var themeToggle = event.target.closest("[data-theme-toggle]");
        if (themeToggle) {
            var isDark = document.documentElement.classList.contains("dark");
            setTheme(isDark ? "light" : "dark");
        }

        if (event.target.closest("[data-lumina-toggle-sidebar]")) {
            document.body.classList.add("lumina-sidebar-open");
        }

        if (event.target.closest("[data-lumina-close-sidebar]")) {
            closeSidebar();
        }
    });

    document.addEventListener("keydown", function (event) {
        if (event.key === "Escape") {
            closeSidebar();
        }
    });

    window.addEventListener("resize", function () {
        if (window.innerWidth >= 992) {
            closeSidebar();
        }
    });

    if (window.lucide && typeof window.lucide.createIcons === "function") {
        window.lucide.createIcons();
    }
})();
