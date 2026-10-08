# HIT ME — Đặc tả & kế hoạch triển khai (dành cho Antigravity)

> Tài liệu này là nguồn sự thật duy nhất (single source of truth) cho dự án. Agent phải đọc toàn bộ trước khi viết code.
> Quy ước: **[ĐÃ CHỐT]** = chủ dự án đã quyết định. **[ĐỀ XUẤT]** = giả định mặc định để làm tiếp, chủ dự án chưa xác nhận, phải để trong file config để đổi được dễ dàng. **[CẦN XÁC NHẬN]** = còn mơ hồ.

---

## 1. Tổng quan

- **Tên game:** Hit Me
- **Thể loại:** game đấu trường online, lượt đồng thời (simultaneous turn-based), 2–6 người, có chế độ đấu bot.
- **Nền tảng:** chủ yếu mobile (iOS/Android), màn dọc.
- **Thị trường:** Việt Nam. Ngôn ngữ: Tiếng Việt (mặc định) và English.
- **Phong cách art:** 2D hoặc low-poly. **[ĐỀ XUẤT] MVP làm 2D top-down**, low-poly 3D để sau.
- **Điểm hấp dẫn:** đoán vị trí đối thủ, né và ném trúng; vũ khí hài hước đậm chất Việt (xe đạp, xe máy cũ, dép tổ ong, cốc bia, chó vàng...).

## 2. Luật chơi

### 2.1 Đấu trường
- Hình tròn (có thể elip sau), chiếm 70–80% màn hình mobile. Bối cảnh: đấu trường La Mã, Hy Lạp, sân vận động.
- Tọa độ chuẩn hóa: tâm (0,0), bán kính `ARENA_RADIUS = 1000` đơn vị logic. Client scale theo màn hình.

### 2.2 Một vòng (round)
Mỗi trận gồm nhiều vòng, mỗi vòng có các pha sau:

1. **PLACEMENT (≈ 5 giây) [ĐÃ CHỐT]:** mỗi người chơi (còn sống) đồng thời:
   - chọn **vị trí** đặt nhân vật trong đấu trường;
   - **kéo thả để chọn hướng ném** [ĐÃ CHỐT].
   - Người chơi **không thấy** vị trí và hướng của người khác.
2. **REVEAL:** hết giờ, server khóa hành động, tính kết quả, rồi gửi về cho mọi người. Tất cả nhân vật **hiện ra cùng lúc**.
3. **THROW [ĐÃ CHỐT]:** **tất cả ném đồng thời**, vũ khí bay theo **đường thẳng** theo hướng đã chọn đến **bờ tường** đấu trường.
4. **RESOLVE:** tính trúng, trừ máu, loại người hết máu, rồi sang vòng mới nếu còn ≥ 2 người.

### 2.3 Va chạm (hitbox) [ĐÃ CHỐT: hitbox cố định, đường bay thẳng đến tường]
- Hitbox người chơi: hình tròn bán kính `PLAYER_RADIUS`.
- Hitbox vũ khí: hình tròn bán kính `PROJECTILE_RADIUS`. **Mọi vũ khí có hitbox giống hệt nhau**; chỉ hình ảnh và hiệu ứng khác nhau (xem mục 6).
- Vũ khí trúng khi khoảng cách từ tâm người bị ném tới đường bay ≤ `PLAYER_RADIUS + PROJECTILE_RADIUS`.
- **Xuyên người [ĐÃ CHỐT, diễn giải cần xác nhận]:** vũ khí **không xuyên qua nhiều người**; mỗi vũ khí chỉ trúng **tối đa 1 người** (người đầu tiên nằm trên đường bay, tính theo khoảng cách từ người ném).
  - Cài đặt bằng tham số `MAX_PIERCE_TARGETS = 1` để sau này đổi thành 2 chỉ cần sửa một dòng.
  - **[CẦN XÁC NHẬN]:** câu gốc là "chỉ xuyên qua một người". Nếu ý là "xuyên qua người thứ nhất rồi trúng người thứ hai", đặt `MAX_PIERCE_TARGETS = 2`.
