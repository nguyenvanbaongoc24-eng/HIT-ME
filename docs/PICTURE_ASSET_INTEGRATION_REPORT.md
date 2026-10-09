# Picture Asset Integration Report

Ngày: 09/10/2026. Triển khai trên Unity 6000.6.4f1 hiện tại; không tạo project.

## Implemented

- Main Menu: nền là derivative sạch từ MAIN MENU.png, không dùng ảnh concept nguyên tấm làm UI hoặc hotspot. Logo Image độc lập; tất cả labels/control vẫn TMP/Button VI/EN, routing và profile server giữ nguyên.
- UI Kit: 16 prefab độc lập cũ được tái sinh cùng GUID; button/panel có texture riêng và 9-slice. Nền/label riêng; native icon set còn fallback.
- Hai primary action cards dùng artwork nhỏ độc lập (Char01/village crop), title/description riêng và nút thật.
- Bốn Feature Cards có crop illustration sạch từ UI CONCEPT.png, không mang chữ baked của footer.
- Battle: backdrop AI chỉnh từ Play in game, không có fighters/HUD/hitbox/border baked; nền lát đá là crop thật và Image tiled trong mask hình học hiện có. Viền hiển thị khoảng 7 px, không sửa coordinate/input/collision/arena dimensions.
- Character preview tóc hồng: PNG riêng có alpha thật, hiển thị trong popup ghi rõ static AI-edited preview, không chọn/trang bị vào gameplay. Ba CharacterDefinition/CharacterVisual hiện có được giữ.
- WeaponDefinition hiện có chính là visual definition (sprite, gripPivot, visualSize). Bổ sung flightSpinDegreesPerSecond/impactSprite, projectile visual đọc spin cấu hình; mặc định 650 giữ motion hiện tại. Không thêm damage/collision/server weapon IDs.
- Motion foundation trước đó giữ: Idle/Aim/Throw/Hit/Eliminated/Victory procedural, projectile spin/trail, confirmed-impact feedback, environment fallback, reduced motion. Không có frame animation mới hoặc animal rig giả.

## Assets / classification

Không có artwork mới nào được gọi là Production-ready khi chưa duyệt art. Tất cả 15 PNG mới: Usable with limitations. Teal/parchment chỉ import, chưa bind runtime; 13 PNG còn lại dùng thực tế. Sprite import/atlas và sử dụng runtime đã được kiểm thử. Source map/dimensions/alpha/hash/crop: PICTURE_ASSET_INTEGRATION_MANIFEST.json.

- `unity-client\Assets\HitMe\Art\Production\MainMenu\village-clean.png` — AI edit; not an original-source crop; 941×1672, alpha None.
- `unity-client\Assets\HitMe\Art\Production\Environments\battle-clean.png` — AI edit; not an original-source crop; 967×1627, alpha None.
- `unity-client\Assets\HitMe\Art\Production\UI\logo.png` — AI edit; not an original-source crop; 1774×887, alpha [0, 255].
- `unity-client\Assets\HitMe\Art\Production\Characters\pink-static-preview.png` — AI edit; not an original-source crop; 1312×1199, alpha [0, 255].
- `unity-client\Assets\HitMe\Art\Production\UI\gold.png` — Crop of genuine generated blank UI texture; 655×196, alpha [0, 255].
- `unity-client\Assets\HitMe\Art\Production\UI\red.png` — Crop of genuine generated blank UI texture; 657×194, alpha [0, 255].
- `unity-client\Assets\HitMe\Art\Production\UI\teal.png` — Crop of genuine generated blank UI texture; 656×195, alpha [0, 255].
- `unity-client\Assets\HitMe\Art\Production\UI\card.png` — Crop of genuine generated blank UI texture; 649×254, alpha [0, 255].
- `unity-client\Assets\HitMe\Art\Production\UI\parchment.png` — Crop of genuine generated blank UI texture; 652×267, alpha [0, 255].
- `unity-client\Assets\HitMe\Art\Production\UI\nav.png` — Crop of genuine generated blank UI texture; 657×184, alpha [0, 255].
- `unity-client\Assets\HitMe\Art\Production\MainMenu\feature-map.png` — Opaque illustration crop; no transparency claim; 142×96, alpha [234, 250].
- `unity-client\Assets\HitMe\Art\Production\MainMenu\feature-character.png` — Opaque illustration crop; no transparency claim; 140×97, alpha [239, 251].
- `unity-client\Assets\HitMe\Art\Production\MainMenu\feature-collection.png` — Opaque illustration crop; no transparency claim; 137×96, alpha [242, 252].
- `unity-client\Assets\HitMe\Art\Production\MainMenu\feature-ranking.png` — Opaque illustration crop; no transparency claim; 140×96, alpha [233, 251].
- `unity-client\Assets\HitMe\Art\Production\Environments\stone-tile.png` — Opaque courtyard texture crop; 160×100, alpha None.

