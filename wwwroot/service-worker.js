// Service worker mínimo: torna o app instalável (PWA). Sem cache offline
// porque o Blazor Server precisa de conexão com o servidor.
self.addEventListener('install', event => self.skipWaiting());
self.addEventListener('activate', event => self.clients.claim());
self.addEventListener('fetch', event => { /* repassa para a rede */ });
