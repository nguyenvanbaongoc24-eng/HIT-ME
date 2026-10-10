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
