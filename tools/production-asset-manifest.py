"""Read-only asset classification for Phase 2A / 6A. Never slices or edits images."""
from pathlib import Path
from PIL import Image
import json, hashlib
root=Path(__file__).resolve().parents[1]
previous=json.loads((root/'docs/PICTURE_ASSET_AUDIT.json').read_text(encoding='utf-8'))
records=[]
for path in sorted((root/'Picture').glob('*')):
 if not path.is_file(): continue
 with Image.open(path) as im:
  alpha='A' in im.getbands()
  records.append(dict(asset_id='ref_'+path.stem.replace(' ','_'),asset_name=path.name,source=str(path),destination='none: reference only',intended_use='Art direction, never runtime sprite sheet/UI',required_dimensions='separate production exports needed',source_dimensions=list(im.size),transparency=alpha,alpha_extrema=im.getchannel('A').getextrema() if alpha else None,pivot='N/A',ppu='N/A',sorting_layer='N/A',animation_requirements='real independent frames or rig layers',status='A_CONCEPT_REFERENCE',sha256=hashlib.sha256(path.read_bytes()).hexdigest()))
for item in previous['existingRuntimeSprites']:
 path=root/item['source_path']
 with Image.open(path) as im:
  alpha='A' in im.getbands(); bounds=im.getchannel('A').getbbox() if alpha else None
  char='/Characters/' in item['source_path']; weapon='/Weapons/' in item['source_path']; ui='/UI/' in item['source_path']
  area=(bounds[2]-bounds[0])*(bounds[3]-bounds[1])/(im.width*im.height) if bounds else 1
  cleanup=alpha and area<.45
  records.append(dict(asset_id=item['asset_id'],asset_name=path.stem,source=item['source_path'],destination=item['target_path'],intended_use=item['usage'],required_dimensions='keep aspect; 70–75 logical px character height' if char else 'keep aspect; no baked UI text',source_dimensions=list(im.size),transparency=alpha,pivot=item['pivot'],ppu=item['ppu'],sorting_layer='Default / Canvas ActorLayer sorted by foot Y' if char else 'Default / Canvas overlay UI' if ui else 'Default / Canvas environment/projectile order',animation_requirements='E: single-frame procedural fallback ready; no rig/multi-frame claim' if char else 'projectile spin/trail fallback ready' if weapon else 'UI tween ready' if ui else 'composite background, no separate prop/crowd movement',status='C_USABLE_CLEANUP_RECOMMENDED' if cleanup else 'B_EXISTING_USABLE',alpha_bounds=bounds,cleanup_notes='Large transparent padding; request trimmed source and repivot, do not auto-crop current foot asset' if cleanup else 'No cleanup blocker detected from metadata'))
missing={
 'characters':['NamLang_final','NuNangDong_pink','QuayBoy','CoBa','AnhTeen','ChoVang','MeoMun'],
 'weapons':['Phone','MosquitoRacket','PickleballRacket','TV','MiniFridge','WeaponVariants'],
 'environment':['VillageLayers','Flags','Lanterns','Leaves','CrowdFrames','Foreground'],
 'ui':['PremiumSkins','Portraits','IllustratedCards','HUDChatSkin'],
 'vfx':['DustFrames','ImpactFrames','TrailTexture','ConfettiFrames']}
for category,ids in missing.items():
 for name in ids:
  dest='unity-client/Assets/HitMe/Art/'+{'characters':'Characters','weapons':'Weapons','environment':'Environment','ui':'UI','vfx':'VFX'}[category]+'/'+name+'/'
  records.append(dict(asset_id='missing_'+name,asset_name=name,source='not supplied',destination=dest,intended_use=category,required_dimensions='256×256 frame export preferred for poses; preserve aspect/pivot, not a mandatory rescale' if category=='characters' else 'separate PNG/layer; dimensions per intended screen use',transparency=True,pivot='foot pivot metadata per character' if category=='characters' else 'center/grip/layer metadata',ppu=100,sorting_layer='Default / Canvas presentation order',animation_requirements='real frame sequence or rig layers for required states' if category in ('characters','environment','vfx') else 'single PNG supports fallback motion',status='D_WAITING_FOR_ARTWORK'))
out={'readOnly':True,'sourceImagesEdited':False,'conceptImported':False,'records':records}
(root/'docs/HITME_PRODUCTION_ASSET_MANIFEST.json').write_text(json.dumps(out,ensure_ascii=False,indent=2),encoding='utf-8')
lines=['# HIT ME production asset manifest','', '09/10/2026. Read-only audit: 6 Picture boards, 27 existing PNG sprites. No concept slicing/import or image cleanup performed. UI KIT.png is a labeled composite despite its printed production-pack claims.','', 'A = concept reference; B = usable existing sprite; C = usable but cleanup recommended; D = missing production export; E = animation readiness is recorded separately. E single-frame fallback does not mean rig or authored frame animation.','', '| ID / name | Source → destination | Use / required dimensions | Alpha / pivot / PPU | Sorting | Animation | Status |','|---|---|---|---|---|---|---|']
for r in records:
 lines.append(f"| {r['asset_id']} / {r['asset_name']} | {r['source']} → {r['destination']} | {r['intended_use']} / {r['required_dimensions']} | {r['transparency']} / {r['pivot']} / {r['ppu']} | {r['sorting_layer']} | {r['animation_requirements']} | {r['status']} |")
lines+=['','## Import / hot-swap workflow','', 'Existing PNG library: HIT ME/Characters/Import and Validate Library validates actual PNG frames, PPU, foot pivots, clips and atlas; existing CharacterDefinition references are updated and the sole CharacterVisual samples them. Replace a real PNG in the same asset path and reimport; no new gameplay controller is created. UI skins/icons use existing MainMenuAssetSetup/UIThemeDefinition and UIKitSetup. Preserve .meta GUIDs for replacements.','', 'New pets/rigs/environment layers remain WAITING_FOR_ARTWORK. CharacterDefinition frame/clip references and profiles can be bound once genuine exports exist; no full rig was manufactured. C cleanup is advisory, not a runtime blocker; existing pivots and gameplay positions stay unchanged.','', 'Phase 2A UI polish and Sprint 6A procedural visual fallback are authorized independently of Phase 2B art completion. Ambient particles/dust/trails are explicitly procedural effects, not production character or environment sprites.']
(root/'docs/HITME_PRODUCTION_ASSET_MANIFEST.md').write_text('\n'.join(lines)+'\n',encoding='utf-8')
print(f'Classified {len(records)} records; source images unchanged.')

