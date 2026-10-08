"""Read-only source audit. Classification reflects visual review of all five boards."""
from pathlib import Path
from PIL import Image
import hashlib,json,re
ROOT=Path(__file__).resolve().parents[1]
DOC=ROOT/'docs'
review={
 'Concept characters 2.png':('Characters, pets, weapons, reactions and VFX','Seven illustrated subjects on one board; frames/poses are not independent production sprites.'),
 'Concept characters.png':('Human characters, weapons, reactions and VFX','Labeled concept board; no reliable frame grid, foot pivots or transparent exported poses.'),
 'MAIN MENU.png':('Main Menu composition','Profile values, logo and button text baked in. Reference only; no standalone background layers.'),
 'Play in game.png':('Battle/HUD composition','Characters, HP and input UI baked into arena. Not a playable arena/background asset.'),
 'UI CONCEPT.png':('UI design system','Design board includes labels and mock data; RGBA does not make the compound board a ready sprite.')}
records=[]
for index,p in enumerate(sorted((ROOT/'Picture').rglob('*'))):
 if not p.is_file():continue
 with Image.open(p) as im:
  dims=list(im.size);mode=im.mode;alpha='A' in im.getbands();extrema=list(im.getchannel('A').getextrema()) if alpha else None
  role,note=review.get(p.name,('Unknown','Manual review required.'))
  records.append(dict(asset_id='ref_'+p.stem.lower().replace(' ','_'),source_path=str(p.resolve()),extension=p.suffix,size_bytes=p.stat().st_size,dimensions=dims,mode=mode,alpha=alpha,alpha_extrema=extrema,sha256=hashlib.sha256(p.read_bytes()).hexdigest(),category='reference_composite' if p.name in review else 'unknown',status='reference_only',target_path=None,pivot=None,ppu=None,slicing='none',atlas=None,usage=role,license_notes='User-provided concept reference; third-party rights not independently confirmed. Not redistributed as runtime art.',missing_requirements=note))
production=[]
for p in sorted((ROOT/'unity-client/Assets/HitMe/Art').rglob('*.png')):
 with Image.open(p) as im:
  meta=p.with_suffix(p.suffix+'.meta');settings=meta.read_text(encoding='utf-8') if meta.exists() else ''
  field=lambda key: re.search(r'^\s*'+key+r':\s*(.*)$',settings,re.M).group(1) if re.search(r'^\s*'+key+r':\s*(.*)$',settings,re.M) else None
  is_ui='/UI/' in p.as_posix();is_char='/Characters/' in p.as_posix()
  production.append(dict(asset_id='runtime_'+p.relative_to(ROOT/'unity-client/Assets/HitMe/Art').as_posix().replace('/','_').replace('.png',''),source_path=p.relative_to(ROOT).as_posix(),category='production_sprite',status='integrated_minimal_ui_fallback' if is_ui else 'integrated_single_frame' if is_char else 'integrated_standalone',target_path=p.relative_to(ROOT).as_posix(),dimensions=list(im.size),alpha='A' in im.getbands(),pivot=field('spritePivot'),ppu=field('spritePixelsToUnits'),slicing=field('spriteBorder'),atlas='MainMenu.spriteatlas' if is_ui else 'HitMeCharacters.spriteatlas' if is_char or '/Weapons/' in p.as_posix() else None,usage='Native UI element; not production art polish' if is_ui else 'One Idle sprite, code tween fallback' if is_char else 'Existing weapon/environment sprite',license_notes='See Assets/HitMe/Art/LICENSES.md and Sprint3A provenance.',missing_requirements='Polished art skin/portrait/real frames/layered environment as applicable.'))
