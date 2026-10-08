# Character asset pipeline — current state

2026-10-08: three standalone transparent Idle PNGs and three held weapon PNGs generated using image_gen from the user-approved concept are imported. Each character has one frame. Sprite atlas holds six real images; no authored multi-frame clips yet. Portraits, separate projectile/icon art and Aim/Throw/Hit/Eliminated/Victory frames remain missing. Runtime uses Idle plus code motion for missing poses and held sprite for missing projectile art.

Characters: Char01_Player (white star tank/blue shorts), Char02_BotMale (green hat/black tank), Char03_BotFemale (pink hoodie/flower hair). Weapons: DepToOng, Chao, Vot.

## Add or replace art

- Individual transparent PNG frames: Assets/HitMe/Art/Characters/<ID>/<State>/frame_000.png, frame_001.png...; exact three-digit names. States: Idle, Aim, Throw, Hit, Eliminated, Victory. Separate portrait.png at character root.
- Weapon input: Art/Weapons/<ID>/held.png, projectile.png, icon.png.
- Use same canvas and foot baseline across actual frames. Do not treat a concept collage as a sheet.
- Run HIT ME > Characters > Import and Validate Library, or powershell -NoProfile -File tools/character-assets.ps1 from D:/HIT ME.
- Inspect docs/SPRINT3A_ASSET_VALIDATION.md for missing/rejected inputs. Open Battle and Play, then CHƠI VỚI BOT.

Importer: Sprite 2D/UI Single, PPU 100, transparent alpha, no mipmaps, max import size 512, bilinear. File pixels remain unchanged. Definition footPivot is calibrated to the visible sole; authoredFacing matches the drawing. handAnchor/gripPivot are visual coordinates only. Multiple real frames produce clips; runtime samples the same frames/FPS. Animation never changes gameplay foot anchor or hitbox.

LangQueBacBo backdrop and sand are separate raster art in Art/Arenas; the existing ellipse masks the floor texture. No new collision geometry is derived from painted props. Five other maps remain reference concepts.

Source PNGs and prompt provenance: docs/SPRINT3A_ART_ASSETS.json, docs/SPRINT3A_ART_PROMPTS.json, docs/SPRINT3A_ARENA_PROMPTS.json. Latest execution results/screenshots: docs/SPRINT_3A_CHARACTER_REPORT.md.
