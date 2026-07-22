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

        if (root.getAttribute('data-theme') !== normalized)
            root.setAttribute('data-theme', normalized);
        const bootstrapTheme = dark ? 'dark' : 'light';
        if (root.getAttribute('data-bs-theme') !== bootstrapTheme)
            root.setAttribute('data-bs-theme', bootstrapTheme);
        root.style.colorScheme = dark ? 'dark' : 'light';

        if (document.body) {
            if (document.body.getAttribute('data-theme') !== normalized)
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

        // Reaplica apenas quando o Blazor conclui uma navegação aprimorada ou
        // quando outra aba altera explicitamente a preferência. Eventos de foco,
        // visibilidade e histórico foram removidos porque disparavam repinturas
        // frequentes e davam a impressão de refresh automático.
        document.addEventListener('enhancedload', refresh);
        window.addEventListener('storage', function (event) {
            if (event.key === storageKey) refresh();
        });
        document.addEventListener('DOMContentLoaded', refresh, { once: true });

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
