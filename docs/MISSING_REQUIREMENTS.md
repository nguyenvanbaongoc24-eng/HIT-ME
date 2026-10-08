# HIT ME — Yêu cầu còn thiếu, cập nhật Sprint 2

Prompt 03 đã chốt: mỗi vũ khí trúng một mục tiêu đầu tiên; tie xác định; 0 người sống là HÒA; reveal khi tất cả người sống khóa hoặc deadline; 2–6 người; một human + bot; Easy/Normal được phép triển khai. Spec 2.3 và 6.4 đã xác định sát thương đồng thời từ snapshot, mỗi hit 1 HP, HP đầu 3.

| Điểm còn mở | Giới hạn hiện tại |
|---|---|
| Timeout thiếu position/aim | Spec 2.5.1 vẫn ghi ĐỀ XUẤT đứng ở vị trí cũ (vòng đầu dùng seed), không ném. Bản thử nghiệm bật `proposedTimeout` và có nhãn ở menu; không gọi là luật chính thức. False chặn timeout thiếu input để chờ xác nhận. |
| Chồng vị trí / nudge | Không thêm khoảng cách tối thiểu hoặc nudge. Hit dùng t > 0 đúng spec; tie cùng khoảng cách dùng ordinal ID. |
| Sudden death / giới hạn số vòng | Không thu nhỏ sân hay ép kết quả. Trận có thể kéo dài nếu liên tục miss. |
| Hard bot | Có enum/điểm mở rộng, không cho bật mức Hard chưa triển khai. |
| Art/audio | Sprint 3A đã có 3 sprite Idle, 3 vũ khí và nền Làng quê. Chưa portrait, animation nhiều frame hoặc audio hoàn chỉnh; xem SPRINT_3A_CHARACTER_REPORT.md. |
| Mobile thật / performance | Test viewport và inset giả lập không thay thế Safari iOS, Chrome Android thật hoặc benchmark FPS/memory. |
| Online privacy / AFK | Ngoài Sprint 2. Offline không truyền network payload; không áp dụng ban AFK. |

Nguồn luật hiện tại: `SPRINT2_RULES.md`. Kết quả thực thi: `SPRINT2_REPORT.md` (khi hoàn tất). Các xác nhận Unity/URP/Web Support của Sprint 1 vẫn có hiệu lực.

---

## Danh sách Sprint 1 lưu để đối chiếu lịch sử

Các điểm đã được Prompt 03 chốt ở trên thay thế trạng thái thiếu tương ứng trong bảng cũ dưới đây.

# HIT ME — Thiếu thông tin và xung đột cần xử lý

Không thiếu ba tài liệu yêu cầu: đã tìm thấy ở root và chép vào docs. Các điểm sau chưa đủ để tự quyết luật hoặc tuyên bố Unity chạy được.

