from pathlib import Path
import json
root=Path(__file__).resolve().parents[1]
tables={
'vi':{'mapSharedFloor':'FALLBACK • FLOOR CHUNG','mapLangQueBacBo':'Sân Đình Bắc Bộ','mapPhoCoHoiAn':'Phố Cổ Hội An','mapArtPartial':'Art tích hợp · PARTIAL','mapFallbackPreview':'FALLBACK: floor chung; chưa có artwork địa điểm.','mapOfflinePolicy':'Chỉ cấu hình map offline. Online chưa hỗ trợ đồng bộ map.','mapConfirm':'Xác nhận map offline','mapConfirmFallback':'Dùng fallback offline'},
'en':{'mapSharedFloor':'FALLBACK • SHARED FLOOR','mapLangQueBacBo':'Northern Village Courtyard','mapPhoCoHoiAn':'Hoi An Ancient Town','mapArtPartial':'Integrated art · PARTIAL','mapFallbackPreview':'FALLBACK: shared floor; location artwork missing.','mapOfflinePolicy':'Offline map config only. Online map sync is not supported.','mapConfirm':'Confirm offline map','mapConfirmFallback':'Use offline fallback'}}
for language,entries in tables.items():
 p=root/'unity-client/Assets/HitMe/Resources/Localization'/f'{language}.json';data=json.loads(p.read_text(encoding='utf-8-sig'));existing={e['key']:e for e in data['entries']}
 for key,value in entries.items():
  if key in existing:existing[key]['value']=value
  else:data['entries'].append({'key':key,'value':value})
 p.write_text(json.dumps(data,ensure_ascii=False,indent=2),encoding='utf-8')
