(function () {
    const storageKey = 'gestao-acordos-theme-mode';
    const darkQuery = window.matchMedia('(prefers-color-scheme: dark)');
    let configuredMode = 'automatico';
    let listening = false;

    function normalize(mode) {
        return mode === 'claro' || mode === 'escuro' || mode === 'automatico'
            ? mode
            : 'automatico';
    }

    function resolvedTheme(mode) {
        return mode === 'automatico'
            ? (darkQuery.matches ? 'escuro' : 'claro')
            : mode;
    }

    function updateBrowserChrome(theme) {
        const dark = theme === 'escuro';
        document.documentElement.style.colorScheme = dark ? 'dark' : 'light';
        document.documentElement.setAttribute('data-bs-theme', dark ? 'dark' : 'light');

        const meta = document.querySelector('meta[name="theme-color"]');
        if (meta) meta.setAttribute('content', dark ? '#0d111b' : '#f6f7fb');
    }

    function apply(mode, persist) {
        configuredMode = normalize(mode);
        const theme = resolvedTheme(configuredMode);

        document.documentElement.setAttribute('data-theme', theme);
        document.documentElement.setAttribute('data-theme-mode', configuredMode);
        updateBrowserChrome(theme);

        if (persist !== false) {
            try { localStorage.setItem(storageKey, configuredMode); } catch { }
        }

        return theme;
    }

    function onSystemThemeChanged() {
        if (configuredMode === 'automatico') apply('automatico', false);
    }

    function ensureListener() {
        if (listening) return;
        if (darkQuery.addEventListener) darkQuery.addEventListener('change', onSystemThemeChanged);
        else if (darkQuery.addListener) darkQuery.addListener(onSystemThemeChanged);
        listening = true;
    }

    window.themeManager = {
        initialize: function (mode) {
            ensureListener();
            return apply(mode, true);
        },
        setPreference: function (mode) {
            ensureListener();
            return apply(mode, true);
        },
        refresh: function () {
            return apply(configuredMode, false);
        },
        initializeEarly: function () {
            let saved = 'automatico';
            try { saved = localStorage.getItem(storageKey) || 'automatico'; } catch { }
            ensureListener();
            return apply(saved, false);
        }
    };
})();
