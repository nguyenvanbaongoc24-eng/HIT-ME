# HIT ME — Bộ prompt làm giao diện (dành cho Codex trên Antigravity)

> Cách dùng: đặt file này và `HitMe_Spec_for_Antigravity.md` vào thư mục `docs/` của dự án. Dán **từng prompt một** (Prompt 1 → 2 → 3) cho Codex, chạy và kiểm tra xong prompt trước mới sang prompt sau.
> Luật chơi và các con số cân bằng đã nằm trong file spec; file này chỉ nói về **giao diện và trải nghiệm**.

---

## 0. Hiện trạng prototype (từ ảnh chụp màn hình)

**Đang có (giữ lại):**
- Đấu trường tròn, viền vàng, có vạch chia 12 hướng; nền tối.
- 1 người chơi (xanh, nhãn "Bạn 2/3") + 3 bot (đỏ, nhãn "Bot 1 3/3"...), hiển thị tên và máu dạng số dưới mỗi nhân vật.
- HUD chữ ở góc trái: Tên game, Vòng, Máu, Còn sống, đếm ngược pha đặt vị trí.
- Cách chơi: chạm trong đấu trường để đặt vị trí, kéo ra để chọn hướng ném; có đường ngắm màu xanh tới tường.
- Dòng hướng dẫn ở cuối màn hình.

**Chưa đúng so với thiết kế mong muốn:**
1. **Bố cục ngang kiểu desktop**, trong khi game chủ yếu chơi trên **mobile dọc**; đấu trường phải chiếm **70–80% màn hình** (theo tài liệu thiết kế).
2. **Chưa có bối cảnh đấu trường La Mã/Hy Lạp/sân vận động**: hiện chỉ là hình tròn trơn.
3. **Lộ vị trí đối thủ trong pha đặt vị trí:** ảnh chụp cho thấy cả 3 bot hiện sẵn khi đồng hồ còn đếm ngược. Theo luật, người chơi **không được biết vị trí của nhau** trước khi lộ diện. Phải sửa (xem Prompt 1, mục 4).
4. HUD chỉ là chữ thô; chưa có thanh/vòng đếm ngược, chưa có biểu tượng máu (tim), chưa có nút chat, cài đặt.
5. Nhân vật còn là ảnh tròn đơn giản; chưa có hiệu ứng lộ diện, ném vũ khí, trúng đòn, loại.
6. Chưa có các màn hình ngoài trận: đăng nhập, menu, tạo nhân vật, store, cài đặt, kết quả.

**Lưu ý kích thước quan trọng:** trong spec, `PLAYER_RADIUS = 30` trên `ARENA_RADIUS = 1000` (3%). Trên màn hình điện thoại (đấu trường đường kính ~300–340px) nhân vật chỉ còn ~10px, **quá nhỏ để nhìn và để thao tác**. Khi chuyển sang mobile phải tăng tỉ lệ này (đề xuất bắt đầu thử ở 8%, tức `PLAYER_RADIUS = 80`, và `PROJECTILE_RADIUS = 20`), đồng thời **hitbox phải khớp kích thước hiển thị**. Đây là thay đổi cân bằng, phải để trong `shared-config` và cần playtest.

---

## 1. Quy tắc chung cho mọi prompt

Dán đoạn này vào đầu mỗi prompt (hoặc vào file `AGENTS.md` của repo):

```
Trước khi làm, đọc toàn bộ docs/HitMe_Spec_for_Antigravity.md và docs/HitMe_UI_Prompts_for_Codex.md.

Quy tắc:
- Không đổi luật chơi (mục 2–3 của spec). Chỉ đổi giao diện/trải nghiệm. Nếu thấy mâu thuẫn hoặc lỗ hổng, ghi vào mục "Câu hỏi" ở cuối báo cáo, không tự quyết.
- Giữ cấu trúc repo hiện tại của prototype. Không tái cấu trúc lớn. Nếu thuận tiện thì tách logic game (tính va chạm, trừ máu) ra module riêng không phụ thuộc UI.
- Mọi hằng số (kích thước, thời gian, màu, giá) đặt trong một file config duy nhất, không hard-code.
- Mọi chuỗi hiển thị dùng khóa i18n (vi.json, en.json), mặc định Tiếng Việt.
- Chưa có asset thật: dùng placeholder vẽ bằng code (canvas/SVG/CSS) hoặc emoji/hình khối. Không tải ảnh từ internet, không thêm thư viện nặng nếu không cần.
- Chạy được trên trình duyệt desktop VÀ giả lập mobile dọc (390x844, 360x800). Dùng Pointer Events, touch-action: none trên vùng đấu trường, chặn zoom/scroll khi chơi, tôn trọng safe-area (notch).
- Cuối mỗi lần làm: chạy ứng dụng, tự kiểm tra bằng trình duyệt ở 390x844 và 360x800, chụp màn hình, rồi báo cáo theo mẫu ở mục 5.
```

