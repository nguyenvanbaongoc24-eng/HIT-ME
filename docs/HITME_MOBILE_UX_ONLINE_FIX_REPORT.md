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


## Runtime verification
- PASS: local and public nickname/character save and refresh restore; public auto authentication, Quick searching real 1-player room and Cancel.
- PASS: canvas origin (0,0), exact viewport coverage at 360x800, 390x844, 393x852, 402x874, 430x932. See MOBILE_UX_VIEWPORTS.json and screenshot folder MobileOnline-20261010. Device safe-area hardware remains NOT_TESTED.
- PASS: real two-account public Render probe (MOBILE_ONLINE_PUBLIC_PROBE.json); this is protocol/REST evidence, not two isolated browser UIs.
- PASS: PlayMode 39/39 after clipboard bridge, 2026-10-10 05:00 UTC. An intermediate run failed because an old screenshot path was locked; Capture now writes per-run paths. One earlier attempt without HITME_LIVE_TEST_URL was ignored, not counted as a pass.
- Existing prefabs: no prefab assets changed in this sprint. Runtime UI reuses Independent UI Kit factory and existing character definitions.
- Git runtime release: main 50b5a9e; Vercel served matching data/wasm/session JS. HTML differed only by CRLF checkout normalization. Clipboard correction is being rebuilt separately.


## Changed files (runtime release)
- docs/HITME_CHARACTER_RUNTIME_ASSET_AUDIT.md
- docs/HITME_MOBILE_UX_ONLINE_FIX_REPORT.md
- docs/HITME_QUICK_MATCH_E2E_TEST.md
- docs/MOBILE_ONLINE_LOCAL_PROBE.json
- docs/MOBILE_UX_EDITMODE.xml
- docs/MOBILE_UX_PLAYMODE.xml
- docs/screenshots/MobileOnline-20261010/Local-ProfileSaved-390x844.png
- docs/screenshots/MobileOnline-20261010/Local-Queue-390x844.png
- multiplayer-server/scripts/mobile-online-probe.ts
- multiplayer-server/src/main.ts
- multiplayer-server/src/rooms.ts
- multiplayer-server/src/store.ts
- multiplayer-server/src/supabase-auth.ts
- multiplayer-server/tests/quick-auth.test.ts
- multiplayer-server/tests/web-session.test.ts
- unity-client/Assets/HitMe/Plugins/WebGL/HitMeMobile.jslib
- unity-client/Assets/HitMe/Plugins/WebGL/HitMeNetwork.jslib
- unity-client/Assets/HitMe/Resources/Localization/en.json
- unity-client/Assets/HitMe/Resources/Localization/vi.json
- unity-client/Assets/HitMe/Scripts/UI/MainMenu/MainMenuController.cs
- unity-client/Assets/HitMe/Scripts/UI/MainMenu/MainMenuView.Profile.cs
- unity-client/Assets/HitMe/Scripts/UI/MainMenu/MainMenuView.Profile.cs.meta
- unity-client/Assets/HitMe/Scripts/UI/MainMenu/MainMenuView.cs
- unity-client/Assets/HitMe/Scripts/UI/NetworkLobbyView.cs
- unity-client/Assets/HitMe/Scripts/UI/NetworkSession.cs
- unity-client/Assets/HitMe/Scripts/UI/WebMobileBridge.cs
- unity-client/Assets/HitMe/Tests/PlayMode/MainMenuIntegrationTests.cs
- unity-client/Assets/HitMe/Tests/PlayMode/MobileOnlineUxTests.cs
- unity-client/Assets/HitMe/Tests/PlayMode/MobileOnlineUxTests.cs.meta
- unity-client/Assets/HitMe/Tests/PlayMode/NetworkPresentationTests.cs
- unity-client/Assets/HitMe/Tests/PlayMode/Phase1MenuFlowTests.cs
- unity-client/Assets/HitMe/Tests/PlayMode/Phase2APolishTests.cs
- unity-client/Assets/WebGLTemplates/HitMePortrait/hitme-session.js
- unity-client/Assets/WebGLTemplates/HitMePortrait/hitme-session.js.meta
- unity-client/Assets/WebGLTemplates/HitMePortrait/index.html
- vercel.json
- web-release/Build/Web.data.unityweb
- web-release/Build/Web.framework.js.unityweb
- web-release/Build/Web.loader.js
- web-release/Build/Web.wasm.unityweb
- web-release/hitme-session.js
- web-release/index.html

## Final release checks
- Runtime commit main 0d634b3: Vercel production Ready (screenshot Vercel-0d634b3-Ready.png), actual public data/wasm/framework/session.js match hashes; index equals after CRLF normalization.
- Final Unity build: 0 errors, 4 warnings, 156.14 seconds (MOBILE_UX_WEB_BUILD.json).
- Clipboard: local Web verified exact room code 9E8A2E22. Public verification initially retained old clipboard value; NOT PASS yet. HTML loader now disables Unity's internal IndexedDB build cache to force current assembly/framework during release validation. Tradeoff: full build download on each load until versioned asset caching is added.
- Public Console: no captured errors; UnityCache warns that CDN responses omit Content-Length. No measured mobile FPS claim.
- Native share and physical iPhone Safari/Zalo/Dynamic Island: NOT_TESTED.
- Server rooms live in memory and can disappear during Render redeploy. Coins/history use SQLite; persistent cloud economy remains NOT VERIFIED.

