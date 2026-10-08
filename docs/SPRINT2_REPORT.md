# HIT ME — Sprint 2 Gameplay Core & Offline Bot

Ngày bàn giao: 08/10/2026. Unity 6000.6.4f1 đã compile, chạy test/render và build Web thật. Project 2D URP 17.6.0 và sáu scene Sprint 1 được giữ nguyên. Web Build Support có tại D:/App/UNITY/6000.6.4f1/Editor/Data/PlaybackEngines/WebGLSupport.

## Đã triển khai

- Offline match một người + 1–5 bot, mặc định hai bot. Menu chọn số bot/Easy/Normal, seed 2026 trong offline-match-config.json. Hard là điểm mở rộng chưa bật.
- C# thuần tách CombatRules, OfflineMatch, GameEvent/public views và BotController; không dùng Unity physics hay animation để quyết định hit/damage. Placement/Aim/Locked là trách nhiệm input, reveal tất cả người sống đã khóa hoặc deadline, throw đồng thời, resolve HP, vòng tiếp hoặc Result.
- Snapshot bất biến tại reveal; chỉ mục tiêu đầu tiên theo projected t nhận 1 damage, tie bằng ordinal ID. Cộng tất cả damage trước khi clamp HP về 0. HP khởi tạo 3, loại sau vòng. Kết quả có throws Hit/Miss/Target/Damage, Health HPBefore/HPAfter/Eliminated và Outcome/Winner. Một người còn sống thắng; tất cả chết là HÒA.
- Easy chọn vị trí/hướng ngẫu nhiên hợp lệ; Normal ngắm lịch sử vị trí công khai của vòng trước. Bot context không chứa match/current opponent inputs. PRNG uint có seed; test lặp lại trong môi trường xác định. Người chết và input khóa bị chặn.
- Mở rộng cùng BattleView/Sprint 1 arena: tim thực, round/timer, ready lock, projectile đồng thời, hit/miss, loại/X, theo dõi, sáu pose placeholder Idle/Aim/Throw/Hit/Dead/Win. Giữ actor bị loại trong RoundResult cho visual, loại khỏi vòng sau.
- Responsive bốn viewport, safe area giả lập top44/bottom34, input pointer đầu tiên duy nhất, đường ngắm tới tường, bot ẩn trước reveal. Giữ chân/hitbox logic khi co chiều cao placeholder sát HUD. Nhãn sân trang trí ẩn trong match để tránh chồng nhân vật. Nameplate chuyển tới khoảng trống; nếu không đủ chỗ, HP vẫn có ở roster.
- Result scene có Victory/Defeat/Draw, tên người thắng/HÒA, số vòng, replay và menu. Replay tạo match/controller/history mới với 3 HP, round1 và chưa có input. Không thêm online, economy, ranking, auth, shop hoặc AFK ban.

## Timeout — còn là đề xuất, không tự chốt luật

Spec 2.5.1 chỉ đề xuất giữ vị trí cũ (vòng đầu spawn hợp lệ theo seed), không ném và vẫn có thể bị trúng. Bản thử nghiệm bật `proposedTimeout: true`; menu ghi rõ đề xuất. Hành động đầy đủ tự khóa và giữ nguyên. Thiếu position/aim dùng hành động skip tại vị trí cũ. Đặt false sẽ dừng timeout thiếu input với NeedsTimeoutConfirmation, không âm thầm coi đề xuất là luật. Chưa nhận xác nhận chính thức trong lượt này. Chi tiết và các yêu cầu còn mở: MISSING_REQUIREMENTS.md, SPRINT2_RULES.md.

## Kiểm chứng đã chạy

