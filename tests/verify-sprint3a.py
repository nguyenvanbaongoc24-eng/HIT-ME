import hashlib,json,pathlib
root=pathlib.Path(__file__).resolve().parents[1]
baseline=json.loads((root/'docs/SPRINT3A_BASELINE.json').read_text(encoding='utf-8'))
allowed={'unity-client/Assets/HitMe/Scripts/UI/BattleView.cs','unity-client/Assets/HitMe/Scripts/UI/BattleView.Offline.cs','unity-client/Assets/HitMe/Scripts/UI/CharacterPresentation.cs','unity-client/Assets/HitMe/Art/LICENSES.md'}
changed=[]
for path,digest in baseline.items():
 p=root/path
 assert p.is_file(),f'Deleted Sprint 2 file: {path}'
 if hashlib.sha256(p.read_bytes()).hexdigest()!=digest:
  assert path in allowed,f'Unrelated change: {path}'
  changed.append(path)
print(f'PASS: no Sprint 2 files deleted; only {len(changed)} intended existing files changed; original tests, scenes, core, settings and root package files preserved.')
new=[]
for folder in ['unity-client/Assets','unity-client/Packages','unity-client/ProjectSettings','tests','tools']:
 for p in (root/folder).rglob('*'):
  if p.is_file():
   path=p.relative_to(root).as_posix()
   if path not in baseline:new.append(path)
rows=['status\tpath\tbytes']+[f'modified\t{p}\t{(root/p).stat().st_size}' for p in sorted(changed)]+[f'created\t{p}\t{(root/p).stat().st_size}' for p in sorted(new)]
(root/'docs/SPRINT3A_FILES.tsv').write_text('\n'.join(rows)+'\n',encoding='utf-8')
