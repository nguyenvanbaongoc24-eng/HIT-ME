# Gate 1 — xác minh Web thực tế

Ngày 09/10/2026, Asia/Bangkok. **Gate 1: PASS** theo phạm vi Main Menu functional/responsive. Không phải nghiệm thu toàn bộ game hoặc thiết bị thật.

- Xác minh listener HTTP `127.0.0.1:8791`, PID 21644, Python `tools/serve-web.py --port 8791`; WS `127.0.0.1:8788`, PID 31352.
- URL thực tế: http://127.0.0.1:8791/?performance=1. HTML/loader/data/framework/wasm trả HTTP 200; xem GATE1_HTTP_VERIFICATION.json.
- Tab 1, browser 2 đã chuyển sang HTTP, title HIT ME. Không còn blocker data:error. Reload qua HTTP khởi động Main Menu thành công.
- Thao tác thật: Practice mở thiết lập 2 bot/Normal; PlayBots vào Battle, placement hiện nhân vật bản thân, Ready khóa input. Đối thủ không xuất hiện trong placement.
- Settings chuyển VI→EN→VI; Close về Home. Phòng riêng kết nối backend thật, tạo phòng 58878369, rời phòng; Back về Home. Quick Match tạo phòng chờ 6CBFC464, rời phòng. Inventory hiện chảo/dép/vợt từ service; Back hoạt động. Profile Quest có 0 coins trước/sau navigation.
- Screenshot trực tiếp trong tool đã kiểm tra Main Menu 390×844 và 430×932, Settings EN, Battle, Lobby, Inventory. Sau resize cần chờ canvas cập nhật một frame; ảnh ổn định không clipping. Không lưu các ảnh browser thành file; ảnh Unity PlayMode đã có trong báo cáo Phase 1.
- Console browser captured warn/error: danh sách rỗng trong các lần kiểm tra. Không đồng nghĩa mọi lỗi trên thiết bị/phiên khác đều đã được loại trừ.
- Kiểm thử Unity Phase 1 trước đó: EditMode 75/75, PlayMode 19/19; backend 16/16. Build Phase 1 Succeeded, 0 errors, 5 warnings.

## Pending Manual Verification

Result của một trận Web trọn vẹn chưa được kiểm tra trong lần smoke này. Trận có thể kéo dài khi liên tục miss; không ép kết quả hoặc tự thêm giới hạn vòng. Result regression hiện có ở Unity tests. iPhone Safari/Android Chrome thật, keyboard/touch thật, background/reconnect, đo FPS/memory: NOT RUN. Counter hiện trên Web không phải bằng chứng đạt target thiết bị.

UI Kit được phép bắt đầu; các gate UI Kit/Artwork/Motion chưa được đánh dấu PASS.
