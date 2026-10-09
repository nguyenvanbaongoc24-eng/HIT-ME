# HIT ME — Aim Visual QA (09/10/2026)

Scope: replace the actual Battle aim renderer. Northern Village changes already made before the user switched scope remain in the workspace; this aim sprint does not expand Main Menu, map assets, character kit, gameplay or network protocol.

## Implementation

The previous renderer was the green `AimLine` UI Image created by BattleView.Build and resized/rotated by foundation Aim(), offline PaintMatch(), and online PaintOnline(). All three branches now use one ProductionAimIndicator mesh prefab, under the existing arena floor mask. There is no second LineRenderer, Image line, Gizmo or parallel controller.

The overlay Canvas uses no world camera. BattleView.ToCanvas retains its logical scale and ArenaOffset. The indicator receives floor-local feet/endpoint coordinates by subtracting ArenaOffset. Endpoint remains the existing ArenaGeometry.ProjectileCollision(position, direction, projectileRadius): wall endpoint only, never opponent hit prediction. Input, combat trajectory, hit resolution and server actions are unchanged.

The narrow-to-wide cream cone has a translucent inner gradient and three-pixel feather strips, six animated mesh dots, and a pulsing endpoint ring. Renderer smoothing uses an exponential angle interpolation at 65/s; it never writes back to gameplay direction. Pointer release and Locked snap to authoritative direction. The emission begins 24 reference pixels forward along the authoritative launch ray, rather than at the foot center. This is a virtual weapon release offset, not an authored skeletal hand socket. The mesh is behind actors and clipped to the exact arena mask. No character or weapon art is manufactured for this sprint.

Hidden generates no mesh. Aiming follows drag. Locked retains exact direction with a short confirmation pulse, including Reveal. Throwing fades over 0.14 seconds when the actual Throw phase starts; RoundResult hides it. Reduced Motion disables dot travel and ring oscillation. The default Unity UI material is used; no extra emissive material/post-processing is required.

No GameObject is created per frame. Geometry uses a fixed bounded mesh layout; warmed VertexHelper buffers are reused. Renderer code creates no managed objects per frame. Actual total GC, mobile FPS and FPS impact have not been profiled; no 60 FPS claim is made.

## Files

- Resources/UI/Kit/ProductionAimIndicator.prefab (editable, instantiated in Battle).
- Scripts/UI/ProductionAimIndicator.cs (mesh and visual states).
- Scripts/UI/BattleView.AimVisual.cs (existing geometry to renderer).
- Scripts/UI/BattleView.cs, BattleView.Offline.cs, BattleView.Network.cs (old renderer removed).
- Scripts/UI/ArenaInput.cs (release snaps presentation only).
- Editor/AimVisualSetup.cs, Editor/HitMeWebBuild.cs.
- Tests/PlayMode/ProductionAimTests.cs.

## Evidence

PlayMode drives real pointer down/drag/up and Ready through the existing floor/input handlers, checks direction dot product >0.999 after release, exact geometry endpoint within 0.02 canvas units, nonzero launch inset, one indicator, no AimLine, no raycast interception, and actual Locked → Reveal → Throw → Hidden phases. Four viewports: 360×800, 390×844, 402×874, 430×932.

Screenshots: `screenshots/Aim-{Up,Down,DiagonalLeft,DiagonalRight,NearWall}-{width}x{height}.png`; `Aim-SixCharacters-Locked-Reveal-{width}x{height}.png` shows all six actors after their positions become public. During active placement/aim only the player is visible, preserving the established privacy rule. `Aim-Before-Debug.png` is a historical real Unity capture; `Aim-Iteration1.png` records the initial missing renderer defect; `Aim-Iteration2.png` records corrected rendering before the final cone-opacity adjustment. No screenshot is a concept composited over the game.

Initial QA caught the missing CanvasRenderer and a test that attempted to override the presentation state while Battle still owned Locked. Both were corrected; the transition check now advances the real match instead of simulating success. Final results and browser verification are appended below.

## Pending manual verification

Physical mobile touch/thermal/FPS/GC profiling; authored weapon hand sockets if exact hand attachment is wanted. No 5–10 second video has been captured. Screenshots demonstrate the current visual treatment, not pixel-identical recreation of the full concept UI.

## Final Unity checks

- VERIFIED: EditMode 83/83 (`AIM_EDITMODE_RESULTS.xml`).
- VERIFIED: PlayMode 32/32 (`AIM_PLAYMODE_RESULTS.xml`), including existing gameplay, safe-area, character and live-server checks plus the new actual touch/drag/state/endpoint test. No unexpected Console messages were accepted by those tests.
- VERIFIED: six-actor Locked/Reveal screenshots in all four viewports. Opponents are not revealed during placement.
- Browser/Web Build verification: in progress; see final appended result.

## Final Web verification

VERIFIED: `AIM_WEB_BUILD.json` reports Succeeded, 38,744,394 bytes, 0 errors, 4 warnings, 221.0022722 seconds. This is an actually executed Unity WebGL build, not a source-only claim. `AIM_HTTP_CHECK.json`: index, loader, data, framework and wasm all HTTP 200 with response lengths matching build files at http://127.0.0.1:8791/.

VERIFIED: browser reload used that HTTP URL. Started offline training and performed actual drag at 390×844; observed cream cone/dots/wall-endpoint ring and no green debug line. Browser Console warning/error capture was empty. Also checked live Battle canvas sizing and HUD/floor responsiveness at 360×800, 402×874, 430×932; restored default viewport afterwards. Full per-direction screenshots and six-actor checks were executed in Unity PlayMode at all four requested sizes. Background-tab throttle initially swallowed a short click; a held click started the match. No Unity failure was inferred from that automation limitation. Overlay desktop FPS is not treated as mobile performance evidence.

Status: IMPLEMENTED / TESTED / WEB BUILD VERIFIED. Physical-phone performance, total runtime GC measurement and video remain PENDING MANUAL VERIFICATION. No claim that the entire reference UI, character rigging, map kit, or strict separate visual-pack sprint is complete.
