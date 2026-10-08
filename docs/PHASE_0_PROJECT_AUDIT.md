# Phase0 — pre-flight project audit

Date:09/10/2026 Asia/Bangkok. Checkpoint before phase:`72bea53`.

Verified project:`D:/HIT ME/unity-client`; Editor:`D:/App/UNITY/6000.6.4f1/Editor/Unity.exe`, version6000.6.4f1. WebGLSupport module exists. No Unity project Editor process/lock was active; Unity Hub helper is not a project Editor. Existing URP2D/project/scenes/build template preserved.

## Existing implementation

| Area | Existing system / actual capability | Boundary |
|---|---|---|
| Scenes | Boot, MainMenu, Lobby, CharacterSelect, Battle, Result in enabled build settings | No replacement project/scene |
| Canvas/input | MainMenuView runtime Canvas390×844, SafeAreaAdapter/WebMobileBridge, SceneEntry EventSystem/InputSystemUIInputModule | Desktop emulation doesn't prove real phone touch |
| Navigation | FoundationMenu→MainMenuController prefab, MenuNavigationService, NetworkLobbyView | State/loading/cancel needs additional regression coverage |
| Localization | Strings + VI/EN JSON, TMP NunitoSDF | Be Vietnam Pro not supplied |
| Bots | OfflineRunContext / OfflineMatch / BattleView;1–5bots, Easy/Normal | Hard not implemented; timeout rules still marked proposed |
| Online | Existing server WS/WSS, guest resume, private/quick, authority, SQLite profile/rewards | Supabase/publicWSS not configured; no server changes in this phase |
| Profile/inventory | Real NetProfile binding, owned cosmetic equip, daily/weekly quests | No fake balance/rank; no crafting recipes |
| Characters |3 independent Idle sprites via CharacterVisual | No new7-character concept exports; animation states use code fallback |
| Maps |6 options; existing Làng quê backdrop/sand;5explicit missing-art fallbacks | No crop of Play in game concept |
| Settings | Graphics60/30 target and immediateVI/EN | Music/SFX assets absent |
| Placeholder features | Shop, rank, inbox, community show Coming soon; characters preview only | Not represented as production features |

The existing MainMenu already has CTA/cards/nav. It still exposes art/coins trial annotations on Home; Phase1 will move diagnostics out of the Home presentation and separate MenuScreenState explicitly. Current dependent widget prefab count is1root menu, not an independent reusable UI Kit. UI Kit is a later gate, not completed by this audit.

## Asset audit / Gate0

PICTURE_ASSET_AUDIT.md + JSON and HITME_UI_ASSET_MANIFEST.md classify all5files as reference_composite, including the RGBA UI board.27existing standalone runtime PNGs are distinguished from those references. No Picture file imported into Assets and no original changed. Gate0 asset audit:PASS.

## Execution evidence

Fresh tests/build per phase are recorded in Phase0-EditMode-results.xml, Phase0-PlayMode-results.xml and PHASE0_WEB_BUILD.json. Logs:unity-EditMode.log,unity-PlayMode.log,phase0-WebBuild.log (ignored in Git). Baseline code/runtime behavior is unchanged; only audit tooling and Editor build report entry points added.

Browser tab is still a data:error-page blocked by the browser tool's HTTP/HTTPS policy. User was requested to reopen the valid loopback URL; no workaround attempted. This blocks browser smoke evidence, not asset classification. iPhone Safari device test:NOT RUN. No FPS/device claims.

Fresh results:EditMode71/71,PlayMode17/17,Web Build Succeeded21.398.041bytes,0errors,1warning,143.69seconds. Gate0 PASS; screenshot Unity PlayMode:Phase0-MainMenu-390x844.png and430x932.png. The warning is TMP deprecated shader; browser/device checks remain separate.
