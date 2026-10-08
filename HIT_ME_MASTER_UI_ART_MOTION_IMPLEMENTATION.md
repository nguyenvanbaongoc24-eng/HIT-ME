# HIT ME — MASTER UI, ART & MOTION IMPLEMENTATION PLAN

> **Version:** 1.0  
> **Target:** Unity 6 / Universal 2D / Unity Web (mobile portrait first)  
> **Operator:** Codex working in the existing HIT ME repository  
> **Execution mode:** Audit → Phase 1 → UI Kit → Phase 2 → Sprint 6 → QA & handoff  
> **Reference art location (Windows):** `D:\HIT ME\Picture`

## 0. Mục tiêu và quy tắc bất biến

Xây dựng giao diện HIT ME đạt chất lượng game casual multiplayer mobile: dễ thao tác, có bản sắc Việt Nam, đồng bộ giữa **Main Menu / Online Lobby / Character & Weapon Selection / Battle / Results**, chuyển động có trọng lượng, không còn cảm giác prototype. Sử dụng hình concept làm **art direction**, không nhúng nguyên ảnh chứa chữ/nút vào Canvas.

**Không được:** tạo project mới; phá vỡ gameplay, luật trận, bot, network, backend hoặc dữ liệu người chơi; xóa prototype Phaser/TypeScript; thay combat hitbox theo animation; thay UI hoạt động bằng hình minh họa không tương tác; tự tuyên bố có sprite/animation khi chỉ có concept; tự báo pass test/build khi chưa chạy.

**Bắt buộc:** tạo Git checkpoint trước mỗi phase; ghi log và báo cáo bằng chứng; giữ VI/EN; hỗ trợ mobile Safari iOS / Chrome Android; không hardcode secrets; mọi UI action phải hoạt động hoặc có disabled/coming-soon state rõ ràng.

### 0.1. Quy tắc thực thi

1. **Khám phá repo:** tìm `unity-client/`, Unity Editor, `Assets/HitMe/Scenes`, script, prefab, docs/spec, hệ thống UI đang có. Đường dẫn chỉ là giả định cho tới khi Codex xác minh.
2. **Audit trước khi sửa:** báo cáo thực trạng và danh sách chức năng có thật. Không viết lại hệ thống đã có.
3. **Thực hiện từng phase:** chạy compile/test/build, lưu screenshot, báo cáo và checkpoint rồi mới chuyển phase tiếp theo.
4. **Thiếu asset:** tạo manifest và fallback rõ ràng; không sinh nhân vật chuyên nghiệp bằng primitive hoặc cắt bừa ảnh concept.
5. **Unity Editor:** chỉ dùng batchmode khi project không bị Editor khác khóa; tránh mở hai tiến trình cùng project.

---

## 1. PRE-FLIGHT — ASSET AUDIT: `D:\HIT ME\Picture`

### 1.1. Nguồn tham khảo người dùng đã chuẩn bị

Trong ảnh chụp File Explorer, thư mục `D:\HIT ME\Picture` hiển thị năm mục sau (tên thực tế, phần mở rộng và nội dung phải được Codex xác minh bằng filesystem):

| Tên hiển thị | Vai trò dự kiến | Quy tắc sử dụng |
|---|---|---|
| `Concept characters 2` | Concept nhân vật người + động vật, đồ ném, VFX | Reference only cho tới khi có sprite riêng |
| `Concept characters` | Concept nhân vật, vũ khí và động tác | Reference only cho tới khi có sprite riêng |
| `MAIN MENU` | Mẫu bố cục/art direction trang chủ | Không sử dụng làm một nút/bản UI tĩnh |
| `Play in game` | Mẫu đấu trường/HUD | Chỉ tham khảo; hình học và hitbox vẫn theo code |
| `UI CONCEPT` | Design board cho các thành phần UI | Không crop chữ/nút đã vẽ làm prefab cuối |

> **Chú ý:** ảnh chụp chỉ chứng minh các tên mục được hiển thị; chưa xác nhận extension, resolution, alpha, quyền sử dụng hoặc có các PNG tách lớp hay không.

### 1.2. Việc Codex phải làm

