"""Reproducible, code-native UI assets. No character or environment art synthesis."""
from pathlib import Path
from PIL import Image, ImageDraw, ImageFont
import random

ROOT=Path(__file__).resolve().parents[1]/'unity-client/Assets/HitMe/Art/UI'
random.seed(5)
for folder in ['Buttons','Icons','MainMenu']: (ROOT/folder).mkdir(parents=True,exist_ok=True)
for name,fill,edge in [('paper',(243,193,108),(101,62,30)),('brick',(137,51,43),(57,27,25)),('ink',(30,43,42),(12,24,24)),('card',(48,57,50),(15,27,24))]:
    im=Image.new('RGBA',(128,128));d=ImageDraw.Draw(im)
    d.rounded_rectangle((3,6,125,127),radius=22,fill=(0,0,0,95))
    d.rounded_rectangle((2,2,125,122),radius=22,fill=edge+(255,))
    d.rounded_rectangle((6,6,121,117),radius=18,fill=fill+(255,),outline=(237,202,139,160),width=2)
    im.save(ROOT/'Buttons'/f'{name}.png')
im=Image.new('RGBA',(640,170));d=ImageDraw.Draw(im)
d.polygon([(12,18),(623,9),(608,146),(25,159)],fill=(53,39,28),outline=(210,161,75),width=4)
font=ImageFont.truetype(str(ROOT.parents[1]/'Resources/Fonts/Nunito.ttf'),115)
d.text((320,78),'HIT ME',font=font,anchor='mm',fill=(252,223,154),stroke_width=2,stroke_fill=(106,64,27))
im.save(ROOT/'MainMenu/logo.png')
im=Image.new('RGBA',(256,90));d=ImageDraw.Draw(im);d.ellipse((8,8,248,82),fill=(18,28,22,85));im.save(ROOT/'MainMenu/shadow.png')
# Cohesive line icons, independent PNGs with a consistent square canvas.
color=(246,221,172,255)
for name in ['home','shop','bag','map','people','settings','mail','trophy','quest','arrow','cancel','coin','practice']:
    im=Image.new('RGBA',(96,96));d=ImageDraw.Draw(im);kw={'fill':color,'width':6}
    if name=='home':
        d.line([(14,45),(48,14),(82,45)],**kw);d.line([(23,40),(23,81),(73,81),(73,40)],**kw);d.rectangle((40,57,55,82),fill=color)
    elif name=='shop':
        d.rectangle((20,43,76,80),outline=color,width=6);d.polygon([(16,40),(25,18),(72,18),(82,40)],outline=color,width=6);d.rectangle((42,58,57,80),outline=color,width=4)
    elif name=='bag':
        d.rounded_rectangle((19,33,77,83),10,outline=color,width=6);d.arc((32,12,64,52),180,360,fill=color,width=6)
    elif name=='map':
        d.line([(12,78),(12,27),(35,18),(61,29),(84,18),(84,69),(61,80),(35,69),(12,78)],**kw);d.line([(35,18),(35,69)],**kw);d.line([(61,29),(61,80)],**kw)
    elif name=='people':
        for x,y in [(48,23),(18,35),(78,35)]:d.ellipse((x-10,y-10,x+10,y+10),outline=color,width=5)
        d.arc((25,42,71,92),180,360,fill=color,width=7);d.arc((0,53,35,88),180,300,fill=color,width=6);d.arc((61,53,96,88),240,360,fill=color,width=6)
    elif name=='settings':
        d.ellipse((21,21,75,75),outline=color,width=11);d.ellipse((38,38,58,58),outline=color,width=5)
        for x,y,a,b in [(43,9,53,22),(43,74,53,87),(9,43,22,53),(74,43,87,53)]:d.rectangle((x,y,a,b),fill=color)
    elif name=='mail':
        d.rounded_rectangle((12,25,84,74),8,outline=color,width=6);d.line([(15,29),(48,53),(81,29)],**kw)
    elif name=='trophy':
        d.line([(27,17),(69,17),(65,51),(48,66),(31,51),(27,17)],**kw);d.line([(48,66),(48,80),(29,80),(67,80)],**kw);d.arc((9,21,43,57),75,280,fill=color,width=5);d.arc((53,21,87,57),260,100,fill=color,width=5)
    elif name=='quest':
        d.rounded_rectangle((20,14,76,82),8,outline=color,width=6);d.line([(31,47),(42,57),(64,33)],**kw)
    elif name=='arrow':d.line([(35,22),(61,48),(35,74)],**kw)
    elif name=='cancel':d.line([(23,23),(73,73)],**kw);d.line([(23,73),(73,23)],**kw)
    elif name=='coin':d.ellipse((12,12,84,84),outline=color,width=7);d.ellipse((23,23,73,73),outline=color,width=3);d.text((48,45),'$',font=ImageFont.truetype(str(ROOT.parents[1]/'Resources/Fonts/Nunito.ttf'),48),anchor='mm',fill=color)
    else:
        d.ellipse((29,14,58,42),outline=color,width=6);d.line([(44,43),(44,65),(26,84)],**kw);d.line([(44,63),(70,81)],**kw);d.line([(26,56),(62,45),(80,23)],**kw)
    im.resize((192,192)).save(ROOT/'Icons'/f'{name}.png')
print('Created native UI sprites; character and background art unchanged.')
