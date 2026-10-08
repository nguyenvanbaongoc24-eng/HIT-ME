# HIT ME — Thiết kế lại giao diện v2 & Prompt 1C

> **Vị trí trong lộ trình:** Prompt 1C **thay thế Prompt 1B** (kết quả 1B chưa đạt). Giữ nguyên: luật chơi (spec), thao tác chạm/kéo, ẩn vị trí đối thủ ở pha đặt vị trí, trình tự REVEAL → THROW (Prompt 1, mục 4–7).
> Đọc kèm: `HitMe_Spec_for_Antigravity.md`. File này có **một thay đổi so với spec: đấu trường chuyển từ hình tròn sang hình elip dọc** (tài liệu thiết kế gốc cho phép "hình tròn hoặc elip"; đấu trường La Mã thật cũng là hình elip). Xem mục 3.

---

## 1. Chẩn đoán ảnh chụp hiện tại

**Lỗi bố cục**
1. **Khoảng đen trống rất lớn** phía trên và dưới. Đấu trường là hình tròn trong màn hình dọc nên chừa thừa chỗ; vùng sân cát thật chỉ chiếm khoảng 70% chiều rộng, phần còn lại bị tường, khán giả chiếm.
2. **Khán giả là các chấm màu rải quanh vòng**, nhìn như hạt confetti, lại bị cắt ở hai mép màn hình.
3. **Thẻ "Bạn" bị cắt/mờ** ở góc trên trái, trong khi thẻ bot hiển thị đầy đủ.

**Lỗi dễ đọc**
4. **Chữ chồng nhau:** tên "Bot 1/Bot 2" chồng lên nhân vật và chấm máu; "Không trúng ai" đè lên khán giả; "Ném!" màu nâu trên nền đen gần như không đọc được.
5. **Ba chấm ở góc dưới phải** không rõ là gì (máu của bạn?) và quá mờ.
6. **Nhân vật bạn bị mờ** giữa sân, không rõ là trạng thái "chưa đặt vị trí" hay lỗi.

**Lỗi chất lượng hình ảnh**
7. Nhân vật quá nhỏ và quá đơn giản (đầu tròn + thân chấm), không có sức hút; vũ khí chỉ là hình chữ nhật nâu.
8. Nét mảnh, phẳng, thiếu khối và thiếu độ sắc nét (nghi do vẽ canvas không nhân theo độ phân giải màn hình).

**Nguyên nhân gốc:** nhân vật và cảnh bị yêu cầu "vẽ bằng code". Cách này không thể đạt mức hình ảnh bạn muốn (ảnh tham chiếu nhân vật có nét viền đậm, đổ bóng tay vẽ, nhiều tư thế, đó là art của họa sĩ). Prompt 1C vì vậy đổi hướng: **Codex làm khung, bố cục, hiệu ứng, HUD; còn nhân vật và cảnh dùng asset thật** (xem mục 6). Phần code chỉ vẽ placeholder rõ ràng để thay sau.

---

## 2. Bố cục mới (màn dọc, tham chiếu 390×844)

Nguyên tắc: **cả màn hình là sân vận động**. Không còn nền đen trống. Đấu trường elip chiếm gần hết màn hình; phần thừa ở bốn góc và hai dải trên/dưới là **khán đài**, và HUD nổi trên khán đài.

```
┌──────────────────────────────┐ 0
│ khán đài (HUD nổi)           │
│ [VÒNG 7]   ( 5 )     [⚙]     │ dải trên ≈ 90–100px
│ (Bạn♥♥♥)(B1)(B2)(B3)         │
│     ╭────────────────╮       │
│   ╱  tường đá + đuốc    ╲    │
│  │     SÂN CÁT (elip)    │   │ elip ngoài ≈ 96% bề ngang,
│  │                       │   │ ≈ 78% chiều cao
│  │   (các nhân vật)      │   │
│   ╲                     ╱    │
│     ╰────────────────╯       │
│ [💬]   [ SẴN SÀNG ]   [🩴]   │ dải dưới ≈ 90–100px
└──────────────────────────────┘ 844
```