---

## 2. PROMPT 1 — Màn đấu trong trận (làm ngay)

```
Nhiệm vụ: nâng cấp màn hình đang chơi của prototype lên giao diện mobile dọc theo thiết kế Hit Me.

1) Bố cục mobile dọc
- Màn hình dọc, đấu trường hình tròn ở giữa, đường kính = 75% chiều rộng khung (không vượt 80%), canh giữa theo chiều dọc phần còn lại.
- Thanh trên: [Vòng N]  [Còn sống X/Y]  [nút Cài đặt ⚙]. Dưới thanh trên là THANH ĐẾM NGƯỢC pha đặt vị trí (thanh/vòng chạy hết trong PLACEMENT_TIME_MS = 5000 ms, đổi sang màu đỏ khi còn dưới 2 giây).
- Thanh dưới: [nút Chat 💬 ở góc trái] [vũ khí đang trang bị ở giữa, placeholder] [thanh máu của bạn: 3 trái tim]. Dòng hướng dẫn ngắn phía trên thanh dưới: "Chạm để đặt vị trí, kéo ra để chọn hướng ném".
- Trên desktop: khung giả lập mobile dọc canh giữa màn hình (tỉ lệ ~9:19.5), nền ngoài khung tối.

2) Giao diện đấu trường La Mã (vẽ bằng code, không dùng ảnh ngoài)
- Mặt sân: cát/đất nâu ấm có gradient nhẹ, vài hoa văn đồng tâm.
- Tường đấu trường: viền đá dày quanh vòng tròn với các vòm cung/cột lặp lại (SVG/canvas), có độ sâu đơn giản (bóng đổ).
- Bên ngoài tường: khán đài tối với khán giả là các chấm/hình bóng nhỏ, vài ngọn đuốc phát sáng nhẹ ở các hướng.
- Chủ đề đấu trường đặt trong config (roman | greek | stadium), hiện chỉ cần làm "roman"; để sẵn khung cho các chủ đề khác.
- Giữ vạch chia hướng mờ nếu giúp ngắm, nhưng không làm rối.

3) Nhân vật (placeholder, sẵn sàng thay bằng layer sprite sau này)
- Kích thước hiển thị theo config. Đề xuất PLAYER_RADIUS = 8% bán kính đấu trường, PROJECTILE_RADIUS = 2%. Hitbox dùng đúng giá trị này (cùng một nguồn config).
- Nhân vật bạn: viền sáng + nhãn "Bạn". Bot/đối thủ: màu khác nhau theo người, nhãn tên.
- Máu hiển thị bằng 3 chấm/tim nhỏ dưới nhân vật (không dùng chữ "2/3" nữa).
- Cấu trúc nhân vật tách thành các layer (thân, mặt, tóc, áo, vũ khí) dù hiện chỉ là placeholder, để sau này thay bằng sprite.

4) SỬA LỖI LUẬT: ẩn thông tin trong pha đặt vị trí
- Trong pha PLACEMENT chỉ hiển thị nhân vật CỦA BẠN. KHÔNG hiển thị vị trí hay hướng của bot/đối thủ.
- Thêm config SHOW_LAST_ROUND_GHOSTS (mặc định false). Nếu true thì hiện bóng mờ vị trí đối thủ ở vòng trước. Mặc định tắt vì spec yêu cầu người chơi không biết vị trí nhau.
- Danh sách "Còn sống" chỉ hiện số lượng và máu (nếu cần), không chỉ ra vị trí.

5) Thao tác đặt vị trí và ngắm (mobile)
- Chạm vào đấu trường: đặt nhân vật tại điểm chạm (kẹp trong đường tròn, cách tường tối thiểu bằng bán kính nhân vật).
- Giữ và kéo từ vị trí đó: hướng ném = hướng từ nhân vật đến ngón tay. Có ngưỡng kéo tối thiểu ~12px để một cú chạm đơn không bị tính là kéo.
- Khi kéo: vẽ vạch ngắm nét đứt từ nhân vật tới tường (kéo dài hết đường bay), kèm mũi tên ở đầu; vạch ngắm hiển thị bên ngoài vùng ngón tay che khuất.
- Có thể chạm lại để đổi vị trí/hướng cho đến khi hết giờ. Nếu hết giờ chưa chọn hướng thì mặc định hướng về tâm đấu trường.
- Hết giờ thì khóa hành động tự động. Thêm nút "Sẵn sàng" tùy chọn để khóa sớm.

6) Pha REVEAL và THROW (hiệu ứng)
- Khi hết giờ: tất cả nhân vật hiện ra cùng lúc (phóng to từ 0 lên kích thước thật, ~300ms, kèm gợn sóng nhẹ).
- Vũ khí bay đồng thời theo đường thẳng tới tường (~600–800ms), có vệt đuôi. Vũ khí xoay khi bay. Dùng placeholder (hình khối) cho vũ khí; cho phép gán sprite theo weapon id.
- Khi trúng: nhân vật nhấp nháy đỏ, chữ "-1" bay lên, rung màn hình rất nhẹ, tim mất một cái (animation tim vỡ). Vũ khí biến mất tại điểm trúng; vũ khí không trúng ai thì bay tới tường rồi biến mất.
- Loại: nhân vật mờ dần kèm dấu "X", có banner "Bot 2 bị loại" ngắn.
- Sau animation: 600ms nghỉ rồi sang vòng mới (PLACEMENT).
- Âm thanh: để sẵn hook phát SFX (ném, trúng, loại, đếm ngược) dùng WebAudio beep tạm; có công tắc tắt trong cài đặt.

7) Kết thúc trận
- Khi chỉ còn 1 người: overlay "CHIẾN THẮNG" hoặc "BẠN BỊ LOẠI" kèm bảng thứ hạng (placeholder), nút "Chơi lại" và "Về menu". (Chi tiết màn kết quả làm ở Prompt 3.)

Tiêu chí nghiệm thu:
- [ ] 390x844 và 360x800: đấu trường chiếm ~75% chiều rộng, không có thanh cuộn, không bị notch che.
- [ ] Chơi trọn một trận vs 3 bot bằng cảm ứng/giả lập cảm ứng.
- [ ] Trong pha đặt vị trí KHÔNG thấy vị trí bot (kiểm tra bằng mắt và bằng cách đọc DOM/canvas state).
- [ ] Hiệu ứng REVEAL → THROW → trúng → loại chạy đúng thứ tự, không giật.
- [ ] Mọi con số kích thước/thời gian nằm trong file config.
- [ ] Chuỗi hiển thị dùng khóa i18n (vi mặc định).
```

