# HIT ME — Character Runtime Asset Audit

Audit date: 2026-10-10. Status: BLOCKED for full concept/animation fidelity; PASS for distinct current canonical mapping.

Picture contains exactly nine composite references: Concept characters.png, Concept characters 2.png, MAIN MENU.png, Match Result.png, Play in game.png, UI KIT.png, UI CONCEPT.png, Sân Đình Bắc Bộ.jpg and Phố Cổ Hội An.jpg. They are references, not ready transparent frame sets. No crude crops were imported in this sprint.

| Runtime ID | Actual body | Actual portrait | Prefab / Definition | Status |
|---|---|---|---|---|
| Char01_Player | Art/Characters/Char01_Player/Idle/frame_000.png; GUID ba260a38afc654e4489fc855780ee3cf | Existing BlackBoy portrait; GUID d4904d1612ffbcb4a87f01ead8398592 | Prefabs/Characters/Char01_Player + ScriptableObjects/Characters/Char01_Player | PASS mapped; single-frame procedural motion |
| Char02_BotMale | Art/Characters/Char02_BotMale/Idle/frame_000.png; GUID 972258814e0d6f145a9d53b07953bb31 | Existing military male portrait; GUID 0c7204a1ebadece46b726ab3c2d4327d | Prefabs/Characters/Char02_BotMale + ScriptableObjects/Characters/Char02_BotMale | PASS mapped; older concept; full latest-roster replacement BLOCKED |
| Char03_BotFemale | Art/Production/Characters/pink-static-preview.png; GUID 8e4befe12746c164dab993c583067352 | Portraits/PinkGirl.png; GUID 3f068f708f0a255408cbe1bf3484a27f | Existing Char03_BotFemale prefab + definition | PASS actual pink replacement from prior fix; single frame, baked racket |

All paths above are relative to unity-client/Assets/HitMe. The canonical IDs are unchanged in Supabase profile fields/server whitelist. Three body GUIDs are distinct, not a repeated fallback.

Runtime trace: CharacterCatalog → Supabase profile.avatar/server profile.avatar → MainMenu RefreshStatus → profile/character selection → NetworkLobby RoomRoster → Battle Rebuild → online ResultPortrait. Definition foot pivots and CharacterVisual.Fit control artwork; logical foot/hitbox remain independent. Offline ResultView still uses its existing text result presentation (no new portrait claim).

The seven-entry ProductionVisualPack gallery contains independent static previews. It is not the playable server catalog and has no authored multi-frame gameplay animation. Animals are showcase previews, not online fighters.

Required production assets: clean weapon-free bodies or separate rig layers for each approved roster member; authored Idle/Aim/Throw/Hit/Eliminated/Victory frames or rig clips; separate hand/grip anchors; transparent portraits. PinkGirl currently has a baked racket, so interchangeable held weapons/weapon disappearance during Throw are BLOCKED by weapon-free artwork. Other procedural motion is verified as presentation only, not claimed as authored animation.

Runtime motion evidence: fresh ProductionMotion-* PlayMode capture sequences; exact final directory recorded in sprint delivery. Tests observe changing artwork over time, all combat states, and fixed logical feet/hitboxes.