- Phần elip (tường + sân) ≥ **92% chiều rộng** và ≥ **70% chiều cao** màn hình trên mọi cỡ máy thử (360×800, 390×844, 412×915, 430×932).
- Dải HUD trên/dưới mỗi dải ≤ 12% chiều cao, tôn trọng safe-area (tai thỏ, thanh điều hướng). Nội dung HUD **nổi** lên khán đài, có nền bán trong suốt, không đặt trong khung đen.
- Khán đài lấp đầy mọi vùng ngoài elip, chạy sát mép màn hình (không bị cắt lộ nền đen).
- **Khung logic cố định, hiển thị co giãn:** hình học đấu trường trong `game-core` là cố định cho mọi thiết bị (cần cho server và công bằng). Mỗi màn hình chỉ chọn hệ số tỉ lệ `k` để vừa khung; phần thừa (màn dài hơn) được lấp bằng khán đài.

---

## 3. Bản vá spec: đấu trường elip

| Hằng số | Giá trị đề xuất | Ghi chú |
|---|---|---|
| `ARENA_SHAPE` | `"ellipse"` | giữ `"circle"` để chạy test cũ |
| `ARENA_A` | 1000 | bán trục ngang (đơn vị logic) |
| `ARENA_B` | 1750 | bán trục dọc (tỉ lệ ≈ 1.75; **cần playtest**) |
| `PLAYER_RADIUS` | 90 | hitbox = vòng bóng dưới chân; ≈ 15px ở 390 rộng. **Cần playtest** |
| `PROJECTILE_RADIUS` | 30 | ≈ 5px |

Quy đổi hiển thị tham chiếu ở 390px rộng: sân (không tính tường) rộng ≈ 84% màn hình, nên `k ≈ 0.168 px/đơn vị`; nhân vật cao ≈ 70–75px, bóng dưới chân bán kính ≈ 15px. Mọi con số này nằm trong `shared-config`; **không đổi luật nào khác**.

**Thay đổi cần làm trong `game-core` (có unit test):**
- Khoảng cách từ người ném đến tường theo hướng `d` (đơn vị), với `p` là vị trí:
  ```
  A = dx²/a² + dy²/b²
  B = 2·(px·dx/a² + py·dy/b²)
  C = px²/a² + py²/b² − 1        (âm khi p ở trong elip)
  t = (−B + sqrt(B² − 4·A·C)) / (2·A)      // nghiệm dương
  ```
- Kẹp vị trí trong sân: nếu `(x/(a−r))² + (y/(b−r))² > 1` thì co `(x, y)` về tâm theo hệ số `1/√(giá trị đó)` với `r = PLAYER_RADIUS` (sai số nhỏ chấp nhận được).
- Sudden death (nếu bật): nhân cả `a` và `b` với `(1 − SHRINK_PER_ROUND)` mỗi vòng.
- Phần "khoảng cách từ tâm người bị ném đến đường bay" và `MAX_PIERCE_TARGETS` **giữ nguyên**.
- Test: ném dọc trục ngang, trục dọc, chéo; đứng sát tường; ném về phía sau; hai người thẳng hàng.

---

## 4. Thiết kế hình ảnh v2

### 4.1 Hướng nghệ thuật tổng thể
- Giữ cặp màu ấm cam/xanh ngọc nhưng **tăng độ tương phản và chất liệu**: đá có vân, cát có hạt, kim loại/vàng có điểm sáng, ánh sáng có hướng (từ trên-trái), bóng đổ rõ.
- Khối rõ ràng hơn phong cách phẳng cũ: mỗi vật có 3 mảng (sáng, trung, tối) và viền tối cùng tông; nhân vật có **viền đậm 2–3px** theo ảnh tham chiếu.
- Mọi chữ trong thế giới game có **viền tối 2–3px** để đọc được trên mọi nền.

### 4.2 Các lớp cảnh (từ sau ra trước)
1. **Khán đài:** các hàng bậc thang theo vòng elip đồng tâm, thay hình chấm bằng sprite khán giả có thân và đầu (nhiều áo, nhiều tư thế: ngồi, giơ tay, vẫy cờ), hàng sau mờ hơn hàng trước (độ sâu). Khán giả phản ứng theo sự kiện (giật mình khi có trúng đòn, reo hò khi có người bị loại).
2. **Tường đấu trường:** vòng tường đá **dày**, thấy mặt trên (sáng) và mặt trong (tối) cùng vòm cung/cột; viền vàng; bóng đổ mềm xuống sân.
3. **Sân cát:** nền cát có hạt, vết giày, vài vết cào; hoa văn kiểu La Mã (viền elip đồng tâm mờ, biểu tượng "HIT ME" ở giữa sân, mờ, không che nhân vật); sáng ở giữa, tối ở rìa (vignette).
4. **Đồ trang trí gắn tường:** 4–6 ngọn đuốc có lửa nhấp nháy và quầng sáng; băng rôn xanh ngọc có thêu; không đặt cây "tròn" vô nghĩa như bản cũ.
5. **Nhân vật** (xếp theo y của chân: ai thấp hơn trên màn hình thì vẽ sau để đè lên).
6. **Vũ khí và hiệu ứng** (xem Prompt 1 mục 6 + mục 4.5).
7. **HUD nổi.**