---

## 3. PROMPT 2 — Các màn hình ngoài trận (UI-only, dữ liệu giả)

```
Nhiệm vụ: thêm các màn hình ngoài trận cho Hit Me, mobile dọc, CHỈ làm giao diện và điều hướng với dữ liệu giả (mock). Chưa nối backend, chưa đăng nhập thật.

Điều hướng: Splash/Đăng nhập → Menu chính → (Chơi với Bot | Chơi Online [khóa, ghi "Sắp ra mắt"] | Nhân vật | Cửa hàng | Cài đặt). Có nút quay lại thống nhất.

1) Đăng nhập (UI-only)
- Nút: "Tiếp tục với Google", "Tiếp tục với Facebook", "Tiếp tục với Apple" (Apple đặt đúng thiết kế, hiển thị chuẩn), và "Chơi nhanh với tên khách (Quest)".
- Chọn Quest: tạo tên mặc định dạng "Quest_1234", cho nhập tên khách tùy ý. Ghi chú nhỏ: "Khách có thể liên kết tài khoản sau để giữ tiến trình".
- Các nút social chỉ chuyển sang menu với tài khoản giả, kèm hook rõ ràng (TODO) để nối auth thật ở M3.

2) Menu chính
- Hiển thị: tên, avatar, rank (tên bậc đặt trong config, không dùng chữ "Vàng" cho bậc), số vàng (mock 199).
- Nút lớn "Chơi với Bot". Các nút nhỏ: Nhân vật, Cửa hàng, Cài đặt.

3) Tạo/chỉnh nhân vật
- Chọn Nam/Nữ; kiểu tóc (3), khuôn mặt (3), trang phục (quần đùi, áo ba lỗ, áo vest, váy cưới). Xem trước nhân vật ở giữa màn hình (placeholder dạng layer), nút "Xác nhận".
- Dữ liệu lựa chọn lưu trong state, truyền được sang nhân vật trong trận.

4) Cửa hàng
- Lưới thẻ vũ khí làm đẹp chủ đề Việt Nam: Xe đạp, Xe máy cũ (KHÔNG dùng chữ "Honda"), Dép tổ ong, Cốc bia, Chó vàng (hoạt hình dễ thương). Mỗi thẻ: hình placeholder, tên, bậc (tier), giá, nhãn "Đã sở hữu/Đang dùng".
- Giá tính theo công thức trong config: price(tier) = round(BASE_PRICE * PRICE_MULTIPLIER^(tier-1)) (mặc định BASE_PRICE=199, PRICE_MULTIPLIER=1.5).
- Giá càng cao thì thẻ và hiệu ứng xem thử (preview) càng đẹp (viền, hạt sáng). Nút "Xem thử" phát hiệu ứng ném trên một sân nhỏ.
- Hiện số vàng; mua thì trừ vàng (mock), không đủ vàng thì báo.
- Nhấn mạnh trong UI: vũ khí chỉ làm đẹp, không đổi sức mạnh.

5) Cài đặt
- Nhạc nền và SFX có thanh trượt riêng; công tắc "SFX khi ném trúng người".
- Ngôn ngữ: Tiếng Việt / English (đổi ngay, không tải lại).
- Đổi tên: lần đầu miễn phí; từ lần thứ hai hiện giá (NAME_CHANGE_COST trong config) và trừ vàng. Đếm số lần đổi trong state.

Tiêu chí nghiệm thu:
- [ ] Đi được toàn bộ luồng Splash → Menu → từng màn hình → về lại, không có màn hình cụt.
- [ ] Đổi ngôn ngữ làm toàn bộ chuỗi đổi, không sót chuỗi cứng.
- [ ] Giá cửa hàng đúng công thức, đổi config thì giá đổi theo.
- [ ] Lựa chọn nhân vật và vũ khí đang dùng hiển thị trong trận.
```

