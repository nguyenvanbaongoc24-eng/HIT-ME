# HIT ME — Independent Unity UI Kit

09/10/2026, Asia/Bangkok. Checkpoint trước UI Kit: `7bbbf1e`. Unity 6000.6.4f1, project hiện có. **Gate UI Kit core: PASS. Production artwork polish and full-screen migration remain incomplete.**

## Implemented

16 Unity prefab thật trong `unity-client/Assets/HitMe/Resources/UI/Kit/`, không phải ảnh mockup. Nằm trong Resources để factory hiện có thể instantiate mà không thêm controller/navigation song song. Các prefab có RectTransform, Image, TMP, Button, script và trường serialized chỉnh được trong Inspector.

| Prefab | View/state API |
|---|---|
| HitMeButtonPrimary / Secondary / Tertiary | Normal, Pressed qua Button/MenuInteraction, Disabled, Loading |
| HitMeIconButton | Icon preserveAspect, Badge, disabled/loading |
| HitMeFeatureCard | Selected/Locked/Unavailable, title/detail/icon riêng |
| HitMeCharacterCard / WeaponCard / MapCard | View binding, Selected/Locked; chưa tự tạo sở hữu/mua/unlock |
| HitMeTopBar | Profile/resource value từ owner; chưa có data thì owner bind `—` |
| HitMeBottomNavigation | 5 widget; selection là presentation; owner bind navigation |
| HitMePopup | Open/Close, confirm/cancel UnityEvents |
| HitMePlayerHUD | BindHealth, BindBattle, Hit/Eliminated; không sửa HP/hitbox |
| HitMeChatPanel | TMP input, submitted event, availability/reason; owner bind transport |
| HitMeCharacterSelection / WeaponSelection / MapSelection | 3 card slot, selection group; owner bind catalog/service |

`HitMeWidget` tách view/state/binding khỏi gameplay/network; `HitMePanel` phát events và nhận data. Chuỗi dùng key VI/EN hoặc dữ liệu được owner bind; TMP text độc lập, không bake chữ/nút concept. Dùng UIThemeDefinition và font Nunito/Symbols hiện có, 9-slice sprites hiện có; chưa phải artwork UI thương mại mới. Kích thước/layout chỉnh trong prefab Inspector; font dùng asset chung.

**Tái sử dụng thật:** MainMenuView.B và NetworkLobbyView.B instantiate cùng HitMeButtonTertiary; giữ callback và text/data owner cũ. Lobby đổi button height 42→44 để cải thiện touch target; không đổi network commands, quyền sở hữu, thưởng, room logic hoặc luật gameplay. Các panel/card/HUD mới là reusable UI components, chưa tuyên bố đã thay toàn bộ Battle/selection/chat UI đang chạy bằng UI Kit.

## Showcase / cách mở

- `Assets/HitMe/Prefabs/UI/Showcase/UIShowcase.prefab`: gallery cuộn các widget states và panel.
- Kéo prefab vào scene kiểm tra có EventSystem, vào Play. UIShowcaseController tạo Canvas portrait lúc chạy; tránh Unity batchmode lưu Canvas scale 0. Không có Showcase trong danh sách scene build production.
- Editor menu `HIT ME/Configure UI Kit` tái tạo các prefab; đây là lệnh overwrite chính các generated kit assets, nên chỉnh generator nếu muốn giữ sửa đổi qua các lần configure. Không ghi đè gameplay/scenes/Phaser.
- API factory: `HitMeWidgetFactory.Create("HitMeButtonPrimary", parent)`, bind label/icon, `SetState`, `activated.AddListener`; owner giữ navigation và service riêng.
- Panel `confirmed`, `cancelled`, `submitted` không tự gửi network, mua hàng, phát thưởng hoặc chọn nhân vật gameplay. Owner cần bind khi thay màn hình tiếp theo.

## Tested

- UIKit-EditMode-results.xml: 77/77 pass.
- UIKit-PlayMode-results.xml: 21/21 pass, 0 skipped; có test UI kết nối server thật, tạo/rời private/quick và inventory.
- Backend rerun: 16/16 pass; hai WS clients chơi trận tới kết quả và reward ledger, WSS test CA. Không phải hai browser clients.
- Kiểm tra serialized references/fonts/sprites, locked/loading/disabled, parent transform không bị đổi, event popup/chat, selection controls và reuse MainMenu/Lobby.
- Kiểm tra render size của Showcase và ảnh `screenshots/UIKit-Showcase.png`; ban đầu phát hiện scale 0 và localization loading bị bake thành key khi configure chưa load Strings, đã sửa và chạy lại.
- Bộ regression MainMenu 7 viewport/safe area/VI/EN và gameplay/multiplayer cũ vẫn chạy trong full suites.
- Web Build cuối: Succeeded, 21,431,101 bytes, 0 errors, 5 warnings, 169.26 seconds; UIKIT_WEB_BUILD.json. Reload bản cuối qua HTTP, MainMenu/Practice/Connection/OnlineLobby dùng chung prefab button hoạt động; captured warn/error rỗng.

## Missing Assets / Pending Manual Verification

Skin/UI icons/portraits/cards thương mại theo concept chưa có; prefab dùng native PNG fallback hiện có. Character/weapon/map cards generic không đồng nghĩa mọi nhân vật/vũ khí/bản đồ đã có artwork riêng hoặc backend unlock.

ChatPanel chưa thay chat network thật; HUD chưa thay toàn bộ Battle HUD. Đây là component để tích hợp từng màn hình qua owner hiện có. Input keyboard/touch trên iPhone/Android thật, full Web match Result, background reconnect và FPS/memory thiết bị: pending; không coi desktop resize là device test.

Gate UI Kit core đã chốt sau test/build/browser cuối. Gate Artwork/Motion riêng, không gộp nghiệm thu.