- Xác minh thư mục tồn tại và liệt kê **tên đầy đủ, extension, kích thước, độ phân giải, alpha channel** của từng file.
- Phân loại: `reference_composite`, `production_sprite`, `sprite_sheet`, `layered_source`, `font`, `unknown`.
- Kiểm tra liệu có sprite trong suốt, UI backgrounds không chữ, icon riêng, frame animation, rig-ready layers.
- Không chỉnh sửa/xóa ảnh gốc; tạo bản sao vào `docs/references/` hoặc `Assets/HitMe/Art/References/` **chỉ khi cần**, tránh import ảnh tham khảo quá lớn vào runtime build.
- Tạo `docs/PICTURE_ASSET_AUDIT.md` và `docs/HITME_UI_ASSET_MANIFEST.md`.
- Manifest nên có: `asset_id`, `source_path`, `category`, `status`, `target_path`, `dimensions`, `alpha`, `pivot`, `ppu`, `slicing`, `atlas`, `usage`, `license_notes`, `missing_requirements`.

### 1.3. Asset sản xuất cần có

- **Character sprites:** Nam Làng (tóc đen), Nữ Năng Động (tóc hồng), Quậy Boy (tóc vàng), Cô Ba (nón lá), Anh Teen (mũ lưỡi trai), Chó Vàng, Mèo Mun.
- **Character states:** Idle, Aim, Throw, Hit, Eliminated, Victory; optional Blink/Run. Mỗi trạng thái có frame thật hoặc rig tách lớp; fallback tween phải ghi rõ.
- **Weapon sprites:** dép tổ ong nhiều màu, điện thoại, vợt muỗi, vợt pickleball, cốc bia (đạo cụ hoạt hình), chảo, TV, tủ lạnh mini, ghế nhựa, nồi cơm điện, nón lá, quạt giấy; chó/mèo chỉ ở dạng **đạo cụ hoạt hình phi bạo lực**, không thể hiện ngược đãi động vật.
- **UI:** logo riêng, nút nền không chữ, icon, panels, 9-slice sprites, portraits, HUD, popup, map cards, navigation icons.
- **Environment:** nền sân đình, sân gạch, cổng đình, cây đa, cờ, đèn lồng, khán giả, bóng đổ, lá rơi; cần tách layer để chuyển động độc lập.
- **FX:** dust, hit burst, throw trail, sparkle, confetti, leaf particle, HP reaction.

**Gate 0:** Asset audit hoàn tất; danh sách missing rõ ràng; không nhầm reference với production-ready asset.

---

## 2. PHASE 1 — MAIN MENU: UI STRUCTURE, NAVIGATION & RESPONSIVE

**Mục tiêu:** UI sạch, logic đầy đủ, responsive và chạy được ngay cả khi chưa có artwork. **Không** triển khai texture/animation phức tạp ở phase này.

### 2.1. Audit UI hiện tại

- Tìm scene Main Menu, Canvas, EventSystem, localization, navigation, màn Bot, Online Lobby, Character Selection, Arena Selection, Settings.
- Lập bảng các hành động đang hoạt động / chưa hoạt động.
- Ghi nhận và **loại khỏi trang chủ** dòng thông báo debug/spec như `Trial: incomplete timeout...`; chỉ giữ trong dev diagnostics.
- Giữ lối vào Chơi với Bot hiện có, số bot, độ khó, map, graphics, VI/EN.

### 2.2. Bố cục đề xuất

1. **Top Status Bar:** avatar, tên/cấp độ, số dư nếu có backend thật, inbox, settings; mock data phải có nhãn dev.
2. **Brand & Character Stage:** logo HIT ME, tagline `NÉM LÀ VUI`, vùng preview nhân vật, không che CTA.
3. **Primary CTA:** `CHƠI NHANH` (matchmaking nếu đã có); `PHÒNG RIÊNG` (tạo/tham gia phòng); `CHƠI VỚI BOT` (luyện tập). Mỗi nút có Normal / Pressed / Disabled / Loading.
4. **Feature Cards:** `BẢN ĐỒ`, `NHÂN VẬT`, `BỘ SƯU TẬP`, `XẾP HẠNG`; chưa có chức năng phải hiện `Sắp ra mắt`.
5. **Bottom Navigation:** `Trang chủ`, `Cửa hàng`, `Túi đồ`, `Sổ tay du hành`, `Cộng đồng`; active/disabled rõ.
6. **Panels phụ:** Bot Setup (bot count, difficulty), Arena Selection, Settings (graphics, audio nếu có, VI/EN), notifications.