- Người ném không tự trúng vũ khí của mình. Vũ khí không va chạm với nhau.
- Mọi hit được tính từ **vị trí ban đầu của vòng** (đồng thời), không có thứ tự theo thời gian.

### 2.4 Máu và loại
- `MAX_HP = 3`, mỗi lần trúng trừ 1 [ĐÃ CHỐT: "tầm 3 lần trúng sẽ die"].
- Một người có thể trúng nhiều vũ khí trong cùng vòng (mỗi vũ khí trừ 1).
- Hết máu thì bị loại ngay sau vòng đó.

### 2.5 Kết thúc trận [ĐÃ CHỐT]
- Trận kết thúc khi **chỉ còn 1 người cuối cùng**.
- **[ĐỀ XUẤT] Cùng bị loại trong một vòng:** những người bị loại cùng vòng chia sẻ thứ hạng tốt nhất của nhóm đó. Nếu tất cả người còn lại cùng chết ở vòng cuối thì họ đồng hạng nhất, không ai "thắng một mình".
- **[ĐỀ XUẤT] Chống kéo dài trận (sudden death):** sau `SUDDEN_DEATH_ROUND = 12`, mỗi vòng bán kính đấu trường thu nhỏ `SHRINK_PER_ROUND = 8%` để buộc người chơi áp sát nhau. Tránh trận hòa vô tận vì tỉ lệ trúng thấp.
- **[ĐỀ XUẤT] Đặt chồng vị trí:** khoảng cách tối thiểu giữa hai người là `2 * PLAYER_RADIUS`. Nếu hai người chọn chồng nhau thì server đẩy nhẹ ra xa (nudge) theo quy tắc xác định, không từ chối.

### 2.5.1 Người chơi không gửi hành động trong 5 giây
- **[ĐỀ XUẤT]** Vòng đó nhân vật **đứng yên ở vị trí cũ** (vòng 1 thì vị trí mặc định ngẫu nhiên do server chọn), **không ném**, vẫn bị trúng bình thường. Mỗi vòng bỏ lỡ đếm là 1 lần "missed".

### 2.6 AFK và rớt mạng [ĐÃ CHỐT, ngưỡng AFK là đề xuất]
- **AFK:** bị đưa ra khỏi trận và **cấm thi đấu 2 phút** (`AFK_BAN_MS = 120000`).
  - **[ĐỀ XUẤT]** coi là AFK khi bỏ lỡ `AFK_MISSED_ROUNDS = 2` vòng **liên tiếp** (bỏ lỡ 1 vòng do lag/vô ý thì chưa bị phạt).
  - Cấm lưu ở server (`bans.banned_until` theo `user_id`), không phụ thuộc thiết bị. Trong thời gian cấm không được vào hàng chờ.
- **Rớt mạng [ĐÃ CHỐT]: không bị cấm.**
  - **[ĐỀ XUẤT]** cho `RECONNECT_GRACE_MS = 30000` để vào lại phòng cũ. Trong lúc mất kết nối, nhân vật xử lý như bỏ lỡ vòng nhưng **không tính vào AFK**. Quá thời gian chờ thì tính là rời trận (thua, không cấm).
  - Phải phân biệt "rớt kết nối" (socket đóng bất thường) với "AFK" (còn kết nối nhưng không gửi hành động).

## 3. Rank, vàng và kinh tế

### 3.1 Rank [ĐÃ CHỐT: có rank]
- Có hệ thống rank dựa trên thứ hạng cuối trận. Chi tiết điểm là **[ĐỀ XUẤT]**, đặt trong config:

| Số người trong trận | Hạng 1 | Hạng 2 | Hạng 3 | Hạng 4 | Hạng 5 | Hạng 6 |
|---|---|---|---|---|---|---|
| 2 | +25 | -20 | | | | |
| 4 | +30 | +10 | -10 | -25 | | |
| 6 | +35 | +20 | +5 | -10 | -20 | -30 |

