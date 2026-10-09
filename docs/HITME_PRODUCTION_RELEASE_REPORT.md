# HIT ME — Production release gate report

2026-10-09. Overall release status: **BLOCKED**. Verified local improvements are ready for source delivery; this is not a production launch approval.

## Baseline and scope

`RELEASE_BASELINE.md` records the pre-edit executions and preserved commit/branch. Existing Unity project and UI Kit reused. No changes to Phaser `apps/` or `packages/`, combat math, collision geometry, bot decisions, placement privacy or throw intent semantics. Picture and SFX originals unchanged. No concept board imported as an animation sheet, floor, HUD or result screen.

## Gate matrix

| Gate / feature | Status | Evidence and limits |
|---|---|---|
| Existing Unity baseline | PASS | Unity 6000.6.4f1; fresh baseline 83 EditMode, 32 PlayMode, 16 backend tests |
| Viewport implementation | PASS | Removed Web shell max-width/centering; fixed viewport surface follows visualViewport resize/scroll; safe-area intersection updated |
| Desktop browser mobile viewport checks | PASS | HTTP localhost; final canvas bounds start at (0,0); Main Menu/Battle responsive evidence below |
| Physical iPhone 15 / Safari / Zalo | NOT_TESTED | No actual device or those browser environments available |
| Production character/animal frame animation | BLOCKED | Real independent frame sequences/rig layers unavailable; existing single-frame code motion remains |
| Complete Picture character/weapon replacement | BLOCKED | Canonical third character remains legacy artwork; pink gallery sprite has baked racket, not a clean interchangeable weapon rig; gallery items are not new gameplay/online entitlements |
| Local authenticated guest profile editing | PASS | Server session determines identity; validated nickname + canonical character only; immutable ID/currency/inventory; WS tests + Unity profile/selection/lobby flow |
| Supabase anonymous Auth/profile adapter | BLOCKED | Auth settings HTTP 401: Unregistered API key; provided key not registered for supplied project |
| Live Supabase schema/data/RLS isolation | NOT_TESTED | Cannot inspect with rejected key; no Supabase migration or admin operation applied |
| Local multiplayer regression | PASS | Real WS match test, privacy/action validation, reconnect, 2/3/6 Rooms tests and test-CA WSS handshake |
| Public cross-device multiplayer | BLOCKED | Public WSS host, TLS and deployment access missing. Local tests are not public proof |
| Lobby portraits and room-code copy | PASS | Reused UI Kit, canonical portraits, names/owner/Ready, six bounded roster rows; Unity actual-server profile test |
| Versioned existing trial rewards | PASS | trial-v1 recorded in idempotent SQLite ledger; additive legacy-schema upgrade and duplicate/reopen preservation tested |
| New beta reward production activation | BLOCKED | beta-v1 recorded disabled; daily cap, tie/draw and elimination-credit rules unresolved; existing amounts preserved |
| Temporary WAV import and audio settings | PASS | 20 imported clips; master/SFX/music sliders + mute saved; resource references/settings/dedup tested |
| Audio production quality / device listening | NOT_TESTED | Source pack explicitly synthetic placeholders; no auditory quality certification or iOS gesture/resume verification |
| Match statistics counters | PASS | Throws/hits/misses/received from confirmed resolver results; backend conservation/dedup tests; offline result screenshot |
| Complete production Match Result/podium/ranking | BLOCKED | Clean result art and shared-rank/elimination-credit policy missing; no fabricated rank or reward breakdown |
| Unity EditMode / PlayMode | PASS | Final 83/83 and 34/34, no skipped tests |
| Backend build / tests | PASS | TypeScript build; 21/21 tests |
| Phaser regression | PASS | Existing root npm test: 9/9; source unchanged |
| Unity Web Build | PASS | Fresh BuildPipeline result Succeeded, 0 errors, 1 warning, 40,184,481 bytes |
| Public Vercel deployment | BLOCKED | Requested alias returned HTTP 404; connected account does not expose target HIT ME project |
| Production Supabase persistence/reward isolation | NOT_TESTED | SQLite development store remains active; cannot certify Supabase or production economy |
| Physical-device FPS, GC and audio latency | NOT_TESTED | No profiling assertion based on targetFrameRate or a desktop counter |

