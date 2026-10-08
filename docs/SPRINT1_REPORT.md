# HIT ME — Unity Foundation Sprint 1

Ngày bàn giao: 08/10/2026 (Asia/Bangkok).

## Kết quả triển khai

Đã tạo project Unity riêng bằng Editor thực tế tại `unity-client/`, sau đó cấu hình URP Renderer2D qua API Editor. Unity **6000.6.4f1** hoạt động, license cho phép chạy Editor; Web Build Support có sẵn. Các package pin theo template đi kèm Editor: URP 17.6.0, Input System 1.19.0, UGUI 2.6.0, Test Framework 1.8.0. Không dùng binary hoặc scene giả.

- Đủ scene Boot, MainMenu, Lobby, CharacterSelect, Battle, Result, được Editor tạo/save và đưa vào Build Settings.
- Portrait 390×844, một hệ số scale cho elip, HUD safe area và Web template có loading progress/DPR cap/context-loss notice.
- C# Core không phụ thuộc UnityEngine: config, Point, geometry, viewport và placement state machine.
- Elip 1000×1750 bán trục, player radius 90, projectile radius 30. Ray được normalize, xử lý biên/tangent/invalid input; kẹp vị trí theo xấp xỉ Prompt 1C.
- Battle preview: sân/tường/khán đài đầy màn bằng placeholder có nhãn, một player/hai bot, vòng hitbox ở chân, HUD vòng/timer/chân dung/3 tim/chat/ready/vũ khí.
- Placement demo: tap/drag, ngắm tới điểm tường, ready khóa cá nhân, bot ẩn tới deadline. Điều chỉnh nhãn/HUD khi sát mép, giữ nguyên tọa độ chân/hitbox. Không suy đoán resolve/timeout/pierce.
- VI/EN có bộ key tương ứng; font HitMe Nunito/Noto Sans Symbols 2 nhúng theo SIL OFL, tránh mất dấu/tim ở Unity Web do OS fallback chỉ có trong Editor. Manifest có nam/nữ, sáu trạng thái và slot mở rộng; frame còn rỗng. Chat/vũ khí chỉ thông báo placeholder, chưa online.

## Kiểm thử đã thực thi

| Kiểm tra | Kết quả và bằng chứng |
|---|---|
| Tạo/configure bằng Editor | Thành công, exit code 0; `unity-create.log`, `unity-Configure.log` |
| C# EditMode | **22/22 qua**, `unity-EditMode-results.xml` |
| Battle PlayMode | **3/3 qua**, `unity-PlayMode-results.xml`; load Battle, render thực, pointer input, ready/deadline, 4 viewport, notch giả lập và 4 mép sân |
| Console trong PlayMode | Test không ghi nhận Error/Exception ngoài dự kiến; `LogAssert.NoUnexpectedReceived()` qua |
| Giữ prototype | **48/48 file byte-for-byte nguyên vẹn**, không thêm file vào apps/packages; `python tests/verify-prototype.py` |
| Unicode/font | **22 khóa mỗi ngôn ngữ**, cmap phủ toàn bộ chuỗi VI/EN và U+2665; `python tests/verify-fonts.py`, font nhúng theo OFL |
| Web build source cuối | **Succeeded**, exit code 0; `unity-BuildWeb.log`, `HITME_WEB_BUILD: Succeeded, bytes=46626507`; output `unity-client/Builds/Web` |
| Browser chạy bản Web | Đã chạy trong Codex IAB (Chromium) qua HTTP localhost, 390×844, canvas 780×1688 (DPR 2); Boot → MainMenu → Battle, VI/tim hiển thị đủ, tap/drag/ready thực tế qua, bot ẩn khi placement |
| Safari iOS/Chrome Android thật, FPS/memory | Chưa thực hiện; không tuyên bố 60 FPS |

PlayMode screenshot là ảnh Game View được Unity ScreenCapture tạo, không phải mockup. Kiểm tra màu sân trên frame GPU thực giúp bắt trường hợp hierarchy tồn tại nhưng mesh chưa vẽ. Sau đổi resolution, capture chờ tối đa hai giây cho frame thực tế nếu GPU còn trả frame đen.

| Game View | Bề ngang elip ngoài | Chiều cao elip ngoài | Ảnh |
|---|---:|---:|---|
| 360×800 | 96% | 71,62% | `screenshots/Battle-360x800.png` |
| 390×844 | 96% | 73,54% | `screenshots/Battle-390x844.png` |
| 412×915 | 96% | 71,66% | `screenshots/Battle-412x915.png` |
| 430×932 | 96% | 73,43% | `screenshots/Battle-430x932.png` |

Thêm ảnh Editor placement: `screenshots/Battle-placement-390x844.png`. Ảnh Editor có đúng số pixel ghi trong tên, chưa coi là test DPR 2/3 hoặc ảnh từ thiết bị thật. Safe area được giả lập inset top 44px/bottom 34px, chưa phải Dynamic Island thật.