### 4.3 Nhân vật (theo ảnh tham chiếu)
Ảnh tham chiếu là bộ sprite 2D phong cách hoạt hình viền đậm: đầu to vừa phải (≈ 1/3 chiều cao), tô bóng 2–3 tông, nhiều tư thế (đứng, ngắm, chạy, cầm vũ khí), có bóng elip dưới chân.

- **Chỉ lấy phong cách làm mốc, không dùng chính bộ sprite đó nếu chưa có giấy phép.** Đó là sản phẩm có bản quyền của người khác (xem mục 6).
- **Đổi chủ đề cho hợp Hit Me:** vui nhộn, đậm chất Việt Nam: áo ba lỗ, quần đùi, dép tổ ong, áo vest, váy cưới (theo tài liệu thiết kế), thay cho trang bị quân sự.
- **Góc nhìn:** ảnh tham chiếu là nhìn nghiêng. Trong đấu trường nhìn từ trên xuống, dùng **sprite nhìn nghiêng + lật ngang** theo hướng ném: `facing = dấu của cos(góc ném)`. Hướng lên/xuống thể hiện bằng **xoay lớp tay + vũ khí** theo góc ném. Khi lật ngang, góc xoay của tay phải được phản chiếu tương ứng.
- **Hitbox = vòng bóng dưới chân** (bán kính `PLAYER_RADIUS`); sprite có thể cao hơn nhiều. Hiển thị vòng bóng đủ rõ.
- **Bộ tư thế tối thiểu (mỗi nhân vật):** idle (2–4 khung nhún/chớp mắt), aim (tay giơ), throw (3 khung: lấy đà, ném, thu tay), hit (1–2 khung choáng), dead (1 khung, code làm xoay + bay), win (2 khung).
- **Phạm vi MVP:** 6 nhân vật dựng sẵn (3 nam, 3 nữ) kèm biến thể màu; hệ thống tùy biến tóc/mặt/trang phục (layer) **để giai đoạn sau** vì chi phí art rất cao.
- **Cách tiết kiệm art:** dùng các tư thế tĩnh, để code làm animation phụ (nhún, nghiêng, squash & stretch, xoay tay) thay vì vẽ từng khung.
- **Layer khuyến nghị (để xoay tay):** `shadow`, `leg`, `body`, `head`, `hair_back/front`, `arm_back`, `arm_front` (có điểm xoay ở vai và điểm cầm vũ khí).

### 4.4 HUD thiết kế lại
- **Dải trên (nổi trên khán đài):** trái là huy hiệu "VÒNG 7"; giữa là **đồng hồ đếm ngược số lớn** trong vòng tròn chạy hết 5 giây, đập nhịp mỗi giây và đổi đỏ khi còn dưới 2 giây; phải là nút cài đặt ⚙.
- **Hàng "chip" người chơi (tối đa 6):** mỗi chip là chân dung tròn (cắt mặt nhân vật) có viền màu riêng + huy hiệu số, dưới chân dung là **3 trái tim nhỏ**. Chip của bạn to hơn và nằm đầu hàng. Người bị loại: chân dung xám, dấu X. **Không in tên cố định trên chip** (tên hiện khi chạm); tránh rối.
- **Tên trên đầu nhân vật:** nhãn nhỏ dạng viên thuốc, chữ có viền, chỉ hiện cho đối thủ; không chồng lên máu. Máu thể hiện bằng các trái tim nhỏ ngay trong nhãn.
- **Dải dưới:** trái là nút chat 💬; giữa là nút lớn **"SẴN SÀNG"** (khóa hành động sớm; mờ đi sau khi đã khóa); phải là ô vũ khí đang trang bị (icon, nhấn mở cửa hàng ở màn hình menu, không phải trong trận). Bỏ ba chấm cũ.
- **Thông báo sự kiện** ("Không trúng ai", "Bot 2 bị loại", "-1") hiện ở phần ba trên của sân, trong băng giấy/dải màu có viền, **không đè lên khán giả**, tự biến mất sau ~1.2s.
- **Gợi ý thao tác** ("Chạm để đặt vị trí, kéo ra để chọn hướng ném"): hiện trong 2 vòng đầu của người mới, sau đó ẩn.
- **Nhân vật của bạn:** luôn đậm, không mờ. Nếu chưa đặt vị trí thì hiển thị **vòng hướng dẫn nhấp nháy** ở giữa sân thay vì nhân vật mờ.
- **Phông chữ:** tròn, đậm, đúng dấu tiếng Việt (Baloo 2 hoặc Nunito; kiểm tra đầy đủ dấu), chữ trắng viền tối.

