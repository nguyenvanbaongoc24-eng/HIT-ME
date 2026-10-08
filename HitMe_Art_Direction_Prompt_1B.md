# HIT ME — Định hướng hình ảnh & Prompt 1B (tham chiếu phong cách Overcooked, làm 2D trước)

> **Vị trí trong lộ trình:** file này **thay thế** mục 2 (giao diện đấu trường) và mục 3 (nhân vật) của Prompt 1 trong `HitMe_UI_Prompts_for_Codex.md`, đồng thời nâng cấp phần HUD. Các mục về bố cục mobile dọc, ẩn vị trí đối thủ, thao tác chạm/kéo, REVEAL/THROW (mục 1, 4, 5, 6, 7 của Prompt 1) **giữ nguyên**. Nếu đã chạy Prompt 1 rồi thì chạy Prompt 1B như bước nâng cấp.
> Đọc kèm: `HitMe_Spec_for_Antigravity.md` (luật chơi, không đổi).

---

## 0. Ranh giới: lấy cảm hứng, không sao chép

Ảnh tham chiếu là game Overcooked. Mục tiêu là **chất lượng và cảm giác hình ảnh tương đương** (tươi sáng, dễ thương, "mọng", dễ đọc), **không** sao chép nhân vật, bố cục, icon, UI hay bất kỳ asset cụ thể nào của game đó (có bản quyền). Mọi thiết kế phải là của riêng Hit Me: đấu trường La Mã, nhân vật và vũ khí Việt Nam. Khi tạo asset bằng AI, **mô tả đặc điểm phong cách** (xem mục 6), không ghi tên game hay nhân vật có bản quyền vào prompt.

---

## 1. "DNA phong cách" rút ra từ ảnh tham chiếu

| # | Đặc điểm trong ảnh | Vì sao nó đẹp và dễ chơi |
|---|---|---|
| 1 | **Bảng màu ấm, bão hòa cao**: sàn cam, quầy xanh ngọc, tường gạch nâu đỏ | Cặp màu cam và xanh ngọc tương phản mạnh, sân chơi nổi bật hẳn khỏi nền |
| 2 | **Hình khối tròn, mập, đơn giản**, không viền đen dày; chuyển sắc mềm | Cảm giác "đồ chơi", thân thiện, nhìn rõ ở kích thước nhỏ |
| 3 | **Chiều sâu giả**: tường có độ dày, thấy mặt trong, bóng tiếp xúc dưới mọi vật | Phẳng mà vẫn có khối; mắt hiểu ngay vật nào đứng ở đâu |
| 4 | **Nhân vật to so với sân**, đầu to, mũ/đồ đội đầu riêng, màu riêng | Nhận ra từng người trong 1 giây |
| 5 | **Môi trường kể chuyện ngoài sân chơi**: cây, chậu cây, khách ngồi bàn, đường phố | Thế giới sống động, sân chơi không bị "trống" |
| 6 | **HUD kiểu phiếu giấy** (order ticket) xếp hàng trên cùng, có icon | Thông tin gọn, đẹp, đọc nhanh |
| 7 | **Hiệu ứng "juice"**: lửa, khói, hơi nước, rung, phản ứng của nhân vật | Mọi sự kiện đều có phản hồi hình ảnh rõ ràng |
| 8 | **Sân chơi sáng, rìa tối hơn**: khung ngoài tối nhẹ, giữa sân sáng | Dẫn mắt người chơi vào vùng thao tác |

## 2. Dịch sang Hit Me (2D, nhìn từ trên xuống)

