# HIT ME — SPRINT 3B
## UNITY 6 + CODEX — ARENA REDESIGN & WEB MOBILE INTEGRATION

Bạn là Senior Unity 6 Developer, Technical Artist, Gameplay Engineer và Build Engineer.

**MỤC TIÊU:** Tiếp tục phát triển HIT ME trên Unity 6 hiện tại, kết nối quy trình làm việc Codex với Unity Editor, thiết kế lại đấu trường thành hình chữ nhật bo góc và tạo bản Web có thể chơi trên trình duyệt mobile.

Không xây dựng lại dự án từ đầu.

---

## 1. KIỂM TRA MÔI TRƯỜNG UNITY

Hệ thống hiện tại:

- Unity Editor: 6000.6.4f1.
- Unity 2D URP.
- Web Build Support đã cài đặt.
- Unity Project nằm trong `unity-client/`.
- Đã hoàn thành Unity Foundation Sprint 1.
- Đã có Battle scene và gameplay prototype.
- Repository còn chứa prototype Phaser/TypeScript.

Yêu cầu:

1. Xác định đường dẫn Unity Editor thực tế.
2. Xác định đường dẫn Unity Project thực tế.
3. Kiểm tra phiên bản Editor tương thích.
4. Kiểm tra các package đang sử dụng.
5. Kiểm tra trạng thái Git.
6. Xác nhận khả năng chạy Unity Editor ở chế độ batchmode.

Nếu không tìm thấy Unity Editor, kiểm tra các vị trí cài đặt phổ biến trên Windows và yêu cầu tôi cung cấp đường dẫn nếu cần.

Không cài đặt lại Unity khi đã có phiên bản phù hợp.

Không tạo Unity Project mới.

---

## 2. KẾT NỐI CODEX VỚI UNITY

Thiết lập workflow:

Codex → Unity Project → Unity Editor → Tests → Web Build → Browser.

Codex phải:

- Chỉnh sửa trực tiếp C# scripts trong Unity Project.
- Sử dụng Unity Editor để import và compile.
- Kiểm tra Console.
- Chạy EditMode Tests.
- Chạy PlayMode Tests.
- Thực hiện Web Build.
- Thu thập build logs.
- Báo cáo lỗi chính xác.

Nếu Codex không thể điều khiển Unity Editor bằng giao diện, sử dụng Unity CLI/batchmode cho những thao tác hỗ trợ.

Không tự tuyên bố đã kết nối hoặc chạy Unity khi chưa xác minh.

Không chạy nhiều tiến trình Unity cùng mở một project gây xung đột.

---

## 3. THIẾT KẾ LẠI ĐẤU TRƯỜNG

Thay đấu trường elip hiện tại bằng **Rounded Rectangle Arena**.

Thông số đề xuất:

- Arena Width: 2000 logic units.
- Arena Height: 3500 logic units.
- Corner Radius: 300 logic units.
- Player Radius: giữ nguyên theo spec.
- Projectile Radius: giữ nguyên theo spec.

Tất cả thông số phải cấu hình được.

Yêu cầu:

- Hình chữ nhật dọc.
- Bốn góc bo tròn lớn.
- Không quá vuông.
- Không quá giống hình elip.
- Tận dụng tối đa diện tích màn hình mobile.
- Hỗ trợ 2–6 nhân vật.

Giữ lại EllipseArenaGeometry cũ để tương thích và rollback.

Tạo RoundedRectangleArenaGeometry hỗ trợ:

1. PointInside.
2. ClampPosition.
3. RayBoundaryIntersection.
4. ProjectileCollision.
5. PlayerPlacementValidation.
6. ArenaBounds.

Đảm bảo hitbox không vượt ra khỏi biên hợp lệ.

Không thay đổi các quy tắc chiến đấu đã chốt:

- Chỉ trúng mục tiêu đầu tiên.
- Timeout tự khóa hành động.
- Tất cả cùng chết thì hòa.
- Reveal khi tất cả khóa hoặc hết giờ.

Không sửa luật chơi khác nếu chưa được xác nhận trong spec.

---

## 4. THIẾT KẾ GIAO DIỆN MOBILE

Ưu tiên iPhone đời mới, Safari iOS và Chrome Android.

Thiết kế portrait responsive.

Kích thước kiểm thử:

- 360×800
- 390×844
- 393×852
- 402×874
- 412×915
- 430×932

Bố cục:

### HUD trên

- Vòng đấu.
- Timer.
- Avatar.
- HP.
- Nút cài đặt/ngôn ngữ.

### Khu vực trung tâm

- Rounded Rectangle Arena.
- 2–6 nhân vật.
- Vị trí đặt nhân vật.
- Hướng ngắm.
- Đường ném.
- Hiệu ứng trúng đòn.

### HUD dưới

- Chat.
- Nút SẴN SÀNG.
- Vũ khí.
- Hướng dẫn thao tác.

Yêu cầu:

- Hỗ trợ safe area.
- Không bị Dynamic Island che.
- Không bị thanh điều hướng iPhone che.
- Touch input chính xác.
- Không để HUD che sân.
- Không để chữ bị mờ.
- Sử dụng TextMeshPro với font hỗ trợ tiếng Việt.