- **[ĐỀ XUẤT]** Đặt tên bậc rank **khác từ "Vàng"** vì tiền tệ trong game tên là "vàng", dễ nhầm. Ví dụ: Sắt, Đồng, Bạc, Bạch Kim, Kim Cương, Huyền Thoại.
- **[ĐỀ XUẤT]** Ghép trận theo khoảng rank gần nhau; chờ quá `MATCHMAKING_TIMEOUT_S = 15` thì lấp chỗ trống bằng bot.
- **[ĐỀ XUẤT]** Trận có bot: **không đổi điểm rank**, và nhận vàng giảm (`BOT_GOLD_FACTOR = 0.5`) để chống cày bot.

### 3.2 Vàng [ĐÃ CHỐT: kiếm qua thắng top 3, thưởng 199]
- Tiền ảo tên "vàng", kiếm khi **vào top 3 của trận**. Phần thưởng cơ bản: **199 vàng** (`GOLD_REWARD_TOP3 = 199`).
- **[CẦN XÁC NHẬN]** 199 vàng cho mỗi người vào top 3 (cùng mức), hay chia theo hạng (1 nhiều hơn 3)? Mặc định: chia theo bảng cấu hình, hạng 1 = 199, hạng 2 và 3 có thể thấp hơn.
- **[ĐỀ XUẤT]** Trận 2–3 người thì "top 3" gần như ai cũng được thưởng → dễ cày. Quy tắc mặc định: trận < 4 người chỉ hạng 1 được vàng. Đặt trong config.

### 3.3 Giá trang bị [ĐÃ CHỐT một phần]
- Mỗi trang bị mới có giá **gấp 1.x lần** trang bị trước (giá tăng theo bậc).
- **[CẦN XÁC NHẬN]:** đã ghi "gấp 1x lần" — hiểu là hệ số **1.x** (ví dụ 1.5). Mặc định `PRICE_MULTIPLIER = 1.5` và `BASE_PRICE = 199` (giá món đầu bằng đúng một lần thưởng). Công thức: `price(tier) = round(BASE_PRICE * PRICE_MULTIPLIER^(tier-1))`.
- Ví dụ mặc định (hệ số 1.5): tier 1 = 199, tier 5 ≈ 1.007, tier 10 ≈ 7.650 vàng. Tức là món tier 10 cần khoảng 38 lần vào top 3. Nếu hệ số = 2 thì tier 10 là ≈ 101.888 vàng, gần như bắt buộc nạp tiền. **Cần playtest để cân bằng, đừng chốt số cứng trong code.**

### 3.4 Đổi tên [ĐÃ CHỐT từ tài liệu ban đầu]
- Lần đầu miễn phí; từ lần thứ hai tốn vàng (`NAME_CHANGE_COST` **[CẦN XÁC NHẬN số tiền]**, mặc định 99).

## 4. Nhân vật và vũ khí

### 4.1 Nhân vật [ĐÃ CHỐT]
- Chọn nam/nữ, kiểu tóc, khuôn mặt, trang phục lần đầu: quần đùi, áo ba lỗ, áo vest, váy cưới, v.v.
- **Hệ thống tùy biến phải dạng layer** (mỗi bộ phận là một layer sprite riêng: thân, mặt, tóc, áo, quần/váy) để ghép được mọi tổ hợp mà không phải vẽ từng tổ hợp.

### 4.2 Vũ khí làm đẹp [ĐÃ CHỐT]
- Vũ khí chủ đề Việt Nam: xe đạp, xe máy cũ, dép tổ ong, cốc bia, chó vàng...
- **Chỉ khác về hình ảnh và hiệu ứng, không khác hitbox, tầm, sát thương** → công bằng, không pay-to-win.
- Giá càng cao, hiệu ứng càng đẹp [ĐÃ CHỐT]. Mỗi vũ khí có: sprite, `vfx_throw` (hiệu ứng lúc bay: vệt, hạt), `vfx_hit` (hiệu ứng lúc trúng), `sfx_hit`.
- **Lưu ý pháp lý:** tránh nhãn hiệu thật (ví dụ không dùng chữ "Honda", đặt tên "Xe máy cũ"). Tránh các hình ảnh dễ bị store đánh giá là bạo lực với động vật (chó vàng nên là hoạt hình dễ thương, không có biểu hiện bị đau).

