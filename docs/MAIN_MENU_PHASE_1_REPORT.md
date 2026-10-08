# Main Menu — Phase1 functional/responsive report

Date:09/10/2026 Asia/Bangkok. Checkpoint before phase:`86ec5bc`; Phase0 Gate0 passed first. Work uses the existing Unity6000.6.4f1 project.

## Implemented

- Retained MainMenu scene/bootstrap, MainMenu.prefab, existing Canvas/EventSystem/TMP/font/theme and independent sprites. No new project or duplicate character/lobby controller.
- Added MenuScreenState: Home/Connection/Settings/Practice/Maps/Characters/Notice panels; Idle/Connecting/WaitingForRoom/Failed request states. MainMenuController owns intent, timeout, cancel and late cancelled-room handling. State never changes gameplay, profile, rewards or inventory.
- Back/close connection clears the pending intent; close after a failed request can cancel its outstanding intent. Escape supports panel back. Existing NetworkSession guest resume/backoff remains in use; private/quick and account panels route through MenuNavigationService. MenuSceneIds centralizes the new menu routes.
- Removed Home trial/spec/art diagnostics. Currency is actual server coins with Xu Làng label, without a fake balance when disconnected. Art/timeout annotations are behind Dev Diagnostics in Editor/development builds; missing-art facts stay in the manifest/report. Practice panel shows a player-facing aim tip instead of spec text. Help opens a real instructions popup.
- Retained bot count1–5, Easy/Normal, arena choice, graphics60/30 target and immediateVI/EN. Close/back does not reset bot configuration. Hard/audio/shop/rank/social are not falsely enabled.
- Compact layout hides secondary cards first when safe height<720reference units, retains practice/CTA/footer and adjusts character stage. Standard7portrait sizes retain cards. Modal close target increased48×48reference units. Nullable runtime SafeArea override is now a property, avoiding a serialization warning.
- No texture polish, concept cropping, new animation frames, rigs, pet gameplay, VFX or environment motion added in Phase1. Existing single-frame character tween and minimal UI fallback remain clearly recorded as fallback artwork.

## Button audit

| Action | Actual behavior / state |
|---|---|
| Quick / Private | Explicit guest connect when needed; network loading/error/timeout/cancel. Quick goes to real waiting room; Private opens Create/Join Lobby. |
| Profile / Quests / Inventory / Collection | Existing server data pages; no client balance writes or fabricated grant. |
| Bot practice | Count/difficulty setup and actual OfflineRunContext.Play → Battle. |
| Maps / Travel | Existing arena selection, with missing-art status. Not a new travel progression feature. |
| Characters | Preview3existing sprites; no fake selection of the new7concept characters. Inventory equip remains functional. |
| Settings | Graphics and instantVI/EN; audio assets unavailable. Dev diagnostics hidden in nondevelopment Web build. |
| Help / Home / close / cancel | Real popup/return/cancel actions. |
| Shop / Rank / Inbox / Community | Coming-soon explanation, not empty tab or fake production feature. |

Buttons retain normal/pressed/selected/disabled colors and lightweight existing press feedback. A shared loading spinner and disabled primary actions reflect the real request state. No additional UI Kit skin or complex animation introduced.

## Executed tests

- EditMode **75/75 PASS** (71existing +4MenuScreenState regressions).
- PlayMode **19/19 PASS,0skipped** (17existing +2Phase1flows). HITME_LIVE_TEST_URL was set to the running local backend; the live test was executed, not skipped.
- Live Unity Editor client used actual MainMenu buttons to guest-connect, create a private room, leave, Quick Match into a public waiting room, leave and navigate to owned Inventory. The new test profile stayed0coins; navigation did not grant rewards. This is a real Editor WebSocket test, **not two Unity Web clients**.
- Bot button launches a real OfflineMatch with the chosen player count; Back preserves count/difficulty. Transport loading/cancel and compact footer bounds verified. The existing suite covers Battle privacy, HP, timeout, first target, results, sprites and network presentation.
- Backend tests **16/16 PASS**, including actualWS full match and verified test-CAWSS. Server sources were not changed.
- Seven sizes:360×800,375×812,390×844,393×852,402×874,412×915,430×932; inset simulation top59/bottom34px; TMP bounds/overflow/fonts, interactive touch target sizes and nonoverlap checked. Additional short-height compact-layout regression passed.
- Fresh Unity screenshots: `screenshots/Phase1-MainMenu-{size}.png`, including390×844and430×932. These are PlayMode renders; not concept mockups and not browser screenshots.

XML:Phase1-EditMode-results.xml,Phase1-PlayMode-results.xml. Build result:PHASE1_WEB_BUILD.json. Logs:unity-EditMode.log,unity-PlayMode.log,phase1-WebBuild.log (ignored in Git). Final build status and gate decision are appended after the build finishes.

## Assets and missing features

No file from Picture was imported. All5boards stay reference_only. Existing27standalonePNGassets are described in HITME_UI_ASSET_MANIFEST.md and PICTURE_ASSET_AUDIT.json. Still missing: new human/pet exports, true state frames/rig layers, new weapons, portraits, layered village-gate environment, illustrated cards and separateVFX. Current Làng quê background is an existing arena fallback, not the approved layered MainMenu artwork.

UI Kit independent widgets/showcase are **NOT RUN**; Phase2Artwork and Sprint6Motion are **NOT RUN**. No transition to a later phase while unresolved acceptance remains.

## How to run

Start multiplayer-server per its README. Set `$env:HITME_LIVE_TEST_URL='ws://127.0.0.1:8788/play'` then run `./tools/master-phase.ps1 -Phase 1`. Tests are opt-in for external live server outside this invocation. Web host:`python ./tools/serve-web.py --port 8791`; URL:http://127.0.0.1:8791/.

Unity scene:`Assets/HitMe/Scenes/MainMenu.unity`. Source/original files remain intact. Do not open a second project Editor while batch tests/build run. iPhone Safari device test:**NOT RUN**; Chrome Android device test:**NOT RUN**. NoFPS/memory acceptance claimed.

## Final evidence and gate decision

Web Build **Succeeded**,21,395,100bytes,0errors,5warnings,191.82seconds. Inherited warnings:LobbyCS0108,TMP shader deprecation and compiler splitting. New nullable override serialization warning resolved.

**Gate1:PARTIAL** pending browser smoke/final Web Console verification. The existing tab remains a data:error page; browser policy blocks binding it. User was asked to reopen the validHTTP URL. No bypass attempted. Unity PlayMode screenshots and realEditor-server tests are valid evidence but not mislabeled as Web/device tests. No transition to UI Kit/Phase2/Motion.

Phase1 checkpoint:local tag hitme-master-phase1-20261009. Source files listed in PHASE1_FILE_INVENTORY.json. All5Picture original hashes still match the Phase0audit.