## Implemented source changes

Exact created/modified paths are listed in `RELEASE_CHANGED_FILES.json`; the staged source/package review is recorded in `RELEASE_STAGED_AUDIT.json`. Generated historical screenshots are preserved; current screenshots are in separate run directories.

- Web template fills the actual viewport, with no max-width brown gutters. Browser resize changes presentation only; arena math remains unchanged.
- Shared NetworkEndpointSettings removes the localhost fallback on public sites and rejects invalid/insecure production endpoints. A real public WSS address still must be configured.
- Server validates nicknames using trim/NFC, 1–24 Unicode code points, rejecting control/format characters and HTML delimiters. Keeps existing one-character compatibility. Only the three existing starter IDs are accepted; arbitrary gallery/premium IDs rejected.
- Authenticated profile command persists name/character in the current SQLite store, refreshes lobby member data, and rejects changes after Waiting. Client-supplied IDs, coins, XP and inventory fields cannot change ownership or balances.
- Character Selection keeps card selection provisional; Confirm saves offline cosmetic choice or submits the authenticated profile command. Online display follows the server profile, not a client reward/state assumption.
- Lobby renders portraits and names, updates its fingerprint when profile presentation changes, and exposes Copy room code. Existing all-ready auto-start behavior preserved; no new host-start rule inferred.
- Server accumulates confirmed throw/hit/miss/received counters, publishes them at MatchResult only. Offline presentation accumulates the same categories without changing OfflineMatch. Elimination credit/rank tie rules are not inferred. Server result UI shows missing reward as a dash, not a fabricated zero grant.
- Reused OfflineResultView adds actual counters, readable existing clean background and explicit no-permanent-offline-rewards notice. The composite Match Result.png remains reference-only; full podium/stat styling is not completed.
- Existing trial payout values unchanged: win 100/50 XP, lose 40/20, draw 65/30; private trial cap unchanged. trial-v1 ledger metadata added safely; beta-v1 proposal (+20 participation, +30/+15/+10 rank, +5 elimination) remains disabled with unresolved rules null.
- Current local SQLite database backed up under ignored data/backups before the additive ledger upgrade. No Supabase database modified.

## Imported audio and actual bindings

All 20 original WAV files are copied byte-for-byte into `Assets/HitMe/Resources/Audio/` and imported by Unity. Imported does not mean every clip has a runtime binding.

Active bindings: UI click, Ready, round Reveal, Throw, confirmed hit/wall miss, elimination, win/lose, positive server-confirmed coin grant; menu/match music starts after pointer/touch input. Eight reused AudioSources bound concurrent SFX; network event IDs deduplicated with a bounded 256-entry cache. No audio changes combat or opponent visibility.

Missing dedicated assets/bindings: lobby join, separate confirmation/aim-lock, weapon travel/weapon-specific variants, draw, reward reveal. No unrelated clip substituted. Chat/countdown/ambience/purchase/heart-break clips are imported but not newly bound in this release. iOS interruption/resume and actual audible playback quality remain NOT_TESTED. The available audio-input tool did not support listening; QA used actual PCM measurements, the manifest and generator inspection.

`RELEASE_AUDIO_QA.txt`: 20 WAV files, zero missing files or technical QA issues; this is not a subjective sound-quality PASS.

## Artwork gaps

All nine Picture files were inspected. The concept/UI/character/result boards have baked text, multiple poses, examples or HUD; they remain references. The Northern Village/Hoi An JPEGs are flattened illustrations, not separated animated environments. Reuse existing clean runtime art and production manifests; original files preserved.

Needed for full visual gate:

