# HIT ME — SPRINT 4: MULTIPLAYER & REWARD FOUNDATION

Bạn là Senior Unity 6 Developer, Multiplayer Backend Engineer và Game Systems Architect.

## MỤC TIÊU

Phát triển HIT ME thành game multiplayer Web 2–6 người, hỗ trợ phòng riêng, ghép trận, tài khoản, phần thưởng và kho vật phẩm.

Tiếp tục trên Unity Project hiện tại. Không xây lại từ đầu.

## 1. AUDIT TRƯỚC KHI TRIỂN KHAI

- Đọc các tài liệu HIT ME hiện có.
- Kiểm tra Gameplay Core, CombatResolver, BattleState và ArenaGeometry.
- Xác định những phần logic có thể dùng chung giữa Unity và multiplayer server.
- Kiểm tra tình trạng Sprint 3B.
- Kiểm tra cấu hình Supabase nếu đã có.
- Không tự ý ghi đè hệ thống hoặc dữ liệu hiện tại.

Tạo `docs/SPRINT_4_ARCHITECTURE.md`.

## 2. MULTIPLAYER SERVER

Sử dụng Node.js + TypeScript + WebSocket qua WSS.

Máy chủ chịu trách nhiệm:

- Tạo phòng.
- Tham gia phòng.
- Rời phòng.
- Quản lý 2–6 người chơi.
- Ready/Unready.
- Countdown.
- Validate placement.
- Validate aim.
- Lock action.
- Reveal.
- Resolve combat.
- Update HP.
- Match result.
- Reconnect.
- Disconnect timeout.

Server phải authoritative.

Không tin tưởng damage, HP, kết quả hoặc phần thưởng do client tự gửi.

Không thay đổi các quy tắc Gameplay Core đã được chốt.

Nếu gameplay resolver hiện tại viết bằng C#, hãy thiết kế cách duy trì tính tương đương giữa server và client, có bộ test dùng chung các trường hợp đầu vào/đầu ra.

## 3. ROOM SYSTEM

Hỗ trợ:

- Quick Match.
- Create Private Room.
- Join by Room Code.
- 2–6 Players.
- Room Owner.
- Ready Status.
- Start Match.
- Leave Room.

Tạo bot fallback nếu phù hợp với quy tắc phòng.

Mã phòng phải có thời hạn và được kiểm tra phía server.

## 4. GAME MODES

Triển khai trước:

- Classic Survival FFA.
- Private Room Survival.

Thiết kế GameModeDefinition có khả năng mở rộng cho:

- Team Battle.
- Treasure Rush.
- Co-op Boss.
- Chaos Mode.
- Tournament.

Không triển khai đồng thời toàn bộ chế độ khi chưa hoàn thành nền tảng multiplayer.

## 5. ACCOUNT & PLAYER PROGRESSION

Thiết kế:

- Player Profile.
- Display Name.
- Avatar.
- Level.
- XP.
- Coins.
- Inventory.
- Equipped Cosmetics.
- Match History.

Có thể sử dụng Supabase Auth và PostgreSQL nếu cấu hình thực tế đã sẵn sàng.

Nếu chưa có credentials, tạo migration và hướng dẫn cấu hình an toàn.

Không hardcode service-role key trong Unity Web client.

## 6. REWARD SYSTEM

Tạo RewardService trên server.

Ví dụ mức thưởng thử nghiệm:

- Win: 100 coins, 50 XP.
- Lose: 40 coins, 20 XP.
- Draw: 65 coins, 30 XP.

RewardService phải:

- Chỉ thưởng trận hợp lệ.
- Chống thưởng hai lần.
- Ghi transaction log.
- Có idempotency key.
- Giới hạn farm trong phòng riêng.
- Xử lý retry an toàn.
- Không để client tự sửa số dư.

## 7. INVENTORY & COLLECTIBLES

Thiết kế:

- ItemDefinition.
- ItemRarity.
- PlayerInventory.
- CosmeticEquipment.
- MapCollection.
- MaterialWallet.
- CraftingRecipe.

Các cosmetic không được thay đổi damage, HP, hitbox hoặc các thông số chiến đấu xếp hạng.

Hỗ trợ danh mục sưu tầm theo bản đồ Việt Nam.

## 8. DAILY & WEEKLY QUESTS

Tạo hệ thống nhiệm vụ dựa trên sự kiện server đã xác thực:

- MatchCompleted.
- MatchWon.
- ValidHit.
- MapPlayed.
- FriendMatchCompleted.

Mỗi sự kiện phải được xử lý chống trùng lặp.

Nhiệm vụ có trạng thái:

- Active.
- Completed.
- Claimed.
- Expired.

## 9. UNITY UI

Bổ sung các màn hình:

- Main Lobby.
- Quick Match.
- Private Room.
- Match Waiting Room.
- Match Result.
- Player Profile.
- Inventory.
- Daily Quests.

Phong cách UI chibi Việt Nam, mobile portrait, hỗ trợ tiếng Việt và tiếng Anh.

Giữ nguyên các gameplay scene đã có.

## 10. NETWORK RELIABILITY

Bổ sung:

- Heartbeat.
- Reconnection Token.
- Session Resume.
- Duplicate Message Handling.
- Action Sequence Number.
- Server Timestamp.
- Network Error UI.

Không để người chơi tự gửi lại hành động sau khi đã lock.

## 11. TESTS

Bắt buộc kiểm thử:

- 2, 3 và 6 người chơi.
- Tạo và tham gia phòng.
- Ready đồng thời.
- Timeout.
- Disconnect.
- Reconnect.
- First Target Hit.
- Simultaneous Elimination.
- Match Result.
- Reward Granted Once.
- Inventory Persistence.
- Unauthorized Reward Request.
- Duplicate Action.

Chạy Unity EditMode, PlayMode và backend tests.

## 12. DELIVERABLES

- Multiplayer server có thể chạy local.
- Unity Web client kết nối được với server.
- Phòng riêng hoạt động.
- Một trận Classic Survival hoàn chỉnh.
- Kết quả được server xác nhận.
- Phần thưởng được lưu bền vững.
- UI Lobby và Result.
- Database migrations.
- Automated tests.
- Web Build.
- Tài liệu chạy thử.

Tạo `docs/SPRINT_4_MULTIPLAYER_REPORT.md`.

Báo cáo rõ phần đã chạy thực tế, phần mới scaffold và phần còn thiếu.

Không tuyên bố multiplayer hoàn thành nếu chưa kiểm thử được hai Unity Web clients tham gia cùng một trận.

Bắt đầu bằng audit và triển khai theo từng phần có thể kiểm chứng. Không dừng lại ở bản kế hoạch.