# HIT ME — Bộ âm thanh (Audio Kit) cho Codex và Unity

> **Đọc kèm:** `HitMe_Spec_for_Antigravity.md` (luật chơi), `HitMe_Character_Kit.md` (cùng quy ước manifest/tên file).
> **Đi kèm:** `HitMe_Audio_Placeholders.zip` gồm 20 file WAV, `audio_manifest.json`, `generate_audio.py`, `audio_qa.py`.

---

## 1. Hiện trạng và giới hạn (đọc trước)

- Bộ trong zip là **placeholder tổng hợp bằng code** (sóng sin/nhiễu/mô phỏng nhạc cụ), **không dùng mẫu âm thanh có sẵn** nên không vướng bản quyền bên thứ ba. Mục đích: **ghép được vào game ngay, kiểm tra thời điểm phát, độ to, loop, mixer**.
- **Chất lượng chỉ ở mức tạm.** Mình đo được các thông số kỹ thuật (đỉnh, RMS, vỡ tiếng, độ liền mạch của loop) nhưng **không nghe được** nên không đánh giá được hay hay dở. Bạn cần nghe thử trên điện thoại thật.
- Phần yếu nhất: **tiếng khán giả** (mô phỏng từ nhiễu, nghe như tiếng xì xào mờ) và **nhạc nền** (nghe như nhạc game đơn giản, không có độ "đời" của nhạc thật).
- Bản chính thức nên dùng **nhạc do nhà soạn nhạc làm** hoặc **công cụ AI tạo nhạc có quyền thương mại** (mục 5), thay vào bằng **cùng tên file**, không phải sửa code.

## 2. Danh sách asset hiện có

| File | Sự kiện game (`event`) | Bus | Dài | Ghi chú |
|---|---|---|---|---|
| `sfx_tap_place` | `place_position` | SFX | 0.14s | Chạm đặt vị trí |
| `sfx_countdown_tick` | `countdown_tick` | SFX | 0.09s | Mỗi giây ở giây 5 đến 3 |
| `sfx_countdown_last` | `countdown_last` | SFX | 0.20s | Giây 2 và 1 (cao hơn) |
| `sfx_ready` | `ready_pressed` | UI | 0.80s | Nút SẴN SÀNG |
| `sfx_reveal` | `round_reveal` | SFX | 0.40s | Nhân vật lộ diện |
| `sfx_throw` | `weapon_throw` | SFX | 0.42s | Vũ khí vút bay |
| `sfx_hit_bop` | `player_hit` | SFX | 0.35s | BỐP; có công tắc riêng "SFX khi ném trúng" |
| `sfx_wall_thud` | `weapon_hit_wall` | SFX | 0.25s | Vũ khí chạm tường |
| `sfx_heart_break` | `heart_lost` | SFX | 0.32s | Mất tim |
| `sfx_eliminate` | `player_eliminated` | SFX | 0.90s | Bị loại |
| `sfx_win` | `match_win` | SFX | 1.60s | Fanfare ngắn |
| `sfx_lose` | `match_lose` | SFX | 1.40s | Thua |
| `sfx_coin` | `gold_gain` | UI | 0.70s | Nhận vàng |
| `sfx_purchase` | `purchase_success` | UI | 0.90s | Mua đồ |
| `sfx_ui_click` | `ui_click` | UI | 0.06s | Bấm nút thường |
| `sfx_chat_pop` | `chat_message` | UI | 0.12s | Tin nhắn mới |
| `sfx_error` | `ui_error` | UI | 0.32s | Lỗi/không đủ vàng |
| `bgm_match_loop` | `music_match` | Music | 16.0s, 120 BPM, 8 ô nhịp | Loop, vui nhộn, thang âm ngũ cung |
| `bgm_menu_loop` | `music_menu` | Music | 21.3s, 90 BPM, 8 ô nhịp | Loop, nhẹ |
| `amb_crowd_murmur_loop` | `ambience_crowd` | Ambience | 12.0s | Loop, tạm |

