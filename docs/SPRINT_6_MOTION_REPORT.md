# HIT ME — Sprint 6 motion status

09/10/2026, Asia/Bangkok. **NOT RUN / blocked by Gate 2 artwork.** Báo cáo này ghi dependency, không phải bàn giao motion đã triển khai.

| Phần | Có thật từ sprint trước | Thiếu để triển khai/ nghiệm thu |
|---|---|---|
| Human motion | CharacterVisual một Idle sprite + pose tween phản ánh gameplay | Bộ nhân vật mới, frames/layers/profiles theo concept; chưa nhận là animation đầy đủ |
| Chó Vàng/Mèo Mun | Không có production PNG riêng | Sprite/layers/frames cho chạy/đi/tail/jump/aim/throw/hit/victory |
| Weapons | PNG riêng dép/chảo/vợt; visual hiện có | Điện thoại/vợt mới/TV/tủ lạnh, spin/trail/impact profiles và VFX exports |
| Environment | Backdrop/sand Làng quê tổng hợp | Cờ/lá/đèn/khán giả/VFX riêng; không thể làm 3 motion loại thật từ board |
| UI | MenuInteraction press/recover nhẹ đã có; UI Kit states | Popup/ready/result/HP/timer motion system và Reduced Motion chưa triển khai Sprint 6 |
| Performance | Web có counter; chưa benchmark thiết bị | FPS/memory thực tế iPhone/Android, pooling/overdraw/background QA |

Không thay gameplay resolver/hitbox/bot/network, không chuyển logical root theo animation. Không tạo frame giả từ concept. Không đổi catalog/weapon rules để minh họa những vật phẩm chưa có asset.

Validation mới thuộc UI Kit (77 EditMode/21 PlayMode/16 backend và Web Build), không báo là Sprint 6 tests/build PASS. Xem MAIN_MENU_PHASE_2_REPORT.md để biết exports cần bổ sung. Giữ đúng thứ tự: Gate 2 đạt → motion trên asset hợp lệ → tests/Web Build/browser/device QA → Gate 6.
