# HIT ME — Sprint 3B Unity Integration Report

Ngày thực thi: 08/10/2026. Đã triển khai trên project hiện tại, import/compile bằng Unity, chạy test, build Web và thao tác trên browser thực tế. Không thay Phaser/TypeScript; chưa coi toàn bộ art hoặc game production là hoàn tất.

## 1. Môi trường và nguồn yêu cầu

- Editor: `D:/App/UNITY/6000.6.4f1/Editor/Unity.exe`.
- Project duy nhất: `D:/HIT ME/unity-client`; ProjectVersion 6000.6.4f1 khớp Editor.
- URP 17.6.0, Input System 1.19.0, UGUI 2.6.0 (chứa TextMeshPro), Test Framework 1.8.0. Không cài lại Unity hoặc tạo project thứ hai.
- Web Build Support thực sự được sử dụng để build IL2CPP/WebAssembly.
- Editor hoạt động qua batchmode và PlayMode có GPU; kiểm tra log compiler/runtime. Không điều khiển Console GUI bằng desktop automation.
- Đã đọc AGENTS.md, ba tài liệu `HitMe_Spec_for_Antigravity.md`, `HitMe_UI_Prompts_for_Codex.md`, `HitMe_UI_Redesign_Prompt_1C.md`, MISSING_REQUIREMENTS và yêu cầu Sprint 3B (lưu tại `SPRINT_3B_REQUIREMENTS.md`). Sprint 3B thay yêu cầu hình học elip của 1C.
- Ban đầu chưa có Git: đã tạo baseline `56ca54b` trước khi sửa. Không có remote/push. Commit Sprint 3B lưu thay đổi để rollback (xem `git log --oneline`).

## 2. Hình học và gameplay

`IArenaGeometry` tách khỏi UnityEngine, gồm Bounds, PointInside, ClampPosition, RayBoundaryIntersection, ProjectileCollision, PlayerPlacementValidation. `RoundedRectangleArenaGeometry` dùng khoảng cách tới biên bo góc và giao tia với đoạn thẳng/cung tròn.

Mặc định runtime trong `foundation-config.json`: width **2000**, height **3500**, corner radius **300**, player radius **90**, projectile radius **30**. Clamp và ray projectile sử dụng biên co vào theo bán kính để toàn bộ vòng tròn nằm trong sân. UI mesh/mask/raycast, input, placement, bot sampler và resolver cùng dùng cấu hình này.

`EllipseArenaGeometry` và Geometry cũ còn nguyên để tương thích. FoundationConfig thuần C# mặc định elip phục vụ compatibility; JSON runtime chọn roundedRectangle. Đổi `arenaShape` về `ellipse` rồi build để rollback hình học.

Giữ nguyên: 3 HP, mỗi hit mất 1 HP, mục tiêu đầu tiên, tie ordinal ID, sát thương đồng thời, 0 người sống là hòa, reveal sau mọi người sống lock hoặc deadline. Không thêm nudge, sudden death, Hard AI, online hoặc Supabase. Bot chỉ đổi sampling vị trí theo biên mới; heuristic ngắm và capability không đọc hành động đối thủ vẫn giữ nguyên.

Timeout thiếu input vẫn là **đề xuất** có nhãn ở menu; không tự nâng thành luật đã xác nhận. Snapshot hiển thị được cache nhưng bất biến và riêng theo viewer; test kiểm tra invalidation không lộ hành động đối thủ.

## 3. UI và asset

- Battle scene và controller hiện có được mở rộng, không viết lại scene hoặc tạo CharacterController thứ hai.
- Sân chữ nhật bo góc, khán đài/nền lấp toàn bộ vùng ngoài sân. Tính scale và độ lệch theo safe area để chừa khoảng cao cho sprite sát biên; không dịch chân trong tọa độ logic. Khi inset lớn, sân hiển thị nhỏ hơn để bảo vệ HUD.
- Round/timer, avatar thumbnail, HP, VI/EN, chat, Ready và icon vũ khí thật. Portrait thumbnail dùng **sprite Idle hiện có**, chưa phải portrait PNG riêng.
- TextMeshProUGUI hiển thị chữ; HitMeText giữ API Text cũ cho adapters/tests nhưng mesh Text cũ rỗng. Static SDF Nunito và Noto Sans Symbols 2 có glyph tiếng Việt/♥; material dùng viền tối và tăng độ dày nét. TMP Essential Resources lấy từ package UGUI đã cài.
- Giữ 3 Idle PNG, 3 weapon PNG, atlas, animation phụ bằng code và CharacterVisual của Sprint 3A. Không sinh frame giả từ concept.
- Chọn 6 Map ID từ menu: LangQueBacBo, ChoVietNam, VinhHaLong, CoDoHue, ChoNoiCaiRang, TayBac. Chỉ load ArenaArtDefinition của map đang chọn. Importer và build tạo/cập nhật definition cho map **thực sự có backdrop.png**.
- **Art thực có:** Làng quê Bắc Bộ dùng lại backdrop/sand của Sprint 3A. Chưa có bộ environment mới khớp hoàn toàn concept rounded rectangle. Năm map khác hiện fallback có chữ “chưa có art”; README trong từng thư mục ghi PNG cần bổ sung. Không coi ảnh concept tổng hợp là asset đã import.

