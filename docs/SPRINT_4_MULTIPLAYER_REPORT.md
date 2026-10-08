# HIT ME — Sprint 4 Multiplayer & Reward Foundation

Thực thi ngày 08/10/2026 trên Unity 6000.6.4f1 và Node 24.14.0. Triển khai trên project hiện tại; không sửa Phaser `apps/`, `packages/`, Core C# hoặc bot offline. Không gọi đây là bản production.

## Đã triển khai

- Server Node/TypeScript/ws riêng trong `multiplayer-server/`: phòng riêng mã 8 ký tự có hạn 30 phút, Quick Match 2–6 người, owner, ready/unready, countdown 3 giây, leave ở phòng chờ/kết quả, authoritative placement/aim/lock/reveal/throw/resolve/result.
- Core server port từ RoundedRectangleArenaGeometry/CombatRules: sân 2000×3500 R300, player90/projectile30, HP3, hit đầu tiên và tie ordinal ID, sát thương đồng thời, 0 người sống là hòa. Fixture CSV dùng chung được chạy bởi C# và TS.
- Snapshot riêng từng người; không gửi vị trí/hướng đối thủ trong Placement, kể cả khi reconnect. Khóa và deadline do server kiểm tra. Sequence chống action trùng, request ID cache có giới hạn, rate/payload limits, heartbeat ping/pong, server timestamp, guest token hash và resume.
- SQLite/WAL lưu profile, coins/XP, inventory/equipped cosmetics, lịch sử thưởng, ledger match+player, quest events/progress. Retry thưởng không tăng tiền lần nữa; mỗi người phải từng có action hoàn chỉnh trong trận. Private cap thử nghiệm: tối đa 3 trận có thưởng/ngày UTC.
- Win100/50, lose40/20, draw65/30 là **coins/XP thử nghiệm theo Prompt Sprint 4**, chưa thay kinh tế chính thức 199 vàng của spec cũ. Client không có API ghi balance/damage/result/reward.
- Quest daily/weekly từ các event server đã xác minh; dedupe và claim một lần. Claim chỉ ghi trạng thái, **chưa tự đặt phần thưởng nhiệm vụ**. Các period cũ không xuất hiện trong danh sách active; API profile.expiredQuests trả Expired cho period cũ chưa claim; chưa có UI lịch sử nhiệm vụ hết hạn.
- Unity Web transport và Editor transport, sảnh online, Quick Match, Create/Join Private Room, waiting room, server result overlay trong Battle hiện có, profile, inventory/equip và daily/weekly quests. VI/EN. BattleView/CharacterVisual được mở rộng; không có controller nhân vật thứ hai, không tính damage ở client.
- Dùng lại các PNG/atlas đã có; không thêm frame giả hoặc coi concept tổng hợp là sprite sheet. Lobby tận dụng backdrop Làng quê; avatar dùng Idle làm thumbnail vì thiếu portrait riêng.
- Migration Supabase/PostgreSQL dùng schema `hitme_s4` riêng, RLS, client chỉ SELECT dữ liệu của mình. Không ghi đè bảng hiện tại, không hardcode key.

## Kiểm thử thực tế

| Kiểm tra | Kết quả |
|---|---|
| Backend TypeScript build | Pass |
| Backend automated | **16/16 pass**: parity, geometry, 2/3/6 người, privacy, ready, timeout, locked/duplicate actions, reconnect, reward once, inventory persistence, private cap, quest dedupe, unauthorized reward |
| WSS | Handshake + guest auth thực sự chạy, chứng chỉ test CA được xác minh; không tắt certificate validation |
| Hai client WS thật | Chơi 3 vòng đến Draw, mỗi bên65 coins30XP, không lỗi protocol |
| Unity EditMode | **71/71 pass**, gồm toàn bộ test cũ và fixture server/C# |
| Unity PlayMode | **16/16 pass**, gồm toàn bộ test cũ và network presentation/privacy/null snapshot hồi quy |
| Hai Unity Web client thực tế | Hai tab độc lập Player A/B: Private **0345337E** Draw vòng13; Quick **0D6C553D** và **597E0D25** đều Draw vòng3 |
| Server reward thật | SQLite read-only xác nhận **6 ledger entries** qua ba trận, A/B mỗi người195 coins90XP; A equipped chảo |
| Browser UI | Profile/equip/quests đã thao tác; ValidHit3, MatchCompleted1, MapPlayed1, FriendMatchCompleted1 từ trận thật |

Các vòng đầu có timeout trong lúc thao tác test; không ép match result hoặc sửa HP qua browser JS. Hai client dùng native UI click/drag. Snapshot rỗng IL2CPP ban đầu gây chuyển nhầm Battle đã được phát hiện bằng browser console, sửa guard/normalization và thêm hồi quy; lần có lỗi không được dùng làm nghiệm thu cuối.

Web build cuối **Succeeded, 21.310.908 bytes, 0 errors, 5 warnings, 176,16 giây**. Warning có CS0108 (field name che Object.name), TMP shader deprecated và compiler splitting; chưa coi là build sạch warning. Kiểm tra browser sau chỉnh nhãn: xem `SPRINT4_WEB_BUILD.json`, `SPRINT4_BROWSER_VERIFICATION.json`. XML kiểm thử được lưu riêng với tiền tố Sprint4 để không ghi đè bằng chứng Sprint cũ. Log build/test nằm trong docs, bị Git ignore.

## Cách chạy và mở scene

