# Current release

Open **unity-client/** in Unity Hub. The root Assets/Packages/ProjectSettings are the preserved GitHub starter; current gameplay and artwork live in unity-client.

The tested local Unity Web package is **web-release/**; root vercel.json is configured for static hosting. Latest validation: 83/83 EditMode, 34/34 PlayMode, 21/21 backend tests; Web Build succeeded with 0 errors / 1 TMP shader warning. Viewport gutters, confirmed character selection, local profile editing, lobby portraits, result counters and temporary SFX integration are implemented. **Production release is BLOCKED**: Supabase public key returns 401, requested Vercel alias returns 404, and public WSS hosting/production artwork gates remain unresolved. See docs/HITME_PRODUCTION_RELEASE_REPORT.md. The older foundation notes below describe earlier stages.

---
# HIT ME

Unity 6 Web-first foundation lives in `unity-client/`. The original Phaser prototype remains under `apps/client`, with TypeScript gameplay/config under `packages/`; it is preserved independently.

## Unity foundation

Editor: **6000.6.4f1**, detected at `D:/App/UNITY/6000.6.4f1/Editor/Unity.exe`, with Web Build Support. URP **17.6.0**, Input System **1.19.0**, UGUI **2.6.0**, Test Framework **1.8.0** match the package versions supplied by this installed Editor's URP template.

Add `D:/HIT ME/unity-client` in Unity Hub, open with the matching Editor, and open `Assets/HitMe/Scenes/Battle.unity`. Press Play: the UI is constructed at runtime. `HIT ME > Open Battle` is an alternative. `HIT ME > Configure Foundation` creates any missing scenes and configures the 2D renderer/portrait/Web settings without replacing existing scenes.

Battle first shows a clearly labeled layout preview with 1 player and 2 bot placeholders. Press **SẴN SÀNG** to begin the five-second placement demonstration. Tap/drag inside the arena to place/aim; press ready again to lock only your input. Opponents stay hidden until the deadline. A supplied position and aim allow the reveal preview; timeout without a complete action stops at the unresolved-rule gate. No combat resolution, winner, online traffic or economy is fabricated.

Settings button currently switches VI/EN. Chat and weapon buttons show placeholder notices. Lobby, CharacterSelect and Result are navigation shells, not implemented systems.

```powershell
powershell -NoProfile -File tools/unity.ps1 Configure
powershell -NoProfile -File tools/unity.ps1 EditMode
powershell -NoProfile -File tools/unity.ps1 PlayMode
powershell -NoProfile -File tools/unity.ps1 BuildWeb
```

Run one Editor operation against this project at a time; close its interactive Editor before batch commands. Override `-Editor` if your installation path differs. Logs/results go to `docs/`; Web output goes to `unity-client/Builds/Web`.

See [Unity Editor guide](docs/UNITY_EDITOR_GUIDE.md), [Sprint report](docs/SPRINT1_REPORT.md), [acceptance checklist](docs/ACCEPTANCE_CHECKLIST.md), and [unresolved requirements](docs/MISSING_REQUIREMENTS.md).

## Existing Phaser prototype

Existing root npm commands are unchanged: `npm run dev:client`, `npm test`, `npm run build:client`. Unity does not depend on them.
