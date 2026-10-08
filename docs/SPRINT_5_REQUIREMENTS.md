# HIT ME — SPRINT 5: PREMIUM MAIN MENU REDESIGN
## Unity 6 + Codex | Mobile-First Game UI/UX

Bạn là Senior Unity UI/UX Engineer, Game UI Artist, Technical Artist và Unity Build Engineer.

Nhiệm vụ: Thiết kế lại hoàn toàn giao diện Main Menu của game HIT ME, dựa trên concept trang chủ mới nhất đã được duyệt.

Đây là một nhiệm vụ triển khai thực tế trên Unity Project hiện tại, không phải nhiệm vụ chỉ tạo mockup hoặc viết tài liệu.

**Yêu cầu quan trọng nhất: Giao diện phải đạt chất lượng của một game mobile thương mại, không còn cảm giác prototype, không sử dụng các button hình chữ nhật đơn giản hoặc hình ảnh AI làm toàn bộ giao diện.**

---

# PHẦN 1 — KIỂM TRA DỰ ÁN HIỆN TẠI

Project:

- Game: HIT ME.
- Engine: Unity 6.
- Render Pipeline: Universal 2D.
- Platform: Unity Web, ưu tiên Safari iOS và Chrome Android.
- Orientation: Portrait.
- Unity Project: `unity-client/`.

Trước khi chỉnh sửa:

1. Kiểm tra cấu trúc Unity Project.
2. Tìm MainMenu scene hiện tại.
3. Kiểm tra UI Canvas, EventSystem và các script điều hướng.
4. Kiểm tra hệ thống Localization VI/EN.
5. Kiểm tra hệ thống Bot Match và Online Lobby.
6. Kiểm tra Character Selection, Arena Selection và Settings.
7. Kiểm tra các sprite, font, texture và UI assets có sẵn.
8. Xác định những chức năng đã hoạt động và những chức năng còn placeholder.

Không xây lại project.

Không chỉnh sửa gameplay, CombatResolver, Multiplayer Server hoặc Supabase nếu không cần thiết.

Không xóa các scene hiện tại.

---

# PHẦN 2 — ĐỊNH HƯỚNG THIẾT KẾ

## 2.1. Art Direction

Phong cách: Vietnamese Stylized 2D Mobile Game.

Tham khảo nguyên tắc thiết kế từ các game mobile thương mại như:

- Brawl Stars: phân cấp thông tin và CTA rõ ràng.
- Clash Royale: giao diện gọn, các trạng thái dễ nhận biết.
- Stumble Guys: màu sắc vui nhộn, thao tác đơn giản.
- Các game casual mobile: điều hướng bằng tab, card và icon.

Không sao chép trực tiếp asset, logo, bố cục độc quyền hoặc trade dress của các game tham khảo.

Yêu cầu:

- Màu sắc tự nhiên, có chủ đích.
- Nét vẽ cartoon thủ công.
- Texture gỗ, giấy, đá và vật liệu Việt Nam.
- Không lạm dụng gradient.
- Không dùng glow quá mức.
- Không dùng glassmorphism.
- Không dùng quá nhiều màu neon.
- Không tạo cảm giác các thành phần UI được AI ghép ngẫu nhiên.

Các thành phần phải có cùng một hệ thống thiết kế.

## 2.2. Background

Bối cảnh mặc định: Sân đình làng Bắc Bộ.

Chi tiết:

- Cổng đình truyền thống.
- Cây đa.
- Mái ngói.
- Cờ hội.
- Quầy hàng nhỏ.
- Đèn lồng.
- Sân gạch.
- Một vài vật dụng như dép, chảo, ghế gỗ.

Background cần có chiều sâu bằng các lớp:

1. Far Background.
2. Midground Architecture.
3. Foreground Decorations.
4. Character Presentation.
5. Interactive UI.

Có thể sử dụng parallax rất nhẹ.

Không để background quá nhiều chi tiết gây khó đọc UI.

Nếu chưa có background asset phù hợp, sử dụng fallback có kiểm soát và ghi rõ asset còn thiếu.