### 2.3. Navigation & state

- Tạo/tái sử dụng `MenuNavigationService`, `MainMenuView`, `MainMenuController`, `MenuScreenState`.
- Không hardcode tên scene rải rác; xử lý back/cancel, loading/error, reconnect nếu online đã có.
- Mỗi button phải có onClick thật, hoặc disabled + lý do. Không giả lập matchmaking thành công.
- Không mất cấu hình Bot khi back; không tạo scene/prefab trùng lặp.

### 2.4. Mobile responsive

- Reference 390×844; kiểm thử 360×800, 375×812, 390×844, 393×852, 402×874, 412×915, 430×932.
- `CanvasScaler`, anchors, safe-area root, layout constraints, TMP autosizing có giới hạn, scroll/compact layout nếu thiếu chiều cao.
- Tránh che Dynamic Island / Home Indicator; nút chính không bị bottom bar che; touch target ~44 logical pt trở lên khi khả thi.
- UI hiển thị ổn trên desktop browser portrait preview và mobile touch.
- TMP font có đủ glyph tiếng Việt, ưu tiên Be Vietnam Pro khi asset/license hợp lệ.

### 2.5. Gate 1 — nghiệm thu

- [ ] Main Menu không còn danh sách 7 nút xanh trên nền nâu.
- [ ] Luồng Chơi với Bot vẫn chạy; số bot/độ khó/bản đồ vẫn chỉnh được.
- [ ] Luồng Online Lobby/Phòng riêng không bị phá; chưa sẵn sàng thì thông báo đúng.
- [ ] Tất cả button có state và hành vi; Back hoạt động.
- [ ] VI/EN hiển thị đúng; không clipping, overlap, tràn safe area.
- [ ] Unity compile, EditMode/PlayMode tests phù hợp, Web Build thành công.
- [ ] Screenshot thực tế 390×844 và 430×932 nếu công cụ cho phép.

**Deliverable:** `docs/MAIN_MENU_PHASE_1_REPORT.md` + prefab/layout có thể dùng được mà không phụ thuộc ảnh concept.

---

## 3. INDEPENDENT UNITY UI KIT — PREFABS & DESIGN TOKENS

**Mục tiêu:** các thành phần độc lập, có thể dùng lại ở Main Menu, Lobby, Battle, Results, Inventory. Có thể bắt đầu sau Gate 1 và hoàn thiện skin ở Phase 2.

### 3.1. Component catalog

| Prefab / system | Chức năng | Trạng thái bắt buộc |
|---|---|---|
| `HitMeUITheme` | Tokens: colors, typography, spacing, radii, sprites | Theme/fallback |
| `HitMeButton` | Primary, secondary, tertiary | Normal/Pressed/Disabled/Loading |
| `HitMeIconButton` | Settings, notifications, chat | Normal/Pressed/Disabled/Badge |
| `HitMeTopBar` | Profile, resources, settings | Data/Loading/Unavailable |
| `HitMeFeatureCard` | Map, Character, Collection, Rank | Normal/Selected/Locked |
| `HitMeBottomNavigation` | 5 tab | Active/Inactive/Disabled |
| `HitMePopup` | Notice, Settings, Result | Open/Close/Confirm/Cancel |
| `HitMePlayerHUD` | Portrait, hearts, round, timer | Alive/Hit/Eliminated |
| `HitMeChatPanel` | Lobby/Battle chat | Open/Close/Disabled |
| `HitMeCharacterCard` | Chọn người, Chó Vàng, Mèo Mun | Selected/Locked/Available |
| `HitMeWeaponCard` | Vũ khí hài hước | Selected/Locked/Available |
| `HitMeMapCard` | Bản đồ Việt Nam | Selected/Locked/Available |

### 3.2. Kiến trúc

- Tách **View / State / Navigation / Data Binding**; dùng prefab variants hoặc ScriptableObject theme khi hợp lý.
- Text là TMP component, **không bake chữ vào sprite**.
- Button background dùng 9-slice, icon là sprite riêng; không kéo giãn icon.
- Dữ liệu tài khoản, xu, vật phẩm, room state lấy từ service thực; mock chỉ trong dev/test.
- Thêm `UI Showcase` scene hoặc prefab demo (không đưa vào production build nếu không cần) để kiểm tra component states.
- Không dùng nguyên `UI CONCEPT` làm UI Image.

