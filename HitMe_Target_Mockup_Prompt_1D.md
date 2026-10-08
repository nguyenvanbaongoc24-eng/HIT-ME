# HIT ME — Bám theo mockup mục tiêu: phân tích, quy trình asset và Prompt 1D

> **Vị trí trong lộ trình:** Prompt 1D **thay thế phần hiển thị của Prompt 1C** (mục 2, 4 và 6 của 1C). Giữ từ 1C: ý tưởng đấu trường elip trong `game-core`, độ sắc nét (mục 5), nguyên tắc "hitbox = vòng dưới chân", luồng Bước 1 (game-core + test). Giữ từ Prompt 1: thao tác chạm/kéo, ẩn vị trí đối thủ ở pha đặt vị trí, trình tự REVEAL → THROW.
> **Bước 0 (bạn làm):** lưu ảnh mockup vào `docs/reference/target_mockup.png`. Đây là **ảnh mục tiêu** để Codex và bạn đối chiếu.
> Đọc kèm: `HitMe_Spec_for_Antigravity.md`.

---

## 1. Mockup cho thấy gì và mỗi thứ làm bằng cách nào

Mockup là **ảnh concept tĩnh** (rất giống ảnh do AI tạo), không phải asset dùng được trực tiếp. Nó là mốc phong cách rất tốt: góc nhìn nghiêng từ trên cao, bảng màu ấm, đấu trường kiểu "sân vận động phố" Việt Nam, nhân vật chibi cầm vật dụng. Mỗi thành phần cần một cách làm khác nhau:

| Thành phần trong mockup | Cách tạo | Ghi chú |
|---|---|---|
| Nền đấu trường (sân cát, tường vòm, khán đài, quầy hàng, thùng gỗ, lốp xe, xe van cũ, cờ, khán giả) | **Asset ảnh "nền" (background plate)** tạo bằng AI hoặc họa sĩ, 1 ảnh toàn màn hình, **không có nhân vật và UI** | Phần quyết định 80% vẻ đẹp. Code không vẽ nổi |
| Nhân vật chibi nhìn chếch 3/4, cầm vũ khí | **Sprite nhân vật** (AI/họa sĩ), vũ khí tách riêng | Chất lượng cao, cần nhất quán giữa các tư thế |
| Vũ khí (dép tổ ong xanh, chảo, búa) | Sprite riêng, mỗi cái một ảnh | Kích thước hiển thị khác nhau nhưng **hitbox giống nhau** |
| Vòng dưới chân (xanh = bạn, đỏ = đối thủ) | **Code** | Vẽ theo phép chiếu của sân, chính là hitbox |
| Tên + tim trên đầu | **Code** | Chữ có viền tối |
| HUD: thẻ "Bạn", bảng "VÒNG 3 + đồng hồ cát", chip Bot, nút cài đặt, thanh dưới, nút "SẴN SÀNG" | **Code (CSS/SVG)**, icon từ SVG | Kiểu bảng gỗ nâu viền vàng, làm tốt được bằng code |
| Đám đông phía trước, cờ, cây | Nằm sẵn trong nền; có thể thêm chuyển động nhẹ bằng code | Giai đoạn sau |
| Bóng đổ nhân vật, bụi, hiệu ứng ném/trúng | **Code** | Như Prompt 1 mục 6 |

**Những điểm lệch giữa mockup và luật/spec cần lưu ý:**
- Đồng hồ trong mockup là **15 giây**; spec chốt **~5 giây** (`PLACEMENT_TIME_MS`). Giữ 5 giây, chỉ là config. Lưu ý 5 giây cho cả đặt vị trí lẫn kéo hướng trên điện thoại khá gấp; cần playtest.
- Thanh dưới có **"Nhân vật / Trang bị / Túi đồ"**: đây là chức năng của sảnh chờ. Trong trận nên khóa hoặc chỉ xem (xem mục 8, câu 3).
- Mockup có hai ngôn ngữ trên nút ("SẴN SÀNG / READY"); bản chạy chỉ hiện theo ngôn ngữ đang chọn.
- Mockup chỉ có 3 người; trận thật 2–6 người nên hàng chip phải co lại được.