Không dùng ảnh concept nguyên tấm làm giao diện có thể tương tác.

---

# PHẦN 3 — MAIN MENU LAYOUT

Thiết kế theo portrait mobile.

Thứ tự hiển thị:

## A. TOP BAR — PLAYER STATUS

Chiều cao nhỏ gọn, nằm trong Safe Area.

Bên trái:

- Avatar.
- Tên người chơi.
- Level.
- Rank hoặc điểm xếp hạng nếu đã có.

Ở giữa:

- Xu Làng.
- Nguyên liệu hoặc tài nguyên hiện có.

Bên phải:

- Inbox/Notifications.
- Settings.

Quy tắc:

- Dữ liệu phải đến từ hệ thống thực tế.
- Nếu backend chưa có, sử dụng mock data được đánh dấu rõ trong Development Mode.
- Không tự tạo tài khoản, tiền tệ hoặc dữ liệu giả trên production.
- Không hiển thị quá nhiều tài nguyên khi màn hình hẹp.
- Không tạo nút "+" nếu chưa có chức năng tương ứng.

## B. GAME BRANDING

Logo HIT ME đặt ở phần trên của màn hình.

Có tagline:

"NÉM LÀ VUI"

Logo:

- Chữ mạnh mẽ, dễ nhận diện.
- Không quá to.
- Có thể kết hợp hình dép tổ ong.
- Không che nhân vật hoặc nút chính.

Logo phải là asset độc lập.

## C. CENTRAL PRESENTATION

Khu vực giữa màn hình:

- Hiển thị nhân vật đang chọn.
- Có idle animation.
- Có thể hiển thị vũ khí đang trang bị.
- Có bóng dưới chân.
- Không che nút chơi.

Sử dụng character prefab/sprite hiện có.

Nếu chưa có sprite, dùng fallback được ghi rõ.

Không tạo nhân vật bằng các hình chữ nhật có chữ PH.

## D. PRIMARY ACTIONS

Đây là khu vực quan trọng nhất.

Chỉ có hai hành động chính.

### Button 1: CHƠI NHANH

Primary CTA.

Màu vàng ấm hoặc cam đất.

Nội dung:

CHƠI NHANH
"Vào trận 2–6 người"

Hành vi:

- Nếu Quick Match online đã hoạt động: kết nối matchmaking.
- Nếu chưa hoạt động: hiển thị trạng thái chưa khả dụng, không giả lập kết nối thành công.
- Có loading, cancel và error states.

### Button 2: PHÒNG RIÊNG

Secondary CTA.

Màu đỏ gạch hoặc xanh đậm phù hợp palette.

Nội dung:

PHÒNG RIÊNG
"Tạo phòng • Mời bạn bè"

Hành vi:

- Điều hướng tới Private Room.
- Hỗ trợ Create Room và Join Room nếu đã triển khai.
- Nếu backend chưa sẵn sàng, hiển thị trạng thái phù hợp.

Hai nút phải có:

- Normal.
- Pressed.
- Disabled.
- Loading.
- Focus/Selected khi cần.

Sử dụng 9-sliced sprites để thay đổi kích thước mà không méo góc.

Không dùng button phẳng mặc định của Unity.

## E. SECONDARY FEATURES

Tạo 4 feature cards nhỏ:

1. BẢN ĐỒ
   - Chọn đấu trường Việt Nam.
2. NHÂN VẬT
   - Trang phục, vũ khí, hiệu ứng.
3. BỘ SƯU TẬP
   - Vật phẩm theo vùng miền.
4. XẾP HẠNG
   - Thành tích và mùa giải.

Mỗi card gồm:

- Artwork hoặc icon.
- Tiêu đề.
- Một dòng mô tả ngắn nếu đủ không gian.
- Trạng thái khóa hoặc sắp ra mắt nếu chưa triển khai.

Card cần có kích thước nhất quán.

Không dùng ảnh phức tạp gây khó đọc chữ.

## F. BOTTOM NAVIGATION