| Overcooked (3D) | Hit Me (2D) |
|---|---|
| Sàn gạch cam | Sàn cát/gạch đá ấm, lát hình vòng cung đồng tâm, có vết chân, vài vết bụi |
| Quầy xanh ngọc, tường gạch đỏ | **Tường đấu trường** đá nung/terracotta dày, mặt trong có vòm cung và cột, viền vàng; điểm nhấn xanh ngọc (băng rôn, cờ) |
| Khách hàng ngồi bàn bên ngoài | **Khán giả** trên khán đài quanh đấu trường: nhiều màu áo, vẫy tay, nhảy, phản ứng khi có người trúng đòn |
| Cây, chậu cây | Cây ô liu trong chậu, đuốc, tượng, băng rôn, lá nguyệt quế |
| Phiếu order trên đầu màn hình | **Thẻ người chơi** kiểu phiếu: avatar, tên, 3 trái tim (xem mục 4.4) |
| Lửa, khói | Ngọn lửa đuốc nhấp nháy, bụi khi nhân vật xuất hiện, sao va chạm, chữ "BỐP!" kiểu truyện tranh |
| Camera nghiêng 3/4 | MVP: **nhìn từ trên xuống thật** (vòng tròn đúng hình) để hướng ngắm và hitbox khớp tuyệt đối với màn hình. Nhân vật vẽ nhìn thẳng (billboard) đứng trên sàn, có bóng dưới chân |

**Lưu ý kỹ thuật quan trọng (độ nghiêng camera):** nếu sau này muốn ép sân thành hình elip để có cảm giác 3/4, đường ngắm và hitbox hiển thị sẽ lệch với logic. Vì vậy cài đặt `VIEW_TILT_Y = 1.0` (không nghiêng) trong config, và **mọi chuyển đổi tọa độ phải đi qua đúng hai hàm** `worldToScreen()` và `screenToWorld()` để sau này đổi độ nghiêng mà không phải sửa nơi khác. Không tự chỉnh `VIEW_TILT_Y` khác 1.0 nếu chưa có yêu cầu.

**Nguyên tắc hitbox hiển thị (quan trọng cho công bằng):** hitbox của nhân vật là **vòng tròn bóng dưới chân** (contact shadow). Sprite có thể cao hơn hitbox (đầu to, mũ), nhưng **chỉ phần chân/bóng mới tính trúng**. Hiển thị vòng bóng này đủ rõ (tối, viền nhẹ) để người chơi hiểu mình bị trúng vì sao.

---

## 3. Art bible (các giá trị mặc định, đặt hết vào `theme` config)

**Bảng màu** (token):
- Nền ngoài (dusk): `#3A2438` → `#2A1A2B` (gradient dọc)
- Sàn sáng / sàn tối / ron gạch: `#F6D29A` / `#E9B472` / `#D79F5F`
- Tường đỉnh / mặt trong / điểm sáng: `#C65D3B` / `#8F3B26` / `#E07B54`
- Xanh ngọc (băng rôn, cờ): `#2FA7A0`, đậm: `#1E7A76`
- Vàng viền: `#F2B544`
- Tim đầy: `#EF4B5A`, tim rỗng: `#6B4A55`
- Màu người chơi (6): xanh dương `#3BA9FF`, đỏ `#FF5A5F`, xanh lá `#4CC36B`, vàng `#FFC93C`, tím `#A06CFF`, hồng `#FF7EB6`
- **Không chỉ dựa vào màu để phân biệt người chơi:** mỗi người có thêm huy hiệu/mũ/số khác nhau (hỗ trợ người mù màu).

**Hình khối và đổ bóng:**
- Bo tròn mọi góc; tránh viền đen dày. Dùng viền tối nhẹ cùng tông cho vật cần tách nền.
- Mỗi vật đứng trên sàn có **bóng tiếp xúc mềm** (hình elip mờ). Bóng đổ nhẹ về một hướng nhất quán (ánh sáng từ trên-trái).
- Sân sáng nhất ở giữa, tối dần (vignette) ra rìa màn hình.

**Chữ:** font tròn đậm, thân thiện, **hiển thị đúng dấu tiếng Việt** (ví dụ Baloo 2 hoặc Nunito; phải kiểm tra thực tế các dấu "ă â ê ô ơ ư ạ ả ễ ..." trước khi chốt). Có fallback.

**Nhân vật (chibi):** đầu ~50–55% chiều cao, thân tròn mập, tay chân ngắn, mặt đơn giản (2 mắt, miệng). Mỗi người một kiểu tóc/mũ khác nhau. Trạng thái: idle (nhún nhẹ), aim (nghiêng người, mắt nhìn theo hướng ngắm), throw (co giãn squash & stretch), hit (choáng, sao quay quanh đầu), dead (xoay tròn bay ra rìa, mặt buồn).