## Missing artwork / Blocked visual scope

- Các PNG/rig/frames animation sạch còn thiếu cho nhân vật vàng tóc, nón lá, mũ lưỡi trai, chó vàng và mèo mun. Các concept board vẫn Concept-only; không tách chung thành sprite sheet.
- Phone/vợt muỗi/vợt pickleball/lon bia/TV/tủ lạnh chưa có PNG sạch/cầm/bay/pivot được duyệt. Không đưa crop nền giấy vào Battle để giả asset hoàn chỉnh.
- Các loại vũ khí chưa có server gameplay định nghĩa không được thêm bằng suy đoán. Dép/chảo/vợt hiện tại giữ artwork và combat cũ.
- Top bar chỉ hiển thị coins/profile thực từ server; currency/level offline là “—”/chưa kết nối. Không tự thêm giá trị economy hoặc production ranking/shop.
- Năm map còn lại giữ trạng thái missing artwork; không sử dụng board chứa labels/arena actors làm background runtime.

## Tested

- Unity configure/compile thực tế thành công.
- EditMode: 78/78 pass (ARTWORK_EDITMODE_RESULTS.xml).
- PlayMode: 27/27 pass, live WS local (ARTWORK_PLAYMODE_RESULTS.xml). Kiểm tra navigation/safe area/VI-EN/placement privacy/motion/hitbox/match result và preview không đổi trang bị.
- Main Menu responsive: 360×800,375×812,390×844,393×852,402×874,412×915,430×932 trong test thật.
- Backend: 16/16 pass; gồm hai WS clients thực, parity C#, auth/resume/durable rewards/WSS test CA.
- Phaser/apps/packages/backend/Core source không sửa; GUID 16 UI prefab giữ nguyên.

## Web / Pending Manual Verification

Bản cuối: Web Build Succeeded, 30,426,202 bytes, 154.4537 s, 0 errors / 4 warnings. ARTWORK_WEB_BUILD.json. HTTP localhost thực tế 127.0.0.1:8791: index/loader/data/framework/wasm đều 200 và SHA256 khớp đĩa (ARTWORK_HTTP_CHECK.json). Browser thật: Main Menu VI/EN, Settings, Character Selection/static preview, Practice → Battle và bot spectator đã quan sát; Console error/warn log rỗng. Viewport Web 360×800,390×844,402×874,430×932 được kiểm tra bằng screenshot + DOM dimensions. Không coi thao tác drag trễ sau khi player đã bị loại là kiểm tra placement input thành công; placement privacy/input vẫn có PlayMode tests.

Warnings: 1 TMP shader pragma deprecated và 3 phương thức TMP lớn được IL2CPP tách file C++; build hoàn tất, không thấy runtime Console errors trong luồng đã quan sát. Không sửa package TMP. Build tăng từ khoảng 21.4 MB lên 30.4 MB do artwork mới; cần QA memory/download trên thiết bị trước production. Chưa có test thiết bị di động vật lý, FPS benchmark hoặc multiplayer hai trình duyệt trong lượt này.

## Screenshots thực tế

- screenshots/Artwork-MainMenu-SafeArea-390x844.png — MainMenu Unity PlayMode, safe area mô phỏng.
- screenshots/Artwork-Battle-Actual.png — Unity Battle, layout preview được ghi nhãn; không phải gameplay đối thủ bị lộ Placement.
- screenshots/Artwork-Static-Preview.png — preview một ảnh tĩnh, không có animation frames.

## Mở và tái cấu hình

Mở unity-client bằng Unity Hub 6000.6.4f1. HIT ME > Configure Production Artwork; mở Assets/HitMe/Scenes/MainMenu.unity hoặc Battle.unity. tools/extract-production-assets.py chỉ crop/copy, không thay Picture. Build entry HitMe.Editor.ProductionArtSetup.Build.

AI provenance/prompt constraints: PICTURE_ARTWORK_PROMPTS.md; công cụ built-in imagegen, không dùng CLI/API key. Giữ generated originals và Picture originals.
