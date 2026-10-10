from pathlib import Path
import json,re,hashlib
from PIL import Image,ImageDraw
root=Path(__file__).resolve().parents[1]
assets=root/'unity-client/Assets/HitMe'
guid={}
for p in assets.rglob('*.meta'):
 m=re.search(r'^guid: (\w+)',p.read_text(encoding='utf-8-sig'),re.M)
 if m:guid[m[1]]=Path(str(p)[:-5])
def image_info(p):
 im=Image.open(p);a=im.convert('RGBA').getchannel('A').getextrema()
 meta=Path(str(p)+'.meta');t=meta.read_text(encoding='utf-8-sig') if meta.exists() else ''
 return {'path':str(p.relative_to(root)),'size':im.size,'alphaRange':a,'spriteMode':re.findall(r'^\s+spriteMode: (\d+)',t),'sha256':hashlib.sha256(p.read_bytes()).hexdigest()}
out={'picture':[image_info(p) for p in (root/'Picture').rglob('*') if p.suffix.lower() in ['.png','.jpg','.jpeg']], 'definitions':[]}
for p in list((assets/'ScriptableObjects/Characters').glob('Char*.asset'))+list((assets/'Resources/Production').glob('*.asset')):
 t=p.read_text(encoding='utf-8-sig');m=re.search(r'^  characterId: (.+)',t,re.M)
 if not m:continue
 refs=[]
 for g in re.findall(r'guid: (\w+)',t):
  target=guid.get(g)
  if target and target.suffix.lower()=='.png':refs.append(image_info(target))
 prefab=re.search(r'visualPrefab: \{fileID: \d+, guid: (\w+)',t)
 out['definitions'].append({'id':m[1],'definition':str(p.relative_to(root)),'sprites':refs,'frameCounts':[len(re.findall('guid:',s.split('clip:')[0])) for s in t.split('  - state:')[1:]],'prefab':str(guid.get(prefab[1],'unresolved')) if prefab else None,'activeBattleCatalog':p.parent.name=='Characters'})
(root/'docs/STRICT_CHARACTER_ASSET_INVENTORY.json').write_text(json.dumps(out,indent=2,ensure_ascii=False),encoding='utf-8')
print('Picture',len(out['picture']),'definitions',len(out['definitions']))
canvas=Image.new('RGB',(1200,1100),'#17212b');draw=ImageDraw.Draw(canvas)
reference=Image.open(root/'Picture/Concept characters.png').convert('RGB');reference.thumbnail((1200,700));canvas.paste(reference,(0,30));draw.text((12,8),'PICTURE REFERENCE (whole composite, not imported as a sprite sheet)',fill='white')
for i,name in enumerate(['Char01_Player','Char02_BotMale','Char03_BotFemale']):
 im=Image.open(assets/f'Art/Characters/{name}/Idle/frame_000.png').convert('RGBA');im.thumbnail((280,320));canvas.paste(im,(60+i*390,760),im);draw.text((50+i*390,735),name+' ACTIVE RUNTIME SPRITE',fill='white')
canvas.save(root/'docs/screenshots/StrictProduction-20261010/Picture-vs-Runtime-Sprites.png')