1. Clean primary pink-character body without a baked weapon, matching portrait and foot-pivot metadata; matching black-boy and military-boy exports at production resolution.
2. Independent Idle/Aim/Throw anticipation-release-follow-through/Hit/Eliminated/Victory frames or actual separated rig layers for each playable character; equivalent dog/cat locomotion/tail/jump sets. Existing code bobs are not authored frame animation.
3. Standalone matching handheld/projectile/impact assets and approved gameplay catalog/entitlements for phone, mosquito racket and pickleball additions. Current visual galleries do not grant new online inventory.
4. Clean result podium, medals, paper table and outcome variants without baked example players/currency/HUD.
5. Separate map motion layers and true ambient/weapon FX frames where the existing manifest marks gaps.

`RELEASE_SOURCE_AUDIT.json` contains current filenames/dimensions/audio headers. Earlier production manifests describe existing derived art; their older counts are not a fresh audit count.

## Execution evidence

- `RELEASE_EDITMODE_RESULTS.xml`: 83/83.
- `RELEASE_PLAYMODE_RESULTS.xml`: 34/34; actual local profile rename, Confirm selection, lobby portrait; existing Sprint 1/2/aim/privacy/layout checks retained.
- `RELEASE_BACKEND_TEST_RESULTS.txt`: 21/21, including real WS profile ownership and match-state rejection, versioned duplicate ledger and additive legacy migration.
- `PRODUCTION_RELEASE_WEB_BUILD.json`: Succeeded, 21.6730207 seconds, 40,184,481 bytes, 0 errors, 1 TMP shader deprecation warning. One earlier build was replaced after visual QA found a missing translation key.
- `DEPLOYMENT_PACKAGE_CHECK.json`: all five published Web package files match the fresh Unity output by SHA-256.
- `RELEASE_CONNECTIVITY_CHECK.json`: local health and actual public HTTP failures.

Intermediate failed checks were corrected: old instant-selection assertion updated for required confirmation; batch-mode WaitForEndOfFrame replaced; screenshot writes moved to run-specific directories after Windows locked a historical file; missing offline reward localization key fixed and all new VI/EN keys asserted. Final counts above are rerun results, not baseline counts reused as final proof.

Actual Unity screenshots:

- `screenshots/Release-Run-20261009-101918/Release-Audio-Settings.png`
- `screenshots/Release-Run-20261009-101918/Release-Lobby-Profile.png`
- `screenshots/Release-Run-20261009-101918/Offline-Result-390x844.png` — offline real combat counters, not production podium.
- `screenshots/Northern-Run-20261009-101859/` — actual responsive idle/aim/combat frames; existing multi-character reveal screenshots show legal revealed state, not opponent placement leakage.
- `screenshots/Release-Run-20261009-101918/Unity-Menu-SafeArea-*` — actual Unity menu safe-area checks, distinct from browser shell verification.

## Delivery and required access

Open the existing Unity project in Unity Hub, then Battle/MainMenu scenes under `Assets/HitMe/Scenes/`. Local Web: `http://127.0.0.1:8791/`. Rebuild with `tools/build-production-release.ps1`; run tests with `tools/unity.ps1 -Action EditMode` / `PlayMode` and `npm test` in multiplayer-server. Live PlayMode requires HITME_LIVE_TEST_URL pointing to the actual local backend.

Production remains blocked until:

- Correct Supabase Project URL/public key for the same project; revoke the secret exposed in chat and put a replacement directly on the backend host. Then inspect live schema/data, anonymous-auth setting and RLS before any reviewed migration/adapter integration.
- Access to the Vercel account/team/project owning HIT ME and the requested alias.
- Persistent public backend host/deployment credentials, public WSS URL and valid TLS/origin configuration; verify two devices end-to-end, then 3–6 participants.
- Confirmed tie/draw/elimination-credit/daily-cap rules and missing artwork/audio exports.

No public multiplayer completion, Supabase persistence, physical-device compatibility, production visual completion or full production release is claimed.
