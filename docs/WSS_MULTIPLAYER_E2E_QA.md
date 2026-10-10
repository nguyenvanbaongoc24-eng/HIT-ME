# Public WSS Multiplayer E2E QA

2026-10-10. Public guest protocol fixture **PASS**; Supabase-authenticated browser E2E **FAIL** (missing identity integration).

Endpoint discovered from current project connectivity report and tested: `wss://hit-me-zj17.onrender.com/play`, frontend Origin `https://hit-me-game.vercel.app`. Node ws used normal certificate validation. Evidence: `STRICT_PUBLIC_SERVICE_PROBE.json`, source `multiplayer-server/scripts/strict-production-probe.ts`.

Separate fixtures with two and six independent HIT ME guest sessions created private rooms, joined, synchronized ready, sent placement/aim/lock actions over three rounds and finished Draw. Unique identities, placement privacy after first lock, identical authoritative final resolution, zero protocol errors and one history entry/client were verified. Trial payout observed65 each; beta is disabled. Dedicated QA sessions left rooms and closed sockets. No live service restart performed.

These clients are actual public WebSockets, not localhost, but are protocol clients rather than six browser-rendered Unity instances. No Supabase account is used by this protocol. Browser avatar rendering, refresh/reconnect, rematch, explicit full-room/unauthorized/duplicate-join failures, injected delay, expired JWT, durable history across Render restart and published server/client version are **NOT_TESTED** on public service.

Focused backend local regression suite covers identity scoping, client reward rejection, duplicate event handling, combat parity, private-room cap and reconnect scenarios. These local results must not be represented as public browser verification. Render free-plan SQLite has no persistent disk in checked-in deployment config; long-term storage across restart is **BLOCKED** pending persistent storage/runtime adapter.
