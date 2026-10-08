# Sprint 1 — checklist nghiệm thu

Các kết quả thực thi cuối sprint nằm trong `SPRINT1_REPORT.md`, log và XML; checklist thủ công dưới đây không tự đánh dấu pass.

- [x] Editor CLI mở project bằng Unity 6000.6.4f1; compile và PlayMode không lỗi Console ngoài dự kiến.
- [x] Pipeline là URP Renderer2D, có đủ Boot/MainMenu/Lobby/CharacterSelect/Battle/Result.
- [x] Battle Play có sân elip và nền khán đài placeholder đầy màn, không dải đen trống.
- [x] Preview có một player/hai bot, tên/tim/hitbox chân; nhãn rõ đây là layout preview.
- [x] Placement ẩn bot; thao tác tap/drag ngắm đúng điểm tường.
- [x] SẴN SÀNG chỉ khóa input cá nhân, deadline không rút ngắn.
- [x] Không tự xử lý chiến thắng/sát thương/timeout chưa chốt.
- [x] Geometry và state test chạy qua trong Unity Test Runner: 22/22.
- [x] Bốn cỡ Game View 360×800, 390×844, 412×915, 430×932: HUD không cắt, arena ≥92% rộng/≥70% cao; 3/3 PlayMode qua.
- [ ] Notch/Dynamic Island và thanh điều hướng: nút nằm trong safe area trên thiết bị thật.
- [x] Font nhúng đủ glyph VI/EN/tim, kiểm tra cmap và UI thực tế; OFL đính kèm. Art thật còn chờ.
- [x] Web build thật thành công, server HTTP tải WASM/data/loader, Menu/Battle và input hoạt động trên IAB Chromium 390×844/DPR 2.
- [ ] Safari iOS/Chrome Android thực tế: tap/drag, đổi tab, resize/context loss được kiểm tra.
- [ ] Đo FPS, texture memory, loading time trên máy thật trước tuyên bố hiệu năng.
- [x] Hash 48 file Phaser/TypeScript và output khớp baseline trước sprint.