**Thông số đo được (do `audio_qa.py`):** SFX đỉnh −10 đến −1 dBFS; nhạc đỉnh −1 dBFS, RMS −17.6 (trận) và −15.7 (menu) dBFS; ambience RMS −25 dBFS; **không có file nào vỡ tiếng**, loop đều có tỉ lệ nối < 1 (liền mạch). Nhạc menu hơi to hơn nhạc trận khoảng 2 dB, hãy cân bằng bằng mixer nếu cần.

**Còn thiếu so với bản hoàn chỉnh:** nhạc căng thẳng (khi thu nhỏ sân/sudden death), jingle thắng/thua dạng nhạc, tiếng khán giả reo/giật mình theo sự kiện, tiếng bước chân/nhún, SFX theo từng vũ khí (xe đạp, dép tổ ong, cốc bia, chó vàng; giá cao hơn thì hiệu ứng đẹp hơn theo thiết kế).

## 3. Quy chuẩn kỹ thuật (áp cho cả asset thật)

- **Định dạng giao:** WAV PCM. Placeholder là 44.1 kHz/16-bit; **bản master nên giao 48 kHz/24-bit**, Unity sẽ nén lại.
- **Kênh:** SFX **mono**; nhạc và ambience **stereo**.
- **Độ to (tham chiếu, đo bằng LUFS nếu có công cụ):** nhạc nền ≈ **−18 LUFS** tích hợp (placeholder ≈ −16 đến −18 dBFS RMS); SFX đỉnh ≤ −3 dBFS, tiếng quan trọng (trúng đòn, loại) to nhất; ambience ≈ −28 đến −24 dBFS RMS. Chừa dư địa vì người chơi dùng loa điện thoại.
- **Loop liền mạch:** độ dài tròn số ô nhịp; **đuôi vang (reverb/nhạc cụ ngân) phải gấp ngược về đầu** (không cắt cứng); cắt tại điểm cắt zero. Kiểm tra bằng `audio_qa.py` (tỉ lệ nối < 1).
- **Mốc mở đầu SFX:** tiếng phải bắt đầu trong ≤ 30 ms (không im lặng đầu file) để cảm giác tức thì.
- **Tên file:** đúng như bảng ở mục 2, chữ thường, không dấu. Thêm bản biến thể theo hậu tố `_a`, `_b`, `_c` (ví dụ `sfx_hit_bop_a.wav`) để phát ngẫu nhiên, tránh nhàm tai.
- **Giấy phép:** mọi file ghi nguồn vào `assets/LICENSES.md` (nguồn, công cụ, ngày, điều khoản thương mại).

## 4. Tích hợp vào game (nguyên tắc)

1. **Âm thanh chỉ phản ánh kết quả** đã tính bởi `game-core`/server, không quyết định luật. Hết giờ thì server khóa hành động → `round_reveal` → `weapon_throw` đồng thời → `player_hit` và `heart_lost` cho từng cú trúng → `player_eliminated`.
2. **Đếm ngược theo đồng hồ server** (deadline), không dùng đồng hồ máy: tick giây 5 đến 3 dùng `countdown_tick`, giây 2 và 1 dùng `countdown_last`.
3. **Nhiều tiếng trúng cùng lúc:** phát lệch nhau 40 đến 80 ms theo thứ tự trúng để nghe được từng cú, giới hạn tối đa ~6 tiếng đồng thời.
4. **Công tắc cài đặt** (theo tài liệu thiết kế): nhạc và SFX có thanh trượt riêng; công tắc "SFX khi ném trúng người" chỉ tắt `player_hit` (và `heart_lost`), không tắt các SFX khác.
5. **Âm thanh 2D** (không phụ thuộc vị trí). Nếu sau này muốn nghe vị trí, chỉ làm pan theo trục ngang, **không để lộ vị trí đối thủ** ở pha đặt vị trí (luật ẩn thông tin).
6. **Biến thiên:** đổi cao độ ±5% và âm lượng ±1 dB ngẫu nhiên mỗi lần phát cho `weapon_throw`, `player_hit`, `ui_click`.

## 5. Nhạc chính thức: hướng và prompt