### 4.5 Hiệu ứng
Giữ các hiệu ứng ở Prompt 1B (gợn sóng chạm, bụi lúc xuất hiện, squash & stretch, vệt đuôi vũ khí, sao va chạm + chữ "BỐP!", rung nhẹ, tim vỡ, bay ra khi bị loại, pháo giấy). Bổ sung:
- Vệt đường ngắm: nét đứt chạy từ tay nhân vật tới điểm chạm tường (điểm cuối đánh dấu trên tường elip), không bị ngón tay che hoàn toàn.
- Bóng đổ động dưới chân khi nhún/nhảy.
- Khói/bụi mờ khi nhân vật bị loại.

---

## 5. Độ sắc nét (nguyên nhân "nhìn mờ" và cách sửa)

1. Canvas phải có kích thước thật = `kích thước CSS × devicePixelRatio` (giới hạn tối đa 3), rồi scale context; **không** để canvas mặc định rồi phóng to bằng CSS.
2. Các lớp tĩnh cache vào offscreen canvas/texture **ở độ phân giải thật (đã nhân DPR)**, không cache ở độ phân giải thấp.
3. Sprite chuẩn bị ở **@2x hoặc @3x** so với kích thước hiển thị; không phóng to ảnh nhỏ. Làm tròn tọa độ của sprite tĩnh/chữ để tránh nhòe do nửa điểm ảnh.
4. Chữ dùng font web thật (DOM/CSS hoặc font chữ canvas), không dùng ảnh chữ phóng to. Biểu tượng UI dùng SVG.
5. Tắt `image-rendering: pixelated` (đây không phải game pixel art); bật lọc tuyến tính, tạo mipmap nếu dùng WebGL.
6. Kiểm tra bằng ảnh chụp màn hình ở DPR 2 và 3.

---

## 6. Asset thật: cách có được (bạn cần chọn)

Mình **không thể tạo file sprite chất lượng như ảnh tham chiếu trực tiếp trong cuộc trò chuyện này**; code của Codex cũng không vẽ được mức đó. Cần một trong các hướng sau:

| Hướng | Ưu | Nhược |
|---|---|---|
| **A. Mua/cấp phép bộ sprite có sẵn** (chợ asset như itch.io, GameDev Market, CraftPix, OpenGameArt, Kenney) | Nhanh, rẻ, đúng chất lượng "viền đậm hoạt hình" | Phong cách chung chung; **phải đọc kỹ giấy phép** (thương mại, chỉnh sửa, dùng trong bản web/mobile); khó có chủ đề Việt Nam |
| **B. Thuê họa sĩ 2D** (concept trước, rồi sprite hoặc rig Spine/DragonBones) | Đúng chủ đề Việt, độc quyền, đồng nhất | Tốn chi phí và thời gian; cần brief rõ |
| **C. Tạo bằng AI rồi chỉnh sửa** | Nhanh để thử concept | Khó đồng nhất giữa các tư thế, cần cắt nền và chỉnh tay; **phải kiểm tra điều khoản thương mại** của công cụ |

