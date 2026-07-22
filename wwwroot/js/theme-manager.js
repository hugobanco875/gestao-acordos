(function () {
    const storageKey = 'gestao-acordos-theme-mode';
    const allowedThemes = new Set(['claro', 'escuro']);
    let listenersRegistered = false;

    function normalize(theme) {
        return allowedThemes.has(theme) ? theme : 'claro';
    }

    function readStoredTheme() {
        try {
            const stored = localStorage.getItem(storageKey);
            return allowedThemes.has(stored) ? stored : null;
        } catch {
            return null;
        }
    }

    function saveTheme(theme) {
        try { localStorage.setItem(storageKey, theme); } catch { }
    }

    function applyTheme(theme) {
        const normalized = normalize(theme);
        const dark = normalized === 'escuro';
        const root = document.documentElement;

        root.setAttribute('data-theme', normalized);
        root.setAttribute('data-bs-theme', dark ? 'dark' : 'light');
        root.style.colorScheme = dark ? 'dark' : 'light';

        if (document.body) {
            document.body.setAttribute('data-theme', normalized);
            document.body.classList.toggle('theme-dark', dark);
            document.body.classList.toggle('theme-light', !dark);
        }

        const meta = document.querySelector('meta[name="theme-color"]');
        if (meta) meta.setAttribute('content', dark ? '#0d111b' : '#f6f7fb');

        return normalized;
    }

    function refresh() {
        return applyTheme(readStoredTheme() || 'claro');
    }

    function registerListeners() {
        if (listenersRegistered) return;

        // A preferência salva no dispositivo é sempre reaplicada depois de qualquer navegação.
        document.addEventListener('enhancedload', refresh);
        window.addEventListener('pageshow', refresh);
        window.addEventListener('popstate', refresh);
        window.addEventListener('focus', refresh);
        document.addEventListener('visibilitychange', function () {
            if (!document.hidden) refresh();
        });
        window.addEventListener('storage', function (event) {
            if (event.key === storageKey) refresh();
        });

        listenersRegistered = true;
    }

    window.themeManager = {
        // Usado pelos layouts. Não sobrescreve a preferência já salva no celular.
        initialize: function (serverTheme) {
            registerListeners();
            const stored = readStoredTheme();
            const selected = stored || normalize(serverTheme);
            if (!stored) saveTheme(selected);
            return applyTheme(selected);
        },

        // Usado somente quando o usuário salva explicitamente a opção de tema.
        setPreference: function (theme) {
            registerListeners();
            const selected = normalize(theme);
            saveTheme(selected);
            return applyTheme(selected);
        },

        refresh: function () {
            registerListeners();
            return refresh();
        },

        initializeEarly: function () {
            registerListeners();
            return refresh();
        }
    };
})();