**Định hướng chất liệu (theo mockup đấu trường phố Việt Nam):** vui nhộn, lễ hội, hơi hài; nền hiện đại (trống, bass nhẹ) pha **nhạc cụ dân tộc: sáo trúc, đàn tranh, đàn bầu, mõ/song loan, trống nhỏ**; thang âm ngũ cung. Tránh sao chép giai điệu có sẵn (kể cả dân ca có bản thu âm còn bản quyền) và **không yêu cầu "giống bài/nghệ sĩ cụ thể"**.

Bản chính thức cần ít nhất: 1) menu, 2) trận, 3) căng thẳng (sudden death), 4) jingle thắng (3 đến 5 giây), 5) jingle thua, 6) khán giả reo/giật mình/xì xào (nên dùng bản thu thật, có giấy phép).

**Prompt cho công cụ AI tạo nhạc** (tiếng Anh; chọn gói có quyền thương mại và đọc điều khoản; quyền sở hữu với nội dung AI tạo có thể khác nhau theo quốc gia):
```
MATCH LOOP:
Upbeat playful instrumental for a mobile arena party game, 120 BPM, major pentatonic feel,
bamboo flute (sao truc) lead, plucked zither (dan tranh) arpeggios, wood block and small hand drums,
light modern bass and snare, cheerful street-festival mood with a slightly comic touch,
no vocals, no sampled melodies from existing songs, seamless loop, 16 bars, dry mix with short reverb.

MENU LOOP:
Calm warm instrumental, 90 BPM, soft zither arpeggios, gentle flute melody, light shaker,
relaxed Vietnamese festival evening mood, no vocals, seamless loop, 16 bars.

TENSION LOOP:
Same palette as the match loop but 140 BPM, driving percussion, low pulsing bass,
rising tension, flute plays short repeated figures, no vocals, seamless loop, 8 bars.

WIN STINGER (4 s): triumphant fanfare with zither glissando, bright flute and festive drums, ends on a clear major chord.
LOSE STINGER (3 s): comic descending trombone-like slide with a soft wood block, ends unresolved.
```
Sau khi tạo: tách đoạn loop, cắt tại điểm cắt zero, **gấp đuôi về đầu**, chuẩn hóa độ to, chạy `audio_qa.py`. Hoặc thuê nhà soạn nhạc (gửi mockup, bảng sự kiện và prompt trên làm brief; yêu cầu giao thêm **stem** (trống/bass/giai điệu) để có thể tăng/giảm cường độ theo tình huống).

**Nguồn có sẵn (kiểm tra giấy phép thương mại từng file):** thư viện CC0 như Kenney, OpenGameArt, Freesound (lọc CC0); chợ asset trả phí. Ghi lại nguồn và giấy phép vào `assets/LICENSES.md`.

## 6. Unity: import, mixer, code

**Cấu hình import (Editor script tự áp theo `kind` trong manifest):**

| Loại | Load Type | Compression | Khác |
|---|---|---|---|
| SFX < 200 KB | Decompress On Load | ADPCM (hoặc Vorbis chất lượng cao) | **Force To Mono**, Preload Audio Data |
| Nhạc nền | **Streaming** | Vorbis (~70%) | Load In Background, đặt Loop ở AudioSource |
| Ambience | Compressed In Memory | Vorbis (~60%) | Loop |

**AudioMixer:** `Master` → `Music`, `SFX`, `UI`, `Ambience`; mỗi bus mở (expose) tham số `MusicVol`, `SfxVol`, `UiVol`, `AmbVol` (dB). Thanh trượt 0..1 đổi sang dB: `dB = Mathf.Log10(Mathf.Max(v, 0.0001f)) * 20f`. **Ducking:** hạ `Music` khoảng −4 dB trong lúc `round_reveal` đến hết `weapon_throw`, rồi trả về (dùng Snapshot hoặc hai bước chuyển mượt 150 ms). **Chuyển nhạc** menu → trận: crossfade 0.8 giây.

**Di động:** DSP Buffer Size đặt "Best latency"; tạm dừng `AudioListener.pause` khi ứng dụng bị ngắt (cuộc gọi, ra nền); tôn trọng công tắc im lặng/âm lượng hệ thống; thử độ trễ trên máy Android tầm trung.

