# Vietnam map visual QA

Status: **PARTIAL**, acceptance requires actual test/build results below. References: Picture/Play in game.png, MAIN MENU.png, UI CONCEPT.png and UI KIT.png; character boards are actor/prop references, not map layers.

Bắc Bộ uses actual clean background, shared stone floor and faint lotus. No concept HUD/actors are baked into these production background/floor PNGs. Border/collision geometry remains unchanged. Selection preview is explicitly a preview, with no gameplay HUD. Existing ImageCover preserves background aspect; floor tiles repeat, not stretched photos.

Hội An and Hạ Long lack independent artwork/thumbnail/far/mid/foreground. Their actual implemented previews/Battle fallbacks are labeled as missing location artwork. Shared stone/lotus is generic fallback, not Vietnamese coastal/town production art. No screenshot is represented as completed Hội An/Hạ Long scenery.

Planned actual captures: Map-Selection.png, Map-BacBo-Actual.png, Map-HoiAn-Fallback.png, Map-HaLong-Fallback.png, Map-HoiAn-Fallback-Preview.png. Missing references, safe area, card text, confirmation semantics and online isolation are validated through Unity tests and final browser smoke where available. Real devices/Dynamic Island/FPS remain pending.

Actual captures above have now been produced by real PlayMode rendering. Map-Visual-Comparison.png places the Battle concept beside Bắc Bộ and clearly labeled Hội An/Hạ Long fallbacks. Fallback screenshot is a layout preview with actor sprites, not proof of completed location art. Bắc Bộ thumbnail uses an aspect-fill masked image; no photo stretching. Preview floor dimensions preserve the current 2000:3500 visual proportion; preview is not a collider.

Unity compile succeeded. Final **83/83 EditMode and 30/30 PlayMode PASS**, including previous gameplay/network/UI checks, map confirmation/isolation and motion focus/quality/reduced-motion/touch policy. Actual evidence: MAP_EDITMODE_RESULTS.xml and MAP_PLAYMODE_RESULTS.xml. No unexpected Console errors in these passing tests. Latest Web Build/browser results will be recorded below.

Browser first pass found a selected-marker offset inherited from the larger prefab dimensions. Corrected the marker to the resized map card's left edge; added an actual bounds assertion and Map-Selection-Confirmed.png screenshot. Also corrected fallback text to identify shared stone floor instead of the older sand placeholder label. These presentation corrections do not change selected-map persistence, geometry or simulation.

After corrections: **83/83 EditMode and 30/30 PlayMode PASS** again. Map-Selection-Confirmed.png has the selected marker inside the Hội An card; the bounds regression assertion passes. Backend rerun: **16/16 PASS**, including real WS matches/privacy/reward dedupe/TLS. Evidence: MAP_BACKEND_TESTS.txt. No Phaser/Core/backend source modifications.

Final Web Build: **Succeeded, 38,395,588 bytes, 157.7302808 seconds, 0 errors / 4 warnings**. Warnings are existing TMP deprecated shader pragma / generated C++ translation-file messages. Evidence MAP_WEB_BUILD.json / unity-map-web.log. HTTP server actually verified at 127.0.0.1:8791 (PID 21644). Index and four build files return 200 with SHA-256 matching current disk files: MAP_HTTP_CHECK.json.

Real localhost browser: selected marker correct on Hội An and Bắc Bộ; preview/confirm flow works; fallback selection changed actual offline Battle rendering in initial smoke. Final build Map Selection visually checked at **360×800, 375×812, 390×844, 402×874, 430×932**. Bắc Bộ preview shows real independent decorations, EN map names/policy render correctly, then VI restored and Bắc Bộ selected for delivery. Browser warning/error logs empty. Override reset. Tool-displayed responsive screenshots are observations; persistent local captures come from actual Unity PlayMode. No Safari/Android hardware, Dynamic Island device or FPS benchmark is claimed.

Open MainMenu → Bản đồ, click a card to preview, then explicitly confirm. Bắc Bộ uses existing art. Hội An/Hạ Long confirmation is labeled offline fallback. Unsupported legacy maps without floor remain unconfirmable. Unity menu **HIT ME → Configure Vietnam Map Kit** rebuilds definitions using the existing project. Online map synchronization remains MISSING; online uses default visual map. Full three-map production kit is not COMPLETE.