### 3.3. Cấu trúc thư mục đề xuất

```text
unity-client/Assets/HitMe/
  Art/UI/{Buttons,Icons,Panels,Cards,HUD,Backgrounds}/
  Art/Characters/
  Art/Weapons/
  Art/Environment/
  Art/VFX/
  Prefabs/UI/
  Prefabs/Characters/
  Animations/{UI,Characters,Environment,VFX}/
  ScriptableObjects/{UI,Characters,Weapons}/
  Scripts/{UI,Visuals,Environment}/
  Editor/
docs/
  references/
  PICTURE_ASSET_AUDIT.md
  HITME_UI_ASSET_MANIFEST.md
```

Tận dụng thư mục sẵn có; không tạo cấu trúc song song nếu project đã có tương đương.

### 3.4. Gate UI Kit

- [ ] Tất cả prefab chính độc lập, chỉnh được trong Inspector.
- [ ] Prefab không tham chiếu cứng scene cụ thể khi không cần.
- [ ] Data binding/Localization không hardcode.
- [ ] Có UI Showcase test states.
- [ ] Main Menu và ít nhất một UI khác tái sử dụng component thật.

**Deliverable:** `docs/HITME_UI_KIT_IMPLEMENTATION.md` và manifest được cập nhật.

---

## 4. PHASE 2 — ARTWORK, SPRITES, TEXTURE, ICONS & UI POLISH

**Điều kiện:** Gate 1 + UI Kit core pass. **Không** thay lại layout/navigation trừ lỗi responsive được chứng minh.

### 4.1. Art direction

- Vietnamese stylized 2D; chất liệu gỗ, gạch sân đình, mái ngói, cờ hội; màu sắc vui, rõ, có chiều sâu, tiết chế glow/gradient.
- Phong cách thủ công nhất quán, không AI glossy/plastic, không dùng ảnh concept nguyên tấm như giao diện.
- Dùng concept `MAIN MENU`, `Play in game`, `UI CONCEPT`, `Concept characters`, `Concept characters 2` làm tài liệu tham khảo; **đối chiếu trước khi sản xuất**.

### 4.2. Thứ tự tích hợp asset

1. **Logo riêng** và backgrounds tách layer (không chữ/nút baked-in).
2. **Button skins** (9-slice) cho primary/secondary/tertiary và các states.
3. **Icons** đồng bộ stroke/shape/size cho nav, resources, features, settings.
4. **Feature cards, popup, HUD, chat**; bảo đảm text TMP luôn độc lập.
5. **Character preview**: sprite thực tế, shadow, vũ khí, idle (nếu chưa có, ghi rõ fallback).
6. **Arena UI**: HUD gọn, timer/HP/ready/chat không che sân; mobile safe area.
7. **Atlas & import optimization**: alpha, compression, PPU, filter mode theo style, sprite atlas, 9-slice border, pivots.

### 4.3. Tiêu chí chất lượng

- Không blur/méo ảnh khi thay resolution; không icon có phong cách lẫn lộn.
- Trên iPhone portrait, hai CTA chính hiện rõ và không bị đẩy xuống dưới màn hình.
- Trên Battle, arena playable không bị các layer decor che; player marker, projectile trail và touch zone đọc được.
- Không tự gán asset chưa có; report rõ `asset_id` nào còn thiếu.

### 4.4. Gate 2

- [ ] UI có artwork thật độc lập; không còn nút mặc định màu phẳng ở màn chính.
- [ ] Không có Missing Sprite / Missing Font / NullReference.
- [ ] Layout/Navigation/Localization Phase 1 không bị regression.
- [ ] Test + Web Build pass; screenshot trước/sau trên 2 viewport.

**Deliverable:** `docs/MAIN_MENU_PHASE_2_REPORT.md` + updated asset manifest.

---

## 5. SPRINT 6 — MOTION & ANIMATION SYSTEM

**Điều kiện:** có prefab/asset phù hợp. Có thể tạo motion fallback nhẹ nếu asset chưa tách lớp, nhưng phải ghi rõ giới hạn.

### 5.1. Animation architecture

