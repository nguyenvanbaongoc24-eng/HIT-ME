"""Verify the pre-sprint Phaser/TypeScript snapshot is byte-for-byte unchanged."""
import hashlib
import json
from pathlib import Path

root = Path(__file__).resolve().parents[1]
baseline = json.loads((root / "docs/SPRINT1_PROTOTYPE_BASELINE.json").read_text())
changed = []
for relative, expected in baseline.items():
    path = root / relative
    if not path.exists() or hashlib.sha256(path.read_bytes()).hexdigest() != expected:
        changed.append(relative)
current = {str(path.relative_to(root)) for folder in ["apps", "packages"] for path in (root / folder).rglob("*") if path.is_file()}
added = sorted(current - set(baseline))
if changed or added:
    raise SystemExit(f"FAIL: changed/deleted={changed}, added={added}")
print(f"PASS: all {len(baseline)} existing prototype files preserved byte-for-byte; no added files in apps/packages.")
