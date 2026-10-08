# HIT ME — Sprint 5 Main Menu

Triển khai 09/10/2026 (Asia/Bangkok), Unity 6000.6.4f1. MainMenu hiện dùng UI native có thể thao tác, không nhập ảnh concept nguyên tấm làm giao diện. Gameplay, CombatResolver, bot AI, server và Supabase không thay đổi trong Sprint 5. **Phần visual art thương mại chưa nghiệm thu: background sân đình, portrait và artwork feature card riêng vẫn thiếu.**

## Scene, prefab và kiến trúc

- Scene `Assets/HitMe/Scenes/MainMenu.unity` được giữ nguyên. FoundationMenu định tuyến scene đó tới `Prefabs/UI/MainMenu.prefab`; SceneEntry giữ EventSystem/InputSystemUIInputModule hiện có. Các scene khác vẫn tồn tại.
- MainMenuController quản lý session/pending intent/loading/cancel/error; MainMenuView tạo hierarchy UI, modal và binding. MenuNavigationService mang intent một lần tới NetworkLobbyView hiện có, không tạo Lobby/CharacterController thứ hai.
- UIThemeDefinition tại `Resources/UI/MainMenuTheme.asset` tham chiếu logo, atlas, sprite 9-slice, icon và prefab. Widget player/currency, primary actions, feature cards và bottom nav được tách thành các phần hierarchy trong View; chúng chưa phải các prefab con riêng biệt.
- SafeAreaAdapter dùng WebMobileBridge.SafeArea; CanvasScaler tham chiếu390×844, match width. Hero thu gọn theo chiều cao khả dụng, giữ header/CTA/footer. MenuInteraction dùng animation unscaled khoảng160ms, nhấn0.97, hover/focus nhẹ; Idle dùng CharacterVisual hiện có. Loading spinner nhẹ, badge chỉ hiện nếu server có quest Completed.

## Asset sử dụng và còn thiếu

Đã import19 PNG UI độc lập: 4 button frame, 13 line icon, logo và shadow. `tools/menu-assets.py` tái tạo asset native, không sinh nhân vật. MainMenuPngImporter cấu hình Sprite/100PPU, alpha, bilinear, no mipmaps; button border32px dùng Image.Type.Sliced. MainMenu.spriteatlas có padding4, rotation tắt. Logo độc lập dùng HitMe Nunito theo OFL. Text runtime dùng TextMeshPro NunitoSDF hiện có, hỗ trợ VI/EN.

Tái sử dụng 3 Idle chibi và vũ khí thật cùng backdrop Làng quê Bắc Bộ. Avatar tạm dùng Idle thumbnail; không giả frame hoặc portrait mới. Background hiện là arena art tái sử dụng, **chưa phải cổng đình nhiều layer/parallax theo concept**; có ghi chú fallback trong UI. Concept lưu ở docs/art-reference/Sprint5-MainMenu-Concept.png chỉ làm tham chiếu.

Cần sản xuất: far/mid/foreground sân đình riêng, portrait PNG, logo display được art-direct thêm, artwork cho bốn feature cards, texture giấy/gỗ/đá hoàn thiện. Icon/frame hiện là fallback tối giản thống nhất. Brief ở Art/Backgrounds/README.md và Art/UI/MainMenu/README.md. Chưa có audio asset hoàn chỉnh.

## Nút và điều hướng

| Thành phần | Hành vi thực tế |
|---|---|
| Chơi nhanh | Có session: gửi quick tới server hiện có rồi tới Lobby. Chưa kết nối: modal endpoint/tên khách, Connect rõ ràng. Có loading, hủy, timeout15s và lỗi; không giả matchmaking thành công. |
| Phòng riêng | Kết nối guest nếu cần, tới Lobby Create/Join hiện có. Không tự tạo phòng hoặc gửi lời mời ngoài game. |
| Avatar | Profile thật trong Lobby. Khi chưa kết nối hiển thị Khách/Chưa kết nối, coins “—”; không mock balance. |
| Currency | Chỉ coins/level/XP từ profile server; coins có nhãn thử nghiệm. Không thêm gem/material giả, rank giả hoặc nút mua “+”. |
| Nhiệm vụ | Daily/weekly quests hiện có, badge chỉ từ Completed thật. |
| Bản đồ / Sổ tay | Chọn 6 arena cho offline, lưu ArenaMaps.Selected. Năm map thiếu art được ghi rõ; online dùng Làng quê hiện tại. Sổ tay chưa có tiến trình khám phá vùng riêng. |
| Nhân vật | Preview3 sprite MVP; không giả rằng chọn preview thay avatar online/layer outfit. Túi đồ dẫn tới equip vũ khí cosmetic thật. CharacterSelect scene cũ giữ nguyên. |
| Bộ sưu tập / Túi đồ | Inventory thật qua Lobby, không tạo đồ hoặc recipe mới. |
| Luyện tập | BotCount1–5, Easy/Normal và Play With Bots. Không thêm Hard chưa hỗ trợ. Timeout proposal vẫn được ghi rõ. |
| Settings | Quality60/30FPS và chuyển VI/EN ngay. Audio ghi thiếu asset; không làm toggle âm thanh giả. |
| Home | Giữ Home active, đóng modal. |
| Inbox, Ranking, Shop, Community | Phản hồi “Sắp ra mắt”; không badge giả, leaderboard giả hay tab không phản hồi. |