---

## 4. PROMPT 3 — Màn kết quả, chat, kết bạn (UI-only, dữ liệu giả)

```
Nhiệm vụ: hoàn thiện phần xã hội và kết thúc trận, vẫn dùng dữ liệu giả.

1) Màn kết quả trận
- Bảng thứ hạng theo thứ tự loại: tên, avatar, hạng, số lần trúng đối thủ, số lần bị trúng.
- Phần thưởng: vàng (theo bảng trong config; mặc định chỉ top 3 và trận từ 4 người trở lên mới có vàng; trận có bot nhận hệ số giảm BOT_GOLD_FACTOR) và điểm rank (+/-), có animation số chạy.
- Với mỗi người chơi khác: nút "Kết bạn" (UI giả; bot thì ẩn nút này). Nút "Chơi lại", "Về menu".

2) Chat trong trận
- Nút 💬 mở khung chat thu gọn phía dưới, không che đấu trường quá nhiều. Có tin nhắn nhanh (preset): "Chào!", "Hay lắm!", "Ném trượt rồi 😅", "GG".
- Cho nhập tin tự do (giả lập); lọc từ tục bằng danh sách từ cấm đặt trong config (placeholder, chưa cần đầy đủ).
- Mỗi tin có nút báo cáo/chặn người gửi (UI giả). Tin của bot là tin giả lập định kỳ để thử giao diện.

3) Danh sách bạn
- Màn hình nhỏ trong Menu: danh sách bạn (mock), trạng thái online/offline, lời mời đang chờ.

Tiêu chí nghiệm thu:
- [ ] Sau trận hiện đúng bảng hạng, vàng và rank theo config.
- [ ] Chat mở/đóng mượt, không che nhân vật quá nhiều ở 360x800.
- [ ] Có nút báo cáo/chặn trên mọi tin của người khác.
```

---

## 5. Mẫu báo cáo mà Codex phải trả lời sau mỗi prompt

```
## Đã làm
- ...
## Cách chạy
- lệnh cài, lệnh chạy, đường dẫn
## Đã kiểm tra
- 390x844: ...
- 360x800: ...
- (kèm ảnh chụp màn hình)
## Thay đổi cấu hình/hằng số
- ...
## Chưa làm / hạn chế
- ...
## Câu hỏi cần chủ dự án quyết định
- ...
```

## 6. Những điểm cần playtest sau Prompt 1
1. Kích thước nhân vật và hitbox (8% có hợp lý, có khó né quá không).
2. 5 giây có đủ để vừa đặt vị trí vừa kéo hướng trên điện thoại không.
3. Có nên bật `SHOW_LAST_ROUND_GHOSTS` (hiện vị trí vòng trước) để giảm yếu tố may rủi không. Đây là thay đổi luật, phải do chủ dự án quyết định.
4. Cảm giác ngắm bằng ngón tay (ngón che nhân vật): vạch ngắm đủ rõ chưa.
