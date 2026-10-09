# HIT ME — Sprint 6A Motion

09/10/2026. Motion fallback dùng artwork hiện có; Phase 2B vẫn WAITING_FOR_ARTWORK.

## Implemented and verified

- Tái sử dụng CharacterVisual duy nhất. CharacterMotionProfile điều chỉnh bob ≤0.65 px, breathing ≤0.6%, Aim 3°, Throw anticipation/recovery 8°, Hit shake/flash và Victory bounce ≤1.6 px. Eliminated nghiêng/mờ nhẹ. Không tạo rig giả hoặc frame mới. Multi-frame thật (khi có) ưu tiên frame và bỏ procedural acting.
- MotionArtwork là Image con; root pivot/anchoredPosition, parent chân và hitbox không đổi. Facing vẫn flip theo state cũ; Fit dành khoảng 2 px cho motion, nhãn/HUD vẫn sử dụng layout owner.
- ProjectileVisualController chỉ đọc vị trí canvas do BattleView cập nhật: spin, trail, squash/stretch rất nhỏ. BattleView.Motion pool tái sử dụng projectile giữa vòng; bộ đệm trail 8 điểm và impact 8 bursts giới hạn, không Instantiate theo frame.
- Impact/dust procedural được kích hoạt khi phase RoundResult có resolution thật: offline Throws.End/Hit; online server end/damage. Không tính collision hoặc damage từ VFX. Đây là fallback effect đơn giản, không phải sprite VFX production.
- EnvironmentMotionController dùng mesh nhỏ với ambient hạt ở mép khán đài; không dịch nền PNG hay animate lá/cờ/khán giả đã bake. Low/Medium/High giới hạn 3/6/10 hạt, 2/4/6 trail; Reduced Motion tắt strength, ambient, trail và UI spinner.
- Popup/result entrance, nút press và HP text pulse; Result giữ outcome cũ, online reward feedback chỉ hiển thị ledger server xác nhận, không award thêm. Offline Result dùng kit button, locale hiện tại và SafeAreaAdapter.
- Có asset profile thật Resources/Motion/Character, Low, Medium, High. Editor: HIT ME/Configure Motion Profiles. Settings tại Main Menu; PlayerPrefs chỉ lưu visual preference.

## Executed validation

- EditMode 77/77, PlayMode 26/26, 0 skipped. Sprint6A-EditMode-results.xml và Sprint6A-PlayMode-results.xml.
- Backend npm --prefix multiplayer-server test: 16/16, 0 failures/skipped; gồm hai real WS clients complete match/reward, privacy, reconnect, equip/ledger, WSS CA. Không sửa source backend.
- Test mới kiểm tra root chân/hitbox không đổi, Reduced Motion cho mọi VisualState; projectile instance được tái sử dụng qua vòng; gọi nhiều impact không đổi HP; giới hạn quality. Test combat cũ vẫn kiểm tra Throw/Hit/Eliminated/Victory và real-clock precomputed flight.
- Kiểm thử đầu phát hiện tên GameObject projectile đã đổi làm adapter test Sprint 2 không tìm được; đã giữ lại ProjectilePH/NetworkProjectile rồi chạy lại toàn bộ PlayMode 26/26. Test dùng API obsolete GetInstanceID được thay bằng so sánh object thật, không thay luật.
- Unity screenshot thực tế: screenshots/Sprint6A-Battle-Actual.png; MainMenu/Battle các kích thước mang prefix Sprint6A. Layout-preview screenshot được ghi rõ là preview, không tiết lộ opponents trong Placement thật. Result-Actual.png là test-controlled offline victory 3 vòng; khác với trận Web spectator defeat 21 vòng đã quan sát.

## Web Build / browser

Build cuối Succeeded: 21,441,062 bytes, 0 errors, 4 warnings, 149.63 s. SPRINT6A_WEB_BUILD.json lấy từ BuildPipeline thực thi; scripts đã compile, project/Console checks qua bộ test.

HTTP localhost 8791 đã xác minh HTML/loader/data/framework/wasm đều 200, bytes/hash khớp build. Browser thực tế kiểm tra Main Menu, Character Selection, Motion settings/Reduced Motion toggle rồi khôi phục, guest connection, Lobby/leave room/inventory/back. Bản sửa compact inventory đã hiển thị đúng trên Web. Main Menu viewport 360×800, 390×844, 430×932 được xem trực tiếp; các size khác có Unity regression/screenshots. Console warn/error captured rỗng trong smoke window. Không có file browser screenshot lưu; các ảnh bàn giao là Unity capture thực tế.

Bot Web match chạy đến Result: THẤT BẠI, Bot 2 thắng, 21 vòng; Result → Về Menu hoạt động. Đây là quan sát trận offline sau khi player bị loại và spectate, không phải xác minh hai browser multiplayer hoặc thắng bằng chuỗi touch/aim.

## Warnings

Giữ nguyên phân loại TMP/Bee trong báo cáo Phase 2A; số warning cuối lấy từ SPRINT6A_WEB_BUILD.json, không tuyên bố warning-free. Không có MissingReference/NullReference trong các suite cuối đã pass. Browser Console sẽ ghi theo smoke window thực tế.

## Waiting for artwork / missing assets

- Chó vàng, mèo mun chưa có PNG/frame/rig riêng: chưa có animal motion thực tế.
- Chưa có production Phone/MosquitoRacket/PickleballRacket/TV/MiniFridge/variants; pipeline projectile có thể dùng FlightSprite khi bổ sung, không hiển thị vũ khí giả như đã hoàn thành.
- Flag/lantern/leaf/crowd/foreground layers, authored impact/dust/confetti frames, premium UI portraits/skin/cards thiếu. Prop sway/crowd rig chưa triển khai vì không có object/frame độc lập; camera parallax bỏ qua để giữ căn chỉnh arena và touch.
- Frame animation/rig đầy đủ cho ba người vẫn chờ exports thật. Đã hoạt động là code motion trên PNG Idle hiện có.

## Remaining manual verification

Safari iPhone/Chrome Android, touch/keyboard/safe area/reconnect/background, full online match Result trên Web và FPS/memory thiết bị thực tế chưa được chứng nhận. Counter desktop chỉ là quan sát, không phải nghiệm thu 60 FPS thiết bị. Reward/Result motion được integration/regression test; Result offline đã xem trên Web; visual QA reward/result online với hai browser vẫn pending.

## Files / cách mở và thay asset

HITME_PHASE2A_6A_FILE_INVENTORY.txt liệt kê file tạo/sửa. Mở project unity-client bằng Unity 6000.6.4f1, scene Assets/HitMe/Scenes/Battle.unity. Scene mặc định là layout preview; dùng MainMenu → Luyện tập với Bot để kiểm tra Placement thật. Giữ Core/resolver/bot/network logic.

CharacterAssetPipeline tiếp tục import real transparent PNG/frame, foot pivot, PPU100, atlas và CharacterDefinition. Thay file đúng path và giữ .meta rồi reimport/update definition; Web cần rebuild để nhận asset mới. Không có hệ thống remote download/hot update tự động. Manifest có danh sách exports Phase 2B.