## Kiểm thử thực thi

- Unity compile/configure thành công, theme/prefab/atlas load được.
- EditMode **71/71 pass**; PlayMode **17/17 pass**, gồm16 test cũ và test menu mới. XML riêng: Sprint5-EditMode-results.xml, Sprint5-PlayMode-results.xml.
- Test menu tải MainMenu scene, kiểm tra7 sizes, inset giả lập top59/bottom34px, bounds trong safe area, CTA không overlap, character không che CTA, touch targets main UI≥43.5px tại cỡ nhỏ, TMP không overflow/missing font, VI→EN, modal unavailable, BotCount, chọn map, Quick connect/cancel và binding profile fixture riêng trong test. Sau đó tải Battle; test Battle/offline/network cũ vẫn pass.
- Test đầu phát hiện clipping CTA và avatar trắng: đã sửa auto-sizing/fallback Unity-null; chạy lại PlayMode17/17. Sprite noise kéo dài dưới9-slice đã loại bỏ trước bản cuối.
- Ảnh Unity thực tế có inset: screenshots/Sprint5-Menu-SafeArea-{size}.png cho360×800,375×812,390×844,393×852,402×874,412×915,430×932. Không phải mockup.
- Web build/browser verification cập nhật ở cuối báo cáo sau khi có kết quả thực tế. Không coi test viewport là test iPhone thật.

## Cách kiểm tra

Unity Hub mở `D:/HIT ME/unity-client`, mở MainMenu scene rồi Play. Muốn cấu hình lại asset chọn HIT ME → Configure Main Menu Assets. Build bằng `./tools/build-web.ps1 -Sprint 5` (còn hỗ trợ -Sprint4 để giữ báo cáo cũ).

Chạy backend theo multiplayer-server/README.md; chạy `python ./tools/serve-web.py --port 8791`. Mở http://127.0.0.1:8791/ → kiểm tra Luyện tập, Settings, Bản đồ, Inventory, Chơi nhanh và Phòng riêng.

Trên điện thoại cần host Web qua HTTPS và endpoint WSS hợp lệ (127.0.0.1 trên điện thoại là chính điện thoại). Cấu hình allowlist origin đúng server. Kiểm tra portrait, notch/home indicator, bàn phím guest, touch và quay lại menu trên Safari iOS/Chrome Android. Chưa có thiết bị thật trong phiên này; không tuyên bố iPhone/Android đã pass.

Danh sách file Sprint5: SPRINT5_FILE_INVENTORY.json. Mã Phaser/apps/packages và server được giữ nguyên; dữ liệu SQLite thử nghiệm và build output không đưa vào Git.

## Web Build và giới hạn kiểm tra trình duyệt

Web Build Sprint5 đã chạy thực tế và **Succeeded**,21.397.238bytes,0errors,6warnings,249,01giây. Bằng chứng: SPRINT5_WEB_BUILD.json. Warning gồm CS0108 field name trong Lobby cũ, UAC1001 nullable Rect Override chỉ dùng runtime/test không serialize, TMP deprecated shader và compiler warnings. Không gọi đây là build sạch warning.

Server HTTP8791 và backend WS8788 đang chạy local. Tab browser của người dùng đang ở trang lỗi; cua.getTab bị chính sách công cụ chặn vì giao thức trang lỗi không được phép. Không thử bypass hoặc chuyển surface khác để né chặn. Đã yêu cầu người dùng tự reload URL hợp lệ. **Chưa thực thi browser navigation/ảnh Web Sprint5 hoặc kiểm tra Console Web cuối**; ảnh đính kèm là MainMenu thật từ Unity PlayMode, không phải ảnh browser. Unity PlayMode không có unexpected console log và Battle regression pass. Chưa test Safari/iPhone/Android thật.
