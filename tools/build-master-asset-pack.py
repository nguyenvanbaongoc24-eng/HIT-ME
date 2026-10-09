"""Explicit cell/cutout crops. Never alters source alpha or synthesizes frames."""
from pathlib import Path
from PIL import Image
import json,hashlib
ROOT=Path(__file__).resolve().parents[1]
GEN=Path('C:/Users/LENOVO/.codex/generated_images/01a119c8-2e1f-7b80-b157-90323eb08e7e')
ART=ROOT/'unity-client/Assets/HitMe/Art/Production'
items=[]
def crop(source,category,name,box):
    image=Image.open(source).convert('RGBA');cell=image.crop(box)
    bbox=cell.getchannel('A').point(lambda a:255 if a>128 else 0).getbbox()
    if bbox is None:raise ValueError(name+' has no opaque content')
    cell=cell.crop(bbox);path=ART/category/(name+'.png');path.parent.mkdir(parents=True,exist_ok=True);cell.save(path)
    items.append(dict(assetId=name,category=category,sourceFile=str(source),targetPath=path.relative_to(ROOT).as_posix(),crop=box,trim=bbox,dimensions=list(cell.size),transparent=cell.getchannel('A').getextrema()[0]==0,alphaRange=cell.getchannel('A').getextrema(),sha256=hashlib.sha256(path.read_bytes()).hexdigest(),provenance='AI-derived static illustration; actual alpha preserved; no frames/rig implied'))
    return path
source=GEN/'exec-bf3a069e-a58e-4eb4-a658-b818b057c56f.png';im=Image.open(source)
for i,name in enumerate(['BlondBoy','NonLaBoy','CapBoy','GoldenDog','BlackCat']):
    x=i%3;y=i//3;category='Animals' if i>2 else 'Characters'
    crop(source,category,name,(x*im.width//3,y*im.height//2,(x+1)*im.width//3,(y+1)*im.height//2))
source=GEN/'exec-21f872be-6aa1-44f8-95e7-f711dd23a161.png';im=Image.open(source)
for i,name in enumerate(['Slipper','Smartphone','MosquitoRacket','PickleballPaddle','BeerMug','FryingPan','Television','Refrigerator','PlasticStool']):
    if name in ['Slipper','FryingPan']:continue # Canonical good existing sprites are reused; no duplicate exports.
    x=i%3;y=i//3;crop(source,'Weapons',name,(x*im.width//3,y*im.height//3,(x+1)*im.width//3,(y+1)*im.height//3))
source=GEN/'exec-1b43430f-24fc-4ba1-b16b-6d5c794f3841.png'
# Manual observed bounds: preserve the audience group crossing nominal cell edges.
for name,box in [('FestivalFlag',(20,55,525,480)),('Lantern',(660,0,945,540)),('BanyanBranch',(975,55,1536,510)),('LotusMedallion',(20,490,510,980)),('Audience',(515,540,1105,970)),('TeaProps',(1108,565,1520,975))]:
    crop(source,'Environments',name,box)
source=GEN/'exec-c44b557e-b206-46d7-b265-48dd17ebe2fc.png'
im=Image.open(source);items=[e for e in items if e['assetId']!='PickleballPaddle'];crop(source,'Weapons','PickleballPaddle',(0,0,im.width,im.height))
(ROOT/'docs/HITME_MASTER_ASSET_DERIVATIONS.json').write_text(json.dumps(items,ensure_ascii=False,indent=2),encoding='utf-8')
print(f'{len(items)} independent static PNGs exported; source originals unchanged')