---

## 4. PROMPT 1B — dán cho Codex

```
Trước khi làm, đọc docs/HitMe_Spec_for_Antigravity.md, docs/HitMe_UI_Prompts_for_Codex.md và docs/HitMe_Art_Direction_Prompt_1B.md.

Nhiệm vụ: nâng chất lượng hình ảnh màn đấu lên mức "tươi sáng, dễ thương, bóng bẩy như game casual 3D hiện đại nhưng làm 2D", theo đúng mục 1–3 của file Art Direction. KHÔNG đổi luật chơi và KHÔNG sao chép asset/UI của bất kỳ game nào.

Quy tắc kỹ thuật:
- Tạo file theme (ví dụ src/theme/theme.ts) chứa toàn bộ màu, kích thước, thời lượng hiệu ứng theo mục 3. Không hard-code.
- Mọi chuyển đổi tọa độ qua worldToScreen() / screenToWorld() với VIEW_TILT_Y = 1.0.
- Hiệu năng: mục tiêu 60fps trên máy mobile tầm trung (kiểm tra bằng CPU throttling 4x trong DevTools). Vẽ các lớp TĨNH (nền, khán đài, tường, sàn) MỘT LẦN vào offscreen canvas/texture rồi tái sử dụng; mỗi khung hình chỉ vẽ lại lớp động (nhân vật, vũ khí, hạt, khán giả). Giới hạn devicePixelRatio tối đa 2. Hạt: tối đa ~150 hạt cùng lúc. Khán giả: tối đa ~80 sprite trên mobile.
- Chưa có art thật: vẽ bằng code (canvas/SVG/CSS) với gradient, bo tròn, bóng mềm; cấu trúc mọi sprite theo "slot" để sau này thay bằng ảnh PNG/atlas mà không sửa logic (mỗi slot có id và kích thước chuẩn, đọc từ manifest assets/manifest.json; nếu không có file thì dùng placeholder vẽ bằng code).

1) Cảnh (vẽ từ sau ra trước)
   a. Nền dusk gradient + vignette.
   b. Khán đài quanh đấu trường: nhiều hàng ghế bậc thang; khán giả là sprite đơn giản nhiều màu áo, mỗi người nhún nhẹ lệch pha nhau.
   c. Tường đấu trường đá nung/terracotta DÀY, thấy rõ mặt trong (vòm cung, cột lặp lại, viền vàng), có bóng đổ xuống sàn. Điểm nhấn xanh ngọc: băng rôn/cờ treo ở vài vòm, đuốc có lửa nhấp nháy ở các hướng chính.
   d. Mặt sân: gạch/cát ấm lát vòng cung đồng tâm, ron gạch nhẹ, vài vết chân và bụi mờ, sáng nhất ở giữa.
   e. Trang trí ngoài sàn: cây ô liu trong chậu, tượng, lá nguyệt quế; vài thứ đặt đối xứng, không che vùng thao tác.
   f. Vạch chia hướng cũ: bỏ hoặc làm rất mờ nếu không cần thiết.
   g. Chủ đề lấy từ config (roman | greek | stadium), chỉ cần làm "roman" nhưng cấu trúc để thêm chủ đề khác chỉ bằng thay bộ theme/assets.

2) Nhân vật chibi (placeholder vẽ bằng code, sẵn sàng thay bằng layer sprite)
   - Theo mục 3 "Nhân vật": đầu to, thân tròn, mũ/tóc riêng, màu riêng + huy hiệu/số phân biệt.
   - Bóng tiếp xúc dưới chân = hitbox hiển thị; kích thước hitbox lấy từ config (PLAYER_RADIUS_RATIO, thử 0.08–0.12 bán kính sân; để một biến duy nhất cho cả hiển thị và va chạm).
   - Layer: bóng, thân, mặt, tóc/mũ, áo, vũ khí cầm tay. Trạng thái idle/aim/throw/hit/dead như mục 3.
   - Nhân vật của bạn có vòng sáng và mũi tên nhỏ phía trên đầu.

3) HUD kiểu "thẻ phiếu" (không copy UI game nào, tự thiết kế)
   - Hàng thẻ trên cùng (tối đa 6): mỗi thẻ là tấm giấy/da bo góc nghiêng nhẹ, có avatar, tên, 3 trái tim. Thẻ của bạn được làm nổi bật. Người bị loại: thẻ xám đi, có dấu X rồi trượt ra.
   - Trái tim vỡ có animation khi mất máu.
   - Giữa phía trên: đồng hồ đếm ngược số lớn 5→1, đập nhịp mỗi giây, đổi sang đỏ khi còn dưới 2 giây (kết hợp thanh/vòng chạy).
   - Nút chat 💬 và cài đặt ⚙ dạng nút tròn bo mềm, có bóng.
   - Dòng hướng dẫn trong khung băng giấy nhỏ.

4) "Juice" (tối thiểu các hiệu ứng sau)
   - Chạm đặt vị trí: gợn sóng tròn trên sàn + tiếng "tick" (hook SFX).
   - Vạch ngắm: nét đứt chạy, đầu mũi tên đập nhịp.
   - REVEAL: nhân vật nảy ra từ 0 lên có overshoot, kèm đám bụi và vòng sóng.
   - THROW: nhân vật squash & stretch, vũ khí xoay, có vệt đuôi (màu/độ đẹp theo tier vũ khí).
   - HIT: ngôi sao va chạm, chữ truyện tranh "BỐP!" bật lên và biến mất, "-1" bay lên, rung màn hình rất nhẹ (~6px/120ms, có công tắc tắt), nhân vật choáng với sao quay quanh đầu, trái tim vỡ.
   - ELIMINATE: nhân vật xoay tròn bay ra rìa kèm vệt sao chổi, banner "<tên> bị loại".
   - KHÁN GIẢ phản ứng: giật mình/nhảy khi có người trúng đòn, reo mừng khi có người bị loại hoặc kết thúc trận.
   - WIN: pháo giấy, khán giả nhảy.
   - Tất cả hiệu ứng có hook SFX (WebAudio beep tạm), công tắc tắt trong cài đặt.

5) Chuyển động nền
   - Lửa đuốc nhấp nháy, cờ phấp phới nhẹ, khán giả nhún. Tắt/giảm khi đang ở chế độ tiết kiệm pin (config LOW_POWER).

6) Chế độ hiệu năng
   - Thêm config QUALITY = high | medium | low: low giảm khán giả, hạt, tắt rung và đổ bóng mềm. Tự chọn mặc định theo hiệu năng đo được (frame time) hoặc cho đổi trong cài đặt.

Tiêu chí nghiệm thu (so với ảnh tham chiếu, đối chiếu từng mục):
- [ ] Bảng màu ấm bão hòa, cặp cam/xanh ngọc nổi bật; sân sáng, rìa tối.
- [ ] Tường có độ dày, thấy mặt trong, có bóng đổ xuống sàn.
- [ ] Mọi vật đứng trên sàn đều có bóng tiếp xúc mềm.
- [ ] Có khán đài + khán giả + đồ trang trí bên ngoài sân; khán giả phản ứng với sự kiện.
- [ ] Nhân vật chibi đọc rõ ở 360x800 (không còn là chấm tròn); phân biệt được 6 người không chỉ bằng màu.
- [ ] HUD dạng thẻ phiếu đúng mô tả, tim vỡ có animation, đếm ngược số lớn.
- [ ] Đủ các hiệu ứng ở mục 4 (tối thiểu 8 loại), có hook SFX.
- [ ] 60fps ở 390x844 với CPU throttling 4x (hoặc báo cáo số liệu thật nếu không đạt và đề xuất giảm gì).
- [ ] Hitbox hiển thị (vòng bóng dưới chân) khớp đúng với va chạm trong logic.
- [ ] Không có thành phần nào sao chép asset/UI từ game khác.
```