Thanh điều hướng cố định phía dưới:

- Trang chủ.
- Cửa hàng.
- Túi đồ.
- Sổ tay du hành.
- Cộng đồng.

Thiết kế:

- Nền tối nhẹ.
- Icon rõ ràng.
- Active tab nổi bật.
- Không có viền quá dày.
- Không có hiệu ứng glow mạnh.
- Có label dưới icon.
- Hỗ trợ Safe Area Bottom.

Không thêm tab rỗng không có phản hồi.

Các tab chưa có tính năng phải hiển thị trạng thái "Sắp ra mắt" hoặc disabled rõ ràng.

---

# PHẦN 4 — SETTINGS VÀ OFFLINE GAME

Các chức năng cũ:

- Play With Bots.
- Bot Count.
- Bot Difficulty.
- Arena Selection.
- Graphics Quality.
- Language VI/EN.

Không được xóa.

Tổ chức lại:

### Chơi với Bot

Đưa vào màn hình chọn chế độ hoặc mục Luyện tập, truy cập từ Main Menu.

### Bot Count

Đặt trong cấu hình trận Bot.

### Difficulty

Easy / Normal / Hard nếu đã được hỗ trợ thực tế.

### Arena Selection

Đặt trong màn hình Bản đồ.

### Graphics

Đưa vào Settings.

### Language

Đưa vào Settings và hỗ trợ chuyển đổi tức thời nếu kiến trúc hiện tại cho phép.

Không hiển thị toàn bộ các tùy chọn này thành những nút lớn xếp dọc trên Main Menu.

---

# PHẦN 5 — RESPONSIVE MOBILE

Thiết kế tham chiếu: 390×844.

Kiểm thử:

- 360×800.
- 375×812.
- 390×844.
- 393×852.
- 402×874.
- 412×915.
- 430×932.

Yêu cầu:

- Canvas Scaler phù hợp.
- Safe Area tương thích Dynamic Island.
- Bottom Navigation không bị Home Indicator che.
- Không có text clipping.
- Không có button overlap.
- Không có icon biến dạng.
- Không có horizontal overflow.
- Hỗ trợ touch input.
- Touch target ưu tiên tối thiểu khoảng 44×44 logical points trên iOS.

Nếu không đủ chiều cao, ưu tiên giữ nguyên Top Bar, Primary CTA và Bottom Navigation; cho phép thu gọn hoặc cuộn phần Secondary Features.

Không thu nhỏ toàn bộ UI một cách máy móc.

---

# PHẦN 6 — FONT VÀ TYPOGRAPHY

Sử dụng TextMeshPro.

Font phải:

- Hỗ trợ tiếng Việt đầy đủ.
- Không lỗi dấu.
- Không bị mờ.
- Có hệ thống kích thước nhất quán.
- Không dùng nhiều hơn 2 font families.

Đề xuất:

- Be Vietnam Pro cho UI.
- Font display riêng cho logo nếu có license phù hợp.

Phân cấp:

- Logo.
- Primary CTA.
- Section Title.
- Body.
- Caption.

Không lạm dụng viền chữ trắng/đen.

---

# PHẦN 7 — MICRO-INTERACTIONS

Thêm chuyển động nhẹ:

- Button press scale 0.96–0.98.
- Button release.
- Card hover khi dùng desktop.
- Character idle.
- Background parallax nhẹ.
- Tab selection.
- Notification badge.
- Loading spinner.

Thời lượng animation UI khoảng 120–220ms tùy loại.

Ưu tiên cảm giác phản hồi nhanh, không dùng hiệu ứng kéo dài.

Không tạo particle hoặc animation quá nặng trên Safari iOS.

---

# PHẦN 8 — UNITY ARCHITECTURE

Tạo hoặc tái sử dụng các thành phần:

- MainMenuController.
- MainMenuView.
- PlayerStatusWidget.
- CurrencyWidget.
- PrimaryActionButton.
- FeatureCard.
- BottomNavigation.
- MenuNavigationService.
- MenuLocalization.
- SafeAreaAdapter.
- UIThemeDefinition.