## 4. Web/mobile và hiệu năng

Web template dùng 100dvh, viewport-fit=cover, CSS env safe-area và bridge gửi inset chuẩn hóa sang Unity. Canvas touch-action:none, DPR cap 2, loading/progress và thông báo khi context lost/load lỗi. Không mở file://. Gzip có decompression fallback cho local HTTP host.

Preset Mượt 60 FPS / Tiết kiệm 30 FPS giới hạn frame rate; đã sửa SceneEntry để không reset preset khi vào Battle. Các lựa chọn map/preset lưu PlayerPrefs. Đây là FPS cap, không thay luật hoặc giảm tốc mô phỏng. Sprite Atlas, texture importer có compression, URP không HDR/MSAA, cache snapshot và các buffer UI/bot schedule giảm cấp phát. **Không tuyên bố zero GC hoặc đã đo draw calls/overdraw/texture memory.** Resources lựa chọn map tránh load tất cả map art ở runtime, nhưng các Resources vẫn được đóng gói trong Web data; chưa có Addressables/CDN download map riêng.

Đo bằng frame counter **Unity Update**, cửa sổ 2 giây, mỗi preset 10 mẫu (~20 giây) trên Codex in-app Chromium/Windows ở 390×844, sau startup:

| Preset | FPS trung bình từng cửa sổ | Khung hình dài nhất ghi nhận |
|---|---|---|
| 60 | 60.0–60.0 | 22.0 ms |
| 30 | 30.0–30.0 | 42.0 ms |

Số đo max ms không phải p95. Mẫu ngắn không chứng minh độ ổn định dài hạn hoặc hiệu năng điện thoại. JSON raw: `SPRINT3B_WEB_PERFORMANCE_HIGH.json`, `SPRINT3B_WEB_PERFORMANCE_LOW.json`. File LOW_INITIAL_BUG lưu lần đo phát hiện preset bị reset; **không** dùng nó làm kết quả 30 FPS cuối.

## 5. Kết quả kiểm thử thực thi

| Kiểm tra | Kết quả | Bằng chứng |
|---|---|---|
| EditMode | **70/70 pass**, 0 fail | unity-EditMode-results.xml |
| PlayMode cuối | **15/15 pass**, 0 fail | unity-PlayMode-results.xml |
| Test cũ | Giữ nguyên 51 EditMode / 11 PlayMode, đều pass trong suite | Không xóa/sửa nội dung test cũ |
| Geometry mới | Point/corner, clamp toàn vòng tròn, rays, projectile clearance, first hit, draw, timeout, reveal, seeded positions, snapshot privacy | RoundedArenaTests.cs |
| UI mới | 6 sizes + inset giả lập, chân/sprite/HUD, thiếu map fallback, SDF glyph/overflow, preset qua scene | Sprint3BIntegrationTests.cs |
| Web flow | MainMenu → Battle, drag/Ready, reveal/throw, HP/elimination → Result sau 21 vòng, Replay hoạt động | Browser screenshot Result và Battle |
| Web viewport | 360×800, 390×844, 393×852, 402×874, 412×915, 430×932 | SPRINT3B_WEB_VIEWPORTS.json + screenshots |
| Prototype | 26 file nguồn apps/packages/tests kiểm tra hash không đổi; Git diff các thư mục rỗng | SPRINT3B_PROTOTYPE_VERIFICATION.json |
| Build guard | Project đang khóa bị từ chối; Editor path không tồn tại trả exit 1 | SPRINT3B_VALIDATION_SUMMARY.json |

Không có lỗi compiler C# hoặc MissingReference trong lần test cuối. Browser đã kiểm tra console; kết quả cuối ghi riêng tại `SPRINT3B_BROWSER_VERIFICATION.json`. Các log Unity có cảnh báo dịch vụ/license/telemetry của môi trường; không coi chúng là bằng chứng lỗi gameplay.

## 6. Web build

- Unity BuildPipeline: **Succeeded**, errors **0**, warnings **1**.
- Tổng theo BuildReport: **21,283,266 bytes** (~21.28 MB). Build tăng dần cuối mất 11.4s; lần đầu biên dịch TMP/IL2CPP mất ~340s.
- Cảnh báo còn lại: pragma debug symbols deprecated trong shader TMP bundled. Các lần compile trước còn cảnh báo phương thức TMP lớn được tách thành C++ file riêng. Không sửa dependency shader chỉ để giấu cảnh báo.
- Output: `D:/HIT ME/unity-client/Builds/Web`. Loader thực dùng data/framework/wasm `.unityweb` và `Web.loader.js`.
- Scripts kiểm tra Editor, project, support module, khóa, exit code, **BuildReport mới** và các output có nội dung. Unity có thể giữ timestamp index.html khi template không đổi; không dùng timestamp index làm điều kiện thành công.
- Log: `docs/sprint3b-WebBuild.log`; summary: `SPRINT3B_WEB_BUILD.json`. Builds/Library/Logs không commit.