**Khuyến nghị:** dùng **A cho bản prototype/MVP** để thấy được giao diện đúng chất lượng ngay, rồi dùng kết quả làm brief cho **B** ở bản chính thức. Nếu muốn dùng chính bộ sprite trong ảnh bạn gửi, hãy tìm nguồn của nó, kiểm tra giấy phép và mua đúng gói; nhưng lưu ý bộ đó là trang bị quân sự nhìn nghiêng, cần đổi trang phục/vũ khí cho hợp Hit Me.

### 6.1 Danh sách asset tối thiểu cho MVP
- **Nhân vật x6** (tư thế như mục 4.3), kèm khuôn mặt để cắt làm chân dung chip.
- **Vũ khí x5** (xe đạp, xe máy cũ không ghi nhãn hiệu, dép tổ ong, cốc bia, chó vàng dễ thương): icon 128px và sprite bay.
- **Môi trường:** ô sàn cát lặp, đoạn tường vòm cung lặp, cột, đuốc (4 khung), băng rôn, bậc khán đài, khán giả x6 mẫu (2–3 tư thế).
- **UI:** chip người chơi, tim đầy/rỗng/vỡ, nút tròn, nút "SẴN SÀNG", băng thông báo.
- **FX:** bụi, sao va chạm, chữ "BỐP!", pháo giấy.
- **Âm thanh:** tick, vút ném, bốp trúng, loại, thắng, khán giả reo/giật mình, nhạc nền.

### 6.2 Quy ước khai báo (Codex đọc `assets/manifest.json`)
```
{
  "characters": {
    "char01": {
      "frames": { "idle": ["char01_idle_0.png","char01_idle_1.png"], "aim": [...], "throw": [...], "hit": [...], "dead": [...], "win": [...] },
      "anchor": { "feet": [x, y] },
      "pivots": { "shoulder": [x, y], "hand": [x, y] },
      "faceFacing": "right"
    }
  },
  "weapons": { "dep_to_ong": { "icon": "...", "sprite": "...", "trail": "..." } }
}
```
Đặt ảnh vào `assets/`. Thiếu file nào thì code dùng placeholder (khối màu có viền, ghi rõ tên slot), **không cố vẽ placeholder cho đẹp**.

---

## 7. PROMPT 1C — dán cho Codex

