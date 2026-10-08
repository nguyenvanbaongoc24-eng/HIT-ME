"""Check the bundled font cmap covers every localized character, not OS fallback."""
import json
from pathlib import Path
from fontTools.ttLib import TTFont

root = Path(__file__).resolve().parents[1] / "unity-client/Assets/HitMe/Resources"
text_map = TTFont(root / "Fonts/Nunito.ttf").getBestCmap()
symbols_map = TTFont(root / "Fonts/NotoSansSymbols2-Regular.ttf").getBestCmap()
keys = []
for language in ["vi", "en"]:
    table = json.loads((root / f"Localization/{language}.json").read_text(encoding="utf-8"))
    keys.append({entry["key"] for entry in table["entries"]})
    missing = {ord(c) for entry in table["entries"] for c in entry["value"] if not c.isspace() and ord(c) not in text_map}
    assert not missing, f"{language} missing code points: {sorted(missing)}"
assert keys[0] == keys[1], "VI/EN key sets differ"
assert 0x2665 in symbols_map, "Heart glyph missing"
print(f"PASS: {len(keys[0])} keys per language; all localized glyphs and U+2665 covered in bundled font files.")
