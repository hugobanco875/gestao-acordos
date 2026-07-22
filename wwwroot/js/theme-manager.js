(function () {
    const storageKey = 'gestao-acordos-theme-mode';
    const darkQuery = window.matchMedia('(prefers-color-scheme: dark)');
    let configuredMode = 'automatico';
    let listening = false;
    let applying = false;

    function normalize(mode) {
        return mode === 'claro' || mode === 'escuro' || mode === 'automatico'
            ? mode
            : 'automatico';
    }

    function readSavedMode() {
        try { return normalize(localStorage.getItem(storageKey) || configuredMode); }
        catch { return normalize(configuredMode); }
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
        applying = true;
        try {
            configuredMode = normalize(mode);
            const theme = resolvedTheme(configuredMode);
            const root = document.documentElement;

            root.setAttribute('data-theme', theme);
            root.setAttribute('data-theme-mode', configuredMode);
            updateBrowserChrome(theme);

            if (persist !== false) {
                try { localStorage.setItem(storageKey, configuredMode); } catch { }
            }

            return theme;
        } finally {
            applying = false;
        }
    }

    function refreshFromPreference() {
        return apply(readSavedMode(), false);
    }

    function onSystemThemeChanged() {
        configuredMode = readSavedMode();
        if (configuredMode === 'automatico') apply('automatico', false);
    }

    function ensureListener() {
        if (listening) return;

        if (darkQuery.addEventListener) darkQuery.addEventListener('change', onSystemThemeChanged);
        else if (darkQuery.addListener) darkQuery.addListener(onSystemThemeChanged);

        // O Blazor pode atualizar partes do documento durante a navegação aprimorada.
        // Reaplicamos o tema após cada troca de rota para impedir o retorno ao modo claro.
        document.addEventListener('enhancedload', refreshFromPreference);
        window.addEventListener('pageshow', refreshFromPreference);
        window.addEventListener('popstate', refreshFromPreference);
        window.addEventListener('focus', refreshFromPreference);
        document.addEventListener('visibilitychange', function () {
            if (!document.hidden) refreshFromPreference();
        });
        window.addEventListener('storage', function (event) {
            if (event.key === storageKey) refreshFromPreference();
        });

        // Protege os atributos do <html> caso algum carregamento de rota os remova.
        const observer = new MutationObserver(function (mutations) {
            if (applying) return;
            const lostTheme = mutations.some(function (mutation) {
                return mutation.type === 'attributes' &&
                    (mutation.attributeName === 'data-theme' ||
                     mutation.attributeName === 'data-theme-mode' ||
                     mutation.attributeName === 'data-bs-theme');
            });
            if (lostTheme) queueMicrotask(refreshFromPreference);
        });
        observer.observe(document.documentElement, {
            attributes: true,
            attributeFilter: ['data-theme', 'data-theme-mode', 'data-bs-theme']
        });

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
            ensureListener();
            return refreshFromPreference();
        },
        initializeEarly: function () {
            ensureListener();
            return refreshFromPreference();
        }
    };
})();