**Prompt cho Codex (dán nguyên):**
```
Đọc docs/HitMe_Audio_Kit.md (mục 3, 4, 6) và Assets/HitMe/Audio/audio_manifest.json.

1) Editor script Assets/HitMe/Editor/AudioImporter.cs (menu "HitMe/Import Audio"):
   - Đọc audio_manifest.json, áp cấu hình import theo bảng ở mục 6 cho từng WAV (kind sfx/music/ambience).
   - Tạo ScriptableObject HitMeAudioLibrary: danh sách {event, AudioClip[] variants, bus, loop}.
     Biến thể lấy các file cùng tên cơ sở có hậu tố _a, _b, _c.
   - Báo lỗi rõ ràng nếu event nào trong manifest thiếu clip, hoặc clip nào không có trong manifest.
2) AudioManager.cs (singleton, DontDestroyOnLoad):
   - enum AudioEvent sinh từ manifest (hoặc string key); Play(AudioEvent e, float delay = 0f);
     PlayMusic(MusicId id, float fadeSeconds = 0.8f); StopMusic(float fade);
   - Pool 12 AudioSource cho SFX (bus SFX hoặc UI), giới hạn 6 tiếng cùng lúc, tiếng ưu tiên (player_hit, player_eliminated) được cướp nguồn của tiếng yếu nhất.
   - Biến thiên pitch ±5% và volume ±1 dB cho weapon_throw, player_hit, ui_click.
   - Nhạc: hai AudioSource để crossfade; ambience: một AudioSource loop.
   - Ducking nhạc theo Snapshot khi round_reveal → hết weapon_throw.
3) Cài đặt: SettingsAudio.cs đọc/ghi PlayerPrefs, nối thanh trượt nhạc/SFX với các tham số đã expose (dB), công tắc "SFX khi ném trúng người" chỉ tắt player_hit và heart_lost.
4) Kết nối sự kiện game (KHÔNG đổi luật): đặt các điểm gọi Play(...) ở nơi hiển thị kết quả
   (đếm ngược theo deadline của server: giây 5–3 countdown_tick, giây 2–1 countdown_last;
   hit/loại phát lệch nhau 60 ms theo thứ tự trúng).
5) Kiểm thử: scene test có nút phát thử từng event, thanh trượt mixer, và chạy được ở Editor.
Báo cáo: danh sách tài sản đã tạo, cảnh báo, việc phải làm tay. Chạy: python3 audio_qa.py Assets/HitMe/Audio và ghi kết quả vào báo cáo.
```

**Bản web hiện tại (Phaser/Canvas):** dùng cùng bộ file, nén sang `.ogg` (và `.m4a` cho iOS Safari), mở khóa âm thanh bằng cú chạm đầu tiên (trình duyệt yêu cầu), preload qua manifest.

## 7. Quy trình thay placeholder bằng bản thật

1. Giữ **đúng tên file** và `audio_manifest.json`; thêm biến thể `_a/_b/_c` nếu muốn.
2. Chạy `python3 audio_qa.py <thư mục>`; sửa đến khi báo OK (không vỡ tiếng, loop liền mạch, không im lặng đầu SFX).
3. Chạy `HitMe/Import Audio` trong Unity, nghe thử bằng scene test **trên điện thoại thật**.
4. Cân bằng lại mixer (nhạc ≈ −18 LUFS, SFX nổi hơn nhạc 6 đến 10 dB ở các tiếng quan trọng).
5. Ghi nguồn và giấy phép vào `assets/LICENSES.md`.

`generate_audio.py` vẫn dùng được để tinh chỉnh placeholder (đổi nốt, nhịp, âm sắc) và chạy lại, vì mọi tham số đều là hằng số trong file.

## 8. Điều cần bạn quyết định
1. **Phong cách nhạc:** vui nhộn lễ hội phố Việt (như placeholder, tông của mockup) hay hùng tráng kiểu đấu trường La Mã? Mình khuyên vui nhộn để hợp tông hài hước của vũ khí Việt.
2. **Nguồn nhạc chính thức:** AI tạo nhạc, nhà soạn nhạc, hay thư viện có giấy phép?
3. **Có muốn SFX riêng theo từng vũ khí** (hiệu ứng càng đắt càng đẹp) ngay ở bản đầu, hay dùng chung bộ âm thanh trúng đòn?
