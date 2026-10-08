# Sprint 2 — Luật và phạm vi

Prompt 03 ưu tiên hơn spec cũ. Mỗi ném trúng tối đa một mục tiêu đầu tiên theo khoảng cách chiếu t; khoảng cách bằng nhau chính xác thì ID ordinal nhỏ hơn thắng tie. Không nudge, không sudden death. Hit theo bán kính 90 + 30; t > 0 và không vượt tường elip 1000/1750. Snapshot tất cả người sống trước vòng; mọi ném vẫn có hiệu lực dù người ném bị loại trong vòng. Cộng sát thương rồi clamp HP về 0. HP đầu 3. Một người sống: thắng; không ai sống: DRAW; ít nhất hai: vòng tiếp.

Placement 5 giây. Aim/Locked là trạng thái input của từng người. MatchStart → RoundStart → Placement → Reveal → Throw → Resolve → RoundResult → vòng tiếp hoặc MatchResult. Reveal sớm chỉ khi tất cả người sống khóa; timeout giữ hành động đầy đủ. Kết quả được tính ngay khi reveal bằng C# thuần; presentation không quyết định damage. Event ghi một lần mỗi chuyển pha. Reveal 0,3s theo foundation; Throw 0,7s và nghỉ kết quả 0,6s là cấu hình trình bày theo UI Prompt 1, không đổi luật hit.

## Timeout chưa đầy đủ: đề xuất được gắn nhãn

Spec 2.5.1 chỉ ĐỀ XUẤT giữ vị trí cũ, vòng đầu vị trí ngẫu nhiên hợp lệ theo seed, không ném và vẫn chịu đòn. Sprint 2 chưa tự coi đó là luật chính thức. `offline-match-config.json` có `proposedTimeout: true` cho bản thử nghiệm; menu ghi rõ đề xuất. Đặt false để chặn timeout thiếu input (`NeedsTimeoutConfirmation`) thay vì âm thầm phát sinh hành động. Có thể chạy combat/state với mọi hành động đầy đủ mà không cần đề xuất. Không áp dụng phạt AFK online.

## Bot và thông tin

Một người + 1–5 bot, mặc định hai. Seed 2026, PRNG xorshift32 có cùng output trên Editor/Web. Easy chọn đều diện tích elip co bán kính và hướng ngẫu nhiên. Normal chọn hướng tới vị trí đối thủ vòng trước đã công khai. Context chỉ có ID, HP bản thân, vòng/deadline, người sống và lịch sử đã reveal; không giữ reference tới match hoặc input hiện tại. Hard được khai báo nhưng chưa cho bật, không giả vờ có chiến thuật hoàn chỉnh.

## Tương thích Sprint 1

Giữ nguyên PlacementSession và toàn bộ test Sprint 1 byte-for-byte. Đó là preview riêng có nhãn, không phải luật của trận offline mới. Mở Battle trực tiếp vẫn thấy preview; nút CHƠI VỚI BOT bật match thật ngay trong cùng BattleView. Luồng MainMenu → CHƠI VỚI BOT vào match thật trực tiếp. Không sao chép sân/HUD. Kết quả dùng scene Result có sẵn, replay tạo OfflineMatch mới hoàn toàn. Người chết được giữ visual X ở RoundResult, không xuất hiện vòng sau.

Không thêm online, shop, phần thưởng, ranking, auth, customization hoặc animation art thật. Chưa có giới hạn số vòng được chốt nên không ép thắng/hòa sau N vòng; trận có thể dài nếu liên tục ném trượt.
