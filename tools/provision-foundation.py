"""Create source-only resources for the isolated Unity project. Never writes apps/ or packages/."""
import json
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1] / "unity-client" / "Assets" / "HitMe"

def write(relative, content):
    path = ROOT / relative
    path.parent.mkdir(parents=True, exist_ok=True)
    path.write_text(json.dumps(content, indent=2, ensure_ascii=False) + "\n", encoding="utf-8")

write("Resources/foundation-config.json", dict(arenaA=1000, arenaB=1750, playerRadius=90,
    projectileRadius=30, placementSeconds=5, maxHp=3, minPlayers=2, maxPlayers=6,
    wallLogic=140, hudFraction=.10, aimDragPixels=12, revealSeconds=.3, referenceWidth=390, referenceHeight=844))

vi = dict(arenaPlaceholder="SÂN CÁT • PLACEHOLDER", you="Bạn", bot="Bot", round="VÒNG", settings="VI / EN",
    chat="CHAT", ready="SẴN SÀNG", weapon="VŨ KHÍ\nPH", preview="XEM BỐ CỤC • Bấm SẴN SÀNG để thử đặt vị trí",
    hint="Chạm để đặt vị trí, kéo để ngắm", locked="ĐÃ KHÓA", reveal="LỘ DIỆN • PLACEHOLDER",
    awaitingRules="Demo kết thúc • Chờ xác nhận luật xử lý lượt", retry="THỬ LẠI", close="ĐÓNG",
    chatMock="Chat placeholder • Chưa kết nối online", weaponMock="Vũ khí placeholder • Chưa có sprite",
    sceneMainMenu="Menu chính", sceneLobby="Phòng chờ • Placeholder", sceneCharacterSelect="Chọn nhân vật • Placeholder",
    sceneResult="Kết quả • Chưa xử lý trận", openBattle="MỞ BATTLE FOUNDATION")
en = dict(arenaPlaceholder="SAND • PLACEHOLDER", you="You", bot="Bot", round="ROUND", settings="VI / EN",
    chat="CHAT", ready="READY", weapon="WEAPON\nPH", preview="LAYOUT PREVIEW • Press READY to try placement",
    hint="Tap to place, drag to aim", locked="LOCKED", reveal="REVEAL • PLACEHOLDER",
    awaitingRules="Demo ended • Round rules await confirmation", retry="RETRY", close="CLOSE",
    chatMock="Chat placeholder • Offline", weaponMock="Weapon placeholder • Missing sprite",
    sceneMainMenu="Main menu", sceneLobby="Lobby • Placeholder", sceneCharacterSelect="Characters • Placeholder",
    sceneResult="Result • Match not resolved", openBattle="OPEN BATTLE FOUNDATION")
assert vi.keys() == en.keys()
for language, entries in [("vi", vi), ("en", en)]:
    write(f"Resources/Localization/{language}.json", dict(entries=[dict(key=k, value=v) for k, v in entries.items()]))
write("Resources/Art/manifest.json", dict(version=1, characters=[dict(id=f"{g}-placeholder", gender=g,
    customizationSlots=["body", "face", "hair", "outfit", "weapon"], feetAnchor=dict(x=.5, y=0),
    states=[dict(state=s, resourcePaths=[]) for s in ["Idle", "Aim", "Throw", "Hit", "Dead", "Win"]]) for g in ["male", "female"]]))
for module in ["Arena", "Combat", "Networking", "Audio"]:
    path = ROOT / "Scripts" / module / "README.md"
    path.parent.mkdir(parents=True, exist_ok=True)
    path.write_text(f"# {module}\n\nFoundation module reserved for the next authorized sprint. Geometry/state live in Core. No online transport or inferred combat rules.\n", encoding="utf-8")

print("Created config, vi/en strings, manifest and reserved module notes.")
