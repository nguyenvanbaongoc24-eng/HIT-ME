# HIT ME — Mobile UX & Online Fix

## Root causes and implementation
- Brown color was supplied by HTML body/loading and Safari theme styling, not a Unity gameplay requirement. Template now uses dark green, visualViewport dimensions, viewport-fit=cover and CSS safe-area measurements. Background covers/crops; interactive Unity canvases retain safe-area adapters.
- Socket-open was previously treated as Connected before hello was accepted. Connected now requires server welcome; profile routing also requires loaded profile.
- Public connection UI exposed a WSS address/manual Connect. Normal menu/lobby now auto-connect, display Vietnamese status and bounded retries. Bot practice stays available independently.
- Guest identity was tab-scoped custom SQLite auth. Browser now restores/refreshes a real Supabase anonymous session, or signs up a supported guest. Backend verifies bearer through Supabase /auth/v1/user and loads cosmetic profile under that user's RLS.
- Names/avatars are saved with authenticated RLS PATCH. Coins/XP are never client-writable; displayed rewards remain backend-authoritative. Existing SQLite profiles/ledgers remain intact; authenticated profiles use a separate Supabase identity namespace.
- Avatar opens the actual player profile with portrait, editable name, three canonical character choices and server balance. First-time guest is shown this profile flow.
- Quick Match formerly created a public room then required separate Ready. Public waiting rooms now form the server-side queue: idempotent join, cancel, reconnect, version check, real-player counts, configurable minimum 2 (max 6), automatic countdown. No bots enter online queues.
- Queue expiry is configurable, default 120 seconds; private invite-code expiry remains 30 minutes. No combat timing/rules were changed.
- Private entry presents Create/Join room-code actions, room roster, ready states, copy/share invitation. No endpoint input.

## Test status (update with final run)
- EditMode: PASS 83/83.
- PlayMode: PASS 39/39, final run 2026-10-10 04:42 UTC.
- Backend: PASS 27/27, no skipped tests.
- Actual two Supabase identities + local server Quick Match: PASS. See MOBILE_ONLINE_LOCAL_PROBE.json.
- Fresh Web Build: PASS, 0 errors / 4 warnings, 160.7 seconds. Local HTTP: PASS index and hitme-session.js (200). Local actual UI: PASS auto authentication, profile nickname/avatar save and refresh restore, real one-player Quick queue and Cancel. Public deployment pending.
- Physical iPhone, Safari/Zalo browser chrome, Dynamic Island touch and native share: NOT_TESTED, pending physical verification.

## Limits
- Supabase persists identity, nickname and avatar. Server coins/history still use existing SQLite. Render free disk durability and reward synchronization to Supabase are NOT VERIFIED; do not claim permanent cloud economy.
- Anonymous account upgrade/email/Google UI is not supported in this sprint.
- Only three canonical avatars are playable. Gallery animals/extra characters are not mislabeled as playable/rigged.
- Prefab definitions are reused; this sprint changes runtime UI/controllers/bridges, not independent gameplay controllers.
- No Phaser/TypeScript prototype source, combat resolver, bot policy or production schema was rewritten.

