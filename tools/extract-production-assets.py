"""Explicit crops only. Originals and generated source images are preserved."""
from pathlib import Path
from PIL import Image
import hashlib, json, shutil
ROOT = Path(__file__).resolve().parents[1]
ART = ROOT / 'unity-client/Assets/HitMe/Art/Production'
GENERATED = Path('C:/Users/LENOVO/.codex/generated_images/01a119c8-2e1f-7b80-b157-90323eb08e7e')
entries = []
def record(source, target, operation, box=None):
    im = Image.open(target)
    entries.append(dict(source=str(source), target=str(target.relative_to(ROOT)), operation=operation, crop=box,
        width=im.width, height=im.height, alpha=im.getchannel('A').getextrema() if 'A' in im.getbands() else None,
        sha256=hashlib.sha256(target.read_bytes()).hexdigest(), status='Usable with limitations'))
def copy(name, destination):
    source=GENERATED/name; target=ART/destination; target.parent.mkdir(parents=True,exist_ok=True)
    shutil.copy2(source,target); record(source,target,'AI edit; not an original-source crop')
copy('exec-dd507b4d-1d93-4cb1-a1b8-98032192bd6f.png','MainMenu/village-clean.png')
copy('exec-7e6fde29-92c1-4b12-8cc9-2528038c27d2.png','Environments/battle-clean.png')
copy('exec-824bb3d6-6610-4706-accf-a21ae253ede0.png','UI/logo.png')
copy('exec-2572e7bb-ccd6-4c16-90c9-26139257d335.png','Characters/pink-static-preview.png')
source=GENERATED/'exec-e89dd1f6-06d0-4a1c-8360-95c20643b87b.png'; sheet=Image.open(source).convert('RGBA')
for i,name in enumerate(['gold','red','teal','card','parchment','nav']):
    x=i%3; y=i//3; box=(x*sheet.width//3,y*sheet.height//2,(x+1)*sheet.width//3,(y+1)*sheet.height//2)
    cell=sheet.crop(box); bounds=cell.getchannel('A').point(lambda a: 255 if a>128 else 0).getbbox()
    if bounds: cell=cell.crop(bounds)
    target=ART/'UI'/f'{name}.png';cell.save(target);record(source,target,'Crop of genuine generated blank UI texture',box)
for name,box in [('map',(439,434,581,530)),('character',(596,434,736,531)),('collection',(750,435,887,531)),('ranking',(901,435,1041,531))]:
    source=ROOT/'Picture/UI CONCEPT.png';target=ART/'MainMenu'/f'feature-{name}.png'
    Image.open(source).crop(box).save(target);record(source,target,'Opaque illustration crop; no transparency claim',box)
source=ROOT/'Picture/Play in game.png';target=ART/'Environments/stone-tile.png';box=(300,550,460,650)
Image.open(source).crop(box).save(target);record(source,target,'Opaque courtyard texture crop',box)
(ROOT/'docs/PICTURE_ASSET_INTEGRATION_MANIFEST.json').write_text(json.dumps(entries,ensure_ascii=False,indent=2),encoding='utf-8')
print(json.dumps(entries,ensure_ascii=False,indent=2))
