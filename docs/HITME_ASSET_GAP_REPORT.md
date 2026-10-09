# Master production asset gaps — 2026-10-09

## INTEGRATED

Seven static character definitions and portraits, nine cosmetic weapon definitions, seven independent menu/battle decoration instances, transparent contact shadow, tiled stone floor and central lotus. Original three gameplay character IDs and three gameplay weapon IDs remain authoritative. New roster/weapon entries are a clearly labeled visual gallery, not online equipment.

Follow-up on reissued Master request: menu now has a fourth static foreground tea-prop layer (eight menu/battle decoration instances total). Gallery reuses canonical existing slipper/pan sprites; generated alternatives archived under docs/art-candidates, outside Unity. Weapon trail width/color/enabled are configurable visual data and actually read by ProjectileVisualController. Gameplay projectile position/damage are unaffected.

## FALLBACK / NEEDS_ARTWORK

- Character Idle/Aim/Throw/Hit/Eliminated/Victory use existing procedural visual motion on real static sprites. No invented animation frames or rig. Pink girl preview has a baked racket; it is not a separable weapon attachment.
- Dog/cat have whole-body motion only. Tail wag, independent limbs, jump/throw clips need actual parts or genuine frames.
- Main Menu has four genuine independent overlays (leaves, flag, lantern, tea props); the cleaned village background still contains flattened architecture, sky, courtyard and foreground. Eight registered scene layers and depth parallax are not complete.
- Battle uses four independent decorations including crowd. Flag/lantern/leaf motion transforms each complete PNG; no cloth/leaf rig. Crowd reacts only to confirmed combat results.
- Main Menu and Battle cleaned backgrounds are AI-derived reference illustrations, not exact layer exports from source. Logo is illustrated UI-kit artwork rather than the reference brush-lettered logo.
- Native icon fallbacks remain where standalone concept icons are unavailable. Teal/parchment skins are imported but unused. Generated slipper/pan alternatives are archived outside Unity; runtime uses good existing canonical art.
- Other map backgrounds, genuine multiframe animation, independent eye/hand/tail layers and production atlas/device tuning remain missing.

## BLOCKED / PENDING MANUAL VERIFICATION

No confirmed server equipment/unlock definitions for the new visual IDs. No new damage, rewards, shop, ranking or currency rules inferred. Gallery selection does not modify loadout. Real iOS Safari/Android devices, Dynamic Island hardware and device FPS/memory benchmarks have not been tested. Full visual-production acceptance remains PARTIAL.

Online Lobby retains its older working controls and room-code input presentation. Result uses existing themed actions/procedural feedback; a full concept-specific layered result illustration is not complete. Verified navigation is not represented as complete visual redesign of these screens.
