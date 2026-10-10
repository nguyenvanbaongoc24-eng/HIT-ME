# HIT ME — Quick Match E2E

## Local authoritative flow — PASS
MOBILE_ONLINE_LOCAL_PROBE.json records two distinct real Supabase anonymous identities, separate authenticated WebSocket clients, different saved names/avatars, duplicate Quick prevention, Cancel, same server-assigned public room, automatic start, three mutually hitting rounds, identical Draw resolution, private placement actions, one ledger result each, and refresh restoring identity/profile. No offline bots were used.

## Public flow — PASS protocol/REST
The same probe supports HITME_PROBE_ENDPOINT=wss://hit-me-zj17.onrender.com/play and writes a redacted summary. Tokens remain only in memory. MOBILE_ONLINE_PUBLIC_PROBE.json records PASS on actual Render public WSS: two distinct Supabase users, matching room, three rounds, same Draw, one server ledger per user and refreshed identity/profile. Render health reports mobile-ux-v2, authConfigured=true, authRequired=true, minimumPlayers=2. Public UI separately verifies profile save/refresh and Quick/Cancel.

## Two public browser/device interfaces — BLOCKED by available browser surfaces
Automation exposes one IAB browser profile with shared same-origin storage, and MCP Apps. It cannot supply two isolated Safari/iPhone profiles. Protocol evidence is not labeled as two physical-user UI acceptance. A single public browser profile/queue/room smoke will be captured after deploy; two-device complete UI acceptance remains NOT_TESTED.

## Negative checks
Backend tests cover queue duplicate/cancel/disconnect/reconnect, invalid identity, legacy balance preservation, incompatible version handling in implementation, and queue expiry. Browser-session tests preserve identity on transport failure and renew/create guest only after invalid refresh. Unity tests require real welcome before Connected, bounded reconnect failure, and avatar/profile UI without endpoint controls.