- `CharacterVisualController`: phản ánh gameplay events, không quyết định HP/hit/damage.
- `CharacterAnimationProfile`: thông số theo nhân vật.
- `WeaponAnimationProfile`: throw arc visual, rotation, trail, impact.
- `EnvironmentMotionController`: cờ, lá, đèn, khán giả, chim, bụi.
- `UIMotionController`: button, popup, tab, reward reveal.
- `MotionQualityProfile`: Low / Medium / High / Reduced Motion.

**Bắt buộc:** tách `LogicalRoot` (tọa độ/hitbox) và `VisualRoot` (animation); không dịch chuyển logical root bằng squash/stretch/knockback.

### 5.2. Nhân vật người và động vật

- Người: Nam Làng, Nữ Năng Động, Quậy Boy, Cô Ba, Anh Teen.
- Động vật: Chó Vàng, Mèo Mun — có idle, blink, wag/tail, aim, throw, hit reaction, victory vui nhộn.
- States chung: `Idle`, `Aim`, `Throw`, `Hit`, `Eliminated`, `Victory`.
- `Throw`: anticipation → wind-up → release → follow-through → recovery. Thời điểm release phải theo event đã được gameplay xác nhận.
- Rig 2D chỉ khi có layers/bones hợp lệ; sprite-sheet chỉ khi có frame thật; fallback procedural tween phải tinh tế, không xoay toàn nhân vật như tấm bìa.
- Vũ khí chó/mèo nếu triển khai chỉ thể hiện kiểu **thú cưng hoạt hình tham gia trò đùa an toàn** (ví dụ chạy tới quấy rối, hiệu ứng sao/khói), không quăng/ném động vật như vật thể hay minh họa đau đớn.

### 5.3. Vũ khí và impact VFX

- Dép tổ ong, điện thoại, vợt muỗi, vợt pickleball, cốc bia hoạt hình, chảo, TV, tủ lạnh mini… có visual profile riêng.
- Projectile spin, arc/trail, hit flash, dust, comedic squash/stretch, screen shake rất nhẹ; scale VFX theo kích thước item.
- Các vật thể lớn như TV/tủ lạnh cần animation hài hước và kích thước **không che toàn sân**, không làm thay đổi hitbox hoặc luật first-target-hit.
- Sử dụng object pooling, không Instantiate/Destroy VFX liên tục.

### 5.4. Environment motion

- Tách cảnh sân đình thành layers: sky/far architecture, tree/canopy, flags, lanterns, audience, arena, foreground.
- Cờ flutter, lá rung, đèn sway, crowd idle/cheer, dust motes, bird passes, shadow movement nhẹ.
- Phase offset/seed riêng để không đồng bộ giả tạo; parallax rất nhẹ, không làm lệch input/arena geometry.
- Nếu chỉ có ảnh nền tổng hợp, **không giả vờ đã tách lá/cờ**; yêu cầu asset riêng hoặc giới hạn ở chuyển động camera/background cực nhẹ.

### 5.5. UI motion

- Press scale 0.96–0.98 và hồi lại; tab slide/fade ngắn; popup enter/exit; HP loss; timer warning; ready/reveal/result.
- UI tween ngắn khoảng 120–220ms tùy chức năng; không cản touch hoặc tạo motion quá nhiều.
- Reduced Motion: giảm rung, parallax, particle, lặp môi trường.

### 5.6. Mobile performance

- Target 60 FPS nếu thiết bị đủ mạnh, 30 FPS fallback; **phải đo** mới được báo đạt.
- Pool VFX, atlas sprites, hạn chế overdraw, không dùng shader nặng; hạn chế GC allocations; LOD motion theo quality.
- Không để animation tốn CPU trong background tab; kiểm tra reconnect khi Safari chuyển app.
- Test thực tế trên iPhone phải ghi riêng; không đánh đồng desktop emulation với Safari thật.

### 5.7. Gate 6

- [ ] Ít nhất 1 nhân vật người + Chó Vàng + Mèo Mun có idle/aim/throw/hit (asset thật hoặc fallback được đánh dấu).
- [ ] Vũ khí có throw/impact VFX mà không làm thay đổi gameplay resolver.
- [ ] Ít nhất 3 loại môi trường chuyển động thực sự.
- [ ] UI button press, popup, ready/result có feedback.
- [ ] Motion Quality + Reduced Motion hoạt động.
- [ ] Tests và Web Build pass; ghi FPS/bộ nhớ nếu đo được.