---

## 5. Giới hạn cần biết

**Vẽ bằng code đạt đến đâu?** Code thuần túy làm được phong cách phẳng/cartoon sạch, có bóng và hiệu ứng, đủ để thấy "đúng hướng". Nhưng **độ bóng bẩy như ảnh tham chiếu** (khối 3D render, vật liệu, ánh sáng) thì cần **asset thật** (sprite do họa sĩ vẽ hoặc tạo bằng AI rồi chỉnh). Vì vậy quy trình đề xuất là: Prompt 1B làm khung và placeholder đẹp trước → bạn làm bộ asset → thay vào qua `assets/manifest.json` mà không sửa logic.

**2D trước, 3D sau:** giữ tầng hiển thị tách khỏi tầng luật (`game-core`) để sau này đổi sang low-poly 3D chỉ cần thay renderer. Ảnh tham chiếu bản chất là 3D; bản 2D sẽ là "tương tự về cảm giác", không giống hệt.

---

## 6. Quy trình asset thật (sau khi Prompt 1B chạy ổn)

### 6.1 Danh sách asset cần có
**Môi trường:** gạch sàn (ô lặp 256×256), đoạn tường vòm cung (lặp), cột, băng rôn x3 màu, đuốc (4 khung hình), cây ô liu trong chậu, tượng, ghế khán đài, khán giả x6 mẫu (2 khung hình nhún).
**Nhân vật (nhìn thẳng, nền trong suốt):** thân nam, thân nữ; tóc x3; mặt x3; trang phục x4 (quần đùi, áo ba lỗ, áo vest, váy cưới); 6 màu/huy hiệu. Khung hình: idle 2, aim 1, throw 3, hit 2, dead 1.
**Vũ khí (icon 128px + sprite bay):** xe đạp, xe máy cũ (đặt tên chung, không dùng nhãn hiệu thật), dép tổ ong, cốc bia, chó vàng (hoạt hình dễ thương). Mỗi tier có vệt đuôi và hiệu ứng trúng riêng.
**UI:** thẻ phiếu người chơi, tim đầy/rỗng/vỡ, nút tròn bo mềm, khung băng giấy, icon chat/cài đặt, bảng cửa hàng.
**FX:** đám bụi, ngôi sao va chạm, chữ "BỐP!", pháo giấy, tia sáng.
**Âm thanh:** chạm đặt, tick đếm ngược, vút khi ném, "bốp" khi trúng, loại, thắng, khán giả reo/giật mình, nhạc nền lặp.

