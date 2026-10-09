from pathlib import Path
from PIL import Image
import json,re,hashlib,shutil
ROOT=Path(__file__).resolve().parents[1]
project=ROOT/'unity-client';art=project/'Assets/HitMe/Art'
derived={e['targetPath']:e for e in json.loads((ROOT/'docs/HITME_MASTER_ASSET_DERIVATIONS.json').read_text(encoding='utf-8'))}
previous={e['target'].replace('\\','/'):e for e in json.loads((ROOT/'docs/PICTURE_ASSET_INTEGRATION_MANIFEST.json').read_text(encoding='utf-8'))}
records=[]
for p in sorted(art.rglob('*.png')):
    im=Image.open(p);alpha=im.getchannel('A').getextrema() if 'A' in im.getbands() else None
    target=p.relative_to(project).as_posix();relative=p.relative_to(ROOT).as_posix();meta=p.with_suffix('.png.meta').read_text(encoding='utf-8') if p.with_suffix('.png.meta').exists() else ''
    ppu=re.search(r'spritePixelsToUnits: ([\d.]+)',meta);pivot=re.search(r'spritePivot: \{x: ([\d.]+), y: ([\d.]+)\}',meta)
    border=re.search(r'spriteBorder: \{x: ([\d.]+), y: ([\d.]+), z: ([\d.]+), w: ([\d.]+)\}',meta)
    source=derived.get(relative) or previous.get(relative);category=target.split('/Art/')[1].split('/')[0];production='/Production/' in target
    if '/Portraits/' in target:
        portrait_sources={'BlackBoy':'Characters/Char01_Player/Idle/frame_000.png','PinkGirl':'Production/Characters/pink-static-preview.png','LegacyBotMale':'Characters/Char02_BotMale/Idle/frame_000.png','LegacyBotFemale':'Characters/Char03_BotFemale/Idle/frame_000.png'}
        body=portrait_sources.get(p.stem,('Production/Animals/' if p.stem in ['GoldenDog','BlackCat'] else 'Production/Characters/')+p.name)
        source={'sourceFile':str(art/body),'cropMethod':'Upper 58 percent of source body; alpha preserved.'}
    if production:category=target.split('/Production/')[1].split('/')[0]
    referenced=production and p.name not in ['teal.png','parchment.png','Slipper.png','FryingPan.png']
    status='READY_TO_IMPORT' if production and not referenced else 'INTEGRATED' if referenced or ('/Characters/' in target and not production) or '/Weapons/' in target else 'FALLBACK'
    records.append(dict(assetId=target.removeprefix('Assets/HitMe/Art/').removesuffix('.png').replace('/','_'),category=category,sourceFile=source.get('sourceFile',source.get('source')) if source else str(p),targetPath=target,dimensions=list(im.size),transparent=alpha is not None and alpha[0]==0,alphaRange=alpha,pixelsPerUnit=float(ppu[1]) if ppu else None,pivot=[float(pivot[1]),float(pivot[2])] if pivot else None,slicing=[float(x) for x in border.groups()] if border else None,animationReady=False,status=status,dependencies=[source.get('sourceFile',source.get('source'))] if source else [],sha256=hashlib.sha256(p.read_bytes()).hexdigest(),notes='Static sprite; procedural motion is separate from real frame/rig animation. Imported-only assets explicitly listed in gap report.' if production else 'Existing art preserved; native UI icons/skins remain fallback where no concept export exists.'))
sources=[]
for p in sorted((ROOT/'Picture').iterdir()):
    if not p.is_file():continue
    im=Image.open(p);alpha=im.getchannel('A').getextrema() if 'A' in im.getbands() else None
    content={'MAIN MENU.png':'Village setting with baked branding, buttons, navigation and profile HUD.','Play in game.png':'Stone arena, spectators, actors and baked battle HUD.','UI CONCEPT.png':'Composite interface and feature artwork references.','UI KIT.png':'Composite UI skins, icons and labels.','Concept characters.png':'Composite human/animal/weapon design references.','Concept characters 2.png':'Additional composite character, animal and prop references.'}.get(p.name,'Additional artwork requiring visual audit.')
    sources.append(dict(filename=p.name,format=im.format,dimensions=list(im.size),alphaRange=alpha,transparent=alpha is not None and alpha[0]==0,status='REFERENCE_ONLY',extractionStatus='NEEDS_RECONSTRUCTION' if 'characters' in p.name.lower() or p.name=='MAIN MENU.png' else 'EXTRACTABLE',visualContent=content,extractionFeasibility='Opaque composite: independent transparent exports require reconstruction; safe rectangular skin/texture crops only where clean. No automatic rig/frame slicing.',intendedUnityUsage='Reference for separate PNGs, SO definitions and existing prefabs; never a whole interactive screen or animation sheet.',sha256=hashlib.sha256(p.read_bytes()).hexdigest(),usage='Reference only as a whole sheet. Independently cropped/reconstructed sprites are separate assets.'))
path=ROOT/'docs/HITME_PRODUCTION_ASSET_MANIFEST.json';backup=ROOT/'docs/HITME_PRODUCTION_ASSET_MANIFEST_PRE_MASTER.json'
if path.exists() and not backup.exists():shutil.copy2(path,backup)
path.write_text(json.dumps(dict(sourceOriginalsChanged=False,animationReadyMeaning='True only for real multi-frame or rig-ready assets; procedural motion does not qualify.',sources=sources,assets=records),ensure_ascii=False,indent=2),encoding='utf-8')
print(f'{len(sources)} sources audited; {len(records)} PNG assets with actual alpha/import metadata')
