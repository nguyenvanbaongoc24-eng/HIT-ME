from pathlib import Path
import json,re,hashlib
from PIL import Image
root=Path(__file__).resolve().parents[1];assets=root/'unity-client/Assets/HitMe'
guid_paths={}
for meta in assets.rglob('*.meta'):
 match=re.search(r'^guid: (\w+)',meta.read_text(encoding='utf-8'),re.M)
 if match:guid_paths[match[1]]=meta.with_suffix('').relative_to(root).as_posix()
def reference(text,key):
 match=re.search(r'^  '+key+r':.*guid: (\w+)',text,re.M)
 return guid_paths.get(match[1]) if match else None
maps=[]
for id in ['LangQueBacBo','PhoCoHoiAn','VinhHaLong']:
 p=assets/'Resources/Arenas'/f'{id}.asset';text=p.read_text(encoding='utf-8');items=[]
 for key,field in [('previewSprite','previewSprite'),('floorSprite','sand'),('borderSprite','borderSprite'),('centerDecoration','centerDecoration'),('backgroundComposite','backdrop'),('backgroundFar','backgroundFar'),('backgroundMid','backgroundMid'),('foregroundProps','foregroundProps'),('shadowOverlay','shadowOverlay')]:
  path=reference(text,field);entry={'assetId':id+'_'+key,'targetPath':path,'status':'VERIFIED' if path else 'MISSING','usage':'Shared fallback floor/decoration, not location-specific artwork.' if id!='LangQueBacBo' and path else 'Independent imported art; missing layers not inferred.'}
  if path:
   image=Image.open(root/path);entry.update(dimensions=list(image.size),transparent='A' in image.getbands() and image.getchannel('A').getextrema()[0]==0,sha256=hashlib.sha256((root/path).read_bytes()).hexdigest())
  items.append(entry)
 maps.append({'mapId':id,'definition':p.relative_to(root).as_posix(),'status':'PARTIAL' if id=='LangQueBacBo' else 'BLOCKED_BY_ARTWORK','offlineFallbackSelectable':True,'onlineMapSync':'MISSING: client uses default map; local choices not sent to server','assets':items,'isolatedDecorations':['FestivalFlag','BanyanBranch','Lantern','Audience'] if id=='LangQueBacBo' else [],'motionProfile':reference(text,'motionProfile'),'ambientEffectProfile':reference(text,'ambientEffectProfile'),'audioProfile':reference(text,'audioProfile'),'missingMotionSprites':['independent far/mid layers','dust sprite/tree shadow layers'] if id=='LangQueBacBo' else ['lantern glow/falling leaf/background walkers'] if id=='PhoCoHoiAn' else ['water/wave layers','boat sprites','bird sprites','cloud sprites']})
sources=[]
for p in sorted((root/'Picture').iterdir()):
 if not p.is_file():continue
 im=Image.open(p);sources.append({'filename':p.name,'format':im.format,'dimensions':list(im.size),'alphaRange':im.getchannel('A').getextrema() if 'A' in im.getbands() else None,'status':'REFERENCE_ONLY','sha256':hashlib.sha256(p.read_bytes()).hexdigest()})
inventory={'png':len(list(assets.rglob('*.png'))),'prefabs':len(list(assets.rglob('*.prefab'))),'animationClips':len(list(assets.rglob('*.anim'))),'scenes':[p.name for p in (assets/'Scenes').glob('*.unity')],'uiKitPrefabs':[p.name for p in (assets/'Resources/UI/Kit').glob('*.prefab')]}
(root/'docs/HITME_MAP_ASSET_MANIFEST.json').write_text(json.dumps({'inventory':inventory,'sourceArtwork':sources,'maps':maps,'note':'Verified denotes actual asset/import metadata, not completed map artwork or real-device readiness.'},ensure_ascii=False,indent=2),encoding='utf-8')
print(json.dumps(inventory,ensure_ascii=False))
