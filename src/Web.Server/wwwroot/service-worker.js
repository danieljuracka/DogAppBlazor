// Service worker pre inštaláciu aplikácie (PWA).
// Aplikácia potrebuje server (prerendering, gRPC), preto sa obsah necachuje -
// pri výpadku siete sa namiesto stránky zobrazí len offline.html.

const CACHE_NAME = 'dogapp-offline-v1';
const OFFLINE_URL = 'offline.html';

self.addEventListener('install', event => {
	event.waitUntil(
		caches.open(CACHE_NAME)
			.then(cache => cache.addAll([OFFLINE_URL, 'icons/icon-192.png']))
			.then(() => self.skipWaiting()));
});

self.addEventListener('activate', event => {
	event.waitUntil(
		caches.keys()
			.then(keys => Promise.all(keys.filter(key => key !== CACHE_NAME).map(key => caches.delete(key))))
			.then(() => self.clients.claim()));
});

self.addEventListener('fetch', event => {
	if (event.request.mode !== 'navigate') {
		return;
	}

	event.respondWith(
		fetch(event.request).catch(() => caches.match(OFFLINE_URL)));
});