| Kiểm tra | Kết quả thực tế |
|---|---|
| Unity EditMode | **49/49 qua**; toàn bộ 22 ca Sprint 1 cộng 27 ca Sprint 2. Bằng chứng sprint2-EditMode-results.xml. |
| Unity PlayMode | **8/8 qua**; ba test Sprint 1 giữ nguyên, năm test mới. Bằng chứng sprint2-PlayMode-results.xml. |
| Geometry/combat | Ngang/dọc/chéo/sát tường/ngược hướng, hai mục tiêu thẳng hàng, radius projectile, tie order độc lập, sát thương đồng thời và clamp HP. |
| State/gameplay | 2/6 người, giới hạn sai, ready/deadline/edit lock/không reveal sớm, all locked, người chết, winner/draw, replay, kết quả/event order với clock ticks khác nhau. Trận bot theo seed thực sự kết thúc, không ép winner. |
| Bot | 300 decision mỗi Easy/Normal hợp lệ/lặp seed, context chỉ lịch sử, ngắm vị trí công khai, một lần submit và không quyết định khi HP0. |
| PlayMode/render | Pointer placement/drag/ready, chống pointer thứ hai; bốn sizes và safe area; cả player và bots sát rim không chồng HUD; Result ba outcomes/replay/menu. Test đồng hồ thật xác nhận projectile rời điểm xuất phát, damage đã tính trước nhưng HP áp dụng ở Resolve. LogAssert không có unexpected Error/Exception. |
| Web build cuối | **Succeeded**, Editor exit0, **46,729,372 byte** (~46,73 MB), compression Disabled. Bằng chứng sprint2-WebBuild.log; output unity-client/Builds/Web. |
| Browser | Bản build cuối chạy qua HTTP localhost trong Codex IAB/Chromium. Chọn một bot Normal; Placement/drag/Ready thật, bot ẩn khi locked, NÉM thực tế có projectile, tim giảm, Result Bot1 thắng sau4 vòng, replay vòng1/3tim, về menu. Không Error/Warning được captured ở lượt cuối. |
| Web responsive | 360×800, 390×844, 412×915, 430×932; DOM canvas backing và CSS đúng các kích thước đó trong lượt này (DPR1). Đã lưu và xem ảnh thật. Không dùng kết quả DPR2 của Sprint1 để tuyên bố DPR2 cho Sprint2. |
| Preservation | **48/48 prototype file nguyên byte**, không thêm file apps/packages; root package.json/lockfile, scene, URP, config project, core cũ và test Sprint1 nguyên byte. Chỉ năm file cũ trong baseline được mở rộng đúng scope, không có file Sprint1 bị xóa. |
| Font/i18n | **46 key mỗi VI/EN**, font glyph/tim qua verify-fonts.py. Font OFL giữ nguyên; VI/tim hiển thị thật trong Editor/Web. |

Workspace vẫn không có .git; `git status --short` trước/sau đều báo không phải Git repository nên không thể tạo Git diff. `SPRINT2_BASELINE.json`, `verify-sprint2.py` và `SPRINT2_FILES.tsv` cung cấp đối chiếu hash thay thế. Không khởi tạo Git hoặc thay đổi prototype để vượt qua kiểm tra. Các lỗi compile/layout phát hiện khi triển khai đã sửa; bảng trên là kết quả chạy cuối, không coi những lần lỗi trước là pass.

## File tạo / mở rộng

Danh sách chính xác source/config và meta: **SPRINT2_FILES.tsv**.

File C# mới:
- Assets/HitMe/Scripts/Core/CombatRules.cs
- Assets/HitMe/Scripts/Core/OfflineMatch.cs
- Assets/HitMe/Scripts/Core/BotController.cs
- Assets/HitMe/Scripts/UI/BattleView.Offline.cs
- Assets/HitMe/Scripts/UI/CharacterPresentation.cs
- Assets/HitMe/Scripts/UI/OfflineRunContext.cs
- Assets/HitMe/Scripts/UI/OfflineResultView.cs
- Assets/HitMe/Tests/EditMode/OfflineGameplayTests.cs
- Assets/HitMe/Tests/PlayMode/OfflineBattleTests.cs