---

## 2. Hình học và kích thước (cập nhật so với 1C)

**Quan trọng: 1C đặt `VIEW_TILT_Y = 1.0`. Prompt 1D đổi lại, vì mockup rõ ràng là góc nhìn nghiêng** (vòng dưới chân dẹt, nhân vật đứng thẳng).

**Nguyên tắc nhất quán:** mọi tính toán (vị trí, hướng ném, va chạm) nằm trong **không gian mặt sàn** (logic). Màn hình chỉ là phép chiếu của mặt sàn đó với độ nén dọc `VIEW_TILT_Y` (0 < tilt ≤ 1). Mọi nơi chuyển tọa độ phải đi qua `worldToScreen()` và `screenToWorld()`. Nhờ vậy **cái nhìn thấy trên màn hình đúng bằng cái xảy ra trong luật**: đường ngắm, điểm chạm tường, vòng hitbox đều là ảnh chiếu của cùng một hình học.

Ước lượng từ mockup (trên khung điện thoại, chỉ để làm mục tiêu):

| Đại lượng | Ước lượng |
|---|---|
| Sân cát (elip) | rộng ≈ 95% bề ngang màn hình, cao ≈ 54% chiều cao màn hình |
| Tâm sân | khoảng (50%, 52%) của màn hình |
| Tỉ lệ elip nhìn thấy trên màn hình | ≈ 1.1 (cao hơn rộng một chút) |
| Nhân vật | cao ≈ 18% bề ngang màn hình (≈ 70px ở 390 rộng) |
| Vòng dưới chân | rộng ≈ 12% bề ngang màn hình; dẹt ≈ 0.5 |

**Cấu hình đề xuất (giữ trong `shared-config`, đều cần playtest):**
- `ARENA_A = 1000`, `ARENA_B = 1750` (tỉ lệ logic 1.75).
- `PLAYER_RADIUS = 120`, `PROJECTILE_RADIUS = 40`.
- `VIEW_TILT_Y`: **không đặt cứng** mà **suy ra từ calibration** (xem 3.2), mục tiêu quanh 0.6 để elip nhìn thấy có tỉ lệ ≈ 1.05–1.1.

Vòng hitbox hiển thị là hình chiếu của đường tròn bán kính `PLAYER_RADIUS` (nên dẹt đúng độ nghiêng), **không** vẽ bằng elip tùy ý.

---

## 3. Kiến trúc hiển thị theo lớp

### 3.1 Các lớp (sau ra trước)
1. **Nền (plate)**: 1 ảnh tĩnh phủ toàn màn hình (`bg_arena_plate.png`).
2. **Lớp trang trí động** (giai đoạn sau): cờ phấp phới, khán giả nhún, bụi.
3. **Vòng dưới chân + bóng nhân vật** (code).
4. **Nhân vật + vũ khí** (sprite), xếp theo `y` của chân (thấp hơn trên màn hình thì vẽ sau).
5. **Hiệu ứng ném/trúng** (code).
6. **Nhãn tên + tim trên đầu** (code).
7. **HUD** (CSS/SVG, nổi trên cùng).

### 3.2 Calibration: khớp sân logic với sân trong ảnh nền
Ảnh nền do AI tạo nên elip sân không bao giờ khớp chính xác với con số. Cần **công cụ calibration**:
- Bật bằng `?debug=1`: vẽ chồng elip logic lên ảnh nền, có tay cầm kéo để chỉnh **tâm (cx, cy), bán trục ngang `rx_px`, bán trục dọc `ry_px`** (theo tỉ lệ so với kích thước ảnh nền).
- Lưu vào `assets/arena_calibration.json` (tọa độ chuẩn hóa theo ảnh nền).
- Renderer suy ra: `k = rx_px / ARENA_A` (px mỗi đơn vị) và `VIEW_TILT_Y = ry_px / (ARENA_B · k)`.
- Đây là cách để **đổi ảnh nền lúc nào cũng được** mà không sửa luật.

