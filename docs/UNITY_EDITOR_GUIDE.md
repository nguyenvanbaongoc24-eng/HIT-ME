# Unity Foundation — hướng dẫn Editor và Web

## Mở project và Battle

1. Unity Hub > Projects > Add project from disk > chọn `D:/HIT ME/unity-client`.
2. Dùng Editor `6000.6.4f1` đã có tại `D:/App/UNITY/6000.6.4f1/Editor/Unity.exe`. Chờ Package Manager/import hoàn tất.
3. Mở `Assets/HitMe/Scenes/Battle.unity`, hoặc menu `HIT ME > Open Battle`.
4. Nhấn Play. Scene lưu entry component, UI được tạo lúc chạy; không có bản layout tĩnh giả làm screenshot trong scene.
5. Game View > Fixed Resolution: 390×844. Thêm 360×800, 412×915, 430×932 để kiểm tra. Console cần không có Error/Exception.
6. Mặc định xem bố cục đủ ba placeholder. Bấm SẴN SÀNG để thử placement: không có bot trên sân; chạm/kéo tạo vị trí chân và hướng. Ready chỉ khóa input của bạn, không kết thúc timer sớm.
7. Hết timer: nếu đã có position+aim thì hiện preview REVEAL. Sau đó dừng ở trạng thái chờ luật, không tính trúng, xuyên, máu, thứ hạng. Chưa đủ input cũng dừng, không tự ném về tâm.

Boot → MainMenu; MainMenu mở Battle/Lobby/CharacterSelect. Result là shell chưa nhận kết quả trận. Mọi scene đều có trong Build Settings.

## Cấu hình và asset

- Giá trị geometry: `Assets/HitMe/Resources/foundation-config.json`. Hitbox là vị trí chân, không phải tâm sprite. Elip dùng một hệ số scale cho cả hai trục.
- Chuỗi: `Resources/Localization/vi.json` và `en.json`; cùng bộ key. VI mặc định.
- Sprite: `Resources/Art/manifest.json`. Hai định nghĩa giới tính, sáu trạng thái và slot body/face/hair/outfit/weapon. Danh sách frame rỗng dùng khối PH có nhãn.
- Import sprite vào thư mục Resources riêng cho bản đồ/nhân vật đang dùng; Texture Type = Sprite (2D and UI), Filter Mode = Bilinear. Feet pivot ở dưới giữa. Ghi resource path không có phần mở rộng vào manifest. Loader foundation chọn frame đầu cho Idle; animation đầy đủ thuộc sprint sau.
- Tạo Sprite Atlas riêng cho nhóm asset dùng cùng màn. Chưa có sprite thật nên không tạo atlas rỗng rồi tuyên bố tối ưu.
- Font dùng bản static **HitMe Nunito** cho VI/EN và **Noto Sans Symbols 2** cho tim, đã nhúng vào build theo SIL OFL 1.1; không dựa vào font fallback của OS Editor. Runtime kiểm tra dấu Việt/tim đại diện, `tests/verify-fonts.py` kiểm tra toàn bộ chuỗi localization trong cmap. Giấy phép/nguồn nằm trong `Assets/HitMe/Art/LICENSES.md` và `Resources/Fonts`.
- Pipeline asset `Assets/HitMe/Settings/HitMe2DURP.asset` dùng `Renderer2D.asset`, HDR tắt/MSAA 1. UI Canvas Overlay; geometry không dùng Physics2D.

## Test

Window > General > Test Runner > EditMode: chạy FoundationTests. PlayMode: chạy BattleSmokeTests. Bộ smoke test mở Battle, kiểm tra bốn kích thước, text không bị cắt, nút trong safe area, giả lập inset trên 44px/dưới 34px và xác minh bot vẫn ẩn sau resize. Screenshot thực tế được ghi ở `docs/screenshots` nếu môi trường Editor có graphics hỗ trợ capture.

PlayMode còn kiểm tra chuỗi pointer input → ready → deadline và đặt sát bốn mép sân với inset giả lập. Nhãn giữ trong màn hình; HUD/placeholder điều chỉnh hiển thị để tránh nhau, không đổi vị trí chân hay hitbox logic.

Batch commands nằm trong README/tools/unity.ps1. Test XML mới là bằng chứng pass/fail; file test tồn tại không đồng nghĩa đã chạy qua. Thử notch giả lập không thay thế thiết bị có Dynamic Island. Capture test chờ frame GPU thực tế sau resize (tối đa hai giây), vì frame đầu của Editor batch có thể còn đen.

## Web

1. Hub > Installs > Editor menu > Add modules > Web Build Support (máy hiện tại đã có).
2. File > Build Profiles > Web > Switch Platform. Hoặc `tools/unity.ps1 BuildWeb`.
3. Menu `HIT ME > Build Web`. Output: `unity-client/Builds/Web`. Custom template: `Assets/WebGLTemplates/HitMePortrait`.
4. Development foundation để compression Disabled cho máy chủ local đơn giản; production compression/cache/header cần cấu hình và đo ở sprint deployment.
5. Chạy máy chủ HTTP, không mở index bằng file://:

```powershell
python -m http.server 8080 --directory unity-client/Builds/Web --bind 0.0.0.0
```

6. Desktop: `http://localhost:8080`. Điện thoại cùng Wi-Fi: IP LAN máy dev với port 8080, nếu firewall/network cho phép. Test Safari iOS và Chrome Android thực tế; chưa coi browser giả lập là nghiệm thu mobile.
7. Template có progress tải, canvas portrait, DPR cap 3 và thông báo context loss yêu cầu tải lại. Chưa có khôi phục state tự động.

Web orientation dùng canvas portrait; browser không bị ép orientation bằng API không được hỗ trợ. Canvas nền đầy, safe area HUD đọc Screen.safeArea. Cần kiểm tra inset browser thực tế vì OS/browser có thể cung cấp khác Editor.

## Phạm vi chưa hoàn thiện

Không phải trận offline hoàn chỉnh. Combat/pierce, timeout, cùng chết, nudge, sudden death và thời điểm protocol tiết lộ vẫn theo MISSING_REQUIREMENTS. Chưa art chibi thật, khán giả có sprite, animation sáu trạng thái, audio, chất lượng High/Medium/Low, projectile pool hoặc FPS/texture profiling. Không có online multiplayer.


## Sprint 2 — Chơi offline

Mở Boot hoặc MainMenu rồi Play → CHƠI VỚI BOT. Menu cho chọn 1–5 bot và Easy/Normal; mặc định 2 bot, Normal, seed 2026. Chạm vị trí trong sân, kéo ít nhất 12px để ngắm, bấm SẴN SÀNG. Khi tất cả người sống khóa hoặc hết 5s sẽ reveal, ném đồng thời và xử lý vòng. Người chết chuyển theo dõi; trận vẫn tiếp tục tới một người sống hoặc HÒA. Result có CHƠI LẠI/VỀ MENU.

Mở Battle trực tiếp giữ preview Sprint 1 có nhãn để các test cũ còn nguyên; bấm CHƠI VỚI BOT ở giữa sân để vào trận thật. Nút SẴN SÀNG trong preview vẫn chạy demo Sprint 1.

`Assets/HitMe/Resources/offline-match-config.json`: botCount 1–5, difficulty 0 Easy/1 Normal (2 Hard chưa bật), seed, throwSeconds, roundResultSeconds. `proposedTimeout: true` chỉ dùng đề xuất spec 2.5.1: thiếu input thì giữ vị trí cũ, không ném; false dừng timeout thiếu input chờ luật chính thức. Thông số arena/HP/timer vẫn ở foundation-config.json.

Kiểm tra giữ nguyên prototype/Sprint 1: `python tests/verify-prototype.py`, `python tests/verify-sprint2.py`. Kiểm tra font: `python tests/verify-fonts.py`. Test Unity và Web build dùng tools/unity.ps1 như phần trước; chạy từng Editor operation tuần tự. Web được serve bằng HTTP, không mở index.html trực tiếp.


## Workflow Sprint 3B

Project duy nhất: `D:/HIT ME/unity-client`, Editor `D:/App/UNITY/6000.6.4f1/Editor/Unity.exe`.

- Đóng Editor đang mở project trước khi batch test/build. Script kiểm tra UnityLockfile.
- Test: `./tools/unity.ps1 -Action EditMode` và `./tools/unity.ps1 -Action PlayMode`.
- Web: `./tools/build-web.ps1`; menu Editor **HIT ME → Build Web** cũng gọi `HitMeWebBuild.Build`.
- HTTP: `python tools/serve-web.py --port 8791`, mở `http://127.0.0.1:8791/`.
- Đo FPS debug: URL `http://127.0.0.1:8791/?performance=1`. Nhãn lấy từ frame counter Unity mỗi 2 giây; max ms là khoảng khung hình lớn nhất của cửa sổ đó, không phải p95.
- Mở Battle: Unity Hub → Add project `D:/HIT ME/unity-client` → dùng Editor 6000.6.4f1 → mở `Assets/HitMe/Scenes/Battle.unity` hoặc **HIT ME → Open Battle**. Play từ Battle mặc định là preview, bấm Chơi với bot để vào trận; Boot/MainMenu là luồng thông thường.
- Font: **HIT ME → Prepare Sprint 3B** bake Nunito/Noto Sans Symbols 2 vào static SDF; TMP Essential Resources lấy từ package UGUI đã cài, không tải một package mới.
- Art: thêm PNG riêng vào `Assets/HitMe/Art/Arenas/<MapID>/backdrop.png` và tùy chọn `sand.png`. Importer cấu hình Sprite; build cập nhật ArenaArtDefinition chỉ cho map thực sự có PNG. Không tách concept tổng hợp thành frame giả.
- Rollback hình học: đặt `arenaShape` thành `ellipse` trong `Assets/HitMe/Resources/foundation-config.json`, build lại. Rollback toàn sprint: xem commit bằng `git log --oneline`; dùng `git revert <commit Sprint 3B>` trên working tree sạch. Baseline trước Sprint 3B là `56ca54b`.