File mới khác: Resources/offline-match-config.json (và meta), tests/verify-sprint2.py, docs/SPRINT2_BASELINE.json, SPRINT2_FILES.tsv, SPRINT2_RULES.md, SPRINT2_REPORT.md, sprint2-EditMode-results.xml, sprint2-PlayMode-results.xml, sprint2-WebBuild.log và screenshot Sprint2. Unity sinh meta thật cho source mới. Library/Builds không tính là source.

Năm file baseline mở rộng: BattleView.cs, FoundationMenu.cs, SceneEntry.cs và Resources/Localization/vi.json, en.json. Docs cập nhật: MISSING_REQUIREMENTS.md, UNITY_EDITOR_GUIDE.md. Không xóa/sửa scene hay test Sprint1; PlacementSession preview cũ giữ nguyên. Các log/XML unity-* được công cụ ghi kết quả mới, vì vậy bản sprint2-* đã chép riêng để giữ bằng chứng Sprint2.

## Chạy và mở Battle

Unity Hub → project D:/HIT ME/unity-client → Editor6000.6.4f1 → mở Assets/HitMe/Scenes/Boot.unity hoặc MainMenu.unity → Play → CHƠI VỚI BOT. Có thể mở Battle.unity trực tiếp → Play → nút CHƠI VỚI BOT giữa sân. Direct Battle vẫn có preview Sprint1 với nhãn; nút SẴN SÀNG của preview chạy demo cũ, không phải match mới. MainMenu đi thẳng vào match thật.

Chuột/chạm đặt vị trí trong elip, kéo ít nhất12px để ngắm, SẴN SÀNG khóa. Chờ reveal, ném và vòng sau. Khi bị loại, theo dõi các bot còn lại. Result → CHƠI LẠI hoặc VỀ MENU. Chạy Web qua `python -m http.server 8790 --directory unity-client/Builds/Web --bind 127.0.0.1`; mở http://127.0.0.1:8790/. Không mở index.html bằng file://.

Lệnh kiểm tra: python tests/verify-prototype.py; python tests/verify-sprint2.py; python tests/verify-fonts.py; powershell -NoProfile -File tools/unity.ps1 EditMode / PlayMode / BuildWeb (mỗi Action chạy riêng tuần tự). Hướng dẫn chi tiết: UNITY_EDITOR_GUIDE.md.

## Ảnh thật và giới hạn

Ảnh Game View: screenshots/Sprint2-Placement-{360x800,390x844,412x915,430x932}.png, Sprint2-Reveal-390x844.png, Sprint2-Throw-390x844.png, Sprint2-Throw-RealClock.png và Sprint2-Result-390x844.png. Ảnh clock thật ở viewport430×932 của fixture; không đổi tên thành390×844.

Ảnh Web: Sprint2-Web-Locked-390x844.jpg, Sprint2-Web-Throw-390x844.jpg, Sprint2-Web-Result-390x844.jpg, Sprint2-Web-Menu-390x844.jpg và Sprint2-Web-{360x800,390x844,412x915,430x932}.jpg. Tất cả là capture runtime thật, không phải mockup. Resize có thể cần thêm frame để GPU cập nhật; ảnh Result390 cuối được chụp sau khi canvas ổn định.

Chưa xác nhận timeout thiếu input là luật chính thức. Chưa Hard AI, sprite/animation/audio thật, playtest độ vui, Safari iOS/Chrome Android trên máy thật, touch thật trong browser, DPR2/3 Sprint2, FPS/memory/context loss hoặc tối ưu tải production. Touch pointer được giả lập qua EventSystem trong PlayMode, không gọi đó là test trên điện thoại. Trận không có số vòng tối đa hoặc sudden death chưa được chốt; có thể kéo dài nếu liên tục miss. Build dev ~46,73MB chưa tối ưu compression. Không có trở ngại Editor/compile/build cần chủ dự án xử lý.