```
Trước khi làm, đọc docs/HitMe_Spec_for_Antigravity.md, docs/HitMe_UI_Prompts_for_Codex.md và docs/HitMe_UI_Redesign_Prompt_1C.md. Prompt 1C thay thế Prompt 1B.

Nhiệm vụ: thiết kế lại màn đấu theo bố cục mới: đấu trường elip phủ gần toàn màn hình, khán đài lấp mọi vùng thừa, HUD nổi, nhân vật dùng sprite theo manifest. Không đổi luật chơi ngoài việc chuyển đấu trường sang elip như mục 3 của file thiết kế. KHÔNG sao chép asset/UI của bất kỳ game nào.

Bước 1 — game-core (làm trước, có unit test)
- Thêm ARENA_SHAPE ("ellipse" mặc định, "circle" để test cũ), ARENA_A = 1000, ARENA_B = 1750, PLAYER_RADIUS = 90, PROJECTILE_RADIUS = 30 trong shared-config.
- Cài hàm khoảng cách tới tường (giải phương trình bậc hai ray–elip theo mục 3), kẹp vị trí vào sân, co sân (nếu có sudden death).
- Cập nhật resolveRound và các chỗ dùng bán kính đấu trường cũ. Giữ nguyên logic trúng đòn và MAX_PIERCE_TARGETS.
- Viết test: ném dọc trục ngang/dọc/chéo, đứng sát tường, hai người thẳng hàng, ném về phía sau.

Bước 2 — Khung hiển thị toàn màn hình
- Tạo hàm tính tỉ lệ k vừa khung từ kích thước màn hình, chiều cao dải HUD trên/dưới và độ dày tường; đưa mọi chuyển đổi tọa độ qua worldToScreen()/screenToWorld() (VIEW_TILT_Y = 1.0).
- Yêu cầu: elip ngoài ≥ 92% bề ngang và ≥ 70% chiều cao ở 360x800, 390x844, 412x915, 430x932; không có nền đen trống (khán đài lấp mọi vùng ngoài elip).
- Canvas theo DPR (tối đa 3); các lớp tĩnh cache ở độ phân giải thật. Làm theo mục 5 của file thiết kế.

Bước 3 — Cảnh (mục 4.2)
- Khán đài bậc thang elip với khán giả có thân/đầu (đọc từ manifest; thiếu thì placeholder rõ ràng), tường dày thấy mặt trên và mặt trong, sân cát có hạt và hoa văn, đuốc có lửa và quầng sáng, băng rôn.

Bước 4 — Nhân vật (mục 4.3)
- Đọc manifest, vẽ sprite nhìn nghiêng, lật ngang theo hướng ném, xoay lớp tay/vũ khí theo góc ném, sắp xếp theo y của chân. Bóng dưới chân = hitbox hiển thị (khớp PLAYER_RADIUS).
- Thiếu asset: dùng placeholder (khối màu có viền, ghi tên slot). Không dành công sức cho việc vẽ nhân vật đẹp bằng code.

Bước 5 — HUD (mục 4.4)
- Dải trên: huy hiệu vòng, đồng hồ số lớn, cài đặt. Hàng chip người chơi với 3 tim. Dải dưới: chat, nút "SẴN SÀNG", ô vũ khí.
- Nhãn tên/máu trên đầu đối thủ, chữ có viền tối, không chồng nhau. Thông báo sự kiện không đè lên khán giả.
- Nhân vật của bạn luôn đậm; chưa đặt vị trí thì hiện vòng hướng dẫn nhấp nháy.

Bước 6 — Hiệu ứng (mục 4.5 và Prompt 1B mục 4)
- Giữ/hoàn thiện: gợn sóng chạm, vạch ngắm nét đứt chạy tới điểm chạm tường, bụi lúc xuất hiện, squash & stretch, vệt đuôi vũ khí, sao va chạm + "BỐP!", rung nhẹ (có công tắc), tim vỡ, bay ra khi bị loại, khán giả phản ứng, pháo giấy. Có hook SFX (beep tạm).

Hiệu năng: 60fps ở 390x844 với CPU throttling 4x (hoặc báo cáo số liệu thật và đề xuất giảm gì). QUALITY = high | medium | low giảm khán giả/hạt/bóng mềm.

Sửa các lỗi đã thấy: thẻ "Bạn" bị cắt, chữ chồng nhau, chữ "Ném!" quá tối, ba chấm góc dưới phải, khán giả bị cắt mép, nhân vật bạn bị mờ.

Tiêu chí nghiệm thu: xem mục 8 của file thiết kế. Báo cáo theo mẫu ở HitMe_UI_Prompts_for_Codex.md mục 5, kèm ảnh chụp ở 390x844 và 360x800, DPR 2.
```

---

## 8. Tiêu chí nghiệm thu (đối chiếu từng mục)
- [ ] Không còn vùng đen trống; khán đài lấp mọi vùng ngoài elip.
- [ ] Elip ngoài ≥ 92% bề ngang, ≥ 70% chiều cao ở 4 cỡ máy thử; HUD nổi trong dải ≤ 12% chiều cao mỗi bên.
- [ ] Chạy đúng luật trên elip (test `game-core` đều qua), điểm chạm tường khớp với đường ngắm hiển thị.
- [ ] Nhân vật đọc rõ ở 360×800: thân cao ≈ 70px, bóng dưới chân khớp hitbox, lật hướng và xoay tay đúng.
- [ ] Không có chữ chồng nhau; mọi chữ có viền và đọc được trên mọi nền.
- [ ] Chip người chơi + tim, đồng hồ số lớn, nút "SẴN SÀNG" hoạt động; hàng chip không bị cắt.
- [ ] Ảnh chụp ở DPR 2/3 sắc nét; không có sprite phóng to bị vỡ.
- [ ] 60fps (CPU throttle 4x) hoặc có báo cáo số liệu.
- [ ] Không thành phần nào sao chép asset/UI của game khác; asset dùng đều có giấy phép rõ ràng (ghi vào `assets/LICENSES.md`).

## 9. Điều bạn cần quyết định
1. **Nguồn asset** (mục 6): A, B hay C cho nhân vật và cảnh?
2. **Tỉ lệ elip** `ARENA_B/ARENA_A` (mặc định 1.75) và **kích thước hitbox** (`PLAYER_RADIUS`): đây là thay đổi cân bằng, cần chơi thử trên điện thoại thật.
3. Có chấp nhận **MVP chỉ có 6 nhân vật dựng sẵn** thay vì tùy biến tóc/mặt/trang phục ngay không?
