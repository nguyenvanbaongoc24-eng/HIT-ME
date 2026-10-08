# Picture asset audit — Phase 0

Visual review and filesystem inspection completed for every file. Originals unchanged; no reference board imported into Unity runtime.

| Full name | Bytes | Resolution | Mode / alpha | Classification |
|---|---:|---|---|---|
| Concept characters 2.png | 3097500 | 1536×1024 | RGB / none | reference_composite |
| Concept characters.png | 2975709 | 1536×1024 | RGB / none | reference_composite |
| MAIN MENU.png | 2786372 | 941×1672 | RGB / none | reference_composite |
| Play in game.png | 2836209 | 967×1627 | RGB / none | reference_composite |
| UI CONCEPT.png | 3215000 | 1536×1024 | RGBA / [19, 252] | reference_composite |

## Gate 0

PASS: all5 boards classified reference_composite. None is a production sprite/sprite sheet/layered source/font. Alpha on UI CONCEPT ranges19–252 but belongs to the whole labeled board. No transparent isolated character/icon, frame exports, PSD/rig or text-free background exists in Picture.

Concept strength/speed/damage claims are art-board annotations; gameplay parameters and cosmetic-only weapon rules stay unchanged. Animals shown on boards are not imported as throwable objects.

## Existing runtime assets

27 independent PNGs already integrated before this phase:3 single-frame characters,3 weapons,2 arena images and19 minimal UI sprites. No new Picture artwork integrated. Existing code idle/pose tweens are fallback, not real frame animation.

See HITME_UI_ASSET_MANIFEST.md and PICTURE_ASSET_AUDIT.json for paths, dimensions, alpha, pivots, PPU, atlas and missing items. Rights/provenance for current runtime art are documented separately; ownership of the concept boards is not independently established.

## Missing production exports

### characters
- Nam Lang matching new concept (existing Char01 is visual fallback)
- Nu Nang Dong pink-haired standalone (current Char03 brown-haired fallback)
- Quay Boy
- Co Ba
- Anh Teen
- Cho Vang
- Meo Mun

### states
- Real Idle/Aim/Throw/Hit/Eliminated/Victory frames or rig-ready layers for each character; current3 characters have only1 Idle frame

### weapons
- Standalone variants: colored slippers, phone, mosquito racket, pickleball racket, beer prop, TV, mini fridge, plastic chair, rice cooker, conical hat, paper fan
- Pets as safe cartoon participants only; no violent throwing

### ui
- Dedicated portraits
- Illustrated feature/map/character/weapon cards
- Production material skins
- Independent HUD/chat/popup art

### environment
- Village-gate far/mid/foreground layers
- Flags, lanterns, leaves, crowd as separate exports
- Five other arena art sets

### fx
- Dust, impact, trail, sparkle, confetti, leaf, HP reaction sprites/frames

### font
- Be Vietnam Pro not provided; existing licensed Nunito supports Vietnamese and remains in use