### 3.3 Kích thước và co giãn ảnh nền
- Tạo ảnh nền ở tỉ lệ **9:21** (ví dụ 1290×3010), sân đặt quanh tâm (50%, 52%). Màn hình ngắn hơn thì **cắt đối xứng trên/dưới**; màn hình dài thì dùng nguyên. Scale theo **bề ngang** để sân luôn ≈ 95% bề ngang.
- Phải kiểm tra ở **9:16 (360×640)** đến **9:21 (412×915)**: sân luôn nằm trọn, dải HUD không cắt mất nội dung quan trọng.
- Xuất ảnh ở độ phân giải thật (@2x/@3x), không phóng to ảnh nhỏ.

---

## 4. HUD bám mockup (làm bằng code)

**Phong cách:** bảng gỗ nâu sẫm, viền vàng nâu, bo góc ~14px, bóng đổ nhẹ; chữ trắng/kem có viền tối. Giá trị tham chiếu (đặt vào `theme`):
- Nền bảng `#3A2418` (độ mờ ~90%), viền vàng nâu `#C8913F` 2–3px, viền trong tối `#1E100A`.
- Chữ chính kem `#FFE9B8`; chữ trong thế giới (tên trên đầu) trắng viền tối 3px.
- Nút "SẴN SÀNG": gradient `#FFE066 → #F7A81B`, viền nâu `#8A4B0F` 4px, chữ nâu đậm `#5A2E0A`, vệt sáng phía trên, icon hai kiếm bắt chéo.
- Tim đầy `#E8334A` có điểm sáng trắng; tim rỗng `#4A1F2A`.
- Vòng dưới chân: bạn = xanh `#2E8BFF`; đối thủ = màu riêng của từng người (6 màu), kèm số để phân biệt, không chỉ dựa vào màu.

**Bố cục (màn dọc):**
- **Trên trái:** thẻ lớn của bạn: chân dung vuông bo góc, chữ "Bạn", 3 tim.
- **Trên giữa:** bảng "VÒNG N" dạng tấm biển, bên dưới là đồng hồ cát kèm số giây; số giây to, đập nhịp, đỏ khi còn dưới 2 giây.
- **Trên phải:** các chip đối thủ nhỏ (chân dung tròn, tên, tim); tối đa 5 chip, tự thu nhỏ/xếp 2 hàng khi nhiều người; nút cài đặt ⚙ tròn.
- **Dưới:** nút chat 💬; (sảnh) "Nhân vật"; **nút lớn "SẴN SÀNG"**; (sảnh) "Trang bị", "Túi đồ". Trong trận: ẩn hoặc khóa các nút của sảnh, giữ chat + SẴN SÀNG.
- Nhãn trên đầu nhân vật: tên trên, 3 tim dưới (như mockup); chỉ chồng lên sân, không chồng lên nhân vật khác (tự đẩy nhãn khi sát nhau).
- Mọi HUD có **safe-area** (tai thỏ, thanh vuốt về nhà) và không đè lên vùng thao tác quan trọng của sân.

---

## 5. Quy trình tạo asset từ mockup

Mockup là ảnh tham chiếu phong cách. Các prompt sau viết bằng tiếng Anh cho công cụ tạo ảnh có hỗ trợ **ảnh tham chiếu** (đính kèm mockup). Chạy theo thứ tự; mỗi bước cần lặp để chọn kết quả tốt nhất. **Kiểm tra điều khoản thương mại của công cụ bạn dùng** trước khi dùng cho sản phẩm bán ra.