| Vấn đề | Bằng chứng | Cách giới hạn triển khai |
|---|---|---|
| Số mục tiêu mỗi vũ khí | Spec 2.3 và câu hỏi 12.1 còn hỏi trúng tối đa 1 hay xuyên người đầu và trúng người thứ hai | Giữ config TS hiện tại để tham khảo, không coi là xác nhận mới; Unity để resolver sau interface, chưa tự chốt kết quả |
| Cùng chết ở vòng cuối | Spec 2.5 chỉ đề xuất đồng hạng nhất | Không tự gán người thắng hay thứ hạng cho trường hợp 0 người sống |
| Không gửi hành động | Spec 2.5.1 đề xuất đứng yên/không ném; UI Prompt 1 mục 5 mặc định ngắm về tâm | Chưa quyết định timeout không có input là ném hay không; timer/state skeleton không tự phát sinh hành động chưa xác nhận |
| Vị trí chồng nhau | Nudge và khoảng cách tối thiểu trong spec 2.5 là đề xuất | Không tự thêm nudge hoặc cấm đặt chồng |
| Sudden death | Spec 2.5 và mục 12.6 chưa chốt | Không bật thu nhỏ đấu trường mặc định |
| Thời điểm gửi thông tin | Spec 2.2 nói REVEAL; 6.3 nói trước RESOLVE phải giữ kín; UI yêu cầu hiện ở REVEAL | Offline hiển thị sau khóa deadline; protocol production cần xác nhận tên pha cho phép tiết lộ, trước đó không phát payload bí mật |
| Nút sẵn sàng | UI cho khóa hành động sớm; prototype gọi resolve toàn vòng ngay khi bấm | Unity nên chỉ khóa input của mình, giữ deadline của vòng; không kế thừa việc tự kết thúc vòng của prototype |
| Bot | Heuristic/độ khó trong spec 7 là đề xuất | Bot offline dùng policy placeholder có nhãn, chỉ gửi cùng loại input; không đọc input bí mật của người chơi để ngắm |
| Editor/version — đã tìm thấy trong Sprint 1 | Unity 6000.6.4f1 nằm ở D:/App/UNITY, Web Build Support có sẵn; project được tạo bằng Editor | URP 17.6.0 và các package lấy từ template đi kèm Editor. Kết quả import/test/build ghi ở SPRINT1_REPORT, không dùng kết luận audit cũ |
| Art/audio còn thiếu; font đã bổ sung | Manifest frame rỗng, chưa có art/audio thật. Sprint 1 có HitMe Nunito + Noto Sans Symbols 2 theo SIL OFL, kiểm tra cmap/PlayMode | Giữ placeholder có nhãn; font Web được kiểm tra trong báo cáo Sprint 1; art/animation/audio chưa nghiệm thu |

Thông số elip 1000/1750, player radius 90 và projectile radius 30 đã được master prompt hiện tại cho phép làm mặc định cấu hình. Không coi các giá trị đề xuất về kinh tế/AFK trong spec là phạm vi giai đoạn này.

Tiến độ foundation và kết quả Unity thực thi mới nhất: xem SPRINT1_REPORT.md. Vẫn cần trận offline hoàn chỉnh sau khi chốt luật, Safari iOS/Chrome Android trên máy thật, số liệu FPS/texture memory và context loss. Test TS 9/9 trong audit không thay thế kiểm chứng Unity.


## Sprint 3B — phạm vi được cập nhật

Prompt Sprint 3B thay arena elip mặc định bằng chữ nhật bo góc 2000 × 3500, radius 300; player 90/projectile 30 không đổi. Elip còn trong core để rollback. Các luật timeout còn đề xuất, chồng vị trí, sudden death và Hard bot trong bảng trên vẫn chưa được chốt; không tự bổ sung.

- Chỉ Làng quê Bắc Bộ có PNG art riêng từ Sprint 3A; năm bối cảnh khác có lựa chọn và fallback được ghi rõ thiếu art. Nền Làng quê hiện được dùng lại, chưa phải bộ environment mới khớp hoàn toàn concept Sprint 3B.
- HUD dùng thumbnail từ sprite Idle thật; vẫn thiếu portrait PNG riêng, frame animation nhiều tư thế, audio, crowd animation và bộ VFX hoàn chỉnh.
- Chat vẫn là UI mock; không có online/multiplayer, kết bạn, kinh tế, shop hoặc ranking production. Lobby/CharacterSelect chưa có luồng tài khoản/lựa chọn hoàn chỉnh.
- Preset 60/30 FPS là cấu hình mục tiêu. Đo browser desktop không thay cho Safari iOS/Chrome Android thật, notch/Dynamic Island thật hay benchmark nhiệt/memory dài hạn.
- Kết quả thực thi Sprint 3B được ghi riêng tại SPRINT_3B_UNITY_INTEGRATION_REPORT.md; không dùng báo cáo này để khẳng định các mục còn thiếu đã hoàn tất.