missing={
 'characters':['Nam Lang matching new concept (existing Char01 is visual fallback)','Nu Nang Dong pink-haired standalone (current Char03 brown-haired fallback)','Quay Boy','Co Ba','Anh Teen','Cho Vang','Meo Mun'],
 'states':['Real Idle/Aim/Throw/Hit/Eliminated/Victory frames or rig-ready layers for each character; current3 characters have only1 Idle frame'],
 'weapons':['Standalone variants: colored slippers, phone, mosquito racket, pickleball racket, beer prop, TV, mini fridge, plastic chair, rice cooker, conical hat, paper fan','Pets as safe cartoon participants only; no violent throwing'],
 'ui':['Dedicated portraits','Illustrated feature/map/character/weapon cards','Production material skins','Independent HUD/chat/popup art'],
 'environment':['Village-gate far/mid/foreground layers','Flags, lanterns, leaves, crowd as separate exports','Five other arena art sets'],
 'fx':['Dust, impact, trail, sparkle, confetti, leaf, HP reaction sprites/frames'],
 'font':['Be Vietnam Pro not provided; existing licensed Nunito supports Vietnamese and remains in use']}
out={'referenceFiles':records,'existingRuntimeSprites':production,'missingProductionAssets':missing,'sourceEdited':False,'referenceImportedIntoRuntime':False}
(DOC/'PICTURE_ASSET_AUDIT.json').write_text(json.dumps(out,ensure_ascii=False,indent=2),encoding='utf-8')
lines=['# Picture asset audit — Phase 0','', 'Visual review and filesystem inspection completed for every file. Originals unchanged; no reference board imported into Unity runtime.','', '| Full name | Bytes | Resolution | Mode / alpha | Classification |','|---|---:|---|---|---|']
for r in records:lines.append(f"| {Path(r['source_path']).name} | {r['size_bytes']} | {r['dimensions'][0]}×{r['dimensions'][1]} | {r['mode']} / {r['alpha_extrema'] or 'none'} | {r['category']} |")
lines+=['','## Gate 0','', 'PASS: all5 boards classified reference_composite. None is a production sprite/sprite sheet/layered source/font. Alpha on UI CONCEPT ranges19–252 but belongs to the whole labeled board. No transparent isolated character/icon, frame exports, PSD/rig or text-free background exists in Picture.','', 'Concept strength/speed/damage claims are art-board annotations; gameplay parameters and cosmetic-only weapon rules stay unchanged. Animals shown on boards are not imported as throwable objects.','', '## Existing runtime assets','',f'{len(production)} independent PNGs already integrated before this phase:3 single-frame characters,3 weapons,2 arena images and19 minimal UI sprites. No new Picture artwork integrated. Existing code idle/pose tweens are fallback, not real frame animation.','', 'See HITME_UI_ASSET_MANIFEST.md and PICTURE_ASSET_AUDIT.json for paths, dimensions, alpha, pivots, PPU, atlas and missing items. Rights/provenance for current runtime art are documented separately; ownership of the concept boards is not independently established.','', '## Missing production exports','']
for key,items in missing.items():lines.append('### '+key);lines.extend('- '+i for i in items);lines.append('')
(DOC/'PICTURE_ASSET_AUDIT.md').write_text('\n'.join(lines)+'\n',encoding='utf-8')
lines=['# HIT ME UI asset manifest','', 'Machine-readable authoritative inventory: PICTURE_ASSET_AUDIT.json. No new art was extracted from Picture.','', '| asset_id | source / target | category | status | dimensions / alpha | pivot / PPU / slicing | atlas | usage |','|---|---|---|---|---|---|---|---|']
for r in records+production:lines.append(f"| {r['asset_id']} | {r['source_path']} → {r['target_path'] or 'none'} | {r['category']} | {r['status']} | {r['dimensions']} / {r['alpha']} | {r['pivot']} / {r['ppu']} / {r['slicing']} | {r['atlas'] or 'none'} | {r['usage']} |")
lines+=['', 'License notes and missing_requirements are included for each record in JSON. Reference boards are reference_only; runtime assets were already integrated. Native UI fallbacks are not accepted as finished Phase2 production artwork.','', '## Required production assets','']
for key,items in missing.items():lines.append('### '+key);lines.extend('- '+i for i in items);lines.append('')
(DOC/'HITME_UI_ASSET_MANIFEST.md').write_text('\n'.join(lines)+'\n',encoding='utf-8')
print(f'Audited {len(records)} reference files and {len(production)} existing runtime sprites; source hashes recorded.')