## 5. Tài khoản và xã hội

- **Đăng nhập [ĐÃ CHỐT]:** Gmail (Google), Facebook, Apple ID, hoặc chơi nhanh với tên người chơi "Quest" (khách).
  - Khách phải **liên kết được** với tài khoản thật sau này để giữ tiến trình.
  - Apple yêu cầu có Sign in with Apple khi có đăng nhập social khác.
- **Chat trong trận [ĐÃ CHỐT]:** icon chat, tin nhắn preset hoặc nhập tự do. **[ĐỀ XUẤT]** lọc từ tục tiếng Việt, có nút báo cáo và chặn (store bắt buộc khi có chat giữa người lạ).
- **Kết bạn [ĐÃ CHỐT]:** có thể gửi lời mời kết bạn sau khi trận kết thúc.
- **Cài đặt [ĐÃ CHỐT]:** âm thanh riêng (nhạc nền, SFX), SFX khi ném trúng người, ngôn ngữ Việt/Anh.

## 6. Kiến trúc kỹ thuật [ĐỀ XUẤT]

### 6.1 Stack
- **Client:** TypeScript + Vite + Phaser 3 (2D). i18n bằng file JSON (`vi.json`, `en.json`).
- **Mobile:** Capacitor (đóng gói web thành app iOS/Android).
- **Server:** Node.js + TypeScript + Colyseus (room, state machine, reconnection).
- **Backend dữ liệu & auth:** Supabase (Postgres + Auth: Google/Facebook/Apple/Anonymous) hoặc Firebase.
- **Hạ tầng:** 1 VPS nhỏ, đặt gần Việt Nam (Singapore hoặc Việt Nam). SSL bắt buộc.
- **Chốt phiên bản** mọi thư viện trong `package.json` (không dùng `latest`), vì AI hay dùng API cũ/sai.

### 6.2 Cấu trúc thư mục
```
hit-me/
  apps/
    client/        # Phaser + Vite + Capacitor
    server/        # Colyseus rooms, matchmaking, bot
  packages/
    game-core/     # Luật game thuần túy, dùng chung client + server (có unit test)
    shared-config/ # Mọi hằng số/cân bằng (mục 8)
  docs/
    HitMe_Spec_for_Antigravity.md   # file này
```

### 6.3 Nguyên tắc bắt buộc
1. **Server là nguồn quyết định (authoritative).** Client chỉ gửi ý định (vị trí, góc ném); server tính mọi kết quả.
2. **Giữ kín thông tin.** Server **không được** gửi hành động của người chơi A cho người chơi B trước pha RESOLVE. Nếu gửi sớm thì có thể đọc lén được (gian lận). Kiểm tra đặc biệt mục này.
3. **Logic game nằm trong `packages/game-core`**: hàm thuần túy, không phụ thuộc Phaser hay Colyseus, có seeded RNG, có unit test. Client dùng cùng code để phát lại animation.
4. **Mọi con số cân bằng nằm trong `shared-config`**, không hard-code.
5. **Validate đầu vào ở server:** vị trí trong đấu trường, góc hợp lệ, một lần gửi mỗi vòng, chỉ trước deadline.
6. **Đồng hồ do server giữ.** Client chỉ hiển thị đếm ngược; hành động đến sau deadline bị bỏ.

### 6.4 Thuật toán xử lý hit (tham chiếu cho `game-core`)
```
resolveRound(players, actions, config):
  hits = []
  for each A in alive players with valid action:
    dir = (cos(A.angle), sin(A.angle))
    candidates = []
    for each B in alive players, B != A:
      t = dot(B.pos - A.pos, dir)            # khoảng cách chiếu dọc đường bay
      if t <= 0: continue                    # phía sau người ném
      d = |cross(B.pos - A.pos, dir)|        # khoảng cách vuông góc tới đường bay
      if d <= PLAYER_RADIUS + PROJECTILE_RADIUS
         and t <= distanceToWall(A.pos, dir):
        candidates.push({B, t})
    sort candidates by t ascending
    for first MAX_PIERCE_TARGETS in candidates: hits.push({A -> B})
  apply damage to all targets simultaneously
  mark players with hp <= 0 as eliminated
  return { hits, hpAfter, eliminated }
```