### 5.1 Ảnh nền (quan trọng nhất)
```
Use the attached image as the reference for art style, camera angle and layout.
Create the same Vietnamese street-stadium arena as a CLEAN BACKGROUND PLATE.
Portrait, aspect ratio 9:21, high-angle 3/4 view, hand-painted mobile game art, warm saturated colors, soft cel shading, clean outlines.
A large oval sand arena floor in the center covering about 95% of the image width, floor centered at about 52% of image height, evenly lit, completely EMPTY.
Surrounded by a low stone wall with arches, plain red and yellow bunting and banners (no national emblem), wooden crates, tires, traffic cones, an old van, market stalls with striped awnings, and tiers of cheering spectators, with a large foreground crowd along the bottom edge.
IMPORTANT: no characters on the sand, no UI, no HUD, no text, no phone frame, no watermark.
```
Lặp đến khi: sân **trống, sáng đều, cân đối**; tường không che sân; viền sân dễ nhìn thấy để calibration. Nếu ảnh ra tỉ lệ khác, dùng chức năng mở rộng ảnh (outpainting) để đạt 9:21.

### 5.2 Nhân vật (bảng tư thế)
```
Using the attached image's character style (chibi, big head, thick soft outlines, hand-painted shading), create a character sheet of ONE young Vietnamese street fighter, 3/4 front view, spiky dark hair, white tank top, blue shorts, flip-flops.
Hands EMPTY (a loose fist where an item would be held). Full body visible in every pose.
Poses, each separate with equal spacing on a flat solid light-gray background: idle, aim (arm raised), throw wind-up, throw release, hit (dizzy), knocked out, victory.
Keep the exact same face, proportions and costume in all poses. No weapons, no text, no shadows on the ground.
```
Làm 6 nhân vật (3 nam, 3 nữ) bằng cách đổi mô tả tóc/trang phục nhưng **giữ nguyên câu về phong cách**. Dùng nhân vật đầu tiên làm ảnh tham chiếu cho các nhân vật sau để giữ đồng nhất.

### 5.3 Vũ khí (mỗi cái một ảnh)
```
Single game item icon, same hand-painted chibi style as the reference, front view, centered, flat solid background, thick soft outline, no text:
[ Vietnamese honeycomb rubber sandal, blue | frying pan | mallet with blue-green head | beer mug | small old bicycle | cute golden dog toy | old scooter (no brand) ]
```
Mỗi vũ khí tách riêng; đặt điểm cầm ở cùng một chỗ để gắn vào tay nhân vật. **Không để chữ nhãn hiệu trên vật phẩm** (ví dụ tên hãng xe).

### 5.4 Xử lý hậu kỳ (bắt buộc)
1. Xóa nền thành PNG trong suốt; cắt sát (trim) và giữ lề nhỏ đều nhau.
2. Chuẩn hóa tỉ lệ để các tư thế của một nhân vật cao bằng nhau; đặt **điểm neo ở giữa hai bàn chân**.
3. Ghi lại **điểm cầm vũ khí (bàn tay)** của từng tư thế (tọa độ tương đối).
4. Xuất @2x/@3x so với kích thước hiển thị (nhân vật hiển thị ≈ 70px cao ở 390 rộng ⇒ ảnh gốc ≥ 210px cao).
5. Gộp vào atlas hoặc để rời, khai báo trong `assets/manifest.json`.
6. Ghi nguồn và giấy phép của từng asset vào `assets/LICENSES.md`.

### 5.5 Quy ước manifest (bổ sung cho 1C)
```
{
  "arena": {
    "plate": "bg_arena_plate.png",
    "calibration": "arena_calibration.json"
  },
  "characters": {
    "char01": {
      "frames": { "idle": [...], "aim": [...], "throw": [...], "hit": [...], "dead": [...], "win": [...] },
      "anchor": { "feet": [x, y] },
      "handGrip": { "idle": [x, y], "aim": [x, y], "throw": [x, y] },
      "portrait": "char01_portrait.png"
    }
  },
  "weapons": { "dep_to_ong": { "icon": "...", "sprite": "...", "grip": [x, y] } }
}
```