Không thay đổi gameplay để giải quyết vấn đề giao diện.

---

## 5. CẢI THIỆN HÌNH ẢNH

Phong cách mục tiêu:

- 2D chibi Việt Nam.
- Cartoon sắc nét.
- Màu sắc tươi sáng.
- Viền nhân vật rõ.
- Bóng đổ dưới chân.
- Không dùng pixel art.
- Không dùng primitive làm nhân vật cuối cùng.

Chuẩn bị hệ thống hỗ trợ các bản đồ:

- Làng quê Bắc Bộ.
- Chợ Việt Nam.
- Vịnh Hạ Long.
- Cố đô Huế.
- Chợ nổi Cái Răng.
- Tây Bắc.

Mỗi bản đồ sử dụng chung logic Rounded Rectangle Arena.

Tách biệt:

- Gameplay Geometry.
- Arena Background.
- Decorative Environment.
- Character Sprites.
- UI Overlay.

Nếu chưa có asset PNG thật, giữ placeholder tạm thời và báo cáo rõ asset cần bổ sung.

Không dùng ảnh concept tổng hợp làm sprite sheet gameplay.

---

## 6. UNITY WEB PERFORMANCE

Ưu tiên:

- Web Build tương thích mobile.
- Sprite Atlas.
- Giảm draw calls.
- Giảm overdraw.
- Texture compression phù hợp.
- Hạn chế GC allocation.
- Không tải toàn bộ bản đồ cùng lúc.
- Loading screen.
- Quality presets.

Mục tiêu:

- 60 FPS trên thiết bị đủ mạnh.
- Có phương án giảm chất lượng để đạt 30 FPS ổn định.
- Không có lỗi runtime nghiêm trọng.
- Không bị cắt màn hình.

Các mục tiêu hiệu năng phải được đo thực tế, không tự đánh dấu đạt.

---

## 7. TỰ ĐỘNG HÓA BUILD

Tạo Editor script:

`Assets/HitMe/Editor/HitMeWebBuild.cs`

Chức năng:

- Xác định scene cần build.
- Kiểm tra Web Build Support.
- Build Unity Web.
- Ghi logs.
- Báo cáo trạng thái.
- Trả mã lỗi khác 0 khi build thất bại.

Tạo script PowerShell:

`tools/build-web.ps1`

Script phải:

1. Kiểm tra Unity Editor.
2. Kiểm tra Unity Project.
3. Không build khi Unity đang khóa project.
4. Gọi Unity batchmode.
5. Lưu logs.
6. Kiểm tra output.
7. Báo cáo thành công/thất bại.

Nếu có thể, bổ sung script chạy local HTTP server để kiểm tra Web Build.

Không mở trực tiếp `index.html` qua file://.

---

## 8. KIỂM THỬ

Bắt buộc:

### EditMode

- Rounded Rectangle PointInside.
- ClampPosition.
- RayBoundaryIntersection.
- Player boundary collision.
- Projectile boundary collision.
- First target hit.
- Match result.
- Timeout.
- Reveal.

### PlayMode

- Battle scene load.
- Nhân vật hiển thị đúng.
- Đặt vị trí.
- Kéo ngắm.
- Ready.
- Reveal.
- Throw.
- Result.

### Web

- Build thành công.
- Game tải được qua HTTP/HTTPS.
- Không có lỗi JavaScript nghiêm trọng.
- Không có lỗi Unity runtime nghiêm trọng.
- Các nút hoạt động.
- Text tiếng Việt hiển thị đúng.
- Không bị cắt HUD.

Nếu không có thiết bị iPhone thực tế, ghi rõ chưa kiểm thử Safari iOS trên thiết bị thật.

---

## 9. BẢO VỆ DỰ ÁN

Không được:

- Xóa code Phaser/TypeScript.
- Xóa Unity Foundation.
- Viết lại toàn bộ Battle scene.
- Thay đổi CombatResolver không cần thiết.
- Thay đổi HP/damage.
- Xóa test cũ.
- Tạo Unity Project thứ hai.
- Tự thêm multiplayer online.
- Tự tích hợp Supabase.

Mọi thay đổi phải có thể rollback bằng Git.

---

## 10. BÀN GIAO

Tạo báo cáo:

`docs/SPRINT_3B_UNITY_INTEGRATION_REPORT.md`

Báo cáo:

1. Unity Editor path.
2. Unity Project path.
3. Các file đã sửa.
4. Rounded Rectangle Geometry đã triển khai.
5. Kết quả EditMode Tests.
6. Kết quả PlayMode Tests.
7. Kết quả Web Build.
8. Đường dẫn Web Build.
9. Cách chạy thử local.
10. Các hạn chế còn tồn tại.

Nếu có khả năng chụp màn hình browser thực tế, cung cấp ảnh Battle scene sau khi build.

**BẮT ĐẦU THỰC HIỆN NGAY. KHÔNG CHỈ VIẾT KẾ HOẠCH.**

Tiếp tục đến khi hoàn thành các bước có thể thực thi, hoặc gặp trở ngại thực sự cần tôi xử lý.