```powershell
cd 'D:/HIT ME/multiplayer-server'
npm ci
npm run build
npm test
npm start
```

Từ root chạy `./tools/build-web.ps1`, rồi `python ./tools/serve-web.py --port 8791`. Mở `http://127.0.0.1:8791/` trong hai tab → SẢNH ONLINE → nhập tên khách → Kết nối `ws://127.0.0.1:8788/play` → Create/Join code → cả hai Ready. Tab dùng sessionStorage riêng để không dùng chung guest token. Reload giữ credential của tab; đóng tab mất credential nếu chưa liên kết tài khoản.

Unity Hub mở `D:/HIT ME/unity-client`; scene `Assets/HitMe/Scenes/Battle.unity` giữ preview/offline, MainMenu→Lobby mở luồng online. Editor transport ClientWebSocket có mã và được biên dịch; **chưa thao tác trận online bằng Editor GUI**. Hai Unity Web client đã được thao tác thật.

WSS triển khai: TLS_CERT/TLS_KEY và origin allowlist chính xác, NODE_ENV=production. Server từ chối production hoặc non-loopback WS. Browser local test dùng WS trên loopback; **chưa có domain/chứng chỉ hợp lệ để chạy hai Unity browser qua public WSS**. Không bypass cảnh báo browser.

## Phần chưa đủ dữ liệu / mới là scaffold

- Không có Supabase credentials/env thực tế ngoài `.env.example` mới. Migration **chưa apply**, chưa có Supabase Auth/PostgreSQL runtime adapter. Google/Facebook/Apple và chuyển guest sang account cần cấu hình provider/credentials cùng migration liên kết dữ liệu được review.
- MaterialWallet/CraftingRecipe/ItemRarity/GameModeDefinition có contract; chưa có recipe, chi phí hoặc gameplay Team/Treasure/Co-op/Chaos/Tournament. Hai mode Classic/Private Survival hoạt động.
- Owner/status/start tự động khi mọi người ready; chưa có phòng moderation, chat online, friend graph, ranking hoặc shop production.
- Leave trong trận đang chơi/AFK forfeiture chưa tự đặt quy tắc. Waiting/result disconnect timeout30 giây được xử lý; fighter đang trong trận vẫn nhận combat bình thường và có thể reconnect. Chưa áp ban AFK hoặc tự trừ HP khi mất kết nối. Cần chốt cách xử lý quá grace trong trận.
- Rooms/match đang chơi nằm trong RAM; restart server giữ account/inventory/reward nhưng **không khôi phục trận đang chơi**. Chưa cluster/Redis/shared room persistence.
- Bot fallback chưa bật do chưa chốt policy cho phòng. No sudden death/nudge/Hard bot.
- Còn thiếu portrait PNG, animation nhiều frame, art5 map, audio/VFX hoàn chỉnh, UI art riêng cho lobby. Không tuyên bố hoàn tất hình ảnh.
- Chưa kiểm tra Safari iOS/Chrome Android máy thật hoặc benchmark tải/concurrency/thermal. Viewport và test inset chỉ là giả lập.

## File và nguồn tham khảo

Danh sách file tạo/sửa được ghi tại `SPRINT4_FILE_INVENTORY.json`. Kiến trúc: `SPRINT_4_ARCHITECTURE.md`; hướng dẫn server: `multiplayer-server/README.md`; yêu cầu gốc: `SPRINT_4_REQUIREMENTS.md`; luật còn mở: `MISSING_REQUIREMENTS.md`.

API đã đối chiếu với [Node SQLite](https://nodejs.org/docs/latest-v24.x/api/sqlite.html), [ws](https://github.com/websockets/ws/blob/master/doc/ws.md), [Supabase RLS](https://supabase.com/docs/guides/database/postgres/row-level-security). Phiên bản local Node24 còn cảnh báo experimental SQLite; không dùng tài liệu Node26 để đổi API đang chạy.

## Nghiệm thu cuối

- Build cuối lúc 16:27 UTC ngày 08/10/2026. Console hai tab sau build cuối không ghi nhận error/warning mới; log cũ có lỗi null snapshot và glyph sao đã sửa, không dùng log cũ làm bằng chứng bản cuối.
- Quick Match cuối: hai Unity Web client chơi ba vòng tới Draw, server thưởng mỗi người65 coins30XP; màn kết quả tách thưởng trận khỏi số dư195 coins90XP. Lịch sử thưởng/profile tồn tại sau restart backend.
- Kết quả ở 360×800, 390×844, 412×915, 430×932 hiển thị đầy đủ, nút quay về sảnh còn truy cập được. Khi resize có một frame cũ trước khi Unity cập nhật; screenshot lưu sau khi ổn định. Settings VI/EN và resize giữ overlay kết quả. Canvas tại390×844 được template giới hạn còn389,398px. Đây là viewport giả lập desktop, không thay test điện thoại.
- Backend16/16, EditMode71/71, PlayMode16/16; PlayMode cuối có hồi quy result overlay sau rebuild safe area. ws8.22.0 được dùng sau audit; production dependency audit0 advisory.
- Ảnh cuối: screenshots/Sprint4-Final-ResultA.png, Sprint4-Final-ResultB.png và Sprint4-Result-{size}.png. Battle thật: screenshots/Sprint4-QuickMatch-Battle.png. Ảnh Room/Quests/Result-ClientA/B cũ là bằng chứng bước kiểm thử trước polish, không phải UI cuối.