Ảnh **Web thực tế**: `screenshots/Battle-Web-390x844.jpg` và `screenshots/Battle-Web-placement-390x844.jpg`. Browser viewport là 390×844; DOM xác nhận backing canvas 780×1688 (DPR 2), ảnh browser được xuất ở kích thước viewport. Chưa kiểm tra DPR 3. Đã bỏ gọi Screen.orientation trên Web để tránh API lock không hỗ trợ; CSS giữ khung portrait. Dev logs còn lịch sử lỗi lock của bản trước, không có Error/Warning mới trong lượt kiểm tra bản cuối.

Build cuối có tổng size theo BuildReport **46.626.507 byte (~46,63 MB)** với compression Disabled. Đây là build dev để thử foundation, chưa tối ưu tải production; cần cấu hình compression/server và profiling trước phát hành.

## File đã tạo/thay đổi

Danh sách chính xác source/config (kèm số byte, bao gồm `.meta` Editor sinh): **`SPRINT1_FILES.tsv`**. Thư mục Library/Temp/Builds không được tính là source.

| Nhóm | File chính |
|---|---|
| Root | `AGENTS.md`, `README.md` |
| Project | `unity-client/Packages/manifest.json`, `packages-lock.json`, `ProjectSettings/*`, `.gitignore` |
| Core | `FoundationConfig.cs`, `Geometry.cs`, `ArenaViewport.cs`, `PlacementSession.cs`, `HitMe.Core.asmdef` |
| UI/runtime | `BattleView.cs`, `EllipseGraphic.cs`, `ArenaInput.cs`, `SceneEntry.cs`, `FoundationMenu.cs`, `HitMe.Runtime.asmdef` |
| Character/i18n | `CharacterManifest.cs`, `Strings.cs`, `FoundationFonts.cs`, `Resources/foundation-config.json`, `Resources/Art/manifest.json`, `Resources/Localization/{vi,en}.json`, `Resources/Fonts/*` và giấy phép OFL |
| Scene/URP | `Scenes/{Boot,MainMenu,Lobby,CharacterSelect,Battle,Result}.unity`, `Settings/{Renderer2D,HitMe2DURP}.asset`; global settings/profile do URP sinh |
| Editor | `FoundationSetup.cs`, `HitMe.Editor.asmdef` |
| Tests | `FoundationTests.cs`, `BattleSmokeTests.cs`, hai asmdef tests, `tests/verify-prototype.py`, `tests/verify-fonts.py` |
| Web/assets | `Assets/WebGLTemplates/HitMePortrait/index.html`, `Assets/HitMe/Art/LICENSES.md` |
| Tooling | `tools/unity.ps1`, `provision-foundation.py`, `foundation-inventory.py` |
| Reserved | README cho Arena/Combat/Networking/Audio, `server/README.md`, `shared/README.md`; chưa server/transport online |
| Docs mới | `UNITY_EDITOR_GUIDE.md`, `ACCEPTANCE_CHECKLIST.md`, `SPRINT1_REPORT.md`, `SPRINT1_FILES.tsv`, `SPRINT1_PROTOTYPE_BASELINE.json`, `Nunito-OFL.txt`, log/XML/screenshot thực tế |
| Docs cập nhật | `MISSING_REQUIREMENTS.md` cập nhật vị trí Editor và tiến độ, giữ nguyên các luật còn mở |

Không sửa source hoặc output prototype dưới `apps/`, `packages/`; không sửa root package.json/lockfile. Repository vẫn chưa có Git nên dùng hash baseline làm bằng chứng giữ nguyên.

## Cách mở Battle

Unity Hub > Add project from disk > `D:/HIT ME/unity-client` > mở bằng 6000.6.4f1 > mở `Assets/HitMe/Scenes/Battle.unity` > **Play**. Hoặc menu `HIT ME > Open Battle`. UI được dựng lúc chạy; Edit Mode scene lưu entry component.

Hướng dẫn test/import sprite/build Web và lệnh HTTP server: `UNITY_EDITOR_GUIDE.md`. Build pipeline/tooling không yêu cầu cài SDK .NET riêng ngoài Unity.

## Còn thiếu để thành game offline hoàn chỉnh

Foundation chạy được tới placement/reveal preview, chưa phải trận đấu hoàn chỉnh. Cần chốt số mục tiêu xuyên, timeout thiếu input, cùng chết, chồng vị trí/nudge và sudden death trước resolver thật. Bot hiện là actor/position placeholder, chưa AI chiến thuật. Khi thiếu position/aim, demo dừng ở AwaitingRules; không tự ném mặc định.

Chưa art chibi thật, sprite khán giả, animation sáu trạng thái, âm thanh, Sprite Atlas thật, pool projectile, chất lượng High/Medium/Low hoặc đo hiệu năng. Lobby/CharacterSelect/Result còn là navigation shells. Privacy protocol online và server-authoritative production thuộc sprint sau; foundation không gửi network message.

Checklist thủ công và thiết bị thật: `ACCEPTANCE_CHECKLIST.md`. Không có trở ngại cài Editor cần chủ dự án xử lý; các quyết định gameplay còn thiếu vẫn được giữ trong MISSING_REQUIREMENTS cho sprint tiếp theo.