Tách:

- View.
- State.
- Navigation.
- Data Binding.

Không hardcode dữ liệu người chơi vào UI Prefab.

Sử dụng ScriptableObject cho theme/config nếu phù hợp.

Tất cả các button phải được kết nối với chức năng hoặc trạng thái thực tế.

Không dùng UI chỉ để trình diễn mà không thể thao tác.

---

# PHẦN 9 — ASSET PIPELINE

Kiểm tra asset đã có trước khi tạo mới.

Tạo thư mục hợp lý:

Assets/HitMe/Art/UI/MainMenu/
Assets/HitMe/Art/UI/Icons/
Assets/HitMe/Art/UI/Buttons/
Assets/HitMe/Art/Backgrounds/
Assets/HitMe/Prefabs/UI/
Assets/HitMe/ScriptableObjects/UI/

Yêu cầu:

- PNG trong suốt khi cần.
- Sprite Atlas.
- 9-slice button backgrounds.
- Texture import settings phù hợp.
- Consistent PPU.
- Không kéo giãn icon.
- Không sử dụng hình ảnh AI tổng hợp làm giao diện tương tác.

Nếu thiếu asset quan trọng:

1. Xây dựng UI layout có thể chạy.
2. Sử dụng fallback tối giản.
3. Tạo danh sách asset cần sản xuất.
4. Không tuyên bố hoàn thành phần visual art.

---

# PHẦN 10 — TESTS VÀ WEB BUILD

Bắt buộc:

1. Unity compile thành công.
2. MainMenu scene load thành công.
3. Các nút hoạt động đúng.
4. Navigation hoạt động.
5. VI/EN hoạt động.
6. Safe Area đúng.
7. Không có missing references.
8. Không có Console errors nghiêm trọng.
9. Không phá vỡ Battle scene.
10. Unity Web Build thành công.

Chạy EditMode và PlayMode tests phù hợp.

Nếu có công cụ browser automation, chụp ảnh Main Menu ở ít nhất 390×844 và 430×932.

Không tuyên bố đã kiểm thử iPhone thực tế nếu chưa có thiết bị.

---

# PHẦN 11 — ACCEPTANCE CRITERIA

Sprint chỉ được coi là hoàn thành khi:

- Main Menu không còn là danh sách button xanh trên nền nâu.
- Có phân cấp rõ ràng giữa hành động chính và phụ.
- Logo HIT ME hiển thị tốt.
- Top Bar gọn gàng.
- Chơi nhanh và Phòng riêng nổi bật.
- Secondary Feature Cards nhất quán.
- Bottom Navigation hoạt động.
- Các chức năng Bot cũ vẫn truy cập được.
- Tiếng Việt hiển thị đúng.
- Responsive trên các kích thước mobile đã chọn.
- Unity Web Build chạy được.
- Không phá vỡ gameplay.
- Không có chức năng giả mạo hoạt động.

---

# PHẦN 12 — BÁO CÁO

Tạo:

`docs/SPRINT_5_MAIN_MENU_REPORT.md`

Báo cáo:

1. Những file đã chỉnh sửa.
2. Scene và Prefab đã cập nhật.
3. Asset đã sử dụng.
4. Asset còn thiếu.
5. Các button đã kết nối.
6. Các button chưa khả dụng.
7. Kết quả tests.
8. Kết quả Web Build.
9. Ảnh chụp Main Menu.
10. Hướng dẫn kiểm tra trên mobile.

**BẮT ĐẦU TRIỂN KHAI TRỰC TIẾP TRÊN UNITY PROJECT HIỆN TẠI.**

Không chỉ đưa ra kế hoạch.

Không thay đổi gameplay hoặc multiplayer ngoài phạm vi cần thiết.

Không dùng mockup AI làm thành phẩm cuối.

Nếu gặp thiếu asset, hãy hoàn thiện kiến trúc và UI trước, sau đó báo cáo chính xác những asset cần bổ sung.