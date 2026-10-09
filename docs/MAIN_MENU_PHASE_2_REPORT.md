# HIT ME — Phase 2 artwork preflight

09/10/2026, Asia/Bangkok. Gate 1 và UI Kit core đã xác minh trước khi kiểm tra điều kiện Phase 2. **Phase 2: BLOCKED — thiếu production artwork. Không nghiệm thu Gate 2.**

## Implemented / artwork có thật

UI Kit đã tích hợp các sprite PNG native fallback riêng hiện có vào 16 prefab; MainMenu/Lobby dùng button prefab thật. Có 27 PNG runtime đã tích hợp từ các sprint trước: 3 nhân vật một frame Idle, 3 vũ khí, 2 ảnh arena Làng quê, 19 UI sprites minimal fallback. Không ghi nhận chúng là artwork mới hoàn chỉnh khớp toàn bộ concept.

| Nguồn | Trạng thái thực tế |
|---|---|
| Char01/Char02/Char03 | Sprite riêng, pivot chân, atlas; một frame Idle, motion code fallback đã có |
| Dép tổ ong/chảo/vợt hiện có | PNG riêng đã dùng; vợt hiện có không tự coi là hai loại vợt muỗi/pickleball mới |
| Làng quê backdrop/sand | Arena art thật hiện có, nền tổng hợp; chưa tách cờ/cây/khán giả |
| UI paper/brick/ink/card/icons/logo/shadow | PNG native fallback riêng, 9-slice/atlas; không phải bộ skin thương mại theo UI CONCEPT |
| 5 ảnh Picture | reference_composite/reference_only, không import runtime; toàn bộ hash giữ nguyên |

Không crop concept thành sprite sheet, không lấy chữ/nút baked-in làm UI, không sinh nhân vật bằng primitive hoặc đoán frame animation. Không có asset sản xuất mới trong Picture để import.

## Missing Assets — đầu vào để gỡ blocker

1. UI skins/buttons/panels/cards/HUD/icon/portrait riêng theo concept, PNG không chữ; sprites có alpha/border phù hợp. Logo riêng hiện có là fallback, cần export art cuối nếu muốn thay.
2. Main Menu và arena background/layers riêng không chứa UI baked-in. Cờ, lá/canopy, đèn lồng, khán giả, foreground/VFX cần PNG riêng để chuyển động thật.
3. Bộ 5 nhân vật người mới và Chó Vàng/Mèo Mun, PNG alpha riêng, portrait; frame thật Idle/Aim/Throw/Hit/Eliminated/Victory hoặc layers rig. Chó/mèo cần run/walk/tail/jump phù hợp, không suy ra từ board.
4. Điện thoại, vợt muỗi, vợt pickleball, TV, tủ lạnh và các weapon variants cần PNG riêng; không tự thay đổi catalog gameplay, damage, hitbox hoặc quyền sở hữu khi có art.
5. Nguồn/license rõ cho exports; PPU hiện tại 100, foot pivot từng nhân vật cần metadata/đối chiếu chân thật. Không yêu cầu mọi sprite có cùng pixel dimensions.

Manifest chi tiết: PICTURE_ASSET_AUDIT.json, HITME_UI_ASSET_MANIFEST.md, UIKIT_COMPONENT_MANIFEST.json. Kiểm tra hash lại: PICTURE_RECHECK_20261009.json.

## Tested / giới hạn

Validation code cuối của UI Kit: EditMode 77/77, PlayMode 21/21, backend 16/16. Build Succeeded, 0 errors/5 warnings, 21,431,101 bytes; HTTP files 200 và browser MainMenu/Lobby smoke thực tế. Existing sprite/pivot/atlas, localization/safe-area/gameplay/network regressions có trong suites. Đây là bằng chứng UI Kit và artwork đã có, **không phải Phase 2 production-art PASS**.

Phase 2 dừng tại asset preflight, không có thay đổi runtime tiếp theo nên không chạy lại build chỉ vì viết báo cáo. Chưa có ảnh before/after artwork mới để nghiệm thu. iPhone/Android thật, full Web match Result, FPS/memory: Pending Manual Verification.

## Next gate

Cần PNG sản xuất riêng hoặc source tách layer để tiếp tục art integration. Sprint 6 chưa bắt đầu vì Gate Artwork chưa đạt. Không tạo fake animal animation hoặc giả vờ nền tổng hợp đã có 3 loại environment motion.