### 5.6 Nếu bạn muốn họa sĩ thật
Mockup là brief rất tốt: gửi kèm mockup, ba prompt trên và danh sách asset ở đây. Họa sĩ chuyên nghiệp sẽ làm tốt hơn AI ở độ nhất quán giữa các tư thế và ở layer tách rời (để sau này tùy biến tóc/trang phục).

---

## 6. PROMPT 1D — dán cho Codex

```
Trước khi làm, đọc docs/HitMe_Spec_for_Antigravity.md, docs/HitMe_UI_Prompts_for_Codex.md, docs/HitMe_UI_Redesign_Prompt_1C.md và docs/HitMe_Target_Mockup_Prompt_1D.md. Prompt 1D thay thế phần hiển thị của 1C. Xem ảnh mục tiêu tại docs/reference/target_mockup.png và coi đó là chuẩn để đối chiếu.

Nhiệm vụ: làm màn đấu theo đúng ảnh mục tiêu (bố cục, phong cách, HUD), với nền là ảnh, nhân vật là sprite theo manifest, HUD bằng code. KHÔNG đổi luật chơi. KHÔNG sao chép asset/UI của game khác.

Bước 1 — Hình học (làm trước, có unit test)
- Đảm bảo game-core dùng elip ARENA_A = 1000, ARENA_B = 1750, PLAYER_RADIUS = 120, PROJECTILE_RADIUS = 40 (như 1C bước 1, đã đổi bán kính). Mọi tính toán nằm trong không gian mặt sàn.
- Cài worldToScreen()/screenToWorld() với VIEW_TILT_Y; test: chuyển qua lại cho kết quả gốc; đường ngắm vẽ trên màn hình kết thúc đúng điểm chạm tường; vòng hitbox hiển thị là hình chiếu của đường tròn bán kính PLAYER_RADIUS.
- Thao tác kéo: người chơi kéo trên màn hình, đổi sang không gian sàn bằng screenToWorld() rồi mới tính hướng.

Bước 2 — Nền và calibration (mục 3 của file thiết kế)
- Nạp assets/bg_arena_plate.png phủ toàn màn hình, scale theo bề ngang, cắt đối xứng trên/dưới khi màn hình ngắn hơn, tâm sân quanh (50%, 52%).
- Cài chế độ ?debug=1 để chồng elip logic lên nền và kéo tay cầm chỉnh tâm/bán trục; lưu assets/arena_calibration.json; renderer suy ra k và VIEW_TILT_Y như mục 3.2.
- Chưa có ảnh nền thật: dùng nền placeholder xám có elip sân và nhãn "BG PLATE MISSING" (không cố vẽ cho đẹp).

Bước 3 — Nhân vật (sprite theo manifest)
- Vẽ nhân vật nhìn chếch 3/4, lật ngang theo dấu hướng ném (trên màn hình); vũ khí là sprite riêng gắn vào điểm cầm tay (handGrip), xoay theo góc ném khi ngắm, vung khi ném.
- Vòng dưới chân + bóng = hitbox hiển thị (hình chiếu đường tròn), xanh cho "Bạn", màu riêng + số cho đối thủ. Xếp thứ tự vẽ theo y của chân.
- Tên trên, 3 tim dưới ở trên đầu nhân vật; chữ trắng viền tối; tự đẩy nhãn khi các nhân vật sát nhau.
- Thiếu asset: placeholder (khối màu có viền + tên slot). Không dành công vẽ nhân vật đẹp bằng code.

Bước 4 — HUD bằng code theo mục 4 của file thiết kế: thẻ "Bạn" lớn, bảng "VÒNG N" + đồng hồ cát + số giây, chip đối thủ, nút ⚙; thanh dưới với chat và nút "SẴN SÀNG" (trong trận ẩn/khóa "Nhân vật/Trang bị/Túi đồ"). Tôn trọng safe-area, hỗ trợ 2–6 người, i18n vi/en.

Bước 5 — Hiệu ứng: giữ các hiệu ứng ở Prompt 1 mục 6 và 1C mục 4.5 (gợn sóng chạm, vạch ngắm nét đứt tới điểm chạm tường, bụi lúc xuất hiện, squash & stretch, vệt đuôi vũ khí, sao va chạm + "BỐP!", rung nhẹ có công tắc, tim vỡ, bay ra khi bị loại, pháo giấy) và hook SFX.

Độ sắc nét theo mục 5 của file 1C. Hiệu năng 60fps ở 390x844 với CPU throttling 4x (hoặc báo cáo số liệu).

Báo cáo: chụp ảnh ở 360x640, 390x844 và 412x915, DPR 2; đặt cạnh docs/reference/target_mockup.png và liệt kê từng điểm khác biệt còn lại. Ghi câu hỏi cần quyết định vào cuối báo cáo.
```

