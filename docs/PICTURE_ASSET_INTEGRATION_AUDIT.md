# Picture Asset Integration Audit

Audit thực tế ngày 09/10/2026. Sáu file gốc được xem trực tiếp; không sửa nguồn.

| Source | Size | Alpha range | Classification | SHA256 |
|---|---|---|---|---|
| Concept characters 2.png | 1536×1024 | RGB opaque | Concept-only; chỉ crop vùng sạch | b2837e15bdd58d133ff395821ef254c113f399bbd118ea44bd144e4ca7301ed4 |
| Concept characters.png | 1536×1024 | RGB opaque | Concept-only; chỉ crop vùng sạch | 1df2dc6ee8b270dfd42ebd1ec1ccd79018fa6fe18ffc43cf28cc095ff901450f |
| MAIN MENU.png | 941×1672 | RGB opaque | Concept-only; chỉ crop vùng sạch | 84baaac099e8ef6076f345ab30f7f171a67c3242845e11a54e4966b2da5a2798 |
| Play in game.png | 967×1627 | RGB opaque | Concept-only; chỉ crop vùng sạch | d2eebdf4317b335cd7e5ee8d30e2c5b66c9f162ece6cbbc8216ed0e4f80a06cf |
| UI CONCEPT.png | 1536×1024 | (19, 252) | Concept-only; chỉ crop vùng sạch | 97ba21d9b28ba189f0f8ee610412a4486002c2ba9549240f0a17268760e11de0 |
| UI KIT.png | 1536×1024 | (167, 251) | Concept-only; chỉ crop vùng sạch | 85ff048083f3b43b16587b96bbb7ba71e9e3c4aa4534079e4a8b71f57f981fe7 |

UI KIT/UI CONCEPT có kênh alpha nhưng không có pixel alpha=0. Checker/paper không phải transparency thật. Không dùng các board làm sprite sheet hay giả frame animation.

## Artwork đang tích hợp

- Hai nền AI chỉnh từ MAIN MENU/Play in game: đã bỏ UI/chữ/nhân vật sân; phân loại Usable with limitations, cần duyệt art.
- Sáu texture UI không chữ được AI tạo theo mẫu, crop từng ô riêng; có alpha=0..255 thật.
- Logo tách bằng AI, alpha thật, không phải crop nguyên bản.
- Bốn Feature Card crop sạch, giữ alpha nguồn; minh họa UI có nền, không phải character cutout.
- Stone tile crop sân từ Play in game, không chứa HUD/nhân vật.
- Pink static PNG alpha thật do AI tách/chỉnh, chưa phải rig hoặc animation; imported preview only.

## Missing / Concept-only

- Sáu nhân vật/động vật bổ sung và animation frames thật; chó/mèo chưa có cutout production.
- Phone, mosquito racket, pickleball racket, beer, TV, fridge: chưa có sprite bay/cầm sạch được duyệt.
- Ba sprite nhân vật + dép/chảo/vợt hiện tại tiếp tục được giữ; motion procedural hiện có vẫn độc lập combat.
- Icon set native hiện tại còn fallback; không gọi các icon này là concept artwork đã chuyển hoàn chỉnh.