### 6.5 Giao thức tin nhắn (đề xuất)
- Client → Server: `join_queue {mode}`, `submit_action {x, y, angle}`, `chat {text|presetId}`, `friend_request {userId}`, `report {userId, reason}`.
- Server → Client:
  - `round_start {roundNo, deadlineTs, arenaRadius}`
  - `round_result {revealedPositions, throws, hits, hpAfter, eliminated}` (**chỉ gửi khi hết giờ**)
  - `match_end {placements, goldDelta, rankDelta}`
  - `player_disconnected {playerId}`, `player_reconnected {playerId}`
  - `banned {until}` khi cố vào hàng chờ trong thời gian cấm

### 6.6 Mô hình dữ liệu (tối thiểu)
- `profiles(user_id, display_name, name_change_count, gender, hair_id, face_id, outfit_id, rank_points, gold, created_at)`
- `weapons(id, name_vi, name_en, tier, price, vfx_throw_id, vfx_hit_id, sfx_hit_id)` (danh mục)
- `inventory(user_id, weapon_id, equipped)`
- `friends(user_id, friend_id, status)`
- `matches(id, started_at, ended_at, mode, player_count)`
- `match_players(match_id, user_id, placement, hits_dealt, hits_taken, gold_earned, rank_delta, is_bot)`
- `bans(user_id, banned_until, reason)`

## 7. Bot [ĐỀ XUẤT]
- Bot chọn vị trí và hướng bằng heuristic: ngẫu nhiên có trọng số, tránh vị trí vòng trước của người thật nếu chúng "hay đứng yên", có độ khó (dễ/thường/khó) chỉnh bằng tham số.
- Bot chạy **trong server** (cùng giao diện hành động như người chơi) để dễ lấp chỗ trống khi ghép trận.

## 8. Config mặc định (`shared-config`)

```ts
export const CONFIG = {
  ARENA_RADIUS: 1000,
  PLAYER_RADIUS: 30,
  PROJECTILE_RADIUS: 10,
  MAX_HP: 3,
  MIN_PLAYERS: 2,
  MAX_PLAYERS: 6,
  PLACEMENT_TIME_MS: 5000,
  MAX_PIERCE_TARGETS: 1,          // [CẦN XÁC NHẬN]
  AFK_MISSED_ROUNDS: 2,           // [ĐỀ XUẤT]
  AFK_BAN_MS: 120000,
  RECONNECT_GRACE_MS: 30000,      // [ĐỀ XUẤT]
  SUDDEN_DEATH_ROUND: 12,         // [ĐỀ XUẤT]
  SHRINK_PER_ROUND: 0.08,         // [ĐỀ XUẤT]
  MATCHMAKING_TIMEOUT_S: 15,      // [ĐỀ XUẤT]
  GOLD_REWARD_TOP3: 199,
  MIN_PLAYERS_FOR_TOP3_REWARD: 4, // [ĐỀ XUẤT]
  BOT_GOLD_FACTOR: 0.5,           // [ĐỀ XUẤT]
  BASE_PRICE: 199,                // [CẦN XÁC NHẬN]
  PRICE_MULTIPLIER: 1.5,          // [CẦN XÁC NHẬN]
  NAME_CHANGE_FREE_COUNT: 1,
  NAME_CHANGE_COST: 99,           // [CẦN XÁC NHẬN]
};
```

## 9. Lộ trình và tiêu chí nghiệm thu