---

## 7. Tiêu chí nghiệm thu (so với mockup)
- [ ] Sân phủ ≈ 95% bề ngang; khán đài/đám đông lấp phần còn lại; không có vùng nền trống.
- [ ] Elip logic khớp elip sân trong ảnh nền sau calibration (kiểm tra bằng `?debug=1`).
- [ ] Đường ngắm, điểm chạm tường và vòng hitbox khớp tuyệt đối với kết quả va chạm (test `game-core` qua).
- [ ] Nhân vật chibi đọc rõ ở 360×640: tên + tim không chồng nhau, vòng dưới chân hiển thị đúng.
- [ ] HUD đúng bố cục và phong cách mục 4; hỗ trợ 2–6 người; không bị tai thỏ che.
- [ ] Ảnh chụp DPR 2 sắc nét; không có sprite hoặc nền bị vỡ hạt do phóng to.
- [ ] 60fps (CPU throttle 4x) hoặc có báo cáo số liệu.
- [ ] `assets/LICENSES.md` ghi nguồn và giấy phép của mọi asset.

## 8. Rủi ro và câu hỏi cần quyết định
1. **Quốc kỳ trong asset:** mockup có nhiều cờ đỏ sao vàng. Mình không chắc quy định hiện hành của Việt Nam về việc dùng Quốc kỳ trong sản phẩm giải trí/thương mại, nên bạn nên kiểm tra trước khi phát hành. Prompt nền ở mục 5.1 đã dùng **băng rôn đỏ vàng trơn** làm mặc định an toàn; nếu muốn dùng cờ thật, hãy để đó là **lớp asset thay thế được** (có thể bật/tắt) thay vì đóng cứng vào ảnh nền.
2. **Giấy phép ảnh AI:** kiểm tra điều khoản thương mại của công cụ bạn dùng để tạo mockup và asset.
3. **Nút "Nhân vật / Trang bị / Túi đồ" trong trận:** nên (a) ẩn trong trận, chỉ có ở sảnh; (b) cho xem chỉ-đọc; hay (c) cho đổi trang bị giữa các vòng? Vì vũ khí chỉ để làm đẹp nên (c) không ảnh hưởng công bằng, nhưng thêm độ phức tạp.
4. **Thời gian đặt vị trí:** mockup ghi 15 giây, spec là 5 giây. Chốt con số cuối sau khi playtest trên điện thoại thật.
5. **Cân bằng:** `ARENA_B/ARENA_A` (1.75), `PLAYER_RADIUS` (120) và `PROJECTILE_RADIUS` (40) là giá trị đề xuất, chưa phải giá trị đã kiểm chứng.
6. **Phạm vi nhân vật MVP:** 6 nhân vật dựng sẵn; tùy biến tóc/mặt/trang phục để sau (cần layer tách rời, chi phí art cao).
