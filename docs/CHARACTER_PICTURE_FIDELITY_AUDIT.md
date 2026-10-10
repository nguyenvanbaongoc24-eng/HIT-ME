# Character Picture Fidelity Audit

2026-10-10. Gate status: **FAIL** for fidelity to the current Picture roster.

Source: all nine files in `Picture` inspected by image decoding, dimensions, alpha and SHA256; both character concept sheets visually inspected. They are opaque composite reference sheets, not transparent production sheets or separate animation frames. No layered source files or individual character PNGs exist in Picture. Machine-readable per-definition GUID resolution, sprite paths, dimensions, import mode, alpha range and frame counts: `STRICT_CHARACTER_ASSET_INVENTORY.json`.

| Character | Actual use | Comparison | Status |
|---|---|---|---|
| Char01_Player | Battle prefab + menu + confirmed character selector | Black hair, white red-star shirt, blue shorts/slipper match the broad brief. Face/pose differs from current concept; no directional/state frames. | FAIL |
| Char02_BotMale | Battle prefab + selector | Green military cap/black shirt/pan match the earlier brief, but this boy is absent from current Picture's five-person roster. Not the blue-cap/red-shirt TV boy. | FAIL |
| Char03_BotFemale | Battle prefab + selector | Brown hair/flowers/bunny hoodie. Current Picture requires pink hair/heart outfit. Actual public selector still uses brown-haired sprite. | FAIL |
| PinkGirl | Production roster preview only | Independent PNG has a baked racket; not a clean hand/weapon replacement. Preview policy explicitly separates it from online/offline selectable starters. | BLOCKED |
| BlondBoy, NonLaBoy, CapBoy | Production roster preview only | Single-frame preview assets; not selected through the three-character combat catalog. NonLaBoy differs in gender/outfit from Picture's Cô Ba. | BLOCKED |
| GoldenDog, BlackCat | Production roster preview only | Single-frame animal previews, not combat character IDs or six-state animation. | BLOCKED |

Runtime path: `Scenes/Battle.unity` → `BattleView.Rebuild` → `CharacterCatalog.ForSlot` → `CharacterDefinition.visualPrefab` → one `CharacterVisual` with `CharacterPresentation` adapter. Canonical definitions are in `ScriptableObjects/Characters`, gallery definitions in `Resources/Production`. All three canonical Idle sprites have distinct SHA256 and GUIDs; no accidental same-sprite reference detected. Six actors deliberately reuse three archetypes; this is not six authored unique characters.

Foot anchors, hitbox rings and shadows are separate actor children. UI Image uses preserveAspect; CharacterVisual.Fit uses definition footPivot/referenceHeight. Runtime evidence includes timestamped combat frames and actual public selector screenshot. Source-based pivot/reference existence alone is not a full artistic acceptance.

Required artwork: pink-haired girl body without baked weapon and matching clean portrait; reference-faithful Cô Ba and military-boy decision/art; independent per-state/direction frames or rig layers for all playable human/animal IDs; separate hand/grip-compatible weapons. No crude concept crop was imported. No character or gameplay IDs were invented.

UI defect found: third starter name overlapped Confirm button. Fixed card layout in existing MainMenuView, with a world-bounds regression assertion. This repair does not resolve the art mismatch.
