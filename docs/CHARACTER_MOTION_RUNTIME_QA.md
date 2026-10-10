# Character Motion Runtime QA

2026-10-10. **PASS** for limited runtime transform motion evidence; **BLOCKED** for authored frame/rig animation.

`CharacterVisual.Present` is the active renderer. Canonical definitions each have one Idle sprite, no non-Idle frame arrays and no assigned clips. No active Animator controller drives these sprites. `CharacterPresentation` forwards real gameplay poses from BattleView. Gallery uses CharacterShowcaseMotion and is a preview.

| State | Actual implementation | Status |
|---|---|---|
| Idle | C: artwork child bob + uniform breathing scale | PASS |
| Positioning | Root position follows confirmed placement; no walk clip | BLOCKED |
| Aim | C: full-body slight tilt, facing flip and held-weapon direction | PASS |
| Aim Locked | Same Aim pose; aim indicator confirms lock | PASS |
| Anticipation/follow-through | C: Throw angle changes over elapsed time | PASS |
| Weapon release | Held sprite hidden in Throw; pooled visual projectiles | PASS |
| Hit | C: child shake and red tint | PASS |
| Eliminated | C: full-body 58-degree tilt, gray tint, weapon hidden | PASS |
| Victory | C: artwork child bounce | PASS |
| Defeat | No separate CharacterVisual state | BLOCKED |

PASS denotes code motion, not concept animation fidelity. Full-body rotations remain; 58-degree elimination is not an authored fall. Rig/clip durations, expressive face frames and weapon release frames cannot pass without source artwork.

Evidence test: `ProductionMotionEvidenceTests.RealCombatProducesTimestampedMotionEvidence`, fresh XML `STRICT_MOTION_TEST_RESULTS.xml`. Seven Idle captures span over three seconds, with actual artwork-position changes and fixed logical feet asserted. Three rounds use normal Core Place/Aim/Lock/Tick and Battle rendering, with automatic bot scheduling disabled only in the test fixture so actions remain controlled. Records Aim, Locked, Reveal, Throw, follow-through, Hit, final Eliminated/Victory. Timestamp files identify captures. No generated sprite frames are used.

Initial probe failures were fixed in the harness: Game View creation, monotonic action times after idle recording, and automatic bot locks interfering with controlled actions. Those failures were not attributed to Unity gameplay. Final screenshot directories must be selected from passing runs, not failed captures.

Weapons: canonical slipper and pan have independent held/flight sprites, configurable hand/grip anchors, pooled ProjectileVisualController spin; gameplay origin remains logical and rendering does not compute damage. Source binding and existing pool/hitbox tests verify independence. Frame-perfect hand articulation and concept release fidelity remain BLOCKED. Actual auditory SFX quality NOT_TESTED; SFX pack remains temporary synthetic audio.
