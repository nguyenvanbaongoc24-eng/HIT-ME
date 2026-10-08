"""Inventory reviewable Unity source/config, excluding caches and built binaries."""
from pathlib import Path

root = Path(__file__).resolve().parents[1]
files = []
for folder in ["unity-client/Assets", "unity-client/Packages", "unity-client/ProjectSettings", "tools", "tests", "server", "shared"]:
    files.extend(p for p in (root / folder).rglob("*") if p.is_file() and "__pycache__" not in p.parts)
files.extend(root / p for p in ["AGENTS.md", "README.md", "unity-client/.gitignore"])
output = root / "docs/SPRINT1_FILES.tsv"
output.write_text("path\tbytes\n" + "".join(f"{p.relative_to(root).as_posix()}\t{p.stat().st_size}\n" for p in sorted(files)), encoding="utf-8")
print(f"Wrote {len(files)} source/config entries to {output}")
