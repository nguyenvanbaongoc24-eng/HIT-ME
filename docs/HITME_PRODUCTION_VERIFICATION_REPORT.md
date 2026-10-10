# HIT ME Production Verification Report

2026-10-10. Overall **BLOCKED**. The production definition of done is not met: active art differs from Picture, frame/rig animation is missing, and Unity/WSS are not bound to Supabase identities.

| Area | Status | Evidence | Issue | Fix |
|---|---|---|---|---|
| Character artwork | FAIL | STRICT_CHARACTER_ASSET_INVENTORY.json; Picture-vs-Runtime-Sprites.png; active Battle frames | Brown girl versus pink reference; legacy military boy versus current roster; gallery not playable | Missing clean art recorded; no crude crops |
| Character portraits | FAIL | Public CharacterSelection-Before.png; catalog portrait references | Brown-haired starter differs from current pink concept; menu feature-card pink art differs from selectable starter | Reference mismatch documented |
| Idle animation | PASS | 7 actual timestamped captures >3s, fixed feet and moving child assertions | Type C transform motion, no authored frames | No fabricated clips |
| Aim animation | PASS | Real Core aim/lock and captured runtime state | Type C; lock shares Aim pose | Preserved gameplay direction |
| Throw animation | PASS | Core Throw phase assertion, actual Throw/FollowThrough frames | Type C; no authored hand release frame | Pooled visual implementation retained |
| Hit animation | PASS | Real damage resolution/state assertions and Hit frames | Type C shake/tint | No animation-derived damage |
| Victory animation | PASS | Final winner/Eliminated states captured in Battle | Type C bounce/tilt | No fake frame sequence |
| Supabase Auth API | PASS | Two independent anonymous bearer sessions in STRICT_PUBLIC_SERVICE_PROBE.json | API fixture is not UI login | Tested refresh/logout/invalid session |
| Unity Supabase Auth | FAIL | NetworkSession hello + backend Store.guest | Active game uses separate guest identity | Runtime adapter still required |
| Profile persistence API | PASS | Own profile creation/save/read after refresh; RLS negative checks | Browser persistence and offline-to-Supabase mapping not tested | No migrations needed for tested operations |
| WSS connectivity | PASS | Public Render TLS/WSS connections with expected frontend Origin | Published version unavailable | No TLS bypass |
| Two-player public protocol | PASS | Three-round public guest match, identical Draw/history | Supabase browser match not implemented | Scope explicitly limited to guest protocol |
| Six-player public protocol | PASS | Six distinct public guests, simultaneous resolution/history | Six rendered browsers and ranks not verified | No local result substituted |
| Match synchronization | PASS | Equal authoritative final resolution, privacy assertions | Public failure/reconnect matrix incomplete | Local backend21/21 supplements, not replaces |
| Match results | PASS | Guest history returned once; actual offline Result screenshot | Supabase result persistence and true podium/rank missing | No fabricated ranking |
| Character selector layout | PASS | Third-card versus Confirm world-bounds regression | Reproducible production name/button overlap | Existing MainMenuView card positions repaired |
| Vercel HTTP package | PASS | Production domain and Unity assets HTTP200 | Deployment of repaired client tracked separately | Same existing project used |
| Physical iPhone/Safari/Zalo | NOT_TESTED | No physical device available | Simulated viewports are not physical tests | Manual checklist below |

Fresh verification: Unity EditMode83/83, PlayMode35/35 (no skipped), backend21/21. Standalone motion evidence test1/1 also passed. Build evidence `STRICT_WEB_BUILD.json`: Succeeded,40,184,848 bytes,0errors,4warnings,180.3926785s. Warnings: TMP deprecated shader pragma and IL2CPP costly-method compilation notices. Source changes are UI positioning and tests/probes only; gameplay/Core, Phaser, multiplayer protocol and economy unchanged.

Actual motion frames: `screenshots/ProductionMotion-20261010-024657/` with timestamps.txt (25 captures); standalone passing run `ProductionMotion-20261010-024218/`. Failed harness-run directories are not acceptance evidence. Detailed scope/limits: CHARACTER_MOTION_RUNTIME_QA.md, CHARACTER_PICTURE_FIDELITY_AUDIT.md, SUPABASE_AUTH_RUNTIME_QA.md, WSS_MULTIPLAYER_E2E_QA.md.

Production before-fix Main Menu canvases cover viewport exactly at390×844,393×852,402×874,430×932; screenshots recorded. Battle430×932 rendered with actual bot gameplay; Result390/393 captures are labelled Result, not Battle. Full Battle portrait matrix, real touch gestures, audio audibility and FPS: NOT_TESTED unless supplemented by later evidence. Existing Unity responsive regression screenshots are distinct from public browser observations.

Security: no secret/service key used, tokens stay only in probe memory. No destructive database migration or service restart. Dedicated anonymous API QA profiles and guest match fixtures remain for traceability; no user records deleted. Authentication expiry/re-login, network interruption, browser session isolation and public failure scenarios remain NOT_TESTED.

Manual acceptance: on physical iPhone open public domain; check safe-area portrait and orientation; log in via the eventual Supabase adapter with two devices; save Vietnamese nicknames/avatars, refresh/logout/re-login; create/join room, lock actions, complete match, reconnect and compare outcomes; confirm hardware audio after gesture. Record device/browser/version and evidence. This flow cannot currently pass Supabase binding with the current runtime.

Delivery: verified local Web Build served over HTTP8790; repair must be visually checked there before source delivery. Remote main advanced during audit; merge/cherry-pick must preserve its Render/Supabase updates. Production UI deployment proof will be added after Ready and public smoke verification. No full-production DONE claim.