Chạy từ `D:/HIT ME`:

```powershell
./tools/unity.ps1 -Action EditMode
./tools/unity.ps1 -Action PlayMode
./tools/build-web.ps1
python tools/serve-web.py --port 8791
```

Mở `http://127.0.0.1:8791/`. Server bind loopback, không publish hoặc mở mạng ngoài. Thêm `?performance=1` để xem bộ đếm debug.

## 7. Mở Battle

Unity Hub → Add `D:/HIT ME/unity-client` → chọn 6000.6.4f1 → mở `Assets/HitMe/Scenes/Battle.unity` hoặc **HIT ME → Open Battle**. Play từ Battle là preview; bấm Chơi với bot để vào trận. Mở Boot/MainMenu để dùng luồng menu/chọn số bot, độ khó, map, preset. Đóng Editor trước khi chạy batch script.

## 8. File tạo/sửa và rollback

File mới chính: Core/ArenaGeometry.cs; UI/RoundedRectangleGraphic.cs, HitMeText.cs, ArenaMaps.cs, WebMobileBridge.cs; Editor/TextMeshProSetup.cs, HitMeWebBuild.cs; Plugins/WebGL/HitMeMobile.jslib; RoundedArenaTests.cs; Sprint3BIntegrationTests.cs; fonts NunitoSDF/SymbolsSDF; TMP essentials; 5 map README; tools/build-web.ps1, serve-web.py; yêu cầu, inventory, validation JSON và screenshot/report Sprint 3B.

File sửa chính: FoundationConfig, ArenaViewport, PlacementSession, OfflineMatch, CombatRules, BotController; BattleView/BattleView.Offline, FoundationMenu, OfflineResultView, SceneEntry, ArenaArtDefinition; ArenaArtSetup, FoundationSetup và asmdefs; foundation-config, localization VI/EN, Web template, ProjectSettings; tools/unity.ps1; MISSING_REQUIREMENTS và UNITY_EDITOR_GUIDE. Không sửa các scene .unity hoặc PNG character gốc.

Danh sách đầy đủ từng file, gồm .meta và TMP dependency assets: **SPRINT3B_FILE_INVENTORY.json**. Screenshot cũ được khôi phục từ baseline để báo cáo Sprint 1/2/3A không bị thay ảnh; ảnh mới lưu prefix Sprint3B. Log raw tồn tại local theo đường dẫn trên, bị .gitignore loại trừ.

Rollback toàn sprint bằng `git revert <commit Sprint 3B>` trên working tree sạch; baseline trước khi sửa là `56ca54b`. Không dùng reset --hard để xóa việc khác của người dùng. Xem commit mới nhất bằng `git log --oneline`.

## 9. Hạn chế và việc tiếp theo

1. Chưa test Safari iOS/Chrome Android trên máy thật, Dynamic Island thật, nhiệt/memory dài hạn, context-loss recovery thực, pinch/touch thật hoặc benchmark draw calls/overdraw/GC. Inset trong PlayMode là giả lập, browser test dùng mouse pointer.
2. Thiếu art production cho 5 map, background mới đồng bộ concept rounded, crowd/decor animation, portrait riêng, nhiều frame animation, audio và VFX đầy đủ. Có sprite thật nhưng **không hoàn tất toàn bộ phần hình ảnh**.
3. Chat là UI mock; Lobby/CharacterSelect chưa có luồng lựa chọn/tài khoản hoàn chỉnh. Shop, economy, ranking, friends và multiplayer ngoài phạm vi Sprint 3B.
4. Các luật còn mở trong MISSING_REQUIREMENTS giữ nguyên. Không tự suy đoán để hoàn thiện gameplay.
5. Cần giảm payload/download theo map bằng Addressables khi có đầy đủ art và chiến lược host; hiện gzip build chạy local, chưa deploy HTTPS production.

Ảnh browser thực tế: `screenshots/Sprint3B-Web-Battle-390x844.png`; Result: `screenshots/Sprint3B-Web-Result.png`. PlayMode có 6 ảnh `Sprint3B-<size>.png` với inset giả lập; browser có 6 ảnh `Sprint3B-Web-<size>.png`.


### Browser Console cuối

`SPRINT3B_BROWSER_VERIFICATION.json` ghi 0 error được thu thập, không có warning mới sau khi dùng runtime cuối. Hai warning FS synchronization trong log là từ các lần chạy cũ trước khi bật autoSyncPersistentDataPath; giữ nguyên dữ liệu lịch sử để đối chiếu. VI/EN đã được thao tác và kiểm tra bằng ảnh.
