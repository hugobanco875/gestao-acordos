// Esta aplicação usa Blazor Server e não mantém arquivos em cache offline.
// Ao receber esta versão, qualquer cache/PWA antigo é removido.
self.addEventListener('install', event => self.skipWaiting());
self.addEventListener('activate', event => {
    event.waitUntil((async () => {
        const keys = await caches.keys();
        await Promise.all(keys.map(key => caches.delete(key)));
        await self.registration.unregister();
        await self.clients.claim();
    })());
});
self.addEventListener('fetch', () => {});
