# Visible runtime fixes — 2026-10-10

## Implemented
- SceneEntry installs one AudioListener when the runtime camera has none. Previously no Listener existed in source/scenes, so imported AudioSources could play without an output listener.
- Audio unlock runs before UI click playback; unplayed events are no longer consumed while audio is locked.
- Public connection defaults to the verified `wss://hit-me-zj17.onrender.com/play`; localhost/editor retains local backend. Guest name is localized.
- Actual Battle Char03 uses the existing independent transparent pink-haired PNG and its portrait. Canonical avatar ID and combat rules are preserved.
- Pink artwork includes a baked racket. Additional held sprite is suppressed to prevent overlap. Swapping/removing that baked racket needs weapon-free artwork; this is NOT complete interchangeable-weapon animation.
- Held weapon is parented to MotionArtwork, so it follows body breathing/bob/hit motion.

## Tested
- Unity EditMode: 83/83 passed.
- Unity PlayMode: 36/36 passed, zero skipped. Includes one active AudioListener and imported throw clip on a playing AudioSource, plus foot/hitbox invariance and combat presentation.
- No new animation frames were generated or claimed. Six states still use procedural motion where authored frames are missing.

## Pending
- Fresh Web Build, Git/Vercel delivery and public screenshots: recorded below once verified.
- Audible sound on physical mobile and measured FPS: not verified.
- Production frame sets/rigging for all characters and animals remain incomplete. The nine Picture files are composite references; available independent Unity PNGs are a separate set. No Picture/SFX originals were changed.
- Imported SFX are the existing temporary audio pack, not final studio recordings.

- Fresh Unity Web Build: Succeeded, 0 errors (see PRODUCTION_RELEASE_WEB_BUILD.json). Backend: 21/21 passed.

## Public delivery verified
- Git main commit ed8c67f; Vercel production deployment 7qXCT7n6FtpY9a7HTo8e9p5xPXtm is Ready.
- Public Web.data.unityweb HTTP 200; SHA256 equals fresh local release.
- Actual 390x844 browser connection modal contains Render WSS and localized guest name. A first request hit the existing 15-second UI timeout; connection completed later and real profile/lobby opened. This UX timeout is still unresolved; do not claim seamless first connection.
- Browser captured zero error-level console entries during this check.
- Public Battle ran, but screenshot timing caught placement/spectating; use Battle-PlayMode.png for the actual three-body replacement evidence, not the empty public placement screenshot.
- Browser audio audibility has not been measured. Unity playback/Listener verification passed; this report does not claim a physical-device listening test.