**Deliverable:** `docs/SPRINT_6_MOTION_REPORT.md`, danh sách animation clips/prefabs, screenshot/video nếu môi trường hỗ trợ.

---

## 6. QUALITY ASSURANCE & RELEASE GATES

### 6.1. Regression không được phá

- Gameplay: placement, aim, ready, reveal, throw, first target hit, HP, timeout, simultaneous draw.
- Modes: bot offline và multiplayer nếu đã triển khai.
- Settings: bot count, difficulty, arena, graphics, VI/EN.
- Navigation: Main Menu → Bot Match / Online Lobby / Character / Map / Settings → Back.
- Rewards/inventory: không thay số dư hoặc trạng thái người chơi do sửa UI.

### 6.2. Build pipeline

1. Xác minh Unity Editor path và Web Build Support.
2. Đảm bảo không có Unity Editor khác đang khóa project.
3. Unity compile → EditMode tests → PlayMode tests → Web Build.
4. Host build bằng HTTP/HTTPS, không mở `file://`.
5. Browser smoke test: Main Menu, Bot Match, Battle, Result, navigation.
6. Chụp ảnh 390×844 và 430×932; ghi lỗi console và network nếu có.
7. Nếu không có thiết bị thật, ghi `iPhone Safari device test: NOT RUN`.

### 6.3. Checklist tổng

- [ ] Gate 0 — Picture audit và asset manifest.
- [ ] Gate 1 — Main Menu functional/responsive.
- [ ] UI Kit — Prefabs độc lập, showcase, data binding.
- [ ] Gate 2 — Artwork độc lập, sprite, icons, texture.
- [ ] Gate 6 — Character/environment/weapon/UI motion.
- [ ] All applicable tests pass; Unity Web Build pass.
- [ ] Screenshots và reports có đường dẫn thực.
- [ ] Known issues / missing assets / next steps được ghi rõ.

---

## 7. THỨ TỰ CHẠY CHO CODEX

### Lệnh khởi động (dán nguyên khối vào Codex)

> Hãy đọc file `HIT_ME_MASTER_UI_ART_MOTION_IMPLEMENTATION.md` này trong repository HIT ME và thực thi theo đúng thứ tự: **Pre-flight Asset Audit → Phase 1 Main Menu → Independent Unity UI Kit → Phase 2 Artwork → Sprint 6 Motion → QA**. Các hình concept đã được người dùng đặt tại `D:\HIT ME\Picture`; xác minh tên/extension và tình trạng asset trước khi import. Không dùng nguyên ảnh concept tổng hợp làm UI tương tác hoặc sprite sheet. Làm trực tiếp trong Unity Project hiện tại, giữ nguyên gameplay và multiplayer. Sau mỗi phase, chạy test/build thực tế, tạo báo cáo theo đường dẫn trong tài liệu, liệt kê file thay đổi và cung cấp screenshot nếu có. Nếu thiếu production sprite, triển khai phần code/prefab có thể làm, đánh dấu thiếu asset, **không tự nhận phần art đã hoàn thành**. Không chuyển sang phase tiếp theo khi gate chưa đạt; hỏi người dùng chỉ khi có blocker thực sự không thể tự xử lý.

### Báo cáo cuối

Tạo `docs/HITME_MASTER_IMPLEMENTATION_STATUS.md` gồm:

- Unity version / editor path / project path.
- Git checkpoint/commit per phase (nếu môi trường cho phép).
- Checklist gates (PASS / PARTIAL / BLOCKED / NOT RUN).
- Files changed và asset manifest.
- Test counts, logs, Web build path, browser screenshot paths.
- Missing art assets và những việc cần người dùng cung cấp.
- Risks: performance, mobile safe area, sprite readability, localization, network.
- Next sprint recommendations.

---

## 8. Định nghĩa hoàn thành

Dự án chỉ được coi là hoàn thành theo phạm vi tài liệu khi **Main Menu có bố cục mobile chuẩn, điều hướng hoạt động, UI Kit tái sử dụng được, artwork là sprite độc lập, chuyển động thật trong Unity, gameplay không regression và Web Build được kiểm chứng**. Nếu thiếu sprite gốc hoặc chưa thử Safari iPhone thật, báo cáo rõ **PARTIAL** thay vì **DONE**.