### 6.2 Mẫu prompt tạo ảnh bằng AI (viết tiếng Anh cho công cụ tạo ảnh)
Mô tả đặc điểm phong cách, **không nhắc tên game hay nhân vật có bản quyền**:

```
cute chibi cartoon character, front view, big head about 50% of body height, thick rounded shapes, soft cel shading, clean vector-like look, warm bright saturated colors, no heavy black outline, subtle soft shadow, plain transparent or solid flat background, game sprite, consistent proportions, [mô tả: hairstyle / outfit / color]
```
```
casual mobile game prop icon, Vietnamese honeycomb rubber sandal (dep to ong), chunky rounded shapes, soft shading, warm saturated colors, front view, clean flat background, game asset
```
Mẹo giữ nhất quán: tạo một tấm "model sheet" nhân vật trước rồi dùng làm ảnh tham chiếu cho các lần sau; giữ cùng một công cụ, cùng seed/độ phân giải; xóa nền thành PNG trong suốt; xếp vào atlas có tên file nhất quán (ví dụ `char_male_hair01.png`). **Kiểm tra điều khoản bản quyền/thương mại của công cụ tạo ảnh** trước khi dùng trong sản phẩm bán ra.

### 6.3 Cách đưa vào dự án
Bỏ file vào `assets/`, khai báo trong `assets/manifest.json` (id → đường dẫn, kích thước, điểm neo). Codex đã được yêu cầu để code đọc manifest và rơi về placeholder nếu thiếu file.

---

## 7. Gửi lại cho mình khi xong
Gửi ảnh chụp màn hình ở 390×844 (pha đặt vị trí, pha ném/trúng, màn kết thúc) kèm báo cáo của Codex, mình sẽ đối chiếu với các tiêu chí ở mục 4 và đề xuất chỉnh tiếp.