| Mốc | Nội dung | Nghiệm thu |
|---|---|---|
| M0 | Dựng monorepo, `game-core` + unit test cho `resolveRound` | Test phủ các ca: trúng 1 người, không xuyên (MAX_PIERCE=1), nhiều người cùng bắn một người, cùng chết một vòng, ném ra ngoài không trúng ai |
| M1 | **Prototype offline đấu bot** (web, 2D): đặt vị trí + kéo thả hướng, 5 giây, animation ném | Chơi trọn 1 trận vs 1–5 bot trên trình duyệt mobile; **playtest xem có vui không** |
| M2 | Multiplayer 2–6 người, đăng nhập khách (Quest), kết nối lại | 3 thiết bị chơi cùng một phòng; client không nhận được hành động người khác trước RESOLVE; AFK và rớt mạng đúng luật |
| M3 | Tài khoản Google/Facebook/Apple, tạo nhân vật (layer), vàng, store, rank | Mua trang bị trừ đúng vàng, giá theo công thức, rank cập nhật sau trận |
| M4 | Chat (lọc từ, báo cáo, chặn), kết bạn sau trận, âm thanh, i18n Việt/Anh | Chuyển ngôn ngữ không sót chuỗi cứng; có SFX khi trúng |
| M5 | Đóng gói Capacitor, closed beta (TestFlight / Google Play internal testing) | Chạy mượt trên máy mobile tầm trung |
| M6 | IAP, thủ tục pháp lý (giấy phép game online, dữ liệu cá nhân), ra mắt | Theo yêu cầu store và quy định hiện hành tại Việt Nam (**cần tra cứu, không mặc định**) |

## 10. Quy tắc làm việc cho agent
1. Làm **từng mốc một**; mỗi lần chỉ giao một tính năng nhỏ, chạy test trước khi sang bước tiếp.
2. Không đổi luật chơi ở mục 2–3 nếu chưa hỏi chủ dự án; nếu thấy mâu thuẫn hoặc lỗ hổng, **ghi vào danh sách câu hỏi** thay vì tự quyết.
3. Mọi thay đổi cân bằng chỉ sửa trong `shared-config`.
4. Chuỗi hiển thị dùng khóa i18n, không hard-code tiếng Việt/Anh trong code.
5. Các bước cần làm thủ công (build iOS cần Mac/Xcode, tài khoản developer, nộp store, cấu hình đăng nhập Facebook/Apple) thì **liệt kê rõ các bước** cho chủ dự án thay vì giả vờ tự làm được.
6. Asset (art, nhạc, SFX): dùng placeholder (hình khối, âm thanh tạm) cho đến khi có asset thật; giữ tên file và kích thước nhất quán để thay thế dễ.

## 11. Rủi ro cần theo dõi
1. **Độ vui của cơ chế đoán:** tất cả cùng đoán và cùng ném có thể thành may rủi. Prototype M1 phải chứng minh game vui; nếu không, cân nhắc thêm yếu tố kỹ năng (ví dụ lộ một phần thông tin vòng trước, di chuyển giữa các vòng, đường bay vũ khí khác nhau).
2. **Art tùy biến nhân vật** tốn công nhất; bắt đầu bằng ít lựa chọn (ví dụ 3 tóc, 3 mặt, 4 trang phục).
3. **Kinh tế vàng** dễ lệch (cày bot, hệ số giá quá cao); cần theo dõi số liệu sau beta.
4. **Chat giữa người lạ** kéo theo kiểm duyệt và rủi ro bị store từ chối.
5. **Pháp lý Việt Nam** (giấy phép game online, xác thực người chơi, dữ liệu cá nhân): kiểm tra trước khi ra mắt công khai.

## 12. Danh sách câu hỏi còn mở
1. "Chỉ xuyên qua một người": mỗi vũ khí trúng tối đa 1 người (mặc định) hay xuyên qua người đầu rồi trúng người thứ hai?
2. "Gấp 1x lần": hệ số cụ thể (1.2? 1.5? 1.8?) và giá món đầu tiên.
3. 199 vàng: cho mọi người top 3 hay chia theo hạng? Trận 2–3 người có được thưởng không?
4. Giá đổi tên lần thứ hai.
5. Điều kiện AFK: bỏ lỡ bao nhiêu vòng liên tiếp thì coi là AFK?
6. Có chấp nhận cơ chế thu nhỏ đấu trường (sudden death) để tránh trận kéo dài không?
