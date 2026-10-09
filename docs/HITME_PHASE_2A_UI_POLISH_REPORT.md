# HIT ME — Phase 2A UI Polish

09/10/2026. Triển khai trên Unity 6000.6.4f1 hiện tại. Yêu cầu UNBLOCK mới cho phép UI/motion fallback tiếp tục độc lập với Phase 2B artwork.

## Implemented and verified

- Giữ 16 prefab độc lập, GUID/path cũ; tái sinh bằng UIKitSetup. Button có press/release, selected/disabled/loading, debounce pointer/submit 180 ms. Popup entrance/exit 160 ms khóa tương tác khi chuyển động; Reduced Motion xử lý tức thời.
- Main Menu giữ layout, navigation và trạng thái loading của controller cũ; popup nhân vật/vũ khí sử dụng CharacterSelection/WeaponSelection, ba sprite PNG có thật. Lựa chọn Bot/preview lưu riêng, tài khoản online vẫn theo server. Feature cards/bottom navigation giữ callback/layout hiện có và press feedback, không viết lại menu.
- Lobby sử dụng nút kit, SafeAreaAdapter và vị trí theo chiều cao vùng an toàn; inventory dùng WeaponCard, quantity/equipped thật từ server, callback equip cũ. Sửa CS0108 bằng đổi trường guestInput.
- Scene CharacterSelect sử dụng CharacterSelectionView + prefab selection. Battle giữ BattleView/Core, tích hợp PlayerHUD round/timer và nút Ready/Chat/Weapon từ kit; giữ adapter label cho kiểm thử/layout cũ.
- ChatPanel thực tế mở/đóng; input bị vô hiệu hóa vì chưa có chat transport/protocol. Không tuyên bố chat online đã hoạt động. Popup vũ khí thể hiện cosmetic policy.
- Nunito/Symbols/TMP và theme/atlas hiện có được giữ. Cài đặt Motion Low/Medium/High, Reduced Motion với khóa VI/EN.

## Tests và Web Build

- Phase 2A: EditMode 77/77; PlayMode 23/23, 0 skipped, có real local WS UI checks.
- Build Phase 2A Succeeded: 21,438,232 bytes, 0 errors, 4 warnings, 169.81 s. PHASE2A_WEB_BUILD.json là báo cáo BuildPipeline thực thi.
- HTTP listener 127.0.0.1:8791 PID 21644, WS 8788 PID 31352 đã kiểm tra thực tế; HTML/loader/data/framework/wasm đều 200. PHASE2A_HTTP_VERIFICATION.json.
- Browser mở localhost trực tiếp: Main Menu → Character Selection → Weapon Selection → Practice/Battle → ChatPanel. Console warn/error thu được rỗng trong smoke window. Hình browser đã xem trong công cụ, không có file browser PNG được lưu ở bước này.
- QA browser thấy mô tả thẻ bị tràn: đã bỏ mô tả lặp và giữ policy chung; tên vũ khí chuyển sang i18n; các sửa này được kiểm thử/build lại cùng bản Sprint 6A cuối. Browser inventory phát hiện title/selection của WeaponCard compact vượt hàng 48 px: đã sửa và kiểm tra lại cả test containment lẫn Web trực tiếp. Portrait HUD cũng đồng bộ lựa chọn nhân vật Bot, được test integration.
- Bộ regression cuối và screenshot responsive được ghi trong báo cáo Sprint 6A. Các XML Phase2A-* giữ riêng, không ghi đè lịch sử sprint cũ.

## Warnings

Warning ban đầu CS0108 NetworkLobbyView đã sửa. Bốn warning còn lại: pragma debug deprecated trong shader TMP; ba cảnh báo Bee/IL2CPP chia translation-unit lớn của TMP_TextParsingUtilities::.cctor, TextMeshPro::GenerateTextMesh, TextMeshProUGUI::GenerateTextMesh. Mức thấp/vendor/build performance; không sửa package. Tests có CS0618 API FindObjectsByType cũ trong UIKitFlowTests, không phải runtime build error.

## Implemented but not fully visually verified / pending manual

Responsive Unity checks/screenshots cover 360×800, 375×812, 390×844, 393×852, 402×874, 412×915, 430×932. Không thay thế kiểm thử Safari iPhone/Chrome Android thật. Chưa xác nhận keyboard/touch/full Web match trên hai browser client hoặc device FPS/memory. Chat transport còn thiếu. Các UI skins hiện có vẫn là native fallback, chưa phải premium artwork hoàn chỉnh.

## Files

Danh sách tạo/sửa đầy đủ (bao gồm .meta/prefab/profile/test/evidence) trong HITME_PHASE2A_6A_FILE_INVENTORY.txt. Nguồn chính: UIKitSetup, MainMenuView/MenuInteraction, NetworkLobbyView, CharacterSelectionView, CosmeticPreview, BattleView.UIKit/BattleView, HitMePanel, DebouncedButton, UIMotionController, localization vi/en, Phase2APolishTests.

## Waiting for artwork

Phase 2B = WAITING_FOR_ARTWORK. Không import/cắt concept sheet UI KIT.png hay năm board còn lại. Production manifest phân biệt 6 concept, 27 PNG runtime hiện có và exports thiếu. Không có sản phẩm shop/ranking/economy mới được giả lập.
